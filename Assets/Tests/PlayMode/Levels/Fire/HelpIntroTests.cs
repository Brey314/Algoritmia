using System;
using System.Threading.Tasks;
using Game.Core;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Levels.Fire.Tests
{
    /// <summary>
    /// El aviso que explica el botón de ayuda en la primera mecánica (INC-158): sale al abrir la
    /// cueva solo mientras el perfil no haya terminado la fase 1, detiene el tiempo, ilumina «Pista»
    /// por encima del fondo oscuro y, al cerrarse, todo vuelve a como estaba.
    /// </summary>
    [Category("Integration")]
    public class HelpIntroTests
    {
        [TearDown]
        public void LimpiarElEstadoGlobal()
        {
            Time.timeScale = 1f; // una prueba que falla con el aviso abierto no puede dejar el tiempo detenido

            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(runner.gameObject);
            }
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FireLevel_INC158_ElAvisoDetieneElTiempoIluminaLaAyudaYAlCerrarTodoVuelve()
        {
            var controller = await FirePanelTests.LoadPanel();
            var intro = controller.HelpIntro;
            var helpButton = intro.HelpButton;

            intro.Show("Texto de prueba", "Cerrar");
            await Awaitable.NextFrameAsync();

            Assert.That(intro.IsOpen, Is.True);
            Assert.That(intro.gameObject.activeSelf, Is.True);
            Assert.That(Time.timeScale, Is.EqualTo(0f), "mientras se lee, el tiempo está detenido");
            var lit = helpButton.GetComponent<Canvas>();
            Assert.That(lit, Is.Not.Null, "«Pista» se ilumina con un Canvas propio");
            Assert.That(lit.overrideSorting, Is.True);
            Assert.That(lit.sortingOrder, Is.GreaterThan(helpButton.GetComponentInParent<Canvas>(true).rootCanvas.sortingOrder),
                "por encima del fondo oscuro, que es del lienzo de la escena");
            Assert.That(helpButton.GetComponent<GraphicRaycaster>(), Is.Not.Null);

            FirePanelTests.Click(intro.CloseButton);
            await Awaitable.NextFrameAsync(); // Destroy quita el Canvas al final del cuadro

            Assert.That(intro.IsOpen, Is.False);
            Assert.That(intro.gameObject.activeSelf, Is.False);
            Assert.That(Time.timeScale, Is.EqualTo(1f), "se devuelve el tiempo que había");
            Assert.That(helpButton.GetComponent<Canvas>(), Is.Null, "«Pista» deja de tener Canvas propio");
            Assert.That(helpButton.GetComponent<GraphicRaycaster>(), Is.Null);
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FireLevel_INC158_UnPerfilNuevoVeElAvisoAlAbrirLaCuevaYElRatoDeLecturaNoCuentaComoResolucion()
        {
            await ArrancarElFlujo(NewProfile());

            var controller = await FirePanelTests.LoadPanel();

            Assert.That(controller.HelpIntro.IsOpen, Is.True, "el perfil no ha confirmado la fase 1");
            Assert.That(Time.timeScale, Is.EqualTo(0f));

            var deadline = Time.realtimeSinceStartup + 0.5f; // medio segundo de lectura, en tiempo real
            while (Time.realtimeSinceStartup < deadline)
            {
                await Awaitable.NextFrameAsync();
            }

            FirePanelTests.Click(controller.HelpIntro.CloseButton);

            Assert.That(Time.timeScale, Is.EqualTo(1f));
            Assert.That(controller.Indicators.Complete().ResolutionSeconds, Is.LessThan(0.4f),
                "leer el aviso no es resolver: se descuenta igual que la pausa (RF-07)");
        }

        [Test]
        [Timeout(20000)]
        [Category("Acceptance")]
        public async Task FireLevel_INC158_UnPerfilQueYaTerminoLaFase1NoVeElAviso()
        {
            var profile = NewProfile();
            profile.ConfirmPhase(new PhaseId(LevelId.Fire, 1), new PerformanceIndicators(1, 0, 1, 10f));
            await ArrancarElFlujo(profile);

            var controller = await FirePanelTests.LoadPanel();

            Assert.That(controller.HelpIntro.IsOpen, Is.False);
            Assert.That(controller.HelpIntro.gameObject.activeSelf, Is.False);
            Assert.That(Time.timeScale, Is.EqualTo(1f));
        }

        // --- helpers -----------------------------------------------------------------------

        private static PlayerProfile NewProfile() =>
            PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;

        /// <summary>
        /// El flujo con el perfil activo y en <see cref="GameState.Playing"/> **antes** de cargar la
        /// cueva, que es como llega de verdad: su <c>Start</c> pregunta al perfil si ya terminó la fase 1.
        /// </summary>
        private static async Task ArrancarElFlujo(PlayerProfile profile)
        {
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu

            runner.GoTo(GameState.ProfileSelect);
            runner.SelectProfile(profile);
            runner.StartPlaying(LevelId.Fire, 1);
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing),
                "el flujo no llegó a Playing: el arreglo de la prueba está roto");
        }
    }
}
