using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Cómo se ve un tronco que rueda: la textura que lo envuelve y desde dónde se lo mira (CT-05).
    /// Lo comparten el bosque y las escenas 2.1 y 2.2, así que el tronco es el mismo jugando que en
    /// la narrativa (INC-50).
    /// </summary>
    [CreateAssetMenu(menuName = "Algoritm/Tronco que rueda", fileName = "N0_TroncoRodante")]
    public class RollingLogLook : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Una sola textura para todo el tronco. A la derecha, un cuadrado con el corte: disco que llena el cuadrado, con el anillo del contorno en su borde. A la izquierda, el resto: la corteza desenrollada, el largo del tronco en horizontal y una vuelta entera en vertical, sin costura entre arriba y abajo. Sin brillos ni sombras pintados: girarían con el tronco.")]
        public Texture2D Texture { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Hacia dónde se aleja el tronco en pantalla, en grados desde la horizontal (positivo: hacia arriba). Arriba a la derecha deja ver el costado de una fila de troncos pegados.")]
        public float AxisDegrees { get; private set; } = 35f;

        [field: SerializeField]
        [field: Tooltip("Desde dónde se mira: 0 de frente (solo el corte), 90 de costado o desde arriba (solo la corteza). En medio, 3/4.")]
        [field: Range(0f, 90f)]
        public float TiltDegrees { get; private set; } = 40f;

        [field: SerializeField]
        [field: Tooltip("Largo del tronco, en radios.")]
        [field: Min(0f)]
        public float Depth { get; private set; } = 1.9f;

        [field: SerializeField]
        [field: Tooltip("Grosor del contorno como fracción del radio. Tiene que coincidir con el anillo pintado en el corte de la textura: la silueta toma su color de ese anillo.")]
        [field: Range(0.01f, 0.5f)]
        public float OutlineWidth { get; private set; } = 0.16f;
    }
}
