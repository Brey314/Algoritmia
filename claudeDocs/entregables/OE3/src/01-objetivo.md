# 1. OBJETIVO, ALCANCE Y CRITERIOS DE CUMPLIMIENTO

Este documento constituye el entregable del tercer objetivo específico del trabajo de grado. Continúa los entregables radicados del primer objetivo (especificación de requerimientos, OE1) y del segundo (diseño pedagógico, narrativo y técnico, OE2), y antecede a la evaluación del prototipo, que corresponde al cuarto objetivo (OE4). Describe el prototipo en el corte del 1 de octubre de 2026, que es el árbol de trabajo de ese día en la rama `feat/cierre-de-slices-y-oe3`, abierta desde el commit `359365e`; la huella del árbol con que se compiló el ejecutable figura en el apartado 8.3. Expone qué se construyó, cómo se construyó y con qué evidencia se sostiene que cumple los indicadores fijados para este objetivo. Las cifras de verificación del corte salen de la corrida completa de la suite y de las mediciones sobre el ejecutable compilado desde él; las de fechas anteriores se citan como historia del desarrollo.

## 1.1 El objetivo específico 3

El trabajo de grado formula el tercer objetivo específico en los siguientes términos:

> Desarrollar el prototipo funcional del videojuego incorporando niveles y desafíos progresivos centrados en la resolución de problemas lógicos.

El objetivo se subordina al objetivo general: «Desarrollar un prototipo de videojuego educativo como herramienta tecnológica didáctica para incorporar las facetas del pensamiento computacional en estudiantes de grado cuarto de primaria del colegio El Libertador IED». Los dos primeros objetivos definieron qué debía hacer el videojuego y cómo debía organizarse. Este lo convierte en un producto ejecutable, y el cuarto lo somete a pruebas funcionales.

El trabajo de grado ordena el proceso en cuatro fases: definición de lineamientos y requerimientos (OE1), diseño del videojuego educativo (OE2), desarrollo del prototipo (OE3) y evaluación (OE4). La gestión sigue la metodología Árcade, apoyada en Kanban, y dentro de Árcade este objetivo ocupa el ciclo Diseño ↔ Desarrollo ↔ Pruebas. Las diez actas de seguimiento (D01 a D10, del 02/09/2026 al 30/09/2026) registran el recorrido: la D01 se ubica todavía en la fase de diseño de Árcade, la D02 planea el ciclo y las ocho restantes, hasta la sesión de cierre del 30 de septiembre, documentan su ejecución (apartados 2.1 y 2.3). En ese ciclo el diseño radicado en OE2 se implementó por incrementos. Cada incremento se verificó con pruebas automatizadas, y cuando la implementación chocó con lo diseñado, el conflicto quedó registrado en el documento de inconsistencias con la corrección que lo resolvió.

Para la fase de desarrollo, el trabajo de grado fija siete actividades, desde la implementación de la estructura base del videojuego hasta la optimización de su rendimiento. La Tabla 2.1 (apartado 2.1) señala dónde se ejecutó cada una y qué capítulo de este documento la reporta.

## 1.2 Alcance del prototipo entregado

El prototipo se presenta en su pantalla de inicio con el título «Algoritmia». El acta D03 lo había fijado como «Algoritm», el mismo nombre del guía; el juego cambió a «Algoritmia» el 09/09/2026 y el guion lo recoge desde el 29/09/2026, fecha en que se cerró el punto PG-01 (Anexo B, apartado 3). Es una aventura narrativa en 2D para un jugador. Una familia prehistórica avanza hacia la civilización resolviendo tres problemas de supervivencia (la oscuridad, el transporte y el río) con la compañía de Algoritm, que en el Nivel 1 es una llama con brazos y piernas y en los Niveles 2 y 3 aparece con el mismo cuerpo recoloreado en madera, como rueda, y en agua, como gota (INC-45, INC-52). El guía pregunta y descompone el objetivo, pero deja la solución al estudiante (CP-06). Cada nivel ejercita una faceta principal del pensamiento computacional, según la estructura de niveles del guion (Solución OE2, apartado 1.1.2), y se divide en fases jugables separadas por escenas narrativas.

