# S-N2C-rc2 · laberinto entero con `OE4_B3` (01/10/2026, 19:56–20:08)

- **Exe** rc2 (SHA-256 coincide), ventana sin marco 1920×1080 en el monitor único de 1920×1080 al 125 %, pid 47208. Sonido silenciado (reunión de Webex abierta; zona de su ventana tapada en las evidencias).
- **Pre:** `Preparar-Datos OE4_B3` (N2 fases 1 y 2 confirmadas). «Jugar» → «OE4_B3» → «Nivel 2» → 19 «Continuar» por tres narrativas → `Level2_Maze`.
- **Conteo de ediciones** según `casos.md` S-N2C (desde la última ejecución fallida): soltar +1, papelera +1, retirar arrastrando +1, reordenar +2, cada «+»/«−»/«‹»/«›» +1; no cuentan ▲/▼ ni el cajón.

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 19:57:46,9 | Carga de `Level2_Maze` (**T0**) | 0,088 s. Laberinto al atardecer con cuadrícula, carretilla en (0,8) mirando al este con la Niña afuera, familia en el refugio, «Tu secuencia», «Suelta un bloque aquí», cajón cerrado, «Ejecutar» | P (PF-RF30-01, RNF-04) |
| 2 | 19:58 | Foto y transcripción (`tablero.py`, luminancia del centro de cada casilla, revisada a ojo) | Tablero nuevo (abajo); (1,8) y (14,2) libres | OK |
| 3 | 19:58 | «Ejecutar» con la secuencia vacía | «Añade al menos un bloque a tu secuencia antes de ejecutar.»; la carretilla no se mueve | P (PF-RF32-01) |
| 4 | 19:58 | Abrir el cajón | «Avanzar», «Girar», «Retroceder», «Arrastra un bloque a tu secuencia» | P (PF-RF31-01) |
| 5 | 19:59 | «Girar» a la secuencia → «‹»; «Avanzar» debajo (×1) | Girar con ‹ en naranja; Avanzar ×1 con − 1 +; aparecen ▲/▼ | P (PF-RF31-01) |
| 6–7 | 19:59–20:00 | «Ejecutar» ×3 | 1.ª y 2.ª: la carretilla gira al norte, choca con el seto y vuelve; «La carretilla no llegó al refugio. Mira dónde se detuvo y corrige tu secuencia.» con alerta y el bloque «Avanzar» resaltado; la secuencia sigue igual. 3.ª: la pista «Mira en qué paso se detuvo la carretilla. ¿Hacia dónde miraba justo antes?» (I 3) (`PF-RF34-01_tres_fallos_y_pista.png`) | P (PF-RF32-01, PF-RF33-01, PF-RF13-04) |
| 8 | 20:00 | Botón de pista | Repite «Lleva la carretilla hasta el refugio…» | P (PF-RF13-04) |
| 9 | 20:00 | Papelera de «Avanzar» (seleccionado) | Desaparece; **«Girar» queda seleccionado con su papelera** (ed. 1) (`PF-RF34-01_papelera_avanzar.png`). Un primer clic cayó 50 px por encima de la papelera, porque tras el fallo el bloque en curso se resalta y se agranda y la lista se recoloca; no pulsó nada | P (PF-RF34-01) |
| 10 | 20:01 | **Pulsar «Girar» en la secuencia y soltarlo sobre el tablero** (un arrastre); clic suelto en otro sitio | La secuencia queda **vacía** y **nada sigue al cursor** (ed. 2) (`PF-RF34-02_retirar_fuera_sin_pegarse.png`) | **P (PF-RF34-02, retirar)** |
| 11a | 20:02 | Armar «Avanzar ×1», «Girar», «Avanzar» y «+» → ×2; cerrar el cajón | Avanzar 1 · Girar › · Avanzar 2 (ed. 6) | OK |
| 11b | 20:03 | **Tomar el 3.º y soltarlo sobre el 1.º (un solo gesto)** | **Avanzar ×2, Avanzar ×1, Girar** a la primera; nada pegado al cursor (ed. 8) (`PF-RF34-02_reordenado_en_un_gesto.png`) | **P (PF-RF34-02, reordenar; cierra DEF-GP1-01)** |
| 11c | 20:03 | Con el cajón **cerrado**, papelera ×3 en el mismo sitio (clics separados) | Tras cada papelera la fila que ocupa el lugar queda seleccionada con su papelera; al tercero, vacía y sin papelera (ed. 11) (`DEF-GP1-03_papelera_con_cajon_cerrado_x3.png`) | **P (cierra DEF-GP1-03)** |
| 12 | 20:04–20:05 | Secuencia planificada (abajo): 9 bloques, 9 soltados + 18 «+» + 2 «‹» (ed. 40) | Lista de 9 bloques; con el cajón cerrado las filas van comprimidas con «›» y la última desplegada (`PF-RF34-01_nueve_bloques_cajon_cerrado.png`) | OK |
| 13 | 20:05 | ▲ ×3 y ▼ ×4 | La lista sube hasta el primer bloque y vuelve al final, solo con clic (`PF-RNF02-01_lista_con_flechas.png`) | P (PF-RF34-01, PF-RNF02-01) |
| 14 | 20:05:44,968 | **«Ejecutar» con la lista abajo del todo** + ráfaga de 26 s a 2 fps (`combo.ps1`) | A +0 s la lista está al final; **a +0,5 s ya subió sola y el bloque 1 («Avanzar») se ve resaltado con contorno naranja; a +1 s el 2.º («Girar»)**; durante todo el recorrido el bloque en curso queda dentro de la vista y la lista baja con él; a +17 s el 9.º («Avanzar 6») resaltado abajo. **Llega al refugio a la primera** con «¡La carretilla llegó al refugio! Los pasos, en ese orden, funcionaron.» (`PF-RF32-01_ejecucion_2fps.png`, `DEF-GP1-02_bloque_en_curso_visible.png`) | **P (PF-RF32-01; cierra DEF-GP1-02)** |
| 15 | 20:06:04,6 | `Datos/OE4_B3.json` | Fase `{2, 3, attempts 3, correctedErrors 40, stepsUsed 9, resolutionSeconds 497,81}`, `reachedLevel` 2. **Previsto I 3 · C 40 · P 9 · T = 01:06:04,593Z − 00:57:46,897Z = 497,70 s** (Δ 0,1 s). C ya no trae ediciones fabricadas por el defecto (en GP1 eran 3 de 40) | **P (PF-RF04-02, RF-45)** |
| 16 | 20:06–20:07 | `N2_Escena25_Cierre` (6 líneas) | Sin «Omitir»; L1 «La familia está en su refugio alrededor del fuego, preparando alimentos.»; L6 ALGORITM «Y después ordenaron los pasos antes de dar el primero: eso es pensar como un algoritmo.» | P (PF-RF06-01, PF-RF12-02) |
| 17 | 20:07:10,9 | `LevelSummary` (0,035 s) | «Esto es lo que pasó con la rueda», «Descubriste que lo redondo rueda…», «Probaste objetos, piezas y caminos que no servían…», «Cuando algo no encajó, cambiaste de idea…», «Eso se llama abstraer y pensar como un algoritmo…»; **ningún dígito**. JSON: `reachedLevel` **3**, fases intactas (`PF-RF45-02_cierre_y_resumen_N2.png`) | P (PF-RF45-02, PF-RF04-02) |
| 18 | 20:07:33 | «Volver al menú de niveles» | N1 y N2 «Completado», **N3 habilitado** (`PF-RF03-02_N3_desbloqueado.png`) | P (PF-RF03-02) |
| 19 | 20:07 | `Cerrar-Juego`; `Revisar-Log`; `Medidas`; `Residuos`; `Restaurar-Registro` | Cierre normal. **Sin Exception, Error ni Assert.** Red: **323/323 muestras sin sockets**. Privada máx. 1093 MiB. LocalLow: solo `Player.log`. HKCU: los mismos 21 valores que en S-INI-rc2 (con los tres `unity_connect.*`, DEF-RC2-01), borrados | P · DEF-RC2-01 |

