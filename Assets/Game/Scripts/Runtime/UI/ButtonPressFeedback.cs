using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Hunde la cara de un botón mientras se lo mantiene presionado: es la respuesta al clic y al
    /// clic sostenido (RNF-02), y lo único que confirma al estudiante que el toque llegó.
    /// </summary>
    /// <remarks>
    /// Geometría del mockup de interfaz y §10.2 de la dirección de arte: la sombra plana inferior
    /// mide seis píxeles en reposo y dos al presionar, así que la cara baja los cuatro de
    /// diferencia y el borde inferior de la sombra no se mueve.
    ///
    /// No se usa <see cref="Selectable.transition"/>: sus tres modos tiñen, cambian de sprite o
    /// disparan un <c>Animator</c>, y ninguno desplaza el <c>RectTransform</c> del hijo, que es lo
    /// que pide el mockup. Tampoco hay interpolación por fotograma —el desfase se aplica de una—:
    /// son cuatro píxeles y animarlos costaría un <c>Update</c> permanente por botón.
    /// </remarks>
    [RequireComponent(typeof(Button))]
    public class ButtonPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
        IPointerExitHandler
    {
        [SerializeField]
        [Tooltip("La cara del botón. Si se deja vacía, se toma el primer hijo.")]
        private RectTransform face;

        /// <summary>Píxeles que baja la cara al presionar: la sombra pasa de seis a dos.</summary>
        [field: SerializeField]
        [field: Tooltip("Píxeles que baja la cara del botón al presionarlo.")]
        public float PressDepth { get; set; } = 4f;

        private bool pressed;

        /// <summary>
        /// La anatomía del botón es uniforme en las cuatro pantallas: la raíz lleva el contorno y
        /// la sombra, y el primer hijo es la cara. Resolverla así evita cablear a mano una
        /// referencia por botón sin perder la posibilidad de serializar otra.
        /// </summary>
        private RectTransform Face =>
            face != null ? face :
            transform.childCount > 0 ? transform.GetChild(0) as RectTransform : null;

        /// <inheritdoc/>
        public void OnPointerDown(PointerEventData eventData) => SetPressed(true);

        /// <inheritdoc/>
        public void OnPointerUp(PointerEventData eventData) => SetPressed(false);

        /// <inheritdoc/>
        public void OnPointerExit(PointerEventData eventData) => SetPressed(false);

        /// <summary>
        /// Aplica o revierte el desfase. El estado se guarda en vez de recordar la posición de
        /// reposo: un clic sostenido repite <c>OnPointerDown</c>, y sumar dos veces hundiría la
        /// cara fuera del botón.
        /// </summary>
        private void SetPressed(bool value)
        {
            if (pressed == value || Face is not { } target)
            {
                return;
            }

            // Un botón bloqueado —el nivel aún no desbloqueado del menú (RF-03)— no se hunde:
            // hundirse promete que el clic hizo algo.
            if (value && !GetComponent<Button>().interactable)
            {
                return;
            }

            pressed = value;
            target.anchoredPosition += new Vector2(0f, value ? -PressDepth : PressDepth);
        }
    }
}