**Tabla 1.1.** Niveles del prototipo, fases jugables y escenas que los implementan.

| Nivel | Faceta principal | Fases jugables | Escenas jugables |
|---|---|---|---|
| 1. La Oscuridad | Iteración y depuración | 1 (encendido del fuego) | `Level1_Cave` |
| 2. La Rueda | Abstracción y pensamiento algorítmico | 3 (bosque, taller, laberinto) | `Level2_Forest`, `Level2_Workshop`, `Level2_Maze` |
| 3. El Río | Descomposición y depuración | 3 (base, amarre, mástil y vela) | `Level3_River` |

El código fija el número de fases por nivel (`PhaseId.PhasesPerLevel = { 1, 3, 3 }`), siete en total, y cada una se guarda en el perfil al confirmarse. En el Nivel 3 la recolección de materiales precede al ensamblaje sin ser una fase guardada: al retomar el nivel se da por cumplida (Anexo D, apartado 2.1).

El ejecutable contiene doce escenas, registradas en la configuración de compilación: las cinco jugables de la Tabla 1.1 y siete de flujo (`Boot`, `MainMenu`, `LevelSelect`, `Credits`, `Narrative`, `LevelSummary` y `TeacherReport`). Junto a ellas, el prototipo entrega estos componentes:

- **Escenas narrativas.** Las quince escenas narrativas del guion se reproducen en una única escena reutilizable a partir de dieciocho secuencias de contenido: cuatro del Nivel 1, siete del Nivel 2 y siete del Nivel 3. El número de secuencias supera al de escenas porque cada secuencia tiene una sola ilustración, y una escena que cambia de fondo se reparte en dos o tres secuencias encadenadas (apartado 4.5).
- **Personajes.** Siete personajes animados: Papá, Mamá, la Niña, el Niño y las tres formas de Algoritm, que deja a su paso una estela de puntos de luz. Intervienen en las dieciocho secuencias narrativas y en las cinco escenas jugables (apartado 7.2 y Anexo F).
- **Perfiles.** Cada estudiante tiene un perfil identificado únicamente por un nombre o alias, guardado como archivo en la carpeta `Datos/`, junto al ejecutable. El perfil almacena el nivel alcanzado, las fases confirmadas y los cuatro indicadores de desempeño definidos en OE1, apartado 3.6.1 (intentos, errores corregidos, pasos utilizados y tiempo de resolución), y ningún otro dato (RNF-09).
- **Consulta docente.** La opción «Progreso del equipo» del menú principal abre el informe docente, que lista los perfiles del equipo y presenta, para el que se elige, los indicadores por nivel y por fase (RF-46). Si el guardado tuvo que usar la ruta de respaldo porque `Datos/` no admitía escritura, la pantalla lo advierte (INC-34, INC-77). Desde ella se elimina además un perfil de forma definitiva, tras una confirmación explícita (RF-47).
- **Distribución portable.** El prototipo se entrega como una carpeta ejecutable para Windows de escritorio, que funciona sin instalación (RNF-07) y está diseñada para no depender de internet (RNF-08; apartado 3.6). El ejecutable del corte se compila con los servicios de Unity apagados, y la compilación falla si el juego compilado nombra sus servidores; en los siete lanzamientos medidos no se observó ninguna conexión de red, con lo que cumple RNF-10 en el equipo de desarrollo (apartados 3.6 y 8.3).

