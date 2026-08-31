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

Diez assets. Generador principal: **Gemini / Nano Banana Pro**. Los prompts están en español y se
pegan tal cual.

**Regla de uso:** copiar el bloque de estilo y el bloque de paleta **literalmente** al inicio de
cada prompt, antes de la descripción del asset. Ahí está la consistencia entre generaciones: lo
que varía es solo la descripción; lo que se repite palabra por palabra es todo lo demás.

**Los personajes no se vuelven a generar.** Chispa, Papá, Mamá, la Niña y el Niño son los assets
`A1`..`A5` del Slice 1 y se reutilizan tal cual. La Niña es la personaje jugable de este nivel
(guion §1.2): si su lámina del Slice 1 no trae la pose que el bosque necesita, se regenera **con
el prompt de `A2`..`A5` del Slice 1**, no con uno nuevo.

**Autoría (CT-09, RNF-23).** Todos son **escenarios y objetos originales**. Los personajes siguen
siendo originales mientras PG-07 no llegue por escrito. Cada asset generado se reconoce en la
pantalla de créditos (Slice 1, T08).

**Transparencia.** Gemini no produce canal alfa fiable, así que se genera sobre fondo plano y se
recorta después. **El verde no sirve en este slice para nada vegetal**: los assets marcados
**Chroma magenta** se piden sobre `#FF00FF`, los marcados **Chroma verde** sobre `#00FF00`, y los
fondos de escena no llevan chroma.

---

## Bloque de estilo fijo — copiar al inicio de cada prompt

Es el bloque del Slice 1 con **una sola frase cambiada**: la de la iluminación, que allí era «luz
del fuego como única fuente» y aquí sería falsa. Todo lo demás es idéntico, palabra por palabra,
para que el arte de los dos slices sea el mismo juego.

```
ESTILO (fijo, no variar): ilustración plana 2D vectorial para videojuego educativo infantil.
Formas redondeadas y macizas, sin puntas agresivas. Contorno limpio y uniforme de 4 px en
color #1C2333. Color en planos sólidos, sin degradados complejos, sin texturas fotográficas,
sin sombreado realista: como máximo una sombra plana de un solo tono. Sin efectos de brillo
volumétrico ni destellos intensos. Iluminación diurna suave y pareja, procedente de arriba,
sin sol visible en el encuadre y sin sombras largas. Tono amable, acogedor y no amenazante,
apropiado para niños de 9 a 11 años. Ambientación prehistórica estilizada, no realista. Sin
violencia, sin sangre, sin armas, sin texto de ningún tipo dentro de la imagen, sin marcas de
agua, sin logotipos. Composición centrada y legible a tamaño pequeño, pensada para proyector y
pantallas de baja calidad: siluetas distinguibles y alto contraste entre figura y fondo.
```

## Bloque de paleta fija — copiar al inicio de cada prompt

La paleta del Slice 1 **se conserva íntegra** —los personajes y el marco de diálogo ya generados
tienen que seguir encajando— y se le añade el bloque de exteriores. Copiar las dos partes.

```
PALETA (fija, usar solo estos colores):

  — Heredada del Nivel 1 —
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

  — Exteriores del Nivel 2 —
  Cielo de día            #A8C8D8
  Follaje claro           #7FA05A
  Follaje medio           #5A7A3F
  Follaje oscuro          #3C5429
  Planta baja             #6E9B4E
  Corteza clara           #8A6B4A
  Corteza oscura          #5C4530
  Madera trabajada        #C79A5E
  Piedra fría             #7A8290
  Piedra fría oscura      #4E5561
  Metal de herramienta    #9AA3AE
```

*Las dos familias hacen el trabajo de RNF-19 sin depender del color: lo que **rueda** es cálido y
de corteza (`#8A6B4A`, `#5C4530`, `#C79A5E`); lo que **no rueda** es frío y mineral (`#7A8290`,
`#4E5561`) o vegetal (`#6E9B4E`). Aun así, la forma tiene que bastar por sí sola.*

*Para RNF-20: el texto sigue siendo `#F2E8D5` sobre el interior `#0B0E14` del marco de diálogo del
Slice 1. Sobre el bosque claro, ningún texto va directo al fondo — siempre sobre marco.*

---

## B1 · Escenario del bosque

**Traza:** RF-22, guion §6.1.1 y §6.1.2 (escenario de la fase 1).
**Chroma:** **no** — es el fondo completo de la escena.
**Entregar:** una lámina 16:9.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Fondo de escena — claro de bosque prehistórico de día. Vista lateral, plano fijo, SIN
personajes y SIN objetos sueltos en el suelo.

