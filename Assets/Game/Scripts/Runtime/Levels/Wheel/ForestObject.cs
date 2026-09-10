using System;
using UnityEngine;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// Las dos categorías del bosque vistas de cerca: el tronco redondo, que es el único válido,
    /// y las tres clases de distractor del guion §6.1.2.
    /// </summary>
    /// <remarks>
    /// La categoría es estructural —de ella cuelga el mensaje de rechazo, uno por categoría— y por
    /// eso vive en el código; lo que es parámetro ajustable, cuántos objetos hay y qué dice cada
    /// uno, vive en el asset (CT-05, RNF-18).
    /// </remarks>
    public enum ForestObjectCategory
    {
        RoundLog = 0,
        Stone = 1,
        Plant = 2,
        Tool = 3
    }

    /// <summary>
    /// Un objeto tirado por el bosque: su identidad y la categoría que decide si comparte el
    /// patrón (RF-23, guion §6.1.2).
    /// </summary>
    [Serializable]
    public class ForestObject
    {
        [field: SerializeField]
        [field: Tooltip("Identificador del objeto, p. ej. «tronco_1». Único dentro del catálogo.")]
        public string Id { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Categoría a la que pertenece. Solo «RoundLog» comparte el patrón.")]
        public ForestObjectCategory Category { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Cómo se nombra en pantalla. Varios objetos pueden compartirlo: es lo que hay que mirar, no leer.")]
        public string DisplayName { get; private set; }

        [field: SerializeField]
        [field: Tooltip("La ilustración del objeto. Cambiar el arte es repuntar este campo: no se toca código.")]
        public Sprite Art { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Dónde cae en el suelo, en fracción del área jugable: (0,0) abajo a la izquierda, (1,1) arriba a la derecha.")]
        public Vector2 FloorPosition { get; private set; } = new Vector2(0.5f, 0.5f);

        public ForestObject(string id, ForestObjectCategory category, string displayName = null,
            Sprite art = null, Vector2 floorPosition = default)
        {
            Id = id;
            Category = category;
            DisplayName = displayName ?? id;
            Art = art;
            FloorPosition = floorPosition == default ? new Vector2(0.5f, 0.5f) : floorPosition;
        }

        /// <summary>Requerido por la serialización de Unity.</summary>
        private ForestObject()
        {
        }
    }
}
