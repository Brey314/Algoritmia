using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>
    /// Lo que el asset del Nivel 2 tiene que declarar bien para que el bosque tenga salida.
    /// </summary>
    public class WheelLevelConfigTests
    {
        [Test]
        public void WheelLevelConfig_RNF13_LaSecuenciaDeCierreDeLaFase1ExisteEnElProyecto()
        {
            var config = AssetDatabase.FindAssets($"t:{nameof(WheelLevelConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<WheelLevelConfig>)
                .Single();

            var sequences = AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Select(sequence => sequence.Id)
                .ToArray();

            // A dónde sale el bosque al empujar la caja es contenido del asset, no una fórmula en
            // el código (RF-05). Un id sin asset detrás dejaría al estudiante en el bosque con la
            // caja ya rodada y sin nada más que pulsar: una pantalla sin salida (RNF-13).
            Assert.That(config.ClosingSequenceId, Is.Not.Empty, "la fase 1 declara a dónde sale");
            Assert.That(sequences, Does.Contain(config.ClosingSequenceId),
                "y esa secuencia existe entre las narrativas del proyecto");
        }

        [Test]
        public void WheelLevelConfig_RF05_LaEscenaDeCierreAbreConLaMismaVistaConLaQueTerminaElBosque()
        {
            var config = AssetDatabase.FindAssets($"t:{nameof(WheelLevelConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<WheelLevelConfig>)
                .Single();
            var cierre = AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Single(sequence => sequence.Id == config.ClosingSequenceId);

            // Del bosque a la narrativa la vista no cambia: el encuadre exacto en el que acaba el
            // zoom de cierre, los cinco troncos donde estaban y la caja rodando con la misma duración.
            Assert.That(cierre.CameraStart.Focus, Is.EqualTo(config.CompletionFraming.Focus),
                "el mismo foco con el que terminó el bosque");
            Assert.That(cierre.CameraStart.Zoom, Is.EqualTo(config.CompletionFraming.Zoom).Within(0.001f),
                "y el mismo acercamiento");
            Assert.That(cierre.CameraKeys, Is.Not.Empty, "y la cámara no se mueve hasta que el texto la mueve");
            Assert.That(cierre.CameraKeys.Min(key => key.Line), Is.GreaterThanOrEqualTo(1),
                "la primera línea se lee con la vista del bosque");

            var rodando = cierre.Props.Where(prop => prop.Motion == PropMotion.Roll && prop.MotionLine == 0).ToArray();
            Assert.That(rodando.Count(prop => prop.MotionDistance == 0f), Is.EqualTo(config.RequiredLogs),
                "un tronco girando en el sitio por cada tronco de la fila");
            Assert.That(rodando.Single(prop => prop.MotionDistance > 0f).MotionSeconds,
                Is.EqualTo(config.RollSeconds + config.FallSeconds).Within(0.001f),
                "la caja rueda y cae lo mismo que en el bosque");
        }

        [Test]
        public void WheelLevelConfig_RF05_ElBosqueAbreConLaVistaConLaQueTerminaLaEscena21()
        {
            var config = AssetDatabase.FindAssets($"t:{nameof(WheelLevelConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<WheelLevelConfig>)
                .Single();
            var bosque = AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Single(sequence => sequence.Id == "N2_Escena21_Bosque");

            // La última parada de la 2.1 abre al plano de juego: de la narrativa al bosque no hay salto
            // (Camara_Narrativa_N2.md §5.2 y §5.3).
            var ultima = bosque.CameraKeys.OrderBy(key => key.Line).Last().Framing;
            Assert.That(ultima.Focus, Is.EqualTo(config.PlayFraming.Focus), "el mismo foco");
            Assert.That(ultima.Zoom, Is.EqualTo(config.PlayFraming.Zoom).Within(0.001f), "y el mismo acercamiento");
        }
    }
}
