# 3. RESULTADOS

Los veredictos de este capítulo son los de rc5 (apartado 2.6), obtenidos el 04 y el 05/11/2026 y consolidados en `src/veredictos-rc5.csv`, con una fila por caso. Cada cifra de rc3 se cita como tal: es la primera pasada y sirve de referencia.

## 3.1 Veredictos por sesión

**Tabla 3.1.** Veredictos de rc5 por sesión. La eficacia es (P + PD) / (P + PD + F); NE queda fuera del denominador.

{{TABLA_RESULTADOS_SESION}}

De los {{N_CASOS}} casos, 130 terminan en P, 8 en PD y 4 en F, y 3 no se ejecutaron. La eficacia es (130 + 8) / (130 + 8 + 4) = 138 / 142 = 97,2 %. La de rc3 fue de 97,1 % (132 de 136), sobre otro universo: el apartado 3.9 da la cuenta caso por caso.

Los cuatro F son estos, y ninguno cambia el resultado de un reto:

- `PF-RF21-02` (DEF-RC5-10, Menor, S-OMI). RF-21 pide que la luz alcance su máximo al soplar. Con exactamente tres golpes efectivos así ocurre: el borde de la cueva sube de 36,91 a 38,37 en una escala de 0 a 255. Con un cuarto golpe la luz llega a 38,35 antes de soplar y el soplido ya no aclara nada. RF-21 es de prioridad Baja.
- `PF-RNF15-01` (DEF-RC5-16, Menor, S-INSP). 3 de las 10 clases de la muestra usan 23 identificadores en español, la misma cuenta que en rc3 (DEF-RC3-I01).
- `PF-RNF17-01` (DEF-RC5-17, Observación, S-INSP). 9 de 92 commits de código llevan tarjeta del Kanban (9,8 %). Continúa DEF-RC3-I02 y no se corrige sin reescribir la historia.
- `PF-RNF20-01` (DEF-RC5-15, Menor, S-INSP). La etiqueta «Arrastra un bloque a tu secuencia» del laberinto tiene un contraste de 3,15:1 frente al mínimo de 4,5:1; las otras 61 medidas dan de 4,88:1 a 16,54:1.

Los ocho PD siguen una decisión registrada. `PF-RF31-01`: el bloque «Retroceder» termina en un círculo y el diseño radicado pide un rombo; se decidió conservar lo implementado, porque las tres formas se distinguen, y el OE2 radicado no se editó (DEF-RC5-04, `INC-161`). `PF-RNF16-01` tuvo primera lectura F: el criterio solo admite los fallos que lista, y al retirar un nivel fallaron además 13 pruebas de PlayMode sin el Nivel 2 y 17 sin el Nivel 1. Se reclasificó porque esas 17 pruebas distintas fallan igual en la copia de `ca6fd91` sin retirar nada, por la Game View de 640 × 480 del modo por lotes, y en el Editor a 1920 × 1080 la suite de rc5 da 438 de 438: retirar un nivel no las causa. Esas 17 no aportan evidencia en esas corridas (DEF-RC5-20, `INC-162`).

Los otros seis son `PF-RNF04-01`, `PF-RNF05-01`, `PF-RNF07-02`, `PF-CT02-01`, `PF-RNF08-01` y `PF-RNF13-01`. Su parte humana estaba prevista sobre rc5 en una sesión que no se hizo, y el 05/11/2026 se decidió cerrar sobre rc5 con lo que ya existía: la sesión del curso 501 y la medición del 16/10/2026 (`INC-163`, `evidencias/rc5/S-COLEGIO/`). Ese día no se siguió el guion del catálogo, de ahí la desviación; los límites están en el apartado 5.5.

Los tres casos en NE son los de oído, `PF-SON-01` a `PF-SON-03`: nadie marcó las piezas de sonido sobre rc5. Aprobaron sobre rc3 el 13/10/2026 (apartado 5.2), y ese resultado no se hereda. Entre `f49ccc9` y `ca6fd91` no cambió ningún archivo de audio ni sus scripts, pero sí cambiaron narrativas y sus tiempos, así que esa comprobación no equivale a oír el juego sobre rc5.

Tres P tienen un alcance que conviene leer con su fila del Anexo A. `PF-RNF12-01`, `PF-RNF22-01` y `PF-RNF23-01` no se repitieron porque no dependen de la versión: el formato de consentimiento, las autorizaciones firmadas y la confirmación humana del contenido son de octubre, y sobre rc5 solo se rehízo la inspección de textos e ilustraciones de `PF-RNF22-01` y la lectura de los créditos de `PF-RNF23-01`.

## 3.2 Veredictos por etiqueta

