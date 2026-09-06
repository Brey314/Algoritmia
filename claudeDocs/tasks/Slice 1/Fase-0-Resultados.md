# Fase 0 — Cimientos: lo construido y sus resultados

**Slice 1 · Golden Path temprano** · Cierre de fase: 3 de septiembre de 2026
Plan técnico: [`plan.md`](plan.md) · Tablero: [`todo.md`](todo.md) · Contrato: `claudeDocs/SPEC.md`

Este documento registra qué quedó implementado en la Fase 0, con qué pruebas se verificó y qué
resultado dieron al correrlas. No reabre decisiones de `SPEC.md`: las cita.

---

## 1. Alcance de la fase

La Fase 0 no produce nada jugable. Levanta los cimientos sobre los que se apoyan las tres fases
siguientes del slice: la separación en assemblies, el perfil persistente, la máquina de estados y
los dos objetos que sobreviven al cambio de escena.

| Tarea | Qué entrega | Modo de prueba |
|---|---|---|
| T01 | Carpetas y cinco `.asmdef` de runtime con sus pares de pruebas | EditMode |
| T02 | `PlayerProfile`, `SaveStore`, `IFileSystem` — JSON por perfil en `Datos/` | EditMode |
| T03 | `GameFlow`, la FSM en C# plano, y `GameState` / `LevelId` | EditMode |
| T04 | `SceneLoader`, `GameFlowRunner` y la escena `Boot` | PlayMode |

---

## 2. Qué quedó en el repositorio

### 2.1 Assemblies (T01)

Cinco módulos de runtime bajo `Assets/Game/Scripts/Runtime/`, con dependencias en un solo sentido:

```
Game.Core  ←  Game.Scaffolding
           ←  Game.Levels.Fire
           ←  Game.UI
           ←  Game.Audio
```

`Game.Core` no referencia a nadie. `Game.Scaffolding`, `Game.Levels.Fire`, `Game.UI` y `Game.Audio`
existen con su `.asmdef` pero **todavía sin código**: los crea esta fase para que la regla de
dependencias quede fijada antes de que haya algo que pueda violarla (RNF-15, RNF-16, INC-40).

Cada módulo tiene su assembly de pruebas `<Módulo>.Tests` en `Assets/Tests/EditMode/<Módulo>/`, más
`Game.Core.PlayMode.Tests` en `Assets/Tests/PlayMode/Core/` y `Game.Architecture.Tests`.

### 2.2 Perfil y persistencia (T02)

- **`PlayerProfile`** — nombre o alias, nivel alcanzado y fases confirmadas con sus cuatro
  indicadores. La lista de campos es cerrada (RNF-09, OE1 §3.6.1 nota 5). **No hay campo de
  puntaje**: lo prohíben CP-03 y RF-17, y está anotado en el código como razón pedagógica para que
  una futura «mejora» no lo reintroduzca.
- **`PlayerProfile.Create`** devuelve un `ProfileCreationResult` tipado en lugar de lanzar: nombre
  vacío y nombre duplicado son flujos alternos previstos de HU-01 (FA-01, FA-02), no fallos.
  Se añadió un tercer rechazo, `InvalidName`, para los nombres que no sirven como nombre de archivo
  dentro de `Datos/` — sin él, un nombre con separadores de ruta escribiría fuera de la carpeta
  portable y rompería RNF-07 y RNF-11.
- **`SaveStore`** — un archivo JSON por perfil. Escribe en `Datos/` junto al ejecutable y **no** en
  `Application.persistentDataPath`, porque esa ruta vive en `%AppData%\LocalLow`, fuera de la
  carpeta portable. La ruta del sistema queda solo como respaldo si `Datos/` no es escribible
  (INC-34), y en ese caso `UsingFallback` lo expone para poder advertir al docente.
- **`IFileSystem`** existe para poder probar el guardado sin tocar disco y, sobre todo, para poder
  simular la carpeta no escribible que exige INC-34.

### 2.3 La máquina de estados (T03)

`GameFlow` es C# plano, sin una sola referencia a Unity: por eso se prueba entera en EditMode, sin
escena y sin frames.

