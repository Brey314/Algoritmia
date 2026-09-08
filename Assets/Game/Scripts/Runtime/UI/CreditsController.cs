using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Pantalla de créditos (RF-08). Se llega desde la pantalla de inicio y se vuelve a ella.
    /// El texto se lee de un <see cref="CreditsContent"/> (RNF-18); esta clase solo lo pinta.
    /// </summary>
    public class CreditsController : MonoBehaviour
    {
        [SerializeField] private CreditsContent content;
        [SerializeField] private Text bodyLabel;
        [SerializeField] private Button backButton;

        internal GameFlowRunner Runner { get; set; }

#if UNITY_INCLUDE_TESTS
        internal Text BodyLabel => bodyLabel;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            if (content != null && bodyLabel != null)
            {
                bodyLabel.text = content.Body;
            }

            backButton.onClick.AddListener(BackToMainMenu);
        }

        private void BackToMainMenu()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            Runner.GoTo(GameState.MainMenu);
        }
    }
}
