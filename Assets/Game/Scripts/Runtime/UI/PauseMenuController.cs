using System;
using Game.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// El menú de pausa de las escenas jugables (HU-17, mockup 6): una capa sobre
    /// <see cref="GameState.Playing"/>, no un estado nuevo. «Reanudar» restituye el estado exacto
    /// sin tocar nada (RF-07); «Reiniciar» pide confirmación de una frase y repite **la fase
    /// activa** desde cero sin perder progreso ya guardado (RF-41, CP-02, INC-25); «Volver a ver
    /// la historia» lleva a la escena narrativa que presenta la fase y de ahí se vuelve a la misma
    /// fase (INC-159); «Volver al menú de niveles» sale del nivel conservando lo confirmado; y
    /// «Guardar y salir» pide confirmación, guarda el perfil activo y cierra el juego (INC-159).
    /// </summary>
    /// <remarks>
    /// Vive en <c>Game.UI</c> y no en un assembly de nivel: es navegación general, como
    /// <see cref="LevelSelectController"/> o <see cref="NarrativeSceneController"/>, no una regla
    /// del nivel — así ningún assembly de nivel gana una dependencia nueva. Es el prefab
    /// <c>MenuPausa</c>, el mismo en las cinco escenas jugables (W17): el bloqueador de pantalla
    /// completa del overlay intercepta el clic hacia el panel de abajo, y <c>Time.timeScale</c>
    /// a cero detiene lo que se mueve —rodado, ejecución paso a paso— mientras está abierto.
    /// Este controlador no toca ningún controlador de nivel.
    /// </remarks>
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject overlay;
        [SerializeField] private GameObject baseButtons;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartButton;
        [SerializeField, FormerlySerializedAs("mainMenuButton")] private Button levelSelectButton;
        [SerializeField] private GameObject confirmPanel;
        [SerializeField] private Button confirmRestartButton;
        [SerializeField] private Button cancelRestartButton;

        [Header("Volver a ver la historia (INC-159)")]
        [SerializeField] private Button historyButton;

        [SerializeField]
        [Tooltip("Qué escena narrativa presenta cada fase jugable. Una fase sin fila no ofrece el botón «Volver a ver la historia» (el amarre del Nivel 3 arranca sin escena propia). Cada fila se cruza con su NarrativeSequence en las pruebas: el Level y el NextPhase de la escena tienen que ser los de la fila.")]
        private PhaseNarrative[] phaseNarratives = Array.Empty<PhaseNarrative>();

        [Header("Guardar y salir (INC-159)")]
        [SerializeField] private Button saveQuitButton;
        [SerializeField] private GameObject quitConfirmPanel;
        [SerializeField] private Text quitPromptLabel;
        [SerializeField] private Button confirmQuitButton;
        [SerializeField] private Button cancelQuitButton;

        [SerializeField]
        [Tooltip("Textos de los dos botones nuevos y de la confirmación de salida (CT-05). Sin asset, quedan los del prefab.")]
        private PauseMenuContent content;

        internal GameFlowRunner Runner { get; set; }

        /// <summary>
        /// Guarda el perfil activo al salir. Producción: la sesión de <see cref="GameFlowRunner"/>,
        /// que se resuelve solo al confirmar la salida —como en <see cref="MainMenuController"/>—
        /// para que una prueba que no llega a confirmarla no acabe construyendo el almacén en disco.
        /// </summary>
        internal IProfileSaver Saver { get; set; }

        /// <summary>Cierra la aplicación. Producción: <see cref="Application.Quit()"/>.</summary>
        internal Action Quit { get; set; }

#if UNITY_INCLUDE_TESTS
        internal Button PauseButton => pauseButton;
        internal GameObject Overlay => overlay;
        internal GameObject BaseButtons => baseButtons;
        internal Button ContinueButton => continueButton;
        internal Button RestartButton => restartButton;
        internal Button LevelSelectButton => levelSelectButton;
        internal GameObject ConfirmPanel => confirmPanel;
        internal Button ConfirmRestartButton => confirmRestartButton;
        internal Button CancelRestartButton => cancelRestartButton;
        internal Button HistoryButton => historyButton;
        internal PhaseNarrative[] PhaseNarratives => phaseNarratives;
        internal Button SaveQuitButton => saveQuitButton;
        internal GameObject QuitConfirmPanel => quitConfirmPanel;
        internal Text QuitPromptLabel => quitPromptLabel;
        internal Button ConfirmQuitButton => confirmQuitButton;
        internal Button CancelQuitButton => cancelQuitButton;
        internal PauseMenuContent Content => content;
