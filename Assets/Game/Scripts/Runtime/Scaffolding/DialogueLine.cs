using System;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Una línea de una escena narrativa: quién habla y qué dice (RF-05).
    /// </summary>
    /// <remarks>
    /// El texto vive en el asset y nunca en una clase (CT-05, RNF-18): así se corrige una línea
    /// sin recompilar. La acotación narrativa —lo que en el guion va en cursiva— es una línea
    /// más, con <see cref="Speaker"/> vacío.
    /// </remarks>
    [Serializable]
    public class DialogueLine
    {
        [field: SerializeField]
        [field: Tooltip("Quién habla. Vacío en las acotaciones, que no llevan nombre.")]
        public string Speaker { get; private set; }

        [field: SerializeField, TextArea(2, 6)]
        [field: Tooltip("La línea tal como la lee el estudiante. Ninguna oración pasa de 20 palabras (RNF-01).")]
        public string Text { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Efecto que suena al mostrarse la línea, para que lo que el texto describe se oiga. Refuerza, nunca informa solo (Dirección de sonido §2.4). Vacío = nada.")]
        public AudioClip Sound { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Ambiente que entra al mostrarse la línea, con fundido cruzado sobre el que sonara — la cueva cuando la familia entra en ella. Vacío = sigue el que suena.")]
        public AudioClip Ambient { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Silencio del guion que dispara esta línea (Dirección de sonido §5): corte seco de la música —queda el ambiente— o de todo. Ninguno por defecto.")]
        public SilenceCut Silence { get; private set; }

        // INC-148: van AL FINAL de los campos serializados. Los 18 assets narrativos se escribieron sin ellos y Unity
        // deserializa lo que falta con el valor del inicializador (sin marcar): ninguna línea cambia ni se reescribe.
        [field: SerializeField]
        [field: Tooltip("Si la línea fija la expresión con la que se ve su retrato en la tarjeta del cuadro de diálogo. SOLO para las voces cuyo hablante no está en escena (la voz fuera de cuadro): quien está en escena lleva la expresión que le fija su paso (Beats), y ahí esta casilla no cuenta. Sin marcar, la voz fuera de escena se ve con la cara neutra.")]
        public bool SetsVoiceEmotion { get; private set; }

        [field: SerializeField]
        [field: Tooltip("La expresión del retrato de la voz fuera de escena en esta línea. Solo cuenta si «Sets Voice Emotion» está marcado. No hay expresión de derrota ni de tristeza (CP-02).")]
        public FacialEmotion VoiceEmotion { get; private set; } = FacialEmotion.Neutral;

        public DialogueLine(string speaker, string text)
        {
            Speaker = speaker;
            Text = text;
        }

        /// <summary>Requerido por la serialización de Unity.</summary>
        private DialogueLine()
        {
        }

        /// <summary>Si la línea es una acotación y no un parlamento.</summary>
        public bool IsStageDirection => string.IsNullOrWhiteSpace(Speaker);
    }
}
