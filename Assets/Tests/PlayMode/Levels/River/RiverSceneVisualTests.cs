using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// Capturas de la orilla para la revisión con el usuario (Checkpoint R-C): al abrir y con los
    /// cuatro materiales recogidos y la zona abierta. La aserción es que la captura existe; lo que
    /// dice es de Santiago.
    /// </summary>
    [Category("Integration")]
    public class RiverSceneVisualTests
    {
        [Test]
        [Timeout(30000)]
        [Category("VisualVerification")]
        [Description("Revisar la captura: lista de tareas arriba a la izquierda, inventario vacío abajo a la " +
                     "izquierda, flechas y «Recoger» a la derecha, zona marcada junto al agua y los cuatro " +
                     "materiales repartidos por la orilla (mockup 11-12).")]
        public async Task RiverScene_RF36_CapturaDeLaOrillaAlAbrir()
        {
            await RiverMovementTests.OpenRiver();
            await Awaitable.EndOfFrameAsync();
            CaptureScreenshot("RiverScene_RF36_OrillaAlAbrir");
        }

        [Test]
        [Timeout(30000)]
        [Category("VisualVerification")]
        [Description("Revisar la captura: tareas 1 y 2 con visto verde, 3 y 4 con círculo; inventario con " +
                     "los cuatro materiales; panel de ensamblaje abierto y flechas retiradas.")]
        public async Task RiverScene_RF39_CapturaConTodoRecogidoYLaZonaAbierta()
        {
            var river = await RiverMovementTests.OpenRiver();
            foreach (var (collectible, _) in river.Spawned.ToArray())
            {
                WalkTo(river, collectible.Position);
                river.CollectButton.onClick.Invoke();
            }

            WalkTo(river, river.Config.BuildZonePosition);
            Assert.That(river.Zone.IsOpen, Is.True);
            await Awaitable.NextFrameAsync();
            await Awaitable.EndOfFrameAsync();
            CaptureScreenshot("RiverScene_RF39_ZonaAbierta");
        }

        private static void WalkTo(RiverSceneController river, Vector2 target)
        {
            for (var step = 0; step < 2000 && !river.Zone.IsOpen && Vector2.Distance(river.Walk.Position, target) > 0.005f; step++)
            {
                river.Tick(target - river.Walk.Position, 0.02f);
            }
        }

        /// <remarks>Copia del patrón de <c>CaveLightingTests</c>: cada archivo lleva su propio helper.</remarks>
        private static void CaptureScreenshot(string name)
        {
            if (Application.isBatchMode)
            {
                Assert.Ignore("La captura exige una Game View: correr desde el Editor.");
            }

            var directory = $"{Application.persistentDataPath}/TestScreenshots";
            Directory.CreateDirectory(directory);
            var path = $"{directory}/{name}.png";
            File.Delete(path);

            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.Destroy(texture);

            Assert.That(File.Exists(path), Is.True, $"no se escribió la captura en «{path}»");
            TestContext.WriteLine($"Captura: {path}");
        }
    }
}
