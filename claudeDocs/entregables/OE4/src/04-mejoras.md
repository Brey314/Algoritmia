# 4. OPORTUNIDADES DE MEJORA PARA TRABAJOS FUTUROS

El objetivo pide identificar oportunidades de mejora, no solo contar casos aprobados. Este capítulo recoge lo que la pasada rc3 dejó abierto, cada punto con su origen y su causa, agrupado por el tema donde habría que trabajar. Entran cuatro clases de hallazgo: los defectos Menores y las Observaciones de la Tabla 3.6, cuyo triaje se decidió el 13/10/2026 (apartado 3.6); los casos que aprobaron con una reserva; las fallas del propio instrumento de evaluación, y lo que el método no pudo observar. Cada fila trae el tipo de arreglo que propuso quien probó; la decisión del triaje del 13/10/2026 (documento, código en rc4 o trabajo futuro) está en la Tabla 3.6.

## 4.1 Interfaz

**Tabla 4.1.** Oportunidades de mejora de la interfaz.

| Oportunidad | Origen | Qué ve el estudiante o el docente | Tipo propuesto |
|---|---|---|---|
| Dar a la cabecera del informe docente una altura propia para los iconos y ensanchar «Volver al menú» para que no parta su texto | DEF-RC3-05 (Menor), `PF-RNF03-01` | El docente ve los iconos de dos columnas cruzando el texto y los de otras dos sueltos y pequeños; las cifras se leen | Código (disposición) o arte |
| Acortar la instrucción del bosque en `N2_Guia.asset` o subir la altura de su tablilla, para que quepan las tres líneas | DEF-RC3-10 (Menor), `PF-RNF03-01` | El estudiante lee «qué pasa.» colgando sobre el borde inferior de la tarjeta; las tablillas del taller, el laberinto y el río sí caben | Código, escena o texto |
| Ensanchar el botón «Eliminar datos» (o acortar su rótulo) y dar más aire a la ruta de respaldo del diálogo de salida | DEF-RC3-11 (Observación), `PF-RF47-02` | El docente ve «Eliminar / datos» en dos líneas; la ruta de respaldo queda a unos 8 px de los botones | Disposición |
| Sustituir el círculo con el texto «Algoritm» del resumen de nivel por el arte final del guía | DEF-RC3-01 (Observación), `PF-RF45-01` | El estudiante ve, al terminar el Nivel 1, un círculo naranja que parece un rótulo de trabajo (rc4 todavía lo muestra) | Código: corregido el 14/10/2026 (`86a9898`), posterior a rc4; entra en el siguiente candidato |

Las pruebas automáticas que nombran RNF-03 pasaron en la suite del 10/10/2026 (34 de 34 resultados) y las capturas del ejecutable mostraron igualmente los dos desbordes de la Tabla 4.1. Esas pruebas no cubren la tablilla del bosque ni la cabecera del informe; si la suite ha de vigilarlas, hay que añadirlas.

## 4.2 Flujo del juego

**Tabla 4.2.** Oportunidades de mejora del flujo del juego.

| Oportunidad | Origen | Qué ve el estudiante | Tipo propuesto |
|---|---|---|---|
| Ignorar la entrada unos 300 ms después de cargar el resumen de nivel, para que un doble clic en «Omitir» no pulse «Volver al menú de niveles» | DEF-RC3-09 (Menor), `PF-RF45-05` (F) | Con un doble clic rápido al final del Nivel 1 el estudiante cae en el menú sin ver el resumen (2 de 3 intentos); el progreso no se pierde | Código |
| Fijar la distribución de las siete piezas del Nivel 1 si la evaluación con estudiantes necesita partidas comparables; hoy es aleatoria en cada entrada | DEF-RC3-04 (Observación), `PF-RF14-01` | Nada: es una decisión de diseño. Cambia lo que se puede comparar entre dos estudiantes | Decisión pendiente |
| Confirmar que es intencional la forma de Algoritm en las secuencias puente: llama en `N2_PuenteI`, disco en `N3_PuenteII` y `N3_PuenteII_Horizonte`, gota desde `N3_PuenteII_Rio` | DEF-RC3-08 (Observación), `PF-RF05-04` | El estudiante ve al guía con la forma del nivel que acaba de terminar | Decisión pendiente o documento |
| Decidir si el gesto del Niño en reposo, con la mano a la sien tapando parte de un ojo, se acepta como excepción (`CARA_TAPADA`) o se corrige en el arte | DEF-RC3-03 (Observación), `PF-RF05-05` | Un ojo parcialmente tapado en la portada y en la cueva | Documento o arte |