El indicador de eficacia nombra los botones, los sonidos y los saltos de nivel. La Tabla 3.2 reparte los mismos casos por etiqueta para leerlo así.

**Tabla 3.2.** Veredictos de rc5 por etiqueta.

{{TABLA_RESULTADOS_ETIQUETA}}

NAV llega a 100,0 % (21 de 21: 20 P y 1 PD). BOT llega a 100,0 % (33 de 33: 32 P y 1 PD) y DAT a 100,0 % (30 de 30). RETO queda en 97,4 % (37 de 38) por `PF-RF21-02`. **La etiqueta RNF queda en 85,0 % (17 de 20), por debajo de 90 %**, con tres F; el indicador no la nombra, pero el resultado se declara. SON no tiene dato sobre rc5: sus tres casos no se ejecutaron, y la meta de sonidos no queda demostrada sobre esta versión.

Sobre rc3 las etiquetas dieron NAV 94,4 % (17 de 18), BOT 100,0 % (28 de 28), RETO 100,0 % (37 de 37), DAT 100,0 % (30 de 30), SON 100,0 % (3 de 3, juicio de oído del 13/10/2026) y RNF 85,0 % (17 de 20).

## 3.3 Indicadores del objetivo

**Tabla 3.3.** Los dos indicadores del OE4 sobre rc5, con la referencia de rc3.

| Indicador | Meta | Resultado en rc5 | Referencia en rc3 |
|---|---|---|---|
| Eficacia de las pruebas funcionales | 90 % o más | 97,2 % = (130 + 8) / (130 + 8 + 4), 138 de 142, sobre {{N_CASOS}} casos, 3 de ellos sin ejecutar | 97,1 % (132 de 136). Con solo veredictos obtenidos sobre rc3, 97,0 % (131 de 135) |
| Eficacia en las etiquetas `NAV`, `BOT` y `SON` | 90 % o más cada una | `NAV` 100,0 % (21 de 21) · `BOT` 100,0 % (33 de 33) · `SON` sin dato: la meta no queda demostrada sobre rc5 | `NAV` 94,4 % (17 de 18) · `BOT` 100,0 % (28 de 28) · `SON` 100,0 % (3 de 3) |
| RF de prioridad Alta con algún F Bloqueante o Mayor abierto | Ninguno de los 45 | Ninguno: 45 de 45. Ningún caso de un RF de prioridad Alta termina en F; el único F sobre un RF es `PF-RF21-02`, de RF-21, prioridad Baja | Ninguno: 45 de 45. El único F sobre un RF de prioridad Alta fue `PF-RF45-05`, Menor |

Sobre rc5 el primer indicador se cumple en su valor global y en `NAV` y `BOT`; en sonidos la meta no queda demostrada. Según el resultado de los 3 de sonido, la eficacia quedaría entre 95,2 % (138 de 145) y 97,2 % (141 de 145), por encima de la meta en los dos extremos. Ningún defecto de rc5 es Bloqueante ni Mayor.

## 3.4 Recorridos completos del juego

El caso `PF-RNF13-01` exige recorrer el juego entero, de la pantalla de inicio a los créditos y sin omitir ninguna narrativa, sin bloqueos, cierres inesperados ni estados irrecuperables, en tres recorridos: dos del arnés y uno con cronómetro (H8). El tercero no se hizo sobre rc5; en su lugar cuenta el curso de 30 estudiantes del 16/10/2026, que terminó los tres niveles sin ninguna complicación, y por eso el caso queda en PD. Los dos del arnés se hicieron el 05/11/2026 sobre rc5, cada uno con un perfil nuevo y sin «Omitir» (`evidencias/rc5/GP2/`). Terminaron con los tres niveles completados, las siete fases confirmadas, 31 cargas de escena y sin `Exception`, `Error` ni `Assert` en `Player.log`. El equipo tenía un solo monitor de 1920 × 1080, así que la pantalla completa fue de esa resolución. Un tercer intento se interrumpió y se descartó (`GP2/interrumpido/`). Los recorridos del 16/10/2026, con los que se cerró el caso en la primera pasada, se hicieron con Word abierto, de modo que sus cargas y su memoria son indicativas y no entran aquí.

Las duraciones del arnés incluyen la latencia de cada llamada y no miden a un estudiante. La duración del recorrido sobre rc5 no se registró. Los 40 a 45 minutos son de H8, sobre rc3 (Tabla 3.4), y no son un resultado de rc5.

**Tabla 3.4.** Recorridos completos del juego.

