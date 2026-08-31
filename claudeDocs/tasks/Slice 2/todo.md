# Tablero — Slice 2: La Rueda

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification · `MCP` = la verificación **exige** el corredor de pruebas conectado.

> ⚠️ **R2 — el Slice 1 no está hecho.** Este slice generaliza ocho piezas del Slice 1 que aún no
> existen. No abrir W02 antes del Checkpoint D del Slice 1. Solo **W01** y **W10** son
> independientes y pueden adelantarse.

> ⚠️ **R1 abierto — no hay corredor de pruebas MCP.** `run_unity_tests` sigue sin conectar.
> Toda casilla marcada `MCP` exige haberla corrido **a mano** en la ventana Test Runner del
> Editor y **declarar el resultado**. No dar por hecho que la suite pasó.

---

## Fase 0 — Cimientos del slice

- [ ] **W01 · Assembly `Game.Levels.Wheel` y prueba de exclusión real** — `XS` · `EM`
      RNF-15, RNF-16, INC-40 · depende de: Slice 1 T01
- [ ] **W02 · `PhaseId` y desbloqueo secuencial del Nivel 2** — `M` · `EM`
      RF-03, RF-04, RNF-09, RNF-14, HU-14, CU-06, INC-27, CP-02 · depende de: W01, Slice 1 T02/T07

### ✅ Checkpoint W-A — Cimientos
- [ ] Compila sin errores ni warnings nuevos (`check_compile_errors`)
- [ ] Prueba de exclusión RNF-16 con **dos niveles reales**, corrida y **declarada**
- [ ] El menú habilita el Nivel 2 solo tras completar el Nivel 1
- [ ] Revisado con el usuario

---

## Fase 1 — Andamiaje generalizado (`andamiaje`)

- [ ] **W03 · `HintPolicy` por fase, no por nivel** — `M` · `EM`
      RF-13, RF-10, RF-11, RNF-03, CP-06, HU-03, HU-04, CU-06..CU-08, INC-41 · depende de: W02
- [ ] **W04 · Las seis secuencias narrativas del Nivel 2** — `S` · `EM` + `PM`
      RF-05, RF-06, RF-10, RF-12, RNF-01, RNF-18, HU-02, CP-07, INC-28, guion §5, §6.1.1, §6.1.3,
      §6.2.1, §6.3.1, §6.4 · depende de: W03

### ✅ Checkpoint W-B — Andamiaje generalizado
- [ ] Las seis escenas narrativas se recorren completas
- [ ] Ninguna pista del Nivel 2 resuelve la tarea (CP-06)
- [ ] `DialogueRunner` no necesitó cambios — si los necesitó, anotar por qué
- [ ] Revisado con el usuario

---

## Fase 2 — Bosque: selección por patrón (`nivel-rueda`, fase 1)

- [ ] **W05 · `WheelLevelConfig`, `ForestObject`, `PatternSelection`** — `M` · `EM`
      RF-23, RF-24, RF-11, RF-17, RF-18, CT-05, RNF-18, RNF-01, CP-02, HU-08, CU-06,
      guion §6.1.2 · depende de: W04
- [ ] **W06 · Escena `Level2_Forest` y panel de selección** — `M` · `PM` `MCP`
      RF-22, RF-23, RF-24, RF-10, RF-13, RNF-02, RNF-03, RNF-19, CT-06, HU-08, CU-06 · depende de: W05
- [ ] **W07 · Colocación de la carga y demostración del rodado** — `M` · `PM` + `VV` `MCP`
      RF-25, RF-26, RF-04, RNF-02, RNF-21, CT-06, CP-02, HU-08, CU-06 (FA-4a) · depende de: W06

### ✅ Checkpoint W-C — Fase 1 completa
- [ ] El bosque se juega entero: seleccionar → acopiar cinco → colocar la caja → empujar
- [ ] Ningún rechazo penaliza, bloquea ni muestra cifra de desempeño (CP-02, CP-03)
- [ ] El estado de error se distingue **sin depender del color** (RNF-19)
- [ ] Cierre forzado tras confirmar la fase 1 → retoma en la fase 2 (RNF-14)
- [ ] Revisado con el usuario

---

## Fase 3 — Taller: ensamblaje secuencial (`nivel-rueda`, fase 2)

- [ ] **W08 · `AssemblySequence`, la máquina de ensamblaje** — `M` · `EM`
      RF-28, RF-29, RF-11, RF-17, RNF-18, CP-02, CP-06, CP-03, HU-09, CU-07, guion §6.2.2 · depende de: W07
- [ ] **W09 · Escena `Level2_Workshop` y cableado del ensamblaje** — `M` · `PM` `MCP`
      RF-27, RF-28, RF-29, RF-04, RF-10, RNF-02, RNF-03, RNF-19, CT-06, HU-09, CU-07,
      INC-41 · depende de: W08

