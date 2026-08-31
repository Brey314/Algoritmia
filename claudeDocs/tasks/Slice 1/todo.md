# Tablero — Slice 1: Golden Path temprano

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification.

> ⚠️ **R1 abierto — no hay corredor de pruebas MCP.** `run_unity_tests` no está conectado.
> Toda casilla de prueba marcada abajo exige haberla corrido **a mano** en la ventana Test
> Runner del Editor y **declarar el resultado**. No dar por hecho que la suite pasó.

---

## Fase 0 — Cimientos

- [ ] **T01 · Estructura de carpetas y assemblies** — `S` · `EM`
      RNF-15, RNF-16, INC-40 · depende de: —
- [ ] **T02 · `PlayerProfile` y `SaveStore` (JSON en `Datos/`)** — `M` · `EM`
      RF-02, RF-04, RNF-07, RNF-09, RNF-11, RNF-14, HU-01, CU-01, INC-27, INC-34 · depende de: T01
- [ ] **T03 · `GameFlow`, la FSM en C# plano** — `M` · `EM`
      RF-01, RF-03, RF-05, RF-07, RF-08, RF-09, CP-02 · depende de: T01, T02
- [ ] **T04 · `SceneLoader`, `GameFlowRunner` y escena `Boot`** — `S` · `PM`
      RNF-04, RNF-16 · depende de: T03

### ✅ Checkpoint A — Cimientos
- [ ] Compila sin errores ni warnings nuevos (`check_compile_errors`)
- [ ] Pruebas EditMode de Core corridas a mano y **declaradas**
- [ ] Arranca en `Boot` y llega a `MainMenu`
- [ ] Revisado con el usuario

---

## Fase 1 — Navegación mínima (`sistema-navegacion`)

- [ ] **T05 · Pantalla de inicio** — `S` · `PM` + `VV`
      RF-01, RF-09, RNF-01, RNF-20, HU-01, CU-01, PG-01 · depende de: T04
- [ ] **T06 · Perfil de un solo nombre** — `M` · `EM` + `PM`
      RF-02, RF-03, RNF-09, HU-01 (FA-01..FA-03), CU-01, CU-02 · depende de: T05
- [ ] **T07 · Menú de niveles con desbloqueo progresivo** — `M` · `EM` + `PM`
      RF-03, RNF-19, RNF-20, HU-01, CU-02 · depende de: T06
- [ ] **T08 · Pantalla de créditos mínima** — `XS` · `PM`
      RF-08, CT-09, RNF-18, RNF-23 · depende de: T05

### ✅ Checkpoint B — Navegación
- [ ] Perfil nuevo → Nivel 1 habilitado, Niveles 2 y 3 bloqueados **con icono además de color**
- [ ] Cerrar y reabrir conserva el perfil y su progreso (RNF-14, manual)
- [ ] `Datos/` aparece junto al ejecutable, sin residuos fuera de ella (RNF-07)
- [ ] Revisado con el usuario

---

## Fase 2 — Andamiaje mínimo (`andamiaje`)

- [ ] **T09 · `NarrativeSequence` y `DialogueRunner`** — `M` · `EM`
      RF-05, RF-06, RF-10, RNF-01, RNF-18, HU-02, INC-28 · depende de: T07
- [ ] **T10 · Escena `Narrative` parametrizada + tres secuencias del N1** — `M` · `PM`
      RF-05, RF-06, RNF-06, guion §3.1/§4.1/§4.2 · depende de: T09
- [ ] **T11 · `HintPolicy`: ayuda a demanda + pista tras tres fallos** — `M` · `EM`
      RF-13, RNF-03, CP-06, HU-03, HU-04, guion §4.3.6 · depende de: T09

### ✅ Checkpoint C — Andamiaje
- [ ] Las tres escenas narrativas se recorren completas
- [ ] El botón de omitir aparece **solo** en la segunda visita (INC-28)
- [ ] Ninguna pista resuelve la tarea ni nombra «Muy cerca» (CP-06)
- [ ] Revisado con el usuario

---

## Fase 3 — Nivel fuego (`nivel-fuego`)

- [ ] **T12 · `FireLevelConfig`, `StrikePosition`, `FireAttempt`** — `M` · `EM`
      RF-15, RF-16, RF-18, RF-19, CP-02, CT-05, RNF-18, HU-06, HU-07, CU-04, INC-32 · depende de: T11
- [ ] **T13 · `FireFeedbackLog`, mensajes sin repetición** — `M` · `EM`
      RF-11, RF-17, RF-18, CP-03, HU-05, HU-06, guion §4.3.4 · depende de: T12
- [ ] **T14 · Panel de encendido y escena `Level1_Cave`** — `M` · `PM`
      RF-14, RF-15, RF-17, RNF-02, RNF-03, RNF-19, CT-06, HU-06, CU-04, INC-41 · depende de: T13
- [ ] **T15 · Convergencia: «Soplar» → nacimiento del fuego** — `S` · `PM` + `VV`
      RF-19, RF-20, RF-04, RF-03, RNF-21, HU-07, CU-04 · depende de: T14
