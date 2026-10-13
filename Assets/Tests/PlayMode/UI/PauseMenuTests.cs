using System;
using System.Collections.Generic;
using System.Linq;
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
    // No referencia Game.Levels.Fire (ni su .asmdef ni su InternalsVisibleTo): el panel del
    // Nivel 1 se conduce por estructura —botones por etiqueta, el registro por nombre—, igual
    // que PauseMenuController lo trata desde Game.UI (plan T16, «ningún .asmdef nuevo ni editado»).
    [Category("Integration")]
    public class PauseMenuTests
    {
        private const string SceneName = "Level1_Cave";
        private const string NarrativeSceneName = "Narrative";

        // N1_Config (Fase 5): el deslizante mide fuerza, 7 cae en la franja efectiva.
        private const float VeryClosePosition = 7f; // fuerza efectiva (N1_Config, Fase 5)
        private const float EffectiveSpacing = 5f; // la única muesca efectiva (N1_Config)
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
        [Category("Acceptance")]
        public async Task PauseMenu_HU17_ContinuarRestituyeElEstadoExacto()
        {
            var (pauseMenu, _) = await LoadPauseMenuWithProfile(NewProfile());
            await ReunirAsync();
            var slider = SliderNamed("FuerzaSlider");
            slider.value = VeryClosePosition;
            SliderNamed("CercaniaSlider").value = EffectiveSpacing;
            ClickTimes(ButtonWithLabel("Golpear"), 2);
            await Awaitable.NextFrameAsync();

            var registroAntes = LogLines().text;
            var soplarAntes = ButtonWithLabel("Soplar").interactable;
            var deslizanteAntes = slider.value;

            Click(pauseMenu.PauseButton);
            await Awaitable.NextFrameAsync();
            Assert.That(pauseMenu.Overlay.activeSelf, Is.True, "la pausa no se abrió");
            Assert.That(pauseMenu.BaseButtons.activeSelf, Is.True,
                "los botones base no aparecieron al abrir la pausa");

            Click(pauseMenu.ContinueButton);
            await Awaitable.NextFrameAsync();

            Assert.That(pauseMenu.Overlay.activeSelf, Is.False, "«Continuar» no cerró la pausa");
            Assert.That(LogLines().text, Is.EqualTo(registroAntes), "el registro cambió al continuar");
            Assert.That(ButtonWithLabel("Soplar").interactable, Is.EqualTo(soplarAntes),
                "los golpes efectivos contabilizados cambiaron al continuar");
            Assert.That(slider.value, Is.EqualTo(deslizanteAntes),
                "la posición del deslizante cambió al continuar");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task PauseMenu_HU17_ReiniciarPideConfirmacionYCancelarNoCambiaNada()
        {
            var (pauseMenu, _) = await LoadPauseMenuWithProfile(NewProfile());
            await ReunirAsync();
            var slider = SliderNamed("FuerzaSlider");
            slider.value = VeryClosePosition;
            SliderNamed("CercaniaSlider").value = EffectiveSpacing;
            ClickTimes(ButtonWithLabel("Golpear"), 2);
            await Awaitable.NextFrameAsync();

            var registroAntes = LogLines().text;
            var soplarAntes = ButtonWithLabel("Soplar").interactable;

            Click(pauseMenu.PauseButton);
            await Awaitable.NextFrameAsync();
            Click(pauseMenu.RestartButton);
            await Awaitable.NextFrameAsync();

            Assert.That(pauseMenu.ConfirmPanel.activeSelf, Is.True,
                "no apareció la confirmación de «Reiniciar»");
            Assert.That(pauseMenu.BaseButtons.activeSelf, Is.False,
                "los botones base no desaparecieron al pedir confirmación");
            Assert.That(TextNamed("TextoConfirmacion")?.text, Is.Not.Null.And.Not.Empty,
                "el texto de confirmación está vacío");

            Click(pauseMenu.CancelRestartButton);
            await Awaitable.NextFrameAsync();

            Assert.That(pauseMenu.BaseButtons.activeSelf, Is.True,
                "«Cancelar» no restituyó los botones base");
            Assert.That(pauseMenu.ConfirmPanel.activeSelf, Is.False, "«Cancelar» no ocultó la confirmación");
            Assert.That(LogLines().text, Is.EqualTo(registroAntes),
                "el registro cambió tras pedir y cancelar el reinicio");
            Assert.That(ButtonWithLabel("Soplar").interactable, Is.EqualTo(soplarAntes),
                "los golpes efectivos contabilizados cambiaron tras pedir y cancelar el reinicio");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task PauseMenu_HU17_ConfirmarReiniciarReinicioElIntentoSinTocarElProgreso()
        {
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel); // Nivel 2 ya desbloqueado por una convergencia previa (RF-41).

            var (pauseMenu, _) = await LoadPauseMenuWithProfile(profile);
            // El `SceneLoader` se crea después de entrar a Playing la primera vez, no antes: con
            // GameFlowRunner.Apply ya forzando la recarga en cualquier reentrada a Playing (T16),
            // crearlo antes recargaría la escena de más aquí mismo y destruiría `pauseMenu` a
            // mitad de la prueba. Solo hace falta para la recarga real de «Confirmar reiniciar».
            new GameObject("TestSceneLoader").AddComponent<SceneLoader>();
            var instruccionInicial = LogLines().text;
            await ReunirAsync();
            var slider = SliderNamed("FuerzaSlider");
            slider.value = VeryClosePosition;
            SliderNamed("CercaniaSlider").value = EffectiveSpacing;
            ClickTimes(ButtonWithLabel("Golpear"), MinimumEffectiveStrikes);
            await Awaitable.NextFrameAsync();
            Assume.That(LogLines().text, Is.Not.EqualTo(instruccionInicial), "los golpes escribieron en la tablilla");
            Assume.That(ButtonWithLabel("Soplar").interactable, Is.True,
                "el arreglo de la prueba no convergió");

            Click(pauseMenu.PauseButton);
            await Awaitable.NextFrameAsync();
            Click(pauseMenu.RestartButton);
            await Awaitable.NextFrameAsync();
            Click(pauseMenu.ConfirmRestartButton);

            var recargo = await WaitUntilAsync(() =>
            {
                var controllers = Object.FindObjectsByType<PauseMenuController>(FindObjectsInactive.Include);
                return controllers.Length == 1 && controllers[0] != pauseMenu;
            }, 5f);
            Assert.That(recargo, Is.True, "la escena no se recargó al confirmar «Reiniciar»");
            await Awaitable.NextFrameAsync(); // deja correr Start() del panel recién cargado

            var registroTrasReiniciar = LogLines();
            Assert.That(registroTrasReiniciar, Is.Not.Null);
            // La tablilla vuelve a la instrucción: no queda ningún mensaje del intento anterior.
            Assert.That(registroTrasReiniciar.text, Is.EqualTo(instruccionInicial), "la tablilla no volvió a la instrucción tras reiniciar");
            Assert.That(ButtonWithLabel("Soplar").interactable, Is.False,
                "quedó un golpe efectivo contabilizado tras reiniciar");
            Assert.That(profile.IsUnlocked(LevelId.Wheel), Is.True,
                "«Reiniciar» re-bloqueó un nivel ya desbloqueado");
        }

        /// <summary>
        /// HU-17 corregido (INC-49) y ampliado (INC-159, sesión con estudiantes del 13/10/2026): los
        /// cinco botones, en este orden. «Volver a ver la historia» solo se ofrece si la fase en curso
        /// tiene escena, así que la prueba entra a la fase 1 del Nivel 1, que sí la tiene.
        /// </summary>
        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task PauseMenu_INC159_OfreceCincoBotonesEnOrden()
        {
            var (pauseMenu, _) = await LoadPauseMenuWithProfile(NewProfile());

            Click(pauseMenu.PauseButton);
            await Awaitable.NextFrameAsync();

            var buttons = pauseMenu.BaseButtons.GetComponentsInChildren<Button>(includeInactive: true);
            Assert.That(buttons.Select(button => button.GetComponentInChildren<Text>().text), Is.EqualTo(new[]
            {
                "Reanudar", "Reiniciar", "Volver a ver la historia", "Volver al menú de niveles", "Guardar y salir"
            }));
            Assert.That(buttons.Select(button => button.gameObject.activeSelf), Has.All.True,
                "con una fase que tiene historia, los cinco se ven");
            Assert.That(buttons[0], Is.SameAs(pauseMenu.ContinueButton));
            Assert.That(buttons[1], Is.SameAs(pauseMenu.RestartButton));
            Assert.That(buttons[2], Is.SameAs(pauseMenu.HistoryButton));
            Assert.That(buttons[3], Is.SameAs(pauseMenu.LevelSelectButton));
            Assert.That(buttons[4], Is.SameAs(pauseMenu.SaveQuitButton));
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task PauseMenu_INC159_SinFlowNoOfreceVolverAVerLaHistoria()
        {
            var pauseMenu = await LoadPauseMenu(); // la escena suelta, sin Boot: no hay fase en curso

            Click(pauseMenu.PauseButton);
            await Awaitable.NextFrameAsync();

            Assert.That(pauseMenu.HistoryButton.gameObject.activeSelf, Is.False,
                "sin fase con historia el botón se oculta en vez de quedar mudo");
            Assert.That(pauseMenu.BaseButtons.GetComponentsInChildren<Button>(includeInactive: false).Length,
                Is.EqualTo(4));
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task PauseMenu_INC159_VolverAVerLaHistoriaDevuelveElTiempoYEntraALaEscenaDeLaFase()
        {
            var profile = NewProfile();
            var (pauseMenu, runner) = await LoadPauseMenuWithProfile(profile);
            Click(pauseMenu.PauseButton);
            await Awaitable.NextFrameAsync();
            Assert.That(Time.timeScale, Is.EqualTo(0f), "la pausa detiene el tiempo");

            Click(pauseMenu.HistoryButton);
            await Awaitable.NextFrameAsync();

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative));
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N1_Hallazgo"));
            Assert.That(Time.timeScale, Is.EqualTo(1f), "al salir de la pausa se devuelve el tiempo");
            Assert.That(pauseMenu.Overlay.activeSelf, Is.False);
            Assert.That(profile.ConfirmedPhases, Is.Empty, "ver la historia no toca el progreso (RF-41)");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task PauseMenu_INC159_GuardarYSalirPideConfirmacionYGuardaAntesDeCerrar()
        {
            var pauseMenu = await LoadPauseMenu();
            var order = new List<string>();
            pauseMenu.Saver = new RecordingSaver(order);
            pauseMenu.Quit = () => order.Add("quit");

            Click(pauseMenu.PauseButton);
            await Awaitable.NextFrameAsync();
            Click(pauseMenu.SaveQuitButton);
            await Awaitable.NextFrameAsync();

            Assert.That(pauseMenu.QuitConfirmPanel.activeSelf, Is.True, "no apareció la confirmación de «Guardar y salir»");
            Assert.That(pauseMenu.BaseButtons.activeSelf, Is.False);
            Assert.That(pauseMenu.QuitPromptLabel.text,
                Is.EqualTo("¿Quieres salir del juego? Lo que ya terminaste queda guardado."));
            Assert.That(pauseMenu.ConfirmQuitButton.GetComponentInChildren<Text>().text, Is.EqualTo("Sí, salir"));
            Assert.That(pauseMenu.CancelQuitButton.GetComponentInChildren<Text>().text, Is.EqualTo("Cancelar"));

            Click(pauseMenu.CancelQuitButton);
            await Awaitable.NextFrameAsync();

            Assert.That(pauseMenu.QuitConfirmPanel.activeSelf, Is.False, "«Cancelar» cierra la confirmación");
            Assert.That(pauseMenu.BaseButtons.activeSelf, Is.True, "y devuelve los botones base");
            Assert.That(order, Is.Empty, "cancelar no guarda ni sale");

            Click(pauseMenu.SaveQuitButton);
            await Awaitable.NextFrameAsync();
            Click(pauseMenu.ConfirmQuitButton);

            Assert.That(order, Is.EqualTo(new[] { "save", "quit" }), "primero se guarda, después se sale (RF-09)");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task PauseMenu_HU17_NoSeMuestraEnEscenasNarrativas()
        {
            var load = SceneManager.LoadSceneAsync(NarrativeSceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            Assert.That(ButtonWithLabel("Pausa"), Is.Null, "la escena narrativa muestra un botón «Pausa»");
        }

        private sealed class RecordingSaver : IProfileSaver
        {
            private readonly List<string> _order;

            public RecordingSaver(List<string> order) => _order = order;

            public void SaveActive() => _order.Add("save");
        }

        // --- helpers -----------------------------------------------------------------------

        private static async Task<PauseMenuController> LoadPauseMenu()
        {
            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync(); // deja correr PauseMenuController.Start()
            // Mismo motivo que FirePanelTests.LoadPanel: sin este fotograma extra, FindObjectsByType
            // puede devolver el controlador de la prueba anterior en vez del recién cargado.
            await Awaitable.NextFrameAsync();

            var controllers = Object.FindObjectsByType<PauseMenuController>(FindObjectsInactive.Include);
            Assert.That(controllers.Length, Is.EqualTo(1),
                $"se esperaba un único PauseMenuController vivo, había {controllers.Length}");
            return controllers[0];
        }

        /// <summary>
        /// Variante de <see cref="LoadPauseMenu"/> con un <see cref="GameFlowRunner"/> real y un
        /// perfil activo (mismo patrón que <c>FirePanelTests.LoadPanelWithProfile</c>).
        /// </summary>
        private static async Task<(PauseMenuController pauseMenu, GameFlowRunner runner)>
            LoadPauseMenuWithProfile(PlayerProfile profile)
        {
            var pauseMenu = await LoadPauseMenu();

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu

            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(profile);
            runner.StartPlaying(LevelId.Fire, 1);
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing),
                "el flujo no llegó a Playing: el arreglo de la prueba está roto");

            pauseMenu.Runner = runner;
            return (pauseMenu, runner);
        }

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

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

        private static Text TextNamed(string name) => Array.Find(
            Object.FindObjectsByType<Text>(FindObjectsInactive.Include), t => t.name == name);

        // La tablilla superior muestra el último mensaje del registro (Fase 5, T23).
        private static Text LogLines() => TextNamed("InstruccionLabel");

        /// <summary>
        /// Sondea <paramref name="condition"/> cuadro a cuadro hasta que se cumpla o venza el
        /// tiempo real, igual que <c>FirePanelTests.WaitUntilAsync</c>: la recarga de escena no
        /// termina en un número fijo de fotogramas.
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

        /// <summary>Hojas y piedras al punto del fuego: sin eso ningún golpe prende (T22).</summary>
        /// <summary>
        /// Reúne todas las piezas en el punto del fuego y suelta la última con un fin de arrastre
        /// real (T25): el panel pasa al encendido por su propio evento, sin tocar
        /// <c>Game.Levels.Fire</c>. Termina cuando «Golpear» está en pantalla.
        /// </summary>
        private static async Task ReunirAsync()
        {
            var suelo = GameObject.Find("Suelo").transform;
            var punto = (RectTransform)suelo.Find("PuntoDeFuego");
            RectTransform ultima = null;
            foreach (RectTransform pieza in suelo)
            {
                // Solo lo que se arrastra: el montón y la llama también cuelgan de «Suelo» y soltar
                // sobre la llama no reúne nada (la prueba quedaba sin concluir desde el 22/09/2026).
                if (pieza.GetComponent<IEndDragHandler>() != null)
                {
                    pieza.anchoredPosition = punto.anchoredPosition;
                    ultima = pieza;
                }
            }

            var pointer = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(null, punto.position)
            };
            ExecuteEvents.Execute(ultima.gameObject, pointer, ExecuteEvents.endDragHandler);

            var deadline = Time.realtimeSinceStartup + 5f;
            while (!(ButtonWithLabel("Golpear")?.isActiveAndEnabled ?? false) && Time.realtimeSinceStartup < deadline)
            {
                await Awaitable.NextFrameAsync();
            }

            Assume.That(ButtonWithLabel("Golpear").isActiveAndEnabled, Is.True, "el panel pasó al encendido");
            await Awaitable.NextFrameAsync(); // deja correr Awake/Start de la interfaz recién activada
        }

        private static Slider SliderNamed(string name) => Array.Find(
            Object.FindObjectsByType<Slider>(FindObjectsInactive.Include), slider => slider.name == name);
    }
}
