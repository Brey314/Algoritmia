# 1. OBJETIVO, ALCANCE Y CRITERIOS DE CUMPLIMIENTO

Este documento constituye el entregable del tercer objetivo específico del trabajo de grado. Da continuidad a los entregables radicados del primer objetivo (especificación de requerimientos, OE1) y del segundo (diseño pedagógico, narrativo y técnico, OE2), y precede a la evaluación del prototipo que corresponde al cuarto objetivo (OE4). Describe el prototipo tal como quedó en la versión del repositorio del 25 de septiembre de 2026 (commit `ccf77e6`), qué se construyó, cómo se construyó y con qué evidencia se sostiene que cumple los indicadores fijados para este objetivo. Después de ese corte, el commit `44fd479` (25/09/2026) incorporó los objetos definitivos de los niveles 2 y 3: bajó la resolución de los del Nivel 2 y recompuso el ensamblaje de la balsa del Nivel 3 sobre su nueva ilustración en perspectiva de tres cuartos, con una prueba más. No cambia las mecánicas ni los indicadores que aquí se describen; lo registra el Anexo E, apéndice B, y las figuras del ensamblaje de la balsa (§4.4) muestran el estado anterior.

## 1.1 El objetivo específico 3

El trabajo de grado formula el tercer objetivo específico en los siguientes términos:

> Desarrollar el prototipo funcional del videojuego incorporando niveles y desafíos progresivos centrados en la resolución de problemas lógicos.

El objetivo se subordina al objetivo general: «Desarrollar un prototipo de videojuego educativo como herramienta tecnológica didáctica para incorporar las facetas del pensamiento computacional en estudiantes de grado cuarto de primaria del colegio El Libertador IED». Los dos primeros objetivos definieron qué debía hacer el videojuego y cómo debía estar organizado; este objetivo lo convierte en un producto ejecutable, y el cuarto lo somete a pruebas funcionales.

El trabajo de grado organiza el proceso en cuatro fases: definición de lineamientos y requerimientos (OE1), diseño del videojuego educativo (OE2), desarrollo del prototipo (OE3) y evaluación (OE4). La gestión se apoya en la metodología Árcade, complementada con Kanban. Dentro de Árcade, este objetivo corresponde al ciclo Diseño ↔ Desarrollo ↔ Pruebas. Las nueve actas de seguimiento del objetivo (D01 a D09, del 02/09/2026 al 24/09/2026; Anexo H) registran su recorrido: la primera todavía se ubica en la fase de diseño de Árcade y las ocho siguientes, en el ciclo (véanse §2.1 y §2.3). En ese ciclo, el diseño radicado en OE2 se implementa por incrementos. Cada incremento se verifica con pruebas automatizadas y, cuando la implementación revela un conflicto con lo diseñado, se registra la corrección en el documento de inconsistencias en lugar de resolverla en silencio.

El trabajo de grado asigna a la fase de desarrollo siete actividades, desde la implementación de la estructura base del videojuego hasta la optimización de su rendimiento. La Tabla 2.1 (véase §2.1) indica dónde se ejecutó cada una y en qué capítulo de este documento se da cuenta de ella.

## 1.2 Alcance del prototipo entregado

El prototipo, que se presenta en su pantalla de inicio con el título «Algoritmia» (título provisional: el acta D03 lo había fijado como «Algoritm», igual que el guía, y el 09/09/2026 pasó a «Algoritmia»; el punto PG-01 sigue abierto, véase Anexo B, Fase 1, §A.2), es una aventura narrativa en 2D para un jugador. Una familia prehistórica avanza hacia la civilización resolviendo tres problemas de supervivencia (la oscuridad, el transporte y el río), acompañada por el guía Algoritm, que adopta la forma de fuego, rueda o gota de agua según el nivel (INC-44, INC-45); la rueda y la gota llevan todavía arte provisional (INC-52). El guía pregunta y descompone el objetivo, pero no resuelve el reto (CP-06). Cada nivel ejercita una faceta principal del pensamiento computacional, conforme a la estructura de niveles del guion (OE2 §1.1.2), y se divide en fases jugables separadas por escenas narrativas.

**Tabla 1.1.** Niveles del prototipo, fases jugables y escenas que los implementan.

| Nivel | Faceta principal | Fases jugables | Escenas jugables |
|---|---|---|---|
| 1 — La Oscuridad | Iteración y depuración | 1 (encendido del fuego) | `Level1_Cave` |
| 2 — La Rueda | Abstracción y pensamiento algorítmico | 3 (bosque, taller, laberinto) | `Level2_Forest`, `Level2_Workshop`, `Level2_Maze` |
| 3 — El Río | Descomposición y depuración | 3 (base, amarre, mástil y vela) | `Level3_River` |

