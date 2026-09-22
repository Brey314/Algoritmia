using UnityEngine;

namespace Game.Levels.River
{
    /// <summary>
    /// El desplazamiento de Mamá en dos dimensiones dentro de los límites del escenario (RF-35).
    /// </summary>
    /// <remarks>
    /// C# plano: posición y límites en fracciones de la ilustración, sin <c>Transform</c>, para
    /// probar en EditMode que nunca se sale. La dirección la ponen los botones en pantalla
    /// (<see cref="DirectionPad"/>); aquí no se lee ninguna entrada (CT-06, INC-01).
    /// </remarks>
    public class RiverWalk
    {
        private readonly float _speed;

        public RiverWalk(Vector2 start, Rect bounds, float speed)
        {
            Bounds = bounds;
            _speed = Mathf.Max(0f, speed);
            Position = Clamp(start);
        }

        /// <summary>Dónde está, en fracciones de la ilustración.</summary>
        public Vector2 Position { get; private set; }

        /// <summary>Por dónde se puede andar: la orilla, no el agua.</summary>
        public Rect Bounds { get; }

        /// <summary>Avanza en esa dirección durante ese tiempo. Se recorta a los límites, nunca los cruza.</summary>
        public void Step(Vector2 direction, float deltaTime)
        {
            if (direction.sqrMagnitude < 1e-6f || deltaTime <= 0f)
            {
                return;
            }

            Position = Clamp(Position + direction.normalized * (_speed * deltaTime));
        }

        private Vector2 Clamp(Vector2 point) => new Vector2(
            Mathf.Clamp(point.x, Bounds.xMin, Bounds.xMax),
            Mathf.Clamp(point.y, Bounds.yMin, Bounds.yMax));
    }
}
