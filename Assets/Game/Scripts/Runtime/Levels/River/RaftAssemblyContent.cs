using System;
using System.Linq;
using Game.Scaffolding;
using UnityEngine;

namespace Game.Levels.River
{
    /// <summary>
    /// El contenido del ensamblaje fuera del código (CT-05, RNF-18): los espacios de la balsa,
    /// el arte de cada pieza y su silueta, cuántos amarres da un rollo de soga, el encuadre del
    /// panel y todas las frases.
    /// </summary>
    /// <remarks>
    /// Las posiciones del área de la balsa son fracciones de la ilustración, como todo lo que
    /// cuelga de ella; las de cada espacio son fracciones de esa área (<see cref="RaftSlot"/>).
    /// El encuadre pasa por las mismas reglas que las paradas narrativas: con 16:9 sin duplicar
    /// el rango del foco a zoom 2.2 es <c>[0.227, 0.773]</c> y <see cref="OnValidate"/> lo avisa.
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritm/Ensamblaje del Nivel 3", fileName = "N3_RaftAssemblyContent")]
    public class RaftAssemblyContent : ScriptableObject
    {
        /// <summary>El arte de una clase de material dentro de la balsa: puesta y como silueta.</summary>
        [Serializable]
        public class PieceArt
        {
            [field: SerializeField]
            public MaterialKind Kind { get; private set; }

            [field: SerializeField]
            [field: Tooltip("La pieza ya colocada en su espacio.")]
            public Sprite Placed { get; private set; }

            [field: SerializeField]
            [field: Tooltip("La silueta del espacio vacío: misma forma, dibujada aparte (decisión del 20/09/2026).")]
            public Sprite Silhouette { get; private set; }

            public PieceArt(MaterialKind kind, Sprite placed = null, Sprite silhouette = null)
            {
                Kind = kind;
                Placed = placed;
                Silhouette = silhouette;
            }

            private PieceArt()
            {
            }
        }

        [field: SerializeField]
        [field: Tooltip("Los espacios de la balsa: cinco troncos, diez amarres, mástil y vela. Cada uno dice en qué fase se abre y qué material acepta.")]
        public RaftSlot[] Slots { get; private set; } = Array.Empty<RaftSlot>();

        [field: SerializeField]
        [field: Tooltip("El arte de cada clase de material dentro de la balsa.")]
        public PieceArt[] Art { get; private set; } = Array.Empty<PieceArt>();

        [field: SerializeField]
        [field: Tooltip("Cuántos amarres salen de un rollo de soga: el rollo se arrastra una vez por amarre y se agota con el último.")]
        public int KnotsPerRope { get; private set; } = 10;

        [field: SerializeField]
        [field: Tooltip("Encuadre del ensamblaje: el río ocupa el 85 % del ancho y la orilla parte la pantalla en dos (decisión del 20/09/2026).")]
        public CameraFraming AssemblyFraming { get; private set; } = new CameraFraming(new Vector2(0.5f, 0.23f), 2.2f);

        [field: SerializeField]
        [field: Tooltip("Cuánto dura el empuje de cámara del plano de juego al del ensamblaje, en segundos.")]
        public float PushSeconds { get; private set; } = 1f;

        [field: SerializeField]
        [field: Tooltip("Centro del área de la balsa, en fracciones de la ilustración. En el foco del encuadre queda en la mitad de la pantalla.")]
        public Vector2 RaftPosition { get; private set; } = new Vector2(0.5f, 0.23f);

        [field: SerializeField]
        [field: Tooltip("Lado del área de la balsa (cuadrada) como fracción del alto de la ilustración.")]
        public float RaftSize { get; private set; } = 0.28f;

        [field: SerializeField]
        [field: Tooltip("Cuánto dura la balsa girando y hundiéndose por un costado tras una prueba fallida, en segundos (Direccion_de_Arte §11: 0.6 s, y vuelve).")]
        public float SinkSeconds { get; private set; } = 0.6f;

        [field: SerializeField]
        [field: Tooltip("La escena narrativa que se reproduce la primera vez que la prueba falla y nunca más (guion §1.8.4.1).")]
        public string FirstFailureSequenceId { get; private set; } = "N3_Escena32_PrimerIntento";

        [field: SerializeField]
        [field: Tooltip("La escena narrativa del cruce, a la que se sale con la balsa aprobada (guion §1.8.5).")]
        public string ClosingSequenceId { get; private set; } = "N3_Escena33_Cruce";

