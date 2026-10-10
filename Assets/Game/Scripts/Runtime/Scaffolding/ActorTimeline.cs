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
        /// Lo mínimo que tiene que medir un desplazamiento, en fracciones de la ilustración, para que
        /// diga hacia dónde mira el personaje (INC-134): un paso de una centésima no se ve y el rumbo que
        /// saliera de él sería ruido.
        /// </summary>
        private const float MinimumDisplacement = 0.01f;

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

        /// <summary>
        /// La expresión con la que se ve al personaje en la línea: la que el guion fija
        /// (<see cref="EmotionAt"/>) o, si ningún paso la fija, la de la acción que hace durante la línea
        /// (<see cref="ActionEmotion"/>, con <see cref="Cue"/>). Es la misma que le pone al rig la escena
        /// narrativa (<c>cue.Emotion</c>, y si es <c>null</c> la de <c>cue.During</c>), resuelta desde los
        /// datos y no desde el estado del rig, así que la tarjeta del cuadro de diálogo (INC-148) muestra al
        /// hablante con la misma cara que el personaje en la escena.
        /// </summary>
        /// <param name="speaking">Si la línea la dice este personaje: de pie y quieto gesticula (Talk), cuya emoción es la neutra.</param>
        public static FacialEmotion EmotionOf(NarrativeProp prop, int line, bool speaking)
        {
            var scripted = EmotionAt(prop, line);
            return scripted.HasValue ? scripted.Value : ActionEmotion.For(Cue(prop, line, speaking).During);
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

        /// <summary>
        /// Si el personaje mira a la izquierda (<c>true</c>) o a la derecha (<c>false</c>) cuando su
        /// acción se ve de perfil en la línea <paramref name="line"/> (INC-134). No dice si va de perfil
        /// —eso lo decide la acción, <see cref="ActionView"/>—, solo el lado. Se resuelve en este orden:
        /// <list type="number">
        /// <item>el <see cref="ActorBeat.Facing"/> explícito del paso que rige en la línea
        /// (<see cref="ExplicitFacingAt"/>);</item>
        /// <item>hacia donde se desplaza el paso con movimiento de esa misma línea
        /// (<see cref="Heading.FacesLeft"/>), si el desplazamiento mide al menos una centésima;</item>
        /// <item>hacia donde se desplazó la última vez antes de la línea: quien se arrodilla o empuja
        /// después de caminar sigue mirando hacia donde iba;</item>
        /// <item>hacia donde se desplazará la primera vez después: quien espera antes de partir ya
        /// mira hacia allá;</item>
        /// <item>hacia el centro de la ilustración, si nunca se mueve: un personaje a la izquierda mira
        /// a la derecha y al revés, como quien mira la escena.</item>
        /// </list>
        /// </summary>
        public static bool FacesLeftAt(NarrativeProp prop, int line)
        {
            var explicitFacing = ExplicitFacingAt(prop, line);
            if (explicitFacing.HasValue)
            {
                return explicitFacing.Value;
            }

            var beat = BeatAt(prop, line);
            if (beat != null && beat.Moves)
            {
                var own = HeadingOfDisplacement(PositionBefore(prop, line), beat.Destination);
                if (own.HasValue)
                {
                    return own.Value;
                }
            }

            // El último desplazamiento anterior que se vea (o con rumbo fijado a mano). Con dos en la misma línea
            // manda el último de la lista, como BeatAt; uno que no se ve (menos de una centésima) y no fija el
            // rumbo no cuenta y deja valer el previo.
            bool? previous = null;
            var latest = -1;
            foreach (var step in prop.Beats)
            {
                if (step != null && step.Moves && step.Line < line && step.Line >= latest)
                {
                    // El rumbo fijado a mano de ese paso pesa más que su desplazamiento (INC-134).
                    var heading = FacesLeftOf(step.Facing) ?? HeadingOfDisplacement(PositionBefore(prop, step.Line), step.Destination);
                    if (heading.HasValue)
                    {
                        previous = heading;
                        latest = step.Line;
                    }
                }
            }

            if (previous.HasValue)
            {
                return previous.Value;
            }

            // El primer desplazamiento posterior que se vea.
            bool? next = null;
            var earliest = int.MaxValue;
            foreach (var step in prop.Beats)
            {
                if (step != null && step.Moves && step.Line > line && step.Line <= earliest)
                {
                    var heading = FacesLeftOf(step.Facing) ?? HeadingOfDisplacement(PositionBefore(prop, step.Line), step.Destination);
                    if (heading.HasValue)
                    {
                        next = heading;
                        earliest = step.Line;
                    }
                }
            }

            if (next.HasValue)
            {
                return next.Value;
            }

            return prop.Position.x >= 0.5f;
        }

        /// <summary>
        /// El rumbo que el guion fija a mano para la línea: el <see cref="ActorBeat.Facing"/> del paso que
        /// rige en ella —el último que empieza en la línea o antes, y si hay dos en la misma, el último de
        /// la lista, como <see cref="EmotionAt"/>—. <c>true</c> = izquierda, <c>false</c> = derecha, <c>null</c>
        /// = Auto, o ningún paso todavía. **Rige un solo paso:** uno posterior que no lo declara lo devuelve
        /// a Auto, así que un rumbo fijado no sobrevive, sin quererlo, a un paso que camina hacia el otro lado.
        /// </summary>
        internal static bool? ExplicitFacingAt(NarrativeProp prop, int line)
        {
            ActorBeat inForce = null;
            foreach (var beat in prop.Beats)
            {
                if (beat != null && beat.Line <= line && (inForce == null || beat.Line >= inForce.Line))
                {
                    inForce = beat;
                }
            }

            return inForce == null ? null : FacesLeftOf(inForce.Facing);
        }

        /// <summary>
        /// El rumbo de un paso que arranca en <paramref name="from"/>: el suyo explícito, o hacia donde se
        /// desplaza, o <paramref name="fallback"/> si no decide nada. Es la regla del paso encadenado de quien
        /// termina sus pasos (<see cref="PendingStep"/>), que sale de donde llegó y no de donde el paso decía
        /// empezar.
        /// </summary>
        public static bool FacesLeftOfStep(ActorBeat step, Vector2 from, bool fallback)
        {
            if (step == null)
            {
                return fallback;
            }

            var explicitFacing = FacesLeftOf(step.Facing);
            if (explicitFacing.HasValue)
            {
                return explicitFacing.Value;
            }

            return (step.Moves ? HeadingOfDisplacement(from, step.Destination) : null) ?? fallback;
        }

        /// <summary>Hacia dónde mira un desplazamiento de <paramref name="from"/> a <paramref name="to"/>; <c>null</c> si mide menos de <see cref="MinimumDisplacement"/>.</summary>
        private static bool? HeadingOfDisplacement(Vector2 from, Vector2 to)
        {
            var delta = to - from;
            if (delta.sqrMagnitude < MinimumDisplacement * MinimumDisplacement)
            {
                return null;
            }

            return Heading.FacesLeft(delta);
        }

        private static bool? FacesLeftOf(ActorFacing facing)
        {
            switch (facing)
            {
                case ActorFacing.Left: return true;
                case ActorFacing.Right: return false;
                default: return null;
            }
        }

        /// <summary>
        /// Lo que el personaje mantiene al llegar a la línea: lo que dejó el último paso anterior, o su salida.
        /// Interno y no privado para que las pruebas de datos reconozcan el paso que solo fija la expresión
        /// (INC-148): el que repite esta acción, sin moverse.
        /// </summary>
        internal static ActorAction HeldBefore(NarrativeProp prop, int line)
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
