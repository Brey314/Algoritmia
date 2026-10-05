using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Los sprites de la cara de un personaje —ojos por emoción, párpados, bocas— y los tiempos de
    /// su parpadeo y su habla (CT-05, plan de personajes finales §5). Un asset por personaje; en
    /// Algoritm, uno por forma, porque cada forma va recoloreada (INC-52).
    /// </summary>
    /// <remarks>
    /// Todo campo puede quedar vacío: <see cref="CharacterFace"/> deja sin dibujar la capa a la que
    /// le falte el sprite, así que el arte provisional se sigue viendo entero y nunca aparece un
    /// recuadro blanco. Por eso los métodos devuelven <c>null</c> en vez de caer a otro sprite.
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritmia/Cara de personaje", fileName = "Cara_")]
    public sealed class CharacterFaceSet : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Ojos de la emoción Neutral: mirada serena. El reposo de todos.")]
        public Sprite EyesNeutral { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Ojos de la emoción Happy: achinados y sonrientes.")]
        public Sprite EyesHappy { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Ojos de la emoción Surprised: muy abiertos.")]
        public Sprite EyesSurprised { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Ojos de la emoción Worried: cejas inclinadas hacia el centro. Es duda, nunca reproche ni tristeza (CP-02).")]
        public Sprite EyesWorried { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Ojos de la emoción Focused: mirada concentrada, cejas bajas.")]
        public Sprite EyesFocused { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Ojos de la emoción Sleeping: cerrados, con pestañas hacia abajo. Mientras duerme no hay parpadeo.")]
        public Sprite EyesSleeping { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Ojos a medio cerrar: el primer y el último tercio del parpadeo.")]
        public Sprite EyesBlinkHalf { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Ojos cerrados: el tercio central del parpadeo.")]
        public Sprite EyesBlinkClosed { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Boca cerrada y relajada: el reposo de Neutral y de Sleeping, y la posición entre sílabas.")]
        public Sprite MouthClosed { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Boca al hablar, apertura amplia (A, O).")]
        public Sprite MouthA { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Boca al hablar, apertura ancha o media (E, I).")]
        public Sprite MouthE { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Boca al hablar, pequeña y redondeada (U, M, B, P).")]
        public Sprite MouthU { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Boca de reposo de la emoción Happy: sonrisa.")]
        public Sprite MouthHappy { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Boca de reposo de la emoción Surprised: en «O».")]
        public Sprite MouthSurprised { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Boca de reposo de la emoción Worried: apenas curvada, nunca una mueca de llanto (CP-02).")]
        public Sprite MouthWorried { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Boca de reposo de la emoción Focused: recta y apretada.")]
        public Sprite MouthFocused { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Segundos medios entre dos parpadeos.")]
        [field: Min(0f)]
        public float BlinkInterval { get; private set; } = 3.5f;

        [field: SerializeField]
        [field: Tooltip("Cuánto se aparta cada intervalo del medio, en más o en menos: 3,5 ± 1,2 s no se repite y no parece un metrónomo.")]
        [field: Min(0f)]
        public float BlinkJitter { get; private set; } = 1.2f;

        [field: SerializeField]
        [field: Tooltip("Lo que dura el parpadeo completo, en segundos.")]
        [field: Min(0.01f)]
        public float BlinkSeconds { get; private set; } = 0.12f;

        [field: SerializeField]
        [field: Tooltip("Cuánto dura cada posición de la boca al hablar (A, E, U, cerrada), en segundos.")]
        [field: Min(0.01f)]
        public float FlapSeconds { get; private set; } = 0.09f;

        /// <summary>Los ojos de la emoción, o <c>null</c> si el set no los trae.</summary>
        public Sprite Eyes(FacialEmotion emotion)
        {
            switch (emotion)
            {
                case FacialEmotion.Happy: return EyesHappy;
                case FacialEmotion.Surprised: return EyesSurprised;
                case FacialEmotion.Worried: return EyesWorried;
                case FacialEmotion.Focused: return EyesFocused;
                case FacialEmotion.Sleeping: return EyesSleeping;
                default: return EyesNeutral;
            }
        }

        /// <summary>
        /// Los ojos durante el parpadeo, o <c>null</c> si el set no los trae. Abiertos son los de la
        /// emoción (<see cref="Eyes"/>).
        /// </summary>
        public Sprite Eyes(BlinkPhase phase, FacialEmotion emotion)
        {
            switch (phase)
            {
                case BlinkPhase.Half: return EyesBlinkHalf;
                case BlinkPhase.Closed: return EyesBlinkClosed;
                default: return Eyes(emotion);
            }
        }

        /// <summary>La boca cuando no habla, según la emoción. Neutral y Sleeping llevan la cerrada.</summary>
        public Sprite RestMouth(FacialEmotion emotion)
        {
            switch (emotion)
            {
                case FacialEmotion.Happy: return MouthHappy;
                case FacialEmotion.Surprised: return MouthSurprised;
                case FacialEmotion.Worried: return MouthWorried;
                case FacialEmotion.Focused: return MouthFocused;
                default: return MouthClosed;
            }
        }

        /// <summary>
        /// La boca en esa forma del habla. En <see cref="MouthShape.Rest"/>, la de reposo de la
        /// emoción; las demás no dependen de ella.
        /// </summary>
        public Sprite Mouth(MouthShape shape, FacialEmotion emotion)
        {
            switch (shape)
            {
                case MouthShape.A: return MouthA;
                case MouthShape.E: return MouthE;
                case MouthShape.U: return MouthU;
                case MouthShape.Closed: return MouthClosed;
                default: return RestMouth(emotion);
            }
        }
    }
}
