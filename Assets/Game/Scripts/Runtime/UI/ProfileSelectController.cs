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

        [SerializeField] private Transform profileList;
        [SerializeField] private Button profileEntryPrototype;
        [SerializeField] private InputField nameField;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Text messageLabel;
        [SerializeField] private GameObject mainPanel;

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
        }

        private void OnEnable()
        {
            messageLabel.text = string.Empty;
            nameField.text = string.Empty;

            if (ScreenFlow.Ready(Session, this))
            {
                Populate();
            }
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

            foreach (var profileName in Session.ExistingProfileNames())
            {
                var entry = Instantiate(profileEntryPrototype, profileList);
                entry.gameObject.name = $"ProfileEntry({profileName})";
                entry.gameObject.SetActive(true);
                entry.GetComponentInChildren<Text>().text = profileName;
                var captured = profileName;
                entry.onClick.AddListener(() => SelectExisting(captured));
            }
        }

        private void SelectExisting(string profileName)
        {
            if (!ScreenFlow.Ready(Session, this) || !ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            Runner.SelectProfile(Session.Load(profileName));
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
                    messageLabel.text = "Escribe un nombre para empezar.";
                    return;
                case ProfileCreationResult.Status.DuplicateName:
                    messageLabel.text = "Ya hay un perfil con ese nombre. Elige otro.";
                    return;
                case ProfileCreationResult.Status.InvalidName:
                    messageLabel.text = "Ese nombre no se puede usar. Prueba con otro.";
                    return;
            }

            Session.Save(result.Profile);
            Runner.SelectProfile(result.Profile);
            Runner.StartNarrative(IntroNarrativeId);
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
