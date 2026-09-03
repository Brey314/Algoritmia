using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core.Tests
{
    [Category("Integration")]
    public class BootFlowTests
    {
        private const int FrameBudget = 600;

        [TearDown]
        public void DestruirLosObjetosPersistentes()
        {
            // `GameFlowRunner` y `SceneLoader` sobreviven al cambio de escena (DontDestroyOnLoad).
            // Sin esto, cada prueba heredaría el flujo de la anterior y sus esperas se cumplirían
            // de entrada: pasarían sin haber probado nada.
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
        public async Task BootFlow_RF01_ArrancaEnBootYLlegaSoloAMainMenu()
        {
            SceneManager.LoadScene("Boot");

            // Se espera la escena y no el estado: `SceneManager.LoadScene` aplica al final del
            // frame, así que la FSM llega a MainMenu un frame antes que la escena. Esperar el
            // estado dejaba pasar la aserción con «Boot» todavía activa.
            await WaitUntil(() => GameFlowRunner.Instance != null
                                  && SceneManager.GetActiveScene().name == "MainMenu");

            Assert.That(GameFlowRunner.Instance.Flow.Current, Is.EqualTo(GameState.MainMenu));
        }

        [Test]
        public async Task GameFlowRunner_RNF16_NoSeDuplicaAlRecargarEscena()
        {
            SceneManager.LoadScene("Boot");
            await WaitUntil(() => GameFlowRunner.Instance != null);

            // Volver a una escena ya visitada: el objeto persistente vuelve a instanciarse y el
            // guardado de Awake tiene que descartar la copia.
            SceneManager.LoadScene("Boot");
            await WaitUntil(() => SceneManager.GetActiveScene().name == "Boot");

            Assert.That(Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include),
                Has.Length.EqualTo(1), "GameFlowRunner");
            Assert.That(Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include),
                Has.Length.EqualTo(1), "SceneLoader");
        }

        [Test]
        public async Task GameFlowRunner_RNF16_NoDecideTransicionesLasDelegaEnGameFlow()
        {
            SceneManager.LoadScene("Boot");
            // La escena, no el estado: la carga se aplica al final del frame (ver RF01 arriba).
            await WaitUntil(() => GameFlowRunner.Instance != null
                                  && SceneManager.GetActiveScene().name == "MainMenu");
            var sut = GameFlowRunner.Instance;

            // Desde MainMenu no se entra a LevelSummary. Quien lo rechaza es la FSM, no el
            // adaptador: si la regla se colara aquí dejaría de ser probable en EditMode.
            var accepted = sut.GoTo(GameState.LevelSummary);

            Assert.That(accepted, Is.False);
            Assert.That(sut.Flow.Current, Is.EqualTo(GameState.MainMenu));
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("MainMenu"));
        }

        /// <summary>
        /// Espera a que se cumpla la condición, no a un número fijo de frames: cuántos hacen
        /// falta depende de lo que tarde en cargar la escena, y eso cambia con el equipo.
        /// </summary>
        private static async Task WaitUntil(System.Func<bool> condition)
        {
            for (var frame = 0; frame < FrameBudget; frame++)
            {
                if (condition())
                {
                    return;
                }

                await Awaitable.NextFrameAsync();
            }

            Assert.Fail($"La condición no se cumplió en {FrameBudget} frames.");
        }
    }
}
