# 9. VERIFICACIÓN DEL PROTOTIPO

La fase de desarrollo de la metodología incluye, entre sus actividades, la realización de pruebas técnicas continuas durante el desarrollo. En el prototipo esa actividad tomó la forma de una suite de pruebas automatizadas escrita junto al código con Unity Test Framework 1.7.0, ejecutada al cierre de cada fase y de cada slice y registrada en los documentos de resultados (Anexos B a G). Este capítulo presenta la estrategia que siguió esa suite, las pruebas que recorren la ruta principal del juego, el resultado de la corrida completa del 25/09/2026 sobre el commit `ccf77e6` y la trazabilidad de los requerimientos funcionales a las pruebas. Las verificaciones que exigen el ejecutable o una persona jugando no se resuelven con la suite; se recogen en el capítulo 10 y pasan al objetivo específico 4.

## 9.1 Estrategia de pruebas

La estrategia quedó fijada en la especificación técnica del proyecto y distingue dos niveles automatizados y uno manual:

- **Pruebas EditMode.** Verifican la lógica escrita en C# plano, sin escena y sin cuadros de animación: la máquina de estados del flujo del juego, los validadores de secuencia de la carretilla, del editor de bloques y de la balsa, los contadores, los desbloqueos condicionales, la política de pistas, la selección de mensajes narrativos y la serialización del perfil. Son posibles porque la arquitectura deja la lógica en clases independientes del motor y reduce los componentes de Unity a adaptadores delgados (véase §3.1).
- **Pruebas PlayMode.** Cargan las escenas reales del juego y comprueban el cableado de cada escena, el flujo de la interfaz, el guardado y la recarga, el desbloqueo de niveles y los recorridos completos de cada nivel. Incluyen aserciones de disposición (cada elemento dentro de la pantalla, sin solapamientos, texto sin desbordar, botón alcanzable por el clic) y pruebas de verificación visual que guardan una captura cuando la propiedad no admite una aserción estricta: contraste (RNF-20), doble indicador de color y forma (RNF-19) y ausencia de destellos (RNF-21).
- **Pruebas manuales.** Los presupuestos de rendimiento sobre el ejecutable, la ejecución portable en dos equipos, la ejecución sin red y el cierre forzado con recuperación quedan para el plan de pruebas del objetivo específico 4.

La suite se organiza con un módulo de pruebas por módulo de código, en carpetas separadas para cada modo. Dos módulos son excepcionales por diseño. `Game.Architecture.Tests` no referencia ningún módulo del juego: lee del disco las definiciones de los assemblies (`.asmdef`) y verifica que cada módulo declare sus assemblies de código y de pruebas con un nombre que corresponde a su ruta (RNF-15) y que las dependencias entre módulos respeten sus reglas, entre ellas que ningún nivel referencie a otro (RNF-16); verifica además la ausencia de la clase de entrada heredada (RNF-02), las reglas de importación del arte (RNF-23) y del audio (RNF-06) y la regla de trazabilidad (CT-10). `Game.Content.Tests` es el único que ve los tres niveles a la vez, porque recorre todo el contenido que lee el estudiante en busca de cifras de desempeño (CP-03). En el commit `ccf77e6` la suite suma 17 módulos de prueba, 90 clases, 567 métodos y 667 casos; los casos resultan de expandir los métodos parametrizados.

**Tabla 9.1.** Módulos de prueba del prototipo con su número de métodos y de casos, según el conteo estático sobre el commit `ccf77e6` (Anexo G, «Corrida completa de la suite»).

