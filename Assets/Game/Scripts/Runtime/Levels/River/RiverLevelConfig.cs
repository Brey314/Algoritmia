using System;
using Game.Scaffolding;
using UnityEngine;

namespace Game.Levels.River
{
    /// <summary>
    /// Los parámetros de la fase de recolección del Nivel 3 fuera del código (CT-05, RNF-18):
    /// los textos de las cuatro tareas, el catálogo de materiales y dónde cae cada uno, el
    /// radio de proximidad, la velocidad, la zona de construcción y qué dice cada cosa.
    /// </summary>
    /// <remarks>
    /// Un asset por nivel, como <see cref="WheelLevelConfig"/>. **Todas las posiciones son
    /// fracciones de la ilustración** (<c>Camara_Narrativa_N3.md</c> §6), no píxeles ni fracciones
    /// de pantalla: la cámara de la recolección es un plano fijo (foco 0.400, 0.410 · zoom 1.50)
    /// y lo que se ve es el recorte <c>foco ± 0.5/zoom</c>; lo que caiga fuera no aparece nunca,
    /// y <see cref="OnValidate"/> lo avisa. El radio de proximidad y la velocidad son la pregunta
    /// abierta 3 del plan: se ajustan jugando, sin recompilar.
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritm/Configuración del Nivel 3", fileName = "N3_RiverLevelConfig")]
    public class RiverLevelConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Los textos de las cuatro tareas, en el orden del guion §1.8.1 (RF-36). Sin cifras (CP-03).")]
        public string[] TaskLabels { get; private set; } =
        {
            "Recoger troncos", "Encontrar sogas", "Ensamblar la balsa", "Colocar el mástil y la vela"
        };

        [field: SerializeField]
        [field: Tooltip("A qué distancia de un material aparece «Recoger», en fracción de la ilustración (RF-37). Pregunta abierta 3: se valida jugando.")]
        public float ProximityRadius { get; private set; } = 0.06f;

        [field: SerializeField]
        [field: Tooltip("Cuánto avanza Mamá por segundo con un botón sostenido, en fracción de la ilustración.")]
        public float MoveSpeed { get; private set; } = 0.25f;

        [field: SerializeField]
        [field: Tooltip("Dónde empieza Mamá, en fracciones de la ilustración.")]
        public Vector2 StartPosition { get; private set; } = new Vector2(0.267f, 0.477f);

        [field: SerializeField]
        [field: Tooltip("Por dónde se puede andar: la orilla izquierda, sin meterse al agua (x < 0.45). Tiene que caber en el recorte de PlayFraming con margen para que Mamá entre entera en cámara: ~0.03 en x y ~0.05 en y, porque la ilustración es más ancha que alta.")]
        public Rect WalkableArea { get; private set; } = new Rect(0.10f, 0.13f, 0.33f, 0.56f);

        [field: SerializeField]
        [field: Tooltip("Centro de la zona de construcción, al borde del agua (Camara_Narrativa_N3.md §6).")]
        public Vector2 BuildZonePosition { get; private set; } = new Vector2(0.40f, 0.15f);

        [field: SerializeField]
        [field: Tooltip("Radio de la zona de construcción, en fracción de la ilustración.")]
        public float BuildZoneRadius { get; private set; } = 0.05f;

