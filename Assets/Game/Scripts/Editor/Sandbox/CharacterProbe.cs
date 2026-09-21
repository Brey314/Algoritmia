using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.EditorTools.Sandbox
{
    /// <summary>
    /// La maqueta de personaje: sigue al cursor caminando, se agacha al llegar y rebota contra el
    /// borde de la pantalla.
    /// </summary>
    /// <remarks>
    /// **Maqueta de prueba, no contenido del juego.** Va en <c>Game.EditorTools</c>, que es solo
    /// Editor: no entra en el ejecutable, y su escena no está en <c>EditorBuildSettings</c>.
    /// Lo que se quiere ver aquí es cuánto da de sí la animación por fotogramas frente al rigging
    /// que eligió <c>Direccion_de_Arte.md</c> §13.1 — ver <see cref="CharacterProbeMotion"/>.
    ///
    /// **Los sprites se asignan en el Inspector y hoy no existen**: la carpeta
    /// <c>Assets/Game/Art/Characters/Mother/</c> está vacía y el asset A3 sigue sin marcar en el
    /// `todo.md` del Slice 1. Sin sprites la maqueta se mueve igual, solo que sin dibujo: sirve
    /// para probar el movimiento antes de que llegue el arte, y basta soltar seis imágenes
    /// cualesquiera para verla andar.
    ///
    /// **El sentido izquierdo no lleva sprites propios**: se voltea con la escala, que es lo que
    /// pedía resolver con el motor en vez de con el doble de arte.
    /// </remarks>
    [RequireComponent(typeof(Image))]
    public class CharacterProbe : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Los tres fotogramas de la caminata, en orden. De perfil hacia la derecha: la izquierda se hace volteando.")]
        private Sprite[] walkFrames = new Sprite[3];

        [SerializeField]
        [Tooltip("Los tres fotogramas de agacharse, en orden. Solo sentido derecho.")]
        private Sprite[] crouchFrames = new Sprite[3];

        [SerializeField]
        [Tooltip("A qué velocidad persigue al cursor, en píxeles por segundo.")]
        private float speed = 260f;

        [SerializeField]
        [Tooltip("Cuánto dura cada fotograma, en segundos. Un ciclo de caminata son tres.")]
        private float frameSeconds = 0.12f;

        [SerializeField]
        [Tooltip("A qué distancia del cursor considera que ya llegó y se agacha, en píxeles.")]
        private float arriveRadius = 40f;

        private CharacterProbeMotion _motion;
        private RectTransform _rect;
        private Image _image;

        private void Start()
        {
            _rect = (RectTransform)transform;
            _image = GetComponent<Image>();
            _motion = NewMotion();
        }

        private void Update()
        {
            // Input System nuevo, nunca la clase `Input` legada (CT-06).
            var mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            // La pantalla puede cambiar de tamaño mientras corre: se rehace el confinamiento en
            // vez de dejar a la maqueta rebotando contra un borde que ya no está donde estaba.
            if (!Mathf.Approximately(_motion.Bounds.width, Screen.width) ||
                !Mathf.Approximately(_motion.Bounds.height, Screen.height))
            {
                _motion = NewMotion();
            }

            _motion.Step(mouse.position.ReadValue(), Time.deltaTime);

            _rect.position = _motion.Position;
            _rect.localScale = new Vector3(_motion.Mirrored ? -1f : 1f, 1f, 1f);

            var frames = _motion.Pose == ProbePose.Walking ? walkFrames : crouchFrames;
            if (frames != null && frames.Length > 0)
            {
                var sprite = frames[_motion.Frame % frames.Length];
                _image.sprite = sprite;
                _image.enabled = sprite != null;
            }
        }

        private CharacterProbeMotion NewMotion() => new CharacterProbeMotion(
            new Rect(0f, 0f, Screen.width, Screen.height), speed, frameSeconds, arriveRadius);
    }
}
