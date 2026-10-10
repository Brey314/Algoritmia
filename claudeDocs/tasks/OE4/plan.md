# Plan de pruebas funcionales — OE4

> **Nota del 10/10/2026.** La pasada vigente es sobre **rc3**, compilado desde el estado actual de la rama. rc1 y
> rc2 (01/10/2026) se ejecutaron con este mismo plan y sus resultados y su tablero viven en el commit `4e78f47`.
> Este plan no se reescribe: donde dice `oe4-rc1` o «rc1» léase el candidato vigente, y los resultados, el tablero
> y el catálogo (`casos.md`, hoy con 136 casos) se reiniciaron para rc3 (`OE4-Resultados.md`, `todo.md`).

> **Estado:** propuesto el 29/09/2026, pendiente de revisión de Santiago (ver §12).
> **Qué hay en esta carpeta:** este `plan.md` (estrategia, reglas y decisiones), `casos.md` (el
> catálogo de casos con su guion paso a paso: el *instrumento de evaluación funcional* del
> cronograma) y `todo.md` (el tablero). Los resultados se escriben en `OE4-Resultados.md`, que se
> crea en la tarea T04; las evidencias van en `evidencias/` y el arnés en `herramientas/`.
> Como los demás `plan.md` del proyecto, este no se reescribe: lo que cambie al ejecutar se anota
> en `OE4-Resultados.md` o en `todo.md`.

## 1. Qué pide el OE4 y con qué vara se mide

**Objetivo (Trabajo de grado §2.2):** «Evaluar el prototipo del videojuego mediante pruebas
funcionales basadas en los requerimientos previamente definidos, con el propósito de comprobar su
funcionamiento e identificar oportunidades de mejora para trabajos futuros.»

**Los dos KPI (Trabajo de grado §2.3; `SPEC.md` §Criterios de éxito 7 y 8):**

| KPI | Meta | Cómo se calcula aquí |
|---|---|---|
| **Eficacia de pruebas funcionales**: «que los botones, sonidos y saltos de nivel funcionen» | **≥ 90 %** | `(P + PD) / (P + PD + F) × 100` sobre los casos `PF-*` de `casos.md` (veredictos en §3.2). Se publica **global y por etiqueta** (`NAV`, `BOT`, `SON`, `RETO`, `DAT`, `RNF`), porque el KPI nombra botones, sonidos y saltos de nivel. |
| **Grado de cumplimiento de requerimientos**: todos los RF de prioridad Alta implementados | **100 %** | Un RF Alta está implementado si **ninguno** de sus casos termina en F de severidad Bloqueante o Mayor (§6). Un F Menor (un rótulo, un criterio secundario de HU) resta en la eficacia, pero no niega que el RF exista. Son 45 RF Alta; RF-06 es Media y RF-21 Baja (OE1 §3). |

Los dos se reportan **dos veces**: en la primera pasada (`oe4-rc1`) y en la final, tras las
correcciones. El KPI se juzga sobre la final, pero la primera se publica igual: es la evidencia de
que el ciclo prueba → corrección → reprueba existió (Trabajo de grado §5.1.4, «retroalimentación
para la mejora iterativa»).

**Actividades del cronograma → entregable → dónde queda:**

| Actividad (Trabajo de grado, cronograma OE4) | Entregable | En este carril |
|---|---|---|
| Definición de criterios de evaluación y diseño del plan de pruebas | *Plan de pruebas* | `plan.md` + `casos.md` |
| Aplicación de pruebas funcionales al prototipo | *Resultados de pruebas* | `OE4-Resultados.md` §Casos + `evidencias/` |
| Verificación del cumplimiento de requerimientos | *Matriz de trazabilidad* | `OE4-Resultados.md` §Matriz |
| Identificación y corrección de errores e inconsistencias | *Registro de correcciones* | `OE4-Resultados.md` §Defectos + INC nuevos propuestos |
| Documentación final y retroalimentación para mejora iterativa | *Informe final* | `OE4-Resultados.md` §Mejoras (insumo; el `.docx` queda fuera de este plan, ver §12) |

**Qué NO es este OE4.** No es una evaluación con estudiantes: los puntos `PG-05` (el cambio de
esquema de control entre niveles no confunde) y `PG-06` (validar jugando los valores del Nivel 1)
exigen niños y el consentimiento de RNF-12, y pasan a *trabajos futuros* salvo que Santiago decida
otra cosa (§12, pregunta 6). Tampoco es otra corrida de la suite automatizada: la suite es
evidencia de apoyo (columna de la matriz), no cuenta en el KPI.

