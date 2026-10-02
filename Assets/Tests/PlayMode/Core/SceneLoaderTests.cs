using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

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
            // Se espera a que el cronómetro quede anotado —el efecto de `completed`—, no a que
            // «MainMenu» esté activa: si ya lo estaba (otra prueba la dejó cargada) esa condición
            // se cumple durante toda la recarga y la aserción correría con el dato aún en cero.
            await WaitUntil(() => sut.LastLoadSeconds > 0f);
            var observedSeconds = Time.realtimeSinceStartupAsDouble - startedAt;

            Assert.That(sut.LastLoadSeconds,
                Is.GreaterThan(0.001).And.LessThanOrEqualTo(observedSeconds));
        }

        [Test]
        [Category("Acceptance")]
        [Timeout(30000)]
        public async Task SceneLoader_RNF04_CadaCargaDejaSuTiempoEnElRegistro()
        {
            // «Por qué no» LogAssert.Expect: pide el mensaje antes de que salga, y el tiempo de una
            // carga solo se sabe después; con una expresión regular no se comprobaría que el número
            // sea el de esa carga. Se escucha el mismo canal que escribe Player.log.
            var registro = new List<string>();
            Application.LogCallback anotar = (mensaje, _, tipo) =>
            {
                if (tipo == LogType.Log && mensaje.StartsWith("RNF-04: ", System.StringComparison.Ordinal))
                {
                    registro.Add(mensaje);
                }
            };

            // Con coma decimal, como un Windows en es-CO: la línea tiene que salir igual, porque la
            // lee el arnés del ejecutable.
            var culturaDelEquipo = CultureInfo.CurrentCulture;
            var conComa = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            conComa.NumberFormat.NumberDecimalSeparator = ",";
            CultureInfo.CurrentCulture = conComa;
            Application.logMessageReceived += anotar;
            try
            {
                var sut = new GameObject(nameof(SceneLoader)).AddComponent<SceneLoader>();

                sut.Load("MainMenu");
                await WaitUntil(() => sut.LastLoadSeconds > 0f);
                Assert.That(registro, Is.EqualTo(new[] { Linea("MainMenu", sut.LastLoadSeconds) }),
                    "la primera carga deja su línea");

                sut.Load("Credits");
                await WaitUntil(() => registro.Count == 2);
                Assert.That(registro[1], Is.EqualTo(Linea("Credits", sut.LastLoadSeconds)),
                    "y la segunda, la suya");
            }
            finally
            {
                Application.logMessageReceived -= anotar;
                CultureInfo.CurrentCulture = culturaDelEquipo;
            }
        }

        /// <summary>
        /// Con fundido, la pantalla se va a negro **antes** de cargar y vuelve **después**: la
        /// escena nueva nunca aparece de golpe ni la vieja se corta sin cerrar.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task SceneLoader_RF05_ConFundidoCargaEnNegroYVuelveALaImagen()
        {
            var sut = new GameObject(nameof(SceneLoader)).AddComponent<SceneLoader>();
            sut.FadeSeconds = 0.2f;

            sut.Load("MainMenu", fade: true);
            Assert.That(sut.IsFading, Is.True, "el fundido arranca al pedir la carga");

            await WaitUntil(() => sut.FadeAlpha >= 1f || sut.LastLoadSeconds > 0f);
            Assert.That(sut.LastLoadSeconds, Is.EqualTo(0f), "la pantalla llega a negro antes de que cargue la escena");

            await WaitUntil(() => sut.LastLoadSeconds > 0f);
            Assert.That(sut.FadeAlpha, Is.GreaterThan(0f), "la escena carga con la pantalla todavía en negro");

            await WaitUntil(() => !sut.IsFading);
            Assert.That(sut.FadeAlpha, Is.EqualTo(0f), "y al terminar la imagen vuelve entera");
        }

        [Test]
        [Timeout(30000)]
        public async Task SceneLoader_RF05_SinFundidoNoOscureceNada()
        {
            var sut = new GameObject(nameof(SceneLoader)).AddComponent<SceneLoader>();

            sut.Load("MainMenu");
            Assert.That(sut.IsFading, Is.False);
            await WaitUntil(() => sut.LastLoadSeconds > 0f);
            Assert.That(sut.FadeAlpha, Is.EqualTo(0f), "los menús cortan en seco");
        }

        // La línea que lee el arnés del ejecutable: formato fijo, cultura invariante.
        private static string Linea(string escena, float segundos) =>
            System.FormattableString.Invariant($"RNF-04: «{escena}» cargó en {segundos:0.000} s");

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
