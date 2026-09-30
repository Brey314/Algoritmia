# 10. LIMITACIONES Y TRABAJO PENDIENTE

Al cierre del objetivo específico 3 el prototipo implementa los 47 requerimientos funcionales, no solo los 45 de prioridad alta (Anexo G, §1), y la suite automatizada supera 664 de sus 667 casos (véase §9.3). Lo que queda abierto no impide recorrer el juego de principio a fin, pero condiciona la entrega y debe declararse. Se agrupa en cinco frentes: diferencias entre el prototipo y los documentos radicados, verificaciones que solo pueden hacerse sobre el ejecutable, sonido y arte por entregar, deuda técnica menor conocida y lo que pasa al objetivo específico 4. Todo lo que sigue está registrado en los documentos de resultados (Anexos B a G), en el registro de inconsistencias del proyecto o en las actas de seguimiento (Anexo H).

## 10.1 Hallazgos de consistencia abiertos

El proyecto registra en un documento propio cada conflicto entre los documentos fuente, o entre ellos y lo implementado, junto con la corrección aplicada. Los hallazgos INC-01 a INC-45 están cerrados; al 25/09/2026 siguen abiertos nueve, de INC-46 a INC-54. En siete de ellos el prototipo ya aplica la decisión tomada y lo que falta es corregir el documento. En seis se trata de documentos radicados, que se corrigen a mano sobre el original y nunca desde el código, y las correcciones que modifican un RF o un criterio de aceptación exigen además la aprobación expresa de los autores; en INC-53 se trata de la dirección de arte del proyecto (§13.1). Los dos restantes esperan una decisión de diseño visual: INC-46, entre dos descripciones distintas de la lista de tareas del Nivel 3, e INC-52, entre la dirección de arte y el arte entregado. La Tabla 10.1 los enumera.

**Tabla 10.1.** Hallazgos de consistencia abiertos al 25/09/2026.

| INC | En qué se aparta | Estado en el prototipo |
|----|------------------------------|------------------------------|
| INC-46 | La lista de tareas del Nivel 3 se describe como cuerda con nudos en la dirección de arte y como panel de casillas en el plan del slice | Lista implementada y probada (RF-36); su forma visual definitiva está por decidir |
| INC-47 | El Nivel 1 reúne los materiales en un círculo y mide fuerza y cercanía de las piedras en dos deslizantes, no la posición de RF-15 y RF-16 | Aplicado; falta corregir guion §1.4.3 y §1.4.4, RF-15, RF-16 y HU-06 |
| INC-48 | El §4 del documento de diseño refundido conserva la arquitectura anterior a su alineación | El código sigue la arquitectura alineada; falta corregir el documento |
| INC-49 | El menú de pausa es el del mockup 6 (Reanudar, Reiniciar, Volver al menú de niveles) | Aplicado; falta corregir HU-17 |
| INC-50 | «Empujar» cierra la fase del bosque y el rodado de la caja se ve en la escena narrativa 2.2 | Aplicado; falta corregir RF-26, HU-08 y CU-06 |
| INC-51 | El cierre del Nivel 3 no se puede omitir al repetirlo | Aceptado; falta corregir HU-14 FA-01 |
| INC-52 | El guía entregado es una llama con extremidades, no la estrella de cinco puntas de la dirección de arte | Se usa el arte entregado; las formas de rueda y gota son provisionales |
| INC-53 | Los personajes se animan por recorte con la interfaz de Unity, no con el paquete 2D Animation | Aplicado; falta corregir §13.1 de la dirección de arte |
| INC-54 | El taller del Nivel 2 suma una cuerda como séptima pieza y un último paso, amarrar la caja | Aplicado; falta corregir guion §1.6.2, RF-27, RF-29, la historia de la fase 2 y CU-07 |

Siguen abiertos también tres residuos menores de redacción (INC-24-r, INC-44-r, INC-44-r2) y tres puntos del guion: PG-01, el título del producto; PG-05, verificar en pruebas que el cambio de esquema de control entre niveles no confunde; y PG-06, validar jugando los valores del Nivel 1. El nombre del guía, PG-02, consta como cerrado en el guion con el nombre Algoritm (INC-44), pero dos de esos residuos, INC-44-r e INC-44-r2, son restos de su denominación provisional en el mismo documento, y el acta D09 registra que, igual que el título, sigue sin cerrarse formalmente; el cierre de ambos es una casilla abierta del cuarto slice (Anexo G, §8).

