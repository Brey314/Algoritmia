# Sesión con estudiantes del 13/10/2026: correcciones al juego y cierre del OE4

## Contexto
El 13/10/2026 se hizo la sesión con 4 estudiantes de grado quinto en el Colegio El Libertador IED, sede A. Un observador miró las partidas y los estudiantes pensaron en voz alta. Salieron 10 hallazgos sobre el juego y los resultados humanos de las Tablas 5.1 y 5.2 del OE4.

El objetivo de esta tanda:
1. Corregir el juego.
2. Compilar rc4 y medir su peso, sin etiqueta.
3. Llevar los resultados al entregable OE4, a la tesis (cap. 9) y al guion radicado (solo el vocabulario, con su fila de control de cambios).
4. Pasar todo de «grado cuarto» a «grado quinto» y quitar la tercera persona de Santiago.

Decisiones ya tomadas por el usuario:
- H13 no se hizo y pasa a trabajo futuro.
- El guion se edita, con su fila en el control.
- Se compila rc4.
- El formato de consentimiento lo entrega el usuario más adelante. Ese anexo queda `[Pendiente]`; ahora solo se deja constancia.

Hecho comprobado:
- La autorización del escritorio (`D:\User\Desktop\Autorización Colegio.jpeg`) la firma el **rector Richard Maldonado**, con fecha 17/09/2026, para la investigación doctoral de la docente **Bibiana Patricia Rey Barrote**. Abarca a estudiantes de grado quinto y exige el consentimiento de los padres.
- `firmas.pdf` tiene 2 páginas escaneadas.
- Los documentos lo dirán así: la autorización la firma el rector y el contacto es la docente Rey.

## Agentes
- `programador` y `documentador` existen en `~/.claude/agents/` y ya cargaron.
- `documentador` tiene `effort: medium`. Se lanza con `effort: "high"` porque así lo pediste.

Reglas comunes que van en cada prompt:
- No hacer commit ni push. No escribir `.meta`.
- El informe final es conciso: archivos, decisiones y riesgos.
- PlayMode no lo corren los implementadores (memoria `playmode-solo-en-revision`).

## Números INC reservados
| INC | Tema |
|---|---|
| INC-151 | Grado quinto |
| INC-152 | Oscuridad del N1 |
| INC-153 | Caminata de la escena 1.1 |
| INC-154 | Abrazo y sueño de perfil |
| INC-155 | Troncos del bosque |
| INC-156 | Flechas curvas de «Girar» |
| INC-157 | Vocabulario |
| INC-158 | Modal del botón de ayuda |
| INC-159 | Pausa con cinco botones; reemplaza INC-49 |
| INC-160 | Botón «Atrás» del diálogo |

Las pruebas nuevas se nombran `<Sujeto>_INC15x_<QuéHace>`, o con el RF que tracen.

## Ola 1: código (cuatro `programador` en paralelo, con archivos disjuntos)
Regla del Editor: los `.unity` y `.prefab` se editan por YAML con la skill `edit-scene`. **Ningún agente abre escenas con coplay.**

Del Editor solo se usa:
- Import y refresh de assets.
- `execute_script` de `BuildRigsFinal`, solo P2.
- `editor.ps1 tests-edit` y `recompile`. El mutex serializa; si sale 3, se espera y se reintenta.

### P1 · Narrativa: textos, luz, caminata y botón «Atrás»
**Toca:**
- `Assets/Game/Data/Narrative/*.asset`, **excepto `N1_NacimientoDelFuego.asset`**.
- Los textos visibles de `Data/River/*`, `Data/Wheel/N2_ResumenNivel.asset` y `Reporting` (no).
- `DialogueRunner.cs`, `NarrativeSceneController.cs` y `Narrative.unity`.
- Sus pruebas.

**No toca:**
- `N2_WheelLevelConfig.asset`, `N2_MazeLayout.asset`, `Level1_Cave.unity` ni `MenuPausa.prefab`.

**Vocabulario (INC-157).** Reemplazos base:

| Original | Reemplazo |
|---|---|
| civilización | otras familias |
| abruptamente | de repente |
| intemperie | al aire libre, sin techo |
| maleza | matorrales |
| estela | rastro |
| silueta | sombra |
| estabilidad | rephrase «para que no se voltee» |
| computacionalmente | pensar como un algoritmo |
| irregulares | torcidos o de formas distintas |
| inventario | tus materiales |

