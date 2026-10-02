using System;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// Las dos reglas de la lista «Tu secuencia» que no dependen de uGUI, para probarlas en
    /// EditMode: qué fila queda seleccionada tras retirar un bloque y hasta dónde se desplaza la
    /// lista para que el bloque en curso quede a la vista. <see cref="MazeSceneController"/> solo
    /// las aplica.
    /// </summary>
    internal static class SequenceListRules
    {
        /// <summary>
        /// La fila seleccionada —la única que lleva la papelera— tras retirar la de ese índice: la
        /// que ocupa su lugar, o la nueva última si se retiró la última; -1 si no queda ninguna.
        /// </summary>
        /// <remarks>
        /// **Siempre queda una**: con el cajón cerrado y pocos bloques las filas van desplegadas,
        /// no hay flecha «→» para elegir otra y la papelera del seleccionado es la única forma de
        /// retirar un bloque sin abrir el cajón. Dejar <c>-1</c> tras cada retiro (DEF-GP1-03)
        /// obligaba a abrir el cajón solo para recuperar la papelera. Retirar es editar y editar no
        /// cuesta (CP-02): pulsarla varias veces vacía la secuencia.
        /// </remarks>
        internal static int SelectionAfterRemoval(int removedIndex, int countAfter) =>
            countAfter <= 0 ? -1 : Math.Clamp(removedIndex, 0, countAfter - 1);

        /// <summary>
        /// Cuánto hay que desplazar la lista para que una fila quede dentro de su ventana, con un
        /// margen para que se vea el contorno del resaltado (DEF-GP1-02).
        /// </summary>
        /// <param name="scroll">Cuánto está desplazada ahora: distancia desde el borde superior de la lista al de la ventana.</param>
        /// <param name="itemTop">Distancia de la fila al borde superior de la lista.</param>
        /// <param name="itemBottom">Distancia de su borde inferior al borde superior de la lista.</param>
        /// <param name="viewport">Alto de la ventana.</param>
        /// <param name="margin">Lo que debe sobrar entre la fila y el borde de la ventana.</param>
        /// <returns>
        /// El desplazamiento nuevo, sin acotar: el llamador lo limita a lo que la lista permite. Si
        /// la fila ya se ve no cambia; si no cabe en la ventana, queda su borde superior.
        /// </returns>
        internal static float RevealScroll(float scroll, float itemTop, float itemBottom, float viewport, float margin)
        {
            if (itemBottom - itemTop + 2f * margin >= viewport || itemTop - margin < scroll)
            {
                return itemTop - margin;
            }

            return itemBottom + margin > scroll + viewport ? itemBottom + margin - viewport : scroll;
        }
    }
}
