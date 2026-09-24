using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>
    /// Un personaje animado por recorte: partes (torso, brazos, piernas) que un Animator gira y
    /// desplaza, con un estado por <see cref="ActorAction"/> (Dirección de arte §13.3). Lo usan por
    /// igual la escena narrativa y las cinco mecánicas.
    /// </summary>
    /// <remarks>
    /// **Por qué recorte con <c>Image</c> y no el rig de 2D Animation (§13.1):** todas las escenas son
    /// uGUI en un Canvas overlay, y <c>SpriteSkin</c> solo deforma un <c>SpriteRenderer</c>, que
    /// quedaría debajo del Canvas, tapado por la ilustración. Las partes son <c>Image</c> hijas, así
    /// que el personaje cuelga de la ilustración y acompaña el paneo y el zoom como cualquier objeto.
    ///
    /// Los clips trabajan en unidades del lienzo de los sprites base (1024 × 1024): el lienzo
    /// (<see cref="Stage"/>) se escala para llenar la casilla del personaje, y un balanceo mide lo
    /// mismo en un niño lejano que en Papá de cerca. La escala del lienzo la pone este componente y
    /// ningún clip la anima.
    /// </remarks>
    [RequireComponent(typeof(RectTransform))]
    public class CharacterRig : MonoBehaviour
    {
        /// <summary>Lado del lienzo de los sprites base, en píxeles: la unidad de los clips.</summary>
        public const float CanvasUnits = 1024f;

        /// <summary>Segundos del fundido entre dos acciones: sin saltos de pose (RNF-21).</summary>
        private const float BlendSeconds = 0.18f;

        private static readonly int IdleState = Animator.StringToHash(nameof(ActorAction.Idle));

        [field: SerializeField]
        [field: Tooltip("Nombres con los que el guion lo hace hablar, tal como van en las líneas: «PAPÁ», y también «NIÑOS» en los dos niños.")]
        public string[] SpeakerNames { get; private set; } = Array.Empty<string>();

        [field: SerializeField]
        [field: Tooltip("Retrato del cuadro de diálogo cuando habla (char_*_retrato_neutra). En Algoritm es el sprite de su forma, así que sustituir el archivo cambia los dos.")]
        public Sprite Portrait { get; private set; }

        [SerializeField]
        [Tooltip("El Animator con un estado por acción (Idle, Walk, Strike…). Los clips animan las partes, nunca el lienzo.")]
        private Animator animator;

        [SerializeField]
        [Tooltip("El lienzo de 1024 × 1024 que contiene las partes. Se escala para llenar la casilla del personaje.")]
        private RectTransform stage;

        private Vector2 _fittedSize = new Vector2(-1f, -1f);
        private bool _mirrored;
        private bool _started;
        private CancellationTokenSource _returning;

        /// <summary>La última acción pedida.</summary>
        public ActorAction Current { get; private set; } = ActorAction.Idle;

        /// <summary>El lienzo que contiene las partes.</summary>
        public RectTransform Stage => stage;

        /// <summary>
        /// Si se dibuja en espejo. Voltea el lienzo y no la raíz: en el río la escala de la raíz la
        /// pone la profundidad (<c>RiverSceneController</c>) y la vigila una prueba.
        /// </summary>
        public bool Mirrored
        {
            get => _mirrored;
            set
            {
                _mirrored = value;
                Fit(true);
            }
        }

        private void Awake() => Fit(true);

        private void LateUpdate() => Fit(false);

        /// <summary>
        /// Pasa a la acción pedida con un fundido corto. Un rig sin ese estado hace
        /// <see cref="ActorAction.Idle"/>: la acción del guion nunca rompe la escena.
        /// </summary>
        public void Play(ActorAction action)
        {
            _returning?.Cancel();
            _returning = null;
            Apply(action);
        }

        /// <summary>
        /// Hace la acción durante unos segundos y vuelve sola a <paramref name="then"/>: el gesto
        /// de una mecánica (señalar lo elegido, el ánimo tras un intento) que no se queda puesto.
        /// El tiempo es escalado, así que la pausa lo congela (RF-07). Otra acción pedida antes
        /// lo cancela.
        /// </summary>
        public void PlayFor(ActorAction action, float seconds, ActorAction then = ActorAction.Idle)
        {
            Play(action);
            _returning = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            _ = ReturnAsync(seconds, then, _returning.Token);
        }

        private async Awaitable ReturnAsync(float seconds, ActorAction then, CancellationToken token)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(seconds, token);
            }
            catch (OperationCanceledException)
            {
                return; // otra acción ya ocupó su lugar
            }

            _returning = null;
            Apply(then);
        }

        private void Apply(ActorAction action)
        {
            // Pedir lo que ya hace no lo reinicia: una acción que se mantiene entre líneas —se
            // apagó, sigue arrodillado— no vuelve a empezar cada vez que avanza el texto.
            if (action == Current && _started)
            {
                return;
            }

            _started = true;
            Current = action;
            if (animator == null || animator.runtimeAnimatorController == null || !animator.isActiveAndEnabled)
            {
                return;
            }

            var state = Animator.StringToHash(action.ToString());
            if (!animator.HasState(0, state))
            {
                state = IdleState;
            }

            animator.CrossFadeInFixedTime(state, BlendSeconds, 0);
        }

        /// <summary>Si la línea la dice este personaje. Sin distinguir mayúsculas: el guion las escribe en versales.</summary>
        public bool Speaks(string speaker)
        {
            if (string.IsNullOrWhiteSpace(speaker))
            {
                return false;
            }

            foreach (var name in SpeakerNames)
            {
                if (string.Equals(name?.Trim(), speaker.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tiñe todas las partes, para escenas que tiñen lo que cuelga del entorno (el laberinto al
        /// atardecer, <c>MazeLayout.LightTint</c>). Ningún clip anima el color, así que el tinte dura.
        /// </summary>
        public void Tint(Color color)
        {
            foreach (var graphic in GetComponentsInChildren<Graphic>(true))
            {
                graphic.color = color;
            }
        }

        /// <summary>Escala el lienzo para llenar la casilla, como <c>preserveAspect</c> con un sprite cuadrado.</summary>
        private void Fit(bool force)
        {
            if (stage == null)
            {
                return;
            }

            var size = ((RectTransform)transform).rect.size;
            if (!force && size == _fittedSize)
            {
                return;
            }

            _fittedSize = size;
            var scale = Mathf.Min(size.x, size.y) / CanvasUnits;
            stage.localScale = new Vector3(_mirrored ? -scale : scale, scale, 1f);
        }
    }
}
