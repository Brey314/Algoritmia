using System.Collections.Generic;

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
    /// cicla las bocas del habla y la cerrada a un ritmo fijo, en un orden fijo y sin aleatoriedad, así
    /// que la misma línea se ve igual cada vez. C# plano: se prueba en EditMode sin escena ni cuadros.
    /// </summary>
    /// <remarks>
    /// El ciclo clásico es A, E, U y cerrada. El arte final (INC-148, 10/10/2026) trae **una sola boca
    /// de hablar** (la A): con ella el ciclo es A y cerrada, y la boca alterna abierta y cerrada cada
    /// <c>flapSeconds</c>. <see cref="CycleFor"/> arma el ciclo con las bocas que el set tiene.
    /// </remarks>
    public sealed class MouthFlap
    {
        private static readonly MouthShape[] ClassicCycle = { MouthShape.A, MouthShape.E, MouthShape.U, MouthShape.Closed };

        private readonly float _flapSeconds;
        private readonly MouthShape[] _cycle;
        private bool _wasSpeaking;
        private float _elapsed;
        private int _index;

        /// <summary>El ciclo clásico: A, E, U y cerrada.</summary>
        /// <param name="flapSeconds">Cuánto dura cada posición de la boca.</param>
        public MouthFlap(float flapSeconds) : this(flapSeconds, ClassicCycle)
        {
        }

        /// <param name="flapSeconds">Cuánto dura cada posición de la boca.</param>
        /// <param name="cycle">Las formas que recorre, en orden; vacío o nulo = el ciclo clásico.</param>
        public MouthFlap(float flapSeconds, MouthShape[] cycle)
        {
            _flapSeconds = flapSeconds > 0.0001f ? flapSeconds : 0.0001f;
            _cycle = cycle != null && cycle.Length > 0 ? (MouthShape[])cycle.Clone() : ClassicCycle;
        }

        /// <summary>
        /// El ciclo de un personaje según las bocas del habla que su set trae: las que tenga, en el orden
        /// A, E, U, y al final la cerrada, que separa las sílabas. Con una sola boca (la A del arte final)
        /// es A y cerrada. Si no trae ninguna devuelve el ciclo clásico: <see cref="CharacterFace"/> cae al
        /// reposo de la emoción en las formas que el set no tiene, igual que antes.
        /// </summary>
        public static MouthShape[] CycleFor(bool hasA, bool hasE, bool hasU)
        {
            var shapes = new List<MouthShape>(4);
            if (hasA)
            {
                shapes.Add(MouthShape.A);
            }

            if (hasE)
            {
                shapes.Add(MouthShape.E);
            }

            if (hasU)
            {
                shapes.Add(MouthShape.U);
            }

            if (shapes.Count == 0)
            {
                return (MouthShape[])ClassicCycle.Clone();
            }

            shapes.Add(MouthShape.Closed);
            return shapes.ToArray();
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
                return _cycle[0];
            }

            _elapsed += deltaSeconds > 0f ? deltaSeconds : 0f;
            while (_elapsed >= _flapSeconds)
            {
                _elapsed -= _flapSeconds;
                _index = (_index + 1) % _cycle.Length;
            }

            return _cycle[_index];
        }
    }
}