El número de fases por nivel está fijado en el código (`PhaseId.PhasesPerLevel = { 1, 3, 3 }`), para un total de siete fases que se guardan en el perfil. En el Nivel 3, la recolección de materiales precede al ensamblaje, pero no se guarda como fase propia: al retomar el nivel se da por hecha (véase Anexo D, §3).

El ejecutable reúne doce escenas, registradas en la configuración de compilación del proyecto: las cinco jugables de la Tabla 1.1 y siete escenas de flujo (`Boot`, `MainMenu`, `LevelSelect`, `Credits`, `Narrative`, `LevelSummary` y `TeacherReport`). Además de las escenas, el prototipo entrega los siguientes componentes:

- **Escenas narrativas.** Las quince escenas narrativas del guion se reproducen en una sola escena reutilizable a partir de dieciocho secuencias de contenido: cuatro del Nivel 1, siete del Nivel 2 y siete del Nivel 3. Hay más secuencias que escenas porque cada secuencia lleva una sola ilustración, de modo que una escena que cambia de fondo a mitad de camino se divide en dos o tres secuencias encadenadas (véase §4.5).
- **Personajes.** Se entregan siete personajes animados: Papá, Mamá, la Niña, el Niño y las tres formas de Algoritm, de las cuales la rueda y la gota tienen arte provisional (INC-52). Aparecen en las dieciocho secuencias narrativas y en las cinco escenas jugables (véase §7.2 y Anexo F).
- **Perfiles.** Cada estudiante tiene un perfil identificado solo por un nombre o alias, que se guarda como archivo en la carpeta `Datos/`, junto al ejecutable. El perfil almacena el nivel alcanzado, las fases confirmadas y los cuatro indicadores de desempeño de OE1 §3.6.1 (intentos, errores corregidos, pasos utilizados y tiempo de resolución), sin ningún otro dato (RNF-09).
- **Consulta docente.** Desde el menú principal, la opción «Progreso del equipo» abre el informe docente, que presenta los valores de los indicadores por nivel y por fase para cada perfil (RF-46). La misma pantalla permite eliminar de forma definitiva un perfil tras una confirmación explícita (RF-47).
- **Distribución portable.** El prototipo se distribuye como una carpeta ejecutable para Windows de escritorio, que no requiere instalación ni conexión a internet (RNF-07, RNF-08; véase §3.6).

Con el cierre del cuarto incremento quedan implementados los 47 requerimientos funcionales de OE1, y no solo los 45 de prioridad alta. Cada uno cuenta con al menos una prueba automatizada que lo nombra, lo que comprueba la prueba `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` (CT-10; véase §9.4 y Anexo A).

**Fuera del alcance.** No forman parte del prototipo:

- El nivel avanzado opcional que describe el guion en OE2 §1.10. Se excluyó porque carece de requerimiento asociado y porque introduce presión de tiempo, lo que entra en tensión con el criterio CP-02.
- La exportación del informe docente a archivo, las gráficas y la comparación entre estudiantes, que RF-46 no pide (véase Anexo G, §1).
- Otras plataformas distintas de Windows de escritorio y los entornos tridimensionales, conforme a las restricciones de OE1 §2.4.
- La evaluación funcional formal del prototipo, que corresponde a OE4.

Al cierre quedan abiertas nueve inconsistencias (INC-46 a INC-54), registradas mientras no se corrijan los documentos fuente. Tres son desviaciones de las mecánicas radicadas, implementadas por decisión del equipo: la mecánica del Nivel 1, que reúne los materiales y luego mide la fuerza del golpe y la cercanía de las piedras (INC-47); la demostración del rodado del Nivel 2, que se narra en lugar de jugarse (INC-50), y la cuerda como séptima pieza del taller (INC-54), analizadas en §5.3. Dos apartan la implementación de las historias de usuario (INC-49 e INC-51), dos se apartan de la dirección de arte (INC-52 e INC-53) y las otras dos atañen a la lista de tareas del Nivel 3 y al capítulo de arquitectura del documento de diseño (INC-46 e INC-48); la relación completa está en §10.1.

## 1.3 Indicadores del objetivo y su cumplimiento

El trabajo de grado asocia dos indicadores clave de desempeño a este objetivo. La Tabla 1.2 los contrasta con lo obtenido.

**Tabla 1.2.** Indicadores del tercer objetivo específico y su cumplimiento.