Están implementados los 47 requerimientos funcionales de OE1: los 45 de prioridad alta, RF-06, de prioridad media, y RF-21, de prioridad baja. Cada uno tiene al menos una prueba automatizada que lo nombra, condición que vigila la prueba `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` (CT-10; apartado 9.4 y Anexo A). Ninguno de los 103 casos de prueba funcional del OE4 que tienen veredicto termina en fallo: los que fallaron en el primer candidato del ejecutable se repitieron sobre el del corte y pasaron (apartado 9.5).

**Fuera del alcance.** El prototipo no incluye:

- El nivel avanzado opcional que el guion describe en Solución OE2, apartado 1.10. Se excluyó porque no tiene requerimiento asociado y porque introduce presión de tiempo, en tensión con el criterio CP-02.
- La exportación del informe docente a un archivo, las gráficas y la comparación entre estudiantes, que RF-46 no pide (Anexo G, apartado 1).
- Una protección de acceso al informe docente. Ningún requerimiento la pide, y el acta D10 decidió no añadirla.
- Plataformas distintas de Windows de escritorio y entornos tridimensionales, según las restricciones de OE1, apartado 2.4.
- La evaluación funcional formal del prototipo, que corresponde a OE4.

El registro de inconsistencias quedó cerrado: los hallazgos INC-01 a INC-130 (revisión 16, del 01/10/2026) se resolvieron con la regla adoptada el 29/09/2026, y los documentos radicados se corrigieron para describir el prototipo tal como funciona (apartados 2.5 y 10.1).

## 1.3 Indicadores del objetivo y su cumplimiento

El trabajo de grado asocia a este objetivo dos indicadores de desempeño. La Tabla 1.2 los contrasta con lo obtenido.

**Tabla 1.2.** Indicadores del tercer objetivo específico y su cumplimiento.

| Indicador | Meta del trabajo de grado | Resultado | Evidencia |
|---|---|---|---|
| Tasa de implementación de mecánicas de juego | Al menos tres mecánicas principales (semana 11) | Siete implementadas de siete planeadas en el guion | Apartado 5.1; Anexos B (apartado 2.5), C y D |
| Índice de Niveles Jugables (*Golden Path*) | Tres niveles jugables con Golden Path sin bloqueos (semana 12) | Tres niveles sin bloqueos, en tres recorridos completos del juego sobre el ejecutable sin incidencias bloqueantes y en los recorridos automatizados del Editor | Apartados 9.2 y 9.5 |

**Mecánicas.** El guion de OE2 especifica una mecánica para el Nivel 1 (Solución OE2, apartado 1.4.3), una para cada fase del Nivel 2 (apartados 1.6.1 a 1.6.3) y tres para el Nivel 3 (apartados 1.8.2 a 1.8.4). Las siete están implementadas:

- **Nivel 1.** Reunir los materiales en el centro de la cueva y encender el fuego ensayando y ajustando la fuerza del golpe y la cercanía de las piedras.
- **Nivel 2.** Seleccionar objetos por su patrón en el bosque, ensamblar la carretilla en secuencia en el taller y guiarla por el laberinto con un editor de bloques.
- **Nivel 3.** Recolectar materiales moviendo a Mamá con botones en pantalla; ensamblar la balsa en tres fases bloqueantes, que admiten una prueba anticipada, y probar y depurar la balsa terminada.

La meta del indicador se supera en más del doble. Las siete mecánicas reúnen las tres formas de interacción que el indicador da como ejemplo:

- El movimiento del personaje, con el desplazamiento de Mamá por la orilla en el Nivel 3 y el de la carretilla que ejecuta la secuencia de bloques en el Nivel 2.
- La interacción con objetos: arrastrar, seleccionar, ensamblar y recolectar.
- La resolución de retos lógicos, presente en todas las mecánicas.

A ellas se añaden dos formas de interacción comunes a los tres niveles, el diálogo narrativo con el guía y la ayuda contextual, descritas en los apartados 4.5 y 6.1.

