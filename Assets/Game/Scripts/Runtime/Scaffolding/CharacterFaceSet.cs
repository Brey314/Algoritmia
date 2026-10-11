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
    /// recuadro blanco.
    ///
    /// **Una emoción sin su sprite cae a la neutra** (<see cref="Eyes(FacialEmotion)"/>,
    /// <see cref="RestMouth"/>): el arte llega expresión a expresión, y con solo la neutra entregada
    /// el personaje perdía la cara entera en las doce acciones que no son Neutral (decisión del
    /// 06/10/2026). Mejor una cara serena donde el guion pedía alegría que un personaje sin cara; la
    /// emoción solo cambia el dibujo, nunca lo que el estudiante entiende. Si tampoco hay neutra
    /// los métodos devuelven <c>null</c> y la cara se apaga, como antes. Los cuadros del parpadeo y
    /// de las bocas del habla NO caen aquí: devuelven <c>null</c> y <see cref="CharacterFace"/> vuelve
    /// al reposo de la emoción, que ya es el respaldo correcto (un párpado a medias no se sustituye
    /// por unos ojos de otro tipo). **Una sola excepción (INC-135, 09/10/2026):** el arte final trae
    /// dos cuadros del parpadeo, ojos abiertos y ojos cerrados, sin párpado a medias; sin
    /// <see cref="EyesBlinkHalf"/> el cuadro medio es el cerrado, y el parpadeo enseña los ojos
    /// cerrados sus 0,12 s completos y no los 0,04 s del tercio central.
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
        [field: Tooltip("Ojos de la emoción Sleeping: cerrados, con pestañas hacia abajo. Mientras duerme no hay parpadeo. Opcional: si se deja vacío (el arte final no trae sueño), duerme con los ojos cerrados del parpadeo.")]
        public Sprite EyesSleeping { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Ojos a medio cerrar: el primer y el último tercio del parpadeo. Opcional: si se deja vacío (el arte final trae solo abiertos y cerrados), esos dos tercios usan los ojos cerrados.")]
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

        /// <summary>
        /// Los ojos de la emoción; si el set no los trae, los de Neutral; si tampoco, <c>null</c>. Los de
        /// Sleeping caen primero a los ojos cerrados del parpadeo (<see cref="EyesBlinkClosed"/>) y solo
        /// después a los de Neutral: dormir es tener los ojos cerrados, y unos ojos abiertos en un personaje
        /// dormido dirían lo contrario (INC-148).
        /// </summary>
        public Sprite Eyes(FacialEmotion emotion)
        {
            switch (emotion)
            {
                case FacialEmotion.Happy: return OrNeutral(EyesHappy, EyesNeutral);
                case FacialEmotion.Surprised: return OrNeutral(EyesSurprised, EyesNeutral);
                case FacialEmotion.Worried: return OrNeutral(EyesWorried, EyesNeutral);
                case FacialEmotion.Focused: return OrNeutral(EyesFocused, EyesNeutral);
                case FacialEmotion.Sleeping: return OrNeutral(EyesSleeping, OrNeutral(EyesBlinkClosed, EyesNeutral));
                default: return EyesNeutral;
            }
        }

        /// <summary>
        /// Los ojos durante el parpadeo, o <c>null</c> si el set no trae el cuadro (sin caer a otro
        /// sprite: <see cref="CharacterFace"/> se queda con los de la emoción). Abiertos son los de la
        /// emoción (<see cref="Eyes(FacialEmotion)"/>, con su respaldo a la neutra). Sin cuadro medio
        /// (<see cref="EyesBlinkHalf"/>) el parpadeo de dos cuadros usa el cerrado también en los
        /// tercios de bajada y de subida (INC-135).
        /// </summary>
        public Sprite Eyes(BlinkPhase phase, FacialEmotion emotion)
        {
            switch (phase)
            {
                // Comparación explícita con null: «??» se salta la comprobación de objetos destruidos.
                case BlinkPhase.Half: return EyesBlinkHalf != null ? EyesBlinkHalf : EyesBlinkClosed;
                case BlinkPhase.Closed: return EyesBlinkClosed;
                default: return Eyes(emotion);
            }
        }

        /// <summary>
        /// La boca cuando no habla, según la emoción. Neutral y Sleeping llevan la cerrada, y es también
        /// la que usa una emoción a la que le falte la suya; si tampoco hay cerrada, <c>null</c>.
        /// </summary>
        public Sprite RestMouth(FacialEmotion emotion)
        {
            switch (emotion)
            {
                case FacialEmotion.Happy: return OrNeutral(MouthHappy, MouthClosed);
                case FacialEmotion.Surprised: return OrNeutral(MouthSurprised, MouthClosed);
                case FacialEmotion.Worried: return OrNeutral(MouthWorried, MouthClosed);
                case FacialEmotion.Focused: return OrNeutral(MouthFocused, MouthClosed);
                default: return MouthClosed;
            }
        }

        /// <summary>
        /// La boca en esa forma del habla. En <see cref="MouthShape.Rest"/>, la de reposo de la
        /// emoción (<see cref="RestMouth"/>); las demás no dependen de ella y devuelven <c>null</c> si
        /// el set no trae el cuadro.
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

        /// <summary>
        /// El sprite de la emoción si existe y, si no, el de la neutra. Comparación explícita con
        /// <c>null</c>: «??» se salta la comprobación de objetos destruidos de Unity.
        /// </summary>
        private static Sprite OrNeutral(Sprite emotion, Sprite neutral)
        {
            return emotion != null ? emotion : neutral;
        }
    }
}
