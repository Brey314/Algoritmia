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
        [Tooltip("La caja de alimentos. Lleva un CargoHandle: clic sostenido para agarrarla, soltar para dejarla (RF-25).")]
        private RectTransform cargo;

        [SerializeField]
        [Tooltip("«Empujar»: se habilita con la caja colocada y reproduce el rodado (RF-26).")]
        private Button pushButton;

        [SerializeField]
        [Tooltip("La fila de troncos alineados a la derecha de la caja. Aparece al completar el acopio y es donde se suelta la caja.")]
        private RectTransform logRow;

        [SerializeField]
        [Tooltip("El mundo: entorno, suelo, caja y fila. Es lo que la cámara acerca al completar el acopio; las tablillas quedan fuera.")]
        private RectTransform world;

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

        /// <summary>Los troncos acopiados, en orden: son los que se alinean junto a la caja.</summary>
        private readonly List<ForestObject> _collected = new List<ForestObject>();

        /// <summary>Los troncos alineados en la fila, que giran bajo la caja durante el rodado.</summary>
        private readonly List<Image> _row = new List<Image>();

        private static readonly PhaseId Phase1 = new PhaseId(LevelId.Wheel, 1);

        private PatternSelection _selection;
        private CargoPlacement _cargo;
        private HintPolicy _hints;
        private Canvas _canvas;

        /// <summary>El flujo del juego. Lo pone <c>Boot</c>; una prueba puede inyectar otro.</summary>
        internal GameFlowRunner Runner { get; set; }

        /// <summary>Si el rodado está en curso (RF-26).</summary>
        internal bool IsRolling { get; private set; }

        /// <summary>Si los troncos van volando del acopio a la caja y la cámara se está acercando.</summary>
        internal bool IsTransitioning { get; private set; }