**Golden Path.** RNF-13 pide que la ruta principal de cada nivel se complete sin bloqueos, cierres inesperados ni estados irrecuperables, y OE1 lo verifica con dos recorridos completos por nivel sin incidencias bloqueantes. El prototipo lo comprueba en dos planos. Sobre el ejecutable, la ruta principal del juego entero se recorrió tres veces de principio a fin de forma automatizada. Las dos primeras se hicieron sobre el primer candidato del ejecutable, con un perfil nuevo a 1920 × 1080 en 87 min y a pantalla completa en 33 min; la tercera, sobre el ejecutable del corte, con un perfil nuevo a pantalla completa, en 16 min. Ninguna tuvo incidencias bloqueantes, y con ellas el criterio de RNF-13 se cumple en los tres niveles, el Nivel 1 incluido. Los dos defectos no bloqueantes del laberinto que vieron las dos primeras están corregidos en el ejecutable del corte: la fila arrastrada de «Tu secuencia» se aparta de la lista hasta soltarla, y la lista se desplaza sola hasta el bloque en curso (apartado 9.5).

En el Editor, una prueba recorre la máquina de estados completa. El Nivel 2 tiene dos recorridos desde el arranque del juego con un perfil real, que desde el 29/09/2026 exigen leer la escena de cierre 2.5 antes de que se desbloquee el Nivel 3. El Nivel 3 tiene otros dos: uno acierta la prueba de la balsa al primer intento y el otro la falla, ve la escena condicional y corrige. El Nivel 1 se verifica en dos tramos consecutivos, de la apertura narrativa al resumen. La retoma tras un cierre forzado (RNF-14) se probó en el Editor sobre el Nivel 3 y, sobre el ejecutable, en nueve momentos. En los siete del primer candidato el perfil reabrió sin error y el juego retomó en la primera fase pendiente, con las fases confirmadas y sus indicadores intactos; los dos del ejecutable del corte se forzaron en el instante mismo del guardado, y en ninguno quedó un perfil truncado. La duración de una partida completa con estudiantes, prevista entre 20 y 40 minutos, la cronometra Santiago Benavides Rey en la evaluación del OE4. El detalle, prueba por prueba, está en los apartados 9.2 y 9.5.

La corrida completa de la suite del 01/10/2026, la última antes de compilar el ejecutable del corte, superó 845 de 846 casos, sin fallos y con una omisión por entorno (apartado 9.3).

## 1.4 Entregables del cronograma

El cronograma del trabajo de grado asigna al tercer objetivo seis entregables. La Tabla 1.3 resume en qué consiste cada uno en el prototipo y dónde se desarrolla.

**Tabla 1.3.** Entregables del cronograma del tercer objetivo.

| Entregable del cronograma | Qué se entrega | Sección |
|---|---|---|
| Entorno configurado | Unity 6 con plantilla 2D y URP, pruebas automatizadas y repositorio git | Apartado 2.4 |
| Build inicial | Estructura base y primer ejecutable portable (08/09/2026) | Capítulo 3 |
| Mecánicas implementadas | Las siete mecánicas principales de los tres niveles | Capítulo 5 |
| Niveles funcionales | Tres niveles con siete fases jugables y dieciocho secuencias narrativas | Capítulo 4 |
| Interfaz integrada | Pantallas del flujo, escenarios, personajes animados y sonido | Capítulo 7 |
| Prototipo optimizado | Optimizaciones aplicadas y presupuestos medidos sobre el ejecutable de doce escenas: peor carga 0,78 s, memoria de trabajo máxima 446 MB (1 179 MB de memoria privada) y paquete de 479,0 MB; el segundo equipo y CT-02 se verifican en el OE4 | Capítulo 8 |

## 1.5 Organización del documento y anexos

El documento se organiza en doce capítulos. Después de este vienen:

