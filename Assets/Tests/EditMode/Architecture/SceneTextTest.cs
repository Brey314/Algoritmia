// Texto de trabajo que no debe llegar al estudiante (RNF-01, INC-82) y tipografía integrada del
// motor que no debe llegar a ninguna escena (Direccion_de_Arte.md §11.2-§11.3, INC-79). Se lee lo
// declarado en disco, como InputSchemeTest y AudioListenerTest: es la declaración la que decide
// qué ve el estudiante.

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Game.Architecture.Tests
{
    public class SceneTextTest
    {
        // Solo el valor de un Text visible, no el nombre de un GameObject: el "Placeholder" del
        // InputField de nombre (MainMenu) es una pieza legítima de uGUI, no un rótulo de trabajo.
        private static readonly Regex TextoDeTrabajo = new Regex(@"^\s*m_Text: .*placeholder", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static string[] Scenes => Directory.GetFiles($"{Application.dataPath}/Game/Scenes", "*.unity");

        [Test]
        public void Scenes_RNF01_NingunTextoDeEscenaEsUnRotuloDeTrabajo()
        {
            var conRotulo = Scenes
                .Where(path => TextoDeTrabajo.IsMatch(File.ReadAllText(path)))
                .Select(Path.GetFileNameWithoutExtension);

            Assert.That(conRotulo, Is.Empty,
                "ninguna escena deja a la vista un rótulo de trabajo «... · placeholder» (RNF-01, INC-82)");
        }

        [Test]
        public void Scenes_CN04_NingunTextoUsaLaFuenteIntegradaDelMotor()
        {
            var conFuenteIntegrada = Scenes
                .Where(path => File.ReadAllText(path).Contains("m_Font: {fileID: 10102, guid: 0000000000000000e000000000000000"))
                .Select(Path.GetFileNameWithoutExtension);

            Assert.That(conFuenteIntegrada, Is.Empty,
                "ningún Text de ninguna escena usa la fuente integrada del motor: todo va en Baloo 2 o Nunito (Direccion_de_Arte.md §11.2-§11.3, INC-79)");
        }
    }
}
