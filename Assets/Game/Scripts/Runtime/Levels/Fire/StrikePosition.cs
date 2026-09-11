namespace Game.Levels.Fire
{
    /// <summary>
    /// Las tres distancias del control deslizante del Nivel 1, de más lejos a más cerca del montón
    /// de hojas (guion §4.3.2). «Muy cerca» es la única posición efectiva por defecto.
    /// </summary>
    public enum StrikePosition
    {
        Far = 0,
        Near = 1,
        VeryClose = 2
    }
}