Además, barrer todos los `Text:` visibles y proponer otras palabras difíciles para niños de 10 años.
- **Se conservan los términos del pensamiento computacional:** abstraer, iterar, descomponer. Las pruebas `LevelSummaryComposerTests:93,121-129` los exigen. Cada aparición debe llevar al lado una explicación llana; por ejemplo, «abstraer: quedarse solo con lo que importa».
- Entrega: la tabla aplicada y un `pares` JSON para el guion (`claudeDocs/entregables/tools/pares/guion-inc157.json`), en el formato de `reemplazar_parrafos.ps1`.

**Oscuridad (INC-152).**
- En `N1_Apertura`, `N1_AparicionGuia` y `N1_Hallazgo`, a cada `Ambient` de `LightStart` y de las paradas se le suma 0,12, con un mínimo de 0,15.
- Excepción: el negro intencional de `N1_Apertura` línea 6 (ambient 0, HardCut).

**Caminata 1.4.1 (INC-153).**
- En `N1_AparicionGuia`, línea 7, los beats Walk de Papá, Mamá, Niña y Niño tienen destinos en x ≈ 0,18–0,22, conservando orden y separación, y llevan `Facing: 1` (izquierda).
- **Guarda de causa raíz:** una prueba de contenido sobre las 18 narrativas. Todo beat Walk/Run/Carry/Push con `Moves` debe desplazarse al menos `ActorTimeline.MinimumDisplacement`.
- Tiene que seguir pasando `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`.

**Botón «Atrás» (INC-160).**
- `DialogueRunner.Back()`: `_index--` con tope en 0; no toca `IsFinished`.
- `NarrativeSceneController.Back()`: re-renderiza la línea **sin transición**.
  - Texto, retrato y cara del hablante.
  - Cámara y luz **cortadas en seco** a la parada de esa línea, sin suavizado.
  - Actores colocados en el `To` de esa línea. Si la acción es locomoción, quedan en Idle; si no, hacen la acción.
  - No se repiten sonidos, `MotionAmbient` ni `PlayMotions`.
- Hace falta `_furthestRendered`: al volver a avanzar sobre líneas ya vistas, se renderizan quietas; solo las líneas nuevas se reproducen completas.
- `BotonAtras` va en `CuadroDialogo`, a la izquierda de `BotonContinuar`, con el mismo estilo. Está oculto en la línea 0 y no cruza secuencias encadenadas. Su etiqueta sale del SO donde ya viven las de Continuar y Omitir (CT-05).
- Pruebas EditMode de `DialogueRunner.Back`.

### P2 · Personajes: abrazo (1.4.4) y sueño (cierre del N1) de perfil (INC-154)
**Toca:**
- `ActionView.cs`: `Hug` y `Sleep` pasan a `Profile`; se actualiza el `<remarks>`.
- `CharacterViewTests.cs:80`.
- `coreografia.py` y `clips_personajes.json`.
- Los clips `char_*_anim_abrazar` y `char_*_anim_dormir` de la familia, regenerados con el modo `clips` de `BuildRigsFinal.cs.txt`. El orden y el procedimiento están en `claudeDocs/tasks/Personajes/`.
- `N1_NacimientoDelFuego.asset`, incluidas sus palabras: crepita → «chisporrotea» / «hace crac»; silueta → sombra. «iterar» se conserva, con explicación al lado.

**Qué debe lograr:**
- **Abrazo:** de perfil, los brazos cercano y lejano se cierran hacia adelante. En las líneas 5 y 12, los beats llevan `Facing` hacia Papá y casillas juntas.
- **Dormir:** el cuerpo de perfil está acostado, con `Lienzo/Perfil` o `Perfil/Tronco` a unos ±90° y el anclaje ajustado a la casilla. Respira y tiene los ojos cerrados. En la línea 16 los cuatro rodean el fuego sin taparlo.
- Algoritm sigue de frente (no tiene torso de perfil).
- Hay que localizar los demás usos de `Action 16/18` en las narrativas. Si alguno está en un asset de P1 y necesita retoque, **se informa, no se edita**.
- **Nunca reconstruir prefabs:** cambian los fileID.

**Verificar:**
- `pose_preview.py`.
- EditMode de `Game.Scaffolding`.
- Las capturas que dé `capture_ui_canvas` de esas líneas, si el Editor está libre.

### P3 · Nivel 2: troncos (INC-155) y flechas de giro (INC-156)
**Toca:**
- `N2_WheelLevelConfig.asset` (Nudges RoundLog).
- `ForestSceneController.cs` y `ForestObjectNudge.cs`.
- `Level2_Forest.unity`, `MazeSceneController.cs` y `Level2_Maze.unity`.
- `Assets/Game/Art/UI/Common/ui_girar_izq.png` y `ui_girar_der.png`.
- Sus pruebas.

