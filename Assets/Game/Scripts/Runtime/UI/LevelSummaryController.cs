using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// El resumen de fin de nivel (RF-45, HU-14 paso 6): una sola pantalla, sin cifras, que
    /// describe lo que hizo el estudiante. Al continuar confirma la fase, desbloquea el nivel
    /// siguiente y guarda (HU-14 paso 7) — antes vivía en <c>FirePanelController</c>, pero eso
    /// confirmaba la fase antes de que se reprodujera la escena narrativa de cierre y hacía que
    /// «ya vista» diera verdadero desde la primerísima vez (CP-07). Aquí llega después de esa
    /// escena, así que la primera vez todavía no está confirmada cuando toca decidirlo.
    /// </summary>
    public class LevelSummaryController : MonoBehaviour
    {
        [SerializeField] private LevelSummaryMessages messages;
        [SerializeField] private Text bodyLabel;
        [SerializeField] private Button continueButton;

        internal GameFlowRunner Runner { get; set; }

        /// <summary>Override de prueba para el guardado, patrón de <c>MainMenuController</c>.</summary>
        internal IProfileSaver Saver { get; set; }

#if UNITY_INCLUDE_TESTS
        internal Text BodyLabel => bodyLabel;
        internal Button ContinueButton => continueButton;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            continueButton.onClick.AddListener(Continue);
            Show();
        }

        /// <summary>
        /// Confirma la fase, desbloquea y guarda, y pinta el resumen (HU-14 paso 6-7). Se separa
        /// de <c>Start</c> porque el runner se inyecta después de cargar la escena en las pruebas.
        /// </summary>
        internal void Show()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            var flow = Runner.Flow;
            if (flow.ActiveProfile == null || flow.PlayingLevel is not { } level)
            {
                Debug.LogWarning(
                    "«LevelSummary» se abrió sin perfil o nivel activo: no hay nada que resumir " +
                    "ni confirmar.", this);
                return;
            }

            var indicators = Runner.PendingIndicators;
            flow.ActiveProfile.ConfirmPhase(level, flow.PlayingPhase, indicators);
            LevelUnlockPolicy.UnlockAfterCompleting(flow.ActiveProfile, level);
            (Saver ?? Runner.Session).SaveActive();

            bodyLabel.text = LevelSummaryComposer.Compose(messages, indicators);
        }

        private void Continue()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            Runner.GoTo(GameState.LevelSelect);
        }
    }
}
