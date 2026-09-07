# Fase 1 — Navegación mínima: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **T05–T08 implementadas** — corte de este documento: 7 de septiembre de 2026
Plan técnico: [`plan.md`](plan.md) · Tablero: [`todo.md`](todo.md) · Contrato: `claudeDocs/SPEC.md`
Fase anterior: [`Fase-0-Resultados.md`](Fase-0-Resultados.md)

Este documento registra qué se implementó en la Fase 1, con qué pruebas se verificó y qué
resultado dieron. **Las cuatro tareas (T05–T08) están terminadas y verdes; el Checkpoint B sigue
abierto** —faltan las comprobaciones manuales (RNF-07/RNF-14 sobre el ejecutable) y la revisión
con el usuario—. No reabre decisiones de `SPEC.md`: las cita.

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
  Salir** en columna centrada.
- **`GameTitleConfig`** — ScriptableObject con el título del producto. Vive en un asset porque
  PG-01 sigue abierto: cambiar el título es editar `Assets/Game/Data/GameTitleConfig.asset`, no
  recompilar (RNF-18). Valor provisional confirmado por el usuario el 06/09/2026: **«Algoritm»**.
  Coincide con el nombre del guía (Dir. Arte §7.6, INC-44) — es deliberado.
- **`MainMenuController`** — adaptador delgado. Pone el título desde el SO y cablea los tres
  botones en `Start()`. «Jugar» intercambia `MainPanel` por `ProfilePanel` (ver T06). «Créditos»
  transiciona a `Credits` (escena de T08). «Salir» guarda el perfil activo **antes** de cerrar
  (RF-09), en ese orden: al revés se perdería lo que el estudiante acababa de lograr.
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
- **Doble indicador del bloqueo (RNF-19).** El nivel bloqueado no se distingue solo por color:
  lleva un **candado** (`Assets/Game/Art/UI/ui_lock.png`, sprite generado a mano — no había asset)
  **y** la palabra «Bloqueado», además de atenuarse. El botón no responde al clic (`interactable`
  = false). Verificado en captura (§3.4).
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

**RNF-19 — `LevelSelect_RNF19_ElEstadoBloqueadoLlevaCandadoYTextoAdemasDelColor`.** Captura
analizada: el nivel bloqueado se distingue por **tres señales** —candado, la palabra «Bloqueado»
y la atenuación—, no solo por color. El nombre del nivel sigue legible (atenuado). «El Río»
dibuja la tilde.

Los glifos con tilde se dibujan en las dos escenas; no hay recortes ni deformación. **Tipografía
pendiente:** los textos usan la fuente del sistema; Baloo 2 (títulos) y Nunito (cuerpo) de la
Dirección de Arte §11.2 son una tarea de assets, no de código. La escena `Credits` **no** tiene
verificación visual automatizada (T08 es `XS`); el texto se comprobó por su contenido en
`Credits_RF08_MuestraElReconocimientoDeLaAutoria`.

### 3.5 Compilación

Sin errores ni warnings nuevos de compilador en ninguna corrida. Los únicos warnings del proyecto
son los preexistentes de los `.asmdef` de módulos aún sin código (`Game.Scaffolding`,
`Game.Levels.Fire`, `Game.Audio`, y sus pares de pruebas).

---

## 4. Checkpoint B — estado

Las cuatro tareas de código (T05–T08) están terminadas y verdes. Quedan las **comprobaciones
manuales sobre el ejecutable** y la **revisión con el usuario**.

| Criterio | Estado |
|---|---|
| Perfil nuevo → Nivel 1 habilitado, Niveles 2 y 3 bloqueados **con icono además de color** | ✅ probado (`LevelSelect_RF03_*`, RNF-19 §3.4) |
| Cerrar y reabrir conserva el perfil y su progreso (RNF-14, manual) | ⏳ manual, sobre el ejecutable |
| `Datos/` aparece junto al ejecutable, sin residuos fuera de ella (RNF-07) | ⏳ manual, sobre el ejecutable |
| Revisado con el usuario | ⏳ |

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
