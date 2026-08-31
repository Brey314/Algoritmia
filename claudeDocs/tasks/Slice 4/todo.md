# Tablero — Slice 4: Progreso y registro

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification · `MCP` = la verificación **exige** el corredor de pruebas conectado.

> ⚠️ **R1 — los Slices 1, 2 y 3 no están hechos.** Este módulo consume indicadores que aún no
> emite nadie. No abrir P02 antes del Checkpoint R-E del Slice 3. Solo **P00**, **P01**, **P03**
> y **P08** son independientes.

> ⚠️ **R3 — RF-47 borra archivos de verdad.** `P10` corre **solo** sobre un directorio temporal
> que ella misma crea y destruye. Escribir la guarda que impide tocar `Datos/` o
> `persistentDataPath` reales **antes** que el código de borrado.

> ℹ️ **El corredor de pruebas MCP ya no es un riesgo: es `P00`.** Este slice se abre
> resolviéndolo o dejando escrita la decisión de no hacerlo.

---

## Fase 0 — Cimientos del slice

- [ ] **P00 · Confirmar o instalar el corredor de pruebas MCP de Unity** — `XS` · sin código
      `SPEC.md` §Comandos, `CLAUDE.md` §Comandos · depende de: —
      Si no se instala, **dejarlo escrito**: las siete tareas `MCP` se corren a mano y se declaran
- [ ] **P01 · Assembly `Game.Reporting` y su frontera** — `XS` · `EM`
      RNF-15, **RNF-16**, `SPEC.md` §Estructura · depende de: P00, Slice 3 R01
      ⚠️ Prueba **negativa**: `Game.Reporting` no referencia a ningún assembly de nivel
      ⚠️ Confirmar antes la **pregunta abierta 1** — define la frontera del módulo

### ✅ Checkpoint P-A — Cimientos
- [ ] `run_unity_tests` responde, o queda **escrito** que se corre a mano y se declara
- [ ] `Game.Reporting` existe y no referencia a ningún nivel
- [ ] La exclusión de RNF-16 sigue pasando con el módulo de informe presente
- [ ] Revisado con el usuario

---

## Fase 1 — El lado del estudiante: resumen sin cifras

> El resumen por nivel **ya está planeado** en Slice 1 `T18`, Slice 2 `W16` y Slice 3 `R14`.
> Aquí solo va lo que exige que los tres existan: unificar y barrer.

- [ ] **P02 · `LevelSummaryContent` unificado y barrido transversal de cifras** — `S` · `EM`
      RF-45, RF-17, RF-12, RNF-01, RNF-18, CP-03, CP-07, HU-14, **INC-26** · depende de: P01, Slice 3 R14
      ⚠️ Reutiliza `DialogueRunner` y el marco `A10`. Si acaba creando una vista propia, va mal

### ✅ Checkpoint P-B — El estudiante no ve cifras
- [ ] Los tres resúmenes barridos: cero dígitos, cero juicios de valor
- [ ] Un solo componente resuelve los tres, sobre el marco de diálogo del andamiaje
- [ ] Revisado con el usuario

---

## Fase 2 — El lado del docente: `TeacherReport`

- [ ] **P03 · `ProfileRepository` — los perfiles del equipo** — `S` · `EM`
      RF-46, RF-02, RNF-07, RNF-09, RNF-13, CU-11 (FA-2a), HU-16, INC-34 · depende de: P01
- [ ] **P04 · `IndicatorReport` — agregación por nivel y por fase** — `M` · `EM`
      RF-45, RF-46, RNF-09, CP-09, OE1 §3.6.1, CU-11, HU-16, **INC-35**, INC-27 · depende de: P03
      ⚠️ **Ningún agregado que §3.6.1 no defina** — nada de promedios ni «nivel de dominio»
- [ ] **P05 · Escena `TeacherReport` y su estado en la FSM** — `M` · `PM` `MCP`
      RF-46, RNF-02, RNF-04, CT-06, CU-11, HU-16 · depende de: P04
