using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding.Tests
{
    /// <summary>La sombra de gota de los objetos (Dirección de Arte §5.3).</summary>
    [TestFixture]
    public class PropShadowTests
    {
        private GameObject _suelo;
        private RectTransform _rect;
        private Image _image;

        [SetUp]
        public void SetUp()
        {
            _suelo = new GameObject("Suelo", typeof(RectTransform));
            var objeto = new GameObject("Objeto", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            _rect = (RectTransform)objeto.transform;
            _rect.SetParent(_suelo.transform, false);
            _rect.sizeDelta = new Vector2(200f, 200f);
            _image = objeto.GetComponent<Image>();
            _image.preserveAspect = true;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_suelo);
        }

        [Test]
        public void PropShadow_RNF23_LaSombraSeDibujaDetrasDelObjetoYNoEncima()
        {
            var antes = new GameObject("Fondo", typeof(RectTransform));
            antes.transform.SetParent(_suelo.transform, false);
            antes.transform.SetAsFirstSibling();

            var sombra = Adjuntar("prop_n2_caja_suelo");

            Assert.That(sombra.ShadowRect.IsChildOf(_rect), Is.False,
                "en uGUI un hijo se pinta encima del gráfico de su padre: la sombra no puede colgar del objeto");
            Assert.That(sombra.Layer.name, Is.EqualTo(PropShadow.LayerName));
            Assert.That(sombra.Layer.parent, Is.SameAs(_suelo.transform), "la capa es hermana del objeto");
            Assert.That(sombra.Layer.GetSiblingIndex(), Is.EqualTo(_rect.GetSiblingIndex() - 1),
                "y va justo antes que él: se pinta debajo del objeto y encima de lo que ya había");
            Assert.That(sombra.ShadowGraphic.raycastTarget, Is.False, "la sombra no se lleva los clics");
        }

        [Test]
        public void PropShadow_RNF23_SiElObjetoPasaDelanteDeLaCapaLaCapaSeAdelanta()
        {
            var sombra = Adjuntar("prop_n2_caja_suelo");

            _rect.SetAsFirstSibling(); // como quien reordena los objetos por profundidad
            sombra.ApplyTransform();

            Assert.That(sombra.Layer.GetSiblingIndex(), Is.LessThan(_rect.GetSiblingIndex()),
                "la sombra sigue debajo del objeto");
        }

        [Test]
        public void PropShadow_RNF23_ElObjetoEnPicadoProyectaSuPropiaSiluetaCorridaHaciaAbajoYALaDerecha()
        {
            foreach (var nombre in new[] { "prop_n2_caja_suelo", "prop_n3_tronco", "prop_n2_pieza_4", "prop_n2_carretilla_e5", "prop_n3_balsa_cruzando" })
            {
                var sombra = Adjuntar(nombre);

                Assert.That(sombra.Kind, Is.EqualTo(PropShadowKind.Silhouette), $"{nombre} se ve en picado");
                var copia = sombra.ShadowGraphic as Image;
                Assert.That(copia, Is.Not.Null);
                Assert.That(copia.sprite, Is.SameAs(_image.sprite), $"la sombra de {nombre} tiene la forma de su dibujo");
                Assert.That(copia.preserveAspect, Is.EqualTo(_image.preserveAspect));
                Assert.That(copia.color, Is.EqualTo(new Color(0f, 0f, 0f, 0.25f)), "negro al 25 % (DA §5.3)");
                Assert.That(sombra.ShadowRect.rect.size, Is.EqualTo(_rect.rect.size));

                var desplazamiento = sombra.ShadowRect.position - _rect.position;
                Assert.That(desplazamiento.x, Is.GreaterThan(0f), "la luz llega de arriba a la izquierda: la sombra cae a la derecha");
                Assert.That(desplazamiento.y, Is.LessThan(0f), "y hacia abajo");
                Assert.That(desplazamiento.magnitude, Is.LessThan(_rect.rect.height * 0.1f), "pegada al objeto, no desprendida");
            }
        }

        [Test]
        public void PropShadow_RNF23_LaSiluetaGiraYSeReflejaConElObjetoPeroLaLuzNoCambia()
        {
            var sombra = Adjuntar("prop_n3_tronco");
            var enReposo = sombra.ShadowRect.position - _rect.position;

            foreach (var espejo in new[] { false, true })
            {
                foreach (var giro in new[] { 0f, 30f, 90f, 215f })
                {
                    _rect.localScale = new Vector3(espejo ? -1f : 1f, 1f, 1f);
                    _rect.localRotation = Quaternion.Euler(0f, 0f, giro);
                    sombra.ApplyTransform();

                    Assert.That(Quaternion.Angle(sombra.ShadowRect.rotation, _rect.rotation), Is.LessThan(0.1f),
                        $"espejo={espejo}, giro={giro}°: la silueta gira con el tronco");
                    Assert.That(Vector3.Distance(sombra.ShadowRect.lossyScale, _rect.lossyScale), Is.LessThan(1e-4f), "y se refleja con él");
                    Assert.That(Vector3.Distance(sombra.ShadowRect.position - _rect.position, enReposo), Is.LessThan(0.01f),
                        "pero cae siempre hacia el mismo lado: la luz no gira con el objeto");
                }
            }
        }

        [Test]
        public void PropShadow_RNF23_ElTroncoQueRuedaProyectaLaSiluetaDelCilindro()
        {
            var vista = ScriptableObject.CreateInstance<RollingLogLook>();
            try
            {
                _image.sprite = Dibujo("prop_n2_tronco_a");
                var tronco = RollingLog.Attach(_image, vista);

                var sombra = PropShadow.Attach(_rect.gameObject, _image.sprite);

                var cilindro = sombra.ShadowGraphic as RollingLog;
                Assert.That(cilindro, Is.Not.Null, "el tronco que rueda no tiene sprite que copiar: su sombra es el cilindro");
                Assert.That(cilindro, Is.Not.SameAs(tronco));
                Assert.That(cilindro.SilhouetteOnly, Is.True, "solo el contorno");
                Assert.That(cilindro.Face, Is.SameAs(_image), "que lee el giro del mismo objeto");
                Assert.That(cilindro.color, Is.EqualTo(new Color(0f, 0f, 0f, 0.25f)));
                Assert.That(cilindro.transform.IsChildOf(_rect), Is.False, "y que no cuelga del objeto");
                Assert.That(tronco.SilhouetteOnly, Is.False, "el tronco se sigue dibujando entero");
            }
            finally
            {
                Object.DestroyImmediate(vista);
            }
        }

        [Test]
        public void PropShadow_RNF23_LasPlantasDePieProyectanUnaElipseQueNoGira()
        {
            var sombra = Adjuntar("prop_n2_planta_a");

            Assert.That(sombra.Kind, Is.EqualTo(PropShadowKind.Contact), "la planta está de pie: elipse en la base");
            var elipse = (Image)sombra.ShadowGraphic;
            Assert.That(elipse.sprite, Is.Not.SameAs(_image.sprite));
            Assert.That(sombra.ShadowRect.rect.width, Is.GreaterThan(sombra.ShadowRect.rect.height), "plana sobre el suelo");

            _rect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            sombra.ApplyTransform();
            Assert.That(Mathf.DeltaAngle(0f, sombra.ShadowRect.eulerAngles.z), Is.EqualTo(0f).Within(0.1f),
                "la elipse está en el plano del suelo y no gira con la planta");
        }

        [Test]
        public void PropShadow_RNF23_LaElipseSeApoyaEnLaBaseDelDibujoYNoEnElMargenDelPng()
        {
            var entera = AlturaDeLaElipse(Dibujo("prop_n2_planta_a", desdeFila: 0));
            var conMargen = AlturaDeLaElipse(Dibujo("prop_n2_planta_b", desdeFila: 32));

            Assert.That(conMargen, Is.GreaterThan(entera + 50f),
                "con la mitad inferior del PNG transparente la elipse sube hasta donde empieza el dibujo");
        }

        [Test]
        public void PropShadow_RNF23_ExcluyeSiluetasBalsaHundidaYFuego()
        {
            foreach (var nombre in new[]
                     {
                         "prop_n3_tronco_silueta", "prop_n3_balsa_hundida", "fuego_normal_nivel_1_0000",
                         "prop_n1_fuego_cenital", "humo_nivel_1_0009", "prop_n1_monton_hojas", "env_final_fogatas",
                         "hoguera_central",
                     })
            {
                var sprite = Dibujo(nombre);
                Assert.That(PropShadow.IsExcluded(sprite), Is.True, $"{nombre} no proyecta sombra");
                Assert.That(PropShadow.Attach(_rect.gameObject, sprite), Is.Null);
                Assert.That(_suelo.transform.Find(PropShadow.LayerName), Is.Null, "y no deja capa");
            }

            Assert.That(PropShadow.IsExcluded(Dibujo("prop_n3_balsa_cruzando")), Is.False,
                "la balsa que cruza sí: está en picado sobre el agua");
        }

        [Test]
        public void PropShadow_RNF23_AlLevantarseElObjetoLaSombraSeQuedaEnElSueloYSeEncoge()
        {
            const float alto = 120f;
            var sombra = Adjuntar("prop_n2_caja_suelo");
            var enSuelo = sombra.ShadowRect.position;

            _rect.anchoredPosition += new Vector2(0f, alto / 2f);
            sombra.UpdateMotion(alto / 2f, alto);

            Assert.That(Vector3.Distance(sombra.ShadowRect.position, enSuelo), Is.LessThan(0.01f),
                "el objeto sube y su sombra se queda donde estaba");
            Assert.That(sombra.ShadowRect.localScale.x, Is.LessThan(1f), "más pequeña");
            Assert.That(sombra.ShadowGraphic.color.a, Is.LessThan(0.25f), "y más clara");

            _rect.anchoredPosition += new Vector2(0f, alto / 2f);
            sombra.UpdateMotion(alto, alto);
            Assert.That(sombra.ShadowRect.localScale.x, Is.EqualTo(0.35f).Within(0.01f), "en lo más alto, la mínima");

            _rect.anchoredPosition -= new Vector2(0f, alto);
            sombra.UpdateMotion(0f, alto);
            Assert.That(Vector3.Distance(sombra.ShadowRect.position, enSuelo), Is.LessThan(0.01f));
            Assert.That(sombra.ShadowRect.localScale.x, Is.EqualTo(1f).Within(0.001f), "de vuelta al suelo, entera");
            Assert.That(sombra.ShadowGraphic.color.a, Is.EqualTo(0.25f).Within(0.001f));
        }

        [Test]
        public void PropShadow_RNF23_LaSombraSeApagaConElObjeto()
        {
            var sombra = Adjuntar("prop_n2_caja_suelo");

            _rect.gameObject.SetActive(false);
            sombra.ApplyTransform();
            Assert.That(sombra.ShadowGraphic.enabled, Is.False, "el objeto recogido no deja su sombra en el suelo");

            _rect.gameObject.SetActive(true);
            sombra.ApplyTransform();
            Assert.That(sombra.ShadowGraphic.enabled, Is.True);
        }

        private PropShadow Adjuntar(string nombre)
        {
            var anterior = _rect.GetComponent<PropShadow>();
            if (anterior != null)
            {
                Object.DestroyImmediate(anterior.ShadowRect.gameObject);
                Object.DestroyImmediate(anterior);
            }

            _rect.localScale = Vector3.one;
            _rect.localRotation = Quaternion.identity;
            _image.sprite = Dibujo(nombre);
            return PropShadow.Attach(_rect.gameObject, _image.sprite);
        }

        /// <summary>La altura del centro de la elipse respecto al centro del objeto.</summary>
        private float AlturaDeLaElipse(Sprite sprite)
        {
            var anterior = _rect.GetComponent<PropShadow>();
            if (anterior != null)
            {
                Object.DestroyImmediate(anterior.ShadowRect.gameObject);
                Object.DestroyImmediate(anterior);
            }

            _image.sprite = sprite;
            var sombra = PropShadow.Attach(_rect.gameObject, sprite);
            return sombra.ShadowRect.position.y - _rect.position.y;
        }

        /// <summary>Un sprite de 64 × 64 opaco desde <paramref name="desdeFila"/> hacia arriba y transparente debajo.</summary>
        private static Sprite Dibujo(string nombre, int desdeFila = 0)
        {
            const int lado = 64;
            var textura = new Texture2D(lado, lado, TextureFormat.RGBA32, false);
            var pixeles = new Color32[lado * lado];
            for (var y = 0; y < lado; y++)
            {
                for (var x = 0; x < lado; x++)
                {
                    pixeles[y * lado + x] = y >= desdeFila ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
                }
            }

            textura.SetPixels32(pixeles);
            textura.Apply();
            var sprite = UnityEngine.Sprite.Create(textura, new Rect(0f, 0f, lado, lado), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.Tight);
            sprite.name = nombre;
            return sprite;
        }
    }
}
