using Game.Core;

namespace Game.Reporting
{
    /// <summary>
    /// Los cuatro indicadores de una fase para el informe docente (RF-46, INC-35), o la ausencia
    /// de ellos si la fase no se jugó. «Cero intentos» y «no haber jugado» no son lo mismo
    /// (RF-46): por eso <see cref="Played"/> existe en vez de devolver ceros.
    /// </summary>
    public readonly struct PhaseIndicators
    {
        public PhaseId Phase { get; }

        /// <summary>Si la fase tiene datos. En falso, <see cref="Indicators"/> no se lee.</summary>
        public bool Played { get; }

        public PerformanceIndicators Indicators { get; }

        private PhaseIndicators(PhaseId phase, bool played, PerformanceIndicators indicators)
        {
            Phase = phase;
            Played = played;
            Indicators = indicators;
        }

        public static PhaseIndicators NotPlayed(PhaseId phase) => new PhaseIndicators(phase, false, default);

        public static PhaseIndicators FromProfile(PhaseId phase, PerformanceIndicators indicators) =>
            new PhaseIndicators(phase, true, indicators);
    }
}
