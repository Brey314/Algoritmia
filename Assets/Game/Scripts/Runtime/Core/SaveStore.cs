using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// Guardado local del perfil: un archivo JSON por perfil (RF-04, RNF-14).
    /// </summary>
    /// <remarks>
    /// La carpeta buena es <c>Datos/</c> junto al ejecutable y no
    /// <c>Application.persistentDataPath</c>: esa escribe en <c>%AppData%\LocalLow</c>, fuera de
    /// la carpeta portable, y entonces «sin instalación» (RNF-07) y «sin residuos» (RNF-11)
    /// dejarían de significar lo mismo. La ruta del sistema queda solo como respaldo para el
    /// equipo donde la carpeta del ejecutable no sea escribible (INC-34), y en ese caso el
    /// almacén lo expone para poder advertir al docente.
    /// </remarks>
    public class SaveStore
    {
        private const string Extension = ".json";

        private readonly IFileSystem _fileSystem;

        public SaveStore(IFileSystem fileSystem, string portableRoot, string fallbackRoot)
        {
            _fileSystem = fileSystem;

            if (fileSystem.TryPrepareDirectory(portableRoot))
            {
                ActiveDirectory = portableRoot;
                return;
            }

            ActiveDirectory = fallbackRoot;
            UsingFallback = true;
            fileSystem.TryPrepareDirectory(fallbackRoot);
        }

        /// <summary>Carpeta en la que se está guardando de verdad.</summary>
        public string ActiveDirectory { get; }

        /// <summary>Si se cayó a la ruta de respaldo, para poder advertirlo (INC-34).</summary>
        public bool UsingFallback { get; }

        public void Save(PlayerProfile profile) =>
            _fileSystem.WriteAllText(PathOf(profile.Name), JsonUtility.ToJson(profile));

        public PlayerProfile Load(string profileName) =>
            JsonUtility.FromJson<PlayerProfile>(_fileSystem.ReadAllText(PathOf(profileName)));

        public bool Exists(string profileName) => _fileSystem.FileExists(PathOf(profileName));

        /// <summary>Nombres de los perfiles guardados, para poder detectar duplicados (RF-02).</summary>
        public IReadOnlyList<string> ProfileNames() => _fileSystem
            .GetFiles(ActiveDirectory, Extension)
            .Select(path => path.Substring(path.LastIndexOf('/') + 1))
            .Select(file => file.Substring(0, file.Length - Extension.Length))
            .ToArray();

        private string PathOf(string profileName) => $"{ActiveDirectory}/{profileName}{Extension}";
    }
}
