namespace Game.Levels.Fire
{
    /// <summary>
    /// Estado del panel de encendido del Nivel 1: resuelve un golpe según la distancia y lleva los
    /// dos contadores —golpes efectivos y fallos consecutivos— del guion §4.3.3/§4.3.5.
    /// </summary>
    /// <remarks>
    /// C# plano, sin dependencias de Unity: la regla se prueba en EditMode, sin escena ni frames.
    /// El panel jugable (T14) solo traduce el clic de «Golpear» a <see cref="Strike"/> y el estado
    /// a la UI. No guarda la posición del deslizante: esa es estado de UI y solo entra al llamar
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

        /// <summary>Resuelve un golpe desde la posición dada y devuelve lo observado.</summary>
        public StrikeOutcome Strike(StrikePosition position)
        {
            if (position != _config.EffectivePosition)
            {
                _consecutiveFailures++;
                return StrikeOutcome.SparksDied(position);
            }

            // «Por qué no» pedagógico: lo ganado no se reduce nunca. Un fallo posterior no baja
            // _effectiveStrikes y CanBlow no vuelve a falso (INC-32, CP-02, guion §4.3.6). Tampoco
            // hay tope de intentos ni contador de derrota (RF-18): _consecutiveFailures solo crece
            // hasta el siguiente acierto. Una «mejora» que penalice reintroduce la pantalla de
            // derrota que el proyecto prohíbe.
            _effectiveStrikes++;
            _consecutiveFailures = 0;
            return StrikeOutcome.SparkLanded(_effectiveStrikes);
        }
    }
}
