# Plan técnico — Slice 2: La Rueda

Contrato de referencia: `claudeDocs/SPEC.md`. Este plan no rediscute arquitectura ni alcance:
los aplica. Cuando algo aquí contradiga a `SPEC.md`, gana `SPEC.md`.

Plan del slice anterior: [`../Slice 1/plan.md`](../Slice%201/plan.md). Tablero de este slice:
[`todo.md`](todo.md).

**Rev. 1 — 30/08/2026.**

> ⚠️ **Precondición de slice.** El Slice 1 aún no tiene ninguna tarea cerrada y `Assets/` solo
> contiene la escena por defecto. Este plan **supone terminados `T01`..`T18` del Slice 1**:
> assemblies, `SaveStore`, `GameFlow`, `SceneLoader`, `DialogueRunner`, `HintPolicy`,
> `ILevelReporter`, menú de pausa y `LevelSummary`. Cada tarea de abajo dice qué pieza del
> Slice 1 generaliza. No es un plan ejecutable antes de cerrar el Checkpoint D del Slice 1.

---

## Alcance

El Nivel 2 completo —las tres fases encadenadas, cada una con su escenario— más lo que le falta
a `sistema-navegacion` y a `andamiaje` cuando dejan de servir a un solo nivel:

| Módulo | Qué entra en este slice | Qué NO entra |
|---|---|---|
| `nivel-rueda` (D) | Completo: bosque (selección por patrón), taller (ensamblaje secuencial), laberinto (editor de bloques con lectura relativa), y los tres escenarios | — |
| `sistema-navegacion` (A) | Desbloqueo secuencial real del Nivel 2, modelo de fase generalizado (`PhaseId`), guardado por fase en un nivel de tres fases, pausa y reinicio sobre las tres escenas | Informe docente, eliminación de datos (Slice 4) |
| `andamiaje` (B) | Ayuda a demanda y pista automática **por fase**, no por nivel; seis secuencias narrativas del Nivel 2; cierre reflexivo del §6.4 | Ayuda contextual del Nivel 3 |
| `progreso-registro` (F) | Solo la **emisión** de los cuatro indicadores del Nivel 2 vía `ILevelReporter`, con su definición por fase (OE1 §3.6.1) | Agregación y presentación docente (Slice 4) |

**Fuera de alcance explícito:** `nivel-rio`, `TeacherReport`, RF-46, RF-47, y el nivel avanzado
opcional del guion §10 (introduce presión de tiempo, contradice CP-02).

**Requerimientos que este slice cierra:** RF-22..RF-34, todos de prioridad **Alta**. Generaliza
además RF-03, RF-04, RF-05, RF-06, RF-07, RF-10, RF-11, RF-12, RF-13 y RF-45 más allá del
Nivel 1, y es donde por primera vez se puede verificar RNF-16 (prueba de exclusión con dos
niveles reales) y RNF-19 (cuyo criterio de verificación es, literalmente, «inspección de los
estados de error de los niveles 2 y 3»).

---

## Decisiones ya tomadas que este plan aplica

Ninguna se rediscute; se listan para que las tareas no las reinventen.

- Assembly nuevo: **`Game.Levels.Wheel`, que depende solo de `Game.Core`.** No referencia a
  `Game.Levels.Fire` ni al revés — esa es la condición de RNF-16.
- FSM `GameFlow` en C# plano, ya existente. El Nivel 2 **no añade estados**: es `Playing` con
  `LevelId.Wheel` + una de tres fases. Añadir un nivel no toca el enum.
- Escena `Narrative` única y parametrizada. Las seis escenas narrativas del Nivel 2 son
  **seis assets**, no seis escenas ni seis ramas.
- Todo texto visible y todo parámetro ajustable jugando vive en ScriptableObject (CT-05, RNF-18).
- Entrada limitada a **clic y clic sostenido** (CT-06, RNF-02). El Nivel 2 usa las dos: clic para
  seleccionar y para soltar, clic sostenido para arrastrar. El botón «Ejecutar» es **clic simple**
  (PG-04 cerrado, RF-32) — el documento fuente decía doble clic y quedó normalizado.
- **Los bloques del laberinto son relativos** a la orientación de la carretilla: «Avanzar» y
  «Retroceder» la mueven una casilla adelante o atrás según hacia dónde mire, «Girar» la rota 90°
  en sentido horario (RF-31, guion §6.3.2, INC-33 cerrado, supuesto 8). **Nunca lectura absoluta
  de dirección**: con ella el refugio podía ser inalcanzable.
- La lógica de cada fase es C# plano probable en EditMode; el MonoBehaviour solo traduce clics.
- **El Nivel 2 no tiene lista de tareas visible** — esa es del Nivel 3 (RF-36, INC-41). RNF-03
  restringe la tarea *activa*: una por fase.
- Nada de `GameOver`, puntajes, cifras al estudiante ni pérdida de fase confirmada.

---

## Grafo de dependencias

```
                         W01 Game.Levels.Wheel + exclusión
                                      │
                    ┌─────────────────┴──────────────────┐
                    │                                    │
        W02 PhaseId + desbloqueo N2              W10 MazeGrid + CartState
        (RF-03, RF-04)                           (RF-31, INC-33)   ◄── adelantable
                    │                                    │
        W03 HintPolicy por fase                 W11 BlockSequence
        (RF-13)                                 (RF-31, RF-34)
                    │                                    │
        W04 Seis NarrativeSequence del N2       W12 SequenceExecutor + retroceso
        (RF-05, RF-06, RF-10)                   (RF-32, RF-33)
                    │                                    │
   ┌────────────────┴────────────────┐                   │
   │                                 │                   │
W05 PatternSelection (puro)   W08 AssemblySequence (puro)│
(RF-23, RF-24)                (RF-28, RF-29)             │
   │                                 │                   │
W06 Level2_Forest             W09 Level2_Workshop        │
(RF-22)                       (RF-27)                    │
   │                                 │                   │
W07 Caja + Empujar                   │            W13 Level2_Maze + editor
(RF-25, RF-26)                       │            (RF-30, RF-31, RF-32)
   │                                 │                   │
   └─────────────────┬───────────────┘                   │
                     │                            W14 Reintento sin reinicio
                     │                            (RF-34)
                     └──────────────┬─────────────────────┘
                                    │
        ┌───────────────────────────┼───────────────────────────┐
        │                           │                           │
  W15 Indicadores N2         W17 Pausa en 3 fases        W18 RNF-19 + RNF-20
  (RF-45, §3.6.1)            (RF-07, INC-25)             en estados de error
        │                           │                           │
        └───────────────────────────┴───────────────────────────┘
                                    │
                     W16 Resumen, cierre reflexivo y desbloqueo N3
                            (RF-12, RF-45, RF-03)
```

El orden dentro de cada fase es de abajo hacia arriba: primero la lógica pura probable sin
escena, después el cableado. **Cada tarea deja el proyecto compilando y jugable hasta donde
llegó.**

**W10 no depende de W02..W09.** Es la tarea de mayor riesgo del slice (la semántica relativa de
INC-33 es donde se equivoca cualquiera) y solo necesita W01. Conviene adelantarla y verla pasar
en EditMode antes de construir el bosque, aunque el orden de juego la ponga al final.

---

## Convenciones de las tareas

Idénticas a las del Slice 1, se repiten para no obligar a saltar de archivo.

- **Modo de prueba:** `EditMode` = lógica pura, sin escena ni frames. `PlayMode` = cableado, UI,
  integración, Golden Path; lleva `[Category("Integration")]`. `VV` = `[Category("VisualVerification")]`.
- **Trazabilidad (CT-10):** el nombre del método de prueba cita el identificador. Ejemplo:
  `CartState_RF31_AvanzarEsRelativoALaOrientacion`.
- **Tamaño:** XS = 1 archivo · S = 1-2 · M = 3-5. Ninguna tarea de este plan supera M.
- **Corredor de pruebas (R1):** cada tarea declara si su verificación **exige** el servidor MCP de
  Unity conectado o si se puede correr a mano en el Test Runner sin perder rigor. Las tareas
  EditMode se sostienen a mano; las PlayMode y las de verificación visual son las que se vuelven
  caras sin él.
- **Flujo test-first por tarea:** `test-designer` → `failing-test-writer` → ver fallar →
  implementar → `resolve-diagnostics` → deduplicar.

---

# Fase 0 — Cimientos del slice

## W01: Assembly `Game.Levels.Wheel` y prueba de exclusión real

**Descripción.** Crear `Assets/Game/Scripts/Runtime/Levels/Wheel/` con su `.asmdef`
`Game.Levels.Wheel` (referencia única: `Game.Core`) y el assembly de pruebas correspondiente.
Con dos niveles en el proyecto, la prueba de exclusión de RNF-16 deja de ser declarativa y pasa a
verificar algo: retirar un nivel y comprobar que el otro sigue ejecutándose.

**Traza:** RNF-15, RNF-16, INC-40, `SPEC.md` §Estructura del proyecto, §Mapa de capacidades.

**Modo de prueba:** EditMode (prueba de arquitectura sobre referencias de assembly).
**Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Existe `Game.Levels.Wheel` y su assembly de pruebas.
- [ ] `Game.Levels.Wheel` **no** referencia a `Game.Levels.Fire`, ni `Fire` a `Wheel`.
- [ ] `Game.Core` no referencia a ningún assembly de nivel.
- [ ] El namespace es `Game.Levels.Wheel`, siguiendo la ruta bajo `Scripts/` y elidiendo `Runtime`.

**Verificación**
- [ ] EditMode: `Architecture_RNF16_NingunNivelReferenciaAOtroNivel` (ahora con dos niveles reales).
- [ ] `mcp__coplay-mcp__check_compile_errors` → sin errores.
- [ ] Ningún `.meta` escrito a mano.

**Depende de:** Slice 1 T01 · **Tamaño:** XS

**Archivos**
- `Assets/Game/Scripts/Runtime/Levels/Wheel/Game.Levels.Wheel.asmdef`
- `Assets/Tests/EditMode/Levels/Wheel/Game.Levels.Wheel.Tests.asmdef`
- `Assets/Tests/EditMode/Architecture/AssemblyDependencyTests.cs` (ampliar)

---

## W02: `PhaseId` y desbloqueo secuencial del Nivel 2

**Descripción.** El Slice 1 guardó «fases confirmadas» contra un nivel de una sola fase. El
Nivel 2 tiene tres, y es donde el modelo de progreso se pone a prueba de verdad. Generalizar
`PlayerProfile`/`SaveStore` a un `PhaseId` (`LevelId` + índice de fase), y hacer que `LevelSelect`
habilite el Nivel 2 solo cuando el Nivel 1 esté completo para el perfil activo.