| Módulo de pruebas | Modo | Métodos | Casos |
|--------------------------|----------|----|----|
| `Game.Architecture.Tests` | EditMode | 15 | 15 |
| `Game.Audio.Tests` | EditMode | 0 | 0 |
| `Game.Content.Tests` | EditMode | 1 | 1 |
| `Game.Core.Tests` | EditMode | 54 | 60 |
| `Game.EditorTools.Tests` | EditMode | 5 | 5 |
| `Game.Levels.Fire.Tests` | EditMode | 34 | 49 |
| `Game.Levels.River.Tests` | EditMode | 41 | 41 |
| `Game.Levels.Wheel.Tests` | EditMode | 69 | 69 |
| `Game.Reporting.Tests` | EditMode | 9 | 14 |
| `Game.Scaffolding.Tests` | EditMode | 68 | 88 |
| `Game.UI.Tests` | EditMode | 19 | 19 |
| `Game.Audio.PlayMode.Tests` | PlayMode | 4 | 4 |
| `Game.Core.PlayMode.Tests` | PlayMode | 11 | 11 |
| `Game.Levels.Fire.PlayMode.Tests` | PlayMode | 38 | 39 |
| `Game.Levels.River.PlayMode.Tests` | PlayMode | 42 | 44 |
| `Game.Levels.Wheel.PlayMode.Tests` | PlayMode | 88 | 89 |
| `Game.UI.PlayMode.Tests` | PlayMode | 69 | 119 |
| **Total EditMode (11 módulos)** | | **315** | **361** |
| **Total PlayMode (6 módulos)** | | **252** | **306** |

El módulo `Game.Audio.Tests` existe sin pruebas propias: el sonido se verifica en PlayMode y en las reglas de importación del módulo de arquitectura.

**Nombre de las pruebas y trazabilidad (CT-10).** El nombre de cada método sigue el patrón `<Sujeto>_<Requisito>_<QuéHace>`, redactado en español, de modo que el identificador del medio traza la prueba hasta el requerimiento que verifica; por ejemplo, `GameFlow_CP02_NoExisteEstadoDeDerrota` verifica que la máquina de estados no tiene ningún estado de derrota. De los 567 métodos, 314 citan un requerimiento funcional; los demás citan requerimientos no funcionales (128), criterios pedagógicos (36), secciones de la dirección de arte (33), hallazgos de consistencia (19), historias de usuario (15), casos de uso (8), secciones del guion (6), restricciones técnicas (3), la tabla de indicadores de OE1 (3) y un punto abierto del guion (1). El método restante es parametrizado y lleva el identificador en el nombre de cada uno de sus siete casos (RF-05 y RF-07). Así, los invariantes pedagógicos no se asumen: los tres niveles tienen pruebas que comprueban que no hay pantalla de derrota ni límite de intentos (CP-02), que ninguna cifra llega al estudiante (CP-03) y que las pistas orientan sin nombrar la respuesta (CP-06). Que lo aprobado no se pierde se comprueba en el perfil del jugador, común a los tres niveles (RF-41), y en el Nivel 3, donde las fases aprobadas y las tareas cumplidas sobreviven a una prueba fallida de la balsa (RF-41, RF-43).

**Pruebas continuas.** Conforme a la práctica de escribir la prueba antes que el código (véase §2.5), cada fase o slice declaró la corrida con que se cerró. La Tabla 9.2 resume esas corridas. Como los slices avanzaron en paralelo por carriles separados (véase §2.2), cada fila corresponde a la rama de su carril en esa fecha, por lo que las cifras no forman una serie estrictamente creciente.

**Tabla 9.2.** Corridas de la suite declaradas al cierre de cada fase o slice.

| Fecha | Corte | EditMode | PlayMode | Anexo |
|----------|------------------|----------------|--------------------------|----|
| 06/09/2026 | Slice 1, fase 0 | 23 de 23 | 4 de 4 | B |
| 07/09/2026 | Slice 1, fase 1 | 34 de 34 | 23 de 23 | C |
| 08/09/2026 | Slice 1, fase 2 | 57 de 57 | 33 de 35 (2 omitidas) | D |
| 11/09/2026 | Slice 1, fase 3 | 94 de 94 | 54 de 54 | E |
| 15/09/2026 | Slice 2 | 212 de 212 | 146 de 148 (2 expectativas actualizadas) | G |
| 15/09/2026 | Slice 1, fase 6 | 221 de 221 | 144 de 153 (6 omitidas, 3 de disposición) | F |
| 21/09/2026 | Slice 3 | 282 de 282 | 165 de 184 (6 omitidas, 13 de disposición) | H |
| 24/09/2026 | Slice 4 | 321 superadas, 1 omitida | Dirigida a interfaz: 23 de 25 (2 sin concluir) | K |
| 25/09/2026 | Suite completa | 360 de 361 (1 omitida) | 304 de 306 | K |

