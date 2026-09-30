# 2. METODOLOGÍA DEL DESARROLLO

Este capítulo describe cómo se organizó, se planeó, se siguió y se verificó el desarrollo del prototipo entre el 29 de agosto de 2026, fecha del registro inicial del repositorio, y el 25 de septiembre de 2026, fecha del corte de este documento. Se aplica la metodología adoptada en el trabajo de grado —Árcade complementada con Kanban— y se detalla la forma concreta que tomó en un equipo de dos estudiantes que contó, para los recursos gráficos, con una colaboradora externa.

## 2.1 Árcade y Kanban en la fase de desarrollo

El trabajo de grado adopta la metodología Árcade, propuesta por López-Mera para el desarrollo de videojuegos en contextos formativos con equipos reducidos, recursos limitados y plazos acotados, y la estructura en cuatro fases: Sensibilización, Especificación, Diseño y el Ciclo Diseño ↔ Desarrollo ↔ Pruebas. La gestión operativa de las actividades se confía a Kanban, con un tablero de tres columnas: «por hacer», «en proceso» y «finalizado». La fase 3 del trabajo de grado, asociada a este objetivo, enumera siete actividades que van de la implementación de la estructura base a la optimización del rendimiento; la Tabla 2.1 indica dónde se ejecutó cada una y en qué capítulo de este documento se da cuenta de ella.

En el desarrollo, la sesión del 2 de septiembre (acta D01), en la que se incorporó a la colaboradora de assets y se le entregó el documento de dirección de arte, se registró en el cuarto momento de la fase de Diseño de Árcade —identificación y elaboración de assets—. Desde la sesión del 5 de septiembre (acta D02), todas las actas registran el trabajo dentro del Ciclo Diseño ↔ Desarrollo ↔ Pruebas: la D02 como planeación del ciclo y las D03 a D09 como su ejecución.

El ciclo operó en tres escalas. En la más fina, cada tarea del plan se diseñó a partir de los documentos radicados, se implementó y se verificó con pruebas automáticas antes de darse por terminada (véase §2.5). En la intermedia, cada fase de un incremento terminó en un punto de control con criterios declarados. En la más amplia, lo aprendido al desarrollar volvió al diseño: la mecánica del Nivel 1 se rehízo en dos fases, entre el 12 y el 15 de septiembre, que no figuraban en el plan original (véase Anexo B, Fases 5 y 6), y el ensamblaje de la balsa del Nivel 3 se definió en sesión (acta D07) apartándose de la ventana superpuesta que preveía el plan. Es la retroalimentación hacia fases previas que el trabajo de grado anticipa para su enfoque iterativo. Las pruebas del ciclo fueron, en esta fase, pruebas técnicas continuas, que es la actividad que el trabajo de grado asigna a la fase 3; la evaluación del prototipo mediante pruebas funcionales corresponde al cuarto objetivo.

Kanban se aplicó en dos niveles. El tablero del proyecto vive en la sección 6 de cada acta, «Compromisos y tablero Kanban»: cada compromiso es una tarjeta numerada con la serie del acta que la abre (D02-1, D07-3), con responsable y fecha límite, y cada sesión registra los movimientos entre columnas o declara finalizadas las tarjetas cumplidas. Debajo de cada tarjeta de desarrollo de un incremento, el tablero de tareas de ese incremento (véase §2.2) descompone el compromiso en casillas verificables; así, la tarjeta D07-1, «cerrar el tercer slice», se cumplió con el cierre de las tareas R09 a R16 del Nivel 3. La planeación de la D02 es coherente con la limitación del trabajo en curso, principio que el trabajo de grado toma de Anderson: de las cuatro tareas de la fase de navegación solo se comprometieron las dos que se sostienen mutuamente —pantalla de inicio y perfil— y que obligan a cablear por primera vez la persistencia real en disco, y las otras dos pasaron a la iteración siguiente.

