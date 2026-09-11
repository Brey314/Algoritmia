using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Core;
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

            foreach (var (objeto, boton, _) in forest.Spawned)
            {
                // El objeto es su ilustración: sin tarjeta ni rótulo, tirado en el suelo.
                Assert.That(boton.image.sprite, Is.SameAs(objeto.Art),
                    $"{boton.name} pinta el sprite que dice su asset, no otro");
                Assert.That(boton.image.enabled, Is.True, $"{boton.name} se ve");
                Assert.That(boton.GetComponentInChildren<Text>(true), Is.Null,
                    $"{boton.name} no lleva rótulo: el patrón se busca mirando, no leyendo");
            }

            // Distintas categorías traen ilustraciones distintas: el patrón se busca mirando, así
            // que si todas compartieran sprite la fase no se podría jugar (RF-23).
            //
            // **Se exige por categoría y no por objeto.** Antes se pedía un sprite distinto para
            // cada uno de los catorce; con arte generado eso son cinco troncos «parecidos pero no
            // iguales» y cinco oportunidades de que uno deje de leerse como redondo. Lo que RF-23
            // necesita es que las categorías no se confundan entre sí, y eso es lo que se afirma
            // aquí; que dentro de una categoría no salgan calcados lo cubre la postura, abajo.
            var spritesPorCategoria = forest.Spawned
                .GroupBy(entrada => entrada.Object.Category)
                .ToDictionary(grupo => grupo.Key,
                    grupo => grupo.Select(entrada => entrada.Object.Art).Distinct().ToArray());

            Assert.That(spritesPorCategoria.Values.Where(sprites => sprites.Length != 1), Is.Empty,
                "cada categoría se dibuja con un solo sprite");
            Assert.That(spritesPorCategoria.Values.SelectMany(sprites => sprites).Distinct().Count(),
                Is.EqualTo(spritesPorCategoria.Count), "y ninguna lo comparte con otra");
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

            // La caja se agarra con el clic sostenido y se suelta al soltar el clic (RF-25): eso
            // es pulsar y soltar, exactamente el esquema de RNF-02, y no el arrastre de uGUI, que
            // nadie implementa. Se comprueba por la interfaz que Unity usa para enrutar cada cosa
            // (CT-06). `Selectable` también atiende el pulsar, pero solo para pintarse: se aparta.
            var comportamientos = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include);
            var arrastrables = comportamientos
                .Where(comportamiento => comportamiento is IDragHandler || comportamiento is IBeginDragHandler)
                .Select(comportamiento => comportamiento.name)
                .ToArray();
            var sostenidos = comportamientos
                .Where(comportamiento => comportamiento is IPointerDownHandler && !(comportamiento is Selectable))
                .Select(comportamiento => comportamiento.name)
                .ToArray();

            Assert.That(arrastrables, Is.Empty, "ningún elemento del bosque usa el arrastre de uGUI");
            Assert.That(sostenidos, Is.EqualTo(new[] { "Objeto_Caja" }),
                "lo único que responde al clic sostenido es la caja");
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
                ("empujar", (RectTransform)forest.PushButton.transform),
                ("caja", forest.CargoRect),
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
        public async Task ForestScene_RF22_LosObjetosCaenEntreElCincoYElSesentaPorCientoDeLaPantalla()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            // El suelo del claro va del 5 % al 60 % de la altura de la pantalla, medido desde
            // abajo: por debajo queda el borde y por encima los troncos del fondo, que no se pisan.
            var techo = Screen.height * 0.60f;
            var piso = Screen.height * 0.05f;
            var fuera = forest.Spawned
                .Select(entrada => (entrada.Button.name, Caja: EnPantalla((RectTransform)entrada.Button.transform)))
                .Where(x => x.Caja.yMax > techo + 0.5f || x.Caja.yMin < piso - 0.5f)
                .Select(x => $"{x.name} ocupa y={x.Caja.yMin:0}..{x.Caja.yMax:0} y el suelo va de {piso:0} a {techo:0}")
                .ToArray();

            Assert.That(fuera, Is.Empty, string.Join(" · ", fuera));
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
                ("la caja", EnPantalla(forest.CargoRect)),
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

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF22_CadaObjetoApareceConLaOrientacionDeSuAsset()
        {
            var forest = await OpenForest();

            foreach (var (objeto, boton, _) in forest.Spawned)
            {
                var rect = (RectTransform)boton.transform;

                Assert.That(rect.localEulerAngles.z,
                    Is.EqualTo(Mathf.Repeat(objeto.RotationDegrees, 360f)).Within(0.01f),
                    $"{boton.name} aparece girado lo que dice su asset");
                Assert.That(Mathf.Sign(rect.localScale.x), Is.EqualTo(objeto.Mirrored ? -1f : 1f),
                    $"{boton.name} usa el espejo del asset, no un segundo sprite");

                // El espejo invierte, no estira: si las dos escalas dejan de medir lo mismo el
                // objeto sale deformado y el patrón de RF-23 se lee mal.
                Assert.That(Mathf.Abs(rect.localScale.x),
                    Is.EqualTo(Mathf.Abs(rect.localScale.y)).Within(0.001f),
                    $"{boton.name} no queda deformado por el espejo");
            }

            var posturas = forest.Spawned
                .Select(entrada => (entrada.Object.RotationDegrees, entrada.Object.Mirrored))
                .Distinct()
                .Count();

            Assert.That(posturas, Is.GreaterThan(1),
                "el bosque no es papel pintado: la misma ilustración aparece en posturas distintas");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF22_ElObjetoSeApartaAlAcercarseElCursor()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var tronco = forest.Spawned.First(
                entrada => entrada.Object.Category == ForestObjectCategory.RoundLog);
            var rect = (RectTransform)tronco.Button.transform;
            var sitio = rect.anchorMin;

            // El cursor se pega al objeto por su izquierda y se queda ahí.
            var cursor = EnPantalla(rect).center + Vector2.left * 24f;
            for (var frame = 0; frame < 90; frame++)
            {
                forest.Nudge(cursor, 1f / 60f);
            }

            Assert.That(rect.anchorMin, Is.Not.EqualTo(sitio), "el tronco acusa el paso del cursor");
            Assert.That(rect.anchorMin.x, Is.GreaterThan(sitio.x),
                "y se aparta de él, no hacia él");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_CP02_ApartarLosObjetosNoTocaElAcopioNiLaFrase()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var contador = forest.CounterLabel.text;
            var frase = forest.MessageLabel.text;
            var acopiados = forest.Selection.Collected;

            await BarrerElSuelo(forest);

            // **Lo que sostiene que esto es adorno y no mecánica.** Pasear el cursor por todo el
            // bosque mueve objetos y no toca nada más: ni el contador, ni la frase del guía, ni el
            // acopio. Sin esta prueba, el efecto sería una segunda forma de interactuar y RNF-02
            // dejaría de ser cierto (CT-06, CP-02).
            Assert.That(forest.CounterLabel.text, Is.EqualTo(contador), "el contador no se mueve");
            Assert.That(forest.MessageLabel.text, Is.EqualTo(frase), "el guía no dice nada nuevo");
            Assert.That(forest.Selection.Collected, Is.EqualTo(acopiados), "no se acopió nada");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RNF03_NingunObjetoSeVaDelSueloPorMuchoQueSeEmpuje()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            await BarrerElSuelo(forest);
            await BarrerElSuelo(forest);

            var fugados = forest.Spawned
                .Select(entrada => (entrada.Button.name, Ancla: ((RectTransform)entrada.Button.transform).anchorMin))
                .Where(objeto => objeto.Ancla.x < 0f || objeto.Ancla.x > 1f ||
                                 objeto.Ancla.y < 0f || objeto.Ancla.y > 1f)
                .Select(objeto => $"{objeto.name} en {objeto.Ancla}")
                .ToArray();

            Assert.That(fugados, Is.Empty, string.Join(" · ", fugados));
        }

        // --- W07: la caja y el rodado ------------------------------------------------------------

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF25_LaCajaNoSeArrastraSinLosCincoTroncos()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Pulsar(forest, ForestObjectCategory.RoundLog, 1);
            var sitio = forest.CargoRect.anchoredPosition;

            forest.TakeCargo();
            forest.DragCargoTo(Centro((RectTransform)forest.Slots[0].transform.parent));

            Assert.That(forest.Cargo.IsHeld, Is.False, "sin los cinco troncos la caja no se agarra");
            Assert.That(forest.CargoRect.anchoredPosition, Is.EqualTo(sitio), "y no se mueve del sitio");
            Assert.That(forest.MessageLabel.text,
                Is.EqualTo(string.Format(forest.Config.PendingLogsFormat, forest.Config.RequiredLogs - 1)),
                "el guía dice cuántos faltan, con el texto del asset (CU-06 FA-4a, RNF-18)");
            Assert.That(forest.Cargo.IsPlaced, Is.False);
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF25_ConLosCincoTroncosLaCajaSigueAlClicSostenidoYSeColocaAlSoltar()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            await Acopiar(forest);
            var fila = forest.LogRow;

            forest.TakeCargo();
            Assert.That(forest.Cargo.IsHeld, Is.True, "con el acopio completo el clic sostenido agarra la caja");

            forest.DragCargoTo(Centro(fila));
            Canvas.ForceUpdateCanvases();
            Assert.That(Vector2.Distance(Centro(forest.CargoRect), Centro(fila)), Is.LessThan(2f),
                "mientras se sostiene, la caja va donde está el cursor");

            forest.ReleaseCargo();
            Canvas.ForceUpdateCanvases();

            Assert.That(forest.Cargo.IsHeld, Is.False, "soltar el clic suelta la caja");
            Assert.That(forest.Cargo.IsPlaced, Is.True, "cayó sobre los troncos alineados");
            Assert.That(EnPantalla(forest.CargoRect).Overlaps(EnPantalla(fila)), Is.True,
                "y se ve encima de ellos");
            Assert.That(forest.MessageLabel.text, Is.EqualTo(forest.Config.CargoPlacedMessage),
                "el guía lo dice con el texto del asset");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF26_EmpujarSeHabilitaSoloConLaCajaColocada()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var fila = forest.LogRow;
            var fuera = Centro((RectTransform)forest.HelpButton.transform);

            Assert.That(forest.PushButton.gameObject.activeSelf, Is.False,
                "«Empujar» no aparece durante el acopio: todavía no tiene tarea");

            await Acopiar(forest);
            Assert.That(forest.PushButton.isActiveAndEnabled, Is.True, "aparece con la fila de troncos");
            Assert.That(forest.PushButton.interactable, Is.False, "pero no se puede accionar sin la caja colocada");

            forest.TakeCargo();
            forest.DragCargoTo(fuera);
            forest.ReleaseCargo();

            Assert.That(forest.PushButton.interactable, Is.False, "suelta fuera de los troncos, sigue sin habilitarse");
            Assert.That(forest.MessageLabel.text, Is.EqualTo(forest.Config.CargoMissedMessage),
                "y se dice dónde quedó, sin regañar (CP-02)");

            forest.TakeCargo();
            forest.DragCargoTo(Centro(fila));
            forest.ReleaseCargo();

            Assert.That(forest.PushButton.interactable, Is.True, "colocada, «Empujar» se habilita (RF-26)");

            // Se vuelve a levantar y se deja en cualquier otro sitio: lo ganado permanece y el
            // botón no vuelve a deshabilitarse (CP-02, RF-41, mismo criterio que INC-32).
            forest.TakeCargo();
            forest.DragCargoTo(fuera);
            forest.ReleaseCargo();

            Assert.That(forest.PushButton.interactable, Is.True,
                "«Empujar», una vez habilitado, no se vuelve a deshabilitar");
        }

        [Test]
        [Timeout(30000)]
        [Category("VisualVerification")]
        [Description("Tras ejecutar esta prueba, revisar la captura de mitad del rodado: la caja va " +
                     "sobre la fila de troncos y ninguno parpadea ni cambia de color; el movimiento " +
                     "se lee como un rodado continuo, sin saltos ni destellos.")]
        public async Task ForestScene_RNF21_LaAnimacionDelRodadoNoTieneDestellos()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            await Colocar(forest);

            var graficos = Object.FindObjectsByType<Graphic>(FindObjectsInactive.Exclude);
            var antes = graficos.ToDictionary(grafico => grafico, grafico => (grafico.enabled, grafico.color));
            var xPrevia = forest.CargoRect.anchoredPosition.x;
            var frames = 0;
            var capturada = false;

            forest.PushButton.onClick.Invoke();
            Assert.That(forest.IsRolling, Is.True, "accionar «Empujar» arranca el rodado");

            while (forest.IsRolling)
            {
                await Awaitable.NextFrameAsync();
                frames++;

                // Un destello es un gráfico que se apaga y enciende o cambia de color entre dos
                // cuadros. Durante el rodado nada de eso ocurre: solo se mueven la caja y los
                // troncos (RNF-21).
                foreach (var grafico in graficos.Where(grafico => grafico != null))
                {
                    Assert.That((grafico.enabled, grafico.color), Is.EqualTo(antes[grafico]),
                        $"{grafico.name} no parpadea ni cambia de color durante el rodado");
                }

                // Con tolerancia de una centésima de píxel: el ruido de convertir pantalla ↔
                // lienzo no es un salto, y un salto de verdad son píxeles enteros.
                Assert.That(forest.CargoRect.anchoredPosition.x, Is.GreaterThanOrEqualTo(xPrevia - 0.01f),
                    "la caja avanza siempre en el mismo sentido: no salta hacia atrás");
                xPrevia = forest.CargoRect.anchoredPosition.x;

                if (!capturada && frames > 5)
                {
                    capturada = Capturar("ForestScene_RNF21_Rodado");
                }
            }

            Assert.That(frames, Is.GreaterThan(1), "el rodado dura varios cuadros: no es un salto");
            Assert.That(forest.Cargo.IsPushed, Is.True);
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF26_AlPasarElUltimoTroncoLaCajaCaeAlSueloPorLaDerecha()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            await Colocar(forest);
            var fila = EnPantalla(forest.LogRow);

            forest.PushButton.onClick.Invoke();
            while (forest.IsRolling)
            {
                await Awaitable.NextFrameAsync();
            }

            // La caja cae ladeada, así que su envolvente crece: se mide por el centro y por su
            // alto sin girar, no por las esquinas de la envolvente.
            var caja = EnPantalla(forest.CargoRect);
            var alto = forest.CargoRect.rect.height * Mathf.Abs(forest.CargoRect.lossyScale.y);
            Assert.That(caja.center.x, Is.GreaterThan(fila.xMax), "la caja queda a la derecha del último tronco");
            Assert.That(caja.center.y - alto / 2f, Is.EqualTo(fila.yMin).Within(2f),
                "y en el suelo, a la altura de la base de los troncos");
            Assert.That(forest.CargoRect.localEulerAngles.z, Is.Not.EqualTo(0f).Within(0.5f), "ladeada, como cae lo que pesa");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF26_ElRodadoSeReproduceUnaSolaVezYNoDeshaceLaColocacion()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            await Colocar(forest);

            forest.PushButton.onClick.Invoke();
            while (forest.IsRolling)
            {
                await Awaitable.NextFrameAsync();
            }

            var final = forest.CargoRect.anchoredPosition;
            forest.PushButton.onClick.Invoke();

            Assert.That(forest.IsRolling, Is.False, "la segunda pulsación no relanza el rodado");
            Assert.That(forest.CargoRect.anchoredPosition, Is.EqualTo(final), "ni mueve la caja de donde acabó");
            Assert.That(forest.PushButton.interactable, Is.True, "y el botón sigue habilitado (CP-02)");
        }

        [Test]
        [Timeout(60000)]
        public async Task ForestScene_RF04_ConfirmarLaFase1GuardaElProgreso()
        {
            const string nombre = "W07_Prueba";
            GameFlowRunner runner = null;

            try
            {
                SceneManager.LoadScene("Boot");
                await Esperar(() => GameFlowRunner.Instance != null
                                    && SceneManager.GetActiveScene().name == "MainMenu");
                runner = GameFlowRunner.Instance;
                runner.Session.Delete(nombre);

                runner.GoTo(GameState.ProfileSelect);
                var perfil = PlayerProfile.Create(nombre, System.Array.Empty<string>()).Profile;
                perfil.Reach(LevelId.Wheel);
                runner.SelectProfile(perfil);
                runner.StartPlaying(LevelId.Wheel, 1);
                await Esperar(() => SceneManager.GetActiveScene().name == SceneName);
                await Esperar(() => Object.FindAnyObjectByType<ForestSceneController>()?.Spawned.Count > 0);
                var forest = Object.FindAnyObjectByType<ForestSceneController>();
                Canvas.ForceUpdateCanvases();
                await Awaitable.NextFrameAsync();
                var fase1 = new PhaseId(LevelId.Wheel, 1);
                Assume.That(perfil.IsPhaseConfirmed(fase1), Is.False);

                await Colocar(forest);
                forest.PushButton.onClick.Invoke();
                await Esperar(() => runner.Flow.Current != GameState.Playing);

                Assert.That(perfil.IsPhaseConfirmed(fase1), Is.True, "el rodado confirma la fase 1 (RF-04)");
                Assert.That(runner.Session.Load(nombre).IsPhaseConfirmed(fase1), Is.True,
                    "y queda en disco: un cierre forzado retoma en la fase 2 (RNF-14)");
                Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative),
                    "el bosque sale a la escena narrativa que declara el asset");
                Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo(forest.Config.ClosingSequenceId));
            }
            finally
            {
                runner?.Session.Delete(nombre);
                foreach (var persistente in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
                {
                    Object.DestroyImmediate(persistente.gameObject);
                }

                foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
                {
                    Object.DestroyImmediate(loader.gameObject);
                }
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF23_AlCompletarElAcopioQuedanLaCajaALaIzquierdaYLosTroncosASuDerecha()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var acopio = forest.Slots[0].transform.parent.gameObject;
            Assert.That(forest.LogRow.gameObject.activeSelf, Is.False, "durante el acopio no hay fila de troncos");
            Assert.That(forest.World.localScale, Is.EqualTo(Vector3.one), "y la cámara está en el plano general");

            await Acopiar(forest);

            // Ya no queda nada que elegir: lo que no es la tarea se va y la pantalla se lee de un
            // vistazo — una caja, cinco troncos en fila a su derecha y un botón.
            Assert.That(forest.Spawned.Where(entrada => entrada.Button.gameObject.activeSelf), Is.Empty,
                "los distractores desaparecen del suelo");
            Assert.That(acopio.activeSelf, Is.False, "y la tablilla del acopio también");
            Assert.That(forest.World.localScale.x, Is.GreaterThan(1f), "la cámara se acercó a la caja");
            // Acercarse es llegar **exactamente** al encuadre con el que abre la 2.2, que lo hereda
            // sin moverse (Camara_Narrativa_N2.md §5.3): el punto de la ilustración que queda en el
            // centro de la pantalla es el foco de cierre del asset.
            var entorno = EnPantalla(forest.Environment.rectTransform);
            var enElCentro = new Vector2(
                (Screen.width / 2f - entorno.xMin) / entorno.width,
                (Screen.height / 2f - entorno.yMin) / entorno.height);
            Assert.That(enElCentro.x, Is.EqualTo(forest.Config.CompletionFraming.Focus.x).Within(0.005f),
                "la vista termina en el foco de cierre");
            Assert.That(enElCentro.y, Is.EqualTo(forest.Config.CompletionFraming.Focus.y).Within(0.005f));
            Assert.That(forest.World.localScale.x,
                Is.EqualTo(forest.Config.CompletionFraming.Zoom / forest.Config.PlayFraming.Zoom).Within(0.005f),
                "y con su acercamiento");
            Assert.That(forest.PushButton.isActiveAndEnabled, Is.True, "y aparece «Empujar»");
            Assert.That(forest.CounterLabel.isActiveAndEnabled, Is.True, "el contador se queda (RF-24)");
            Assert.That(forest.LogRow.gameObject.activeSelf, Is.True, "aparece la fila de troncos");
            Assert.That(forest.Row.Select(tronco => tronco.sprite),
                Is.EqualTo(forest.Spawned.Where(e => e.Object.Category == ForestObjectCategory.RoundLog)
                    .Select(e => e.Object.Art)), "con los cinco troncos acopiados, en su orden");

            var caja = EnPantalla(forest.CargoRect);
            var fila = EnPantalla(forest.LogRow);
            Assert.That(fila.xMin, Is.GreaterThan(caja.xMax), "los troncos quedan a la derecha de la caja");
            Assert.That(Mathf.Abs(fila.yMin - caja.yMin), Is.LessThan(caja.height),
                "y a su misma altura, no en otra parte de la pantalla");

            var visibles = new[]
            {
                ("la fila", fila), ("«Empujar»", EnPantalla((RectTransform)forest.PushButton.transform)),
                ("la ayuda", EnPantalla((RectTransform)forest.HelpButton.transform))
            };
            Assert.That(visibles.Where(x => x.Item2.xMin < caja.xMin).Select(x => x.Item1), Is.Empty,
                "la caja es lo que está más a la izquierda del todo");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF25_LaCajaEsperaAlCuarentaPorCientoDeLaAlturaYNingunObjetoSeLeMonta()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var caja = EnPantalla(forest.CargoRect);
            Assert.That(caja.center.y / Screen.height, Is.EqualTo(0.40f).Within(0.02f),
                "la caja espera a un 40 % de la altura de la pantalla");

            var montados = forest.Spawned
                .Where(e => EnPantalla((RectTransform)e.Button.transform).Overlaps(caja))
                .Select(e => e.Button.name)
                .ToArray();
            Assert.That(montados, Is.Empty, "ningún objeto del suelo se monta sobre la caja");

            // Y ningún objeto se monta sobre otro: apilados, solo el de encima recibe el clic.
            var rects = forest.Spawned.Select(e => (e.Button.name, Caja: EnPantalla((RectTransform)e.Button.transform))).ToArray();
            var apilados = new List<string>();
            for (var i = 0; i < rects.Length; i++)
            {
                for (var j = i + 1; j < rects.Length; j++)
                {
                    if (rects[i].Caja.Overlaps(rects[j].Caja))
                    {
                        apilados.Add($"{rects[i].name} {rects[i].Caja} sobre {rects[j].name} {rects[j].Caja}");
                    }
                }
            }

            Assert.That(apilados, Is.Empty,
                $"pantalla {Screen.width}x{Screen.height}, suelo {EnPantalla(forest.FloorArea)}: " + string.Join(" · ", apilados));
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF22_LosObjetosMasAltosSeVenMasPequenos()
        {
            var forest = await OpenForest();

            // El suelo sube hacia el fondo del claro: cuanto más arriba, más lejos y más pequeño.
            var porAltura = forest.Spawned
                .OrderBy(entrada => entrada.Object.FloorPosition.y)
                .Select(entrada => (entrada.Button.name, Altura: entrada.Object.FloorPosition.y,
                    Escala: Mathf.Abs(entrada.Button.transform.localScale.y)))
                .ToArray();

            for (var i = 1; i < porAltura.Length; i++)
            {
                Assert.That(porAltura[i].Escala, Is.LessThanOrEqualTo(porAltura[i - 1].Escala + 0.001f),
                    $"{porAltura[i].name} (y={porAltura[i].Altura}) no puede verse más grande que {porAltura[i - 1].name} (y={porAltura[i - 1].Altura})");
            }

            Assert.That(porAltura.Last().Escala, Is.LessThan(porAltura.First().Escala),
                "el objeto más alto es visiblemente más pequeño que el más bajo");
            Assert.That(porAltura.First().Escala, Is.LessThanOrEqualTo(1f), "nada se dibuja más grande que su tamaño");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RF22_ElObjetoCreceAlAcercarseElCursorYVuelveAlAlejarse()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            // Un tronco y no una piedra: la piedra vuelca a saltos aunque no pase tiempo, y aquí
            // lo que se mide es el tamaño, no el movimiento. Con deltaTime cero nada se desplaza.
            var tronco = forest.Spawned.First(e => e.Object.Category == ForestObjectCategory.RoundLog);
            var rect = (RectTransform)tronco.Button.transform;
            var lejos = new Vector2(-1000f, -1000f);
            forest.Nudge(lejos, 0f);
            var enReposo = Mathf.Abs(rect.localScale.y);

            forest.Nudge(EnPantalla(rect).center, 0f);
            var conElCursor = Mathf.Abs(rect.localScale.y);

            forest.Nudge(lejos, 0f);

            Assert.That(conElCursor, Is.GreaterThan(enReposo), "con el cursor encima crece");
            Assert.That(Mathf.Abs(rect.localScale.y), Is.EqualTo(enReposo).Within(0.001f), "y al alejarse vuelve");
            Assert.That(forest.Selection.Collected, Is.EqualTo(0), "crecer no es seleccionar (RNF-02)");
        }

        [Test]
        [Timeout(30000)]
        public async Task ForestScene_RNF23_ElEntornoCubreLaPantallaSinDeformarse()
        {
            var forest = await OpenForest();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            Assert.That(forest.Environment.sprite, Is.Not.Null, "el bosque ya tiene entorno");
            Assert.That(forest.Environment.enabled, Is.True, "y se ve");

            var caja = EnPantalla(forest.Environment.rectTransform);
            var pantalla = new Rect(0f, 0f, Screen.width, Screen.height);
            var sprite = forest.Environment.sprite;

            // Plano general: el entorno cubre la pantalla entera sin deformarse, a la resolución
            // que traiga el archivo de hoy o el definitivo (RNF-23).
            Assert.That(caja.xMin <= pantalla.xMin + 0.5f && caja.xMax >= pantalla.xMax - 0.5f
                        && caja.yMin <= pantalla.yMin + 0.5f && caja.yMax >= pantalla.yMax - 0.5f, Is.True,
                $"cubre la pantalla: {caja} sobre {pantalla}");
            Assert.That(caja.width / caja.height, Is.EqualTo(sprite.rect.width / sprite.rect.height).Within(0.01f),
                "sin deformar");
            Assert.That(forest.Environment.raycastTarget, Is.False, "y no se traga los clics del suelo");
        }

        /// <summary>Acopia los cinco troncos y deja la caja colocada sobre ellos.</summary>
        private static async Task Colocar(ForestSceneController forest)
        {
            await Acopiar(forest);
            forest.TakeCargo();
            forest.DragCargoTo(Centro(forest.LogRow));
            forest.ReleaseCargo();
            Assume.That(forest.Cargo.IsPlaced, Is.True, "la caja quedó sobre los troncos");
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
        }

        /// <summary>Acopia los cinco troncos y espera a que vuelen a la fila y la cámara se acerque.</summary>
        private static async Task Acopiar(ForestSceneController forest)
        {
            foreach (var entrada in forest.Spawned.Where(e => e.Object.Category == ForestObjectCategory.RoundLog))
            {
                entrada.Button.onClick.Invoke();
            }

            Assume.That(forest.Selection.IsComplete, Is.True, "el acopio está completo");
            while (forest.IsTransitioning)
            {
                await Awaitable.NextFrameAsync();
            }

            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
        }

        private static Vector2 Centro(RectTransform rect) => EnPantalla(rect).center;

        /// <summary>
        /// Espera con presupuesto en **segundos y no en cuadros**: en batchmode los cuadros corren
        /// sin vsync y 900 de ellos pasan antes de que el rodado —que dura los segundos que dice el
        /// asset— termine. Lo cazó la primera corrida de RF-04, no la vista del Editor.
        /// </summary>
        private static async Task Esperar(System.Func<bool> condicion, float segundos = 20f)
        {
            var inicio = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - inicio < segundos)
            {
                if (condicion())
                {
                    return;
                }

                await Awaitable.NextFrameAsync();
            }

            Assert.Fail($"La condición no se cumplió en {segundos} s.");
        }

        /// <summary>
        /// Guarda una captura si hay Game View. En batchmode no la hay y se sigue sin ella: las
        /// aserciones de esta prueba valen igual, así que no se omite la prueba entera.
        /// </summary>
        private static bool Capturar(string nombre)
        {
            if (Application.isBatchMode)
            {
                return true;
            }

            var carpeta = $"{Application.persistentDataPath}/TestScreenshots";
            System.IO.Directory.CreateDirectory(carpeta);
            var ruta = $"{carpeta}/{nombre}.png";
            System.IO.File.Delete(ruta);

            var textura = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.File.WriteAllBytes(ruta, textura.EncodeToPNG());
            Object.Destroy(textura);
            TestContext.WriteLine($"Captura: {ruta}");
            return true;
        }

        /// <summary>Pasea el cursor por todo el suelo, que es lo que más empuja a los objetos.</summary>
        private static async Task BarrerElSuelo(ForestSceneController forest)
        {
            for (var paso = 0; paso <= 40; paso++)
            {
                var cursor = new Vector2(Screen.width * paso / 40f, Screen.height * (paso % 2) * 0.5f);
                for (var frame = 0; frame < 12; frame++)
                {
                    forest.Nudge(cursor, 1f / 60f);
                }
            }

            await Awaitable.NextFrameAsync();
        }

        /// <summary>La caja del elemento en píxeles de pantalla, que es donde se ve.</summary>
        private static Rect EnPantalla(RectTransform rect)
        {
            var esquinas = new Vector3[4];
            rect.GetWorldCorners(esquinas);

            // Envolvente por mínimos y máximos y no `esquinas[0]`..`esquinas[2]`: en cuanto un
            // objeto aparece girado esas dos dejan de ser las esquinas opuestas de la caja y la
            // diagonal sale negativa, con lo que `Overlaps` no detecta nada y la prueba pasa sola.
            var minX = Mathf.Min(esquinas[0].x, esquinas[1].x, esquinas[2].x, esquinas[3].x);
            var minY = Mathf.Min(esquinas[0].y, esquinas[1].y, esquinas[2].y, esquinas[3].y);
            var maxX = Mathf.Max(esquinas[0].x, esquinas[1].x, esquinas[2].x, esquinas[3].x);
            var maxY = Mathf.Max(esquinas[0].y, esquinas[1].y, esquinas[2].y, esquinas[3].y);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
