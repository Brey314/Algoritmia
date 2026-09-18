using System.Linq;
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
        public void RiverLevelConfig_RF38_ElAssetTraeExactamenteUnMaterialDeCadaClase()
        {
            var config = Asset();

            Assert.That(config.Collectibles, Has.Length.EqualTo(4));
            Assert.That(config.Collectibles.Select(item => item.Kind), Is.EquivalentTo(new[]
            {
                MaterialKind.Logs, MaterialKind.Ropes, MaterialKind.Cloth, MaterialKind.Mast
            }));
            Assert.That(config.Collectibles.Select(item => item.Id).Distinct().Count(), Is.EqualTo(4), "ids únicos");
            Assert.That(config.Collectibles.Select(item => item.Art), Has.All.Not.Null, "cada uno con su ilustración (RNF-23)");
            Assert.That(config.Collectibles.Select(item => item.DisplayName), Has.All.Not.Empty, "y con nombre para decir qué falta");
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
            Assert.That(config.Collectibles.Select(item => item.Position).Distinct().Count(), Is.EqualTo(4),
                "no están todos en el mismo punto");
        }
    }
}
