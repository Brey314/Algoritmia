using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// El vocabulario del panel de selección de perfil (HU-01, RF-02, RF-47): fuera del código
    /// (CT-05, RNF-18), como todo texto visible del proyecto.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/UI/Profile Select Content", fileName = "ProfileSelectContent")]
    public class ProfileSelectContent : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Aviso cuando se confirma el nombre sin escribir nada (HU-01 FA-01).")]
        public string EmptyNameMessage { get; private set; } = "Escribe un nombre para empezar.";

        [field: SerializeField]
        [field: Tooltip("Aviso cuando el nombre ya lo usa otro perfil del equipo (HU-01 FA-02).")]
        public string DuplicateNameMessage { get; private set; } = "Ya hay un perfil con ese nombre. Elige otro.";

        [field: SerializeField]
        [field: Tooltip("Aviso cuando el nombre no pasa la validación de PlayerProfile.Create.")]
        public string InvalidNameMessage { get; private set; } = "Ese nombre no se puede usar. Prueba con otro.";

        [field: SerializeField]
        [field: Tooltip("Formato de la pregunta de confirmación de borrado; {0} es el nombre del perfil (RF-47).")]
        public string DeletePromptFormat { get; private set; } = "¿Borras el perfil de {0}?";

        [field: SerializeField]
        [field: Tooltip("Aviso cuando el borrado deja rastro en disco (RNF-11): no admite «casi borrado».")]
        public string DeleteFailedMessage { get; private set; } = "No se pudo borrar del todo ese perfil. Avisa a tu profe.";

        [field: SerializeField]
        [field: Tooltip("Aviso cuando el archivo de un perfil no se puede leer (guardado interrumpido o dañado). Sin culpa ni cifras (CP-02, CP-03).")]
        public string UnreadableProfileMessage { get; private set; } = "No se pudo abrir ese perfil. Avisa a tu profe.";
    }
}
