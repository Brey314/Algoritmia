using System.Linq;
using Game.Core;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Tests
{
    /// <summary>
    /// Lo que el prefab <c>MenuPausa</c> guarda en disco y no se ve hasta abrir la pausa (INC-159):
    /// la tabla de «Volver a ver la historia» cruzada con las escenas reales, los cinco botones
    /// con su orden y su cableado, y que el panel quepa en la pantalla.
    /// </summary>
    public class PauseMenuControllerTests
    {
        private const string PrefabPath = "Assets/Game/Prefabs/UI/MenuPausa.prefab";
        private const string ContentPath = "Assets/Game/Data/PauseMenuContent.asset";

        private static PauseMenuController LoadController() =>
            AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath).GetComponent<PauseMenuController>();

        private static string LabelOf(Button button) => button.GetComponentInChildren<Text>(true).text;

        /// <summary>
        /// Cada fila dice qué escena presenta una fase; la escena lo declara por su lado con
        /// <see cref="NarrativeSequence.Level"/> y <see cref="NarrativeSequence.NextPhase"/>, que es
        /// por donde <c>NarrativeSceneController.Leave</c> la devuelve a jugar. Si no coinciden,
        /// «Volver a ver la historia» dejaría al estudiante en otra fase al terminar.
        /// </summary>
        [Test]
        public void PauseMenu_INC159_CadaFilaDeLaTablaApuntaALaEscenaQueArrancaEsaFase()
        {
            var sequences = AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .ToDictionary(sequence => sequence.Id);
            var rows = LoadController().PhaseNarratives;

            Assert.That(rows, Is.Not.Empty);
            foreach (var row in rows)
            {
                Assert.That(sequences.TryGetValue(row.SequenceId, out var sequence), Is.True,
                    $"no existe ninguna NarrativeSequence con Id «{row.SequenceId}»");
                Assert.That(sequence.Level, Is.EqualTo(row.Level), $"{row.SequenceId}: nivel de la escena");
                Assert.That(sequence.NextPhase, Is.EqualTo(row.Phase),
                    $"{row.SequenceId}: la escena tiene que terminar en la fase {row.Phase} de {row.Level}");
            }
        }

        [Test]
        public void PauseMenu_INC159_TodaFaseTieneSuHistoriaSalvoElAmarreDelNivel3()
        {
            var covered = LoadController().PhaseNarratives.Select(row => (row.Level, row.Phase)).ToHashSet();
            var every = LevelUnlockPolicy.AllLevels.SelectMany(level =>
                Enumerable.Range(1, PhaseId.PhaseCountOf(level)).Select(phase => (level, phase)));

            // El amarre no arranca con una escena propia: ahí el botón se oculta.
            Assert.That(every.Where(phase => !covered.Contains(phase)), Is.EqualTo(new[] { (LevelId.River, 2) }));
        }

        [Test]
        public void PauseMenu_INC159_ElPanelBaseTieneCincoBotonesEnOrdenYCableados()
        {
            var controller = LoadController();

            var names = controller.BaseButtons.GetComponentsInChildren<Button>(true).Select(button => button.name);
            Assert.That(names, Is.EqualTo(new[]
            {
                "BotonContinuar", "BotonReiniciar", "BotonHistoria", "BotonVolverMenu", "BotonGuardarSalir"
            }));
            Assert.That(controller.ContinueButton.name, Is.EqualTo("BotonContinuar"));
            Assert.That(controller.RestartButton.name, Is.EqualTo("BotonReiniciar"));
            Assert.That(controller.HistoryButton.name, Is.EqualTo("BotonHistoria"));
            Assert.That(controller.LevelSelectButton.name, Is.EqualTo("BotonVolverMenu"));
            Assert.That(controller.SaveQuitButton.name, Is.EqualTo("BotonGuardarSalir"));

            Assert.That(controller.QuitConfirmPanel.name, Is.EqualTo("PanelConfirmarSalida"));
            Assert.That(controller.QuitConfirmPanel.activeSelf, Is.False, "la confirmación nace cerrada");
            Assert.That(controller.QuitConfirmPanel.transform.parent, Is.SameAs(controller.BaseButtons.transform.parent),
                "hermana del panel base, como la de «Reiniciar»");
            Assert.That(controller.ConfirmQuitButton.name, Is.EqualTo("BotonConfirmarSalida"));
            Assert.That(controller.CancelQuitButton.name, Is.EqualTo("BotonCancelarSalida"));
            Assert.That(controller.ConfirmQuitButton.transform.IsChildOf(controller.QuitConfirmPanel.transform), Is.True);
            Assert.That(controller.CancelQuitButton.transform.IsChildOf(controller.QuitConfirmPanel.transform), Is.True);
            Assert.That(controller.QuitPromptLabel.transform.IsChildOf(controller.QuitConfirmPanel.transform), Is.True);
        }

        [Test]
        public void PauseMenu_INC159_LosTextosNuevosSalenDelAssetYElPrefabDiceLoMismo()
        {
            var controller = LoadController();
            var content = AssetDatabase.LoadAssetAtPath<PauseMenuContent>(ContentPath);

            Assert.That(controller.Content, Is.SameAs(content), "el prefab referencia el asset de textos (CT-05)");
            Assert.That(content.HistoryLabel, Is.EqualTo("Volver a ver la historia"));
            Assert.That(content.SaveQuitLabel, Is.EqualTo("Guardar y salir"));
            Assert.That(content.QuitPrompt, Is.EqualTo("¿Quieres salir del juego? Lo que ya terminaste queda guardado."));
            Assert.That(content.ConfirmQuitLabel, Is.EqualTo("Sí, salir"));
            Assert.That(content.CancelQuitLabel, Is.EqualTo("Cancelar"));

            // El asset manda en el juego; lo que dice el prefab es solo lo que se ve al abrirlo en el Editor.
            Assert.That(LabelOf(controller.HistoryButton), Is.EqualTo(content.HistoryLabel));
            Assert.That(LabelOf(controller.SaveQuitButton), Is.EqualTo(content.SaveQuitLabel));
            Assert.That(controller.QuitPromptLabel.text, Is.EqualTo(content.QuitPrompt));
            Assert.That(LabelOf(controller.ConfirmQuitButton), Is.EqualTo(content.ConfirmQuitLabel));
            Assert.That(LabelOf(controller.CancelQuitButton), Is.EqualTo(content.CancelQuitLabel));
        }

        /// <summary>
        /// A 1920 × 1080 (la referencia del CanvasScaler) el panel con el título y los cinco botones
        /// no se sale de la pantalla: el alto que el layout le da es la suma de sus hijos.
        /// </summary>
        [Test]
        public void PauseMenu_INC159_ElPanelConCincoBotonesYElTituloCabeEnLos1080DeAlto()
        {
            var panel = (RectTransform)LoadController().BaseButtons.transform;
            var layout = panel.GetComponent<VerticalLayoutGroup>();
            var inFlow = Enumerable.Range(0, panel.childCount)
                .Select(index => (RectTransform)panel.GetChild(index))
                .Where(child => child.GetComponent<LayoutElement>() is not { ignoreLayout: true })
                .ToArray();

            var height = layout.padding.vertical + inFlow.Sum(child => child.sizeDelta.y)
                         + layout.spacing * (inFlow.Length - 1);

            Assert.That(inFlow, Has.Length.EqualTo(6), "el título y los cinco botones");
            Assert.That(height, Is.LessThanOrEqualTo(1080f), "alto del panel de pausa");
        }
    }
}
