# Plan técnico — Slice 1: Golden Path temprano

Contrato de referencia: `claudeDocs/SPEC.md`. Este plan no rediscute arquitectura ni alcance:
los aplica. Cuando algo aquí contradiga a `SPEC.md`, gana `SPEC.md`.

**Rev. 1 — 30/08/2026.**

---

## Alcance

De la pantalla de inicio al Nivel 1 terminado y su escena de cierre, con un perfil real que
sobrevive al cierre de la aplicación:

| Módulo | Qué entra en este slice | Qué NO entra |
|---|---|---|
| `sistema-navegacion` (A) | Inicio, perfil de un solo nombre, menú de tres niveles con dos bloqueados, guardado al completar fase, pausa, créditos mínimos, salida | Informe docente, eliminación de datos (Slice 4) |
| `andamiaje` (B) | Diálogo secuencial del guía, omisión de escena ya vista, ayuda a demanda, pista tras tres fallos, cierre reflexivo | Ayuda contextual de los niveles 2 y 3 |
| `nivel-fuego` (C) | Completo: panel de encendido, iteración, depuración, convergencia, resolución, indicadores | — |
| `progreso-registro` (F) | Solo la **emisión** de los cuatro indicadores vía `ILevelReporter` | Agregación, presentación docente, borrado |

**Fuera de alcance explícito:** `nivel-rueda`, `nivel-rio`, `TeacherReport`, RF-46, RF-47.

---

## Decisiones ya tomadas que este plan aplica

Ninguna se rediscute; se listan para que las tareas no las reinventen.

- FSM `GameFlow` en **C# plano**, sin `MonoBehaviour`; `GameFlowRunner` es el adaptador delgado.
- Estados parametrizados: `Narrative` recibe un `NarrativeSequence`; `Playing` recibe `LevelId` + fase.
- Assemblies del slice: `Game.Core`, `Game.Scaffolding`, `Game.Levels.Fire`, `Game.UI`, `Game.Audio`.
  `Game.UI` y `Game.Audio` dependen de `Game.Core`, **nunca al revés** (INC-40).
- Tres singletons con `DontDestroyOnLoad` y ninguno más: `GameFlowRunner`, `SceneLoader`, `AudioManager`.
- Persistencia: JSON por perfil en `Datos/` junto al ejecutable; si no es escribible, cae a
  `Application.persistentDataPath` y se advierte (supuesto 1, INC-34).
- Todo texto visible y todo parámetro ajustable jugando vive en ScriptableObject (CT-05, RNF-18).
- Entrada limitada a clic y clic sostenido (CT-06, RNF-02). El Nivel 1 solo usa clic.
- **Nada de `GameOver`, puntajes, cifras al estudiante ni pérdida de progreso confirmado.**

---

## Grafo de dependencias

```
T01 asmdefs + estructura
 │
 ├─→ T02 PlayerProfile + SaveStore ──┐
 │                                    │
 └─→ T03 GameFlow (FSM plano) ────────┤
                                      │
                    T04 SceneLoader + GameFlowRunner + Boot
                                      │
        ┌─────────────────────────────┼──────────────────────────┐
        │                             │                          │
   T05 Inicio                   T06 Perfil                  T07 Menú niveles
   (RF-01, RF-09)               (RF-02)                     (RF-03)
        │                             │                          │
   T08 Créditos                       └──────────┬───────────────┘
   (RF-08)                                       │
                                      T09 DialogueRunner (puro)
                                                 │
                                      T10 Escena Narrative
                                                 │
                                      T11 HintPolicy + ayuda
                                                 │
                              ┌──────────────────┴─────────────────┐
                              │                                    │
                   T12 FireAttempt (puro)              T13 FireFeedbackLog (puro)
                              └──────────────────┬─────────────────┘
                                                 │
                                      T14 FirePanel + Level1_Cave
                                                 │
                                      T15 Soplar → resolución
                                                 │
                    ┌────────────────────────────┼────────────────┐
                    │                            │                │
              T16 Pausa                   T17 Indicadores    T19 Iluminación
              (RF-07)                     (RF-45 emisión)    (RF-21, Baja)
                    └────────────────────────────┤
                                                 │
                                      T18 LevelSummary + desbloqueo
```

El orden es de abajo hacia arriba en el grafo: primero la lógica pura probable sin escena,
después el cableado. **Cada tarea deja el proyecto compilando y jugable hasta donde llegó.**

---

## Convenciones de las tareas

- **Modo de prueba:** `EditMode` = lógica pura, sin escena ni frames. `PlayMode` = cableado,
  UI, integración, Golden Path; lleva `[Category("Integration")]`.
- **Trazabilidad (CT-10):** el nombre del método de prueba cita el identificador. Ejemplo:
  `FireAttempt_RF19_SoplarNoSeDeshabilitaTrasFalloPosterior`.
- **Tamaño:** XS = 1 archivo · S = 1-2 · M = 3-5. Ninguna tarea de este plan supera M.
- **Flujo test-first por tarea:** `test-designer` → `failing-test-writer` → ver fallar →
  implementar → `resolve-diagnostics` → deduplicar.

---

# Fase 0 — Cimientos

## T01: Estructura de carpetas y assemblies del slice

**Descripción.** Crear `Assets/Game/Scripts/Runtime/{Core,Scaffolding,Levels/Fire,UI,Audio}`,
`Assets/Game/Data/`, `Assets/Tests/{EditMode,PlayMode}/` y los `.asmdef` correspondientes con
sus referencias unidireccionales. Sin lógica todavía.

**Traza:** RNF-15 (convención uniforme), RNF-16 (nivel como assembly independiente),
INC-40 (`Game.UI` y `Game.Audio` existen), `SPEC.md` §Estructura del proyecto.

**Modo de prueba:** EditMode (prueba de arquitectura sobre las referencias de assembly).

**Criterios de aceptación**
- [ ] Existen `Game.Core`, `Game.Scaffolding`, `Game.Levels.Fire`, `Game.UI`, `Game.Audio` y un
      assembly de pruebas por cada uno.
- [ ] `Game.Core` **no** referencia a `Game.UI`, `Game.Audio` ni a ningún assembly de nivel.
- [ ] `Game.Levels.Fire` no referencia a otro assembly de nivel (hoy no hay otro; la prueba fija
      la regla para cuando lo haya — RNF-16).
- [ ] Los namespaces siguen la ruta bajo `Scripts/` elidiendo `Runtime`.

**Verificación**
- [ ] `mcp__coplay-mcp__check_compile_errors` → sin errores.
- [ ] Test EditMode `Architecture_RNF16_CoreNoDependeDeUINiDeNiveles` pasa.
- [ ] Ningún `.meta` escrito a mano (los genera el Editor).

**Depende de:** ninguna · **Tamaño:** S

**Archivos**
- `Assets/Game/Scripts/Runtime/*/Game.*.asmdef` (5)
- `Assets/Tests/EditMode/Core/Game.Core.Tests.asmdef` (+ pares)
- `Assets/Tests/EditMode/Architecture/AssemblyDependencyTests.cs`

---

## T02: `PlayerProfile` y `SaveStore` — JSON en `Datos/`

**Descripción.** Modelo de perfil y almacén JSON, ambos C# plano. Un archivo por perfil en la
carpeta `Datos/` junto al ejecutable, con caída a `Application.persistentDataPath` si la ruta no
es escribible, y advertencia registrada. El perfil guarda **nombre o alias, nivel alcanzado,
fases confirmadas y los cuatro indicadores por fase, y nada más**.

**Traza:** RF-02, RF-04, RNF-07, RNF-09, RNF-11 (base), RNF-14, HU-01, CU-01,
INC-27 (el progreso sí se persiste), INC-34 (dos rutas), supuestos 1, 2 y 9.