#if UNITY_INCLUDE_TESTS
        internal IReadOnlyList<(ForestObject Object, Button Button, ForestObjectNudge Nudge)> Spawned => _spawned;
        internal IReadOnlyList<Image> Slots => _slots;
        internal PatternSelection Selection => _selection;
        internal CargoPlacement Cargo => _cargo;
        internal RectTransform CargoRect => cargo;
        internal Button PushButton => pushButton;
        internal RectTransform LogRow => logRow;
        internal RectTransform World => world;
        internal IReadOnlyList<Image> Row => _row;
        internal RectTransform FloorArea => floorArea;
        internal Image Environment => environment;
        internal Text CounterLabel => counterLabel;
        internal Text MessageLabel => messageLabel;
        internal Image MessageIcon => messageIcon;
        internal Button HelpButton => helpButton;
        internal WheelLevelConfig Config => config;
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Start()
        {
            _selection = new PatternSelection(config);
            _cargo = new CargoPlacement(_selection, config);
            _hints = new HintPolicy(StepForPhase1());
            _canvas = floorArea.GetComponentInParent<Canvas>();

            // La caja se agarra al pulsar y se suelta al soltar: pulsar y soltar, no arrastrar
            // (RF-25, RNF-02). Dónde va mientras se sostiene lo decide Update leyendo el ratón.
            var handle = cargo.GetComponent<CargoHandle>();
            handle.Taken += TakeCargo;
            handle.Released += ReleaseCargo;

            // «Empujar» no existe durante el acopio: aparece con la fila de troncos y se habilita
            // con la caja colocada (RF-26). Un botón que no tiene tarea todavía no se muestra.
            pushButton.interactable = _cargo.CanPush;
            pushButton.gameObject.SetActive(false);
            pushButton.onClick.AddListener(Push);

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
            // Con arte, **cubre** la pantalla sin deformarse a la resolución que traiga, en el
            // plano general que declara el asset —el mismo con el que termina la escena 2.1—, y
            // sustituir el archivo basta.
            if (environment != null)
            {
                environment.enabled = environment.sprite != null;
                if (environment.sprite != null)
                {
                    environment.preserveAspect = false;
                    IllustrationFraming.Apply(environment.rectTransform, environment.sprite.rect.size,
                        config.PlayFraming);
                }
            }

            logRow.gameObject.SetActive(false);

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
            rect.anchoredPosition = Vector2.zero;

            // Giro y espejo vienen del asset porque **una categoría entera comparte un solo
            // sprite**: la variedad del bosque la pone la postura, no quince ilustraciones. El
            // espejo va por escala negativa en X y no por un segundo sprite volteado, que sería
            // arte duplicado para lo que el motor hace gratis.
            rect.localRotation = Quaternion.Euler(0f, 0f, forestObject.RotationDegrees);
            Place(rect, forestObject.FloorPosition, forestObject.Mirrored, null);

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
            var placed = new List<RectTransform>();
            foreach (var (_, button, _) in _spawned)
            {
                var rect = (RectTransform)button.transform;
                Apartar(rect, placed);
                placed.Add(rect);
            }
        }

        /// <remarks>
        /// Estorba una tablilla **o un objeto ya colocado**: sin lo segundo, tres objetos que
        /// caen bajo la misma tablilla salen al mismo punto y quedan apilados, y solo el de
        /// encima recibe el clic — se vio con el acopio en el centro (11/09/2026).
        ///
        /// Se prueban **tres sitios por estorbo** —a su derecha, a su izquierda, encima— y se
        /// toma el primero que quede libre de **todos** los estorbos y dentro del suelo. Mover
        /// solo «hacia el centro» se anulaba en una ventana estrecha: el acopio empujaba a la
        /// derecha y la piedra ya colocada empujaba a la izquierda, y el objeto no cabía entre
        /// los dos. Si ningún sitio está libre se avanza hacia el centro y se vuelve a mirar; al
        /// agotar las vueltas se queda como esté antes que salirse del suelo (RF-22).
        /// </remarks>
        private void Apartar(RectTransform objeto, List<RectTransform> placed)
        {
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
            var suelo = EnPantalla(floorArea);
            var vueltas = reservedAreas.Length + placed.Count + 1;

            for (var vuelta = 0; vuelta < vueltas; vuelta++)
            {
                var caja = EnPantalla(objeto);
                if (!TryFindObstacle(caja, placed, out var estorbo))
                {
                    return;
                }

                var haciaElCentro = estorbo.center.x < Screen.width / 2f ? 1f : -1f;
                var candidatos = new[]
                {
                    new Vector2(haciaElCentro > 0f ? estorbo.xMax + caja.width / 2f + margen : estorbo.xMin - caja.width / 2f - margen, caja.center.y),
                    new Vector2(haciaElCentro > 0f ? estorbo.xMin - caja.width / 2f - margen : estorbo.xMax + caja.width / 2f + margen, caja.center.y),
                    new Vector2(caja.center.x, estorbo.yMax + caja.height / 2f + margen)
                };

                var destino = candidatos[0];
                foreach (var candidato in candidatos)
                {
                    var sitio = new Rect(candidato - caja.size / 2f, caja.size);
                    var dentro = sitio.xMin >= suelo.xMin && sitio.xMax <= suelo.xMax
                                 && sitio.yMin >= suelo.yMin && sitio.yMax <= suelo.yMax;
                    if (dentro && !TryFindObstacle(sitio, placed, out _))
                    {
                        destino = candidato;
                        break;
                    }
                }

                objeto.anchoredPosition += (destino - caja.center) / escala;
                Canvas.ForceUpdateCanvases();
            }
        }

        /// <summary>Lo primero que estorba a esa caja: una tablilla o un objeto ya colocado.</summary>
        private bool TryFindObstacle(Rect caja, List<RectTransform> placed, out Rect obstacle)
        {
            var estorbo = System.Array.Find(reservedAreas,
                              zona => zona != null && EnPantalla(zona).Overlaps(caja))
                          ?? placed.Find(otro => EnPantalla(otro).Overlaps(caja));

            obstacle = estorbo != null ? EnPantalla(estorbo) : default;
            return estorbo != null;
        }

        /// <summary>El suelo en fracciones, que es el espacio en que se mueven los objetos.</summary>
        private static Rect SueloUnitario => new Rect(0f, 0f, 1f, 1f);

        /// <summary>
        /// Deja el objeto en un punto del suelo con el tamaño que le toca ahí.
        /// </summary>
        /// <remarks>
        /// **La perspectiva es la altura**: el suelo sube hacia el fondo del claro, así que cuanto
        /// más arriba está un objeto más lejos se lee y más pequeño se dibuja, de 1 en el borde
        /// bajo a <see cref="WheelLevelConfig.FarScale"/> en el alto. Y con el cursor cerca crece
        /// un poco: es realce para señalar «esto se puede pulsar», no selección — el clic sigue
        /// siendo el único control (RNF-02). Las dos escalas se multiplican y se aplican con el
        /// espejo del asset, que va en el signo y no en un segundo sprite.
        /// </remarks>
        private void Place(RectTransform rect, Vector2 floor, bool mirrored, Vector2? cursor)
        {
            rect.anchorMin = floor;
            rect.anchorMax = floor;

            var scale = Mathf.Lerp(1f, config.FarScale, Mathf.Clamp01(floor.y));
            if (cursor.HasValue && config.HoverRadius > 0f)
            {
                var near = 1f - Mathf.Clamp01(Vector2.Distance(floor, cursor.Value) / config.HoverRadius);
                scale *= Mathf.Lerp(1f, config.HoverScale, near);
            }

            rect.localScale = new Vector3(mirrored ? -scale : scale, scale, 1f);
        }

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

            var cursor = mouse.position.ReadValue();
            DragCargoTo(cursor);
            Nudge(cursor, Time.deltaTime);
        }

        // --- la caja y el rodado (RF-25, RF-26) -------------------------------------------------

        /// <summary>
        /// El clic sostenido sobre la caja. Antes de los cinco troncos **no ejecuta nada** y el
        /// guía dice cuántos faltan: es un «todavía no», no un error (CU-06 FA-4a, CP-02).
        /// </summary>
        internal void TakeCargo()
        {
            if (IsTransitioning)
            {
                return; // Los troncos todavía van de camino: no hay fila donde dejar la caja.
            }

            var outcome = _cargo.TryTake();
            if (!outcome.Accepted)
            {
                Show(outcome.Message, helpIcon, helpColor);
                return;
            }

            // Mientras se sostiene, la caja pasa por encima de todo lo demás.
            cargo.SetAsLastSibling();
        }

        /// <summary>La caja sigue al cursor mientras se sostiene el clic; si no, no se mueve.</summary>
        internal void DragCargoTo(Vector2 screenPoint)
        {
            if (_cargo == null || !_cargo.IsHeld)
            {
                return;
            }

            cargo.anchoredPosition = ToAnchored(cargo, screenPoint);
        }

        /// <summary>
        /// Soltar el clic. Si la caja quedó sobre la fila de troncos se asienta en su arranque y
        /// «Empujar» se habilita; si no, se queda donde cayó y se dice, sin regañar (CP-02).
        /// </summary>
        /// <remarks>
        /// Una vez habilitado, «Empujar» **no se vuelve a deshabilitar** aunque la caja se levante
        /// otra vez y se deje fuera: lo ganado permanece (CP-02, RF-41). La regla vive en
        /// <see cref="CargoPlacement"/>; aquí solo se copia su respuesta al botón.
        /// </remarks>
        internal void ReleaseCargo()
        {
            if (!_cargo.IsHeld)
            {
                return;
            }

            var overLogs = logRow.gameObject.activeSelf && EnPantalla(cargo).Overlaps(EnPantalla(logRow));
            var outcome = _cargo.Drop(overLogs);

            if (outcome.Accepted)
            {
                cargo.anchoredPosition = ToAnchored(cargo, RowPoint(0f));
                pushButton.interactable = _cargo.CanPush;
                Show(outcome.Message, acceptedIcon, acceptedColor);
                return;
            }

            Show(outcome.Message, rejectedIcon, rejectedColor);
        }

        /// <summary>«Empujar»: reproduce el rodado una sola vez; la segunda pulsación no hace nada.</summary>
        private void Push()
        {
            if (!_cargo.Push())
            {
                return;
            }

            _ = RollAsync();
        }

        /// <summary>
        /// La demostración del rodado (RF-26): la caja recorre la fila de troncos mientras estos
        /// giran bajo ella, y al terminar la fase 1 queda confirmada.
        /// </summary>
        /// <remarks>
        /// Es una interpolación continua de posición y giro, y **nada más cambia**: ningún gráfico
        /// se apaga, se enciende ni cambia de color durante el recorrido. Así es como se cumple
        /// RNF-21 —sin parpadeos ni destellos— y así lo vigila la prueba que muestrea cada cuadro.
        /// </remarks>
        private async Awaitable RollAsync()
        {
            IsRolling = true;
            var rollSeconds = Mathf.Max(config.RollSeconds, 0f);
            var fallSeconds = Mathf.Max(config.FallSeconds, 0f);
            var total = rollSeconds + fallSeconds;
            var rollShare = total > 0f ? rollSeconds / total : 1f;
            var elapsed = 0f;

            try
            {
                // Pasado el último tronco la caja cae al suelo por la derecha: el rodado termina
                // con la caja en el piso, no flotando sobre el borde de la fila. **Es el mismo
                // rodado que cuenta la narrativa**: RollMotion decide avance, giro, caída y ladeo.
                var row = EnPantalla(logRow);
                var box = EnPantalla(cargo);
                var landing = new Vector2(row.xMax + box.width / 2f, row.yMin + box.height / 2f);

                do
                {
                    elapsed += Time.deltaTime;
                    var t = total > 0f ? Mathf.Clamp01(elapsed / total) : 1f;
                    var roll = RollMotion.Evaluate(t, rollShare);

                    var end = RowPoint(1f);
                    var point = roll.Fall <= 0f
                        ? RowPoint(roll.Along)
                        : new Vector2(Mathf.Lerp(end.x, landing.x, roll.Fall), Mathf.Lerp(end.y, landing.y, roll.Drop));
                    cargo.anchoredPosition = ToAnchored(cargo, point);
                    // La caja **rueda**: gira sobre sí misma mientras avanza, exactamente como en
                    // la narrativa, y al caer se ladea.
                    cargo.localRotation = Quaternion.Euler(0f, 0f, roll.Spin + roll.Tilt);
                    foreach (var log in _row)
                    {
                        log.rectTransform.localRotation = Quaternion.Euler(0f, 0f, roll.Spin);
                    }

                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                } while (elapsed < total);
            }
            catch (System.OperationCanceledException)
            {
                return; // La escena se descargó a mitad del rodado: no hay nada que confirmar.
            }

            IsRolling = false;
            ConfirmPhase1AndLeave();
        }

        /// <summary>
        /// Confirma y guarda la fase 1 (RF-04) y sale a la escena narrativa que declara el asset.
        /// </summary>
        /// <remarks>
        /// Sin <see cref="GameFlowRunner"/> —la escena se abrió suelta, sin pasar por <c>Boot</c>—
        /// avisa y se queda: el rodado ya se vio y no hay flujo al que pedir la salida. Con él,
        /// si la secuencia de cierre no existe se cae al menú de niveles antes que dejar al
        /// estudiante en una pantalla sin salida (RNF-13).
        /// </remarks>
        private void ConfirmPhase1AndLeave()
        {
            if (Runner == null)
            {
                Debug.LogWarning(
                    $"«{gameObject.scene.name}» se abrió sin pasar por «Boot»: no hay " +
                    $"{nameof(GameFlowRunner)}, así que el rodado no confirma la fase ni navega.", this);
                return;
            }

            var profile = Runner.Flow.ActiveProfile;
            if (profile != null)
            {
                // W15 emitirá los cuatro indicadores reales de la fase; hasta entonces se confirma
                // con los de una fase sin medir. Lo que importa hoy es que lo aprobado quede en
                // disco antes de salir (RF-04, RNF-14).
                profile.ConfirmPhase(Phase1, default);
                Runner.Session.SaveActive();
            }

            if (!Runner.StartNarrative(config.ClosingSequenceId))
            {
                Runner.GoTo(GameState.LevelSelect);
            }
        }

        /// <summary>
        /// El centro de la caja, en píxeles de pantalla, a lo largo de la fila de troncos:
        /// <c>0</c> es el arranque y <c>1</c> el final.
        /// </summary>
        private Vector2 RowPoint(float t)
        {
            var row = EnPantalla(logRow);
            // El ancho **sin girar**: la caja rueda sobre sí misma y su envolvente en pantalla
            // crece y encoge con el giro; medir con ella hacía temblar el avance.
            var width = cargo.rect.width * Mathf.Abs(cargo.lossyScale.x);
            var x = Mathf.Lerp(row.xMin + width / 2f, row.xMax - width / 2f, t);
            // Encima de los troncos, no dentro: la caja va montada sobre la fila.
            return new Vector2(x, row.yMax);
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

            foreach (var (forestObject, button, nudge) in _spawned)
            {
                if (!button.gameObject.activeSelf)
                {
                    continue;
                }

                var position = forestObject.FloorPosition;
                if (nudge != null)
                {
                    nudge.Step(cursor, deltaTime);
                    position = nudge.Position;
                }

                Place((RectTransform)button.transform, position, forestObject.Mirrored, cursor);
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

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    floorArea, screenCursor, UiCamera, out var local))
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

                if (_selection.IsComplete)
                {
                    _ = TransitionAsync();
                }

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
            _collected.Add(forestObject);
        }

        /// <summary>
        /// Con el acopio completo el bosque se despeja: los distractores se van, los cinco troncos
        /// **vuelan del acopio a la fila, a la derecha de la caja**, y la cámara se acerca a esa
        /// zona. Es lo que el guion §6.1.2 llama «los troncos alineados» y donde se suelta la caja.
        /// </summary>
        /// <remarks>
        /// La cámara es el <c>world</c>: se escala con un pivote calculado para que, al terminar,
        /// la pantalla muestre exactamente el encuadre con el que abre la escena 2.2 —ese es el
        /// contrato del asset, no «la caja quieta»: <c>Camara_Narrativa_N2.md</c> §5.3—. Las
        /// tablillas quedan fuera del mundo y no escalan. El contador de RF-24 se queda: el guion
        /// lo pide visible toda la fase.
        ///
        /// Empieza **un cuadro después** del quinto tronco a propósito: quien pulsó ve el tronco
        /// llegar a su casilla antes de que la casilla se vacíe para volar.
        /// </remarks>
        private async Awaitable TransitionAsync()
        {
            IsTransitioning = true;

            try
            {
                await Awaitable.NextFrameAsync(destroyCancellationToken);

                foreach (var (_, button, _) in _spawned)
                {
                    button.gameObject.SetActive(false);
                }

                // Los troncos salen de sus casillas: las casillas quedan vacías y la fila nace
                // con ellos, que es de donde vuelan.
                var origins = new List<Vector2>();
                for (var i = 0; i < _collected.Count && i < _slots.Count; i++)
                {
                    origins.Add(EnPantalla(_slots[i].rectTransform).center);
                    _slots[i].sprite = null;
                    _slots[i].color = emptySlotColor;
                }

                logRow.gameObject.SetActive(true);
                foreach (var log in _collected)
                {
                    var image = Instantiate(inventorySlotTemplate, logRow);
                    image.name = $"Tronco_{log.Id}";
                    image.sprite = log.Art;
                    image.preserveAspect = true;
                    image.color = Color.white;
                    image.gameObject.SetActive(true);
                    _row.Add(image);
                }

                // La rejilla decide dónde acaba cada tronco; luego se apaga para poder moverlos.
                Canvas.ForceUpdateCanvases();
                var destinations = _row.Select(log => log.rectTransform.anchoredPosition).ToArray();
                var grid = logRow.GetComponent<LayoutGroup>();
                if (grid != null)
                {
                    grid.enabled = false;
                }

                var (pivot, zoom) = IllustrationFraming.ScaleAbout(
                    environment != null && environment.sprite != null ? environment.sprite.rect.size : Vector2.zero,
                    world.rect.size, config.PlayFraming, config.CompletionFraming);
                world.pivot = pivot;
                var seconds = Mathf.Max(config.TransitionSeconds, 0f);
                var elapsed = 0f;

                do
                {
                    elapsed += Time.deltaTime;
                    var t = seconds > 0f ? Mathf.Clamp01(elapsed / seconds) : 1f;
                    var eased = Mathf.SmoothStep(0f, 1f, t);

                    world.localScale = Vector3.one * Mathf.Lerp(1f, zoom, eased);
                    for (var i = 0; i < _row.Count && i < origins.Count; i++)
                    {
                        // El origen se recalcula cada cuadro: la casilla no escala con el mundo
                        // pero la fila sí, y el punto de partida tiene que seguir siendo la casilla.
                        var from = ToAnchored(_row[i].rectTransform, origins[i]);
                        _row[i].rectTransform.anchoredPosition = Vector2.Lerp(from, destinations[i], eased);
                    }

                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                } while (elapsed < seconds);
            }
            catch (System.OperationCanceledException)
            {
                return; // La escena se descargó a mitad de la transición.
            }

            inventoryArea.gameObject.SetActive(false);
            pushButton.gameObject.SetActive(true);
            IsTransitioning = false;
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
        private GuideStep StepForPhase1() =>
            guide != null && guide.Steps.Length >= Phase1.Phase ? guide.Steps[Phase1.Phase - 1] : null;
    }
}
