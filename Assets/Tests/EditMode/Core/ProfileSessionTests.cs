using System;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class ProfileSessionTests
    {
        private const string PortableRoot = "C:/Juego/Datos";
        private const string FallbackRoot = "C:/Usuario/AppData/Juego";

        private FakeFileSystem _fileSystem;
        private GameFlow _flow;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new FakeFileSystem();
            _flow = new GameFlow();
        }

        private ProfileSession CreateSession() =>
            new ProfileSession(_flow, new SaveStore(_fileSystem, PortableRoot, FallbackRoot));

        private void SelectProfile(string profileName)
        {
            _flow.TryGoTo(GameState.MainMenu);
            _flow.TryGoTo(GameState.ProfileSelect);
            _flow.TrySelectProfile(PlayerProfile.Create(profileName, Array.Empty<string>()).Profile);
        }

        [Test]
        public void ProfileSession_RF09_SinPerfilActivoNoEscribeNingunPerfil()
        {
            var sut = CreateSession();

            sut.SaveActive();

            Assert.That(_fileSystem.Files, Is.Empty);
        }

        [Test]
        public void ProfileSession_RF09_ConPerfilActivoGuardaEsePerfil()
        {
            SelectProfile("Ana");
            var sut = CreateSession();

            sut.SaveActive();

            Assert.That(_fileSystem.Files.Keys, Has.Exactly(1).Contains("Ana"));
        }

        [Test]
        public void ProfileSession_RF02_ListaLosNombresDeLosPerfilesGuardados()
        {
            var sut = CreateSession();
            sut.Save(PlayerProfile.Create("Ana", Array.Empty<string>()).Profile);
            sut.Save(PlayerProfile.Create("Beto", Array.Empty<string>()).Profile);

            Assert.That(sut.ExistingProfileNames(), Is.EquivalentTo(new[] { "Ana", "Beto" }));
        }

        [Test]
        public void ProfileSession_RF02_CreateRechazaUnNombreYaGuardado()
        {
            var sut = CreateSession();
            sut.Save(PlayerProfile.Create("Ana", Array.Empty<string>()).Profile);

            var result = sut.Create("Ana");

            Assert.That(result.Result, Is.EqualTo(ProfileCreationResult.Status.DuplicateName));
        }

        [Test]
        public void ProfileSession_RF03_LoadDevuelveElPerfilConSuProgreso()
        {
            var sut = CreateSession();
            var saved = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            saved.Reach(LevelId.Wheel);
            sut.Save(saved);

            Assert.That(sut.Load("Ana").ReachedLevel, Is.EqualTo(LevelId.Wheel));
        }

        [Test]
        public void ProfileSession_RF47_BorraElPerfilYDejaDeListarlo()
        {
            var sut = CreateSession();
            sut.Save(PlayerProfile.Create("Ana", Array.Empty<string>()).Profile);
            sut.Save(PlayerProfile.Create("Beto", Array.Empty<string>()).Profile);

            Assert.That(sut.Delete("Ana"), Is.True);
            Assert.That(sut.ExistingProfileNames(), Is.EquivalentTo(new[] { "Beto" }));
        }

        [Test]
        public void ProfileSession_RNF11_BorrarElPerfilActivoImpideQueSalirLoVuelvaAEscribir()
        {
            // Sin esto el borrado se deshace solo: «Salir» persiste el perfil activo (RF-09) y
            // volvería a crear el archivo que se acaba de eliminar. El residuo no es tolerable.
            SelectProfile("Ana");
            var sut = CreateSession();
            sut.Save(_flow.ActiveProfile);

            sut.Delete("Ana");
            sut.SaveActive();

            Assert.That(_fileSystem.Files, Is.Empty);
        }

    }
}