| Recorrido | Condición | Duración | Niveles | `Exception` en `Player.log` |
|---|---|---|---|---|
| Arnés 1, rc5, 05/11/2026 | Ventana de 1920 × 1080, perfil nuevo GP6V | Unos 15 min (con la latencia del arnés) | Los tres niveles completados | Ninguna; 31 cargas de escena, la peor de 0,680 s |
| Arnés 2, rc5, 05/11/2026 | Pantalla completa de 1920 × 1080, perfil nuevo GP7C | Unos 14 min (con la latencia del arnés) | Los tres niveles completados | Ninguna; 31 cargas de escena, la peor de 0,714 s |
| Curso 501, rc5, 16/10/2026 | 30 estudiantes, 30 equipos del colegio | No se registró | Los tres niveles terminados, según el registro de la sesión | Solo hay log de un equipo y del Nivel 1, sin excepciones |
| Comprobación H8, rc3, 13/10/2026 | Cronómetro (hoja HUM) | Entre 40 y 45 min; supera en hasta 5 min el tope de 20 a 40 min del OE1, sin causa registrada | Sin dato | Sin dato |

## 3.5 Requerimientos no funcionales medidos en el ejecutable

Los tres presupuestos duros del proyecto se miden sobre el ejecutable y no se estiman. Las cargas salen de la línea `RNF-04` que `SceneLoader` deja en `Player.log`. `Boot` no pasa por `SceneLoader`: el arranque hasta `MainMenu` se toma entre el inicio del proceso y la última escritura del log, y es una cota, no el primer cuadro visible. La memoria sale del muestreo de `WorkingSet64` y `PrivateMemorySize64` del proceso cada 2 s, en 31 sesiones sobre rc5. El tamaño es el de la carpeta entregable sin `Datos/` ni las carpetas `*_DoNotShip`. Las cifras de la Tabla 3.5 son del equipo 1. Las del equipo 2 son de un equipo del colegio, solo durante el Nivel 1 (más abajo), y por eso `PF-RNF04-01` y `PF-RNF05-01` quedan en PD.

**Tabla 3.5.** Presupuestos de rendimiento y de paquete en rc5 (05/11/2026), con rc3 (10/10/2026) y rc4 (13/10/2026) como referencia.

| Requerimiento | Umbral | rc5 | rc3 | rc4 | Veredicto (rc5) |
|---|---|---|---|---|---|
| RNF-04, carga de cualquier escena | Menos de 10 s | 0,714 s, la peor dentro del juego, en `Narrative` (n = 188, media 0,252 s); de 3 a 188 mediciones por escena. Arranque hasta `MainMenu`: 1,48 s, como cota, en 3 lanzamientos | 0,268 s, en `Narrative` (media 0,089 s, n = 26); `Boot` a `MainMenu`, unos 3,4 s | 0,403 s, en `Narrative`, sobre 46 cargas en 4 lanzamientos; `Boot` a `MainMenu`, 2,76 s | PD |
| RNF-05, memoria | Menos de 2 GB | 1.223 MiB de memoria privada, en `Level1_Cave`; 475 MiB de memoria de trabajo, en `Narrative` | 1.094 MiB de memoria privada; 438 MiB de memoria de trabajo. `Level1_Cave` | 1.083,3 MiB de memoria privada, en `Level1_Cave`; 434,6 MiB de memoria de trabajo, en `Level3_River` | PD |
| RNF-06, tamaño del paquete | Menos de 500 MB | 426,9 MB (426 935 666 bytes, 407,2 MiB); margen de 73,1 MB | 426,6 MB (426.633.111 bytes); margen de 73,4 MB | 426,9 MB (426.932.086 bytes); margen de 73,1 MB | P |

Fuentes: `evidencias/rc5/S-RNF/` para rc5, `evidencias/rc3/S-RNF/` para rc3 y `evidencias/build-rc4.md` para rc4.

Frente a rc3 la peor carga dentro del juego subió de 0,268 s a 0,714 s y la memoria privada máxima de 1.094 MiB a 1.223 MiB. Las dos siguen lejos del límite (7 % y 60 % de él), y las de rc5 reúnen más mediciones. No se investigó la causa de la subida. En el desglose del paquete, las texturas pesan 273,1 MB, el 95 % de los activos de usuario (BuildReport de rc3, `evidencias/build-rc3.buildreport`). rc2 pesó 479,0 MB (01/10/2026), 21 MB por debajo del tope; la bajada sale de `INC-146` (cinco props del Nivel 1 importados a 256 px, 09/10/2026).

