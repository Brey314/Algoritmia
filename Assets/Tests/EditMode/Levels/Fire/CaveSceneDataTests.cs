using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

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

        /// <summary>
        /// El aviso del botón de ayuda (INC-158) vive apagado en la escena, enchufado al controlador,
        /// como último hijo del Canvas —por encima de la capa de oscuridad de la cueva, del panel y
        /// del botón de pausa, que así queda bajo el fondo sin poder pulsarse— y con un fondo a
        /// pantalla completa que bloquea los clics hacia lo de abajo.
        /// </summary>
        [Test]
        public void FireLevel_INC158_ElAvisoDeLaAyudaVaApagadoPorEncimaDeTodoYBloqueaLosClics()
        {
            var escena = EditorSceneManager.OpenPreviewScene(ScenePath);
            try
            {
                var controlador = ControladorDe(escena);
                var aviso = controlador.HelpIntro;

                Assert.That(aviso, Is.Not.Null, "FirePanelController tiene el aviso enchufado");
                Assert.That(aviso.gameObject.activeSelf, Is.False, "el aviso nace apagado: lo abre el controlador");
                var lienzo = aviso.transform.parent;
                Assert.That(lienzo.GetComponent<Canvas>(), Is.Not.Null, "cuelga directo del Canvas");
                Assert.That(aviso.transform.GetSiblingIndex(), Is.EqualTo(lienzo.childCount - 1),
                    "último hijo del Canvas: dibuja por encima de la oscuridad, del panel y de la pausa");
                Assert.That(aviso.HelpButton, Is.SameAs(controlador.HintButton.transform),
                    "el botón que se ilumina es «Pista», el círculo de Algoritm");

                var fondo = aviso.GetComponent<Image>();
                var rect = (RectTransform)aviso.transform;
                Assert.That(fondo.raycastTarget, Is.True, "el fondo bloquea los clics");
                Assert.That(fondo.color.a, Is.EqualTo(0.72f).Within(0.001f), "el mismo 0,72 del panel de pausa");
                Assert.That(rect.anchorMin, Is.EqualTo(Vector2.zero));
                Assert.That(rect.anchorMax, Is.EqualTo(Vector2.one), "a pantalla completa");
                Assert.That(aviso.CloseButton, Is.Not.Null);
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(escena);
            }
        }

        /// <summary>
        /// «Al inicio del Nivel 1 está muy oscuro» (sesión con estudiantes del 13/10/2026, INC-152):
        /// la luz del «fondo» va de 0,22 a 0,35 y no de 0,10 a 0,30, en la escena y no solo en el
        /// valor por defecto del componente.
        /// </summary>
        [Test]
        public void CaveLighting_INC152_LaCuevaArrancaMasClaraQueEnElDisenoOriginal()
        {
            var escena = EditorSceneManager.OpenPreviewScene(ScenePath);
            try
            {
                var luz = ControladorDe(escena).Lighting;

                Assert.That(luz.Darkest.Ambient, Is.EqualTo(0.22f).Within(0.0005f), "luz sin ningún golpe efectivo");
                Assert.That(luz.Brightest.Ambient, Is.EqualTo(0.35f).Within(0.0005f), "luz al converger");
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(escena);
            }
        }

        private static FirePanelController ControladorDe(UnityEngine.SceneManagement.Scene escena) =>
            escena.GetRootGameObjects()
                .Select(raiz => raiz.GetComponentInChildren<FirePanelController>(true))
                .Single(candidato => candidato != null);
    }
}
