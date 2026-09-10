using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Game.Core;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.Fire.Tests
{
    [Category("Integration")]
    public class FirePanelTests
    {
        private const string SceneName = "Level1_Cave";

        // N1_Config: única posición efectiva «Muy cerca», mínimo de golpes efectivos = 3.
        private const int MinimumEffectiveStrikes = 3;

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
        public async Task FirePanel_RF14_PresentaDeslizanteBotonGolpearYRegistro()
        {
            var controller = await LoadPanel();
            var strike = ButtonWithLabel("Golpear");
            var registro = RectNamed("Registro");

            Assert.That(ReachableByRaycast(controller.PositionSlider.GetComponent<RectTransform>()), Is.True,
                "deslizante de posición alcanzable");
            Assert.That(strike is not null && ReachableByRaycast(strike.GetComponent<RectTransform>()), Is.True,
                "botón «Golpear» alcanzable");
            Assert.That(registro is not null && ReachableByRaycast(registro), Is.True,
                "área de registro alcanzable");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF15_MoverElDeslizanteNoEjecutaNingunGolpe()
        {
            var controller = await LoadPanel();
            var slider = controller.PositionSlider;

            slider.value = (int)StrikePosition.VeryClose;
            slider.value = (int)StrikePosition.Near;
            slider.value = (int)StrikePosition.Far;
            await Awaitable.NextFrameAsync();

            Assert.That(LogLines().text, Is.Empty, "el registro sigue vacío");
            Assert.That(controller.Attempt?.EffectiveStrikes ?? 0, Is.EqualTo(0),
                "ningún golpe efectivo contabilizado");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF16_GolpearEscribeEnElRegistroElMensajeDeLaDistancia()
        {
            var controller = await LoadPanel();
            controller.PositionSlider.value = (int)StrikePosition.Far;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var trasPrimerGolpe = LogLines().text;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var trasSegundoGolpe = LogLines().text;

            Assert.That(trasPrimerGolpe, Is.Not.Empty, "el primer golpe escribe el mensaje de la distancia");
            Assert.That(SplitLines(trasSegundoGolpe).Length, Is.EqualTo(2),
                "el segundo golpe acumula una segunda línea");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF19_SoplarSeHabilitaAlAlcanzarElMinimoDeGolpesEfectivos()
        {
            var controller = await LoadPanel();
            controller.PositionSlider.value = (int)StrikePosition.VeryClose;

            Assert.That(controller.BlowButton.interactable, Is.False, "«Soplar» deshabilitado antes de converger");
            Assert.That(controller.BlowLockedBadge.activeInHierarchy, Is.True, "badge de bloqueo visible antes");

            ClickTimes(controller.StrikeButton, MinimumEffectiveStrikes);
            await Awaitable.NextFrameAsync();

            Assert.That(controller.BlowButton.interactable, Is.True,
                "«Soplar» habilitado tras el mínimo de golpes efectivos");
            Assert.That(controller.BlowLockedBadge.activeInHierarchy, Is.False,
                "badge de bloqueo oculto tras converger");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF19_SoplarAtenuadoNoRespondeNiRegistraError()
        {
            var controller = await LoadPanel();
            Assume.That(controller.BlowButton.interactable, Is.False);

            Click(controller.BlowButton);
            await Awaitable.NextFrameAsync();

            Assert.That(LogLines().text, Is.Empty);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RNF02_ElMapaDeControlesNoTieneNingunBindingDeTeclado()
        {
            await LoadPanel();
            var module = Object.FindAnyObjectByType<InputSystemUIInputModule>(FindObjectsInactive.Include);
            Assume.That(module, Is.Not.Null);
            Assume.That(module.actionsAsset, Is.Not.Null);

            Assert.That(module.actionsAsset.bindings,
                Has.None.Matches<InputBinding>(MentionsKeyboardOrGamepad),
                "el mapa de controles no cita Keyboard ni Gamepad");
            Assert.That(Object.FindObjectsByType<PlayerInput>(FindObjectsInactive.Include), Is.Empty,
                "la escena no lleva PlayerInput");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RNF03_LaEscenaNoPresentaListaDeTareas()
        {
            await LoadPanel();

            var listaDeTareas = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include)
                .Where(t => NameSuggestsTaskList(t.name))
                .ToArray();
            var etiquetasDeInstruccion = Object.FindObjectsByType<Text>(FindObjectsInactive.Include)
                .Where(t => t.name == "InstruccionLabel" && t.isActiveAndEnabled)
                .ToArray();

            Assert.That(listaDeTareas, Is.Empty, "ningún contenedor de lista de tareas ni casillas (INC-41)");
            Assert.That(etiquetasDeInstruccion.Length, Is.EqualTo(1), "una sola etiqueta de instrucción activa");
        }

        // Aserción de layout determinista (no verificación visual): que el ScrollRect absorba el
        // crecimiento del registro. Va con [Category("Integration")] de la clase.
        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF17_ElRegistroNoDesbordaTrasDiezIntentos()
        {
            var controller = await LoadPanel();
            controller.PositionSlider.value = (int)StrikePosition.Far;

            ClickTimes(controller.StrikeButton, 10);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var content = RectNamed("Content");
            var viewport = RectNamed("Viewport");

            Assert.That(SplitLines(LogLines().text).Length, Is.EqualTo(10), "diez líneas registradas");
            Assert.That(content.rect.height, Is.GreaterThan(viewport.rect.height),
                "el contenido creció más allá del viewport: hay crecimiento que absorber");
            Assert.That(IsWithin(viewport, RectNamed("Panel")), Is.True,
                "el viewport (la zona de recorte) queda dentro del panel");
        }

        [Test]
        [Timeout(20000)]
        [Category("VisualVerification")]
        [Description("Tras ejecutar esta prueba, revisar la captura: el botón «Soplar» atenuado " +
                     "muestra un icono de candado y la palabra «Aún no» además del color, no solo por " +
                     "color; el contraste texto/fondo del registro y de los controles es suficiente " +
                     "(≥ 4.5:1, RNF-20); los glifos con tilde («Aún no», la instrucción) se dibujan.")]
        public async Task FirePanel_RNF19_SoplarAtenuadoSeDistinguePorIconoYTextoAdemasDelColor()
        {
            await LoadPanel();

            CaptureScreenshot("FirePanel_RNF19_SoplarAtenuado");
        }

        // --- helpers -----------------------------------------------------------------------

        private static async Task<FirePanelController> LoadPanel()
        {
            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync(); // deja correr FirePanelController.Start()

            var controller = Object.FindAnyObjectByType<FirePanelController>(FindObjectsInactive.Include);
            Assert.That(controller, Is.Not.Null, "la escena Level1_Cave no tiene FirePanelController");
            return controller;
        }

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        private static void ClickTimes(Button button, int times)
        {
            for (var i = 0; i < times; i++)
            {
                Click(button);
            }
        }

        private static Button ButtonWithLabel(string label) => Array.Find(
            Object.FindObjectsByType<Button>(FindObjectsInactive.Include),
            b => b.GetComponentInChildren<Text>(true) is { } text && text.text.Trim() == label);

        private static Text LogLines() => Array.Find(
            Object.FindObjectsByType<Text>(FindObjectsInactive.Include), t => t.name == "Lineas");

        private static RectTransform RectNamed(string name) => Array.Find(
            Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include), rt => rt.name == name);

        private static string[] SplitLines(string text) =>
            text.Split('\n').Where(line => line.Length > 0).ToArray();

        private static bool ReachableByRaycast(RectTransform target)
        {
            var raycaster = Object.FindAnyObjectByType<GraphicRaycaster>();
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(null, target.position)
            };
            var results = new List<RaycastResult>();
            raycaster.Raycast(pointer, results);
            return results.Exists(r => r.gameObject.transform == target
                                      || r.gameObject.transform.IsChildOf(target));
        }

        private static bool MentionsKeyboardOrGamepad(InputBinding binding)
        {
            var paths = (binding.path ?? string.Empty) + " " + (binding.effectivePath ?? string.Empty);
            return paths.Contains("Keyboard", StringComparison.OrdinalIgnoreCase)
                   || paths.Contains("Gamepad", StringComparison.OrdinalIgnoreCase);
        }

        private static bool NameSuggestsTaskList(string name)
        {
            var words = new[] { "Tarea", "Task", "Checklist", "Lista" };
            return words.Any(word => name.Contains(word, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsWithin(RectTransform inner, RectTransform outer)
        {
            var innerCorners = new Vector3[4];
            var outerCorners = new Vector3[4];
            inner.GetWorldCorners(innerCorners);
            outer.GetWorldCorners(outerCorners);
            var bounds = Rect.MinMaxRect(outerCorners[0].x, outerCorners[0].y,
                outerCorners[2].x, outerCorners[2].y);
            return bounds.Contains(innerCorners[0]) && bounds.Contains(innerCorners[2]);
        }

        /// <remarks>
        /// Copia del patrón de <c>LevelSelectTests</c>: una prueba de verificación visual sin este
        /// aserto «pasa» sin escribir nada. En batchmode se ignora porque <c>ScreenCapture</c>
        /// necesita la render texture de la Game View.
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

            File.Delete(path);

            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.Destroy(texture);

            Assert.That(File.Exists(path), Is.True, $"no se escribió la captura en «{path}»");
            TestContext.WriteLine($"Captura: {path}");
        }
    }
}
