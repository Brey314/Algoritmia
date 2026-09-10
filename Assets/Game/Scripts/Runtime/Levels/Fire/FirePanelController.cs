using System;
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
    /// <see cref="FireAttempt"/> (T12) y el mensaje <see cref="FireFeedbackLog"/> (T13). El salto a
    /// la escena de cierre y el guardado son T15.
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

            // T15: aquí van la animación del nacimiento del fuego, el guardado de fase (RF-04) y el
            // salto a la escena narrativa de cierre. Hoy solo deja constancia en el registro.
            _log.RecordBlowSuccess();
            logView.Show(_log.Entries);
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
