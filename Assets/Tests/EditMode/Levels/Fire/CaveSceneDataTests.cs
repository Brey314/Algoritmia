using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor.SceneManagement;

namespace Game.Levels.Fire.Tests
{
    /// <summary>
    /// Lo que la escena de la cueva guarda en disco y no se ve hasta que el fuego prende: la llama
    /// cenital lleva su halo de luz (decisión de Santiago, 08/10/2026).
    /// </summary>
    public class CaveSceneDataTests
    {
        private const string ScenePath = "Assets/Game/Scenes/Level1_Cave.unity";

        /// <summary>
        /// La llama cenital lleva <see cref="FireGlow"/> y su halo va al fondo de «Suelo»
        /// (<see cref="FireGlow.BehindSiblings"/>): baña el suelo de luz de fuego, pero no vela el
        /// montón de hojas ni las piedras que acaban de prender —con el halo justo antes de la llama
        /// quedaba una película naranja sobre ellas— y la llama sigue siendo el último hermano
        /// (<c>FireLevel_RF20_AlSoplarPrendeLaLlamaCenitalSobreElMonton</c>).
        /// </summary>
        [Test]
        public void FireLevel_RF20_LaLlamaCenitalLlevaElHaloQueBanaElSueloYNoVelaElMonton()
        {
            var escena = EditorSceneManager.OpenPreviewScene(ScenePath);
            try
            {
                var controlador = escena.GetRootGameObjects()
                    .Select(raiz => raiz.GetComponentInChildren<FirePanelController>(true))
                    .Single(candidato => candidato != null);
                var llama = controlador.FireFlame;
                var halo = llama.GetComponent<FireGlow>();

                Assert.That(halo, Is.Not.Null, "la llama cenital lleva FireGlow");
                Assert.That(halo.BehindSiblings, Is.True, "con el halo al fondo de su contenedor");
                Assert.That(llama.gameObject.activeSelf, Is.False, "la llama sigue apagada hasta soplar: el halo nace con ella");
                Assert.That(llama.transform.GetSiblingIndex(), Is.EqualTo(llama.transform.parent.childCount - 1),
                    "y en disco ya es el último hermano");
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(escena);
            }
        }
    }
}
