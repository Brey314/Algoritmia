using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Core.Tests
{
    /// <summary>
    /// RNF-02 sobre el juego completo (R16): las cinco escenas jugables, cargadas de verdad, solo
    /// aceptan clic y clic sostenido (CT-06, INC-01). Lo que cada nivel permite hundir o arrastrar
    /// lo fija su propia prueba <c>*_RNF02_*</c>; aquí va lo que es igual en las cinco.
    /// </summary>
    [Category("Integration")]
    public class ControlSchemeTests
    {
        private static readonly string[] PlayableScenes =
        {
            "Level1_Cave", "Level2_Forest", "Level2_Workshop", "Level2_Maze", "Level3_River"
        };

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
        [Timeout(120000)]
        [Category("Acceptance")]
        public async Task Controls_RNF02_LasCincoEscenasJugablesSoloAceptanClicYClicSostenido()
        {
            foreach (var escena in PlayableScenes)
            {
                var carga = SceneManager.LoadSceneAsync(escena, LoadSceneMode.Single);
                while (!carga.isDone)
                {
                    await Awaitable.NextFrameAsync();
                }

                await Awaitable.NextFrameAsync(); // `Start` corre después de que la carga se declare terminada.

                var module = Object.FindAnyObjectByType<InputSystemUIInputModule>(FindObjectsInactive.Include);
                Assert.That(module, Is.Not.Null, $"{escena}: lleva el módulo de entrada del Input System (CT-06)");
                var mapa = module.actionsAsset != null ? module.actionsAsset : InputSystem.actions;
                Assert.That(mapa, Is.Not.Null, $"{escena}: el módulo resuelve un mapa de controles");
                Assert.That(mapa.name, Is.EqualTo("ControlesJugables"), $"{escena}: usa el mapa del juego, no el de la plantilla");
                Assert.That(mapa.bindings.Select(binding => binding.effectivePath), Has.All.Matches<string>(EsClic),
                    $"{escena}: toda vinculación es un clic o un clic sostenido (INC-01)");
                Assert.That(module.point?.action, Is.Not.Null, $"{escena}: el puntero está cableado al mapa");
                Assert.That(module.leftClick?.action, Is.Not.Null, $"{escena}: el clic está cableado al mapa");
                Assert.That(Object.FindObjectsByType<PlayerInput>(FindObjectsInactive.Include), Is.Empty,
                    $"{escena}: sin PlayerInput, nada lee un dispositivo aparte del módulo de UI");
                Assert.That(Object.FindObjectsByType<ScrollRect>(FindObjectsInactive.Include), Is.Empty,
                    $"{escena}: sin ScrollRect —lo que desborda se desplaza con botones, no arrastrando—");
            }
        }

        private static bool EsClic(string path) =>
            path.StartsWith("<Pointer>/") || path.StartsWith("<Touchscreen>/") || path.StartsWith("<Pen>/")
            || (path.StartsWith("<Mouse>/") && !path.Contains("scroll"));
    }
}
