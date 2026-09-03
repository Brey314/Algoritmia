// Reglas de dependencia entre assemblies del proyecto (RNF-15, RNF-16, INC-40).
//
// Se leen los .asmdef en disco, no los ensamblados cargados en el dominio: un módulo que
// todavía no tiene código no produce DLL, y de todos modos lo que hay que fijar es la
// *declaración* — es la referencia del .asmdef la que Unity usa para impedir el `using`.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Game.Architecture.Tests
{
    public class AssemblyDependencyTest
    {
        /// <summary>Módulos de runtime del proyecto, con su ruta bajo <c>Scripts/Runtime/</c>.</summary>
        private static readonly (string Assembly, string Path)[] RuntimeModules =
        {
            ("Game.Core", "Core"),
            ("Game.Scaffolding", "Scaffolding"),
            ("Game.Levels.Fire", "Levels/Fire"),
            ("Game.UI", "UI"),
            ("Game.Audio", "Audio")
        };

        private const string LevelPrefix = "Game.Levels.";

        private Dictionary<string, AssemblyDefinition> _definitions;

        [SetUp]
        public void LoadDefinitions()
        {
            _definitions = Directory
                .GetFiles(Application.dataPath, "*.asmdef", SearchOption.AllDirectories)
                .Select(path => AssemblyDefinition.Read(path))
                .ToDictionary(definition => definition.name);
        }

        [Test]
        public void Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas()
        {
            foreach (var (assembly, _) in RuntimeModules)
            {
                Assert.That(_definitions.Keys, Contains.Item(assembly));
                Assert.That(_definitions.Keys, Contains.Item($"{assembly}.Tests"));
            }
        }

        [Test]
        public void Architecture_RNF15_ElNombreDelAssemblyCoincideConSuRutaBajoScripts()
        {
            foreach (var (assembly, path) in RuntimeModules)
            {
                var expected = $"{Application.dataPath}/Game/Scripts/Runtime/{path}/{assembly}.asmdef";
                Assert.That(_definitions[assembly].SourcePath, Is.EqualTo(expected));
                // El rootNamespace fija la convención de namespaces: ruta bajo Scripts/ elidiendo Runtime.
                Assert.That(_definitions[assembly].rootNamespace, Is.EqualTo(assembly));
            }
        }

        [Test]
        public void Architecture_RNF16_CoreNoDependeDeUINiDeAudioNiDeNiveles()
        {
            var actual = DependenciesOf("Game.Core");

            Assert.That(actual, Does.Not.Contain("Game.UI"));
            Assert.That(actual, Does.Not.Contain("Game.Audio"));
            Assert.That(actual.Where(name => name.StartsWith(LevelPrefix)), Is.Empty);
        }

        [Test]
        public void Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel()
        {
            var levels = RuntimeModules
                .Select(module => module.Assembly)
                .Where(name => name.StartsWith(LevelPrefix));
            foreach (var level in levels)
            {
                var others = DependenciesOf(level).Where(name => name.StartsWith(LevelPrefix));

                Assert.That(others, Is.Empty, $"{level} depende de otro nivel");
            }
        }

        /// <summary>Cierre transitivo de las referencias del proyecto, excluida la raíz.</summary>
        private HashSet<string> DependenciesOf(string assembly)
        {
            var visited = new HashSet<string>();
            var pending = new Queue<string>(_definitions[assembly].references);
            while (pending.Count > 0)
            {
                var current = pending.Dequeue();
                if (!_definitions.ContainsKey(current) || !visited.Add(current))
                {
                    continue; // Fuera del proyecto (UnityEngine.TestRunner, nunit…) o ya recorrido.
                }

                foreach (var reference in _definitions[current].references)
                {
                    pending.Enqueue(reference);
                }
            }

            return visited;
        }

        private class AssemblyDefinition
        {
#pragma warning disable CS0649 // Lo rellena JsonUtility.
            public string name;
            public string rootNamespace;
            public string[] references;
#pragma warning restore CS0649

            public string SourcePath { get; private set; }

            public static AssemblyDefinition Read(string path)
            {
                var definition = JsonUtility.FromJson<AssemblyDefinition>(File.ReadAllText(path));
                definition.SourcePath = path.Replace(Path.DirectorySeparatorChar, '/');
                return definition;
            }
        }
    }
}
