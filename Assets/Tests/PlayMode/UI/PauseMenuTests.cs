using System;
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
            var slider = Object.FindAnyObjectByType<Slider>(FindObjectsInactive.Include);
            slider.value = VeryClosePosition;
            ArrangeFire();
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
            var slider = Object.FindAnyObjectByType<Slider>(FindObjectsInactive.Include);
            slider.value = VeryClosePosition;
            ArrangeFire();
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
            var slider = Object.FindAnyObjectByType<Slider>(FindObjectsInactive.Include);
            var instruccionInicial = LogLines().text;
            slider.value = VeryClosePosition;
            ArrangeFire();
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
        private static void ArrangeFire()
        {
            var suelo = GameObject.Find("Suelo").transform;
            var punto = ((RectTransform)suelo.Find("PuntoDeFuego")).anchoredPosition;
            foreach (RectTransform pieza in suelo)
            {
                if (pieza.name != "PuntoDeFuego")
                {
                    pieza.anchoredPosition = punto;
                }
            }
        }
    }
}
