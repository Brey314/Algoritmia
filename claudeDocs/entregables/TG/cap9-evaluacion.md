<!-- Capítulo 9 «EVALUACIÓN DEL PROTOTIPO» (OE4). Texto del 10/10/2026, actualizado el 13/10/2026 con la sesión con estudiantes y la hoja HUM; listo para insertar. Fuente de los JSON de tools/pares (los genera cap9_a_json.py).

REGLAS DE LECTURA
- inserta-antes: BIBLIOGRAFÍA (el título Título 1 «BIBLIOGRAFÍA»; el último párrafo anterior es el cierre de 8.3: «Lo pendiente pasa al cuarto objetivo: …»).
- Cada bloque lleva encima un comentario «estilo: …». Estilos del capítulo 8: Título 1, Título 2, Título 3, Normal (cuerpo), Descripción (pie «Tabla N. …» ARRIBA de la tabla, con campo SEQ; lo escribe insertar_tabla.ps1) y Fuente (la línea «Fuente: …» debajo de la tabla; en el documento es Normal con formato directo, así que se copia de la línea de Fuente de una tabla modelo).
- Numeración de tablas: la última del documento es la Tabla 11, así que este capítulo usa de la 12 a la 17 (campo SEQ, no texto). La numeración de títulos está escrita a mano.
- Citas en texto plano [n]: [6] López-Mera (Árcade, Thinking Aloud), [29] Pressman y Maxim (pruebas de caja negra), [35] Ley 1581 de 2012, [53] actas. No hay referencias nuevas.
- «Anexo G» es la letra que sigue a la F (consentimiento) en la lista de anexos: el documento del OE4, docs/OE4/Solucion_OE4_Evaluacion_prototipo.docx, con su anexo A (matriz). Si los autores eligen otra letra, se cambia con buscar y reemplazar en este archivo antes de generar los JSON.
- Cifras: salen de claudeDocs/tasks/OE4/OE4-Resultados.md y de evidencias/rc3/ (10/10/2026, rc3 en f49ccc9).
- Los párrafos no llevan negrita, rayas, comillas inglesas ni código con comillas invertidas.
-->

<!-- estilo: Título 1 -->
# 9. EVALUACIÓN DEL PROTOTIPO

<!-- estilo: Normal -->
El cuarto objetivo pedía comprobar el funcionamiento del prototipo con pruebas funcionales basadas en los requerimientos del primero e identificar oportunidades de mejora para trabajos futuros. Para cumplirlo se aplicó un catálogo de 136 casos al ejecutable portable de Windows, y no al Editor de Unity, en su tercera versión candidata (rc3, 10 de octubre de 2026). El Anexo G reúne el catálogo completo, los resultados y el registro de cada sesión; la matriz de casos por requerimiento es su Anexo A (Anexo_A_Matriz_casos.docx), que se entrega dentro del Anexo G. El 13 de octubre de 2026 se sumó una sesión con 4 estudiantes de grado quinto del Colegio El Libertador IED y la ejecución de la hoja de comprobaciones humanas (apartado 9.2).

<!-- estilo: Título 2 -->
## 9.1 Desarrollo

<!-- estilo: Título 3 -->
### 9.1.1 Catálogo de casos

<!-- estilo: Normal -->
El plan de pruebas entró al proyecto el 29 de septiembre de 2026, con unos 120 casos y seis puntos de control, y el acta D10 lo recogió [53]. El resultado esperado de cada caso sale del requerimiento radicado, de los criterios de aceptación de la historia de usuario y del caso de uso que lo trazan y, si un hallazgo de consistencia cambió el comportamiento, de la decisión de ese hallazgo; si un criterio vive solo en una historia y el juego no lo cumple, el caso falla igual.