**Equipo 2: un equipo del colegio, sobre rc5.** El 16/10/2026, durante la sesión del apartado 5.4, se midió rc5 con el medidor compilado `MedirAlgoritmia.exe` (`claudeDocs/tasks/OE4/herramientas/medidor/`) en uno de los equipos del colegio: un AMD A10-9600P con gráficos integrados Radeon R5 y 3,9 GB de RAM, bajo Windows 10 Education, el mismo modelo del equipo A de la Tabla 5.3. El recorrido duró 0,9 min y pasó por `MainMenu`, `LevelSelect`, `Narrative` (tres cargas) y `Level1_Cave`, con 200 muestras de memoria. La peor carga fue de 1,223 s (`MainMenu`; `Narrative`, 1,075 s), frente a los 10 s de RNF-04, y el máximo de memoria privada, de 633 MiB (589 MiB de memoria de trabajo), frente a los 2.048 MiB de RNF-05. El `Player.log` no registró ninguna excepción (`evidencias/rc5/medicion-colegio/`). Esta medición respalda los PD de `PF-RNF04-01`, `PF-RNF05-01`, `PF-RNF07-02` y `PF-CT02-01`. No cubre los Niveles 2 y 3 ni los demás equipos, el medidor no comprueba el SHA-256 del ejecutable y la huella de esa carpeta no se tomó ese día.

Las otras mediciones sobre rc5 dieron lo siguiente (cada caso está en el Anexo A):

- RNF-10, sin conexiones: 0 conexiones TCP ni UDP del proceso en 12 536 muestras de 31 archivos, y 0 apariciones de `unity3d.com` en `globalgamemanagers`. `PF-RNF08-01` queda en PD: los 30 equipos del 16/10/2026 no tenían conexión a internet y funcionaron sin complicación, según el registro de la sesión, sin el modo avión del guion.
- RNF-07 y RNF-11, portabilidad y residuos: una copia idéntica de los 253 archivos en una ruta con espacio arranca con un proceso no elevado, crea `Datos/` junto al `.exe` y llega a `Level1_Cave`. Tras borrar los perfiles de prueba no queda su rastro en la carpeta, en `LocalLow`, en el registro ni en `%TEMP%`. Fuera de `Datos/` quedan `Player.log` y 9 valores de sesión de Unity en `HKCU`: el residuo del motor ya registrado como DEF-RC2-01 (decisión D-RC2-01).
- RNF-16, exclusión de un nivel: en un worktree de `ca6fd91` sin el Nivel 2 la suite da EditMode 700 de 706 y PlayMode 306 de 335; sin el Nivel 1, 730 de 736 y 340 de 370. Aprueba con desviación (`PD`, DEF-RC5-20).
- RNF-18, parámetros sin recompilar: con el Editor abierto y en Play se cambiaron `MinimumEffectiveStrikes` y una línea de `N1_Hallazgo`, con solo dos `.asset` modificados y sin errores de compilación. Aprueba como variante, porque el Play se condujo con scripts del Editor y no con el ratón.

## 3.6 Defectos de la primera pasada (rc3)

Se registraron 16 defectos en rc3: 0 Bloqueantes, 0 Mayores, 7 Menores y 9 Observaciones. Observación es lo que se anota sin que incumpla un RF ni un criterio de aceptación. `I03` e `I04` salen de verificaciones de apoyo (`INSP-ART-SPRITES` y `SUITE-T05`) y no de un caso `PF`. Todos quedaron abiertos al cierre de esa pasada; el triaje del 13/10/2026 se recoge más abajo, DEF-RC3-I04 se cerró el 16/10/2026 (apartado 3.7) y el estado de cada uno sobre rc5 está en el apartado 3.8.

**Tabla 3.6.** Defectos hallados en rc3, con su severidad, el tipo de arreglo propuesto y la decisión del triaje del 13/10/2026.

