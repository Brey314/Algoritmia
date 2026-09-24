using System.Collections.Generic;
using System.Linq;
using Game.Core;

namespace Game.Reporting
{
    /// <summary>
    /// Agrega los indicadores de un perfil por nivel y por fase (RF-45, RF-46, INC-35), para la
    /// tabla del informe docente. Lógica pura: lee <see cref="PlayerProfile"/>, no formatea nada.
    /// </summary>
    /// <remarks>
    /// No calcula ningún agregado que OE1 §3.6.1 no defina: nada de promedios, porcentajes,
    /// totales ni «nivel de dominio». Inventar uno sería inventar un dato del estudiante que
    /// nadie autorizó (RNF-09).
    /// </remarks>
    public static class IndicatorReport
    {
        public static IReadOnlyList<LevelReportSection> For(PlayerProfile profile) =>
            new[] { LevelId.Fire, LevelId.Wheel, LevelId.River }
                .Select(level => BuildSection(profile, level))
                .ToArray();

        private static LevelReportSection BuildSection(PlayerProfile profile, LevelId level)
        {
            var phases = PhaseId.AllOf(level)
                .Select(phase => profile.IsPhaseConfirmed(phase)
                    ? PhaseIndicators.FromProfile(phase, profile.IndicatorsFor(phase))
                    : PhaseIndicators.NotPlayed(phase))
                .ToArray();

            return new LevelReportSection(level, phases);
        }
    }
}