- **No existe `GameOver`** ni equivalente en `GameState`. CP-02 prohíbe la pantalla de derrota, el
  límite de intentos y la penalización; la razón está escrita en el enum.
- Una transición ilegal **no lanza**: devuelve `false` y deja el estado como estaba. Un clic a
  destiempo no puede dejar a un estudiante en una pantalla sin salida.
- `Narrative` y `Playing` van parametrizados, así que una escena narrativa nueva es un asset nuevo
  y no un estado nuevo con su rama.

**Desvío respecto al plan, ya registrado en `todo.md`:** `Narrative` se parametriza con el **id** de
la secuencia y no con el `NarrativeSequence` en sí. Ese ScriptableObject vive en
`Game.Scaffolding`, que depende de `Game.Core`; pasarlo por la FSM cerraría un ciclo de assemblies.
Quien resuelve el id a asset es el adaptador, en T09/T10.

### 2.4 Los adaptadores y la escena `Boot` (T04)

- **`SceneLoader`** y **`GameFlowRunner`** son dos de los tres únicos objetos con
  `DontDestroyOnLoad` del proyecto. Ambos descartan la copia en `Awake` si ya hay una instancia
  viva, que es lo que exige RNF-16 al volver a una escena ya visitada.
- **`GameFlowRunner` no decide nada**: pregunta a `GameFlow` si la transición es legal y, si lo es,
  traduce el estado a una escena. Si una regla se colara en el adaptador dejaría de ser verificable
  en EditMode.
- **`Boot`** no es una pantalla: un único GameObject `Persistent` con los dos componentes, y un
  `Start` que pasa el control a `MainMenu` (RF-01). `MainMenu` está vacía a propósito — la llena T05.
- `SceneLoader.LastLoadSeconds` deja anotado cuánto tardó la última carga y registra un aviso por
  encima de los diez segundos de RNF-04. **Corregido el 06/09/2026** (ver §3.4): la primera versión
  cronometraba `SceneManager.LoadScene`, que solo encola la carga, así que el campo reportaba ≈ 0 s
  y no era una medición. Ahora usa `LoadSceneAsync` y cierra el cronómetro en `completed`.

---

## 3. Resultados de las pruebas

**Cómo se corrieron.** El puente Rider↔Unity no conectaba (falta `Library/ProtocolInstance.json`:
el editor externo configurado en Unity no es Rider), así que las pruebas se corrieron con el
corredor por línea de comandos del propio Unity:

```
Unity.exe -runTests -batchmode -projectPath "C:\Users\benab\My project" \
          -testPlatform {EditMode|PlayMode} -testResults <salida>.xml
```

Los resultados de abajo salen del XML de NUnit que produce esa corrida, no de una estimación.

### 3.1 EditMode — 23/23 pasan

| Suite | Pruebas | Resultado |
|---|---|---|
| `Game.Architecture.Tests.AssemblyDependencyTest` | 4 | ✅ |
| `Game.Core.Tests.GameFlowTests` | 7 | ✅ |
| `Game.Core.Tests.PlayerProfileTests` | 7 | ✅ |
| `Game.Core.Tests.SaveStoreTests` | 5 | ✅ |
| **Total** | **23** | **✅ 23 / 0 fallos** |

Detalle por requisito:

| Prueba | Requisito |
|---|---|
| `Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas` | RNF-15 |
| `Architecture_RNF15_ElNombreDelAssemblyCoincideConSuRutaBajoScripts` | RNF-15 |
| `Architecture_RNF16_CoreNoDependeDeUINiDeAudioNiDeNiveles` | RNF-16 |
| `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel` | RNF-16 |
| `GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable` | RNF-13 |
| `GameFlow_CP02_NoExisteEstadoDeDerrota` | CP-02 |
| `GameFlow_RF03_NoPermiteEntrarANivelBloqueado` | RF-03 |
| `GameFlow_RF05_NarrativeSeParametrizaConLaSecuenciaYPlayingConNivelYFase` | RF-05 |
| `GameFlow_RF07_UnaTransicionIlegalNoCambiaDeEstadoYSeObserva` | RF-07 |
| `GameFlow_RF07_ReiniciarElNivelVuelveAPlayingSinPasarPorNingunaDerrota` | RF-07, CP-02 |
| `GameFlow_RF08_LosCreditosSeAlcanzanDesdeElInicioYVuelvenAEl` | RF-08 |
| `PlayerProfile_RF02_RechazaNombreVacioYDuplicado` | RF-02, HU-01 FA-01/FA-02 |
| `PlayerProfile_RF02_RechazaUnNombreQueNoSirveComoNombreDeArchivo` | RF-02, RNF-07 |
| `PlayerProfile_RF03_AlcanzarUnNivelNuncaRetrocede` | RF-03, RF-41 |
| `PlayerProfile_RF04_ConfirmarUnaFaseGuardaSusCuatroIndicadores` | RF-04, RF-45 |
| `PlayerProfile_RF41_UnaFaseConfirmadaNoSePierdeAlVolverAJugarla` | RF-41 |
| `PlayerProfile_HU01_ElPerfilNuevoEmpiezaSinFasesYSoloAlcanzaElNivel1` | HU-01 FA-03 |
| `PlayerProfile_CP03_NoExponeNingunMiembroDePuntaje` | CP-03, RF-17 |
| `SaveStore_RF04_GuardaYRecuperaElPerfilCompleto` | RF-04, RNF-14 |
| `SaveStore_RF02_ListaLosPerfilesExistentesParaDetectarDuplicados` | RF-02 |
| `SaveStore_RNF07_EscribeDentroDeDatosJuntoAlEjecutableCuandoSePuede` | RNF-07, RNF-11 |
| `SaveStore_RNF09_NoPersisteCampoAlgunoFueraDeLaListaCerrada` | RNF-09 |
| `SaveStore_INC34_CaeALaRutaDeRespaldoSiDatosNoEsEscribible` | INC-34 |

### 3.2 PlayMode — 3/3 pasan, tras corregir dos defectos de la prueba

Estas eran las que quedaron sin correr al cerrar la sesión anterior. **La primera corrida falló**, y
el fallo era real: no en el código de producción, sino en cómo esperaban las pruebas.

| Prueba | 1ª corrida | Final |
|---|---|---|
| `BootFlow_RF01_ArrancaEnBootYLlegaSoloAMainMenu` | ❌ | ✅ |
| `GameFlowRunner_RNF16_NoDecideTransicionesLasDelegaEnGameFlow` | ✅ (falso positivo) | ✅ |
| `GameFlowRunner_RNF16_NoSeDuplicaAlRecargarEscena` | ✅ | ✅ |

**Defecto 1 — se esperaba el estado y se afirmaba la escena.** Las pruebas esperaban a que
`GameFlow.Current` llegara a `MainMenu` y acto seguido afirmaban que la escena activa era
`MainMenu`. Pero `SceneManager.LoadScene` se aplica **al final del frame**: la FSM llega un frame
antes que la escena, así que la aserción corría con `Boot` todavía activa. Corregido esperando la
escena —el efecto diferido— y afirmando el estado.

**Defecto 2 — las pruebas se contaminaban entre sí.** `GameFlowRunner` y `SceneLoader` sobreviven
al cambio de escena por diseño, y también sobrevivían de una prueba a la siguiente. La segunda y la
tercera arrancaban con el flujo ya en `MainMenu`, de modo que su espera se cumplía de entrada y
pasaban sin haber probado nada. Añadido un `[TearDown]` que destruye los objetos persistentes; al
hacerlo, la tercera prueba dejó de pasar por accidente y expuso el mismo defecto 1, que se corrigió
igual.

Vale la pena dejarlo escrito: **el `[TearDown]` convirtió un falso positivo en un fallo visible.**
Sin él, la Fase 0 se habría cerrado con una prueba verde que no probaba nada.

**Defecto 3 — ocho warnings `CS0618`.** `Object.FindObjectsByType<T>(FindObjectsSortMode)` está
obsoleta en Unity 6000.5. Sustituida por la sobrecarga con `FindObjectsInactive`.

Ningún cambio tocó código de producción: los tres defectos estaban en
`Assets/Tests/PlayMode/Core/BootFlowTests.cs`.

### 3.3 Compilación

Corrida final: **0 errores, 0 warnings** de compilador en ambos modos.

