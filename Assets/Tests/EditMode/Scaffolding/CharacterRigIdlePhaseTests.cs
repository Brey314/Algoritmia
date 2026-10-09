using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// La fase del reposo (D9, 08/10/2026): la portada del menú pone a los cinco personajes en Idle y, con el
    /// mismo arranque, respirarían al unísono. El campo <c>idlePhase</c> los desfasa dentro de su ciclo.
    /// </summary>
    /// <remarks>
    /// Se prueba sobre una copia del prefab, nunca sobre el asset, y con el Animator real: <c>Play</c> hace
    /// <c>animator.Play</c> y <c>Update(0)</c>, así que el estado ya está puesto al volver.
    /// </remarks>
    public class CharacterRigIdlePhaseTests
    {
        private const string Carpeta = "Assets/Game/Prefabs/Characters/";

        [Test]
        public void CharacterRig_RF01_ElReposoArrancaEnLaFaseQueSeLePide(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego")] string nombre,
            [Values(0f, 0.35f, 0.7f)] float fase)
        {
            var copia = UnaCopia(nombre, fase);
            try
            {
                var rig = copia.GetComponent<CharacterRig>();

                rig.Play(ActorAction.Idle);

                var estado = copia.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
                Assert.That(estado.IsName(nameof(ActorAction.Idle)), Is.True, $"{nombre}: está en el estado de reposo");
                Assert.That(estado.normalizedTime, Is.EqualTo(fase).Within(0.01f), $"{nombre}: arranca a {fase} de su ciclo");
            }
            finally
            {
                Object.DestroyImmediate(copia);
            }
        }

        // La fase es solo del reposo: un gesto (hablar, señalar…) que arrancara a mitad se vería cortado.
        [Test]
        public void CharacterRig_RF01_UnGestoQueNoEsElReposoArrancaEnSuPrimerCuadroAunqueHayaFase(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego")] string nombre)
        {
            var copia = UnaCopia(nombre, 0.6f);
            try
            {
                var rig = copia.GetComponent<CharacterRig>();

                rig.Play(ActorAction.Talk);

                var estado = copia.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
                Assert.That(estado.IsName(nameof(ActorAction.Talk)), Is.True, $"{nombre}: está hablando");
                Assert.That(estado.normalizedTime, Is.EqualTo(0f).Within(0.01f), $"{nombre}: el gesto empieza en su primer cuadro");
            }
            finally
            {
                Object.DestroyImmediate(copia);
            }
        }

        [Test]
        public void CharacterRig_RF01_SinFaseElReposoArrancaEnSuPrimerCuadroComoSiempre(
            [Values("Papa", "Mama", "Nina", "Nino", "Algoritm_Fuego")] string nombre)
        {
            var copia = UnaCopia(nombre, 0f);
            try
            {
                var rig = copia.GetComponent<CharacterRig>();

                rig.Play(ActorAction.Idle);

                Assert.That(rig.IdlePhase, Is.EqualTo(0f), $"{nombre}: el prefab no trae fase");
                Assert.That(copia.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime, Is.EqualTo(0f).Within(0.01f));
            }
            finally
            {
                Object.DestroyImmediate(copia);
            }
        }

        private static GameObject UnaCopia(string nombre, float fase)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<CharacterRig>($"{Carpeta}{nombre}.prefab");
            Assert.That(prefab, Is.Not.Null, $"existe el prefab {nombre}");

            var copia = Object.Instantiate(prefab.gameObject);
            var serializado = new SerializedObject(copia.GetComponent<CharacterRig>());
            serializado.FindProperty("idlePhase").floatValue = fase;
            serializado.ApplyModifiedProperties();
            return copia;
        }
    }
}
