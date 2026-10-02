# Tablero — OE4: Evaluación funcional del prototipo

Plan: [`plan.md`](plan.md) · Casos y guiones: [`casos.md`](casos.md) · Resultados:
`OE4-Resultados.md` (nace en T04). Contrato: `claudeDocs/SPEC.md`.

**Leyenda:** tamaño `XS`–`L` · ejecutor `EXE` (Claude con el arnés sobre el ejecutable), `INSP`
(inspección de archivos), `SUITE` (`unity test`), `EDIT` (Editor abierto por `coplay-mcp`), `HUM`
(Santiago). Una sesión EXE se da por **hecha** cuando cada caso que toca tiene veredicto en
`OE4-Resultados.md`, con evidencia y, si fue F, su `DEF-nn`. **Hecha no significa aprobada.**

> ⚠️ **Nada de esto empieza sin T01.** Probar algo que no está congelado deja veredictos que no
> se pueden atribuir a ninguna versión.

> ⚠️ **Editor cerrado** durante SUITE y durante toda sesión EXE que mida carga o memoria. Dos
> carriles no corren `unity test` a la vez (CLAUDE.md §Comandos): avisar al otro carril antes.

---

## Fase 0 — Preparación

- [x] **T01 · Congelar la versión candidata `oe4-rc1`** — `XS` · HUM + git
      `plan.md` §2, §12 preguntas 1 y 7 · depende de: —
      Santiago decide: integrar a `main`, qué hacer con los paquetes de IA y la analítica, y en
      qué commit se congela. Claude crea la etiqueta **solo con su visto bueno** (es una acción
      hacia fuera si se sube).
      *Acepta:* existe `oe4-rc1` sobre un commit con el árbol limpio; la decisión sobre los
      paquetes de IA queda escrita en `plan.md` §12.
      *Verifica:* `git tag --list oe4-rc1` y `git status` limpio en ese commit.
      *(01/10/2026) Sustituida por decisión de Santiago (01/10/2026: un único commit al final, sin
      etiquetas, `evidencias/build-rc1.md`; deja sin efecto la etiqueta de la parada P1 del acta
      D10): hecha **sin etiqueta**, así que no existe `oe4-rc1` ni `oe4-rc2`. Cada candidato se identifica por su procedencia
      —HEAD `359365e`, huellas de `git diff HEAD` y de los archivos sin seguimiento, y la huella
      del contenido de la carpeta del build— en `evidencias/build-rc1.md` y
      `evidencias/build-rc2.md`. La decisión sobre los paquetes de IA y la analítica es D3: no se
      quita ningún paquete del manifiesto (INC-97, `build-rc1.md`), y los servicios de Unity se
      apagan al compilar (`UnityServicesOff`, DEF-SPIKE-01, `OE4-Resultados.md` §8.3). La
      decisión sobre los paquetes de IA consta en `plan.md` §12, pregunta 7 (29/09).*
- [x] **T02 · Compilar `rc1` y registrar su procedencia** — `S` · SUITE (CLI)
      RNF-06, CT-03 · depende de: T01
      Renombrar el `Build/Algoritmia` del 21/09 a `Build/Algoritmia_2026-09-21` (no borrarlo).
      `unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe" --log-file
      build.log --no-banner --non-interactive` con el Editor cerrado.
      *Acepta:* el build tiene **12** escenas (`level0..level11` en `Algoritmia_Data`) con `Boot`
      primero; `build.log` sin errores.
      *Verifica:* commit, `Algoritmia.provenance.json`, SHA-256 del exe y el tamaño bruto,
      anotados para la cabecera de resultados.
      ⚠️ Cada corrida de Unity en batchmode quita `SENTIS_ANALYTICS_ENABLED` de
      `ProjectSettings.asset`, y el commit lo trae puesto. Después de compilar, `git diff
      ProjectSettings/ProjectSettings.asset`: anotar con qué símbolos quedó compilado el ejecutable
      (lo usa PF-RNF10-01) y restaurar el archivo con `git checkout`.
      *(01/10/2026) rc1 compilado a las 08:29:53–08:30:10 por la API del pipeline con el Editor
      abierto (no por `unity build`): 12 escenas con `Boot` primera, 0 errores, `Licencias/`
      junto al exe. Procedencia, SHA-256, huellas y tamaño (479,0 MB) en `evidencias/build-rc1.md`,
      que hace las veces del `Algoritmia.provenance.json`. El build del 21/09 se conserva como
      `Build/Algoritmia_2026-09-21`; `ProjectSettings` se restauró tras el build. rc2 lo sustituyó a
      las 18:04 (`evidencias/build-rc2.md`).*
