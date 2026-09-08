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

        [Test]
        public void SaveStore_RF47_BorraElPerfilDeLasDosRutas()
        {
            // Un perfil puede haber quedado en las dos carpetas: escrito en «Datos/» un día y en
            // la de respaldo otro, cuando «Datos/» no era escribible (INC-34). Borrar solo la
            // activa dejaría media copia viva, que es exactamente el residuo que RNF-11 prohíbe.
            var sut = CreateStore();
            sut.Save(ProfileWithProgress());
            _fileSystem.WriteAllText($"{FallbackRoot}/Ana.json", "{}");

            var deleted = sut.Delete("Ana");

            Assert.That(deleted, Is.True);
            Assert.That(_fileSystem.Files.Keys, Has.None.Contains("Ana"));
        }

        [Test]
        public void SaveStore_RF47_NoAfectaAOtrosPerfiles()
        {
            var sut = CreateStore();
            foreach (var name in new[] { "Ana", "Beto", "Caro" })
            {
                sut.Save(PlayerProfile.Create(name, Array.Empty<string>()).Profile);
            }

            sut.Delete("Beto");

            Assert.That(sut.ProfileNames(), Is.EquivalentTo(new[] { "Ana", "Caro" }));
            Assert.That(sut.Load("Ana").Name, Is.EqualTo("Ana"));
            Assert.That(sut.Load("Caro").Name, Is.EqualTo("Caro"));
        }

        [Test]
        public void SaveStore_INC34_BorraDesdeLaRutaDeRespaldoAunqueDatosNoSeaEscribible()
        {
            _fileSystem.ReadOnlyDirectories.Add(PortableRoot);
            var sut = CreateStore();
            sut.Save(ProfileWithProgress());

            var deleted = sut.Delete("Ana");

            Assert.That(deleted, Is.True);
            Assert.That(_fileSystem.Files, Is.Empty);
        }

        [Test]
        public void SaveStore_RNF11_UnBorradoParcialNoSeReportaComoExito()
        {
            // «Sin residuos» no admite mejor esfuerzo: si queda una copia en la ruta que no se
            // pudo tocar, el borrado no fue un borrado y quien llama tiene que enterarse.
            _fileSystem.WriteAllText($"{PortableRoot}/Ana.json", "{}");
            _fileSystem.ReadOnlyDirectories.Add(PortableRoot);
            var sut = CreateStore();
            sut.Save(ProfileWithProgress());

            var deleted = sut.Delete("Ana");

            Assert.That(deleted, Is.False);
            Assert.That(_fileSystem.Files.Keys, Is.EqualTo(new[] { $"{PortableRoot}/Ana.json" }));
        }

    }
}
