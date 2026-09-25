using System.Linq;
using System.Threading.Tasks;
using Game.Audio;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// Qué suena en el Nivel 3 (Direccion_de_Musica_y_Sonido.md §8 y §13). Se comprueba **qué**
    /// pieza se disparó y cuántas veces, no cómo suena: eso es lo que puede romper una «mejora de
    /// feedback».
    /// </summary>
    [Category("Integration")]
    public class RiverSoundsTests
    {
        private AudioManager _audio;

        [SetUp]
        public void CrearElGestor() => _audio = new GameObject("TestAudio").AddComponent<AudioManager>();

        [TearDown]
        public void DestruirLosObjetosPersistentes()
        {
            Object.DestroyImmediate(_audio.gameObject);
            AssemblyPanelController.ForgetLevelMemory();
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(runner.gameObject);
            }
        }

        /// <summary>El río suena todo el nivel y el bosque encima, en la segunda capa (§8).</summary>
        [Test]
        [Timeout(30000)]
        public async Task RiverScene_RF35_LaOrillaSuenaAlRioConElBosqueDeFondo()
        {
            var river = await RiverMovementTests.OpenRiver();
            var sonidos = river.Sounds;

            // Assert y no Assume: si la escena pierde la referencia el nivel se juega en silencio
            // y esto tiene que fallar, no quedar «inconclusive».
            Assert.That(sonidos, Is.Not.Null, "Level3_River tiene N3_Sonidos asignado en la orilla");
            Assert.That(sonidos.name, Is.EqualTo("N3_Sonidos"), "el del Nivel 3");
            Assert.That(_audio.AmbientClip, Is.SameAs(sonidos.RiverAmbient), "el río de fondo");
            Assert.That(_audio.AmbientClip.name, Is.EqualTo("amb_n3_rio_orilla"), "con la toma de la orilla");
            Assert.That(_audio.AmbientLayerClip, Is.SameAs(sonidos.ForestAmbient), "y el bosque encima");
            Assert.That(_audio.AmbientLayerClip.name, Is.EqualTo("amb_n2_bosque_dia"), "con la toma del bosque de día");
        }

        /// <summary>Recoger un material suena a encaje, una vez por material (RF-37).</summary>
        [Test]
        [Timeout(30000)]
        public async Task RiverScene_RF37_RecogerUnMaterialSuenaAEncajeUnaVez()
        {
            var river = await RiverMovementTests.OpenRiver();
            Assume.That(river.Sounds, Is.Not.Null, "Level3_River tiene N3_Sonidos asignado");
            var (material, _) = river.Spawned.First();
            AssemblyPanelTests.WalkTo(river, material.Position);
            Assume.That(river.Reachable, Is.SameAs(material), "Mamá llegó al material");
            var antes = _audio.SfxCount;

            river.CollectButton.onClick.Invoke();

            Assert.That(_audio.SfxCount - antes, Is.EqualTo(1), "un solo efecto al recoger");
            Assert.That(_audio.LastSfx.name, Is.EqualTo("sfx_encaje_pieza"), "el encaje");
        }

        /// <summary>
        /// Cada pieza que queda puesta en la balsa suena a un martillazo (RF-40); soltarla fuera
        /// de la balsa no suena, porque nada se clavó y no hay sonido de fallo (§2.1, CP-02).
        /// </summary>
        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RF40_ColocarUnaPiezaSuenaUnMartillazoYSoltarlaFueraNada()
        {
            var (river, panel) = await AssemblyPanelTests.OpenAssembly();
            var espacio = panel.Slots.Values.First(entry => entry.Slot.Phase == RaftPhase.Base);
            var fuera = RiverMovementTests.EnPantalla(river.TaskArea).center;
            var antes = _audio.SfxCount;

            panel.Take(MaterialKind.Logs, fuera);
            panel.Release(fuera);
            var alSoltarFuera = _audio.SfxCount - antes;
            await AssemblyPanelTests.Drag(panel, MaterialKind.Logs, espacio.Image.rectTransform);

            Assert.That(panel.Sounds, Is.Not.Null, "Level3_River tiene N3_Sonidos asignado en el panel");
            Assert.That(alSoltarFuera, Is.EqualTo(0), "soltar fuera de la balsa no suena");
            Assert.That(_audio.SfxCount - antes, Is.EqualTo(1), "la pieza puesta suena una vez");
            Assert.That(_audio.LastSfx, Is.SameAs(panel.Sounds.PiecePlaced), "a martillazo");
            Assert.That(_audio.LastSfx.name, Is.EqualTo("sfx_martillo_madera"), "de madera");
        }

        /// <summary>
        /// Aprobar una fase suena a tres martillazos seguidos, ni uno más (RF-41), y el panel no
        /// se suelta hasta el último: la pieza siguiente no se clava encima de ellos (§2.3).
        /// </summary>
        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_RF41_AprobarUnaFaseSuenaTresMartillazosAntesDeSeguir()
        {
            var (_, panel) = await AssemblyPanelTests.OpenAssembly();
            var sonidos = panel.Sounds;
            Assume.That(sonidos, Is.Not.Null, "el panel tiene N3_Sonidos asignado");
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.Base);
            var antes = _audio.SfxCount;

            await AssemblyPanelTests.Confirm(panel);
            var alSoltarse = _audio.SfxCount - antes;
            await EsperarSegundos(sonidos.PhaseHitSeconds * 2f);

            Assert.That(panel.Assembly.ActivePhase, Is.EqualTo(RaftPhase.Lashing), "la base quedó aprobada");
            Assert.That(alSoltarse, Is.EqualTo(3), "con los tres martillazos ya sonados cuando el panel se suelta");
            Assert.That(_audio.SfxCount - antes, Is.EqualTo(3), "y ni uno más después");
            Assert.That(_audio.LastSfx, Is.SameAs(sonidos.PhaseHammer), "todos de martillo");
            Assert.That(_audio.LastSfx.name, Is.EqualTo("sfx_martillo_madera"), "de madera");
        }

        /// <summary>
        /// Una fase que no pasa no suena (§2.1, CP-02): la pieza mal puesta vuelve al inventario en
        /// silencio y la familia anima. Solo lo aprobado se clava.
        /// </summary>
        [Test]
        [Timeout(60000)]
        public async Task AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena()
        {
            var (_, panel) = await AssemblyPanelTests.OpenAssembly();
            Assume.That(panel.Sounds, Is.Not.Null, "el panel tiene N3_Sonidos asignado");
            var espacios = panel.Slots.Values.Where(entry => entry.Slot.Phase == RaftPhase.Base).ToArray();
            await AssemblyPanelTests.Drag(panel, MaterialKind.Mast, espacios[0].Image.rectTransform);
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.Base);
            var antes = _audio.SfxCount;

            await AssemblyPanelTests.Confirm(panel);
            await EsperarSegundos(panel.Sounds.PhaseHits * panel.Sounds.PhaseHitSeconds);

            Assert.That(panel.Assembly.ActivePhase, Is.EqualTo(RaftPhase.Base), "la fase sigue abierta");
            Assert.That(_audio.SfxCount, Is.EqualTo(antes), "y nada sonó");
        }

        /// <summary>
        /// La balsa terminada suena a pieza tomada **después** de los tres martillazos de la última
        /// fase, no encima (§2.3), y antes de salir al cruce.
        /// </summary>
        [Test]
        [Timeout(90000)]
        public async Task AssemblyPanel_RF44_LaBalsaTerminadaSuenaAPiezaTomadaTrasLosMartillazos()
        {
            var (_, panel) = await AssemblyPanelTests.OpenAssembly();
            var sonidos = panel.Sounds;
            Assume.That(sonidos, Is.Not.Null, "el panel tiene N3_Sonidos asignado");
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.Base);
            await AssemblyPanelTests.Confirm(panel);
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.Lashing);
            await AssemblyPanelTests.Confirm(panel);
            await AssemblyPanelTests.FillPhase(panel, RaftPhase.MastAndSail);
            Assume.That(sonidos.PhaseHitSeconds, Is.GreaterThan(0f), "hay pausa entre golpes");
            var antes = _audio.SfxCount;

            panel.ConfirmButton.onClick.Invoke();
            var (ultimoGolpe, terminada) = await Instantes(panel, antes, sonidos.PhaseHits);

            Assert.That(panel.Assembly.IsComplete, Is.True, "la balsa está terminada");
            Assert.That(_audio.SfxCount - antes, Is.EqualTo(sonidos.PhaseHits + 1), "tres martillazos y la balsa terminada");
            Assert.That(terminada - ultimoGolpe, Is.GreaterThanOrEqualTo(sonidos.PhaseHitSeconds * 0.8f),
                "la balsa terminada suena un golpe después del último martillazo, no encima (§2.3)");
            Assert.That(_audio.LastSfx, Is.SameAs(sonidos.RaftBuilt), "la última, la balsa terminada");
            Assert.That(_audio.LastSfx.name, Is.EqualTo("sfx_n1_pieza_tomar"), "con la pieza tomada");
        }

        /// <summary>
        /// Cuadro a cuadro mientras el panel trabaja: cuándo sonó el efecto número
        /// <paramref name="golpes"/> contado desde <paramref name="antes"/>, y cuándo el siguiente.
        /// En tiempo de juego, el mismo reloj con el que el panel espera.
        /// </summary>
        private async Task<(float UltimoGolpe, float Siguiente)> Instantes(AssemblyPanelController panel, int antes, int golpes)
        {
            var ultimoGolpe = float.NaN;
            var siguiente = float.NaN;
            var fin = Time.realtimeSinceStartup + 10f;
            while ((panel.IsBusy || float.IsNaN(siguiente)) && Time.realtimeSinceStartup < fin)
            {
                var sonados = _audio.SfxCount - antes;
                if (sonados >= golpes && float.IsNaN(ultimoGolpe))
                {
                    ultimoGolpe = Time.time;
                }

                if (sonados >= golpes + 1 && float.IsNaN(siguiente))
                {
                    siguiente = Time.time;
                }

                await Awaitable.NextFrameAsync();
            }

            Assert.That(panel.IsBusy, Is.False, "la animación del panel terminó");
            return (ultimoGolpe, siguiente);
        }

        private static async Task EsperarSegundos(float segundos)
        {
            var fin = Time.realtimeSinceStartup + segundos;
            while (Time.realtimeSinceStartup < fin)
            {
                await Awaitable.NextFrameAsync();
            }
        }
    }
}