## 10.2 Verificaciones pendientes sobre el ejecutable

Los presupuestos de rendimiento son requisitos que se miden, no que se estiman, y varias de sus casillas solo pueden cerrarse sobre la versión portable, no dentro del editor. El acta D09 (24/09/2026; Anexo H) las deja abiertas, los documentos de resultados las detallan y el estado de cada presupuesto se presenta en §8.1:

- **Memoria (RNF-05).** La medición automatizada dentro del editor no es representativa: el 25/09/2026 el editor reservaba 2 652 MB antes de cargar ningún nivel (véase §9.3). El límite de 2 GB debe comprobarse con el ejecutable.
- **Carga, paquete y residuos (RNF-04, RNF-06, RNF-07, RNF-11).** La última medición del paquete, 217 MB con once escenas y el arte de tres slices, es del 21/09/2026 (Anexo D). Es anterior a la duodécima escena, la del informe docente, y al sonido y el arte incorporados entre el 21 y el 25/09/2026, entre ellos los personajes animados (véase §8.1). Falta repetir las mediciones de carga, memoria y tamaño con la versión final, ejecutar el programa y comprobar que su carpeta de datos nace junto al ejecutable sin dejar residuos fuera de ella.
- **Equipo de referencia, portabilidad y red (CT-02, RNF-07, RNF-08).** Ejecutar el prototipo en un equipo sin tarjeta gráfica dedicada (CT-02), desde la carpeta portable en al menos dos equipos distintos (RNF-07) y con el adaptador de red deshabilitado (RNF-08).
- **Paquetes de inteligencia artificial del editor.** La compilación del 21/09/2026 incluyó en el paquete bibliotecas de los paquetes de IA del editor (`DirectML.dll`, de 14 MB, y la carpeta `D3D12`), que siguen declarados en el manifiesto del proyecto. Retirarlos es una decisión pendiente de los autores antes de la entrega, a la luz de RNF-08 y RNF-10, que exigen funcionar sin conexión a internet y conservar los datos en el equipo, sin transmisión por red a terceros (Anexo D, §8). También falta retirar a mano del entregable la carpeta de depuración `My project_BurstDebugInformation_DoNotShip`, de 1 MB.

## 10.3 Sonido y arte por entregar

La incorporación del sonido avanzó por niveles (véase §7.3): el Nivel 1 entró el 21/09/2026, el Nivel 2 el 23/09/2026 y el Nivel 3 el 25/09/2026 (Anexo E). Lo que falta está asignado a las tarjetas D07-2 y D08-2 del tablero, con fecha límite del 27/09/2026:

- **Música.** No se ha entregado ninguna pieza musical. El silencio del guion que corta la música cuando el guía se apaga en la escena 1.1 ya está cableado, pero no tendrá efecto audible mientras no exista la música del nivel.
- **Sonido del diálogo.** No se ha entregado el sonido breve y único que debe acompañar la escritura del texto en todos los diálogos, en lugar de una locución.
- **Ambiente nocturno del Nivel 2.** No se ha entregado el ambiente del refugio junto al fuego; la escena 2.5 y el arranque del puente hacia el Nivel 3 usan los ambientes de la cueva del Nivel 1. Otra pieza nocturna está en disco sin que la use ninguna escena.
- **Hundimiento de la balsa.** Por decisión registrada en la dirección de sonido el 25/09/2026, la balsa que se hunde no suena, igual que una fase que no pasa, en aplicación de la regla de no emitir ningún sonido de fallo (CP-02). El inventario de la dirección de sonido prevé en cambio para ese momento una pieza descriptiva y no punitiva, y la salpicadura que podría cumplir ese papel está en disco sin que ningún recurso la referencie.
- **Piezas propias de cada nivel.** Ninguna de las piezas de los Niveles 2 y 3 que nombra el inventario de la dirección de sonido llegó con su nombre. Las suplen piezas globales de encaje, martillo y toma de pieza y, en el Nivel 2, tres piezas entregadas con nombres que no figuran en el inventario: los troncos, la piedra que cae y la carretilla.
- **Puntos abiertos de la dirección de sonido (PS-01 a PS-05).** Entre ellos, si se añade un control de volumen, que ningún RF pide, y si RNF-23 debe mencionar también los recursos sonoros.