**Traza:** RF-03, RF-04, RNF-09, RNF-14, HU-14, CU-06 (precondición), INC-27, supuestos 2 y 9.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] El Nivel 2 está bloqueado mientras el Nivel 1 no esté completo, y se habilita al completarlo.
- [ ] Completar la fase 1 del Nivel 2 guarda y sobrevive a un cierre; al reabrir se retoma en la
      fase 2, no al principio del nivel (RNF-14).
- [ ] Una fase confirmada **nunca** se desconfirma: ni por fallar después, ni por reiniciar el
      nivel. Comentario «por qué no» en el código: la razón es CP-02, no técnica.
- [ ] El perfil sigue sin campo de puntaje y sin dato alguno fuera de la lista cerrada (RNF-09).

**Verificación**
- [ ] EditMode: `LevelSelect_RF03_Nivel2BloqueadoHastaCompletarNivel1`,
      `SaveStore_RF04_ConfirmarFase1DelNivel2SobreviveAlCierre`,
      `PlayerProfile_CP02_UnaFaseConfirmadaNoSePierdeNunca`,
      `SaveStore_RNF09_NoPersisteCampoAlgunoFueraDeLaListaCerrada` (ampliar con tres fases).

**Depende de:** W01, Slice 1 T02/T07 · **Tamaño:** M

**Archivos**
- `.../Core/PhaseId.cs`, `.../Core/PlayerProfile.cs` (ampliar), `.../Core/SaveStore.cs` (ampliar)
- `Assets/Tests/EditMode/Core/PhaseProgressTests.cs`

---

### ✅ Checkpoint W-A — Cimientos

- [ ] Compila sin errores ni warnings nuevos (`check_compile_errors`).
- [ ] La prueba de exclusión de RNF-16 pasa con dos niveles reales (**corrida a mano — ver R1**).
- [ ] El menú muestra el Nivel 2 habilitado tras completar el Nivel 1, y bloqueado sin él.
- [ ] Revisado con el usuario.

---

# Fase 1 — Andamiaje generalizado (`andamiaje`)

## W03: `HintPolicy` por fase, no por nivel

**Descripción.** El Slice 1 resolvió RF-13 contra una sola tarea. El Nivel 2 tiene tres fases con
tres instrucciones vigentes distintas y tres nociones distintas de «fallo». Generalizar
`HintPolicy` para que reciba la tarea activa y su contador propio, sin que el nivel tenga que
saber cómo funciona el andamiaje.

**Traza:** RF-13, RF-10, RF-11, CP-06, RNF-03, HU-03, HU-04, CU-06, CU-07, CU-08, guion §6.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] La ayuda a demanda repite **la instrucción de la fase vigente** y no muta ningún contador
      (CP-06).
- [ ] La pista automática se dispara al tercer fallo consecutivo **dentro de la fase activa**;
      cambiar de fase reinicia el contador.
- [ ] Ninguna pista del Nivel 2 nombra la respuesta: no dice «redondos» en la fase 1, no dicta el
      orden de ensamblaje en la fase 2, no nombra la secuencia de bloques en la fase 3. Prueba
      explícita por fase.
- [ ] Hay exactamente **una tarea activa** por fase (RNF-03). El Nivel 2 no muestra lista de
      tareas: esa es del Nivel 3 (INC-41).

**Verificación**
- [ ] EditMode: `HintPolicy_RF13_ContadorDeFallosEsPorFaseNoPorNivel`,
      `HintPolicy_CP06_NingunaPistaDelNivel2NombraLaRespuesta`,
      `HintPolicy_RF13_AyudaADemandaNoAlteraElEstadoEnLasTresFases`,
      `WheelLevel_RNF03_UnaSolaTareaActivaPorFase`.

**Depende de:** W02, Slice 1 T11 · **Tamaño:** M

**Archivos**
- `.../Scaffolding/HintPolicy.cs` (generalizar), `.../Scaffolding/GuideContent.cs` (ampliar)
- `Assets/Game/Data/Wheel/N2_GuideContent.asset`
- `Assets/Tests/EditMode/Scaffolding/HintPolicyTests.cs` (ampliar)

---

## W04: Las seis secuencias narrativas del Nivel 2

**Descripción.** Seis `NarrativeSequence` sobre la escena `Narrative` ya existente. **Ni una línea
de código nuevo**: si esta tarea obliga a tocar `DialogueRunner`, el Slice 1 lo dejó mal
parametrizado y eso es lo que hay que corregir. Textos exactos del guion.

| Asset | Guion | Contenido |
|---|---|---|
| `N2_PuenteI` | §5 | Del fuego al alimento; aparece el problema del transporte |
| `N2_Escena21_Bosque` | §6.1.1 | «No todo lo que ven es importante» — objetivo de la fase 1 |
| `N2_Escena22_ElPatron` | §6.1.3 | «Acabas de encontrar un patrón» — cierre de la fase 1 |
| `N2_Escena23_Construccion` | §6.2.1 | Chispa enuncia la secuencia de construcción |
| `N2_Escena24_Regreso` | §6.3.1 | «Escribe antes todos los pasos, y luego los ejecutas» |
| `N2_Escena25_Cierre` | §6.4 | Cierre reflexivo — se consume en W16 |

**Traza:** RF-05, RF-06 (prioridad Media), RF-10, RF-12, RNF-01, RNF-18, HU-02, INC-28, CP-07.

**Modo de prueba:** EditMode (contenido) + PlayMode (recorrido). **Corredor MCP:** el PlayMode lo
agradece; el EditMode no lo exige.

**Criterios de aceptación**
- [ ] Las seis se resuelven en la **misma** escena `Narrative`, sin rama nueva en el código.
- [ ] El botón de omitir aparece **solo** si el perfil ya vio esa escena (RF-06, INC-28).
- [ ] `N2_Escena25_Cierre` **no es omitible la primera vez** (CP-07, RF-12).
- [ ] Ninguna oración supera veinte palabras (RNF-01) — prueba automática sobre los seis assets.
- [ ] El texto no desborda su cuadro en la secuencia más larga.
- [ ] El botón de pausa no se muestra en escenas narrativas (HU-17 FA-04).

**Verificación**
- [ ] EditMode: `NarrativeSequence_RNF01_NingunaOracionDelNivel2Supera20Palabras`,
      `NarrativeSequence_RF06_ElCierreDelNivel2NoEsOmitibleLaPrimeraVez`.
- [ ] PlayMode: `NarrativeScene_RF05_ResuelveLasSeisSecuenciasDelNivel2SinRamas`.
- [ ] Aserción de layout sobre el cuadro más largo (§6.4, dos parlamentos de Chispa).

**Depende de:** W03, Slice 1 T09/T10 · **Tamaño:** S (seis assets, cero clases)

**Archivos**
- `Assets/Game/Data/Narrative/N2_*.asset` (6)
- `Assets/Tests/EditMode/Scaffolding/NarrativeContentTests.cs` (ampliar)

---

### ✅ Checkpoint W-B — Andamiaje generalizado

- [ ] Las seis escenas narrativas se recorren de principio a fin.
- [ ] Ninguna pista del Nivel 2 resuelve la tarea (revisión de texto contra CP-06).
- [ ] `DialogueRunner` no necesitó cambios para el Nivel 2 — si los necesitó, anotar por qué.
- [ ] Revisado con el usuario.

---

# Fase 2 — Bosque: selección por patrón (`nivel-rueda`, fase 1)

## W05: `WheelLevelConfig`, `ForestObject` y `PatternSelection`

**Descripción.** La lógica de la fase 1 en C# plano: catálogo de objetos del bosque con su
categoría, validación de la selección por la propiedad «rueda», contador de acopio y el mensaje
narrativo que corresponde a cada categoría de distractor. Sin escena, sin frames.

**Traza:** RF-23, RF-24, RF-11, RF-17, CT-05, RNF-18, RNF-01, HU-08, CU-06, guion §6.1.2.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] `WheelLevelConfig` (ScriptableObject) expone al menos: troncos requeridos (5), mínimo de
      distractores (8) y el reparto por categoría. Ningún literal en el código (CT-05, RNF-18).
- [ ] Un tronco redondo se acepta y suma al contador; un distractor se rechaza, **vuelve a su
      posición** y devuelve el mensaje de su categoría — piedra, planta o herramienta, uno por
      categoría (guion §6.1.2).
- [ ] Seleccionar un tronco correcto devuelve la pregunta «Este rueda. ¿Qué tiene que los otros no
      tienen?» — la retroalimentación del acierto también es narrativa (RF-11).
- [ ] **Nada de cifras en el mensaje**; el contador «Troncos redondos: n de 5» es la única cifra
      permitida de la fase y es estado de tarea, no desempeño (RF-24 lo exige; CP-03 no lo prohíbe
      porque no es un puntaje). Dejarlo escrito en un comentario para que nadie lo «limpie».
- [ ] Rechazar un objeto **no reduce** el contador ni bloquea nada: sin penalización (CP-02, RF-18).
- [ ] Un objeto ya acopiado no se puede volver a seleccionar ni contar dos veces.

**Verificación**
- [ ] EditMode: `PatternSelection_RF23_AceptaTroncoRedondoYRechazaDistractor`,
      `PatternSelection_RF23_CadaCategoriaDeDistractorDevuelveSuPropioMensaje`,
      `PatternSelection_RF24_ElContadorNoRetrocedeAnteUnRechazo`,
      `PatternSelection_CP02_UnRechazoNoPenalizaNiBloquea`,
      `WheelLevelConfig_RNF18_NingunParametroDeLaFase1EstaEnElCodigo`.

**Depende de:** W04 · **Tamaño:** M

**Archivos**
- `.../Levels/Wheel/WheelLevelConfig.cs`, `.../Levels/Wheel/ForestObject.cs`,
  `.../Levels/Wheel/PatternSelection.cs`
- `Assets/Game/Data/Wheel/N2_WheelLevelConfig.asset`
- `Assets/Tests/EditMode/Levels/Wheel/PatternSelectionTests.cs`

---

## W06: Escena `Level2_Forest` y panel de selección

**Descripción.** El escenario de bosque con los objetos dispersos, el contador permanente y el
cableado de clic. El MonoBehaviour traduce clics a `PatternSelection` y estado a UI; no contiene
reglas.

**Traza:** RF-22, RF-23, RF-24, RF-10, RNF-02, RNF-03, RNF-19, CT-06, HU-08, CU-06, guion §6.1.2.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.
**Corredor MCP:** **sí lo exige** para automatizarse; sin él, recorrido manual declarado.

**Criterios de aceptación**
- [ ] El escenario presenta cinco troncos redondos válidos entre al menos ocho distractores de las
      tres categorías (RF-22).
- [ ] La selección es **clic simple** y nada más (RNF-02, CT-06) — inspección del mapa de controles.
- [ ] El contador «Troncos redondos: n de 5» es visible de forma permanente (RF-24).
- [ ] El rechazo se señala con **color más un segundo indicador** (icono o forma), no solo color
      (RNF-19).
