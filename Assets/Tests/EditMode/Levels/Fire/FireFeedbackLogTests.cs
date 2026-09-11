using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireFeedbackLogTests
    {
        private readonly List<Object> _messages = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var messages in _messages)
            {
                Object.DestroyImmediate(messages);
            }

            _messages.Clear();
        }

        private FireMessages CreateMessages()
        {
            var messages = ScriptableObject.CreateInstance<FireMessages>();
            messages.FarSparksFade = "lejos-cualquiera";
            messages.FarNeverReach = "lejos-tras-dos";
            messages.NearColdStone = "cerca-cualquiera";
            messages.NearBesideLeaves = "cerca-tras-dos";
            messages.FirstEffectiveStrike = "efectivo-primero";
            messages.SecondEffectiveStrike = "efectivo-segundo";
            messages.FinalEffectiveStrike = "efectivo-final";
            messages.BlowSuccess = "soplo-exito";
            _messages.Add(messages);
            return messages;
        }

        private FireFeedbackLog CreateSystemUnderTest(int minimumEffectiveStrikes) =>
            new FireFeedbackLog(CreateMessages(), minimumEffectiveStrikes);

        private static bool HayMensajesConsecutivosIguales(IReadOnlyList<string> entradas) =>
            entradas.Where((entrada, indice) => indice > 0 && entrada == entradas[indice - 1]).Any();

        [TestCase(StrikePosition.Far, "lejos-cualquiera")]
        [TestCase(StrikePosition.Near, "cerca-cualquiera")]
        public void FireFeedbackLog_guion434_CadaDistanciaNoEfectivaDevuelveSuMensaje(
            StrikePosition posicionFallida, string mensajeEsperado)
        {
            var sut = CreateSystemUnderTest(minimumEffectiveStrikes: 3);

            var actual = sut.Record(StrikeOutcome.SparksDied(posicionFallida), consecutiveFailures: 1);

            Assert.That(actual, Is.EqualTo(mensajeEsperado), "devuelve el mensaje «cualquiera» de esa distancia");
            Assert.That(sut.Entries, Is.EqualTo(new[] { mensajeEsperado }), "y lo registra en el historial");
        }

        [Test]
        public void FireFeedbackLog_RF18_TrasDosFallosSeguidosEnLaMismaDistanciaEscalaElMensaje()
        {
            var sut = CreateSystemUnderTest(minimumEffectiveStrikes: 3);
            sut.Record(StrikeOutcome.SparksDied(StrikePosition.Far), consecutiveFailures: 1);

            var actual = sut.Record(StrikeOutcome.SparksDied(StrikePosition.Far), consecutiveFailures: 2);

            Assert.That(actual, Is.EqualTo("lejos-tras-dos"));
        }

        [TestCase(1, "efectivo-primero")]
        [TestCase(2, "efectivo-segundo")]
        [TestCase(3, "efectivo-final")]
        public void FireFeedbackLog_HU05_CadaGolpeEfectivoDevuelveElMensajeDeSuOrdinal(
            int golpesEfectivos, string mensajeEsperado)
        {
            var sut = CreateSystemUnderTest(minimumEffectiveStrikes: 3);

            var actual = sut.Record(StrikeOutcome.SparkLanded(golpesEfectivos), consecutiveFailures: 0);

            Assert.That(actual, Is.EqualTo(mensajeEsperado));
        }

        [Test]
        [Category("Acceptance")]
        public void FireFeedbackLog_RF18_NoRepiteElMismoMensajeDosVecesSeguidas()
        {
            var sut = CreateSystemUnderTest(minimumEffectiveStrikes: 3);

            sut.Record(StrikeOutcome.SparksDied(StrikePosition.Far), consecutiveFailures: 1);
            sut.Record(StrikeOutcome.SparksDied(StrikePosition.Far), consecutiveFailures: 2);
            sut.Record(StrikeOutcome.SparksDied(StrikePosition.Far), consecutiveFailures: 3);

            Assert.That(sut.Entries, Has.Count.EqualTo(3), "tres golpes seguidos dejan tres entradas");
            Assert.That(HayMensajesConsecutivosIguales(sut.Entries), Is.False,
                "ningún par consecutivo de entradas es igual (RF-18)");
        }

        [Test]
        [Category("Acceptance")]
        public void FireFeedbackLog_HU06_AcumulaElHistorialEnOrden()
        {
            var sut = CreateSystemUnderTest(minimumEffectiveStrikes: 3);

            var primero = sut.Record(StrikeOutcome.SparksDied(StrikePosition.Far), consecutiveFailures: 1);
            var segundo = sut.Record(StrikeOutcome.SparksDied(StrikePosition.Near), consecutiveFailures: 1);
            var tercero = sut.Record(StrikeOutcome.SparkLanded(1), consecutiveFailures: 0);

            Assert.That(sut.Entries, Is.EqualTo(new[] { primero, segundo, tercero }),
                "conserva todos los mensajes anteriores en orden");
            Assert.That(sut.Latest, Is.EqualTo(tercero), "y Latest es el último");
        }

        [Test]
        public void FireFeedbackLog_HU05_ElSoploExitosoRegistraSuMensaje()
        {
            var sut = CreateSystemUnderTest(minimumEffectiveStrikes: 3);

            var actual = sut.RecordBlowSuccess();

            Assert.That(actual, Is.EqualTo("soplo-exito"), "devuelve el mensaje del soplo");
            Assert.That(sut.Entries, Is.EqualTo(new[] { "soplo-exito" }), "y lo añade al historial");
        }
    }
}
