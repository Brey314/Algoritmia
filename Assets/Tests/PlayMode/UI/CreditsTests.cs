using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    [Category("Integration")]
    public class CreditsTests
    {
        private const string SceneName = "Credits";

        [TearDown]
        public void DestruirLosObjetosPersistentes()
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
        [Timeout(20000)]
        public async Task Credits_RF08_MuestraElReconocimientoDeLaAutoria()
        {
            var (controller, _) = await OpenCredits();

            Assert.That(controller.BodyLabel.text, Does.Contain("Anonaky"));
        }

        [Test]
        [Timeout(20000)]
        public async Task Credits_RF08_VolverRegresaAlMenuPrincipal()
        {
            var (_, runner) = await OpenCredits();

            Click(FindButtonByLabel("Volver"));

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.MainMenu));
        }

        [Test]
        [Timeout(20000)]
        public async Task Credits_RNF13_VolverNoLanzaSiLaEscenaSeAbreSinPasarPorBoot()
        {
            await LoadCredits();
            // Sin GameFlowRunner —esta escena se cargó sin pasar por Boot— «Volver» avisa y no
            // navega.
            LogAssert.Expect(LogType.Warning, new Regex("sin pasar por"));

            Click(FindButtonByLabel("Volver"));

            // Lo que no puede pasar es que el clic lance: cualquier excepción registrada aquí
            // sería un mensaje no esperado y esta llamada la delata.
            LogAssert.NoUnexpectedReceived();
        }

        // --- helpers -----------------------------------------------------------------------

        private static async Task LoadCredits()
        {
            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
        }

        private static async Task<(CreditsController controller, GameFlowRunner runner)> OpenCredits()
        {
            await LoadCredits();

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu
            runner.GoTo(GameState.Credits);
            Assume.That(runner.Flow.Current, Is.EqualTo(GameState.Credits));

            var controller = Object.FindAnyObjectByType<CreditsController>(FindObjectsInactive.Include);
            controller.Runner = runner;
            await Awaitable.NextFrameAsync();
            return (controller, runner);
        }

        private static Button FindButtonByLabel(string label) => Array.Find(
            Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude),
            button => button.GetComponentInChildren<Text>() is { } text && text.text.Trim() == label);

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);
    }
}
