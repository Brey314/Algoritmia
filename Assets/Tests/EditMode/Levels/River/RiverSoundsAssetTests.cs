using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// El fondo del Nivel 3 no tiene costuras: la orilla y las escenas del río piden los mismos
    /// dos clips, y pedir lo que ya suena no lo reinicia (§3.3). Si una escena cambia de toma y la
    /// orilla no, el río se corta al entrar a jugar.
    /// </summary>
    public class RiverSoundsAssetTests
    {
        private static readonly string[] EscenasDelRio =
        {
            "N3_PuenteII_Rio", "N3_Escena31_Llegada", "N3_Escena32_PrimerIntento", "N3_Escena33_Cruce"
        };

        private static T Asset<T>(string name) where T : Object => AssetDatabase.FindAssets($"t:{typeof(T).Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>)
            .Single(asset => asset.name == name);

        [Test]
        public void RiverSounds_RF05_LasEscenasDelRioSuenanComoLaOrilla()
        {
            var sonidos = Asset<RiverSounds>("N3_Sonidos");
            var escenas = EscenasDelRio.Select(Asset<NarrativeSequence>).ToArray();

            Assert.That(sonidos.RiverAmbient.name, Is.EqualTo("amb_n3_rio_orilla"), "el río de la orilla");
            Assert.That(sonidos.ForestAmbient.name, Is.EqualTo("amb_n2_bosque_dia"), "y el bosque de día encima");
            Assert.That(escenas.Select(escena => escena.Ambient), Has.All.SameAs(sonidos.RiverAmbient),
                "cada escena del río pide el mismo río que la orilla");
            Assert.That(escenas.Select(escena => escena.AmbientLayer), Has.All.SameAs(sonidos.ForestAmbient),
                "y el mismo bosque en la segunda capa");
        }

        /// <summary>
        /// Lo que pidió Santiago el 25/09/2026, pieza por pieza: sin esto, reasignar un campo del
        /// asset a otra toma deja pasar las pruebas de la escena, que comparan contra el propio
        /// asset. La balsa que cruza es contenido de la 3.3, no de <c>N3_Sonidos</c>.
        /// </summary>
        [Test]
        public void RiverSounds_RF44_CadaMomentoDelNivelSuenaConSuPieza()
        {
            var sonidos = Asset<RiverSounds>("N3_Sonidos");
            var balsa = Asset<NarrativeSequence>("N3_Escena33_Cruce").Props.Single(prop => prop.Motion == PropMotion.Drift);

            Assert.That(sonidos.Collected.name, Is.EqualTo("sfx_encaje_pieza"), "recoger un material");
            Assert.That(sonidos.PiecePlaced.name, Is.EqualTo("sfx_martillo_madera"), "poner una pieza en la balsa");
            Assert.That(sonidos.PhaseHammer.name, Is.EqualTo("sfx_martillo_madera"), "aprobar una fase");
            Assert.That(sonidos.PhaseHits, Is.EqualTo(3), "con tres golpes");
            Assert.That(sonidos.RaftBuilt.name, Is.EqualTo("sfx_n1_pieza_tomar"), "la balsa terminada");
            Assert.That(balsa.MotionAmbient, Is.Not.Null, "la balsa suena al cruzar");
            Assert.That(balsa.MotionAmbient.name, Is.EqualTo("amb_balsa_movimiento"), "con el movimiento de la balsa");
        }
    }
}
