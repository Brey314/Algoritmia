# Correcciones a las actas, 06/10/2026

Encargo de Santiago Benavides Rey: corregir las actas tomando como fuente de verdad la más reciente. Cuando dos actas se contradicen en un dato (identificador, nombre de archivo, código de tarjeta, encabezado, contador, fecha de un mismo hecho, responsable de una tarjeta o nombre de algo), prevalece la posterior y se corrige la anterior. Se recorrió de la D10 (30/09/2026) al seguimiento 01 (10/04/2026). Series: seguimiento 01 a 10, OE1 serie R, OE2 serie O, OE3 serie D.

No se tocó ninguna decisión que cambió con el tiempo, ningún estado registrado en su fecha ni ningún dato sin fuente posterior. No se inventó contenido: donde una acta posterior no da el dato, queda como está y figura en el apartado 4.

## 1. Archivos renombrados o movidos

| Origen | Destino | Motivo |
|---|---|---|
| `OE1/Acta_R04_2026-06-13.md` | `OE2/Acta_O04_2026-09-05.md` | El contenido es el acta O04, del 05/09/2026 (serie del segundo objetivo). |
| `OE1/Acta_R05_2026-06-20.md` | `OE2/Acta_O05_2026-09-06.md` | El contenido es el acta O05, del 06/09/2026. |
| `OE1/Acta_R001_2026-07-04.md` | `OE1/Acta_R01_2026-06-13.md` | El encabezado dice R01, sábado 13 de junio. |
| `OE1/Acta_R02_2026-07-11.md` | `OE1/Acta_R02_2026-06-20.md` | El encabezado dice R02, sábado 20 de junio. |
| `OE3/Acta_D09_2026-09-24_Plana.md` | `OE3/Acta_D09_2026-09-24.md` | Se quita el sufijo para seguir el patrón de las demás. Ningún archivo del repositorio enlazaba el nombre con `_Plana`. |
| `OE3/Acta_D10_2026-09-30_Plana.md` | `OE3/Acta_D10_2026-09-30.md` | Igual que la D09. |

Los demás nombres se compararon con el número y la fecha de su encabezado y coinciden (seguimiento 01 a 10, R03, R06 a R10, O01 a O03, D01 a D08). No se crearon las R04 y R05 verdaderas: por la R03 («Sábado 4 de julio») y por las tarjetas R05-1 y R05-2 que cierra la R06 corresponderían al 04/07 y al 11/07/2026, y no existen en el repositorio.

## 2. Cambios en el contenido

El contador «NN de 10» sale de la D10 («D10 de 10»), de la serie R («R01 de 10» a «R10 de 10») y del acta 10 de seguimiento, que cierra el periodo con las actas «N.º 01 a N.º 10». La serie O se fija en 5 (O01 a O05).

