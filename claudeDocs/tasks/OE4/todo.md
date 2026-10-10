# Tablero — OE4: Evaluación funcional del prototipo (pasada rc3)

Plan: [`plan.md`](plan.md) · Casos y guiones: [`casos.md`](casos.md) (136 casos) · Resultados:
[`OE4-Resultados.md`](OE4-Resultados.md) · Contrato: `claudeDocs/SPEC.md`.

> **Reiniciado el 10/10/2026.** La pasada vigente es sobre **rc3**, compilado desde el estado actual de la
> rama. El tablero de la pasada rc1/rc2 (01/10/2026), con todas sus casillas, queda en el historial de git, en el
> commit `4e78f47`; ninguna de sus casillas marcadas se hereda, porque el juego cambió en 108 commits. Los ids
> `T01`–`T28` se conservan para que los documentos que los citan sigan resolviendo.

**Leyenda:** tamaño `XS`–`L` · ejecutor `EXE` (Claude con el arnés sobre el ejecutable), `INSP`
(inspección de archivos), `SUITE` (pruebas automáticas), `EDIT` (Editor abierto por `coplay-mcp`), `HUM`
(Santiago). Una sesión EXE se da por **hecha** cuando cada caso que toca tiene veredicto en
`OE4-Resultados.md`, con evidencia y, si fue F, su `DEF-nn`. **Hecha no significa aprobada.**

> ⚠️ **Nada de esto empieza sin T01.** Probar algo que no está congelado deja veredictos que no
> se pueden atribuir a ninguna versión.

> ⚠️ **Editor cerrado** durante SUITE con `unity test` y durante toda sesión EXE que mida carga o memoria.
> Dos carriles no corren pruebas a la vez (CLAUDE.md §Comandos; `NORMA-PRUEBAS.md`): avisar al otro carril antes.

---

## Fase 0 — Preparación

- [ ] **T01 · Congelar la versión candidata `rc3`** — `XS` · HUM + git
      `plan.md` §2, §12 · depende de: —
      Se compila desde el árbol de trabajo de la rama, **sin etiqueta**; Claude pone la etiqueta
      **solo con el visto bueno de Santiago** (es una acción hacia fuera si se sube).
      *Acepta:* la procedencia (HEAD, huella de `git diff HEAD`, lista de archivos sin seguimiento) queda
      escrita en `evidencias/build-rc3.md`; la decisión sobre la etiqueta, escrita aquí.
- [ ] **T02 · Compilar `rc3` y registrar su procedencia** — `S` · SUITE
      RNF-06, CT-03 · depende de: T01
      Con el Editor cerrado: `unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe"
      --log-file build.log --no-banner --non-interactive` (o `editor.ps1 exec build` con el Editor abierto,
      moviendo la salida a `Build/Algoritmia/`). No borrar el build anterior: renombrarlo.
      *Acepta:* 12 escenas con `Boot` primera; `build.log` sin errores; `Licencias/` junto al `.exe`.
      *Verifica:* `evidencias/build-rc3.md` con SHA-256 del exe, tamaño bruto y huella del contenido.
      ⚠️ Tras el build, restaurar con `git checkout` el ruido de `ProjectSettings.asset` (defines de
      Standalone y `preloadedAssets`) antes de medir la huella del árbol.
- [ ] **T03 · Arnés `herramientas/oe4.ps1`: comprobar que sigue sirviendo con rc3** — `S` · EXE
      `plan.md` §5 · depende de: T02
      El arnés ya existe; el spike de rc1 pasó. Hay que repetirlo sobre rc3 (ventana, menú, «Créditos» y
      vuelta, arrastre en la cueva, Mamá con clic sostenido, `Medir-Carga`) porque las pantallas cambiaron
      (portada, tarjeta del diálogo, personajes de perfil).
      *Acepta:* el spike pasa o se documenta qué coordenada o espera cambió.
