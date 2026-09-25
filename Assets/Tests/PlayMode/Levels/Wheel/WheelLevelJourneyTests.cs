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
    /// El recorrido completo del Nivel 2 desde <c>Boot</c> con perfil real (RNF-13, Checkpoint
    /// W-F): puente → bosque → patrón → taller → regreso → laberinto → cierre → menú con el
    /// Nivel 3 desbloqueado. Se corre dos veces porque el checkpoint pide dos recorridos.
    /// </summary>
    /// <remarks>
    /// Las escenas narrativas se recorren pulsando su «Continuar» línea a línea, como el
    /// estudiante: no se salta ninguna (CP-07).
    /// </remarks>
    [Category("Integration")]
    public class WheelLevelJourneyTests
    {
        [TestCase(1, TestName = "WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado_PrimerRecorrido")]
        [TestCase(2, TestName = "WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado_SegundoRecorrido")]
        [Timeout(300000)]
        [Category("Acceptance")]
        public async Task WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado(int recorrido)
        {
            var nombre = $"WF_Recorrido_{recorrido}";
            GameFlowRunner runner = null;

            try
            {
                SceneManager.LoadScene("Boot");
                await Esperar(() => GameFlowRunner.Instance != null && SceneManager.GetActiveScene().name == "MainMenu");
                runner = GameFlowRunner.Instance;
                runner.Session.Delete(nombre);
                runner.GoTo(GameState.ProfileSelect);
                var perfil = PlayerProfile.Create(nombre, Array.Empty<string>()).Profile;
                perfil.Reach(LevelId.Wheel);
                runner.SelectProfile(perfil);

                // Puente → bosque (fase 1).
                Assume.That(runner.StartNarrative("N2_PuenteI"), Is.True);
                await AvanzarHasta(runner, () => runner.Flow.Current == GameState.Playing && runner.Flow.PlayingPhase == 1);
                var forest = await Esperar<ForestSceneController>(f => f.Spawned.Count > 0);
                foreach (var entrada in forest.Spawned.Where(e => e.Object.Category == ForestObjectCategory.RoundLog))
                {
                    entrada.Button.onClick.Invoke();
                }

                await Esperar(() => !forest.IsTransitioning);
                Canvas.ForceUpdateCanvases();
                await Awaitable.NextFrameAsync();
                forest.TakeCargo();
                forest.DragCargoTo(EnPantalla(forest.LogRow).center);
                forest.ReleaseCargo();
                Assume.That(forest.Cargo.IsPlaced, Is.True, "la caja quedó sobre los troncos");
                await Awaitable.NextFrameAsync();
                forest.PushButton.onClick.Invoke();

                // Patrón (2.2 → 2.3) → taller (fase 2).
                await AvanzarHasta(runner, () => runner.Flow.Current == GameState.Playing && runner.Flow.PlayingPhase == 2, 90f);
                Assert.That(perfil.IsPhaseConfirmed(new PhaseId(LevelId.Wheel, 1)), Is.True, "el rodado confirmó la fase 1");
                var workshop = await Esperar<WorkshopSceneController>(w => w.Pieces.Count > 0);
                workshop.Select(WorkshopPiece.ShortLogA);
                workshop.MachineButton.onClick.Invoke();
                workshop.Select(WorkshopPiece.ShortLogB);
                workshop.MachineButton.onClick.Invoke();
                await Arrastrar(workshop, WorkshopPiece.LongLog, workshop.Pieces[WorkshopPiece.ShortLogA].Rect);
                await Arrastrar(workshop, WorkshopPiece.Plank, workshop.AssemblyImage.rectTransform);
                await Arrastrar(workshop, WorkshopPiece.Cargo, workshop.AssemblyImage.rectTransform);
                await Arrastrar(workshop, WorkshopPiece.Rope, workshop.AssemblyImage.rectTransform);

                // Regreso (2.4) → laberinto (fase 3).
                await AvanzarHasta(runner, () => runner.Flow.Current == GameState.Playing && runner.Flow.PlayingPhase == 3, 90f);
                Assert.That(perfil.IsPhaseConfirmed(new PhaseId(LevelId.Wheel, 2)), Is.True, "la carretilla confirmó la fase 2");
                var maze = await Esperar<MazeSceneController>(m => m.Rows != null);
                var solucion = maze.Grid.Solution();
                foreach (var block in solucion.Blocks)
                {
                    maze.AddBlock(block);
                }

                maze.Execute();

                // Cierre (2.5) → resumen → menú.
                await AvanzarHasta(runner, () => runner.Flow.Current == GameState.LevelSummary, 120f);
                Assert.That(perfil.IsPhaseConfirmed(new PhaseId(LevelId.Wheel, 3)), Is.True, "llegar al refugio confirmó la fase 3");
                await Esperar(() => ButtonWithLabel("Continuar") != null);
                await Awaitable.NextFrameAsync();
                Click(ButtonWithLabel("Continuar"));
                await Esperar(() => runner.Flow.Current == GameState.LevelSelect);

                Assert.That(perfil.IsUnlocked(LevelId.River), Is.True, "el Nivel 3 queda desbloqueado (RF-03)");
                var guardado = runner.Session.Load(nombre);
                Assert.That(guardado.IsLevelComplete(LevelId.Wheel), Is.True, "las tres fases están en disco (RF-04, RNF-14)");
                Assert.That(guardado.IsUnlocked(LevelId.River), Is.True);
                Assert.That(guardado.IndicatorsFor(new PhaseId(LevelId.Wheel, 2)).StepsUsed, Is.EqualTo(6),
                    "fase 2: seis acciones de ensamblaje en orden, la última amarrar la caja (RF-45, §3.6.1, INC-54)");
                Assert.That(guardado.IndicatorsFor(new PhaseId(LevelId.Wheel, 3)).StepsUsed, Is.EqualTo(solucion.Count),
                    "fase 3: los bloques de la secuencia que llegó (RF-45, §3.6.1)");
                Assert.That(guardado.IndicatorsFor(new PhaseId(LevelId.Wheel, 1)).ResolutionSeconds, Is.GreaterThan(0f),
                    "fase 1: el tiempo de resolución se midió");
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

        // --- helpers -----------------------------------------------------------------------

        /// <summary>Pulsa «Continuar» de cada escena narrativa, línea a línea, hasta que el flujo llegue a donde se espera.</summary>
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

        private static async Task Arrastrar(WorkshopSceneController workshop, WorkshopPiece pieza, RectTransform objetivo)
        {
            Canvas.ForceUpdateCanvases();
            workshop.Take(pieza);
            workshop.DragTo(EnPantalla(objetivo).center);
            workshop.Release(pieza);
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
    }
}
