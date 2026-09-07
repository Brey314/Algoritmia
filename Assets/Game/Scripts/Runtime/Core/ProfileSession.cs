using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>
    /// La sesión de juego en curso: la fachada de perfiles del <c>sistema-navegacion</c>. Lista los
    /// perfiles guardados, los carga, crea uno nuevo validado y persiste el progreso del activo
    /// (RF-02, RF-04, RF-09). El perfil activo lo decide <see cref="GameFlow"/> —es su
    /// <see cref="GameFlow.ActiveProfile"/>—; esta clase solo lo lee y lo guarda.
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

        /// <summary>Nombres de los perfiles ya guardados, para pintar la lista y detectar duplicados (RF-02).</summary>
        public IReadOnlyList<string> ExistingProfileNames() => _store.ProfileNames();

        /// <summary>Carga un perfil guardado con todo su progreso (RF-03, CU-01).</summary>
        public PlayerProfile Load(string profileName) => _store.Load(profileName);

        /// <summary>
        /// Crea un perfil validando el nombre contra los ya existentes. Devuelve un resultado
        /// tipado: nombre vacío o duplicado son flujos alternos de HU-01, no fallos (FA-01, FA-02).
        /// No lo activa ni lo guarda: de eso se encargan <see cref="GameFlow.TrySelectProfile"/> y
        /// <see cref="Save"/>.
        /// </summary>
        public ProfileCreationResult Create(string profileName) =>
            PlayerProfile.Create(profileName, _store.ProfileNames());

        /// <summary>Guarda un perfil en disco (RF-04).</summary>
        public void Save(PlayerProfile profile) => _store.Save(profile);

        /// <summary>
        /// Guarda el perfil activo. Sin perfil activo no hace nada: antes de que el estudiante
        /// elija uno no hay progreso que perder al salir.
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
