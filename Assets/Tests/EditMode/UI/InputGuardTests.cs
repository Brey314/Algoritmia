using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.UI.Tests
{
    /// <summary>
    /// DEF-RC3-09: un doble clic rápido en «Omitir» del cierre del Nivel 1 dejaba al estudiante en el menú
    /// de niveles sin ver el resumen. El primer clic sale de la escena; el segundo caía sobre el botón
    /// «Volver al menú de niveles» del resumen, que ya estaba cargado y justo debajo. La guarda es la de
    /// toda carga, no la de esa narrativa: tras cargar una escena, la entrada no llega a su interfaz
    /// durante una ventana corta.
    /// </summary>
    /// <remarks>
    /// EditMode a propósito: la ventana se prueba con el tiempo inyectado (<c>Hold</c> y <c>Tick</c>), sin
    /// escena ni cuadros. Que la carga de una escena real la abre y la cierra lo prueba
    /// <c>InputGuardTests</c> de PlayMode.
    /// </remarks>
    public class InputGuardTests
    {
        private readonly List<GameObject> _created = new List<GameObject>();

        [TearDown]
        public void DestruirLoCreado()
        {
            foreach (var go in _created)
            {
                Object.DestroyImmediate(go);
            }

            _created.Clear();
        }

        [Test]
        public void InputGuard_RF45_DEFRC309_ApagaElModuloDeEntradaAlAbrirLaVentana()
        {
            var module = CrearModulo();
            var sut = CrearGuarda();

            sut.Hold(until: 10f);

            Assert.That(module.enabled, Is.False);
        }

        [Test]
        public void InputGuard_RF45_DEFRC309_NoDevuelveLaEntradaAntesDeQueTermineLaVentana()
        {
            var module = CrearModulo();
            var sut = CrearGuarda();
            sut.Hold(until: 10f);

            sut.Tick(now: 9.9f);

            Assert.That(module.enabled, Is.False);
        }

        [Test]
        public void InputGuard_RF45_DEFRC309_DevuelveLaEntradaCuandoTerminaLaVentana()
        {
            var module = CrearModulo();
            var sut = CrearGuarda();
            sut.Hold(until: 10f);

            sut.Tick(now: 10f);

            Assert.That(module.enabled, Is.True);
        }

        [Test]
        public void InputGuard_RF45_DEFRC309_NoEnciendeUnModuloQueYaEstabaApagado()
        {
            var module = CrearModulo();
            module.enabled = false;
            var sut = CrearGuarda();
            sut.Hold(until: 10f);

            sut.Tick(now: 10f);

            Assert.That(module.enabled, Is.False);
        }

        [Test]
        public void InputGuard_RF45_DEFRC309_UnaCargaDentroDeLaVentanaLaAmplia()
        {
            var module = CrearModulo();
            var sut = CrearGuarda();
            sut.Hold(until: 10f);
            sut.Hold(until: 20f);

            sut.Tick(now: 15f);

            Assert.That(module.enabled, Is.False);
        }

        [Test]
        public void InputGuard_RF45_DEFRC309_UnaEscenaCargadaAbreLaVentana()
        {
            var module = CrearModulo();
            var sut = CrearGuarda();
            sut.Seconds = 5f;

            sut.OnSceneLoaded(default, LoadSceneMode.Single);

            Assert.That(module.enabled, Is.False);
        }

        [Test]
        public void InputGuard_RF45_DEFRC309_ConCeroSegundosNoHaceNada()
        {
            var module = CrearModulo();
            var sut = CrearGuarda();
            sut.Seconds = 0f;

            sut.OnSceneLoaded(default, LoadSceneMode.Single);

            Assert.That(module.enabled, Is.True);
        }

        private StubInputModule CrearModulo()
        {
            var go = new GameObject("EventSystem(Guarda)");
            _created.Add(go);
            return go.AddComponent<StubInputModule>();
        }

        private InputGuard CrearGuarda()
        {
            var go = new GameObject("InputGuard(Guarda)");
            _created.Add(go);
            return go.AddComponent<InputGuard>();
        }

        private sealed class StubInputModule : BaseInputModule
        {
            public override void Process()
            {
            }
        }
    }
}
