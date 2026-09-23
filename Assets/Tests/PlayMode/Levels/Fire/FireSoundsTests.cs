using System.Linq;
using System.Threading.Tasks;
using Game.Audio;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Game.Levels.Fire.Tests
{
    /// <summary>
    /// Qué suena en el Nivel 1 (Direccion_de_Musica_y_Sonido.md §8 y §11). Se comprueba **qué**
    /// pieza se disparó, no cómo suena: eso es lo que puede romper una «mejora de feedback».
    /// </summary>
    [Category("Integration")]
    public class FireSoundsTests
    {
        // N1_Config: la cercanía efectiva es solo la muesca cinco; desde la dos las piedras se tocan.
        private const int FarSpacing = 0;
        private const int EffectiveSpacing = 5;
        private const int SoftForce = 2;
        private const int EffectiveForce = 7;

        private AudioManager _audio;

        [SetUp]
        public void CrearElGestor() => _audio = new GameObject("TestAudio").AddComponent<AudioManager>();

        [TearDown]
        public void DestruirLosObjetosPersistentes()
        {
            Object.DestroyImmediate(_audio.gameObject);
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(runner.gameObject);
            }

            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }
        }

        /// <summary>
        /// Ningún sonido de fallo (§2.1, CP-02): separadas, las piedras no chocan y no hay nada que
        /// oír; en contacto suenan a piedra prenda o no, y solo el golpe efectivo añade la chispa
        /// que cae en las hojas (RF-16).
        /// </summary>
        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_CP02_SeparadasNoSuenanEnContactoSuenanAPiedraYSoloElEfectivoAnadeLaChispa()
        {
            var controller = await FirePanelTests.LoadPanel();
            var sounds = controller.Sounds;
            Assume.That(sounds, Is.Not.Null, "Level1_Cave tiene N1_Sonidos asignado");
            Assert.That(_audio.AmbientClip, Is.SameAs(sounds.CaveAmbient),
                "la cueva suena desde que abre el nivel (amb_n1_cueva_oscura, RF-14)");

            await FirePanelTests.Reunir(controller);
            controller.ForceSlider.value = EffectiveForce;
            controller.SpacingSlider.value = FarSpacing;
            FirePanelTests.Click(controller.StrikeButton);

            Assert.That(_audio.LastSfx, Is.Not.SameAs(sounds.Strike), "separadas no chocan: nada que oír");

            controller.ForceSlider.value = SoftForce; // en contacto pero floja: no prende
            controller.SpacingSlider.value = EffectiveSpacing;
            FirePanelTests.Click(controller.StrikeButton);

            Assert.That(_audio.LastSfx, Is.SameAs(sounds.Strike),
                "un fallo suena a piedra contra piedra, nunca a pitido ni a caída (§2.1)");

            controller.ForceSlider.value = EffectiveForce;
            FirePanelTests.Click(controller.StrikeButton);

            Assert.That(_audio.LastSfx, Is.SameAs(sounds.Spark), "el golpe efectivo añade la chispa (RF-16)");
        }

        /// <summary>
        /// Guion §4.3.5 E7: al soplar, la llama prende y el crepitar de la hoguera entra por
        /// fundido **sobre** la cueva, que no se va (RF-20, RF-21) — sin golpe inicial (RNF-21).
        /// </summary>
        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF20_SoplarSuenaYLaHogueraEntraSobreLaCueva()
        {
            var controller = await FirePanelTests.LoadPanel();
            var sounds = controller.Sounds;
            Assume.That(sounds, Is.Not.Null, "Level1_Cave tiene N1_Sonidos asignado");
            await FirePanelTests.Reunir(controller);
            controller.ForceSlider.value = EffectiveForce;
            controller.SpacingSlider.value = EffectiveSpacing;
            FirePanelTests.ClickTimes(controller.StrikeButton, 3);

            FirePanelTests.Click(controller.BlowButton);

            Assert.That(_audio.LastSfx, Is.SameAs(sounds.Blow), "el aire de «Soplar»");
            Assert.That(_audio.AmbientLayerClip, Is.SameAs(sounds.FireAmbient), "la hoguera entra al nacer el fuego");
            Assert.That(_audio.AmbientClip, Is.SameAs(sounds.CaveAmbient), "y la cueva sigue debajo");
        }

        /// <summary>
        /// Reunir (INC-47): una hoja suena mientras se arrastra —clic sostenido, RNF-02— y calla al
        /// soltarla; con todo dentro del círculo suena una vez el paso al encendido.
        /// </summary>
        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF14_UnaHojaSuenaMientrasSeArrastraYReunirTodoSuenaAlPasarAlEncendido()
        {
            var controller = await FirePanelTests.LoadPanel();
            var sounds = controller.Sounds;
            Assume.That(sounds, Is.Not.Null, "Level1_Cave tiene N1_Sonidos asignado");
            var hoja = controller.Pieces.First(piece => piece.Kind == PieceKind.Leaf);
            var pointer = new PointerEventData(EventSystem.current);

            ExecuteEvents.Execute(hoja.gameObject, pointer, ExecuteEvents.beginDragHandler);
            Assert.That(_audio.HeldClip, Is.SameAs(sounds.LeafDrag), "la hoja suena mientras se la lleva");

            ExecuteEvents.Execute(hoja.gameObject, pointer, ExecuteEvents.endDragHandler);
            Assert.That(_audio.HeldClip, Is.Null, "y calla al soltarla");

            await FirePanelTests.Reunir(controller);
            Assert.That(_audio.LastSfx, Is.SameAs(sounds.Gathered), "todo reunido: el paso al encendido suena una vez");
        }
    }
}
