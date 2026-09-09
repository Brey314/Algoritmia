using System;
using Game.Core;
using Game.Scaffolding;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// La **única** escena narrativa (RF-05, RF-06). Recibe qué secuencia reproducir por su
    /// identificador —el que <see cref="GameFlow.NarrativeSequenceId"/> guardó al entrar— y la
    /// resuelve contra la lista de secuencias del proyecto.
    /// </summary>
    /// <remarks>
    /// Aquí está el motivo de que <c>GameState.Narrative</c> sea un estado parametrizado y no
    /// quince: añadir una escena narrativa es crear un asset y arrastrarlo a esta lista, no una
    /// escena, un estado y una rama. Por eso este controlador **no tiene ni un `if` por
    /// secuencia**: las tres del Nivel 1 recorren exactamente el mismo código.
    ///
    /// Adaptador delgado: avanzar, omitir y saber si ya se vio la escena viven en
    /// <see cref="DialogueRunner"/> y <see cref="NarrativeVisitPolicy"/>, que son C# plano.
    ///
    /// No lleva botón de pausa, y es deliberado: HU-17 FA-04 lo pide solo durante el juego.
    /// </remarks>
    public class NarrativeSceneController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Todas las secuencias narrativas. La escena elige por identificador, sin ramas.")]
        private NarrativeSequence[] sequences = Array.Empty<NarrativeSequence>();

        [SerializeField] private Image illustration;
        [SerializeField] private Text speakerLabel;
        [SerializeField] private Text bodyLabel;
        [SerializeField] private Button advanceButton;
        [SerializeField] private Button skipButton;

        internal GameFlowRunner Runner { get; set; }

        /// <summary>El avance de la escena en curso. <c>null</c> si no se pudo resolver.</summary>
        internal DialogueRunner Dialogue { get; private set; }

#if UNITY_INCLUDE_TESTS
        internal Text BodyLabel => bodyLabel;
        internal Text SpeakerLabel => speakerLabel;
        internal Button AdvanceButton => advanceButton;
        internal Button SkipButton => skipButton;
        internal NarrativeSequence[] Sequences => sequences;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            advanceButton.onClick.AddListener(Advance);
            skipButton.onClick.AddListener(SkipScene);
            Begin();
        }

        /// <summary>
        /// Arranca la secuencia que el flujo pidió. Se separa de <c>Start</c> porque el runner se
        /// inyecta después de cargar la escena en las pruebas.
        /// </summary>
        internal void Begin()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            var wanted = Runner.Flow.NarrativeSequenceId;
            var sequence = Array.Find(sequences,
                candidate => candidate != null && candidate.Id == wanted);

            if (sequence == null)
            {
                Debug.LogWarning($"No hay ninguna secuencia narrativa con el id «{wanted}».", this);
                return;
            }

            Dialogue = new DialogueRunner(sequence.Lines,
                NarrativeVisitPolicy.AlreadySeen(Runner.Flow.ActiveProfile, sequence.Level));

            illustration.sprite = sequence.Illustration;
            illustration.enabled = sequence.Illustration != null;
            // RF-06 e INC-28: el botón de omitir no existe en la primera visita, no basta con
            // deshabilitarlo — la escena de cierre es donde el guía nombra lo aprendido.
            skipButton.gameObject.SetActive(Dialogue.CanSkip);
            Render();
        }

        private void Advance()
        {
            if (Dialogue == null)
            {
                return;
            }

            if (Dialogue.Advance())
            {
                Render();
                return;
            }

            Leave();
        }

        private void SkipScene()
        {
            if (Dialogue != null && Dialogue.Skip())
            {
                Leave();
            }
        }

        private void Render()
        {
            var line = Dialogue.Current;
            if (line == null)
            {
                return;
            }

            speakerLabel.text = line.Speaker;
            // La acotación —lo que en el guion va en cursiva— no lleva nombre de hablante.
            speakerLabel.gameObject.SetActive(!line.IsStageDirection);
            bodyLabel.text = line.Text;
        }

        private void Leave()
        {
            // PROVISIONAL (T14): la salida natural de una escena narrativa del Nivel 1 es entrar
            // a jugar, pero la escena del nivel todavía no existe. `LevelSelect` es la otra
            // salida legal desde `Narrative` y deja el recorrido cerrado en vez de plantar al
            // estudiante en una pantalla sin salida (RNF-13). Cuando T14 traiga `Level1_Cave`,
            // esto pasa a `StartPlaying`.
            Runner.GoTo(GameState.LevelSelect);
        }
    }
}
