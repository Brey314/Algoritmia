namespace Game.Scaffolding
{
    /// <summary>
    /// De qué lado se ve a un personaje según lo que hace (Santiago, 09/10/2026, INC-134): de perfil
    /// cuando recorre o trabaja el entorno —camina, corre, carga, empuja, recoge, se arrodilla, sopla—
    /// o cuando abraza o duerme (INC-154, 13/10/2026) y de frente en todo lo demás. C# plano, como
    /// <see cref="ActionEmotion"/>: la tabla vive aquí y
    /// cambiar una fila es una línea; las cinco mecánicas y la narrativa ganan la vista sin tocar sus
    /// controladores, que ya piden la acción.
    /// </summary>
    /// <remarks>
    /// **Manda la acción, no el desplazamiento.** Alguien que se mueve con <see cref="ActorAction.Idle"/>
    /// —la familia sobre la balsa que cruza, Algoritm flotando— sigue de frente, y quien está quieto
    /// pero agachado (<see cref="ActorAction.Kneel"/>) se ve de perfil: lo que cuenta la imagen es el
    /// esfuerzo sobre el entorno, no el traslado. Hablar, señalar, celebrar o animar son gestos hacia
    /// el estudiante y por eso van de frente: el ánimo tras un intento sin éxito (CP-02) nunca se da
    /// de espaldas ni de costado.
    ///
    /// **Abrazar y dormir no son gestos hacia el estudiante** (INC-154, decisión de Santiago tras la
    /// sesión con estudiantes del 13/10/2026): el abrazo es entre dos personajes y de frente los brazos
    /// abiertos se leían como una T, no como un abrazo; y dormir es estar acostado, que solo se ve de
    /// lado. Los dos van de perfil, y el guion decide hacia dónde con <see cref="ActorBeat.Facing"/>:
    /// quien abraza mira a quien abraza, y quien duerme queda con la cabeza hacia el lado contrario al
    /// que mira su perfil.
    ///
    /// El corte entre vistas es seco y lo hace <see cref="CharacterRig"/>; un rig sin arte de perfil
    /// ignora esta tabla y se queda de frente.
    /// </remarks>
    public static class ActionView
    {
        public static CharacterView For(ActorAction action)
        {
            switch (action)
            {
                case ActorAction.Walk:
                case ActorAction.Run:
                case ActorAction.Carry:
                case ActorAction.Push:
                case ActorAction.PickUp:
                case ActorAction.Kneel:
                case ActorAction.Blow:
                case ActorAction.Hug:
                case ActorAction.Sleep:
                    return CharacterView.Profile;

                default:
                    return CharacterView.Front;
            }
        }
    }
}