COMPOSICIÓN FIJA: claro amplio de bosque visto de lado. Suelo de tierra despejado en el tercio
inferior, en #6B5344, con textura mínima. A izquierda y derecha, troncos de árbol verticales en
#5C4530 que enmarcan la escena sin cerrarla. Copas de follaje en tres planos: #3C5429 al fondo,
#5A7A3F en el medio, #7FA05A al frente, todas en planos sólidos sin degradado. Al fondo, cielo
despejado #A8C8D8 visible entre los troncos. Arbustos bajos en #6E9B4E pegados a los bordes
inferiores izquierdo y derecho.

ZONA LIBRE OBLIGATORIA: el centro del suelo debe quedar completamente despejado y sin detalle:
es donde se dispersarán los objetos seleccionables y donde irá el contador. No poner nada ahí.

Relación de aspecto 16:9. Sin animales, sin fuego, sin humo, sin sendero marcado.
```

---

## B2 · Objetos del bosque — válidos y distractores

**Traza:** RF-22, RF-23, RNF-19 (los estados se distinguen por forma), guion §6.1.2.
**Chroma:** **magenta `#FF00FF`** — hay plantas verdes entre ellos.
**Entregar:** una lámina con las cuatro familias separadas y etiquetadas por posición, no por texto.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Objetos seleccionables del bosque del Nivel 2. Lámina de objetos sueltos, cada uno
completo y separado de los demás, sin superponerse.

Generar CUATRO FILAS, en este orden y sin texto de ningún tipo dentro de la imagen:

FILA 1 — TRONCOS REDONDOS (los válidos): cinco troncos cortos, cada uno una sección de tronco
vista en perspectiva de tres cuartos, con la CARA CIRCULAR bien visible y una silueta claramente
CILÍNDRICA. Corteza en #8A6B4A con vetas en #5C4530; cara circular en #C79A5E con anillos
concéntricos simples. Los cinco iguales en forma, con variación mínima de tamaño. La redondez
debe ser el rasgo más evidente del objeto, legible en silueta.

FILA 2 — PIEDRAS IRREGULARES (distractor): cuatro piedras de silueta ANGULOSA y facetada, con
esquinas planas marcadas, ninguna curva. Cuerpo en #7A8290 con facetas en sombra #4E5561.
Deben leerse como lo contrario de un cilindro.

FILA 3 — PLANTAS (distractor): tres matas de hojas anchas y flexibles, silueta abierta y
ramificada. Hojas en #6E9B4E con nervadura #3C5429, tallos delgados en #5A7A3F.

FILA 4 — HERRAMIENTAS (distractor): tres herramientas prehistóricas simples, silueta ALARGADA y
recta: un mango de madera #8A6B4A con una cabeza de piedra pulida #7A8290 atada con tira de
cuero #8C4A2F. Sin filo agresivo, sin punta, no debe leerse como arma.

REQUISITO DE ACCESIBILIDAD: las cuatro familias deben distinguirse por SILUETA en escala de
grises, sin depender del color: cilindro, faceta angulosa, mata abierta, barra alargada.

FONDO: magenta chroma key plano #FF00FF, sin sombra proyectada sobre el fondo.
```

---

## B3 · Caja de alimentos — tres estados

**Traza:** RF-25, RF-26, guion §5 y §6.1.2.
**Chroma:** verde `#00FF00`.
**Entregar:** tres versiones del mismo objeto.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Caja de alimentos de la familia, del Nivel 2.

FORMA FIJA: cesto bajo y ancho de mimbre trenzado en #B08541, de esquinas redondeadas y base
plana, con dos asas laterales de cuerda trenzada #6B5344. Dentro, asomando por el borde: frutos
redondos en #E4572E y #FFC94A y hojas verdes #6E9B4E. Misma forma y mismo tamaño en los tres
estados: solo cambia la relación con el suelo.

Generar TRES versiones de la MISMA caja, idénticas en forma:
  (1) EN EL SUELO: apoyada directamente sobre la tierra, ligeramente hundida, con una sombra
      plana ancha debajo. Debe leerse pesada y atascada.
  (2) SOBRE LOS TRONCOS: la misma caja apoyada encima de tres troncos redondos alineados en
      paralelo bajo ella, vistos de lado como círculos. Sombra plana más corta.
  (3) RODANDO: la misma caja desplazada hacia la derecha sobre los mismos tres troncos, con los
      troncos girados —los anillos de la cara circular rotados— y tres líneas cortas de
      movimiento horizontales en #F2E8D5 detrás de la caja. Sin destellos, sin polvo brillante.