### ✅ Checkpoint W-D — Fase 2 completa
- [ ] El taller se juega entero: perforar → perforar → eje → tabla → caja
- [ ] Cada intento fuera de orden da el mensaje del guion **y no deshace nada** (CP-02)
- [ ] Cierre forzado tras confirmar la fase 2 → retoma en la fase 3 (RNF-14)
- [ ] Revisado con el usuario

---

## Fase 4 — Laberinto: editor de bloques (`nivel-rueda`, fase 3)

> **W10, W11 y W12 no dependen de W02..W09.** Son la parte de mayor riesgo del slice (INC-33) y
> conviene adelantarlas. Ver R3 y la pregunta abierta 4 del plan.

- [ ] **W10 · `MazeGrid` y `CartState` — la orientación relativa** — `M` · `EM`
      RF-30, RF-31, RF-33, RNF-13, RNF-18, CT-05, **INC-33**, supuesto 8, guion §6.3.2,
      CU-08 · depende de: W01
- [ ] **W11 · `BlockSequence` — composición y edición** — `S` · `EM`
      RF-31, RF-34, RF-18, CP-02, HU-10, CU-08 (FA-3a, FA-6a) · depende de: W10
- [ ] **W12 · `SequenceExecutor` — paso a paso y validación por retroceso** — `M` · `EM`
      RF-32, RF-33, RF-34, RF-11, RNF-13, CP-02, CP-03, CP-06, HU-10, CU-08 · depende de: W11
- [ ] **W13 · Escena `Level2_Maze` y editor de bloques** — `M` · `PM` `MCP`
      RF-30, RF-31, RF-32, RF-13, RNF-02, RNF-03, RNF-19, CT-06, **PG-04**, HU-10,
      CU-08 · depende de: W12, W09
- [ ] **W14 · Reintento sin reiniciar el nivel** — `S` · `PM` `MCP`
      RF-34, RF-18, RF-04, CP-02, HU-10, CU-08 (FA-6a) · depende de: W13

### ✅ Checkpoint W-E — Fase 3 completa
- [ ] El laberinto se resuelve componiendo → ejecutando → corrigiendo → volviendo a ejecutar
- [ ] «Avanzar» produce desplazamientos distintos según la orientación, **verificado jugando** (INC-33)
- [ ] «Ejecutar» responde a **clic simple**, no a doble clic (PG-04, RNF-02)
- [ ] Ninguna retroalimentación nombra el bloque a corregir (CP-06)
- [ ] Revisado con el usuario

---

## Fase 5 — Cierre del nivel

- [ ] **W15 · Emisión de los cuatro indicadores del Nivel 2** — `M` · `EM`
      RF-45, RF-04, RF-07, RNF-09, RNF-14, CP-03, CP-09, OE1 §3.6.1 (notas 1–5), INC-27,
      INC-29 · depende de: W14, Slice 1 T17
      ⚠️ **`Pasos utilizados` de la fase 1 no está definido en §3.6.1** — ver pregunta abierta 1
- [ ] **W17 · Pausa y reinicio sobre las tres escenas del Nivel 2** — `S` · `PM` `MCP`
      RF-07, RF-03, RF-04, CP-02, HU-17 (FA-01..FA-05), INC-25 · depende de: W14, Slice 1 T16
- [ ] **W18 · Doble indicador y contraste en los estados de error** — `S` · `VV` `MCP`
      **RNF-19** (su criterio de verificación es este nivel), RNF-20, RNF-21, CN-04 · depende de: W07, W09, W14
- [ ] **W16 · Resumen, cierre reflexivo y desbloqueo del Nivel 3** — `M` · `EM` + `PM`
      RF-45, RF-12, RF-17, RF-03, CP-03, CP-07, CP-10, HU-14, CU-08, INC-26,
      guion §6.4 · depende de: W15, W17, W18

### ✅ Checkpoint W-F — Slice 2 completo
- [ ] **Dos recorridos completos** del Nivel 2 sin incidencias (RNF-13): puente → bosque → patrón
      → taller → regreso → laberinto → cierre → menú con Nivel 3 desbloqueado
- [ ] Cierre forzado **en cada una de las tres fases** → retoma desde la última confirmada (RNF-14)
- [ ] Prueba de exclusión RNF-16 en los dos sentidos: quitar `Wheel` y quitar `Fire`
- [ ] Carga de las tres escenas < 10 s y memoria < 2 GB, **medidas** (RNF-04, RNF-05)
- [ ] Paquete acumulado < 500 MB con el arte del Slice 2 incluido (RNF-06)
- [ ] Mapa de controles de las tres escenas: solo clic y clic sostenido (RNF-02, CT-06)
- [ ] **PG-05** verificado: el paso del panel del N1 al arrastre del N2 no confunde. Anotarlo
- [ ] RF-22..RF-34 tienen cada uno al menos una prueba que los nombra (CT-10)
- [ ] Revisado con el usuario antes de abrir el Slice 3

