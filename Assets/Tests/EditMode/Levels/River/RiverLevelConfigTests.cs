using System;
using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

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

        private static T Asset<T>(string nombre) where T : Object => AssetDatabase.FindAssets($"t:{typeof(T).Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>)
            .Single(asset => asset.name == nombre);

        /// <summary>
        /// Abre la orilla como escena de vista previa —sin tocar las escenas abiertas ni correr
        /// Awake ni Start— y la cierra siempre: lo que se mira es lo que está guardado en disco.
        /// </summary>
        private static void ConLaOrilla(Action<RiverSceneController> mirar)
        {
            var escena = EditorSceneManager.OpenPreviewScene("Assets/Game/Scenes/Level3_River.unity");
            try
            {
                mirar(escena.GetRootGameObjects()
                    .Select(raiz => raiz.GetComponentInChildren<RiverSceneController>(true))
                    .Single(controlador => controlador != null));
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(escena);
            }
        }

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
        public void RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsLaOrillaConElRio()
        {
            var config = Asset();
            var half = 0.5f / config.PlayFraming.Zoom;
            var right = config.PlayFraming.Focus.x + half;

            // Lectura B (decisión de Santiago del 30/09/2026, INC-118): el plano se abre para que
            // entren la orilla, el río y el pie de la cascada (guion §1.8, «orilla del río y bosque
            // circundante»). El agua de env_n3_rio va de x ≈ 0.42–0.54 a 0.67–0.80 según la altura
            // (medido el 30/09/2026): el recorte pasa de 0.65 para que el río se vea y no de 0.80
            // para que la orilla siga llenando el plano.
            Assert.That(right, Is.InRange(0.65f, 0.80f), "el río entra en el plano de la recolección");
            Assert.That(config.PlayFraming.Focus.y - half, Is.InRange(-0.001f, 0.001f),
                "el plano se apoya en el borde inferior: es el suelo de la orilla");
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

            // La zona se atiende al cruzar su borde (RiverSceneController.Tick): si el alcance de un
            // material la tocara, recoger el último dentro de ella no la abriría hasta salir y
            // volver a entrar (RF-39). Nada castiga, pero la zona parecería rota.
            Assert.That(config.Collectibles.Select(item => Vector2.Distance(item.Position, config.BuildZonePosition)),
                Has.All.GreaterThan(config.ProximityRadius + config.BuildZoneRadius), "el alcance de ningún material toca la zona");
            Assert.That(Vector2.Distance(config.StartPosition, config.BuildZonePosition), Is.GreaterThan(config.BuildZoneRadius),
                "Mamá no arranca dentro de la zona");

            // El piso: nada se coloca donde empiezan los arbustos, y la orilla andable tampoco sube ahí.
            Assert.That(config.GroundTop, Is.InRange(0.3f, 0.45f), "el piso termina donde lo mide la ilustración (env_n3_rio)");
            Assert.That(config.WalkableArea.yMax, Is.LessThanOrEqualTo(config.GroundTop), "la orilla andable no pasa del piso");
            Assert.That(config.Collectibles.Select(item => item.Position.y), Has.All.LessThanOrEqualTo(config.GroundTop), "todos los materiales están en el piso");
            Assert.That(config.BuildZonePosition.y, Is.LessThanOrEqualTo(config.GroundTop));
            Assert.That(config.Collectibles.Select(item => item.Position).Distinct().Count(), Is.EqualTo(config.Collectibles.Length),
                "no están todos en el mismo punto: cada hallazgo tiene el suyo");
        }

        /// <summary>
        /// La familia y Mamá se ven en la orilla a la escala con la que abre la escena 3.1
        /// (INC-118, lectura B): a 0,4 de ella la mecánica y la narrativa parecían dos mundos. Mamá
        /// se mide donde se reúne con la familia, en la zona: allí su profundidad la deja a la escala
        /// de la narrativa. La familia no se escala por profundidad: se mide su casilla.
        /// </summary>
        [Test]
        public void RiverScene_INC118_LosPersonajesDeLaMecanicaTienenLaEscalaDeLaNarrativa()
        {
            var llegada = Asset<NarrativeSequence>("N3_Escena31_Llegada");
            float EnLaLlegada(string personaje) => llegada.Props.Single(prop => prop.Actor != null && prop.Actor.name == personaje).Size
                                                 * llegada.Illustration.rect.height;

            ConLaOrilla(river =>
            {
                foreach (var rig in river.Family)
                {
                    var casilla = (RectTransform)rig.transform;
                    var personaje = rig.name.Replace("Personaje_", string.Empty); // «Personaje_Papa» → prefab «Papa»
                    Assert.That(casilla.sizeDelta.y * casilla.localScale.y, Is.EqualTo(EnLaLlegada(personaje)).Within(15).Percent,
                        $"«{rig.name}» mide en la orilla lo que en la 3.1");
                }

                var enLaZona = river.Player.sizeDelta.y * river.Config.DepthScaleAt(river.Config.BuildZonePosition.y);
                Assert.That(enLaZona, Is.EqualTo(EnLaLlegada("Mama")).Within(15).Percent, "Mamá, en la zona, mide lo que en la 3.1");
            });
        }

        /// <summary>
        /// Mamá se ancla por los pies, como la familia: la profundidad se lee donde pisa y la orilla
        /// andable es el pasto que pisa (DA83, lectura B). Con el pivote en el centro, a ×2,3 los pies
        /// le quedan 0,08 por debajo de su posición, en el seto y en el agua. Esta guarda no depende
        /// de la Game View: RF35 solo lo nota a 1920×1080.
        /// </summary>
        [Test]
        public void RiverScene_DA83_MamaSeAnclaPorLosPiesComoLaFamilia()
        {
            ConLaOrilla(river =>
            {
                var pies = river.Family.Select(rig => ((RectTransform)rig.transform).pivot.y).Distinct().Single();
                Assert.That(river.Player.pivot.x, Is.EqualTo(0.5f).Within(1e-4f), "centrada en x");
                Assert.That(river.Player.pivot.y, Is.EqualTo(pies).Within(1e-4f), "y con los pies donde los tiene la familia");
            });
        }

        /// <summary>
        /// La lista de la 3.1 se dibuja con lo mismo que luego se recoge (INC-118): el tronco en 3/4
        /// de la orilla —no el montón plano de antes— y también el mástil, que la mecánica pide.
        /// </summary>
        [Test]
        public void RiverLevelConfig_INC118_LaEscena31PintaCadaMaterialConElArteDeLaOrilla()
        {
            var pintados = Asset<NarrativeSequence>("N3_Escena31_Llegada").Props
                .Where(prop => prop.Actor == null)
                .Select(prop => prop.Art)
                .ToArray();

            Assert.That(Asset().Collectibles.Select(item => item.Art).Distinct(), Is.SubsetOf(pintados));
        }
    }
}
