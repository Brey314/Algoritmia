using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// El Checkpoint R-E ejecutable: los recorridos completos del Nivel 3 desde <c>Boot</c> con
    /// perfil real (RNF-13) —uno acertando la prueba de la balsa al primer intento, otro fallándola
    /// y pasando por la escena 3.2—, el cierre forzado tras cada fase confirmada (RNF-14) y los dos
    /// presupuestos que se miden en vez de estimarse (RNF-04, RNF-05).
    /// </summary>
    /// <remarks>
    /// Las escenas narrativas se recorren pulsando su «Continuar» línea a línea, como el
    /// estudiante: no se salta ninguna (CP-07). Las medidas de carga y memoria salen del Editor:
    /// son cota superior de lo que hace el ejecutable, que carga menos.
    /// </remarks>
    [Category("Integration")]
    public class RiverLevelJourneyTests
    {
        private const string Nombre = "PruebaRecorridoRio";
        private const long DosGigas = 2L * 1024 * 1024 * 1024;

        [SetUp]
        public void SetUp() => AssemblyPanelController.ForgetLevelMemory();

        [TearDown]
        public void TearDown()
        {
            AssemblyPanelController.ForgetLevelMemory();
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                runner.Session.Delete(Nombre);
                Object.DestroyImmediate(runner.gameObject);
            }

            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }
        }

        [TestCase(false, TestName = "RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio_AcertandoAlPrimerIntento")]
        [TestCase(true, TestName = "RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio_FallandoLaPrueba")]
        [Timeout(300000)]
        [Category("Acceptance")]
        public async Task RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio(bool falla)
        {
            var (runner, perfil) = await ArrancarConPerfil();

            // Puente II → llegada → orilla (fase 1).
            Assume.That(runner.StartNarrative("N3_PuenteII"), Is.True);
            await AvanzarHasta(runner, () => runner.Flow.Current == GameState.Playing && runner.Flow.PlayingPhase == 1);
            var river = await Esperar<RiverSceneController>(r => r.Spawned.Count > 0);
            var panel = await RecogerYAbrirLaZona(river);

            // Base y amarre, bien a la primera.
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.Base);
            await AssemblyPanelTests.Confirm(panel);
            Assert.That(perfil.IsPhaseConfirmed(new PhaseId(LevelId.River, 1)), Is.True, "la base confirmó la fase 1 (RF-41)");
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.Lashing);
            await AssemblyPanelTests.Confirm(panel);
            Assert.That(perfil.IsPhaseConfirmed(new PhaseId(LevelId.River, 2)), Is.True, "el amarre confirmó la fase 2");

            if (falla)
            {
                // Mástil y vela cruzados: la balsa se hunde y, por ser el primer fallo, narra la 3.2.
                var mastil = panel.Slots.Values.Single(e => e.Slot.Accepts == MaterialKind.Mast).Image.rectTransform;
                var vela = panel.Slots.Values.Single(e => e.Slot.Accepts == MaterialKind.Cloth).Image.rectTransform;
                await AssemblyPanelTests.Drag(panel, MaterialKind.Cloth, mastil);
                await AssemblyPanelTests.Drag(panel, MaterialKind.Mast, vela);
                await AssemblyPanelTests.Confirm(panel);
                Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo(panel.Content.FirstFailureSequenceId), "el primer fallo narra la escena 3.2 (guion §8.4.1)");
                Assert.That(perfil.IsPhaseConfirmed(new PhaseId(LevelId.River, 2)), Is.True, "lo aprobado no se pierde (RF-41, RF-43)");

                // Vuelve a la fase 3 con la escena recargada y las piezas devueltas al inventario.
                await AvanzarHasta(runner, () => runner.Flow.Current == GameState.Playing && runner.Flow.PlayingPhase == 3);
                river = await Esperar<RiverSceneController>(r => r != river && r.Zone.IsOpen);
                panel = river.Assembly;
                await AssemblyPanelTests.WaitIdle(panel);
                Assert.That(panel.Assembly.ActivePhase, Is.EqualTo(RaftPhase.MastAndSail), "retoma en la fase que falló (CP-02)");
            }

            await AssemblyPanelTests.FillPhase(panel, RaftPhase.MastAndSail);
            await AssemblyPanelTests.Confirm(panel);
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative));
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo(panel.Content.ClosingSequenceId),
                falla ? "tras corregir, al cruce (RF-44)" : "acertar a la primera va directo al cruce, sin la 3.2 (guion §8.4.1)");

            // Cruce → resumen → escena final → créditos → inicio (INC-39).
            await AvanzarHasta(runner, () => runner.Flow.Current == GameState.LevelSummary, 120f);
            await Esperar(() => SceneManager.GetActiveScene().name == "LevelSummary" && ButtonWithLabel("Continuar") != null);
            await Awaitable.NextFrameAsync();
            Click(ButtonWithLabel("Continuar"));
            await Esperar(() => runner.Flow.NarrativeSequenceId == "N3_EscenaFinal" && SceneManager.GetActiveScene().name == "Narrative");
            await AvanzarHasta(runner, () => runner.Flow.Current == GameState.Credits, 120f);
            await Esperar(() => SceneManager.GetActiveScene().name == "Credits" && ButtonWithLabel("Volver") != null);
            await Awaitable.NextFrameAsync();
            Click(ButtonWithLabel("Volver"));
            await Esperar(() => runner.Flow.Current == GameState.MainMenu && SceneManager.GetActiveScene().name == "MainMenu");

            var guardado = runner.Session.Load(Nombre);
            Assert.That(guardado.IsLevelComplete(LevelId.River), Is.True, "las tres fases están en disco (RF-04, RNF-14)");
            var fase3 = guardado.IndicatorsFor(new PhaseId(LevelId.River, 3));
            Assert.That(fase3.Attempts, Is.EqualTo(falla ? 1 : 0), "intentos de la fase 3: la prueba fallida cuenta uno (RF-45, §3.6.1)");
            Assert.That(fase3.CorrectedErrors, falla ? Is.GreaterThan(0) : Is.EqualTo(0), "errores corregidos de la fase 3");
            Assert.That(guardado.IndicatorsFor(new PhaseId(LevelId.River, 1)).ResolutionSeconds, Is.GreaterThan(0f), "el tiempo de la base se midió");
        }

        [TestCase(1, TestName = "RiverLevel_RNF14_CierreForzadoTrasCadaFaseConfirmadaRetomaDondeIba_TrasLaBase")]
        [TestCase(2, TestName = "RiverLevel_RNF14_CierreForzadoTrasCadaFaseConfirmadaRetomaDondeIba_TrasElAmarre")]
        [Timeout(120000)]
        [Category("Acceptance")]
        public async Task RiverLevel_RNF14_CierreForzadoTrasCadaFaseConfirmadaRetomaDondeIba(int confirmadas)
        {
            // Lo que queda en disco al cerrar el juego tras confirmar `confirmadas` fases.
            var (runner, perfil) = await ArrancarConPerfil();
            for (var fase = 1; fase <= confirmadas; fase++)
            {
                perfil.ConfirmPhase(new PhaseId(LevelId.River, fase), new PerformanceIndicators(0, 0, 1, 5f));
            }

            runner.Session.SaveActive();

            // Cierre forzado: el proceso muere y el juego arranca de cero desde Boot.
            Object.DestroyImmediate(runner.gameObject);
            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }

            SceneManager.LoadScene("Boot");
            await Esperar(() => GameFlowRunner.Instance != null && GameFlowRunner.Instance != runner
                                && SceneManager.GetActiveScene().name == "MainMenu");
            runner = GameFlowRunner.Instance;
            var recuperado = runner.Session.Load(Nombre);
            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(recuperado);

            // La ficha del nivel siempre pide la fase 1: el flujo retoma en la primera pendiente.
            Assume.That(runner.StartPlaying(LevelId.River, 1), Is.True);
            Assert.That(runner.Flow.PlayingPhase, Is.EqualTo(confirmadas + 1), "retoma en la primera fase pendiente (RNF-14)");
            var river = await Esperar<RiverSceneController>(r => r.Zone.IsOpen);
            var panel = river.Assembly;
            await AssemblyPanelTests.WaitIdle(panel);

            Assert.That(panel.Assembly.ActivePhase, Is.EqualTo((RaftPhase)(confirmadas + 1)), "el panel abre en la fase pendiente");
            for (var fase = 1; fase <= confirmadas; fase++)
            {
                Assert.That(panel.Assembly.IsConfirmed((RaftPhase)fase), Is.True, $"la fase {fase} sigue consolidada (RF-41)");
            }

            Assert.That(river.Tasks.IsDone(RiverTaskId.CollectLogs) && river.Tasks.IsDone(RiverTaskId.FindRopes), Is.True,
                "la recolección no se repite: no se persiste, se da por hecha al retomar");
        }

        [Test]
        [Timeout(60000)]
        [Category("Acceptance")]
        public async Task RiverLevel_RNF04_LaEscenaDelNivel3CargaEnMenosDeDiezSegundos()
        {
            var (runner, _) = await ArrancarConPerfil();

            Assume.That(runner.StartPlaying(LevelId.River, 1), Is.True);
            await Esperar(() => SceneManager.GetActiveScene().name == "Level3_River" && SceneLoader.Instance.LastLoadSeconds > 0f);

            TestContext.Out.WriteLine($"RNF-04: Level3_River cargó en {SceneLoader.Instance.LastLoadSeconds:0.000} s (Editor)");
            Assert.That(SceneLoader.Instance.LastLoadSeconds, Is.LessThan(10f),
                $"Level3_River cargó en {SceneLoader.Instance.LastLoadSeconds:0.00} s (RNF-04, medido en el Editor)");
        }

        [Test]
        [Timeout(60000)]
        [Category("Acceptance")]
        public async Task RiverLevel_RNF05_LaMemoriaQuedaBajoDosGigasConElNivel3Cargado()
        {
            var (runner, _) = await ArrancarConPerfil();
            Assume.That(runner.StartPlaying(LevelId.River, 1), Is.True);
            var river = await Esperar<RiverSceneController>(r => r.Spawned.Count > 0);
            await RecogerYAbrirLaZona(river); // Con el panel abierto está todo el arte del nivel en memoria.

            var reservada = Profiler.GetTotalReservedMemoryLong();
            TestContext.Out.WriteLine($"RNF-05: memoria reservada con el Nivel 3 cargado: {reservada / (1024f * 1024f):0} MB, asignada {Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f):0} MB (Editor)");
            Assert.That(reservada, Is.LessThan(DosGigas),
                $"memoria reservada con el Nivel 3 cargado: {reservada / (1024f * 1024f):0} MB (RNF-05, medida en el Editor)");
        }

        // --- utilería ---------------------------------------------------------------------------

        private static async Task<(GameFlowRunner Runner, PlayerProfile Perfil)> ArrancarConPerfil()
        {
            SceneManager.LoadScene("Boot");
            await Esperar(() => GameFlowRunner.Instance != null && SceneManager.GetActiveScene().name == "MainMenu");
            var runner = GameFlowRunner.Instance;
            runner.Session.Delete(Nombre);
            runner.GoTo(GameState.ProfileSelect);
            var perfil = PlayerProfile.Create(Nombre, Array.Empty<string>()).Profile;
            perfil.Reach(LevelId.River);
            runner.SelectProfile(perfil);
            return (runner, perfil);
        }

        /// <summary>Recoge los cuatro materiales caminando hasta cada uno y entra a la zona de construcción.</summary>
        private static async Task<AssemblyPanelController> RecogerYAbrirLaZona(RiverSceneController river)
        {
            foreach (var (collectible, _) in river.Spawned.ToArray())
            {
                CaminarHasta(river, collectible.Position);
                river.CollectButton.onClick.Invoke();
            }

            CaminarHasta(river, river.Config.BuildZonePosition);
            Assert.That(river.Zone.IsOpen, Is.True, "con los cuatro materiales la zona abre el panel (RF-39)");
            var panel = river.Assembly;
            await AssemblyPanelTests.WaitIdle(panel);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            return panel;
        }

        private static void CaminarHasta(RiverSceneController river, Vector2 destino)
        {
            for (var paso = 0; paso < 2000 && !river.Zone.IsOpen && Vector2.Distance(river.Walk.Position, destino) > 0.005f; paso++)
            {
                river.Tick(destino - river.Walk.Position, 0.02f);
            }
        }

        private static async Task AvanzarHasta(GameFlowRunner runner, Func<bool> destino, float segundos = 60f)
        {
            var limite = Time.realtimeSinceStartup + segundos;
            while (!destino())
            {
                if (Time.realtimeSinceStartup >= limite)
                {
                    Assert.Fail($"El flujo no llegó a donde se esperaba en {segundos} s: está en {runner.Flow.Current} / {runner.Flow.NarrativeSequenceId}.");
                }

                if (runner.Flow.Current == GameState.Narrative && ButtonWithLabel("Continuar") is { } continuar && continuar.isActiveAndEnabled)
                {
                    Click(continuar);
                }

                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
        }

        private static Button ButtonWithLabel(string label) => Array.Find(
            Object.FindObjectsByType<Button>(),
            b => b.GetComponentInChildren<Text>(true) is { } text && text.text.Trim() == label);

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        private static async Task<T> Esperar<T>(Func<T, bool> condicion, float segundos = 30f) where T : Object
        {
            T encontrado = null;
            await Esperar(() => (encontrado = Object.FindObjectsByType<T>().FirstOrDefault(condicion)) != null, segundos);
            Canvas.ForceUpdateCanvases();
            await Awaitable.NextFrameAsync();
            return encontrado;
        }

        private static async Task Esperar(Func<bool> condicion, float segundos = 30f)
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
    }
}
