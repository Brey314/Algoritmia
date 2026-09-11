namespace Game.Core
{
    /// <summary>
    /// Lo que el nivel activo necesita saber sobre la pausa para no contar ese tiempo en el
    /// indicador de resolución (RF-07, RF-45). Vive en Core para que Game.UI (quien pausa) y
    /// Game.Levels.* (quien mide) no se referencien entre sí.
    /// </summary>
    public interface ILevelReporter
    {
        void PauseOpened();

        void PauseClosed();
    }
}
