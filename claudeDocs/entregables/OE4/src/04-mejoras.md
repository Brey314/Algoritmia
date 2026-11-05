# 4. OPORTUNIDADES DE MEJORA PARA TRABAJOS FUTUROS

El objetivo pide identificar oportunidades de mejora, además de contar casos aprobados. Este capítulo recoge lo que las dos pasadas dejaron abierto, cada punto con su origen y su causa, agrupado por el tema donde habría que trabajar. Entran cuatro clases de hallazgo: los defectos Menores y las Observaciones de las Tablas 3.6 y 3.8; los casos que aprobaron con una reserva; las fallas del propio instrumento de evaluación, y lo que el método no pudo observar. Las filas que nacieron en rc3 y ya se corrigieron se conservan con su estado sobre rc5, porque explican de dónde salió cada corrección. Ningún defecto de rc5 se corrigió en el juego, porque no se compiló un rc6.

## 4.1 Interfaz

**Tabla 4.1.** Oportunidades de mejora de la interfaz.

| Oportunidad | Origen | Qué ve el estudiante o el docente | Tipo propuesto |
|---|---|---|---|
| Dar a la cabecera del informe docente una altura propia para los iconos y ensanchar «Volver al menú» para que no parta su texto | DEF-RC3-05 (Menor), `PF-RNF03-01` | El docente ve los iconos de dos columnas cruzando el texto y los de otras dos sueltos y pequeños; las cifras se leen | Corregido en `0405eba`; comprobado sobre rc5 |
| Acortar la instrucción del bosque en `N2_Guia.asset` o subir la altura de su tablilla, para que quepan las tres líneas | DEF-RC3-10 (Menor), `PF-RNF03-01` | El estudiante lee «qué pasa.» colgando sobre el borde inferior de la tarjeta; las tablillas del taller, el laberinto y el río sí caben | Corregido en `0405eba`; comprobado sobre rc5 (`PF-RNF03-01`) |
| Ensanchar el botón «Eliminar datos» (o acortar su rótulo) y dar más aire a la ruta de respaldo del diálogo de salida | DEF-RC3-11 (Observación), `PF-RF47-02` | El docente ve «Eliminar / datos» en dos líneas; la ruta de respaldo queda a unos 8 px de los botones | Corregido en `0405eba`; comprobado sobre rc5, con 60 px de hueco |
| Sustituir el círculo con el texto «Algoritm» del resumen de nivel por el arte final del guía | DEF-RC3-01 (Observación), `PF-RF45-01` | El estudiante ve, al terminar el Nivel 1, un círculo naranja que parece un rótulo de trabajo (rc4 todavía lo muestra) | Corregido el 14/10/2026 (`86a9898`); comprobado sobre rc5 (`PF-RF45-01`) |
| Subir el contraste de la etiqueta «Arrastra un bloque a tu secuencia» del laberinto a 4,5:1 o más | DEF-RC5-15 (Menor), `PF-RNF20-01` (F) | Un estudiante lee una pista secundaria con 3,15:1; la instrucción principal, en la tablilla, tiene 13,49:1 | Código o escena |
| Mostrar la cuenta y el lado de cada bloque en una secuencia larga del laberinto | DEF-RC5-05 (Observación), `PF-RF31-01`, `PF-RF34-01` | Con 13 bloques hay que seleccionar uno a uno para leerlos | Diseño, por confirmar |
| Corregir el OE2 radicado, que pide un rombo para «Retroceder», si se autoriza: se decidió conservar el círculo del juego | DEF-RC5-04 (Observación), `INC-161`, `PF-RF31-01` (PD) | Las tres formas se distinguen; la diferencia queda entre juego y documento | Documento |
| Mostrar «La pieza vuelve a tus materiales.» al retirar una pieza del Nivel 3 y quitar «Puesto…» | DEF-RC5-07 (Menor) | Con la pieza fuera de la balsa la tablilla sigue diciendo «Puesto…» | Código |
| Revisar el arco dentado que recorta la llama al nacer, el tronco que se agranda con la pausa abierta y la alerta del amarre en grises | DEF-RC5-03, DEF-RC5-08 y DEF-RC5-19 (Observaciones) | Tres detalles visuales sin efecto en la partida | Por confirmar |

