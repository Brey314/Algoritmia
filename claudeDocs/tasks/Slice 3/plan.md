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

Diez assets. Generador principal: **Gemini / Nano Banana Pro**. Los prompts están en español y se
pegan tal cual.

**Regla de uso:** copiar el bloque de estilo y el bloque de paleta **literalmente** al inicio de
cada prompt, antes de la descripción del asset. Lo que varía es solo la descripción; lo que se
repite palabra por palabra es todo lo demás.

**Los personajes no se rediseñan.** Mamá es la personaje jugable de este nivel (guion §1.2) y ya
existe como asset `A3` del Slice 1, en vista lateral. Este nivel es en **vista superior**, así que
`C2` genera su versión cenital — pero **reutilizando los rasgos físicos fijos del prompt de `A3`
palabra por palabra**, no inventando un personaje nuevo. Papá, la Niña y el Niño acompañan y se
reutilizan tal cual; Chispa es `A1`.

**Autoría (CT-09, RNF-23).** Escenarios y objetos **originales**. Los personajes siguen siendo
originales mientras PG-07 no llegue por escrito. Cada asset se reconoce en la pantalla de créditos
(Slice 1, T08).

**Transparencia.** Gemini no produce canal alfa fiable. Los assets marcados **Chroma** se generan
sobre fondo verde plano `#00FF00` y se recortan después. En este slice **ningún asset con chroma
contiene verde**, así que no hace falta el magenta que exigió el bosque del Slice 2.

---

## Bloque de estilo fijo — copiar al inicio de cada prompt

**Idéntico al del Slice 2, sin cambiar una palabra.** El Nivel 3 también transcurre de día y al
aire libre, así que el bloque sirve tal cual; que los dos slices compartan bloque es lo que hace
que el juego se vea como un solo producto.

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

Las dos familias anteriores **se conservan íntegras** —los personajes y la UI ya generados tienen
que seguir encajando— y se añade la del agua. Copiar las tres partes.

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

  — Agua del Nivel 3 —
  Agua de orilla          #5E90AC
  Agua del río            #3E6E8E
  Agua profunda           #2A4E68
  Espuma y reflejo        #C9DCE6
  Arena de la orilla      #C2A878
  Tela de la vela         #E4D8BC
  Fibra de soga           #A98C5F
```

*Para RNF-19 el agua ayuda: lo que **flota y sirve** es cálido y de madera (`#8A6B4A`, `#C79A5E`);
el río es frío (`#3E6E8E`, `#2A4E68`). Aun así, la forma tiene que bastar por sí sola.*

*Para RNF-20: el escenario es claro y en vista superior. **Ningún texto va directo sobre el
fondo** — la lista de tareas, el inventario y los mensajes van siempre sobre marco de interior
`#0B0E14` con texto `#F2E8D5`, como en los slices anteriores.*

---

## C1 · Escenario del río — vista superior

**Traza:** RF-35, RF-39, guion §8 y §8.2 (orilla del río y bosque circundante, vista superior).
**Chroma:** **no** — es el fondo completo de la escena.
**Entregar:** una lámina 16:9.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Fondo de escena — orilla de un río y bosque circundante, VISTA CENITAL pura (90 grados,
desde arriba). Sin personajes, sin objetos recolectables y sin interfaz.

COMPOSICIÓN FIJA: el río cruza el encuadre en HORIZONTAL, ocupando la franja superior: agua de
orilla #5E90AC en los bordes, agua del río #3E6E8E en el cuerpo, agua profunda #2A4E68 en el
centro del cauce, en planos sólidos sin degradado, con tres o cuatro líneas curvas de espuma
#C9DCE6 que sugieren corriente hacia la derecha. Una franja de arena #C2A878 separa el agua de
la tierra a todo lo ancho.

Los dos tercios inferiores son terreno transitable: tierra #6B5344 despejada, con manchas de
hierba #6E9B4E dispersas y planas. Alrededor del terreno, en los bordes izquierdo, derecho e
inferior, una masa de copas de árbol vistas desde arriba en #3C5429 y #5A7A3F que cierra el
escenario: fuera de ahí no se puede ir, y debe verse así.

Al otro lado del río, en el borde superior del encuadre, la orilla opuesta con tierra #6B5344 y
DOS COLUMNAS DE HUMO delgadas y grises #7A8290 elevándose: son las fogatas de la civilización,
el destino. Pequeñas y al fondo, sin fuego visible.

ZONAS LIBRES OBLIGATORIAS: dejar cuatro claros despejados y sin detalle repartidos por el
terreno —uno en cada cuadrante— para los materiales recolectables, y un claro más junto a la
arena, en el centro, para la zona de construcción. No poner nada en esos cinco sitios.

