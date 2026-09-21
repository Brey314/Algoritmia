namespace Game.Levels.Fire
{
    /// <summary>
    /// Cómo quedan las dos piedras según la muesca del deslizante de cercanía (Fase 6, 15/09/2026:
    /// la cercanía de las piedras se elige con un deslizante, no arrastrándolas — pedido de
    /// Santiago, INC-47).
    /// </summary>
    public enum SpacingBand
    {
        /// <summary>Separadas: no llegan a tocarse y el golpe no produce chispa.</summary>
        TooFar,

        /// <summary>Se rozan, apenas encimadas: el choque hace chispa.</summary>
        Effective,

        /// <summary>Tan encimadas que no chocan: solo se frotan.</summary>
        TooClose
    }
}