**Troncos:**
- Ajuste de empuje: `Push` 0,75 → 0,35 y `Drag` 1,4 → 3,0.
- El tronco no se empuja mientras el cursor está sobre su área de clic. Es la causa probable de «difícil de coger».
- El área de clic de los RoundLog cubre el cuerpo que dibuja `RollingLog`, no solo el disco de 120×120. Se usa `raycastPadding` negativo, o se dimensiona desde la geometría de `RollingLog`.
- Lleva una prueba de la geometría.

**Flechas de giro:**
- Rasterizar Phosphor `arrow-arc-left` y `arrow-arc-right` (MIT, `@phosphor-icons/core` fijado por versión) a 128 px en blanco, con el mismo proceso de `Art/Inventario.md:363-378`. Se importan como sprite `Single`.
- Sustituyen al DropdownArrow girado ±90° en `Fondo/Lado/Izq|Der/Icono` y en todo lugar donde se muestre el lado del giro (`Mini_Girar`, ventana de secuencia).
- Se evita `ui_reiniciar` para no confundirlo con «Reiniciar».

### P4 · Interfaz: pausa (INC-159) y modal de ayuda (INC-158)
**Toca:**
- `PauseMenuController.cs`, `MenuPausa.prefab` y `PauseMenuPolicy.cs` (si hace falta).
- `PauseMenuTests.cs`.
- Un componente nuevo en `Game.Scaffolding` y `FirePanelController.cs`.
- El SO de contenido del N1 y `Level1_Cave.unity`. Incluye la oscuridad de la cueva: `darkest.Ambient` 0,10 → 0,22 y `brightest` 0,30 → 0,35, en la escena y en los valores por defecto de `CaveLightingController.cs`.

**Pausa: cinco botones en este orden:**
1. Reanudar.
2. Reiniciar.
3. **Volver a ver la historia.**
4. Volver al menú de niveles.
5. **Guardar y salir.**

Cada uno con el estilo de los actuales: `ui_boton`, 480×94, Baloo2-Bold 34.

- **Volver a ver la historia:**
  - En el prefab, una tabla serializada `PhaseNarrative { LevelId level; int phase; string sequenceId; }` con la narrativa cuya `NextPhase` lleva a esa fase: N1f1 `N1_Hallazgo`, N2f1 `N2_Escena21_Bosque`, N2f2 `N2_Escena23_Construccion`, N2f3 `N2_Escena24_Regreso`, N3f1 `N3_Escena31_Llegada`, N3f3 `N3_Escena32_PrimerIntento`.
  - Sin entrada (N3f2), el botón se oculta.
  - Al pulsarlo: `ClosePause()` y luego `Runner.StartNarrative(id)`. Al terminar, `Leave()` vuelve a la fase.
  - Lleva una prueba EditMode que cruza la tabla con los assets reales (Level y NextPhase).
- **Guardar y salir:**
  - Abre `PanelConfirmarSalida`, una copia del patrón `PanelConfirmarReinicio`, con el texto «¿Quieres salir del juego? Lo que ya terminaste queda guardado.», «Sí, salir» en ámbar y «Cancelar».
  - Al confirmar: `ProfileSession.SaveActive()` y luego `Quit` inyectable, como `MainMenuController.Exit`.
  - Los textos van en un SO (CT-05).
- `PauseMenuTests` pasa de 3 a 5 botones.

**Modal de ayuda:** componente `HelpButtonIntro` en `Game.Scaffolding`.
- Fondo oscuro (`a=0.72`, como la pausa) que bloquea los raycasts.
- El botón de ayuda queda encima con un `Canvas` `overrideSorting` mientras el modal está abierto. Se quita al cerrar.
- Lleva una tarjeta `ui_panel` marfil con el texto en Nunito 34 («Este es el botón de ayuda. Si no recuerdas qué hacer, haz clic en él y Algoritm te repetirá la instrucción.») y un botón «Cerrar» con `ui_boton`.
- Mientras está abierto, `Time.timeScale = 0` (se restaura al cerrar).
- **Cuándo aparece:** lo dispara `FirePanelController` al cargar `Level1_Cave` si el perfil activo no tiene confirmada la fase N1f1. Se deriva del progreso; no se persiste nada nuevo (RNF-09).
- Los textos van en el SO del N1.
- Lleva una prueba EditMode de la condición de aparición.

