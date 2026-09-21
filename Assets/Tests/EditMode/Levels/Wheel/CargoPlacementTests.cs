using NUnit.Framework;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>
    /// La cola de la fase 1: colocar la caja sobre los troncos y empujarla (RF-25, RF-26).
    /// </summary>
    /// <remarks>
    /// **Van en EditMode y no en PlayMode como pedía el plan**: <see cref="CargoPlacement"/> es
    /// C# plano y no toca escena ni frames, así que probarlo con el corredor de PlayMode costaría
    /// una carga de escena por caso sin verificar nada más. El cableado —arrastre real, botón y
    /// animación— sí es PlayMode y va en <c>ForestSceneTests</c>.
    /// </remarks>
    public class CargoPlacementTests
    {
        // Deliberadamente distintos del asset del juego: si la regla se apoyara en un literal en
        // vez de en la configuración, estas pruebas fallarían (RNF-18).
        private const int TroncosDePrueba = 3;

        private static readonly ForestObject Tronco1 = new ForestObject("tronco_1", ForestObjectCategory.RoundLog);
        private static readonly ForestObject Tronco2 = new ForestObject("tronco_2", ForestObjectCategory.RoundLog);
        private static readonly ForestObject Tronco3 = new ForestObject("tronco_3", ForestObjectCategory.RoundLog);
        private static readonly ForestObject Piedra = new ForestObject("piedra_1", ForestObjectCategory.Stone);

        [Test]
        public void CargoPlacement_RF25_LaCajaNoSeArrastraSinLosCincoTroncos()
        {
            var (seleccion, sut) = Bosque();
            seleccion.Select(Tronco1);

            var intento = sut.TryTake();

            Assert.That(sut.CanDrag, Is.False, "sin el acopio completo la caja no se mueve");
            Assert.That(intento.Accepted, Is.False, "y el intento no ejecuta nada (CU-06 FA-4a)");
            Assert.That(sut.IsHeld, Is.False, "la caja no queda agarrada");
            Assert.That(intento.Message, Is.EqualTo("faltan 2"),
                "informa cuántos troncos faltan, con el texto de la configuración (RNF-18)");
        }

        [Test]
        public void CargoPlacement_CP02_IntentarMoverlaAntesDeTiempoNoPenalizaNiBloquea()
        {
            var (seleccion, sut) = Bosque();

            for (var i = 0; i < 5; i++)
            {
                sut.TryTake();
            }

            Acopiar(seleccion);

            Assert.That(sut.CanDrag, Is.True,
                "cinco intentos prematuros no cierran ningún camino: al completar el acopio la caja se mueve igual (CP-02)");
            Assert.That(sut.TryTake().Accepted, Is.True);
        }

        [Test]
        public void CargoPlacement_RF25_ConElAcopioCompletoLaCajaSeAgarraYSeSuelta()
        {
            var (seleccion, sut) = Bosque();
            Acopiar(seleccion);

            var agarre = sut.TryTake();
            Assert.That(agarre.Accepted, Is.True, "el clic sostenido agarra la caja");
            Assert.That(sut.IsHeld, Is.True);
            Assert.That(agarre.Message, Is.Empty, "agarrarla no describe nada: todavía no pasó nada");

            var suelta = sut.Drop(overLogs: true);

            Assert.That(sut.IsHeld, Is.False, "soltarla la deja de agarrar");
            Assert.That(suelta.Accepted, Is.True, "quedó sobre los troncos alineados");
            Assert.That(suelta.Message, Is.EqualTo("colocada"), "y se dice, con el texto del asset");
            Assert.That(sut.IsPlaced, Is.True);
        }

        [Test]
        public void CargoPlacement_RF26_EmpujarSeHabilitaSoloConLaCajaColocada()
        {
            var (seleccion, sut) = Bosque();

            Assert.That(sut.CanPush, Is.False, "sin troncos no hay nada que empujar");
            Assert.That(sut.Push(), Is.False, "y accionarlo no hace nada");

            Acopiar(seleccion);
            sut.TryTake();
            sut.Drop(overLogs: false);

            Assert.That(sut.CanPush, Is.False, "la caja está suelta pero no sobre los troncos");
            Assert.That(sut.Push(), Is.False);

            sut.TryTake();
            sut.Drop(overLogs: true);

            Assert.That(sut.CanPush, Is.True, "colocada, «Empujar» se habilita (RF-26)");
            Assert.That(sut.Push(), Is.True);
            Assert.That(sut.IsPushed, Is.True, "y el rodado queda demostrado");
        }

        [Test]
        public void CargoPlacement_CP02_ColocarLaCajaNoSeDeshaceAlVolverASoltarlaFuera()
        {
            var (seleccion, sut) = Bosque();
            Acopiar(seleccion);
            sut.TryTake();
            sut.Drop(overLogs: true);

            // Se vuelve a levantar y se deja en cualquier otro sitio.
            sut.TryTake();
            var fuera = sut.Drop(overLogs: false);

            Assert.That(fuera.Accepted, Is.False, "ahí no toca los troncos y se dice");
            Assert.That(fuera.Message, Is.EqualTo("fuera"));
            Assert.That(sut.IsPlaced, Is.True, "pero lo ya conseguido no se retira");
            Assert.That(sut.CanPush, Is.True,
                "«Empujar», una vez habilitado, no vuelve a deshabilitarse (CP-02, RF-41)");
        }

        [Test]
        public void CargoPlacement_RF26_ElRodadoSeDemuestraUnaSolaVez()
        {
            var (seleccion, sut) = Bosque();
            Acopiar(seleccion);
            sut.TryTake();
            sut.Drop(overLogs: true);
            sut.Push();

            Assert.That(sut.Push(), Is.False, "la segunda pulsación no relanza el rodado");
            Assert.That(sut.IsPushed, Is.True, "y no deshace el de la primera");
            Assert.That(sut.CanPush, Is.True, "el botón sigue habilitado: no se retira nada (CP-02)");
        }

        [Test]
        public void CargoPlacement_RF25_SoltarSinHaberAgarradoNoColocaLaCaja()
        {
            var (seleccion, sut) = Bosque();
            Acopiar(seleccion);

            var suelta = sut.Drop(overLogs: true);

            Assert.That(suelta.Accepted, Is.False, "sin clic sostenido previo no hay nada que soltar");
            Assert.That(sut.IsPlaced, Is.False, "la caja no se coloca sola");
            Assert.That(sut.CanPush, Is.False);
        }

        // --- helpers -------------------------------------------------------------------------

        private static (PatternSelection Seleccion, CargoPlacement Caja) Bosque()
        {
            var config = ConfiguracionDePrueba();
            var seleccion = new PatternSelection(config);
            return (seleccion, new CargoPlacement(seleccion, config));
        }

        /// <summary>Completa el acopio: los troncos que la configuración de prueba pide.</summary>
        private static void Acopiar(PatternSelection seleccion)
        {
            seleccion.Select(Tronco1);
            seleccion.Select(Tronco2);
            seleccion.Select(Tronco3);
            Assume.That(seleccion.IsComplete, Is.True);
        }

        private static WheelLevelConfig ConfiguracionDePrueba() => WheelLevelConfig.Create(
            TroncosDePrueba,
            4,
            "acopiados {0} de {1}",
            new[] { Tronco1, Tronco2, Tronco3, Piedra },
            new[]
            {
                new CategoryFeedback(ForestObjectCategory.RoundLog, "acierto"),
                new CategoryFeedback(ForestObjectCategory.Stone, "piedra")
            });
    }
}
