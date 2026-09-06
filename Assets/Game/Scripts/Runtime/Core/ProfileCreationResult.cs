namespace Game.Core
{
    /// <summary>
    /// Resultado de crear un perfil. Es un resultado tipado y no una excepción porque los dos
    /// rechazos —nombre vacío y nombre duplicado— son flujos alternos previstos de HU-01
    /// (FA-01 y FA-02), no fallos del sistema: la pantalla los muestra como un aviso.
    /// </summary>
    public readonly struct ProfileCreationResult
    {
        public enum Status
        {
            Created,

            /// <summary>HU-01 FA-01: el campo es obligatorio.</summary>
            EmptyName,

            /// <summary>HU-01 FA-02: ya hay un perfil con ese nombre.</summary>
            DuplicateName,

            /// <summary>El nombre no sirve como nombre de archivo dentro de <c>Datos/</c>.</summary>
            InvalidName
        }

        public PlayerProfile Profile { get; }

        public Status Result { get; }

        public bool Succeeded => Result == Status.Created;

        private ProfileCreationResult(PlayerProfile profile, Status result)
        {
            Profile = profile;
            Result = result;
        }

        internal static ProfileCreationResult Created(PlayerProfile profile) =>
            new ProfileCreationResult(profile, Status.Created);

        internal static ProfileCreationResult Rejected(Status reason) =>
            new ProfileCreationResult(null, reason);
    }
}
