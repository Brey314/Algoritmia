using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// Las tres fases bloqueantes del ensamblaje (RF-40, RF-41, CP-02, INC-30, guion §1.8.3).
    /// Regla pura: sin escena ni cuadros.
    /// </summary>
    public class RaftAssemblyTests
    {
        // Una balsa más chica que la del asset —dos troncos, dos amarres— a propósito: si la regla
        // contara «cinco» en vez de leer los espacios del contenido, estas pruebas fallarían (RNF-18).
        internal static RaftSlot[] Slots() => new[]
        {
            new RaftSlot("tronco_1", RaftPhase.Base, MaterialKind.Logs),
            new RaftSlot("tronco_2", RaftPhase.Base, MaterialKind.Logs),
            new RaftSlot("amarre_1", RaftPhase.Lashing, MaterialKind.Ropes),
            new RaftSlot("amarre_2", RaftPhase.Lashing, MaterialKind.Ropes),
            new RaftSlot("mastil", RaftPhase.MastAndSail, MaterialKind.Mast),
            new RaftSlot("vela", RaftPhase.MastAndSail, MaterialKind.Cloth)
        };

        internal static Dictionary<MaterialKind, int> Supply() => new Dictionary<MaterialKind, int>
        {
            [MaterialKind.Logs] = 2, [MaterialKind.Ropes] = 2, [MaterialKind.Mast] = 1, [MaterialKind.Cloth] = 1
        };

        internal static RaftAssembly Sut(RaftAssemblyContent content = null) =>
            new RaftAssembly(content ?? RaftAssemblyContent.Create(Slots(), knotsPerRope: 2), Supply());

        /// <summary>Arma la base correctamente y la confirma.</summary>
        internal static void BuildBase(RaftAssembly sut)
        {
            sut.Place("tronco_1", MaterialKind.Logs);
            sut.Place("tronco_2", MaterialKind.Logs);
            Assume.That(sut.Confirm().Passed, Is.True);
        }

        internal static void BuildLashing(RaftAssembly sut)
        {
            sut.Place("amarre_1", MaterialKind.Ropes);
            sut.Place("amarre_2", MaterialKind.Ropes);
            Assume.That(sut.Confirm().Passed, Is.True);
        }

        [Test]
        public void RaftAssembly_RF40_LasTresFasesSeHabilitanEnOrdenYNoAntes()
        {
            var sut = Sut();

            Assert.That(sut.ActivePhase, Is.EqualTo(RaftPhase.Base), "abre con la base");
            Assert.That(sut.Place("amarre_1", MaterialKind.Ropes).Accepted, Is.False, "el amarre no admite nada todavía");
            Assert.That(sut.Place("mastil", MaterialKind.Mast).Accepted, Is.False, "ni el mástil");

            BuildBase(sut);
            Assert.That(sut.ActivePhase, Is.EqualTo(RaftPhase.Lashing), "confirmada la base se habilita el amarre");
            Assert.That(sut.Place("mastil", MaterialKind.Mast).Accepted, Is.False, "el mástil sigue cerrado");

            BuildLashing(sut);
            Assert.That(sut.ActivePhase, Is.EqualTo(RaftPhase.MastAndSail), "y después el mástil y la vela");
            Assert.That(sut.IsComplete, Is.False, "que todavía no está resuelta");

            sut.Place("mastil", MaterialKind.Mast);
            sut.Place("vela", MaterialKind.Cloth);
            Assert.That(sut.Confirm().Passed, Is.True);
            Assert.That(sut.IsComplete, Is.True, "con las tres confirmadas la balsa está lista (RF-42)");
        }

        [Test]
        public void RaftAssembly_RF40_LosEspaciosDeUnaFaseNoSonAccesiblesAntesDeHabilitarse()
        {
            var sut = Sut();
            var content = RaftAssemblyContent.Create(Slots());

            Assert.That(sut.VisibleSlots.Select(slot => slot.Id), Is.EquivalentTo(new[] { "tronco_1", "tronco_2" }),
                "solo se ven los espacios de la base: nada de la fase siguiente (guion §1.8.3)");
            Assert.That(content.Slots.Where(slot => slot.Phase != RaftPhase.Base).All(slot => !sut.IsVisible(slot) && !sut.IsOpen(slot)),
                Is.True, "los demás ni se ven ni se tocan");

            BuildBase(sut);
            Assert.That(sut.VisibleSlots.Select(slot => slot.Id),
                Is.EquivalentTo(new[] { "tronco_1", "tronco_2", "amarre_1", "amarre_2" }),
                "la base consolidada se sigue viendo debajo del amarre (HU-12)");
            Assert.That(sut.IsOpen(content.Slots.First(slot => slot.Id == "tronco_1")), Is.False,
                "pero ya no se toca: lo confirmado es fijo (RF-41)");
            Assert.That(sut.TakeBack("tronco_1").Accepted, Is.False, "ni se puede sacar");
        }

        [Test]
        public void RaftAssembly_RF41_UnaFaseConfirmadaNoSePierdeEnIntentosPosteriores()
        {
            var sut = Sut();
            BuildBase(sut);

            // Un amarre mal (la tela) y otro vacío, y confirmar: la fase se rechaza.
            sut.Place("amarre_1", MaterialKind.Cloth);
            var rechazo = sut.Confirm();

            Assert.That(rechazo.Passed, Is.False);
            Assert.That(sut.ActivePhase, Is.EqualTo(RaftPhase.Lashing), "la fase sigue abierta para otro intento");
            Assert.That(sut.IsConfirmed(RaftPhase.Base), Is.True, "y la base no se perdió (RF-41, CP-02)");
            Assert.That(sut.PlacedIn("tronco_1"), Is.EqualTo(MaterialKind.Logs), "con sus troncos puestos");
            Assert.That(sut.PlacedIn("tronco_2"), Is.EqualTo(MaterialKind.Logs));
            Assert.That(sut.Remaining(MaterialKind.Logs), Is.Zero, "que no volvieron al inventario");
        }

        [Test]
        public void RaftAssembly_INC30_ConfirmarElAmarreMarcaLaTarea3YLaBaseNoMarcaNada()
        {
            var sut = Sut();
            var tasks = new TaskList();

            BuildBase(sut);
            tasks.MarkPhaseConfirmed((int)RaftPhase.Base);
            Assert.That(RiverTask.All.Where(tasks.IsDone), Is.Empty, "confirmar la base no marca tarea alguna (INC-30)");

            BuildLashing(sut);
            Assert.That(tasks.MarkPhaseConfirmed((int)RaftPhase.Lashing), Is.EqualTo(RiverTaskId.AssembleRaft),
                "confirmar el amarre marca «Ensamblar la balsa»");

            Assert.That((int)RaftPhase.MastAndSail, Is.EqualTo(3), "y la fase 3 es la que marca la tarea 4");
            Assert.That(RiverTask.ForConfirmedPhase((int)RaftPhase.MastAndSail), Is.EqualTo(RiverTaskId.PlaceMastAndSail));
        }

        [Test]
        public void RaftAssembly_CP02_NoHayLimiteDeIntentosNiPantallaDeDerrota()
        {
            var sut = Sut();

            for (var attempt = 0; attempt < 25; attempt++)
            {
                sut.Place("tronco_1", MaterialKind.Mast); // El tronco largo entra en la base: cabe, y se rechaza al confirmar.
                var result = sut.Confirm();
                Assert.That(result.Passed, Is.False);
                Assert.That(result.WrongSlotIds, Is.EquivalentTo(new[] { "tronco_1", "tronco_2" }));
                Assert.That(sut.Remaining(MaterialKind.Mast), Is.EqualTo(1), "el mástil vuelve al inventario cada vez");
            }

            Assert.That(sut.Rejections, Is.EqualTo(25), "los rechazos se cuentan para el informe docente (RF-45)…");
            Assert.That(sut.ActivePhase, Is.EqualTo(RaftPhase.Base), "…pero la fase sigue abierta: sin límite ni derrota (CP-02)");
            Assert.That(typeof(RaftAssembly).GetProperties().Select(property => property.Name),
                Has.None.Match("(?i)fail|lose|derrota|gameover"), "no existe estado de derrota");

            BuildBase(sut);
            Assert.That(sut.IsConfirmed(RaftPhase.Base), Is.True, "después de todo eso se confirma igual");
        }

        [Test]
        public void RaftAssembly_RNF14_RetomarEnUnaFaseConsolidaLasAnterioresConSusPiezas()
        {
            var sut = Sut();

            sut.Resume(RaftPhase.MastAndSail);

            Assert.That(sut.ActivePhase, Is.EqualTo(RaftPhase.MastAndSail));
            Assert.That(sut.IsConfirmed(RaftPhase.Base) && sut.IsConfirmed(RaftPhase.Lashing), Is.True, "las dos anteriores confirmadas");
            Assert.That(new[] { "tronco_1", "tronco_2", "amarre_1", "amarre_2" }.Select(sut.PlacedIn),
                Is.EqualTo(new MaterialKind?[] { MaterialKind.Logs, MaterialKind.Logs, MaterialKind.Ropes, MaterialKind.Ropes }),
                "con sus piezas puestas");
            Assert.That(sut.Remaining(MaterialKind.Logs) + sut.Remaining(MaterialKind.Ropes), Is.Zero, "y descontadas del inventario");
            Assert.That(sut.Remaining(MaterialKind.Mast) + sut.Remaining(MaterialKind.Cloth), Is.EqualTo(2), "queda lo de la fase abierta");
        }

        [Test]
        public void RaftAssembly_RF38_ElRolloDeSogaDaTantosAmarresComoDiceElAsset()
        {
            var config = RiverLevelConfig.Create(new[]
            {
                new Collectible("tronco_a", MaterialKind.Logs, "Troncos"),
                new Collectible("tronco_b", MaterialKind.Logs, "Troncos"),
                new Collectible("sogas", MaterialKind.Ropes, "Sogas"),
                new Collectible("tela", MaterialKind.Cloth, "Tela"),
                new Collectible("mastil", MaterialKind.Mast, "Mástil")
            });
            var inventory = new Inventory(config);
            foreach (var item in config.Collectibles)
            {
                inventory.TryCollect(item);
            }

            var supply = RaftAssembly.SupplyFrom(inventory, RaftAssemblyContent.Create(Slots(), knotsPerRope: 7));

            Assert.That(supply[MaterialKind.Logs], Is.EqualTo(2), "cada tronco recogido es una pieza");
            Assert.That(supply[MaterialKind.Ropes], Is.EqualTo(7), "el rollo se arrastra una vez por amarre (RNF-18)");
            Assert.That(supply[MaterialKind.Cloth], Is.EqualTo(1));
            Assert.That(supply[MaterialKind.Mast], Is.EqualTo(1));
        }
    }
}
