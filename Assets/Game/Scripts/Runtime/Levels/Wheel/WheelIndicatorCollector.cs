using System;
using Game.Core;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// Los cuatro indicadores del Nivel 2 (RF-45, OE1 §3.6.1), con la definición operativa de
    /// **cada fase** —que en este nivel es distinta en las tres—. Nunca se muestran al estudiante
    /// (CP-03): solo llegan a <see cref="PlayerProfile.ConfirmPhase"/>.
    /// </summary>
    /// <remarks>
    /// C# plano, sin escena: se prueba en EditMode con un reloj inyectado, igual que el recolector
    /// del Nivel 1. Un solo tipo para las tres fases porque la mecánica de registro es la misma
    /// —una acción se acepta o se rechaza; una ejecución llega o no— y lo que cambia por fase es
    /// qué cuenta como paso, que se decide aquí y no en tres clases:
    ///
    /// | Indicador | Fase 1 (bosque) | Fase 2 (taller) | Fase 3 (laberinto) |
    /// |---|---|---|---|
    /// | Intentos | selecciones de un objeto no válido | acciones rechazadas por estar fuera de secuencia | ejecuciones que no alcanzan el refugio |
    /// | Errores corregidos | rechazo seguido de la acción correcta | ídem | ediciones de la secuencia entre una ejecución fallida y la siguiente |
    /// | Pasos utilizados | **sin definición en §3.6.1** (pregunta abierta 1): se emite 0 | acciones de ensamblaje aceptadas | bloques de la secuencia que llegó |
    /// | Tiempo | reloj menos la ventana de pausa (nota 1) | ídem | ídem |
    ///
    /// Implementa <see cref="ILevelReporter"/> para que <c>PauseMenuController</c> (Game.UI) le
    /// notifique la pausa sin que ningún assembly gane una referencia hacia el otro — la
    /// mediación es <see cref="GameFlowRunner.ActiveReporter"/>.
    /// </remarks>
    public class WheelIndicatorCollector : ILevelReporter
    {
        private readonly int _phase;
        private readonly Func<float> _now;
        private readonly float _startedAt;

        private int _attempts;
        private int _correctedErrors;
        private int _stepsUsed;
        private bool _lastActionRejected;
        private bool _lastExecutionFailed;
        private int _editsSinceFailure;

        private float _pausedSeconds;
        private float _pausedAt;
        private bool _paused;

        /// <param name="phase">Fase del Nivel 2, en base 1: decide qué cuenta como paso.</param>
        /// <param name="now">
        /// Reloj inyectado (producción: <c>() => Time.realtimeSinceStartup</c>) para que el
        /// tiempo de resolución se pueda probar en EditMode sin esperas reales.
        /// </param>
        public WheelIndicatorCollector(int phase, Func<float> now)
        {
            _phase = phase;
            _now = now;
            _startedAt = now();
        }

        /// <summary>Fases 1 y 2: un objeto no válido o un paso fuera de secuencia. Suma un intento.</summary>
        public void RecordRejected()
        {
            _attempts++;
            _lastActionRejected = true;
        }

        /// <summary>
        /// Fases 1 y 2: una acción aceptada. Si sigue a un rechazo es un error corregido; en la
        /// fase 2 es además un paso de ensamblaje. En la fase 1 no suma pasos: §3.6.1 no define
        /// «Pasos utilizados» para el bosque y eso no se decide desde el código (pregunta abierta 1).
        /// </summary>
        public void RecordAccepted()
        {
            if (_lastActionRejected)
            {
                _correctedErrors++;
            }

            _lastActionRejected = false;
            if (_phase == 2)
            {
                _stepsUsed++;
            }
        }

        /// <summary>Fase 3: una edición de la secuencia —retirar, reordenar, cambiar cuenta o lado, añadir—.</summary>
        public void RecordEdit()
        {
            if (_lastExecutionFailed)
            {
                _editsSinceFailure++;
            }
        }

        /// <summary>
        /// Fase 3: una ejecución. Las ediciones hechas desde la última ejecución fallida se
        /// contabilizan como errores corregidos al ejecutar de nuevo, llegue o no: lo que §3.6.1
        /// mide es que el jugador cambió la hipótesis, no que acertara a la primera.
        /// </summary>
        public void RecordExecution(bool reachedGoal, int blocks)
        {
            if (_lastExecutionFailed)
            {
                _correctedErrors += _editsSinceFailure;
            }

            _editsSinceFailure = 0;
            _lastExecutionFailed = !reachedGoal;
            if (reachedGoal)
            {
                _stepsUsed = blocks;
            }
            else
            {
                _attempts++;
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

        /// <summary>Cierra el registro al completar la fase. El tiempo excluye la pausa abierta (RF-07).</summary>
        public PerformanceIndicators Complete() =>
            new PerformanceIndicators(_attempts, _correctedErrors, _stepsUsed,
                _now() - _startedAt - _pausedSeconds);
    }
}
