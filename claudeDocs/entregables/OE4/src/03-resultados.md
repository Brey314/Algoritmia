# 3. RESULTADOS

Todos los veredictos de este capítulo son los de rc3 (apartado 2.6), obtenidos el 10/10/2026 y consolidados en `claudeDocs/tasks/OE4/OE4-Resultados.md`. Donde un caso se repitió después de una corrección, vale el veredicto del candidato más reciente; la primera pasada se conserva en el registro de trabajo y se resume en el apartado 3.6.

## 3.1 Veredictos por sesión

**Tabla 3.1.** Veredictos de rc3 por sesión. La eficacia es (P + PD) / (P + PD + F).

{{TABLA_RESULTADOS_SESION}}

En las sesiones de recorridos jugados con el arnés (S-INI a S-DOC) todos los casos salieron P, salvo uno de S-OMI: `PF-RF45-05`, por el defecto DEF-RC3-09 (apartado 3.6). Un caso repartido entre sesiones, como `PF-RF05-04` (partes en los Niveles 1, 2 y 3), cuenta una vez con el peor veredicto de sus partes, y todos esos salieron P. Un caso de S-INSP que quedó en B y se ejecutó después sobre el ejecutable, como `PF-RNF03-01` o `PF-RNF16-01`, cuenta con el veredicto del ejecutable.

S-INSP concentra los otros tres F, que son de requerimientos no funcionales: `PF-RNF03-01` (DEF-RC3-10 y DEF-RC3-05, dos pantallas con un texto fuera de su tablilla y unos iconos sobre el texto), `PF-RNF15-01` (DEF-RC3-I01, identificadores en español) y `PF-RNF17-01` (DEF-RC3-I02, commits sin tarjeta). Ninguno de los tres cambia lo que el juego hace. Por eso su eficacia, 72,7 % (8 de 11), no dice nada sobre la mecánica.

Tres casos están en PS. Uno es de S-HUM: `PF-RNF13-01`, el recorrido completo, porque los dos recorridos del arnés sobre rc3 no se ejecutaron y la comprobación H8 registró la duración (40 a 45 min) pero no cubre por sí sola los tres recorridos que el caso pide (apartado 3.4). Dos son de S-INSP: `PF-RNF22-01` (la inspección está hecha, con 344 cadenas sin enlaces ni compras y 9 ilustraciones sin violencia, y falta la confirmación) y `PF-RNF23-01` (los créditos y las licencias coinciden con lo esperado, y falta el soporte escrito de la Familia Anonaky y de la colaboradora de arte). No hay casos en B, NA ni NE.

En el corte del 10/10/2026 eran diez los casos en PS (126 con veredicto: 122 P y 4 F, eficacia 96,8 %). Siete pasaron a P el 13/10/2026. Seis lo hicieron con la hoja HUM, en una sesión manual sobre rc3 con 4 estudiantes de grado quinto (apartado 5.2), cada uno con la comprobación que lo cubre y que se registró como «Aprobado»: `PF-RNF07-02` (H3, segundo equipo), `PF-RNF08-01` (H4, sin conexión a internet), `PF-CT02-01` (H5, equipo sin tarjeta gráfica dedicada) y `PF-SON-01` a `PF-SON-03` (H12, con H6 y H7 para `PF-SON-03`). El séptimo, `PF-RNF12-01`, pasó a P con el formato de consentimiento informado, que consta en el Anexo F, y los 4 consentimientos firmados de la sesión del 13/10/2026. Las comprobaciones H1 y H2 (PG-05 y PG-06) y H9 a H11 no corresponden a ningún caso nuevo, y H13 no se realizó: `PF-RNF02-01`, `PF-RF34-01` y `PF-RF46-01` conservan el P de rc3.

## 3.2 Veredictos por etiqueta

El indicador de eficacia nombra los botones, los sonidos y los saltos de nivel. La Tabla 3.2 reparte los mismos casos por etiqueta para leerlo así.

**Tabla 3.2.** Veredictos de rc3 por etiqueta.

{{TABLA_RESULTADOS_ETIQUETA}}

NAV tiene 18 casos: 16 en P, 1 en F (`PF-RF45-05`) y 1 en PS (`PF-RNF13-01`, el recorrido completo). BOT cierra los 28 en P. SON queda en 100,0 % (3 de 3): sus tres casos son de oído, el arnés no mide el sonido y los aprobó una persona el 13/10/2026 (comprobaciones H12, H6 y H7 de la hoja HUM), así que es un juicio de oído y no una medición. La etiqueta RNF queda en 83,3 % (15 de 18 con veredicto) por los tres F de S-INSP; era 78,6 % (11 de 14) en el corte del 10/10/2026, antes de que H3, H4, H5 y el consentimiento pasaran a P. El indicador no nombra esa etiqueta.

