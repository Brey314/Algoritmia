using Game.Core;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Cuándo se explica el botón de ayuda en la cueva (INC-158). C# plano, para probarlo sin escena.
    /// </summary>
    public static class FireHelpIntro
    {
        /// <summary>
        /// La primera mecánica del juego es la fase 1 del Nivel 1: el aviso sale mientras el perfil
        /// activo no la haya confirmado. Se deriva del progreso, como <c>NarrativeVisitPolicy</c>:
        /// no hay un «ya lo vio» guardado porque la lista persistida es cerrada (RNF-09). Sin perfil
        /// (la escena abierta sin pasar por Boot, las pruebas) no sale: no hay a quién explicarle
        /// nada, y el aviso detiene el tiempo.
        /// </summary>
        public static bool ShouldShow(PlayerProfile profile) =>
            profile != null && !profile.IsPhaseConfirmed(new PhaseId(LevelId.Fire, 1));
    }
}
