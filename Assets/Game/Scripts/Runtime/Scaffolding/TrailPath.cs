using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>Un punto de <see cref="TrailPath"/>: posición y qué tan grande y opaco se ve.</summary>
    public readonly struct TrailPoint
    {
        public TrailPoint(Vector2 position, float scale, float alpha)
        {
            Position = position;
            Scale = scale;
            Alpha = alpha;
        }

        public Vector2 Position { get; }
        public float Scale { get; }
        public float Alpha { get; }
    }

    /// <summary>
    /// Guarda las últimas posiciones de un cuerpo en movimiento, a una distancia mínima entre sí, y
    /// devuelve de <see cref="MinPoints"/> a <see cref="MaxPoints"/> puntos con escala y alfa
    /// decrecientes hacia atrás: la estela de Algoritm (guion §1.1.1, Dirección de arte §7.6,
    /// INC-52). C# plano, sin MonoBehaviour: el adaptador es <see cref="GuideTrail"/>.
    /// </summary>
    /// <remarks>
    /// Por debajo de <see cref="MinPoints"/> muestras no hay estela que mostrar (un cuerpo quieto,
    /// o que recién empieza a moverse, no deja marca): así "sin movimiento, ninguno" no depende de
    /// contar frames sino de cuántas posiciones distintas se acumularon.
    /// </remarks>
    public class TrailPath
    {
        public const int MinPoints = 5;
        public const int MaxPoints = 7;

        /// <summary>Sin un movimiento aceptado en este tiempo, la estela se olvida (se para el cuerpo, para el punto).</summary>
        public const float StillSeconds = 0.3f;

        private readonly float _minDistance;
        private readonly List<Vector2> _samples = new List<Vector2>(MaxPoints);
        private float _lastMovedAt;

        public TrailPath(float minDistance = 24f)
        {
            _minDistance = minDistance;
        }

        /// <summary>
        /// Registra una posición si se movió al menos la distancia mínima desde la última.
        /// <paramref name="time"/> es opcional (0 no rompe las pruebas que no lo pasan): cuando se
        /// pasa un reloj real, un cuerpo quieto más de <see cref="StillSeconds"/> olvida la estela
        /// en vez de dejarla congelada en su último punto.
        /// </summary>
        public void Sample(Vector2 position, float time = 0f)
        {
            if (_samples.Count > 0 && Vector2.Distance(_samples[^1], position) < _minDistance)
            {
                if (time - _lastMovedAt > StillSeconds)
                {
                    Clear();
                }

                return;
            }

            _samples.Add(position);
            _lastMovedAt = time;
            if (_samples.Count > MaxPoints)
            {
                _samples.RemoveAt(0);
            }
        }

        /// <summary>Olvida las muestras guardadas: la estela desaparece de inmediato.</summary>
        public void Clear()
        {
            _samples.Clear();
        }

        /// <summary>
        /// De la más vieja a la más nueva. Vacío si aún no hay <see cref="MinPoints"/> muestras.
        /// </summary>
        public IReadOnlyList<TrailPoint> Points
        {
            get
            {
                var count = _samples.Count;
                if (count < MinPoints)
                {
                    return Array.Empty<TrailPoint>();
                }

                var points = new TrailPoint[count];
                for (var i = 0; i < count; i++)
                {
                    var t = (float)(i + 1) / count; // la más nueva (i = count-1) llega a 1
                    points[i] = new TrailPoint(_samples[i], t, t);
                }

                return points;
            }
        }
    }
}
