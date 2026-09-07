namespace Game.Core
{
    /// <summary>
    /// Guarda el progreso del perfil activo. Lo consume la UI que puede cerrar la sesión
    /// —el menú principal al pulsar «Salir» (RF-09)— sin conocer cómo se persiste.
    /// </summary>
    public interface IProfileSaver
    {
        void SaveActive();
    }
}
