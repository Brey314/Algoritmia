using System;
using System.Collections.Generic;
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
    /// **Dos excepciones, las dos por el peso del paquete (RNF-06).**
    ///
    /// *Los cuadros del fuego y el humo del N1* (<c>Art/Props/Fire/Animations/</c>). Son 67 PNG (134
    /// entradas del BuildReport) de hasta 2144×2108 y sin comprimir pesaban 533 MB, con lo que el
    /// paquete llegó a 866,7 MB (826,6 MiB) frente al tope de 500 MB. Ya son
    /// un dibujo por clave, sin duplicados, así que no hay arreglo sin pérdida. En pantalla el fuego normal se ve a ~650 px como
    /// mucho (Size ≤ 0,22 del alto) y el humo a menos de 300 px, de modo que 1024 px no se nota. Se
    /// **sigue sin comprimir**, por el mismo motivo de la regla general: la rejilla de bloques de
    /// 4×4 sobre degradados largos. Cambiar el tope es cambiar <c>FireFramesMaxTextureSize</c> y
    /// <c>ArtImport_RNF06_…</c>.
    ///
    /// *Los cinco props pequeños del N1* (<c>prop_n1_pedernal</c>, <c>prop_n1_silex</c>,
    /// <c>prop_n1_hoja</c>, <c>prop_n1_monton_hojas</c> y <c>prop_n1_monton_hojas_cenital</c>, en
    /// <c>Art/Props/Fire/</c>; INC-146). El 09/10/2026 el ejecutable midió 499,3 MB frente al tope de
    /// 500 MB de RNF-06, con 0,7 MB de margen (build sobre <c>0212469</c>), y cualquier entrega de
    /// arte nueva lo ponía en riesgo. Los cinco llegan a 2000×2000 y, sin comprimir, ocupan 15,3 MB cada uno
    /// en el build. En pantalla no se acercan a eso: en <c>Level1_Cave</c> las hojas miden 84 px, las
    /// piedras 72 px y el montón cenital 300 px; en las narrativas las piedras y las hojas llegan a
    /// ~96–135 px y el montón de frente a ~230–280 px con el zoom más cerrado. **Decisión de
    /// Santiago (09/10/2026): van a 256×256**, también sin comprimir (≈ 0,26 MB cada uno, unos 75 MB
    /// menos en total). Las piedras y las hojas se siguen viendo reducidas (256 px de textura para
    /// ≤ 135 px en pantalla); los dos montones se amplían como mucho ~1,1–1,2× con el zoom más
    /// cerrado, y esa pérdida se aceptó. La lista es explícita, por ruta completa, y no una carpeta:
    /// <c>Art/Props/Fire/</c> guarda también las animaciones (que tienen su tope) y lo que llegue
    /// después, que debe entrar a 4096. Cambiar el tope o añadir un prop a la lista es cambiar
    /// <c>SmallPropPaths</c> y <c>ArtImport_RNF06_…</c>.
    ///
    /// **Ojo:** esto pisa lo que se toque a mano en el Inspector para estos dos campos. Cambiar la
    /// regla es cambiar este archivo. Un cambio de tope no reimporta solo lo que ya está en el
    /// proyecto: los <c>.meta</c> se actualizan reimportando desde el Editor.
    /// </remarks>
    internal sealed class ArtImportRules : AssetPostprocessor
    {
        private const string ArtRoot = "Assets/Game/Art/";

        /// <summary>Lado máximo, en píxeles: las panorámicas se entregan a 3840 y no se reducen.</summary>
        private const int MaxTextureSize = 4096;

        private const string FireFramesRoot = ArtRoot + "Props/Fire/Animations/";

        /// <summary>Tope de los cuadros del fuego y el humo (RNF-06, ver la nota de la clase).</summary>
        private const int FireFramesMaxTextureSize = 1024;

        /// <summary>Tope de los cinco props pequeños del N1 (RNF-06, INC-146, ver la nota de la clase).</summary>
        private const int SmallPropsMaxTextureSize = 256;

        /// <summary>
        /// Los cinco props del N1 que entran a <see cref="SmallPropsMaxTextureSize"/>, por ruta
        /// completa: lista explícita y no carpeta, porque <c>Props/Fire/</c> guarda también lo que
        /// debe entrar a <see cref="MaxTextureSize"/>.
        /// </summary>
        private static readonly HashSet<string> SmallPropPaths = new HashSet<string>(StringComparer.Ordinal)
        {
            ArtRoot + "Props/Fire/prop_n1_pedernal.png",
            ArtRoot + "Props/Fire/prop_n1_silex.png",
            ArtRoot + "Props/Fire/prop_n1_hoja.png",
            ArtRoot + "Props/Fire/prop_n1_monton_hojas.png",
            ArtRoot + "Props/Fire/prop_n1_monton_hojas_cenital.png",
        };

        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ArtRoot, StringComparison.Ordinal))
            {
                return;
            }

            var importer = (TextureImporter)assetImporter;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = MaxTextureSizeFor(assetPath);
        }

        private static int MaxTextureSizeFor(string path)
        {
            if (path.StartsWith(FireFramesRoot, StringComparison.Ordinal))
            {
                return FireFramesMaxTextureSize;
            }

            return SmallPropPaths.Contains(path) ? SmallPropsMaxTextureSize : MaxTextureSize;
        }
    }
}
