using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// El inventario, la proximidad, el paso por la orilla y la zona de construcción
    /// (RF-35, RF-37, RF-38, RF-39, CU-09). Reglas puras: sin escena ni cuadros.
    /// </summary>
    public class InventoryTests
    {
        // Deliberadamente distintos del asset del juego: si la regla se apoyara en un literal en
        // vez de en la configuración, estas pruebas fallarían (RNF-18).
        private static readonly Collectible Troncos = new Collectible("troncos", MaterialKind.Logs, "Troncos", position: new Vector2(0.2f, 0.2f));
        private static readonly Collectible Sogas = new Collectible("sogas", MaterialKind.Ropes, "Sogas", position: new Vector2(0.8f, 0.2f));
        private static readonly Collectible Tela = new Collectible("tela", MaterialKind.Cloth, "Tela", position: new Vector2(0.2f, 0.8f));
        private static readonly Collectible Mastil = new Collectible("mastil", MaterialKind.Mast, "Mástil", position: new Vector2(0.8f, 0.8f));

        private static RiverLevelConfig Config() => RiverLevelConfig.Create(
            new[] { Troncos, Sogas, Tela, Mastil },
            proximityRadius: 0.1f,
            buildZonePosition: new Vector2(0.5f, 0.5f),
            buildZoneRadius: 0.05f,
            collectedFormat: "recogido {0}",
            allCollectedMessage: "todo",
            missingFormat: "falta {0}",
            zoneOpenedMessage: "abierta");

        [Test]
        public void Inventory_RF38_CapacidadEsCuatroYNoHayObjetosSobrantes()
        {
            var config = Config();
            var sut = new Inventory(config);

            Assert.That(sut.Capacity, Is.EqualTo(config.Collectibles.Length),
                "caben exactamente los del catálogo: ni gestión de espacio ni sobrantes (RF-38)");
            Assert.That(sut.Capacity, Is.EqualTo(4));
            Assert.That(config.Collectibles.Select(item => item.Kind).Distinct().Count(), Is.EqualTo(4),
                "uno por clase: troncos, sogas, tela y mástil (guion §1.8.2)");

            foreach (var item in config.Collectibles)
            {
                sut.TryCollect(item);
            }

            Assert.That(sut.IsFull, Is.True, "con los cuatro el inventario está lleno");
            Assert.That(sut.Missing, Is.Empty, "y no falta nada");
        }

        [Test]
        public void Collectible_RF37_ElBotonRecogerSoloApareceDentroDelRadioDeProximidad()
        {
            var radio = Config().ProximityRadius;

            Assert.That(Troncos.IsWithinReach(new Vector2(0.2f, 0.2f), radio), Is.True, "encima");
            Assert.That(Troncos.IsWithinReach(new Vector2(0.2f, 0.29f), radio), Is.True, "dentro del radio");
            Assert.That(Troncos.IsWithinReach(new Vector2(0.2f, 0.31f), radio), Is.False, "justo fuera");
            Assert.That(Troncos.IsWithinReach(new Vector2(0.6f, 0.6f), radio), Is.False, "lejos");
            // El radio es del asset y no un literal: con otro radio la misma distancia cambia de lado.
            Assert.That(Troncos.IsWithinReach(new Vector2(0.2f, 0.31f), 0.2f), Is.True, "con un radio mayor entra (RNF-18)");
        }

        [Test]
        public void Inventory_CU09_ConElInventarioLlenoInformaQueYaTieneTodo()
        {
            var sut = new Inventory(Config());
            var mensajes = new[] { Troncos, Sogas, Tela, Mastil }.Select(item => sut.TryCollect(item).Message).ToArray();

            Assert.That(mensajes.Take(3), Is.EqualTo(new[] { "recogido Troncos", "recogido Sogas", "recogido Tela" }),
                "cada recogida se describe con el texto del asset y el nombre del material");
            Assert.That(mensajes[3], Is.EqualTo("todo"), "la cuarta ya informa que cuenta con todo (CU-09 FA-4a)");

            var extra = sut.TryCollect(new Collectible("otro", MaterialKind.Logs, "Otro"));
            Assert.That(extra.Accepted, Is.False, "no cabe nada más");
            Assert.That(extra.Message, Is.EqualTo("todo"), "y se informa, no se regaña (CP-02)");
            Assert.That(sut.Items, Has.Count.EqualTo(4));
        }

        [Test]
        public void Inventory_RF37_UnMaterialNoSePuedeRecogerDosVeces()
        {
            var sut = new Inventory(Config());

            Assert.That(sut.TryCollect(Troncos).Accepted, Is.True);
            var repetido = sut.TryCollect(Troncos);

            Assert.That(repetido.Accepted, Is.False, "ya estaba en el inventario");
            Assert.That(repetido.Message, Is.Empty, "y no hubo intento que describir");
            Assert.That(sut.Items, Has.Count.EqualTo(1), "sigue contando una sola vez");
            Assert.That(sut.Has(MaterialKind.Logs), Is.True);
        }

        [Test]
        public void BuildZone_CU09_NombraLoQueFaltaSinCifrasYDejaSalir()
        {
            var config = Config();
            var inventario = new Inventory(config);
            var sut = new BuildZone(config);
            inventario.TryCollect(Troncos);

            var intento = sut.TryEnter(inventario);

            Assert.That(intento.Accepted, Is.False, "sin todo no abre");
            Assert.That(intento.Message, Is.EqualTo("falta Sogas, Tela y Mástil"),
                "dice cuáles por su nombre, con el texto del asset (CU-09 FA-6a)");
            Assert.That(intento.Message, Does.Not.Match(@"\d"), "y nunca cuántos (CP-03)");
            Assert.That(sut.IsOpen, Is.False, "se puede salir y seguir buscando: nada cambió (CP-02)");

            inventario.TryCollect(Sogas);
            inventario.TryCollect(Tela);
            Assert.That(sut.TryEnter(inventario).Message, Is.EqualTo("falta Mástil"), "con uno solo no hay coma ni «y»");
        }

        [Test]
        public void BuildZone_RF39_AbreSoloConLosCuatroMaterialesYSeQuedaAbierta()
        {
            var config = Config();
            var inventario = new Inventory(config);
            var sut = new BuildZone(config);
            foreach (var item in config.Collectibles)
            {
                inventario.TryCollect(item);
            }

            Assert.That(sut.Contains(new Vector2(0.52f, 0.5f)), Is.True, "dentro del radio de la zona");
            Assert.That(sut.Contains(new Vector2(0.6f, 0.5f)), Is.False, "fuera");

            var apertura = sut.TryEnter(inventario);
            Assert.That(apertura.Accepted, Is.True, "con los cuatro abre el ensamblaje (RF-39)");
            Assert.That(apertura.Message, Is.EqualTo("abierta"));
            Assert.That(sut.IsOpen, Is.True);

            var repetida = sut.TryEnter(inventario);
            Assert.That(repetida.Accepted, Is.True, "entrar otra vez no cierra nada");
            Assert.That(repetida.Message, Is.Empty, "ni repite la apertura");
        }

        [Test]
        public void RiverWalk_RF35_SeDesplazaEnDosDimensionesSinAtravesarLosLimites()
        {
            var limites = new Rect(0.1f, 0.2f, 0.3f, 0.4f); // x 0.1..0.4 · y 0.2..0.6
            var sut = new RiverWalk(new Vector2(0.2f, 0.3f), limites, speed: 1f);

            sut.Step(new Vector2(1f, 1f), 0.1f);
            Assert.That(sut.Position.x, Is.GreaterThan(0.2f), "avanza en x");
            Assert.That(sut.Position.y, Is.GreaterThan(0.3f), "y en y a la vez: dos dimensiones");

            sut.Step(Vector2.right, 10f);
            Assert.That(sut.Position.x, Is.EqualTo(0.4f).Within(1e-5f), "se detiene en el borde derecho, no lo cruza");
            sut.Step(Vector2.down, 10f);
            Assert.That(sut.Position.y, Is.EqualTo(0.2f).Within(1e-5f), "ni el inferior");
            sut.Step(Vector2.zero, 10f);
            Assert.That(sut.Position, Is.EqualTo(new Vector2(0.4f, 0.2f)), "sin dirección no se mueve");

            var fuera = new RiverWalk(new Vector2(0.9f, 0.9f), limites, 1f);
            Assert.That(fuera.Position, Is.EqualTo(new Vector2(0.4f, 0.6f)), "un arranque fuera se recorta al escenario");
        }
    }
}
