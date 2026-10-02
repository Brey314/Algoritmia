# Tablero — Slice 4: Progreso y registro

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification · `MCP` = la verificación **exige** el corredor de pruebas conectado.

> ⚠️ **R1 — los Slices 1, 2 y 3 no están hechos.** Este módulo consume indicadores que aún no
> emite nadie. No abrir P02 antes del Checkpoint R-E del Slice 3. Solo **P00**, **P01**, **P03**
> y **P08** son independientes.

> ⚠️ **R3 — RF-47 borra archivos de verdad.** `P10` corre **solo** sobre un directorio temporal
> que ella misma crea y destruye. Escribir la guarda que impide tocar `Datos/` o
> `persistentDataPath` reales **antes** que el código de borrado.

> ℹ️ **El corredor de pruebas MCP ya no es un riesgo: es `P00`.** Este slice se abre
> resolviéndolo o dejando escrita la decisión de no hacerlo.

---

## Fase 0 — Cimientos del slice

- [x] **P00 · Confirmar o instalar el corredor de pruebas MCP de Unity** — `XS` · sin código
      `SPEC.md` §Comandos, `CLAUDE.md` §Comandos · depende de: —
      Declarado 23/09/2026: sin Editor abierto, `mcp__coplay-mcp__get_unity_editor_state` y
      `mcp__rider__get_unity_compilation_result` no responden (comportamiento documentado, no
      falla). El corredor confirmado es la CLI `unity test`, que ya corre EditMode con éxito
      (ver P01). Las siete tareas `MCP` de este slice (P05..P07, P09, P11) se ejecutan a mano en
      el Test Runner con el Editor abierto cuando toque cablear escena, y ese resultado se
      declara explícitamente en cada una — no se instaló ningún servidor MCP nuevo.
- [x] **P01 · Assembly `Game.Reporting` y su frontera** — `XS` · `EM`
      RNF-15, **RNF-16**, `SPEC.md` §Estructura · depende de: P00, Slice 3 R01
      ⚠️ Prueba **negativa**: `Game.Reporting` no referencia a ningún assembly de nivel
      Creado `Game.Reporting` (solo referencia `Game.Core`) y su assembly de pruebas EditMode.
      `AssemblyDependencyTest` ampliado con `Architecture_RNF16_ReportingNoReferenciaANingunAssemblyDeNivel`
      y `Architecture_RNF16_RetirarUnNivelNoRompeElInformeDocente`. 14/14 pruebas de Architecture
      pasan vía `unity test --mode EditMode --filter "Game\.Architecture\.Tests"`.

### ✅ Checkpoint P-A — Cimientos
- [x] `run_unity_tests` responde, o queda **escrito** que se corre a mano y se declara
- [x] `Game.Reporting` existe y no referencia a ningún nivel
- [x] La exclusión de RNF-16 sigue pasando con el módulo de informe presente
- [x] Revisado con el usuario
      *(01/10/2026: Decisión — revisado por Santiago el 30/09/2026 (acta D10, §5), tras comprobar en
      verde en la suite completa del 01/10/2026 la frontera de `Game.Reporting`
      (`Architecture_RNF16_ReportingNoReferenciaANingunAssemblyDeNivel`,
      `Architecture_RNF16_RetirarUnNivelNoRompeElInformeDocente`); el corredor de pruebas quedó
      escrito en P00 y desde el 30/09 `claudeDocs/tasks/OE4/herramientas/editor.ps1` corre las
      suites con el Editor abierto.)*

---

## Fase 1 — El lado del estudiante: resumen sin cifras

> El resumen por nivel **ya está planeado** en Slice 1 `T18`, Slice 2 `W16` y Slice 3 `R14`.
> Aquí solo va lo que exige que los tres existan: unificar y barrer.

- [x] **P02 · `LevelSummaryContent` unificado y barrido transversal de cifras** — `S` · `EM`
      RF-45, RF-17, RF-12, RNF-01, RNF-18, CP-03, CP-07, HU-14, **INC-26** · depende de: P01, Slice 3 R14
      ⚠️ Reutiliza `DialogueRunner` y el marco `A10`. Si acaba creando una vista propia, va mal
      **Nota:** los Slices 1-3 ya unificaron los tres niveles sobre `LevelSummaryController` +
      `LevelSummaryComposer` (`Game.UI`), su propia tablilla — no el marco `A10` que este plan
      asumía; es la arquitectura ya revisada en sus checkpoints y no se rehace aquí. Lo que faltaba
      era el barrido transversal: añadidos a `LevelSummaryComposerTests.cs`
      `LevelSummary_RF45_NingunResumenDeLosTresNivelesContieneUnDigito`,
      `LevelSummary_RF17_NingunResumenEmiteJuicioDeValor`,
      `LevelSummary_CP03_NoExisteClaseDePuntajeEnElProyecto`,
      `LevelSummaryContent_RNF01_NingunaOracionSupera20Palabras`. 13/13 pruebas pasan.

