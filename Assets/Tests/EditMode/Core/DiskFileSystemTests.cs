using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class DiskFileSystemTests
    {
        private string _directory;

        [SetUp]
        public void SetUp() =>
            _directory = Path.Combine(Path.GetTempPath(), "AlgoritmDiskFs_" + Guid.NewGuid().ToString("N"))
                .Replace('\\', '/');

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, recursive: true);
            }
        }

        [Test]
        public void DiskFileSystem_RNF07_EscribeYReleeElContenidoEnLaRutaIndicada()
        {
            var sut = new DiskFileSystem();
            Assume.That(sut.TryPrepareDirectory(_directory), Is.True);
            var path = $"{_directory}/Ana.json";

            sut.WriteAllText(path, "{\"name\":\"Ana\"}");

            Assert.That(sut.ReadAllText(path), Is.EqualTo("{\"name\":\"Ana\"}"));
        }

        [Test]
        public void DiskFileSystem_RF02_ListaLosArchivosDeLaCarpetaConLaExtensionYRutaEnBarras()
        {
            var sut = new DiskFileSystem();
            Assume.That(sut.TryPrepareDirectory(_directory), Is.True);
            sut.WriteAllText($"{_directory}/Ana.json", "{}");
            sut.WriteAllText($"{_directory}/Beto.json", "{}");
            sut.WriteAllText($"{_directory}/notas.txt", "x");

            var found = sut.GetFiles(_directory, ".json");

            Assert.That(found, Is.EquivalentTo(new[] { $"{_directory}/Ana.json", $"{_directory}/Beto.json" }));
        }
    }
}
