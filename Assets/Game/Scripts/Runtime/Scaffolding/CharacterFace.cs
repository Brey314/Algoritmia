using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scaffolding
{
    /// <summary>
    /// La cara de un personaje: dos <c>Image</c> —ojos y boca— cuyo sprite cambia con la emoción, el
    /// parpadeo y el habla (Dirección de arte §7.3; plan de personajes finales §5). Adaptador
    /// delgado: el reloj del parpadeo (<see cref="BlinkClock"/>), el aleteo de la boca
    /// (<see cref="MouthFlap"/>) y la tabla de sprites (<see cref="CharacterFaceSet"/>) son C# plano.
    /// </summary>
    /// <remarks>
    /// **Sin set, o sin el sprite pedido ni el de la neutra, la <c>Image</c> queda desactivada.** Una
    /// <c>Image</c> sin sprite pinta un recuadro blanco, y hasta que llegue el arte final los
    /// personajes no tienen cara: el componente tiene que ser invisible mientras no haya con qué
    /// dibujarla, y el arte provisional se sigue viendo entero. Una emoción a la que le falta su
    /// sprite no apaga la cara: <see cref="CharacterFaceSet"/> cae a la neutra (06/10/2026).
    ///
    /// Avanza con <see cref="Time.deltaTime"/>, que es escalado: la pausa (<c>timeScale = 0</c>,
    /// RF-07) congela el parpadeo y la boca igual que congela los clips.
    ///
    /// Quien manda aquí es <see cref="CharacterRig"/>: le empuja la emoción de la acción o la que
    /// fija el guion, y si el personaje está diciendo su línea.
    ///
    /// **Dos caras, un solo reloj (INC-134).** El personaje de perfil tiene su propia cara —las capas
    /// <c>profileEyes</c> y <c>profileMouth</c>, con su <c>profileFaceSet</c>—, opcional como todo lo de
    /// perfil. Un único <see cref="BlinkClock"/> y un único <see cref="MouthFlap"/> las gobiernan a las
    /// dos, y las dos se actualizan en cada cuadro aunque solo se vea una (<see cref="CharacterRig"/>
    /// enciende un cuerpo u otro): así el parpadeo y la boca no saltan ni se reinician al cambiar de
    /// vista, que es lo que pasaría si cada cara llevara el suyo. Los tiempos salen del set de frente
    /// y, si ese falta, del de perfil. Cada cara cae a «sin dibujar» por su cuenta: la de perfil sin set o
    /// sin sprite se apaga igual que la de frente, y no afecta a la otra.
    ///
    /// **La tarjeta del cuadro de diálogo también lleva una (INC-148, 10/10/2026).** Es una cara armada en
    /// tiempo de ejecución por <c>NarrativeSceneController</c> (<see cref="Bind"/>) que presta el set de
    /// quien habla en cada línea (<see cref="FaceSet"/>), con la expresión del guion, y parpadea y mueve la
    /// boca mientras la línea está en pantalla igual que el rig. Con una sola boca de hablar en el set, el
    /// aleteo alterna esa boca y la cerrada (<see cref="MouthFlap.CycleFor"/>).
    /// </remarks>
    public sealed class CharacterFace : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("La capa de los ojos. Sin sprite se queda desactivada (una Image vacía pinta un recuadro blanco).")]
        private Image eyes;

        [SerializeField]
        [Tooltip("La capa de la boca. Sin sprite se queda desactivada.")]
        private Image mouth;

        [SerializeField]
        [Tooltip("Los sprites de la cara y los tiempos del parpadeo y del habla. Vacío = el personaje no tiene cara todavía y las dos capas no se dibujan.")]
        private CharacterFaceSet faceSet;

        // INC-134: la cara del cuerpo de perfil (Lienzo/Perfil). Opcional: sin ella, o sin set, no se dibuja.
        [SerializeField]
        [Tooltip("La capa de los ojos del cuerpo de perfil. Sin sprite se queda desactivada. Opcional: un personaje sin perfil la deja vacía.")]
        private Image profileEyes;

        [SerializeField]
        [Tooltip("La capa de la boca del cuerpo de perfil. Sin sprite se queda desactivada. Opcional.")]
        private Image profileMouth;

        [SerializeField]
        [Tooltip("Los sprites de la cara de perfil. Sus tiempos no se usan: el parpadeo y el habla siguen el reloj de la cara de frente, para no saltar al cambiar de vista. Vacío = la cara de perfil no se dibuja.")]
        private CharacterFaceSet profileFaceSet;

        private Func<float> _random;
        private CharacterFaceSet _clocksFor;
        private BlinkClock _blink;
        private MouthFlap _flap;
        private FacialEmotion _emotion = FacialEmotion.Neutral;
        private bool _speaking;

        /// <summary>
        /// El set de la cara de frente: sus sprites y los tiempos del parpadeo y del habla. Asignar otro
        /// rehace los relojes con los tiempos y las bocas del set nuevo y redibuja al instante (INC-148): es
        /// lo que hace la tarjeta del cuadro de diálogo, que es una sola cara y presta la de quien habla
        /// en cada línea. El mismo set de antes no toca nada, así que el parpadeo no salta entre dos líneas
        /// del mismo hablante.
        /// </summary>
        public CharacterFaceSet FaceSet
        {
            get => faceSet;
            set
            {
                if (faceSet == value)
                {
                    return;
                }

                faceSet = value;
                Step(0f); // EnsureClocks ve el set nuevo y rehace el parpadeo y el aleteo
            }
        }

        /// <summary>
        /// Conecta las dos capas de una cara armada en tiempo de ejecución (la de la tarjeta del diálogo,
        /// INC-148), que no pasa por un prefab con ellas serializadas. Las capas de perfil no cambian.
        /// Las capas sin sprite quedan apagadas hasta que haya un set con qué dibujarlas.
        /// </summary>
        public void Bind(Image eyesImage, Image mouthImage)
        {
            eyes = eyesImage;
            mouth = mouthImage;
            Step(0f);
        }

        /// <summary>La emoción puesta. Cambiarla cambia los ojos y la boca de reposo al instante.</summary>
        public FacialEmotion Emotion
        {
            get => _emotion;
            set
            {
                if (_emotion == value)
                {
                    return;
                }

                _emotion = value;
                Step(0f);
            }
        }

        /// <summary>Si el personaje está diciendo su línea. Al dejar de hablar la boca vuelve al reposo de golpe.</summary>
        public bool Speaking
        {
            get => _speaking;
            set
            {
                if (_speaking == value)
                {
                    return;
                }

                _speaking = value;
                Step(0f);
            }
        }

        private void OnEnable() => Step(0f);

        private void Update() => Step(Time.deltaTime);

        /// <summary>
        /// Avanza los relojes <paramref name="deltaSeconds"/> y pone los sprites. Interno y no privado
        /// para probarlo sin cuadros (<c>InternalsVisibleTo</c>).
        /// </summary>
        internal void Step(float deltaSeconds)
        {
            // Los tiempos del parpadeo y del habla salen del set de frente; si ese falta, del de perfil.
            // (Comparaciones explícitas con null: «??» se salta la comprobación de objetos destruidos.)
            var clockSet = faceSet != null ? faceSet : profileFaceSet;
            if (clockSet == null)
            {
                Hide(eyes);
                Hide(mouth);
                Hide(profileEyes);
                Hide(profileMouth);
                return;
            }

            EnsureClocks(clockSet);
            // Dormido, los ojos ya son los cerrados: un parpadeo encima los abriría un instante.
            _blink.Enabled = _emotion != FacialEmotion.Sleeping;
            var phase = _blink.Tick(deltaSeconds);
            var shape = _flap.Tick(deltaSeconds, _speaking);

            // Los relojes avanzan UNA vez por cuadro y las dos caras leen el mismo resultado.
            Dress(faceSet, eyes, mouth, phase, shape);
            Dress(profileFaceSet, profileEyes, profileMouth, phase, shape);
        }

        /// <summary>Pone en una cara —la de frente o la de perfil— los sprites que le tocan a este instante.</summary>
        private void Dress(CharacterFaceSet set, Image eyesImage, Image mouthImage, BlinkPhase phase, MouthShape shape)
        {
            if (set == null)
            {
                Hide(eyesImage);
                Hide(mouthImage);
                return;
            }

            // Si el set no trae los párpados o una boca del habla, se queda lo de la emoción: faltar
            // un cuadro del parpadeo no debe hacer desaparecer los ojos a cada rato.
            var eyesSprite = set.Eyes(phase, _emotion);
            if (eyesSprite == null)
            {
                eyesSprite = set.Eyes(_emotion);
            }

            var mouthSprite = set.Mouth(shape, _emotion);
            if (mouthSprite == null)
            {
                mouthSprite = set.RestMouth(_emotion);
            }

            Show(eyesImage, eyesSprite);
            Show(mouthImage, mouthSprite);
        }