### 3.4 Corrección de `SceneLoader` — RNF-04 (06/09/2026)

Al revisar el Checkpoint A se encontró que `SceneLoader.LastLoadSeconds` **no medía nada**. El
método cronometraba con un `Stopwatch` la llamada a `SceneManager.LoadScene(sceneName)`, pero esa
llamada solo **encola** la carga —se aplica al final del frame—, así que el cronómetro medía el
encolado y no la carga. RNF-04 exige una medición, no una estimación (T04, §4.1 abajo).

Flujo test-first (`unity test`, Editor cerrado):

| Paso | Resultado |
|---|---|
| Test de reproducción `SceneLoader_RNF04_LastLoadSecondsMideElTiempoRealDeCargaDeLaEscena` en `Assets/Tests/PlayMode/Core/SceneLoaderTests.cs` | RED — `Expected: greater than 0.001 ... But was: 0.00034530001f` (la carga real de `MainMenu` tardó 0,76 s; el campo reportó 0,0003 s) |
| Fix en `SceneLoader.Load`: `SceneManager.LoadSceneAsync` + cerrar el cronómetro en `operation.completed`, con comentario «por qué no» | — |
| Re-corrida | GREEN — **PlayMode 4/4, EditMode 23/23**, 0 warnings |

`GameFlowRunner` no cambia: sigue llamando a `Load` y devolviendo; la escena se activa unos frames
después, como ya ocurría. Los `BootFlowTests` esperan la escena por condición, no por conteo de
frames, así que absorben el cambio sin tocarse.

---

## 4. Checkpoint A — estado

| Criterio | Estado |
|---|---|
| Compila sin errores ni warnings nuevos | ✅ 0 / 0 (re-verificado 06/09 con `unity test`) |
| Pruebas EditMode de Core corridas y declaradas | ✅ 23/23 (re-verificado 06/09) |
| Pruebas PlayMode corridas y declaradas | ✅ 4/4 (06/09, incluye el test nuevo de RNF-04) |
| Arranca en `Boot` y llega a `MainMenu` | ✅ `BootFlow_RF01_…` pasa |
| Mecanismo de medición de RNF-04 correcto | ✅ corregido y verificado 06/09 (§3.4) |
| Cifra de RNF-04 sobre config vinculante anotada | ✅ trasladada al Checkpoint D (build portable, equipo de referencia) |
| Revisado con el usuario | ✅ 06/09/2026 |

**Checkpoint A cerrado el 06/09/2026. Se abre la Fase 1.**

### Notas que se llevan a fases posteriores

1. **Cifra de RNF-04.** El mecanismo ya mide de verdad (§3.4). La única cifra de hoy es de batch
   mode: `MainMenu` cargó en **≈ 0,76 s**, 13× bajo el presupuesto de 10 s. La medición
   **vinculante** —ejecutable portable en el equipo de referencia— es del Checkpoint D
   (`plan.md` §Checkpoint D, RNF-04 + RNF-05).
2. **Herramienta de pruebas.** Rider 2026.2 instalado el 06/09/2026; plugin «MCP Server Extension
   for Unity» (id 30357) instalado. Falta reiniciar Claude Code para que la sesión vea
   `mcp__rider__run_unity_tests` y correr contra el Editor abierto; `unity test` (CLI, Editor
   cerrado) queda como respaldo y para CI.

---

## 5. Trazabilidad

Requisitos con al menos una prueba que los nombra al cerrar la Fase 0 (CT-10):

RF-01, RF-02, RF-03, RF-04, RF-05, RF-07, RF-08, RF-41 · CP-02, CP-03 ·
RNF-04, RNF-07, RNF-09, RNF-11, RNF-13, RNF-15, RNF-16 · HU-01 · INC-34.

RNF-04 lo nombra `SceneLoader_RNF04_LastLoadSecondsMideElTiempoRealDeCargaDeLaEscena` (06/09, §3.4);
la cifra sobre la config vinculante sigue siendo del Checkpoint D.

Declarado por el plan de Fase 0 pero **sin prueba todavía**: RNF-14 (la ida y vuelta está probada
en EditMode; el cierre y reapertura reales son de Checkpoint B).
