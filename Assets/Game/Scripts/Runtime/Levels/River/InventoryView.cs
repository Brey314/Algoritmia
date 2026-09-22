using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.River
{
    /// <summary>
    /// Las casillas del inventario en pantalla (RF-38): una por clase de material. La de los
    /// troncos lleva cinco marcas que se encienden de a una: se ve cuántos van **sin una cifra**
    /// (CP-03, decisión del 20/09/2026).
    /// </summary>
    /// <remarks>
    /// Solo pinta: qué hay y cuántos lo dicen <see cref="Inventory"/> en la recolección y
    /// <see cref="RaftAssembly"/> en el ensamblaje, que son quienes la llaman. Compartida entre
    /// las dos porque las casillas son el mismo objeto todo el nivel: de ahí se recoge y de ahí
    /// se arrastra.
    /// </remarks>
    public class InventoryView : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Casilla modelo. Permanece inactiva.")]
        private Image slotTemplate;

        [SerializeField]
        [Tooltip("La marca de cada unidad en una casilla de varias (los troncos). Vacío deja la casilla sin marcas.")]
        private Sprite pipSprite;

        [SerializeField] private Color emptyColor = new Color(0.8784f, 0.8314f, 0.7529f);
        [SerializeField] private Color pipOnColor = new Color(0.20f, 0.40f, 0.22f);
        [SerializeField] private Color pipOffColor = new Color(0.72f, 0.66f, 0.58f);

        [SerializeField]
        [Tooltip("Lado de cada marca, en píxeles del lienzo de referencia.")]
        private float pipSize = 16f;

        private readonly List<(MaterialKind Kind, Image Slot, Image[] Pips)> _slots = new List<(MaterialKind, Image, Image[])>();

        /// <summary>Las casillas, en el orden del catálogo.</summary>
        public IReadOnlyList<Image> Slots => _slots.Select(entry => entry.Slot).ToArray();

        public Image SlotOf(MaterialKind kind) => _slots.FirstOrDefault(entry => entry.Kind == kind).Slot;

        /// <summary>Una casilla vacía por clase; con marcas si esa clase se recoge de a varias.</summary>
        public void Build(IEnumerable<(MaterialKind Kind, int Required)> kinds)
        {
            slotTemplate.gameObject.SetActive(false);
            foreach (var (kind, required) in kinds)
            {
                var slot = Instantiate(slotTemplate, transform);
                // El nombre hace único el camino de jerarquía: sin él ninguna prueba puede señalarla.
                slot.name = $"Casilla_{kind}";
                slot.gameObject.SetActive(true);
                var pips = required > 1 ? BuildPips(slot.rectTransform, required) : System.Array.Empty<Image>();
                _slots.Add((kind, slot, pips));
                Show(kind, null, 0);
            }
        }

        /// <summary>Pinta la casilla: el arte si queda algo, vacía si no, y tantas marcas encendidas como unidades.</summary>
        public void Show(MaterialKind kind, Sprite art, int count)
        {
            var entry = _slots.FirstOrDefault(candidate => candidate.Kind == kind);
            if (entry.Slot == null)
            {
                return;
            }

            entry.Slot.sprite = count > 0 ? art : null;
            entry.Slot.preserveAspect = true;
            entry.Slot.color = count > 0 && art != null ? Color.white : emptyColor;
            for (var index = 0; index < entry.Pips.Length; index++)
            {
                entry.Pips[index].color = index < count ? pipOnColor : pipOffColor;
            }
        }

        private Image[] BuildPips(RectTransform slot, int count)
        {
            var pips = new Image[count];
            for (var index = 0; index < count; index++)
            {
                var pip = new GameObject($"Marca_{index + 1}", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                pip.rectTransform.SetParent(slot, false);
                var x = (index + 0.5f) / count;
                pip.rectTransform.anchorMin = new Vector2(x, 0.1f);
                pip.rectTransform.anchorMax = new Vector2(x, 0.1f);
                pip.rectTransform.anchoredPosition = Vector2.zero;
                pip.rectTransform.sizeDelta = Vector2.one * pipSize;
                pip.sprite = pipSprite;
                pip.raycastTarget = false;
                pips[index] = pip;
            }

            return pips;
        }
    }
}
