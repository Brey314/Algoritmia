using UnityEngine;

namespace Game.UI.Tests
{
    /// <summary>
    /// Mide, sobre los rectángulos que de verdad se dibujan, qué parte de una ilustración encuadrada
    /// (<see cref="FramedIllustration"/>) se ve dentro de su ventana. Lo comparten las pruebas del menú
    /// de inicio y del menú de niveles.
    /// </summary>
    internal static class IllustrationProbe
    {
        /// <summary>
        /// El tramo horizontal de la ilustración que cae dentro de la ventana, en fracciones del ancho de la
        /// ilustración: 0 es su borde izquierdo, 1 el derecho y 0,5 el eje del espejo de los lienzos duplicados.
        /// </summary>
        public static (float Left, float Right) VisibleRange(FramedIllustration window)
        {
            var art = WorldRect(window.Art.rectTransform);
            var view = WorldRect((RectTransform)window.transform);
            return ((view.xMin - art.xMin) / art.width, (view.xMax - art.xMin) / art.width);
        }

        /// <summary>El rectángulo de un RectTransform en el espacio de mundo; en un Canvas Overlay, píxeles de pantalla.</summary>
        public static Rect WorldRect(RectTransform rect)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            return Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
        }

        /// <summary>El rectángulo mínimo que contiene a los dos.</summary>
        public static Rect Union(Rect a, Rect b) => Rect.MinMaxRect(
            Mathf.Min(a.xMin, b.xMin), Mathf.Min(a.yMin, b.yMin),
            Mathf.Max(a.xMax, b.xMax), Mathf.Max(a.yMax, b.yMax));
    }
}
