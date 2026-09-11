using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.Fire
{
    /// <summary>
    /// La iluminación de la cueva sube con el avance del reto (RF-21, guion §4.3.1/§4.3.5 E4/E7).
    /// </summary>
    /// <remarks>
    /// «Por qué no» pedagógico: es refuerzo visual del avance, nunca temporizador ni penalización
    /// — no hay ninguna cuenta atrás ni descuento aquí, solo espejo de <c>EffectiveStrikes</c>
    /// (CP-02, RNF-19: nunca es el único canal de retroalimentación).
    /// </remarks>
    public class CaveLightingController : MonoBehaviour
    {
        [SerializeField] private Image background;

        [SerializeField]
        [Tooltip("Color al inicio, sin ningún golpe efectivo. Ajustado (no el #0F1526 del " +
            "documento de arte) para mantener el contraste de texto ≥4.5:1 incluso en el " +
            "estado más oscuro (RNF-20) — ver plan T19.")]
        private Color darkest = new Color(0.541f, 0.592f, 0.671f); // ~#8A97AB

        private Color _litColor;

        internal float Progress { get; private set; }

#if UNITY_INCLUDE_TESTS
        internal Image Background => background;
        internal Color Darkest => darkest;
#endif

        private void Awake() => _litColor = background.color;

        /// <summary>Fija la iluminación proporcional al avance del reto (RF-21).</summary>
        internal void SetProgress(float fraction)
        {
            Progress = Mathf.Clamp01(fraction);
            background.color = Color.Lerp(darkest, _litColor, Progress);
        }
    }
}
