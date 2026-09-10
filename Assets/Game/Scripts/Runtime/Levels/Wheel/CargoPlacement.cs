using System;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// La cola de la fase 1 del Nivel 2: colocar la caja de alimentos sobre los troncos alineados
    /// y empujarla para ver el rodado (RF-25, RF-26, guion §6.1.2).
    /// </summary>
    /// <remarks>
    /// C# plano y sin escena, igual que <see cref="PatternSelection"/>: la escena solo traduce el
    /// clic sostenido a <see cref="TryTake"/>, el soltar a <see cref="Drop"/> y el botón a
    /// <see cref="Push"/>. Aquí no hay ni una coordenada — dónde está la caja y si cayó sobre los
    /// troncos lo mide el adaptador, que es quien conoce la geometría.
    ///
    /// **Lo habilitado no se vuelve a deshabilitar.** <see cref="IsPlaced"/> solo pasa de falso a
    /// verdadero: volver a levantar la caja y dejarla en cualquier otro sitio **no** apaga
    /// «Empujar». La razón es pedagógica y no técnica — lo ganado permanece y ningún error
    /// posterior lo retira (CP-02, RF-41, mismo criterio que INC-32 en el Nivel 1). Quien
    /// «arregle» esto reintroduce la penalización que el proyecto prohíbe.
    /// </remarks>
    public class CargoPlacement
    {
        private readonly PatternSelection _selection;
        private readonly WheelLevelConfig _config;

        public CargoPlacement(PatternSelection selection, WheelLevelConfig config)
        {
            _selection = selection ?? throw new ArgumentNullException(nameof(selection));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        /// <summary>La caja no se mueve hasta reunir los cinco troncos (RF-25, CU-06 FA-4a).</summary>
        public bool CanDrag => _selection.IsComplete;

        /// <summary>La caja está agarrada ahora mismo: el clic sigue sostenido.</summary>
        public bool IsHeld { get; private set; }

        /// <summary>La caja quedó sobre los troncos alineados. Nunca vuelve a falso (CP-02).</summary>
        public bool IsPlaced { get; private set; }

        /// <summary>El rodado ya se demostró: la fase 1 está resuelta (RF-26).</summary>
        public bool IsPushed { get; private set; }

        /// <summary>«Empujar» se habilita solo con la caja colocada (RF-26).</summary>
        public bool CanPush => IsPlaced;

        /// <summary>Troncos que todavía faltan para poder mover la caja.</summary>
        public int PendingLogs => Math.Max(0, _selection.Required - _selection.Collected);

        /// <summary>
        /// Intenta agarrar la caja. Antes de los cinco troncos **no ejecuta nada** y responde con
        /// cuántos faltan: es un «todavía no», no un error (CU-06 FA-4a, CP-02).
        /// </summary>
        /// <remarks>
        /// Devuelve el mismo <see cref="PatternSelection.Outcome"/> que la selección para que la
        /// escena tenga una sola forma de pintar «qué pasó y qué se dice», en vez de dos.
        /// </remarks>
        public PatternSelection.Outcome TryTake()
        {
            if (!CanDrag)
            {
                return new PatternSelection.Outcome(false,
                    string.Format(_config.PendingLogsFormat, PendingLogs));
            }

            IsHeld = true;
            return new PatternSelection.Outcome(true, string.Empty);
        }

        /// <summary>
        /// Suelta la caja. <paramref name="overLogs"/> lo decide la escena midiendo si la caja
        /// quedó sobre la fila de troncos.
        /// </summary>
        public PatternSelection.Outcome Drop(bool overLogs)
        {
            if (!IsHeld)
            {
                return new PatternSelection.Outcome(false, string.Empty);
            }

            IsHeld = false;

            if (!overLogs)
            {
                // La caja se queda donde el estudiante la dejó: soltarla fuera no la devuelve a su
                // sitio, no cierra ningún camino y no penaliza (CP-02, RF-18).
                return new PatternSelection.Outcome(false, _config.CargoMissedMessage);
            }

            IsPlaced = true;
            return new PatternSelection.Outcome(true, _config.CargoPlacedMessage);
        }

        /// <summary>
        /// Acciona el empuje. Solo con la caja colocada y solo una vez: la segunda pulsación no
        /// hace nada, pero **el botón sigue habilitado** — deshabilitarlo retiraría algo ya
        /// conseguido (CP-02).
        /// </summary>
        public bool Push()
        {
            if (!CanPush || IsPushed)
            {
                return false;
            }

            IsPushed = true;
            return true;
        }
    }
}