| Indicador | Meta del trabajo de grado | Resultado | Evidencia |
|---|---|---|---|
| Tasa de implementación de mecánicas de juego | Al menos tres mecánicas principales (semana 11) | Siete implementadas de siete planeadas en el guion | §5.1; Anexos B (Fases 5 y 6), C y D |
| Índice de Niveles Jugables (Golden Path) | Tres niveles jugables con Golden Path sin bloqueos (semana 12) | Tres niveles sin bloqueos en las pruebas automatizadas (N2 y N3 completos; N1 por tramos) | §9.2; pruebas de RNF-13 |

**Mecánicas.** El guion de OE2 especifica una mecánica para el Nivel 1 (§1.4.3), una para cada una de las tres fases del Nivel 2 (§1.6.1 a §1.6.3) y tres para el Nivel 3 (§1.8.2 a §1.8.4). Las siete están implementadas:

- **Nivel 1.** Reunir los materiales y encender el fuego mediante ensayo y ajuste de la fuerza del golpe y la cercanía de las piedras. Sustituye al deslizante de posición que planeaba el guion (INC-47).
- **Nivel 2.** Selección de objetos por patrón en el bosque, ensamblaje secuencial de la carretilla en el taller y editor de bloques para guiar la carretilla por el laberinto.
- **Nivel 3.** Recolección de materiales con desplazamiento por botones en pantalla, ensamblaje de la balsa en tres fases bloqueantes, y prueba y depuración de la balsa.

La meta del indicador se supera en más del doble. Las siete mecánicas cubren además las tres formas de interacción que el indicador pone como ejemplo:

- El movimiento del personaje: el desplazamiento de Mamá por la orilla en el Nivel 3 y el de la carretilla que ejecuta la secuencia de bloques en el Nivel 2.
- La interacción con objetos: arrastrar, seleccionar, ensamblar y recolectar.
- La resolución de retos lógicos: todas las mecánicas plantean uno.

A ellas se suman dos formas de interacción transversales a los tres niveles: el diálogo narrativo con el guía y la ayuda contextual, descritas en §4.5 y §6.1.

**Golden Path.** RNF-13 exige que la ruta principal de cada nivel pueda completarse sin bloqueos, cierres inesperados ni estados irrecuperables, y OE1 lo verifica con dos recorridos completos por nivel sin incidencias bloqueantes. Una prueba recorre la máquina de estados del juego completa. El Nivel 2 tiene dos recorridos automatizados desde el arranque del juego y con un perfil real, hasta el menú con el Nivel 3 desbloqueado. El Nivel 3 tiene dos, uno que acierta la prueba de la balsa al primer intento y otro que la falla, ve la escena condicional y corrige. El Nivel 1 no tiene un recorrido único: se verifica en dos tramos consecutivos, ejecutados una vez cada uno, que van desde la apertura narrativa hasta el resumen que devuelve al menú con el Nivel 2 desbloqueado. La retoma en la fase pendiente tras un cierre forzado, con reinicio desde la escena de arranque, está probada en el Nivel 3 (RNF-14). El detalle, prueba por prueba, está en §9.2.

La corrida completa de la suite del 25/09/2026 dejó dos fallos en PlayMode, ninguno en un recorrido: una medición de memoria tomada sobre el Editor y no sobre el ejecutable, y una prueba de animación sensible al tiempo de cuadro (véase §9.3 y Anexo G). Sin bloqueantes de código (véase Anexo G, §8, y capítulo 10), quedan como actividades de cierre el recorrido del juego entero dos veces por una persona, que el cuarto incremento dejó pendiente para RNF-13, y la revisión con el usuario de los puntos de control.

## 1.4 Entregables del cronograma

El cronograma del trabajo de grado asigna al tercer objetivo seis entregables. La Tabla 1.3 resume en qué consiste cada uno en el prototipo y dónde se desarrolla.

**Tabla 1.3.** Entregables del cronograma del tercer objetivo.

| Entregable del cronograma | Qué se entrega | Sección |
|---|---|---|
| Entorno configurado | Unity 6 con plantilla 2D y URP, pruebas automatizadas y repositorio git | §2.4 |
| Build inicial | Estructura base y primer ejecutable portable (08/09/2026) | Capítulo 3 |
| Mecánicas implementadas | Las siete mecánicas principales de los tres niveles | Capítulo 5 |
| Niveles funcionales | Tres niveles con siete fases jugables y dieciocho secuencias narrativas | Capítulo 4 |
| Interfaz integrada | Pantallas del flujo, escenarios, personajes animados y sonido | Capítulo 7 |
| Prototipo optimizado | Optimizaciones aplicadas; carga y memoria medidas en el Editor y tamaño sobre el build (21/09/2026); falta la medición en el equipo de referencia (RNF-04 a RNF-06) | Capítulo 8; Anexo G, §8 |

