# Encargo para la sesión en la nube (rama `ajustes-OE4`)

La sesión local (Windows, con Unity y Word) y esta sesión en la nube trabajan en paralelo sobre
la misma rama. El plan completo está en `claudeDocs/tasks/OE4/Plan-Sesion-13-10.md`. Esta nube
hace **C1, C2 y C3**, y **C4** cuando `Nube-estado.md` diga que llegó el push del código local.

## Reglas de git (no se negocian)

- Primero `git fetch && git switch ajustes-OE4`. Se trabaja solo en esa rama.
- Todo commit y todo push sale **únicamente a nombre de `santivaldiri`**. Antes del primer
  commit hay que fijar `git config user.name "santivaldiri"` y el email de esa cuenta de GitHub
  (el noreply `…+santivaldiri@users.noreply.github.com` si no se conoce otro).
  **Sin `Co-Authored-By` de Claude ni ninguna línea de atribución** en el mensaje.
- Antes de cada push, `git pull --rebase`. Los commits son pequeños, van en inglés y citan su
  INC, por ejemplo `Plainer vocabulary in the narratives for fifth graders (INC-157)`.
- En cada push se actualiza `claudeDocs/tasks/OE4/Nube-estado.md` con la tarea, el commit, lo
  hecho y lo que falta. La sesión local solo se entera de tu avance por ese archivo y por los
  commits.
- No se escriben `.meta`, nunca.

## Lo que esta nube no toca

- `*.unity`, `*.prefab` y `*.anim`.
- Ningún `.docx`.
- Ningún `*.cs` fuera de la prueba de C2.
- `ActionView`, `coreografia.py`, `clips_personajes.json`, `PauseMenu*`, `Forest*`, `Maze*`,
  `Fire*`, `CaveLighting*`, `N2_WheelLevelConfig.asset`, `N2_MazeLayout.asset`,
  `DialogueRunner.cs` y `NarrativeSceneController.cs`.
- Los beats y posiciones de `N1_NacimientoDelFuego.asset`, que son de la sesión local. Ahí solo
  se tocan las líneas de texto.

## Reglas del proyecto que aplican

- Código e identificadores en inglés; textos del jugador y documentación en español.
- Los assets de Unity (`.asset`) se editan como YAML con cuidado: solo el valor del campo,
  sin reordenar ni tocar `fileID`/`guid`. Las cadenas con caracteres especiales conservan el
  formato de comillas o escapes que ya tengan, porque Unity escribe los no ASCII como
  `"\xE1"`/`á` dentro de comillas dobles. Hay que mirar cómo está escrita la línea antes de
  cambiarla.
- No hay Unity en la nube. El C# se verifica con `mcs` (`apt-get install -y mono-mcs`) y stubs
  mínimos de `UnityEngine`/NUnit en un scratchpad **fuera del repo**.
- Las pruebas se nombran `<Sujeto>_<Requisito>_<QuéHace>` en español, por ejemplo
  `NarrativeSequence_INC153_LosPasosDeLocomocionDesplazanAlActor`.
- Los documentos siguen `claudeDocs/entregables/Guia-redaccion-sin-marcas-de-IA.md`, con estas
  reglas añadidas:
  - Voz impersonal en pretérito perfecto simple («se decidió», «se definió»). Se quita toda la
    tercera persona de Santiago («Santiago decidió», «decisión de Santiago», «lo hace
    Santiago»).
  - El grupo es de **grado quinto**, no cuarto. «Cuarto objetivo» y «cuarto golpe» no cambian.
  - No se afirma ningún resultado que no esté abajo o medido.
- No hay que releer `CLAUDE.md` entero. Si hace falta algo, se busca con grep.

## C1 · Vocabulario para niños de 10 años (INC-157)

Hay que cambiar los textos visibles al estudiante en `Assets/Game/Data/Narrative/*.asset`
(las 18 narrativas, incluido el texto de `N1_NacimientoDelFuego.asset`) y en los SO de texto de
`Assets/Game/Data/River/` y `Assets/Game/Data/Wheel/`. No se toca `Reporting/`, que es la
pantalla docente.

| Palabra | Reemplazo | Dónde aparece |
|---|---|---|
| civilización | otras familias | `N3_Escena33_Cruce`, `N3_PuenteII_Horizonte` |
| abruptamente | de repente | `N3_PuenteII_Rio` |
| intemperie | al aire libre, sin techo | `N1_Apertura` |
| maleza | matorrales | `N1_Apertura` |
| silueta | sombra | `N1_Apertura`, `N1_NacimientoDelFuego` |
| estela | rastro | `N1_AparicionGuia` |
| crepita | chisporrotea / hace crac | `N1_NacimientoDelFuego` |
| estabilidad | reescribir: «para que no se voltee» | `N3_Escena33_Cruce` |
| computacionalmente | reescribir con «pensar como un algoritmo» | `N3_Escena33_Cruce` |
| irregulares | torcidos / de formas distintas | `N2_Escena21_Bosque` |
| inventario | tus materiales | `River/N3_RaftAssemblyContent`, `N3_RiverLevelConfig` |

