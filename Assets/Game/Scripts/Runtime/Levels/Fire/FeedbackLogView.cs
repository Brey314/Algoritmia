using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Pinta el historial acumulado del área de registro (guion §4.3.1, HU-06): una línea por
    /// intento, en orden, con el último visible. El <see cref="ScrollRect"/> absorbe el
    /// crecimiento — no hay tope de intentos (RF-18).
    /// </summary>
    /// <remarks>
    /// Vista tonta: recibe las cadenas ya elegidas por <see cref="FireFeedbackLog"/> (C# plano) y
    /// solo las muestra. Genérica —no conoce ningún tipo del nivel— pero vive en
    /// <c>Game.Levels.Fire</c> hasta que otro nivel la necesite.
    /// </remarks>
    public class FeedbackLogView : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Etiqueta dentro del Content del ScrollRect donde se escriben las líneas.")]
        private Text lines;

        [SerializeField]
        [Tooltip("El ScrollRect que contiene el registro.")]
        private ScrollRect scroll;

        /// <summary>Muestra todo el historial y baja el scroll hasta la última línea.</summary>
        public void Show(IReadOnlyList<string> entries)
        {
            lines.text = string.Join("\n", entries);

            // El ContentSizeFitter del Content necesita un recálculo inmediato para que la última
            // línea quede visible al bajar el scroll en el mismo frame.
            Canvas.ForceUpdateCanvases();
            if (scroll != null)
            {
                scroll.verticalNormalizedPosition = 0f;
            }
        }
    }
}
