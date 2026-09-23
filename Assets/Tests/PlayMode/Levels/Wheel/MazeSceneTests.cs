using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Audio;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.Utils;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>
    /// La escena del laberinto según el mockup 10: presentación, bloques encajables, cajón,
    /// compresión, ejecución y reintento (W13, W14).
    /// </summary>
    [Category("Integration")]
    public class MazeSceneTests
    {
        private const string SceneName = "Level2_Maze";

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF30_PresentaCarretillaRefugioYObstaculos()
        {
            var maze = await OpenMaze();
            var nombres = maze.Pieces.Select(piece => piece.name).ToArray();

            Assert.That(nombres, Does.Contain("Refugio"), "el refugio está en el hueco del seto derecho");
            Assert.That(nombres, Does.Contain(maze.Cart.name), "y la carretilla en el izquierdo");
            Assert.That(nombres.Count(nombre => nombre.StartsWith("Obstaculo_")), Is.EqualTo(maze.Grid.Obstacles.Count),
                "un obstáculo pintado por cada uno de la matriz (RF-30)");
            Assert.That(maze.Grid.Obstacles, Is.Not.Empty, "y hay obstáculos que rodear");
            foreach (var piece in maze.Pieces)
            {
                Assert.That(piece.parent, Is.SameAs(maze.Environment.rectTransform),
                    $"{piece.name} cuelga del entorno: acompaña cualquier resolución del arte (RNF-23)");
            }

            Assert.That(maze.Cart.anchorMin, Is.EqualTo(maze.CellAnchor(maze.Grid.Start.Cell)).Using(Vector2EqualityComparer.Instance),
                "la carretilla arranca en la salida");
            Assert.That(maze.IsPaletteOpen, Is.False, "el cajón «Bloques» empieza cerrado (mockup 10)");
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RNF23_ElEntornoCabeEnteroEnSuPanelSinDeformarseYLaMatrizCubreElSeto()
        {
            var maze = await OpenMaze();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var sprite = maze.Environment.sprite;
            Assert.That(sprite, Is.Not.Null, "el laberinto tiene entorno cenital");
            var entorno = EnPantalla(maze.Environment.rectTransform);
            var panel = EnPantalla((RectTransform)maze.Environment.rectTransform.parent);
            var pantalla = new Rect(0f, 0f, Screen.width, Screen.height);

            // Entero y sin deformar: los setos son el laberinto, recortarlos lo rompería.
            Assert.That(entorno.xMin, Is.GreaterThanOrEqualTo(panel.xMin - 0.5f));
            Assert.That(entorno.xMax, Is.LessThanOrEqualTo(panel.xMax + 0.5f));
            Assert.That(entorno.yMin, Is.GreaterThanOrEqualTo(panel.yMin - 0.5f));
            Assert.That(entorno.yMax, Is.LessThanOrEqualTo(panel.yMax + 0.5f));
            Assert.That(entorno.width / entorno.height, Is.EqualTo(sprite.rect.width / sprite.rect.height).Within(0.01f),
                "conserva la proporción del dibujo");
            Assert.That(Mathf.Approximately(entorno.width, panel.width) || Mathf.Approximately(entorno.height, panel.height),
                "y llena el panel por uno de los dos ejes: se ajusta a la pantalla");

            // La matriz de 16 × 11 cubre el seto entero: sus casillas miden ~8 % del alto y las
            // piezas quedan dentro del entorno y fuera del área de bloques (pantalla dividida).
            var casilla = (maze.Layout.BoardMax - maze.Layout.BoardMin) / new Vector2(maze.Grid.Columns, maze.Grid.Rows);
            Assert.That(casilla.y, Is.EqualTo(0.087f).Within(0.01f), "una casilla es ~8 % del alto de la ilustración");
            Assert.That(casilla.x * sprite.rect.width / (casilla.y * sprite.rect.height), Is.EqualTo(1f).Within(0.05f), "y es cuadrada");

            var secuencia = EnPantalla(maze.SequenceList);
            foreach (var piece in maze.Pieces)
            {
                var caja = EnPantalla(piece);
                Assert.That(caja.xMin, Is.GreaterThanOrEqualTo(entorno.xMin - 0.5f), $"{piece.name} dentro del entorno");
                Assert.That(caja.xMax, Is.LessThanOrEqualTo(entorno.xMax + 0.5f));
                Assert.That(caja.yMin, Is.GreaterThanOrEqualTo(entorno.yMin - 0.5f));
                Assert.That(caja.yMax, Is.LessThanOrEqualTo(entorno.yMax + 0.5f));
                Assert.That(caja.Overlaps(secuencia), Is.False, $"{piece.name} no cae bajo el área de bloques");
                Assert.That(pantalla.Contains(caja.min) && pantalla.Contains(caja.max), $"{piece.name} está en pantalla (RNF-03)");
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_PG04_EjecutarRespondeAClicSimpleNoADobleClic()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Turn());

            maze.ExecuteButton.onClick.Invoke();

            Assert.That(maze.IsExecuting, Is.True, "un solo clic arranca la ejecución (PG-04, RF-32)");
            Assert.That(maze.Executions, Is.EqualTo(1));
            maze.ExecuteButton.onClick.Invoke();
            Assert.That(maze.Executions, Is.EqualTo(1), "un segundo clic mientras corre no arranca otra");
            Assert.That(maze.ExecuteButton.interactable, Is.False, "y el botón lo dice mientras tanto");
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RNF19_LosTresBloquesSeDistinguenPorFormaYElCajonLosGuarda()
        {
            var maze = await OpenMaze();
            Assert.That(maze.PaletteBlocks.Any(bloque => bloque.gameObject.activeInHierarchy), Is.False, "cerrado, la paleta no se ve");

            maze.PaletteToggle.onClick.Invoke();
            Assert.That(maze.IsPaletteOpen, Is.True, "un clic en «Bloques» abre el cajón");
            var bloques = maze.PaletteBlocks.ToArray();

            Assert.That(bloques.Select(bloque => bloque.name),
                Is.EquivalentTo(new[] { "Bloque_Forward", "Bloque_Backward", "Bloque_Turn" }), "tres bloques (mockup 10, RF-31)");
            Assert.That(bloques.Select(bloque => bloque.GetComponentInChildren<Text>().text).Distinct().Count(), Is.EqualTo(3),
                "cada uno con su nombre");

            // Tres siluetas, no tres colores (RNF-19): «Avanzar» es un rectángulo, «Girar» una
            // píldora (marco redondo) y «Retroceder» lleva el rombo de encaje a la derecha.
            var formas = bloques.Select(bloque => (bloque.GetComponent<Image>().sprite.name, bloque.Find("Fondo/Rombo").gameObject.activeSelf)).ToArray();
            Assert.That(formas.Distinct().Count(), Is.EqualTo(3));
            Assert.That(bloques.Single(b => b.name == "Bloque_Turn").GetComponent<Image>().sprite,
                Is.Not.SameAs(bloques.Single(b => b.name == "Bloque_Forward").GetComponent<Image>().sprite), "«Girar» es la píldora");
            Assert.That(bloques.Single(b => b.name == "Bloque_Backward").Find("Fondo/Rombo").gameObject.activeSelf, "«Retroceder» lleva rombo");
            Assert.That(bloques.All(bloque => bloque.Find("Pestana").gameObject.activeSelf), "y en la paleta todos muestran su pestaña de encaje");

            maze.PaletteToggle.onClick.Invoke();
            Assert.That(maze.IsPaletteOpen, Is.False, "otro clic lo cierra");
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF31_LaOrientacionDeLaCarretillaEsVisibleYGiraACadaLado()
        {
            var maze = await OpenMaze();
            var inicio = maze.Grid.Start;
            Assert.That(maze.ShownFacing, Is.EqualTo(inicio.Facing), "al abrir, la carretilla mira hacia donde dice la matriz");

            maze.AddBlock(InstructionBlock.Turn(TurnDirection.Right));
            maze.Execute();
            await Esperar(() => maze.ShownFacing == inicio.TurnedClockwise().Facing);
            await Esperar(() => !maze.IsExecuting);
            Assert.That(maze.ShownFacing, Is.EqualTo(inicio.Facing), "al terminar vuelve a la salida y a su orientación");

            maze.SetDirection(0, TurnDirection.Left);
            Assert.That(maze.Sequence[0].Direction, Is.EqualTo(TurnDirection.Left), "el lado se edita en el bloque");
            maze.Execute();
            await Esperar(() => maze.ShownFacing == inicio.TurnedCounterclockwise().Facing);
            Assert.That(maze.ShownFacing, Is.EqualTo(inicio.TurnedCounterclockwise().Facing),
                "y la rotación en pantalla lo muestra: sin eso la lectura relativa es ilegible (INC-33)");
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF31_ElContadorDeAvanzarYRetrocederSeEditaEnElBloqueEntre1Y9()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Forward(2));
            maze.AddBlock(InstructionBlock.Backward(1));
            var fila = maze.Rows[0];

            Assert.That(fila.Find("Fondo/Contador").gameObject.activeSelf, Is.True, "desplegado, «Avanzar» muestra − n +");
            Assert.That(maze.Rows[1].Find("Fondo/Contador").gameObject.activeSelf, Is.True, "y «Retroceder» también");
            maze.Rows[1].Find("Fondo/Contador/Mas").GetComponent<Button>().onClick.Invoke();
            Assert.That(maze.Sequence[1], Is.EqualTo(InstructionBlock.Backward(2)), "su cuenta se edita igual");
            Assert.That(fila.Find("Fondo/Contador/Cifra").GetComponent<Text>().text, Is.EqualTo("2"));

            fila.Find("Fondo/Contador/Mas").GetComponent<Button>().onClick.Invoke();
            Assert.That(maze.Sequence[0].Count, Is.EqualTo(3), "«+» suma uno en su sitio");
            Assert.That(maze.Rows[0].Find("Fondo/Contador/Cifra").GetComponent<Text>().text, Is.EqualTo("3"));

            for (var i = 0; i < 10; i++)
            {
                maze.Rows[0].Find("Fondo/Contador/Mas").GetComponent<Button>().onClick.Invoke();
            }

            Assert.That(maze.Sequence[0].Count, Is.EqualTo(9), "no pasa de 9 (mockup)");
            for (var i = 0; i < 12; i++)
            {
                maze.Rows[0].Find("Fondo/Contador/Menos").GetComponent<Button>().onClick.Invoke();
            }

            Assert.That(maze.Sequence[0].Count, Is.EqualTo(1), "ni baja de 1");
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF34_TrasUnaEjecucionFallidaLaSecuenciaPermaneceEnPantalla()
        {
            var maze = await OpenMaze();
            // Desde la salida, mirando al este, girar a la izquierda y avanzar topa con el seto:
            // se intenta y se vuelve.
            maze.AddBlock(InstructionBlock.Turn(TurnDirection.Left));
            maze.AddBlock(InstructionBlock.Forward(1));

            maze.Execute();
            await Esperar(() => !maze.IsExecuting);

            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[] { InstructionBlock.Turn(TurnDirection.Left), InstructionBlock.Forward(1) }),
                "la secuencia sigue intacta para corregirla (RF-34)");
            Assert.That(maze.Rows.Count, Is.EqualTo(2), "y sigue en pantalla");
            Assert.That(maze.Cart.anchorMin, Is.EqualTo(maze.CellAnchor(maze.Grid.Start.Cell)).Using(Vector2EqualityComparer.Instance),
                "la carretilla volvió a la salida");
            Assert.That(maze.ShownFacing, Is.EqualTo(maze.Grid.Start.Facing), "mirando como al principio");
            Assert.That(maze.Rows[1].GetComponent<Outline>().enabled, Is.True,
                "el paso donde se detuvo el avance queda resaltado: eso es lo que el jugador lee (CP-06)");
            Assert.That(maze.Rows[1].localScale.x, Is.GreaterThan(1f), "con contorno y tamaño: dos indicadores (RNF-19)");
            Assert.That(maze.MessageLabel.text, Is.EqualTo(maze.Layout.StoppedMessage));
            Assert.That(maze.MessageLabel.text, Does.Not.Match(@"\d"), "sin cifras (CP-03)");
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneName), "el escenario no se recargó (CU-08 FA-6a)");
        }

        /// <summary>
        /// La carretilla suena mientras recorre la secuencia —también en el intento que choca y
        /// vuelve, que se oye como rueda y nunca como error (RF-33, §2.1, CP-02)— y calla al
        /// terminar; debajo sigue el bosque de día.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF32_LaCarretillaSuenaMientrasRecorreLaSecuenciaYCallaAlTerminar()
        {
            var audio = new GameObject("TestAudio").AddComponent<AudioManager>();
            try
            {
                var maze = await OpenMaze();
                var sounds = maze.Sounds;
                Assume.That(sounds, Is.Not.Null, "Level2_Maze tiene N2_Sonidos asignado");
                Assert.That(audio.AmbientClip, Is.SameAs(sounds.ForestAmbient), "el bosque de día de fondo");

                // Girar a la izquierda y avanzar topa con el seto: se intenta y se vuelve.
                maze.AddBlock(InstructionBlock.Turn(TurnDirection.Left));
                maze.AddBlock(InstructionBlock.Forward(1));
                maze.Execute();

                Assert.That(audio.HeldClip, Is.SameAs(sounds.CartMove), "la carretilla rueda mientras recorre la secuencia");

                await Esperar(() => !maze.IsExecuting);

                Assert.That(audio.HeldClip, Is.Null, "y calla al terminar");
                Assert.That(audio.AmbientClip, Is.SameAs(sounds.ForestAmbient), "el bosque sigue debajo");
            }
            finally
            {
                Object.DestroyImmediate(audio.gameObject);
            }
        }

        [Test]
        [Timeout(60000)]
        public async Task MazeScene_CP02_NoHayLimiteDeEjecucionesNiPantallaDeDerrota()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Turn(TurnDirection.Left));
            maze.AddBlock(InstructionBlock.Forward(1));

            for (var i = 0; i < 3; i++)
            {
                maze.Execute();
                await Esperar(() => !maze.IsExecuting);
            }

            Assert.That(maze.Executions, Is.EqualTo(3));
            Assert.That(maze.ExecuteButton.interactable, Is.True, "se puede volver a ejecutar cuantas veces haga falta (CP-02, RF-18)");
            Assert.That(maze.Sequence.Count, Is.EqualTo(2));
            Assert.That(Object.FindObjectsByType<Transform>(FindObjectsInactive.Include)
                    .Select(transform => transform.name.ToLowerInvariant())
                    .Any(nombre => nombre.Contains("derrota") || nombre.Contains("gameover") || nombre.Contains("perdiste")),
                Is.False, "no existe pantalla de derrota (CP-02)");
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF34_SoltarSobreLaSecuenciaEnganchaYSoltarFueraRetira()
        {
            var maze = await OpenMaze();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var lista = EnPantalla(maze.SequenceList);

            maze.TakeFromPalette(InstructionBlock.Default(BlockKind.Forward));
            Assert.That(maze.Held, Is.Not.Null, "el clic sostenido toma una copia del bloque");
            maze.Drop(lista.center);
            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[] { InstructionBlock.Forward(1) }), "soltarlo sobre la secuencia lo engancha");
            Assert.That(maze.Held, Is.Null);

            maze.TakeFromPalette(InstructionBlock.Default(BlockKind.Turn));
            maze.Drop(new Vector2(lista.center.x, lista.yMax - 1f));
            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[] { InstructionBlock.Turn(), InstructionBlock.Forward(1) }),
                "soltarlo arriba lo engancha antes: el orden es el de la posición donde cae");
            Assert.That(maze.Rows[1].Find("Muesca").gameObject.activeSelf && maze.Rows[1].Find("Pestana").gameObject.activeSelf,
                "el último bloque muestra la muesca de arriba y la pestaña de abajo: van encajados");

            maze.TakeFromSequence(1);
            Assert.That(maze.Sequence.Count, Is.EqualTo(1), "tomar un bloque enganchado lo saca mientras se sostiene");
            maze.Drop(new Vector2(10f, 10f));
            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[] { InstructionBlock.Turn() }), "soltarlo fuera lo retira (RF-34)");

            maze.TakeFromPalette(InstructionBlock.Default(BlockKind.Backward));
            maze.Drop(new Vector2(10f, 10f));
            Assert.That(maze.Sequence.Count, Is.EqualTo(1), "un bloque de la paleta soltado fuera no hace nada");
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RNF03_AlAcumularseLosBloquesSeComprimenYLaFlechaDespliegaUno()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Forward(2));
            maze.AddBlock(InstructionBlock.Turn());
            Assert.That(maze.IsCompact, Is.False, "con pocos bloques todos van desplegados");
            Assert.That(maze.Rows[0].Find("Fondo/Contador").gameObject.activeSelf, Is.True);
            Assert.That(maze.Rows[0].Find("Fondo/Flecha").gameObject.activeSelf, Is.False, "sin flecha: no hay nada que desplegar");

            for (var i = 0; i < 10; i++)
            {
                maze.AddBlock((i % 3) switch
                {
                    0 => InstructionBlock.Forward(i % 9 + 1),
                    1 => InstructionBlock.Turn(),
                    _ => InstructionBlock.Backward(2)
                });
            }

            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Canvas.ForceUpdateCanvases();

            Assert.That(maze.IsCompact, Is.True, "doce bloques no caben desplegados: se comprimen");
            Assert.That(maze.Rows.Where(fila => fila.GetComponentsInChildren<Text>().Any(t => t.text == "Avanzar"))
                    .All(fila => !fila.Find("Fondo/Contador").gameObject.activeSelf), "comprimidos, desaparece el contador");
            Assert.That(maze.Rows.All(fila => fila.Find("Fondo/Flecha").gameObject.activeSelf), "y aparece la flecha «→»");

            var lista = EnPantalla(maze.SequenceList);
            var filas = maze.Rows.Select(EnPantalla).ToArray();
            for (var i = 0; i < filas.Length; i++)
            {
                Assert.That(filas[i].yMin, Is.GreaterThanOrEqualTo(lista.yMin - 0.5f), $"Paso_{i} no se sale por abajo");
                Assert.That(filas[i].yMax, Is.LessThanOrEqualTo(lista.yMax + 0.5f), $"Paso_{i} no se sale por arriba");
                Assert.That(filas[i].height, Is.GreaterThan(24f), $"Paso_{i} sigue siendo legible");
                for (var j = i + 1; j < filas.Length; j++)
                {
                    Assert.That(Solapan(filas[i], filas[j]), Is.False, $"Paso_{i} y Paso_{j} no se solapan");
                }
            }

            Assert.That(filas.Select(fila => fila.center.y), Is.Ordered.Descending, "de arriba abajo, en el orden de la secuencia");

            // Paso_2 es el primer «Avanzar» añadido en el bucle; Paso_3 es un «Girar».
            maze.Rows[2].Find("Fondo/Flecha").GetComponent<Button>().onClick.Invoke();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Assert.That(maze.Expanded, Is.EqualTo(2), "la flecha despliega ese bloque");
            Assert.That(maze.Rows[2].Find("Fondo/Contador").gameObject.activeSelf, Is.True, "y vuelve su contador para editarlo");
            Assert.That(maze.Rows[2].Find("Fondo/Flecha").gameObject.activeSelf, Is.False);
            Assert.That(EnPantalla(maze.Rows[2]).height, Is.GreaterThan(EnPantalla(maze.Rows[3]).height), "desplegado es más alto que los comprimidos");
            maze.Rows[3].Find("Fondo/Flecha").GetComponent<Button>().onClick.Invoke();
            Assert.That(maze.Expanded, Is.EqualTo(3), "desplegar otro comprime el anterior");
            Assert.That(maze.Rows[3].Find("Fondo/Lado").gameObject.activeSelf, Is.True, "«Girar» desplegado muestra su lado");
            Assert.That(maze.Rows[2].Find("Fondo/Flecha").gameObject.activeSelf, Is.True);
            Assert.That(maze.Rows.Count(fila => fila.Find("Fondo/Flecha").gameObject.activeSelf), Is.EqualTo(maze.Rows.Count - 1),
                "uno desplegado a la vez");
        }

        /// <summary>
        /// Con más bloques de los que caben ni comprimidos, la lista **se desplaza**: las filas
        /// conservan su alto y se recorren con los dos botones, que es el único esquema de entrada
        /// que hay (RNF-02, CT-06: ni rueda ni arrastre). Antes se repartían el sitio a partes
        /// iguales y acababan una encima de otra, con el rótulo saliéndose de su marco.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RNF03_ConMasBloquesDeLosQueCabenLaListaSeDesplazaSinEncogerLasFilas()
        {
            var maze = await OpenMaze();
            for (var i = 0; i < 20; i++)
            {
                maze.AddBlock((i % 3) switch
                {
                    0 => InstructionBlock.Forward(i % 9 + 1),
                    1 => InstructionBlock.Turn(),
                    _ => InstructionBlock.Backward(2)
                });
            }

            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Canvas.ForceUpdateCanvases();

            var ventana = EnPantalla(maze.SequenceViewport);
            var filas = maze.Rows.Select(EnPantalla).ToArray();

            Assert.That(filas.Select((fila, i) => (i, fila.height)).Where(par => par.height < 60f),
                Is.Empty, "ninguna fila se encoge por debajo de lo legible");
            for (var i = 0; i < filas.Length; i++)
            {
                for (var j = i + 1; j < filas.Length; j++)
                {
                    Assert.That(Solapan(filas[i], filas[j]), Is.False, $"Paso_{i} y Paso_{j} no se solapan");
                }
            }

            Assert.That(maze.ScrollUpButton.gameObject.activeSelf, Is.True, "aparecen los botones de desplazamiento");
            Assert.That(maze.ScrollDownButton.gameObject.activeSelf, Is.True);

            // Al añadir, la lista se queda mirando el final: el bloque recién puesto y la casilla
            // «Suelta un bloque aquí» están a la vista.
            Assert.That(EnPantalla(maze.DropZone).yMin, Is.GreaterThanOrEqualTo(ventana.yMin - 1f),
                "tras añadir se ve el final de la lista");

            // Y con «subir» se llega al principio.
            for (var i = 0; i < 20; i++)
            {
                maze.ScrollUpButton.onClick.Invoke();
            }

            Canvas.ForceUpdateCanvases();
            var primera = EnPantalla(maze.Rows[0]);
            Assert.That(primera.yMax, Is.LessThanOrEqualTo(ventana.yMax + 1f), "el primer bloque queda dentro de la ventana");
            Assert.That(primera.yMin, Is.GreaterThanOrEqualTo(ventana.yMin - 1f));
            Assert.That(maze.ScrollUpButton.interactable, Is.False, "arriba del todo, «subir» ya no tiene nada que hacer");
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido()
        {
            var maze = await OpenMaze();
            maze.TogglePalette();
            maze.AddBlock(InstructionBlock.Forward(2));
            var interactivos = Object.FindObjectsByType<Selectable>(FindObjectsInactive.Include);

            Assert.That(interactivos, Is.Not.Empty);
            Assert.That(interactivos.Where(elemento => !(elemento is Button)).Select(elemento => elemento.name),
                Is.Empty, "todo lo interactivo es un botón: sin deslizadores, barras ni campos");

            var comportamientos = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include);
            var arrastrables = comportamientos
                .Where(comportamiento => comportamiento is IDragHandler || comportamiento is IBeginDragHandler)
                .Select(comportamiento => comportamiento.name)
                .ToArray();
            var sostenidos = comportamientos
                .Where(comportamiento => comportamiento is IPointerDownHandler && !(comportamiento is Selectable)
                                         && comportamiento.GetComponent<Button>() == null
                                         && comportamiento.gameObject.activeInHierarchy)
                .Select(comportamiento => comportamiento.name)
                .OrderBy(nombre => nombre)
                .ToArray();

            Assert.That(arrastrables, Is.Empty, "ningún elemento del laberinto usa el arrastre de uGUI (CT-06)");
            Assert.That(sostenidos, Is.EqualTo(new[] { "Bloque_Backward", "Bloque_Forward", "Bloque_Turn", "Paso_0" }),
                "lo único que responde al clic sostenido son los bloques: los de la paleta y los encajados");
        }

        [Test]
        [Timeout(120000)]
        public async Task MazeScene_RF04_AlcanzarElRefugioConfirmaYGuardaLaFase3()
        {
            const string nombre = "W14_Prueba";
            GameFlowRunner runner = null;

            try
            {
                var (perfil, arrancado) = await ArrancarDesdeBoot(nombre);
                runner = arrancado;
                runner.StartPlaying(LevelId.Wheel, 3);
                var maze = await EsperarLaberinto();
                var fase3 = new PhaseId(LevelId.Wheel, 3);
                Assume.That(perfil.IsPhaseConfirmed(fase3), Is.False);

                foreach (var block in maze.Grid.Solution().Blocks)
                {
                    maze.AddBlock(block);
                }

                maze.Execute();
                await Esperar(() => runner.Flow.Current != GameState.Playing, 90f);

                Assert.That(perfil.IsPhaseConfirmed(fase3), Is.True, "llegar al refugio confirma la fase 3 (RF-04)");
                Assert.That(runner.Session.Load(nombre).IsPhaseConfirmed(fase3), Is.True, "y queda en disco (RNF-14)");
                Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative), "el laberinto sale al cierre del nivel");
                Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo(maze.Layout.ClosingSequenceId));
            }
            finally
            {
                runner?.Session.Delete(nombre);
                LimpiarPersistentes();
            }
        }

        [Test]
        [Timeout(60000)]
        [Category("VisualVerification")]
        [Description("Verificar en las capturas: el claro cenital entero y sin deformar en el panel izquierdo; " +
                     "carretilla en el hueco izquierdo mirando al este, refugio en el hueco derecho, piedras dentro del seto; " +
                     "a la derecha «Tu secuencia» con bloques encajados (contador, lado, rombo), el cajón «Bloques» abierto y «Ejecutar»; " +
                     "con muchos bloques, filas comprimidas con flecha y una desplegada; con más de los que caben, " +
                     "las filas conservan su alto, la lista se recorta en su ventana y aparecen los dos botones de " +
                     "desplazamiento junto al título; el bloque en curso resaltado.")]
        public async Task MazeScene_RNF20_CapturaDelLaberintoEnReposoConBloquesYEnEjecucion()
        {
            var maze = await OpenMaze();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Capturar("Maze_01_reposo");

            maze.TogglePalette();
            maze.AddBlock(InstructionBlock.Forward(2));
            maze.AddBlock(InstructionBlock.Turn(TurnDirection.Right));
            maze.AddBlock(InstructionBlock.Backward(1));
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Capturar("Maze_02_bloques_y_cajon");

            maze.TogglePalette();
            foreach (var block in maze.Grid.Solution().Blocks.Take(9))
            {
                maze.AddBlock(block);
            }

            maze.Expand(4);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Capturar("Maze_03_comprimidos");

            // Más bloques de los que caben ni comprimidos: la lista se desplaza dentro de su ventana.
            for (var i = 0; i < 10; i++)
            {
                maze.AddBlock(i % 2 == 0 ? InstructionBlock.Forward(i % 9 + 1) : InstructionBlock.Turn());
            }

            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Capturar("Maze_05_desplazada");

            maze.Execute();
            await Awaitable.WaitForSecondsAsync(Mathf.Max(maze.Layout.StepSeconds, 0.1f) * 2.2f);
            Capturar("Maze_04_ejecutando");
            await Esperar(() => !maze.IsExecuting, 50f);

            Assert.That(maze.Executions, Is.EqualTo(1));
        }

        // --- helpers ---------------------------------------------------------------------------

        private static void Capturar(string nombre)
        {
            if (Application.isBatchMode)
            {
                return;
            }

            var carpeta = $"{Application.persistentDataPath}/TestScreenshots";
            System.IO.Directory.CreateDirectory(carpeta);
            var ruta = $"{carpeta}/{nombre}.png";
            System.IO.File.Delete(ruta);

            var textura = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.File.WriteAllBytes(ruta, textura.EncodeToPNG());
            Object.Destroy(textura);
            TestContext.WriteLine($"Captura: {ruta}");
        }

        private static async Task<MazeSceneController> OpenMaze()
        {
            var carga = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (!carga.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            var maze = Object.FindAnyObjectByType<MazeSceneController>();
            Assert.That(maze, Is.Not.Null, $"la escena «{SceneName}» trae su controlador");
            Assert.That(maze.Grid, Is.Not.Null, "el laberinto ya se generó");
            return maze;
        }

        private static async Task<MazeSceneController> EsperarLaberinto()
        {
            await Esperar(() => Object.FindAnyObjectByType<MazeSceneController>()?.Grid != null);
            await Awaitable.NextFrameAsync();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            return Object.FindAnyObjectByType<MazeSceneController>();
        }

        private static async Task<(PlayerProfile Profile, GameFlowRunner Runner)> ArrancarDesdeBoot(string nombre)
        {
            SceneManager.LoadScene("Boot");
            await Esperar(() => GameFlowRunner.Instance != null && SceneManager.GetActiveScene().name == "MainMenu");
            var runner = GameFlowRunner.Instance;
            runner.Session.Delete(nombre);

            runner.GoTo(GameState.ProfileSelect);
            var perfil = PlayerProfile.Create(nombre, Array.Empty<string>()).Profile;
            perfil.Reach(LevelId.Wheel);
            perfil.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), default);
            perfil.ConfirmPhase(new PhaseId(LevelId.Wheel, 2), default);
            runner.SelectProfile(perfil);
            return (perfil, runner);
        }

        private static async Task Esperar(Func<bool> condicion, float segundos = 20f)
        {
            var limite = Time.realtimeSinceStartup + segundos;
            while (Time.realtimeSinceStartup < limite)
            {
                if (condicion())
                {
                    return;
                }

                await Awaitable.NextFrameAsync();
            }

            Assert.Fail($"La condición no se cumplió en {segundos} s.");
        }

        private static void LimpiarPersistentes()
        {
            foreach (var persistente in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(persistente.gameObject);
            }

            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }
        }

        /// <summary>Los bloques van encajados con separación cero: tocarse no es solaparse. Medio píxel de tolerancia por el redondeo.</summary>
        private static bool Solapan(Rect a, Rect b)
        {
            var estrecha = new Rect(a.xMin + 0.5f, a.yMin + 0.5f, a.width - 1f, a.height - 1f);
            return estrecha.Overlaps(b);
        }

        private static Rect EnPantalla(RectTransform rect)
        {
            var esquinas = new Vector3[4];
            rect.GetWorldCorners(esquinas);

            var minX = Mathf.Min(esquinas[0].x, esquinas[1].x, esquinas[2].x, esquinas[3].x);
            var minY = Mathf.Min(esquinas[0].y, esquinas[1].y, esquinas[2].y, esquinas[3].y);
            var maxX = Mathf.Max(esquinas[0].x, esquinas[1].x, esquinas[2].x, esquinas[3].x);
            var maxY = Mathf.Max(esquinas[0].y, esquinas[1].y, esquinas[2].y, esquinas[3].y);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
