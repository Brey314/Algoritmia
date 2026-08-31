# Plan técnico — Slice 3: El Río

Contrato de referencia: `claudeDocs/SPEC.md`. Este plan no rediscute arquitectura ni alcance:
los aplica. Cuando algo aquí contradiga a `SPEC.md`, gana `SPEC.md`.

Planes anteriores: [`../Slice 1/plan.md`](../Slice%201/plan.md) ·
[`../Slice 2/plan.md`](../Slice%202/plan.md). Tablero de este slice: [`todo.md`](todo.md).

**Rev. 1 — 30/08/2026.**

> ⚠️ **Precondición de slice.** Este plan **supone terminados los Slices 1 y 2**: assemblies,
> `SaveStore` con `PhaseId`, `GameFlow`, `SceneLoader`, `DialogueRunner`, `HintPolicy` por fase,
> `ILevelReporter`, menú de pausa y `LevelSummary`. Hoy ninguno de los dos tiene tareas cerradas
> y `Assets/` sigue sin código. Cada tarea dice qué pieza previa generaliza.

---

## Alcance

El Nivel 3 completo —recolección y ensamblaje por fases— más el **cierre del juego**, que es de
este slice y no del siguiente: la animación de cruce, la escena final y el regreso al menú.

| Módulo | Qué entra en este slice | Qué NO entra |
|---|---|---|
| `nivel-rio` (E) | Completo: movimiento con botones en pantalla, recolección por proximidad, inventario de cuatro, lista de cuatro tareas, ensamblaje en tres fases bloqueantes, prueba de balsa y depuración | — |
| `sistema-navegacion` (A) | Desbloqueo del Nivel 3, cierre del juego (`LevelSummary` → escena final → créditos → menú), y el cierre de RNF-02 y RNF-16 sobre las tres escenas jugables | Informe docente, eliminación de datos (Slice 4) |
| `andamiaje` (B) | Ayuda a demanda y pista tras tres fallos para recolección y ensamblaje; cinco secuencias narrativas, una de ellas **condicional**; cierre reflexivo global | — |
| `progreso-registro` (F) | Solo la **emisión** de los cuatro indicadores del Nivel 3 (OE1 §3.6.1) | Agregación y presentación docente (Slice 4) |

**Fuera de alcance explícito:** RF-46 (consulta docente) y RF-47 (eliminación de datos), que son
el Slice 4; y el nivel avanzado opcional del guion §10 —introduce presión de tiempo y contradice
CP-02—.

**Requerimientos que este slice cierra:** RF-35..RF-44, todos de prioridad **Alta**. Con ellos,
los 45 RF de prioridad Alta quedan implementados salvo RF-46 y RF-47. Es además el primer momento
en que se pueden cerrar **RNF-02** (cuyo criterio es «inspección del mapa de controles de las tres
escenas jugables») y **RNF-16** con los tres niveles existiendo de verdad.

---

## Decisiones ya tomadas que este plan aplica

Ninguna se rediscute; se listan para que las tareas no las reinventen.

- Assembly nuevo: **`Game.Levels.River`, que depende solo de `Game.Core`.** No referencia a
  `Game.Levels.Fire` ni a `Game.Levels.Wheel`, ni ellos a él (RNF-16).
- **El movimiento usa botones de dirección EN PANTALLA, accionados con clic. Nunca teclado.**
  Letra vigente en RF-35, guion §2.1 y §8.2, CU-09, HU-11 y arquitectura §1; INC-01 cerrado,
  supuesto 6. No hay excepción a RNF-02 ni a CT-06: los botones son UI accionada con clic y por
  eso están *dentro* del esquema, no fuera.
- El Nivel 3 **sí tiene lista de tareas permanente** (RF-36) — es el único que la tiene
  (INC-41). RNF-03 restringe la tarea *activa*, no cuántas se muestran.
- **Correspondencia exacta de la lista de tareas** (INC-30 cerrado, guion §8.1/§8.2, HU-11,
  CU-09 FA-5a), que este plan implementa literalmente:

  | Tarea | Se marca cuando | No se marca con |
  |---|---|---|
  | 1 · Recoger troncos | Al recoger los troncos | — |
  | 2 · Encontrar sogas | Al recoger las sogas | — |
  | 3 · Ensamblar la balsa | Al confirmar la fase de **amarre** | La fase de **base**, que no marca tarea por sí sola |
  | 4 · Colocar el mástil y la vela | Al confirmar la fase de mástil y vela | Recoger la tela ni el mástil, que entran al inventario sin marcar nada |

- **Tres fases de ensamblaje bloqueantes** (RF-40): base → amarre → mástil y vela. El panel no
  muestra todos los espacios a la vez; esa restricción es lo que lo convierte en descomposición y
  no en ensayo y error.
- La FSM `GameFlow` no cambia: el Nivel 3 es `Playing` con `LevelId.River` + fase. Añadir un
  nivel no toca el enum.
- Todo texto visible y todo parámetro ajustable jugando vive en ScriptableObject (CT-05, RNF-18).
- **Fin del juego** (INC-39): `LevelSummary` → `Narrative` (escena final, guion §9) → `Credits`
  → `MainMenu` (RF-44, RF-08, RF-12).
- Nada de `GameOver`, puntajes, cifras al estudiante ni pérdida de fase confirmada.

---

## Grafo de dependencias

```
                    R01 Game.Levels.River + exclusión (tres niveles)
                                      │
                    ┌─────────────────┴─────────────────┐
                    │                                   │
        R02 Desbloqueo N3 + granularidad de fase        │
        (RF-03, RF-04)                                  │
                    │                                   │
        R03 HintPolicy para recolección y ensamblaje    │
        (RF-13, CP-06)                                  │
                    │                                   │
        R04 Cinco NarrativeSequence del N3              │
        (RF-05, RF-06, RF-10)                           │
                    │                                   │
   ┌────────────────┼────────────────┐                  │
   │                │                │                  │
R05 TaskList   R06 Inventory    R09 RaftAssembly  ◄──────┘  adelantable
(RF-36,        + Collectible    (RF-40, RF-41)
 INC-30)       (RF-37, RF-38)         │
   │                │                 │
   └────────┬───────┘         R10 RaftValidator
            │                 (RF-42, RF-43)
   R07 Level3_River + movimiento              │
   (RF-35, INC-01)                            │
            │                                 │
   R08 Zona de construcción                   │
   (RF-39)                                    │
            └────────────────┬────────────────┘
                             │
                  R11 Panel de ensamblaje en escena
                  (RF-40..RF-43, RNF-19)
                             │
                  R12 Escena 3.2 condicional al primer fallo
                  (guion §8.4.1)
                             │
        ┌────────────────────┼────────────────────┐
        │                    │                    │
  R13 Indicadores N3   R15 RNF-19 / RNF-20   R16 RNF-02 y RNF-16
  (RF-45, §3.6.1)      en errores del N3     sobre el juego completo
        │                    │                    │
        └────────────────────┴────────────────────┘
                             │
              R14 Cruce, escena final y cierre del juego
              (RF-44, RF-12, RF-08, INC-39)
```

El orden dentro de cada fase es de abajo hacia arriba: primero la lógica pura probable sin escena,
después el cableado. **Cada tarea deja el proyecto compilando y jugable hasta donde llegó.**

**R09 y R10 no dependen de R05..R08.** Son C# plano y se pueden adelantar: el ensamblaje por
fases y su depuración son la parte con más reglas del nivel y conviene verla pasar en EditMode
antes de existir la escena.

---

## Convenciones de las tareas

Idénticas a las de los slices anteriores, se repiten para no obligar a saltar de archivo.

- **Modo de prueba:** `EditMode` = lógica pura, sin escena ni frames. `PlayMode` = cableado, UI,
  integración, Golden Path; lleva `[Category("Integration")]`. `VV` = `[Category("VisualVerification")]`.
- **Trazabilidad (CT-10):** el nombre del método de prueba cita el identificador. Ejemplo:
  `TaskList_INC30_LaFaseDeBaseNoMarcaTareaPorSiSola`.
- **Tamaño:** XS = 1 archivo · S = 1-2 · M = 3-5. Ninguna tarea de este plan supera M.
- **Corredor de pruebas (R1):** cada tarea declara si su verificación **exige** el servidor MCP de
  Unity conectado o si se sostiene a mano en el Test Runner sin perder rigor.
- **Flujo test-first por tarea:** `test-designer` → `failing-test-writer` → ver fallar →
  implementar → `resolve-diagnostics` → deduplicar.

---

# Fase 0 — Cimientos del slice

## R01: Assembly `Game.Levels.River` y exclusión con tres niveles

**Descripción.** Crear `Assets/Game/Scripts/Runtime/Levels/River/` con su `.asmdef`
`Game.Levels.River` (referencia única: `Game.Core`) y su assembly de pruebas. Con los tres niveles
existiendo, la prueba de exclusión de RNF-16 se cierra de verdad: retirar cualquiera de los tres y
comprobar que los otros dos siguen ejecutándose.

**Traza:** RNF-15, RNF-16, INC-40, `SPEC.md` §Estructura del proyecto, §Mapa de capacidades.

**Modo de prueba:** EditMode (prueba de arquitectura sobre referencias de assembly).
**Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Existe `Game.Levels.River` y su assembly de pruebas.
- [ ] Ningún assembly de nivel referencia a otro: las tres combinaciones se prueban, no una.
- [ ] `Game.Core` no referencia a ningún assembly de nivel.
- [ ] El namespace es `Game.Levels.River`, siguiendo la ruta bajo `Scripts/` y elidiendo `Runtime`.

**Verificación**
- [ ] EditMode: `Architecture_RNF16_NingunoDeLosTresNivelesReferenciaAOtro`.
- [ ] `mcp__coplay-mcp__check_compile_errors` → sin errores.
- [ ] Ningún `.meta` escrito a mano.

**Depende de:** Slice 2 W01 · **Tamaño:** XS

**Archivos**
- `Assets/Game/Scripts/Runtime/Levels/River/Game.Levels.River.asmdef`
- `Assets/Tests/EditMode/Levels/River/Game.Levels.River.Tests.asmdef`
- `Assets/Tests/EditMode/Architecture/AssemblyDependencyTests.cs` (ampliar)