## 4.3 Arte y paquete del ejecutable

**Tabla 4.3.** Oportunidades de mejora del arte y del paquete.

| Oportunidad | Origen | Qué ve el estudiante o el equipo | Tipo propuesto |
|---|---|---|---|
| Limpiar el halo de croma de las 9 caras de la familia con `arte_check.py halo --apply`, con copia de respaldo | DEF-RC3-I03 (Menor) | Casi nada a 1080p: 15 a 51 píxeles con alfa de 10 a 25 sobre 255; rc2 tenía 0, y las caras entraron entre el 05 y el 10/10/2026 | Arte, con decisión pendiente |
| Quitar del paquete `DirectML.dll`, `D3D12/` y los módulos de red de Unity que nada del juego llama, o dejar documentado por qué se conservan | Observación de S-INSP sobre `PF-RNF10-01` | Bajaría el peso (la carpeta `D3D12` pesa 4,5 MiB) y reduciría código de red que el juego no usa, algo que RNF-10 no pide | Proyecto |
| Medir el tamaño del paquete en cada entrega de arte y no solo en el candidato | `PF-RNF06-01`: 426,6 MB, margen de 73,4 MB (rc4: 426,9 MB, margen de 73,1 MB); las texturas son el 95 % de los activos de usuario en rc3 | rc2 estuvo a 21 MB del tope (479,0 MB) y bajó a 426,6 MB solo por `INC-146`; la próxima entrega de arte vuelve a gastar ese margen | Proceso |
| Decidir si el residuo del motor en `HKCU` (`unity.*`, `unity_connect.*`, `Screenmanager*`) se acepta o se limpia | DEF-RC2-01, decisión D-RC2-01; persiste en rc3 | Un colegio que revise el registro lo encontrará aunque el perfil se haya borrado | Decisión pendiente |

## 4.4 Código y trazabilidad

**Tabla 4.4.** Oportunidades de mejora del código y de la trazabilidad.

| Oportunidad | Origen | Qué cuesta hoy | Tipo propuesto |
|---|---|---|---|
| Renombrar a inglés los identificadores de `ForestSceneController` (20, como `Apartar`, `EnPantalla`, `SueloUnitario`), de `WorkshopSceneController` (`EnPantalla`, `esquinas`) y de `MazeSceneController` (`desborda`), por ejemplo `EnPantalla` a `ToScreenRect` | DEF-RC3-I01 (Menor), `PF-RNF15-01` (F): 3 de 10 clases de la muestra | RNF-15 pide una nomenclatura uniforme en inglés; las otras siete clases de la muestra la cumplen. El cambio no afecta al juego | Estilo de código |
| Exigir la tarjeta del Kanban en el mensaje de cada commit, con una plantilla o un hook de `commit-msg` que rechace el marcador `(tarjeta: <id>)` sin resolver | DEF-RC3-I02 (Menor), `PF-RNF17-01` (F): 9 de 122 commits con tarjeta (7,4 %), 4 con el marcador sin resolver, 109 sin tarjeta (82 citan INC, R, W o T) | RNF-17 y CT-11 piden trazar cada commit de código a su tarjeta. Corregirlo en lo ya hecho exigiría reescribir la historia | Proceso |
| Hacer que las pruebas de arquitectura toleren un nivel ausente: `Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas`, `Architecture_RNF15_ElNombreDelAssemblyCoincideConSuRutaBajoScripts`, `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel`, `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` y `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` | DEF-RC3-12 (Observación), `PF-RNF16-01`: sin `Levels/Wheel` fallan en EditMode 6 de 663 y sin `Levels/Fire` 6 de 696 | Hoy exigen los tres assemblies y todos los RF, no detectan acoplamiento; con ellas tolerantes, RNF-16 podría afirmar cero fallos de acoplamiento | Prueba |
| Fijar la resolución de `unity test` en lote: 15 pruebas de River, Wheel y UI (RNF-03, RNF-19, RNF-20, RNF-21) fallan a 640 × 480 también en la copia completa y pasan con `editor.ps1` a 1920 × 1080 | `PF-RNF16-01`, `S-RNF/rnf16/resumen.md` | Un fallo de entorno se lee como fallo del juego; `NORMA-PRUEBAS.md` ya manda la vía de `editor.ps1` | Herramienta |
| Correr la suite completa, o al menos `Maze` en PlayMode, sobre el HEAD que se entregue | DEF-RC3-I04 (Menor): la suite corrió sobre `292b108` y no sobre `f49ccc9` | Las 78 líneas de `Level2_Maze.unity` reserializadas en `f49ccc9` no tienen una corrida propia. En rc4 la suite sí corrió sobre `0405eba`, el commit del build | Proceso |
| Revisar si `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos` puede medir sin depender de la carga del equipo | La prueba falló una vez (salto de cuadro de 0,056 frente a 0,05 con dos Editores en paralelo) y pasa sola | Un falso rojo en la suite distribuida obliga a repetirla | Prueba |

