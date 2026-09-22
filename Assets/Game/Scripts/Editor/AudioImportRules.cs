using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.EditorTools
{
    /// <summary>
    /// Regla de importación del audio del juego: la tabla de <c>Direccion_de_Musica_y_Sonido.md</c>
    /// §4.3, por familia. Hermana de <see cref="ArtImportRules"/> y por la misma razón: las
    /// piezas llegan por entregas que sustituyen el archivo, y un archivo nuevo entra con los
    /// valores de fábrica (comprimido, estéreo, 3D) sin que nadie lo note.
    /// </summary>
    /// <remarks>
    /// La familia sale del prefijo del nombre (§4.1): <c>mus_</c> música, <c>amb_</c> ambiente y
    /// todo lo demás efecto — sin prefijo también es efecto, así ninguna pieza queda sin regla.
    /// Los efectos se parten por duración: hasta dos segundos entran en PCM y precargados, porque
    /// la respuesta a un clic no espera a descomprimir (RF-11); más largos, comprimidos en
    /// memoria.
    ///
    /// **Ojo:** esto pisa lo que se toque a mano en el Inspector para estos campos. Cambiar la
    /// regla es cambiar este archivo.
    /// </remarks>
    internal sealed class AudioImportRules : AssetPostprocessor
    {
        private const string AudioRoot = "Assets/Game/Audio/";

        /// <summary>Un efecto hasta aquí entra en PCM y precargado (§4.3, RF-11).</summary>
        private const float ShortSfxSeconds = 2f;

        private void OnPreprocessAudio()
        {
            if (!assetPath.StartsWith(AudioRoot, StringComparison.Ordinal))
            {
                return;
            }

            var importer = (AudioImporter)assetImporter;
            var name = Path.GetFileName(assetPath);
            var settings = importer.defaultSampleSettings;
            settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
            settings.preloadAudioData = false;
            settings.compressionFormat = AudioCompressionFormat.Vorbis;
            importer.forceToMono = true; // 2D sin espacialización: un efecto en estéreo solo pesa el doble (§4.2)

            if (name.StartsWith("mus_", StringComparison.Ordinal))
            {
                settings.loadType = AudioClipLoadType.Streaming;
                settings.quality = 0.7f;
                importer.forceToMono = false; // la música es la única familia en estéreo
            }
            else if (name.StartsWith("amb_", StringComparison.Ordinal))
            {
                settings.loadType = AudioClipLoadType.Streaming;
                settings.quality = 0.6f;
            }
            else if (WavSeconds(assetPath) > ShortSfxSeconds)
            {
                settings.loadType = AudioClipLoadType.CompressedInMemory;
                settings.quality = 0.7f;
            }
            else
            {
                settings.loadType = AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.PCM;
                settings.preloadAudioData = true;
            }

            importer.defaultSampleSettings = settings;
        }

        /// <summary>
        /// Duración de un <c>.wav</c> leída de su cabecera: en <c>OnPreprocessAudio</c> el clip
        /// todavía no existe. Formato de origen fijado en §4.2 (PCM), así que basta con
        /// canales, frecuencia y bits por muestra.
        /// </summary>
        private static float WavSeconds(string path)
        {
            using var reader = new BinaryReader(File.OpenRead(path));
            reader.BaseStream.Seek(22, SeekOrigin.Begin);
            var channels = reader.ReadInt16();
            var sampleRate = reader.ReadInt32();
            reader.BaseStream.Seek(34, SeekOrigin.Begin);
            var bitsPerSample = reader.ReadInt16();
            var bytesPerSecond = sampleRate * channels * bitsPerSample / 8f;
            // ponytail: se descuentan solo los 44 bytes de la cabecera canónica; un chunk LIST
            // extra desplaza la cuenta unos milisegundos, irrelevante contra un umbral de 2 s.
            return bytesPerSecond > 0f ? (reader.BaseStream.Length - 44f) / bytesPerSecond : 0f;
        }
    }
}
