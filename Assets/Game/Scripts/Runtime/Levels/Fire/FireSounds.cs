using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Las piezas de sonido del Nivel 1 (<c>Direccion_de_Musica_y_Sonido.md</c> §8 y §11): piedra
    /// y fuego. Viven en un asset y no en el código para que cambiar una toma no cueste una
    /// recompilación (CT-05, RNF-18); el panel las dispara y <c>AudioManager</c> las mezcla.
    /// </summary>
    /// <remarks>
    /// «Por qué no» pedagógico: aquí no hay ninguna pieza para el fallo, y no debe haberla. Un
    /// golpe que no prende suena a lo que pasó —las piedras chocan— y nada más (§2.1, CP-02).
    /// Cualquier campo que se añada con nombre de derrota, error o fallo reintroduce el pitido que
    /// el proyecto prohíbe.
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritm/Sonidos del nivel fuego", fileName = "N1_Sonidos")]
    public class FireSounds : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("amb_n1_cueva_oscura · goteo espaciado, eco largo, un hilo de aire: el sonido del problema del nivel. Suena desde la escena 1.1, entra aquí sin costura y no se va ni con el fuego.")]
        public AudioClip CaveAmbient { get; private set; }

        [field: SerializeField]
        [field: Tooltip("amb_n1_cueva_fuego · el crepitar de la hoguera. Entra por fundido SOBRE la cueva al nacer el fuego y ya no se va (RF-20, RF-21).")]
        public AudioClip FireAmbient { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Segundos que tarda en entrar la hoguera: la llama prende y crece sin golpe inicial (RNF-21; §11, sfx_n1_fuego_nace).")]
        public float FireAmbientFadeSeconds { get; private set; } = 3f;

        [field: SerializeField]
        [field: Tooltip("sfx_n1_hojas_acomodo · hojas secas que se mueven. Suena en bucle mientras se arrastra una hoja (clic sostenido, RNF-02) y calla al soltarla (INC-47).")]
        public AudioClip LeafDrag { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_n1_pieza_tomar · todo reunido dentro del círculo: suena una vez al pasar de reunir a encender (INC-47).")]
        public AudioClip Gathered { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_n1_golpe · sílex contra pedernal. Suena cuando las piedras se tocan, prenda o no —el fallo se describe, no se castiga (§2.1)—, y más fuerte cuanto más encimadas.")]
        public AudioClip Strike { get; private set; }

        [field: SerializeField]
        [field: Range(0f, 1f)]
        [field: Tooltip("Volumen del golpe al primer roce; crece hasta el máximo con una piedra encima de la otra.")]
        public float StrikeMinVolume { get; private set; } = 0.3f;

        [field: SerializeField]
        [field: Range(0f, 0.2f)]
        [field: Tooltip("Variación de tono ± del golpe: hay una sola toma y no puede sonar idéntica dos veces seguidas (§14.6).")]
        public float StrikePitchJitter { get; private set; } = 0.06f;

        [field: SerializeField]
        [field: Tooltip("sfx_n1_chispa · la chispa que cae dentro de las hojas. Solo en el golpe efectivo (RF-16, guion §4.3.3).")]
        public AudioClip Spark { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_n1_soplo · el aire de «Soplar» (RF-20).")]
        public AudioClip Blow { get; private set; }
    }
}
