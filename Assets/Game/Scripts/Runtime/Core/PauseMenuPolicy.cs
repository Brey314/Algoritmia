using System;
using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>Las reglas del menú de pausa (RF-07, HU-17): qué hacen «Reiniciar», «Volver a ver la historia» y «Guardar y salir».</summary>
    public static class PauseMenuPolicy
    {
        /// <summary>
        /// Repite la fase en curso desde cero, a través de <see cref="GameFlowRunner.StartPlaying"/>
        /// —igual que cualquier otro controlador, nunca llamando a <see cref="GameFlow"/>
        /// directamente— para que la recarga de escena de <c>GameFlowRunner.Apply</c> se dispare
        /// de verdad. Nunca re-bloquea un nivel ni borra un indicador ya confirmado (RF-41,
        /// CP-02): esos solo cambian con <see cref="PlayerProfile.Reach"/> /
        /// <see cref="PlayerProfile.ConfirmPhase"/>, que ni esto ni <c>TryStartPlaying</c> llaman.
        /// </summary>
        public static bool Restart(GameFlowRunner runner) =>
            runner.Flow.PlayingLevel is { } level && runner.StartPlaying(level, runner.Flow.PlayingPhase);

        /// <summary>
        /// La secuencia que presenta la fase que se está jugando, o <c>null</c> si no hay ninguna
        /// (INC-159): fuera de <see cref="GameState.Playing"/>, o una fase que arranca sin escena
        /// propia —el amarre del Nivel 3—, en cuyo caso el botón no se ofrece.
        /// </summary>
        public static string StoryFor(GameFlow flow, IReadOnlyList<PhaseNarrative> table)
        {
            if (flow == null || table == null || flow.Current != GameState.Playing
                || flow.PlayingLevel is not { } level)
            {
                return null;
            }

            foreach (var row in table)
            {
                if (row.Level == level && row.Phase == flow.PlayingPhase && !string.IsNullOrEmpty(row.SequenceId))
                {
                    return row.SequenceId;
                }
            }

            return null;
        }

        /// <summary>
        /// «Volver a ver la historia»: lleva a la escena narrativa de la fase en curso. Al terminar,
        /// <c>NarrativeSceneController.Leave</c> la devuelve a jugar esa misma fase (su
        /// <c>NextPhase</c>), así que el recorrido es el de «Reiniciar» con la historia por delante:
        /// no toca el perfil (RF-41, CP-02) y no hay ningún camino que castigue o cuente.
        /// </summary>
        public static bool WatchStory(GameFlowRunner runner, IReadOnlyList<PhaseNarrative> table) =>
            StoryFor(runner.Flow, table) is { } sequenceId && runner.StartNarrative(sequenceId);

        /// <summary>
        /// «Guardar y salir»: primero se persiste el perfil activo y solo después se cierra
        /// (RF-09). Al revés se perdería lo que el estudiante acababa de lograr.
        /// </summary>
        public static void SaveThenQuit(IProfileSaver saver, Action quit)
        {
            saver?.SaveActive();
            quit();
        }
    }
}
