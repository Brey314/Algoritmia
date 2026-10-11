# Defectos de la pasada rc3 (S-INI, S-N1, S-RNF)

rc3, HEAD `f49ccc9`, 10/10/2026. Ninguno impide terminar la ruta ni incumple un RF: son una incidencia menor y observaciones. Los de S-INSP están en `S-INSP/defectos.md` (ids `DEF-RC3-Ixx`).

### DEF-RC3-01 · El avatar de Algoritm en el resumen de nivel es un círculo con el texto «Algoritm»
- Severidad: Observación · RF-12, RF-17, RF-45 (resumen de fin de nivel) · Sesión S-N1 paso 23.
- Pasos: terminar el Nivel 1 → pantalla «Esto es lo que pasó en la cueva».
- Esperado / observado: Interfaces.md §13 documenta el «rótulo «Algoritm» del resumen», pero el juego ya tiene el diseño final de Algoritm y la pantalla sigue con un círculo naranja con el nombre escrito; parece un rótulo de trabajo.
- Evidencia: `rc3/S-N1/PF-RF45-01_23.png`. Tipo propuesto: decisión de Santiago (documento o arte).

### DEF-RC3-02 · casos.md describe la portada de otra manera
- Severidad: Observación · RF-01 · S-INI paso 2 (PF-RF01-03).
- Observado: panel ámbar a la izquierda con título, lema y botones, y el bosque con los cinco personajes enmarcado a la derecha; casos.md dice «el título queda sobre una tarjeta más baja y detrás se ve el bosque». El criterio de la tabla (cinco personajes enteros, de frente, sin tapar botones) se cumple.
- Evidencia: `rc3/S-INI/PF-RF01-01_1.png`, `PF-RF01-03_2.png`. Tipo propuesto: documento (casos.md).

### DEF-RC3-03 · El criterio de PF-RF05-05 no contempla el gesto del Niño
- Severidad: Observación · RF-05 · S-N1 (cueva) y portada.
- Observado: en reposo el Niño levanta la mano a la sien y el antebrazo cruza parte de la cara (un ojo parcialmente tapado, el otro a la vista). Está documentado como excepción `("nino","Idle")` en `claudeDocs/tasks/Personajes/herramientas/coreografia.py` (`CARA_TAPADA`), pero PF-RF05-05 dice «ningún personaje con la cara tapada por un brazo».
- Evidencia: `rc3/S-N1/PF-RF14-01_4.png` (esquina derecha, el Niño). Tipo propuesto: documento (casos.md) o arte, lo decide Santiago.

### DEF-RC3-04 · Las piezas del Nivel 1 se reparten al azar en cada entrada
- Severidad: Observación · RF-14 · S-N1 pasos 7–8.
- Observado: en cuatro entradas a `Level1_Cave` que se miraron (la de S-N1, la de S-RNF2, su reinicio y la repetición) las siete piezas cayeron en sitios distintos; las coordenadas de casos.md («(0,20; 0,50)», destinos «≤ 0,04 del centro») solo sirven si se lee antes la pantalla. Comportamiento del juego, no un fallo; se anota para que casos.md no prometa posiciones fijas.
- Evidencia: `rc3/S-N1/PF-RF14-01_4.png`, `rc3/S-RNF/rnf2_cave.png`, `rnf2_cave2.png`, `rnf3_cave.png`. Tipo propuesto: documento (casos.md).

### DEF-RC3-05 · Informe docente: los iconos de las cabeceras se montan sobre el texto
- Severidad: Menor · RF-46 (informe docente) · visto de paso en S-RNF; el caso es de S-DOC.
- Pasos: menú de inicio → «Progreso del equipo».
- Observado: sobre «Errores corregidos» y «Tiempo de resolución» el icono (herradura y reloj de arena) cruza el texto de la cabecera; los iconos de «Intentos» y «Pasos utilizados» quedan sueltos por encima y son muy pequeños. El botón «Volver al menú» parte el texto en dos líneas.
- Evidencia: `rc3/S-RNF/rnf_teacher.png`. Tipo propuesto: código (disposición) o arte; se confirma en S-DOC.