### ✅ Checkpoint P-B — El estudiante no ve cifras
- [x] Los tres resúmenes barridos: cero dígitos, cero juicios de valor
- [x] Un solo componente resuelve los tres (`LevelSummaryController`/`LevelSummaryComposer`)
- [x] Revisado con el usuario
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de Claude
      del 01/10/2026: `LevelSummary_RF45_NingunResumenDeLosTresNivelesContieneUnDigito`,
      `LevelSummary_RF17_NingunResumenEmiteJuicioDeValor`,
      `LevelSummary_CP03_NoExisteClaseDePuntajeEnElProyecto` y
      `LevelSummaryContent_RNF01_NingunaOracionSupera20Palabras`, en verde en la suite de rc2
      (`claudeDocs/tasks/OE4/evidencias/suites/rc2-final/`); en el ejecutable los tres resúmenes salen
      sin cifras ni juicios —PF-RF17-01, PF-RF45-01 y PF-RF45-02 en P sobre rc1, y GP3 confirma
      PF-RF45-01 a 03 sobre rc2— (`claudeDocs/tasks/OE4/OE4-Resultados.md`, §5 y §8.5; capturas
      `claudeDocs/tasks/OE4/evidencias/rc2/GP3_N1.png`, `GP3_N2.png` y
      `GP3_resumen_N3_y_escena_final.png`).

---

## Fase 2 — El lado del docente: `TeacherReport`

- [x] **P03 · `ProfileRepository` — los perfiles del equipo** — `S` · `EM`
      RF-46, RF-02, RNF-07, RNF-09, RNF-13, CU-11 (FA-2a), HU-16, INC-34 · depende de: P01
      Creado en `Game.Reporting`, contra `IFileSystem` (reutiliza el de `Game.Core`/`SaveStore`).
      3/3 pruebas pasan (`ProfileRepositoryTests.cs`).
- [x] **P04 · `IndicatorReport` — agregación por nivel y por fase** — `M` · `EM`
      RF-45, RF-46, RNF-09, CP-09, OE1 §3.6.1, CU-11, HU-16, **INC-35**, INC-27 · depende de: P03
      ⚠️ **Ningún agregado que §3.6.1 no defina** — nada de promedios ni «nivel de dominio»
      `IndicatorReport` + `PhaseIndicators` + `LevelReportSection` en `Game.Reporting`. 3/3 pruebas
      pasan (`IndicatorReportTests.cs`); la de RNF-09 fija por reflexión que `PerformanceIndicators`
      solo tiene los cuatro campos.
- [x] **P05 · Escena `TeacherReport` y su estado en la FSM** — `M` · `PM` `MCP`
      RF-46, RNF-02, RNF-04, CT-06, CU-11, HU-16 · depende de: P04
      `coplay-mcp` reconectó el 24/09/2026. Escena creada con `edit-scene` (andamiaje de editor
      efímero, borrado al terminar) y registrada en Build Settings. `GameFlowRunner.Scenes` gana
      `[TeacherReport] = "TeacherReport"`; `GameFlowRunner` expone `PortableRoot`/`FallbackRoot`
      para que `Game.UI` construya su propio `ProfileRepository` sin que `Game.Core` conozca ese
      tipo. **Los controladores viven en `Game.UI`, no en `Game.Reporting`** (`TeacherReportController`,
      `IndicatorTableView`, `EraseConfirmationDialog`) — es donde ya viven todos los controladores
      de pantalla (`MainMenuController`, `ProfileSelectController`, `LevelSummaryController`);
      `Game.Reporting` se queda puro dato/lógica, sin `UnityEngine.UI`, tal como fijó P01.
      `GameFlow_RF46_TeacherReportSeAlcanzaDesdeElInicioYVuelveAEl` (EditMode) y
      `TeacherReport_RF46_SeAlcanzaDesdeElMenuYVuelveSinPerderElPerfilActivo` (PlayMode) en verde.
- [x] **P06 · Tabla de los cuatro indicadores — aquí sí van las cifras** — `M` · `PM` + `VV` `MCP`
      RF-46, RF-45, RNF-01, RNF-19, RNF-20, CP-03 (su límite), CP-09, CU-11, HU-16,
      INC-35 · depende de: P05
      ⚠️ Prueba de exclusión: **ninguna ruta del estudiante alcanza esta pantalla**
      `IndicatorTableView` clona una fila por fase (mismo patrón que `LevelSummaryController`),
      agrupadas por nivel; el tiempo se formatea con `ResolutionTimeFormat` (Game.Reporting, 6/6
      casos EditMode en verde). Verificado en vivo (captura de `capture_ui_canvas`) con perfiles
      reales: cabecera y filas alineadas tras corregir el ancho de columna
      (`LayoutElement(preferredWidth=0, flexibleWidth=1)` en las seis celdas — la cabecera con
      texto largo repartía el ancho distinto a las filas, que arrancan vacías). Un nivel no jugado
      muestra «Sin datos» en las cuatro columnas, no ceros — verificado con el perfil «pepe» del
      equipo, sin fases confirmadas. Prueba de exclusión CP-03 añadida a `GameFlowTests.cs`:
      `TeacherReport_CP03_NingunaRutaDelEstudianteAlcanzaLaPantallaDeCifras` recorre cada
      `GameState` y confirma que solo `MainMenu` puede pedir `TeacherReport`.
- [x] **P07 · Acceso desde el menú principal** — `S` · `PM` `MCP`
      RF-46, RF-01, RNF-02, RNF-03, CT-06, CU-11, HU-16, HU-18 · depende de: P06
      Botón «Progreso del equipo» añadido a `MainMenu.unity` bajo Créditos/Salir, cableado a
      `MainMenuController.teacherReportButton` → `Runner.GoTo(GameState.TeacherReport)`.
      `MainMenu_RF46_OfreceLaOpcionDeProgresoDelDocente` y
      `TeacherReport_RF46_ConsultarNoAlteraNingunPerfil` en verde; `MainMenuTests` existentes
      (raycast, solapamiento, no-throw sin Boot) extendidos para cubrir también este botón.