| Acta | Qué decía | Qué dice ahora | Fuente posterior |
|---|---|---|---|
| Seguimiento 01 a 10 | Casilla «Acta N.º» con solo el número (01) | «01 de 10», y así hasta «10 de 10» (10 cambios, uno por acta) | Serie R, que usa «N de 10»; acta 10, «N.º 01 a N.º 10» |
| R08 | §8: «Se cierra la serie [...] entre el 11 de junio y el 1 de agosto de 2026» | La serie sigue abierta, se abrió el 13 de junio y se cierra el 16 de agosto; siguiente sesión el sábado 15 de agosto para releer y refinar el borrador | R01 (13/06), R09 (15/08, «Borrador [...] 1/08/2026»), R10 §8 («entre el 13 de junio y el 16 de agosto») |
| O03 | `O03 de 03` | `O03 de 05` | O01, O02, O04 y O05 («de 05») |
| O03 | Serie: «[...] historias de usuario y criterios de aceptación» | Texto de serie de O01, O02, O04 y O05 («[...] historias de usuario, arquitectura y preparación del desarrollo») | O04, O05 |
| O03 | «las seis facetas» con seis nombres | «las siete facetas» con «reconocimiento de patrones» añadido | La tabla de la propia O03 y su decisión final dicen 7; R10 §3 enumera las siete |
| O04 (antes `OE1/Acta_R04`) | Movimientos con `O03-1`, `O03-2`, `O03-3`; la primera descrita como «propuestas de personajes y del personaje guía» | `D01-1` («propuestas de entornos»), `D01-2`, `D01-3` | D02 §6 usa D01-1 a D01-3; D03 y D04 tratan los bocetos de entornos como esa entrega |
| O04 | `O03-3` (ahora D01-3, carpeta de assets) «En proceso → Finalizado» | `D01-3`, «Permanece en proceso» | D02 (misma fecha, «Permanece en proceso»), D03 §7 («la carpeta de assets [...] sigue sin crearse») y D04 (D01-3 «Permanece en proceso») |
| O05 (antes `OE1/Acta_R05`) | Siguiente sesión: «entrega del primer lote de assets» | «segundo lote de assets» | D03-2 (06/09) y D04 §3 («segundo lote de bocetos de entornos») |
| D01 | Título «N.º O03», casilla «O03 de 05» | «N.º D01», «D01 de 10» | D02 a D10 («D02 de …», serie D); CLAUDE.md y D02 sitúan la D01 el 02/09 como primera de la serie |
| D01 | Serie: «del segundo objetivo específico — [...] arquitectura y preparación del desarrollo» | Serie del tercer objetivo, con el texto de D02 a D10 | D02 a D10 |
| D01 | Objetivo específico: «OE3 — Desarrollo del videojuego» | «OE3 — Desarrollar el prototipo funcional del videojuego» | D02 a D10 |
| D01 | Tarjetas `O03-1`, `O03-2`, `O03-3` (tabla de compromisos y de movimientos) | `D01-1`, `D01-2`, `D01-3` (6 filas). El código O03 ya designaba las tarjetas del acta O03 del 30/08 | D02 (movimientos D01-1 a D01-3), D03 y D04 (D01-3) |
| D01 | Movimiento de `O03-1`: «propuestas de personajes y del personaje guía» (la tabla de compromisos decía «entornos») | «propuestas de entornos» en las dos tablas | D02 lo copió con «personajes», pero D03 (06/09: «primeros bocetos de entornos», D03-2 «segundo lote de entornos») y D04 («segundo lote de bocetos de entornos») fijan que la primera entrega fue de entornos; el calendario de la propia D01 pone los personajes el 16/09 |
| D01 | Calendario: «Miercoles 7 de Octubre», «1:00 p.m» | «Miércoles 7 de octubre», «1:00 p. m.» (ortografía y formato de hora de las demás filas; la fecha no se cambia, ver apartado 4) | Formato de hora de D02 a D10 |
| D02 | Contador `D02 de 03` | `D02 de 10` | D10 |
| D02 | `D01-1`: «propuestas de personajes y del personaje guía» | «propuestas de entornos» | D03 y D04, como arriba |
| D03 | `D03 de 03` | `D03 de 10` | D10 |
| D04 | `D04 de 04` | `D04 de 10` | D10 |
| D05 | `D05 de 05` | `D05 de 10` | D10 |
| D05 | §6: primera tarjeta `D04-4` (y no hay `D05-1`) | `D05-1` (texto, responsable y fecha sin cambios) | La D04 solo tiene D04-1 a D04-3; cada acta de D06 a D10 numera sus tarjetas con su propio número desde 1; la D05 ya trae D05-2; `Slice-2-Resultados.md` la llama D05-1 |
| D06 | `D06 de 06` | `D06 de 10` | D10 |
| D07 | `D07 de 07` | `D07 de 10` | D10 |
| D07 | `D06-1` con responsables «Santiago Benavides Rey y Santiago Valdiri García» | «Sofía Valentina Giraldo Segovia» | D06 §6 (origen de la tarjeta), D09 §6 |
| D07 | §7 remite a `D07-3`, ausente de su §6 | Fila `D07-3` añadida con el texto, responsables y fecha de la D09: «Continuar el seguimiento del desarrollo de los sprites con la colaboradora», Santiago Benavides Rey y Santiago Valdiri García, domingo 27/09/2026 | D09 §6; `Slice-3-Resultados.md` la registra como abierta por la D07 para ambos estudiantes con límite 27/09 |
| D07 | «Santiago Valdiri presenta el avance» | «Santiago Valdiri García» | Nombre completo en todas las actas |
| D08 | `D08 de 08` | `D08 de 10` | D10 |
| D08 | `D06-1` con responsables «Santiago Benavides Rey y Santiago Valdiri García  Sofía Valentina Giraldo Segovia» (mezcla, con doble espacio) | «Sofía Valentina Giraldo Segovia» | D09 §6 |
| D09 | `D09 de 09` | `D09 de 10` | D10 |
| D09 | Tres menciones de «entrega del segundo objetivo» el 25/09 | «entrega del tercer objetivo» (§3, §5 y §8) | `claudeDocs/entregables/OE3/src/12-control-cambios.md`: 25/09/2026, «Entregable inicial objetivo específico 3» |
| D10 | §6 lista D10-1 a D10-7, mientras su plan (§4, parada P3) y su adenda hablan de D10-1 a D10-8; la D10-7 mezclaba el entregable con el capítulo 8 | D10-7 pasa a ser el entregable del tercer objetivo y se añade D10-8 con el texto que tenía la D10-7 (capítulo 8 y radicados). Responsable y fecha como las demás | D10 §4 («entregable (D10-7) y [...] trabajo de grado (D10-8)»), §9 («D10-1 a D10-8 quedan cumplidas»); `02-metodologia.md`: «ocho tarjetas de la D10 [...] este entregable y el capítulo 8» |

