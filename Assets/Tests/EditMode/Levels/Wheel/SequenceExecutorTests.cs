using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>El recorrido paso a paso y la validación por retroceso (RF-32, RF-33, CP-06).</summary>
    public class SequenceExecutorTests
    {
        // 5×5 con el seto en el anillo: interior 3×3. Salida en el hueco izquierdo (0,1) mirando
        // al este, refugio en el hueco derecho (4,3), piedra en (2,1).
        private static MazeGrid Rejilla() => new MazeGrid(5, 5,
            new CartState(new Vector2Int(0, 1), Orientation.East), new Vector2Int(4, 3), new[] { new Vector2Int(2, 1) });

        private static BlockSequence Secuencia(params InstructionBlock[] blocks)
        {
            var sequence = new BlockSequence();
            foreach (var block in blocks)
            {
                sequence.Add(block);
            }

            return sequence;
        }

        [Test]
        public void SequenceExecutor_RF32_RecorreLaSecuenciaPasoAPasoResaltandoElBloqueEnCurso()
        {
            var sut = Secuencia(InstructionBlock.Forward(1), InstructionBlock.Turn(TurnDirection.Left), InstructionBlock.Forward(2));

            var resultado = SequenceExecutor.Execute(sut, Rejilla());

            Assert.That(resultado.Steps.Select(step => step.Index), Is.EqualTo(new[] { 0, 1, 2 }),
                "un paso por bloque, en orden, con el índice que la escena resalta");
            Assert.That(resultado.Steps.Select(step => step.Block), Is.EqualTo(sut.Blocks));
            Assert.That(resultado.Steps[0].Moves.Count, Is.EqualTo(1), "«Avanzar ×1» es un movimiento");
            Assert.That(resultado.Steps[1].After.Facing, Is.EqualTo(Orientation.North), "girar a la izquierda desde el este mira al norte");
            Assert.That(resultado.Steps[2].Moves.Count, Is.EqualTo(2), "«Avanzar ×2» son dos movimientos de una casilla");
            Assert.That(resultado.Steps[2].After.Cell, Is.EqualTo(new Vector2Int(1, 3)));
            Assert.That(resultado.ReachedGoal, Is.False);
        }

        [Test]
        public void SequenceExecutor_RF33_UnMovimientoInvalidoRetrocedeYLaEjecucionContinua()
        {
            // «Avanzar ×3» topa con la piedra en la segunda casilla: intenta, vuelve, y no sigue
            // probando la tercera; el giro y el avance siguientes se ejecutan igual.
            var sut = Secuencia(InstructionBlock.Forward(3), InstructionBlock.Turn(TurnDirection.Left), InstructionBlock.Forward(1));

            var resultado = SequenceExecutor.Execute(sut, Rejilla());

            var avance = resultado.Steps[0];
            Assert.That(avance.Moves.Count, Is.EqualTo(2), "la primera casilla se pisa, la segunda se intenta, la tercera ya no");
            Assert.That(avance.Moves[1].Blocked, Is.True, "el segundo movimiento topa con la piedra");
            Assert.That(avance.Moves[1].Attempted.Cell, Is.EqualTo(new Vector2Int(2, 1)), "la carretilla la intenta");
            Assert.That(avance.Moves[1].To, Is.EqualTo(avance.Moves[1].From), "y vuelve a la casilla anterior");
            Assert.That(avance.After.Cell, Is.EqualTo(new Vector2Int(1, 1)));
            Assert.That(resultado.Steps.Count, Is.EqualTo(3), "la ejecución no se aborta: siguen los otros dos (CP-02)");
            Assert.That(resultado.Steps[2].After.Cell, Is.EqualTo(new Vector2Int(1, 2)), "y el último avance sí mueve");
            Assert.That(resultado.StoppedAtStep, Is.EqualTo(0), "el resultado dice en qué paso se detuvo el avance");

            // Contra el seto pasa lo mismo: desde la salida, girar a la izquierda y avanzar.
            var contraElSeto = SequenceExecutor.Execute(Secuencia(InstructionBlock.Turn(TurnDirection.Left), InstructionBlock.Forward(1)), Rejilla());
            Assert.That(contraElSeto.Steps[1].Blocked, Is.True, "el anillo de arbustos está ocupado");
            Assert.That(contraElSeto.Steps[1].After.Cell, Is.EqualTo(new Vector2Int(0, 1)));
        }

        [Test]
        public void SequenceExecutor_RF31_RetrocederMueveHaciaAtrasSinCambiarLaOrientacion()
        {
            // Una casilla adelante, giro a la izquierda (mira al norte), dos arriba y «Retroceder ×3»:
            // baja dos y la tercera topa con el seto de abajo. Sigue mirando al norte.
            var sut = Secuencia(InstructionBlock.Forward(1), InstructionBlock.Turn(TurnDirection.Left), InstructionBlock.Forward(2),
                InstructionBlock.Backward(3));

            var resultado = SequenceExecutor.Execute(sut, Rejilla());

            var atras = resultado.Steps[3];
            Assert.That(atras.Before.Cell, Is.EqualTo(new Vector2Int(1, 3)));
            Assert.That(atras.Moves.Count, Is.EqualTo(3), "dos casillas atrás y un intento contra el seto");
            Assert.That(atras.Moves[2].Blocked, Is.True);
            Assert.That(atras.After.Cell, Is.EqualTo(new Vector2Int(1, 1)), "retrocede hacia donde no mira");
            Assert.That(atras.After.Facing, Is.EqualTo(Orientation.North), "sin cambiar la orientación (RF-31)");
            Assert.That(resultado.StoppedAtStep, Is.EqualTo(3));
        }

        [Test]
        public void SequenceExecutor_CP06_ElResultadoNoNombraElBloqueQueDebeCorregirse()
        {
            var resultado = SequenceExecutor.Execute(Secuencia(InstructionBlock.Forward(3)), Rejilla());

            // Lo que expone el resultado es dónde se detuvo el avance. Ninguna propiedad
            // sugiere un bloque a cambiar ni una corrección: esa deducción es del jugador.
            var propiedades = typeof(ExecutionResult).GetProperties();
            Assert.That(propiedades.Select(property => property.Name), Is.EquivalentTo(new[] { "Steps", "ReachedGoal", "IsEmpty", "StoppedAtStep" }));
            Assert.That(propiedades.Select(property => property.PropertyType),
                Has.None.EqualTo(typeof(InstructionBlock)).And.None.EqualTo(typeof(BlockKind)), "el resultado no señala un bloque concreto");
            Assert.That(propiedades.Select(property => property.PropertyType),
                Has.None.EqualTo(typeof(float)).And.None.EqualTo(typeof(double)), "ni lleva cifras de desempeño (CP-03)");
            Assert.That(resultado.StoppedAtStep, Is.EqualTo(0));
        }

        [Test]
        public void SequenceExecutor_RNF13_LaSecuenciaCorrectaAlcanzaElRefugio()
        {
            // Este, piedra delante: una casilla, giro a la izquierda, dos arriba, giro a la
            // derecha y tres a la derecha hasta el hueco del refugio. El bloque sobrante no corre.
            var sut = Secuencia(InstructionBlock.Forward(1), InstructionBlock.Turn(TurnDirection.Left), InstructionBlock.Forward(2),
                InstructionBlock.Turn(TurnDirection.Right), InstructionBlock.Forward(3), InstructionBlock.Backward(1));

            var resultado = SequenceExecutor.Execute(sut, Rejilla());

            Assert.That(resultado.ReachedGoal, Is.True);
            Assert.That(resultado.Steps.Count, Is.EqualTo(5), "al alcanzar el refugio la fase termina: el bloque sobrante no se ejecuta");
            Assert.That(resultado.Steps[^1].After.Cell, Is.EqualTo(new Vector2Int(4, 3)));
            Assert.That(resultado.StoppedAtStep, Is.EqualTo(-1));

            var solucion = SequenceExecutor.Execute(Rejilla().Solution(), Rejilla());
            Assert.That(solucion.ReachedGoal, Is.True, "y la solución que calcula la rejilla (solo para pruebas) también llega");
        }
    }
}
