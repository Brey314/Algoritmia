namespace Game.Core
{
    /// <summary>
    /// Estados del flujo del juego. Son pocos a propósito: <see cref="Narrative"/> y
    /// <see cref="Playing"/> van parametrizados, así que una escena narrativa nueva es un asset
    /// nuevo y no un estado nuevo con su rama.
    /// </summary>
    /// <remarks>
    /// No hay <c>GameOver</c> ni equivalente, y no es un olvido: CP-02 prohíbe la pantalla de
    /// derrota, el límite de intentos y la penalización. La razón es pedagógica, no técnica —
    /// el error es material de trabajo, no un final. Añadir aquí un estado de derrota rompe el
    /// criterio aunque compile.
    /// </remarks>
    public enum GameState
    {
        Boot,
        MainMenu,
        ProfileSelect,
        LevelSelect,
        Narrative,
        Playing,
        LevelSummary,
        Credits,

        /// <summary>RF-46. Existe en el flujo; su escena llega en el Slice 4.</summary>
        TeacherReport
    }
}