## 3.3 Indicadores del objetivo

**Tabla 3.3.** Los dos indicadores del OE4 sobre rc3.

| Indicador | Meta | Resultado en rc3 |
|---|---|---|
| Eficacia de las pruebas funcionales | 90 % o más | 97,0 % = 129 / (129 + 0 + 4), sobre 133 casos con veredicto; 3 casos en PS fuera del cálculo. En el corte del 10/10/2026 era 96,8 % = 122 / (122 + 0 + 4), sobre 126 |
| Eficacia en las etiquetas `NAV`, `BOT` y `SON` | 90 % o más cada una | `NAV` 94,1 % (16 de 17) · `BOT` 100,0 % (28 de 28) · `SON` 100,0 % (3 de 3, aprobados en la sesión manual del 13/10/2026; en el corte del 10/10/2026, sin calcular) |
| RF de prioridad Alta con algún F Bloqueante o Mayor abierto | Ninguno de los 45 | Ninguno: 45 de 45 RF de prioridad Alta sin F Bloqueante ni Mayor. El único F que toca un RF de prioridad Alta es `PF-RF45-05` (RF-45), de severidad Menor |

Los 4 F de la pasada son todos Menores y ninguno de los 16 defectos hallados es Bloqueante ni Mayor (apartado 3.6). Con la sesión del 13/10/2026 las tres etiquetas que nombra el indicador superan la meta, y la de sonidos se cierra con un juicio de oído de una persona y no con una medición. La eficacia global depende todavía de los 3 casos en PS: si los tres fallaran quedaría en 94,9 % (129 de 136) y seguiría sobre el 90 %.

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

**Tabla 3.5.** Presupuestos de rendimiento y de paquete en rc3 (10/10/2026) y en rc4 (13/10/2026).

| Requerimiento | Umbral | rc3 | rc4 | Veredicto (rc3) |
|---|---|---|---|---|
| RNF-04, carga de cualquier escena | Menos de 10 s | 0,268 s, el peor de las 12 escenas (media 0,089 s, n = 26); `Boot` a `MainMenu`, unos 3,4 s. Escena `Narrative` (`S-RNF/rnf04_tabla.csv`) | 0,403 s, el peor de 46 cargas en 4 lanzamientos (n = 1 a 2 por escena, salvo 17 en `Narrative`); `Boot` a `MainMenu`, 2,76 s. Escena `Narrative` (`evidencias/build-rc4.md`) | P |
| RNF-05, memoria | Menos de 2 GB | 1.094 MiB (1,15 GB) de memoria privada; 438 MiB de memoria de trabajo. Ninguna de las 4.213 muestras supera 2.048 MiB. `Level1_Cave` (sesión `S-RNF2`) | 1.083,3 MiB (1,06 GiB) de memoria privada, en `Level1_Cave`; 434,6 MiB de memoria de trabajo, en `Level3_River`. 373 muestras, ninguna sobre 2.048 MiB | P |
| RNF-06, tamaño del paquete | Menos de 500 MB | 426,6 MB (426.633.111 bytes, 406,9 MiB); margen de 73,4 MB. Carpeta entregable, 253 archivos | 426,9 MB (426.932.086 bytes, 407,2 MiB); margen de 73,1 MB. Carpeta entregable, 252 archivos | P |

Fuente de rc4: `claudeDocs/tasks/OE4/evidencias/build-rc4.md`, medido con `oe4.ps1` sobre el ejecutable, a 1920 × 1080 en el equipo 1.

En el desglose del paquete, las texturas pesan 273,1 MB, el 95 % de los activos de usuario (BuildReport, `evidencias/build-rc3.buildreport`). Para comparar, rc2 pesó 479,0 MB (01/10/2026), 21 MB por debajo del tope; la bajada a 426,6 MB sale de `INC-146` (cinco props del Nivel 1 importados a 256 px, 09/10/2026), que dejó el paquete en 420,6 MB sobre `4062ae3` antes de las imágenes nuevas. Las mediciones de rc4 confirman las de rc3 y no las sustituyen como evidencia de `PF-RNF04-01`: ese caso pide tres cargas por escena y rc4 tiene una o dos (17 en `Narrative`), por lo que la tabla de rc3 sigue siendo la del caso. En RNF-10, las 373 filas de red del muestreo de rc4 no registraron ninguna conexión TCP ni UDP a una dirección que no fuera de loopback. Las cifras de RNF-04 y RNF-05 en un segundo equipo son todas del equipo 1; las comprobaciones H3 y H5 del 13/10/2026, en otros dos equipos, están en el apartado 5.2.

