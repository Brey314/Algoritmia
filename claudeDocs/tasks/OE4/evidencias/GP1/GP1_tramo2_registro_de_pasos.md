# GP1 · tramo 2/3 — S-N2A, S-N2B y S-N2C completos con OE4GP1 — 01/10/2026

- **Recorrido:** Golden Path #1 (PF-RNF13-01), mismo proceso del tramo 1 (pid 40152, sesión del arnés `GP1`, ventana sin marco 1920×1080 en (0,0)).
- **Perfil:** `OE4GP1`, recibido en el menú de niveles con N1 completado y N2 habilitado.
- **Evidencias:** `claudeDocs/tasks/OE4/evidencias/GP1/` (prefijo de las fotos de trabajo `gp2_`); figuras del OE3 en `claudeDocs/tasks/OE4/evidencias/figuras/`.
- **Este tramo NO cierra el juego**: lo recibe el tramo 3 en el menú de niveles con el Nivel 3 desbloqueado.

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 09:24:11 | Foto gp2_001 | «Elige un nivel»: N1 «Completado» con visto, N2 habilitado, N3 «Bloqueado» con candado; proceso 40152 vivo | OK (recibido) |
| 2 | 09:25:44 | Clic «Nivel 2 · La Rueda» + `Esperar-Carga Narrative` | 0,668 s → `N2_PuenteI` L1 «Amanece. La familia sale de la cueva. ALGORITM flota sobre las brasas que aún humean.»; cueva con brasas y humo, sin «Omitir» y sin pausa (gp2_002) | P (RNF-04, PF-RF06-01) |
| 3 | 09:25:57–26:03 | «Continuar» ×2 | 3 líneas distintas: L2 ALGORITM «Ahora que tienen fuego, pueden cocinar. Pero para cocinar hace falta algo más.», L3 MAMÁ «Comida.»; retrato y nombre de quien habla; luz de amanecer azulada (hoja_n2pi) | P (PF-RF05-02) |
| 4 | 09:26:38–27:00 | «Continuar» → `N2_PuenteI_Bosque` (0,035 s) y «Continuar» ×8 | 9 líneas distintas, una por clic, de «La familia sale a recolectar.» a NIÑA «¿Y si el problema no es la fuerza... sino la forma?»; sin «Omitir» (hoja_n2pb) | P (PF-RF05-02, PF-RF06-01) |
| 5 | 09:27:17–35 | «Continuar» → `N2_Escena21_Bosque` (0,035 s) y «Continuar» ×6 | 7 líneas distintas; L3 ALGORITM «Observen bien. No todo lo que ven es importante.»; L7 ALGORITM «Selecciona los objetos que se muevan con facilidad y únelos. Cuando tengas cinco, veremos qué pasa.» (indicación paso a paso, **sin pregunta**); sin «Omitir»; Algoritm en su forma de rueda (madera) → `GP1/PF-RF10-02_objetivo_bosque.png` | P (PF-RF05-02, PF-RF06-01, PF-RF10-02) |
| 6 | 09:27:52 | Último «Continuar» + `Esperar-Carga Level2_Forest` | **0,052 s; T0 = 09:27:52,43** | P (RNF-04) |
| 7 | 09:27:55 | Foto gp2_006 | Tablilla con la instrucción; «Troncos redondos: 0 de 5» y 5 casillas vacías; **14 objetos**: 5 troncos redondos, 3 piedras, 3 plantas (amarilla, verde, naranja) y 3 herramientas (palos); la caja en el suelo junto a la Niña; sin «Empujar»; ayuda (Algoritm de rueda, abajo a la derecha) y pausa → `GP1/PF-RF22-01_bosque_inicio.png`. El texto de tres líneas llena la tablilla: la primera línea toca el borde superior (OBS-8) | P (PF-RF22-01, PF-RF24-01) |
| 8 | 09:28:14 | Teclas de control negativo | Nada cambia salvo el balanceo de reposo de los personajes (gp2_007) | P (PF-RNF02-01) |
| 9 | 09:28:44 | Arrastrar la caja a su lado (0,24; 0,65 → 0,33; 0,70) | «Todavía faltan 5 troncos para mover la caja.»; la caja no se mueve (gp2_008) | P (PF-RF25-01) |
| 10 | 09:29:15 | Clic en el tronco (0,089; 0,444) — **orden cambiado**: primero un tronco para que la figura 4.4 tenga «tronco ya en el acopio»; las cifras previstas no cambian (el tronco previo no es un error corregido) | «Este rueda. ¿Qué tiene que los otros no tienen?» con el icono de acierto (círculo verde, `ui_circulo`); el tronco pasa a la casilla 1; «1 de 5»; la Niña señala (gp2_009 → `GP1/PF-RF23-01_tronco_acierto.png`) | P |
| 11 | 09:29:18 | Clic en una piedra (0,447; 0,657), foto a +450 ms | «Esta piedra tiene esquinas. Cuando la empujas, se traba.» con triángulo de alerta; la piedra vuelca una esquina y sigue en su sitio; «1 de 5»; la Niña con el puño arriba (ánimo) → **fig-04b-1-bosque-rechazo.png**, `GP1/PF-RF23-01_piedra_alerta.png` | P (PF-RF23-01, PF-RF11-02, PF-RNF19-01) |
| 12 | 09:29:45 | Clic en la planta amarilla (0,583; 0,454) | «Esto no soporta el peso de la caja.» con alerta (gp2_011) | P (PF-RF23-01) |
| 13 | 09:29:48 | Clic en una herramienta (0,750; 0,454) — 3.er rechazo seguido | En lugar del mensaje, la pista «Mira cómo se mueve cada uno cuando lo empujas. ¿Cuáles se traban y cuáles no?», **sin icono** → `GP1/PF-RF13-02_pista_bosque.png` | P (PF-RF13-02) |
| 14 | 09:29:57 | Botón de ayuda (0,9375; 0,903) | Repite «Selecciona los objetos que se muevan con facilidad y únelos. Cuando tengas cinco, veremos qué pasa.» (gp2_013). Al pasar el cursor, la planta y el palo se desplazan un poco (`ForestObjectNudge`, diseño: cada categoría reacciona distinto al cursor) y quedan en el suelo | P (PF-RF13-02) |
| 15 | 09:30:15–20 | Clic en los troncos 2, 3 y 4 | «Este rueda…» con acierto cada vez; el contador sube 2 → 3 → 4 de 5 y nunca baja; las casillas se llenan en orden (hoja_bq2) | P (PF-RF24-01, PF-RF23-01) |
| 16 | 09:30:36–41 | Clic en el quinto tronco; fotos a +0,4 s y +5 s | Celebran (Mamá, Papá y la Niña con los brazos arriba); desaparecen los distractores; los cinco troncos vuelan del acopio y se alinean en el centro; la cámara se acerca; aparece «Empujar» **deshabilitado y con candado**; «Troncos redondos: 5 de 5» sigue a la vista (las casillas se retiran) → `GP1/PF-RF26-01_alineados_empujar_candado.png` | P (PF-RF24-01, PF-RF26-01, PF-RNF19-01) |
| 17 | 09:30:56 | Arrastrar la caja lejos de los troncos | «Ahí la caja no toca los troncos. Déjala encima de ellos.» con alerta; la caja queda donde se soltó; «Empujar» sigue con candado → `GP1/PF-RF25-01_caja_lejos.png` | P (PF-RF25-01) |
| 18 | 09:31:09 | Arrastrar la caja sobre los troncos | «La caja quedó sobre los troncos. Ahora empújala.» con acierto; la caja se asienta sobre el primer tronco (x 650–880, y 300–478 px); «Empujar» habilitado y sin candado → `GP1/PF-RF25-01_caja_colocada_empujar.png` | P (PF-RF25-01, PF-RF26-01) |
| 19 | 09:31:19,9 | «Empujar» (foto a +0,9 s) y `Rafaga gp2_e22_rodado 5 8` | La foto a +0,9 s cae en el fundido (negro); `N2_Escena22_ElPatron` 0,685 s (09:31:20,996). La ráfaga empezó ~2 s después: en su primer cuadro los cinco troncos están **en los mismos píxeles** que en el bosque y la caja llena está sobre ellos **a la misma altura (y 300–478) y del mismo tamaño (≈228 px de ancho)**, ya corrida ≈110 px a la derecha por el rodado; en los cuadros siguientes rueda por encima de los troncos y cae al final, junto a Papá → `GP1/PF-RF26-01_e22_caja_sobre_troncos.png`, `rafagas/gp2_e22_rodado.csv`. El cuadro exacto de apertura (antes de rodar) no se capturó: la posición de arranque en x se infiere (OBS-9) | P (PF-RF26-01; corrección «caja de la 2.2 = caja del bosque»: altura, tamaño y troncos comprobados; x de arranque inferida) |
| 20 | 09:31:19,88 | `Datos/OE4GP1.json` | Fase `{2, 1, attempts 3, correctedErrors 1, stepsUsed 0, resolutionSeconds 207,48}`. Previsto: I 3 · C 1 · P 0 y T(«Empujar») − T0 = 09:31:19,88 − 09:27:52,43 = **207,45 s** (diferencia 0,03 s) | P (PF-RF04-02) |
| 21 | 09:32:24–33:00 | `N2_Escena22_ElPatron` L1–L7 | 7 líneas distintas: «La caja rueda sobre los troncos y avanza sin esfuerzo.», NIÑO «¡Este tronco rueda!», PAPÁ «Pero esa piedra no...», MAMÁ «¿Qué diferencia hay?», NIÑA «Los que ruedan... son redondos.», ALGORITM «Acabas de encontrar un patrón.», ALGORITM «Lo que se repite en todos los que funcionan, y en ninguno de los que no.»; sin «Omitir». L6 → **fig-04b-2-escena22-patron.png** | P (PF-RF05-02) |
| 22 | 09:34:12–35:00 | «Continuar» → `N2_Escena23_Construccion` (0,035 s) y «Continuar» ×7 | 8 líneas distintas: NIÑA «Si hacemos algo redondo... podemos usarlo.», PAPÁ «Construyamos uno.», NIÑO «¡Probemos!», MAMÁ «Paso a paso... podemos lograrlo.» y ALGORITM ×4 hasta «Y sobre ella, la caja de alimentos; luego amárrala con la cuerda. En ese orden: cada paso necesita que el anterior esté hecho.» (**nombra la cuerda**); la cuerda está pintada en el suelo; sin «Omitir» (hoja_e23) | P (PF-RF05-02) |
| 23 | 09:35:01 | Último «Continuar» + `Esperar-Carga Level2_Workshop` | **0,069 s; T0 = 09:35:01,00**. Las **7 piezas**: cuerda, tronco corto A, tronco corto B, tronco largo, mazo, tabla y caja; tablilla con la instrucción completa (3 líneas); ayuda arriba a la izquierda; «Mecanizar» deshabilitado con candado y «Aún no» → `GP1/PF-RF27-01_taller_siete_piezas.png` | P (RNF-04, PF-RF27-01, PF-RF28-01) |
| 24 | 09:35:33 | Arrastrar el mazo sobre el tronco corto A sin elegirlo (I 1) | «Mecanizar necesita un tronco corto resaltado.» con alerta → `GP1/PF-RF28-01_mazo_sin_elegir.png` | P (PF-RF28-01) |
| 25 | 09:35:42 | Clic en el tronco corto A | «El tronco quedó resaltado.»; el tronco lleva contorno y crece un poco; «Mecanizar» se habilita y desaparece «Aún no» (gp2_026) | P (PF-RF28-01) |
| 26 | 09:35:53 | «Mecanizar» (C 1, P 1) | El tronco A muestra el agujero; «Se abrió un agujero en el centro: el tronco ya es una rueda.» con acierto; «Mecanizar» vuelve a candado y «Aún no» → `GP1/PF-RF28-01_rueda1.png` | P (PF-RF28-01) |
| 27 | 09:36:05 | Tabla sobre el tronco B, foto a +1,3 s (I 2) | «La tabla no tiene sobre qué apoyarse todavía.» con alerta; la tabla ya está de vuelta en su sitio; la rueda A sigue siendo rueda; «Mecanizar» con candado y «Aún no» → **fig-04b-3-taller-fuera-de-orden.png**, `GP1/PF-RF29-01_tabla_fuera_de_orden.png` | P (PF-RF29-01) |
| 28 | 09:36:22 | Caja sobre el tronco B (I 3) | «La caja se caería. Falta algo plano debajo.» con alerta; la caja vuelve (gp2_029) | P (PF-RF29-01) |
| 29 | 09:36:26 | Cuerda sobre el tronco B (I 4, 3.er rechazo seguido) | En lugar del mensaje, la pista «Mira la pieza que quieres poner. ¿Sobre qué se apoyaría si la sueltas ahora?», sin icono → `GP1/PF-RF13-03_pista_taller.png` | P (PF-RF13-03) |
| 30 | 09:36:38 | Botón de pista (0,0625; 0,111) | Repite la instrucción completa del taller (gp2_031) | P (PF-RF13-03) |
| 31 | 09:36:43 | Tronco largo soltado lejos (0,62; 0,45) | «La pieza vuelve a su sitio: cayó lejos del lugar de armado.»; vuelve a su sitio; no suma (gp2_032) | P (PF-RF29-01, INC-115) |
| 32 | 09:37:11–15 | Clic en el tronco B; mazo soltado lejos (0,80; 0,45) | «El tronco quedó resaltado.»; después «La pieza vuelve a su sitio: cayó lejos del lugar de armado.» y **el tronco B sigue resaltado** con «Mecanizar» habilitado → `GP1/PF-RF29-01_mazo_lejos_INC115.png` | P (PF-RF28-01, PF-RF29-01) |
| 33 | 09:37:26 | «Mecanizar» (C 2, P 2) | «Segunda rueda lista. Las dos tienen por dónde entrar algo.»; las dos ruedas con agujero; candado de vuelta (gp2_035) | P (PF-RF28-01) |
| 34 | 09:37:38 | Tronco largo sobre la rueda A (P 3) | «El tronco largo entró por el centro de las dos ruedas: ya hay un eje.»; aparece la carretilla con su eje (gp2_036) | P (PF-RF29-01) |
| 35 | 09:38:08 | Caja sobre la carretilla (I 5) | «La caja se caería. Falta algo plano debajo.» con alerta; el eje se conserva (gp2_037) | P (PF-RF29-01) |
| 36 | 09:38:13 | Tabla sobre la carretilla (C 3, P 4) | «La tabla quedó montada sobre el conjunto.» (gp2_038) | P |
| 37 | 09:38:26 | Caja sobre la carretilla (P 5) | «La caja va sobre la tabla.»; la caja llena encima de la tabla (gp2_039) | P |
| 38 | 09:38:37 | Cuerda sobre la carretilla (P 6), foto a +0,5 s | «La cuerda sujeta la caja. La carretilla está completa.»; **la carretilla pasa al dibujo amarrado: la cuerda rodea la caja y baja hasta la tabla (e5)**; la cámara se acerca y la familia celebra con los brazos arriba → `GP1/PF-RF29-01_cuerda_amarrada_e5.png`. A +1,5 s ya está en el fundido (gp2_041, casi negro) | **P — corrección «e5, carretilla amarrada» comprobada** |
| 39 | 09:38:38,93 | `Datos/OE4GP1.json` | Fase `{2, 2, attempts 5, correctedErrors 3, stepsUsed 6, resolutionSeconds 217,93}`. Previsto: I 5 · C 3 · P 6 y T(cuerda) − T0 = 09:38:38,93 − 09:35:01,00 = **217,93 s** | P (PF-RF04-02) |
| 40 | 09:38:40 | `Esperar-Carga Narrative` | `N2_Escena24_Regreso` 0,651 s | P (RNF-04) |
| 41 | 09:39:22–40:00 | `N2_Escena24_Regreso` L1–L8 | 8 líneas distintas: ALGORITM «Ahora tienen la carretilla. Pero tenerla no basta.», «Hay que pensar qué hacer primero y qué después.», PAPÁ «Empujar...», NIÑO «Girar...», MAMÁ «Evitar la piedra...», NIÑA «Si ordenamos bien los pasos... funcionará mejor.», ALGORITM «Lleva la carretilla hasta el refugio. Pero no la vas a empujar tú.», «Vas a escribir antes todos los pasos, y luego los ejecutas de una sola vez.»; luz de atardecer; sin «Omitir» (hoja_e24) | P (PF-RF05-02) |
| 42 | 09:40:27 | Último «Continuar» + `Esperar-Carga Level2_Maze` | **0,069 s; T0 = 09:40:27,96**. Laberinto en vista superior con cuadrícula dentro del seto, al atardecer; la carretilla en el hueco izquierdo mirando al este (la Niña afuera); arbustos; Papá, Mamá y el Niño en el hueco derecho (refugio, sin dibujo propio); a la derecha «Tu secuencia», «Suelta un bloque aquí», el cajón «Bloques» cerrado y «Ejecutar»; tablilla con la instrucción y ayuda (Algoritm de rueda) → `GP1/PF-RF30-01_laberinto_tablero.png` | P (RNF-04, PF-RF30-01, PF-RF31-01) |
| 43 | 09:41 | Transcripción del tablero (rejilla verificada sobre la foto → `GP1/PF-RF30-01_laberinto_rejilla.png`) | Ver «Tablero de GP1» abajo: 24 arbustos; (1,8) y (14,2) libres; la «L» directa bloqueada | OK |
| 44 | 09:42:04 | «Ejecutar» con la secuencia vacía | «Añade al menos un bloque a tu secuencia antes de ejecutar.»; la carretilla no se mueve (gp2_044) | P (PF-RF32-01) |
| 45 | 09:42:14 | Abrir el cajón | «Avanzar» (rectángulo con muesca y pestaña), «Girar» (píldora), «Retroceder» (más ancho, con un **círculo** asomando a la derecha; el código lo llama «rombo», OBS-10) y «Arrastra un bloque a tu secuencia»; con el cajón abierto la lista se encoge → `GP1/PF-RF31-01_cajon_bloques.png` | P (PF-RF31-01, PF-RNF19-01) |
| 46 | 09:42:44–43:12 | Arrastrar «Girar» a la secuencia; «‹»; arrastrar «Avanzar» debajo (×1) | «Girar» entra con «‹ ›» («›» por defecto) y papelera; tras «‹» el lado izquierdo queda en naranja y con contorno grueso; «Avanzar» entra con «− 1 +»; con dos bloques y el cajón abierto aparecen ▲/▼ y la barra → **fig-04b-4-laberinto-editor.png** (gp2_047) | P (PF-RF31-01) |
| 47 | 09:43:54 | «Ejecutar» (I 1), foto a +0,7 s y ráfaga de 3 s | La carretilla gira a la izquierda (mira al norte), intenta avanzar contra el seto y vuelve; queda resaltado «Avanzar» (contorno naranja); «La carretilla no llegó al refugio. Mira dónde se detuvo y corrige tu secuencia.» con alerta; en ≤ 1 s la carretilla está otra vez en la salida mirando al este y la secuencia sigue igual (`rafagas/gp2_ejec1_*`) | P (PF-RF32-01, PF-RF33-01, PF-RF34-01, PF-RNF19-01) |
| 48 | 09:44:29–33 | «Ejecutar» ×2 (I 2, I 3) | La 2.ª repite el mensaje; la 3.ª muestra en su lugar la pista «Mira en qué paso se detuvo la carretilla. ¿Hacia dónde miraba justo antes?», sin icono. **Desde aquí cuentan las ediciones** | P (PF-RF13-04, PF-RF34-01) |
| 49 | 09:45:06 | Botón de pista | Repite la instrucción «Lleva la carretilla hasta el refugio…» | P (PF-RF13-04) |
| 50 | 09:45:16 | Papelera de «Avanzar» (ed. 1) | Desaparece; queda «Girar» (comprimido, con «›») | P (PF-RF34-01) |
| 51 | 09:45:42–46:20 | ⚠ Clic sostenido sobre «Girar» y soltarlo sobre el tablero (ed. 2) | La secuencia queda vacía, **pero el bloque «Girar» se queda en pantalla pegado al cursor**: a +0,7 s y a +2 s sigue donde se soltó, y tras un clic en (0,30; 0,92) aparece allí, bajo el cursor → `GP1/DEF-GP1-01_a_*.png`, `_b_bloque_pegado_al_cursor.png`. El registro no muestra excepciones | **F (PF-RF34-02) — DEF-GP1-01** |
| 52 | 09:47:56 | Arrastrar «Avanzar» del cajón a la secuencia con el bloque pegado (ed. 3) | **Entra «Girar ‹» (el bloque pegado) y no «Avanzar»**: no se puede tomar otro bloque mientras hay uno pegado; el soltar sobre la lista engancha el pegado → `GP1/DEF-GP1-01_c_entra_girar_en_vez_de_avanzar.png`. El estado queda coherente (nada pegado), así que **no se usa «Reiniciar»**: el tablero se conserva y la cuenta sigue | F (DEF-GP1-01) |
| 53 | 09:48:22–49:02 | Arrastrar «Avanzar» (ed. 4) y otro «Avanzar» (ed. 5); «+» en el tercero (ed. 6); cerrar el cajón | Secuencia «Girar ‹», «Avanzar ×1», «Avanzar ×2» (gp2_061) | OK |
| 54 | 09:49:29–43 | ⚠ Tomar el tercero y soltarlo encima del primero (ed. 7) | **El bloque sale de la lista y se queda pegado al cursor** sobre «Girar»: la lista queda «Girar», «Avanzar ×1» → `GP1/DEF-GP1-01_d_reordenar_bloque_pegado.png`. Un **segundo clic** en el mismo punto lo engancha (ed. 8): «Avanzar ×2», «Girar ‹», «Avanzar ×1» → `GP1/DEF-GP1-01_e_reordenar_con_segundo_clic.png`. Reordenar solo funciona con dos gestos | **F (PF-RF34-02) — DEF-GP1-01** |
| 55 | 09:49:56–51:28 | Vaciar con la papelera (ed. 9, 10, 11) | Tras la 1.ª papelera, con el cajón cerrado y las filas desplegadas, **ningún bloque muestra papelera** (`_expanded = -1` y la flecha «→» solo existe en filas comprimidas); se abre el cajón para que las filas se compriman, «→» despliega una y su papelera la retira (abrir el cajón y «→» no cuentan). Secuencia vacía (gp2_070) | OK, con DEF-GP1-03 (leve) |
| 56 | 09:52:10–55:03 | Componer la secuencia ganadora sobre la rejilla (ed. 12–40): «Avanzar ×1», «Girar ›», «Avanzar ×7», «Girar ‹», «Avanzar ×9», «Avanzar ×4», «Girar ‹», «Avanzar ×1», «Girar ›», «Avanzar ×1» | 10 bloques (+10), 17 «+» y 2 «‹» → **40 ediciones** desde la 3.ª ejecución fallida. Cada «+» sube la cuenta de uno en uno hasta 9 (gp2_073, _077, _079); con la lista larga las filas se comprimen y solo la última va desplegada | OK |
| 57 | 09:55:14–31 | Cerrar el cajón; ▲ ×3 y ▼ ×3 (clic, sin arrastrar) | La lista sube hasta el primer bloque y vuelve al final; la barra acompaña (gp2_085, gp2_086 → `GP1/PF-RF34-01_lista_desplazada_con_botones.png`) | P (PF-RF34-01, PF-RNF02-01) |
| 58 | 09:55:42,95 | «Ejecutar» (foto a +1 s) y `Rafaga gp2_ganadora 26 1.5` | La carretilla recorre la secuencia casilla a casilla; **«Avanzar» tras «Girar ›» baja por la columna 1 (se mueve según la orientación)**; tras «Girar ‹» va al este por la fila 1, sube a (14,2) y entra al refugio por el este. Resaltado (contorno naranja y tamaño ×1,04) del bloque en curso: **los bloques 1 y 2 («Avanzar ×1», «Girar ›») se ejecutan fuera de la vista** —la lista quedó desplazada al final y no sigue al bloque en curso: a +1 s la carretilla gira en (1,8) y no hay ningún resaltado visible— y del bloque 3 («Avanzar ×7», ~4,7 s) solo asoman 4 px de su contorno en el borde superior; del bloque 4 en adelante el resaltado se ve entero → `GP1/DEF-GP1-02_a_*.png`, `_b_*.png`, `GP1/PF-RF32-01_bloque_en_curso_resaltado.png` | **F (PF-RF32-01) — DEF-GP1-02** |
| 59 | 09:56:03,98 | Llegada | «¡La carretilla llegó al refugio! Los pasos, en ese orden, funcionaron.» con acierto; la familia celebra; fundido (cuadro 29 de la ráfaga, negro) a `N2_Escena25_Cierre` (Narrative 0,703 s) → `GP1/PF-RF32-01_llego_al_refugio.png` | P |
| 60 | 09:56:03,98 | `Datos/OE4GP1.json` | Fase `{2, 3, attempts 3, correctedErrors 40, stepsUsed 10, resolutionSeconds 936,72}`; `reachedLevel` sigue en 2 (el desbloqueo va después del cierre reflexivo). Previsto: I 3 (ninguna ejecución fallida por mala lectura del tablero) · **C 40** (conteo de Claude, idéntico) · P 10 y T(llegada) − T0 = 09:56:03,98 − 09:40:27,96 = **936,02 s** (diferencia 0,7 s, la misma que en el tramo 1: T0 es la hora de la línea del registro) | P (PF-RF04-02) |

