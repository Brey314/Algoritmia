using System;
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
    /// <summary>
    /// El menú de pausa (prefab <c>MenuPausa</c>, mockup 6) sobre las tres escenas del Nivel 2
    /// (W17, HU-17): es donde «Reiniciar» se vuelve ambiguo y tiene que reiniciar **la fase
    /// activa**, nunca las ya confirmadas.
    /// </summary>
    /// <remarks>
    /// No referencia <c>Game.UI</c>: el menú se conduce por estructura —botones por etiqueta,
    /// paneles por nombre—, igual que <c>PauseMenuTests</c> conduce el panel del Nivel 1.
    /// </remarks>
    [Category("Integration")]
    public class PauseMenuWheelTests
    {
        [TearDown]
        public void RestituirElTiempo() => Time.timeScale = 1f;

        [Test]
        [Timeout(30000)]
        [Category("Acceptance")]
        public async Task PauseMenu_HU17_ContinuarRestituyeLaSecuenciaDeBloquesAMedioComponer()
        {
            var maze = await OpenScene<MazeSceneController>("Level2_Maze", m => m.Rows != null);
            maze.AddBlock(InstructionBlock.Forward(3));
            maze.AddBlock(InstructionBlock.Turn());
            await Awaitable.NextFrameAsync();
            var filasAntes = maze.Rows.Count;

            Click(PauseButton());
            await Awaitable.NextFrameAsync();
            Assert.That(Overlay().activeSelf, Is.True, "la pausa no se abrió");
            Assert.That(Time.timeScale, Is.Zero, "con la pausa abierta el nivel queda detenido (HU-17)");

            Click(ButtonWithLabel("Reanudar"));
            await Awaitable.NextFrameAsync();

            Assert.That(Overlay().activeSelf, Is.False, "«Reanudar» no cerró la pausa");
            Assert.That(Time.timeScale, Is.EqualTo(1f), "«Reanudar» devuelve el tiempo");
            Assert.That(maze.Sequence.Count, Is.EqualTo(2), "la secuencia a medio componer sigue ahí (RF-07)");
            Assert.That(maze.Rows.Count, Is.EqualTo(filasAntes), "y sigue pintada igual");
            Assert.That(maze.IsExecuting, Is.False, "continuar no ejecuta nada");
        }

        [Test]
        [Timeout(60000)]
        [Category("Acceptance")]
        public async Task PauseMenu_INC25_ReiniciarNivelNoDescartaLasFasesYaConfirmadas()
        {
            const string nombre = "W17_Prueba";
            GameFlowRunner runner = null;

            try
            {
                var (perfil, arrancado) = await ArrancarDesdeBoot(nombre);
                runner = arrancado;
                var fase1 = new PhaseId(LevelId.Wheel, 1);
                perfil.ConfirmPhase(fase1, new PerformanceIndicators(2, 1, 0, 40f));
                runner.StartPlaying(LevelId.Wheel, 2);
                var workshop = await Esperar<WorkshopSceneController>(w => w.Pieces.Count > 0);
                workshop.Select(WorkshopPiece.ShortLogA);
                workshop.MachineButton.onClick.Invoke();
                Assume.That(workshop.Assembly.DrilledWheels, Is.EqualTo(1), "hay una rueda a medias en la fase activa");

                Click(PauseButton());
                await Awaitable.NextFrameAsync();
                Click(ButtonWithLabel("Reiniciar"));
                await Awaitable.NextFrameAsync();
                Assert.That(GameObject.Find("PanelConfirmarReinicio"), Is.Not.Null, "«Reiniciar» pide confirmación (HU-17 FA-02)");
                Click(ButtonWithLabel("Sí, reiniciar"));

                var recargado = await Esperar<WorkshopSceneController>(w => w != workshop && w.Pieces.Count > 0);

                Assert.That(recargado.Assembly.DrilledWheels, Is.Zero, "la fase activa vuelve a empezar desde cero (RF-07)");
                Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing));
                Assert.That(runner.Flow.PlayingPhase, Is.EqualTo(2), "se reinicia la fase activa, no el nivel entero (INC-25)");
                Assert.That(perfil.IsPhaseConfirmed(fase1), Is.True, "la fase 1 confirmada no se descarta (RF-41)");
                Assert.That(perfil.IndicatorsFor(fase1), Is.EqualTo(new PerformanceIndicators(2, 1, 0, 40f)),
                    "ni sus indicadores registrados (OE1 §3.6.1 nota 4)");
                Assert.That(perfil.IsUnlocked(LevelId.Wheel), Is.True, "«Reiniciar» no re-bloquea el nivel (RF-03)");
                Assert.That(Time.timeScale, Is.EqualTo(1f), "la recarga devuelve el tiempo");
            }
            finally
            {
                runner?.Session.Delete(nombre);
                LimpiarPersistentes();
            }
        }

        [Test]
        [Timeout(60000)]
        [Category("Acceptance")]
        public async Task PauseMenu_CP02_NingunaRutaDeLaPausaLlevaAUnaPantallaDeDerrota()
        {
            const string nombre = "W17_Prueba_Salida";
            GameFlowRunner runner = null;

            try
            {
                var (perfil, arrancado) = await ArrancarDesdeBoot(nombre);
                runner = arrancado;
                runner.StartPlaying(LevelId.Wheel, 1);
                var forest = await Esperar<ForestSceneController>(f => f.Spawned.Count > 0);
                Assert.That(runner.ActiveReporter, Is.InstanceOf<WheelIndicatorCollector>(),
                    "la escena registra su recolector para que la pausa no sume tiempo (RF-07, W15)");

                // Un rechazo antes de salir: tampoco él abre ninguna ruta de derrota.
                var distractor = forest.Spawned.First(e => e.Object.Category != ForestObjectCategory.RoundLog);
                distractor.Button.onClick.Invoke();

                Click(PauseButton());
                await Awaitable.NextFrameAsync();
                Click(ButtonWithLabel("Volver al menú de niveles"));
                await Esperar(() => runner.Flow.Current == GameState.LevelSelect);

                Assert.That(Time.timeScale, Is.EqualTo(1f));
                Assert.That(perfil.IsUnlocked(LevelId.Wheel), Is.True, "salir conserva el progreso (RF-41)");
                Assert.That(Enum.GetNames(typeof(GameState)),
                    Has.None.Matches<string>(n => n.Contains("Over") || n.Contains("Defeat") || n.Contains("Derrota")),
                    "no existe pantalla de derrota (CP-02)");
            }
            finally
            {
                runner?.Session.Delete(nombre);
                LimpiarPersistentes();
            }
        }

        // --- helpers -----------------------------------------------------------------------

        private static Button PauseButton() => GameObject.Find("BotonPausa")?.GetComponent<Button>();

        private static GameObject Overlay() => Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include)
            .First(r => r.name == "PanelPausa").gameObject;

        private static Button ButtonWithLabel(string label) => Array.Find(
            Object.FindObjectsByType<Button>(FindObjectsInactive.Include),
            b => b.GetComponentInChildren<Text>(true) is { } text && text.text.Trim() == label);

        private static void Click(Button button)
        {
            Assert.That(button, Is.Not.Null, "no se encontró el botón que se quería pulsar");
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);
        }

        private static async Task<T> OpenScene<T>(string sceneName, Func<T, bool> ready) where T : Object
        {
            var carga = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!carga.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
            return await Esperar(ready);
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
            runner.SelectProfile(perfil);
            return (perfil, runner);
        }

        private static async Task<T> Esperar<T>(Func<T, bool> condicion, float segundos = 20f) where T : Object
        {
            T encontrado = null;
            await Esperar(() =>
            {
                encontrado = Object.FindObjectsByType<T>().FirstOrDefault(condicion);
                return encontrado != null;
            }, segundos);
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
    }
}
