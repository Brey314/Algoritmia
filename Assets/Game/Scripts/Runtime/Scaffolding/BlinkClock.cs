using System;

namespace Game.Scaffolding
{
    /// <summary>Qué tan cerrados están los ojos en un instante del parpadeo.</summary>
    public enum BlinkPhase
    {
        Open,
        Half,
        Closed
    }

    /// <summary>
    /// El reloj del parpadeo espontáneo: cada <c>interval ± jitter</c> segundos los ojos se cierran
    /// durante <c>duration</c> (plan de personajes finales §5.2). C# plano con la aleatoriedad
    /// inyectada, para probarlo en EditMode con una semilla fija y sin <c>UnityEngine.Random</c>.
    /// </summary>
    /// <remarks>
    /// El plan dice «abierto → medio → cerrado → abierto». Aquí el cierre es
    /// <see cref="BlinkPhase.Half"/> → <see cref="BlinkPhase.Closed"/> → <see cref="BlinkPhase.Half"/>
    /// y luego abierto, repartido en tres tercios de la duración: un párpado que baja y sube pasa
    /// dos veces por la mitad, y abrir de golpe desde cerrado se vería como un tic. Lo que dura
    /// cerrado el parpadeo completo sigue siendo <c>duration</c>.
    /// </remarks>
    public sealed class BlinkClock
    {
        private readonly float _interval;
        private readonly float _jitter;
        private readonly float _duration;
        private readonly Func<float> _random;

        private float _untilBlink;
        private float _blinking; // segundos transcurridos dentro del parpadeo; < 0 si no parpadea
        private bool _enabled = true;

        /// <param name="random">Devuelve un número en [0, 1). Con un valor fijo, el intervalo es predecible.</param>
        public BlinkClock(float interval, float jitter, float duration, Func<float> random)
        {
            _interval = Math.Max(interval, 0f);
            _jitter = Math.Max(jitter, 0f);
            _duration = Math.Max(duration, 0.0001f);
            _random = random ?? (() => 0.5f);
            _blinking = -1f;
            _untilBlink = NextInterval();
        }

        /// <summary>
        /// Si parpadea. Apagado, los ojos quedan siempre abiertos: la cara lo apaga mientras duerme,
        /// porque esos ojos ya son los cerrados.
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                {
                    return;
                }

                _enabled = value;
                _blinking = -1f;
                _untilBlink = NextInterval();
            }
        }

        /// <summary>Avanza el reloj y dice cómo están los ojos ahora.</summary>
        public BlinkPhase Tick(float deltaSeconds)
        {
            if (!_enabled)
            {
                return BlinkPhase.Open;
            }

            var remaining = Math.Max(deltaSeconds, 0f);
            if (_blinking < 0f)
            {
                if (remaining < _untilBlink)
                {
                    _untilBlink -= remaining;
                    return BlinkPhase.Open;
                }

                remaining -= _untilBlink;
                _blinking = 0f;
            }

            _blinking += remaining;
            if (_blinking >= _duration)
            {
                // Terminó: el siguiente se cuenta desde el final del parpadeo, no desde su inicio.
                _blinking = -1f;
                _untilBlink = NextInterval();
                return BlinkPhase.Open;
            }

            var third = _blinking / _duration * 3f;
            return third < 1f || third >= 2f ? BlinkPhase.Half : BlinkPhase.Closed;
        }

        private float NextInterval()
        {
            var unit = Math.Min(Math.Max(_random(), 0f), 1f);
            return Math.Max(_interval + (unit * 2f - 1f) * _jitter, 0f);
        }
    }
}