**Modo de prueba:** EditMode (con un `IFileSystem` inyectado, sin tocar disco real salvo en un
caso de integración).

**Criterios de aceptación**
- [ ] `PlayerProfile` expone nombre/alias, `LevelId` alcanzado, conjunto de fases confirmadas e
      indicadores por nivel y fase. **No existe campo de puntaje** (CP-03) — dejarlo comentado
      como razón pedagógica, no técnica.
- [ ] Crear un perfil con nombre vacío o duplicado falla con un resultado tipado, no con excepción
      (HU-01 FA-01, FA-02).
- [ ] Un perfil recién creado tiene progreso en cero y solo el Nivel 1 alcanzable.
- [ ] Guardar y releer devuelve un perfil equivalente (ida y vuelta).
- [ ] Si `Datos/` no es escribible, `SaveStore` usa la ruta de respaldo y expone que lo hizo.

**Verificación**
- [ ] EditMode: `SaveStore_RF04_GuardaYRecuperaElPerfilCompleto`,
      `PlayerProfile_RF02_RechazaNombreVacioYDuplicado`,
      `SaveStore_RNF09_NoPersisteCampoAlgunoFueraDeLaListaCerrada`,
      `SaveStore_INC34_CaeALaRutaDeRespaldoSiDatosNoEsEscribible`.
- [ ] Revisión manual del JSON generado: ningún campo fuera de la lista cerrada de OE1 §3.6.1.

**Depende de:** T01 · **Tamaño:** M

**Archivos**
- `.../Core/PlayerProfile.cs`, `.../Core/SaveStore.cs`, `.../Core/IFileSystem.cs`
- `Assets/Tests/EditMode/Core/SaveStoreTests.cs`, `PlayerProfileTests.cs`

---

## T03: `GameFlow` — la FSM en C# plano

**Descripción.** Máquina de estados sin dependencias de Unity con el enum acordado
(`Boot, MainMenu, ProfileSelect, LevelSelect, Narrative, Playing, LevelSummary, Credits,
TeacherReport`) y sus transiciones legales. `TeacherReport` existe en el enum pero en este slice
no tiene destino: la transición hacia él queda declarada y sin implementar.

**Traza:** RF-01, RF-03, RF-05, RF-07, RF-08, RF-09, RF-44 (forma del cierre), CP-02,
`SPEC.md` §Arquitectura.

**Modo de prueba:** EditMode. Sin escena, sin frames — ése es el punto.

**Criterios de aceptación**
- [ ] **No existe `GameOver`** en el enum ni transición alguna hacia una pantalla de derrota
      (CP-02). Comentario «por qué no» en el enum.
- [ ] `Narrative` se parametriza con un `NarrativeSequence`; `Playing`, con `LevelId` + fase.
- [ ] Una transición ilegal no cambia de estado y es observable (no lanza y no deja el flujo roto).
- [ ] El recorrido `Boot → MainMenu → ProfileSelect → LevelSelect → Narrative → Playing →
      LevelSummary → LevelSelect` se recorre entero en una prueba.

**Verificación**
- [ ] EditMode: `GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable`,
      `GameFlow_CP02_NoExisteEstadoDeDerrota`,
      `GameFlow_RF03_NoPermiteEntrarANivelBloqueado`.

**Depende de:** T01, T02 · **Tamaño:** M

**Archivos**
- `.../Core/GameState.cs`, `.../Core/GameFlow.cs`, `.../Core/LevelId.cs`
- `Assets/Tests/EditMode/Core/GameFlowTests.cs`

---

## T04: `SceneLoader`, `GameFlowRunner` y escena `Boot`

**Descripción.** Los dos adaptadores `MonoBehaviour` con `DontDestroyOnLoad` y la escena `Boot`
que los instancia. `GameFlowRunner` traduce cada transición de `GameFlow` a una carga de escena;
no contiene reglas.

**Traza:** RNF-04 (< 10 s por escena), RNF-16, `SPEC.md` §Arquitectura.

**Modo de prueba:** PlayMode, `[Category("Integration")]`.

**Criterios de aceptación**
- [ ] `Boot` arranca y transiciona sola a `MainMenu`.
- [ ] `GameFlowRunner` y `SceneLoader` sobreviven a un cambio de escena y **no se duplican** al
      volver a una escena ya visitada.
- [ ] `GameFlowRunner` no contiene ninguna regla de transición: solo traduce.

**Verificación**
- [ ] PlayMode: `GameFlowRunner_RNF16_NoSeDuplicaAlRecargarEscena`.
- [ ] Medición manual del tiempo de carga de `Boot` y `MainMenu` (RNF-04) — se anota, no se estima.

**Depende de:** T03 · **Tamaño:** S

**Archivos**
- `.../Core/SceneLoader.cs`, `.../Core/GameFlowRunner.cs`
- `Assets/Game/Scenes/Boot.unity`
- `Assets/Tests/PlayMode/Core/BootFlowTests.cs`

---

### ✅ Checkpoint A — Cimientos

- [ ] Compila sin errores ni warnings nuevos.
- [ ] Todas las pruebas EditMode de Core pasan (**corridas a mano en Test Runner — ver Riesgo R1**).
- [ ] La aplicación arranca en `Boot` y llega a `MainMenu` vacío.
- [ ] Revisión con el usuario antes de seguir.

---

# Fase 1 — Navegación mínima (`sistema-navegacion`)

## T05: Pantalla de inicio

**Descripción.** Escena `MainMenu` con título del juego y las tres opciones **Jugar, Créditos y
Salir**. Salir guarda el estado del perfil activo antes de cerrar. El título vive en un
ScriptableObject porque PG-01 sigue abierto.

**Traza:** RF-01, RF-09, HU-01, CU-01, RNF-01, RNF-20, PG-01.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.

**Criterios de aceptación**
- [ ] Las tres opciones existen, son alcanzables por raycast y caben en pantalla sin desbordar.
- [ ] «Salir» invoca el guardado del perfil activo antes de cerrar (RF-09).
- [ ] El título se lee de un SO; cambiarlo no exige recompilar (RNF-18).
- [ ] Contraste texto/fondo ≥ 4.5:1 (RNF-20), verificado en `[Category("VisualVerification")]`.

**Verificación**
- [ ] PlayMode: `MainMenu_RF01_MuestraJugarCreditosYSalir`,
      `MainMenu_RF09_GuardaElPerfilActivoAntesDeSalir`.
- [ ] Aserción de layout: ningún elemento fuera de pantalla ni solapado.

**Depende de:** T04 · **Tamaño:** S

**Archivos**
- `.../UI/MainMenuController.cs`, `Assets/Game/Data/GameTitleConfig.asset`
- `Assets/Game/Scenes/MainMenu.unity`
- `Assets/Tests/PlayMode/UI/MainMenuTests.cs`

---

## T06: Perfil de un solo nombre

**Descripción.** Escena `ProfileSelect`: lista de perfiles existentes y opción «Nuevo perfil» con
un único campo de nombre o alias. Validación de vacío y duplicado. Al seleccionar un perfil se
carga su progreso; al crear uno nuevo, progreso en cero y solo el Nivel 1 habilitado.

**Traza:** RF-02, RF-03, RNF-09, HU-01 (flujo básico 3-7, FA-01, FA-02, FA-03), CU-01, CU-02.

**Modo de prueba:** EditMode para la validación (`ProfileValidator` puro) + PlayMode para el flujo.

**Criterios de aceptación**
- [ ] Se pide **un solo dato**; no hay ningún otro campo en el formulario (RNF-09).
- [ ] Nombre vacío → mensaje de campo obligatorio, sin avanzar (HU-01 FA-01).
- [ ] Nombre duplicado → notifica duplicidad y pide otro (HU-01 FA-02).
- [ ] Perfil existente seleccionado → el menú refleja **exactamente** el progreso previo.
- [ ] Perfil nuevo → solo Nivel 1 habilitado y arranca la escena narrativa de introducción
      (HU-01 FA-03).

