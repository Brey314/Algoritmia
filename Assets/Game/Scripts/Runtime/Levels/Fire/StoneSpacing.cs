using UnityEngine;

namespace Game.Levels.Fire
{
    /// <summary>
    /// Traduce la muesca del deslizante de cercanía a la distancia entre el sílex y el pedernal y
    /// la clasifica (Fase 6, T26): muesca cero = lo más lejos, a la longitud de una hoja; muesca
    /// máxima = una piedra encima de la otra. El golpe certero es cuando se rozan, encimadas unos
    /// pocos píxeles (<see cref="FireLevelConfig.EffectiveOverlap"/>).
    /// </summary>
    /// <remarks>
    /// C# plano: se prueba en EditMode sin escena. El panel le pasa los anchos que mide en el
    /// suelo y solo le pregunta dónde va cada piedra y si el golpe puede prender. Todo en la misma
    /// unidad que las posiciones del suelo (píxeles del lienzo de referencia, antes del
    /// acercamiento).
    /// </remarks>
    public class StoneSpacing
    {
        private readonly FireLevelConfig _config;
        private readonly float _stoneWidth;
        private readonly float _farDistance;

        /// <param name="stoneWidth">Ancho de una piedra: con distancia menor, se enciman.</param>
        /// <param name="farDistance">Distancia entre centros en la muesca cero: la longitud de una hoja.</param>
        public StoneSpacing(FireLevelConfig config, float stoneWidth, float farDistance)
        {
            _config = config;
            _stoneWidth = stoneWidth;
            _farDistance = farDistance;
        }

        /// <summary>Distancia entre los centros de las piedras en esa muesca. Nunca negativa.</summary>
        public float Distance(int notch) =>
            Mathf.Lerp(_farDistance, 0f, Mathf.Clamp01((float)notch / Mathf.Max(_config.SpacingLevels, 1)));

        /// <summary>Cuánto se enciman en esa muesca. Negativo: hay hueco entre ellas.</summary>
        public float Overlap(int notch) => _stoneWidth - Distance(notch);

        /// <summary>Dónde cae la muesca respecto del roce que hace chispa.</summary>
        public SpacingBand Classify(int notch)
        {
            var delta = Overlap(notch) - _config.EffectiveOverlap;
            return delta < -_config.OverlapTolerance ? SpacingBand.TooFar
                : delta > _config.OverlapTolerance ? SpacingBand.TooClose
                : SpacingBand.Effective;
        }
    }
}