- [ ] **T04 · Semillas, contraste del catálogo y esqueleto de resultados** — `S` · INSP
      `plan.md` §7, §3.1 punto 5, §8 · depende de: T02
      Las semillas de `herramientas/perfiles/` se validan otra vez contra `PlayerProfile.cs`. Contrastar
      **cada** texto entre «» de `casos.md` con los assets de rc3 (grep) y corregir el catálogo si algo cambió,
      incluidos los nueve casos que entraron el 10/10/2026 (`PF-RF01-03`, `PF-RF05-04..06`, `PF-RF08-02`,
      `PF-RF10-04`, `PF-RF20-04`, `PF-RF30-02`, `PF-RF35-02`). `OE4-Resultados.md` ya tiene el esqueleto.
      *Acepta:* cada semilla carga en el juego y cero textos del catálogo difieren de rc3.
      *Verifica:* `Preparar-Datos OE4_Z` → «Jugar» → «OE4_Z» → los tres niveles habilitados.

### Checkpoint A — Listos para probar
- [ ] `rc3` congelado y compilado; SHA-256 en la cabecera de resultados
- [ ] El arnés pasó el spike sobre rc3, **o** Santiago eligió el plan B (HUM o EDIT) y quedó escrito
- [ ] Las semillas cargan; el catálogo coincide con rc3
- [ ] Revisado con Santiago

---

## Fase 1 — Línea base

- [ ] **T05 · Suite completa sobre `rc3` y matriz automática** — `M` · SUITE
      CT-10 · depende de: T01
      `herramientas/suite2.ps1` (según `NORMA-PRUEBAS.md`: dos Editores en paralelo, ~21 min) o
      `unity test` en EditMode y PlayMode, con salida en `evidencias/suites/rc3/`. Pruebas con falla
      conocida por entorno: `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`
      (omitida por entorno), `RiverLevel_RNF05_*` (mide el Editor, no el juego) y
      `RiverLevel_RNF21_*` (sensible al tiempo; se reintenta sola).
      Grep de `_RFnn_` / `_RNFnn_` en los nombres de método de `Assets/Tests` → cuántas pruebas nombran
      cada requerimiento y cuántas pasaron (`claudeDocs/entregables/OE3/tools/rf_matrix.py --worktree`).
      *Acepta:* resultado de las dos corridas en resultados; toda falla nueva, con un `DEF`.
- [ ] **T06 · Inspecciones estáticas** — `M` · INSP (puede ir a un subagente)
      PF-RNF01-01, PF-RNF15-01, PF-RNF17-01, PF-RNF22-01 (textos), PF-RNF23-01 (créditos);
      CT-10 del catálogo · depende de: T01
      *Acepta:* los cinco casos con veredicto; el grep de `PF-RFnn` y `PF-RNFnn` en `casos.md`
      cubre RF-01..47 y RNF-01..23 sin huecos (hoy 136 casos).
      *Verifica:* los scripts de un solo uso quedan en el scratchpad y su salida, en evidencias.

---

## Fase 2 — Recorridos sobre el ejecutable

Cada tarea es una sesión de `casos.md`. Al cerrarla: veredictos, evidencias, JSON final copiado y
`Revisar-Log`.

- [ ] **T07 · S-INI · Inicio, perfiles, créditos y salida** — `S` · EXE · depende de: A
      *Acepta:* 14 casos con veredicto (los once de antes, PF-RF02-06 y los nuevos PF-RF01-03 y PF-RF08-02:
      portada con los cinco personajes y Algoritm saludando en los créditos).
- [ ] **T08 · S-N1 · Nivel 1 completo con perfil nuevo** — `M` · EXE · depende de: A
      *Acepta:* los 25 casos de la sesión con veredicto, incluidos retrato animado, personajes de perfil,
      sombras y halo de la fogata; las cifras del JSON contrastadas con I 5 · C 1 · P 3 y el tiempo menos 30 s.
- [ ] **T09 · S-N2A · Bosque** — `S` · EXE · depende de: A
      *Acepta:* 10 casos con veredicto; N2F1 = I 3 · C 1 · P 0.
- [ ] **T10 · S-N2B · Taller** — `S` · EXE · depende de: T09 (o semilla `OE4_B2`)
      *Acepta:* 4 casos con veredicto; N2F2 = I 5 · C 3 · P 6.
- [ ] **T11 · S-N2C · Laberinto y cierre del N2** — `M` · EXE · depende de: T10 (o `OE4_B3`)
      *Acepta:* 10 casos con veredicto (incluido PF-RF30-02: sin tinte, ámbar `#C4A882`); la rejilla
      transcrita y el conteo de ediciones, en el registro de la sesión; N2F3 igual a lo previsto.
      ⚠️ PF-RF34-02 puede dejar el bloque pegado: recuperar y seguir; no investigar el código en esta tarea.
