# Plan técnico — Slice 4: Progreso y registro

Contrato de referencia: `claudeDocs/SPEC.md`. Este plan no rediscute arquitectura ni alcance:
los aplica. Cuando algo aquí contradiga a `SPEC.md`, gana `SPEC.md`.

Planes anteriores: [`../Slice 1/plan.md`](../Slice%201/plan.md) ·
[`../Slice 2/plan.md`](../Slice%202/plan.md) · [`../Slice 3/plan.md`](../Slice%203/plan.md).
Tablero de este slice: [`todo.md`](todo.md).

**Rev. 1 — 30/08/2026.**

> ⚠️ **Precondición de slice.** Supone terminados los Slices 1, 2 y 3: los tres niveles emiten ya
> sus cuatro indicadores vía `ILevelReporter` y el perfil los persiste. **Este módulo no produce
> un solo indicador nuevo: los agrega y los presenta.** Si un indicador no existe al llegar aquí,
> el problema está en el slice que debía emitirlo, no en éste.

---

## Alcance

`progreso-registro` (módulo F) completo, y con él los dos últimos RF de prioridad Alta del
proyecto.

| Entrega | Qué entra | Qué NO entra |
|---|---|---|
| Resumen de fin de nivel | **Consolidación y verificación transversal** del resumen narrativo sin cifras que los tres slices ya construyeron (RF-45, RF-17, CP-03) | Volver a implementarlo — ver la nota de abajo |
| `TeacherReport` (RF-46) | Enumeración de perfiles del equipo, agregación de los cuatro indicadores **por nivel y por fase**, presentación con cifras, acceso desde el menú principal | Exportación a archivo, gráficas, comparación entre estudiantes: no están en ningún RF |
| Eliminación de datos (RF-47) | Borrado irreversible con confirmación explícita, sobre `Datos/` **y** la ruta de respaldo, sin residuos, probado en los dos escenarios de INC-34 | — |
| Cierre del proyecto | Barrido final de CP-03 y RNF-09 sobre el juego entero; verificación de presupuestos y ejecución portable | — |

**Sobre el resumen de fin de nivel.** Lo pediste como alcance de este slice, y hay que decir una
cosa antes de planearlo: **ya está planeado tres veces** —Slice 1 `T18`, Slice 2 `W16`, Slice 3
`R14`—, una por nivel, porque cada uno cierra con el suyo. Replanearlo aquí sería duplicar. Lo que
sí falta, y es trabajo real, es lo que **solo se puede hacer cuando existen los tres**: unificar
el contenido en un único `LevelSummaryContent` parametrizado si los tres slices lo dejaron
divergente, y correr un barrido de dígitos sobre los tres resúmenes a la vez. Eso es `P02`, y es
lo único que este plan pone bajo RF-45 del lado del estudiante. La **emisión** por nivel ya está
en T17 / W15 / R13.

**Fuera de alcance:** cualquier mecánica de juego. Este módulo no toca los niveles.

**Requerimientos que este slice cierra:** RF-46 y RF-47. Con ellos, **los 45 RF de prioridad Alta
quedan implementados** y el proyecto tiene los 47 cubiertos salvo RF-06 (Media) y RF-21 (Baja),
que son los dos únicos que el trabajo de grado admite dejar fuera.

---

## Decisiones ya tomadas que este plan aplica

- **El assembly es `Game.Reporting`, y ya está decidido.** `SPEC.md` §Estructura del proyecto lo
  nombra en la lista de assemblies y `Scripts/Runtime/Reporting/` en la de carpetas. No hay que
  elegir entre «vive en `Game.UI`» o «assembly propio»: el contrato ya lo resolvió.
- **`Game.Reporting` depende solo de `Game.Core`** — ver la nota siguiente, que es la única
  decisión de diseño que este plan añade.
- Las cifras existen **únicamente** en `TeacherReport` (RF-46). No hay `ScoreManager` ni campo de
  puntaje en el guardado (CP-03, INC-26).
- Los indicadores se presentan **por nivel y por fase** (RF-45, RF-46, INC-35 cerrado).
- La eliminación borra `Datos/` **y** la ruta de respaldo, y la prueba de RNF-11 corre en los dos
  escenarios (INC-34 cerrado, supuesto 1).
- La lista de datos persistidos es **cerrada**: nombre o alias, nivel alcanzado, fases confirmadas
  y los cuatro indicadores. Nada más (RNF-09, INC-27).
- El estado `TeacherReport` **ya existe en el enum** de `GameFlow` desde el Slice 1 (`T03`),
  declarado y sin destino. Este slice le da destino; no añade un estado.
- Textos del resumen narrativo en ScriptableObject, nunca literales (CT-05, RNF-18).

### La única decisión de diseño que este plan añade

El mapa de capacidades de `SPEC.md` dice que el módulo F «depende de A, C, D, E». Eso es una
dependencia **de datos** —F consume lo que los niveles emiten—, no una referencia de assembly. Si
`Game.Reporting` referenciara a `Game.Levels.Fire`, retirar el Nivel 1 rompería el informe
docente y **la prueba de exclusión de RNF-16 dejaría de pasar**, que es justo lo que esa prueba
existe para impedir.

Por eso: **`Game.Reporting` referencia solo a `Game.Core`** y lee los indicadores del
`PlayerProfile`, indexados por `LevelId` y `PhaseId`. Nunca conoce una clase de nivel. `P01` lo
prueba. Si esta lectura no es la que ustedes tenían en mente, hay que decirlo antes de `P01`,
porque cambia la frontera del módulo.

---

## Grafo de dependencias

```
              P00 Corredor de pruebas MCP conectado
                                │
              P01 Game.Reporting y su frontera
              (RNF-16, SPEC §Estructura)
                                │
        ┌───────────────────────┼───────────────────────┐
        │                       │                       │
   P02 Resumen sin cifras   P03 ProfileRepository       │
   (RF-45, RF-17, CP-03)    (RF-46, CU-11)              │
        │                       │                       │
        │              P04 IndicatorReport               │
        │              por nivel y por fase              │
        │              (RF-46, INC-35)                   │
        │                       │                       │
        │              P05 Escena TeacherReport     P08 ProfileEraser
        │              (RF-46, CU-11)               (RF-47, RNF-11, INC-34)
        │                       │                       │
        │              P06 Tabla de los cuatro      P09 Confirmación
        │              indicadores (RF-46)          explícita (CU-12)
        │                       │                       │
        │              P07 Acceso desde el menú     P10 Prueba de residuos
        │              (RF-46, CU-11 FA-2a)         en disco real (RNF-11)
        │                       │                       │
        └───────────────────────┴───────────┬───────────┘
                                            │
                        P11 Cierre de CP-03 y RNF-09 sobre el juego
                                            │
                        P12 Presupuestos y ejecución portable
                        (RNF-04..RNF-08, RNF-13)
```

Primero la lógica pura probable sin escena, después el cableado. **Cada tarea deja el proyecto
compilando y jugable hasta donde llegó.**

**P08 no depende de P03..P07.** El borrado es lógica de sistema de archivos y se puede adelantar:
es la parte con consecuencias irreversibles y conviene tener sus pruebas verdes antes de que haya
una UI que lo dispare.