<!-- estilo: Normal -->
El catálogo vigente tiene 136 casos. El identificador lleva el requerimiento y un consecutivo: PF-RF34-02 es el segundo caso de RF-34. Los 47 requerimientos funcionales y los 23 no funcionales tienen al menos uno, como exige la restricción CT-10, y se suman uno de CT-02 (equipo sin tarjeta gráfica dedicada) y tres de sonido, porque el indicador de eficacia nombra los sonidos y ningún requerimiento no funcional los prueba. Eran 126 casos el 1 de octubre; los diez restantes entraron después, nueve de ellos el 10 de octubre por lo que cambió el juego. Cada uno trae un guion paso a paso y una etiqueta: NAV (navegación y saltos de nivel), BOT (botones), SON (sonido), RETO (mecánica), DAT (perfil, guardado, informe y borrado) o RNF (rendimiento, portabilidad, accesibilidad y contenido). Trece sesiones los reparten (Tabla 13): seis recorren un tramo jugable y siete son transversales.

<!-- estilo: Título 3 -->
### 9.1.2 Quién ejecuta y cómo se observa

<!-- estilo: Normal -->
El principal de los cinco ejecutores previstos, EXE, es un arnés de caja negra en PowerShell (oe4.ps1) que lanza Algoritmia.exe tras comprobar su huella SHA-256 y lo maneja como un jugador: clic, clic sostenido y arrastre en coordenadas relativas a la ventana, y texto solo en el nombre del perfil. No lee el código ni la memoria del juego, así que ve lo mismo que un estudiante, que es lo que define una prueba de caja negra [29], y cada 2 s muestrea la memoria y las conexiones de red del proceso. INSP inspecciona archivos, código e historial de git; SUITE corre la suite automatizada con el Editor cerrado; EDIT abriría el Editor para RNF-18 y no se usó (apartado 9.2); HUM es una persona con una hoja de 13 comprobaciones, de la que se registra solo lo que reporta.

<!-- estilo: Normal -->
Claude Code, el agente de programación que ya intervino en el desarrollo (apartado 8.1.1), ejecuta las sesiones EXE, INSP y SUITE bajo la supervisión de los autores, que deciden el triaje de cada defecto. Tres reglas impiden que un veredicto se invente: se observa y no se infiere, con una captura leída tras cada acción que cuenta para un caso; se separa el error del probador del error del juego; y durante las sesiones no se toca código, escenas ni assets.

<!-- estilo: Título 3 -->
### 9.1.3 El candidato rc3

<!-- estilo: Normal -->
El objeto de prueba es el ejecutable portable de Windows de 64 bits, compilado con Unity 6000.5.10f1 sin development build y con las doce escenas del juego. rc1 y rc2 se compilaron el 1 de octubre de 2026 desde el árbol de trabajo de la rama, sin etiqueta ni commit; rc2 dejó 103 de 127 casos con veredicto (89 aprobados, 13 aprobados parcialmente, ninguno fallido y uno sin ejecutar). Esos veredictos no se heredaron: entre rc2 y la pasada actual hubo 108 commits que cambiaron lo que ve el estudiante, entre ellos el rig de los personajes con codos y rodillas (INC-131), la familia de perfil al moverse (INC-134), el diseño final de Algoritm (INC-136 e INC-147) y las expresiones y el retrato animado del diálogo (INC-148 a INC-150). Cada caso se ejecutó de nuevo sobre rc3 y cada veredicto lleva la versión en que se obtuvo.

<!-- estilo: Normal -->
rc3 se compiló el 10 de octubre de 2026 en 1 min 39 s, desde el commit f49ccc9 de la rama feat/personajes-animados y con el árbol de trabajo limpio, lo que rc1 y rc2 no tuvieron. Algoritmia.exe es solo el lanzador y conserva la huella SHA-256 de los candidatos anteriores, así que cada uno se distingue por la huella del contenido de Algoritmia_Data (b8e30db6…). La carpeta entregable tiene 253 archivos y pesa 426,6 MB frente a los 479,0 MB de rc2, aunque ya incluye el arte final de los personajes, porque cinco imágenes del Nivel 1 pasaron a importarse a 256 px (INC-146). Las sesiones corrieron en el equipo de desarrollo, un AMD Ryzen 5 7600X con 32 GB de RAM, una NVIDIA GeForce RTX 5070 Ti y Windows 11 Home de 64 bits, con la ventana a 1920 × 1080.