### Tablero (`Level2_Maze`, 19:58) · `evidencias/rc2/S-N2C-rc2_tablero.txt`

```
     0123456789012345
10   ################
 9   #.........X....#
 8   S.....X...X....#
 7   #....XX.....X..#
 6   #....XX.....X..#
 5   #....XX.....X..#
 4   #....X.X....X..#
 3   #....X.XX.XXXX.#
 2   #.......X......R
 1   #.......X......#
 0   ################
```

Secuencia ganadora (9 bloques): Avanzar ×1 → (1,8) · Girar ‹ (norte) · Avanzar ×1 → (1,9) · Girar › (este) ·
Avanzar ×8 → (9,9) · Girar › (sur) · Avanzar ×7 → (9,2) · Girar ‹ (este) · Avanzar ×6 → (15,2), entrando desde (14,2).

## Casos

| Caso | Veredicto | Nota |
|---|---|---|
| PF-RF30-01 · PF-RF31-01 · PF-RF33-01 · PF-RF13-04 | P | — |
| **PF-RF32-01** | **P** (antes F) | Vacía informada; con la lista abajo del todo, el bloque en curso sube a la vista desde +0,5 s y sigue a la vista hasta el final |
| **PF-RF34-01** | **P** (antes F) | Tras el fallo la secuencia sigue; papelera con el cajón abierto y cerrado, sin rodeo |
| **PF-RF34-02** | **P** (antes F) | Retirar soltando fuera y reordenar en **un** gesto, sin bloque pegado |
| PF-RF04-02 (N2F3) · RF-45 | P | 3 / 40 / 9 / 497,8 s, como lo previsto |
| PF-RF45-02 · PF-RF12-02 · PF-RF06-01 · PF-RF03-02 | P | — |
| PF-RNF10-01 · PF-RNF11-01 (LocalLow) | P | Sin red; HKCU con DEF-RC2-01 |

**Defectos:** ninguno nuevo del laberinto. DEF-GP1-01, DEF-GP1-02 y DEF-GP1-03 **cerrados sobre el exe**.

**Observación OBS-rc2-5 (para la Hoja-HUM):** tras una ejecución fallida el bloque donde se detuvo se resalta y se agranda y
la lista se recoloca, así que la papelera de ese bloque no queda donde estaba antes de ejecutar (aquí, 50 px más abajo).
No es defecto (el bloque se ve y su papelera también), pero un niño que apunta de memoria puede fallar el primer clic.
*(corr. 01/10/2026, 21:45, revisión final de rc2)* **Triaje:** la introduce la corrección de DEF-GP1-02. `Highlight` termina desde rc2 en `RevealRow` (`SequenceListRules.RevealScroll`), que mueve la lista para dejar a la vista el bloque en curso; tras el fallo queda en la posición que muestra el bloque donde se detuvo, distinta de la de antes de «Ejecutar». Severidad: observación (equivale a Trivial). Arreglo propuesto y decisión D-OBS en `OE4-Resultados.md` §8.9 y §8.10; va a la Hoja-HUM, H13.