**Tabla 2.1.** Actividades de la fase 3 del trabajo de grado, dónde se ejecutaron y dónde se documentan.

| Actividad de la fase 3 | Dónde se ejecutó | Dónde se documenta |
|---|---|---|
| Implementación de la estructura base del videojuego | Slice 1, fases 0 a 2 | Capítulo 3 |
| Integración de la interfaz, escenarios y elementos visuales | Slices 1 a 4, carril de arte y sonido y personajes animados | Capítulo 7 |
| Desarrollo de los niveles y retos definidos | Slices 1, 2 y 3 | Capítulo 4 |
| Programación de las mecánicas de juego | Slices 1, 2 y 3 | Capítulo 5 |
| Implementación de desafíos centrados en problemas lógicos | Slices 1, 2 y 3 | Capítulos 4 y 5 |
| Realización de pruebas técnicas continuas durante el desarrollo | Todas las tareas | §2.5 y capítulo 9 |
| Optimización del rendimiento del videojuego | Slices 1 y 3, carril de arte y sonido y personajes animados | Capítulo 8 |

## 2.2 Planificación por slices y carriles

Los planes técnicos de los cuatro incrementos se redactaron el 30 de agosto de 2026 y se formalizaron en la sesión del 5 de septiembre (acta D02). El criterio acordado fue organizar el desarrollo por incrementos verticales y no por capas técnicas: una planeación por capas deja el juego sin poder ejecutarse hasta el final, mientras que un incremento vertical —un *slice*— entrega una porción jugable de principio a fin. El primer incremento recorre el juego desde la pantalla de inicio hasta el Nivel 1 terminado, de modo que la ruta principal completa (Golden Path) existe sobre un nivel real antes de replicar el andamiaje pedagógico en los otros dos.

Cada incremento cuenta con dos documentos: un plan técnico, con el alcance, el grafo de dependencias entre tareas y la descripción de cada una, y un tablero de tareas, con casillas verificables agrupadas en fases que terminan en un punto de control. En la misma sesión se comprobó que los cuatro incrementos cubren entre ellos los cuarenta y siete requerimientos funcionales, sin dejar ninguno huérfano ni duplicar la responsabilidad de ninguno, y que las dieciocho historias de usuario tienen incremento asignado. La Tabla 2.2 resume el resultado; en el Slice 1, la fecha inicial de ejecución es la del cierre de la fase de cimientos y de su commit, el 3 de septiembre, aunque la D02 sitúa el arranque de esa fase en la tarjeta de inicialización comprometida el 30 de agosto.

**Tabla 2.2.** Incrementos del desarrollo.

| Slice | Alcance | Tareas | Puntos de control | Ejecución |
|---|---|---|---|---|
| 1 | Ruta principal y Nivel 1 «La Oscuridad» | T01 a T27 | A a F | 03/09 – 15/09 |
| 2 | Nivel 2 «La Rueda», en tres fases | W01 a W19 | W-A a W-F | 10/09 – 15/09 |
| 3 | Nivel 3 «El Río» y cierre del juego | R01 a R16 | R-A a R-E | 16/09 – 21/09 |
| 4 | Progreso, informe docente y borrado | P00 a P12 | P-A a P-E | 23/09 – 24/09 |

La D02 fijó un orden estricto: ningún incremento se abriría sin haber cerrado el anterior. El 10 de septiembre, con el primero en su fase del Nivel 1, el orden se revisó tarea por tarea: casi todas las tareas del segundo incremento eran independientes de esa fase, y solo quedaron condicionadas W02, que modificaba los mismos archivos que la tarea T17 del Nivel 1, y W15 a W17, que esperaban código de las tareas T16 y T17, entregado al día siguiente. Se decidió entonces abrir el Slice 2 en paralelo y, desde esa fecha, el trabajo se reparte por módulo de código (assembly) y no por incremento. La razón es arquitectónica: los módulos tienen dependencias en un solo sentido y ningún nivel referencia a otro (véase §3.2), de modo que dos personas que trabajan en módulos distintos no modifican los mismos archivos. Cuando la tarea W09 del Slice 2 tuvo que modificar, el 12 de septiembre, el núcleo (`Game.Core`) y la interfaz (`Game.UI`), lo hizo con prueba y dejó el cruce declarado en el archivo de instrucciones del proyecto para el otro carril (véase Anexo C, §3). La Tabla 2.3 recoge el reparto vigente.

