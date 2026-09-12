using System.Collections.Generic;

namespace Game.Levels.Fire
{
    /// <summary>
    /// El área de registro del Nivel 1 (guion §4.3.1): traduce cada golpe a un mensaje narrativo
    /// del asset <see cref="FireMessages"/> y acumula el historial para que el estudiante compare
    /// sus intentos (HU-05, HU-06).
    /// </summary>
    /// <remarks>
    /// C# plano, sin dependencias de Unity. El panel jugable (T14) traduce el resultado de
    /// <see cref="FireAttempt.Strike"/> a <see cref="Record"/> y pinta <see cref="Entries"/>.
    /// No repite el mismo mensaje dos veces seguidas cuando hay una alternativa aplicable
    /// (RF-18, guion §4.3.4).
    /// </remarks>
    public class FireFeedbackLog
    {
        private readonly FireMessages _messages;
        private readonly int _minimumEffectiveStrikes;
        private readonly List<string> _entries = new();

        public FireFeedbackLog(FireMessages messages, int minimumEffectiveStrikes)
        {
            _messages = messages;
            _minimumEffectiveStrikes = minimumEffectiveStrikes;
        }

        /// <summary>El historial acumulado, del primer intento al último (HU-06).</summary>
        public IReadOnlyList<string> Entries => _entries;

        /// <summary>El último mensaje escrito, o cadena vacía si no hay ninguno.</summary>
        public string Latest => _entries.Count > 0 ? _entries[^1] : string.Empty;

        /// <summary>Registra el resultado de un golpe y devuelve el mensaje elegido (RF-11, RF-17).</summary>
        public string Record(StrikeOutcome outcome, int consecutiveFailures)
        {
            var message = outcome.Effective ? EffectiveMessage(outcome.EffectiveStrikes)
                : !outcome.StonesNear ? StonesFarMessage(consecutiveFailures)
                : FailureMessage(outcome.Band, consecutiveFailures);
            _entries.Add(message);
            return message;
        }

        /// <summary>Registra el éxito del soplo (guion §4.3.4, HU-05 FA-01). Lo llama T15.</summary>
        public string RecordBlowSuccess()
        {
            _entries.Add(_messages.BlowSuccess);
            return _messages.BlowSuccess;
        }

        /// <summary>Soplo con las hojas regadas o las piedras lejos: se describe, no se penaliza (T23, CP-02).</summary>
        public string RecordBlowFailed()
        {
            _entries.Add(_messages.BlowNoPile);
            return _messages.BlowNoPile;
        }

        private string StonesFarMessage(int consecutiveFailures)
        {
            // Mismo criterio que los fallos de fuerza: la variante «tras dos intentos» solo desde
            // el segundo fallo seguido, y sin repetir dos veces seguidas cuando hay alternativa.
            if (consecutiveFailures < 2)
            {
                return _messages.StonesFar;
            }

            return _messages.StonesFarAgain == Latest ? _messages.StonesFar : _messages.StonesFarAgain;
        }

        /// <summary>
        /// Escribe una línea del guía —la instrucción pedida con «Pista» o la pista tras los
        /// fallos seguidos (RF-13)— en el mismo registro que los mensajes de los golpes. Vacía o
        /// nula no escribe nada: <c>HintPolicy</c> devuelve nulo mientras no toca pista.
        /// </summary>
        public void RecordGuide(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _entries.Add(message);
            }
        }

        private string EffectiveMessage(int effectiveStrikes)
        {
            if (effectiveStrikes >= _minimumEffectiveStrikes)
            {
                return _messages.FinalEffectiveStrike; // convergencia: el hilo de humo (guion §4.3.5 E6)
            }

            return effectiveStrikes == 1
                ? _messages.FirstEffectiveStrike
                : _messages.SecondEffectiveStrike;
        }

        private string FailureMessage(ForceBand band, int consecutiveFailures)
        {
            // Un golpe no efectivo se pasó de suave o de fuerte (Fase 5): cada lado tiene su par
            // de mensajes, por defecto y «tras dos intentos».
            var (fallback, escalated) = band == ForceBand.TooSoft
                ? (_messages.SoftNoSpark, _messages.SoftStonesGraze)
                : (_messages.HardSparksScatter, _messages.HardSparksFly);

            // La variante «tras dos intentos» (guion §4.3.4) solo aplica desde el segundo fallo
            // seguido. Antes no hay alternativa y un mensaje repetido se permite.
            if (consecutiveFailures < 2)
            {
                return fallback;
            }

            // Con la alternativa disponible, si «tras dos intentos» repetiría el último mensaje se
            // vuelve al de por defecto para no repetir dos veces seguidas (RF-18).
            return escalated == Latest ? fallback : escalated;
        }
    }
}