## 2. Qué se prueba: la versión congelada

- El objeto de prueba es el **ejecutable portable** compilado desde un commit etiquetado
  `oe4-rc1` (T01–T02), no el Editor. Los RNF de rendimiento, portabilidad, red y residuos solo
  tienen sentido sobre él (OE3 §10.2, §10.5).
- El `Build/Algoritmia/` que hay en disco es del **21/09/2026**: trae 11 escenas y le falta
  `TeacherReport`. **No sirve.** Se aparta (renombrándolo, no borrándolo) y se compila de nuevo.
- La cabecera de `OE4-Resultados.md` registra: commit, etiqueta, `Algoritmia.provenance.json`,
  SHA-256 de `Algoritmia.exe` y fecha. **Cada sesión sobre el ejecutable empieza comprobando que
  el SHA-256 coincide**; si no coincide, la sesión no cuenta.
- Tras correcciones hay `oe4-rc2`, `rc3`…: cada veredicto lleva la versión en la que se obtuvo.
- `Algoritmia_BurstDebugInformation_DoNotShip` no es parte del entregable: RNF-06 se mide sin esa
  carpeta y se anotan los dos tamaños.

## 3. Criterios de evaluación

### 3.1 El oráculo: de dónde sale el «resultado esperado»

1. El texto del **RF/RNF radicado** (`docs/md/Solución OE1_Requerimientos.md`).
2. Los **criterios de aceptación y flujos alternos** de la HU y del CU que lo trazan
   (`docs/md/Historias_de_Usuario_HU01_HU18_v2 (1).md`, `docs/md/Solucion_OE2_Diseno_final.md`
   §2–§3).
3. Cuando un **INC con decisión tomada** cambia el comportamiento (INC-47, 49, 50, 51, 54), el
   esperado es el del INC y el paso se marca `[PD: INC-nn]`.
4. **Precedencia** del proyecto cuando dos fuentes chocan: OE1 > guion > CU/HU > HU detalladas >
   arquitectura (CLAUDE.md §Precedencia). Ejemplo resuelto en el catálogo: repetir un nivel no
   reescribe los indicadores; OE1 §3.6.1 nota 4 manda sobre HU-14 FA-01, así que el esperado es el
   de OE1, y el choque se anota como observación documental. *(Nota 29/09/2026: HU-14 FA-01 y la
   nota 4 ya se corrigieron y dicen lo mismo, INC-75; el ejemplo queda como antecedente.)*
5. Los textos exactos citados en `casos.md` salen de los assets en `HEAD` al 29/09/2026. Antes de
   ejecutar se contrastan contra `oe4-rc1` (T04): si un texto cambió, **se corrige el catálogo
   antes de ejecutar, nunca durante**.

Si un criterio solo vive en una HU (no en el RF) y la implementación no lo cumple, el caso **falla**
igual: el objetivo evalúa contra «los requerimientos previamente definidos», y las HU son parte de
ellos. Si eso se arregla con código o con un INC lo decide Santiago en el triaje (§6).

### 3.2 Veredictos

| Veredicto | Cuándo | Cuenta en el KPI como |
|---|---|---|
| **P** · Aprobado | Se observan todos los esperados de los pasos que el caso agrupa. | Aprobado |
| **PD** · Aprobado con desviación documentada | Se observa lo esperado, pero algún paso sigue a un INC con decisión tomada (`[PD: INC-nn]`). | Aprobado, reportado aparte |
| **F** · Fallido | Algún esperado no se observa. Se abre un `DEF-nn` (§6). | No aprobado |
| **B** · Bloqueado | No se pudo ejecutar (precondición, entorno). **Si lo bloquea un defecto del juego, es F**, no B. | Fuera del denominador; al cierre tiene que haber **cero** B o cada uno justificado |
| **NA** · No aplica | Solo para lo que el propio RNF deja fuera de un prototipo sin usuarios (RNF-12, ver `casos.md`). | Fuera del denominador |

Un F pasa a PD **solo** si durante el triaje se registra un INC con la decisión de Santiago, y esa
reclasificación se ve en la columna de la versión final, nunca reescribiendo la primera pasada.

### 3.3 Etiquetas (para leer el KPI por lo que nombra)