## 4.5 Instrumento y proceso de evaluación

**Tabla 4.5.** Oportunidades de mejora del instrumento de evaluación.

| Oportunidad | Origen | Qué cuesta hoy | Tipo propuesto |
|---|---|---|---|
| Arreglar `Restaurar-Registro` en `oe4.ps1`: sale con código 2 («The type of the value object did not match the specified RegistryValueKind», línea 1502) y no repone los valores cambiados, solo borra los nuevos | Sesiones S-INI, S-N3, S-N2R, S-PAU, S-OMI, S-PER y S-DOC | Los valores que cambia Unity (sesión y pantalla) no se reponen; hubo que reimportar la clave del reproductor desde una copia hecha antes | Herramienta |
| Poner en marcha la ráfaga de capturas antes de la acción: la primera captura llega con unos 1,3 s de retraso | Sesiones S-N1, S-OMI y S-PAU | No hay cuadro de la ignición, del primer cuadro del acercamiento de la cueva ni de la pausa a mitad del recorrido del laberinto (se tomó a unos 0,3 s de pulsar «Ejecutar») | Herramienta |
| Desactivar la superposición de NVIDIA en el equipo de pruebas | Aviso «Presione Alt+Z…» en `PF-RF01-01_1.png` y `PF-RF06-01_1a.png` | Cae unos segundos sobre la esquina superior derecha tras el arranque y puede tapar un control en una captura | Entorno |
| Reescribir la portada de `PF-RF01-03`: panel ámbar a la izquierda con título, lema y botones, bosque con los cinco personajes a la derecha | DEF-RC3-02 | El caso describe un título «sobre una tarjeta más baja» que el juego ya no tiene | `casos.md` |
| No prometer coordenadas de las piezas en `PF-RF14-01`: pedir leer la pantalla antes de arrastrar | DEF-RC3-04 | Las coordenadas del caso no valen, y los pasos 7 y 8 se rehicieron leyendo la pantalla | `casos.md` |
| En el paso 21 de `PF-RF44-01`, pedir los clics hasta la línea 4 y fotografiar entonces | DEF-RC3-06 | Con un clic por segundo la última línea y «a mitad del río» no coinciden nunca | `casos.md` |
| Admitir ±1 en los colores medidos por píxel de `PF-RF30-02` | DEF-RC3-07 | Quien compare píxeles puede dar por defecto un redondeo de color que no se ve | `casos.md` |
| Decir qué forma de Algoritm lleva cada secuencia puente en `PF-RF05-04`, y contemplar el gesto del Niño en el criterio de `PF-RF05-05` | DEF-RC3-08 y DEF-RC3-03 | Los criterios de esos casos no describen lo que el juego hace | `casos.md` |
| Listar las cinco pruebas de arquitectura como esperadas en `PF-RNF16-01`, o dejarlas de listar si se corrigen | DEF-RC3-12 | El caso solo admite fallos de pruebas que cargan una escena por nombre | `casos.md` |
| Reformular la advertencia de riesgo de `PF-RF21-02`: la narrativa de cierre entra a unos 1,3 s tras «Soplar», no a los 3,5 s que cita el caso, y el máximo de luz tras el cuarto golpe efectivo no se pudo medir | Observación de S-OMI, sin id propio | El caso no se puede comprobar como está escrito | `casos.md` |
| Decidir si Claude ejecuta los dos recorridos completos del arnés de `PF-RNF13-01`, además del recorrido con cronómetro de H8 | Tabla 3.4 | El caso está en PS y sus dos recorridos del arnés, planeados, no se corrieron | Decisión pendiente |

