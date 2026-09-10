using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.Wheel.Tests
{
    [Category("Integration")]
    public class ForestSceneTests
    {
        private const string SceneName = "Level2_Forest";

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF22_PresentaCincoValidosEntreOchoDistractores()
        {
            var forest = await OpenForest();
            var porCategoria = forest.Spawned
                .GroupBy(entrada => entrada.Object.Category)
                .ToDictionary(grupo => grupo.Key, grupo => grupo.Count());

            Assert.That(forest.Spawned.Count, Is.EqualTo(forest.Config.ForestObjects.Length),
                "el bosque se reparte del catálogo, no de objetos puestos a mano (RNF-18)");
            Assert.That(porCategoria[ForestObjectCategory.RoundLog], Is.EqualTo(5),
                "cinco troncos redondos válidos");
            Assert.That(porCategoria.Where(par => par.Key != ForestObjectCategory.RoundLog).Sum(par => par.Value),
                Is.GreaterThanOrEqualTo(8), "entre al menos ocho distractores");
            Assert.That(porCategoria.Keys, Has.Count.EqualTo(4),
                "las tres categorías de distractor están representadas");
            Assert.That(forest.Spawned.Select(entrada => entrada.Button.name).Distinct().Count(),
                Is.EqualTo(forest.Spawned.Count), "cada objeto es señalable por su nombre");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RNF23_CadaObjetoMuestraLaIlustracionDeSuAsset()
        {
            var forest = await OpenForest();

            foreach (var (objeto, boton) in forest.Spawned)
            {
                // El objeto es su ilustración: sin tarjeta ni rótulo, tirado en el suelo.
                Assert.That(boton.image.sprite, Is.SameAs(objeto.Art),
                    $"{boton.name} pinta el sprite que dice su asset, no otro");
                Assert.That(boton.image.enabled, Is.True, $"{boton.name} se ve");
                Assert.That(boton.GetComponentInChildren<Text>(true), Is.Null,
                    $"{boton.name} no lleva rótulo: el patrón se busca mirando, no leyendo");
            }

            // Distintas categorías traen ilustraciones distintas: el patrón se busca mirando, así
            // que si todos compartieran sprite la fase no se podría jugar (RF-23).
            Assert.That(forest.Spawned.Select(entrada => entrada.Object.Art).Distinct().Count(),
                Is.EqualTo(forest.Spawned.Count), "cada objeto tiene su propia ilustración");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF24_ElContadorEsVisibleDuranteTodaLaFase()
        {
            var forest = await OpenForest();
            var visto = new List<string> { forest.CounterLabel.text };

            Assert.That(forest.CounterLabel.text, Is.EqualTo("Troncos redondos: 0 de 5"),
                "arranca en cero con el texto del asset");

            foreach (var entrada in forest.Spawned)
            {
                entrada.Button.onClick.Invoke();
                Assert.That(forest.CounterLabel.isActiveAndEnabled, Is.True,
                    $"el contador sigue visible tras pulsar {entrada.Button.name}");
                visto.Add(forest.CounterLabel.text);
            }

            Assert.That(visto.Last(), Is.EqualTo("Troncos redondos: 5 de 5"),
                "acopiados los cinco, el contador lo dice");
            Assert.That(forest.Selection.IsComplete, Is.True);
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF24_ElContadorNoRetrocedeAlPulsarUnDistractor()
        {
            var forest = await OpenForest();
            Pulsar(forest, ForestObjectCategory.RoundLog, 1);
            var trasElAcierto = forest.CounterLabel.text;

            Pulsar(forest, ForestObjectCategory.Stone, 3);

            Assert.That(forest.CounterLabel.text, Is.EqualTo(trasElAcierto),
                "tres rechazos no mueven el contador (CP-02, RF-18)");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RNF19_ElRechazoSeDistingueSinDependerDelColor()
        {
            var forest = await OpenForest();
            var colorDeLaFrase = forest.MessageLabel.color;

            Pulsar(forest, ForestObjectCategory.RoundLog, 1);
            var formaAcierto = forest.MessageIcon.sprite;
            var colorAcierto = forest.MessageIcon.color;

            Pulsar(forest, ForestObjectCategory.Plant, 1);
            var formaRechazo = forest.MessageIcon.sprite;

            Assert.That(formaAcierto, Is.Not.Null, "el acierto trae forma, no solo color");
            Assert.That(formaRechazo, Is.Not.Null, "el rechazo también");
            Assert.That(forest.MessageIcon.enabled, Is.True, "y el icono se ve");
            Assert.That(formaRechazo, Is.Not.SameAs(formaAcierto),
                "la forma distingue los dos estados por sí sola: quien no perciba el color los separa igual (RNF-19)");
            Assert.That(forest.MessageIcon.color, Is.Not.EqualTo(colorAcierto),
                "y el color acompaña, que es el primer canal");
            Assert.That(forest.MessageLabel.color, Is.EqualTo(colorDeLaFrase),
                "la frase no cambia de color: teñirla la dejó ilegible sobre la tablilla (RNF-20)");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF13_ElBotonDeAyudaRepiteLaInstruccionYNoAcercaLaPista()
        {
            var forest = await OpenForest();
            var instruccion = forest.MessageLabel.text;

            Assert.That(instruccion, Is.Not.Empty, "la fase abre con la instrucción vigente");
            Assert.That(forest.HelpButton.isActiveAndEnabled, Is.True,
                "la ayuda está a mano durante toda la fase, no escondida tras señalar algo");

            Pulsar(forest, ForestObjectCategory.Stone, 2);
            for (var i = 0; i < 5; i++)
            {
                forest.HelpButton.onClick.Invoke();
                Assert.That(forest.MessageLabel.text, Is.EqualTo(instruccion),
                    "pedir ayuda repite la instrucción vigente (RF-13)");
            }

            // Pedir ayuda cinco veces no contó como intento: el tercer fallo sigue siendo el
            // siguiente rechazo, y es ahí donde aparece la pista (CP-06, RF-13).
            Pulsar(forest, ForestObjectCategory.Stone, 1);
            Assert.That(forest.MessageLabel.text, Is.Not.EqualTo(instruccion),
                "el tercer fallo consecutivo trae la pista");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RNF02_LaEscenaSoloRespondeAClicSimple()
        {
            await OpenForest();
            var interactivos = Object.FindObjectsByType<Selectable>(FindObjectsInactive.Include);

            Assert.That(interactivos, Is.Not.Empty);
            Assert.That(interactivos.Where(elemento => !(elemento is Button)).Select(elemento => elemento.name),
                Is.Empty, "todo lo interactivo es un botón: sin deslizadores, barras ni campos");

            // Clic sostenido y arrastre son de W07 y del Nivel 3; en el bosque no hay ninguno
            // (RNF-02, CT-06). Se comprueba por la interfaz que Unity usa para enrutarlos.
            var arrastrables = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include)
                .Where(comportamiento => comportamiento is IDragHandler || comportamiento is IBeginDragHandler)
                .Select(comportamiento => comportamiento.name)
                .ToArray();

            Assert.That(arrastrables, Is.Empty, "ningún elemento del bosque se arrastra");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RNF03_NadaSeSaleDePantallaNiSeSolapa()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var pantalla = new Rect(0, 0, Screen.width, Screen.height);
            var permanentes = new (string Nombre, RectTransform Rect)[]
            {
                ("contador", (RectTransform)forest.CounterLabel.transform),
                ("acopio", (RectTransform)forest.Slots[0].transform.parent),
                ("ayuda", (RectTransform)forest.HelpButton.transform),
                ("tablilla del guía", (RectTransform)forest.MessageLabel.transform.parent)
            };

            foreach (var (nombre, rect) in permanentes)
            {
                var caja = EnPantalla(rect);
                Assert.That(pantalla.Contains(caja.min) && pantalla.Contains(caja.max), Is.True,
                    $"«{nombre}» cabe entero en pantalla — {caja} dentro de {pantalla}");
            }

            for (var i = 0; i < permanentes.Length; i++)
            {
                for (var j = i + 1; j < permanentes.Length; j++)
                {
                    Assert.That(EnPantalla(permanentes[i].Rect).Overlaps(EnPantalla(permanentes[j].Rect)),
                        Is.False, $"«{permanentes[i].Nombre}» y «{permanentes[j].Nombre}» no se solapan");
                }
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF22_LosObjetosCaenEnLaMitadInferiorDeLaPantalla()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var mitad = Screen.height / 2f;
            var arriba = forest.Spawned
                .Select(entrada => (entrada.Button.name, Caja: EnPantalla((RectTransform)entrada.Button.transform)))
                .Where(x => x.Caja.yMax > mitad)
                .Select(x => $"{x.name} llega a y={x.Caja.yMax:0} y el suelo acaba en {mitad:0}")
                .ToArray();

            Assert.That(arriba, Is.Empty, "el suelo es la mitad inferior de la pantalla");
            Assert.That(forest.Spawned.Select(entrada => entrada.Object.FloorPosition).Distinct().Count(),
                Is.EqualTo(forest.Spawned.Count), "están repartidos, no apilados en el mismo punto");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF24_ElAcopioSeLlenaConformeSeRecogenLosTroncos()
        {
            var forest = await OpenForest();

            Assert.That(forest.Slots, Has.Count.EqualTo(forest.Config.RequiredLogs),
                "una casilla por tronco que pide la fase");
            Assert.That(forest.Slots.Select(casilla => casilla.sprite), Has.All.Null,
                "el acopio empieza vacío");

            var troncos = forest.Spawned
                .Where(entrada => entrada.Object.Category == ForestObjectCategory.RoundLog)
                .ToArray();

            for (var i = 0; i < troncos.Length; i++)
            {
                troncos[i].Button.onClick.Invoke();

                Assert.That(troncos[i].Button.gameObject.activeSelf, Is.False,
                    "el tronco recogido deja el suelo");
                Assert.That(forest.Slots[i].sprite, Is.SameAs(troncos[i].Object.Art),
                    "y aparece en la casilla siguiente del acopio");
                Assert.That(forest.Slots.Count(casilla => casilla.sprite != null), Is.EqualTo(i + 1),
                    "el acopio se llena en orden y sin saltos");
            }

            // Un distractor no ocupa casilla: lo acopiado es solo lo que comparte el patrón.
            Pulsar(forest, ForestObjectCategory.Stone, 3);
            Assert.That(forest.Slots.Count(casilla => casilla.sprite != null),
                Is.EqualTo(forest.Config.RequiredLogs), "rechazar no vacía ninguna casilla (CP-02)");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF22_NingunObjetoQuedaTapadoPorLaInterfaz()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var tapan = new (string Nombre, Rect Caja)[]
            {
                ("el acopio", EnPantalla((RectTransform)forest.Slots[0].transform.parent)),
                ("la ayuda", EnPantalla((RectTransform)forest.HelpButton.transform)),
                ("el contador", EnPantalla((RectTransform)forest.CounterLabel.transform)),
                ("la tablilla del guía", EnPantalla((RectTransform)forest.MessageLabel.transform.parent))
            };

            var tapados = forest.Spawned
                .SelectMany(entrada => tapan
                    .Where(panel => panel.Caja.Overlaps(EnPantalla((RectTransform)entrada.Button.transform)))
                    .Select(panel => $"{entrada.Button.name} queda bajo {panel.Nombre}"))
                .ToArray();

            // Un objeto debajo de un panel no recibe el clic: si es uno de los cinco troncos, la
            // fase no se puede terminar. El reparto lo decide el asset, así que esto vigila el
            // contenido tanto como el diseño de la escena.
            Assert.That(tapados, Is.Empty, string.Join(" · ", tapados));
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RNF03_LaInterfazEscalaConLaResolucionComoElRestoDelJuego()
        {
            await OpenForest();
            var escalador = Object.FindAnyObjectByType<CanvasScaler>();

            // El bosque era la única escena en `ConstantPixelSize` sobre 800x600 mientras las
            // otras cinco escalan sobre 1920x1080: una composición hecha a la medida del mockup
            // salía descuadrada en cuanto la ventana no medía exactamente eso.
            Assert.That(escalador.uiScaleMode,
                Is.EqualTo(CanvasScaler.ScaleMode.ScaleWithScreenSize), "modo de escalado");
            Assert.That(escalador.referenceResolution, Is.EqualTo(new Vector2(1920f, 1080f)),
                "la resolución de diseño de los mockups");
        }

        // --- helpers ---------------------------------------------------------------------------

        private static async Task<ForestSceneController> OpenForest()
        {
            var carga = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (!carga.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            // Un cuadro más: `Start` corre después de que la carga se declare terminada, y es ahí
            // donde el bosque se reparte.
            await Awaitable.NextFrameAsync();

            var forest = Object.FindAnyObjectByType<ForestSceneController>();
            Assert.That(forest, Is.Not.Null, $"la escena «{SceneName}» trae su controlador");
            Assert.That(forest.Spawned, Is.Not.Empty, "el bosque ya se repartió");
            return forest;
        }

        private static void Pulsar(ForestSceneController forest, ForestObjectCategory categoria, int veces)
        {
            var candidatos = forest.Spawned
                .Where(entrada => entrada.Object.Category == categoria)
                .ToArray();

            for (var i = 0; i < veces; i++)
            {
                candidatos[i % candidatos.Length].Button.onClick.Invoke();
            }
        }

        /// <summary>La caja del elemento en píxeles de pantalla, que es donde se ve.</summary>
        private static Rect EnPantalla(RectTransform rect)
        {
            var esquinas = new Vector3[4];
            rect.GetWorldCorners(esquinas);
            return new Rect(esquinas[0].x, esquinas[0].y,
                esquinas[2].x - esquinas[0].x, esquinas[2].y - esquinas[0].y);
        }
    }
}
