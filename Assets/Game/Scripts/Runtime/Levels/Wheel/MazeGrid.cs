using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// La matriz del laberinto: tamaño, setos, obstáculos, salida y refugio (RF-30,
    /// RF-33). C# plano; la escena solo la pinta.
    /// </summary>
    /// <remarks>
    /// **El anillo exterior es el seto** y está ocupado salvo la salida y el refugio, que son
    /// los huecos del dibujo. Los obstáculos sembrados van dentro; los setos no se pintan porque
    /// ya están en la ilustración. <see cref="TryMove"/> es la validación por retroceso entera:
    /// un destino ocupado o fuera de la matriz devuelve la casilla de partida. No aborta, no
    /// cuenta, no penaliza (CP-02).
    /// </remarks>
    public class MazeGrid
    {
        private readonly HashSet<Vector2Int> _obstacles;

        public int Columns { get; }

        public int Rows { get; }

        /// <summary>Casilla y orientación con las que arranca cada ejecución (RF-34).</summary>
        public CartState Start { get; }

        public Vector2Int Goal { get; }

        /// <summary>Los obstáculos sembrados: los que se pintan. Los setos no están aquí.</summary>
        public IReadOnlyCollection<Vector2Int> Obstacles => _obstacles;

        public MazeGrid(int columns, int rows, CartState start, Vector2Int goal, IEnumerable<Vector2Int> obstacles)
        {
            Columns = columns;
            Rows = rows;
            Start = start;
            Goal = goal;
            _obstacles = new HashSet<Vector2Int>(obstacles);
        }

        public bool Contains(Vector2Int cell) =>
            cell.x >= 0 && cell.x < Columns && cell.y >= 0 && cell.y < Rows;

        /// <summary>El seto: la casilla está en el borde de la matriz y no es ni la salida ni el refugio.</summary>
        public bool IsWall(Vector2Int cell) =>
            Contains(cell) && cell != Start.Cell && cell != Goal
            && (cell.x == 0 || cell.y == 0 || cell.x == Columns - 1 || cell.y == Rows - 1);

        public bool IsObstacle(Vector2Int cell) => _obstacles.Contains(cell);

        public bool IsFree(Vector2Int cell) => Contains(cell) && !IsWall(cell) && !_obstacles.Contains(cell);

        public bool IsGoal(Vector2Int cell) => cell == Goal;

        /// <summary>
        /// Intenta llegar a <paramref name="to"/>. Si la casilla es libre la carretilla se queda
        /// ahí; si hay seto, obstáculo o está fuera, vuelve a <paramref name="from"/> (RF-33).
        /// </summary>
        public bool TryMove(CartState from, CartState to, out CartState result)
        {
            var valid = IsFree(to.Cell);
            result = valid ? to : from;
            return valid;
        }

        /// <summary>
        /// Siembra obstáculos al azar dentro del seto y garantiza que el refugio siga
        /// alcanzable con un rodeo mínimo. Salida y refugio no cambian nunca: son los huecos
        /// del seto en la ilustración.
        /// </summary>
        /// <remarks>
        /// Los obstáculos van en **tramos** de una a <see cref="MaxSegment"/> casillas en línea,
        /// no sueltos: piedras aisladas en una matriz de 14 × 9 casi nunca obligan a rodear, y sin
        /// rodeo no hay laberinto. Con la misma semilla sale el mismo trazado (RNF-13: se puede
        /// reproducir un recorrido). Si con los obstáculos pedidos no queda camino en
        /// <see cref="Attempts"/> siembras, se siembra uno menos: un laberinto irresoluble sería
        /// una pantalla sin salida.
        /// </remarks>
        public static MazeGrid Generate(MazeLayout layout, int seed)
        {
            var random = new System.Random(seed);
            var start = new CartState(layout.StartCell, layout.StartFacing);
            var empty = new MazeGrid(layout.Columns, layout.Rows, start, layout.GoalCell, Array.Empty<Vector2Int>());
            var interior = new List<Vector2Int>();
            for (var x = 0; x < layout.Columns; x++)
            {
                for (var y = 0; y < layout.Rows; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (empty.IsFree(cell) && cell != layout.StartCell && cell != layout.GoalCell)
                    {
                        interior.Add(cell);
                    }
                }
            }

            var direct = Mathf.Abs(layout.GoalCell.x - layout.StartCell.x) + Mathf.Abs(layout.GoalCell.y - layout.StartCell.y);
            for (var count = Mathf.Min(layout.ObstacleCount, interior.Count); count >= 0; count--)
            {
                for (var attempt = 0; attempt < Attempts; attempt++)
                {
                    var obstacles = SowSegments(interior, count, random);
                    var grid = new MazeGrid(layout.Columns, layout.Rows, start, layout.GoalCell, obstacles);
                    var path = grid.ShortestPath();
                    // Con cero obstáculos el rodeo es imposible: ahí solo se pide que haya camino.
                    if (path != null && (count == 0 || path.Count - 1 >= direct + layout.MinimumDetour))
                    {
                        return grid;
                    }
                }
            }

            return empty;
        }

        // ponytail: tramos al azar y comprobación por BFS; suficiente para 14×9 casillas
        // interiores. Si el laberinto crece, un generador de pasillos (retroceso aleatorio)
        // da rodeos más interesantes.
        private const int Attempts = 64;

        /// <summary>Largo máximo de un tramo de obstáculos, en casillas.</summary>
        private const int MaxSegment = 5;

        private static List<Vector2Int> SowSegments(List<Vector2Int> interior, int count, System.Random random)
        {
            var free = new HashSet<Vector2Int>(interior);
            var sown = new List<Vector2Int>();
            for (var guard = 0; sown.Count < count && guard < 1000; guard++)
            {
                var origin = interior[random.Next(interior.Count)];
                var delta = CartState.Delta((Orientation)random.Next(4));
                var length = 1 + random.Next(MaxSegment);
                for (var k = 0; k < length && sown.Count < count; k++)
                {
                    var cell = origin + delta * k;
                    if (free.Remove(cell))
                    {
                        sown.Add(cell);
                    }
                }
            }

            return sown;
        }

        /// <summary>Las casillas del camino más corto de la salida al refugio, ambas incluidas; null si no hay.</summary>
        internal List<Vector2Int> ShortestPath()
        {
            var previous = new Dictionary<Vector2Int, Vector2Int> { [Start.Cell] = Start.Cell };
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(Start.Cell);

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                if (cell == Goal)
                {
                    var path = new List<Vector2Int>();
                    for (var step = Goal; step != Start.Cell; step = previous[step])
                    {
                        path.Add(step);
                    }

                    path.Add(Start.Cell);
                    path.Reverse();
                    return path;
                }

                foreach (Orientation facing in Enum.GetValues(typeof(Orientation)))
                {
                    var next = cell + CartState.Delta(facing);
                    if (IsFree(next) && previous.TryAdd(next, cell))
                    {
                        queue.Enqueue(next);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Una secuencia que lleva la carretilla al refugio —avances agrupados, giros al lado más
        /// corto—, o null si el laberinto no tiene salida. **Solo para las pruebas**: el guía nunca
        /// la enseña ni la sugiere (CP-06); el jugador compone la suya.
        /// </summary>
        internal BlockSequence Solution()
        {
            var path = ShortestPath();
            if (path == null)
            {
                return null;
            }

            var sequence = new BlockSequence();
            var state = Start;
            var run = 0;
            for (var i = 1; i < path.Count; i++)
            {
                var turns = 0;
                while (state.Ahead().Cell != path[i])
                {
                    state = state.TurnedClockwise();
                    turns++;
                }

                if (turns > 0)
                {
                    Flush(sequence, ref run);
                    sequence.Add(turns == 3 ? InstructionBlock.Turn(TurnDirection.Left) : InstructionBlock.Turn(TurnDirection.Right));
                    if (turns == 2)
                    {
                        sequence.Add(InstructionBlock.Turn(TurnDirection.Right));
                    }
                }

                state = state.Ahead();
                run++;
                if (run == InstructionBlock.MaxCount)
                {
                    Flush(sequence, ref run);
                }
            }

            Flush(sequence, ref run);
            return sequence;
        }

        private static void Flush(BlockSequence sequence, ref int run)
        {
            if (run > 0)
            {
                sequence.Add(InstructionBlock.Forward(run));
                run = 0;
            }
        }
    }
}
