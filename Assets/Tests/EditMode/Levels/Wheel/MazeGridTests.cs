using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>
    /// La carretilla y la matriz del laberinto (RF-30, RF-31, RF-33, INC-33, guion §6.3.2).
    /// </summary>
    /// <remarks>
    /// EditMode: <see cref="CartState"/> y <see cref="MazeGrid"/> son C# plano. El cableado —la
    /// escena, el arrastre, la animación— va en <c>MazeSceneTests</c>.
    /// </remarks>
    public class MazeGridTests
    {
        private static readonly Vector2Int Origin = new Vector2Int(2, 2);

        /// <summary>El trazado del asset: 16 × 11 con el seto en el anillo, salida (0,8) al este, refugio (15,2).</summary>
        private static MazeLayout Trazado(int seed, int obstacles = 24, int detour = 2) =>
            MazeLayout.Create(16, 11, new Vector2Int(0, 8), Orientation.East, new Vector2Int(15, 2),
                obstacleCount: obstacles, seed: seed, minimumDetour: detour);

        [Test]
        public void CartState_RF31_AvanzarEsRelativoALaOrientacionNoAbsoluto()
        {
            // Misma casilla, cuatro orientaciones: «Avanzar» produce cuatro desplazamientos
            // distintos. Con lectura absoluta («avanzar = arriba») los cuatro serían iguales y
            // esta prueba fallaría — es el error que INC-33 cerró.
            var destinos = new[] { Orientation.North, Orientation.East, Orientation.South, Orientation.West }
                .Select(facing => new CartState(Origin, facing).Ahead().Cell)
                .ToArray();

            Assert.That(destinos.Distinct().Count(), Is.EqualTo(4), "cuatro orientaciones, cuatro casillas");
            Assert.That(new CartState(Origin, Orientation.North).Ahead().Cell, Is.EqualTo(Origin + Vector2Int.up));
            Assert.That(new CartState(Origin, Orientation.East).Ahead().Cell, Is.EqualTo(Origin + Vector2Int.right));
            Assert.That(new CartState(Origin, Orientation.South).Ahead().Cell, Is.EqualTo(Origin + Vector2Int.down));
            Assert.That(new CartState(Origin, Orientation.West).Ahead().Cell, Is.EqualTo(Origin + Vector2Int.left));
        }

        [Test]
        public void CartState_INC33_GirarRota90GradosACadaLadoSinDesplazar()
        {
            var sut = new CartState(Origin, Orientation.North);

            var derecha = sut.TurnedClockwise();
            Assert.That(derecha.Facing, Is.EqualTo(Orientation.East), "norte → este es girar a la derecha");
            Assert.That(derecha.Cell, Is.EqualTo(Origin), "girar no mueve");

            var izquierda = sut.TurnedCounterclockwise();
            Assert.That(izquierda.Facing, Is.EqualTo(Orientation.West), "norte → oeste es girar a la izquierda");
            Assert.That(izquierda.TurnedClockwise(), Is.EqualTo(sut), "un giro a cada lado se anulan");

            var cuatro = derecha.TurnedClockwise().TurnedClockwise().TurnedClockwise();
            Assert.That(cuatro, Is.EqualTo(sut), "cuatro giros devuelven la orientación inicial sin mover");
        }

        [Test]
        public void CartState_RF31_RetrocederNoCambiaLaOrientacion()
        {
            var sut = new CartState(Origin, Orientation.East);

            var atras = sut.Behind();

            Assert.That(atras.Cell, Is.EqualTo(Origin + Vector2Int.left), "una casilla atrás respecto de donde mira");
            Assert.That(atras.Facing, Is.EqualTo(Orientation.East), "y sigue mirando al este");
            Assert.That(atras.Ahead(), Is.EqualTo(sut), "avanzar deshace exactamente el retroceso");
        }

        [Test]
        public void MazeGrid_RF33_UnMovimientoInvalidoDevuelveALaCasillaAnterior()
        {
            var salida = new CartState(new Vector2Int(0, 1), Orientation.East);
            var sut = new MazeGrid(5, 5, salida, new Vector2Int(4, 3), new[] { new Vector2Int(2, 1) });

            var unaCasilla = salida.Ahead();
            Assert.That(sut.TryMove(salida, unaCasilla, out var libre), Is.True, "la primera casilla del interior está libre");
            Assert.That(libre, Is.EqualTo(unaCasilla), "y la carretilla se queda en la nueva");

            Assert.That(sut.TryMove(unaCasilla, unaCasilla.Ahead(), out var contraPiedra), Is.False, "hay una piedra delante");
            Assert.That(contraPiedra, Is.EqualTo(unaCasilla), "la carretilla intenta y vuelve a la casilla anterior");

            var haciaElSeto = unaCasilla.TurnedClockwise();
            Assert.That(sut.TryMove(haciaElSeto, haciaElSeto.Ahead(), out var contraSeto), Is.False, "hacia el sur está el seto");
            Assert.That(contraSeto, Is.EqualTo(haciaElSeto));

            Assert.That(sut.TryMove(salida, salida.Behind(), out var fuera), Is.False, "detrás de la salida no hay matriz");
            Assert.That(fuera, Is.EqualTo(salida));
        }

        [Test]
        public void MazeGrid_RF30_ElAnilloExteriorEsElSetoSalvoLaSalidaYElRefugio()
        {
            var sut = new MazeGrid(16, 11, new CartState(new Vector2Int(0, 8), Orientation.East), new Vector2Int(15, 2), new Vector2Int[0]);

            Assert.That(sut.IsWall(new Vector2Int(0, 0)), Is.True, "esquina");
            Assert.That(sut.IsWall(new Vector2Int(7, 10)), Is.True, "seto de arriba");
            Assert.That(sut.IsWall(new Vector2Int(7, 0)), Is.True, "seto de abajo");
            Assert.That(sut.IsWall(new Vector2Int(15, 5)), Is.True, "seto derecho");
            Assert.That(sut.IsWall(new Vector2Int(0, 8)), Is.False, "la salida es el hueco del seto izquierdo y se pisa");
            Assert.That(sut.IsFree(new Vector2Int(15, 2)), Is.True, "el refugio es el hueco del seto derecho y se pisa");
            Assert.That(sut.IsWall(new Vector2Int(7, 5)), Is.False, "el interior no es seto");
            Assert.That(Enumerable.Range(1, 14).SelectMany(x => Enumerable.Range(1, 9).Select(y => new Vector2Int(x, y))).All(sut.IsFree),
                "las 14 × 9 casillas interiores están libres cuando no hay obstáculos");
        }

        [Test]
        public void MazeGrid_RNF13_ExisteUnaSecuenciaQueAlcanzaElRefugio()
        {
            var layout = Trazado(20260913);

            var sut = MazeGrid.Generate(layout, layout.Seed);

            Assert.That(sut.Obstacles.Count, Is.EqualTo(24), "se sembraron los obstáculos pedidos");
            Assert.That(sut.Obstacles.All(cell => cell.x >= 1 && cell.x <= 14 && cell.y >= 1 && cell.y <= 9), "todos dentro del seto");
            Assert.That(sut.Obstacles, Has.No.Member(layout.StartCell).And.No.Member(layout.GoalCell), "ni la salida ni el refugio se tapan");

            var solucion = sut.Solution();
            Assert.That(solucion, Is.Not.Null, "el laberinto tiene salida");
            var recorrido = SequenceExecutor.Execute(solucion, sut);
            Assert.That(recorrido.ReachedGoal, Is.True, "y la secuencia, ejecutada, llega al refugio");
            Assert.That(recorrido.StoppedAtStep, Is.EqualTo(-1), "sin tropezar con nada");
            Assert.That(solucion.Blocks.All(block => block.Kind != BlockKind.Forward || block.Count <= InstructionBlock.MaxCount),
                "los avances van agrupados como los del mockup, nunca más de 9");

            // El rodeo mínimo: más casillas que la distancia directa (15 + 6), para que no baste
            // con avanzar en línea recta y girar una vez.
            Assert.That(sut.ShortestPath().Count - 1, Is.GreaterThanOrEqualTo(21 + 2));
        }

        [Test]
        public void MazeGrid_RNF18_LaMismaSemillaProduceElMismoTrazadoYSalidaYRefugioNoCambian()
        {
            var layout = Trazado(7);

            var uno = MazeGrid.Generate(layout, 7);
            var dos = MazeGrid.Generate(layout, 7);
            var otro = MazeGrid.Generate(layout, 8);

            Assert.That(uno.Obstacles, Is.EquivalentTo(dos.Obstacles), "misma semilla, mismos obstáculos");
            Assert.That(otro.Obstacles, Is.Not.EquivalentTo(uno.Obstacles), "otra semilla, otro laberinto");
            foreach (var grid in new[] { uno, dos, otro })
            {
                Assert.That(grid.Start, Is.EqualTo(new CartState(new Vector2Int(0, 8), Orientation.East)),
                    "siempre se parte del hueco del seto izquierdo");
                Assert.That(grid.Goal, Is.EqualTo(new Vector2Int(15, 2)), "y siempre se llega al hueco del seto derecho");
                Assert.That(grid.Solution(), Is.Not.Null, "y todos tienen salida");
            }
        }

        [Test]
        public void MazeGrid_RNF13_ConDemasiadosObstaculosSiembraMenosAntesQueDejarElRefugioInalcanzable()
        {
            // 5×5 con 9 obstáculos: se taparía el interior entero, sin camino. Se siembran menos.
            var layout = MazeLayout.Create(5, 5, new Vector2Int(0, 1), Orientation.East, new Vector2Int(4, 3), obstacleCount: 9, seed: 3);

            var sut = MazeGrid.Generate(layout, layout.Seed);

            Assert.That(sut.Obstacles.Count, Is.LessThan(9));
            Assert.That(sut.Solution(), Is.Not.Null, "el laberinto siempre tiene salida (RNF-13)");
        }

        [Test]
        public void MazeLayout_RNF13_LaSecuenciaDeCierreDeLaFase3ExisteEnElProyecto()
        {
            var layout = AssetDatabase.FindAssets($"t:{nameof(MazeLayout)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<MazeLayout>)
                .Single();
            var sequences = AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .ToArray();

            Assert.That(sequences.Select(sequence => sequence.Id), Does.Contain(layout.ClosingSequenceId),
                "llegar al refugio sale a una narrativa que existe: sin ella, pantalla sin salida (RNF-13)");
            Assert.That(sequences.Single(sequence => sequence.Id == "N2_Escena24_Regreso").NextPhase, Is.EqualTo(3),
                "y la escena 2.4 entra al laberinto, que es la fase 3");

            // La matriz del mockup: 16 × 11 con el seto en el anillo, salida y refugio en los huecos.
            Assert.That((layout.Columns, layout.Rows), Is.EqualTo((16, 11)));
            Assert.That(layout.StartCell.x, Is.EqualTo(0), "la salida está en el seto izquierdo");
            Assert.That(layout.GoalCell.x, Is.EqualTo(layout.Columns - 1), "el refugio en el seto derecho");
            Assert.That(layout.BoardMin.x, Is.LessThan(layout.BoardMax.x));
            Assert.That(layout.BoardMin.y, Is.LessThan(layout.BoardMax.y));
            Assert.That(MazeGrid.Generate(layout, 1).Solution(), Is.Not.Null, "el trazado del asset tiene salida");
        }
    }
}