| Defecto | Caso | Severidad | Qué ocurre | Tipo propuesto | Triaje del 13/10/2026 |
|---|---|---|---|---|---|
| DEF-RC3-01 | `PF-RF45-01` | Observación | El avatar de Algoritm en el resumen de nivel es un círculo con el texto «Algoritm», pese al diseño final del guía | Corregido en el código el 14/10/2026 (`86a9898`, posterior a rc4) | Trabajo futuro en el triaje del 13/10/2026; corregido después, entra en rc5 |
| DEF-RC3-02 | `PF-RF01-03` | Observación | `casos.md` describe la portada de otra manera: el panel ámbar queda a la izquierda y el bosque con los cinco personajes a la derecha; el criterio se cumple | Documento | Documento: `casos.md` describe la portada como es |
| DEF-RC3-03 | `PF-RF05-05` | Observación | El criterio «ningún personaje con la cara tapada» no contempla el gesto del Niño en reposo (la mano a la sien tapa parte de un ojo), registrado como excepción en el generador de poses | Documento o arte | Trabajo futuro en el triaje; el 16/10/2026 se aceptó como diseño y `casos.md` lo contempla |
| DEF-RC3-04 | `PF-RF14-01` | Observación | Las siete piezas del Nivel 1 caen en sitios distintos en cada entrada; las coordenadas del caso no valen | Documento | Documento: `casos.md` sin coordenadas fijas de las piezas |
| DEF-RC3-05 | `PF-RNF03-01` (visto en `PF-RF46-01`) | Menor | Informe docente: los iconos de «Errores corregidos» y «Tiempo de resolución» cruzan la primera línea de su cabecera, los de «Intentos» y «Pasos utilizados» quedan sueltos y pequeños, y «Volver al menú» parte su texto en dos líneas | Código (disposición) o arte | Código: se corrige en rc4 |
| DEF-RC3-06 | `PF-RF44-01` | Observación | El paso 21 de S-N3 no se puede reproducir como está escrito: la balsa arranca con la línea 4 y la última línea llega cuando los cuatro ya bajaron al pasto | Documento | Documento: el paso 21 de S-N3 como se reproduce |
| DEF-RC3-07 | `PF-RF30-02` | Observación | Los colores medidos del laberinto difieren en un nivel de 8 bits de los del documento (`#6A5A45` frente a `#6A5A44`, `#A0320D` frente a `#A0330D`) | Documento (admitir ±1) o ninguno | Documento: se admite ±1 en 8 bits |
| DEF-RC3-08 | `PF-RF05-04` | Observación | Algoritm aparece con la forma del nivel anterior en las secuencias puente (llama en `N2_PuenteI`, disco en `N3_PuenteII`) | Decisión pendiente o documento | Trabajo futuro en el triaje; el 16/10/2026 se confirmó que es intencional y `casos.md` lo describe |
| DEF-RC3-09 | `PF-RF45-05` | Menor | Un doble clic en «Omitir» del cierre del Nivel 1 deja en el menú de niveles sin ver el resumen (2 de 3 intentos); el progreso no se pierde | Código (ignorar la entrada unos 300 ms tras cargar el resumen) o ajustar el caso | Código: se corrige en rc4 |
| DEF-RC3-10 | `PF-RNF03-01` | Menor | Bosque del Nivel 2: la instrucción de tres líneas se sale de su tablilla de dos | Código o escena (altura de la tablilla), o acortar la línea de `N2_Guia.asset` | Código: se corrige en rc4 |
| DEF-RC3-11 | `PF-RF47-02` | Observación | «Eliminar datos» se parte en dos líneas dentro de su botón; la ruta de respaldo del diálogo de salida queda a unos 8 px de los botones | Disposición | Código: se corrige en rc4 |
| DEF-RC3-12 | `PF-RNF16-01` | Observación | Cinco pruebas de EditMode fallan al retirar un nivel, en los dos sentidos, y el caso no las lista | Documento (listarlas como esperadas) o prueba (tolerar un nivel ausente) | Documento: las cinco pruebas se listan como esperadas |
| DEF-RC3-I01 | `PF-RNF15-01` | Menor | Identificadores en español en `ForestSceneController` (20), `WorkshopSceneController` y `MazeSceneController` | Estilo de código | Trabajo futuro: estilo de código |
| DEF-RC3-I02 | `PF-RNF17-01` | Menor | 9 de 122 commits que tocan `Assets/` llevan tarjeta del Kanban (7,4 %) | Proceso; no se corrige sin reescribir la historia | Trabajo futuro: proceso; no se corrige sin reescribir la historia |
| DEF-RC3-I03 | `INSP-ART-SPRITES` (apoyo) | Menor | Halo de croma en 9 PNG de caras de la familia (15 a 51 píxeles con alfa de 10 a 25 sobre 255); rc2 tenía 0 | Arte | Trabajo futuro: arte |
| DEF-RC3-I04 | `SUITE-T05` (apoyo) | Menor | La suite no corrió sobre el contenido exacto de rc3 (HEAD `292b108` y no `f49ccc9`) | Proceso | Cerrado el 16/10/2026: suite completa sobre el contenido de rc5 |

Los cuatro F de la pasada salen de cinco de los defectos Menores: `PF-RF45-05` de DEF-RC3-09, `PF-RNF03-01` de DEF-RC3-10 y DEF-RC3-05, `PF-RNF15-01` de DEF-RC3-I01 y `PF-RNF17-01` de DEF-RC3-I02. DEF-RC3-05 se vio primero de paso en S-RNF y se confirmó en S-DOC, donde `PF-RF46-01` aprobó porque las cifras y los cuatro indicadores son correctos. El caso de DEF-RC3-09, `PF-RF45-05`, había aprobado en rc2 (01/10/2026) con un doble clic de 95 ms sobre «Omitir»; en rc3 falla con 100 ms según la coordenada del clic, y la causa probable es la geometría del botón «Volver al menú de niveles» del resumen, que carga en 13 ms y queda bajo ese segundo clic (se infiere de las capturas, sin leer el código).

