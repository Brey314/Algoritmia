# Fase 1 — Navegación mínima: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **T05–T08 implementadas** — corte de este documento: 7 de septiembre de 2026
Plan técnico: [`plan.md`](plan.md) · Tablero: [`todo.md`](todo.md) · Contrato: `claudeDocs/SPEC.md`
Fase anterior: [`Fase-0-Resultados.md`](Fase-0-Resultados.md)

Este documento registra qué se implementó en la Fase 1, con qué pruebas se verificó y qué
resultado dieron. **Las cuatro tareas (T05–T08) están terminadas y verdes; el Checkpoint B sigue
abierto**: el build portable del 08/09 confirmó RNF-07 y dejó a medias RNF-14, y encontró un
residuo que incumple RNF-11 (§4). No reabre decisiones de `SPEC.md`: las cita.

> **Estado al 25/09/2026:** el texto de abajo es la foto del 07/09 y se conserva tal cual. Al día
> siguiente empezó la **revisión visual de T05–T08** —`145a63e` (08/09) y `2893cdb` (09/09)—, que
> rehízo estas pantallas según los mockups: tipografías Baloo 2 y Nunito, título «Algoritmia»,
> créditos en rejilla de pares papel/persona, borrado de perfil desde el panel del estudiante
> (RF-47) y el hundido de botón `ButtonPressFeedback`. Después, el Slice 2 hizo que cada ficha del
> menú de niveles declare su narrativa de apertura (`faaaa13`, 10/09), y el Slice 4 añadió al inicio
> el acceso al informe docente (`9c34924`, 24/09). Cada afirmación que ya no es cierta lleva una
> nota fechada en su sitio; el detalle está en el **Anexo** al final. Las cifras de §3 son las del
> 07/09: la corrida vigente de la suite está en
> [`Slice-4-Resultados.md`](../Slice%204/Slice-4-Resultados.md), «Corrida completa de la suite
> (25/09/2026)». El hallazgo de RNF-11 de §4.1 **sigue abierto**:
> `com.unity.modules.unityanalytics` sigue en `Packages/manifest.json:48` y `usePlayerLog: 1` en
> `ProjectSettings/ProjectSettings.asset:98`.

---

## 1. Alcance de la fase

La Fase 1 es el módulo `sistema-navegacion`: de la pantalla de inicio al menú de niveles, con un
perfil real que sobrevive al cierre de la aplicación. No hay nivel jugable todavía.

| Tarea | Qué entrega | Modo de prueba | Estado |
|---|---|---|---|
| T05 | Pantalla de inicio: título, Jugar / Créditos / Salir; guardado al salir | PlayMode + VV | ✅ terminada |
| T06 | Perfil de un solo nombre: lista de perfiles + crear uno nuevo validado | EditMode + PlayMode | ✅ terminada |
| T07 | Menú de niveles con desbloqueo progresivo | EditMode + PlayMode + VV | ✅ terminada |
| T08 | Pantalla de créditos mínima | PlayMode | ✅ terminada |

**T04b** (arreglo de la medición de RNF-04 en `SceneLoader`) se hizo entre fases y está en
`Fase-0-Resultados.md` §3.4; aquí solo se menciona porque un test suyo volvió a tocarse (§3.3).

---

## 2. Qué quedó en el repositorio

### 2.1 Pantalla de inicio (T05)

- **Escena `MainMenu`** poblada: `EventSystem` (con `InputSystemUIInputModule`, no el módulo
  legado), `Canvas` Screen Space-Overlay con `CanvasScaler` a 1920×1080 (match 0.5), un
  `Background` de color marfil siempre visible, el título y los tres botones **Jugar, Créditos y
  Salir** en columna centrada. *(Vencido el 25/09/2026: desde `9c34924` (24/09, Slice 4) hay además
  «Progreso del equipo», que abre el informe docente (RF-46); la pantalla se rehízo con el mockup en
  `145a63e`/`2893cdb`. Ver Anexo, A.6.)*
- **`GameTitleConfig`** — ScriptableObject con el título del producto. Vive en un asset porque
  PG-01 sigue abierto: cambiar el título es editar `Assets/Game/Data/GameTitleConfig.asset`, no
  recompilar (RNF-18). Valor provisional confirmado por el usuario el 06/09/2026: **«Algoritm»**.
  Coincide con el nombre del guía (Dir. Arte §7.6, INC-44) — es deliberado. *(Vencido el
  25/09/2026: desde `2893cdb` (09/09) el asset dice «Algoritmia» (`GameTitleConfig.asset:15`), que es
  también el valor por defecto de `GameTitleConfig.cs:13`; el título ya no coincide con el del guía,
  que sigue siendo Algoritm. PG-01 sigue abierto. Ver Anexo, A.2.)*
- **`MainMenuController`** — adaptador delgado. Pone el título desde el SO y cablea los tres
  botones en `Start()`. «Jugar» intercambia `MainPanel` por `ProfilePanel` (ver T06). «Créditos»
  transiciona a `Credits` (escena de T08). «Salir» guarda el perfil activo **antes** de cerrar
  (RF-09), en ese orden: al revés se perdería lo que el estudiante acababa de lograr. *(Vencido el
  25/09/2026: hoy `Start()` cablea cuatro botones; el cuarto, `teacherReportButton`, abre el informe
  docente (`MainMenuController.cs:58`, desde `9c34924`, 24/09).)*
