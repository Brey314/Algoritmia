using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Fire.Tests
{
    public class FloorScatterTests
    {
        [Test]
        public void FloorScatter_RF14_NingunaPiezaPisaLaInterfazNiElCirculoNiSeEncimaConOtra()
        {
            // El lienzo de referencia (1920×1080) con la tablilla arriba, «Pista» y pausa en las
            // esquinas superiores y el círculo de reunión en el centro, como Level1_Cave.
            var floor = new Rect(-960f, -540f, 1920f, 1080f);
            var blocked = new[]
            {
                new Rect(-500f, 380f, 1000f, 96f),
                new Rect(-896f, 364f, 112f, 112f),
                new Rect(784f, 364f, 88f, 88f),
                new Rect(-300f, -300f, 600f, 600f)
            };
            var sizes = Enumerable.Repeat(84f * 1.6f, 7).ToArray();

            var positions = FloorScatter.Place(floor, blocked, sizes, new System.Random(7));

            var rects = positions
                .Select((position, i) => new Rect(position.x - sizes[i] / 2f, position.y - sizes[i] / 2f, sizes[i], sizes[i]))
                .ToArray();
            Assert.That(rects, Has.All.Matches<Rect>(rect =>
                    rect.xMin >= floor.xMin && rect.xMax <= floor.xMax && rect.yMin >= floor.yMin && rect.yMax <= floor.yMax),
                "dentro del suelo");
            Assert.That(rects, Has.All.Matches<Rect>(rect => blocked.All(zone => !rect.Overlaps(zone))),
                "fuera de la interfaz y del círculo");
            Assert.That(rects.SelectMany((a, i) => rects.Skip(i + 1).Select(b => (a, b))),
                Has.All.Matches<(Rect a, Rect b)>(pair => !pair.a.Overlaps(pair.b)), "sin encimarse entre ellas");
        }
    }
}
