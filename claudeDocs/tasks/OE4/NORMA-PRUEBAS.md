# Norma de ejecución de pruebas (OE4)

> **Para quién:** la sesión de Claude o el agente que vuelva a correr pruebas del proyecto, y Santiago.
> **Desde:** 02/10/2026, tras el candidato rc2.
> **Qué manda:** cómo se corren las pruebas. El qué y el porqué siguen en `plan.md` (criterios y veredictos),
> `casos.md` (catálogo `PF-*`) y `OE4-Resultados.md` (lo obtenido). Los comandos de esta norma están probados:
> se lanzan tal cual, sin releer los scripts.

## 0. Reglas de economía

1. **Lanzar, no investigar.** No hace falta leer `editor.ps1`, `oe4.ps1`, `suite2.ps1` ni el plan para correr lo que
   aquí se dice.
2. **Lo que tarda más de un minuto va en segundo plano** (`run_in_background`, `timeout` 7200000) y se espera su
   aviso, sin sondear ni dormir.
3. **Se lee solo el resumen.** `resumen.md` ocupa unas 30 líneas. El JSON o el XML se abren solo si algo falla, y solo
   en la prueba que falla.
4. **Correr pruebas no pide Opus.** Lo hace el propio orquestador, con un comando, o un agente Sonnet con esfuerzo
   `high` (plantilla en §7). Opus se reserva para juzgar evidencia, refutar veredictos y jugar el ejecutable.
5. **PlayMode, según la etapa** (decisión de Santiago, 01/10/2026):
   - en la implementación no se corre: se compila y se corre EditMode;
   - en la revisión de una etapa se corre solo lo afectado (§3);
   - la suite completa, una vez: en la verificación final antes del build, o cuando se toca código compartido (§2).
6. **Sin commits, etiquetas ni push**, que los hace Santiago, y sin tocar `Packages/manifest.json` (decisión D3).

## 1. Qué correr en cada caso

| Situación | Qué | Tiempo de pared |
|---|---|---|
| Verificación final antes del build, o código compartido tocado (`Game.Core`, `Game.Scaffolding`, `Game.UI`) | **`suite2.ps1`**: suite completa en dos Editores (§2) | 21 min (§6) |
| Revisión de una etapa | `editor.ps1 tests-play` / `tests-edit` filtrado (§3) | de segundos a 5 min |
| Implementación | `editor.ps1 recompile` y `tests-edit` filtrado | segundos |
| Compilar un candidato | `editor.ps1 exec build` y moverlo (§4) | ~1 min |
| Ejecutable como caja negra (`S-*`, recorridos) | `oe4.ps1`: exclusivo y en serie (§4) | de 15 a 90 min por sesión |
| Inspecciones (arte, matriz de RF) | `arte_check.py`, `rf_matrix.py --worktree` | segundos; en paralelo con cualquiera |

**`unity test` en batchmode no se usa para la suite.** Exige el Editor cerrado, y a 640×480 fallan 13 pruebas de
disposición.

## 2. Suite completa en dos Editores: `suite2.ps1`

```powershell
pwsh -NoProfile -File "C:\Dev\Algoritmia\claudeDocs\tasks\OE4\herramientas\suite2.ps1"
```