Relación de aspecto 16:9. Sin animales, sin puente, sin balsa, sin sendero marcado.
```

---

## C2 · Mamá en vista superior — cuatro direcciones

**Traza:** RF-35, guion §1.1 y §8.2, CU-09, HU-11. **Personaje original, PG-07, CT-09, RNF-23.**
**Chroma:** sí (verde).
**Entregar:** cuatro versiones del mismo personaje, una por dirección.

> **Antes de generar:** abrir el prompt `A3` del Slice 1 y copiar su bloque **RASGOS FÍSICOS
> FIJOS** palabra por palabra dentro de este prompt, en el lugar indicado. Mamá ya existe; esto es
> su vista cenital, no un personaje nuevo. Si los rasgos no coinciden con `A3`, el asset está mal.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Mamá, personaje jugable del Nivel 3, en VISTA CENITAL pura (90 grados, desde arriba),
para desplazarse por un escenario visto desde arriba. Personaje ORIGINAL, no basado en ninguna
franquicia ni personaje existente.

RASGOS FÍSICOS FIJOS: [PEGAR AQUÍ, LITERALMENTE, EL BLOQUE «RASGOS FÍSICOS FIJOS» DEL PROMPT A3
DEL SLICE 1 — mismo peinado, misma piel, misma túnica, mismos accesorios. No variar nada.]

CARÁCTER QUE DEBE LEERSE: serena y metódica, alguien que observa antes de decidir.

COMPOSICIÓN: cuerpo entero visto DESDE ARRIBA. Desde este ángulo se ven sobre todo la cabeza y
los hombros, el peinado, la túnica y la punta de los pies. La silueta debe ser reconocible como
una persona a tamaño pequeño, sin depender del rostro.

Generar CUATRO versiones del MISMO personaje, idénticas salvo por la dirección en que camina:
  (1) hacia ARRIBA (se le ve la espalda y la coronilla);
  (2) hacia la DERECHA (perfil desde arriba);
  (3) hacia ABAJO (se le ve la cara desde arriba);
  (4) hacia la IZQUIERDA (perfil desde arriba, espejo de (2)).

En las cuatro, una pierna adelantada para que se lea como paso, no como figura estática.

FONDO: verde chroma key plano #00FF00, sin sombra proyectada sobre el fondo.
```

---

## C3 · Botones de dirección y botón «Recoger»

**Traza:** **RF-35**, RF-37, RNF-02, RNF-19, RNF-20, CT-06, **INC-01**, guion §2.1 y §8.2.
**Es el asset que materializa INC-01: el control es UI en pantalla, no teclado.**
**Chroma:** sí (verde).
**Entregar:** los cuatro botones de dirección en dos estados, más el botón «Recoger» en dos estados.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Controles en pantalla del Nivel 3. Sin texto de ningún tipo dentro de la imagen.

BOTONES DE DIRECCIÓN: cuatro botones circulares del mismo tamaño, de cuero #8C4A2F con contorno
#1C2333 y un reborde interior de puntadas #A98C5F. Dentro de cada uno, una flecha maciza
#F2E8D5 de punta ancha y roma:
  (1) apuntando ARRIBA;  (2) apuntando ABAJO;  (3) apuntando IZQUIERDA;  (4) apuntando DERECHA.

Generar cada uno en DOS estados:
  REPOSO: cuerpo #8C4A2F, flecha #F2E8D5.
  PRESIONADO: el MISMO botón con el cuerpo #A9713F, la flecha #1C2333 y un anillo exterior
  grueso y continuo #FFC94A. El cambio debe leerse también SIN color, por el anillo.

BOTÓN «RECOGER»: botón redondeado ancho, de cuero #8C4A2F con contorno #1C2333, y dentro una
mano abierta estilizada #F2E8D5 en silueta simple, con una flecha corta #FFC94A que entra hacia
la palma. Generarlo en dos estados:
  DISPONIBLE: como se describe.
  NO DISPONIBLE: el mismo botón atenuado, en #6B5344 con la mano en #4E5561, y además con el
  contorno DISCONTINUO a trazos. La diferencia no puede ser solo el color (RNF-19).

REQUISITO: los cuatro botones de dirección deben ser grandes y de área generosa, pensados para
que un niño de nueve años los acierte en una pantalla de sala de sistemas. Legibles a tamaño
pequeño y en escala de grises.

