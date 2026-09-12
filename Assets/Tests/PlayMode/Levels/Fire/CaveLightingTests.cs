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

namespace Game.Levels.Fire.Tests
{
    [Category("Integration")]
    public class CaveLightingTests
    {
        private const string SceneName = "Level1_Cave";

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
        [Category("VisualVerification")]
        [Category("Acceptance")]
        [Description("Tras ejecutar esta prueba, revisar la captura: la cueva luce visiblemente " +
                     "más iluminada tras soplar que durante la convergencia (guion E7, iluminación " +
                     "completa), sin manchas ni zonas oscuras residuales, y el color resultante " +
                     "mantiene la familia de azules fríos del acorde cromático de la escena.")]
        public async Task FireLevel_RF21_IluminacionSubeUnEscalonPorGolpeEfectivo()
        {
            var (controller, _) = await LoadPanelWithProfile(NewProfile());
            controller.ForceSlider.value = 7; // fuerza efectiva (N1_Config, Fase 5)
            foreach (var piece in controller.Pieces)
            {
                piece.MoveTo(controller.FireSpot.anchoredPosition); // todo en su sitio (T22)
            }

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var progressTrasPrimerGolpe = controller.Lighting.Progress;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var progressTrasSegundoGolpe = controller.Lighting.Progress;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var progressTrasTercerGolpe = controller.Lighting.Progress;

            Assert.That(progressTrasPrimerGolpe, Is.GreaterThan(0f),
                "el primer golpe efectivo ya sube un escalón desde el fondo más oscuro");
            Assert.That(progressTrasSegundoGolpe, Is.GreaterThan(progressTrasPrimerGolpe),
                "el segundo golpe efectivo sube otro escalón");
            Assert.That(progressTrasTercerGolpe, Is.GreaterThan(progressTrasSegundoGolpe),
                "el tercer golpe efectivo sube otro escalón más");
            Assert.That(progressTrasTercerGolpe, Is.LessThan(1f),
                "alcanzar el mínimo de golpes («Soplar» habilitado) todavía no es la iluminación " +
                "completa — E4 (converger) no es E7 (ignición)");

            Click(controller.BlowButton);
            var llegoAlMaximo = await WaitUntilAsync(
                () => controller.Lighting.Progress >= 1f - 0.001f, 5f);

            Assert.That(llegoAlMaximo, Is.True,
                "la iluminación no llegó al máximo tras la animación de ignición (guion E7)");

            CaptureScreenshot("FireLevel_RF21_IluminacionCompleta");
        }