**Tabla 2.3.** Carriles de trabajo y módulos que toca cada uno.

| Carril | Módulos y recursos que toca |
|---|---|
| Slice 1 | `Game.Levels.Fire`, `Game.Core`, escenas del Nivel 1, build portable |
| Slice 2 | `Game.Levels.Wheel`, `Game.Scaffolding` |
| Slice 3 | `Game.Levels.River`, `Game.Core`, `Game.Scaffolding`, escenas del Nivel 3 |
| Slice 4 | `Game.Reporting`, `Game.Core`, `Game.UI` |
| Arte y sonido | Carpetas de arte y audio, `Game.Audio`, `Game.Levels.Fire`, `Game.Levels.Wheel` |

El reparto entre personas se decidió en las actas. Las dos primeras tareas de navegación se comprometieron a ambos estudiantes (D02); Santiago Valdiri García asumió las dos siguientes en la D03 (tarjeta D03-1) y la continuación del primer incremento en la D04 (tarjeta D04-2) —las tareas T12 a T19 del Nivel 1 constan en su cuenta del repositorio—, mientras Santiago Benavides Rey abría el segundo desde el 10 de septiembre, compromiso que el tablero de la D05 registra como tarjeta D04-4. En la D05, Santiago Benavides Rey asumió el cierre del Slice 2 y Santiago Valdiri García el sonido del juego; en la D07, el cierre del Slice 3 quedó a cargo del primero; en la D08, el Slice 4 pasó al segundo y la implementación de los sprites y sonidos restantes, al primero.

Dos líneas de trabajo no estaban previstas como tales en los planes del 30 de agosto. La primera son las fases 5 y 6 del Slice 1, ejecutadas entre el 12 y el 15 de septiembre para rehacer la mecánica del Nivel 1, cuyo apartamiento del guion radicado quedó registrado como INC-47 (véase Anexo B, Fases 5 y 6). La segunda es la organización del arte y el sonido como un carril propio: los planes incluían el módulo de audio y el procesamiento de los recursos dentro de cada incremento, pero no un carril separado que incorpora los recursos entregados —entornos, objetos, animaciones del fuego y del humo, personajes animados y sonido de los niveles— sustituyendo archivos sin rehacer escenas cuando la composición se conserva (véanse Anexos E y F). Ese carril se apoya en las revisiones semanales de los miércoles acordadas en la D01 con la colaboradora contratada, Sofía Valentina Giraldo Segovia, estudiante de animación y diseño, cuya participación se limitó expresamente a la producción y la asesoría gráfica; la autoría del trabajo de grado y las decisiones de diseño pedagógico y de mecánica permanecen en los estudiantes. Las piezas gráficas se siguen, además, en un tablero de arte propio, externo al repositorio.

## 2.3 Seguimiento con actas (D01–D09)

El seguimiento del objetivo quedó registrado en nueve actas de sesión, entre el 2 y el 24 de septiembre de 2026, cuyo texto íntegro se transcribe en el Anexo H, una por página. Todas comparten una estructura: una cabecera que sitúa la sesión en su fase de la metodología Árcade, seguida de asistentes, objetivo, desarrollo del trabajo, una cuarta sección —los resultados de la sesión desde la D03, el calendario de entregas de assets en la D01 y el plan de trabajo acordado en la D02—, decisiones adoptadas, compromisos con el tablero Kanban, puntos pendientes y próxima sesión. Los dos estudiantes asistieron a las nueve sesiones; la colaboradora de assets, a las de los días 2, 9, 15 y 24 de septiembre (D01, D04, D06 y D09). La D01 corresponde a la sesión que se registró originalmente como la tercera de la serie del segundo objetivo, cuyo número conserva en la cabecera; sus tarjetas reaparecen renumeradas en la serie D a partir de la D02. La Tabla 2.4 resume las decisiones principales de cada sesión.

