using UnityEngine;

namespace Game.EditorTools.Sandbox
{
    /// <summary>Qué está haciendo el personaje ahora mismo.</summary>
    public enum ProbePose
    {
        Walking = 0,
        Crouching = 1
    }

    /// <summary>
    /// El movimiento y la elección de fotograma de la maqueta de personaje: camina hacia el
    /// cursor, se agacha al llegar y no se sale de la pantalla.
    /// </summary>
    /// <remarks>
    /// **Esto es una maqueta de prueba y no entra en el juego.** Vive en <c>Game.EditorTools</c>,
    /// que es solo Editor, así que no puede acabar en el ejecutable ni contar para RNF-06.
    ///
    /// **Contradice a `Direccion_de_Arte.md` §13.1 a propósito, y por eso existe.** Ese apartado
    /// eligió rigging 2D sobre animación fotograma a fotograma, con el argumento de que los
    /// generadores de imagen no dan secuencias consistentes; su §13.3 además pone a Mamá
    /// caminando en el Nivel 3 con vista superior y cuatro direcciones, no de perfil con espejo.
    /// Esta maqueta sirve para ver cuánto cuesta la alternativa antes de decidir. **Si el
    /// fotograma a fotograma se adopta, hay que corregir §13.1 explícitamente**; mientras tanto
    /// la ley visual sigue siendo la que está escrita.
    ///
    /// C# plano, en píxeles y sin MonoBehaviour: se prueba entero en EditMode sin escena ni
    /// frames, igual que la lógica de los niveles.
    /// </remarks>
    public class CharacterProbeMotion
    {
        /// <summary>Cuánta velocidad conserva al rebotar contra el borde.</summary>
        private const float BounceRetention = 0.6f;

        private readonly float _speed;
        private readonly float _frameSeconds;
        private readonly float _arriveRadius;
        private readonly int _frameCount;
        private readonly Rect _bounds;

        private float _elapsed;

        public CharacterProbeMotion(Rect bounds, float speed, float frameSeconds,
            float arriveRadius, int frameCount = 3)
        {
            _bounds = bounds;
            _speed = speed;
            _frameSeconds = Mathf.Max(frameSeconds, 1e-4f);
            _arriveRadius = arriveRadius;
            _frameCount = Mathf.Max(frameCount, 1);
            Position = bounds.center;
        }

        /// <summary>El área en que se la confina, en píxeles.</summary>
        public Rect Bounds => _bounds;

        public Vector2 Position { get; private set; }

        public Vector2 Velocity { get; private set; }

        public ProbePose Pose { get; private set; } = ProbePose.Crouching;

        /// <summary>Qué fotograma de la secuencia toca pintar, empezando en cero.</summary>
        public int Frame { get; private set; }

        /// <summary>
        /// Si hay que voltear el sprite. **Nunca es cierto agachada**: de esa postura solo existe
        /// el sentido derecho, así que voltearla mostraría una pose que no se dibujó.
        /// </summary>
        public bool Mirrored { get; private set; }

        /// <summary>Avanza un fotograma persiguiendo al cursor donde esté, en píxeles.</summary>
        public void Step(Vector2 target, float deltaTime)
        {
            var toTarget = target - Position;
            var distance = toTarget.magnitude;

            if (distance > _arriveRadius)
            {
                Pose = ProbePose.Walking;
                Velocity = toTarget / distance * _speed;

                // Solo se decide el espejo mientras camina: al pararse conserva hacia dónde
                // miraba, en vez de dar un volantazo cuando la velocidad cae a cero.
                Mirrored = Velocity.x < 0f;
            }
            else
            {
                // Llegó: se agacha a recoger, siempre de cara a la derecha.
                Pose = ProbePose.Crouching;
                Velocity = Vector2.zero;
                Mirrored = false;
            }

            Position += Velocity * deltaTime;
            Confine();

            _elapsed += deltaTime;
            Frame = Mathf.FloorToInt(_elapsed / _frameSeconds) % _frameCount;
        }

        /// <summary>La mantiene en pantalla y la devuelve al tocar el borde.</summary>
        /// <remarks>
        /// ponytail: el rebote dura un fotograma — persiguiendo al cursor, la velocidad se
        /// recalcula entera en el <see cref="Step"/> siguiente y borra la reflexión, así que lo
        /// que se ve es que se para contra el borde. Lo que aquí importa de verdad es que **nunca
        /// se sale**. Si hace falta un rebote visible, hay que darle inercia: acumular la
        /// velocidad en vez de reasignarla, con el coste de que perseguir el cursor se vuelva
        /// flotante. El rebote con recorrido está en <c>ForestObjectNudge</c>, donde los objetos
        /// sí conservan la suya.
        /// </remarks>
        private void Confine()
        {
            var position = Position;
            var velocity = Velocity;

            if (position.x < _bounds.xMin || position.x > _bounds.xMax)
            {
                position.x = Mathf.Clamp(position.x, _bounds.xMin, _bounds.xMax);
                velocity.x = -velocity.x * BounceRetention;
            }

            if (position.y < _bounds.yMin || position.y > _bounds.yMax)
            {
                position.y = Mathf.Clamp(position.y, _bounds.yMin, _bounds.yMax);
                velocity.y = -velocity.y * BounceRetention;
            }

            Position = position;
            Velocity = velocity;
        }
    }
}