**Verificación**
- [ ] EditMode: `ProfileValidator_RF02_RechazaVacioYDuplicado`.
- [ ] PlayMode: `ProfileSelect_HU01_PerfilExistenteRestauraElProgresoExacto`.

**Depende de:** T05 · **Tamaño:** M

**Archivos**
- `.../Core/ProfileValidator.cs`, `.../UI/ProfileSelectController.cs`
- `Assets/Game/Scenes/ProfileSelect.unity`
- `Assets/Tests/EditMode/Core/ProfileValidatorTests.cs`, `Assets/Tests/PlayMode/UI/ProfileSelectTests.cs`

---

## T07: Menú de niveles con desbloqueo progresivo

**Descripción.** Escena `LevelSelect` con los tres niveles visibles y dos bloqueados. La regla de
desbloqueo es una clase pura `LevelUnlockPolicy`; la escena solo la pinta. El estado bloqueado
lleva **color más un segundo indicador** (candado + texto), nunca solo color.

**Traza:** RF-03, RNF-19, RNF-20, HU-01, CU-02.

**Modo de prueba:** EditMode (`LevelUnlockPolicy`) + PlayMode (pintado y bloqueo real).

**Criterios de aceptación**
- [ ] Los tres niveles se muestran siempre; los no disponibles aparecen bloqueados, no ocultos.
- [ ] Un nivel bloqueado no responde al clic y no cambia de estado del flujo.
- [ ] El bloqueo se señala con icono de candado **y** texto, además del color (RNF-19).
- [ ] Completar el Nivel 1 habilita el Nivel 2 y **nunca** re-bloquea uno ya desbloqueado.

**Verificación**
- [ ] EditMode: `LevelUnlockPolicy_RF03_SoloHabilitaElSiguienteAlCompletarElAnterior`,
      `LevelUnlockPolicy_CP02_NuncaRebloqueaUnNivelDesbloqueado`.
- [ ] VisualVerification: `LevelSelect_RNF19_EstadoBloqueadoTieneIconoAdemasDeColor`.

**Depende de:** T06 · **Tamaño:** M

**Archivos**
- `.../Core/LevelUnlockPolicy.cs`, `.../UI/LevelSelectController.cs`
- `Assets/Game/Scenes/LevelSelect.unity`
- `Assets/Tests/EditMode/Core/LevelUnlockPolicyTests.cs`, `Assets/Tests/PlayMode/UI/LevelSelectTests.cs`

---

## T08: Pantalla de créditos mínima

**Descripción.** Escena `Credits` con el reconocimiento de autoría de personajes y recursos. Entra
en este slice porque RF-01 pone el botón en la pantalla de inicio y un botón que no lleva a
ningún lado es un defecto; además los assets generados en este mismo slice necesitan su
reconocimiento (CT-09, RNF-23). El contenido vive en un SO.

**Traza:** RF-08, CT-09, RNF-23, RNF-18, RNF-20.

**Modo de prueba:** PlayMode `[Category("Integration")]`.

**Criterios de aceptación**
- [ ] Se llega desde `MainMenu` y se vuelve a `MainMenu`.
- [ ] El texto se lee de un ScriptableObject.
- [ ] Incluye la línea de autoría de los personajes originales del Slice 1.

**Verificación**
- [ ] PlayMode: `Credits_RF08_MuestraLaAutoriaYRegresaAlMenu`.

**Depende de:** T05 · **Tamaño:** XS

**Archivos**
- `.../UI/CreditsController.cs`, `Assets/Game/Data/CreditsContent.asset`
- `Assets/Game/Scenes/Credits.unity`

---

### ✅ Checkpoint B — Navegación

- [ ] Perfil nuevo → menú con Nivel 1 habilitado y 2 y 3 bloqueados con icono.
- [ ] Cerrar la aplicación y reabrirla conserva el perfil y su progreso (RNF-14, manual).
- [ ] La carpeta `Datos/` aparece junto al ejecutable y **no hay residuos fuera de ella** (RNF-07).
- [ ] Revisión con el usuario.

---

# Fase 2 — Andamiaje mínimo (`andamiaje`)

## T09: `NarrativeSequence` y `DialogueRunner`

**Descripción.** ScriptableObject de secuencia narrativa (ilustración + líneas con hablante) y el
avanzador de diálogo en C# plano. **No es video**: ilustración fija con cuadros secuenciales.
El botón de omitir aparece **solo si la escena ya fue vista** por ese perfil.

**Traza:** RF-05, RF-06 (prioridad Media), RF-10, RNF-01, RNF-18, HU-02, INC-28,
guion §2 y §4.1–4.2.

**Modo de prueba:** EditMode.

**Criterios de aceptación**
- [ ] `DialogueRunner` avanza una línea por clic y señala cuándo terminó.
- [ ] `CanSkip` es falso la primera vez que el perfil ve la escena y verdadero después (INC-28).
- [ ] El cierre reflexivo **no se puede omitir la primera vez** (CP-07, RF-12).
- [ ] Los textos viven en el SO; ninguna cadena visible está incrustada en una clase.
- [ ] Toda línea cumple RNF-01: máximo 20 palabras por oración.

**Verificación**
- [ ] EditMode: `DialogueRunner_RF05_AvanzaUnaLineaPorClic`,
      `DialogueRunner_RF06_NoOfreceOmitirLaPrimeraVez`,
      `NarrativeSequence_RNF01_NingunaOracionSupera20Palabras`.

**Depende de:** T07 · **Tamaño:** M

**Archivos**
- `.../Scaffolding/NarrativeSequence.cs`, `.../Scaffolding/DialogueLine.cs`, `.../Scaffolding/DialogueRunner.cs`
- `Assets/Tests/EditMode/Scaffolding/DialogueRunnerTests.cs`

---

## T10: Escena `Narrative` parametrizada y contenido del Nivel 1

**Descripción.** Una sola escena reutilizable que recibe un `NarrativeSequence`. Se crean los tres
assets narrativos del Nivel 1: apertura (§3.1), aparición del guía (§4.1) y el hallazgo (§4.2).
Añadir una escena narrativa debe ser crear un asset, no una escena.

**Traza:** RF-05, RF-06, RNF-06 (peso), guion §3.1, §4.1, §4.2.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.

**Criterios de aceptación**
- [ ] La misma escena resuelve las tres secuencias sin ramas en el código.
- [ ] El texto no desborda su cuadro en ninguna de las tres.
- [ ] El botón de pausa **no** se muestra en escenas narrativas (HU-17 FA-04).

**Verificación**
- [ ] PlayMode: `NarrativeScene_RF05_ResuelveTresSecuenciasDistintasSinRamas`.
- [ ] Aserción de layout sobre el cuadro de diálogo más largo.

**Depende de:** T09 · **Tamaño:** M

**Archivos**
- `.../UI/NarrativeSceneController.cs`
- `Assets/Game/Scenes/Narrative.unity`
- `Assets/Game/Data/Narrative/N1_Apertura.asset`, `N1_AparicionGuia.asset`, `N1_Hallazgo.asset`

---

## T11: `HintPolicy` — ayuda a demanda y pista tras tres fallos

**Descripción.** Los **dos mecanismos por separado**, como exige RF-13: un botón de ayuda visible
durante toda la escena jugable que repite la instrucción vigente **sin alterar el estado**, y una
pista automática tras `intentosParaPista` fallos consecutivos. La pista nunca nombra la respuesta.

**Traza:** RF-13, CP-06, HU-03, HU-04, guion §4.3.6, RNF-03.

**Modo de prueba:** EditMode.

