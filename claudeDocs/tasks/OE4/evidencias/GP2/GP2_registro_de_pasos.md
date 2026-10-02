# GP#2 — Golden Path a pantalla completa, «como se entrega» (PF-RNF13-01, segunda vez) — 01/10/2026

- **Recorrido:** Golden Path #2 con perfil nuevo `OE4GP2`, de la pantalla de inicio a los créditos (N1, N2, N3), **pantalla completa** en el monitor principal (`Start-Juego GP2 -Completa`), un solo proceso y un solo agente.
- **Exe:** `C:\Dev\Algoritmia\Build\Algoritmia\Algoritmia.exe` (rc1, 479,0 MB, SHA-256 `6CD16EDF…ADAB`, coincide). Editor cerrado (solo Unity Hub abierto: no cuenta como contaminación).
- **Datos:** `Preparar-Datos` sin perfiles a las 10:41:37 (el `OE4GP1.json` que quedaba era idéntico a `evidencias/GP1/GP1_OE4GP1_final.json`).
- **Fotos:** solo en los cambios de escena y donde hizo falta ubicar algo; sin pruebas de error deliberadas salvo las que el juego exige.
- **Evidencias:** `claudeDocs/tasks/OE4/evidencias/GP2/`.

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 10:41:37 | `Preparar-Datos` (sin perfiles) | `Datos/` vacía | OK |
| 2 | 10:41:43,96 | `Start-Juego GP2 -Completa -Sha256 …` | SHA coincide; pantalla completa, cliente 1920×1080 en (0,0), DPI 125 %; pid 44004, muestreador 18136; sin aviso de contaminación | OK |
| 3 | 10:41:46 | `Esperar-Carga MainMenu` | 0,086 s | OK (RNF-04) |
| 4 | 10:41:57 | Foto gp2b_001 | Inicio: título, lema, «Jugar», «Créditos», «Salir», «Progreso del equipo», Algoritm de fuego; arriba a la derecha el aviso de NVIDIA (OBS-1 de GP1, ajeno al juego) | OK |
| 5 | 10:42:19 | «Jugar» (foto a +0,8 s) | «¿Quién juega?»: «Perfiles guardados» vacía, «Perfil nuevo», campo «Tu nombre», «Volver», «Continuar» (gp2b_002) | OK |
| 6 | 10:42:28 | Clic en el campo + `Escribir OE4GP2` | El campo muestra «OE4GP2» (gp2b_003) | OK |
| 7 | 10:42:39 | «Continuar» + `Esperar-Carga Narrative` | Narrative 0,747 s; `Datos/OE4GP2.json` = `{"name":"OE4GP2","reachedLevel":1,"phases":[]}`; `N1_Apertura` L1 «Noche helada, sin luna…», **sin «Omitir»** (gp2b_004) | P (PF-RF02-01, PF-RF06-01) |
| 8 | 10:42:50–57 | «Continuar» ×6 (cada 700 ms) | L7 «Solo oscuridad.» (gp2b_005) | P |
| 9 | 10:43:08–19 | «Continuar» → `N1_AparicionGuia` (0,035 s) y ×10 | L1 «En la oscuridad total, un pequeño destello amarillo…»; L11 «Antes de que alguien pueda responder, Algoritm parpadea… La oscuridad vuelve de golpe.»; sin «Omitir» (gp2b_006, _007) | P |
| 10 | 10:43:53–44:08 | «Continuar» → `N1_Hallazgo` (0,035 s) y ×17 | L1 NIÑA «¿Se fue?»; L18 ALGORITM «Lo que no sabes todavía es cómo. ¿Desde dónde vas a intentarlo?»; sin «Omitir» (gp2b_008, _009) | P |
| 11 | 10:44:40 | Último «Continuar» + `Esperar-Carga Level1_Cave` | 0,155 s; **T0 = 10:44:41,33**. Cinco hojas regadas, sílex y pedernal en otros sitios que en GP1 (se colocan por entrada), tablilla «Reúne todas las hojas y las dos piedras en el centro de la pantalla.», pista (Algoritm de fuego) y pausa; sin deslizantes (gp2b_010) | P (RNF-04, PF-RF14-01) |
| 12 | 10:45:11–36 | Arrastrar las 5 hojas y las 2 piedras al centro (fotos en gp2b_011 y a +2,5 s del último) | Las hojas quedan juntas en el centro; con la segunda piedra, acercamiento: piedras sobre el montón, «Fuerza del golpe» vertical, «Lejos»–«Cerca», «Soplar» con candado y «Golpear»; tablilla «Elige la fuerza y qué tan cerca van las piedras, y golpea para hacer chispas.» (gp2b_012) | P (PF-RF14-01, PF-RF15-01) |
| 13 | 10:46:15–17 | Fuerza → muesca 7 (0,931; 0,439), cercanía → muesca 5 (0,5; 0,794) | Las asas se mueven; el asa de la fuerza pasa de azul a magenta (OBS-4 de GP1) | OK |
| 14 | 10:46:18–35 | «Golpear» ×3 (fotos a +1,5 s y +1,8 s) | 1.º «Una chispa cae dentro de las hojas. Brilla un instante y se apaga.»; 2.º «Otra chispa cae dentro. Esta vez el brillo dura un poco más.»; 3.º «Un hilo de humo sube despacio, como si la cueva estuviera suspirando.», con el hilo de humo sobre el montón y **«Soplar» habilitado sin candado** (gp2b_013–015) | P (PF-RF16-01, PF-RF19-01) |
| 15 | 10:46:47,15 | «Soplar» (foto a +1,2 s) | «El humo se abre. Algo naranja tiembla entre las hojas.»; nace la llama; «Soplar» y «Golpear» atenuados (gp2b_016) | P (PF-RF20-01) |
| 16 | 10:46:51,74 | `Esperar-Carga Narrative` | 0,336 s → `N1_NacimientoDelFuego` L1 «Una llama naranja y dorada crece despacio desde las hojas…», sin «Omitir» (gp2b_017). `OE4GP2.json` aún sin fases: la del N1 se guarda al mostrarse el resumen (diseño conocido, PF-RNF14-03) | P |
| 17 | 10:47:36–47 | «Continuar» ×16 | L17 «Esa noche la familia duerme caliente y unida.» (gp2b_018) | P (PF-RF05-01) |
| 18 | 10:48:01,70 | Último «Continuar» + `Esperar-Carga LevelSummary` | 0,035 s. «Esto es lo que pasó en la cueva», «Descubriste que el fuego necesita chispa y aire.», «Encontraste la fuerza y el sitio correctos casi de inmediato.», «Seguiste el mismo camino hasta que las chispas prendieron.», «Eso se llama probar y ajustar…»; **ningún dígito**. Los mensajes difieren de GP1 porque aquí no hubo golpes fallidos: el resumen se elige por desempeño (gp2b_019) | P (PF-RF45-01, PF-RF17-01) |
| 19 | 10:48:01 | `Datos/OE4GP2.json` | `reachedLevel` 2 y `{1, 1, attempts 0, correctedErrors 0, stepsUsed 3, resolutionSeconds 129,83}`. Previsto: I 0 (ningún golpe fallido) · C 0 · P 3 (tres golpes efectivos) y T(fin del encendido ≈ «Soplar» + 3,5 s = 10:46:50,65) − T0 = **129,3 s** (diferencia 0,5 s, del orden de la de GP1) → `GP2/GP2_OE4GP2_tras_N1.json` | P (PF-RF04-01) |
| 20 | 10:48:18 | «Continuar» del resumen + `Esperar-Carga *` | LevelSelect 0,068 s: N1 «Completado» con visto, N2 habilitado, N3 «Bloqueado» con candado (gp2b_020) | P (PF-RF03-02) |
| 21 | 10:48:40 | «Nivel 2 · La Rueda» + `Esperar-Carga Narrative` | 0,701 s → `N2_PuenteI` L1 «Amanece. La familia sale de la cueva. ALGORITM flota sobre las brasas que aún humean.», sin «Omitir»; ×2 → L3 MAMÁ «Comida.» (gp2b_021, _022) | P (RNF-04, PF-RF06-01) |
| 22 | 10:48:54–49:02 | «Continuar» → `N2_PuenteI_Bosque` (0,035 s) y ×8 | L1 «La familia sale a recolectar.»; L9 NIÑA «¿Y si el problema no es la fuerza... sino la forma?» (gp2b_023, _024) | P (PF-RF05-02) |
| 23 | 10:49:15–23 | «Continuar» → `N2_Escena21_Bosque` (0,035 s) y ×6 | L1 «Bosque. Objetos dispersos por el suelo…»; L7 ALGORITM «Selecciona los objetos que se muevan con facilidad y únelos. Cuando tengas cinco, veremos qué pasa.» (gp2b_025, _026) | P (PF-RF10-02) |
| 24 | 10:49:37,35 | Último «Continuar» + `Esperar-Carga Level2_Forest` | 0,052 s; **T0 = 10:49:37,35**. 14 objetos en otras posiciones que en GP1 (5 troncos, 3 piedras, 3 plantas, 3 palos), caja junto a la Niña, «Troncos redondos: 0 de 5» con 5 casillas, ayuda y pausa; la tablilla de tres líneas toca el borde superior (OBS-8 de GP1) (gp2b_027) | P (RNF-04, PF-RF22-01) |
| 25 | 10:50:05–40 | Clic en los 5 troncos (foto tras el 4.º y a +5 s del 5.º) | «Este rueda. ¿Qué tiene que los otros no tienen?» con el círculo verde; 4 de 5 con 4 casillas llenas; con el 5.º desaparecen los distractores, los troncos se alinean en el centro, la cámara se acerca y aparece «Empujar» con candado; «5 de 5» (gp2b_028, _029) | P (PF-RF23-01, PF-RF24-01, PF-RF26-01) |
| 26 | 10:50:53 | Arrastrar la caja (0,146; 0,611) sobre los troncos (0,52; 0,36) | «La caja quedó sobre los troncos. Ahora empújala.» con acierto; la caja sobre el primer tronco; «Empujar» habilitado sin candado (gp2b_030) | P (PF-RF25-01) |
| 27 | 10:51:04,83 | «Empujar» + `Esperar-Carga Narrative` | 0,669 s → `N2_Escena22_ElPatron` L1 «La caja rueda sobre los troncos y avanza sin esfuerzo.», la caja sobre los cinco troncos (gp2b_031). JSON: `{2, 1, 0, 0, 0, 87,46}`; previsto I 0 · C 0 · P 0 · T = 10:51:04,83 − 10:49:37,35 = **87,48 s** | P (PF-RF26-01, PF-RF04-02) |
| 28 | 10:51:22–38 | ×6 en la 2.2; «Continuar» → `N2_Escena23_Construccion` (0,035 s) y ×7 | 2.2 L7 ALGORITM «Lo que se repite en todos los que funcionan, y en ninguno de los que no.»; 2.3 L1 NIÑA «Si hacemos algo redondo... podemos usarlo.»; L8 ALGORITM «Y sobre ella, la caja de alimentos; luego amárrala con la cuerda. En ese orden…» (gp2b_032–034) | P (PF-RF05-02) |
| 29 | 10:52:01,66 | Último «Continuar» + `Esperar-Carga Level2_Workshop` | 0,069 s; **T0 = 10:52:01,66**. Las 7 piezas en fila (cuerda, tronco corto A, tronco corto B, tronco largo, mazo, tabla, caja), tablilla con la instrucción completa, ayuda, «Mecanizar» con candado y «Aún no» (gp2b_035) | P (RNF-04, PF-RF27-01) |
| 30 | 10:52:22–41 | Clic en el tronco A + «Mecanizar»; clic en el tronco B + «Mecanizar»; tronco largo sobre la rueda A | «Se abrió un agujero en el centro: el tronco ya es una rueda.» (gp2b_036); después «El tronco largo entró por el centro de las dos ruedas: ya hay un eje.», con la carretilla y Papá señalando (gp2b_037) | P (PF-RF28-01, PF-RF29-01) |
| 31 | 10:52:56–59 | Tabla y caja sobre la carretilla (0,36; 0,71) | «La caja va sobre la tabla.»; la caja llena sobre la tabla (gp2b_038) | P (PF-RF29-01) |
| 32 | 10:53:44 | Cuerda sobre la carretilla (foto a +0,5 s) | «La cuerda sujeta la caja. La carretilla está completa.»; la familia celebra con los brazos arriba; la carretilla con la cuerda amarrada (e5) (gp2b_039) | P (PF-RF29-01) |
| 33 | 10:53:46,90 | `Datos/OE4GP2.json` | `{2, 2, attempts 0, correctedErrors 0, stepsUsed 6, resolutionSeconds 105,36}`. Previsto I 0 · C 0 · P 6 (dos «Mecanizar», eje, tabla, caja, cuerda) y T = escritura del JSON (10:53:46,90) − T0 = **105,24 s** (diferencia 0,1 s; el reloj se detiene al guardar, ~1,5 s después de soltar la cuerda) | P (PF-RF04-02) |
| 34 | 10:53:48 | `Esperar-Carga Narrative` | 0,652 s → `N2_Escena24_Regreso` L1 ALGORITM «Ahora tienen la carretilla. Pero tenerla no basta.», luz de atardecer (gp2b_040) | P (RNF-04) |
| 35 | 10:54:42–51 | «Continuar» ×7 | L8 ALGORITM «Vas a escribir antes todos los pasos, y luego los ejecutas de una sola vez.» (gp2b_041) | P (PF-RF05-02) |
| 36 | 10:54:59,97 | Último «Continuar» + `Esperar-Carga Level2_Maze` | 0,071 s; **T0 = 10:54:59,97**. Vista superior con cuadrícula al atardecer, carretilla en (0,8) mirando al este con la Niña afuera, familia en el refugio, «Tu secuencia», «Suelta un bloque aquí», cajón «Bloques» cerrado y «Ejecutar» (gp2b_042) | P (RNF-04, PF-RF30-01) |
| 37 | 10:55:30 | Transcripción del tablero (rejilla superpuesta y clasificación por luminancia de cada casilla, revisada a ojo → `GP2/GP2_laberinto_rejilla.png`) | Ver «Tablero de GP2» abajo: **otro tablero que el de GP1** (aleatorio por entrada), 25 arbustos; (1,8) y (14,2) libres. La columna 9 está tapada de la fila 2 a la 9, así que el único paso es la fila 1 | OK |
| 38 | 10:56:06 | Abrir el cajón | «Avanzar», «Girar», «Retroceder» y «Arrastra un bloque a tu secuencia» (gp2b_043) | OK |
| 39 | 10:56:53–58:55 | Componer la secuencia **solo arrastrando del cajón** (nunca desde la lista, por DEF-GP1-01) y con «+»/«‹» del bloque recién soltado: «Avanzar ×1», «Girar ›», «Avanzar ×7», «Girar ‹», «Avanzar ×9», «Avanzar ×4», «Girar ‹», «Avanzar ×1», «Girar ›», «Avanzar ×1» | Cada bloque entra al final y queda desplegado; los «+» suben de uno en uno (7, 9, 4) y «‹» queda en naranja; con dos bloques aparecen ▲/▼ y la barra (gp2b_044–054). 10 bloques, 17 «+» y 2 «‹» | OK |
| 40 | 10:59:22 | Cerrar el cajón | La lista crece; filas comprimidas con «→» y la última desplegada (gp2b_055) | OK |
| 41 | 10:59:37 | «Ejecutar» (fotos a +1 s y ~+7 s) | **Llega al refugio a la primera.** A +1 s la carretilla gira en (1,8) y **ningún bloque se ve resaltado**: la lista quedó al final y los bloques 1 y 2 están fuera de la vista (gp2b_056 → `GP2/DEF-GP1-02_reproducido_1s.png`); a ~+7 s la carretilla sale de (1,1) hacia el este y el bloque 5 («Avanzar ×9») se ve resaltado (gp2b_057) | P; **DEF-GP1-02 se reproduce** (PF-RF32-01 sigue en F) |
| 42 | 10:59:58,09 | Llegada; `Datos/OE4GP2.json` | Fase `{2, 3, attempts 0, correctedErrors 0, stepsUsed 10, resolutionSeconds 298,35}`; `reachedLevel` sigue en 2. Previsto I 0 · C 0 (sin ejecución fallida no se cuentan ediciones) · P 10 · T = 10:59:58,09 − T0 = **298,12 s** (diferencia 0,2 s) | P (PF-RF04-02) |
| 43 | 10:59:59,39 | `Esperar-Carga Narrative` | 0,703 s → `N2_Escena25_Cierre` L1 «La familia está en su refugio alrededor del fuego, preparando alimentos.», de noche, sin «Omitir» (gp2b_058) | P (RNF-04, PF-RF06-01) |
| 44 | 11:00:46–53 | «Continuar» ×5 | L6 ALGORITM «Y después ordenaron los pasos antes de dar el primero: eso es pensar como un algoritmo.»; Algoritm sobre la fogata (gp2b_059) | P (PF-RF12-02) |
| 45 | 11:01:04,79 | Último «Continuar» + `Esperar-Carga LevelSummary` | 0,035 s. «Esto es lo que pasó con la rueda», «Descubriste que lo redondo rueda y que una carretilla se arma en orden.», «Elegiste los troncos, el orden de las piezas y el camino casi de inmediato.», «Seguiste tu plan del bosque al taller y del taller al refugio.», «Eso se llama abstraer y pensar como un algoritmo…»; **ningún dígito** (mensajes de «sin errores», distintos de GP1) (gp2b_060). JSON: `reachedLevel` **3** y las cuatro fases intactas → `GP2/GP2_OE4GP2_tras_N2.json` | P (PF-RF45-02, PF-RF17-01, PF-RF04-02) |
| 46 | 11:01:18 | «Continuar» del resumen + `Esperar-Carga *` | LevelSelect 0,068 s: N1 y N2 «Completado» con visto; **N3 habilitado** sin candado (gp2b_061) | P (PF-RF03-02) |
| 47 | 11:01:45–49 | «Nivel 3 · El Río» + `Esperar-Carga Narrative`; ×1 | 0,684 s → `N3_PuenteII` L1 «La familia celebra en el refugio. La carretilla está cargada con alimentos y piedras.», sin «Omitir»; L2 ALGORITM (retrato de madera) «¡Lo lograron! La rueda los ayudó a traer todo hasta aquí…» (gp2b_062, _063) | P (RNF-04, PF-RF05-03, PF-RF06-01) |
| 48 | 11:01:50–56 | «Continuar» → `N3_PuenteII_Horizonte` (0,035 s) y ×4 | L1 «La cámara se desplaza hacia el horizonte…»; L5 ALGORITM «El camino existe... pero hay un problema.» (gp2b_064, _065) | P |
| 49 | 11:02:12–18 | «Continuar» → `N3_PuenteII_Rio` (0,035 s) y ×3 | L1 «La familia avanza con la carretilla. El camino se corta abruptamente…»; L4 ALGORITM, ya gota de agua, «Exacto. La rueda los trajo hasta acá…» (gp2b_066, _067) | P (INC-45) |
| 50 | 11:02:19–27 | «Continuar» → `N3_Escena31_Llegada` (0,035 s) y ×7 | L1 ALGORITM «¡Alto, viajeros! Este río no es sencillo…»; L8 «En pantalla aparece la lista de tareas, que permanecerá visible durante todo el nivel.» (gp2b_068, _069) | P (PF-RF10-03) |
| 51 | 11:02:45,54 | Último «Continuar» + `Esperar-Carga Level3_River` | 0,069 s. Orilla: lista con las cuatro tareas pendientes, «Recorre la orilla y recoge lo que la balsa necesita. La lista te dice qué falta.», inventario 2×2 vacío, cruceta, ayuda (gota) y pausa; 8 materiales (5 troncos, sogas, tela, mástil de pie) en los mismos sitios que en GP1; zona marcada; sin «Recoger» (gp2b_070) | P (RNF-04, PF-RF36-01, PF-RF35-01) |
| 52 | 11:03:19–06:20 | Recolección con la ruta de GP1 sin la visita a la zona incompleta (`Sostener` ↓300 ←3000 · ↑… según `coords.json`): tronco_1, tronco_5, tela, sogas, tronco_2, mástil, tronco_4, tronco_3; «Recoger» tras cada llegada (fotos de control antes de cada «Recoger») | «Recoger» aparece solo junto a cada material; cada uno con su mensaje («Troncos: al inventario. Mira la lista: ¿qué falta?», «Tela: …», «Sogas: …», «Mástil: …»); **con las sogas se marca «Encontrar sogas»**, las marcas de troncos suben de 1 a 5 y **con el quinto tronco se marca «Recoger troncos»** y sale «Ya tienes todos los materiales. Ve a la zona marcada junto al agua.» (gp2b_071–081) | P (PF-RF35-01, PF-RF36-01, PF-RF37-01, PF-RF38-01) |
| 53 | 11:06:39–46 | ↓720, →290, ↑120: entrar a la zona (foto a +2,5 s) | «Tienes todo lo de la lista. Aquí se arma la balsa.»; desaparecen la cruceta y «Recoger»; la cámara se acerca: balsa con las 5 siluetas de la base sobre el agua, «Listo» y «Probar balsa» lado a lado; Mamá en la zona (gp2b_082). **T0 ≈ 11:06:43,3** (fin del último `Sostener`) | P (PF-RF39-01, PF-RF40-01) |
| 54 | 11:07:00–11 | Arrastrar 5 troncos del inventario a las 5 siluetas | «Puesto. Cuando toda esta parte esté, pulsa el botón.»; la base completa sobre el agua; casilla de troncos vacía (gp2b_083) | OK |
| 55 | 11:07:28,86 | «Listo» (foto a +1,5 s) | «La base quedó firme. Ahora hay que sujetarla para que no se abra.» con acierto; aparecen las 10 siluetas de amarre (gp2b_084). JSON (11:07:28,84): `{3, 1, 0, 0, 1, 45,57}`; previsto I 0 · C 0 · P 1 · T = 11:07:28,86 − 11:06:43,30 = **45,56 s** | P (PF-RF41-01, PF-RF04-03) |
| 56 | 11:07:45–08:04 | Sogas a los 10 amarres | Las 10 bandas verdes puestas; casilla de sogas vacía (gp2b_085) | OK |
| 57 | 11:08:17,48 | «Listo» (foto a +1,5 s) | «La balsa ya no se abre. Falta lo que atrapa el viento.»; **se marca «Ensamblar la balsa»**; siluetas de mástil y vela; abajo un solo botón, «Probar balsa» (gp2b_086). JSON: `{3, 2, 0, 0, 2, 48,61}`; previsto T = 11:08:17,48 − 11:07:28,86 = **48,62 s** | P (PF-RF41-01, PF-RF36-01, PF-RF04-03) |
| 58 | 11:09:04–08 | Mástil a su silueta y tela a la vela | Puestos (gp2b_087) | OK |
| 59 | 11:09:34,63 | «Probar balsa» (foto a +0,7 s) | «La balsa flota derecha. ¡A cruzar!» con acierto; la familia celebra; «Probar balsa» atenuado (gp2b_088). JSON (11:09:35,52): `reachedLevel` 3 y `{3, 3, 0, 0, 3, 77,15}`; previsto T = 11:09:34,63 − 11:08:17,48 = **77,15 s** | P (PF-RF44-01, PF-RF04-03) |
| 60 | 11:09:37,36 | `Esperar-Carga Narrative` | 0,702 s → **directo a `N3_Escena33_Cruce`, sin la 3.2**: con la balsa terminada bien a la primera no hay primer fallo, y el guion §1.8.4.1 dice «si el jugador acierta en la primera prueba de esa fase, la escena no se reproduce y se pasa directamente al cruce». L1 «La familia sube a la balsa corregida. Esta vez flota con estabilidad. La vela se llena de viento.» (gp2b_089) — ver OBS-GP2-1 | P (PF-RF05-03, PF-RF44-01) |
| 61 | 11:09:48–11:39 | 3.3 a ritmo de lectura (×6 cada 2 s) y foto a ~+9 s | L7 ALGORITM «Eso es pensar computacionalmente.»; la balsa llegó a la otra orilla y los cuatro bajaron al pasto a celebrar, igual que en GP1 (gp2b_090, _091) | P (PF-RF44-01, PF-RF12-03) |
| 62 | 11:12:36,07 | Último «Continuar» + `Esperar-Carga LevelSummary` | 0,035 s. «Esto es lo que pasó en el río», «Descubriste que una balsa grande se arma por partes…», «Pusiste cada pieza en su sitio y la balsa flotó a la primera.», «No hizo falta rehacer nada: cada fase quedó bien antes de seguir.», «Eso se llama descomponer y depurar…»; **ningún dígito**. La tarjeta llega a los bordes de arriba y abajo sin cortar texto (OBS-15 de GP1) (gp2b_092) | P (PF-RF45-03, PF-RF17-01) |
| 63 | 11:12:51 | **«Continuar»** del resumen (GP1 usó «Volver al menú de niveles») + `Esperar-Carga *` | Narrative 0,701 s → `N3_EscenaFinal` L1 «La familia camina hacia las fogatas. Detrás quedan el río, el bosque y, muy lejos, la cueva.», sin «Omitir». Los dos botones del resumen del último nivel llevan a la escena final (INC-39) (gp2b_093) | P (PF-RF45-03) |
| 64 | 11:12:58–13:12 | ×6 (cada 1,5 s) | L7 «Algoritm gira una última vez y se apaga suavemente. Fundido a negro.»; la fogata central sigue saliendo detrás de la cabeza de la Niña (OBS-13 de GP1) (gp2b_094) | P (PF-RF05-03) |
| 65 | 11:13:38,51 | Último «Continuar» + `Esperar-Carga Credits` | 0,036 s; «Créditos» con la rejilla y Algoritm de fuego (gp2b_095) | P (PF-RF44-01, PF-RF08-01) |
| 66 | 11:14:28,44 | «Volver» + `Esperar-Carga MainMenu` | 0,052 s; pantalla de inicio (gp2b_096) | P |
| 67 | 11:12:35,96 | `Datos/OE4GP2.json` final (último guardado: al mostrarse el resumen del N3) | `reachedLevel` 3 y las siete fases → `GP2/GP2_OE4GP2_final.json` (ver la tabla de indicadores) | P (PF-RF04-01..03) |
| 68 | 11:14:40,76 | `Cerrar-Juego` | Cierre normal (WM_CLOSE); ningún proceso Algoritmia.exe; muestreador terminado; `Datos/` solo con `OE4GP2.json` | OK |
| 69 | 11:14:41 | `Revisar-Log GP2` | **31 cargas RNF-04, todas < 1 s** (peor: Narrative 0,747 s, la de `N1_Apertura` tras crear el perfil); **sin Exception, Error ni Assert** → `GP2/GP2_Player.txt` | P (PF-RNF04-01, PF-RNF13-01) |
| 70 | 11:14:48 | `Medidas GP2` | 987 muestras. **RNF-05: Private máx. 1133 MiB (< 2048): CUMPLE.** RNF-10: 1287 filas EXTERNA, las mismas tres conexiones TCP :443 a 34.8.90.77, 34.111.113.40 y 34.107.172.168 → **sigue DEF-SPIKE-01** → `GP2/GP2_{mem,red}.csv`, `GP2/GP2_registro.tsv`. Medidas cuenta 30 cargas y el registro tiene 31: la de LevelSelect que pasa un instante al crear el perfil no tuvo `Esperar-Carga` | RNF-05 P · RNF-10 F (DEF-SPIKE-01) |
| 71 | 11:14:57 | `Residuos GP2 -Buscar OE4GP2` | Fuera de la carpeta portable: `Player.log`, `Unity\<id>\Analytics\…`, `Insights\…` y `ShaderVariantAnalytics\…` en LocalLow (DEF-SPIKE-01), y la clave HKCU del reproductor (20 valores). %TEMP%: nada del juego. **«OE4GP2» solo aparece en `Datos\OE4GP2.json`** → `GP2/GP2_residuos.txt` | P en el perfil; F de RNF-11 por DEF-SPIKE-01 (ya registrado) |
| 72 | 11:15:04 | `Restaurar-Registro` | Borradas la clave del producto y la de la empresa (no existían antes); `Test-Path` da False en las dos | OK |

