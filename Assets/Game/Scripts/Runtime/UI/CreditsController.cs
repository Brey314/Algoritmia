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

        [Header("Rejilla de dos columnas")]
        [SerializeField] private Transform entryList;
        [SerializeField] private GameObject entryPrototype;

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

            PaintEntries();

            backButton.onClick.AddListener(BackToMainMenu);
        }

        /// <summary>
        /// Pinta los pares papel/persona clonando el prototipo. El prototipo se queda apagado y
        /// en la escena: es la plantilla, no una fila más.
        /// </summary>
        private void PaintEntries()
        {
            if (content == null || entryList == null || entryPrototype == null)
            {
                return;
            }

            entryPrototype.SetActive(false);

            foreach (var entry in content.Entries)
            {
                var row = Instantiate(entryPrototype, entryList);
                row.name = $"Entry({entry.Role})";
                row.SetActive(true);
                row.transform.Find("Rol").GetComponent<Text>().text = entry.Role;
                row.transform.Find("Nombre").GetComponent<Text>().text = entry.Name;
            }
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
