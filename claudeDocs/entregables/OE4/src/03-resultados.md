# 3. RESULTADOS

Todos los veredictos de este capítulo son los de rc3 (apartado 2.6), obtenidos el 10/10/2026 y consolidados en `claudeDocs/tasks/OE4/OE4-Resultados.md`. Donde un caso se repitió después de una corrección, vale el veredicto del candidato más reciente; la primera pasada se conserva en el registro de trabajo y se resume en el apartado 3.6.

## 3.1 Veredictos por sesión

**Tabla 3.1.** Veredictos de rc3 por sesión. La eficacia es (P + PD) / (P + PD + F).

{{TABLA_RESULTADOS_SESION}}

En las sesiones de recorridos jugados con el arnés (S-INI a S-DOC) todos los casos salieron P, salvo uno de S-OMI: `PF-RF45-05`, por el defecto DEF-RC3-09 (apartado 3.6). Un caso repartido entre sesiones, como `PF-RF05-04` (partes en los Niveles 1, 2 y 3), cuenta una vez con el peor veredicto de sus partes, y todos esos salieron P. Un caso de S-INSP que quedó en B y se ejecutó después sobre el ejecutable, como `PF-RNF03-01` o `PF-RNF16-01`, cuenta con el veredicto del ejecutable.

S-INSP concentra los otros tres F, que son de requerimientos no funcionales: `PF-RNF03-01` (DEF-RC3-10 y DEF-RC3-05, dos pantallas con un texto fuera de su tablilla y unos iconos sobre el texto), `PF-RNF15-01` (DEF-RC3-I01, identificadores en español) y `PF-RNF17-01` (DEF-RC3-I02, commits sin tarjeta). Ninguno de los tres cambia lo que el juego hace. Por eso su eficacia, 70,0 % (7 de 10), no dice nada sobre la mecánica.

Cuatro casos están en PS. Uno es de S-HUM: `PF-RNF13-01`, el recorrido completo, porque los dos recorridos del arnés sobre rc3 no se ejecutaron y la comprobación H8 registró la duración (40 a 45 min) pero no cubre por sí sola los tres recorridos que el caso pide (apartado 3.4). Tres son de S-INSP: `PF-RNF12-01` (el formato de consentimiento informado; ninguna comprobación de la hoja HUM lo cubre), `PF-RNF22-01` (la inspección está hecha, con 344 cadenas sin enlaces ni compras y 9 ilustraciones sin violencia, y falta la confirmación) y `PF-RNF23-01` (los créditos y las licencias coinciden con lo esperado, y falta el soporte escrito de la Familia Anonaky y de la colaboradora de arte). No hay casos en B, NA ni NE.

En el corte del 10/10/2026 eran diez los casos en PS (126 con veredicto: 122 P y 4 F, eficacia 96,8 %). Seis pasaron a P el 13/10/2026 con la hoja HUM, en una sesión manual sobre rc3 con 4 estudiantes de grado quinto (apartado 5.2), cada uno con la comprobación que lo cubre y que se registró como «Aprobado»: `PF-RNF07-02` (H3, segundo equipo), `PF-RNF08-01` (H4, sin conexión a internet), `PF-CT02-01` (H5, equipo sin tarjeta gráfica dedicada) y `PF-SON-01` a `PF-SON-03` (H12, con H6 y H7 para `PF-SON-03`). Las comprobaciones H1 y H2 (PG-05 y PG-06) y H9 a H11 no corresponden a ningún caso nuevo, y H13 no se realizó: `PF-RNF02-01`, `PF-RF34-01` y `PF-RF46-01` conservan el P de rc3.

## 3.2 Veredictos por etiqueta

El indicador de eficacia nombra los botones, los sonidos y los saltos de nivel. La Tabla 3.2 reparte los mismos casos por etiqueta para leerlo así.

**Tabla 3.2.** Veredictos de rc3 por etiqueta.

{{TABLA_RESULTADOS_ETIQUETA}}