**Criterios de aceptación**
- [ ] La ayuda a demanda devuelve la instrucción vigente y **no muta ningún contador** (CP-06).
- [ ] La pista automática se dispara exactamente al tercer fallo consecutivo y reinicia el contador.
- [ ] Un acierto intermedio reinicia el contador de fallos consecutivos.
- [ ] En el Nivel 1 **ninguna pista menciona «Muy cerca»** ni la posición efectiva. Prueba explícita.

**Verificación**
- [ ] EditMode: `HintPolicy_RF13_AyudaADemandaNoAlteraElEstado`,
      `HintPolicy_RF13_PistaSeOfreceAlTercerFalloConsecutivo`,
      `HintPolicy_CP06_LaPistaNuncaNombraLaPosicionEfectiva`.

**Depende de:** T09 · **Tamaño:** M

**Archivos**
- `.../Scaffolding/HintPolicy.cs`, `.../Scaffolding/GuideContent.cs` (SO)
- `Assets/Tests/EditMode/Scaffolding/HintPolicyTests.cs`

---

### ✅ Checkpoint C — Andamiaje

- [ ] Las tres escenas narrativas del Nivel 1 se recorren de principio a fin.
- [ ] El botón de omitir aparece solo en la segunda visita.
- [ ] Ninguna pista del guía resuelve la tarea (revisión de texto contra CP-06).
- [ ] Revisión con el usuario.

---

# Fase 3 — Nivel fuego (`nivel-fuego`)

## T12: `FireLevelConfig`, `StrikePosition` y `FireAttempt`

**Descripción.** El corazón del nivel, C# plano. `FireAttempt` resuelve un golpe según la posición
y lleva los dos contadores: golpes efectivos y fallos consecutivos. La firma ya está ejemplificada
en `SPEC.md` §Estilo de código y se sigue tal cual.

**Traza:** RF-15, RF-16, RF-18, RF-19, CP-02, CT-05, RNF-18, HU-06, HU-07, CU-04,
INC-32, guion §4.3.2, §4.3.3, §4.3.5, §4.3.6, supuesto 7 (PG-06).

**Modo de prueba:** EditMode. Sin escena.

**Criterios de aceptación**
- [ ] Los cuatro parámetros (`posicionesDisponibles`, `posicionEfectiva`, `golpesEfectivosMinimos`,
      `intentosParaPista`) viven en `FireLevelConfig` con `[field: SerializeField]` + `[Tooltip]`.
- [ ] Golpear desde una posición no efectiva **ejecuta el golpe y produce consecuencia visible**;
      no incrementa golpes efectivos e incrementa fallos consecutivos (RF-16, PG-03 cerrado).
- [ ] Golpear desde la posición efectiva incrementa golpes efectivos y **reinicia** los fallos.
- [ ] `CanBlow` se vuelve verdadero al alcanzar el mínimo y **nunca vuelve a falso** (INC-32,
      guion §4.3.6). Comentario «por qué no»: es criterio pedagógico, no técnica.
- [ ] Mover el deslizante **no produce efecto alguno** hasta accionar «Golpear» (RF-15).
- [ ] No existe límite de intentos ni contador de derrota (RF-18, CP-02).

**Verificación**
- [ ] EditMode: `FireAttempt_RF19_SoplarNoSeDeshabilitaTrasFalloPosterior`,
      `FireAttempt_RF16_GolpeNoEfectivoProduceConsecuenciaYNoSuma`,
      `FireAttempt_RF15_CambiarPosicionNoAlteraElEstado`,
      `FireAttempt_RF18_AceptaIntentosIlimitados`,
      `FireLevelConfig_CT05_ExponeLosCuatroParametrosDelGuion`.

**Depende de:** T11 · **Tamaño:** M

**Archivos**
- `.../Levels/Fire/StrikePosition.cs`, `FireLevelConfig.cs`, `FireAttempt.cs`, `StrikeOutcome.cs`
- `Assets/Tests/EditMode/Levels/Fire/FireAttemptTests.cs`

---

## T13: `FireFeedbackLog` — mensajes narrativos sin repetición

**Descripción.** Selección del mensaje del log a partir del resultado del golpe. Los ocho mensajes
del guion §4.3.4 viven en un SO. El sistema **no repite el mismo mensaje dos veces seguidas cuando
existe alternativa aplicable**, y el log acumula el historial para que el estudiante compare.

**Traza:** RF-11, RF-17, RF-18, CP-03, HU-05, HU-06, guion §4.3.4.

**Modo de prueba:** EditMode.

**Criterios de aceptación**
- [ ] Los ocho mensajes del guion están en el SO, literales, sin incrustar en código.
- [ ] Ningún mensaje contiene cifras ni juicio de valor — prueba que barre el SO buscando dígitos.
- [ ] Con alternativa aplicable, dos golpes iguales consecutivos producen mensajes distintos.
- [ ] El log **acumula**: el mensaje anterior sigue visible (HU-06, criterio de aceptación 4).
- [ ] La respuesta se produce en el mismo cuadro de la acción (RF-11, < 1 s).

**Verificación**
- [ ] EditMode: `FireFeedbackLog_RF17_NingunMensajeContieneCifras`,
      `FireFeedbackLog_RF17_NoRepiteElMismoMensajeDosVecesSeguidas`,
      `FireFeedbackLog_HU06_AcumulaElHistorialDeIntentos`.

**Depende de:** T12 · **Tamaño:** M

**Archivos**
- `.../Levels/Fire/FireFeedbackLog.cs`, `.../Levels/Fire/FireMessages.cs` (SO)
- `Assets/Game/Data/Fire/FireMessages.asset`
- `Assets/Tests/EditMode/Levels/Fire/FireFeedbackLogTests.cs`

---

## T14: Panel de encendido y escena `Level1_Cave`

**Descripción.** El `MonoBehaviour` adaptador y la escena: deslizante de tres posiciones, botón
«Golpear», área de registro, botón «Soplar» atenuado, montón de hojas con sus estados visuales.
El adaptador solo traduce clics a llamadas de `FireAttempt` y estado a UI — cero reglas.

**Traza:** RF-14, RF-15, RF-17, RNF-02, RNF-03, RNF-19, RNF-20, CT-06, HU-06, CU-04, guion §4.3.1.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.

**Criterios de aceptación**
- [ ] Los tres elementos de RF-14 están presentes: deslizante, botón de golpear, área de registro.
- [ ] **Solo clic y clic sostenido**; el mapa de controles se inspecciona sin salvedades (RNF-02).
      Ningún binding de teclado.
- [ ] Una sola tarea activa en pantalla (RNF-03). El Nivel 1 **no lleva lista de tareas** (INC-41).
- [ ] «Soplar» atenuado no produce mensaje de error: simplemente no responde (guion §4.3.6).
- [ ] El estado atenuado se distingue por forma/icono además de por color (RNF-19).
- [ ] El adaptador no contiene ninguna condición del juego: se verifica leyendo el diff.

**Verificación**
- [ ] PlayMode: `FirePanel_RF14_PresentaDeslizanteGolpearYRegistro`,
      `FirePanel_RNF02_NingunBindingDeTeclado`,
      `FirePanel_RF19_SoplarAtenuadoNoRespondeNiMuestraError`.
- [ ] Aserción de layout: registro sin desbordar tras diez intentos acumulados.

**Depende de:** T13 · **Tamaño:** M

**Archivos**
- `.../Levels/Fire/FirePanelController.cs`, `.../UI/FeedbackLogView.cs`
- `Assets/Game/Scenes/Level1_Cave.unity`
- `Assets/Tests/PlayMode/Levels/Fire/FirePanelTests.cs`

---

## T15: Convergencia y resolución — «Soplar» y nacimiento del fuego

