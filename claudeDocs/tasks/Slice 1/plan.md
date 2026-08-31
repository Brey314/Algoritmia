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
| R4 | ~~PG-07: sin autorización escrita de la Familia Anonaky~~ | **Cerrado (30/08/2026)** | La autorización se concedió por escrito. Los personajes son **obra derivada** de los diseños Anonaky —rediseñados, pero partiendo de ellos—, así que su **reconocimiento en créditos es obligatorio** (T08, supuesto 3, CT-09, RNF-23). |
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

Diez assets. Generador: **Gemini / Nano Banana Pro**. Los prompts están en español y se pegan
tal cual, en un solo mensaje, **sin resumirlos**.

**Documento que manda:** `claudeDocs/Direccion_de_Arte.md`. La paleta, el grosor de línea, el
sombreado y las especificaciones técnicas de abajo salen de ahí; si algo se contradice, gana la
dirección de arte, y si esta contradice a `SPEC.md`, gana `SPEC.md`.

**Cómo se arma un prompt.** Cinco bloques fijos, siempre en este orden, y después la descripción
del asset:

```
[1 CONTEXTO]  [2 ESTILO]  [3 PALETA N1]  [4 ENTREGA]  [5 PROHIBICIONES]  +  ELEMENTO
```

Los cinco primeros se copian **literalmente, palabra por palabra, en cada generación**. Ahí está
la consistencia entre piezas: lo único que cambia de un asset a otro es el bloque `ELEMENTO`.
Un asset generado sin los cinco bloques se descarta y se vuelve a pedir; no se «arregla» a mano.

**Autoría (CT-09, RNF-23).** Dos regímenes distintos, y conviene no mezclarlos:

- **Personajes (`A1`..`A5`): obra derivada** de los diseños de la Familia Anonaky. Se
  rediseñaron, pero partieron de ellos, y por eso hacía falta permiso. La **autorización escrita
  está concedida** (PG-07 cerrado el 30/08/2026) y su reconocimiento expreso en la pantalla de
  créditos es **obligatorio**, no opcional (T08).
- **Entornos, props e interfaz (`A6`..`A10`): originales del proyecto.** No dependen de PG-07.

Todo asset generado —de cualquiera de los dos grupos— se registra en `CreditsContent.asset`
(T08), con la mención de autoría que le corresponda.

**Transparencia.** Gemini no produce canal alfa fiable. Los assets marcados **Chroma sí** se
piden sobre verde `#00FF00` plano y se recortan después en Unity. El único **Chroma no** es `A6`,
que es el fondo completo de la escena.

---

## Bloque 1 · CONTEXTO — copiar primero, siempre

```
CONTEXTO DEL ENCARGO
Soy diseñador de un videojuego educativo 2D hecho en Unity para estudiantes de grado cuarto de
primaria, de 9 a 11 años. El juego acompaña a una familia prehistórica en tres descubrimientos:
el fuego, la rueda y el cruce de un río. Este encargo pertenece al Nivel 1, «La Oscuridad», que
transcurre de noche dentro de una cueva.

Lo que necesito NO es una ilustración de escena, ni una lámina de presentación, ni un concept
art. Es un ASSET DE PRODUCCIÓN: un archivo que voy a recortar e importar a Unity como sprite,
que se verá en movimiento, superpuesto a otros elementos, a un tamaño mucho menor que el de
generación, y proyectado en pantallas de aula de baja calidad. Una imagen bonita que no se pueda
recortar limpiamente, o que no se lea a tamaño pequeño, no me sirve y la descarto.

Tres condiciones mandan sobre cualquier consideración estética:
1. PÚBLICO INFANTIL. Nada amenazante, afilado, sombrío, triste ni violento. La prehistoria se
   representa como un mundo de descubrimiento y asombro, nunca de supervivencia o peligro. No
   hay depredadores, no hay armas, no hay heridas, no hay muerte.
2. BAJO CONSUMO DE RECURSOS. Los equipos del colegio no tienen tarjeta gráfica dedicada. El arte
   es plano y simple por diseño, no por descuido.
3. LEGIBILIDAD ANTES QUE DETALLE. Si un detalle compite con la lectura de la silueta, sobra.

INSTRUCCIÓN SOBRE LO QUE NO TE DIGA: sigue las secciones de abajo al pie de la letra. Donde no
te dé un dato, NO lo inventes ni lo rellenes con tu criterio: elige la opción más simple
compatible con las reglas y deja el resto vacío. No añadas objetos, personajes, adornos, texto,
fondo, marcos ni elementos decorativos que no haya pedido explícitamente. Si crees que falta
algo, omítelo: prefiero un asset incompleto a uno inventado.
```

## Bloque 2 · ESTILO — fijo para los tres niveles