NAV tiene 18 casos: 16 en P, 1 en F (`PF-RF45-05`) y 1 en PS (`PF-RNF13-01`, el recorrido completo). BOT cierra los 28 en P. SON queda en 100,0 % (3 de 3): sus tres casos son de oído, el arnés no mide el sonido y los aprobó una persona el 13/10/2026 (comprobaciones H12, H6 y H7 de la hoja HUM), así que es un juicio de oído y no una medición. La etiqueta RNF queda en 82,4 % (14 de 17 con veredicto) por los tres F de S-INSP; era 78,6 % (11 de 14) en el corte del 10/10/2026, antes de que H3, H4 y H5 pasaran a P. El indicador no nombra esa etiqueta.

## 3.3 Indicadores del objetivo

**Tabla 3.3.** Los dos indicadores del OE4 sobre rc3.

| Indicador | Meta | Resultado en rc3 |
|---|---|---|
| Eficacia de las pruebas funcionales | 90 % o más | 97,0 % = 128 / (128 + 0 + 4), sobre 132 casos con veredicto; 4 casos en PS fuera del cálculo. En el corte del 10/10/2026 era 96,8 % = 122 / (122 + 0 + 4), sobre 126 |
| Eficacia en las etiquetas `NAV`, `BOT` y `SON` | 90 % o más cada una | `NAV` 94,1 % (16 de 17) · `BOT` 100,0 % (28 de 28) · `SON` 100,0 % (3 de 3, aprobados en la sesión manual del 13/10/2026; en el corte del 10/10/2026, sin calcular) |
| RF de prioridad Alta con algún F Bloqueante o Mayor abierto | Ninguno de los 45 | Ninguno: 45 de 45 RF de prioridad Alta sin F Bloqueante ni Mayor. El único F que toca un RF de prioridad Alta es `PF-RF45-05` (RF-45), de severidad Menor |

Los 4 F de la pasada son todos Menores y ninguno de los 16 defectos hallados es Bloqueante ni Mayor (apartado 3.6). Con la sesión del 13/10/2026 las tres etiquetas que nombra el indicador superan la meta, y la de sonidos se cierra con un juicio de oído de una persona y no con una medición. La eficacia global depende todavía de los 4 casos en PS: si los cuatro fallaran quedaría en 94,1 % (128 de 136) y seguiría sobre el 90 %.

## 3.4 Recorridos completos del juego

El caso `PF-RNF13-01` exige recorrer el juego entero, de la pantalla de inicio a los créditos y sin omitir ninguna narrativa, sin bloqueos, cierres inesperados ni estados irrecuperables. Lo planearon dos recorridos del arnés, uno en ventana de 1920 × 1080 y otro a pantalla completa, y uno con cronómetro, la comprobación H8 de la hoja HUM (apartado 5.2). Los dos recorridos del arnés no se ejecutaron en esta pasada: las sesiones EXE jugaron cada nivel con semillas y perfiles distintos, no el juego entero con un perfil nuevo. Si se ejecutan, además del recorrido con cronómetro de H8, es una decisión abierta (apartado 4.5). La duración de los recorridos del arnés incluye la latencia de cada llamada y las capturas de control, y no mide a un estudiante; la referencia de 20 a 40 minutos del OE1 se contrasta con la comprobación H8, que el 13/10/2026 registró entre 40 y 45 minutos (Tabla 3.4).

Como referencia parcial, en S-N1 la resolución del Nivel 1 con un perfil nuevo (`OE4N1`) dio 225,84 s en el JSON del perfil, frente a 225,4 s previstos con 41 s de pausa (`evidencias/rc3/S-N1/resumen.md`). Ninguno de los lanzamientos de las sesiones EXE dejó `Exception`, `Error` ni `Assert` en `Player.log`.

**Tabla 3.4.** Recorridos completos sobre rc3.

| Recorrido | Condición | Duración | Fases en el JSON | `Exception` en `Player.log` |
|---|---|---|---|---|
| Arnés 1 | Ventana de 1920 × 1080, perfil nuevo | No ejecutado en esta pasada | No ejecutado | No ejecutado |
| Arnés 2 | Pantalla completa, perfil nuevo | No ejecutado en esta pasada | No ejecutado | No ejecutado |
| Comprobación H8, 13/10/2026 | Cronómetro (hoja HUM) | Entre 40 y 45 min; supera en hasta 5 min el tope de 20 a 40 min del OE1, sin causa registrada | Sin dato | Sin dato |

## 3.5 Requerimientos no funcionales medidos en el ejecutable

