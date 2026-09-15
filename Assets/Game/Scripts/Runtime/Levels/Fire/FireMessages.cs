using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Los mensajes del registro del Nivel 1 (guion §4.3.4; los de fallo reescritos en las Fases
    /// 5 y 6 para hablar de fuerza y de cercanía de las piedras en vez de distancia).
    /// Observacionales y sin cifras (RF-17).
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
        [field: Tooltip("Golpe con las piedras separadas: no se tocan (T26). Mensaje por defecto.")]
        public string StonesFar { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe con las piedras separadas tras dos fallos seguidos así.")]
        public string StonesFarAgain { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe con las piedras demasiado encimadas: se frotan, no chocan (T26). Mensaje por defecto.")]
        public string StonesTooClose { get; set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Golpe con las piedras demasiado encimadas tras dos fallos seguidos así.")]
        public string StonesTooCloseAgain { get; set; }

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
