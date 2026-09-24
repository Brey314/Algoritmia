using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        /// <summary>Las siete líneas de texto visible del asset: lo que el barrido de P02 recorre.</summary>
        private static IEnumerable<string> LineasVisibles(LevelSummaryMessages m) => new[]
        {
            m.Intro, m.Discovery, m.TriedSeveralPositions, m.FoundRightAway,
            m.CorrectedApproach, m.NoNeedToCorrect, m.SkillNamed
        };

        /// <summary>Palabras que convierten una descripción en un veredicto (RF-17, CP-03): la lista
        /// no busca sinónimos exhaustivos, busca que nadie cuele un adjetivo de desempeño.</summary>
        private static readonly string[] JuiciosDeValor =
        {
            "excelente", "perfecto", "genial", "increíble", "fantástico", "pésimo",
            "malo", "mal hecho", "torpe", "lento", "rápido", "bien hecho", "muy bien"
        };

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

        /// <summary>
        /// Barrido transversal de Slice 4 (P02): los tres niveles a la vez, sobre el asset real que
        /// juega el estudiante y no sobre un doble — es donde HU-14 ya coló una cifra (INC-26).
        /// </summary>
        [Test]
        public void LevelSummary_RF45_NingunResumenDeLosTresNivelesContieneUnDigito()
        {
            foreach (var nivel in new[] { LevelId.Fire, LevelId.Wheel, LevelId.River })
            {
                var lineas = LineasVisibles(CargarMensajes(nivel));
                Assert.That(lineas, Has.All.Matches<string>(texto => !texto.Any(char.IsDigit)),
                    $"RF-45/CP-03: el resumen del nivel {nivel} no puede contener ninguna cifra");
            }
        }

        [Test]
        public void LevelSummary_RF17_NingunResumenEmiteJuicioDeValor()
        {
            foreach (var nivel in new[] { LevelId.Fire, LevelId.Wheel, LevelId.River })
            {
                var lineas = LineasVisibles(CargarMensajes(nivel));
                foreach (var texto in lineas)
                {
                    foreach (var juicio in JuiciosDeValor)
                    {
                        Assert.That(texto.ToLowerInvariant(), Does.Not.Contain(juicio),
                            $"RF-17/CP-03: el resumen del nivel {nivel} describe, no califica («{juicio}» en «{texto}»)");
                    }
                }
            }
        }

        /// <summary>
        /// CP-03: las cifras solo existen en TeacherReport (RF-46). Un nombre de clase es lo que
        /// impide que alguien reintroduzca un marcador de puntaje sin que nadie lo note.
        /// </summary>
        [Test]
        public void LevelSummary_CP03_NoExisteClaseDePuntajeEnElProyecto()
        {
            var root = Path.Combine(Application.dataPath, "Game", "Scripts");
            var declaraciones = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
                .SelectMany(file => Regex.Matches(File.ReadAllText(file), @"\b(?:class|struct)\s+(\w+)")
                    .Select(match => match.Groups[1].Value));

            var prohibidas = declaraciones.Where(nombre =>
                nombre.IndexOf("Score", StringComparison.OrdinalIgnoreCase) >= 0 ||
                nombre.IndexOf("Points", StringComparison.OrdinalIgnoreCase) >= 0);

            Assert.That(prohibidas, Is.Empty,
                "CP-03: el estudiante no ve cifras de desempeño; no puede existir un tipo de puntaje aunque nadie lo muestre todavía");
        }

        [Test]
        public void LevelSummaryContent_RNF01_NingunaOracionSupera20Palabras()
        {
            foreach (var nivel in new[] { LevelId.Fire, LevelId.Wheel, LevelId.River })
            {
                foreach (var texto in LineasVisibles(CargarMensajes(nivel)))
                {
                    var oraciones = texto.Split(new[] { '.', '!', '?', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var oracion in oraciones)
                    {
                        var palabras = oracion.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                        Assert.That(palabras.Length, Is.LessThanOrEqualTo(20),
                            $"RNF-01 en nivel {nivel}: «{oracion.Trim()}» tiene {palabras.Length} palabras");
                    }
                }
            }
        }
    }
}
