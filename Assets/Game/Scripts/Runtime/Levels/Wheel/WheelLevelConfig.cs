using System;
using System.Linq;
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

        [field: SerializeField]
        [field: Tooltip("Los objetos que se reparten por el bosque, válidos y distractores.")]
        public ForestObject[] ForestObjects { get; private set; } = Array.Empty<ForestObject>();

        [field: SerializeField]
        [field: Tooltip("Un mensaje por categoría, incluida la del acierto (guion §6.1.2).")]
        public CategoryFeedback[] Feedback { get; private set; } = Array.Empty<CategoryFeedback>();

        /// <summary>
        /// El mensaje de una categoría, o cadena vacía si el asset todavía no lo tiene. Vacío y no
        /// excepción: un contenido incompleto deja al estudiante sin frase, nunca sin partida
        /// (CP-02).
        /// </summary>
        public string MessageFor(ForestObjectCategory category) =>
            Feedback.FirstOrDefault(entry => entry.Category == category)?.Message ?? string.Empty;

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
            CategoryFeedback[] feedback)
        {
            var config = CreateInstance<WheelLevelConfig>();
            config.RequiredLogs = requiredLogs;
            config.MinimumDistractors = minimumDistractors;
            config.CounterFormat = counterFormat;
            config.ForestObjects = forestObjects;
            config.Feedback = feedback;
            return config;
        }
#endif
    }
}
