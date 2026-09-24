using System.Collections.Generic;
using System.Linq;
using Game.Core;

namespace Game.Reporting
{
    /// <summary>
    /// Un nivel del informe docente con sus fases, en el orden en que se juegan (RF-46, INC-35):
    /// el Nivel 1 con una, el Nivel 2 y el Nivel 3 con tres cada uno.
    /// </summary>
    public readonly struct LevelReportSection
    {
        public LevelId Level { get; }

        public IReadOnlyList<PhaseIndicators> Phases { get; }

        /// <summary>Si al menos una fase del nivel tiene datos (RF-46): un nivel no jugado no muestra ceros.</summary>
        public bool Played => Phases.Any(phase => phase.Played);

        public LevelReportSection(LevelId level, IReadOnlyList<PhaseIndicators> phases)
        {
            Level = level;
            Phases = phases;
        }
    }
}
