using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    [Category("Integration")]
    public class ProfileSelectTests
    {
        private const string SceneName = "MainMenu";

        private string _dataDirectory;
        private SaveStore _store;

        [SetUp]
        public void SetUp()
        {
            _dataDirectory = Path.Combine(Path.GetTempPath(), "AlgoritmProfileSelect_" + Guid.NewGuid().ToString("N"))
                .Replace('\\', '/');
            _store = new SaveStore(new DiskFileSystem(), _dataDirectory, _dataDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(runner.gameObject);
            }

            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }

            if (Directory.Exists(_dataDirectory))
            {
                Directory.Delete(_dataDirectory, recursive: true);
            }
        }

        [Test]
        [Timeout(20000)]
        public async Task ProfileSelect_HU01_PerfilExistenteRestauraElProgresoExacto()
        {
            var saved = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            saved.Reach(LevelId.Wheel);
            saved.ConfirmPhase(LevelId.Fire, 1, new PerformanceIndicators(4, 2, 3, 90f));
            _store.Save(saved);
            var runner = await OpenProfilePanel();

            ClickProfileEntry("Ana");

            Assert.That(runner.Flow.ActiveProfile.ReachedLevel, Is.EqualTo(LevelId.Wheel), "nivel alcanzado");
            Assert.That(runner.Flow.ActiveProfile.IsPhaseConfirmed(LevelId.Fire, 1), Is.True, "fase confirmada");
        }

        [Test]
        [Timeout(20000)]
        public async Task ProfileSelect_HU01_NombreVacioMuestraAvisoYNoAvanza()
        {
            var runner = await OpenProfilePanel();

            TypeNameAndConfirm("   ");

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.ProfileSelect));
        }

        [Test]
        [Timeout(20000)]
        public async Task ProfileSelect_HU01_NombreDuplicadoMuestraAvisoYNoAvanza()
        {
            _store.Save(PlayerProfile.Create("Ana", Array.Empty<string>()).Profile);
            var runner = await OpenProfilePanel();

            TypeNameAndConfirm("Ana");

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.ProfileSelect));
        }

        [Test]
        [Timeout(20000)]
        public async Task ProfileSelect_HU01_PerfilNuevoAlcanzaSoloElNivel1YArrancaLaNarrativa()
        {
            var runner = await OpenProfilePanel();

            TypeNameAndConfirm("Beto");

            Assert.That(runner.Flow.ActiveProfile.ReachedLevel, Is.EqualTo(LevelId.Fire), "nivel alcanzado");
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative), "estado");
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo("N1_Apertura"), "secuencia");
        }

        [Test]
        [Timeout(20000)]
        public async Task ProfileSelect_RNF09_ElFormularioPideUnSoloDato()
        {
            await OpenProfilePanel();
            var panel = Object.FindAnyObjectByType<ProfileSelectController>(FindObjectsInactive.Include);

            Assert.That(panel.GetComponentsInChildren<InputField>(true), Has.Length.EqualTo(1));
        }

        [Test]
        [Timeout(20000)]
        public async Task ProfileSelect_RNF13_ElPanelNoLanzaSiSeAbreSinPasarPorBoot()
        {
            await LoadMainMenu();
            var panel = Object.FindAnyObjectByType<ProfileSelectController>(FindObjectsInactive.Include);

            // Mostrar el panel a mano —lo que se hace al iterar la interfaz en el Editor— lo
            // deja sin ProfileSession ni GameFlowRunner: los inyecta Boot. Avisa dos veces: al
            // listar los perfiles y al confirmar el nombre.
            LogAssert.Expect(LogType.Warning, new Regex("sin pasar por"));
            LogAssert.Expect(LogType.Warning, new Regex("sin pasar por"));

            panel.gameObject.SetActive(true);
            await Awaitable.NextFrameAsync();
            TypeNameAndConfirm("Beto");

            // Lo que no puede pasar es que lance: cualquier excepción registrada aquí sería un
            // mensaje no esperado y esta llamada la delata.
            LogAssert.NoUnexpectedReceived();
        }

        // --- helpers -----------------------------------------------------------------------

        private static async Task LoadMainMenu()
        {
            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
        }

        private async Task<GameFlowRunner> OpenProfilePanel()
        {
            await LoadMainMenu();

            // El panel usa su propia Session (contra el almacén temporal); la del runner nunca se
            // construye porque en estas pruebas nadie pulsa «Salir».
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            // Un frame para que corra GameFlowRunner.Start(), que navega solo a MainMenu; recién
            // después se lleva el flujo al panel de perfil, o Start lo devolvería a MainMenu.
            await Awaitable.NextFrameAsync();
            runner.GoTo(GameState.ProfileSelect);

            var panel = Object.FindAnyObjectByType<ProfileSelectController>(FindObjectsInactive.Include);
            panel.Session = new ProfileSession(runner.Flow, _store);
            panel.Runner = runner;
            panel.gameObject.SetActive(true);
            await Awaitable.NextFrameAsync();
            Assume.That(runner.Flow.Current, Is.EqualTo(GameState.ProfileSelect));
            return runner;
        }

        private static void ClickProfileEntry(string profileName)
        {
            var entry = Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude)
                .First(button => button.name == $"ProfileEntry({profileName})");
            ExecuteEvents.Execute(entry.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);
        }

        private static void TypeNameAndConfirm(string profileName)
        {
            var panel = Object.FindAnyObjectByType<ProfileSelectController>(FindObjectsInactive.Include);
            var field = panel.GetComponentsInChildren<InputField>(true).Single();
            field.text = profileName;

            var confirm = panel.GetComponentsInChildren<Button>(true)
                .First(button => button.GetComponentInChildren<Text>() is { } text
                                 && text.text.Trim() is "Crear" or "Empezar" or "Confirmar");
            ExecuteEvents.Execute(confirm.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);
        }
    }
}