        [field: SerializeField]
        [field: Tooltip("Rótulo del botón de confirmar en las fases de base y amarre (RF-40).")]
        public string ConfirmLabel { get; private set; } = "Listo";

        [field: SerializeField]
        [field: Tooltip("Rótulo del botón en la última fase (RF-42).")]
        public string TestLabel { get; private set; } = "Probar balsa";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al dejar una pieza en un espacio. Describe, no juzga: todavía no se ha validado nada.")]
        public string PlacedMessage { get; private set; } = "Puesto. Cuando toda esta parte esté, pulsa el botón.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al sacar una pieza de un espacio y devolverla al inventario.")]
        public string TakenBackMessage { get; private set; } = "La pieza vuelve al inventario.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al soltar una pieza fuera de todo espacio. Describe, no regaña (CP-02).")]
        public string MissedMessage { get; private set; } = "Cayó fuera de la balsa: la pieza vuelve al inventario.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al soltar sobre un espacio que ya tiene pieza.")]
        public string OccupiedMessage { get; private set; } = "Ese espacio ya tiene algo. Busca uno vacío.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al confirmar la base o el amarre con algo mal puesto o vacío: qué revisar, nunca cuál es la pieza (CP-06).")]
        public string PhaseRejectedMessage { get; private set; } = "Algo de esta parte no encaja. Mira lo marcado: ¿esa pieza va ahí?";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al confirmar la base correctamente (RF-41).")]
        public string BaseConfirmedMessage { get; private set; } = "La base quedó firme. Ahora hay que sujetarla para que no se abra.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al confirmar el amarre correctamente (RF-41).")]
        public string LashingConfirmedMessage { get; private set; } = "La balsa ya no se abre. Falta lo que atrapa el viento.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Cuando la prueba de la balsa falla: se hunde, y el mensaje dice qué revisar (RF-42, HU-13).")]
        public string TestFailedMessage { get; private set; } = "La balsa se volteó. Mira el espacio marcado: ¿qué debería ir ahí?";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Cuando la prueba de la balsa pasa: se cruza el río (guion §1.8.5).")]
        public string TestPassedMessage { get; private set; } = "La balsa flota derecha. ¡A cruzar!";

        /// <summary>Los espacios de una fase, en el orden del asset.</summary>
        public RaftSlot[] SlotsOf(RaftPhase phase) => Slots.Where(slot => slot.Phase == phase).ToArray();

        public PieceArt ArtFor(MaterialKind kind) => Art.FirstOrDefault(art => art.Kind == kind);

#if UNITY_INCLUDE_TESTS
        /// <summary>
        /// Construye el contenido en memoria. Solo para pruebas: es lo que permite verificar que
        /// la lógica sigue al asset y no a un literal (RNF-18).
        /// </summary>
        internal static RaftAssemblyContent Create(
            RaftSlot[] slots,
            int knotsPerRope = 10,
            string phaseRejectedMessage = "revisa lo marcado",
            string baseConfirmedMessage = "base lista",
            string lashingConfirmedMessage = "amarre listo",
            string testFailedMessage = "se hunde: revisa",
            string testPassedMessage = "cruza",
            string placedMessage = "puesto",
            string takenBackMessage = "devuelta",
            string missedMessage = "fuera",
            string occupiedMessage = "ocupado")
        {
            var content = CreateInstance<RaftAssemblyContent>();
            content.Slots = slots;
            content.KnotsPerRope = knotsPerRope;
            content.PhaseRejectedMessage = phaseRejectedMessage;
            content.BaseConfirmedMessage = baseConfirmedMessage;
            content.LashingConfirmedMessage = lashingConfirmedMessage;
            content.TestFailedMessage = testFailedMessage;
            content.TestPassedMessage = testPassedMessage;
            content.PlacedMessage = placedMessage;
            content.TakenBackMessage = takenBackMessage;
            content.MissedMessage = missedMessage;
            content.OccupiedMessage = occupiedMessage;
            return content;
        }
#endif

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var warning in IllustrationFraming.Warnings(AssemblyFraming, null, mirroredForest: false,
                         IllustrationFraming.ScreenAspect))
            {
                Debug.LogWarning($"{name} · ensamblaje: {warning}", this);
            }

            var duplicated = Slots.GroupBy(slot => slot.Id).Where(group => group.Count() > 1).Select(group => group.Key);
            foreach (var id in duplicated)
            {
                Debug.LogWarning($"{name} · el espacio «{id}» está repetido", this);
            }
        }
#endif
    }
}
