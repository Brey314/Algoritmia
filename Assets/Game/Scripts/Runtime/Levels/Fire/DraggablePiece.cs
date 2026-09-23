using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Una pieza del suelo de la cueva —hoja, sílex o pedernal— que se arrastra con clic sostenido
    /// (CT-06, RNF-02) y se queda donde se suelta. No sabe de reglas: solo se mueve y avisa al
    /// soltarla. Aparece girada al azar y, al tomarla, se levanta —crece y se inclina un poco— y
    /// vuelve a posarse al soltarla (pedido de Santiago, 22/09/2026).
    /// </summary>
    /// <remarks>
    /// Va por los eventos de arrastre de uGUI (<c>InputSystemUIInputModule</c>), no por la clase
    /// <c>Input</c> legada. Se recorta al rect del padre para que ninguna pieza salga de la cueva.
    /// Con el componente deshabilitado deja de recibir el arrastre: así se fijan las piezas al
    /// pasar al encendido (T25). Levantarla es una única interpolación de escala y giro —nada
    /// parpadea (RNF-21)— que no toca la posición: esa sigue siendo la del puntero.
    /// </remarks>
    [RequireComponent(typeof(RectTransform))]
    public class DraggablePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [field: SerializeField]
        [field: Tooltip("Hoja, sílex o pedernal.")]
        public PieceKind Kind { get; private set; }

        /// <summary>Cuánto crece la pieza al tomarla (1 = nada): es «levantarla del suelo».</summary>
        [field: SerializeField]
        [field: Tooltip("Cuánto crece la pieza al tomarla (1 = nada). Es la respuesta visible al clic sostenido (RNF-02).")]
        public float LiftScale { get; set; } = 1.12f;

        /// <summary>Grados que se inclina la pieza al tomarla; vuelve a su postura al soltarla.</summary>
        [field: SerializeField]
        [field: Tooltip("Grados que se inclina la pieza al tomarla; vuelve a su postura al soltarla.")]
        public float LiftTiltDegrees { get; set; } = 8f;

        /// <summary>Segundos que dura levantarla, y los mismos para posarla.</summary>
        [field: SerializeField]
        [field: Tooltip("Segundos que dura levantarla, y los mismos para posarla.")]
        public float LiftSeconds { get; set; } = 0.12f;

        private RectTransform _rect;
        private RectTransform _floor;
        private Vector2 _grabOffset;
        private float _restAngle;
        private int _liftVersion;

        /// <summary>Se tomó la pieza del suelo: empieza el arrastre.</summary>
        public event Action<DraggablePiece> PickedUp;

        /// <summary>Se soltó la pieza tras arrastrarla. Es cuando el panel mira si ya está todo reunido.</summary>
        public event Action<DraggablePiece> Dropped;

        /// <summary>Posición en el suelo, en píxeles del lienzo de referencia, centrada en el padre.</summary>
        public Vector2 Position => Rect.anchoredPosition;

        /// <summary>Ancho de la pieza, en la misma unidad que <see cref="Position"/>. No cambia con el giro ni al levantarla.</summary>
        public float Width => Rect.rect.width;

        private RectTransform Rect => _rect != null ? _rect : _rect = (RectTransform)transform;

        private RectTransform Floor => _floor != null ? _floor : _floor = (RectTransform)transform.parent;

        private void Awake()
        {
            // Cada partida las piezas caen en una postura distinta: el suelo de la cueva no es una
            // cuadrícula. Solo el giro es al azar; dónde caen lo fija la escena.
            _restAngle = UnityEngine.Random.Range(0f, 360f);
            Rect.localRotation = Quaternion.Euler(0f, 0f, _restAngle);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _grabOffset = Rect.anchoredPosition - Local(eventData);
            Rect.SetAsLastSibling(); // lo que se arrastra va encima
            _ = LiftAsync(lifted: true);
            OnPickedUp();
        }

        public void OnDrag(PointerEventData eventData) => MoveTo(Local(eventData) + _grabOffset);

        public void OnEndDrag(PointerEventData eventData)
        {
            MoveTo(Local(eventData) + _grabOffset);
            _ = LiftAsync(lifted: false);
            OnDropped();
        }

        /// <summary>Mueve la pieza, sin sacarla del suelo. Lo usan el arrastre, el acomodo y las pruebas.</summary>
        public void MoveTo(Vector2 position)
        {
            var half = Floor.rect.size / 2f - Rect.rect.size / 2f;
            Rect.anchoredPosition = new Vector2(
                Mathf.Clamp(position.x, -half.x, half.x),
                Mathf.Clamp(position.y, -half.y, half.y));
        }

        /// <summary>
        /// Levanta o posa la pieza: crece y se inclina al tomarla, vuelve a su tamaño y a su postura
        /// al soltarla. Un gesto nuevo interrumpe al anterior a medias y arranca desde donde quedó,
        /// así que tomar y soltar rápido no acumula ni salta.
        /// </summary>
        private async Awaitable LiftAsync(bool lifted)
        {
            var version = ++_liftVersion;
            var fromScale = Rect.localScale;
            var fromAngle = Rect.localEulerAngles.z;
            var toScale = Vector3.one * (lifted ? LiftScale : 1f);
            var toAngle = _restAngle + (lifted ? LiftTiltDegrees : 0f);
            var seconds = Mathf.Max(LiftSeconds, 0f);

            try
            {
                for (var elapsed = 0f; elapsed < seconds && version == _liftVersion; elapsed += Time.deltaTime)
                {
                    var t = Mathf.SmoothStep(0f, 1f, elapsed / seconds);
                    Rect.localScale = Vector3.Lerp(fromScale, toScale, t);
                    Rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(fromAngle, toAngle, t));
                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                return; // la pieza se destruyó (cambio de escena) a mitad del gesto.
            }

            if (version == _liftVersion)
            {
                Rect.localScale = toScale;
                Rect.localRotation = Quaternion.Euler(0f, 0f, toAngle);
            }
        }

        private void OnPickedUp() => PickedUp?.Invoke(this);

        private void OnDropped() => Dropped?.Invoke(this);

        private Vector2 Local(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Floor, eventData.position, eventData.pressEventCamera, out var local);
            return local;
        }
    }
}