**Tabla 2.4.** Actas de seguimiento del tercer objetivo (texto íntegro en el Anexo H).

| Acta | Fecha | Decisiones principales |
|---|---|---|
| D01 | 02/09/2026 | Vinculación de la colaboradora de assets; entrega de la dirección de arte; revisiones de arte los miércoles |
| D02 | 05/09/2026 | Desarrollo en cuatro incrementos verticales con plan y tablero; cobertura de los 47 RF y las 18 HU verificada; cimientos del Slice 1 verificados |
| D03 | 06/09/2026 | Pruebas contra el editor abierto; selección de perfil como panel; título provisional «Algoritm»; referencia 1920 × 1080 y contraste mínimo 4,5 a 1 |
| D04 | 09/09/2026 | Luz del Nivel 1 con pieza base y máscara; entornos separados por planos a 2048 × 1152; ajustes de interfaz; reparto del Slice 1 |
| D05 | 13/09/2026 | Lo que el texto nombra se ve sobre el cuadro de diálogo; fundidos en los cortes de cámara; cierre del Slice 2 y sonido repartidos |
| D06 | 15/09/2026 | Entornos finales aprobados; criterios de los objetos; mecánica del Nivel 1 en dos momentos aprobada; fusión del Slice 2 |
| D07 | 19/09/2026 | Balsa armada sobre el río en tres partes bloqueantes; escena 3.2 solo tras el primer fallo; ningún sonido de derrota |
| D08 | 23/09/2026 | Cierre del Slice 3 aprobado; el Nivel 2 transcurre del amanecer a la noche con luz; apertura del Slice 4 |
| D09 | 24/09/2026 | Cierre del Slice 4; troncos del Nivel 2 animados en el motor; fogata en el cierre del Nivel 2 |

Las actas y los documentos de resultados se remiten entre sí. Los documentos de cierre de los Slices 2 y 3 incluyen una sección de seguimiento metodológico que traza las actas que gobernaron el trabajo y declara el movimiento del tablero que el documento respalda (véanse Anexo C, §2, y Anexo D, §2); la tarjeta que el del Slice 2 cita para su cierre (D05-1) no figura en el tablero de la D05, cuyo único compromiso sobre ese incremento es la tarjeta D04-4, que lo abre en paralelo al primero. El Slice 4 se ejecutó sin una tarjeta de origen citada, y la suya, la D08-1, se identificó después en una nota fechada (véase Anexo G, §2). La tarjeta D07-1, por ejemplo, quedó cumplida en su parte de código el 21 de septiembre, seis días antes de su fecha límite, y la D08 la declaró finalizada; la D08-1, que abrió el Slice 4, se declaró finalizada en la D09, con el incremento cerrado y fusionado en la rama principal. Los puntos pendientes, por su parte, se arrastran de acta en acta hasta resolverse: la medición del tiempo de carga de escena, pendiente desde la D02, seguía en proceso en la D04, y mientras tanto el requerimiento correspondiente no se dio por verificado.

## 2.4 Entorno y herramientas de desarrollo

Las versiones del motor, del canal de render, del sistema de entrada y del marco de pruebas quedaron fijadas en el contrato de desarrollo del proyecto; las demás herramientas constan en la configuración del repositorio y en los documentos de resultados. La Tabla 2.5 las recoge, verificadas contra el repositorio.

**Tabla 2.5.** Entorno y herramientas de desarrollo.

