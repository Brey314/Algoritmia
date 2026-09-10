using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Core;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    [Category("Integration")]
    public class NarrativeSceneTests
    {
        private const string SceneName = "Narrative";

        [TearDown]
        public void DestruirLosObjetosPersistentes() => LimpiarObjetosPersistentes();

        /// <summary>
        /// Borra los objetos con <c>DontDestroyOnLoad</c>. No basta con hacerlo en el
        /// <c>[TearDown]</c>: sobreviven a <c>LoadSceneMode.Single</c>, así que una prueba que
        /// abre la escena **dos veces** encuentra vivo el runner de la vuelta anterior,
        /// <c>GameFlowRunner.Awake</c> destruye el duplicado (RNF-16) y la prueba se queda con una
        /// referencia muerta cuya FSM nunca salió de <c>Boot</c>.
        /// </summary>
        private static void LimpiarObjetosPersistentes()
        {
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(runner.gameObject);
            }

            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_ResuelveTresSecuenciasDistintasSinRamas()
        {
            var primeras = new string[3];
            var ids = new[] { "N1_Apertura", "N1_AparicionGuia", "N1_Hallazgo" };

            for (var i = 0; i < ids.Length; i++)
            {
                var (controller, _) = await OpenNarrative(ids[i]);
                primeras[i] = controller.BodyLabel.text;
            }

            // La misma escena y el mismo código resolvieron las tres: cada una empieza por su
            // propia línea y ninguna se parece a las otras.
            Assert.That(primeras, Has.All.Not.Empty);
            Assert.That(primeras.Distinct().Count(), Is.EqualTo(3));
        }

        [Test]
        [Timeout(60000)]
        public async Task NarrativeScene_RF05_ResuelveLasSeisSecuenciasDelNivel2SinRamas()
        {
            var ids = new[]
            {
                "N2_PuenteI", "N2_Escena21_Bosque", "N2_Escena22_ElPatron",
                "N2_Escena23_Construccion", "N2_Escena24_Regreso", "N2_Escena25_Cierre"
            };
            var primeras = new string[ids.Length];

            for (var i = 0; i < ids.Length; i++)
            {
                var (controller, _) = await OpenNarrative(ids[i]);
                primeras[i] = controller.BodyLabel.text;

                foreach (var _ in SequenceNamed(controller, ids[i]).Lines)
                {
                    Click(controller.AdvanceButton);
                }

                Assert.That(controller.Dialogue.IsFinished, Is.True, $"{ids[i]} se recorre entera");
            }

            // Seis escenas más, cero ramas: el Nivel 2 no añadió ni un `if` al controlador.
            Assert.That(primeras, Has.All.Not.Empty);
            Assert.That(primeras.Distinct().Count(), Is.EqualTo(ids.Length));
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_LaPrimeraLineaEsLaDelAssetPedido()
        {
            var (controller, _) = await OpenNarrative("N1_Hallazgo");
            var esperada = SequenceNamed(controller, "N1_Hallazgo").Lines[0];

            Assert.That(controller.BodyLabel.text, Is.EqualTo(esperada.Text));
            Assert.That(controller.SpeakerLabel.text, Is.EqualTo(esperada.Speaker));
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RF05_AvanzarLlegaHastaElFinalYSaleAOtraPantalla()
        {
            var (controller, runner) = await OpenNarrative("N1_Apertura");
            var lineas = SequenceNamed(controller, "N1_Apertura").Lines.Length;

            for (var i = 0; i < lineas; i++)
            {
                Click(controller.AdvanceButton);
            }

            Assert.That(controller.Dialogue.IsFinished, Is.True, "la escena terminó");
            Assert.That(runner.Flow.Current, Is.Not.EqualTo(GameState.Narrative),
                "al terminar sale de la escena narrativa, no se queda sin salida (RNF-13)");
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_INC28_NoMuestraOmitirLaPrimeraVezQueSeVeLaEscena()
        {
            var (controller, _) = await OpenNarrative("N1_Apertura");

            Assert.That(controller.SkipButton.gameObject.activeInHierarchy, Is.False);
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_HU17_NoHayBotonDePausaEnUnaEscenaNarrativa()
        {
            await OpenNarrative("N1_Apertura");

            var pausa = Object.FindObjectsByType<Button>(FindObjectsInactive.Include)
                .Where(button => button.GetComponentInChildren<Text>(true) is { } text
                                 && text.text.Trim().Equals("Pausa", StringComparison.OrdinalIgnoreCase));

            Assert.That(pausa, Is.Empty);
        }

        [Test]
        [Timeout(30000)]
        public async Task NarrativeScene_RNF01_LaLineaMasLargaCabeEnSuCuadroDeDialogo()
        {
            var (controller, _) = await OpenNarrative("N1_Hallazgo");
            var masLarga = controller.Sequences
                .SelectMany(sequence => sequence.Lines)
                .OrderByDescending(line => line.Text.Length)
                .First();

            controller.BodyLabel.text = masLarga.Text;
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var caja = controller.BodyLabel.rectTransform.rect;
            var generador = controller.BodyLabel.cachedTextGenerator;
            var alto = generador.GetPreferredHeight(masLarga.Text,
                controller.BodyLabel.GetGenerationSettings(caja.size)) / controller.BodyLabel.pixelsPerUnit;

            Assert.That(alto, Is.LessThanOrEqualTo(caja.height),
                $"«{masLarga.Text}» desborda su cuadro: {alto:0} px en una caja de {caja.height:0} px");
        }

        // --- helpers -----------------------------------------------------------------------

        private static NarrativeSequence SequenceNamed(NarrativeSceneController controller, string id) =>
            controller.Sequences.First(sequence => sequence.Id == id);

        private static async Task<(NarrativeSceneController controller, GameFlowRunner runner)>
            OpenNarrative(string sequenceId)
        {
            LimpiarObjetosPersistentes();

            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu

            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(PlayerProfile.Create("Ana", Array.Empty<string>()).Profile);
            runner.StartNarrative(sequenceId);
            // Assert y no Assume: un arreglo roto tiene que fallar fuerte. Con `Assume` esta
            // misma comprobación dejó la prueba en «inconclusive» —pasando sin probar nada—
            // durante una corrida entera (07/09/2026).
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative),
                "el flujo no llegó a la escena narrativa: el arreglo de la prueba está roto");

            var controller = Object.FindAnyObjectByType<NarrativeSceneController>(FindObjectsInactive.Include);
            controller.Runner = runner;
            controller.Begin();
            await Awaitable.NextFrameAsync();
            return (controller, runner);
        }

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);
    }
}
