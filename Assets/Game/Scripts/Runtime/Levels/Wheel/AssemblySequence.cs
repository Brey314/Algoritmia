using System;
using System.Collections.Generic;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// La máquina de ensamblaje de la fase 2 del Nivel 2 (RF-28, RF-29, guion §6.2.2): seis
    /// piezas, cuatro pasos con condición de habilitación, y un rechazo que dice **qué falta
    /// antes** en vez de qué está mal.
    /// </summary>
    /// <remarks>
    /// C# plano y sin escena, igual que <see cref="PatternSelection"/>: la escena traduce el clic
    /// a <see cref="Select"/>, el botón a <see cref="Machine"/> y el soltar a <see cref="Place"/>.
    /// Aquí no hay ni una coordenada: si la pieza cayó sobre el lugar de armado lo mide el
    /// adaptador, que es quien conoce la geometría.
    ///
    /// **El estado solo avanza.** Un paso fuera de secuencia no se ejecuta y no toca nada de lo
    /// hecho: perforar una rueda no se pierde por intentar mal el eje después. La razón es
    /// pedagógica y no técnica (CP-02, RF-41): quien añada un «deshacer» al rechazo reintroduce
    /// la penalización que el proyecto prohíbe.
    /// </remarks>
    public class AssemblySequence
    {
        private readonly AssemblyContent _content;
        private readonly HashSet<WorkshopPiece> _drilled = new HashSet<WorkshopPiece>();

        public AssemblySequence(AssemblyContent content)
        {
            _content = content != null ? content : throw new ArgumentNullException(nameof(content));
        }

        /// <summary>El tronco corto resaltado ahora mismo, si lo hay (guion §6.2.2 paso 1).</summary>
        public WorkshopPiece? Selected { get; private set; }

        /// <summary>El último paso completado. Solo avanza (CP-02).</summary>
        public AssemblyStep LastCompleted { get; private set; } = AssemblyStep.None;

        /// <summary>Cuántos troncos cortos ya son ruedas (0..2).</summary>
        public int DrilledWheels => _drilled.Count;

        /// <summary>«Mecanizar» solo con un tronco corto macizo seleccionado (RF-28, CU-07 FA-3a).</summary>
        public bool CanMachine => Selected.HasValue && !_drilled.Contains(Selected.Value);

        public bool IsAxleFormed => LastCompleted >= AssemblyStep.Axle;

        public bool IsPlankPlaced => LastCompleted >= AssemblyStep.Plank;

        /// <summary>La carretilla está completa: la fase 2 queda resuelta (RF-29).</summary>
        public bool IsComplete => LastCompleted == AssemblyStep.Cargo;

        public bool IsDrilled(WorkshopPiece piece) => _drilled.Contains(piece);

        /// <summary>Los troncos cortos se seleccionan con clic; el resto de piezas no.</summary>
        public static bool IsShortLog(WorkshopPiece piece) =>
            piece == WorkshopPiece.ShortLogA || piece == WorkshopPiece.ShortLogB;

        /// <summary>Solo el tronco largo, la tabla y la caja se arrastran (guion §6.2.2 pasos 4–6).</summary>
        public static bool IsDraggable(WorkshopPiece piece) =>
            piece == WorkshopPiece.LongLog || piece == WorkshopPiece.Plank || piece == WorkshopPiece.Cargo;

        /// <summary>
        /// Clic sobre una pieza. Un tronco corto queda resaltado —siempre disponible—; cualquier
        /// otra pieza no responde al clic y no hay nada que decir.
        /// </summary>
        public PatternSelection.Outcome Select(WorkshopPiece piece)
        {
            if (!IsShortLog(piece))
            {
                return new PatternSelection.Outcome(false, string.Empty);
            }

            Selected = piece;
            return new PatternSelection.Outcome(true,
                _drilled.Contains(piece) ? _content.AlreadyWheelMessage : _content.SelectedMessage);
        }

        /// <summary>«Mecanizar»: abre el agujero central del tronco resaltado, que pasa a ser rueda.</summary>
        public PatternSelection.Outcome Machine()
        {
            if (!CanMachine)
            {
                return new PatternSelection.Outcome(false,
                    Selected.HasValue ? _content.AlreadyWheelMessage : _content.SelectNeededMessage);
            }

            _drilled.Add(Selected.Value);
            Selected = null;
            Advance(_drilled.Count == 1 ? AssemblyStep.FirstWheel : AssemblyStep.SecondWheel);
            return new PatternSelection.Outcome(true,
                _drilled.Count == 2 ? _content.BothWheelsMessage : _content.WheelDrilledMessage);
        }

        /// <summary>
        /// Soltar una pieza arrastrada. <paramref name="overAssembly"/> lo decide la escena
        /// midiendo si cayó sobre el lugar de armado. Fuera de secuencia no se ejecuta y el mensaje
        /// dice qué falta antes (RF-29, CP-06); el adaptador devuelve la pieza a su sitio.
        /// </summary>
        public PatternSelection.Outcome Place(WorkshopPiece piece, bool overAssembly)
        {
            if (!IsDraggable(piece))
            {
                return new PatternSelection.Outcome(false, string.Empty);
            }

            if (!overAssembly)
            {
                return new PatternSelection.Outcome(false, _content.MissedMessage);
            }

            switch (piece)
            {
                case WorkshopPiece.LongLog when _drilled.Count < 2:
                    return new PatternSelection.Outcome(false, _content.AxleTooEarlyMessage);
                case WorkshopPiece.LongLog:
                    Advance(AssemblyStep.Axle);
                    return new PatternSelection.Outcome(true, _content.AxleFormedMessage);
                case WorkshopPiece.Plank when !IsAxleFormed:
                    return new PatternSelection.Outcome(false, _content.PlankTooEarlyMessage);
                case WorkshopPiece.Plank:
                    Advance(AssemblyStep.Plank);
                    return new PatternSelection.Outcome(true, _content.PlankPlacedMessage);
                case WorkshopPiece.Cargo when !IsPlankPlaced:
                    return new PatternSelection.Outcome(false, _content.CargoTooEarlyMessage);
                default:
                    Advance(AssemblyStep.Cargo);
                    return new PatternSelection.Outcome(true, _content.CompleteMessage);
            }
        }

        /// <summary>Nunca retrocede: soltar la caja dos veces no vuelve a «tabla montada».</summary>
        private void Advance(AssemblyStep step)
        {
            if (step > LastCompleted)
            {
                LastCompleted = step;
            }
        }
    }
}