---

## Convenciones de las tareas

Idénticas a las de los slices anteriores.

- **Modo de prueba:** `EditMode` = lógica pura, sin escena ni frames. `PlayMode` = cableado, UI,
  integración; lleva `[Category("Integration")]`. `VV` = `[Category("VisualVerification")]`.
- **Trazabilidad (CT-10):** el nombre del método de prueba cita el identificador. Ejemplo:
  `ProfileEraser_RNF11_NoDejaResiduosEnNingunaDeLasDosRutas`.
- **Tamaño:** XS = 1 archivo · S = 1-2 · M = 3-5. Ninguna tarea de este plan supera M.
- **Corredor de pruebas:** este slice **empieza** por conectarlo (`P00`). Aun así, cada tarea
  declara si lo exige, por si `P00` no se resuelve.
- **Flujo test-first por tarea:** `test-designer` → `failing-test-writer` → ver fallar →
  implementar → `resolve-diagnostics` → deduplicar.

---

# Fase 0 — Cimientos del slice

## P00: Confirmar o instalar el corredor de pruebas MCP de Unity

**Descripción.** Tarea previa, sin código de producción. `run_unity_tests`,
`get_unity_compilation_result` y `unity_play_control` vienen de un servidor MCP de Unity que hoy
no está instalado; el único servidor configurado es `coplay-mcp`, que no los trae. Este slice es
el que cierra el proyecto y el que menos margen tiene para pruebas declaradas a mano: se abre
resolviéndolo.

**Traza:** `SPEC.md` §Comandos, `CLAUDE.md` §Comandos, riesgo R1 de los tres slices anteriores.

**Modo de prueba:** no aplica — la verificación es que la herramienta responde.

**Criterios de aceptación**
- [ ] `run_unity_tests` está disponible y ejecuta una suite EditMode existente devolviendo
      resultados.
- [ ] `get_unity_compilation_result` devuelve el estado de compilación.
- [ ] Si **no** se instala: queda escrito aquí que las siete tareas `MCP` de este slice se corren
      a mano en el Test Runner y **se declara el resultado**, nunca se da por hecho. La decisión
      se toma, no se deja implícita.
- [ ] Añadir el servidor a la configuración MCP **no** es añadir un paquete a
      `Packages/manifest.json`: si el servidor exigiera un paquete de Unity, eso sí es
      «preguntar primero» (`SPEC.md` §Límites).

**Verificación**
- [ ] Correr la suite EditMode de `Game.Core` con `run_unity_tests` y pegar el resultado.
- [ ] Actualizar la nota de `SPEC.md` §Comandos y la de `CLAUDE.md` §Comandos, que hoy dicen que
      no hay corredor conectado.

**Depende de:** ninguna · **Tamaño:** XS (sin código)

**Archivos**
- Configuración MCP (fuera del repositorio)
- `claudeDocs/SPEC.md` §Comandos, `CLAUDE.md` §Comandos — actualizar la nota

---

## P01: Assembly `Game.Reporting` y su frontera

**Descripción.** Crear `Assets/Game/Scripts/Runtime/Reporting/` con su `.asmdef` `Game.Reporting`
(referencia única: `Game.Core`) y su assembly de pruebas. La prueba importante es negativa:
**`Game.Reporting` no referencia a ningún assembly de nivel**, porque si lo hiciera, retirar un
nivel rompería el informe y RNF-16 dejaría de pasar.

**Traza:** RNF-15, RNF-16, `SPEC.md` §Estructura del proyecto, §Mapa de capacidades.

**Modo de prueba:** EditMode (prueba de arquitectura). **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Existe `Game.Reporting` y su assembly de pruebas.
- [ ] `Game.Reporting` referencia **solo** a `Game.Core`: ni `Fire`, ni `Wheel`, ni `River`.
- [ ] `Game.Core` no referencia a `Game.Reporting`.
- [ ] Retirar cualquiera de los tres niveles deja `TeacherReport` compilando y ejecutándose, con
      los datos de los niveles restantes (RNF-16). Prueba explícita, no razonamiento.
- [ ] El namespace es `Game.Reporting`, siguiendo la ruta bajo `Scripts/` y elidiendo `Runtime`.

**Verificación**
- [ ] EditMode: `Architecture_RNF16_ReportingNoReferenciaANingunAssemblyDeNivel`,
      `Architecture_RNF16_RetirarUnNivelNoRompeElInformeDocente`.
- [ ] `mcp__coplay-mcp__check_compile_errors` → sin errores.
- [ ] Ningún `.meta` escrito a mano.

**Depende de:** P00, Slice 3 R01 · **Tamaño:** XS

**Archivos**
- `Assets/Game/Scripts/Runtime/Reporting/Game.Reporting.asmdef`
- `Assets/Tests/EditMode/Reporting/Game.Reporting.Tests.asmdef`
- `Assets/Tests/EditMode/Architecture/AssemblyDependencyTests.cs` (ampliar)

---

### ✅ Checkpoint P-A — Cimientos

- [ ] `run_unity_tests` responde, o queda **escrito** que se corre a mano y se declara.
- [ ] `Game.Reporting` existe y no referencia a ningún nivel.
- [ ] La exclusión de RNF-16 sigue pasando con el módulo de informe presente.
- [ ] Revisado con el usuario.

---

# Fase 1 — El lado del estudiante: resumen sin cifras

## P02: `LevelSummaryContent` unificado y barrido transversal de cifras

**Descripción.** Los tres slices anteriores crearon el resumen de cada nivel por separado. Aquí se
hace lo único que no se podía hacer antes: comprobar que **los tres a la vez** no contienen una
sola cifra, y unificar el contenido en un `LevelSummaryContent` parametrizado si quedaron
divergentes. **No es una pantalla nueva**: reutiliza el `DialogueRunner` y el marco de diálogo del
andamiaje (asset `A10` del Slice 1). Si esta tarea acaba creando una vista propia, algo se
entendió al revés.

**Traza:** RF-45, RF-17, RF-12, CP-03, CP-07, RNF-01, RNF-18, HU-14, **INC-26**, `SPEC.md`
§Estrategia de pruebas (invariante 3).

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] **Cero dígitos** en los tres resúmenes: una prueba parametrizada que barre el texto
      renderizado de los tres niveles buscando `\d`. Es el punto donde HU-14 ya coló una cifra
      (INC-26) y donde volverá a colarse si nadie la busca.
- [ ] Los tres resúmenes se resuelven con **el mismo** componente y el mismo marco de diálogo: no
      hay una vista por nivel.
- [ ] Ningún resumen califica el desempeño ni emite juicio de valor (RF-17, CP-03): describe lo
      que el estudiante hizo.
- [ ] Ninguna oración supera veinte palabras (RNF-01).
- [ ] El cierre reflexivo sigue sin ser omitible la primera vez en los tres niveles (CP-07).
- [ ] **Ninguna clase del proyecto se llama `Score`, `Points` o equivalente** — prueba de
      exclusión por nombre, con comentario «por qué no»: la razón es CP-03, no técnica.

