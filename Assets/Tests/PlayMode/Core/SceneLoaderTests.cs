using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core.Tests
{
    [Category("Integration")]
    public class SceneLoaderTests
    {
        private const int FrameBudget = 600;

        [TearDown]
        public void DestruirElCargador()
        {
            // `SceneLoader` sobrevive al cambio de escena (DontDestroyOnLoad); sin esto la
            // instancia de una prueba se filtraría a la siguiente.
            foreach (var loader in Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(loader.gameObject);
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task SceneLoader_RNF04_LastLoadSecondsMideElTiempoRealDeCargaDeLaEscena()
        {
            var sut = new GameObject(nameof(SceneLoader)).AddComponent<SceneLoader>();

            var startedAt = Time.realtimeSinceStartupAsDouble;
            sut.Load("MainMenu");
            await WaitUntil(() => SceneManager.GetActiveScene().name == "MainMenu");
            var observedSeconds = Time.realtimeSinceStartupAsDouble - startedAt;

            Assert.That(sut.LastLoadSeconds,
                Is.GreaterThan(0.001).And.LessThanOrEqualTo(observedSeconds));
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