- [x] **T03 · Arnés `herramientas/oe4.ps1` y su spike** — `M` · EXE
      `plan.md` §5 · depende de: T02 · **riesgo alto: va primero**
      *Acepta:* las funciones de `plan.md` §5 existen, y el spike pasa: se lanza el juego en
      ventana, se captura el menú, se va a «Créditos» y se vuelve con clics, se arrastra una pieza
      del N1 al círculo, Mamá se mueve con clic sostenido en una flecha y `Medir-Carga` devuelve
      un número plausible.
      *Verifica:* capturas del spike en el scratchpad; `mem.csv` y `red.csv` con filas.
      *Si falla la entrada sintetizada:* no forzar. Se documenta lo probado y se lleva el plan B
      al Checkpoint A.
      Archivos: `claudeDocs/tasks/OE4/herramientas/oe4.ps1`.
      *(01/10/2026) El spike pasa (S-SPIKE, 08:45–09:00): ventana, menú, «Créditos» y vuelta con
      clics, arrastre en la cueva, Mamá movida con clic sostenido en la flecha → y cargas de 0,04 a
      0,75 s, con cuatro arreglos del arnés. Evidencia: `evidencias/S-SPIKE/` (registro, fotos,
      `mem.csv`, `red.csv`). Guía de uso: `herramientas/oe4-USO.md`.*
- [ ] **T04 · Semillas, contraste del catálogo y esqueleto de resultados** — `S` · INSP
      `plan.md` §7, §3.1 punto 5, §8 · depende de: T02
      Escribir las 8 semillas de `plan.md` §7 en `herramientas/perfiles/`, con los nombres de campo
      verificados contra `PlayerProfile.cs`. Contrastar **cada** texto entre «» de `casos.md` con
      los assets de `oe4-rc1` (grep) y corregir el catálogo si algo cambió. Crear
      `OE4-Resultados.md` con cabecera (T02), la tabla de casos vacía (todos los `PF-*` de
      `casos.md`) y las secciones de defectos, matriz, KPI y mejoras.
      *Acepta:* cada semilla carga en el juego (el menú de niveles muestra lo esperado) y cero
      textos del catálogo difieren de `rc1`.
      *Verifica:* `Preparar-Datos OE4_Z` → «Jugar» → «OE4_Z» → los tres niveles habilitados.
      Archivos: `herramientas/perfiles/*.json`, `OE4-Resultados.md`, `casos.md` (solo si hubo
      diferencias).
      *(01/10/2026, sigue abierta) Hechas las semillas (las ocho del plan y `OE4_Corrupto`),
      validadas por `Preparar-Datos` y cargadas en las sesiones (S-SPIKE paso 28: `OE4_Z` con los
      tres niveles «Completado»), y creado `OE4-Resultados.md`. Falta el contraste sistemático de
      cada «texto» de `casos.md` con los assets del candidato, y las secciones de matriz, KPI y
      mejoras, que llegan con T24–T27.*

### Checkpoint A — Listos para probar
- [x] `oe4-rc1` congelado y compilado; SHA-256 en la cabecera de resultados
      *(01/10/2026) Congelado por procedencia, sin etiqueta (T01). SHA-256 del exe, huellas y tamaño de rc1 en
      `OE4-Resultados.md` §1; los de rc2, en §8.1.*
- [x] El arnés pasó el spike, **o** Santiago eligió el plan B (HUM o EDIT) y quedó escrito
      *(01/10/2026) Pasó el spike (T03); no hizo falta el plan B.*