- [ ] **T12 · S-N3 · Nivel 3 y cierre del juego** — `M` · EXE · depende de: A
      *Acepta:* 20 casos con veredicto (incluido PF-RF35-02, Mamá de perfil); el amarre y el segundo fallo
      jugados; las cifras previstas son N3F1 2/1/1 · N3F2 5/2/2 · N3F3 7/4/3.

### Checkpoint B — Golden Path negro completo
- [ ] Los cuatro niveles del recorrido (N1, N2 × 3, N3) jugados de punta a punta sobre rc3, dos veces
      (ventana y pantalla completa), sin bloqueos ni estados irrecuperables
- [ ] Ningún `DEF` Bloqueante sin triar: si lo hay, se lleva a Santiago **antes** de seguir
- [ ] Eficacia parcial calculada y anotada
- [ ] Revisado con Santiago

- [ ] **T13 · S-PAU · Pausa y reinicio en las cinco escenas** — `M` · EXE · depende de: B
      *Acepta:* PF-RF07-01..04 con veredicto en las seis filas de la tabla de la sesión.
- [ ] **T14 · S-OMI · Omisión, rejuego y negativos del N1** — `M` · EXE · depende de: B
      *Acepta:* PF-RF06-02..04, PF-RF20-02, PF-RF20-03, PF-RF21-02, PF-RF45-04 y PF-RF45-05 con veredicto.
- [ ] **T15 · S-PER · Persistencia, cierre forzado y robustez** — `M` · EXE · depende de: B
      *Acepta:* PF-RNF14-01..05, PF-RF02-04 y PF-RF02-05 con veredicto.
- [ ] **T16 · S-DOC · Informe docente, borrado y residuos** — `M` · EXE + INSP
      depende de: T08, T11, T12 (usa sus JSON finales)
      *Acepta:* PF-RF46-01..03, PF-RF47-01..03, PF-RF09-04, PF-RNF11-01 y PF-RNF09-01 con veredicto; la
      carpeta `Datos/` restaurada al final.
- [ ] **T17 · S-INSP · Accesibilidad y RNF restantes** — `M` · INSP + EDIT + SUITE
      PF-RNF02-01, 03-01, 16-01, 18-01, 19-01, 20-01, 21-01 · depende de: T08–T12 (capturas)
      *Acepta:* los siete casos con veredicto. RNF-18 queda **revertido**, con `git status` limpio. El
      worktree de RNF-16 queda **eliminado**. RNF-21 incluye el halo de las fogatas (`FireGlow`).

### Checkpoint C — Primera pasada completa
- [ ] Todo caso EXE, INSP, SUITE y EDIT con veredicto; cero B sin justificar
- [ ] Eficacia de la pasada calculada (global y por etiqueta) y KPI-2 preliminar
- [ ] Lista de `DEF` con severidad y tipo propuesto
- [ ] Revisado con Santiago

---

## Fase 3 — Personas y otros equipos

- [ ] **T18 · Hoja de pruebas humanas** — `S` · INSP · depende de: T04
      Para Santiago: Golden Path ×2 con cronómetro, hojas de sonido por nivel (solo las piezas
      **cableadas** según `Direccion_de_Musica_y_Sonido.md` §19), equipo 2, equipo sin GPU,
      modo avión, y los documentos de RNF-12, RNF-22 y RNF-23.
      *Acepta:* `Hoja-HUM.md` al día con rc3, con una casilla y un campo de nota por comprobación y cada
      fila con su caso `PF-*`; faltan las confirmaciones documentales de PF-RNF22-01 y PF-RNF23-01.
- [ ] **T19 · Ejecución HUM (Santiago)** — `M` · HUM (+ Claude en PF-RNF08-01)
      PF-RNF13-01, PF-SON-01..03, PF-RNF07-02, PF-CT02-01, PF-RNF12-01, PF-RNF22-01,
      PF-RNF23-01 · depende de: T18, A
      **S-HUM y la observación con estudiantes (PG-05, PG-06) las hace Santiago.** Claude transcribe lo que
      él reporte, sin completar nada, y cada caso tiene veredicto o queda B con motivo.
