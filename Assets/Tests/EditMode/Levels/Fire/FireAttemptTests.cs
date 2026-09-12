using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireAttemptTests
    {
        // N1_Config propuesto (Fase 5): diez muescas, efectivas la siete y la ocho.
        private const int Soft = 2;
        private const int Hard = 10;
        private const int Effective = 7;

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

        private FireLevelConfig CreateConfig(int minimumEffectiveStrikes, int attemptsBeforeHint)
        {
            var config = ScriptableObject.CreateInstance<FireLevelConfig>();
            config.EffectiveForceMin = 7;
            config.EffectiveForceMax = 8;
            config.MinimumEffectiveStrikes = minimumEffectiveStrikes;
            config.AttemptsBeforeHint = attemptsBeforeHint;
            _configs.Add(config);
            return config;
        }

        private FireAttempt CreateSystemUnderTest(int minimumEffectiveStrikes, int attemptsBeforeHint) =>
            new FireAttempt(CreateConfig(minimumEffectiveStrikes, attemptsBeforeHint));

        private static void StrikeRepeatedly(FireAttempt attempt, int force, int times)
        {
            for (var i = 0; i < times; i++)
            {
                attempt.Strike(force);
            }
        }

        [TestCase(0, ForceBand.TooSoft)]
        [TestCase(6, ForceBand.TooSoft)]
        [TestCase(7, ForceBand.Effective)]
        [TestCase(8, ForceBand.Effective)]
        [TestCase(9, ForceBand.TooHard)]
        [TestCase(10, ForceBand.TooHard)]
        public void FireAttempt_RNF18_ClasificaLaFuerzaSegunLaFranjaDeLaConfiguracion(int fuerza, ForceBand franjaEsperada)
        {
            var sut = CreateSystemUnderTest(3, 3);

            Assert.That(sut.Classify(fuerza), Is.EqualTo(franjaEsperada));
        }

        [TestCase(Soft, ForceBand.TooSoft)]
        [TestCase(Hard, ForceBand.TooHard)]
        [Category("Acceptance")]
        public void FireAttempt_RF16_GolpeNoEfectivoProduceConsecuenciaYNoSuma(int fuerzaFallida, ForceBand franja)
        {
            var sut = CreateSystemUnderTest(3, 3);

            var actual = sut.Strike(fuerzaFallida);

            Assert.That(actual.Effective, Is.False, "un golpe fuera de la franja efectiva no prende chispa");
            Assert.That(actual.Force, Is.EqualTo(fuerzaFallida), "el resultado recuerda con qué fuerza se golpeó");
            Assert.That(actual.Band, Is.EqualTo(franja), "y hacia qué lado se falló: eso elige el mensaje");
            Assert.That(sut.EffectiveStrikes, Is.Zero, "un fallo no suma golpe efectivo (RF-16)");
            Assert.That(sut.ConsecutiveFailures, Is.EqualTo(1), "el fallo cuenta como fallo consecutivo");
        }

        [Test]
        [Category("Acceptance")]
        public void FireAttempt_RF16_ConLasPiedrasLejosNoPrendeAunqueLaFuerzaSeaLaEfectiva()
        {
            var sut = CreateSystemUnderTest(3, 3);

            var actual = sut.Strike(Effective, stonesNear: false);

            Assert.That(actual.Effective, Is.False, "sin las piedras cerca no hay chispa que caiga en las hojas (T22)");
            Assert.That(actual.StonesNear, Is.False, "el resultado dice que el fallo fue de sitio");
            Assert.That(actual.Band, Is.EqualTo(ForceBand.Effective), "la fuerza sí era la buena");
            Assert.That(sut.EffectiveStrikes, Is.Zero);
            Assert.That(sut.ConsecutiveFailures, Is.EqualTo(1), "cuenta como fallo consecutivo hacia la pista");
        }

        [Test]
        public void FireAttempt_RF16_GolpeEfectivoSumaYReiniciaLosFallos()
        {
            var sut = CreateSystemUnderTest(3, 3);
            sut.Strike(Soft);
            sut.Strike(Hard);

            var actual = sut.Strike(Effective);

            Assert.That(actual.Effective, Is.True, "el golpe con la fuerza efectiva prende una chispa");
            Assert.That(actual.EffectiveStrikes, Is.EqualTo(1), "el resultado trae el acumulado tras contar este golpe");
            Assert.That(sut.EffectiveStrikes, Is.EqualTo(1), "el golpe efectivo suma");
            Assert.That(sut.ConsecutiveFailures, Is.Zero, "un acierto reinicia los fallos consecutivos (guion §4.3.3)");
        }

        [Test]
        [Category("Acceptance")]
        public void FireAttempt_RF15_CambiarLaFuerzaNoAlteraElEstado()
        {
            var sut = CreateSystemUnderTest(3, 3);

            Assert.That(sut.EffectiveStrikes, Is.Zero, "sin golpear no hay golpes efectivos");
            Assert.That(sut.ConsecutiveFailures, Is.Zero, "sin golpear no hay fallos");
            Assert.That(sut.CanBlow, Is.False, "sin golpear el soplo no está disponible");
            Assert.That(sut.Classify(Effective), Is.EqualTo(ForceBand.Effective), "clasificar una fuerza no golpea");
            Assert.That(sut.EffectiveStrikes, Is.Zero, "leer el estado repetidamente no lo cambia");
            Assert.That(sut.CanBlow, Is.False, "leer el estado repetidamente no lo cambia");
        }

        [Test]
        [Category("Acceptance")]
        public void FireAttempt_RF18_AceptaIntentosIlimitados()
        {
            var sut = CreateSystemUnderTest(3, 3);

            Assert.That(() => StrikeRepeatedly(sut, Soft, 100), Throws.Nothing,
                "cien golpes no efectivos seguidos no lanzan excepción (CP-02, RF-18)");
            Assert.That(sut.ConsecutiveFailures, Is.EqualTo(100), "cada intento se registra: no hay tope (RF-18)");
            Assert.That(sut.EffectiveStrikes, Is.Zero, "cien fallos no suman ningún golpe efectivo");
            Assert.That(sut.CanBlow, Is.False, "sin golpes efectivos el soplo nunca se habilita");
        }

        [Test]
        public void FireAttempt_INC32_SoplarSeHabilitaAlAlcanzarElMinimo()
        {
            var sut = CreateSystemUnderTest(3, 3);

            sut.Strike(Effective);
            Assert.That(sut.CanBlow, Is.False, "con un golpe efectivo el soplo aún no");
            sut.Strike(Effective);
            Assert.That(sut.CanBlow, Is.False, "con dos golpes efectivos el soplo aún no");
            sut.Strike(Effective);
            Assert.That(sut.CanBlow, Is.True, "al tercer golpe efectivo el soplo se habilita (INC-32)");
        }

        [Test]
        [Category("Acceptance")]
        public void FireAttempt_RF19_SoplarNoSeDeshabilitaTrasFalloPosterior()
        {
            var sut = CreateSystemUnderTest(3, 3);
            StrikeRepeatedly(sut, Effective, 3);

            sut.Strike(Soft);
            sut.Strike(Hard);

            Assert.That(sut.CanBlow, Is.True, "lo ganado no se pierde por un fallo posterior (RF-19, INC-32)");
            Assert.That(sut.EffectiveStrikes, Is.EqualTo(3), "un fallo posterior no reduce los golpes efectivos");
        }

        [Test]
        public void FireAttempt_RNF18_ElUmbralDePistaSaleDeLaConfiguracion()
        {
            var sut = CreateSystemUnderTest(3, 5);

            StrikeRepeatedly(sut, Soft, 4);
            Assert.That(sut.ShouldOfferHint, Is.False, "cuatro fallos con umbral cinco: todavía no");
            sut.Strike(Soft);
            Assert.That(sut.ShouldOfferHint, Is.True, "el quinto fallo alcanza el umbral definido en la configuración (RNF-18)");

            sut.Strike(Effective);
            Assert.That(sut.ConsecutiveFailures, Is.Zero, "el acierto reinicia los fallos consecutivos");
            Assert.That(sut.ShouldOfferHint, Is.False, "y con ellos se retira el ofrecimiento de pista (RF-19)");
        }
    }
}