```
ESTILO (fijo, no variar entre generaciones)
Ilustración vectorial 2D, cartoon clásico de animación televisiva, formas grandes y contornos
firmes. Aspecto limpio y macizo, nunca esbozado ni pintado.

COLOR: completamente plano y saturado. PROHIBIDOS los degradados de cualquier tipo, el
aerógrafo, el difuminado, las transiciones suaves, el ruido, la textura de superficie y el
volumen pintado.

SOMBRA (crítico): exactamente DOS tonos por color, base y sombra, separados por un borde duro y
nítido, como un recorte de papel. Ni un tercer tono, ni luz especular, ni brillo. Luz global
desde arriba a la izquierda a 45 grados: la sombra ocupa el lado derecho de cada forma. ÚNICA
EXCEPCIÓN: el fuego, que lleva tres tonos por ser fuente de luz.

LÍNEA: contorno cerrado en todo el perímetro, sin aberturas. Grosor según la capa, medido a
1024 px de alto:
  - personajes: 8 a 12 px, color #3A1E18, más grueso en la silueta exterior que en los
    detalles internos;
  - objetos interactivos: 7 a 9 px, color #3A1E18;
  - decorado de primer plano: 6 px, color #4A2E24;
  - decorado de plano medio: 4 px, color #5C4038;
  - fondo lejano: SIN contorno.
Sin líneas internas innecesarias: nada de pliegues de ropa, arrugas, músculos, clavículas,
vetas de madera ni texturas de piedra. Una superficie es un color plano con su contorno.

FORMA: todo redondeado. Rocas de cantos romos, troncos de sección ovalada, esquinas suaves.
Ningún vértice en ángulo agudo, salvo en objetos que deban leerse como «fabricados por alguien»
(herramientas, piezas talladas), donde la angulosidad es intencional.

SILUETA: el asset debe ser reconocible solo por su contorno, relleno de negro sólido, a 128 px
de alto. Si dos elementos no se distinguen en silueta, están mal diseñados.
```

## Bloque 3 · PALETA DEL NIVEL 1 — usar solo estos colores

```
PALETA (fija, no usar ningún color fuera de esta lista)

PERSONAJES (idéntica en los tres niveles, no se tiñe con la luz del entorno):
  Piel base #F2D3BC        Piel sombra #D9AF95
  Cabello base #5C2B22     Cabello sombra #3D1A14
  Piel de leopardo (adultos) #E8C07A   su sombra #C49A55   manchas #2B1A12
  Túnica del niño (oliva) #C4C24E      su sombra #9BA03A   manchas #3F6B2E
  Conjunto de la niña (ocre) #D9B23A   su sombra #B08A25   manchas #7A5418
  Rubor infantil #F0A5A0
  Contorno de personaje #3A1E18

ENTORNO DEL NIVEL 1 (cueva de noche):
  Cielo exterior #1B2A4A       Silueta del exterior #141F38 (sin contorno)
  Roca #3E3550                 Roca en sombra #2A2438
  Suelo #5E4A52                Suelo en sombra #42333A
  Estalagmitas #6B5A60         Musgo #4A5C42
  Charcos #2E4258 con reflejo de línea recta #4A6B8C
  Pinturas rupestres #8C4A2F (sin contorno, formas muy simplificadas)
  Estrellas #F7EFE2 y #BFD4E8 (puntos de dos tamaños)

ACENTO DEL NIVEL — SOLO PARA EL FUEGO Y LO INTERACTIVO:
  Núcleo de llama #FFE9A8    Cuerpo de llama #F5A62E    Borde de llama #E2571F
  Halo de luz #F0A84E al 20 por ciento, círculo plano

NEUTROS DE INTERFAZ (comunes a todo el juego):
  Marfil #F7EFE2   Marfil sombra #E0D4C0   Borde de panel #C4A882
  Carbón #3A1E18 (texto y contorno)        Carbón suave #6B5248
  Éxito #5FA842    Atención #E8A33D

REGLA DE ACENTO (crítica): el naranja, el amarillo y el rojo pertenecen EXCLUSIVAMENTE al fuego
y a los objetos del reto. Ninguna roca, pared, planta, sombra ni elemento de decorado puede
usarlos. Si el elemento que te pido no es fuego ni es interactivo, no lleva ni una pincelada de
esos tonos.
```

## Bloque 4 · ENTREGA TÉCNICA — lo que hace que el archivo sirva

```
ENTREGA TÉCNICA (obligatoria)
FONDO: verde croma puro #00FF00, plano y absolutamente uniforme, sin degradado, sin viñeta y
sin sombra proyectada sobre él. El verde no aparece en ninguna otra parte de la imagen.

ENCUADRE: el elemento completo dentro del lienzo, sin recortes por ningún borde, con al menos un
10 por ciento de margen vacío arriba, abajo, a izquierda y a derecha.

COMPOSICIÓN: un único elemento centrado, de frente al plano indicado, sin perspectiva dramática,
sin escorzo y sin inclinación de cámara. Sin suelo, sin horizonte y sin escenario detrás: el
elemento flota aislado sobre el verde.

RESOLUCIÓN: imagen cuadrada, la mayor que puedas, con el elemento ocupando al menos el 70 por
ciento de la altura útil. PNG sin compresión con pérdida.

NADA DE TEXTO: ni letras, ni números, ni palabras, ni firmas, ni marcas de agua, ni logotipos,
ni viñetas, ni bordes decorativos, ni fichas de personaje, ni paletas de color al margen. Las
etiquetas las pone el motor del juego, no la imagen.

SI TE PIDO VARIAS VERSIONES: genéralas en la MISMA imagen, separadas y alineadas, sobre el mismo
verde, sin marcos ni títulos entre ellas, y sin variar ni un rasgo del elemento entre una versión
y otra: solo cambia lo que yo indique que cambia.
```