## Revisión (yo)
1. `git diff --stat` y los hunks críticos:
   - Firmas públicas y contratos de arriba.
   - fileID en `.unity`/`.prefab`: sin reconstruir prefabs de personaje.
   - CP-02: ningún mensaje de fallo nuevo.
   - Grep de `Destroy`, `SaveAsPrefabAsset`, `ClearCurves` y `DeleteAsset`.
2. Un `programador` corre PlayMode solo de lo afectado: `PauseMenu`, `Narrative`, `Forest`, `Maze`, `Fire`/`Cave` y `Character`. Las correcciones las hace un agente por error, con el criterio exacto.
3. Verificación final del mismo agente:
   - Suite completa con `claudeDocs/tasks/OE4/herramientas/suite2.ps1`.
   - Build rc4 con `editor.ps1 exec build`, moviéndolo a `Build/Algoritmia/`.
   - Peso con `oe4.ps1 Tamano` (RNF-06 < 500 MB).
   - `claudeDocs/tasks/OE4/evidencias/build-rc4.md` con commit y SHA-256.
   - El ruido de `ProjectSettings.asset` se restaura con `git checkout`.
4. Commit local de código y assets a nombre de Santiago, sin Co-Authored-By (memoria), citando INC-152 a INC-160.

## Ola 2: documentación (`documentador`, effort high, en secuencia; Word COM no se comparte)
Todos siguen `claudeDocs/entregables/Guia-redaccion-sin-marcas-de-IA.md` con estas reglas:
- Impersonal en pretérito perfecto simple, sin «Santiago decidió/planeó».
- «Grado quinto» en lugar de «grado cuarto». «Cuarto objetivo» y «cuarto golpe» no cambian.
- No afirmar nada que no esté medido.

