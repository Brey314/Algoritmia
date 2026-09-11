using System;
using Game.Core;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Los cuatro indicadores del Nivel 1 (RF-45, OE1 §3.6.1), construidos en vivo a partir de los
    /// golpes y la pausa. Nunca se muestran al estudiante (CP-03): solo llegan a
    /// <see cref="PlayerProfile.ConfirmPhase"/>.
    /// </summary>
    /// <remarks>
    /// C# plano, sin dependencias de escena: se prueba en EditMode con un reloj inyectado, igual
    /// que <see cref="FireAttempt"/>. Implementa <see cref="ILevelReporter"/> para que
    /// <c>PauseMenuController</c> (Game.UI) pueda notificarle la pausa sin que ningún assembly
    /// gane una referencia nueva hacia el otro — la mediación es <see cref="GameFlowRunner.ActiveReporter"/>.
    /// </remarks>
    public class FireIndicatorCollector : ILevelReporter
    {
        private readonly int _minimumEffectiveStrikes;
        private readonly Func<float> _now;
        private readonly float _startedAt;

        private int _attempts;
        private int _correctedErrors;
        private int _stepsUsed;
        private bool _lastStrikeFailed;
        private bool _stepsCaptured;

        private float _pausedSeconds;
        private float _pausedAt;
        private bool _paused;

        /// <param name="now">
        /// Reloj inyectado (producción: <c>() => Time.realtimeSinceStartup</c>) para que el
        /// tiempo de resolución se pueda probar en EditMode sin esperas reales.
        /// </param>
        public FireIndicatorCollector(FireLevelConfig config, Func<float> now)
        {
            _minimumEffectiveStrikes = config.MinimumEffectiveStrikes;
            _now = now;
            _startedAt = now();
        }

        /// <summary>Cuenta el golpe hacia Intentos, Errores corregidos y Pasos utilizados (RF-45).</summary>
        public void RecordStrike(StrikeOutcome outcome)
        {
            if (!outcome.Effective)
            {
                _attempts++; // Intentos = golpes ejecutados desde una posición no efectiva.
                _lastStrikeFailed = true;
                return;
            }

            // Error corregido = un golpe efectivo que sigue de inmediato a uno que no lo fue. No
            // hace falta comparar StrikePosition aparte: la posición efectiva es única, así que
            // cualquier golpe fallido está, por construcción, en una posición distinta a la del
            // acierto que la corrige.
            if (_lastStrikeFailed)
            {
                _correctedErrors++;
            }

            _lastStrikeFailed = false;

            if (!_stepsCaptured && outcome.EffectiveStrikes >= _minimumEffectiveStrikes)
            {
                // Pasos utilizados se congela al cruzar el umbral que habilita «Soplar»; golpes
                // efectivos posteriores (el jugador puede seguir golpeando) no lo mueven.
                _stepsUsed = outcome.EffectiveStrikes;
                _stepsCaptured = true;
            }
        }

        public void PauseOpened()
        {
            _paused = true;
            _pausedAt = _now();
        }

        public void PauseClosed()
        {
            if (!_paused)
            {
                return;
            }

            _pausedSeconds += _now() - _pausedAt;
            _paused = false;
        }

        /// <summary>Cierra el registro al converger (RF-20). El tiempo de resolución excluye la
        /// pausa abierta (RF-07).</summary>
        public PerformanceIndicators Complete() =>
            new PerformanceIndicators(_attempts, _correctedErrors, _stepsUsed,
                _now() - _startedAt - _pausedSeconds);
    }
}
