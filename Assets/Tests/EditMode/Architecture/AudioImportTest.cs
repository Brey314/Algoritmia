// Cómo entra el audio del juego al proyecto (Direccion_de_Musica_y_Sonido.md §2.1, §4.3).
//
// La regla de importación vive en Game.EditorTools/AudioImportRules.cs. Esto avisa si alguien la
// quita, la rodea a mano en el Inspector, o si una entrega nueva se cuela con los valores de
// fábrica — y vigila el corolario de §2.1: ninguna pieza de este juego se llama derrota.

using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Architecture.Tests
{
    public class AudioImportTest
    {
        private const string AudioRoot = "Assets/Game/Audio";

        /// <summary>Hasta aquí un efecto entra en PCM y precargado (§4.3, RF-11).</summary>
        private const float ShortSfxSeconds = 2f;

        private static string[] Clips() => AssetDatabase.FindAssets("t:AudioClip", new[] { AudioRoot })
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToArray();

        /// <summary>
        /// Se verifica por inspección, dice §2.1: un intento sin éxito suena descriptivo, nunca
        /// punitivo, y la primera señal de un sonido de fallo es su nombre (CP-02).
        /// </summary>
        [Test]
        public void AudioAssets_CP02_NingunaPiezaSeLlamaDerrotaNiError()
        {
            var forbidden = new[] { "derrota", "gameover", "error", "fallo", "wrong" };
            var clips = Clips();
            Assert.That(clips, Is.Not.Empty, $"«{AudioRoot}» tiene piezas que revisar");

            foreach (var path in clips)
            {
                var name = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
                Assert.That(forbidden.Where(name.Contains), Is.Empty,
                    $"{path}: un sonido de fallo no existe en este juego (§2.1, CP-02)");
            }
        }

        /// <summary>
        /// La tabla de §4.3: música y ambientes en streaming (la música en estéreo, todo lo
        /// demás mono), efectos cortos en PCM precargado y efectos largos comprimidos en memoria.
        /// Es lo que mantiene el audio dentro de su techo de RNF-06 sin retirar cobertura (§15).
        /// </summary>
        [Test]
        public void AudioImport_RNF06_CadaFamiliaEntraConLosAjustesDeSuTabla()
        {
            var clips = Clips();
            Assert.That(clips, Is.Not.Empty, $"«{AudioRoot}» tiene piezas que revisar");

            foreach (var path in clips)
            {
                var importer = (AudioImporter)AssetImporter.GetAtPath(path);
                var settings = importer.defaultSampleSettings;
                var name = Path.GetFileName(path);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);

                Assert.That(settings.sampleRateSetting, Is.EqualTo(AudioSampleRateSetting.PreserveSampleRate), $"{name}: frecuencia de origen");

                if (name.StartsWith("mus_"))
                {
                    Assert.That(settings.loadType, Is.EqualTo(AudioClipLoadType.Streaming), $"{name}: música en streaming");
                    Assert.That(settings.compressionFormat, Is.EqualTo(AudioCompressionFormat.Vorbis), $"{name}: música en Vorbis");
                    Assert.That(importer.forceToMono, Is.False, $"{name}: la música es la única familia en estéreo");
                }
                else if (name.StartsWith("amb_"))
                {
                    Assert.That(settings.loadType, Is.EqualTo(AudioClipLoadType.Streaming), $"{name}: ambiente en streaming");
                    Assert.That(settings.compressionFormat, Is.EqualTo(AudioCompressionFormat.Vorbis), $"{name}: ambiente en Vorbis");
                    Assert.That(importer.forceToMono, Is.True, $"{name}: ambiente mono");
                }
                else if (clip.length > ShortSfxSeconds)
                {
                    Assert.That(settings.loadType, Is.EqualTo(AudioClipLoadType.CompressedInMemory), $"{name}: efecto largo comprimido en memoria");
                    Assert.That(settings.compressionFormat, Is.EqualTo(AudioCompressionFormat.Vorbis), $"{name}: efecto largo en Vorbis");
                    Assert.That(importer.forceToMono, Is.True, $"{name}: efecto mono");
                }
                else
                {
                    Assert.That(settings.loadType, Is.EqualTo(AudioClipLoadType.DecompressOnLoad), $"{name}: efecto corto descomprimido al cargar");
                    Assert.That(settings.compressionFormat, Is.EqualTo(AudioCompressionFormat.PCM), $"{name}: efecto corto en PCM");
                    Assert.That(settings.preloadAudioData, Is.True, $"{name}: efecto corto precargado — un clic no espera a descomprimir (RF-11)");
                    Assert.That(importer.forceToMono, Is.True, $"{name}: efecto mono");
                }
            }
        }
    }
}
