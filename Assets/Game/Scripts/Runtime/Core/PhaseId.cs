using System;
using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>
    /// Identifica una fase jugable: el nivel más el índice de la fase dentro de él, en base 1
    /// (RF-04). El Nivel 1 se resuelve en una sola fase; el Nivel 2 encadena tres.
    /// </summary>
    /// <remarks>
    /// Es un tipo de tiempo de ejecución, no la forma del guardado: el JSON del perfil sigue
    /// escribiendo <c>level</c> y <c>phase</c> por separado, que es la lista cerrada ya radicada
    /// (RNF-09, OE1 §3.6.1 nota 5). Unificar los dos campos en el archivo cambiaría un
    /// entregable, no solo el código.
    /// </remarks>
    public readonly struct PhaseId : IEquatable<PhaseId>
    {
        /// <summary>
        /// Cuántas fases tiene cada nivel, en el orden de <see cref="LevelId"/>. El Nivel 3
        /// ensambla la balsa en tres fases (RF-40, INC-30) aunque muestre cuatro tareas: son
        /// cosas distintas y la tabla cuenta fases.
        /// </summary>
        private static readonly int[] PhasesPerLevel = { 1, 3, 3 };

        /// <summary>Nivel al que pertenece la fase.</summary>
        public LevelId Level { get; }

        /// <summary>Índice de la fase dentro del nivel, desde 1.</summary>
        public int Phase { get; }

        public PhaseId(LevelId level, int phase)
        {
            if (phase < 1 || phase > PhaseCountOf(level))
            {
                throw new ArgumentOutOfRangeException(nameof(phase), phase,
                    $"El nivel {level} tiene {PhaseCountOf(level)} fase(s).");
            }

            Level = level;
            Phase = phase;
        }

        /// <summary>Cuántas fases hay que confirmar para completar el nivel (RF-03, RF-04).</summary>
        public static int PhaseCountOf(LevelId level) => PhasesPerLevel[(int)level - 1];

        /// <summary>Las fases del nivel en el orden en que se juegan.</summary>
        public static IEnumerable<PhaseId> AllOf(LevelId level)
        {
            for (var phase = 1; phase <= PhaseCountOf(level); phase++)
            {
                yield return new PhaseId(level, phase);
            }
        }

        public bool Equals(PhaseId other) => Level == other.Level && Phase == other.Phase;

        public override bool Equals(object obj) => obj is PhaseId other && Equals(other);

        public override int GetHashCode() => HashCode.Combine((int)Level, Phase);

        public override string ToString() => $"{Level}/fase {Phase}";
    }
}
