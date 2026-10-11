using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding.Tests
{
    public class CharacterFaceTests
    {
        private static readonly string[] SpriteProperties =
        {
            "EyesNeutral", "EyesHappy", "EyesSurprised", "EyesWorried", "EyesFocused", "EyesSleeping",
            "EyesBlinkHalf", "EyesBlinkClosed",
            "MouthClosed", "MouthA", "MouthE", "MouthU",
            "MouthHappy", "MouthSurprised", "MouthWorried", "MouthFocused"
        };

        private readonly List<Object> _creados = new List<Object>();
        private readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

        [TearDown]
        public void Limpiar()
        {
            foreach (var objeto in _creados)
            {
                if (objeto != null)
                {
                    Object.DestroyImmediate(objeto);
                }
            }

            _creados.Clear();
            _sprites.Clear();
        }

        private GameObject Nuevo(string nombre, params System.Type[] componentes)
        {
            var go = new GameObject(nombre, componentes);
            _creados.Add(go);
            return go;
        }

        private Image Capa(string nombre)
        {
            var image = Nuevo(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
            Assert.That(image.enabled, Is.True, "de fábrica una Image está encendida: es lo que la cara tiene que apagar");
            return image;
        }

        private Sprite NuevoSprite(string nombre)
        {
            var textura = new Texture2D(2, 2) { name = nombre };
            var sprite = Sprite.Create(textura, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            sprite.name = nombre;
            _creados.Add(textura);
            _creados.Add(sprite);
            return sprite;
        }

        /// <summary>Un set con un sprite distinto por cada campo, como lo rellenaría el generador del editor.</summary>
        private CharacterFaceSet SetCompleto()
        {
            return SetCon(SpriteProperties);
        }

        /// <summary>
        /// Un set con un sprite distinto solo en los campos pedidos y el resto vacío: así llega el arte,
        /// expresión a expresión, antes de estar completo.
        /// </summary>
        private CharacterFaceSet SetCon(params string[] campos)
        {
            var set = ScriptableObject.CreateInstance<CharacterFaceSet>();
            _creados.Add(set);
            var serializado = new SerializedObject(set);
            foreach (var nombre in campos)
            {
                var sprite = NuevoSprite(nombre);
                _sprites[nombre] = sprite;
                var propiedad = serializado.FindProperty($"<{nombre}>k__BackingField");
                Assert.That(propiedad, Is.Not.Null, $"el campo «{nombre}» del contrato");
                propiedad.objectReferenceValue = sprite;
            }

            serializado.ApplyModifiedPropertiesWithoutUndo();
            return set;
        }

        private CharacterFace Cara(Image ojos, Image boca, CharacterFaceSet set)
        {
            var cara = Nuevo("Cara", typeof(RectTransform)).AddComponent<CharacterFace>();
            cara.Configure(ojos, boca, set, () => 0.5f); // azar fijo: parpadea exactamente cada 3,5 s
            return cara;
        }

        [Test]
        public void CharacterFace_DA73_SinSpritesNoDibujaNada()
        {
            var ojos = Capa("Ojos");
            var boca = Capa("Boca");

            var sinSet = Cara(ojos, boca, null);
            sinSet.Emotion = FacialEmotion.Happy;
            sinSet.Speaking = true;
            sinSet.Step(1f);
            Assert.That(ojos.enabled, Is.False, "sin set, los ojos no se dibujan");
            Assert.That(boca.enabled, Is.False, "sin set, la boca no se dibuja");

            ojos.enabled = true;
            boca.enabled = true;
            var vacio = ScriptableObject.CreateInstance<CharacterFaceSet>();
            _creados.Add(vacio);
            var sinSprites = Cara(ojos, boca, vacio);
            sinSprites.Emotion = FacialEmotion.Surprised;
            sinSprites.Speaking = true;
            sinSprites.Step(1f);
            Assert.That(ojos.enabled, Is.False, "una Image sin sprite pinta un recuadro blanco: se queda apagada");
            Assert.That(boca.enabled, Is.False);
        }

        /// <summary>
        /// El arte llega expresión a expresión (06/10/2026): con solo la neutra entregada, el personaje
        /// perdía la cara entera en las doce acciones que no son Neutral. Una emoción sin su sprite
        /// muestra la neutra, con las dos capas encendidas; el parpadeo, sin cuadros de párpado, sigue
        /// con los ojos de la emoción.
        /// </summary>
        [Test]
        public void CharacterFace_DA73_SinLaExpresionDeLaEmocionUsaLaNeutra()
        {
            var set = SetCon("EyesNeutral", "MouthClosed");

            foreach (var emocion in new[]
                     {
                         FacialEmotion.Happy, FacialEmotion.Focused, FacialEmotion.Surprised, FacialEmotion.Worried,
                         FacialEmotion.Sleeping
                     })
            {
                var ojos = Capa("Ojos");
                var boca = Capa("Boca");
                var cara = Cara(ojos, boca, set);

                cara.Emotion = emocion;

                Assert.That(ojos.enabled, Is.True, $"{emocion}: los ojos siguen encendidos");
                Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesNeutral"]), $"{emocion}: sin sus ojos, los de la neutra");
                Assert.That(boca.enabled, Is.True, $"{emocion}: la boca sigue encendida");
                Assert.That(boca.sprite, Is.SameAs(_sprites["MouthClosed"]), $"{emocion}: sin su boca, la cerrada de la neutra");

                cara.Step(3.6f); // pasa el instante del parpadeo: no hay cuadros de párpado y los ojos no desaparecen
                Assert.That(ojos.enabled, Is.True, $"{emocion}: al parpadear los ojos no se apagan");
                Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesNeutral"]), $"{emocion}: ni cambian");

                cara.Speaking = true;
                cara.Step(0.2f);
                Assert.That(boca.enabled, Is.True, $"{emocion}: sin bocas del habla la boca tampoco se apaga");
                Assert.That(boca.sprite, Is.SameAs(_sprites["MouthClosed"]), $"{emocion}: hablando se queda en la cerrada");
            }
        }

        /// <summary>
        /// El respaldo es por sprite, no por set: una emoción a medias usa lo que tiene y completa con la
        /// neutra. Y sin neutra no se inventa otra expresión: la capa se apaga, como con el set vacío.
        /// </summary>
        [Test]
        public void CharacterFace_DA73_UnaEmocionAMediasCompletaConLaNeutra()
        {
            var ojos = Capa("Ojos");
            var boca = Capa("Boca");
            var cara = Cara(ojos, boca, SetCon("EyesNeutral", "MouthClosed", "EyesHappy"));

            cara.Emotion = FacialEmotion.Happy;
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesHappy"]), "lo que el set sí trae se usa");
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthClosed"]), "lo que falta, la neutra");

            // Desde aquí _sprites guarda los del segundo set (SetCon reemplaza la entrada de cada campo que repite).
            var ojosSinNeutra = Capa("OjosSinNeutra");
            var bocaSinNeutra = Capa("BocaSinNeutra");
            var sinNeutra = Cara(ojosSinNeutra, bocaSinNeutra, SetCon("EyesHappy", "MouthHappy"));
            Assert.That(ojosSinNeutra.enabled, Is.False, "Neutral sin neutra: los ojos no se dibujan");
            Assert.That(bocaSinNeutra.enabled, Is.False, "Neutral sin neutra: la boca no se dibuja");

            sinNeutra.Emotion = FacialEmotion.Surprised;
            Assert.That(ojosSinNeutra.enabled, Is.False, "sin sorpresa ni neutra no se cae a la alegría: se apaga");
            Assert.That(bocaSinNeutra.enabled, Is.False);

            sinNeutra.Emotion = FacialEmotion.Happy;
            Assert.That(ojosSinNeutra.sprite, Is.SameAs(_sprites["EyesHappy"]));
            Assert.That(ojosSinNeutra.enabled, Is.True, "y la emoción que sí tiene arte se sigue viendo");
            Assert.That(bocaSinNeutra.sprite, Is.SameAs(_sprites["MouthHappy"]));
        }

        [Test]
        public void CharacterFace_DA73_ToleraCapasSinAsignar()
        {
            var cara = Cara(null, null, SetCompleto());

            Assert.DoesNotThrow(() =>
            {
                cara.Emotion = FacialEmotion.Focused;
                cara.Speaking = true;
                cara.Step(1f);
            });
        }

        [Test]
        public void CharacterFace_DA73_ConSetMuestraLosOjosDeLaEmocion()
        {
            var ojos = Capa("Ojos");
            var boca = Capa("Boca");
            var cara = Cara(ojos, boca, SetCompleto());

            Assert.That(ojos.enabled, Is.True);
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesNeutral"]));
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthClosed"]), "Neutral descansa con la boca cerrada");

            cara.Emotion = FacialEmotion.Happy;
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesHappy"]));
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthHappy"]));

            cara.Emotion = FacialEmotion.Focused;
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesFocused"]));
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthFocused"]));

            cara.Emotion = FacialEmotion.Neutral;
            cara.Step(3.49f);
            cara.Step(0.02f);
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesBlinkHalf"]), "a los 3,5 s parpadea");
            cara.Step(0.04f);
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesBlinkClosed"]));
            cara.Step(0.1f);
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesNeutral"]), "y vuelve a abrir");
        }

        [Test]
        public void CharacterFace_DA73_DormidoNoParpadea()
        {
            var ojos = Capa("Ojos");
            var cara = Cara(ojos, Capa("Boca"), SetCompleto());

            cara.Emotion = FacialEmotion.Sleeping;
            for (var i = 0; i < 40; i++)
            {
                cara.Step(0.5f);
                Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesSleeping"]), $"a los {(i + 1) * 0.5f} s");
            }
        }

        // ---- INC-134: la cara del cuerpo de perfil; INC-135: parpadeo de dos cuadros ----

        private CharacterFace CaraConPerfil(Image ojos, Image boca, CharacterFaceSet set,
            Image ojosDePerfil, Image bocaDePerfil, CharacterFaceSet setDePerfil)
        {
            var cara = Nuevo("Cara", typeof(RectTransform)).AddComponent<CharacterFace>();
            cara.Configure(ojos, boca, set, ojosDePerfil, bocaDePerfil, setDePerfil, () => 0.5f);
            return cara;
        }

        /// <summary>
        /// Un solo BlinkClock y un solo MouthFlap mueven las dos caras (INC-134): el parpadeo y la boca no
        /// saltan al cambiar de vista. Las dos se actualizan en cada cuadro aunque solo se vea una, y cada
        /// una con los sprites de su propio set.
        /// </summary>
        [Test]
        public void CharacterFace_INC134_ElPerfilParpadeaAlCompasDelFrente()
        {
            var setDeFrente = SetCompleto();
            var delFrente = new Dictionary<string, Sprite>(_sprites);
            var setDePerfil = SetCompleto(); // SetCon reemplaza la entrada de cada campo que repite
            var delPerfil = new Dictionary<string, Sprite>(_sprites);
            Assert.That(delPerfil["EyesNeutral"], Is.Not.SameAs(delFrente["EyesNeutral"]), "cada set lleva sus propios sprites (si no, la prueba no distingue las caras)");
            var ojos = Capa("Ojos");
            var boca = Capa("Boca");
            var ojosDePerfil = Capa("OjosDePerfil");
            var bocaDePerfil = Capa("BocaDePerfil");
            var cara = CaraConPerfil(ojos, boca, setDeFrente, ojosDePerfil, bocaDePerfil, setDePerfil);

            Assert.That(ojos.sprite, Is.SameAs(delFrente["EyesNeutral"]), "el frente abre con sus ojos");
            Assert.That(ojosDePerfil.sprite, Is.SameAs(delPerfil["EyesNeutral"]), "el perfil, con los suyos");
            Assert.That(ojosDePerfil.enabled && bocaDePerfil.enabled, Is.True, "y sus capas están encendidas");

            cara.Step(3.49f);
            Assert.That(ojosDePerfil.sprite, Is.SameAs(delPerfil["EyesNeutral"]), "todavía no toca");

            cara.Step(0.02f);
            Assert.That(ojos.sprite, Is.SameAs(delFrente["EyesBlinkHalf"]), "a los 3,5 s el frente parpadea");
            Assert.That(ojosDePerfil.sprite, Is.SameAs(delPerfil["EyesBlinkHalf"]), "y el perfil parpadea en el mismo cuadro");

            cara.Step(0.04f);
            Assert.That(ojos.sprite, Is.SameAs(delFrente["EyesBlinkClosed"]));
            Assert.That(ojosDePerfil.sprite, Is.SameAs(delPerfil["EyesBlinkClosed"]), "cerrados a la vez");

            cara.Step(0.1f);
            Assert.That(ojos.sprite, Is.SameAs(delFrente["EyesNeutral"]));
            Assert.That(ojosDePerfil.sprite, Is.SameAs(delPerfil["EyesNeutral"]), "y abren a la vez");

            // La boca: un solo aleteo para las dos.
            cara.Speaking = true;
            Assert.That(boca.sprite, Is.SameAs(delFrente["MouthA"]));
            Assert.That(bocaDePerfil.sprite, Is.SameAs(delPerfil["MouthA"]), "las dos abren al empezar a hablar");
            cara.Step(0.1f);
            Assert.That(boca.sprite, Is.SameAs(delFrente["MouthE"]));
            Assert.That(bocaDePerfil.sprite, Is.SameAs(delPerfil["MouthE"]), "y avanzan juntas");
            cara.Speaking = false;
            Assert.That(bocaDePerfil.sprite, Is.SameAs(delPerfil["MouthClosed"]), "al callar vuelven al reposo");

            // La emoción también es de las dos.
            cara.Emotion = FacialEmotion.Happy;
            Assert.That(ojos.sprite, Is.SameAs(delFrente["EyesHappy"]));
            Assert.That(ojosDePerfil.sprite, Is.SameAs(delPerfil["EyesHappy"]));
        }

        /// <summary>
        /// La cara de perfil es opcional y cae a «sin dibujar» por su cuenta, con la misma regla de siempre
        /// (una Image sin sprite pinta un recuadro blanco): sin set o sin capas, la de frente sigue.
        /// </summary>
        [Test]
        public void CharacterFace_INC134_SinCaraDePerfilLaDeFrenteSigueYLaDePerfilNoSeDibuja()
        {
            var ojos = Capa("Ojos");
            var boca = Capa("Boca");
            var ojosDePerfil = Capa("OjosDePerfil");
            var bocaDePerfil = Capa("BocaDePerfil");

            var sinSetDePerfil = CaraConPerfil(ojos, boca, SetCompleto(), ojosDePerfil, bocaDePerfil, null);
            sinSetDePerfil.Emotion = FacialEmotion.Happy;
            sinSetDePerfil.Step(1f);

            Assert.That(ojos.enabled && boca.enabled, Is.True, "la cara de frente no depende de la de perfil");
            Assert.That(ojosDePerfil.enabled, Is.False, "perfil sin set: los ojos no se dibujan");
            Assert.That(bocaDePerfil.enabled, Is.False, "perfil sin set: la boca no se dibuja");

            // Y al revés: sin cara de frente, los tiempos salen del set de perfil y esa cara sí se dibuja.
            var ojosB = Capa("OjosB");
            var bocaB = Capa("BocaB");
            var ojosDePerfilB = Capa("OjosDePerfilB");
            var bocaDePerfilB = Capa("BocaDePerfilB");
            var soloPerfil = CaraConPerfil(ojosB, bocaB, null, ojosDePerfilB, bocaDePerfilB, SetCompleto());
            soloPerfil.Step(0f);
            Assert.That(ojosB.enabled || bocaB.enabled, Is.False, "sin set de frente, las capas de frente no se dibujan");
            Assert.That(ojosDePerfilB.enabled && bocaDePerfilB.enabled, Is.True, "y la de perfil sí");

            // Una cara sin capas de perfil asignadas no falla (es lo que tiene el arte provisional y Algoritm).
            var sinCapas = CaraConPerfil(Capa("OjosC"), Capa("BocaC"), SetCompleto(), null, null, SetCompleto());
            Assert.DoesNotThrow(() =>
            {
                sinCapas.Emotion = FacialEmotion.Focused;
                sinCapas.Speaking = true;
                sinCapas.Step(1f);
            });
        }

        /// <summary>
        /// El arte final trae dos cuadros del parpadeo, abiertos y cerrados (INC-135, 09/10/2026). El reloj
        /// pasa por «medio» en el primer y el último tercio, y sin cuadro medio ese tercio usa el cerrado:
        /// el parpadeo muestra los ojos cerrados sus 0,12 s completos y no los 0,04 s del tercio central.
        /// </summary>
        [Test]
        public void CharacterFaceSet_INC135_SinCuadroMedioElParpadeoUsaElCerrado()
        {
            var dosCuadros = SetCon("EyesNeutral", "EyesBlinkClosed");

            Assert.That(dosCuadros.Eyes(BlinkPhase.Half, FacialEmotion.Neutral), Is.SameAs(_sprites["EyesBlinkClosed"]), "sin medio, el cerrado");
            Assert.That(dosCuadros.Eyes(BlinkPhase.Closed, FacialEmotion.Neutral), Is.SameAs(_sprites["EyesBlinkClosed"]));
            Assert.That(dosCuadros.Eyes(BlinkPhase.Open, FacialEmotion.Neutral), Is.SameAs(_sprites["EyesNeutral"]), "abiertos son los de la emoción");

            var ojos = Capa("Ojos");
            var cara = Cara(ojos, Capa("Boca"), dosCuadros);
            cara.Step(3.49f);
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesNeutral"]), "todavía abiertos");
            cara.Step(0.02f);
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesBlinkClosed"]), "primer tercio: ya cerrados, no a medias");
            cara.Step(0.04f);
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesBlinkClosed"]), "tercio central");
            cara.Step(0.04f);
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesBlinkClosed"]), "último tercio: siguen cerrados los 0,12 s completos");
            cara.Step(0.04f);
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesNeutral"]), "y abren");
        }

        /// <summary>
        /// Con el cuadro medio, sigue siendo el medio (no cambia el arte provisional ni los sets de tres
        /// cuadros); y sin ninguno de los dos no se inventa nada: la cara conserva los ojos de la emoción.
        /// </summary>
        [Test]
        public void CharacterFaceSet_INC135_ConCuadroMedioSigueSiendoElMedioYSinNingunoNoSeInventaNada()
        {
            var tresCuadros = SetCon("EyesNeutral", "EyesBlinkHalf", "EyesBlinkClosed");
            Assert.That(tresCuadros.Eyes(BlinkPhase.Half, FacialEmotion.Neutral), Is.SameAs(_sprites["EyesBlinkHalf"]));

            var sinParpadeo = SetCon("EyesNeutral");
            Assert.That(sinParpadeo.Eyes(BlinkPhase.Half, FacialEmotion.Neutral), Is.Null, "sin párpados, el set no inventa un cuadro");
            Assert.That(sinParpadeo.Eyes(BlinkPhase.Closed, FacialEmotion.Neutral), Is.Null);
        }

        [Test]
        public void CharacterFace_RF05_HablandoLaBocaCambiaYAlCallarVuelveAlReposo()
        {
            var boca = Capa("Boca");
            var cara = Cara(Capa("Ojos"), boca, SetCompleto());
            cara.Emotion = FacialEmotion.Happy;
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthHappy"]));

            cara.Speaking = true;
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthA"]), "abre en cuanto empieza a hablar");
            cara.Step(0.1f);
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthE"]));
            cara.Step(0.09f);
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthU"]));
            cara.Step(0.09f);
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthClosed"]));

            cara.Speaking = false;
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthHappy"]), "al callar vuelve de golpe al reposo de su emoción");
            cara.Step(0.5f);
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthHappy"]), "y ahí se queda");
        }

        [Test]
        public void CharacterRig_DA73_LaEmocionSigueALaAccionSalvoQueUnPasoLaFije()
        {
            var ojos = Capa("Ojos");
            var go = Nuevo("Rig", typeof(RectTransform));
            var rig = go.AddComponent<CharacterRig>();
            var cara = go.AddComponent<CharacterFace>();
            cara.Configure(ojos, Capa("Boca"), SetCompleto(), () => 0.5f);

            // Sin Animator, Play sigue actualizando la acción en curso (Apply fija Current antes de
            // buscar el estado), y la cara la sigue igual.
            Assert.That(rig.Emotion, Is.EqualTo(FacialEmotion.Neutral));

            rig.Play(ActorAction.Celebrate);
            Assert.That(rig.Current, Is.EqualTo(ActorAction.Celebrate));
            Assert.That(rig.Emotion, Is.EqualTo(FacialEmotion.Happy));
            Assert.That(cara.Emotion, Is.EqualTo(FacialEmotion.Happy));
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesHappy"]));

            rig.EmotionOverride = FacialEmotion.Worried;
            Assert.That(rig.Emotion, Is.EqualTo(FacialEmotion.Worried), "el paso del guion manda sobre la acción");
            Assert.That(cara.Emotion, Is.EqualTo(FacialEmotion.Worried));
            Assert.That(ojos.sprite, Is.SameAs(_sprites["EyesWorried"]));

            rig.Play(ActorAction.Strike);
            Assert.That(rig.Emotion, Is.EqualTo(FacialEmotion.Worried), "y se mantiene aunque cambie la acción");

            rig.EmotionOverride = null;
            Assert.That(rig.Emotion, Is.EqualTo(FacialEmotion.Focused), "sin paso, la de la acción en curso");
            Assert.That(cara.Emotion, Is.EqualTo(FacialEmotion.Focused));

            rig.Speaking = true;
            Assert.That(cara.Speaking, Is.True);
            rig.Speaking = false;
            Assert.That(cara.Speaking, Is.False);
        }

        [Test]
        public void CharacterRig_DA73_SinCaraLaEmocionYElHabloSeGuardanSinFallar()
        {
            var rig = Nuevo("Rig", typeof(RectTransform)).AddComponent<CharacterRig>();

            Assert.DoesNotThrow(() =>
            {
                rig.Play(ActorAction.Sleep);
                rig.EmotionOverride = FacialEmotion.Happy;
                rig.Speaking = true;
            });
            Assert.That(rig.Emotion, Is.EqualTo(FacialEmotion.Happy));
            Assert.That(rig.Speaking, Is.True);
        }

        // ---- INC-148 (10/10/2026): el arte final trae una sola boca de hablar, ojos cerrados de frente y ni sueño ni alegría ----

        /// <summary>
        /// El arte final no trae ojos de sueño: dormir es tener los ojos cerrados, y los cerrados del parpadeo
        /// ya están. Sin ojos de sueño, Sleeping usa los del parpadeo y solo después los neutros; con ojos de
        /// sueño, esos mandan (el arte provisional y los sets de antes no cambian). El respaldo no es del
        /// parpadeo: dormido no parpadea y los ojos se quedan cerrados.
        /// </summary>
        [Test]
        public void CharacterFaceSet_INC148_DormidoSinSusOjosUsaLosCerradosDelParpadeo()
        {
            var conSueno = SetCon("EyesNeutral", "EyesSleeping", "EyesBlinkClosed");
            Assert.That(conSueno.Eyes(FacialEmotion.Sleeping), Is.SameAs(_sprites["EyesSleeping"]), "con ojos de sueño, esos mandan");

            var sinSueno = SetCon("EyesNeutral", "EyesBlinkClosed");
            Assert.That(sinSueno.Eyes(FacialEmotion.Sleeping), Is.SameAs(_sprites["EyesBlinkClosed"]),
                "sin ojos de sueño, los cerrados del parpadeo: dormir es tener los ojos cerrados");
            Assert.That(sinSueno.Eyes(FacialEmotion.Neutral), Is.SameAs(_sprites["EyesNeutral"]), "y despierto no cambia nada");

            var soloNeutros = SetCon("EyesNeutral");
            Assert.That(soloNeutros.Eyes(FacialEmotion.Sleeping), Is.SameAs(_sprites["EyesNeutral"]),
                "sin ojos cerrados de ninguna clase, el último respaldo sigue siendo la neutra");

            var vacio = ScriptableObject.CreateInstance<CharacterFaceSet>();
            _creados.Add(vacio);
            Assert.That(vacio.Eyes(FacialEmotion.Sleeping) == null, Is.True, "y sin nada no se inventa un sprite");

            // En la cara: dormido pone los cerrados y no parpadea (un parpadeo encima los abriría un instante).
            var ojos = Capa("Ojos");
            var cara = Cara(ojos, Capa("Boca"), sinSueno);
            var cerrados = sinSueno.Eyes(FacialEmotion.Sleeping);
            cara.Emotion = FacialEmotion.Sleeping;
            for (var i = 0; i < 20; i++)
            {
                cara.Step(0.5f);
                Assert.That(ojos.sprite, Is.SameAs(cerrados), $"a los {(i + 1) * 0.5f} s sigue con los ojos cerrados");
            }
        }

        /// <summary>
        /// Con una sola boca de hablar (la A del arte final) el aleteo alterna esa boca y la cerrada cada
        /// <c>FlapSeconds</c>, sin pasar por las formas E y U que el set no tiene (caerían al reposo y la boca
        /// se vería quieta la mitad del tiempo).
        /// </summary>
        [Test]
        public void CharacterFace_INC148_HablandoConSoloLaBocaAbiertaAlternaCadaFlapSeconds()
        {
            var boca = Capa("Boca");
            var cara = Cara(Capa("Ojos"), boca, SetCon("EyesNeutral", "MouthClosed", "MouthA"));
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthClosed"]), "callado, la cerrada");

            cara.Speaking = true;
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthA"]), "abre en cuanto empieza a hablar");
            cara.Step(0.05f);
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthA"]), "y sigue abierta hasta cumplir FlapSeconds (0,09 s)");
            cara.Step(0.05f);
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthClosed"]), "luego se cierra, sin pasar por E ni U que no existen");
            cara.Step(0.1f);
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthA"]), "y vuelve a abrir");
            cara.Step(0.1f);
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthClosed"]), "alternan, siempre las mismas dos");

            cara.Speaking = false;
            Assert.That(boca.sprite, Is.SameAs(_sprites["MouthClosed"]), "al callar, el reposo");
        }

        /// <summary>
        /// La tarjeta del diálogo es una sola cara que presta el set de quien habla: cambiar el set cambia los
        /// sprites al instante y rehace los relojes con las bocas del set nuevo (el aleteo recomienza con su ciclo,
        /// el parpadeo cuenta desde cero). Asignar el mismo set no toca nada: el parpadeo no salta entre dos líneas
        /// del mismo hablante.
        /// </summary>
        [Test]
        public void CharacterFace_INC148_CambiarDeSetCambiaLaCaraYSusRelojes()
        {
            var primero = SetCon("EyesNeutral", "EyesBlinkClosed", "MouthClosed", "MouthA", "MouthE", "MouthU");
            var delPrimero = new Dictionary<string, Sprite>(_sprites);
            var segundo = SetCon("EyesNeutral", "EyesBlinkClosed", "MouthClosed", "MouthA"); // una sola boca de hablar
            var delSegundo = new Dictionary<string, Sprite>(_sprites);
            Assert.That(delSegundo["EyesNeutral"], Is.Not.SameAs(delPrimero["EyesNeutral"]), "cada set lleva sus propios sprites (si no, la prueba no distingue las caras)");

            var ojos = Capa("Ojos");
            var boca = Capa("Boca");
            var cara = Cara(ojos, boca, primero);
            Assert.That(cara.FaceSet, Is.SameAs(primero));
            Assert.That(ojos.sprite, Is.SameAs(delPrimero["EyesNeutral"]));

            cara.Speaking = true;
            cara.Step(3f);        // casi nada del parpadeo (a los 3,5 s)
            cara.Step(0.1f);
            Assert.That(boca.sprite, Is.SameAs(delPrimero["MouthU"]), "el primer set cicla A, E, U: a los 3,1 s va en la U");

            cara.FaceSet = segundo;

            Assert.That(cara.FaceSet, Is.SameAs(segundo));
            Assert.That(ojos.sprite, Is.SameAs(delSegundo["EyesNeutral"]), "los ojos del set nuevo, al instante");
            Assert.That(boca.sprite, Is.SameAs(delSegundo["MouthA"]), "el aleteo recomienza con la boca del set nuevo");
            cara.Step(0.1f);
            Assert.That(boca.sprite, Is.SameAs(delSegundo["MouthClosed"]), "y con SU ciclo: A y cerrada, no la U que seguía");

            // El parpadeo del set nuevo cuenta desde cero: con el reloj viejo (3,1 s) ya habría parpadeado.
            cara.Step(3.3f);
            Assert.That(ojos.sprite, Is.SameAs(delSegundo["EyesNeutral"]), "3,4 s después del cambio todavía no toca");
            cara.Step(0.15f);
            Assert.That(ojos.sprite, Is.SameAs(delSegundo["EyesBlinkClosed"]), "y a los 3,5 s parpadea, con los ojos cerrados del set nuevo");

            // Asignar el mismo set no reinicia nada.
            cara.Step(0.2f); // abre de nuevo; el siguiente parpadeo está a ~3,5 s
            cara.Step(3.3f);
            cara.FaceSet = segundo;
            cara.Step(0.3f);
            Assert.That(ojos.sprite, Is.SameAs(delSegundo["EyesBlinkClosed"]), "el mismo set no reinicia el reloj: parpadea a su hora");
        }

        // ---- INC-148: la base y la cara de la tarjeta animada del cuadro de diálogo ----

        /// <summary>
        /// La tarjeta cae al retrato fijo en cuanto le falta algo con qué ser animada: la base, el set, los ojos
        /// neutros del set o un recuadro de cara que quepa en la base. Ninguna línea se queda sin retrato.
        /// </summary>
        [Test]
        public void PortraitLook_INC148_SinBaseOSinCaraCaeAlRetratoFijo()
        {
            var set = SetCon("EyesNeutral", "MouthClosed");
            var sinOjos = SetCon("MouthClosed", "MouthA");
            var baseSprite = NuevoSprite("Base");
            var fijo = NuevoSprite("Fijo");
            var cara = new Rect(0.3f, 0.5f, 0.4f, 0.3f);

            var sinBase = PortraitLook.Of(null, cara, set, fijo);
            Assert.That(sinBase.Animated, Is.False, "sin base no hay dónde pintar la cara");
            Assert.That(sinBase.Art, Is.SameAs(fijo), "el retrato fijo de siempre");
            Assert.That(sinBase.Set == null, Is.True, "y no arrastra un set que no se usa");

            var sinSet = PortraitLook.Of(baseSprite, cara, null, fijo);
            Assert.That(sinSet.Animated, Is.False, "una base sin cara con qué pintarla sería un personaje sin cara");
            Assert.That(sinSet.Art, Is.SameAs(fijo));

            var sinOjosNeutros = PortraitLook.Of(baseSprite, cara, sinOjos, fijo);
            Assert.That(sinOjosNeutros.Animated, Is.False, "sin los ojos neutros la cara no se puede dibujar en ninguna emoción");
            Assert.That(sinOjosNeutros.Art, Is.SameAs(fijo));

            var invalidas = new[]
            {
                (new Rect(0.3f, 0.5f, 0f, 0.3f), "sin ancho"),
                (new Rect(0.3f, 0.5f, 0.4f, 0f), "sin alto"),
                (new Rect(0.3f, 0.5f, -0.2f, 0.3f), "de ancho negativo"),
                (new Rect(-0.1f, 0.5f, 0.4f, 0.3f), "se sale por la izquierda"),
                (new Rect(0.3f, -0.1f, 0.4f, 0.3f), "se sale por abajo"),
                (new Rect(0.8f, 0.5f, 0.4f, 0.3f), "se sale por la derecha"),
                (new Rect(0.3f, 0.8f, 0.4f, 0.3f), "se sale por arriba"),
            };
            foreach (var (rect, motivo) in invalidas)
            {
                var look = PortraitLook.Of(baseSprite, rect, set, fijo);
                Assert.That(look.Animated, Is.False, $"una cara {motivo} no cabe en la base");
                Assert.That(look.Art, Is.SameAs(fijo), $"una cara {motivo}: retrato fijo");
            }

            var nada = PortraitLook.Of(null, default, null, null);
            Assert.That(nada.Animated, Is.False);
            Assert.That(nada.Art == null, Is.True, "sin base ni retrato fijo no hay tarjeta: el cuadro de diálogo la oculta");
        }

        [Test]
        public void PortraitLook_INC148_ConBaseYCaraEsAnimado()
        {
            var set = SetCon("EyesNeutral");
            var baseSprite = NuevoSprite("Base");
            var fijo = NuevoSprite("Fijo");
            var cara = new Rect(0.3f, 0.5f, 0.4f, 0.3f);

            var look = PortraitLook.Of(baseSprite, cara, set, fijo);

            Assert.That(look.Animated, Is.True);
            Assert.That(look.Art, Is.SameAs(baseSprite), "la tarjeta es la base sin cara, no el retrato fijo");
            Assert.That(look.Face, Is.EqualTo(cara), "el recuadro de la cara tal como lo declara el personaje");
            Assert.That(look.Set, Is.SameAs(set));

            // La base entera es una cara válida (sin margen) y también la que termina exactamente en el borde.
            Assert.That(PortraitLook.Of(baseSprite, new Rect(0f, 0f, 1f, 1f), set, fijo).Animated, Is.True, "la cara puede llenar la base");
            Assert.That(PortraitLook.Of(baseSprite, new Rect(0.25f, 0.5f, 0.75f, 0.5f), set, null).Animated, Is.True,
                "y puede tocar el borde: el retrato fijo ni se necesita");
        }
    }
}
