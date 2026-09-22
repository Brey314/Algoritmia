namespace Game.Levels.River
{
    /// <summary>
    /// Las tres fases bloqueantes del ensamblaje (RF-40, guion §1.8.3), en el orden en que se
    /// habilitan. El valor es el índice de fase que persiste <c>PhaseId</c> (R02).
    /// </summary>
    public enum RaftPhase
    {
        Base = 1,
        Lashing = 2,
        MastAndSail = 3
    }
}