**Triaje del 13/10/2026.** Los 16 defectos se repartieron en tres destinos. Ninguno estaba corregido en rc3, y esta decisión no cambió ningún veredicto de esa pasada.

- **Se corrigen en el documento de casos (5):** DEF-RC3-02, -04, -06, -07 y -12. En `casos.md`, `PF-RF01-03` describe la portada como es (panel ámbar a la izquierda con el título, el lema y los botones, y el bosque con los cinco personajes a la derecha); `PF-RF14-01` no promete coordenadas de las siete piezas del Nivel 1, que caen en sitios distintos en cada entrada, y pide leer la pantalla antes de arrastrar; el paso 21 de S-N3 (`PF-RF44-01`) pide los clics hasta la línea 4, que es cuando arranca la balsa, y fotografiar entonces; `PF-RF30-02` admite ±1 en cada canal de 8 bits (`#6A5A45` frente a `#6A5A44`, `#A0320D` frente a `#A0330D`); y `PF-RNF16-01` lista como esperadas las cinco pruebas que fallan al retirar un nivel en los dos sentidos: `Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas`, `Architecture_RNF15_ElNombreDelAssemblyCoincideConSuRutaBajoScripts`, `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel`, `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` y `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra`.
- **Se corrigen en el código, en rc4 (4):** DEF-RC3-05, -09, -10 y -11. Quedaron corregidas en el commit `0405eba` (13/10/2026) y entraron en rc4: el informe docente ya no monta las cabeceras con sus iconos (DEF-RC3-05); `InputGuard` ignora los clics durante 0,3 s tras cada carga de escena (DEF-RC3-09); la tablilla del bosque es más ancha y caben las tres líneas (DEF-RC3-10); y los textos de los botones del informe docente y de los diálogos de borrar y salir ya no se parten (DEF-RC3-11). Pruebas nuevas las vigilan (`InputGuard`, `TeacherReport`, `EraseDialog`, `MainMenu` y `ForestScene`; `evidencias/build-rc4.md`). rc4 no tuvo pasada de casos; los casos afectados se repitieron sobre rc5 (apartado 3.8).
- **Pasaron a trabajo futuro (7):** DEF-RC3-01, el círculo con el texto «Algoritm» en el resumen de nivel, que después se corrigió en el código (párrafo siguiente); DEF-RC3-03, porque el gesto del Niño en reposo, con la mano a la sien, está registrado como excepción en el generador de poses y el criterio de `PF-RF05-05` no lo contempla; DEF-RC3-08, porque Algoritm aparece con la forma del nivel anterior en las secuencias puente y faltaba confirmar si era intencional. El 16/10/2026 el equipo cerró estos dos como decisión de diseño: el gesto del Niño se acepta y la forma del guía en los puentes es intencional, y los criterios de `PF-RF05-04` y `PF-RF05-05` en `casos.md` ya lo describen. Siguen en trabajo futuro DEF-RC3-I01, por los identificadores en español de tres controladores del Nivel 2, que es estilo de código; DEF-RC3-I02, porque solo 9 de 122 commits llevan tarjeta y corregirlo exigiría reescribir la historia; y DEF-RC3-I03, por el halo de croma de nueve caras de la familia, que es arte. DEF-RC3-I04, porque la suite no corrió sobre el contenido exacto de rc3, se cerró el 16/10/2026 con una suite completa sobre el contenido de rc5 (apartado 3.7).

El 14/10/2026, después del triaje, DEF-RC3-01 se corrigió en el código (commit `86a9898`): el avatar del resumen de nivel es ahora el guía con su diseño final, en la forma del nivel terminado (llama, disco de madera o gota). La corrección es posterior a rc4, que se compiló desde `0405eba`: el ejecutable rc4 todavía muestra el círculo, y la corrección entra en rc5. Las pruebas `LevelSummary_DEFRC301_*` pasan (3 en EditMode y 3 en PlayMode), y `PF-RF45-01` aprobó sobre rc5 con la corrección.

La captura del resumen del Nivel 3 sobre el ejecutable rc4 mostró además que la tarjeta medía más de 1080 px de alto a 1920 × 1080 y se cortaban sus bordes superior e inferior (DEF-RC4-01, Observación, `PF-RF45-01`). Se corrigió el 14/10/2026 en `ca6fd91`: el espaciado vertical entre bloques bajó de 48 a 32 px y el texto de la habilidad se ensanchó de 710 a 900 px, de modo que la del Nivel 3 ocupa dos líneas. En el Editor, a 1920 × 1080, la tarjeta mide ahora de 927 a 989 px en los tres niveles (capturas en `claudeDocs/tasks/OE4/evidencias/rc5/`). rc5 incluye la corrección, y `PF-RF45-01`, `-02` y `-03` la comprobaron sobre el ejecutable en la pasada final.

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

