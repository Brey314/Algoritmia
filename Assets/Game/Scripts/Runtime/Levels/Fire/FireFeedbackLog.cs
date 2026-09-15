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
                : outcome.Spacing != SpacingBand.Effective ? SpacingMessage(outcome.Spacing, consecutiveFailures)
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

        /// <summary>Las piedras no chocaron: separadas o demasiado encimadas (T26). Cada lado tiene su par.</summary>
        private string SpacingMessage(SpacingBand spacing, int consecutiveFailures) =>
            spacing == SpacingBand.TooFar
                ? Escalate(_messages.StonesFar, _messages.StonesFarAgain, consecutiveFailures)
                : Escalate(_messages.StonesTooClose, _messages.StonesTooCloseAgain, consecutiveFailures);

        /// <summary>Un golpe no efectivo se pasó de suave o de fuerte (Fase 5). Cada lado tiene su par.</summary>
        private string FailureMessage(ForceBand band, int consecutiveFailures) =>
            band == ForceBand.TooSoft
                ? Escalate(_messages.SoftNoSpark, _messages.SoftStonesGraze, consecutiveFailures)
                : Escalate(_messages.HardSparksScatter, _messages.HardSparksFly, consecutiveFailures);

        private string Escalate(string fallback, string escalated, int consecutiveFailures)
        {
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