FONDO: verde chroma key plano #00FF00.
```

---

## B4 · Escenario del área de trabajo

**Traza:** RF-27, guion §6.2.2 (escenario de la fase 2).
**Chroma:** **no** — es el fondo completo de la escena.
**Entregar:** una lámina 16:9.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Fondo de escena — área de trabajo al aire libre junto al refugio prehistórico. Vista
lateral, plano fijo, SIN personajes y SIN las piezas de la carretilla.

COMPOSICIÓN FIJA: explanada de tierra compacta #6B5344 que ocupa la mitad inferior. A la
izquierda, la entrada de un refugio simple de ramas y pieles: estructura triangular baja con
palos #5C4530 y pieles #A9713F, sin interior visible. A la derecha, un banco de trabajo rústico:
una losa de piedra plana #7A8290 apoyada sobre dos troncos cortos #8A6B4A. Al fondo, línea de
follaje #5A7A3F y cielo #A8C8D8. Una fogata apagada con brasas #E4572E y un hilo tenue de humo
#2E3A4F junto al refugio, pequeña y en el borde izquierdo.

ZONA LIBRE OBLIGATORIA: toda la franja central y baja del encuadre debe quedar despejada y sin
detalle: es donde se dispondrán las seis piezas y donde se ensamblará la carretilla. No poner
nada ahí.

Relación de aspecto 16:9.
```

---

## B5 · Piezas del taller — las seis de RF-27

**Traza:** RF-27, RF-28, guion §6.2.2.
**Chroma:** verde `#00FF00`.
**Entregar:** una lámina con las seis piezas separadas.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Las seis piezas del área de trabajo del Nivel 2, sueltas y separadas, sin superponerse.
Sin texto de ningún tipo dentro de la imagen.

Generar las SEIS piezas, cada una completa y aislada:
  (1) TRONCO CORTO A: sección cilíndrica de tronco vista de tres cuartos, cara circular visible.
      Corteza #8A6B4A, cara #C79A5E con anillos concéntricos. Centro SIN agujero.
  (2) TRONCO CORTO B: idéntico al anterior en forma y tamaño.
  (3) TRONCO LARGO: barra cilíndrica larga y recta, de diámetro claramente menor que el de los
      troncos cortos, en #8A6B4A con extremos #C79A5E. Debe leerse como algo que cabría por el
      centro de los troncos cortos.
  (4) TABLA: pieza rectangular plana de madera trabajada #C79A5E con vetas #8A6B4A y borde
      recto, esquinas ligeramente redondeadas.
  (5) HERRAMIENTA: mango de madera #8A6B4A con cabeza de piedra pulida y punta roma #7A8290,
      atada con tira de cuero #8C4A2F. Sin filo, no debe leerse como arma.
  (6) CAJA DE ALIMENTOS: el mismo cesto del asset B3, estado (1), sin variar su forma.

Las seis deben distinguirse por SILUETA en escala de grises: cilindro corto, cilindro corto,
barra fina larga, placa plana, mango con cabeza, cesto.

FONDO: verde chroma key plano #00FF00.
```

---

## B6 · La rueda y la carretilla — cinco estados de ensamblaje

**Traza:** RF-28, RF-29, HU-09, CU-07, guion §6.2.2. Es el asset que hace legible la secuencia.
**Chroma:** verde `#00FF00`.
**Entregar:** cinco versiones encadenadas, en el orden exacto del ensamblaje.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Los cinco estados del ensamblaje de la carretilla del Nivel 2. Vista lateral de tres
cuartos, la MISMA en los cinco. Sin personajes, sin texto dentro de la imagen.

Generar CINCO estados en secuencia, cada uno construido literalmente sobre el anterior sin
cambiar lo ya presente:
  (1) TRONCO SIN PERFORAR: un tronco corto cilíndrico, corteza #8A6B4A, cara circular #C79A5E con
      anillos concéntricos, centro macizo, SIN agujero.
  (2) RUEDA PERFORADA: el MISMO tronco, ahora con un agujero circular limpio en el centro exacto
      de la cara, oscuro #4A3B32, con el borde interior en #8A6B4A. Nada más cambia.
  (3) EJE FORMADO: DOS ruedas perforadas idénticas, separadas y paralelas, atravesadas por el
      tronco largo #8A6B4A que entra por el centro de ambas y sobresale un poco a cada lado.
      Las ruedas quedan verticales; el eje, horizontal.
  (4) TABLA MONTADA: el mismo conjunto de (3) con la tabla #C79A5E apoyada horizontalmente
      encima del eje, centrada, sobresaliendo por delante y por detrás.
  (5) CARRETILLA COMPLETA: el conjunto de (4) con el cesto de alimentos del asset B3 apoyado
      encima de la tabla, y dos varas de empuje #8A6B4A saliendo hacia atrás desde la tabla, en
      diagonal ascendente, terminadas en mango redondeado.

