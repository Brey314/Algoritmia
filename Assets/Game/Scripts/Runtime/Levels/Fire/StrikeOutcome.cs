namespace Game.Levels.Fire
{
    /// <summary>
    /// Lo que se observa tras un golpe (guion §4.3.3). Lo consume el log de retroalimentación
    /// (T13) para elegir el mensaje narrativo.
    /// </summary>
    public readonly struct StrikeOutcome
    {
        /// <summary>El golpe fue desde la posición efectiva y una chispa prendió en las hojas.</summary>
        public bool Effective { get; }

        /// <summary>
        /// Distancia desde la que se golpeó. Solo es significativa cuando <see cref="Effective"/>
        /// es falso: indica qué distancia se quedó corta.
        /// </summary>
        public StrikePosition Position { get; }

        /// <summary>Golpes efectivos acumulados tras este golpe.</summary>
        public int EffectiveStrikes { get; }

        private StrikeOutcome(bool effective, StrikePosition position, int effectiveStrikes)
        {
            Effective = effective;
            Position = position;
            EffectiveStrikes = effectiveStrikes;
        }

        /// <summary>Golpe desde una distancia no efectiva: las chispas se apagan (guion §4.3.3).</summary>
        public static StrikeOutcome SparksDied(StrikePosition position) => new(false, position, 0);

        /// <summary>
        /// Golpe efectivo: una chispa cae dentro del montón. <paramref name="effectiveStrikes"/>
        /// es el acumulado tras contarlo. La distancia es siempre la efectiva de la configuración.
        /// </summary>
        public static StrikeOutcome SparkLanded(int effectiveStrikes) =>
            new(true, default, effectiveStrikes);
    }
}