- [ ] **P06 · Tabla de los cuatro indicadores — aquí sí van las cifras** — `M` · `PM` + `VV` `MCP`
      RF-46, RF-45, RNF-01, RNF-19, RNF-20, CP-03 (su límite), CP-09, CU-11, HU-16,
      INC-35 · depende de: P05
      ⚠️ Prueba de exclusión: **ninguna ruta del estudiante alcanza esta pantalla**
- [ ] **P07 · Acceso desde el menú principal** — `S` · `PM` `MCP`
      RF-46, RF-01, RNF-02, RNF-03, CT-06, CU-11, HU-16, HU-18 · depende de: P06

### ✅ Checkpoint P-C — Informe docente completo
- [ ] Un perfil con los tres niveles se consulta entero, **por nivel y por fase** (INC-35)
- [ ] Ninguna ruta del estudiante llega a la pantalla de cifras (CP-03)
- [ ] Un nivel no jugado aparece **sin datos**, no con ceros
- [ ] Contraste verificado sobre la tabla más larga (RNF-20)
- [ ] Revisado con el usuario

---

## Fase 3 — Eliminación de datos

> **P08 no depende de P03..P07** y conviene adelantarla: sus pruebas deberían estar verdes antes
> de que exista un botón que dispare un borrado irreversible.

- [ ] **P08 · `ProfileEraser` — borrado en las dos rutas, sin residuos** — `M` · `EM`
      **RF-47**, **RNF-11**, RNF-09, CT-07, CU-12, HU-16, **INC-34**, supuesto 1 · depende de: P01
- [ ] **P09 · Confirmación explícita e irreversibilidad en la UI** — `S` · `PM` `MCP`
      RF-47, RNF-11, RNF-19, RNF-20, CU-12 (FA-4a), HU-16 · depende de: P08, P06
      ⚠️ La acción destructiva **no** puede ser la opción por defecto
- [ ] **P10 · Prueba de residuos sobre disco real** — `S` · `EM` (integración)
      **RNF-11**, RF-47, RNF-07, CU-12, HU-16, INC-34 · depende de: P09
      ⚠️ Solo sobre directorio temporal propio. Guarda explícita contra `Datos/` y
      `persistentDataPath` reales

### ✅ Checkpoint P-D — Eliminación conforme
- [ ] Un perfil se elimina con confirmación y desaparece de **las dos rutas**
- [ ] Cancelar no cambia nada **en disco** (no solo en la navegación)
- [ ] Ningún otro perfil se ve afectado
- [ ] Árbol de almacenamiento inspeccionado tras el borrado: **cero residuos** (RNF-11)
- [ ] Revisado con el usuario

---

## Fase 4 — Cierre del proyecto

- [ ] **P11 · Cierre de CP-03 y RNF-09 sobre el juego completo** — `M` · `EM` + `PM` `MCP`
      CP-03, RF-17, RF-45, RNF-09, **CT-10**, INC-26, INC-27, OE1 §3.6.1 (notas 3 y 5)
      · depende de: P02, P07, P10
      Excepciones permitidas y **cerradas** en la prueba: contador «n de 5» (RF-24) y lista de
      cuatro tareas (RF-36) — son estado de tarea, no desempeño
- [ ] **P12 · Presupuestos y ejecución portable** — `S` · manual (plan OE4)
      RNF-04, RNF-05, RNF-06, RNF-07, RNF-08, RNF-10, RNF-13, RNF-14, CT-03, HU-15,
      HU-18 · depende de: P11

### Tabla de mediciones — `P12`

Números, no adjetivos. Rellenar en el equipo de referencia.

| Medición | Presupuesto | Equipo 1 | Equipo 2 |
|---|---|---|---|
| Carga `Boot` | < 10 s | | |
| Carga `MainMenu` | < 10 s | | |
| Carga `Narrative` | < 10 s | | |
| Carga `Level1_Cave` | < 10 s | | |
| Carga `Level2_Forest` | < 10 s | | |
| Carga `Level2_Workshop` | < 10 s | | |
| Carga `Level2_Maze` | < 10 s | | |
| Carga `Level3_River` | < 10 s | | |
| Carga `TeacherReport` | < 10 s | | |
| Memoria máxima en ejecución | < 2 GB | | |
| Tamaño del paquete | < 500 MB | | |
| Golden Path completo | 20–40 min | | |

