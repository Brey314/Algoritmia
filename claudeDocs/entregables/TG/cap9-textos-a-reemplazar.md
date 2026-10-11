# Textos exactos del capítulo 5 que hay que reemplazar al insertar el capítulo 9

Extraídos el 10/10/2026 de docs/md/Trabajo_de_Grado_Entrega_Plantilla_28jul.md (conversión del .docx de 399e149). Son insumo para preparar pares de reemplazar_parrafos.ps1 (antes -> después); el texto del .md puede traer escapes de markdown (por ejemplo una barra invertida antes de un guion bajo) que no están en Word, así que se contrastan con -DryRun. Este archivo no es parte del trabajo de grado.

## 5.1 (tercer párrafo): la frase «todavía no tiene capítulo»

La correspondencia con los objetivos es la siguiente. Las dos primeras fases y la escritura del guion se hicieron antes de la serie R y se narran en los apartados 5.1.1 y 5.1.2; el guion forma parte del documento de diseño del objetivo 2 (capítulo 7), de modo que los apartados 5.1.2 y 5.1.4 describen dos momentos de la misma fase. La Especificación es el objetivo 1 y el capítulo 6 (5.1.3), el Diseño es el objetivo 2 y el capítulo 7 (5.1.4), el ciclo Diseño ↔ Desarrollo ↔ Pruebas es el objetivo 3 y el capítulo 8 (5.1.5), y la evaluación del prototipo es el objetivo 4, que todavía no tiene capítulo (5.1.6).

## 5.1.6, párrafo 1 (probablemente se conserva)

El objetivo 4 no tiene serie de actas propia. Lo que el archivo de actas registra de esta fase está en la D10 y en su adenda del 01/10/2026, que la tratan como parte del ciclo y no como una fase aparte, y en los documentos de resultados del objetivo que la adenda cita.

## 5.1.6, párrafo 2: cifras de rc2 (103 casos, 89 P, 13 P parcial, 24 sin ejecutar)

La D10 recoge que el 29/09/2026 entró el plan de pruebas, con unos 120 casos y seis puntos de control, y fija cómo se ejecuta: sobre el ejecutable compilado y con un arnés que lo maneja como caja negra, e incluye dos recorridos completos de la ruta principal, cierres forzados en seis momentos, borrado de un perfil, comprobación de residuos fuera de la carpeta de datos, ejecución desde una ruta con espacios, tres mediciones de la carga de cada escena y la prueba de exclusión de RNF-16. La D10 preveía un segundo candidato si la primera pasada hallaba un defecto bloqueante o mayor, y la adenda registra que esa pasada, sobre el candidato rc1, halló defectos que se corrigieron antes de la entrega: entre ellos, las conexiones de red de los servicios de Unity, que ahora se apagan al compilar. El primer ejecutable de doce escenas pesaba 866,7 MB y quedó en 479,0 MB. El segundo candidato, rc2, se reverificó con un tercer recorrido completo, y quedaron 103 casos con veredicto: 89 con P, 13 con P parcial, ninguno con F y uno con NE. Otros 24 casos siguen sin ejecutar, entre ellos todos los de verificación manual.

## 5.1.6, párrafo 3: casillas pendientes y sesión con estudiantes

En el tablero, la D10 cerró los cuatro tableros de slices salvo ocho casillas que dependen de uno de los autores: la observación de estudiantes jugando (PG-05 y PG-06) y tres ejecuciones: en un segundo equipo (RNF-07), con el adaptador de red deshabilitado (RNF-08) y en un equipo sin tarjeta gráfica dedicada (CT-02). Se marcarán cuando entregue los resultados y no pasan a trabajos futuros. Para la sesión con estudiantes (apartado 5.3) tiene un guion y el formato de consentimiento informado (RNF-12). En términos de Árcade, esa sesión es la parte del ciclo que el marco teórico asigna a la técnica Thinking Aloud (apartado 3.2.3.2), y a la fecha de la adenda sigue pendiente.
