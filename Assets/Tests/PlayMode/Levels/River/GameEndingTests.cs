using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// El último tramo del juego (R14): la prueba superada sale al cruce (RF-44) y, tras el resumen
    /// del Nivel 3, el flujo es exactamente <c>LevelSummary → Narrative → Credits → MainMenu</c>
    /// (INC-39, guion §9), sin ningún estado final irrecuperable (RNF-13).
    /// </summary>
    /// <remarks>
    /// Vive aquí y no en <c>Game.Core.PlayMode.Tests</c> porque el cruce se dispara desde el panel
    /// de ensamblaje, que es interno al nivel. Las escenas narrativas se recorren pulsando su
    /// «Continuar» línea a línea, como el estudiante: no se salta ninguna (CP-07).
    /// </remarks>
    [Category("Integration")]
    public class GameEndingTests
    {
        private const string Nombre = "PruebaCierre";

        [SetUp]
        public void SetUp() => AssemblyPanelController.ForgetLevelMemory();

        [TearDown]
        public void TearDown()
        {
            AssemblyPanelController.ForgetLevelMemory();
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                runner.Session.Delete(Nombre);
                Object.DestroyImmediate(runner.gameObject);
            }

            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }
        }

        [Test]
        [Timeout(180000)]
        [Category("Acceptance")]
        public async Task GameEnding_INC39_RecorreLevelSummaryNarrativeCreditsYMainMenu()
        {
            SceneManager.LoadScene("Boot");
            await Esperar(() => GameFlowRunner.Instance != null && SceneManager.GetActiveScene().name == "MainMenu");
            var runner = GameFlowRunner.Instance;
            runner.Session.Delete(Nombre);
            runner.GoTo(GameState.ProfileSelect);
            var perfil = PlayerProfile.Create(Nombre, Array.Empty<string>()).Profile;
            perfil.Reach(LevelId.River);
            perfil.ConfirmPhase(new PhaseId(LevelId.River, 1), default);
            perfil.ConfirmPhase(new PhaseId(LevelId.River, 2), default);
            runner.SelectProfile(perfil);
            Assume.That(runner.StartPlaying(LevelId.River, 3), Is.True);
            await Esperar(() => SceneManager.GetActiveScene().name == "Level3_River");

            // La prueba superada sale al cruce con la fase 3 confirmada (lo cubre RF-44 abajo):
            // aquí se entra a él directamente, porque lo que se recorre es el cierre.
            perfil.ConfirmPhase(new PhaseId(LevelId.River, 3), default);
            Assume.That(runner.StartNarrative("N3_Escena33_Cruce"), Is.True);

            // Cruce → resumen.
            await AvanzarHasta(runner, () => runner.Flow.Current == GameState.LevelSummary);
            await Esperar(() => SceneManager.GetActiveScene().name == "LevelSummary" && ButtonWithLabel("Continuar") != null);
            await Awaitable.NextFrameAsync();
            Click(ButtonWithLabel("Continuar"));

            // Resumen → escena final, que no se puede omitir la primera vez (CP-07).
            await Esperar(() => runner.Flow.Current == GameState.Narrative && runner.Flow.NarrativeSequenceId == "N3_EscenaFinal");
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N3_EscenaFinal"),
                "«Continuar» tras el Nivel 3 va a la escena final, no al menú (INC-39)");
            await Esperar(() => SceneManager.GetActiveScene().name == "Narrative" && ButtonWithLabel("Continuar") != null);
            Assert.That(ButtonWithLabel("Omitir"), Is.Null, "la escena final no ofrece omitir (CP-07)");

            // Escena final → créditos → inicio.
            await AvanzarHasta(runner, () => runner.Flow.Current == GameState.Credits);
            await Esperar(() => SceneManager.GetActiveScene().name == "Credits" && ButtonWithLabel("Volver") != null);
            await Awaitable.NextFrameAsync();
            Click(ButtonWithLabel("Volver"));
            await Esperar(() => runner.Flow.Current == GameState.MainMenu && SceneManager.GetActiveScene().name == "MainMenu");

            var guardado = runner.Session.Load(Nombre);
            Assert.That(guardado.IsLevelComplete(LevelId.River), Is.True, "el perfil queda íntegro en disco (RF-04)");
            Assert.That(LevelUnlockPolicy.AllLevels.All(guardado.IsUnlocked), Is.True, "con los tres niveles desbloqueados (RF-03)");
            Assert.That(runner.GoTo(GameState.ProfileSelect), Is.True, "y desde el inicio se puede volver a jugar: ningún estado final irrecuperable (RNF-13)");
        }

        [Test]
        [Timeout(120000)]
        [Category("Acceptance")]
        public async Task GameEnding_RF44_LaPruebaSuperadaReproduceElCruceYElCierre()
        {
            var runner = await ConditionalNarrativeTests.StartAt(RaftPhase.MastAndSail, Nombre);
            var (_, panel) = await AssemblyPanelTests.OpenAssembly();

            await AssemblyPanelTests.FillPhase(panel, RaftPhase.MastAndSail);
            await AssemblyPanelTests.Confirm(panel);

            Assume.That(panel.Assembly.IsComplete, Is.True, "la balsa quedó completa");
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative), "la prueba superada sale a la narrativa");
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N3_Escena33_Cruce"), "al cruce (RF-44, guion §8.5)");

            // Sin SceneLoader el flujo no carga la escena: se carga aquí para ver el cruce.
            SceneManager.LoadScene("Narrative");
            var balsa = await Esperar<Image>(image => image.sprite != null && image.sprite.name == "prop_n3_balsa_cruzando");
            var origen = balsa.rectTransform.anchoredPosition;
            var giro = balsa.rectTransform.localRotation;

            // Arranca con la primera línea, tras el fundido de apertura: se espera el avance, no
            // un número fijo de cuadros.
            await Esperar(() => balsa.rectTransform.anchoredPosition.x > origen.x + 1f, 15f);
            Assert.That(balsa.rectTransform.anchoredPosition.x, Is.GreaterThan(origen.x + 1f),
                "la balsa cruza hacia la otra orilla desde la primera línea (RF-44)");
            Assert.That(balsa.rectTransform.localRotation, Is.EqualTo(giro), "y no gira: se desliza (RNF-21)");

            await AvanzarHasta(runner, () => runner.Flow.Current == GameState.LevelSummary);
            Assert.That(runner.Flow.ActiveProfile.IsLevelComplete(LevelId.River), Is.True, "el cierre llega con las tres fases confirmadas (RF-04)");
        }

        // --- utilería ---------------------------------------------------------------------------

        /// <summary>Pulsa «Continuar» de cada escena narrativa, línea a línea, hasta que el flujo llegue a donde se espera.</summary>
        private static async Task AvanzarHasta(GameFlowRunner runner, Func<bool> destino, float segundos = 60f)
        {
            var limite = Time.realtimeSinceStartup + segundos;
            while (!destino())
            {
                if (Time.realtimeSinceStartup >= limite)
                {
                    Assert.Fail($"El flujo no llegó a donde se esperaba en {segundos} s: está en {runner.Flow.Current} / {runner.Flow.NarrativeSequenceId}.");
                }

                if (runner.Flow.Current == GameState.Narrative && ButtonWithLabel("Continuar") is { } continuar && continuar.isActiveAndEnabled)
                {
                    Click(continuar);
                }

                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
        }

        private static Button ButtonWithLabel(string label) => Array.Find(
            Object.FindObjectsByType<Button>(),
            b => b.GetComponentInChildren<Text>(true) is { } text && text.text.Trim() == label);

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        private static async Task<T> Esperar<T>(Func<T, bool> condicion, float segundos = 30f) where T : Object
        {
            T encontrado = null;
            await Esperar(() => (encontrado = Object.FindObjectsByType<T>().FirstOrDefault(condicion)) != null, segundos);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            return encontrado;
        }

        private static async Task Esperar(Func<bool> condicion, float segundos = 30f)
        {
            var limite = Time.realtimeSinceStartup + segundos;
            while (Time.realtimeSinceStartup < limite)
            {
                if (condicion())
                {
                    return;
                }

                await Awaitable.NextFrameAsync();
            }

            Assert.Fail($"La condición no se cumplió en {segundos} s.");
        }
    }
}
