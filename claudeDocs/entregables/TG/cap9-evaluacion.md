<!-- Capítulo 9 «EVALUACIÓN DEL PROTOTIPO» (OE4). Texto definitivo del 10/10/2026, listo para insertar. Fuente de los JSON de tools/pares (los genera cap9_a_json.py).

REGLAS DE LECTURA
- inserta-antes: BIBLIOGRAFÍA (el título Título 1 «BIBLIOGRAFÍA»; el último párrafo anterior es el cierre de 8.3: «Lo pendiente pasa al cuarto objetivo: …»).
- Cada bloque lleva encima un comentario «estilo: …». Estilos del capítulo 8: Título 1, Título 2, Título 3, Normal (cuerpo), Descripción (pie «Tabla N. …» ARRIBA de la tabla, con campo SEQ; lo escribe insertar_tabla.ps1) y Fuente (la línea «Fuente: …» debajo de la tabla; en el documento es Normal con formato directo, así que se copia de la línea de Fuente de una tabla modelo).
- Numeración de tablas: la última del documento es la Tabla 11, así que este capítulo usa de la 12 a la 16 (campo SEQ, no texto). La numeración de títulos está escrita a mano.
- Citas en texto plano [n]: [6] López-Mera (Árcade, Thinking Aloud), [29] Pressman y Maxim (pruebas de caja negra), [35] Ley 1581 de 2012, [53] actas. No hay referencias nuevas.
- «Anexo G» es la letra que sigue a la F (consentimiento) en la lista de anexos: el documento del OE4, docs/OE4/Solucion_OE4_Evaluacion_prototipo.docx, con su anexo A (matriz). Si los autores eligen otra letra, se cambia con buscar y reemplazar en este archivo antes de generar los JSON.
- Cifras: salen de claudeDocs/tasks/OE4/OE4-Resultados.md y de evidencias/rc3/ (10/10/2026, rc3 en f49ccc9).
- Los párrafos no llevan negrita, rayas, comillas inglesas ni código con comillas invertidas.
-->

<!-- estilo: Título 1 -->
# 9. EVALUACIÓN DEL PROTOTIPO

<!-- estilo: Normal -->
El cuarto objetivo pedía comprobar el funcionamiento del prototipo con pruebas funcionales basadas en los requerimientos del primero e identificar oportunidades de mejora para trabajos futuros. Para cumplirlo se aplicó un catálogo de 136 casos al ejecutable portable de Windows, y no al Editor de Unity, en su tercera versión candidata (rc3, 10 de octubre de 2026). El Anexo G reúne el catálogo completo, los resultados y el registro de cada sesión; la matriz de casos por requerimiento es su Anexo A (Anexo_A_Matriz_casos.docx), que se entrega dentro del Anexo G.

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
El principal de los cinco ejecutores previstos, EXE, es un arnés de caja negra en PowerShell (oe4.ps1) que lanza Algoritmia.exe tras comprobar su huella SHA-256 y lo maneja como un jugador: clic, clic sostenido y arrastre en coordenadas relativas a la ventana, y texto solo en el nombre del perfil. No lee el código ni la memoria del juego, así que ve lo mismo que un estudiante, que es lo que define una prueba de caja negra [29], y cada 2 s muestrea la memoria y las conexiones de red del proceso. INSP inspecciona archivos, código e historial de git; SUITE corre la suite automatizada con el Editor cerrado; EDIT abriría el Editor para RNF-18 y no se usó (apartado 9.2); HUM es Santiago Benavides Rey, con una hoja de 13 comprobaciones de la que se registra solo lo que él reporta.

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
Un caso termina en uno de cinco veredictos: aprobado (P) si se observan todos los resultados esperados de sus pasos; aprobado con desviación documentada (PD) si lo observado sigue la decisión de un hallazgo de consistencia; fallido (F) si alguno no se observa, y se abre un defecto; bloqueado (B) si la precondición o el entorno impidieron ejecutarlo; y no aplica (NA) si el requerimiento lo deja fuera de un prototipo sin usuarios, como RNF-12. B, NA y los casos pendientes de Santiago Benavides Rey (PS) quedan fuera del denominador.

