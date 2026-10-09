using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Game.Core;
using Game.Scaffolding;
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

        /// <summary>Un píxel del lienzo duplicado (3840 de ancho) en fracción de su ancho: lo que se perdona al decir «no cruza el eje».</summary>
        private const float SeamTolerance = 1f / 3840f;

        /// <summary>Lo que el icono de estado se separa de la esquina de abajo a la izquierda de la imagen, en unidades del lienzo (D11).</summary>
        private const float IconInset = 16f;

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
        public async Task LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura()
        {
            var (controller, _) = await OpenLevelSelect(NewProfile());

            foreach (var level in new[] { LevelId.Fire, LevelId.Wheel, LevelId.River })
            {
                var window = WindowFor(controller, level);
                Assert.That(window, Is.Not.Null, $"{level}: sin «Ventana» con la ilustración bajo la tarjeta");
                Assert.That(window.Art.sprite, Is.Not.Null, $"{level}: sin sprite asignado");

                // Desde la etapa 4b (D11) el Nivel 2 muestra el bosque, un lienzo duplicado de 3840 × 1080: ya no basta con exigir
                // 16:9, hay que mirar qué parte del lienzo cae en la ventana. Un 16:9 se ve entero; un duplicado, solo por un lado del eje.
                var size = window.Art.sprite.rect.size;
                var aspect = size.x / size.y;
                var (left, right) = IllustrationProbe.VisibleRange(window);
                var duplicated = Mathf.Abs(aspect - IllustrationFraming.DuplicatedCanvasAspect) < 0.01f;

                Assert.That(aspect, Is.EqualTo(16f / 9f).Within(0.01f).Or.EqualTo(IllustrationFraming.DuplicatedCanvasAspect).Within(0.01f),
                    $"{level}: el sprite de la tarjeta no es 16:9 ni el lienzo duplicado");
                Assert.That(left, Is.GreaterThanOrEqualTo(-SeamTolerance), $"{level}: la ilustración llena la ventana por la izquierda");
                Assert.That(right, Is.LessThanOrEqualTo(1f + SeamTolerance), $"{level}: la ilustración llena la ventana por la derecha");
                Assert.That(duplicated && left < IllustrationFraming.MirrorAxis - SeamTolerance && right > IllustrationFraming.MirrorAxis + SeamTolerance,
                    Is.False, $"{level}: la ventana cruza el eje x = 0,5 del lienzo duplicado y enseña la costura");
            }
        }

        // D11 (08/10/2026): el icono de estado (candado o visto) va abajo a la izquierda de la imagen y su texto, centrado
        // entre la imagen y el botón. Se miden las dos insignias de cada tarjeta, aunque en pantalla solo se vea una.
        [Test]
        [Timeout(20000)]
        public async Task LevelSelect_RNF19_ElIconoDeEstadoVaAbajoALaIzquierdaDeLaImagenYSuTextoEntreImagenYBoton()
        {
            var (controller, _) = await OpenLevelSelect(NewProfile());

            foreach (var level in new[] { LevelId.Fire, LevelId.Wheel, LevelId.River })
            {
                var framed = WindowFor(controller, level);
                var scale = framed.GetComponentInParent<Canvas>().rootCanvas.scaleFactor;
                var window = ScreenRectOf((RectTransform)framed.transform);
                var button = ScreenRectOf((RectTransform)controller.ButtonFor(level).transform);

                foreach (var badge in new[] { controller.LockedBadgeFor(level), controller.CompletedBadgeFor(level) })
                {
                    badge.SetActive(true);
                    var icon = badge.GetComponentsInChildren<Image>(true).Select(image => ScreenRectOf(image.rectTransform)).Aggregate(IllustrationProbe.Union);
                    var text = ScreenRectOf(badge.GetComponentInChildren<Text>(true).rectTransform);
                    var name = $"{level} · {badge.name}";

                    Assert.That(icon.xMin - window.xMin, Is.EqualTo(IconInset * scale).Within(1.5f), $"{name}: el icono, a 16 de la izquierda de la imagen");
                    Assert.That(icon.yMin - window.yMin, Is.EqualTo(IconInset * scale).Within(1.5f), $"{name}: y a 16 de su borde de abajo");
                    Assert.That(icon.xMax, Is.LessThanOrEqualTo(window.xMax), $"{name}: el icono cabe dentro de la imagen por la derecha");
                    Assert.That(icon.yMax, Is.LessThanOrEqualTo(window.yMax), $"{name}: y por arriba");
                    Assert.That(text.center.x, Is.EqualTo(window.center.x).Within(1.5f), $"{name}: el texto, centrado respecto de la imagen");
                    Assert.That(text.yMax, Is.LessThanOrEqualTo(window.yMin + 0.5f), $"{name}: el texto empieza debajo de la imagen");
                    Assert.That(text.yMin, Is.GreaterThanOrEqualTo(button.yMax - 0.5f), $"{name}: y termina encima del botón");
                    Assert.That(text.center.y, Is.EqualTo((window.yMin + button.yMax) / 2f).Within(1.5f), $"{name}: a media altura entre la imagen y el botón");
                }
            }
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
        public async Task LevelSelect_RNF14_UnNivelConTodasSusFasesConfirmadasDesbloqueaElSiguiente()
        {
            // Un cierre forzado a mitad de la escena de cierre del Nivel 2 (2.5) deja las tres
            // fases confirmadas pero nunca llama a LevelUnlockPolicy.UnlockAfterCompleting —eso
            // solo pasa en LevelSummaryController, al mostrarse el resumen (PF-RNF14-04,
            // 30/09/2026)—, así que ReachedLevel se queda en Wheel. El menú tiene que desbloquear
            // River igual, derivándolo de las fases ya guardadas.
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel);
            foreach (var phase in PhaseId.AllOf(LevelId.Wheel))
            {
                profile.ConfirmPhase(phase, new PerformanceIndicators(1, 0, 1, 10f));
            }

            var (controller, _) = await OpenLevelSelect(profile);

            Assert.That(controller.ButtonFor(LevelId.River).interactable, Is.True,
                "el Nivel 3 se desbloquea aunque el resumen del Nivel 2 nunca se mostró");
        }

        [Test]
        [Timeout(20000)]
        public async Task LevelSelect_HU14_ElNivelCompletadoSeMarcaEnElMenu()
        {
            var profile = NewProfile();
            var (controller, _) = await OpenLevelSelect(profile);

            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(1, 0, 1, 10f));
            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Fire);
            controller.Refresh();

            Assert.That(controller.CompletedBadgeShownFor(LevelId.Fire), Is.True,
                "el Nivel 1 completado no se marcó");
            Assert.That(controller.CompletedBadgeShownFor(LevelId.Wheel), Is.False,
                "el Nivel 2, recién desbloqueado y sin jugar, no está completado");
            Assert.That(controller.CompletedBadgeShownFor(LevelId.River), Is.False);
        }

        [Test]
        [Timeout(20000)]
        public async Task LevelSelect_HU14_UnNivelConFasesPendientesNoSeMarcaCompletado()
        {
            var profile = NewProfile();
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(1, 0, 1, 10f));
            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Fire);
            var (controller, _) = await OpenLevelSelect(profile);

            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), new PerformanceIndicators(1, 0, 1, 10f));
            controller.Refresh();

            Assert.That(controller.CompletedBadgeShownFor(LevelId.Wheel), Is.False,
                "el Nivel 2 solo tiene una de sus tres fases confirmadas");
            Assert.That(controller.ButtonFor(LevelId.Wheel).interactable, Is.True,
                "un nivel a medias sigue siendo jugable");
        }

        [Test]
        [Timeout(20000)]
        public async Task LevelSelect_HU14_UnPerfilNuevoNoMuestraNingunNivelCompletado()
        {
            var (controller, _) = await OpenLevelSelect(NewProfile());

            Assert.That(new[] { LevelId.Fire, LevelId.Wheel, LevelId.River },
                Has.All.Matches<LevelId>(level => !controller.CompletedBadgeShownFor(level)));
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

        [Test]
        [Timeout(20000)]
        public async Task LevelSelect_RF05_CadaNivelAbreLaSecuenciaDeAperturaDeSuFicha()
        {
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel);
            var (controller, runner) = await OpenLevelSelect(profile);

            Click(controller.ButtonFor(LevelId.Wheel));

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative), "entra a la narrativa");
            // El id se componía con la fórmula «N{nivel}_Apertura», que solo existe para el
            // Nivel 1: el Nivel 2 pedía «N2_Apertura», ninguna secuencia respondía y la escena
            // narrativa se quedaba en blanco. Qué secuencia abre cada nivel es contenido: desde
            // el Checkpoint W-F (15/09/2026) el Nivel 2 abre con la escena puente, que encadena
            // con la 2.1 por su asset (Camara_Narrativa_N2 §5).
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N2_PuenteI"),
                "el Nivel 2 abre por la escena puente del guion");
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

        /// <summary>La ventana con la ilustración de la tarjeta del nivel: sube desde su botón hasta «Level{N}Card».</summary>
        private static FramedIllustration WindowFor(LevelSelectController controller, LevelId level)
        {
            var card = controller.ButtonFor(level).transform;
            while (card != null && !card.name.EndsWith("Card"))
            {
                card = card.parent;
            }

            return card != null ? card.GetComponentInChildren<FramedIllustration>(true) : null;
        }

        /// <summary>El rectángulo en píxeles de pantalla: el Canvas de la escena es Screen Space - Overlay.</summary>
        private static Rect ScreenRectOf(RectTransform rectTransform) => IllustrationProbe.WorldRect(rectTransform);

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
