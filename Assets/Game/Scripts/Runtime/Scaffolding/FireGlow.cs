using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>
    /// El halo de luz de una fogata encendida: un círculo naranja tenue detrás de la llama que
    /// crece y mengua despacio (Dirección de Arte §8.1, «La hoguera»; decisión de Santiago,
    /// 08/10/2026).
    /// </summary>
    /// <remarks>
    /// «Por qué no» un círculo plano: la DA decía «círculo plano, sin degradado», pero a 20 % un
    /// disco de borde duro no se lee como luz sino como una mancha naranja —de noche tapa media
    /// cueva y a plena luz deja aros fantasma donde se cruzan dos fogatas (capturas de la ronda de
    /// ajustes del 08/10/2026)—. El halo conserva el color, el 20 % y la respiración de la DA, y su
    /// opacidad se desvanece hacia el borde (<see cref="GetOrCreateGlowSprite"/>).
    ///
    /// Va en el objeto de la llama y el halo es su **hermano**, no su hijo: en uGUI un hijo se dibuja
    /// encima del gráfico de su padre y taparía el fuego. Por defecto es el hermano inmediatamente
    /// anterior: lo que está delante de la fogata —la familia— se dibuja encima del halo, y lo que
    /// está detrás —el humo, el suelo, las piedras— queda bañado por su luz. Cada cuadro copia el
    /// centro y el tamaño de la llama, así que la acompaña en el paneo, el zoom y cualquier
    /// movimiento.
    ///
    /// «Por qué no» un parpadeo: el halo respira con una sola onda lenta (un ciclo cada
    /// <see cref="CycleSeconds"/>, 0,5 Hz) y solo cambia su escala; su opacidad no cambia nunca.
    /// Nada destella (RNF-21). El tiempo es el escalado del juego: con el menú de pausa abierto
    /// el halo se queda quieto, igual que la llama.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class FireGlow : MonoBehaviour
    {
        /// <summary>Nombre del halo: este prefijo y el nombre de la llama. Quien cuente hijos de un contenedor lo salta.</summary>
        public const string NamePrefix = "Halo_";

        /// <summary>Escala mínima de la respiración (DA §8.1).</summary>
        public const float MinScale = 0.95f;

        /// <summary>Escala máxima de la respiración (DA §8.1).</summary>
        public const float MaxScale = 1.05f;

        /// <summary>Lo que dura una respiración entera, en segundos: lenta y suave (Santiago, 08/10/2026; la DA decía 1,2 s).</summary>
        public const float CycleSeconds = 2f;

        /// <summary><c>#F0A84E</c> al 20 % (DA §8.1): el naranja de la luz de fuego, casi transparente. El 20 % es la opacidad del centro.</summary>
        public static readonly Color GlowColor = new Color32(0xF0, 0xA8, 0x4E, 0x33);

        /// <summary>Hasta qué fracción del radio el halo conserva toda su opacidad; de ahí al borde se desvanece.</summary>
        private const float CoreRadius = 0.3f;

        private static Sprite s_glowSprite;

        [field: SerializeField]
        [field: Tooltip("Diámetro del halo en lados de la llama: 1,8 = un círculo 1,8 veces el lado menor del cuadro de la llama.")]
        public float Spread { get; set; } = 1.8f;

        [field: SerializeField]
        [field: Tooltip("Si el halo va al fondo de su contenedor —el primer hermano— en vez de justo antes de la llama: la luz baña el suelo y no vela lo que hay encima. Para la llama cenital del Nivel 1, donde sobre el montón están las hojas y las piedras que acaban de encenderse; en las narrativas, donde lo que se ve cerca de la fogata se baña de su luz, se deja sin marcar.")]
        public bool BehindSiblings { get; set; }

        [field: SerializeField]
        [field: Tooltip("Dónde cae el centro de la luz respecto al centro del cuadro de la llama, en lados de la llama. La llama dibujada ocupa la mitad de abajo de su cuadro (su cuerpo queda a ~0,1 por debajo del centro), así que la luz baja con ella.")]
        public Vector2 Offset { get; set; } = new Vector2(0f, -0.1f);

        private RectTransform _halo;
        private Image _haloImage;

        /// <summary>El halo: hermano de la llama —justo antes que ella, o al fondo del contenedor si <see cref="BehindSiblings"/>—. <c>null</c> hasta que la llama se enciende.</summary>
        public RectTransform Halo => _halo;

        /// <summary>La escala del halo a los <paramref name="seconds"/> de juego: una sola onda lenta entre <see cref="MinScale"/> y <see cref="MaxScale"/>.</summary>
        public static float ScaleAt(float seconds) =>
            Mathf.Lerp(MinScale, MaxScale, 0.5f + 0.5f * Mathf.Sin(2f * Mathf.PI * seconds / CycleSeconds));

        /// <summary>Le pone el halo a la llama <paramref name="flame"/> (un objeto de uGUI con padre) y lo devuelve.</summary>
        public static FireGlow Attach(GameObject flame)
        {
            var glow = flame.GetComponent<FireGlow>();
            if (glow == null)
            {
                glow = flame.AddComponent<FireGlow>();
            }

            glow.EnsureHalo();
            return glow;
        }

        /// <summary>
        /// La opacidad relativa del disco (de 0 a 1) a una fracción del radio: 1 hasta
        /// <see cref="CoreRadius"/> y de ahí baja suave hasta 0 en el borde (curva de Hermite), sin
        /// escalón: es lo que hace que el halo se lea como luz y no como un disco.
        /// </summary>
        public static float OpacityAt(float radius)
        {
            var t = Mathf.Clamp01((radius - CoreRadius) / (1f - CoreRadius));
            return 1f - t * t * (3f - 2f * t);
        }

        /// <summary>
        /// Un disco blanco con la opacidad de <see cref="OpacityAt"/>: tintado con
        /// <see cref="GlowColor"/> es el halo. Se genera en memoria —ningún archivo nuevo, como el
        /// disco de <see cref="BurnReveal"/>—.
        /// </summary>
        public static Sprite GetOrCreateGlowSprite()
        {
            if (s_glowSprite != null)
            {
                return s_glowSprite;
            }

            const int diameter = 128;
            var texture = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false)
            {
                name = "HaloFogata_Degradado",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color32[diameter * diameter];
            var center = (diameter - 1) / 2f;
            for (var y = 0; y < diameter; y++)
            {
                for (var x = 0; x < diameter; x++)
                {
                    var radius = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / (diameter / 2f);
                    pixels[y * diameter + x] = new Color32(255, 255, 255, (byte)(OpacityAt(radius) * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            s_glowSprite = Sprite.Create(texture, new Rect(0f, 0f, diameter, diameter), new Vector2(0.5f, 0.5f), 100f);
            return s_glowSprite;
        }

        private void OnEnable() => EnsureHalo();

        private void LateUpdate() => Follow(Time.time);

        private void OnDisable()
        {
            // Apagar el gráfico y no el objeto: activar o desactivar otro objeto mientras Unity
            // recorre una jerarquía que se apaga es un error (como hace PropShadow).
            if (_haloImage != null)
            {
                _haloImage.enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (_halo == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(_halo.gameObject);
            }
            else
            {
                DestroyImmediate(_halo.gameObject);
            }
        }

        /// <summary>Crea el halo si no existe, lo deja justo antes de la llama y lo enciende. Se puede repetir.</summary>
        internal void EnsureHalo()
        {
            if (!(transform.parent is RectTransform parent))
            {
                return;
            }

            if (_halo == null)
            {
                var go = new GameObject(NamePrefix + name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                go.layer = gameObject.layer;
                _halo = (RectTransform)go.transform;
                _halo.SetParent(parent, false);
                _halo.anchorMin = _halo.anchorMax = _halo.pivot = new Vector2(0.5f, 0.5f);
                _haloImage = go.GetComponent<Image>();
                _haloImage.sprite = GetOrCreateGlowSprite();
                _haloImage.color = GlowColor;
                _haloImage.raycastTarget = false;
            }

            // Justo antes de la llama: debajo de ella y encima de lo que ya estaba. Si el halo va
            // por delante de la llama, al sacarlo de su sitio ella baja un puesto. Con
            // BehindSiblings, al fondo del contenedor.
            var flame = transform.GetSiblingIndex();
            var before = _halo.GetSiblingIndex() < flame ? flame - 1 : flame;
            _halo.SetSiblingIndex(BehindSiblings ? 0 : before);
            _haloImage.enabled = isActiveAndEnabled; // adjuntar a una llama apagada no enciende su halo
            Follow(Time.time);
        }

        /// <summary>Lleva el halo al centro de la llama, con su tamaño, y le da la escala de <paramref name="seconds"/>.</summary>
        internal void Follow(float seconds)
        {
            if (_halo == null || !(transform.parent is RectTransform parent))
            {
                return;
            }

            var flame = (RectTransform)transform;
            var scale = flame.localScale;
            var side = Mathf.Min(flame.rect.width * Mathf.Abs(scale.x), flame.rect.height * Mathf.Abs(scale.y));

            // El centro del cuadro de la llama en el espacio del padre, valga el pivote, el giro o el
            // espejo que tenga. El halo está anclado al centro del padre.
            var center = (Vector2)parent.InverseTransformPoint(flame.TransformPoint(flame.rect.center));
            var position = center - parent.rect.center + Offset * side;
            var size = Vector2.one * (side * Spread);

            // Solo lo que cambia: reasignar lo mismo cada cuadro haría rehacer el lienzo entero.
            if (_halo.anchoredPosition != position)
            {
                _halo.anchoredPosition = position;
            }

            if (_halo.sizeDelta != size)
            {
                _halo.sizeDelta = size;
            }

            _halo.localScale = Vector3.one * ScaleAt(seconds);
        }
    }
}
