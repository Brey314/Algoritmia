using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Core.Tests
{
    public class PauseMenuPolicyTests
    {
        [TearDown]
        public void DestruirElRunner()
        {
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(runner.gameObject);
            }
        }

        [Test]
        [Category("Acceptance")]
        public void PauseMenuPolicy_RF07_ReiniciarNoRebloqueaNiBorraIndicadores()
        {
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            var indicators = new PerformanceIndicators(attempts: 4, correctedErrors: 2, stepsUsed: 3,
                resolutionSeconds: 91.5f);
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), indicators);
            profile.Reach(LevelId.Wheel); // Nivel 2 ya desbloqueado por una convergencia previa.

            // GameFlowRunner desnudo: sin escena ni SceneLoader, así que Apply() no tiene nada que
            // recargar y esta prueba se queda en el flujo y el perfil, igual que las demás de
            // EditMode — Restart pasa por el Runner (no por GameFlow directo) precisamente para que
            // la recarga real se dispare en producción, pero aquí no hay nada que la reciba.
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            runner.Flow.TryGoTo(GameState.MainMenu);
            runner.Flow.TryGoTo(GameState.ProfileSelect);
            runner.Flow.TrySelectProfile(profile);
            runner.Flow.TryStartPlaying(LevelId.Fire, 1);

            var result = PauseMenuPolicy.Restart(runner);

            Assert.That(result, Is.True, "Restart no reinició la fase en curso");
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing));
            Assert.That(runner.Flow.PlayingLevel, Is.EqualTo(LevelId.Fire));
            Assert.That(runner.Flow.PlayingPhase, Is.EqualTo(1));
            Assert.That(profile.IsUnlocked(LevelId.Wheel), Is.True,
                "Restart re-bloqueó un nivel ya desbloqueado");
            Assert.That(profile.IsPhaseConfirmed(new PhaseId(LevelId.Fire, 1)), Is.True,
                "Restart borró una fase ya confirmada");
            Assert.That(profile.IndicatorsFor(new PhaseId(LevelId.Fire, 1)), Is.EqualTo(indicators),
                "Restart alteró los indicadores de una fase ya confirmada");
            Assert.That(profile.ConfirmedPhases.Count, Is.EqualTo(1),
                "Restart cambió el número de fases confirmadas");
        }

        // --- INC-159: «Volver a ver la historia» y «Guardar y salir» -------------------------

        private static readonly PhaseNarrative[] Table =
        {
            new PhaseNarrative(LevelId.Fire, 1, "N1_Hallazgo"),
            new PhaseNarrative(LevelId.Wheel, 2, "N2_Escena23_Construccion")
        };

        private static GameFlowRunner RunnerPlaying(LevelId level, int phase)
        {
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            profile.Reach(level);
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            runner.Flow.TryGoTo(GameState.MainMenu);
            runner.Flow.TryGoTo(GameState.ProfileSelect);
            runner.Flow.TrySelectProfile(profile);
            runner.Flow.TryStartPlaying(level, phase);
            return runner;
        }

        [Test]
        public void PauseMenuPolicy_INC159_LaFaseEnCursoPresentaLaEscenaDeSuFila()
        {
            var runner = RunnerPlaying(LevelId.Wheel, 2);

            Assert.That(PauseMenuPolicy.StoryFor(runner.Flow, Table), Is.EqualTo("N2_Escena23_Construccion"));
        }

        [Test]
        public void PauseMenuPolicy_INC159_UnaFaseSinFilaNoOfreceHistoria()
        {
            var runner = RunnerPlaying(LevelId.Wheel, 1); // la tabla solo tiene la fase 2 del Nivel 2

            Assert.That(PauseMenuPolicy.StoryFor(runner.Flow, Table), Is.Null,
                "sin fila el botón se oculta en vez de quedar mudo");
            Assert.That(PauseMenuPolicy.WatchStory(runner, Table), Is.False);
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing), "y el flujo no se movió");
        }

        [Test]
        public void PauseMenuPolicy_INC159_FueraDeJugarNoHayHistoria()
        {
            var runner = RunnerPlaying(LevelId.Fire, 1);
            runner.Flow.TryGoTo(GameState.LevelSelect); // PlayingLevel sigue fijo, pero ya no se juega

            Assert.That(PauseMenuPolicy.StoryFor(runner.Flow, Table), Is.Null);
            Assert.That(PauseMenuPolicy.StoryFor(null, Table), Is.Null);
            Assert.That(PauseMenuPolicy.StoryFor(runner.Flow, null), Is.Null);
        }

        [Test]
        public void PauseMenuPolicy_INC159_VolverAVerLaHistoriaEntraALaNarrativaSinTocarElPerfil()
        {
            var runner = RunnerPlaying(LevelId.Fire, 1);
            var profile = runner.Flow.ActiveProfile;
            var reached = profile.ReachedLevel;

            var result = PauseMenuPolicy.WatchStory(runner, Table);

            Assert.That(result, Is.True);
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative));
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N1_Hallazgo"));
            Assert.That(profile.ReachedLevel, Is.EqualTo(reached), "ver la historia no mueve el progreso (RF-41)");
            Assert.That(profile.ConfirmedPhases, Is.Empty);
        }

        [Test]
        public void PauseMenuPolicy_INC159_GuardarYSalirGuardaAntesDeCerrar()
        {
            var order = new System.Collections.Generic.List<string>();

            PauseMenuPolicy.SaveThenQuit(new RecordingSaver(order), () => order.Add("quit"));

            Assert.That(order, Is.EqualTo(new[] { "save", "quit" }), "primero se guarda, después se sale (RF-09)");
        }

        [Test]
        public void PauseMenuPolicy_INC159_SalirSinPerfilActivoSigueSaliendo()
        {
            var quit = false;

            PauseMenuPolicy.SaveThenQuit(null, () => quit = true);

            Assert.That(quit, Is.True, "sin perfil no hay nada que guardar, pero salir tiene que funcionar");
        }

        private sealed class RecordingSaver : IProfileSaver
        {
            private readonly System.Collections.Generic.List<string> _order;

            public RecordingSaver(System.Collections.Generic.List<string> order) => _order = order;

            public void SaveActive() => _order.Add("save");
        }
    }
}