#endif

        private void Awake()
        {
            Runner ??= GameFlowRunner.Instance;
            Quit ??= Application.Quit;
        }

        private void Start()
        {
            ApplyContent();
            pauseButton.onClick.AddListener(OpenPause);
            continueButton.onClick.AddListener(ClosePause);
            restartButton.onClick.AddListener(RequestRestart);
            cancelRestartButton.onClick.AddListener(CancelRestart);
            confirmRestartButton.onClick.AddListener(ConfirmRestart);
            historyButton.onClick.AddListener(WatchStory);
            levelSelectButton.onClick.AddListener(GoToLevelSelect);
            saveQuitButton.onClick.AddListener(RequestQuit);
            cancelQuitButton.onClick.AddListener(CancelQuit);
            confirmQuitButton.onClick.AddListener(ConfirmQuit);
        }

        /// <summary>La escena se descarga con la pausa abierta —«Reiniciar»—: el tiempo no puede quedarse detenido.</summary>
        private void OnDestroy() => Time.timeScale = 1f;

        /// <summary>Pone los textos del asset sobre los rótulos del prefab: el prefab dice lo mismo, el asset manda.</summary>
        private void ApplyContent()
        {
            if (content == null)
            {
                return;
            }

            SetLabel(historyButton, content.HistoryLabel);
            SetLabel(saveQuitButton, content.SaveQuitLabel);
            quitPromptLabel.text = content.QuitPrompt;
            SetLabel(confirmQuitButton, content.ConfirmQuitLabel);
            SetLabel(cancelQuitButton, content.CancelQuitLabel);
        }

        private static void SetLabel(Button button, string text) =>
            button.GetComponentInChildren<Text>(true).text = text;

        private void OpenPause()
        {
            Runner?.ActiveReporter?.PauseOpened(); // RF-07: desde aquí no cuenta el tiempo de resolución.
            Time.timeScale = 0f; // HU-17: mientras está abierto, el nivel queda detenido.
            overlay.SetActive(true);
            baseButtons.SetActive(true);
            confirmPanel.SetActive(false);
            quitConfirmPanel.SetActive(false);

            // Una fase sin escena propia no ofrece el botón en vez de dejarlo mudo: un clic sin
            // efecto es peor que un botón ausente para un niño de cuarto.
            historyButton.gameObject.SetActive(
                Runner != null && PauseMenuPolicy.StoryFor(Runner.Flow, phaseNarratives) != null);
        }

        /// <summary>
        /// «Reanudar» y «Volver al menú de niveles» (RF-07, HU-17): además de ocultar el overlay,
        /// cierra la ventana de pausa abierta por <see cref="OpenPause"/> para el indicador de
        /// resolución y devuelve el tiempo.
        /// </summary>
        private void ClosePause()
        {
            overlay.SetActive(false);
            Time.timeScale = 1f;
            Runner?.ActiveReporter?.PauseClosed();
        }

        private void RequestRestart()
        {
            baseButtons.SetActive(false);
            confirmPanel.SetActive(true);
        }

        /// <summary>«Cancelar»: no cambia nada (HU-17 FA-02).</summary>
        private void CancelRestart()
        {
            confirmPanel.SetActive(false);
            baseButtons.SetActive(true);
        }

        private void ConfirmRestart()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            // La escena se recarga sola (GameFlowRunner.Apply, T16) y este objeto desaparece con
            // ella: no hay nada más que hacer aquí tras pedir el reinicio.
            PauseMenuPolicy.Restart(Runner);
        }

        /// <summary>
        /// «Volver a ver la historia» (INC-159): sale a la escena narrativa de la fase. Sin
        /// confirmación, porque no pierde nada —lo confirmado sigue confirmado (RF-41, CP-02)— y,
        /// como «Reiniciar», la fase en curso se vuelve a empezar al regresar de la historia.
        /// </summary>
        private void WatchStory()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            ClosePause();
            PauseMenuPolicy.WatchStory(Runner, phaseNarratives);
        }

        /// <summary>
        /// «Volver al menú de niveles» (mockup 6). Lo confirmado sigue confirmado: salir no toca
        /// el perfil (RF-41, CP-02), y no hay pantalla de derrota a la que ir.
        /// </summary>
        private void GoToLevelSelect()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            ClosePause();
            Runner.GoTo(GameState.LevelSelect);
        }

        /// <summary>«Guardar y salir» (INC-159): pide confirmación antes de cerrar el juego, como «Reiniciar».</summary>
        private void RequestQuit()
        {
            baseButtons.SetActive(false);
            quitConfirmPanel.SetActive(true);
        }

        /// <summary>«Cancelar»: no cambia nada, igual que <see cref="CancelRestart"/>.</summary>
        private void CancelQuit()
        {
            quitConfirmPanel.SetActive(false);
            baseButtons.SetActive(true);
        }

        /// <summary>
        /// El orden importa (RF-09, el mismo de <see cref="MainMenuController"/>): se guarda y solo
        /// después se cierra. Sin flujo no hay perfil que guardar, pero salir tiene que seguir
        /// funcionando: este botón no navega, así que no pasa por <see cref="ScreenFlow"/>.
        /// </summary>
        private void ConfirmQuit() =>
            PauseMenuPolicy.SaveThenQuit(Saver ?? Runner?.Session, Quit);
    }
}
