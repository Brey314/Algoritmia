using System;
using NUnit.Framework;
using UnityEditor;

namespace Game.UI.Tests
{
    /// <summary>
    /// El texto de <see cref="CreditsContent"/> respeta el máximo de RNF-01, igual que
    /// <c>LevelSummaryContent_RNF01_NingunaOracionSupera20Palabras</c> lo hace para el resumen de nivel.
    /// </summary>
    public class CreditsContentTests
    {
        private const string AssetPath = "Assets/Game/Data/CreditsContent.asset";

        [Test]
        public void CreditsContent_RNF01_NingunaOracionSupera20Palabras()
        {
            var content = AssetDatabase.LoadAssetAtPath<CreditsContent>(AssetPath);
            Assert.That(content, Is.Not.Null, $"no se encontró el asset en {AssetPath}");

            foreach (var oracion in content.Body.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var palabras = oracion.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                Assert.That(palabras.Length, Is.LessThanOrEqualTo(20),
                    $"RNF-01 en CreditsContent.Body: «{oracion.Trim()}» tiene {palabras.Length} palabras");
            }
        }
    }
}
