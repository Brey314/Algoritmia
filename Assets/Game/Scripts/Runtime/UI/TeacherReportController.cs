using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Game.Reporting;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Consulta de progreso del docente (RF-46): lista de perfiles del equipo, y los cuatro
    /// indicadores del seleccionado, por nivel y por fase. Adaptador delgado: la enumeración y la
    /// agregación viven en <see cref="ProfileRepository"/> e <see cref="IndicatorReport"/>
    /// (<c>Game.Reporting</c>, que no conoce uGUI); aquí solo se reparte en la pantalla.
    /// </summary>
    /// <remarks>
    /// Solo lectura: entrar, consultar y salir no toca el perfil activo del estudiante ni su
    /// progreso (RF-46 paso 1, CU-11) — la única escritura posible es el borrado explícito de
    /// <see cref="EraseConfirmationDialog"/>.
    /// </remarks>
    public class TeacherReportController : MonoBehaviour
    {
        [SerializeField] private ReportContent content;
        [SerializeField] private Transform profileList;
        [SerializeField] private Button profileEntryPrototype;
        [SerializeField] private Button backButton;

        [Header("Sin perfiles (CU-11 FA-2a)")]
        [SerializeField] private GameObject emptyStatePanel;
        [SerializeField] private Text emptyStateLabel;

        [Header("Perfil seleccionado")]
        [SerializeField] private GameObject dataPanel;
        [SerializeField] private IndicatorTableView tableView;

        [Header("Eliminación (RF-47)")]
        [SerializeField] private Button deleteButton;
        [SerializeField] private EraseConfirmationDialog eraseDialog;

        private readonly List<Button> _entries = new List<Button>();

        internal GameFlowRunner Runner { get; set; }

        /// <summary>El origen de los perfiles. Producción: uno real sobre las rutas de <see cref="GameFlowRunner"/>.</summary>
        internal ProfileRepository Repository { get; set; }

        private PlayerProfile _selected;

#if UNITY_INCLUDE_TESTS
        internal IReadOnlyList<Button> Entries => _entries;
        internal GameObject EmptyStatePanel => emptyStatePanel;
        internal GameObject DataPanel => dataPanel;
        internal Button DeleteButton => deleteButton;
        internal EraseConfirmationDialog EraseDialog => eraseDialog;
        internal Button BackButton => backButton;
        internal PlayerProfile Selected => _selected;
#endif

        private void Awake()
        {
            Runner ??= GameFlowRunner.Instance;
            if (Repository == null && Runner != null)
            {
                Repository = new ProfileRepository(new DiskFileSystem(), Runner.PortableRoot, Runner.FallbackRoot);
            }
        }

        private void Start()
        {
            profileEntryPrototype.gameObject.SetActive(false);
            if (content != null && emptyStateLabel != null)
            {
                emptyStateLabel.text = content.NoProfilesLabel;
            }

            backButton.onClick.AddListener(Back);
            deleteButton.onClick.AddListener(AskDeletion);
            eraseDialog.gameObject.SetActive(false);
            Show();
        }

        internal void Show()
        {
            if (Repository == null)
            {
                return;
            }

            var profiles = Repository.AllProfiles();
            Populate(profiles);

            var hasProfiles = profiles.Count > 0;
            emptyStatePanel.SetActive(!hasProfiles);
            dataPanel.SetActive(hasProfiles);

            if (!hasProfiles)
            {
                _selected = null;
                return;
            }

            // Conserva la selección si el perfil sigue en la lista (p. ej. tras borrar otro);
            // si no, cae al primero.
            var stillThere = _selected != null && profiles.Any(p => p.Name == _selected.Name);
            Select(stillThere ? profiles.First(p => p.Name == _selected.Name) : profiles[0]);
        }

        private void Populate(IReadOnlyList<PlayerProfile> profiles)
        {
            foreach (var entry in _entries)
            {
                Destroy(entry.gameObject);
            }

            _entries.Clear();

            foreach (var profile in profiles)
            {
                var entry = Instantiate(profileEntryPrototype, profileList);
                entry.gameObject.name = $"Perfil({profile.Name})";
                entry.gameObject.SetActive(true);
                entry.GetComponentInChildren<Text>().text = profile.Name;
                var captured = profile;
                entry.onClick.AddListener(() => Select(captured));
                _entries.Add(entry);
            }
        }

        private void Select(PlayerProfile profile)
        {
            _selected = profile;
            tableView.Render(IndicatorReport.For(profile));
        }

        private void AskDeletion()
        {
            if (_selected == null)
            {
                return;
            }

            eraseDialog.Ask(_selected.Name, deleted =>
            {
                if (deleted)
                {
                    Show();
                }
            });
        }

        private void Back()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            Runner.GoTo(GameState.MainMenu);
        }
    }
}
