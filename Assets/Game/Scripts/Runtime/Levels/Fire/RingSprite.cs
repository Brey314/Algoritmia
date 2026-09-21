using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Dibuja el contorno circular de la zona de reunión (T25) en memoria: no hay sprite de anillo
    /// en el arte y el diámetro depende de dónde estén los botones, así que se genera al tamaño
    /// exacto la primera vez que se pide.
    /// </summary>
    internal static class RingSprite
    {
        /// <param name="diameter">Diámetro en píxeles de la textura; el sprite mide lo mismo.</param>
        /// <param name="thickness">Grosor del trazo en píxeles.</param>
        public static Sprite Create(int diameter, float thickness)
        {
            diameter = Mathf.Max(diameter, 4);
            var texture = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false)
            {
                name = "AnilloReunion",
                filterMode = FilterMode.Bilinear
            };
            var pixels = new Color32[diameter * diameter];
            var center = (diameter - 1) / 2f;
            var outer = diameter / 2f;
            var inner = outer - thickness;

            for (var y = 0; y < diameter; y++)
            {
                for (var x = 0; x < diameter; x++)
                {
                    var distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    // Un píxel de suavizado a cada lado del trazo para que el borde no dé escalones.
                    var alpha = Mathf.Clamp01(outer - distance) * Mathf.Clamp01(distance - inner);
                    pixels[y * diameter + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0f, 0f, diameter, diameter), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