### ✅ Checkpoint P-C — Informe docente completo
- [x] Un perfil con los tres niveles se consulta entero, **por nivel y por fase** (INC-35) —
      verificado en vivo con el perfil «Ana» (siete filas: 1+3+3 fases)
- [x] Ninguna ruta del estudiante llega a la pantalla de cifras (CP-03) — prueba de exclusión
      sobre los ocho `GameState`
- [x] Un nivel no jugado aparece **sin datos**, no con ceros — verificado en vivo con «pepe»
- [x] Contraste verificado sobre la tabla más larga (RNF-20) — par Carbón/Marfil, el mismo que
      ya sostiene RNF-20 «con holgura» en el resto de la interfaz (`Direccion_de_Arte.md`)
- [x] Revisado con el usuario
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de Claude
      del 01/10/2026: `TeacherReport_RF46_PresentaLosCuatroIndicadoresPorNivelYPorFase`,
      `TeacherReport_RF46_ElDetalleNombraElPerfilQueSeEstaMirando`,
      `TeacherReport_RF46_ConsultarNoAlteraNingunPerfil` y
      `TeacherReport_CP03_NingunaRutaDelEstudianteAlcanzaLaPantallaDeCifras`, en verde en la suite de
      rc2 (`claudeDocs/tasks/OE4/evidencias/suites/rc2-final/`); sobre el ejecutable, PF-RF46-01 a 03
      en P —siete filas por perfil, «Sin datos» en lo no jugado, solo lectura y, desde rc2, el
      encabezado «Perfil de …» con el perfil que se mira— (`claudeDocs/tasks/OE4/OE4-Resultados.md`,
      §5, §8.3 y §8.4; `claudeDocs/tasks/OE4/evidencias/rc2/PF-RF46-01_informe_perfil_de_OE4_A.png`).
      La consulta queda sin protección de acceso: ningún RF la pide.

---

## Fase 3 — Eliminación de datos

> **P08 no depende de P03..P07** y conviene adelantarla: sus pruebas deberían estar verdes antes
> de que exista un botón que dispare un borrado irreversible.

- [x] **P08 · `ProfileEraser` — borrado en las dos rutas, sin residuos** — `M` · `EM`
      **RF-47**, **RNF-11**, RNF-09, CT-07, CU-12, HU-16, **INC-34**, supuesto 1 · depende de: P01
      **Hallazgo 24/09/2026: no hace falta código nuevo.** `SaveStore.Delete` (`Game.Core`, Slice 1)
      ya borra las dos rutas, no reporta éxito parcial y no toca otros perfiles, con pruebas que ya
      citan RF-47 e INC-34 (`SaveStore_RF47_BorraElPerfilDeLasDosRutas`,
      `SaveStore_RF47_NoAfectaAOtrosPerfiles`, `SaveStore_INC34_BorraDesdeLaRutaDeRespaldoAunqueDatosNoSeaEscribible`,
      `SaveStore_RNF11_UnBorradoParcialNoSeReportaComoExito`, en `Game.Core.Tests`). `ProfileSession.Delete`
      además limpia el perfil activo si es el borrado — resuelve de una vez la **pregunta abierta 3**
      en el mismo sentido que proponía este plan. Una clase `ProfileEraser` nueva sería reinventar
      esa lógica: P09 llama a `ProfileSession.Delete` directamente desde `TeacherReport`.
- [x] **P09 · Confirmación explícita e irreversibilidad en la UI** — `S` · `PM` `MCP`
      RF-47, RNF-11, RNF-19, RNF-20, CU-12 (FA-4a), HU-16 · depende de: P08, P06
      ⚠️ La acción destructiva **no** puede ser la opción por defecto
      `EraseConfirmationDialog` (`Game.UI`) reutiliza el patrón ya construido y revisado en
      `ProfileSelectController.DeletePanel/Dialogo` (Slice 1): alerta (`ui_alerta.png`), aviso de
      irreversibilidad, «Cancelar» en marfil neutro y «Eliminar datos» marcado en ámbar `#E8A33D`
      con icono de papelera (`ui_papelera.png`) — la señal «acción destacada» es el color +
      el icono, nunca solo el color (RNF-19), y no es la opción por defecto. Llama a
      `ProfileSession.Delete` a través de `EraseConfirmationDialog.Session` (nueva propiedad
      inyectable, mismo patrón que `ProfileSelectController.Session`) — cero borrado nuevo, per P08.
      Verificado en vivo (clic real simulado): abrir → Cancelar no cambia el perfil en disco;
      3/3 pruebas PlayMode en verde (`EraseDialogTests.cs`, directorio temporal real, nunca
      `Datos/` del proyecto).
- [x] **P10 · Prueba de residuos sobre disco real** — `S` · `EM` (integración)
      **RNF-11**, RF-47, RNF-07, CU-12, HU-16, INC-34 · depende de: P09 *(adelantada: prueba
      `SaveStore.Delete`, no la UI de P09, así que no tenía dependencia técnica real)*
      ⚠️ Solo sobre directorio temporal propio. Guarda explícita contra `Datos/` y
      `persistentDataPath` reales
      `ProfileEraserDiskTests.cs`: escenario escribible pasa sobre disco real; el escenario de
      solo lectura se **declaró omitido con motivo** — este equipo no hace cumplir el atributo de
      solo lectura de carpetas en NTFS, tal como el plan permite explícitamente.

