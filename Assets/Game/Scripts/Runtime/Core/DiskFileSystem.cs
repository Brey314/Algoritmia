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
        // No termina en la extensión de los perfiles: GetFiles("*.json") no lo lista.
        private const string TempSuffix = ".tmp";

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

        /// <summary>
        /// Escritura atómica (DEF-SPER-02): el contenido va a un archivo temporal contiguo y solo
        /// cuando está completo reemplaza al original. Un cierre o un corte de luz a mitad de
        /// guardado deja el perfil anterior intacto en vez de un JSON truncado que bloquearía todo
        /// el progreso del estudiante (RNF-14, y lo aprobado no se pierde: invariante 3).
        /// </summary>
        public void WriteAllText(string path, string contents)
        {
            var temp = path + TempSuffix;
            try
            {
                File.WriteAllText(temp, contents);
                if (File.Exists(path))
                {
                    try
                    {
                        File.Replace(temp, path, null);
                    }
                    catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                    {
                        // File.Replace exige poder borrar el perfil; un antivirus, el indexador o un
                        // cliente de sincronización con el archivo abierto sin FILE_SHARE_DELETE lo
                        // impiden. Copiar encima solo pide escritura, como el guardado de antes
                        // (rc1); el camino atómico sigue siendo el normal. El mismo escáner que
                        // retuvo el perfil puede retener el temporal recién cerrado: una vez copiado
                        // el perfil, borrarlo es de mejor esfuerzo (un .tmp huérfano no se lista
                        // con GetFiles("*.json"), el siguiente guardado lo sobrescribe y
                        // DeleteFile lo limpia, RNF-11).
                        File.Copy(temp, path, true);
                        TryDelete(temp);
                    }
                }
                else
                {
                    File.Move(temp, path);
                }
            }
            catch (Exception)
            {
                // Sin permisos para limpiar: el error que importa es el de la escritura.
                TryDelete(temp);
                throw;
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
            }
        }

        public string ReadAllText(string path) => File.ReadAllText(path);

        public bool FileExists(string path) => File.Exists(path);

        public bool DeleteFile(string path)
        {
            try
            {
                // Un temporal huérfano de un cierre a mitad de guardado también es rastro (RNF-11).
                File.Delete(path + TempSuffix);
                File.Delete(path);
                return !File.Exists(path);
            }
            catch (Exception)
            {
                // Carpeta de solo lectura, archivo abierto por otro proceso, permisos: el borrado
                // no ocurrió y hay que decirlo. RNF-11 no admite «casi borrado» (INC-34).
                return false;
            }
        }

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