REQUISITO: los cinco estados deben leerse como el mismo objeto creciendo, no como cinco objetos
distintos. Mantener escala, ángulo y posición constantes entre los cinco.

FONDO: verde chroma key plano #00FF00.
```

---

## B7 · Carretilla en vista superior — cuatro orientaciones

**Traza:** RF-30, RF-31, INC-33, supuesto 8, guion §6.3.2. **Sin las cuatro orientaciones, la
lectura relativa de los bloques es incomprensible en pantalla.**
**Chroma:** **magenta `#FF00FF`** — irá sobre un tablero con vegetación.
**Entregar:** cuatro versiones del mismo objeto, rotadas.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: La carretilla del Nivel 2 vista DESDE ARRIBA, en perspectiva cenital pura (90 grados),
para el tablero del laberinto. Sin personajes, sin texto dentro de la imagen.

FORMA FIJA vista desde arriba: la tabla rectangular #C79A5E ocupa el centro; sobre ella el cesto
de alimentos #B08541 con frutos #E4572E y #FFC94A asomando. A cada lado de la tabla, una rueda
vista de canto como un rectángulo estrecho y redondeado en #8A6B4A. Por detrás, las dos varas de
empuje #8A6B4A en paralelo. El conjunto debe caber holgadamente en una casilla cuadrada.

INDICADOR DE ORIENTACIÓN OBLIGATORIO: un triángulo macizo #FFC94A con contorno #E4572E en el
frente de la carretilla, apuntando hacia donde mira. Es el único elemento que le dice al jugador
hacia dónde avanzará. Debe ser grande, inconfundible y legible a tamaño pequeño.

Generar CUATRO versiones del MISMO objeto, idénticas salvo por la rotación:
  (1) mirando hacia ARRIBA;
  (2) mirando hacia la DERECHA;
  (3) mirando hacia ABAJO;
  (4) mirando hacia la IZQUIERDA.

Cada una es la anterior rotada 90 grados en SENTIDO HORARIO. Verificar que la secuencia
(1)→(2)→(3)→(4) sea exactamente eso.

FONDO: magenta chroma key plano #FF00FF.
```

---

## B8 · Tablero del laberinto — vista superior

**Traza:** RF-30, RF-33, guion §6.3.2.
**Chroma:** **no** — es el fondo del área izquierda de la escena.
**Entregar:** una lámina cuadrada, más las piezas de obstáculo sueltas.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Tablero del laberinto del Nivel 2, VISTA CENITAL pura (90 grados). Sin la carretilla y
sin texto dentro de la imagen.

COMPOSICIÓN FIJA: rejilla cuadrada de 8 por 8 casillas, con la línea de rejilla apenas insinuada
en #6B5344 sobre suelo de sendero #8A6B4A. Alrededor de la rejilla, un borde de follaje denso
#3C5429 y #5A7A3F que cierra el tablero: fuera de la rejilla no se puede ir, y debe verse así.

En la esquina superior izquierda, sobre una casilla, el REFUGIO visto desde arriba: estructura
triangular de ramas #5C4530 y pieles #A9713F, con la abertura orientada hacia el interior de la
rejilla. Es el destino y debe ser el elemento más llamativo del tablero.

Generar además, SUELTAS y separadas al lado del tablero, las tres piezas de obstáculo, cada una
del tamaño de una casilla y distinguibles entre sí POR FORMA en escala de grises:
  (1) PIEDRA: bloque angular facetado #7A8290 con sombra #4E5561, silueta cerrada y compacta.
  (2) CURVA: tramo de sendero que gira, con dos bordes de arbusto #6E9B4E en las esquinas
      exteriores, silueta en ángulo.
  (3) PENDIENTE: franja de terreno inclinado con tres líneas paralelas de nivel en #5C4530
      cruzando la casilla en diagonal, silueta rayada.

La rejilla debe quedar limpia y con las casillas claramente separables a simple vista: es sobre
ella donde el estudiante cuenta los pasos de su secuencia.
```

---

## B9 · Bloques de instrucción y botón «Ejecutar»

**Traza:** RF-31, RF-32, RNF-19, PG-04, guion §6.3.2. **Los tres bloques tienen que distinguirse
por forma: es el criterio de verificación literal de RNF-19.**
**Chroma:** verde `#00FF00`.
**Entregar:** los tres bloques en dos estados cada uno, más el botón.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Bloques de instrucción del editor del Nivel 2 y su botón de ejecutar. Sin texto de ningún
tipo dentro de la imagen: los bloques se leen por su icono, no por una palabra.

