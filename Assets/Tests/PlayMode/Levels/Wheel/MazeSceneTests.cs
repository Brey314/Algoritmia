using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Audio;
using Game.Core;
using Game.Scaffolding;
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
        public async Task MazeScene_RNF19_ElLadoElegidoDeGirarSeDistingueSinColor()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Turn(TurnDirection.Right));
            // Con un solo bloque la fila 0 ya está desplegada.
            var fila = maze.Rows[0];
            Assert.That(fila.Find("Fondo/Lado/Der").GetComponent<Outline>().enabled, Is.True,
                "el lado elegido lleva contorno además del color de atención (RNF-19)");
            Assert.That(fila.Find("Fondo/Lado/Izq").GetComponent<Outline>().enabled, Is.False, "el otro, sin contorno");

            maze.SetDirection(0, TurnDirection.Left);
            fila = maze.Rows[0]; // RefreshRows recrea las filas
            Assert.That(fila.Find("Fondo/Lado/Izq").GetComponent<Outline>().enabled, Is.True, "y ahora el elegido es «Izq»");
            Assert.That(fila.Find("Fondo/Lado/Der").GetComponent<Outline>().enabled, Is.False);
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

        /// <summary>
        /// El laberinto no lleva tinte de luz: el entorno, la carretilla, el refugio y los
        /// obstáculos quedan tal cual viene el arte, y el entorno lleva el contorno del panel
        /// de diálogo (Dirección de arte §10.2).
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF30_ElEntornoQuedaSinTinteYConSuContorno()
        {
            var maze = await OpenMaze();

            Assert.That(maze.Environment.color, Is.EqualTo(Color.white), "el entorno sin tinte");
            Assert.That(maze.Cart.GetComponent<Image>().color, Is.EqualTo(Color.white), "y la carretilla tampoco");
            Assert.That(maze.Pieces.Select(pieza => pieza.GetComponent<Image>())
                    .Where(imagen => imagen != null && imagen.sprite != null)
                    .Select(imagen => imagen.color),
                Has.All.EqualTo(Color.white), "y el refugio y los obstáculos dibujados");

            var outline = maze.Environment.GetComponent<Outline>();
            Assert.That(outline, Is.Not.Null, "el entorno lleva su contorno");
            Assert.That(outline.enabled, Is.True);
            var esperado = new Color(0.769f, 0.659f, 0.510f);
            Assert.That(outline.effectColor.r, Is.EqualTo(esperado.r).Within(0.002f), "contorno #C4A882");
            Assert.That(outline.effectColor.g, Is.EqualTo(esperado.g).Within(0.002f));
            Assert.That(outline.effectColor.b, Is.EqualTo(esperado.b).Within(0.002f));
        }

        /// <summary>
        /// La cuadrícula de la matriz se dibuja dentro del seto: una línea por frontera entre las
        /// casillas del interior, ninguna sobre el seto, y detrás de piedras y carretilla.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF31_LaCuadriculaDeLaMatrizSeDibujaDentroDelSeto()
        {
            var maze = await OpenMaze();
            var cuadricula = maze.Environment.rectTransform.Find("Cuadricula");

            Assert.That(cuadricula, Is.Not.Null, "el entorno trae la cuadrícula");
            Assert.That(cuadricula.GetSiblingIndex(), Is.Zero, "detrás de todo lo que se pinta encima");
            Assert.That(cuadricula.childCount, Is.EqualTo(maze.Grid.Columns - 1 + maze.Grid.Rows - 1),
                "una línea por frontera interior, en columnas y en filas");

            var tablero = (RectTransform)cuadricula;
            Assert.That(tablero.anchorMin, Is.EqualTo(maze.Layout.BoardMin), "la cuadrícula ocupa el tablero de la matriz");
            Assert.That(tablero.anchorMax, Is.EqualTo(maze.Layout.BoardMax));

            // En fracciones del tablero: el anillo exterior, una casilla, es el seto.
            var dentroMin = new Vector2(1f / maze.Grid.Columns, 1f / maze.Grid.Rows);
            var dentroMax = Vector2.one - dentroMin;
            foreach (RectTransform linea in cuadricula)
            {
                foreach (var ancla in new[] { linea.anchorMin, linea.anchorMax })
                {
                    Assert.That(ancla.x, Is.InRange(dentroMin.x - 1e-4f, dentroMax.x + 1e-4f), $"{linea.name} no pisa el seto");
                    Assert.That(ancla.y, Is.InRange(dentroMin.y - 1e-4f, dentroMax.y + 1e-4f), $"{linea.name} no pisa el seto");
                }

                Assert.That(linea.GetComponent<Image>().raycastTarget, Is.False, $"{linea.name} no recibe clics");
            }
        }

        /// <summary>
        /// La salida no se marca con un cuadro de color —se lee en el entorno, en el hueco del
        /// seto—; el panel que rodea al entorno es el marfil de interfaz; y el entorno lleva su
        /// contraste y saturación en una copia del material, no en el asset compartido.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF30_LaSalidaSeLeeEnElEntornoYElPanelEsMarfil()
        {
            var maze = await OpenMaze();
            var layout = maze.Layout;
            var refugio = maze.Pieces.First(pieza => pieza.name == "Refugio").GetComponent<Image>();

            Assume.That(layout.ShelterArt, Is.Null, "hoy el refugio no tiene ilustración propia");
            Assert.That(refugio.enabled, Is.False, "sin ilustración, la casilla de llegada no se pinta de color");

            var panel = maze.Environment.rectTransform.parent.GetComponent<Image>();
            Assert.That(panel.color.r, Is.EqualTo(layout.BackdropColor.r).Within(0.002f), "el panel es el color de interfaz del asset");
            Assert.That(panel.color.g, Is.EqualTo(layout.BackdropColor.g).Within(0.002f));
            Assert.That(panel.color.b, Is.EqualTo(layout.BackdropColor.b).Within(0.002f));
            Assert.That(panel.color.r, Is.EqualTo(0.969f).Within(0.01f), "marfil #F7EFE2");

            Assert.That(maze.Environment.material, Is.Not.SameAs(layout.EnvironmentMaterial), "una copia por escena");
            Assert.That(maze.Environment.material.GetFloat("_Contrast"), Is.EqualTo(layout.Contrast), "con el contraste del asset");
            Assert.That(maze.Environment.material.GetFloat("_Saturation"), Is.EqualTo(layout.Saturation), "y su saturación");
            Assert.That(layout.Contrast, Is.EqualTo(1f), "sin contraste añadido: el entorno queda plano");
            Assert.That(layout.Saturation, Is.EqualTo(1f), "y sin tocar el color del arte");
        }

        /// <summary>
        /// Soltar un bloque deja la lista abajo del todo, con «Suelta un bloque aquí» a la vista; y
        /// abrir el cajón también, aunque se hubiera subido a mirar el principio.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RNF03_AlSoltarUnBloqueYAlAbrirElCajonLaSecuenciaQuedaAbajoDelTodo()
        {
            var maze = await OpenMaze();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            for (var i = 0; i < 12; i++)
            {
                maze.TakeFromPalette(InstructionBlock.Default(i % 2 == 0 ? BlockKind.Forward : BlockKind.Turn));
                maze.Drop(EnPantalla(maze.SequenceViewport).center);
            }

            Canvas.ForceUpdateCanvases();
            Assume.That(maze.ScrollDownButton.gameObject.activeSelf, Is.True, "doce bloques ya no caben");
            Assert.That(maze.ScrollDownButton.interactable, Is.False, "tras soltar, la lista está abajo del todo");
            Assert.That(EnPantalla(maze.DropZone).yMin, Is.GreaterThanOrEqualTo(EnPantalla(maze.SequenceViewport).yMin - 1f),
                "con la casilla de soltar a la vista");

            for (var i = 0; i < 20 && maze.ScrollUpButton.interactable; i++)
            {
                maze.ScrollUpButton.onClick.Invoke();
            }

            Assume.That(maze.ScrollDownButton.interactable, Is.True, "se subió a mirar el principio");
            maze.TogglePalette();
            Canvas.ForceUpdateCanvases();

            Assert.That(maze.IsPaletteOpen, Is.True);
            Assert.That(maze.ScrollDownButton.interactable, Is.False, "al abrir el cajón, la secuencia vuelve abajo del todo");
        }

        /// <summary>
        /// La barra de la derecha acompaña a «▲» y «▼» y se puede pulsar: un clic abajo lleva al
        /// final y uno arriba al principio. Se pulsa, no se arrastra: no trae el arrastre de uGUI
        /// (RNF-02, CT-06).
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RNF02_LaBarraDeDesplazamientoSePulsaPeroNoSeArrastra()
        {
            var maze = await OpenMaze();
            Assert.That(maze.ScrollBar.gameObject.activeSelf, Is.False, "sin desborde no hay barra");

            for (var i = 0; i < 16; i++)
            {
                maze.AddBlock(InstructionBlock.Forward(1));
            }

            Canvas.ForceUpdateCanvases();
            Assert.That(maze.ScrollBar.gameObject.activeSelf, Is.True, "con desborde aparece la barra");
            Assert.That(maze.ScrollThumb.anchorMin.y, Is.EqualTo(0f).Within(0.01f), "abajo del todo, el tirador toca el fondo");
            var alto = maze.ScrollThumb.anchorMax.y - maze.ScrollThumb.anchorMin.y;
            Assert.That(alto, Is.GreaterThan(0f).And.LessThan(1f), "y mide la parte visible de la lista");

            var antes = maze.ScrollThumb.anchorMax.y;
            maze.ScrollUpButton.onClick.Invoke();
            Assert.That(maze.ScrollThumb.anchorMax.y, Is.GreaterThan(antes), "«▲» sube el tirador");

            var barra = EnPantalla(maze.ScrollBar);
            Pulsar(maze.ScrollBar, new Vector2(barra.center.x, barra.yMax - 1f));
            Assert.That(maze.ScrollUpButton.interactable, Is.False, "un clic arriba de la barra lleva al principio");
            Assert.That(maze.ScrollThumb.anchorMax.y, Is.EqualTo(1f).Within(0.01f));

            Pulsar(maze.ScrollBar, new Vector2(barra.center.x, barra.yMin + 1f));
            Assert.That(maze.ScrollDownButton.interactable, Is.False, "y uno abajo, al final");
            Assert.That(maze.ScrollThumb.anchorMin.y, Is.EqualTo(0f).Within(0.01f));

            Assert.That(maze.ScrollBar.GetComponent<Image>().raycastTarget, Is.True, "la barra recibe el clic");
            Assert.That(maze.ScrollBar.GetComponentsInChildren<ScrollRect>(true), Is.Empty, "no es un ScrollRect");
            Assert.That(maze.ScrollBar.GetComponentsInChildren<Scrollbar>(true), Is.Empty, "ni un Scrollbar de uGUI");
            Assert.That(maze.ScrollBar.GetComponentsInChildren<IDragHandler>(true), Is.Empty, "ni nada que se arrastre");
            Assert.That(maze.ScrollThumb.GetComponent<Image>().raycastTarget, Is.False, "el tirador no se agarra");
        }

        /// <summary>
        /// Solo el bloque seleccionado lleva la papelera —el mismo icono que borra un perfil—, a
        /// la derecha de su fila; pulsarla lo retira de la secuencia y la papelera pasa a la fila que
        /// ocupa su lugar (RF-34, DEF-GP1-03).
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF34_ElBloqueSeleccionadoMuestraLaPapeleraYEstaLoRetira()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Forward(1));
            maze.AddBlock(InstructionBlock.Turn());
            maze.AddBlock(InstructionBlock.Backward(2));
            Assert.That(Papeleras(maze), Is.Empty, "sin bloque seleccionado no hay papelera");

            maze.Expand(1);
            Canvas.ForceUpdateCanvases();
            var papeleras = Papeleras(maze);
            Assert.That(papeleras, Has.Length.EqualTo(1), "el bloque seleccionado, y solo él, muestra su papelera");
            var papelera = papeleras[0];
            Assert.That(papelera.transform.parent, Is.SameAs(maze.Rows[1]), "en la fila seleccionada");
            Assert.That(papelera.image.sprite, Is.SameAs(maze.Layout.DeleteIcon), "con el icono del asset");
            Assert.That(maze.Layout.DeleteIcon.name, Is.EqualTo("ui_papelera"), "el mismo que borra un perfil");
            Assert.That(EnPantalla((RectTransform)papelera.transform).xMin, Is.GreaterThan(EnPantalla(maze.Rows[1]).xMax),
                "a la derecha de la fila, fuera de ella");
            Assert.That(Solapan(EnPantalla((RectTransform)papelera.transform), EnPantalla(maze.ScrollBar)), Is.False,
                "sin pisar la barra de desplazamiento");

            papelera.onClick.Invoke();

            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[] { InstructionBlock.Forward(1), InstructionBlock.Backward(2) }),
                "la papelera retira ese bloque y deja los demás en su orden");
            // DEF-GP1-03: la selección pasa a la fila que ocupa su lugar, así que la papelera se
            // desplaza con ella y es la única a la vista.
            var tras = Papeleras(maze);
            Assert.That(tras, Has.Length.EqualTo(1), "queda una sola papelera, la del bloque que ocupa el lugar");
            Assert.That(tras[0].transform.parent, Is.SameAs(maze.Rows[1]), "en la fila que ocupa su lugar (Retroceder x2)");
        }

        private static Button[] Papeleras(MazeSceneController maze) =>
            maze.Rows.Select(fila => fila.Find("Eliminar"))
                .Where(papelera => papelera != null && papelera.gameObject.activeInHierarchy)
                .Select(papelera => papelera.GetComponent<Button>())
                .ToArray();

        /// <summary>Un clic simple sobre el gráfico, en ese punto de la pantalla, por el sistema de eventos.</summary>
        private static void Pulsar(RectTransform objetivo, Vector2 punto)
        {
            var pointer = new PointerEventData(EventSystem.current) { position = punto };
            ExecuteEvents.Execute(objetivo.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }

        /// <summary>
        /// Un clic sostenido sobre un bloque y su soltar, por el camino del <c>EventSystem</c>: el pulsar
        /// y el soltar entran por el <c>CargoHandle</c> del mismo objeto, con fotogramas de por medio para
        /// que corra el <c>Destroy</c> diferido de lo que el pulsar repinta.
        /// </summary>
        private static async Task Gesto(GameObject bloque, Vector2 hasta)
        {
            var puntero = new PointerEventData(EventSystem.current)
            {
                position = EnPantalla((RectTransform)bloque.transform).center,
                button = PointerEventData.InputButton.Left
            };

            ExecuteEvents.Execute(bloque, puntero, ExecuteEvents.pointerDownHandler);
            await Asentar();
            puntero.position = hasta;
            ExecuteEvents.Execute(bloque, puntero, ExecuteEvents.pointerUpHandler);
            await Asentar();
        }

        /// <summary>Deja que se asiente la interfaz: layout reconstruido y el <c>Destroy</c> diferido ya ejecutado.</summary>
        private static async Task Asentar()
        {
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            Canvas.ForceUpdateCanvases();
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

        /// <summary>
        /// DEF-GP1-01 (OE4, GP#1). Un bloque tomado de «Tu secuencia» se queda pegado al cursor:
        /// repintar la secuencia destruye la fila que recibió el pulsar, y el <c>EventSystem</c> entrega
        /// el soltar al objeto que recibió el pulsar, no a otro. Las pruebas de arriba llaman a
        /// <c>TakeFromSequence</c> y a <c>Drop</c> directamente y nunca pasaron por ese camino.
        /// Aquí el pulsar y el soltar entran por el <c>CargoHandle</c> de la fila, con un fotograma de
        /// por medio para que corra el <c>Destroy</c> diferido.
        /// </summary>
        /// <remarks>
        /// Rojo esperado sin la corrección: la fila pulsada ya no existe tras el primer fotograma, el
        /// soltar no se entrega y <c>Held</c> sigue ocupado.
        /// </remarks>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF34_ReordenarUnBloqueEsUnSoloGestoYNoQuedaPegado()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Forward(1));
            maze.AddBlock(InstructionBlock.Turn(TurnDirection.Left));
            maze.AddBlock(InstructionBlock.Forward(2));
            await Asentar();

            var tercera = maze.Rows[2].gameObject; // lo que el EventSystem guarda como «el que recibió el pulsar»
            var puntero = new PointerEventData(EventSystem.current)
            {
                position = EnPantalla(maze.Rows[2]).center,
                button = PointerEventData.InputButton.Left
            };

            ExecuteEvents.Execute(tercera, puntero, ExecuteEvents.pointerDownHandler); // clic sostenido sobre el tercero
            await Asentar();

            Assert.That(maze.Held, Is.Not.Null, "mientras se sostiene, el bloque sigue al cursor");
            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[] { InstructionBlock.Forward(1), InstructionBlock.Turn(TurnDirection.Left) }),
                "y salió de la secuencia");
            Assert.That(tercera != null && tercera.activeInHierarchy, Is.True,
                "la fila que recibió el pulsar sigue viva: es a ella a quien el EventSystem entrega el soltar");

            // El soltar llega por donde llegaría de verdad: pointerUp al objeto que recibió el pulsar.
            puntero.position = new Vector2(EnPantalla(maze.SequenceViewport).center.x, EnPantalla(maze.SequenceViewport).yMax - 1f);
            ExecuteEvents.Execute(tercera, puntero, ExecuteEvents.pointerUpHandler);
            await Asentar();

            Assert.That(maze.Held, Is.Null, "un solo gesto: el soltar llegó y el bloque no se quedó pegado");
            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[]
                {
                    InstructionBlock.Forward(2), InstructionBlock.Forward(1), InstructionBlock.Turn(TurnDirection.Left)
                }),
                "soltado encima del primero, queda el primero: se reordenó");
            Assert.That(tercera == null, Is.True, "la fila apartada se destruyó al soltar");
            Assert.That(maze.Rows.Count, Is.EqualTo(3), "y la lista vuelve a tener una fila por bloque");

            // El siguiente arrastre toma el bloque que se pide, no el que quedó colgando.
            maze.TogglePalette();
            await Asentar();
            var retroceder = maze.PaletteBlocks.Single(bloque => bloque.name == "Bloque_Backward").gameObject;
            ExecuteEvents.Execute(retroceder, puntero, ExecuteEvents.pointerDownHandler);
            Assert.That(maze.Held, Is.Not.Null);
            Assert.That(maze.Held.GetComponentInChildren<Text>().text, Is.EqualTo(maze.Layout.BackwardLabel), "el sostenido es «Retroceder»");
            puntero.position = new Vector2(EnPantalla(maze.SequenceViewport).center.x, EnPantalla(maze.SequenceViewport).yMin + 1f);
            ExecuteEvents.Execute(retroceder, puntero, ExecuteEvents.pointerUpHandler);
            await Asentar();

            Assert.That(maze.Held, Is.Null);
            Assert.That(maze.Sequence.Blocks.Last(), Is.EqualTo(InstructionBlock.Backward(1)), "entró «Retroceder», al final");
            Assert.That(maze.Sequence.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// DEF-GP1-01, retirar: soltar fuera del panel un bloque tomado de la secuencia lo retira con
        /// un solo gesto y no deja nada pegado. Y el indicador «errores corregidos» cuenta lo que se
        /// hizo —retirar +1, reordenar +2 (tomarlo y soltarlo), añadir desde el cajón +1; así lo fija
        /// <c>casos.md</c> S-N2C— y ya no las ediciones que el defecto provocaba: tomar el bloque
        /// atascado, el enganche involuntario del siguiente arrastre y el segundo clic del reordenado.
        /// </summary>
        [Test]
        [Timeout(90000)]
        public async Task MazeScene_RF34_RetirarYReordenarArrastrandoCuentanSoloLasEdicionesHechas()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Turn(TurnDirection.Left));
            maze.AddBlock(InstructionBlock.Forward(1));
            maze.AddBlock(InstructionBlock.Forward(2));
            await Asentar();

            maze.Execute(); // girar a la izquierda y avanzar topa con el seto: falla, y desde aquí cuentan las ediciones
            await Esperar(() => !maze.IsExecuting, 50f);
            await Asentar();
            Assert.That(maze.Indicators.Complete().Attempts, Is.EqualTo(1));

            var arriba = new Vector2(EnPantalla(maze.SequenceViewport).center.x, EnPantalla(maze.SequenceViewport).yMax - 1f);
            await Gesto(maze.Rows[2].gameObject, arriba); // reordenar: el tercero, encima del primero (+2)
            Assert.That(maze.Held, Is.Null, "reordenar fue un solo gesto");
            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[]
            {
                InstructionBlock.Forward(2), InstructionBlock.Turn(TurnDirection.Left), InstructionBlock.Forward(1)
            }));

            await Gesto(maze.Rows[0].gameObject, new Vector2(10f, 10f)); // retirar: soltarlo fuera (+1)
            Assert.That(maze.Held, Is.Null, "retirar fue un solo gesto");
            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[] { InstructionBlock.Turn(TurnDirection.Left), InstructionBlock.Forward(1) }));

            maze.TogglePalette(); // abrir el cajón no es una edición
            await Asentar();
            var retroceder = maze.PaletteBlocks.Single(bloque => bloque.name == "Bloque_Backward").gameObject;
            await Gesto(retroceder, new Vector2(EnPantalla(maze.SequenceViewport).center.x, EnPantalla(maze.SequenceViewport).yMin + 1f)); // añadir (+1)
            Assert.That(maze.Sequence.Blocks.Last(), Is.EqualTo(InstructionBlock.Backward(1)),
                "el arrastre siguiente toma el bloque pedido y no uno pegado");

            maze.Execute();
            await Esperar(() => !maze.IsExecuting, 50f);

            Assert.That(maze.Indicators.Complete().CorrectedErrors, Is.EqualTo(4),
                "2 del reordenado + 1 del retiro + 1 del añadido: ni una más por el bloque pegado");
        }

        /// <summary>
        /// Con el soltar ya entregado (DEF-GP1-01), un clic sin mover sobre un bloque lo toma y lo suelta
        /// en su propio hueco: se queda donde estaba y no cuenta como edición. Soltarlo en la mitad de
        /// abajo de su fila, con el siguiente bloque ya subido a ese sitio, lo habría pasado detrás de
        /// él: un clic habría cambiado el algoritmo sin avisar.
        /// </summary>
        [Test]
        [Timeout(90000)]
        public async Task MazeScene_RF34_UnClicSinMoverSobreUnBloqueLoDejaDondeEstabaYNoCuentaEdicion()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Turn(TurnDirection.Left));
            maze.AddBlock(InstructionBlock.Forward(1));
            maze.AddBlock(InstructionBlock.Forward(2));
            await Asentar();
            Assume.That(maze.IsCompact, Is.False, "con tres bloques las filas van desplegadas");

            maze.Execute(); // falla: desde aquí cuentan las ediciones
            await Esperar(() => !maze.IsExecuting, 50f);
            await Asentar();

            var segunda = EnPantalla(maze.Rows[1]);
            await Gesto(maze.Rows[1].gameObject, new Vector2(segunda.center.x, segunda.yMin + segunda.height * 0.25f)); // mitad de abajo, sin salir de su fila

            Assert.That(maze.Held, Is.Null, "el clic tomó y soltó: no quedó nada pegado");
            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[]
            {
                InstructionBlock.Turn(TurnDirection.Left), InstructionBlock.Forward(1), InstructionBlock.Forward(2)
            }), "y el bloque sigue donde estaba");
            Assert.That(Papeleras(maze), Has.Length.EqualTo(1), "queda seleccionado: su fila ofrece la papelera");
            Assert.That(maze.Rows[1].Find("Eliminar"), Is.Not.Null, "la de ese bloque");

            maze.Execute();
            await Esperar(() => !maze.IsExecuting, 50f);

            Assert.That(maze.Indicators.Complete().CorrectedErrors, Is.Zero, "un clic sin mover no cambió la secuencia: no es una edición");
        }

        /// <summary>
        /// DEF-GP1-02 (OE4, GP#2). Con la lista desplazada, el resaltado del bloque en curso no se ve:
        /// la lista no sigue a la ejecución y los primeros bloques se recorren fuera de la ventana.
        /// </summary>
        /// <remarks>
        /// Rojo esperado sin la corrección: tras añadir los bloques la lista queda abajo del todo y
        /// el bloque 0, el primero en ejecutarse, queda por encima de la ventana.
        /// </remarks>
        [Test]
        [Timeout(90000)]
        public async Task MazeScene_RF32_AlEjecutarLaListaMuestraElBloqueEnCurso()
        {
            const int bloques = 12;
            var maze = await OpenMaze();
            for (var i = 0; i < bloques; i++)
            {
                maze.AddBlock(InstructionBlock.Turn()); // girar nunca choca: se recorren todos
            }

            await Asentar();
            Assume.That(maze.ScrollDownButton.gameObject.activeSelf, Is.True, "doce bloques no caben en la ventana");
            Assume.That(maze.ScrollDownButton.interactable, Is.False, "y la lista quedó abajo del todo, con los primeros bloques fuera de la vista");

            var ventana = EnPantalla(maze.SequenceViewport);
            var resaltados = new SortedSet<int>();
            var fuera = new List<string>();

            maze.Execute();
            await Esperar(() =>
            {
                for (var i = 0; i < maze.Rows.Count; i++)
                {
                    if (!maze.Rows[i].GetComponent<Outline>().enabled)
                    {
                        continue;
                    }

                    resaltados.Add(i);
                    var fila = EnPantalla(maze.Rows[i]);
                    if (fila.yMin < ventana.yMin - 1f || fila.yMax > ventana.yMax + 1f)
                    {
                        fuera.Add($"Paso_{i} ({fila.yMin:F0}..{fila.yMax:F0} contra la ventana {ventana.yMin:F0}..{ventana.yMax:F0})");
                    }
                }

                return !maze.IsExecuting;
            }, 60f);

            Assert.That(resaltados, Is.EquivalentTo(Enumerable.Range(0, bloques)), "se resaltó cada bloque, empezando por el primero");
            Assert.That(fuera, Is.Empty, "y en todo momento el bloque resaltado estaba dentro de la ventana (RF-32)");
        }

        /// <summary>
        /// DEF-GP1-03 (OE4, GP#1). Tras usar la papelera con el cajón cerrado ninguna fila la ofrece
        /// y no hay flecha «→» para elegir otra: la secuencia solo se vacía abriendo el cajón.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF34_TrasLaPapeleraConElCajonCerradoLasFilasLaOfrecen()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Forward(1));
            maze.AddBlock(InstructionBlock.Turn());
            maze.AddBlock(InstructionBlock.Backward(2));
            maze.Expand(0); // el primero queda seleccionado: su fila lleva la papelera
            await Asentar();
            Assume.That(maze.IsPaletteOpen, Is.False, "el cajón está cerrado");
            Assume.That(maze.IsCompact, Is.False, "con tres bloques las filas van desplegadas y no hay flecha «→»");

            for (var quedan = 3; quedan > 0; quedan--)
            {
                var papeleras = Papeleras(maze);
                Assert.That(papeleras, Has.Length.EqualTo(1), $"con {quedan} bloques y el cajón cerrado, una fila ofrece su papelera");

                papeleras[0].onClick.Invoke();
                await Asentar();
                Assert.That(maze.Sequence.Count, Is.EqualTo(quedan - 1), "cada papelera retira un bloque");
            }

            Assert.That(maze.Sequence.IsEmpty, Is.True, "se vació sin abrir el cajón");
            Assert.That(Papeleras(maze), Is.Empty, "y sin bloques no hay papelera que ofrecer");
        }

        /// <summary>DEF-GP1-03, la otra salida: retirar un bloque arrastrándolo fuera tampoco deja las filas sin papelera.</summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_RF34_TrasRetirarArrastrandoLasFilasOfrecenLaPapelera()
        {
            var maze = await OpenMaze();
            maze.AddBlock(InstructionBlock.Forward(1));
            maze.AddBlock(InstructionBlock.Turn());
            maze.AddBlock(InstructionBlock.Backward(2));
            await Asentar();
            Assume.That(maze.IsCompact, Is.False, "con tres bloques las filas van desplegadas y no hay flecha «→»");

            await Gesto(maze.Rows[1].gameObject, new Vector2(10f, 10f));

            Assert.That(maze.Sequence.Blocks, Is.EqualTo(new[] { InstructionBlock.Forward(1), InstructionBlock.Backward(2) }));
            Assert.That(Papeleras(maze), Has.Length.EqualTo(1), "una fila ofrece la papelera, con el cajón cerrado");
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

        /// <summary>
        /// La Niña programa de pie junto a la salida, mirando el tablero, y la familia la espera en
        /// el refugio (§1.6.3.1, 2.5): cuelgan del entorno como las piezas —así las vigilan las
        /// pruebas de encuadre—, no reciben clics y no se tiñen con la luz del decorado (§5.4).
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_DA133_LaNinaMiraElTableroJuntoALaSalidaYLaFamiliaEsperaEnElRefugio()
        {
            var maze = await OpenMaze();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(maze.Player, Is.Not.Null, "la Niña está en el laberinto");
            Assert.That(maze.Family.Count(miembro => miembro != null), Is.EqualTo(3), "Papá, Mamá y el Niño esperan en el refugio");
            Assert.That(maze.Player.Current, Is.EqualTo(ActorAction.Observe), "la Niña mira el tablero mientras escribe (§13.3)");
            Assert.That(maze.Family.Select(miembro => miembro.Current), Has.All.EqualTo(ActorAction.Idle), "la familia espera en reposo");

            var reparto = maze.Family.Append(maze.Player).ToArray();
            foreach (var personaje in reparto)
            {
                Assert.That(maze.Pieces, Does.Contain((RectTransform)personaje.transform), $"{personaje.name} cuelga del entorno");
                foreach (var grafico in personaje.GetComponentsInChildren<Graphic>(true))
                {
                    Assert.That(grafico.raycastTarget, Is.False, $"{personaje.name}/{grafico.name} no roba clics");
                    if (grafico.name == "Sombra")
                    {
                        // La sombra de contacto (INC-109, Direccion de arte §5.3) lleva su propio
                        // tono fijo #000000 al 25%: no participa del tinte de la luz del decorado.
                        continue;
                    }

                    Assert.That(grafico.color, Is.EqualTo(Color.white), $"{personaje.name}/{grafico.name} no se tiñe con el atardecer (§4.2, §5.4)");
                }
            }

            var entorno = EnPantalla(maze.Environment.rectTransform);
            Vector2 Casilla(Vector2Int celda) => entorno.min + Vector2.Scale(maze.CellAnchor(celda), entorno.size);
            var salida = Casilla(maze.Grid.Start.Cell);
            var refugio = Casilla(maze.Grid.Goal);
            var nina = EnPantalla((RectTransform)maze.Player.transform).center;
            Assert.That(Vector2.Distance(nina, salida), Is.LessThan(Vector2.Distance(nina, refugio)), "la Niña, junto a la salida");
            foreach (var miembro in maze.Family)
            {
                var centro = EnPantalla((RectTransform)miembro.transform).center;
                Assert.That(Vector2.Distance(centro, refugio), Is.LessThan(Vector2.Distance(centro, salida)), $"{miembro.name}, junto al refugio");
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task MazeScene_DA133_LaNinaSenalaAlEjecutarYVuelveAMirarElTablero()
        {
            var maze = await OpenMaze();
            // Tres giros: nunca chocan y la ejecución dura más que el gesto.
            for (var i = 0; i < 3; i++)
            {
                maze.AddBlock(InstructionBlock.Turn());
            }

            maze.ExecuteButton.onClick.Invoke();

            Assert.That(maze.Player.Current, Is.EqualTo(ActorAction.Point), "al pulsar «Ejecutar» la Niña señala (§13.3)");
            await Esperar(() => maze.Player.Current != ActorAction.Point);
            Assert.That(maze.IsExecuting, Is.True, "el gesto acaba antes que la ejecución");
            Assert.That(maze.Player.Current, Is.EqualTo(ActorAction.Observe), "y vuelve sola a mirar la carretilla");
        }

        /// <summary>
        /// Que la carretilla no llegue es depurar, no perder: la Niña se anima y vuelve a mirar el
        /// tablero, sin ningún otro gesto, y la familia sigue esperando (CP-02, RF-33, §7.3).
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_DA133_SiLaCarretillaNoLlegaLaNinaSeAnimaYNuncaHaceOtroGesto()
        {
            var maze = await OpenMaze();
            // Girar a la izquierda y avanzar topa con el seto: se intenta y se vuelve.
            maze.AddBlock(InstructionBlock.Turn(TurnDirection.Left));
            maze.AddBlock(InstructionBlock.Forward(1));

            var vistos = new HashSet<ActorAction>();
            var trasElFallo = new List<ActorAction>();
            maze.Execute();
            await Esperar(() =>
            {
                vistos.Add(maze.Player.Current);
                if (maze.IsExecuting && maze.MessageLabel.text == maze.Layout.StoppedMessage)
                {
                    trasElFallo.Add(maze.Player.Current);
                }

                return !maze.IsExecuting && maze.Player.Current == ActorAction.Observe;
            });

            Assert.That(trasElFallo, Is.Not.Empty.And.All.EqualTo(ActorAction.Encourage),
                "desde que el guía dice que no llegó, la Niña se anima, y nada más (CP-02)");
            Assert.That(vistos, Is.SubsetOf(new[] { ActorAction.Point, ActorAction.Observe, ActorAction.Encourage }),
                "en todo el intento: señalar, mirar y animarse; ni celebrar ni un gesto de derrota");
            Assert.That(maze.Family.Select(miembro => miembro.Current), Has.All.EqualTo(ActorAction.Idle), "la familia sigue esperando");
        }

        [Test]
        [Timeout(120000)]
        public async Task MazeScene_DA133_AlLlegarAlRefugioLaNinaYLaFamiliaCelebran()
        {
            var maze = await OpenMaze();
            foreach (var block in maze.Grid.Solution().Blocks)
            {
                maze.AddBlock(block);
            }

            maze.Execute();
            await Esperar(() => maze.MessageLabel.text == maze.Layout.ReachedMessage, 90f);

            Assert.That(maze.Player.Current, Is.EqualTo(ActorAction.Celebrate), "la Niña celebra que la carretilla llegó (§13.3)");
            Assert.That(maze.Family.Select(miembro => miembro.Current), Has.All.EqualTo(ActorAction.Celebrate), "y la familia con ella, en el refugio");
        }

        /// <summary>
        /// El botón de pista lleva dentro de su círculo a Algoritm con la forma del nivel —la rueda
        /// en el Nivel 2— en lugar del «?»: quien ayuda es el guía (§7.6, §10.2). El dibujo no
        /// recibe el clic; lo recibe el botón, que sigue repitiendo la instrucción.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task MazeScene_DA76_ElBotonDePistaMuestraAAlgoritmEnFormaDeRueda()
        {
            var maze = await OpenMaze();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var fondo = maze.HelpButton.transform.Find("Fondo");
            Assert.That(fondo, Is.Not.Null, "el círculo conserva su fondo ámbar");
            var hijo = fondo.Find("Algoritm");
            Assert.That(hijo, Is.Not.Null, "dentro del círculo va Algoritm");
            var algoritm = hijo.GetComponent<Image>();

            Assert.That(algoritm.sprite, Is.Not.Null);
            Assert.That(algoritm.sprite.name, Is.EqualTo("char_algoritm_n2_rueda_reposo"), "con la forma del Nivel 2: la rueda (§7.6)");
            Assert.That(algoritm.preserveAspect, Is.True, "sin deformarse");
            Assert.That(algoritm.raycastTarget, Is.False, "el clic lo recibe el botón, no el dibujo");
            Assert.That(maze.HelpButton.GetComponentsInChildren<Text>(true), Is.Empty, "y ya no lleva el «?»");

            var circulo = EnPantalla((RectTransform)fondo);
            var dibujo = EnPantalla(algoritm.rectTransform);
            Assert.That(circulo.Contains(dibujo.min) && circulo.Contains(dibujo.max - Vector2.one * 0.01f), Is.True,
                "Algoritm cabe dentro del círculo");
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
