using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// La escena 3.2, condicional al primer fallo (guion §1.8.4.1, RF-05, CP-02, CP-07), y la
    /// salida al cruce con la balsa aprobada. Con un <see cref="GameFlowRunner"/> de prueba y sin
    /// <c>SceneLoader</c>: la FSM cambia de estado y la escena se queda, que es lo que permite
    /// mirar el estado del ensamblaje justo después de salir.
    /// </summary>
    [Category("Integration")]
    public class ConditionalNarrativeTests
    {
        private const string Nombre = "PruebaRio";

        [SetUp]
        public void SetUp() => AssemblyPanelController.ForgetLevelMemory();

        [TearDown]
        public void TearDown()
        {
            AssemblyPanelController.ForgetLevelMemory();
            foreach (var runner in Object.FindObjectsByType<GameFlowRunner>(FindObjectsInactive.Include))
            {
                runner.Session.Delete(Nombre);
                Object.DestroyImmediate(runner.gameObject);
            }
        }

        [Test]
        [Timeout(60000)]
        public async Task RiverScene_Guion841_LaEscena32SeReproduceSoloTrasElPrimerFallo()
        {
            var runner = await StartAt(RaftPhase.MastAndSail);
            var (_, panel) = await AssemblyPanelTests.OpenAssembly();
            var (mastil, vela) = MastAndSail(panel);

            await AssemblyPanelTests.Drag(panel, MaterialKind.Cloth, mastil);
            await AssemblyPanelTests.Drag(panel, MaterialKind.Mast, vela);
            await AssemblyPanelTests.Confirm(panel);

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative), "tras el primer fallo se sale a la narrativa");
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo(panel.Content.FirstFailureSequenceId), "a la escena 3.2");

            // Segundo fallo, sin escena de por medio: la 3.2 no se repite.
            runner.StartPlaying(LevelId.River, 3);
            await AssemblyPanelTests.Drag(panel, MaterialKind.Cloth, mastil);
            await AssemblyPanelTests.Drag(panel, MaterialKind.Mast, vela);
            await AssemblyPanelTests.Confirm(panel);

            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing), "el segundo fallo se queda en el juego");
            Assert.That(panel.Assembly.ActivePhase, Is.EqualTo(RaftPhase.MastAndSail), "con la fase abierta para otro intento (CP-02)");
        }

        [Test]
        [Timeout(60000)]
        public async Task RiverScene_Guion841_AcertarAlPrimerIntentoVaDirectoAlCruce()
        {
            var runner = await StartAt(RaftPhase.MastAndSail);
            var (river, panel) = await AssemblyPanelTests.OpenAssembly();
            var (mastil, vela) = MastAndSail(panel);

            await AssemblyPanelTests.Drag(panel, MaterialKind.Mast, mastil);
            await AssemblyPanelTests.Drag(panel, MaterialKind.Cloth, vela);
            await AssemblyPanelTests.Confirm(panel);

            Assert.That(panel.Assembly.IsComplete, Is.True,
                $"guía: «{river.MessageLabel.text}» · mástil={panel.Assembly.PlacedIn("mastil")} vela={panel.Assembly.PlacedIn("vela")}");
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative));
            Assert.That(runner.Flow.NarrativeSequenceId, Is.EqualTo(panel.Content.ClosingSequenceId), "directo al cruce, sin la 3.2");
            Assert.That(runner.Flow.ActiveProfile.IsPhaseConfirmed(new PhaseId(LevelId.River, 3)), Is.True, "la fase 3 quedó confirmada (RF-04)");
            Assert.That(runner.Flow.ActiveProfile.IsLevelComplete(LevelId.River), Is.True);
        }

        [Test]
        [Timeout(60000)]
        public async Task RiverScene_CP02_LaEscena32NoReiniciaElEnsamblaje()
        {
            var runner = await StartAt(RaftPhase.MastAndSail);
            var (_, panel) = await AssemblyPanelTests.OpenAssembly();
            var (mastil, vela) = MastAndSail(panel);

            // El mástil bien y la tela en ningún sitio: falla por el espacio vacío.
            await AssemblyPanelTests.Drag(panel, MaterialKind.Mast, mastil);
            await AssemblyPanelTests.Confirm(panel);
            Assume.That(runner.Flow.Current, Is.EqualTo(GameState.Narrative));

            // Vuelve de la 3.2: la escena se recarga en la fase 3 y el mástil sigue puesto.
            runner.StartPlaying(LevelId.River, 3);
            var (river, reopened) = await AssemblyPanelTests.OpenAssembly();

            Assert.That(reopened.Assembly.ActivePhase, Is.EqualTo(RaftPhase.MastAndSail));
            Assert.That(reopened.Assembly.IsConfirmed(RaftPhase.Base) && reopened.Assembly.IsConfirmed(RaftPhase.Lashing), Is.True,
                "base y amarre consolidados (RF-43)");
            var mastilId = reopened.Content.Slots.Single(slot => slot.Accepts == MaterialKind.Mast).Id;
            Assert.That(reopened.Assembly.PlacedIn(mastilId), Is.EqualTo(MaterialKind.Mast), "lo bien puesto sigue puesto: la escena narra, no reinicia (CP-02)");
            Assert.That(reopened.Assembly.Remaining(MaterialKind.Cloth), Is.EqualTo(1), "y la tela espera en el inventario");
            Assert.That(river.Tasks.IsDone(RiverTaskId.AssembleRaft), Is.True, "la tarea 3 sigue marcada");
            Assert.That(river.Pads.All(pad => !pad.gameObject.activeSelf), Is.True, "no se vuelve a la orilla");
        }

        /// <summary>Un runner de prueba con un perfil que tiene el Nivel 3 abierto y las fases anteriores confirmadas.</summary>
        internal static async Task<GameFlowRunner> StartAt(RaftPhase phase, string nombre = Nombre)
        {
            var runner = new GameObject("TestRunner").AddComponent<GameFlowRunner>();
            await Awaitable.NextFrameAsync(); // GameFlowRunner.Start() navega solo a MainMenu
            runner.Session.Delete(nombre);

            runner.GoTo(GameState.ProfileSelect);
            var profile = PlayerProfile.Create(nombre, Array.Empty<string>()).Profile;
            profile.Reach(LevelId.River);
            for (var previous = RaftPhase.Base; previous < phase; previous++)
            {
                profile.ConfirmPhase(new PhaseId(LevelId.River, (int)previous), default);
            }

            runner.SelectProfile(profile);
            Assert.That(runner.StartPlaying(LevelId.River, (int)phase), Is.True);
            Assert.That(runner.Flow.Current, Is.EqualTo(GameState.Playing), "el flujo no llegó a Playing: el arreglo de la prueba está roto");
            return runner;
        }

        private static (RectTransform Mastil, RectTransform Vela) MastAndSail(AssemblyPanelController panel) => (
            panel.Slots.Values.Single(entry => entry.Slot.Accepts == MaterialKind.Mast).Image.rectTransform,
            panel.Slots.Values.Single(entry => entry.Slot.Accepts == MaterialKind.Cloth).Image.rectTransform);
    }
}