        [field: SerializeField]
        [field: Tooltip("Plano fijo de toda la recolección: el último encuadre de la escena 3.1, para que de la narrativa al juego no haya salto.")]
        public CameraFraming PlayFraming { get; private set; } = new CameraFraming(new Vector2(0.4f, 0.41f), 1.5f);

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Lo que se dice al recoger un material. {0} es su nombre. Describe, no felicita (RF-17).")]
        public string CollectedFormat { get; private set; } = "{0}: al inventario. Mira la lista: ¿qué falta?";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Lo que se dice con el inventario completo (CU-09 FA-4a).")]
        public string AllCollectedMessage { get; private set; } =
            "Ya tienes todos los materiales. Ve a la zona marcada junto al agua.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Lo que se dice al entrar a la zona sin todo. {0} son los nombres de lo que falta, nunca cuántos (CU-09 FA-6a, CP-03).")]
        public string MissingFormat { get; private set; } = "Para armar la balsa todavía falta: {0}. Sigue buscando por la orilla.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Lo que se dice al entrar a la zona con todo: se abre el ensamblaje (RF-39).")]
        public string ZoneOpenedMessage { get; private set; } = "Tienes todo lo de la lista. Aquí se arma la balsa.";

        [field: SerializeField]
        [field: Tooltip("Los cuatro materiales y dónde está cada uno. Exactamente uno por clase (RF-38).")]
        public Collectible[] Collectibles { get; private set; } = Array.Empty<Collectible>();

        /// <summary>El texto de una tarea, o vacío si el asset todavía no lo tiene (CP-02: sin frase, nunca sin partida).</summary>
        public string LabelFor(RiverTaskId task)
        {
            var index = (int)task - 1;
            return index >= 0 && index < TaskLabels.Length ? TaskLabels[index] : string.Empty;
        }

#if UNITY_INCLUDE_TESTS
        /// <summary>
        /// Construye una configuración en memoria. Solo para pruebas: es lo que permite verificar
        /// que la lógica sigue al asset y no a un literal (RNF-18).
        /// </summary>
        internal static RiverLevelConfig Create(
            Collectible[] collectibles,
            string[] taskLabels = null,
            float proximityRadius = 0.1f,
            float moveSpeed = 1f,
            Vector2 startPosition = default,
            Rect walkableArea = default,
            Vector2 buildZonePosition = default,
            float buildZoneRadius = 0.05f,
            string collectedFormat = "recogido {0}",
            string allCollectedMessage = "todo",
            string missingFormat = "falta {0}",
            string zoneOpenedMessage = "abierta")
        {
            var config = CreateInstance<RiverLevelConfig>();
            config.Collectibles = collectibles;
            config.TaskLabels = taskLabels ?? config.TaskLabels;
            config.ProximityRadius = proximityRadius;
            config.MoveSpeed = moveSpeed;
            config.StartPosition = startPosition;
            config.WalkableArea = walkableArea == default ? new Rect(0f, 0f, 1f, 1f) : walkableArea;
            config.BuildZonePosition = buildZonePosition;
            config.BuildZoneRadius = buildZoneRadius;
            config.CollectedFormat = collectedFormat;
            config.AllCollectedMessage = allCollectedMessage;
            config.MissingFormat = missingFormat;
            config.ZoneOpenedMessage = zoneOpenedMessage;
            return config;
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// El encuadre pasa las mismas reglas que las paradas narrativas, y todo lo que se pone
        /// en escena tiene que caber en su recorte: con 16:9 sin duplicar el rango es estrecho y
        /// un valor escrito a ojo se sale sin que nadie lo note.
        /// </summary>
        private void OnValidate()
        {
            foreach (var warning in IllustrationFraming.Warnings(PlayFraming, null, mirroredForest: false,
                         IllustrationFraming.ScreenAspect))
            {
                Debug.LogWarning($"{name} · juego: {warning}", this);
            }

            var half = 0.5f / Mathf.Max(PlayFraming.Zoom, 1f);
            var visible = new Rect(PlayFraming.Focus.x - half, PlayFraming.Focus.y - half, 2f * half, 2f * half);
            Warn(visible, WalkableArea.min, "el límite inferior de la orilla");
            Warn(visible, WalkableArea.max, "el límite superior de la orilla");
            Warn(visible, StartPosition, "el arranque de Mamá");
            Warn(visible, BuildZonePosition, "la zona de construcción");
            foreach (var collectible in Collectibles)
            {
                Warn(visible, collectible.Position, $"«{collectible.Id}»");
            }
        }

        private void Warn(Rect visible, Vector2 point, string what)
        {
            if (!visible.Contains(point))
            {
                Debug.LogWarning(FormattableString.Invariant(
                    $"{name} · {what} en ({point.x:0.000}, {point.y:0.000}) queda fuera del recorte {visible}"), this);
            }
        }
#endif
    }
}
