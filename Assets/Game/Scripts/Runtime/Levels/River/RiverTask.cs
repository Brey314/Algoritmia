namespace Game.Levels.River
{
    /// <summary>Las cuatro tareas de la lista del Nivel 3, en el orden del guion §1.8.1 (RF-36).</summary>
    public enum RiverTaskId
    {
        CollectLogs = 1,
        FindRopes = 2,
        AssembleRaft = 3,
        PlaceMastAndSail = 4
    }

    /// <summary>
    /// Cuándo se marca cada tarea (guion §1.8.2, INC-30). Es la regla que hace visible la
    /// descomposición y la más fácil de perder en la implementación.
    /// </summary>
    /// <remarks>
    /// **Confirmar la fase de base no marca nada.** La lectura intuitiva —«cada fase marca una
    /// tarea»— es la incorrecta: la tarea 3 («Ensamblar la balsa») la marca el **amarre**, y la 4
    /// el mástil y la vela. La tela y el mástil se recogen sin marcar tarea, porque los consume
    /// la tarea 4, que es de construcción (CU-09 FA-5a). Está resuelto en INC-30 y probado en
    /// negativo: sin esa prueba el error pasa (riesgo R4 del plan).
    /// </remarks>
    public static class RiverTask
    {
        public static readonly RiverTaskId[] All =
        {
            RiverTaskId.CollectLogs, RiverTaskId.FindRopes, RiverTaskId.AssembleRaft, RiverTaskId.PlaceMastAndSail
        };

        /// <summary>La tarea que marca recoger ese material, o ninguna.</summary>
        public static RiverTaskId? ForCollected(MaterialKind kind) => kind switch
        {
            MaterialKind.Logs => RiverTaskId.CollectLogs,
            MaterialKind.Ropes => RiverTaskId.FindRopes,
            _ => null
        };

        /// <summary>La tarea que marca confirmar esa fase de ensamblaje (base 1 · amarre 2 · mástil y vela 3), o ninguna.</summary>
        public static RiverTaskId? ForConfirmedPhase(int phase) => phase switch
        {
            2 => RiverTaskId.AssembleRaft,
            3 => RiverTaskId.PlaceMastAndSail,
            _ => null
        };
    }
}