### Tablero de GP2 (`Level2_Maze`, 10:55:00)

Leyenda: `#` seto, `S` salida, `R` refugio, `X` arbusto, `.` libre; la fila 10 va arriba.

```
     0123456789012345
10   ################
 9   #........X.....#
 8   S.X.XXX..X.....#
 7   #.......XX.....#
 6   #.......XXXX...#
 5   #.....X.XX....X#
 4   #.....X..X.....#
 3   #........X.....#
 2   #....XXX.XX....R
 1   #..............#
 0   ################
```

**Camino previsto y ejecutado** (10 bloques; la carretilla llegó a la primera):

| Bloque | Llega a |
|---|---|
| Salida | (0,8) |
| «Avanzar ×1» | (1,8) |
| «Girar ›» | gira al sur |
| «Avanzar ×7» | (1,1) |
| «Girar ‹» | gira al este |
| «Avanzar ×9» | (10,1) |
| «Avanzar ×4» | (14,1) |
| «Girar ‹» | gira al norte |
| «Avanzar ×1» | (14,2) |
| «Girar ›» | gira al este |
| «Avanzar ×1» | (15,2) = refugio |

La secuencia ganadora coincide con la de GP1 aunque el tablero es otro. En los dos, la columna 9 está tapada de la fila 2 a la 9, y eso obliga a pasar por la fila 1.