<!-- estilo: Título 3 -->
### 9.1.4 Criterios de veredicto

<!-- estilo: Normal -->
Un caso termina en uno de cinco veredictos: aprobado (P) si se observan todos los resultados esperados de sus pasos; aprobado con desviación documentada (PD) si lo observado sigue la decisión de un hallazgo de consistencia; fallido (F) si alguno no se observa, y se abre un defecto; bloqueado (B) si la precondición o el entorno impidieron ejecutarlo; y no aplica (NA) si el requerimiento lo deja fuera de un prototipo sin usuarios, como RNF-12. B, NA y los casos pendientes de una persona (PS) quedan fuera del denominador.

<!-- estilo: Normal -->
La eficacia es (P + PD) / (P + PD + F) × 100, con meta de 90 % o más, y se publica global y por etiqueta porque el indicador nombra botones, sonidos y saltos de nivel y un promedio podría esconder una etiqueta bajo la meta. El cumplimiento exige que los 45 RF de prioridad Alta (RF-06 es Media y RF-21 Baja, según el OE1) estén implementados, y un RF lo está si ninguno de sus casos termina en F Bloqueante o Mayor; un F Menor resta eficacia, pero no niega que el requerimiento exista.

<!-- estilo: Normal -->
La severidad es Bloqueante si el defecto impide terminar la ruta principal, cierra el juego, pierde progreso confirmado o deja un estado sin salida; Mayor si no se cumple un RF o un criterio de aceptación pero el juego sigue; y Menor si toca un texto, el aspecto o un criterio secundario. Lo que se anota sin incumplir ningún criterio es una observación. Los Bloqueantes y Mayores se corrigen dentro del objetivo y los demás se deciden en el triaje. Si el juego y un documento radicado discrepan, la evaluación propone un hallazgo nuevo en el registro de inconsistencias y no edita el documento.

<!-- estilo: Título 2 -->
## 9.2 Resultados

<!-- estilo: Normal -->
Los veredictos son los de rc3. El objetivo tenía dos indicadores (Tabla 12), una eficacia de 90 % o más en botones, sonidos y saltos de nivel y los RF de prioridad Alta implementados, y se añade la cobertura de CT-10 como condición de partida.

<!-- estilo: Descripción -->
Tabla 12. Metas del cuarto objetivo y resultado obtenido

| Indicador | Meta | Resultado |
| --- | --- | --- |
| Eficacia de las pruebas funcionales | 90 % o más | 97,0 % (128 de 132 casos con veredicto; 96,8 %, 122 de 126, en el corte del 10 de octubre) |
| Eficacia en las etiquetas NAV, BOT y SON | 90 % o más en cada una | NAV 94,1 %, BOT 100 % y SON 100 % (3 de 3, aprobados por una persona el 13 de octubre; sin calcular en el corte del 10 de octubre) |
| RF de prioridad Alta implementados | 45 de 45 | 45 de 45, sin defecto Bloqueante ni Mayor |
| Requerimientos con al menos un caso de prueba (CT-10) | 47 RF y 23 RNF | 47 de 47 RF y 23 de 23 RNF, con 136 casos |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir de la pasada de pruebas sobre rc3 (Anexo G).

<!-- estilo: Normal -->
Antes de la pasada se corrió la suite completa, el 10 de octubre: 1 167 pruebas, con 1 165 aprobadas, una omitida y una fallida. En EditMode pasan 748 de 749 (una omisión por entorno) y en PlayMode 417 de 418; el fallo es RiverLevel_RNF21, sensible al tiempo, que pasa corrida sola. Son 321 pruebas más que las 846 del 1 de octubre (Tabla 10). Se corrió sobre el commit 292b108 y no sobre el contenido exacto de rc3, que solo difiere en una escena reserializada y seis imágenes sin referencias (DEF-RC3-I04). Es evidencia de apoyo y no cuenta en los indicadores, porque los casos se juzgan sobre el ejecutable.

