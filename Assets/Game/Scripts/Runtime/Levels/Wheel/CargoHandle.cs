using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// El asa de la caja de alimentos: traduce el clic sostenido a «agarrar» y el soltar el clic a
    /// «soltar» (RF-25, RNF-02).
    /// </summary>
    /// <remarks>
    /// **Pulsar y soltar, no arrastrar.** Implementa solo <see cref="IPointerDownHandler"/> e
    /// <see cref="IPointerUpHandler"/> y ninguna interfaz de arrastre de uGUI a propósito: el
    /// esquema radicado es clic y clic sostenido (RNF-02, CT-06), y <c>EventTrigger</c> —que ya
    /// venía con uGUI— implementa el arrastre entero, así que usarlo habría hecho falsa la prueba
    /// que vigila ese esquema. El seguimiento del cursor mientras se sostiene lo hace
    /// <see cref="ForestSceneController"/> leyendo el Input System nuevo.
    ///
    /// El <c>EventSystem</c> entrega el soltar al mismo objeto que recibió el pulsar aunque el
    /// cursor ya no esté encima, así que soltar rápido fuera de la caja también la suelta.
    /// </remarks>
    public class CargoHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action Taken;
        public event Action Released;

        public void OnPointerDown(PointerEventData eventData) => Taken?.Invoke();

        public void OnPointerUp(PointerEventData eventData) => Released?.Invoke();
    }
}