### ✅ Checkpoint P-D — Eliminación conforme
- [x] Un perfil se elimina con confirmación y desaparece de **las dos rutas**
      (`EraseDialog_RF47_TrasConfirmarElPerfilDesapareceDeLaLista`, `SaveStore_RF47_*`)
- [x] Cancelar no cambia nada **en disco** (no solo en la navegación)
      (`EraseDialog_CU12_CancelarNoRealizaNingunCambioEnDisco`)
- [x] Ningún otro perfil se ve afectado (`SaveStore_RF47_NoAfectaAOtrosPerfiles`)
- [x] Árbol de almacenamiento inspeccionado tras el borrado: **cero residuos** (P10, escenario escribible)
- [x] Revisado con el usuario
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de Claude
      del 01/10/2026: `EraseDialog_RF47_ExigeConfirmacionExplicitaYAdvierteIrreversibilidad`,
      `EraseDialog_CU12_CancelarNoRealizaNingunCambioEnDisco`,
      `EraseDialog_RF47_TrasConfirmarElPerfilDesapareceDeLaLista`, los dos `SaveStore_RF47_*` y
      `ProfileEraser_RNF11_SobreDiscoRealNoQuedaNingunaEntradaDelPerfil`, en verde en la suite de rc2
      (`claudeDocs/tasks/OE4/evidencias/suites/rc2-final/`); sobre el ejecutable, PF-RF47-01 a 03 y
      PF-RNF11-01 en P: tras borrar cuatro perfiles desde el panel y desde el informe, sus nombres
      no aparecen en la carpeta portable, en `LocalLow`, en `%TEMP%` ni en el registro; el perfil de
      la ruta de respaldo (INC-34) se borra de ella (PF-RF47-03), y en rc2 los perfiles borrados
      desaparecen de `Datos/`
      (`claudeDocs/tasks/OE4/OE4-Resultados.md`, §4.5, §5 y §8.4;
      `claudeDocs/tasks/OE4/evidencias/S-DOC/S-DOC_residuos_rastreo_tras_borrado.txt`).

---

## Fase 4 — Cierre del proyecto

- [x] **P11 · Cierre de CP-03 y RNF-09 sobre el juego completo** — `M` · `EM` (la parte `PM` queda
      como recorrido manual, ver nota) · CP-03, RF-17, RF-45, RNF-09, **CT-10**, INC-26, INC-27,
      OE1 §3.6.1 (notas 3 y 5) · depende de: P02, P07, P10
      Excepciones permitidas y **cerradas** en la prueba: contador «n de 5» (RF-24, se resta el
      índice de formato `{0}`/`{1}`, no el campo entero) y lista de cuatro tareas (RF-36, en la
      práctica no le hace falta excepción: `RiverSceneController` la implementa sin cifra —
      «Nunca «2 de 4»: el avance se lee tarea a tarea»— así que el barrido simplemente no
      encuentra nada que exceptuar ahí).
      **`ContentInvariantTests.cs`** (assembly nuevo `Game.Content.Tests`, el único que referencia
      los tres niveles a la vez — justificado en el propio archivo: es una auditoría de cierre,
      no código de producción) recorre el grafo serializado de once tipos de contenido (todo
      `ScriptableObject` con texto salvo `ReportContent` —la única pantalla donde una cifra es
      correcta— y `CreditsContent` —año de producción, no retroalimentación de desempeño—),
      saltando los campos que terminan en «Id» (identificadores técnicos, nunca texto leído). La
      primera corrida encontró exactamente lo esperado — IDs de secuencia narrativa, identificadores
      de objetos de catálogo y las cadenas de formato de dos niveles— y ninguna cifra real.
      **`SaveStore_RNF09_ElJsonDeUnPerfilCompletoNoTieneCampoFueraDeLaListaCerrada`** (ampliado en
      `SaveStoreTests.cs`) confirma la lista cerrada con un perfil que jugó los tres niveles.
      **`Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra`** (`TraceabilityTests.cs`,
      `Game.Architecture.Tests`) deriva la matriz de los nombres de método: los 47 RF están citados.
      321/321 EditMode en verde (1 skip declarado de antes). **Pendiente de Santiago:** el
      recorrido visual completo del juego (una pantalla a la vez) que pedía la parte `PM` es
      manual — se solapa con el Golden Path de P12 y queda para esa pasada.
- [ ] **P12 · Presupuestos y ejecución portable** — `S` · manual (plan OE4)
      RNF-04, RNF-05, RNF-06, RNF-07, RNF-08, RNF-10, RNF-13, RNF-14, CT-03, HU-15,
      HU-18 · depende de: P11
      — sigue abierta el 01/10/2026 por la decisión D-b (acta D10, §5), por la columna del segundo
      equipo y la fila del Golden Path cronometrado: las llena Santiago con los guiones H3 (carpeta
      portable y cargas en el equipo 2), H4 (sin red) y H8 (Golden Path con cronómetro) de
      `claudeDocs/tasks/OE4/Hoja-HUM.md`. La columna
      del equipo 1 está medida sobre el ejecutable (tabla de abajo), y en ese equipo pasan RNF-07
      —copia portable en una ruta con espacio, sin instalar nada ni elevar privilegios
      (PF-RNF07-01)—, RNF-10 —cero conexiones en 1312 muestras sobre rc2 (PF-RNF10-01)—, RNF-13
      —tres recorridos completos sin incidencias bloqueantes— y RNF-14 —nueve cierres forzados, dos
      de ellos en el instante del guardado, sin ningún perfil truncado (PF-RNF14-01 a 05)—
      (`claudeDocs/tasks/OE4/OE4-Resultados.md`, §3, §4.6, §5, §8.3, §8.5 y §8.6).

