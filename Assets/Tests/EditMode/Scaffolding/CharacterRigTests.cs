using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding.Tests
{
    public class CharacterRigTests
    {
        private const string Carpeta = "Assets/Game/Prefabs/Characters/";

        private static readonly string[] Familia = { "Papa", "Mama", "Nina", "Nino" };
        private static readonly string[] Guia = { "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota" };

        /// <summary>Lo que el guion y §13.3 piden a la familia: todo salvo el trompo del guía.</summary>
        private static readonly ActorAction[] AccionesDeLaFamilia =
            ((ActorAction[])Enum.GetValues(typeof(ActorAction))).Where(accion => accion != ActorAction.Spin).ToArray();

        private static readonly ActorAction[] AccionesDelGuia =
        {
            ActorAction.Idle, ActorAction.Hidden, ActorAction.Talk, ActorAction.Point, ActorAction.Spin,
            ActorAction.Celebrate, ActorAction.Encourage, ActorAction.Appear, ActorAction.Vanish
        };

        [Test]
        public void CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var estados = Estados(Rig(nombre));
            var esperadas = Familia.Contains(nombre) ? AccionesDeLaFamilia : AccionesDelGuia;

            Assert.That(esperadas.Select(accion => accion.ToString()).Except(estados), Is.Empty,
                $"{nombre}: cada acción que puede pedir una escena es un estado de su Animator");
        }

        /// <summary>
        /// Ninguna parte de un personaje recibe clics: pintado encima de una pieza, le robaría el
        /// clic sostenido (RNF-02) y rompería las listas cerradas de cada mecánica.
        /// </summary>
        [Test]
        public void CharacterRig_RNF02_NingunaParteDeUnPersonajeRecibeClics(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);

            Assert.That(rig.GetComponentsInChildren<Graphic>(true).Where(g => g.raycastTarget).Select(g => g.name), Is.Empty);
            Assert.That(rig.GetComponentsInChildren<CanvasGroup>(true).All(grupo => !grupo.blocksRaycasts), Is.True);
        }

        /// <summary>
        /// No existen animaciones de derrota, caída ni salto (CP-02, CT-06, §13.3): tras un
        /// intento sin éxito el personaje da ánimo.
        /// </summary>
        [Test]
        public void CharacterRig_CP02_NingunClipEsDeDerrotaCaidaNiSalto()
        {
            var vetados = new[] { "derrota", "gameover", "caer", "caida", "saltar", "salto", "triste" };
            var clips = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/Game/Art/Characters" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(ruta => vetados.Any(palabra => ruta.ToLowerInvariant().Contains(palabra)))
                .ToArray();

            Assert.That(clips, Is.Empty);
        }

        [Test]
        public void CharacterRig_RF05_CadaPersonajeTieneRetratoYNombresConQueHabla(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego", "Algoritm_Rueda", "Algoritm_Gota")] string nombre)
        {
            var rig = Rig(nombre);

            Assert.That(rig.Portrait, Is.Not.Null, "su retrato para el cuadro de diálogo");
            Assert.That(rig.SpeakerNames, Is.Not.Empty, "el nombre con que el guion lo hace hablar");
            Assert.That(rig.Stage, Is.Not.Null, "el lienzo que escala para llenar su casilla");
        }

        /// <summary>
        /// Los dos niños hablan a la vez en la 1.3 («NIÑOS»): los dos gesticulan y el retrato sale
        /// de uno de ellos.
        /// </summary>
        [Test]
        public void CharacterRig_RF05_LosDosNinosHablanComoNinos()
        {
            Assert.That(Rig("Nina").Speaks("NIÑOS"), Is.True);
            Assert.That(Rig("Nino").Speaks("niños"), Is.True, "sin distinguir mayúsculas");
            Assert.That(Rig("Papa").Speaks("NIÑOS"), Is.False);
            Assert.That(Rig("Papa").Speaks("PAPÁ"), Is.True);
        }

        private static CharacterRig Rig(string nombre)
        {
            var rig = AssetDatabase.LoadAssetAtPath<CharacterRig>($"{Carpeta}{nombre}.prefab");
            Assert.That(rig, Is.Not.Null, $"existe el prefab {nombre}");
            return rig;
        }

        private static IEnumerable<string> Estados(CharacterRig rig)
        {
            var controller = rig.GetComponent<Animator>().runtimeAnimatorController as AnimatorController;
            Assert.That(controller, Is.Not.Null, $"{rig.name} tiene su controlador");
            return controller.layers[0].stateMachine.states.Select(estado => estado.state.name);
        }
    }
}
