namespace Game.Levels.Wheel
{
    /// <summary>
    /// Las seis piezas del área de trabajo (RF-27, guion §6.2.2). Los dos troncos cortos son
    /// gemelos y se distinguen solo para saber cuál de los dos ya se perforó.
    /// </summary>
    public enum WorkshopPiece
    {
        ShortLogA = 0,
        ShortLogB = 1,
        LongLog = 2,
        Plank = 3,
        Tool = 4,
        Cargo = 5
    }
}