`NAV` saltos de nivel y navegación entre pantallas · `BOT` botones y controles · `SON` sonido ·
`RETO` resolución de la mecánica · `DAT` perfil, guardado, informe y borrado · `RNF` rendimiento,
portabilidad, accesibilidad y contenido. Un caso lleva una etiqueta principal.

## 4. Quién ejecuta cada caso y con qué

| Ejecutor | Quién / cómo | Editor de Unity | Para qué |
|---|---|---|---|
| **EXE** | Claude maneja `Algoritmia.exe` con el arnés `herramientas/oe4.ps1` (§5): clic, clic sostenido, arrastre y capturas. **Caja negra**: solo ve lo que ve un jugador. | Cerrado (mide rendimiento) | La mayoría de los casos funcionales |
| **INSP** | Claude inspecciona archivos: JSON de `Datos/`, assets de texto, código, historial de git, carpeta del build, capturas ya tomadas. | Indiferente | Datos persistidos, contenido, nomenclatura, contraste y destellos sobre capturas |
| **SUITE** | Claude corre `unity test` (CLAUDE.md §Comandos). | **Cerrado** | Línea base y regresión; columna «automatizadas» de la matriz |
| **EDIT** | Claude con el Editor abierto por `coplay-mcp` (Play desde `Boot`). | Abierto | Solo RNF-18 (cambiar un parámetro sin recompilar) y como plan B del arnés |
| **HUM** | Santiago ejecuta con una hoja que prepara Claude (T18); Claude registra lo que Santiago reporte, sin completar nada de su cosecha. | — | Oído (sonido), cronómetro del Golden Path, segundo equipo, equipo sin GPU, modo avión, documentos (RNF-12, RNF-23) |

**Reglas para Claude en toda sesión EXE** — son las que evitan un veredicto inventado:

1. **Observar, nunca inferir.** Después de cada acción cuyo resultado cuente para un caso: `Foto`
   y leer la imagen con Read. Que no haya error no significa que el esperado ocurrió.
2. Esperar los fundidos (0,4 s por mitad) y las animaciones que el guion nombra antes de
   fotografiar. Para medir cargas se usa `Medir-Carga` (§5), no una foto a ojo.
3. Anotar cada paso en el registro de la sesión (`scratchpad/oe4/<sesión>.md`) en el momento:
   paso, hora, lo observado y la captura. `OE4-Resultados.md` se llena al cerrar la sesión.
4. Si algo se sale del guion (un bloque pegado, una pantalla que no avanza): foto, `Revisar-Log`,
   `DEF` provisional, y se **recupera** (pausa → «Volver al menú de niveles», o `Matar` y
   relanzar con `Preparar-Datos`) para seguir con los pasos independientes. Un tropiezo no aborta
   la sesión entera.
5. **Separar el error del probador del error del juego.** Si la carretilla choca porque Claude leyó
   mal el tablero, no hay defecto: se replantea la secuencia y se deja una nota.
6. **Durante las sesiones no se toca código, escenas ni assets.** La única excepción es RNF-18, que
   se revierte (T17).
7. Toda sesión termina con `Revisar-Log`: cualquier `Exception` en `Player.log` es un `DEF`.

## 5. El arnés `herramientas/oe4.ps1` (tarea T03)

Script de PowerShell sin dependencias nuevas: `Add-Type` con P/Invoke a `user32` para `SendInput`,
`SetCursorPos` y la ventana, y `System.Drawing` para capturar. **No es código del juego** (como
`claudeDocs/tasks/Personajes/herramientas/`).

