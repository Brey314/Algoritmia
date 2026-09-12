namespace Game.Levels.Fire
{
    /// <summary>
    /// Estado del panel de encendido del Nivel 1: resuelve un golpe según la fuerza y lleva los
    /// dos contadores —golpes efectivos y fallos consecutivos— del guion §4.3.3/§4.3.5.
    /// </summary>
    /// <remarks>
    /// C# plano, sin dependencias de Unity: la regla se prueba en EditMode, sin escena ni frames.
    /// El panel jugable (T14) solo traduce el clic de «Golpear» a <see cref="Strike"/> y el estado
    /// a la UI. No guarda la fuerza del deslizante: esa es estado de UI y solo entra al llamar
    /// <see cref="Strike"/> (RF-15).
    /// </remarks>
    public class FireAttempt
    {
        private readonly FireLevelConfig _config;
        private int _effectiveStrikes;
        private int _consecutiveFailures;

        public FireAttempt(FireLevelConfig config) => _config = config;

        /// <summary>Golpes efectivos acumulados. Nunca decrece (guion §4.3.6).</summary>
        public int EffectiveStrikes => _effectiveStrikes;

        /// <summary>Fallos seguidos desde el último golpe efectivo.</summary>
        public int ConsecutiveFailures => _consecutiveFailures;

        /// <summary>El soplo está disponible: se alcanzó el mínimo de golpes efectivos (INC-32).</summary>
        public bool CanBlow => _effectiveStrikes >= _config.MinimumEffectiveStrikes;

        /// <summary>Toca que el guía ofrezca una pista: fallos consecutivos suficientes (RF-13).</summary>
        public bool ShouldOfferHint => _consecutiveFailures >= _config.AttemptsBeforeHint;

        /// <summary>Dónde cae una fuerza respecto de la franja efectiva de la configuración.</summary>
        public ForceBand Classify(int force) =>
            force < _config.EffectiveForceMin ? ForceBand.TooSoft
            : force > _config.EffectiveForceMax ? ForceBand.TooHard
            : ForceBand.Effective;

        /// <summary>
        /// Resuelve un golpe con la fuerza dada y devuelve lo observado. Con alguna piedra lejos
        /// de las hojas (<paramref name="stonesNear"/> falso) no prende con ninguna fuerza (T22).
        /// </summary>
        public StrikeOutcome Strike(int force, bool stonesNear = true)
        {
            var band = Classify(force);
            if (!stonesNear)
            {
                _consecutiveFailures++;
                return StrikeOutcome.StonesTooFar(force, band);
            }

            if (band != ForceBand.Effective)
            {
                _consecutiveFailures++;
                return StrikeOutcome.SparksDied(force, band);
            }

            // «Por qué no» pedagógico: lo ganado no se reduce nunca. Un fallo posterior no baja
            // _effectiveStrikes y CanBlow no vuelve a falso (INC-32, CP-02, guion §4.3.6). Tampoco
            // hay tope de intentos ni contador de derrota (RF-18): _consecutiveFailures solo crece
            // hasta el siguiente acierto. Una «mejora» que penalice reintroduce la pantalla de
            // derrota que el proyecto prohíbe.
            _effectiveStrikes++;
            _consecutiveFailures = 0;
            return StrikeOutcome.SparkLanded(force, _effectiveStrikes);
        }
    }
}