Las omitidas de las filas de las fases 2 y 6 del Slice 1 y del Slice 3 son pruebas de verificación visual, que no se ejecutan sin la vista de juego del editor. Los casos «de disposición» de esas mismas filas son pruebas de colocación o de captura que fallan en la resolución de 640×480 con que el modo sin interfaz abre la vista de juego y pasan con el editor abierto a 1920×1080: el origen es el entorno de ejecución, no el código. En la corrida completa del 25/09/2026, hecha con el editor abierto, no aparecieron. En la fila del Slice 2, los dos casos restantes eran expectativas que el propio slice había invalidado, y pasaron al actualizarlas (Anexo C, §6). En la fila del 24/09 la corrida PlayMode se limitó al módulo de interfaz que tocó el Slice 4; sus dos casos no superados no fallaron, sino que quedaron sin concluir, son anteriores al slice y corresponden a la convergencia del Nivel 1 en las pruebas del resumen de fin de nivel.

## 9.2 Golden Path

El indicador «Índice de Niveles Jugables» del trabajo de grado exige que la ruta principal del jugador (Golden Path) esté libre de obstáculos que impidan terminar la experiencia educativa. OE1 lo concreta en RNF-13, que pide que la ruta principal de cada nivel pueda completarse de principio a fin sin bloqueos, cierres inesperados ni estados irrecuperables, y lo verifica con dos recorridos completos por nivel sin incidencias bloqueantes; la especificación del proyecto adopta ese mismo criterio de éxito. Ese recorrido se automatizó por tramos. Las pruebas de recorrido comprueban el encadenamiento de las escenas, la lógica de cada mecánica y lo que queda registrado en el perfil, pero no reproducen toda la interacción del estudiante. Crean y seleccionan el perfil desde el código, sin usar la pantalla de perfiles, y piden la primera escena del nivel directamente a la máquina de estados, sin elegirla en el menú de niveles. Recorren cada escena narrativa pulsando «Continuar» línea por línea con un clic simulado, sin saltarse ninguna (CP-07). Las mecánicas, en cambio, no se juegan con el puntero: la prueba invoca directamente las operaciones de los controladores y de los botones de cada escena —tomar, arrastrar y soltar piezas, añadir bloques y ejecutar la secuencia, desplazar a Mamá hasta cada material— y, en el Nivel 1, fija el valor de los deslizantes antes de simular el clic en «Golpear» y «Soplar». Las pruebas que recorren el Golden Path, tramo por tramo, son las siguientes:

- **Flujo de estados** (EditMode). `GameFlow_RNF13_RecorreElGoldenPathCompletoSinEstadoIrrecuperable` recorre en la máquina de estados el arranque, el menú, el perfil, la selección de nivel, la narrativa, el juego, el resumen y el regreso a la selección, sin quedar en un estado sin salida.
- **Arranque.** `BootFlow_RF01_ArrancaEnBootYLlegaSoloAMainMenu` comprueba que el juego arranca en la escena de arranque y llega al menú principal.
- **Nivel 1, apertura.** `NarrativeScene_RF10_LaAperturaEncadenaLasTresEscenasDelNivel1YEntraAJugar` carga la escena narrativa con un perfil preparado por la prueba, lee enteras y en orden las tres escenas iniciales y comprueba que desembocan en la fase jugable de la cueva.
- **Nivel 1, cierre.** `LevelSummary_RF03_DevuelveAlMenuConNivel2Desbloqueado` carga la cueva con un perfil preparado por la prueba, enciende el fuego, lee entera la escena de cierre y comprueba el resumen sin cifras, el regreso al menú con el Nivel 2 desbloqueado, la fase confirmada y la llamada al guardado.
- **Nivel 2 completo** (dos casos). `WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado` parte de la escena de arranque y recorre el puente narrativo, el bosque, el taller, el laberinto y el cierre hasta el menú con el Nivel 3 desbloqueado; se ejecuta dos veces.
- **Nivel 3 completo** (dos casos). `RiverLevel_RNF13_RecorreElNivel3CompletoHastaElInicio` parte de la escena de arranque, una vez acertando la prueba de la balsa al primer intento y otra fallándola, y recorre el puente narrativo, la recolección, el ensamblaje, el cruce, el resumen, la escena final y los créditos hasta el menú principal.
- **Final del juego.** `GameEnding_INC39_RecorreLevelSummaryNarrativeCreditsYMainMenu` comprueba que tras el cruce vienen el resumen del Nivel 3, la escena final, los créditos y el menú principal, en ese orden.
- **Cierre forzado, Nivel 3** (dos casos). `RiverLevel_RNF14_CierreForzadoTrasCadaFaseConfirmadaRetomaDondeIba` confirma y guarda desde el código la base, o la base y el amarre, simula el cierre del juego, lo arranca de nuevo desde la escena de arranque y comprueba que el nivel retoma en la fase pendiente.