---

## Assets visuales — `plan.md` §Assets visuales del Slice 2

Escenarios, props e interfaz **originales del proyecto**. **Los personajes no se regeneran**: se
reutilizan `A1`..`A5` del Slice 1, que son **obra derivada** de los diseños Anonaky con
autorización concedida y mención obligatoria en créditos (CT-09, RNF-23). Cada asset se registra
en `CreditsContent.asset` (Slice 1, T08).

**Cinco bloques fijos por prompt**, copiados palabra por palabra antes de la descripción:
`[1 CONTEXTO] [2 ESTILO] [3 PALETA] [4 ENTREGA] [5 PROHIBICIONES]`. Un asset generado sin los
cinco se descarta y se vuelve a pedir. La paleta y las especificaciones salen de
`claudeDocs/Direccion_de_Arte.md`.

**El Nivel 2 es un BOSQUE**, no un cañón desértico (guion §6.1.1). El acento del nivel es la
**madera clara trabajada `#C79A5E`**, exclusiva de lo interactivo: ningún árbol ni suelo del
decorado la lleva (`Direccion_de_Arte.md` §8.2).

**Chroma:** verde `#00FF00` por defecto; **magenta `#FF00FF`** donde hay verde en el propio asset
(R5). Los fondos de escena no llevan chroma.

- [ ] **B1 · Escenario del bosque** — chroma **no** — RF-22, guion §6.1.1
- [ ] **B2 · Objetos del bosque: válidos y tres distractores** — chroma **magenta** — RF-22, RF-23, RNF-19
- [ ] **B3 · Caja de alimentos, tres estados** — chroma verde — RF-25, RF-26, guion §6.1.2
- [ ] **B4 · Escenario del área de trabajo** — chroma **no** — RF-27, guion §6.2.2
- [ ] **B5 · Las seis piezas del taller** — chroma verde — RF-27, RF-28
- [ ] **B6 · La rueda y la carretilla, cinco estados de ensamblaje** — chroma verde — RF-28, RF-29, HU-09
- [ ] **B7 · Carretilla vista superior, cuatro orientaciones** — chroma **magenta** — RF-30, RF-31, **INC-33**
- [ ] **B8 · Tablero del laberinto y tres obstáculos** — chroma **no** — RF-30, RF-33, guion §6.3.2
- [ ] **B9 · Bloques Avanzar/Retroceder/Girar y botón Ejecutar** — chroma verde — RF-31, RF-32, **RNF-19**, PG-04
- [ ] **B10 · Contador de acopio e iconos de resultado** — chroma verde — RF-24, RF-11, RNF-19, RNF-20
- [ ] Postproceso: recorte del chroma, alfa, halo, **mismo `Pixels Per Unit` que el Slice 1**
- [ ] Cada asset pasa la **checklist de `Direccion_de_Arte.md` §17** y su línea «Verificación»
- [ ] Desaturar B2, B7, B8, B9 y B10 y verificar que se siguen distinguiendo (RNF-19)
- [ ] Verificar RNF-20 sobre el arte final: el bosque es claro, el texto va siempre sobre marco

---

## Bloqueantes y decisiones pendientes

- [ ] **R2 · Cerrar el Slice 1** hasta su Checkpoint D antes de abrir W02. Bloqueante duro.
- [ ] **R1 · Instalar el servidor MCP de Unity** (`run_unity_tests`). Este slice tiene ocho
      tareas `MCP` contra las seis del Slice 1: el costo de no tenerlo crece.
- [ ] **Pregunta abierta 1 · `Pasos utilizados` de la fase 1** sin definir en OE1 §3.6.1.
      Es un entregable radicado: **no se decide desde el código**. Hasta que se resuelva, W15 lo
      emite como «no aplica» y lo documenta.
- [ ] **Pregunta abierta 3 · Trazado del laberinto.** Validar `N2_MazeLayout.asset` jugando: al
      menos una solución que **exija girar**, ninguna que se resuelva con «Avanzar» repetido.
- [ ] **Pregunta abierta 4 · ¿Se adelantan W10..W12?** Recomendación: sí, para descargar INC-33.
- [ ] **PG-02 · nombre del guía.** El documento fuente del Nivel 2 lo llamaba «Algorim»; el guion
      adopta «Chispa». Confirmar antes de generar el arte de la escena 2.1.
- [ ] **PG-05 · cambio de esquema de control** N1 → N2. Verificar en el Checkpoint W-F.