---

## R02: Desbloqueo del Nivel 3 y granularidad de fase

**Descripción.** Habilitar el Nivel 3 solo con el Nivel 2 completo, y fijar qué cuenta como «fase
confirmada» en este nivel para el guardado automático (RF-04) y la recuperación tras cierre
(RNF-14). **La granularidad no la decide este plan** — ver pregunta abierta 1: el Nivel 3 usa la
palabra «fase» en dos sentidos y eso afecta al formato de datos persistidos, que es
«preguntar primero» según `SPEC.md` §Límites.

**Traza:** RF-03, RF-04, RNF-09, RNF-14, HU-11, CU-09 (precondición), CU-10, INC-27, supuestos 2, 9 y 11.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] El Nivel 3 está bloqueado mientras el Nivel 2 no esté completo, y se habilita al completarlo.
- [ ] El progreso del Nivel 3 sobrevive a un cierre forzado y retoma en la última fase confirmada,
      con la granularidad que se acuerde en la pregunta abierta 1.
- [ ] Una fase confirmada **nunca** se desconfirma: ni por fallar la prueba de balsa después, ni
      por reiniciar el nivel (RF-41, RF-43, CP-02). Comentario «por qué no» en el código.
- [ ] El perfil sigue sin campo de puntaje y sin dato alguno fuera de la lista cerrada (RNF-09).

**Verificación**
- [ ] EditMode: `LevelSelect_RF03_Nivel3BloqueadoHastaCompletarNivel2`,
      `SaveStore_RF04_ConfirmarUnaFaseDelNivel3SobreviveAlCierre`,
      `PlayerProfile_RF41_UnaFaseAprobadaNoSePierdeTrasUnaPruebaFallida`.

**Depende de:** R01, Slice 2 W02 · **Tamaño:** S

**Archivos**
- `.../Core/PhaseId.cs` (ampliar), `.../Core/PlayerProfile.cs` (ampliar)
- `Assets/Tests/EditMode/Core/PhaseProgressTests.cs` (ampliar)

---

### ✅ Checkpoint R-A — Cimientos

- [ ] Compila sin errores ni warnings nuevos (`check_compile_errors`).
- [ ] Prueba de exclusión RNF-16 con **tres niveles reales**, corrida y **declarada**.
- [ ] El menú habilita el Nivel 3 solo tras completar el Nivel 2.
- [ ] **Pregunta abierta 1 resuelta con el usuario** antes de seguir: toca datos persistidos.
- [ ] Revisado con el usuario.

---

# Fase 1 — Andamiaje del Nivel 3 (`andamiaje`)

## R03: `HintPolicy` para recolección y ensamblaje

**Descripción.** Extender el andamiaje a dos mecánicas nuevas: buscar materiales por el mapa y
colocar piezas en una fase. Lo que cuenta como «fallo consecutivo» es distinto en cada una y el
nivel no debe tener que saber cómo funciona el andamiaje.

**Traza:** RF-13, RF-10, RF-11, CP-06, RNF-03, HU-03, HU-04, CU-09, CU-10, guion §8.1, §8.3, §8.4.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] La ayuda a demanda repite la instrucción de la tarea vigente y **no muta ningún contador ni
      el inventario** (CP-06).
- [ ] En recolección, la pista tras tres fallos **orienta hacia dónde mirar sin decir dónde está
      el material**: no da coordenadas, no nombra el objeto exacto que falta ubicar.
- [ ] En ensamblaje, la pista tras tres colocaciones rechazadas **no dice qué pieza va en qué
      espacio**: describe la fase, no la solución.
- [ ] El contador de fallos es por fase; cambiar de fase lo reinicia.
- [ ] Hay una sola tarea **activa** a la vez (RNF-03), aunque la lista muestre las cuatro
      (RF-36, INC-41). Prueba que distingue las dos cosas.

**Verificación**
- [ ] EditMode: `HintPolicy_RF13_PistaDeRecoleccionNoNombraLaUbicacionDelMaterial`,
      `HintPolicy_CP06_PistaDeEnsamblajeNoDiceQuePiezaVaEnQueEspacio`,
      `HintPolicy_RF13_AyudaADemandaNoAlteraElInventario`,
      `RiverLevel_RNF03_LaListaDeCuatroTareasNoImplicaCuatroTareasActivas`.

**Depende de:** R02, Slice 2 W03 · **Tamaño:** M

**Archivos**
- `.../Scaffolding/HintPolicy.cs` (generalizar), `.../Scaffolding/GuideContent.cs` (ampliar)
- `Assets/Game/Data/River/N3_GuideContent.asset`
- `Assets/Tests/EditMode/Scaffolding/HintPolicyTests.cs` (ampliar)

---

## R04: Las cinco secuencias narrativas del Nivel 3

**Descripción.** Cinco `NarrativeSequence` sobre la escena `Narrative` ya existente. **Una de
ellas es condicional**, y ése es el único comportamiento nuevo de esta tarea: la escena 3.2 se
dispara solo la primera vez que la prueba de la balsa falla; si el jugador acierta al primer
intento, no se reproduce.

| Asset | Guion | Cuándo |
|---|---|---|
| `N3_PuenteII` | §7 | Al entrar al nivel |
| `N3_Escena31_Llegada` | §8.1 | Antes de la recolección; es donde Chispa descompone y aparece la lista |
| `N3_Escena32_PrimerIntento` | §8.4.1 | **Condicional** — solo la primera vez que falla la prueba |
| `N3_Escena33_Cruce` | §8.5 | Al superar la prueba |
| `N3_EscenaFinal` | §9 | Cierre del juego, se consume en R14 |

**Traza:** RF-05, RF-06, RF-10, RF-12, RNF-01, RNF-18, HU-02, CP-07, INC-28, INC-39.

**Modo de prueba:** EditMode (contenido y condición) + PlayMode (recorrido).
**Corredor MCP:** el PlayMode lo agradece; el EditMode no lo exige.

**Criterios de aceptación**
- [ ] Las cinco se resuelven en la **misma** escena `Narrative`, sin rama nueva en el código.
- [ ] `N3_Escena32_PrimerIntento` se dispara **exactamente una vez**, y solo si hubo un fallo:
      acertar al primer intento la salta por completo (guion §8.4.1).
- [ ] El botón de omitir aparece solo si el perfil ya vio esa escena (RF-06, INC-28).
- [ ] El cierre reflexivo de §8.5 y la escena final de §9 **no son omitibles la primera vez**
      (CP-07, RF-12).
- [ ] Ninguna oración supera veinte palabras (RNF-01) — prueba automática sobre los cinco assets.

**Verificación**
- [ ] EditMode: `NarrativeSequence_RNF01_NingunaOracionDelNivel3Supera20Palabras`,
      `NarrativeTrigger_Guion841_LaEscena32SoloSeDisparaTrasElPrimerFallo`,
      `NarrativeTrigger_Guion841_AcertarAlPrimerIntentoSaltaLaEscena32`.
- [ ] PlayMode: `NarrativeScene_RF05_ResuelveLasCincoSecuenciasDelNivel3SinRamas`.

**Depende de:** R03, Slice 1 T09/T10 · **Tamaño:** S

**Archivos**
- `Assets/Game/Data/Narrative/N3_*.asset` (5)
- `.../Scaffolding/ConditionalNarrativeTrigger.cs`
- `Assets/Tests/EditMode/Scaffolding/NarrativeContentTests.cs` (ampliar)

---

### ✅ Checkpoint R-B — Andamiaje del Nivel 3

- [ ] Las cinco escenas narrativas se recorren completas.
- [ ] La escena 3.2 aparece tras un fallo y **no aparece** si se acierta al primer intento.
- [ ] Ninguna pista del Nivel 3 resuelve la tarea (revisión de texto contra CP-06).
- [ ] Revisado con el usuario.

---

# Fase 2 — Recolección (`nivel-rio`)

## R05: `TaskList` — las cuatro tareas y su correspondencia exacta

**Descripción.** La lista de tareas permanente del Nivel 3 y, sobre todo, **cuándo se marca cada
una**. Es la tarea donde INC-30 se materializa o se pierde: la fase de base no marca ninguna
tarea, la tarea 3 la marca el **amarre**, y la tela y el mástil se recogen sin marcar nada.

**Traza:** RF-36, RF-11, RNF-03, RNF-19, **INC-30** (cerrado), HU-11, CU-09 (FA-5a), CU-10,
supuesto 11, guion §8.1 y §8.2.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] La lista tiene exactamente cuatro tareas, con los textos del guion §8.1.
- [ ] **Tarea 1** se marca al recoger los troncos; **tarea 2**, al recoger las sogas.
- [ ] **Tarea 3 se marca al confirmar la fase de amarre.** Confirmar la fase de **base** no marca
      ninguna tarea — prueba explícita y negativa, no implícita.
- [ ] **Tarea 4** se marca al confirmar la fase de mástil y vela.
- [ ] Recoger la **tela** o el **mástil** no marca tarea alguna (CU-09 FA-5a): los consume la
      tarea 4, que es de construcción.
- [ ] Una tarea marcada **nunca** se desmarca, ni tras una prueba de balsa fallida (RF-43, CP-02).
- [ ] El estado de cada tarea se señala con **más que color** (RNF-19).
- [ ] Ninguna cifra de desempeño en la lista (CP-03): «2 de 4» no aparece.

**Verificación**
- [ ] EditMode: `TaskList_RF36_Tareas1y2SeMarcanAlRecogerTroncosYSogas`,
      `TaskList_INC30_LaTarea3SeMarcaAlConfirmarElAmarreNoLaBase`,
      `TaskList_INC30_LaFaseDeBaseNoMarcaTareaPorSiSola`,
      `TaskList_CU09_RecogerLaTelaOElMastilNoMarcaTarea`,
      `TaskList_RF43_UnaTareaMarcadaNoSeDesmarcaTrasUnaPruebaFallida`.

**Depende de:** R04 · **Tamaño:** S

**Archivos**
- `.../Levels/River/RiverTask.cs`, `.../Levels/River/TaskList.cs`
- `Assets/Game/Data/River/N3_TaskListContent.asset`
- `Assets/Tests/EditMode/Levels/River/TaskListTests.cs`

