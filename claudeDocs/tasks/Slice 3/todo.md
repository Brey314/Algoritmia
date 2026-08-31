# Tablero — Slice 3: El Río

Plan técnico: [`plan.md`](plan.md). Contrato: `claudeDocs/SPEC.md`.
Cada tarea se cierra con su commit asociado (RNF-17, CT-11).

**Leyenda:** `EM` = EditMode (lógica pura, sin escena) · `PM` = PlayMode (integración) ·
`VV` = VisualVerification · `MCP` = la verificación **exige** el corredor de pruebas conectado.

> ⚠️ **R2 — los Slices 1 y 2 no están hechos.** Este slice generaliza piezas que aún no existen.
> No abrir R02 antes del Checkpoint W-F del Slice 2. Solo **R01**, **R09** y **R10** son
> independientes.

> ⚠️ **R1 abierto — no hay corredor de pruebas MCP.** `run_unity_tests` sigue sin conectar.
> Toda casilla marcada `MCP` exige haberla corrido **a mano** en el Test Runner y **declarar el
> resultado**. No dar por hecho que la suite pasó.

> ⚠️ **Pregunta abierta 1 bloquea R02.** «Fase» significa dos cosas en el Nivel 3 y de ello
> depende el formato de datos persistidos, que es **«preguntar primero»** (`SPEC.md` §Límites).

---

## Fase 0 — Cimientos del slice

- [ ] **R01 · Assembly `Game.Levels.River` y exclusión con tres niveles** — `XS` · `EM`
      RNF-15, RNF-16, INC-40 · depende de: Slice 2 W01
- [ ] **R02 · Desbloqueo del Nivel 3 y granularidad de fase** — `S` · `EM`
      RF-03, RF-04, RF-41, RNF-09, RNF-14, CP-02, HU-11, CU-09, CU-10, INC-27, supuestos 2/9/11
      · depende de: R01, Slice 2 W02 · **bloqueado por la pregunta abierta 1**

### ✅ Checkpoint R-A — Cimientos
- [ ] Compila sin errores ni warnings nuevos (`check_compile_errors`)
- [ ] Prueba de exclusión RNF-16 con **tres niveles reales**, corrida y **declarada**
- [ ] El menú habilita el Nivel 3 solo tras completar el Nivel 2
- [ ] **Pregunta abierta 1 resuelta con el usuario** — toca datos persistidos
- [ ] Revisado con el usuario

---

## Fase 1 — Andamiaje del Nivel 3 (`andamiaje`)

- [ ] **R03 · `HintPolicy` para recolección y ensamblaje** — `M` · `EM`
      RF-13, RF-10, RF-11, RNF-03, CP-06, HU-03, HU-04, CU-09, CU-10, INC-41 · depende de: R02
- [ ] **R04 · Las cinco secuencias narrativas del N3** (una **condicional**) — `S` · `EM` + `PM`
      RF-05, RF-06, RF-10, RF-12, RNF-01, RNF-18, CP-07, HU-02, INC-28, INC-39,
      guion §7, §8.1, §8.4.1, §8.5, §9 · depende de: R03

### ✅ Checkpoint R-B — Andamiaje del Nivel 3
- [ ] Las cinco escenas narrativas se recorren completas
- [ ] La escena 3.2 aparece tras un fallo y **no aparece** si se acierta al primer intento
- [ ] Ninguna pista del Nivel 3 resuelve la tarea (CP-06)
- [ ] Revisado con el usuario

---

## Fase 2 — Recolección (`nivel-rio`)

- [ ] **R05 · `TaskList` — las cuatro tareas y su correspondencia exacta** — `S` · `EM`
      RF-36, RF-11, RF-43, RNF-03, RNF-19, CP-02, CP-03, **INC-30**, HU-11, CU-09 (FA-5a), CU-10,
      supuesto 11, guion §8.1/§8.2 · depende de: R04
      ⚠️ Prueba **negativa** obligatoria: la fase de base **no** marca tarea (R4 del plan)
- [ ] **R06 · `Inventory`, `Collectible` y proximidad** — `M` · `EM`
      RF-37, RF-38, RF-11, CT-05, RNF-18, CP-02, HU-11, CU-09 (FA-4a) · depende de: R05
- [ ] **R07 · Escena `Level3_River` y movimiento con botones en pantalla** — `M` · `PM` `MCP`
      **RF-35**, RF-10, RF-13, RNF-02, RNF-03, CT-06, **INC-01**, supuesto 6, HU-11, CU-09,
      guion §2.1/§8.2 · depende de: R06
      ⚠️ Prueba sobre el **`.inputactions`**, no solo sobre el comportamiento: cero teclado
- [ ] **R08 · Zona de construcción** — `S` · `PM` `MCP`
      RF-39, RF-11, RF-04, CP-02, CP-03, HU-11, CU-09 (FA-6a) · depende de: R07

