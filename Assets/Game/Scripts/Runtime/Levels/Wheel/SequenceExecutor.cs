using System.Collections.Generic;

namespace Game.Levels.Wheel
{
    /// <summary>Un movimiento de una casilla —o un giro—: de dónde salió, qué intentó y dónde quedó (RF-33).</summary>
    public readonly struct CartMove
    {
        public CartState From { get; }

        /// <summary>La casilla que se pedía. Si <see cref="Blocked"/>, la carretilla la intentó y volvió.</summary>
        public CartState Attempted { get; }

        public CartState To { get; }

        public bool Blocked { get; }

        public CartMove(CartState from, CartState attempted, CartState to, bool blocked)
        {
            From = from;
            Attempted = attempted;
            To = to;
            Blocked = blocked;
        }
    }

    /// <summary>Qué le pasó a la carretilla en un bloque: sus movimientos y si tropezó (RF-32, RF-33).</summary>
    public readonly struct ExecutionStep
    {
        public int Index { get; }

        public InstructionBlock Block { get; }

        /// <summary>Un movimiento por casilla de «Avanzar» o «Retroceder», uno por «Girar».</summary>
        public IReadOnlyList<CartMove> Moves { get; }

        public CartState Before { get; }

        public CartState After { get; }

        /// <summary>Algún movimiento del bloque tropezó: la carretilla lo intentó y volvió.</summary>
        public bool Blocked { get; }

        public ExecutionStep(int index, InstructionBlock block, IReadOnlyList<CartMove> moves, CartState before, CartState after, bool blocked)
        {
            Index = index;
            Block = block;
            Moves = moves;
            Before = before;
            After = after;
            Blocked = blocked;
        }
    }

    /// <summary>
    /// El resultado de una ejecución. Dice **dónde** se detuvo el avance y **nunca** qué bloque
    /// corregir (CP-06): esa deducción es del jugador. Tampoco lleva cifras de desempeño (CP-03).
    /// </summary>
    public class ExecutionResult
    {
        public IReadOnlyList<ExecutionStep> Steps { get; }

        public bool ReachedGoal { get; }

        public bool IsEmpty => Steps.Count == 0;

        /// <summary>Índice del primer bloque en que la carretilla intentó y no pudo; -1 si ninguno.</summary>
        public int StoppedAtStep { get; }

        public ExecutionResult(IReadOnlyList<ExecutionStep> steps, bool reachedGoal, int stoppedAtStep)
        {
            Steps = steps;
            ReachedGoal = reachedGoal;
            StoppedAtStep = stoppedAtStep;
        }
    }

    /// <summary>
    /// Recorre la secuencia bloque a bloque sobre la rejilla (RF-32). Es C# plano: la pausa
    /// entre pasos y el resaltado son cosa de la escena.
    /// </summary>
    /// <remarks>
    /// **El retroceso es el mecanismo de depuración del nivel.** Un movimiento inválido se
    /// intenta, vuelve a la casilla anterior y la ejecución **continúa** con el bloque siguiente:
    /// no se aborta ni se penaliza (RF-33, CP-02). Dentro de un «Avanzar ×n» o «Retroceder ×n»
    /// se tropieza una sola vez —las casillas que faltaban ya no se intentan— para que el intento se lea claro.
    /// Quien haga que un bloque fallido corte la ejecución o señale «este bloque está mal»
    /// reintroduce lo que CP-06 prohíbe.
    /// </remarks>
    public static class SequenceExecutor
    {
        public static ExecutionResult Execute(BlockSequence sequence, MazeGrid grid)
        {
            var steps = new List<ExecutionStep>();
            var state = grid.Start;
            var stoppedAt = -1;
            var reached = grid.IsGoal(state.Cell);

            for (var i = 0; i < sequence.Count && !reached; i++)
            {
                var block = sequence[i];
                var before = state;
                var moves = new List<CartMove>();
                var blocked = false;

                switch (block.Kind)
                {
                    case BlockKind.Forward:
                    case BlockKind.Backward:
                        for (var n = 0; n < block.Count && !reached; n++)
                        {
                            // «Retroceder» va hacia atrás respecto de donde mira, sin girar (RF-31).
                            var attempted = block.Kind == BlockKind.Forward ? state.Ahead() : state.Behind();
                            var moved = grid.TryMove(state, attempted, out var after);
                            moves.Add(new CartMove(state, attempted, after, !moved));
                            state = after;
                            reached = grid.IsGoal(state.Cell);
                            if (!moved)
                            {
                                blocked = true;
                                break;
                            }
                        }

                        break;
                    default:
                        var turned = block.Direction == TurnDirection.Left ? state.TurnedCounterclockwise() : state.TurnedClockwise();
                        moves.Add(new CartMove(state, turned, turned, false));
                        state = turned;
                        break;
                }

                if (blocked && stoppedAt < 0)
                {
                    stoppedAt = i;
                }

                steps.Add(new ExecutionStep(i, block, moves, before, state, blocked));
            }

            return new ExecutionResult(steps, reached, stoppedAt);
        }
    }
}
