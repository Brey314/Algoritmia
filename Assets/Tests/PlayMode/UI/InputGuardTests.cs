using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    /// <summary>
    /// DEF-RC3-09: el segundo clic de un doble clic en «Omitir» caía sobre el botón de la escena que
    /// acababa de cargar. Aquí se comprueba con una carga real que la escena nueva abre sin entrada y
    /// que la recupera sola; la ventana con el tiempo inyectado está en EditMode.
    /// </summary>
    [Category("Integration")]
    public class InputGuardTests
    {
        // «LevelSummary» es la escena donde cayó el segundo clic; el resto de pantallas pasan por lo mismo.
        private const string SceneName = "LevelSummary";

        private GameObject _guardObject;

        [TearDown]
        public void DestruirLaGuarda()
        {
            if (_guardObject != null)
            {
                Object.DestroyImmediate(_guardObject);
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task InputGuard_RF45_DEFRC309_LaEscenaCargadaNoRecibeEntradaDuranteLaVentanaYLaRecuperaDespues()
        {
            // Persistente como en «Boot»: la guarda tiene que sobrevivir a la carga que va a vigilar.
            _guardObject = new GameObject("InputGuard(Prueba)");
            Object.DontDestroyOnLoad(_guardObject);
            var sut = _guardObject.AddComponent<InputGuard>();
            sut.Seconds = 1.5f; // sobra para observar el estado justo tras la carga aunque un cuadro se alargue

            var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            var module = Object.FindAnyObjectByType<BaseInputModule>(FindObjectsInactive.Include);
            Assume.That(module, Is.Not.Null, $"«{SceneName}» lleva su módulo de entrada");
            Assert.That(module.enabled, Is.False, "recién cargada, la escena no recibe clics: el segundo clic del doble clic se pierde");

            var deadline = Time.realtimeSinceStartup + 10f;
            while (!module.enabled && Time.realtimeSinceStartup < deadline)
            {
                await Awaitable.NextFrameAsync();
            }

            Assert.That(module.enabled, Is.True, "pasada la ventana, la escena vuelve a recibir clics");
        }
    }
}
