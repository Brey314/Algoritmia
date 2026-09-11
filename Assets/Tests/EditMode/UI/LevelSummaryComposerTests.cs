using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    [TestFixture]
    public class LevelSummaryComposerTests
    {
        private readonly List<Object> _instances = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var instance in _instances)
            {
                Object.DestroyImmediate(instance);
            }

            _instances.Clear();
        }

        private LevelSummaryMessages CreateMessages()
        {
            var messages = ScriptableObject.CreateInstance<LevelSummaryMessages>();
            _instances.Add(messages);
            return messages;
        }

        private static NarrativeSequence CargarSecuenciaDeCierre() =>
            AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Single(sequence => sequence != null && sequence.Id == "N1_NacimientoDelFuego");

        [Test]
        [Category("Acceptance")]
        public void LevelSummary_RF45_NoContieneNingunDigito()
        {
            var messages = CreateMessages();
            var combinaciones = new[]
            {
                new PerformanceIndicators(0, 0, 3, 12f),
                new PerformanceIndicators(42, 7, 3, 300f),
                new PerformanceIndicators(1, 0, 3, 1f),
                new PerformanceIndicators(0, 1, 3, 1f)
            };

            var resultados = combinaciones
                .Select(indicators => LevelSummaryComposer.Compose(messages, indicators));

            Assert.That(resultados, Has.All.Matches<string>(texto => !texto.Any(char.IsDigit)),
                "RF-45/CP-03: el resumen no puede contener ninguna cifra, sin importar los valores");
        }

        [Test]
        [Category("Acceptance")]
        public void LevelSummary_RF12_NombraLaHabilidadEjercitada()
        {
            var sequence = CargarSecuenciaDeCierre();
            var texto = string.Join(" ", sequence.Lines.Select(line => line.Text));

            Assert.That(texto, Does.Contain("iterar").IgnoreCase,
                "el guía nombra la habilidad practicada (RF-12)");
            Assert.That(texto, Does.Contain("cambiaste").IgnoreCase,
                "y la liga a una acción concreta que hizo el jugador (RF-12, HU-14)");
        }

        [Test]
        public void LevelSummaryComposer_RF45_SinErroresCorregidosNoUsaLaVarianteDeCambioDeEstrategia()
        {
            var messages = CreateMessages();
            var indicators = new PerformanceIndicators(1, 0, 3, 10f);

            var actual = LevelSummaryComposer.Compose(messages, indicators);

            Assert.That(actual, Does.Not.Contain(messages.CorrectedApproach),
                "sin errores corregidos no puede aparecer la variante de «cambiaste de estrategia»");
            Assert.That(actual, Does.Contain(messages.NoNeedToCorrect));
        }

        [Test]
        public void LevelSummaryComposer_RF45_SinIntentosFallidosNoUsaLaVarianteDeVariasPosiciones()
        {
            var messages = CreateMessages();
            var indicators = new PerformanceIndicators(0, 1, 3, 10f);

            var actual = LevelSummaryComposer.Compose(messages, indicators);

            Assert.That(actual, Does.Not.Contain(messages.TriedSeveralPositions),
                "sin intentos fallidos no puede aparecer la variante de «probaste varias posiciones»");
            Assert.That(actual, Does.Contain(messages.FoundRightAway));
        }
    }
}