**Descripción.** Habilitación de «Soplar» al alcanzar el mínimo, animación del nacimiento del fuego
y salto a la escena narrativa de cierre (§4.4). Marca el Nivel 1 como completado, desbloquea el
Nivel 2 y **guarda** (RF-04).

**Traza:** RF-19, RF-20, RF-04, RF-03, CP-02, HU-07, CU-04, guion §4.3.5 (E6, E7), §4.4.

**Modo de prueba:** PlayMode `[Category("Integration")]`.

**Criterios de aceptación**
- [ ] «Soplar» se habilita exactamente al alcanzar `golpesEfectivosMinimos` y ya no se deshabilita.
- [ ] Accionarlo reproduce la animación y encadena a la escena narrativa de cierre.
- [ ] Al completar: perfil guardado, Nivel 1 marcado, Nivel 2 desbloqueado.
- [ ] La animación **no tiene destellos de alta frecuencia** (RNF-21) —
      `[Category("VisualVerification")]`.

**Verificación**
- [ ] PlayMode: `FireLevel_RF20_SoplarEncadenaAnimacionYEscenaDeCierre`,
      `FireLevel_RF04_GuardaAlCompletarLaFase`,
      `FireLevel_RF03_DesbloqueaElNivel2`.
- [ ] VisualVerification: `FireLevel_RNF21_SinDestellosDeAltaFrecuencia`.

**Depende de:** T14 · **Tamaño:** S

**Archivos**
- `.../Levels/Fire/FireResolutionController.cs`
- `Assets/Game/Data/Narrative/N1_NacimientoDelFuego.asset`
- `Assets/Tests/PlayMode/Levels/Fire/FireResolutionTests.cs`

---

## T16: Menú de pausa

**Descripción.** Capa de UI **sobre** `Playing`, no un estado nuevo: Continuar (restituye el estado
exacto), Reiniciar nivel (con confirmación) y Volver al menú principal. Reiniciar **nunca**
re-bloquea un nivel desbloqueado ni borra los indicadores ya registrados.

**Traza:** RF-07, RF-03, RF-04, CP-02, HU-17 (FA-01..FA-05), INC-25, OE1 §3.6.1 nota 4.

**Modo de prueba:** EditMode (`PauseMenuPolicy`) + PlayMode (restitución real).

**Criterios de aceptación**
- [ ] Continuar restituye el estado exacto: posición del deslizante, contadores y log intactos.
- [ ] Reiniciar pide confirmación indicando **en una frase** qué vuelve a empezar (HU-17 FA-01).
- [ ] Cancelar la confirmación no cambia nada (FA-02).
- [ ] Reiniciar conserva niveles desbloqueados, progreso confirmado e indicadores previos.
- [ ] El botón de pausa **no aparece en escenas narrativas** (FA-04).
- [ ] No existe `GameOver` ni penalización por pausar (CP-02).

**Verificación**
- [ ] EditMode: `PauseMenuPolicy_RF07_ReiniciarNoRebloqueaNiBorraIndicadores`.
- [ ] PlayMode: `PauseMenu_HU17_ContinuarRestituyeElEstadoExacto`,
      `PauseMenu_HU17_NoSeMuestraEnEscenasNarrativas`.

**Depende de:** T15 · **Tamaño:** M

**Archivos**
- `.../Core/PauseMenuPolicy.cs`, `.../UI/PauseMenuController.cs`
- `Assets/Tests/EditMode/Core/PauseMenuPolicyTests.cs`, `Assets/Tests/PlayMode/UI/PauseMenuTests.cs`

---

## T17: Emisión de los cuatro indicadores del Nivel 1

**Descripción.** `ILevelReporter` inyectado por Core y su implementación para el Nivel 1, con la
definición operativa **exacta** de OE1 §3.6.1. Se persisten con el guardado de fase (RF-04). No se
muestran al estudiante en ninguna forma.

**Traza:** RF-45 (registro), RF-04, RNF-14, CP-03, CP-09, OE1 §3.6.1, INC-27, INC-29.

**Modo de prueba:** EditMode, con un doble de `ILevelReporter`.

**Criterios de aceptación**
- [ ] **Intentos** = golpes ejecutados desde una posición no efectiva.
- [ ] **Errores corregidos** = cambios de posición tras un golpe no efectivo que desembocan en uno
      efectivo.
- [ ] **Pasos utilizados** = golpes efectivos acumulados hasta habilitar «Soplar».
- [ ] **Tiempo de resolución** excluye escenas narrativas y el tiempo con la pausa abierta.
- [ ] Se emiten **exactamente cuatro** indicadores; ninguno adicional (lista cerrada, RNF-09).
- [ ] Ninguno llega a la UI del estudiante — prueba de exclusión.

**Verificación**
- [ ] EditMode: `FireIndicators_RF45_IntentosCuentaSoloGolpesNoEfectivos`,
      `FireIndicators_RF45_ErrorCorregidoExigeCambioDePosicionSeguidoDeAcierto`,
      `FireIndicators_RF07_LaPausaNoSumaTiempoDeResolucion`,
      `FireIndicators_CP03_NingunIndicadorLlegaALaUIDelEstudiante`.

**Depende de:** T15, T16 · **Tamaño:** M

**Archivos**
- `.../Core/ILevelReporter.cs`, `.../Levels/Fire/FireIndicatorCollector.cs`
- `Assets/Tests/EditMode/Levels/Fire/FireIndicatorTests.cs`

---

## T18: Resumen de fin de nivel y cierre reflexivo

**Descripción.** Estado `LevelSummary`: resumen **narrativo y sin una sola cifra** de lo que hizo
el estudiante, seguido del cierre reflexivo donde Chispa nombra la habilidad ejercitada — «se
llama iterar» (guion §4.4). De ahí vuelve al menú de niveles con el Nivel 2 desbloqueado.

**Traza:** RF-45, RF-12, RF-17, RF-03, CP-03, CP-07, CP-10, HU-14, INC-26.

**Modo de prueba:** EditMode (generación del texto) + PlayMode (flujo).

**Criterios de aceptación**
- [ ] **Cero cifras** en el resumen: prueba que barre el texto renderizado buscando dígitos
      (INC-26 — es el punto donde HU-14 ya coló una).
- [ ] El cierre reflexivo nombra explícitamente la iteración y la relaciona con lo que el jugador
      hizo (RF-12).
- [ ] El cierre reflexivo **no es omitible la primera vez** (CP-07).
- [ ] Al terminar, `LevelSelect` muestra el Nivel 2 habilitado.

**Verificación**
- [ ] EditMode: `LevelSummary_RF45_NoContieneNingunDigito`,
      `LevelSummary_RF12_NombraLaHabilidadEjercitada`.
- [ ] PlayMode: `LevelSummary_RF03_DevuelveAlMenuConNivel2Desbloqueado`.

**Depende de:** T17 · **Tamaño:** M

**Archivos**
- `.../UI/LevelSummaryController.cs`, `.../Scaffolding/LevelSummaryContent.cs` (SO)
- `Assets/Game/Data/Fire/N1_CierreReflexivo.asset`
- `Assets/Tests/EditMode/Scaffolding/LevelSummaryTests.cs`

---

## T19: Iluminación progresiva del escenario

**Descripción.** La cueva pasa de oscuridad total a iluminación completa por escalones, uno por
golpe efectivo. **Prioridad Baja (RF-21)** — es el único RF de este slice que puede quedar fuera
sin comprometer el Golden Path; va último por eso.

**Traza:** RF-21 (Baja), RNF-21, HU-07, guion §4.3.1, §4.3.5 (E4).

**Modo de prueba:** PlayMode `[Category("VisualVerification")]`.

