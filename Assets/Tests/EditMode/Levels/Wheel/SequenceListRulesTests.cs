using NUnit.Framework;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>
    /// Las dos reglas de la lista «Tu secuencia» que no dependen de uGUI: qué fila queda
    /// seleccionada —la que lleva la papelera— tras retirar un bloque (DEF-GP1-03) y hasta dónde
    /// se desplaza la lista para que el bloque en curso quede a la vista (DEF-GP1-02).
    /// </summary>
    [TestFixture]
    public class SequenceListRulesTests
    {
        [TestCase(0, 3, 0, TestName = "Primera de cuatro: queda la que ocupa su lugar")]
        [TestCase(1, 3, 1, TestName = "Del medio: queda la que ocupa su lugar")]
        [TestCase(3, 3, 2, TestName = "Última: queda la nueva última")]
        [TestCase(0, 1, 0, TestName = "Quedaba una sola: es esa")]
        public void SequenceListRules_RF34_TrasRetirarUnBloqueQuedaSeleccionadoElQueOcupaSuLugar(int retirado, int quedan, int esperado)
        {
            Assert.That(SequenceListRules.SelectionAfterRemoval(retirado, quedan), Is.EqualTo(esperado),
                "con el cajón cerrado alguna fila sigue ofreciendo la papelera: se puede vaciar la secuencia sin abrirlo");
        }

        [Test]
        public void SequenceListRules_RF34_SinBloquesNoHayFilaSeleccionada()
        {
            Assert.That(SequenceListRules.SelectionAfterRemoval(0, 0), Is.EqualTo(-1), "la secuencia vacía no tiene nada que seleccionar");
        }

        [Test]
        public void SequenceListRules_RF32_UnBloqueYaVisibleNoMueveLaLista()
        {
            // Ventana de 400 px desplazada 100: se ve de 100 a 500. La fila ocupa 200..264.
            Assert.That(SequenceListRules.RevealScroll(100f, 200f, 264f, 400f, 8f), Is.EqualTo(100f));
        }

        [Test]
        public void SequenceListRules_RF32_UnBloqueDebajoDeLaVentanaLaBajaHastaDejarloALaVistaConSuMargen()
        {
            // Se ve de 0 a 400; la fila ocupa 600..664: su borde inferior, más el margen, queda al pie de la ventana.
            Assert.That(SequenceListRules.RevealScroll(0f, 600f, 664f, 400f, 8f), Is.EqualTo(664f + 8f - 400f));
        }

        [Test]
        public void SequenceListRules_RF32_UnBloqueEncimaDeLaVentanaLaSubeHastaDejarloALaVistaConSuMargen()
        {
            // Se ve de 500 a 900; la fila ocupa 100..164 (el principio de una lista larga que se quedó al final).
            Assert.That(SequenceListRules.RevealScroll(500f, 100f, 164f, 400f, 8f), Is.EqualTo(100f - 8f));
        }

        [Test]
        public void SequenceListRules_RF32_UnBloqueMasAltoQueLaVentanaMuestraSuBordeSuperior()
        {
            Assert.That(SequenceListRules.RevealScroll(0f, 600f, 1100f, 400f, 8f), Is.EqualTo(600f - 8f),
                "si no cabe entero, lo que importa es empezar a leerlo por arriba");
        }

        [Test]
        public void SequenceListRules_RF32_RecorrerLaListaPasoAPasoNuncaDejaElBloqueEnCursoFueraDeLaVentana()
        {
            const float alto = 64f;
            const float ventana = 300f;
            const float margen = 8f;
            var scroll = 700f; // la lista quedó abajo del todo tras añadir el último bloque

            for (var i = 0; i < 20; i++)
            {
                var arriba = 16f + i * alto;
                scroll = SequenceListRules.RevealScroll(scroll, arriba, arriba + alto, ventana, margen);
                scroll = scroll < 0f ? 0f : scroll; // el llamador acota: no hay nada por encima del principio

                Assert.That(arriba - scroll, Is.GreaterThanOrEqualTo(0f), $"Paso_{i}: su borde superior se ve");
                Assert.That(arriba + alto - scroll, Is.LessThanOrEqualTo(ventana), $"Paso_{i}: su borde inferior se ve");
            }
        }
    }
}
