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

        // N1_Config (Fase 5): fuerza efectiva 7–8 de 10, mínimo de golpes efectivos = 3.
        private const int MinimumEffectiveStrikes = 3;
        private const int SoftForce = 2;
        private const int HardForce = 10;
        private const int EffectiveForce = 7;

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
            var mensaje = RectNamed("Mensaje");

            Assert.That(ReachableByRaycast(controller.ForceSlider.GetComponent<RectTransform>()), Is.True,
                "deslizante de fuerza alcanzable");
            Assert.That(strike is not null && ReachableByRaycast(strike.GetComponent<RectTransform>()), Is.True,
                "botón «Golpear» alcanzable");
            // La tablilla no se pulsa: basta con que esté visible y dentro del panel, sin nada encima.
            Assert.That(mensaje is not null && mensaje.gameObject.activeInHierarchy && IsWithin(mensaje, RectNamed("Panel")), Is.True,
                "tablilla de mensajes visible dentro del panel");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF15_MoverElDeslizanteNoEjecutaNingunGolpe()
        {
            var controller = await LoadPanel();
            var slider = controller.ForceSlider;
            var antes = LogLines().text;

            slider.value = EffectiveForce;
            slider.value = HardForce;
            slider.value = SoftForce;
            await Awaitable.NextFrameAsync();

            Assert.That(LogLines().text, Is.EqualTo(antes), "la tablilla sigue con la instrucción: nada se ejecutó");
            Assert.That(controller.Attempt?.EffectiveStrikes ?? 0, Is.EqualTo(0),
                "ningún golpe efectivo contabilizado");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF16_GolpearEscribeEnElRegistroElMensajeDeLaFuerza()
        {
            var controller = await LoadPanel();
            controller.ForceSlider.value = SoftForce;
            var instruccion = LogLines().text;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var trasPrimerGolpe = LogLines().text;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var trasSegundoGolpe = LogLines().text;

            Assert.That(trasPrimerGolpe, Is.Not.EqualTo(instruccion).And.Not.Empty, "el primer golpe escribe el mensaje de la fuerza");
            Assert.That(controller.Log.Entries, Has.Count.EqualTo(2), "el segundo golpe acumula una segunda entrada");
            Assert.That(trasSegundoGolpe, Is.EqualTo(controller.Log.Latest), "y la tablilla muestra la última");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF19_SoplarSeHabilitaAlAlcanzarElMinimoDeGolpesEfectivos()
        {
            var controller = await LoadPanel();
            controller.ForceSlider.value = EffectiveForce;
            Arrange(controller);

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
        public async Task FirePanel_RF15_ElDeslizanteTieneDiezMuescasYElAsaCreceYEnrojeceConLaFuerza()
        {
            var controller = await LoadPanel();
            var slider = controller.ForceSlider;
            var feedback = slider.GetComponent<ForceSliderFeedback>();
            Assume.That(feedback, Is.Not.Null, "el deslizante lleva ForceSliderFeedback");

            Assert.That(slider.wholeNumbers, Is.True, "muescas enteras");
            Assert.That(slider.minValue, Is.EqualTo(0f));
            Assert.That(slider.maxValue, Is.EqualTo(10f), "diez muescas (N1_Config.ForceLevels)");

            slider.value = slider.minValue;
            await Awaitable.NextFrameAsync();
            var escalaSuave = feedback.Knob.localScale.x;
            var colorSuave = feedback.KnobFace.color;

            slider.value = slider.maxValue;
            await Awaitable.NextFrameAsync();
            var escalaFuerte = feedback.Knob.localScale.x;
            var colorFuerte = feedback.KnobFace.color;

            // Doble indicador (RNF-19): tamaño y color, no solo color.
            Assert.That(escalaFuerte, Is.GreaterThan(escalaSuave), "el asa crece con la fuerza");
            Assert.That(colorFuerte.r - colorFuerte.g, Is.GreaterThan(colorSuave.r - colorSuave.g),
                "y se vuelve más roja: el rojo domina más sobre el verde");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF13_PistaADemandaEscribeLaInstruccionEnElRegistro()
        {
            var controller = await LoadPanel();
            Assume.That(controller.HintButton, Is.Not.Null, "la escena debe traer el botón «Pista» cableado");

            Click(controller.HintButton);
            await Awaitable.NextFrameAsync();

            Assert.That(controller.Log.Entries, Has.Count.EqualTo(1), "una sola entrada: la instrucción del paso activo");
            Assert.That(LogLines().text, Is.EqualTo(controller.Log.Latest), "y la tablilla la muestra");
            Assert.That(LogLines().text, Does.Not.Match(@"\d"), "la ayuda no nombra la fuerza correcta (CP-06)");
            Assert.That(controller.Attempt.EffectiveStrikes, Is.EqualTo(0), "pedir ayuda no golpea");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF13_TrasTresFallosSeguidosElRegistroSumaLaPista()
        {
            var controller = await LoadPanel();
            controller.ForceSlider.value = SoftForce;

            ClickTimes(controller.StrikeButton, 2);
            await Awaitable.NextFrameAsync();
            var trasDosFallos = controller.Log.Entries.Count;

            Click(controller.StrikeButton);
            await Awaitable.NextFrameAsync();
            var trasTresFallos = controller.Log.Entries;

            Assert.That(trasDosFallos, Is.EqualTo(2), "dos fallos: solo los dos mensajes de la fuerza");
            Assert.That(trasTresFallos, Has.Count.EqualTo(4), "el tercer fallo seguido añade la pista (N1_Config: 3)");
            Assert.That(controller.Log.Latest, Does.Not.Match(@"\d"), "la pista no nombra la fuerza efectiva (CP-06)");
            Assert.That(LogLines().text, Is.EqualTo(controller.Log.Latest), "y la tablilla muestra la pista");
        }

        [Test]
        [Timeout(20000)]
        public async Task FirePanel_RF19_SoplarAtenuadoNoRespondeNiRegistraError()
        {
            var controller = await LoadPanel();
            Assume.That(controller.BlowButton.interactable, Is.False);
            var antes = LogLines().text;

            Click(controller.BlowButton);
            await Awaitable.NextFrameAsync();

            Assert.That(LogLines().text, Is.EqualTo(antes), "la tablilla no cambia");
            Assert.That(controller.Log.Entries, Is.Empty);
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
        public async Task FirePanel_RF17_LaTablillaMuestraElUltimoMensajeSinDesbordarTrasDiezIntentos()
        {
            var controller = await LoadPanel();
            controller.ForceSlider.value = SoftForce;

            ClickTimes(controller.StrikeButton, 10);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var texto = LogLines();
            // Diez mensajes de la fuerza más una pista cada tres fallos seguidos (RF-13, N1_Config).
            Assert.That(controller.Log.Entries, Has.Count.EqualTo(10 + 10 / 3), "trece entradas registradas");
            Assert.That(texto.text, Is.EqualTo(controller.Log.Latest), "la tablilla muestra solo el último mensaje");
            Assert.That(texto.preferredHeight, Is.LessThanOrEqualTo(texto.rectTransform.rect.height + 0.5f),
                "y cabe en ella sin desbordar");
            Assert.That(IsWithin(texto.rectTransform, RectNamed("Panel")), Is.True, "la tablilla queda dentro del panel");
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

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FireLevel_RF20_SoplarEncadenaAnimacionYEscenaDeCierre()
        {
            var (controller, runner) = await LoadPanelWithProfile(NewProfile());

            ConvergeAndBlow(controller);
            var llego = await WaitUntilAsync(() => runner.Flow.Current == GameState.Narrative, 5f);

            Assert.That(llego, Is.True, "el flujo no llegó a Narrative tras soplar");
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N1_NacimientoDelFuego"));
            Assert.That(runner.PendingIndicators.StepsUsed, Is.EqualTo(MinimumEffectiveStrikes),
                "FirePanelController ya no confirma ni guarda, pero sigue calculando los " +
                "indicadores y dejándolos listos para LevelSummary (T18)");
        }

        // FireLevel_RF04_GuardaAlCompletarLaFase y FireLevel_RF03_DesbloqueaElNivel2 se trasladaron
        // a LevelSummaryTests (T18): CompleteLevel ya no confirma/desbloquea/guarda, solo calcula
        // los indicadores y los deja en Runner.PendingIndicators — ese efecto ahora ocurre en
        // LevelSummaryController, después de la escena narrativa de cierre (CP-07).

        [Test]
        [Timeout(20000)]
        [Category("VisualVerification")]
        [Category("Acceptance")]
        [Description("Tras ejecutar esta prueba, revisar la captura: la transición de color del " +
                     "nacimiento del fuego es una única transición gradual, sin parpadeo ni " +
                     "oscilación (RNF-21).")]
        public async Task FireLevel_RNF21_SinDestellosDeAltaFrecuencia()
        {
            var (controller, _) = await LoadPanelWithProfile(NewProfile());

            ConvergeAndBlow(controller);
            for (var i = 0; i < 10; i++)
            {
                await Awaitable.NextFrameAsync();
            }

            CaptureScreenshot("FireLevel_RNF21_ConvergenciaAMitad");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_CT06_LasPiezasSeArrastranConClicSostenidoYNoSalenDelSuelo()
        {
            var controller = await LoadPanel();
            var hoja = Array.Find(controller.Pieces, piece => piece.Kind == PieceKind.Leaf);
            Assume.That(hoja, Is.Not.Null, "la escena trae hojas arrastrables");
            var rect = (RectTransform)hoja.transform;
            var antes = rect.anchoredPosition;

            var origen = RectTransformUtility.WorldToScreenPoint(null, rect.position);
            var destino = origen + new Vector2(120f, -80f);
            ExecuteEvents.Execute(hoja.gameObject, Pointer(origen, origen), ExecuteEvents.beginDragHandler);
            ExecuteEvents.Execute(hoja.gameObject, Pointer(destino, origen), ExecuteEvents.dragHandler);
            ExecuteEvents.Execute(hoja.gameObject, Pointer(destino, origen), ExecuteEvents.endDragHandler);
            await Awaitable.NextFrameAsync();

            Assert.That(rect.anchoredPosition, Is.Not.EqualTo(antes), "la hoja siguió al puntero");
            var suelo = (RectTransform)rect.parent;
            hoja.MoveTo(new Vector2(9999f, -9999f));
            Assert.That(Mathf.Abs(rect.anchoredPosition.x), Is.LessThanOrEqualTo(suelo.rect.width / 2f + 0.5f),
                "y nunca sale del suelo por el lado");
            Assert.That(Mathf.Abs(rect.anchoredPosition.y), Is.LessThanOrEqualTo(suelo.rect.height / 2f + 0.5f),
                "ni por abajo");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_RF16_ConLasPiedrasLejosDeLasHojasElGolpeNoPrendeNiPenaliza()
        {
            var controller = await LoadPanel();
            controller.ForceSlider.value = EffectiveForce;
            Assume.That(controller.StonesNear, Is.False, "al abrir, sílex y pedernal están regados lejos del punto del fuego");

            ClickTimes(controller.StrikeButton, MinimumEffectiveStrikes);
            await Awaitable.NextFrameAsync();

            Assert.That(controller.Attempt.EffectiveStrikes, Is.Zero, "con las piedras lejos ninguna fuerza prende");
            Assert.That(controller.BlowButton.interactable, Is.False, "y «Soplar» sigue bloqueado");
            Assert.That(LogLines().text, Is.EqualTo(controller.Log.Latest).And.Not.Empty, "la tablilla cuenta lo que pasó");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FirePanel_RF19_SoplarConLasHojasRegadasNoPrendeElFuegoYNoPenaliza()
        {
            var (controller, runner) = await LoadPanelWithProfile(NewProfile());
            controller.ForceSlider.value = EffectiveForce;
            Arrange(controller);
            ClickTimes(controller.StrikeButton, MinimumEffectiveStrikes);
            Assume.That(controller.BlowButton.interactable, Is.True, "convergió con todo en su sitio");

            // Una hoja se va lejos: hay convergencia pero ya no hay montón.
            Array.Find(controller.Pieces, piece => piece.Kind == PieceKind.Leaf).MoveTo(new Vector2(700f, -400f));
            Assume.That(controller.LeavesPiled, Is.False);
            var entradasAntes = controller.Log.Entries.Count;

            Click(controller.BlowButton);
            for (var i = 0; i < 5; i++)
            {
                await Awaitable.NextFrameAsync();
            }

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing), "no nace el fuego: sigue jugando");
            Assert.That(controller.Log.Entries, Has.Count.EqualTo(entradasAntes + 1), "se describe lo que pasó");
            Assert.That(controller.BlowButton.interactable, Is.True, "y «Soplar» no se bloquea: lo ganado permanece (CP-02)");
            Assert.That(controller.Attempt.EffectiveStrikes, Is.EqualTo(MinimumEffectiveStrikes));
        }

        // --- helpers -----------------------------------------------------------------------

        /// <summary>Deja hojas y piedras en el punto del fuego: la disposición correcta (T22).</summary>
        private static void Arrange(FirePanelController controller)
        {
            foreach (var piece in controller.Pieces)
            {
                piece.MoveTo(controller.FireSpot.anchoredPosition);
            }
        }

        private static PointerEventData Pointer(Vector2 position, Vector2 pressPosition) =>
            new PointerEventData(EventSystem.current) { position = position, pressPosition = pressPosition };

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
            // del recién cargado, y una prueba se queda comparando contra el registro de otra
            // (medido el 11/09/2026: mensajes de `FirePanel_RF16_…` colándose en otras pruebas).
            await Awaitable.NextFrameAsync();

            // Assert y no un simple FindAnyObjectByType: si alguna vez vuelve a haber dos
            // controladores vivos a la vez, esto tiene que fallar fuerte y decir por qué, no
            // devolver uno cualquiera en silencio.
            var controllers = Object.FindObjectsByType<FirePanelController>(FindObjectsInactive.Include);
            Assert.That(controllers.Length, Is.EqualTo(1),
                $"se esperaba un único FirePanelController vivo, había {controllers.Length}");
            return controllers[0];
        }

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        /// <summary>
        /// Variante de <see cref="LoadPanel"/> con un <see cref="GameFlowRunner"/> real y un perfil
        /// activo (T15): las tres pruebas de convergencia necesitan `Flow` para observar a dónde
        /// salta «Soplar» y a quién guarda/desbloquea.
        /// </summary>
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

        /// <summary>Marca «Muy cerca», converge con el mínimo de golpes efectivos y pulsa «Soplar».</summary>
        private static void ConvergeAndBlow(FirePanelController controller)
        {
            controller.ForceSlider.value = EffectiveForce;
            Arrange(controller);
            ClickTimes(controller.StrikeButton, MinimumEffectiveStrikes);
            Click(controller.BlowButton);
        }

        /// <summary>
        /// Sondea <paramref name="condition"/> cuadro a cuadro hasta que se cumpla o venza el
        /// tiempo real: la animación de convergencia (~0.6 s) corre en tiempo real, así que un
        /// ajuste futuro de su duración no puede romper estas pruebas (Testability, plan T15).
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
            Object.FindObjectsByType<Text>(FindObjectsInactive.Include), t => t.name == "InstruccionLabel");

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