---

## R06: `Inventory`, `Collectible` y recolección por proximidad

**Descripción.** Los cuatro materiales del escenario —troncos, sogas, tela y mástil—, el
inventario de capacidad exactamente cuatro y la regla de proximidad que hace aparecer el botón
«Recoger». Lógica pura: la distancia se evalúa contra una posición inyectada, no contra un
`Transform`.

**Traza:** RF-37, RF-38, RF-11, CT-05, RNF-18, HU-11, CU-09 (FA-4a, FA-6a), guion §8.2.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Hay **exactamente cuatro** materiales y la capacidad del inventario es cuatro: no hay
      objetos sobrantes ni gestión de espacio (RF-38, guion §8.2).
- [ ] El botón «Recoger» se ofrece solo dentro del radio de proximidad, que es un parámetro del
      ScriptableObject y no un literal (RF-37, CT-05, RNF-18).
- [ ] Al recoger, el objeto entra al inventario y desaparece del escenario; recogerlo dos veces es
      imposible.
- [ ] Con el inventario lleno, el sistema informa que ya cuenta con todos los materiales (CU-09 FA-4a).
- [ ] Recoger un material **no puede fallar**: no hay acción rechazada ni penalización en esta
      mecánica (CP-02).

**Verificación**
- [ ] EditMode: `Inventory_RF38_CapacidadEsCuatroYNoHayObjetosSobrantes`,
      `Collectible_RF37_ElBotonRecogerSoloApareceDentroDelRadioDeProximidad`,
      `Inventory_CU09_ConElInventarioLlenoInformaQueYaTieneTodo`,
      `Inventory_RF37_UnMaterialNoSePuedeRecogerDosVeces`.

**Depende de:** R05 · **Tamaño:** M

**Archivos**
- `.../Levels/River/Collectible.cs`, `.../Levels/River/Inventory.cs`,
  `.../Levels/River/RiverLevelConfig.cs`
- `Assets/Game/Data/River/N3_RiverLevelConfig.asset`
- `Assets/Tests/EditMode/Levels/River/InventoryTests.cs`

---

## R07: Escena `Level3_River` y movimiento con botones en pantalla

**Descripción.** El escenario en vista superior y el desplazamiento de Mamá en dos dimensiones
**con los botones de dirección mostrados en los costados de la pantalla, accionados con clic**.
Es la tarea donde INC-01 se cumple o se rompe: si aparece una sola vinculación de teclado, el
mapa de controles deja de inspeccionarse limpio y RNF-02 no se puede cerrar.

**Traza:** **RF-35**, RF-10, RNF-02, RNF-03, CT-06, **INC-01** (cerrado), supuesto 6, HU-11,
CU-09, guion §2.1 y §8.2, arquitectura §1.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.
**Corredor MCP:** **sí lo exige** para automatizarse; sin él, recorrido manual declarado.

**Criterios de aceptación**
- [ ] El personaje se desplaza en dos dimensiones dentro de los límites del escenario (RF-35).
- [ ] Los botones de dirección están **en pantalla**, en los costados izquierdo y derecho, y son
      alcanzables por raycast en las cuatro direcciones.
- [ ] **No existe ninguna vinculación de teclado** en el `.inputactions` para este nivel — prueba
      que inspecciona el asset de acciones, no solo el comportamiento (RNF-02, CT-06, INC-01).
- [ ] La clase `Input` legada no se usa en ninguna parte del assembly.
- [ ] El personaje no atraviesa los límites del escenario ni queda fuera de cámara.
- [ ] El botón de ayuda a demanda está visible durante toda la fase (RF-13).

**Verificación**
- [ ] PlayMode: `RiverScene_RF35_ElPersonajeSeDesplazaConLosBotonesEnPantalla`,
      `RiverScene_INC01_NoExisteVinculacionDeTecladoEnElMapaDeControles`,
      `RiverScene_RF35_ElPersonajeNoAtraviesaLosLimitesDelEscenario`.
- [ ] Aserción de layout: los cuatro botones dentro de pantalla y sin solaparse con la lista de
      tareas ni con el inventario.

**Depende de:** R06 · **Tamaño:** M

**Archivos**
- `.../Levels/River/RiverSceneController.cs`, `.../Levels/River/DirectionPad.cs`
- `Assets/Game/Scenes/Level3_River.unity`
- `Assets/Tests/PlayMode/Levels/River/RiverMovementTests.cs`

---

## R08: Zona de construcción

**Descripción.** La zona señalizada junto al río. Al ingresar con los cuatro materiales se abre el
panel de ensamblaje; al ingresar sin ellos, el sistema **dice cuáles faltan** y no abre nada.

**Traza:** RF-39, RF-11, RF-04, HU-11, CU-09 (FA-6a), guion §8.2.

**Modo de prueba:** PlayMode `[Category("Integration")]`.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] La zona es visible y está señalizada junto al río antes de que el jugador la necesite (RF-39).
- [ ] Ingresar con los cuatro materiales abre el panel de ensamblaje.
- [ ] Ingresar sin todos los materiales **indica cuáles faltan por su nombre**, no cuántos
      (CU-09 FA-6a, CP-03), y deja salir sin penalización (CP-02).
- [ ] Al abrirse el panel, la fase de recolección queda confirmada y guardada (RF-04).

**Verificación**
- [ ] PlayMode: `BuildZone_RF39_AbreElPanelSoloConLosCuatroMateriales`,
      `BuildZone_CU09_SinTodosLosMaterialesIndicaCualesFaltanSinCifras`.

**Depende de:** R07 · **Tamaño:** S

**Archivos**
- `.../Levels/River/BuildZone.cs`
- `Assets/Tests/PlayMode/Levels/River/BuildZoneTests.cs`

---

### ✅ Checkpoint R-C — Recolección completa

- [ ] Se recorre el mapa, se recogen los cuatro materiales y se entra a la zona de construcción.
- [ ] **Ninguna tecla mueve al personaje** — inspección del mapa de controles (RNF-02, INC-01).
- [ ] Las tareas 1 y 2 quedan marcadas; las 3 y 4 siguen sin marcar.
- [ ] La lista de tareas y el inventario son visibles todo el tiempo y no se solapan.
- [ ] Revisado con el usuario.

---

# Fase 3 — Ensamblaje y depuración (`nivel-rio`)

> **R09 y R10 no dependen de R05..R08** y se pueden adelantar. Concentran las reglas del nivel.

## R09: `RaftAssembly` — tres fases bloqueantes

**Descripción.** El panel de ensamblaje como lógica pura: tres fases sucesivas —base, amarre,
mástil y vela—, cada una con su botón de confirmación, y ninguna se habilita hasta que la anterior
sea correcta. El panel **no muestra todos los espacios a la vez**: eso es lo que lo hace
descomposición y no un rompecabezas.

**Traza:** RF-40, RF-41, RF-11, RF-17, CP-02, CP-06, RNF-18, HU-12, CU-10, guion §8.3.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Exactamente tres fases, en el orden base → amarre → mástil y vela (RF-40).
- [ ] Los espacios de una fase **no son visibles ni accesibles** antes de que la fase se habilite.
- [ ] Una colocación incorrecta dentro de una fase: «Listo» no valida, el espacio se marca y **el
      objeto vuelve al inventario**; la fase **permanece abierta** para otro intento (guion §8.4).
- [ ] Confirmar correctamente consolida la fase: no se pierde nunca después (RF-41, CP-02).
- [ ] Confirmar la fase de **amarre** marca la tarea 3; la de **base** no marca ninguna (INC-30,
      enlaza con R05).
- [ ] Intentos ilimitados, sin pantalla de derrota ni penalización (RF-18, CP-02). Comentario
      «por qué no» junto al contador de rechazos: existe para el indicador docente, no para limitar.

**Verificación**
- [ ] EditMode: `RaftAssembly_RF40_LasTresFasesSeHabilitanEnOrdenYNoAntes`,
      `RaftAssembly_RF40_LosEspaciosDeUnaFaseNoSonAccesiblesAntesDeHabilitarse`,
      `RaftAssembly_RF41_UnaFaseConfirmadaNoSePierdeEnIntentosPosteriores`,
      `RaftAssembly_INC30_ConfirmarElAmarreMarcaLaTarea3YLaBaseNoMarcaNada`,
      `RaftAssembly_CP02_NoHayLimiteDeIntentosNiPantallaDeDerrota`.

**Depende de:** R01 · **Tamaño:** M

**Archivos**
- `.../Levels/River/AssemblyPhase.cs`, `.../Levels/River/RaftSlot.cs`, `.../Levels/River/RaftAssembly.cs`
- `Assets/Game/Data/River/N3_RaftAssemblyContent.asset`
- `Assets/Tests/EditMode/Levels/River/RaftAssemblyTests.cs`

---

## R10: `RaftValidator` — prueba de balsa y depuración

**Descripción.** La validación final y su depuración. Ante un fallo: se señala **el espacio
incorrecto**, se acompaña de un mensaje que indica qué revisar, y **solo** los objetos mal
ubicados vuelven al inventario. Las fases aprobadas se conservan: el jugador reintenta sobre la
parte que falló, no sobre el ensamblaje completo. Ése es el mensaje pedagógico del nivel y hay que
dejarlo comentado.

**Traza:** RF-42, RF-43, RF-11, RF-17, RF-18, CP-02, CP-03, HU-13, CU-10 (FA-6a, FA-6b),
guion §8.4.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Ante una prueba fallida se identifica **el espacio incorrecto**, no «el ensamblaje» entero
      (RF-42, HU-13).
- [ ] El mensaje indica **qué revisar** y no cuál es la pieza correcta (CP-06, RF-17): orienta sin
      resolver.
- [ ] **Solo los objetos mal ubicados** regresan al inventario; los bien puestos permanecen
      (RF-43).
- [ ] Las fases previamente aprobadas **no se pierden** tras una prueba fallida (RF-43, guion §8.4).
- [ ] El número de pruebas es ilimitado y ninguna produce pantalla de derrota (RF-18, CU-10 FA-6b).
- [ ] Ningún mensaje lleva cifras ni juicio de valor (RF-17, CP-03).