- **Persistencia real cableada por primera vez:**
  - **`DiskFileSystem : IFileSystem`** — implementación sobre `System.IO`. Comprueba que la
    carpeta sea escribible con un archivo de sonda, no lo asume (INC-34). Devuelve rutas con
    barras hacia delante.
  - **`ProfileSession`** — construida de forma diferida por `GameFlowRunner` (así una prueba que
    no la toca no escribe la sonda en disco). Apunta `SaveStore` a `Datos/` junto al ejecutable
    —en el Editor, la raíz del proyecto— con caída a `Application.persistentDataPath` (RNF-07,
    RNF-11, INC-34). `/[Dd]atos/` se añadió al `.gitignore`.
  - **`IProfileSaver`** — la interfaz que consume la UI que puede cerrar sesión, sin conocer
    cómo se persiste.

### 2.2 Perfil de un solo nombre (T06)

- **`ProfileSelect` es un panel dentro de `MainMenu`, no una escena.** `SPEC.md` §Estructura no
  lista `ProfileSelect.unity`; el plan sí. Gana `SPEC.md`, y ya lo decía el comentario commiteado
  en `GameFlowRunner` desde T04. `MainMenu` se parte en `Canvas/MainPanel` y `Canvas/ProfilePanel`
  (inactivo por defecto); «Jugar» los intercambia.
- **`GameFlowRunner.Apply` no recarga la escena que ya está activa.** Antes, volver de
  `ProfileSelect` a `MainMenu` habría recargado la escena entera. Ahora, si el estado destino
  vive en la escena activa, el cambio es un intercambio de paneles y lo hace la UI. Se añadió
  `GameFlowRunner.SelectProfile(profile)` como envoltura de `GameFlow.TrySelectProfile`.
- **`ProfileSession` pasó a ser la fachada de perfiles** del `sistema-navegacion`:
  `ExistingProfileNames()`, `Load(name)`, `Create(name)` (valida contra los existentes) y
  `Save(profile)`, además del `SaveActive()` de T05.
- **`ProfileSelectController`** — adaptador delgado del panel. Lista los perfiles guardados
  clonando un prototipo; ofrece crear uno nuevo con **un único campo de nombre** (RNF-09). La
  validación (FA-01 nombre vacío, FA-02 duplicado, más nombre no válido como archivo) reutiliza
  `PlayerProfile.Create` de T02 y mapea cada rechazo a un aviso en pantalla. Perfil existente →
  `SelectProfile` (carga su progreso). Perfil nuevo → `SelectProfile` + `StartNarrative("N1_Apertura")`
  (HU-01 FA-03; la escena `Narrative` es de T10, hasta entonces avisa por consola).
  *(Ampliado al 25/09/2026: desde `145a63e` (08/09) cada perfil de la lista lleva también un botón
  para borrarlo, con confirmación, y `ProfileSession` ganó `Delete` (RF-47). Ver Anexo, A.4.)*

### 2.3 Menú de niveles (T07)

- **`LevelUnlockPolicy`** — C# plano. `UnlockAfterCompleting(profile, completed)` habilita el
  nivel siguiente y **nada más** (RF-03); completar el último no habilita ninguno. Nunca
  retrocede —lo garantiza `PlayerProfile.Reach`—, así que un fallo posterior no re-bloquea nada
  (CP-02, RF-41). Expone también `AllLevels` e `IsUnlocked`.
- **Escena `LevelSelect`** (nueva, alta en Build Settings, mapeada en `GameFlowRunner.Scenes`):
  los **tres niveles siempre visibles** —«La Oscuridad», «La Rueda», «El Río»— y un botón «Volver».
- **`LevelSelectController`** — adaptador delgado. Lee el perfil activo de `GameFlowRunner.Flow`;
  por cada nivel, `LevelUnlockPolicy.IsUnlocked` decide si el botón responde. `Refresh()` repinta
  el estado —se llama al mostrar el menú y lo llamará T15/T18 al volver de completar un nivel—.
  Un nivel desbloqueado lleva a `StartNarrative("N{n}_Apertura")` (la escena `Narrative` es de T10).
  *(Vencido el 25/09/2026: desde `faaaa13` (10/09) cada ficha declara su secuencia en
  `LevelEntry.openingSequenceId`, porque la fórmula solo acertaba en el Nivel 1. Ver Anexo, A.6.)*
- **Doble indicador del bloqueo (RNF-19).** El nivel bloqueado no se distingue solo por color:
  lleva un **candado** (`Assets/Game/Art/UI/ui_lock.png`, sprite generado a mano — no había asset)
  **y** la palabra «Bloqueado», además de atenuarse. El botón no responde al clic (`interactable`
  = false). Verificado en captura (§3.4). *(Vencido el 25/09/2026 en cuanto a la ruta: `2a588f3`
  (07/09) movió el sprite, con su `.meta`, a `Assets/Game/Art/UI/Common/ui_lock.png`. Ver Anexo,
  A.6.)*
- **`GameFlowRunner.Apply` tolera no tener `SceneLoader`.** Una prueba que solo ejercita el flujo
  —sin la escena `Boot` que crea los objetos persistentes— hace que la transición actualice la
  FSM sin tocar escenas, en vez de reventar con un `NullReferenceException`.

### 2.4 Pantalla de créditos (T08)

- **Escena `Credits`** (nueva, alta en Build Settings, mapeada en `GameFlowRunner.Scenes`):
  encabezado, cuerpo de texto y un botón «Volver» que regresa a `MainMenu`.