## No son defectos del juego (arnés y entorno)
- `oe4.ps1 Restaurar-Registro` sale con 2 al reponer un valor cambiado («The type of the value object did not match the specified RegistryValueKind», línea 1502); no repone los valores que cambiaron, solo borra los nuevos.
- Un aviso de NVIDIA Superposición cae unos segundos sobre la esquina superior derecha tras el arranque (`rc3/S-INI/PF-RF01-01_1.png`, `rc3/S-N1/PF-RF06-01_1a.png`).

## Pasada S-N2A, S-N2B, S-N2C y S-N3 (10/10/2026)

Ninguno impide terminar la ruta ni incumple un RF: tres observaciones. Todos los casos de las cuatro sesiones salieron P.

### DEF-RC3-06 · El paso 21 de S-N3 no se puede reproducir como está escrito
- Severidad: Observación · RF-44 · Sesión S-N3 paso 21 (PF-RF44-01).
- Pasos: llegar a `N3_Escena33_Cruce` y pulsar «Continuar» seis veces, una por segundo, desde que aparece la escena.
- Esperado (casos.md): en la foto tomada a mitad del río los cuatro siguen sobre la balsa y el cuadro muestra la última línea, «Eso es pensar computacionalmente.»; a los ~12 s la balsa llegó y los cuatro bajaron caminando al pasto.
- Observado: la balsa no se mueve mientras la escena espera en la línea 1 ni en la 3 (19 s sin clic y 14 s tras dos clics: familia a bordo, sin movimiento). El cruce arranca con la línea 4 («Llegan a la orilla opuesta…»). Con un clic por segundo la última línea aparece a ~6,4 s, cuando los cuatro ya bajaron caminando al pasto (el Niño pisa la orilla ~1 s después de la línea 4), así que nunca coinciden «última línea» y «a mitad del río». Con tres clics y espera, el desembarco tarda ~2,5 s tras la línea 4. Nadie salta, nadie cae al agua y todos bajan caminando: lo que exige PF-RF44-01 se cumple.
- Evidencia: `rc3/S-N3/PF-RF44-01_21_seis_clics_montaje.png`, `_detalle_desembarco.png`, `_tres_clics_montaje.png`, `_sin_clics_montaje.png`, `_dos_clics_montaje.png`. Tipo propuesto: documento (casos.md): pedir los clics hasta la línea 4 y fotografiar entonces.

### DEF-RC3-07 · Los colores medidos del laberinto difieren en un nivel de 8 bits de los del documento
- Severidad: Observación · RF-30 · S-N2C paso 1 (PF-RF30-02).
- Observado: el marco del entorno y el de «Tu secuencia» salen `#6A5A45` en la foto y el documento dice `#6A5A44`; el aro de la pista sale `#A0320D` y el documento dice `#A0330D`. El resto coincide exacto: fondo `#C4A882`, zona de soltar `#E0D4C0`. Es redondeo de color (conversión de espacio de color) y no se ve; se anota para que quien compare píxeles no lo dé por defecto.
- Evidencia: `rc3/S-N2C/PF-RF30-02_1_1.png`. Tipo propuesto: documento (admitir ±1) o ninguno.

### DEF-RC3-08 · ALGORITM aparece con la forma del nivel anterior en las secuencias puente
- Severidad: Observación · RF-05 (INC-45, INC-136) · S-N2A paso 1 y S-N3 paso 1 (PF-RF05-04).
- Observado: en `N2_PuenteI` (cueva, al amanecer) el guía es la llama y en `N3_PuenteII` y `N3_PuenteII_Horizonte` es el disco de madera; la gota con pantaloneta empieza en `N3_PuenteII_Rio`. El criterio de PF-RF05-04 («Algoritm, con brazos y como antes») se cumple; casos.md no dice qué forma lleva cada puente.
- Evidencia: `rc3/S-N2A/PF-RF05-04_1_hoja_PuenteI.png`, `rc3/S-N3/PF-RF05-04_1_hoja_N3_a.png` y `_b.png`. Tipo propuesto: decisión de Santiago (confirmar que es intencional) o documento.

