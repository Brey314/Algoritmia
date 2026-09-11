using System;
using System.Collections.Generic;
using Game.Core;
using Game.Scaffolding;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// La **única** escena narrativa (RF-05, RF-06). Recibe qué secuencia reproducir por su
    /// identificador —el que <see cref="GameFlow.NarrativeSequenceId"/> guardó al entrar— y la
    /// resuelve contra la lista de secuencias del proyecto.
    /// </summary>
    /// <remarks>
    /// Aquí está el motivo de que <c>GameState.Narrative</c> sea un estado parametrizado y no
    /// quince: añadir una escena narrativa es crear un asset y arrastrarlo a esta lista, no una
    /// escena, un estado y una rama. Por eso este controlador **no tiene ni un `if` por
    /// secuencia**: las tres del Nivel 1 recorren exactamente el mismo código.
    ///
    /// Adaptador delgado: avanzar, omitir y saber si ya se vio la escena viven en
    /// <see cref="DialogueRunner"/> y <see cref="NarrativeVisitPolicy"/>, que son C# plano.
    ///
    /// No lleva botón de pausa, y es deliberado: HU-17 FA-04 lo pide solo durante el juego.
    /// </remarks>
    public class NarrativeSceneController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Todas las secuencias narrativas. La escena elige por identificador, sin ramas.")]
        private NarrativeSequence[] sequences = Array.Empty<NarrativeSequence>();

        [SerializeField] private Image illustration;
        [SerializeField] private Text speakerLabel;
        [SerializeField] private Text bodyLabel;
        [SerializeField] private Button advanceButton;
        [SerializeField] private Button skipButton;

        internal GameFlowRunner Runner { get; set; }

        /// <summary>El avance de la escena en curso. <c>null</c> si no se pudo resolver.</summary>
        internal DialogueRunner Dialogue { get; private set; }

        /// <summary>La secuencia en curso. Es quien dice a dónde sale la escena.</summary>
        private NarrativeSequence _sequence;

        /// <summary>Dónde está la cámara ahora mismo, de camino al encuadre de la línea en curso.</summary>
        private CameraFraming _camera;

        /// <summary>Los objetos pintados sobre el entorno, con su declaración: son lo que se anima por línea.</summary>
        private readonly List<(NarrativeProp Prop, RectTransform Rect)> _props =
            new List<(NarrativeProp, RectTransform)>();

