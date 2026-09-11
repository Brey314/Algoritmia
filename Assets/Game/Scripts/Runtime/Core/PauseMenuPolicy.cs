namespace Game.Core
{
    /// <summary>Las reglas del menú de pausa (RF-07, HU-17): qué hace «Reiniciar» y qué no.</summary>
    public static class PauseMenuPolicy
    {
        /// <summary>
        /// Repite la fase en curso desde cero, a través de <see cref="GameFlowRunner.StartPlaying"/>
        /// —igual que cualquier otro controlador, nunca llamando a <see cref="GameFlow"/>
        /// directamente— para que la recarga de escena de <c>GameFlowRunner.Apply</c> se dispare
        /// de verdad. Nunca re-bloquea un nivel ni borra un indicador ya confirmado (RF-41,
        /// CP-02): esos solo cambian con <see cref="PlayerProfile.Reach"/> /
        /// <see cref="PlayerProfile.ConfirmPhase"/>, que ni esto ni <c>TryStartPlaying</c> llaman.
        /// </summary>
        public static bool Restart(GameFlowRunner runner) =>
            runner.Flow.PlayingLevel is { } level && runner.StartPlaying(level, runner.Flow.PlayingPhase);
    }
}