La suite del candidato rc5 (16/10/2026, `suite2.ps1`, 25,5 min) corrió sobre `06a5b61`, cuyo `Assets/`, `Packages/` y `ProjectSettings/` coinciden con `ca6fd91`, el commit del que se compiló rc5; solo difiere `Assets/Game/Art/Inventario.md`, un índice en texto que no entra al ejecutable. EditMode dio 796 pruebas, 795 aprobadas y la omitida por entorno de siempre, y PlayMode 438 de 438, con 1.234 pruebas listadas y las 1.234 ejecutadas, repetidas 0 y ninguna fallida (`evidencias/suites/rc5-06a5b61/resumen.md`). Es la primera suite sobre el contenido exacto de un candidato que se probó con estudiantes, y con ella se cierra DEF-RC3-I04.

Cada uno de los 47 RF tiene al menos una prueba automática que lo nombra (el mínimo es 1, en RF-21 y RF-27). Ninguna prueba nombra RNF-08, RNF-12, RNF-17 ni RNF-22, que se verifican por inspección o por documento. El recuento de pruebas por requerimiento está en el apartado 8 de `OE4-Resultados.md`.

## 3.8 Defectos de la pasada final (rc5)

La pasada sobre rc5 registró 20 defectos: 0 Bloqueantes, 0 Mayores, 5 Menores y 15 Observaciones (triaje de cierre del 05/11/2026, en `evidencias/rc5/defectos.md`). Ninguno exige recompilar: se cierra sobre rc5 sin compilar otra versión, así que ninguno se corrigió en el juego.

**Tabla 3.8.** Defectos hallados en rc5, con su severidad y su disposición en el triaje de cierre.

| Defecto | Caso | Severidad | Qué ocurre | Disposición |
|---|---|---|---|---|
| DEF-RC5-01 | `PF-RF05-07`, `-08`, `PF-RF21-03` | Observación | `casos.md` numera las líneas del Nivel 1 desde 0 y «Atrás» desde 1 | Catálogo corregido |
| DEF-RC5-02 | `PF-RF05-09`, `PF-RF11-01` | Observación | Dos descripciones de pose del catálogo no coinciden literalmente con lo que se ve | Catálogo corregido, salvo «Papá golpea y levanta el puño» |
| DEF-RC5-03 | `PF-RF20-01`, `-04`, `PF-RNF21-01` | Observación | La llama que nace se ve recortada por un arco dentado durante unos 2,5 s | Conocido en rc5 |
| DEF-RC5-04 | `PF-RF31-01` (PD), `PF-RF30-01` | Observación | «Retroceder» termina en círculo y el diseño pide rombo; el catálogo dice que el refugio no se dibuja y se dibuja una cueva | Decidido: se conserva lo implementado (`INC-161`) |
| DEF-RC5-05 | `PF-RF31-01`, `PF-RF34-01` | Observación | En una secuencia larga solo se ven la cuenta y el lado del bloque seleccionado | Conocido en rc5 |
| DEF-RC5-06 | `PF-RF44-01` | Observación | La balsa apenas se desplaza en el cruce y el paso del catálogo no se puede fotografiar; si eso basta para RF-44 no se decidió | Conocido en rc5 |
| DEF-RC5-07 | Fuera del guion de S-N3 | Menor | «La pieza vuelve a tus materiales.» no se muestra nunca y «Puesto…» sigue en pantalla al retirar una pieza | Conocido en rc5 |
| DEF-RC5-08 | `PF-RF07-01` | Observación | Con la pausa abierta, un tronco del bosque se agranda bajo el cursor | Conocido en rc5 |
| DEF-RC5-09 | `PF-RF07-01`, `-02` | Observación | La flecha sostenida al pausar no se puede reproducir con un solo puntero | No reproducible con el arnés |
| DEF-RC5-10 | `PF-RF21-02` (F) | Menor | La luz de la cueva llega a su máximo con el cuarto golpe efectivo, antes de soplar | Conocido en rc5 |
| DEF-RC5-11 | `PF-RNF14-01`, `-04`, `-05` | Observación | Al retomar el Nivel 2 se vuelve a ver la apertura del bosque y no la narrativa de la fase pendiente | Conocido en rc5 |
| DEF-RC5-12 | `PF-RF02-05` | Observación | El juego acepta el nombre reservado «CON» y crea `Datos\CON.json` | Conocido en rc5 |
| DEF-RC5-13 | `PF-RF47-03`, `PF-RF09-04` | Observación | La guarda del manifiesto rechazaba el archivo `Datos` que pide el paso 11 de S-DOC | Cerrado: corregido en el arnés y repetido el paso 11 el 05/11/2026 |
| DEF-RC5-14 | `PF-RNF13-01` | Observación | El resumen del Nivel 3 puede unir «te lo dijo al hundirse» y «No hizo falta rehacer nada» | Conocido en rc5 |
| DEF-RC5-15 | `PF-RNF20-01` (F) | Menor | «Arrastra un bloque a tu secuencia» tiene un contraste de 3,15:1; ya estaba en rc3 | Conocido en rc5 |
| DEF-RC5-16 | `PF-RNF15-01` (F) | Menor | 23 identificadores en español en 3 de 10 clases; continúa DEF-RC3-I01 | Conocido en rc5 |
| DEF-RC5-17 | `PF-RNF17-01` (F) | Observación | 9 de 92 commits de código con tarjeta; continúa DEF-RC3-I02 | No se corrige hacia atrás |
| DEF-RC5-18 | `PF-RNF02-01` | Menor | El catálogo pedía teclas en las cinco escenas y traía el paso en tres | Cerrado: `casos.md` trae el paso y la sesión S-TECLAS probó el taller y el laberinto |
| DEF-RC5-19 | `PF-RNF19-01` | Observación | La alerta del espacio marcado del amarre apenas se distingue en grises | Conocido en rc5 |
| DEF-RC5-20 | `PF-RNF16-01` (PD) | Observación | 17 pruebas de PlayMode fallan en modo por lotes con o sin retirar un nivel, por la Game View de 640 × 480 | Decidido: entorno, no acoplamiento (`INC-162`) |

