namespace Game.Levels.Fire
{
    /// <summary>
    /// Lo que se observa tras un golpe (guion §4.3.3, con fuerza en vez de distancia desde la
    /// Fase 5). Lo consume el log de retroalimentación (T13) para elegir el mensaje narrativo.
    /// </summary>
    public readonly struct StrikeOutcome
    {
        /// <summary>El golpe cayó en la franja efectiva y una chispa prendió en las hojas.</summary>
        public bool Effective { get; }

        /// <summary>Fuerza con la que se golpeó, en las muescas del deslizante.</summary>
        public int Force { get; }

        /// <summary>Dónde cayó la fuerza respecto de la franja efectiva. Decide el mensaje de un fallo.</summary>
        public ForceBand Band { get; }

        /// <summary>Las dos piedras estaban cerca de las hojas al golpear (T22). Falso: el fallo fue de sitio, no de fuerza.</summary>
        public bool StonesNear { get; }

        /// <summary>Golpes efectivos acumulados tras este golpe.</summary>
        public int EffectiveStrikes { get; }

        private StrikeOutcome(bool effective, int force, ForceBand band, bool stonesNear, int effectiveStrikes)
        {
            Effective = effective;
            Force = force;
            Band = band;
            StonesNear = stonesNear;
            EffectiveStrikes = effectiveStrikes;
        }

        /// <summary>Golpe fuera de la franja: las chispas no llegan a las hojas.</summary>
        public static StrikeOutcome SparksDied(int force, ForceBand band) => new(false, force, band, true, 0);

        /// <summary>Golpe con alguna piedra lejos de las hojas: las chispas caen donde no hay nada que prender.</summary>
        public static StrikeOutcome StonesTooFar(int force, ForceBand band) => new(false, force, band, false, 0);

        /// <summary>
        /// Golpe efectivo: una chispa cae dentro del montón. <paramref name="effectiveStrikes"/>
        /// es el acumulado tras contarlo.
        /// </summary>
        public static StrikeOutcome SparkLanded(int force, int effectiveStrikes) =>
            new(true, force, ForceBand.Effective, true, effectiveStrikes);
    }
}
