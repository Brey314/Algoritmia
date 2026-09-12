using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireLevelConfigTests
    {
        [Test]
        [Category("Acceptance")]
        public void FireLevelConfig_CT05_ExponeLosCincoParametrosDelNivel()
        {
            var sut = ScriptableObject.CreateInstance<FireLevelConfig>();

            Assert.That(sut.ForceLevels, Is.EqualTo(10), "muescas del deslizante de fuerza (Fase 5)");
            Assert.That(sut.EffectiveForceMin, Is.EqualTo(7), "fuerza efectiva mínima propuesta");
            Assert.That(sut.EffectiveForceMax, Is.EqualTo(8), "fuerza efectiva máxima propuesta");
            Assert.That(sut.MinimumEffectiveStrikes, Is.EqualTo(3), "golpes efectivos mínimos propuestos (guion §4.3.2)");
            Assert.That(sut.AttemptsBeforeHint, Is.EqualTo(3), "fallos consecutivos antes de pista propuestos");

            sut.ForceLevels = 5;
            sut.EffectiveForceMin = 3;
            sut.EffectiveForceMax = 4;
            sut.MinimumEffectiveStrikes = 7;
            sut.AttemptsBeforeHint = 2;

            Assert.That(sut.ForceLevels, Is.EqualTo(5), "el cambio de muescas se conserva");
            Assert.That(sut.EffectiveForceMin, Is.EqualTo(3), "el cambio de fuerza mínima se conserva");
            Assert.That(sut.EffectiveForceMax, Is.EqualTo(4), "el cambio de fuerza máxima se conserva");
            Assert.That(sut.MinimumEffectiveStrikes, Is.EqualTo(7), "el cambio de mínimo se conserva");
            Assert.That(sut.AttemptsBeforeHint, Is.EqualTo(2), "el cambio de umbral de pista se conserva");

            Object.DestroyImmediate(sut);
        }
    }
}
