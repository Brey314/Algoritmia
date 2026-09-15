using System.Collections.Generic;
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

        /// <summary>
        /// El resumen de un nivel de varias fases (el Nivel 2, W16): un solo relato que cubre las
        /// tres, así que lo que hubo en cualquiera de ellas se suma antes de elegir la variante.
        /// Sumar no crea una cifra nueva a la vista: el texto sigue siendo una línea fija.
        /// </summary>
        public static string Compose(LevelSummaryMessages messages, IEnumerable<PerformanceIndicators> phases)
        {
            var attempts = 0;
            var correctedErrors = 0;
            foreach (var phase in phases)
            {
                attempts += phase.Attempts;
                correctedErrors += phase.CorrectedErrors;
            }

            return Compose(messages, new PerformanceIndicators(attempts, correctedErrors, 0, 0f));
        }
    }
}
