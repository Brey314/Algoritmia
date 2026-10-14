using System.Linq;
using Game.Core;
using NUnit.Framework;
using UnityEditor;

namespace Game.UI.Tests
{
    /// <summary>
    /// DEF-RC3-01: la forma de Algoritm en el resumen de cada nivel es contenido (CT-05, INC-45): vive en el
    /// asset de mensajes del nivel y no en una rama del controlador.
    /// </summary>
    [TestFixture]
    public class LevelSummaryGuideTests
    {
        [TestCase(LevelId.Fire, "Algoritm_Fuego")]
        [TestCase(LevelId.Wheel, "Algoritm_Rueda")]
        [TestCase(LevelId.River, "Algoritm_Gota")]
        public void LevelSummary_DEFRC301_CadaAssetDeMensajesTraeAAlgoritmEnLaFormaDeSuNivel(LevelId level, string form)
        {
            var messages = AssetDatabase.FindAssets($"t:{nameof(LevelSummaryMessages)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<LevelSummaryMessages>)
                .Single(candidate => candidate != null && candidate.Level == level);

            Assert.That(messages.Guide, Is.Not.Null, $"el resumen del nivel {level} no declara a Algoritm");
            Assert.That(messages.Guide.name, Is.EqualTo(form), "la forma es la del nivel: fuego, rueda, gota (INC-45)");
            Assert.That(messages.Guide.FaceSet, Is.Not.Null, "con la cara final, que parpadea y habla (INC-148)");
        }
    }
}