- **Capítulo 2.** Metodología del desarrollo: Árcade y Kanban, planificación por incrementos (*slices*) y carriles de trabajo, seguimiento con las actas D01 a D10, entorno de desarrollo y prácticas de ingeniería.
- **Capítulo 3.** Arquitectura implementada frente al primer diseño, módulos y dependencias, flujo de estados y escenas, contenido parametrizable, persistencia de perfiles y distribución portable.
- **Capítulo 4.** Los tres niveles y sus desafíos progresivos: qué juega el estudiante, qué problema lógico plantea cada reto y a qué faceta del pensamiento computacional responde; escenas narrativas y síntesis de la progresión de dificultad.
- **Capítulo 5.** Mecánicas planeadas frente a mecánicas implementadas, formas de interacción y control, y ajustes al diseño introducidos durante el desarrollo.
- **Capítulo 6.** Andamiaje pedagógico: el guía y las pistas, la retroalimentación sin derrota ni puntajes, el cierre reflexivo de cada nivel, los indicadores de desempeño y el informe docente.
- **Capítulo 7.** Interfaz, escenarios, personajes, animación, sonido y accesibilidad.
- **Capítulo 8.** Presupuestos de rendimiento medidos sobre el ejecutable, mediciones registradas durante el desarrollo y optimizaciones aplicadas.
- **Capítulo 9.** Estrategia de pruebas, verificación del Golden Path, resultado de la suite completa del 01/10/2026, trazabilidad de requerimientos a pruebas y verificaciones sobre el ejecutable.
- **Capítulo 10.** Limitaciones, puntos abiertos al corte y verificaciones que pasan al cuarto objetivo.
- **Capítulo 11.** Conclusiones.
- **Capítulo 12.** Control de cambios del documento.

Los anexos reúnen la evidencia de detalle y se entregan como documentos independientes, uno por anexo, junto a este documento principal. Cada uno indica en su primera página el entregable al que pertenece, y la Tabla 1.4 da el nombre de su archivo. El Anexo A se genera a partir del código de pruebas del corte. Los anexos B a G se redactaron al estado final del prototipo, uno por incremento o carril, a partir de los documentos de resultados de cada uno; el repositorio conserva esos documentos como registro histórico, con las notas fechadas que fueron recibiendo. Las actas D01 a D10 se conservan en Word en el SharePoint del proyecto y este documento las cita por su código y su fecha.

**Tabla 1.4.** Anexos del entregable, cada uno en su propio documento.

| Anexo | Documento | Contenido | Archivo |
|---|---|---|---|
| A | Matriz de trazabilidad | Cada uno de los 47 RF con las pruebas automatizadas que lo nombran en el corte | Solucion_OE3_Anexo_A_Matriz_trazabilidad.docx |
| B | Slice 1: ruta principal y Nivel 1 «La Oscuridad» | Cimientos, navegación, andamiaje mínimo y el Nivel 1 en su forma final: reunir y encender | Solucion_OE3_Anexo_B_Slice_1.docx |
| C | Slice 2: Nivel 2 «La Rueda» | Bosque, taller, laberinto y cierre del nivel | Solucion_OE3_Anexo_C_Slice_2.docx |
| D | Slice 3: Nivel 3 «El Río» y cierre del juego | Recolección, ensamblaje con prueba anticipada, depuración, cruce, escena final y créditos | Solucion_OE3_Anexo_D_Slice_3.docx |
| E | Carril de arte y sonido | Entornos, objetos, efectos e interfaz; reglas de importación; las piezas de sonido y dónde suenan | Solucion_OE3_Anexo_E_Arte_y_sonido.docx |
| F | Personajes animados | La familia y Algoritm por recorte en narrativas y mecánicas, estela, sombra y escala del río | Solucion_OE3_Anexo_F_Personajes.docx |
| G | Slice 4: progreso, informe docente y eliminación de datos | Informe docente, borrado, cierre del proyecto, corridas completas de la suite y mediciones sobre el ejecutable | Solucion_OE3_Anexo_G_Slice_4.docx |
