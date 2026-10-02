using System.IO;
using NUnit.Framework;

namespace Game.EditorTools.Tests
{
    /// <summary>Los avisos de licencia de terceros acompañan al ejecutable distribuido.</summary>
    public class LicenseNoticesTests
    {
        private string root;

        [SetUp]
        public void SetUp() =>
            root = Path.Combine(Path.GetTempPath(), "licenses-" + Path.GetRandomFileName());

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }

        private string Project() => Path.Combine(root, "project");
        private string Build() => Path.Combine(root, "build");

        private void CrearFuentes(params string[] omitir)
        {
            foreach (var relative in LicenseNotices.Sources)
            {
                if (System.Array.IndexOf(omitir, relative) >= 0) continue;
                var path = Path.Combine(Project(), relative);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, "aviso " + Path.GetFileName(path));
            }
        }

        [Test]
        public void LicenseNotices_RNF23_ElEjecutableLlevaLasLicenciasDeTerceros()
        {
            CrearFuentes();
            Directory.CreateDirectory(Build());

            LicenseNotices.CopyTo(Build(), Project());

            foreach (var relative in LicenseNotices.Sources)
            {
                var name = Path.GetFileName(relative);
                var copia = Path.Combine(Build(), LicenseNotices.FolderName, name);
                Assert.That(File.Exists(copia), $"falta {name} junto al ejecutable");
                Assert.That(File.ReadAllText(copia), Is.EqualTo("aviso " + name));
            }
        }

        [Test]
        public void LicenseNotices_RNF23_UnBuildRepetidoSobrescribeLosAvisos()
        {
            CrearFuentes();
            LicenseNotices.CopyTo(Build(), Project());

            Assert.DoesNotThrow(() => LicenseNotices.CopyTo(Build(), Project()));
        }

        [Test]
        public void LicenseNotices_RNF23_SiFaltaUnAvisoElBuildFalla()
        {
            CrearFuentes(omitir: LicenseNotices.Sources[1]);

            Assert.That(() => LicenseNotices.CopyTo(Build(), Project()), Throws.InstanceOf<IOException>());
        }

        [Test]
        public void LicenseNotices_RNF23_LosAvisosDelProyectoExistenDeVerdad()
        {
            var projectRoot = Path.GetDirectoryName(UnityEngine.Application.dataPath);
            foreach (var relative in LicenseNotices.Sources)
                Assert.That(File.Exists(Path.Combine(projectRoot, relative)), relative);
        }
    }
}