### Tablero de GP1 (`Level2_Maze`, 09:40:28; `#` seto, `S` salida, `R` refugio, `X` arbusto, `.` libre; fila 10 arriba)

```
     0123456789012345
10   ################
 9   #.....X........#
 8   S.....X.X......#
 7   #.....X.X......#
 6   #......XX......#
 5   #.....X.XX.....#
 4   #.....XXXX.....#
 3   #........X..XXX#
 2   #.XXXXX..X.....R
 1   #..............#
 0   ################
```

Camino previsto y ejecutado: (0,8) → «Avanzar ×1» (1,8) → «Girar ›» (sur) → «Avanzar ×7» (1,1) → «Girar ‹» (este) → «Avanzar ×9» (10,1) → «Avanzar ×4» (14,1) → «Girar ‹» (norte) → «Avanzar ×1» (14,2) → «Girar ›» (este) → «Avanzar ×1» (15,2) = refugio. 10 bloques.

## Cierre del Nivel 2

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 61 | 09:59:33–10:00:40 | `N2_Escena25_Cierre` L1–L6 | 6 líneas distintas, sin «Omitir», en la cueva de noche junto al fuego, con la carretilla cargada: «La familia está en su refugio alrededor del fuego, preparando alimentos.», «Cerca de ellos se observa la carretilla cargada.», ALGORITM «Las grandes ideas nacen cuando observas, comparas y organizas.», «Hoy hicieron girar algo más que una rueda: hicieron girar su pensamiento.», «Miraron muchas cosas y se quedaron solo con lo que importaba. Eso se llama abstraer.», «Y después ordenaron los pasos antes de dar el primero: eso es pensar como un algoritmo.» → `GP1/PF-RF12-02_abstraer.png`, `GP1/PF-RF12-02_pensar_como_un_algoritmo.png`. Algoritm con el color de madera del N2 (el del N1 es naranja y amarillo) | P (PF-RF12-02, PF-RF06-01, PF-RF05-02) |
| 62 | 10:00:54 | Último «Continuar» + `Esperar-Carga LevelSummary` | 0,035 s. «Esto es lo que pasó con la rueda», «Descubriste que lo redondo rueda y que una carretilla se arma en orden.», «Probaste objetos, piezas y caminos que no servían antes de dar con los buenos.», «Cuando algo no encajó, cambiaste de idea y volviste a intentar.», «Eso se llama abstraer y pensar como un algoritmo: quedarte con lo que importa y ordenar los pasos.»; **ningún dígito**; «Volver al menú de niveles» y «Continuar» → `GP1/PF-RF45-02_resumen_N2.png` | P (PF-RF45-02, PF-RF17-01) |
| 63 | 10:00:54,01 | `Datos/OE4GP1.json` | `reachedLevel` **3** (escrito al mostrarse el resumen, después del cierre reflexivo) y las cuatro fases intactas → `GP1/GP1_OE4GP1_tras_N2.json` | P (PF-RF04-02) |
| 64 | 10:01:09 | «Volver al menú de niveles» + `Esperar-Carga LevelSelect` | 0,068 s. N1 y N2 con visto y «Completado»; **«Nivel 3 · El Río» habilitado, sin candado ni «Bloqueado»**, con la ilustración del río → `GP1/PF-RF03-02_menu_tras_N2.png` | P (PF-RF03-02) |
| 65 | 10:01:36 | `Revisar-Log GP1` (con el juego abierto) | 23 líneas RNF-04, todas < 1 s (peor: Narrative 0,731 s, del tramo 1; en este tramo, 0,703 s); **sin Exception, Error ni Assert** → `GP1/GP1_tramo2_Player.txt` | P |
| 66 | 10:01:40 | `Medidas GP1` (parcial) | RNF-05: Private máx. **1136 MiB** (< 2048), CUMPLE. RNF-10: 2133 filas EXTERNA, las mismas tres conexiones TCP :443 a 34.8.90.77, 34.111.113.40 y 34.107.172.168 → **sigue DEF-SPIKE-01**. Copias parciales: `GP1/GP1_tramo2_{mem,red}.csv`, `GP1/GP1_tramo2_registro.tsv` | RNF-05 OK · RNF-10 F (DEF-SPIKE-01) |

