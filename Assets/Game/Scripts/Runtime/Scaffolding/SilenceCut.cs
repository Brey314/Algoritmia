namespace Game.Scaffolding
{
    /// <summary>
    /// Los silencios que el guion escribe de forma explícita son piezas con disparador
    /// (<c>Direccion_de_Musica_y_Sonido.md</c> §5), y una línea de diálogo puede disparar uno.
    /// El corte es seco: sin el fundido que llevan las demás transiciones.
    /// </summary>
    public enum SilenceCut
    {
        /// <summary>La línea no calla nada.</summary>
        None,

        /// <summary>Callan música y voz; el ambiente sigue — S2: Algoritm se apaga y queda la cueva.</summary>
        Music,

        /// <summary>Calla todo salvo el efecto que ya suena — S1: el negro tras entrar en la cueva.</summary>
        Everything
    }
}
