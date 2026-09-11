using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireLevelConfigTests
    {
        [Test]
        [Category("Acceptance")]
        public void FireLevelConfig_CT05_ExponeLosCuatroParametrosDelGuion()
        {
            var sut = ScriptableObject.CreateInstance<FireLevelConfig>();

            Assert.That(sut.AvailablePositions, Is.EqualTo(3), "distancias disponibles propuestas (guion §4.3.2)");
            Assert.That(sut.EffectivePosition, Is.EqualTo(StrikePosition.VeryClose), "posición efectiva propuesta");
            Assert.That(sut.MinimumEffectiveStrikes, Is.EqualTo(3), "golpes efectivos mínimos propuestos");
            Assert.That(sut.AttemptsBeforeHint, Is.EqualTo(3), "fallos consecutivos antes de pista propuestos");

            sut.AvailablePositions = 5;
            sut.EffectivePosition = StrikePosition.Near;
            sut.MinimumEffectiveStrikes = 7;
            sut.AttemptsBeforeHint = 2;

            Assert.That(sut.AvailablePositions, Is.EqualTo(5), "el cambio de distancias se conserva");
            Assert.That(sut.EffectivePosition, Is.EqualTo(StrikePosition.Near), "el cambio de posición efectiva se conserva");
            Assert.That(sut.MinimumEffectiveStrikes, Is.EqualTo(7), "el cambio de mínimo se conserva");
            Assert.That(sut.AttemptsBeforeHint, Is.EqualTo(2), "el cambio de umbral de pista se conserva");

            Object.DestroyImmediate(sut);
        }
    }
}
