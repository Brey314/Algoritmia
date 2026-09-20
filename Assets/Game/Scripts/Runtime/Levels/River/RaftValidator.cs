using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Levels.River
{
    /// <summary>
    /// La validación de una fase (RF-42, guion §1.8.4): señala **el espacio incorrecto**, no el
    /// ensamblaje completo. Un espacio está mal si está vacío o si lo que tiene no es lo que
    /// le corresponde.
    /// </summary>
    /// <remarks>
    /// Función pura, sin estado: <see cref="RaftAssembly"/> la llama al confirmar y aplica el
    /// resultado —devolver solo lo mal puesto y conservar lo aprobado (RF-43)—. Separada para que
    /// la regla de «qué está mal» se pruebe sola, sin fases ni inventario.
    /// </remarks>
    public static class RaftValidator
    {
        /// <param name="slots">Los espacios de la fase que se confirma.</param>
        /// <param name="placedIn">Qué hay en cada espacio, o nada.</param>
        public static IReadOnlyList<string> WrongSlots(IEnumerable<RaftSlot> slots, Func<string, MaterialKind?> placedIn) =>
            slots.Where(slot => placedIn(slot.Id) != slot.Accepts).Select(slot => slot.Id).ToArray();
    }
}
