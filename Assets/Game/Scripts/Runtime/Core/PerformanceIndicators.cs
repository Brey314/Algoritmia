using System;

namespace Game.Core
{
    /// <summary>
    /// Los cuatro indicadores de desempeño de OE1 §3.6.1, por fase de nivel. La lista es
    /// cerrada: no se registra ningún indicador adicional (RNF-09, nota 5). Su definición
    /// operativa cambia con el nivel; el tipo solo los transporta.
    /// </summary>
    /// <remarks>
    /// No hay puntaje ni nada equivalente, y no es un olvido: CP-03 y RF-17 lo prohíben. Estos
    /// valores solo se muestran en el informe docente (RF-46), nunca al estudiante (RF-45).
    /// </remarks>
    [Serializable]
    public readonly struct PerformanceIndicators : IEquatable<PerformanceIndicators>
    {
        /// <summary>Acciones evaluadas que no producen avance en la tarea activa.</summary>
        public int Attempts { get; }

        /// <summary>Intentos fallidos que, tras cambiar la hipótesis, terminan en acierto.</summary>
        public int CorrectedErrors { get; }

        /// <summary>Unidades de acción que componen la solución aceptada.</summary>
        public int StepsUsed { get; }

        /// <summary>
        /// Segundos entre el inicio de la fase jugable y su completación. Excluye escenas
        /// narrativas y el tiempo con la pausa abierta (nota 1): con ellos mediría la duración
        /// de la clase, no la resolución del reto.
        /// </summary>
        public float ResolutionSeconds { get; }

        public PerformanceIndicators(int attempts, int correctedErrors, int stepsUsed,
            float resolutionSeconds)
        {
            Attempts = attempts;
            CorrectedErrors = correctedErrors;
            StepsUsed = stepsUsed;
            ResolutionSeconds = resolutionSeconds;
        }

        public bool Equals(PerformanceIndicators other) =>
            Attempts == other.Attempts
            && CorrectedErrors == other.CorrectedErrors
            && StepsUsed == other.StepsUsed
            && Math.Abs(ResolutionSeconds - other.ResolutionSeconds) < 0.001f;

        public override bool Equals(object obj) => obj is PerformanceIndicators other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(Attempts, CorrectedErrors, StepsUsed, ResolutionSeconds);

        public override string ToString() =>
            $"intentos={Attempts} corregidos={CorrectedErrors} pasos={StepsUsed} segundos={ResolutionSeconds}";
    }
}