## Bloque 5 · PROHIBICIONES — la lista que más se incumple

```
PROHIBIDO (si aparece alguno, el asset se descarta)
- Degradados, difuminados, aerógrafo, ruido, textura, papel, acuarela o pintura visible.
- Un tercer tono de sombra, brillo especular, reflejo o iluminación volumétrica.
- Halos, glows, destellos, rayos de luz, partículas brillantes o lens flare.
- Sombra proyectada sobre el fondo verde.
- Contorno abierto o interrumpido en cualquier punto del perímetro.
- Texto, números, firmas, marcas de agua o logotipos.
- Violencia, armas, sangre, heridas, dientes afilados, gestos de enfado o de tristeza.
- Colores fuera de la paleta indicada arriba.
- Naranja, amarillo o rojo en cualquier cosa que no sea fuego o un objeto del reto.
- Elementos de escenario, suelo, cielo o decorado alrededor del elemento pedido.
- Estilo realista, 3D, render, pixel art, anime o acuarela.
```

---

## A1 · Chispa, el guía

**Traza:** guion §1.1 y §4.1, PG-02, RF-10, RF-12, RF-13, CN-03 (presente en los tres niveles).
**Chroma:** sí. **Archivo:** `char_chispa_base_reposo.png`, `_girando`, `_atenuado`.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: Chispa, el personaje guía del juego. No es humano ni animal: es una pequeña criatura
de luz con forma de estrella.

FORMA EXACTA: estrella de CINCO puntas, todas ellas REDONDEADAS en el extremo, nunca en punta
afilada. El cuerpo es macizo y compacto, del tamaño de una palma de mano adulta; su anchura
total equivale a poco más de una cabeza humana. Las cinco puntas son de la misma longitud y
están repartidas de forma regular, con la punta superior apuntando verticalmente hacia arriba.

RELLENO: cuerpo entero en #F5A62E. Sobre él, un área de núcleo más clara en #FFE9A8 que ocupa
aproximadamente la mitad central de la estrella y repite su forma en pequeño, con BORDE DURO,
sin degradado alguno entre los dos tonos.

CONTORNO: 8 px en #E2571F. Es el único elemento del nivel con contorno cálido en lugar del
#3A1E18 habitual, y es intencional: marca que Chispa emite luz propia.

CARA: dos ojos negros grandes y ovalados, muy separados entre sí, situados en el tercio superior
del cuerpo, cada uno con un punto de luz blanco circular en su esquina superior izquierda. Boca
pequeña, una sola línea curva hacia arriba, sonrisa cerrada y amable. SIN nariz, SIN cejas, SIN
brazos, SIN piernas, SIN manos, SIN sombrero, SIN accesorios.

ESTELA: entre cinco y siete puntos de luz sueltos en #FFE9A8, circulares, de tamaño decreciente,
alineados en una curva suave detrás del cuerpo. Puntos separados y bien definidos, nunca una
nube difuminada ni un rastro degradado.

GENERAR TRES VERSIONES en la misma imagen, alineadas horizontalmente, IDÉNTICAS en forma, color,
tamaño y expresión salvo en lo que se indica:
  (1) EN REPOSO: estrella vertical, estela corta de tres puntos, sonrisa suave.
  (2) GIRANDO: la misma estrella inclinada 25 grados hacia la derecha, estela larga de siete
      puntos describiendo un arco. Sigue siendo la misma estrella: no la deformes.
  (3) ATENUADA: la misma estrella con el relleno más apagado —cuerpo en #E2571F y núcleo en
      #F5A62E, un escalón más oscuro— y estela de solo dos puntos. Sin transparencia y sin
      difuminado: el apagado se consigue cambiando de tono, no bajando la opacidad.