**Criterios de aceptación**
- [ ] La iluminación sube un escalón por golpe efectivo y llega al máximo en la resolución.
- [ ] La transición es **gradual**, sin parpadeo ni destello de alta frecuencia (RNF-21).
- [ ] Es refuerzo visual del avance, no temporizador ni penalización — comentario en el código.
- [ ] El texto del panel mantiene contraste ≥ 4.5:1 **en el estado más oscuro** (RNF-20).

**Verificación**
- [ ] VisualVerification: `FireLevel_RF21_IluminacionSubeUnEscalonPorGolpeEfectivo`,
      `FirePanel_RNF20_ContrasteSuficienteEnElEstadoMasOscuro`.

**Depende de:** T15 · **Tamaño:** S

**Archivos**
- `.../Levels/Fire/CaveLightingController.cs`
- `Assets/Tests/PlayMode/Levels/Fire/CaveLightingTests.cs`

---

### ✅ Checkpoint D — Slice 1 completo

- [ ] **Dos recorridos completos** del Golden Path sin incidencias (RNF-13): inicio → perfil →
      menú → narrativa → panel → resolución → cierre → menú con Nivel 2 desbloqueado.
- [ ] Cierre forzado a mitad del nivel: al reabrir, el perfil retoma desde la última fase
      confirmada (RNF-14).
- [ ] Carga de cada escena < 10 s y memoria < 2 GB, medidas en el equipo de referencia (RNF-04, RNF-05).
- [ ] Ejecución desde carpeta portable con el adaptador de red deshabilitado (RNF-07, RNF-08).
- [ ] Todo RF tocado por el slice tiene al menos una prueba que lo nombra (CT-10) — matriz derivada
      de los nombres de método.
- [ ] Revisión con el usuario antes de abrir el Slice 2.

---

## Riesgos

| # | Riesgo | Impacto | Mitigación |
|---|---|---|---|
| **R1** | **No hay corredor de pruebas MCP.** `run_unity_tests`, `get_unity_compilation_result` y `unity_play_control` no están conectados; solo existe `coplay-mcp`. Todo el flujo test-first de este plan depende de poder ver una prueba fallar. | **Alto — abierto** | Mientras no se instale: correr cada suite **a mano** en la ventana Test Runner y **declarar el resultado explícitamente**, nunca darlo por hecho. Instalar el servidor MCP de Unity es la acción que desbloquea el plan; conviene hacerla antes de T02. |
| R2 | PG-06: los valores del Nivel 1 (`Muy cerca`, 3, 3) no se han validado jugando | Medio | Viven en `FireLevelConfig`; ajustarlos no cuesta recompilación (RNF-18). Validar en el checkpoint D. |
| R3 | PG-01: el título del producto sigue sin definirse y RF-01 lo exige en pantalla | Medio | Título en `GameTitleConfig` (SO), con marcador provisional. Cambiarlo es editar un asset. |
| R4 | PG-07: sin autorización escrita de la Familia Anonaky | Medio | **Personajes originales por defecto** (supuesto 3, CT-09, RNF-23). Los prompts de la sección siguiente generan personajes originales. |
| R5 | `Datos/` no escribible en los equipos de la institución | Medio | Caída a `Application.persistentDataPath` con advertencia; T02 lo prueba en los dos escenarios (INC-34). |
| R6 | Deriva visual entre generaciones de arte | Medio | Bloque de estilo y paleta fijos, copiados literalmente al inicio de cada prompt. |
| R7 | Gemini no genera transparencia real | Bajo | Fondo chroma key `#00FF00` y limpieza posterior; marcado asset por asset abajo. |

## Preguntas abiertas

1. **PG-01 — título provisional.** ¿Qué cadena ponemos en `GameTitleConfig` hasta que se defina?
   Propuesta: «Chispa» como marcador, coherente con el guía.
2. **Créditos en el Slice 1 (T08).** Se incluyen porque RF-01 pone el botón en la pantalla de
   inicio. Si se prefiere diferir, hay que quitar el botón, y eso incumple RF-01. Confirmar.
3. **Instalación del servidor MCP de Unity (R1).** ¿Se hace antes de T02, o el slice avanza con
   pruebas corridas a mano y declaradas?

---

# Assets visuales del Slice 1

Diez assets. Generador principal: **Gemini / Nano Banana Pro**. Los prompts están en español y se
pegan tal cual.

**Regla de uso:** copiar el bloque de estilo y el bloque de paleta **literalmente** al inicio de
cada prompt, antes de la descripción del asset. Ahí está la consistencia entre generaciones: lo
que varía es solo la descripción; lo que se repite palabra por palabra es todo lo demás.

**Autoría (CT-09, RNF-23).** Todos son **personajes y escenarios originales**. No se usa la
Familia Anonaky mientras no llegue la autorización escrita de PG-07. Cada asset generado se
reconoce en la pantalla de créditos (T08).

**Transparencia.** Gemini no produce canal alfa fiable. Los assets marcados **Chroma sí** se piden
sobre fondo verde `#00FF00` plano y se recorta después en el importador de sprites de Unity.

---

## Bloque de estilo fijo — copiar al inicio de cada prompt

```
ESTILO (fijo, no variar): ilustración plana 2D vectorial para videojuego educativo infantil.
Formas redondeadas y macizas, sin puntas agresivas. Contorno limpio y uniforme de 4 px en
color #1C2333. Color en planos sólidos, sin degradados complejos, sin texturas fotográficas,
sin sombreado realista: como máximo una sombra plana de un solo tono. Sin efectos de brillo
volumétrico ni destellos intensos. Iluminación cálida, procedente del fuego, como única fuente
de luz cálida de la escena. Tono amable, acogedor y no amenazante, apropiado para niños de 9 a
11 años. Ambientación prehistórica estilizada, no realista. Sin violencia, sin sangre, sin
armas, sin texto de ningún tipo dentro de la imagen, sin marcas de agua, sin logotipos.
Composición centrada y legible a tamaño pequeño, pensada para proyector y pantallas de baja
calidad: siluetas distinguibles y alto contraste entre figura y fondo.
```

## Bloque de paleta fija — copiar al inicio de cada prompt

```
PALETA (fija, usar solo estos colores):
  Oscuridad de cueva      #0B0E14
  Piedra en sombra        #1C2333
  Piedra iluminada        #2E3A4F
  Roca cálida             #4A3B32
  Tierra                  #6B5344
  Piel cálida clara       #E8B48C
  Piel cálida media       #C98B62
  Piel cálida oscura      #8E5A3B
  Pieles / ropa ocre      #A9713F
  Pieles / ropa terracota #8C4A2F
  Hoja seca               #B08541
  Fuego amarillo          #FFC94A
  Fuego naranja           #FF8A3D
  Fuego rojo              #E4572E
  Hueso (texto y UI)      #F2E8D5
```

*El par `#F2E8D5` sobre `#0B0E14` es el que sostiene RNF-20 (contraste ≥ 4.5:1) en el estado más
oscuro del nivel.*

---

## A1 · Chispa, el guía

**Traza:** guion §1.1 y §4.1, PG-02, RF-10, RF-12, RF-13, CN-03 (presente en los tres niveles).
**Chroma:** sí — aparece flotando sobre escenarios distintos.
**Entregar:** tres variantes en la misma generación — reposo, girando con estela, atenuado.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Chispa, guía luminoso del videojuego.

RASGOS FÍSICOS FIJOS: pequeña criatura luminosa con forma de estrella de cinco puntas
redondeadas, del tamaño de una palma de mano. Cuerpo relleno en #FFC94A con un halo plano
inmediato en #FF8A3D, sin degradado. Dos ojos negros grandes, ovalados, muy separados, con un
punto de luz blanco en la esquina superior de cada uno. Boca pequeña y sonriente, en línea
simple. Sin nariz, sin brazos, sin piernas. Contorno de 4 px en #E4572E, no en azul: es el único
elemento de la paleta que lleva contorno cálido. Estela de cinco a siete puntos de luz sueltos
en #FFC94A de tamaño decreciente detrás del cuerpo.