## Indicadores (`Datos/OE4GP2.json`)

El recorrido no tuvo errores: ninguna acción evaluada falló. Por eso I y C valen 0 en todas las fases y no se disparó ninguna pista.

| Fase | Previsto (I · C · P · T) | JSON | Veredicto |
|---|---|---|---|
| N1F1 cueva | 0 · 0 · 3 · ≈129,3 s (fin del encendido − T0) | 0 · 0 · 3 · 129,83 s | P |
| N2F1 bosque | 0 · 0 · 0 · 87,48 s | 0 · 0 · 0 · 87,46 s | P |
| N2F2 taller | 0 · 0 · 6 · 105,24 s (escritura del JSON − T0) | 0 · 0 · 6 · 105,36 s | P |
| N2F3 laberinto | 0 · 0 · 10 · 298,12 s | 0 · 0 · 10 · 298,35 s | P |
| N3F1 base | 0 · 0 · 1 · 45,56 s | 0 · 0 · 1 · 45,57 s | P |
| N3F2 amarre | 0 · 0 · 2 · 48,62 s | 0 · 0 · 2 · 48,61 s | P |
| N3F3 mástil y vela | 0 · 0 · 3 · 77,15 s | 0 · 0 · 3 · 77,15 s | P |

Tiempo de resolución de las siete fases: **792,33 s (13 min 12 s)**.

