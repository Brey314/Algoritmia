using System;
using System.Collections.Generic;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// La regla de la fase 1 del Nivel 2: aceptar los objetos que comparten el patrón, devolver
    /// los demás a su sitio con una frase que describe lo ocurrido, y llevar la cuenta del acopio
    /// (RF-23, RF-24, RF-11, guion §6.1.2).
    /// </summary>
    /// <remarks>
    /// C# plano, sin dependencias de escena: la regla se prueba en EditMode, sin frames. La escena
    /// de W06 solo traduce clics a <see cref="Select"/> y el estado a la UI.
    ///
    /// **El patrón vive en el código y no en el asset a propósito.** Qué se pide acopiar —troncos
    /// redondos— es la mecánica misma del nivel, no un parámetro ajustable jugando: moverlo al
    /// asset no lo haría configurable, lo haría rompible. Lo que sí es parámetro —cuántos, cuáles
    /// y qué se dice— está todo en <see cref="WheelLevelConfig"/> (CT-05, RNF-18).
    /// </remarks>
    public class PatternSelection
    {
        /// <summary>
        /// La categoría que comparte el patrón. Todo lo demás es distractor (guion §6.1.2).
        /// </summary>
        public const ForestObjectCategory ValidCategory = ForestObjectCategory.RoundLog;

        private readonly WheelLevelConfig _config;
        private readonly HashSet<string> _collectedIds = new HashSet<string>();

        public PatternSelection(WheelLevelConfig config)
        {
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        /// <summary>Troncos redondos ya acopiados.</summary>
        public int Collected => _collectedIds.Count;

        /// <summary>Cuántos hay que acopiar para terminar la fase.</summary>
        public int Required => _config.RequiredLogs;

        /// <summary>La fase termina al completar el acopio; no hay otra forma de salir (CP-02).</summary>
        public bool IsComplete => Collected >= Required;

        /// <summary>
        /// El contador permanente de RF-24: «Troncos redondos: n de 5».
        /// </summary>
        /// <remarks>
        /// **Es la única cifra permitida de la fase y no contradice CP-03**: dice cuánto falta de
        /// la tarea, no qué tan bien lo hizo el estudiante. RF-24 la exige de forma permanente.
        /// Quien venga a «limpiar cifras» por CP-03 debe dejar esta en pie.
        /// </remarks>
        public string CounterText => string.Format(_config.CounterFormat, Collected, Required);

        /// <summary>
        /// Registra un clic sobre un objeto del bosque y devuelve lo que hay que mostrar.
        /// </summary>
        public Outcome Select(ForestObject forestObject)
        {
            if (forestObject == null)
            {
                return new Outcome(false, string.Empty);
            }

            // Ya está en la zona de acopio: el clic no suma, no resta y no dice nada. Sin mensaje
            // porque no hubo intento que describir, y sin penalización porque nunca la hay (CP-02).
            if (_collectedIds.Contains(forestObject.Id))
            {
                return new Outcome(false, string.Empty);
            }

            var message = _config.MessageFor(forestObject.Category);
            if (forestObject.Category != ValidCategory)
            {
                // El distractor vuelve a su posición original: rechazar no retira el objeto del
                // escenario ni cierra ningún camino (RF-18, CP-02).
                return new Outcome(false, message);
            }

            _collectedIds.Add(forestObject.Id);
            return new Outcome(true, message);
        }

        /// <summary>
        /// Lo que produce una selección: si el objeto se acopió y la frase que lo describe.
        /// </summary>
        /// <remarks>
        /// El mensaje del acierto también es narrativo —«Este rueda. ¿Qué tiene que los otros no
        /// tienen?»— y no una felicitación: la retroalimentación describe lo ocurrido y no
        /// califica (RF-11, RF-17).
        /// </remarks>
        public readonly struct Outcome
        {
            public Outcome(bool accepted, string message)
            {
                Accepted = accepted;
                Message = message ?? string.Empty;
            }

            /// <summary>El objeto compartía el patrón y sumó al acopio.</summary>
            public bool Accepted { get; }

            /// <summary>La frase a mostrar, o vacía cuando no hay nada que decir.</summary>
            public string Message { get; }
        }
    }
}
