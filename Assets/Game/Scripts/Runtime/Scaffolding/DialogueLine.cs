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
