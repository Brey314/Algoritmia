using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class StoneSpacingTests
    {
        // Geometría de la escena de hoy: piedras de 72 px y hojas de 84 px. Con diez muescas, cada
        // muesca encima 8,4 px más; la cinco deja 30 px, el roce que hace chispa (30 ± 4), y las
        // piedras se tocan desde la dos (4,8 px).
        private const float StoneWidth = 72f;
        private const float LeafWidth = 84f;

        private FireLevelConfig _config;

        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<FireLevelConfig>();
            _config.SpacingLevels = 10;
            _config.EffectiveOverlap = 30f;
            _config.OverlapTolerance = 4f;
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_config);

        private StoneSpacing CreateSystemUnderTest() => new StoneSpacing(_config, StoneWidth, LeafWidth);

        [Test]
        [Category("Acceptance")]
        public void StoneSpacing_RF15_LaMuescaCeroSeparaUnaHojaYLaMaximaEncimaLasPiedras()
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.Distance(0), Is.EqualTo(LeafWidth), "lo más lejos es la longitud de una hoja");
            Assert.That(sut.Distance(10), Is.EqualTo(0f), "lo más cerca es una piedra encima de la otra");
            Assert.That(sut.Distance(5), Is.EqualTo(LeafWidth / 2f), "y entre medias es lineal");
        }

        [TestCase(0, SpacingBand.TooFar)]
        [TestCase(4, SpacingBand.TooFar)]
        [TestCase(5, SpacingBand.Effective)]
        [TestCase(6, SpacingBand.TooClose)]
        [TestCase(10, SpacingBand.TooClose)]
        public void StoneSpacing_RF16_ElGolpeCerteroEsSoloLaMuescaCinco(int muesca, SpacingBand esperada)
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.Classify(muesca), Is.EqualTo(esperada), $"muesca {muesca}: encimadas {sut.Overlap(muesca):F1} px");
        }

        [Test]
        public void StoneSpacing_RNF18_ElRoceEfectivoYSuMargenSalenDeLaConfiguracion()
        {
            _config.EffectiveOverlap = 30f;
            _config.OverlapTolerance = 6f;
            var sut = CreateSystemUnderTest();

            Assert.That(sut.Classify(5), Is.EqualTo(SpacingBand.Effective), "encimadas 30 px: justo el roce configurado");
            Assert.That(sut.Classify(4), Is.EqualTo(SpacingBand.TooFar), "encimadas 21,6 px: fuera del margen por abajo");
            Assert.That(sut.Classify(6), Is.EqualTo(SpacingBand.TooClose), "encimadas 38,4 px: fuera del margen por arriba");
        }

        [Test]
        public void StoneSpacing_RF16_ElContactoEsCeroConHuecoYCreceHastaEncimarlasDelTodo()
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.Contact(1), Is.Zero, "hueco de 3,6 px: no chocan, no se oye golpe");
            Assert.That(sut.Contact(2), Is.GreaterThan(0f).And.LessThan(sut.Contact(5)), "se tocan apenas: el golpe más suave");
            Assert.That(sut.Contact(10), Is.EqualTo(1f), "una encima de la otra: el golpe más fuerte");
        }

        [Test]
        public void StoneSpacing_RF15_UnaMuescaFueraDeRangoNoRompeNada()
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.Distance(-3), Is.EqualTo(LeafWidth), "por debajo de cero se toma como lo más lejos");
            Assert.That(sut.Distance(99), Is.EqualTo(0f), "por encima del máximo, como lo más cerca");
        }
    }
}
