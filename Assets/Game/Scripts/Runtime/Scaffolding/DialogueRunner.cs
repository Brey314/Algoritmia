using System;
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
    /// El cierre reflexivo **sí necesita una regla propia**, y aquí no está: se llega a él la
    /// primera vez justo después de confirmar la última fase, cuando «ya vio alguna escena de
    /// este nivel» es cierto pero la escena es nueva. Quien lo distingue es
    /// <see cref="NarrativeVisitPolicy"/>; para este runner sigue siendo un booleano de entrada.
    /// </remarks>
    public class DialogueRunner
    {
        private readonly IReadOnlyList<DialogueLine> _lines;
        private int _index;

        /// <summary>La línea más lejana que ya se mostró: lo que está por detrás de ella se vuelve a leer, no se vuelve a vivir.</summary>
        private int _furthest;

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
        /// Cuánto de la escena va leído, de 0 en la primera línea a 1 en la última o al terminar.
        /// Es lo que lleva la cámara por la ilustración **al ritmo de la narrativa** y no del reloj.
        /// </summary>
        public float Progress =>
            IsFinished || _lines.Count <= 1 ? 1f : (float)_index / (_lines.Count - 1);

        /// <summary>Índice de la línea en curso, desde 0. Es lo que dispara lo que se anima en cada línea.</summary>
        public int Index => _index;

        /// <summary>
        /// Si la línea en curso se muestra por primera vez. Falso al volver atrás (<see cref="Back"/>) y al
        /// avanzar de nuevo sobre líneas ya vistas: ahí se pinta quieta, sin repetir sonidos ni movimientos,
        /// para que un objeto movido no se mueva dos veces.
        /// </summary>
        public bool IsNewLine { get; private set; } = true;

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
            IsNewLine = _index > _furthest;
            _furthest = Math.Max(_furthest, _index);
            return true;
        }

        /// <summary>Si se puede volver a la línea anterior: no en la primera, ni con la escena terminada.</summary>
        public bool CanGoBack => !IsFinished && _index > 0;

        /// <summary>
        /// Vuelve a la línea anterior para releerla (INC-160). Es solo lectura: no cruza a la secuencia
        /// anterior —la primera línea es el tope— y no reabre una escena terminada. Lo que cada línea
        /// dispara (sonidos, movimientos) no se repite: lo decide quien pinta, con <see cref="IsNewLine"/>.
        /// </summary>
        public void Back()
        {
            if (!CanGoBack)
            {
                return;
            }

            _index--;
            IsNewLine = false;
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