- [ ] **T20 · S-RNF · Medidas en el ejecutable (RNF-04, RNF-05, RNF-06)** — `S` · INSP + EXE · depende de: T07–T17, T19
      PF-RNF04-01, 05-01, 06-01, 07-01, 08-01, 10-01
      *Acepta:* la tabla de cargas de las 12 escenas (3 mediciones en el equipo 1, más las del equipo 2), los
      picos de memoria, el tamaño de la carpeta (< 500 MB) y el análisis de red, con su veredicto. Se llena
      también la tabla `P12` de `claudeDocs/tasks/Slice 4/todo.md`. Faltan, además, RNF-04 y RNF-05 **sobre un
      ejecutable**: hasta hoy solo se midieron en el Editor.

### Checkpoint D — Pasada completa, incluida la humana
- [ ] Los 136 casos con veredicto
- [ ] KPI de la pasada **publicado** en resultados, aunque no llegue al 90 %
- [ ] Revisado con Santiago

---

## Fase 4 — Corrección y regresión

- [ ] **T21 · Triaje** — `S` · HUM + INSP · depende de: D
      Santiago decide cada `DEF`: corregir en OE4, registrar un INC o dejarlo como trabajo futuro.
      Los F de criterios de HU sin INC se deciden aquí.
      *Acepta:* cada `DEF` con decisión y fecha.
- [ ] **T22 · Correcciones** — `S` a `M` cada una · código (`fix-bug`) · depende de: T21
      Una subtarea por `DEF` aprobado, que se añade aquí al abrirse: prueba que falla, con el RF en
      su nombre → corrección → suite del módulo → mensaje de commit (sin hacer el commit).
- [ ] **T23 · `rc4` y regresión (solo si hubo correcciones)** — `M` · SUITE + EXE · depende de: T22
      Build nuevo (T01–T02 abreviados), la suite completa, y cada caso fallido junto con **toda su sesión**.
      *Acepta:* los veredictos del build corregido quedan en una columna nueva; los de rc3 no se tocan.

### Checkpoint E — Después de corregir
- [ ] Ningún F Bloqueante ni Mayor abierto sin decisión de Santiago
- [ ] La suite completa sobre la última versión, sin fallos nuevos
- [ ] Revisado con Santiago

---

## Fase 5 — Consolidación

- [ ] **T24 · KPI** — `XS` · INSP · depende de: E
      **KPI 1, eficacia ≥ 90 %:** `(P + PD) / (P + PD + F)`, global y por etiqueta, de la primera pasada y de la
      final. **KPI 2:** los 45 RF de prioridad Alta sin ningún F Bloqueante ni Mayor. Se deja la fórmula a la vista.
- [ ] **T25 · Matriz de trazabilidad CT-10** — `S` · INSP · depende de: E
      Una fila por requerimiento (47 RF + 23 RNF): casos, veredictos, pruebas automáticas que lo nombran
      (con cuántas pasaron) y `DEF`/INC.
- [ ] **T26 · Registro de correcciones e INC propuestos** — `S` · INSP · depende de: E
      Los `DEF` con su corrección y su commit. Para las desviaciones documentales, INC propuestos en
      `claudeDocs/INCONSISTENCIAS.md` (sin tocar ningún `.docx`).
- [ ] **T27 · Oportunidades de mejora y trabajos futuros** — `S` · INSP · depende de: T26
      Salen de: los `DEF` no corregidos, los P parcial, las observaciones de las sesiones, OE3 §10.3–10.4,
      los puntos PS-01..05 de la dirección de sonido, y PG-05/PG-06 (evaluación con estudiantes).
- [ ] **T28 · Revisión final e insumos del informe** — `XS` · HUM · depende de: T24–T27

### Entregables finales
- [ ] **Entregable docx del OE4** (resultados, matriz y KPI al estado de rc3, por Word COM como el del OE3)
- [ ] **Capítulo 9 de la tesis** (OE4: evaluación funcional) redactado con las cifras de este tablero

### Checkpoint F — OE4 evaluado
- [ ] Los entregables del cronograma tienen dónde vivir (`plan.md` §1)
- [ ] KPI-1 y KPI-2 finales publicados, con las dos pasadas
- [ ] **S-HUM y PG-05/PG-06 hechos por Santiago** y transcritos
- [ ] Revisado con Santiago. **Fin del OE4 en su parte de pruebas.**
