# PF-RNF16-01 · Prueba de exclusión RNF-16 en los dos sentidos (01/10/2026)

**Veredicto: CUMPLE.** Retirar el Nivel 2 (`Game.Levels.Wheel`) o el Nivel 1 (`Game.Levels.Fire`) deja un proyecto que
**compila con 0 errores** (`error CS` = 0 en los seis `Editor.log` guardados) y en el que **pasan enteras las pruebas de los
niveles que quedan, de `Game.Core`, `Game.Scaffolding`, `Game.Audio`, `Game.Reporting` y `Game.UI`**, salvo las
transversales que cargan por nombre una escena del nivel retirado y las estructurales que exigen el repositorio completo.
Ningún archivo de los niveles restantes se tocó.

## Método

- Copia **temporal** `C:\Dev\Algoritmia-rnf16` (solo `Assets/`, `Packages/`, `ProjectSettings/`; sin `Library`, `Temp`, `Logs`,
  `obj`, `Build`, `UserSettings`; robocopy), Editor del repositorio cerrado, `unity test . --mode EditMode|PlayMode`
  (Unity 6000.5.10f1, batchmode, pantalla de 640×480). La copia se borró al terminar; el repositorio no cambió (`git status` igual antes y después).
- **Sin N2:** se borró `Assets/Game/Scripts/Runtime/Levels/Wheel/`, `Assets/Tests/{EditMode,PlayMode}/Levels/Wheel/` y
  `Assets/Tests/EditMode/Content/` (el único assembly que referencia los tres niveles; con sus `.meta`) y se quitaron las tres
  `Level2_*` de `ProjectSettings/EditorBuildSettings.asset` (YAML editado en la copia).
- **Sin N1:** se restauró Wheel y se hizo lo mismo con `Levels/Fire/`, sus pruebas, `Content/` y `Level1_Cave`.
- Las escenas `.unity` del nivel retirado siguen en `Assets/` (con *scripts* sin resolver, que Unity solo avisa); no entran a las pruebas porque ya no están en Build Settings.
- **Control:** con el árbol completo restaurado en la copia se repitieron en batchmode las pruebas que fallaban por entorno, para demostrar que fallan igual sin quitar nada.

## Resultados

| Corrida | Total | Pasan | Fallan | Omitidas | Errores de compilación | XML |
|---|---|---|---|---|---|---|
| Sin N2 · EditMode | 360 | 354 | 5 | 1 | 0 | `sinN2-EditMode.xml` |
| Sin N2 · PlayMode | 271 | 245 | 17 | 9 | 0 | `sinN2-PlayMode.xml` (+ `sinN2-PlayMode-rerun-River.xml`, repetición de las de River: mismas 11) |
| Sin N1 · EditMode | 382 | 376 | 5 | 1 | 0 | `sinN1-EditMode.xml` |
| Sin N1 · PlayMode | 305 | 281 | 22 | 2 | 0 | `sinN1-PlayMode.xml` |
| Control (árbol completo, filtro de las 14 de entorno) | 38 | 24 | 14 | 0 | 0 | `control-PlayMode-batchmode.xml` |

(Omitidas: en EditMode `ProfileEraser_INC34_…` (entorno). En PlayMode, pruebas que se saltan solas en batchmode sin gráficos: `LevelSelect_RNF19_…` y `MainMenu_RNF20_…` (2, en los dos sentidos) y, sin N2, 7 del Nivel 1 (`FireLevel_RF20/RF21/RNF21`, `FirePanel_RF16/RF19/RNF19/RNF20`).)
Los `*-Editor.log.txt` son el `Editor.log` de cada corrida.

## Fallos, todos explicados

### A. Estructurales: exigen el repositorio completo, no la independencia (EditMode, 5 en cada sentido)
- `Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas`, `…_ElNombreDelAssemblyCoincideConSuRutaBajoScripts`,
  `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel` y `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos`:
  leen los `.asmdef` del disco y esperan los tres niveles (`KeyNotFoundException: 'Game.Levels.Wheel'|'Fire'`,
  `DirectoryNotFoundException` en la última sin N1). Comprueban justamente lo contrario de lo que se hizo aquí (que el árbol tenga los tres), así que se espera que fallen en un árbol sin un nivel.
- `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra`: los RF del nivel retirado (sin N2: RF-24..; sin N1: RF-14, 15, 16, 18, 19, 21) pierden las pruebas que los nombraban al borrarlas.

### B. Pruebas transversales que cargan por nombre una escena del nivel retirado (PlayMode)
Fallan con `Scene '<escena>' couldn't be loaded because it has not been added to the active build profile` (la lista prevista en `casos.md`, PF-RNF16-01):
- **Sin N2 (6):** `Controls_RNF02_LasCincoEscenasJugables…` (`Level2_Forest`), `GameFlowRunner_RF22_JugarLaFase1DelNivel2…`,
  `GameFlowRunner_RNF14_…EntrarAlNivel2RetomaEnElLaberinto`, `Personajes_DA133_CapturaCadaMecanica…` en `Level2_Forest`, `Level2_Workshop` y `Level2_Maze`.
- **Sin N1 (8):** `Controls_RNF02_…` (`Level1_Cave`), `Personajes_DA133_…("Level1_Cave")`, `LevelSummary_CP07_…`, `LevelSummary_RF03_…`
  y cuatro de `PauseMenuTests` (`HU17_Ofrece…`, `…Continuar…`, `…ReiniciarPideConfirmacion…`, `…ConfirmarReiniciar…`).

### C. Fallos de entorno (batchmode 640×480 sin ventana), idénticos en el árbol completo
Son los de la lista conocida (`todo.md` T05, `Fase-5-6-Resultados.md`) más los de disposición de Wheel; el **control** los reproduce los 14 sin quitar nada.
- Disposición a 640×480: `AssemblyPanel_RNF03_…` (×2), `RiverScene_RNF03_…`, `NarrativeScene_RNF01_LaLineaMasLarga…` (135 px en una caja de 134),
  `ForestScene_RF26_…`, `MazeScene_RNF03_…`, `WorkshopScene_RNF03_…` (estas tres solo existen cuando Wheel está presente: aparecen **sin N1**, no sin N2).
- Tiempos agotados sin cuadros reales de pantalla: `SceneLoader_RF05_ConFundidoCargaEnNegro…` (el negro se pinta con `OnGUI`), `AssemblyPanel_HU12_…`,
  `RiverLevel_RNF19_…` (×2), `RiverLevel_RNF20_…`, `RiverScene_RF36_…`, `RiverScene_RF39_…` (capturas).
- Contabilidad: sin N2 = 6 (B) + 11 (C: 1 SceneLoader + 9 River + 1 Narrative) = 17. Sin N1 = 8 (B) + 14 (C, las del control) = 22.

## Conclusión
Ninguna falla señala una dependencia entre niveles: todo fallo es (A) una prueba que mide el árbol completo, (B) una prueba transversal
prevista que nombra una escena retirada, o (C) un fallo de entorno que también ocurre sin tocar nada. Las pruebas de `Game.Core`, `Game.Scaffolding`,
`Game.Audio`, `Game.Reporting` y de los dos niveles que quedan pasan como en la línea base, salvo lo listado.
