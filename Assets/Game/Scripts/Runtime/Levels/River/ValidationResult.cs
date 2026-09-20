using System.Collections.Generic;

namespace Game.Levels.River
{
    /// <summary>
    /// Lo que devuelve confirmar una fase o probar la balsa (RF-42): si pasó, **qué espacios**
    /// están mal —nunca «el ensamblaje»— y la frase que dice qué revisar.
    /// </summary>
    /// <remarks>
    /// La frase orienta y no resuelve (CP-06): nombra el espacio marcado, no la pieza que va en
    /// él. Sin cifras (CP-03, RF-17).
    /// </remarks>
    public readonly struct ValidationResult
    {
        public ValidationResult(bool passed, IReadOnlyList<string> wrongSlotIds, string message)
        {
            Passed = passed;
            WrongSlotIds = wrongSlotIds ?? System.Array.Empty<string>();
            Message = message ?? string.Empty;
        }

        public bool Passed { get; }

        /// <summary>Los espacios señalados: vacíos o con una pieza que no es la suya.</summary>
        public IReadOnlyList<string> WrongSlotIds { get; }

        public string Message { get; }
    }
}
