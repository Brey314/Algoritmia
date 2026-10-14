using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Game.Scaffolding.Tests")]
// HelpButtonIntro (INC-158): la escena de la cueva y su prueba de disposición viven en Game.Levels.Fire.
[assembly: InternalsVisibleTo("Game.Levels.Fire.Tests")]
[assembly: InternalsVisibleTo("Game.Levels.Fire.PlayMode.Tests")]
