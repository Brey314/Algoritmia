using System;
using Game.Core;
using Game.Scaffolding;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.Fire
{
    /// <summary>
    /// El panel de encendido del Nivel 1 (RF-14, guion §4.3.1): traduce los clics de «Golpear» y
    /// «Soplar» a llamadas de <see cref="FireAttempt"/> / <see cref="FireFeedbackLog"/> y el estado
    /// resultante a la UI.
    /// </summary>
    /// <remarks>
    /// Adaptador delgado: **no contiene ninguna regla del juego**. El deslizante representa la
    /// hipótesis y no produce efecto hasta «Golpear» (RF-15); el resultado de cada golpe lo decide
    /// <see cref="FireAttempt"/> (T12) y el mensaje <see cref="FireFeedbackLog"/> (T13). Al converger
    /// y accionar «Soplar», anima el nacimiento del fuego y encadena a la confirmación de fase, el
    /// desbloqueo del Nivel 2 y la escena narrativa de cierre (T15, RF-03, RF-04, RF-20).
    /// </remarks>
    public class FirePanelController : MonoBehaviour
    {
        [SerializeField] private FireLevelConfig config;
        [SerializeField] private FireMessages messages;

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("positionSlider")]
        [Tooltip("Deslizante de fuerza (Fase 5). Su rango lo fija la configuración al arrancar.")]
        private Slider forceSlider;
        [SerializeField] private Button strikeButton;
        [SerializeField] private Button blowButton;

        [SerializeField]
        [Tooltip("Icono de candado + texto «Aún no»: el segundo indicador de RNF-19 para «Soplar».")]
        private GameObject blowLockedBadge;

        [SerializeField] private FeedbackLogView logView;

        [SerializeField]
        [Tooltip("Las piezas del suelo: hojas, sílex y pedernal (T22). Hermanas del punto del fuego.")]
        private DraggablePiece[] pieces = Array.Empty<DraggablePiece>();

        [SerializeField]
        [Tooltip("El punto del fuego: donde hay que amontonar las hojas y acercar las piedras. Los radios salen de la configuración, como fracción del alto del suelo.")]
        private RectTransform fireSpot;

        [SerializeField]
        [Tooltip("Iluminación progresiva del escenario (RF-21, prioridad Baja). Puede quedar sin " +
            "asignar: el resto del nivel sigue jugable sin ella.")]
        private CaveLightingController lighting;

        [SerializeField]
        [Tooltip("Pasos del guía del Nivel 1 (N1_Guia): instrucción y pista de «Golpear» y de «Soplar».")]
        private GuideContent guide;

        [SerializeField]
        [Tooltip("Botón «Pista» (mockup 7, arriba a la izquierda): repite la instrucción del paso (RF-13).")]
        private Button hintButton;

        private FireAttempt _attempt;
        private FireFeedbackLog _log;
        private FireIndicatorCollector _indicators;
        private HintPolicy _hints;

        /// <summary>El flujo del juego. Sin él (escena abierta sin pasar por Boot) no se navega.</summary>
        internal GameFlowRunner Runner { get; set; }

        /// <summary>La fuerza que marca el deslizante. Solo <see cref="Strike"/> la lee (RF-15).</summary>
        internal int SelectedForce =>
            Mathf.Clamp(Mathf.RoundToInt(forceSlider.value), 0, config.ForceLevels);

#if UNITY_INCLUDE_TESTS
        internal Slider ForceSlider => forceSlider;
        internal Button StrikeButton => strikeButton;
        internal Button BlowButton => blowButton;
        internal GameObject BlowLockedBadge => blowLockedBadge;
        internal FeedbackLogView LogView => logView;
        internal DraggablePiece[] Pieces => pieces;
        internal RectTransform FireSpot => fireSpot;
        internal FireAttempt Attempt => _attempt;
        internal FireFeedbackLog Log => _log;
        internal FireIndicatorCollector Indicators => _indicators;
        internal CaveLightingController Lighting => lighting;
        internal Button HintButton => hintButton;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            _attempt = new FireAttempt(config);
            _log = new FireFeedbackLog(messages, config.MinimumEffectiveStrikes);
            _indicators = new FireIndicatorCollector(config, () => Time.realtimeSinceStartup);
            if (Runner != null)
            {
                Runner.ActiveReporter = _indicators;
            }

            // La ayuda es una capa aparte (CP-06, T11): el panel solo la pulsa. Sin asset de guía
            // la política queda con paso nulo y no escribe nada — el nivel sigue jugable.
            _hints = new HintPolicy(Step("Golpear"), config.AttemptsBeforeHint);

            // El rango del deslizante sale de la configuración, no de la escena (CT-05, RNF-18).
            forceSlider.wholeNumbers = true;
            forceSlider.minValue = 0;
            forceSlider.maxValue = config.ForceLevels;

            strikeButton.onClick.AddListener(Strike);
            blowButton.onClick.AddListener(Blow);
            if (hintButton != null)
            {
                hintButton.onClick.AddListener(RequestHelp);
            }

            RefreshBlow();
            RefreshLighting();
        }

        /// <summary>Las dos piedras dentro del radio de las piedras (T22). Sin punto de fuego cableado, siempre.</summary>
        internal bool StonesNear =>
            fireSpot == null || Arrangement.StonesNear(PositionOf(PieceKind.Silex), PositionOf(PieceKind.Pedernal));

        /// <summary>Todas las hojas dentro del radio del montón (T22). Sin punto de fuego cableado, siempre.</summary>
        internal bool LeavesPiled =>
            fireSpot == null || Arrangement.IsPiled(Array.ConvertAll(
                Array.FindAll(pieces, piece => piece.Kind == PieceKind.Leaf), piece => piece.Position));

        private FireArrangement Arrangement
        {
            get
            {
                var floorHeight = ((RectTransform)fireSpot.parent).rect.height;
                return new FireArrangement(fireSpot.anchoredPosition,
                    config.PileRadius * floorHeight, config.StonesRadius * floorHeight);
            }
        }

        private Vector2 PositionOf(PieceKind kind)
        {
            var piece = Array.Find(pieces, candidate => candidate.Kind == kind);
            // Sin esa piedra en la escena se toma como lejos: la regla nunca prende sin las dos.
            return piece != null ? piece.Position : new Vector2(float.MaxValue / 4f, 0f);
        }

        /// <summary>Ejecuta un golpe con la fuerza marcada y refresca la UI (RF-16).</summary>
        internal void Strike()
        {
            var outcome = _attempt.Strike(SelectedForce, StonesNear);
            _indicators.RecordStrike(outcome);
            _log.Record(outcome, _attempt.ConsecutiveFailures);
            if (outcome.Effective)
            {
                _hints.RegisterSuccessfulAttempt();
                if (outcome.EffectiveStrikes == config.MinimumEffectiveStrikes)
                {
                    _hints.Activate(Step("Soplar")); // la tarea cambia al converger (guion §4.3.6)
                }
            }
            else
            {
                _log.RecordGuide(_hints.RegisterFailedAttempt()); // pista solo al tercer fallo seguido
            }

            logView.Show(_log.Entries);
            RefreshBlow();
            RefreshLighting();
        }

        /// <summary>«Pista»: repite la instrucción del paso activo en el registro (RF-13, HU-03).</summary>
        internal void RequestHelp()
        {
            _log.RecordGuide(_hints.RequestHelp());
            logView.Show(_log.Entries);
        }

        private GuideStep Step(string id) =>
            guide != null ? Array.Find(guide.Steps, step => step.Id == id) : null;

        /// <summary>Acciona «Soplar». Atenuado antes de la convergencia: no responde (RF-19).</summary>
        internal void Blow()
        {
            // Guarda defensiva: el botón ya está atenuado antes de converger (guion §4.3.6), pero
            // un clic simulado en pruebas no pasa por esa comprobación de la UI.
            if (!_attempt.CanBlow)
            {
                return;
            }

            // «Por qué no» pedagógico: soplar con las hojas regadas no es un error que se
            // castigue —no hay contador ni bloqueo (CP-02)—; solo se cuenta lo que pasó y el
            // estudiante reacomoda y vuelve a soplar (T23).
            if (!LeavesPiled || !StonesNear)
            {
                _log.RecordBlowFailed();
                logView.Show(_log.Entries);
                return;
            }

            _log.RecordBlowSuccess();
            logView.Show(_log.Entries);
            _ = ResolveAsync();
        }

        /// <summary>Anima la convergencia y, al terminar, marca el nivel completado (RF-20).</summary>
        private async Awaitable ResolveAsync()
        {
            try
            {
                await PlayIgnitionAsync();
            }
            catch (OperationCanceledException)
            {
                return; // el panel se destruyó (cambio de escena) a mitad de la animación.
            }

            CompleteLevel();
        }

        /// <summary>Nacimiento del fuego: un barrido de color, guion §4.3.5 E7.</summary>
        private async Awaitable PlayIgnitionAsync()
        {
            // Un único barrido, en una sola dirección: RNF-21 prohíbe destellos de alta
            // frecuencia, y un `Mathf.PingPong` o cualquier oscilación los produciría. Las hojas
            // amontonadas se tiñen de fuego; el sprite de fuego real es arte pendiente (A7).
            const float duration = 0.6f;
            var leaves = Array.ConvertAll(
                Array.FindAll(pieces, piece => piece.Kind == PieceKind.Leaf), piece => piece.GetComponent<Image>());
            var starts = Array.ConvertAll(leaves, image => image != null ? image.color : Color.white);
            var fire = new Color(0.91f, 0.40f, 0.10f);
            var startLighting = lighting != null ? lighting.Progress : 1f;
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                var t = elapsed / duration;
                for (var i = 0; i < leaves.Length; i++)
                {
                    if (leaves[i] != null)
                    {
                        leaves[i].color = Color.Lerp(starts[i], fire, t);
                    }
                }

                // Un solo barrido combinado con el fuego: la iluminación completa llega en la
                // resolución (guion E7), no antes (E4 solo sube un escalón por golpe efectivo).
                lighting?.SetProgress(Mathf.Lerp(startLighting, 1f, t));
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }

            foreach (var leaf in leaves)
            {
                if (leaf != null)
                {
                    leaf.color = fire;
                }
            }

            lighting?.SetProgress(1f);
        }

        /// <summary>
        /// Deja los indicadores listos y encadena a la escena de cierre (RF-20). No confirma la
        /// fase ni desbloquea ni guarda aquí: eso pasa en <c>LevelSummaryController</c>, después
        /// de la escena narrativa (HU-14 paso 6-7) — confirmarlo antes haría que «ya vista»
        /// (<see cref="Game.Scaffolding.NarrativeVisitPolicy"/>) diera verdadero desde la
        /// primerísima vez y el guía se saltara el nombre de la habilidad practicada (CP-07).
        /// </summary>
        private void CompleteLevel()
        {
            if (Runner == null || Runner.Flow.ActiveProfile == null)
            {
                // Mismo criterio que ScreenFlow (Game.UI), sin depender de ese assembly: la
                // escena se abrió sin pasar por Boot, no hay a dónde navegar.
                Debug.LogWarning(
                    "«Level1_Cave» se abrió sin pasar por «Boot»: no hay perfil activo, no se " +
                    "avanza a la escena de cierre.", this);
                return;
            }

            Runner.PendingIndicators = _indicators.Complete();
            Runner.StartNarrative("N1_NacimientoDelFuego");
        }

        private void RefreshBlow()
        {
            // «Por qué no» pedagógico: `CanBlow` de FireAttempt nunca vuelve a falso una vez
            // alcanzado el mínimo (INC-32), así que el botón y su badge tampoco re-bloquean tras
            // un fallo posterior. Lo ganado permanece.
            blowButton.interactable = _attempt.CanBlow;
            if (blowLockedBadge != null)
            {
                blowLockedBadge.SetActive(!_attempt.CanBlow);
            }
        }

        /// <summary>Sube un escalón por golpe efectivo, sin llegar al máximo (RF-21, guion E4) —
        /// el último tramo hasta la iluminación completa lo da <see cref="PlayIgnitionAsync"/> en
        /// la resolución (E7).</summary>
        private void RefreshLighting() =>
            lighting?.SetProgress((float)_attempt.EffectiveStrikes / (config.MinimumEffectiveStrikes + 1));

    }
}