- Se barren **todos** los textos visibles en busca de otras palabras difíciles y se aplican
  cambios conservadores. La frase sigue diciendo lo mismo y el tono no cambia.
- **Se conservan** los términos del pensamiento computacional («abstraer», «iterar»,
  «descomponer», «algoritmo»). Los exige `Assets/Tests/EditMode/UI/LevelSummaryComposerTests.cs`
  (líneas ~93 y ~121-129). Donde aparezcan sin explicación, se añade al lado una explicación
  llana: «Eso se llama abstraer: quedarse solo con lo que importa».
- Hay que mirar si alguna prueba en `Assets/Tests/` cita literalmente una frase que se cambie
  (grep de un fragmento). Si es así, se actualiza la prueba.
- **Entregable para el guion:** `claudeDocs/entregables/tools/pares/guion-inc157.json`, con los
  pares texto viejo → texto nuevo **tal como aparecen en el guion radicado**. La conversión está
  en `docs/md/Solucion_OE2_Diseno_final.md`, pero `docs/md/` está en `.gitignore` y no existe en
  la nube. Por eso se escriben los pares con el texto del asset y se marca en `Nube-estado.md` que
  la sesión local debe cotejarlos con el guion. El formato es el de los otros JSON de
  `claudeDocs/entregables/tools/pares/`, y hay que mirar uno antes de escribirlo. La aplicación
  al `.docx` la hace la sesión local.
- En `Nube-estado.md` va la tabla de todo lo aplicado: archivo, texto viejo y texto nuevo.

## C2 · Oscuridad del Nivel 1 (INC-152) y caminata de la escena 1.1 (INC-153)

Los estudiantes vieron el inicio del Nivel 1 demasiado oscuro.

En `N1_Apertura.asset`, `N1_AparicionGuia.asset` y `N1_Hallazgo.asset`:
- A cada `Ambient` de `LightStart` y de cada parada (`CameraKey.Light`) se le suma **0,12**, con
  un mínimo de **0,15**.
- **Excepción:** el negro intencional de `N1_Apertura`, la parada de la línea 6 (`Ambient: 0`
  con `HardCut`), se queda en 0.
- No se toca nada más de la luz.

En la escena 1.1 de `N1_AparicionGuia.asset` (línea de diálogo índice 7, «Avanza despacio hacia
el fondo»), Papá y Mamá caminaban en sentido contrario sin desplazarse. Sus beats Walk iban de
x 0.285 a 0.281 y de 0.335 a 0.325. Como el desplazamiento quedaba por debajo de
`ActorTimeline.MinimumDisplacement` (0.01), el rumbo caía al lado de la pantalla.
- Hay que mover los destinos (`To`) de los beats Walk de esa línea de **Papá, Mamá, Niña y Niño**
  a x ≈ 0,18-0,22, conservando el orden y la separación entre ellos y la `y`.
- Esos beats llevan `Facing: 1` (izquierda; `ActorFacing`: Auto = 0, Left = 1).
- **Prueba de causa raíz** en `Assets/Tests/EditMode/` (junto a `NarrativeSequenceTests`):
  - Recorre las 18 narrativas.
  - Exige que todo beat Walk, Run, Carry o Push con `Moves` tenga un desplazamiento de al menos
    `ActorTimeline.MinimumDisplacement`.
  - Se lee el código de `ActorTimeline` y `ActorBeat` para usar sus campos reales.
  - Se compila con `mcs` y stubs.
- Se anota en `Nube-estado.md` que la sesión local debe comprobar en el Editor que se cumple
  `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo` y que el encuadre los
  contiene.

## C3 · Resultados de la sesión con estudiantes, en markdown (INC-151)

**Datos de la sesión, que no se amplían:**
- Fecha y lugar: el 13/10/2026, en el Colegio El Libertador IED, sede A, con 4 estudiantes de
  grado quinto.
- Método: un observador miró cómo jugaban los estudiantes. Los estudiantes pensaron en voz alta
  y comunicaron sus inquietudes al observador.
- Autorización institucional: «Formato de autorización institucional» del 17/09/2026, firmado por
  el rector del Colegio El Libertador IED, Richard Maldonado. Autoriza a grado quinto de primaria
  dentro de la investigación doctoral de la docente Bibiana Patricia Rey Barrote, contacto del
  equipo en el colegio, con la condición del consentimiento informado de padres o acudientes. No
  se publican el número de cédula ni las firmas.
- Consentimientos: los cuatro estudiantes tienen el consentimiento firmado y se custodian.
  **El formato de consentimiento lo entrega esta sesión en la nube**: súbelo a
  `claudeDocs/tasks/OE4/evidencias/consentimiento/` (formato en blanco, sin datos de menores),
  descríbelo en el texto (quién firma, qué autoriza, borrado de datos) y anótalo en
  `Nube-estado.md`; la sesión local lo anexa a los `.docx`. Hasta que esté, el hueco queda
  `[Pendiente: formato de consentimiento de la aplicación]`.

