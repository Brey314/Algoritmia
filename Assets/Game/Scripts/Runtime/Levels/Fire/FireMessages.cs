using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Los mensajes del registro del Nivel 1 (guion §4.3.4; los cuatro de fallo reescritos en la
    /// Fase 5 para hablar de fuerza en vez de distancia). Observacionales y sin cifras (RF-17).
    /// </summary>
    [CreateAssetMenu(menuName = "Algoritm/Mensajes del nivel fuego", fileName = "N1_Mensajes")]
    public class FireMessages : ScriptableObject
    {
        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe demasiado suave. Mensaje por defecto de esa franja.")]
        public string SoftNoSpark { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe demasiado suave tras dos fallos seguidos en esa franja.")]
        public string SoftStonesGraze { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe demasiado fuerte. Mensaje por defecto de esa franja.")]
        public string HardSparksScatter { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe demasiado fuerte tras dos fallos seguidos en esa franja.")]
        public string HardSparksFly { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe con el sílex o el pedernal lejos de las hojas (T22). Mensaje por defecto.")]
        public string StonesFar { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe con las piedras lejos tras dos fallos seguidos así.")]
        public string StonesFarAgain { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Soplo con las hojas regadas o las piedras lejos: no nace el fuego y no pasa nada más (T23, CP-02).")]
        public string BlowNoPile { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Primer golpe efectivo.")]
        public string FirstEffectiveStrike { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Segundo golpe efectivo.")]
        public string SecondEffectiveStrike { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe efectivo que alcanza el mínimo y habilita el soplo (convergencia).")]
        public string FinalEffectiveStrike { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("El soplo tiene éxito y nace el fuego (guion §4.3.4, §4.4).")]
        public string BlowSuccess { get; set; }
    }
}
