namespace Game.Core
{
    /// <summary>
    /// La regla de desbloqueo de niveles (RF-03): completar un nivel habilita el siguiente y nada
    /// más. C# plano, sin Unity — la escena del menú de niveles solo la pinta.
    /// </summary>
    public static class LevelUnlockPolicy
    {
        /// <summary>Los tres niveles en el orden en que se recorren.</summary>
        public static readonly LevelId[] AllLevels = { LevelId.Fire, LevelId.Wheel, LevelId.River };

        /// <summary>
        /// Habilita el nivel siguiente al que se acaba de completar. Nunca retrocede —lo garantiza
        /// <see cref="PlayerProfile.Reach"/>— así que un fallo posterior no re-bloquea nada (CP-02,
        /// RF-41). Completar el último nivel no habilita ninguno más.
        /// </summary>
        public static void UnlockAfterCompleting(PlayerProfile profile, LevelId completed)
        {
            if (completed < LevelId.River)
            {
                profile.Reach(completed + 1);
            }
        }

        /// <summary>Si el nivel se puede jugar con el progreso del perfil (RF-03).</summary>
        public static bool IsUnlocked(PlayerProfile profile, LevelId level) => profile.IsUnlocked(level);
    }
}