### Tabla de mediciones — `P12`

Números, no adjetivos. Rellenar en el equipo de referencia.

| Medición | Presupuesto | Equipo 1 | Equipo 2 |
|---|---|---|---|
| Carga `Boot` | < 10 s | escena inicial: arranque hasta `MainMenu` cargado ≤ 3,23 s | |
| Carga `MainMenu` | < 10 s | 0,11 s | |
| Carga `LevelSelect` *(fila añadida el 01/10/2026)* | < 10 s | 0,08 s | |
| Carga `Credits` *(fila añadida el 01/10/2026)* | < 10 s | 0,04 s | |
| Carga `Narrative` | < 10 s | 0,78 s | |
| Carga `Level1_Cave` | < 10 s | 0,16 s | |
| Carga `LevelSummary` *(fila añadida el 01/10/2026)* | < 10 s | 0,04 s | |
| Carga `Level2_Forest` | < 10 s | 0,05 s | |
| Carga `Level2_Workshop` | < 10 s | 0,09 s | |
| Carga `Level2_Maze` | < 10 s | 0,09 s | |
| Carga `Level3_River` | < 10 s | 0,09 s | |
| Carga `TeacherReport` | < 10 s | 0,06 s | |
| Memoria máxima en ejecución | < 2 GB | 446 MB de trabajo · 1 179 MB privada | |
| Tamaño del paquete | < 500 MB | 479,0 MB | |
| Golden Path completo | 20–40 min | | |

*(01/10/2026: la columna «Equipo 1» se midió sobre el ejecutable en el equipo de desarrollo —AMD
Ryzen 5 7600X, 32 GB de RAM, NVIDIA GeForce RTX 5070 Ti y Windows 11 Home de 64 bits, con un monitor
de 1920 × 1080—. Las cargas son el peor caso por escena entre rc1 (188 cargas en 20 lanzamientos) y
rc2 (74 en 7), leído de la línea `RNF-04` del `Player.log`; `Boot` no pasa por `SceneLoader`, así que
se da la cota superior del arranque. Memoria y paquete son de rc2, el ejecutable vigente; rc1 dio
457 MB de trabajo y 1 195 MB privada. Fuente: `claudeDocs/tasks/OE4/OE4-Resultados.md`, §4.1 a §4.3
y §8.6. Las filas de `LevelSelect`, `Credits` y `LevelSummary` completan las doce escenas del build.
La columna «Equipo 2» y la fila «Golden Path completo» quedan para Santiago por la decisión D-b
—guiones H3 y H8 de `claudeDocs/tasks/OE4/Hoja-HUM.md`—: los tres recorridos del arnés, de 87, 33 y
16 min, no miden a un estudiante.)*

### ✅ Checkpoint P-E — Proyecto completo
- [x] **RF-46 y RF-47 cerrados** → los 45 RF de prioridad Alta implementados (P05-P10, 24/09/2026)
- [x] RF-06 (Media) y RF-21 (Baja): **implementados**, no declarados fuera — RF-06 en
      `NarrativeVisitPolicy`/`DialogueRunner`/`NarrativeSceneController` (Slice 1); RF-21 en
      `CaveLightingController` (Nivel 1). Los 47 RF están implementados, no solo los 45 de
      prioridad Alta.
- [x] Los diez criterios de éxito de `SPEC.md` revisados uno por uno
      — cerrado el 01/10/2026: la tabla de los diez, con su evidencia y su veredicto vigente, está en
      `claudeDocs/tasks/Slice 4/Slice-4-Resultados.md` («Criterios de éxito de SPEC.md, revisión uno
      por uno»). Cumplen del todo el 1 al 6, el 8 y el 10; el 7 cumple con lo ejecutado del OE4 —103
      casos con veredicto, 89 P, 13 P parcial, 0 F y 1 NE, con una eficacia provisional de 89 / 89
      sin contar P parcial ni NE, y 44 de los 47 RF con un caso aprobado sobre el
      ejecutable—, a falta del KPI final sobre el catálogo completo (T24), y el 9 cumple en el
      equipo 1, con la mitad de dos equipos y red apagada a cargo de Santiago (decisión D-b;
      `claudeDocs/tasks/OE4/Hoja-HUM.md`, H3 y H4).
- [x] `INCONSISTENCIAS.md` revisado: ningún hallazgo reabierto por el código
      — cerrado el 01/10/2026: la rev. 16 (01/10/2026) registra y cierra INC-118 a INC-130 con la
      regla del 29/09, y las 111 filas de su tabla, de INC-01 a INC-130, están «Cerrado»; ninguna se
      reabrió, porque las correcciones de rc2 solo añaden notas fechadas a INC-97 (DEF-SPIKE-01 y su
      residuo DEF-RC2-01) y a INC-80 (los `ScrollRect` del informe docente y de los créditos). Siguen
      abiertos solo PG-05 y PG-06 y los pendientes de Santiago (`claudeDocs/INCONSISTENCIAS.md`:
      cabecera, control de cambios y «Residuos y puntos abiertos»).