Los tres presupuestos duros del proyecto se miden sobre el ejecutable y no se estiman. Las cargas salen de la línea `RNF-04` que `SceneLoader` deja en `Player.log`; el tiempo incluye los dos medios fundidos de 0,4 s, por lo que es una medida conservadora. La memoria sale del muestreo de `WorkingSet64` y `PrivateMemorySize64` del proceso cada 2 s, con 4.213 muestras de todas las sesiones. El tamaño es el de la carpeta entregable sin `Datos/` ni las carpetas `*_DoNotShip`. Todo se midió en el equipo 1.

**Tabla 3.5.** Presupuestos de rendimiento y de paquete en rc3.

| Requerimiento | Umbral | Valor medido | Dónde se dio | Veredicto |
|---|---|---|---|---|
| RNF-04, carga de cualquier escena | Menos de 10 s | 0,268 s, el peor de las 12 escenas (media 0,089 s, n = 26); `Boot` a `MainMenu`, unos 3,4 s | Escena `Narrative` (`S-RNF/rnf04_tabla.csv`) | P |
| RNF-05, memoria | Menos de 2 GB | 1.094 MiB (1,15 GB) de memoria privada; 438 MiB de memoria de trabajo. Ninguna de las 4.213 muestras supera 2.048 MiB | `Level1_Cave` (sesión `S-RNF2`) | P |
| RNF-06, tamaño del paquete | Menos de 500 MB | 426,6 MB (426.633.111 bytes, 406,9 MiB); margen de 73,4 MB | Carpeta entregable, 253 archivos | P |

En el desglose del paquete, las texturas pesan 273,1 MB, el 95 % de los activos de usuario (BuildReport, `evidencias/build-rc3.buildreport`). Para comparar, rc2 pesó 479,0 MB (01/10/2026), 21 MB por debajo del tope; la bajada a 426,6 MB sale de `INC-146` (cinco props del Nivel 1 importados a 256 px, 09/10/2026), que dejó el paquete en 420,6 MB sobre `4062ae3` antes de las imágenes nuevas. Las cifras de RNF-04 y RNF-05 en un segundo equipo son todas del equipo 1; las comprobaciones H3 y H5 del 13/10/2026, en otros dos equipos, están en el apartado 5.2.

Las otras mediciones sobre el ejecutable dieron lo siguiente (cada caso está en el Anexo A):