<!-- estilo: Normal -->
La eficacia es (P + PD) / (P + PD + F) × 100, con meta de 90 % o más, y se publica global y por etiqueta porque el indicador nombra botones, sonidos y saltos de nivel y un promedio podría esconder una etiqueta bajo la meta. El cumplimiento exige que los 45 RF de prioridad Alta (RF-06 es Media y RF-21 Baja, según el OE1) estén implementados, y un RF lo está si ninguno de sus casos termina en F Bloqueante o Mayor; un F Menor resta eficacia, pero no niega que el requerimiento exista.

<!-- estilo: Normal -->
La severidad es Bloqueante si el defecto impide terminar la ruta principal, cierra el juego, pierde progreso confirmado o deja un estado sin salida; Mayor si no se cumple un RF o un criterio de aceptación pero el juego sigue; y Menor si toca un texto, el aspecto o un criterio secundario. Lo que se anota sin incumplir ningún criterio es una observación. Los Bloqueantes y Mayores se corrigen dentro del objetivo y los demás los decide Santiago Benavides Rey. Si el juego y un documento radicado discrepan, la evaluación propone un hallazgo nuevo en el registro de inconsistencias y no edita el documento.

<!-- estilo: Título 2 -->
## 9.2 Resultados

<!-- estilo: Normal -->
Los veredictos son los de rc3. El objetivo tenía dos indicadores (Tabla 12), una eficacia de 90 % o más en botones, sonidos y saltos de nivel y los RF de prioridad Alta implementados, y se añade la cobertura de CT-10 como condición de partida.

<!-- estilo: Descripción -->
Tabla 12. Metas del cuarto objetivo y resultado obtenido

| Indicador | Meta | Resultado |
| --- | --- | --- |
| Eficacia de las pruebas funcionales | 90 % o más | 96,8 % (122 de 126 casos con veredicto) |
| Eficacia en las etiquetas NAV, BOT y SON | 90 % o más en cada una | NAV 94,1 % y BOT 100 %; SON sin calcular, con sus 3 casos de oído pendientes [Pendiente: resultado de H6, H7 y H12] |
| RF de prioridad Alta implementados | 45 de 45 | 45 de 45, sin defecto Bloqueante ni Mayor |
| Requerimientos con al menos un caso de prueba (CT-10) | 47 RF y 23 RNF | 47 de 47 RF y 23 de 23 RNF, con 136 casos |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir de la pasada de pruebas sobre rc3 (Anexo G).

<!-- estilo: Normal -->
Antes de la pasada se corrió la suite completa, el 10 de octubre: 1 167 pruebas, con 1 165 aprobadas, una omitida y una fallida. En EditMode pasan 748 de 749 (una omisión por entorno) y en PlayMode 417 de 418; el fallo es RiverLevel_RNF21, sensible al tiempo, que pasa corrida sola. Son 321 pruebas más que las 846 del 1 de octubre (Tabla 10). Se corrió sobre el commit 292b108 y no sobre el contenido exacto de rc3, que solo difiere en una escena reserializada y seis imágenes sin referencias (DEF-RC3-I04). Es evidencia de apoyo y no cuenta en los indicadores, porque los casos se juzgan sobre el ejecutable.

<!-- estilo: Normal -->
De los 136 casos, 122 terminaron en P, 4 en F y 10 quedaron pendientes de Santiago Benavides Rey, sin PD, B ni NA (Tabla 13). Los fallidos son PF-RF45-05, de la sesión de omisión, y PF-RNF03-01, PF-RNF15-01 y PF-RNF17-01, de la de inspecciones; las demás sesiones aprobaron todos sus casos.

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
| S-RNF, medidas sobre el ejecutable | 6 | 5 · 0 · 1 |
| S-INSP, inspecciones y accesibilidad | 13 | 7 · 3 · 3 |
| S-HUM, pruebas con personas y en otros equipos | 6 | 0 · 0 · 6 |
| Total | 136 | 122 · 4 · 10 |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir del catálogo de casos y de la pasada sobre rc3 (Anexo G). La matriz de casos por requerimiento está en el Anexo A del documento del OE4 (Anexo_A_Matriz_casos.docx), que se entrega dentro del Anexo G.