Las pruebas automáticas que nombran RNF-03 pasaron en la suite del 10/10/2026 (34 de 34 resultados) y aun así las capturas del ejecutable rc3 mostraron los desbordes de la tablilla del bosque y de la cabecera del informe. Sobre rc5 esos desbordes ya no aparecen y la suite da 35 de 35.

## 4.2 Flujo del juego

**Tabla 4.2.** Oportunidades de mejora del flujo del juego.

| Oportunidad | Origen | Qué ve el estudiante | Tipo propuesto |
|---|---|---|---|
| Ignorar la entrada unos 300 ms después de cargar el resumen de nivel, para que un doble clic en «Omitir» no pulse «Volver al menú de niveles» | DEF-RC3-09 (Menor), `PF-RF45-05` (F en rc3, P en rc5) | Con un doble clic rápido al final del Nivel 1 el estudiante caía en el menú sin ver el resumen (2 de 3 intentos); en rc5 no se reprodujo en cuatro intentos | Corregido en `0405eba`; comprobado sobre rc5 |
| Dejar el último escalón de luz por golpe por debajo del máximo, o documentar en INC-67 el cuarto escalón | DEF-RC5-10 (Menor), `PF-RF21-02` (F) | Quien da un cuarto golpe efectivo ve la luz máxima antes de soplar y el barrido del soplido ya no se nota | Código o documento |
| Hacer que la narrativa previa al retomar el Nivel 2 corresponda a la fase pendiente | DEF-RC5-11 (Observación), `PF-RNF14-01` | Al retomar se repiten 19 líneas de la apertura del bosque antes del taller o del laberinto | Diseño, por confirmar |
| Decidir si la travesía de la balsa en la escena 3.3 basta para «la animación automática del cruce» de RF-44 | DEF-RC5-06 (Observación), `PF-RF44-01` | La balsa se desplaza poco y gran parte del movimiento se ve en el fondo | Decisión pendiente |
| Rechazar los nombres reservados de Windows al crear un perfil | DEF-RC5-12 (Observación), `PF-RF02-05` | «CON» crea `Datos\CON.json`, que puede dar problemas con herramientas de Windows | Código, por confirmar |
| Evitar que el resumen del Nivel 3 una «te lo dijo al hundirse» con «No hizo falta rehacer nada» | DEF-RC5-14 (Observación), `PF-RNF13-01` | Dos frases elegidas por separado pueden no concordar | Contenido |
| Fijar la distribución de las siete piezas del Nivel 1 si la evaluación con estudiantes necesita partidas comparables; hoy es aleatoria en cada entrada | DEF-RC3-04 (Observación), `PF-RF14-01` | Nada: es una decisión de diseño. Cambia lo que se puede comparar entre dos estudiantes | Decisión pendiente |
| Revisar con más estudiantes la franja de cercanía de las piedras del Nivel 1, un parámetro de `FireLevelConfig` | Sesión del 16/10/2026 (apartado 5.4) | A una estudiante le costó encender el fuego porque las piedras quedaban demasiado juntas o demasiado separadas; PG-06 había dado los valores por buenos con 4 estudiantes | Parámetro, sin recompilar (CT-05) |
| Equilibrar la dificultad del Nivel 3, o reforzar su instrucción, después de observar a más estudiantes | Sesión del 16/10/2026 (apartado 5.4) | A un estudiante el nivel le pareció muy fácil y a otra le costó entender qué hacer | Diseño, por decidir con más datos |
| Mostrar en el inventario del Nivel 3 cuántas sogas faltan por amarrar, como ya lo hace con los troncos | Sesión del 16/10/2026 (apartado 5.4), persona externa | Quien amarra no sabe cuántos de los diez amarres le quedan | Interfaz |

## 4.3 Arte y paquete del ejecutable

**Tabla 4.3.** Oportunidades de mejora del arte y del paquete.