- **`CreditsContent`** — ScriptableObject con el texto completo (RNF-18, editable sin recompilar).
  Incluye la **atribución obligatoria** a la Familia Anonaky: los personajes del Slice 1 son obra
  derivada de sus diseños, con autorización escrita concedida (PG-07 cerrado el 30/08/2026 —
  aunque el guion §12 aún lo declare abierto, INC-43). También nombra a los autores del proyecto
  y a la universidad.
- **`CreditsController`** — adaptador delgado: vuelca `CreditsContent.Body` al `Text` y cablea
  «Volver» → `GoTo(MainMenu)`.

*(Vencido el 25/09/2026: desde `145a63e` (08/09) los autores, la universidad y los demás papeles son
una rejilla de pares papel/persona —`CreditsContent.Entries`, que `CreditsController` pinta
clonando un prototipo—, y `Body` quedó para la atribución a Anonaky y la nota de lo original del
proyecto. El contenido va dentro de un `ScrollRect` vertical. Ver Anexo, A.3.)*

---

## 3. Resultados de las pruebas

**Cómo se corrieron.** Con el runner de Rider (`mcp__rider__run_unity_tests`) contra el Editor
abierto — Rider 2026.2 + plugin «MCP Server Extension for Unity» quedaron operativos el 06/09/2026
tras poner Rider como editor externo de Unity y reiniciar. `unity test` (CLI, Editor cerrado) sigue
como respaldo y para CI. Los números salen del runner, no de una estimación.

### 3.1 EditMode — 34 / 34

| Suite | Pruebas | Nuevas en Fase 1 |
|---|---|---|
| `Game.Architecture.Tests` | 4 | — |
| `Game.Core.Tests.GameFlowTests` | 7 | — |
| `Game.Core.Tests.PlayerProfileTests` | 7 | — |
| `Game.Core.Tests.SaveStoreTests` | 5 | — |
| `Game.Core.Tests.DiskFileSystemTests` | 2 | **2** (T05) |
| `Game.Core.Tests.ProfileSessionTests` | 5 | **5** (2 T05 + 3 T06) |
| `Game.Core.Tests.LevelUnlockPolicyTests` | 4 | **4** (T07) |
| **Total** | **34** | **11** |

### 3.2 PlayMode — 23 / 23

| Suite | Pruebas | Nuevas en Fase 1 |
|---|---|---|
| `Game.Core.Tests.BootFlowTests` | 3 | — |
| `Game.Core.Tests.SceneLoaderTests` | 1 | — (T04b) |
| `Game.Core.Tests.GameFlowRunnerTests` | 2 | **2** (T06) |
| `Game.UI.Tests.MainMenuTests` | 5 | **5** (T05) |
| `Game.UI.Tests.ProfileSelectTests` | 5 | **5** (T06) |
| `Game.UI.Tests.LevelSelectTests` | 5 | **5** (T07) |
| `Game.UI.Tests.CreditsTests` | 2 | **2** (T08) |
| **Total** | **23** | **19** |

**Total Fase 1 (EditMode + PlayMode): 57 / 57.**

*(Nota del 25/09/2026: cifras del 07/09. Cuántas pruebas tienen hoy estas suites y de dónde salió cada una nueva está
en el Anexo, A.7.)*

Detalle de las suites nuevas:

| Prueba | Requisito |
|---|---|
| `DiskFileSystem_RNF07_EscribeYReleeElContenidoEnLaRutaIndicada` | RNF-07 |
| `DiskFileSystem_RF02_ListaLosArchivosDeLaCarpetaConLaExtensionYRutaEnBarras` | RF-02 |
| `ProfileSession_RF09_SinPerfilActivoNoEscribeNingunPerfil` | RF-09 |
| `ProfileSession_RF09_ConPerfilActivoGuardaEsePerfil` | RF-09 |
| `ProfileSession_RF02_ListaLosNombresDeLosPerfilesGuardados` | RF-02 |
| `ProfileSession_RF02_CreateRechazaUnNombreYaGuardado` | RF-02, HU-01 FA-02 |
| `ProfileSession_RF03_LoadDevuelveElPerfilConSuProgreso` | RF-03 |
| `LevelUnlockPolicy_RF03_CompletarUnNivelSoloHabilitaElSiguiente` | RF-03 |
| `LevelUnlockPolicy_RF03_CompletarElUltimoNivelNoHabilitaNingunoMas` | RF-03 |
| `LevelUnlockPolicy_CP02_NuncaRebloqueaUnNivelYaDesbloqueado` | CP-02, RF-41 |
| `LevelUnlockPolicy_RF03_UnPerfilNuevoSoloTieneElNivel1Desbloqueado` | RF-03, HU-01 FA-03 |
| `GameFlowRunner_RF02_SelectProfileFijaElPerfilYEntraALevelSelect` | RF-02 |
| `GameFlowRunner_RNF16_VolverAlMenuDesdeElPanelDePerfilNoRecargaLaEscena` | RNF-16 |
| `MainMenu_RF01_MuestraJugarCreditosYSalirAlcanzablesPorRaycast` | RF-01 |
| `MainMenu_RF09_GuardaElPerfilActivoAntesDeCerrar` | RF-09 |
| `MainMenu_RNF18_ElTituloSeLeeDelScriptableObject` | RNF-18 |
| `MainMenu_RF01_LosBotonesCabenEnPantallaYNoSeSolapan` | RF-01 (aserción de layout) |
| `MainMenu_RNF20_ContrasteTextoFondoSuficiente` | RNF-20 (verificación visual) |
| `ProfileSelect_HU01_PerfilExistenteRestauraElProgresoExacto` | HU-01, RF-03 |
| `ProfileSelect_HU01_NombreVacioMuestraAvisoYNoAvanza` | HU-01 FA-01 |
| `ProfileSelect_HU01_NombreDuplicadoMuestraAvisoYNoAvanza` | HU-01 FA-02 |
| `ProfileSelect_HU01_PerfilNuevoAlcanzaSoloElNivel1YArrancaLaNarrativa` | HU-01 FA-03 |
| `ProfileSelect_RNF09_ElFormularioPideUnSoloDato` | RNF-09 |
| `LevelSelect_RF03_ElNivelAlcanzadoSeMuestraDesbloqueadoYSinCandado` | RF-03 |
| `LevelSelect_RF03_LosNivelesNoAlcanzadosSeMuestranBloqueadosConCandado` | RF-03, RNF-19 |
| `LevelSelect_RF03_PulsarUnNivelBloqueadoNoCambiaElFlujo` | RF-03, CP-02 |
| `LevelSelect_RF03_CompletarElNivel1HabilitaElNivel2` | RF-03 |
| `LevelSelect_RNF19_ElEstadoBloqueadoLlevaCandadoYTextoAdemasDelColor` | RNF-19 (verificación visual) |
| `Credits_RF08_MuestraElReconocimientoDeLaAutoria` | RF-08, CT-09, RNF-23 |
| `Credits_RF08_VolverRegresaAlMenuPrincipal` | RF-08 |