<!-- estilo: Normal -->
De los 136 casos, 128 terminaron en P, 4 en F y 4 siguen pendientes (PF-RNF12-01, PF-RNF13-01, PF-RNF22-01 y PF-RNF23-01), sin PD, B ni NA (Tabla 13). Seis de los 128 P salen de la hoja de comprobaciones humanas, que se ejecutó el 13 de octubre en una sesión manual sobre rc3 y no con el arnés: PF-RNF07-02 (H3), PF-RNF08-01 (H4), PF-CT02-01 (H5) y PF-SON-01 a PF-SON-03 (H12, con H6 y H7). En el corte del 10 de octubre eran 122 P, 4 F y 10 pendientes. Los fallidos son PF-RF45-05, de la sesión de omisión, y PF-RNF03-01, PF-RNF15-01 y PF-RNF17-01, de la de inspecciones; las demás sesiones aprobaron todos sus casos.

<!-- estilo: Descripción -->
Tabla 13. Veredictos de rc3 por sesión

| Sesión | Casos | Veredictos (P · F · pendientes) |
| --- | --- | --- |
| S-INI, inicio, perfiles, créditos y salida | 14 | 14 · 0 · 0 |
| S-N1, Nivel 1 completo | 25 | 25 · 0 · 0 |
| S-N2A, bosque | 10 | 10 · 0 · 0 |
| S-N2B, taller | 4 | 4 · 0 · 0 |
| S-N2C, laberinto y cierre del Nivel 2 | 10 | 10 · 0 · 0 |
| S-N3, Nivel 3 y cierre del juego | 20 | 20 · 0 · 0 |
| S-PAU, pausa y reinicio | 4 | 4 · 0 · 0 |
| S-OMI, omisión, rejuego y casos negativos | 8 | 7 · 1 · 0 |
| S-PER, persistencia y cierre forzado | 7 | 7 · 0 · 0 |
| S-DOC, informe docente y borrado | 9 | 9 · 0 · 0 |
| S-RNF, medidas sobre el ejecutable | 6 | 6 · 0 · 0 |
| S-INSP, inspecciones y accesibilidad | 13 | 7 · 3 · 3 |
| S-HUM, pruebas con personas y en otros equipos | 6 | 5 · 0 · 1 |
| Total | 136 | 128 · 4 · 4 |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir del catálogo de casos y de la pasada sobre rc3 (Anexo G). La matriz de casos por requerimiento está en el Anexo A del documento del OE4 (Anexo_A_Matriz_casos.docx), que se entrega dentro del Anexo G.

<!-- estilo: Normal -->
Por etiqueta, NAV alcanza 94,1 % (16 de 17), BOT 100 % (28 de 28), RETO 100 % (37 de 37) y DAT 100 % (30 de 30). SON queda en 100 % (3 de 3), porque sus tres casos son de oído y los aprobó una persona el 13 de octubre, lo que es un juicio y no una medición; RNF queda en 82,4 % (14 de 17) por los tres fallidos de la inspección, frente a 78,6 % (11 de 14) en el corte del 10 de octubre.

<!-- estilo: Normal -->
PF-RNF13-01 pide recorrer el juego de la pantalla de inicio a los créditos sin omitir ninguna narrativa. Los dos recorridos del arnés no se repitieron sobre rc3 y la comprobación H8, con cronómetro, dio el 13 de octubre entre 40 y 45 min, hasta 5 min por encima de la referencia de 20 a 40 min del OE1; no se registró la causa. Cada nivel sí se jugó entero en su sesión, con perfiles y semillas distintos: ninguno se bloqueó y el Player.log de todos los lanzamientos quedó sin Exception. La resolución del Nivel 1 con un perfil nuevo duró 225,84 s, y los tres recorridos completos de versiones anteriores, 87, 33 y 16 min (apartado 8.2).

