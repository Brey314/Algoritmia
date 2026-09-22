using System;
using Game.Audio;
using Game.Core;
using Game.Scaffolding;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.Fire
{
    /// <summary>
    /// El panel de encendido del Nivel 1 (RF-14, guion §4.3.1), en dos fases (Fase 6, 15/09/2026):
    /// **reunir** —arrastrar hojas, sílex y pedernal al círculo del centro; «Pista» dibuja el
    /// círculo— y **encender** —la cámara se acerca, las hojas se acomodan en fogata y aparecen
    /// los deslizantes de fuerza y de cercanía de las piedras con «Golpear» y «Soplar»—. Traduce
    /// los clics a <see cref="FireAttempt"/> / <see cref="FireFeedbackLog"/> y el estado a la UI.
    /// </summary>
    /// <remarks>
    /// Adaptador delgado: **no contiene ninguna regla del juego**. Si está todo reunido lo dice
    /// <see cref="FireArrangement"/>; dónde van las piedras y si chocan, <see cref="StoneSpacing"/>;
    /// los deslizantes representan la hipótesis y no producen efecto hasta «Golpear» (RF-15); el
    /// resultado de cada golpe lo decide <see cref="FireAttempt"/> (T12) y el mensaje
    /// <see cref="FireFeedbackLog"/> (T13). Al converger y accionar «Soplar», anima el nacimiento
    /// del fuego y encadena a la escena narrativa de cierre (T15, RF-20).
    /// </remarks>
    public class FirePanelController : MonoBehaviour
    {
        [SerializeField] private FireLevelConfig config;
        [SerializeField] private FireMessages messages;

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("positionSlider")]
        [Tooltip("Deslizante de fuerza (Fase 5). Su rango lo fija la configuración al arrancar.")]
        private Slider forceSlider;

        [SerializeField]
        [Tooltip("Deslizante de cercanía de las piedras (T26): a la izquierda lejos, a la derecha una encima de otra. Su rango lo fija la configuración.")]
        private Slider spacingSlider;

        [SerializeField] private Button strikeButton;
        [SerializeField] private Button blowButton;

        [SerializeField]
        [Tooltip("Icono de candado sobre «Soplar»: el segundo indicador de RNF-19 mientras está atenuado.")]
        private GameObject blowLockedBadge;

        [SerializeField] private FeedbackLogView logView;

        [SerializeField]
        [Tooltip("Las piezas del suelo: hojas, sílex y pedernal (T22). Hermanas del punto del fuego.")]
        private DraggablePiece[] pieces = Array.Empty<DraggablePiece>();

        [SerializeField]
        [Tooltip("El punto del fuego: el centro de la pantalla, donde se reúnen los materiales y se arma la fogata.")]
        private RectTransform fireSpot;

        [SerializeField]
        [Tooltip("Contorno de la zona de reunión (T25). Se dibuja al pedir ayuda; su tamaño lo fija el panel a partir del botón de abajo.")]
        private Image gatherRing;

        [SerializeField]
        [Tooltip("Lo que la cámara acerca al reunir los materiales: el fondo de la cueva y el suelo con las piezas (T25).")]
        private RectTransform[] environment = Array.Empty<RectTransform>();

        [SerializeField]
        [Tooltip("La interfaz del encendido —deslizantes, etiquetas, «Golpear» y «Soplar»—, oculta mientras se reúnen los materiales (T25).")]
        private GameObject[] ignitionUi = Array.Empty<GameObject>();

        [SerializeField]
        [Tooltip("Iluminación progresiva del escenario (RF-21, prioridad Baja). Puede quedar sin " +
            "asignar: el resto del nivel sigue jugable sin ella.")]
        private CaveLightingController lighting;

        [SerializeField]
        [Tooltip("Pasos del guía del Nivel 1 (N1_Guia): instrucción y pista de «Reunir», «Golpear» y «Soplar».")]
        private GuideContent guide;

        [SerializeField]
        [Tooltip("Botón «Pista» (mockup 7, arriba a la izquierda): repite la instrucción del paso (RF-13) y, al reunir, dibuja el círculo.")]
        private Button hintButton;

        [SerializeField]
        [Tooltip("Las piezas de sonido del nivel (N1_Sonidos). Puede quedar sin asignar: el nivel se juega igual en silencio — el audio refuerza, nunca informa solo (§2.4).")]
        private FireSounds sounds;

        private FireAttempt _attempt;
        private FireFeedbackLog _log;
        private FireIndicatorCollector _indicators;
        private HintPolicy _hints;
        private StoneSpacing _spacing;

        /// <summary>El flujo del juego. Sin él (escena abierta sin pasar por Boot) no se navega.</summary>
        internal GameFlowRunner Runner { get; set; }

        /// <summary>Todavía se están reuniendo los materiales: sin interfaz de encendido (T25).</summary>
        internal bool IsGathering { get; private set; } = true;

        /// <summary>La cámara se está acercando y las hojas acomodando: entre las dos fases.</summary>
        internal bool IsTransitioning { get; private set; }

        /// <summary>La fuerza que marca el deslizante. Solo <see cref="Strike"/> la lee (RF-15).</summary>
        internal int SelectedForce =>
            Mathf.Clamp(Mathf.RoundToInt(forceSlider.value), 0, config.ForceLevels);

        /// <summary>La muesca de cercanía que marca el deslizante. Solo <see cref="Strike"/> la juzga (RF-15).</summary>
        internal int SelectedSpacing =>
            Mathf.Clamp(Mathf.RoundToInt(spacingSlider.value), 0, config.SpacingLevels);

        /// <summary>
        /// Radio del círculo de reunión: del centro a la mitad del botón de abajo en horizontal
        /// (pedido de Santiago, 15/09/2026). Sale de la escena, no del asset: es disposición.
        /// </summary>
        internal float GatherRadius
        {
            get
            {
                var button = (RectTransform)strikeButton.transform;
                var center = Floor.InverseTransformPoint(button.TransformPoint(button.rect.center));
                return Mathf.Abs(center.x - fireSpot.anchoredPosition.x);
            }
        }

        /// <summary>Todas las piezas dentro del círculo de reunión (T25).</summary>
        internal bool AllGathered =>
            new FireArrangement(fireSpot.anchoredPosition, GatherRadius)
                .IsGathered(Array.ConvertAll(pieces, piece => piece.Position));

#if UNITY_INCLUDE_TESTS
        internal Slider ForceSlider => forceSlider;
        internal Slider SpacingSlider => spacingSlider;
        internal Button StrikeButton => strikeButton;
        internal Button BlowButton => blowButton;
        internal GameObject BlowLockedBadge => blowLockedBadge;
        internal FeedbackLogView LogView => logView;
        internal DraggablePiece[] Pieces => pieces;
        internal RectTransform FireSpot => fireSpot;
        internal Image GatherRing => gatherRing;
        internal RectTransform[] Environment => environment;
        internal GameObject[] IgnitionUi => ignitionUi;
        internal FireAttempt Attempt => _attempt;
        internal FireFeedbackLog Log => _log;
        internal FireIndicatorCollector Indicators => _indicators;
        internal StoneSpacing Spacing => _spacing;
        internal CaveLightingController Lighting => lighting;
        internal Button HintButton => hintButton;
        internal FireSounds Sounds => sounds;
#endif

        private RectTransform Floor => (RectTransform)fireSpot.parent;

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            _attempt = new FireAttempt(config);
            _log = new FireFeedbackLog(messages, config.MinimumEffectiveStrikes);
            _indicators = new FireIndicatorCollector(config, () => Time.realtimeSinceStartup);
            if (Runner != null)
            {
                Runner.ActiveReporter = _indicators;
            }

            // La ayuda es una capa aparte (CP-06, T11): el panel solo la pulsa. Sin asset de guía
            // la política queda con paso nulo y no escribe nada — el nivel sigue jugable.
            _hints = new HintPolicy(Step("Reunir"), config.AttemptsBeforeHint);

            // Los rangos de los deslizantes salen de la configuración, no de la escena (CT-05, RNF-18).
            forceSlider.wholeNumbers = true;
            forceSlider.minValue = 0;
            forceSlider.maxValue = config.ForceLevels;
            spacingSlider.wholeNumbers = true;
            spacingSlider.minValue = 0;
            spacingSlider.maxValue = config.SpacingLevels;
            spacingSlider.onValueChanged.AddListener(_ => PlaceStones());

            // Lo más lejos que se separan las piedras es la longitud de una hoja (T26).
            _spacing = new StoneSpacing(config, WidthOf(PieceKind.Silex), WidthOf(PieceKind.Leaf));

            strikeButton.onClick.AddListener(Strike);
            blowButton.onClick.AddListener(Blow);
            if (hintButton != null)
            {
                hintButton.onClick.AddListener(RequestHelp);
            }

            foreach (var piece in pieces)
            {
                piece.PickedUp += Piece_PickedUp;
                piece.Dropped += Piece_Dropped;
            }

            // La cueva ya sonaba en la escena 1.2: pedir el mismo ambiente la deja seguir sin costura.
            if (sounds != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAmbient(sounds.CaveAmbient);
            }

            // Reuniendo solo hay piezas, tablilla y «Pista»: la interfaz del encendido llega con
            // el acercamiento (T25). El círculo se dibuja al pedir ayuda.
            gatherRing.gameObject.SetActive(false);
            foreach (var element in ignitionUi)
            {
                element.SetActive(false);
            }

            RefreshBlow();
            RefreshLighting();
        }

        /// <summary>Una hoja suena mientras se la lleva (clic sostenido, RNF-02); las piedras no.</summary>
        private void Piece_PickedUp(DraggablePiece piece)
        {
            if (piece.Kind == PieceKind.Leaf && sounds != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHeld(sounds.LeafDrag);
            }
        }

        private void Piece_Dropped(DraggablePiece _)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopHeld();
            }

            TryFinishGathering();
        }

        /// <summary>Dispara una pieza de sonido, si hay gestor y hay pieza. Sin gestor (escena abierta sin pasar por Boot) no suena nada y el nivel sigue.</summary>
        private static void Play(AudioClip clip, float pitchJitter = 0f, float volume = 1f)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(clip, pitchJitter, volume);
            }
        }

        /// <summary>Si ya está todo dentro del círculo, pasa al encendido. Lo llaman las piezas al soltarse y las pruebas.</summary>
        internal void TryFinishGathering()
        {
            if (!IsGathering || IsTransitioning || !AllGathered)
            {
                return;
            }

            _ = EnterIgnitionAsync();
        }

        /// <summary>
        /// El paso de reunir a encender (T25): las piezas dejan de arrastrarse, la cámara se
        /// acerca al entorno, las hojas se acomodan en fogata —juntas, no encimadas— y las
        /// piedras donde diga el deslizante; al terminar aparece la interfaz del encendido.
        /// </summary>
        private async Awaitable EnterIgnitionAsync()
        {
            IsTransitioning = true;
            gatherRing.gameObject.SetActive(false);
            Play(sounds?.Gathered); // todo reunido: el paso al encendido suena una vez
            foreach (var piece in pieces)
            {
                piece.enabled = false; // ya no se arrastran: desde aquí las piedras las mueve el deslizante
            }

            var starts = Array.ConvertAll(pieces, piece => piece.Position);
            var targets = CampfireLayout();
            var seconds = Mathf.Max(config.GatherSeconds, 0f);

            try
            {
                // Una única interpolación continua: nada parpadea ni cambia de color (RNF-21).
                for (var elapsed = 0f; elapsed < seconds; elapsed += Time.deltaTime)
                {
                    var t = Mathf.SmoothStep(0f, 1f, elapsed / seconds);
                    Zoom(Mathf.Lerp(1f, config.GatherZoom, t));
                    for (var i = 0; i < pieces.Length; i++)
                    {
                        pieces[i].MoveTo(Vector2.Lerp(starts[i], targets[i], t));
                    }

                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                return; // el panel se destruyó (cambio de escena) a mitad del acercamiento.
            }

            Zoom(config.GatherZoom);
            for (var i = 0; i < pieces.Length; i++)
            {
                pieces[i].MoveTo(targets[i]);
            }

            IsGathering = false;
            IsTransitioning = false;
            foreach (var element in ignitionUi)
            {
                element.SetActive(true);
            }

            PlaceStones();
            RefreshBlow();

            // La tarea cambia: la tablilla pasa a la instrucción de golpear (guion §4.3.1).
            _hints.Activate(Step("Golpear"));
            _log.RecordGuide(_hints.RequestHelp());
            logView.Show(_log.Entries);
        }

        private void Zoom(float scale)
        {
            foreach (var element in environment)
            {
                element.localScale = Vector3.one * scale;
            }
        }

        /// <summary>
        /// Dónde acaba cada pieza al encender: las hojas en anillo alrededor del punto del fuego,
        /// tangentes entre sí —juntas, no una encima de otra—, y las piedras en el centro, a la
        /// distancia que marque el deslizante.
        /// </summary>
        private Vector2[] CampfireLayout()
        {
            var spot = fireSpot.anchoredPosition;
            var leaves = Array.FindAll(pieces, piece => piece.Kind == PieceKind.Leaf).Length;
            var leafWidth = WidthOf(PieceKind.Leaf);
            // Radio con el que dos hojas vecinas del anillo se tocan sin encimarse.
            var ringRadius = leaves > 1 ? leafWidth / (2f * Mathf.Sin(Mathf.PI / leaves)) : 0f;
            var distance = _spacing.Distance(SelectedSpacing);

            var targets = new Vector2[pieces.Length];
            var leaf = 0;
            for (var i = 0; i < pieces.Length; i++)
            {
                targets[i] = pieces[i].Kind switch
                {
                    PieceKind.Silex => spot + new Vector2(-distance / 2f, 0f),
                    PieceKind.Pedernal => spot + new Vector2(distance / 2f, 0f),
                    _ => spot + ringRadius * Direction(leaf++, leaves)
                };
            }

            return targets;
        }

        private static Vector2 Direction(int index, int count)
        {
            var angle = Mathf.PI / 2f + 2f * Mathf.PI * index / count; // la primera hoja arriba
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        /// <summary>El sílex y el pedernal a la distancia de la muesca, uno a cada lado del punto del fuego (T26).</summary>
        private void PlaceStones()
        {
            if (IsGathering)
            {
                return; // reuniendo, las piedras van donde el estudiante las suelte
            }

            var spot = fireSpot.anchoredPosition;
            var distance = _spacing.Distance(SelectedSpacing);
            foreach (var piece in pieces)
            {
                if (piece.Kind == PieceKind.Silex)
                {
                    piece.MoveTo(spot + new Vector2(-distance / 2f, 0f));
                    piece.transform.SetAsLastSibling(); // las piedras van sobre las hojas
                }
                else if (piece.Kind == PieceKind.Pedernal)
                {
                    piece.MoveTo(spot + new Vector2(distance / 2f, 0f));
                    piece.transform.SetAsLastSibling();
                }
            }
        }

        private float WidthOf(PieceKind kind)
        {
            var piece = Array.Find(pieces, candidate => candidate.Kind == kind);
            return piece != null ? piece.Width : 1f;
        }

        /// <summary>Ejecuta un golpe con la fuerza y la cercanía marcadas y refresca la UI (RF-16).</summary>
        internal void Strike()
        {
            if (IsGathering)
            {
                return; // sin fogata no hay qué golpear: el botón ni siquiera está en pantalla.
            }

            var outcome = _attempt.Strike(SelectedForce, _spacing.Classify(SelectedSpacing));
            _indicators.RecordStrike(outcome);
            _log.Record(outcome, _attempt.ConsecutiveFailures);

            // «Por qué no» un sonido de fallo: un golpe sin chispa suena a lo que pasó —separadas,
            // las piedras no chocan y no se oye nada; en contacto chocan, más fuerte cuanto más
            // encimadas, y nada más—, nunca a pitido de error ni a acorde de derrota (Dirección de
            // sonido §2.1, CP-02). El efectivo añade la chispa que cae en las hojas (guion §4.3.3).
            // Quien ponga aquí un sonido para el fallo reintroduce la penalización que el proyecto
            // prohíbe.
            if (sounds != null)
            {
                var contact = _spacing.Contact(SelectedSpacing);
                if (contact > 0f)
                {
                    Play(sounds.Strike, sounds.StrikePitchJitter, Mathf.Lerp(sounds.StrikeMinVolume, 1f, contact));
                }

                if (outcome.Effective)
                {
                    Play(sounds.Spark);
                }
            }
            if (outcome.Effective)
            {
                _hints.RegisterSuccessfulAttempt();
                if (outcome.EffectiveStrikes == config.MinimumEffectiveStrikes)
                {
                    _hints.Activate(Step("Soplar")); // la tarea cambia al converger (guion §4.3.6)
                }
            }
            else
            {
                _log.RecordGuide(_hints.RegisterFailedAttempt()); // pista solo al tercer fallo seguido
            }

            logView.Show(_log.Entries);
            RefreshBlow();
            RefreshLighting();
        }

        /// <summary>
        /// «Pista»: repite la instrucción del paso activo en la tablilla (RF-13, HU-03). Reuniendo,
        /// además dibuja el círculo donde tiene que quedar todo (T25).
        /// </summary>
        internal void RequestHelp()
        {
            _log.RecordGuide(_hints.RequestHelp());
            logView.Show(_log.Entries);
            if (IsGathering && !IsTransitioning)
            {
                ShowRing();
            }
        }

        private void ShowRing()
        {
            var diameter = 2f * GatherRadius;
            if (gatherRing.sprite == null)
            {
                gatherRing.sprite = RingSprite.Create(Mathf.RoundToInt(diameter), thickness: 6f);
            }

            var rect = gatherRing.rectTransform;
            rect.anchoredPosition = fireSpot.anchoredPosition;
            rect.sizeDelta = Vector2.one * diameter;
            gatherRing.gameObject.SetActive(true);
        }

        private GuideStep Step(string id) =>
            guide != null ? Array.Find(guide.Steps, step => step.Id == id) : null;

        /// <summary>Acciona «Soplar». Atenuado antes de la convergencia: no responde (RF-19).</summary>
        internal void Blow()
        {
            // Guarda defensiva: el botón ya está atenuado antes de converger (guion §4.3.6), pero
            // un clic simulado en pruebas no pasa por esa comprobación de la UI.
            if (!_attempt.CanBlow)
            {
                return;
            }

            _log.RecordBlowSuccess();
            logView.Show(_log.Entries);
            Play(sounds?.Blow);
            _ = ResolveAsync();
        }

        /// <summary>Anima la convergencia y, al terminar, marca el nivel completado (RF-20).</summary>
        private async Awaitable ResolveAsync()
        {
            try
            {
                await PlayIgnitionAsync();
            }
            catch (OperationCanceledException)
            {
                return; // el panel se destruyó (cambio de escena) a mitad de la animación.
            }

            CompleteLevel();
        }

        /// <summary>Nacimiento del fuego: un barrido de color, guion §4.3.5 E7.</summary>
        private async Awaitable PlayIgnitionAsync()
        {
            // Un único barrido, en una sola dirección: RNF-21 prohíbe destellos de alta
            // frecuencia, y un `Mathf.PingPong` o cualquier oscilación los produciría. Las hojas
            // amontonadas se tiñen de fuego; el sprite de fuego real es arte pendiente (A7).
            const float duration = 0.6f;
            // La hoguera entra como capa sobre la cueva, que sigue sonando, y ya no se va (§8,
            // amb_n1_cueva_fuego): crece en sus segundos sin golpe inicial (RNF-21), y la escena de
            // cierre pide los dos mismos clips.
            if (sounds != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAmbientLayer(sounds.FireAmbient, sounds.FireAmbientFadeSeconds);
            }

            var leaves = Array.ConvertAll(
                Array.FindAll(pieces, piece => piece.Kind == PieceKind.Leaf), piece => piece.GetComponent<Image>());
            var starts = Array.ConvertAll(leaves, image => image != null ? image.color : Color.white);
            var fire = new Color(0.91f, 0.40f, 0.10f);
            var startLighting = lighting != null ? lighting.Progress : 1f;
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                var t = elapsed / duration;
                for (var i = 0; i < leaves.Length; i++)
                {
                    if (leaves[i] != null)
                    {
                        leaves[i].color = Color.Lerp(starts[i], fire, t);
                    }
                }

                // Un solo barrido combinado con el fuego: la iluminación completa llega en la
                // resolución (guion E7), no antes (E4 solo sube un escalón por golpe efectivo).
                lighting?.SetProgress(Mathf.Lerp(startLighting, 1f, t));
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }

            foreach (var leaf in leaves)
            {
                if (leaf != null)
                {
                    leaf.color = fire;
                }
            }

            lighting?.SetProgress(1f);
        }

        /// <summary>
        /// Deja los indicadores listos y encadena a la escena de cierre (RF-20). No confirma la
        /// fase ni desbloquea ni guarda aquí: eso pasa en <c>LevelSummaryController</c>, después
        /// de la escena narrativa (HU-14 paso 6-7) — confirmarlo antes haría que «ya vista»
        /// (<see cref="Game.Scaffolding.NarrativeVisitPolicy"/>) diera verdadero desde la
        /// primerísima vez y el guía se saltara el nombre de la habilidad practicada (CP-07).
        /// </summary>
        private void CompleteLevel()
        {
            if (Runner == null || Runner.Flow.ActiveProfile == null)
            {
                // Mismo criterio que ScreenFlow (Game.UI), sin depender de ese assembly: la
                // escena se abrió sin pasar por Boot, no hay a dónde navegar.
                Debug.LogWarning(
                    "«Level1_Cave» se abrió sin pasar por «Boot»: no hay perfil activo, no se " +
                    "avanza a la escena de cierre.", this);
                return;
            }

            Runner.PendingIndicators = _indicators.Complete();
            Runner.StartNarrative("N1_NacimientoDelFuego");
        }

        private void RefreshBlow()
        {
            // «Por qué no» pedagógico: `CanBlow` de FireAttempt nunca vuelve a falso una vez
            // alcanzado el mínimo (INC-32), así que el botón y su candado tampoco re-bloquean tras
            // un fallo posterior. Lo ganado permanece. Reuniendo no hay botón, así que tampoco candado.
            blowButton.interactable = _attempt.CanBlow;
            if (blowLockedBadge != null)
            {
                blowLockedBadge.SetActive(!IsGathering && !_attempt.CanBlow);
            }
        }

        /// <summary>Sube un escalón por golpe efectivo, sin llegar al máximo (RF-21, guion E4) —
        /// el último tramo hasta la iluminación completa lo da <see cref="PlayIgnitionAsync"/> en
        /// la resolución (E7).</summary>
        private void RefreshLighting() =>
            lighting?.SetProgress((float)_attempt.EffectiveStrikes / (config.MinimumEffectiveStrikes + 1));
    }
}
