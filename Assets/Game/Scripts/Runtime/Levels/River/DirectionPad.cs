using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels.River
{
    /// <summary>
    /// Un botón de dirección en pantalla (RF-35, INC-01): mientras se sostiene el clic sobre él,
    /// Mamá avanza en su dirección. Uno por flecha.
    /// </summary>
    /// <remarks>
    /// **Clic sostenido, no teclado y no arrastre.** Implementa solo <see cref="IPointerDownHandler"/>
    /// e <see cref="IPointerUpHandler"/> —el mismo esquema que el asa de la caja del Nivel 2— y
    /// ninguna acción del Input System: el mapa de controles del juego no tiene, ni puede tener,
    /// una vinculación de teclado (RNF-02, CT-06). Quien «mejore» esto con <c>Keyboard.current</c>
    /// o con una acción <c>Move</c> rompe INC-01, que R07 prueba sobre el asset de acciones.
    ///
    /// El <c>EventSystem</c> entrega el soltar al objeto que recibió el pulsar aunque el cursor
    /// ya se haya ido: soltar fuera del botón también detiene el paso.
    /// </remarks>
    public class DirectionPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        [Tooltip("Hacia dónde empuja, en fracciones de la ilustración: (0,1) arriba, (1,0) derecha…")]
        private Vector2 direction = Vector2.up;

        public Vector2 Direction => direction;

        /// <summary>Si el clic sigue sostenido sobre el botón.</summary>
        public bool IsHeld { get; private set; }

        public void OnPointerDown(PointerEventData eventData) => IsHeld = true;

        public void OnPointerUp(PointerEventData eventData) => IsHeld = false;

        /// <summary>Al ocultar el botón —pausa, ensamblaje— el paso no puede quedarse pegado.</summary>
        private void OnDisable() => IsHeld = false;
    }
}
