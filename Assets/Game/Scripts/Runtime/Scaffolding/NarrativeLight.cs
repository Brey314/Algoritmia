using System;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// El estado de la capa de oscuridad en una parada (RF-05, `docs/Camara_Narrativa_N1.md` §4):
    /// dónde está la fuente de luz, cuánto abarca, cuánto se ve fuera de ella y de qué color.
    /// </summary>
    /// <remarks>
    /// Es contenido, no código: en el Nivel 1 casi todo ocurre a oscuras y lo que el jugador ve
    /// lo decide la luz, no el zoom. Por eso cada parada de cámara lleva su luz al lado. El valor
    /// por defecto es «plena luz, sin fuente, tinte blanco»: multiplicar por blanco no cambia
    /// nada, así que las secuencias que no declaran luz (todo el Nivel 2) se ven igual que antes.
    /// </remarks>
    [Serializable]
    public class NarrativeLight
    {
        [field: SerializeField]
        [field: Tooltip("Centro de la fuente, en fracciones de la ilustración: (0,0) abajo-izquierda, (1,1) arriba-derecha. Queda pegado al mundo, no a la pantalla.")]
        public Vector2 Center { get; private set; } = new Vector2(0.5f, 0.5f);

        [field: SerializeField]
        [field: Tooltip("Radio del charco, en altos de la ilustración. 0 = no hay fuente puntual.")]
        public float Radius { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Brillo fuera del charco («fondo»): 0 negro puro, 1 plena luz.")]
        public float Ambient { get; private set; } = 1f;

        [field: SerializeField]
        [field: Tooltip("Multiplicador de color: frío (0.55, 0.68, 1) para la noche, ámbar (1, 0.9, 0.72) para Chispa, fuego (1, 0.8, 0.55) para la hoguera.")]
        public Color Tint { get; private set; } = Color.white;

        [field: SerializeField]
        [field: Tooltip("Si es mayor que 0, este estado se aplica de golpe, dura esos segundos y la luz vuelve al estado de la parada anterior: el ¡CLIC! del sílex.")]
        public float FlashSeconds { get; private set; }

        public NarrativeLight()
        {
        }

        public NarrativeLight(Vector2 center, float radius, float ambient, Color tint, float flashSeconds = 0f)
        {
            Center = center;
            Radius = radius;
            Ambient = ambient;
            Tint = tint;
            FlashSeconds = flashSeconds;
        }

        public static NarrativeLight Lerp(NarrativeLight from, NarrativeLight to, float t) =>
            new NarrativeLight(
                Vector2.Lerp(from.Center, to.Center, t),
                Mathf.Lerp(from.Radius, to.Radius, t),
                Mathf.Lerp(from.Ambient, to.Ambient, t),
                Color.Lerp(from.Tint, to.Tint, t));
    }
}