- [ ] **T16 · Menú de pausa** — `M` · `EM` + `PM`
      RF-07, RF-03, RF-04, CP-02, HU-17 (FA-01..FA-05), INC-25 · depende de: T15
- [ ] **T17 · Emisión de los cuatro indicadores del N1** — `M` · `EM`
      RF-45, RF-04, RNF-14, CP-03, CP-09, OE1 §3.6.1, INC-29 · depende de: T15, T16
- [ ] **T18 · Resumen de fin de nivel y cierre reflexivo** — `M` · `EM` + `PM`
      RF-45, RF-12, RF-17, RF-03, CP-03, CP-07, HU-14, INC-26 · depende de: T17
- [ ] **T19 · Iluminación progresiva del escenario** — `S` · `VV`
      **RF-21 (prioridad Baja)**, RNF-20, RNF-21, HU-07 · depende de: T15

### ✅ Checkpoint D — Slice 1 completo
- [ ] **Dos recorridos completos** del Golden Path sin incidencias (RNF-13)
- [ ] Cierre forzado a mitad de nivel → retoma desde la última fase confirmada (RNF-14)
- [ ] Carga de escena < 10 s y memoria < 2 GB, **medidas** en el equipo de referencia (RNF-04, RNF-05)
- [ ] Ejecución portable con el adaptador de red deshabilitado (RNF-07, RNF-08)
- [ ] Todo RF del slice tiene al menos una prueba que lo nombra (CT-10)
- [ ] Revisado con el usuario antes de abrir el Slice 2

---

## Assets visuales — `plan.md` §Assets visuales del Slice 1

Personajes = **obra derivada** de los diseños Anonaky con **autorización escrita concedida**
(PG-07 cerrado): su reconocimiento en créditos es obligatorio. Entornos, props e interfaz son
originales del proyecto (CT-09, RNF-23).
Cada asset generado se registra en `CreditsContent.asset` (T08).

**Cinco bloques fijos por prompt**, copiados palabra por palabra antes de la descripción:
`[1 CONTEXTO] [2 ESTILO] [3 PALETA] [4 ENTREGA] [5 PROHIBICIONES]`. Un asset generado sin los
cinco se descarta y se vuelve a pedir. La paleta y las especificaciones salen de
`claudeDocs/Direccion_de_Arte.md`.

**Los personajes se piden en A-pose**, no en poses de acción: las poses del nivel se producen
animando el sprite con 2D Animation (`Direccion_de_Arte.md` §7.5 y §13.1). Pedirle a Gemini tres
poses del mismo personaje devuelve tres personajes distintos.

- [ ] **A1 · Chispa, el guía** — chroma sí — guion §1.1/§4.1, PG-02, RF-10, RF-12, RF-13
- [ ] **A2 · Papá (jugable N1)** — chroma sí — **A-pose** — guion §1.1/§4.2, RF-14, HU-06, CN-02
- [ ] **A3 · Mamá** — chroma sí — **A-pose** — guion §1.1/§4.2
- [ ] **A4 · Niña** — chroma sí — **A-pose**, penacho alto — guion §1.1/§4.2
- [ ] **A5 · Niño** — chroma sí — **A-pose**, copete hacia adelante — guion §1.1/§4.2
- [ ] **A6 · Cueva, cuatro escalones de luz** — chroma **no** — guion §3.1/§4, RF-21
- [ ] **A7 · Montón de hojas, cuatro estados** — chroma sí — guion §4.3.1/§4.3.3, RF-14, RF-16
- [ ] **A8 · Sílex y pedernal** — chroma sí — guion §4.1/§4.2, RF-16, RNF-19
- [ ] **A9 · Controles del panel de encendido** — chroma sí — RF-14, RF-15, RF-19, RNF-19
- [ ] **A10 · Marco de diálogo del guía** — chroma sí — RF-05, RF-06, RNF-20
- [ ] Cada asset pasa la **checklist de `Direccion_de_Arte.md` §17** y su línea «Verificación»
- [ ] Postproceso: recorte del verde, alfa, halo, nombre según §15.4, import con los ajustes de
      §15.2 (**PPU 100**, pivot `Bottom` en personajes y `Center` en props e interfaz)
- [ ] Verificar RNF-20 y RNF-19 sobre el arte final, no sobre el prompt

---

## Bloqueantes y decisiones pendientes

- [ ] **R1 · Instalar el servidor MCP de Unity** (`run_unity_tests`). Sin él no hay flujo
      test-first automatizado. Conviene resolverlo **antes de T02**.
- [ ] **PG-01** · cadena provisional del título para `GameTitleConfig` (T05). Propuesta: «Chispa».
- [ ] **T08** · confirmar que los créditos entran en el Slice 1 (RF-01 pone el botón en el inicio).
- [ ] **PG-06** · validar jugando los valores de `FireLevelConfig` en el Checkpoint D.