- [ ] Las semillas cargan; el catálogo coincide con `rc1`
      *(01/10/2026) Las semillas cargan; el contraste del catálogo sigue pendiente (T04).*
- [ ] Revisado con Santiago

---

## Fase 1 — Línea base

- [ ] **T05 · Suite completa sobre `rc1` y matriz automática** — `M` · SUITE
      CT-10 · depende de: T01
      `unity test --mode EditMode …` y `--mode PlayMode …` con salida en `evidencias/`. Se
      conocen de antes: `RiverLevel_RNF05_*` (mide el Editor, no el juego),
      `RiverLevel_RNF21_*` (sensible al tiempo; se reintenta sola) y
      `NarrativeScene_RNF01_LaLineaMasLarga…` (la pantalla de 640×480 del batchmode;
      `Slice 1/Fase-5-6-Resultados.md:367`). Si el Editor está abierto (quizá para coplay), se
      cierra con `unity close .` y se reabre al terminar.
      Grep de `_RFnn_` / `_RNFnn_` en los nombres de método de `Assets/Tests` → cuántas pruebas
      nombran cada requerimiento y cuántas pasaron.
      *Acepta:* resultado de las dos corridas en resultados; toda falla nueva, con un `DEF`.
      *Verifica:* `evidencias/suite-rc1-EditMode.xml`, `…-PlayMode.xml`.
      *(01/10/2026, sigue abierta) Sobre el árbol de rc1 corrió la EditMode completa (434 = 433 + 1
      omitida, `evidencias/suites/W4b/`), pero no la PlayMode: la de 364/364 (`suites/final/`) es
      anterior al cambio de importación de W4b (`build-rc1.md`). Sobre rc2, que sustituye a rc1, la
      SUITE completa sí está: PlayMode 376/376 y EditMode 470 = 469 + 1 omitida, sin fallos
      (`suites/rc2-final/`, `OE4-Resultados.md` §8.1). La matriz por nombre de prueba la escribe
      `claudeDocs/entregables/OE3/tools/rf_matrix.py` (Anexo A del OE3), pero sin la columna de
      cuántas pasaron por requerimiento.*
- [ ] **T06 · Inspecciones estáticas** — `M` · INSP (puede ir a un subagente)
      PF-RNF01-01, PF-RNF15-01, PF-RNF17-01, PF-RNF22-01 (textos), PF-RNF23-01 (créditos);
      CT-10 del catálogo · depende de: T01
      *Acepta:* los cinco casos con veredicto; el grep de `PF-RFnn` y `PF-RNFnn` en `casos.md`
      cubre RF-01..47 y RNF-01..23 sin huecos.
      *Verifica:* los scripts de un solo uso quedan en el scratchpad y su salida, en evidencias.
      *(01/10/2026, sigue abierta) Comprobado que las tablas de `casos.md` cubren RF-01..47 y
      RNF-01..23 sin huecos (127 casos). Los cinco casos de la tarea siguen sin ejecutar.*

---

## Fase 2 — Recorridos sobre el ejecutable

Cada tarea es una sesión de `casos.md`. Al cerrarla: veredictos, evidencias, JSON final copiado y
`Revisar-Log`.

- [ ] **T07 · S-INI · Inicio, perfiles, créditos y salida** — `S` · EXE · depende de: A
      *Acepta:* 11 casos con veredicto (PF-RF01-01/02, PF-RF02-01 parcial/02/03, PF-RF03-01/03,
      PF-RF08-01, PF-RF09-01/02/03).
      *(01/10/2026, sigue abierta) Sin veredicto PF-RF01-02, PF-RF02-03 y PF-RF03-01, y PF-RF02-01
      en P parcial (faltan los rechazos). PF-RF02-06, que entró en la sesión el 01/10 (pasos
      18–21), es P sobre rc2 (`OE4-Resultados.md` §8.4).*
