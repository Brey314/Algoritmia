using NUnit.Framework;

namespace Game.Core.Tests
{
    /// <summary>
    /// Cuándo se funde a negro al cambiar de escena: entre la historia y el juego, en los dos
    /// sentidos, y entre dos narrativas encadenadas. Los menús cortan en seco.
    /// </summary>
    public class SceneFadeTests
    {
        [TestCase("Narrative", "Level2_Maze", ExpectedResult = true, TestName = "GameFlowRunner_RF05_FundeDeLaNarrativaALaMecanica")]
        [TestCase("Level2_Workshop", "Narrative", ExpectedResult = true, TestName = "GameFlowRunner_RF05_FundeDeLaMecanicaALaNarrativa")]
        [TestCase("Narrative", "Narrative", ExpectedResult = true, TestName = "GameFlowRunner_RF05_FundeEntreDosNarrativasEncadenadas")]
        [TestCase("MainMenu", "LevelSelect", ExpectedResult = false, TestName = "GameFlowRunner_RF05_NoFundeEntreMenus")]
        [TestCase("LevelSelect", "Narrative", ExpectedResult = false, TestName = "GameFlowRunner_RF05_NoFundeDelMenuALaNarrativa")]
        [TestCase("Narrative", "LevelSummary", ExpectedResult = false, TestName = "GameFlowRunner_RF05_NoFundeDeLaNarrativaAlResumen")]
        [TestCase("Level1_Cave", "Level1_Cave", ExpectedResult = false, TestName = "GameFlowRunner_RF07_ReiniciarLaMecanicaNoFunde")]
        public bool FadesBetween(string from, string to) => GameFlowRunner.FadesBetween(from, to);
    }
}