### ✅ Checkpoint R-C — Recolección completa
- [ ] Se recorre el mapa, se recogen los cuatro materiales y se entra a la zona de construcción
- [ ] **Ninguna tecla mueve al personaje** — inspección del mapa de controles (RNF-02, INC-01)
- [ ] Las tareas 1 y 2 quedan marcadas; las 3 y 4 siguen sin marcar
- [ ] Lista de tareas e inventario visibles todo el tiempo y sin solaparse
- [ ] Radio de proximidad de RF-37 validado jugando (pregunta abierta 3)
- [ ] Revisado con el usuario

---

## Fase 3 — Ensamblaje y depuración (`nivel-rio`)

> **R09 y R10 no dependen de R05..R08** y se pueden adelantar. Ver pregunta abierta 6.

- [ ] **R09 · `RaftAssembly` — tres fases bloqueantes** — `M` · `EM`
      RF-40, RF-41, RF-11, RF-17, RF-18, RNF-18, CP-02, CP-06, **INC-30**, HU-12, CU-10,
      guion §8.3 · depende de: R01
- [ ] **R10 · `RaftValidator` — prueba de balsa y depuración** — `M` · `EM`
      RF-42, RF-43, RF-11, RF-17, RF-18, CP-02, CP-03, CP-06, HU-13, CU-10 (FA-6a, FA-6b),
      guion §8.4 · depende de: R09
- [ ] **R11 · Panel de ensamblaje en escena** — `M` · `PM` + `VV` `MCP`
      RF-40, RF-41, RF-42, RF-43, RNF-02, RNF-03, RNF-19, RNF-21, CT-06, HU-12, HU-13,
      CU-10 · depende de: R10, R08
- [ ] **R12 · Escena 3.2, condicional al primer fallo** — `S` · `PM` `MCP`
      RF-05, RF-06, RF-11, RF-12, CP-02, CP-07, guion §8.4.1 · depende de: R11

### ✅ Checkpoint R-D — Ensamblaje completo
- [ ] La balsa se construye por las tres fases y se prueba
- [ ] Un fallo devuelve **solo** lo mal puesto y conserva las fases aprobadas (RF-43)
- [ ] La escena 3.2 aparece tras el primer fallo y no aparece si se acierta de una
- [ ] Ningún mensaje nombra la pieza correcta (CP-06)
- [ ] Revisado con el usuario

---

## Fase 4 — Cierre del nivel y del juego

- [ ] **R13 · Emisión de los cuatro indicadores del N3** — `M` · `EM`
      RF-45, RF-04, RF-07, RNF-09, RNF-14, CP-03, CP-09, OE1 §3.6.1 (notas 1–5), INC-27,
      INC-30 · depende de: R12, Slice 2 W15
- [ ] **R15 · Doble indicador y contraste en los estados de error del N3** — `S` · `VV` `MCP`
      **RNF-19** (cierra su segunda mitad), RNF-20, RNF-21, CN-04, HU-13 · depende de: R11
- [ ] **R16 · Cierre de RNF-02 y RNF-16 sobre el juego completo** — `S` · `PM` + `EM` `MCP`
      **RNF-02**, **RNF-16**, CT-06, **INC-01** · depende de: R11
- [ ] **R14 · Cruce, escena final y cierre del juego** — `M` · `EM` + `PM` `MCP`
      RF-44, RF-12, RF-45, RF-17, RF-08, RF-03, RNF-13, CP-03, CP-07, CP-10, HU-13, HU-14,
      CU-10, INC-26, INC-37, **INC-39**, guion §8.5/§9 · depende de: R13, R15, R16

### ✅ Checkpoint R-E — Slice 3 completo
- [ ] **Dos recorridos completos** del Nivel 3 sin incidencias (RNF-13)
- [ ] Un recorrido **acertando al primer intento** (sin escena 3.2) y otro **fallando** (con ella)
- [ ] Cierre forzado en cada fase confirmada → retoma donde iba (RNF-14)
- [ ] **RNF-02 cerrado**: cinco escenas jugables inspeccionadas, cero teclado (INC-01)
- [ ] **RNF-16 cerrado**: las tres combinaciones de exclusión
- [ ] Carga de `Level3_River` < 10 s y memoria < 2 GB, **medidas** (RNF-04, RNF-05)
- [ ] Paquete < 500 MB con el arte de los **tres** slices (RNF-06) — última oportunidad de verlo
- [ ] **PG-05** verificado sobre los tres niveles
- [ ] RF-35..RF-44 tienen cada uno al menos una prueba que los nombra (CT-10)
- [ ] **Golden Path del juego entero**, de la pantalla de inicio a los créditos, en 20–40 minutos
- [ ] Revisado con el usuario antes de abrir el Slice 4