## No son defectos del juego (arnés y entorno), pasada S-N2/S-N3
- `oe4.ps1 Restaurar-Registro` volvió a salir con 2 en las sesiones S-N3c y S-N2R (mismo fallo del arnés que arriba); el aviso de NVIDIA no estorbó las capturas de esta pasada.
- Las coordenadas de casos.md del N3 (materiales, ensamblaje) sí valen: el plano es fijo entre entradas, a diferencia del N1 (DEF-RC3-04). El tablero del laberinto sí cambia en cada entrada (dos tableros distintos vistos).

## Pasada S-PAU, S-OMI, S-PER y S-DOC (10/10/2026)

Ninguno es Bloqueante ni Mayor. S-PAU y S-PER salieron P sin defectos; el único F es PF-RF45-05.

### DEF-RC3-05 · Informe docente: los iconos de las cabeceras se montan sobre el texto — CONFIRMADO
- Severidad definitiva: **Menor** · RF-46 (Alta), RNF-03 · S-DOC paso 1 (PF-RF46-01).
- Pasos: menú de inicio → «Progreso del equipo» (cualquier perfil).
- Observado en rc3 (1920×1080): los iconos de «Errores corregidos» (herradura) y «Tiempo de resolución» (reloj de arena) cruzan la primera línea de la cabecera; los de «Intentos» y «Pasos utilizados» quedan por encima, sueltos y muy pequeños. El texto y las cifras siguen legibles y los cuatro indicadores y sus valores son correctos, así que el RF se cumple (criterio secundario de aspecto). El botón «Volver al menú» parte su texto en dos líneas.
- Evidencia: `rc3/S-DOC/PF-RF46-01_1.png`, `rc3/S-DOC/PF-RF46-01_1_cabeceras-x2.png`. Tipo propuesto: código (disposición de la cabecera) o arte; lo decide Santiago.

### DEF-RC3-09 · Un doble clic en «Omitir» del cierre deja en el menú de niveles sin ver el resumen
- Severidad: **Menor** · RF-45 (Alta, resumen al terminar el nivel), RF-12 · S-OMI paso 5 (PF-RF45-05; PF-RF06-03 pasa con otra coordenada).
- Pasos exactos: `Preparar-Datos OE4_Z` → «Jugar» → OE4_Z → «Nivel 1» → «Omitir» ×3 hasta `Level1_Cave` → reunir las 7 piezas → fuerza 7, cercanía 5 → «Golpear» ×3 → «Soplar» → en `N1_NacimientoDelFuego` doble clic rápido (100 ms) en el centro de «Omitir» (0,65; 0,92 del área cliente, 1920×1080).
- Esperado: `LevelSummary` («Esto es lo que pasó en la cueva»). Observado: aparece el menú de niveles; el resumen se ve ~0,1 s o nada. Con (0,65; 0,905) igual; con (0,65; 0,93) se queda el resumen (dos de tres intentos fallan).
- Causa probable (por geometría, sin leer el código): `LevelSummary` carga en 13 ms y su botón «Volver al menú de niveles» (x 0,40–0,65; y 0,84–0,92 con tres viñetas de dos líneas) queda justo bajo el segundo clic, que lo pulsa. No es la regresión de `NarrativeSceneController.Leave()` que el paso vigila. El progreso no se pierde: la fase ya está guardada y el N2 queda abierto.
- Evidencia: `rc3/S-OMI/PF-RF45-05_5_def09-y092.png`, `…y0905.png`, `rc3/S-OMI/PF-RF06-03_5_y093.png`. Tipo propuesto: código (ignorar la entrada unos ~300 ms tras cargar el resumen) o aceptar y ajustar el caso.