## Indicadores del Nivel 2 (`Datos/OE4GP1.json`)

| Fase | Previsto (I · C · P · T) | JSON | Veredicto |
|---|---|---|---|
| N2F1 bosque | 3 · 1 · 0 · 207,45 s | 3 · 1 · 0 · 207,48 s | P |
| N2F2 taller | 5 · 3 · 6 · 217,93 s | 5 · 3 · 6 · 217,93 s | P |
| N2F3 laberinto | 3 · 40 · 10 · 936,02 s | 3 · 40 · 10 · 936,72 s | P (C incluye 3 ediciones causadas por DEF-GP1-01: el bloque tomado, el enganche involuntario y el segundo clic del reordenado; sin el defecto habrían sido 37) |

## Casos de S-N2A, S-N2B y S-N2C en GP1

| Caso | Veredicto | Nota |
|---|---|---|
| PF-RF05-02 | P | 7 narrativas en orden (3 + 9 + 7 + 7 + 8 + 8 + 6 líneas), una por clic |
| PF-RF06-01 | P | Primera visita del N2: ninguna narrativa ofrece «Omitir» |
| PF-RF10-02 | P | Objetivo como indicación paso a paso; la pista llega como pregunta |
| PF-RF22-01 · PF-RF23-01 · PF-RF24-01 | P | 14 objetos de cuatro categorías; mensaje propio por categoría; «n de 5» nunca baja |
| PF-RF25-01 · PF-RF26-01 | P | Caja: «faltan 5», «no toca los troncos», colocada → «Empujar»; la 2.2 la rueda (OBS-9) |
| PF-RF13-02 · PF-RF13-03 · PF-RF13-04 | P | Pista al 3.er rechazo o ejecución fallida seguidos; la ayuda repite la instrucción en las tres escenas |
| PF-RF11-02 | P | Toda acción evaluada responde en las fotos a +0,4–0,7 s (mensaje con icono, resaltado o movimiento) |
| PF-RF27-01 · PF-RF28-01 · PF-RF29-01 | P | Siete piezas; «Mecanizar» solo con tronco corto resaltado; fuera de orden se rechaza sin deshacer; soltar lejos no cuenta (INC-115); **e5 comprobada** |
| PF-RF30-01 · PF-RF31-01 · PF-RF33-01 | P | Vista superior al atardecer; bloques con cuenta y lado; choque: intenta, vuelve y no reinicia |
| **PF-RF32-01** | **F** | **DEF-GP1-02**: el bloque en curso no se ve cuando la lista está desplazada |
| PF-RF34-01 | P | La secuencia sigue tras el fallo; papelera; ▲/▼ desplazan; con DEF-GP1-03 (leve) |
| **PF-RF34-02** | **F** | **DEF-GP1-01**: retirar o reordenar arrastrando deja el bloque pegado al cursor |
| PF-RF12-02 · PF-RF45-02 · PF-RF03-02 | P | El cierre nombra abstraer y algoritmo; resumen sin dígitos; N3 desbloqueado |
| PF-RF04-02 | P | Las tres fases del N2 en el JSON con las cifras previstas |
| PF-RNF02-01 · PF-RNF04-01 · PF-RNF19-01 | P | Teclas sin efecto; 12 cargas del tramo < 1 s; alerta, acierto, pista y lado de «Girar» se distinguen por forma |
| PF-RNF05-01 (parcial) | P | 1136 MiB |
| PF-RNF10-01 (parcial) | F | DEF-SPIKE-01 (ya registrado en el spike y en el tramo 1) |

