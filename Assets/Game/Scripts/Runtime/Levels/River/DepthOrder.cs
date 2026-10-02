using System.Collections.Generic;
using System.Linq;

namespace Game.Levels.River
{
    /// <summary>
    /// El orden de dibujo de lo que pisa la orilla: lo que está más abajo —más cerca de la
    /// cámara— tapa a lo que está más arriba (DA83, INC-118, lectura B).
    /// </summary>
    /// <remarks>
    /// Es la pareja de <see cref="RiverLevelConfig.DepthScaleAt"/>. La escala dice cuánto se ve
    /// de grande cada cosa según su altura; esto dice quién queda delante. Los materiales se
    /// instancian al final del entorno, así que sin ello tapaban a Mamá siempre, aunque los
    /// recogiera desde abajo; a la escala de la narrativa su cuerpo se mete delante o detrás de
    /// lo que recoge y el orden se nota.
    ///
    /// C# plano, sin <c>Transform</c>: el controlador le da cada cosa con su altura y pinta el
    /// orden que devuelve.
    /// </remarks>
    internal static class DepthOrder
    {
        /// <summary>
        /// De atrás hacia delante: lo más alto primero y lo más bajo al final, que es el orden de
        /// hermanos que uGUI dibuja de primero a último.
        /// </summary>
        /// <remarks>
        /// Estable a propósito, y por eso <c>OrderByDescending</c> y no <c>List.Sort</c>: a igual
        /// altura —la familia comparte los pies— se conserva el orden que traían las entradas. Un
        /// orden que cambiara entre cuadros reordenaría a los hermanos sin que nada se haya movido.
        /// </remarks>
        internal static List<T> BackToFront<T>(IEnumerable<(T Item, float Y)> entries) =>
            entries.OrderByDescending(entry => entry.Y).Select(entry => entry.Item).ToList();
    }
}