**Verificación**
- [ ] EditMode: `RaftValidator_RF42_IdentificaElEspacioIncorrectoNoElEnsamblajeCompleto`,
      `RaftValidator_RF43_SoloLasPiezasMalUbicadasRegresanAlInventario`,
      `RaftValidator_RF43_LasFasesAprobadasSobrevivenAUnaPruebaFallida`,
      `RaftValidator_CP06_ElMensajeDiceQueRevisarNoCualEsLaPiezaCorrecta`,
      `RaftValidator_RF17_NingunMensajeContieneDigitos`.

**Depende de:** R09 · **Tamaño:** M

**Archivos**
- `.../Levels/River/RaftValidator.cs`, `.../Levels/River/ValidationResult.cs`
- `Assets/Tests/EditMode/Levels/River/RaftValidatorTests.cs`

---

## R11: Panel de ensamblaje en escena

**Descripción.** El cableado del panel: arrastres con clic sostenido, botones «Listo» y «Probar
balsa» con clic simple, resaltado del espacio incorrecto con color **e icono**, y la balsa que
crece visualmente al confirmar cada fase.

**Traza:** RF-40, RF-41, RF-42, RF-43, RNF-02, RNF-03, RNF-19, RNF-21, CT-06, HU-12, HU-13, CU-10.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] Arrastres con **clic sostenido**, soltar y confirmar con **clic** (RNF-02, CT-06).
- [ ] El panel muestra solo los espacios de la fase activa (RF-40).
- [ ] El espacio incorrecto se resalta con **color acompañado de un icono** (RNF-19, guion §8.4):
      no basta con el color.
- [ ] La balsa refleja visualmente el avance: base → con amarre → con mástil y vela (HU-12), de
      modo que el estudiante vea el progreso sin leer nada.
- [ ] La animación de completado y la de hundimiento no incluyen destellos de alta frecuencia
      (RNF-21).
- [ ] Una tarea activa a la vez (RNF-03), con las cuatro de la lista visibles (RF-36, INC-41).

**Verificación**
- [ ] PlayMode: `AssemblyPanel_RF40_MuestraSoloLosEspaciosDeLaFaseActiva`,
      `AssemblyPanel_RNF19_ElEspacioIncorrectoSeResaltaConColorEIcono`,
      `AssemblyPanel_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido`.
- [ ] VisualVerification: `AssemblyPanel_HU12_LaBalsaReflejaLasTresEtapasDeAvance`.
- [ ] Aserción de layout sobre el panel con la fase 3 abierta.

**Depende de:** R10, R08 · **Tamaño:** M

**Archivos**
- `.../Levels/River/AssemblyPanelController.cs`, `.../Levels/River/RaftView.cs`
- `Assets/Tests/PlayMode/Levels/River/AssemblyPanelTests.cs`

---

## R12: Escena 3.2, condicional al primer fallo

**Descripción.** Cablear el disparo condicional de `N3_Escena32_PrimerIntento` (R04) a la primera
prueba de balsa fallida. Si el jugador acierta al primer intento, la escena no se reproduce y se
pasa directo al cruce.

**Traza:** RF-05, RF-06, RF-11, RF-12, CP-02, CP-07, guion §8.4.1.

**Modo de prueba:** PlayMode `[Category("Integration")]`.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] La escena se reproduce tras la **primera** prueba fallida y no vuelve a reproducirse en las
      siguientes.
- [ ] Acertar al primer intento la salta por completo y va directo al cruce (guion §8.4.1).
- [ ] Tras la escena, el estado del ensamblaje es **exactamente** el que quedó: la escena narra,
      no reinicia (CP-02).
- [ ] La escena no califica el fallo como error: Chispa lo nombra «depurar» (RF-11, CP-06).

**Verificación**
- [ ] PlayMode: `RiverScene_Guion841_LaEscena32SeReproduceSoloTrasElPrimerFallo`,
      `RiverScene_Guion841_AcertarAlPrimerIntentoVaDirectoAlCruce`,
      `RiverScene_CP02_LaEscena32NoReiniciaElEnsamblaje`.

**Depende de:** R11 · **Tamaño:** S

**Archivos**
- `.../Levels/River/RiverSceneController.cs` (ampliar)
- `Assets/Tests/PlayMode/Levels/River/ConditionalNarrativeTests.cs`

---

### ✅ Checkpoint R-D — Ensamblaje completo

- [ ] La balsa se construye por las tres fases y se prueba.
- [ ] Un fallo devuelve **solo** lo mal puesto y conserva las fases aprobadas (RF-43).
- [ ] La escena 3.2 aparece tras el primer fallo y no aparece si se acierta de una.
- [ ] Ningún mensaje nombra la pieza correcta (CP-06).
- [ ] Revisado con el usuario.

---

# Fase 4 — Cierre del nivel y del juego

## R13: Emisión de los cuatro indicadores del Nivel 3

**Descripción.** `ILevelReporter` para el Nivel 3 con la definición operativa de OE1 §3.6.1. Se
persisten con el guardado de fase (RF-04). No llegan al estudiante en ninguna forma.

| Indicador | Definición para el Nivel 3 (§3.6.1, literal) |
|---|---|
| **Intentos** | Confirmaciones de fase rechazadas y pruebas de balsa fallidas |
| **Errores corregidos** | Piezas devueltas al inventario que se recolocan correctamente en el intento siguiente |
| **Pasos utilizados** | Confirmaciones de fase aceptadas, sobre un máximo de tres: base, amarre, y mástil y vela |
| **Tiempo de resolución** | Desde el inicio de la fase jugable hasta su completación, excluyendo escenas narrativas y el tiempo con la pausa abierta |

**Traza:** RF-45, RF-04, RNF-09, RNF-14, CP-03, CP-09, OE1 §3.6.1 (notas 1 a 5), INC-27, INC-30.

**Modo de prueba:** EditMode, con un doble de `ILevelReporter`. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Se emiten **exactamente cuatro** indicadores; ninguno adicional (lista cerrada, RNF-09).
- [ ] `Pasos utilizados` tiene **máximo tres**: base, amarre, mástil y vela (§3.6.1). Recolectar
      no suma pasos.
- [ ] `Errores corregidos` exige la secuencia completa: pieza devuelta → recolocada
      **correctamente** en el intento siguiente. Una devolución sin acierto posterior no cuenta.
- [ ] El tiempo excluye escenas narrativas —incluida la 3.2— y el tiempo con la pausa abierta
      (nota 1).
- [ ] Reiniciar el nivel no borra los indicadores ya registrados (nota 4).
- [ ] Ninguno llega a la UI del estudiante — prueba de exclusión (CP-03, nota 3).

**Verificación**
- [ ] EditMode: `RiverIndicators_RF45_IntentosCuentaFasesRechazadasYPruebasFallidas`,
      `RiverIndicators_RF45_PasosUtilizadosNoSuperaTres`,
      `RiverIndicators_RF45_ErrorCorregidoExigeRecolocacionCorrectaPosterior`,
      `RiverIndicators_RF07_LaPausaNoSumaTiempoDeResolucion`,
      `RiverIndicators_CP03_NingunIndicadorLlegaALaUIDelEstudiante`.

**Depende de:** R12, Slice 2 W15 · **Tamaño:** M

**Archivos**
- `.../Levels/River/RiverIndicatorCollector.cs`
- `Assets/Tests/EditMode/Levels/River/RiverIndicatorTests.cs`

---

## R14: Cruce, escena final y cierre del juego

**Descripción.** El último tramo del juego, con la forma que fija INC-39: animación de cruce →
`LevelSummary` del Nivel 3 → escena final (§9) → créditos → menú principal. Es donde Chispa nombra
las tres formas de pensar de los tres niveles y se apaga.

**Traza:** RF-44, RF-12, RF-45, RF-17, RF-08, RF-03, CP-03, CP-07, CP-10, HU-13, HU-14, CU-10,
INC-26, INC-37, INC-39, guion §8.5 y §9.

**Modo de prueba:** EditMode (texto) + PlayMode (flujo). **Corredor MCP:** el PlayMode lo exige.

**Criterios de aceptación**
- [ ] La prueba superada reproduce la animación de cruce y la escena narrativa de cierre (RF-44).
- [ ] **Cero cifras** en el resumen del Nivel 3: prueba que barre el texto renderizado buscando
      dígitos (RF-45, INC-26).
- [ ] El cierre reflexivo nombra **la descomposición y la depuración** y las relaciona con lo que
      el jugador hizo (RF-12, §8.5).
- [ ] La escena final recorre los tres niveles y sus tres facetas (§9, CP-10) y **no es omitible
      la primera vez** (CP-07).
- [ ] El flujo es exactamente `LevelSummary → Narrative → Credits → MainMenu` (INC-39): una
      prueba lo recorre entero.
- [ ] Volver al menú deja los tres niveles desbloqueados y el perfil íntegro; **no hay estado
      final irrecuperable** (RNF-13).

**Verificación**
- [ ] EditMode: `LevelSummary_RF45_ElResumenDelNivel3NoContieneNingunDigito`,
      `LevelSummary_RF12_NombraLaDescomposicionYLaDepuracion`.
- [ ] PlayMode: `GameEnding_INC39_RecorreLevelSummaryNarrativeCreditsYMainMenu`,
      `GameEnding_RF44_LaPruebaSuperadaReproduceElCruceYElCierre`.

**Depende de:** R13, R15, R16 · **Tamaño:** M

**Archivos**
- `.../Scaffolding/LevelSummaryContent.cs` (ampliar), `.../Levels/River/RiverCrossingSequence.cs`
- `Assets/Game/Data/River/N3_ResumenNivel.asset`, `Assets/Game/Data/Narrative/N3_EscenaFinal.asset`
- `Assets/Tests/PlayMode/Core/GameEndingTests.cs`

---

## R15: Doble indicador y contraste en los estados de error del Nivel 3

**Descripción.** RNF-19 se verifica «inspeccionando los estados de error de los niveles 2 y 3»:
aquí se cierra la segunda mitad. Barrer los estados de rechazo del nivel —colocación incorrecta en
una fase, prueba de balsa fallida, entrada a la zona sin materiales— y comprobar que ninguno
depende solo del color.

**Traza:** RNF-19, RNF-20, RNF-21, CN-04, guion §8.4, HU-13.