| Oportunidad | Origen | Qué ve el estudiante o el equipo | Tipo propuesto |
|---|---|---|---|
| Limpiar el halo de croma de las 9 caras de la familia con `arte_check.py halo --apply`, con copia de respaldo | DEF-RC3-I03 (Menor) | Casi nada a 1080p: 15 a 51 píxeles con alfa de 10 a 25 sobre 255; rc2 tenía 0, y las caras entraron entre el 05 y el 10/10/2026 | Arte, con decisión pendiente |
| Quitar del paquete `DirectML.dll`, `D3D12/` y los módulos de red de Unity que nada del juego llama, o dejar documentado por qué se conservan | Observación de S-INSP sobre `PF-RNF10-01` | Bajaría el peso (la carpeta `D3D12` pesa 4,5 MiB) y reduciría código de red que el juego no usa, algo que RNF-10 no pide | Proyecto |
| Medir el tamaño del paquete en cada entrega de arte y no solo en el candidato | `PF-RNF06-01`: 426,9 MB en rc5, margen de 73,1 MB (rc3: 426,6 MB); las texturas son el 95 % de los activos de usuario en rc3 | rc2 estuvo a 21 MB del tope (479,0 MB) y bajó a 426,6 MB solo por `INC-146`; la próxima entrega de arte vuelve a gastar ese margen | Proceso |
| Decidir si el residuo del motor en `HKCU` (`unity.*`, `unity_connect.*`, `Screenmanager*`) se acepta o se limpia | DEF-RC2-01, decisión D-RC2-01; persiste en rc5 | Un colegio que revise el registro lo encontrará aunque el perfil se haya borrado | Decisión pendiente |

## 4.4 Código y trazabilidad

**Tabla 4.4.** Oportunidades de mejora del código y de la trazabilidad.

| Oportunidad | Origen | Qué cuesta hoy | Tipo propuesto |
|---|---|---|---|
| Renombrar a inglés los identificadores de `ForestSceneController` (20, como `Apartar`, `EnPantalla`, `SueloUnitario`), de `WorkshopSceneController` (`EnPantalla`, `esquinas`) y de `MazeSceneController` (`desborda`), por ejemplo `EnPantalla` a `ToScreenRect` | DEF-RC5-16 (Menor), que continúa DEF-RC3-I01, `PF-RNF15-01` (F en rc3 y en rc5): 3 de 10 clases de la muestra | RNF-15 pide una nomenclatura uniforme en inglés; las otras siete clases de la muestra la cumplen. El cambio no afecta al juego | Estilo de código |
| Exigir la tarjeta del Kanban en el mensaje de cada commit, con una plantilla o un hook de `commit-msg` que rechace el marcador `(tarjeta: <id>)` sin resolver | DEF-RC5-17 (Observación), que continúa DEF-RC3-I02, `PF-RNF17-01` (F en rc3 y en rc5): en rc5, 9 de 92 commits de código con tarjeta (9,8 %), 3 con el marcador sin resolver y 80 sin nada (65 citan otro identificador). rc3 contó 9 de 122 commits que tocan `Assets/` (7,4 %) | RNF-17 y CT-11 piden trazar cada commit de código a su tarjeta. Corregirlo en lo ya hecho exigiría reescribir la historia | Proceso |
| Hacer que las pruebas de arquitectura toleren un nivel ausente: `Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas`, `Architecture_RNF15_ElNombreDelAssemblyCoincideConSuRutaBajoScripts`, `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel`, `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` y `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` | DEF-RC3-12 (Observación), `PF-RNF16-01`: en rc5 fallan las cinco en EditMode en los dos sentidos | Hoy exigen los tres assemblies y todos los RF, no detectan acoplamiento; con ellas tolerantes, RNF-16 podría afirmar cero fallos de acoplamiento | Prueba |
| Fijar la resolución de `unity test` en lote: 17 pruebas de River, Wheel y UI fallan a 640 × 480 también sin retirar nada y pasan en el Editor a 1920 × 1080 | DEF-RC5-20, `PF-RNF16-01` (PD) | Un fallo de entorno se lee como fallo del juego; `NORMA-PRUEBAS.md` ya manda la vía de `editor.ps1` | Herramienta |
| Revisar si `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos` puede medir sin depender de la carga del equipo | La prueba falló una vez (salto de cuadro de 0,056 frente a 0,05 con dos Editores en paralelo) y pasa sola | Un falso rojo en la suite distribuida obliga a repetirla | Prueba |

## 4.5 Instrumento y proceso de evaluación

**Tabla 4.5.** Oportunidades de mejora del instrumento de evaluación.

