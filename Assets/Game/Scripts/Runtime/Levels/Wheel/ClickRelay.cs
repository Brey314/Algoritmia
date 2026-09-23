using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// Avisa de un clic simple sobre un gráfico y del punto de pantalla donde cayó.
    /// </summary>
    /// <remarks>
    /// Solo clic: no implementa ninguno de los manejadores de arrastre de uGUI, así que no compite
    /// con el clic sostenido de los bloques ni amplía el esquema de control (RNF-02, CT-06). Es lo
    /// que hace clicable la barra de desplazamiento de la secuencia sin convertirla en un
    /// <c>ScrollRect</c>.
    /// </remarks>
    public class ClickRelay : MonoBehaviour, IPointerClickHandler
    {
        public event Action<Vector2> Clicked;

        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(eventData.position);
    }
}