| Elemento | Herramienta y versión | Uso en el proyecto |
|---|---|---|
| Motor | Unity 6000.5.10f1, plantilla 2D | Escenas, recursos y compilación (CT-01) |
| Canal de render | Universal Render Pipeline 17.6.0 | Render 2D orientado a equipos sin tarjeta gráfica dedicada (CT-02) |
| Lenguaje | C# | Lógica del juego y pruebas |
| Entrada | Input System 1.20.0 | Solo clic y clic sostenido (CT-06, RNF-02) |
| Interfaz | uGUI 2.5.0 | Pantallas, diálogos y personajes por recorte |
| Pruebas | Unity Test Framework 1.7.0 (NUnit) | Suites EditMode y PlayMode |
| Línea de comandos | CLI `unity` | Pruebas por lotes y build portable |
| Editores de código | JetBrains Rider 2026.2 y Visual Studio Code | Edición y ejecución de pruebas |
| Control de versiones | Git y GitHub | Ramas de trabajo y pull requests |
| Documentos fuente | markitdown | Conversión consultable de los .docx radicados |

La ejecución de las pruebas evolucionó con el proyecto. La fase de cimientos se verificó por línea de comandos, con el editor cerrado, y así se declaró en la D02. En la D03 se conectó JetBrains Rider como editor externo del motor, con su extensión de puente, y las pruebas pasaron a ejecutarse contra el editor abierto, con la línea de comandos como respaldo, para las corridas limpias y para generar la build portable. Esa conexión no se sostuvo: entre el 13 y el 19 de septiembre las actas D05 a D07 registran el corredor del entorno de desarrollo sin conectar o intermitente. Los puntos de control E y F del Slice 1, que verificaron la mecánica rehecha del Nivel 1, se corrieron por línea de comandos con el editor cerrado; la corrida del punto de control F, el 15 de septiembre, dio 221 de 221 casos en EditMode y 144 de 153 en PlayMode, y es la que declara la D06. Cuando el puente no estuvo disponible con el editor abierto se empleó un tercer camino, un script temporal dentro del editor que invoca la interfaz de pruebas de Unity. Con él se corrieron, entre otras, la suite del punto de control W-F del Slice 2, el mismo 15 de septiembre —212 de 212 casos en EditMode y 146 de 148 en PlayMode, con las dos restantes en verde tras actualizar su expectativa (véase Anexo C, §6)—, y la corrida completa del 25 de septiembre (véase Anexo G).

El repositorio registra 91 commits entre el 29 de agosto y el 25 de septiembre de 2026, hechos desde las cuentas de los dos estudiantes. El trabajo se hizo en ramas por fase, incremento o carril, integradas en la rama principal mediante solicitudes de integración (pull requests); el Slice 1 se integró en cuatro ramas de fase, y sus fases 5 y 6 entraron con la rama del Slice 2. Se fusionaron doce, desde la de los cimientos del Slice 1 (n.º 49, 6 de septiembre) hasta las del Slice 4 y los personajes animados (n.º 85 y 86, 24 de septiembre). El corte de este documento, el commit `ccf77e6`, está en la rama del carril de arte y sonido, cinco commits por delante de la rama principal (véase Anexo E). El desarrollo se apoyó además en un agente de programación asistida por inteligencia artificial (Claude Code, de Anthropic), que operó sobre el repositorio, el editor y las pruebas bajo la dirección y la revisión de los autores, como consta en el archivo de instrucciones del agente versionado en el repositorio y en los documentos de resultados.

## 2.5 Prácticas de ingeniería

**Prueba antes que código.** El contrato de desarrollo exige escribir la prueba antes que la implementación y verla fallar. El flujo por tarea es planeación, diseño de los casos de prueba, prueba que falla, implementación, y refactorización con eliminación de duplicados; para los defectos, reproducir, diagnosticar y corregir. La práctica se sostiene en una decisión de arquitectura: la lógica de cada nivel —máquinas de estado, validadores, contadores, desbloqueos— es C# plano que se prueba en EditMode sin escena ni fotogramas, y el componente del motor es un adaptador delgado que se prueba en PlayMode junto con el cableado de las escenas (véase §3.1). La suite se organiza en diecisiete módulos de prueba, once de EditMode y seis de PlayMode; su resultado a la fecha de corte se presenta en §9.3.

