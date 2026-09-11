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
            profile.ConfirmPhase(LevelId.Fire, 1, indicators);
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
            Assert.That(profile.IsPhaseConfirmed(LevelId.Fire, 1), Is.True,
                "Restart borró una fase ya confirmada");
            Assert.That(profile.IndicatorsFor(LevelId.Fire, 1), Is.EqualTo(indicators),
                "Restart alteró los indicadores de una fase ya confirmada");
            Assert.That(profile.ConfirmedPhases.Count, Is.EqualTo(1),
                "Restart cambió el número de fases confirmadas");
        }
    }
}
