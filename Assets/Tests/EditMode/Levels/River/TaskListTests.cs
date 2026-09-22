using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Game.Levels.River.Tests
{
    /// <summary>
    /// La lista de tareas del Nivel 3 y, sobre todo, cuándo se marca cada una (RF-36, INC-30).
    /// </summary>
    /// <remarks>
    /// La negativa de INC-30 va explícita: «confirmar fase marca tarea» es la lectura intuitiva y
    /// la incorrecta (riesgo R4 del plan). Sin esa prueba el error pasa.
    /// </remarks>
    public class TaskListTests
    {
        [Test]
        public void TaskList_RF36_Tareas1y2SeMarcanAlRecogerTroncosYSogas()
        {
            var sut = new TaskList();

            Assert.That(sut.Count, Is.EqualTo(4), "la lista tiene exactamente cuatro tareas (guion §1.8.1)");
            Assert.That(RiverTask.All.All(task => !sut.IsDone(task)), Is.True, "arranca sin nada hecho");

            Assert.That(sut.MarkCollected(MaterialKind.Logs), Is.EqualTo(RiverTaskId.CollectLogs));
            Assert.That(sut.IsDone(RiverTaskId.CollectLogs), Is.True, "recoger los troncos marca la tarea 1");

            Assert.That(sut.MarkCollected(MaterialKind.Ropes), Is.EqualTo(RiverTaskId.FindRopes));
            Assert.That(sut.IsDone(RiverTaskId.FindRopes), Is.True, "recoger las sogas marca la tarea 2");

            Assert.That(sut.IsDone(RiverTaskId.AssembleRaft), Is.False, "las de construcción siguen pendientes");
            Assert.That(sut.IsDone(RiverTaskId.PlaceMastAndSail), Is.False);
        }

        [Test]
        public void TaskList_INC30_LaTarea3SeMarcaAlConfirmarElAmarreNoLaBase()
        {
            var sut = new TaskList();

            sut.MarkPhaseConfirmed(1);
            Assert.That(sut.IsDone(RiverTaskId.AssembleRaft), Is.False, "la base no marca «Ensamblar la balsa»");

            Assert.That(sut.MarkPhaseConfirmed(2), Is.EqualTo(RiverTaskId.AssembleRaft));
            Assert.That(sut.IsDone(RiverTaskId.AssembleRaft), Is.True, "el amarre sí (INC-30)");

            Assert.That(sut.MarkPhaseConfirmed(3), Is.EqualTo(RiverTaskId.PlaceMastAndSail));
            Assert.That(sut.IsDone(RiverTaskId.PlaceMastAndSail), Is.True, "el mástil y la vela marcan la tarea 4");
        }

        [Test]
        public void TaskList_INC30_LaFaseDeBaseNoMarcaTareaPorSiSola()
        {
            var sut = new TaskList();

            var marked = sut.MarkPhaseConfirmed(1);

            Assert.That(marked, Is.Null, "confirmar la base no devuelve tarea alguna");
            Assert.That(RiverTask.All.Where(sut.IsDone), Is.Empty,
                "y ninguna de las cuatro queda marcada: la base es un paso de la tarea 3, no una tarea (INC-30)");
        }

        [Test]
        public void TaskList_CU09_RecogerLaTelaOElMastilNoMarcaTarea()
        {
            var sut = new TaskList();

            Assert.That(sut.MarkCollected(MaterialKind.Cloth), Is.Null, "la tela no marca nada (CU-09 FA-5a)");
            Assert.That(sut.MarkCollected(MaterialKind.Mast), Is.Null, "el mástil tampoco");
            Assert.That(RiverTask.All.Where(sut.IsDone), Is.Empty,
                "los consume la tarea 4, que es de construcción y se marca al confirmar su fase");
        }

        [Test]
        public void TaskList_RF43_UnaTareaMarcadaNoSeDesmarcaTrasUnaPruebaFallida()
        {
            var sut = new TaskList();
            sut.MarkCollected(MaterialKind.Logs);
            sut.MarkPhaseConfirmed(2);

            // Lo que pasa después de marcar —volver a la base, recoger otra vez, una prueba de
            // balsa fallida en R10— entra por estos mismos métodos, y ninguno resta.
            sut.MarkPhaseConfirmed(1);
            sut.MarkCollected(MaterialKind.Logs);
            sut.MarkCollected(MaterialKind.Cloth);

            Assert.That(sut.IsDone(RiverTaskId.CollectLogs), Is.True, "la tarea 1 sigue marcada");
            Assert.That(sut.IsDone(RiverTaskId.AssembleRaft), Is.True, "la 3 también (RF-43, CP-02)");

            // Y no existe ningún camino para desmarcar: la superficie pública solo marca. Quien
            // añada un Unmark reintroduce la penalización, y esta prueba lo dice por su nombre.
            var mutators = typeof(TaskList)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName)
                .Select(method => method.Name)
                .ToArray();
            Assert.That(mutators, Is.EquivalentTo(new[] { "IsDone", "MarkCollected", "MarkPhaseConfirmed" }),
                "la lista solo sabe marcar y consultar");
        }
    }
}
