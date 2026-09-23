using System;
using UnityEngine;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// El trazado del laberinto y los parámetros de la fase 3 del Nivel 2, fuera del código
    /// (CT-05, RNF-18): la matriz, dónde entra y a dónde llega la carretilla, cuántos obstáculos
    /// se siembran, el arte y los mensajes.
    /// </summary>
    /// <remarks>
    /// **La matriz se traza sobre el entorno, no sobre la pantalla.** <see cref="BoardMin"/> y
    /// <see cref="BoardMax"/> son las esquinas **exteriores** del seto de
    /// <c>entorno_n2_laberinto.png</c>, en fracciones de la ilustración, y la matriz de
    /// <see cref="Columns"/> × <see cref="Rows"/> (16 × 11, casillas de ~8 % del alto) la cubre
    /// entera: **el anillo exterior son los arbustos** y ya está ocupado, salvo las casillas de
    /// salida y de refugio, que son los dos huecos del seto y sí se pisan. Las casillas cuelgan
    /// del entorno en esas fracciones y acompañan cualquier resolución del arte: sustituir el
    /// archivo basta (RNF-23); si el dibujo mueve los setos, se corrigen las dos esquinas aquí.
    ///
    /// Siempre se parte y se llega al mismo punto; lo que cambia con la semilla es dónde caen
    /// los obstáculos (<see cref="MazeGrid.Generate"/>).
    /// </remarks>
    [CreateAssetMenu(menuName = "Algoritm/Laberinto del Nivel 2", fileName = "N2_MazeLayout")]
    public class MazeLayout : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Columnas de la matriz, setos incluidos.")]
        public int Columns { get; private set; } = 16;

        [field: SerializeField]
        [field: Tooltip("Filas de la matriz, setos incluidos.")]
        public int Rows { get; private set; } = 11;

        [field: SerializeField]
        [field: Tooltip("Casilla de salida (columna desde la izquierda, fila desde abajo): el hueco del seto izquierdo.")]
        public Vector2Int StartCell { get; private set; } = new Vector2Int(0, 8);

        [field: SerializeField]
        [field: Tooltip("Hacia dónde mira la carretilla al empezar. Entra por la izquierda, así que mira al este.")]
        public Orientation StartFacing { get; private set; } = Orientation.East;

        [field: SerializeField]
        [field: Tooltip("Casilla del refugio: el hueco del seto derecho, junto a la roca.")]
        public Vector2Int GoalCell { get; private set; } = new Vector2Int(15, 2);

        [field: SerializeField]
        [field: Tooltip("Casillas de obstáculo que se siembran dentro del seto, en tramos de 1 a 5 en línea. Si con tantas no queda camino, se siembra una menos, y así hasta que lo haya.")]
        public int ObstacleCount { get; private set; } = 24;

        [field: SerializeField]
        [field: Tooltip("Cuántas casillas más que la distancia directa debe medir el camino más corto: 0 admite un laberinto trivial.")]
        public int MinimumDetour { get; private set; } = 2;

        [field: SerializeField]
        [field: Tooltip("Semilla del trazado. 0 = distinta en cada partida; cualquier otro valor repite el mismo laberinto.")]
        public int Seed { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Esquina inferior izquierda del seto (borde exterior), en fracciones de la ilustración.")]
        public Vector2 BoardMin { get; private set; } = new Vector2(0.128f, 0.022f);

        [field: SerializeField]
        [field: Tooltip("Esquina superior derecha del seto (borde exterior), en fracciones de la ilustración.")]
        public Vector2 BoardMax { get; private set; } = new Vector2(0.897f, 0.982f);

        [field: SerializeField]
        [field: Tooltip("Cuánto de la casilla ocupa cada pieza (1 = la casilla entera).")]
        public float PieceSize { get; private set; } = 0.9f;

        [field: SerializeField]
        [field: Tooltip("La carretilla en vista superior, mirando hacia arriba. Vacío deja un cuadro con su morro.")]
        public Sprite CartArt { get; private set; }

        [field: SerializeField]
        [field: Tooltip("La papelera que aparece junto al bloque seleccionado de la secuencia para retirarlo (RF-34). El mismo icono que borra un perfil: ui_papelera.")]
        public Sprite DeleteIcon { get; private set; }

        [field: SerializeField]
        [field: Tooltip("El refugio. Vacío deja un cuadro.")]
        public Sprite ShelterArt { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Piedras, curvas y pendientes (RF-30): cada obstáculo toma una al azar. Vacío deja cuadros.")]
        public Sprite[] ObstacleArt { get; private set; } = Array.Empty<Sprite>();

        [field: SerializeField]
        [field: Tooltip("La luz de la hora del día: tiñe el entorno y todo lo que cuelga de él. El Nivel 2 dura un día entero y la fase 3 es al atardecer. Blanco = sin tinte.")]
        public Color LightTint { get; private set; } = Color.white;

        [field: SerializeField]
        [field: Tooltip("Material con el shader «Algoritm/UI Contraste» (fx_contraste). Vacío = el entorno sin contraste añadido.")]
        public Material EnvironmentMaterial { get; private set; }

        [field: SerializeField]
        [field: Range(0.5f, 2f)]
        [field: Tooltip("Contraste del entorno: separa suelo, piedras y setos. 1 = como viene el arte.")]
        public float Contrast { get; private set; } = 1f;

        [field: SerializeField]
        [field: Range(0f, 2f)]
        [field: Tooltip("Saturación del entorno: por debajo de 1 le quita color al arte (el suelo del laberinto es anaranjado de origen). 1 = como viene.")]
        public float Saturation { get; private set; } = 1f;

        [field: SerializeField]
        [field: Tooltip("Color del panel que rodea al entorno cuando la ilustración no lo llena. Se tiñe con la luz del momento, como el entorno, para que parezca su continuación.")]
        public Color BackdropColor { get; private set; } = Color.black;

        [field: SerializeField]
        [field: Tooltip("Color de las líneas de la cuadrícula de la matriz dentro del seto. Transparente = sin cuadrícula.")]
        public Color GridColor { get; private set; } = new Color(0f, 0f, 0f, 0.2f);

        [field: SerializeField]
        [field: Min(0f)]
        [field: Tooltip("Grosor de las líneas de la cuadrícula, en píxeles de la ilustración.")]
        public float GridThickness { get; private set; } = 3f;

        [field: SerializeField]
        [field: Tooltip("Pausa entre pasos al ejecutar, para que el movimiento sea legible (RF-32).")]
        public float StepSeconds { get; private set; } = 0.5f;

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("Al pulsar «Ejecutar» sin bloques (CU-08 FA-3a).")]
        public string EmptySequenceMessage { get; private set; } = "Añade al menos un bloque a tu secuencia antes de ejecutar.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("La carretilla llegó al refugio.")]
        public string ReachedMessage { get; private set; } = "¡La carretilla llegó al refugio! Los pasos, en ese orden, funcionaron.";

        [field: SerializeField, TextArea(2, 3)]
        [field: Tooltip("La ejecución terminó sin llegar. Dice dónde mirar, nunca qué bloque cambiar (CP-06). Sin cifras (CP-03).")]
        public string StoppedMessage { get; private set; } = "La carretilla no llegó al refugio. Mira dónde se detuvo y corrige tu secuencia.";

        [field: SerializeField]
        [field: Tooltip("Id de la secuencia narrativa a la que sale la fase al alcanzar el refugio (guion §6.4).")]
        public string ClosingSequenceId { get; private set; } = "N2_Escena25_Cierre";

        /// <summary>Un trazado en memoria para las pruebas, sin asset en disco.</summary>
        internal static MazeLayout Create(int columns, int rows, Vector2Int start, Orientation facing, Vector2Int goal,
            int obstacleCount = 0, int seed = 1, int minimumDetour = 0)
        {
            var layout = CreateInstance<MazeLayout>();
            layout.Columns = columns;
            layout.Rows = rows;
            layout.StartCell = start;
            layout.StartFacing = facing;
            layout.GoalCell = goal;
            layout.ObstacleCount = obstacleCount;
            layout.Seed = seed;
            layout.MinimumDetour = minimumDetour;
            layout.StepSeconds = 0f;
            return layout;
        }
    }
}