FONDO: verde chroma key plano #00FF00.
```

---

## C4 · Los cuatro materiales recolectables

**Traza:** RF-37, RF-38, RNF-19, HU-11, CU-09, guion §8.2. Son **exactamente cuatro**: los que
exige RF-38, sin objetos sobrantes.
**Chroma:** sí (verde).
**Entregar:** los cuatro objetos sueltos, más su versión de icono para el inventario.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Los cuatro materiales recolectables del Nivel 3, vistos DESDE ARRIBA para el escenario, y
sus cuatro iconos para el inventario. Sin texto de ningún tipo dentro de la imagen.

FILA 1 — OBJETOS EN EL ESCENARIO (vista cenital, cada uno suelto y separado):
  (1) TRONCOS: un haz de tres troncos rectos y largos, paralelos y juntos, corteza #8A6B4A con
      vetas #5C4530 y los extremos circulares #C79A5E visibles. Silueta rectangular alargada.
  (2) SOGAS: dos cuerdas gruesas de fibra trenzada #A98C5F enrolladas en sendos rollos
      circulares planos, con el trenzado insinuado por líneas cortas #6B5344. Silueta redonda.
  (3) TELA: una pieza de tela clara #E4D8BC doblada en un montón bajo, con pliegues marcados por
      líneas simples #C2A878 y una esquina levantada. Silueta blanda e irregular.
  (4) MÁSTIL: un palo recto, largo y delgado #8A6B4A, más fino que los troncos, con una punta
      redondeada y una muesca tallada #5C4530 cerca del extremo superior. Silueta de barra fina.

FILA 2 — ICONOS DE INVENTARIO: los mismos cuatro objetos, simplificados a silueta plana y
centrados dentro de una casilla cuadrada de esquinas redondeadas en #0B0E14 con borde #A98C5F.

REQUISITO DE ACCESIBILIDAD: los cuatro deben distinguirse por SILUETA en escala de grises y a
tamaño pequeño: haz rectangular, rollos redondos, montón blando, barra fina. Es el criterio de
RNF-19 y también lo que permite que el inventario se lea de un vistazo.

FONDO: verde chroma key plano #00FF00.
```

---

## C5 · Lista de tareas e inventario

**Traza:** RF-36, RF-38, RNF-03, RNF-19, RNF-20, INC-30, INC-41, HU-11, guion §8.1.
**Chroma:** sí (verde).
**Entregar:** el marco de la lista con cuatro filas vacías, los dos estados de marca, y la tira de
inventario de cuatro casillas.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Marcos de la lista de tareas y del inventario del Nivel 3. SIN texto dentro: las cuatro
tareas las escribe el juego.

  (1) MARCO DE LA LISTA: panel vertical de esquinas redondeadas, borde de cuero cosido #8C4A2F
      con puntadas #F2E8D5, fondo interior liso #0B0E14 de opacidad alta para sostener el
      contraste de lectura. Dentro, CUATRO filas iguales separadas por una línea fina #6B5344.
      Al inicio de cada fila, una casilla cuadrada vacía de borde #A98C5F. El resto de cada fila
      queda VACÍO.

  (2) MARCA DE TAREA PENDIENTE: la casilla cuadrada vacía, borde #A98C5F continuo, interior
      #0B0E14.

  (3) MARCA DE TAREA COMPLETADA: la MISMA casilla, ahora rellena en #7FA05A, con una marca de
      verificación #F2E8D5 de trazo grueso dentro, y el borde engrosado. La diferencia entre (2)
      y (3) tiene que leerse en escala de grises: casilla vacía frente a casilla llena con marca.

  (4) TIRA DE INVENTARIO: banda horizontal con CUATRO casillas cuadradas iguales de esquinas
      redondeadas, borde #A98C5F, interior #0B0E14, separadas por un espacio uniforme. Vacías.
      Exactamente cuatro, ni una más: es la capacidad del nivel.

REQUISITO: los dos marcos deben poder convivir en pantalla con los botones de dirección sin
competir visualmente. Sobrios, de borde delgado.

FONDO: verde chroma key plano #00FF00.
```

---

## C6 · Zona de construcción señalizada

**Traza:** RF-39, RF-11, HU-11, CU-09 (FA-6a), guion §8.2.
**Chroma:** sí (verde).
**Entregar:** la zona en dos estados.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Zona de construcción del Nivel 3, vista CENITAL, para colocar sobre la arena junto al río.
Sin personajes y sin texto dentro de la imagen.

FORMA FIJA: área rectangular de suelo despejado en arena #C2A878, delimitada por un borde de
piedras planas #7A8290 dispuestas a intervalos regulares. En el centro, la SILUETA HUECA de una
balsa dibujada en el suelo con línea discontinua #6B5344: marca dónde se construirá, y debe
leerse como un plano, no como un objeto. En una esquina, dos estacas cortas de madera #8A6B4A
clavadas en diagonal.

Generar DOS estados de la MISMA zona:
  (1) INACTIVA: como se describe, en tonos apagados, sin resalte.
  (2) DISPONIBLE: la misma zona con un contorno exterior continuo y grueso en #FFC94A alrededor
      de todo el rectángulo, y las piedras del borde en #C79A5E. El cambio debe leerse también
      sin color, por el grosor y la continuidad del contorno.

FONDO: verde chroma key plano #00FF00.
```

