using System;
using System.Linq;
using Game.Core;
using Game.Core.Tests;
using NUnit.Framework;

namespace Game.Reporting.Tests
{
    public class ProfileRepositoryTests
    {
        private const string PortableRoot = "C:/Juego/Datos";
        private const string FallbackRoot = "C:/Usuario/AppData/Juego";

        private FakeFileSystem _fileSystem;

        [SetUp]
        public void SetUp() => _fileSystem = new FakeFileSystem();

        private ProfileRepository CreateRepository() =>
            new ProfileRepository(_fileSystem, PortableRoot, FallbackRoot);

        private void Guardar(string root, string profileName)
        {
            var profile = PlayerProfile.Create(profileName, Array.Empty<string>()).Profile;
            _fileSystem.WriteAllText($"{root}/{profileName}.json", UnityEngine.JsonUtility.ToJson(profile));
        }

        [Test]
        public void ProfileRepository_RF46_EnumeraLosPerfilesDeLasDosRutasSinDuplicar()
        {
            Guardar(PortableRoot, "Ana");
            Guardar(PortableRoot, "Beto");
            Guardar(FallbackRoot, "Beto"); // El mismo perfil quedó también en la ruta de respaldo (INC-34).
            Guardar(FallbackRoot, "Caro");

            var perfiles = CreateRepository().AllProfiles();

            Assert.That(perfiles.Count, Is.EqualTo(3));
            Assert.That(perfiles.Select(p => p.Name), Is.EquivalentTo(new[] { "Ana", "Beto", "Caro" }));
        }

        [Test]
        public void ProfileRepository_CU11_SinPerfilesDevuelveListaVaciaYNoLanza()
        {
            Assert.That(() => CreateRepository().AllProfiles(), Throws.Nothing);
            Assert.That(CreateRepository().AllProfiles(), Is.Empty);
        }

        [Test]
        public void ProfileRepository_RNF13_UnArchivoCorruptoNoTumbaLaEnumeracion()
        {
            Guardar(PortableRoot, "Ana");
            _fileSystem.WriteAllText($"{PortableRoot}/Corrupto.json", "{ esto no es json ");

            var perfiles = CreateRepository().AllProfiles();

            Assert.That(perfiles.Count, Is.EqualTo(1));
            Assert.That(perfiles[0].Name, Is.EqualTo("Ana"));
        }
    }
}
