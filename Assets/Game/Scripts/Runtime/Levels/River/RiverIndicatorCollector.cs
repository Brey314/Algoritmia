using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core;

namespace Game.Levels.River
{
    /// <summary>
    /// Los cuatro indicadores del Nivel 3 (RF-45, OE1 §3.6.1), con su definición operativa
    /// literal. Nunca se muestran al estudiante (CP-03): solo llegan a
    /// <see cref="PlayerProfile.ConfirmPhase"/>.
    /// </summary>
    /// <remarks>
    /// C# plano, sin escena: se prueba en EditMode con un reloj inyectado, igual que los
    /// recolectores de los Niveles 1 y 2. **Un solo recolector para las tres fases** porque en
    /// este nivel los indicadores se definen sobre el nivel entero —«pasos utilizados» son las
    /// confirmaciones aceptadas *sobre un máximo de tres*— y las tres fases se juegan en el mismo
    /// panel; solo el tiempo es de cada fase, y <see cref="Complete"/> lo reinicia al cerrarla.
    ///
    /// | Indicador | Definición para el Nivel 3 (§3.6.1) |
    /// |---|---|
    /// | Intentos | confirmaciones de fase rechazadas y pruebas de balsa fallidas |
    /// | Errores corregidos | piezas devueltas al inventario que se recolocan correctamente en el intento siguiente |
    /// | Pasos utilizados | confirmaciones aceptadas: base, amarre, y mástil y vela. Recolectar no suma |
    /// | Tiempo | desde el inicio de la fase hasta su confirmación, menos la pausa y las escenas narrativas (nota 1) |
    ///
    /// La recolección no es fase persistida (<c>PhaseId.PhasesPerLevel</c>), así que el reloj de
    /// la base arranca al abrir el ensamblaje y no en la orilla. Implementa
    /// <see cref="ILevelReporter"/> para que <c>PauseMenuController</c> (Game.UI) le notifique
    /// la pausa sin que ningún assembly gane una referencia hacia el otro — la mediación es
    /// <see cref="GameFlowRunner.ActiveReporter"/>.
    /// </remarks>
    public class RiverIndicatorCollector : ILevelReporter
    {
        private readonly Func<float> _now;
        private float _startedAt;

        private int _attempts;
        private int _correctedErrors;
        private int _stepsUsed;
        private string[] _returnedLastAttempt = Array.Empty<string>();

        private float _pausedSeconds;
        private float _pausedAt;
        private bool _paused;

        /// <param name="now">
        /// Reloj inyectado (producción: <c>() => Time.realtimeSinceStartup</c>) para que el
        /// tiempo de resolución se pueda probar en EditMode sin esperas reales.
        /// </param>
        /// <param name="confirmedPhases">
        /// Fases ya aceptadas en otra sesión al retomar (RNF-14): cuentan como pasos utilizados.
        /// </param>
        public RiverIndicatorCollector(Func<float> now, int confirmedPhases = 0)
        {
            _now = now;
            _startedAt = now();
            _stepsUsed = confirmedPhases;
        }

        /// <summary>
        /// Una confirmación de fase o prueba de balsa (RF-42). Rechazada suma un intento; aceptada
        /// suma un paso. De las piezas devueltas en el intento anterior, cada una que ahora está
        /// bien —su espacio ya no aparece señalado— es un error corregido.
        /// </summary>
        /// <param name="returnedSlotIds">
        /// De los espacios señalados, los que tenían pieza y volvieron al inventario. Un espacio
        /// vacío no devuelve nada: llenarlo después no es corregir una pieza.
        /// </param>
        public void RecordConfirmation(ValidationResult result, IEnumerable<string> returnedSlotIds)
        {
            _correctedErrors += _returnedLastAttempt.Count(id => !result.WrongSlotIds.Contains(id));
            _returnedLastAttempt = returnedSlotIds.ToArray();

            if (result.Passed)
            {
                _stepsUsed++;
            }
            else
            {
                _attempts++;
            }
        }

        public void PauseOpened()
        {
            _paused = true;
            _pausedAt = _now();
        }

        public void PauseClosed()
        {
            if (!_paused)
            {
                return;
            }

            _pausedSeconds += _now() - _pausedAt;
            _paused = false;
        }

        /// <summary>
        /// Cierra el registro de la fase confirmada: el tiempo excluye la pausa abierta (RF-07) y
        /// el reloj arranca de nuevo para la fase siguiente. Los conteos siguen acumulando.
        /// </summary>
        public PerformanceIndicators Complete()
        {
            var now = _now();
            var indicators = new PerformanceIndicators(_attempts, _correctedErrors, _stepsUsed,
                now - _startedAt - _pausedSeconds);
            _startedAt = now;
            _pausedSeconds = 0f;
            return indicators;
        }
    }
}
