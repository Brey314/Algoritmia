using System;

namespace Game.UI
{
    /// <summary>
    /// Páginas de la lista «Perfiles guardados» (DEF-W5R-01, RF-02): con más perfiles de los que
    /// caben en el panel, el resto se alcanza con «▲» y «▼» —clic, nunca arrastre ni rueda (RNF-02,
    /// CT-06)— y cada fila se dibuja entera dentro del panel. C# plano para probarlo en EditMode.
    /// </summary>
    internal static class ProfilePaging
    {
        /// <summary>Cuántas páginas hacen falta; siempre al menos una, aunque no haya perfiles.</summary>
        internal static int PageCount(int total, int perPage) =>
            total <= 0 ? 1 : (total + perPage - 1) / perPage;

        /// <summary>La página pedida, acotada a las que existen (tras borrar la última fila de la última página).</summary>
        internal static int ClampPage(int page, int total, int perPage) =>
            Math.Clamp(page, 0, PageCount(total, perPage) - 1);
    }
}