Se lanza en segundo plano. Cuando avisa, se lee la línea `FIN` del final y `resumen.md`. Ambos quedan en
`%TEMP%\Algoritmia-suite\<fecha_hora>\`, o en `-Out <carpeta>`.

**Salidas:**
- `0`: todo en verde.
- `1`: hay pruebas que fallan (listadas en `resumen.md`).
- `2`: falló la infraestructura o la cobertura quedó incompleta.

**Qué hace, en orden:**
1. Cierra el Editor de la copia `C:\Dev\Algoritmia-B` y la sincroniza con robocopy `/MIR` (`Assets`, `Packages` y
   `ProjectSettings`). La primera vez copia también `Library` (4,6 GB, ~25 s), sin `Library\Pipeline`.
2. Abre el Editor del proyecto si está cerrado, y siempre uno nuevo para la copia. Espera a que los dos estén listos y
   sin errores de compilación, y comprueba que cada uno responde con el pid de su propio proyecto.
3. Refresca el Editor del proyecto, que sin foco no ve lo editado desde fuera, y espera a que compile.
4. Repara los scripts:
   - En cada Editor busca los `MonoBehaviour` y `ScriptableObject` cuyo `MonoScript` no resuelve su clase y los
     reimporta.
   - Con la `Library` copiada, a la copia le pasó con `GameFlowRunner` y `SceneLoader`: `Boot` perdió esos componentes,
     nada navegaba y el carril B dio 35 fallos falsos (02/10/2026).
   - Si tras reimportar sigue alguno, sale con `2`.
5. Pide `list_tests` (hoy 847 pruebas: 471 EditMode y 376 PlayMode).
6. Corre los dos carriles a la vez:

   | Carril | Editor | Corre | Tiempo de prueba |
   |---|---|---|---|
   | A | el proyecto | EditMode completa, después PlayMode de `Game.UI.PlayMode.Tests` | ~1200 s |
   | B | la copia, recién abierta | PlayMode de los demás assemblies, uno por corrida (Audio, Core, Fire, River, Wheel) | ~550 s |

   El Editor B se abre de nuevo en cada suite porque `RiverLevel_RNF05` mide la memoria del Editor y solo pasa en uno
   recién abierto que no haya corrido EditMode. Un assembly de PlayMode nuevo entra solo en el carril B.
7. Cruza lo ejecutado con `list_tests`: cada prueba listada debe salir exactamente una vez. Si falta o se repite
   alguna, la suite sale con `2`.

**Las pruebas esperan por tiempo, no por frames.**
- Con dos Editores, cada uno corre a cientos de frames por segundo y comparte disco y CPU. Una espera de «600 frames»
  dura ~1 s, menos que el fundido de 0,8 s más la carga.
- Así fallaban 7 pruebas de `Game.Core` (02/10/2026), que desde entonces esperan 10 s, como las de los niveles (de 20
  a 30 s).
- Una prueba nueva que espere a que algo ocurra usa un tope en segundos (`Time.realtimeSinceStartup`), nunca un
  número de frames.

**Por qué el reparto es por assembly y no más fino.** El filtro de la API del pipeline es una sola subcadena.
`NarrativeSceneTests` dura por sí sola 1135 s, el 65 % de la PlayMode, y no se puede partir sin una exclusión que la
API no tiene. Partirla por método costaría unos 12 s por corrida para ganar unos 2 minutos.

**Mientras corre:**
- no se abre Play a mano y no se editan escenas;
- en el Editor del proyecto, `editor.ps1` responderá con salida 3 mientras un carril tenga el mutex; eso es lo
  esperado;
- las consultas de solo lectura (`status`, `scenes`, `console`) sí funcionan.

**La copia es desechable:**
- borrar `C:\Dev\Algoritmia-B` libera unos 5 GB, y la siguiente suite la rehace;
- no está versionada y no se edita;
- su `Datos/` de pruebas es propio;
- los dos Editores comparten la carpeta `LocalLow` del juego: `editor.ps1` descarta el `TestResults.xml` de la otra
  corrida, y vale el JSON.

**Si sale con 2:**

| En `registro.txt` o `resumen.md` | Causa probable | Qué hacer |
|---|---|---|
| «no quedó listo en N s» | Un diálogo modal en ese Editor: guardar escena, Safe Mode | Mirar la ventana. El rescate es `~/.claude/scripts/unity-dialog.ps1`, con el sí de Santiago en ese momento |
| «tiene N errores de compilación» | El código no compila | `editor.ps1 console`; corregir y relanzar |
| «corrida con código 3» | Escena sucia, o compilación sin terminar | `editor.ps1 scenes` y `saveall` o `recompile`; relanzar |
| «faltan N» | Una corrida abortada, o un fallo de `SetUp` de fixture, que no sale en el detalle | Abrir solo el JSON de esa corrida, en `A\` o `B\` |
| «sigue con scripts sin clase» | La `Library` de la copia quedó incoherente | Borrar `C:\Dev\Algoritmia-B\Library` y relanzar: la copia se rehace entera |
| Muchas pruebas de B esperan a `MainMenu` y no llega, con avisos «no hay GameFlowRunner» | Lo mismo: componentes de `Boot` sin script en la copia | Relanzar `suite2.ps1`, que los repara; si sigue, la fila anterior |
| `RiverLevel_RNF05` falla | El Editor B no estaba recién abierto | Relanzar `suite2.ps1`, que lo reabre |
| Muchas pruebas de disposición fallan | La Game View no está a 1920×1080 | `editor.ps1 gameview1080`; nunca batchmode |

**Preparar sin correr:** `suite2.ps1 -SoloPreparar` sincroniza la copia, deja los dos Editores listos y sale. Sirve
para lanzar a mano dos corridas filtradas en paralelo (§3).

## 3. Solo lo afectado (revisión de una etapa)

```powershell
$ED = 'C:\Dev\Algoritmia\claudeDocs\tasks\OE4\herramientas\editor.ps1'
pwsh -NoProfile -File $ED recompile                                   # tras editar código
pwsh -NoProfile -File $ED tests-edit <filtro|-> [testName|assembly]   # EditMode
pwsh -NoProfile -File $ED tests-play <filtro> [testName|assembly] [timeoutSec]
```

- El filtro es una **subcadena** del nombre completo (`testName`) o del assembly (`assembly`); `-` quiere decir «sin
  filtro». No es una expresión regular.
- Para correr dos filtros a la vez:
  1. `suite2.ps1 -SoloPreparar`;
  2. lanza cada comando en segundo plano, uno por Editor;
  3. al del Editor B antepón `$env:EDITOR_PROYECTO='C:\Dev\Algoritmia-B';`.
  Antes de usar B, la copia debe estar sincronizada (lo hace `-SoloPreparar`).

| Lo que cambió | PlayMode (tiempo de prueba) | EditMode |
|---|---|---|
| `Levels/Fire/**`, `Level1_Cave.unity`, `N1_*.asset` del nivel | `Game.Levels.Fire.PlayMode.Tests assembly` (~110 s) | `Game.Levels.Fire.Tests assembly` |
| `Levels/Wheel/**`, escenas `Level2_*` | `Game.Levels.Wheel.PlayMode.Tests assembly` (~300 s); solo el laberinto: `MazeSceneTests` (~140 s) | `Game.Levels.Wheel.Tests assembly` |
| `Levels/River/**`, `Level3_River.unity` | `Game.Levels.River.PlayMode.Tests assembly` (~135 s) | `Game.Levels.River.Tests assembly` |
| Una narrativa `N*_*.asset` | `NarrativeSceneTests` filtrado por el id de la secuencia, p. ej. `N3_EscenaFinal` (de 30 a 120 s) | `NarrativeSequence` |
| Una pantalla de `Game.UI` | su clase (`MainMenuTests` 7 s, `LevelSelectTests` 6, `ProfileSelectTests` 6, `PauseMenuTests` 6, `TeacherReportTests` 4, `CreditsTests` 2, `EraseDialogTests` 2, `LevelSummaryTests` 15, `CharacterCaptureTests` 11) | `Game.UI.Tests assembly` |
| `Game.Audio`, piezas de sonido | `Sounds` (todas las `*SoundsTests`) más `Game.Audio.PlayMode.Tests assembly` | `AudioImport` |
| `Game.Reporting` | `TeacherReportTests` | `Game.Reporting.Tests assembly` |
| Arte en `Assets/Game/Art/**` | la clase de captura de la escena que lo usa | `ArtImport` y `Game.Architecture.Tests assembly` |
| `ProjectSettings`, `Game.EditorTools`, `.asmdef` | — | `Game.Architecture.Tests assembly` y `Game.EditorTools.Tests assembly` |
| `Game.Core`, `Game.Scaffolding`, `NarrativeSceneController`, `NarrativeProp`, `ActorTimeline` | **`suite2.ps1`** | (incluida) |

Las clases de captura (`*CaptureTests`, `NarrativeScene_RF05_Captura…`) solo se corren para la escena que cambió; el
resto lo cubre la suite completa.

## 4. El ejecutable (caja negra)

**Compilar un candidato** (Editor del proyecto abierto y sin escenas sucias):
1. Aparta el candidato anterior: `Build\Algoritmia` pasa a `Build\Algoritmia_<rcN-1>`.
2. Compila con `pwsh -NoProfile -File $ED exec build`. La API ignora `output_path` y deja la salida en
   `Builds\StandaloneWindows64\`: muévela a `Build\Algoritmia\`.
3. En el registro del build, comprueba que pasaron `UnityServicesOff` (antes y después) y `LicenseNotices`.
4. Mide el tamaño sin `Datos/` ni `*_DoNotShip`, que debe ser < 500 MB.
5. Escribe `evidencias/build-<rcN>.md` con el mismo formato que `build-rc2.md`:
   - HEAD;
   - huella de `git diff HEAD -- Assets Packages ProjectSettings`;
   - huella de la lista de archivos sin seguimiento;
   - SHA-256 del `.exe`;
   - huella del contenido, con el comando de `build-rc2.md`.
6. Restaura con `git checkout` el ruido que el build deja en `ProjectSettings`.

**Jugar el ejecutable:** con `oe4.ps1`. Su guía es `herramientas/oe4-USO.md` y las sesiones son las `S-*` de
`casos.md`.
- **Exclusivo y en serie.** El arnés necesita la ventana del juego en primer plano y al usuario quieto, porque
  `SendInput` es global.
- No se corre ninguna prueba del Editor durante una sesión: un Editor que termina de compilar o entra en Play puede
  robar el foco.
- Sí pueden ir a la vez las inspecciones y la redacción.
- Una sesión, un agente Opus `xhigh` (hay que leer fotos). En un workflow, en serie y nunca en paralelo.
- **Evidencia:**
  - va en `evidencias/<rcN>/<sesión>_*`;
  - el registro de pasos, en `<sesión>_registro_de_pasos.md`;
  - nada se cita desde el scratchpad, porque se borra al cerrar la sesión: se copia antes.
- **Veredictos:** P, P parcial, F, NE o B, cada uno con la versión en que se obtuvo. Los defectos siguen la plantilla
  `DEF-nn` de `plan.md` §6.
- **Al terminar cada sesión:** el arnés restaura el registro y se borra `Datos/` del candidato. Al empaquetar la
  entrega se quitan `Datos/` y `*_DoNotShip`.

## 5. Dónde queda cada resultado

| Qué | Dónde |
|---|---|
| Suite | Copiar `resumen.md` y los JSON/XML de `A\` y `B\` a `evidencias/suites/<etiqueta>/`, y añadir su fila a `evidencias/suites/README.md` |
| Pasada sobre un candidato | Apartado nuevo en `OE4-Resultados.md`, «Pasada N sobre rcN», con su tabla de veredictos y sus medidas. Las pasadas anteriores no se reescriben: solo ganan notas fechadas |
| Tableros | La casilla, con su nota fechada y la ruta de la evidencia (`claudeDocs/tasks/Slice N/todo.md`) |
| Pruebas humanas | `Hoja-HUM.md` (H1–H13), con la plantilla que devuelve Santiago |

## 6. Tiempos medidos (referencia para planificar)

| Corrida | Tiempo |
|---|---|
| EditMode completa (471) | 9 s de prueba, más ~30 s de compilación y arranque |
| PlayMode completa en un solo Editor (376) | 1743,5 s (29 min) |
| PlayMode por assembly | UI 1192 s (de ella, `NarrativeSceneTests` 1135) · Wheel 296 · River 134 · Fire 106 · Core 15 · Audio 0 |
| **`suite2.ps1`, suite completa en dos Editores** | **21,2 min de pared** (02/10/2026, 847/847 en verde): ~1 min de preparación (sincronizar 5 s, abrir la copia 17 s, refrescar y revisar scripts ~30 s), carril A 16 s + 1199 s, carril B 577 s (Audio 7, Core 17, Fire 113, River 138, Wheel 303). En un solo Editor serían unos 30 min |
| Abrir el Editor de la copia | ~1 min, con `Library` copiada |
| Build por la API | ~45 s |
| Recorrido completo con el arnés | 16 min a pantalla completa (GP3), más el análisis de las fotos |

## 7. Plantilla de encargo para un agente corredor

```
Corre la suite completa con: pwsh -NoProfile -File "C:\Dev\Algoritmia\claudeDocs\tasks\OE4\herramientas\suite2.ps1"
en segundo plano (timeout 7200000) y espera su aviso, sin sondear.
Devuelve solo:
- el código de salida;
- la línea FIN de registro.txt;
- si hay fallos, la sección «Fallos» de resumen.md.
No leas otros archivos, no corrijas nada y no hagas commits.
```
