using System;
using Game.Scaffolding;
using UnityEngine;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// Contenido de la fase 2 del Nivel 2, el taller: las seis piezas con su sitio, el arte de
    /// cada estado de la carretilla y todo lo que dice el juego (RF-27..RF-29, RNF-18, CT-05).
    /// </summary>
    /// <remarks>
    /// Los mensajes de rechazo son los literales del guion §6.2.2 y dicen **qué falta antes**,
    /// nunca cuál es el paso correcto (CP-06). Ninguno lleva cifras (RF-17, CP-03): lo vigila
    /// <c>AssemblySequence_RF17_NingunMensajeContieneDigitos</c>.
    /// </remarks>
    [CreateAssetMenu(fileName = "N2_AssemblyContent", menuName = "Algoritmia/Nivel 2/Contenido del taller")]
    public class AssemblyContent : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Las seis piezas del área de trabajo con su sitio en el suelo (RF-27).")]
        public WorkshopPiecePlacement[] Pieces { get; private set; } = Array.Empty<WorkshopPiecePlacement>();

        [field: SerializeField]
        [field: Tooltip("El tronco corto ya perforado: la rueda (RF-28). Sustituye al arte del tronco al mecanizarlo.")]
        public Sprite DrilledWheelArt { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Las dos ruedas atravesadas por el eje (estado 3 de la carretilla).")]
        public Sprite AxleArt { get; private set; }

        [field: SerializeField]
        [field: Tooltip("El conjunto con la tabla montada (estado 4).")]
        public Sprite PlankArt { get; private set; }

        [field: SerializeField]
        [field: Tooltip("La carretilla completa con la caja encima (estado 5).")]
        public Sprite CompleteArt { get; private set; }

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al seleccionar un tronco corto todavía macizo.")]
        public string SelectedMessage { get; private set; } = "El tronco quedó resaltado.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al seleccionar un tronco que ya es rueda: no hay nada que mecanizar.")]
        public string AlreadyWheelMessage { get; private set; } = "Ese tronco ya tiene su agujero: es una rueda.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al pulsar «Mecanizar» sin tronco seleccionado. El botón va deshabilitado, así que solo se ve si algo lo salta.")]
        public string SelectNeededMessage { get; private set; } = "Mecanizar necesita un tronco corto resaltado.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al perforar la primera rueda (guion §6.2.2 paso 2).")]
        public string WheelDrilledMessage { get; private set; } = "Se abrió un agujero en el centro: el tronco ya es una rueda.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al perforar la segunda rueda (paso 3).")]
        public string BothWheelsMessage { get; private set; } = "Segunda rueda lista. Las dos tienen por dónde entrar algo.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al formar el eje (paso 4).")]
        public string AxleFormedMessage { get; private set; } = "El tronco largo entró por el centro de las dos ruedas: ya hay un eje.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al montar la tabla (paso 5).")]
        public string PlankPlacedMessage { get; private set; } = "La tabla quedó montada sobre el conjunto.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al colocar la caja: la carretilla queda completa (paso 6).")]
        public string CompleteMessage { get; private set; } = "La caja va sobre la tabla. La carretilla está completa.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Guion §6.2.2: insertar el eje en un tronco sin perforar.")]
        public string AxleTooEarlyMessage { get; private set; } = "El tronco todavía no tiene por dónde entrar el palo.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Guion §6.2.2: colocar la tabla sin eje formado.")]
        public string PlankTooEarlyMessage { get; private set; } = "La tabla no tiene sobre qué apoyarse todavía.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Guion §6.2.2: colocar la caja antes que la tabla.")]
        public string CargoTooEarlyMessage { get; private set; } = "La caja se caería. Falta algo plano debajo.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al soltar una pieza lejos del lugar de armado. Describe, no regaña (CP-02).")]
        public string MissedMessage { get; private set; } = "La pieza vuelve a su sitio: cayó lejos del lugar de armado.";

        [field: SerializeField]
        [field: Tooltip("Dónde crece la carretilla, en fracciones de la ilustración: el sitio de las ruedas, que es por donde entra el eje (guion §6.2.2 paso 4).")]
        public Vector2 AssemblyPosition { get; private set; } = new Vector2(0.69f, 0.31f);

        [field: SerializeField]
        [field: Tooltip("Lado de la casilla de la carretilla como fracción del alto de la ilustración.")]
        public float AssemblySize { get; private set; } = 0.24f;

        [field: SerializeField]
        [field: Tooltip("Cuánto dura el empuje de cámara sobre la carretilla terminada antes de salir de la fase, en segundos (Camara_Narrativa_N2.md §5.6: 1,2 s, rima con el cierre de la fase 1).")]
        public float CompletionSeconds { get; private set; } = 1.2f;

        [field: SerializeField]
        [field: Tooltip("Secuencia narrativa a la que sale el taller al terminar (guion §6.3.1). A dónde sale una fase es contenido, no código (RF-05).")]
        public string ClosingSequenceId { get; private set; } = "N2_Escena24_Regreso";

        [field: SerializeField]
        [field: Tooltip("Plano fijo del área de trabajo: idéntico al último encuadre de la escena 2.3, de la narrativa al juego no hay salto (Camara_Narrativa_N2.md §5.6).")]
        public CameraFraming PlayFraming { get; private set; } = new CameraFraming(new Vector2(0.772f, 0.47f), 1.32f);

        [field: SerializeField]
        [field: Tooltip("Encuadre al que empuja la cámara sobre la carretilla terminada (Camara_Narrativa_N2.md §5.6, CIERRE).")]
        public CameraFraming CompletionFraming { get; private set; } = new CameraFraming(new Vector2(0.705f, 0.445f), 1.56f);

#if UNITY_INCLUDE_TESTS
        /// <summary>
        /// Contenido en memoria, solo para pruebas: mensajes deliberadamente distintos del asset
        /// para que la regla no pueda apoyarse en un literal (RNF-18).
        /// </summary>
        internal static AssemblyContent Create(
            string selected = "seleccionado",
            string alreadyWheel = "ya es rueda",
            string selectNeeded = "elige un tronco",
            string wheelDrilled = "rueda uno",
            string bothWheels = "rueda dos",
            string axleFormed = "eje listo",
            string plankPlaced = "tabla lista",
            string complete = "carretilla lista",
            string axleTooEarly = "falta perforar",
            string plankTooEarly = "falta el eje",
            string cargoTooEarly = "falta la tabla",
            string missed = "lejos",
            string closingSequenceId = "cierre",
            float completionSeconds = 0.05f,
            WorkshopPiecePlacement[] pieces = null)
        {
            var content = CreateInstance<AssemblyContent>();
            content.SelectedMessage = selected;
            content.AlreadyWheelMessage = alreadyWheel;
            content.SelectNeededMessage = selectNeeded;
            content.WheelDrilledMessage = wheelDrilled;
            content.BothWheelsMessage = bothWheels;
            content.AxleFormedMessage = axleFormed;
            content.PlankPlacedMessage = plankPlaced;
            content.CompleteMessage = complete;
            content.AxleTooEarlyMessage = axleTooEarly;
            content.PlankTooEarlyMessage = plankTooEarly;
            content.CargoTooEarlyMessage = cargoTooEarly;
            content.MissedMessage = missed;
            content.ClosingSequenceId = closingSequenceId;
            content.CompletionSeconds = completionSeconds;
            content.Pieces = pieces ?? Array.Empty<WorkshopPiecePlacement>();
            return content;
        }
#endif
    }
}
