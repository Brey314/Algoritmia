namespace Game.Levels.Wheel
{
    /// <summary>
    /// Las piezas del área de trabajo: las seis del guion §6.2.2 (RF-27) y la cuerda que amarra la
    /// caja (INC-54). Los dos troncos cortos son gemelos y se distinguen solo para saber cuál de
    /// los dos ya se perforó.
    /// </summary>
    public enum WorkshopPiece
    {
        ShortLogA = 0,
        ShortLogB = 1,
        LongLog = 2,
        Plank = 3,
        Tool = 4,
        Cargo = 5,
        Rope = 6
    }
}
