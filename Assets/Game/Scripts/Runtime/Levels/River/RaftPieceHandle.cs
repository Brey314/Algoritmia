using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels.River
{
    /// <summary>
    /// El asa de una pieza de la balsa —una casilla del inventario o un espacio ya ocupado—:
    /// traduce el clic sostenido a «agarrar» y el soltar el clic a «soltar» (RF-40, RNF-02).
    /// </summary>
    /// <remarks>
    /// **Pulsar y soltar, no arrastrar.** Solo <see cref="IPointerDownHandler"/> e
    /// <see cref="IPointerUpHandler"/>, ninguna interfaz de arrastre de uGUI: el esquema
    /// radicado es clic y clic sostenido (RNF-02, CT-06), y la prueba del mapa de controles lo
    /// vigila. El seguimiento del cursor lo da <see cref="PointerTracker"/>. Es el mismo asa que
    /// la caja del Nivel 2 (<c>CargoHandle</c>), repetida aquí porque ningún nivel puede
    /// referenciar a otro (RNF-16).
    /// </remarks>
    public class RaftPieceHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>Se agarró la pieza, en esa posición de pantalla.</summary>
        public event Action<Vector2> Taken;

        /// <summary>Se soltó el clic, en esa posición de pantalla.</summary>
        public event Action<Vector2> Released;

        public void OnPointerDown(PointerEventData eventData) => Taken?.Invoke(eventData.position);

        public void OnPointerUp(PointerEventData eventData) => Released?.Invoke(eventData.position);
    }
}
