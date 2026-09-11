namespace Game.Scaffolding
{
    /// <summary>
    /// Qué pantalla sigue al terminar una escena narrativa (RF-05, RF-20). Lo declara el asset,
    /// no el código: así <c>NarrativeSceneController</c> no necesita un <c>if</c> por secuencia
    /// para saber a dónde ir.
    /// </summary>
    public enum NarrativeOutcome
    {
        /// <summary>Entra a jugar el nivel de la secuencia — narrativa de apertura.</summary>
        EntersLevel = 0,

        /// <summary>Vuelve al menú de niveles — narrativa de cierre.</summary>
        ReturnsToLevelSelect = 1
    }
}
