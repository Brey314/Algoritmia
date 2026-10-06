using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TestTools;
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

        /// <summary>Cuello, cabeza y cara de la familia (la cara es CaraBase, Ojos y Boca). Algoritm no tiene cuello: su cara va en el cuerpo.</summary>
        private static readonly string[] CabezaDeLaFamilia =
        {
            Tronco + "/Cuello", Tronco + "/Cuello/Cabeza", Tronco + "/Cuello/Cabeza/CaraBase",
            Tronco + "/Cuello/Cabeza/Ojos", Tronco + "/Cuello/Cabeza/Boca",
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
        /// Codos, rodillas, cuello y cabeza con su cara existen en las rutas del plan. El orden de dibujo
        /// del cuello bajo Tronco ya no se comprueba aquí: lo fija, con el de los brazos y el torso,
        /// CharacterRig_INC132_LosBrazosSeDibujanDetrasDelTorsoYDelanteDeLaCabeza (cabeza al fondo, luego los
        /// brazos y el torso delante; INC-132 sustituye lo que fijaba INC-131). Los nodos nuevos se añaden,
        /// nunca se recrean: los fileID de lo que ya existía no cambian (Direccion_de_Arte §13.1).
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
        /// En la familia los brazos se dibujan DETRÁS del torso y DELANTE de la cabeza (Santiago,
        /// 05/10/2026): los hombros salen por detrás del torso y los brazos que suben pasan por delante de
        /// la cara. uGUI pinta los hijos de atrás adelante, así que el orden bajo Tronco es Cuello (con la
        /// cabeza dentro), BrazoIzq, BrazoDer y, al final, Torso. Con el arte provisional de una pieza la
        /// cabeza está pintada dentro del torso, y la regla solo se nota cuando llega el arte final; vale ya
        /// para los cuatro para que todo quede listo. Lo fija el modo «orden» del generador con
        /// SetSiblingIndex, que reordena y no cambia ningún fileID.
        /// </summary>
        [Test]
        public void CharacterRig_INC132_LosBrazosSeDibujanDetrasDelTorsoYDelanteDeLaCabeza(
            [Values("Papa", "Mama", "Nina", "Nino")] string nombre)
        {
            var tronco = Rig(nombre).transform.Find(Tronco);
            Assert.That(tronco, Is.Not.Null, $"{nombre}: existe {Tronco}");

            var cuello = Orden(tronco, nombre, "Cuello");
            var torso = Orden(tronco, nombre, "Torso");
            foreach (var brazo in new[] { "BrazoIzq", "BrazoDer" })
            {
                var indice = Orden(tronco, nombre, brazo);
                Assert.That(indice, Is.GreaterThan(cuello), $"{nombre}: {brazo} se dibuja después del cuello, o la cabeza lo tapa");
                Assert.That(indice, Is.LessThan(torso), $"{nombre}: {brazo} se dibuja antes del torso, para salir por detrás de él");
            }
        }

        /// <summary>
        /// En Algoritm los brazos van delante del cuerpo y la cara ENCIMA de ellos no: son las manos las
        /// que se pintan encima de la cara (Santiago, 05/10/2026, INC-132). El orden bajo Tronco es Torso,
        /// Ojos, Boca, BrazoIzq, BrazoDer: sin esto una mano que sube a la cara quedaría escondida detrás
        /// de ella. Lo fija el modo «orden» del generador.
        /// </summary>
        [Test]
        public void CharacterRig_INC132_AlgoritmPintaLasManosEncimaDeLaCara(
            [Values("Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var tronco = Rig(nombre).transform.Find(Tronco);
            Assert.That(tronco, Is.Not.Null, $"{nombre}: existe {Tronco}");

            var torso = Orden(tronco, nombre, "Torso");
            foreach (var cara in new[] { "Ojos", "Boca" })
            {
                var indiceCara = Orden(tronco, nombre, cara);
                Assert.That(indiceCara, Is.GreaterThan(torso), $"{nombre}: {cara} se dibuja sobre el torso");
                foreach (var brazo in new[] { "BrazoIzq", "BrazoDer" })
                {
                    Assert.That(Orden(tronco, nombre, brazo), Is.GreaterThan(indiceCara), $"{nombre}: {brazo} se dibuja sobre {cara}: la mano tapa la cara, no al revés");
                }
            }
        }

        /// <summary>
        /// Al golpear las piedras las manos chocan delante del pecho, y con los brazos detrás del torso el
        /// choque no se vería (Santiago, 05/10/2026, INC-132): mientras golpea, los brazos pasan delante del
        /// torso, conservando el orden entre ellos, y el cuello —con la cabeza dentro— sigue detrás de ellos.
        /// Se prueba sobre una copia del prefab, nunca sobre el asset.
        /// </summary>
        [Test]
        public void CharacterRig_INC132_AlGolpearLosBrazosPasanDelanteDelTorso(
            [Values("Papa", "Mama", "Nina", "Nino")] string nombre)
        {
            var copia = UnityEngine.Object.Instantiate(Rig(nombre).gameObject);
            try
            {
                var tronco = copia.transform.Find(Tronco);
                Assert.That(tronco, Is.Not.Null, $"{nombre}: existe {Tronco}");

                new ArmLayering(tronco).Apply(true);

                var cuello = Orden(tronco, nombre, "Cuello");
                var torso = Orden(tronco, nombre, "Torso");
                var izquierdo = Orden(tronco, nombre, "BrazoIzq");
                var derecho = Orden(tronco, nombre, "BrazoDer");
                Assert.That(izquierdo, Is.GreaterThan(torso), $"{nombre}: BrazoIzq se dibuja después del torso, para que se vea el choque");
                Assert.That(derecho, Is.GreaterThan(izquierdo), $"{nombre}: entre los brazos se conserva el orden");
                Assert.That(cuello, Is.LessThan(izquierdo), $"{nombre}: el cuello sigue detrás de los brazos");
                Assert.That(cuello, Is.LessThan(derecho));
                Assert.That(cuello, Is.LessThan(torso));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(copia);
            }
        }

        /// <summary>
        /// Lo que pasó delante vuelve a su sitio exacto al terminar el golpe, aunque se pida varias veces
        /// lo mismo: no queda ningún brazo delante del torso para el resto de las acciones.
        /// </summary>
        [Test]
        public void CharacterRig_INC132_AlTerminarElGolpeLosBrazosVuelvenDetrasDelTorso(
            [Values("Papa", "Mama", "Nina", "Nino")] string nombre)
        {
            var copia = UnityEngine.Object.Instantiate(Rig(nombre).gameObject);
            try
            {
                var tronco = copia.transform.Find(Tronco);
                Assert.That(tronco, Is.Not.Null, $"{nombre}: existe {Tronco}");
                var inicial = OrdenDeDibujo(tronco);
                var capas = new ArmLayering(tronco);

                capas.Apply(true);
                capas.Apply(true);
                Assert.That(OrdenDeDibujo(tronco), Is.Not.EqualTo(inicial), $"{nombre}: el golpe sí cambia el orden (si no, la prueba no prueba nada)");

                capas.Apply(false);
                Assert.That(OrdenDeDibujo(tronco), Is.EqualTo(inicial), $"{nombre}: al terminar el golpe vuelve el orden de origen");

                capas.Apply(false);
                Assert.That(OrdenDeDibujo(tronco), Is.EqualTo(inicial), $"{nombre}: devolverlo dos veces no lo cambia");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(copia);
            }
        }

        /// <summary>
        /// En Algoritm los brazos ya van después del torso: golpear no mueve nada, ni al empezar ni al
        /// terminar. Así su cara (Ojos y Boca) tampoco se ve afectada.
        /// </summary>
        [Test]
        public void CharacterRig_INC132_EnAlgoritmElGolpeNoCambiaElOrdenDeDibujo(
            [Values("Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var copia = UnityEngine.Object.Instantiate(Rig(nombre).gameObject);
            try
            {
                var tronco = copia.transform.Find(Tronco);
                Assert.That(tronco, Is.Not.Null, $"{nombre}: existe {Tronco}");
                var inicial = OrdenDeDibujo(tronco);
                var capas = new ArmLayering(tronco);

                capas.Apply(true);
                Assert.That(OrdenDeDibujo(tronco), Is.EqualTo(inicial), $"{nombre}: golpear no cambia el orden");

                capas.Apply(false);
                Assert.That(OrdenDeDibujo(tronco), Is.EqualTo(inicial), $"{nombre}: terminar de golpear tampoco");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(copia);
            }
        }

        /// <summary>
        /// Solo golpear las piedras pone los brazos delante del torso (valor por defecto del campo
        /// armsInFrontActions): el choque de las manos delante del pecho es el único gesto que lo pide.
        /// Los siete prefabs lo traen sin tenerlo serializado, así que mide lo que Unity entrega al
        /// cargarlos. El nombre del campo es un contrato: la herramienta de Python lo lee de CharacterRig.cs.
        /// </summary>
        [Test]
        public void CharacterRig_INC132_SoloElGolpePoneLosBrazosDelante(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var campo = typeof(CharacterRig).GetField("armsInFrontActions", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(campo, Is.Not.Null, "CharacterRig tiene el campo armsInFrontActions");

            var acciones = (ActorAction[])campo.GetValue(Rig(nombre));
            Assert.That(acciones, Is.EqualTo(new[] { ActorAction.Strike }), $"{nombre}: solo Strike pone los brazos delante");
        }

        /// <summary>
        /// Un personaje al que le falta el torso o un brazo, o sin Tronco, no rompe la escena: avisa una
        /// vez y deja la jerarquía como estaba. La acción del guion nunca debe lanzar.
        /// </summary>
        [Test]
        public void CharacterRig_INC132_SiFaltaUnNodoElGolpeAvisaYNoMueveNada()
        {
            var tronco = new GameObject("Tronco", typeof(RectTransform));
            try
            {
                foreach (var hijo in new[] { "Cuello", "BrazoIzq", "BrazoDer" })
                {
                    new GameObject(hijo, typeof(RectTransform)).transform.SetParent(tronco.transform, false);
                }

                var inicial = OrdenDeDibujo(tronco.transform);
                var sinTorso = new ArmLayering(tronco.transform);

                LogAssert.Expect(LogType.Warning, new Regex("ArmLayering"));
                Assert.DoesNotThrow(() => sinTorso.Apply(true), "sin torso no lanza");
                Assert.DoesNotThrow(() => sinTorso.Apply(true), "ni la segunda vez (y no repite el aviso)");
                Assert.DoesNotThrow(() => sinTorso.Apply(false));
                Assert.That(OrdenDeDibujo(tronco.transform), Is.EqualTo(inicial), "sin torso no se mueve nada");

                var sinTronco = new ArmLayering(null);
                LogAssert.Expect(LogType.Warning, new Regex("ArmLayering"));
                Assert.DoesNotThrow(() => sinTronco.Apply(true), "sin Tronco no lanza");
                Assert.DoesNotThrow(() => sinTronco.Apply(false));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(tronco);
            }
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
        /// nuevos; la sombra, la estela y las partes que ya tenían arte no entran. En la familia, la
        /// capa nueva de la cara (CaraBase: nariz y rubor, bajo los ojos y la boca) entra con el mismo
        /// criterio que Ojos y Boca: sin su sprite no se dibuja.
        /// </summary>
        [Test]
        public void CharacterRig_DA131_UnaCapaSinSpriteNoSeDibuja(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);
            var nuevas = new List<string> { "AntebrazoIzq", "AntebrazoDer", "AntepiernaIzq", "AntepiernaDer", "Ojos", "Boca" };
            // Algoritm trae de cero brazos, piernas y torso; en la familia ya tenían su sprite.
            nuevas.AddRange(Familia.Contains(nombre)
                ? new[] { "Cabeza", "CaraBase" }
                : new[] { "BrazoIzq", "BrazoDer", "PiernaIzq", "PiernaDer", "Torso" });

            var capas = rig.GetComponentsInChildren<Image>(true).Where(imagen => nuevas.Contains(imagen.name)).ToArray();
            Assert.That(capas.Select(imagen => imagen.name), Is.EquivalentTo(nuevas), $"{nombre}: cada capa nueva es una Image");
            foreach (var imagen in capas.Where(imagen => imagen.sprite == null))
            {
                Assert.That(imagen.enabled, Is.False, $"{nombre}: {imagen.name} no tiene sprite y no se dibuja");
            }
        }

        /// <summary>
        /// La cara de la familia lleva una capa estática, CaraBase (nariz y rubor, que ni parpadean ni
        /// hablan), y va DETRÁS de Ojos y Boca: uGUI pinta los hijos de atrás adelante, así que es el
        /// primer hijo de Cabeza. El generador la inserta donde la tabla la pone entre sus hermanos
        /// (modo «nodos»); CharacterFace no la gobierna, solo las capas de ojos y boca.
        /// </summary>
        [Test]
        public void CharacterRig_DA131_LaCaraBaseVaDetrasDeLosOjosYDeLaBoca(
            [Values("Papa", "Mama", "Nina", "Nino")] string nombre)
        {
            var rig = Rig(nombre);
            var cabeza = rig.transform.Find(Tronco + "/Cuello/Cabeza");
            Assert.That(cabeza, Is.Not.Null, $"{nombre}: existe Cabeza");

            var caraBase = cabeza.Find("CaraBase");
            Assert.That(caraBase, Is.Not.Null, $"{nombre}: Cabeza tiene CaraBase");
            Assert.That(caraBase.GetSiblingIndex(), Is.Zero, $"{nombre}: CaraBase es el primer hijo de Cabeza");
            foreach (var capa in new[] { "Ojos", "Boca" })
            {
                var nodo = cabeza.Find(capa);
                Assert.That(nodo, Is.Not.Null, $"{nombre}: Cabeza tiene {capa}");
                Assert.That(nodo.GetSiblingIndex(), Is.GreaterThan(caraBase.GetSiblingIndex()), $"{nombre}: {capa} se dibuja sobre CaraBase");
            }

            var serializada = new SerializedObject(rig.GetComponent<CharacterFace>());
            var imagen = caraBase.GetComponent<Image>();
            Assert.That(imagen, Is.Not.Null, $"{nombre}: CaraBase es una Image");
            Assert.That(serializada.FindProperty("eyes").objectReferenceValue, Is.Not.EqualTo(imagen), $"{nombre}: CaraBase no es la capa de ojos");
            Assert.That(serializada.FindProperty("mouth").objectReferenceValue, Is.Not.EqualTo(imagen), $"{nombre}: CaraBase no es la capa de boca");
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

        /// <summary>El lugar de un hijo de Tronco en el orden de dibujo (0 = el más al fondo). Falla con su nombre si no existe.</summary>
        private static int Orden(Transform tronco, string personaje, string hijo)
        {
            var nodo = tronco.Find(hijo);
            Assert.That(nodo, Is.Not.Null, $"{personaje}: {Tronco} tiene {hijo}");
            return nodo.GetSiblingIndex();
        }

        /// <summary>Los nombres de los hijos de Tronco, de atrás adelante, en una sola línea para comparar y para el mensaje.</summary>
        private static string OrdenDeDibujo(Transform tronco)
        {
            var nombres = new List<string>();
            for (var i = 0; i < tronco.childCount; i++)
            {
                nombres.Add(tronco.GetChild(i).name);
            }

            return string.Join(", ", nombres);
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
