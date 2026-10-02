using System.Linq;
using NUnit.Framework;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// El orden de dibujo por profundidad de la orilla (DA83, INC-118): lo que está más abajo —más
    /// cerca de la cámara— se dibuja delante de lo que está más arriba. Es la pareja de la escala
    /// por profundidad; la escena la cumple en <c>BuildZoneTests</c>.
    /// </summary>
    public class DepthOrderTests
    {
        [Test]
        public void DepthOrder_DA83_LoQueEstaMasAbajoSeDibujaDelante()
        {
            var entradas = new[] { ("lejos", 0.33f), ("cerca", 0.06f), ("en medio", 0.19f), ("familia", 0.31f) };

            var deAtrasAdelante = DepthOrder.BackToFront(entradas);

            Assert.That(deAtrasAdelante, Is.EqualTo(new[] { "lejos", "familia", "en medio", "cerca" }),
                "de atrás hacia delante: lo más alto primero, lo más bajo al final");
        }

        [Test]
        public void DepthOrder_DA83_ALaMismaAlturaConservaElOrdenQueTraia()
        {
            var entradas = new[] { ("Papa", 0.31f), ("Nina", 0.31f), ("Nino", 0.31f), ("Mama", 0.31f) };

            Assert.That(DepthOrder.BackToFront(entradas), Is.EqualTo(new[] { "Papa", "Nina", "Nino", "Mama" }),
                "a igual altura nadie salta sobre otro: un orden que cambiara entre cuadros haría parpadear a la familia");
            Assert.That(DepthOrder.BackToFront(entradas.Reverse()), Is.EqualTo(new[] { "Mama", "Nino", "Nina", "Papa" }),
                "y lo que traía es lo que se respeta, sea cual sea");
        }
    }
}