**Verificación**
- [ ] EditMode: `LevelSummary_RF45_NingunResumenDeLosTresNivelesContieneUnDigito`,
      `LevelSummary_RF17_NingunResumenEmiteJuicioDeValor`,
      `LevelSummary_CP03_NoExisteClaseDePuntajeEnElProyecto`,
      `LevelSummaryContent_RNF01_NingunaOracionSupera20Palabras`.

**Depende de:** P01, Slice 3 R14 · **Tamaño:** S

**Archivos**
- `.../Scaffolding/LevelSummaryContent.cs` (unificar)
- `Assets/Game/Data/*/N*_ResumenNivel.asset` (revisar los tres)
- `Assets/Tests/EditMode/Scaffolding/LevelSummaryTests.cs` (ampliar)

---

### ✅ Checkpoint P-B — El estudiante no ve cifras

- [ ] Los tres resúmenes barridos: cero dígitos, cero juicios de valor.
- [ ] Un solo componente resuelve los tres, sobre el marco de diálogo del andamiaje.
- [ ] Revisado con el usuario.

---

# Fase 2 — El lado del docente: `TeacherReport`

## P03: `ProfileRepository` — los perfiles del equipo

**Descripción.** Enumerar los perfiles registrados en el equipo leyendo `Datos/` y, si aplica, la
ruta de respaldo. Lógica pura contra un `IFileSystem` inyectado; el mismo que usa `SaveStore`
desde el Slice 1.

**Traza:** RF-46, RF-02, RNF-07, RNF-09, CU-11 (pasos 2 y 3, FA-2a), HU-16, INC-34, supuesto 1.

**Modo de prueba:** EditMode con doble de sistema de archivos. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Devuelve los perfiles de **las dos rutas** sin duplicar uno que exista en ambas (INC-34).
- [ ] Sin perfiles registrados, devuelve una lista vacía y **no lanza**: la UI lo traduce a un
      mensaje y vuelve al menú (CU-11 FA-2a).
- [ ] Un archivo corrupto o ilegible no tumba la enumeración: se omite y se registra la
      advertencia (RNF-13, sin estados irrecuperables).
- [ ] No expone ningún dato fuera de la lista cerrada de RNF-09.

**Verificación**
- [ ] EditMode: `ProfileRepository_RF46_EnumeraLosPerfilesDeLasDosRutasSinDuplicar`,
      `ProfileRepository_CU11_SinPerfilesDevuelveListaVaciaYNoLanza`,
      `ProfileRepository_RNF13_UnArchivoCorruptoNoTumbaLaEnumeracion`.

**Depende de:** P01 · **Tamaño:** S

**Archivos**
- `.../Reporting/ProfileRepository.cs`
- `Assets/Tests/EditMode/Reporting/ProfileRepositoryTests.cs`

---

## P04: `IndicatorReport` — agregación por nivel y por fase

**Descripción.** El núcleo del informe: tomar los indicadores persistidos en un perfil y
organizarlos **por nivel y por fase** (INC-35), con la lista cerrada de cuatro. Lógica pura, sin
formato ni UI.

**Traza:** RF-45, RF-46, RNF-09, CP-09, OE1 §3.6.1 (notas 1 a 5), CU-11, HU-16, **INC-35**,
INC-27, INC-29, INC-30.

**Modo de prueba:** EditMode. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] La agregación es **por nivel y por fase**, no un total por nivel (INC-35): Nivel 1 con su
      fase, Nivel 2 con sus tres, Nivel 3 con las suyas.
- [ ] Exactamente **cuatro** indicadores por fase; ninguno adicional, ninguno derivado. La lista
      es cerrada (RNF-09, §3.6.1).
- [ ] **No se calcula ningún agregado que §3.6.1 no defina**: nada de promedios, porcentajes,
      totales ni «nivel de dominio». Comentario «por qué no» — inventar un indicador es inventar
      un dato del estudiante que nadie autorizó (RNF-09).
- [ ] Un nivel no jugado se presenta como **sin datos**, no como ceros: cero intentos y no haber
      jugado no son lo mismo.
- [ ] El tiempo de resolución llega en la unidad en que se persistió; el formato es de P06.

**Verificación**
- [ ] EditMode: `IndicatorReport_INC35_AgrupaPorNivelYPorFaseNoSoloPorNivel`,
      `IndicatorReport_RNF09_EmiteExactamenteLosCuatroIndicadoresYNingunAgregadoNuevo`,
      `IndicatorReport_RF46_UnNivelNoJugadoSePresentaSinDatosNoConCeros`.

**Depende de:** P03 · **Tamaño:** M

**Archivos**
- `.../Reporting/IndicatorReport.cs`, `.../Reporting/PhaseIndicators.cs`,
  `.../Reporting/LevelReportSection.cs`
- `Assets/Tests/EditMode/Reporting/IndicatorReportTests.cs`

---

## P05: Escena `TeacherReport` y su estado en la FSM

**Descripción.** La escena `TeacherReport.unity` y la transición que la alcanza. El estado ya
existe en el enum desde el Slice 1 (`T03`), declarado y sin destino: aquí se le da destino. La
escena muestra la lista de perfiles y permite seleccionar uno.

**Traza:** RF-46, RNF-04, CU-11 (pasos 1 a 3), HU-16, `SPEC.md` §Arquitectura.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] `GameFlow` transiciona `MainMenu → TeacherReport → MainMenu` y la vuelta no pierde el
      perfil activo del estudiante, si lo había.
- [ ] La lista de perfiles es navegable con clic y solo con clic (RNF-02, CT-06).
- [ ] Sin perfiles, la pantalla lo informa y ofrece volver al menú (CU-11 FA-2a).
- [ ] La escena carga en menos de 10 s (RNF-04) — medido, no estimado.
- [ ] Ningún elemento se sale de pantalla; los nombres largos no desbordan su fila.

**Verificación**
- [ ] PlayMode: `TeacherReport_RF46_SeAlcanzaDesdeElMenuYVuelveSinPerderElPerfilActivo`,
      `TeacherReport_CU11_SinPerfilesInformaYOfreceVolver`.
- [ ] Aserción de layout con un nombre de perfil largo.

**Depende de:** P04 · **Tamaño:** M

**Archivos**
- `.../Reporting/TeacherReportController.cs`, `.../Reporting/ProfileListView.cs`
- `Assets/Game/Scenes/TeacherReport.unity`
- `Assets/Tests/PlayMode/Reporting/TeacherReportTests.cs`

---

## P06: Tabla de los cuatro indicadores — aquí sí van las cifras

**Descripción.** La presentación de los indicadores del perfil seleccionado, por nivel y por fase,
**con sus valores numéricos**. Es el único lugar del juego donde una cifra es correcta, y hay que
dejarlo escrito en el código: no es una excepción olvidada, es RF-46.

**Traza:** RF-46, RF-45, RNF-19, RNF-20, RNF-01, CP-03 (su límite), CP-09, OE1 §3.6.1, CU-11
(paso 4), HU-16, INC-35, INC-26.

**Modo de prueba:** PlayMode `[Category("Integration")]` + aserción de layout.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] Se muestran los cuatro indicadores con su valor, agrupados por nivel y dentro de cada nivel
      por fase (RF-46, INC-35).