<!-- estilo: Normal -->
Por etiqueta, NAV alcanza 94,1 % (16 de 17), BOT 100 % (28 de 28), RETO 100 % (37 de 37) y DAT 100 % (30 de 30). SON no se puede calcular, porque sus tres casos son de oído y los hace Santiago Benavides Rey; RNF queda en 78,6 % (11 de 14) por los tres fallidos de la inspección.

<!-- estilo: Normal -->
PF-RNF13-01 pide recorrer el juego de la pantalla de inicio a los créditos sin omitir ninguna narrativa. Los dos recorridos del arnés no se repitieron sobre rc3 y el de Santiago Benavides Rey con cronómetro está pendiente, así que la referencia de 20 a 40 min del OE1 no se contrastó [Pendiente: duración e incidencias de la comprobación H8]. Cada nivel sí se jugó entero en su sesión, con perfiles y semillas distintos: ninguno se bloqueó y el Player.log de todos los lanzamientos quedó sin Exception. La resolución del Nivel 1 con un perfil nuevo duró 225,84 s, y los tres recorridos completos de versiones anteriores, 87, 33 y 16 min (apartado 8.2).

<!-- estilo: Normal -->
Los tres presupuestos duros se miden sobre el ejecutable (Tabla 14). La carga sale de la línea RNF-04 del Player.log y mide la carga, no el primer cuadro visible; la peor fue la de una escena narrativa, con 0,268 s, las demás cargaron en 0,13 s o menos y el arranque, de Boot al menú, tarda cerca de 3,4 s. La memoria privada máxima fue de 1094 MiB, en la cueva del Nivel 1, y la de trabajo de 438 MiB, sobre 4 213 muestras de todas las sesiones. En RNF-10, 4 516 muestras de red tomadas cada 2 s no registraron ninguna conexión TCP ni UDP del proceso, y el ejecutable no nombra los servidores de Unity. Las cifras de rc2 no se heredan, porque el arte de perfil, las caras y los retratos entraron después.

<!-- estilo: Descripción -->
Tabla 14. Presupuestos de rendimiento y de paquete en rc3

| Requerimiento | Límite | rc2 (01/10/2026) | rc3 (10/10/2026) |
| --- | --- | --- | --- |
| RNF-04, carga de escena | 10 s | 0,78 s en la carga más lenta | 0,268 s en la carga más lenta |
| RNF-05, memoria | 2 GB | Máximos de 426 MiB de memoria de trabajo y 1125 MiB de memoria privada | Máximos de 438 MiB de memoria de trabajo y 1094 MiB de memoria privada |
| RNF-06, tamaño del paquete | 500 MB | 479,0 MB | 426,6 MB, con 73,4 MB de margen |
| CT-02, equipo sin tarjeta gráfica dedicada | Ejecutar sin GPU dedicada | Sin medir | Sin medir [Pendiente: equipo y resultado de la comprobación H5] |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir de la pasada de pruebas (Anexo G) y de la Tabla 11 de este trabajo.

<!-- estilo: Normal -->
Esas medidas salen de un solo equipo. RNF-16 se probó con unity test en copias del proyecto sin un nivel, donde cinco pruebas de arquitectura que exigen los tres niveles fallan en los dos sentidos (DEF-RC3-12), y RNF-18 en un ejecutable aparte, no con el Editor abierto, porque el arnés no pudo conducir su ventana de juego.

<!-- estilo: Normal -->
La primera pasada, sobre rc1, halló nueve defectos: cuatro Mayores y cinco Menores, ninguno Bloqueante (Tabla 15). Los Mayores fueron las conexiones de red que abría el reproductor en cada arranque (RNF-10), el panel de perfiles, que con ocho dejaba sin acceso a los estudiantes del séptimo en adelante (RF-02), el bloque del laberinto pegado al cursor (RF-34) y el resaltado del bloque en curso, invisible con la lista desplazada (RF-32). Cada corrección llevó una prueba nueva que nombra su requerimiento, y rc2 reverificó los nueve sobre el ejecutable. Quedó DEF-RC2-01, un residuo del motor de severidad Menor: el reproductor escribe valores unity_connect en el registro de Windows, sin datos del estudiante, y persiste en rc3.

