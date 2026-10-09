using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;
using UnityEngine.UI;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// El halo de luz de las fogatas (Dirección de Arte §8.1, «La hoguera»; decisión de Santiago,
    /// 08/10/2026): un círculo <c>#F0A84E</c> al 20 % que respira entre 0,95 y 1,05 en 2 s, sin
    /// destellos (RNF-21).
    /// </summary>
    [TestFixture]
    public class FireGlowTests
    {
        private GameObject _suelo;
        private RectTransform _llama;

        [SetUp]
        public void SetUp()
        {
            _suelo = new GameObject("Suelo", typeof(RectTransform));
            var detras = new GameObject("Detras", typeof(RectTransform));
            detras.transform.SetParent(_suelo.transform, false);
            var llama = new GameObject("Fuego", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            _llama = (RectTransform)llama.transform;
            _llama.SetParent(_suelo.transform, false);
            _llama.sizeDelta = new Vector2(200f, 300f);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_suelo);
        }

        [Test]
        public void FireGlow_RNF21_LaEscalaRespiraEntre095Y105ConUnCicloDeDosSegundos()
        {
            var escalas = Enumerable.Range(0, 801).Select(i => FireGlow.ScaleAt(i * 0.005f)).ToArray();

            Assert.That(escalas.Min(), Is.EqualTo(0.95f).Within(1e-4f), "mínimo de la respiración");
            Assert.That(escalas.Max(), Is.EqualTo(1.05f).Within(1e-4f), "máximo de la respiración");
            Assert.That(FireGlow.ScaleAt(0.37f), Is.EqualTo(FireGlow.ScaleAt(2.37f)).Within(1e-5f), "un ciclo dura 2 s");
            Assert.That(FireGlow.ScaleAt(0.37f), Is.Not.EqualTo(FireGlow.ScaleAt(1.37f)).Within(1e-3f), "y no se repite a mitad de ciclo");
        }

        [Test]
        public void FireGlow_RNF21_LaEscalaCambiaDespacioYSinSaltos()
        {
            const float cuadro = 1f / 60f;

            var mayorCambio = Enumerable.Range(0, 480)
                .Select(i => Mathf.Abs(FireGlow.ScaleAt((i + 1) * cuadro) - FireGlow.ScaleAt(i * cuadro)))
                .Max();

            // La pendiente máxima de la onda es 0,05·π ≈ 0,157 por segundo: 0,0026 por cuadro a 60 fps.
            Assert.That(mayorCambio, Is.LessThan(0.003f), "ningún cuadro cambia la escala más que eso: nada destella");
        }

        [Test]
        public void FireGlow_RNF21_ElColorEsF0A84EAl20PorCiento()
        {
            Assert.That(ColorUtility.ToHtmlStringRGBA(FireGlow.GlowColor), Is.EqualTo("F0A84E33"));
        }

        [Test]
        public void FireGlow_RNF21_ElHaloSeDesvaneceHaciaElBordeSinEscalon()
        {
            // Un círculo plano a 20 % se lee como una mancha naranja, no como luz: lo que lo vuelve luz
            // es que su opacidad no tenga escalón (capturas del 08/10/2026).
            var radios = Enumerable.Range(0, 65).Select(i => i / 64f).ToArray();
            var opacidades = radios.Select(FireGlow.OpacityAt).ToArray();
            var mayorEscalon = Enumerable.Range(0, 64).Select(i => Mathf.Abs(opacidades[i + 1] - opacidades[i])).Max();

            Assert.That(FireGlow.OpacityAt(0f), Is.EqualTo(1f), "el centro conserva el 20 %");
            Assert.That(FireGlow.OpacityAt(1f), Is.EqualTo(0f), "y el borde se funde con el fondo");
            Assert.That(opacidades, Is.Ordered.Descending, "la luz solo mengua hacia fuera");
            Assert.That(mayorEscalon, Is.LessThan(0.05f), "sin escalón de un cuadro al siguiente: un disco de borde duro daría 1");
        }

        [Test]
        public void FireGlow_RNF21_ElHaloNoCambiaDeOpacidadNiSeLlevaLosClics()
        {
            var glow = FireGlow.Attach(_llama.gameObject);
            var halo = glow.Halo.GetComponent<Image>();

            foreach (var segundos in new[] { 0f, 0.5f, 1f, 1.5f, 2f, 3.3f })
            {
                glow.Follow(segundos);
                Assert.That(halo.color, Is.EqualTo(FireGlow.GlowColor), $"t={segundos}: la opacidad no oscila, solo la escala");
            }

            Assert.That(halo.raycastTarget, Is.False, "el halo no se lleva los clics");
            Assert.That(halo.sprite, Is.SameAs(FireGlow.GetOrCreateGlowSprite()), "y es el disco que se desvanece");
        }

        [Test]
        public void FireGlow_RF05_ElHaloEsHermanoDeLaLlamaJustoAntesQueEllaYNoSuHijo()
        {
            var glow = FireGlow.Attach(_llama.gameObject);

            Assert.That(glow.Halo.IsChildOf(_llama), Is.False, "en uGUI un hijo se pinta encima de su padre y taparía la llama");
            Assert.That(glow.Halo.parent, Is.SameAs(_suelo.transform), "hermano de la llama");
            Assert.That(glow.Halo.name, Is.EqualTo("Halo_Fuego"), "con el nombre que las pruebas saltan al contar objetos");
            Assert.That(glow.Halo.GetSiblingIndex(), Is.EqualTo(_llama.GetSiblingIndex() - 1), "justo antes: debajo de ella y encima de lo anterior");
            Assert.That(_llama.GetSiblingIndex(), Is.EqualTo(_suelo.transform.childCount - 1), "la llama sigue siendo el último hermano");
        }

        [Test]
        public void FireGlow_RF05_AdjuntarDosVecesNoDuplicaElHalo()
        {
            FireGlow.Attach(_llama.gameObject);
            FireGlow.Attach(_llama.gameObject);

            Assert.That(_suelo.transform.childCount, Is.EqualTo(3), "Detras, el halo y la llama");
        }

        [Test]
        public void FireGlow_RF05_AdjuntarAUnaLlamaApagadaNoEnciendeElHalo()
        {
            _llama.gameObject.SetActive(false);

            var glow = FireGlow.Attach(_llama.gameObject);

            Assert.That(glow.Halo.GetComponent<Image>().enabled, Is.False, "sin llama no hay luz: el halo nace con ella");
        }

        [Test]
        public void FireGlow_RF05_ConBehindSiblingsElHaloVaAlFondoYLaLlamaSigueUltima()
        {
            var glow = _llama.gameObject.AddComponent<FireGlow>();
            glow.BehindSiblings = true;
            glow.EnsureHalo();

            Assert.That(glow.Halo.GetSiblingIndex(), Is.Zero, "al fondo del contenedor: baña el suelo y no vela lo que hay encima");
            Assert.That(_llama.GetSiblingIndex(), Is.EqualTo(_suelo.transform.childCount - 1), "la llama sigue siendo el último hermano");
        }

        [Test]
        public void FireGlow_RF05_ElHaloSeCentraEnLaLlamaYMideSuLadoMenorPorSpread()
        {
            var glow = FireGlow.Attach(_llama.gameObject);

            glow.Follow(0f); // a los 0 s la escala vale 1

            Assert.That(glow.Halo.sizeDelta, Is.EqualTo(new Vector2(360f, 360f)).Using(new Vector2EqualityComparer(1e-3f)), "1,8 veces el lado menor (200)");
            Assert.That(glow.Halo.anchoredPosition, Is.EqualTo(new Vector2(0f, -20f)).Using(new Vector2EqualityComparer(1e-3f)), "centrado, un poco abajo: el cuerpo de la llama ocupa la mitad de abajo de su cuadro");
            Assert.That(glow.Halo.localScale.x, Is.EqualTo(1f).Within(1e-5f), "a los 0 s la escala vale 1");
        }

        [Test]
        public void FireGlow_RF05_ElHaloSeCentraEnElCuadroDeLaLlamaAunqueSuPivoteNoSeaElCentro()
        {
            _llama.pivot = new Vector2(0.5f, 0f); // pivote en la base
            var glow = FireGlow.Attach(_llama.gameObject);

            glow.Follow(0f);

            // El centro del cuadro está 150 por encima del pivote; el halo baja 0,1 lados (20).
            Assert.That(glow.Halo.anchoredPosition, Is.EqualTo(new Vector2(0f, 130f)).Using(new Vector2EqualityComparer(1e-3f)));
        }

        [Test]
        public void FireGlow_RF05_ElHaloSigueALaLlamaCuandoSeMueve()
        {
            var glow = FireGlow.Attach(_llama.gameObject);

            _llama.anchoredPosition = new Vector2(100f, 40f);
            _llama.localScale = new Vector3(2f, 2f, 1f);
            glow.Follow(0f);

            Assert.That(glow.Halo.anchoredPosition, Is.EqualTo(new Vector2(100f, 40f - 40f)).Using(new Vector2EqualityComparer(1e-3f)), "con la llama, bajado 0,1 lados (40 a escala 2)");
            Assert.That(glow.Halo.sizeDelta, Is.EqualTo(new Vector2(720f, 720f)).Using(new Vector2EqualityComparer(1e-3f)), "y del tamaño que la llama tiene ahora");
        }
    }
}
