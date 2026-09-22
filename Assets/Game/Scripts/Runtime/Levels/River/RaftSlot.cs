using System;
using UnityEngine;

namespace Game.Levels.River
{
    /// <summary>
    /// Un espacio de la balsa: a qué fase pertenece, qué material le corresponde y dónde está
    /// (RF-40). Es contenido (CT-05, RNF-18): la balsa de cinco troncos y diez amarres son
    /// diecisiete entradas del asset, no diecisiete láminas ni diecisiete campos.
    /// </summary>
    /// <remarks>
    /// La posición y el tamaño son **fracciones del área de la balsa**, un cuadro que a su vez
    /// cuelga de la ilustración: el panel se compone espacio a espacio —silueta hasta que se
    /// llena, pieza después— y cualquier estado sale de la suma, sin una lámina por estado
    /// (decisión de Santiago del 20/09/2026).
    /// </remarks>
    [Serializable]
    public class RaftSlot
    {
        [field: SerializeField]
        [field: Tooltip("Identificador del espacio, p. ej. «tronco_3» o «amarre_3_izq». Único en la balsa.")]
        public string Id { get; private set; }

        [field: SerializeField]
        [field: Tooltip("En qué fase se abre este espacio (RF-40).")]
        public RaftPhase Phase { get; private set; }

        [field: SerializeField]
        [field: Tooltip("El material que va aquí. Cualquier otro es una colocación incorrecta (guion §1.8.4).")]
        public MaterialKind Accepts { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Centro del espacio, en fracciones del área de la balsa: (0,0) abajo-izquierda, (1,1) arriba-derecha.")]
        public Vector2 Position { get; private set; } = new Vector2(0.5f, 0.5f);

        [field: SerializeField]
        [field: Tooltip("Ancho y alto del espacio, en fracciones del área de la balsa. El sprite se ajusta dentro sin deformarse.")]
        public Vector2 Size { get; private set; } = new Vector2(0.2f, 0.2f);

        [field: SerializeField]
        [field: Tooltip("Giro del espacio en grados, antihorario. El mástil se dibuja en diagonal suelto y va vertical en la balsa.")]
        public float Rotation { get; private set; }

        public RaftSlot(string id, RaftPhase phase, MaterialKind accepts, Vector2 position = default, Vector2 size = default,
            float rotation = 0f)
        {
            Id = id;
            Phase = phase;
            Accepts = accepts;
            Position = position == default ? new Vector2(0.5f, 0.5f) : position;
            Size = size == default ? new Vector2(0.2f, 0.2f) : size;
            Rotation = rotation;
        }

        /// <summary>Requerido por la serialización de Unity.</summary>
        private RaftSlot()
        {
        }
    }
}
