using System;
using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;

namespace Game.Levels.Wheel.Tests
{
    // Lo que el contenido del taller tiene que declarar bien para que el corte a la narrativa no
    // cambie la carretilla.
    public class AssemblyContentTests
    {
        [Test]
        [Category("Acceptance")]
        public void AssemblyContent_INC54_LaCarretillaAmarradaEsElDibujoConElQueAbreLaEscena24()
        {
            var content = AssetDatabase.FindAssets($"t:{nameof(AssemblyContent)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<AssemblyContent>)
                .Single(asset => asset != null);
            var regreso = AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Single(sequence => sequence != null && sequence.Id == content.ClosingSequenceId);
            var carretilla = regreso.Props.Single(prop =>
                prop.Art != null && prop.Art.name.StartsWith("prop_n2_carretilla_", StringComparison.Ordinal));

            // El taller termina en el dibujo con el que abre la 2.4. El arte ya corrió una vez la
            // numeración (44fd479) y dejó el taller en e4 sin que nada lo avisara.
            Assert.That(content.TiedArt, Is.Not.Null.And.SameAs(carretilla.Art));
        }
    }
}
