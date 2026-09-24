// CT-10: el nombre del método de prueba es la trazabilidad. La matriz se deriva de los nombres
// de método —no se mantiene a mano en un documento aparte, que se desactualiza en el primer
// commit que no la toque.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Game.Architecture.Tests
{
    public class TraceabilityTests
    {
        private const int TotalRf = 47;

        /// <summary>Cita el identificador en el nombre del método de prueba, no en un comentario suelto.</summary>
        private static readonly Regex MetodoDePrueba =
            new Regex(@"public\s+(?:async\s+)?(?:void|Task)\s+\w*RF(\d{2})\w*\s*\(");

        [Test]
        public void Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra()
        {
            var testsRoot = Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "Assets", "Tests");
            var citados = new HashSet<int>();

            foreach (var archivo in Directory.GetFiles(testsRoot, "*.cs", SearchOption.AllDirectories))
            {
                foreach (Match match in MetodoDePrueba.Matches(File.ReadAllText(archivo)))
                {
                    citados.Add(int.Parse(match.Groups[1].Value));
                }
            }

            var faltantes = Enumerable.Range(1, TotalRf).Where(rf => !citados.Contains(rf))
                .Select(rf => $"RF-{rf:D2}");

            Assert.That(faltantes, Is.Empty,
                "CT-10: todo RF necesita al menos una prueba que lo nombre en el nombre del método.");
        }
    }
}
