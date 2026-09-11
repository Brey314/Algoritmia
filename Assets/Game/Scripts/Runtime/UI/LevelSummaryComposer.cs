using Game.Core;

namespace Game.UI
{
    /// <summary>
    /// Compone el resumen narrativo de fin de nivel a partir de los indicadores (RF-45, HU-14
    /// paso 6).
    /// </summary>
    /// <remarks>
    /// C# plano, sin dependencias de escena: se prueba en EditMode sin frames. Solo concatena
    /// líneas fijas del asset — nunca formatea un número —, así que «sin cifras» (CP-03, RF-17) es
    /// estructural y no un cuidado en tiempo de ejecución. Solo dos de los cuatro indicadores
    /// (Intentos, Errores corregidos) tienen una forma narrativa natural sin sonar a cifra
    /// disfrazada; Pasos utilizados y Tiempo de resolución no se traducen a texto.
    /// </remarks>
    public static class LevelSummaryComposer
    {
        public static string Compose(LevelSummaryMessages messages, PerformanceIndicators indicators)
        {
            var attemptsLine = indicators.Attempts > 0
                ? messages.TriedSeveralPositions
                : messages.FoundRightAway;
            var correctedLine = indicators.CorrectedErrors > 0
                ? messages.CorrectedApproach
                : messages.NoNeedToCorrect;

            return string.Join("\n\n", messages.Intro, attemptsLine, correctedLine);
        }
    }
}