- [ ] El tiempo de resolución se formatea legible —minutos y segundos—, no en milisegundos crudos.
- [ ] Cada indicador lleva **su nombre además de su icono** (RNF-19): el icono solo no basta.
- [ ] El contraste de la tabla es ≥ 4.5:1 (RNF-20): es una pantalla densa y proyectada.
- [ ] Los encabezados usan el vocabulario de §3.6.1, no jerga inventada (RNF-01 aplica también al
      docente).
- [ ] **Esta pantalla no es alcanzable desde ninguna ruta del estudiante**: prueba de exclusión
      que recorre el flujo del juego y comprueba que ningún camino jugable llega aquí (CP-03).

**Verificación**
- [ ] PlayMode: `TeacherReport_RF46_PresentaLosCuatroIndicadoresPorNivelYPorFase`,
      `TeacherReport_RF46_ElTiempoSeFormateaEnMinutosYSegundos`,
      `TeacherReport_CP03_NingunaRutaDelEstudianteAlcanzaLaPantallaDeCifras`.
- [ ] VisualVerification: `TeacherReport_RNF20_ContrasteSuficienteEnLaTablaCompleta`.
- [ ] Aserción de layout con un perfil que completó los tres niveles: es la tabla más larga.

**Depende de:** P05 · **Tamaño:** M

**Archivos**
- `.../Reporting/IndicatorTableView.cs`, `.../Reporting/ReportContent.cs` (SO de encabezados)
- `Assets/Game/Data/Reporting/ReportContent.asset`
- `Assets/Tests/PlayMode/Reporting/IndicatorTableTests.cs`

---

## P07: Acceso desde el menú principal

**Descripción.** RF-46 dice «desde una opción del menú principal»: el informe no se alcanza desde
dentro de una partida. Añadir la opción y comprobar que no interfiere con el flujo del estudiante.

**Traza:** RF-46, RF-01, RNF-02, RNF-03, CT-06, CU-11 (paso 1), HU-16, HU-18.

**Modo de prueba:** PlayMode `[Category("Integration")]`.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] La opción está en el menú principal, junto a Jugar, Créditos y Salir (RF-01, RF-46).
- [ ] Es alcanzable con clic y solo con clic (RNF-02, CT-06).
- [ ] Entrar y salir del informe **no altera** el perfil activo ni el progreso de nadie: es solo
      lectura, salvo la eliminación de P09.
- [ ] El menú principal sigue cumpliendo RNF-03 y no desborda con la opción añadida.

**Verificación**
- [ ] PlayMode: `MainMenu_RF46_OfreceLaOpcionDeProgresoDelDocente`,
      `TeacherReport_RF46_ConsultarNoAlteraNingunPerfil`.
- [ ] Aserción de layout sobre el menú principal completo.

**Depende de:** P06 · **Tamaño:** S

**Archivos**
- `.../UI/MainMenuController.cs` (ampliar)
- `Assets/Tests/PlayMode/Core/MainMenuTests.cs` (ampliar)

---

### ✅ Checkpoint P-C — Informe docente completo

- [ ] Un perfil que jugó los tres niveles se consulta entero, por nivel y por fase (INC-35).
- [ ] Ninguna ruta del estudiante llega a la pantalla de cifras (CP-03).
- [ ] Un nivel no jugado aparece sin datos, no con ceros.
- [ ] Contraste verificado sobre la tabla más larga (RNF-20).
- [ ] Revisado con el usuario.

---

# Fase 3 — Eliminación de datos

> **P08 no depende de P03..P07** y conviene adelantarla: es la parte irreversible del proyecto y
> sus pruebas deberían estar verdes antes de que exista un botón que la dispare.

## P08: `ProfileEraser` — borrado en las dos rutas, sin residuos

**Descripción.** La eliminación definitiva de un perfil y todos sus datos asociados, sobre
`Datos/` **y** la ruta de respaldo. Lógica pura contra un `IFileSystem` inyectado. Es la tarea con
consecuencias reales del slice: un borrado incompleto incumple RNF-11 y uno excesivo destruye
datos de otro estudiante.

**Traza:** **RF-47**, **RNF-11**, RNF-09, RNF-12, CT-07, CU-12, HU-16, **INC-34**, supuesto 1,
`SPEC.md` §Criterios de éxito (10).

**Modo de prueba:** EditMode con dobles de sistema de archivos. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Borra el perfil de **`Datos/` y de la ruta de respaldo**, no de una sola (INC-34).
- [ ] Los **dos escenarios de INC-34** están probados: `Datos/` escribible, y `Datos/` de solo
      lectura con el perfil en la ruta de respaldo.
- [ ] **No deja residuos**: ni archivo de perfil, ni índice, ni copia temporal, ni carpeta vacía
      con su nombre (RNF-11, criterio de verificación literal).
- [ ] **No toca ningún otro perfil.** Prueba con tres perfiles: se borra uno y los otros dos
      quedan íntegros y legibles.
- [ ] Si una de las dos rutas no es accesible, el resultado lo dice y **no reporta éxito
      parcial como éxito**: RNF-11 exige ausencia de residuos, no mejor esfuerzo.
- [ ] Es irreversible por diseño: no hay papelera, ni copia de seguridad, ni deshacer.

**Verificación**
- [ ] EditMode: `ProfileEraser_RNF11_NoDejaResiduosEnNingunaDeLasDosRutas`,
      `ProfileEraser_INC34_BorraTambienDesdeLaRutaDeRespaldoSiDatosEsDeSoloLectura`,
      `ProfileEraser_RF47_NoAfectaAOtrosPerfiles`,
      `ProfileEraser_RNF11_UnBorradoParcialNoSeReportaComoExito`.

**Depende de:** P01 · **Tamaño:** M

**Archivos**
- `.../Reporting/ProfileEraser.cs`, `.../Reporting/EraseResult.cs`
- `Assets/Tests/EditMode/Reporting/ProfileEraserTests.cs`

---

## P09: Confirmación explícita e irreversibilidad en la UI

**Descripción.** El diálogo que advierte que la acción es irreversible y exige confirmación
explícita. Cancelar vuelve a la pantalla de progreso sin cambiar nada.

**Traza:** RF-47, RNF-11, CU-12 (pasos 3 y 4, FA-4a), HU-16, RNF-19, RNF-20.

**Modo de prueba:** PlayMode `[Category("Integration")]`.
**Corredor MCP:** **sí lo exige** para automatizarse.

**Criterios de aceptación**
- [ ] La confirmación es **explícita** y advierte que la acción es irreversible (CU-12 paso 3).
- [ ] La acción destructiva **no** es la opción por defecto ni la más fácil de accionar por error.
- [ ] Cancelar regresa a la pantalla de progreso **sin realizar cambio alguno** (CU-12 FA-4a) —
      prueba que verifica el sistema de archivos después de cancelar, no solo la navegación.
- [ ] Tras confirmar, el perfil desaparece de la lista y la pantalla lo refleja sin recargar la
      escena.
- [ ] El diálogo señala su carácter destructivo con **más que color** (RNF-19) y con contraste
      suficiente (RNF-20).

