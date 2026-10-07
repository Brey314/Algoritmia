using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Qué hace un personaje de la narrativa al aparecer una línea, a partir de sus pasos
    /// (<see cref="NarrativeProp.Beats"/>). C# plano: la regla se prueba en EditMode, y la escena
    /// solo ejecuta la indicación que devuelve.
    /// </summary>
    /// <remarks>
    /// Entre pasos el personaje **mantiene** lo último que hizo —arrodillado sigue arrodillado—
    /// y no vuelve al reposo solo. Así un paso describe un estado y no un instante, que es como
    /// las escribe el guion: «el niño se arrodilla» vale hasta que otra acotación lo levanta.
    ///
    /// Un paso nuevo interrumpe el camino en curso (<see cref="Interrupts"/>), salvo a quien termina
    /// sus pasos (<see cref="NarrativeProp.FinishesSteps"/>): ese llega y después da el paso con
    /// movimiento que se leyó mientras caminaba (<see cref="PendingStep"/>).
    /// </remarks>
    public static class ActorTimeline
    {
        /// <summary>
        /// La indicación para la línea <paramref name="line"/>: dónde está al empezarla, si se
        /// mueve y a dónde, y qué hace durante y después.
        /// </summary>
        /// <param name="speaking">Si la línea la dice este personaje: quieto y sin paso propio, gesticula.</param>
        public static ActorCue Cue(NarrativeProp prop, int line, bool speaking)
        {
            var from = PositionBefore(prop, line);
            var emotion = EmotionAt(prop, line);
            var beat = BeatAt(prop, line);
            if (beat == null)
            {
                var held = HeldBefore(prop, line);
                return new ActorCue(from, from, false, 0f, Talking(held, speaking), Talking(held, speaking), emotion);
            }

            if (!beat.Moves)
            {
                // Un paso Idle es «de pie, sin hacer nada más»: si la línea es suya, la dice.
                var action = Talking(beat.Action, speaking);
                return new ActorCue(from, from, false, 0f, action, action, emotion);
            }

            return new ActorCue(from, beat.Destination, true, Mathf.Max(beat.Seconds, 0f), beat.Action,
                Talking(beat.Arrival, speaking), emotion);
        }

        /// <summary>
        /// La expresión que el guion fija para la línea: la del último paso que la fija en esa línea
        /// o antes (si hay dos en la misma, el último de la lista, como <see cref="BeatAt"/>). Se
        /// mantiene hasta que otro paso la cambia, igual que la acción. <c>null</c> si ninguno la
        /// fija: entonces manda la de la acción (<see cref="ActionEmotion"/>), y los assets que no
        /// declaran emociones no cambian.
        /// </summary>
        public static FacialEmotion? EmotionAt(NarrativeProp prop, int line)
        {
            FacialEmotion? emotion = null;
            var latest = -1;
            foreach (var beat in prop.Beats)
            {
                if (beat != null && beat.SetsEmotion && beat.Line <= line && beat.Line >= latest)
                {
                    emotion = beat.Emotion;
                    latest = beat.Line;
                }
            }

            return emotion;
        }

        /// <summary>Dónde está el personaje al empezar la línea: el destino del último paso con movimiento anterior a ella.</summary>
        public static Vector2 PositionBefore(NarrativeProp prop, int line)
        {
            var position = prop.Position;
            var latest = -1;
            foreach (var beat in prop.Beats)
            {
                if (beat != null && beat.Moves && beat.Line < line && beat.Line >= latest)
                {
                    position = beat.Destination;
                    latest = beat.Line;
                }
            }

            return position;
        }

        /// <summary>Dónde queda al terminar la línea, con su movimiento ya hecho.</summary>
        public static Vector2 PositionAfter(NarrativeProp prop, int line) => PositionBefore(prop, line + 1);

        /// <summary>
        /// El último paso con movimiento que empieza antes de la línea y que ningún paso posterior
        /// interrumpe: el camino que puede seguir en curso al llegar a ella, porque quien camina
        /// termina su camino aunque el texto avance. <c>null</c> si no hay ninguno.
        /// </summary>
        public static ActorBeat WalkUnderway(NarrativeProp prop, int line)
        {
            ActorBeat latest = null;
            foreach (var beat in prop.Beats)
            {
                if (beat != null && beat.Line < line && (latest == null || beat.Line >= latest.Line))
                {
                    latest = beat;
                }
            }

            return latest != null && latest.Moves ? latest : null;
        }

        /// <summary>El paso que empieza en esa línea, o <c>null</c>. Si hay dos, manda el último de la lista.</summary>
        public static ActorBeat BeatAt(NarrativeProp prop, int line)
        {
            ActorBeat found = null;
            foreach (var beat in prop.Beats)
            {
                if (beat != null && beat.Line == line)
                {
                    found = beat;
                }
            }

            return found;
        }

        /// <summary>
        /// Si la línea interrumpe el camino en curso: un paso nuevo, sí; una línea sin paso, no
        /// —quien camina termina su camino—. A quien termina sus pasos
        /// (<see cref="NarrativeProp.FinishesSteps"/>) no lo interrumpe nada.
        /// </summary>
        public static bool Interrupts(NarrativeProp prop, int line) =>
            !prop.FinishesSteps && BeatAt(prop, line) != null;

        /// <summary>
        /// El paso que le queda pendiente al llegar a quien termina sus pasos: el último con
        /// movimiento de los que se leyeron mientras daba <paramref name="walked"/> —después de su
        /// línea y hasta <paramref name="line"/>—, que es el que dice dónde acaba
        /// (<see cref="PositionAfter"/>). Si se leyeron varios, va directo al destino del último.
        /// <c>null</c> si no hay ninguno, y siempre para quien no termina sus pasos: a ese el paso
        /// nuevo lo interrumpió al leerse (<see cref="Interrupts"/>).
        /// </summary>
        public static ActorBeat PendingStep(NarrativeProp prop, ActorBeat walked, int line)
        {
            if (!prop.FinishesSteps)
            {
                return null;
            }

            ActorBeat pending = null;
            foreach (var beat in prop.Beats)
            {
                // Estricto: con «>=» el paso recién dado volvería a salir pendiente y el personaje
                // lo repetiría en el sitio sin fin.
                if (beat != null && beat.Moves && beat.Line > walked.Line && beat.Line <= line
                    && (pending == null || beat.Line >= pending.Line))
                {
                    pending = beat;
                }
            }

            return pending;
        }

        /// <summary>Lo que el personaje mantiene al llegar a la línea: lo que dejó el último paso anterior, o su salida.</summary>
        private static ActorAction HeldBefore(NarrativeProp prop, int line)
        {
            var held = prop.ActorStart;
            var latest = -1;
            foreach (var beat in prop.Beats)
            {
                if (beat != null && beat.Line < line && beat.Line >= latest)
                {
                    held = beat.Moves ? beat.Arrival : beat.Action;
                    latest = beat.Line;
                }
            }

            return held;
        }

        /// <summary>
        /// Quien habla de pie y quieto gesticula; cualquier otra cosa que esté haciendo —arrodillado,
        /// abrazando— manda sobre el gesto, porque es lo que el guion dijo que hace.
        /// </summary>
        private static ActorAction Talking(ActorAction held, bool speaking) =>
            speaking && held == ActorAction.Idle ? ActorAction.Talk : held;
    }
}
