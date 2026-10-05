namespace Game.Scaffolding
{
    /// <summary>
    /// La emoción que lleva puesta un personaje según lo que hace, cuando el guion no fija otra
    /// (<see cref="ActorBeat.SetsEmotion"/>). C# plano: así las cinco mecánicas ganan expresión sin
    /// tocar sus controladores, que ya piden la acción (Dirección de arte §7.3).
    /// </summary>
    public static class ActionEmotion
    {
        public static FacialEmotion For(ActorAction action)
        {
            switch (action)
            {
                // Celebrar y abrazar son alegres. Animar también: el ánimo viene tras un intento sin
                // éxito, y la cara que lo acompaña es positiva y nunca abatida (CP-02).
                case ActorAction.Celebrate:
                case ActorAction.Hug:
                case ActorAction.Encourage:
                    return FacialEmotion.Happy;

                case ActorAction.Surprise:
                    return FacialEmotion.Surprised;

                case ActorAction.Sleep:
                    return FacialEmotion.Sleeping;

                // Lo que exige esfuerzo o cuidado: cejas bajas, mirada puesta en lo que se hace.
                case ActorAction.Strike:
                case ActorAction.Hammer:
                case ActorAction.Blow:
                case ActorAction.Push:
                case ActorAction.Carry:
                case ActorAction.PickUp:
                case ActorAction.Kneel:
                    return FacialEmotion.Focused;

                default:
                    return FacialEmotion.Neutral;
            }
        }
    }
}