### 3.3 Defectos encontrados y corregidos

- **T05 — `GameFlowRunner` construía la persistencia en `Awake`.** Cada prueba de PlayMode que
  instanciaba un `GameFlowRunner` acababa escribiendo la sonda de `DiskFileSystem` en la raíz del
  proyecto. Se pasó a construcción diferida (`Session` se crea al primer uso).
- **T06 — `GameFlowRunner.Start()` navega solo a `MainMenu`.** Ocurre un frame después de
  `AddComponent`, y en las pruebas del panel de perfil clobbeaba el estado que la prueba acababa
  de fijar. Corregido en las pruebas: se espera ese frame antes de llevar el flujo al panel.
- **T06 — el test RNF-04 de `SceneLoader` (T04b) era frágil al orden.** Esperaba a que
  «MainMenu» fuera la escena activa; si otra prueba la dejaba cargada, esa condición se cumplía
  durante toda la recarga y la aserción corría con el cronómetro aún en cero. Ahora espera a que
  el dato del cronómetro quede anotado. Solo cambió código de prueba.
- **T07 — el Editor de Unity se colgó** (main thread bloqueado 20+ min en «compiling», sin error
  de compilación real). Sospechosos: los hooks de arranque de `com.unity.ai.assistant` (el MCP
  nativo, deprecado) y de Coplay. Se resolvió cerrando y relanzando el Editor. El agente
  `edit-scene` no pudo trabajar; las escenas de T07 y T08 se construyeron con un editor-script
  propio vía `execute_script`, más determinista.
- **T07 — la primera versión de `LevelSelect` tenía los tres botones pegados** (sin separación) y
  el nombre del nivel se transparentaba bajo la insignia «Bloqueado». Ambos se corrigieron tras
  ver la captura: más separación entre botones, y la atenuación pasó de tapar a solo oscurecer,
  con el candado y «Bloqueado» a los lados del nombre.
- **T08 — el campo `content` de `CreditsController` no se guardaba** al asignarlo por
  `SerializedObject` + `SaveScene` (los otros dos campos, referencias internas de escena, sí). Se
  arregló forzando `EditorUtility.SetDirty` + `MarkSceneDirty` antes de guardar.

### 3.4 Verificación visual

**RNF-20 — `MainMenu_RNF20_ContrasteTextoFondoSuficiente`.** Captura analizada a mano:

| Elemento | Texto / fondo | Contraste aprox. | ≥ 4,5:1 |
|---|---|---|---|
| Título «Algoritm» | `#3A1E18` sobre `#F7EFE2` | ~13,5:1 | ✅ |
| Botón «Jugar» | `#3A1E18` sobre `#E8A33D` | ~7:1 | ✅ |
| Botones «Créditos» / «Salir» | `#3A1E18` sobre `#E0D4C0` | ~10:1 | ✅ |

*(Nota del 25/09/2026: la medición es del 07/09, sobre la pantalla de entonces. Desde `2893cdb` (09/09) el título dice
«Algoritmia» y la pantalla es la del mockup; este documento no registra una medición de contraste
posterior.)*

**RNF-19 — `LevelSelect_RNF19_ElEstadoBloqueadoLlevaCandadoYTextoAdemasDelColor`.** Captura
analizada: el nivel bloqueado se distingue por **tres señales** —candado, la palabra «Bloqueado»
y la atenuación—, no solo por color. El nombre del nivel sigue legible (atenuado). «El Río»
dibuja la tilde.

Los glifos con tilde se dibujan en las dos escenas; no hay recortes ni deformación. **Tipografía
pendiente:** los textos usan la fuente del sistema; Baloo 2 (títulos) y Nunito (cuerpo) de la
Dirección de Arte §11.2 son una tarea de assets, no de código. *(Vencido el 25/09/2026: Baloo 2 y
Nunito entraron con `145a63e` (08/09) en `Assets/Game/Art/Fonts/` y hoy las usan todos los textos de
`LevelSelect`, `Credits`, `Narrative` y `LevelSummary`, y 23 de los 24 de `MainMenu`. Ver Anexo,
A.1.)* La escena `Credits` **no** tiene
verificación visual automatizada (T08 es `XS`); el texto se comprobó por su contenido en
`Credits_RF08_MuestraElReconocimientoDeLaAutoria`.

