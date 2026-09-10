using System.Linq;
using Game.Core;

namespace Game.Scaffolding
{
    /// <summary>
    /// Decide si un perfil **ya vio** las escenas narrativas de un nivel (RF-06, INC-28).
    /// </summary>
    /// <remarks>
    /// No hay un registro de escenas vistas y no puede haberlo: lo que se persiste es una lista
    /// cerrada —nombre, nivel alcanzado, fases confirmadas y los cuatro indicadores (RNF-09,
    /// INC-27)— y ampliarla sería cambiar un entregable ya radicado. Así que «ya vista» se
    /// **deriva del progreso**: si el perfil confirmó alguna fase del nivel, ya pasó por sus
    /// escenas antes y el botón de omitir tiene sentido.
    ///
    /// La consecuencia buscada es la de HU-14: la primera vuelta se lee entera —es donde el guía
    /// nombra la habilidad practicada (RF-12, CP-07)— y solo quien repite puede saltar.
    /// </remarks>
    public static class NarrativeVisitPolicy
    {
        public static bool AlreadySeen(PlayerProfile profile, NarrativeSequence sequence)
        {
            if (profile == null || sequence == null)
            {
                return false;
            }

            // El cierre reflexivo no puede usar la misma señal que las demás escenas del nivel:
            // se llega a él la primera vez **justo después** de confirmar la última fase, así que
            // «confirmó alguna fase» ya es cierto y ofrecería omitir precisamente donde CP-07 y
            // RF-12 lo prohíben. Lo que sí distingue la primera vuelta de las siguientes es que
            // el nivel siguiente aún no está desbloqueado: eso solo ocurre al terminar el nivel
            // (HU-14 FA-01/FA-02), y se deriva de la lista cerrada sin ampliarla (RNF-09).
            //
            // Esto le impone un orden a quien cierre el nivel (T18 en el N1, W16 en el N2):
            // primero el cierre reflexivo, después el desbloqueo. Al revés, el botón de omitir
            // aparecería en la primera vuelta.
            if (sequence.IsReflectiveClosing)
            {
                return profile.ReachedLevel > sequence.Level;
            }

            return profile.ConfirmedPhases.Any(phase => phase.Level == sequence.Level);
        }
    }
}