| Función | Qué hace |
|---|---|
| `Start-Juego [-Ancho 1280 -Alto 720]` | Comprueba el SHA-256 contra la cabecera de resultados y lanza el exe con `-screen-fullscreen 0 -screen-width <A> -screen-height <H> -popupwindow` (ventana sin marco, coordenadas estables). Guarda el PID y arranca en segundo plano el **muestreador**: cada 2 s escribe memoria (`WorkingSet64`, `PrivateMemorySize64`, picos) en `mem.csv` y conexiones (`Get-NetTCPConnection` / `Get-NetUDPEndpoint -OwningProcess`) en `red.csv`. |
| `Foto <nombre>` | Captura el área cliente y la guarda en `scratchpad/oe4/<nombre>.png`. Devuelve la ruta para leerla con Read. |
| `Clic <x> <y>` | Clic en coordenadas **normalizadas** del área cliente (0–1, origen arriba a la izquierda): la misma idea de fracciones que usa `IllustrationFraming`, y no depende de la resolución. |
| `Sostener <x> <y> <ms>` | Clic sostenido sin moverse (flechas del N3). |
| `Arrastrar <x1> <y1> <x2> <y2> [<ms>]` | Pulsar, mover en ~20 pasos y soltar (piezas del N1, caja, bloques, piezas de la balsa). |
| `Escribir <texto>` | Escribe en el campo con foco (`SendInput` Unicode). Solo lo usa el nombre del perfil, la única entrada de texto del juego. |
| `Teclas <lista>` | Envía teclas (Esc, Enter, flechas, espacio, WASD, rueda). Solo sirve para comprobar que **no** hacen nada (RNF-02). |
| `Medir-Carga` | Desde el clic (o desde el arranque del proceso), sondea capturas cada 100 ms y devuelve los segundos hasta el **primer cuadro no negro de la escena destino tras el fundido** (luminancia media y desviación sobre un umbral; el umbral se ajusta en el spike, porque la cueva del N1 es oscura). Incluye el fundido: es una medida conservadora. |
| `Preparar-Datos <perfiles…>` | Con el juego cerrado, vacía `Build/Algoritmia/Datos/` y copia los perfiles pedidos: un nombre (`OE4_B`) sale de `herramientas/perfiles/`, una ruta (`evidencias/S-N1_OE4N1.json`) se copia tal cual con el nombre del perfil. |
| `Matar` | `taskkill /F` del PID (RNF-14). |
| `Cerrar-Juego` | Cierre normal (o `Matar` si no responde en 10 s) y detiene el muestreador. |
| `Revisar-Log <sesión>` | Copia `%USERPROFILE%\AppData\LocalLow\Universidad Catolica de Colombia\Algoritmia\Player.log` a `evidencias/<sesión>_Player.log` y lista las líneas con `Exception`, `Error` o `RNF-04:`. |

Detalles técnicos que el spike tiene que resolver: el proceso del arnés declara DPI-aware (si no,
las coordenadas no coinciden en pantallas escaladas); la ventana del juego tiene que estar en primer
plano para recibir `SendInput`; entre pulsar y soltar hay al menos 50 ms.

**Spike (criterio de aceptación de T03):** lanzar, capturar el menú, abrir «Créditos» y volver con
clics, arrastrar una pieza del N1 al círculo, y mover a Mamá con clic sostenido en una flecha del
N3. **Si Unity no recibe la entrada sintetizada:** plan B, decidido en el Checkpoint A con
Santiago: los casos EXE pasan a HUM con el mismo guion, o a EDIT (Play en el Editor, clics con
`ExecuteEvents` vía `coplay-mcp`), dejando constancia de que dejan de ser sobre el ejecutable.

La ventana de 1280×720 es una condición de prueba. La experiencia entregada es a pantalla
completa, y la cubre el Golden Path humano (PF-RNF13-01).

## 6. Defectos y correcciones

Cada F abre un defecto en `OE4-Resultados.md` §Defectos:

```
### DEF-nn · <título en una línea>
- Caso: PF-… paso n · Versión: oe4-rcN · Severidad: Bloqueante | Mayor | Menor
- Tipo propuesto: código | documento (→ INC nuevo) — lo decide Santiago en el triaje
- Reproducción mínima: 1… 2… 3…
- Esperado (fuente) / Observado
- Evidencia: evidencias/…
- Decisión (Santiago, fecha): corregir en OE4 | registrar INC | trabajo futuro
- Corrección: commit sugerido · prueba nueva (nombre con el RF, p. ej. MazeScene_RF34_…) · reprueba: rcN → veredicto
```

**Severidad.** *Bloqueante*: impide terminar la ruta principal, cierra el juego, pierde progreso
confirmado o deja un estado del que no se sale. *Mayor*: no se cumple un RF o un criterio de
aceptación, pero el juego sigue. *Menor*: texto, aspecto o criterio secundario, sin efecto sobre el
reto.

**Política.** Bloqueantes y Mayores se corrigen dentro del OE4 salvo que Santiago diga otra cosa;
los Menores los decide Santiago. Cada corrección sigue `unity-coding-skills:fix-bug` (reproducir
con una prueba que falla, diagnosticar, corregir). Claude **no hace commit**: deja el mensaje
(CLAUDE.md §Commits), asociado a la tarjeta del Kanban que abra el acta. Las correcciones
documentales **nunca** tocan un `.docx`: se proponen como INC nuevos en `INCONSISTENCIAS.md` para
que los autores corrijan el documento a mano.

