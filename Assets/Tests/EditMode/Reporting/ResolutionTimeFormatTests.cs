using NUnit.Framework;

namespace Game.Reporting.Tests
{
    public class ResolutionTimeFormatTests
    {
        [TestCase(0f, "0:00")]
        [TestCase(5f, "0:05")]
        [TestCase(59f, "0:59")]
        [TestCase(60f, "1:00")]
        [TestCase(91.5f, "1:32")] // redondea al segundo más cercano
        [TestCase(3661f, "61:01")] // sin tope de una hora: no hay temporizador que lo limite
        public void TeacherReport_RF46_ElTiempoSeFormateaEnMinutosYSegundos(float seconds, string expected)
        {
            Assert.That(ResolutionTimeFormat.MinutesAndSeconds(seconds), Is.EqualTo(expected));
        }
    }
}