### 3.5 Compilación

Sin errores ni warnings nuevos de compilador en ninguna corrida. Los únicos warnings del proyecto
son los preexistentes de los `.asmdef` de módulos aún sin código (`Game.Scaffolding`,
`Game.Levels.Fire`, `Game.Audio`, y sus pares de pruebas).

---

## 4. Checkpoint B — estado

Las cuatro tareas de código (T05–T08) están terminadas y verdes. El **08/09/2026 se hizo el build
portable** y con él las comprobaciones manuales: una pasa, la otra encontró un residuo.

| Criterio | Estado |
|---|---|
| Perfil nuevo → Nivel 1 habilitado, Niveles 2 y 3 bloqueados **con icono además de color** | ✅ probado (`LevelSelect_RF03_*`, RNF-19 §3.4) |
| Cerrar y reabrir conserva el perfil y su progreso (RNF-14, manual) | 🟡 el perfil creado jugando sobrevive en disco; falta verlo listado al reabrir (§4.1) |
| `Datos/` aparece junto al ejecutable (RNF-07) | ✅ nace en la primera ejecución, junto al `.exe` (§4.1) |
| Sin residuos fuera de `Datos/` (RNF-11) | ❌ hallazgo: `Player.log` y Analytics/Insights en `LocalLow` (§4.1) |
| Revisado con el usuario | ⏳ |

### 4.1 El build portable y lo que enseñó

`unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe"` (08/09/2026):
**137 MB, cinco escenas, `Boot` la primera**. La cifra de RNF-06 (< 500 MB) sobra con margen, pero
la medición vinculante sigue siendo la del Checkpoint D, sobre el equipo de referencia.

**Lo que salió bien.** La primera ejecución crea `Build/Algoritmia/Datos/` junto al `.exe`, sin
instalación y sin tocar el registro. Un perfil creado jugando quedó ahí como
`Datos/yo.json` = `{"name":"yo","reachedLevel":1,"phases":[]}` — la lista cerrada de RNF-09/INC-27,
sin un solo campo de más, escrita por el juego real y no por una prueba. RNF-07 se da por bueno.