**Regresión tras cada tanda de correcciones:** nueva versión `rcN`, la suite completa (SUITE), el
caso fallido y **todos los casos de su sesión**.

## 7. Perfiles semilla (tarea T04)

Un perfil sembrado a mano es un `Datos/<name>.json`, donde `name` **tiene que coincidir** con el
nombre del archivo, con las tres claves siempre presentes. El cargador no valida nada más allá del
parseo (`SaveStore.Load` = `JsonUtility.FromJson`): un `reachedLevel` 0 o una fase inexistente
dejan el juego sin salida. Por eso las semillas se copian del catálogo y **no se improvisan**.
Formato (verificado en `PlayerProfile.cs`):

```json
{"name":"X","reachedLevel":2,"phases":[{"level":1,"phase":1,"attempts":7,"correctedErrors":3,"stepsUsed":3,"resolutionSeconds":125.0}]}
```

| Semilla | Estado | Se usa para |
|---|---|---|
| `OE4_B` | N1 hecho (N1F1 = 7/3/3/125,0 → «2:05»); `reachedLevel` 2 | N2 bosque desde el menú |
| `OE4_B2` | + N2F1 (4/2/0/61,0) | Entrar directo al taller |
| `OE4_B3` | + N2F2 (5/3/6/95,0) | Entrar directo al laberinto |
| `OE4_C` | N2 completo (+ N2F3 2/5/12/140,0); `reachedLevel` 3 | N3 desde la recolección |
| `OE4_D` | + N3F1 (1/1/1/40,0) | N3 en el amarre |
| `OE4_E` | + N3F2 (2/1/2/70,0) | N3 en mástil y vela |
| `OE4_Z` | `OE4_E` + N3F3 (3/2/3/100,0); las siete fases; `reachedLevel` 3 | Rejugar (aparece «Omitir»), pausa rápida, informe docente |
| `OE4_Corrupto` | `{"name":"OE4_Corrupto","reachedLevel":` (JSON truncado) | Robustez (PF-RF02-04) |

Las cifras de las semillas son **distintas entre sí a propósito**: en el informe docente
(PF-RF46-01) se sabe de qué fila viene cada número. **Los indicadores solo se guardan la primera
vez que se confirma una fase** (`PlayerProfile.ConfirmPhase`), así que toda sesión guionizada cuyas
cifras se vayan a comprobar arranca de una semilla recién copiada o de un perfil nuevo. Nunca se
reutiliza un perfil ya jugado.

## 8. Evidencia y registro

- Las capturas de trabajo van al scratchpad. A `claudeDocs/tasks/OE4/evidencias/` solo se copia
  **la que sostiene un veredicto** (1–3 por caso), con el nombre `<caso>_<paso>.png`. Los `.png`
  van por LFS (`.gitattributes`). Junto a ellas: `<sesión>_Player.log`, `<sesión>_mem.csv`,
  `<sesión>_red.csv` y, en las sesiones de datos, el JSON antes y después.
- `OE4-Resultados.md` tiene cinco secciones: cabecera de la versión; tabla de casos (caso · req ·
  etiqueta · ejecutor · versión · fecha · veredicto · evidencia · DEF/INC · nota); defectos;
  matriz; KPI y mejoras.
- **La matriz de trazabilidad** (T25): una fila por requerimiento (47 RF + 23 RNF), con sus casos
  PF y sus veredictos, las pruebas automáticas que lo nombran (grep de `_RFnn_` / `_RNFnn_` en
  `Assets/Tests`, con cuántas pasaron en la última SUITE) y el DEF o INC asociado. Parte del Anexo A
  del OE3, cuyo generador ya no existe: se rehace con un grep, sin herramienta nueva.

## 9. Riesgos que se sabe que existen antes de empezar

La exploración del código (29/09/2026) encontró puntos donde un caso **probablemente fallará**. Se
dejan escritos para que nadie los lea después como sorpresa ni como error del arnés:

