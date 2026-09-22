using System.Collections.Generic;
using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Reparte las piezas por el suelo al azar sin pisar la interfaz ni el círculo de reunión y
    /// sin encimarse entre ellas (pedido de Santiago, 22/09/2026). C# plano: se prueba sin escena.
    /// </summary>
    /// <remarks>
    /// Muestreo por rechazo: cada pieza prueba posiciones al azar dentro del suelo y se queda con la
    /// primera que no choca con nada; si agota los intentos se queda con la última — una pieza mal
    /// puesta se arrastra igual, un nivel que no arranca no.
    /// </remarks>
    public static class FloorScatter
    {
        /// <param name="floor">El suelo, en el espacio en que se devuelven las posiciones.</param>
        /// <param name="blocked">Zonas que ninguna pieza puede pisar: la interfaz y el círculo de reunión.</param>
        /// <param name="sizes">Lado de la huella cuadrada de cada pieza, en el mismo espacio.</param>
        /// <param name="random">Fuente de azar; las pruebas pasan una con semilla.</param>
        /// <param name="attempts">Intentos por pieza antes de conformarse con la última muestra.</param>
        /// <returns>El centro de cada pieza, en el orden de <paramref name="sizes"/>.</returns>
        public static Vector2[] Place(
            Rect floor, IReadOnlyList<Rect> blocked, IReadOnlyList<float> sizes, System.Random random, int attempts = 200)
        {
            var placed = new List<Rect>(sizes.Count);
            var positions = new Vector2[sizes.Count];
            for (var i = 0; i < sizes.Count; i++)
            {
                var half = sizes[i] / 2f;
                var candidate = default(Rect);
                for (var attempt = 0; attempt < attempts; attempt++)
                {
                    var x = Mathf.Lerp(floor.xMin + half, floor.xMax - half, (float)random.NextDouble());
                    var y = Mathf.Lerp(floor.yMin + half, floor.yMax - half, (float)random.NextDouble());
                    candidate = new Rect(x - half, y - half, sizes[i], sizes[i]);
                    if (!Collides(candidate, blocked) && !Collides(candidate, placed))
                    {
                        break;
                    }
                }

                placed.Add(candidate);
                positions[i] = candidate.center;
            }

            return positions;
        }

        private static bool Collides(Rect rect, IReadOnlyList<Rect> others)
        {
            foreach (var other in others)
            {
                if (rect.Overlaps(other))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