```

**Verificación (§17):** silueta de estrella reconocible en negro a 128 px · dos tonos de relleno
con borde duro · contorno cálido cerrado · sin brazos ni piernas · estela de puntos separados ·
las tres versiones son la misma criatura.

---

## A2 · Papá — personaje jugable del Nivel 1

**Traza:** guion §1.1 y §4.2, RF-14, HU-06, CN-02. Obra derivada con autorización concedida
(PG-07, CT-09, RNF-23) — mención obligatoria en créditos.
**Chroma:** sí. **Archivo:** `char_papa_base_apose.png`.

> **Por qué A-pose y no las poses del nivel.** Las poses de golpear y soplar **no se generan**:
> se producen en Unity animando este sprite con el paquete 2D Animation, que ya está en
> `manifest.json` (`Direccion_de_Arte.md` §13.1). Pedirle a Gemini tres poses del mismo personaje
> devuelve tres personajes distintos; pedirle una pose de producción devuelve un asset que se
> puede riggear. Lo que el rig necesita es que **los brazos y las piernas estén separados del
> torso, con fondo visible entre ellos**.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: Papá, personaje jugable del Nivel 1. Hombre adulto prehistórico, de aspecto amable y
tranquilo. Personaje de diseño propio: NO reproduzcas ningún personaje de franquicia, película,
serie ni videojuego existente.

PROPORCIÓN (crítica, es lo que produce la lectura infantil): la CABEZA ocupa un tercio de la
altura total del cuerpo. Cuerpo ancho y robusto, hombros marcados, torso macizo, piernas cortas
y sólidas. Altura total de referencia: 1.00 (mamá 0.92, los niños 0.60).

CABEZA: redonda y grande. Cabello #5C2B22 con sombra #3D1A14, tupido, hasta el hombro, recogido
atrás con una tira de cuero. Barba corta y espesa del mismo color, de contorno redondeado, nunca
en punta. Cejas gruesas y RECTAS, separadas entre sí. Ojos negros, ovalados, medianos, mirada
serena y atenta, NUNCA fiera ni entrecerrada con agresividad. Nariz ancha y redondeada, dibujada
con una sola línea curva. Boca en sonrisa cerrada suave. Piel #F2D3BC con sombra #D9AF95 en el
lado derecho del rostro y bajo el mentón.

VESTUARIO: túnica de piel de leopardo sin mangas en #E8C07A con sombra #C49A55, que cae hasta
media pierna, con manchas ovaladas irregulares en #2B1A12 repartidas sin patrón regular. Ceñida
a la cintura con una cuerda trenzada en #5C2B22. Descalzo, pies desnudos. SIN cicatrices, SIN
pinturas de guerra, SIN collares, SIN armas, SIN garrote, SIN objetos en las manos.

MANOS: color piel, CUATRO dedos visibles, dedos gruesos y redondeados, mano abierta y relajada.
Nunca puño cerrado, nunca guante, nunca mano fundida con el cuerpo.

POSE (crítica; es una pose de producción y debe verse rígida a propósito): A-POSE de frente.
Cuerpo completamente frontal a la cámara, mirando al espectador. Brazos extendidos hacia los
lados y hacia abajo, formando unos 45 grados con el torso, COMPLETAMENTE SEPARADOS del cuerpo,
con fondo verde claramente visible entre cada brazo y el costado, y las axilas abiertas. Piernas
rectas y ligeramente separadas, con fondo verde visible entre ellas. Pies apoyados, apuntando al
frente. Expresión neutra: cejas rectas, ojos abiertos, sonrisa cerrada suave.

NO generes poses de acción, ni de perfil, ni arrodillado, ni soplando, ni sosteniendo nada. Una
sola figura, una sola pose, centrada.

SIN LÍNEAS INTERNAS ANATÓMICAS: nada de pectorales, clavículas, abdominales, ombligo, rodillas
marcadas ni músculos. El torso es un color plano con su contorno y su sombra.
```

**Verificación (§17):** cabeza = 1/3 de la altura · A-pose con axilas abiertas y verde entre
brazos y torso · cuatro dedos, mano abierta · sin líneas anatómicas internas · piel `#F2D3BC` sin
teñir · silueta distinguible de mamá, niña y niño en negro sólido.

---

## A3 · Mamá

**Traza:** guion §1.1 y §4.2 (acompañante en el Nivel 1; jugable en el Nivel 3, CN-02).
**Chroma:** sí. **Archivo:** `char_mama_base_apose.png`.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: Mamá, personaje de la familia. Acompaña en el Nivel 1 y es la jugable del Nivel 3.
Mujer adulta prehistórica, serena y metódica. Diseño propio, sin parecido con personajes de
franquicias existentes.

PROPORCIÓN: la cabeza ocupa un tercio de la altura total. Altura relativa 0.92 respecto a papá
—algo más baja— y torso claramente MÁS ESTRECHO: 0.68 frente a 1.00. Figura esbelta y de curvas
suaves, que es lo que la separa de papá en silueta.

CABEZA: redonda. Cabello #5C2B22 con sombra #3D1A14, largo hasta media espalda, liso, apartado
del rostro y recogido en la nuca con una tira de cuero; el volumen del recogido debe verse por
detrás de la silueta de la cabeza. Cejas finas y ligeramente arqueadas. Ojos negros ovalados,
algo más grandes que los de papá, mirada atenta y cálida. Nariz pequeña y redondeada. Boca en
sonrisa cerrada suave. Rubor #F0A5A0 en dos óvalos planos, uno en cada mejilla. Piel #F2D3BC con
sombra #D9AF95.

VESTUARIO: túnica de piel de leopardo #E8C07A con sombra #C49A55, sin mangas, hasta la rodilla,
con manchas ovaladas #2B1A12 irregulares. Ceñida con cuerda trenzada #5C2B22. Descalza. SIN
collares, SIN pulseras, SIN flores en el pelo, SIN objetos en las manos.

MANOS: color piel, cuatro dedos, abiertas y relajadas.

POSE: A-POSE de frente, idéntica en criterio a la de papá. Brazos a 45 grados, separados del
torso, axilas abiertas y fondo verde visible entre brazos y costados. Piernas rectas y
ligeramente separadas, con verde visible entre ellas. Expresión neutra.

SIN LÍNEAS INTERNAS ANATÓMICAS: nada de busto marcado, cintura sombreada, clavículas ni ombligo.
La túnica es un color plano con su contorno y su sombra.
```

**Verificación (§17):** torso más estrecho y silueta claramente distinta de la de papá en negro
sólido · A-pose con axilas abiertas · sin líneas anatómicas · misma piel `#F2D3BC` que el resto.

---

## A4 · Niña

**Traza:** guion §1.1 y §4.2 (acompañante en el Nivel 1; jugable en el Nivel 2, CN-02).
**Chroma:** sí. **Archivo:** `char_nina_base_apose.png`.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: la Niña de la familia, de unos nueve años. Observadora y curiosa. Acompaña en el
Nivel 1 y es la jugable del Nivel 2. Diseño propio, sin parecido con personajes existentes.

