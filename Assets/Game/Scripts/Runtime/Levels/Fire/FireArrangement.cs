using System.Collections.Generic;
using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Si los materiales están reunidos (Fase 6, T25): todas las piezas —hojas, sílex y pedernal—
    /// dentro del círculo del centro de la pantalla. Todo en la misma unidad que las posiciones.
    /// </summary>
    /// <remarks>
    /// C# plano: se prueba en EditMode sin escena. El panel solo le pasa posiciones; el radio lo
    /// mide el panel a partir del botón de abajo (el borde del círculo queda a la altura de la
    /// mitad del botón), así que no es un parámetro del asset.
    /// </remarks>
    public class FireArrangement
    {
        private readonly Vector2 _spot;
        private readonly float _radius;

        public FireArrangement(Vector2 spot, float radius)
        {
            _spot = spot;
            _radius = radius;
        }

        /// <summary>Todas las piezas dentro del círculo. Sin piezas no hay nada reunido.</summary>
        public bool IsGathered(IReadOnlyList<Vector2> pieces)
        {
            if (pieces.Count == 0)
            {
                return false;
            }

            foreach (var piece in pieces)
            {
                if ((piece - _spot).sqrMagnitude > _radius * _radius)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
