# 2. MÉTODO

## 2.1 El catálogo de casos PF

El instrumento de evaluación es un catálogo de {{N_CASOS}} casos de prueba funcional, identificados con el prefijo PF. El identificador lleva el requerimiento que prueba y un consecutivo: `PF-RF34-02` es el segundo caso de RF-34, y `PF-RNF04-01` el primero de RNF-04. Cada uno de los 47 RF y de los 23 RNF tiene al menos un caso, como exige la restricción CT-10 del proyecto. Se suman un caso de la restricción técnica CT-02 (equipo sin tarjeta gráfica dedicada, `PF-CT02-01`) y tres de sonido (`PF-SON-01` a `PF-SON-03`), porque el indicador de eficacia nombra los sonidos y ninguno de los RNF los prueba directamente.

Cada caso trae su requerimiento, una etiqueta, el ejecutor, un guion paso a paso con el resultado esperado de cada paso y el criterio con que aprueba. La etiqueta principal es una de seis: `NAV` (saltos de nivel y navegación entre pantallas), `BOT` (botones y controles), `SON` (sonido), `RETO` (resolución de la mecánica), `DAT` (perfil, guardado, informe y borrado) y `RNF` (rendimiento, portabilidad, accesibilidad y contenido). Con ellas se lee el primer indicador por lo que nombra: botones, sonidos y saltos de nivel.

El resultado esperado sale, en este orden, del texto del RF o RNF radicado en el OE1, de los criterios de aceptación y flujos alternos de la historia de usuario y del caso de uso que lo trazan, y de la decisión de un hallazgo de consistencia (INC) cuando alguno cambió el comportamiento. Si dos fuentes chocan, manda la de mayor precedencia del proyecto: trabajo de grado, OE1, guion, casos de uso e historias, historias de usuario detalladas y arquitectura. Si un criterio vive solo en una historia de usuario y el juego no lo cumple, el caso falla igual, porque el objetivo evalúa contra «los requerimientos previamente definidos» y las historias forman parte de ellos.

## 2.2 Las sesiones

Los casos se ejecutan en {{N_SESIONES}} sesiones. Cada una es un corte vertical del juego, recorrido de corrido de la pantalla que lo abre a la que lo cierra, y verifica en el camino todos los casos que ese tramo toca. Por eso un caso como RF-13 (la ayuda) aparece en varias sesiones, una por escena jugable, y por eso la Tabla 2.1 cuenta cada caso en la sesión donde se juzga.

**Tabla 2.1.** Sesiones de la evaluación funcional, quién las ejecuta y cuántos casos juzga cada una.

{{TABLA_SESIONES}}

## 2.3 Quién ejecuta cada caso

Hay cinco ejecutores. El ejecutor EXE es un arnés de caja negra, `oe4.ps1`, que maneja `Algoritmia.exe` como lo haría un jugador: lanza el ejecutable tras comprobar su SHA-256, hace clic, clic sostenido y arrastre en coordenadas relativas al área cliente de la ventana, escribe en el único campo de texto del juego (el nombre del perfil) y captura la pantalla. No lee el código ni la memoria del juego; solo ve lo que ve un jugador, y de ahí viene el nombre de caja negra. Mide además la carga de cada escena a partir de la línea `RNF-04` que `SceneLoader` escribe en `Player.log`, la memoria con un muestreo cada 2 s del proceso y las conexiones de red del proceso en el mismo muestreo.

El ejecutor INSP inspecciona archivos: los JSON de `Datos/`, los textos de los assets, el código, el historial de git, la carpeta del build y las capturas ya tomadas. El ejecutor SUITE corre la suite automatizada con el Editor cerrado. El ejecutor EDIT usa el Editor abierto y se reserva para RNF-18, que exige cambiar un parámetro sin recompilar. El ejecutor HUM es Santiago Benavides Rey, con una hoja de pasos que prepara el equipo y de la que se registra solo lo que él reporta (comprobaciones de oído, el recorrido con cronómetro, un segundo equipo y un equipo sin tarjeta gráfica dedicada).

**Tabla 2.2.** Casos por ejecutor, tal como los fija el catálogo.

{{TABLA_EJECUTORES}}

Tres reglas gobiernan las sesiones EXE y evitan que un veredicto se invente. Se observa y no se infiere: después de cada acción que cuenta para un caso se toma una captura y se lee, porque la ausencia de error no prueba que ocurriera lo esperado. Se separa el error del probador del error del juego: si la carretilla choca porque se leyó mal el tablero, no hay defecto. Y durante las sesiones no se toca código, escenas ni assets; la única excepción es RNF-18, y se revierte.

## 2.4 Veredictos

**Tabla 2.3.** Veredictos de un caso y cómo cuentan en el primer indicador.