Cambios por acta: seguimiento 01 a 10, 1 cada una; R08, 1; O03, 3; O04, 3; O05, 1; D01, 11; D02, 2; D03, 1; D04, 1; D05, 2; D06, 1; D07, 4; D08, 2; D09, 4; D10, 1 (la sustitución de la fila D10-7 y el alta de D10-8 cuentan como uno). R01, R02 y D10 solo cambian de nombre en el resto. Total: 47 sustituciones en 24 archivos modificados, más los seis renombres.

La D10-7 reescrita («Reescribir el entregable del tercer objetivo al estado vigente del prototipo con el generador recuperado: documento principal y anexos A a G») se redactó con lo que dicen el §3 («se reescriben el entregable del tercer objetivo [...] y los anexos A a G») y la adenda; ningún acta posterior la transcribe, así que conviene que Santiago la confirme.

## 3. Pendiente para el `.docx` de la D01

`docs/actas/OE3/Acta_D01_2026-09-02.docx` no se tocó. Su texto es el mismo que tenía el `.md` antes de los cambios, y habría que hacer lo siguiente:

1. Título: «ACTA DE SESIÓN DE TRABAJO N.º O03» pasa a «… N.º D01».
2. Línea «Serie»: «segundo objetivo específico — diseño de la estructura pedagógica y narrativa, historias de usuario, arquitectura y preparación del desarrollo» pasa a «tercer objetivo específico — desarrollo del prototipo funcional: assets, arquitectura, planeación por incrementos y ejecución del código».
3. Tabla de datos: «Acta N.º: O03 de 05» pasa a «D01 de 10».
4. Tabla de datos: «Objetivo específico: OE3 — Desarrollo del videojuego» pasa a «OE3 — Desarrollar el prototipo funcional del videojuego».
5. §4, última fila del calendario: «Miercoles 7 de Octubre | 1:00 p.m» pasa a «Miércoles 7 de octubre | 1:00 p. m.».
6. §6, tabla de compromisos: los códigos O03-1, O03-2, O03-3 pasan a D01-1, D01-2, D01-3.
7. §6, tabla de movimientos: los mismos tres códigos pasan a D01-1, D01-2, D01-3, y la descripción de la primera pasa de «Preparar las primeras propuestas de personajes y del personaje guía conforme a la dirección de arte» a «Preparar las primeras propuestas de entornos conforme a la dirección de arte».

