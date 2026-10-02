using System;
using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding.Tests
{
    [TestFixture]
    public class PropShadowTests
    {
        private GameObject _holder;
        private RectTransform _rect;
        private Sprite _regularSprite;
        private Sprite _siluetaSprite;
        private Sprite _balsaSprite;

        [SetUp]
        public void SetUp()
        {
            _holder = new GameObject("TestProp", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            _rect = (RectTransform)_holder.transform;
            _rect.sizeDelta = new Vector2(200f, 200f);

            var tex = new Texture2D(32, 32);
            _regularSprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            _regularSprite.name = "prop_n2_caja_suelo";

            _siluetaSprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            _siluetaSprite.name = "prop_n3_tronco_silueta";

            _balsaSprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            _balsaSprite.name = "prop_n3_balsa_cruzando";
        }

        [TearDown]
        public void TearDown()
        {
            if (_holder != null)
            {
                UnityEngine.Object.DestroyImmediate(_holder);
            }
        }

        [Test]
        public void PropShadow_RNF23_CreaSombraDeGotaComoHijoConSpriteCirculoYOpacidad25PorCiento()
        {
            var shadow = PropShadow.Attach(_holder, _regularSprite, _rect.sizeDelta);

            Assert.That(shadow, Is.Not.Null, "debe adjuntar el componente PropShadow");
            Assert.That(shadow.ShadowRect, Is.Not.Null, "debe crear el RectTransform de la sombra");
            Assert.That(shadow.ShadowImage, Is.Not.Null, "debe crear la Image de la sombra");
            Assert.That(shadow.ShadowRect.name, Is.EqualTo("Sombra"));
            Assert.That(shadow.ShadowRect.parent, Is.SameAs(_rect), "la sombra es hija del prop");
            Assert.That(shadow.ShadowImage.sprite, Is.Not.Null, "posee sprite circular");
            Assert.That(shadow.ShadowImage.color.r, Is.EqualTo(0f));
            Assert.That(shadow.ShadowImage.color.g, Is.EqualTo(0f));
            Assert.That(shadow.ShadowImage.color.b, Is.EqualTo(0f));
            Assert.That(shadow.ShadowImage.color.a, Is.EqualTo(0.25f).Within(0.01f), "opacidad del 25 % según DA §5.3");
            Assert.That(shadow.ShadowRect.GetSiblingIndex(), Is.EqualTo(0), "se posiciona como primer hijo (detrás del sprite)");
        }

        [Test]
        public void PropShadow_RNF23_ExcluyeSpritesDeSilueta()
        {
            var shadow = PropShadow.Attach(_holder, _siluetaSprite, _rect.sizeDelta);

            Assert.That(shadow, Is.Null, "no debe crear sombra para sprites de silueta");
            Assert.That(_holder.transform.Find("Sombra"), Is.Null, "no debe existir el hijo Sombra");
            Assert.That(PropShadow.IsExcluded(_siluetaSprite), Is.True);
        }

        [Test]
        public void PropShadow_RNF23_ExcluyeSpritesDeLaBalsa()
        {
            var shadow = PropShadow.Attach(_holder, _balsaSprite, _rect.sizeDelta);

            Assert.That(shadow, Is.Null, "no debe crear sombra para la balsa");
            Assert.That(_holder.transform.Find("Sombra"), Is.Null, "no debe existir el hijo Sombra");
            Assert.That(PropShadow.IsExcluded(_balsaSprite), Is.True);
        }

        [Test]
        public void PropShadow_RNF23_ExcluyeSpritesDeFogataYFuego()
        {
            var tex = new Texture2D(32, 32);
            var fuegoNormal = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            fuegoNormal.name = "fuego_normal_nivel_1_0000";

            var fuegoCenital = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            fuegoCenital.name = "prop_n1_fuego_cenital";

            var humo = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            humo.name = "humo_nivel_1_0009";

            var montonHojas = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            montonHojas.name = "prop_n1_monton_hojas";

            var fogata = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            fogata.name = "env_final_fogatas";

            var hoguera = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            hoguera.name = "hoguera_central";

            var spritesFogata = new[] { fuegoNormal, fuegoCenital, humo, montonHojas, fogata, hoguera };

            foreach (var sprite in spritesFogata)
            {
                Assert.That(PropShadow.IsExcluded(sprite), Is.True, $"sprite {sprite.name} debe estar excluido");

                var shadow = PropShadow.Attach(_holder, sprite, _rect.sizeDelta);
                Assert.That(shadow, Is.Null, $"no debe crear sombra de gota para {sprite.name}");
                Assert.That(_holder.transform.Find("Sombra"), Is.Null, $"no debe existir el hijo Sombra para {sprite.name}");
            }
        }

        [Test]
        public void PropShadow_RNF23_SombraSeMantieneEnElSueloCuandoElObjetoSeLevanta()
        {
            var shadow = PropShadow.Attach(_holder, _regularSprite, _rect.sizeDelta);
            var baseLocalY = shadow.BaseLocalY;

            // Simula el objeto levantándose 60 píxeles del suelo
            const float liftHeight = 60f;
            const float maxLift = 120f;
            _rect.anchoredPosition = new Vector2(100f, liftHeight);

            shadow.UpdateMotion(liftHeight, maxLift, 0f);

            // Al subir el padre +liftHeight, la sombra se desplaza localmente -liftHeight,
            // de modo que su posición absoluta en Y respecto al suelo es:
            // anchoredPosition.y del padre + anchoredPosition.y de la sombra = liftHeight + (baseLocalY - liftHeight) = baseLocalY
            var yEnSuelo = _rect.anchoredPosition.y + shadow.ShadowRect.anchoredPosition.y;
            Assert.That(yEnSuelo, Is.EqualTo(baseLocalY).Within(0.01f),
                "la sombra se queda exactamente en el suelo en la posición que tenía antes de levantarse");
        }

        [Test]
        public void PropShadow_RNF23_TamanoDeLaSombraDisminuyeConformeElObjetoSeLevanta()
        {
            var shadow = PropShadow.Attach(_holder, _regularSprite, _rect.sizeDelta);
            const float maxLift = 100f;

            // En el suelo
            shadow.UpdateMotion(0f, maxLift, 0f);
            var escalaEnSuelo = shadow.ShadowRect.localScale.x;
            Assert.That(escalaEnSuelo, Is.EqualTo(1.0f).Within(0.01f), "en el suelo escala es 1.0");

            // A media altura
            shadow.UpdateMotion(50f, maxLift, 0f);
            var escalaMedio = shadow.ShadowRect.localScale.x;
            Assert.That(escalaMedio, Is.LessThan(escalaEnSuelo), "a media altura la sombra se reduce");

            // En la altura máxima
            shadow.UpdateMotion(100f, maxLift, 0f);
            var escalaCima = shadow.ShadowRect.localScale.x;
            Assert.That(escalaCima, Is.LessThan(escalaMedio), "en la cima la sombra es mínima");
            Assert.That(escalaCima, Is.EqualTo(0.35f).Within(0.05f), "llega al factor mínimo de escala");
        }

        [Test]
        public void PropShadow_RNF23_LaSombraNoGiraCuandoElObjetoRueda()
        {
            var shadow = PropShadow.Attach(_holder, _regularSprite, _rect.sizeDelta);

            // Simula el objeto rodando e inclinándose 45 grados
            _rect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            shadow.UpdateMotion(0f, 100f, 45f);

            // La rotación combinada (padre * local) debe ser 0 grados (plano horizontal del suelo)
            var rotacionMundialZ = _rect.localEulerAngles.z + shadow.ShadowRect.localEulerAngles.z;
            Assert.That(Mathf.DeltaAngle(0f, rotacionMundialZ), Is.EqualTo(0f).Within(0.1f),
                "la sombra proyectada se mantiene horizontal en el suelo sin girar con el objeto");
        }

        [Test]
        public void PropShadow_RNF23_SombraAcompanaElDesplazamientoYLaEscalaDelObjeto()
        {
            var shadow = PropShadow.Attach(_holder, _regularSprite, _rect.sizeDelta);

            // Desplazamiento por el suelo
            _rect.anchoredPosition = new Vector2(250f, 50f);
            // Sombra es hija de _rect, por lo que su posición mundial acompaña al objeto automáticamente
            Assert.That(shadow.ShadowRect.parent, Is.SameAs(_rect));

            // Cambio de tamaño del objeto (escala)
            _rect.localScale = new Vector3(1.5f, 1.5f, 1f);
            // Al ser hija, su escala efectiva proyectada escala idénticamente en 1.5x
            Assert.That(shadow.ShadowRect.lossyScale.x, Is.EqualTo(_rect.lossyScale.x).Within(0.01f),
                "la sombra de gota escala proporcionalmente con el objeto al aumentar o disminuir su tamaño");
        }

        [Test]
        public void PropShadow_RNF23_SombraSePosicionaEnLaBaseDelDibujoYNoEnElFondoDelCanvas()
        {
#if UNITY_EDITOR
            var herramientaSprite = UnityEditor.AssetDatabase
                .LoadAllAssetsAtPath("Assets/Game/Art/Props/Wheel/prop_n2_herramienta_c.png")
                .OfType<Sprite>()
                .FirstOrDefault();

            var cajaSprite = UnityEditor.AssetDatabase
                .LoadAllAssetsAtPath("Assets/Game/Art/Props/Wheel/prop_n2_caja_suelo.png")
                .OfType<Sprite>()
                .FirstOrDefault();

            if (herramientaSprite != null && cajaSprite != null)
            {
                var shadowHerramienta = PropShadow.Attach(_holder, herramientaSprite, _rect.sizeDelta);
                var baseYHerramienta = shadowHerramienta.BaseLocalY;

                // Destruimos la sombra previa para probar con la caja
                UnityEngine.Object.DestroyImmediate(shadowHerramienta);
                UnityEngine.Object.DestroyImmediate(_holder.transform.Find("Sombra").gameObject);

                var shadowCaja = PropShadow.Attach(_holder, cajaSprite, _rect.sizeDelta);
                var baseYCaja = shadowCaja.BaseLocalY;

                // La herramienta tiene ~40 % de margen transparente inferior en su PNG,
                // mientras que la caja llega hasta abajo. La sombra de la herramienta debe colocarse
                // considerablemente más arriba que la de la caja (en la base del dibujo y no en el fondo del PNG).
                Assert.That(baseYHerramienta, Is.GreaterThan(baseYCaja + 30f),
                    "la sombra de la herramienta con margen transparente inferior se sitúa en la base del objeto y no en el fondo del PNG");
                Assert.That(baseYHerramienta, Is.GreaterThan(-40f),
                    "la sombra de la herramienta no cae al fondo del canvas (-84 o -100 px), sino cerca de -23 px");
            }
#endif
        }

        [Test]
        public void PropShadow_RNF23_LaSombraPermaneceHorizontalYEnElSueloCuandoElTroncoRuedaCompleto()
        {
            var shadow = PropShadow.Attach(_holder, _regularSprite, _rect.sizeDelta);
            var baseLocalY = shadow.BaseLocalY;
            var angles = new[] { 0f, 30f, 45f, 90f, 180f, 270f, 360f };

            // 1. Tronco sin espejo (escala 1.0)
            _rect.localScale = Vector3.one;
            foreach (var angle in angles)
            {
                _rect.localRotation = Quaternion.Euler(0f, 0f, angle);
                shadow.UpdateMotion(0f, 80f, angle);

                var rotZ = Mathf.DeltaAngle(0f, shadow.ShadowRect.eulerAngles.z);
                Assert.That(rotZ, Is.EqualTo(0f).Within(0.1f),
                    $"la sombra debe permanecer horizontal (0°) con giro {angle}°");

                Assert.That(shadow.ShadowRect.position.x, Is.EqualTo(_rect.position.x).Within(0.1f),
                    $"la sombra no debe orbitar horizontalmente con giro {angle}°");

                Assert.That(shadow.ShadowRect.position.y, Is.EqualTo(_rect.position.y + baseLocalY).Within(0.1f),
                    $"la sombra debe permanecer en el suelo debajo del objeto con giro {angle}°");
            }

            // 2. Tronco con espejo (escala negativa en X, como en el Nivel 2)
            _rect.localScale = new Vector3(-1.5f, 1.5f, 1f);
            foreach (var angle in angles)
            {
                _rect.localRotation = Quaternion.Euler(0f, 0f, angle);
                shadow.UpdateMotion(0f, 80f, angle);

                var rotZ = Mathf.DeltaAngle(0f, shadow.ShadowRect.eulerAngles.z);
                Assert.That(rotZ, Is.EqualTo(0f).Within(0.1f),
                    $"la sombra debe permanecer horizontal (0°) con espejo y giro {angle}°");

                Assert.That(shadow.ShadowRect.position.x, Is.EqualTo(_rect.position.x).Within(0.1f),
                    $"la sombra no debe orbitar horizontalmente con espejo y giro {angle}°");

                Assert.That(shadow.ShadowRect.position.y, Is.EqualTo(_rect.position.y + baseLocalY * 1.5f).Within(0.1f),
                    $"la sombra debe permanecer en el suelo debajo del objeto con espejo y giro {angle}°");
            }
        }

        [Test]
        public void PropShadow_RNF23_SombraDeTroncoAlargadaYConMismoAnguloAlRodar()
        {
            var tex = new Texture2D(32, 32);
            var troncoSprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            troncoSprite.name = "prop_n3_tronco";

            var shadow = PropShadow.Attach(_holder, troncoSprite, _rect.sizeDelta);
            var baseLocalY = shadow.BaseLocalY;
            var angles = new[] { 0f, 30f, 45f, 90f, 180f, 270f, 360f };

            Assert.That(shadow.BaseShadowAngle, Is.EqualTo(35f), "el ángulo base de los troncos es 35°");
            Assert.That(shadow.BaseSize.x, Is.GreaterThan(shadow.BaseSize.y * 2.5f), "la sombra de los troncos es alargada");

            // 1. Tronco sin espejo (ángulo +35°)
            _rect.localScale = Vector3.one;
            foreach (var angle in angles)
            {
                _rect.localRotation = Quaternion.Euler(0f, 0f, angle);
                shadow.UpdateMotion(0f, 80f, angle);

                var rotZ = Mathf.DeltaAngle(35f, shadow.ShadowRect.eulerAngles.z);
                Assert.That(rotZ, Is.EqualTo(0f).Within(0.1f),
                    $"la sombra del tronco debe permanecer a 35° al rodar con giro {angle}°");

                Assert.That(shadow.ShadowRect.position.y, Is.EqualTo(_rect.position.y + baseLocalY).Within(0.1f),
                    $"la sombra debe permanecer en la base de contacto debajo del tronco");
            }

            // 2. Tronco con espejo (la perspectiva del cilindro siempre se orienta a +35° hacia el fondo)
            _rect.localScale = new Vector3(-1.5f, 1.5f, 1f);
            foreach (var angle in angles)
            {
                _rect.localRotation = Quaternion.Euler(0f, 0f, angle);
                shadow.UpdateMotion(0f, 80f, angle);

                var rotZ = Mathf.DeltaAngle(35f, shadow.ShadowRect.eulerAngles.z);
                Assert.That(rotZ, Is.EqualTo(0f).Within(0.1f),
                    $"la sombra del tronco reflejado debe permanecer a 35° hacia el fondo sin renderizarse al lado opuesto");

                Assert.That(shadow.ShadowRect.position.y, Is.EqualTo(_rect.position.y + baseLocalY * 1.5f).Within(0.1f),
                    $"la sombra debe permanecer en la base de contacto debajo del tronco reflejado");
            }
        }

        [Test]
        public void PropShadow_RNF23_SombraDeTelaSePosicionaBajoLosPlieguesYNoEnLaPuntaAislada()
        {
#if UNITY_EDITOR
            var telaSprite = UnityEditor.AssetDatabase
                .LoadAssetAtPath<Sprite>("Assets/Game/Art/Props/River/prop_n3_tela.png");

            if (telaSprite != null)
            {
                var shadowTela = PropShadow.Attach(_holder, telaSprite, _rect.sizeDelta);
                // La tela tiene un pico alargado que baja a -68 %, pero sus pliegues principales
                // se sitúan cerca de -15 %. Su BaseLocalY debe estar cerca de -30 px en 200x200 (no en -60 px ni -80 px).
                Assert.That(shadowTela.BaseLocalY, Is.GreaterThan(-40f),
                    "la sombra de la tela no se descuelga hasta la punta aislada, sino que abraza la base del dibujo");
            }
#endif
        }
    }
}