- [ ] **T08 · S-N1 · Nivel 1 completo con perfil nuevo** — `M` · EXE · depende de: A
      *Acepta:* los 20 casos de la sesión con veredicto; las cifras del JSON contrastadas con
      I 5 · C 1 · P 3 y el tiempo menos 30 s.
      *(01/10/2026, sigue abierta) 18 de 20 con veredicto, en GP1 tramo 1 y en S-INI-rc2 (pasos
      17–22 sobre rc2). N1F1 = 5 · 1 · 3 en GP1. Faltan PF-RF11-01 y PF-RF21-01.*
- [ ] **T09 · S-N2A · Bosque** — `S` · EXE · depende de: A
      *Acepta:* 10 casos con veredicto; N2F1 = I 3 · C 1 · P 0.
      *(01/10/2026, sigue abierta) Los 10 con veredicto y N2F1 = 3 · 1 · 0 en GP1 tramo 2, pero
      PF-RF11-02 está en P parcial: el «< 1 s» no se midió en todas las acciones.*
- [x] **T10 · S-N2B · Taller** — `S` · EXE · depende de: T09 (o semilla `OE4_B2`)
      *Acepta:* 4 casos más con veredicto; N2F2 = I 5 · C 3 · P 6.
      *(01/10/2026) Los cuatro en P y N2F2 = 5 · 3 · 6 · 217,93 s, como lo previsto (GP1 tramo 2,
      `evidencias/GP1/GP1_tramo2_registro_de_pasos.md`; `GP1/GP1_OE4GP1_tras_N2.json`).
      PF-RF11-02, compartido con T09, sigue en P parcial y se cuenta allí.*
- [x] **T11 · S-N2C · Laberinto y cierre del N2** — `M` · EXE · depende de: T10 (o `OE4_B3`)
      *Acepta:* 9 casos más con veredicto; la rejilla transcrita y el conteo de ediciones, en el
      registro de la sesión; N2F3 igual a lo previsto.
      ⚠️ PF-RF34-02 puede dejar el bloque pegado: recuperar y seguir; no investigar el código
      en esta tarea.
      *(01/10/2026) Hecha entera sobre rc2 (S-N2C-rc2, 19:56–20:08): los nueve casos en P, el
      tablero transcrito, el conteo de ediciones y N2F3 = 3 · 40 · 9 · 497,81 s frente a 497,70
      previsto. Sobre rc1 había dejado DEF-GP1-01, -02 y -03, corregidos en rc2.
      `evidencias/rc2/S-N2C-rc2_registro_de_pasos.md`, `OE4-Resultados.md` §8.3–§8.4.
      PF-RF11-02, compartido con T09, sigue en P parcial y se cuenta allí.*
- [ ] **T12 · S-N3 · Nivel 3 y cierre del juego** — `M` · EXE · depende de: A
      *Acepta:* 18 casos con veredicto; N3 = 1/1/1 · 4/1/2 · 6/3/3.
      *(01/10/2026, sigue abierta) Los 19 casos que hoy tiene la sesión tienen veredicto, pero
      PF-RF11-03, PF-RF13-05, PF-RF42-01 y PF-RF42-02 están en P parcial y PF-RF40-02 en NE: GP1
      no hizo la prueba del amarre ni el segundo fallo. Las cifras previstas son hoy N3F1 2/1/1 ·
      N3F2 5/2/2 · N3F3 7/4/3 (`casos.md` S-N3).*

### Checkpoint B — Golden Path negro completo
- [x] Los cuatro niveles del recorrido (N1, N2 × 3, N3) jugados de punta a punta sobre `rc1`
      *(01/10/2026) GP1 (87 min) y GP2 (33 min) sobre rc1, y GP3 (16 min) sobre rc2, sin
      bloqueos ni estados irrecuperables (`OE4-Resultados.md` §3 y §8.5).*
- [x] Ningún `DEF` Bloqueante sin triar: si lo hay, se lleva a Santiago **antes** de seguir
      *(01/10/2026) Ninguno fue Bloqueante (`OE4-Resultados.md` §6).*
- [x] Eficacia parcial calculada y anotada
      *(01/10/2026) rc1: 77 / 83 = 92,8 % (§7); rc2: 89 / 89 = 100 %, sin contar los P parcial
      ni el NE (§8.8). No es el KPI.*
