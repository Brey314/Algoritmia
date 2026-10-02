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

        [Test]
        public void DiskFileSystem_RNF14_ReemplazarUnPerfilNoDejaTemporalesNiMezclaContenido()
        {
            var sut = new DiskFileSystem();
            Assume.That(sut.TryPrepareDirectory(_directory), Is.True);
            var path = $"{_directory}/Ana.json";
            sut.WriteAllText(path, "{\"name\":\"Ana\",\"largo\":\"muchos-caracteres-del-guardado-anterior\"}");

            sut.WriteAllText(path, "{\"name\":\"Ana\"}");

            Assert.That(sut.ReadAllText(path), Is.EqualTo("{\"name\":\"Ana\"}"), "contenido nuevo, sin restos del viejo");
            Assert.That(Directory.GetFiles(_directory).Select(Path.GetFileName),
                Is.EquivalentTo(new[] { "Ana.json" }), "ningún temporal queda junto al perfil");
        }

        [Test]
        public void DiskFileSystem_RNF14_ConElPerfilAbiertoPorOtroProcesoElGuardadoNoFallaNiDejaTemporal()
        {
            var sut = new DiskFileSystem();
            Assume.That(sut.TryPrepareDirectory(_directory), Is.True);
            var path = $"{_directory}/Ana.json";
            sut.WriteAllText(path, "{\"name\":\"Ana\"}");

            // Un antivirus o un indexador con el JSON abierto sin FILE_SHARE_DELETE: File.Replace
            // no puede quitarlo, y el guardado no debe perderse por eso.
            using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Assert.That(() => sut.WriteAllText(path, "{\"name\":\"Ana\",\"nuevo\":1}"), Throws.Nothing);
            }

            Assert.That(sut.ReadAllText(path), Is.EqualTo("{\"name\":\"Ana\",\"nuevo\":1}"));
            Assert.That(Directory.GetFiles(_directory).Select(Path.GetFileName),
                Is.EquivalentTo(new[] { "Ana.json" }), "ningún temporal queda junto al perfil");
        }

        [Test]
        public void DiskFileSystem_RNF14_ConElTemporalRetenidoPorOtroProcesoElGuardadoNoFalla()
        {
            var sut = new DiskFileSystem();
            Assume.That(sut.TryPrepareDirectory(_directory), Is.True);
            var path = $"{_directory}/Ana.json";
            sut.WriteAllText(path, "{\"name\":\"Ana\"}");
            File.WriteAllText(path + ".tmp", "x");

            // El mismo escáner de B1, mirando el temporal recién cerrado: sin FILE_SHARE_DELETE
            // File.Replace falla y, tras copiar, tampoco se puede borrar. El perfil ya quedó
            // guardado, así que no debe lanzar ni se asegura que el temporal desaparezca.
            using (File.Open(path + ".tmp", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Assert.That(() => sut.WriteAllText(path, "{\"name\":\"Ana\",\"nuevo\":1}"), Throws.Nothing);
                Assert.That(sut.ReadAllText(path), Is.EqualTo("{\"name\":\"Ana\",\"nuevo\":1}"));
            }
        }

        [Test]
        public void DiskFileSystem_RNF14_SiLaEscrituraFallaElPerfilAnteriorQuedaIntacto()
        {
            var sut = new DiskFileSystem();
            Assume.That(sut.TryPrepareDirectory(_directory), Is.True);
            var path = $"{_directory}/Ana.json";
            sut.WriteAllText(path, "{\"name\":\"Ana\"}");

            // Una carpeta con el nombre del temporal impide escribirlo: simula el fallo a mitad
            // de guardado sin depender de matar el proceso.
            Directory.CreateDirectory(path + ".tmp");

            Assert.That(() => sut.WriteAllText(path, "{\"name\":\"Ana\",\"nuevo\":1}"), Throws.Exception);
            Assert.That(sut.ReadAllText(path), Is.EqualTo("{\"name\":\"Ana\"}"));
        }

        [Test]
        public void DiskFileSystem_RNF11_BorrarUnPerfilBorraTambienUnTemporalHuerfano()
        {
            var sut = new DiskFileSystem();
            Assume.That(sut.TryPrepareDirectory(_directory), Is.True);
            var path = $"{_directory}/Ana.json";
            sut.WriteAllText(path, "{}");
            File.WriteAllText(path + ".tmp", "{\"trunca");

            var deleted = sut.DeleteFile(path);

            Assert.That(deleted, Is.True);
            Assert.That(Directory.GetFiles(_directory), Is.Empty);
        }
    }
}