---

## C7 · La balsa en sus tres estados de avance

**Traza:** RF-40, RF-41, HU-12, CU-10, guion §8.3. **Es el asset que hace visible la
descomposición:** el estudiante ve el progreso sin leer nada.
**Chroma:** sí (verde).
**Entregar:** tres estados encadenados, en el orden exacto de las tres fases.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Los tres estados de avance de la balsa del Nivel 3, vista CENITAL desde arriba. Sin
personajes, sin agua alrededor y sin texto dentro de la imagen.

Generar TRES estados en secuencia, cada uno construido literalmente sobre el anterior sin
cambiar ni mover nada de lo ya presente:

  (1) BASE — al confirmar la fase 1: CINCO troncos rectos, iguales y paralelos, dispuestos en
      horizontal y juntos formando una plataforma rectangular. Corteza #8A6B4A con vetas
      #5C4530, extremos circulares #C79A5E. Nada más. Debe leerse sólida pero incompleta: no hay
      nada que la mantenga unida.

  (2) CON AMARRE — al confirmar la fase 2: la MISMA plataforma de cinco troncos, ahora con DOS
      sogas de fibra trenzada #A98C5F cruzándola en perpendicular a los troncos, una cerca de
      cada extremo, pasando por encima y por debajo de cada tronco de forma alterna, con un nudo
      visible en los cuatro extremos. Los troncos no cambian de posición.

  (3) CON MÁSTIL Y VELA — al confirmar la fase 3: la MISMA balsa amarrada, ahora con un mástil
      #8A6B4A clavado verticalmente en el centro —visto desde arriba, como un círculo con su
      sombra plana corta— y una vela de tela #E4D8BC extendida desde él en forma triangular,
      con tres pliegues marcados por líneas #C2A878 y una soga #A98C5F tensándola hacia la proa.

REQUISITO: los tres estados deben leerse como la MISMA balsa creciendo, no como tres balsas
distintas. Mantener escala, ángulo y posición constantes entre los tres. El avance tiene que
notarse de un vistazo y sin color: cinco líneas paralelas, luego cruzadas, luego con un
triángulo encima.

FONDO: verde chroma key plano #00FF00.
```

---

## C8 · Panel de ensamblaje — espacios y estados

**Traza:** RF-40, RF-42, RNF-19, HU-12, HU-13, guion §8.3 y §8.4.
**Chroma:** sí (verde).
**Entregar:** el marco del panel y los tres estados de un espacio.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Panel de ensamblaje del Nivel 3 y los estados de sus espacios. Sin texto dentro de la
imagen.

  (1) MARCO DEL PANEL: recuadro amplio de esquinas redondeadas, borde de cuero cosido #8C4A2F
      con puntadas #F2E8D5, fondo interior #0B0E14 de opacidad alta. Interior completamente
      vacío. En el borde inferior derecho, un botón redondeado ancho de cuero #A9713F con
      contorno #1C2333 y, dentro, una marca de verificación #F2E8D5: es «Listo».

  (2) ESPACIO VACÍO: contorno de silueta DISCONTINUO a trazos en #A98C5F sobre el fondo del
      panel, con el interior vacío. Es donde va una pieza.

  (3) ESPACIO CORRECTO: el mismo contorno, ahora CONTINUO y grueso en #7FA05A, con un círculo
      pequeño #7FA05A y una marca de verificación #F2E8D5 en la esquina superior derecha.

  (4) ESPACIO INCORRECTO: el mismo contorno, ahora CONTINUO y grueso en #E4572E, con un ROMBO
      #E4572E y un signo de admiración #F2E8D5 en la esquina superior derecha. NO usar una equis:
      la pieza no está mal, está en el sitio equivocado y vuelve al inventario.

REQUISITO DE ACCESIBILIDAD: (2), (3) y (4) deben distinguirse en escala de grises por el trazo
—discontinuo, continuo con círculo, continuo con rombo— y no por el color. Es el criterio de
verificación literal de RNF-19 y lo que exige HU-13.

FONDO: verde chroma key plano #00FF00.
```

