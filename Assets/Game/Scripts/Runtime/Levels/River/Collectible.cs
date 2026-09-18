using System;
using UnityEngine;

namespace Game.Levels.River
{
    /// <summary>
    /// Los cuatro materiales de la balsa (guion §1.8.2): troncos, sogas, tela y mástil.
    /// </summary>
    /// <remarks>
    /// Es estructural —de cada material cuelga qué tarea marca al recogerlo
    /// (<see cref="RiverTask.ForCollected"/>)— y por eso vive en el código; lo que es parámetro
    /// —dónde cae cada uno, cómo se llama y con qué arte— vive en el asset (CT-05, RNF-18).
    /// </remarks>
    public enum MaterialKind
    {
        Logs = 0,
        Ropes = 1,
        Cloth = 2,
        Mast = 3
    }

    /// <summary>
    /// Un material tirado por la orilla: su identidad, de qué clase es y dónde está (RF-37).
    /// </summary>
    /// <remarks>
    /// La posición es una fracción de la **ilustración**, no del suelo ni de la pantalla: son las
    /// coordenadas de <c>Camara_Narrativa_N3.md</c> §6 y las mismas en que se anclan los objetos
    /// de las escenas narrativas, así que sustituir el arte no mueve nada. La proximidad se evalúa
    /// contra una posición inyectada en ese mismo espacio, sin <c>Transform</c>: se prueba en
    /// EditMode.
    /// </remarks>
    [Serializable]
    public class Collectible
    {
        [field: SerializeField]
        [field: Tooltip("Identificador del material, p. ej. «troncos». Único dentro del catálogo.")]
        public string Id { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Clase de material. Decide qué tarea se marca al recogerlo.")]
        public MaterialKind Kind { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Cómo se nombra en pantalla: en el inventario y al decir qué falta (CU-09 FA-6a).")]
        public string DisplayName { get; private set; }

        [field: SerializeField]
        [field: Tooltip("La ilustración del material, en la orilla y en el inventario. Cambiar el arte es repuntar este campo.")]
        public Sprite Art { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Dónde está, en fracciones de la ilustración: (0,0) abajo-izquierda, (1,1) arriba-derecha.")]
        public Vector2 Position { get; private set; } = new Vector2(0.5f, 0.5f);

        public Collectible(string id, MaterialKind kind, string displayName = null, Sprite art = null,
            Vector2 position = default)
        {
            Id = id;
            Kind = kind;
            DisplayName = displayName ?? id;
            Art = art;
            Position = position;
        }

        /// <summary>Requerido por la serialización de Unity.</summary>
        private Collectible()
        {
        }

        /// <summary>Si el personaje está lo bastante cerca para que aparezca «Recoger» (RF-37).</summary>
        public bool IsWithinReach(Vector2 position, float radius) =>
            Vector2.Distance(Position, position) <= radius;
    }
}