- [x] Golden Path del juego entero, dos veces, sin incidencias (RNF-13)
      — cerrado el 01/10/2026: tres recorridos completos sobre el ejecutable con el arnés
      `claudeDocs/tasks/OE4/herramientas/oe4.ps1`, de la pantalla de inicio a los créditos y de
      vuelta, cada uno con perfil nuevo: GP1 (87 min, ventana de 1920 × 1080) y GP2 (33 min, pantalla
      completa) sobre rc1, y GP3 (16 min, pantalla completa) sobre rc2. Los tres terminaron sin
      bloqueos, cierres inesperados ni estados irrecuperables y con las siete fases en el JSON. Los
      defectos no bloqueantes que vieron los dos primeros —en el laberinto, DEF-GP1-01 a 03, y las
      conexiones de red de los servicios de Unity, DEF-SPIKE-01— se corrigieron en rc2 y se
      reverificaron, salvo tres valores `unity_connect.*` y un contador y un identificador de sesión del
      reproductor que siguen en el registro (DEF-RC2-01,
      residuo del motor), y GP3 no halló ninguno del juego (`claudeDocs/tasks/OE4/OE4-Resultados.md`,
      §3, §8.3 y §8.5). Sus fotos de control, pantalla por pantalla, son el recorrido visual que P11
      dejó para esta pasada (`claudeDocs/tasks/OE4/evidencias/rc2/GP3_*.png`). La duración con una
      persona es H8 (Slice 3).
- [x] **PG-01** (título) y **PG-02** (nombre del guía) cerrados y en pantalla *(29/09/2026: el
      guion §1.2 cierra PG-01 con «Algoritmia», que la pantalla de inicio muestra desde `2893cdb`;
      PG-02 estaba cerrado desde el 02/09 y Algoritm habla en las narrativas y en las pistas. Los
      créditos no llevan el título: RF-08 no lo pide.)*
- [x] **RNF-12**: formato de consentimiento informado en los anexos del proyecto
      — cerrado el 01/10/2026: el formato —autorización de los acudientes y asentimiento del
      estudiante, conforme a la Ley 1581 de 2012 y al Decreto 1377 de 2013— va en blanco como Anexo F
      del trabajo de grado, en su lista de anexos y en el cuerpo
      (`docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx`; conversión en
      `docs/md/Trabajo_de_Grado_Entrega_Plantilla_28jul.md`), y está en
      `claudeDocs/tasks/OE4/Consentimiento-RNF12.md`. Las copias firmadas acompañan la sesión con
      estudiantes que hace Santiago (decisión D-b; `claudeDocs/tasks/OE4/Hoja-HUM.md`, H1 y H2).
- [x] **RNF-22**: inspección integral del contenido — sin violencia explícita, publicidad,
      compras integradas ni enlaces externos, en los tres niveles y en los créditos
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de Claude
      del 01/10/2026: ningún texto de `Assets/Game/Data`, de las escenas ni de los prefabs lleva
      enlaces, dominios, precios, compras, tiendas ni publicidad —cero resultados de «http», «www»,
      «.com», «.org», «precio», «compra», «tienda», «anuncio», «publicidad» y «suscrib», fuera de la
      cabecera YAML de Unity—; `Packages/manifest.json` no trae paquetes de compras ni de anuncios, y
      sobre el ejecutable vigente no se observó ninguna conexión de red en 1312 muestras de 2 s, con
      el arranque de cada lanzamiento sin muestrear (PF-RNF10-01; `claudeDocs/tasks/OE4/OE4-Resultados.md`,
      §8.6). Las capturas del tercer recorrido completo —los tres niveles, sus narrativas, la escena
      final y los créditos— no muestran violencia explícita, marcas ni anuncios, y los créditos no
      llevan enlaces (PF-RF08-01) (`claudeDocs/tasks/OE4/evidencias/rc2/GP3_N1.png`, `GP3_N2.png`,
      `GP3_N3_y_creditos.png` y `GP3_resumen_N3_y_escena_final.png`). El caso PF-RNF22-01 del
      catálogo del OE4 se ejecuta aparte, en la pasada de ese carril.
- [ ] **CT-02**: el ejecutable corre en un equipo **sin tarjeta gráfica dedicada**, dentro de
      los presupuestos de RNF-04 y RNF-05
      — sigue abierta el 01/10/2026 por la decisión D-b (acta D10, §5): la ejecuta Santiago con el
      guion H5 de `claudeDocs/tasks/OE4/Hoja-HUM.md` en un equipo con solo gráfica integrada —el
      equipo 1 tiene una NVIDIA GeForce RTX 5070 Ti y no sirve para este criterio— y se marca con las
      cargas y la memoria que mida.
- [x] Revisado con el usuario. **Fin del prototipo.**
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de Claude
      del 01/10/2026: el ejecutable vigente es rc2 (`claudeDocs/tasks/OE4/evidencias/build-rc2.md`),
      con 103 casos con veredicto —89 P, 13 P parcial, 0 F y 1 NE— (`claudeDocs/tasks/OE4/OE4-Resultados.md`,
      §8.8), y las demás casillas de este checkpoint están cerradas salvo CT-02; P12 sigue abierta
      por la columna del segundo equipo y el Golden Path cronometrado. Las dos quedan con Santiago
      por la decisión D-b (`claudeDocs/tasks/OE4/Hoja-HUM.md`, H3, H4, H5 y H8).

---

## Assets visuales — `plan.md` §Assets visuales del Slice 4

**Tres assets, todos de interfaz.** Sin personajes, sin escenarios y **sin chroma key**: se
generan sobre marfil plano `#F7EFE2` y se usan tal cual. Registrar en `CreditsContent.asset`
(Slice 1, T08) — CT-09, RNF-23.

