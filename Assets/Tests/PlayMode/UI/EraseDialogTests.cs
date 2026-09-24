using System;
using System.IO;
using System.Threading.Tasks;
using Game.Core;
using Game.Reporting;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    /// <summary>
    /// RF-47/CU-12: confirmación explícita e irreversibilidad del borrado desde el informe
    /// docente. Corre sobre un directorio temporal real — el mismo patrón que
    /// <c>ProfileSelectTests</c> usa para su propio diálogo de borrado — nunca sobre «Datos/»
    /// del proyecto.
    /// </summary>
    [Category("Integration")]
    public class EraseDialogTests
    {
        private const string SceneName = "TeacherReport";

        private string _dataDirectory;
        private SaveStore _store;

        [SetUp]
        public void SetUp()
        {
            _dataDirectory = Path.Combine(Path.GetTempPath(), "AlgoritmEraseDialog_" + Guid.NewGuid().ToString("N"))
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
        public async Task EraseDialog_RF47_ExigeConfirmacionExplicitaYAdvierteIrreversibilidad()
        {
            var (_, controller) = await OpenWithProfile("Ana");

            Click(controller.DeleteButton);

            Assert.That(controller.EraseDialog.gameObject.activeInHierarchy, Is.True,
                "CU-12 paso 3: exige un paso explícito antes de borrar");
            Assert.That(controller.EraseDialog.Prompt.text, Does.Contain("Ana")
                .And.Contain("no se puede deshacer").IgnoreCase);
        }

        [Test]
        [Timeout(20000)]
        public async Task EraseDialog_CU12_CancelarNoRealizaNingunCambioEnDisco()
        {
            var (_, controller) = await OpenWithProfile("Ana");
            Click(controller.DeleteButton);
            await Awaitable.NextFrameAsync(); // deja correr Start() del diálogo recién activado

            Click(controller.EraseDialog.CancelButton);

            Assert.That(controller.EraseDialog.gameObject.activeInHierarchy, Is.False, "el diálogo se cierra");
            Assert.That(_store.Exists("Ana"), Is.True, "CU-12 FA-4a: cancelar no cambia nada en disco");
            Assert.That(_store.ProfileNames().Count, Is.EqualTo(1), "ningún otro perfil se ve afectado");
        }

        [Test]
        [Timeout(20000)]
        public async Task EraseDialog_RF47_TrasConfirmarElPerfilDesapareceDeLaLista()
        {
            var (_, controller) = await OpenWithProfile("Ana");
            Click(controller.DeleteButton);
            await Awaitable.NextFrameAsync(); // deja correr Start() del diálogo recién activado

            Click(controller.EraseDialog.ConfirmButton);

            Assert.That(controller.EraseDialog.gameObject.activeInHierarchy, Is.False);
            Assert.That(_store.Exists("Ana"), Is.False, "el perfil ya no está en disco");
            Assert.That(controller.Entries, Is.Empty, "y la pantalla lo refleja sin recargar la escena");
        }

        // --- helpers -----------------------------------------------------------------------

        private async Task<(GameFlowRunner, TeacherReportController)> OpenWithProfile(string profileName)
        {
            _store.Save(PlayerProfile.Create(profileName, Array.Empty<string>()).Profile);

            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync();
            runner.GoTo(GameState.TeacherReport);

            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            new GameObject("TestSceneLoader").AddComponent<SceneLoader>();

            var controller = await WaitForComponentAsync<TeacherReportController>(10f);
            Assume.That(controller, Is.Not.Null, "no apareció TeacherReportController");

            controller.Repository = new ProfileRepository(new DiskFileSystem(), _dataDirectory, _dataDirectory);
            controller.EraseDialog.Session = new ProfileSession(runner.Flow, _store);
            controller.Show();

            return (runner, controller);
        }

        private static void Click(Button button) =>
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

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
    }
}
