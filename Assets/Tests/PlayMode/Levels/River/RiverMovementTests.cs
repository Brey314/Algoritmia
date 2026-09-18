using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// La escena de la orilla y el desplazamiento con botones en pantalla (RF-35, RNF-02, INC-01,
    /// RF-13, RNF-03). Es donde INC-01 se cumple o se rompe.
    /// </summary>
    [Category("Integration")]
    public class RiverMovementTests
    {
        private const string SceneName = "Level3_River";

        [Test]
        [Timeout(30000)]
        public async Task RiverScene_RF35_ElPersonajeSeDesplazaConLosBotonesEnPantalla()
        {
            var river = await OpenRiver();
            var origen = river.Walk.Position;
            var enPantallaAntes = EnPantalla(river.Player).center;
            var derecha = river.Pads.Single(pad => pad.Direction == Vector2.right);

            // Clic sostenido sobre la flecha: el mismo evento que entrega el EventSystem.
            derecha.OnPointerDown(new PointerEventData(EventSystem.current));
            for (var i = 0; i < 5; i++)
            {
                await Awaitable.NextFrameAsync();
            }

            Assert.That(river.Walk.Position.x, Is.GreaterThan(origen.x), "sostener «→» mueve a Mamá hacia la derecha");
            Assert.That(river.Walk.Position.y, Is.EqualTo(origen.y).Within(1e-4f), "y solo hacia la derecha");
            Assert.That(EnPantalla(river.Player).center.x, Is.GreaterThan(enPantallaAntes.x), "y se ve moverse en pantalla");

            derecha.OnPointerUp(new PointerEventData(EventSystem.current));
            await Awaitable.NextFrameAsync();
            var quieta = river.Walk.Position;
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            Assert.That(river.Walk.Position, Is.EqualTo(quieta), "al soltar se detiene: clic sostenido, no interruptor");

            var direcciones = river.Pads.Select(pad => pad.Direction).ToArray();
            Assert.That(direcciones, Is.EquivalentTo(new[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right }),
                "cuatro flechas, una por dirección (RF-35)");
            Assert.That(river.Pads.All(pad => pad.GetComponent<Button>() != null && pad.isActiveAndEnabled), Is.True,
                "cada flecha es un botón en pantalla, alcanzable por raycast");
        }

        [Test]
        public void RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles()
        {
            // El asset de acciones del juego, no solo el comportamiento: basta una acción con
            // binding de teclado —aunque nadie la use— para que RNF-02 deje de salir limpio (R3).
            var mapa = File.ReadAllText(Path.Combine(Application.dataPath, "Game/Input/ControlesJugables.inputactions"));
            Assert.That(mapa, Does.Not.Contain("<Keyboard>"), "ninguna vinculación de teclado en ControlesJugables (CT-06, INC-01)");
            Assert.That(mapa, Does.Not.Contain("<Gamepad>"), "ni de mando");

            // Y el assembly del nivel no puede ni leer el teclado: no referencia el Input System
            // —los botones son uGUI puro— ni el módulo de la clase `Input` legada (CT-06).
            var referencias = typeof(RiverSceneController).Assembly.GetReferencedAssemblies().Select(a => a.Name).ToArray();
            Assert.That(referencias, Does.Not.Contain("Unity.InputSystem"), "Game.Levels.River no usa acciones del Input System");
            Assert.That(referencias, Does.Not.Contain("UnityEngine.InputLegacyModule"), "ni la clase Input legada");
        }

        [Test]
        [Timeout(30000)]
        public async Task RiverScene_RF35_ElPersonajeNoAtraviesaLosLimitesDelEscenario()
        {
            var river = await OpenRiver();
            var orilla = river.Config.WalkableArea;
            var pantalla = new Rect(0, 0, Screen.width, Screen.height);

            foreach (var (direccion, esperado) in new[]
                     {
                         (Vector2.left, new Vector2(orilla.xMin, float.NaN)),
                         (Vector2.down, new Vector2(float.NaN, orilla.yMin)),
                         (Vector2.right, new Vector2(orilla.xMax, float.NaN)),
                         (Vector2.up, new Vector2(float.NaN, orilla.yMax))
                     })
            {
                river.Tick(direccion, 100f); // Mucho más de lo que hace falta para cruzar la orilla entera.
                Canvas.ForceUpdateCanvases();
                await Awaitable.NextFrameAsync();

                if (!float.IsNaN(esperado.x))
                {
                    Assert.That(river.Walk.Position.x, Is.EqualTo(esperado.x).Within(1e-4f), $"se detiene en el borde {direccion}");
                }

                if (!float.IsNaN(esperado.y))
                {
                    Assert.That(river.Walk.Position.y, Is.EqualTo(esperado.y).Within(1e-4f), $"se detiene en el borde {direccion}");
                }

                var caja = EnPantalla(river.Player);
                Assert.That(pantalla.Contains(caja.min) && pantalla.Contains(caja.max), Is.True,
                    $"en el borde {direccion} Mamá sigue entera en cámara — {caja}");
                Assert.That(EnPantalla(river.Environment.rectTransform).Contains(caja.center), Is.True, "y sobre la ilustración");
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task RiverScene_RF13_ElBotonDeAyudaEstaVisibleYRepiteLaInstruccion()
        {
            var river = await OpenRiver();
            var instruccion = river.MessageLabel.text;

            Assert.That(instruccion, Is.Not.Empty, "la fase abre con la instrucción de «Recolectar»");
            Assert.That(river.HelpButton.isActiveAndEnabled, Is.True, "la ayuda está a mano toda la fase (RF-13)");

            river.MessageLabel.text = string.Empty;
            river.HelpButton.onClick.Invoke();
            Assert.That(river.MessageLabel.text, Is.EqualTo(instruccion), "pedir ayuda repite la instrucción vigente");
        }

        [Test]
        [Timeout(30000)]
        public async Task RiverScene_RNF02_LaEscenaSoloRespondeAClicYClicSostenido()
        {
            await OpenRiver();
            var interactivos = Object.FindObjectsByType<Selectable>(FindObjectsInactive.Include);

            Assert.That(interactivos, Is.Not.Empty);
            Assert.That(interactivos.Where(elemento => !(elemento is Button)).Select(elemento => elemento.name),
                Is.Empty, "todo lo interactivo es un botón: sin deslizadores ni campos");

            var comportamientos = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include);
            var arrastrables = comportamientos
                .Where(c => c is IDragHandler || c is IBeginDragHandler)
                .Select(c => c.name).ToArray();
            var sostenidos = comportamientos
                .Where(c => c is IPointerDownHandler && !(c is Selectable))
                .Select(c => c.name).ToArray();

            Assert.That(arrastrables, Is.Empty, "nada usa el arrastre de uGUI");
            // Las cuatro flechas son el clic sostenido de RF-35; «BotonPausa» (prefab MenuPausa)
            // atiende el pulsar solo para hundir su cara mientras dura el clic.
            Assert.That(sostenidos, Is.EquivalentTo(new[] { "Flecha_Arriba", "Flecha_Abajo", "Flecha_Izquierda", "Flecha_Derecha", "BotonPausa" }),
                "lo único que responde al clic sostenido son las flechas (y el botón de pausa, solo para pintarse)");
        }

        [Test]
        [Timeout(30000)]
        public async Task RiverScene_RNF03_ControlesListaEInventarioCabenEnPantallaYNoSeSolapan()
        {
            var river = await OpenRiver();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var pantalla = new Rect(0, 0, Screen.width, Screen.height);
            var permanentes = river.Pads.Select(pad => (pad.name, (RectTransform)pad.transform)).Concat(new[]
            {
                ("recoger", (RectTransform)river.CollectButton.transform),
                ("lista de tareas", river.TaskArea),
                ("inventario", river.InventoryArea),
                ("ayuda", (RectTransform)river.HelpButton.transform),
                ("tablilla del guía", (RectTransform)river.MessageLabel.transform.parent)
            }).ToArray();

            foreach (var (nombre, rect) in permanentes)
            {
                var caja = EnPantalla(rect);
                Assert.That(pantalla.Contains(caja.min) && pantalla.Contains(caja.max), Is.True,
                    $"«{nombre}» cabe entero en pantalla — {caja}");
            }

            for (var i = 0; i < permanentes.Length; i++)
            {
                for (var j = i + 1; j < permanentes.Length; j++)
                {
                    Assert.That(EnPantalla(permanentes[i].Item2).Overlaps(EnPantalla(permanentes[j].Item2)), Is.False,
                        $"«{permanentes[i].Item1}» y «{permanentes[j].Item1}» no se solapan");
                }
            }

            Assert.That(river.TaskArea.gameObject.activeInHierarchy && river.InventoryArea.gameObject.activeInHierarchy, Is.True,
                "lista e inventario visibles desde el primer cuadro (RF-36, RF-38)");
        }

        // --- helpers ---------------------------------------------------------------------------

        internal static async Task<RiverSceneController> OpenRiver()
        {
            var carga = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (!carga.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            // Un cuadro más: `Start` corre después de que la carga se declare terminada.
            await Awaitable.NextFrameAsync();

            var river = Object.FindAnyObjectByType<RiverSceneController>();
            Assert.That(river, Is.Not.Null, $"la escena «{SceneName}» trae su controlador");
            Assert.That(river.Spawned, Is.Not.Empty, "la orilla ya se repartió");
            return river;
        }

        internal static Rect EnPantalla(RectTransform rect)
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