**Lo que salió mal.** El reproductor escribe además en
`%AppData%\LocalLow\DefaultCompany\My project\`:

- `Player.log`, porque `PlayerSettings.usePlayerLog` está en 1;
- `Unity/<id>/Analytics/{config,values}` e `Insights/ArchivedEvents/Session/session_end_event`,
  que el módulo `com.unity.modules.unityanalytics` sigue escribiendo **aunque
  `UnityConnectSettings` esté en `m_Enabled: 0`** — se rehízo el build con la analítica apagada y
  los archivos volvieron a aparecer.

Ningún dato del jugador vive ahí: el perfil está solo en `Datos/`. Pero RNF-11 no pide «ningún dato
sensible fuera», pide **ausencia**, y RNF-08 prohíbe la telemetría. Son **dos decisiones del
usuario**, no del código: quitar `com.unity.modules.unityanalytics` de `Packages/manifest.json`
(tocar el manifiesto es «preguntar primero») y poner `usePlayerLog` en 0.

**Y no basta con revertirlo.** `ProjectSettings/UnityConnectSettings.asset` apareció modificado con
`m_Enabled: 0 → 1` —la analítica **encendida** sin que nadie lo pidiera—; se revirtió a 0 y **al
volver a abrir el Editor volvió a 1**, comprobado el mismo día. Mientras
`com.unity.modules.unityanalytics` siga en el manifiesto, el interruptor se vuelve a encender solo:
quitar el módulo es el arreglo, revertir el archivo es solo limpiar el síntoma.

**Lo verificable hoy:** la persistencia funciona (RF-04/RF-09 probados con dobles y con disco
temporal real); el perfil nuevo solo alcanza el Nivel 1 (`LevelUnlockPolicy` + `PlayerProfile.Create`);
el flujo **inicio → panel de perfil → selección → menú de niveles** está probado de punta a punta
en PlayMode, con los niveles 2 y 3 bloqueados con candado y texto; y **Créditos** muestra la
atribución obligatoria y vuelve al menú. La cifra de RNF-04/RNF-05 sobre config vinculante sigue
siendo del Checkpoint D.

**Golden Path hasta donde llega la Fase 1:** `Boot → MainMenu → ProfileSelect → LevelSelect` corre
entero. `Jugar` sobre un nivel desbloqueado transiciona a `Narrative`, cuya escena la construye
T10 (por ahora avisa por consola).

---

## 5. Trazabilidad

Requisitos con al menos una prueba que los nombra, añadidos o reforzados en la Fase 1 (CT-10):

RF-01, RF-02, RF-03, RF-08, RF-09, RF-41 · CP-02 · CT-09 · RNF-07, RNF-09, RNF-16, RNF-18,
RNF-19, RNF-20, RNF-23 · HU-01 (FA-01, FA-02, FA-03).

Declarados por el plan de Fase 1 pero **sin prueba en esta fase**: RNF-01 (longitud de oración en
los textos del jugador — se revisa al crear los ScriptableObjects de contenido, T09+); RNF-14
(cierre y reapertura reales — comprobación manual del Checkpoint B).

---

## Anexo — lo que cambió después del cierre (25/09/2026)

Verificado contra el código y los assets de `ccf77e6`. El grueso es la **revisión visual de
T05–T08**, que rehízo las pantallas de esta fase según los mockups
(`claudeDocs/Mockups de interfaz Algoritmia.html`): `145a63e` (08/09, «T05-T08 revisión visual +
RF-47: interfaces según los mockups y borrado de perfil») y `2893cdb` (09/09, «hundido de botón,
botón secundario, sombra plana y placeholders»). Lo demás son retoques de otros carriles sobre estas
mismas pantallas. El inventario de superficies con su estado vive en `claudeDocs/Interfaces.md`.

### A.1 Tipografía: Baloo 2 y Nunito, no la fuente del sistema

`145a63e` añadió `Assets/Game/Art/Fonts/`: `Baloo2-Bold`, `Baloo2-ExtraBold`, `Nunito-Bold`,
`Nunito-Regular` y `Nunito-SemiBold` (`.ttf`), más su licencia `OFL.txt` (SIL OFL 1.1). Son las de
la Dirección de Arte §11.2: Baloo 2 para títulos, Nunito para el cuerpo. Contadas por el GUID de
esas cinco fuentes en el `m_Font` de cada `Text`:

| Pantalla | `Text` con Baloo 2 / Nunito |
|---|---|
| `MainMenu.unity` | 23 de 24 |
| `LevelSelect.unity` | 11 de 11 |
| `Credits.unity` | 7 de 7 |
| `Narrative.unity` | 4 de 4 |
| `LevelSummary.unity` | 7 de 7 |
| `MenuPausa.prefab` | 7 de 7 (desde `d81cfc7`, 17/09) |

El único `Text` de `MainMenu` con la fuente integrada es la etiqueta «Progreso del equipo», que
entró después, con `9c34924` (24/09, Slice 4). `TeacherReport.unity` (Slice 4) usa la fuente
integrada en sus 20 textos. La línea de licencias de `Credits.unity` dice «Tipografías Baloo 2,
Nunito y Fredoka bajo licencia SIL OFL 1.1.»; en el proyecto no hay ningún archivo de Fredoka.

### A.2 Título: «Algoritmia»

`2893cdb` (09/09) cambió `GameTitleConfig.asset:15` de «Algoritm» a **«Algoritmia»**, y con él el
valor por defecto de `GameTitleConfig.cs:13`; `MainMenu.unity` lo pinta. El mecanismo de §2.1 no
cambió: el título sigue leyéndose del asset (`MainMenu_RNF18_ElTituloSeLeeDelScriptableObject`).
Desde entonces el título del producto y el nombre del guía (Algoritm, INC-44) son distintos. PG-01
—el título del producto— sigue abierto en `SPEC.md`.

### A.3 Créditos: rejilla de pares papel/persona

- `145a63e` dio a `CreditsContent` un arreglo `Entries` de pares `Entry { Role, Name }` («Se
  reparten en dos columnas», dice su `Tooltip`). `CreditsController.PaintEntries`
  (`CreditsController.cs:45-62`) apaga `EntryPrototype` y lo clona una vez por par, rellenando sus
  hijos `Rol` y `Nombre`. `Body` quedó para lo que no es un par: «Personajes basados en diseños de
  la Familia Anonaky, usados con autorización escrita.» y «Entornos, objetos e interfaz: originales
  del proyecto.».
- `CreditsContent.asset` tiene **8 pares**: proyecto de grado, asesor de proyecto, institución,
  dirección de arte, producción de arte, programación en Unity, música y sonido, y acompañamiento y
  asesoría en planeación. Los valores actuales son de `eb676ed` (08/09), que sustituyó los «Por
  definir» con que `145a63e` dejó tres papeles.
- La escena suma la línea de licencias de A.1 y un hueco de arte con el rótulo «Algoritm saluda ·
  placeholder» (`2893cdb`).
- **Desplazamiento.** `Credits.unity` tiene un `ScrollRect` vertical (objeto `Scroll`;
  `m_Vertical: 1`, `m_Horizontal: 0`, con barra vertical y `m_ScrollSensitivity: 40`). Entró con
  `eb676ed` (08/09), y `ad99f0c` (09/09, «el contenido se desplaza en vez de desbordarse») ajustó
  sus anclas: según su mensaje, 908 px de contenido en un viewport de 622 px. Choca con la regla de
  `CLAUDE.md` de desplazar las listas que desbordan con botones y no con un `ScrollRect`.
- `Credits_RF08_MuestraElReconocimientoDeLaAutoria` sigue comprobando la atribución sobre `Body`;
  ninguna prueba recorre la rejilla.

### A.4 Borrado de perfil desde el panel del estudiante (RF-47)

- `145a63e` añadió a cada fila de la lista de perfiles (`ProfileEntryPrototype`) un `DeleteButton`
  con el icono `ui_papelera`. Pulsarlo **no borra**: abre `DeletePanel`, que pregunta «¿Borras el
  perfil de {nombre}?» (`ProfileSelectController.cs:133`) y avisa «Su avance se pierde y no se puede
  recuperar.». «Conservar» es el botón prominente y «Borrar» el de menor peso visual, siguiendo la
  maqueta D3 del `plan.md` del Slice 4, según el mensaje del commit.
- Por debajo, en `Game.Core`: `IFileSystem.DeleteFile`, `SaveStore.Delete` sobre las dos rutas
  —`Datos/` y la de respaldo, INC-34—, que devuelve falso si queda algún residuo (RNF-11);
  `GameFlow.ClearActiveProfile`, y `ProfileSession.Delete`, que suelta el perfil activo si es el que
  se borra, porque si no «Salir» lo volvería a escribir (RF-09). Si el borrado no queda completo, el
  panel lo dice: «No se pudo borrar del todo ese perfil. Avisa a tu profe.»
  (`ProfileSelectController.cs:148`).
- No es el diálogo del informe docente. `TeacherReport.unity` tiene el suyo,
  `EraseConfirmationDialog` (`9c34924`, 24/09, Slice 4), con «Cancelar» y «Eliminar datos». Los dos
  piden el mismo `ProfileSession.Delete`.
- Pruebas, todas de `145a63e`: `ProfileSession_RF47_BorraElPerfilYDejaDeListarlo`,
  `ProfileSession_RNF11_BorrarElPerfilActivoImpideQueSalirLoVuelvaAEscribir`,
  `SaveStore_RF47_BorraElPerfilDeLasDosRutas`, `SaveStore_RF47_NoAfectaAOtrosPerfiles`,
  `SaveStore_INC34_BorraDesdeLaRutaDeRespaldoAunqueDatosNoSeaEscribible` y
  `SaveStore_RNF11_UnBorradoParcialNoSeReportaComoExito` (EditMode), más
  `ProfileSelect_RF47_BorrarPideConfirmacionAntesDeEliminarElPerfil` y
  `ProfileSelect_RF47_ConservarDejaElPerfilIntacto` (PlayMode).

### A.5 El hundido del botón: `ButtonPressFeedback` (`2893cdb`, 09/09)

- `Game.UI.ButtonPressFeedback` baja la cara del botón `PressDepth` = 4 px mientras se mantiene el
  clic y la devuelve al soltar o cuando el puntero sale. Según el `<remarks>` de la clase, la sombra
  plana mide 6 px en reposo y 2 al presionar (Dirección de Arte §10.2), así que el borde inferior de
  la sombra no se mueve. No usa `Selectable.transition`, porque ninguno de sus modos desplaza el
  `RectTransform` del hijo.
- Tres cuidados, cada uno con su prueba: no acumula el hundido con el clic sostenido, porque guarda
  el estado en vez de sumar; no hunde un botón no interactivo, como un nivel bloqueado (RF-03),
  porque hundirse promete que el clic hizo algo; y si no se le serializa cara, toma el primer hijo.
- Está en diez escenas: `MainMenu` (10 componentes), `Level2_Maze` (8), `TeacherReport` (5),
  `LevelSelect` (4), `Level1_Cave`, `Level2_Workshop`, `LevelSummary` y `Narrative` (2 cada una), y
  `Credits` y `Level3_River` (1 cada una). También en `MenuPausa.prefab` (1), que lo lleva a las
  cinco escenas jugables.
- `ButtonPressFeedbackTests` (EditMode, assembly `Game.UI.Tests`), 6 pruebas, todas de `2893cdb`:

| Prueba | Requisito |
|---|---|
| `ButtonPressFeedback_RNF02_HundeLaCaraMientrasElClicSeSostiene` | RNF-02 |
| `ButtonPressFeedback_RNF02_DevuelveLaCaraASuSitioAlSoltar` | RNF-02 |
| `ButtonPressFeedback_RNF02_DevuelveLaCaraASuSitioSiElPunteroSaleSinSoltar` | RNF-02 |
| `ButtonPressFeedback_RNF02_NoAcumulaElHundidoSiElClicSeSostiene` | RNF-02 |
| `ButtonPressFeedback_RF03_NoHundeUnBotonNoInteractivo` | RF-03 |
| `ButtonPressFeedback_RNF02_TomaElPrimerHijoComoCaraSiNoSeSerializaNinguna` | RNF-02 |

### A.6 Las demás pantallas, la pausa y los recursos comunes

- **Inicio.** Además del título, el subtítulo «Piensa el orden, enciende el fuego» (`145a63e`) y un
  hueco de arte «Silueta · familia + Algoritm · placeholder» (`2893cdb`). El panel de perfil dice
  «¿Quién juega?», «Sin avatar, sin edad y sin curso.» y «Solo el nombre: nada más se guarda».
  Desde `9c34924` (24/09) hay un cuarto botón, «Progreso del equipo», que lleva a `TeacherReport`
  (RF-46, `MainMenu_RF46_OfreceLaOpcionDeProgresoDelDocente`).
- **Menú de niveles.** Las fichas dicen «Nivel 1 · La Oscuridad», «Nivel 2 · La Rueda» y «Nivel 3 ·
  El Río», bajo «Elige un nivel», cada una con su hueco de arte «Arte · cueva / bosque / río ·
  placeholder» (`2893cdb`). Desde `faaaa13` (10/09, W06) la narrativa de apertura es un dato de la
  ficha, `LevelEntry.openingSequenceId`: el comentario de `LevelSelectController.StartLevel` explica
  que «N{n}_Apertura» solo acertaba en el Nivel 1 y dejaba el 2 inalcanzable. Hoy las fichas abren
  con `N1_Apertura`, `N2_PuenteI` y `N3_PuenteII` (`LevelSelect.unity:2872-2880`); los valores del
  N2 y del N3 son de `8de614f` (15/09) y `e3575bf` (16/09). Lo vigila
  `LevelSelect_RF05_CadaNivelAbreLaSecuenciaDeAperturaDeSuFicha`.
- **Narrativa.** La ilustración pasó a pantalla completa en `145a63e`: lo registra el anexo de
  [`Fase-2-Resultados.md`](Fase-2-Resultados.md).
- **Menú de pausa.** No es de esta fase, pero comparte la revisión: `MenuPausa.prefab` (`8de614f`,
  15/09, Slice 2) está instanciado en las cinco escenas jugables —`Level1_Cave`, `Level2_Forest`,
  `Level2_Maze`, `Level2_Workshop` y `Level3_River`—. Desde `d81cfc7` (17/09) sus 7 textos usan
  Baloo 2 y Nunito, y sus botones llevan los glifos `ui_reanudar`, `ui_reiniciar` y `ui_pausa`. El
  velo que cubre la pantalla (`PanelPausa`) es tinta `#3A1E18` al 72 %, como ya lo era en `8de614f`.
