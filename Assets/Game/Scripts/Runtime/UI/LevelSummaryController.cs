using System.Collections.Generic;
using System.Linq;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// El resumen de fin de nivel (RF-17, RF-45; mockups 13 y 13b): lo que el estudiante hizo, en
    /// palabras y sin cifras, y el nombre de la habilidad que ejercitó. Confirma la fase,
    /// desbloquea el nivel siguiente y guarda (RF-03, RF-04) al mostrarse.
    /// </summary>
    /// <remarks>
    /// Adaptador delgado: el texto lo compone <see cref="LevelSummaryComposer"/> a partir de los
    /// indicadores de **todas** las fases del nivel —una en el Nivel 1, tres en el Nivel 2—;
    /// aquí solo se reparte en la tablilla —título, hallazgo, viñetas del relato y la habilidad
    /// nombrada— y se cablean los dos botones, que hoy salen al mismo sitio.
    /// </remarks>
    public class LevelSummaryController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Un asset de mensajes por nivel (RF-45, CT-05). Se elige por el nivel que se acaba de jugar.")]
        private LevelSummaryMessages[] messagesByLevel;

        [SerializeField]
        [Tooltip("Título de la tablilla: la línea de apertura del resumen, sin los dos puntos.")]
        private Text titleLabel;

        [SerializeField]
        [Tooltip("Lo que el estudiante descubrió (mockup 13).")]
        private Text discoveryLabel;

        [SerializeField]
        [Tooltip("Contenedor del relato: una viñeta por frase (mockup 13b).")]
        private RectTransform bulletsContainer;

        [SerializeField]
        [Tooltip("Fila plantilla de viñeta, inactiva; se clona por frase. Su Text es el que se rellena.")]
        private GameObject bulletTemplate;

        [SerializeField]
        [Tooltip("La habilidad nombrada, en el recuadro verde (RF-12, CP-07).")]
        private Text skillLabel;

        [SerializeField] private Button continueButton;
        [SerializeField] private Button menuButton;

        private readonly List<Text> _bullets = new List<Text>();

        internal GameFlowRunner Runner { get; set; }

        internal IProfileSaver Saver { get; set; }

#if UNITY_INCLUDE_TESTS
        internal Text TitleLabel => titleLabel;
        internal Text DiscoveryLabel => discoveryLabel;
        internal Text SkillLabel => skillLabel;
        internal IReadOnlyList<Text> Bullets => _bullets;
        internal Button ContinueButton => continueButton;
        internal Button MenuButton => menuButton;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            continueButton.onClick.AddListener(Continue);
            if (menuButton != null)
            {
                menuButton.onClick.AddListener(Continue);
            }

            Show();
        }

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

            // La última fase la trae el runner (HU-14 paso 6); las anteriores del nivel ya están
            // confirmadas en el perfil (RF-04). Confirmar una fase ya confirmada no la toca.
            var profile = flow.ActiveProfile;
            profile.ConfirmPhase(new PhaseId(level, flow.PlayingPhase), Runner.PendingIndicators);
            LevelUnlockPolicy.UnlockAfterCompleting(profile, level);
            (Saver ?? Runner.Session).SaveActive();

            var messages = MessagesFor(level);
            if (messages == null)
            {
                Debug.LogWarning($"«LevelSummary» no tiene mensajes para el nivel {level}.", this);
                return;
            }

            // El compositor devuelve la apertura y las frases del relato separadas por línea en
            // blanco: la primera es el título, el resto son las viñetas.
            var phases = PhaseId.AllOf(level).Select(profile.IndicatorsFor);
            var lines = LevelSummaryComposer.Compose(messages, phases).Split(new[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            titleLabel.text = lines[0].TrimEnd(':');
            discoveryLabel.text = messages.Discovery;
            skillLabel.text = messages.SkillNamed;
            FillBullets(lines, 1);
        }

        private LevelSummaryMessages MessagesFor(LevelId level) =>
            messagesByLevel?.FirstOrDefault(messages => messages != null && messages.Level == level);

        private void FillBullets(string[] lines, int from)
        {
            foreach (var bullet in _bullets)
            {
                Destroy(bullet.transform.parent.gameObject);
            }

            _bullets.Clear();
            for (var i = from; i < lines.Length; i++)
            {
                var row = Instantiate(bulletTemplate, bulletsContainer, false);
                row.name = $"Vineta_{i}";
                row.SetActive(true);
                var text = row.GetComponentInChildren<Text>(true);
                text.text = lines[i];
                _bullets.Add(text);
            }
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
