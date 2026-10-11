# Suite en dos Editores — 10/10/2026 16:48

HEAD `292b108` + árbol `b2a4d015fe44` · Unity 6000.5.10f1 · pared **25.9 min** (carril A hasta 16:48, carril B hasta 16:37)

| Carril | Corrida | Total | Pasan | Fallan | Omitidas | s |
|---|---|---|---|---|---|---|
| A | editmode  | 749 | 748 | 0 | 1 | 24.4 |
| A | playmode Game.UI.PlayMode.Tests | 178 | 178 | 0 | 0 | 1400 |
| B | playmode Game.Audio.PlayMode.Tests | 4 | 4 | 0 | 0 | 12.4 |
| B | playmode Game.Core.PlayMode.Tests | 12 | 12 | 0 | 0 | 37.4 |
| B | playmode Game.Levels.Fire.PlayMode.Tests | 64 | 64 | 0 | 0 | 137.7 |
| B | playmode Game.Levels.River.PlayMode.Tests | 58 | 57 | 1 | 0 | 187.9 |
| B | playmode Game.Levels.Wheel.PlayMode.Tests | 102 | 102 | 0 | 0 | 363.2 |

**EditMode** 749 = 748 + 1 omitidas, 0 fallos · **PlayMode** 418 = 417 + 0 omitidas, 1 fallos
**Cobertura** (list_tests, sin [Explicit]): 1167 listadas · 1167 ejecutadas · faltan 0 · repetidas 0

**Fallos**
- `Game.Levels.River.Tests.RiverAccessibilityTests.RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`: completado: ningún cuadro salta más de lo que el ojo sigue (máximo 0,056)