- **Recursos de `Assets/Game/Art/UI/Common/`:**

| Recurso | Entró con | Dónde se usa hoy |
|---|---|---|
| `ui_lock.png` | `ddcbb42` (07/09, T07) en `Art/UI/`; `2a588f3` (07/09) lo movió a `Common/` | `LevelSelect`, `Level1_Cave`, `Level2_Workshop` |
| `ui_panel.png` | `145a63e` | ocho escenas y `MenuPausa.prefab` |
| `ui_boton.png` | `145a63e` | once escenas y `MenuPausa.prefab` |
| `ui_circulo.png` | `145a63e` | siete escenas |
| `ui_alerta.png` | `145a63e` | seis escenas |
| `ui_papelera.png` | `145a63e` | `MainMenu` (borrar perfil), `TeacherReport`, `N2_MazeLayout.asset` (`DeleteIcon` del laberinto, desde `d7ace67`, 23/09) |
| `ui_pausa.png`, `ui_reanudar.png`, `ui_reiniciar.png` | `d81cfc7` | `MenuPausa.prefab` |
| `ui_flecha.png` | `d81cfc7` | `Level2_Maze`, `Level3_River` |
| `ui_pulso_pista.anim` | `fa5d032` (12/09) | `Level1_Cave` |
| `ui_ind_{errores,intentos,pasos,tiempo}_boceto.png` | `9c34924` (24/09) | `TeacherReport` |