- [ ] El botón de ayuda a demanda está visible durante toda la fase (RF-13).
- [ ] Ningún elemento se sale de pantalla ni se solapa; el texto no desborda.

**Verificación**
- [ ] PlayMode: `ForestScene_RF22_PresentaCincoValidosEntreOchoDistractores`,
      `ForestScene_RF24_ElContadorEsVisibleDuranteTodaLaFase`,
      `ForestScene_RNF02_ElMapaDeControlesSoloTieneClic`.
- [ ] Aserción de layout: contador, botón de ayuda y objetos alcanzables por raycast.

**Depende de:** W05 · **Tamaño:** M

**Archivos**
- `.../Levels/Wheel/ForestSceneController.cs`
- `Assets/Game/Scenes/Level2_Forest.unity`
- `Assets/Tests/PlayMode/Levels/Wheel/ForestSceneTests.cs`

---

## W07: Colocación de la carga y demostración del rodado

**Descripción.** Al reunir los cinco troncos se habilita el arrastre de la caja de alimentos:
clic sostenido para arrastrarla sobre los troncos alineados, clic para soltarla. Con la caja
colocada se habilita «Empujar», que reproduce el rodado y cierra la fase 1.

**Traza:** RF-25, RF-26, RF-04, RNF-02, CT-06, HU-08, CU-06 (FA 4a), guion §6.1.2.

**Modo de prueba:** PlayMode `[Category("Integration")]`.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] El arrastre de la caja está deshabilitado hasta los cinco troncos; intentarlo antes informa
      **cuántos faltan** y no ejecuta nada (CU-06 FA-4a).
- [ ] El arrastre es **clic sostenido** y el soltar es **clic** — exactamente el esquema de
      RNF-02, sin excepción.
- [ ] «Empujar» se habilita solo con la caja colocada y, una vez habilitado, **no vuelve a
      deshabilitarse**: lo ganado permanece (CP-02, mismo criterio que INC-32 en el Nivel 1).
- [ ] Al accionarlo, la caja se desplaza sobre los troncos que giran bajo ella (RF-26) y la fase 1
      queda confirmada y guardada (RF-04).
- [ ] La animación no incluye parpadeos ni destellos de alta frecuencia (RNF-21).

**Verificación**
- [ ] PlayMode: `ForestScene_RF25_LaCajaNoSeArrastraSinLosCincoTroncos`,
      `ForestScene_RF26_EmpujarSeHabilitaSoloConLaCajaColocada`,
      `ForestScene_RF04_ConfirmarLaFase1GuardaElProgreso`.
- [ ] VisualVerification: `ForestScene_RNF21_LaAnimacionDelRodadoNoTieneDestellos`.

**Depende de:** W06 · **Tamaño:** M

**Archivos**
- `.../Levels/Wheel/CargoPlacement.cs`, `.../Levels/Wheel/ForestSceneController.cs` (ampliar)
- `Assets/Tests/PlayMode/Levels/Wheel/CargoPlacementTests.cs`

---

### ✅ Checkpoint W-C — Fase 1 completa

- [ ] El bosque se juega de principio a fin: seleccionar, acopiar cinco, colocar la caja, empujar.
- [ ] Ningún rechazo penaliza, bloquea ni muestra cifra de desempeño (CP-02, CP-03).
- [ ] El estado de error se distingue sin depender del color (RNF-19).
- [ ] Al confirmar la fase 1, un cierre forzado retoma en la fase 2 (RNF-14).
- [ ] Revisado con el usuario.

---

# Fase 3 — Taller: ensamblaje secuencial (`nivel-rueda`, fase 2)

## W08: `AssemblySequence` — la máquina de ensamblaje

**Descripción.** Lógica pura de la fase 2: seis piezas, cuatro pasos con condición de
habilitación, y el rechazo que **dice qué falta antes** en vez de qué está mal. El orden es
perforar las dos ruedas → insertar el eje → colocar la tabla → colocar la caja.

**Traza:** RF-28, RF-29, RF-11, RF-17, CP-06, RNF-18, HU-09, CU-07, guion §6.2.2.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] «Mecanizar» está habilitado **únicamente** con un tronco corto seleccionado (RF-28, CU-07 FA-3a).
- [ ] El eje solo entra con las **dos** ruedas perforadas; la tabla solo con el eje formado; la
      caja solo con la tabla colocada (RF-29).
- [ ] Un paso fuera de secuencia **no se ejecuta**, devuelve la pieza a su posición y da el mensaje
      exacto del guion §6.2.2: «El tronco todavía no tiene por dónde entrar el palo.» / «La tabla
      no tiene sobre qué apoyarse todavía.» / «La caja se caería. Falta algo plano debajo.»
- [ ] El mensaje dice **qué falta antes**, nunca cuál es el paso correcto: orienta sin resolver
      (CP-06).
- [ ] Un rechazo no deshace ningún paso ya completado (CP-02): perforar una rueda no se pierde
      por intentar mal el eje después.
- [ ] Ningún mensaje lleva cifras ni juicio de valor (RF-17, CP-03).

**Verificación**
- [ ] EditMode: `AssemblySequence_RF28_MecanizarExigeTroncoCortoSeleccionado`,
      `AssemblySequence_RF29_RechazaCadaPasoFueraDeSecuenciaConSuMensaje`,
      `AssemblySequence_CP02_UnPasoFueraDeOrdenNoDeshaceLoYaHecho`,
      `AssemblySequence_CP06_ElMensajeDiceQueFaltaAntesNoCualEsElPasoCorrecto`,
      `AssemblySequence_RF17_NingunMensajeContieneDigitos`.

**Depende de:** W07 · **Tamaño:** M

**Archivos**
- `.../Levels/Wheel/AssemblyStep.cs`, `.../Levels/Wheel/AssemblySequence.cs`,
  `.../Levels/Wheel/WorkshopPiece.cs`
- `Assets/Game/Data/Wheel/N2_AssemblyContent.asset`
- `Assets/Tests/EditMode/Levels/Wheel/AssemblySequenceTests.cs`

---

## W09: Escena `Level2_Workshop` y cableado del ensamblaje

**Descripción.** El área de trabajo con las seis piezas, el botón «Mecanizar» y los tres arrastres
(eje, tabla, caja). Al completarse, animación de carretilla terminada y confirmación de fase.

**Traza:** RF-27, RF-28, RF-29, RF-04, RF-10, RNF-02, RNF-03, RNF-19, CT-06, HU-09, CU-07.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] La escena presenta exactamente las seis piezas de RF-27: dos troncos cortos, un tronco largo,
      una tabla, una herramienta y la caja de alimentos.
- [ ] Selección con clic, arrastres con clic sostenido y soltar con clic (RNF-02, CT-06).
- [ ] El botón «Mecanizar» refleja su habilitación de forma visible y **con doble indicador**
      (RNF-19): no basta con atenuar el color.
- [ ] Una tarea activa a la vez (RNF-03). Sin lista de tareas (INC-41).
- [ ] Al completar el ensamblaje se reproduce la animación de terminado, se confirma la fase 2 y se
      guarda (RF-29, RF-04).

**Verificación**
- [ ] PlayMode: `WorkshopScene_RF27_PresentaLasSeisPiezas`,
      `WorkshopScene_RF29_LaSecuenciaCompletaConfirmaYGuardaLaFase2`,
      `WorkshopScene_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido`.
- [ ] Aserción de layout sobre las seis piezas y el botón.

**Depende de:** W08 · **Tamaño:** M

**Archivos**
- `.../Levels/Wheel/WorkshopSceneController.cs`
- `Assets/Game/Scenes/Level2_Workshop.unity`
- `Assets/Tests/PlayMode/Levels/Wheel/WorkshopSceneTests.cs`

---

### ✅ Checkpoint W-D — Fase 2 completa

- [ ] El taller se juega entero: perforar, perforar, eje, tabla, caja.
- [ ] Cada intento fuera de orden da el mensaje del guion y no deshace nada.
- [ ] Al confirmar la fase 2, un cierre forzado retoma en la fase 3 (RNF-14).
- [ ] Revisado con el usuario.

---

# Fase 4 — Laberinto: editor de bloques (`nivel-rueda`, fase 3)

> **Esta es la fase de mayor riesgo del slice.** W10, W11 y W12 son C# plano y no dependen de
> W05..W09: pueden adelantarse a la Fase 2 de este plan. Conviene hacerlo.

## W10: `MazeGrid` y `CartState` — la orientación relativa

**Descripción.** El núcleo de INC-33. La rejilla del laberinto con sus obstáculos y el refugio, y
el estado de la carretilla: casilla **y orientación**. «Avanzar» y «Retroceder» se resuelven
contra la orientación actual; «Girar» rota 90° en sentido horario. Nada en esta clase lee una
dirección absoluta.

**Traza:** RF-30, RF-31, RF-33, INC-33 (cerrado), supuesto 8, CT-05, RNF-18, guion §6.3.2, CU-08.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] `CartState` guarda casilla **y** orientación. «Avanzar» desde orientaciones distintas produce
      desplazamientos distintos desde la misma casilla — prueba directa contra la lectura absoluta.
- [ ] «Girar» rota exactamente 90° **en sentido horario**; cuatro giros devuelven la orientación
      inicial y **no mueven** la carretilla.
- [ ] «Retroceder» mueve una casilla atrás respecto de la orientación **sin cambiarla**.
- [ ] Un movimiento hacia un obstáculo o fuera de la rejilla se **intenta** y devuelve a la casilla
      anterior; uno válido deja la carretilla en la casilla nueva (RF-33).
- [ ] El trazado del laberinto (rejilla, obstáculos, salida, refugio, orientación inicial) vive en
      un ScriptableObject, no en el código (CT-05, RNF-18).
- [ ] Existe al menos una secuencia que alcanza el refugio desde el estado inicial —el laberinto es
      resoluble— y la prueba la ejecuta.

**Verificación**
- [ ] EditMode: `CartState_RF31_AvanzarEsRelativoALaOrientacionNoAbsoluto`,
      `CartState_INC33_GirarRota90GradosHorarioSinDesplazar`,
      `CartState_RF31_RetrocederNoCambiaLaOrientacion`,
      `MazeGrid_RF33_UnMovimientoInvalidoDevuelveALaCasillaAnterior`,
      `MazeGrid_RNF13_ExisteUnaSecuenciaQueAlcanzaElRefugio`.

**Depende de:** W01 · **Tamaño:** M

**Archivos**
- `.../Levels/Wheel/Orientation.cs`, `.../Levels/Wheel/CartState.cs`, `.../Levels/Wheel/MazeGrid.cs`,
  `.../Levels/Wheel/MazeLayout.cs` (SO)
