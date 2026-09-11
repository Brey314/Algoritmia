using System;
using System.Linq;
using Game.Scaffolding;
using UnityEngine;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// La retroalimentación narrativa que corresponde a una categoría de objeto
    /// (guion §6.1.2, RF-11, RF-17).
    /// </summary>
    [Serializable]
    public class CategoryFeedback
    {
        [field: SerializeField]
        [field: Tooltip("Categoría a la que responde este mensaje.")]
        public ForestObjectCategory Category { get; private set; }

        [field: SerializeField, TextArea(2, 4)]
        [field: Tooltip("Lo que dice el juego al seleccionar un objeto de esa categoría. Sin cifras (CP-03, RF-17).")]
        public string Message { get; private set; }

        public CategoryFeedback(ForestObjectCategory category, string message)
        {
            Category = category;
            Message = message;
        }

        /// <summary>Requerido por la serialización de Unity.</summary>
        private CategoryFeedback()
        {
        }
    }

    /// <summary>
    /// Los parámetros de la fase 1 del Nivel 2 — el bosque — fuera del código (CT-05, RNF-18):
    /// cuántos troncos se piden, cuántos distractores acompañan, el catálogo y qué dice cada
    /// categoría.
    /// </summary>
    /// <remarks>
    /// Un asset por nivel, como <see cref="Game.Scaffolding.GuideContent"/>: ajustar la proporción
    /// de objetos o corregir un mensaje es editar contenido, no recompilar.
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritm/Configuración del Nivel 2", fileName = "N2_WheelLevelConfig")]
    public class WheelLevelConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Troncos redondos que hay que acopiar para completar la fase (guion §6.1.2).")]
        public int RequiredLogs { get; private set; } = 5;

        [field: SerializeField]
        [field: Tooltip("Distractores mínimos que acompañan a los troncos en el escenario.")]
        public int MinimumDistractors { get; private set; } = 8;

        [field: SerializeField]
        [field: Tooltip("Texto del contador permanente. {0} son los acopiados y {1} los requeridos (RF-24).")]
        public string CounterFormat { get; private set; } = "Troncos redondos: {0} de {1}";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Lo que se dice al intentar mover la caja antes de tiempo. {0} son los troncos que faltan (CU-06 FA-4a).")]
        public string PendingLogsFormat { get; private set; } = "Todavía faltan {0} troncos para mover la caja.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Lo que se dice al dejar la caja sobre los troncos alineados (RF-25).")]
        public string CargoPlacedMessage { get; private set; } = "La caja quedó sobre los troncos. Ahora empújala.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Lo que se dice al soltar la caja fuera de los troncos. Describe, no regaña (CP-02).")]
        public string CargoMissedMessage { get; private set; } = "Ahí la caja no toca los troncos. Déjala encima de ellos.";

        [field: SerializeField]
        [field: Tooltip("Cuánto dura la demostración del rodado, en segundos (RF-26).")]
        public float RollSeconds { get; private set; } = 2.5f;

        [field: SerializeField]
        [field: Tooltip("Cuánto tarda la caja en caer al suelo al pasar el último tronco, en segundos.")]
        public float FallSeconds { get; private set; } = 0.45f;

        [field: SerializeField]
        [field: Tooltip("Secuencia narrativa a la que sale el bosque tras el rodado (guion §6.1.3). A dónde sale una fase es contenido, no código (RF-05).")]
        public string ClosingSequenceId { get; private set; } = "N2_Escena22_ElPatron";

        [field: SerializeField]
        [field: Tooltip("Escala de un objeto en el borde alto del suelo, el más lejano. El borde bajo es 1: cuanto más arriba, más pequeño.")]
        public float FarScale { get; private set; } = 0.6f;

        [field: SerializeField]
        [field: Tooltip("Cuánto crece un objeto con el cursor encima (1.15 = un 15 %). Es realce, no selección: el clic sigue siendo el único control.")]
        public float HoverScale { get; private set; } = 1.15f;

        [field: SerializeField]
        [field: Tooltip("A qué distancia del cursor empieza a crecer, en fracción del suelo. Cero lo apaga.")]
        public float HoverRadius { get; private set; } = 0.15f;

        [field: SerializeField]
        [field: Tooltip("Cuánto dura la transición al completar el acopio: los troncos vuelan a la caja y la cámara se acerca, en segundos.")]
        public float TransitionSeconds { get; private set; } = 1.2f;

        [field: SerializeField]
        [field: Tooltip("Plano general fijo de toda la recolección. Igual al último encuadre de la escena 2.1: de la narrativa al juego no hay salto.")]
        public CameraFraming PlayFraming { get; private set; } = new CameraFraming();

        [field: SerializeField]
        [field: Tooltip("Encuadre en el que termina el acercamiento al completar el acopio. Igual al primero de la escena 2.2, que lo hereda sin moverse.")]
        public CameraFraming CompletionFraming { get; private set; } = new CameraFraming();

        [field: SerializeField]
        [field: Tooltip("Los objetos que se reparten por el bosque, válidos y distractores.")]
        public ForestObject[] ForestObjects { get; private set; } = Array.Empty<ForestObject>();

        [field: SerializeField]
        [field: Tooltip("Un mensaje por categoría, incluida la del acierto (guion §6.1.2).")]
        public CategoryFeedback[] Feedback { get; private set; } = Array.Empty<CategoryFeedback>();

        [field: SerializeField]
        [field: Tooltip("Cómo reacciona cada categoría al paso del cursor. Uno por categoría; es tacto y se ajusta jugando.")]
        public NudgeSettings[] Nudges { get; private set; } = Array.Empty<NudgeSettings>();

        /// <summary>
        /// El mensaje de una categoría, o cadena vacía si el asset todavía no lo tiene. Vacío y no
        /// excepción: un contenido incompleto deja al estudiante sin frase, nunca sin partida
        /// (CP-02).
        /// </summary>
        public string MessageFor(ForestObjectCategory category) =>
            Feedback.FirstOrDefault(entry => entry.Category == category)?.Message ?? string.Empty;

        /// <summary>
        /// Cómo se aparta esa categoría al paso del cursor, o <c>null</c> si el asset todavía no lo
        /// dice. Nulo y no excepción, y quien lo consuma se limita a no animar: el movimiento es
        /// adorno, y un adorno que falta nunca puede dejar sin partida a nadie (CP-02).
        /// </summary>
        public NudgeSettings NudgeFor(ForestObjectCategory category) =>
            Nudges.FirstOrDefault(entry => entry.Category == category);

