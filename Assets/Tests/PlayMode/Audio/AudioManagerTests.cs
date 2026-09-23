using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Audio.Tests
{
    [Category("Integration")]
    public class AudioManagerTests
    {
        private AudioManager _audio;

        [SetUp]
        public void CrearElGestor()
        {
            foreach (var previous in Object.FindObjectsByType<AudioManager>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(previous.gameObject);
            }

            _audio = new GameObject("TestAudio").AddComponent<AudioManager>();
        }

        [TearDown]
        public void DestruirElGestor() => Object.DestroyImmediate(_audio.gameObject);

        /// <summary>
        /// Las tres escenas del Nivel 1 se encadenan (RF-05) y comparten la cueva: la segunda pide
        /// el mismo ambiente que dejó la primera y no puede haber costura ni reinicio.
        /// </summary>
        [Test]
        [Timeout(10000)]
        public async Task AudioManager_RF05_PedirElAmbienteQueYaSuenaNoLoReinicia()
        {
            var cueva = Clip("amb_n1_cueva_oscura");
            _audio.PlayAmbient(cueva);
            await Awaitable.NextFrameAsync();

            _audio.PlayAmbient(cueva);

            Assert.That(_audio.AmbientStarts, Is.EqualTo(1), "el mismo clip sigue sonando, no arranca de cero");
            Assert.That(_audio.AmbientClip, Is.SameAs(cueva));
        }

        /// <summary>
        /// S1 del guion §3.1: «la pantalla queda completamente negra» y el sonido con ella. Es un
        /// corte seco, no el fundido de las demás transiciones (Direccion_de_Musica_y_Sonido.md §5).
        /// </summary>
        [Test]
        public void AudioManager_RF05_ElCorteSecoCallaElAmbienteAlInstante()
        {
            _audio.PlayAmbient(Clip("amb_noche_intemperie"));

            _audio.CutToSilence();

            Assert.That(_audio.AmbientClip, Is.Null, "sin fundido: el negro y el silencio llegan juntos");
        }

        /// <summary>S2 del guion §4.1: Algoritm se apaga, «la oscuridad vuelve de golpe» — y queda la cueva.</summary>
        [Test]
        public void AudioManager_RF05_ElCorteDeMusicaConservaElAmbiente()
        {
            var cueva = Clip("amb_n1_cueva_oscura");
            _audio.PlayAmbient(cueva);
            _audio.PlayMusic(Clip("mus_n1_cueva_loop"));

            _audio.CutToSilence(keepAmbient: true);

            Assert.That(_audio.AmbientClip, Is.SameAs(cueva), "el ambiente de la cueva es lo único que sigue");
        }

        /// <summary>
        /// Un efecto tiene que oírse: <c>PlayOneShot</c> escala el volumen de su fuente, y una
        /// fuente a cero lo deja mudo aunque el gestor registre que sonó (21/09/2026: golpe y
        /// soplo mudos en el nivel con todas las pruebas de «qué sonó» en verde).
        /// </summary>
        [Test]
        public void AudioManager_RF11_UnEfectoSeReproduceConVolumenAudible()
        {
            _audio.PlaySfx(Clip("sfx_n1_golpe"));

            var oneShotSources = _audio.GetComponents<AudioSource>().Where(source => !source.loop).ToArray();
            Assert.That(oneShotSources, Is.Not.Empty, "hay fuentes de un disparo");
            Assert.That(oneShotSources.Select(source => source.volume), Has.All.GreaterThan(0f),
                "ninguna fuente de efectos o voz está a cero: PlayOneShot multiplica por ese volumen");
        }

        private static AudioClip Clip(string name) => AudioClip.Create(name, 44100, 1, 44100, false);
    }
}
