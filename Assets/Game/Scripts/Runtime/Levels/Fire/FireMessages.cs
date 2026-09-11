using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Los ocho mensajes del área de registro del Nivel 1 (guion §4.3.4). Viven en un asset y no
    /// en el código: corregir un mensaje es editarlo en el Inspector, no recompilar (CT-05,
    /// RNF-18). Son observacionales, sin cifras ni juicio de valor (RF-17, CP-03).
    /// </summary>
    [CreateAssetMenu(menuName = "Algoritm/Mensajes del nivel fuego", fileName = "N1_Mensajes")]
    public class FireMessages : ScriptableObject
    {
        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe desde «Lejos». Mensaje por defecto de esa distancia.")]
        public string FarSparksFade { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe desde «Lejos» tras dos fallos seguidos en esa distancia.")]
        public string FarNeverReach { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe desde «Cerca». Mensaje por defecto de esa distancia.")]
        public string NearColdStone { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe desde «Cerca» tras dos fallos seguidos en esa distancia.")]
        public string NearBesideLeaves { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Primer golpe efectivo desde «Muy cerca».")]
        public string FirstEffectiveStrike { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Segundo golpe efectivo desde «Muy cerca».")]
        public string SecondEffectiveStrike { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe efectivo que alcanza el mínimo y habilita el soplo (convergencia).")]
        public string FinalEffectiveStrike { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("El soplo tiene éxito y nace el fuego (guion §4.3.4, §4.4).")]
        public string BlowSuccess { get; set; }
    }
}
