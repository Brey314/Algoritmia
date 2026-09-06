using System;
using System.IO;
using System.Linq;

namespace Game.Core
{
    /// <summary>
    /// Implementación real de <see cref="IFileSystem"/> sobre <c>System.IO</c>. La usan las
    /// escenas; las pruebas inyectan un doble en memoria.
    /// </summary>
    public class DiskFileSystem : IFileSystem
    {
        public bool TryPrepareDirectory(string directory)
        {
            try
            {
                Directory.CreateDirectory(directory);

                // Que la carpeta exista no basta (INC-34): en los equipos del colegio puede no
                // ser escribible. Se comprueba con un archivo de sonda en vez de asumirlo.
                var probe = $"{directory}/.probe";
                File.WriteAllText(probe, string.Empty);
                File.Delete(probe);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

        public string ReadAllText(string path) => File.ReadAllText(path);

        public bool FileExists(string path) => File.Exists(path);

        public string[] GetFiles(string directory, string extension)
        {
            if (!Directory.Exists(directory))
            {
                return Array.Empty<string>();
            }

            // SaveStore separa el nombre buscando la última '/', así que las rutas se devuelven
            // con barras hacia delante aunque Windows las dé con barra invertida.
            return Directory.GetFiles(directory, $"*{extension}")
                .Select(path => path.Replace('\\', '/'))
                .ToArray();
        }
    }
}
