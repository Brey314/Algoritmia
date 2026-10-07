namespace Game.Scaffolding
{
    /// <summary>La forma de la boca en un instante del habla (plan de personajes finales §5.3).</summary>
    public enum MouthShape
    {
        /// <summary>No habla: la boca de reposo de la emoción.</summary>
        Rest,

        /// <summary>Apertura amplia (A, O).</summary>
        A,

        /// <summary>Apertura ancha o media (E, I).</summary>
        E,

        /// <summary>Pequeña y redondeada (U, M, B, P).</summary>
        U,

        /// <summary>Cerrada, entre sílabas: sin ella el aleteo sería una boca que solo se abre.</summary>
        Closed
    }

    /// <summary>
    /// El aleteo de la boca mientras el personaje habla. Sin audio fonético, no sigue las sílabas:
    /// cicla A, E, U y cerrada a un ritmo fijo, en un orden fijo y sin aleatoriedad, así que la
    /// misma línea se ve igual cada vez. C# plano: se prueba en EditMode sin escena ni cuadros.
    /// </summary>
    public sealed class MouthFlap
    {
        private static readonly MouthShape[] Cycle = { MouthShape.A, MouthShape.E, MouthShape.U, MouthShape.Closed };

        private readonly float _flapSeconds;
        private bool _wasSpeaking;
        private float _elapsed;
        private int _index;

        /// <param name="flapSeconds">Cuánto dura cada posición de la boca.</param>
        public MouthFlap(float flapSeconds)
        {
            _flapSeconds = flapSeconds > 0.0001f ? flapSeconds : 0.0001f;
        }

        /// <summary>
        /// Avanza el reloj. Al empezar a hablar abre en A de inmediato; al dejar de hablar devuelve
        /// <see cref="MouthShape.Rest"/> en seco y el ciclo vuelve a empezar la próxima vez.
        /// </summary>
        public MouthShape Tick(float deltaSeconds, bool speaking)
        {
            if (!speaking)
            {
                _wasSpeaking = false;
                _elapsed = 0f;
                _index = 0;
                return MouthShape.Rest;
            }

            if (!_wasSpeaking)
            {
                _wasSpeaking = true;
                _elapsed = 0f;
                _index = 0;
                return Cycle[0];
            }

            _elapsed += deltaSeconds > 0f ? deltaSeconds : 0f;
            while (_elapsed >= _flapSeconds)
            {
                _elapsed -= _flapSeconds;
                _index = (_index + 1) % Cycle.Length;
            }

            return Cycle[_index];
        }
    }
}
