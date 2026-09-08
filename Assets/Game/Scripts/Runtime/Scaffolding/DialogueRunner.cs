using System.Collections.Generic;

namespace Game.Scaffolding
{
    /// <summary>
    /// Avanza una escena narrativa línea por línea (RF-05, RF-06).
    /// </summary>
    /// <remarks>
    /// C# plano, sin dependencias de Unity: el recorrido completo de una escena se prueba en
    /// EditMode, sin escena y sin frames. El adaptador de la escena solo traduce clics a
    /// <see cref="Advance"/> y pinta <see cref="Current"/>.
    ///
    /// **Omitir depende de si el perfil ya vio la escena** (INC-28, RF-06). Quién lo sabe es
    /// quien construye el runner: <c>PlayerProfile</c> guarda una lista cerrada —nombre, nivel
    /// alcanzado, fases confirmadas e indicadores (RNF-09, INC-27)— y no puede crecer con un
    /// registro de escenas vistas, así que «ya vista» se **deriva del progreso** en lugar de
    /// persistirse. Por eso llega como un dato de entrada y no se calcula aquí.
    ///
    /// El cierre reflexivo no necesita una regla propia: como cualquier otra escena, la primera
    /// vez no ofrece omitir, que es justo lo que piden CP-07 y RF-12 — es donde el guía nombra la
    /// habilidad practicada, y saltárselo vaciaría el nivel de su parte pedagógica.
    /// </remarks>
    public class DialogueRunner
    {
        private readonly IReadOnlyList<DialogueLine> _lines;
        private int _index;

        public DialogueRunner(IReadOnlyList<DialogueLine> lines, bool alreadySeen)
        {
            _lines = lines ?? new DialogueLine[0];
            CanSkip = alreadySeen;
            IsFinished = _lines.Count == 0;
        }

        /// <summary>La línea que se está mostrando. <c>null</c> si la escena ya terminó.</summary>
        public DialogueLine Current => IsFinished ? null : _lines[_index];

        /// <summary>Si ya no queda nada por leer.</summary>
        public bool IsFinished { get; private set; }

        /// <summary>Si se puede ofrecer el botón de omitir (INC-28).</summary>
        public bool CanSkip { get; }

        /// <summary>
        /// Pasa a la línea siguiente. Devuelve si quedaba alguna: en la última línea devuelve
        /// <c>false</c> y marca la escena como terminada, que es la señal para salir de ella.
        /// </summary>
        public bool Advance()
        {
            if (IsFinished)
            {
                return false;
            }

            if (_index + 1 >= _lines.Count)
            {
                IsFinished = true;
                return false;
            }

            _index++;
            return true;
        }

        /// <summary>
        /// Salta al final de la escena. Devuelve si estaba permitido: pedirlo sin permiso no
        /// hace nada y no lanza — un clic a destiempo no puede romper la escena.
        /// </summary>
        public bool Skip()
        {
            if (!CanSkip)
            {
                return false;
            }

            IsFinished = true;
            return true;
        }
    }
}
