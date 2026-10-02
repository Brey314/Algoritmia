using System;
using System.Collections.Generic;
using System.Threading;
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
    /// círculo— y **encender** —la cámara se acerca, las hojas sueltas se funden en el montón visto
    /// desde arriba y aparecen los deslizantes de fuerza y de cercanía de las piedras con «Golpear»
    /// y «Soplar»—. Traduce los clics a <see cref="FireAttempt"/> / <see cref="FireFeedbackLog"/> y
    /// el estado a la UI.
    /// </summary>
    /// <remarks>
    /// Adaptador delgado: **no contiene ninguna regla del juego**. Si está todo reunido lo dice
    /// <see cref="FireArrangement"/>; dónde van las piedras y si chocan, <see cref="StoneSpacing"/>;
    /// los deslizantes representan la hipótesis y no producen efecto hasta «Golpear» (RF-15); el
    /// resultado de cada golpe lo decide <see cref="FireAttempt"/> (T12) y el mensaje
    /// <see cref="FireFeedbackLog"/> (T13). Al converger y accionar «Soplar», prende la llama
    /// cenital sobre el montón y encadena a la escena narrativa de cierre (T15, RF-20).
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

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Opacidad de los deslizantes mientras descansan (encendido). Su ColorTint no se ve: el objetivo del Slider es el contorno del asa y la cara opaca lo tapa, así que se atenúa todo el deslizante.")]
        private float restingSliderAlpha = 0.5f;

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
        [Tooltip("Interfaz que las piezas no deben tapar al aparecer: la tablilla, «Pista» y el botón de pausa. El círculo de reunión se excluye solo.")]
        private RectTransform[] keepClear = Array.Empty<RectTransform>();

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
        [Tooltip("El montón de hojas visto desde arriba (prop_n1_monton_hojas_cenital), hermano de las piezas: reemplaza a las hojas sueltas al armar la fogata y se quema desde el centro al nacer el fuego. Oculto mientras se reúne.")]
        private Image leafPile;

        [SerializeField]
        [Tooltip("La llama cenital (prop_n1_fuego_cenital) sobre el montón: su Animator arranca al soplar y sigue en bucle hasta la escena de cierre. Oculta hasta entonces.")]
        private Animator fireFlame;

        [SerializeField]
        [Tooltip("El quemado del montón (BurnReveal en MontonHojas): al soplar crece despacio desde donde cae la llama hasta N1_Config.BurnExtent y ahí se queda.")]
        private BurnReveal burn;

        [SerializeField]
        [Tooltip("El hilo de humo del montón (fx_n1_humo_nacer), con el pivote en su base: nace en el punto del golpe al converger y ya no se apaga (guion §1.4.3.5, fila E6); al soplar sube hasta la corona de la llama, detrás de ella y encogiéndose. Oculto hasta entonces, por encima del montón y por debajo de las piedras.")]
        private Animator pileSmoke;

        [SerializeField]
        [Tooltip("El rayo de la chispa (#FFE9A8), con el pivote en su cola: sale del punto del fuego y cae en las hojas si el golpe es efectivo, o salta lejos y se apaga en el aire si se pasó de fuerte. Su ancho es el largo del rayo y su alto el grosor. Oculto el resto del tiempo.")]
        private RectTransform spark;

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

        [SerializeField]
        [Tooltip("Papá, el que juega (guion §1.4.3): no se desplaza, hace lo que hace el estudiante —recoge al tomar una pieza, golpea, sopla y se arrodilla ante el fuego—. Puede quedar sin asignar: el nivel se juega igual.")]
        private CharacterRig player;

        [SerializeField]
        [Tooltip("Mamá, la Niña y el Niño, «bien atrás» (guion §1.4.2): miran sin moverse de su sitio. Nunca dentro de «Suelo», que las pruebas recorren pieza a pieza.")]
        private CharacterRig[] family = Array.Empty<CharacterRig>();

        [SerializeField]
        [Tooltip("De la familia, el que no se está quieto (el Niño, Dirección de arte §7.4): observa en vez de reposar.")]
        private CharacterRig restless;

        // Lo que dura una pasada de cada gesto de Papá: el largo de su clip en char_papa.controller
        // (Dirección de arte §13.3). Se cambian con el clip, no para ajustar el juego.
        private const float PickUpSeconds = 1.61f;
        private const float StrikeSeconds = 0.69f;
        private const float EncourageSeconds = 1.035f;
        private const float BlowSeconds = 1.035f;

        // Cuánto dura la chispa de un golpe: son tiempos de un efecto y no parámetros de juego,
        // así que no van a FireLevelConfig. La efectiva dura más con cada golpe (guion §1.4.3.3).
        private const float SparkSeconds = 0.2f;
        private const float DyingSparkSeconds = 0.35f;

        // Hasta dónde vuela el rayo, en fracciones del lado del montón (leafPile: 300 px de suelo,
        // antes del acercamiento). Medido el 30/09/2026 sobre prop_n1_monton_hojas_cenital con las
        // piedras en la muesca efectiva (a ±21 px, 36,6 px de radio dibujado):
        // - el efectivo vuela hacia la mitad de abajo, donde el montón se extiende bajo las piedras.
        //   Por arriba las hojas acaban a 43–90 px del golpe y las piedras ya tapan 39: con un
        //   círculo entero, una de cada tres chispas caería en el suelo de la cueva. Entre 0,205 y
        //   0,22 del lado toda caída es hoja y ninguna es piedra;
        // - el que se pasa de fuerte salta hacia la mitad de arriba: pasa de la última hoja (154,5
        //   px) y se apaga antes de la tablilla (190 px de suelo con la cámara al doble, a 16:9).
        //   Hacia abajo se metería bajo el deslizante de cercanía.
        // Son medidas del arte, no reglas del juego: se rehacen si cambia el dibujo del montón.
        private const float LandingMinReach = 0.205f;
        private const float LandingMaxReach = 0.22f;
        private const float DyingMinReach = 0.52f;
        private const float DyingMaxReach = 0.6f;

        // La corona de la llama cenital, en fracción de su alto sobre el centro: la punta de los
        // dibujos del bucle (fuego_cenital_nivel_1_0095…0168) cae entre 0,22 y 0,32; con 0,25 la
        // punta tapa la base del humo al terminar el encendido (cuadro 0103). Es una medida del
        // arte, no un parámetro del juego: se rehace si cambia el dibujo de la llama.
        private const float FlameCrownFraction = 0.25f;

        // Cuánto se encoge el humo al subir a la corona, como el que acompaña a cada llama en las
        // narrativas (Inventario.md: «a 0,6 de su escala»): a tamaño completo, su remate asomaba por
        // la franja entre la tablilla y el borde de arriba, y se cortaba contra él (captura del
        // 30/09/2026). Con 0,6 el humo acaba bajo el borde de arriba de la tablilla, a 16:9.
        private const float SmokeCrownScale = 0.6f;

        private FireAttempt _attempt;
        private FireFeedbackLog _log;
        private FireIndicatorCollector _indicators;
        private HintPolicy _hints;
        private StoneSpacing _spacing;
        private CancellationTokenSource _playerRest;
        private CancellationTokenSource _sparkAnimation;
        private bool _igniting; // «Soplar» ya se pulsó: el fuego nace y el nivel se cierra al terminar

        /// <summary>El flujo del juego. Sin él (escena abierta sin pasar por Boot) no se navega.</summary>
        internal GameFlowRunner Runner { get; set; }

        /// <summary>
        /// Azar de la dirección y del largo de la chispa: el mismo patrón que
        /// <see cref="FloorScatter.Place"/>. Las pruebas ponen uno con semilla.
        /// </summary>
        internal System.Random SparkRandom { get; set; } = new System.Random();

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
        internal RectTransform[] KeepClear => keepClear;
        internal Image GatherRing => gatherRing;
        internal RectTransform[] Environment => environment;
        internal GameObject[] IgnitionUi => ignitionUi;
        internal Image LeafPile => leafPile;
        internal Animator FireFlame => fireFlame;
        internal BurnReveal Burn => burn;
        internal Animator PileSmoke => pileSmoke;
        internal RectTransform Spark => spark;
        internal FireAttempt Attempt => _attempt;
        internal FireFeedbackLog Log => _log;
        internal FireIndicatorCollector Indicators => _indicators;
        internal StoneSpacing Spacing => _spacing;
        internal CaveLightingController Lighting => lighting;
        internal Button HintButton => hintButton;
        internal FireSounds Sounds => sounds;
        internal CharacterRig Player => player;
        internal CharacterRig[] Family => family;
        internal CharacterRig Restless => restless;

        /// <summary>Dónde cae el último rayo de la chispa, en el espacio del suelo.</summary>
        internal Vector2 SparkLanding { get; private set; }
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

            ScatterPieces();

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
            leafPile.gameObject.SetActive(false);
            pileSmoke.gameObject.SetActive(false);
            spark.gameObject.SetActive(false);
            fireFlame.gameObject.SetActive(false);
            burn.Extent = 0f; // nada quemado hasta que nace el fuego
            foreach (var element in ignitionUi)
            {
                element.SetActive(false);
            }

            RefreshBlow();
            RefreshLighting();

            // La familia espera atrás sin moverse; el Niño, inquieto, observa (§7.4).
            foreach (var member in family)
            {
                if (member != null)
                {
                    member.Play(member == restless ? ActorAction.Observe : ActorAction.Idle);
                }
            }
        }

        /// <summary>
        /// Reparte las piezas por el suelo al azar, fuera de la interfaz y del círculo de reunión
        /// (pedido de Santiago, 22/09/2026): la posición que trae la escena es solo el punto de partida.
        /// </summary>
        private void ScatterPieces()
        {
            Canvas.ForceUpdateCanvases(); // los rects anclados de la interfaz valen desde el primer cuadro
            var blocked = new List<Rect>(Array.ConvertAll(keepClear, InFloorSpace));
            var spot = fireSpot.anchoredPosition;
            var radius = GatherRadius;
            blocked.Add(new Rect(spot.x - radius, spot.y - radius, 2f * radius, 2f * radius));

            // El montón de hojas dibuja fuera de su rect (LeafPile): la huella lleva margen.
            var sizes = Array.ConvertAll(pieces, piece => piece.Width * 1.6f);
            var positions = FloorScatter.Place(Floor.rect, blocked, sizes, new System.Random());
            for (var i = 0; i < pieces.Length; i++)
            {
                pieces[i].MoveTo(positions[i]);
            }
        }

        /// <summary>El rect de un elemento de interfaz en el espacio del suelo, donde viven las posiciones de las piezas.</summary>
        private Rect InFloorSpace(RectTransform ui)
        {
            var corners = new Vector3[4];
            ui.GetWorldCorners(corners);
            var min = Floor.InverseTransformPoint(corners[0]);
            var max = Floor.InverseTransformPoint(corners[2]);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        /// <summary>Una hoja suena mientras se la lleva (clic sostenido, RNF-02); las piedras no.</summary>
        private void Piece_PickedUp(DraggablePiece piece)
        {
            Act(ActorAction.PickUp, PickUpSeconds); // Papá amontona lo que el estudiante toma (guion §1.4.2)
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
        /// acerca al entorno, las hojas sueltas convergen al punto del fuego mientras sobre ellas
        /// entra por fundido el montón visto desde arriba, y las piedras van donde diga el
        /// deslizante; al terminar, las hojas sueltas ya no están y aparece la interfaz del encendido.
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

            // Orden de dibujo del encendido desde el primer cuadro del acercamiento: las hojas que
            // llegan < el montón, que entra por fundido < las piedras. Si las piedras subieran recién
            // al terminar, se verían 0,8 s bajo las hojas (pedido de Santiago, 30/09/2026).
            var pileColor = leafPile.color;
            leafPile.color = new Color(pileColor.r, pileColor.g, pileColor.b, 0f);
            leafPile.rectTransform.SetAsLastSibling();
            RaiseStones();
            leafPile.gameObject.SetActive(true);

            // La tablilla pasa sobre el suelo, como el resto de la interfaz: el humo sube hasta la
            // corona de la llama y taparía su texto (RNF-03). «Por qué no» en la escena: reuniendo,
            // una pieza soltada bajo la tablilla quedaría tapada y, con su texto como raycastTarget,
            // no se podría volver a tomar; el nivel no se terminaría. Es el mismo par de llamadas que
            // el humo en Strike, y por la misma razón: SetSiblingIndex solo calcula mal el destino.
            var tablet = logView.transform;
            tablet.SetAsLastSibling();
            tablet.SetSiblingIndex(Floor.GetSiblingIndex() + 1);

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

                    leafPile.color = new Color(pileColor.r, pileColor.g, pileColor.b, t);
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
                if (pieces[i].Kind == PieceKind.Leaf)
                {
                    pieces[i].gameObject.SetActive(false); // las hojas sueltas ya son el montón
                }
            }

            leafPile.color = pileColor;
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
        /// Dónde acaba cada pieza al encender: las hojas en el punto del fuego —desaparecen bajo
        /// el montón— y las piedras en el centro, a la distancia que marque el deslizante.
        /// </summary>
        private Vector2[] CampfireLayout()
        {
            var spot = fireSpot.anchoredPosition;
            var distance = _spacing.Distance(SelectedSpacing);
            return Array.ConvertAll(pieces, piece => piece.Kind switch
            {
                PieceKind.Silex => spot + new Vector2(-distance / 2f, 0f),
                PieceKind.Pedernal => spot + new Vector2(distance / 2f, 0f),
                _ => spot
            });
        }

        /// <summary>Sube el sílex y el pedernal al final del suelo: se dibujan sobre las hojas y el montón (T26).</summary>
        private void RaiseStones()
        {
            foreach (var piece in pieces)
            {
                if (piece.Kind != PieceKind.Leaf)
                {
                    piece.transform.SetAsLastSibling();
                }
            }
        }

        /// <summary>
        /// El sílex y el pedernal a la distancia de la muesca, uno a cada lado del punto del fuego
        /// (T26). Solo los mueve: el orden de dibujo lo fijó <see cref="RaiseStones"/> al pasar al
        /// encendido, y moverlo aquí subiría las piedras sobre la chispa o la llama con cada
        /// cambio del deslizante.
        /// </summary>
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
                }
                else if (piece.Kind == PieceKind.Pedernal)
                {
                    piece.MoveTo(spot + new Vector2(distance / 2f, 0f));
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
            if (IsGathering || _igniting)
            {
                // Sin fogata no hay qué golpear: el botón ni siquiera está en pantalla. Y con el fuego
                // naciendo los mandos descansan (LockControls): un golpe pondría la chispa sobre la llama.
                return;
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

            // La chispa es un rayo que sale del punto del golpe (pedido de Santiago, 30/09/2026).
            // Con un golpe efectivo cae en las hojas y el brillo dura más con cada golpe. Si se pasó
            // de fuerte con las piedras en su sitio, salta lejos y se apaga en el aire. Con un golpe
            // suave o las piedras mal puestas no hay chispa (guion §1.4.3.3).
            // «Por qué no» otro color ni otro trazo para el golpe de más: es el mismo rayo, más largo
            // y hacia otro lado, que es lo que cuenta el guion; un rojo o un destello lo convertirían
            // en marca de error (CP-02, Dirección de arte §12.3).
            if (outcome.Effective)
            {
                var strikesCounted = Mathf.Min(outcome.EffectiveStrikes, config.MinimumEffectiveStrikes);
                ShowSpark(SparkLandingPoint(intoPile: true), SparkSeconds * strikesCounted);
            }
            else if (outcome.Spacing == SpacingBand.Effective && outcome.Band == ForceBand.TooHard)
            {
                ShowSpark(SparkLandingPoint(intoPile: false), DyingSparkSeconds);
            }

            // Papá golpea siempre. «Por qué no» un gesto de fallo tras un golpe sin chispa: se anima
            // —puño arriba— y vuelve al reposo, porque cabeza gacha u hombros caídos serían la
            // pantalla de derrota en pequeño (Dirección de arte §7.3, CP-02).
            if (outcome.Effective)
            {
                Act(ActorAction.Strike, StrikeSeconds);
                _hints.RegisterSuccessfulAttempt();
                if (outcome.EffectiveStrikes == config.MinimumEffectiveStrikes)
                {
                    _hints.Activate(Step("Soplar")); // la tarea cambia al converger (guion §4.3.6)
                    // El montón pasa a humeante en la convergencia (guion §1.4.3.5, fila E6) y ya
                    // no deja de humear: lo ganado no se retira por un fallo posterior (INC-32).
                    // Entre el montón y las piedras, que ya están puestas (RaiseStones las subió al
                    // pasar al encendido). Al último primero: si el humo empezaba **antes** que el
                    // montón en la jerarquía, calcular el índice de destino sin moverlo antes salía
                    // mal — quitarlo de en medio corre los índices de detrás una posición, y el
                    // destino ya calculado quedaba corto.
                    pileSmoke.transform.SetAsLastSibling();
                    pileSmoke.transform.SetSiblingIndex(leafPile.transform.GetSiblingIndex() + 1);
                    pileSmoke.gameObject.SetActive(true);
                }
            }
            else
            {
                Act(ActorAction.Strike, StrikeSeconds, ActorAction.Encourage, EncourageSeconds);
                _log.RecordGuide(_hints.RegisterFailedAttempt()); // pista solo al tercer fallo seguido
            }

            logView.Show(_log.Entries);
            RefreshBlow();
            RefreshLighting();
        }

        /// <summary>
        /// Dónde cae la chispa. La dirección es una media vuelta al azar: la de abajo si cae en las
        /// hojas, la de arriba si salta lejos. El largo es un valor al azar dentro de su franja.
        /// Se sortea primero la dirección y después el largo.
        /// </summary>
        private Vector2 SparkLandingPoint(bool intoPile)
        {
            var angle = Mathf.PI * (float)SparkRandom.NextDouble();
            var direction = new Vector2(Mathf.Cos(angle), intoPile ? -Mathf.Sin(angle) : Mathf.Sin(angle));
            var reach = intoPile
                ? Mathf.Lerp(LandingMinReach, LandingMaxReach, (float)SparkRandom.NextDouble())
                : Mathf.Lerp(DyingMinReach, DyingMaxReach, (float)SparkRandom.NextDouble());
            return fireSpot.anchoredPosition + direction * (reach * leafPile.rectTransform.rect.width);
        }

        /// <summary>Lanza el rayo de la chispa hasta <paramref name="landing"/> en <paramref name="seconds"/>; un golpe nuevo cancela el anterior.</summary>
        private void ShowSpark(Vector2 landing, float seconds)
        {
#if UNITY_INCLUDE_TESTS
            SparkLanding = landing;
#endif
            _sparkAnimation?.Cancel();
            _sparkAnimation = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            _ = PlaySparkAsync(landing, seconds, _sparkAnimation.Token);
        }

        /// <summary>
        /// El rayo sale del punto del golpe y cae en <paramref name="landing"/>: la cabeza llega a
        /// la caída en la primera mitad del tiempo y la cola la alcanza en la segunda, y ahí se
        /// apaga. Es un único barrido en una sola dirección, sin oscilar la escala ni el alfa
        /// (RNF-21). Va por encima de las piedras, que subieron al pasar al encendido, y por debajo
        /// de la llama, que se pone última al soplar (RF-20).
        /// </summary>
        private async Awaitable PlaySparkAsync(Vector2 landing, float seconds, CancellationToken token)
        {
            var origin = fireSpot.anchoredPosition;
            var path = landing - origin;
            spark.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(path.y, path.x) * Mathf.Rad2Deg);
            spark.SetAsLastSibling();
            spark.gameObject.SetActive(true);

            try
            {
                for (var elapsed = 0f; elapsed < seconds; elapsed += Time.deltaTime)
                {
                    var t = elapsed / seconds;
                    var head = Mathf.Clamp01(2f * t);
                    var tail = Mathf.Clamp01(2f * t - 1f);
                    spark.anchoredPosition = origin + path * tail;
                    spark.sizeDelta = new Vector2(path.magnitude * (head - tail), spark.sizeDelta.y);
                    await Awaitable.NextFrameAsync(token);
                }
            }
            catch (OperationCanceledException)
            {
                return; // un golpe nuevo canceló esta chispa, o el panel se destruyó
            }

            spark.gameObject.SetActive(false);
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
            // un clic simulado en pruebas no pasa por esa comprobación de la UI. Y una sola vez:
            // `CanBlow` no vuelve a falso (INC-32), así que sin `_igniting` cada «Soplar» durante
            // el encendido lanzaría otro ResolveAsync.
            if (!_attempt.CanBlow || _igniting)
            {
                return;
            }

            _igniting = true;
            LockControls();
            _log.RecordBlowSuccess();
            logView.Show(_log.Entries);
            Play(sounds?.Blow);
            // Sopla y se queda arrodillado mirando nacer el fuego (guion §1.4.4); la celebración
            // es de la escena de cierre.
            Act(ActorAction.Blow, BlowSeconds, ActorAction.Kneel);
            _ = ResolveAsync();
        }

        /// <summary>
        /// Desde «Soplar» y hasta que termina el nivel los mandos del panel descansan (RNF-21): un
        /// segundo «Soplar» relanzaría el encendido —el quemado volvería a cero y el cierre correría
        /// dos veces— y un «Golpear» pondría la chispa sobre la llama y bajaría la luz un cuadro, de
        /// la subida del encendido al escalón del golpe. No es un castigo (CP-02): el estudiante ya
        /// hizo lo que se le pedía y solo mira nacer el fuego; por eso tampoco sale el candado de
        /// «Soplar», que dice «todavía no», no «ya está».
        /// </summary>
        private void LockControls()
        {
            blowButton.interactable = false;
            strikeButton.interactable = false;
            Rest(forceSlider);
            Rest(spacingSlider);
        }

        /// <summary>
        /// Deja el deslizante sin respuesta y a la vista como tal (DEF-W5R-02). <c>interactable</c>
        /// solo aplica el <c>ColorTint</c> a su gráfico objetivo —el contorno del asa—, y la cara que
        /// el niño ve va encima y opaca, así que sin esto «no responde» parecería «sigue activo».
        /// </summary>
        private void Rest(Slider slider)
        {
            slider.interactable = false;
            if (!slider.TryGetComponent<CanvasGroup>(out var group))
            {
                group = slider.gameObject.AddComponent<CanvasGroup>();
            }

            group.alpha = restingSliderAlpha;
        }

        /// <summary>
        /// Papá hace <paramref name="action"/> lo que dura su clip y pasa a <paramref name="then"/>,
        /// que se queda puesto; con <paramref name="thenSeconds"/> positivo también eso termina y
        /// vuelve al reposo. Un gesto nuevo cancela lo pendiente del anterior. Sin Papá en la
        /// escena no hace nada: el nivel se juega igual.
        /// </summary>
        private void Act(ActorAction action, float seconds, ActorAction then = ActorAction.Idle, float thenSeconds = 0f)
        {
            if (player == null)
            {
                return;
            }

            _playerRest?.Cancel();
            _playerRest = null;
            player.PlayFor(action, seconds, then);
            if (thenSeconds > 0f)
            {
                _playerRest = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                _ = RestAfterAsync(seconds + thenSeconds, _playerRest.Token);
            }
        }

        private async Awaitable RestAfterAsync(float seconds, CancellationToken token)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(seconds, token);
            }
            catch (OperationCanceledException)
            {
                return; // otro gesto ocupó su lugar, o el panel se destruyó
            }

            player.Play(ActorAction.Idle);
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

        /// <summary>
        /// Nacimiento del fuego (guion §4.3.5 E7): la llama cenital prende sobre el montón y las
        /// hojas se queman desde el centro hacia fuera en <see cref="FireLevelConfig.IgnitionSeconds"/>;
        /// el humo sube hasta la corona de la llama, detrás de ella, encogiéndose.
        /// </summary>
        private async Awaitable PlayIgnitionAsync()
        {
            // Un único barrido, en una sola dirección: RNF-21 prohíbe destellos de alta
            // frecuencia, y un `Mathf.PingPong` o cualquier oscilación los produciría.
            var duration = Mathf.Max(config.IgnitionSeconds, 0f);
            // La hoguera entra como capa sobre la cueva, que sigue sonando, y ya no se va (§8,
            // amb_n1_cueva_fuego): crece en sus segundos sin golpe inicial (RNF-21), y la escena de
            // cierre pide los dos mismos clips.
            if (sounds != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAmbientLayer(sounds.FireAmbient, sounds.FireAmbientFadeSeconds);
            }

            // La llama es el clip prop_n1_fuego_cenital: el Animator arranca en su primer cuadro al
            // activarse y sigue en bucle hasta el cambio de escena. Va encima de todo, piedras incluidas.
            fireFlame.transform.SetAsLastSibling();
            fireFlame.gameObject.SetActive(true);

            // El humo sube con el fuego hasta la corona de la llama, detrás de ella, y se encoge al
            // subir (pedido de Santiago, 30/09/2026). Va con el mismo t que el quemado: un solo
            // barrido, sin volver (RNF-21). Arranca de donde nació, en el punto del golpe y a su
            // escala.
            var smoke = (RectTransform)pileSmoke.transform;
            var flame = (RectTransform)fireFlame.transform;
            var smokeFrom = smoke.anchoredPosition;
            var crown = flame.anchoredPosition + new Vector2(0f, FlameCrownFraction * flame.rect.height);
            var smokeScaleFrom = smoke.localScale;
            var smokeScaleAtCrown = smokeScaleFrom * SmokeCrownScale;

            // Las hojas se queman desde donde cayó la llama, despacio y solo hasta donde diga la
            // configuración; después el quemado se queda quieto. Un solo crecimiento, sin oscilar (RNF-21).
            var startLighting = lighting != null ? lighting.Progress : 1f;
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                var t = elapsed / duration;
                burn.Extent = config.BurnExtent * t;
                smoke.anchoredPosition = Vector2.Lerp(smokeFrom, crown, t);
                smoke.localScale = Vector3.Lerp(smokeScaleFrom, smokeScaleAtCrown, t);

                // Un solo barrido combinado con el fuego: la iluminación completa llega en la
                // resolución (guion E7), no antes (E4 solo sube un escalón por golpe efectivo).
                lighting?.SetProgress(Mathf.Lerp(startLighting, 1f, t));
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }

            burn.Extent = config.BurnExtent;
            smoke.anchoredPosition = crown;
            smoke.localScale = smokeScaleAtCrown;
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
