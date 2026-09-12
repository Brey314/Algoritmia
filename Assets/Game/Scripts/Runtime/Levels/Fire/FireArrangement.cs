using System.Collections.Generic;
using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Dónde están las piezas respecto del punto del fuego (Fase 5, T22): las hojas hacen montón
    /// cuando todas caen dentro del radio del montón, y las piedras están «cerca» cuando las dos
    /// caen dentro del radio de las piedras. Todo en la misma unidad que las posiciones.
    /// </summary>
    /// <remarks>
    /// C# plano: se prueba en EditMode sin escena. El panel solo le pasa posiciones; la regla de
    /// qué cuenta como «cerca» vive aquí y sus radios en <see cref="FireLevelConfig"/>.
    /// </remarks>
    public class FireArrangement
    {
        private readonly Vector2 _spot;
        private readonly float _pileRadius;
        private readonly float _stonesRadius;

        public FireArrangement(Vector2 spot, float pileRadius, float stonesRadius)
        {
            _spot = spot;
            _pileRadius = pileRadius;
            _stonesRadius = stonesRadius;
        }

        /// <summary>Todas las hojas dentro del radio del montón. Sin hojas no hay montón.</summary>
        public bool IsPiled(IReadOnlyList<Vector2> leaves)
        {
            if (leaves.Count == 0)
            {
                return false;
            }

            foreach (var leaf in leaves)
            {
                if (!Within(leaf, _pileRadius))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>El sílex y el pedernal, los dos, dentro del radio de las piedras.</summary>
        public bool StonesNear(Vector2 silex, Vector2 pedernal) =>
            Within(silex, _stonesRadius) && Within(pedernal, _stonesRadius);

        private bool Within(Vector2 position, float radius) => (position - _spot).sqrMagnitude <= radius * radius;
    }
}