## Defectos hallados en este tramo

### DEF-GP1-01 · Un bloque tomado de la secuencia se queda pegado al cursor (PF-RF34-02) — severidad media

- **Reproducción (exe rc1, 1920×1080):** con al menos un bloque en «Tu secuencia», clic sostenido sobre él, mover y soltar (fuera del panel o sobre otro bloque).
- **Observado:** la fila sale de la lista (la edición se cuenta), pero el bloque **no se suelta**: sigue al cursor sin botón pulsado (`DEF-GP1-01_a`, `_b`). El siguiente clic sostenido sobre cualquier bloque, también del cajón, no toma ese bloque: al soltar **se engancha el bloque pegado** donde esté el cursor (`_c`: se pidió «Avanzar» y entró «Girar ‹»). Reordenar solo funciona con dos gestos (`_d`, `_e`). El registro no muestra excepciones.
- **Esperado (casos.md S-N2C 10–11):** soltar fuera retira el bloque; soltar encima de otro lo reordena, con un solo gesto.
- **Causa probable (lectura de código, sin corregir):** `CargoHandle` recibe el `OnPointerDown` en la fila; `MazeSceneController.TakeFromSequence` (≈ l. 952) llama a `RefreshRows`, que destruye las filas (≈ l. 717), entre ellas la que recibió el pulsar, así que el `EventSystem` ya no entrega el `OnPointerUp` y `Drop` nunca corre; `_held` queda vivo y `Update` lo mueve con el ratón. El cajón no falla porque su pieza no se destruye. La suite no lo ve porque las pruebas llaman a `TakeFromSequence` y `Drop` directamente.
- **Impacto:** no se pierde nada aprobado ni se reinicia el tablero, pero quien arrastra un bloque de la lista lo deja «colgando» del cursor y el siguiente intento mete un bloque que no pidió. El riesgo ya estaba marcado con ⚠ en `casos.md`.