**Verificación**
- [ ] PlayMode: `EraseDialog_RF47_ExigeConfirmacionExplicitaYAdvierteIrreversibilidad`,
      `EraseDialog_CU12_CancelarNoRealizaNingunCambioEnDisco`,
      `EraseDialog_RF47_TrasConfirmarElPerfilDesapareceDeLaLista`.

**Depende de:** P08, P06 · **Tamaño:** S

**Archivos**
- `.../Reporting/EraseConfirmationDialog.cs`
- `Assets/Tests/PlayMode/Reporting/EraseDialogTests.cs`

---

## P10: Prueba de residuos sobre disco real

**Descripción.** `P08` prueba la lógica con dobles; ésta la prueba con archivos de verdad en un
directorio temporal. El criterio de verificación de RNF-11 es «verificación de la **ausencia de
residuos en el almacenamiento local**», y eso solo se comprueba mirando el almacenamiento.

**Traza:** RNF-11, RF-47, RNF-07, CU-12, HU-16, INC-34, `SPEC.md` §Criterios de éxito (10).

**Modo de prueba:** EditMode `[Category("Integration")]`, sobre un directorio temporal creado y
destruido por la prueba. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Crea perfiles reales en un directorio temporal, borra uno y **enumera el árbol completo**
      comprobando que no queda ninguna entrada asociada a ese perfil.
- [ ] Corre en los **dos escenarios** de INC-34: directorio escribible, y directorio marcado de
      solo lectura con respaldo.
- [ ] La prueba limpia detrás de sí y no escribe fuera del directorio temporal — nunca en
      `Datos/` del proyecto ni en `Application.persistentDataPath` real.
- [ ] Si el entorno no permite marcar un directorio de solo lectura, la prueba **se declara
      omitida con motivo**, no se da por pasada.

**Verificación**
- [ ] EditMode: `ProfileEraser_RNF11_SobreDiscoRealNoQuedaNingunaEntradaDelPerfil`,
      `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`.
- [ ] Inspección manual del árbol tras una eliminación real en el ejecutable portable — es el
      criterio de OE4, y se anota.

**Depende de:** P09 · **Tamaño:** S

**Archivos**
- `Assets/Tests/EditMode/Reporting/ProfileEraserDiskTests.cs`

---

### ✅ Checkpoint P-D — Eliminación conforme

- [ ] Un perfil se elimina con confirmación y desaparece de las dos rutas.
- [ ] Cancelar no cambia nada en disco.
- [ ] Ningún otro perfil se ve afectado.
- [ ] Árbol de almacenamiento inspeccionado tras el borrado: cero residuos (RNF-11).
- [ ] Revisado con el usuario.

---

# Fase 4 — Cierre del proyecto

## P11: Cierre de CP-03 y RNF-09 sobre el juego completo

**Descripción.** Los dos invariantes que atraviesan todo el proyecto y que solo se pueden cerrar
cuando existe todo: que **el estudiante no ve una cifra en ninguna parte**, y que **el JSON no
guarda un campo fuera de la lista cerrada**. Es una prueba que se escribe, no una revisión: una
inspección sin prueba se degrada en la siguiente sesión.

**Traza:** CP-03, RF-17, RF-45, RNF-09, CT-10, INC-26, INC-27, OE1 §3.6.1 (notas 3 y 5),
`SPEC.md` §Límites, §Estrategia de pruebas.

**Modo de prueba:** EditMode + PlayMode `[Category("Integration")]`.
**Corredor MCP:** **sí lo exige** para el PlayMode.

**Criterios de aceptación**
- [ ] Barrido de todos los ScriptableObject de texto visible al estudiante: **ningún dígito**
      salvo los explícitamente permitidos —el contador «n de 5» del Nivel 2 (RF-24) y la lista de
      cuatro tareas del Nivel 3 (RF-36)—, que son estado de tarea y no desempeño. La lista de
      excepciones es cerrada y está en la prueba, no en la cabeza de nadie.
- [ ] El JSON generado por un perfil que jugó los tres niveles contiene **solo** nombre o alias,
      nivel alcanzado, fases confirmadas y los cuatro indicadores por fase (RNF-09, INC-27).
- [ ] No existe en el proyecto ningún campo, clase ni asset de puntaje, tiempo visible al
      estudiante, ranking ni comparación entre perfiles.
- [ ] Recorrido completo del juego con inspección: en ninguna pantalla del estudiante aparece una
      cifra de desempeño.
- [ ] **Todo RF tiene al menos una prueba que lo nombra** (CT-10): la matriz se deriva de los
      nombres de método, no se mantiene a mano.

**Verificación**
- [ ] EditMode: `Content_CP03_NingunTextoVisibleAlEstudianteContieneCifrasDeDesempeno`,
      `SaveStore_RNF09_ElJsonDeUnPerfilCompletoNoTieneCampoFueraDeLaListaCerrada`,
      `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra`.
- [ ] PlayMode: recorrido completo con captura de cada pantalla del estudiante.

**Depende de:** P02, P07, P10 · **Tamaño:** M

**Archivos**
- `Assets/Tests/EditMode/Content/ContentInvariantTests.cs`
- `Assets/Tests/EditMode/Architecture/TraceabilityTests.cs`

---

## P12: Presupuestos y ejecución portable

**Descripción.** El último gate del proyecto. Los presupuestos de RNF-04..RNF-06 **se verifican en
el equipo de referencia, no se estiman** (`SPEC.md` §Stack), y con los tres slices de arte
importados es la primera vez que RNF-06 se puede medir de verdad.

**Traza:** RNF-04, RNF-05, RNF-06, RNF-07, RNF-08, RNF-10, RNF-13, RNF-14, HU-15, HU-18, CT-03,
`SPEC.md` §Criterios de éxito (6, 9).

**Modo de prueba:** manual, plan de pruebas de OE4. **Corredor MCP:** no lo exige.

**Criterios de aceptación**
- [ ] Carga de **cada** escena < 10 s, medida y anotada escena por escena (RNF-04).
- [ ] Memoria en ejecución < 2 GB durante un recorrido completo (RNF-05).
- [ ] Paquete de distribución < 500 MB con todo el arte de los cuatro slices (RNF-06).
- [ ] Ejecución desde carpeta portable en **dos equipos distintos**, sin instalación ni
      privilegios de administrador (RNF-07, HU-15).
- [ ] Ejecución completa con el **adaptador de red deshabilitado** (RNF-08), y sin tráfico de red
      durante una partida (RNF-10).
- [ ] **Dos recorridos completos por nivel** sin bloqueos ni estados irrecuperables (RNF-13).
- [ ] Cierre forzado y reapertura: el progreso retoma en la última fase confirmada (RNF-14).
- [ ] Salir desde el menú guarda el estado del perfil activo (RF-09, HU-18).

**Verificación**
- [ ] Tabla de mediciones anotada en el tablero: escena por escena, equipo por equipo. Números,
      no adjetivos.
- [ ] Monitor de red durante una partida completa (RNF-10).

**Depende de:** P11 · **Tamaño:** S (sin código; es medición y registro)

**Archivos**
- `claudeDocs/tasks/Slice 4/todo.md` — tabla de mediciones

