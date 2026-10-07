using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>Cómo cae la sombra de un objeto, según cómo lo dibuja su ilustración.</summary>
    public enum PropShadowKind
    {
        /// <summary>
        /// Vista en picado —troncos, tablas, cajas, la carretilla, la balsa—: el objeto está tendido
        /// en el suelo y su sombra es su propia silueta, corrida hacia abajo y a la derecha porque la
        /// luz llega de arriba a la izquierda (Dirección de Arte §5.2).
        /// </summary>
        Silhouette,

        /// <summary>De pie —las plantas—: la elipse plana en la base del §5.3.</summary>
        Contact,
    }

    /// <summary>
    /// Sombra de gota de los objetos de la escena: color plano <c>#000000</c> al 25 %
    /// (Dirección de Arte §5.3), sin pintar en el sprite.
    /// </summary>
    /// <remarks>
    /// **La sombra no es hija del objeto.** En uGUI un hijo se dibuja siempre después —encima— del
    /// gráfico de su padre, así que una sombra hija tapa el dibujo por mucho que sea el primer
    /// hijo. Las sombras viven en una capa propia (<see cref="LayerName"/>), hermana de los objetos
    /// y anterior a ellos: se pintan sobre el suelo y debajo de todo lo que está de pie en él. Este
    /// componente, que sí va en el objeto, copia cada cuadro su posición, giro y escala en la
    /// sombra, y la apaga o la destruye con él.
    ///
    /// **La forma depende de la vista** (<see cref="PropShadowKind"/>): lo que se ve en picado
    /// proyecta su silueta —la misma imagen en negro—, que es la única forma que concuerda con un
    /// tronco en diagonal o con una tabla; una elipse solo le sirve a lo que está de pie. El tronco
    /// que rueda (<see cref="RollingLog"/>) no tiene sprite que copiar: su sombra es la silueta del
    /// cilindro, que gira y se refleja con él.
    ///
    /// Se excluyen:
    /// - Las siluetas del ensamblaje (*silueta*): son el hueco donde va la pieza, no un objeto.
    /// - La balsa hundida (*balsa_hundida*): trae el agua pintada y su silueta sombrearía el río.
    /// - El fuego y lo que arde (*fogata*, *fuego*, *humo*, *monton_hojas*, *hoguera*): el fuego
    ///   emite luz y no proyecta sombra en el suelo.
    /// - Los personajes, que traen su sombra en el prefab.
    ///
    /// Si el objeto se levanta del suelo (<see cref="UpdateMotion"/>), la sombra se queda donde
    /// estaba en el suelo y se encoge y aclara conforme gana altura.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class PropShadow : MonoBehaviour
    {
        /// <summary>Nombre de la capa de sombras dentro del padre de los objetos.</summary>
        public const string LayerName = "Sombras";

        private const float DefaultAlpha = 0.25f;
        private const float MinLiftAlpha = 0.08f;
        private const float MinLiftScale = 0.35f;

        /// <summary>
        /// Cuánto se corre la silueta, en fracción del lado menor del dibujo: hacia la derecha y
        /// hacia abajo, opuesta a la luz de arriba a la izquierda (DA §5.2).
        /// </summary>
        internal static readonly Vector2 SilhouetteOffset = new Vector2(0.035f, -0.05f);

        private static Sprite s_circleSprite;

        [SerializeField] private RectTransform shadowRect;
        [SerializeField] private Graphic shadowGraphic;
        [SerializeField] private PropShadowKind kind;
        [SerializeField] private Vector2 contactCenter;
        [SerializeField] private float contactWidth;
        [SerializeField] private float currentHeight;
        [SerializeField] private float liftRatio;

        private Image _art;
        private Graphic _source;
        private Canvas _canvas;

        /// <summary>La sombra: vive en la capa <see cref="LayerName"/>, no bajo el objeto.</summary>
        public RectTransform ShadowRect => shadowRect;

        /// <summary>Lo que pinta la sombra: una <see cref="Image"/> o, en el tronco que rueda, un <see cref="RollingLog"/>.</summary>
        public Graphic ShadowGraphic => shadowGraphic;

        /// <summary>La capa de sombras en la que vive la de este objeto.</summary>
        public RectTransform Layer => shadowRect != null ? shadowRect.parent as RectTransform : null;

        public PropShadowKind Kind => kind;

        /// <summary>Altura a la que está levantado el objeto, en unidades de su padre.</summary>
        public float CurrentHeight => currentHeight;

        /// <summary>
        /// Comprueba si un sprite está excluido de proyectar sombra de gota
        /// (siluetas, balsa hundida y elementos de la fogata o del fuego).
        /// </summary>
        public static bool IsExcluded(Sprite sprite)
        {
            if (sprite == null)
            {
                return true;
            }

            var name = sprite.name;
            return Contains(name, "silueta")
                   || Contains(name, "balsa_hundida")
                   || Contains(name, "fogata")
                   || Contains(name, "fuego")
                   || Contains(name, "humo")
                   || Contains(name, "monton_hojas")
                   || Contains(name, "hoguera");
        }

        /// <summary>
        /// Qué sombra le toca a un sprite: elipse a lo que está de pie, silueta a lo que se ve en
        /// picado. Hoy de pie solo están las plantas; todo lo demás está tendido en el suelo.
        /// </summary>
        public static PropShadowKind KindFor(Sprite sprite) =>
            sprite != null && Contains(sprite.name, "planta") ? PropShadowKind.Contact : PropShadowKind.Silhouette;

        /// <summary>¿Es <paramref name="candidate"/> una capa de sombras? Para quien recorre los hijos de un contenedor.</summary>
        public static bool IsLayer(Transform candidate) => candidate != null && candidate.name == LayerName;

        /// <summary>
        /// Adjunta y configura la sombra de gota a un objeto si no está excluido. El objeto tiene
        /// que tener padre: la sombra vive en una capa hermana suya.
        /// </summary>
        public static PropShadow Attach(GameObject target, Sprite art)
        {
            if (target == null || IsExcluded(art) || !(target.transform.parent is RectTransform))
            {
                return null;
            }

            var shadow = target.GetComponent<PropShadow>();
            if (shadow == null)
            {
                shadow = target.AddComponent<PropShadow>();
            }

            shadow.Initialize(art);
            return shadow;
        }

        /// <summary>
        /// La capa de sombras de <paramref name="parent"/>; si no tiene, la crea en
        /// <paramref name="index"/>, delante del primer objeto que la necesita. La rejilla de la
        /// fila de troncos no la cuenta (<see cref="LayoutElement.ignoreLayout"/>).
        /// </summary>
        public static RectTransform LayerFor(RectTransform parent, int index)
        {
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (IsLayer(child))
                {
                    return (RectTransform)child;
                }
            }

            var go = new GameObject(LayerName, typeof(RectTransform), typeof(LayoutElement));
            go.layer = parent.gameObject.layer;
            go.GetComponent<LayoutElement>().ignoreLayout = true;
            var layer = (RectTransform)go.transform;
            layer.SetParent(parent, false);
            layer.anchorMin = Vector2.zero;
            layer.anchorMax = Vector2.one;
            layer.pivot = new Vector2(0.5f, 0.5f);
            layer.offsetMin = layer.offsetMax = Vector2.zero;
            layer.SetSiblingIndex(Mathf.Clamp(index, 0, parent.childCount - 1));
            return layer;
        }

        private void Initialize(Sprite art)
        {
            var target = (RectTransform)transform;
            _art = GetComponent<Image>();
            kind = KindFor(art);

            if (shadowRect != null)
            {
                DestroyObject(shadowRect.gameObject);
            }

            var layer = LayerFor((RectTransform)target.parent, target.GetSiblingIndex());
            var go = new GameObject($"Sombra_{name}", typeof(RectTransform));
            go.layer = gameObject.layer;
            shadowRect = (RectTransform)go.transform;
            shadowRect.SetParent(layer, false);
            shadowRect.anchorMin = shadowRect.anchorMax = new Vector2(0.5f, 0.5f);

            var log = GetComponentInChildren<RollingLog>(true);
            if (kind == PropShadowKind.Silhouette && log != null && log.Look != null && log.Face != null)
            {
                // El tronco que rueda se dibuja en el motor: su sombra es el contorno del cilindro.
                shadowGraphic = RollingLog.AttachSilhouette(shadowRect, log.Face, log.Look, ShadowColor(DefaultAlpha));
                _source = log;
            }
            else
            {
                go.AddComponent<CanvasRenderer>();
                var image = go.AddComponent<Image>();
                image.raycastTarget = false;
                if (kind == PropShadowKind.Silhouette)
                {
                    image.sprite = art;
                    image.preserveAspect = _art == null || _art.preserveAspect;
                    image.useSpriteMesh = _art != null && _art.useSpriteMesh;
                }
                else
                {
                    image.sprite = GetOrCreateCircleSprite();
                    image.preserveAspect = false;
                    MeasureContact(art);
                }

                image.color = ShadowColor(DefaultAlpha);
                shadowGraphic = image;
                _source = log != null ? log : _art;
            }

            currentHeight = 0f;
            liftRatio = 0f;
            ApplyTransform();
        }

        private void OnEnable()
        {
            ApplyTransform();
        }

        private void LateUpdate()
        {
            ApplyTransform();
        }

        private void OnDisable()
        {
            // Apagar el gráfico y no el objeto: activar o desactivar otro objeto mientras Unity
            // recorre una jerarquía que se apaga es un error, y la capa suele apagarse a la vez.
            if (shadowGraphic != null)
            {
                shadowGraphic.enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (shadowRect != null)
            {
                DestroyObject(shadowRect.gameObject);
            }
        }

        /// <summary>
        /// Lleva la sombra a donde está el objeto: la silueta copia su giro, su espejo y su escala;
        /// la elipse solo su sitio y su tamaño, porque está en el plano del suelo y no gira. Las dos
        /// se quedan en el suelo si el objeto está levantado.
        /// </summary>
        public void ApplyTransform()
        {
            if (shadowRect == null || shadowGraphic == null || !(transform.parent is RectTransform parent))
            {
                return;
            }

            var target = (RectTransform)transform;
            KeepBehind(target, parent);

            var source = _source != null ? _source : _art;
            var visible = enabled && gameObject.activeInHierarchy &&
                          (source == null || (source.enabled && source.gameObject.activeInHierarchy));
            shadowGraphic.enabled = visible;
            if (!visible)
            {
                return;
            }

            var sourceAlpha = source != null ? source.color.a : 1f;
            shadowGraphic.color = ShadowColor(Mathf.Lerp(DefaultAlpha, MinLiftAlpha, liftRatio) * sourceAlpha);
            if (kind == PropShadowKind.Silhouette && shadowGraphic is Image copy && _art != null && _art.sprite != null &&
                copy.sprite != _art.sprite)
            {
                copy.sprite = _art.sprite; // el objeto cambió de dibujo: la silueta también
            }

            var canvas = _canvas != null ? _canvas : (_canvas = GetComponentInParent<Canvas>());
            var screen = canvas != null ? canvas.rootCanvas.transform : null;
            var right = screen != null ? screen.right : Vector3.right;
            var up = screen != null ? screen.up : Vector3.up;

            // Levantado, el objeto sube y su sombra no: se baja lo que subió, en unidades del padre.
            var groundDrop = currentHeight * Mathf.Abs(parent.lossyScale.y);
            var liftScale = Mathf.Lerp(1f, MinLiftScale, liftRatio);
            var drawn = Drawn(target);
            var lossy = target.lossyScale;

            Vector2 pivot, size;
            Quaternion rotation;
            Vector3 scale, position;
            if (kind == PropShadowKind.Silhouette)
            {
                // La capa está en el mismo padre que el objeto y sin transformación propia: el giro
                // y la escala locales del objeto valen tal cual para su sombra.
                pivot = target.pivot;
                size = target.rect.size;
                rotation = target.localRotation;
                scale = target.localScale * liftScale;

                var side = Mathf.Min(drawn.x, drawn.y) * Mathf.Abs(lossy.y);
                position = target.position
                           + right * (SilhouetteOffset.x * side)
                           + up * (SilhouetteOffset.y * side - groundDrop);
            }
            else
            {
                var width = Mathf.Max(contactWidth * drawn.x, 10f);
                var height = Mathf.Max(width * 0.38f, 6f);
                pivot = new Vector2(0.5f, 0.5f);
                size = new Vector2(width, height);
                rotation = Quaternion.identity; // en el plano del suelo: no gira con la planta
                scale = new Vector3(Mathf.Abs(target.localScale.x), Mathf.Abs(target.localScale.y), 1f) * liftScale;

                // El centro de la elipse sube un poco sobre la base del dibujo para abrazarla y no
                // quedar desprendida; se mide desde el centro de la casilla, no desde su pivote.
                var local = new Vector2(contactCenter.x * drawn.x * 0.5f, contactCenter.y * drawn.y * 0.5f + height * 0.35f)
                            + Vector2.Scale(new Vector2(0.5f, 0.5f) - target.pivot, target.rect.size);
                position = target.position
                           + right * (local.x * lossy.x)
                           + up * (local.y * Mathf.Abs(lossy.y) - groundDrop);
            }

            // Solo lo que cambia: reasignar lo mismo cada cuadro haría rehacer el lienzo entero.
            if (shadowRect.pivot != pivot)
            {
                shadowRect.pivot = pivot;
            }

            if (shadowRect.sizeDelta != size)
            {
                shadowRect.sizeDelta = size;
            }

            if (shadowRect.localRotation != rotation)
            {
                shadowRect.localRotation = rotation;
            }

            if (shadowRect.localScale != scale)
            {
                shadowRect.localScale = scale;
            }

            if (shadowRect.position != position)
            {
                shadowRect.position = position;
            }
        }

        /// <summary>
        /// Actualiza la sombra mientras el objeto se levanta del suelo.
        /// </summary>
        /// <param name="height">Altura del objeto sobre el suelo, en unidades de su padre.</param>
        /// <param name="maxLift">Altura a la que la sombra llega a su tamaño y opacidad mínimos.</param>
        public void UpdateMotion(float height, float maxLift)
        {
            currentHeight = Mathf.Max(0f, height);
            liftRatio = maxLift > 0.001f ? Mathf.Clamp01(currentHeight / maxLift) : 0f;
            ApplyTransform();
        }

        /// <summary>
        /// La capa va delante —antes— de todo objeto que sombrea. Quien reordena a los objetos
        /// (el que se sostiene pasa al final, el Nivel 3 los ordena por profundidad) los deja
        /// después de ella; si alguno queda antes, la capa se adelanta, nunca se atrasa.
        /// </summary>
        private void KeepBehind(RectTransform target, RectTransform parent)
        {
            var layer = shadowRect.parent as RectTransform;
            if (layer == null || layer.parent != parent)
            {
                shadowRect.SetParent(LayerFor(parent, target.GetSiblingIndex()), false);
                return;
            }

            var index = target.GetSiblingIndex();
            if (index < layer.GetSiblingIndex())
            {
                layer.SetSiblingIndex(index);
            }
        }

        /// <summary>El tamaño con que se dibuja la ilustración dentro de la casilla (con <c>preserveAspect</c>).</summary>
        private Vector2 Drawn(RectTransform target)
        {
            var size = target.rect.size;
            var sprite = _art != null ? _art.sprite : null;
            if (sprite == null || _art == null || !_art.preserveAspect || sprite.rect.height <= 0f || size.y <= 0f)
            {
                return size;
            }

            var spriteAspect = sprite.rect.width / sprite.rect.height;
            return size.x / size.y > spriteAspect
                ? new Vector2(size.y * spriteAspect, size.y)
                : new Vector2(size.x, size.x / spriteAspect);
        }

        /// <summary>
        /// La base del dibujo, a partir de la malla del sprite (ajustada al contorno opaco): así la
        /// elipse no cae en el margen transparente del PNG. En fracción de la media caja dibujada.
        /// </summary>
        private void MeasureContact(Sprite art)
        {
            var minX = -0.85f;
            var maxX = 0.85f;
            var minY = -0.84f;

            var vertices = art != null ? art.vertices : null;
            if (vertices != null && vertices.Length > 0 && art.rect.width > 0f && art.rect.height > 0f)
            {
                var halfW = art.rect.width * 0.5f;
                var halfH = art.rect.height * 0.5f;
                var ppu = art.pixelsPerUnit > 0f ? art.pixelsPerUnit : 100f;
                minX = float.MaxValue;
                maxX = float.MinValue;
                minY = float.MaxValue;
                foreach (var vertex in vertices)
                {
                    // Los vértices van en unidades desde el pivote del sprite; se pasan al centro.
                    var x = (vertex.x * ppu + art.pivot.x - halfW) / halfW;
                    var y = (vertex.y * ppu + art.pivot.y - halfH) / halfH;
                    minX = Mathf.Min(minX, x);
                    maxX = Mathf.Max(maxX, x);
                    minY = Mathf.Min(minY, y);
                }

                minX = Mathf.Clamp(minX, -1f, 1f);
                maxX = Mathf.Clamp(maxX, -1f, 1f);
                minY = Mathf.Clamp(minY, -1f, 1f);
            }

            contactCenter = new Vector2((minX + maxX) * 0.5f, minY);
            contactWidth = (maxX - minX) * 0.5f * 0.95f;
        }

        private static Color ShadowColor(float alpha) => new Color(0f, 0f, 0f, alpha);

        private static bool Contains(string name, string part) =>
            name.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;

        private static void DestroyObject(UnityEngine.Object target)
        {
            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
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
