# S-INSP-U · PF-RNF18-01 y PF-RNF16-01 sobre rc5 (`ca6fd91`) · 05/11/2026

Worktree desechable `%TEMP%\Algoritmia-rc5-insp` en HEAD desprendido de `ca6fd91`. Unity 6000.5.10f1. `Library/` **sembrada** copiándola de `C:\Dev\Algoritmia\Library` (robocopy; no había Editor abierto sobre el proyecto principal). Ruido común: `Packages/Coplay/.gitignore` figura modificado sin diff (fin de línea) y, tras batchmode, `ProjectSettings/ProjectSettings.asset` (ruido de CLAUDE.md).

## PF-RNF18-01 · P (variante)
- Editor abierto sobre el worktree, `coplay-mcp` conectado a él. Se editó el YAML (`N1_Config.asset` línea `MinimumEffectiveStrikes` 3→2; `N1_Hallazgo.asset` línea 0 «¿Se fue?» → «TEXTO NUEVO RNF18: la linea se cambio desde el asset.») y `AssetDatabase.Refresh`.
- Play desde `Boot`, perfil nuevo «Inspector», avanzando las narrativas: la línea nueva se vio en la tarjeta (`PF-RNF18-01_1_…png`). En la cueva (fuerza 7, muesca 5, efectivas): golpe 1 → `CanBlow=False`, Soplar con candado (`_2_`); golpe 2 → `CanBlow=True`, Soplar habilitado (`_3_`).
- `git status`: solo los dos `.asset` (`PF-RNF18-01_git_status.txt`). `check_compile_errors`: sin errores. Revertido con `git checkout`, árbol limpio; Editor cerrado.
- **Variante:** no se pudo pulsar con ratón la Game View; Play se condujo con scripts de editor efímeros (en el scratchpad, fuera del proyecto) que invocaron `onClick` de los botones y escribieron el nombre; los golpes se dieron con el `onClick` del botón «Golpear» y los deslizantes en una combinación efectiva, y las piezas se colocaron en la fogata por posición (en vez de arrastrarlas). El diálogo de ayuda inicial (pausa el tiempo) se cerró con su botón.

## PF-RNF16-01 · F estricto (propuesta PD, decide el orquestador)
Desviaciones del guion: (1) un solo worktree en serie restaurado entre sentidos (`git checkout -- .` + `git clean -fd`, sin `-x`), no dos copias; (2) las tres `Level2_*` / `Level1_Cave` se quitaron de Build Settings editando `ProjectSettings/EditorBuildSettings.asset` a mano en el worktree, no con un script de editor (mismo efecto; evita otro arranque).
| Sentido | Modo | Total | Pasadas | Fallidas | Ignoradas/inconclusas | Duración (reloj) |
|---|---|---|---|---|---|---|
| Sin N2 | EditMode | 706 | 700 | 5 | 1 ignorada | 1 min 45 s |
| Sin N2 | PlayMode | 335 | 306 | 19 | 9 ignoradas, 1 inconclusa | 35 min 26 s |
| Sin N1 | EditMode | 736 | 730 | 5 | 1 ignorada | 1 min 32 s |
| Sin N1 | PlayMode | 370 | 340 | 28 | 2 ignoradas | 38 min 40 s |
Ignoradas: `ProfileEraser_INC34_…` (solo lectura sobre carpetas) y las de «La captura exige una Game View».
- `git status` por sentido: solo borrados (98 sin N2, 86 sin N1, con sus `.meta`), `EditorBuildSettings.asset` y el ruido de arriba. Ningún archivo de los niveles restantes tocado.
- **EditMode, esperadas y todas presentes en los dos sentidos (5/5):** `Architecture_RNF15_CadaModulo…`, `…ElNombreDelAssembly…`, `Architecture_RNF16_Ningun…`, `…RetirarUnNivel…`, `Traceability_CT10_…`.
- **PlayMode sin N2, esperadas presentes (6):** `Controls_RNF02_LasCinco…`, `GameFlowRunner_RF22_JugarLaFase1…`, `GameFlowRunner_RNF14_…RetomaEnElLaberinto`, `Personajes_DA133_…` ×3 (Forest, Workshop, Maze).
- **PlayMode sin N1, esperadas presentes (11):** `Controls_RNF02_…`, `Personajes_DA133_…("Level1_Cave")`, `LevelSummary_CP07_…`, `LevelSummary_RF03_…`, `PauseMenu_HU17_ConfirmarReiniciar/Continuar/ReiniciarPide…` y `PauseMenu_INC159_` ×4.
- **Fuera de la lista del guion** (13 sin N2, 17 sin N1; la suma de River 9 + `NarrativeScene_RNF21_*` 3 + `TeacherReport_RF46_DEFRC305` 1 es común, y sin N1 se suman 4 de Wheel): `AssemblyPanel_HU12`, `AssemblyPanel_RNF03` ×2, `RiverLevel_RNF19` ×2, `RiverLevel_RNF20`, `RiverScene_RNF03`, `RiverScene_RF36`, `RiverScene_RF39` (tiempos agotados o solape a 640×480), `NarrativeScene_RNF21_ElHaloDeLaFogataPulsa…`, `…SeQuedaQuieto…`, `…LaBalsaCruza…`, `TeacherReport_RF46_DEFRC305_…` (iconos 0,0385 < 0,04) y, solo sin N1, `ForestScene_RF22` (HeldClip nulo), `ForestScene_RF26`, `MazeScene_RNF03_ConMasBloques…`, `WorkshopScene_RNF03_…` (caja a 640×480; 645,07 > 640,5). Mensajes completos en los XML.
- **Hipótesis de entorno (no reclasificado):** se restauró el worktree entero y se corrieron esas 22 pruebas con `--filter` sin quitar nada (`base-PlayMode-filtrada.xml`): 17 fallan igual con los mismos mensajes, 1 inconclusa (`PauseMenu_HU17_ConfirmarReiniciar`, la misma de sin N2) y 4 pasan. En el Editor, la suite de rc5 en 1920×1080 dio 438/438. Defecto: DEF-RC5-20.

## Estado final
Worktree eliminado y podado; Unity cerrado.