**Modo de prueba:** PlayMode `[Category("VisualVerification")]`.
**Corredor MCP:** **sí lo exige**; sin él, inspección manual sobre capturas, declarada.

**Criterios de aceptación**
- [ ] Los tres estados de error llevan **color + un segundo indicador**.
- [ ] El estado marcado / sin marcar de las cuatro tareas se distingue en escala de grises (RF-36).
- [ ] El contraste texto/fondo es ≥ 4.5:1 sobre el arte final, no sobre la paleta nominal (RNF-20).
      La escena es clara y en vista superior: es el caso más expuesto del juego.
- [ ] Ninguna animación —recolección, completado de fase, hundimiento, cruce— incluye parpadeos
      rápidos ni destellos (RNF-21).

**Verificación**
- [ ] VisualVerification: `RiverLevel_RNF19_LosEstadosDeErrorSeLeenSinColor`,
      `RiverLevel_RNF19_LaListaDeTareasSeLeeEnEscalaDeGrises`,
      `RiverLevel_RNF20_ContrasteSuficienteSobreElEscenarioClaro`,
      `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`.

**Depende de:** R11 · **Tamaño:** S

**Archivos**
- `Assets/Tests/PlayMode/Levels/River/RiverAccessibilityTests.cs`

---

## R16: Cierre de RNF-02 y RNF-16 sobre el juego completo

**Descripción.** Con las tres escenas jugables existiendo, los dos requerimientos que solo se
pueden verificar al final se cierran aquí. **No es trabajo de checkpoint: es una prueba que se
escribe**, porque su criterio de verificación es una inspección y una inspección sin prueba se
degrada en la siguiente sesión.

**Traza:** RNF-02, RNF-16, CT-06, INC-01, `SPEC.md` §Límites.

**Modo de prueba:** PlayMode `[Category("Integration")]` + EditMode (arquitectura).
**Corredor MCP:** **sí lo exige** para el PlayMode.

**Criterios de aceptación**
- [ ] El mapa de controles de `Level1_Cave`, `Level2_Forest`, `Level2_Workshop`, `Level2_Maze` y
      `Level3_River` contiene **solo** clic y clic sostenido: ninguna vinculación de teclado, de
      gamepad ni de rueda del ratón (RNF-02, CT-06).
- [ ] La clase `Input` legada no aparece en ningún assembly del proyecto.
- [ ] Retirar cualquiera de los tres niveles deja los otros dos ejecutándose (RNF-16): las tres
      combinaciones, no una.
- [ ] Ningún assembly de nivel referencia a otro.

**Verificación**
- [ ] EditMode: `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos`,
      `Architecture_RNF02_NingunAssemblyUsaLaClaseInputLegada`.
- [ ] PlayMode: `Controls_RNF02_LasCincoEscenasJugablesSoloAceptanClicYClicSostenido`.

**Depende de:** R11 · **Tamaño:** S

**Archivos**
- `Assets/Tests/PlayMode/Core/ControlSchemeTests.cs`
- `Assets/Tests/EditMode/Architecture/AssemblyDependencyTests.cs` (ampliar)

---

### ✅ Checkpoint R-E — Slice 3 completo

- [ ] **Dos recorridos completos** del Nivel 3 sin incidencias (RNF-13): puente → llegada →
      recolección → zona → base → amarre → mástil y vela → prueba → cruce → escena final →
      créditos → menú.
- [ ] Un recorrido **acertando la prueba al primer intento** (la escena 3.2 no aparece) y otro
      **fallándola** (aparece una vez).
- [ ] Cierre forzado en cada fase confirmada del Nivel 3 → retoma donde iba (RNF-14).
- [ ] **RNF-02 cerrado**: las cinco escenas jugables inspeccionadas, cero teclado (INC-01).
- [ ] **RNF-16 cerrado**: las tres combinaciones de exclusión.
- [ ] Carga de `Level3_River` < 10 s y memoria < 2 GB, **medidas** (RNF-04, RNF-05).
- [ ] Paquete < 500 MB con el arte de los tres slices (RNF-06). Es la última oportunidad de
      detectarlo antes de la entrega.
- [ ] **PG-05 verificado** en los tres niveles: el cambio de esquema de control no confundió.
- [ ] RF-35..RF-44 tienen cada uno al menos una prueba que los nombra (CT-10).
- [ ] **Golden Path del juego entero**, de la pantalla de inicio a los créditos, en 20–40 minutos
      (`SPEC.md` §Objetivo).
- [ ] Revisado con el usuario antes de abrir el Slice 4.

---

## Riesgos

| # | Riesgo | Impacto | Mitigación |
|---|---|---|---|
| **R1** | **No hay corredor de pruebas MCP.** Este slice tiene **siete tareas PlayMode/VV** y es el que cierra el juego: es donde menos conviene no poder ver una prueba fallar. | **Alto — abierto** | Cada tarea declara si lo exige. Las EditMode (R01..R06, R09, R10, R13) cubren toda la lógica del nivel y se sostienen a mano. Las PlayMode se corren a mano en Test Runner y **se declara el resultado**. |
| **R2** | **Los Slices 1 y 2 no están hechos.** Este plan generaliza piezas que aún no existen. | **Alto — abierto** | No abrir R02 antes del Checkpoint W-F del Slice 2. R01, R09 y R10 son las únicas tareas sin dependencia previa. |
| **R3** | **INC-01 es fácil de romper sin darse cuenta.** Basta una acción del `.inputactions` con binding de teclado —aunque nadie la use— para que la inspección de RNF-02 deje de salir limpia. | Alto | R07 prueba el **asset de acciones**, no solo el comportamiento; R16 lo cierra sobre las cinco escenas. Es prueba, no revisión manual. |
| **R4** | **INC-30 se pierde en la implementación.** «Confirmar fase marca tarea» es la lectura intuitiva, y es la incorrecta: la base no marca nada. | Alto | R05 lleva una prueba **negativa** explícita (`LaFaseDeBaseNoMarcaTareaPorSiSola`), no solo positivas. Sin ella el error pasa. |
| **R5** | **La palabra «fase» significa dos cosas en el Nivel 3** (ver pregunta abierta 1) y de ello depende el formato de datos persistidos, que es «preguntar primero». | Medio | Bloquea R02. Resolver con el usuario en el Checkpoint R-A antes de escribir el modelo. |
| **R6** | La vista superior es nueva: los personajes del Slice 1 están en vista lateral y no sirven aquí. | Medio | Asset `C2` genera a Mamá cenital en cuatro direcciones **con los rasgos fijos del prompt del Slice 1**, no con uno nuevo. Ver la nota de la sección de assets. |
| **R7** | El escenario del río es claro y en vista superior: es donde más fácil se incumple RNF-20. | Medio | R15 mide el contraste sobre el arte final. Todo texto va sobre marco, nunca directo al fondo. |
| **R8** | Es el último slice de contenido: si el paquete supera 500 MB, se detecta aquí y ya no hay margen. | Medio | Medir RNF-06 en el Checkpoint R-E con el arte de los tres slices importado, no estimarlo. |
| **R9** | Deriva visual entre los tres slices. | Medio | Bloque de estilo **idéntico al del Slice 2**, paleta heredada más la extensión de agua. Los personajes no se rediseñan. |

---

## Preguntas abiertas

1. **¿Qué es una «fase» del Nivel 3 para el guardado?** Los documentos usan la palabra en dos
   granularidades: CU-09 y CU-10 llaman «fase 1» a la recolección y «fase 2» al ensamblaje,
   mientras RF-40 y OE1 §3.6.1 cuentan **tres** fases de ensamblaje (base, amarre, mástil y vela).
   Para RF-04 y RNF-14 hay que elegir dónde se guarda. Propuesta: **cuatro puntos de guardado**
   —fin de recolección, base, amarre, mástil y vela—, con `Pasos utilizados` contando solo los
   tres de ensamblaje, como manda §3.6.1. **No se implementa sin confirmar:** cambiar el formato
   de datos persistidos es «preguntar primero» (`SPEC.md` §Límites). Bloquea R02.
2. **Trazado del escenario del río.** El guion pide los cuatro materiales «dispuestos
   estratégicamente», de modo que obliguen a recorrer el mapa y no estén todos en el mismo punto,
   pero no fija posiciones. R06 las propone en `N3_RiverLevelConfig.asset`; hay que validarlas
   jugando: ningún material visible desde la posición inicial, y la zona de construcción
   señalizada desde el principio.
3. **Radio de proximidad de RF-37.** Sin validar jugando. Vive en el SO para que ajustarlo no
   cueste recompilación (RNF-18), pero conviene revisarlo en el Checkpoint R-C: demasiado corto
   frustra, demasiado largo hace que el botón aparezca sin intención.
4. **PG-02 — nombre del guía.** El documento fuente del Nivel 3 lo llamaba «Bubo» y «Sabio»
   alternados; el guion adopta «Chispa». Los textos de R04 y la escena final usan Chispa. Es la
   **última** oportunidad de cambiarlo antes de la entrega: la escena final lo nombra.
5. **PG-01 — título del producto.** Los créditos (R14) y la pantalla de inicio lo exigen (RF-01).
   Sigue abierto y el juego ya está completo. Hay que cerrarlo antes de la entrega.
6. **Adelantar R09 y R10.** ¿Se ejecuta en orden de juego, o se adelanta el ensamblaje para
   descargar el riesgo de INC-30? Recomendación: adelantarlas.

---

# Assets visuales del Slice 3

Diez assets. Generador: **Gemini / Nano Banana Pro**. Los prompts están en español y se pegan
tal cual, en un solo mensaje, **sin resumirlos**.

**Documento que manda:** `claudeDocs/Direccion_de_Arte.md`, §8.3 para este nivel. Si algo se
contradice, gana la dirección de arte; si esta contradice a `SPEC.md`, gana `SPEC.md`.

**Cómo se arma un prompt.** Los mismos cinco bloques fijos del Slice 1, cambiando el de contexto
y el de paleta:

```
[1 CONTEXTO N3]  [2 ESTILO]  [3 PALETA N3]  [4 ENTREGA]  [5 PROHIBICIONES]  +  ELEMENTO
```