- **`Game.UI` abre sus internos a PlayMode** desde T05 (`eab7c57`, 06/09):
  `Assets/Game/Scripts/Runtime/UI/AssemblyInfo.cs` declara
  `[assembly: InternalsVisibleTo("Game.UI.PlayMode.Tests")]`. Es lo que deja a las pruebas de §3.2
  alcanzar miembros `internal` como `CreditsController.BodyLabel`. Este documento no lo nombraba.

### A.7 Las suites de la Fase 1, hoy

Contadas por grep sobre `Assets/Tests/` en `ccf77e6`. Ninguna usa `[TestCase]`, así que pruebas y
casos coinciden.

| Suite | 07/09 | Hoy | Qué se sumó |
|---|---|---|---|
| `DiskFileSystemTests` | 2 | 2 | — |
| `ProfileSessionTests` | 5 | 7 | las dos del borrado (A.4), `145a63e` |
| `LevelUnlockPolicyTests` | 4 | 4 | — |
| `SaveStoreTests` | 5 | 10 | cuatro del borrado (A.4), `145a63e`; `SaveStore_RNF09_ElJsonDeUnPerfilCompletoNoTieneCampoFueraDeLaListaCerrada`, `9c34924` |
| `ButtonPressFeedbackTests` | — | 6 | la clase entera (A.5), `2893cdb` |
| `GameFlowRunnerTests` | 2 | 4 | `GameFlowRunner_RF22_JugarLaFase1DelNivel2CargaLaEscenaDelBosque` (`faaaa13`, 10/09); `GameFlowRunner_RNF14_ConLasDosPrimerasFasesConfirmadasEntrarAlNivel2RetomaEnElLaberinto` (`b1b423a`, 13/09) |
| `MainMenuTests` | 5 | 7 | `MainMenu_RNF13_SusBotonesNoLanzanSiLaEscenaSeAbreSinPasarPorBoot` (`2a588f3`, 07/09, T08b); `MainMenu_RF46_OfreceLaOpcionDeProgresoDelDocente` (`9c34924`) |
| `ProfileSelectTests` | 5 | 8 | `ProfileSelect_RNF13_ElPanelNoLanzaSiSeAbreSinPasarPorBoot` (`2a588f3`, T08b); las dos RF-47 (A.4), `145a63e` |
| `LevelSelectTests` | 5 | 7 | `LevelSelect_RNF13_VolverNoLanzaSiLaEscenaSeAbreSinPasarPorBoot` (`2a588f3`, T08b); `LevelSelect_RF05_CadaNivelAbreLaSecuenciaDeAperturaDeSuFicha` (`faaaa13`) |
| `CreditsTests` | 2 | 3 | `Credits_RNF13_VolverNoLanzaSiLaEscenaSeAbreSinPasarPorBoot` (`2a588f3`, T08b) |

Las cuatro `_RNF13_` son las de T08b, que `Fase-2-Resultados.md` ya cuenta en sus 27 pruebas
PlayMode «al cerrar la Fase 1». La cifra de la suite completa no se repite aquí: está en
[`Slice-4-Resultados.md`](../Slice%204/Slice-4-Resultados.md), «Corrida completa de la suite
(25/09/2026)».

### A.8 Trazabilidad añadida después del corte

Con las pruebas de A.4, A.5 y A.7, las suites de esta fase nombran además RF-05, RF-22, RF-46,
RF-47, RNF-02, RNF-11, RNF-13, RNF-14 e INC-34 (CT-10).