| Veredicto | Cuándo | Cuenta como |
|---|---|---|
| P, aprobado | Se observan todos los resultados esperados de los pasos que el caso agrupa | Aprobado |
| PD, aprobado con desviación documentada | Se observa lo esperado, pero un paso sigue la decisión de un INC | Aprobado, reportado aparte |
| F, fallido | Algún resultado esperado no se observa; se abre un defecto | No aprobado |
| B, bloqueado | No se pudo ejecutar por precondición o entorno; si lo bloquea un defecto del juego, es F | Fuera del denominador |
| NA, no aplica | Lo que el propio requerimiento deja fuera de un prototipo sin usuarios (RNF-12) | Fuera del denominador |

Durante la pasada un caso puede estar a medias o sin ejecutar. Son estados de trabajo y no veredictos: al cierre cada uno de los {{N_CASOS}} casos tiene uno de los cinco de la Tabla 2.3, y cada B restante se justifica por escrito. Un F pasa a PD solo si el triaje registra un INC con la decisión de Santiago, y la reclasificación se ve en la columna de la versión final sin reescribir la primera pasada.

## 2.5 Indicadores

**Eficacia de las pruebas funcionales.** Se calcula como (P + PD) / (P + PD + F) × 100 sobre los {{N_CASOS}} casos, con meta de 90 % o más. Se publica el valor global y el de cada etiqueta, porque el indicador nombra botones, sonidos y saltos de nivel y un promedio global podría ocultar una etiqueta por debajo de la meta.

**Cumplimiento de requerimientos de prioridad Alta.** Son 45 RF de prioridad Alta; RF-06 es de prioridad Media y RF-21 de prioridad Baja, según el OE1. Un RF de prioridad Alta cuenta como implementado si ninguno de sus casos termina en F con severidad Bloqueante o Mayor. Un F Menor, como un rótulo o un criterio secundario de una historia, resta en la eficacia pero no niega que el RF exista.

## 2.6 El candidato rc3

El objeto de prueba es el ejecutable portable compilado para Windows de 64 bits con Unity 6000.5.10f1 (Mono, sin development build), con las 12 escenas del juego y `Boot` como primera. Los candidatos rc1 y rc2 se compilaron el 01/10/2026 sin etiqueta ni commit, desde el árbol de trabajo de la rama, por decisión de Santiago de hacer un único commit al final. Como el árbol sigue cambiando, la procedencia de cada candidato se identifica con el HEAD sobre el que se compiló, la huella de `git diff HEAD`, la lista de archivos sin seguimiento y la huella del contenido de la carpeta del build. Cada sesión sobre el ejecutable empieza comprobando el SHA-256 de `Algoritmia.exe`; si no coincide con el registrado, la sesión no cuenta. El `.exe` es solo el lanzador, así que lo que distingue un candidato de otro es la huella del contenido de `Algoritmia_Data`.

**Tabla 2.4.** Procedencia del candidato rc3.

| Dato | Valor |
|---|---|
| Fecha y hora del build | PENDIENTE-CIFRA |
| Rama y HEAD | PENDIENTE-CIFRA |
| Huella de `git diff HEAD` | PENDIENTE-CIFRA |
| SHA-256 de `Algoritmia.exe` | PENDIENTE-CIFRA |
| Huella del contenido de la carpeta | PENDIENTE-CIFRA |
| Número de escenas en el build | PENDIENTE-CIFRA |
| Equipo 1 | AMD Ryzen 5 7600X, 32 GB de RAM, NVIDIA GeForce RTX 5070 Ti de 16 GB, Windows 11 Home Single Language de 64 bits, monitor de 1920 × 1080 al 125 % (el mismo de las pasadas sobre rc1 y rc2) |
| Condiciones | Editor de Unity cerrado en las sesiones que miden carga o memoria; nada más abierto |

Los perfiles de las sesiones guionizadas parten de semillas copiadas del catálogo, con cifras distintas entre sí a propósito, para que en el informe docente se sepa de qué fila viene cada número. Los indicadores de una fase solo se guardan la primera vez que se confirma, así que ninguna sesión cuyas cifras se comprueben reutiliza un perfil ya jugado.

## 2.7 Defectos y correcciones

Cada F abre un defecto con su caso, la versión en que se halló, la severidad, la reproducción mínima, el resultado esperado y el observado, la evidencia y la decisión de Santiago. La severidad es Bloqueante cuando impide terminar la ruta principal, cierra el juego, pierde progreso confirmado o deja un estado del que no se sale; Mayor cuando no se cumple un RF o un criterio de aceptación pero el juego sigue; y Menor cuando afecta un texto, el aspecto o un criterio secundario sin efecto sobre el reto. Los Bloqueantes y Mayores se corrigen dentro del OE4 salvo decisión contraria de Santiago; los Menores los decide él. Tras cada tanda de correcciones se compila un candidato nuevo y se repiten la suite completa, el caso fallido y todos los casos de su sesión.

Las discrepancias entre el juego y un documento radicado no se corrigen en el documento: se proponen como hallazgos nuevos en `INCONSISTENCIAS.md`, para que los autores editen el documento a mano.
