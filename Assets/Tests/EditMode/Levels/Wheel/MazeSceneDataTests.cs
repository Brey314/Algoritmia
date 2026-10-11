using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.Wheel.Tests
{
    /// <summary>
    /// Lo que la escena del laberinto guarda en disco y no se ve corriéndola: datos de pantalla
    /// que tienen que coincidir entre sí aunque vivan en dos sitios (RF-30, RNF-20).
    /// </summary>
    public class MazeSceneDataTests
    {
        private const string ScenePath = "Assets/Game/Scenes/Level2_Maze.unity";

        /// <summary>
        /// Abre el laberinto como escena de vista previa —sin tocar las escenas abiertas ni correr
        /// Awake ni Start— y la cierra siempre: lo que se mira es lo que está guardado en disco.
        /// </summary>
        private static void ConElLaberinto(Action<MazeSceneController, Transform> mirar)
        {
            var escena = EditorSceneManager.OpenPreviewScene(ScenePath);
            try
            {
                var raices = escena.GetRootGameObjects();
                mirar(
                    raices.Select(raiz => raiz.GetComponentInChildren<MazeSceneController>(true)).Single(controlador => controlador != null),
                    raices.Single(raiz => raiz.name == "Canvas").transform);
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(escena);
            }
        }

        /// <summary>
        /// El fondo de la pantalla es uno solo, el ámbar de atención #E8A33D (decisión de Santiago,
        /// 08/10/2026), y vive en dos sitios: <c>Fondo_Escena</c>, que va detrás de la tarjeta de la
        /// secuencia, y el <c>BackdropColor</c> del asset, con el que <c>FitEnvironment</c> pinta el
        /// panel del entorno. Si se separan, la pantalla se parte en dos colores y la costura sale
        /// justo entre el entorno y la tarjeta.
        /// </summary>
        [Test]
        public void MazeScene_RF30_ElFondoDeLaEscenaYElDelAssetSonElMismoAmbar()
        {
            ConElLaberinto((controlador, canvas) =>
            {
                var fondoEscena = ColorUtility.ToHtmlStringRGBA(canvas.Find("Fondo_Escena").GetComponent<Image>().color);
                var delAsset = ColorUtility.ToHtmlStringRGBA(controlador.Layout.BackdropColor);

                Assert.That(fondoEscena, Is.EqualTo(delAsset), "Fondo_Escena y MazeLayout.BackdropColor son el mismo color");
                Assert.That(delAsset, Is.EqualTo("C4A882FF"), "ámbar de atención #C4A882");
            });
        }

        /// <summary>
        /// El aro del botón de pista se distingue del fondo ámbar: 3:1, el mínimo de contraste de un
        /// componente que no es texto (WCAG 1.4.11; el 4,5:1 de RNF-20 es para texto). Con el aro de
        /// fuego #E2571F que llevan los otros niveles eran 1,73:1 y el botón se perdía ámbar sobre ámbar.
        /// </summary>
        [Test]
        public void MazeScene_RNF20_ElAroDelBotonDePistaSeDistingueDelFondoAmbar()
        {
            ConElLaberinto((controlador, _) =>
            {
                var aro = controlador.HelpButton.GetComponent<Image>().color;
                var fondo = controlador.Layout.BackdropColor;
                var contraste = ContrastRatio(aro, fondo);

                Assert.That(contraste, Is.GreaterThanOrEqualTo(3.0),
                    $"aro #{ColorUtility.ToHtmlStringRGB(aro)} sobre fondo #{ColorUtility.ToHtmlStringRGB(fondo)}: {contraste:F2}:1");
            });
        }

        private static double ContrastRatio(Color a, Color b)
        {
            var lighter = Math.Max(RelativeLuminance(a), RelativeLuminance(b));
            var darker = Math.Min(RelativeLuminance(a), RelativeLuminance(b));
            return (lighter + 0.05) / (darker + 0.05);
        }

        private static double RelativeLuminance(Color color) =>
            0.2126 * Linear(color.r) + 0.7152 * Linear(color.g) + 0.0722 * Linear(color.b);

        private static double Linear(float channel) =>
            channel <= 0.03928 ? channel / 12.92 : Math.Pow((channel + 0.055) / 1.055, 2.4);
    }
}
