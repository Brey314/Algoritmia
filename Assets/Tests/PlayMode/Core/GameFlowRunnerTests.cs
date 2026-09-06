using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.Core.Tests
{
    [Category("Integration")]
    public class GameFlowRunnerTests
    {
        private const int FrameBudget = 600;

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
        [Timeout(20000)]
        public async Task GameFlowRunner_RF02_SelectProfileFijaElPerfilYEntraALevelSelect()
        {
            var runner = await BootToMainMenu();
            runner.GoTo(GameState.ProfileSelect);
            var profile = PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

            var selected = runner.SelectProfile(profile);

            Assert.That(selected, Is.True, "SelectProfile");
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.LevelSelect), "estado");
            Assert.That(runner.Flow.ActiveProfile, Is.SameAs(profile), "perfil activo");
        }

        [Test]
        [Timeout(20000)]
        public async Task GameFlowRunner_RNF16_VolverAlMenuDesdeElPanelDePerfilNoRecargaLaEscena()
        {
            var runner = await BootToMainMenu();
            var probe = new GameObject("ReloadProbe").AddComponent<MainMenuMarker>();
            runner.GoTo(GameState.ProfileSelect);

            runner.GoTo(GameState.MainMenu);
            await Awaitable.NextFrameAsync();

            // Si MainMenu se hubiera recargado, la carga de escena en modo Single habría
            // destruido este objeto. Sigue vivo: fue un intercambio de paneles, no una recarga.
            Assert.That((bool)probe, Is.True);
        }

        private static async Task<GameFlowRunner> BootToMainMenu()
        {
            SceneManager.LoadScene("Boot");
            await WaitUntil(() => GameFlowRunner.Instance != null
                                  && SceneManager.GetActiveScene().name == "MainMenu");
            return GameFlowRunner.Instance;
        }

        private static async Task WaitUntil(Func<bool> condition)
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

        /// <summary>Marcador puesto en la escena MainMenu por la prueba para detectar una recarga.</summary>
        private sealed class MainMenuMarker : MonoBehaviour
        {
        }
    }
}
