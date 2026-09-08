using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class MainMenuTests
    {
        private const string SceneName = "MainMenu";
        private static readonly string[] Options = { "Jugar", "Créditos", "Salir" };

        /// <summary>Aviso de <c>ScreenFlow</c> cuando la pantalla no tiene con qué navegar.</summary>
        private static readonly Regex MissingFlow = new Regex("sin pasar por");

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
        public async Task MainMenu_RF01_MuestraJugarCreditosYSalirAlcanzablesPorRaycast()
        {
            await LoadMainMenu();

            Assert.That(Options.Select(label => FindOption(label) is { } button
                                               && IsReachableByRaycast(button)),
                Has.All.True);
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RF09_GuardaElPerfilActivoAntesDeCerrar()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();
            var order = new List<string>();
            sut.Saver = new SpyProfileSaver(() => order.Add("guardar"));
            sut.Quit = () => order.Add("cerrar");

            Click(FindOption("Salir"));

            Assert.That(order, Is.EqualTo(new[] { "guardar", "cerrar" }));
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RNF18_ElTituloSeLeeDelScriptableObject()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();

            Assert.That(sut.TitleLabel.text, Is.EqualTo(sut.TitleConfig.Title));
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RF01_LosBotonesCabenEnPantallaYNoSeSolapan()
        {
            await LoadMainMenu();
            var screen = new Rect(0f, 0f, Screen.width, Screen.height);
            var rects = Options.Select(FindOption).Select(ScreenRectOf).ToArray();

            Assert.That(rects, Has.All.Matches<Rect>(rect =>
                screen.Contains(rect.min) && screen.Contains(rect.max)
                && rects.Count(other => other.Overlaps(rect)) == 1));
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RNF13_SusBotonesNoLanzanSiLaEscenaSeAbreSinPasarPorBoot()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();
            // «Salir» no puede cerrar el reproductor a mitad de la suite.
            sut.Quit = () => { };
            // Sin GameFlowRunner —la escena se cargó sin pasar por Boot— «Jugar» y «Créditos»
            // avisan y no navegan; «Salir» sigue cerrando, que no necesita flujo.
            LogAssert.Expect(LogType.Warning, MissingFlow);
            LogAssert.Expect(LogType.Warning, MissingFlow);

            foreach (var label in Options)
            {
                Click(FindOption(label));
            }

            // Lo que no puede pasar es que el clic lance: cualquier excepción registrada aquí
            // sería un mensaje no esperado y esta llamada la delata.
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        [Timeout(20000)]
        [Category("VisualVerification")]
        [Description("Tras ejecutar esta prueba, revisar la captura: contraste entre el texto " +
                     "(título y botones) y su fondo, que los glifos con tilde y «¿ ¡» se dibujen, " +
                     "y que nada se recorte ni deforme.")]
        public async Task MainMenu_RNF20_ContrasteTextoFondoSuficiente()
        {
            await LoadMainMenu();

            CaptureScreenshot("MainMenu_RNF20_ContrasteTextoFondo");
        }

        // --- helpers -----------------------------------------------------------------------

        private static async Task LoadMainMenu()
        {
            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            // Un frame más para que corran Awake y Start de los controladores de la escena.
            await Awaitable.NextFrameAsync();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
        }

        private static Button FindOption(string label) => Object
            .FindObjectsByType<Button>(FindObjectsInactive.Exclude)
            .FirstOrDefault(button => button.GetComponentInChildren<Text>() is { } text
                                      && text.text.Trim() == label);

        private static bool IsReachableByRaycast(Selectable target)
        {
            var raycaster = Object.FindAnyObjectByType<GraphicRaycaster>();
            var data = new PointerEventData(EventSystem.current)
            {
                position = ScreenRectOf((RectTransform)target.transform).center
            };
            var results = new List<RaycastResult>();
            raycaster.Raycast(data, results);
            return results.Any(hit => hit.gameObject.GetComponentInParent<Selectable>() == target);
        }

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        private static Rect ScreenRectOf(Selectable selectable) =>
            ScreenRectOf((RectTransform)selectable.transform);

        private static Rect ScreenRectOf(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            // El Canvas de la pantalla es Screen Space - Overlay: las esquinas de mundo ya
            // están en píxeles de pantalla.
            var min = new Vector2(corners[0].x, corners[0].y);
            var max = new Vector2(corners[2].x, corners[2].y);
            return new Rect(min, max - min);
        }

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

        private sealed class SpyProfileSaver : IProfileSaver
        {
            private readonly Action _onSaveActive;
            public SpyProfileSaver(Action onSaveActive) => _onSaveActive = onSaveActive;
            public void SaveActive() => _onSaveActive();
        }
    }
}