PROPORCIÓN (distinta a la de los adultos): la CABEZA ocupa DOS QUINTOS de la altura total, no un
tercio. Cabeza grande y muy redonda. Altura relativa 0.60 respecto a papá, torso 0.52. Cuerpo
delgado, brazos y piernas finos.

RASGO DE SILUETA (crítico): el pelo forma un PENACHO ALTO, un mechón recogido en lo alto de la
cabeza que se eleva verticalmente y termina redondeado. Es lo que la distingue del niño en negro
sólido, así que debe ser inconfundible y sobresalir con claridad del cráneo. Cabello #5C2B22 con
sombra #3D1A14.

CARA: ojos negros muy grandes y redondos, más grandes en proporción que los de los adultos, muy
separados, con un punto de luz blanco en la esquina superior de cada uno. Cejas finas y rectas.
Nariz mínima, un solo trazo curvo corto. Boca pequeña en sonrisa cerrada. Rubor #F0A5A0 en dos
óvalos planos. Piel #F2D3BC con sombra #D9AF95.

VESTUARIO: conjunto ocre #D9B23A con sombra #B08A25, una pieza sin mangas hasta la rodilla, con
manchas irregulares #7A5418. Descalza. SIN adornos, SIN collares, SIN juguetes.

MANOS: color piel, cuatro dedos, abiertas.

POSE: A-POSE de frente, mismo criterio que los adultos. Brazos a 45 grados separados del torso,
axilas abiertas con verde visible, piernas ligeramente separadas con verde entre ellas. Expresión
neutra pero despierta.

MISMA ALTURA Y MISMA LÍNEA DE SUELO que el niño del asset A5: los dos comparten estatura exacta.
No hagas a uno más alto que el otro.
```

**Verificación (§17):** cabeza = 2/5 de la altura · penacho alto inconfundible en silueta · misma
altura exacta que el niño · A-pose con axilas abiertas.

---

## A5 · Niño

**Traza:** guion §1.1 y §4.2 (acompañante en los tres niveles).
**Chroma:** sí. **Archivo:** `char_nino_base_apose.png`.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: el Niño de la familia, de unos siete años. Impulsivo y entusiasta. Acompaña en los
tres niveles y nunca es jugable. Diseño propio, sin parecido con personajes existentes.

PROPORCIÓN: la cabeza ocupa DOS QUINTOS de la altura total, igual que la niña. Altura relativa
0.60 —EXACTAMENTE la misma que la niña, con la misma línea de suelo— pero cuerpo algo más
rechoncho: torso 0.55. Cabeza muy grande y redonda.

RASGO DE SILUETA (crítico): el pelo forma un COPETE PUNTIAGUDO hacia adelante, corto y revuelto,
que sobresale sobre la frente. Es lo que lo distingue de la niña en negro sólido: ella lleva un
penacho vertical alto, él un copete inclinado hacia el frente. Que no se parezcan. Cabello
#5C2B22 con sombra #3D1A14.

CARA: ojos negros muy grandes y redondos, con punto de luz blanco. Cejas cortas y elevadas, que
le dan expresión despierta. Nariz mínima. Boca en sonrisa abierta pequeña, alegre, sin mostrar
dientes afilados. Rubor #F0A5A0 en dos óvalos planos. Piel #F2D3BC con sombra #D9AF95.

VESTUARIO: túnica oliva #C4C24E con sombra #9BA03A, sin mangas, hasta la rodilla, con manchas
irregulares #3F6B2E. Descalzo. SIN adornos, SIN armas, SIN palos.

MANOS: color piel, cuatro dedos, abiertas.

POSE: A-POSE de frente, mismo criterio que los demás. Brazos a 45 grados separados del torso,
axilas abiertas con verde visible entre brazo y costado, piernas ligeramente separadas.
```

**Verificación (§17):** copete hacia adelante claramente distinto del penacho de la niña en negro
sólido · misma altura y línea de suelo que ella · cabeza = 2/5.

---

## A6 · Escenario — interior de la cueva, cuatro escalones de luz

**Traza:** guion §3.1 y §4 (escenario del Nivel 1), RF-21 (iluminación progresiva, prioridad
Baja), RNF-20. **Chroma:** **no** — es el fondo completo de la escena.
**Archivo:** `env_n1_cueva_luz1.png` … `env_n1_cueva_luz4.png`.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [5 PROHIBICIONES]

ELEMENTO: fondo completo de escena. Interior de una cueva prehistórica de noche, vista LATERAL,
plano fijo, SIN personajes y SIN objetos sueltos.

FORMATO: rectangular 16:9 horizontal. Este asset NO lleva fondo verde: la imagen entera es el
escenario y se importa tal cual. Ignora la instrucción de croma; el resto de las reglas sigue
vigente.

COMPOSICIÓN FIJA (idéntica en las cuatro versiones, no la muevas ni un píxel):
- Suelo de roca en el tercio inferior, superficie irregular pero continua y transitable, de
  ondulaciones suaves y cantos romos. Color #5E4A52 con sombra #42333A.
