using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding.Tests
{
    public class CharacterRigTests
    {
        private const string Carpeta = "Assets/Game/Prefabs/Characters/";

        private static readonly string[] Familia = { "Papa", "Mama", "Nina", "Nino" };
        private static readonly string[] Guia = { "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota" };

        /// <summary>Lo que el guion y §13.3 piden a la familia: todo salvo el trompo del guía.</summary>
        private static readonly ActorAction[] AccionesDeLaFamilia =
            ((ActorAction[])Enum.GetValues(typeof(ActorAction))).Where(accion => accion != ActorAction.Spin).ToArray();

        private static readonly ActorAction[] AccionesDelGuia =
        {
            ActorAction.Idle, ActorAction.Hidden, ActorAction.Talk, ActorAction.Point, ActorAction.Spin,
            ActorAction.Celebrate, ActorAction.Encourage, ActorAction.Appear, ActorAction.Vanish
        };

        // ---- Articulaciones (Plan-Personajes-Finales, Direccion_de_Arte §13.1): rutas desde la raíz del prefab ----

        private const string Cuerpo = "Lienzo/Cuerpo";
        private const string Tronco = Cuerpo + "/Tronco";

        /// <summary>Lo que cuelga de brazos y piernas en la familia y en Algoritm: codo, antebrazo, rodilla y antepierna.</summary>
        private static readonly string[] Extremidades =
        {
            Tronco + "/BrazoIzq/CodoIzq/AntebrazoIzq", Tronco + "/BrazoDer/CodoDer/AntebrazoDer",
            Cuerpo + "/PiernaIzq/RodillaIzq/AntepiernaIzq", Cuerpo + "/PiernaDer/RodillaDer/AntepiernaDer",
        };

        /// <summary>Cuello, cabeza y cara de la familia. Algoritm no tiene cuello: su cara va en el cuerpo.</summary>
        private static readonly string[] CabezaDeLaFamilia =
        {
            Tronco + "/Cuello", Tronco + "/Cuello/Cabeza", Tronco + "/Cuello/Cabeza/Ojos", Tronco + "/Cuello/Cabeza/Boca",
        };

        private static readonly string[] CuerpoDeAlgoritm =
        {
            Cuerpo + "/PiernaIzq", Cuerpo + "/PiernaDer", Tronco, Tronco + "/BrazoIzq", Tronco + "/BrazoDer",
            Tronco + "/Torso", Tronco + "/Ojos", Tronco + "/Boca",
        };

        /// <summary>Los huesos de la familia: cada clip gira todos, y el que no usa uno lo deja en su pose de reposo y no en la pose en T.</summary>
        private static readonly string[] HuesosDeLaFamilia =
        {
            Cuerpo, Tronco, Tronco + "/BrazoIzq", Tronco + "/BrazoDer", Tronco + "/BrazoIzq/CodoIzq", Tronco + "/BrazoDer/CodoDer",
            Cuerpo + "/PiernaIzq", Cuerpo + "/PiernaDer", Cuerpo + "/PiernaIzq/RodillaIzq", Cuerpo + "/PiernaDer/RodillaDer",
            Tronco + "/Cuello", Tronco + "/Cuello/Cabeza",
        };

        /// <summary>Los de Algoritm, que no tiene cuello ni cabeza.</summary>
        private static readonly string[] HuesosDeAlgoritm =
        {
            Cuerpo, Tronco, Tronco + "/BrazoIzq", Tronco + "/BrazoDer", Tronco + "/BrazoIzq/CodoIzq", Tronco + "/BrazoDer/CodoDer",
            Cuerpo + "/PiernaIzq", Cuerpo + "/PiernaDer", Cuerpo + "/PiernaIzq/RodillaIzq", Cuerpo + "/PiernaDer/RodillaDer",
        };

        private const string GiroEnZ = "localEulerAnglesRaw.z";

        [Test]
        public void CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var estados = Estados(Rig(nombre));
            var esperadas = Familia.Contains(nombre) ? AccionesDeLaFamilia : AccionesDelGuia;

            Assert.That(esperadas.Select(accion => accion.ToString()).Except(estados), Is.Empty,
                $"{nombre}: cada acción que puede pedir una escena es un estado de su Animator");
        }

        /// <summary>
        /// Ninguna parte de un personaje recibe clics: pintado encima de una pieza, le robaría el
        /// clic sostenido (RNF-02) y rompería las listas cerradas de cada mecánica.
        /// </summary>
        [Test]
        public void CharacterRig_RNF02_NingunaParteDeUnPersonajeRecibeClics(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);

            Assert.That(rig.GetComponentsInChildren<Graphic>(true).Where(g => g.raycastTarget).Select(g => g.name), Is.Empty);
            Assert.That(rig.GetComponentsInChildren<CanvasGroup>(true).All(grupo => !grupo.blocksRaycasts), Is.True);
        }

        /// <summary>
        /// No existen animaciones de derrota, caída ni salto (CP-02, CT-06, §13.3): tras un
        /// intento sin éxito el personaje da ánimo.
        /// </summary>
        [Test]
        public void CharacterRig_CP02_NingunClipEsDeDerrotaCaidaNiSalto()
        {
            var vetados = new[] { "derrota", "gameover", "caer", "caida", "saltar", "salto", "triste" };
            var clips = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/Game/Art/Characters" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(ruta => vetados.Any(palabra => ruta.ToLowerInvariant().Contains(palabra)))
                .ToArray();

            Assert.That(clips, Is.Empty);
        }

        [Test]
        public void CharacterRig_RF05_CadaPersonajeTieneRetratoYNombresConQueHabla(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);

            Assert.That(rig.Portrait, Is.Not.Null, "su retrato para el cuadro de diálogo");
            Assert.That(rig.SpeakerNames, Is.Not.Empty, "el nombre con que el guion lo hace hablar");
            Assert.That(rig.Stage, Is.Not.Null, "el lienzo que escala para llenar su casilla");
        }

        /// <summary>
        /// Los dos niños hablan a la vez en la 1.3 («NIÑOS»): los dos gesticulan y el retrato sale
        /// de uno de ellos.
        /// </summary>
        [Test]
        public void CharacterRig_RF05_LosDosNinosHablanComoNinos()
        {
            Assert.That(Rig("Nina").Speaks("NIÑOS"), Is.True);
            Assert.That(Rig("Nino").Speaks("niños"), Is.True, "sin distinguir mayúsculas");
            Assert.That(Rig("Papa").Speaks("NIÑOS"), Is.False);
            Assert.That(Rig("Papa").Speaks("PAPÁ"), Is.True);
        }

        /// <summary>Estela de puntos de luz (guion §1.1.1, Dirección de arte §7.6, INC-52).</summary>
        [Test]
        public void CharacterRig_DA76_LasTresFormasDeAlgoritmLlevanEstelaSinRecibirClics(
            [Values("Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            // La estela muestrea el espacio del contenedor de la casilla (GuideTrail, INC-52): sin
            // esos dos niveles de jerarquía por encima del rig, no hay dónde dibujar.
            var contenedorGo = new GameObject("Contenedor", typeof(RectTransform));
            ((RectTransform)contenedorGo.transform).sizeDelta = new Vector2(1000f, 1000f);
            var casillaGo = new GameObject("Casilla", typeof(RectTransform));
            casillaGo.transform.SetParent(contenedorGo.transform, false);
            var casilla = (RectTransform)casillaGo.transform;

            var instancia = UnityEngine.Object.Instantiate(Rig(nombre), casilla);
            try
            {
                var estela = instancia.GetComponentInChildren<GuideTrail>(true);
                Assert.That(estela, Is.Not.Null, $"{nombre} lleva la estela de Algoritm");
                estela.InitializeForTest();

                for (var i = 0; i < TrailPath.MaxPoints + 2; i++)
                {
                    casilla.anchorMin = casilla.anchorMax = new Vector2(0.05f * i, 0.5f);
                    estela.Step(i * 0.05f);
                }

                var puntos = estela.GetComponentsInChildren<Graphic>(true);
                Assert.That(puntos, Is.Not.Empty, "la estela dibuja puntos tras el movimiento");
                Assert.That(puntos.Where(g => g.raycastTarget), Is.Empty, "ningún punto de la estela recibe clics");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(contenedorGo);
            }
        }

        /// <summary>Sombra de contacto bajo los pies de la familia (Dirección de arte §5.3, Interfaces §4, INC-109).</summary>
        [Test]
        public void CharacterRig_DA53_LaFamiliaLlevaSombraDeContactoBajoLosPies(
            [Values("Papa", "Mama", "Nina", "Nino")] string nombre)
        {
            var rig = Rig(nombre);
            var lienzo = rig.Stage;
            var sombra = lienzo.Find("Sombra");

            Assert.That(sombra, Is.Not.Null, $"{nombre} tiene Lienzo/Sombra");
            Assert.That(sombra.GetSiblingIndex(), Is.Zero, "la sombra es el primer hijo: se dibuja detrás del cuerpo");

            var imagen = sombra.GetComponent<Image>();
            Assert.That(imagen, Is.Not.Null, "la sombra es una Image");
            Assert.That(imagen.color, Is.EqualTo(new Color(0f, 0f, 0f, 0.25f)));
            Assert.That(imagen.raycastTarget, Is.False, "la sombra no recibe clics");
        }

        /// <summary>Algoritm flota: no lleva la sombra de contacto de la familia.</summary>
        [Test]
        public void CharacterRig_DA53_AlgoritmNoLlevaSombraDeContacto(
            [Values("Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            Assert.That(Rig(nombre).Stage.Find("Sombra"), Is.Null);
        }

        /// <summary>
        /// Codos, rodillas, cuello y cabeza con su cara existen en las rutas del plan, y el cuello se
        /// dibuja después del torso (la cabeza va encima). Los nodos nuevos se añaden, nunca se
        /// recrean: los fileID de lo que ya existía no cambian (Direccion_de_Arte §13.1).
        /// </summary>
        [Test]
        public void CharacterRig_DA131_LaFamiliaTieneCodosRodillasYCuello(
            [Values("Papa", "Mama", "Nina", "Nino")] string nombre)
        {
            var rig = Rig(nombre);

            foreach (var ruta in Extremidades.Concat(CabezaDeLaFamilia))
            {
                Assert.That(rig.transform.Find(ruta), Is.Not.Null, $"{nombre}: existe {ruta}");
            }

            var tronco = rig.transform.Find(Tronco);
            Assert.That(tronco.Find("Cuello").GetSiblingIndex(), Is.GreaterThan(tronco.Find("Torso").GetSiblingIndex()),
                $"{nombre}: el cuello va después del torso, así la cabeza se dibuja encima");
        }

        /// <summary>
        /// Algoritm gana brazos, piernas, codos y rodillas con las mismas rutas que la familia, y su
        /// cara va en el cuerpo, sobre el tronco: no hay nodo de cuello ni de cabeza.
        /// </summary>
        [Test]
        public void CharacterRig_DA131_AlgoritmTieneCodosYRodillasYCaraSinCuello(
            [Values("Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);

            foreach (var ruta in Extremidades.Concat(CuerpoDeAlgoritm))
            {
                Assert.That(rig.transform.Find(ruta), Is.Not.Null, $"{nombre}: existe {ruta}");
            }

            Assert.That(rig.transform.Find(Tronco + "/Cuello"), Is.Null, $"{nombre}: Algoritm no tiene cuello");
            Assert.That(rig.GetComponentsInChildren<Transform>(true).Select(t => t.name).Intersect(new[] { "Cuello", "Cabeza" }),
                Is.Empty, $"{nombre}: ni cuello ni cabeza en ningún sitio");
            var tronco = rig.transform.Find(Tronco);
            Assert.That(tronco.Find("Ojos").GetSiblingIndex(), Is.GreaterThan(tronco.Find("Torso").GetSiblingIndex()),
                $"{nombre}: los ojos se dibujan sobre el torso");
            Assert.That(tronco.Find("Boca").GetSiblingIndex(), Is.GreaterThan(tronco.Find("Torso").GetSiblingIndex()));
        }

        /// <summary>
        /// Una curva que apunta a un nodo que no existe (o a un componente que ese nodo no tiene) no
        /// falla ni avisa: el Animator la ignora y la parte se queda en pose en T. Hasta ahora nada
        /// recorría los bindings de los clips.
        /// </summary>
        [Test]
        public void CharacterRig_DA131_TodaCurvaApuntaAUnaParteQueExiste(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);
            var colgadas = new List<string>();

            foreach (var clip in Clips(rig))
            {
                var bindings = AnimationUtility.GetCurveBindings(clip).Concat(AnimationUtility.GetObjectReferenceCurveBindings(clip));
                foreach (var binding in bindings)
                {
                    var parte = string.IsNullOrEmpty(binding.path) ? rig.transform : rig.transform.Find(binding.path);
                    if (parte == null)
                    {
                        colgadas.Add($"{clip.name}: no existe «{binding.path}»");
                    }
                    else if (parte.GetComponent(binding.type) == null)
                    {
                        colgadas.Add($"{clip.name}: «{binding.path}» no tiene {binding.type.Name}");
                    }
                }
            }

            Assert.That(colgadas, Is.Empty, $"{nombre}: curvas colgadas");
        }

        /// <summary>
        /// Cada clip gira todos los huesos de su personaje. Con writeDefaultValues, lo que un clip no
        /// anima vuelve al valor del prefab (rotación 0): un brazo sin curva caía a pose en T durante
        /// Talk, Point o Encourage. Un hueso que el clip no usa lleva una curva constante en su reposo.
        /// </summary>
        [Test]
        public void CharacterRig_DA131_CadaClipFijaTodasLasArticulaciones(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);
            var huesos = Familia.Contains(nombre) ? HuesosDeLaFamilia : HuesosDeAlgoritm;
            var sinCurva = new List<string>();

            var clips = Clips(rig).ToArray();
            Assert.That(clips, Is.Not.Empty, $"{nombre} tiene clips");
            foreach (var clip in clips)
            {
                var girados = AnimationUtility.GetCurveBindings(clip)
                    .Where(b => b.type == typeof(RectTransform) && b.propertyName == GiroEnZ)
                    .Select(b => b.path)
                    .ToArray();
                sinCurva.AddRange(huesos.Except(girados).Select(hueso => $"{clip.name}: {hueso}"));
            }

            Assert.That(sinCurva, Is.Empty, $"{nombre}: huesos sin rotación en algún clip");
        }

        /// <summary>
        /// Una Image sin sprite pinta un recuadro blanco: toda capa nueva que aún no tiene arte está
        /// apagada. Se enciende al asignarle el sprite (modo «sprites» del generador). Solo los nodos
        /// nuevos; la sombra, la estela y las partes que ya tenían arte no entran.
        /// </summary>
        [Test]
        public void CharacterRig_DA131_UnaCapaSinSpriteNoSeDibuja(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);
            var nuevas = new List<string> { "AntebrazoIzq", "AntebrazoDer", "AntepiernaIzq", "AntepiernaDer", "Ojos", "Boca" };
            // Algoritm trae de cero brazos, piernas y torso; en la familia ya tenían su sprite.
            nuevas.AddRange(Familia.Contains(nombre) ? new[] { "Cabeza" } : new[] { "BrazoIzq", "BrazoDer", "PiernaIzq", "PiernaDer", "Torso" });

            var capas = rig.GetComponentsInChildren<Image>(true).Where(imagen => nuevas.Contains(imagen.name)).ToArray();
            Assert.That(capas.Select(imagen => imagen.name), Is.EquivalentTo(nuevas), $"{nombre}: cada capa nueva es una Image");
            foreach (var imagen in capas.Where(imagen => imagen.sprite == null))
            {
                Assert.That(imagen.enabled, Is.False, $"{nombre}: {imagen.name} no tiene sprite y no se dibuja");
            }
        }

        /// <summary>
        /// Con las extremidades partidas la pierna se acorta por donde se ve: baja el tronco, abre el
        /// muslo y la pantorrilla se escala en la rodilla; ningún clip estira el muslo. Con el arte
        /// actual (la antepierna sin sprite) rige el truco de siempre y las piernas enteras se escalan:
        /// la misma condición, en el estado en que esté el prefab, impide escalar la parte que no se ve.
        /// </summary>
        [Test]
        public void CharacterRig_DA131_ConExtremidadesPartidasNingunClipEstiraLasPiernas(
            [Values("Papa", "Mama", "Nina", "Nino")] string nombre)
        {
            var rig = Rig(nombre);
            var antepierna = rig.transform.Find(Cuerpo + "/PiernaIzq/RodillaIzq/AntepiernaIzq");
            Assert.That(antepierna, Is.Not.Null, $"{nombre}: tiene antepierna");
            var imagen = antepierna.GetComponent<Image>();
            var partidas = imagen != null && imagen.sprite != null && imagen.enabled;
            var permitidas = partidas
                ? new[] { Cuerpo + "/PiernaIzq/RodillaIzq", Cuerpo + "/PiernaDer/RodillaDer" }
                : new[] { Cuerpo + "/PiernaIzq", Cuerpo + "/PiernaDer" };
            var estiradas = new List<string>();

            foreach (var clip in Clips(rig))
            {
                estiradas.AddRange(AnimationUtility.GetCurveBindings(clip)
                    .Where(b => b.propertyName.StartsWith("m_LocalScale") && b.path.StartsWith(Cuerpo + "/Pierna"))
                    .Where(b => !permitidas.Contains(b.path))
                    .Select(b => $"{clip.name}: {b.path} {b.propertyName}"));
            }

            Assert.That(estiradas, Is.Empty, $"{nombre} ({(partidas ? "partidas" : "arte actual")}): escalas fuera de {string.Join(" y ", permitidas)}");
        }

        /// <summary>
        /// El rig apunta a las capas de ojos y boca de su propio prefab (Plan §5.1, DA §7.3): sin esa
        /// referencia el CharacterFace no tiene qué cambiar y la cara no se mueve.
        /// </summary>
        [Test]
        public void CharacterRig_DA73_LaCaraDelRigApuntaASusCapasDeOjosYBoca(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);
            var cara = rig.GetComponent<CharacterFace>();
            Assert.That(cara, Is.Not.Null, $"{nombre} lleva CharacterFace en la raíz");

            var serializada = new SerializedObject(cara);
            var ojos = rig.GetComponentsInChildren<Image>(true).Single(imagen => imagen.name == "Ojos");
            var boca = rig.GetComponentsInChildren<Image>(true).Single(imagen => imagen.name == "Boca");

            Assert.That(serializada.FindProperty("eyes").objectReferenceValue, Is.EqualTo(ojos), $"{nombre}: eyes es su capa de ojos");
            Assert.That(serializada.FindProperty("mouth").objectReferenceValue, Is.EqualTo(boca), $"{nombre}: mouth es su capa de boca");
        }

        private static CharacterRig Rig(string nombre)
        {
            var rig = AssetDatabase.LoadAssetAtPath<CharacterRig>($"{Carpeta}{nombre}.prefab");
            Assert.That(rig, Is.Not.Null, $"existe el prefab {nombre}");
            return rig;
        }

        /// <summary>Los clips del controlador del personaje (sin repetidos: las tres formas de Algoritm comparten el suyo).</summary>
        private static IEnumerable<AnimationClip> Clips(CharacterRig rig)
        {
            var controller = rig.GetComponent<Animator>().runtimeAnimatorController;
            Assert.That(controller, Is.Not.Null, $"{rig.name} tiene su controlador");
            return controller.animationClips;
        }

        private static IEnumerable<string> Estados(CharacterRig rig)
        {
            var controller = rig.GetComponent<Animator>().runtimeAnimatorController as AnimatorController;
            Assert.That(controller, Is.Not.Null, $"{rig.name} tiene su controlador");
            return controller.layers[0].stateMachine.states.Select(estado => estado.state.name);
        }
    }
}
