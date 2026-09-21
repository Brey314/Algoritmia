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

        private static NarrativeSequence CargarSecuenciaDeCierre(string id = "N1_NacimientoDelFuego") =>
            AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Single(sequence => sequence != null && sequence.Id == id);

        /// <summary>El asset real de mensajes de un nivel: lo que se prueba es el contenido radicado, no un doble.</summary>
        private static LevelSummaryMessages CargarMensajes(LevelId level) =>
            AssetDatabase.FindAssets($"t:{nameof(LevelSummaryMessages)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<LevelSummaryMessages>)
                .Single(messages => messages != null && messages.Level == level);

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
        [Category("Acceptance")]
        public void LevelSummary_RF45_ElResumenDelNivel2NoContieneNingunDigito()
        {
            var messages = CargarMensajes(LevelId.Wheel);
            var fases = new[]
            {
                new[] { new PerformanceIndicators(0, 0, 0, 12f), new PerformanceIndicators(0, 0, 5, 30f), new PerformanceIndicators(0, 0, 7, 40f) },
                new[] { new PerformanceIndicators(9, 3, 0, 120f), new PerformanceIndicators(4, 2, 5, 300f), new PerformanceIndicators(11, 6, 8, 900f) },
                new[] { new PerformanceIndicators(0, 0, 0, 1f), new PerformanceIndicators(0, 0, 5, 1f), new PerformanceIndicators(1, 0, 4, 1f) },
                new[] { new PerformanceIndicators(0, 0, 0, 1f), new PerformanceIndicators(0, 1, 5, 1f), new PerformanceIndicators(0, 0, 4, 1f) }
            };

            var textos = fases.Select(indicadores => LevelSummaryComposer.Compose(messages, indicadores))
                .Concat(new[] { messages.Discovery, messages.SkillNamed });

            Assert.That(textos, Has.All.Matches<string>(texto => !texto.Any(char.IsDigit)),
                "RF-45/CP-03 (INC-26): el resumen del Nivel 2 no contiene ninguna cifra, sean cuales sean las tres fases");
        }

        [Test]
        [Category("Acceptance")]
        public void LevelSummary_RF12_NombraLaAbstraccionYElPensamientoAlgoritmico()
        {
            var cierre = string.Join(" ", CargarSecuenciaDeCierre("N2_Escena25_Cierre").Lines.Select(line => line.Text));
            var resumen = CargarMensajes(LevelId.Wheel).SkillNamed;

            foreach (var texto in new[] { cierre, resumen })
            {
                Assert.That(texto, Does.Contain("abstraer").IgnoreCase, "nombra la abstracción (RF-12, guion §6.4)");
                Assert.That(texto, Does.Contain("algoritmo").IgnoreCase, "y el pensamiento algorítmico");
            }

            Assert.That(cierre, Does.Contain("ordenaron").IgnoreCase.Or.Contain("ordenar").IgnoreCase,
                "y las liga a lo que el jugador acaba de hacer (HU-14)");
        }

        [Test]
        [Category("Acceptance")]
        public void LevelSummary_RF45_ElResumenDelNivel3NoContieneNingunDigito()
        {
            var messages = CargarMensajes(LevelId.River);
            var fases = new[]
            {
                new[] { new PerformanceIndicators(0, 0, 1, 12f), new PerformanceIndicators(0, 0, 1, 30f), new PerformanceIndicators(0, 0, 1, 40f) },
                new[] { new PerformanceIndicators(9, 3, 1, 120f), new PerformanceIndicators(4, 2, 1, 300f), new PerformanceIndicators(11, 6, 1, 900f) },
                new[] { new PerformanceIndicators(0, 0, 1, 1f), new PerformanceIndicators(0, 0, 1, 1f), new PerformanceIndicators(1, 0, 1, 1f) },
                new[] { new PerformanceIndicators(0, 0, 1, 1f), new PerformanceIndicators(0, 1, 1, 1f), new PerformanceIndicators(0, 0, 1, 1f) }
            };

            var textos = fases.Select(indicadores => LevelSummaryComposer.Compose(messages, indicadores))
                .Concat(new[] { messages.Discovery, messages.SkillNamed });

            Assert.That(textos, Has.All.Matches<string>(texto => !texto.Any(char.IsDigit)),
                "RF-45/CP-03 (INC-26): el resumen del Nivel 3 no contiene ninguna cifra, sean cuales sean las tres fases");
        }

        [Test]
        [Category("Acceptance")]
        public void LevelSummary_RF12_NombraLaDescomposicionYLaDepuracion()
        {
            var cruce = string.Join(" ", CargarSecuenciaDeCierre("N3_Escena33_Cruce").Lines.Select(line => line.Text));
            var mensajes = CargarMensajes(LevelId.River);

            Assert.That(mensajes.SkillNamed, Does.Contain("descompon").IgnoreCase, "el resumen nombra la descomposición (RF-12)");
            Assert.That(mensajes.SkillNamed, Does.Contain("depurar").IgnoreCase, "y la depuración");
            Assert.That(cruce, Does.Contain("partes pequeñas").IgnoreCase,
                "el cruce liga la descomposición a lo que el jugador hizo (guion §8.5, HU-14)");
            Assert.That(cruce, Does.Contain("lo encontraron y lo arreglaron").IgnoreCase, "y la depuración");
            Assert.That(mensajes.ClosingSequenceId, Is.EqualTo("N3_EscenaFinal"),
                "y «Continuar» sale a la escena final del juego, no al menú (INC-39)");
        }

        [Test]
        public void LevelSummaryComposer_RF45_ElResumenDeUnNivelDeVariasFasesSumaLasFases()
        {
            var messages = CreateMessages();
            var fases = new[]
            {
                new PerformanceIndicators(0, 0, 0, 10f),
                new PerformanceIndicators(0, 0, 5, 10f),
                new PerformanceIndicators(2, 1, 4, 10f) // solo el laberinto tuvo intentos y correcciones
            };

            var actual = LevelSummaryComposer.Compose(messages, fases);

            Assert.That(actual, Does.Contain(messages.TriedSeveralPositions), "un intento en cualquier fase cuenta para el relato");
            Assert.That(actual, Does.Contain(messages.CorrectedApproach), "y un error corregido también");
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
