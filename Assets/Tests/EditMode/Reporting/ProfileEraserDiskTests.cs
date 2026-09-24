using System;
using System.IO;
using System.Linq;
using Game.Core;
using NUnit.Framework;

namespace Game.Reporting.Tests
{
    /// <summary>
    /// El criterio de verificación de RNF-11 es «ausencia de residuos en el almacenamiento
    /// local»: <c>Game.Core.Tests.SaveStoreTests</c> lo prueba con un <see cref="IFileSystem"/> en
    /// memoria, y esta clase lo prueba mirando de verdad el disco (R3, Slice 4 P10). No hay una
    /// clase <c>ProfileEraser</c> nueva que probar: el borrado ya vive en
    /// <see cref="SaveStore.Delete"/> desde el Slice 1, con la misma traza RF-47 — ver
    /// <c>SaveStore_RF47_*</c> y <c>SaveStore_INC34_*</c> en <c>Game.Core.Tests</c>. Lo que
    /// faltaba era la verificación sobre archivos reales, y es lo único que añade esta clase.
    /// </summary>
    public class ProfileEraserDiskTests
    {
        private string _root;
        private string _portableRoot;
        private string _fallbackRoot;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), "AlgoritmiaProfileEraserTests_" + Guid.NewGuid().ToString("N"));
            _portableRoot = Path.Combine(_root, "Datos").Replace('\\', '/');
            _fallbackRoot = Path.Combine(_root, "Respaldo").Replace('\\', '/');

            // Guarda de R3 — escrita antes que cualquier borrado: esta prueba nunca puede tocar
            // «Datos/» del proyecto ni el persistentDataPath real.
            Assert.That(_root, Does.Not.Contain("Algoritmia\\Datos").And.Not.Contain("Algoritmia/Datos"));
            Assert.That(_portableRoot, Is.Not.EqualTo(UnityEngine.Application.persistentDataPath).IgnoreCase);
            Assert.That(_fallbackRoot, Is.Not.EqualTo(UnityEngine.Application.persistentDataPath).IgnoreCase);
        }

        [TearDown]
        public void TearDown()
        {
            if (!Directory.Exists(_root))
            {
                return;
            }

            // Una prueba puede dejar el directorio marcado de solo lectura: quitarlo antes de
            // borrar, o la limpieza fallaría y ensuciaría la siguiente corrida.
            foreach (var dir in Directory.GetDirectories(_root, "*", SearchOption.AllDirectories).Append(_root))
            {
                var attributes = File.GetAttributes(dir);
                if ((attributes & FileAttributes.ReadOnly) != 0)
                {
                    File.SetAttributes(dir, attributes & ~FileAttributes.ReadOnly);
                }
            }

            Directory.Delete(_root, recursive: true);
        }

        [Test]
        public void ProfileEraser_RNF11_SobreDiscoRealNoQuedaNingunaEntradaDelPerfil()
        {
            var store = new SaveStore(new DiskFileSystem(), _portableRoot, _fallbackRoot);
            store.Save(PlayerProfile.Create("Ana", Array.Empty<string>()).Profile);
            store.Save(PlayerProfile.Create("Beto", Array.Empty<string>()).Profile);

            var deleted = store.Delete("Ana");

            Assert.That(deleted, Is.True);
            var nombres = Directory.GetFiles(_root, "*", SearchOption.AllDirectories).Select(Path.GetFileName);
            Assert.That(nombres, Has.None.Contains("Ana.json"));
            Assert.That(nombres, Has.Some.Contains("Beto.json"));
        }

        [Test]
        public void ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento()
        {
            // Escenario 1 (portable escribible) ya lo cubre la prueba anterior. Este cubre el
            // escenario 2: «Datos/» de solo lectura desde el principio, así que SaveStore cae al
            // respaldo (INC-34) y el borrado tiene que alcanzar esa ruta sin dejar residuo.
            Directory.CreateDirectory(_portableRoot);
            File.SetAttributes(_portableRoot, File.GetAttributes(_portableRoot) | FileAttributes.ReadOnly);

            if (!ReadOnlyIsEnforced(_portableRoot))
            {
                Assert.Ignore("Este equipo no hace cumplir el atributo de solo lectura sobre carpetas: " +
                    "el escenario de solo lectura de INC-34 no se puede simular aquí.");
                return;
            }

            var store = new SaveStore(new DiskFileSystem(), _portableRoot, _fallbackRoot);
            Assume.That(store.UsingFallback, Is.True, "precondición: la portable de solo lectura fuerza el respaldo");
            store.Save(PlayerProfile.Create("Ana", Array.Empty<string>()).Profile);

            var deleted = store.Delete("Ana");

            Assert.That(deleted, Is.True);
            var restantes = Directory.Exists(_fallbackRoot)
                ? Directory.GetFiles(_fallbackRoot, "*", SearchOption.AllDirectories)
                : Array.Empty<string>();
            Assert.That(restantes, Is.Empty, "RNF-11: sin residuos en la ruta de respaldo tras el borrado");
        }

        /// <summary>Prueba de verdad si el atributo de solo lectura restringe la escritura, en vez de asumirlo.</summary>
        private static bool ReadOnlyIsEnforced(string directory)
        {
            var probe = Path.Combine(directory, ".probe");
            try
            {
                File.WriteAllText(probe, string.Empty);
                File.Delete(probe);
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
        }
    }
}
