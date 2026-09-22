using System;
using UnityEditor;

namespace Game.EditorTools
{
    /// <summary>
    /// Regla de importación del arte del juego: las ilustraciones entran **sin comprimir** y a la
    /// resolución con la que las entrega la colaboradora (RNF-23, acta D06).
    /// </summary>
    /// <remarks>
    /// **Por qué no la compresión por defecto de Unity.** El arte es plano, de degradados largos y
    /// zonas muy oscuras, y el Nivel 1 lo multiplica además por la capa de oscuridad. Con BC7 —y
    /// peor con DXT1, que es lo que le toca a un archivo recién soltado en la carpeta— cada bloque
    /// de 4×4 píxeles recibe su propio par de colores y la pared de la cueva se ve como una rejilla
    /// (captura del 16/09/2026); en espacio lineal el error se amplifica justo en los tonos
    /// oscuros. Sin comprimir, las 44 texturas de <c>Art/</c> ocupan ~100 MB, muy por debajo de
    /// RNF-05 (2 GB).
    ///
    /// **Por qué en el importador y no en cada <c>.meta</c>.** El arte definitivo llega por entregas
    /// semanales que *sustituyen el archivo* y añaden piezas nuevas; un archivo nuevo entra con los
    /// valores de fábrica (2048, comprimido) y nadie lo nota hasta verlo en pantalla. Aquí la regla
    /// se aplica sola, incluido el reimport.
    ///
    /// **Ojo:** esto pisa lo que se toque a mano en el Inspector para estos dos campos. Cambiar la
    /// regla es cambiar este archivo.
    /// </remarks>
    internal sealed class ArtImportRules : AssetPostprocessor
    {
        private const string ArtRoot = "Assets/Game/Art/";

        /// <summary>Lado máximo, en píxeles: las panorámicas se entregan a 3840 y no se reducen.</summary>
        private const int MaxTextureSize = 4096;

        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ArtRoot, StringComparison.Ordinal))
            {
                return;
            }

            var importer = (TextureImporter)assetImporter;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = MaxTextureSize;
        }
    }
}
