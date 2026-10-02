using System.Linq;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Panel de selección de perfil dentro de la pantalla de inicio (HU-01, RF-02). Lista los
    /// perfiles guardados y ofrece crear uno nuevo con un **único** campo de nombre (RNF-09).
    /// Adaptador delgado: la validación y el guardado viven en <see cref="ProfileSession"/>.
    /// </summary>
    public class ProfileSelectController : MonoBehaviour
    {
        // HU-01 FA-03: un perfil nuevo arranca en la escena narrativa de introducción del Nivel 1.
        private const string IntroNarrativeId = "N1_Apertura";

        [SerializeField] private ProfileSelectContent content;
        [SerializeField] private Transform profileList;
        [SerializeField] private Button profileEntryPrototype;
        [SerializeField] private InputField nameField;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Text messageLabel;
        [SerializeField] private GameObject mainPanel;

        [Header("Páginas de la lista (RF-02, RNF-03)")]
        [SerializeField]
        [Tooltip("Cuántos perfiles caben enteros en el panel. El resto se alcanza con las flechas.")]
        [Min(1)]
        private int profilesPerPage = 3;

        [SerializeField]
        [Tooltip("Página anterior. Solo aparece si hay una página antes: clic, nunca arrastre ni rueda (RNF-02, CT-06).")]
        private Button pageUpButton;

        [SerializeField]
        [Tooltip("Página siguiente. Solo aparece si hay una página después.")]
        private Button pageDownButton;

        [Header("Confirmación de borrado (RF-47, RNF-11)")]
        [SerializeField] private GameObject deletePanel;
        [SerializeField] private Text deletePrompt;
        [SerializeField] private Button deleteConfirmButton;
        [SerializeField] private Button deleteCancelButton;

        /// <summary>Perfil que el estudiante pidió borrar y aún no ha confirmado.</summary>
        private string _pendingDeletion;

        private int _page;

        internal Button PageUpButton => pageUpButton;
        internal Button PageDownButton => pageDownButton;

        internal ProfileSession Session { get; set; }
        internal GameFlowRunner Runner { get; set; }

        private void Awake()
        {
            Session ??= GameFlowRunner.Instance != null ? GameFlowRunner.Instance.Session : null;
            Runner ??= GameFlowRunner.Instance;
        }

        private void Start()
        {
            profileEntryPrototype.gameObject.SetActive(false);
            confirmButton.onClick.AddListener(CreateNew);
            backButton.onClick.AddListener(Back);
            deleteConfirmButton.onClick.AddListener(ConfirmDeletion);
            deleteCancelButton.onClick.AddListener(CancelDeletion);
            pageUpButton.onClick.AddListener(() => ShowPage(_page - 1));
            pageDownButton.onClick.AddListener(() => ShowPage(_page + 1));
            deletePanel.SetActive(false);
        }

        private void OnEnable()
        {
            messageLabel.text = string.Empty;
            nameField.text = string.Empty;
            CancelDeletion();

            if (ScreenFlow.Ready(Session, this))
            {
                _page = 0;
                Populate();
            }
        }

        private void ShowPage(int page)
        {
            _page = page;
            Populate();
        }

        private void Populate()
        {
            for (var i = profileList.childCount - 1; i >= 0; i--)
            {
                var child = profileList.GetChild(i);
                if (child != profileEntryPrototype.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            var names = Session.ExistingProfileNames();
            _page = ProfilePaging.ClampPage(_page, names.Count, profilesPerPage);
            var pages = ProfilePaging.PageCount(names.Count, profilesPerPage);
            // Cada flecha solo está donde lleva a algún sitio: sin perfiles de más no hay ninguna.
            pageUpButton.gameObject.SetActive(_page > 0);
            pageDownButton.gameObject.SetActive(_page < pages - 1);

            foreach (var profileName in names.Skip(_page * profilesPerPage).Take(profilesPerPage))
            {
                var entry = Instantiate(profileEntryPrototype, profileList);
                entry.gameObject.name = $"ProfileEntry({profileName})";
                entry.gameObject.SetActive(true);
                entry.GetComponentInChildren<Text>().text = profileName;
                var captured = profileName;
                entry.onClick.AddListener(() => SelectExisting(captured));

                var delete = entry.transform.Find("DeleteButton").GetComponent<Button>();
                delete.onClick.AddListener(() => AskDeletion(captured));
            }
        }

        private void SelectExisting(string profileName)
        {
            if (!ScreenFlow.Ready(Session, this) || !ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            // Un JSON dañado (corte de luz a mitad de guardado, archivo editado a mano) no puede
            // dejar el clic sin respuesta ni lanzar: se avisa y el perfil sigue pudiéndose borrar.
            if (!Session.TryLoad(profileName, out var profile))
            {
                messageLabel.text = content.UnreadableProfileMessage;
                return;
            }

            Runner.SelectProfile(profile);
        }

        private void CreateNew()
        {
            if (!ScreenFlow.Ready(Session, this) || !ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            var result = Session.Create(nameField.text);
            switch (result.Result)
            {
                case ProfileCreationResult.Status.EmptyName:
                    messageLabel.text = content.EmptyNameMessage;
                    return;
                case ProfileCreationResult.Status.DuplicateName:
                    messageLabel.text = content.DuplicateNameMessage;
                    return;
                case ProfileCreationResult.Status.InvalidName:
                    messageLabel.text = content.InvalidNameMessage;
                    return;
            }

            Session.Save(result.Profile);
            Runner.SelectProfile(result.Profile);
            Runner.StartNarrative(IntroNarrativeId);
        }

        /// <summary>
        /// Pide confirmación antes de borrar. El borrado es irreversible (RF-47) y por eso nunca
        /// ocurre en el clic que lo pide: siempre media una segunda pantalla que lo nombra.
        /// </summary>
        private void AskDeletion(string profileName)
        {
            _pendingDeletion = profileName;
            deletePrompt.text = string.Format(content.DeletePromptFormat, profileName);
            deletePanel.SetActive(true);
        }

        private void ConfirmDeletion()
        {
            if (_pendingDeletion == null || !ScreenFlow.Ready(Session, this))
            {
                CancelDeletion();
                return;
            }

            if (!Session.Delete(_pendingDeletion))
            {
                // RNF-11 no admite «casi borrado»: si quedó rastro hay que decirlo, no callarlo.
                messageLabel.text = content.DeleteFailedMessage;
            }

            CancelDeletion();
            Populate();
        }

        private void CancelDeletion()
        {
            _pendingDeletion = null;
            deletePanel.SetActive(false);
        }

        private void Back()
        {
            // El intercambio de paneles es local a la escena: se hace aunque no haya flujo, para
            // que volver nunca deje al estudiante encerrado en el panel.
            mainPanel.SetActive(true);
            gameObject.SetActive(false);

            if (ScreenFlow.Ready(Runner, this))
            {
                Runner.GoTo(GameState.MainMenu);
            }
        }
    }
}
