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
    /// activa** desde cero sin perder progreso ya guardado (RF-41, CP-02, INC-25); «Volver al
    /// menú de niveles» sale del nivel conservando lo confirmado.
    /// </summary>
    /// <remarks>
    /// Vive en <c>Game.UI</c> y no en un assembly de nivel: es navegación general, como
    /// <see cref="LevelSelectController"/> o <see cref="NarrativeSceneController"/>, no una regla
    /// del nivel — así ningún assembly de nivel gana una dependencia nueva. Es el prefab
    /// <c>MenuPausa</c>, el mismo en las cuatro escenas jugables (W17): el bloqueador de pantalla
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

        internal GameFlowRunner Runner { get; set; }

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
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            pauseButton.onClick.AddListener(OpenPause);
            continueButton.onClick.AddListener(ClosePause);
            restartButton.onClick.AddListener(RequestRestart);
            cancelRestartButton.onClick.AddListener(CancelRestart);
            confirmRestartButton.onClick.AddListener(ConfirmRestart);
            levelSelectButton.onClick.AddListener(GoToLevelSelect);
        }

        /// <summary>La escena se descarga con la pausa abierta —«Reiniciar»—: el tiempo no puede quedarse detenido.</summary>
        private void OnDestroy() => Time.timeScale = 1f;

        private void OpenPause()
        {
            Runner?.ActiveReporter?.PauseOpened(); // RF-07: desde aquí no cuenta el tiempo de resolución.
            Time.timeScale = 0f; // HU-17: mientras está abierto, el nivel queda detenido.
            overlay.SetActive(true);
            baseButtons.SetActive(true);
            confirmPanel.SetActive(false);
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
    }
}
