namespace Game.Levels.River
{
    /// <summary>
    /// Lo que produce una acción del nivel: si se ejecutó y la frase que lo describe.
    /// </summary>
    /// <remarks>
    /// La frase describe lo ocurrido y no califica (RF-11, RF-17), y nunca trae una cifra de
    /// desempeño (CP-03). Vacía cuando no hubo nada que decir.
    /// </remarks>
    public readonly struct Outcome
    {
        public Outcome(bool accepted, string message)
        {
            Accepted = accepted;
            Message = message ?? string.Empty;
        }

        public bool Accepted { get; }

        public string Message { get; }
    }
}
