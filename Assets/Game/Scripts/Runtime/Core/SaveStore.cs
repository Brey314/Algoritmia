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

        /// <summary>
        /// Las dos carpetas donde puede haber quedado un perfil, no solo la activa: un mismo
        /// perfil pudo escribirse en «Datos/» un día y en la de respaldo otro (INC-34), y borrar
        /// una sola dejaría media copia viva.
        /// </summary>
        private readonly string[] _roots;

        public SaveStore(IFileSystem fileSystem, string portableRoot, string fallbackRoot)
        {
            _fileSystem = fileSystem;
            _roots = new[] { portableRoot, fallbackRoot };

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

        /// <summary>
        /// Borra el perfil de las dos rutas. Devuelve si no quedó rastro de él en ninguna: un
        /// borrado a medias es un residuo, y RNF-11 exige ausencia, no mejor esfuerzo (RF-47).
        /// Es irreversible por diseño — no hay papelera ni copia de seguridad.
        /// </summary>
        public bool Delete(string profileName)
        {
            foreach (var path in _roots.Select(root => $"{root}/{profileName}{Extension}"))
            {
                if (_fileSystem.FileExists(path))
                {
                    _fileSystem.DeleteFile(path);
                }
            }

            return !_roots.Any(root =>
                _fileSystem.FileExists($"{root}/{profileName}{Extension}"));
        }

        private string PathOf(string profileName) => $"{ActiveDirectory}/{profileName}{Extension}";
    }
}
