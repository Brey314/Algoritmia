using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>
    /// El aviso que explica para qué sirve el botón de ayuda, la primera vez que se juega una
    /// mecánica (INC-158: «no se entiende para qué sirve el círculo con Algoritm»). Oscurece la
    /// pantalla, deja el botón iluminado encima, y lo explica en una tarjeta con un solo botón,
    /// «Cerrar».
    /// </summary>
    /// <remarks>
    /// Adaptador delgado: no decide cuándo se muestra (eso lo pregunta el nivel a su progreso) ni
    /// de qué dice nada (el texto llega de un asset del nivel, CT-05). Es la raíz del aviso:
    /// debe vivir apagada en la escena, dibujada después de todo lo demás —también del botón de
    /// pausa, que así queda bajo el fondo sin poder pulsarse— y con un fondo a pantalla completa
    /// que bloquea los clics hacia lo de abajo. Mientras está abierto el tiempo está detenido,
    /// igual que en la pausa, porque no hay nada que resolver mientras se lee.
    /// </remarks>
    public class HelpButtonIntro : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("El botón de ayuda («Pista», con Algoritm). Mientras el aviso está abierto se dibuja por encima del fondo oscuro.")]
        private RectTransform helpButton;

        [SerializeField]
        [Tooltip("El texto de la tarjeta.")]
        private Text messageLabel;

        [SerializeField]
        [Tooltip("El botón que cierra el aviso.")]
        private Button closeButton;

        [SerializeField]
        [Tooltip("El rótulo del botón de cierre.")]
        private Text closeLabel;

        [SerializeField]
        [Tooltip("Orden de dibujo del botón de ayuda mientras está iluminado. Tiene que ser mayor que el del lienzo de la escena (0).")]
        private int litSortingOrder = 10;

        private Canvas _litCanvas;
        private GraphicRaycaster _litRaycaster;
        private float _timeScaleBefore = 1f;
        private Action _onClosed;

        /// <summary>El aviso está a la vista.</summary>
        public bool IsOpen { get; private set; }

#if UNITY_INCLUDE_TESTS
        internal RectTransform HelpButton => helpButton;
        internal Button CloseButton => closeButton;
        internal Canvas LitCanvas => _litCanvas;
#endif

        private void Awake() => closeButton.onClick.AddListener(Close);

        /// <summary>
        /// Abre el aviso. <paramref name="onClosed"/> avisa al nivel de que ya terminó la lectura
        /// (el nivel descuenta ese rato del tiempo de resolución, como el de la pausa, RF-07).
        /// </summary>
        public void Show(string message, string closeText, Action onClosed = null)
        {
            if (IsOpen)
            {
                return;
            }

            IsOpen = true;
            _onClosed = onClosed;
            messageLabel.text = message;
            closeLabel.text = closeText;
            _timeScaleBefore = Time.timeScale;
            Time.timeScale = 0f;
            gameObject.SetActive(true);

            // Un Canvas anidado con orden propio saca al botón de debajo del fondo sin cambiarle el
            // sitio en la jerarquía; sin su GraphicRaycaster, un Canvas anidado no recibiría clics.
            _litCanvas = helpButton.gameObject.AddComponent<Canvas>();
            _litCanvas.overrideSorting = true;
            _litCanvas.sortingOrder = litSortingOrder;
            _litRaycaster = helpButton.gameObject.AddComponent<GraphicRaycaster>();
        }

        /// <summary>Cierra el aviso, devuelve el tiempo y deja el botón de ayuda como estaba.</summary>
        public void Close()
        {
            if (!IsOpen)
            {
                return;
            }

            Release();
            gameObject.SetActive(false);
            var onClosed = _onClosed;
            _onClosed = null;
            onClosed?.Invoke();
        }

        /// <summary>La escena se descarga con el aviso abierto: el tiempo no puede quedarse detenido.</summary>
        private void OnDestroy()
        {
            if (IsOpen)
            {
                Release();
            }
        }

        private void Release()
        {
            IsOpen = false;
            Time.timeScale = _timeScaleBefore;

            // Por qué no Destroy a secas: el Canvas se niega a salir mientras el GraphicRaycaster
            // siga pegado a él, y Destroy lo quita hasta el final del cuadro. El raycaster sale
            // primero y de inmediato; el Canvas, ya libre, sí puede esperar.
            if (_litRaycaster != null)
            {
                DestroyImmediate(_litRaycaster);
            }

            if (_litCanvas != null)
            {
                Destroy(_litCanvas);
            }
        }
    }
}
