using Game.Scaffolding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Una ventana que encuadra su ilustración: aplica <see cref="IllustrationFraming"/> a la imagen
    /// hija <see cref="Art"/> contra el tamaño de esta ventana, como hace la escena narrativa con su
    /// ilustración. Es lo que deja ver «la mitad izquierda del bosque» en la portada del menú y en la
    /// tarjeta del Nivel 2 sin recortar el PNG ni duplicar el lienzo.
    /// </summary>
    /// <remarks>
    /// Quien quiera la ventana recortada le añade un <see cref="RectMask2D"/>: este componente solo
    /// mueve y escala la imagen. Lo que cuelgue de la imagen se coloca en fracciones de ella y
    /// acompaña el encuadre, igual que los objetos de una escena narrativa
    /// (<c>NarrativeProp.Position</c>).
    ///
    /// **Va en la ventana y no en la imagen** porque Unity solo avisa de que cambió el tamaño
    /// (<c>OnRectTransformDimensionsChange</c>) al objeto cuyo RectTransform cambia, y la imagen
    /// —anclada al centro con tamaño fijo— no cambia cuando la ventana sí: se probó en el motor el
    /// 08/10/2026 y la imagen se quedaba con la escala del primer cuadro, que es cero porque el
    /// <c>Canvas</c> aún no ha repartido el tamaño. La ventana sí lo recibe, así que el encuadre
    /// sigue valiendo cuando el <c>CanvasScaler</c> reparte otra proporción de pantalla (16:10, 4:3).
    ///
    /// Los lienzos de 3840 de ancho (el bosque del Nivel 2) tienen una costura en x = 0,5 que ningún
    /// encuadre debe cruzar: con el foco en 0.25 y una ventana no más ancha que 16:9 se ve solo la
    /// mitad izquierda. No se valida en <c>OnValidate</c> porque
    /// <see cref="IllustrationFraming.Warnings"/> da por hecha una ventana 16:9, y esta no lo es.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class FramedIllustration : UIBehaviour
    {
        [SerializeField]
        [Tooltip("La imagen que se encuadra: hija directa de esta ventana, con la ilustración como sprite.")]
        private Image art;

        [field: SerializeField]
        [field: Tooltip("Foco (fracciones de la ilustración, (0,0) abajo-izquierda) y acercamiento sobre el encuadre que cubre la ventana justo. En un lienzo de 3840×1080 el foco 0.25 muestra la mitad izquierda.")]
        public CameraFraming Framing { get; set; } = new CameraFraming();

        /// <summary>La imagen que se encuadra.</summary>
        public Image Art
        {
            get => art;
            set => art = value;
        }

        /// <summary>
        /// Encuadra la imagen contra el tamaño actual de la ventana. No hace nada sin imagen, sin sprite
        /// o con la ventana vacía: así no deja la imagen a escala cero antes de que el <c>Canvas</c>
        /// reparta el tamaño.
        /// </summary>
        public void Apply()
        {
            if (art == null || art.sprite == null || art.rectTransform.parent != transform)
            {
                return;
            }

            var viewport = ((RectTransform)transform).rect.size;
            if (viewport.x <= 0f || viewport.y <= 0f)
            {
                return;
            }

            IllustrationFraming.Apply(art.rectTransform, art.sprite.rect.size, Framing);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Apply();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (IsActive())
            {
                Apply();
            }
        }
    }
}