| Oportunidad | Origen | Qué cuesta hoy | Tipo propuesto |
|---|---|---|---|
| Arreglar `Restaurar-Registro` en `oe4.ps1`, que no reponía los valores `Binary` del registro | Sesiones de rc3 | Hubo que reimportar la clave del reproductor desde una copia hecha antes | Corregido el 04/11/2026; funcionó en las sesiones de rc5 |
| Decir en el catálogo cómo se prueba la flecha ya pulsada al abrir la pausa, con una convención única de numeración de líneas y con las poses que de verdad se ven | DEF-RC5-01, -02 y -09 (Observaciones) | Tres pasos no se pueden seguir al pie de la letra con un solo puntero o con el texto actual | `casos.md` y arnés |
| Poner en marcha la ráfaga de capturas antes de la acción: la primera captura llega con unos 1,3 s de retraso | Sesiones S-N1, S-OMI y S-PAU | No hay cuadro de la ignición, del primer cuadro del acercamiento de la cueva ni de la pausa a mitad del recorrido del laberinto (se tomó a unos 0,3 s de pulsar «Ejecutar») | Herramienta |
| Desactivar la superposición de NVIDIA en el equipo de pruebas | Aviso «Presione Alt+Z…» en `PF-RF01-01_1.png` y `PF-RF06-01_1a.png` | Cae unos segundos sobre la esquina superior derecha tras el arranque y puede tapar un control en una captura | Entorno |
| Reescribir la portada de `PF-RF01-03`: panel ámbar a la izquierda con título, lema y botones, bosque con los cinco personajes a la derecha | DEF-RC3-02 | El caso describe un título «sobre una tarjeta más baja» que el juego ya no tiene | `casos.md` |
| No prometer coordenadas de las piezas en `PF-RF14-01`: pedir leer la pantalla antes de arrastrar | DEF-RC3-04 | Las coordenadas del caso no valen, y los pasos 7 y 8 se rehicieron leyendo la pantalla | `casos.md` |
| En el paso 21 de `PF-RF44-01`, pedir los clics hasta la línea 4 y fotografiar entonces | DEF-RC3-06 | Con un clic por segundo la última línea y «a mitad del río» no coinciden nunca | `casos.md` |
| Admitir ±1 en los colores medidos por píxel de `PF-RF30-02` | DEF-RC3-07 | Quien compare píxeles puede dar por defecto un redondeo de color que no se ve | `casos.md` |
| Listar las cinco pruebas de arquitectura como esperadas en `PF-RNF16-01`, o dejarlas de listar si se corrigen | DEF-RC3-12 | El caso solo admite fallos de pruebas que cargan una escena por nombre | `casos.md` |
| Reformular la advertencia de riesgo de `PF-RF21-02`: la narrativa de cierre entra a unos 1,3 s tras «Soplar», no a los 3,5 s que cita el caso, y el máximo de luz tras el cuarto golpe efectivo no se midió en rc3 | Observación de S-OMI, sin id propio | rc5 sí lo midió, y el caso quedó en F (DEF-RC5-10) | `casos.md` |

El 13/10/2026 se aplicaron en `casos.md` las correcciones de DEF-RC3-02, -04, -06, -07 y -12 de esta tabla. Las de DEF-RC3-08 y DEF-RC3-03 se aplicaron el 16/10/2026, cuando el equipo los aceptó como diseño (apartado 3.6).

## 4.6 Lo que la evaluación con personas resolvió y lo que dejó abierto

El 13/10/2026 se ejecutó la hoja HUM sobre rc3 con 4 estudiantes de grado quinto del Colegio El Libertador IED (apartado 5.1). PG-05 y PG-06 del guion dieron resultado positivo (H1 y H2), y las comprobaciones H3 a H7 y H9 a H13 se aprobaron (Tabla 5.2): el segundo equipo, la ejecución sin conexión a internet, el equipo sin tarjeta gráfica dedicada, el oído de la escena final y del hundimiento de la balsa, la vista del cruce de la escena 3.3 y de la recolección del Nivel 3, y las piezas que suenan. En H11 la balsa se hundió, se comprendió por qué, se arregló y finalmente cruzó.

