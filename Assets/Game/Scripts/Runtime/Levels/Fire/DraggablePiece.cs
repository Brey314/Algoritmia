using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Una pieza del suelo de la cueva —hoja, sílex o pedernal— que se arrastra con clic sostenido
    /// (CT-06, RNF-02) y se queda donde se suelta. No sabe de reglas: solo se mueve.
    /// </summary>
    /// <remarks>
    /// Va por los eventos de arrastre de uGUI (<c>InputSystemUIInputModule</c>), no por la clase
    /// <c>Input</c> legada. Se recorta al rect del padre para que ninguna pieza salga de la cueva.
    /// </remarks>
    [RequireComponent(typeof(RectTransform))]
    public class DraggablePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [field: SerializeField]
        [field: Tooltip("Hoja, sílex o pedernal.")]
        public PieceKind Kind { get; private set; }

        private RectTransform _rect;
        private RectTransform _floor;
        private Vector2 _grabOffset;

        /// <summary>Posición en el suelo, en píxeles del lienzo de referencia, centrada en el padre.</summary>
        public Vector2 Position => Rect.anchoredPosition;

        private RectTransform Rect => _rect != null ? _rect : _rect = (RectTransform)transform;

        private RectTransform Floor => _floor != null ? _floor : _floor = (RectTransform)transform.parent;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _grabOffset = Rect.anchoredPosition - Local(eventData);
            Rect.SetAsLastSibling(); // lo que se arrastra va encima
        }

        public void OnDrag(PointerEventData eventData) => MoveTo(Local(eventData) + _grabOffset);

        public void OnEndDrag(PointerEventData eventData) => MoveTo(Local(eventData) + _grabOffset);

        /// <summary>Mueve la pieza, sin sacarla del suelo. Lo usan el arrastre y las pruebas.</summary>
        public void MoveTo(Vector2 position)
        {
            var half = Floor.rect.size / 2f - Rect.rect.size / 2f;
            Rect.anchoredPosition = new Vector2(
                Mathf.Clamp(position.x, -half.x, half.x),
                Mathf.Clamp(position.y, -half.y, half.y));
        }

        private Vector2 Local(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Floor, eventData.position, eventData.pressEventCamera, out var local);
            return local;
        }
    }
}
