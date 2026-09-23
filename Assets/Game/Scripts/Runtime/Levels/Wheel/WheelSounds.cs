using UnityEngine;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// Las piezas de sonido del Nivel 2 (<c>Direccion_de_Musica_y_Sonido.md</c> §8 y §12): el
    /// bosque de día, lo que suena al apartarse cada objeto del suelo y la carretilla del
    /// laberinto. Viven en un asset y no en el código para que cambiar una toma no cueste una
    /// recompilación (CT-05, RNF-18); las escenas las disparan y <c>AudioManager</c> las mezcla.
    /// </summary>
    /// <remarks>
    /// «Por qué no» pedagógico: aquí no hay ninguna pieza para el fallo, y no debe haberla. Un
    /// distractor que se aparta suena a lo que es —piedra, hojas— y la carretilla que choca y
    /// vuelve suena a carretilla (§2.1, CP-02). Cualquier campo con nombre de derrota, error o
    /// fallo reintroduce el pitido que el proyecto prohíbe.
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritm/Sonidos del nivel rueda", fileName = "N2_Sonidos")]
    public class WheelSounds : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("amb_n2_bosque_dia · pájaros, hojas, viento medio. El fondo de las tres fases del nivel y de sus escenas de día; entra sin costura desde la narrativa, que pide el mismo clip.")]
        public AudioClip ForestAmbient { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_n2_troncos · un tronco redondo que rueda al pasarle el cursor cerca. Suena una vez por acercamiento.")]
        public AudioClip LogNudge { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_n2_piedra_cae · la piedra que vuelca una esquina, y la herramienta que se arrastra un pelo: las dos son lo que no rueda. Una vez por acercamiento.")]
        public AudioClip StoneNudge { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_n1_hojas_acomodo · hojas que se mueven. Suena en bucle mientras alguna planta sigue en el aire o deslizándose, y calla cuando todas se posan.")]
        public AudioClip LeafNudge { get; private set; }

        [field: SerializeField]
        [field: Range(0f, 0.2f)]
        [field: Tooltip("Variación de tono ± al apartarse un objeto: hay una sola toma por categoría y no puede sonar idéntica dos veces seguidas (§14.6).")]
        public float NudgePitchJitter { get; private set; } = 0.06f;

        [field: SerializeField]
        [field: Tooltip("sfx_n2_carretilla · la rueda de la carretilla. Suena en bucle mientras la carretilla recorre la secuencia en el laberinto, también en el intento que choca y vuelve (RF-32, RF-33), y calla al terminar.")]
        public AudioClip CartMove { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_encaje_pieza · un tronco redondo llega al acopio (RF-24).")]
        public AudioClip LogCollected { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_n1_pieza_tomar · los cinco troncos acopiados: suena una vez, cuando empiezan a volar hacia la fila.")]
        public AudioClip AllLogsCollected { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_encaje_pieza · un tronco corto se perfora y pasa a ser rueda, con «Mecanizar» o con el martillo (RF-28).")]
        public AudioClip Drilled { get; private set; }

        [field: SerializeField]
        [field: Tooltip("sfx_martillo_madera · una pieza encaja en la carretilla (eje, tabla o caja, RF-29): suena varios golpes seguidos.")]
        public AudioClip AssemblyHammer { get; private set; }

        [field: SerializeField]
        [field: Min(1)]
        [field: Tooltip("Cuántos golpes de martillo suenan al encajar una pieza.")]
        public int AssemblyHits { get; private set; } = 3;

        [field: SerializeField]
        [field: Min(0f)]
        [field: Tooltip("Segundos entre un golpe de martillo y el siguiente.")]
        public float AssemblyHitSeconds { get; private set; } = 0.3f;

        [field: SerializeField]
        [field: Tooltip("sfx_n1_pieza_tomar · la carretilla terminada: suena una vez al acabar el acercamiento de cámara, antes de salir a la narrativa.")]
        public AudioClip CartBuilt { get; private set; }

        /// <summary>Lo que suena cuando el cursor aparta un objeto de esa categoría; nulo si nada.</summary>
        /// <remarks>La planta no está aquí: la suya es un bucle que dura lo que dure el vuelo, no un disparo.</remarks>
        public AudioClip NudgeClipFor(ForestObjectCategory category) => category switch
        {
            ForestObjectCategory.RoundLog => LogNudge,
            ForestObjectCategory.Stone => StoneNudge,
            ForestObjectCategory.Tool => StoneNudge,
            _ => null
        };
    }
}
