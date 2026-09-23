using System.Collections.Generic;
using System.Linq;
using Game.Audio;
using Game.Core;
using Game.Scaffolding;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// La escena de la fase 3 del Nivel 2 — el laberinto (RF-30..RF-34). Pantalla dividida
    /// según el **mockup 10 · Nivel 2 · laberinto · bloques encajables**: a la izquierda el claro
    /// en vista superior con la carretilla, el refugio y los obstáculos; a la
    /// derecha «Tu secuencia» (los bloques encajados, con su contador o su lado), el cajón
    /// «Bloques» con la paleta, y «Ejecutar».
    /// </summary>
    /// <remarks>
    /// Adaptador delgado, como <see cref="WorkshopSceneController"/>: la matriz es
    /// <see cref="MazeGrid"/>, la secuencia <see cref="BlockSequence"/> y el recorrido
    /// <see cref="SequenceExecutor"/>, todo C# plano probado en EditMode. Aquí solo hay
    /// geometría —sobre qué cayó un bloque, dónde se pinta una casilla— y pintura.
    ///
    /// **El entorno se ajusta solo.** La ilustración se escala para caber entera en el panel
    /// izquierdo, con su proporción, y la matriz de 16 × 11 cuelga de ella en fracciones del
    /// asset (<see cref="MazeLayout.BoardMin"/>/<see cref="MazeLayout.BoardMax"/>, el borde
    /// exterior del seto): el boceto de hoy y el arte definitivo se ven igual y **sustituir el
    /// archivo basta** (RNF-23). No se usa el «cubrir» de las otras escenas porque recortaría
    /// los setos, y los setos son el laberinto.
    ///
    /// **Cuando los bloques se acumulan se comprimen**: desaparecen el contador y el lado y queda
    /// el rótulo con una flecha «→» que despliega ese bloque para editarlo; uno desplegado a la
    /// vez. Los bloques se **toman con clic sostenido y se sueltan con clic** (<see cref="CargoHandle"/>,
    /// nunca el arrastre de uGUI — RNF-02, CT-06): de la paleta a la secuencia se engancha donde
    /// cae; de la secuencia hacia fuera, se retira (RF-34).
    /// </remarks>
    public class MazeSceneController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Trazado, arte, tiempos y mensajes de la fase 3.")]
        private MazeLayout layout;

        [SerializeField]
        [Tooltip("Contenido del guía del Nivel 2. La fase 3 activa su tercera tarea, «Programar».")]
        private GuideContent guide;

        [SerializeField]
        [Tooltip("Sonidos del Nivel 2. El bosque de día de fondo y la carretilla mientras recorre la secuencia; vacío = silencio.")]
        private WheelSounds sounds;

        [SerializeField]
        [Tooltip("El entorno cenital. Cabe entero en su panel sin deformarse; la matriz cuelga de él (RNF-23).")]
        private Image environment;

        [SerializeField]
        [Tooltip("Casilla modelo, hija del entorno. Se clona por obstáculo y para el refugio. Permanece inactiva.")]
        private Image cellTemplate;

        [SerializeField]
        [Tooltip("La carretilla, hija del entorno. Su rotación es su orientación: siempre visible (INC-33).")]
        private RectTransform cart;

        [SerializeField]
        [Tooltip("Lista vertical donde se enganchan los bloques. Es también la zona donde soltarlos.")]
        private RectTransform sequenceList;

        [SerializeField]
        [Tooltip("Ventana de la lista: recorta lo que no cabe y es la zona válida para soltar. La lista se mueve dentro de ella.")]
        private RectTransform sequenceViewport;

        [SerializeField]
        [Tooltip("«Suelta un bloque aquí»: la casilla punteada al final de la lista.")]
        private RectTransform dropZone;

        [SerializeField]
        [Tooltip("Sube la lista una página. Solo aparece cuando la secuencia no cabe: el desplazamiento es por clic, nunca rueda ni arrastre (RNF-02, CT-06).")]
        private Button scrollUpButton;

        [SerializeField]
        [Tooltip("Baja la lista una página.")]
        private Button scrollDownButton;

        [SerializeField]
        [Tooltip("Cajón «Bloques»: el botón que lo abre y cierra.")]
        private Button paletteToggle;

        [SerializeField]
        [Tooltip("La flecha del cajón: arriba cerrado, abajo abierto.")]
        private RectTransform paletteArrow;

        [SerializeField]
        [Tooltip("Contenido del cajón: la paleta con los tres bloques. Cerrado al empezar, como en el mockup.")]
        private GameObject paletteBody;

        [SerializeField]
        [Tooltip("Fila de la paleta donde viven «Avanzar» y «Girar».")]
        private RectTransform paletteRowA;

        [SerializeField]
        [Tooltip("Fila de la paleta donde vive «Retroceder».")]
        private RectTransform paletteRowB;

        [SerializeField]
        [Tooltip("Bloque modelo con todas sus partes (marco, muesca, pestaña, rombo, rótulo, contador, lado, flecha). Permanece inactivo.")]
        private RectTransform blockTemplate;

        [SerializeField]
        [Tooltip("Marco redondo para «Girar» (ui_circulo en nueve partes): el bloque píldora del mockup.")]
        private Sprite pillSprite;

        [SerializeField]
        [Tooltip("«Ejecutar»: clic simple, nunca doble (PG-04, RF-32).")]
        private Button executeButton;

        [SerializeField]
        [Tooltip("Botón de pista: repite la instrucción vigente sin gastar un intento (RF-13).")]
        private Button helpButton;

        [SerializeField]
        [Tooltip("Lo que dice el guía ahora: instrucción, resultado de la ejecución o pista (RF-11, RF-13).")]
        private Text messageLabel;

        [SerializeField]
        [Tooltip("Icono que acompaña a la frase: el segundo canal de RNF-19.")]
        private Image messageIcon;

        [SerializeField] private Sprite acceptedIcon;
        [SerializeField] private Sprite rejectedIcon;
        [SerializeField] private Sprite helpIcon;
        [SerializeField] private Color acceptedColor = new Color(0.20f, 0.40f, 0.22f);
        [SerializeField] private Color rejectedColor = new Color(0.60f, 0.36f, 0.10f);
        [SerializeField] private Color helpColor = new Color(0.24f, 0.30f, 0.44f);
        [SerializeField] private Color attentionColor = new Color(0.91f, 0.64f, 0.24f);
        [SerializeField] private Color ivoryShadeColor = new Color(0.878f, 0.831f, 0.753f);
        [SerializeField] private Color charcoalColor = new Color(0.227f, 0.118f, 0.094f);
        [SerializeField] private Color softCharcoalColor = new Color(0.42f, 0.32f, 0.28f);

        [SerializeField]
        [Tooltip("Alto de un bloque desplegado (mockup: 112) y comprimido.")]
        private float expandedHeight = 112f;

        [SerializeField] private float compactHeight = 64f;

        [SerializeField]
        [Tooltip("Cuánto crece el bloque en ejecución (1.04 = un 4 %). Acompaña al contorno: dos indicadores (RNF-19).")]
        private float highlightScale = 1.04f;

        private static readonly PhaseId Phase3 = new PhaseId(LevelId.Wheel, 3);

        private static readonly Dictionary<BlockKind, string> Labels = new Dictionary<BlockKind, string>
        {
            [BlockKind.Forward] = "Avanzar",
            [BlockKind.Backward] = "Retroceder",
            [BlockKind.Turn] = "Girar"
        };

        private readonly List<RectTransform> _rows = new List<RectTransform>();
        private readonly BlockSequence _sequence = new BlockSequence();

        private MazeGrid _grid;
        private HintPolicy _hints;
        private WheelIndicatorCollector _indicators;
        private Canvas _canvas;
        private RectTransform _held;
        private InstructionBlock _heldBlock;
        private int _expanded = -1;

        /// <summary>El flujo del juego. Lo pone <c>Boot</c>; una prueba puede inyectar otro.</summary>
        internal GameFlowRunner Runner { get; set; }

        internal bool IsExecuting { get; private set; }

        /// <summary>
        /// Cuántas veces se pulsó «Ejecutar». Existe para el indicador docente de W15 (RF-46) y
        /// **no** para limitar: no hay tope de ejecuciones ni pantalla de derrota (CP-02, RF-18).
        /// </summary>
        internal int Executions { get; private set; }

        internal MazeGrid Grid => _grid;
        internal BlockSequence Sequence => _sequence;
        internal MazeLayout Layout => layout;
        internal bool IsPaletteOpen => paletteBody != null && paletteBody.activeSelf;

        /// <summary>Cuánto se ha bajado la lista dentro de su ventana, en píxeles.</summary>
        private float _scroll;

        /// <summary>Que la próxima pintura mire al final: acaba de entrar un bloque.</summary>
        private bool _scrollToEnd;

        /// <summary>Si las filas están comprimidas (rótulo + flecha) porque ya no caben desplegadas.</summary>
        internal bool IsCompact { get; private set; }

        /// <summary>El índice del bloque desplegado en modo comprimido; -1 si ninguno.</summary>
        internal int Expanded => _expanded;

        /// <summary>La orientación que muestra la carretilla en pantalla, leída de su rotación.</summary>
        internal Orientation ShownFacing =>
            (Orientation)((Mathf.RoundToInt(-cart.localEulerAngles.z / 90f) % 4 + 4) % 4);