De los defectos de rc3, la pasada sobre rc5 comprobó corregidos DEF-RC3-01, -05, -09, -10 y -11 y DEF-RC4-01. Continúan DEF-RC3-I01 y DEF-RC3-I02, como DEF-RC5-16 y DEF-RC5-17. DEF-RC3-I03 sigue abierto: los PNG de esas caras no cambiaron entre rc3 y rc5, y la pasada no lo midió de nuevo. DEF-RC2-01 persiste. Al cierre de la pasada quedan abiertos, con severidad Menor, DEF-RC2-01, DEF-RC3-I03 y DEF-RC5-07, -10, -15 y -16. DEF-RC5-17 continúa uno que en rc3 fue Menor y el triaje de rc5 lo deja como Observación.

## 3.9 Comparación entre las dos pasadas

El universo creció de 136 a {{N_CASOS}} casos por los nueve nuevos, y el denominador de la eficacia pasó de 136 a 142 porque los 3 de sonido no se ejecutaron. La Tabla 3.9 reúne lo que cambió.

**Tabla 3.9.** Casos cuyo resultado cambió entre rc3 y rc5.

| rc3 → rc5 | Casos | Motivo |
|---|---|---|
| F → P (2) | `PF-RF45-05`, `PF-RNF03-01` | Corregidos en `0405eba`: DEF-RC3-09, y DEF-RC3-05 y -10 |
| P → F (2) | `PF-RF21-02`, `PF-RNF20-01` | No son regresiones: el problema ya estaba en rc3 y esa pasada no lo midió. rc3 no midió el máximo de luz tras el cuarto golpe (DEF-RC5-10) y su muestreo de contraste no incluyó esa etiqueta (DEF-RC5-15). El P de rc3 de los dos está dentro de su 132 de 136 |
| P → PD (8) | `PF-RF31-01`, `PF-RNF16-01`, `PF-RNF04-01`, `PF-RNF05-01`, `PF-RNF07-02`, `PF-CT02-01`, `PF-RNF08-01`, `PF-RNF13-01` | Los dos primeros, mismo dibujo y misma conducta que en rc3 con la desviación ya registrada (`INC-161`, `INC-162`); los seis restantes, respaldados por la sesión y la medición del 16/10/2026 (`INC-163`) |
| P → NE (3) | `PF-SON-01`, `PF-SON-02`, `PF-SON-03` | No se ejecutaron sobre rc5 y el resultado de rc3 no se hereda |
| Sin caso → P (9) | `PF-RF05-07`, `-08`, `-09`, `PF-RF07-07`, `-08`, `PF-RF13-06`, `PF-RF21-03`, `PF-RF23-02`, `PF-RF31-02` | Casos nuevos (apartado 2.1) |

Siguen en F `PF-RNF15-01` y `PF-RNF17-01`. Los P pasan de 132 a 130: 132 + 2 − 2 − 8 − 3 + 9.
