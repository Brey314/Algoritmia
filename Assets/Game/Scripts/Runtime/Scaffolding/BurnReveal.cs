using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>
    /// Quema una imagen desde un punto: un círculo-máscara centrado en <see cref="Origin"/> descubre
    /// una copia de la misma imagen teñida de brasa, y <see cref="Extent"/> dice hasta dónde llega.
    /// Sirve al montón de hojas del Nivel 1 —crece al soplar y se queda— y al del cierre narrativo,
    /// donde nace ya quieto (pedido de Santiago, 22/09/2026).
    /// </summary>
    /// <remarks>
    /// Va con <c>Mask</c> y no con un shader: el recorte por esténcil ya existe en uGUI y respeta el
    /// alfa del disco, así que el borde sigue la forma sin material propio. El disco se genera en
    /// memoria —no hay que cablear un sprite— y se comparte entre instancias. El círculo solo cambia
    /// de tamaño cuando se lo piden: nada parpadea (RNF-21).
    /// </remarks>
    [RequireComponent(typeof(Image))]
    public class BurnReveal : MonoBehaviour
    {
        private static Sprite s_disc;

        /// <summary>Color con el que se tiñe la copia quemada.</summary>
        [field: SerializeField]
        [field: Tooltip("Color con el que se tiñe la copia quemada de la imagen.")]
        public Color EmberColor { get; set; } = new Color(0.45f, 0.16f, 0.06f);

        /// <summary>Desde dónde se quema, en píxeles locales respecto al centro de la imagen.</summary>
        [field: SerializeField]
        [field: Tooltip("Desde dónde se quema, en píxeles locales respecto al centro de la imagen: donde cae la llama.")]
        public Vector2 Origin { get; set; }

        private RectTransform _mask;
        private float _extent;

        /// <summary>
        /// Hasta dónde llega lo quemado, como fracción del ancho de la imagen: 0 nada, 1 un círculo
        /// tan ancho como ella. Se puede fijar antes de <c>Awake</c>; se aplica en cuanto existe la máscara.
        /// </summary>
        public float Extent
        {
            get => _extent;
            set
            {
                _extent = Mathf.Max(value, 0f);
                if (_mask != null)
                {
                    Apply();
                }
            }
        }

        private void Awake()
        {
            var image = GetComponent<Image>();
            var rect = (RectTransform)transform;
            var center = new Vector2(0.5f, 0.5f);

            var maskObject = new GameObject("Brasa", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
            maskObject.layer = gameObject.layer;
            _mask = (RectTransform)maskObject.transform;
            _mask.SetParent(rect, false);
            _mask.anchorMin = _mask.anchorMax = center;
            _mask.pivot = center;
            _mask.anchoredPosition = Origin;
            var maskImage = maskObject.GetComponent<Image>();
            maskImage.sprite = Disc;
            maskImage.raycastTarget = false;
            maskObject.GetComponent<Mask>().showMaskGraphic = false;

            // La copia va centrada en la imagen, no en la máscara: se compensa el origen.
            var embersObject = new GameObject("Quemado", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            embersObject.layer = gameObject.layer;
            var embers = (RectTransform)embersObject.transform;
            embers.SetParent(_mask, false);
            embers.anchorMin = embers.anchorMax = center;
            embers.pivot = center;
            embers.sizeDelta = rect.rect.size;
            embers.anchoredPosition = -Origin;
            var embersImage = embersObject.GetComponent<Image>();
            embersImage.sprite = image.sprite;
            embersImage.preserveAspect = image.preserveAspect;
            embersImage.color = EmberColor;
            embersImage.raycastTarget = false;

            Apply();
        }

        private void Apply() =>
            _mask.sizeDelta = Vector2.one * (((RectTransform)transform).rect.width * _extent);

        private static Sprite Disc => s_disc != null ? s_disc : s_disc = CreateDisc(64);

        /// <summary>Un disco blanco con un píxel de suavizado en el borde, al estilo del anillo de reunión del N1.</summary>
        private static Sprite CreateDisc(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "DiscoBrasa",
                filterMode = FilterMode.Bilinear
            };
            var pixels = new Color32[size * size];
            var center = (size - 1) / 2f;
            var radius = size / 2f;
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)(Mathf.Clamp01(radius - distance) * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
