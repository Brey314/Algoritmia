using System.Linq;
using System.Threading.Tasks;
using Game.Scaffolding;
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

        /// <summary>
        /// Lo que está más abajo se dibuja delante (DA83, INC-118, lectura B): la perspectiva no es
        /// solo escala. Mamá pasa por delante de los materiales y de la familia que tiene más arriba
        /// y por detrás de los que tiene más abajo. Antes los materiales se instanciaban al final
        /// del entorno y tapaban a Mamá siempre, aun recogiéndolos desde abajo; a ×2,3 su cuerpo
        /// cubre un material entero, así que el orden se nota. La sombra y el área de la balsa
        /// quedan por encima de todo: al abrirse el ensamblaje oscurecen a Mamá y a la familia
        /// como antes.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public async Task RiverScene_DA83_LoQueEstaMasAbajoSeDibujaDelante()
        {
            var river = await RiverMovementTests.OpenRiver();
            var config = river.Config;
            var sombra = river.Assembly.Shade.rectTransform;
            var areaDeLaBalsa = river.Assembly.RaftArea;

            // Cada cosa que pisa la orilla con la altura a la que pisa: la familia en el ancla de sus
            // pies, los materiales donde los pone el asset y Mamá donde está ahora.
            (string Nombre, RectTransform Rect, float Y)[] Pisan() => river.Family
                .Select(rig => (rig.name, (RectTransform)rig.transform, ((RectTransform)rig.transform).anchorMin.y))
                .Concat(river.Spawned.Select(entry => (entry.Image.name, entry.Image.rectTransform, entry.Collectible.Position.y)))
                .Append(("Mamá", river.Player, river.Walk.Position.y))
                .ToArray();

            void AssertLoDeAbajoVaDelante(string donde)
            {
                var pisan = Pisan();
                foreach (var (nombre, rect, y) in pisan)
                {
                    foreach (var (otroNombre, otro, otraY) in pisan.Where(otra => otra.Y < y - 1e-4f))
                    {
                        Assert.That(otro.GetSiblingIndex(), Is.GreaterThan(rect.GetSiblingIndex()),
                            $"{donde}: «{otroNombre}» (y {otraY:0.000}) está más abajo que «{nombre}» (y {y:0.000}) y se dibuja detrás");
                    }
                }

                var ultimo = pisan.Max(pisa => pisa.Rect.GetSiblingIndex());
                Assert.That(sombra.GetSiblingIndex(), Is.GreaterThan(ultimo), $"{donde}: la sombra del ensamblaje queda sobre todos");
                Assert.That(areaDeLaBalsa.GetSiblingIndex(), Is.GreaterThan(ultimo), $"{donde}: y el área de la balsa también");
            }

            AssertLoDeAbajoVaDelante("al abrir");

            // Cuadro a cuadro, como Update: sube hasta el borde de arriba y baja hasta el de abajo, y en
            // cada paso el orden sigue a las alturas. Mamá cruza la de cada material y la de la familia.
            var paso = config.MoveSpeed * 0.02f;
            var subida = Mathf.CeilToInt((config.WalkableArea.yMax - config.StartPosition.y) / paso) + 2;
            for (var cuadro = 0; cuadro < subida; cuadro++)
            {
                river.Tick(Vector2.up, 0.02f);
                AssertLoDeAbajoVaDelante($"subiendo, cuadro {cuadro}");
            }

            Assert.That(river.Walk.Position.y, Is.EqualTo(config.WalkableArea.yMax).Within(1e-4f), "llegó al borde de arriba");
            Assert.That(river.Player.GetSiblingIndex(), Is.LessThan(river.Spawned.Min(entry => entry.Image.rectTransform.GetSiblingIndex())),
                "con los pies más arriba que cualquier material, Mamá queda detrás de todos");

            var bajada = Mathf.CeilToInt((config.WalkableArea.yMax - config.WalkableArea.yMin) / paso) + 2;
            for (var cuadro = 0; cuadro < bajada; cuadro++)
            {
                river.Tick(Vector2.down, 0.02f);
                AssertLoDeAbajoVaDelante($"bajando, cuadro {cuadro}");
            }

            Assert.That(river.Walk.Position.y, Is.EqualTo(config.WalkableArea.yMin).Within(1e-4f), "llegó al borde de abajo");
            Assert.That(river.Player.GetSiblingIndex(), Is.GreaterThan(river.Spawned.Max(entry => entry.Image.rectTransform.GetSiblingIndex())),
                "con los pies más abajo que cualquier material, Mamá queda delante de todos");
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

        [Test]
        [Timeout(30000)]
        public async Task RiverScene_RF36_RecogerTroncosSeMarcaConElUltimoTroncoYNoAntes()
        {
            var river = await RiverMovementTests.OpenRiver();
            var troncos = river.Spawned.Where(entry => entry.Collectible.Kind == MaterialKind.Logs).ToArray();
            var fila = river.Rows.Single(row => row.Task == RiverTaskId.CollectLogs);
            var iconoPendiente = fila.Icon.sprite;

            for (var i = 0; i < troncos.Length; i++)
            {
                var (tronco, _) = troncos[i];
                WalkTo(river, tronco.Position);
                river.CollectButton.onClick.Invoke();

                var esElUltimo = i == troncos.Length - 1;
                Assert.That(river.Tasks.IsDone(RiverTaskId.CollectLogs), Is.EqualTo(esElUltimo),
                    esElUltimo ? "con el último tronco la tarea se marca" : "antes del último, la tarea sigue sin marcar (RF-36)");
                Assert.That(fila.Icon.sprite, esElUltimo ? Is.Not.SameAs(iconoPendiente) : Is.SameAs(iconoPendiente),
                    "el icono de la fila acompaña al estado de la tarea (RNF-19)");
            }
        }

        [Test]
        [Timeout(30000)]
        public async Task RiverScene_DA133_MamaRecogeAlPulsarRecogerYVuelveSolaAlReposo()
        {
            var river = await RiverMovementTests.OpenRiver();
            var (material, _) = river.Spawned.First();
            WalkTo(river, material.Position);

            river.CollectButton.onClick.Invoke();

            Assert.That(river.PlayerRig.Current, Is.EqualTo(ActorAction.PickUp), "recoger se ve: Mamá se agacha y lo levanta (§13.3)");

            var limite = Time.realtimeSinceStartup + 3f;
            while (river.PlayerRig.Current != ActorAction.Idle && Time.realtimeSinceStartup < limite)
            {
                await Awaitable.NextFrameAsync();
            }

            Assert.That(river.PlayerRig.Current, Is.EqualTo(ActorAction.Idle), "y vuelve sola al reposo: el gesto no se queda puesto");
        }

        [Test]
        [Timeout(30000)]
        public async Task BuildZone_DA133_EntrarSinTodoLosMaterialesDaAnimoYNingunGestoDeDerrota()
        {
            var river = await RiverMovementTests.OpenRiver();
            var todos = river.Family.Append(river.PlayerRig).ToArray();
            Assert.That(river.Family.Count, Is.EqualTo(3), "Papá, la Niña y el Niño esperan junto a la zona");
            Assert.That(todos, Has.None.Null);

            WalkTo(river, river.Config.BuildZonePosition);
            Assume.That(river.Zone.IsOpen, Is.False, "sin nada recogido la zona no abre");

            Assert.That(todos.Select(rig => rig.Current), Is.All.EqualTo(ActorAction.Encourage),
                "Mamá y la familia animan: faltar algo no es una derrota (CP-02, §7.3)");
            await RiverMovementTests.AssertSoloAnimo(todos);
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
