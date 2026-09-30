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
        /// <remarks>
        /// «Completado» no es que lo diga quien llama, sino que estén confirmadas **todas** las
        /// fases del nivel (W02). Con un nivel de una sola fase daba igual; con las tres del
        /// Nivel 2, aceptar la palabra del llamante dejaría el Nivel 3 abierto a media rueda.
        /// </remarks>
        public static void UnlockAfterCompleting(PlayerProfile profile, LevelId completed)
        {
            if (completed < LevelId.River && profile.IsLevelComplete(completed))
            {
                profile.Reach(completed + 1);
            }
        }

        /// <summary>
        /// Si el nivel se puede jugar con el progreso del perfil (RF-03): lo dice
        /// <see cref="PlayerProfile.ReachedLevel"/>, o —cuando el cierre del nivel anterior no
        /// llegó a mostrar el resumen que lo avanza (un cierre forzado a mitad de la escena de
        /// cierre, RNF-14, PF-RNF14-04, 30/09/2026)— que sus fases estén **todas** confirmadas.
        /// </summary>
        /// <remarks>
        /// Derivado, no persistido (RNF-09): no añade ningún campo, solo repite aquí la misma
        /// lectura que ya hace <see cref="PlayerProfile.IsLevelComplete"/>. La consultan el menú de
        /// niveles y <see cref="GameFlow.TryStartPlaying"/>, para que un nivel pintado como
        /// desbloqueado también se pueda empezar. No vive en <see cref="PlayerProfile.IsUnlocked"/>
        /// porque el cierre reflexivo (<c>Game.Scaffolding.NarrativeVisitPolicy</c>) sigue leyendo
        /// <c>ReachedLevel</c> directamente: así la primera vuelta del cierre reflexivo sigue sin
        /// poder omitirse aunque el desbloqueo llegue por aquí.
        /// </remarks>
        public static bool IsUnlocked(PlayerProfile profile, LevelId level) =>
            profile.IsUnlocked(level) || (level > LevelId.Fire && profile.IsLevelComplete(level - 1));
    }
}