### ✅ Checkpoint P-E — Proyecto completo
- [ ] **RF-46 y RF-47 cerrados** → los 45 RF de prioridad Alta implementados
- [ ] RF-06 (Media) y RF-21 (Baja): implementados **o declarados fuera con su razón**
- [ ] Los diez criterios de éxito de `SPEC.md` revisados uno por uno
- [ ] `INCONSISTENCIAS.md` revisado: ningún hallazgo reabierto por el código
- [ ] Golden Path del juego entero, dos veces, sin incidencias (RNF-13)
- [ ] **PG-01** (título) y **PG-02** (nombre del guía) cerrados y en pantalla
- [ ] **RNF-12**: formato de consentimiento informado en los anexos del proyecto
- [ ] Revisado con el usuario. **Fin del prototipo.**

---

## Assets visuales — `plan.md` §Assets visuales del Slice 4

**Tres assets, todos de interfaz.** Sin personajes, sin escenarios y **sin chroma key**: se
generan sobre el fondo de panel `#0B0E14` y se usan tal cual. Registrar en `CreditsContent.asset`
(Slice 1, T08) — CT-09, RNF-23.

**El resumen de fin de nivel no genera arte**: reutiliza el marco de diálogo `A10` del Slice 1.
Es lo que hace que se lea como andamiaje y no como pantalla de puntaje.

- [ ] **D1 · Iconografía de los cuatro indicadores** — RF-46, RNF-19, OE1 §3.6.1
      Uno por **indicador**, no por faceta: las facetas se mapean a RF, no a indicadores
      El reloj de arena va **sin números y sin cuenta regresiva** — este juego no tiene temporizador
- [ ] **D2 · Layout del panel de `TeacherReport`** — RF-46, RNF-20, **INC-35**
      La maqueta debe mostrar filas por fase, no solo por nivel
- [ ] **D3 · Diálogo de confirmación de eliminación** — RF-47, RNF-11, RNF-19, CU-12
      «Cancelar» es el botón prominente; «Eliminar» se distingue **también en escala de grises**
- [ ] Exportar PNG con alfa (los iconos van sobre cualquier fila, no solo sobre `#0B0E14`)
- [ ] Mismo `Pixels Per Unit` que los tres slices anteriores
- [ ] **`D2` y `D3` son maquetas, no arte final**: se usan para construir la jerarquía de UI, no
      se importan como imagen de fondo
- [ ] Desaturar `D1` y `D3` y verificar que se siguen distinguiendo (RNF-19)
- [ ] Verificar RNF-20 sobre la tabla **construida y llena de datos**, no sobre la maqueta

---

## Bloqueantes y decisiones pendientes

- [ ] **R1 · Cerrar los Slices 1, 2 y 3** antes de abrir P02. Bloqueante duro.
- [ ] **P00 · Corredor de pruebas MCP.** Primera tarea del slice, por decisión explícita.
- [ ] **Pregunta abierta 1 · ¿`Game.Reporting` referencia a los niveles?** Este plan dice que
      **no**: si lo hiciera, retirar un nivel rompería el informe y RNF-16 dejaría de pasar.
      Confirmar antes de P01 — cambia la frontera del módulo.
- [ ] **Pregunta abierta 2 · ¿El informe docente necesita protección de acceso?** Ningún RF la
      pide. Este plan **no la añade**: sería una mecánica fuera de los documentos. Si la
      institución la espera, radicarla como cambio de requerimiento.
- [ ] **Pregunta abierta 3 · ¿Qué pasa si el docente elimina el perfil activo?** CU-12 no lo dice.
      Propuesta: volver a `MainMenu` sin perfil seleccionado. Confirmar antes de P09.
- [ ] **Pregunta abierta 4 · Unidad del tiempo de resolución.** Propuesta: persistir en segundos,
      presentar en minutos y segundos. Cambiar la unidad persistida después es «preguntar primero».
- [ ] **PG-01 · título del producto** y **PG-02 · nombre del guía.** Último slice: cerrarlos antes
      de P12. Aparecen en inicio, créditos y las quince escenas narrativas.
- [ ] **RNF-12 · consentimiento informado.** Verificación documental, no de código. Sin tarea
      porque no la tiene; queda listado en el Checkpoint P-E.