**Tabla 5.1** de `claudeDocs/entregables/OE4/src/05-estudiantes.md`:
- Visto bueno del colegio: el de arriba.
- Consentimientos firmados: 4.
- Fecha y lugar: los de arriba.
- Método: el de arriba.
- PG-05: positivo.
- PG-06: positivo.
- Constancia de borrado: positivo. Se borraron los perfiles de la sesión y `Datos\` quedó vacía
  en los equipos.

**Tabla 5.2:**

| Id | Resultado |
|---|---|
| H1 | Aprobado |
| H2 | Aprobado |
| H3 | Aprobado |
| H4 | Aprobado |
| H5 | Aprobado (equipos abajo) |
| H6 | Aprobado: el sonido se escucha |
| H7 | Aprobado |
| H8 | Entre 40 y 45 min |
| H9 | Aprobado, sin comentarios: todo se comprendió |
| H10 | Aprobado, sin comentarios: todo se comprendió |
| H11 | Aprobado |
| H12 | Aprobado, sin comentarios: todo se comprendió |
| H13 | No se realizó en la sesión; pasa a trabajo futuro |

- H5, equipos:
  - Equipo 1: AMD A10-9600P, Radeon R5, 4 GB de RAM, 66 MB de VRAM, 512 GB de disco, sin
    conexión a internet.
  - Equipo 2: Intel Celeron B830, Intel HD Graphics de 32 MB, 4 GB de RAM, 512 GB de disco, sin
    conexión a internet.
  - La sala tenía internet disponible, de 20 Mb/s de bajada y 7 de subida, y no se usó.
- H8: la meta es de 20 a 40 min, así que supera el tope en hasta 5 min. Se reporta sin inventar
  una causa.
- H11: la balsa se hundió, se comprendió por qué, se arregló y finalmente cruzó.
- H13 son las comprobaciones a mano de `Hoja-HUM.md`: el clic tras cargar una escena, la papelera
  del laberinto y la octava fila del informe.

**Archivos:**
- `claudeDocs/entregables/OE4/src/03-resultados.md` (la línea ~51 sobre H8 y su fila).
- Los demás `src/*.md` del OE4, con la tercera persona de Santiago y «cuarto» → «quinto»
  (`01-introduccion`, `02-metodo`, `04-mejoras`, `05-estudiantes` y `06-conclusiones`).
- `claudeDocs/entregables/TG/cap9-evaluacion.md`: los `[Pendiente: …]` de la sesión, del método,
  de PG-05/PG-06, de H3 a H13 y de los equipos. El pendiente de la Familia Anonaky y la
  colaboradora de arte (PF-RNF22-01, PF-RNF23-01) **no se toca**. También la tercera persona y
  «quinto».
- `claudeDocs/INCONSISTENCIAS.md`: **INC-151** (rev. 25, 13/10/2026). El grupo de la evaluación
  es de grado quinto por la autorización del colegio. El juego y los entregables OE4/TG pasan a
  «quinto», y los radicados OE1 y OE2 siguen diciendo «cuarto» hasta que haya autorización
  expresa para editarlos.
- `claudeDocs/tasks/OE4/OE4-Resultados.md`, `todo.md` y `Hoja-HUM.md`: notas fechadas del
  13/10/2026 con los resultados. Los `*-Resultados.md` son registro histórico: se añaden notas y
  no se reescribe lo anterior.
- **No** se escribe todavía la sección de hallazgos y correcciones; es C4.

## C4 · Al llegar el push del código local

`Nube-estado.md` dirá cuándo empieza y qué commit leer.

Para el entregable OE4 se escribe una sección de hallazgos de la sesión, con la corrección
aplicada a cada uno:
- La oscuridad.
- La caminata 1.1.
- El abrazo de perfil.
- El sueño de perfil.
- Los troncos del bosque, más lentos y con el área de clic sobre todo el cuerpo.
- Las flechas curvas de «Girar».
- El vocabulario.
- El modal del botón de ayuda.
- La pausa con «Volver a ver la historia» y «Guardar y salir».
- El botón «Atrás» del diálogo.

También van INC-152 a INC-160 en `INCONSISTENCIAS.md`, `claudeDocs/Interfaces.md`,
`Assets/Game/Art/Inventario.md` (las flechas nuevas) y la línea de grado de `CLAUDE.md`. Todo se
escribe sobre lo que diga el diff del commit, y nada más.

## Orquestación en la nube

Conviene repartir con subagentes `sonnet`:
- C1 y C2: un agente programador.
- C3: un agente documentador.
- C4: al final.

Cada agente: sin commit ni push (los hace la sesión principal de la nube con la identidad de
arriba), sin `.meta` y con un informe conciso.