#if UNITY_INCLUDE_TESTS
        internal IReadOnlyList<RectTransform> Rows => _rows;
        internal RectTransform Cart => cart;
        internal Image Environment => environment;
        internal RectTransform SequenceList => sequenceList;
        internal RectTransform SequenceViewport => sequenceViewport;
        internal RectTransform DropZone => dropZone;
        internal Button ScrollUpButton => scrollUpButton;
        internal Button ScrollDownButton => scrollDownButton;
        internal Button PaletteToggle => paletteToggle;
        internal Button ExecuteButton => executeButton;
        internal Button HelpButton => helpButton;
        internal Text MessageLabel => messageLabel;
        internal Image MessageIcon => messageIcon;
        internal RectTransform Held => _held;
        internal WheelSounds Sounds => sounds;
        internal IEnumerable<RectTransform> PaletteBlocks => paletteRowA.Cast<Transform>().Concat(paletteRowB.Cast<Transform>())
            .Select(child => (RectTransform)child)
            .Where(child => child.gameObject.activeSelf);
        internal IEnumerable<RectTransform> Pieces => environment.rectTransform.Cast<Transform>()
            .Select(child => (RectTransform)child)
            .Where(child => child.gameObject.activeSelf);
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            _grid = MazeGrid.Generate(layout, layout.Seed != 0 ? layout.Seed : System.Environment.TickCount);
            _hints = new HintPolicy(StepForPhase3());
            _indicators = new WheelIndicatorCollector(Phase3.Phase, () => Time.realtimeSinceStartup);
            if (Runner != null)
            {
                Runner.ActiveReporter = _indicators; // RF-07: la pausa no suma tiempo de resolución.
            }

            _canvas = environment.GetComponentInParent<Canvas>();

            if (sounds != null && AudioManager.Instance != null)
            {
                // El bosque de día sigue de fondo: el mismo clip que la escena anterior, sin costura.
                AudioManager.Instance.PlayAmbient(sounds.ForestAmbient);
            }

            FitEnvironment();
            cellTemplate.gameObject.SetActive(false);
            blockTemplate.gameObject.SetActive(false);

            var random = new System.Random(_grid.Obstacles.Count);
            foreach (var obstacle in _grid.Obstacles)
            {
                var art = layout.ObstacleArt.Length > 0 ? layout.ObstacleArt[random.Next(layout.ObstacleArt.Length)] : null;
                Spawn($"Obstaculo_{obstacle.x}_{obstacle.y}", obstacle, art, new Color(0.35f, 0.32f, 0.42f));
            }

            Spawn("Refugio", _grid.Goal, layout.ShelterArt, attentionColor);
            PlaceCart(_grid.Start);
            cart.SetAsLastSibling();

            foreach (var kind in Labels.Keys)
            {
                var piece = SpawnBlock(InstructionBlock.Default(kind), kind == BlockKind.Backward ? paletteRowB : paletteRowA, RowMode.Palette);
                piece.name = $"Bloque_{kind}";
                var taken = kind;
                var handle = piece.GetComponent<CargoHandle>();
                handle.Taken += () => TakeFromPalette(InstructionBlock.Default(taken));
                handle.Released += Drop;
            }

            paletteToggle.onClick.AddListener(TogglePalette);
            SetPalette(false);

            executeButton.onClick.AddListener(Execute);
            scrollUpButton.onClick.AddListener(() => Scroll(-1f));
            scrollDownButton.onClick.AddListener(() => Scroll(1f));
            // Pedir ayuda no es un intento: repite la instrucción vigente y no toca ningún
            // contador (RF-13, CP-06). La regla vive en HintPolicy; aquí solo se pulsa.
            helpButton.onClick.AddListener(() => Show(_hints.RequestHelp(), helpIcon, helpColor));

            RefreshRows();
            Show(_hints.RequestHelp(), helpIcon, helpColor);
        }

        /// <summary>
        /// La ilustración cabe **entera** en su panel: escala mínima entre ancho y alto, tamaño
        /// nativo y <c>localScale</c>, igual que <see cref="IllustrationFraming.Apply"/> pero sin
        /// recortar. Lo que cuelga de ella se mide en fracciones de la imagen.
        /// </summary>
        private void FitEnvironment()
        {
            environment.enabled = environment.sprite != null;
            environment.preserveAspect = false;
            var image = environment.sprite != null ? environment.sprite.rect.size : new Vector2(16f, 9f);
            var viewport = ((RectTransform)environment.rectTransform.parent).rect.size;
            var scale = Mathf.Min(viewport.x / image.x, viewport.y / image.y);
            var rect = environment.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = image;
            rect.localScale = new Vector3(scale, scale, 1f);
            rect.anchoredPosition = Vector2.zero;
        }

        /// <summary>Centro de una casilla en fracciones de la ilustración: la matriz trazada sobre el seto.</summary>
        internal Vector2 CellAnchor(Vector2Int cell)
        {
            var span = layout.BoardMax - layout.BoardMin;
            return layout.BoardMin + new Vector2(
                span.x * (cell.x + 0.5f) / _grid.Columns,
                span.y * (cell.y + 0.5f) / _grid.Rows);
        }

        private Vector2 CellSize()
        {
            var image = environment.sprite != null ? environment.sprite.rect.size : environment.rectTransform.sizeDelta;
            var span = Vector2.Scale(layout.BoardMax - layout.BoardMin, image);
            return new Vector2(span.x / _grid.Columns, span.y / _grid.Rows) * layout.PieceSize;
        }

        /// <summary>Cuelga un elemento del entorno en el centro de una casilla, como los props de la narrativa.</summary>
        private void Hang(RectTransform rect, Vector2Int cell)
        {
            rect.SetParent(environment.rectTransform, false);
            rect.anchorMin = rect.anchorMax = CellAnchor(cell);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = CellSize();
        }

        private Image Spawn(string name, Vector2Int cell, Sprite art, Color fallback)
        {
            var image = Instantiate(cellTemplate, environment.rectTransform);
            image.name = name;
            image.sprite = art;
            image.color = art != null ? Color.white : fallback;
            image.preserveAspect = true;
            image.raycastTarget = false;
            Hang(image.rectTransform, cell);
            image.gameObject.SetActive(true);
            return image;
        }

        private void PlaceCart(CartState state)
        {
            Hang(cart, state.Cell);
            var image = cart.GetComponent<Image>();
            if (image != null && layout.CartArt != null)
            {
                image.sprite = layout.CartArt;
                image.color = Color.white;
            }

            cart.localRotation = Rotation(state.Facing);
        }

        /// <summary>El arte mira hacia arriba a 0°; el sentido horario de la matriz es negativo en Z.</summary>
        private static Quaternion Rotation(Orientation facing) => Quaternion.Euler(0f, 0f, -90f * (int)facing);

        // --- los bloques ----------------------------------------------------------------------------

        private enum RowMode
        {
            Palette,
            Expanded,
            Compact,
            Held
        }

        /// <summary>
        /// Clona el bloque modelo y enciende solo las partes que ese modo usa: el marco píldora
        /// para «Girar», el rombo para «Retroceder» (la tercera silueta, RNF-19), la muesca y la
        /// pestaña del encaje, el contador o el lado cuando está desplegado, la flecha cuando
        /// está comprimido.
        /// </summary>
        private RectTransform SpawnBlock(InstructionBlock block, Transform parent, RowMode mode, int index = -1)
        {
            var piece = Instantiate(blockTemplate, parent);
            var pill = block.Kind == BlockKind.Turn && pillSprite != null;
            var frame = piece.GetComponent<Image>();
            var face = piece.Find("Fondo")?.GetComponent<Image>();
            if (pill)
            {
                frame.sprite = pillSprite;
                if (face != null)
                {
                    face.sprite = pillSprite;
                }
            }

            var label = piece.Find("Fondo/Label")?.GetComponent<Text>();
            if (label != null)
            {
                label.text = Labels[block.Kind];
                // El rótulo deja sitio a lo que haya a su derecha: contador o lado desplegados,
                // la flecha comprimida, o nada (en la paleta va centrado, como en el mockup).
                var right = mode switch
                {
                    RowMode.Expanded => -200f,
                    RowMode.Compact => -80f,
                    _ => -20f
                };
                label.rectTransform.offsetMax = new Vector2(right, label.rectTransform.offsetMax.y);
                label.alignment = mode == RowMode.Palette || mode == RowMode.Held ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft;
            }

            Part(piece, "Fondo/Rombo", block.Kind == BlockKind.Backward);
            // El encaje del mockup: muesca arriba y pestaña abajo. En la secuencia la muesca se
            // ve «llena» (marfil) cuando hay un bloque encajado encima y vacía en el primero.
            Part(piece, "Muesca", mode != RowMode.Held);
            var notch = piece.Find("Muesca/Hueco")?.GetComponent<Image>();
            if (notch != null)
            {
                notch.color = mode != RowMode.Palette && index > 0 ? face != null ? face.color : Color.white : ivoryShadeColor;
            }

            Part(piece, "Pestana", mode == RowMode.Palette || (mode != RowMode.Held && index == _sequence.Count - 1));
            Part(piece, "Fondo/Flecha", mode == RowMode.Compact);
            Part(piece, "Fondo/Contador", mode == RowMode.Expanded && block.Kind != BlockKind.Turn);
            Part(piece, "Fondo/Lado", mode == RowMode.Expanded && block.Kind == BlockKind.Turn);

            if (mode == RowMode.Expanded)
            {
                var i = index;
                Wire(piece, "Fondo/Contador/Menos", () => SetCount(i, -1));
                Wire(piece, "Fondo/Contador/Mas", () => SetCount(i, +1));
                Wire(piece, "Fondo/Lado/Izq", () => SetDirection(i, TurnDirection.Left));
                Wire(piece, "Fondo/Lado/Der", () => SetDirection(i, TurnDirection.Right));
                var count = piece.Find("Fondo/Contador/Cifra")?.GetComponent<Text>();
                if (count != null)
                {
                    count.text = block.Count.ToString();
                }

                Side(piece, "Fondo/Lado/Izq", block.Direction == TurnDirection.Left);
                Side(piece, "Fondo/Lado/Der", block.Direction == TurnDirection.Right);
            }
            else if (mode == RowMode.Compact)
            {
                var i = index;
                Wire(piece, "Fondo/Flecha", () => Expand(i));
                // «Retroceder» lleva el rombo asomando por la derecha: la flecha se corre para no pisarlo.
                var arrow = piece.Find("Fondo/Flecha") as RectTransform;
                if (arrow != null && block.Kind == BlockKind.Backward)
                {
                    arrow.anchoredPosition = new Vector2(-44f, arrow.anchoredPosition.y);
                }
            }

            var outline = piece.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }

            var element = piece.GetComponent<LayoutElement>();
            if (element != null)
            {
                element.preferredHeight = mode == RowMode.Compact ? compactHeight : expandedHeight;
            }

            piece.gameObject.SetActive(true);
            return piece;
        }

        private static void Part(Transform piece, string path, bool on)
        {
            var part = piece.Find(path);
            if (part != null)
            {
                part.gameObject.SetActive(on);
            }
        }

        private static void Wire(Transform piece, string path, UnityEngine.Events.UnityAction action)
        {
            var button = piece.Find(path)?.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(action);
            }
        }

        /// <summary>El lado elegido de «Girar» va en color de atención; el otro en marfil sombra (mockup).</summary>
        private void Side(Transform piece, string path, bool selected)
        {
            var side = piece.Find(path);
            if (side == null)
            {
                return;
            }

            var image = side.GetComponent<Image>();
            if (image != null)
            {
                image.color = selected ? attentionColor : ivoryShadeColor;
            }

            var icon = side.Find("Icono")?.GetComponent<Image>();
            if (icon != null)
            {
                icon.color = selected ? charcoalColor : softCharcoalColor;
            }
        }

        /// <summary>
        /// Vuelve a pintar la secuencia entera: una fila por bloque, en orden. Si desplegadas no
        /// caben, se comprimen todas menos la desplegada a mano (mockup: «→» para editar).
        /// </summary>
        private void RefreshRows()
        {
            foreach (var row in _rows)
            {
                Destroy(row.gameObject);
            }

            _rows.Clear();
            if (_expanded >= _sequence.Count)
            {
                _expanded = -1;
            }

            // El alto de la lista lo decide el grupo vertical del panel (el cajón abierto se lo
            // come): se fuerza el cálculo antes de medir, o la primera pintura mide cero.
            Canvas.ForceUpdateCanvases();
            var group = sequenceList.GetComponent<VerticalLayoutGroup>();
            var padding = group != null ? group.padding.vertical : 0;
            var spacing = group != null ? group.spacing : 0f;
            var dropElement = dropZone != null ? dropZone.GetComponent<LayoutElement>() : null;
            var listHeight = sequenceViewport.rect.height - padding - spacing * _sequence.Count;
            IsCompact = _sequence.Count * expandedHeight + expandedHeight > listHeight;
            // La casilla «Suelta un bloque aquí» también se encoge cuando el sitio escasea.
            var dropHeight = IsCompact ? compactHeight : expandedHeight;
            if (dropElement != null)
            {
                dropElement.preferredHeight = dropHeight;
            }

            var available = listHeight - dropHeight;

            for (var i = 0; i < _sequence.Count; i++)
            {
                var mode = !IsCompact || i == _expanded ? RowMode.Expanded : RowMode.Compact;
                var row = SpawnBlock(_sequence[i], sequenceList, mode, i);
                row.name = $"Paso_{i}";
                var index = i;
                var handle = row.GetComponent<CargoHandle>();
                handle.Taken += () => TakeFromSequence(index);
                handle.Released += Drop;
                _rows.Add(row);
            }

            if (dropZone != null)
            {
                dropZone.SetAsLastSibling();
            }

            // Ni comprimidas caben a partir de unos ocho bloques. Antes se repartían el hueco a
            // partes iguales y acababan de 26 px, una encima de otra y con el rótulo fuera de su
            // marco; ahora **conservan su alto y la lista se desplaza** dentro de su ventana, que
            // es lo que hace el mockup con su `overflow-y: auto`. Se recorre con dos botones y no
            // con un ScrollRect: ese trae el arrastre de uGUI, y el esquema radicado es clic y
            // clic sostenido (RNF-02, CT-06) — lo vigila
            // `MazeScene_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido`.
            LayoutRebuilder.ForceRebuildLayoutImmediate(sequenceList);
            SetScroll(_scrollToEnd ? ScrollMax : _scroll);
            _scrollToEnd = false;
        }

        /// <summary>Cuánto sobra de lista por debajo de la ventana. Cero = cabe entera.</summary>
        private float ScrollMax => Mathf.Max(0f, sequenceList.rect.height - sequenceViewport.rect.height);

        /// <summary>Una página menos una fila, para no perder el hilo entre página y página.</summary>
        private float Page => Mathf.Max(sequenceViewport.rect.height - compactHeight, compactHeight);

        private void Scroll(float pages) => SetScroll(_scroll + pages * Page);

        /// <summary>
        /// Deja la lista a esa altura dentro de su ventana y pone los dos botones acordes: el que
        /// no tiene a dónde ir se apaga, y si la secuencia cabe entera no se muestra ninguno.
        /// </summary>
        private void SetScroll(float value)
        {
            var max = ScrollMax;
            _scroll = Mathf.Clamp(value, 0f, max);
            sequenceList.anchoredPosition = new Vector2(sequenceList.anchoredPosition.x, _scroll);

            var desborda = max > 0.5f;
            scrollUpButton.gameObject.SetActive(desborda);
            scrollDownButton.gameObject.SetActive(desborda);
            scrollUpButton.interactable = _scroll > 0.5f;
            scrollDownButton.interactable = _scroll < max - 0.5f;
        }

        /// <summary>«→» sobre un bloque comprimido: se despliega para editarlo; el anterior se comprime.</summary>
        internal void Expand(int index)
        {
            _expanded = _expanded == index ? -1 : index;
            RefreshRows();
        }

        /// <summary>«−» / «+» de «Avanzar» y «Retroceder»: la cuenta se edita en su sitio, entre 1 y 9 (mockup).</summary>
        internal void SetCount(int index, int delta)
        {
            if (IsExecuting || index >= _sequence.Count)
            {
                return;
            }

            _sequence.Replace(index, _sequence[index].WithCount(_sequence[index].Count + delta));
            _indicators.RecordEdit();
            RefreshRows();
        }

        /// <summary>Izquierda / derecha de «Girar»: el lado se edita en su sitio.</summary>
        internal void SetDirection(int index, TurnDirection direction)
        {
            if (IsExecuting || index >= _sequence.Count)
            {
                return;
            }

            _sequence.Replace(index, _sequence[index].WithDirection(direction));
            _indicators.RecordEdit();
            RefreshRows();
        }

        /// <summary>El cajón «Bloques» del mockup: cerrado al abrir la escena, un clic lo abre o lo cierra.</summary>
        internal void TogglePalette() => SetPalette(!IsPaletteOpen);

        private void SetPalette(bool open)
        {
            if (paletteBody != null)
            {
                paletteBody.SetActive(open);
            }

            if (paletteArrow != null)
            {
                // Cerrado: flecha arriba («hay más abajo»); abierto: flecha abajo.
                paletteArrow.localRotation = Quaternion.Euler(0f, 0f, open ? 0f : 180f);
            }

            if (_grid != null)
            {
                // El cajón abierto le quita alto a la lista: las filas se recomprimen.
                RefreshRows();
            }
        }

        /// <summary>Clic sostenido sobre un bloque de la paleta: se toma una copia; la paleta no se vacía.</summary>
        internal void TakeFromPalette(InstructionBlock block)
        {
            if (IsExecuting || _held != null)
            {
                return;
            }

            _heldBlock = block;
            _held = SpawnBlock(block, _canvas.transform, RowMode.Held);
            _held.name = "Bloque_Sostenido";
            _held.sizeDelta = new Vector2(blockTemplate.sizeDelta.x, expandedHeight);
            foreach (var graphic in _held.GetComponentsInChildren<Graphic>())
            {
                graphic.raycastTarget = false;
            }
        }

        /// <summary>Clic sostenido sobre un bloque ya enganchado: sale de la secuencia mientras se sostiene (RF-34).</summary>
        internal void TakeFromSequence(int index)
        {
            if (IsExecuting || _held != null || index >= _sequence.Count)
            {
                return;
            }

            var block = _sequence.RemoveAt(index);
            _indicators.RecordEdit(); // Retirar un bloque (§3.6.1, fase 3).
            RefreshRows();
            TakeFromPalette(block);
        }

        /// <summary>
        /// Soltar el clic. Sobre la secuencia, el bloque se engancha donde cae; fuera, se
        /// descarta —para el de la paleta no pasa nada, para el de la secuencia es retirarlo—.
        /// Editar no reinicia nada: el laberinto sigue donde estaba (CU-08 FA-6a).
        /// </summary>
        internal void Drop()
        {
            if (_held == null)
            {
                return;
            }

            var mouse = Mouse.current;
            var point = mouse != null ? mouse.position.ReadValue() : (Vector2)_held.position;
            Drop(point);
        }

        internal void Drop(Vector2 screenPoint)
        {
            if (_held == null)
            {
                return;
            }

            // La ventana y no la lista: la lista es más alta que su hueco cuando hay que
            // desplazarla, y soltar sobre la parte recortada no es soltar sobre la lista.
            if (RectTransformUtility.RectangleContainsScreenPoint(sequenceViewport, screenPoint, UiCamera))
            {
                var index = IndexAt(screenPoint);
                _sequence.Insert(index, _heldBlock);
                _indicators.RecordEdit(); // Enganchar —o reordenar— un bloque (§3.6.1, fase 3).
                _expanded = index;
            }

            Destroy(_held.gameObject);
            _held = null;
            RefreshRows();
        }

        /// <summary>Entre qué filas cae el punto: tantas como tengan su centro por encima de él.</summary>
        private int IndexAt(Vector2 screenPoint)
        {
            var index = 0;
            foreach (var row in _rows)
            {
                var center = RectTransformUtility.WorldToScreenPoint(UiCamera, row.position);
                if (center.y > screenPoint.y)
                {
                    index++;
                }
            }

            return index;
        }

        /// <summary>Programa la secuencia sin sostener nada: el atajo de las pruebas.</summary>
        internal void AddBlock(InstructionBlock block)
        {
            _sequence.Add(block);
            _indicators.RecordEdit();
            _scrollToEnd = true;
            RefreshRows();
        }

        // --- la ejecución ---------------------------------------------------------------------------

        /// <summary>«Ejecutar» con un clic simple (PG-04): recorre la secuencia paso a paso (RF-32).</summary>
        internal void Execute()
        {
            if (IsExecuting)
            {
                return;
            }

            if (_sequence.IsEmpty)
            {
                Show(layout.EmptySequenceMessage, helpIcon, helpColor);
                return;
            }

            Executions++;
            var result = SequenceExecutor.Execute(_sequence, _grid);
            // Intentos = ejecuciones que no llegan; Pasos = bloques de la que llegó (§3.6.1, fase 3).
            _indicators.RecordExecution(result.ReachedGoal, _sequence.Count);
            _ = ExecuteAsync(result);
        }

        /// <summary>
        /// Pinta el recorrido: resalta el bloque en curso, mueve la carretilla casilla a casilla
        /// —o la hace intentar y volver—, gira, y al final la devuelve al inicio con la
        /// secuencia intacta (RF-33, RF-34).
        /// </summary>
        private async Awaitable ExecuteAsync(ExecutionResult result)
        {
            IsExecuting = true;
            executeButton.interactable = false;
            var seconds = Mathf.Max(layout.StepSeconds, 0f);

            try
            {
                // La carretilla suena mientras recorre la secuencia, también en el intento que
                // choca y vuelve: la depuración se oye como rueda, nunca como error (RF-33, §2.1,
                // CP-02). **En bucle y no un disparo por paso**: la pieza dura 1,8 s y los pasos
                // 0,6, así que por paso se amontonarían tres ruedas a la vez.
                PlayCart(true);
                foreach (var step in result.Steps)
                {
                    Highlight(step.Index);
                    foreach (var move in step.Moves)
                    {
                        if (step.Block.Kind == BlockKind.Turn)
                        {
                            await Rotate(move.From.Facing, move.To.Facing, seconds);
                        }
                        else if (move.Blocked)
                        {
                            // Intenta y regresa: llega a medio camino de la casilla pedida y vuelve.
                            await Slide(move.From.Cell, move.Attempted.Cell, seconds / 2f, 0.4f);
                            await Slide(move.Attempted.Cell, move.From.Cell, seconds / 2f, 0.4f, reverse: true);
                        }
                        else
                        {
                            await Slide(move.From.Cell, move.To.Cell, seconds, 1f);
                        }
                    }

                    await Awaitable.WaitForSecondsAsync(seconds / 2f, destroyCancellationToken);
                }

                PlayCart(false);
                if (result.ReachedGoal)
                {
                    Highlight(-1);
                    _hints.RegisterSuccessfulAttempt();
                    Show(layout.ReachedMessage, acceptedIcon, acceptedColor);
                    await Awaitable.WaitForSecondsAsync(seconds, destroyCancellationToken);
                    IsExecuting = false;
                    ConfirmPhase3AndLeave();
                    return;
                }

                // El bloque donde se detuvo el avance queda resaltado: eso es lo que el jugador
                // lee para deducir qué corregir. No se le dice cuál (CP-06).
                Highlight(result.StoppedAtStep >= 0 ? result.StoppedAtStep : result.Steps.Count - 1);
                var hint = _hints.RegisterFailedAttempt();
                Show(hint ?? layout.StoppedMessage, hint != null ? helpIcon : rejectedIcon, hint != null ? helpColor : rejectedColor);
                await Awaitable.WaitForSecondsAsync(seconds, destroyCancellationToken);
                PlaceCart(_grid.Start);
            }
            catch (System.OperationCanceledException)
            {
                return;
            }

            IsExecuting = false;
            executeButton.interactable = true;
        }

        /// <summary>Arranca o calla la rueda de la carretilla.</summary>
        private void PlayCart(bool rolling)
        {
            if (sounds == null || AudioManager.Instance == null)
            {
                return;
            }

            if (rolling)
            {
                AudioManager.Instance.PlayHeld(sounds.CartMove);
            }
            else
            {
                AudioManager.Instance.StopHeld();
            }
        }

        private async Awaitable Slide(Vector2Int from, Vector2Int to, float seconds, float fraction, bool reverse = false)
        {
            var a = CellAnchor(from);
            var b = Vector2.Lerp(CellAnchor(from), CellAnchor(to), fraction);
            if (reverse)
            {
                (a, b) = (Vector2.Lerp(CellAnchor(to), CellAnchor(from), fraction), CellAnchor(to));
            }

            await Tween(seconds, t => cart.anchorMin = cart.anchorMax = Vector2.Lerp(a, b, t));
        }

        private async Awaitable Rotate(Orientation from, Orientation to, float seconds)
        {
            var a = Rotation(from);
            var b = Rotation(to);
            await Tween(seconds, t => cart.localRotation = Quaternion.Slerp(a, b, t));
        }

        private async Awaitable Tween(float seconds, System.Action<float> apply)
        {
            var elapsed = 0f;
            do
            {
                elapsed += Time.deltaTime;
                apply(seconds > 0f ? Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / seconds)) : 1f);
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            } while (elapsed < seconds);
        }

        /// <summary>Resalta una fila con **dos** indicadores —contorno y tamaño—; -1 apaga todas (RNF-19).</summary>
        private void Highlight(int index)
        {
            for (var i = 0; i < _rows.Count; i++)
            {
                var on = i == index;
                var outline = _rows[i].GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = on;
                }

                _rows[i].localScale = Vector3.one * (on ? highlightScale : 1f);
            }
        }

        /// <summary>Confirma y guarda la fase 3 (RF-04) y sale al cierre del nivel que declara el asset.</summary>
        private void ConfirmPhase3AndLeave()
        {
            if (Runner == null)
            {
                Debug.LogWarning(
                    $"«{gameObject.scene.name}» se abrió sin pasar por «Boot»: no hay " +
                    $"{nameof(GameFlowRunner)}, así que llegar al refugio no confirma la fase ni navega.", this);
                executeButton.interactable = true;
                return;
            }

            var profile = Runner.Flow.ActiveProfile;
            if (profile != null)
            {
                // Los cuatro indicadores de la fase (RF-45, W15) quedan en disco antes de salir
                // (RF-04, RNF-14). Nunca se muestran al estudiante (CP-03).
                profile.ConfirmPhase(Phase3, _indicators.Complete());
                Runner.Session.SaveActive();
            }

            if (!Runner.StartNarrative(layout.ClosingSequenceId))
            {
                Runner.GoTo(GameState.LevelSelect);
            }
        }

        /// <summary>**`Mouse.current` del Input System nuevo, nunca la clase `Input` legada** (CT-06).</summary>
        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null || _held == null)
            {
                return;
            }

            _held.position = ToWorld(mouse.position.ReadValue());
        }

        private Vector3 ToWorld(Vector2 screenPoint)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)_canvas.transform, screenPoint, UiCamera, out var world);
            return world;
        }

        private Camera UiCamera => _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? _canvas.worldCamera
            : null;

        /// <summary>La frase con sus **dos** indicadores: el icono dice lo mismo que el color (RNF-19, RNF-20).</summary>
        private void Show(string message, Sprite icon, Color color)
        {
            messageLabel.text = message;
            messageIcon.sprite = icon;
            messageIcon.color = color;
            messageIcon.enabled = icon != null;
        }

        private GuideStep StepForPhase3() =>
            guide != null && guide.Steps.Length >= Phase3.Phase ? guide.Steps[Phase3.Phase - 1] : null;
    }
}