---

### ✅ Checkpoint P-E — Proyecto completo

- [ ] **RF-46 y RF-47 cerrados.** Con ellos, los 45 RF de prioridad Alta están implementados.
- [ ] RF-06 (Media) y RF-21 (Baja): implementados, o **declarados fuera** con su razón. Son los
      dos únicos que el trabajo de grado admite dejar fuera.
- [ ] Los diez criterios de éxito de `SPEC.md` §Criterios de éxito revisados uno por uno.
- [ ] `INCONSISTENCIAS.md` revisado: ningún hallazgo reabierto por el código.
- [ ] Golden Path del juego entero en 20–40 minutos, dos veces (RNF-13, `SPEC.md` §Objetivo).
- [ ] **PG-01** (título) y **PG-02** (nombre del guía) cerrados: aparecen en pantalla y en créditos.
- [ ] **RNF-12**: formato de consentimiento informado presente en los anexos del proyecto.
- [ ] **RNF-22**: inspección integral del contenido — sin violencia explícita, publicidad,
      compras integradas ni enlaces externos, en los tres niveles y en los créditos
- [ ] **CT-02**: el ejecutable corre en un equipo **sin tarjeta gráfica dedicada**, dentro de
      los presupuestos de RNF-04 y RNF-05
- [ ] Revisado con el usuario. Fin del prototipo.

---

## Riesgos

| # | Riesgo | Impacto | Mitigación |
|---|---|---|---|
| **R1** | **Los Slices 1, 2 y 3 no están hechos.** Este módulo consume indicadores que aún no emite nadie. | **Alto — abierto** | No abrir P02 antes del Checkpoint R-E del Slice 3. P00, P01, P03 y P08 son las únicas tareas que no dependen de que los niveles existan. |
| **R2** | **Corredor de pruebas MCP.** Sigue sin conectar tras tres slices. | Alto | **`P00` lo convierte en tarea, no en riesgo.** Este slice se abre resolviéndolo o dejando escrita la decisión de no hacerlo. |
| **R3** | **RF-47 es irreversible y se prueba con archivos reales.** Una prueba mal escrita puede borrar datos que no debía. | **Alto** | `P10` corre **solo** sobre un directorio temporal que ella misma crea y destruye, con una aserción de que la ruta bajo prueba no es `Datos/` ni `persistentDataPath` real. Escribir esa guarda **antes** que el borrado. |
| **R4** | **La frontera de `Game.Reporting`.** Si referencia a los niveles, RNF-16 deja de pasar y no se nota hasta la prueba de exclusión. | Medio | `P01` lo prueba de entrada, con una prueba negativa. Ver «la única decisión de diseño que este plan añade». |
| **R5** | **Inventar indicadores.** Un promedio o un «nivel de dominio» parece útil y es un dato del estudiante que nadie autorizó (RNF-09). | Medio | `P04` prueba que no se calcula ningún agregado fuera de §3.6.1, con comentario «por qué no». |
| **R6** | **RNF-06 (< 500 MB) se mide por primera vez aquí**, con el arte de los cuatro slices. Si se pasa, ya no hay margen. | Medio | `P12` lo mide. Si preocupa antes, medirlo también en el Checkpoint R-E del Slice 3, que es la penúltima oportunidad. |
| **R7** | La tabla de indicadores es la pantalla más densa del juego y la más expuesta a RNF-20. | Bajo | `P06` verifica el contraste sobre la tabla más larga —un perfil con los tres niveles—, no sobre una fila de ejemplo. |

---

## Preguntas abiertas

1. **¿`Game.Reporting` referencia a los assemblies de nivel?** Este plan dice que **no**, y
   explica por qué: si lo hiciera, retirar un nivel rompería el informe y RNF-16 dejaría de pasar.
   El mapa de capacidades de `SPEC.md` dice «F depende de A, C, D, E», que este plan lee como
   dependencia de datos y no de assembly. Confirmar antes de `P01`: cambia la frontera del módulo.
2. **¿El informe docente necesita protección de acceso?** Ningún RF la pide, y RF-46 solo dice
   «desde una opción del menú principal». Este plan **no añade contraseña ni gesto oculto**:
   añadirla sería una mecánica que no está en los documentos («preguntar primero»). Si la
   institución la espera, hay que radicarla como cambio de requerimiento, no colarla en el código.
3. **¿Qué pasa con el perfil activo si el docente lo elimina desde el informe?** CU-12 no lo dice.
   Propuesta: si el perfil eliminado es el activo, el juego vuelve a `MainMenu` sin perfil
   seleccionado. Confirmar antes de `P09`.
4. **Unidad y formato del tiempo de resolución.** §3.6.1 lo define pero no fija unidad. Propuesta:
   persistir en segundos y presentar en minutos y segundos (`P06`). Cambiar la unidad persistida
   después es cambiar el formato de datos, que es «preguntar primero».
5. **PG-01 y PG-02** siguen abiertos y este es el último slice. El título aparece en la pantalla
   de inicio y en los créditos (RF-01, RF-08); el nombre del guía, en las quince escenas
   narrativas y en la final. Cerrarlos antes de `P12`.
6. **RNF-12 — consentimiento informado.** Su criterio de verificación es documental («existencia
   del formato firmado en los anexos»), no de código. No hay tarea que lo cubra porque no la
   tiene: queda como pendiente del trabajo de grado, listado en el Checkpoint P-E.

---

# Assets visuales del Slice 4

**Tres assets, y ninguno es de juego.** Este slice es interfaz: iconografía y maquetas de layout.
No hay personajes ni escenarios nuevos.

**Documento que manda:** `claudeDocs/Direccion_de_Arte.md` §10 (interfaz), §11 (tipografía) y
§14 (accesibilidad). Si algo se contradice, gana la dirección de arte; si esta contradice a
`SPEC.md`, gana `SPEC.md`.

**Cómo se arma un prompt.** Los mismos bloques fijos de los slices anteriores, con el de paleta
reducido a los neutros de interfaz:

```
[1 CONTEXTO UI]  [2 ESTILO]  [3 PALETA UI]  [4 ENTREGA UI]  [5 PROHIBICIONES]  +  ELEMENTO
```

`[2 ESTILO]` y `[5 PROHIBICIONES]` se copian **idénticos** a los del `Slice 1/plan.md`.
`[4 ENTREGA UI]` sustituye al bloque de entrega de los otros slices, porque aquí no hay croma.

> **Cambio respecto de la versión anterior de este plan: el informe docente es CLARO, no oscuro.**
> La dirección de arte §10.3 y la checklist §17 lo fijan sin excepción — «texto oscuro sobre fondo
> claro, nunca al revés»—, y la pantalla más densa del juego es justo donde más importa. El par
> vigente es carbón `#3A1E18` sobre marfil `#F7EFE2`, que sostiene RNF-20 con holgura.

**El resumen de fin de nivel no genera arte.** Reutiliza el marco de diálogo `A10` del Slice 1,
igual que las escenas narrativas — es lo que hace que el resumen se lea como andamiaje y no como
una pantalla de puntaje (CP-03, RF-45). Si alguien pide un marco nuevo para el resumen, la
respuesta es `A10`.

