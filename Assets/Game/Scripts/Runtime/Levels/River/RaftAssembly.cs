using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Levels.River
{
    /// <summary>
    /// El panel de ensamblaje como lógica pura (RF-40..RF-43, guion §1.8.3–§1.8.4): tres fases
    /// sucesivas —base, amarre, mástil y vela—, los espacios de cada una, qué hay en cada
    /// espacio, y cuántas piezas de cada clase quedan por poner.
    /// </summary>
    /// <remarks>
    /// C# plano, sin escena: la escena traduce el soltar a <see cref="Place"/>, el sacar a
    /// <see cref="TakeBack"/> y el botón a <see cref="Confirm"/>, y pinta lo que se devuelve.
    ///
    /// **Solo se muestran los espacios de la fase activa y de las ya confirmadas**, nunca los de
    /// la siguiente: es lo que convierte el ensamblaje en descomposición y no en un rompecabezas
    /// de ensayo y error (guion §1.8.3). **Colocar no valida**: una pieza cabe en cualquier
    /// espacio abierto —el tronco largo entra en la base— y es el botón el que dice qué está mal
    /// (guion §1.8.4). Al rechazar, **solo lo mal puesto vuelve al inventario** y lo bien puesto
    /// se queda; una fase confirmada no se toca nunca más (RF-41, RF-43). No hay «deshacer» ni
    /// límite de intentos: la razón es pedagógica (CP-02), y quien añada cualquiera de los dos
    /// reintroduce la penalización que el proyecto prohíbe. <see cref="Rejections"/> existe solo
    /// para el indicador docente de RF-45 (R13), no para limitar.
    /// </remarks>
    public class RaftAssembly
    {
        private readonly RaftAssemblyContent _content;
        private readonly Dictionary<MaterialKind, int> _remaining;
        private readonly Dictionary<string, MaterialKind> _placed = new Dictionary<string, MaterialKind>();
        private readonly HashSet<RaftPhase> _confirmed = new HashSet<RaftPhase>();

        /// <param name="supply">Cuántas piezas de cada clase hay para poner: los troncos
        /// recogidos, los amarres del rollo, la tela y el mástil (<see cref="SupplyFrom"/>).</param>
        public RaftAssembly(RaftAssemblyContent content, IReadOnlyDictionary<MaterialKind, int> supply)
        {
            _content = content != null ? content : throw new ArgumentNullException(nameof(content));
            _remaining = supply.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>La fase abierta ahora. Tras la última confirmada sigue en ella, con <see cref="IsComplete"/>.</summary>
        public RaftPhase ActivePhase { get; private set; } = RaftPhase.Base;

        /// <summary>La balsa pasó la prueba: las tres fases están confirmadas (RF-42).</summary>
        public bool IsComplete => _confirmed.Count == 3;

        /// <summary>Cuántas veces se rechazó una confirmación. Solo para el informe docente (RF-45); nunca limita (CP-02).</summary>
        public int Rejections { get; private set; }

        public bool IsConfirmed(RaftPhase phase) => _confirmed.Contains(phase);

        /// <summary>Cuántas piezas de esa clase quedan en el inventario por poner.</summary>
        public int Remaining(MaterialKind kind) => _remaining.TryGetValue(kind, out var count) ? count : 0;

        /// <summary>Lo que hay en un espacio, o nada.</summary>
        public MaterialKind? PlacedIn(string slotId) => _placed.TryGetValue(slotId, out var kind) ? kind : (MaterialKind?)null;

        /// <summary>Los espacios de fases anteriores no se ven hasta llegar a ellas (RF-40).</summary>
        public bool IsVisible(RaftSlot slot) => slot.Phase <= ActivePhase;

        /// <summary>Solo los espacios de la fase activa admiten poner o sacar; los confirmados quedan fijos (RF-41).</summary>
        public bool IsOpen(RaftSlot slot) => slot.Phase == ActivePhase && !IsComplete;

        /// <summary>Los espacios que se ven ahora: la fase activa y las ya consolidadas.</summary>
        public IEnumerable<RaftSlot> VisibleSlots => _content.Slots.Where(IsVisible);

        /// <summary>
        /// Lo que se puede poner cuando el inventario está completo: cada tronco recogido es
        /// una pieza, el rollo de soga da <see cref="RaftAssemblyContent.KnotsPerRope"/> amarres,
        /// y la tela y el mástil una cada uno.
        /// </summary>
        public static Dictionary<MaterialKind, int> SupplyFrom(Inventory inventory, RaftAssemblyContent content) =>
            inventory.Items
                .GroupBy(item => item.Kind)
                .ToDictionary(group => group.Key,
                    group => group.Key == MaterialKind.Ropes ? group.Count() * content.KnotsPerRope : group.Count());

        /// <summary>
        /// Deja una pieza en un espacio. Cabe en cualquier espacio abierto y vacío: aquí no se
        /// valida nada, eso lo hace <see cref="Confirm"/> (guion §1.8.4).
        /// </summary>
        public Outcome Place(string slotId, MaterialKind kind)
        {
            var slot = Find(slotId);
            if (slot == null || !IsOpen(slot) || Remaining(kind) <= 0)
            {
                return new Outcome(false, string.Empty);
            }

            if (_placed.ContainsKey(slotId))
            {
                return new Outcome(false, _content.OccupiedMessage);
            }

            _placed[slotId] = kind;
            _remaining[kind]--;
            return new Outcome(true, _content.PlacedMessage);
        }

        /// <summary>Saca una pieza de un espacio abierto y la devuelve al inventario. Lo confirmado no se saca (RF-41).</summary>
        public Outcome TakeBack(string slotId)
        {
            var slot = Find(slotId);
            if (slot == null || !IsOpen(slot) || !_placed.TryGetValue(slotId, out var kind))
            {
                return new Outcome(false, string.Empty);
            }

            _placed.Remove(slotId);
            _remaining[kind]++;
            return new Outcome(true, _content.TakenBackMessage);
        }

        /// <summary>
        /// «Listo» o «Probar balsa»: valida la fase activa. Si pasa, la consolida y abre la
        /// siguiente; si no, señala los espacios incorrectos y devuelve **solo** esas piezas al
        /// inventario (RF-42, RF-43). La fase sigue abierta para otro intento (CP-02).
        /// </summary>
        public ValidationResult Confirm()
        {
            if (IsComplete)
            {
                return new ValidationResult(true, null, string.Empty);
            }

            var wrong = RaftValidator.WrongSlots(_content.SlotsOf(ActivePhase), PlacedIn);
            if (wrong.Count > 0)
            {
                foreach (var slotId in wrong)
                {
                    TakeBack(slotId); // Vacío no devuelve nada; mal puesto sí. Lo correcto se queda.
                }

                Rejections++;
                var message = ActivePhase == RaftPhase.MastAndSail ? _content.TestFailedMessage : _content.PhaseRejectedMessage;
                return new ValidationResult(false, wrong, message);
            }

            _confirmed.Add(ActivePhase);
            var confirmed = ActivePhase;
            if (confirmed != RaftPhase.MastAndSail)
            {
                ActivePhase = confirmed + 1;
            }

            return new ValidationResult(true, null, confirmed switch
            {
                RaftPhase.Base => _content.BaseConfirmedMessage,
                RaftPhase.Lashing => _content.LashingConfirmedMessage,
                _ => _content.TestPassedMessage
            });
        }

        /// <summary>
        /// Retoma en una fase (RNF-14): las anteriores quedan consolidadas con sus piezas puestas
        /// y descontadas del inventario, como si se hubieran confirmado en esta sesión.
        /// </summary>
        public void Resume(RaftPhase phase)
        {
            for (var previous = RaftPhase.Base; previous < phase; previous++)
            {
                foreach (var slot in _content.SlotsOf(previous))
                {
                    _placed[slot.Id] = slot.Accepts;
                    _remaining[slot.Accepts] = Remaining(slot.Accepts) - 1;
                }

                _confirmed.Add(previous);
            }

            ActivePhase = phase;
        }

        private RaftSlot Find(string slotId) => _content.Slots.FirstOrDefault(slot => slot.Id == slotId);
    }
}
