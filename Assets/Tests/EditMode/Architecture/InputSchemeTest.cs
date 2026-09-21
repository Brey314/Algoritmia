// El esquema de control del juego entero, cerrado en el Slice 3 (R16): solo clic y clic sostenido
// (RNF-02, CT-06, INC-01). Se lee lo declarado en disco —ajustes del proyecto, el mapa de controles y
// las escenas— porque es la declaración la que decide qué entrada existe, y una inspección sin
// prueba se degrada en la siguiente sesión.

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Game.Architecture.Tests
{
    public class InputSchemeTest
    {
        /// <summary>El único mapa de controles del juego: <c>Assets/Game/Input/ControlesJugables.inputactions</c>.</summary>
        private const string ControlMapGuid = "b3645607813efad45a8b04ad8d306d47";

        private static readonly string[] PlayableScenes =
        {
            "Level1_Cave", "Level2_Forest", "Level2_Workshop", "Level2_Maze", "Level3_River"
        };

        private static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;

        [Test]
        public void Architecture_RNF02_NingunAssemblyUsaLaClaseInputLegada()
        {
            // 1 = «Input System Package (New)»: con ese ajuste la clase Input legada lanza en
            // ejecución, así que ningún código puede depender de ella sin que se note.
            var settings = File.ReadAllText(Path.Combine(ProjectRoot, "ProjectSettings/ProjectSettings.asset"));
            Assert.That(settings, Does.Match(@"activeInputHandler: 1\b"),
                "el manejador de entrada activo es solo el Input System nuevo (CT-06)");

            // Y ningún script del juego la nombra: `Input.GetKey`, `Input.mousePosition`…
            var legado = new Regex(@"(?<![\w.])Input\.(GetKey|GetMouse|GetAxis|GetButton|mousePosition|mouseScrollDelta|anyKey|touch|inputString)");
            var usos = Directory.GetFiles(Path.Combine(Application.dataPath, "Game/Scripts"), "*.cs", SearchOption.AllDirectories)
                .Where(path => legado.IsMatch(File.ReadAllText(path)))
                .Select(path => path.Substring(ProjectRoot.Length + 1))
                .ToArray();

            Assert.That(usos, Is.Empty, "ningún script usa la clase Input legada (RNF-02)");
        }

        [Test]
        public void Architecture_INC01_ElMapaDeControlesNoTieneTecladoMandoNiRueda()
        {
            var mapa = File.ReadAllText(Path.Combine(Application.dataPath, "Game/Input/ControlesJugables.inputactions"));
            var rutas = Regex.Matches(mapa, "\"path\": \"([^\"]*)\"").Select(match => match.Groups[1].Value).ToArray();

            Assert.That(rutas, Is.Not.Empty);
            Assert.That(rutas, Has.All.Matches<string>(EsClic),
                "toda vinculación es un clic o un clic sostenido: puntero, ratón sin rueda, táctil o lápiz (CT-06, INC-01)");
        }

        [Test]
        public void Architecture_RNF02_LasEscenasYElProyectoUsanSoloElMapaDeControlesDelJuego()
        {
            // Cada escena con módulo de entrada apunta al mapa del juego, no al de la plantilla del
            // paquete (`DefaultInputActions`, con teclado y mando), que es lo que se coló en nueve
            // escenas hasta R16.
            var referencia = new Regex(@"m_ActionsAsset: \{fileID: -?\d+, guid: ([0-9a-f]{32})");
            var escenas = Directory.GetFiles(Path.Combine(Application.dataPath, "Game/Scenes"), "*.unity")
                .Select(path => (Nombre: Path.GetFileNameWithoutExtension(path), Guids: referencia.Matches(File.ReadAllText(path)).Select(m => m.Groups[1].Value).ToArray()))
                .ToArray();

            foreach (var jugable in PlayableScenes)
            {
                Assert.That(escenas.Single(escena => escena.Nombre == jugable).Guids, Is.Not.Empty,
                    $"{jugable} lleva el módulo de entrada del Input System (CT-06)");
            }

            var ajenas = escenas.Where(escena => escena.Guids.Any(guid => guid != ControlMapGuid)).Select(escena => escena.Nombre);
            Assert.That(ajenas, Is.Empty, "ninguna escena usa otro mapa de controles que el del juego (RNF-02)");

            // El mapa del proyecto (Input System → Project-wide Actions) entra al ejecutable y se
            // habilita solo al arrancar: también tiene que ser el del juego.
            var build = File.ReadAllText(Path.Combine(ProjectRoot, "ProjectSettings/EditorBuildSettings.asset"));
            Assert.That(build, Does.Match($@"com\.unity\.input\.settings\.actions: \{{fileID: -?\d+, guid: {ControlMapGuid}"),
                "el mapa de acciones del proyecto es ControlesJugables, no la plantilla con teclado (INC-01)");
        }

        private static bool EsClic(string path) =>
            path.StartsWith("<Pointer>/") || path.StartsWith("<Touchscreen>/") || path.StartsWith("<Pen>/")
            || (path.StartsWith("<Mouse>/") && !path.Contains("scroll"));
    }
}
