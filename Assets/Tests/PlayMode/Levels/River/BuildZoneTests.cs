using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// Recoger por proximidad, marcar tareas y entrar a la zona de construcción, cableado en la
    /// escena (RF-36..RF-39, CU-09). Mamá se mueve con <see cref="RiverSceneController.Tick"/>, que
    /// es lo que <c>Update</c> hace con las flechas sostenidas: aquí se verifica el juego, no que
    /// el EventSystem entregue el clic (eso es <c>RiverMovementTests</c>).
    /// </summary>
    [Category("Integration")]
    public class BuildZoneTests
    {
        [Test]
        [Timeout(30000)]
        public async Task BuildZone_RF39_AbreElPanelSoloConLosCuatroMateriales()
        {
            var river = await RiverMovementTests.OpenRiver();

            Assert.That(river.BuildZoneMarker.gameObject.activeInHierarchy, Is.True, "la zona está señalizada desde el principio (RF-39)");
            Assert.That(river.AssemblyPanel.activeSelf, Is.False, "el ensamblaje empieza cerrado");
            Assert.That(river.CollectButton.gameObject.activeSelf, Is.False, "«Recoger» no aparece lejos de todo (RF-37)");

            foreach (var (collectible, image) in river.Spawned.ToArray())
            {
                WalkTo(river, collectible.Position);
                Assert.That(river.CollectButton.gameObject.activeSelf, Is.True, $"cerca de «{collectible.Id}» aparece «Recoger»");
                Assert.That(river.Reachable, Is.SameAs(collectible));

                river.CollectButton.onClick.Invoke();

                Assert.That(image.gameObject.activeSelf, Is.False, $"«{collectible.Id}» deja la orilla");
                Assert.That(river.Inventory.Count(collectible.Kind), Is.GreaterThan(0), "y entra al inventario");
                Assert.That(river.Slots.Count(slot => slot.sprite != null),
                    Is.EqualTo(river.Inventory.Items.Select(item => item.Kind).Distinct().Count()),
                    "el inventario se llena por clase: una casilla por material, los troncos comparten la suya");
                Assert.That(river.TaskArea.gameObject.activeInHierarchy && river.InventoryArea.gameObject.activeInHierarchy,
                    Is.True, "lista e inventario siguen visibles (RF-36, RF-38)");
                Assert.That(river.MessageLabel.text, Does.Not.Match(@"\d"), "ninguna cifra en la retroalimentación (CP-03)");
            }

            Assert.That(river.Inventory.IsFull, Is.True);
            Assert.That(river.Tasks.IsDone(RiverTaskId.CollectLogs), Is.True, "la tarea 1 quedó marcada");
            Assert.That(river.Tasks.IsDone(RiverTaskId.FindRopes), Is.True, "la 2 también");
            Assert.That(river.Tasks.IsDone(RiverTaskId.AssembleRaft), Is.False, "la 3 sigue sin marcar: es de construcción (INC-30)");
            Assert.That(river.Tasks.IsDone(RiverTaskId.PlaceMastAndSail), Is.False, "y la 4 también");

            var hechas = river.Rows.Where(row => river.Tasks.IsDone(row.Task)).ToArray();
            var pendientes = river.Rows.Where(row => !river.Tasks.IsDone(row.Task)).ToArray();
            Assert.That(hechas.Select(row => row.Icon.sprite).Distinct().Single(),
                Is.Not.SameAs(pendientes.Select(row => row.Icon.sprite).Distinct().Single()),
                "hecha y pendiente se distinguen por la forma del icono, no solo por color (RNF-19)");
            Assert.That(hechas.First().Label.color, Is.Not.EqualTo(pendientes.First().Label.color), "y el color acompaña");

            WalkTo(river, river.Config.BuildZonePosition);

            Assert.That(river.Zone.IsOpen, Is.True, "con los cuatro materiales la zona abre");
            Assert.That(river.AssemblyPanel.activeSelf, Is.True, "y el panel de ensamblaje se enciende (RF-39)");
            Assert.That(river.Pads.All(pad => !pad.gameObject.activeSelf), Is.True, "las flechas se retiran: ya no se anda por la orilla");
            Assert.That(river.MessageLabel.text, Is.EqualTo(river.Config.ZoneOpenedMessage));
        }

        [Test]
        [Timeout(30000)]
        public async Task RiverScene_DA83_MamaYLosMaterialesSeVenMasGrandesCuantoMasAbajoEstan()
        {
            var river = await RiverMovementTests.OpenRiver();
            var config = river.Config;

            var porAltura = river.Spawned.OrderBy(entry => entry.Collectible.Position.y).ToArray();
            var escalas = porAltura.Select(entry => entry.Image.rectTransform.localScale.x).ToArray();
            Assert.That(escalas, Is.Ordered.Descending, "los materiales más abajo (más cerca) se ven más grandes que los de arriba");
            Assert.That(escalas.First(), Is.GreaterThan(escalas.Last()));
            Assert.That(escalas.First(), Is.EqualTo(config.DepthScaleAt(porAltura.First().Collectible.Position.y)).Within(1e-4f),
                "y la escala es la del asset para esa altura (RNF-18)");

            WalkTo(river, new Vector2(config.StartPosition.x, config.WalkableArea.yMax));
            var arriba = river.Player.localScale.x;
            WalkTo(river, new Vector2(config.StartPosition.x, config.WalkableArea.yMin));
            var abajo = river.Player.localScale.x;
            Assert.That(abajo, Is.GreaterThan(arriba), "Mamá también: abajo se ve más grande que arriba");
            Assert.That(abajo, Is.EqualTo(config.DepthScaleAt(river.Walk.Position.y)).Within(1e-4f), "con la escala del asset para su altura real");
        }

        [Test]
        [Timeout(30000)]
        public async Task BuildZone_CU09_SinTodosLosMaterialesIndicaCualesFaltanSinCifras()
        {
            var river = await RiverMovementTests.OpenRiver();
            // Se recogen los cinco troncos: la clase completa es lo que deja de pedirse en la zona.
            foreach (var (tronco, _) in river.Spawned.Where(entry => entry.Collectible.Kind == MaterialKind.Logs).ToArray())
            {
                WalkTo(river, tronco.Position);
                river.CollectButton.onClick.Invoke();
            }

            var troncos = river.Spawned.First(entry => entry.Collectible.Kind == MaterialKind.Logs).Collectible;

            WalkTo(river, river.Config.BuildZonePosition);

            Assert.That(river.Zone.IsOpen, Is.False, "sin todo no abre");
            Assert.That(river.AssemblyPanel.activeSelf, Is.False);
            var faltan = river.Config.Collectibles.Where(item => item.Kind != MaterialKind.Logs).Select(item => item.DisplayName).Distinct();
            foreach (var nombre in faltan)
            {
                Assert.That(river.MessageLabel.text, Does.Contain(nombre), $"nombra «{nombre}» entre lo que falta (CU-09 FA-6a)");
            }

            Assert.That(river.MessageLabel.text, Does.Not.Contain(troncos.DisplayName), "lo recogido no se pide");
            Assert.That(river.MessageLabel.text, Does.Not.Match(@"\d"), "cuáles, no cuántos (CP-03)");

            // Y se sale sin penalización: la recogida sigue funcionando igual (CP-02).
            var sogas = river.Spawned.Single(entry => entry.Collectible.Kind == MaterialKind.Ropes);
            WalkTo(river, sogas.Collectible.Position);
            Assert.That(river.CollectButton.gameObject.activeSelf, Is.True, "«Recoger» vuelve a aparecer junto a las sogas");
            river.CollectButton.onClick.Invoke();
            Assert.That(river.Inventory.Has(MaterialKind.Ropes), Is.True);
            Assert.That(river.Pads.All(pad => pad.gameObject.activeSelf), Is.True, "las flechas siguen ahí");
        }

        /// <summary>Lleva a Mamá hasta ese punto de la ilustración a pasos, como haría un jugador sosteniendo las flechas.</summary>
        private static void WalkTo(RiverSceneController river, Vector2 target)
        {
            for (var step = 0; step < 2000 && Vector2.Distance(river.Walk.Position, target) > 0.005f; step++)
            {
                river.Tick(target - river.Walk.Position, 0.02f);
                if (river.Zone.IsOpen)
                {
                    return; // Al abrir la zona ya no se anda: el paso se detiene donde entró.
                }
            }

            Assert.That(Vector2.Distance(river.Walk.Position, target), Is.LessThan(0.01f),
                $"Mamá llega a {target} andando por la orilla — quedó en {river.Walk.Position}");
        }
    }
}
