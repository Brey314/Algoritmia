using System;
using System.IO;
using System.Threading.Tasks;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

        // --- helpers -----------------------------------------------------------------------

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        private static async Task<(LevelSelectController controller, GameFlowRunner runner)>
            OpenLevelSelect(PlayerProfile profile)
        {
            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();

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

        private static void CaptureScreenshot(string name)
        {
            var directory = $"{Application.persistentDataPath}/TestScreenshots";
            Directory.CreateDirectory(directory);
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes($"{directory}/{name}.png", texture.EncodeToPNG());
            Object.Destroy(texture);
        }
    }
}