---

## C9 · La balsa en el agua — hundimiento y cruce

**Traza:** RF-42, RF-44, RNF-21, HU-13, guion §8.4 y §8.5.
**Chroma:** sí (verde).
**Entregar:** dos estados de la balsa terminada, sobre agua.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: La balsa terminada del Nivel 3 en el agua, en sus dos desenlaces. Vista CENITAL desde
arriba, con una franja de agua bajo ella. Sin personajes y sin texto dentro de la imagen.

Partir del estado (3) del asset C7 —balsa con mástil y vela— sin variar su forma.

Generar DOS versiones:

  (1) HUNDIÉNDOSE: la balsa INCLINADA, girada unos 20 grados y con un costado sumergido: ese
      costado se ve a través del agua en #2A4E68, más oscuro, con el contorno difuminado a
      planos. La vela #E4D8BC caída y arrugada hacia el lado hundido. Alrededor del costado
      sumergido, tres o cuatro anillos concéntricos de espuma #C9DCE6 de trazo fino. El tono
      debe ser de contratiempo, NO de catástrofe: sin remolino, sin espuma violenta, sin
      oscuridad. Es una escena que un niño de nueve años debe poder mirar sin alarmarse.

  (2) CRUZANDO: la MISMA balsa perfectamente horizontal y estable sobre el agua #3E6E8E, con la
      vela #E4D8BC extendida y tensa, curvada por el viento. Detrás, una estela de tres líneas
      curvas #C9DCE6 que se abren en V. Ninguna inclinación, ningún costado sumergido.

RESTRICCIÓN: sin destellos, sin partículas brillantes, sin líneas de velocidad rápidas. La
animación que se monte con estos dos estados no puede tener parpadeos de alta frecuencia
(RNF-21).

FONDO: verde chroma key plano #00FF00.
```

---

## C10 · Escenario de la escena final — las fogatas

**Traza:** RF-44, RF-12, CP-10, guion §9.
**Chroma:** **no** — es la ilustración fija de la escena narrativa final.
**Entregar:** una lámina 16:9.

```
[BLOQUE DE ESTILO]
[BLOQUE DE PALETA]

ASSET: Ilustración fija de la escena final del videojuego. VISTA LATERAL, como las demás escenas
narrativas —no cenital—, sin personajes y sin texto dentro de la imagen.

COMPOSICIÓN FIJA: un camino de tierra #6B5344 que entra por el borde inferior izquierdo y se
aleja hacia el centro derecho del encuadre, estrechándose con la distancia. A media distancia, a
la derecha, un asentamiento pequeño: tres o cuatro refugios de ramas y pieles #A9713F y #5C4530,
con DOS COLUMNAS DE HUMO cálido #FF8A3D y #7A8290 elevándose desde fogatas que no se ven. Es el
destino, y debe leerse acogedor.

A la izquierda y muy al fondo, empequeñecidos por la distancia, los tres escenarios que quedaron
atrás, apenas insinuados en silueta plana: la boca oscura de una cueva #1C2333, una masa de
bosque #3C5429, y una franja de río #3E6E8E. Deben reconocerse sin robar protagonismo.

Cielo #A8C8D8 ocupando el tercio superior, sereno, sin nubes marcadas y sin sol visible.

ZONA LIBRE OBLIGATORIA: el tercio inferior del encuadre queda despejado: ahí va el cuadro de
diálogo del guía (asset A10 del Slice 1).

Relación de aspecto 16:9. Tono de llegada y de calma, no de despedida triste.
```

---

## Postproceso de los assets con chroma

1. Recortar el verde `#00FF00` y exportar PNG con alfa.
2. Revisar el halo verde en los bordes; si queda, encogerlo un píxel.
3. Importar como Sprite en `Assets/Game/Art/`, con el **mismo `Pixels Per Unit` que los Slices 1
   y 2** — si difiere, Mamá y la balsa no comparten escala con el resto del juego.
4. Verificar RNF-19 sobre el arte final: desaturar `C3`, `C4`, `C5` y `C8` y comprobar que los
   estados y las categorías se siguen distinguiendo. Si no, la corrección es de forma, no de color.
5. Verificar RNF-20 sobre el arte final: el escenario del río es claro y en vista superior, el
   caso más expuesto del juego. La lista de tareas y el inventario van siempre sobre marco
   `#0B0E14`, nunca directos al fondo.
6. Verificar RNF-21 sobre las animaciones montadas con `C7` y `C9`, no sobre las láminas sueltas.
7. Registrar cada asset en `CreditsContent.asset` (Slice 1, T08) — CT-09, RNF-23.
