using System.Collections.Generic;

namespace Game.Levels.River
{
    /// <summary>
    /// La lista de tareas permanente del Nivel 3 y qué está hecho (RF-36, HU-11).
    /// </summary>
    /// <remarks>
    /// C# plano, sin escena. **No existe forma de desmarcar**, y no es un olvido: una tarea
    /// marcada no se pierde por una prueba de balsa fallida ni por nada posterior (RF-43, CP-02).
    /// Quien añada un <c>Unmark</c> reintroduce la penalización. Los textos viven en el asset
    /// (<see cref="RiverLevelConfig.TaskLabels"/>); aquí solo hay estado y regla.
    /// </remarks>
    public class TaskList
    {
        private readonly HashSet<RiverTaskId> _done = new HashSet<RiverTaskId>();

        public int Count => RiverTask.All.Length;

        public bool IsDone(RiverTaskId task) => _done.Contains(task);

        /// <summary>Recoger un material. Devuelve la tarea recién marcada, o nada (CU-09 FA-5a).</summary>
        public RiverTaskId? MarkCollected(MaterialKind kind) => Mark(RiverTask.ForCollected(kind));

        /// <summary>Confirmar una fase de ensamblaje. La base no marca nada (INC-30).</summary>
        public RiverTaskId? MarkPhaseConfirmed(int phase) => Mark(RiverTask.ForConfirmedPhase(phase));

        private RiverTaskId? Mark(RiverTaskId? task)
        {
            if (task.HasValue)
            {
                _done.Add(task.Value);
            }

            return task;
        }
    }
}
