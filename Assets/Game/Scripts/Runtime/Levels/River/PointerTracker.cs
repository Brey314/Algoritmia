using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels.River
{
    /// <summary>
    /// Dónde está el cursor, leído del <c>EventSystem</c> y no de ningún dispositivo. Va en la
    /// raíz del lienzo: cada movimiento del puntero sobre cualquier elemento sube por la
    /// jerarquía hasta aquí.
    /// </summary>
    /// <remarks>
    /// Por qué no <c>Mouse.current</c> como el Nivel 2: <c>Game.Levels.River</c> no referencia el
    /// Input System a propósito, y una prueba lo vigila (INC-01, CT-06). Y por qué no el arrastre
    /// de uGUI: es la interfaz que RNF-02 prohíbe. <see cref="IPointerMoveHandler"/> es un
    /// evento de puntero, no de arrastre, y lo entrega el módulo de entrada ya configurado.
    /// </remarks>
    public class PointerTracker : MonoBehaviour, IPointerMoveHandler
    {
        /// <summary>La última posición conocida del puntero, en píxeles de pantalla.</summary>
        public Vector2 ScreenPosition { get; private set; }

        public void OnPointerMove(PointerEventData eventData) => ScreenPosition = eventData.position;

        /// <summary>Siembra la posición con la del clic que agarró la pieza.</summary>
        public void Seed(Vector2 screenPosition) => ScreenPosition = screenPosition;
    }
}