- [ ] Revisado con Santiago

- [ ] **T13 · S-PAU · Pausa y reinicio en las cinco escenas** — `M` · EXE · depende de: B
      *Acepta:* PF-RF07-01..04 con veredicto en las seis filas de la tabla de la sesión.
- [ ] **T14 · S-OMI · Omisión, rejuego y negativos del N1** — `M` · EXE · depende de: B
      *Acepta:* PF-RF06-02..04, PF-RF20-02, PF-RF21-02, PF-RF45-04 y PF-RF45-05 con veredicto.
      *(01/10/2026, sigue abierta) PF-RF06-02, PF-RF06-03, PF-RF20-02, PF-RF45-04 y PF-RF45-05 en
      P (S-INI-rc2, `OE4-Resultados.md` §8.4). Faltan PF-RF06-04 y PF-RF21-02.*
- [x] **T15 · S-PER · Persistencia, cierre forzado y robustez** — `M` · EXE · depende de: B
      *Acepta:* PF-RNF14-01..05, PF-RF02-04 y PF-RF02-05 con veredicto.
      *(01/10/2026) Los siete en P: S-PER sobre rc1, con siete cierres forzados, y el bloque D más
      dos cierres forzados en el instante del guardado sobre rc2 (S-PER-rc2), que cierran
      DEF-SPER-02. `evidencias/S-PER/` y `evidencias/rc2/S-PER-rc2_registro_de_pasos.md`;
      `OE4-Resultados.md` §5 y §8.3–§8.4.*
- [x] **T16 · S-DOC · Informe docente, borrado y residuos** — `M` · EXE + INSP
      depende de: T08, T11, T12 (usa sus JSON finales)
      *Acepta:* PF-RF46-01..03, PF-RF47-01..03, PF-RNF11-01 y PF-RNF09-01 con veredicto; la
      carpeta `Datos/` restaurada al final.
      *(01/10/2026) Los ocho en P (S-DOC, 12:16–12:27, con los JSON reales de GP1 y S-PER en lugar
      de los de S-N1 y S-N3), `Datos/` y el registro restaurados al final; PF-RF46-01 y
      PF-RF47-01 se repitieron en P sobre rc2. `evidencias/S-DOC/S-DOC_registro_de_pasos.md`,
      `OE4-Resultados.md` §5 y §8.4.*
- [ ] **T17 · S-INSP · Accesibilidad y RNF restantes** — `M` · INSP + EDIT + SUITE
      PF-RNF02-01, 03-01, 16-01, 18-01, 19-01, 20-01, 21-01 · depende de: T08–T12 (capturas)
      *Acepta:* los siete casos con veredicto. RNF-18 queda **revertido**, con `git status`
      limpio. El worktree de RNF-16 queda **eliminado**.

### Checkpoint C — Primera pasada completa
- [ ] Todo caso EXE, INSP, SUITE y EDIT con veredicto; cero B sin justificar
- [ ] Eficacia de la primera pasada calculada (global y por etiqueta) y KPI-2 preliminar
      *(01/10/2026) Hecha la global (`OE4-Resultados.md` §7 y §8.8) y el KPI-2 preliminar (§6 y
      §8.8); falta la eficacia por etiqueta.*
- [x] Lista de `DEF` con severidad y tipo propuesto
      *(01/10/2026) `OE4-Resultados.md` §6 (rc1, con su estado tras rc2) y §8.7 (DEF-RC2-01 y el
      riesgo residual R-RC2-A).*
- [ ] Revisado con Santiago

---

## Fase 3 — Personas y otros equipos