FORMA COMÚN: ficha rectangular horizontal de esquinas redondeadas, en cuero #8C4A2F con contorno
#1C2333, con una muesca cóncava en el borde superior y una lengüeta convexa en el inferior, de
modo que las fichas se encajen visualmente unas con otras al apilarse en vertical. Todas del
mismo tamaño.

Generar TRES bloques, cada uno en DOS estados (reposo y resaltado en ejecución):
  (1) AVANZAR: flecha maciza recta apuntando HACIA ARRIBA, en #F2E8D5, centrada en la ficha.
  (2) RETROCEDER: flecha maciza recta apuntando HACIA ABAJO, en #F2E8D5, con el asta partida por
      dos líneas transversales cortas para que no se confunda con (1) al girar la lámina.
  (3) GIRAR: flecha CURVA en arco de tres cuartos de círculo, con la punta apuntando en sentido
      horario, en #F2E8D5.

  Estado REPOSO: ficha #8C4A2F, icono #F2E8D5.
  Estado RESALTADO: la MISMA ficha con el cuerpo en #FFC94A, el icono en #1C2333 y un marco
      exterior grueso continuo en #E4572E. El cambio debe leerse también sin color, por el marco.

Generar además el BOTÓN EJECUTAR: botón redondeado ancho en #A9713F con contorno #1C2333 y, en
el centro, un triángulo macizo #F2E8D5 apuntando a la derecha. Un solo botón, sin variantes: se
acciona con un clic simple.

REQUISITO DE ACCESIBILIDAD: los tres iconos deben distinguirse entre sí en escala de grises y a
tamaño pequeño. Recta arriba, recta abajo con marcas, curva en arco.

FONDO: verde chroma key plano #00FF00.
```

---

## B10 · Contador de acopio y marco de retroalimentación de fase

**Traza:** RF-24, RF-11, RF-17, RNF-19, RNF-20, guion §6.1.2.
**Chroma:** verde `#00FF00`.
**Entregar:** el marco del contador vacío, más los dos iconos de resultado.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Contador de acopio del Nivel 2 y los iconos de resultado de una selección. Sin texto
dentro de la imagen: el número y el mensaje los pone el juego.

Generar TRES elementos separados:

  (1) MARCO DEL CONTADOR: placa horizontal pequeña de hueso #F2E8D5 con borde de cuero cosido
      #8C4A2F y puntadas #6B5344, de esquinas redondeadas. A la izquierda de la placa, la
      silueta de un tronco redondo visto de frente —círculo con anillos concéntricos #C79A5E y
      corteza #8A6B4A—. El resto de la placa queda VACÍO: ahí irá «n de 5». Interior liso
      #0B0E14 con opacidad alta para sostener el contraste del texto.

  (2) ICONO DE ACEPTADO: círculo macizo #7FA05A con contorno #1C2333 y, dentro, una marca de
      verificación #F2E8D5 de trazo grueso.

  (3) ICONO DE DEVUELTO: rombo macizo #E4572E con contorno #1C2333 y, dentro, una flecha curva
      #F2E8D5 que apunta hacia atrás, indicando que el objeto regresa a su lugar. NO usar una
      equis ni una cruz: el objeto no está mal, vuelve a su sitio.

REQUISITO DE ACCESIBILIDAD: (2) y (3) deben distinguirse por FORMA además de por color —círculo
frente a rombo, marca frente a flecha— y ser legibles en escala de grises. Es el criterio de
verificación de RNF-19.

FONDO: verde chroma key plano #00FF00.
```

---

## Postproceso de los assets con chroma

1. Recortar el fondo —`#00FF00` o `#FF00FF` según lo marcado en cada asset— y exportar PNG con alfa.
2. Revisar el halo de color en los bordes; si queda, encogerlo un píxel. **El halo magenta se nota
   más que el verde sobre follaje**: revisar B2 y B7 con especial cuidado.
3. Importar como Sprite en `Assets/Game/Art/`, con el **mismo `Pixels Per Unit` que el Slice 1** —
   si difiere, la carretilla y los personajes no comparten escala.
4. Verificar RNF-19 sobre el arte final: desaturar B2, B9 y B10 y comprobar que las categorías y
   los estados siguen distinguiéndose. Si no, la corrección es de forma, no de color.
5. Verificar RNF-20 sobre el arte final, no sobre la paleta nominal: el contraste se mide en la
   imagen, y el bosque es claro.
6. Registrar cada asset en `CreditsContent.asset` (Slice 1, T08) — CT-09, RNF-23.