Las otras mediciones sobre el ejecutable dieron lo siguiente (cada caso está en el Anexo A):

- RNF-10, sin conexiones: 0 conexiones TCP ni UDP del proceso en 4.516 muestras de 39 archivos, y 0 apariciones de `unity3d.com` en `globalgamemanagers`. La comprobación H4 (RNF-08, sin conexión a internet) se aprobó el 13/10/2026 (apartado 5.2); `PF-RNF08-01` pasó a P con ella.
- RNF-07 y RNF-11, portabilidad y residuos: la copia a una ruta con espacio (`D:\User\Desktop\Algoritmia prueba\`), con usuario estándar y el mismo SHA-256, arranca, crea `Datos/` y llega a `Level1_Cave` en 36 clics. Tras borrar los perfiles de prueba no queda su rastro en la carpeta, en `LocalLow`, en el registro ni en `Player.log`. Fuera de `Datos/` quedan `Player.log` y valores `unity.*`, `unity_connect.*` y `Screenmanager*` en `HKCU`: el residuo del motor ya registrado como DEF-RC2-01 (decisión D-RC2-01), que persiste igual que en rc2.
- RNF-16, exclusión de un nivel: en una copia desechable sin `Levels/Wheel` el proyecto compila con 0 errores y la suite da EditMode 657/663 y PlayMode 288/316; sin `Levels/Fire`, 690/696 y 329/354. El caso aprueba con una desviación, DEF-RC3-12 (apartado 3.6).
- RNF-18, parámetros sin recompilar: se cambió `MinimumEffectiveStrikes` a 2 y el texto de una línea en un ejecutable de la copia desechable, con solo dos `.asset` modificados. Aprueba como variante, porque no se probó con el Editor abierto y en Play.

## 3.6 Defectos

Se registraron 16 defectos en rc3: 0 Bloqueantes, 0 Mayores, 7 Menores y 9 Observaciones. Observación es lo que se anota sin que incumpla un RF ni un criterio de aceptación. `I03` e `I04` salen de verificaciones de apoyo (`INSP-ART-SPRITES` y `SUITE-T05`) y no de un caso `PF`. Todos siguen abiertos al cierre de la pasada; el triaje del 13/10/2026 se recoge más abajo.

**Tabla 3.6.** Defectos hallados en rc3, con su severidad, el tipo de arreglo propuesto y la decisión del triaje del 13/10/2026.

| Defecto | Caso | Severidad | Qué ocurre | Tipo propuesto | Triaje del 13/10/2026 |
|---|---|---|---|---|---|
| DEF-RC3-01 | `PF-RF45-01` | Observación | El avatar de Algoritm en el resumen de nivel es un círculo con el texto «Algoritm», pese al diseño final del guía | Corregido en el código el 14/10/2026 (`86a9898`, posterior a rc4) | Trabajo futuro en el triaje del 13/10/2026; corregido después, entra en el siguiente candidato |
| DEF-RC3-02 | `PF-RF01-03` | Observación | `casos.md` describe la portada de otra manera: el panel ámbar queda a la izquierda y el bosque con los cinco personajes a la derecha; el criterio se cumple | Documento | Documento: `casos.md` describe la portada como es |
| DEF-RC3-03 | `PF-RF05-05` | Observación | El criterio «ningún personaje con la cara tapada» no contempla el gesto del Niño en reposo (la mano a la sien tapa parte de un ojo), registrado como excepción en el generador de poses | Documento o arte | Trabajo futuro: documento o arte, lo decide el equipo de arte |
| DEF-RC3-04 | `PF-RF14-01` | Observación | Las siete piezas del Nivel 1 caen en sitios distintos en cada entrada; las coordenadas del caso no valen | Documento | Documento: `casos.md` sin coordenadas fijas de las piezas |
| DEF-RC3-05 | `PF-RNF03-01` (visto en `PF-RF46-01`) | Menor | Informe docente: los iconos de «Errores corregidos» y «Tiempo de resolución» cruzan la primera línea de su cabecera, los de «Intentos» y «Pasos utilizados» quedan sueltos y pequeños, y «Volver al menú» parte su texto en dos líneas | Código (disposición) o arte | Código: se corrige en rc4 |
| DEF-RC3-06 | `PF-RF44-01` | Observación | El paso 21 de S-N3 no se puede reproducir como está escrito: la balsa arranca con la línea 4 y la última línea llega cuando los cuatro ya bajaron al pasto | Documento | Documento: el paso 21 de S-N3 como se reproduce |
| DEF-RC3-07 | `PF-RF30-02` | Observación | Los colores medidos del laberinto difieren en un nivel de 8 bits de los del documento (`#6A5A45` frente a `#6A5A44`, `#A0320D` frente a `#A0330D`) | Documento (admitir ±1) o ninguno | Documento: se admite ±1 en 8 bits |
| DEF-RC3-08 | `PF-RF05-04` | Observación | Algoritm aparece con la forma del nivel anterior en las secuencias puente (llama en `N2_PuenteI`, disco en `N3_PuenteII`) | Decisión pendiente o documento | Trabajo futuro: confirmar si la forma es intencional, o documento |
| DEF-RC3-09 | `PF-RF45-05` | Menor | Un doble clic en «Omitir» del cierre del Nivel 1 deja en el menú de niveles sin ver el resumen (2 de 3 intentos); el progreso no se pierde | Código (ignorar la entrada unos 300 ms tras cargar el resumen) o ajustar el caso | Código: se corrige en rc4 |
| DEF-RC3-10 | `PF-RNF03-01` | Menor | Bosque del Nivel 2: la instrucción de tres líneas se sale de su tablilla de dos | Código o escena (altura de la tablilla), o acortar la línea de `N2_Guia.asset` | Código: se corrige en rc4 |
| DEF-RC3-11 | `PF-RF47-02` | Observación | «Eliminar datos» se parte en dos líneas dentro de su botón; la ruta de respaldo del diálogo de salida queda a unos 8 px de los botones | Disposición | Código: se corrige en rc4 |
| DEF-RC3-12 | `PF-RNF16-01` | Observación | Cinco pruebas de EditMode fallan al retirar un nivel, en los dos sentidos, y el caso no las lista | Documento (listarlas como esperadas) o prueba (tolerar un nivel ausente) | Documento: las cinco pruebas se listan como esperadas |
| DEF-RC3-I01 | `PF-RNF15-01` | Menor | Identificadores en español en `ForestSceneController` (20), `WorkshopSceneController` y `MazeSceneController` | Estilo de código | Trabajo futuro: estilo de código |
| DEF-RC3-I02 | `PF-RNF17-01` | Menor | 9 de 122 commits que tocan `Assets/` llevan tarjeta del Kanban (7,4 %) | Proceso; no se corrige sin reescribir la historia | Trabajo futuro: proceso; no se corrige sin reescribir la historia |
| DEF-RC3-I03 | `INSP-ART-SPRITES` (apoyo) | Menor | Halo de croma en 9 PNG de caras de la familia (15 a 51 píxeles con alfa de 10 a 25 sobre 255); rc2 tenía 0 | Arte | Trabajo futuro: arte |
| DEF-RC3-I04 | `SUITE-T05` (apoyo) | Menor | La suite no corrió sobre el contenido exacto de rc3 (HEAD `292b108` y no `f49ccc9`) | Proceso | Trabajo futuro: proceso |

Los cuatro F de la pasada salen de cinco de los defectos Menores: `PF-RF45-05` de DEF-RC3-09, `PF-RNF03-01` de DEF-RC3-10 y DEF-RC3-05, `PF-RNF15-01` de DEF-RC3-I01 y `PF-RNF17-01` de DEF-RC3-I02. DEF-RC3-05 se vio primero de paso en S-RNF y se confirmó en S-DOC, donde `PF-RF46-01` aprobó porque las cifras y los cuatro indicadores son correctos. El caso de DEF-RC3-09, `PF-RF45-05`, había aprobado en rc2 (01/10/2026) con un doble clic de 95 ms sobre «Omitir»; en rc3 falla con 100 ms según la coordenada del clic, y la causa probable es la geometría del botón «Volver al menú de niveles» del resumen, que carga en 13 ms y queda bajo ese segundo clic (se infiere de las capturas, sin leer el código).

**Triaje del 13/10/2026.** Los 16 defectos se repartieron en tres destinos. Ninguno estaba corregido en rc3, y esta decisión no cambia ningún veredicto de las Tablas 3.1 a 3.3.

- **Se corrigen en el documento de casos (5):** DEF-RC3-02, -04, -06, -07 y -12. En `casos.md`, `PF-RF01-03` describe la portada como es (panel ámbar a la izquierda con el título, el lema y los botones, y el bosque con los cinco personajes a la derecha); `PF-RF14-01` no promete coordenadas de las siete piezas del Nivel 1, que caen en sitios distintos en cada entrada, y pide leer la pantalla antes de arrastrar; el paso 21 de S-N3 (`PF-RF44-01`) pide los clics hasta la línea 4, que es cuando arranca la balsa, y fotografiar entonces; `PF-RF30-02` admite ±1 en cada canal de 8 bits (`#6A5A45` frente a `#6A5A44`, `#A0320D` frente a `#A0330D`); y `PF-RNF16-01` lista como esperadas las cinco pruebas que fallan al retirar un nivel en los dos sentidos: `Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas`, `Architecture_RNF15_ElNombreDelAssemblyCoincideConSuRutaBajoScripts`, `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel`, `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` y `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra`.
- **Se corrigen en el código, en rc4 (4):** DEF-RC3-05, -09, -10 y -11. Quedaron corregidas en el commit `0405eba` (13/10/2026) y entraron en rc4: el informe docente ya no monta las cabeceras con sus iconos (DEF-RC3-05); `InputGuard` ignora los clics durante 0,3 s tras cada carga de escena (DEF-RC3-09); la tablilla del bosque es más ancha y caben las tres líneas (DEF-RC3-10); y los textos de los botones del informe docente y de los diálogos de borrar y salir ya no se parten (DEF-RC3-11). Pruebas nuevas las vigilan (`InputGuard`, `TeacherReport`, `EraseDialog`, `MainMenu` y `ForestScene`; `evidencias/build-rc4.md`). Los cuatro F de la Tabla 3.1 conservan el veredicto de rc3, porque los casos `PF-*` no se volvieron a ejecutar sobre rc4.
- **Pasan a trabajo futuro (7):** DEF-RC3-01, porque el resumen de nivel conserva un círculo con el texto «Algoritm» pese al diseño final del guía y falta decidir si se cambia el documento o el arte; DEF-RC3-03, porque el gesto del Niño en reposo, con la mano a la sien, está registrado como excepción en el generador de poses y el criterio de `PF-RF05-05` no lo contempla, y lo decide el equipo de arte; DEF-RC3-08, porque Algoritm aparece con la forma del nivel anterior en las secuencias puente y falta confirmar si es intencional; DEF-RC3-I01, por los identificadores en español de tres controladores del Nivel 2, que es estilo de código; DEF-RC3-I02, porque solo 9 de 122 commits llevan tarjeta y corregirlo exigiría reescribir la historia; DEF-RC3-I03, por el halo de croma de nueve caras de la familia, que es arte; y DEF-RC3-I04, porque la suite no corrió sobre el contenido exacto de rc3, que es proceso.

El 14/10/2026, después del triaje, DEF-RC3-01 se corrigió en el código (commit `86a9898`): el avatar del resumen de nivel es ahora el guía con su diseño final, en la forma del nivel terminado (llama, disco de madera o gota). La corrección es posterior a rc4, que se compiló desde `0405eba` y no se recompiló: el ejecutable rc4 todavía muestra el círculo, y la corrección entra en el siguiente candidato. Las pruebas `LevelSummary_DEFRC301_*` pasan (3 en EditMode y 3 en PlayMode), pero el veredicto de `PF-RF45-01` sigue siendo el de rc3.

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

La suite del candidato rc4 (13/10/2026, `suite2.ps1`, 24,4 min) corrió sobre `0405eba`, el mismo commit del que se compiló: EditMode 793 pruebas, 792 pasan y una se omite por entorno, y PlayMode 435 de 435, con 1.228 pruebas listadas y las 1.228 ejecutadas, repetidas 0 y ninguna fallida (`evidencias/suites/rc4-0405eba/resumen.md`). Una suite anterior, sobre `0839352`, dio EditMode 785 de 786 y PlayMode 427 de 427; el apartado 5.3 da su contexto.

Cada uno de los 47 RF tiene al menos una prueba automática que lo nombra (el mínimo es 1, en RF-21 y RF-27). Ninguna prueba nombra RNF-08, RNF-12, RNF-17 ni RNF-22, que se verifican por inspección o por documento. El recuento de pruebas por requerimiento está en el apartado 8 de `OE4-Resultados.md`.
