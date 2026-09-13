using System;
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
    public class WorkshopSceneTests
    {
        private const string SceneName = "Level2_Workshop";

        [Test]
        [Timeout(30000)]
        public async Task WorkshopScene_RF27_PresentaLasSeisPiezas()
        {
            var workshop = await OpenWorkshop();
            var esperadas = Enum.GetValues(typeof(WorkshopPiece)).Cast<WorkshopPiece>().ToArray();

            Assert.That(workshop.Pieces.Keys, Is.EquivalentTo(esperadas),
                "dos troncos cortos, un tronco largo, una tabla, una herramienta y la caja (RF-27)");
            Assert.That(workshop.Pieces.Count, Is.EqualTo(workshop.Config.Pieces.Length),
                "las piezas se reparten del asset, no de objetos puestos a mano (RNF-18)");

            foreach (var (pieza, entrada) in workshop.Pieces)
            {
                Assert.That(entrada.Rect.name, Is.EqualTo($"Pieza_{pieza}"), "cada pieza es señalable por su nombre");
                Assert.That(entrada.Rect.gameObject.activeSelf, Is.True, $"{pieza} está en el suelo");
                Assert.That(entrada.Image.sprite, Is.Not.Null, $"{pieza} tiene ilustración (RNF-23)");
                Assert.That(entrada.Rect.parent, Is.SameAs(workshop.Environment.rectTransform),
                    $"{pieza} cuelga del entorno: acompaña el encuadre y cualquier resolución del arte");
            }

            Assert.That(workshop.Pieces[WorkshopPiece.ShortLogA].Image.sprite,
                Is.SameAs(workshop.Pieces[WorkshopPiece.ShortLogB].Image.sprite),
                "los dos troncos cortos son gemelos: es correcto y deliberado (B5)");
            Assert.That(workshop.AssemblyImage.enabled, Is.False, "la carretilla todavía no existe");
        }

        [Test]
        [Timeout(30000)]
        public async Task WorkshopScene_RNF23_ElEntornoEsElDelBosqueYCubreLaPantallaConElEncuadreDeLa23()
        {
            var workshop = await OpenWorkshop();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var sprite = workshop.Environment.sprite;
            Assert.That(sprite, Is.Not.Null, "el taller tiene entorno");
            Assert.That(sprite.name, Is.EqualTo("env_n2_bosque_claro"),
                "es el mismo entorno duplicado del Nivel 2: el taller es el claro este de la ilustración");

            // Camara_Narrativa_N2.md §5.6: plano fijo (0.772, 0.470) a zoom 1.32, idéntico al último
            // encuadre de la 2.3 — de la narrativa al juego no hay salto.
            Assert.That(workshop.Config.PlayFraming.Focus.x, Is.EqualTo(0.772f).Within(0.001f));
            Assert.That(workshop.Config.PlayFraming.Focus.y, Is.EqualTo(0.47f).Within(0.001f));
            Assert.That(workshop.Config.PlayFraming.Zoom, Is.EqualTo(1.32f).Within(0.001f));

            // Cubre —ningún borde de la ventana queda sin pintar— y no deforma: recorta lo que
            // sobre. Vale a la resolución del arte de hoy y a la definitiva: solo cambia el archivo.
            var caja = EnPantalla(workshop.Environment.rectTransform);
            var pantalla = new Rect(0f, 0f, Screen.width, Screen.height);
            Assert.That(caja.xMin, Is.LessThanOrEqualTo(pantalla.xMin + 0.5f), "cubre por la izquierda");
            Assert.That(caja.yMin, Is.LessThanOrEqualTo(pantalla.yMin + 0.5f), "cubre por abajo");
            Assert.That(caja.xMax, Is.GreaterThanOrEqualTo(pantalla.xMax - 0.5f), "cubre por la derecha");
            Assert.That(caja.yMax, Is.GreaterThanOrEqualTo(pantalla.yMax - 0.5f), "cubre por arriba");
            Assert.That(caja.width / caja.height, Is.EqualTo(sprite.rect.width / sprite.rect.height).Within(0.01f),
                "y conserva la proporción del dibujo");
        }

        [Test]
        [Timeout(30000)]
        public async Task WorkshopScene_RF28_MecanizarSeHabilitaSoloConTroncoSeleccionadoYConDobleIndicador()
        {
            var workshop = await OpenWorkshop();

            Assert.That(workshop.MachineButton.interactable, Is.False, "sin tronco resaltado no se mecaniza");
            Assert.That(workshop.MachineLockedBadge.activeSelf, Is.True,
                "y la tablilla «Aún no» con candado lo dice sin depender del color (RNF-19)");

            var tronco = workshop.Pieces[WorkshopPiece.ShortLogA];
            var macizo = tronco.Image.sprite;
            workshop.Select(WorkshopPiece.ShortLogA);

            Assert.That(workshop.MachineButton.interactable, Is.True);
            Assert.That(workshop.MachineLockedBadge.activeSelf, Is.False);
            Assert.That(tronco.Outline.enabled, Is.True, "el tronco resaltado lleva contorno");
            Assert.That(tronco.Rect.localScale.x, Is.GreaterThan(1f), "y crece: dos indicadores, ninguno solo color");

            workshop.MachineButton.onClick.Invoke();

            Assert.That(workshop.Assembly.IsDrilled(WorkshopPiece.ShortLogA), Is.True);
            Assert.That(tronco.Image.sprite, Is.Not.SameAs(macizo), "el tronco muestra su agujero: es una rueda");
            Assert.That(tronco.Image.sprite, Is.SameAs(workshop.Config.DrilledWheelArt));
            Assert.That(tronco.Rect.gameObject.activeSelf, Is.True, "y sigue en el suelo (CP-02)");
            Assert.That(workshop.MachineButton.interactable, Is.False, "la selección se consume");
            Assert.That(workshop.MachineLockedBadge.activeSelf, Is.True);
            Assert.That(workshop.MessageIcon.sprite, Is.Not.Null, "el acierto trae forma");
        }

        [Test]
        [Timeout(30000)]
        public async Task WorkshopScene_RF29_UnPasoFueraDeOrdenDevuelveLaPiezaYNoDeshaceNada()
        {
            var workshop = await OpenWorkshop();
            workshop.Select(WorkshopPiece.ShortLogA);
            workshop.MachineButton.onClick.Invoke();
            var tabla = workshop.Pieces[WorkshopPiece.Plank];

            // La tabla sobre los troncos, sin eje: el intento fuera de orden del guion.
            await Arrastrar(workshop, WorkshopPiece.Plank, workshop.Pieces[WorkshopPiece.ShortLogB].Rect);

            Assert.That(workshop.Assembly.IsPlankPlaced, Is.False, "la tabla no se coloca sin eje");
            Assert.That(tabla.Rect.gameObject.activeSelf, Is.True, "vuelve al suelo");
            Assert.That(tabla.Rect.anchoredPosition, Is.EqualTo(Vector2.zero), "a su sitio de origen");
            Assert.That(workshop.MessageLabel.text, Is.EqualTo(workshop.Config.PlankTooEarlyMessage),
                "con el mensaje del guion que dice qué falta antes (CP-06)");
            Assert.That(workshop.MessageIcon.sprite, Is.Not.Null, "el rechazo trae forma (RNF-19)");
            Assert.That(workshop.Assembly.IsDrilled(WorkshopPiece.ShortLogA), Is.True,
                "la rueda ya perforada no se pierde (CP-02)");

            // El eje sobre un tronco sin perforar: el otro intento fuera de orden del guion.
            await Arrastrar(workshop, WorkshopPiece.LongLog, workshop.Pieces[WorkshopPiece.ShortLogB].Rect);
            Assert.That(workshop.Assembly.IsAxleFormed, Is.False);
            Assert.That(workshop.MessageLabel.text, Is.EqualTo(workshop.Config.AxleTooEarlyMessage));
        }

        [Test]
        [Timeout(30000)]
        public async Task WorkshopScene_RF29_LaCarretillaCreceSobreLoAnteriorSinQueLaInterfazLaTape()
        {
            var workshop = await OpenWorkshop();
            workshop.Select(WorkshopPiece.ShortLogA);
            workshop.MachineButton.onClick.Invoke();
            workshop.Select(WorkshopPiece.ShortLogB);
            workshop.MachineButton.onClick.Invoke();

            await Arrastrar(workshop, WorkshopPiece.LongLog, workshop.Pieces[WorkshopPiece.ShortLogA].Rect);
            Assert.That(workshop.Assembly.IsAxleFormed, Is.True);
            Assert.That(workshop.AssemblyImage.enabled, Is.True, "aparece el conjunto");
            Assert.That(workshop.AssemblyImage.sprite, Is.SameAs(workshop.Config.AxleArt));
            Assert.That(workshop.Pieces[WorkshopPiece.ShortLogA].Rect.gameObject.activeSelf, Is.False,
                "las ruedas dejan el suelo: ya son parte del conjunto");
            Assert.That(workshop.Pieces[WorkshopPiece.LongLog].Rect.gameObject.activeSelf, Is.False);

            await Arrastrar(workshop, WorkshopPiece.Plank, workshop.AssemblyImage.rectTransform);
            Assert.That(workshop.AssemblyImage.sprite, Is.SameAs(workshop.Config.PlankArt));

            // La carretilla se ve entera: dentro de la pantalla y sin ninguna tablilla ni botón
            // encima —lo que sí pasa en la 2.2 con el cuadro de diálogo sobre los props.
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            var carretilla = EnPantalla(workshop.AssemblyImage.rectTransform);
            var pantalla = new Rect(0f, 0f, Screen.width, Screen.height);
            Assert.That(carretilla.xMin, Is.GreaterThanOrEqualTo(pantalla.xMin) & Is.LessThan(pantalla.xMax), "la carretilla entra por la izquierda");
            Assert.That(carretilla.yMin, Is.GreaterThanOrEqualTo(pantalla.yMin), "y por abajo");
            Assert.That(carretilla.xMax, Is.LessThanOrEqualTo(pantalla.xMax), "y por la derecha");
            Assert.That(carretilla.yMax, Is.LessThanOrEqualTo(pantalla.yMax), "y por arriba");
            foreach (var elemento in Interfaz(workshop))
            {
                Assert.That(carretilla.Overlaps(EnPantalla(elemento)), Is.False,
                    $"{elemento.name} no tapa la carretilla");
            }

            await Arrastrar(workshop, WorkshopPiece.Cargo, workshop.AssemblyImage.rectTransform);
            Assert.That(workshop.AssemblyImage.sprite, Is.SameAs(workshop.Config.CompleteArt));
            Assert.That(workshop.Assembly.IsComplete, Is.True);
            Assert.That(workshop.IsCompleting, Is.True, "y arranca el empuje de cámara de cierre");
        }

        [Test]
        [Timeout(30000)]
        public async Task WorkshopScene_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido()
        {
            await OpenWorkshop();
            var interactivos = Object.FindObjectsByType<Selectable>(FindObjectsInactive.Include);

            Assert.That(interactivos, Is.Not.Empty);
            Assert.That(interactivos.Where(elemento => !(elemento is Button)).Select(elemento => elemento.name),
                Is.Empty, "todo lo interactivo es un botón: sin deslizadores, barras ni campos");

            // El eje, la tabla y la caja se agarran con el clic sostenido y se sueltan al soltar
            // el clic: pulsar y soltar, el esquema de RNF-02, y no el arrastre de uGUI (CT-06).
            var comportamientos = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include);
            var arrastrables = comportamientos
                .Where(comportamiento => comportamiento is IDragHandler || comportamiento is IBeginDragHandler)
                .Select(comportamiento => comportamiento.name)
                .ToArray();
            // La pieza modelo está inactiva y nunca recibe un evento: se clona y se le quita lo
            // que cada pieza no usa. Solo cuentan los objetos activos; los botones también
            // atienden el pulsar (se hunden), pero eso es un botón, no un arrastre.
            var sostenidos = comportamientos
                .Where(comportamiento => comportamiento is IPointerDownHandler && !(comportamiento is Selectable)
                                         && comportamiento.GetComponent<Button>() == null
                                         && comportamiento.gameObject.activeInHierarchy)
                .Select(comportamiento => comportamiento.name)
                .OrderBy(nombre => nombre)
                .ToArray();

            Assert.That(arrastrables, Is.Empty, "ningún elemento del taller usa el arrastre de uGUI");
            Assert.That(sostenidos, Is.EqualTo(new[] { "Pieza_Cargo", "Pieza_LongLog", "Pieza_Plank" }),
                "lo único que responde al clic sostenido son las tres piezas que se colocan");
        }

        [Test]
        [Timeout(30000)]
        public async Task WorkshopScene_RNF03_NadaSeSaleDePantallaNiSeSolapa()
        {
            var workshop = await OpenWorkshop();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();

            var pantalla = new Rect(0f, 0f, Screen.width, Screen.height);
            var elementos = workshop.Pieces.Values.Select(entrada => entrada.Rect).Concat(Interfaz(workshop)).ToArray();

            foreach (var elemento in elementos)
            {
                var caja = EnPantalla(elemento);
                Assert.That(caja.xMin, Is.GreaterThanOrEqualTo(pantalla.xMin - 0.5f), $"{elemento.name} entra por la izquierda");
                Assert.That(caja.yMin, Is.GreaterThanOrEqualTo(pantalla.yMin - 0.5f), $"{elemento.name} entra por abajo");
                Assert.That(caja.xMax, Is.LessThanOrEqualTo(pantalla.xMax + 0.5f), $"{elemento.name} entra por la derecha");
                Assert.That(caja.yMax, Is.LessThanOrEqualTo(pantalla.yMax + 0.5f), $"{elemento.name} entra por arriba");
            }

            for (var i = 0; i < elementos.Length; i++)
            {
                for (var j = i + 1; j < elementos.Length; j++)
                {
                    Assert.That(EnPantalla(elementos[i]).Overlaps(EnPantalla(elementos[j])), Is.False,
                        $"{elementos[i].name} y {elementos[j].name} no se solapan: una pieza tapada no recibe el clic, " +
                        "y una pieza en reposo sobre otra se colocaría sola");
                }
            }

            var escalador = Object.FindAnyObjectByType<CanvasScaler>();
            Assert.That(escalador.uiScaleMode, Is.EqualTo(CanvasScaler.ScaleMode.ScaleWithScreenSize));
            Assert.That(escalador.referenceResolution, Is.EqualTo(new Vector2(1920f, 1080f)));
        }

        [Test]
        [Timeout(60000)]
        public async Task WorkshopScene_RF29_LaSecuenciaCompletaConfirmaYGuardaLaFase2()
        {
            const string nombre = "W09_Prueba";
            GameFlowRunner runner = null;

            try
            {
                var (perfil, arrancado) = await ArrancarDesdeBoot(nombre, faseUnoConfirmada: false);
                runner = arrancado;
                runner.StartPlaying(LevelId.Wheel, 2);
                var workshop = await EsperarTaller();
                var fase2 = new PhaseId(LevelId.Wheel, 2);
                Assume.That(perfil.IsPhaseConfirmed(fase2), Is.False);

                workshop.Select(WorkshopPiece.ShortLogA);
                workshop.MachineButton.onClick.Invoke();
                workshop.Select(WorkshopPiece.ShortLogB);
                workshop.MachineButton.onClick.Invoke();
                await Arrastrar(workshop, WorkshopPiece.LongLog, workshop.Pieces[WorkshopPiece.ShortLogA].Rect);
                await Arrastrar(workshop, WorkshopPiece.Plank, workshop.AssemblyImage.rectTransform);
                await Arrastrar(workshop, WorkshopPiece.Cargo, workshop.AssemblyImage.rectTransform);
                await Esperar(() => runner.Flow.Current != GameState.Playing);

                Assert.That(perfil.IsPhaseConfirmed(fase2), Is.True, "la carretilla terminada confirma la fase 2 (RF-04)");
                Assert.That(runner.Session.Load(nombre).IsPhaseConfirmed(fase2), Is.True,
                    "y queda en disco: un cierre forzado retoma en la fase 3 (RNF-14)");
                Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative),
                    "el taller sale a la escena narrativa que declara el asset");
                Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo(workshop.Config.ClosingSequenceId));
            }
            finally
            {
                runner?.Session.Delete(nombre);
                LimpiarPersistentes();
            }
        }

        [Test]
        [Timeout(60000)]
        public async Task WorkshopScene_RNF14_ConLaFase1EnDiscoEntrarAlNivelRetomaEnElTaller()
        {
            const string nombre = "W09_Retoma";
            GameFlowRunner runner = null;

            try
            {
                var (_, arrancado) = await ArrancarDesdeBoot(nombre, faseUnoConfirmada: true);
                runner = arrancado;

                // La apertura del Nivel 2 pide la fase 1 (es lo que declara la escena 2.1); con la
                // fase 1 confirmada se retoma en la 2, en el taller y no en el bosque.
                runner.StartPlaying(LevelId.Wheel, 1);
                await Esperar(() => SceneManager.GetActiveScene().name == SceneName);

                Assert.That(runner.Flow.PlayingPhase, Is.EqualTo(2));
                Assert.That(await EsperarTaller(), Is.Not.Null, "el taller se abre y reparte sus piezas");
            }
            finally
            {
                runner?.Session.Delete(nombre);
                LimpiarPersistentes();
            }
        }

        [Test]
        [Timeout(30000)]
        [Category("VisualVerification")]
        [Description("Verificar en las capturas: el taller es el claro este del bosque con el encuadre de la 2.3; " +
                     "las piezas se leen sobre el pasto sin que la tablilla ni los botones las tapen; la carretilla crece " +
                     "donde estaban las ruedas; «Aún no» con candado sobre «Mecanizar»; contraste carbón sobre marfil.")]
        public async Task WorkshopScene_RNF20_CapturaDelTallerEnReposoYConLaCarretillaMontada()
        {
            var workshop = await OpenWorkshop();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            Capturar("Workshop_01_reposo");

            workshop.Select(WorkshopPiece.ShortLogA);
            await Awaitable.NextFrameAsync();
            Capturar("Workshop_02_tronco_resaltado");

            workshop.MachineButton.onClick.Invoke();
            workshop.Select(WorkshopPiece.ShortLogB);
            workshop.MachineButton.onClick.Invoke();
            await Arrastrar(workshop, WorkshopPiece.LongLog, workshop.Pieces[WorkshopPiece.ShortLogA].Rect);
            await Arrastrar(workshop, WorkshopPiece.Plank, workshop.AssemblyImage.rectTransform);
            await Awaitable.NextFrameAsync();
            Capturar("Workshop_03_tabla_montada");

            Assert.That(workshop.AssemblyImage.enabled, Is.True);
        }

        // --- helpers ---------------------------------------------------------------------------

        /// <summary>
        /// Guarda una captura si hay Game View. En batchmode no la hay y se sigue sin ella.
        /// </summary>
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

        /// <summary>Las tablillas y botones: lo que nunca debe tapar una pieza ni la carretilla.</summary>
        private static IEnumerable<RectTransform> Interfaz(WorkshopSceneController workshop) => new[]
        {
            (RectTransform)workshop.MachineButton.transform,
            (RectTransform)workshop.HelpButton.transform,
            (RectTransform)workshop.MessageLabel.transform.parent.parent
        };

        private static async Task<WorkshopSceneController> OpenWorkshop()
        {
            var carga = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (!carga.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            // Dos cuadros: `Start` corre después de que la carga se declare terminada, y los
            // componentes que cada pieza descarta se destruyen al final de ese cuadro.
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            var workshop = Object.FindAnyObjectByType<WorkshopSceneController>();
            Assert.That(workshop, Is.Not.Null, $"la escena «{SceneName}» trae su controlador");
            Assert.That(workshop.Pieces, Is.Not.Empty, "el taller ya se repartió");
            return workshop;
        }

        private static async Task<WorkshopSceneController> EsperarTaller()
        {
            await Esperar(() => Object.FindAnyObjectByType<WorkshopSceneController>()?.Pieces.Count > 0);
            await Awaitable.NextFrameAsync();
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            return Object.FindAnyObjectByType<WorkshopSceneController>();
        }

        /// <summary>Arranca desde Boot con un perfil real, como lo haría el ejecutable.</summary>
        private static async Task<(PlayerProfile Profile, GameFlowRunner Runner)> ArrancarDesdeBoot(
            string nombre, bool faseUnoConfirmada)
        {
            SceneManager.LoadScene("Boot");
            await Esperar(() => GameFlowRunner.Instance != null
                                && SceneManager.GetActiveScene().name == "MainMenu");
            var runner = GameFlowRunner.Instance;
            runner.Session.Delete(nombre);

            runner.GoTo(GameState.ProfileSelect);
            var perfil = PlayerProfile.Create(nombre, Array.Empty<string>()).Profile;
            perfil.Reach(LevelId.Wheel);
            if (faseUnoConfirmada)
            {
                perfil.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), default);
            }

            runner.SelectProfile(perfil);
            return (perfil, runner);
        }

        /// <summary>Clic sostenido sobre la pieza, arrastre hasta el centro del objetivo y soltar.</summary>
        private static async Task Arrastrar(WorkshopSceneController workshop, WorkshopPiece pieza, RectTransform objetivo)
        {
            Canvas.ForceUpdateCanvases();
            workshop.Take(pieza);
            workshop.DragTo(EnPantalla(objetivo).center);
            // Sin cuadro entre mover y soltar: en el Editor hay un ratón real y `Update` seguiría
            // al cursor de verdad, que está fuera de la ventana de juego (visto el 12/09/2026).
            workshop.Release(pieza);
            await Awaitable.NextFrameAsync();
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

        /// <summary>La caja del elemento en píxeles de pantalla, envuelta por mínimos y máximos de sus esquinas.</summary>
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
