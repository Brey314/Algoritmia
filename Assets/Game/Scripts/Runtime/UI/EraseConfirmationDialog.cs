using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Confirmación explícita del borrado irreversible de un perfil (RF-47, RNF-11, CU-12).
    /// No implementa el borrado: lo pide a <see cref="ProfileSession.Delete"/>, el mismo que ya
    /// usa <see cref="ProfileSelectController"/> desde el Slice 1 — no hay dos caminos para borrar
    /// un perfil, solo dos pantallas que piden confirmación antes de pedirlo.
    /// </summary>
    public class EraseConfirmationDialog : MonoBehaviour
    {
        [SerializeField] private Text prompt;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button confirmButton;

        internal GameFlowRunner Runner { get; set; }

        /// <summary>
        /// La sesión que borra. Producción: la de <see cref="Runner"/>; una prueba la sustituye
        /// por una contra un directorio temporal, igual que <see cref="ProfileSelectController"/>.
        /// </summary>
        internal ProfileSession Session { get; set; }

        private string _profileName;
        private Action<bool> _onClosed;

#if UNITY_INCLUDE_TESTS
        internal Text Prompt => prompt;
        internal Button CancelButton => cancelButton;
        internal Button ConfirmButton => confirmButton;
#endif

        private void Awake()
        {
            Runner ??= GameFlowRunner.Instance;
            Session ??= Runner != null ? Runner.Session : null;
        }

        private void Start()
        {
            cancelButton.onClick.AddListener(Cancel);
            confirmButton.onClick.AddListener(Confirm);
        }

        /// <summary>Abre el diálogo para el perfil dado. <paramref name="onClosed"/> recibe si se borró.</summary>
        internal void Ask(string profileName, Action<bool> onClosed)
        {
            _profileName = profileName;
            _onClosed = onClosed;
            prompt.text = $"¿Eliminas definitivamente los datos de {profileName}? Esta acción no se puede deshacer.";
            gameObject.SetActive(true);
        }

        private void Cancel()
        {
            // CU-12 FA-4a: cancelar no toca disco. No se llama a ningún borrado aquí, ni parcial.
            gameObject.SetActive(false);
            _onClosed?.Invoke(false);
        }

        private void Confirm()
        {
            if (!ScreenFlow.Ready(Session, this))
            {
                Cancel();
                return;
            }

            var deleted = Session.Delete(_profileName);
            gameObject.SetActive(false);
            _onClosed?.Invoke(deleted);
        }
    }
}
