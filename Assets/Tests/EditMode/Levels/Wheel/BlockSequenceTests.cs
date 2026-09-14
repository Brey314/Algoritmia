using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>La secuencia de bloques encajables como dato (RF-31, RF-34, CU-08 FA-3a/FA-6a, mockup 10).</summary>
    public class BlockSequenceTests
    {
        [Test]
        public void BlockSequence_RF31_SoloExistenTresTiposDeBloqueYCadaUnoLlevaSuParametro()
        {
            Assert.That(Enum.GetValues(typeof(BlockKind)).Cast<BlockKind>(),
                Is.EquivalentTo(new[] { BlockKind.Forward, BlockKind.Backward, BlockKind.Turn }),
                "avanzar, retroceder y girar (RF-31), los tres del mockup 10 con su parámetro, ni uno más");

            Assert.That(InstructionBlock.Default(BlockKind.Forward).Count, Is.EqualTo(1), "«Avanzar» sale de la paleta con cuenta 1");
            Assert.That(InstructionBlock.Forward(0).Count, Is.EqualTo(InstructionBlock.MinCount), "la cuenta no baja de 1");
            Assert.That(InstructionBlock.Forward(12).Count, Is.EqualTo(InstructionBlock.MaxCount), "ni sube de 9 (mockup: − / +)");
            Assert.That(InstructionBlock.Forward(2).WithCount(3).Count, Is.EqualTo(3));
            Assert.That(InstructionBlock.Default(BlockKind.Turn).Direction, Is.EqualTo(TurnDirection.Right), "«Girar» sale mirando a la derecha");
            Assert.That(InstructionBlock.Turn().WithDirection(TurnDirection.Left).Direction, Is.EqualTo(TurnDirection.Left));
            Assert.That(InstructionBlock.Default(BlockKind.Backward).Count, Is.EqualTo(1), "«Retroceder» también lleva cuenta y sale con 1");
            Assert.That(InstructionBlock.Backward(4).WithCount(11).Count, Is.EqualTo(InstructionBlock.MaxCount));
        }

        [Test]
        public void BlockSequence_RF34_RetirarReordenarYEditarPreservaElRestoDeLaSecuencia()
        {
            var sut = new BlockSequence();
            sut.Add(InstructionBlock.Forward(2));
            sut.Add(InstructionBlock.Turn(TurnDirection.Right));
            sut.Add(InstructionBlock.Backward(3));
            sut.Add(InstructionBlock.Forward(1));

            sut.RemoveAt(1);
            Assert.That(sut.Blocks, Is.EqualTo(new[] { InstructionBlock.Forward(2), InstructionBlock.Backward(3), InstructionBlock.Forward(1) }),
                "retirar uno deja los demás en su orden");

            sut.Move(2, 0);
            Assert.That(sut.Blocks, Is.EqualTo(new[] { InstructionBlock.Forward(1), InstructionBlock.Forward(2), InstructionBlock.Backward(3) }),
                "reubicar mueve solo ese bloque");

            sut.Replace(1, sut[1].WithCount(5));
            Assert.That(sut.Blocks, Is.EqualTo(new[] { InstructionBlock.Forward(1), InstructionBlock.Forward(5), InstructionBlock.Backward(3) }),
                "editar la cuenta lo cambia en su sitio, sin mover nada");

            sut.Insert(99, InstructionBlock.Turn());
            Assert.That(sut[^1], Is.EqualTo(InstructionBlock.Turn()), "soltar más allá del final engancha al final");
            sut.Insert(-5, InstructionBlock.Turn(TurnDirection.Left));
            Assert.That(sut[0], Is.EqualTo(InstructionBlock.Turn(TurnDirection.Left)), "y antes del principio, al principio");
        }

        [Test]
        public void BlockSequence_CU08_UnaSecuenciaVaciaDevuelveResultadoTipadoNoExcepcion()
        {
            var grid = new MazeGrid(3, 3, new CartState(new Vector2Int(0, 1), Orientation.East), new Vector2Int(2, 1), Array.Empty<Vector2Int>());

            var resultado = SequenceExecutor.Execute(new BlockSequence(), grid);

            Assert.That(resultado.IsEmpty, Is.True, "la UI lo traduce a «añade al menos un bloque» (CU-08 FA-3a)");
            Assert.That(resultado.ReachedGoal, Is.False);
            Assert.That(resultado.Steps, Is.Empty);
        }

        [Test]
        public void BlockSequence_CP02_NoHayLimiteDeBloquesNiDeEdiciones()
        {
            var sut = new BlockSequence();

            for (var i = 0; i < 500; i++)
            {
                sut.Add(InstructionBlock.Forward());
                sut.Move(i, 0);
                sut.Replace(0, sut[0].WithCount(i % 9 + 1));
            }

            Assert.That(sut.Count, Is.EqualTo(500), "quinientos bloques y mil ediciones, sin tope (CP-02, RF-18)");
        }
    }
}