<!-- estilo: Normal -->
Los tres presupuestos duros se miden sobre el ejecutable (Tabla 14). La carga sale de la línea RNF-04 del Player.log y mide la carga, no el primer cuadro visible; la peor fue la de una escena narrativa, con 0,268 s, las demás cargaron en 0,13 s o menos y el arranque, de Boot al menú, tarda cerca de 3,4 s. La memoria privada máxima fue de 1094 MiB, en la cueva del Nivel 1, y la de trabajo de 438 MiB, sobre 4 213 muestras de todas las sesiones. En RNF-10, 4 516 muestras de red tomadas cada 2 s no registraron ninguna conexión TCP ni UDP del proceso, y el ejecutable no nombra los servidores de Unity. Las cifras de rc2 no se heredan, porque el arte de perfil, las caras y los retratos entraron después.

<!-- estilo: Descripción -->
Tabla 14. Presupuestos de rendimiento y de paquete en rc3

| Requerimiento | Límite | rc2 (01/10/2026) | rc3 (10/10/2026) |
| --- | --- | --- | --- |
| RNF-04, carga de escena | 10 s | 0,78 s en la carga más lenta | 0,268 s en la carga más lenta |
| RNF-05, memoria | 2 GB | Máximos de 426 MiB de memoria de trabajo y 1125 MiB de memoria privada | Máximos de 438 MiB de memoria de trabajo y 1094 MiB de memoria privada |
| RNF-06, tamaño del paquete | 500 MB | 479,0 MB | 426,6 MB, con 73,4 MB de margen |
| CT-02, equipo sin tarjeta gráfica dedicada | Ejecutar sin GPU dedicada | Sin medir | Sin cifra de carga ni de memoria; la comprobación H5 se aprobó el 13 de octubre en dos equipos sin conexión a internet (Tabla 17) |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir de la pasada de pruebas (Anexo G) y de la Tabla 11 de este trabajo.

<!-- estilo: Normal -->
Esas medidas salen de un solo equipo. RNF-16 se probó con unity test en copias del proyecto sin un nivel, donde cinco pruebas de arquitectura que exigen los tres niveles fallan en los dos sentidos (DEF-RC3-12), y RNF-18 en un ejecutable aparte, no con el Editor abierto, porque el arnés no pudo conducir su ventana de juego.

<!-- estilo: Normal -->
La primera pasada, sobre rc1, halló nueve defectos: cuatro Mayores y cinco Menores, ninguno Bloqueante (Tabla 15). Los Mayores fueron las conexiones de red que abría el reproductor en cada arranque (RNF-10), el panel de perfiles, que con ocho dejaba sin acceso a los estudiantes del séptimo en adelante (RF-02), el bloque del laberinto pegado al cursor (RF-34) y el resaltado del bloque en curso, invisible con la lista desplazada (RF-32). Cada corrección llevó una prueba nueva que nombra su requerimiento, y rc2 reverificó los nueve sobre el ejecutable. Quedó DEF-RC2-01, un residuo del motor de severidad Menor: el reproductor escribe valores unity_connect en el registro de Windows, sin datos del estudiante, y persiste en rc3.

<!-- estilo: Normal -->
La pasada sobre rc3 registró 16 hallazgos, 7 Menores y 9 observaciones, ninguno Bloqueante ni Mayor y todos abiertos a la espera del triaje. De los Menores, tres tocan el juego: los iconos del informe docente montados sobre el texto de las cabeceras (DEF-RC3-05), la instrucción del bosque que se sale de su tablilla (DEF-RC3-10) y un doble clic en «Omitir» que lleva al menú de niveles sin mostrar el resumen, sin perder el progreso (DEF-RC3-09). Los otros cuatro vienen de la inspección: identificadores en español (DEF-RC3-I01), commits sin tarjeta (DEF-RC3-I02), un halo de croma en nueve imágenes de caras (DEF-RC3-I03) y una suite que no corrió sobre el contenido exacto de rc3 (DEF-RC3-I04). Las nueve observaciones no incumplen ningún criterio: cuatro piden corregir el documento de casos, tres esperan una decisión de arte o de diseño, una es de disposición y una atañe a las pruebas de arquitectura.

<!-- estilo: Descripción -->
Tabla 15. Defectos hallados por severidad y estado al cierre de la pasada sobre rc3

