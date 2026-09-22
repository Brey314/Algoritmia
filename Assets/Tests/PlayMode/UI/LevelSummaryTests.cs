using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Core;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    // No referencia Game.Levels.Fire (ni su .asmdef ni su InternalsVisibleTo): el panel del
    // Nivel 1 se conduce por estructura —botones por etiqueta, el deslizante por tipo—, igual que
    // PauseMenuTests lo hace desde Game.UI (T16). El runner se crea antes de cargar «Level1_Cave»,
    // así que FirePanelController.Awake lo recoge solo (GameFlowRunner.Instance ya existe) sin
    // necesitar el tipo para inyectarlo a mano.
    [Category("Integration")]
    public class LevelSummaryTests
    {
        private const string FireSceneName = "Level1_Cave";

        // N1_Config (Fase 5): el deslizante mide fuerza, 7 cae en la franja efectiva.
        private const float VeryClosePosition = 7f; // fuerza efectiva (N1_Config, Fase 5)
        private const float EffectiveSpacing = 5f; // la única muesca efectiva (N1_Config)
        private const int MinimumEffectiveStrikes = 3;

        [TearDown]
        public void DestruirLosObjetosPersistentes()
        {
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(runner.gameObject);
            }

            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }
        }

        [Test]
        [Timeout(45000)]
        [Category("Acceptance")]
        public async Task LevelSummary_RF03_DevuelveAlMenuConNivel2Desbloqueado()
        {
            var profile = NewProfile();
            var runner = await LoadFireLevelWithProfile(profile);

            await ConvergeAndBlow();
            Assert.That(await WaitUntilAsync(() => runner.Flow.Current == GameState.Narrative, 5f), Is.True,
                "el flujo no llegó a la escena narrativa de cierre tras soplar");

            var narrative = await WaitForComponentAsync<NarrativeSceneController>(10f);
            Assert.That(narrative, Is.Not.Null, "no apareció la escena narrativa de cierre");
            AvanzarNarrativaHastaElFinal(narrative, runner.Flow.NarrativeSequenceId);

            Assert.That(await WaitUntilAsync(() => runner.Flow.Current == GameState.LevelSummary, 5f), Is.True,
                "el flujo no llegó al resumen de fin de nivel");

            var summary = await WaitForComponentAsync<LevelSummaryController>(10f);
            Assert.That(summary, Is.Not.Null, "no apareció el resumen de fin de nivel");

            // Start() ya corrió Show() una vez con el guardado real (Runner.Session): confirmar la
            // fase y desbloquear son idempotentes (PlayerProfile.ConfirmPhase, LevelUnlockPolicy),
            // así que inyectar el espía y repetir Show() es seguro y es la única forma de observar
            // la llamada a guardar, igual que NarrativeSceneController.Begin() se reinvoca a mano
            // en NarrativeSceneTests.
            var guardados = new List<string>();
            summary.Saver = new SpyProfileSaver(() => guardados.Add("guardar"));
            summary.Show();

            // Mockups 13 y 13b: título, hallazgo, relato en viñetas y habilidad nombrada, sin cifras (RF-45, CP-03).
            Assert.That(summary.TitleLabel.text, Is.Not.Empty.And.Not.EndWith(":"), "el título es la apertura del resumen");
            Assert.That(summary.Bullets, Has.Count.EqualTo(2), "una viñeta por frase del relato");
            Assert.That(summary.SkillLabel.text, Does.Contain("probar y ajustar"), "nombra la habilidad (RF-12)");
            var textos = new List<string> { summary.TitleLabel.text, summary.DiscoveryLabel.text, summary.SkillLabel.text };
            textos.AddRange(summary.Bullets.Select(b => b.text));
            Assert.That(textos, Has.All.Matches<string>(t => !t.Any(char.IsDigit)), "ninguna cifra a la vista del estudiante");

            Click(summary.ContinueButton);
            Assert.That(await WaitUntilAsync(() => runner.Flow.Current == GameState.LevelSelect, 5f), Is.True,
                "«Continuar» no volvió al menú de niveles");

            Assert.That(profile.IsUnlocked(LevelId.Wheel), Is.True, "el Nivel 2 queda desbloqueado (RF-03)");
            Assert.That(profile.IsPhaseConfirmed(new PhaseId(LevelId.Fire, 1)), Is.True, "la fase queda confirmada (RF-04)");
            Assert.That(guardados, Contains.Item("guardar"), "el guardado se invoca al llegar al resumen (RF-04)");
        }

        [Test]
        [Timeout(30000)]
        [Category("Acceptance")]
        public async Task LevelSummary_CP07_ElCierreReflexivoNoEsOmitibleLaPrimeraVez()
        {
            var runner = await LoadFireLevelWithProfile(NewProfile());

            await ConvergeAndBlow();
            Assert.That(await WaitUntilAsync(() => runner.Flow.Current == GameState.Narrative, 5f), Is.True,
                "el flujo no llegó a la escena narrativa de cierre tras soplar");

            var narrative = await WaitForComponentAsync<NarrativeSceneController>(10f);

            Assert.That(narrative, Is.Not.Null, "no apareció la escena narrativa de cierre");
            Assert.That(narrative.SkipButton.gameObject.activeInHierarchy, Is.False,
                "CP-07: la primerísima vez que se completa el nivel, el cierre reflexivo no se " +
                "puede omitir — si esto falla, «ConfirmPhase» volvió a adelantarse a la narrativa");
        }

        [Test]
        [Timeout(45000)]
        [Category("Acceptance")]
        public async Task LevelSummary_RF03_DevuelveAlMenuConNivel3Desbloqueado()
        {
            // Un perfil que acaba de resolver el laberinto: las fases 1 y 2 ya están en el perfil
            // (las confirma cada escena, W07/W09) y la 3 la trae el runner (HU-14 paso 6).
            var profile = NewProfile();
            profile.Reach(LevelId.Wheel);
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 1), new PerformanceIndicators(3, 1, 0, 60f));
            profile.ConfirmPhase(new PhaseId(LevelId.Wheel, 2), new PerformanceIndicators(0, 0, 5, 90f));

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu
            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(profile);
            runner.StartPlaying(LevelId.Wheel, 3);
            runner.PendingIndicators = new PerformanceIndicators(2, 2, 6, 120f);
            Assume.That(runner.StartNarrative("N2_Escena25_Cierre"), Is.True, "el laberinto sale al cierre reflexivo");

            var load = SceneManager.LoadSceneAsync("Narrative", LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            new GameObject("TestSceneLoader").AddComponent<SceneLoader>();

            var narrative = await WaitForComponentAsync<NarrativeSceneController>(10f);
            Assert.That(narrative, Is.Not.Null, "no apareció el cierre reflexivo del Nivel 2");
            Assert.That(narrative.SkipButton.gameObject.activeInHierarchy, Is.False,
                "CP-07: la primera vez el cierre reflexivo no se puede omitir");
            AvanzarNarrativaHastaElFinal(narrative, "N2_Escena25_Cierre");

            Assert.That(await WaitUntilAsync(() => runner.Flow.Current == GameState.LevelSummary, 5f), Is.True,
                "el cierre reflexivo no llevó al resumen de fin de nivel");
            var summary = await WaitForComponentAsync<LevelSummaryController>(10f);
            Assert.That(summary, Is.Not.Null, "no apareció el resumen del Nivel 2");

            var textos = new List<string> { summary.TitleLabel.text, summary.DiscoveryLabel.text, summary.SkillLabel.text };
            textos.AddRange(summary.Bullets.Select(b => b.text));
            Assert.That(summary.TitleLabel.text, Does.Contain("rueda").IgnoreCase, "el resumen es el del Nivel 2, no el de la cueva");
            Assert.That(summary.SkillLabel.text, Does.Contain("abstraer").IgnoreCase.And.Contain("algoritmo").IgnoreCase,
                "nombra las dos habilidades ejercitadas (RF-12, guion §6.4)");
            Assert.That(textos, Has.All.Matches<string>(t => !t.Any(char.IsDigit)), "ninguna cifra de las tres fases se filtra (INC-26)");

            Click(summary.ContinueButton);
            Assert.That(await WaitUntilAsync(() => runner.Flow.Current == GameState.LevelSelect, 5f), Is.True,
                "«Continuar» no volvió al menú de niveles");
            Assert.That(profile.IsUnlocked(LevelId.River), Is.True, "el Nivel 3 queda desbloqueado (RF-03)");
            Assert.That(profile.IsLevelComplete(LevelId.Wheel), Is.True, "las tres fases del Nivel 2 quedan confirmadas (RF-04)");

            var levelSelect = await WaitForComponentAsync<LevelSelectController>(10f);
            Assert.That(levelSelect, Is.Not.Null);
            await Awaitable.NextFrameAsync();
            Assert.That(levelSelect.ButtonFor(LevelId.River).interactable, Is.True, "el menú muestra el Nivel 3 habilitado (RF-03)");
            Assert.That(levelSelect.LockBadgeShownFor(LevelId.River), Is.False, "y sin candado (RNF-19)");
        }

        // --- helpers -----------------------------------------------------------------------

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        /// <summary>
        /// Crea el runner **antes** de cargar «Level1_Cave»: así sobrevive a la carga
        /// (<c>DontDestroyOnLoad</c>) y <c>FirePanelController.Awake</c> lo recoge solo vía
        /// <c>GameFlowRunner.Instance</c>, sin que esta prueba necesite el tipo para inyectarlo.
        /// El <see cref="SceneLoader"/> se crea después, ya con la escena estable: crearlo antes
        /// haría que <c>StartPlaying</c> recargara la escena que se está a punto de cargar a mano
        /// (mismo motivo que documenta <c>PauseMenuTests</c>).
        /// </summary>
        private static async Task<GameFlowRunner> LoadFireLevelWithProfile(PlayerProfile profile)
        {
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu

            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(profile);
            runner.StartPlaying(LevelId.Fire, 1);
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing),
                "el flujo no llegó a Playing: el arreglo de la prueba está roto");

            var load = SceneManager.LoadSceneAsync(FireSceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync(); // deja correr FirePanelController.Start()
            await Awaitable.NextFrameAsync();

            new GameObject("TestSceneLoader").AddComponent<SceneLoader>();

            return runner;
        }

        /// <summary>Reúne, marca fuerza y cercanía efectivas, converge con el mínimo de golpes efectivos y pulsa «Soplar».</summary>
        private static async Task ConvergeAndBlow()
        {
            await ReunirAsync();
            SliderNamed("FuerzaSlider").value = VeryClosePosition;
            SliderNamed("CercaniaSlider").value = EffectiveSpacing;
            ClickTimes(ButtonWithLabel("Golpear"), MinimumEffectiveStrikes);
            Click(ButtonWithLabel("Soplar"));
        }

        private static void AvanzarNarrativaHastaElFinal(NarrativeSceneController narrative, string sequenceId)
        {
            var lineas = narrative.Sequences.First(sequence => sequence.Id == sequenceId).Lines.Length;
            for (var i = 0; i < lineas; i++)
            {
                Click(narrative.AdvanceButton);
            }
        }

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

        private static void ClickTimes(Button button, int times)
        {
            for (var i = 0; i < times; i++)
            {
                Click(button);
            }
        }

        private static Button ButtonWithLabel(string label) => Array.Find(
            Object.FindObjectsByType<Button>(FindObjectsInactive.Include),
            b => b.GetComponentInChildren<Text>(true) is { } text && text.text.Trim() == label);

        /// <summary>
        /// Sondea cuadro a cuadro hasta que <paramref name="condition"/> se cumpla o venza el
        /// tiempo real, igual que <c>FirePanelTests.WaitUntilAsync</c>: ni la animación de
        /// convergencia ni la carga de escena terminan en un número fijo de fotogramas.
        /// </summary>
        private static async Task<bool> WaitUntilAsync(Func<bool> condition, float timeoutSeconds)
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (!condition())
            {
                if (Time.realtimeSinceStartup >= deadline)
                {
                    return false;
                }

                await Awaitable.NextFrameAsync();
            }

            return true;
        }

        /// <summary>Sondea hasta que un componente del tipo pedido aparezca en la escena activa.</summary>
        private static async Task<T> WaitForComponentAsync<T>(float timeoutSeconds) where T : Object
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            T found;
            while ((found = Object.FindAnyObjectByType<T>(FindObjectsInactive.Include)) == null)
            {
                if (Time.realtimeSinceStartup >= deadline)
                {
                    return null;
                }

                await Awaitable.NextFrameAsync();
            }

            return found;
        }

        private sealed class SpyProfileSaver : IProfileSaver
        {
            private readonly Action _onSaveActive;
            public SpyProfileSaver(Action onSaveActive) => _onSaveActive = onSaveActive;
            public void SaveActive() => _onSaveActive();
        }

        /// <summary>Hojas y piedras al punto del fuego: sin eso ningún golpe prende (T22).</summary>
        /// <summary>
        /// Reúne todas las piezas en el punto del fuego y suelta la última con un fin de arrastre
        /// real (T25): el panel pasa al encendido por su propio evento, sin tocar
        /// <c>Game.Levels.Fire</c>. Termina cuando «Golpear» está en pantalla.
        /// </summary>
        private static async Task ReunirAsync()
        {
            var suelo = GameObject.Find("Suelo").transform;
            var punto = (RectTransform)suelo.Find("PuntoDeFuego");
            RectTransform ultima = null;
            foreach (RectTransform pieza in suelo)
            {
                if (pieza.name != "PuntoDeFuego" && pieza.name != "AnilloReunion")
                {
                    pieza.anchoredPosition = punto.anchoredPosition;
                    ultima = pieza;
                }
            }

            var pointer = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(null, punto.position)
            };
            ExecuteEvents.Execute(ultima.gameObject, pointer, ExecuteEvents.endDragHandler);

            var deadline = Time.realtimeSinceStartup + 5f;
            while (!(ButtonWithLabel("Golpear")?.isActiveAndEnabled ?? false) && Time.realtimeSinceStartup < deadline)
            {
                await Awaitable.NextFrameAsync();
            }

            Assume.That(ButtonWithLabel("Golpear").isActiveAndEnabled, Is.True, "el panel pasó al encendido");
            await Awaitable.NextFrameAsync(); // deja correr Awake/Start de la interfaz recién activada
        }

        private static Slider SliderNamed(string name) => Array.Find(
            Object.FindObjectsByType<Slider>(FindObjectsInactive.Include), slider => slider.name == name);
    }
}