COMPOSICIÓN: personaje completo, centrado, de frente, flotando.
Generar tres versiones lado a lado del MISMO personaje sin variar sus rasgos:
  (1) en reposo, estela corta, sonrisa suave;
  (2) girando como un trompo, inclinado 25 grados, estela larga en arco;
  (3) atenuado, a punto de apagarse: mismo cuerpo con opacidad reducida y estela de dos puntos.

FONDO: verde chroma key plano #00FF00, sin sombra proyectada sobre el fondo.
```

---

## A2 · Papá — personaje jugable del Nivel 1

**Traza:** guion §1.1 y §4.2, RF-14, HU-06, personajes **originales** (PG-07, CT-09, RNF-23).
**Chroma:** sí.
**Entregar:** cuerpo entero de perfil (vista lateral, que es la del Nivel 1) en tres poses.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Papá, personaje jugable del Nivel 1. Personaje ORIGINAL, no basado en ninguna franquicia
ni personaje existente.

RASGOS FÍSICOS FIJOS: hombre adulto prehistórico, complexión ancha y robusta, estatura media,
hombros marcados. Piel #C98B62. Cabello negro azabache, tupido, largo hasta el hombro, recogido
atrás con una tira de cuero. Barba corta y espesa del mismo negro. Cejas gruesas y rectas. Ojos
negros pequeños y tranquilos, mirada atenta y serena, nunca fiera. Nariz ancha y redondeada.
Túnica de piel sin mangas en #A9713F que cae hasta media pierna, ceñida con una cuerda trenzada
en #6B5344 a la cintura. Brazalete de cuero en el antebrazo derecho, en #8C4A2F. Descalzo.
Sin cicatrices, sin pinturas de guerra, sin armas.

CARÁCTER QUE DEBE LEERSE: prudente y reflexivo, alguien que escucha antes de actuar.

COMPOSICIÓN: cuerpo entero, VISTA LATERAL mirando a la derecha.
Generar tres poses del MISMO personaje sin variar sus rasgos:
  (1) de pie, brazos relajados, en reposo;
  (2) arrodillado sobre una rodilla, inclinado hacia adelante, sosteniendo una piedra en cada
      mano a la altura del pecho, a punto de golpearlas;
  (3) arrodillado, soplando: torso inclinado hacia abajo, labios fruncidos, manos apoyadas en
      el suelo.

FONDO: verde chroma key plano #00FF00.
```

---

## A3 · Mamá

**Traza:** guion §1.1 y §4.2 (acompañante en el Nivel 1; jugable en el Nivel 3).
**Chroma:** sí.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Mamá, personaje acompañante del Nivel 1. Personaje ORIGINAL.

RASGOS FÍSICOS FIJOS: mujer adulta prehistórica, complexión esbelta y erguida, estatura media.
Piel #E8B48C. Cabello castaño muy oscuro, ondulado, largo hasta la cintura, recogido en una
trenza gruesa que cae sobre el hombro izquierdo. Ojos color miel, grandes y almendrados, mirada
observadora y calmada. Cejas finas y arqueadas. Túnica de piel en #8C4A2F que cubre un hombro y
deja el otro descubierto, hasta la rodilla. Collar de cuentas redondas de hueso en #F2E8D5
alrededor del cuello. Descalza.

CARÁCTER QUE DEBE LEERSE: serena y metódica, observa en silencio antes de decidir.

COMPOSICIÓN: cuerpo entero, VISTA LATERAL mirando a la derecha.
Generar tres poses del MISMO personaje sin variar sus rasgos:
  (1) de pie, brazos cruzados con suavidad, observando;
  (2) de pie, una mano apoyada en el hombro de un niño invisible a su lado, gesto protector;
  (3) de pie, ambas manos en el pecho, expresión de alegría emocionada.

FONDO: verde chroma key plano #00FF00.
```

---

## A4 · Niña

**Traza:** guion §1.1 y §4.2 (acompañante en el Nivel 1; jugable en el Nivel 2).
**Chroma:** sí.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Niña, personaje acompañante del Nivel 1. Personaje ORIGINAL.

RASGOS FÍSICOS FIJOS: niña prehistórica de unos nueve años, delgada, estatura de niña, cabeza
proporcionalmente grande respecto del cuerpo. Piel #E8B48C. Cabello negro, liso y desordenado,
a la altura de la mandíbula, con un mechón que le cae sobre la frente. Ojos negros MUY grandes y
redondos, expresivos, con dos puntos de luz blancos: son su rasgo más característico. Cejas
delgadas y altas, que le dan permanente cara de estar preguntando algo. Túnica corta de piel en
#A9713F hasta media pierna, sin mangas. Una pulsera de cuerda trenzada en #6B5344 en la muñeca
izquierda. Descalza.

CARÁCTER QUE DEBE LEERSE: observadora, la que formula la pregunta que nadie más hace.

COMPOSICIÓN: cuerpo entero, VISTA LATERAL mirando a la derecha.
Generar tres poses de la MISMA personaje sin variar sus rasgos:
  (1) de pie, cabeza ligeramente inclinada, mirando con curiosidad;
  (2) en cuclillas, golpeando dos piedras pequeñas una contra otra frente a ella;
  (3) de pie, ambos brazos alzados, expresión de asombro con la boca abierta.

FONDO: verde chroma key plano #00FF00.
```

---

## A5 · Niño

**Traza:** guion §1.1 y §4.2 (acompañante; su función narrativa es el ensayo sin método).
**Chroma:** sí.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Niño, personaje acompañante del Nivel 1. Personaje ORIGINAL.

RASGOS FÍSICOS FIJOS: niño prehistórico de unos siete años, más bajo y algo más rechoncho que la
niña, cabeza grande y redonda. Piel #C98B62. Cabello negro, muy corto y revuelto, con dos
mechones que sobresalen hacia arriba en la coronilla. Ojos negros redondos y pequeños, siempre
muy abiertos. Cejas cortas y altas. Sonrisa amplia que deja ver un diente delantero faltante.
Mejillas redondas con un plano de rubor en #C98B62 más saturado. Túnica corta de piel en
#8C4A2F hasta el muslo, sin mangas, ligeramente torcida. Descalzo.

CARÁCTER QUE DEBE LEERSE: impulsivo y entusiasta, prueba todo sin detenerse a pensar.

COMPOSICIÓN: cuerpo entero, VISTA LATERAL mirando a la derecha.
Generar tres poses del MISMO personaje sin variar sus rasgos:
  (1) de pie, inclinado hacia adelante, señalando con el índice, boca abierta gritando de
      entusiasmo;
  (2) arrodillado, ambas manos tanteando el suelo a ciegas;
  (3) saltando con los dos brazos en alto, celebrando.