| Severidad | Hallados en rc1 | Hallados en rc2 | Hallados en rc3 | Abiertos al cierre |
| --- | --- | --- | --- | --- |
| Bloqueante | 0 | 0 | 0 | 0 |
| Mayor | 4 | 0 | 0 | 0 |
| Menor | 5 | 1 (DEF-RC2-01) | 7 | 8 |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir del registro de defectos (Anexo G); no incluye las nueve observaciones de rc3.

<!-- estilo: Normal -->
Dos tipos de prueba quedan fuera del arnés: las que necesitan niños de grado quinto jugando y las que necesitan los sentidos de una persona o un segundo equipo (Tabla 16). Eran diez casos y dos puntos del guion, PG-05 y PG-06. La hoja de comprobaciones humanas se ejecutó el 13 de octubre de 2026 en el Colegio El Libertador IED, sede A, con 4 estudiantes de grado quinto. Un observador miró cómo jugaban; los estudiantes pensaron en voz alta y comunicaron sus inquietudes al observador, la técnica Thinking Aloud que la metodología Árcade asigna a esta fase [6]. La muestra es no probabilística (apartado 5.3) y la sesión verifica que la interfaz se entienda y la ruta principal se complete, sin medir el efecto del juego.

<!-- estilo: Normal -->
La sesión tuvo dos autorizaciones. El colegio firmó el 17 de septiembre de 2026, por medio de su rector, Richard Maldonado, el Formato de autorización institucional, que permite la participación de estudiantes de grado quinto de primaria dentro de la investigación doctoral de la docente Bibiana Patricia Rey Barrote, contacto del equipo en el colegio, con la condición del consentimiento informado de padres o acudientes. Ese consentimiento, exigido por RNF-12 conforme a la Ley 1581 de 2012 [35], lo firma la madre, el padre o el acudiente que sea representante legal del estudiante. El firmante autoriza que el estudiante participe en una sesión de 20 a 40 minutos y que el juego guarde su perfil, su avance y los cuatro indicadores de cada fase; el perfil lleva un código o un apodo y no el nombre real. El formato prevé el retiro de la autorización en cualquier momento y el borrado de cada perfil con la función Eliminar datos del juego al terminar las pruebas. Los cuatro consentimientos firmados se custodian y no se anexan, porque traen nombres, documentos y firmas de menores y de sus familias; el formato en blanco es el Anexo F. Al terminar la sesión se borraron los perfiles y la carpeta Datos quedó vacía en los equipos. Esa autorización fija el grado de la evaluación: el grupo evaluado es de grado quinto, y este trabajo de grado habla de grado quinto en todo su texto. Los documentos radicados del OE1 y del OE2, las historias de usuario y la arquitectura se actualizaron a grado quinto el 13/10/2026 (INC-151).

<!-- estilo: Descripción -->
Tabla 16. Comprobaciones con personas y su resultado al 13 de octubre de 2026

| Comprobación | Casos y hoja | Resultado |
| --- | --- | --- |
| Estudiantes jugando: cambio de controles y valores del Nivel 1 | PG-05 y PG-06, H1 y H2 | Positivo en ambos, con 4 estudiantes de grado quinto del Colegio El Libertador IED, sede A, el 13 de octubre de 2026 |
| Consentimiento informado y autorización institucional | PF-RNF12-01, Anexo F | Autorización institucional del 17 de septiembre de 2026 y 4 consentimientos firmados, custodiados; constancia de borrado de los perfiles positiva |
| Recorrido completo con cronómetro | PF-RNF13-01, H8 | Entre 40 y 45 min frente a la referencia de 20 a 40 min; supera el tope en hasta 5 min, sin causa registrada |
| Segundo equipo y equipo sin tarjeta gráfica dedicada | PF-RNF07-02 y PF-CT02-01, H3 y H5 | Aprobado; los equipos están en la Tabla 17 |
| Ejecución sin conexión a internet | PF-RNF08-01, H4 | Aprobado, con los dos equipos sin conexión; el arnés no vio conexiones en 4 516 muestras |
| Sonido y vista | PF-SON-01 a PF-SON-03, H6, H7 y H9 a H12 | Aprobado. H6: el sonido se escucha. H9, H10 y H12: sin comentarios, todo se comprendió. H11: la balsa se hundió, se comprendió por qué, se arregló y finalmente cruzó |
| Mano: el clic tras cargar una escena, la papelera del laberinto y la octava fila del informe | PF-RNF02-01, PF-RF34-01 y PF-RF46-01, H13 | No se realizó en la sesión; pasa a trabajo futuro |
| Contenido y autorizaciones | PF-RNF22-01 y PF-RNF23-01 | Inspección hecha; [Pendiente: confirmación del contenido y soporte escrito de la Familia Anonaky y de la colaboradora de arte] |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir de la hoja de comprobaciones humanas (Anexo G).

