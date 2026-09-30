using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Pantalla de inicio (RF-01): el título del producto y las opciones Jugar, Créditos,
    /// Progreso del equipo y Salir. Adaptador delgado — traduce cada clic a una transición de
    /// <see cref="GameFlowRunner"/> y, al confirmar la salida, guarda el perfil activo antes de
    /// cerrar la aplicación (RF-09, HU-18).
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameTitleConfig titleConfig;
        [SerializeField] private Text titleLabel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button teacherReportButton;
        [SerializeField] private Button exitButton;

        [Header("Paneles de la pantalla de inicio")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject profilePanel;

        [Header("Confirmación de salida (HU-18 pasos 3-4, FA-01, FA-04)")]
        [SerializeField] private GameObject exitConfirmPanel;
        [SerializeField] private Button exitCancelButton;
        [SerializeField] private Button exitConfirmButton;

        /// <summary>
        /// Vacía y oculta salvo que el guardado esté cayendo a la ruta de respaldo: solo entonces
        /// informa dónde quedó el progreso (HU-18 FA-04). El texto sale de
        /// <see cref="GameTitleConfig.FallbackSaveNotice"/> (CT-05): nada visible va escrito aquí.
        /// </summary>
        [SerializeField] private Text fallbackNoticeLabel;

        /// <summary>
        /// Guarda el perfil activo al salir. Producción: la sesión de <see cref="GameFlowRunner"/>,
        /// que se resuelve solo al confirmar la salida — así una prueba que no llega a confirmarla
        /// no acaba construyendo el almacén en disco.
        /// </summary>
        internal IProfileSaver Saver { get; set; }

        /// <summary>Cierra la aplicación. Producción: <see cref="Application.Quit()"/>.</summary>
        internal Action Quit { get; set; }

        internal GameFlowRunner Runner { get; set; }

        /// <summary>
        /// Punto de sustitución de HU-18 FA-04: la carpeta real de guardado si el guardado cayó a
        /// la ruta de respaldo, o <c>null</c> con Datos/ escribible. Se evalúa solo al abrir la
        /// confirmación de salida, como <see cref="Saver"/>, para que una prueba que no llega a
        /// abrirla no acabe construyendo el almacén en disco.
        /// </summary>
        internal Func<string> FallbackDirectory { get; set; }

#if UNITY_INCLUDE_TESTS
        internal GameTitleConfig TitleConfig => titleConfig;
        internal Text TitleLabel => titleLabel;
        internal GameObject MainPanel => mainPanel;
        internal GameObject ExitConfirmPanel => exitConfirmPanel;
        internal Text FallbackNoticeLabel => fallbackNoticeLabel;
#endif

        private void Awake()
        {
            Runner ??= GameFlowRunner.Instance;
            Quit ??= Application.Quit;
            FallbackDirectory ??= () =>
                Runner != null && Runner.Session.UsingFallback ? Runner.Session.SaveDirectory : null;
        }

        private void Start()
        {
            if (titleConfig != null && titleLabel != null)
            {
                titleLabel.text = titleConfig.Title;
            }

            playButton.onClick.AddListener(OpenProfilePanel);
            creditsButton.onClick.AddListener(OpenCredits);
            teacherReportButton.onClick.AddListener(OpenTeacherReport);
            exitButton.onClick.AddListener(OpenExitConfirm);
            exitCancelButton.onClick.AddListener(CloseExitConfirm);
            exitConfirmButton.onClick.AddListener(Exit);
            exitConfirmPanel.SetActive(false);
        }

        private void OpenProfilePanel()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            // ProfileSelect es un panel de esta misma escena (SPEC §Estructura): el cambio de
            // estado no recarga MainMenu, solo intercambia paneles.
            Runner.GoTo(GameState.ProfileSelect);
            mainPanel.SetActive(false);
            profilePanel.SetActive(true);
        }

        private void OpenCredits()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            Runner.GoTo(GameState.Credits);
        }

        private void OpenTeacherReport()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            // RF-46: el informe se alcanza desde el menú principal, nunca desde dentro de una
            // partida — no hay ruta a este botón fuera de esta pantalla.
            Runner.GoTo(GameState.TeacherReport);
        }

        /// <summary>
        /// «Salir» (HU-18 pasos 3-4): pide confirmación e informa que lo logrado ya está guardado
        /// — no es una defensa contra pérdida de datos, porque el avance se guardó al confirmar
        /// cada fase (RF-41); es la confirmación que pide la historia (FA-01, vale también sin
        /// perfil activo, FA-03). De paso, si el guardado está cayendo a la ruta de respaldo lo
        /// dice y muestra dónde (FA-04, RNF-07): una ruta no es una cifra de desempeño (CP-03).
        /// </summary>
        private void OpenExitConfirm()
        {
            var fallbackDirectory = FallbackDirectory?.Invoke();
            if (fallbackNoticeLabel != null)
            {
                if (fallbackDirectory != null && titleConfig != null)
                {
                    fallbackNoticeLabel.text =
                        titleConfig.FallbackSaveNotice + "\n" + fallbackDirectory.Replace('/', '\\');
                    fallbackNoticeLabel.gameObject.SetActive(true);
                }
                else
                {
                    fallbackNoticeLabel.gameObject.SetActive(false);
                }
            }

            exitConfirmPanel.SetActive(true);
        }

        /// <summary>«Quedarme» (HU-18 FA-01): vuelve al menú sin guardar ni cerrar nada.</summary>
        private void CloseExitConfirm() => exitConfirmPanel.SetActive(false);

        private void Exit()
        {
            // RF-09: el orden importa — primero se persiste el progreso del perfil activo, y solo
            // después se cierra. Al revés se perdería lo que el estudiante acababa de lograr.
            // Sin flujo no hay perfil activo que perder, pero salir tiene que seguir funcionando:
            // este es el único botón que no necesita navegar.
            (Saver ?? (Runner != null ? Runner.Session : null))?.SaveActive();
            Quit();
        }
    }
}
