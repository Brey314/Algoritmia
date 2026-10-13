using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// Comprueba el contenido de las narrativas: un paso que camina, corre, carga o empuja tiene que
    /// desplazarse lo bastante para que se vea (INC-153).
    /// </summary>
    public class NarrativeLocomotionTests
    {
        [Test]
        public void NarrativeSequence_INC153_LosPasosDeLocomocionDesplazanAlActor()
        {
            // Es privada en ActorTimeline: bajo ese umbral el rumbo no se deduce del movimiento y el
            // personaje camina en sentido contrario a la pantalla. La causa raíz de INC-153 era un
            // paso Walk de 4 milésimas.
            var field = typeof(ActorTimeline).GetField("MinimumDisplacement", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(field, Is.Not.Null, "ActorTimeline.MinimumDisplacement existe");
            var minimum = (float)field.GetRawConstantValue();

            var locomotion = new[] { ActorAction.Walk, ActorAction.Run, ActorAction.Carry, ActorAction.Push };
            var cortos = new List<string>();
            var sequences = TodasLasSecuencias().ToArray();
            Assert.That(sequences.Length, Is.EqualTo(18), "las 18 narrativas");

            foreach (var sequence in sequences)
            {
                for (var index = 0; index < sequence.Props.Length; index++)
                {
                    var prop = sequence.Props[index];
                    foreach (var beat in prop.Beats.Where(b => b != null && b.Moves && locomotion.Contains(b.Action)))
                    {
                        var from = ActorTimeline.PositionBefore(prop, beat.Line);
                        var distance = Vector2.Distance(from, beat.Destination);
                        if (distance < minimum)
                        {
                            cortos.Add($"{sequence.Id} · objeto {index} · línea {beat.Line} · {beat.Action}: {distance:0.####}");
                        }
                    }
                }
            }

            Assert.That(cortos, Is.Empty, $"desplazamiento menor que {minimum}");
        }

        private static IEnumerable<NarrativeSequence> TodasLasSecuencias() =>
            AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Where(sequence => sequence != null);
    }
}
