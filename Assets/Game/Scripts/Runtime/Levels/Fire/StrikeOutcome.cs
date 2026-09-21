namespace Game.Levels.Fire
{
    /// <summary>
    /// Lo que se observa tras un golpe (guion §4.3.3, con fuerza y cercanía de las piedras en vez
    /// de distancia desde la Fase 5/6). Lo consume el log de retroalimentación (T13) para elegir
    /// el mensaje narrativo.
    /// </summary>
    public readonly struct StrikeOutcome
    {
        /// <summary>El golpe cayó en la franja efectiva y una chispa prendió en las hojas.</summary>
        public bool Effective { get; }

        /// <summary>Fuerza con la que se golpeó, en las muescas del deslizante.</summary>
        public int Force { get; }

        /// <summary>Dónde cayó la fuerza respecto de la franja efectiva. Decide el mensaje de un fallo de fuerza.</summary>
        public ForceBand Band { get; }

        /// <summary>Cómo estaban las piedras al golpear (T26). Distinto de efectivo: el fallo fue de cercanía, no de fuerza.</summary>
        public SpacingBand Spacing { get; }

        /// <summary>Golpes efectivos acumulados tras este golpe.</summary>
        public int EffectiveStrikes { get; }

        private StrikeOutcome(bool effective, int force, ForceBand band, SpacingBand spacing, int effectiveStrikes)
        {
            Effective = effective;
            Force = force;
            Band = band;
            Spacing = spacing;
            EffectiveStrikes = effectiveStrikes;
        }

        /// <summary>Golpe fuera de la franja de fuerza: las chispas no llegan a las hojas.</summary>
        public static StrikeOutcome SparksDied(int force, ForceBand band) =>
            new(false, force, band, SpacingBand.Effective, 0);

        /// <summary>Golpe con las piedras separadas o demasiado encimadas: no chocan como para hacer chispa.</summary>
        public static StrikeOutcome StonesMisplaced(int force, ForceBand band, SpacingBand spacing) =>
            new(false, force, band, spacing, 0);

        /// <summary>
        /// Golpe efectivo: una chispa cae dentro del montón. <paramref name="effectiveStrikes"/>
        /// es el acumulado tras contarlo.
        /// </summary>
        public static StrikeOutcome SparkLanded(int force, int effectiveStrikes) =>
            new(true, force, ForceBand.Effective, SpacingBand.Effective, effectiveStrikes);
    }
}
