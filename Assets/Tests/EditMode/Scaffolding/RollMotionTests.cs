using NUnit.Framework;

namespace Game.Scaffolding.Tests
{
    /// <summary>El rodado que comparten el bosque y la narrativa (RF-26, RNF-21).</summary>
    public class RollMotionTests
    {
        [Test]
        public void RollMotion_RF26_LaCajaAvanzaSinRetrocederYLuegoCae()
        {
            var previo = RollMotion.Evaluate(0f, 0.8f);
            Assert.That(previo.Along, Is.EqualTo(0f), "arranca al principio de la fila");
            Assert.That(previo.Fall, Is.EqualTo(0f), "y en el aire no");

            for (var paso = 1; paso <= 100; paso++)
            {
                var ahora = RollMotion.Evaluate(paso / 100f, 0.8f);
                Assert.That(ahora.Along, Is.GreaterThanOrEqualTo(previo.Along), "nunca retrocede");
                Assert.That(ahora.Drop, Is.GreaterThanOrEqualTo(previo.Drop), "y una vez cae no vuelve a subir");
                if (paso / 100f <= 0.8f)
                {
                    Assert.That(ahora.Fall, Is.EqualTo(0f), "mientras rueda no cae");
                }

                previo = ahora;
            }

            var final = RollMotion.Evaluate(1f, 0.8f);
            Assert.That(final.Along, Is.EqualTo(1f), "llega al último tronco");
            Assert.That(final.Fall, Is.EqualTo(1f).And.EqualTo(final.Drop), "y acaba en el suelo");
            Assert.That(final.Tilt, Is.EqualTo(RollMotion.TiltDegrees), "ladeada");
            Assert.That(final.Spin, Is.EqualTo(-360f * RollMotion.Turns), "con los troncos habiendo dado sus vueltas");
        }

        [Test]
        public void RollMotion_RF26_SinTiempoDeCaidaLaCajaSoloRueda()
        {
            var final = RollMotion.Evaluate(1f, 1f);

            Assert.That(final.Along, Is.EqualTo(1f));
            Assert.That(final.Fall, Is.EqualTo(0f), "sin caída no baja");
            Assert.That(final.Tilt, Is.EqualTo(0f), "ni se ladea");
        }
    }
}