<!-- estilo: Normal -->
La pasada sobre rc3 registró 16 hallazgos, 7 Menores y 9 observaciones, ninguno Bloqueante ni Mayor y todos abiertos a la espera del triaje de Santiago Benavides Rey. De los Menores, tres tocan el juego: los iconos del informe docente montados sobre el texto de las cabeceras (DEF-RC3-05), la instrucción del bosque que se sale de su tablilla (DEF-RC3-10) y un doble clic en «Omitir» que lleva al menú de niveles sin mostrar el resumen, sin perder el progreso (DEF-RC3-09). Los otros cuatro vienen de la inspección: identificadores en español (DEF-RC3-I01), commits sin tarjeta (DEF-RC3-I02), un halo de croma en nueve imágenes de caras (DEF-RC3-I03) y una suite que no corrió sobre el contenido exacto de rc3 (DEF-RC3-I04). Las nueve observaciones no incumplen ningún criterio: cuatro piden corregir el documento de casos, tres esperan una decisión de arte o de diseño, una es de disposición y una atañe a las pruebas de arquitectura.

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
Dos tipos de prueba quedan fuera del arnés: las que necesitan niños de grado cuarto jugando y las que necesitan los sentidos de una persona o un segundo equipo (Tabla 16). Son diez casos y dos puntos del guion, PG-05 y PG-06. La sesión con estudiantes los cierra con la técnica Thinking Aloud que la metodología Árcade asigna a esta fase [6], previo consentimiento informado de los acudientes según la Ley 1581 de 2012 [35] (RNF-12; el formato en blanco es el Anexo F). La muestra es no probabilística (apartado 5.3) y la sesión verifica que la interfaz se entienda y la ruta principal se complete, sin medir el efecto del juego. La sesión se hará después de esta entrega y sus resultados llenarán los espacios de la Tabla 16; mientras tanto PG-05, PG-06 y el consentimiento quedan pendientes y no pasan a trabajos futuros.

<!-- estilo: Descripción -->
Tabla 16. Comprobaciones pendientes de Santiago Benavides Rey al 10 de octubre de 2026 y espacio para sus resultados

| Comprobación | Casos y hoja | Resultado |
| --- | --- | --- |
| Estudiantes jugando: cambio de controles y valores del Nivel 1 | PG-05 y PG-06, H1 y H2 | [Pendiente: estudiantes del Colegio El Libertador IED, fecha, sede y resultado observado] |
| Consentimiento informado y asentimiento | PF-RNF12-01, Anexo F | [Pendiente: visto bueno del colegio y número de estudiantes con ambos documentos firmados] |
| Recorrido completo con cronómetro | PF-RNF13-01, H8 | [Pendiente: duración e incidencias frente a la referencia de 20 a 40 min] |
| Segundo equipo y equipo sin tarjeta gráfica dedicada | PF-RNF07-02 y PF-CT02-01, H3 y H5 | [Pendiente: equipos, cargas y memoria] |
| Ejecución con el adaptador de red deshabilitado | PF-RNF08-01, H4 | [Pendiente: resultado con el adaptador de red deshabilitado]; el arnés no vio conexiones en 4 516 muestras |
| Sonido, vista y mano | PF-SON-01 a PF-SON-03, H6 a H7 y H9 a H13 | [Pendiente: piezas que suenan y piezas que no, y resultado de cada comprobación] |
| Contenido y autorizaciones | PF-RNF22-01 y PF-RNF23-01 | Inspección hecha; [Pendiente: confirmación del contenido y soporte escrito de la Familia Anonaky y de la colaboradora de arte] |

<!-- estilo: Fuente -->
Fuente: elaboración propia a partir de la hoja de comprobaciones humanas (Anexo G).

<!-- estilo: Título 2 -->
## 9.3 Conclusiones del objetivo

<!-- estilo: Normal -->
De los 136 casos, 122 terminan en P, 4 en F y 10 esperan a Santiago Benavides Rey. La eficacia es de 96,8 % frente a la meta de 90 %, y los 45 RF de prioridad Alta están implementados, porque ninguno tiene un defecto Bloqueante o Mayor; el único fallido que toca uno de ellos, PF-RF45-05, es Menor. De las etiquetas que el indicador nombra, NAV (94,1 %) y BOT (100 %) superan la meta y SON no se puede calcular hasta que Santiago Benavides Rey escuche las piezas; la más baja es RNF, con 78,6 %, por textos e iconos fuera de su caja (PF-RNF03-01), identificadores en español (PF-RNF15-01) y commits sin tarjeta (PF-RNF17-01). Los dos indicadores se cumplen en lo medido, y el primero queda abierto en su parte de sonido.

