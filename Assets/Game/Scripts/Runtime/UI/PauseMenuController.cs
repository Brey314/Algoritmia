using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// El menú de pausa del Nivel 1 (HU-17): una capa sobre <see cref="GameState.Playing"/>, no un
    /// estado nuevo. Continuar restituye el estado exacto sin tocar nada (RF-07); Reiniciar pide
    /// confirmación de una frase y repite la fase desde cero sin perder progreso ya guardado
    /// (RF-41, CP-02); Volver al menú principal sale del nivel.
    /// </summary>
    /// <remarks>
    /// Vive en <c>Game.UI</c> y no en <c>Game.Levels.Fire</c>: es navegación general, como
    /// <see cref="LevelSelectController"/> o <see cref="NarrativeSceneController"/>, no una regla
    /// del nivel — así ningún assembly de nivel gana una dependencia nueva. El bloqueador de
    /// pantalla completa del overlay intercepta el clic hacia el panel de abajo; este controlador
    /// no toca <c>FirePanelController</c> en absoluto.
    /// </remarks>
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject overlay;
        [SerializeField] private GameObject baseButtons;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private GameObject confirmPanel;
        [SerializeField] private Button confirmRestartButton;
        [SerializeField] private Button cancelRestartButton;

        internal GameFlowRunner Runner { get; set; }

#if UNITY_INCLUDE_TESTS
        internal Button PauseButton => pauseButton;
        internal GameObject Overlay => overlay;
        internal GameObject BaseButtons => baseButtons;
        internal Button ContinueButton => continueButton;
        internal Button RestartButton => restartButton;
        internal Button MainMenuButton => mainMenuButton;
        internal GameObject ConfirmPanel => confirmPanel;
        internal Button ConfirmRestartButton => confirmRestartButton;
        internal Button CancelRestartButton => cancelRestartButton;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            pauseButton.onClick.AddListener(OpenPause);
            continueButton.onClick.AddListener(ClosePause);
            restartButton.onClick.AddListener(RequestRestart);
            cancelRestartButton.onClick.AddListener(CancelRestart);
            confirmRestartButton.onClick.AddListener(ConfirmRestart);
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        private void OpenPause()
        {
            Runner?.ActiveReporter?.PauseOpened(); // RF-07: desde aquí no cuenta el tiempo de resolución.
            overlay.SetActive(true);
            baseButtons.SetActive(true);
            confirmPanel.SetActive(false);
        }

        /// <summary>
        /// «Continuar» y «Volver al menú» (RF-07, HU-17): además de ocultar el overlay, cierra la
        /// ventana de pausa abierta por <see cref="OpenPause"/> para el indicador de resolución.
        /// </summary>
        private void ClosePause()
        {
            overlay.SetActive(false);
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

        private void GoToMainMenu()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            ClosePause();
            Runner.GoTo(GameState.MainMenu);
        }
    }
}