Las observaciones de la sesión sobre el juego produjeron diez correcciones, registradas como `INC-152` a `INC-160` (Tabla 5.4, apartado 5.3). Entraron en rc4 y en rc5, y el 16/10/2026 los 30 estudiantes de un curso jugaron rc5 en 30 equipos del colegio, sin conexión y sin tarjeta gráfica dedicada, sin ninguna complicación y hasta terminar los tres niveles (apartado 5.4). Esa sesión no revisó cada corrección por separado ni siguió el catálogo de casos. De sus testimonios salen tres oportunidades nuevas de la Tabla 4.2: la cercanía de las piedras del Nivel 1, la dificultad del Nivel 3 y el conteo de sogas en su inventario.

Quedan abiertos dos puntos:

- Los tres casos de sonido no se ejecutaron sobre rc5, y H3, H4, H5 y H8 no se repitieron con su guion (apartado 5.5).
- H8, el recorrido con cronómetro, dio sobre rc3 entre 40 y 45 minutos frente a la referencia de 20 a 40 del OE1. Supera el tope en hasta 5 minutos y no se registró la causa. Medir cuánto de ese tiempo es lectura de narrativas, cuánto reto y cuánto espera del estudiante exige una sesión con registro por fase.

De los equipos del colegio hay cifras de carga y memoria de uno solo, medido sobre rc5 el 16/10/2026 en un recorrido de 0,9 min por el Nivel 1 (1,223 s de peor carga y 633 MiB de memoria privada, apartado 3.5). Medir el recorrido completo en varios equipos del colegio con el mismo medidor completaría RNF-04 y RNF-05 en el hardware de destino.

## 4.7 Límites de la evaluación

**Tabla 4.6.** Lo que el método no pudo observar y cómo afecta la lectura de los resultados.

| Límite | Efecto sobre la lectura |
|---|---|
| Las mediciones completas de RNF-04 y RNF-05 son de un solo equipo | Los valores de carga y memoria de todas las escenas valen para el equipo 1 (Ryzen 5 7600X, 32 GB, RTX 5070 Ti). En un equipo del colegio (AMD A10-9600P, 3,9 GB, gráficos integrados) se midió solo el Nivel 1 de rc5: 1,223 s de peor carga y 633 MiB de memoria privada, dentro del límite. El tamaño del paquete no depende del equipo |
| La línea `RNF-04` de `SceneLoader` mide la carga, no el primer cuadro visible; el muestreo de red empieza unos 2 a 8 s después del arranque y no ve conexiones de menos de 2 s | RNF-04 y RNF-10 son cotas del comportamiento, no una medida exhaustiva; `globalgamemanagers` sin `unity3d.com` cubre el arranque |
| RNF-18 se probó sobre rc5 en el Editor y en Play, pero conducido con scripts y no con el ratón | Los dos cambios de asset se vieron en el juego sin recompilar; el caso aprueba como variante |
| RNF-16 se probó con `unity test` en un worktree sin un nivel | El caso queda en PD: 17 pruebas de River, Wheel y UI fallan igual sin retirar nada, por la resolución del lote, y no aportan evidencia en esas corridas |
| Los mensajes de las fotos se leyeron a 0,5 a 0,9 s de cada acción | El límite de 1 s de RF-11 se juzga por ese margen y no por un cronómetro |
| El sonido no lo mide el arnés | Los tres casos `SON` no se ejecutaron sobre rc5 y la meta de sonidos no queda demostrada sobre esa versión. Sobre rc3 los aprobó una persona el 13/10/2026 (H12, con H6 y H7), con un juicio de oído sin medición propia. El audio y sus scripts no cambiaron entre las dos, pero sí narrativas y sus tiempos |
| El recorrido con cronómetro de `PF-RNF13-01` es de rc3 | Los dos recorridos del arnés son de rc5 y terminaron sin bloqueos; sus tiempos incluyen la latencia del arnés y no miden a un estudiante. H8 no se repitió sobre rc5: la duración del recorrido sobre esa versión no se registró |
| La sesión del 16/10/2026 respalda seis PD sin haber seguido el catálogo | La huella de la carpeta no se tomó; el log y las medidas son de un solo equipo y solo del Nivel 1; no consta si la cuenta era de administrador; no se activó el modo avión del guion; los resultados de los 30 equipos son los entregados por el equipo |
| `PF-RNF12-01`, `PF-RNF22-01` y `PF-RNF23-01` no se repitieron sobre rc5 | Dependen de documentos firmados y de una confirmación de octubre, que no cambian con la versión |
