using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.Fire
{
    /// <summary>
    /// El asa del deslizante de fuerza crece y enrojece cuanto más fuerte es el golpe elegido
    /// (mockup 7, pedido de Santiago 12/09/2026): la hipótesis se ve antes de probarla (RF-15).
    /// </summary>
    /// <remarks>
    /// Doble indicador a propósito (RNF-19): tamaño y color, nunca solo el color. No toca el valor
    /// del deslizante ni ejecuta nada: es puro reflejo de la muesca elegida.
    /// </remarks>
    [RequireComponent(typeof(Slider))]
    public class ForceSliderFeedback : MonoBehaviour
    {
        [SerializeField] private RectTransform knob;
        [SerializeField] private Image knobFace;

        [SerializeField]
        [Tooltip("Color del asa con la fuerza mínima: azul del todo. Cada muesca mezcla un décimo más de rojo.")]
        private Color softColor = Color.blue;

        [SerializeField]
        [Tooltip("Color del asa con la fuerza máxima: rojo del todo.")]
        private Color strongColor = Color.red;

        [field: SerializeField]
        [field: Tooltip("Escala del asa con la fuerza mínima.")]
        public float MinScale { get; set; } = 0.8f;

        [field: SerializeField]
        [field: Tooltip("Escala del asa con la fuerza máxima: la de la muesca cinco de antes, para que no tape las etiquetas.")]
        public float MaxScale { get; set; } = 1.2f;

        private Slider _slider;

#if UNITY_INCLUDE_TESTS
        internal RectTransform Knob => knob;
        internal Image KnobFace => knobFace;
#endif

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(_ => Apply());
        }

        private void Start() => Apply();

        private void Apply()
        {
            var t = _slider.normalizedValue;
            knob.localScale = Vector3.one * Mathf.Lerp(MinScale, MaxScale, t);
            knobFace.color = Color.Lerp(softColor, strongColor, t); // muesca 1 = 90 % azul, 10 % rojo
        }
    }
}
