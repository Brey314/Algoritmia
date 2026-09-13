using System.Collections.Generic;
using Game.Core;
using Game.Scaffolding;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// La escena de la fase 2 del Nivel 2 — el área de trabajo (RF-27, RF-28, RF-29). Reparte las
    /// seis piezas sobre el entorno, traduce clic, botón y clic sostenido a
    /// <see cref="AssemblySequence"/> y pinta lo que la regla devuelve.
    /// </summary>
    /// <remarks>
    /// Adaptador delgado, igual que <see cref="ForestSceneController"/>: el orden del ensamblaje y
    /// sus mensajes viven en <see cref="AssemblySequence"/>, C# plano probado en EditMode. Aquí
    /// solo hay geometría —sobre qué cayó la pieza— y pintura.
    ///
    /// **El taller es el mismo entorno del bosque, con el encuadre en que termina la escena 2.3**
    /// (<c>Camara_Narrativa_N2.md</c> §5.6): de la narrativa al juego no hay salto. Las piezas
    /// **cuelgan de la ilustración** en fracciones de ella, como los objetos pintados de la
    /// narrativa, así que acompañan el encuadre y sobreviven a cualquier resolución del arte:
    /// sustituir el archivo basta (RNF-23). Y se instancian del asset (RNF-18, CT-05): mover una
    /// pieza es tocar <see cref="AssemblyContent"/>, no la escena.
    ///
    /// Los arrastres son **pulsar y soltar** con <see cref="CargoHandle"/>, nunca el arrastre de
    /// uGUI, por el mismo motivo que la caja del bosque (RNF-02, CT-06).
    /// </remarks>
    public class WorkshopSceneController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Contenido de la fase 2: piezas, arte de la carretilla, encuadres y mensajes.")]
        private AssemblyContent config;

        [SerializeField]
        [Tooltip("Contenido del guía del Nivel 2. La fase 2 activa su segunda tarea, «Construir».")]
        private GuideContent guide;

        [SerializeField]
        [Tooltip("El mundo: el entorno y todo lo que cuelga de él. Es lo que la cámara empuja al terminar; las tablillas quedan fuera.")]
        private RectTransform world;

        [SerializeField]
        [Tooltip("El entorno. Cubre la pantalla sin deformarse con el encuadre del asset; las piezas cuelgan de él (RNF-23).")]
        private Image environment;

        [SerializeField]
        [Tooltip("Pieza modelo, hija del entorno. Se clona una vez por entrada del catálogo. Permanece inactiva.")]
        private RectTransform pieceTemplate;

        [SerializeField]
        [Tooltip("La carretilla en construcción, hija del entorno. Apagada hasta que se forma el eje; luego muestra cada estado.")]
        private Image assemblyImage;

        [SerializeField]
        [Tooltip("«Mecanizar»: habilitado solo con un tronco corto resaltado (RF-28).")]
        private Button machineButton;

        [SerializeField]
        [Tooltip("Tablilla «Aún no» con candado sobre «Mecanizar»: el segundo indicador de RNF-19 cuando el botón no está disponible.")]
        private GameObject machineLockedBadge;

        [SerializeField]
        [Tooltip("Botón de pista: repite la instrucción vigente sin gastar un intento (RF-13).")]
        private Button helpButton;

        [SerializeField]
        [Tooltip("Lo que dice el guía ahora: instrucción, respuesta al intento o pista (RF-11, RF-13, RF-17).")]
        private Text messageLabel;

        [SerializeField]
        [Tooltip("Icono que acompaña a la frase: es el segundo canal de RNF-19.")]
        private Image messageIcon;

        [SerializeField]
        [Tooltip("Icono del paso aceptado: forma cerrada.")]
        private Sprite acceptedIcon;

        [SerializeField]
        [Tooltip("Icono del paso devuelto: forma distinta de la anterior, no solo otro color (RNF-19).")]
        private Sprite rejectedIcon;

        [SerializeField]
        [Tooltip("Icono de la instrucción y la pista. Vacío deja el hueco sin icono.")]
        private Sprite helpIcon;

        [SerializeField] private Color acceptedColor = new Color(0.20f, 0.40f, 0.22f);
        [SerializeField] private Color rejectedColor = new Color(0.60f, 0.36f, 0.10f);
        [SerializeField] private Color helpColor = new Color(0.24f, 0.30f, 0.44f);

        [SerializeField]
        [Tooltip("Cuánto crece el tronco resaltado (1.1 = un 10 %). Acompaña al contorno: dos indicadores (RNF-19).")]
        private float selectedScale = 1.1f;

        private static readonly PhaseId Phase2 = new PhaseId(LevelId.Wheel, 2);

        private readonly Dictionary<WorkshopPiece, (RectTransform Rect, Image Image, Outline Outline)> _pieces =
            new Dictionary<WorkshopPiece, (RectTransform, Image, Outline)>();

        private AssemblySequence _assembly;
        private HintPolicy _hints;
        private Canvas _canvas;
        private WorkshopPiece? _held;

        /// <summary>El flujo del juego. Lo pone <c>Boot</c>; una prueba puede inyectar otro.</summary>
        internal GameFlowRunner Runner { get; set; }

        /// <summary>Si el empuje de cámara sobre la carretilla terminada está en curso (RF-29).</summary>
        internal bool IsCompleting { get; private set; }

