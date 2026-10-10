using Game.Scaffolding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    /// <summary>
    /// El encuadre de la ilustración de la portada del menú y de las tarjetas del menú de niveles
    /// (D9 y D11, 08/10/2026): qué parte del lienzo cae en la ventana.
    /// </summary>
    /// <remarks>
    /// EditMode a propósito: <c>FramedIllustration</c> solo mueve y escala un <c>RectTransform</c>. En
    /// EditMode no se invoca <c>OnEnable</c> (no es <c>ExecuteAlways</c>), así que las pruebas llaman a
    /// <c>Apply</c>. Lo que depende del <c>Canvas</c> real —el bosque de verdad en la escena, a 16:9 y
    /// con el repartidor de tamaño— lo comprueban las pruebas de PlayMode de cada pantalla.
    /// El sprite es de 384 × 108, la proporción del lienzo duplicado del bosque (3840 × 1080) a una
    /// décima: el encuadre solo usa su proporción, no su tamaño en píxeles.
    /// </remarks>
    public class FramedIllustrationTests
    {
        private GameObject _root;
        private Texture2D _texture;
        private Sprite _sprite;

        [TearDown]
        public void DestruirLaVentana()
        {
            if (_root != null)
            {
                Object.DestroyImmediate(_root);
            }

            if (_sprite != null)
            {
                Object.DestroyImmediate(_sprite);
            }

            if (_texture != null)
            {
                Object.DestroyImmediate(_texture);
            }
        }

        // La mitad izquierda del lienzo duplicado en cualquier ventana no más ancha que 16:9: la portada del menú,
        // su versión a 4:3, la tarjeta del nivel y una ventana vertical.
        [TestCase(952f, 952f)]
        [TestCase(668f, 668f)]
        [TestCase(444f, 250f)]
        [TestCase(695f, 1119f)]
        public void FramedIllustration_RF01_ConElFocoACuartoDelLienzoDuplicadoSoloSeVeLaMitadIzquierda(float width, float height)
        {
            var (sut, art) = UnaVentana(new Vector2(width, height), new CameraFraming(new Vector2(0.25f, 0.5f), 1f));

            sut.Apply();

            var (left, right) = Visible(sut, art);
            Assert.That(left, Is.GreaterThanOrEqualTo(-0.001f), "no descubre el borde izquierdo");
            Assert.That(right, Is.LessThanOrEqualTo(IllustrationFraming.MirrorAxis + 0.001f), "y no pasa del eje del espejo");
        }

        [Test]
        public void FramedIllustration_RF01_LaImagenCubreLaVentanaSinDeformarse()
        {
            var (sut, art) = UnaVentana(new Vector2(952f, 952f), new CameraFraming(new Vector2(0.25f, 0.5f), 1f));

            sut.Apply();

            Assert.That(art.sizeDelta, Is.EqualTo(new Vector2(384f, 108f)), "conserva su tamaño nativo");
            Assert.That(art.localScale.x, Is.EqualTo(art.localScale.y), "se escala por igual en los dos ejes");
            Assert.That(art.localScale.x * 108f, Is.GreaterThanOrEqualTo(952f - 0.01f), "y llena la ventana a lo alto");
        }

        // Antes de que el Canvas reparta el tamaño la ventana mide 0 × 0: encuadrar así dejaría la imagen a escala
        // cero y, como Unity no avisa a la imagen de que su ventana cambió, se quedaría invisible (08/10/2026).
        [Test]
        public void FramedIllustration_RF01_UnaVentanaSinTamanoNoDejaLaImagenAEscalaCero()
        {
            var (sut, art) = UnaVentana(Vector2.zero, new CameraFraming(new Vector2(0.25f, 0.5f), 1f));
            var antes = art.localScale;

            sut.Apply();

            Assert.That(art.localScale, Is.EqualTo(antes));
        }

        [Test]
        public void FramedIllustration_RF01_ElEncuadreSeReajustaAlCambiarElTamanoDeLaVentana()
        {
            var (sut, art) = UnaVentana(new Vector2(952f, 952f), new CameraFraming(new Vector2(0.25f, 0.5f), 1f));
            sut.Apply();
            var cuadrada = art.localScale.x;

            ((RectTransform)sut.transform).sizeDelta = new Vector2(1904f, 1904f);
            sut.Apply();

            Assert.That(art.localScale.x, Is.EqualTo(cuadrada * 2f).Within(0.0001f));
        }

        private (FramedIllustration sut, RectTransform art) UnaVentana(Vector2 size, CameraFraming framing)
        {
            _root = new GameObject("Ventana", typeof(RectTransform));
            var window = (RectTransform)_root.transform;
            window.sizeDelta = size;

            var artObject = new GameObject("Arte", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var art = (RectTransform)artObject.transform;
            art.SetParent(window, false);
            _texture = new Texture2D(384, 108);
            _sprite = Sprite.Create(_texture, new Rect(0f, 0f, 384f, 108f), new Vector2(0.5f, 0.5f));
            artObject.GetComponent<Image>().sprite = _sprite;

            var sut = _root.AddComponent<FramedIllustration>();
            sut.Art = artObject.GetComponent<Image>();
            sut.Framing = framing;
            return (sut, art);
        }

        /// <summary>El tramo horizontal de la imagen que cae en la ventana, en fracciones de su ancho (0 = izquierda, 1 = derecha).</summary>
        private static (float left, float right) Visible(FramedIllustration window, RectTransform art)
        {
            var artCorners = new Vector3[4];
            var viewCorners = new Vector3[4];
            art.GetWorldCorners(artCorners);
            ((RectTransform)window.transform).GetWorldCorners(viewCorners);
            var width = artCorners[2].x - artCorners[0].x;
            return ((viewCorners[0].x - artCorners[0].x) / width, (viewCorners[2].x - artCorners[0].x) / width);
        }
    }
}