Salvo la primera, todas son pruebas PlayMode sobre las escenas reales. Los recorridos del Nivel 2 y del Nivel 3 son dos por nivel, como pide el criterio de verificación de RNF-13. El del Nivel 3 cubre además el camino con error: la balsa mal armada se hunde, se narra la escena 3.2, las fases ya aprobadas se conservan (RF-41, RF-43) y el estudiante retoma la fase que falló. Al terminar, los recorridos del Nivel 2 y del Nivel 3 leen de nuevo el perfil guardado y verifican que el nivel quedó completo, que el siguiente quedó desbloqueado cuando lo hay y que los indicadores de desempeño de las fases se registraron (RF-04, RF-45); el tramo de cierre del Nivel 1 verifica el desbloqueo del Nivel 2, la fase confirmada en el perfil y la llamada al guardado, no el archivo en disco. El Nivel 1 se cubre en dos tramos, apertura y cierre, que se ejecutan una vez cada uno y cargan directamente su escena, sin partir de la escena de arranque como los otros dos. La prueba de cierre forzado con reinicio desde la escena de arranque cubre solo el Nivel 3; en el Nivel 2 se prueba la retoma al entrar al nivel con fases ya confirmadas, y la comprobación del cierre forzado del Nivel 1 (RNF-14) sigue abierta en el tablero del Slice 1.

Todas estas pruebas pasaron en la corrida completa del 25/09/2026 (véase §9.3). Con ello, el indicador «Índice de Niveles Jugables» queda cumplido en su verificación automatizada para los Niveles 2 y 3, cada uno con dos recorridos completos sin bloqueos. El Nivel 1 queda cubierto por tramos, ejecutados una vez cada uno, lo que no alcanza los dos recorridos completos que pide RNF-13. La comprobación manual complementaria, que consiste en recorrer el juego entero dos veces con un cronómetro entre 20 y 40 minutos, sigue abierta en los tableros de los slices y pasa al objetivo específico 4 (véase §10.5).

## 9.3 Resultado de la suite completa (25/09/2026)

La corrida completa se hizo el 25/09/2026 sobre el commit `ccf77e6`, con el árbol de trabajo limpio, y fue la primera tras el trabajo de arte, sonido y personajes de los días 24 y 25. Se ejecutó dentro del editor de Unity abierto mediante la interfaz programática del Test Framework, con un script temporal que registró los resultados en un archivo y que se retiró al terminar, porque la conexión con el entorno de desarrollo Rider no estaba disponible. Las dos corridas, EditMode y PlayMode, se hicieron una tras otra, y el número de casos que ejecutó el corredor coincide con el conteo estático de la Tabla 9.1. Entre el cierre del Slice 4 (commit `9c34924`) y `ccf77e6` la suite creció en 39 casos EditMode y 81 casos PlayMode, aportados por los commits de personajes, troncos, humo, caja y cuerda, y sonido del Nivel 3. La Tabla 9.3 resume el resultado.

**Tabla 9.3.** Resultado de la corrida completa del 25/09/2026 sobre el commit `ccf77e6` (Anexo G).

| Modo | Casos | Superan | Fallan | Omitidas |
|----------|----|----|----|----|
| EditMode | 361 | 360 | 0 | 1 |
| PlayMode | 306 | 304 | 2 | 0 |
| **Total** | **667** | **664** | **2** | **1** |

La corrida EditMode duró 9,8 s y la PlayMode 1 695 s (28 minutos). Los tres casos que no superaron la corrida se explican así:

- **Omitida: `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`.** La prueba se omite sola cuando el equipo no hace cumplir el atributo de solo lectura sobre carpetas, como ocurre en el equipo donde se corrió; el escenario de carpeta no escribible de INC-34 no se puede simular ahí. El borrado de perfiles en sí está cubierto por las demás pruebas de RF-47.
- **Falla: `RiverLevel_RNF05_LaMemoriaQuedaBajoDosGigasConElNivel3Cargado`.** Registró 2 831 MB de memoria reservada, por encima del límite de 2 GB de RNF-05. La medida no corresponde al nivel sino a la sesión del editor: ese mismo día el editor en modo de edición, sin el nivel cargado, ya reservaba 2 652 MB. La misma prueba midió 1 226 MB el 21/09/2026 (Anexo D). El presupuesto de memoria debe medirse sobre el ejecutable (véanse §8.1 y §10.2).
- **Falla: `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`.** Durante el hundimiento de la balsa un cuadro avanzó 0,076 del alto de la balsa, frente a un límite de 0,06. Ejecutada sola inmediatamente después, la prueba pasa, y también había pasado en las corridas previas del mismo día. La animación de hundimiento no cambió en `ccf77e6` y avanza según el tiempo transcurrido entre cuadros, de modo que un cuadro largo del editor, a los 28 minutos de corrida, basta para superar el umbral. Se registra como prueba abierta y sensible al tiempo (véase §10.4).

En síntesis, 664 de los 667 casos superan la corrida. Ninguno de los dos fallos corresponde a un comportamiento funcional de un RF: uno es un presupuesto de rendimiento medido en un entorno que no le corresponde y el otro es la sensibilidad temporal de una prueba visual.

## 9.4 Trazabilidad de requerimientos a pruebas

La regla CT-10 exige que todo requerimiento funcional tenga al menos una prueba que lo nombre. Como el identificador va en el nombre del método, la matriz de trazabilidad no se mantiene a mano: se deriva del código de pruebas. El Anexo A presenta esa matriz, generada de forma automática sobre el commit `ccf77e6`, con el número de pruebas de cada RF, los módulos donde viven y un ejemplo de prueba. La misma condición la verifica en cada corrida la prueba `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra`, que pasó el 25/09/2026.

Los 47 requerimientos funcionales, RF-01 a RF-47, tienen al menos una prueba que los nombra. Los 314 métodos que citan un RF se reparten en 187 EditMode y 127 PlayMode. De los 47 RF, 33 se verifican en los dos modos, es decir, en su lógica y en la escena real; 9 solo en EditMode y 5 solo en PlayMode. La Tabla 9.4 agrupa la cobertura por módulo funcional de OE1.

**Tabla 9.4.** Pruebas que nombran un RF, por módulo funcional de OE1 (Anexo A).

| Módulo de OE1 | Requerimientos | Métodos | EditMode | PlayMode |
|--------------------|------------|----|----|----|
| A. Sistema y navegación | RF-01 a RF-09 | 106 | 68 | 38 |
| B. Andamiaje pedagógico | RF-10 a RF-13 | 19 | 11 | 8 |
| C. Nivel 1 «La Oscuridad» | RF-14 a RF-21 | 41 | 21 | 20 |
| D. Nivel 2 «La Rueda» | RF-22 a RF-34 | 79 | 39 | 40 |
| E. Nivel 3 «El Río» | RF-35 a RF-44 | 35 | 22 | 13 |
| F. Progreso y registro | RF-45 a RF-47 | 34 | 26 | 8 |
| **Total** | **47 RF** | **314** | **187** | **127** |

La matriz cuenta solo las pruebas que citan el RF en su nombre, así que subestima la cobertura real. Tres requerimientos tienen una sola prueba con su identificador: RF-21 (iluminación progresiva, de prioridad baja), RF-27 (área de trabajo de construcción) y RF-42 (prueba de la balsa y depuración). Los recorridos de §9.2 los ejercitan además sin nombrarlos; el del Nivel 3, por ejemplo, hace fallar la prueba de la balsa, comprueba que se narra la escena 3.2 y que las fases aprobadas se conservan, y retoma el ensamblaje hasta el cruce. En sentido inverso, algunas pruebas conservan en su nombre el RF radicado aunque la implementación se aparte de su redacción: las de la mecánica del Nivel 1 citan RF-15 y RF-16 (INC-47), las del taller citan RF-27 y RF-29 pese a la cuerda añadida (INC-54) y las del rodado citan RF-26 (INC-50). La matriz traza entonces al requerimiento tal como fue radicado, y cada diferencia queda registrada en su hallazgo de consistencia (véase §10.1). Esta matriz es el punto de partida de la matriz de trazabilidad que el cronograma asigna al objetivo específico 4.
