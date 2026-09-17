using NUnit.Framework;

namespace Game.Scaffolding.Tests
{
    /// <summary>La escena 3.2 del guion (§8.4.1): tras el primer fallo, una vez; nunca si se acierta.</summary>
    public class ConditionalNarrativeTriggerTests
    {
        private const string Escena32 = "N3_Escena32_PrimerIntento";

        [Test]
        public void NarrativeTrigger_Guion841_LaEscena32SoloSeDisparaTrasElPrimerFallo()
        {
            var sut = new ConditionalNarrativeTrigger(Escena32);

            Assert.That(sut.AfterAttempt(passed: false), Is.EqualTo(Escena32), "primer fallo: se reproduce");
            Assert.That(sut.Fired, Is.True);
            Assert.That(sut.AfterAttempt(passed: false), Is.Null, "segundo fallo: ya se vio, no se repite");
            Assert.That(sut.AfterAttempt(passed: true), Is.Null, "acertar nunca la dispara");
        }

        [Test]
        public void NarrativeTrigger_Guion841_AcertarAlPrimerIntentoSaltaLaEscena32()
        {
            var sut = new ConditionalNarrativeTrigger(Escena32);

            Assert.That(sut.AfterAttempt(passed: true), Is.Null, "se pasa directo al cruce (3.3)");
            Assert.That(sut.Fired, Is.False, "la escena queda sin reproducir");
        }
    }
}
