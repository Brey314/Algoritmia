using System;
using System.IO;
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
    public class LevelSelectTests
    {
        private const string SceneName = "LevelSelect";

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
        public async Task LevelSelect_RF03_ElNivelAlcanzadoSeMuestraDesbloqueadoYSinCandado()
        {
            var (controller, _) = await OpenLevelSelect(NewProfile());

            Assert.That(controller.ButtonFor(LevelId.Fire).interactable, Is.True, "interactable");
            Assert.That(controller.LockBadgeShownFor(LevelId.Fire), Is.False, "candado");
        }

        [Test]
        [Timeout(20000)]
        public async Task LevelSelect_RF03_LosNivelesNoAlcanzadosSeMuestranBloqueadosConCandado()
        {
            var (controller, _) = await OpenLevelSelect(NewProfile());

            Assert.That(new[] { LevelId.Wheel, LevelId.River },
                Has.All.Matches<LevelId>(level => !controller.ButtonFor(level).interactable
                                                  && controller.LockBadgeShownFor(level)));
        }

        [Test]
        [Timeout(20000)]
        public async Task LevelSelect_RF03_PulsarUnNivelBloqueadoNoCambiaElFlujo()
        {
            var (controller, runner) = await OpenLevelSelect(NewProfile());

            Click(controller.ButtonFor(LevelId.Wheel));

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.LevelSelect));
        }

        [Test]
        [Timeout(20000)]
        public async Task LevelSelect_RF03_CompletarElNivel1HabilitaElNivel2()
        {
            var profile = NewProfile();
            var (controller, _) = await OpenLevelSelect(profile);

            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(1, 0, 1, 10f));
            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Fire);
            controller.Refresh();

            Assert.That(controller.ButtonFor(LevelId.Wheel).interactable, Is.True);
        }

        [Test]
        [Timeout(20000)]
        [Category("VisualVerification")]
        [Description("Tras ejecutar esta prueba, revisar la captura: el nivel bloqueado se " +
                     "distingue por candado y la palabra «Bloqueado» además del color, no solo por " +
                     "color; contraste del texto suficiente; los glifos con tilde se dibujan.")]
        public async Task LevelSelect_RNF19_ElEstadoBloqueadoLlevaCandadoYTextoAdemasDelColor()
        {
            await OpenLevelSelect(NewProfile());

            CaptureScreenshot("LevelSelect_RNF19_NivelBloqueado");
        }

        [Test]
        [Timeout(20000)]
        public async Task LevelSelect_RNF13_VolverNoLanzaSiLaEscenaSeAbreSinPasarPorBoot()
        {
            // Sin perfil activo los tres niveles quedan bloqueados y no responden al clic
            // (RF-03), así que «Volver» es el único botón alcanzable de esta escena.
            await LoadLevelSelect();
            // Sin GameFlowRunner —esta escena se cargó sin pasar por Boot— «Volver» avisa y no
            // navega.
            LogAssert.Expect(LogType.Warning, new Regex("sin pasar por"));

            Click(FindButtonByLabel("Volver"));

            // Lo que no puede pasar es que el clic lance: cualquier excepción registrada aquí
            // sería un mensaje no esperado y esta llamada la delata.
            LogAssert.NoUnexpectedReceived();
        }

        // --- helpers -----------------------------------------------------------------------

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        private static async Task LoadLevelSelect()
        {
            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
        }

        private static Button FindButtonByLabel(string label) => Array.Find(
            Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude),
            button => button.GetComponentInChildren<Text>() is { } text && text.text.Trim() == label);

        private static async Task<(LevelSelectController controller, GameFlowRunner runner)>
            OpenLevelSelect(PlayerProfile profile)
        {
            await LoadLevelSelect();

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu

            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(profile);
            Assume.That(runner.Flow.Current, Is.EqualTo(GameState.LevelSelect));

            var controller = Object.FindAnyObjectByType<LevelSelectController>(FindObjectsInactive.Include);
            controller.Runner = runner;
            controller.Refresh();
            await Awaitable.NextFrameAsync();
            return (controller, runner);
        }

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        /// <summary>
        /// Guarda la captura que la prueba deja para revisar a mano, y **afirma que existe**.
        /// </summary>
        /// <remarks>
        /// Una prueba de verificación visual no lleva más asertos: su veredicto lo pone una
        /// persona mirando la imagen. Por eso esta comprobación no es adorno — sin ella
        /// «Passed» solo significa «no lanzó», y el 08/09/2026 las dos pruebas de esta suite
        /// pasaron durante tres corridas seguidas sin escribir un solo archivo. Una prueba que
        /// no puede fallar no verifica nada.
        ///
        /// En batchmode se ignora en vez de fallar: `ScreenCapture` necesita la render texture
        /// de la Game View, que ahí no existe, así que el fallo no diría nada del producto —
        /// solo que no hay pantalla. `Ignored` lo deja visible sin fingir que se verificó.
        /// </remarks>
        private static void CaptureScreenshot(string name)
        {
            if (Application.isBatchMode)
            {
                Assert.Ignore("La captura exige una Game View: correr desde el Editor.");
            }

            var directory = $"{Application.persistentDataPath}/TestScreenshots";
            Directory.CreateDirectory(directory);
            var path = $"{directory}/{name}.png";

            // Una captura de una corrida anterior no puede hacerse pasar por la de esta.
            File.Delete(path);

            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.Destroy(texture);

            Assert.That(File.Exists(path), Is.True, $"no se escribió la captura en «{path}»");
            TestContext.WriteLine($"Captura: {path}");
        }
    }
}
