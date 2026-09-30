using System;
using System.Linq;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class LevelUnlockPolicyTests
    {
        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        [Test]
        public void LevelUnlockPolicy_RF03_CompletarUnNivelSoloHabilitaElSiguiente()
        {
            var profile = NewProfile();
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(1, 0, 1, 10f));

            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Fire);

            Assert.That(profile.ReachedLevel, Is.EqualTo(LevelId.Wheel));
        }

        [Test]
        public void LevelUnlockPolicy_RF03_CompletarElUltimoNivelNoHabilitaNingunoMas()
        {
            var profile = NewProfile();
            profile.Reach(LevelId.River);

            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.River);

            Assert.That(profile.ReachedLevel, Is.EqualTo(LevelId.River));
        }

        [Test]
        public void LevelUnlockPolicy_CP02_NuncaRebloqueaUnNivelYaDesbloqueado()
        {
            var profile = NewProfile();
            profile.Reach(LevelId.River);
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(1, 0, 1, 10f));

            // El Nivel 1 sí está completo, así que el desbloqueo corre de verdad: aun así no
            // devuelve el perfil al Nivel 2.
            LevelUnlockPolicy.UnlockAfterCompleting(profile, LevelId.Fire);

            Assert.That(profile.ReachedLevel, Is.EqualTo(LevelId.River));
        }

        [Test]
        public void LevelUnlockPolicy_RF03_UnPerfilNuevoSoloTieneElNivel1Desbloqueado()
        {
            var profile = NewProfile();

            Assert.That(LevelUnlockPolicy.AllLevels.Where(level => LevelUnlockPolicy.IsUnlocked(profile, level)),
                Is.EqualTo(new[] { LevelId.Fire }));
        }

        /// <summary>
        /// Un cierre forzado durante la escena de cierre del nivel (2.5, 3.3) deja las fases
        /// confirmadas sin pasar por el resumen que llama a <see cref="LevelUnlockPolicy.UnlockAfterCompleting"/>
        /// (RNF-14, PF-RNF14-04, 30/09/2026): el desbloqueo se deriva del progreso guardado.
        /// </summary>
        [Test]
        public void LevelUnlockPolicy_RNF14_UnNivelConTodasSusFasesConfirmadasDesbloqueaElSiguiente()
        {
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel);
            foreach (var phase in PhaseId.AllOf(LevelId.Wheel))
            {
                profile.ConfirmPhase(phase, new PerformanceIndicators(1, 0, 1, 10f));
            }

            // Nunca se llamó a UnlockAfterCompleting: ReachedLevel sigue en Wheel.
            Assert.That(profile.ReachedLevel, Is.EqualTo(LevelId.Wheel), "supuesto: el resumen nunca se mostró");
            Assert.That(LevelUnlockPolicy.IsUnlocked(profile, LevelId.River), Is.True,
                "el Nivel 3 se desbloquea aunque el resumen del Nivel 2 nunca avanzó ReachedLevel");
        }

        /// <summary>El nivel a medio jugar —fases parciales— no se desbloquea por esta regla.</summary>
        [Test]
        public void LevelUnlockPolicy_RNF14_UnNivelConFasesPendientesNoDesbloqueaElSiguiente()
        {
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel);
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), new PerformanceIndicators(1, 0, 1, 10f));

            Assert.That(LevelUnlockPolicy.IsUnlocked(profile, LevelId.River), Is.False);
        }
    }
}
