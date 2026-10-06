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
    }
}
