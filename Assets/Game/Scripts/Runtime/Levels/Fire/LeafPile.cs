using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Convierte la imagen de una hoja en un montoncito: al arrancar clona su propio sprite en
    /// hijos girados al azar y casi encimados, para que cada pieza que se arrastra se lea como
    /// varias hojas secas y no como una sola pegatina (pedido de Santiago, 22/09/2026).
    /// </summary>
    /// <remarks>
    /// Solo presentación: el arrastre lo sigue haciendo <see cref="DraggablePiece"/> en el padre —
    /// uGUI busca el manejador subiendo por la jerarquía, así que los hijos amplían la zona de
    /// agarre sin código propio. Se hace en código y no en la escena para que una hoja siga siendo
    /// un objeto que mantener, y para que el desorden sea distinto en cada partida.
    /// </remarks>
    [RequireComponent(typeof(Image))]
    public class LeafPile : MonoBehaviour
    {
        /// <summary>Cuántas hojas se ven en el montón, contando la propia.</summary>
        [field: SerializeField]
        [field: Tooltip("Cuántas hojas se ven en el montón, contando la propia.")]
        public int Leaves { get; set; } = 3;

        /// <summary>Hasta dónde se aparta cada hoja del centro, como fracción del ancho de la pieza.</summary>
        [field: SerializeField]
        [field: Tooltip("Hasta dónde se aparta cada hoja del centro, como fracción del ancho de la pieza. Pequeño: casi encimadas.")]
        public float Spread { get; set; } = 0.28f;

        private void Awake()
        {
            var image = GetComponent<Image>();
            var rect = (RectTransform)transform;
            for (var i = 1; i < Leaves; i++)
            {
                var leaf = new GameObject($"{name}_{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                leaf.layer = gameObject.layer;
                var leafRect = (RectTransform)leaf.transform;
                leafRect.SetParent(rect, false);
                leafRect.sizeDelta = rect.rect.size;
                leafRect.anchoredPosition = Random.insideUnitCircle * (rect.rect.width * Spread);
                leafRect.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

                var copy = leaf.GetComponent<Image>();
                copy.sprite = image.sprite;
                copy.preserveAspect = image.preserveAspect;
                copy.raycastTarget = true; // agarrar cualquier hoja del montón arrastra la pieza entera
            }
        }
    }
}
