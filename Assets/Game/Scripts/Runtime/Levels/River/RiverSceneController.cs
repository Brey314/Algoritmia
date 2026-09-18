using System.Collections.Generic;
using System.Linq;
using Game.Scaffolding;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.River
{
    /// <summary>
    /// La escena de la recolección del Nivel 3 — la orilla del río (RF-35..RF-39, RF-13). Mueve a
    /// Mamá con los botones de dirección, ofrece «Recoger» cerca de un material, pinta la lista de
    /// tareas y el inventario, y abre el ensamblaje al entrar a la zona con todo.
    /// </summary>
    /// <remarks>
    /// Adaptador delgado: aquí no hay ni una regla. Qué se marca, qué cabe, hasta dónde se anda
    /// y qué falta viven en <see cref="TaskList"/>, <see cref="Inventory"/>, <see cref="RiverWalk"/>
    /// y <see cref="BuildZone"/>, C# plano probado en EditMode; la ayuda en <see cref="HintPolicy"/>.
    ///
    /// **Mamá, los materiales y la zona cuelgan de la ilustración**, anclados en fracciones de
    /// ella (regla de D05 y lección del bosque del 17/09/2026): el plano es fijo y al sustituir el
    /// arte nada se mueve. Los materiales se instancian del catálogo del asset, no están puestos a
    /// mano: añadir uno o moverlo es editar contenido (RNF-18).
    ///
    /// **Aquí no se confirma ninguna fase.** La recolección no se persiste (decisión del
    /// 16/09/2026, <c>PhaseId.PhasesPerLevel</c>); abrir la zona enciende el panel de ensamblaje,
    /// que es quien confirmará base, amarre y mástil (R11). Por eso tampoco hay recolector de
    /// indicadores todavía: los cuatro de RF-45 son por fase confirmada (R13).
    /// </remarks>
    public class RiverSceneController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Parámetros y catálogo de la recolección: tareas, materiales, radio, velocidad y textos.")]
        private RiverLevelConfig config;

        [SerializeField]
        [Tooltip("Contenido del guía del Nivel 3. La recolección activa su primera tarea, «Recolectar».")]
        private GuideContent guide;

        [SerializeField]
        [Tooltip("La ilustración del río. Cubre la pantalla en el plano fijo del asset; de ella cuelgan Mamá, los materiales y la zona.")]
        private Image environment;

        [SerializeField]
        [Tooltip("Mamá en vista cenital. Anclada en fracciones de la ilustración.")]
        private RectTransform player;

        [SerializeField]
        [Tooltip("Material modelo que se clona una vez por entrada del catálogo. Permanece inactivo.")]
        private Image collectibleTemplate;

        [SerializeField]
        [Tooltip("La señal de la zona de construcción, visible desde el principio (RF-39).")]
        private RectTransform buildZoneMarker;

        [SerializeField]
        [Tooltip("Las cuatro flechas en pantalla. La dirección la dice cada botón.")]
        private DirectionPad[] pads = new DirectionPad[0];

        [SerializeField]
        [Tooltip("«Recoger»: solo aparece dentro del radio de proximidad de un material (RF-37).")]
        private Button collectButton;

        [SerializeField]
        [Tooltip("Lista de tareas permanente. Se llena del asset (RF-36).")]
        private RectTransform taskArea;

        [SerializeField]
        [Tooltip("Fila modelo de la lista: un icono de estado y el texto de la tarea como hijos. Permanece inactiva.")]
        private RectTransform taskRowTemplate;

        [SerializeField]
        [Tooltip("Inventario permanente: una casilla por material (RF-38).")]
        private RectTransform inventoryArea;

        [SerializeField]
        [Tooltip("Casilla modelo del inventario. Permanece inactiva.")]
        private Image inventorySlotTemplate;

        [SerializeField]
        [Tooltip("Lo que dice el guía ahora: instrucción, respuesta o pista (RF-11, RF-13).")]
        private Text messageLabel;

        [SerializeField]
        [Tooltip("Icono que acompaña a la frase: segundo canal de RNF-19.")]
        private Image messageIcon;

        [SerializeField]
        [Tooltip("Botón de ayuda: repite la instrucción vigente sin gastar nada (RF-13).")]
        private Button helpButton;

        [SerializeField]
        [Tooltip("El panel de ensamblaje. Se enciende al abrir la zona; su contenido llega con R11.")]
        private GameObject assemblyPanel;

        [SerializeField]
        [Tooltip("Icono de tarea pendiente: forma abierta.")]
        private Sprite pendingIcon;

        [SerializeField]
        [Tooltip("Icono de tarea hecha: forma distinta de la anterior, no solo otro color (RNF-19).")]
        private Sprite doneIcon;

        [SerializeField]
        [Tooltip("Icono de la instrucción y la pista. Vacío deja el hueco sin icono.")]
        private Sprite helpIcon;

        [SerializeField] private Color pendingColor = new Color(0.42f, 0.32f, 0.28f);
        [SerializeField] private Color doneColor = new Color(0.20f, 0.40f, 0.22f);
        [SerializeField] private Color helpColor = new Color(0.24f, 0.30f, 0.44f);
        [SerializeField] private Color emptySlotColor = new Color(0.8784f, 0.8314f, 0.7529f);

        private readonly List<(Collectible Collectible, Image Image)> _spawned = new List<(Collectible, Image)>();
        private readonly List<(RiverTaskId Task, Text Label, Image Icon)> _rows = new List<(RiverTaskId, Text, Image)>();
        private readonly List<Image> _slots = new List<Image>();

        private Inventory _inventory;
        private TaskList _tasks;
        private RiverWalk _walk;
        private BuildZone _zone;
        private HintPolicy _hints;
        private Collectible _reachable;
        private bool _wasInsideZone;

#if UNITY_INCLUDE_TESTS
        internal RiverLevelConfig Config => config;
        internal Inventory Inventory => _inventory;
        internal TaskList Tasks => _tasks;
        internal RiverWalk Walk => _walk;
        internal BuildZone Zone => _zone;
        internal IReadOnlyList<DirectionPad> Pads => pads;
        internal Button CollectButton => collectButton;
        internal Button HelpButton => helpButton;
        internal IReadOnlyList<(Collectible Collectible, Image Image)> Spawned => _spawned;
        internal IReadOnlyList<(RiverTaskId Task, Text Label, Image Icon)> Rows => _rows;
        internal IReadOnlyList<Image> Slots => _slots;
        internal RectTransform Player => player;
        internal RectTransform BuildZoneMarker => buildZoneMarker;
        internal RectTransform TaskArea => taskArea;
        internal RectTransform InventoryArea => inventoryArea;
        internal Image Environment => environment;
        internal Text MessageLabel => messageLabel;
        internal Image MessageIcon => messageIcon;
        internal GameObject AssemblyPanel => assemblyPanel;
        internal Collectible Reachable => _reachable;
#endif

        private void Start()
        {
            _inventory = new Inventory(config);
            _tasks = new TaskList();
            _walk = new RiverWalk(config.StartPosition, config.WalkableArea, config.MoveSpeed);
            _zone = new BuildZone(config);
            _hints = new HintPolicy(StepNamed("Recolectar"));

            // La ilustración cubre la pantalla sin deformarse en el plano fijo del asset —el
            // último de la escena 3.1— y sustituir el archivo basta (RNF-23). Va antes de colgar
            // nada de ella.
            environment.enabled = environment.sprite != null;
            if (environment.sprite != null)
            {
                environment.preserveAspect = false;
                IllustrationFraming.Apply(environment.rectTransform, environment.sprite.rect.size, config.PlayFraming);
            }

            collectibleTemplate.gameObject.SetActive(false);
            taskRowTemplate.gameObject.SetActive(false);
            inventorySlotTemplate.gameObject.SetActive(false);

            foreach (var collectible in config.Collectibles)
            {
                Spawn(collectible);
            }

            Place(buildZoneMarker, config.BuildZonePosition);
            Place(player, _walk.Position);
            BuildTaskList();
            BuildInventory();

            collectButton.onClick.AddListener(Collect);
            collectButton.gameObject.SetActive(false);
            assemblyPanel.SetActive(false);

            // Pedir ayuda no es un intento: repite la instrucción y no toca ningún contador
            // (RF-13, CP-06). La regla vive en HintPolicy; aquí solo se pulsa.
            helpButton.onClick.AddListener(() => Show(_hints.RequestHelp(), helpIcon, helpColor));
            Show(_hints.RequestHelp(), helpIcon, helpColor);
            RefreshReach();
        }

        private void Update() => Tick(HeldDirection, Time.deltaTime);

        /// <summary>La suma de las flechas sostenidas. Sin ninguna, Mamá se queda.</summary>
        internal Vector2 HeldDirection => pads
            .Where(pad => pad != null && pad.IsHeld)
            .Aggregate(Vector2.zero, (sum, pad) => sum + pad.Direction);

        /// <summary>
        /// Un fotograma de juego con esa dirección. Separado de <see cref="Update"/> para que una
        /// prueba mueva a Mamá sin fabricar cuadros ni eventos de puntero.
        /// </summary>
        internal void Tick(Vector2 direction, float deltaTime)
        {
            if (_zone == null || _zone.IsOpen)
            {
                return; // Con el ensamblaje abierto ya no se anda por la orilla.
            }

            _walk.Step(direction, deltaTime);
            Place(player, _walk.Position);
            RefreshReach();

            // La zona se atiende al **entrar**, no cada cuadro: si no, el «todavía falta» se
            // repetiría mientras Mamá esté parada encima.
            var inside = _zone.Contains(_walk.Position);
            if (inside && !_wasInsideZone)
            {
                Enter();
            }

            _wasInsideZone = inside;
        }

        /// <summary>Deja un material en la orilla, en el punto de la ilustración que le da el asset.</summary>
        private void Spawn(Collectible collectible)
        {
            var image = Instantiate(collectibleTemplate, environment.rectTransform);
            // El nombre hace único el camino de jerarquía: sin él ninguna prueba puede señalarlo.
            image.name = $"Material_{collectible.Id}";
            image.sprite = collectible.Art;
            image.preserveAspect = true;
            image.color = Color.white;
            Place(image.rectTransform, collectible.Position);
            image.gameObject.SetActive(true);
            _spawned.Add((collectible, image));
        }

        /// <summary>Ancla el elemento en una fracción de su padre —la ilustración— y lo deja ahí.</summary>
        private static void Place(RectTransform rect, Vector2 fraction)
        {
            rect.anchorMin = fraction;
            rect.anchorMax = fraction;
            rect.anchoredPosition = Vector2.zero;
        }

        /// <summary>«Recoger» aparece solo con un material al alcance; con varios, el más cercano.</summary>
        private void RefreshReach()
        {
            _reachable = _spawned
                .Where(entry => entry.Image.gameObject.activeSelf
                                && entry.Collectible.IsWithinReach(_walk.Position, config.ProximityRadius))
                .OrderBy(entry => Vector2.Distance(entry.Collectible.Position, _walk.Position))
                .Select(entry => entry.Collectible)
                .FirstOrDefault();

            collectButton.gameObject.SetActive(_reachable != null && !_zone.IsOpen);
        }

        /// <summary>
        /// El clic en «Recoger»: el material deja la orilla, entra al inventario y, si tiene tarea
        /// de recolección, la marca (RF-37, RF-36). Recoger no puede fallar (CP-02).
        /// </summary>
        private void Collect()
        {
            var collectible = _reachable;
            var outcome = _inventory.TryCollect(collectible);
            if (!outcome.Accepted)
            {
                if (!string.IsNullOrEmpty(outcome.Message))
                {
                    Show(outcome.Message, helpIcon, helpColor);
                }

                return;
            }

            // Pasa de un sitio al otro y no se pierde de vista: es lo que hace visible el avance
            // sin una sola cifra (CP-03).
            var entry = _spawned.First(spawned => spawned.Collectible == collectible);
            entry.Image.gameObject.SetActive(false);
            Store(collectible);
            _tasks.MarkCollected(collectible.Kind);
            RefreshTasks();
            Show(outcome.Message, doneIcon, doneColor);
            RefreshReach();
        }

        /// <summary>Entrar a la zona. Con todo, abre el ensamblaje; sin todo, dice qué falta y deja salir.</summary>
        private void Enter()
        {
            var outcome = _zone.TryEnter(_inventory);
            if (!outcome.Accepted)
            {
                Show(outcome.Message, helpIcon, helpColor);
                return;
            }

            OpenAssembly(outcome.Message);
        }

        /// <summary>
        /// Enciende el panel de ensamblaje y apaga los controles de la orilla. El panel es un
        /// hueco hasta R11: lo que sí queda hecho es el cambio de tarea del guía.
        /// </summary>
        private void OpenAssembly(string message)
        {
            foreach (var pad in pads)
            {
                pad.gameObject.SetActive(false);
            }

            collectButton.gameObject.SetActive(false);
            assemblyPanel.SetActive(true);
            _hints.Activate(StepNamed("Base"));
            Show(message, doneIcon, doneColor);
        }

        /// <summary>Una fila por tarea del guion, con el texto del asset y su icono de pendiente.</summary>
        private void BuildTaskList()
        {
            foreach (var task in RiverTask.All)
            {
                var row = Instantiate(taskRowTemplate, taskArea);
                row.name = $"Tarea_{(int)task}";
                var label = row.GetComponentInChildren<Text>(true);
                var icon = row.GetComponentInChildren<Image>(true);
                label.text = config.LabelFor(task);
                row.gameObject.SetActive(true);
                _rows.Add((task, label, icon));
            }

            RefreshTasks();
        }

        /// <summary>
        /// Pinta cada tarea con sus **dos** indicadores: el icono cambia de forma y el color
        /// acompaña (RNF-19). Nunca «2 de 4»: el avance se lee tarea a tarea (CP-03).
        /// </summary>
        private void RefreshTasks()
        {
            foreach (var (task, label, icon) in _rows)
            {
                var done = _tasks.IsDone(task);
                label.color = done ? doneColor : pendingColor;
                if (icon != null)
                {
                    icon.sprite = done ? doneIcon : pendingIcon;
                    icon.color = done ? doneColor : pendingColor;
                }
            }
        }

        /// <summary>Una casilla vacía por material del catálogo; se llenan en orden de recogida.</summary>
        private void BuildInventory()
        {
            for (var index = 0; index < _inventory.Capacity; index++)
            {
                var slot = Instantiate(inventorySlotTemplate, inventoryArea);
                slot.name = $"Casilla_{index + 1}";
                slot.color = emptySlotColor;
                slot.gameObject.SetActive(true);
                _slots.Add(slot);
            }
        }

        private void Store(Collectible collectible)
        {
            var index = _inventory.Items.Count - 1;
            if (index < 0 || index >= _slots.Count)
            {
                return;
            }

            var slot = _slots[index];
            slot.sprite = collectible.Art;
            slot.preserveAspect = true;
            slot.color = Color.white;
        }

        /// <summary>
        /// Pinta la frase con sus dos indicadores. El color va en el icono y nunca en la tablilla
        /// ni en la frase (RNF-19, RNF-20), igual que en el Nivel 2.
        /// </summary>
        private void Show(string message, Sprite icon, Color color)
        {
            messageLabel.text = message;
            messageIcon.sprite = icon;
            messageIcon.color = color;
            messageIcon.enabled = icon != null;
        }

        /// <summary>
        /// La tarea del guía por su id. El mapeo tarea del guía ↔ momento del nivel lo resuelve el
        /// nivel: aquí «Recolectar» cubre las tareas 1 y 2 de la lista y «Base» abre con la zona.
        /// </summary>
        private GuideStep StepNamed(string id) =>
            guide != null ? guide.Steps.FirstOrDefault(step => step.Id == id) : null;
    }
}