#if UNITY_INCLUDE_TESTS
        internal IReadOnlyDictionary<WorkshopPiece, (RectTransform Rect, Image Image, Outline Outline)> Pieces => _pieces;
        internal AssemblySequence Assembly => _assembly;
        internal RectTransform World => world;
        internal Image Environment => environment;
        internal Image AssemblyImage => assemblyImage;
        internal Button MachineButton => machineButton;
        internal GameObject MachineLockedBadge => machineLockedBadge;
        internal Button HelpButton => helpButton;
        internal Text MessageLabel => messageLabel;
        internal Image MessageIcon => messageIcon;
        internal AssemblyContent Config => config;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            _assembly = new AssemblySequence(config);
            _hints = new HintPolicy(StepForPhase2());
            _canvas = world.GetComponentInParent<Canvas>();

            // El entorno primero: Apply deja al elemento con el tamaño nativo de la imagen y lo
            // escala, y es sobre ese tamaño sobre el que se miden las piezas que cuelgan de él.
            environment.enabled = environment.sprite != null;
            if (environment.sprite != null)
            {
                environment.preserveAspect = false;
                IllustrationFraming.Apply(environment.rectTransform, environment.sprite.rect.size, config.PlayFraming);
            }

            pieceTemplate.gameObject.SetActive(false);
            foreach (var placement in config.Pieces)
            {
                Spawn(placement);
            }

            Hang(assemblyImage.rectTransform, config.AssemblyPosition, config.AssemblySize);
            assemblyImage.preserveAspect = true;
            assemblyImage.raycastTarget = false;
            assemblyImage.enabled = false;

            machineButton.onClick.AddListener(Machine);
            RefreshMachine();

            // Pedir ayuda no es un intento: repite la instrucción vigente y no toca ningún
            // contador (RF-13, CP-06). La regla vive en HintPolicy; aquí solo se pulsa.
            helpButton.onClick.AddListener(() => Show(_hints.RequestHelp(), helpIcon, helpColor));

            Show(_hints.RequestHelp(), helpIcon, helpColor);
        }

        /// <summary>
        /// Cuelga un elemento de la ilustración: casilla cuadrada centrada en una fracción del
        /// entorno, medida en fracción de su alto. La misma convención que <c>NarrativeProp</c>.
        /// </summary>
        private void Hang(RectTransform rect, Vector2 position, float size)
        {
            rect.SetParent(environment.rectTransform, false);
            rect.anchorMin = position;
            rect.anchorMax = position;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            var imageHeight = environment.sprite != null ? environment.sprite.rect.height : environment.rectTransform.rect.height;
            rect.sizeDelta = Vector2.one * (imageHeight * size);
        }

        /// <summary>
        /// Deja una pieza sobre el entorno, en el punto que le da el asset, y la cablea según lo que
        /// es: los troncos cortos se pulsan, el eje, la tabla y la caja se sostienen, y la
        /// herramienta solo está (guion §6.2.2).
        /// </summary>
        /// <remarks>
        /// El modelo trae botón y asa; a cada pieza se le quita lo que no usa en vez de tener tres
        /// modelos. Así la prueba del mapa de controles puede afirmar que **solo** tres objetos
        /// responden al clic sostenido y que todo lo demás interactivo es un botón (RNF-02).
        /// </remarks>
        private void Spawn(WorkshopPiecePlacement placement)
        {
            var rect = Instantiate(pieceTemplate, environment.rectTransform);
            // El nombre hace único el camino de jerarquía: sin él ninguna prueba automatizada
            // puede señalar una pieza concreta.
            rect.name = $"Pieza_{placement.Piece}";
            Hang(rect, placement.Position, placement.Size);

            var image = rect.GetComponent<Image>();
            image.sprite = placement.Art;
            image.preserveAspect = true;
            image.color = Color.white;

            var outline = rect.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }

            var button = rect.GetComponent<Button>();
            var handle = rect.GetComponent<CargoHandle>();
            var piece = placement.Piece;

            if (AssemblySequence.IsShortLog(piece))
            {
                button.onClick.AddListener(() => Select(piece));
                Destroy(handle);
            }
            else if (AssemblySequence.IsDraggable(piece))
            {
                handle.Taken += () => Take(piece);
                handle.Released += () => Release(piece);
                Destroy(button);
            }
            else
            {
                image.raycastTarget = false;
                Destroy(button);
                Destroy(handle);
            }

            rect.gameObject.SetActive(true);
            _pieces[piece] = (rect, image, outline);
        }

        /// <summary>Clic simple sobre un tronco corto: queda resaltado (guion §6.2.2 paso 1).</summary>
        internal void Select(WorkshopPiece piece)
        {
            var outcome = _assembly.Select(piece);
            if (!outcome.Accepted)
            {
                return;
            }

            Highlight(_assembly.Selected);
            RefreshMachine();
            Show(outcome.Message, helpIcon, helpColor);
        }

        /// <summary>«Mecanizar»: el tronco resaltado pasa a ser rueda (RF-28).</summary>
        internal void Machine()
        {
            var drilled = _assembly.Selected;
            var outcome = _assembly.Machine();

            if (!outcome.Accepted)
            {
                Show(outcome.Message, rejectedIcon, rejectedColor);
                return;
            }

            if (drilled.HasValue && _pieces.TryGetValue(drilled.Value, out var entry))
            {
                // La rueda **es** el mismo tronco con el agujero: se cambia la ilustración en su
                // sitio, sin quitar nada del suelo (RF-28, CP-02).
                entry.Image.sprite = config.DrilledWheelArt != null ? config.DrilledWheelArt : entry.Image.sprite;
            }

            Highlight(null);
            RefreshMachine();
            _hints.RegisterSuccessfulAttempt();
            Show(outcome.Message, acceptedIcon, acceptedColor);
        }

        /// <summary>
        /// Resalta un tronco con **dos** indicadores: contorno y tamaño. Ninguno es solo color,
        /// así que la selección se distingue sin percibirlo (RNF-19).
        /// </summary>
        private void Highlight(WorkshopPiece? selected)
        {
            foreach (var (piece, entry) in _pieces)
            {
                var on = selected.HasValue && selected.Value == piece;
                if (entry.Outline != null)
                {
                    entry.Outline.enabled = on;
                }

                entry.Rect.localScale = Vector3.one * (on ? selectedScale : 1f);
            }
        }

        /// <summary>
        /// «Mecanizar» refleja su habilitación con el botón **y** con la tablilla «Aún no»:
        /// atenuar el color no basta (RNF-19). Misma anatomía que «Soplar» en el Nivel 1.
        /// </summary>
        private void RefreshMachine()
        {
            machineButton.interactable = _assembly.CanMachine;
            if (machineLockedBadge != null)
            {
                machineLockedBadge.SetActive(!_assembly.CanMachine);
            }
        }

        // --- los arrastres (pasos 4–6) ------------------------------------------------------------

        /// <summary>El clic sostenido sobre el eje, la tabla o la caja.</summary>
        internal void Take(WorkshopPiece piece)
        {
            if (IsCompleting || !AssemblySequence.IsDraggable(piece) || !_pieces.TryGetValue(piece, out var entry))
            {
                return;
            }

            _held = piece;
            // Mientras se sostiene, la pieza pasa por encima de todo lo demás.
            entry.Rect.SetAsLastSibling();
        }

        /// <summary>La pieza sigue al cursor mientras se sostiene el clic; si no, no se mueve.</summary>
        internal void DragTo(Vector2 screenPoint)
        {
            if (!_held.HasValue)
            {
                return;
            }

            var rect = _pieces[_held.Value].Rect;
            rect.anchoredPosition = ToAnchored(rect, screenPoint);
        }

        /// <summary>
        /// Soltar el clic. Sobre el lugar de armado y en secuencia, la pieza se integra a la
        /// carretilla; si no, **vuelve a su sitio** y el guía dice qué falta antes (guion §6.2.2,
        /// CP-06). Nada de lo ya hecho cambia (CP-02).
        /// </summary>
        internal void Release(WorkshopPiece piece)
        {
            if (!_held.HasValue || _held.Value != piece)
            {
                return;
            }

            _held = null;
            var rect = _pieces[piece].Rect;
            var over = OverAssembly(piece, rect);
            var outcome = _assembly.Place(piece, over);

            if (outcome.Accepted)
            {
                rect.gameObject.SetActive(false);
                ShowAssembly(piece);
                _hints.RegisterSuccessfulAttempt();
                Show(outcome.Message, acceptedIcon, acceptedColor);

                if (_assembly.IsComplete)
                {
                    _ = CompleteAsync();
                }

                return;
            }

            // La pieza vuelve a donde estaba: su ancla es su sitio en el entorno.
            rect.anchoredPosition = Vector2.zero;

            if (!over)
            {
                // Soltar lejos del lugar de armado no es un razonamiento fallido: se describe y
                // no cuenta para la pista.
                Show(outcome.Message, helpIcon, helpColor);
                return;
            }

            var hint = _hints.RegisterFailedAttempt();
            if (hint != null)
            {
                Show(hint, helpIcon, helpColor);
                return;
            }

            Show(outcome.Message, rejectedIcon, rejectedColor);
        }

        /// <summary>
        /// «El lugar de armado» son las piezas que forman —o van a formar— la carretilla, no una
        /// zona fija: el eje se acerca **a las ruedas** (guion §6.2.2 paso 4) y la tabla y la caja
        /// se sueltan sobre el conjunto una vez existe, o sobre los troncos si todavía no.
        /// </summary>
        /// <remarks>
        /// Sin zona fija ninguna pieza en reposo está «encima» de nada: un clic sin mover no
        /// coloca nada solo. Y soltar el eje sobre un tronco **sin perforar** es exactamente el
        /// intento fuera de orden que el guion describe, con su mensaje.
        /// </remarks>
        private bool OverAssembly(WorkshopPiece piece, RectTransform rect)
        {
            var box = EnPantalla(rect);
            if (piece != WorkshopPiece.LongLog && assemblyImage.enabled)
            {
                return box.Overlaps(EnPantalla(assemblyImage.rectTransform));
            }

            foreach (var (other, entry) in _pieces)
            {
                var isTarget = AssemblySequence.IsShortLog(other)
                               || (piece != WorkshopPiece.LongLog && other == WorkshopPiece.LongLog);
                if (other != piece && isTarget && entry.Rect.gameObject.activeSelf
                    && box.Overlaps(EnPantalla(entry.Rect)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// La carretilla crece literalmente sobre lo anterior: cada paso muestra el estado
        /// siguiente del mismo objeto, en el sitio de las ruedas. Al formar el eje las dos ruedas
        /// y el tronco largo dejan el suelo y pasan a ser el conjunto.
        /// </summary>
        private void ShowAssembly(WorkshopPiece placed)
        {
            switch (placed)
            {
                case WorkshopPiece.LongLog:
                    foreach (var (piece, entry) in _pieces)
                    {
                        if (AssemblySequence.IsShortLog(piece))
                        {
                            entry.Rect.gameObject.SetActive(false);
                        }
                    }

                    assemblyImage.sprite = config.AxleArt;
                    break;
                case WorkshopPiece.Plank:
                    assemblyImage.sprite = config.PlankArt;
                    break;
                default:
                    assemblyImage.sprite = config.CompleteArt;
                    break;
            }

            assemblyImage.enabled = assemblyImage.sprite != null;
        }

        /// <summary>
        /// El cierre de la fase (<c>Camara_Narrativa_N2.md</c> §5.6): un empuje de cámara sobre
        /// la carretilla terminada que rima con el cierre del bosque, y al acabar la fase 2 queda
        /// confirmada.
        /// </summary>
        /// <remarks>
        /// Se escala el mundo, no la ilustración, con el pivote que deja el foco de destino en el
        /// centro de la pantalla: es la misma mecánica del acopio completo del bosque. Es una
        /// interpolación continua y **nada más cambia**: ningún gráfico se apaga ni parpadea
        /// (RNF-21).
        /// </remarks>
        private async Awaitable CompleteAsync()
        {
            IsCompleting = true;
            var seconds = Mathf.Max(config.CompletionSeconds, 0f);
            var elapsed = 0f;
            var (pivot, zoom) = IllustrationFraming.ScaleAbout(
                environment.sprite != null ? environment.sprite.rect.size : Vector2.zero,
                world.rect.size, config.PlayFraming, config.CompletionFraming);
            world.pivot = pivot;

            try
            {
                do
                {
                    elapsed += Time.deltaTime;
                    var t = seconds > 0f ? Mathf.Clamp01(elapsed / seconds) : 1f;
                    world.localScale = Vector3.one * Mathf.Lerp(1f, zoom, Mathf.SmoothStep(0f, 1f, t));
                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                } while (elapsed < seconds);
            }
            catch (System.OperationCanceledException)
            {
                return; // La escena se descargó a mitad del empuje: no hay nada que confirmar.
            }

            IsCompleting = false;
            ConfirmPhase2AndLeave();
        }

        /// <summary>
        /// Confirma y guarda la fase 2 (RF-04) y sale a la escena narrativa que declara el asset.
        /// </summary>
        /// <remarks>
        /// Sin <see cref="GameFlowRunner"/> —la escena se abrió suelta, sin pasar por <c>Boot</c>—
        /// avisa y se queda. Con él, si la secuencia de cierre no existe se cae al menú de
        /// niveles antes que dejar al estudiante en una pantalla sin salida (RNF-13).
        /// </remarks>
        private void ConfirmPhase2AndLeave()
        {
            if (Runner == null)
            {
                Debug.LogWarning(
                    $"«{gameObject.scene.name}» se abrió sin pasar por «Boot»: no hay " +
                    $"{nameof(GameFlowRunner)}, así que la carretilla terminada no confirma la fase ni navega.", this);
                return;
            }

            var profile = Runner.Flow.ActiveProfile;
            if (profile != null)
            {
                // W15 emitirá los cuatro indicadores reales de la fase; hasta entonces se confirma
                // con los de una fase sin medir. Lo que importa es que lo aprobado quede en disco
                // antes de salir (RF-04, RNF-14).
                profile.ConfirmPhase(Phase2, default);
                Runner.Session.SaveActive();
            }

            if (!Runner.StartNarrative(config.ClosingSequenceId))
            {
                Runner.GoTo(GameState.LevelSelect);
            }
        }

        /// <summary>
        /// **`Mouse.current` del Input System nuevo, nunca la clase `Input` legada** (CT-06). Sin
        /// ratón —o en batchmode— no hay nada que seguir.
        /// </summary>
        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            DragTo(mouse.position.ReadValue());
        }

        /// <summary>El <c>anchoredPosition</c> que deja el pivote del elemento en ese punto de pantalla.</summary>
        private Vector2 ToAnchored(RectTransform element, Vector2 screenPoint)
        {
            var parent = (RectTransform)element.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, UiCamera, out var local);
            var anchor = parent.rect.min + Vector2.Scale(parent.rect.size, element.anchorMin);
            return local - anchor;
        }

        private Camera UiCamera => _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? _canvas.worldCamera
            : null;

        /// <summary>La caja del elemento en píxeles de pantalla, envuelta por mínimos y máximos de sus esquinas.</summary>
        private static Rect EnPantalla(RectTransform rect)
        {
            var esquinas = new Vector3[4];
            rect.GetWorldCorners(esquinas);

            var minX = Mathf.Min(esquinas[0].x, esquinas[1].x, esquinas[2].x, esquinas[3].x);
            var minY = Mathf.Min(esquinas[0].y, esquinas[1].y, esquinas[2].y, esquinas[3].y);
            var maxX = Mathf.Max(esquinas[0].x, esquinas[1].x, esquinas[2].x, esquinas[3].x);
            var maxY = Mathf.Max(esquinas[0].y, esquinas[1].y, esquinas[2].y, esquinas[3].y);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Pinta la frase con sus **dos** indicadores: el icono dice lo mismo que el color (RNF-19).
        /// El color va en el icono y nunca en la tablilla ni en la frase (RNF-20).
        /// </summary>
        private void Show(string message, Sprite icon, Color color)
        {
            messageLabel.text = message;
            messageIcon.sprite = icon;
            messageIcon.color = color;
            messageIcon.enabled = icon != null;
        }

        /// <summary>La tarea del guía que corresponde a la fase 2, «Construir». El mapeo fase↔tarea lo resuelve el nivel.</summary>
        private GuideStep StepForPhase2() =>
            guide != null && guide.Steps.Length >= Phase2.Phase ? guide.Steps[Phase2.Phase - 1] : null;
    }
}
