using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireAttemptTests
    {
        private readonly List<Object> _configs = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var config in _configs)
            {
                Object.DestroyImmediate(config);
            }

            _configs.Clear();
        }

        private FireLevelConfig CreateConfig(
            StrikePosition effectivePosition, int minimumEffectiveStrikes, int attemptsBeforeHint)
        {
            var config = ScriptableObject.CreateInstance<FireLevelConfig>();
            config.EffectivePosition = effectivePosition;
            config.MinimumEffectiveStrikes = minimumEffectiveStrikes;
            config.AttemptsBeforeHint = attemptsBeforeHint;
            _configs.Add(config);
            return config;
        }

        private FireAttempt CreateSystemUnderTest(
            StrikePosition effectivePosition, int minimumEffectiveStrikes, int attemptsBeforeHint) =>
            new FireAttempt(CreateConfig(effectivePosition, minimumEffectiveStrikes, attemptsBeforeHint));

        private static void StrikeRepeatedly(FireAttempt attempt, StrikePosition position, int times)
        {
            for (var i = 0; i < times; i++)
            {
                attempt.Strike(position);
            }
        }

        [TestCase(StrikePosition.Far)]
        [TestCase(StrikePosition.Near)]
        [Category("Acceptance")]
        public void FireAttempt_RF16_GolpeNoEfectivoProduceConsecuenciaYNoSuma(StrikePosition posicionFallida)
        {
            var sut = CreateSystemUnderTest(StrikePosition.VeryClose, 3, 3);

            var actual = sut.Strike(posicionFallida);

            Assert.That(actual.Effective, Is.False, "un golpe desde una distancia no efectiva no prende chispa");
            Assert.That(actual.Position, Is.EqualTo(posicionFallida), "el resultado recuerda desde dónde se golpeó");
            Assert.That(sut.EffectiveStrikes, Is.Zero, "un fallo no suma golpe efectivo (RF-16)");
            Assert.That(sut.ConsecutiveFailures, Is.EqualTo(1), "el fallo cuenta como fallo consecutivo");
        }

        [Test]
        public void FireAttempt_RF16_GolpeEfectivoSumaYReiniciaLosFallos()
        {
            var sut = CreateSystemUnderTest(StrikePosition.VeryClose, 3, 3);
            sut.Strike(StrikePosition.Far);
            sut.Strike(StrikePosition.Near);

            var actual = sut.Strike(StrikePosition.VeryClose);

            Assert.That(actual.Effective, Is.True, "el golpe desde la posición efectiva prende una chispa");
            Assert.That(actual.EffectiveStrikes, Is.EqualTo(1), "el resultado trae el acumulado tras contar este golpe");
            Assert.That(sut.EffectiveStrikes, Is.EqualTo(1), "el golpe efectivo suma");
            Assert.That(sut.ConsecutiveFailures, Is.Zero, "un acierto reinicia los fallos consecutivos (guion §4.3.3)");
        }

        [Test]
        [Category("Acceptance")]
        public void FireAttempt_RF15_CambiarPosicionNoAlteraElEstado()
        {
            var sut = CreateSystemUnderTest(StrikePosition.VeryClose, 3, 3);

            Assert.That(sut.EffectiveStrikes, Is.Zero, "sin golpear no hay golpes efectivos");
            Assert.That(sut.ConsecutiveFailures, Is.Zero, "sin golpear no hay fallos");
            Assert.That(sut.CanBlow, Is.False, "sin golpear el soplo no está disponible");
            Assert.That(sut.EffectiveStrikes, Is.Zero, "leer el estado repetidamente no lo cambia");
            Assert.That(sut.CanBlow, Is.False, "leer el estado repetidamente no lo cambia");
        }

        [Test]
        [Category("Acceptance")]
        public void FireAttempt_RF18_AceptaIntentosIlimitados()
        {
            var sut = CreateSystemUnderTest(StrikePosition.VeryClose, 3, 3);

            Assert.That(() => StrikeRepeatedly(sut, StrikePosition.Far, 100), Throws.Nothing,
                "cien golpes no efectivos seguidos no lanzan excepción (CP-02, RF-18)");
            Assert.That(sut.ConsecutiveFailures, Is.EqualTo(100), "cada intento se registra: no hay tope (RF-18)");
            Assert.That(sut.EffectiveStrikes, Is.Zero, "cien fallos no suman ningún golpe efectivo");
            Assert.That(sut.CanBlow, Is.False, "sin golpes efectivos el soplo nunca se habilita");
        }

        [Test]
        public void FireAttempt_INC32_SoplarSeHabilitaAlAlcanzarElMinimo()
        {
            var sut = CreateSystemUnderTest(StrikePosition.VeryClose, 3, 3);

            sut.Strike(StrikePosition.VeryClose);
            Assert.That(sut.CanBlow, Is.False, "con un golpe efectivo el soplo aún no");
            sut.Strike(StrikePosition.VeryClose);
            Assert.That(sut.CanBlow, Is.False, "con dos golpes efectivos el soplo aún no");
            sut.Strike(StrikePosition.VeryClose);
            Assert.That(sut.CanBlow, Is.True, "al tercer golpe efectivo el soplo se habilita (INC-32)");
        }

        [Test]
        [Category("Acceptance")]
        public void FireAttempt_RF19_SoplarNoSeDeshabilitaTrasFalloPosterior()
        {
            var sut = CreateSystemUnderTest(StrikePosition.VeryClose, 3, 3);
            StrikeRepeatedly(sut, StrikePosition.VeryClose, 3);

            sut.Strike(StrikePosition.Far);
            sut.Strike(StrikePosition.Near);

            Assert.That(sut.CanBlow, Is.True, "lo ganado no se pierde por un fallo posterior (RF-19, INC-32)");
            Assert.That(sut.EffectiveStrikes, Is.EqualTo(3), "un fallo posterior no reduce los golpes efectivos");
        }

        [Test]
        public void FireAttempt_RNF18_ElUmbralDePistaSaleDeLaConfiguracion()
        {
            var sut = CreateSystemUnderTest(StrikePosition.VeryClose, 3, 5);

            StrikeRepeatedly(sut, StrikePosition.Far, 4);
            Assert.That(sut.ShouldOfferHint, Is.False, "cuatro fallos con umbral cinco: todavía no");
            sut.Strike(StrikePosition.Far);
            Assert.That(sut.ShouldOfferHint, Is.True, "el quinto fallo alcanza el umbral definido en la configuración (RNF-18)");

            sut.Strike(StrikePosition.VeryClose);
            Assert.That(sut.ConsecutiveFailures, Is.Zero, "el acierto reinicia los fallos consecutivos");
            Assert.That(sut.ShouldOfferHint, Is.False, "y con ellos se retira el ofrecimiento de pista (RF-19)");
        }
    }
}