**Autoría (CT-09, RNF-23).** Iconografía original del proyecto, registrada en
`CreditsContent.asset` (Slice 1, T08) como todo lo demás.

**Una precisión sobre los iconos.** Los cuatro indicadores de OE1 §3.6.1 —intentos, errores
corregidos, pasos utilizados, tiempo de resolución— **no se corresponden uno a uno con las
facetas** del pensamiento computacional: las facetas se mapean a RF, no a indicadores (OE1 §5.1).
La depuración, por ejemplo, la evidencian «intentos» y «errores corregidos» a la vez. Son
**cuatro iconos, uno por indicador**, que es lo que necesita la tabla de `P06`.

---

## Bloque 1 · CONTEXTO UI — sustituye al de los otros slices

```
CONTEXTO DEL ENCARGO
Soy diseñador de un videojuego educativo 2D hecho en Unity para estudiantes de grado cuarto de
primaria. Este encargo NO es para la parte que juega el estudiante: es para la pantalla que
consulta el DOCENTE, donde se muestran los datos de desempeño de sus estudiantes, y para un
diálogo de confirmación de borrado de datos.

Lo que necesito son ELEMENTOS DE INTERFAZ de producción: iconos que voy a importar a Unity y
maquetas de estructura que voy a reconstruir con componentes de UI reales. No es una ilustración
y no es una propuesta de diseño gráfico para presentar: es material de trabajo.

Condiciones que mandan:
1. La pantalla se ve proyectada en un aula, a veces con luz ambiente alta. La legibilidad manda
   sobre cualquier consideración estética.
2. Texto oscuro sobre fondo claro, SIEMPRE. Nunca al revés.
3. Estética de piedra, hueso y cuero tallados, coherente con la ambientación prehistórica del
   juego. NUNCA estética digital moderna: sin bordes metálicos, sin brillos de cristal, sin
   sombras difuminadas, sin gradientes de interfaz.
4. Los elementos son planos y frontales, sin perspectiva y sin volumen, y deben ser legibles a
   32 píxeles de alto.

INSTRUCCIÓN SOBRE LO QUE NO TE DIGA: sigue las secciones de abajo al pie de la letra. Donde no
te dé un dato, NO lo inventes: elige la opción más simple compatible con las reglas y deja el
resto vacío. No añadas adornos, texto, iconos ni elementos que no haya pedido explícitamente.
```

## Bloque 3 · PALETA UI — usar solo estos colores

```
PALETA (fija, no usar ningún color fuera de esta lista)
  Marfil #F7EFE2            Fondo de panel y de tabla
  Marfil sombra #E0D4C0     Separadores y fondo de fila alterna
  Borde de panel #C4A882    Cuero del marco
  Carbón #3A1E18            Texto, contorno e iconos
  Carbón suave #6B5248      Texto secundario y puntadas
  Éxito #5FA842             Confirmación
  Atención #E8A33D          Selección, alerta y acción destacada

REGLA DE CONTRASTE: el par que sostiene la lectura es carbón #3A1E18 sobre marfil #F7EFE2. No
inviertas esa relación en ningún elemento.

NO HAY ROJO DE ERROR en toda la interfaz del juego. La alerta se marca con ámbar #E8A33D más una
FORMA —triángulo, rombo, contorno discontinuo—, nunca con rojo. Es una decisión pedagógica del
proyecto: el rojo comunica castigo, y este juego no castiga.
```

## Bloque 4 · ENTREGA UI — sustituye al bloque de entrega con croma

```
ENTREGA TÉCNICA (obligatoria)
FONDO: marfil plano #F7EFE2, uniforme, sin degradado, sin viñeta y sin sombra proyectada.

ENCUADRE: el elemento completo dentro del lienzo, sin recortes, con al menos un 10 por ciento de
margen vacío a cada lado.

NADA DE TEXTO: ni letras, ni números, ni palabras, ni firmas, ni marcas de agua. Donde vaya a ir
texto, dibuja una BARRA RECTANGULAR llena en #E0D4C0 como marcador de posición, del largo
aproximado que ocuparía la frase. El texto real lo escribe el motor del juego.

FORMATO: PNG sin compresión con pérdida. Los iconos, en lámina cuadrada; las maquetas de panel,
en 16:9 horizontal.

SI TE PIDO VARIOS ELEMENTOS: genéralos en la MISMA imagen, separados y alineados, sin marcos ni
títulos entre ellos, y con el mismo tamaño de caja y el mismo grosor de trazo en todos.
```

---

## D1 · Iconografía de los cuatro indicadores

**Traza:** RF-46, RF-45, RNF-19, RNF-20, OE1 §3.6.1, CU-11, HU-16.
**Chroma:** no. **Archivo:** `ui_ind_intentos.png`, `_errores`, `_pasos`, `_tiempo`.

```
[1 CONTEXTO UI] [2 ESTILO] [3 PALETA UI] [4 ENTREGA UI] [5 PROHIBICIONES]

ELEMENTO: los cuatro iconos de indicadores del informe docente. Sin texto de ningún tipo: cada
icono va acompañado de su nombre, escrito por el motor.

Generar CUATRO iconos en una sola lámina, alineados horizontalmente, TODOS con el mismo tamaño,
el mismo grosor de trazo y la misma caja cuadrada imaginaria, dibujados en carbón #3A1E18 sobre
el marfil del fondo:

  (1) INTENTOS — «acciones que el sistema evalúa y que no producen avance en la tarea activa»:
      una mano cerrada, vista de perfil, golpeando suavemente sobre una superficie plana, con
      DOS arcos cortos de repetición sobre ella. Debe leerse como «volver a probar», nunca como
      agresión ni como fracaso.

  (2) ERRORES CORREGIDOS — «intentos fallidos que, tras cambiar la hipótesis, terminan en
      acierto»: una flecha que traza una curva en U, saliendo hacia abajo y regresando hacia
      arriba, con una marca de verificación pequeña #5FA842 en su extremo final. Es corrección,
      no castigo.

  (3) PASOS UTILIZADOS — «unidades de acción que componen la solución aceptada»: TRES huellas de
      pie descalzo en fila diagonal ascendente, la primera con el trazo más fino y la última con
      el trazo pleno.

  (4) TIEMPO DE RESOLUCIÓN: un reloj de arena de silueta simple y simétrica, con la arena
      superior e inferior insinuadas por dos triángulos macizos. SIN números, SIN manecillas y
      SIN cuenta regresiva: este juego no tiene temporizador y el icono no debe sugerir que lo
      tenga.

REQUISITOS: los cuatro deben distinguirse por SILUETA en escala de grises y ser legibles a 32
píxeles. Ninguno debe leerse como acierto o error, premio o castigo: son medidas, no juicios. El
estudiante nunca los ve, y el docente no debe leer un veredicto en el icono.
```

**Verificación (§17):** las cuatro siluetas se separan a 32 px y en escala de grises · ninguno
sugiere premio ni castigo · el reloj no sugiere cuenta atrás (CP-02) · sin texto.

---

## D2 · Maqueta del panel de `TeacherReport`