        [Test]
        [Timeout(20000)]
        [Category("VisualVerification")]
        [Category("Acceptance")]
        [Description("Tras ejecutar esta prueba, revisar la captura: el texto de la instrucción se " +
                     "lee con claridad sobre el fondo más oscuro de la cueva, sin necesidad de " +
                     "forzar la vista.")]
        public async Task FirePanel_RNF20_ContrasteSuficienteEnElEstadoMasOscuro()
        {
            var controller = await LoadPanel();

            Assume.That(controller.Lighting, Is.Not.Null, "la escena debe traer CaveLightingController wireado");
            Assume.That(controller.Lighting.Progress, Is.EqualTo(0f),
                "panel recién cargado: ningún golpe efectivo todavía");
            Assume.That(controller.Lighting.Current.Ambient, Is.EqualTo(controller.Lighting.Darkest.Ambient),
                "sin golpes efectivos, la capa de oscuridad ya está en su estado más oscuro");

            var instruccion = TextNamed("InstruccionLabel");
            Assume.That(instruccion, Is.Not.Null, "la escena debe traer una InstruccionLabel");

            // La instrucción va sobre su propia tablilla (mockup 7), no sobre la cueva: el fondo que
            // cuenta para el contraste es la cara de esa tablilla, que la oscuridad no toca.
            var tablilla = instruccion.GetComponentInParent<Image>();
            Assume.That(tablilla, Is.Not.Null, "la instrucción cuelga de una tablilla con Image");
            var contraste = ContrastRatio(instruccion.color, tablilla.color);
            TestContext.WriteLine($"Contraste texto/tablilla en el estado más oscuro: {contraste:F2}:1");

            Assert.That(contraste, Is.GreaterThanOrEqualTo(4.5),
                $"el contraste calculado ({contraste:F2}:1) no alcanza el mínimo 4.5:1 de RNF-20");

            CaptureScreenshot("FirePanel_RNF20_EstadoMasOscuro");
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
            // Los objetos de la escena anterior se destruyen al final de fotograma, no en el
            // mismo en que `isDone` se vuelve verdadero: sin este fotograma extra,
            // `FindAnyObjectByType` puede devolver el controlador de la prueba anterior en vez
            // del recién cargado (mismo workaround que FirePanelTests, medido el 11/09/2026).
            await Awaitable.NextFrameAsync();

            var controllers = Object.FindObjectsByType<FirePanelController>(FindObjectsInactive.Include);
            Assert.That(controllers.Length, Is.EqualTo(1),
                $"se esperaba un único FirePanelController vivo, había {controllers.Length}");
            return controllers[0];
        }

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        private static async Task<(FirePanelController controller, GameFlowRunner runner)>
            LoadPanelWithProfile(PlayerProfile profile)
        {
            var controller = await LoadPanel();

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu

            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(profile);
            runner.StartPlaying(LevelId.Fire, 1);
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing),
                "el flujo no llegó a Playing: el arreglo de la prueba está roto");

            controller.Runner = runner;
            return (controller, runner);
        }

        /// <summary>
        /// Sondea <paramref name="condition"/> cuadro a cuadro hasta que se cumpla o venza el
        /// tiempo real: la animación de ignición (~0.6 s) corre en tiempo real (mismo patrón que
        /// <c>FirePanelTests.WaitUntilAsync</c>, plan T15).
        /// </summary>
        private static async Task<bool> WaitUntilAsync(Func<bool> condition, float timeoutSeconds)
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (!condition())
            {
                if (Time.realtimeSinceStartup >= deadline)
                {
                    return false;
                }

                await Awaitable.NextFrameAsync();
            }

            return true;
        }

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        private static Text TextNamed(string name) => Array.Find(
            Object.FindObjectsByType<Text>(FindObjectsInactive.Include),
            t => t.name == name && t.isActiveAndEnabled);

        private static bool ApproximatelyEqual(Color a, Color b, float tolerance = 0.01f) =>
            Mathf.Abs(a.r - b.r) <= tolerance
            && Mathf.Abs(a.g - b.g) <= tolerance
            && Mathf.Abs(a.b - b.b) <= tolerance;

        /// <summary>
        /// Contraste WCAG 2.x entre dos colores (fórmula de luminancia relativa). Cálculo real
        /// contra los colores efectivamente aplicados en la escena, no un valor de confianza
        /// fijado de antemano (RNF-20, plan T19 «El ajuste de RNF-20»).
        /// </summary>
        private static double ContrastRatio(Color a, Color b)
        {
            var luminanceA = RelativeLuminance(a);
            var luminanceB = RelativeLuminance(b);
            var lighter = Math.Max(luminanceA, luminanceB);
            var darker = Math.Min(luminanceA, luminanceB);
            return (lighter + 0.05) / (darker + 0.05);
        }

        private static double RelativeLuminance(Color color) =>
            0.2126 * LinearizedChannel(color.r)
            + 0.7152 * LinearizedChannel(color.g)
            + 0.0722 * LinearizedChannel(color.b);

        private static double LinearizedChannel(float channel) =>
            channel <= 0.03928 ? channel / 12.92 : Math.Pow((channel + 0.055) / 1.055, 2.4);

        /// <remarks>
        /// Copia del patrón de <c>FirePanelTests</c>/<c>LevelSummaryTests</c>: cada archivo de
        /// prueba en este proyecto lleva su propia copia de este helper, no una utilidad
        /// compartida.
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
