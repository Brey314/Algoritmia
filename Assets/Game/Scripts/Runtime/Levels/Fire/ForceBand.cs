namespace Game.Levels.Fire
{
    /// <summary>
    /// Cómo cae la fuerza de un golpe respecto de la franja efectiva de la configuración
    /// (Fase 5, 12/09/2026: el deslizante mide fuerza, no distancia — pedido de Santiago, INC-47).
    /// </summary>
    public enum ForceBand
    {
        /// <summary>Por debajo de la franja: las piedras apenas se rozan.</summary>
        TooSoft,

        /// <summary>Dentro de la franja: una chispa cae en las hojas.</summary>
        Effective,

        /// <summary>Por encima de la franja: las chispas saltan lejos.</summary>
        TooHard
    }
}
