using UnityEngine;

namespace Game.Reporting
{
    /// <summary>
    /// El tiempo de resolución se persiste en segundos y se presenta en minutos y segundos
    /// (Slice 4, pregunta abierta 4): es la única unidad legible para un docente en la tabla.
    /// </summary>
    public static class ResolutionTimeFormat
    {
        public static string MinutesAndSeconds(float seconds)
        {
            var total = Mathf.Max(0, Mathf.RoundToInt(seconds));
            var minutes = total / 60;
            var remaining = total % 60;
            return $"{minutes}:{remaining:D2}";
        }
    }
}
