using UnityEngine;

namespace Game.Levels.River
{
    /// <summary>
    /// Las piezas de sonido del Nivel 3 (<c>Direccion_de_Musica_y_Sonido.md</c> §8 y §13): la
    /// orilla del río con el bosque de fondo, lo que suena al recoger un material y al clavar la
    /// balsa. Viven en un asset y no en el código para que cambiar una toma no cueste una
    /// recompilación (CT-05, RNF-18); la orilla y el panel las disparan y <c>AudioManager</c> las mezcla.
    /// </summary>
    /// <remarks>
    /// El cruce no está aquí: la balsa cruza en la escena narrativa 3.3, y el agua contra los
    /// troncos es contenido de esa secuencia (<c>NarrativeProp.MotionAmbient</c>).
    ///
    /// «Por qué no» pedagógico: aquí no hay ninguna pieza para el fallo, y no debe haberla. La
    /// pieza que vuelve al inventario y la balsa que se hunde no suenan a derrota (§2.1, CP-02).
    /// Cualquier campo con nombre de derrota, error o fallo reintroduce el pitido que el proyecto prohíbe.
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritm/Sonidos del nivel río", fileName = "N3_Sonidos")]
    public class RiverSounds : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("amb_n3_rio_orilla · la corriente contra la piedra. El fondo de todo el nivel (§8): suena en la orilla, en el ensamblaje y en las escenas del río, que piden el mismo clip y entran sin costura.")]
        public AudioClip RiverAmbient { get; private set; }

        [field: SerializeField]
        [field: Tooltip("amb_n2_bosque_dia · el bosque que rodea la orilla, en la segunda capa, sobre el río.")]
        public AudioClip ForestAmbient { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_encaje_pieza · un material entra al inventario al pulsar «Recoger» (RF-37).")]
        public AudioClip Collected { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_martillo_madera · una pieza queda puesta en un espacio de la balsa (RF-40): un solo golpe.")]
        public AudioClip PiecePlaced { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_martillo_madera · la fase aprobada con «Listo» o «Probar balsa» (RF-41): suena varios golpes seguidos, lo aprobado queda clavado.")]
        public AudioClip PhaseHammer { get; private set; }

        [field: SerializeField]
        [field: Min(1)]
        [field: Tooltip("Cuántos golpes de martillo suenan al aprobar una fase.")]
        public int PhaseHits { get; private set; } = 3;

        [field: SerializeField]
        [field: Min(0f)]
        [field: Tooltip("Segundos entre un golpe de martillo y el siguiente. La balsa terminada suena este tiempo después del último.")]
        public float PhaseHitSeconds { get; private set; } = 0.3f;

        [field: SerializeField]
        [field: Tooltip("sfx_n1_pieza_tomar · la balsa terminada: suena una vez tras los martillazos de la última fase, antes de salir al cruce.")]
        public AudioClip RaftBuilt { get; private set; }
    }
}
