using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class SaveStoreTests
    {
        private const string PortableRoot = "C:/Juego/Datos";
        private const string FallbackRoot = "C:/Usuario/AppData/Juego";

        private FakeFileSystem _fileSystem;

        [SetUp]
        public void SetUp() => _fileSystem = new FakeFileSystem();

        private SaveStore CreateStore() => new SaveStore(_fileSystem, PortableRoot, FallbackRoot);

        private static PlayerProfile ProfileWithProgress()
        {
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            profile.ConfirmPhase(LevelId.Fire, 1, new PerformanceIndicators(4, 2, 3, 91.5f));
            profile.Reach(LevelId.Wheel);
            return profile;
        }

        [Test]
        public void SaveStore_RF04_GuardaYRecuperaElPerfilCompleto()
        {
            var sut = CreateStore();
            var expected = ProfileWithProgress();

            sut.Save(expected);
            var actual = sut.Load("Ana");

            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.ReachedLevel, Is.EqualTo(LevelId.Wheel));
            Assert.That(actual.IsPhaseConfirmed(LevelId.Fire, 1), Is.True);
            Assert.That(actual.IndicatorsFor(LevelId.Fire, 1),
                Is.EqualTo(new PerformanceIndicators(4, 2, 3, 91.5f)));
        }

        [Test]
        public void SaveStore_RNF09_NoPersisteCampoAlgunoFueraDeLaListaCerrada()
        {
            // OE1 §3.6.1 nota 5: nombre o alias, progreso de avance —nivel alcanzado y fases
            // confirmadas— y los cuatro indicadores. Nada más: sin puntaje (CP-03), sin fecha,
            // sin identificador de equipo, sin datos de contacto.
            var sut = CreateStore();
            sut.Save(ProfileWithProgress());

            var keys = Regex.Matches(_fileSystem.Files.Values.Single(), "\"([A-Za-z]+)\":")
                .Cast<Match>()
                .Select(match => match.Groups[1].Value)
                .Distinct();

            Assert.That(keys, Is.EquivalentTo(new[]
            {
                "name", "reachedLevel", "phases",
                "level", "phase",
                "attempts", "correctedErrors", "stepsUsed", "resolutionSeconds"
            }));
        }

        [Test]
        public void SaveStore_INC34_CaeALaRutaDeRespaldoSiDatosNoEsEscribible()
        {
            _fileSystem.ReadOnlyDirectories.Add(PortableRoot);

            var sut = CreateStore();
            sut.Save(ProfileWithProgress());

            Assert.That(sut.UsingFallback, Is.True);
            Assert.That(sut.ActiveDirectory, Is.EqualTo(FallbackRoot));
            Assert.That(_fileSystem.Files.Keys.Single(), Does.StartWith(FallbackRoot));
        }

        [Test]
        public void SaveStore_RNF07_EscribeDentroDeDatosJuntoAlEjecutableCuandoSePuede()
        {
            var sut = CreateStore();
            sut.Save(ProfileWithProgress());

            Assert.That(sut.UsingFallback, Is.False);
            Assert.That(sut.ActiveDirectory, Is.EqualTo(PortableRoot));
            Assert.That(_fileSystem.Files.Keys.Single(), Is.EqualTo($"{PortableRoot}/Ana.json"));
        }

        [Test]
        public void SaveStore_RF02_ListaLosPerfilesExistentesParaDetectarDuplicados()
        {
            var sut = CreateStore();
            sut.Save(PlayerProfile.Create("Ana", Array.Empty<string>()).Profile);
            sut.Save(PlayerProfile.Create("Bruno", new[] { "Ana" }).Profile);

            Assert.That(sut.ProfileNames(), Is.EquivalentTo(new[] { "Ana", "Bruno" }));
        }
    }
}
