namespace Game.Core
{
    /// <summary>
    /// La sesión de juego en curso: guarda el progreso del perfil activo cuando hace falta
    /// (RF-04 al confirmar fase, RF-09 al salir). El perfil activo lo decide <see cref="GameFlow"/>
    /// —es su <see cref="GameFlow.ActiveProfile"/>—; esta clase solo lo persiste.
    /// </summary>
    public class ProfileSession : IProfileSaver
    {
        private readonly GameFlow _flow;
        private readonly SaveStore _store;

        public ProfileSession(GameFlow flow, SaveStore store)
        {
            _flow = flow;
            _store = store;
        }

        /// <summary>
        /// Guarda el perfil activo. Sin perfil activo no hace nada: antes de que el estudiante
        /// elija uno (T06) no hay progreso que perder al salir.
        /// </summary>
        public void SaveActive()
        {
            if (_flow.ActiveProfile != null)
            {
                _store.Save(_flow.ActiveProfile);
            }
        }
    }
}
