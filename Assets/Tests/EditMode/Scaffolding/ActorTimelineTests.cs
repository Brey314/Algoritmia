using NUnit.Framework;
using UnityEngine;

namespace Game.Scaffolding.Tests
{
    public class ActorTimelineTests
    {
        private static readonly Vector2 Entrada = new Vector2(0.40f, 0.45f);
        private static readonly Vector2 Fondo = new Vector2(0.30f, 0.44f);

        private static NarrativeProp Personaje(ActorAction salida, params ActorBeat[] pasos) =>
            new NarrativeProp(null, Entrada, 0.3f).WithActor(null, salida, pasos);

        [Test]
        public void ActorTimeline_RF05_SinPasosSeQuedaDondeEstaHaciendoLoDeSalida()
        {
            var cue = ActorTimeline.Cue(Personaje(ActorAction.Hidden), 3, speaking: false);

            Assert.That(cue.Moves, Is.False);
            Assert.That(cue.From, Is.EqualTo(Entrada));
            Assert.That(cue.During, Is.EqualTo(ActorAction.Hidden), "Algoritm no aparece hasta que un paso lo trae");
        }

        [Test]
        public void ActorTimeline_RF05_UnPasoConDestinoAvanzaHaciendoSuAccionYAlLlegarHaceLaLlegada()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(2, ActorAction.Walk).MovingTo(Fondo, 2.5f, ActorAction.Kneel));

            var cue = ActorTimeline.Cue(sut, 2, speaking: false);

            Assert.That(cue.Moves, Is.True);
            Assert.That(cue.From, Is.EqualTo(Entrada));
            Assert.That(cue.To, Is.EqualTo(Fondo));
            Assert.That(cue.Seconds, Is.EqualTo(2.5f));
            Assert.That(cue.During, Is.EqualTo(ActorAction.Walk));
            Assert.That(cue.After, Is.EqualTo(ActorAction.Kneel));
        }

        [Test]
        public void ActorTimeline_RF05_EntrePasosConservaElSitioYLaUltimaAccion()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(2, ActorAction.Walk).MovingTo(Fondo, 2f, ActorAction.Kneel));

            var cue = ActorTimeline.Cue(sut, 5, speaking: false);

            Assert.That(cue.Moves, Is.False);
            Assert.That(cue.From, Is.EqualTo(Fondo), "llegó al fondo en la línea 2 y ahí sigue");
            Assert.That(cue.During, Is.EqualTo(ActorAction.Kneel), "arrodillado sigue arrodillado: no vuelve solo al reposo");
        }

        [Test]
        public void ActorTimeline_RF05_QuienDiceLaLineaDePieYQuietoGesticula()
        {
            var cue = ActorTimeline.Cue(Personaje(ActorAction.Idle), 1, speaking: true);

            Assert.That(cue.During, Is.EqualTo(ActorAction.Talk));
        }

        [Test]
        public void ActorTimeline_RF05_LoQueElGuionDiceQueHaceMandaSobreElGestoDeHablar()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(0, ActorAction.Kneel),
                new ActorBeat(4, ActorAction.Point));

            Assert.That(ActorTimeline.Cue(sut, 2, speaking: true).During, Is.EqualTo(ActorAction.Kneel),
                "arrodillado habla arrodillado");
            Assert.That(ActorTimeline.Cue(sut, 4, speaking: true).During, Is.EqualTo(ActorAction.Point),
                "el paso de su propia línea manda");
        }

        [Test]
        public void ActorTimeline_RF05_UnPasoDeReposoEnSuPropiaLineaLaDiceGesticulando()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(0, ActorAction.Kneel),
                new ActorBeat(3, ActorAction.Idle));

            Assert.That(ActorTimeline.Cue(sut, 3, speaking: true).During, Is.EqualTo(ActorAction.Talk),
                "se pone de pie y dice su línea");
            Assert.That(ActorTimeline.Cue(sut, 3, speaking: false).During, Is.EqualTo(ActorAction.Idle));
        }

        [Test]
        public void ActorTimeline_RF05_ElCaminoEnCursoEsElUltimoPasoConMovimientoSinInterrumpir()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(0, ActorAction.Walk).MovingTo(Fondo, 9f),
                new ActorBeat(4, ActorAction.Celebrate));

            Assert.That(ActorTimeline.WalkUnderway(sut, 0), Is.Null, "en su propia línea no está «en curso»: empieza");
            Assert.That(ActorTimeline.WalkUnderway(sut, 2)?.Line, Is.EqualTo(0), "dos líneas después puede seguir caminando");
            Assert.That(ActorTimeline.WalkUnderway(sut, 5), Is.Null, "celebrar en la 4 lo interrumpió");
        }

        [Test]
        public void ActorTimeline_RF05_AlLlegarHablandoGesticulaSiLaLlegadaEsElReposo()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Run).MovingTo(Fondo, 1f));

            var cue = ActorTimeline.Cue(sut, 1, speaking: true);

            Assert.That(cue.During, Is.EqualTo(ActorAction.Run), "mientras corre, corre");
            Assert.That(cue.After, Is.EqualTo(ActorAction.Talk), "y al llegar dice su línea");
        }

        [Test]
        public void ActorTimeline_RF05_LaPosicionTrasLaLineaIncluyeSuPropioMovimiento()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(2, ActorAction.Walk).MovingTo(Fondo, 2f));

            Assert.That(ActorTimeline.PositionBefore(sut, 2), Is.EqualTo(Entrada));
            Assert.That(ActorTimeline.PositionAfter(sut, 2), Is.EqualTo(Fondo));
        }
    }
}