- `Assets/Game/Data/Wheel/N2_MazeLayout.asset`
- `Assets/Tests/EditMode/Levels/Wheel/MazeGridTests.cs`

---

## W11: `BlockSequence` — composición y edición

**Descripción.** La secuencia de bloques como dato: añadir, retirar, reordenar. Sin UI y sin
ejecución — solo la estructura y sus invariantes. Es lo que hace verificable RF-34 sin escena.

**Traza:** RF-31, RF-34, HU-10, CU-08 (FA-3a, FA-6a), guion §6.3.2.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Tres tipos de bloque y solo tres: `Avanzar`, `Retroceder`, `Girar` (RF-31).
- [ ] Los bloques se encadenan en el orden en que se sueltan; retirar y reubicar preserva el resto
      de la secuencia (RF-34).
- [ ] Editar la secuencia **no reinicia el nivel ni descarta el escenario** (CU-08 FA-6a) — la
      secuencia y el laberinto son estados separados.
- [ ] Ejecutar una secuencia vacía no lanza: devuelve un resultado tipado que la UI traduce a
      «añade al menos un bloque» (CU-08 FA-3a).
- [ ] No hay límite al número de bloques ni al número de ediciones (CP-02, RF-18).

**Verificación**
- [ ] EditMode: `BlockSequence_RF31_SoloExistenTresTiposDeBloque`,
      `BlockSequence_RF34_RetirarYReordenarPreservaElRestoDeLaSecuencia`,
      `BlockSequence_CU08_UnaSecuenciaVaciaDevuelveResultadoTipadoNoExcepcion`,
      `BlockSequence_CP02_NoHayLimiteDeBloquesNiDeEdiciones`.

**Depende de:** W10 · **Tamaño:** S

**Archivos**
- `.../Levels/Wheel/InstructionBlock.cs`, `.../Levels/Wheel/BlockSequence.cs`
- `Assets/Tests/EditMode/Levels/Wheel/BlockSequenceTests.cs`

---

## W12: `SequenceExecutor` — recorrido paso a paso y validación por retroceso

**Descripción.** Recorre la secuencia bloque a bloque sobre `MazeGrid`, emitiendo qué bloque está
en ejecución y qué le pasó a la carretilla. **El retroceso es el mecanismo de depuración del
nivel**: no dice qué bloque está mal, muestra dónde se detuvo el avance. Es el jugador quien
deduce cuál falló — eso es CP-06 y hay que dejarlo comentado en el código.

**Traza:** RF-32, RF-33, RF-34, RF-11, CP-06, HU-10, CU-08, guion §6.3.2.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] La ejecución recorre la secuencia **paso a paso** y emite el índice del bloque en curso
      (RF-32); la pausa entre pasos es un parámetro del SO, no un literal.
- [ ] Ante un movimiento inválido la carretilla lo intenta, vuelve a la casilla anterior y **la
      ejecución continúa** con el siguiente bloque: no se aborta, no se penaliza (RF-33, CP-02).
- [ ] El resultado expone **en qué paso** se detuvo el avance, y **nunca** cuál bloque corregir
      (CP-06). Prueba explícita de que el resultado no contiene una recomendación de bloque.
- [ ] Alcanzar la casilla del refugio termina la fase con éxito (RF-32).
- [ ] Ninguna cifra de desempeño llega al resultado que consume la UI del estudiante (CP-03).

**Verificación**
- [ ] EditMode: `SequenceExecutor_RF32_RecorreLaSecuenciaPasoAPasoResaltandoElBloqueEnCurso`,
      `SequenceExecutor_RF33_UnMovimientoInvalidoRetrocedeYLaEjecucionContinua`,
      `SequenceExecutor_CP06_ElResultadoNoNombraElBloqueQueDebeCorregirse`,
      `SequenceExecutor_RNF13_LaSecuenciaCorrectaAlcanzaElRefugio`.

**Depende de:** W11 · **Tamaño:** M

**Archivos**
- `.../Levels/Wheel/SequenceExecutor.cs`, `.../Levels/Wheel/ExecutionStep.cs`
- `Assets/Tests/EditMode/Levels/Wheel/SequenceExecutorTests.cs`

---

## W13: Escena `Level2_Maze` y editor de bloques

**Descripción.** Pantalla dividida: laberinto en vista superior a la izquierda, área de bloques a
la derecha. Arrastre de bloques con clic sostenido, soltar con clic, y el botón «Ejecutar» con
**clic simple** (PG-04 cerrado).

**Traza:** RF-30, RF-31, RF-32, RNF-02, RNF-03, RNF-19, CT-06, PG-04, HU-10, CU-08, guion §6.3.2.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] La escena presenta la carretilla, el refugio y obstáculos —piedras, curvas y pendientes— en
      vista superior (RF-30).
- [ ] «Ejecutar» responde a **un clic simple**, no a doble clic (PG-04, RF-32, RNF-02). Prueba
      explícita: es la contradicción que el guion normalizó.
- [ ] Los tres bloques se distinguen **por forma además de por color** (RNF-19): flecha adelante,
      flecha atrás, flecha curva de rotación.
- [ ] La orientación de la carretilla es visible en pantalla en todo momento — sin ella, la
      lectura relativa de INC-33 es incomprensible para el estudiante.
- [ ] El bloque en ejecución se resalta con doble indicador (RNF-19).
- [ ] El botón de ayuda a demanda está visible durante toda la fase (RF-13).
- [ ] Ningún elemento se sale de pantalla en la división izquierda/derecha; el área de bloques
      admite una secuencia larga sin desbordar.

**Verificación**
- [ ] PlayMode: `MazeScene_RF30_PresentaCarretillaRefugioYObstaculos`,
      `MazeScene_PG04_EjecutarRespondeAClicSimpleNoADobleClic`,
      `MazeScene_RNF19_LosTresBloquesSeDistinguenPorForma`,
      `MazeScene_RF31_LaOrientacionDeLaCarretillaEsVisible`.
- [ ] Aserción de layout con una secuencia de doce bloques.

**Depende de:** W12, W09 · **Tamaño:** M

**Archivos**
- `.../Levels/Wheel/MazeSceneController.cs`, `.../Levels/Wheel/BlockEditorView.cs`
- `Assets/Game/Scenes/Level2_Maze.unity`
- `Assets/Tests/PlayMode/Levels/Wheel/MazeSceneTests.cs`

---

## W14: Reintento sin reiniciar el nivel

**Descripción.** Al terminar la ejecución, la carretilla vuelve al punto de partida y **la
secuencia permanece en pantalla** para ser corregida. Es lo que convierte el fallo en depuración
en vez de en castigo.

**Traza:** RF-34, RF-18, CP-02, HU-10, CU-08 (FA-6a), guion §6.3.2.

**Modo de prueba:** PlayMode `[Category("Integration")]`.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] Tras una ejecución fallida la carretilla vuelve al inicio y la secuencia **sigue en
      pantalla, intacta** (RF-34).
- [ ] El escenario no se recarga ni se pierde: reintentar no es reiniciar el nivel (CU-08 FA-6a).
- [ ] No hay límite de ejecuciones ni pantalla de derrota (CP-02, RF-18). Comentario «por qué no»
      junto al contador de ejecuciones: existe para el indicador docente, **no** para limitar.
- [ ] Alcanzar el refugio confirma la fase 3, guarda (RF-04) y entra en el cierre del nivel.

**Verificación**
- [ ] PlayMode: `MazeScene_RF34_TrasUnaEjecucionFallidaLaSecuenciaPermaneceEnPantalla`,
      `MazeScene_CP02_NoHayLimiteDeEjecucionesNiPantallaDeDerrota`,
      `MazeScene_RF04_AlcanzarElRefugioConfirmaYGuardaLaFase3`.

**Depende de:** W13 · **Tamaño:** S

**Archivos**
- `.../Levels/Wheel/MazeSceneController.cs` (ampliar)
- `Assets/Tests/PlayMode/Levels/Wheel/MazeRetryTests.cs`

---

### ✅ Checkpoint W-E — Fase 3 completa

- [ ] El laberinto se resuelve componiendo, ejecutando, corrigiendo y volviendo a ejecutar.
- [ ] «Avanzar» produce desplazamientos distintos según la orientación — verificado jugando, no
      solo en EditMode (INC-33).
- [ ] Ninguna retroalimentación nombra el bloque a corregir (CP-06).
- [ ] Revisado con el usuario.

---

# Fase 5 — Cierre del nivel

## W15: Emisión de los cuatro indicadores del Nivel 2

**Descripción.** `ILevelReporter` para el Nivel 2 con la definición operativa **por fase** de
OE1 §3.6.1 — que en este nivel es distinta en cada una de las tres. Se persisten con el guardado
de fase (RF-04). No llegan al estudiante en ninguna forma.

Definición vigente que hay que implementar literalmente:

| Indicador | Fase 1 (bosque) | Fase 2 (taller) | Fase 3 (laberinto) |
|---|---|---|---|
| **Intentos** | Selecciones de un objeto no válido | Acciones rechazadas por estar fuera de secuencia | Ejecuciones de la secuencia que no alcanzan el refugio |
| **Errores corregidos** | Acción rechazada seguida de la acción correcta sobre el mismo elemento | Ídem | Bloques retirados o reordenados entre una ejecución fallida y la siguiente |
| **Pasos utilizados** | **Sin definición en §3.6.1 — ver pregunta abierta 1** | Acciones de ensamblaje ejecutadas en orden | Bloques que componen la secuencia ejecutada con éxito |
| **Tiempo de resolución** | Desde el inicio de la fase jugable hasta su completación, excluyendo escenas narrativas y el tiempo con la pausa abierta | Ídem | Ídem |

**Traza:** RF-45, RF-04, RNF-09, RNF-14, CP-03, CP-09, OE1 §3.6.1 (notas 1 a 5), INC-27, INC-29.

**Modo de prueba:** EditMode, con un doble de `ILevelReporter`. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Se emiten **exactamente cuatro** indicadores por fase; ninguno adicional (lista cerrada,
      RNF-09).
- [ ] Cada uno se calcula con la definición de la tabla de arriba, y hay una prueba por celda.
- [ ] El tiempo de resolución **excluye** las escenas narrativas y el tiempo con el menú de pausa
      abierto (nota 1 de §3.6.1).
- [ ] Reiniciar el nivel **no borra** los indicadores ya registrados (nota 4 de §3.6.1).
- [ ] Ninguno llega a la UI del estudiante — prueba de exclusión (CP-03, nota 3).

**Verificación**
- [ ] EditMode: `WheelIndicators_RF45_IntentosSeCuentanSegunLaDefinicionDeCadaFase`,
      `WheelIndicators_RF45_ErrorCorregidoEnFase3ExigeEdicionEntreDosEjecuciones`,
      `WheelIndicators_RF07_LaPausaNoSumaTiempoDeResolucion`,
      `WheelIndicators_OE1361_ReiniciarElNivelNoBorraLosIndicadoresRegistrados`,
      `WheelIndicators_CP03_NingunIndicadorLlegaALaUIDelEstudiante`.

