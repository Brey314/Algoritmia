using System;

namespace Game.Scaffolding
{
    /// <summary>
    /// Los dos mecanismos de ayuda de RF-13: la instrucción a demanda y la pista tras tres
    /// fallos consecutivos.
    /// </summary>
    /// <remarks>
    /// C# plano, sin dependencias de Unity: la regla se prueba en EditMode, sin escena ni frames.
    /// La escena jugable solo traduce clics a <see cref="RequestHelp"/> y resultados de intento a
    /// <see cref="RegisterFailedAttempt"/> / <see cref="RegisterSuccessfulAttempt"/>.
    ///
    /// **Los dos mecanismos son distintos a propósito** (CP-06, HU-03): pedir ayuda repite la
    /// instrucción vigente y **no toca ningún contador** —si contara como intento, usar la ayuda
    /// adelantaría la pista y el andamiaje pasaría a resolver—; la pista llega sola, orienta y
    /// nunca nombra la respuesta. La razón es pedagógica: sin esta nota, una futura «mejora» que
    /// unifique ambos caminos vacía RF-13 de su mitad.
    /// </remarks>
    public class HintPolicy
    {
        /// <summary>«intentosParaPista» del guion §4.3.2: tres fallos consecutivos.</summary>
        public const int DefaultAttemptsForHint = 3;

        private readonly int _attemptsForHint;

        public HintPolicy(GuideStep step, int attemptsForHint = DefaultAttemptsForHint)
        {
            _attemptsForHint = Math.Max(1, attemptsForHint);
            ActiveStep = step;
        }

        /// <summary>La tarea activa. Solo hay una a la vez (RNF-03).</summary>
        public GuideStep ActiveStep { get; private set; }

        /// <summary>Fallos seguidos acumulados en la tarea activa.</summary>
        public int ConsecutiveFailures { get; private set; }

        /// <summary>
        /// Ayuda a demanda: devuelve la instrucción vigente sin alterar el estado de la partida
        /// (RF-13, HU-03 FA-02).
        /// </summary>
        public string RequestHelp() => ActiveStep?.Instruction ?? string.Empty;

        /// <summary>
        /// Registra un intento fallido. Devuelve la pista cuando el fallo es el tercero seguido
        /// —y reinicia el contador, guion §4.3.5 E5—, o <c>null</c> mientras no toque ofrecerla.
        /// </summary>
        public string RegisterFailedAttempt()
        {
            ConsecutiveFailures++;
            if (ConsecutiveFailures < _attemptsForHint)
            {
                return null;
            }

            ConsecutiveFailures = 0;
            return ActiveStep?.Hint;
        }

        /// <summary>
        /// Registra un intento efectivo: la racha de fallos vuelve a cero. Lo ganado no se pierde
        /// nunca (RF-18, CP-02); esto solo reinicia la cuenta de fallos seguidos.
        /// </summary>
        public void RegisterSuccessfulAttempt() => ConsecutiveFailures = 0;

        /// <summary>
        /// Pasa a la tarea siguiente. Los tres fallos son «en una misma tarea» (RF-13, HU-03), así
        /// que cambiar de tarea reinicia la cuenta.
        /// </summary>
        public void Activate(GuideStep step)
        {
            ActiveStep = step;
            ConsecutiveFailures = 0;
        }
    }
}
