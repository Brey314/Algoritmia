# Suite en dos Editores — 09/10/2026 00:52

HEAD `eb9be13` + árbol `f2afbb9ed338` · Unity 6000.5.10f1 · pared **24.4 min** (carril A hasta 00:52, carril B hasta 00:39)

| Carril | Corrida | Total | Pasan | Fallan | Omitidas | s |
|---|---|---|---|---|---|---|
| A | editmode  | 711 | 706 | 4 | 1 | 16.4 |
| A | playmode Game.UI.PlayMode.Tests | 177 | 177 | 0 | 0 | 1359.6 |
| B | playmode Game.Audio.PlayMode.Tests | 4 | 4 | 0 | 0 | 7.2 |
| B | playmode Game.Core.PlayMode.Tests | 12 | 12 | 0 | 0 | 22.2 |
| B | playmode Game.Levels.Fire.PlayMode.Tests | 64 | 64 | 0 | 0 | 117.4 |
| B | playmode Game.Levels.River.PlayMode.Tests | 58 | 57 | 0 | 0 | 147.4 |
| B | playmode Game.Levels.Wheel.PlayMode.Tests | 102 | 102 | 0 | 0 | 317.7 |

**EditMode** 711 = 706 + 1 omitidas, 4 fallos · **PlayMode** 417 = 416 + 0 omitidas, 0 fallos
**Cobertura** (list_tests, sin [Explicit]): 1128 listadas · 1128 ejecutadas · faltan 0 · repetidas 0

**Fallos**
- `Game.Scaffolding.Tests.CharacterRigTests.CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte("Papa")`: Papa: existe Lienzo/Perfil
- `Game.Scaffolding.Tests.CharacterRigTests.CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte("Mama")`: Mama: existe Lienzo/Perfil
- `Game.Scaffolding.Tests.CharacterRigTests.CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte("Nina")`: Nina: existe Lienzo/Perfil
- `Game.Scaffolding.Tests.CharacterRigTests.CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte("Nino")`: Nino: existe Lienzo/Perfil
