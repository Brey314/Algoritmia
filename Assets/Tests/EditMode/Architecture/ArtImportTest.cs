// Cómo entra el arte del juego al proyecto (RNF-23).
//
// La regla vive en Game.EditorTools/ArtImportRules.cs, que corre en cada importación. Esto es lo
// que avisa si alguien la quita, la rodea con un ajuste a mano en el Inspector, o si una entrega
// nueva se cuela con los valores de fábrica: 2048 px y comprimida.

using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Architecture.Tests
{
    public class ArtImportTest
    {
        private const string ArtRoot = "Assets/Game/Art";

        /// <summary>
        /// Sin comprimir y a su resolución: el arte es plano, de degradados largos, y el Nivel 1
        /// lo multiplica por la capa de oscuridad. Comprimido se ve la rejilla de bloques de 4×4 y
        /// los colores se corren (16/09/2026).
        /// </summary>
        [Test]
        public void ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir()
        {
            var textures = AssetDatabase.FindAssets("t:Texture2D", new[] { ArtRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => Path.GetExtension(path) == ".png")
                .ToArray();

            Assert.That(textures, Is.Not.Empty, $"«{ArtRoot}» tiene ilustraciones que revisar");

            foreach (var path in textures)
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.GetSourceTextureWidthAndHeight(out var width, out var height);

                Assert.That(importer.textureCompression,
                    Is.EqualTo(TextureImporterCompression.Uncompressed),
                    $"{path} entra comprimida");
                Assert.That(importer.maxTextureSize, Is.GreaterThanOrEqualTo(Mathf.Max(width, height)),
                    $"{path} se entrega a {width}×{height} y el importador la reduce");
            }
        }
    }
}
