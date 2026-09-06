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
    }
}
