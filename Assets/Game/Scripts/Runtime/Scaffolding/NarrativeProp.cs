using System;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Lo que un objeto de la narrativa hace cuando llega su línea. Es el patrón de RF-23 contado
    /// con movimiento: lo redondo rueda, lo anguloso se queda.
    /// </summary>
    public enum PropMotion
    {
        None,
        /// <summary>Rueda a la derecha girando. Con distancia cero, gira en el sitio.</summary>
        Roll,
        /// <summary>Alguien lo levanta, lo suelta y rueda.</summary>
        LiftAndRoll,
        /// <summary>Alguien lo levanta, lo suelta y se queda donde cae: no rueda.</summary>
        LiftAndStay
    }

    /// <summary>
    /// Un objeto pintado sobre el entorno de una escena narrativa, para que lo que dice el texto
    /// se vea en la imagen: los objetos repartidos por el suelo, la caja, los troncos (RF-05, RNF-18).
    /// </summary>
    /// <remarks>
    /// Contenido y no código: la posición va en fracciones de la ilustración y el tamaño en
    /// fracción de su alto, así que acompañan el paneo y el zoom de la cámara y sobreviven a que
    /// el arte cambie de resolución. Una escena que quiera mostrar otra cosa edita su lista.
    /// </remarks>
    [Serializable]
    public class NarrativeProp
    {
        [field: SerializeField]
        [field: Tooltip("Ilustración del objeto.")]
        public Sprite Art { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Dónde va, en fracciones de la ilustración: (0,0) abajo-izquierda, (1,1) arriba-derecha.")]
        public Vector2 Position { get; private set; } = new Vector2(0.5f, 0.3f);

        [field: SerializeField]
        [field: Tooltip("Alto del objeto como fracción del alto de la ilustración. Más lejos, más pequeño.")]
        public float Size { get; private set; } = 0.1f;

        [field: SerializeField]
        [field: Tooltip("Giro en grados, para variar la postura.")]
        public float RotationDegrees { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Si se dibuja en espejo.")]
        public bool Mirrored { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Qué hace el objeto cuando llega su línea: nada, rodar, que lo levanten y ruede, o que lo levanten y se quede (la piedra).")]
        public PropMotion Motion { get; private set; } = PropMotion.None;

        [field: SerializeField]
        [field: Tooltip("Línea, desde 0, en la que arranca el movimiento. Lo que dice el texto se ve cuando se lee.")]
        public int MotionLine { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Cuánto recorre al rodar, como fracción del ancho de la ilustración. Cero: gira en el sitio.")]
        public float MotionDistance { get; private set; } = 0.15f;

        [field: SerializeField]
        [field: Tooltip("Cuánto dura el movimiento entero, en segundos.")]
        public float MotionSeconds { get; private set; } = 1.6f;

        [field: SerializeField]
        [field: Tooltip("Cuánto cae al pasar el último tronco, como fracción del alto de la ilustración. Cero: no cae. Es la caída de la caja al final del rodado.")]
        public float MotionDrop { get; private set; }

        /// <summary>Requerido por la serialización de Unity.</summary>
        private NarrativeProp()
        {
        }

        public NarrativeProp WithMotion(PropMotion motion, int line, float distance, float seconds, float drop = 0f)
        {
            Motion = motion;
            MotionLine = line;
            MotionDistance = distance;
            MotionSeconds = seconds;
            MotionDrop = drop;
            return this;
        }

        public NarrativeProp(Sprite art, Vector2 position, float size, float rotationDegrees = 0f, bool mirrored = false)
        {
            Art = art;
            Position = position;
            Size = size;
            RotationDegrees = rotationDegrees;
            Mirrored = mirrored;
        }
    }
}