## 4. No se corrigió

| Asunto | Por qué se deja |
|---|---|
| R08 cuenta 23 RNF y da por cumplido el primer objetivo el 01/08; R10 cuenta 22 y lo da por cumplido el 16/08 | Cambio legítimo del documento de requerimientos entre sesiones (la R09 trata el 01/08 como borrador). Es historia. |
| Hallazgos «46 a 51» (D08), «46 a 53» (D09) y «46 al 54» (D10) | Estado del registro en cada fecha; el 54 es del 25/09 (la cuerda del taller). |
| «Algoritm» como título provisional (D03, D04, D05) frente a «Algoritmia» (D10, decisión D2) | La decisión cambió con el tiempo. «Algoritm» sigue siendo el nombre del guía. |
| Cifras de pruebas de cada acta (221 de 221, 321 de 321, 390 de 391) | Estado registrado en su fecha. |
| Texto de D07-2 y D08-2 reformulado en D09 y D10 (alcance que queda, «Continúa …»); D07-2 pasa de Valdiri a Benavides | La D08 documenta ese traspaso; las reformulaciones reflejan el estado de la tarjeta. |
| D04 fija la entrega de entornos a 2048 × 1152; D06 dice que el 9 de septiembre se fijó 1920 × 1080, ampliable a 3840 × 1080 | Contradicción real, pero D04 registra el dato con su razonamiento (137 MB, techo de importación) y la D06 no dice que lo sustituya. Reescribir cifras de una decisión pide el criterio de Santiago. |
| D05 anuncia la sesión del lunes 14/09 y la sesión con la colaboradora fue la D06, del martes 15/09 | Es una previsión, no un hecho; la D06 no la cita. |
| O02 anuncia «Miércoles 2 de septiembre» como siguiente sesión y entre medias se hizo la O03 (30/08) | Previsión anterior a un cambio de calendario. |
| D01 dice «hasta el 30 de septiembre» y su calendario trae una sesión el 07/10 | Ninguna acta posterior aclara si el 07/10 era sesión de revisión o fecha de entrega final. Solo se corrigió la ortografía. |
| D01 mueve O02-1 a «Finalizado» el 02/09 y la O03 ya lo había hecho el 30/08 | La O03 lo justifica en su texto; no hay acta posterior que lo resuelva. Queda como repetición. |
| Tarjetas O03-1 y O03-2 de la acta O03 (radicar el entregable, abrir la serie del OE3) no se cierran en ningún acta; O03-3 no figura en los movimientos de la O03 y la D02 la cierra desde «En proceso» | Falta el dato posterior. |
| D03-4 y O05-2 («Continuar las sesiones de revisión y entrega de assets de los miércoles hasta el 30 de septiembre») son la misma tarjeta abierta en dos series el 06/09 | Duplicado por la superposición de las series O y D; ver el punto siguiente. |
| Alcance de las series O y D entre el 30/08 y el 06/09: la O03 dice que arquitectura, dirección de arte y planeación pertenecen al tercer objetivo; la O04 y la O05 (segundo objetivo) las tratan y cierran el segundo objetivo otra vez el 06/09; la D02 (tercer objetivo) planea los cuatro slices el 05/09 | Cambia el reparto de contenido entre objetivos, no un dato. Decide Santiago, y afecta al capítulo 5 del trabajo de grado, que ya describe el solape. |
| D06 describe el montón de hojas en «cuatro estados» (§3) y «dos estados con un clip de cruce» (§5) | Contradicción interna sin acta posterior que la resuelva. |
| Horas «[por completar]» de la D09 y la D10, y la «Siguiente sesión» de la D10 | Sin fuente posterior. |
| D10 §6 no lista D06-1, D07-3, D08-1, D09-2, D09-3 ni D09-4 | La D10 §7 declara que no revisó las cuatro últimas; la D08-1 la cierra la D09. No es un dato en conflicto. |
| Actas R04 y R05 verdaderas (04/07 y 11/07/2026) y tarjetas R04-1 y R04-2 | No existen en el repositorio; no se reconstruyen. |
| D01, fase «Diseño — momento 4» frente a «Desarrollo» desde la D02 | Estado de la D01 en su fecha. |

