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
        [field: SerializeField, TextArea(8, 24)]
        [field: Tooltip("Texto completo de la pantalla de créditos.")]
        public string Body { get; private set; } =
            "Algoritm\n\n" +
            "Un proyecto de grado de\n" +
            "Santiago Benavides Rey\n" +
            "Santiago Valdiri García\n" +
            "Universidad Católica de Colombia · 2026\n\n" +
            "Personajes basados en diseños de la Familia Anonaky,\n" +
            "usados con autorización escrita.\n\n" +
            "Entornos, objetos e interfaz: originales del proyecto.";
    }
}