`[2 ESTILO]`, `[4 ENTREGA]` y `[5 PROHIBICIONES]` se copian **idénticos** a los del
`Slice 1/plan.md`, palabra por palabra. Que sean literalmente los mismos es lo que hace que los
tres niveles parezcan el mismo juego.

**Los personajes no se rediseñan.** Mamá es la personaje jugable de este nivel (guion §1.2,
CN-02) y ya existe como asset `A3` del Slice 1, en A-pose frontal. Este nivel es en **vista
superior**, así que `C2` genera su versión cenital —**reutilizando su bloque de rasgos físicos
palabra por palabra**, no inventando un personaje nuevo—. Papá, la Niña y el Niño acompañan y se
reutilizan tal cual; Chispa es `A1`.

**Autoría (CT-09, RNF-23).** Escenarios, props e interfaz **originales del proyecto**. Los
personajes reutilizados del Slice 1 son **obra derivada** de los diseños de la Familia Anonaky,
con **autorización escrita concedida** (PG-07 cerrado el 30/08/2026) y reconocimiento obligatorio
en créditos. Cada asset se registra en `CreditsContent.asset` (Slice 1, T08).

**Transparencia.** Los assets marcados **Chroma** se generan sobre verde plano `#00FF00` y se
recortan después. En este nivel el follaje es abundante, así que **todo asset con chroma que
contenga verde vegetal se pide sobre magenta `#FF00FF`**, igual que en el Slice 2; se indica en
cada uno.

---

## Bloque 1 · CONTEXTO N3 — sustituye al del Slice 1

```
CONTEXTO DEL ENCARGO
Soy diseñador de un videojuego educativo 2D hecho en Unity para estudiantes de grado cuarto de
primaria, de 9 a 11 años. El juego acompaña a una familia prehistórica en tres descubrimientos:
el fuego, la rueda y el cruce de un río. Este encargo pertenece al Nivel 3, «El Río», que
transcurre a primera hora de la mañana, con niebla baja, en la orilla de un río rodeada de
bosque húmedo.

ESTE NIVEL SE VE DESDE ARRIBA. Salvo que te diga lo contrario en un asset concreto, todo se
dibuja en VISTA CENITAL PURA, a 90 grados, como si la cámara colgara del cielo mirando al suelo.
No es una vista isométrica, no es tres cuartos y no es lateral.

Lo que necesito NO es una ilustración de escena, ni una lámina de presentación, ni un concept
art. Es un ASSET DE PRODUCCIÓN: un archivo que voy a recortar e importar a Unity como sprite,
que se verá en movimiento, superpuesto a otros elementos, a un tamaño mucho menor que el de
generación, y proyectado en pantallas de aula de baja calidad.

Tres condiciones mandan sobre cualquier consideración estética:
1. PÚBLICO INFANTIL. El agua NUNCA se representa como amenazante: no hay rápidos violentos, no
   hay espuma turbulenta, no hay oscuridad bajo la superficie, no hay peligro. Si algo sale mal
   en el juego, el tono es de contratiempo, no de catástrofe.
2. BAJO CONSUMO DE RECURSOS. Los equipos del colegio no tienen tarjeta gráfica dedicada. El arte
   es plano y simple por diseño.
3. LEGIBILIDAD ANTES QUE DETALLE. Desde arriba y a tamaño pequeño, la silueta es lo único que se
   lee. Si un detalle compite con ella, sobra.

INSTRUCCIÓN SOBRE LO QUE NO TE DIGA: sigue las secciones de abajo al pie de la letra. Donde no
te dé un dato, NO lo inventes ni lo rellenes con tu criterio: elige la opción más simple
compatible con las reglas y deja el resto vacío. No añadas objetos, personajes, adornos, texto,
fondo, marcos ni elementos decorativos que no haya pedido explícitamente. Si crees que falta
algo, omítelo: prefiero un asset incompleto a uno inventado.
```

## Bloque 3 · PALETA DEL NIVEL 3 — usar solo estos colores

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

RÍO Y BOSQUE HÚMEDO DEL NIVEL 3:
  Follaje #4E8C3F              Follaje en sombra #37662B
  Follaje claro #6FA84E        Follaje claro en sombra #54803A
  Juncos #8CA84E               Musgo #4A5C42
  Agua #3E8FA8                 Agua en sombra (centro del cauce) #2B6B80
  Banda de corriente #5AA8BF   Espuma y niebla #D6F0F5
  Roca húmeda #6B7A72          Roca húmeda en sombra #4C5850
  Suelo de tierra #8A6B4A      Suelo en sombra #6B5344
  Flores frías #B87FC4 y #7FA8E0   (NUNCA en ámbar, por la regla de acento)

ACENTO DEL NIVEL — SOLO PARA LOS MATERIALES Y LO INTERACTIVO:
  Ámbar #E8A33D               Ámbar claro #F2C46B

NEUTROS DE INTERFAZ (comunes a todo el juego):
  Marfil #F7EFE2   Marfil sombra #E0D4C0   Borde de panel #C4A882
  Carbón #3A1E18 (texto y contorno)        Carbón suave #6B5248
  Éxito #5FA842    Atención #E8A33D

REGLA DE ACENTO (crítica): el ámbar pertenece EXCLUSIVAMENTE a los materiales recolectables y a
lo interactivo —troncos utilizables, sogas, tela, mástil, zona de construcción—. Ningún helecho,
junco, roca, flor ni elemento de decorado puede llevarlo. Si el elemento que te pido no es
recolectable ni interactivo, no lleva ni una pincelada de ámbar.
```

---

## C1 · Escenario del río — vista superior

**Traza:** RF-35, RF-39, guion §8 y §8.2 (orilla del río y bosque circundante, vista superior).
**Chroma:** **no** — es el fondo completo de la escena. **Archivo:** `env_n3_rio.png`.

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [5 PROHIBICIONES]

ELEMENTO: fondo completo de escena. Orilla de un río y bosque circundante en VISTA CENITAL pura
de 90 grados. Sin personajes, sin objetos recolectables y sin interfaz.

FORMATO: rectangular 16:9 horizontal, sin croma: la imagen entera es el escenario.

COMPOSICIÓN FIJA:
- El RÍO cruza el encuadre en HORIZONTAL, ocupando la franja superior. Se construye en tres
  planos de color sólido, sin degradado: agua #3E8FA8 en el cuerpo, centro del cauce más oscuro
  #2B6B80, y dos o tres bandas de corriente #5AA8BF de anchura irregular recorriéndolo a lo
  largo. Sobre ellas, tres o cuatro arcos de espuma #D6F0F5 de trazo grueso cerca de las
  orillas. El agua está en calma: sin remolinos, sin rápidos, sin oscuridad amenazante.
- Una franja de ROCA HÚMEDA #6B7A72 con sombra #4C5850 separa el agua de la tierra a todo lo
  ancho: es la orilla.
- Los dos tercios inferiores son TERRENO TRANSITABLE: tierra #8A6B4A con sombra #6B5344,
  despejada, con manchas planas de musgo #4A5C42 dispersas.
- En los bordes izquierdo, derecho e inferior, una masa de copas de árbol vistas desde arriba,
  construida con círculos superpuestos en #4E8C3F y #6FA84E, que cierra el escenario. Fuera de
  ahí no se puede ir, y debe verse así de claro. El follaje del borde va sin contorno.
- Decorado escaso y pegado a los bordes: helechos como abanicos de tres hojas planas, juncos
  #8CA84E como líneas gruesas de punta redondeada junto al agua, y cuatro o cinco flores
  pequeñas de cinco pétalos en #B87FC4 y #7FA8E0.
- AL OTRO LADO DEL RÍO, en el borde superior: la orilla opuesta en tierra #8A6B4A y DOS COLUMNAS
  DE HUMO delgadas #D6F0F5 elevándose. Son las fogatas de la civilización, el destino. Pequeñas,
  al fondo, sin fuego visible.
- NIEBLA: una franja horizontal de #D6F0F5 al 30 por ciento de opacidad sobre el agua, de borde
  superior ondulado. Es una forma plana, no un degradado ni un difuminado.

ZONAS LIBRES OBLIGATORIAS: dejar CINCO claros despejados y sin ningún detalle —cuatro repartidos
por el terreno, uno en cada cuadrante, para los materiales recolectables, y uno más junto a la
orilla, en el centro, para la zona de construcción—. No pongas nada en esos cinco sitios.

Sin animales, sin puente, sin balsa, sin sendero marcado, sin ámbar en ninguna parte.
```

**Verificación (§17):** vista cenital pura, sin isométrica · cinco claros vacíos · ningún ámbar
en el decorado · el agua no resulta amenazante (§8.3) · el follaje del borde lee como límite.

---

## C2 · Mamá en vista superior — cuatro direcciones

**Traza:** RF-35, guion §1.1 y §8.2, CU-09, HU-11, CN-02. Obra derivada con autorización
concedida (PG-07, CT-09, RNF-23). **Chroma:** **magenta `#FF00FF`**.
**Archivo:** `char_mama_cenital_norte.png`, `_este`, `_sur`, `_oeste`.

> **Antes de generar:** abrir el prompt `A3` del Slice 1 y copiar su descripción de cabeza,
> vestuario y color **palabra por palabra** dentro de este prompt, en el lugar indicado. Mamá ya
> existe: esto es su vista cenital, no un personaje nuevo. Si los rasgos no coinciden con `A3`,
> el asset está mal y se descarta.

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: Mamá, personaje jugable del Nivel 3, en VISTA CENITAL pura de 90 grados, para
desplazarse por un escenario visto desde arriba.

FONDO: MAGENTA croma puro #FF00FF, plano y uniforme.

