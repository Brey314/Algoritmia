using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Hacia qué lado mira de perfil quien se mueve en una dirección (Santiago, 09/10/2026, INC-134).
    /// Una sola función pura para las narrativas y para el río: así no puede haber dos versiones de la
    /// regla. C# plano.
    /// </summary>
    /// <remarks>
    /// **La regla.** A la izquierda o hacia arriba, perfil izquierdo; a la derecha o hacia abajo,
    /// perfil derecho. En una diagonal decide el eje dominante: con |Δx| ≥ |Δy| manda el horizontal
    /// (un empate también), y si no, el vertical —arriba mira a la izquierda, abajo a la derecha—.
    /// «Arriba» es y positiva, como en las fracciones de la ilustración y en las flechas del río.
    ///
    /// **Por qué el vertical también gira:** de perfil solo hay dos lados, y quien camina «hacia el
    /// fondo» o «hacia el espectador» tiene que mirar a alguno. Hasta INC-134 un paso vertical
    /// conservaba el último lado; ahora es una regla fija y no depende de lo que hizo antes.
    ///
    /// **El arte de perfil está dibujado mirando a la derecha;** mirar a la izquierda es voltear el
    /// lienzo (<see cref="CharacterRig.Mirrored"/>). Esta clase solo dice el lado; no toca nada.
    /// </remarks>
    public static class Heading
    {
        /// <summary>Por debajo de esto (en longitud al cuadrado) un vector se toma por cero: no decide nada.</summary>
        private const float ZeroSqrLength = 1e-10f;

        /// <summary>
        /// Si quien se mueve en <paramref name="delta"/> mira a la izquierda (<c>true</c>) o a la
        /// derecha (<c>false</c>). <c>null</c> con un vector nulo: no hay rumbo y quien llama conserva
        /// el anterior.
        /// </summary>
        public static bool? FacesLeft(Vector2 delta)
        {
            if (delta.sqrMagnitude < ZeroSqrLength)
            {
                return null;
            }

            var horizontal = Mathf.Abs(delta.x);
            var vertical = Mathf.Abs(delta.y);
            if (horizontal >= vertical)
            {
                return delta.x < 0f;
            }

            return delta.y > 0f;
        }

        /// <summary>
        /// Si hay que mirar a la izquierda para encarar un objetivo que está en <paramref name="toX"/>
        /// desde <paramref name="fromX"/> (cualquier unidad, con tal de que sea la misma): quien trabaja
        /// sobre algo —Papá sobre el montón, la Niña empujando la caja— lo mira de frente en el
        /// horizontal. Si ambos están en la misma vertical, sigue mirando a donde miraba
        /// (<paramref name="fallback"/>): no tiene sentido girar por una diferencia que no se ve.
        /// </summary>
        public static bool FacesLeftToward(float fromX, float toX, bool fallback) =>
            FacesLeft(new Vector2(toX - fromX, 0f)) ?? fallback;
    }
}