En el arte, las formas de rueda y de gota del guía son provisionales hasta que llegue su versión definitiva, que entrará sustituyendo el archivo con el mismo nombre (INC-52). El acta D09 registra además objetos que el guion nombra y que al 24/09/2026 no tenían sprite propio, así como la separación del tronco y los amarres del Nivel 3 en sprites independientes. Los iconos de los cuatro indicadores del informe docente siguen siendo bocetos (Anexo G, §8).

## 10.4 Deuda técnica menor conocida

Los documentos de resultados registran, sin corregirlos todavía, los siguientes puntos. Ninguno afecta la ruta principal del juego:

- **Prueba sensible al tiempo.** `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos` puede fallar en corridas largas porque un cuadro lento del editor hace avanzar el hundimiento más de lo que la prueba admite; pasa ejecutada sola (véase §9.3).
- **Desplazamiento con arrastre.** La pantalla de créditos (Anexo B, Fase 1) y las dos listas del informe docente (Anexo G, §A.4) se desplazan con el componente de desplazamiento de la interfaz de Unity, que admite arrastre, mientras la regla del proyecto limita la entrada a clic y clic sostenido y pide desplazar con botones las listas que desbordan.
- **Tipografía del informe docente.** Los 20 textos de la pantalla del informe y la etiqueta del botón «Progreso del equipo» del menú principal usan la fuente integrada de Unity y no las tipografías del juego (Anexo G, §A.3).
- **Comentarios vencidos en el código del Nivel 1.** Dos comentarios del registrador de indicadores citan todavía el antiguo deslizante de posición, eliminado al rediseñar la mecánica (INC-47), y el del registro de retroalimentación describe un historial que el estudiante ya no ve, pues hoy solo se muestra el último mensaje (Anexo B, Fases 5 y 6, §6).
- **Pulso del botón de pista.** En el Nivel 1 el pulso que llama la atención sobre «Pista» está activo desde que se abre la escena, y no solo tras tres intentos fallidos; en los Niveles 2 y 3 el botón no pulsa en ningún momento (Anexo B, Fases 5 y 6, §6; Anexo F).
- **Nombre de archivo.** La imagen de la caja vacía de la escena 2.2 lleva una tilde en su nombre, contra la convención de nomenclatura de la dirección de arte (Anexo E).

## 10.5 Lo que pasa al objetivo específico 4

El objetivo específico 4 evalúa el prototipo mediante pruebas funcionales basadas en los requerimientos; su cronograma incluye el plan de pruebas, la aplicación de las pruebas funcionales, la verificación del cumplimiento de requerimientos con una matriz de trazabilidad y el registro de correcciones. Este documento le entrega como punto de partida una suite automatizada que nombra los 47 RF, la matriz del Anexo A derivada de ella y los resultados de la corrida del 25/09/2026. Quedan explícitamente para esa fase:

- **Pruebas manuales de la especificación:** presupuestos de rendimiento sobre el ejecutable, ejecución portable en dos equipos, ejecución sin red y cierre forzado con recuperación, además de las verificaciones de §10.2.
- **Golden Path manual:** recorrer el juego entero dos veces, de la pantalla de inicio a los créditos, cronometrando que tome entre 20 y 40 minutos; los tramos están automatizados (véase §9.2), pero el cronómetro lo lleva quien juega.
- **Observación con estudiantes:** PG-05, que el cambio de esquema de control entre niveles no confunda, y PG-06, los valores del Nivel 1 validados jugando, sujetos al consentimiento informado que exige RNF-12. También queda para esa fase la inspección integral del contenido (RNF-22), que es documental y manual.
- **Decisión de alcance pendiente:** si la consulta del docente requiere protección de acceso, algo que ningún RF pide y que el plan del Slice 4 no añadió (Anexo G, §8).

Antes de esa fase, los autores deben cerrar además las casillas de revisión de los puntos de control de cada slice, que ninguna prueba automatizada marca. Los indicadores del objetivo específico 4, el 90 % de eficacia en las pruebas funcionales y la implementación de todos los requerimientos de prioridad alta, se medirán sobre este mismo prototipo.
