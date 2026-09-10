using System.Linq;
using NUnit.Framework;
using UnityEditor;

namespace Game.Levels.Fire.Tests
{
    [TestFixture]
    public class FireMessagesTests
    {
        private static FireMessages CargarAssetDeMensajes() =>
            AssetDatabase.FindAssets($"t:{nameof(FireMessages)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<FireMessages>)
                .Single(messages => messages != null);

        private static string[] TodosLosMensajes(FireMessages messages) => new[]
        {
            messages.FarSparksFade,
            messages.FarNeverReach,
            messages.NearColdStone,
            messages.NearBesideLeaves,
            messages.FirstEffectiveStrike,
            messages.SecondEffectiveStrike,
            messages.FinalEffectiveStrike,
            messages.BlowSuccess
        };

        [Test]
        [Category("Acceptance")]
        public void FireMessages_RNF18_LosOchoMensajesDelGuionEstanDefinidos()
        {
            var sut = CargarAssetDeMensajes();

            Assert.That(TodosLosMensajes(sut),
                Has.All.Matches<string>(texto => !string.IsNullOrWhiteSpace(texto)));
        }

        [Test]
        [Category("Acceptance")]
        public void FireMessages_RF17_NingunMensajeDelAssetContieneCifras()
        {
            var sut = CargarAssetDeMensajes();

            Assert.That(TodosLosMensajes(sut),
                Has.None.Matches<string>(texto => texto.Any(char.IsDigit)));
        }
    }
}
