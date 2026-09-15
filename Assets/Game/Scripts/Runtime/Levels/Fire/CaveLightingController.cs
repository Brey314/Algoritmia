using Game.Scaffolding;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.Fire
{
    /// <summary>
    /// La iluminación de la cueva sube con el avance del reto (RF-21, guion §4.3.1/§4.3.5 E4/E7):
    /// la misma capa de oscuridad de las escenas narrativas, sobre la vista cenital, con un charco
    /// fijo en el montón de hojas y el «fondo» atado al progreso (`docs/Camara_Narrativa_N1.md` §8).
    /// </summary>
    /// <remarks>
    /// «Por qué no» pedagógico: es refuerzo visual del avance, nunca temporizador ni penalización
    /// — no hay ninguna cuenta atrás ni descuento aquí, solo espejo de <c>EffectiveStrikes</c>
    /// (CP-02, RNF-19: nunca es el único canal de retroalimentación).
    /// </remarks>
    public class CaveLightingController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("El entorno cenital. La capa de oscuridad se crea encima, estirada a su rect.")]
        private Image background;

        [SerializeField]
        [Tooltip("Material con el shader Algoritm/Oscuridad (fx_oscuridad). Se instancia: no se toca el asset.")]
        private Material darknessMaterial;

        [SerializeField]
        [Tooltip("Luz sin ningún golpe efectivo y luz al converger: el «fondo» va de la primera a la segunda (§8: 0.10 → 0.30).")]
        private NarrativeLight darkest = new NarrativeLight(new Vector2(0.5f, 0.5f), 0.30f, 0.10f, new Color(1f, 0.90f, 0.72f));

        [SerializeField]
        private NarrativeLight brightest = new NarrativeLight(new Vector2(0.5f, 0.5f), 0.30f, 0.30f, new Color(1f, 0.90f, 0.72f));

        private Material _material;

        internal float Progress { get; private set; }

#if UNITY_INCLUDE_TESTS
        internal Image Background => background;
        internal NarrativeLight Darkest => darkest;
        internal NarrativeLight Current { get; private set; }
#endif

        private void Awake()
        {
            if (darknessMaterial == null)
            {
                return; // sin material no hay capa: el nivel sigue jugable, solo sin luz progresiva
            }

            var go = new GameObject("Oscuridad", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.layer = background.gameObject.layer;
            var rect = (RectTransform)go.transform;
            rect.SetParent(background.rectTransform, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var image = go.GetComponent<Image>();
            image.raycastTarget = false;
            _material = new Material(darknessMaterial);
            image.material = _material;
            var sprite = background.sprite;
            _material.SetFloat("_Aspect", sprite != null ? sprite.rect.width / sprite.rect.height : 1f);
        }

        /// <summary>Fija la iluminación proporcional al avance del reto (RF-21).</summary>
        internal void SetProgress(float fraction)
        {
            Progress = Mathf.Clamp01(fraction);
            var light = NarrativeLight.Lerp(darkest, brightest, Progress);
#if UNITY_INCLUDE_TESTS
            Current = light;
#endif
            if (_material == null)
            {
                return;
            }

            _material.SetVector("_Center", light.Center);
            _material.SetFloat("_Radius", light.Radius);
            _material.SetFloat("_Ambient", light.Ambient);
            _material.SetColor("_Tint", light.Tint);
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
            }
        }
    }
}
