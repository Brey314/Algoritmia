using System;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Texto de la pantalla de créditos (RF-08). Vive en un asset para poder editarlo sin
    /// recompilar (RNF-18) y para ir registrando ahí cada asset generado con su autoría
    /// (CT-09, RNF-23). El reconocimiento de los diseños de la Familia Anonaky es **obligatorio**:
    /// los personajes del Slice 1 son obra derivada de ellos, con autorización escrita concedida.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/UI/Credits Content", fileName = "CreditsContent")]
    public class CreditsContent : ScriptableObject
    {
        /// <summary>Una línea de la rejilla: el papel arriba y quién lo hizo debajo.</summary>
        [Serializable]
        public class Entry
        {
            [field: SerializeField]
            [field: Tooltip("El papel. Se pinta encima del nombre, en tono más claro.")]
            public string Role { get; private set; }

            [field: SerializeField]
            [field: Tooltip("Quién lo hizo.")]
            public string Name { get; private set; }
        }

        [field: SerializeField]
        [field: Tooltip("Los créditos en pares papel/persona. Se reparten en dos columnas.")]
        public Entry[] Entries { get; private set; } = Array.Empty<Entry>();

        [field: SerializeField, TextArea(3, 8)]
        [field: Tooltip("Reconocimientos que no son un par papel/persona. El de la Familia " +
                        "Anonaky no se puede quitar: los personajes son obra derivada suya.")]
        public string Body { get; private set; } =
            "Personajes basados en diseños de la Familia Anonaky, usados con autorización escrita.\n" +
            "Entornos, objetos e interfaz: originales del proyecto.";
    }
}
