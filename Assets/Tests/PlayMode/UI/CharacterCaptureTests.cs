using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI.Tests
{
    /// <summary>
    /// Capturas de las cinco mecánicas recién abiertas, con los personajes en su sitio, para revisar
    /// la composición: la interfaz se usa y los personajes se ven (Dirección de arte §13.3).
    /// </summary>
    [Category("Integration")]
    public class CharacterCaptureTests
    {
        [TestCase("Level1_Cave")]
        [TestCase("Level2_Forest")]
        [TestCase("Level2_Workshop")]
        [TestCase("Level2_Maze")]
        [TestCase("Level3_River")]
        [Timeout(30000)]
        [Category("VisualVerification")]
        [Description("Verificar en las capturas Personajes_Mecanica_*: los personajes se ven enteros, a escala " +
                     "coherente, sin tapar la tablilla, los botones ni las piezas; el botón de ayuda es el círculo " +
                     "con Algoritm del nivel dentro.")]
        public async Task Personajes_DA133_CapturaCadaMecanicaConSusPersonajes(string escena)
        {
            var load = SceneManager.LoadSceneAsync(escena, LoadSceneMode.Single);
            while (load is { isDone: false })
            {
                await Awaitable.NextFrameAsync();
            }

            var inicio = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - inicio < 1.5f)
            {
                await Awaitable.NextFrameAsync();
            }

            if (Application.isBatchMode)
            {
                return; // sin Game View no hay captura
            }

            var carpeta = $"{Application.persistentDataPath}/TestScreenshots";
            System.IO.Directory.CreateDirectory(carpeta);
            var textura = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.File.WriteAllBytes($"{carpeta}/Personajes_Mecanica_{escena}.png", textura.EncodeToPNG());
            Object.Destroy(textura);
        }
    }
}