**Traza:** RF-46, RF-45, RNF-19, RNF-20, RNF-01, CU-11, HU-16, **INC-35** (por nivel y por fase).
**Chroma:** no. **Archivo:** `ui_teacherreport_maqueta.png`.

> **Es una maqueta, no arte final.** Sirve para construir la jerarquía de UI en la escena. Una
> tabla generada como imagen no se puede rellenar con datos.

```
[1 CONTEXTO UI] [2 ESTILO] [3 PALETA UI] [4 ENTREGA UI] [5 PROHIBICIONES]

ELEMENTO: maqueta de estructura del panel de consulta de progreso del docente. VISTA FRONTAL
PLANA, relación de aspecto 16:9. Sin texto: todo el contenido lo escribe el motor. Donde vaya
texto, una barra rectangular #E0D4C0 como marcador de posición.

ESTRUCTURA FIJA, de arriba abajo:

  (1) CABECERA: banda horizontal delgada de cuero #C4A882 con puntadas #6B5248. A la izquierda,
      una barra marcadora larga (el título de la pantalla). A la derecha, un botón redondeado
      #E8A33D con contorno #3A1E18 y una flecha #3A1E18 apuntando a la izquierda: volver al menú.

  (2) COLUMNA IZQUIERDA, un tercio del ancho: lista vertical de perfiles. SEIS filas iguales,
      cada una con una barra marcadora, separadas por líneas finas #E0D4C0. La SEGUNDA fila
      aparece seleccionada: fondo #E0D4C0 y una barra vertical gruesa #E8A33D pegada a su borde
      izquierdo —la selección se marca por color Y por esa barra, no solo por color—.
      Al PIE de la columna, separado del resto por un espacio generoso y una línea, un botón
      ancho de contorno #3A1E18 con un icono de papelera #3A1E18: es eliminar datos. Tiene que
      verse claramente apartado de la lista, nunca contiguo a la fila seleccionada.

  (3) COLUMNA DERECHA, dos tercios del ancho: la tabla de indicadores, en TRES BLOQUES apilados,
      uno por nivel, separados por una línea #C4A882 más gruesa. Cada bloque lleva arriba una
      barra marcadora ancha (el nombre del nivel) y debajo VARIAS FILAS, una por fase: el primer
      bloque con UNA fila, el segundo con TRES, el tercero con CUATRO. Que los bloques tengan
      distinto número de filas es parte de la estructura y tiene que verse.
      Cada fila de fase se divide en cinco columnas: una barra marcadora estrecha a la izquierda
      (el nombre de la fase) y, a su derecha, CUATRO celdas iguales, cada una con un cuadrado
      pequeño de contorno #3A1E18 arriba —donde irá el icono del indicador— y una barra marcadora
      corta debajo —donde irá la cifra—.

  (4) Todo el panel sobre marfil #F7EFE2, enmarcado por un borde de cuero #C4A882 de esquinas
      redondeadas.

REQUISITO: densa pero respirada; es una pantalla para proyector. La maqueta debe dejar claro que
un nivel puede tener más filas que otro y que las cifras viven aquí y en ninguna otra pantalla.
```

**Verificación (§17):** tres bloques con 1, 3 y 4 filas · la selección se marca por barra además
de por fondo (RNF-19) · el botón de eliminar queda separado de la lista · texto oscuro sobre
fondo claro · sin texto real en la imagen.

---

## D3 · Diálogo de confirmación de eliminación

**Traza:** RF-47, RNF-11, RNF-19, RNF-20, CU-12 (pasos 3 y 4, FA-4a), HU-16.
**Chroma:** no. **Archivo:** `ui_dialogo_eliminar.png`.

```
[1 CONTEXTO UI] [2 ESTILO] [3 PALETA UI] [4 ENTREGA UI] [5 PROHIBICIONES]

ELEMENTO: marco del diálogo de confirmación de eliminación de datos. VISTA FRONTAL PLANA. Sin
texto: la advertencia la escribe el motor. Donde vaya texto, barras marcadoras #E0D4C0.

FORMA FIJA: recuadro compacto de esquinas muy redondeadas, centrado, con borde de cuero #C4A882
de grosor uniforme y puntadas #6B5248, sobre interior marfil #F7EFE2. El recuadro es
notablemente más pequeño que un panel de pantalla completa: es un diálogo que se superpone.

CONTENIDO, de arriba abajo:
  (1) Un icono de advertencia centrado arriba: TRIÁNGULO de contorno grueso #E8A33D con un signo
      de admiración #3A1E18 dentro. Es la única marca de alerta del diálogo y va acompañada de
      la forma triangular: no depende del color (RNF-19). NO uses rojo.
  (2) Dos barras marcadoras apiladas y centradas: la advertencia de que la acción es
      irreversible.
  (3) Abajo, DOS BOTONES lado a lado, deliberadamente distintos entre sí:
      — IZQUIERDA, «Cancelar»: botón ANCHO y sólido en #E8A33D con contorno CONTINUO #3A1E18 y
        una barra marcadora #3A1E18 dentro. Es el más grande y el más visible de los dos.
      — DERECHA, «Eliminar»: botón MÁS ESTRECHO, de fondo marfil #F7EFE2 con contorno #3A1E18
        DISCONTINUO a trazos, una barra marcadora #6B5248 dentro y un icono de papelera #3A1E18
        a su izquierda.

REQUISITO DE DISEÑO (no es preferencia estética): la acción destructiva NO puede ser la más fácil
de accionar por error. El botón de cancelar es el prominente; el de eliminar se distingue por su
contorno discontinuo y su menor peso visual, y esa diferencia debe leerse también en escala de
grises. Es un requisito de protección de datos (RF-47, RNF-11).
```

**Verificación (§17):** cancelar es visiblemente más prominente que eliminar · la diferencia se
mantiene en escala de grises · ningún rojo en el diálogo (§12.3) · la alerta lleva forma además
de color (RNF-19) · sin texto real.

---

## Postproceso de los assets

No hay recorte de croma en este slice. Aun así:

1. **Verificar** contra la checklist de `Direccion_de_Arte.md` §17 y contra la línea
   «Verificación» de cada asset.
2. **Exportar PNG con alfa**: los iconos de `D1` deben poder ir sobre cualquier fila de la tabla,
   incluida la fila alterna en `#E0D4C0`, no solo sobre el marfil.
3. **`D2` y `D3` son maquetas, no arte final**: se usan para construir la jerarquía de UI con
   componentes reales en la escena. Una tabla generada como imagen no se puede rellenar con datos.
4. **Nombrar** según §15.4 e **importar** en `Assets/Game/Art/UI/` con los ajustes de §15.2 y el
   **mismo `Pixels Per Unit` `100`** de los tres slices anteriores.
5. **Verificar RNF-19**: desaturar `D1` y `D3` y comprobar que los cuatro iconos se distinguen
   entre sí y que los dos botones del diálogo se siguen diferenciando.
6. **Verificar RNF-20 sobre la tabla construida y llena de datos**, no sobre la maqueta: el
   contraste se mide en la pantalla real (`P06`).
7. **Registrar** los tres en `CreditsContent.asset` (Slice 1, T08) — CT-09, RNF-23.