## 1.5 Organización del documento y anexos

El documento se organiza en los siguientes capítulos, además del presente:

- **Capítulo 2.** Metodología del desarrollo: Árcade y Kanban, planificación por incrementos (slices) y carriles de trabajo, seguimiento con las actas D01 a D09 (Anexo H), entorno de desarrollo y prácticas de ingeniería.
- **Capítulo 3.** Arquitectura efectivamente implementada frente a la diseñada, módulos y dependencias, flujo de estados y escenas, contenido parametrizable, persistencia de perfiles y distribución portable.
- **Capítulo 4.** Los tres niveles y sus desafíos progresivos: qué juega el estudiante, qué problema lógico plantea cada reto y a qué faceta del pensamiento computacional sirve; escenas narrativas y síntesis de la progresión de dificultad.
- **Capítulo 5.** Mecánicas planeadas frente a mecánicas implementadas, formas de interacción y control, y ajustes al diseño durante el desarrollo.
- **Capítulo 6.** Andamiaje pedagógico: el guía y las pistas, la retroalimentación sin derrota ni puntajes, el cierre reflexivo de cada nivel, los indicadores de desempeño y el informe docente.
- **Capítulo 7.** Interfaz, escenarios, personajes, animación, sonido y accesibilidad.
- **Capítulo 8.** Presupuestos de rendimiento, sus mediciones y las optimizaciones aplicadas.
- **Capítulo 9.** Estrategia de pruebas, verificación del Golden Path, resultado de la suite completa del 25/09/2026 y trazabilidad de requerimientos a pruebas.
- **Capítulo 10.** Limitaciones y trabajo pendiente al cierre del objetivo.
- **Capítulo 11.** Conclusiones.
- **Capítulo 12.** Control de cambios del documento.

Los anexos recogen la evidencia de detalle y se entregan como documentos aparte, uno por anexo, junto a este documento principal; cada uno indica en su primera página a qué entregable pertenece, y la Tabla 1.4 da el nombre de su archivo. El Anexo A se genera a partir del código de pruebas. Los anexos B a H reproducen íntegros los documentos producidos durante el desarrollo: los anexos B a G, los documentos de resultados de cada incremento o carril, y el Anexo H, las nueve actas de seguimiento (D01 a D09), una por página. El Anexo B reúne los cinco documentos de resultados del Slice 1, uno por fase, y el documento principal los cita como «Anexo B, Fase N». Ocho de los documentos de resultados (los de las Fases 1 a 3 del Anexo B y los Anexos C a G) conservan su texto original y llevan notas fechadas y apartados de cambios posteriores, verificados al 25/09/2026. El de la Fase 0 se conserva sin esas notas. El de las Fases 5 y 6 se redactó a posteriori el 25/09/2026, porque esas fases cerraron el 12 y el 15/09/2026 sin documento de resultados. La Tabla 1.4 los relaciona.

**Tabla 1.4.** Anexos del entregable, cada uno en su propio documento.

| Anexo | Documento | Contenido | Archivo |
|---|---|---|---|
| A | Matriz de trazabilidad | Cada uno de los 47 RF con las pruebas automatizadas que lo nombran | Solucion_OE3_Anexo_A_Matriz_trazabilidad.docx |
| B | Slice 1: Fases 0, 1, 2, 3, 5 y 6 | Cimientos, navegación mínima, andamiaje mínimo, primera versión del Nivel 1 y su mecánica vigente: reunir y encender (INC-47) | Solucion_OE3_Anexo_B_Slice_1.docx |
| C | Slice 2 | Nivel 2 completo: bosque, taller, laberinto y cierre del nivel | Solucion_OE3_Anexo_C_Slice_2.docx |
| D | Slice 3 | Nivel 3 completo y cierre del juego: recolección, ensamblaje, depuración, escena final y créditos | Solucion_OE3_Anexo_D_Slice_3.docx |
| E | Carril de arte y sonido | Objetos, animaciones y piezas de sonido integrados, archivo por archivo | Solucion_OE3_Anexo_E_Arte_y_sonido.docx |
| F | Personajes animados | La familia y Algoritm animados en las narrativas y las escenas jugables | Solucion_OE3_Anexo_F_Personajes.docx |
| G | Slice 4 | Informe docente, eliminación de datos y corrida completa de la suite del 25/09/2026 | Solucion_OE3_Anexo_G_Slice_4.docx |
| H | Actas D01 a D09 | Decisiones, compromisos y tablero Kanban de cada sesión, del 02/09 al 24/09/2026 | Solucion_OE3_Anexo_H_Actas.docx |
