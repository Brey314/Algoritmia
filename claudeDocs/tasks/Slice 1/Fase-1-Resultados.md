# Fase 1 — Navegación mínima: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Estado: **en curso** — corte de este documento: 6 de septiembre de 2026
Plan técnico: [`plan.md`](plan.md) · Tablero: [`todo.md`](todo.md) · Contrato: `claudeDocs/SPEC.md`
Fase anterior: [`Fase-0-Resultados.md`](Fase-0-Resultados.md)

Este documento registra qué se lleva implementado de la Fase 1, con qué pruebas se verificó y qué
resultado dieron. **La fase no está cerrada:** T05 y T06 están terminadas, T07 va a medias y T08 no
ha empezado. No reabre decisiones de `SPEC.md`: las cita.

---

## 1. Alcance de la fase

La Fase 1 es el módulo `sistema-navegacion`: de la pantalla de inicio al menú de niveles, con un
perfil real que sobrevive al cierre de la aplicación. No hay nivel jugable todavía.

| Tarea | Qué entrega | Modo de prueba | Estado |
|---|---|---|---|
| T05 | Pantalla de inicio: título, Jugar / Créditos / Salir; guardado al salir | PlayMode + VV | ✅ terminada |
| T06 | Perfil de un solo nombre: lista de perfiles + crear uno nuevo validado | EditMode + PlayMode | ✅ terminada |
| T07 | Menú de niveles con desbloqueo progresivo | EditMode + PlayMode + VV | 🔨 en curso |
| T08 | Pantalla de créditos mínima | PlayMode | ⏳ sin empezar |

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
  transiciona a `Credits` (sin escena hasta T08 — avisa por consola). «Salir» guarda el perfil
  activo **antes** de cerrar (RF-09), en ese orden: al revés se perdería lo que el estudiante
  acababa de lograr.
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

### 2.3 Menú de niveles (T07 — parcial)

- **`LevelUnlockPolicy`** — C# plano. `UnlockAfterCompleting(profile, completed)` habilita el
  nivel siguiente y **nada más** (RF-03); completar el último no habilita ninguno. Nunca
  retrocede —lo garantiza `PlayerProfile.Reach`—, así que un fallo posterior no re-bloquea nada
  (CP-02, RF-41). Expone también `AllLevels` e `IsUnlocked`.
- **`GameFlowRunner.Apply` tolera no tener `SceneLoader`.** Una prueba que solo ejercita el flujo
  —sin la escena `Boot` que crea los objetos persistentes— hace que la transición actualice la
  FSM sin tocar escenas, en vez de reventar con un `NullReferenceException`.

**Falta de T07:** la escena `LevelSelect` y su alta en Build Settings, mapearla en
`GameFlowRunner.Scenes`, el `LevelSelectController` (el bloqueo se señala con **candado y texto
además del color** — RNF-19; sin sprite de candado en el proyecto, la segunda señal está por
decidir), y las pruebas PlayMode + verificación visual.

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

### 3.2 PlayMode — 16 / 16

| Suite | Pruebas | Nuevas en Fase 1 |
|---|---|---|
| `Game.Core.Tests.BootFlowTests` | 3 | — |
| `Game.Core.Tests.SceneLoaderTests` | 1 | — (T04b) |
| `Game.Core.Tests.GameFlowRunnerTests` | 2 | **2** (T06) |
| `Game.UI.Tests.MainMenuTests` | 5 | **5** (T05) |
| `Game.UI.Tests.ProfileSelectTests` | 5 | **5** (T06) |
| **Total** | **16** | **12** |

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

### 3.4 Verificación visual — RNF-20

`MainMenu_RNF20_ContrasteTextoFondoSuficiente` guarda una captura; se analizó a mano:

| Elemento | Texto / fondo | Contraste aprox. | ≥ 4,5:1 |
|---|---|---|---|
| Título «Algoritm» | `#3A1E18` sobre `#F7EFE2` | ~13,5:1 | ✅ |
| Botón «Jugar» | `#3A1E18` sobre `#E8A33D` | ~7:1 | ✅ |
| Botones «Créditos» / «Salir» | `#3A1E18` sobre `#E0D4C0` | ~10:1 | ✅ |

Los glifos con tilde («Créditos») se dibujan; no hay recortes ni deformación. **Tipografía
pendiente:** los textos usan la fuente del sistema; Baloo 2 (títulos) y Nunito (cuerpo) de la
Dirección de Arte §11.2 son una tarea de assets, no de código.

### 3.5 Compilación

Sin errores ni warnings nuevos de compilador en ninguna corrida. Los únicos warnings del proyecto
son los preexistentes de los `.asmdef` de módulos aún sin código (`Game.Scaffolding`,
`Game.Levels.Fire`, `Game.Audio`, y sus pares de pruebas).

---

## 4. Checkpoint B — estado

Ningún criterio del Checkpoint B está cerrado todavía: falta T07 (menú de niveles con el doble
indicador) y T08, más las comprobaciones manuales.

| Criterio | Estado |
|---|---|
| Perfil nuevo → Nivel 1 habilitado, Niveles 2 y 3 bloqueados **con icono además de color** | ⏳ T07 |
| Cerrar y reabrir conserva el perfil y su progreso (RNF-14, manual) | ⏳ manual, tras T07 |
| `Datos/` aparece junto al ejecutable, sin residuos fuera de ella (RNF-07) | ⏳ manual, sobre build |
| Revisado con el usuario | ⏳ |

**Lo verificable hoy:** la persistencia funciona (RF-04/RF-09 probados con dobles y con disco
temporal real), el perfil nuevo solo alcanza el Nivel 1 (`LevelUnlockPolicy` +
`PlayerProfile.Create`), y el flujo inicio → panel de perfil → selección está probado de punta a
punta en PlayMode. La cifra de RNF-04/RNF-05 sobre config vinculante sigue siendo del Checkpoint D.

---

## 5. Trazabilidad

Requisitos con al menos una prueba que los nombra, añadidos o reforzados en lo que va de Fase 1
(CT-10):

RF-01, RF-02, RF-03, RF-09, RF-41 · CP-02 · RNF-07, RNF-09, RNF-16, RNF-18, RNF-20 · HU-01 (FA-01,
FA-02, FA-03).

Declarados por el plan de Fase 1 pero **sin prueba todavía**: RNF-19 (doble indicador del bloqueo
— es de T07), RF-08 y RNF-23 (créditos — T08), RNF-01 (longitud de oración en los textos del
jugador — se revisa al crear los ScriptableObjects de contenido, T09+).
