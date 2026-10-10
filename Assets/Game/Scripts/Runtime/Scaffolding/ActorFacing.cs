namespace Game.Scaffolding
{
    /// <summary>
    /// Hacia dónde mira un personaje de perfil cuando el guion lo fija (<see cref="ActorBeat.Facing"/>).
    /// Los valores son explícitos porque los assets los guardan como número: añadir uno nuevo va al final.
    /// </summary>
    public enum ActorFacing
    {
        /// <summary>
        /// Lo decide el motor (<see cref="ActorTimeline.FacesLeftAt"/>): hacia donde se desplaza o, si
        /// no se desplaza, hacia donde se desplazó o se va a desplazar. Es el valor de todos los assets
        /// que no lo declaran.
        /// </summary>
        Auto = 0,

        /// <summary>Mira a la izquierda de la pantalla, sea cual sea el desplazamiento.</summary>
        Left = 1,

        /// <summary>Mira a la derecha de la pantalla, sea cual sea el desplazamiento.</summary>
        Right = 2
    }
}
