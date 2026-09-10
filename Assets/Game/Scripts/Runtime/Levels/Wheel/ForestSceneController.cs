using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Game.Scaffolding;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// La escena de la fase 1 del Nivel 2 — el bosque (RF-22, RF-23, RF-24, RF-13). Reparte los
    /// objetos por el suelo, traduce cada clic a <see cref="PatternSelection"/> y pinta lo que la
    /// regla devuelve.
    /// </summary>
    /// <remarks>
    /// Adaptador delgado: aquí no hay ni una regla. Qué comparte el patrón, qué se dice de cada
    /// categoría y cuándo termina la fase viven en <see cref="PatternSelection"/>, que es C# plano
    /// y se prueba en EditMode sin escena; la ayuda vive en <see cref="HintPolicy"/>.
    ///
    /// **Los objetos no están puestos a mano en la escena**: se instancian del catálogo del asset,
    /// cada uno en la posición que ese mismo asset le da. Si estuvieran cableados uno a uno,
    /// añadir un distractor no cambiaría nada y <see cref="WheelLevelConfig"/> sería un adorno en
    /// vez del parámetro que exige RNF-18.
    /// </remarks>
    public class ForestSceneController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Parámetros y catálogo de la fase 1: cuántos troncos, qué objetos y qué se dice.")]
        private WheelLevelConfig config;

        [SerializeField]
        [Tooltip("Contenido del guía del Nivel 2. La fase 1 activa su primera tarea, «Seleccionar».")]
        private GuideContent guide;

        [SerializeField]
        [Tooltip("El suelo: la mitad inferior de la pantalla, donde caen los objetos.")]
        private RectTransform floorArea;

        [SerializeField]
        [Tooltip("Objeto modelo que se clona una vez por entrada del catálogo. Permanece inactivo.")]
        private Button objectTemplate;

        [SerializeField]
        [Tooltip("Contador permanente de acopio (RF-24). Nunca se oculta.")]
        private Text counterLabel;

        [SerializeField]
        [Tooltip("Fila de casillas del acopio. Se llena conforme se recogen los troncos.")]
        private RectTransform inventoryArea;

        [SerializeField]
        [Tooltip("Casilla modelo del acopio. Se clona una por tronco requerido.")]
        private Image inventorySlotTemplate;

        [SerializeField]
        [Tooltip("Lo que dice el guía ahora: instrucción, respuesta al intento o pista (RF-11, RF-13, RF-17).")]
        private Text messageLabel;

        [SerializeField]
        [Tooltip("Icono que acompaña a la frase: es el segundo canal de RNF-19.")]
        private Image messageIcon;

        [SerializeField]
        [Tooltip("Fondo del entorno. Vacío hasta que exista el arte del bosque (RNF-23).")]
        private Image environment;

        [SerializeField]
        [Tooltip("Botón de ayuda: repite la instrucción vigente sin gastar un intento (RF-13).")]
        private Button helpButton;

        [SerializeField]
        [Tooltip("Tablillas que se apoyan sobre el suelo. Ningún objeto se reparte debajo de ellas.")]
        private RectTransform[] reservedAreas = new RectTransform[0];

        [SerializeField]
        [Tooltip("Icono del objeto acopiado: forma cerrada. Provisional, sustituible sin tocar código.")]
        private Sprite acceptedIcon;

        [SerializeField]
        [Tooltip("Icono del objeto devuelto: forma distinta de la anterior, no solo otro color (RNF-19).")]
        private Sprite rejectedIcon;

        [SerializeField]
        [Tooltip("Icono de la instrucción y la pista. Vacío deja el hueco sin icono, que también vale.")]
        private Sprite helpIcon;

        [SerializeField]
        [Tooltip("Color de la casilla del acopio todavía vacía.")]
        private Color emptySlotColor = new Color(0.8784f, 0.8314f, 0.7529f);

        [SerializeField] private Color acceptedColor = new Color(0.20f, 0.40f, 0.22f);
        [SerializeField] private Color rejectedColor = new Color(0.60f, 0.36f, 0.10f);
        [SerializeField] private Color helpColor = new Color(0.24f, 0.30f, 0.44f);

        private readonly List<(ForestObject Object, Button Button, ForestObjectNudge Nudge)> _spawned =
            new List<(ForestObject, Button, ForestObjectNudge)>();

        private readonly List<Image> _slots = new List<Image>();

        private PatternSelection _selection;
        private HintPolicy _hints;
        private Canvas _canvas;