### DEF-GP1-02 · El bloque en curso puede ejecutarse fuera de la vista (PF-RF32-01) — severidad media

- **Reproducción:** secuencia de 10 bloques (la lista se desplaza y queda al final tras añadir), cajón cerrado, «Ejecutar».
- **Observado:** `Highlight` solo pone contorno y escala; la lista no se desplaza hasta el bloque en curso. Los bloques 1 y 2 se ejecutan sin ningún resaltado visible (foto a +1 s, `DEF-GP1-02_a`) y el 3.º («Avanzar ×7», ~4,7 s) con solo 4 px de su contorno asomando arriba (`_b`); desde el 4.º se ve entero (`rafagas/gp2_ganadora_*`, `PF-RF32-01_rafaga_ganadora.csv`).
- **Esperado (casos.md S-N2C 14, «⚠ … es F de PF-RF32-01»):** en todo momento se ve resaltado el bloque en curso.

### DEF-GP1-03 · Tras usar la papelera, ningún bloque ofrece papelera (PF-RF34-01) — leve

- Con el cajón cerrado y pocos bloques (filas desplegadas, sin «→»), `DeleteBlock` deja `_expanded = -1`: ninguna fila lleva papelera y no hay «→» para elegir otra (`DEF-GP1-03_sin_papelera_tras_borrar.png`). Salidas: abrir el cajón (las filas se comprimen y aparece «→»), añadir un bloque o arrastrarlo fuera (que dispara DEF-GP1-01). PF-RF34-01 pasa con el rodeo.

