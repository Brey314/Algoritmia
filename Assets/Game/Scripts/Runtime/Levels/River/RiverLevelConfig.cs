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
    /// fracciones de la ilustración**, no píxeles ni fracciones de pantalla: la cámara de la
    /// recolección es un plano fijo —el cuadrante del bosque apoyado en la esquina inferior
    /// izquierda, desde el 25/09/2026 foco (0.2632, 0.2632) · zoom 1.9, con **un poco del río
    /// asomando a la derecha** (del 20/09 al 25/09 fue ×2.5, sin río); el río entero entra con
    /// el empuje del ensamblaje— y lo que se ve es el recorte <c>foco ± 0.5/zoom</c>; lo que
    /// caiga fuera no aparece nunca, y
    /// <see cref="OnValidate"/> lo avisa. **El piso termina en <see cref="GroundTop"/>**: arriba
    /// empiezan los arbustos y los troncos de los árboles, y nada se coloca ahí. La perspectiva
    /// es por profundidad (<see cref="DepthScaleAt"/>): lo que está más abajo —más cerca— se ve
    /// más grande, Mamá incluida. El radio de proximidad y la velocidad son la pregunta abierta 3
    /// del plan: se ajustan jugando, sin recompilar.
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
        public float ProximityRadius { get; private set; } = 0.04f;

        [field: SerializeField]
        [field: Tooltip("Cuánto avanza Mamá por segundo con un botón sostenido, en fracción de la ilustración.")]
        public float MoveSpeed { get; private set; } = 0.25f;

        [field: SerializeField]
        [field: Tooltip("Dónde empieza Mamá, en fracciones de la ilustración.")]
        public Vector2 StartPosition { get; private set; } = new Vector2(0.15f, 0.15f);

        [field: SerializeField]
        [field: Tooltip("Por dónde se puede andar: el pasto del bosque, sin el seto de abajo a la izquierda, sin las raíces del árbol y sin pasar de donde termina el piso. Tiene que caber en el recorte de PlayFraming con margen para que Mamá entre entera en cámara.")]
        public Rect WalkableArea { get; private set; } = new Rect(0.13f, 0.05f, 0.23f, 0.26f);

        [field: SerializeField]
        [field: Tooltip("Dónde termina el piso, en fracción del alto de la ilustración: arriba empiezan los arbustos. Medido en env_n3_rio (20/09/2026): el pasto llega a y de 0.34 a 0.40 según la columna.")]
        public float GroundTop { get; private set; } = 0.36f;

        [field: SerializeField]
        [field: Tooltip("Escala de lo que está al borde inferior de la ilustración: lo más cerca de la cámara.")]
        public float DepthScaleNear { get; private set; } = 1f;

        [field: SerializeField]
        [field: Tooltip("Escala de lo que está donde termina el piso: lo más lejos. Entre las dos se interpola por la altura.")]
        public float DepthScaleFar { get; private set; } = 0.55f;

        [field: SerializeField]
        [field: Tooltip("Centro de la zona de construcción, al borde del agua (Camara_Narrativa_N3.md §6).")]
        public Vector2 BuildZonePosition { get; private set; } = new Vector2(0.35f, 0.21f);

        [field: SerializeField]
        [field: Tooltip("Radio de la zona de construcción, en fracción de la ilustración.")]
        public float BuildZoneRadius { get; private set; } = 0.04f;

        [field: SerializeField]
        [field: Tooltip("Plano fijo de toda la recolección: el cuadrante del bosque con un poco de la orilla asomando a la derecha (decisión de Santiago del 25/09/2026; antes, sin río). El río entero entra con el empuje de cámara del ensamblaje. El zoom no puede igualar al del ensamblaje: el empuje no tendría a dónde ir.")]
        public CameraFraming PlayFraming { get; private set; } = new CameraFraming(new Vector2(0.2632f, 0.2632f), 1.9f);

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

        /// <summary>
        /// Cuánto se escala algo puesto a esa altura de la ilustración: lo de abajo está cerca y se
        /// ve grande, lo de arriba —hasta donde termina el piso— lejos y pequeño. Vale para Mamá,
        /// los materiales y la zona, que comparten el piso.
        /// </summary>
        public float DepthScaleAt(float y) =>
            Mathf.Lerp(DepthScaleNear, DepthScaleFar, GroundTop > 0f ? Mathf.Clamp01(y / GroundTop) : 0f);

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
            float groundTop = 1f,
            float depthScaleNear = 1f,
            float depthScaleFar = 1f,
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
            config.GroundTop = groundTop;
            config.DepthScaleNear = depthScaleNear;
            config.DepthScaleFar = depthScaleFar;
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
                if (collectible.Position.y > GroundTop)
                {
                    Debug.LogWarning($"{name} · «{collectible.Id}» queda por encima de donde termina el piso ({GroundTop:0.00})", this);
                }
            }

            if (WalkableArea.yMax > GroundTop)
            {
                Debug.LogWarning($"{name} · la orilla andable sube más allá del piso ({GroundTop:0.00})", this);
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
