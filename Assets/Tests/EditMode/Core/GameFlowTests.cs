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
            var perfil = NewProfile();
            perfil.Reach(LevelId.Wheel);
            _sut.TryGoTo(GameState.MainMenu);
            _sut.TryGoTo(GameState.ProfileSelect);
            _sut.TrySelectProfile(perfil);

            _sut.TryStartNarrative("n1_intro");
            Assert.That(_sut.Current, Is.EqualTo(GameState.Narrative));
            Assert.That(_sut.NarrativeSequenceId, Is.EqualTo("n1_intro"));

            // El Nivel 2 es el que tiene una fase 2: el Nivel 1 se resuelve en una sola fase, y
            // pedirle una segunda se rechaza (PhaseId).
            _sut.TryStartPlaying(LevelId.Wheel, phase: 2);
            Assert.That(_sut.Current, Is.EqualTo(GameState.Playing));
            Assert.That(_sut.PlayingLevel, Is.EqualTo(LevelId.Wheel));
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

        [Test]
        public void GameFlow_RF05_DosEscenasNarrativasSeEncadenanSinPasarPorElMenu()
        {
            _sut.TryGoTo(GameState.MainMenu);
            _sut.TryGoTo(GameState.ProfileSelect);
            _sut.TrySelectProfile(NewProfile());
            _sut.TryStartNarrative("N2_Escena22_ElPatron");

            // La 2.2 y la 2.3 van seguidas en el guion: el asset declara la siguiente y el flujo
            // la acepta como otra entrada a Narrative, con el id nuevo.
            Assert.That(_sut.TryStartNarrative("N2_Escena23_Construccion"), Is.True);
            Assert.That(_sut.Current, Is.EqualTo(GameState.Narrative));
            Assert.That(_sut.NarrativeSequenceId, Is.EqualTo("N2_Escena23_Construccion"));
        }

        [Test]
        public void GameFlow_RNF14_EntrarAUnaFaseYaConfirmadaRetomaEnLaPrimeraPendiente()
        {
            var perfil = NewProfile();
            perfil.Reach(LevelId.Wheel);
            perfil.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), default);
            _sut.TryGoTo(GameState.MainMenu);
            _sut.TryGoTo(GameState.ProfileSelect);
            _sut.TrySelectProfile(perfil);

            // La apertura del nivel siempre pide la fase 1; con la 1 en disco se retoma en la 2,
            // que es lo que RNF-14 llama «desde la última fase confirmada».
            Assert.That(_sut.TryStartPlaying(LevelId.Wheel, 1), Is.True);
            Assert.That(_sut.PlayingPhase, Is.EqualTo(2), "retoma en la primera fase pendiente");

            // Reiniciar la fase en curso, que no está confirmada, la repite tal cual (RF-07).
            Assert.That(_sut.TryStartPlaying(LevelId.Wheel, 2), Is.True);
            Assert.That(_sut.PlayingPhase, Is.EqualTo(2));

            // Con el nivel completo no hay pendiente: se juega la pedida, repetir es legítimo.
            perfil.ConfirmPhase(new PhaseId(LevelId.Wheel, 2), default);
            perfil.ConfirmPhase(new PhaseId(LevelId.Wheel, 3), default);
            Assert.That(_sut.TryStartPlaying(LevelId.Wheel, 1), Is.True);
            Assert.That(_sut.PlayingPhase, Is.EqualTo(1));

            // Una fase que el nivel no tiene se rechaza sin cambiar de estado.
            Assert.That(_sut.TryStartPlaying(LevelId.Wheel, 4), Is.False);
        }

        [Test]
        public void GameFlow_RNF14_SiLaFasePendienteNoEsJugableTodaviaSeJuegaLaPedida()
        {
            // Visto el 12/09/2026: con la 1 y la 2 del Nivel 2 en disco, terminar la escena 2.1
            // saltaba a la fase 3 —que no tiene escena hasta W13— y el estudiante caía al menú
            // «sin continuar». La retoma solo salta a una fase que se pueda jugar.
            var perfil = NewProfile();
            perfil.Reach(LevelId.Wheel);
            perfil.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), default);
            perfil.ConfirmPhase(new PhaseId(LevelId.Wheel, 2), default);
            _sut.TryGoTo(GameState.MainMenu);
            _sut.TryGoTo(GameState.ProfileSelect);
            _sut.TrySelectProfile(perfil);

            Assert.That(_sut.TryStartPlaying(LevelId.Wheel, 1, fase => fase.Phase <= 2), Is.True);
            Assert.That(_sut.PlayingPhase, Is.EqualTo(1), "la pendiente no es jugable: se repite la pedida");

            Assert.That(_sut.TryStartPlaying(LevelId.Wheel, 1, fase => true), Is.True);
            Assert.That(_sut.PlayingPhase, Is.EqualTo(3), "y cuando exista, se retoma en ella");
        }
    }
}