#if UNITY_INCLUDE_TESTS
        /// <summary>
        /// Construye una configuración en memoria. Solo para pruebas: es lo que permite verificar
        /// que la lógica sigue al asset y no a un literal (RNF-18) sin ampliar la superficie
        /// pública del módulo.
        /// </summary>
        internal static WheelLevelConfig Create(
            int requiredLogs,
            int minimumDistractors,
            string counterFormat,
            ForestObject[] forestObjects,
            CategoryFeedback[] feedback,
            string pendingLogsFormat = "faltan {0}",
            string cargoPlacedMessage = "colocada",
            string cargoMissedMessage = "fuera",
            float rollSeconds = 0.1f,
            string closingSequenceId = "cierre")
        {
            var config = CreateInstance<WheelLevelConfig>();
            config.RequiredLogs = requiredLogs;
            config.MinimumDistractors = minimumDistractors;
            config.CounterFormat = counterFormat;
            config.ForestObjects = forestObjects;
            config.Feedback = feedback;
            config.PendingLogsFormat = pendingLogsFormat;
            config.CargoPlacedMessage = cargoPlacedMessage;
            config.CargoMissedMessage = cargoMissedMessage;
            config.RollSeconds = rollSeconds;
            config.ClosingSequenceId = closingSequenceId;
            return config;
        }
#endif

#if UNITY_EDITOR
        /// <summary>Los dos encuadres del bosque pasan las mismas reglas que las paradas narrativas.</summary>
        private void OnValidate()
        {
            foreach (var (stop, framing, previous) in new[] { ("juego", PlayFraming, (CameraFraming)null), ("cierre", CompletionFraming, PlayFraming) })
            {
                foreach (var warning in IllustrationFraming.Warnings(framing, previous, mirroredForest: true))
                {
                    Debug.LogWarning($"{name} · {stop}: {warning}", this);
                }
            }
        }
#endif
    }
}