- [ ] **T18 · Hoja de pruebas humanas** — `S` · INSP · depende de: T04
      Para Santiago: Golden Path ×2 con cronómetro, hojas de sonido por nivel (solo las piezas
      **cableadas** según `Direccion_de_Musica_y_Sonido.md` §19), equipo 2, equipo sin GPU,
      modo avión, y los documentos de RNF-12, RNF-22 y RNF-23.
      *Acepta:* una hoja que se puede imprimir, con una casilla y un campo de nota por cada
      comprobación, y cada fila con su caso `PF-*`.
      Archivos: `claudeDocs/tasks/OE4/Hoja-HUM.md`.
      *(01/10/2026, sigue abierta) La hoja tiene trece guiones (H1–H13), cada uno con su caso, su
      casilla y su plantilla, y el formato de RNF-12 está en `Consentimiento-RNF12.md`. Faltan en
      ella las confirmaciones documentales de PF-RNF22-01 y PF-RNF23-01.*
- [ ] **T19 · Ejecución HUM** — `M` · HUM (+ Claude en PF-RNF08-01)
      PF-RNF13-01, PF-SON-01..03, PF-RNF07-02, PF-CT02-01, PF-RNF12-01, PF-RNF22-01,
      PF-RNF23-01 · depende de: T18, A
      *Acepta:* Claude transcribe lo que reporte Santiago, sin completar nada, y cada caso tiene
      veredicto o queda B con motivo.
- [ ] **T20 · S-RNF · Consolidar las medidas** — `S` · INSP + EXE · depende de: T07–T17, T19
      PF-RNF04-01, 05-01, 06-01, 07-01, 08-01, 10-01
      *Acepta:* la tabla de cargas de las 12 escenas (3 mediciones en el equipo 1, más las del
      equipo 2), los picos de memoria, el tamaño y el análisis de red, con su veredicto. Se llena
      también la tabla `P12` de `claudeDocs/tasks/Slice 4/todo.md`.
      *(01/10/2026, sigue abierta) Hecha la parte del equipo 1, sobre rc1 (S-RNF, §4) y sobre rc2
      (S-RNF-rc2, §8.6): cargas < 1 s, memoria privada máx. 1 124,6 MiB, 479,0 MB y ninguna conexión
      observada. Faltan el equipo 2 y PF-RNF08-01 (T19).*

### Checkpoint D — Primera pasada, incluida la humana
- [ ] Los ~120 casos con veredicto
      *(01/10/2026) El catálogo tiene 127: 103 con veredicto (89 P, 13 P parcial, 1 NE) y 24 sin
      ejecutar (`OE4-Resultados.md` §8.8).*
- [ ] KPI de la primera pasada **publicado** en resultados, aunque no llegue al 90 %
- [ ] Revisado con Santiago

---

## Fase 4 — Corrección y regresión

- [ ] **T21 · Triaje** — `S` · HUM + INSP · depende de: D
      Santiago decide cada `DEF`: corregir en OE4, registrar un INC o dejarlo como trabajo futuro.
      Los F de criterios de HU sin INC (PF-RF03-03, PF-RF09-02, PF-RF19-02, PF-RF35-01,
      PF-RF40-02…) se deciden aquí.
      *Acepta:* cada `DEF` con decisión y fecha.
      *(01/10/2026, sigue abierta hasta la pasada humana) Los defectos y observaciones hallados
      hasta rc2 ya tienen decisión y fecha, tomada por Claude por delegación de Santiago: los nueve
      de rc1 se corrigieron en rc2 (`OE4-Resultados.md` §8.3), y DEF-RC2-01, R-RC2-A y las
      observaciones abiertas siguen las decisiones D-RC2-01, D-RRA, D-SCR y D-OBS (§8.10).*