| Riesgo | Caso | Gravedad probable |
|---|---|---|
| Tomar un bloque que **ya está** en la secuencia del laberinto (para reordenarlo o retirarlo soltándolo fuera): la fila que recibió el «pulsar» se destruye en `RefreshRows` y el «soltar» nunca llega (`MazeSceneController.cs:732`, `:940-942`); el bloque se queda pegado al cursor. Ninguna prueba lo cubre con entrada real. | PF-RF34-02 | Mayor, quizá Bloqueante |
| Cierre forzado durante la narrativa de cierre del N1: la fase del N1 solo se guarda en `LevelSummary`, así que se pierde el nivel entero (RF-04: «al completar cada fase»). | PF-RNF14-03 | Mayor |
| Cierre forzado durante la escena 2.5: las tres fases del N2 quedan en disco, pero `reachedLevel` sigue en 2; el N3 queda bloqueado y hay que rehacer el N2 entero. *(30/09/2026: decisión de Santiago, INC-116 — el menú de niveles deriva el desbloqueo del N3 de las tres fases confirmadas, sin dato nuevo en el perfil; el caso comprueba además que el N3 se pueda jugar.)* | PF-RNF14-04 | Mayor |
| «Salir» cierra sin la confirmación ni el aviso de guardado de HU-18. *(30/09/2026: resuelto en el juego — UI-03/UI-04, INC-77.)* | PF-RF09-02 | Menor (criterio de HU) |
| Nivel bloqueado sin mensaje (CU-02 FA-2a); sin marca de «completado» (HU-14 paso 7). *(30/09/2026: resuelto — UI-07 y CU-02 2a corregido, INC-76.)* | PF-RF03-03 | Menor |
| En el resumen del N3, «Volver al menú de niveles» lleva a la escena final. | PF-RF45-03 | Menor |
| Commits con `(tarjeta: <id>)` sin tarjeta (`44fd479`, `d3a6cc9`, `1d5ce58`, `b38c00b`…). | PF-RNF17-01 | Menor |
| El laberinto se genera al azar en cada partida (`Seed: 0`): no hay una secuencia ganadora fija, y los arbustos se dibujan a ×1,65 y tapan casillas vecinas. | Sesión S-N2C | Del probador: mal leído ≠ defecto |
| RF-35 pide las flechas «en los costados»; están en cruceta abajo a la derecha. RF-38 dice «cuatro objetos»; hay 4 casillas y 8 piezas. Ninguna de las dos tiene INC. *(30/09/2026: INC-91 e INC-89; los documentos se alinearon con el juego.)* | PF-RF35-01, PF-RF38-01 | A decidir: código o INC |
| El objetivo del N2 se da en imperativo; RF-10 pide «diálogo formulado en preguntas». *(30/09/2026: RF-10 corregido, INC-62.)* | PF-RF10-02 | A decidir |
| Doble clic en «Soplar» durante la ignición relanza la resolución (`Blow` no deshabilita los botones). | PF-RF20-02 | Por ver |
| Paquetes de IA del Editor en el build (DirectML.dll, D3D12) y el módulo de analítica en el manifiesto: riesgo para RNF-10. | PF-RNF10-01 | Por ver |

Riesgos del propio plan:

| Riesgo | Impacto | Mitigación |
|---|---|---|
| Unity no recibe `SendInput`, o hay problemas de foco o de DPI | Alto | Spike en T03, antes de todo lo demás; plan B en §5 |
| Medir con el Editor, la suite u otras apps abiertas contamina RNF-04 y RNF-05 | Medio | Nada más abierto durante las sesiones EXE; carga medida 3 veces, se reporta la peor |
| Latencia de captura (~100–200 ms) frente al «< 1 s» de RF-11 | Bajo | Foto fija a +0,5 s del clic: si el mensaje ya está, pasa |
| La primera pasada queda por debajo del 90 % por los riesgos conocidos | Medio | Para eso existe la Fase 4; se publican las dos pasadas |
| Disponibilidad de Santiago y de un segundo equipo para HUM | Medio | La hoja HUM se prepara pronto (T18) y no bloquea el trabajo de Claude |

## 10. Fases (el detalle está en `todo.md`)

