using System;
using System.Linq;
using NUnit.Framework;

namespace Game.Scaffolding.Tests
{
    public class FacialEmotionTests
    {
        [TestCase(ActorAction.Idle, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Hidden, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Walk, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Run, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Talk, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Strike, FacialEmotion.Focused)]
        [TestCase(ActorAction.Hammer, FacialEmotion.Focused)]
        [TestCase(ActorAction.Blow, FacialEmotion.Focused)]
        [TestCase(ActorAction.PickUp, FacialEmotion.Focused)]
        [TestCase(ActorAction.Kneel, FacialEmotion.Focused)]
        [TestCase(ActorAction.Carry, FacialEmotion.Focused)]
        [TestCase(ActorAction.Push, FacialEmotion.Focused)]
        [TestCase(ActorAction.Point, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Observe, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Celebrate, FacialEmotion.Happy)]
        [TestCase(ActorAction.Encourage, FacialEmotion.Happy)]
        [TestCase(ActorAction.Hug, FacialEmotion.Happy)]
        [TestCase(ActorAction.Surprise, FacialEmotion.Surprised)]
        [TestCase(ActorAction.Sleep, FacialEmotion.Sleeping)]
        [TestCase(ActorAction.Appear, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Vanish, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Spin, FacialEmotion.Neutral)]
        [TestCase(ActorAction.Wave, FacialEmotion.Happy)]
        public void FacialEmotion_DA73_CadaAccionTieneSuEmocionPorDefecto(ActorAction accion, FacialEmotion esperada)
        {
            Assert.That(ActionEmotion.For(accion), Is.EqualTo(esperada));
        }

        [Test]
        public void FacialEmotion_DA73_LaTablaCubreTodasLasAcciones()
        {
            // Una acción nueva en el enum que nadie anotó caería a Neutral sin avisar: la lista de
            // casos de arriba tiene que crecer con ella.
            var cubiertas = typeof(FacialEmotionTests).GetMethod(nameof(FacialEmotion_DA73_CadaAccionTieneSuEmocionPorDefecto))
                .GetCustomAttributes(typeof(TestCaseAttribute), false)
                .Cast<TestCaseAttribute>()
                .Select(caso => (ActorAction)caso.Arguments[0])
                .ToArray();

            Assert.That(Enum.GetValues(typeof(ActorAction)).Cast<ActorAction>().Except(cubiertas), Is.Empty);
        }

        /// <summary>
        /// No existe cara de derrota, tristeza ni enfado (CP-02, DA §7.3): tras un intento sin éxito
        /// el personaje anima, y una cara abatida le diría al estudiante que perdió.
        /// </summary>
        [Test]
        public void FacialEmotion_CP02_NingunaEmocionEsDeDerrotaNiTristeza()
        {
            var vetadas = new[] { "sad", "angry", "anger", "mad", "defeat", "lose", "lost", "fail", "cry", "upset", "gameover" };

            var nombres = Enum.GetNames(typeof(FacialEmotion)).Select(nombre => nombre.ToLowerInvariant());

            Assert.That(nombres.Where(nombre => vetadas.Any(vetada => nombre.Contains(vetada))), Is.Empty);
        }

        [Test]
        public void FacialEmotion_CP02_TrasUnIntentoSinExitoElAnimoEsUnaCaraAlegre()
        {
            Assert.That(ActionEmotion.For(ActorAction.Encourage), Is.EqualTo(FacialEmotion.Happy));
        }

        [Test]
        public void BlinkClock_DA73_ParpadeaConElIntervaloYLaDuracion()
        {
            // Azar fijo en 0,5: el desvío es cero y el intervalo, exactamente 3,5 s.
            var sut = new BlinkClock(3.5f, 1.2f, 0.12f, () => 0.5f);

            Assert.That(sut.Tick(3.49f), Is.EqualTo(BlinkPhase.Open), "todavía no toca");
            Assert.That(sut.Tick(0.02f), Is.EqualTo(BlinkPhase.Half), "baja el párpado");
            Assert.That(sut.Tick(0.04f), Is.EqualTo(BlinkPhase.Closed), "cerrado en el tercio central");
            Assert.That(sut.Tick(0.04f), Is.EqualTo(BlinkPhase.Half), "sube el párpado");
            Assert.That(sut.Tick(0.04f), Is.EqualTo(BlinkPhase.Open), "pasados los 0,12 s vuelve a abrir");
            Assert.That(sut.Tick(3.4f), Is.EqualTo(BlinkPhase.Open), "el siguiente se cuenta desde que abrió");
            Assert.That(sut.Tick(0.11f), Is.EqualTo(BlinkPhase.Half), "y llega a los 3,5 s");
        }

        [Test]
        public void BlinkClock_DA73_ElAzarMueveElIntervaloDentroDelDesvio()
        {
            var pronto = new BlinkClock(3.5f, 1.2f, 0.12f, () => 0f); // 3,5 - 1,2
            var tarde = new BlinkClock(3.5f, 1.2f, 0.12f, () => 0.999f); // 3,5 + ~1,2

            Assert.That(pronto.Tick(2.29f), Is.EqualTo(BlinkPhase.Open));
            Assert.That(pronto.Tick(0.02f), Is.EqualTo(BlinkPhase.Half), "a los 2,3 s");
            Assert.That(tarde.Tick(4.6f), Is.EqualTo(BlinkPhase.Open), "aún no a los 4,6 s");
            Assert.That(tarde.Tick(0.2f), Is.Not.EqualTo(BlinkPhase.Open), "ya a los 4,8 s");
        }

        [Test]
        public void BlinkClock_DA73_ApagadoNoParpadea()
        {
            var sut = new BlinkClock(3.5f, 1.2f, 0.12f, () => 0.5f) { Enabled = false };

            for (var i = 0; i < 50; i++)
            {
                Assert.That(sut.Tick(1f), Is.EqualTo(BlinkPhase.Open), "dormido sus ojos ya son los cerrados");
            }

            sut.Enabled = true;
            Assert.That(sut.Tick(3.49f), Is.EqualTo(BlinkPhase.Open), "al encender cuenta desde cero");
            Assert.That(sut.Tick(0.02f), Is.EqualTo(BlinkPhase.Half));
        }

        [Test]
        public void MouthFlap_RF05_LaBocaSeMueveMientrasHablaYSeCierraAlCallar()
        {
            var sut = new MouthFlap(0.1f);

            Assert.That(sut.Tick(0.5f, speaking: false), Is.EqualTo(MouthShape.Rest));
            Assert.That(sut.Tick(0f, speaking: true), Is.EqualTo(MouthShape.A), "abre en cuanto empieza a hablar");
            Assert.That(sut.Tick(0.05f, speaking: true), Is.EqualTo(MouthShape.A));
            Assert.That(sut.Tick(0.06f, speaking: true), Is.EqualTo(MouthShape.E));
            Assert.That(sut.Tick(0.1f, speaking: true), Is.EqualTo(MouthShape.U));
            Assert.That(sut.Tick(0.1f, speaking: true), Is.EqualTo(MouthShape.Closed), "entre sílabas se cierra");
            Assert.That(sut.Tick(0.1f, speaking: true), Is.EqualTo(MouthShape.A), "y el ciclo vuelve a empezar");

            Assert.That(sut.Tick(0.01f, speaking: false), Is.EqualTo(MouthShape.Rest), "al callar cierra en seco");
            Assert.That(sut.Tick(0f, speaking: true), Is.EqualTo(MouthShape.A), "la próxima vez empieza por el principio");
        }
    }
}
