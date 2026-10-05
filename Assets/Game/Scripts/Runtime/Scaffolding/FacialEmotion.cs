namespace Game.Scaffolding
{
    /// <summary>
    /// La expresión de la cara de un personaje (Dirección de arte §7.3; plan de personajes
    /// finales §5.1). Cada valor es un juego de ojos y una boca de reposo en
    /// <see cref="CharacterFaceSet"/>.
    /// </summary>
    /// <remarks>
    /// **No hay emoción de derrota, de tristeza ni de enfado, y es deliberado** (CP-02, DA §7.3):
    /// tras un intento sin éxito el personaje anima (<see cref="Happy"/>, vía
    /// <see cref="ActionEmotion"/>) y nunca se desanima, porque una cara abatida le diría al
    /// estudiante que perdió. <see cref="Worried"/> es la duda de quien pregunta o espera, no un
    /// reproche. Una «mejora» que añada una cara triste reintroduce la pantalla de derrota por la
    /// puerta de atrás; lo vigila <c>FacialEmotion_CP02_NingunaEmocionEsDeDerrotaNiTristeza</c>.
    ///
    /// Los valores son explícitos porque los assets los guardan como número (<see cref="ActorBeat.Emotion"/>):
    /// añadir uno nuevo va al final.
    /// </remarks>
    public enum FacialEmotion
    {
        /// <summary>Mirada serena, boca cerrada relajada. El reposo de todos.</summary>
        Neutral = 0,

        /// <summary>Ojos sonrientes y sonrisa: celebrar, abrazar, animar.</summary>
        Happy = 1,

        /// <summary>Ojos abiertos y boca en «O».</summary>
        Surprised = 2,

        /// <summary>Cejas inclinadas hacia el centro: la duda de quien espera o pregunta.</summary>
        Worried = 3,

        /// <summary>Mirada concentrada, cejas bajas: golpear, martillar, empujar.</summary>
        Focused = 4,

        /// <summary>Ojos cerrados con pestañas hacia abajo. No parpadea.</summary>
        Sleeping = 5
    }
}
