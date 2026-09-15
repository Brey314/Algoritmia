using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>
    /// RNF-19, RNF-20 y RNF-21 sobre las tres escenas del Nivel 2 (W18). RNF-19 se verifica,
    /// literalmente, «inspeccionando los estados de error de los niveles 2 y 3»: este es el
    /// primer sitio donde se puede cerrar.
    /// </summary>
    [Category("Integration")]
    public class WheelAccessibilityTests
    {
        [Test]
        [Timeout(90000)]
        [Category("Acceptance")]
        [Category("VisualVerification")]
        [Description("Verificar en las capturas: cada rechazo lleva icono además de color en la tablilla; " +
                     "en el laberinto el bloque donde se detuvo el avance tiene contorno y tamaño mayor, no solo otro color.")]
        public async Task WheelLevel_RNF19_LosTresEstadosDeErrorSeLeenSinColor()
        {
            // Objeto no válido (bosque).
            var forest = await OpenScene<ForestSceneController>("Level2_Forest", f => f.Spawned.Count > 0);
            var tronco = forest.Spawned.First(e => e.Object.Category == ForestObjectCategory.RoundLog);
            tronco.Button.onClick.Invoke();
            var aceptado = forest.MessageIcon.sprite;
            var distractor = forest.Spawned.First(e => e.Object.Category != ForestObjectCategory.RoundLog);
            distractor.Button.onClick.Invoke();
            await Awaitable.NextFrameAsync();
            AssertSegundoIndicador("bosque · objeto no válido", forest.MessageLabel, forest.MessageIcon, aceptado);
            Capturar("WheelLevel_RNF19_Bosque_ObjetoNoValido");

            // Paso fuera de secuencia (taller).
            var workshop = await OpenScene<WorkshopSceneController>("Level2_Workshop", w => w.Pieces.Count > 0);
            workshop.Select(WorkshopPiece.ShortLogA);
            workshop.MachineButton.onClick.Invoke();
            var mecanizado = workshop.MessageIcon.sprite;
            Canvas.ForceUpdateCanvases();
            workshop.Take(WorkshopPiece.Plank);
            workshop.DragTo(EnPantalla(workshop.Pieces[WorkshopPiece.ShortLogB].Rect).center);
            workshop.Release(WorkshopPiece.Plank);
            await Awaitable.NextFrameAsync();
            AssertSegundoIndicador("taller · paso fuera de secuencia", workshop.MessageLabel, workshop.MessageIcon, mecanizado);
            Assert.That(workshop.Pieces[WorkshopPiece.Plank].Rect.gameObject.activeSelf, Is.True, "la pieza vuelve a su sitio, no desaparece");
            Capturar("WheelLevel_RNF19_Taller_FueraDeSecuencia");

            // Movimiento inválido (laberinto): retroceder desde la salida entra en el seto.
            var maze = await OpenScene<MazeSceneController>("Level2_Maze", m => m.Rows != null);
            maze.AddBlock(InstructionBlock.Backward(2));
            maze.Execute();
            await Esperar(() => !maze.IsExecuting, 40f);
            await Awaitable.NextFrameAsync();
            var resaltadas = maze.Rows.Where(row => row.GetComponent<Outline>() is { enabled: true } && row.localScale.x > 1f).ToArray();
            Assert.That(resaltadas, Has.Length.EqualTo(1),
                "el bloque donde se detuvo el avance se distingue por contorno y tamaño, no solo por color (RNF-19)");
            Assert.That(maze.MessageLabel.text, Is.Not.Empty, "y la tablilla lo describe en palabras");
            Assert.That(maze.MessageIcon.sprite, Is.Not.Null, "con icono");
            Capturar("WheelLevel_RNF19_Laberinto_MovimientoInvalido");
        }

        [Test]
        [Timeout(60000)]
        [Category("Acceptance")]
        public async Task WheelLevel_RNF20_ContrasteSuficienteEnLasTresEscenas()
        {
            var forest = await OpenScene<ForestSceneController>("Level2_Forest", f => f.Spawned.Count > 0);
            forest.Spawned.First(e => e.Object.Category != ForestObjectCategory.RoundLog).Button.onClick.Invoke();
            await Awaitable.NextFrameAsync();
            AssertContraste("bosque · mensaje", forest.MessageLabel);
            AssertContraste("bosque · contador", forest.CounterLabel);
            AssertContrasteDeLaPausa("bosque");

            var workshop = await OpenScene<WorkshopSceneController>("Level2_Workshop", w => w.Pieces.Count > 0);
            workshop.Select(WorkshopPiece.ShortLogA);
            await Awaitable.NextFrameAsync();
            AssertContraste("taller · mensaje", workshop.MessageLabel);
            AssertContraste("taller · mecanizar", workshop.MachineButton.GetComponentInChildren<Text>(true));
            AssertContrasteDeLaPausa("taller");

            var maze = await OpenScene<MazeSceneController>("Level2_Maze", m => m.Rows != null);
            maze.AddBlock(InstructionBlock.Forward(2));
            await Awaitable.NextFrameAsync();
            AssertContraste("laberinto · mensaje", maze.MessageLabel);
            AssertContraste("laberinto · ejecutar", maze.ExecuteButton.GetComponentInChildren<Text>(true));
            foreach (var texto in maze.Rows.SelectMany(row => row.GetComponentsInChildren<Text>(true)).Where(t => !string.IsNullOrWhiteSpace(t.text)))
            {
                AssertContraste($"laberinto · bloque «{texto.text}»", texto);
            }

            AssertContrasteDeLaPausa("laberinto");
        }

        [Test]
        [Timeout(120000)]
        [Category("Acceptance")]
        [Category("VisualVerification")]
        [Description("Verificar en las capturas: el acercamiento del bosque y el recorrido de la carretilla son movimientos continuos, " +
                     "sin parpadeos ni destellos; ningún elemento se apaga y enciende durante la animación.")]
        public async Task WheelLevel_RNF21_NingunaAnimacionDelNivel2TieneDestellos()
        {
            // El acercamiento al completar el acopio: los troncos vuelan a la fila y la cámara se
            // acerca sin que nada se apague en el camino. (El rodado ya no se anima aquí: lo
            // cuenta la escena 2.2, que vigila NarrativeSceneTests.)
            var forest = await OpenScene<ForestSceneController>("Level2_Forest", f => f.Spawned.Count > 0);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            foreach (var entrada in forest.Spawned.Where(e => e.Object.Category == ForestObjectCategory.RoundLog))
            {
                entrada.Button.onClick.Invoke();
            }

            var caja = forest.CargoRect;
            var cajaImagen = caja.GetComponent<Image>();
            var saltoMaximo = 0f;
            var anterior = forest.World.localScale.x;
            var muestras = 0;
            while (forest.IsTransitioning)
            {
                await Awaitable.NextFrameAsync();
                saltoMaximo = Mathf.Max(saltoMaximo, Mathf.Abs(forest.World.localScale.x - anterior));
                anterior = forest.World.localScale.x;
                muestras++;
                Assert.That(caja.gameObject.activeSelf && cajaImagen.enabled && cajaImagen.color.a > 0.99f, Is.True,
                    "la caja no parpadea durante el acercamiento (RNF-21)");
                Assert.That(forest.Environment.enabled && forest.Environment.color.a > 0.99f, Is.True, "el bosque no destella");
                if (muestras == 10)
                {
                    Capturar("WheelLevel_RNF21_Acercamiento");
                }
            }

            Assert.That(muestras, Is.GreaterThan(5), "el acercamiento dura varios cuadros: es una animación, no un corte");
            Assert.That(saltoMaximo, Is.LessThan(0.15f), $"ningún cuadro salta más de lo que el ojo sigue (máximo {saltoMaximo:F3} de escala)");

            // La ejecución paso a paso (RF-32): la carretilla se desliza casilla a casilla.
            var maze = await OpenScene<MazeSceneController>("Level2_Maze", m => m.Rows != null);
            foreach (var block in maze.Grid.Solution().Blocks)
            {
                maze.AddBlock(block);
            }

            var casilla = (maze.Layout.BoardMax - maze.Layout.BoardMin) / new Vector2(maze.Grid.Columns, maze.Grid.Rows);
            maze.Execute();
            var carretilla = maze.Cart;
            var anteriorCasilla = carretilla.anchorMin;
            var saltoCasillas = 0f;
            muestras = 0;
            while (maze.IsExecuting)
            {
                await Awaitable.NextFrameAsync();
                saltoCasillas = Mathf.Max(saltoCasillas, Vector2.Distance(anteriorCasilla, carretilla.anchorMin));
                anteriorCasilla = carretilla.anchorMin;
                muestras++;
                Assert.That(carretilla.gameObject.activeSelf && maze.Environment.enabled, Is.True, "nada se apaga durante la ejecución (RNF-21)");
                Assert.That(maze.Rows, Has.All.Matches<RectTransform>(row => row.gameObject.activeSelf), "los bloques no parpadean al resaltarse");
                if (muestras == 10)
                {
                    Capturar("WheelLevel_RNF21_Ejecucion");
                }
            }

            Assert.That(muestras, Is.GreaterThan(5));
            Assert.That(saltoCasillas, Is.LessThan(casilla.magnitude * 0.6f),
                "la carretilla nunca salta una casilla entera en un cuadro: se desliza (RNF-21)");
        }

        // --- helpers -----------------------------------------------------------------------

        /// <summary>Un rechazo se lee por icono y por palabras, no solo por el color de la tablilla (RNF-19).</summary>
        private static void AssertSegundoIndicador(string donde, Text label, Image icon, Sprite aceptado)
        {
            Assert.That(label.text, Is.Not.Empty, $"{donde}: la tablilla describe el rechazo en palabras");
            Assert.That(icon.enabled && icon.sprite != null, Is.True, $"{donde}: el rechazo lleva icono");
            Assert.That(icon.sprite, Is.Not.SameAs(aceptado), $"{donde}: el icono del rechazo no es el del acierto");
            Assert.That(label.text.Any(char.IsDigit), Is.False, $"{donde}: sin cifras (CP-03)");
        }

        /// <summary>Contraste del texto contra la cara sobre la que se pinta, medido en la escena (RNF-20).</summary>
        private static void AssertContraste(string donde, Text text)
        {
            Assert.That(text, Is.Not.Null, $"{donde}: no hay texto");
            var cara = text.GetComponentInParent<Image>(true); // la pausa está inactiva hasta abrirse
            Assert.That(cara, Is.Not.Null, $"{donde}: el texto no cuelga de ninguna cara con Image");
            var contraste = ContrastRatio(text.color, cara.color);
            TestContext.WriteLine($"{donde}: {contraste:F2}:1");
            Assert.That(contraste, Is.GreaterThanOrEqualTo(4.5), $"{donde}: {contraste:F2}:1 no alcanza el 4.5:1 de RNF-20");
        }

        private static void AssertContrasteDeLaPausa(string escena)
        {
            var botones = Object.FindObjectsByType<Button>(FindObjectsInactive.Include)
                .Where(b => b.GetComponentInChildren<Text>(true) is { } t
                            && new[] { "Reanudar", "Reiniciar", "Volver al menú de niveles", "Sí, reiniciar", "Cancelar" }.Contains(t.text.Trim()))
                .ToArray();
            Assert.That(botones, Has.Length.EqualTo(5), $"{escena}: el menú de pausa está en la escena con sus cinco botones (W17)");
            foreach (var boton in botones)
            {
                AssertContraste($"{escena} · pausa · {boton.GetComponentInChildren<Text>(true).text}", boton.GetComponentInChildren<Text>(true));
            }
        }

        private static double ContrastRatio(Color a, Color b)
        {
            var lighter = Math.Max(RelativeLuminance(a), RelativeLuminance(b));
            var darker = Math.Min(RelativeLuminance(a), RelativeLuminance(b));
            return (lighter + 0.05) / (darker + 0.05);
        }

        private static double RelativeLuminance(Color color) =>
            0.2126 * Linear(color.r) + 0.7152 * Linear(color.g) + 0.0722 * Linear(color.b);

        private static double Linear(float channel) =>
            channel <= 0.03928 ? channel / 12.92 : Math.Pow((channel + 0.055) / 1.055, 2.4);

        private static async Task<T> OpenScene<T>(string sceneName, Func<T, bool> ready) where T : Object
        {
            var carga = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!carga.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
            T encontrado = null;
            await Esperar(() => (encontrado = Object.FindObjectsByType<T>().FirstOrDefault(ready)) != null);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            return encontrado;
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

        private static Rect EnPantalla(RectTransform rect)
        {
            var esquinas = new Vector3[4];
            rect.GetWorldCorners(esquinas);
            var canvas = rect.GetComponentInParent<Canvas>();
            var camara = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            var puntos = esquinas.Select(e => RectTransformUtility.WorldToScreenPoint(camara, e)).ToArray();
            var min = new Vector2(puntos.Min(p => p.x), puntos.Min(p => p.y));
            var max = new Vector2(puntos.Max(p => p.x), puntos.Max(p => p.y));
            return new Rect(min, max - min);
        }

        /// <summary>Guarda una captura si hay Game View; en batchmode no la hay y las aserciones valen igual.</summary>
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
    }
}
