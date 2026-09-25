using System;
using System.Collections.Generic;
using System.Linq;
using Game.Audio;
using Game.Core;
using Game.Scaffolding;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.River
{
    /// <summary>
    /// El panel de ensamblaje en escena (RF-40..RF-43, HU-12, HU-13): la balsa compuesta espacio
    /// a espacio sobre el río, los arrastres desde el inventario con clic sostenido, el botón
    /// «Listo» / «Probar balsa», el resaltado del espacio incorrecto con color **e icono**, y las
    /// animaciones de completado y de hundimiento.
    /// </summary>
    /// <remarks>
    /// Adaptador delgado: qué cabe dónde, qué está mal y qué vuelve al inventario lo decide
    /// <see cref="RaftAssembly"/>, C# plano probado en EditMode. Aquí hay geometría y pintura.
    ///
    /// **La balsa es composición, no láminas.** Cada espacio es una imagen: silueta hasta que
    /// se llena, la pieza después, y el icono de alerta encima cuando la validación lo señala.
    /// Diecisiete espacios y ocho sprites cubren todos los estados (decisión de Santiago del
    /// 20/09/2026, en lugar de treinta y cuatro láminas). El área de la balsa **cuelga de la
    /// ilustración** en fracciones de ella, y al abrirse el panel la cámara empuja del plano de
    /// juego al de ensamblaje —el río al 85 % del ancho, la orilla partiendo la pantalla— como
    /// el cierre del taller del Nivel 2.
    ///
    /// **Memoria de nivel, no persistencia.** La escena 3.2 se reproduce en la escena narrativa
    /// y al volver esta se recarga; para que «la escena narra, no reinicia» (CP-02) las piezas
    /// de la fase abierta y el disparador de la 3.2 se guardan en estáticos del assembly. No van
    /// al perfil: la lista de datos persistidos es cerrada (RNF-09), y un cierre forzado los
    /// pierde a propósito, igual que la recolección (RNF-14).
    /// </remarks>
    public class AssemblyPanelController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Contenido del ensamblaje: espacios, arte, encuadre y frases.")]
        private RaftAssemblyContent content;

        [SerializeField]
        [Tooltip("Contenido del guía del Nivel 3: las tareas «Base», «Amarre» y «MastilYVela».")]
        private GuideContent guide;

        [SerializeField]
        [Tooltip("Sonidos del Nivel 3: el martillazo de cada pieza puesta, los de la fase aprobada y la balsa terminada. Vacío = el ensamblaje se juega en silencio.")]
        private RiverSounds sounds;

        [SerializeField]
        [Tooltip("El mundo: el entorno y todo lo que cuelga de él. Es lo que la cámara empuja al abrir el panel.")]
        private RectTransform world;

        [SerializeField]
        [Tooltip("La ilustración del río. De ella cuelgan la sombra y el área de la balsa.")]
        private Image environment;

        [SerializeField]
        [Tooltip("Sombra oscura sobre la ilustración mientras el panel está abierto (negro al 30 %). Hija del entorno, debajo del área de la balsa.")]
        private Image shade;

        [SerializeField]
        [Tooltip("El área de la balsa: un cuadro colgado de la ilustración donde se posicionan los espacios.")]
        private RectTransform raftArea;

        [SerializeField]
        [Tooltip("Espacio modelo, hijo del área de la balsa: imagen, asa y un icono de alerta como hijo. Permanece inactivo.")]
        private Image slotTemplate;

        [SerializeField]
        [Tooltip("Las casillas del inventario: de ellas se arrastran las piezas.")]
        private InventoryView inventory;

        [SerializeField]
        [Tooltip("Lee la posición del puntero desde el EventSystem, en la raíz del lienzo.")]
        private PointerTracker tracker;

        [SerializeField]
        [Tooltip("«Listo» en la base y el amarre, «Probar balsa» en la última fase (RF-40, RF-42).")]
        private Button confirmButton;

        [SerializeField]
        [Tooltip("El rótulo del botón de confirmar.")]
        private Text confirmLabel;

        [SerializeField]
        [Tooltip("Icono que acompaña al color en el espacio incorrecto (RNF-19).")]
        private Sprite wrongIcon;

        [SerializeField] private Color wrongColor = new Color(0.85f, 0.42f, 0.16f);

        [SerializeField]
        [Tooltip("Cuánto crece la balsa en el pulso de completado (1.06 = un 6 %). Continuo, sin destellos (RNF-21).")]
        private float completionScale = 1.06f;

        [SerializeField]
        [Tooltip("Cuánto gira la balsa al hundirse por un costado, en grados.")]
        private float sinkAngle = -14f;

        // Segundos del pulso de completado: la balsa crece y vuelve (RF-41).
        private const float CompletionPulseSeconds = 0.35f;

        // Memoria de nivel (véase el remarks): sobrevive a la recarga de la escena, no al proceso.
        private static ConditionalNarrativeTrigger s_firstFailure;
        private static (RaftPhase Phase, Dictionary<string, MaterialKind> Placed)? s_stash;
        private static RiverIndicatorCollector s_indicators;

        private readonly Dictionary<string, (RaftSlot Slot, Image Image, Image Alert, RaftPieceHandle Handle)> _slots =
            new Dictionary<string, (RaftSlot, Image, Image, RaftPieceHandle)>();

        private readonly HashSet<string> _wrong = new HashSet<string>();

        private RaftAssembly _assembly;
        private RiverIndicatorCollector _indicators;
        private HintPolicy _hints;
        private Inventory _inventory;
        private CameraFraming _from;
        private Action<string, MessageTone> _show;
        private Action<RaftPhase> _phaseConfirmed;
        private Action<ActorAction> _react;
        private Image _ghost;
        private MaterialKind? _held;
        private bool _handlesAttached;

        /// <summary>El flujo del juego. Lo pone <c>Boot</c>; una prueba puede inyectar otro.</summary>
        internal GameFlowRunner Runner { get; set; }

        /// <summary>Si hay una animación en curso —empuje, completado o hundimiento—: mientras tanto no se toca nada.</summary>
        internal bool IsBusy { get; private set; }

#if UNITY_INCLUDE_TESTS
        internal RaftAssembly Assembly => _assembly;
        internal RaftAssemblyContent Content => content;
        internal RiverSounds Sounds => sounds;
        internal RectTransform RaftArea => raftArea;
        internal Button ConfirmButton => confirmButton;
        internal Text ConfirmLabel => confirmLabel;
        internal Image Shade => shade;
        internal Image Ghost => _ghost;
        internal MaterialKind? Held => _held;
        internal IReadOnlyCollection<string> Wrong => _wrong;
        internal IReadOnlyDictionary<string, (RaftSlot Slot, Image Image, Image Alert, RaftPieceHandle Handle)> Slots => _slots;
        internal static void ForgetLevelMemory()
        {
            s_firstFailure = null;
            s_stash = null;
            s_indicators = null;
        }
#endif

        private void Awake() => Runner ??= GameFlowRunner.Instance;

        private void Update()
        {
            if (_held.HasValue && _ghost != null)
            {
                DragTo(tracker.ScreenPosition);
            }
        }

        /// <summary>
        /// Abre el panel con el inventario completo, en la fase que toque, y empuja la cámara al
        /// plano del ensamblaje. Con una fase mayor que la base, retoma (RNF-14). Los personajes
        /// son de la escena: el panel solo dice cuándo celebran y cuándo animan (<paramref name="react"/>).
        /// </summary>
        internal void Open(Inventory inventoryModel, RaftPhase phase, CameraFraming from,
            Action<string, MessageTone> show, Action<RaftPhase> phaseConfirmed, Action<ActorAction> react)
        {
            _inventory = inventoryModel;
            _from = from;
            _show = show;
            _phaseConfirmed = phaseConfirmed;
            _react = react;
            _assembly = new RaftAssembly(content, RaftAssembly.SupplyFrom(inventoryModel, content));
            if (phase > RaftPhase.Base)
            {
                _assembly.Resume(phase);
            }
            else
            {
                // Empezar la base desde la orilla es empezar el nivel: la 3.2 vuelve a estar por ver.
                s_firstFailure = null;
                s_stash = null;
                s_indicators = null;
            }

            s_firstFailure ??= new ConditionalNarrativeTrigger(content.FirstFailureSequenceId);
            RestoreStash(phase);

            // Los indicadores de RF-45 son del nivel entero y sobreviven a la escena 3.2 como el
            // resto de la memoria de nivel; al retomar desde disco, las fases ya aceptadas cuentan
            // como pasos (RNF-14). De vuelta de una narrativa, el tiempo narrado no cuenta (nota 1).
            _indicators = s_indicators ??= new RiverIndicatorCollector(() => Time.realtimeSinceStartup, (int)phase - 1);
            _indicators.PauseClosed();
            if (Runner != null)
            {
                Runner.ActiveReporter = _indicators; // RF-07: la pausa no suma tiempo de resolución.
            }

            _hints = new HintPolicy(StepFor(phase));
            gameObject.SetActive(true);
            shade.gameObject.SetActive(true);
            shade.raycastTarget = true;

            Hang(raftArea, content.RaftPosition, content.RaftSize);
            BuildSlots();
            AttachInventoryHandles();
            confirmButton.onClick.RemoveListener(Confirm);
            confirmButton.onClick.AddListener(Confirm);
            RefreshAll();
            _ = PushAsync();
        }

        /// <summary>Ayuda a demanda: la instrucción de la fase, sin gastar nada (RF-13).</summary>
        internal string RequestHelp() => _hints?.RequestHelp() ?? string.Empty;

        // --- los arrastres -----------------------------------------------------------------------

        /// <summary>El clic sostenido sobre una casilla del inventario con piezas.</summary>
        internal void Take(MaterialKind kind, Vector2 at)
        {
            if (IsBusy || _held.HasValue || _assembly == null || _assembly.Remaining(kind) <= 0)
            {
                return;
            }

            Hold(kind, at, inventory.SlotOf(kind).rectTransform.rect.size);
        }

        /// <summary>El clic sostenido sobre un espacio ocupado de la fase abierta: la pieza sale y vuelve a la mano.</summary>
        internal void TakeFromSlot(string slotId, Vector2 at)
        {
            if (IsBusy || _held.HasValue || _assembly == null)
            {
                return;
            }

            var kind = _assembly.PlacedIn(slotId);
            var outcome = _assembly.TakeBack(slotId);
            if (!outcome.Accepted || !kind.HasValue)
            {
                return;
            }

            _wrong.Remove(slotId);
            RefreshAll();
            Hold(kind.Value, at, _slots[slotId].Image.rectTransform.rect.size);
        }

        /// <summary>La pieza sigue al cursor mientras se sostiene el clic; si no, no se mueve.</summary>
        internal void DragTo(Vector2 screenPoint)
        {
            if (_held.HasValue && _ghost != null)
            {
                // Lienzo en espacio de pantalla: la posición del mundo son píxeles de pantalla.
                _ghost.rectTransform.position = screenPoint;
            }
        }

        /// <summary>
        /// Soltar el clic. Sobre un espacio abierto y vacío la pieza se queda; si no, vuelve al
        /// inventario y el guía dice por qué (guion §1.8.4, CP-02). Nada se valida aquí.
        /// </summary>
        internal void Release(Vector2 at)
        {
            if (!_held.HasValue)
            {
                return;
            }

            var kind = _held.Value;
            _held = null;
            _ghost.gameObject.SetActive(false);

            // Los espacios se solapan a propósito —el mástil cruza los troncos y roza la vela—,
            // así que entre los que contienen el punto gana el de centro más cercano, no el
            // primero del asset.
            var target = _slots.Values
                .Where(entry => _assembly.IsOpen(entry.Slot)
                                && RectTransformUtility.RectangleContainsScreenPoint(entry.Image.rectTransform, at, null))
                .OrderBy(entry => Vector2.Distance(entry.Image.rectTransform.position, at))
                .FirstOrDefault();

            if (target.Slot == null)
            {
                RefreshAll();
                _show(content.MissedMessage, MessageTone.Help);
                return;
            }

            var outcome = _assembly.Place(target.Slot.Id, kind);
            if (outcome.Accepted)
            {
                _wrong.Remove(target.Slot.Id);
                // Suena lo puesto, no lo correcto: colocar no valida (guion §1.8.4), así que la
                // pieza equivocada también se clava y es el botón quien la devuelve.
                Play(sounds?.PiecePlaced);
            }

            RefreshAll();
            _show(outcome.Accepted ? outcome.Message : content.OccupiedMessage, MessageTone.Help);
        }

        // --- el botón ----------------------------------------------------------------------------

        /// <summary>«Listo» o «Probar balsa» (RF-40, RF-42).</summary>
        internal void Confirm()
        {
            if (IsBusy || _held.HasValue || _assembly == null || _assembly.IsComplete)
            {
                return;
            }

            var phase = _assembly.ActivePhase;
            // Qué espacios tenían pieza antes de validar: los señalados de entre ellos son los que
            // vuelven al inventario, y solo esos pueden ser un error corregido después (§3.6.1).
            var placedBefore = _slots.Keys
                .Where(id => _assembly.IsOpen(_slots[id].Slot) && _assembly.PlacedIn(id).HasValue)
                .ToArray();
            var result = _assembly.Confirm();
            _indicators.RecordConfirmation(result, result.WrongSlotIds.Where(placedBefore.Contains));

            if (result.Passed)
            {
                _hints.RegisterSuccessfulAttempt();
                _ = ConfirmedAsync(phase, result.Message);
                return;
            }

            // El espacio incorrecto se señala con color e icono; el mensaje dice qué revisar y
            // nunca cuál es la pieza (RNF-19, CP-06). Las piezas mal puestas ya volvieron.
            _wrong.Clear();
            _wrong.UnionWith(result.WrongSlotIds);
            RefreshAll();
            var hint = _hints.RegisterFailedAttempt();
            var message = hint ?? result.Message;
            var tone = hint != null ? MessageTone.Help : MessageTone.Rejected;

            // Tras un intento sin éxito —también cuando la balsa se hunde— la familia anima y
            // nunca hace un gesto de derrota: el hundimiento es el guion, no un castigo (CP-02, §7.3).
            _react?.Invoke(ActorAction.Encourage);

            if (phase == RaftPhase.MastAndSail)
            {
                _ = SinkAsync(message, tone);
                return;
            }

            _show(message, tone);
        }

        // --- las animaciones ---------------------------------------------------------------------

        /// <summary>
        /// El empuje de cámara del plano de juego al del ensamblaje: se escala el mundo con el
        /// pivote que deja el foco de destino en el centro, la misma mecánica del taller.
        /// Interpolación continua, nada parpadea (RNF-21).
        /// </summary>
        private async Awaitable PushAsync()
        {
            IsBusy = true;
            RefreshButton();
            var seconds = Mathf.Max(content.PushSeconds, 0f);
            var (pivot, zoom) = IllustrationFraming.ScaleAbout(
                environment.sprite != null ? environment.sprite.rect.size : Vector2.zero,
                world.rect.size, _from, content.AssemblyFraming);
            world.pivot = pivot;

            try
            {
                await Tween(seconds, t => world.localScale = Vector3.one * Mathf.Lerp(1f, zoom, t));
            }
            catch (OperationCanceledException)
            {
                return;
            }

            IsBusy = false;
            RefreshButton();
        }

        /// <summary>
        /// La fase confirmada: pulso breve de completado y martillazos (RF-41), y la fase queda
        /// en disco (RF-04, RNF-14). Con la última, la balsa terminada suena después del último
        /// martillazo y se sale al cruce.
        /// </summary>
        private async Awaitable ConfirmedAsync(RaftPhase phase, string message)
        {
            IsBusy = true;
            RefreshAll();
            _show(message, MessageTone.Done);
            _react?.Invoke(ActorAction.Celebrate);

            // El tiempo de resolución llega hasta la confirmación (RF-45): el reloj se para aquí,
            // no al acabar la animación.
            var indicators = _indicators.Complete();
            var last = _assembly.IsComplete;
            if (!last)
            {
                // Lo aprobado queda en disco al aprobarse, no al acabar los martillazos: una pausa
                // con «Reiniciar» a mitad de ellos no puede perderlo ni contar el paso dos veces
                // al repetirlo (RF-41, RF-45).
                Confirmed(phase, indicators);
            }

            try
            {
                await PulseAndHammerAsync();
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (last)
            {
                s_stash = null;
                s_indicators = null;
                // Un evento, un sonido (§2.3): la balsa terminada suena un golpe después del
                // último martillazo, no encima, y se sale al cruce cuando ya sonó. El gestor
                // sobrevive al cambio de escena y la pieza termina en la narrativa, como la
                // carretilla del taller.
                if (sounds != null && sounds.PhaseHitSeconds > 0f)
                {
                    try
                    {
                        await Awaitable.WaitForSecondsAsync(sounds.PhaseHitSeconds, destroyCancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }

                Play(sounds?.RaftBuilt);
                // La última se guarda al salir y no al aprobarse: con el nivel terminado en disco
                // el cruce —su cierre reflexivo— contaría como visto (CP-07), y quien saliera a
                // mitad de los martillazos lo podría omitir sin haberlo visto nunca.
                Confirmed(phase, indicators);
                IsBusy = false;
                Leave(content.ClosingSequenceId);
                return;
            }

            IsBusy = false;
            _hints.Activate(StepFor(_assembly.ActivePhase));
            RefreshAll();
        }

        /// <summary>La lista marca la fase y la fase queda en disco con sus indicadores.</summary>
        private void Confirmed(RaftPhase phase, PerformanceIndicators indicators)
        {
            _phaseConfirmed?.Invoke(phase);
            Persist(phase, indicators);
        }

        /// <summary>
        /// El pulso de completado y los martillazos de la fase aprobada: lo aprobado queda
        /// clavado y no se pierde (RF-41). Van en el mismo reloj y la fase no se suelta hasta el
        /// último golpe.
        /// </summary>
        /// <remarks>
        /// **Por qué no un martilleo suelto** como el del taller (<c>_ = HammerAsync()</c>): el panel
        /// quedaba libre al acabar el pulso con golpes por sonar, y la pieza siguiente se clavaba
        /// encima de ellos (§2.3, un evento, un sonido).
        /// </remarks>
        private async Awaitable PulseAndHammerAsync()
        {
            var hits = sounds != null ? sounds.PhaseHits : 0;
            var spacing = sounds != null ? sounds.PhaseHitSeconds : 0f;
            var seconds = Mathf.Max(CompletionPulseSeconds, (hits - 1) * spacing);
            var played = 0;
            var elapsed = 0f;
            while (true)
            {
                while (played < hits && elapsed >= played * spacing)
                {
                    Play(sounds.PhaseHammer);
                    played++;
                }

                var t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / CompletionPulseSeconds));
                raftArea.localScale = Vector3.one * Mathf.Lerp(1f, completionScale, Mathf.Sin(t * Mathf.PI));
                if (elapsed >= seconds && played >= hits)
                {
                    break;
                }

                await Awaitable.NextFrameAsync(destroyCancellationToken);
                elapsed += Time.deltaTime;
            }

            raftArea.localScale = Vector3.one;
        }

        /// <summary>
        /// La balsa gira y se hunde por un costado y vuelve (guion §1.8.4, Direccion_de_Arte
        /// §11). Después, la primera vez, la escena 3.2 (R12); las siguientes, solo el mensaje.
        /// </summary>
        private async Awaitable SinkAsync(string message, MessageTone tone)
        {
            IsBusy = true;
            RefreshButton();
            var seconds = Mathf.Max(content.SinkSeconds, 0f);
            var drop = raftArea.rect.height * 0.15f;

            try
            {
                await Tween(seconds, t =>
                {
                    var wave = Mathf.Sin(t * Mathf.PI);
                    raftArea.localRotation = Quaternion.Euler(0f, 0f, sinkAngle * wave);
                    raftArea.anchoredPosition = new Vector2(0f, -drop * wave);
                });
            }
            catch (OperationCanceledException)
            {
                return;
            }

            raftArea.localRotation = Quaternion.identity;
            raftArea.anchoredPosition = Vector2.zero;
            IsBusy = false;
            RefreshButton();
            _show(message, tone);

            var sequenceId = s_firstFailure.AfterAttempt(passed: false);
            if (sequenceId != null && Runner != null)
            {
                // La escena narra y al volver la fase está como quedó: lo puesto bien sigue puesto.
                s_stash = (_assembly.ActivePhase, _slots.Keys
                    .Where(id => _assembly.IsOpen(_slots[id].Slot) && _assembly.PlacedIn(id).HasValue)
                    .ToDictionary(id => id, id => _assembly.PlacedIn(id).Value));
                _indicators.PauseOpened(); // La escena 3.2 no suma tiempo de resolución (§3.6.1 nota 1).
                Leave(sequenceId);
            }
        }

        private async Awaitable Tween(float seconds, Action<float> apply)
        {
            var elapsed = 0f;
            do
            {
                elapsed += Time.deltaTime;
                var t = seconds > 0f ? Mathf.Clamp01(elapsed / seconds) : 1f;
                apply(Mathf.SmoothStep(0f, 1f, t));
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            } while (elapsed < seconds);
        }

        // --- persistencia y salida ---------------------------------------------------------------

        /// <summary>
        /// Guarda la fase confirmada con sus cuatro indicadores (RF-04, RF-45), que nunca ve el
        /// estudiante (CP-03). Los cierra quien confirma, al confirmar: cerrar el registro
        /// reinicia el reloj de la fase siguiente.
        /// </summary>
        private void Persist(RaftPhase phase, PerformanceIndicators indicators)
        {
            var profile = Runner?.Flow.ActiveProfile;
            if (profile == null)
            {
                return;
            }

            profile.ConfirmPhase(new PhaseId(LevelId.River, (int)phase), indicators);
            Runner.Session.SaveActive();
        }

        /// <summary>
        /// Sale a una escena narrativa. Sin <see cref="GameFlowRunner"/> —la escena se abrió
        /// suelta, sin pasar por <c>Boot</c>— avisa y se queda; si la secuencia no existe, cae al
        /// menú de niveles antes que dejar al estudiante sin salida (RNF-13).
        /// </summary>
        private void Leave(string sequenceId)
        {
            if (Runner == null)
            {
                Debug.LogWarning(
                    $"«{gameObject.scene.name}» se abrió sin pasar por «Boot»: no hay " +
                    $"{nameof(GameFlowRunner)}, así que el ensamblaje no navega a «{sequenceId}».", this);
                return;
            }

            if (!Runner.StartNarrative(sequenceId))
            {
                Runner.GoTo(GameState.LevelSelect);
            }
        }

        // --- la pintura --------------------------------------------------------------------------

        private void RestoreStash(RaftPhase phase)
        {
            if (s_stash.HasValue && s_stash.Value.Phase == phase)
            {
                foreach (var (slotId, kind) in s_stash.Value.Placed)
                {
                    _assembly.Place(slotId, kind);
                }
            }

            s_stash = null;
        }

        /// <summary>Un espacio por entrada del asset, colgado del área de la balsa en sus fracciones.</summary>
        private void BuildSlots()
        {
            slotTemplate.gameObject.SetActive(false);
            if (_slots.Count > 0)
            {
                return;
            }

            var side = raftArea.rect.size;
            foreach (var slot in content.Slots)
            {
                var image = Instantiate(slotTemplate, raftArea);
                // El nombre hace único el camino de jerarquía: sin él ninguna prueba puede señalarlo.
                image.name = $"Espacio_{slot.Id}";
                image.rectTransform.anchorMin = slot.Position;
                image.rectTransform.anchorMax = slot.Position;
                image.rectTransform.anchoredPosition = Vector2.zero;
                image.rectTransform.sizeDelta = Vector2.Scale(slot.Size, side);
                image.rectTransform.localRotation = Quaternion.Euler(0f, 0f, slot.Rotation);
                image.preserveAspect = true;

                var alert = image.transform.childCount > 0 ? image.transform.GetChild(0).GetComponent<Image>() : null;
                if (alert != null)
                {
                    alert.sprite = wrongIcon;
                    alert.color = wrongColor;
                    alert.raycastTarget = false;
                }

                var handle = image.GetComponent<RaftPieceHandle>();
                var id = slot.Id;
                handle.Taken += at => TakeFromSlot(id, at);
                handle.Released += Release;
                _slots[id] = (slot, image, alert, handle);
            }
        }

        /// <summary>Las casillas del inventario pasan a ser asas: de ahí se arrastra (RF-40).</summary>
        private void AttachInventoryHandles()
        {
            if (_handlesAttached)
            {
                return;
            }

            _handlesAttached = true;
            foreach (var kind in _inventory.Kinds)
            {
                var slot = inventory.SlotOf(kind);
                if (slot == null)
                {
                    continue;
                }

                // La casilla modelo no recibe raycast (durante la recolección solo se mira); como
                // asa tiene que recibir el clic o el EventSystem se lo entrega al panel de detrás
                // (fallo del 20/09/2026).
                slot.raycastTarget = true;
                var handle = slot.GetComponent<RaftPieceHandle>() ?? slot.gameObject.AddComponent<RaftPieceHandle>();
                var held = kind;
                handle.Taken += at => Take(held, at);
                handle.Released += Release;
            }
        }

        private void Hold(MaterialKind kind, Vector2 at, Vector2 size)
        {
            _ghost ??= BuildGhost();
            _held = kind;
            tracker.Seed(at);
            _ghost.sprite = content.ArtFor(kind)?.Placed;
            _ghost.rectTransform.sizeDelta = size;
            _ghost.rectTransform.SetAsLastSibling();
            _ghost.gameObject.SetActive(true);
            DragTo(at);
        }

        /// <summary>La pieza en la mano: una imagen suelta en la raíz del lienzo, por encima de todo y sin bloquear el puntero.</summary>
        private Image BuildGhost()
        {
            var ghost = new GameObject("Pieza_EnMano", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            ghost.rectTransform.SetParent(tracker.transform, false);
            ghost.preserveAspect = true;
            ghost.raycastTarget = false;
            return ghost;
        }

        private void RefreshAll()
        {
            RefreshSlots();
            RefreshInventory();
            RefreshButton();
        }

        /// <summary>
        /// Cada espacio según lo que dice la regla: oculto si su fase no llegó (RF-40), silueta si
        /// está vacío, la pieza si tiene una, y el icono de alerta si la validación lo señaló.
        /// </summary>
        private void RefreshSlots()
        {
            foreach (var (slot, image, alert, handle) in _slots.Values)
            {
                var visible = _assembly.IsVisible(slot);
                image.gameObject.SetActive(visible);
                if (!visible)
                {
                    continue;
                }

                var placed = _assembly.PlacedIn(slot.Id);
                var art = content.ArtFor(placed ?? slot.Accepts);
                image.sprite = placed.HasValue ? art?.Placed : content.ArtFor(slot.Accepts)?.Silhouette;
                image.color = Color.white;
                image.enabled = image.sprite != null;
                // Solo se agarra lo que está puesto en la fase abierta: lo confirmado es fijo (RF-41).
                image.raycastTarget = true;
                handle.enabled = placed.HasValue && _assembly.IsOpen(slot);
                if (alert != null)
                {
                    alert.gameObject.SetActive(_wrong.Contains(slot.Id));
                }
            }
        }

        private void RefreshInventory()
        {
            foreach (var kind in _inventory.Kinds)
            {
                var art = _inventory.Items.FirstOrDefault(item => item.Kind == kind)?.Art;
                inventory.Show(kind, art, _assembly.Remaining(kind));
            }
        }

        private void RefreshButton()
        {
            if (_assembly == null)
            {
                return;
            }

            confirmLabel.text = _assembly.ActivePhase == RaftPhase.MastAndSail ? content.TestLabel : content.ConfirmLabel;
            confirmButton.interactable = !IsBusy && !_assembly.IsComplete;
        }

        /// <summary>Cuelga un cuadro de la ilustración: centrado en una fracción de ella, con lado en fracción de su alto.</summary>
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

        /// <summary>Un efecto del ensamblaje. Sin gestor —la escena se abrió sin pasar por <c>Boot</c>— se juega en silencio.</summary>
        private static void Play(AudioClip clip)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(clip);
            }
        }

        /// <summary>La tarea del guía de cada fase, por su id en el asset.</summary>
        private GuideStep StepFor(RaftPhase phase)
        {
            var id = phase switch
            {
                RaftPhase.Base => "Base",
                RaftPhase.Lashing => "Amarre",
                _ => "MastilYVela"
            };
            return guide != null ? guide.Steps.FirstOrDefault(step => step.Id == id) : null;
        }
    }
}
