using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireArrangementTests
    {
        private static readonly Vector2 Spot = new Vector2(10f, -20f);

        private static FireArrangement CreateSystemUnderTest() => new FireArrangement(Spot, radius: 100f);

        [Test]
        [Category("Acceptance")]
        public void FireArrangement_RF14_EstaTodoReunidoSoloSiCadaPiezaCaeDentroDelCirculo()
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.IsGathered(new[] { Spot, Spot + new Vector2(60f, 0f), Spot + new Vector2(0f, -99f) }), Is.True,
                "tres piezas dentro del círculo");
            Assert.That(sut.IsGathered(new[] { Spot, Spot + new Vector2(60f, 0f), Spot + new Vector2(0f, 101f) }), Is.False,
                "una sola pieza fuera y no está reunido");
            Assert.That(sut.IsGathered(new[] { Spot + new Vector2(-150f, 0f), Spot }), Is.False,
                "tampoco si la que falta es la primera");
        }

        [Test]
        public void FireArrangement_RF14_SinPiezasNoHayNadaReunido()
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.IsGathered(new Vector2[0]), Is.False);
        }
    }
}
