namespace Game.Levels.Wheel
{
    /// <summary>
    /// El último paso del ensamblaje que quedó hecho (RF-29). El orden del enum **es** el orden
    /// del guion §6.2.2: perforar dos ruedas → eje → tabla → caja, y solo avanza.
    /// </summary>
    public enum AssemblyStep
    {
        None = 0,
        FirstWheel = 1,
        SecondWheel = 2,
        Axle = 3,
        Plank = 4,
        Cargo = 5
    }
}