```
Fase 0  Preparación ─ T01 congelar → T02 build → T03 arnés (spike) → T04 semillas + resultados
                        │ Checkpoint A
Fase 1  Línea base ── T05 suite completa · T06 inspecciones estáticas
Fase 2  Recorridos ── T07 inicio/perfiles → T08 N1 → T09 bosque → T10 taller → T11 laberinto → T12 N3
                        │ Checkpoint B (Golden Path negro completo)
                      T13 pausa · T14 omisión/rejuego · T15 persistencia · T16 informe/borrado · T17 accesibilidad/RNF
                        │ Checkpoint C (primera pasada completa)
Fase 3  Personas ──── T18 hoja HUM → T19 ejecución HUM · T20 consolidar RNF-04/05/10
                        │ Checkpoint D
Fase 4  Corrección ── T21 triaje → T22 un DEF por subtarea → T23 rc2 + regresión
                        │ Checkpoint E
Fase 5  Cierre ─────── T24 KPI · T25 matriz · T26 correcciones e INC · T27 mejoras → T28 revisión
```

Las sesiones de la Fase 2 son **cortes verticales**: cada una recorre un tramo jugable completo,
de la pantalla que la abre a la que la cierra, y verifica en el camino todos los casos que ese
tramo toca. Por eso un caso como RF-13 (pista) aparece en cinco sesiones: una por escena.

**Qué puede ir en paralelo.** Las inspecciones (T06, parte de T17) pueden ir a subagentes mientras
Claude corre sesiones EXE. La SUITE **no** comparte máquina con una sesión EXE que mida carga o
memoria, y dos carriles no corren pruebas a la vez (CLAUDE.md §Comandos).

## 11. Cobertura (CT-10)

`casos.md` §Índice lista los 47 RF y los 23 RNF con al menos un caso cada uno. Es la condición
de CT-10: «todo requerimiento… debe corresponderse con al menos un caso de prueba en el plan de
pruebas del cuarto objetivo específico». T06 la comprueba mecánicamente (grep de `PF-RFnn` y
`PF-RNFnn` en `casos.md` contra la lista cerrada).

## 12. Preguntas abiertas para Santiago

1. **Congelar.** HEAD (`127fbc4`) está en `feat/implementación-de-props-y-sonidos`, 7 commits por
   delante de `main`. ¿Se integra a `main` y se etiqueta `oe4-rc1` ahí? Faltan música, sonido de
   diálogo y ambiente nocturno (D07-2/D08-2). Además, el 29/09 otra sesión dejó sin commit un
   cambio en `WheelLevelJourneyTests.cs`. *Recomendación:* congelar en cuanto ese cambio entre;
   lo que llegue después entra como `rc2` con regresión.
2. **PD en el KPI.** ¿Un aprobado con desviación documentada cuenta como aprobado? *Recomendación:*
   sí, reportado aparte.
3. **Criterios que solo están en HU** (confirmación al salir, mensaje de nivel bloqueado, marca de
   completado…): ¿código o INC? Se decide caso por caso en el triaje (T21).
   *Respondida el 30/09/2026: se implementaron en el juego (INC-76, INC-77).*
4. **El arnés como ejecutor de caja negra.** ¿Vale para el trabajo de grado que las pruebas EXE las
   ejecute Claude con el arnés, declarándolo como se declaró su papel en el OE3? El Golden Path
   humano ×2 queda como evidencia humana.
5. **Equipos.** ¿Cuál es el segundo equipo (RNF-07) y cuál el que no tiene tarjeta gráfica
   dedicada (CT-02)? ¿Santiago activa el modo avión para RNF-08?
6. **Estudiantes.** ¿`PG-05` y `PG-06` quedan como trabajos futuros? *Recomendación:* sí.
7. **Antes de compilar `rc1`:** ¿se retiran los paquetes de IA (`com.unity.ai.*`), DirectML y el
   módulo de analítica? Cambia lo que se mide en RNF-06, 08 y 10, y es una decisión pendiente de
   los autores (OE3 §10.2). Si no se retiran, se prueban tal como están.
   *Respondida el 29/09/2026 (Santiago): no se retira ningún paquete del manifiesto; la analítica,
   las estadísticas de hardware, la pantalla de presentación de Unity y Alt+Intro se desactivan en
   ProjectSettings, y lo vigila una prueba de arquitectura. DirectML.dll y `D3D12/` siguen en el
   paquete y se anotan en PF-RNF06-01 y PF-RNF10-01.*
8. **Informe final.** Las herramientas que armaron el `.docx` del OE3 ya no existen. ¿Quién redacta
   el del OE4? Este plan deja los insumos en `OE4-Resultados.md`.
9. **Evidencias en el repositorio** (por LFS, estimado 30–60 MB): ¿de acuerdo?