**Depende de:** W14, Slice 1 T17 · **Tamaño:** M

**Archivos**
- `.../Levels/Wheel/WheelIndicatorCollector.cs`
- `Assets/Tests/EditMode/Levels/Wheel/WheelIndicatorTests.cs`

---

## W16: Resumen del Nivel 2, cierre reflexivo y desbloqueo del Nivel 3

**Descripción.** `LevelSummary` para el Nivel 2: resumen **narrativo y sin una sola cifra** de lo
que hizo el estudiante en las tres fases, seguido del cierre reflexivo del guion §6.4, donde
Chispa nombra las dos habilidades ejercitadas —abstraer y pensar como un algoritmo—. De ahí
vuelve al menú con el Nivel 3 desbloqueado.

**Traza:** RF-45, RF-12, RF-17, RF-03, CP-03, CP-07, CP-10, HU-14, CU-08, INC-26, guion §6.4.

**Modo de prueba:** EditMode (texto) + PlayMode (flujo). **Corredor MCP:** el PlayMode lo agradece.

**Criterios de aceptación**
- [ ] **Cero cifras** en el resumen: prueba que barre el texto renderizado buscando dígitos
      (INC-26 — es el punto donde HU-14 ya coló una en el Nivel 1).
- [ ] El resumen cubre las **tres** fases y ninguna cifra de las tres se filtra.
- [ ] El cierre reflexivo nombra explícitamente **la abstracción y el pensamiento algorítmico** y
      los relaciona con lo que el jugador acaba de hacer (RF-12, §6.4).
- [ ] El cierre reflexivo **no es omitible la primera vez** (CP-07).
- [ ] Al terminar, `LevelSelect` muestra el Nivel 3 habilitado (RF-03).

**Verificación**
- [ ] EditMode: `LevelSummary_RF45_ElResumenDelNivel2NoContieneNingunDigito`,
      `LevelSummary_RF12_NombraLaAbstraccionYElPensamientoAlgoritmico`.
- [ ] PlayMode: `LevelSummary_RF03_DevuelveAlMenuConNivel3Desbloqueado`.

**Depende de:** W15, W17, W18 · **Tamaño:** M

**Archivos**
- `.../Scaffolding/LevelSummaryContent.cs` (ampliar)
- `Assets/Game/Data/Wheel/N2_ResumenNivel.asset`, `Assets/Game/Data/Narrative/N2_Escena25_Cierre.asset`
- `Assets/Tests/EditMode/Scaffolding/LevelSummaryTests.cs` (ampliar)

---

## W17: Pausa y reinicio sobre las tres escenas del Nivel 2

**Descripción.** El menú de pausa ya existe (Slice 1 T16). Aquí se verifica que funciona en un
nivel de tres fases, que es donde «Reiniciar nivel» se vuelve ambiguo: reinicia **la fase activa**,
nunca las fases ya confirmadas ni el desbloqueo del nivel.

**Traza:** RF-07, RF-03, RF-04, CP-02, HU-17 (FA-01..FA-05), INC-25.

**Modo de prueba:** PlayMode `[Category("Integration")]`.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] «Continuar» restituye el estado exacto de la fase en las tres escenas — incluida una
      secuencia de bloques a medio componer.
- [ ] «Reiniciar nivel» pide confirmación, reinicia la fase activa, **no re-bloquea** el Nivel 2 ni
      descarta las fases ya confirmadas ni los indicadores registrados (INC-25, §3.6.1 nota 4).
- [ ] «Volver al menú principal» conserva el progreso confirmado.
- [ ] Sin `GameOver` en ninguna ruta (CP-02).
- [ ] El tiempo con la pausa abierta no suma al indicador de tiempo (enlaza con W15).

**Verificación**
- [ ] PlayMode: `PauseMenu_HU17_ContinuarRestituyeLaSecuenciaDeBloquesAMedioComponer`,
      `PauseMenu_INC25_ReiniciarNivelNoDescartaLasFasesYaConfirmadas`,
      `PauseMenu_CP02_NingunaRutaDeLaPausaLlevaAUnaPantallaDeDerrota`.

**Depende de:** W14, Slice 1 T16 · **Tamaño:** S

**Archivos**
- `.../UI/PauseMenuController.cs` (ampliar)
- `Assets/Tests/PlayMode/Core/PauseMenuWheelTests.cs`

---

## W18: Doble indicador y contraste en los estados de error del Nivel 2

**Descripción.** RNF-19 se verifica, literalmente, «inspeccionando los estados de error de los
niveles 2 y 3»: este slice es donde ese requerimiento se puede cerrar por primera vez. Barrer los
tres estados de rechazo del nivel —objeto no válido, paso fuera de secuencia, movimiento inválido—
y comprobar que ninguno depende solo del color.

**Traza:** RNF-19, RNF-20, RNF-21, CN-04, guion §6.1.2, §6.2.2, §6.3.2.

**Modo de prueba:** PlayMode `[Category("VisualVerification")]`.
**Corredor MCP:** **sí lo exige**; sin él, inspección manual sobre capturas, declarada.

**Criterios de aceptación**
- [ ] Los tres estados de error llevan **color + un segundo indicador** (icono, texto o forma).
- [ ] Los tres tipos de bloque se distinguen en escala de grises (prueba de desaturación).
- [ ] El contraste texto/fondo es ≥ 4.5:1 en las tres escenas, medido sobre el arte final y no
      sobre la paleta nominal (RNF-20).
- [ ] Ninguna animación del nivel —rodado, mecanizado, ejecución paso a paso, retroceso— incluye
      parpadeos rápidos ni destellos de alta frecuencia (RNF-21).

**Verificación**
- [ ] VisualVerification: `WheelLevel_RNF19_LosTresEstadosDeErrorSeLeenSinColor`,
      `WheelLevel_RNF20_ContrasteSuficienteEnLasTresEscenas`,
      `WheelLevel_RNF21_NingunaAnimacionDelNivel2TieneDestellos`.
- [ ] Análisis de las capturas guardadas tras la corrida (skill `run-tests`).

**Depende de:** W07, W09, W14 · **Tamaño:** S

**Archivos**
- `Assets/Tests/PlayMode/Levels/Wheel/WheelAccessibilityTests.cs`

---

### ✅ Checkpoint W-F — Slice 2 completo

- [ ] **Dos recorridos completos** del Nivel 2 sin incidencias (RNF-13): narrativa puente →
      bosque → patrón → taller → regreso → laberinto → cierre → menú con Nivel 3 desbloqueado.
- [ ] Cierre forzado en cada una de las tres fases: al reabrir, el perfil retoma desde la última
      fase confirmada (RNF-14). Tres pruebas, no una.
- [ ] **Prueba de exclusión de RNF-16 con dos niveles reales:** retirar `Game.Levels.Wheel` y
      comprobar que el Nivel 1 sigue ejecutándose; y al revés.
- [ ] Carga de las tres escenas < 10 s y memoria < 2 GB, medidas en el equipo de referencia
      (RNF-04, RNF-05). El laberinto es la escena con más objetos del slice.
- [ ] Paquete acumulado aún por debajo de 500 MB con el arte del Slice 2 incluido (RNF-06).
- [ ] Mapa de controles de las tres escenas inspeccionado: solo clic y clic sostenido (RNF-02, CT-06).
- [ ] **PG-05 verificado**: el paso del panel del Nivel 1 al arrastre del Nivel 2 no confundió al
      estudiante en la sesión de prueba. Anotar el resultado — es un punto abierto del guion.
- [ ] Todo RF del slice (RF-22..RF-34) tiene al menos una prueba que lo nombra (CT-10).
- [ ] Revisado con el usuario antes de abrir el Slice 3.

---

## Riesgos

| # | Riesgo | Impacto | Mitigación |
|---|---|---|---|
| **R1** | **No hay corredor de pruebas MCP.** `run_unity_tests`, `get_unity_compilation_result` y `unity_play_control` siguen sin conectar. Este slice tiene **ocho tareas PlayMode/VV** contra las seis del Slice 1: el costo de no tenerlo crece. | **Alto — abierto** | Cada tarea de arriba declara si lo exige. Las EditMode (W01, W02, W03, W05, W08, W10, W11, W12, W15) se sostienen a mano sin perder rigor y cubren toda la lógica del nivel. Las PlayMode se corren a mano en Test Runner y **se declara el resultado**, nunca se da por hecho. Instalar el servidor sigue siendo la acción de mayor retorno del proyecto. |
| **R2** | **El Slice 1 no está hecho.** Este plan generaliza ocho piezas que aún no existen. | **Alto — abierto** | No empezar W02 antes del Checkpoint D del Slice 1. W01 y W10 son las únicas tareas que no dependen de código del Slice 1 y podrían adelantarse. |
| R3 | **INC-33: la lectura relativa es el error más fácil de cometer.** «Avanzar» leído como «arriba» compila, se ve razonable y hace el refugio inalcanzable. | Alto | W10 se adelanta y se prueba en EditMode antes de existir la escena. La prueba `AvanzarEsRelativoALaOrientacionNoAbsoluto` compara dos orientaciones desde la misma casilla: con lectura absoluta falla. |
| R4 | **PG-05 abierto**: el esquema de control cambia entre el Nivel 1 (panel) y el Nivel 2 (arrastre). Es exactamente la transición que este slice construye. | Medio | La instrucción de la fase 1 (W03) enseña el gesto antes de exigirlo. Verificar en el Checkpoint W-F con estudiantes y anotar el resultado en el guion §12. |
| R5 | **El chroma verde no sirve para un bosque.** `#00FF00` sobre follaje verde no se recorta. | Medio | Los assets de vegetación y los del laberinto usan **chroma magenta `#FF00FF`**; los demás conservan `#00FF00`. Marcado asset por asset en la sección siguiente. |
| R6 | **La paleta del Slice 1 es de cueva iluminada por fuego.** No cubre un bosque de día ni un taller. | Medio | Se conserva íntegra —los personajes y el marco de diálogo ya generados deben seguir encajando— y se **extiende** con un bloque de exteriores. El bloque de estilo cambia una sola frase, la de la iluminación. Documentado abajo. |
| R7 | El laberinto es la escena con más objetos del slice y la primera con vista superior; puede tensar RNF-04. | Bajo | Medir la carga de `Level2_Maze` explícitamente en el Checkpoint W-F, no estimarla. |
| R8 | Deriva visual entre generaciones de arte, y entre el arte del Slice 1 y el de este. | Medio | Bloques de estilo y paleta fijos, copiados literalmente. Los personajes **no se vuelven a generar**: se reutilizan los del Slice 1. |