<!-- estilo: Normal -->
Los dos equipos de la sesión no tenían conexión a internet. La sala disponía de internet, de 20 Mb/s de bajada y 7 Mb/s de subida, y no se usó. Las cifras de carga y de memoria de la Tabla 14 son las del equipo 1 de las mediciones sobre rc3 (Ryzen 5 7600X, RTX 5070 Ti).

<!-- estilo: Descripción -->
Tabla 17. Equipos de la sesión del 13 de octubre de 2026

| Equipo | Procesador | Gráficos | Memoria | Disco | Red |
| --- | --- | --- | --- | --- | --- |
| Equipo A | AMD A10-9600P | Radeon R5, 66 MB de VRAM | 4 GB de RAM | 512 GB | Sin conexión a internet |
| Equipo B | Intel Celeron B830 | Intel HD Graphics, 32 MB | 4 GB de RAM | 512 GB | Sin conexión a internet |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir de la hoja de comprobaciones humanas (Anexo G).

<!-- estilo: Título 2 -->
## 9.3 Conclusiones del objetivo

<!-- estilo: Normal -->
De los 136 casos, 128 terminan en P, 4 en F y 4 siguen pendientes. La eficacia es de 97,0 % (128 de 132) frente a la meta de 90 % (96,8 % en el corte del 10 de octubre), y los 45 RF de prioridad Alta están implementados, porque ninguno tiene un defecto Bloqueante o Mayor; el único fallido que toca uno de ellos, PF-RF45-05, es Menor. De las etiquetas que el indicador nombra, NAV (94,1 %), BOT (100 %) y SON (100 %, tres casos de oído aprobados el 13 de octubre en H6, H7 y H12) superan la meta; la más baja es RNF, con 82,4 %, por textos e iconos fuera de su caja (PF-RNF03-01), identificadores en español (PF-RNF15-01) y commits sin tarjeta (PF-RNF17-01). Los dos indicadores se cumplen; el de sonidos descansa en un juicio de oído y no en una medición.

<!-- estilo: Normal -->
Evaluar sobre el ejecutable y no sobre el Editor cambió resultados que la suite no podía mostrar. En la primera pasada aparecieron las conexiones de red del reproductor, porque las pruebas leían la configuración del repositorio y no la que queda dentro del ejecutable, y dos defectos del laberinto, porque las pruebas llaman a los controladores en vez de simular el puntero. Sobre rc3, con el arte final, la familia de perfil y las expresiones por línea, todas las sesiones jugables aprobaron y lo que apareció fue de disposición: iconos y textos fuera de su caja y un doble clic en «Omitir».

<!-- estilo: Normal -->
Lo que la evaluación no pudo observar tiene causa: el arnés no oye, no cronometra a un niño ni cambia de equipo, y PG-05 y PG-06 exigen estudiantes y el consentimiento de sus acudientes. Por eso RNF-07, RNF-08, CT-02, el sonido y la ruta de 20 a 40 min dependían de la sesión del 13 de octubre, que dio PG-05 y PG-06 positivos y aprobó las comprobaciones H1 a H7 y H9 a H12; el recorrido de H8 duró entre 40 y 45 min y H13 no se realizó. Seis de los diez casos pasaron a P con esa sesión. Los cuatro que siguen pendientes quedan fuera del denominador: PF-RNF13-01, porque los dos recorridos del arnés sobre rc3 no se ejecutaron y H8 no cubre los tres recorridos que el caso pide; PF-RNF12-01, que ninguna comprobación de la hoja cubre; y PF-RNF22-01 y PF-RNF23-01, que esperan la confirmación del contenido y el soporte escrito de la Familia Anonaky y de la colaboradora de arte. Si los cuatro fallaran, la eficacia sería 94,1 % (128 de 136).