---

## Assets visuales — `plan.md` §Assets visuales del Slice 3

Escenarios, props e interfaz **originales del proyecto**. **Los personajes no se rediseñan**:
Mamá es `A3` del Slice 1 —**obra derivada** con autorización concedida y mención obligatoria en
créditos (CT-09, RNF-23)— y `C2` solo genera su vista cenital **con sus rasgos copiados
literalmente**. Cada asset se registra en `CreditsContent.asset` (Slice 1, T08).

**Cinco bloques fijos por prompt**, copiados palabra por palabra antes de la descripción:
`[1 CONTEXTO] [2 ESTILO] [3 PALETA] [4 ENTREGA] [5 PROHIBICIONES]`. Un asset generado sin los
cinco se descarta y se vuelve a pedir. La paleta y las especificaciones salen de
`claudeDocs/Direccion_de_Arte.md`.

**Todo el nivel se dibuja en vista cenital pura de 90 grados**, salvo `C10`, que es la
ilustración lateral de la escena final.

**Chroma:** verde `#00FF00` por defecto; **magenta `#FF00FF`** en `C2`, porque el personaje se
recorta sobre un entorno de follaje. Los fondos de escena no llevan chroma.

- [ ] **C1 · Escenario del río, vista superior** — chroma **no** — RF-35, RF-39, guion §8/§8.2
- [ ] **C2 · Mamá vista superior, cuatro direcciones** — chroma **magenta** — RF-35, CU-09, HU-11
      ⚠️ pegar el bloque «RASGOS FÍSICOS FIJOS» de `A3` (Slice 1) literalmente
- [ ] **C3 · Botones de dirección y botón «Recoger»** — chroma verde — **RF-35**, RF-37, RNF-02,
      RNF-19, **INC-01**
- [ ] **C4 · Los cuatro materiales + sus iconos de inventario** — chroma verde — RF-37, RF-38, RNF-19
- [ ] **C5 · Lista de tareas e inventario** — chroma verde — RF-36, RF-38, RNF-19, RNF-20, INC-41
- [ ] **C6 · Zona de construcción, dos estados** — chroma verde — RF-39, CU-09 (FA-6a)
- [ ] **C7 · La balsa en tres estados: base / amarre / mástil y vela** — chroma verde — RF-40,
      RF-41, HU-12, guion §8.3
- [ ] **C8 · Panel de ensamblaje: espacio vacío / correcto / incorrecto** — chroma verde — RF-40,
      RF-42, **RNF-19**, HU-13
- [ ] **C9 · Balsa hundiéndose y balsa cruzando** — chroma verde — RF-42, RF-44, RNF-21, guion §8.4/§8.5
- [ ] **C10 · Escenario de la escena final, las fogatas** — chroma **no** — RF-44, RF-12, guion §9
- [ ] Postproceso: recorte del verde, alfa, halo, **mismo `Pixels Per Unit` que los Slices 1 y 2**
- [ ] Desaturar `C3`, `C4`, `C5` y `C8` y verificar que se siguen distinguiendo (RNF-19)
- [ ] Verificar RNF-20 sobre el arte final: escenario claro y cenital, el caso más expuesto
- [ ] Verificar RNF-21 sobre las **animaciones** montadas con `C7` y `C9`, no sobre las láminas

---

## Bloqueantes y decisiones pendientes

- [ ] **R2 · Cerrar los Slices 1 y 2** antes de abrir R02. Bloqueante duro.
- [ ] **Pregunta abierta 1 · ¿Qué es una «fase» del Nivel 3 para el guardado?** CU-09/CU-10 dicen
      dos; RF-40 y §3.6.1 dicen tres de ensamblaje. Propuesta: cuatro puntos de guardado
      —recolección, base, amarre, mástil y vela— con `Pasos utilizados` contando solo los tres de
      ensamblaje. **Cambiar el formato de datos persistidos es «preguntar primero».** Bloquea R02.
- [ ] **R1 · Instalar el servidor MCP de Unity** (`run_unity_tests`). Siete tareas `MCP` en el
      slice que cierra el juego.
- [ ] **Pregunta abierta 2 · Trazado del escenario del río.** Validar `N3_RiverLevelConfig.asset`
      jugando: ningún material visible desde la posición inicial, zona de construcción señalizada
      desde el principio.
- [ ] **Pregunta abierta 3 · Radio de proximidad de RF-37**, sin validar. Revisar en R-C.
- [ ] **Pregunta abierta 6 · ¿Se adelantan R09 y R10?** Recomendación: sí, para descargar INC-30.
- [ ] **PG-02 · nombre del guía.** Última oportunidad antes de la entrega: la escena final lo nombra.
- [ ] **PG-01 · título del producto.** Sigue abierto y el juego ya estaría completo (RF-01, RF-08).
