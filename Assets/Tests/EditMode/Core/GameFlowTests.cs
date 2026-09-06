using System;
using System.Linq;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class GameFlowTests
    {
        private GameFlow _sut;

        [SetUp]
        public void SetUp() => _sut = new GameFlow();

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        [Test]
        public void GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable()
        {
            Assert.That(_sut.Current, Is.EqualTo(GameState.Boot));

            Assert.That(_sut.TryGoTo(GameState.MainMenu), Is.True);
            Assert.That(_sut.TryGoTo(GameState.ProfileSelect), Is.True);
            Assert.That(_sut.TrySelectProfile(NewProfile()), Is.True);
            Assert.That(_sut.Current, Is.EqualTo(GameState.LevelSelect));
            Assert.That(_sut.TryStartNarrative("n1_intro"), Is.True);
            Assert.That(_sut.TryStartPlaying(LevelId.Fire, phase: 1), Is.True);
            Assert.That(_sut.TryGoTo(GameState.LevelSummary), Is.True);
            Assert.That(_sut.TryGoTo(GameState.LevelSelect), Is.True);
        }

        [Test]
        public void GameFlow_CP02_NoExisteEstadoDeDerrota()
        {
            // CP-02: no hay pantalla de derrota, ni límite de intentos, ni penalización. La
            // razón es pedagógica, no técnica; sin esta prueba una futura «mejora» la trae.
            var states = Enum.GetNames(typeof(GameState)).Select(state => state.ToLowerInvariant());

            Assert.That(states.Where(state => state.Contains("gameover")
                                              || state.Contains("defeat")
                                              || state.Contains("derrota")
                                              || state.Contains("lose")), Is.Empty);
        }

        [Test]
        public void GameFlow_RF03_NoPermiteEntrarANivelBloqueado()
        {
            _sut.TryGoTo(GameState.MainMenu);
            _sut.TryGoTo(GameState.ProfileSelect);
            _sut.TrySelectProfile(NewProfile()); // Perfil nuevo: solo el Nivel 1 (HU-01 FA-03).

            Assert.That(_sut.TryStartPlaying(LevelId.Wheel, 1), Is.False);
            Assert.That(_sut.Current, Is.EqualTo(GameState.LevelSelect));
            Assert.That(_sut.TryStartPlaying(LevelId.Fire, 1), Is.True);
        }

        [Test]
        public void GameFlow_RF07_UnaTransicionIlegalNoCambiaDeEstadoYSeObserva()
        {
            // No lanza y no deja el flujo roto: devuelve false y el estado sigue siendo el bueno.
            Assert.That(_sut.TryGoTo(GameState.LevelSummary), Is.False);
            Assert.That(_sut.Current, Is.EqualTo(GameState.Boot));

            Assert.That(_sut.TryGoTo(GameState.MainMenu), Is.True);
            Assert.That(_sut.Current, Is.EqualTo(GameState.MainMenu));
        }

        [Test]
        public void GameFlow_RF05_NarrativeSeParametrizaConLaSecuenciaYPlayingConNivelYFase()
        {
            _sut.TryGoTo(GameState.MainMenu);
            _sut.TryGoTo(GameState.ProfileSelect);
            _sut.TrySelectProfile(NewProfile());

            _sut.TryStartNarrative("n1_intro");
            Assert.That(_sut.Current, Is.EqualTo(GameState.Narrative));
            Assert.That(_sut.NarrativeSequenceId, Is.EqualTo("n1_intro"));

            _sut.TryStartPlaying(LevelId.Fire, phase: 2);
            Assert.That(_sut.Current, Is.EqualTo(GameState.Playing));
            Assert.That(_sut.PlayingLevel, Is.EqualTo(LevelId.Fire));
            Assert.That(_sut.PlayingPhase, Is.EqualTo(2));
        }

        [Test]
        public void GameFlow_RF08_LosCreditosSeAlcanzanDesdeElInicioYVuelvenAEl()
        {
            _sut.TryGoTo(GameState.MainMenu);

            Assert.That(_sut.TryGoTo(GameState.Credits), Is.True);
            Assert.That(_sut.TryGoTo(GameState.MainMenu), Is.True);
        }

        [Test]
        public void GameFlow_RF07_ReiniciarElNivelVuelveAPlayingSinPasarPorNingunaDerrota()
        {
            _sut.TryGoTo(GameState.MainMenu);
            _sut.TryGoTo(GameState.ProfileSelect);
            _sut.TrySelectProfile(NewProfile());
            _sut.TryStartPlaying(LevelId.Fire, 1);

            Assert.That(_sut.TryStartPlaying(LevelId.Fire, 1), Is.True);
            Assert.That(_sut.Current, Is.EqualTo(GameState.Playing));
            Assert.That(_sut.TryGoTo(GameState.MainMenu), Is.True); // Pausa → volver al inicio.
        }
    }
}