## Duración del recorrido GP#2

**Total, de `Start-Juego` a `Cerrar-Juego`:** 15:41:43,956Z → 16:14:40,762Z = **32 min 56,8 s** (10:41:44 → 11:14:41, hora de Bogotá). Fue un solo proceso (pid 44004, lanzamiento 1) y un solo agente, sin relanzar ni relevos.

| Tramo | Horas | Duración |
|---|---|---|
| Inicio y perfil | 10:41:44–10:42:40 | ≈1 min |
| N1 | 10:42:40–10:48:18 | ≈5 min 38 s |
| N2 | 10:48:18–11:01:19 | ≈13 min (5 min en el laberinto) |
| N3 hasta los créditos | 11:01:19–11:13:39 | ≈12 min 20 s |
| Vuelta al inicio y cierre | 11:13:39–11:14:41 | ≈1 min |

La cifra incluye la latencia del arnés (≈1–2 s por llamada), las fotos de control y la transcripción del tablero. No mide a un estudiante: esa duración sale del recorrido de Santiago con cronómetro (Hoja-HUM, H8).

## Casos en GP#2

| Caso | Veredicto | Nota |
|---|---|---|
| **PF-RNF13-01 (GP#2, pantalla completa, «como se entrega»)** | **P** | Inicio → perfil nuevo → N1 → N2 (tres fases) → N3 (recolección y tres fases) → escena final → créditos → inicio. **Un solo proceso a pantalla completa**, 32 min 57 s. Sin bloqueos, sin cierre inesperado y sin estados irrecuperables. **Sin Exception ni Error en `Player.log`**. Las siete fases quedan en el JSON. Con DEF-GP1-02 reproducido (no bloquea) y DEF-SPIKE-01 |
| PF-RF02-01 | P | El perfil nuevo guarda solo el nombre; JSON `{"name","reachedLevel":1,"phases":[]}` |
| PF-RF03-02 | P | Tras el N1, N2 habilitado y N3 bloqueado; tras el N2, N3 habilitado |
| PF-RF04-01 · -02 · -03 | P | Las siete fases con las cifras previstas (tabla de indicadores) |
| PF-RF05-01 · -02 · -03 · PF-RF06-01 | P | 17 narrativas en orden, una línea por clic; ninguna ofrece «Omitir» en la primera visita |
| PF-RF05-03 / guion §1.8.4.1 | P | **Nuevo respecto de GP1:** si la balsa terminada no falla, la 3.2 no aparece y se pasa directo al cruce |
| PF-RF14-01 · -15-01 · -16-01 · -19-01 · -20-01 | P | Reunir, deslizantes, tres golpes efectivos, «Soplar» habilitado al tercero, llama |
| PF-RF22-01 · -23-01 · -24-01 · -25-01 · -26-01 | P | Cinco troncos sin rechazos, la caja sobre los troncos y «Empujar» |
| PF-RF27-01 · -28-01 · -29-01 | P | El taller sale en orden a la primera, con la e5 a la vista |
| PF-RF30-01 · -31-01 · -33-01 | P | Tablero nuevo; los bloques muestran cuenta y lado; llega a la primera |
| **PF-RF32-01** | **F** | **DEF-GP1-02 se reproduce.** Con la lista desplazada al final, los bloques 1 y 2 se ejecutan sin resaltado visible (`GP2/DEF-GP1-02_reproducido_1s.png`); el 5.º sí se ve (`GP2/PF-RF32-01_bloque5_resaltado.png`) |
| PF-RF34-02 (DEF-GP1-01) | NE | El recorrido evitó a propósito arrastrar bloques desde la secuencia |
| PF-RF35-01 · -36-01 · -37-01 · -38-01 · -39-01 | P | Recolección completa; las tareas se marcan por forma y color; la zona abre el ensamblaje |
| PF-RF40-01 · -41-01 · -44-01 | P | Las tres fases se aprueban a la primera; la balsa flota y cruza; escena final y créditos |
| PF-RF45-01 · -02 · -03 · PF-RF17-01 | P | Los tres resúmenes salen **en su variante sin errores** (no vista en GP1) y sin dígitos. «Continuar» del último nivel lleva a la escena final |
| PF-RF13-0x · PF-RF11-0x | NE | Sin errores no hubo pistas ni rechazos que medir (los cubrió GP1) |
| PF-RNF02-01 | NE | No se probaron teclas (lo cubrió GP1) |
| PF-RNF04-01 | P | 31 cargas < 1 s; la peor, 0,747 s |
| PF-RNF05-01 | P | 1133 MiB |
| PF-RNF10-01 · RNF-11 | F | DEF-SPIKE-01 (Unity Analytics/Connect), ya registrado |

## Defectos

- **Nuevos del juego:** ninguno.
- **Reproducidos:** DEF-GP1-02 (PF-RF32-01) y DEF-SPIKE-01 (RNF-10, RNF-11).
- **No ejercitados:** DEF-GP1-01 y DEF-GP1-03. Se evitó arrastrar bloques desde la secuencia y no se usó la papelera.
- **Del arnés:** ninguno. A pantalla completa funcionó con las mismas coordenadas normalizadas que la ventana sin marco de GP1 (cliente 1920×1080 en (0,0), DPI 125 %).

## Observaciones (no son defectos)

**OBS-GP2-1 · La 3.3 da por hecho un fallo que en el camino sin errores no ocurrió.**
- **Qué pasa:** con la balsa bien a la primera, el juego salta la 3.2, como manda el guion §1.8.4.1. Pero la 3.3 abre con «La familia sube a la balsa corregida. Esta vez flota con estabilidad…». Algoritm la cierra con «…cuando algo falló no se rindieron... lo encontraron y lo arreglaron. Eso es pensar computacionalmente.». En este camino no se corrigió ni falló nada (`GP2/OBS-GP2-1_e33_sin_32_balsa_corregida.png`).
- **Por qué no es defecto:** es el texto del guion §1.8.5, un documento radicado; el juego lo cumple.
- **Contraste:** el resumen del N3 sí distingue el caso: «la balsa flotó a la primera».
- **Para Santiago:** ¿se escribe una variante de la 3.3 para el camino sin fallo, como ya hacen los resúmenes, o se acepta así?

**Otras observaciones:**
- **OBS-1 (de GP1):** la superposición de NVIDIA asoma también a pantalla completa los primeros ~10 s (gp2b_001).
- **Se repiten sin cambios:**
  - OBS-5: «Completado» y «Bloqueado» tocan el borde de la píldora.
  - OBS-6: rótulo «Algoritm» en el resumen.
  - OBS-8: tablilla del bosque llena.
  - OBS-12: el mástil de pie se dibuja delante de Mamá.
  - OBS-13: fogata tras la Niña en la escena final.
  - OBS-15: la tarjeta del resumen del N3 llega a los bordes.
- **Pantalla completa frente a ventana:** a 1920×1080 no hay ninguna diferencia de disposición ni de comportamiento respecto de GP1.

## Estado al terminar

- **Juego:** cerrado con cierre normal; muestreador terminado.
- **Registro:** restaurado. Se borraron las claves del producto y de la empresa, como estaban antes de GP2.
- **Perfil:** `Build/Algoritmia/Datos/OE4GP2.json` queda en su sitio, con copia en `GP2/GP2_OE4GP2_final.json`. La siguiente sesión lo vacía con `Preparar-Datos`.
- **LocalLow:** quedan los restos de Unity Analytics de DEF-SPIKE-01 y el `Player.log`. No se borran porque son evidencia del defecto.
- **Editor:** sigue cerrado. No se corrigió código.