---

## Preguntas abiertas

1. **`Pasos utilizados` no está definido para la fase 1** (OE1 §3.6.1). La tabla define el
   indicador para las fases 2 y 3 del Nivel 2, pero no para la fase 1 del bosque. Hay dos lecturas
   razonables —troncos válidos acopiados (5 fijos, luego no informa nada) o selecciones totales
   hasta completar el acopio—. **Es un entregable radicado: no se decide desde el código.** Hasta
   que se resuelva, W15 emite el indicador de la fase 1 como «no aplica» y lo documenta.
2. **PG-02 — nombre del guía.** El guion adopta «Chispa», pero el documento fuente del Nivel 2 lo
   llamaba «Algorim». Los textos de W04 usan Chispa; al vivir en ScriptableObjects, cambiarlo no
   cuesta código. Confirmar antes de generar el arte de la escena 2.1.
3. **Trazado del laberinto.** El guion nombra los obstáculos —piedras, curvas y pendientes— pero no
   fija la rejilla, ni el tamaño, ni la posición del refugio. W10 propone un trazado en
   `N2_MazeLayout.asset`; hay que validarlo jugando: debe tener al menos una solución no trivial
   (que exija girar) y ninguna que se resuelva con «Avanzar» repetido.
4. **Adelantar W10.** ¿Se ejecuta el plan en el orden narrativo (bosque → taller → laberinto) o se
   adelantan W10..W12 para descargar el riesgo de INC-33 antes? Recomendación: adelantarlas.

---

# Assets visuales del Slice 2

Diez assets. Generador: **Gemini / Nano Banana Pro**. Los prompts están en español y se pegan
tal cual, en un solo mensaje, **sin resumirlos**.

**Documento que manda:** `claudeDocs/Direccion_de_Arte.md`, §8.2 para este nivel. Si algo se
contradice, gana la dirección de arte; si esta contradice a `SPEC.md`, gana `SPEC.md`.

**Cómo se arma un prompt.** Los mismos cinco bloques fijos del Slice 1, cambiando solo el de
paleta, y después la descripción del asset:

```
[1 CONTEXTO N2]  [2 ESTILO]  [3 PALETA N2]  [4 ENTREGA]  [5 PROHIBICIONES]  +  ELEMENTO
```

`[2 ESTILO]`, `[4 ENTREGA]` y `[5 PROHIBICIONES]` se copian **idénticos** a los del
`Slice 1/plan.md` — no se reescriben ni se resumen: que sean literalmente los mismos es lo que
hace que los dos niveles parezcan el mismo juego. Aquí abajo solo se redefinen el contexto y la
paleta, que sí cambian de nivel.

**Los personajes no se vuelven a generar.** Chispa, Papá, Mamá, la Niña y el Niño son los assets
`A1`..`A5` del Slice 1 y se reutilizan tal cual. La Niña es la personaje jugable de este nivel
(guion §1.2, CN-02); las poses que el bosque necesite salen de **animar su sprite en A-pose con
2D Animation**, no de una generación nueva (`Direccion_de_Arte.md` §13.1).

**Autoría (CT-09, RNF-23).** Los assets de este slice —escenarios, props e interfaz— son
**originales del proyecto**. Los personajes que reutiliza del Slice 1 son **obra derivada** de
los diseños de la Familia Anonaky, con **autorización escrita concedida** (PG-07 cerrado el
30/08/2026) y reconocimiento obligatorio en créditos. Cada asset generado se registra en
`CreditsContent.asset` (Slice 1, T08).

**Transparencia.** Gemini no produce canal alfa fiable, así que se genera sobre fondo plano y se
recorta después. **El verde no sirve en este nivel para nada vegetal**: los assets marcados
**Chroma magenta** se piden sobre `#FF00FF`, los marcados **Chroma verde** sobre `#00FF00`, y los
fondos de escena no llevan chroma.

---

## Bloque 1 · CONTEXTO N2 — sustituye al del Slice 1

```
CONTEXTO DEL ENCARGO
Soy diseñador de un videojuego educativo 2D hecho en Unity para estudiantes de grado cuarto de
primaria, de 9 a 11 años. El juego acompaña a una familia prehistórica en tres descubrimientos:
el fuego, la rueda y el cruce de un río. Este encargo pertenece al Nivel 2, «La Rueda», que
transcurre de día en un BOSQUE: un claro donde la familia observa objetos, un área de trabajo
junto a su refugio, y un sendero cerrado por vegetación. No es un desierto, no hay mesetas, no
hay cañón, no hay arena y no hay cactus.

Lo que necesito NO es una ilustración de escena, ni una lámina de presentación, ni un concept
art. Es un ASSET DE PRODUCCIÓN: un archivo que voy a recortar e importar a Unity como sprite,
que se verá en movimiento, superpuesto a otros elementos, a un tamaño mucho menor que el de
generación, y proyectado en pantallas de aula de baja calidad. Una imagen bonita que no se pueda
recortar limpiamente, o que no se lea a tamaño pequeño, no me sirve y la descarto.

Tres condiciones mandan sobre cualquier consideración estética:
1. PÚBLICO INFANTIL. Nada amenazante, afilado, sombrío, triste ni violento. No hay animales
   peligrosos, no hay armas, no hay heridas.
2. BAJO CONSUMO DE RECURSOS. Los equipos del colegio no tienen tarjeta gráfica dedicada. El arte
   es plano y simple por diseño, no por descuido.
3. LEGIBILIDAD ANTES QUE DETALLE. Este nivel se juega distinguiendo objetos entre sí: si un
   detalle compite con la lectura de la silueta, sobra.

INSTRUCCIÓN SOBRE LO QUE NO TE DIGA: sigue las secciones de abajo al pie de la letra. Donde no
te dé un dato, NO lo inventes ni lo rellenes con tu criterio: elige la opción más simple
compatible con las reglas y deja el resto vacío. No añadas objetos, personajes, adornos, texto,
fondo, marcos ni elementos decorativos que no haya pedido explícitamente. Si crees que falta
algo, omítelo: prefiero un asset incompleto a uno inventado.
```

## Bloque 3 · PALETA DEL NIVEL 2 — usar solo estos colores

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

BOSQUE DEL NIVEL 2:
  Follaje cercano #7FA05A      Follaje medio #5A7A3F      Follaje lejano #3C5429
  Planta baja #6E9B4E          Musgo #4A5C42
  Suelo de tierra #8A6B4A      Suelo en sombra #6B5344
  Corteza de árbol #5C4530     (decorado: SIEMPRE oscura y desaturada)
  Piedra fría #7A8290          Piedra fría en sombra #4E5561
  Cielo entre las copas #A8DCE6    Nubes #F2F7F5 con sombra #D8E4E8

ACENTO DEL NIVEL — SOLO PARA LO INTERACTIVO:
  Madera trabajada #C79A5E     su sombra #A67C4A

NEUTROS DE INTERFAZ (comunes a todo el juego):
  Marfil #F7EFE2   Marfil sombra #E0D4C0   Borde de panel #C4A882
  Carbón #3A1E18 (texto y contorno)        Carbón suave #6B5248
  Éxito #5FA842    Atención #E8A33D

REGLA DE ACENTO (crítica): la madera clara trabajada #C79A5E pertenece EXCLUSIVAMENTE a los
objetos del reto —tronco cortado, rueda, eje, tabla, carretilla—. Ningún árbol, arbusto, suelo
ni elemento de decorado puede llevarla: la corteza del decorado es siempre #5C4530, oscura. Lo
CORTADO y TRABAJADO se distingue de lo natural. Esa distinción es el nivel entero.
```

---

## B1 · Escenario del bosque

**Traza:** RF-22, guion §6.1.1 y §6.1.2 (escenario de la fase 1).
**Chroma:** **no** — es el fondo completo de la escena. **Archivo:** `env_n2_bosque_claro.png`.

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [5 PROHIBICIONES]

ELEMENTO: fondo completo de escena. Claro de bosque prehistórico de día, vista LATERAL, plano
fijo, SIN personajes y SIN objetos sueltos en el suelo.

FORMATO: rectangular 16:9 horizontal. Este asset NO lleva fondo de croma: la imagen entera es el
escenario y se importa tal cual. Ignora la instrucción de croma; el resto de las reglas sigue
vigente.

COMPOSICIÓN FIJA:
- Suelo de tierra compacta en el tercio inferior, en #8A6B4A con sombra #6B5344, de ondulaciones
  muy suaves y sin textura de grano.
- A izquierda y derecha, dos troncos de árbol verticales que enmarcan la escena sin cerrarla, de
  sección ovalada y corteza OSCURA #5C4530. Nunca madera clara: la madera clara es de los objetos
  del reto, no del decorado.
- Copas de follaje en tres profundidades, construidas con círculos superpuestos y NUNCA con hojas
  individuales: #3C5429 al fondo, #5A7A3F en el medio, #7FA05A al frente. El follaje del fondo va
  SIN contorno; el del frente, con contorno fino #5C4038.
- Entre las copas, huecos por los que se ve el cielo #A8DCE6 con una o dos nubes #F2F7F5 de
  sombra inferior #D8E4E8. El cielo se ve a retazos, nunca como un horizonte abierto.
- Arbustos bajos #6E9B4E y dos manchas de musgo #4A5C42 pegados a los bordes inferiores.
- Dos piedras de canto rodado #7A8290 semienterradas, una a cada lado, pequeñas.

ZONA LIBRE OBLIGATORIA: toda la franja central del suelo queda COMPLETAMENTE despejada y sin
detalle. Ahí se dispersarán los objetos seleccionables y el contador. No pongas nada.

Sin animales, sin fuego, sin humo, sin sendero marcado, sin flores llamativas.
```

**Verificación (§17):** ninguna madera clara `#C79A5E` en el decorado · franja central vacía ·
follaje del fondo sin contorno y desaturado · pasa la prueba de entrecerrado (§6).

---

## B2 · Objetos del bosque — válidos y distractores

**Traza:** RF-22, RF-23, RNF-19 (las categorías se distinguen por forma), guion §6.1.2.
**Chroma:** **magenta `#FF00FF`** — hay plantas verdes entre ellos.
**Archivo:** `prop_n2_tronco_a.png` … `prop_n2_herramienta_c.png`.

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: los objetos seleccionables del claro del bosque. Lámina de objetos sueltos, cada uno
completo, separado de los demás y sin superponerse. Sin texto de ningún tipo.

FONDO: en lugar del verde, usa MAGENTA croma puro #FF00FF, plano y uniforme. Aquí hay plantas
verdes y el verde se comería el recorte.

Generar CUATRO FILAS, en este orden:

FILA 1 — TRONCOS CORTADOS (los VÁLIDOS, los que ruedan): cinco secciones cortas de tronco vistas
en perspectiva de tres cuartos, con la CARA CIRCULAR bien visible y silueta claramente
CILÍNDRICA. Corteza lateral #5C4530; cara circular en MADERA TRABAJADA #C79A5E con sombra
#A67C4A y tres anillos concéntricos simples. Contorno de objeto interactivo, 7 a 9 px, #3A1E18.
Los cinco iguales en forma, con variación mínima de tamaño. La REDONDEZ debe ser el rasgo más
evidente, legible en silueta.

FILA 2 — PIEDRAS IRREGULARES (distractor): cuatro piedras de silueta ANGULOSA y facetada, con
cuatro o cinco caras planas marcadas y ninguna curva, aunque con los vértices suavizados. Cuerpo
#7A8290 con facetas en sombra #4E5561. Contorno #5C4038, más fino que el de los troncos. Deben
leerse como lo contrario de un cilindro.

FILA 3 — PLANTAS (distractor): tres matas de hojas anchas, de silueta ABIERTA y ramificada, que
deja ver el fondo entre las hojas. Hojas #6E9B4E con nervadura #3C5429 de una sola línea, tallos
delgados #5A7A3F. Contorno #5C4038.

FILA 4 — HERRAMIENTAS (distractor): tres herramientas prehistóricas simples, de silueta ALARGADA
y RECTA: mango de madera oscura #5C4530 con una cabeza de piedra pulida #7A8290 de punta ROMA,
atada con una tira de cuero #8C4A2F. Sin filo, sin punta afilada; no debe leerse como arma.
Contorno #5C4038.

REQUISITO DE ACCESIBILIDAD (obligatorio): las cuatro familias deben distinguirse por SILUETA en
escala de grises, sin depender del color — cilindro, faceta angulosa, mata abierta, barra
alargada. Es el criterio de verificación de RNF-19.
```

**Verificación (§17):** las cuatro familias se separan en negro sólido · solo los troncos llevan
`#C79A5E` · las herramientas no parecen armas · contorno más grueso en los válidos que en los
distractores (§9.2).

---

## B3 · Caja de alimentos — tres estados

**Traza:** RF-25, RF-26, guion §5 y §6.1.2. **Chroma:** verde `#00FF00`.
**Archivo:** `prop_n2_caja_suelo.png`, `_sobre_troncos`, `_rodando`.

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: la caja de alimentos que la familia no logra mover. Es el objetivo de la fase 1.

FORMA FIJA (idéntica en los tres estados): cesto bajo y ancho de fibra trenzada #C4A882 con
sombra #A67C4A, de esquinas redondeadas y base plana, con dos asas laterales de cuerda trenzada
#5C2B22. Dentro, asomando por el borde, frutos redondos #E8A33D y hojas #6E9B4E. Contorno de
objeto interactivo, 7 a 9 px, #3A1E18.

Generar TRES versiones de la MISMA caja, idénticas en forma, tamaño y contenido:
  (1) EN EL SUELO: apoyada directamente sobre la tierra, ligeramente hundida, con una sombra
      plana ancha #6B5344 debajo. Debe leerse pesada y atascada.
  (2) SOBRE LOS TRONCOS: la misma caja apoyada encima de TRES troncos cortados alineados en
      paralelo bajo ella, vistos de lado como círculos de cara #C79A5E con anillos y corteza
      #5C4530. Sombra plana más corta.
  (3) RODANDO: la misma caja desplazada hacia la derecha sobre los mismos tres troncos, con los
      anillos de las caras GIRADOS respecto de (2) para que se lea el movimiento, y tres líneas
      cortas horizontales #F7EFE2 detrás de la caja. Sin destellos, sin polvo brillante, sin
      estelas de velocidad curvas.

La diferencia entre (1) y (2) debe leerse por FORMA —hundida contra elevada sobre cilindros— y
no por color: es lo que enseña el patrón del nivel.
```

**Verificación (§17):** la caja es idéntica en los tres · los troncos llevan el acento y la caja
no · el estado (3) se distingue de (2) en escala de grises.

---

## B4 · Escenario del área de trabajo

**Traza:** RF-27, guion §6.2.2 (escenario de la fase 2).
**Chroma:** **no** — fondo completo de escena. **Archivo:** `env_n2_taller.png`.

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [5 PROHIBICIONES]

ELEMENTO: fondo completo de escena. Área de trabajo al aire libre junto al refugio de la
familia, en un claro del bosque. Vista LATERAL, plano fijo, SIN personajes y SIN las piezas de
la carretilla.

FORMATO: rectangular 16:9 horizontal, sin croma, igual que B1.

COMPOSICIÓN FIJA:
- Explanada de tierra compacta #8A6B4A con sombra #6B5344 en la mitad inferior.
- A la IZQUIERDA, el refugio: estructura triangular baja hecha de palos #5C4530 y pieles #E8C07A
  con manchas #2B1A12, cerrada, sin interior visible. Pequeño, ocupa como mucho un cuarto del
  ancho.
- A la DERECHA, el banco de trabajo: una losa de piedra plana #7A8290 con sombra #4E5561 apoyada
  sobre dos troncos cortos de corteza #5C4530. Sin herramientas encima.
- Al fondo, una línea de follaje #5A7A3F y, tras ella, la masa #3C5429 sin contorno. Huecos de
  cielo #A8DCE6 entre las copas.
- Junto al refugio, una fogata APAGADA: círculo de piedras #7A8290 con brasas #E2571F muy
  pequeñas y un hilo tenue de humo #6B5A60. Discreta, en el borde izquierdo. Es lo único cálido
  del encuadre y no debe robar la mirada.

ZONA LIBRE OBLIGATORIA: toda la franja central y baja queda despejada y sin detalle. Ahí se
dispondrán las seis piezas y se ensamblará la carretilla. No pongas nada.

Sin madera clara trabajada en ninguna parte del decorado.
```

**Verificación (§17):** franja central vacía · el refugio no compite con el centro · ninguna
madera `#C79A5E` en el decorado · la fogata apagada no atrae la mirada antes que el centro.

---

## B5 · Piezas del taller — las seis de RF-27

**Traza:** RF-27, RF-28, guion §6.2.2. **Chroma:** verde `#00FF00`.
**Archivo:** `prop_n2_pieza_1.png` … `prop_n2_pieza_6.png`.

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: las seis piezas del área de trabajo, sueltas y separadas, sin superponerse. Sin texto
de ningún tipo. Todas son objetos INTERACTIVOS: contorno de 7 a 9 px en #3A1E18.

Generar las SEIS piezas, cada una completa y aislada, en una fila o en dos filas de tres:
  (1) TRONCO CORTO A: sección cilíndrica vista de tres cuartos, cara circular visible en madera
      trabajada #C79A5E con sombra #A67C4A y tres anillos concéntricos; corteza lateral #5C4530.
      Centro MACIZO, SIN agujero.
  (2) TRONCO CORTO B: idéntico al anterior en forma, tamaño y color. Que sean gemelos.
  (3) EJE LARGO: barra cilíndrica larga y recta, de diámetro claramente MENOR que el de los
      troncos cortos —como un tercio—, en #C79A5E con extremos #A67C4A. Debe leerse a simple
      vista que cabría por el centro de los troncos cortos.
  (4) TABLA: pieza rectangular plana de madera trabajada #C79A5E con vetas #A67C4A dibujadas
      como dos líneas rectas largas, borde recto y esquinas ligeramente redondeadas. Es una de
      las pocas formas angulosas del juego, y es intencional: está fabricada.
  (5) HERRAMIENTA: mango de madera oscura #5C4530 con cabeza de piedra pulida de punta ROMA
      #7A8290, atada con tira de cuero #8C4A2F. Sin filo, no debe leerse como arma.
  (6) CESTO DE ALIMENTOS: el mismo cesto del asset B3, estado (1), sin variar su forma ni su
      color.

REQUISITO: las seis deben distinguirse por SILUETA en escala de grises — cilindro corto,
cilindro corto gemelo, barra fina larga, placa plana, mango con cabeza, cesto. Que los dos
troncos cortos sean indistinguibles entre sí es correcto y deliberado: son dos piezas iguales.
```

**Verificación (§17):** seis siluetas separables · troncos (1) y (2) idénticos · el eje se ve
más fino que el hueco de los troncos · la herramienta no parece un arma.

---

## B6 · La rueda y la carretilla — cinco estados de ensamblaje

**Traza:** RF-28, RF-29, HU-09, CU-07, guion §6.2.2. Es el asset que hace legible la secuencia.
**Chroma:** verde `#00FF00`. **Archivo:** `prop_n2_carretilla_e1.png` … `_e5.png`.

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: los cinco estados del ensamblaje de la carretilla. Vista lateral de tres cuartos, LA
MISMA en los cinco. Sin personajes, sin manos, sin texto, sin fondo.

REGLA QUE MANDA SOBRE TODO: cada estado se construye LITERALMENTE sobre el anterior. Lo que ya
está no cambia de forma, de color, de tamaño ni de ángulo. Los cinco tienen que leerse como el
mismo objeto creciendo, no como cinco objetos distintos. Mantén escala, ángulo de cámara y
posición constantes en los cinco, alineados sobre la misma línea de base.

  (1) TRONCO SIN PERFORAR: un tronco corto cilíndrico, corteza lateral #5C4530, cara circular
      #C79A5E con sombra #A67C4A y tres anillos concéntricos. Centro MACIZO, sin agujero.
  (2) RUEDA PERFORADA: el MISMO tronco, ahora con un agujero circular limpio en el centro exacto
      de la cara, relleno oscuro #4E5561, con el borde interior del agujero en #A67C4A. Nada más
      cambia: mismo diámetro, mismos anillos, misma corteza.
  (3) EJE FORMADO: DOS ruedas perforadas idénticas a (2), separadas y paralelas, atravesadas por
      el eje largo #C79A5E que entra por el centro de ambas y sobresale un poco a cada lado. Las
      ruedas quedan verticales; el eje, horizontal.
  (4) TABLA MONTADA: el conjunto de (3) con la tabla #C79A5E apoyada horizontalmente encima del
      eje, centrada, sobresaliendo por delante y por detrás por igual.
  (5) CARRETILLA COMPLETA: el conjunto de (4) con el cesto de alimentos de B3 apoyado encima de
      la tabla, y dos varas de empuje #C79A5E saliendo hacia atrás desde la tabla en diagonal
      ascendente, terminadas en mango redondeado.