RASGOS FÍSICOS FIJOS: [PEGAR AQUÍ, LITERALMENTE, LA DESCRIPCIÓN DE CABEZA, VESTUARIO Y COLOR DEL
PROMPT A3 DEL SLICE 1 — mismo peinado recogido, misma piel #F2D3BC, misma túnica de piel de
leopardo #E8C07A con manchas #2B1A12, misma cuerda #5C2B22. No variar nada.]

QUÉ SE VE DESDE ARRIBA: sobre todo la coronilla y los hombros, el volumen del recogido del pelo,
la túnica vista en planta y la punta de los pies asomando. Los brazos salen a los lados del
torso y se ven escorzados. La silueta tiene que reconocerse como una persona a 64 px de alto,
sin depender del rostro.

Generar CUATRO versiones del MISMO personaje, idénticas en color, tamaño y vestuario, cambiando
solo la dirección en que camina:
  (1) hacia ARRIBA: se le ve la espalda y la coronilla; el recogido del pelo queda hacia el
      espectador.
  (2) hacia la DERECHA: perfil visto desde arriba, un hombro más adelantado.
  (3) hacia ABAJO: se le ve la cara escorzada desde arriba, con la frente ocupando la mayor parte
      del rostro.
  (4) hacia la IZQUIERDA: espejo exacto de (2).

En las CUATRO, una pierna adelantada para que se lea como paso y no como figura estática.

Las cuatro deben ocupar el mismo espacio y compartir la misma línea de centro, para que al
cambiar de dirección en el juego el personaje no salte de posición.
```

**Verificación (§17):** los rasgos coinciden con `A3` · vista cenital pura · las cuatro comparten
tamaño y centro · legible como persona a 64 px · sin ámbar en el personaje.

---

## C3 · Botones de dirección y botón «Recoger»

**Traza:** **RF-35**, RF-37, RNF-02, RNF-19, RNF-20, CT-06, **INC-01**, guion §2.1 y §8.2.
**Es el asset que materializa INC-01: el control es UI en pantalla, no teclado.**
**Chroma:** verde `#00FF00`. **Archivo:** `ui_n3_dir_arriba_reposo.png` …

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: los controles en pantalla del Nivel 3. Elementos de INTERFAZ: planos, frontales, sin
perspectiva y sin volumen. SIN TEXTO de ningún tipo.

Estos botones son el único modo de mover al personaje: no hay teclado. Tienen que ser grandes,
evidentes y fáciles de acertar con el ratón por un niño de nueve años.

BOTONES DE DIRECCIÓN: cuatro botones CIRCULARES del mismo tamaño, de piedra clara #C4A882 con
contorno #3A1E18 y un reborde interior de puntadas de cuero #6B5248. Dentro de cada uno, una
flecha maciza #3A1E18 de punta ANCHA y ROMA:
  (1) apuntando ARRIBA;  (2) apuntando ABAJO;  (3) apuntando IZQUIERDA;  (4) apuntando DERECHA.

Generar cada uno en DOS estados, en filas:
  REPOSO: cuerpo #C4A882, flecha #3A1E18, con una sombra plana inferior de 6 px en #6B5248.
  PRESIONADO: el MISMO botón desplazado hacia abajo lo que medía su sombra, SIN la sombra, con
      el cuerpo en #E8A33D y un anillo exterior continuo #3A1E18. El cambio se lee por el
      desplazamiento y el anillo, no solo por el color.

BOTÓN «RECOGER»: botón redondeado más ancho que los de dirección, en #E8A33D con contorno
#3A1E18 y, grabado dentro en #3A1E18, un icono de MANO ABIERTA vista desde arriba, con cuatro
dedos, tomando un objeto. Dos estados:
  DISPONIBLE: como se describe.
  NO DISPONIBLE: el mismo botón en #C4A882 apagado, con el icono en #6B5248 y una línea
      diagonal corta cruzando la esquina inferior derecha. La diferencia se lee sin color.

REQUISITO: área táctil generosa, mínimo equivalente a 88 por 88 px a resolución de diseño.
Legibles en escala de grises y a tamaño pequeño.
```

**Verificación (§17):** los cuatro botones son idénticos salvo la flecha · reposo y presionado se
distinguen sin color (RNF-19) · «Recoger» disponible y no disponible también · área táctil
generosa (§10.1) · sin texto.

---

## C4 · Los cuatro materiales recolectables

**Traza:** RF-37, RF-38, RNF-19, HU-11, CU-09, guion §8.2. Son **exactamente cuatro**: los que
exige RF-38, sin objetos sobrantes.
**Chroma:** verde `#00FF00`. **Archivo:** `prop_n3_troncos.png`, `_sogas`, `_tela`, `_mastil`.

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: los cuatro materiales recolectables del Nivel 3, vistos DESDE ARRIBA para el
escenario, y sus cuatro iconos para el inventario. Sin texto de ningún tipo.

Son objetos INTERACTIVOS: contorno de 7 a 9 px en #3A1E18 y color de acento ámbar presente en
todos, que es lo que le dice al estudiante que se pueden recoger.

FILA 1 — OBJETOS EN EL ESCENARIO (vista cenital, cada uno suelto y separado):
  (1) TRONCOS: un haz de TRES troncos rectos y largos, paralelos y juntos. Corteza #8A6B4A con
      vetas #6B5344 y los extremos circulares cortados en ámbar claro #F2C46B, que es la marca
      de que son madera utilizable. Silueta RECTANGULAR ALARGADA.
  (2) SOGAS: DOS cuerdas gruesas enrolladas en sendos rollos circulares planos, en fibra
      #E8A33D con el trenzado insinuado por líneas cortas #C4A882. Silueta REDONDA.
  (3) TELA: una pieza de tela clara #E0D4C0 doblada en un montón bajo, con pliegues marcados por
      líneas simples #C4A882, una esquina levantada y un ribete ámbar #E8A33D en un borde.
      Silueta BLANDA E IRREGULAR.
  (4) MÁSTIL: un palo recto, largo y delgado —claramente más fino que los troncos—, en #8A6B4A
      con una punta redondeada y una muesca tallada #F2C46B cerca del extremo superior. Silueta
      de BARRA FINA.

FILA 2 — ICONOS DE INVENTARIO: los mismos cuatro objetos, simplificados a silueta plana y
centrados dentro de una casilla cuadrada de esquinas redondeadas en marfil #F7EFE2 con borde
#C4A882. Deben reconocerse a 32 px.

REQUISITO DE ACCESIBILIDAD: los cuatro deben distinguirse por SILUETA en escala de grises y a
tamaño pequeño —haz rectangular, rollos redondos, montón blando, barra fina—. Es el criterio de
RNF-19 y lo que permite leer el inventario de un vistazo.
```

**Verificación (§17):** cuatro siluetas separables en negro sólido · los cuatro llevan ámbar y
nada del decorado lo lleva · los iconos se leen a 32 px · exactamente cuatro materiales.

---

## C5 · Lista de tareas e inventario

**Traza:** RF-36, RF-38, RNF-03, RNF-19, RNF-20, INC-30, INC-41, HU-11, guion §8.1.
**Chroma:** verde `#00FF00`. **Archivo:** `ui_n3_lista_marco.png`, `ui_n3_casilla_*`,
`ui_n3_inventario.png`.

> **Es la única lista permanente del juego.** Los niveles 1 y 2 no llevan indicador de progreso
> en pantalla: RNF-03 restringe la tarea **activa**, no cuántas se muestran, y añadir un marcador
> que ningún RF pide sería una mecánica nueva (INC-41, `Direccion_de_Arte.md` §10.1).

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: los marcos de la lista de tareas y del inventario. Elementos de INTERFAZ, planos y
frontales. SIN TEXTO dentro: las cuatro tareas las escribe el motor.

  (1) MARCO DE LA LISTA: panel VERTICAL de esquinas muy redondeadas, interior liso en marfil
      #F7EFE2, borde de cuero cosido #C4A882 con puntadas #6B5248. Dentro, CUATRO filas iguales
      separadas por una línea fina #E0D4C0. Al inicio de cada fila, una casilla cuadrada vacía
      de borde #6B5248. El resto de cada fila queda COMPLETAMENTE VACÍO: ni líneas, ni renglones,
      ni texto.

  (2) CASILLA DE TAREA PENDIENTE: la casilla cuadrada vacía, borde #6B5248 continuo y fino,
      interior marfil.

  (3) CASILLA DE TAREA COMPLETADA: la MISMA casilla, ahora rellena en #5FA842, con una marca de
      verificación #F7EFE2 de trazo grueso dentro y el borde ENGROSADO. La diferencia entre (2)
      y (3) tiene que leerse en escala de grises: casilla vacía de borde fino frente a casilla
      llena de borde grueso con marca.

  (4) TIRA DE INVENTARIO: banda horizontal con CUATRO casillas cuadradas iguales de esquinas
      redondeadas, borde #C4A882, interior marfil #F7EFE2, separadas por un espacio uniforme.
      Vacías. Exactamente cuatro, ni una más: es la capacidad del nivel.

REQUISITO: los dos marcos tienen que convivir en pantalla con los botones de dirección sin
competir visualmente. Sobrios, de borde delgado, sin adornos.
```

**Verificación (§17):** filas y casillas completamente vacías · pendiente y completada se
distinguen en escala de grises · exactamente cuatro casillas de inventario · texto oscuro sobre
fondo claro (§10.3) · contraste ≥ 4.5:1 (RNF-20).

---

## C6 · Zona de construcción señalizada

**Traza:** RF-39, RF-11, HU-11, CU-09 (FA-6a), guion §8.2.
**Chroma:** verde `#00FF00`. **Archivo:** `env_n3_zona_inactiva.png`, `_disponible.png`.

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: la zona de construcción del Nivel 3, VISTA CENITAL, para colocar sobre la orilla junto
al río. Sin personajes y sin texto.

FORMA FIJA: área RECTANGULAR de suelo despejado en roca húmeda clara #6B7A72, delimitada por un
borde de piedras planas y redondeadas #4C5850 dispuestas a intervalos regulares. En el centro, la
SILUETA HUECA de una balsa dibujada en el suelo con línea DISCONTINUA #6B5248: marca dónde se
construirá y debe leerse como un plano dibujado, nunca como un objeto real. En una esquina, dos
estacas cortas #8A6B4A clavadas en diagonal.

Generar DOS estados de la MISMA zona, idénticos en forma y posición:
  (1) INACTIVA: como se describe, en tonos apagados, sin ningún resalte.
  (2) DISPONIBLE: la misma zona con un contorno exterior CONTINUO y GRUESO en ámbar #E8A33D
      rodeando todo el rectángulo, y las piedras del borde en #F2C46B. El cambio debe leerse
      también sin color, por el grosor y la continuidad del contorno.

Sin halos, sin brillos, sin partículas: el resalte es un contorno, no una luz.
```

**Verificación (§17):** los dos estados comparten forma exacta · el cambio se lee en escala de
grises · la silueta de la balsa se lee como plano dibujado · sin halos ni glows.

---

## C7 · La balsa en sus tres estados de avance

**Traza:** RF-40, RF-41, HU-12, CU-10, guion §8.3. **Es el asset que hace visible la
descomposición:** el estudiante ve su progreso sin leer nada.
**Chroma:** verde `#00FF00`. **Archivo:** `prop_n3_balsa_base.png`, `_amarre`, `_vela`.

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: los tres estados de avance de la balsa, VISTA CENITAL desde arriba. Sin personajes,
sin agua alrededor y sin texto.

REGLA QUE MANDA SOBRE TODO: cada estado se construye LITERALMENTE sobre el anterior. Lo que ya
está no cambia de forma, de color, de tamaño ni de posición. Los tres tienen que leerse como la
MISMA balsa creciendo, no como tres balsas distintas. Misma escala, mismo ángulo y misma línea
de centro en los tres.

  (1) BASE — al confirmar la primera fase: CINCO troncos rectos, iguales y paralelos, dispuestos
      en horizontal y juntos, formando una plataforma rectangular. Corteza #8A6B4A con vetas
      #6B5344 y extremos circulares #F2C46B. Nada más. Debe leerse sólida pero incompleta: no
      hay nada que la mantenga unida.

  (2) CON AMARRE — al confirmar la segunda fase: la MISMA plataforma de cinco troncos, ahora con
      DOS sogas #E8A33D cruzándola en perpendicular a los troncos, una cerca de cada extremo,
      pasando por encima y por debajo de cada tronco de forma alterna, con un nudo visible en
      los cuatro extremos. Los troncos no se mueven ni cambian.

  (3) CON MÁSTIL Y VELA — al confirmar la tercera fase: la MISMA balsa amarrada, ahora con un
      mástil #8A6B4A clavado verticalmente en el centro —visto desde arriba, como un círculo con
      su sombra plana corta— y una vela de tela #E0D4C0 extendida desde él en forma triangular,
      con tres pliegues marcados por líneas #C4A882 y una soga #E8A33D tensándola hacia la proa.

El avance tiene que notarse de un vistazo y sin color: cinco líneas paralelas, luego cruzadas,
luego con un triángulo encima.
```

**Verificación (§17):** los tres comparten escala, ángulo y centro · el avance se lee en negro
sólido · (2) no mueve ningún tronco de (1) · sin agua ni personajes en la lámina.

---

## C8 · Panel de ensamblaje — espacios y estados

**Traza:** RF-40, RF-42, RNF-19, HU-12, HU-13, guion §8.3 y §8.4.
**Chroma:** verde `#00FF00`. **Archivo:** `ui_n3_panel_marco.png`, `ui_n3_espacio_*`.

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: el panel de ensamblaje y los estados de sus espacios. Elementos de INTERFAZ, planos y
frontales. Sin texto.

  (1) MARCO DEL PANEL: recuadro amplio de esquinas muy redondeadas, interior liso en marfil
      #F7EFE2, borde de cuero cosido #C4A882 con puntadas #6B5248. Interior COMPLETAMENTE VACÍO.
      En el borde inferior derecho, un botón redondeado ancho en #E8A33D con contorno #3A1E18 y
      dentro una marca de verificación #3A1E18: es «Listo».

  (2) ESPACIO VACÍO: contorno de silueta DISCONTINUO, a trazos, en #6B5248 sobre el interior del
      panel, con el centro vacío. Es donde va una pieza.

  (3) ESPACIO CORRECTO: el mismo contorno, ahora CONTINUO y GRUESO en #5FA842, con un CÍRCULO
      pequeño #5FA842 y una marca de verificación #F7EFE2 en la esquina superior derecha.

  (4) ESPACIO INCORRECTO: el mismo contorno, ahora CONTINUO y GRUESO en ámbar #E8A33D, con un
      ROMBO #E8A33D y un signo de admiración #3A1E18 en la esquina superior derecha. NO uses una
      equis, NO uses una cruz y NO uses rojo: la pieza no está mal, está en el sitio equivocado
      y vuelve al inventario. Es una decisión pedagógica del proyecto, no una preferencia
      estética.

REQUISITO DE ACCESIBILIDAD: (2), (3) y (4) deben distinguirse en escala de grises por el trazo
—discontinuo, continuo con círculo, continuo con rombo— y no por el color. Es el criterio de
verificación literal de RNF-19 y lo que exige HU-13.
```

**Verificación (§17):** interior del marco vacío · los tres estados se separan en escala de
grises · ningún rojo de error (§12.3) · sin equis ni cruces.

---

## C9 · La balsa en el agua — hundimiento y cruce

**Traza:** RF-42, RF-44, RNF-21, HU-13, guion §8.4 y §8.5.
**Chroma:** verde `#00FF00`. **Archivo:** `prop_n3_balsa_hundida.png`, `_cruzando.png`.

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [4 ENTREGA] [5 PROHIBICIONES]

ELEMENTO: la balsa terminada en el agua, en sus dos desenlaces. VISTA CENITAL desde arriba, con
una franja de agua bajo ella. Sin personajes y sin texto.

Partir del estado (3) del asset C7 —balsa con mástil y vela— sin variar su forma ni su color.

Generar DOS versiones:

  (1) SIN ÉXITO: la balsa INCLINADA, girada unos 20 grados, con un costado sumergido. Ese
      costado se ve a través del agua en #2B6B80, un plano más oscuro y con el contorno
      interrumpido, nunca difuminado. La vela #E0D4C0 caída y arrugada hacia el lado hundido.
      Alrededor del costado sumergido, tres o cuatro arcos de espuma #D6F0F5 de trazo fino.
      EL TONO ES DE CONTRATIEMPO, NO DE CATÁSTROFE: sin remolino, sin espuma violenta, sin
      oscuridad bajo la superficie, sin sensación de peligro. Es una imagen que un niño de nueve
      años debe poder mirar sin alarmarse.

  (2) CRUZANDO: la MISMA balsa perfectamente horizontal y estable sobre el agua #3E8FA8, con la
      vela #E0D4C0 extendida y tensa, curvada por el viento. Detrás, una estela de tres líneas
      curvas #D6F0F5 que se abren en V. Ninguna inclinación, ningún costado sumergido.

RESTRICCIÓN: sin destellos, sin partículas brillantes y sin líneas de velocidad rápidas. La
animación que se monte con estos dos estados no puede tener parpadeos de alta frecuencia
(RNF-21).
```

**Verificación (§17):** las dos parten de la misma balsa de `C7` · el hundimiento no resulta
alarmante (§8.3) · sin destellos ni parpadeos (RNF-21) · agua en planos sólidos, sin degradado.

---

## C10 · Escenario de la escena final — las fogatas

**Traza:** RF-44, RF-12, CP-10, guion §9. **Chroma:** **no** — es la ilustración fija de la
escena narrativa final. **Archivo:** `env_final_fogatas.png`.

```
[1 CONTEXTO N3] [2 ESTILO] [3 PALETA N3] [5 PROHIBICIONES]

ELEMENTO: ilustración fija de la escena final del videojuego. VISTA LATERAL, como las demás
escenas narrativas —NO cenital, es la excepción de este slice—, sin personajes y sin texto.

FORMATO: rectangular 16:9 horizontal, sin croma.

COMPOSICIÓN FIJA:
- Un camino de tierra #8A6B4A con sombra #6B5344 que entra por el borde inferior izquierdo y se
  aleja hacia el centro derecho, estrechándose con la distancia.
- A media distancia, a la DERECHA, un asentamiento pequeño: tres o cuatro refugios de ramas
  #5C2B22 y pieles #E8C07A, con DOS COLUMNAS DE HUMO elevándose desde fogatas que no se ven —el
  humo en #D6F0F5, con una base cálida #F5A62E muy pequeña—. Es el destino y debe leerse
  acogedor.
- A la IZQUIERDA y muy al fondo, empequeñecidos por la distancia, los tres escenarios que
  quedaron atrás, apenas insinuados en silueta plana y SIN contorno: la boca oscura de una cueva
  #2A2438, una masa de bosque #37662B y una franja de río #3E8FA8. Deben reconocerse sin robar
  protagonismo.
- Cielo #D6F0F5 ocupando el tercio superior, sereno, sin nubes marcadas y sin sol visible.

ZONA LIBRE OBLIGATORIA: el tercio inferior del encuadre queda despejado. Ahí va el cuadro de
diálogo del guía, que es el asset `A10` del Slice 1.

Tono de llegada y de calma, nunca de despedida triste.
```

**Verificación (§17):** vista lateral, no cenital · tercio inferior despejado para `A10` · los
tres escenarios del pasado se reconocen sin competir · tono cálido de llegada.

---

## Postproceso de los assets con chroma

1. **Verificar** contra la checklist de `Direccion_de_Arte.md` §17 y contra la línea
   «Verificación» de cada asset. Si falla una, se vuelve a generar.
2. **Recortar** el fondo —`#00FF00` o `#FF00FF` según lo marcado— y exportar PNG con alfa.
   Revisar el halo; si queda, encogerlo un píxel. `C2` va en magenta por el follaje del entorno.
3. **Nombrar** según §15.4 e **importar** con los ajustes de §15.2. **Pixels Per Unit `100`**, el
   mismo de los Slices 1 y 2: si difiere, Mamá y la balsa no comparten escala con el resto.
4. **Verificar RNF-19 sobre el arte final**: desaturar `C3`, `C4`, `C5`, `C6` y `C8` y comprobar
   que los estados y las categorías se siguen distinguiendo. Si no, la corrección es de forma, no
   de color.
5. **Verificar RNF-20 sobre el arte final**: el escenario del río es claro y en vista superior, el
   caso más expuesto del juego. La lista y el inventario van siempre sobre marco marfil, nunca
   directos sobre el fondo.
6. **Verificar RNF-21 sobre las animaciones** montadas con `C7` y `C9`, no sobre las láminas
   sueltas.
7. **Registrar** cada asset en `CreditsContent.asset` (Slice 1, T08) — CT-09, RNF-23.