- Paredes de roca a izquierda y derecha que enmarcan la escena, en #3E3550 con sombra #2A2438.
- Techo abovedado de curva amplia, con cuatro o cinco estalactitas CORTAS y de punta REDONDEADA
  colgando, nunca afiladas ni amenazantes, en #6B5A60.
- Tres estalagmitas bajas y romas en el suelo, dos a la izquierda y una a la derecha, en #6B5A60.
- En el fondo a la IZQUIERDA, la boca de la cueva: una abertura con forma de arco redondeado que
  deja ver el exterior nocturno. El exterior es una silueta plana #141F38 SIN contorno y SIN
  detalle, con unos quince puntos de estrella repartidos irregularmente, unos en #F7EFE2 y otros
  más pequeños en #BFD4E8. SIN luna: la única fuente de luz cálida del nivel es el fuego.
- Decorado escaso y pegado a las paredes: dos manchas de musgo #4A5C42, un charco ovalado
  #2E4258 con un solo reflejo de línea recta #4A6B8C, y dos pinturas rupestres muy simplificadas
  en #8C4A2F sobre la pared derecha —siluetas de mano y de animal, sin contorno—.
- EN EL CENTRO DEL SUELO, UN CLARO COMPLETAMENTE VACÍO: una zona despejada, sin rocas, sin musgo
  y sin detalle, donde el motor colocará la hoguera. Déjala limpia.

GENERAR CUATRO VERSIONES de la MISMA imagen, idénticas en composición, encuadre y posición de
cada elemento. Lo ÚNICO que cambia es cuánta oscuridad las cubre, como si una lámina plana
#0F1526 se fuera retirando:
  (1) 65 por ciento de oscuridad: casi todo el encuadre en #0F1526; apenas se insinúan las
      siluetas de las paredes y las estrellas de la abertura.
  (2) 45 por ciento: se leen las paredes cercanas al centro y el contorno del suelo.
  (3) 25 por ciento: se lee la cueva entera con sus colores de paleta, aún apagada.
  (4) 0 por ciento: la cueva completamente visible con sus colores plenos.

La lámina de oscuridad es un COLOR PLANO uniforme sobre toda la imagen: no es una viñeta, no es
un degradado radial y no es un foco de luz. Sin rayos de luz, sin haces, sin partículas.
NO añadas fuego en ninguna de las cuatro versiones: el fuego es otro asset.
```

**Verificación (§17):** las cuatro versiones son el mismo encuadre · el claro central está vacío
en las cuatro · nada de naranja, amarillo ni rojo en el decorado · el contorno del escenario es
más fino que el de un personaje · pasa la prueba de entrecerrado (§6).

---

## A7 · Montón de hojas secas — cuatro estados

**Traza:** guion §4.3.1, §4.3.3 y §4.3.4, RF-14, RF-16, RNF-19.
**Chroma:** sí. **Archivo:** `prop_n1_hojas_intacto.png` … `prop_n1_hojas_encendido.png`.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: el montón de hojas secas y ramitas del Nivel 1. Es el objetivo del reto: el estudiante
tiene que encenderlo. Objeto INTERACTIVO, así que lleva contorno grueso de 7 a 9 px en #3A1E18.

FORMA FIJA (idéntica en los cuatro estados, no la varíes): montículo bajo y ancho, más ancho que
alto, de silueta redondeada e irregular. Compuesto por unas ocho hojas alargadas y ovaladas en
#D9B23A con sombra #B08A25, superpuestas en distintos ángulos, y cinco ramitas delgadas en
#5C2B22 que asoman entre ellas por los lados. Nada más: sin piedras, sin suelo, sin hierba.

GENERAR CUATRO VERSIONES en la misma imagen, alineadas horizontalmente, con el montón IDÉNTICO
en forma, tamaño y color en las cuatro. Lo único que cambia es lo que ocurre encima:
  (1) INTACTO: solo el montón. Sin fuego, sin humo, sin chispas.
  (2) CHISPAS QUE SE APAGAN: seis puntos pequeños en #FFE9A8 repartidos alrededor del montón, de
      tamaño decreciente, algunos ya casi extinguidos. SIN llama, SIN humo, SIN brasa. Es el
      resultado de un intento que no prendió.
  (3) HUMEANTE: un hilo de humo que sube desde el centro, dibujado como una cinta ondulada de
      color plano #6B5A60, de anchura decreciente hacia arriba, con contorno propio. En el centro
      del montón, una brasa: un óvalo pequeño #E2571F con un núcleo #F5A62E.
  (4) ENCENDIDO: una llama de TRES tonos, única excepción a la regla de dos: núcleo #FFE9A8 en
      forma de óvalo pequeño, cuerpo #F5A62E como lengua de llama redondeada de altura moderada,
      y borde exterior #E2571F. Los tres con BORDE DURO entre sí, sin degradado. La llama no
      supera el doble de la altura del montón. SIN chispas volando y SIN destellos.

CRITERIO DE ACCESIBILIDAD (obligatorio): los cuatro estados deben distinguirse por FORMA además
de por color —nada encima, puntos sueltos, cinta de humo, llama—, de modo que sigan siendo
distinguibles en escala de grises.
```

**Verificación (§17):** el montículo es idéntico en los cuatro · los cuatro estados se distinguen
en escala de grises (RNF-19) · la llama tiene exactamente tres tonos de borde duro · ningún
estado lleva destellos ni chispas rápidas (RNF-21).