<!-- estilo: Normal -->
Evaluar sobre el ejecutable y no sobre el Editor cambió resultados que la suite no podía mostrar. En la primera pasada aparecieron las conexiones de red del reproductor, porque las pruebas leían la configuración del repositorio y no la que queda dentro del ejecutable, y dos defectos del laberinto, porque las pruebas llaman a los controladores en vez de simular el puntero. Sobre rc3, con el arte final, la familia de perfil y las expresiones por línea, todas las sesiones jugables aprobaron y lo que apareció fue de disposición: iconos y textos fuera de su caja y un doble clic en «Omitir».

<!-- estilo: Normal -->
Lo que la evaluación no pudo observar tiene causa: el arnés no oye, no cronometra a un niño ni cambia de equipo, y PG-05 y PG-06 exigen estudiantes y el consentimiento de sus acudientes. Por eso RNF-07, RNF-08, CT-02, el sonido y la ruta de 20 a 40 min dependen de Santiago Benavides Rey. Los diez casos quedan fuera del denominador; cuando se ejecuten, la eficacia se recalcula con ellos y el indicador de sonidos se cierra con los tres de SON.

<!-- estilo: Título 3 -->
### 9.3.1 Oportunidades de mejora para trabajos futuros

<!-- estilo: Normal -->
Las oportunidades salen de los defectos abiertos, de las observaciones y de lo que quedó sin medir, y se ordenan por su efecto en el estudiante y en el docente. En la interfaz, la instrucción de tres líneas del bosque se sale de su tablilla (DEF-RC3-10); el informe docente necesita una altura propia para los iconos de la cabecera y un botón «Volver al menú» más ancho (DEF-RC3-05), y «Eliminar datos» parte su texto en dos líneas (DEF-RC3-11); el resumen de nivel muestra a Algoritm como un círculo con su nombre y no con su diseño final (DEF-RC3-01). En la mecánica, ignorar la entrada unos 300 ms tras cargar el resumen evita que un doble clic en «Omitir» llegue a «Volver al menú de niveles» (DEF-RC3-09), y una evaluación con estudiantes que pida partidas comparables exige fijar la distribución de las siete piezas del Nivel 1, hoy aleatoria (DEF-RC3-04).

<!-- estilo: Normal -->
La inspección del código y del historial halló tres. RNF-15 no se cumple en ForestSceneController, WorkshopSceneController y MazeSceneController, del Nivel 2, que conservan identificadores en español (DEF-RC3-I01). La trazabilidad entre commit y tarjeta del tablero (RNF-17) es débil: 9 de los 122 commits que tocan Assets llevan tarjeta (7,4 %), frente al 100 % que pide el criterio; no se corrige sin reescribir la historia, pero se evita desde ahora exigiéndola en la plantilla del mensaje (DEF-RC3-I02). Y nueve imágenes de caras conservan un halo tenue de croma (DEF-RC3-I03), que se limpia con la herramienta arte_check.py y copia de respaldo. Las pruebas de arquitectura deberían tolerar un nivel ausente para que RNF-16 pueda declarar cero fallos de acoplamiento (DEF-RC3-12), el laberinto debe correrse en PlayMode sobre el commit que se entregue (DEF-RC3-I04) y el paquete trae DirectML.dll, la carpeta D3D12 y módulos de red de Unity que nada del juego llama.

<!-- estilo: Normal -->
Los pendientes de la Tabla 16 no son trabajo futuro: se cierran con la sesión con estudiantes y las comprobaciones de Santiago Benavides Rey. Como trabajo futuro quedan el residuo que el motor escribe en el registro de Windows (DEF-RC2-01) y la música, el sonido del diálogo, el ambiente nocturno del Nivel 2 y el silencio de la escena final, del carril de sonido. Cuatro observaciones piden corregir el documento de casos y no el juego (DEF-RC3-02, 04, 06 y 07).
