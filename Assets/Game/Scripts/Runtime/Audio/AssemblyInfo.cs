using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Game.Audio.Tests")]
[assembly: InternalsVisibleTo("Game.Audio.PlayMode.Tests")]
// El nivel comprueba qué sonó tras cada golpe sin ampliar la superficie pública del gestor.
[assembly: InternalsVisibleTo("Game.Levels.Fire.PlayMode.Tests")]
[assembly: InternalsVisibleTo("Game.Levels.Wheel.PlayMode.Tests")]
[assembly: InternalsVisibleTo("Game.UI.PlayMode.Tests")]
