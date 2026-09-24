using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core;
using UnityEngine;

namespace Game.Reporting
{
    /// <summary>
    /// Enumera los perfiles guardados en el equipo, leyendo <c>Datos/</c> y la ruta de respaldo
    /// (RF-46, RF-02, CU-11). Lógica pura contra <see cref="IFileSystem"/>, el mismo que usa
    /// <see cref="SaveStore"/> desde el Slice 1.
    /// </summary>
    public class ProfileRepository
    {
        private const string Extension = ".json";

        private readonly IFileSystem _fileSystem;
        private readonly string[] _roots;

        public ProfileRepository(IFileSystem fileSystem, string portableRoot, string fallbackRoot)
        {
            _fileSystem = fileSystem;
            _roots = new[] { portableRoot, fallbackRoot };
        }

        /// <summary>
        /// Los perfiles de las dos rutas, sin duplicar uno que exista en ambas (INC-34). Un
        /// archivo corrupto o ilegible se omite con una advertencia: no tumba la enumeración
        /// (RNF-13, sin estados irrecuperables). Sin perfiles, devuelve una lista vacía.
        /// </summary>
        public IReadOnlyList<PlayerProfile> AllProfiles()
        {
            var byName = new Dictionary<string, PlayerProfile>(StringComparer.OrdinalIgnoreCase);
            foreach (var root in _roots)
            {
                foreach (var path in _fileSystem.GetFiles(root, Extension))
                {
                    var name = NameFrom(path);
                    if (byName.ContainsKey(name))
                    {
                        continue; // Ya se leyó en la otra ruta (INC-34): no se duplica.
                    }

                    var profile = TryRead(path);
                    if (profile != null)
                    {
                        byName[name] = profile;
                    }
                }
            }

            return byName.Values.ToArray();
        }

        private PlayerProfile TryRead(string path)
        {
            try
            {
                return JsonUtility.FromJson<PlayerProfile>(_fileSystem.ReadAllText(path));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"«ProfileRepository» no pudo leer {path}: {e.Message}");
                return null;
            }
        }

        private static string NameFrom(string path)
        {
            var file = path.Substring(path.LastIndexOf('/') + 1);
            return file.Substring(0, file.Length - Extension.Length);
        }
    }
}