**Cinco bloques fijos por prompt**, copiados palabra por palabra antes de la descripción:
`[1 CONTEXTO] [2 ESTILO] [3 PALETA] [4 ENTREGA] [5 PROHIBICIONES]`. Un asset generado sin los
cinco se descarta y se vuelve a pedir. La paleta y las especificaciones salen de
`claudeDocs/Direccion_de_Arte.md`.

**El informe docente es CLARO, no oscuro.** Carbón `#3A1E18` sobre marfil `#F7EFE2`: texto oscuro
sobre fondo claro, nunca al revés (`Direccion_de_Arte.md` §10.3 y §17). **No hay rojo de error**
en ninguna parte de la interfaz: la alerta es ámbar `#E8A33D` más una forma (§12.3).

**El resumen de fin de nivel no genera arte**: reutiliza el marco de diálogo `A10` del Slice 1.
Es lo que hace que se lea como andamiaje y no como pantalla de puntaje.

- [x] **D1 · Iconografía de los cuatro indicadores** — RF-46, RNF-19, OE1 §3.6.1
      Uno por **indicador**, no por faceta: las facetas se mapean a RF, no a indicadores
      El reloj de arena va **sin números y sin cuenta regresiva** — este juego no tiene temporizador
      **Boceto provisional, no arte final** (`ui_ind_{intentos,errores,pasos,tiempo}_boceto.png`,
      `Assets/Game/Art/UI/Common/`): el generador de imágenes de `coplay-mcp` devolvió
      `401 Unauthorized` con los dos proveedores el 24/09/2026 — no son credenciales que se
      puedan arreglar desde el código. Dibujados con Python/Pillow (formas geométricas simples,
      no la ilustración vectorial cartoon de la dirección de arte) e **importados y cableados de
      verdad**: Sprite/Single, 100 PPU, sobre la cabecera de la tabla de `TeacherReport` (icono
      encima del nombre del indicador, verificado en vivo). Cuando el generador tenga
      credenciales, D1 se regenera con el prompt ya escrito abajo y estos cuatro archivos se
      sustituyen por los finales — mismo hueco, sin volver a cablear. No registrado todavía en
      `CreditsContent.asset`: es boceto, no autoría final (CT-09).
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), que acepta el boceto como arte
      definitivo de los cuatro indicadores, tras la verificación de Claude del 01/10/2026: los cuatro
      `ui_ind_*_boceto.png` están en `Single` a 100 PPU, con alfa, sin halo y con nombre según §15.4
      (`claudeDocs/tasks/OE4/evidencias/arte/sprites.md`), se referencian una vez cada uno en la
      cabecera de `TeacherReport.unity` y se ven en el informe del ejecutable vigente
      (`claudeDocs/tasks/OE4/evidencias/rc2/PF-RF46-01_informe_perfil_de_OE4_A.png`). Como arte
      propio los cubre la línea «Entornos, objetos e interfaz: originales del proyecto, salvo los
      iconos de pausa.» de `Assets/Game/Data/CreditsContent.asset`. Que monten sobre el texto de la
      cabecera es una observación menor que queda documentada (OBS-SDOC-1,
      `claudeDocs/tasks/OE4/OE4-Resultados.md`, §8.9).
- [x] **D2 · Layout del panel de `TeacherReport`** — RF-46, RNF-20, **INC-35** — **resuelto sin
      maqueta**: la pantalla real ya está construida en `TeacherReport.unity` (P05/P06), así que
      la maqueta que este punto pedía para guiar la construcción ya no hace falta. La estructura
      real difiere del layout propuesto (fila plana con nivel+fase por columna en vez de tres
      bloques con cabecera propia) pero cumple el mismo criterio — filas por fase, un nivel con
      más filas que otro se ve — verificado en vivo con el perfil «Ana» (Checkpoint P-C).
- [x] **D3 · Diálogo de confirmación de eliminación** — RF-47, RNF-11, RNF-19, CU-12 — **resuelto
      sin maqueta nueva**: reutiliza el diálogo ya construido y aprobado de
      `ProfileSelectController` (Slice 1) en vez de generar uno nuevo desde cero (P09).
      «Cancelar» es neutro y grande; «Eliminar datos» se distingue por color ámbar + icono de
      papelera, no solo por tamaño — la diferencia con el `DeletePanel` original: ahí es
      «Eliminar» quien lleva el color de atención en vez de ser el discreto, decisión ya tomada
      en Slice 1 y que P09 no reabrió.
- [x] Cada asset pasa la **checklist de `Direccion_de_Arte.md` §17** y su línea «Verificación» —
      pendiente: §17 es la checklist del arte **final**, y D1 hoy es boceto
      — revisado por Santiago el 30/09/2026 (acta D10, decisión D-a), tras la verificación de Claude
      del 01/10/2026: con el boceto aceptado como arte definitivo, §17 se pasó sobre los cuatro iconos
      de D1, los únicos archivos del slice —D2 y D3 no generaron arte—. Cumplen las comprobaciones que
      se miden: nombre según §15.4, PNG con alfa importado sin comprimir, sin halo y sin tocar el
      borde del lienzo (`claudeDocs/tasks/OE4/evidencias/arte/sprites.md`); se distinguen en grises por
      su silueta, 1−IoU 0,83–0,96 (RNF-19, `claudeDocs/tasks/OE4/evidencias/arte/rnf19.md`); son
      estáticos (RNF-21), no llevan cifras —el reloj de arena va sin números (CP-03)— y los cubre la
      línea de créditos de la interfaz original (CT-09). Las de estilo —contorno, sombreado, formas
      redondeadas— las da por buenas la aceptación del boceto, con sus desvíos a la vista: el reloj
      de triángulos huecos y las puntas afiladas del reloj y de la flecha.
