using System;
using Game.Core;
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
        [SerializeField] private Slider positionSlider;
        [SerializeField] private Button strikeButton;
        [SerializeField] private Button blowButton;

        [SerializeField]
        [Tooltip("Icono de candado + texto «Aún no»: el segundo indicador de RNF-19 para «Soplar».")]
        private GameObject blowLockedBadge;

        [SerializeField] private FeedbackLogView logView;
        [SerializeField] private Image leavesImage;

        [SerializeField]
        [Tooltip("Sprites del montón de hojas por número de golpes efectivos (placeholder T14).")]
        private Sprite[] leavesStates = Array.Empty<Sprite>();

        private FireAttempt _attempt;
        private FireFeedbackLog _log;

        /// <summary>El flujo del juego. Sin él (escena abierta sin pasar por Boot) no se navega.</summary>
        internal GameFlowRunner Runner { get; set; }

        /// <summary>Override de prueba para el guardado (T15), patrón de <c>MainMenuController</c>.</summary>
        internal IProfileSaver Saver { get; set; }

        /// <summary>La distancia que marca el deslizante. Solo <see cref="Strike"/> la lee (RF-15).</summary>
        internal StrikePosition Selected =>
            (StrikePosition)Mathf.Clamp(Mathf.RoundToInt(positionSlider.value), 0, 2);

#if UNITY_INCLUDE_TESTS
        internal Slider PositionSlider => positionSlider;
        internal Button StrikeButton => strikeButton;
        internal Button BlowButton => blowButton;
        internal GameObject BlowLockedBadge => blowLockedBadge;
        internal FeedbackLogView LogView => logView;
        internal FireAttempt Attempt => _attempt;
        internal FireFeedbackLog Log => _log;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            _attempt = new FireAttempt(config);
            _log = new FireFeedbackLog(messages, config.MinimumEffectiveStrikes);

            strikeButton.onClick.AddListener(Strike);
            blowButton.onClick.AddListener(Blow);

            RefreshBlow();
            RefreshLeaves();
        }

        /// <summary>Ejecuta un golpe desde la distancia marcada y refresca la UI (RF-16).</summary>
        internal void Strike()
        {
            var outcome = _attempt.Strike(Selected);
            _log.Record(outcome, _attempt.ConsecutiveFailures);
            logView.Show(_log.Entries);
            RefreshLeaves();
            RefreshBlow();
        }

        /// <summary>Acciona «Soplar». Atenuado antes de la convergencia: no responde (RF-19).</summary>
        internal void Blow()
        {
            // Guarda defensiva: el botón ya está atenuado antes de converger (guion §4.3.6), pero
            // un clic simulado en pruebas no pasa por esa comprobación de la UI.
            if (!_attempt.CanBlow)
            {
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
            if (leavesImage == null)
            {
                return;
            }

            // Un único barrido, en una sola dirección: RNF-21 prohíbe destellos de alta
            // frecuencia, y un `Mathf.PingPong` o cualquier oscilación los produciría. La
            // iluminación progresiva del resto del escenario es T19; el sprite de fuego real es
            // arte pendiente.
            const float duration = 0.6f;
            var start = leavesImage.color;
            var fire = new Color(0.91f, 0.40f, 0.10f);
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                leavesImage.color = Color.Lerp(start, fire, elapsed / duration);
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }

            leavesImage.color = fire;
        }

        /// <summary>Confirma la fase, desbloquea el Nivel 2, guarda y encadena al cierre (RF-03, RF-04).</summary>
        private void CompleteLevel()
        {
            var profile = Runner != null ? Runner.Flow.ActiveProfile : null;
            if (profile == null)
            {
                // Mismo criterio que ScreenFlow (Game.UI), sin depender de ese assembly: la
                // escena se abrió sin pasar por Boot, no hay a quién guardarle ni a dónde navegar.
                Debug.LogWarning(
                    "«Level1_Cave» se abrió sin pasar por «Boot»: no hay perfil activo, no se " +
                    "guarda ni se avanza a la escena de cierre.", this);
                return;
            }

            // T17: aquí van los cuatro indicadores reales (FireIndicatorCollector). De momento se
            // confirma la fase sin ellos, igual que T11 dejó el umbral de pista para que T12 lo
            // completara sin tocar HintPolicy.
            profile.ConfirmPhase(LevelId.Fire, 1, default);
            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Fire);
            (Saver ?? Runner.Session).SaveActive();

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

        private void RefreshLeaves()
        {
            if (leavesImage == null || leavesStates.Length == 0)
            {
                return;
            }

            var state = leavesStates[Mathf.Clamp(_attempt.EffectiveStrikes, 0, leavesStates.Length - 1)];
            if (state != null)
            {
                leavesImage.sprite = state;
            }
        }
    }
}
