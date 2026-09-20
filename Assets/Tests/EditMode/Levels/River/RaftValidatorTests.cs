using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// La prueba de la balsa y su depuración (RF-42, RF-43, CP-06, RF-17, guion §1.8.4): qué se
    /// señala, qué vuelve al inventario y qué dice el mensaje.
    /// </summary>
    public class RaftValidatorTests
    {
        private static RaftAssemblyContent Asset() => AssetDatabase.FindAssets($"t:{nameof(RaftAssemblyContent)}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<RaftAssemblyContent>)
            .Single(content => content.name == "N3_RaftAssemblyContent");

        [Test]
        public void RaftValidator_RF42_IdentificaElEspacioIncorrectoNoElEnsamblajeCompleto()
        {
            var slots = RaftAssemblyTests.Slots().Where(slot => slot.Phase == RaftPhase.MastAndSail).ToArray();

            var wrong = RaftValidator.WrongSlots(slots, id => id == "mastil" ? MaterialKind.Cloth : MaterialKind.Mast);

            Assert.That(wrong, Is.EquivalentTo(new[] { "mastil", "vela" }), "los dos intercambiados, por su espacio");

            var oneWrong = RaftValidator.WrongSlots(slots, id => id == "mastil" ? MaterialKind.Mast : (MaterialKind?)null);
            Assert.That(oneWrong, Is.EqualTo(new[] { "vela" }), "un espacio vacío se señala; el correcto no (RF-42, HU-13)");

            Assert.That(RaftValidator.WrongSlots(slots, id => id == "mastil" ? MaterialKind.Mast : MaterialKind.Cloth), Is.Empty);
        }

        [Test]
        public void RaftValidator_RF43_SoloLasPiezasMalUbicadasRegresanAlInventario()
        {
            var sut = RaftAssemblyTests.Sut();
            RaftAssemblyTests.BuildBase(sut);
            RaftAssemblyTests.BuildLashing(sut);
            // El mástil bien puesto y la vela vacía: lo único mal es el espacio vacío.
            sut.Place("mastil", MaterialKind.Mast);
            var result = sut.Confirm();

            Assert.That(result.Passed, Is.False);
            Assert.That(result.WrongSlotIds, Is.EqualTo(new[] { "vela" }));
            Assert.That(sut.PlacedIn("mastil"), Is.EqualTo(MaterialKind.Mast), "el mástil bien puesto se queda (RF-43)");
            Assert.That(sut.Remaining(MaterialKind.Mast), Is.Zero, "y no vuelve al inventario");
            Assert.That(sut.Remaining(MaterialKind.Cloth), Is.EqualTo(1), "la tela sí está disponible para el nuevo intento");

            // Ahora las dos intercambiadas: vuelven las dos, y solo las dos.
            sut.TakeBack("mastil");
            sut.Place("mastil", MaterialKind.Cloth);
            sut.Place("vela", MaterialKind.Mast);
            var swapped = sut.Confirm();

            Assert.That(swapped.WrongSlotIds, Is.EquivalentTo(new[] { "mastil", "vela" }));
            Assert.That(sut.PlacedIn("mastil"), Is.Null);
            Assert.That(sut.PlacedIn("vela"), Is.Null);
            Assert.That(sut.Remaining(MaterialKind.Mast), Is.EqualTo(1));
            Assert.That(sut.Remaining(MaterialKind.Cloth), Is.EqualTo(1));
            Assert.That(sut.PlacedIn("tronco_1"), Is.EqualTo(MaterialKind.Logs), "la base ni se toca");
        }

        [Test]
        public void RaftValidator_RF43_LasFasesAprobadasSobrevivenAUnaPruebaFallida()
        {
            var sut = RaftAssemblyTests.Sut();
            RaftAssemblyTests.BuildBase(sut);
            RaftAssemblyTests.BuildLashing(sut);
            sut.Place("mastil", MaterialKind.Cloth);
            sut.Place("vela", MaterialKind.Mast);

            var result = sut.Confirm();

            Assert.That(result.Passed, Is.False, "la balsa se hunde");
            Assert.That(sut.IsConfirmed(RaftPhase.Base) && sut.IsConfirmed(RaftPhase.Lashing), Is.True,
                "base y amarre siguen aprobados (RF-43, guion §1.8.4)");
            Assert.That(sut.ActivePhase, Is.EqualTo(RaftPhase.MastAndSail), "se reintenta solo la parte que falló");
            Assert.That(new[] { "tronco_1", "tronco_2", "amarre_1", "amarre_2" }.Select(sut.PlacedIn), Has.All.Not.Null,
                "con todas sus piezas en su sitio");
        }

        [Test]
        public void RaftValidator_CP06_ElMensajeDiceQueRevisarNoCualEsLaPiezaCorrecta()
        {
            var content = RaftAssemblyContent.Create(RaftAssemblyTests.Slots(),
                phaseRejectedMessage: "revisa lo marcado", testFailedMessage: "se hunde: mira lo marcado");
            var sut = RaftAssemblyTests.Sut(content);

            sut.Place("tronco_1", MaterialKind.Mast);
            var phase = sut.Confirm();
            Assert.That(phase.Message, Is.EqualTo("revisa lo marcado"), "en la base y el amarre, el texto del asset");

            RaftAssemblyTests.BuildBase(sut);
            RaftAssemblyTests.BuildLashing(sut);
            sut.Place("mastil", MaterialKind.Cloth);
            var test = sut.Confirm();
            Assert.That(test.Message, Is.EqualTo("se hunde: mira lo marcado"), "en la prueba, el suyo");

            // Y en el asset real, ninguna frase nombra la pieza que va en el espacio (CP-06).
            foreach (var message in new[] { Asset().PhaseRejectedMessage, Asset().TestFailedMessage })
            {
                Assert.That(message, Does.Not.Match("(?i)tronco|soga|amarre|tela|vela|m[aá]stil"),
                    $"«{message}» orienta sin resolver: no nombra la pieza correcta");
            }
        }

        [Test]
        public void RaftValidator_RF17_NingunMensajeContieneDigitos()
        {
            var asset = Asset();
            var messages = new[]
            {
                asset.PlacedMessage, asset.TakenBackMessage, asset.MissedMessage, asset.OccupiedMessage,
                asset.PhaseRejectedMessage, asset.BaseConfirmedMessage, asset.LashingConfirmedMessage,
                asset.TestFailedMessage, asset.TestPassedMessage, asset.ConfirmLabel, asset.TestLabel
            };

            Assert.That(messages, Has.All.Not.Empty, "todas las frases están escritas");
            Assert.That(messages.Where(message => Regex.IsMatch(message, @"\d")), Is.Empty, "ninguna cifra (RF-17, CP-03)");
        }

        [Test]
        public void RaftAssemblyContent_RF40_ElAssetTraeLaBalsaDeCincoTroncosDiezAmarresMastilYVela()
        {
            var asset = Asset();

            Assert.That(asset.SlotsOf(RaftPhase.Base).Select(slot => slot.Accepts), Is.EqualTo(Enumerable.Repeat(MaterialKind.Logs, 5)),
                "cinco troncos en la base");
            Assert.That(asset.SlotsOf(RaftPhase.Lashing).Select(slot => slot.Accepts), Is.EqualTo(Enumerable.Repeat(MaterialKind.Ropes, 10)),
                "diez amarres, uno en cada extremo de cada tronco");
            Assert.That(asset.SlotsOf(RaftPhase.MastAndSail).Select(slot => slot.Accepts),
                Is.EquivalentTo(new[] { MaterialKind.Mast, MaterialKind.Cloth }), "mástil y vela en una sola fase (RF-40)");
            Assert.That(asset.Slots.Select(slot => slot.Id).Distinct().Count(), Is.EqualTo(asset.Slots.Length), "ids únicos");
            Assert.That(asset.KnotsPerRope, Is.EqualTo(asset.SlotsOf(RaftPhase.Lashing).Length), "el rollo da exactamente los amarres de la balsa");

            foreach (var kind in new[] { MaterialKind.Logs, MaterialKind.Ropes, MaterialKind.Cloth, MaterialKind.Mast })
            {
                var art = asset.ArtFor(kind);
                Assert.That(art?.Placed, Is.Not.Null, $"«{kind}» tiene arte de pieza (RNF-23)");
                Assert.That(art?.Silhouette, Is.Not.Null, $"y de silueta");
                Assert.That(art.Silhouette, Is.Not.SameAs(art.Placed), "dibujadas aparte");
            }
        }
    }
}
