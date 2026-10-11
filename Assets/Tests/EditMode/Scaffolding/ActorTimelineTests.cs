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

        /// <summary>
        /// La expresión de la tarjeta del diálogo es la del personaje en la escena (INC-148): la que el guion fija
        /// en esa línea y, si ningún paso la fija, la de la acción que hace durante ella. Decir la línea (Talk) no
        /// cambia la cara: el gesto de hablar es neutro.
        /// </summary>
        [Test]
        public void ActorTimeline_INC148_LaEmocionDelRetratoEsLaDelGuionYSiNoLaDeSuAccion()
        {
            var delGuion = Personaje(ActorAction.Idle,
                new ActorBeat(0, ActorAction.Celebrate).WithEmotion(FacialEmotion.Worried),
                new ActorBeat(3, ActorAction.Kneel).WithEmotion(FacialEmotion.Happy));

            Assert.That(ActorTimeline.EmotionOf(delGuion, 0, speaking: true), Is.EqualTo(FacialEmotion.Worried),
                "el guion manda sobre la acción: Celebrate sería Happy y aquí el guion pide Worried");
            Assert.That(ActorTimeline.EmotionOf(delGuion, 2, speaking: false), Is.EqualTo(FacialEmotion.Worried), "y se mantiene entre pasos");
            Assert.That(ActorTimeline.EmotionOf(delGuion, 3, speaking: true), Is.EqualTo(FacialEmotion.Happy), "hasta que otro paso la cambia");
            Assert.That(ActorTimeline.EmotionOf(delGuion, 3, speaking: true), Is.EqualTo(ActorTimeline.EmotionAt(delGuion, 3)),
                "con guion es exactamente EmotionAt");

            var porAccion = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Celebrate),
                new ActorBeat(3, ActorAction.Kneel),
                new ActorBeat(5, ActorAction.Walk).MovingTo(Fondo, 2f, ActorAction.Point));

            Assert.That(ActorTimeline.EmotionOf(porAccion, 0, speaking: true), Is.EqualTo(FacialEmotion.Neutral), "de pie y hablando: el gesto de hablar es neutro");
            Assert.That(ActorTimeline.EmotionOf(porAccion, 0, speaking: false), Is.EqualTo(FacialEmotion.Neutral));
            Assert.That(ActorTimeline.EmotionOf(porAccion, 1, speaking: true), Is.EqualTo(FacialEmotion.Happy), "celebra: alegre");
            Assert.That(ActorTimeline.EmotionOf(porAccion, 2, speaking: true), Is.EqualTo(FacialEmotion.Happy), "y lo que mantiene entre pasos");
            Assert.That(ActorTimeline.EmotionOf(porAccion, 3, speaking: true), Is.EqualTo(FacialEmotion.Focused), "arrodillado: concentrado");
            Assert.That(ActorTimeline.EmotionOf(porAccion, 5, speaking: true), Is.EqualTo(FacialEmotion.Neutral), "mientras camina, la de caminar");
            Assert.That(ActorTimeline.EmotionOf(porAccion, 6, speaking: true), Is.EqualTo(FacialEmotion.Neutral), "y al llegar la de lo que hace allí (señalar)");

            // Es la misma cara que le pone el rig a la escena: cue.Emotion y, si es nulo, la de cue.During.
            for (var linea = 0; linea < 8; linea++)
            {
                var cue = ActorTimeline.Cue(porAccion, linea, speaking: true);
                Assert.That(ActorTimeline.EmotionOf(porAccion, linea, speaking: true),
                    Is.EqualTo(cue.Emotion ?? ActionEmotion.For(cue.During)), $"línea {linea}: la del rig");
            }
        }

        // ---- INC-134: hacia dónde mira de perfil ----
        //
        // FacesLeftAt decide solo el lado (true = izquierda); si el personaje va de perfil lo decide la
        // acción. Orden: el Facing explícito del paso que rige, el desplazamiento de la línea, el último
        // anterior, el primero posterior y, sin ninguno, hacia el centro de la ilustración.

        private static readonly Vector2 AlaDerecha = new Vector2(0.60f, 0.45f);
        // Desde AlaDerecha: sube mucho y avanza poco a la derecha (arriba manda, aunque el horizontal diría derecha)...
        private static readonly Vector2 Arriba = new Vector2(0.65f, 0.80f);

        // ...y desde Arriba: baja mucho y retrocede poco a la izquierda (abajo manda, aunque el horizontal diría izquierda).
        private static readonly Vector2 Abajo = new Vector2(0.60f, 0.30f);

        [Test]
        public void ActorTimeline_INC134_ElRumboExplicitoDelPasoMandaSobreElDesplazamiento()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Walk).MovingTo(Fondo, 2f).WithFacing(ActorFacing.Right),
                new ActorBeat(4, ActorAction.Walk).MovingTo(AlaDerecha, 2f).WithFacing(ActorFacing.Left));

            Assert.That(ActorTimeline.FacesLeftAt(sut, 1), Is.False, "camina a la izquierda pero el guion lo fija a la derecha");
            Assert.That(ActorTimeline.FacesLeftAt(sut, 4), Is.True, "camina a la derecha pero el guion lo fija a la izquierda");
            Assert.That(ActorTimeline.ExplicitFacingAt(sut, 0), Is.Null, "antes del primer paso nada está fijado");
            Assert.That(ActorTimeline.ExplicitFacingAt(sut, 2), Is.False, "una línea sin paso mantiene el rumbo del paso que rige");
        }

        [Test]
        public void ActorTimeline_INC134_ElRumboFijadoRigeUnSoloPasoYElSiguienteVuelveAAuto()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Kneel).WithFacing(ActorFacing.Left),
                new ActorBeat(3, ActorAction.Walk).MovingTo(AlaDerecha, 2f));

            Assert.That(ActorTimeline.FacesLeftAt(sut, 2), Is.True, "arrodillado, mirando a donde se le dijo");
            Assert.That(ActorTimeline.ExplicitFacingAt(sut, 3), Is.Null, "el paso nuevo no declara rumbo: Auto");
            Assert.That(ActorTimeline.FacesLeftAt(sut, 3), Is.False, "camina a la derecha y mira a la derecha, no hereda el rumbo fijado");
        }

        [Test]
        public void ActorTimeline_INC134_SinRumboExplicitoMiraHaciaDondeSeDesplazaElPasoDeLaLinea()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Walk).MovingTo(Fondo, 2f),          // a la izquierda y un poco abajo
                new ActorBeat(3, ActorAction.Walk).MovingTo(AlaDerecha, 2f),     // a la derecha
                new ActorBeat(5, ActorAction.Walk).MovingTo(Arriba, 2f),         // sube: el eje vertical es el dominante
                new ActorBeat(7, ActorAction.Walk).MovingTo(Abajo, 2f));         // baja: el eje vertical es el dominante

            Assert.That(ActorTimeline.FacesLeftAt(sut, 1), Is.True, "a la izquierda");
            Assert.That(ActorTimeline.FacesLeftAt(sut, 3), Is.False, "a la derecha");
            Assert.That(ActorTimeline.FacesLeftAt(sut, 5), Is.True, "hacia arriba mira a la izquierda aunque avance un poco a la derecha");
            Assert.That(ActorTimeline.FacesLeftAt(sut, 7), Is.False, "hacia abajo mira a la derecha aunque retroceda un poco a la izquierda");
        }

        [Test]
        public void ActorTimeline_INC134_UnDesplazamientoMinimoNoDecideElRumbo()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Walk).MovingTo(AlaDerecha, 2f),
                new ActorBeat(3, ActorAction.Walk).MovingTo(AlaDerecha + new Vector2(-0.005f, 0f), 1f)); // 5 milésimas: no se ve

            Assert.That(ActorTimeline.FacesLeftAt(sut, 3), Is.False, "el paso de la línea mide menos de 0,01: vale el último rumbo visible");
        }

        [Test]
        public void ActorTimeline_INC134_SinDesplazamientoEnLaLineaMiraHaciaDondeSeDesplazoAntes()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Walk).MovingTo(Fondo, 2f),
                new ActorBeat(2, ActorAction.Walk).MovingTo(AlaDerecha, 2f),
                new ActorBeat(4, ActorAction.Kneel));

            Assert.That(ActorTimeline.FacesLeftAt(sut, 4), Is.False, "arrodillado tras caminar a la derecha: mira a la derecha (el último, no el primero)");
            Assert.That(ActorTimeline.FacesLeftAt(sut, 6), Is.False, "una línea sin paso lo mantiene");
            Assert.That(ActorTimeline.FacesLeftAt(sut, 2), Is.False, "y en la línea del paso, el suyo");
        }

        /// <summary>El rumbo fijado a mano de un paso con movimiento pesa más que su desplazamiento también cuando lo heredan las líneas de alrededor.</summary>
        [Test]
        public void ActorTimeline_INC134_ElRumboFijadoDeUnPasoAnteriorOPosteriorPesaMasQueSuDesplazamiento()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Walk).MovingTo(AlaDerecha, 2f).WithFacing(ActorFacing.Left),
                new ActorBeat(3, ActorAction.Kneel));

            Assert.That(ActorTimeline.FacesLeftAt(sut, 3), Is.True, "tras caminar a la derecha con rumbo fijado a la izquierda, arrodillado mira a la izquierda");

            var antes = Personaje(ActorAction.Idle,
                new ActorBeat(0, ActorAction.Kneel),
                new ActorBeat(2, ActorAction.Walk).MovingTo(AlaDerecha, 2f).WithFacing(ActorFacing.Left));

            Assert.That(ActorTimeline.FacesLeftAt(antes, 0), Is.True, "y quien espera antes de ese paso ya mira al lado fijado, no al del desplazamiento");
        }

        [Test]
        public void ActorTimeline_INC134_SinDesplazamientoPreviosMiraHaciaDondeSeDesplazaDespues()
        {
            var sut = Personaje(ActorAction.Idle,
                new ActorBeat(1, ActorAction.Kneel),
                new ActorBeat(4, ActorAction.Walk).MovingTo(Fondo, 2f),
                new ActorBeat(6, ActorAction.Walk).MovingTo(AlaDerecha, 2f));

            Assert.That(ActorTimeline.FacesLeftAt(sut, 1), Is.True, "espera antes de partir y ya mira hacia donde va: el PRIMER desplazamiento posterior, a la izquierda");
            Assert.That(ActorTimeline.FacesLeftAt(sut, 0), Is.True, "también antes de su primer paso");
        }

        [Test]
        public void ActorTimeline_INC134_SinNingunDesplazamientoMiraHaciaElCentroDeLaIlustracion()
        {
            var aLaIzquierda = new NarrativeProp(null, new Vector2(0.20f, 0.45f), 0.3f)
                .WithActor(null, ActorAction.Idle, new ActorBeat(1, ActorAction.Kneel));
            var aLaDerecha = new NarrativeProp(null, new Vector2(0.80f, 0.45f), 0.3f)
                .WithActor(null, ActorAction.Idle, new ActorBeat(1, ActorAction.Kneel));
            var enElCentro = new NarrativeProp(null, new Vector2(0.50f, 0.45f), 0.3f).WithActor(null, ActorAction.Idle);

            Assert.That(ActorTimeline.FacesLeftAt(aLaIzquierda, 1), Is.False, "a la izquierda de la imagen mira a la derecha, hacia el centro");
            Assert.That(ActorTimeline.FacesLeftAt(aLaDerecha, 1), Is.True, "a la derecha mira a la izquierda");
            Assert.That(ActorTimeline.FacesLeftAt(enElCentro, 0), Is.True, "justo en el centro (x = 0,5) manda la izquierda, sin pasos de por medio");
        }

        [Test]
        public void ActorTimeline_INC134_ElPasoEncadenadoMiraHaciaDondeVaSalvoQueNoDecidaNada()
        {
            var haciaLaDerecha = new ActorBeat(2, ActorAction.Walk).MovingTo(AlaDerecha, 2f);
            var fijado = new ActorBeat(2, ActorAction.Walk).MovingTo(AlaDerecha, 2f).WithFacing(ActorFacing.Left);
            var enElSitio = new ActorBeat(2, ActorAction.Walk).MovingTo(Entrada + new Vector2(0.002f, 0f), 2f);

            Assert.That(ActorTimeline.FacesLeftOfStep(haciaLaDerecha, Entrada, fallback: true), Is.False, "su desplazamiento, desde donde sale");
            Assert.That(ActorTimeline.FacesLeftOfStep(haciaLaDerecha, new Vector2(0.9f, 0.45f), fallback: false), Is.True,
                "sale de donde llegó, que no es donde el paso decía empezar: desde la derecha va hacia la izquierda");
            Assert.That(ActorTimeline.FacesLeftOfStep(fijado, Entrada, fallback: false), Is.True, "el explícito manda");
            Assert.That(ActorTimeline.FacesLeftOfStep(enElSitio, Entrada, fallback: true), Is.True, "si no decide nada, conserva el lado");
            Assert.That(ActorTimeline.FacesLeftOfStep(null, Entrada, fallback: true), Is.True, "sin paso, tampoco");
        }

        /// <summary>Un asset anterior a INC-134 no declara el campo: Auto, y el resto del paso queda igual.</summary>
        [Test]
        public void ActorBeat_INC134_PorDefectoElRumboEsAuto()
        {
            var paso = new ActorBeat(0, ActorAction.Walk).MovingTo(Fondo, 1f, ActorAction.Kneel);

            Assert.That(paso.Facing, Is.EqualTo(ActorFacing.Auto));
            Assert.That(paso.WithFacing(ActorFacing.Right).Facing, Is.EqualTo(ActorFacing.Right));
            Assert.That(paso.Moves && paso.Arrival == ActorAction.Kneel, Is.True, "fijar el rumbo no toca lo demás");
        }
    }
}
