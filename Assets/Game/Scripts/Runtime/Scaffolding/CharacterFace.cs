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
    /// **Sin set o sin el sprite pedido, la <c>Image</c> queda desactivada.** Una <c>Image</c> sin
    /// sprite pinta un recuadro blanco, y hasta que llegue el arte final los personajes no tienen
    /// cara: el componente tiene que ser invisible mientras no haya con qué dibujarla, y el arte
    /// provisional se sigue viendo entero.
    ///
    /// Avanza con <see cref="Time.deltaTime"/>, que es escalado: la pausa (<c>timeScale = 0</c>,
    /// RF-07) congela el parpadeo y la boca igual que congela los clips.
    ///
    /// Quien manda aquí es <see cref="CharacterRig"/>: le empuja la emoción de la acción o la que
    /// fija el guion, y si el personaje está diciendo su línea.
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

        private Func<float> _random;
        private CharacterFaceSet _clocksFor;
        private BlinkClock _blink;
        private MouthFlap _flap;
        private FacialEmotion _emotion = FacialEmotion.Neutral;
        private bool _speaking;

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
            if (faceSet == null)
            {
                Hide(eyes);
                Hide(mouth);
                return;
            }

            EnsureClocks();
            // Dormido, los ojos ya son los cerrados: un parpadeo encima los abriría un instante.
            _blink.Enabled = _emotion != FacialEmotion.Sleeping;
            var phase = _blink.Tick(deltaSeconds);
            var shape = _flap.Tick(deltaSeconds, _speaking);

            // Si el set no trae los párpados o una boca del habla, se queda lo de la emoción: faltar
            // un cuadro del parpadeo no debe hacer desaparecer los ojos a cada rato.
            // (Comparaciones explícitas con null: «??» se salta la comprobación de objetos destruidos.)
            var eyesSprite = faceSet.Eyes(phase, _emotion);
            if (eyesSprite == null)
            {
                eyesSprite = faceSet.Eyes(_emotion);
            }

            var mouthSprite = faceSet.Mouth(shape, _emotion);
            if (mouthSprite == null)
            {
                mouthSprite = faceSet.RestMouth(_emotion);
            }

            Show(eyes, eyesSprite);
            Show(mouth, mouthSprite);
        }

#if UNITY_INCLUDE_TESTS
        /// <summary>Arma la cara sin prefab, para las pruebas. <paramref name="random"/> vacío usa el azar de Unity.</summary>
        internal void Configure(Image eyesImage, Image mouthImage, CharacterFaceSet set, Func<float> random = null)
        {
            eyes = eyesImage;
            mouth = mouthImage;
            faceSet = set;
            _random = random;
            _clocksFor = null;
            Step(0f);
        }
#endif

        private void EnsureClocks()
        {
            if (_clocksFor == faceSet && _blink != null)
            {
                return;
            }

            _clocksFor = faceSet;
            _blink = new BlinkClock(faceSet.BlinkInterval, faceSet.BlinkJitter, faceSet.BlinkSeconds,
                _random ?? (() => UnityEngine.Random.value));
            _flap = new MouthFlap(faceSet.FlapSeconds);
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
