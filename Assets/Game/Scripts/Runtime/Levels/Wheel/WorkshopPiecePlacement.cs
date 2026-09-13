using System;
using UnityEngine;

namespace Game.Levels.Wheel
{
    /// <summary>
    /// Dónde y con qué ilustración aparece una pieza del taller. Es contenido (CT-05, RNF-18):
    /// mover una pieza o cambiar su arte es tocar el asset, no el código.
    /// </summary>
    /// <remarks>
    /// Las coordenadas son **fracciones de la ilustración del entorno**, igual que los objetos
    /// pintados de las escenas narrativas (<c>NarrativeProp</c>): así la pieza queda pegada al
    /// mundo con cualquier encuadre y cualquier resolución del arte, y la fase 2 se ve donde la
    /// 2.3 dejó la cámara (<c>Camara_Narrativa_N2.md</c> §7).
    /// </remarks>
    [Serializable]
    public class WorkshopPiecePlacement
    {
        [field: SerializeField]
        [field: Tooltip("Qué pieza es.")]
        public WorkshopPiece Piece { get; private set; }

        [field: SerializeField]
        [field: Tooltip("La ilustración de la pieza tal como está suelta en el suelo.")]
        public Sprite Art { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Dónde va, en fracciones de la ilustración del entorno: (0,0) abajo-izquierda, (1,1) arriba-derecha. Es el centro de la pieza.")]
        public Vector2 Position { get; private set; } = new Vector2(0.75f, 0.3f);

        [field: SerializeField]
        [field: Tooltip("Lado de la casilla de la pieza como fracción del alto de la ilustración. El sprite se ajusta dentro sin deformarse.")]
        public float Size { get; private set; } = 0.1f;

        public WorkshopPiecePlacement(WorkshopPiece piece, Sprite art = null, Vector2 position = default,
            float size = 0.1f)
        {
            Piece = piece;
            Art = art;
            Position = position == default ? new Vector2(0.75f, 0.3f) : position;
            Size = size;
        }

        private WorkshopPiecePlacement()
        {
        }
    }
}
