using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// Lo que el asset del Nivel 3 tiene que traer para que la fase se pueda jugar: los textos
    /// del guion, los cuatro materiales y un reparto que quepa en el plano fijo (RF-36, RF-38,
    /// RNF-18, pregunta abierta 2 del plan).
    /// </summary>
    public class RiverLevelConfigTests
    {
        private static RiverLevelConfig Asset() => AssetDatabase.FindAssets($"t:{nameof(RiverLevelConfig)}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<RiverLevelConfig>)
            .Single(config => config.name == "N3_RiverLevelConfig");

        [Test]
        public void RiverLevelConfig_RF36_ElAssetTraeLasCuatroTareasDelGuion()
        {
            var config = Asset();

            Assert.That(RiverTask.All.Select(config.LabelFor), Is.EqualTo(new[]
            {
                "Recoger troncos", "Encontrar sogas", "Ensamblar la balsa", "Colocar el mástil y la vela"
            }), "los textos son los del guion §1.8.1, en su orden");
            Assert.That(config.TaskLabels.Any(label => System.Text.RegularExpressions.Regex.IsMatch(label, @"\d")),
                Is.False, "ninguna cifra en la lista (CP-03)");
        }

        [Test]
        public void RiverLevelConfig_RF38_ElAssetTraeCincoTroncosYUnoDeCadaOtraClase()
        {
            var config = Asset();

            // Cinco troncos sueltos por la orilla (decisión del 20/09/2026): todos hay que encontrarlos.
            Assert.That(config.Collectibles.Count(item => item.Kind == MaterialKind.Logs), Is.EqualTo(5));
            Assert.That(config.Collectibles.Where(item => item.Kind != MaterialKind.Logs).Select(item => item.Kind), Is.EquivalentTo(new[]
            {
                MaterialKind.Ropes, MaterialKind.Cloth, MaterialKind.Mast
            }));
            Assert.That(config.Collectibles.Select(item => item.Id).Distinct().Count(), Is.EqualTo(config.Collectibles.Length), "ids únicos");
            Assert.That(config.Collectibles.Where(item => item.Kind == MaterialKind.Logs).Select(item => item.DisplayName).Distinct().Count(),
                Is.EqualTo(1), "los troncos comparten nombre: en la zona se nombran una vez");
            Assert.That(config.Collectibles.Select(item => item.Art), Has.All.Not.Null, "cada uno con su ilustración (RNF-23)");
            Assert.That(config.Collectibles.Select(item => item.DisplayName), Has.All.Not.Empty, "y con nombre para decir qué falta");
        }

        [Test]
        public void RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsElBosqueConUnPocoDelRio()
        {
            var config = Asset();
            var half = 0.5f / config.PlayFraming.Zoom;
            var right = config.PlayFraming.Focus.x + half;

            // El agua de env_n3_rio empieza entre x ≈ 0.42 y 0.54 según la altura (medido el
            // 25/09/2026): el recorte pasa de 0.46 para que la orilla asome a la derecha
            // (decisión de Santiago del 25/09/2026; guion §1.8, «orilla del río y bosque
            // circundante»), y no de 0.6 para que siga siendo el bosque: el río entero entra con
            // el empuje del ensamblaje.
            Assert.That(right, Is.GreaterThan(0.46f), "la orilla asoma en el plano de la recolección");
            Assert.That(right, Is.LessThanOrEqualTo(0.6f), "pero el plano sigue siendo el bosque");
            Assert.That(config.PlayFraming.Focus.y - half, Is.LessThanOrEqualTo(0.001f).And.GreaterThanOrEqualTo(-0.001f),
                "el plano se apoya en el borde inferior: es el suelo del bosque");
            Assert.That(IllustrationFraming.Warnings(config.PlayFraming, null, mirroredForest: false, IllustrationFraming.ScreenAspect),
                Is.Empty, "y es un encuadre válido para 16:9");
        }

        [Test]
        public void RiverLevelConfig_DA83_LoQueEstaMasAbajoSeVeMasGrandeYLoDeArribaMasPequeno()
        {
            var config = RiverLevelConfig.Create(System.Array.Empty<Collectible>(), groundTop: 0.4f, depthScaleNear: 1.2f, depthScaleFar: 0.6f);

            Assert.That(config.DepthScaleAt(0f), Is.EqualTo(1.2f).Within(1e-5f), "al borde de abajo, lo más cerca");
            Assert.That(config.DepthScaleAt(0.2f), Is.EqualTo(0.9f).Within(1e-5f), "a mitad del piso, a mitad de camino");
            Assert.That(config.DepthScaleAt(0.4f), Is.EqualTo(0.6f).Within(1e-5f), "donde termina el piso, lo más lejos");
            Assert.That(config.DepthScaleAt(0.9f), Is.EqualTo(0.6f).Within(1e-5f), "por encima del piso no sigue encogiendo");
            Assert.That(config.DepthScaleAt(0.05f), Is.GreaterThan(config.DepthScaleAt(0.35f)), "más abajo, más grande");

            var asset = Asset();
            Assert.That(asset.DepthScaleNear, Is.GreaterThan(asset.DepthScaleFar), "el asset aplica la regla, no la anula (RNF-18)");
        }

        [Test]
        public void RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo()
        {
            var config = Asset();
            var half = 0.5f / config.PlayFraming.Zoom;
            var visible = new Rect(config.PlayFraming.Focus.x - half, config.PlayFraming.Focus.y - half, 2f * half, 2f * half);

            Assert.That(config.ProximityRadius, Is.GreaterThan(0f));
            Assert.That(visible.Contains(config.StartPosition), Is.True, "Mamá arranca en cuadro");
            Assert.That(visible.Contains(config.BuildZonePosition), Is.True, "la zona se ve desde el principio (RF-39)");
            Assert.That(visible.Contains(config.WalkableArea.min) && visible.Contains(config.WalkableArea.max), Is.True,
                "la orilla entera cabe en el plano fijo: Mamá nunca sale de cámara (RF-35)");

            foreach (var item in config.Collectibles)
            {
                Assert.That(visible.Contains(item.Position), Is.True, $"«{item.Id}» está en cuadro");
                Assert.That(config.WalkableArea.Contains(item.Position), Is.True, $"«{item.Id}» está al alcance de la orilla");
                Assert.That(item.IsWithinReach(config.StartPosition, config.ProximityRadius), Is.False,
                    $"«{item.Id}» no se recoge sin moverse: el reparto obliga a recorrer el mapa (guion §1.8.2)");
            }

            Assert.That(config.WalkableArea.Contains(config.BuildZonePosition), Is.True, "y a la zona se llega andando");

            // El piso: nada se coloca donde empiezan los arbustos, y la orilla andable tampoco sube ahí.
            Assert.That(config.GroundTop, Is.InRange(0.3f, 0.45f), "el piso termina donde lo mide la ilustración (env_n3_rio)");
            Assert.That(config.WalkableArea.yMax, Is.LessThanOrEqualTo(config.GroundTop), "la orilla andable no pasa del piso");
            Assert.That(config.Collectibles.Select(item => item.Position.y), Has.All.LessThanOrEqualTo(config.GroundTop), "todos los materiales están en el piso");
            Assert.That(config.BuildZonePosition.y, Is.LessThanOrEqualTo(config.GroundTop));
            Assert.That(config.Collectibles.Select(item => item.Position).Distinct().Count(), Is.EqualTo(config.Collectibles.Length),
                "no están todos en el mismo punto: cada hallazgo tiene el suyo");
        }
    }
}