#if UNITY_INCLUDE_TESTS
        /// <summary>Arma la cara sin prefab, para las pruebas. <paramref name="random"/> vacío usa el azar de Unity.</summary>
        internal void Configure(Image eyesImage, Image mouthImage, CharacterFaceSet set, Func<float> random = null) =>
            Configure(eyesImage, mouthImage, set, null, null, null, random);

        /// <summary>Arma la cara con la de perfil también (INC-134), sin prefab, para las pruebas.</summary>
        internal void Configure(Image eyesImage, Image mouthImage, CharacterFaceSet set,
            Image profileEyesImage, Image profileMouthImage, CharacterFaceSet profileSet, Func<float> random = null)
        {
            eyes = eyesImage;
            mouth = mouthImage;
            faceSet = set;
            profileEyes = profileEyesImage;
            profileMouth = profileMouthImage;
            profileFaceSet = profileSet;
            _random = random;
            _clocksFor = null;
            Step(0f);
        }
#endif

        private void EnsureClocks(CharacterFaceSet clockSet)
        {
            if (_clocksFor == clockSet && _blink != null)
            {
                return;
            }

            _clocksFor = clockSet;
            _blink = new BlinkClock(clockSet.BlinkInterval, clockSet.BlinkJitter, clockSet.BlinkSeconds,
                _random ?? (() => UnityEngine.Random.value));
            // El ciclo sale de las bocas que trae el set (INC-148): el arte final trae una sola boca de hablar y
            // con ella la boca alterna abierta y cerrada, en vez de pasar por formas que no existen.
            _flap = new MouthFlap(clockSet.FlapSeconds,
                MouthFlap.CycleFor(clockSet.MouthA != null, clockSet.MouthE != null, clockSet.MouthU != null));
        }

        private static void Show(Image image, Sprite sprite)
        {
            if (image == null)
            {
                return;
            }

            if (sprite == null)
            {
                image.enabled = false;
                return;
            }

            if (image.sprite != sprite)
            {
                image.sprite = sprite;
            }

            image.enabled = true;
        }

        private static void Hide(Image image)
        {
            if (image != null)
            {
                image.enabled = false;
            }
        }
    }
}