### DEF-RC3-10 · Bosque del N2: la instrucción de tres líneas se sale de su tablilla
- Severidad: **Menor** · RNF-03 («ningún texto… queda… bajo otro elemento»), RF-22 · visto en S-RNF (control de teclas del bosque); la misma imagen está en `rc3/S-N2A/PF-RF22-01_2_1.png` (de S-N2A, marcado P).
- Pasos: «Nivel 2» → tres narrativas (o «Omitir» ×3) → `Level2_Forest`, primera pantalla.
- Observado: «Selecciona los objetos que se muevan con facilidad y únelos. Cuando tengas cinco, veremos qué pasa.» ocupa tres líneas en una tablilla de dos: la primera línea toca el borde superior y «qué pasa.» cuelga sobre el borde inferior. El texto se lee, pero queda fuera de la tarjeta. Las tablillas del taller, el laberinto y el río caben.
- Evidencia: `rc3/S-RNF/PF-RNF03-01_bosque_tablilla.png`. Tipo propuesto: código/escena (altura de la tablilla) o acortar la línea de `N2_Guia.asset`; lo decide Santiago.

### DEF-RC3-11 · Textos de botones partidos en dos líneas en el informe y su diálogo
- Severidad: **Observación** · RF-47 · S-DOC pasos 1 y 8.
- Observado: «Eliminar datos» en el diálogo de confirmación del informe se parte en dos líneas dentro de su botón («Eliminar / datos»); en el diálogo de «Salir» con `Datos/` no escribible la ruta de respaldo queda a ~8 px de los botones (no se monta). Con cuatro perfiles, «¿Quién juega?» muestra tres y ofrece ▼ (previsto).
- Evidencia: `rc3/S-DOC/PF-RF47-02_8.png`, `rc3/S-DOC/PF-RF09-04_11_salir.png`.

### DEF-RC3-12 · Cinco pruebas de EditMode fallan al retirar un nivel y PF-RNF16-01 no las lista
- Severidad: **Observación** · RNF-16 · casos con Unity (S-RNF, `veredictos-exe.csv`, `S-RNF/rnf16/resumen.md`).
- Pasos: copia sin `Levels/Wheel` (o sin `Levels/Fire`), sus pruebas y `Tests/EditMode/Content`; `unity test` EditMode.
- Observado: compila y el resto pasa (657/663 sin N2; 690/696 sin N1), pero fallan en los dos sentidos `Architecture_RNF15_CadaModuloDeclaraSuAssemblyDeRuntimeYDePruebas`, `Architecture_RNF15_ElNombreDelAssemblyCoincideConSuRutaBajoScripts`, `Architecture_RNF16_NingunAssemblyDeNivelReferenciaAOtroNivel`, `Architecture_RNF16_RetirarUnNivelNoAfectaALosOtrosDos` y `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra`: exigen los tres assemblies y todos los RF, no detectan acoplamiento. El caso solo admite fallos de pruebas que cargan una escena por nombre.
- Tipo propuesto: documento (casos.md: listarlas como esperadas) o prueba (tolerar un nivel ausente). Además, `unity test` en lote falla 15 pruebas de River, Wheel y UI (RNF-03/19/20/21) que pasan con la suite normal a 1920×1080 (`editor.ps1`); NORMA-PRUEBAS.md ya manda esa vía.

## No son defectos del juego (arnés y entorno), pasada S-PAU/S-OMI/S-PER/S-DOC
- `oe4.ps1 Restaurar-Registro` sigue saliendo con 2 (mismo fallo del arnés).
- Las coordenadas de casos.md del N1 dependen del reparto al azar de las piezas (DEF-RC3-04); las de las demás escenas valen.
- Las dos semillas con perfiles de distinto progreso se ordenan en la lista de «¿Quién juega?» por nombre o por fecha según el momento; hay que mirar la foto antes de elegir (con `OE4_Z` y `OE4_D` a la vez, el primero fue OE4_D).
