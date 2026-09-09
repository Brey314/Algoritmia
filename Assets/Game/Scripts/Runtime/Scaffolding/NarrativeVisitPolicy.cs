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
        public static bool AlreadySeen(PlayerProfile profile, LevelId level) =>
            profile != null && profile.ConfirmedPhases.Any(phase => phase.Level == level);
    }
}
