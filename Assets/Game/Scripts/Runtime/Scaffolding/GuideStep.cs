using System;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Una tarea del nivel vista desde el andamiaje: su instrucción y su pista (RF-13).
    /// </summary>
    /// <remarks>
    /// Los dos textos viven en el asset y nunca en una clase (CT-05, RNF-18): corregir una pista
    /// es editar el asset, no recompilar. Son dos campos y no uno porque RF-13 pide **dos
    /// mecanismos distintos** — la instrucción se repite a demanda, la pista llega sola tras tres
    /// fallos— y confundirlos convertiría la ayuda en la respuesta (CP-06).
    /// </remarks>
    [Serializable]
    public class GuideStep
    {
        [field: SerializeField]
        [field: Tooltip("Identificador de la tarea, p. ej. «Golpear». Solo una está activa a la vez (RNF-03).")]
        public string Id { get; private set; }

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("La instrucción vigente, tal como la repite el botón de ayuda (RF-13, HU-03).")]
        public string Instruction { get; private set; }

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("La pista tras tres fallos: orienta con una pregunta y nunca nombra la respuesta (CP-06).")]
        public string Hint { get; private set; }

        public GuideStep(string id, string instruction, string hint)
        {
            Id = id;
            Instruction = instruction;
            Hint = hint;
        }

        /// <summary>Requerido por la serialización de Unity.</summary>
        private GuideStep()
        {
        }
    }
}
