using Game.Core;
using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Una escena narrativa: una ilustración fija y sus líneas (RF-05, RNF-18).
    /// </summary>
    /// <remarks>
    /// **No es video** (RNF-06, peso del paquete): una ilustración con cuadros de texto
    /// secuenciales. Por eso añadir una escena narrativa es crear un asset y no una escena ni una
    /// rama del flujo — <c>GameState.Narrative</c> se parametriza con <see cref="Id"/>, que es lo
    /// que <c>GameFlow</c> guarda al entrar.
    ///
    /// <see cref="NextPhase"/> es la otra mitad de esa parametrización: **a dónde sale** la
    /// escena también es contenido. Sin ella el controlador tendría que saber por su cuenta que
    /// la escena 2.1 desemboca en el bosque y la 2.2 no, que es exactamente el <c>if</c> por
    /// secuencia que este diseño evita.
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritm/Secuencia narrativa", fileName = "N0_Secuencia")]
    public class NarrativeSequence : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Identificador con el que el flujo pide esta escena, p. ej. «N1_Apertura».")]
        public string Id { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Nivel al que pertenece. Decide si el perfil ya vio la escena (RF-06, INC-28).")]
        public LevelId Level { get; private set; } = LevelId.Fire;

        [field: SerializeField]
        [field: Tooltip("Ilustración fija de la escena. Puede quedar vacía mientras no exista el arte.")]
        public Sprite Illustration { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Marca el cierre reflexivo del nivel: no se puede omitir la primera vez (CP-07).")]
        public bool IsReflectiveClosing { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Fase de este nivel a la que se entra al terminar la escena. 0 = ninguna: vuelve al menú.")]
        public int NextPhase { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Las líneas, en el orden en que se leen.")]
        public DialogueLine[] Lines { get; private set; } = new DialogueLine[0];
    }
}