## 5. Menciones fuera de `docs/actas` que quedan desactualizadas

No se editaron. Se listan archivo y línea.

| Archivo:línea | Qué dice | Por qué queda vieja |
|---|---|---|
| `claudeDocs/entregables/OE3/src/02-metodologia.md:58` | «el tablero de la D05 registra como tarjeta D04-4» | Ahora es D05-1. |
| `claudeDocs/entregables/OE3/src/02-metodologia.md:64` | La D01 «conserva ese número en la cabecera (acta «O03 de 05»)»; «la D02 y la D03 se encabezan «de 03», y desde la D04 el total es el número del acta» | Las cabeceras de D01 a D09 dicen ahora «de 10» y la D01 lleva su código. |
| `claudeDocs/entregables/OE3/src/02-metodologia.md:70` | D01 «abre O03-1 a O03-3» | Ahora D01-1 a D01-3. |
| `claudeDocs/entregables/OE3/src/02-metodologia.md:74` | «D04-4 (Slice 2 mientras se cierra el Slice 1)» | Ahora D05-1. |
| `claudeDocs/entregables/OE3/src/02-metodologia.md:76` | D07 «abre D07-1 (Slice 3) y D07-2 (sonidos)» | La D07 trae también D07-3 en su §6. |
| `claudeDocs/entregables/OE3/src/02-metodologia.md:79` | «abre D10-1 a D10-7, con la D10-8 citada en el plan y en la adenda del 01/10» | La D10 §6 lista ya D10-1 a D10-8. |
| `claudeDocs/entregables/OE3/src/anexos/C-slice-2.md:24` | «La tarjeta que abrió el trabajo fue D04-4 [...] que el acta D05 registra» | Ahora D05-1. |
| `claudeDocs/entregables/TG/cap5-fases.md:58` | «el código O03 designó dos juegos de tarjetas distintos [...] la D01, cuyo encabezado conserva ese código [...] La O04 todavía cita con el código O03 las tarjetas de la D01, y es el único caso de código repetido» | La D01 lleva D01 y la O04 cita D01-1 a D01-3. |
| `claudeDocs/entregables/TG/cap5-fases.md:70` | «arrastró la tarjeta D04-4» | Ahora D05-1. |
| `CLAUDE.md:126` | «La serie `OE2/` (`O01..O03`) no se rehizo: [...] `O03-1` aparece como `D01-1` después» | La serie OE2/ es O01 a O05 y las tarjetas de la D01 ya se llaman D01-x en la propia D01. |
| `claudeDocs/tasks/Slice 2/Slice-2-Resultados.md:84` | Llama D05-1 a la tarjeta del Slice 2 con otro texto y otra fecha (27/09) que la D05 | Ahora coincide el código, pero no el texto ni la fecha de la acta. Es un documento de resultados, no se reescribe. |

Sin cambio necesario: `B-slice-1.md:22` (O03-3 es la tarjeta de inicialización de la O03), `E-arte-y-sonido.md:25` y `F-personajes.md:14,19` (D07-3 ya coincide con la D07 corregida), `TG/cap5-fases.md:10,38,44,54,64` y `claudeDocs/INCONSISTENCIAS.md:2667` (D10-8 como tarjeta del trabajo de grado).