---

## A8 · Las dos piedras: sílex y pedernal

**Traza:** guion §4.1 («esa piedra gris es sílex… esa redonda y café es pedernal») y §4.2, RF-16,
RNF-19. **Chroma:** sí.
**Archivo:** `prop_n1_silex.png`, `prop_n1_pedernal.png`, `prop_n1_piedras_choque.png`.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: las dos piedras con las que se enciende el fuego en el Nivel 1. Objetos INTERACTIVOS:
contorno de 7 a 9 px en #3A1E18. Se generan juntas en la misma imagen y tienen que ser
inconfundibles entre sí.

PIEDRA 1 — SÍLEX: gris, ALARGADA y de silueta ANGULOSA, con cuatro o cinco caras planas y
aristas marcadas, pero con los vértices SUAVIZADOS: se lee como piedra tallada, nunca como
cuchilla ni punta de lanza. Proporción aproximada de dos de largo por uno de alto. Color base
#6B5A60 con sombra #42333A en el lado derecho.

PIEDRA 2 — PEDERNAL: café, claramente REDONDEADA y compacta, del tamaño de un puño, sin aristas
de ningún tipo, silueta casi ovalada. Más maciza y más baja que el sílex. Color base #5E4A52 con
un plano más claro #8C4A2F en la cara superior izquierda.

LA DIFERENCIA ENTRE AMBAS DEBE LEERSE POR SILUETA —angulosa contra redonda— y no solo por color:
si las rellenas de negro sólido, tienen que seguir siendo distinguibles (RNF-19).

COMPOSICIÓN: dos filas sobre la misma imagen.
  FILA SUPERIOR: las dos piedras separadas entre sí, vistas de tres cuartos, sin tocarse. SIN
  manos, SIN personaje, SIN suelo debajo.
  FILA INFERIOR: las mismas dos piedras en contacto por un punto, en el instante del choque, con
  TRES chispas pequeñas en #FFE9A8 saliendo de ese punto como líneas cortas radiales. Tres
  chispas, no más. Nunca una explosión de luz, nunca un destello, nunca un halo.
```

**Verificación (§17):** las dos siluetas se distinguen en negro sólido · aristas suavizadas,
ninguna punta afilada · exactamente tres chispas en la fila inferior · sin manos ni suelo.

---

## A9 · Panel de encendido — controles

**Traza:** RF-14 (los tres elementos del panel), RF-15 (deslizante de tres posiciones), RF-19
(«Soplar» deshabilitado y habilitado), RNF-19, RNF-02, CP-08, guion §4.3.1.
**Chroma:** sí. **Archivo:** `ui_n1_panel_<pieza>_<estado>.png`.

> **PG-06 abierto:** el deslizante lleva **tres** muescas porque es lo que propone el guion
> §4.3.2 (Lejos / Cerca / Muy cerca). Si al validarlo jugando cambia el número, este asset se
> vuelve a generar — el valor vive en `FireLevelConfig`, no en el arte.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: los controles de interfaz del panel de encendido del Nivel 1. Son elementos de
INTERFAZ, no objetos de escenario: planos, frontales, sin perspectiva y sin volumen, legibles a
32 px de alto. Estética de piedra y cuero tallados, coherente con la prehistoria, NUNCA estética
digital moderna: sin bordes metálicos, sin brillos de cristal, sin sombras suaves.

Generar todas las piezas en UNA sola imagen, ordenadas en cuatro filas separadas, sin marcos ni
títulos entre ellas.

FILA 1 — CONTROL DESLIZANTE: un riel horizontal tallado en piedra #6B5A60 con contorno #3A1E18,
de esquinas muy redondeadas, con TRES muescas marcadas, equidistantes y del mismo tamaño. Cada
muesca lleva grabado un icono distinto que indica distancia: en la primera un círculo pequeño, en
la segunda uno mediano, en la tercera uno grande. Los iconos van grabados en #42333A, hundidos en
la piedra. Dibujar ADEMÁS, separado del riel, un tirador suelto: un guijarro redondeado en
#8C4A2F con contorno #3A1E18, del ancho de una muesca. Un solo tirador: el motor lo coloca.

FILA 2 — BOTÓN «GOLPEAR», dos estados uno al lado del otro:
  (a) EN REPOSO: botón rectangular de esquinas muy redondeadas, tallado en piedra #5E4A52 con
      borde de cuero #8C4A2F y contorno #3A1E18, con una sombra plana inferior de 6 px en
      #42333A. Grabado en el centro, un icono de dos piedras chocando en #F7EFE2.
  (b) PRESIONADO: el mismo botón, desplazado hacia abajo lo que medía su sombra, y sin la sombra
      inferior. Nada más cambia.

FILA 3 — BOTÓN «SOPLAR», dos estados que deben distinguirse por FORMA y no solo por color:
  (a) DESHABILITADO: el mismo botón en tonos apagados #42333A y #2A2438, con un icono grabado de
      CANDADO CERRADO en #6B5248. Sin borde de cuero.
  (b) HABILITADO: botón en #5E4A52 con borde de cuero #E8A33D y un icono grabado de SOPLO —tres
      líneas curvas paralelas que sugieren aire— en #F7EFE2.
  El candado y las líneas de aire son el segundo canal de información además del color: sin ellos
  el asset no sirve (RNF-19).

FILA 4 — MARCO DEL ÁREA DE REGISTRO: un recuadro VERTICAL vacío, de esquinas muy redondeadas, con
borde de cuero cosido #8C4A2F y puntadas visibles en #F7EFE2 a lo largo del borde. Interior
completamente liso y VACÍO en marfil #F7EFE2, para que el motor escriba texto oscuro encima con
contraste alto. Nada dentro: sin líneas, sin renglones, sin texto, sin adornos.

TAMAÑO TÁCTIL: todos los botones son cuadrados o casi, de proporción generosa, pensados para
dedos de un niño de nueve años. Ninguno alargado ni estrecho.
```