## Observaciones (no son defectos del tramo)

- **OBS-8 · Tablilla del bosque llena.** La instrucción de tres líneas ocupa toda la tablilla: la primera línea toca el borde superior y la última el inferior (`OBS-8_tablilla_bosque_texto_al_borde.png`). Es legible; queda para el carril de interfaz.
- **OBS-9 · Primer cuadro de la 2.2.** Entre «Empujar» y la primera foto útil pasan el fundido (0,4 s × 2) y ~2 s de arranque del arnés, así que el cuadro de apertura antes del rodado no se capturó. En el primer cuadro de la ráfaga se comprobaron los troncos (mismos píxeles), la altura y el tamaño de la caja; la x de arranque se infiere. Para cerrarlo del todo: la captura de W3 o una mirada en la Hoja-HUM.
- **OBS-10 · «Retroceder» lleva un círculo, no un rombo.** El bloque «Retroceder» del cajón y de la lista asoma un **círculo** a la derecha; `casos.md` S-N2C 4, el `todo.md` del Slice 2 y el código (`Fondo/Rombo`) lo llaman «rombo». La forma sigue distinguiendo los tres bloques (RNF-19); la diferencia es solo de nombre en los documentos.

## Figuras del entregable OE3 tomadas en este tramo (`evidencias/figuras/`)

