using NUnit.Framework;
using UnityEngine;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// La ilustración cubre la ventana sin deformarse, a cualquier resolución del arte y de la
    /// pantalla (RNF-03, RNF-23).
    /// </summary>
    public class IllustrationFramingTests
    {
        private static readonly Vector2 Arte = new Vector2(1599f, 899f); // el bosque de hoy, a baja resolución

        [TestCase(1920f, 1080f, TestName = "IllustrationFraming_RNF03_CubreUnaVentanaPanoramica")]
        [TestCase(800f, 600f, TestName = "IllustrationFraming_RNF03_CubreUnaVentanaCuatroTres")]
        [TestCase(600f, 1200f, TestName = "IllustrationFraming_RNF03_CubreUnaVentanaVertical")]
        public void IllustrationFraming_RNF03_CubreLaVentanaSinDeformarse(float ancho, float alto)
        {
            var ventana = new Vector2(ancho, alto);

            var tamano = IllustrationFraming.CoverSize(Arte, ventana, 1f);

            Assert.That(tamano.x, Is.GreaterThanOrEqualTo(ventana.x - 0.01f), "cubre a lo ancho");
            Assert.That(tamano.y, Is.GreaterThanOrEqualTo(ventana.y - 0.01f), "cubre a lo alto");
            Assert.That(tamano.x / tamano.y, Is.EqualTo(Arte.x / Arte.y).Within(0.001f), "sin deformar");
            Assert.That(Mathf.Min(tamano.x - ventana.x, tamano.y - ventana.y), Is.EqualTo(0f).Within(0.01f),
                "y sin sobrar más de lo necesario: uno de los dos ejes encaja justo");
        }

        [Test]
        public void IllustrationFraming_RNF23_SustituirElArteAOtraResolucionNoCambiaElEncuadre()
        {
            var ventana = new Vector2(1920f, 1080f);
            var definitivo = Arte * 2.4f; // el mismo dibujo, entregado más grande

            var hoy = IllustrationFraming.CoverSize(Arte, ventana, 1.2f);
            var manana = IllustrationFraming.CoverSize(definitivo, ventana, 1.2f);

            Assert.That(manana.x, Is.EqualTo(hoy.x).Within(0.01f));
            Assert.That(manana.y, Is.EqualTo(hoy.y).Within(0.01f));
        }

        [Test]
        public void IllustrationFraming_RF05_ElAcercamientoNuncaDescubreBordes()
        {
            var ventana = new Vector2(1920f, 1080f);

            var alejado = IllustrationFraming.CoverSize(Arte, ventana, 0.5f);
            var cubierto = IllustrationFraming.CoverSize(Arte, ventana, 1f);

            Assert.That(alejado, Is.EqualTo(cubierto), "un zoom menor que uno se lee como uno");
        }

        [Test]
        public void IllustrationFraming_RF05_ElFocoQuedaEnElCentroCuandoHaySitio()
        {
            var ventana = new Vector2(1920f, 1080f);
            var tamano = IllustrationFraming.CoverSize(Arte, ventana, 2f);

            var desplazamiento = IllustrationFraming.Offset(tamano, ventana, new Vector2(0.25f, 0.5f));

            // El punto al cuarto de la imagen está a 0.25·ancho a la izquierda del centro: para
            // centrarlo la imagen se corre esa misma distancia a la derecha.
            Assert.That(desplazamiento.x, Is.EqualTo(0.25f * tamano.x).Within(0.01f));
            Assert.That(desplazamiento.y, Is.EqualTo(0f).Within(0.01f));
        }

        [Test]
        public void IllustrationFraming_RF05_ElFocoEnUnaEsquinaSeRecortaAlBorde()
        {
            var ventana = new Vector2(1920f, 1080f);
            var tamano = IllustrationFraming.CoverSize(Arte, ventana, 1.5f);

            var desplazamiento = IllustrationFraming.Offset(tamano, ventana, new Vector2(0f, 1f));

            // Pedir la esquina no puede mostrar el vacío de fuera de la imagen: se llega hasta
            // donde el borde de la imagen coincide con el de la ventana y ahí se para.
            Assert.That(desplazamiento.x, Is.EqualTo((tamano.x - ventana.x) / 2f).Within(0.01f));
            Assert.That(desplazamiento.y, Is.EqualTo(-(tamano.y - ventana.y) / 2f).Within(0.01f));
        }

        [Test]
        public void IllustrationFraming_RF05_SinAcercamientoNoHayDesplazamientoPosible()
        {
            var ventana = Arte; // exactamente la proporción del arte: no sobra ni un píxel
            var tamano = IllustrationFraming.CoverSize(Arte, ventana, 1f);

            var desplazamiento = IllustrationFraming.Offset(tamano, ventana, new Vector2(0f, 0f));

            Assert.That(desplazamiento, Is.EqualTo(Vector2.zero));
        }

        private static readonly Vector2 Entorno = new Vector2(3198f, 899f); // el bosque espejado

        [Test]
        public void IllustrationFraming_RF05_ElZoomDeCierreDelBosqueDejaLaVistaConLaQueAbreLaEscena22()
        {
            var ventana = new Vector2(1920f, 1080f);
            var juego = new CameraFraming(new Vector2(0.27f, 0.5f), 1.18f);
            var cierre = new CameraFraming(new Vector2(0.232f, 0.42f), 1.58f);

            var (pivote, escala) = IllustrationFraming.ScaleAbout(Entorno, ventana, juego, cierre);

            // Escalar la pantalla entera con ese pivote equivale a encuadrar directamente el cierre:
            // el punto de la ilustración que queda en el centro es el foco de destino, al zoom pedido.
            var tamano = IllustrationFraming.CoverSize(Entorno, ventana, juego.Zoom);
            var centro = -IllustrationFraming.Offset(tamano, ventana, juego.Focus) / tamano + new Vector2(0.5f, 0.5f);
            var pivoteEnVentana = (pivote - new Vector2(0.5f, 0.5f)) * ventana;
            var centroTrasEscalar = pivoteEnVentana + (Vector2.zero - pivoteEnVentana) / escala;
            var focoVisto = centro + centroTrasEscalar / tamano;

            Assert.That(escala, Is.EqualTo(cierre.Zoom / juego.Zoom).Within(0.001f));
            Assert.That(focoVisto.x, Is.EqualTo(cierre.Focus.x).Within(0.001f), "el foco de la 2.2 queda en el centro");
            Assert.That(focoVisto.y, Is.EqualTo(cierre.Focus.y).Within(0.001f));
        }

        [Test]
        public void IllustrationFraming_RF05_AvisaDelFocoQueSeRecortaEnSilencio()
        {
            // Los tres encuadres de Camara_Narrativa_N2.md §1 que no se veían como estaban escritos.
            var bajo = new CameraFraming(new Vector2(0.5f, 0.33f), 1.05f);
            var alEste = new CameraFraming(new Vector2(0.84f, 0.4f), 1.5f);
            var enElBorde = new CameraFraming(new Vector2(0.2f, 0.5f), 1f);
            var bien = new CameraFraming(new Vector2(0.27f, 0.5f), 1.18f);

            Assert.That(IllustrationFraming.Warnings(bajo, null, false), Has.Exactly(1).Contains("y=0.330"));
            Assert.That(IllustrationFraming.Warnings(alEste, null, false), Has.Exactly(1).Contains("x=0.840"));
            Assert.That(IllustrationFraming.Warnings(enElBorde, null, false), Has.Exactly(1).Contains("x=0.200"));
            Assert.That(IllustrationFraming.Warnings(bien, null, false), Is.Empty);
        }

        [Test]
        public void IllustrationFraming_RF05_AvisaDeLaLineaDeArbolesFueraYDelEjeDelEspejo()
        {
            var pasto = new CameraFraming(new Vector2(0.3f, 0.3f), 1.8f); // 0.3 + 0.278 < 0.66
            var sobreElEje = new CameraFraming(new Vector2(0.45f, 0.5f), 1.5f); // [0.283, 0.617] contiene 0.5
            var oeste = new CameraFraming(new Vector2(0.3f, 0.5f), 1.6f);
            var este = new CameraFraming(new Vector2(0.7f, 0.5f), 1.6f);

            Assert.That(IllustrationFraming.Warnings(pasto, null, true), Has.Exactly(1).Contains("árboles"));
            Assert.That(IllustrationFraming.Warnings(sobreElEje, null, true), Has.Exactly(1).Contains("cruza el eje"));
            Assert.That(IllustrationFraming.Warnings(este, oeste, true), Has.Exactly(1).Contains("paneo"));
            Assert.That(IllustrationFraming.Warnings(este, null, true), Is.Empty, "solo en el claro este no hay aviso");
            Assert.That(IllustrationFraming.Warnings(pasto, null, false), Is.Empty,
                "las dos reglas son del entorno espejado, no del motor");
        }
    }
}