**D1 · Markdown y OE4:**
- `claudeDocs/entregables/OE4/src/05-estudiantes.md`, Tabla 5.1:
  - Visto bueno: rector, 17/09/2026.
  - Consentimientos: 4 estudiantes, constancia. El formato anexo queda `[Pendiente: formato que entrega el equipo]`.
  - Fecha y lugar: 13/10/2026, sede A.
  - Método: un observador y pensamiento en voz alta.
  - PG-05 y PG-06: positivos.
  - Borrado: positivo; perfiles borrados y `Datos\` vacía.
- Tabla 5.2:
  - H1 a H4: aprobados.
  - H5: aprobado. Equipos AMD A10-9600P (Radeon R5, 4 GB, sin red) e Intel Celeron B830 (HD Graphics, 4 GB, sin red). La sala tenía red de 20/7 Mbps disponible y no se usó.
  - H6: aprobado, el sonido se escucha.
  - H7: aprobado.
  - H8: 40–45 min, frente a la meta de 20–40. Se reporta que supera el tope en hasta 5 min, sin inventar la causa.
  - H9, H10 y H12: aprobados.
  - H11: aprobado, con el hundimiento comprendido, la corrección y el cruce.
  - H13: no realizado, va a trabajo futuro.
- `03-resultados.md:51`.
- Una sección de hallazgos de la sesión, con las correcciones aplicadas y rc4.
- La tercera persona en `02-metodo`, `04-mejoras` y `06-conclusiones`.
- `INCONSISTENCIAS.md`: INC-151 a INC-160 (rev. 25). INC-151 anota que OE1 y OE2 radicados siguen diciendo «cuarto» y piden autorización.
- `OE4-Resultados.md`, `todo.md` y `Hoja-HUM.md`: notas fechadas.
- `Interfaces.md`, `Art/Inventario.md` (flechas) y la línea de grado en `CLAUDE.md`.
- `python claudeDocs/entregables/OE4/tools/build.py --publish`.

**D2 · Word:**
- Tesis `docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx`:
  - Cap. 9: los `[Pendiente]` de las líneas 73 y 113-166 de `cap9-evaluacion.md`. El de la Familia Anonaky no se toca.
  - «Grado cuarto» → «grado quinto», incluido el título si lo contiene.
  - La tercera persona.
  - Herramientas: `reemplazar_parrafos.ps1` con `-DryRun` primero, más `verificar_reemplazos.py` y `medir_paginas.ps1` (≤ 90 páginas).
- Guion `Solucion_OE2_Diseno_final.docx`: pares `guion-inc157.json` y una fila en el control (§5) con `agregar_fila_tabla.ps1`. Se regenera `docs/md/` con markitdown.

Al final, commit local de docs, `/graphify . --update` y **push solo tras mi revisión**.

## Reparto con la sesión en la nube (otra cuenta, `session_01HeFbet8ozsHn9inuZ954Rf`)
**Rama `ajustes-OE4`.** Git no admite espacios en el nombre. Se crea desde `feat/personajes-animados` (`4dfcf35`) y se publica en `origin`. La modificación local sin commit de `docs/OE4/Solucion_OE4_Evaluacion_prototipo.docx` no se toca.

**Instrucción a la nube:**
- Todo commit y push sale a nombre de `santivaldiri` (`git config user.name/email` de esa cuenta), sin `Co-Authored-By` de Claude.
- Antes de cada push, `git pull --rebase`.
- Los commits son pequeños y citan su INC.
- Los commits locales siguen a nombre de Santiago (memoria).

**Comunicación:**
- Yo envío a la nube con `SendMessage` (nombre exacto con `ListAgents`). La nube no responde por mensaje, así que su avance se lee en los commits de `origin/ajustes-OE4` y en el archivo `claudeDocs/tasks/OE4/Nube-estado.md`, que actualiza en cada push.

**La nube no tiene Unity ni Word.** Por eso recibe solo lo que sale con YAML, C# compilado con `mcs` y stubs en su scratchpad, o markdown:
- **C1 · Vocabulario (INC-157):** todo el bloque de vocabulario de P1, en **todas** las narrativas, incluidas las líneas de texto de `N1_NacimientoDelFuego.asset`, y los SO de `River/` y `Wheel/`. Solo líneas `Text:` y otros campos de texto. También los pares `guion-inc157.json`.
- **C2 · Oscuridad (INC-152) y caminata (INC-153):** los valores de luz de los tres `N1_*.asset` y los beats de la línea 7 de `N1_AparicionGuia`. Además, la prueba de contenido de desplazamiento mínimo, compilada con `mcs`. La comprobación visual y la de RNF-03 se hacen en local, en la revisión.
- **C3 · Resultados en markdown, independientes del código:**
  - Tablas 5.1 y 5.2, `03-resultados.md:51`, la tercera persona y «quinto» en `claudeDocs/entregables/OE4/src/*.md` y en `claudeDocs/entregables/TG/cap9-evaluacion.md`.
  - INC-151 en `INCONSISTENCIAS.md`.
  - Notas fechadas en `OE4-Resultados.md`, `todo.md` y `Hoja-HUM.md`.
  - **No** escribe la sección de hallazgos y correcciones; esa espera al código.
- **C4, cuando se haga el push del código local:** la sección de hallazgos y correcciones del OE4 y INC-152 a INC-160 en `INCONSISTENCIAS.md`. También `Interfaces.md`, `Art/Inventario.md` y la línea de grado en `CLAUDE.md`.

**En local:**
- P1 se reduce al **botón «Atrás»**: `DialogueRunner`, `NarrativeSceneController` y `Narrative.unity`.
- P2, P3 y P4 quedan como están. P2 solo toca beats y posiciones en `N1_NacimientoDelFuego.asset`; los textos van en la nube.
- La revisión, la suite, rc4 y D2 (Word: la tesis, el guion y `build.py --publish` del OE4) se hacen en local y **después** de traer C1 a C4.

**Archivos que la nube no toca:**
- `*.cs` fuera de la prueba de C2.
- `*.unity`, `*.prefab` y `*.anim`.
- `ActionView`, `coreografia.py`, `PauseMenu*`, `Forest*`, `Maze*` y `Fire*`.
- Todo `.docx`.

## Continuidad de la sesión
- **Recordatorios:** después de aprobar, `CronCreate` a las 17:20 y luego cada 5 h, con el prompt «Continúa el plan `pasted-content-id-74b6…vectorized-pelican.md` desde la primera ola o paso sin cerrar».
- **Cupo:** las herramientas de Claude en el navegador (`mcp__claude-in-chrome__*`) **no están conectadas en esta sesión**, así que no puedo leer el cupo cada 15 min. Para el corte, el estado vive en este archivo, con una casilla por ola que actualizo al cerrar cada paso.

## Estado
- [ ] Rama `ajustes-OE4` creada y publicada; encargo C1–C3 enviado a la nube
- [ ] Ola 1 local (P1 Atrás, P2, P3, P4)
- [ ] Revisión y PlayMode afectado
- [ ] Suite completa, rc4 y medida
- [ ] Commit de código
- [ ] D1 OE4 y markdown
- [ ] D2 tesis y guion
- [ ] Commit de docs y push
