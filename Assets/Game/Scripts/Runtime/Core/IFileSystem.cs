namespace Game.Core
{
    /// <summary>
    /// Acceso a disco de <see cref="SaveStore"/>. Existe para que el guardado se pruebe en
    /// EditMode sin tocar disco real y, sobre todo, para poder simular la carpeta no escribible
    /// que exige INC-34.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>Crea la carpeta si hace falta. Devuelve si quedó lista para escribir en ella.</summary>
        bool TryPrepareDirectory(string directory);

        void WriteAllText(string path, string contents);

        string ReadAllText(string path);

        bool FileExists(string path);

        /// <summary>Rutas de los archivos de la carpeta con esa extensión, sin recorrer subcarpetas.</summary>
        string[] GetFiles(string directory, string extension);
    }
}
