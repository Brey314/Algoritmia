// Dónde vive el oyente de audio (RNF-13: el juego completo se recorre con sonido, sin avisos).
//
// Las escenas de flujo no tienen cámara —todo es uGUI en overlay— y sin AudioListener Unity no
// reproduce nada. El único oyente va en el objeto persistente de Boot, con el AudioManager; una
// cámara de nivel con el suyo produce la otra advertencia: dos oyentes vivos.

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Game.Architecture.Tests
{
    public class AudioListenerTest
    {
        /// <summary>Un AudioListener serializado es un documento YAML de clase 81.</summary>
        private static readonly Regex Listener = new Regex(@"^--- !u!81 ", RegexOptions.Multiline);

        [Test]
        public void AudioListener_RNF13_ElUnicoOyenteVaEnBootYNingunaOtraEscenaTieneElSuyo()
        {
            var scenes = Directory.GetFiles($"{Application.dataPath}/Game/Scenes", "*.unity")
                .ToDictionary(Path.GetFileNameWithoutExtension, path => Listener.Matches(File.ReadAllText(path)).Count);

            Assert.That(scenes["Boot"], Is.EqualTo(1), "Boot lleva el oyente en Persistent, junto al AudioManager");
            Assert.That(scenes.Where(scene => scene.Key != "Boot" && scene.Value > 0).Select(scene => scene.Key), Is.Empty,
                "ninguna otra escena declara un AudioListener: con el de Boot vivo serían dos");
        }
    }
}
