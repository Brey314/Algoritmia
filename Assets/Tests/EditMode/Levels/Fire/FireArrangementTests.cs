using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireArrangementTests
    {
        private static readonly Vector2 Spot = new Vector2(10f, -20f);

        private static FireArrangement CreateSystemUnderTest() =>
            new FireArrangement(Spot, pileRadius: 50f, stonesRadius: 100f);

        [Test]
        [Category("Acceptance")]
        public void FireArrangement_RF16_LasPiedrasEstanCercaSoloSiLasDosCaenEnSuRadio()
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.StonesNear(Spot + new Vector2(60f, 0f), Spot + new Vector2(0f, -99f)), Is.True,
                "las dos dentro del radio de las piedras");
            Assert.That(sut.StonesNear(Spot + new Vector2(60f, 0f), Spot + new Vector2(0f, 101f)), Is.False,
                "con el pedernal fuera no cuenta");
            Assert.That(sut.StonesNear(Spot + new Vector2(-150f, 0f), Spot), Is.False,
                "con el sílex fuera tampoco");
        }

        [Test]
        [Category("Acceptance")]
        public void FireArrangement_RF19_HayMontonSoloSiTodasLasHojasCaenEnSuRadio()
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.IsPiled(new[] { Spot, Spot + new Vector2(30f, 30f), Spot + new Vector2(-49f, 0f) }), Is.True,
                "tres hojas dentro del radio del montón");
            Assert.That(sut.IsPiled(new[] { Spot, Spot + new Vector2(30f, 30f), Spot + new Vector2(-51f, 0f) }), Is.False,
                "una sola hoja fuera deshace el montón");
        }

        [Test]
        public void FireArrangement_RF19_SinHojasNoHayMonton()
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.IsPiled(new Vector2[0]), Is.False);
        }
    }
}
