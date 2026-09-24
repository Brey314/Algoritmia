using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Lo que un personaje hace en una línea, ya resuelto por <see cref="ActorTimeline"/>: la escena
    /// lo ejecuta sin decidir nada.
    /// </summary>
    public readonly struct ActorCue
    {
        public ActorCue(Vector2 from, Vector2 to, bool moves, float seconds, ActorAction during, ActorAction after)
        {
            From = from;
            To = to;
            Moves = moves;
            Seconds = seconds;
            During = during;
            After = after;
        }

        /// <summary>Dónde está al empezar la línea, en fracciones de la ilustración.</summary>
        public Vector2 From { get; }

        /// <summary>Dónde queda al terminar su movimiento; igual a <see cref="From"/> si no se mueve.</summary>
        public Vector2 To { get; }

        public bool Moves { get; }

        /// <summary>Cuánto dura el movimiento, en segundos.</summary>
        public float Seconds { get; }

        /// <summary>Lo que hace mientras se mueve, o durante toda la línea si no se mueve.</summary>
        public ActorAction During { get; }

        /// <summary>Lo que hace al llegar.</summary>
        public ActorAction After { get; }
    }
}