El 13/10/2026 se aplicaron en `casos.md` las correcciones de DEF-RC3-02, -04, -06, -07 y -12 de esta tabla. Las de DEF-RC3-08 y DEF-RC3-03 siguen como trabajo futuro (apartado 3.6). Las filas de DEF-RC3-05, -09, -10 y -11 quedaron corregidas en el código en `0405eba` y entraron en rc4 (apartado 3.6); sin pasada propia de los casos, el veredicto de rc3 se conserva.

## 4.6 Lo que la evaluación con personas resolvió y lo que dejó abierto

El 13/10/2026 se ejecutó la hoja HUM con 4 estudiantes de grado quinto del Colegio El Libertador IED (apartado 5.1). PG-05 y PG-06 del guion dieron resultado positivo (H1 y H2), y las comprobaciones H3 a H7 y H9 a H12 se aprobaron (Tabla 5.2): el segundo equipo, la ejecución sin conexión a internet, el equipo sin tarjeta gráfica dedicada, el oído de la escena final y del hundimiento de la balsa, la vista del cruce de la escena 3.3 y de la recolección del Nivel 3, y las piezas que suenan. En H11 la balsa se hundió, se comprendió por qué, se arregló y finalmente cruzó.

Las observaciones de la sesión sobre el juego produjeron diez correcciones, registradas como `INC-152` a `INC-160` (Tabla 5.4, apartado 5.3); ninguna se probó todavía en un ejecutable.

Quedan abiertos tres puntos:

- H8, el recorrido con cronómetro, dio entre 40 y 45 minutos frente a la referencia de 20 a 40 del OE1. Supera el tope en hasta 5 minutos y no se registró la causa. Medir cuánto de ese tiempo es lectura de narrativas, cuánto reto y cuánto espera del estudiante exige una sesión con registro por fase.
- H13, la comprobación de mano, no se realizó. Cubre el clic tras cargar una escena sin mover el puntero, la papelera del laberinto y la octava fila del informe, tres cosas que el arnés enmascara; pasa a trabajo futuro.
- Las confirmaciones documentales de `PF-RNF22-01` y `PF-RNF23-01` siguen pendientes (apartado 3.1).

Este documento no incluye cifras de carga ni de memoria de los dos equipos del colegio, así que RNF-04 y RNF-05 siguen medidos con cifras solo en el equipo 1.

## 4.7 Límites de la evaluación

**Tabla 4.6.** Lo que el método no pudo observar y cómo afecta la lectura de los resultados.

| Límite | Efecto sobre la lectura |
|---|---|
| Un solo equipo en las mediciones de RNF-04, RNF-05 y RNF-06 | Los valores de carga y memoria valen para el equipo 1 (Ryzen 5 7600X, 32 GB, RTX 5070 Ti); un equipo modesto puede dar más. El tamaño del paquete no depende del equipo |
| La línea `RNF-04` de `SceneLoader` mide la carga, no el primer cuadro visible; el muestreo de red empieza unos 2 a 8 s después del arranque y no ve conexiones de menos de 2 s | RNF-04 y RNF-10 son cotas del comportamiento, no una medida exhaustiva; `globalgamemanagers` sin `unity3d.com` cubre el arranque |
| RNF-18 se probó en un ejecutable de la copia desechable y no con el Editor abierto y en Play | «Sin recompilación» no es observable con `unity build`; el caso aprueba como variante |
| RNF-16 se probó con `unity test` en copias sin un nivel | El caso aprueba con la desviación de DEF-RC3-12; 15 pruebas de River, Wheel y UI fallan igual en la copia completa por la resolución del lote |
| Los mensajes de las fotos se leyeron a 0,5 a 0,9 s de cada acción | El límite de 1 s de RF-11 se juzga por ese margen y no por un cronómetro |
| La suite no corrió sobre el contenido exacto de rc3 | Entre `23d7e8c` y rc3 no cambió ningún `.cs`; la reserialización de `Level2_Maze.unity` queda sin corrida propia (DEF-RC3-I04) |
| El sonido no lo mide el arnés | Los tres casos `SON` los aprobó una persona el 13/10/2026 (H12, con H6 y H7); es un juicio de oído sin medición propia |
| La pausa del laberinto se tomó a unos 0,3 s de pulsar «Ejecutar», no a mitad del recorrido | La carretilla ya se movía y siguió tras «Reanudar»; la parada a mitad del recorrido no tiene captura |
| Los dos recorridos del arnés de `PF-RNF13-01` no se ejecutaron | No hay duración medida del juego entero con un perfil nuevo; solo la del Nivel 1 en S-N1 |
