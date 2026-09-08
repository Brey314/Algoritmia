using System.Collections.Generic;
using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// Comprueba el **contenido** de las secuencias narrativas que hay en el proyecto, no la
    /// clase. Los textos viven en assets (RNF-18, CT-05), así que el requisito de legibilidad
    /// también hay que verificarlo sobre el asset y no sobre el código.
    /// </summary>
    public class NarrativeSequenceTests
    {
        private const int MaximoPalabrasPorOracion = 20;

        [Test]
        public void NarrativeSequence_RNF01_NingunaOracionSupera20Palabras()
        {
            var excedidas = TodasLasSecuencias()
                .SelectMany(sequence => sequence.Lines.Select(line => (sequence, line)))
                .SelectMany(par => Oraciones(par.line.Text)
                    .Where(oracion => Palabras(oracion) > MaximoPalabrasPorOracion)
                    .Select(oracion => $"{par.sequence.Id} · {Palabras(oracion)} palabras: «{oracion}»"))
                .ToArray();

            Assert.That(excedidas, Is.Empty);
        }

        [Test]
        public void NarrativeSequence_RNF18_CadaSecuenciaTieneIdYLineas()
        {
            var incompletas = TodasLasSecuencias()
                .Where(sequence => string.IsNullOrWhiteSpace(sequence.Id) || sequence.Lines.Length == 0)
                .Select(sequence => sequence.name)
                .ToArray();

            Assert.That(incompletas, Is.Empty);
        }

        [Test]
        public void NarrativeSequence_RF05_LosIdentificadoresNoSeRepiten()
        {
            var repetidos = TodasLasSecuencias()
                .GroupBy(sequence => sequence.Id)
                .Where(grupo => grupo.Count() > 1)
                .Select(grupo => grupo.Key)
                .ToArray();

            Assert.That(repetidos, Is.Empty);
        }

        // --- helpers -----------------------------------------------------------------------

        private static IEnumerable<NarrativeSequence> TodasLasSecuencias() =>
            AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Where(sequence => sequence != null);

        /// <summary>
        /// Parte el texto en oraciones. Los signos de apertura «¿» y «¡» no cierran nada, así que
        /// solo cuentan los de cierre más el punto y los puntos suspensivos.
        /// </summary>
        private static IEnumerable<string> Oraciones(string text) => (text ?? string.Empty)
            .Replace("…", ".")
            .Split(new[] { '.', '!', '?' }, System.StringSplitOptions.RemoveEmptyEntries)
            .Select(oracion => oracion.Trim())
            .Where(oracion => oracion.Length > 0);

        private static int Palabras(string oracion) =>
            oracion.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
