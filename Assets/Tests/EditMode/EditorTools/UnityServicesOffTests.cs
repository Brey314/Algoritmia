using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Build;

namespace Game.EditorTools.Tests
{
    /// <summary>
    /// Ningún servicio de Unity viaja en el ejecutable (RNF-10, DEF-SPIKE-01): el build apaga a la
    /// fuerza los interruptores y falla si alguno no se apaga o si el juego compilado nombra un servidor.
    /// </summary>
    public class UnityServicesOffTests
    {
        private string root;

        [SetUp]
        public void SetUp() =>
            root = Path.Combine(Path.GetTempPath(), "services-" + Path.GetRandomFileName());

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }

        /// <summary>Interruptor simulado: cuenta las escrituras y puede negarse a apagarse.</summary>
        private sealed class Fake
        {
            public bool On;
            public int Writes;
            public bool Stubborn;

            public UnityServicesOff.Switch AsSwitch(string name) => new UnityServicesOff.Switch(name,
                () => On,
                v =>
                {
                    Writes++;
                    if (!Stubborn) On = v;
                });
        }

        private static UnityServicesOff.Switch Throwing(string name, string message) =>
            new UnityServicesOff.Switch(name, () => throw new InvalidOperationException(message), _ => { });

        private string DataDir(string contents)
        {
            var dir = Path.Combine(root, "Algoritmia_Data");
            Directory.CreateDirectory(dir);
            File.WriteAllBytes(Path.Combine(dir, "globalgamemanagers"), System.Text.Encoding.ASCII.GetBytes(contents));
            return dir;
        }

        [Test]
        public void UnityServicesOff_RNF10_ApagaLosServiciosQueEstabanEncendidos()
        {
            var analytics = new Fake { On = true };
            var insights = new Fake { On = true };
            var purchasing = new Fake { On = false };

            var (turnedOff, stuck) = UnityServicesOff.ForceOff(new[]
            {
                analytics.AsSwitch("Analytics"), insights.AsSwitch("Insights"), purchasing.AsSwitch("Purchasing")
            });

            Assert.That(analytics.On, Is.False);
            Assert.That(insights.On, Is.False);
            Assert.That(turnedOff, Is.EquivalentTo(new[] { "Analytics", "Insights" }));
            Assert.That(stuck, Is.Empty);
        }

        [Test]
        public void UnityServicesOff_RNF10_NoEscribeEnLosQueYaEstabanApagados()
        {
            var apagado = new Fake { On = false };

            UnityServicesOff.ForceOff(new[] { apagado.AsSwitch("Purchasing") });

            // Escribir un valor igual marcaría los ajustes del proyecto como modificados en cada build.
            Assert.That(apagado.Writes, Is.EqualTo(0));
        }

        [Test]
        public void UnityServicesOff_RNF10_ElBuildFallaConElNombreDelServicioQueNoSeApaga()
        {
            var stubborn = new Fake { On = true, Stubborn = true };
            var docile = new Fake { On = true };

            var error = Assert.Throws<BuildFailedException>(() =>
                UnityServicesOff.Enforce(new[] { stubborn.AsSwitch("Insights"), docile.AsSwitch("Analytics") }));

            Assert.That(error.Message, Does.Contain("Insights"));
            Assert.That(error.Message, Does.Not.Contain("Analytics"), "solo se nombra lo que no se pudo apagar");
            Assert.That(docile.On, Is.False, "los demás se apagan igual: el aviso es completo, no del primero");
        }

        [Test]
        public void UnityServicesOff_RNF10_ElBuildFallaSiUnInterruptorNoSePuedeLeer()
        {
            // Si Unity renombra una propiedad interna, pasar de largo dejaría el servicio encendido.
            var error = Assert.Throws<BuildFailedException>(() =>
                UnityServicesOff.Enforce(new[] { Throwing("UnityConnectSettings.enabled", "no existe en este Unity") }));

            Assert.That(error.Message, Does.Contain("UnityConnectSettings.enabled"));
            Assert.That(error.Message, Does.Contain("no existe en este Unity"));
        }

        [Test]
        public void UnityServicesOff_RNF10_ElBuildSigueSiTodoSeApaga()
        {
            var analytics = new Fake { On = true };

            var turnedOff = UnityServicesOff.Enforce(new[] { analytics.AsSwitch("Analytics") });

            Assert.That(turnedOff, Is.EqualTo(new[] { "Analytics" }));
        }

        [Test]
        public void UnityServicesOff_RNF10_LosInterruptoresRealesDeEstaVersionSeApaganTodos()
        {
            // Idempotente: deja el Editor en el estado que el build exige de todos modos.
            var switches = UnityServicesOff.RealSwitches();

            var (_, stuck) = UnityServicesOff.ForceOff(switches);

            Assert.That(stuck, Is.Empty, "algún interruptor de Unity no existe o no se deja apagar en esta versión");
            Assert.That(switches.Where(s => s.Get()).Select(s => s.Name), Is.Empty);
            Assert.That(switches.Select(s => s.Name).Distinct().Count(), Is.EqualTo(switches.Count));
        }

        [Test]
        public void UnityServicesOff_RNF10_ElInterruptorGeneralDeUnityConnectEstaEntreLosVigilados()
        {
            // Es el que dejó el rc1 conectado a Internet con Analytics ya apagado (DEF-SPIKE-01).
            Assert.That(UnityServicesOff.RealSwitches().Select(s => s.Name),
                Has.Some.Contains("UnityConnectSettings.enabled"));
        }

        [Test]
        public void UnityServicesOff_RNF10_ElJuegoCompiladoLimpioPasaLaVerificacion()
        {
            Assert.DoesNotThrow(() => UnityServicesOff.Verify(DataDir("globalgamemanagers sin servidores de Unity")));
        }

        [Test]
        public void UnityServicesOff_RNF10_ElJuegoCompiladoQueNombraUnServidorDeUnityFalla()
        {
            var dir = DataDir("\0\0https://cdp.cloud.unity3d.com/v1/events\0\0");

            var error = Assert.Throws<BuildFailedException>(() => UnityServicesOff.Verify(dir));

            Assert.That(error.Message, Does.Contain(UnityServicesOff.CloudHost));
        }

        [Test]
        public void UnityServicesOff_RNF10_SiNoSePuedeVerificarElJuegoCompiladoElBuildFalla()
        {
            // Un cambio en la disposición de la carpeta de datos no puede convertir la guarda en un pase.
            Directory.CreateDirectory(root);

            Assert.Throws<BuildFailedException>(() => UnityServicesOff.Verify(root));
        }
    }
}
