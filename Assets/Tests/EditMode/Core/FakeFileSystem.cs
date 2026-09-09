using System.Collections.Generic;
using System.Linq;


namespace Game.Core.Tests
{
    /// <summary>
    /// Sistema de archivos en memoria. Evita tocar disco real en las pruebas de <see cref="SaveStore"/>
    /// y, sobre todo, permite declarar una ruta como no escribible, que es el escenario de INC-34.
    /// </summary>
    public class FakeFileSystem : IFileSystem
    {
        private readonly Dictionary<string, string> _files = new Dictionary<string, string>();
        private readonly HashSet<string> _prepared = new HashSet<string>();

        /// <summary>Rutas que se comportan como no escribibles, sea cual sea el intento.</summary>
        public HashSet<string> ReadOnlyDirectories { get; } = new HashSet<string>();

        public IReadOnlyDictionary<string, string> Files => _files;

        public bool TryPrepareDirectory(string directory)
        {
            if (ReadOnlyDirectories.Contains(directory))
            {
                return false;
            }

            _prepared.Add(directory);
            return true;
        }

        public void WriteAllText(string path, string contents) => _files[path] = contents;

        public string ReadAllText(string path) => _files[path];

        public bool FileExists(string path) => _files.ContainsKey(path);

        /// <summary>
        /// Borrar dentro de una carpeta no escribible falla y deja el archivo donde estaba: es el
        /// escenario que hace posible probar el borrado parcial de RNF-11.
        /// </summary>
        public bool DeleteFile(string path)
        {
            if (ReadOnlyDirectories.Any(directory => path.StartsWith(directory + "/")))
            {
                return false;
            }

            _files.Remove(path);
            return true;
        }

        public string[] GetFiles(string directory, string extension) => _files.Keys
            .Where(path => path.StartsWith(directory + "/") && path.EndsWith(extension))
            .OrderBy(path => path)
            .ToArray();
    }
}
