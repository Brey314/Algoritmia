using System;
using System.Collections.Generic;
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
    public class MainMenuTests
    {
        private const string SceneName = "MainMenu";
        private static readonly string[] Options = { "Jugar", "Créditos", "Progreso del equipo", "Salir" };

        /// <summary>El lema que queda solo en la tarjeta del título (D10, 08/10/2026).</summary>
        private const string Tagline = "Piensa el orden, enciende el fuego";

        /// <summary>Los retratos con que se reconoce a los cinco de la portada (D9): la familia y Algoritm de fuego.</summary>
        private static readonly string[] PortraitCast =
        {
            "char_papa_retrato_neutra", "char_mama_retrato_neutra", "char_nina_retrato_neutra",
            "char_nino_retrato_neutra", "char_algoritm_n1_fuego_reposo"
        };

        /// <summary>Separación mínima entre dos fases de reposo, en fracción del ciclo: con menos se respira casi igual.</summary>
        private const float MinimumPhaseGap = 0.1f;

        /// <summary>Un píxel del lienzo duplicado (3840 de ancho) en fracción de su ancho: lo que se perdona al decir «no cruza el eje».</summary>
        private const float SeamTolerance = 1f / 3840f;

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
        public async Task MainMenu_RF46_OfreceLaOpcionDeProgresoDelDocente()
        {
            await LoadMainMenu();

            // Junto a Jugar y Créditos, no dentro de una partida (RF-46, RF-01).
            Assert.That(FindOption("Progreso del equipo"), Is.Not.Null);
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
            Click(FindOption("Cerrar el juego"));

            Assert.That(order, Is.EqualTo(new[] { "guardar", "cerrar" }));
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_HU18_SalirPideConfirmacionAntesDeCerrar()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();
            var order = new List<string>();
            sut.Saver = new SpyProfileSaver(() => order.Add("guardar"));
            sut.Quit = () => order.Add("cerrar");

            Click(FindOption("Salir"));

            Assert.That(sut.ExitConfirmPanel.activeSelf, Is.True, "la confirmación no se abrió");
            Assert.That(TextContaining("ya está guardado"), Is.Not.Null,
                "el aviso no dice que lo logrado ya está guardado");
            Assert.That(order, Is.Empty, "«Salir» guardó o cerró antes de confirmar");
        }

        // DEF-SPER-01: «Cerrar el juego» llevaba la papelera (borrar) y su rótulo se partía en dos
        // líneas, fuera del botón. Rojo esperado sin la corrección: aparece un hijo «Icono» y el
        // rótulo mide más que su caja.
        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RF09_CerrarElJuegoNoLlevaPapeleraYSuRotuloCabeEnUnaLinea()
        {
            await LoadMainMenu();
            Click(FindOption("Salir"));
            await Awaitable.NextFrameAsync();
            Canvas.ForceUpdateCanvases();

            var cerrar = FindOption("Cerrar el juego");
            var rotulo = cerrar.GetComponentInChildren<Text>();

            Assert.That(cerrar.GetComponentsInChildren<Image>(true).Select(imagen => imagen.name), Has.None.EqualTo("Icono"),
                "salir no se parece a borrar: sin papelera");
            Assert.That(rotulo.preferredWidth, Is.LessThanOrEqualTo(rotulo.rectTransform.rect.width), "una sola línea");
            var boton = ScreenRectOf(cerrar);
            var caja = ScreenRectOf(rotulo.rectTransform);
            Assert.That(caja.xMin, Is.GreaterThanOrEqualTo(boton.xMin), "el rótulo no sale por la izquierda");
            Assert.That(caja.xMax, Is.LessThanOrEqualTo(boton.xMax), "ni por la derecha");
            Assert.That(IsReachableByRaycast(cerrar), Is.True, "y se alcanza con un clic");
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_HU18_QuedarmeVuelveAlMenuSinGuardarNiCerrar()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();
            var order = new List<string>();
            sut.Saver = new SpyProfileSaver(() => order.Add("guardar"));
            sut.Quit = () => order.Add("cerrar");

            Click(FindOption("Salir"));
            Click(FindOption("Quedarme"));

            Assert.That(sut.ExitConfirmPanel.activeSelf, Is.False, "«Quedarme» no cerró la confirmación");
            Assert.That(sut.MainPanel.activeSelf, Is.True, "«Quedarme» no dejó ver el menú principal");
            Assert.That(order, Is.Empty, "«Quedarme» guardó o cerró la aplicación");
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_HU18_AdvierteLaRutaDeRespaldoAntesDeCerrar()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();
            var quitCalled = false;
            sut.Quit = () => quitCalled = true;
            sut.FallbackDirectory = () => "C:/Usuario/AppData/Juego";

            Click(FindOption("Salir"));

            Assert.That(sut.FallbackNoticeLabel.gameObject.activeSelf, Is.True,
                "no avisó de la ruta de respaldo");
            Assert.That(sut.FallbackNoticeLabel.text, Does.Contain(sut.TitleConfig.FallbackSaveNotice));
            Assert.That(sut.FallbackNoticeLabel.text, Does.Contain(@"C:\Usuario\AppData\Juego"));
            Assert.That(quitCalled, Is.False, "avisar de la ruta ya cerró la aplicación");
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_HU18_NoMuestraAvisoDeRespaldoConDatosEscribible()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();
            sut.FallbackDirectory = () => null;

            Click(FindOption("Salir"));

            Assert.That(sut.FallbackNoticeLabel.gameObject.activeSelf, Is.False,
                "avisó de una ruta de respaldo que no existe");
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_HU18_ElAvisoDeRespaldoConUnaRutaLargaNoTapaLosBotones()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();
            // La ruta real de respaldo (RNF-07): AppData\LocalLow\<compañía>\<producto> en Windows.
            sut.FallbackDirectory = () => Application.persistentDataPath;

            Click(FindOption("Salir"));
            Canvas.ForceUpdateCanvases();

            var label = sut.FallbackNoticeLabel;
            Assert.That(label.gameObject.activeSelf, Is.True, "no avisó de la ruta de respaldo");
            Assert.That(label.cachedTextGenerator.characterCountVisible, Is.EqualTo(label.text.Length),
                "con la ruta real el texto entero sigue siendo visible: nada se trunca de golpe");

            var noticeRect = ScreenRectOf((RectTransform)label.transform);
            var stayButton = FindOption("Quedarme");
            Assert.That(stayButton, Is.Not.Null, "«Quedarme» no está alcanzable: el aviso pudo taparlo");
            var stayRect = ScreenRectOf(stayButton);
            Assert.That(noticeRect.yMin, Is.GreaterThanOrEqualTo(stayRect.yMax),
                "el aviso de la ruta de respaldo se monta sobre «Quedarme»");
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

        // D9 (08/10/2026): la portada son los cinco en el claro del bosque del Nivel 2, parados y sin respirar a la vez.
        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RF01_LosCincoPersonajesEsperanEnReposoSinRespirarAlUnisono()
        {
            await LoadMainMenu();

            var rigs = Object.FindObjectsByType<CharacterRig>(FindObjectsInactive.Exclude);
            var phases = rigs.Select(rig => rig.IdlePhase).OrderBy(phase => phase).ToArray();
            var gaps = phases.Zip(phases.Skip(1).Append(phases[0] + 1f), (previous, next) => next - previous).ToArray();

            Assert.That(rigs.Select(rig => rig.Portrait.name), Is.EquivalentTo(PortraitCast),
                "Papá, Mamá, Niño, Niña y Algoritm de fuego, uno de cada");
            Assert.That(rigs.Select(rig => rig.Current), Has.All.EqualTo(ActorAction.Idle), "todos en reposo");
            Assert.That(rigs.Select(rig => rig.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(nameof(ActorAction.Idle))),
                Has.All.True, "y su Animator también está en el estado de reposo");
            Assert.That(gaps, Has.All.GreaterThanOrEqualTo(MinimumPhaseGap),
                "cada uno arranca su reposo en otra fase del ciclo (el mismo arranque los haría respirar al unísono)");
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RF01_ElBosqueNoCruzaLaCosturaDelLienzo()
        {
            await LoadMainMenu();
            var window = Object.FindAnyObjectByType<FramedIllustration>();

            var (left, right) = IllustrationProbe.VisibleRange(window);

            Assert.That(window.Art.sprite.name, Is.EqualTo("env_n2_bosque_claro"), "la portada es el bosque del Nivel 2");
            Assert.That(right, Is.LessThanOrEqualTo(IllustrationFraming.MirrorAxis + SeamTolerance),
                "la ventana termina antes del eje del espejo del lienzo (x = 0,5): ahí empieza la costura");
            Assert.That(left, Is.GreaterThanOrEqualTo(-SeamTolerance), "y no descubre el borde izquierdo del lienzo");
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RF01_LosCincoPersonajesQuedanDentroDeLaPortada()
        {
            await LoadMainMenu();
            var window = ScreenRectOf((RectTransform)Object.FindAnyObjectByType<FramedIllustration>().transform);

            var cells = Object.FindObjectsByType<CharacterRig>(FindObjectsInactive.Exclude)
                .Select(rig => ScreenRectOf((RectTransform)rig.transform)).ToArray();

            Assert.That(cells, Has.Length.EqualTo(PortraitCast.Length));
            Assert.That(cells, Has.All.Matches<Rect>(cell =>
                cell.xMin >= window.xMin - 1f && cell.xMax <= window.xMax + 1f
                && cell.yMin >= window.yMin - 1f && cell.yMax <= window.yMax + 1f),
                "la casilla de cada personaje cabe entera en la ventana: ninguno queda recortado por la máscara");
        }

        // D10 (08/10/2026): el nombre del juego sale de la tarjeta y va encima; la tarjeta, más corta, queda con el lema;
        // y el bloque entero —título, tarjeta y botones— está centrado en vertical.
        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RF01_TituloTarjetaYBotonesQuedanCentradosEnVertical()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();
            var cardTransform = TextContaining(Tagline).transform.parent.parent;
            var title = ScreenRectOf(sut.TitleLabel.rectTransform);
            var card = ScreenRectOf((RectTransform)cardTransform);
            var buttons = Options.Select(FindOption).Select(ScreenRectOf).ToArray();
            var block = buttons.Append(title).Append(card).Aggregate(IllustrationProbe.Union);

            Assert.That(sut.TitleLabel.transform.IsChildOf(cardTransform), Is.False, "el nombre del juego ya no va dentro de la tarjeta");
            Assert.That(title.yMin, Is.GreaterThanOrEqualTo(card.yMax), "el título va encima de la tarjeta");
            Assert.That(card.yMin, Is.GreaterThanOrEqualTo(buttons.Max(button => button.yMax)), "y la tarjeta encima de los botones");
            Assert.That(Screen.height - block.yMax, Is.EqualTo(block.yMin).Within(1f),
                "el bloque deja tanto espacio encima del título como debajo de los botones");
        }

        [Test]
        [Timeout(20000)]
        public async Task MainMenu_RNF13_SusBotonesNoLanzanSiLaEscenaSeAbreSinPasarPorBoot()
        {
            await LoadMainMenu();
            var sut = Object.FindAnyObjectByType<MainMenuController>();
            // «Salir» no puede cerrar el reproductor a mitad de la suite.
            sut.Quit = () => { };
            // Sin GameFlowRunner —la escena se cargó sin pasar por Boot— «Jugar», «Créditos» y
            // «Progreso del equipo» avisan y no navegan; «Salir» abre la confirmación sin
            // necesitar flujo, y confirmarla («Cerrar el juego») tampoco lo necesita.
            LogAssert.Expect(LogType.Warning, MissingFlow);
            LogAssert.Expect(LogType.Warning, MissingFlow);
            LogAssert.Expect(LogType.Warning, MissingFlow);

            foreach (var label in Options)
            {
                Click(FindOption(label));
            }

            Click(FindOption("Cerrar el juego"));

            // Lo que no puede pasar es que el clic lance: cualquier excepción registrada aquí
            // sería un mensaje no esperado y esta llamada la delata.
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        [Timeout(20000)]
        [Category("VisualVerification")]
        [Description("Tras ejecutar esta prueba, revisar la captura: contraste entre el texto " +
                     "(título y botones) y su fondo —el título es blanco sobre el ocre y se lee por su " +
                     "reborde carbón de 4 px, que debe verse continuo—, que los glifos con tilde y «¿ ¡» " +
                     "se dibujen, que los cinco personajes de la portada se vean enteros y que nada se " +
                     "recorte ni deforme.")]
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

        /// <summary>El texto visible (activo) cuyo contenido incluye <paramref name="fragment"/>.</summary>
        private static Text TextContaining(string fragment) => Object
            .FindObjectsByType<Text>(FindObjectsInactive.Exclude)
            .FirstOrDefault(text => text.text.Contains(fragment));

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
