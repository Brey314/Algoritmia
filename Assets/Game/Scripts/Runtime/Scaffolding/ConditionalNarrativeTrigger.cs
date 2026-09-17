namespace Game.Scaffolding
{
    /// <summary>
    /// Una escena narrativa que se reproduce **una sola vez y solo si algo falló**: la 3.2 del
    /// guion (§8.4.1), que se dispara la primera vez que la prueba de la balsa falla y no se
    /// reproduce nunca si el jugador acierta al primer intento.
    /// </summary>
    /// <remarks>
    /// C# plano, sin Unity: la regla se prueba en EditMode. La escena jugable solo le pasa el
    /// resultado de cada prueba y reproduce el id que devuelva.
    ///
    /// No se persiste a propósito: la lista de datos guardados es cerrada (RNF-09) y no admite
    /// «escenas vistas». Tras un cierre forzado a mitad del nivel, un fallo nuevo la reproduce
    /// otra vez — es el mismo criterio con el que RF-06 deriva «ya vista» del progreso.
    /// </remarks>
    public class ConditionalNarrativeTrigger
    {
        public ConditionalNarrativeTrigger(string sequenceId)
        {
            SequenceId = sequenceId;
        }

        /// <summary>La secuencia que se reproduce tras el primer fallo.</summary>
        public string SequenceId { get; }

        /// <summary>Si ya se reprodujo: no vuelve a hacerlo aunque siga fallando.</summary>
        public bool Fired { get; private set; }

        /// <summary>
        /// Registra el resultado de una prueba. Devuelve <see cref="SequenceId"/> la primera vez
        /// que falla y <c>null</c> en cualquier otro caso: acierto, o fallo posterior.
        /// </summary>
        public string AfterAttempt(bool passed)
        {
            if (passed || Fired)
            {
                return null;
            }

            Fired = true;
            return SequenceId;
        }
    }
}
