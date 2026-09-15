using System;
using UnityEngine;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// Cómo reacciona una categoría de objeto cuando el cursor se le acerca (CT-05, RNF-18).
    /// </summary>
    /// <remarks>
    /// Son parámetros de tacto: se ajustan jugando y mirando, no razonando, así que viven en el
    /// asset y no en constantes del código. Un solo juego de campos describe las cuatro
    /// reacciones porque todas son el mismo modelo con distinta mano:
    ///
    /// | Categoría | Cómo se consigue |
    /// |---|---|
    /// | Tronco redondo | <see cref="Push"/> alto y <see cref="Drag"/> bajo: rueda lejos y suave |
    /// | Piedra | <see cref="StepSize"/> mayor que cero: vuelca una esquina y se detiene |
    /// | Herramienta | <see cref="Push"/> mínimo con <see cref="Drag"/> y <see cref="Spring"/> altos: se arrastra un pelo |
    /// | Planta u hoja | <see cref="Lift"/> contra <see cref="Gravity"/>: un arco lento de vuelta al suelo |
    /// </remarks>
    [Serializable]
    public class NudgeSettings
    {
        [field: SerializeField]
        [field: Tooltip("Categoría cuya reacción describen estos valores.")]
        public ForestObjectCategory Category { get; private set; }

        [field: SerializeField]
        [field: Tooltip("A qué distancia del cursor reacciona, en fracción del suelo. Fuera de este radio no se mueve nada.")]
        public float Radius { get; private set; } = 0.1f;

        [field: SerializeField]
        [field: Tooltip("Con cuánta fuerza lo aparta el cursor. Es lo que separa al tronco, que rueda, del resto.")]
        public float Push { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Cuánto lo levanta del suelo el paso del cursor. Solo la hoja despega; a los demás les vale cero.")]
        public float Lift { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Cuánto tira de él hacia el suelo. Con Lift es lo que dibuja el arco de la hoja.")]
        public float Gravity { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Con cuánta gana vuelve a su sitio. Cero lo deja donde se detenga.")]
        public float Spring { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Resistencia al avance: rozamiento en el suelo o del aire. Alto frena en seco, bajo deja rodar.")]
        public float Drag { get; private set; } = 1f;

        [field: SerializeField]
        [field: Tooltip("Si es mayor que cero el movimiento es a saltos de este tamaño: vuelca una esquina por acercamiento y se para. Es la piedra.")]
        public float StepSize { get; private set; }

        /// <summary>Requerido por la serialización de Unity.</summary>
        private NudgeSettings()
        {
        }

#if UNITY_INCLUDE_TESTS
        /// <summary>
        /// Construye un ajuste en memoria. Solo para pruebas: permite verificar que el modelo sigue
        /// a la configuración y no a un literal (RNF-18) sin ampliar la superficie del módulo.
        /// </summary>
        internal static NudgeSettings Create(ForestObjectCategory category, float radius, float push,
            float drag, float spring, float lift = 0f, float gravity = 0f, float stepSize = 0f) =>
            new NudgeSettings
            {
                Category = category,
                Radius = radius,
                Push = push,
                Drag = drag,
                Spring = spring,
                Lift = lift,
                Gravity = gravity,
                StepSize = stepSize
            };
#endif
    }

    /// <summary>
    /// El apartarse de un objeto del bosque cuando el cursor le pasa cerca (RF-22, RNF-03).
    /// </summary>
    /// <remarks>
    /// **Es adorno, no mecánica, y la distinción importa.** Nada de lo que hace esta clase
    /// selecciona, acepta ni hace avanzar: acercar el cursor no gasta un intento, no penaliza y no
    /// toca el acopio. El único control sigue siendo el clic (CT-06, RNF-02), y por eso mover el
    /// ratón no amplía el esquema de control aunque lo parezca.
    ///
    /// **Lleva carga pedagógica de todas formas.** Lo redondo rueda lejos y liso; lo anguloso
    /// vuelca hasta la siguiente esquina y se para en seco. Eso es RF-23 —encontrar el patrón de
    /// lo que rueda— dicho con movimiento en vez de con un cartel, y es el canal redundante que
    /// pide `Direccion_de_Arte.md` §14.2 para no depender del color. Su §14.3 pone el techo:
    /// amplitudes pequeñas y ciclos lentos, que es lo que vigila
    /// <c>WheelLevelConfig_RNF21_ElMovimientoAmbientalEsPequenoEnTodasLasCategorias</c>.
    ///
    /// C# plano y en fracciones del suelo, no en píxeles: se prueba entero en EditMode sin escena
    /// ni frames, y el tacto sale igual en cualquier resolución.
    /// </remarks>
    public class ForestObjectNudge
    {
        /// <summary>
        /// Cuánta velocidad conserva al rebotar contra el borde. Ni un espejo perfecto —una hoja
        /// no es una bola de billar— ni un absorbente, que se leería como quedarse pegado.
        /// </summary>
        private const float BounceRetention = 0.6f;

        private readonly NudgeSettings _settings;
        private readonly Vector2 _home;
        private readonly Rect _bounds;

        /// <summary>La piedra vuelca una sola vez por acercamiento: esto es lo que la rearma.</summary>
        private bool _armed = true;

        public ForestObjectNudge(NudgeSettings settings, Vector2 home, Rect bounds)
        {
            _settings = settings;
            _home = home;
            _bounds = bounds;
        }

        /// <summary>Cuánto se ha apartado de su sitio, en fracción del suelo.</summary>
        public Vector2 Offset { get; private set; }

        public Vector2 Velocity { get; private set; }

        /// <summary>Dónde está ahora mismo, en fracción del suelo.</summary>
        public Vector2 Position => _home + Offset;

        /// <summary>Avanza un fotograma con el cursor donde esté.</summary>
        public void Step(Vector2 cursor, float deltaTime)
        {
            var away = Position - cursor;
            var distance = away.magnitude;
            var inside = distance < _settings.Radius;

            // Si el cursor cae justo encima no hay dirección que calcular; se aparta a la derecha
            // en vez de dividir por cero y mandar el objeto a NaN, de donde no vuelve.
            var direction = distance > 1e-6f ? away / distance : Vector2.right;

            if (_settings.StepSize > 0f)
            {
                StepInCorners(inside, direction);
                return;
            }

            if (inside)
            {
                // La fuerza decae con la distancia: el objeto se aparta hasta salir del radio y
                // ahí deja de sentir el cursor, que es lo que acota el recorrido sin un tope.
                var strength = 1f - distance / _settings.Radius;
                Velocity += direction * (_settings.Push * strength * deltaTime);
                Velocity += Vector2.up * (_settings.Lift * strength * deltaTime);
            }

            Velocity -= Offset * (_settings.Spring * deltaTime);
            Velocity += Vector2.down * (_settings.Gravity * deltaTime);

            // Resistencia como divisor y no como `velocity *= 1 - drag * dt`: esa forma se vuelve
            // negativa —el objeto sale disparado hacia atrás— en cuanto un fotograma largo hace
            // que `drag * dt` pase de uno, y un pico de carga basta para eso.
            Velocity /= 1f + _settings.Drag * deltaTime;

            Offset += Velocity * deltaTime;

            if (_settings.Gravity > 0f && Offset.y < 0f)
            {
                // El suelo: lo que pesa no se hunde por debajo de su sitio.
                Offset = new Vector2(Offset.x, 0f);
                Velocity = new Vector2(Velocity.x, 0f);
            }

            Confine();
        }

        /// <summary>
        /// El movimiento de lo anguloso: vuelca hasta la siguiente esquina y se para.
        /// </summary>
        /// <remarks>
        /// Sin velocidad ni inercia a propósito. Una piedra no rueda: se apoya en la cara
        /// siguiente y ahí se queda por mucho que el cursor insista, y hay que retirarlo y volver
        /// para que vuelque otra vez. Esa negativa **es** la mitad del patrón que enseña RF-23.
        /// </remarks>
        private void StepInCorners(bool inside, Vector2 direction)
        {
            if (!inside)
            {
                _armed = true;
                return;
            }

            if (!_armed)
            {
                return;
            }

            Offset += direction * _settings.StepSize;
            _armed = false;
            Confine();
        }

        /// <summary>Lo mantiene dentro del área jugable y lo devuelve al tocar el borde (RNF-03).</summary>
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

            Offset = position - _home;
            Velocity = velocity;
        }
    }
}
