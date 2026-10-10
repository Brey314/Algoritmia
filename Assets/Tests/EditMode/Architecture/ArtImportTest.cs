// Cómo entra el arte del juego al proyecto (RNF-23).
//
// La regla vive en Game.EditorTools/ArtImportRules.cs, que corre en cada importación. Esto es lo
// que avisa si alguien la quita, la rodea con un ajuste a mano en el Inspector, o si una entrega
// nueva se cuela con los valores de fábrica: 2048 px y comprimida.

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Architecture.Tests
{
    public class ArtImportTest
    {
        private const string ArtRoot = "Assets/Game/Art";

        // Primera excepción a «sin reducir»: los cuadros del fuego y el humo del N1 (RNF-06, ver
        // ArtImportRules). Va a 1024 y sigue sin comprimir.
        private const string FireFramesRoot = "Assets/Game/Art/Props/Fire/Animations/";
        private const int FireFramesMaxSize = 1024;

        // Segunda excepción: los cinco props pequeños del N1 (RNF-06, INC-146, ver ArtImportRules).
        // Entregados a 2000×2000, van a 256 y siguen sin comprimir. Las rutas se repiten aquí a
        // propósito: este assembly no referencia ningún Game.* y la prueba no debe heredar la lista
        // del código que vigila.
        private const int SmallPropsMaxSize = 256;
        private static readonly string[] SmallPropPaths =
        {
            "Assets/Game/Art/Props/Fire/prop_n1_pedernal.png",
            "Assets/Game/Art/Props/Fire/prop_n1_silex.png",
            "Assets/Game/Art/Props/Fire/prop_n1_hoja.png",
            "Assets/Game/Art/Props/Fire/prop_n1_monton_hojas.png",
            "Assets/Game/Art/Props/Fire/prop_n1_monton_hojas_cenital.png",
        };

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
                if (path.StartsWith(FireFramesRoot) || SmallPropPaths.Contains(path))
                {
                    continue; // las excepciones tienen su propia prueba, que exige el tope exacto
                }

                Assert.That(importer.maxTextureSize, Is.GreaterThanOrEqualTo(Mathf.Max(width, height)),
                    $"{path} se entrega a {width}×{height} y el importador la reduce");
            }
        }

        /// <summary>
        /// RNF-06 (paquete &lt; 500 MB): los cuadros del fuego y el humo del N1 se importan a 1024 px,
        /// sin comprimir. A tamaño completo pesaban 533 MB y llevaron el build a 827 MB; en pantalla
        /// el fuego mide como mucho ~650 px y el humo menos de 300.
        /// </summary>
        [Test]
        public void ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir()
        {
            var textures = AssetDatabase.FindAssets("t:Texture2D", new[] { FireFramesRoot.TrimEnd('/') })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => Path.GetExtension(path) == ".png")
                .ToArray();

            Assert.That(textures, Is.Not.Empty, $"«{FireFramesRoot}» tiene cuadros que revisar");

            foreach (var path in textures)
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);

                Assert.That(importer.maxTextureSize, Is.EqualTo(FireFramesMaxSize), $"{path}: tope de textura");
                Assert.That(importer.textureCompression,
                    Is.EqualTo(TextureImporterCompression.Uncompressed),
                    $"{path} entra comprimida");
            }
        }

        /// <summary>
        /// RNF-06 (paquete &lt; 500 MB, INC-146): el pedernal, el sílex, la hoja y los dos montones de
        /// hojas del N1 se importan a 256 px, sin comprimir. Entregados a 2000×2000 pesaban 15,3 MB
        /// cada uno en el build (499,3 MB el 09/10/2026, con 0,7 MB de margen); en pantalla miden
        /// entre 72 y 300 px.
        /// </summary>
        [Test]
        public void ArtImport_RNF06_LosCincoPropsDelN1SeImportanA256SinComprimir()
        {
            foreach (var path in SmallPropPaths)
            {
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                Assert.That(importer, Is.Not.Null, $"{path}: no hay importador de textura (¿se movió o se renombró?)");
                Assert.That(importer.maxTextureSize, Is.EqualTo(SmallPropsMaxSize), $"{path}: tope de textura");
                Assert.That(importer.textureCompression,
                    Is.EqualTo(TextureImporterCompression.Uncompressed),
                    $"{path} entra comprimida");
            }
        }

        // Los cuadros de animación del N1 conservan el nombre de entrega (fuego_cenital_nivel_1_0000,
        // humo_nivel_1_0000) y no se renombran: los referencia la curva PPtr del .anim, y
        // renombrarlos es trabajo del motor (CLAUDE.md, carril de arte). Es la única excepción.
        private static readonly Regex DeliveryFrame = new Regex(@"^(fuego|humo)_([a-z]+_)?nivel_1_\d{4}$");

        // Direccion_de_Arte.md §15.4: prefijo de tipo, minúsculas, sin tildes ni espacios.
        private static readonly Regex Nomenclature = new Regex(@"^(char|prop|env|ui|fx|ref)_[a-z0-9_]+$");

        /// <summary>
        /// INC-126: ningún .png de Art/ rompe la nomenclatura de la dirección de arte, salvo los
        /// cuadros de animación con nombre de entrega.
        /// </summary>
        [Test]
        public void ArtImport_RNF23_LosNombresSiguenLaNomenclatura()
        {
            var offenders = AssetDatabase.FindAssets("t:Texture2D", new[] { ArtRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => Path.GetExtension(path) == ".png")
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !Nomenclature.IsMatch(name) && !DeliveryFrame.IsMatch(name))
                .ToArray();

            Assert.That(offenders, Is.Empty, "nombres fuera de la nomenclatura (§15.4)");
        }
    }
}
