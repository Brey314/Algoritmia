using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireLevelConfigTests
    {
        [Test]
        [Category("Acceptance")]
        public void FireLevelConfig_CT05_ExponeLosDiezParametrosDelNivel()
        {
            var sut = ScriptableObject.CreateInstance<FireLevelConfig>();

            Assert.That(sut.ForceLevels, Is.EqualTo(10), "muescas del deslizante de fuerza (Fase 5)");
            Assert.That(sut.EffectiveForceMin, Is.EqualTo(7), "fuerza efectiva mínima propuesta");
            Assert.That(sut.EffectiveForceMax, Is.EqualTo(8), "fuerza efectiva máxima propuesta");
            Assert.That(sut.SpacingLevels, Is.EqualTo(10), "muescas del deslizante de cercanía (Fase 6)");
            Assert.That(sut.EffectiveOverlap, Is.EqualTo(5f), "las piedras se rozan encimadas unos cinco píxeles");
            Assert.That(sut.OverlapTolerance, Is.EqualTo(4f), "con este margen a cada lado");
            Assert.That(sut.GatherZoom, Is.EqualTo(2f), "la cámara se acerca al doble al reunir");
            Assert.That(sut.GatherSeconds, Is.EqualTo(0.8f), "en menos de un segundo");
            Assert.That(sut.MinimumEffectiveStrikes, Is.EqualTo(3), "golpes efectivos mínimos propuestos (guion §4.3.2)");
            Assert.That(sut.AttemptsBeforeHint, Is.EqualTo(3), "fallos consecutivos antes de pista propuestos");

            sut.ForceLevels = 5;
            sut.EffectiveForceMin = 3;
            sut.EffectiveForceMax = 4;
            sut.SpacingLevels = 6;
            sut.EffectiveOverlap = 9f;
            sut.OverlapTolerance = 1f;
            sut.GatherZoom = 1.5f;
            sut.GatherSeconds = 2f;
            sut.MinimumEffectiveStrikes = 7;
            sut.AttemptsBeforeHint = 2;

            Assert.That(sut.ForceLevels, Is.EqualTo(5), "el cambio de muescas se conserva");
            Assert.That(sut.EffectiveForceMin, Is.EqualTo(3), "el cambio de fuerza mínima se conserva");
            Assert.That(sut.EffectiveForceMax, Is.EqualTo(4), "el cambio de fuerza máxima se conserva");
            Assert.That(sut.SpacingLevels, Is.EqualTo(6), "el cambio de muescas de cercanía se conserva");
            Assert.That(sut.EffectiveOverlap, Is.EqualTo(9f), "el cambio de roce efectivo se conserva");
            Assert.That(sut.OverlapTolerance, Is.EqualTo(1f), "el cambio de margen se conserva");
            Assert.That(sut.GatherZoom, Is.EqualTo(1.5f), "el cambio de acercamiento se conserva");
            Assert.That(sut.GatherSeconds, Is.EqualTo(2f), "el cambio de duración se conserva");
            Assert.That(sut.MinimumEffectiveStrikes, Is.EqualTo(7), "el cambio de mínimo se conserva");
            Assert.That(sut.AttemptsBeforeHint, Is.EqualTo(2), "el cambio de umbral de pista se conserva");

            Object.DestroyImmediate(sut);
        }
    }
}