**Resultados declarados, no supuestos.** Desde la D02 se acordó que toda verificación declara cómo se ejecutó y qué dio, y que ninguna casilla de prueba se marca dando por hecho que la suite pasó. Los documentos de resultados reportan las corridas tal cual, incluidos los casos que no pasan y su causa: en la D06 se declararon nueve casos no verdes, tres de ellos preexistentes, antes de fusionar el Slice 2.

**Trazabilidad en el nombre de la prueba (CT-10).** Cada prueba cita en su nombre el requerimiento que verifica, con el patrón `<Sujeto>_<Requisito>_<QuéHace>`, y una prueba de trazabilidad falla si alguno de los cuarenta y siete RF carece de una que lo nombre; el mecanismo se detalla en §9.1 y §9.4, y la matriz del Anexo A se deriva de esos nombres.

**Criterios pedagógicos y de arquitectura como pruebas.** Los invariantes pedagógicos —ninguna derrota (CP-02) y ninguna cifra de desempeño ante el estudiante (CP-03)— y la regla de que ningún nivel referencia a otro (RNF-16) no se asumen: se fijan con pruebas automáticas (véanse §3.2 y §9.1).

**Registro de inconsistencias.** Los conflictos entre los documentos radicados se registran en un documento propio, con la corrección aplicada a cada uno y un orden de precedencia fijo; al corte, INC-01 a INC-45 están cerrados e INC-46 a INC-54 siguen abiertos (véanse §1.2 y §10.1). Ningún documento radicado se edita desde el código, y modificar un RF, un RNF o un criterio de aceptación exige consulta previa.

**Contenido fuera del código (CT-05).** Todo texto visible y todo parámetro ajustable durante el juego reside en recursos de datos del motor y no en las clases (CT-05, RNF-18), de modo que encadenar escenas narrativas, cambiar un mensaje del guía o ajustar un parámetro de nivel es editar un recurso y no el programa (véase §3.4).

**Commits asociados al tablero (CT-11).** CT-11 y RNF-17 piden que cada cambio quede en el repositorio asociado a su tarea del tablero. De los 65 commits sin fusión registrados entre el 1 y el 25 de septiembre, 44 citan la tarea del tablero del incremento (T05, W01, R09–R12) o la tarjeta del acta (D05-2, D07-2, D08-2), 42 en el asunto y dos solo en el cuerpo del mensaje, y muchos citan además los requerimientos que atienden. El cumplimiento es parcial. De los veintiún restantes, cinco incorporan sonidos, uno ajusta la configuración del editor, seis corresponden a la estructura inicial, cierres de fase, documentación o la decisión de planeación del 10 de septiembre, y tres son correcciones; los seis restantes son funcionalidades sin tarjeta: los personajes animados (`88fe0ee`), el rediseño del menú de pausa (`d81cfc7`), los clips de animación del fuego (`dc51804`) y tres de los más recientes (`b38c00b`, `1d5ce58` y `d3a6cc9`), que dejaron sin completar el identificador de la tarjeta.

**Documentos de resultados.** Cada fase o incremento cuenta con un documento de resultados que registra qué se construyó, con qué se verificó y qué quedó abierto; el de las fases 5 y 6 del Slice 1 se escribió a posteriori, el 25 de septiembre. Esos documentos no se reescriben: cuando una afirmación deja de ser cierta se le añade una nota fechada y, en los afectados por cambios posteriores, un anexo recoge lo que cambió después del cierre, verificado contra el código del corte (`ccf77e6`). Son los que este documento incorpora como Anexos B a G.