<!-- estilo: Normal -->
Las correcciones que dejó la sesión del 13 de octubre (luz del Nivel 1, caminata, abrazo y sueño de perfil, troncos del bosque, flechas de «Girar», vocabulario, aviso del botón de ayuda, pausa de cinco botones y botón «Atrás» del diálogo) entraron en el candidato rc4, que pesa 426,9 MB frente al tope de 500 MB de RNF-06 (13/10/2026). La suite completa dio EditMode 785 de 786, con una omitida por entorno, y PlayMode 427 de 427. Los estudiantes jugaron rc3 y rc4 no se volvió a probar con ellos; las cifras de carga y memoria de este capítulo (RNF-04 y RNF-05) son las de rc3.

<!-- estilo: Título 3 -->
### 9.3.1 Oportunidades de mejora para trabajos futuros

<!-- estilo: Normal -->
Las oportunidades salen de los defectos abiertos, de las observaciones y de lo que quedó sin medir, y se ordenan por su efecto en el estudiante y en el docente. En la interfaz, la instrucción de tres líneas del bosque se sale de su tablilla (DEF-RC3-10); el informe docente necesita una altura propia para los iconos de la cabecera y un botón «Volver al menú» más ancho (DEF-RC3-05), y «Eliminar datos» parte su texto en dos líneas (DEF-RC3-11); el resumen de nivel muestra a Algoritm como un círculo con su nombre y no con su diseño final (DEF-RC3-01). En la mecánica, ignorar la entrada unos 300 ms tras cargar el resumen evita que un doble clic en «Omitir» llegue a «Volver al menú de niveles» (DEF-RC3-09), y una evaluación con estudiantes que pida partidas comparables exige fijar la distribución de las siete piezas del Nivel 1, hoy aleatoria (DEF-RC3-04).

<!-- estilo: Normal -->
La inspección del código y del historial halló tres. RNF-15 no se cumple en ForestSceneController, WorkshopSceneController y MazeSceneController, del Nivel 2, que conservan identificadores en español (DEF-RC3-I01). La trazabilidad entre commit y tarjeta del tablero (RNF-17) es débil: 9 de los 122 commits que tocan Assets llevan tarjeta (7,4 %), frente al 100 % que pide el criterio; no se corrige sin reescribir la historia, pero se evita desde ahora exigiéndola en la plantilla del mensaje (DEF-RC3-I02). Y nueve imágenes de caras conservan un halo tenue de croma (DEF-RC3-I03), que se limpia con la herramienta arte_check.py y copia de respaldo. Las pruebas de arquitectura deberían tolerar un nivel ausente para que RNF-16 pueda declarar cero fallos de acoplamiento (DEF-RC3-12), el laberinto debe correrse en PlayMode sobre el commit que se entregue (DEF-RC3-I04) y el paquete trae DirectML.dll, la carpeta D3D12 y módulos de red de Unity que nada del juego llama.

<!-- estilo: Normal -->
De la Tabla 16 pasan a trabajo futuro la comprobación de mano H13, que no se realizó, y la causa de que el recorrido de H8 superara la referencia de 20 a 40 min, que no se registró. Como trabajo futuro quedan también el residuo que el motor escribe en el registro de Windows (DEF-RC2-01) y la música, el sonido del diálogo, el ambiente nocturno del Nivel 2 y el silencio de la escena final, del carril de sonido. Cuatro observaciones piden corregir el documento de casos y no el juego (DEF-RC3-02, 04, 06 y 07).
