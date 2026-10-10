namespace Game.Scaffolding
{
    /// <summary>
    /// Desde dónde se dibuja el cuerpo de un personaje: de frente o de perfil (Santiago, 09/10/2026,
    /// INC-134). Lo decide la acción que hace (<see cref="ActionView"/>), no hacia dónde se
    /// desplaza; <see cref="CharacterRig"/> es quien enciende un cuerpo u otro.
    /// </summary>
    /// <remarks>
    /// Los valores son explícitos por la misma razón que los de <see cref="ActorAction"/>: si algún día
    /// se guardan en un asset, añadir uno nuevo va al final.
    /// </remarks>
    public enum CharacterView
    {
        /// <summary>
        /// De frente: el cuerpo de siempre (<c>Lienzo/Cuerpo</c>). Es el reposo, el habla y todo gesto
        /// que no recorre el entorno; y la única vista de quien no tiene arte de perfil (Algoritm).
        /// Nunca se espeja por el rumbo.
        /// </summary>
        Front = 0,

        /// <summary>
        /// De perfil: el cuerpo dibujado de lado (<c>Lienzo/Perfil</c>), canónico mirando a la derecha;
        /// para mirar a la izquierda se voltea el lienzo (<see cref="CharacterRig.Mirrored"/>).
        /// </summary>
        Profile = 1
    }
}
