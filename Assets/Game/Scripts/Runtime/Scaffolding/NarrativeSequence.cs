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
        [field: Tooltip("Ilustración de la escena: el entorno por el que se mueve la cámara. Puede quedar vacía mientras no exista el arte.")]
        public Sprite Illustration { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Encuadre de la cámara al abrir la escena.")]
        public CameraFraming CameraStart { get; private set; } = new CameraFraming();

        [field: SerializeField]
        [field: Tooltip("Encuadre al que llega la cámara en la última línea. Igual al inicial = plano fijo. Se ignora si hay paradas.")]
        public CameraFraming CameraEnd { get; private set; } = new CameraFraming();

        [field: SerializeField]
        [field: Tooltip("Paradas de la cámara por línea: en cada una la cámara va al encuadre y se queda hasta la siguiente parada. Con paradas, el inicial vale hasta la primera y el final se ignora.")]
        public CameraKey[] CameraKeys { get; private set; } = new CameraKey[0];

        [field: SerializeField]
        [field: Tooltip("Segundos que tarda la cámara en recorrer dos tercios del camino hasta su encuadre. Corto, los pasos se leen sueltos; largo, un solo gesto continuo.")]
        public float CameraSmoothingSeconds { get; private set; } = 1.3f;

        [field: SerializeField]
        [field: Tooltip("Objetos pintados sobre el entorno, para que lo que dice el texto se vea. Acompañan a la cámara.")]
        public NarrativeProp[] Props { get; private set; } = new NarrativeProp[0];

        [field: SerializeField]
        [field: Tooltip("Marca el cierre reflexivo del nivel: no se puede omitir la primera vez (CP-07).")]
        public bool IsReflectiveClosing { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Fase de este nivel a la que se entra al terminar la escena. 0 = ninguna: vuelve al menú.")]
        public int NextPhase { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Las líneas, en el orden en que se leen.")]
        public DialogueLine[] Lines { get; private set; } = new DialogueLine[0];

#if UNITY_EDITOR
        /// <summary>
        /// Avisa en la consola de los encuadres que no se van a ver como están escritos. Es lo
        /// que habría cazado al escribirlos los tres encuadres recortados del Nivel 2
        /// (<c>Camara_Narrativa_N2.md</c> §1). Aviso y no error: la escena se reproduce igual.
        /// </summary>
        private void OnValidate()
        {
            var forest = Level == LevelId.Wheel;
            CameraFraming previous = null;
            var stops = new System.Collections.Generic.List<(string Name, CameraFraming Framing)> { ("inicio", CameraStart) };
            if (CameraKeys.Length == 0)
            {
                stops.Add(("final", CameraEnd));
            }

            foreach (var key in CameraKeys)
            {
                stops.Add(($"línea {key.Line}", key.Framing));
            }

            foreach (var (stop, framing) in stops)
            {
                foreach (var warning in IllustrationFraming.Warnings(framing, previous, forest))
                {
                    Debug.LogWarning($"{name} · {stop}: {warning}", this);
                }

                previous = framing;
            }
        }
#endif
    }
}