- [ ] **T22 · Correcciones** — `S` a `M` cada una · código (`fix-bug`) · depende de: T21
      Una subtarea por `DEF` aprobado, que se añade aquí al abrirse: prueba que falla, con el RF en
      su nombre → corrección → suite del módulo → mensaje de commit (sin hacer el commit).
      Las nueve de rc2 (01/10/2026), cada una con su prueba de rojo a verde y la SUITE completa en
      verde antes del build (`suites/rc2-final/`); el commit es el único que hará Santiago. Detalle
      en `OE4-Resultados.md` §8.3:
      - [x] DEF-SPIKE-01 · servicios de Unity apagados al compilar (`UnityServicesOff`), red y LocalLow en P; resto en DEF-RC2-01
      - [x] DEF-W5R-01 · «¿Quién juega?» en páginas de tres con ▲/▼
      - [x] DEF-GP1-01 · el bloque de «Tu secuencia» ya no se queda pegado al cursor
      - [x] DEF-GP1-02 · la lista sigue al bloque en curso
      - [x] DEF-GP1-03 · la papelera sigue disponible con el cajón cerrado
      - [x] DEF-SPER-01 · «Cerrar el juego» sin papelera y en una línea
      - [x] DEF-SPER-02 · aviso del perfil ilegible y guardado atómico
      - [x] DEF-W5R-02 · deslizantes atenuados al soplar
      - [x] OBS-SDOC-2 · el informe nombra el perfil que se mira
- [ ] **T23 · `rc2` y regresión** — `M` · SUITE + EXE · depende de: T22
      Nueva etiqueta y build (T01–T02 abreviados), la suite completa, y cada caso fallido junto con
      **toda su sesión**.
      *Acepta:* los veredictos de `rc2` quedan en una columna nueva; los de `rc1` no se tocan.
      *(01/10/2026, sigue abierta) rc2 compilado sin etiqueta (`evidencias/build-rc2.md`), con la
      SUITE completa en verde y su columna de veredictos en `OE4-Resultados.md` §8.4, sin tocar los
      de rc1. Se repitieron enteras S-N2C y la medida de red y residuos, más GP3; de S-INI, S-N1 y
      S-PER solo los pasos de los casos fallidos.*

### Checkpoint E — Después de corregir
- [ ] Ningún F Bloqueante ni Mayor abierto sin decisión de Santiago
- [x] La suite completa sobre la última versión, sin fallos nuevos
      *(01/10/2026) Sobre rc2: PlayMode 376/376 y EditMode 470 = 469 + 1 omitida por entorno
      (`evidencias/suites/rc2-final/`). Después del build solo cambiaron una prueba y el comentario
      de un script de Editor, y sus pruebas pasan (`suites/rc2-posterior/`).*
- [ ] Revisado con Santiago

---

## Fase 5 — Consolidación

- [ ] **T24 · KPI** — `XS` · INSP · depende de: E
      Eficacia de la primera pasada y de la final, global y por etiqueta; KPI-2 sobre los 45 RF de
      prioridad Alta. Se deja la fórmula a la vista.
- [ ] **T25 · Matriz de trazabilidad** — `S` · INSP · depende de: E
      Una fila por requerimiento (47 RF + 23 RNF): casos, veredictos, pruebas automáticas que lo
      nombran (con cuántas pasaron) y `DEF`/INC.
- [ ] **T26 · Registro de correcciones e INC propuestos** — `S` · INSP · depende de: E
      Los `DEF` con su corrección y su commit. Para las desviaciones documentales, INC-55… en
      `claudeDocs/INCONSISTENCIAS.md` (**propuestos**, sin tocar ningún `.docx`). Candidatas:
      pasos de omisión (HU-02 FA-01), indicadores al repetir (HU-14 FA-01), cruceta (RF-35),
      inventario (RF-38), acumulado del N3 (RF-45), definición de corregidos del laberinto (OE1
      §3.6.1), confirmación al salir (HU-18).
- [ ] **T27 · Oportunidades de mejora y trabajos futuros** — `S` · INSP · depende de: T26
      Salen de: los `DEF` no corregidos, los PD, las observaciones de las sesiones, OE3 §10.3–10.4,
      los puntos PS-01..05 de la dirección de sonido, y PG-05/PG-06 (evaluación con estudiantes).
- [ ] **T28 · Revisión final e insumos del informe** — `XS` · HUM · depende de: T24–T27

### Checkpoint F — OE4 evaluado
- [ ] Los cinco entregables del cronograma tienen dónde vivir (`plan.md` §1)
- [ ] KPI-1 y KPI-2 finales publicados, con las dos pasadas
- [ ] Revisado con Santiago. **Fin del OE4 en su parte de pruebas.**
