# Defectos de la inspección rc3 (S-INSP)

Versión: rc3 (HEAD `f49ccc9`, contenido del árbol de trabajo sin cambios de código desde `23d7e8c`). Fecha: 10/10/2026.

### DEF-RC3-I01 · Identificadores en español en tres controladores del Nivel 2
- Caso: PF-RNF15-01 · Versión: oe4-rc3 · Severidad: Menor · RF/RNF: RNF-15 (nomenclatura uniforme; identificadores en inglés, `SPEC.md` §Estilo)
- Esperado: las 10 clases de la muestra usan identificadores en inglés.
- Obtenido: `ForestSceneController` usa `Apartar`, `Despejar`, `EnPantalla`, `SueloUnitario` y los locales `objeto`, `suelo`, `caja`, `estorbo`, `haciaElCentro`, `candidato(s)`, `destino`, `sitio`, `esquinas`, `indice`, `margen`, `escala`, `vuelta(s)`, `zona` (20 identificadores); `WorkshopSceneController`, `EnPantalla` y `esquinas`; `MazeSceneController`, `desborda`. Las otras siete clases cumplen, con `<summary>` en las diez.
- Evidencia: `rnf15-identificadores-es.txt`, `rnf15-nomenclatura.txt`, `scripts/rnf15.py`. Sin efecto en el juego: renombrar a mano (p. ej. `EnPantalla` → `ToScreenRect`).
- Tipo propuesto: estilo de código. No bloquea nada.

### DEF-RC3-I02 · Commits de código sin tarjeta del Kanban
- Caso: PF-RNF17-01 · Versión: oe4-rc3 · Severidad: Menor · RF/RNF: RNF-17, CT-11
- Esperado: el 100 % de los commits que tocan `Assets/` lleva tarjeta (`Dnn-n`, `Onn-n`).
- Obtenido: de 122 commits sin merges, 9 con tarjeta (7,4 %), 4 con el marcador sin resolver `(tarjeta: <id>)` (3,3 %) y 109 sin tarjeta (89,3 %); de estos, 82 citan INC, R, W o T, y 27 nada. Desde el 02/09, 9 de 120. Riesgo conocido en el catálogo.
- Evidencia: `rnf17-commits.txt`. No se puede corregir sin reescribir la historia; se documenta como limitación.
- Tipo propuesto: proceso.

### DEF-RC3-I03 · Halo de croma intenso en 9 PNG de caras de la familia
- Caso: INSP-ART-SPRITES (`arte_check.py sprites`; no es un caso PF) · Versión: oe4-rc3 · Severidad: Menor (cosmético) · RF/RNF: Dirección de arte §17, RNF-23
- Obtenido: 9 archivos con 15 a 51 píxeles verdes o magenta de α entre 10 y 25 sobre 255 (`char_nino_cara_base`, `char_nino_perfil_cara_base`, `char_mama_cara_base`, `char_mama_boca_a`, `char_mama_perfil_cara_base`, `char_nina_perfil_cara_base`, `char_nina_boca_a`, `char_nina_cara_base`, `char_mama_boca_preocupacion`). rc2 tenía 0 (las caras llegaron el 05–10/10). Casi invisible a 1080p.
- Evidencia: `arte-sprites.md`. Corrección posible: `arte_check.py halo --apply` con copia de respaldo, a decisión de Santiago.
- Nota aparte, sin defecto nuevo: 12 `.meta` en `Multiple` (los mismos de rc2).

### DEF-RC3-I04 · La suite no corrió sobre el contenido exacto de rc3
- Caso: SUITE-T05 · Versión: oe4-rc3 · Severidad: Menor (observación de trazabilidad) · RF/RNF: CT-10, RNF-13
- La corrida de 1.167 pruebas se hizo sobre HEAD `292b108` + árbol `b2a4d015fe44` y se registró en `23d7e8c`. Hasta HEAD `399e149` no cambió código ni pruebas de `Assets/`, pero sí `Level2_Maze.unity` (78 líneas: `RectTransform` con valores de layout en lugar de ceros, reserialización del Editor, en `f49ccc9`) y se borraron 6 PNG de cara de Algoritm sin referencias (`8602c79`; GUID con 0 usos).
- Sugerencia: corrida dirigida de `Maze` en PlayMode (`MazeScene`, `Maze_`) sobre rc3 antes de cerrar T05; o una suite completa si Santiago la quiere sobre HEAD.
- `RiverLevel_RNF21_…` falló una vez por salto de cuadro 0,056 frente a 0,05 con dos Editores compitiendo por CPU y pasa sola (1 de 1); ya figura como falla conocida de entorno en T05, no se abre DEF aparte.
