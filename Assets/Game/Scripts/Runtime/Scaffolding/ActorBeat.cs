using System;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Un paso de un personaje en una escena narrativa: desde qué línea hace qué, y si se mueve,
    /// a dónde (RF-05). Es la acotación del guion puesta como contenido: «el niño se arrodilla y
    /// busca las piedras» es un paso en la línea 2 con <see cref="ActorAction.Kneel"/>.
    /// </summary>
    /// <remarks>
    /// Contenido y no código (CT-05, RNF-18): el destino va en fracciones de la ilustración, igual
    /// que <see cref="NarrativeProp.Position"/>, así que acompaña el paneo y el zoom de la cámara.
    /// </remarks>
    [Serializable]
    public class ActorBeat
    {
        [field: SerializeField]
        [field: Tooltip("Línea, desde 0, en la que empieza el paso. Lo que dice el texto se ve cuando se lee.")]
        public int Line { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Qué hace desde esa línea hasta el paso siguiente. Si se mueve, es lo que hace mientras avanza (caminar, correr, cargar, empujar).")]
        public ActorAction Action { get; private set; } = ActorAction.Idle;

        [field: SerializeField]
        [field: Tooltip("Si el personaje se desplaza hasta el destino en esta línea.")]
        public bool Moves { get; private set; }

        [field: SerializeField]
        [field: Tooltip("A dónde llega, en fracciones de la ilustración: el centro de su casilla, como la posición del objeto.")]
        public Vector2 Destination { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Cuánto tarda en llegar, en segundos.")]
        public float Seconds { get; private set; } = 2f;

        [field: SerializeField]
        [field: Tooltip("Qué hace al llegar, hasta el paso siguiente. Solo cuenta si se mueve.")]
        public ActorAction Arrival { get; private set; } = ActorAction.Idle;

        /// <summary>Requerido por la serialización de Unity.</summary>
        private ActorBeat()
        {
        }

        public ActorBeat(int line, ActorAction action)
        {
            Line = line;
            Action = action;
        }

        public ActorBeat MovingTo(Vector2 destination, float seconds, ActorAction arrival = ActorAction.Idle)
        {
            Moves = true;
            Destination = destination;
            Seconds = seconds;
            Arrival = arrival;
            return this;
        }
    }
}
