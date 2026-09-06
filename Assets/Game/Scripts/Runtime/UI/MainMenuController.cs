using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Pantalla de inicio (RF-01): el título del producto y las opciones Jugar, Créditos y Salir.
    /// Adaptador delgado — traduce cada clic a una transición de <see cref="GameFlowRunner"/> y, al
    /// pulsar «Salir», guarda el perfil activo antes de cerrar la aplicación (RF-09).
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameTitleConfig titleConfig;
        [SerializeField] private Text titleLabel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitButton;

        [Header("Paneles de la pantalla de inicio")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject profilePanel;

        /// <summary>
        /// Guarda el perfil activo al salir. Producción: la sesión de <see cref="GameFlowRunner"/>,
        /// que se resuelve solo cuando se pulsa «Salir» — así una prueba que no lo pulsa no acaba
        /// construyendo el almacén en disco.
        /// </summary>
        internal IProfileSaver Saver { get; set; }

        /// <summary>Cierra la aplicación. Producción: <see cref="Application.Quit()"/>.</summary>
        internal Action Quit { get; set; }

#if UNITY_INCLUDE_TESTS
        internal GameTitleConfig TitleConfig => titleConfig;
        internal Text TitleLabel => titleLabel;
#endif

        private void Awake() => Quit ??= Application.Quit;

        private void Start()
        {
            if (titleConfig != null && titleLabel != null)
            {
                titleLabel.text = titleConfig.Title;
            }

            playButton.onClick.AddListener(OpenProfilePanel);
            creditsButton.onClick.AddListener(() => GameFlowRunner.Instance.GoTo(GameState.Credits));
            exitButton.onClick.AddListener(Exit);
        }

        private void OpenProfilePanel()
        {
            // ProfileSelect es un panel de esta misma escena (SPEC §Estructura): el cambio de
            // estado no recarga MainMenu, solo intercambia paneles.
            GameFlowRunner.Instance.GoTo(GameState.ProfileSelect);
            mainPanel.SetActive(false);
            profilePanel.SetActive(true);
        }

        private void Exit()
        {
            // RF-09: el orden importa — primero se persiste el progreso del perfil activo, y solo
            // después se cierra. Al revés se perdería lo que el estudiante acababa de lograr.
            (Saver ?? (GameFlowRunner.Instance != null ? GameFlowRunner.Instance.Session : null))
                ?.SaveActive();
            Quit();
        }
    }
}