- [x] Exportar PNG con alfa (los iconos van sobre cualquier fila, incluida la alterna `#E0D4C0`) —
      hecho para el boceto de D1 (`alphaIsTransparency`, fondo transparente)
- [x] Mismo `Pixels Per Unit` que los tres slices anteriores — 100, fijado en el importador
- [x] **`D2` y `D3` son maquetas, no arte final**: quedaron sin objeto — P05/P06/P09 construyeron
      la pantalla y el diálogo reales directamente, sin pasar por una maqueta intermedia
- [x] Desaturar `D1` y `D3` y verificar que se siguen distinguiendo (RNF-19) — pendiente de D1 final
      — cerrado el 01/10/2026: con D1 ya final por la decisión D-a, en escala de grises
      (`arte_check.py rnf19`) los cuatro iconos de D1 se separan por silueta y gris en sus seis
      parejas (1−IoU 0,83–0,96, Δ 72–84) y, en D3, la papelera y la alerta, por 1−IoU 0,70
      (`claudeDocs/tasks/OE4/evidencias/arte/rnf19.md`). En el diálogo de borrado, «Eliminar datos» lleva
      además la papelera y su rótulo
      (`claudeDocs/tasks/OE4/evidencias/S-DOC/PF-RF47-02_dialogo_eliminar_datos.png`).
- [x] Verificar RNF-20 sobre la tabla **construida y llena de datos**, no sobre la maqueta —
      hecho en vivo con el perfil «Ana» (Checkpoint P-C): par Carbón/Marfil, contraste alto

---

## Bloqueantes y decisiones pendientes

- [x] **R1 · Cerrar los Slices 1, 2 y 3** antes de abrir P02. Bloqueante duro.
      *(01/10/2026: Decisión — se cumplió: el tercer slice cerró su código el 21/09 y el acta D08
      (23/09) abrió el cuarto; P02 se hizo ese día con los tres niveles emitiendo indicadores
      (`LevelSummary_RF45_NingunResumenDeLosTresNivelesContieneUnDigito`). Lo que sigue abierto en
      los tableros de los Slices 1 a 3 se cierra en cada uno.)*
- [x] **P00 · Corredor de pruebas MCP.** Primera tarea del slice, por decisión explícita.
      *(01/10/2026: Decisión — es la P00 de la Fase 0, cerrada el 23/09: el corredor confirmado es
      `unity test` y la decisión quedó escrita (`SPEC.md` y `CLAUDE.md` §Comandos; Checkpoint P-A).
      Desde el 30/09 `editor.ps1` corre además las suites con el Editor abierto.)*
- [x] **Pregunta abierta 1 · ¿`Game.Reporting` referencia a los niveles?** Resuelta: **no** — P01
      lo deja probado (`Architecture_RNF16_ReportingNoReferenciaANingunAssemblyDeNivel`).
- [x] **Pregunta abierta 2 · ¿El informe docente necesita protección de acceso?** Ningún RF la
      pide. Este plan **no la añade**: sería una mecánica fuera de los documentos. Si la
      institución la espera, radicarla como cambio de requerimiento. *(Sigue abierta: es una
      decisión de alcance, no de implementación — nadie la ha cerrado con Santiago todavía.)*
      *(01/10/2026: Decisión — Santiago decidió el 30/09/2026 (acta D10, §5) que el informe docente
      no lleve protección de acceso, porque ningún RF la pide; así queda el alcance.)*
- [x] **Pregunta abierta 3 · ¿Qué pasa si el docente elimina el perfil activo?** Resuelta en el
      sentido propuesto: `ProfileSession.Delete` ya llamaba a `ClearActiveProfile()` desde el
      Slice 1 (hallazgo de P08) — vuelve a `MainMenu` sin perfil seleccionado.
- [x] **Pregunta abierta 4 · Unidad del tiempo de resolución.** Resuelta en el sentido propuesto:
      se persiste en segundos (sin cambios en `PerformanceIndicators`/el JSON) y se presenta en
      minutos y segundos vía `ResolutionTimeFormat` (P06).
- [x] **PG-01 · título del producto** y **PG-02 · nombre del guía.** Último slice: cerrarlos antes
      de P12. Aparecen en inicio, créditos y las quince escenas narrativas. *(Cerrados el
      29/09/2026 en el guion §1.2: «Algoritmia» en la pantalla de inicio, sin repetirse en
      créditos; Algoritm en las narrativas y las pistas.)*
- [x] **RNF-12 · consentimiento informado.** Verificación documental, no de código. Sin tarea
      porque no la tiene; queda listado en el Checkpoint P-E.
      — cerrado el 01/10/2026 con la casilla de RNF-12 del Checkpoint P-E: el formato va en blanco
      como Anexo F del trabajo de grado (`docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx`;
      conversión en `docs/md/Trabajo_de_Grado_Entrega_Plantilla_28jul.md`) y está en
      `claudeDocs/tasks/OE4/Consentimiento-RNF12.md`; las copias firmadas acompañan la sesión con
      estudiantes (decisión D-b; `claudeDocs/tasks/OE4/Hoja-HUM.md`, H1).
