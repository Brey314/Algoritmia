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
  encima de los diez segundos de RNF-04.

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

---

## 4. Checkpoint A — estado

| Criterio | Estado |
|---|---|
| Compila sin errores ni warnings nuevos | ✅ 0 / 0 en la corrida final |
| Pruebas EditMode de Core corridas y declaradas | ✅ 23/23 |
| Arranca en `Boot` y llega a `MainMenu` | ✅ `BootFlow_RF01_…` pasa |
| Revisado con el usuario | ⏳ pendiente |

### Lo que queda pendiente de esta fase

1. **Medición de RNF-04.** T04 pide anotar —no estimar— el tiempo de carga de `Boot` y `MainMenu`.
   La corrida en batch mode **no sirve** como medición: va sin ventana de juego y en el equipo de
   desarrollo, no en el de referencia. `SceneLoader.LastLoadSeconds` ya deja el dato listo; falta
   tomarlo con el Editor abierto o sobre el ejecutable portable, y escribirlo aquí.
2. **Revisión con el usuario** antes de abrir la Fase 1.
3. **El puente Rider↔Unity sigue caído.** Mientras el editor externo de Unity no sea Rider, ni
   `mcp__rider__run_unity_tests` ni `mcp__rider__get_unity_compilation_result` responden. La ruta por
   línea de comandos funciona y queda documentada arriba, pero cierra el Editor mientras corre.

---

## 5. Trazabilidad

Requisitos con al menos una prueba que los nombra al cerrar la Fase 0 (CT-10):

RF-01, RF-02, RF-03, RF-04, RF-05, RF-07, RF-08, RF-41 · CP-02, CP-03 ·
RNF-07, RNF-09, RNF-11, RNF-13, RNF-15, RNF-16 · HU-01 · INC-34.

Declarados por el plan de Fase 0 pero **sin prueba todavía**: RNF-04 (pendiente de medición, §4)
y RNF-14 (la ida y vuelta está probada en EditMode; el cierre y reapertura reales son de Checkpoint B).