FONDO: verde chroma key plano #00FF00.
```

---

## A6 · Escenario — interior de la cueva

**Traza:** guion §3.1 y §4 (escenario del Nivel 1), RF-21 (iluminación progresiva), RNF-20.
**Chroma:** **no** — es el fondo completo de la escena.
**Entregar:** cuatro versiones del **mismo encuadre** para los escalones de RF-21.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Fondo de escena — interior de una cueva prehistórica. Vista lateral, plano fijo, sin
personajes y sin objetos sueltos.

COMPOSICIÓN FIJA: interior de cueva amplia vista de lado. Suelo de roca irregular pero
transitable en el tercio inferior. Paredes de roca a izquierda y derecha que enmarcan la escena.
Techo abovedado con estalactitas cortas y redondeadas, nunca puntiagudas ni amenazantes. Al
fondo, a la izquierda, la boca de la cueva como una abertura ovalada hacia la noche exterior.
En el centro del suelo, un claro despejado que es donde se encenderá el fuego: dejar esa zona
libre de detalle. Relación de aspecto 16:9.

Generar CUATRO versiones del MISMO encuadre, idénticas en composición y variando solo la luz:
  (1) OSCURIDAD TOTAL: casi todo en #0B0E14; apenas se insinúan las siluetas de las paredes.
  (2) PENUMBRA: las paredes cercanas al centro se leen en #1C2333, el resto sigue en #0B0E14.
  (3) LUZ MEDIA: la mitad inferior de la escena en #2E3A4F y #4A3B32, con un tinte cálido
      #FF8A3D naciendo del claro central.
  (4) LUZ PLENA: la cueva entera iluminada por fuego, paredes en #4A3B32 y #6B5344, luz cálida
      #FFC94A que baña el centro y se degrada en planos hacia los bordes.

La transición entre las cuatro debe ser gradual y suave. Sin destellos, sin rayos de luz
marcados, sin partículas brillantes.
```

---

## A7 · Montón de hojas secas — cuatro estados

**Traza:** guion §4.3.1 (elemento de escena con cuatro estados visuales), §4.3.3, RF-14, RF-16.
**Chroma:** sí.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Montón de hojas secas y ramitas finas del Nivel 1.

FORMA FIJA: montículo bajo y ancho, de silueta redondeada e irregular, compuesto por hojas
alargadas en #B08541 y ramitas delgadas en #6B5344 que sobresalen en distintos ángulos. Mismo
tamaño y misma silueta en los cuatro estados: solo cambia lo que ocurre encima.

Generar CUATRO versiones del MISMO montón, idénticas en forma:
  (1) INTACTO: solo hojas y ramitas, sin luz.
  (2) CON CHISPAS APAGADAS: cinco o seis puntos pequeños en #FF8A3D alrededor del montón y sobre
      la piedra a su lado, apagándose, sin llama.
  (3) HUMEANTE: un hilo de humo gris azulado #2E3A4F que sube en curva desde el centro, y un
      punto de brasa #E4572E visible entre las hojas.
  (4) ENCENDIDO: llama de tres planos, #FFC94A en el núcleo, #FF8A3D en el cuerpo y #E4572E en
      los bordes, de altura moderada. Sin chispas voladoras rápidas ni destellos.

Los cuatro estados deben distinguirse por FORMA además de por color: sin chispas, chispas
sueltas, hilo de humo, llama. Es un requisito de accesibilidad, no un capricho.

FONDO: verde chroma key plano #00FF00.
```

---

## A8 · Las dos piedras: sílex y pedernal

**Traza:** guion §4.1 («esa piedra gris es sílex… esa redonda y café es pedernal») y §4.2, RF-16.
**Chroma:** sí.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Las dos piedras del Nivel 1, generadas juntas y claramente distinguibles entre sí.

PIEDRA 1 — SÍLEX: gris, angulosa pero de aristas suavizadas, alargada, con bordes que se leen
afilados sin ser puntiagudos. Color base #2E3A4F con un plano más claro #4A3B32 en la cara
iluminada.

PIEDRA 2 — PEDERNAL: café, claramente REDONDEADA, más maciza y compacta que la anterior, del
tamaño de un puño. Color base #6B5344 con un plano más claro #A9713F en la cara iluminada.

La diferencia entre ambas debe leerse por SILUETA —angulosa contra redonda— y no solo por color
(RNF-19).

COMPOSICIÓN: las dos piedras separadas sobre la misma imagen, vistas de tres cuartos, sin
manos y sin personaje. Añadir debajo una segunda fila con las mismas dos piedras en contacto,
en el instante del choque, con tres o cuatro chispas pequeñas en #FFC94A saliendo del punto de
contacto. Chispas discretas, nunca una explosión de luz.

FONDO: verde chroma key plano #00FF00.
```

---

## A9 · Panel de encendido — controles

**Traza:** RF-14 (los tres elementos), RF-15 (deslizante de tres posiciones), RF-19 («Soplar»
deshabilitado y habilitado), RNF-19, RNF-02, guion §4.3.1.
**Chroma:** sí.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Elementos de interfaz del panel de encendido del Nivel 1. Estética de piedra y cuero
tallados, coherente con la ambientación prehistórica, NO estética digital moderna. Sin texto
dentro de la imagen: las etiquetas las pone el motor.

Generar en una sola lámina, ordenados en filas:

FILA 1 — CONTROL DESLIZANTE DE POSICIÓN: riel horizontal tallado en piedra #2E3A4F con tres
muescas marcadas y equidistantes. Sobre él, un tirador con forma de guijarro redondeado en
#A9713F con contorno #1C2333. Dibujar el riel una vez y el tirador tres veces por separado,
para colocarlo en cada muesca. Cada muesca lleva un icono grabado distinto —una silueta pequeña,
una mediana y una grande— para que las tres posiciones se distingan por forma además de por
posición (RNF-19).

FILA 2 — BOTÓN «GOLPEAR»: botón rectangular de esquinas redondeadas, tallado en piedra #4A3B32,
borde de cuero #8C4A2F, con un icono grabado de dos piedras chocando en #F2E8D5. Dos estados:
en reposo, y presionado (mismo botón, ligeramente más bajo y con el plano de sombra reducido).

FILA 3 — BOTÓN «SOPLAR», dos estados que deben distinguirse por FORMA y no solo por color:
  (a) DESHABILITADO: mismo botón en tonos apagados #1C2333 y #2E3A4F, con un icono grabado de
      candado cerrado en #6B5344.
  (b) HABILITADO: botón en #4A3B32 con borde cálido #FFC94A y un icono grabado de soplo —tres
      líneas curvas de aire— en #F2E8D5.

FILA 4 — MARCO DEL ÁREA DE REGISTRO: recuadro vertical vacío, de esquinas redondeadas, con borde
de cuero cosido en #8C4A2F y fondo interior liso #0B0E14 para que el texto en #F2E8D5 se lea con
contraste alto. Completamente vacío por dentro: sin líneas, sin texto, sin adornos.

FONDO: verde chroma key plano #00FF00.
```

---

## A10 · Marco de diálogo del guía

**Traza:** RF-05, RF-06 (botón de omitir), RNF-01, RNF-20, guion §2.
**Chroma:** sí.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Marco del cuadro de diálogo de las escenas narrativas. Sin texto dentro.

FORMA FIJA: recuadro horizontal ancho de esquinas redondeadas, ocupando el tercio inferior del
encuadre. Borde de cuero cosido en #8C4A2F de grosor uniforme, con puntadas visibles en #F2E8D5
a lo largo del borde. Fondo interior liso #0B0E14 con opacidad alta, para contraste de lectura.
En la esquina superior izquierda, una placa pequeña de hueso #F2E8D5 con forma de óvalo
irregular: es donde irá el nombre del personaje que habla. Interior completamente vacío.

Generar además, por separado en la misma lámina:
  (1) un icono de flecha triangular hacia la derecha en #FFC94A, para «continuar»;
  (2) un icono de doble flecha hacia la derecha en #F2E8D5 dentro de un botón redondeado de
      cuero #8C4A2F, para «omitir».

FONDO: verde chroma key plano #00FF00.
```

---

## Postproceso de los assets con chroma

1. Recortar el verde `#00FF00` y exportar PNG con alfa.
2. Revisar el halo verde en los bordes; si queda, encogerlo un píxel.
3. Importar como Sprite en `Assets/Game/Art/`, filtro **Point (no filter)** si el arte se ve mejor
   nítido, `Pixels Per Unit` uniforme para todo el slice.
4. Verificar RNF-20 sobre el arte final, no sobre el prompt: el contraste se mide en la imagen.
5. Registrar el asset en `CreditsContent.asset` (T08) — CT-09, RNF-23.
