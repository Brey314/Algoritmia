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

        private static readonly Vector2 Orilla = new Vector2(0.20f, 0.47f);

        /// <summary>
        /// Un viajero como los de la 3.3: cruza en la línea 0, celebra en la 1, señala sin moverse en
        /// la 2, baja a la orilla en la 3 y celebra en la 4. Devuelve también sus pasos con movimiento.
        /// </summary>
        private static (NarrativeProp Prop, ActorBeat Cruza, ActorBeat Senala, ActorBeat Baja) Viajero(bool terminaSusPasos)
        {
            var cruza = new ActorBeat(0, ActorAction.Idle).MovingTo(Fondo, 9f);
            var senala = new ActorBeat(2, ActorAction.Point).MovingTo(Fondo, 1.2f);
            var baja = new ActorBeat(3, ActorAction.Walk).MovingTo(Orilla, 2.5f);
            var viajero = Personaje(ActorAction.Idle,
                cruza, new ActorBeat(1, ActorAction.Celebrate), senala, baja, new ActorBeat(4, ActorAction.Celebrate));
            return (terminaSusPasos ? viajero.FinishingSteps() : viajero, cruza, senala, baja);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ActorTimeline_RF05_UnPasoNuevoInterrumpeElCaminoDeQuienNoTerminaSusPasos(int linea)
        {
            var sut = Viajero(terminaSusPasos: false).Prop;

            var interrumpe = ActorTimeline.Interrupts(sut, linea);

            Assert.That(interrumpe, Is.True, "el paso nuevo manda: salta a donde tenía que llegar y lo empieza");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ActorTimeline_RF05_UnaLineaSinPasoNoInterrumpeElCamino(bool terminaSusPasos)
        {
            var sut = Viajero(terminaSusPasos).Prop;

            var interrumpe = ActorTimeline.Interrupts(sut, 5);

            Assert.That(interrumpe, Is.False, "quien camina termina su camino aunque el texto avance");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ActorTimeline_RNF21_NingunPasoInterrumpeAQuienTerminaSusPasos(int linea)
        {
            var sut = Viajero(terminaSusPasos: true).Prop;

            var interrumpe = ActorTimeline.Interrupts(sut, linea);

            Assert.That(interrumpe, Is.False, "ni celebrar, ni señalar en el sitio, ni bajar a la orilla lo bajan de la balsa a medio río");
        }

        [TestCase(0)]
        [TestCase(1)]
        public void ActorTimeline_RNF21_SiNoSeLeyoNingunPasoQueLoMuevaNoQuedaNadaPendiente(int linea)
        {
            var (sut, cruza, _, _) = Viajero(terminaSusPasos: true);

            var pendiente = ActorTimeline.PendingStep(sut, cruza, linea);

            Assert.That(pendiente, Is.Null, "celebrar no lo mueve: al llegar hace lo de la línea en curso");
        }

        [Test]
        public void ActorTimeline_RNF21_UnPasoEnElSitioLeidoMientrasCruzabaQuedaPendiente()
        {
            var (sut, cruza, senala, _) = Viajero(terminaSusPasos: true);

            var pendiente = ActorTimeline.PendingStep(sut, cruza, 2);

            Assert.That(pendiente, Is.SameAs(senala), "al llegar señala lo que dura su paso, y después dice su línea");
        }

        [TestCase(3)]
        [TestCase(4)]
        public void ActorTimeline_RNF21_QuedaPendienteElUltimoPasoQueLoMueve(int linea)
        {
            var (sut, cruza, _, baja) = Viajero(terminaSusPasos: true);

            var pendiente = ActorTimeline.PendingStep(sut, cruza, linea);

            Assert.That(pendiente, Is.SameAs(baja), "baja a la orilla: el último que lo mueve es el que dice dónde acaba");
        }

        [Test]
        public void ActorTimeline_RNF21_ElPasoQueAcabaDeDarNoQuedaPendiente()
        {
            var (sut, _, _, baja) = Viajero(terminaSusPasos: true);

            var pendiente = ActorTimeline.PendingStep(sut, baja, 4);

            Assert.That(pendiente, Is.Null, "llegó a la orilla y ahí acaba: no repite el paso que acaba de dar");
        }

        [Test]
        public void ActorTimeline_RF05_QuienNoTerminaSusPasosNoDejaNadaPendiente()
        {
            var (sut, cruza, _, _) = Viajero(terminaSusPasos: false);

            var pendiente = ActorTimeline.PendingStep(sut, cruza, 4);

            Assert.That(pendiente, Is.Null, "a ese el paso nuevo lo interrumpió al leerse");
        }

        [Test]
        public void ActorTimeline_RF05_LaEmocionDeUnPasoSeMantieneHastaQueOtroLaCambie()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Idle).WithEmotion(FacialEmotion.Worried),
                new ActorBeat(4, ActorAction.Kneel),
                new ActorBeat(6, ActorAction.Idle).WithEmotion(FacialEmotion.Happy),
                new ActorBeat(8, ActorAction.Walk).WithEmotion(FacialEmotion.Surprised),
                new ActorBeat(8, ActorAction.Walk).MovingTo(Fondo, 2f).WithEmotion(FacialEmotion.Focused));

            Assert.That(ActorTimeline.Cue(sut, 0, speaking: false).Emotion, Is.Null, "antes del primer paso manda la acción");
            Assert.That(ActorTimeline.Cue(sut, 1, speaking: false).Emotion, Is.EqualTo(FacialEmotion.Worried));
            Assert.That(ActorTimeline.Cue(sut, 3, speaking: true).Emotion, Is.EqualTo(FacialEmotion.Worried), "una línea sin paso la mantiene");
            Assert.That(ActorTimeline.Cue(sut, 4, speaking: false).Emotion, Is.EqualTo(FacialEmotion.Worried),
                "un paso que no la fija no la cambia");
            Assert.That(ActorTimeline.Cue(sut, 6, speaking: false).Emotion, Is.EqualTo(FacialEmotion.Happy));
            Assert.That(ActorTimeline.Cue(sut, 7, speaking: false).Emotion, Is.EqualTo(FacialEmotion.Happy));
            Assert.That(ActorTimeline.Cue(sut, 8, speaking: false).Emotion, Is.EqualTo(FacialEmotion.Focused),
                "si hay dos en la misma línea, el último de la lista");
            Assert.That(ActorTimeline.EmotionAt(sut, 5), Is.EqualTo(FacialEmotion.Worried));
        }

        [Test]
        public void ActorTimeline_RF05_SinEmocionDeclaradaElCueNoLaFija()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(0, ActorAction.Celebrate),
                new ActorBeat(2, ActorAction.Walk).MovingTo(Fondo, 2f, ActorAction.Kneel));

            for (var linea = 0; linea < 5; linea++)
            {
                Assert.That(ActorTimeline.Cue(sut, linea, speaking: linea % 2 == 0).Emotion, Is.Null,
                    $"línea {linea}: los assets que no declaran emociones siguen con la de la acción");
            }

            Assert.That(new ActorBeat(0, ActorAction.Idle).SetsEmotion, Is.False, "por defecto un paso no fija expresión");
        }
    }
}