- RNF-10, sin conexiones: 0 conexiones TCP ni UDP del proceso en 4.516 muestras de 39 archivos, y 0 apariciones de `unity3d.com` en `globalgamemanagers`. La comprobación H4 (RNF-08, sin conexión a internet) se aprobó el 13/10/2026 (apartado 5.2); `PF-RNF08-01` pasó a P con ella.
- RNF-07 y RNF-11, portabilidad y residuos: la copia a una ruta con espacio (`D:\User\Desktop\Algoritmia prueba\`), con usuario estándar y el mismo SHA-256, arranca, crea `Datos/` y llega a `Level1_Cave` en 36 clics. Tras borrar los perfiles de prueba no queda su rastro en la carpeta, en `LocalLow`, en el registro ni en `Player.log`. Fuera de `Datos/` quedan `Player.log` y valores `unity.*`, `unity_connect.*` y `Screenmanager*` en `HKCU`: el residuo del motor ya registrado como DEF-RC2-01 (decisión D-RC2-01), que persiste igual que en rc2.
- RNF-16, exclusión de un nivel: en una copia desechable sin `Levels/Wheel` el proyecto compila con 0 errores y la suite da EditMode 657/663 y PlayMode 288/316; sin `Levels/Fire`, 690/696 y 329/354. El caso aprueba con una desviación, DEF-RC3-12 (apartado 3.6).
- RNF-18, parámetros sin recompilar: se cambió `MinimumEffectiveStrikes` a 2 y el texto de una línea en un ejecutable de la copia desechable, con solo dos `.asset` modificados. Aprueba como variante, porque no se probó con el Editor abierto y en Play.

## 3.6 Defectos

Se registraron 16 defectos en rc3: 0 Bloqueantes, 0 Mayores, 7 Menores y 9 Observaciones. Observación es lo que se anota sin que incumpla un RF ni un criterio de aceptación. `I03` e `I04` salen de verificaciones de apoyo (`INSP-ART-SPRITES` y `SUITE-T05`) y no de un caso `PF`. Todos siguen abiertos al cierre de la pasada.

**Tabla 3.6.** Defectos hallados en rc3, con su severidad y el tipo de arreglo propuesto.

| Defecto | Caso | Severidad | Qué ocurre | Tipo propuesto |
|---|---|---|---|---|
| DEF-RC3-01 | `PF-RF45-01` | Observación | El avatar de Algoritm en el resumen de nivel es un círculo con el texto «Algoritm», pese al diseño final del guía | Decisión pendiente (documento o arte) |
| DEF-RC3-02 | `PF-RF01-03` | Observación | `casos.md` describe la portada de otra manera: el panel ámbar queda a la izquierda y el bosque con los cinco personajes a la derecha; el criterio se cumple | Documento |
| DEF-RC3-03 | `PF-RF05-05` | Observación | El criterio «ningún personaje con la cara tapada» no contempla el gesto del Niño en reposo (la mano a la sien tapa parte de un ojo), registrado como excepción en el generador de poses | Documento o arte |
| DEF-RC3-04 | `PF-RF14-01` | Observación | Las siete piezas del Nivel 1 caen en sitios distintos en cada entrada; las coordenadas del caso no valen | Documento |
| DEF-RC3-05 | `PF-RNF03-01` (visto en `PF-RF46-01`) | Menor | Informe docente: los iconos de «Errores corregidos» y «Tiempo de resolución» cruzan la primera línea de su cabecera, los de «Intentos» y «Pasos utilizados» quedan sueltos y pequeños, y «Volver al menú» parte su texto en dos líneas | Código (disposición) o arte |
| DEF-RC3-06 | `PF-RF44-01` | Observación | El paso 21 de S-N3 no se puede reproducir como está escrito: la balsa arranca con la línea 4 y la última línea llega cuando los cuatro ya bajaron al pasto | Documento |
| DEF-RC3-07 | `PF-RF30-02` | Observación | Los colores medidos del laberinto difieren en un nivel de 8 bits de los del documento (`#6A5A45` frente a `#6A5A44`, `#A0320D` frente a `#A0330D`) | Documento (admitir ±1) o ninguno |
| DEF-RC3-08 | `PF-RF05-04` | Observación | Algoritm aparece con la forma del nivel anterior en las secuencias puente (llama en `N2_PuenteI`, disco en `N3_PuenteII`) | Decisión pendiente o documento |
| DEF-RC3-09 | `PF-RF45-05` | Menor | Un doble clic en «Omitir» del cierre del Nivel 1 deja en el menú de niveles sin ver el resumen (2 de 3 intentos); el progreso no se pierde | Código (ignorar la entrada unos 300 ms tras cargar el resumen) o ajustar el caso |
| DEF-RC3-10 | `PF-RNF03-01` | Menor | Bosque del Nivel 2: la instrucción de tres líneas se sale de su tablilla de dos | Código o escena (altura de la tablilla), o acortar la línea de `N2_Guia.asset` |
| DEF-RC3-11 | `PF-RF47-02` | Observación | «Eliminar datos» se parte en dos líneas dentro de su botón; la ruta de respaldo del diálogo de salida queda a unos 8 px de los botones | Disposición |
| DEF-RC3-12 | `PF-RNF16-01` | Observación | Cinco pruebas de EditMode fallan al retirar un nivel, en los dos sentidos, y el caso no las lista | Documento (listarlas como esperadas) o prueba (tolerar un nivel ausente) |
| DEF-RC3-I01 | `PF-RNF15-01` | Menor | Identificadores en español en `ForestSceneController` (20), `WorkshopSceneController` y `MazeSceneController` | Estilo de código |
| DEF-RC3-I02 | `PF-RNF17-01` | Menor | 9 de 122 commits que tocan `Assets/` llevan tarjeta del Kanban (7,4 %) | Proceso; no se corrige sin reescribir la historia |
| DEF-RC3-I03 | `INSP-ART-SPRITES` (apoyo) | Menor | Halo de croma en 9 PNG de caras de la familia (15 a 51 píxeles con alfa de 10 a 25 sobre 255); rc2 tenía 0 | Arte |
| DEF-RC3-I04 | `SUITE-T05` (apoyo) | Menor | La suite no corrió sobre el contenido exacto de rc3 (HEAD `292b108` y no `f49ccc9`) | Proceso |

Los cuatro F de la pasada salen de cinco de los defectos Menores: `PF-RF45-05` de DEF-RC3-09, `PF-RNF03-01` de DEF-RC3-10 y DEF-RC3-05, `PF-RNF15-01` de DEF-RC3-I01 y `PF-RNF17-01` de DEF-RC3-I02. DEF-RC3-05 se vio primero de paso en S-RNF y se confirmó en S-DOC, donde `PF-RF46-01` aprobó porque las cifras y los cuatro indicadores son correctos. El caso de DEF-RC3-09, `PF-RF45-05`, había aprobado en rc2 (01/10/2026) con un doble clic de 95 ms sobre «Omitir»; en rc3 falla con 100 ms según la coordenada del clic, y la causa probable es la geometría del botón «Volver al menú de niveles» del resumen, que carga en 13 ms y queda bajo ese segundo clic (se infiere de las capturas, sin leer el código).

[Pendiente: triaje de cada uno de los 16 defectos (corregir dentro del OE4, registrar un INC o dejar como trabajo futuro), con su fecha]

La primera pasada, sobre rc1 (01/10/2026), halló nueve defectos: cuatro Mayores propuestos (DEF-SPIKE-01, la conexión del reproductor a servicios de Unity, que incumplía RNF-10; DEF-W5R-01, la lista de perfiles que dejaba fuera del panel o cortados a los de la quinta fila en adelante; DEF-GP1-01, el bloque que se quedaba pegado al cursor en el laberinto; y DEF-GP1-02, el resaltado que no seguía a la lista desplazada) y cinco Menores (DEF-GP1-03, DEF-SPER-01, DEF-SPER-02, DEF-W5R-02 y OBS-SDOC-2). Se corrigieron con pruebas nuevas que nombran su requisito y se reverificaron sobre el ejecutable rc2 (01/10/2026), que dejó 89 casos en P y 13 en P parcial de 127, ninguno en F; el único residuo fue DEF-RC2-01. En rc3 los casos `PF-RF02-06`, `PF-RF20-03`, `PF-RF32-01`, `PF-RF34-01`, `PF-RF34-02` y `PF-RNF10-01`, que habían fallado en rc1, aprobaron, y ninguno de los nueve defectos de rc1 se reabrió. Los 16 defectos de la Tabla 3.6 son nuevos de esta pasada. El detalle de rc1 y rc2 vive en `OE4-Resultados.md` del commit `4e78f47`.

## 3.7 Suite automatizada como evidencia de apoyo

La suite no cuenta en los indicadores (apartado 1.3). Se corrió el 10/10/2026 con `suite2.ps1`, que reparte las pruebas entre dos Editores: 25,9 min, con 1.167 pruebas listadas y las 1.167 ejecutadas.

**Tabla 3.7.** Suite automatizada del 10/10/2026.

| Corrida | Pruebas | Pasan | Fallan | Omitidas |
|---|---|---|---|---|
| EditMode | 749 | 748 | 0 | 1 |
| PlayMode | 418 | 417 | 1 | 0 |
| Total | 1.167 | 1.165 | 1 | 1 |

La omitida es `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`, que se salta por el entorno. El fallo es `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`: mide un salto de cuadro de 0,056 frente al límite de 0,05 con dos Editores compitiendo por CPU, y pasa corrida sola (1 de 1). La suite corrió sobre `292b108` más el árbol de la ronda, registrada en `23d7e8c`, y no sobre el contenido exacto de rc3 (DEF-RC3-I04). Entre `23d7e8c` y rc3 no cambió ningún `.cs`, aunque sí `Level2_Maze.unity` (78 líneas de reserialización del Editor, en `f49ccc9`) y se borraron 6 PNG de cara sin referencias.

La suite del candidato rc4 (13/10/2026) dio EditMode 785 de 786 (una omitida por entorno) y PlayMode 427 de 427, con 1.213 pruebas listadas y las 1.213 ejecutadas; el apartado 5.3 da su contexto.

Cada uno de los 47 RF tiene al menos una prueba automática que lo nombra (el mínimo es 1, en RF-21 y RF-27). Ninguna prueba nombra RNF-08, RNF-12, RNF-17 ni RNF-22, que se verifican por inspección o por documento. El recuento de pruebas por requerimiento está en el apartado 8 de `OE4-Resultados.md`.