**Verificación (§17):** cada estado se distingue por forma y no solo por color (RNF-19) · interior
del marco completamente vacío · sin texto en la imagen · el par `#3A1E18` sobre `#F7EFE2` sostiene
el contraste ≥ 4.5:1 (RNF-20) · botones de área generosa (§10.1).

---

## A10 · Marco de diálogo del guía

**Traza:** RF-05, RF-06 (botón de omitir, solo en escena ya vista), RNF-01, RNF-20, CP-08,
guion §2. **Chroma:** sí. **Archivo:** `ui_dialogo_marco.png`, `ui_dialogo_continuar.png`,
`ui_dialogo_omitir.png`.

> Este asset lo reutilizan los cuatro slices: las escenas narrativas de los tres niveles y el
> resumen de fin de nivel del Slice 4. Es lo que hace que el resumen se lea como andamiaje y no
> como una pantalla de puntaje (CP-03, RF-45). No se genera otro.

```
[1 CONTEXTO] [2 ESTILO] [3 PALETA N1] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: el marco del cuadro de diálogo de las escenas narrativas. Elemento de INTERFAZ: plano,
frontal, sin perspectiva. Va a llevar texto encima, escrito por el motor, así que su interior
tiene que quedar completamente limpio.

PIEZA 1 — MARCO: recuadro HORIZONTAL ancho, de proporción aproximada 3 de ancho por 1 de alto,
con las esquinas MUY redondeadas. Aspecto de tablilla clara enmarcada en cuero: interior liso en
marfil #F7EFE2, borde de cuero #C4A882 de grosor uniforme con contorno #3A1E18 de 6 px, y
puntadas cortas visibles en #6B5248 repartidas regularmente a lo largo del borde.
En la esquina SUPERIOR IZQUIERDA, sobresaliendo un poco del marco, una placa pequeña con forma de
óvalo irregular en #E0D4C0 con su propio contorno: es donde el motor escribirá el nombre de quien
habla. La placa también va VACÍA.
INTERIOR COMPLETAMENTE VACÍO: sin líneas, sin renglones, sin texto, sin iconos, sin adornos y sin
sombra interior.

PIEZA 2 — ICONO DE CONTINUAR: un triángulo relleno apuntando a la derecha, de esquinas
redondeadas, en #E8A33D con contorno #3A1E18. Suelto, sin botón alrededor.

PIEZA 3 — BOTÓN DE OMITIR: un botón redondeado en cuero #C4A882 con contorno #3A1E18 y, dentro,
grabado en #3A1E18, un icono de DOBLE triángulo apuntando a la derecha. La doble punta es lo que
lo distingue del icono de continuar por forma y no solo por tamaño.

Generar las tres piezas en la misma imagen, separadas y alineadas, sobre el verde.
```

**Verificación (§17):** interior del marco y de la placa completamente vacíos · texto oscuro sobre
fondo claro, nunca al revés (§10.3) · continuar y omitir se distinguen por forma · esquinas muy
redondeadas · sin texto en la imagen.

---

## Postproceso — de la imagen generada al asset importado

Un asset no está terminado cuando Gemini lo devuelve, sino cuando pasa por estos seis pasos.

1. **Verificar contra la checklist** de `Direccion_de_Arte.md` §17 y contra la línea
   «Verificación» de cada asset de arriba. Si falla una, se vuelve a generar: no se retoca a mano
   lo que el prompt puede corregir.
2. **Recortar el verde** `#00FF00` y exportar PNG con alfa. Revisar el halo verde del borde; si
   queda, encogerlo un píxel.
3. **Nombrar** según `Direccion_de_Arte.md` §15.4 — prefijos `char_`, `prop_`, `env_`, `ui_`,
   `fx_`, sin tildes, sin espacios y sin mayúsculas.
4. **Importar** en `Assets/Game/Art/` con los ajustes de §15.2: Texture Type `Sprite (2D and UI)`,
   Filter Mode `Bilinear`, Compression `Normal Quality`, Max Size `2048`, Generate Physics Shape
   desactivado. **Pivot `Bottom`** en personajes y **`Center`** en props e interfaz. **Pixels Per
   Unit `100`** en todo el proyecto, sin excepción: papá mide 1.8 unidades.
5. **Medir** el contraste sobre la imagen final, no sobre el prompt (RNF-20), y comprobar en un
   simulador de deuteranopía que nada dependa solo del color (RNF-19, §14.2).
6. **Registrar** el asset en `CreditsContent.asset` (T08) con su mención de autoría — obligatoria
   en los personajes, que son obra derivada autorizada (CT-09, RNF-23).
