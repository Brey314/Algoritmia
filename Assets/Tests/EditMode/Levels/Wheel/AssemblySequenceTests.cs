using System.Linq;
using NUnit.Framework;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>
    /// La máquina de ensamblaje de la fase 2 del Nivel 2 (RF-28, RF-29, guion §6.2.2).
    /// </summary>
    /// <remarks>
    /// EditMode: <see cref="AssemblySequence"/> es C# plano y no toca escena ni frames. El cableado
    /// —arrastre real, botón y animación— va en <c>WorkshopSceneTests</c>.
    /// </remarks>
    public class AssemblySequenceTests
    {
        // Mensajes deliberadamente distintos del asset del juego: si la regla se apoyara en un
        // literal en vez de en la configuración, estas pruebas fallarían (RNF-18).
        private static AssemblySequence Taller() => new AssemblySequence(AssemblyContent.Create());

        private static void PerforarLasDos(AssemblySequence sut)
        {
            sut.Select(WorkshopPiece.ShortLogA);
            sut.Machine();
            sut.Select(WorkshopPiece.ShortLogB);
            sut.Machine();
        }

        [Test]
        public void AssemblySequence_RF28_MecanizarExigeTroncoCortoSeleccionado()
        {
            var sut = Taller();

            Assert.That(sut.CanMachine, Is.False, "sin selección no hay nada que mecanizar (CU-07 FA-3a)");
            Assert.That(sut.Machine().Accepted, Is.False, "y pulsarlo no ejecuta nada");

            Assert.That(sut.Select(WorkshopPiece.Tool).Accepted, Is.False, "la herramienta no se selecciona");
            Assert.That(sut.Select(WorkshopPiece.LongLog).Accepted, Is.False, "el tronco largo tampoco");
            Assert.That(sut.CanMachine, Is.False);

            var seleccion = sut.Select(WorkshopPiece.ShortLogA);
            Assert.That(seleccion.Accepted, Is.True, "un tronco corto queda resaltado (guion §6.2.2 paso 1)");
            Assert.That(seleccion.Message, Is.EqualTo("seleccionado"));
            Assert.That(sut.CanMachine, Is.True, "y ahora «Mecanizar» está habilitado");

            var perforado = sut.Machine();
            Assert.That(perforado.Accepted, Is.True);
            Assert.That(perforado.Message, Is.EqualTo("rueda uno"));
            Assert.That(sut.IsDrilled(WorkshopPiece.ShortLogA), Is.True, "el tronco pasa a ser rueda");
            Assert.That(sut.DrilledWheels, Is.EqualTo(1));
            Assert.That(sut.CanMachine, Is.False, "la selección se consume al mecanizar");

            sut.Select(WorkshopPiece.ShortLogA);
            Assert.That(sut.CanMachine, Is.False, "una rueda ya perforada no se vuelve a mecanizar");
            Assert.That(sut.Machine().Message, Is.EqualTo("ya es rueda"));
        }

        [Test]
        public void AssemblySequence_RF29_RechazaCadaPasoFueraDeSecuenciaConSuMensaje()
        {
            var sut = Taller();

            var eje = sut.Place(WorkshopPiece.LongLog, overAssembly: true);
            Assert.That(eje.Accepted, Is.False, "el eje no entra sin las dos ruedas");
            Assert.That(eje.Message, Is.EqualTo("falta perforar"));
            Assert.That(sut.IsAxleFormed, Is.False);

            sut.Select(WorkshopPiece.ShortLogA);
            sut.Machine();
            Assert.That(sut.Place(WorkshopPiece.LongLog, true).Accepted, Is.False,
                "con una sola rueda el eje sigue sin entrar");

            var tabla = sut.Place(WorkshopPiece.Plank, true);
            Assert.That(tabla.Accepted, Is.False, "la tabla no se coloca sin eje");
            Assert.That(tabla.Message, Is.EqualTo("falta el eje"));

            var caja = sut.Place(WorkshopPiece.Cargo, true);
            Assert.That(caja.Accepted, Is.False, "la caja no se coloca sin tabla");
            Assert.That(caja.Message, Is.EqualTo("falta la tabla"));

            sut.Select(WorkshopPiece.ShortLogB);
            sut.Machine();
            Assert.That(sut.Place(WorkshopPiece.Cargo, true).Message, Is.EqualTo("falta la tabla"),
                "con el eje sin formar la caja sigue rechazada");
            Assert.That(sut.Place(WorkshopPiece.LongLog, true).Accepted, Is.True, "con las dos ruedas el eje entra");
            Assert.That(sut.Place(WorkshopPiece.Cargo, true).Message, Is.EqualTo("falta la tabla"),
                "con el eje pero sin tabla la caja sigue rechazada");
            Assert.That(sut.Place(WorkshopPiece.Plank, true).Accepted, Is.True, "con el eje la tabla se monta");
            Assert.That(sut.Place(WorkshopPiece.Cargo, true).Accepted, Is.True, "y con la tabla la caja completa la carretilla");
            Assert.That(sut.IsComplete, Is.True);
            Assert.That(sut.LastCompleted, Is.EqualTo(AssemblyStep.Cargo));
        }

        [Test]
        public void AssemblySequence_RF29_SoltarLejosDelLugarDeArmadoNoEjecutaNada()
        {
            var sut = Taller();
            PerforarLasDos(sut);

            var lejos = sut.Place(WorkshopPiece.LongLog, overAssembly: false);

            Assert.That(lejos.Accepted, Is.False);
            Assert.That(lejos.Message, Is.EqualTo("lejos"), "se describe dónde cayó, sin regañar (CP-02)");
            Assert.That(sut.IsAxleFormed, Is.False, "fuera del lugar de armado el eje no se forma");
            Assert.That(sut.Place(WorkshopPiece.LongLog, true).Accepted, Is.True, "y sobre él, sí");
        }

        [Test]
        public void AssemblySequence_CP02_UnPasoFueraDeOrdenNoDeshaceLoYaHecho()
        {
            var sut = Taller();
            sut.Select(WorkshopPiece.ShortLogA);
            sut.Machine();

            sut.Place(WorkshopPiece.LongLog, true);
            sut.Place(WorkshopPiece.Plank, true);
            sut.Place(WorkshopPiece.Cargo, true);

            Assert.That(sut.IsDrilled(WorkshopPiece.ShortLogA), Is.True,
                "perforar una rueda no se pierde por intentar mal el eje después");
            Assert.That(sut.LastCompleted, Is.EqualTo(AssemblyStep.FirstWheel));

            PerforarLasDos(sut);
            sut.Place(WorkshopPiece.LongLog, true);
            sut.Place(WorkshopPiece.Cargo, true);

            Assert.That(sut.IsAxleFormed, Is.True, "el eje formado sobrevive a soltar la caja antes de tiempo");
            Assert.That(sut.LastCompleted, Is.EqualTo(AssemblyStep.Axle));
        }

        [Test]
        public void AssemblySequence_CP06_ElMensajeDiceQueFaltaAntesNoCualEsElPasoCorrecto()
        {
            // Los mensajes reales del guion §6.2.2, contra el contenido del juego y no contra el
            // de prueba: es lo que ve el estudiante.
            var content = UnityEngine.ScriptableObject.CreateInstance<AssemblyContent>();
            var rechazos = new[]
            {
                content.AxleTooEarlyMessage, content.PlankTooEarlyMessage, content.CargoTooEarlyMessage
            };

            Assert.That(content.AxleTooEarlyMessage, Is.EqualTo("El tronco todavía no tiene por dónde entrar el palo."));
            Assert.That(content.PlankTooEarlyMessage, Is.EqualTo("La tabla no tiene sobre qué apoyarse todavía."));
            Assert.That(content.CargoTooEarlyMessage, Is.EqualTo("La caja se caería. Falta algo plano debajo."));

            foreach (var rechazo in rechazos)
            {
                // Orienta sin resolver: ningún rechazo nombra la acción que había que hacer.
                Assert.That(rechazo.ToLowerInvariant(), Does.Not.Contain("mecaniza").And.Not.Contain("perfora")
                    .And.Not.Contain("primero").And.Not.Contain("pon ").And.Not.Contain("coloca"),
                    $"«{rechazo}» dicta el paso correcto en vez de decir qué falta (CP-06)");
            }
        }

        [Test]
        public void AssemblySequence_RF17_NingunMensajeContieneDigitos()
        {
            var content = UnityEngine.ScriptableObject.CreateInstance<AssemblyContent>();
            var mensajes = new[]
            {
                content.SelectedMessage, content.AlreadyWheelMessage, content.SelectNeededMessage,
                content.WheelDrilledMessage, content.BothWheelsMessage, content.AxleFormedMessage,
                content.PlankPlacedMessage, content.CompleteMessage, content.AxleTooEarlyMessage,
                content.PlankTooEarlyMessage, content.CargoTooEarlyMessage, content.MissedMessage
            };

            Assert.That(mensajes.Where(mensaje => mensaje.Any(char.IsDigit)), Is.Empty,
                "nada de cifras en lo que ve el estudiante (RF-17, CP-03)");
            Assert.That(mensajes.Where(string.IsNullOrWhiteSpace), Is.Empty, "y ningún mensaje vacío");
        }
    }
}