| Fig. | Archivo | Origen | Contenido |
|---|---|---|---|
| 4.4 | `fig-04b-1-bosque-rechazo.png` | gp2_010 | Bosque tras clic en una piedra: «Esta piedra tiene esquinas. Cuando la empujas, se traba.» con triángulo de alerta, la piedra en su sitio, un tronco ya en la casilla 1 del acopio («1 de 5») y la Niña con el puño arriba |
| 4.5 | `fig-04b-2-escena22-patron.png` | gp2_022_e22_L06 | `N2_Escena22_ElPatron`: los cinco troncos, la **caja llena** que dejó el bosque ya rodada junto a Papá, y ALGORITM (madera) «Acabas de encontrar un patrón.» |
| 4.6 | `fig-04b-3-taller-fuera-de-orden.png` | gp2_028 | Taller tras soltar la tabla sobre el tronco B antes del eje: «La tabla no tiene sobre qué apoyarse todavía.» con alerta, la tabla de vuelta en su sitio, la rueda A conservada, «Mecanizar» con candado y «Aún no» |
| 4.7 | `fig-04b-4-laberinto-editor.png` | gp2_047 | Laberinto con cuadrícula al atardecer, carretilla en la salida, familia en el refugio, cajón «Bloques» abierto y «Girar» en la secuencia con el lado izquierdo «‹» elegido (naranja y con contorno) |

## Entrega al tramo 3

- **Juego ABIERTO:** pid 40152, sesión del arnés `GP1`, lanzamiento 1, ventana sin marco de 1920×1080 en (0,0); muestreador pid 33824 vivo.
- **Pantalla actual:** «Elige un nivel» con `OE4GP1`; N1 y N2 «Completado», **N3 habilitado** (clic en (0,831; 0,606)).
- **Perfil:** `Datos/OE4GP1.json` con `reachedLevel` 3 y N1F1, N2F1, N2F2 y N2F3 (copia en `GP1/GP1_OE4GP1_tras_N2.json`).
- No se cerró el juego ni se restauró el registro. El Editor sigue cerrado.
- `coords.json` lleva ahora también las coordenadas medidas del bosque, el taller, el laberinto y el resumen.