#if UNITY_INCLUDE_TESTS
        internal Text BodyLabel => bodyLabel;
        internal Text SpeakerLabel => speakerLabel;
        internal Button AdvanceButton => advanceButton;
        internal Button SkipButton => skipButton;
        internal NarrativeSequence[] Sequences => sequences;
        internal RectTransform IllustrationRect => illustration.rectTransform;
        internal CameraFraming CameraTarget => TargetFraming();
        internal IReadOnlyList<(NarrativeProp Prop, RectTransform Rect)> Props => _props;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            advanceButton.onClick.AddListener(Advance);
            skipButton.onClick.AddListener(SkipScene);
            Begin();
        }

        /// <summary>
        /// Arranca la secuencia que el flujo pidió. Se separa de <c>Start</c> porque el runner se
        /// inyecta después de cargar la escena en las pruebas.
        /// </summary>
        internal void Begin()
        {
            if (!ScreenFlow.Ready(Runner, this))
            {
                return;
            }

            var wanted = Runner.Flow.NarrativeSequenceId;
            _sequence = Array.Find(sequences,
                candidate => candidate != null && candidate.Id == wanted);

            if (_sequence == null)
            {
                Debug.LogWarning($"No hay ninguna secuencia narrativa con el id «{wanted}».", this);
                return;
            }

            Dialogue = new DialogueRunner(_sequence.Lines,
                NarrativeVisitPolicy.AlreadySeen(Runner.Flow.ActiveProfile, _sequence));

            illustration.sprite = _sequence.Illustration;
            illustration.enabled = _sequence.Illustration != null;
            illustration.preserveAspect = false; // el tamaño lo fija el encuadre, ya sin deformar
            _camera = _sequence.CameraStart;
            Frame();
            PlaceProps(_sequence);
            // RF-06 e INC-28: el botón de omitir no existe en la primera visita, no basta con
            // deshabilitarlo — la escena de cierre es donde el guía nombra lo aprendido.
            skipButton.gameObject.SetActive(Dialogue.CanSkip);
            Render();
        }

        /// <summary>
        /// La cámara se acerca cada cuadro al encuadre de la línea en curso: se mueve por el
        /// entorno **al ritmo de la narrativa**, no del reloj, y llega suave en vez de saltar.
        /// </summary>
        private void Update()
        {
            if (_sequence == null || _camera == null || !illustration.enabled)
            {
                return;
            }

            // El suavizado lo pone cada escena: un movimiento lento y continuo, sin saltos
            // (RNF-21), pero tan corto o tan largo como pida su ritmo.
            var blend = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(_sequence.CameraSmoothingSeconds, 0.01f));
            _camera = CameraFraming.Lerp(_camera, TargetFraming(), blend);
            Frame();
        }

        /// <summary>
        /// Pinta sobre el entorno los objetos que la secuencia declara, para que lo que dice el
        /// texto se vea: cuelgan de la ilustración y por eso acompañan su paneo y su zoom sin
        /// cálculo propio.
        /// </summary>
        private void PlaceProps(NarrativeSequence sequence)
        {
            foreach (Transform previous in illustration.transform)
            {
                Destroy(previous.gameObject);
            }

            _props.Clear();
            if (illustration.sprite == null)
            {
                return;
            }

            var imageHeight = illustration.sprite.rect.height;
            for (var i = 0; i < sequence.Props.Length; i++)
            {
                var prop = sequence.Props[i];
                var go = new GameObject($"Prop_{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                go.layer = illustration.gameObject.layer;
                var rect = (RectTransform)go.transform;
                rect.SetParent(illustration.rectTransform, false);
                rect.anchorMin = prop.Position;
                rect.anchorMax = prop.Position;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.one * (imageHeight * prop.Size);
                rect.localRotation = Quaternion.Euler(0f, 0f, prop.RotationDegrees);
                rect.localScale = new Vector3(prop.Mirrored ? -1f : 1f, 1f, 1f);

                var image = go.GetComponent<Image>();
                image.sprite = prop.Art;
                image.preserveAspect = true;
                image.raycastTarget = false;
                _props.Add((prop, rect));
            }
        }

        /// <summary>
        /// Arranca lo que cada objeto hace en la línea que acaba de aparecer. Lo que dice el texto
        /// se ve **cuando se lee**: la caja rueda en la primera línea, el tronco que el niño suelta
        /// rueda en la segunda y la piedra que suelta después se queda (guion §6.1.3).
        /// </summary>
        private void PlayMotions(int line)
        {
            foreach (var (prop, rect) in _props)
            {
                if (prop.Motion != PropMotion.None && prop.MotionLine == line)
                {
                    _ = MoveAsync(prop, rect);
                }
            }
        }

        /// <summary>
        /// Un movimiento entero de un objeto: levantar (si alguien lo levanta), soltar, y rodar o
        /// quedarse. Interpolación continua de posición y giro: ningún gráfico cambia de color ni
        /// parpadea (RNF-21).
        /// </summary>
        private async Awaitable MoveAsync(NarrativeProp prop, RectTransform rect)
        {
            var image = illustration.sprite.rect.size;
            var origin = rect.anchoredPosition;
            var lifted = prop.Motion != PropMotion.Roll;
            var rolls = prop.Motion != PropMotion.LiftAndStay;
            var lift = image.y * 0.12f;
            var distance = rolls ? image.x * prop.MotionDistance : 0f;
            // La caída del final del rodado, si la hay: baja lo que diga el asset y avanza una
            // caja más, igual que en el bosque, donde la caja aterriza justo pasada la fila.
            var drop = rolls ? image.y * prop.MotionDrop : 0f;
            var overshoot = drop > 0f ? rect.sizeDelta.x : 0f;
            var rollShare = drop > 0f ? 0.8f : 1f;
            var seconds = Mathf.Max(prop.MotionSeconds, 0.01f);
            var elapsed = 0f;

            try
            {
                do
                {
                    elapsed += Time.deltaTime;
                    var t = Mathf.Clamp01(elapsed / seconds);

                    // Mitad del tiempo para subir y caer si lo levantan; el resto, rodar o quedarse.
                    var air = lifted ? Mathf.Clamp01(t / 0.5f) : 1f;
                    var ground = lifted ? Mathf.Clamp01((t - 0.5f) / 0.5f) : t;
                    var height = lifted ? Mathf.Sin(air * Mathf.PI) * lift : 0f;

                    // **El mismo rodado que el bosque**: RollMotion decide avance, giro y caída.
                    var roll = RollMotion.Evaluate(ground, rollShare);
                    var x = distance * roll.Along + overshoot * roll.Fall;
                    var y = height - drop * roll.Drop;
                    var spin = rolls ? roll.Spin + roll.Tilt : Mathf.Sin(ground * Mathf.PI) * 12f;

                    rect.anchoredPosition = origin + new Vector2(x, y);
                    rect.localRotation = Quaternion.Euler(0f, 0f, prop.RotationDegrees + spin);

                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                } while (elapsed < seconds);
            }
            catch (OperationCanceledException)
            {
                // La escena se descargó a medias: nada que dejar en su sitio.
            }
        }

        /// <summary>
        /// El encuadre que toca ahora. Con paradas, el de la última parada ya leída —y el inicial
        /// hasta la primera—: la vista se queda quieta hasta que el texto la mueve. Sin paradas,
        /// del inicial al final según cuánto va leído.
        /// </summary>
        private CameraFraming TargetFraming()
        {
            if (_sequence.CameraKeys.Length == 0)
            {
                return CameraFraming.Lerp(_sequence.CameraStart, _sequence.CameraEnd, Dialogue?.Progress ?? 0f);
            }

            var index = Dialogue?.Index ?? 0;
            var target = _sequence.CameraStart;
            foreach (var key in _sequence.CameraKeys)
            {
                if (key.Line <= index)
                {
                    target = key.Framing;
                }
            }

            return target;
        }

        /// <summary>
        /// Coloca la ilustración cubriendo la pantalla con el encuadre actual. El arte puede
        /// llegar a cualquier resolución: **sustituir el archivo basta** (RNF-23).
        /// </summary>
        private void Frame()
        {
            if (illustration.sprite == null)
            {
                return;
            }

            IllustrationFraming.Apply(illustration.rectTransform, illustration.sprite.rect.size, _camera);
        }

        private void Advance()
        {
            if (Dialogue == null)
            {
                return;
            }

            if (Dialogue.Advance())
            {
                Render();
                return;
            }

            Leave();
        }

        private void SkipScene()
        {
            if (Dialogue != null && Dialogue.Skip())
            {
                Leave();
            }
        }

        private void Render()
        {
            var line = Dialogue.Current;
            if (line == null)
            {
                return;
            }

            speakerLabel.text = line.Speaker;
            // La acotación —lo que en el guion va en cursiva— no lleva nombre de hablante.
            speakerLabel.gameObject.SetActive(!line.IsStageDirection);
            bodyLabel.text = line.Text;
            PlayMotions(Dialogue.Index);
        }

        /// <summary>
        /// Sale de la escena por donde diga la secuencia: a jugar la fase que declara
        /// (<see cref="NarrativeSequence.NextPhase"/>), al resumen de fin de nivel si es el cierre
        /// reflexivo (<see cref="NarrativeSequence.IsReflectiveClosing"/>, T18, HU-14 paso 6), o al
        /// menú de niveles en cualquier otro caso.
        /// </summary>
        /// <remarks>
        /// **La rama es una y sirve para las quince escenas**, porque quien decide es el asset y
        /// no el controlador (RF-05): las de apertura declaran fase, la de cierre se declara
        /// cierre, y las intermedias no declaran nada.
        ///
        /// Si `StartPlaying`/`GoTo` rechaza la transición —nivel bloqueado (RF-03), o escena
        /// abierta sin pasar por Boot— se cae al menú en vez de quedarse: un clic no puede dejar
        /// al estudiante sin salida (RNF-13).
        /// </remarks>
        private void Leave()
        {
            if (_sequence != null && _sequence.NextPhase > 0
                && Runner.StartPlaying(_sequence.Level, _sequence.NextPhase))
            {
                return;
            }

            if (_sequence != null && _sequence.IsReflectiveClosing
                && Runner.GoTo(GameState.LevelSummary))
            {
                return;
            }

            Runner.GoTo(GameState.LevelSelect);
        }
    }
}