Contorno de objeto interactivo, 7 a 9 px, #3A1E18, en los cinco.
```

**Verificación (§17):** los cinco comparten escala, ángulo y línea de base · (2) solo añade el
agujero · el crecimiento se lee en negro sólido · sin manos ni personajes.

---

## B7 · Carretilla en vista superior — cuatro orientaciones

**Traza:** RF-30, RF-31, INC-33, supuesto 8, guion §6.3.2. **Sin las cuatro orientaciones, la
lectura relativa de los bloques es incomprensible en pantalla.**
**Chroma:** **magenta `#FF00FF`** — va sobre un tablero con vegetación.
**Archivo:** `prop_n2_carretilla_cenital_norte.png`, `_este`, `_sur`, `_oeste`.

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: la carretilla vista DESDE ARRIBA, en perspectiva cenital pura de 90 grados, para el
tablero del laberinto. Sin personajes, sin texto, sin suelo debajo.

FONDO: MAGENTA croma puro #FF00FF, plano y uniforme. No uses verde: va sobre vegetación.

FORMA FIJA vista desde arriba: la tabla rectangular #C79A5E con sombra #A67C4A ocupa el centro;
encima, el cesto #C4A882 con frutos #E8A33D asomando. A cada lado de la tabla, una rueda vista
de canto como un rectángulo estrecho y redondeado en #5C4530. Por detrás, las dos varas de
empuje #C79A5E en paralelo. El conjunto debe caber holgadamente dentro de una casilla cuadrada,
con margen a los cuatro lados.

INDICADOR DE ORIENTACIÓN (obligatorio, es lo más importante del asset): un triángulo macizo
#E8A33D con contorno #3A1E18 en el frente de la carretilla, apuntando hacia donde mira. Es lo
único que le dice al jugador hacia dónde avanzará. Grande, inconfundible y legible a 32 px.

Generar CUATRO versiones del MISMO objeto, idénticas en todo salvo en la rotación:
  (1) mirando hacia ARRIBA;
  (2) mirando hacia la DERECHA;
  (3) mirando hacia ABAJO;
  (4) mirando hacia la IZQUIERDA.

Cada una es la anterior rotada 90 grados en SENTIDO HORARIO. Comprueba que la secuencia
(1)→(2)→(3)→(4) sea exactamente eso: el sentido del giro es una regla del juego, no un detalle.
```

**Verificación (§17):** el triángulo de orientación se lee a 32 px · las cuatro son el mismo
objeto rotado · el giro es horario · cabe en una casilla con margen.

---

## B8 · Tablero del laberinto — vista superior

**Traza:** RF-30, RF-33, guion §6.3.2.
**Chroma:** **no** — es el fondo del área izquierda de la escena.
**Archivo:** `env_n2_tablero.png`, `prop_n2_obstaculo_piedra.png`, `_curva`, `_pendiente`.

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [5 PROHIBICIONES]

ELEMENTO: el tablero del laberinto, VISTA CENITAL pura de 90 grados. Sin la carretilla y sin
texto de ningún tipo.

FORMATO: lámina cuadrada, sin croma.

COMPOSICIÓN FIJA: rejilla cuadrada de 8 por 8 casillas sobre suelo de sendero #8A6B4A, con la
línea de rejilla apenas insinuada en #6B5344 —visible pero discreta: el estudiante cuenta pasos
sobre ella—. Alrededor de la rejilla, un borde de follaje denso #3C5429 y #5A7A3F que la cierra
por los cuatro lados: fuera de la rejilla no se puede ir, y tiene que verse así de claro.

En la casilla de la esquina SUPERIOR IZQUIERDA, el REFUGIO visto desde arriba: estructura
triangular de palos #5C4530 y pieles #E8C07A con manchas #2B1A12, con la abertura orientada
hacia el interior de la rejilla. Es el destino, y debe ser el elemento más llamativo del tablero.

Generar ADEMÁS, sueltas y separadas al lado del tablero, las tres piezas de obstáculo, cada una
del tamaño exacto de una casilla y distinguibles entre sí POR FORMA en escala de grises:
  (1) PIEDRA: bloque angular facetado #7A8290 con sombra #4E5561, silueta cerrada y compacta que
      ocupa casi toda la casilla.
  (2) CURVA: tramo de sendero que gira en ángulo recto, con dos matas de arbusto #6E9B4E en las
      esquinas exteriores. Silueta en codo.
  (3) PENDIENTE: franja de terreno inclinado, cruzada en diagonal por tres líneas paralelas de
      nivel en #6B5344. Silueta rayada.

La rejilla debe quedar limpia y con las casillas claramente separables a simple vista.
```

**Verificación (§17):** las 8×8 casillas se cuentan a simple vista · el refugio destaca sobre
todo lo demás · los tres obstáculos se separan en escala de grises · el borde de follaje lee
como límite infranqueable.

---

## B9 · Bloques de instrucción y botón «Ejecutar»

**Traza:** RF-31, RF-32, RNF-19, PG-04 (clic simple), guion §6.3.2. **Los tres bloques tienen
que distinguirse por forma: es el criterio de verificación literal de RNF-19.**
**Chroma:** verde `#00FF00`. **Archivo:** `ui_n2_bloque_avanzar_reposo.png` …

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: los bloques de instrucción del editor de secuencia y su botón de ejecutar. Elementos
de INTERFAZ: planos, frontales, sin perspectiva y sin volumen, legibles a 32 px. SIN TEXTO de
ningún tipo: los bloques se leen por su icono, nunca por una palabra escrita.

FORMA COMÚN: ficha rectangular horizontal de esquinas muy redondeadas, en cuero #C4A882 con
contorno #3A1E18, con una muesca cóncava en el borde superior y una lengüeta convexa en el
inferior, de modo que las fichas se encajen visualmente al apilarse en vertical. Todas del mismo
tamaño, área táctil generosa.

Generar TRES bloques, cada uno en DOS estados, ordenados en tres filas de dos:
  (1) AVANZAR: flecha maciza RECTA apuntando HACIA ARRIBA, en #3A1E18, centrada en la ficha.
  (2) RETROCEDER: flecha maciza RECTA apuntando HACIA ABAJO, en #3A1E18, con el asta cruzada por
      DOS líneas transversales cortas, para que no se confunda con (1) si se gira la lámina.
  (3) GIRAR: flecha CURVA en arco de tres cuartos de círculo, con la punta apuntando en SENTIDO
      HORARIO, en #3A1E18.

  Estado REPOSO: ficha #C4A882, icono #3A1E18.
  Estado RESALTADO (el bloque que se está ejecutando): la MISMA ficha con el cuerpo en #E8A33D,
      el icono en #3A1E18 y un MARCO EXTERIOR grueso y continuo en #5C2B22 rodeando la ficha. El
      cambio tiene que leerse también sin color, por la presencia del marco.

Generar ADEMÁS el BOTÓN EJECUTAR: botón redondeado ancho en #E8A33D con contorno #3A1E18 y, en
el centro, un triángulo macizo #3A1E18 apuntando a la derecha. UN SOLO botón, sin variantes: se
acciona con un clic simple.

REQUISITO DE ACCESIBILIDAD: los tres iconos deben distinguirse entre sí en escala de grises y a
tamaño pequeño — recta arriba, recta abajo con marcas, curva en arco.
```

**Verificación (§17):** los tres iconos se separan en escala de grises · reposo y resaltado se
distinguen por el marco, no solo por color (RNF-19) · sin texto · fichas encajables · área
táctil generosa.

---

## B10 · Contador de acopio y marco de retroalimentación de fase

**Traza:** RF-24, RF-11, RF-17, RNF-19, RNF-20, CP-03, guion §6.1.2.
**Chroma:** verde `#00FF00`. **Archivo:** `ui_n2_contador_marco.png`, `ui_estado_aceptado.png`,
`ui_estado_devuelto.png`.

> **Sin cifras a la vista.** El marco va vacío: el número lo escribe el motor y **solo** aparece
> como «n de 5» del acopio en curso, que es progreso de tarea, no puntaje (RF-24). Ningún
> indicador de desempeño se muestra al estudiante (CP-03, RF-17).

```
[1 CONTEXTO N2] [2 ESTILO] [3 PALETA N2] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: el contador de acopio de la fase 1 y los dos iconos de resultado de una selección.
Elementos de INTERFAZ, planos y frontales. Sin texto dentro de la imagen.

Generar TRES elementos separados en la misma lámina:

  (1) MARCO DEL CONTADOR: placa horizontal pequeña en marfil #F7EFE2 con borde de cuero cosido
      #C4A882 y puntadas #6B5248, de esquinas muy redondeadas. A la IZQUIERDA de la placa, la
      silueta de un tronco cortado visto de frente —círculo con tres anillos concéntricos
      #C79A5E y borde de corteza #5C4530—. El resto de la placa queda COMPLETAMENTE VACÍO, liso
      y en marfil: ahí escribirá el motor. Sin líneas, sin renglones, sin números.

  (2) ICONO DE ACEPTADO: círculo macizo #5FA842 con contorno #3A1E18 y, dentro, una marca de
      verificación #F7EFE2 de trazo grueso y puntas redondeadas.

  (3) ICONO DE DEVUELTO: ROMBO macizo #E8A33D con contorno #3A1E18 y, dentro, una flecha curva
      #F7EFE2 que apunta hacia atrás, indicando que el objeto regresa a su lugar. NO uses una
      equis, ni una cruz, ni el color rojo: el objeto no está mal, simplemente vuelve a su sitio.
      Esta distinción es una decisión pedagógica del proyecto, no una preferencia estética.

REQUISITO DE ACCESIBILIDAD: (2) y (3) deben distinguirse por FORMA además de por color —círculo
frente a rombo, marca frente a flecha— y ser legibles en escala de grises.
```

**Verificación (§17):** el marco va vacío · aceptado y devuelto se separan en escala de grises ·
ningún rojo de error en la lámina (§12.3) · contraste del texto ≥ 4.5:1 sobre el marfil (RNF-20).

---

## Postproceso de los assets con chroma

1. **Verificar** contra la checklist de `Direccion_de_Arte.md` §17 y contra la línea
   «Verificación» de cada asset. Si falla una, se vuelve a generar.
2. **Recortar** el fondo —`#00FF00` o `#FF00FF` según lo marcado— y exportar PNG con alfa.
   Revisar el halo; si queda, encogerlo un píxel. **El halo magenta se nota más que el verde
   sobre follaje**: revisar `B2` y `B7` con especial cuidado.
3. **Nombrar** según §15.4 y **importar** con los ajustes de §15.2. **Pixels Per Unit `100`**,
   el mismo del Slice 1: si difiere, la carretilla y los personajes no comparten escala.
4. **Verificar RNF-19 sobre el arte final**: desaturar `B2`, `B7`, `B8`, `B9` y `B10` y comprobar
   que las categorías y los estados siguen distinguiéndose. Si no, la corrección es de forma, no
   de color.
5. **Verificar RNF-20 sobre el arte final**, no sobre la paleta nominal: el bosque es claro y el
   contraste se mide en la imagen.
6. **Registrar** cada asset en `CreditsContent.asset` (Slice 1, T08) — CT-09, RNF-23.