#if UNITY_INCLUDE_TESTS
        internal IReadOnlyList<(ForestObject Object, Button Button, ForestObjectNudge Nudge)> Spawned => _spawned;
        internal IReadOnlyList<Image> Slots => _slots;
        internal PatternSelection Selection => _selection;
        internal Text CounterLabel => counterLabel;
        internal Text MessageLabel => messageLabel;
        internal Image MessageIcon => messageIcon;
        internal Button HelpButton => helpButton;
        internal WheelLevelConfig Config => config;
#endif

        private void Start()
        {
            _selection = new PatternSelection(config);
            _hints = new HintPolicy(StepForPhase1());
            _canvas = floorArea.GetComponentInParent<Canvas>();

            objectTemplate.gameObject.SetActive(false);
            inventorySlotTemplate.gameObject.SetActive(false);

            foreach (var forestObject in config.ForestObjects)
            {
                Spawn(forestObject);
            }

            BuildInventory();
            Despejar();

            // Pedir ayuda no es un intento: repite la instrucción vigente y no toca ningún
            // contador, así que usarla no acerca la pista ni la convierte en la respuesta
            // (RF-13, CP-06). La regla vive en HintPolicy; aquí solo se pulsa.
            helpButton.onClick.AddListener(() => Show(_hints.RequestHelp(), helpIcon, helpColor));

            // Sin arte de entorno el hueco se apaga en vez de pintar un rectángulo de relleno: la
            // pantalla de juego no lleva color de fondo, solo el entorno cuando exista (RNF-23).
            if (environment != null)
            {
                environment.enabled = environment.sprite != null;
            }

            RefreshCounter();
            Show(_hints.RequestHelp(), helpIcon, helpColor);
        }

        /// <summary>
        /// Deja un objeto en el suelo, en el punto que le da el asset.
        /// </summary>
        /// <remarks>
        /// La posición es una fracción del área jugable y no píxeles: así el reparto sobrevive a
        /// cualquier resolución y sigue siendo contenido editable, no una constante del código
        /// (CT-05, RNF-18).
        /// </remarks>
        private void Spawn(ForestObject forestObject)
        {
            var button = Instantiate(objectTemplate, floorArea);
            // El nombre hace único el camino de jerarquía: sin él ninguna prueba automatizada
            // puede señalar un objeto concreto del bosque.
            button.name = $"Objeto_{forestObject.Id}";

            var rect = (RectTransform)button.transform;
            rect.anchorMin = forestObject.FloorPosition;
            rect.anchorMax = forestObject.FloorPosition;
            rect.anchoredPosition = Vector2.zero;

            // Giro y espejo vienen del asset porque **una categoría entera comparte un solo
            // sprite**: la variedad del bosque la pone la postura, no quince ilustraciones. El
            // espejo va por escala negativa en X y no por un segundo sprite volteado, que sería
            // arte duplicado para lo que el motor hace gratis.
            rect.localRotation = Quaternion.Euler(0f, 0f, forestObject.RotationDegrees);
            rect.localScale = new Vector3(forestObject.Mirrored ? -1f : 1f, 1f, 1f);

            // El objeto **es** su ilustración: no lleva tarjeta ni rótulo debajo, está tirado en el
            // suelo del bosque. Cambiar el arte es repuntar `Art` en el asset (RNF-23, RNF-18).
            var image = button.image;
            image.sprite = forestObject.Art;
            image.preserveAspect = true;
            image.color = Color.white;

            button.gameObject.SetActive(true);
            button.onClick.AddListener(() => Choose(forestObject, button));

            // Sin ajuste en el asset el objeto simplemente no se anima. Nulo y no excepción: el
            // movimiento es adorno, y un adorno que falta no puede dejar a nadie sin partida.
            var nudge = config.NudgeFor(forestObject.Category);
            _spawned.Add((forestObject, button,
                nudge != null ? new ForestObjectNudge(nudge, forestObject.FloorPosition, SueloUnitario) : null));
        }

        /// <summary>
        /// Aparta del suelo los objetos que hayan caído bajo una tablilla.
        /// </summary>
        /// <remarks>
        /// **No sobra por tener el reparto bien puesto en el asset.** Las posiciones son
        /// fracciones del suelo, que escala con la ventana, mientras que el acopio, el contador y
        /// la ayuda miden lo mismo en cualquier resolución: en una ventana más estrecha que el
        /// diseño una fracción que estaba libre acaba debajo del acopio, y un objeto tapado no
        /// recibe el clic — si es uno de los cinco troncos, la fase no se puede terminar. Lo cazó
        /// <c>ForestScene_RF22_NingunObjetoQuedaTapadoPorLaInterfaz</c> corriendo en batchmode,
        /// no la vista del Editor.
        ///
        /// Se aparta **en horizontal y hacia el centro**, nunca hacia arriba: subirlo lo sacaría
        /// de la mitad inferior de la pantalla, que es donde RF-22 pone el suelo.
        /// </remarks>
        private void Despejar()
        {
            if (reservedAreas.Length == 0)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();
            foreach (var (_, button, _) in _spawned)
            {
                Apartar((RectTransform)button.transform);
            }
        }

        private void Apartar(RectTransform objeto)
        {
            // Una vuelta por tablilla más una de gracia: apartarse de una puede meterlo en la
            // siguiente, pero no puede volver a la anterior porque siempre se avanza al centro.
            for (var vuelta = 0; vuelta <= reservedAreas.Length; vuelta++)
            {
                var caja = EnPantalla(objeto);
                var estorbo = System.Array.Find(reservedAreas,
                    zona => zona != null && EnPantalla(zona).Overlaps(caja));

                if (estorbo == null)
                {
                    return;
                }

                var tablilla = EnPantalla(estorbo);

                // La conversión de píxeles a `anchoredPosition` va con la escala **del suelo**, no
                // con la del objeto: `anchoredPosition` se mide en el espacio del padre, y desde
                // que un objeto puede aparecer en espejo su propia `lossyScale.x` es negativa —
                // usarla mandaba el objeto hacia el lado contrario, más adentro de la tablilla.
                var escala = objeto.parent != null ? Mathf.Abs(objeto.parent.lossyScale.x) : 1f;
                if (Mathf.Approximately(escala, 0f))
                {
                    return;
                }

                var margen = 8f * escala;
                var destino = tablilla.center.x < Screen.width / 2f
                    ? tablilla.xMax + caja.width / 2f + margen
                    : tablilla.xMin - caja.width / 2f - margen;

                objeto.anchoredPosition += new Vector2((destino - caja.center.x) / escala, 0f);
                Canvas.ForceUpdateCanvases();
            }
        }

        /// <summary>El suelo en fracciones, que es el espacio en que se mueven los objetos.</summary>
        private static Rect SueloUnitario => new Rect(0f, 0f, 1f, 1f);

        /// <summary>
        /// Aparta los objetos por los que pasa el cursor.
        /// </summary>
        /// <remarks>
        /// **`Mouse.current` del Input System nuevo, nunca la clase `Input` legada** (CT-06). Sin
        /// ratón conectado —o en batchmode— no hay nada que animar y se sale: el nivel se juega
        /// igual, porque esto no es un control sino un adorno.
        /// </remarks>
        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            Nudge(mouse.position.ReadValue(), Time.deltaTime);
        }

        /// <summary>
        /// Un fotograma de movimiento con el cursor donde diga, en píxeles de pantalla.
        /// </summary>
        /// <remarks>
        /// Separado de <see cref="Update"/> para poder empujar los objetos desde una prueba sin
        /// fabricar eventos de ratón: lo que hay que verificar es que se apartan y que no tocan
        /// nada más, no que el Input System entrega la posición.
        /// </remarks>
        internal void Nudge(Vector2 screenCursor, float deltaTime)
        {
            if (!TryFloorPoint(screenCursor, out var cursor))
            {
                return;
            }

            foreach (var (_, button, nudge) in _spawned.Where(entrada => entrada.Nudge != null))
            {
                nudge.Step(cursor, deltaTime);

                var rect = (RectTransform)button.transform;
                rect.anchorMin = nudge.Position;
                rect.anchorMax = nudge.Position;
            }
        }

        /// <summary>El cursor en fracción del suelo, el mismo espacio que <c>FloorPosition</c>.</summary>
        /// <remarks>
        /// **Sin recortar a 0..1 a propósito.** Recortarlo haría que un cursor fuera del suelo se
        /// leyera como pegado al borde, y los objetos de la orilla se apartarían de un ratón que
        /// está en la barra del contador. La distancia tiene que poder ser mayor que uno.
        /// </remarks>
        private bool TryFloorPoint(Vector2 screenCursor, out Vector2 fraction)
        {
            fraction = default;
            var camera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    floorArea, screenCursor, camera, out var local))
            {
                return false;
            }

            var suelo = floorArea.rect;
            if (suelo.width <= 0f || suelo.height <= 0f)
            {
                return false;
            }

            fraction = new Vector2(
                (local.x - suelo.xMin) / suelo.width,
                (local.y - suelo.yMin) / suelo.height);
            return true;
        }

        /// <summary>La caja del elemento en píxeles de pantalla, que es donde se pulsa.</summary>
        /// <remarks>
        /// Se envuelve por mínimos y máximos de las cuatro esquinas, y **no** con
        /// <c>esquinas[0]</c>..<c>esquinas[2]</c> como esquinas opuestas: eso solo vale con el
        /// objeto sin girar. En cuanto uno aparece a 45° la resta sale negativa, el <c>Rect</c>
        /// queda de tamaño negativo y <c>Overlaps</c> deja de detectar nada — el reparto creería
        /// que ningún objeto estorba y la comprobación pasaría sola.
        /// </remarks>
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

        /// <summary>Una casilla vacía por tronco requerido; se llenan en orden de acopio.</summary>
        private void BuildInventory()
        {
            for (var indice = 0; indice < _selection.Required; indice++)
            {
                var slot = Instantiate(inventorySlotTemplate, inventoryArea);
                slot.name = $"Casilla_{indice + 1}";
                slot.color = emptySlotColor;
                slot.gameObject.SetActive(true);
                _slots.Add(slot);
            }
        }

        /// <summary>Atiende el clic sobre un objeto: **clic simple y nada más** (RNF-02, CT-06).</summary>
        private void Choose(ForestObject forestObject, Button button)
        {
            var outcome = _selection.Select(forestObject);
            RefreshCounter();

            if (outcome.Accepted)
            {
                // El tronco deja el suelo y aparece en el acopio. **No se pierde de vista**: pasa
                // de un sitio al otro, que es lo que hace visible el avance sin una sola cifra de
                // desempeño (CP-02, CP-03).
                button.gameObject.SetActive(false);
                Store(forestObject);
                _hints.RegisterSuccessfulAttempt();
                Show(outcome.Message, acceptedIcon, acceptedColor);
                return;
            }

            if (string.IsNullOrEmpty(outcome.Message))
            {
                return; // Clic sobre algo ya acopiado: no hubo intento que describir.
            }

            // El distractor se queda donde estaba: rechazar no retira nada ni cierra ningún
            // camino, y no existe la penalización (CP-02, RF-18).
            var hint = _hints.RegisterFailedAttempt();
            if (hint != null)
            {
                Show(hint, helpIcon, helpColor);
                return;
            }

            Show(outcome.Message, rejectedIcon, rejectedColor);
        }

        /// <summary>Pone el tronco recogido en la primera casilla libre del acopio.</summary>
        private void Store(ForestObject forestObject)
        {
            var indice = _selection.Collected - 1;
            if (indice < 0 || indice >= _slots.Count)
            {
                return;
            }

            var slot = _slots[indice];
            slot.sprite = forestObject.Art;
            slot.preserveAspect = true;
            slot.color = Color.white;
        }

        /// <summary>
        /// Pinta la frase con sus **dos** indicadores.
        /// </summary>
        /// <remarks>
        /// El color nunca viaja solo: el icono dice lo mismo que el color, así que el estado de un
        /// intento se distingue sin percibirlo (RNF-19). Quien quite el icono por limpieza visual
        /// rompe el requisito, no el estilo. **Aceptado y devuelto se distinguen por forma** —una
        /// cerrada y otra de aviso—, que es el criterio literal del requisito; los iconos de hoy
        /// son provisionales y se sustituyen desde el Inspector, sin tocar código.
        ///
        /// **El color va en el icono y nunca en la tablilla ni en la frase.** Teñir la superficie
        /// entera dejó el texto carbón sobre azul y se volvió ilegible —visto en pantalla el
        /// 10/09/2026—: la tablilla es marfil siempre, que es lo que sostiene el contraste que
        /// pide RNF-20.
        /// </remarks>
        private void Show(string message, Sprite icon, Color color)
        {
            messageLabel.text = message;
            messageIcon.sprite = icon;
            messageIcon.color = color;
            messageIcon.enabled = icon != null;
        }

        private void RefreshCounter() => counterLabel.text = _selection.CounterText;

        /// <summary>
        /// La tarea del guía que corresponde a la fase 1. El mapeo fase↔tarea lo resuelve el nivel
        /// y no el andamiaje: no es 1:1 en general — el Nivel 1 tiene dos tareas en una sola fase
        /// (ver W03).
        /// </summary>
        private GuideStep StepForPhase1()
        {
            var phase = new PhaseId(LevelId.Wheel, 1);
            return guide != null && guide.Steps.Length >= phase.Phase ? guide.Steps[phase.Phase - 1] : null;
        }
    }
}
