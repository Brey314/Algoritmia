using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>
    /// Sombra de gota (drop shadow / contact shadow) para los props de la escena.
    /// Crea una elipse plana semitransparente (#000000 al 25 %) en el suelo debajo del objeto
    /// (Dirección de Arte §5.3).
    /// </summary>
    /// <remarks>
    /// Se excluyen explícitamente:
    /// - Sprites de silueta (*silueta*).
    /// - La balsa (*balsa*), tanto al navegar como durante el armado en el Nivel 3.
    /// - Elementos de la fogata (*fogata*, *fuego*, *humo*, *monton_hojas*, *hoguera*), ya que el fuego emite luz y no proyecta sombra en el suelo.
    /// - Personajes (ya poseen su sombra integrada en sus prefabs).
    ///
    /// Comportamiento dinámico:
    /// - Si el objeto se desplaza o escala por el suelo, la sombra acompaña posición y escala.
    /// - Si el objeto rueda/gira, la sombra no gira (se mantiene horizontal respecto al plano del suelo).
    /// - Si el objeto se levanta del suelo (como en el Nivel 2 al alzar troncos o piedras), la sombra
    ///   permanece en el suelo en la posición inicial y su tamaño y opacidad se reducen conforme
    ///   el objeto gana altura.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class PropShadow : MonoBehaviour
    {
        private const float DefaultAlpha = 0.25f;
        private const float MinLiftAlpha = 0.08f;
        private const float MinLiftScale = 0.35f;

        private static Sprite s_circleSprite;

        [SerializeField] private RectTransform shadowRect;
        [SerializeField] private Image shadowImage;
        [SerializeField] private float baseLocalX;
        [SerializeField] private float baseLocalY;
        [SerializeField] private Vector2 baseSize;
        [SerializeField] private float baseRotationDegrees;
        [SerializeField] private float baseShadowAngle;
        [SerializeField] private float currentHeight;
        [SerializeField] private float currentMaxLift = 100f;

        private Canvas cachedCanvas;

        public RectTransform ShadowRect => shadowRect;
        public Image ShadowImage => shadowImage;
        public float BaseLocalX => baseLocalX;
        public float BaseLocalY => baseLocalY;
        public Vector2 BaseSize => baseSize;
        public float BaseShadowAngle => baseShadowAngle;
        public float CurrentHeight => currentHeight;
        public float CurrentMaxLift => currentMaxLift;

        /// <summary>
        /// Comprueba si un sprite está excluido de proyectar sombra de gota
        /// (siluetas, piezas de la balsa y elementos de la fogata/fuego).
        /// </summary>
        public static bool IsExcluded(Sprite sprite)
        {
            if (sprite == null)
            {
                return true;
            }

            var name = sprite.name;
            if (name.IndexOf("silueta", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (name.IndexOf("balsa", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (name.IndexOf("fogata", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (name.IndexOf("fuego", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (name.IndexOf("humo", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (name.IndexOf("monton_hojas", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (name.IndexOf("hoguera", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adjunta y configura la sombra de gota a un GameObject de prop si no está excluido.
        /// </summary>
        public static PropShadow Attach(GameObject target, Sprite art, Vector2 sizeDelta)
        {
            if (target == null || IsExcluded(art))
            {
                return null;
            }

            var shadow = target.GetComponent<PropShadow>();
            if (shadow == null)
            {
                shadow = target.AddComponent<PropShadow>();
            }

            shadow.Initialize(art, sizeDelta);
            return shadow;
        }

        public void Initialize(Sprite art, Vector2 sizeDelta)
        {
            var targetRect = (RectTransform)transform;
            if (shadowRect == null)
            {
                var existing = transform.Find("Sombra");
                GameObject shadowGo;
                if (existing != null)
                {
                    shadowGo = existing.gameObject;
                }
                else
                {
                    shadowGo = new GameObject("Sombra", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    shadowGo.transform.SetParent(transform, false);
                }

                shadowGo.transform.SetAsFirstSibling(); // Se dibuja detrás del sprite del prop
                shadowRect = (RectTransform)shadowGo.transform;
                shadowImage = shadowGo.GetComponent<Image>();
            }

            shadowImage.sprite = GetOrCreateCircleSprite();
            shadowImage.color = new Color(0f, 0f, 0f, DefaultAlpha);
            shadowImage.raycastTarget = false;
            shadowImage.preserveAspect = false;

            // Dimensiones proporcionales a la ilustración real dentro del rect (preserveAspect)
            var artW = (art != null && art.rect.width > 0f) ? art.rect.width : sizeDelta.x;
            var artH = (art != null && art.rect.height > 0f) ? art.rect.height : sizeDelta.y;
            var spriteAspect = (artH > 0f) ? artW / artH : 1f;
            var rectAspect = (sizeDelta.y > 0f) ? sizeDelta.x / sizeDelta.y : 1f;

            float renderedWidth, renderedHeight;
            if (rectAspect > spriteAspect)
            {
                renderedHeight = sizeDelta.y;
                renderedWidth = sizeDelta.y * spriteAspect;
            }
            else
            {
                renderedWidth = sizeDelta.x;
                renderedHeight = spriteAspect > 0f ? sizeDelta.x / spriteAspect : sizeDelta.y;
            }

            // Calculamos el contorno opaco real del objeto a partir de la malla de vértices del sprite
            // (Unity genera la malla ajustada al contorno opaco con spriteMeshType: Tight).
            // Esto evita que la sombra se dibuje muy abajo cuando el PNG tiene márgenes transparentes.
            var verts = art != null ? art.vertices : null;
            var ppu = (art != null && art.pixelsPerUnit > 0f) ? art.pixelsPerUnit : 100f;
            var halfW = artW * 0.5f;
            var halfH = artH * 0.5f;

            float fracMinY = -0.84f;
            float fracMinX = -0.85f;
            float fracMaxX = 0.85f;

            if (verts != null && verts.Length > 0)
            {
                var minY = float.MaxValue;
                var minX = float.MaxValue;
                var maxX = float.MinValue;

                for (var i = 0; i < verts.Length; i++)
                {
                    var v = verts[i];
                    if (v.y < minY) minY = v.y;
                    if (v.x < minX) minX = v.x;
                    if (v.x > maxX) maxX = v.x;
                }

                if (halfH > 0.001f)
                {
                    fracMinY = Mathf.Clamp(minY * ppu / halfH, -1f, 1f);
                }

                if (halfW > 0.001f)
                {
                    fracMinX = Mathf.Clamp(minX * ppu / halfW, -1f, 1f);
                    fracMaxX = Mathf.Clamp(maxX * ppu / halfW, -1f, 1f);
                }
            }

            var spriteName = art != null ? art.name : string.Empty;
            var isRollingLog = (transform.GetComponentInChildren<RollingLog>() != null) ||
                               spriteName.IndexOf("tronco_a", StringComparison.OrdinalIgnoreCase) >= 0;
            var isRiverLog = spriteName.IndexOf("prop_n3_tronco", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             (spriteName.IndexOf("tronco", StringComparison.OrdinalIgnoreCase) >= 0 && !isRollingLog);
            var isTela = spriteName.IndexOf("tela", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isRollingLog)
            {
                // Tronco cilíndrico en perspectiva (Nivel 2): eje a 35° extendiéndose hacia el fondo
                baseShadowAngle = 35f;
                baseLocalX = 0.49f * renderedWidth;
                baseLocalY = -0.22f * renderedHeight;
                var shadowWidth = Mathf.Max(renderedWidth * 1.15f, 10f);
                var shadowHeight = Mathf.Max(shadowWidth * 0.38f, 6f);
                baseSize = new Vector2(shadowWidth, shadowHeight);
            }
            else if (isRiverLog)
            {
                // Tronco alargado diagonal en perspectiva (Nivel 3): orientado a 35°
                baseShadowAngle = 35f;
                baseLocalX = 0f;
                baseLocalY = -0.22f * renderedHeight;
                var shadowWidth = Mathf.Max(renderedWidth * 1.15f, 10f);
                var shadowHeight = Mathf.Max(shadowWidth * 0.38f, 6f);
                baseSize = new Vector2(shadowWidth, shadowHeight);
            }
            else if (isTela)
            {
                // Tela (Nivel 3): la base de los pliegues se sitúa más arriba que la punta inferior aislada
                baseShadowAngle = 0f;
                baseLocalX = 0f;
                var shadowWidth = Mathf.Max(renderedWidth * 0.95f, 10f);
                var shadowHeight = Mathf.Max(shadowWidth * 0.38f, 6f);
                baseLocalY = -0.15f * renderedHeight;
                baseSize = new Vector2(shadowWidth, shadowHeight);
            }
            else
            {
                baseShadowAngle = 0f;
                var objectWidth = Mathf.Max((fracMaxX - fracMinX) * 0.5f * renderedWidth, 10f);
                var shadowWidth = Mathf.Max(objectWidth * 0.95f, 10f);
                var shadowHeight = Mathf.Max(shadowWidth * 0.38f, 6f);

                baseLocalX = ((fracMinX + fracMaxX) * 0.5f) * (renderedWidth * 0.5f);
                // Elevamos el centro de la sombra para que abrace la base del objeto y no quede desprendida
                baseLocalY = fracMinY * (renderedHeight * 0.5f) + shadowHeight * 0.35f;
                baseSize = new Vector2(shadowWidth, shadowHeight);
            }

            baseRotationDegrees = targetRect.localEulerAngles.z;

            currentHeight = 0f;
            currentMaxLift = 100f;

            shadowRect.anchorMin = new Vector2(0.5f, 0.5f);
            shadowRect.anchorMax = new Vector2(0.5f, 0.5f);
            shadowRect.pivot = new Vector2(0.5f, 0.5f);
            shadowRect.sizeDelta = baseSize;
            shadowRect.localScale = Vector3.one;

            ApplyTransform();
        }

        private void LateUpdate()
        {
            ApplyTransform();
        }

        /// <summary>
        /// Aplica la posición y orientación de la sombra de gota fija en el plano del suelo
        /// respecto al Canvas o espacio mundial, evitando que gire u orbite si el prop rota o rueda.
        /// </summary>
        public void ApplyTransform()
        {
            if (shadowRect == null)
            {
                return;
            }

            var canvas = cachedCanvas != null ? cachedCanvas : (cachedCanvas = GetComponentInParent<Canvas>());
            var canvasUp = canvas != null ? canvas.transform.up : Vector3.up;
            var canvasRight = canvas != null ? canvas.transform.right : Vector3.right;
            var canvasRot = canvas != null ? canvas.transform.rotation : Quaternion.identity;

            var targetRect = (RectTransform)transform;
            var scaleX = targetRect.lossyScale.x;
            var scaleY = Mathf.Abs(targetRect.lossyScale.y);

            // 1. Orientación de la sombra: los troncos siempre apuntan a 35° hacia el fondo (RollingLog
            // cancela la inversión de espejo del padre, así que la perspectiva cilíndrica siempre va hacia la derecha)
            var isLog = baseShadowAngle > 0.001f;
            var shadowAngle = isLog ? baseShadowAngle : baseShadowAngle * Mathf.Sign(scaleX);
            shadowRect.rotation = canvasRot * Quaternion.Euler(0f, 0f, shadowAngle);

            // 2. La sombra se fija al suelo directamente debajo del objeto (no orbita con el giro)
            // Para los troncos, la posición X de contacto siempre es hacia el eje del cilindro (+X en espacio del objeto)
            var effectiveLocalX = isLog ? baseLocalX * Mathf.Abs(scaleX) : baseLocalX * scaleX;
            shadowRect.position = targetRect.position
                + canvasRight * effectiveLocalX
                + canvasUp * ((baseLocalY - currentHeight) * scaleY);
        }

        /// <summary>
        /// Actualiza la sombra durante el movimiento o elevación del objeto.
        /// </summary>
        /// <param name="height">Altura actual del objeto levantado del suelo en píxeles.</param>
        /// <param name="maxLift">Altura máxima de despegue alcanzable.</param>
        /// <param name="currentPropRotation">Rotación Z actual del objeto en grados (conservado por compatibilidad).</param>
        public void UpdateMotion(float height, float maxLift, float currentPropRotation = 0f)
        {
            if (shadowRect == null || shadowImage == null)
            {
                return;
            }

            currentHeight = Mathf.Max(0f, height);
            currentMaxLift = maxLift;

            // El tamaño de la sombra y su opacidad disminuyen conforme el objeto se levanta
            var liftRatio = (maxLift > 0.001f && currentHeight > 0f) ? Mathf.Clamp01(currentHeight / maxLift) : 0f;
            var scale = Mathf.Lerp(1.0f, MinLiftScale, liftRatio);
            shadowRect.localScale = new Vector3(scale, scale, 1f);

            var alpha = Mathf.Lerp(DefaultAlpha, MinLiftAlpha, liftRatio);
            shadowImage.color = new Color(0f, 0f, 0f, alpha);

            ApplyTransform();
        }

        /// <summary>
        /// Obtiene o genera proceduralmente un sprite circular nítido y suavizado (#ui_circulo).
        /// </summary>
        public static Sprite GetOrCreateCircleSprite()
        {
            if (s_circleSprite != null)
            {
                return s_circleSprite;
            }

#if UNITY_EDITOR
            s_circleSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Game/Art/UI/Common/ui_circulo.png");
            if (s_circleSprite != null)
            {
                return s_circleSprite;
            }
#endif

            const int diameter = 128;
            var texture = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false)
            {
                name = "SombraGota_Circulo",
                filterMode = FilterMode.Bilinear
            };
            var pixels = new Color32[diameter * diameter];
            var center = (diameter - 1) / 2f;
            var radius = diameter / 2f - 1f;

            for (var y = 0; y < diameter; y++)
            {
                for (var x = 0; x < diameter; x++)
                {
                    var distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    var alpha = Mathf.Clamp01(radius - distance);
                    pixels[y * diameter + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            s_circleSprite = Sprite.Create(texture, new Rect(0f, 0f, diameter, diameter), new Vector2(0.5f, 0.5f), 100f);
            return s_circleSprite;
        }
    }
}

