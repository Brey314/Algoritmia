# RNF-19: parejas en escala de grises (`arte_check.py rnf19`)

- Comando: `arte_check.py rnf19 --file parejas-rnf19.txt --out rnf19.md`
- Criterio: se distinguen si **1−IoU de la silueta ≥ 0,20** (forma) o **diferencia media de gris ≥ 25** de 255 (brillo). Gris Rec. 601 sobre gris medio; lienzo de 256×256 (con alfa, encajado; captura, estirada). «n/d»: sin transparencia en alguna de las dos, solo cuenta el gris. La columna «Δ ≥ 25» es informativa: un cambio pequeño y localizado (candado, visto) da diferencia media baja aunque se vea.

| Grupo | A | B | 1−IoU | Δ gris medio | Δ ≥ 25 | Veredicto |
|---|---|---|---|---|---|---|
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_piedra_a.png` | 0,54 | 50,5 | 59,3 % | OK (forma y gris) |
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_piedra_b.png` | 0,54 | 50,3 | 58,7 % | OK (forma y gris) |
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_piedra_c.png` | 0,54 | 50,5 | 59,3 % | OK (forma y gris) |
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_piedra_d.png` | 0,48 | 54,1 | 64,0 % | OK (forma y gris) |
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_planta_a.png` | 0,67 | 48,1 | 55,8 % | OK (forma y gris) |
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_planta_b.png` | 0,67 | 40,1 | 41,4 % | OK (forma y gris) |
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_planta_c.png` | 0,67 | 38,4 | 45,9 % | OK (forma y gris) |
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_herramienta_a.png` | 0,75 | 48,2 | 52,9 % | OK (forma y gris) |
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_herramienta_b.png` | 0,75 | 48,2 | 52,9 % | OK (forma y gris) |
| B2 válido/distractor | `prop_n2_tronco_a.png` | `prop_n2_herramienta_c.png` | 0,82 | 46,7 | 48,9 % | OK (forma y gris) |
| B2 categorías | `prop_n2_piedra_a.png` | `prop_n2_planta_a.png` | 0,58 | 75,8 | 88,9 % | OK (forma y gris) |
| B2 categorías | `prop_n2_piedra_a.png` | `prop_n2_herramienta_a.png` | 0,74 | 49,6 | 77,2 % | OK (forma y gris) |
| B2 categorías | `prop_n2_planta_a.png` | `prop_n2_herramienta_a.png` | 0,70 | 84,5 | 93,8 % | OK (forma y gris) |
| B2 en captura | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.5911,0.5944,0.6573,0.7056` | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.4260,0.6222,0.4677,0.7019` | n/d | 39,3 | 49,8 % | OK (gris) |
| B2 en captura | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.5911,0.5944,0.6573,0.7056` | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.7490,0.6259,0.7906,0.7074` | n/d | 52,2 | 61,8 % | OK (gris) |
| B2 en captura | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.5911,0.5944,0.6573,0.7056` | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.5198,0.8380,0.5740,0.8981` | n/d | 59,1 | 62,9 % | OK (gris) |
| B2 en captura | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.4260,0.6222,0.4677,0.7019` | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.7490,0.6259,0.7906,0.7074` | n/d | 37,8 | 44,8 % | OK (gris) |
| B2 en captura | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.4260,0.6222,0.4677,0.7019` | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.5198,0.8380,0.5740,0.8981` | n/d | 41,6 | 44,5 % | OK (gris) |
| B2 en captura | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.7490,0.6259,0.7906,0.7074` | `WheelLevel_RNF19_Bosque_ObjetoNoValido.png@0.5198,0.8380,0.5740,0.8981` | n/d | 32,3 | 43,5 % | OK (gris) |
| B8 seto interior/camino | `Maze_01_reposo.png@0.2177,0.4167,0.2479,0.6389` | `Maze_01_reposo.png@0.1589,0.4167,0.2083,0.6389` | n/d | 4,6 | 3,3 % | **FALLA** |
| B8 seto del borde/camino | `Maze_01_reposo.png@0.1562,0.1852,0.5208,0.2176` | `Maze_01_reposo.png@0.1589,0.4167,0.2083,0.6389` | n/d | 117,8 | 99,9 % | OK (gris) |
| B10 acierto/rechazo | `ui_circulo.png` | `ui_alerta.png` | 0,71 | 114,0 | 99,0 % | OK (forma y gris) |
| C4 materiales | `prop_n3_tronco.png` | `prop_n3_sogas.png` | 0,90 | 48,4 | 75,3 % | OK (forma y gris) |
| C4 materiales | `prop_n3_tronco.png` | `prop_n3_tela.png` | 0,70 | 60,3 | 78,6 % | OK (forma y gris) |
| C4 materiales | `prop_n3_tronco.png` | `prop_n3_mastil.png` | 0,89 | 51,4 | 83,1 % | OK (forma y gris) |
| C4 materiales | `prop_n3_sogas.png` | `prop_n3_tela.png` | 0,76 | 47,8 | 68,1 % | OK (forma y gris) |
| C4 materiales | `prop_n3_sogas.png` | `prop_n3_mastil.png` | 0,95 | 45,6 | 74,1 % | OK (forma y gris) |
| C4 materiales | `prop_n3_tela.png` | `prop_n3_mastil.png` | 0,82 | 54,8 | 75,4 % | OK (forma y gris) |
| C5 hecha/pendiente | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.0469,0.0815,0.0698,0.1204` | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.0469,0.1833,0.0698,0.2241` | n/d | 41,0 | 59,7 % | OK (gris) |
| C8 vacío/correcto (sprite tronco) | `prop_n3_tronco_silueta.png` | `prop_n3_tronco.png` | 0,09 | 63,1 | 86,1 % | OK (gris) |
| C8 vacío/correcto (sprite amarre) | `prop_n3_amarre_silueta.png` | `prop_n3_amarre.png` | 0,00 | 53,6 | 93,0 % | OK (gris) |
| C8 vacío/correcto (sprite mastil) | `prop_n3_mastil_silueta.png` | `prop_n3_mastil.png` | 0,11 | 64,5 | 92,7 % | OK (gris) |
| C8 vacío/correcto (sprite vela) | `prop_n3_vela_silueta.png` | `prop_n3_vela.png` | 0,09 | 36,2 | 66,5 % | OK (gris) |
| C8 vacío/correcto (casilla 5) | `AssemblyPanel_HU12_Base.png@0.4948,0.5324,0.6589,0.7685` | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.4948,0.5324,0.6589,0.7685` | n/d | 28,2 | 39,0 % | OK (gris) |
| C8 vacío/incorrecto (casilla 1) | `AssemblyPanel_HU12_Base.png@0.3385,0.3704,0.5078,0.6185` | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.3385,0.3704,0.5078,0.6185` | n/d | 4,3 | 4,5 % | **FALLA** |
| C8 incorrecto/correcto | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.3385,0.3704,0.5078,0.6185` | `RiverLevel_RNF19_ColocacionIncorrecta.png@0.4948,0.5324,0.6589,0.7685` | n/d | 45,3 | 71,7 % | OK (gris) |
| C9 cruzando/hundida | `prop_n3_balsa_cruzando.png` | `prop_n3_balsa_hundida.png` | 0,49 | 56,8 | 81,5 % | OK (forma y gris) |
| A7 montón/humo/fuego | `Props/Fire/prop_n1_monton_hojas_cenital.png` | `Props/Fire/Animations/Smoke/humo_nivel_1_0067.png` | 0,70 | 51,5 | 72,5 % | OK (forma y gris) |
| A7 montón/humo/fuego | `Props/Fire/prop_n1_monton_hojas_cenital.png` | `Props/Fire/Animations/Fuego cenital/fuego_cenital_nivel_1_0049.png` | 0,85 | 30,5 | 57,8 % | OK (forma y gris) |
| A7 montón/humo/fuego | `Props/Fire/Animations/Smoke/humo_nivel_1_0067.png` | `Props/Fire/Animations/Fuego cenital/fuego_cenital_nivel_1_0049.png` | 0,76 | 80,5 | 90,9 % | OK (forma y gris) |
| A9 soplar atenuado/habilitado | `FirePanel_RNF19_SoplarAtenuado.png@0.3281,0.8611,0.4922,0.9333` | `FireLevel_RNF21_ConvergenciaAMitad.png@0.3281,0.8611,0.4922,0.9333` | n/d | 4,4 | 2,0 % | **FALLA** |
| D1 indicadores | `ui_ind_intentos_boceto.png` | `ui_ind_errores_boceto.png` | 0,83 | 72,1 | 79,6 % | OK (forma y gris) |
| D1 indicadores | `ui_ind_intentos_boceto.png` | `ui_ind_pasos_boceto.png` | 0,93 | 84,2 | 93,3 % | OK (forma y gris) |
| D1 indicadores | `ui_ind_intentos_boceto.png` | `ui_ind_tiempo_boceto.png` | 0,89 | 80,2 | 88,8 % | OK (forma y gris) |
| D1 indicadores | `ui_ind_errores_boceto.png` | `ui_ind_pasos_boceto.png` | 0,94 | 80,3 | 88,6 % | OK (forma y gris) |
| D1 indicadores | `ui_ind_errores_boceto.png` | `ui_ind_tiempo_boceto.png` | 0,96 | 84,4 | 93,3 % | OK (forma y gris) |
| D1 indicadores | `ui_ind_pasos_boceto.png` | `ui_ind_tiempo_boceto.png` | 0,91 | 82,5 | 91,3 % | OK (forma y gris) |
| D3 papelera/alerta | `ui_papelera.png` | `ui_alerta.png` | 0,70 | 77,1 | 89,4 % | OK (forma y gris) |

**48 parejas · 45 se distinguen · 3 no cumplen el umbral.**

---

## Lectura (revisión W3, 01/10/2026; escrita a mano, no sale del script)

Las capturas son las vigentes del 01/10 (03:30–04:16). Hay **3 FALLA**, y ninguna es un fallo de arte:

- **B8 seto interior/camino (Δ 4,6):** la caja de W0 está desfasada. El laberinto sortea sus obstáculos en cada partida (`MazeGridTests`: «otra semilla, otro laberinto»), y la caja de W0 cae hoy sobre una franja de camino y otra de seto. Con la caja re-apuntada a una casilla de seto y a una de camino del `Maze_01_reposo` vigente da **Δ 85,0, OK** (`rnf19-w3.md`).
- **C8 vacío/incorrecto, casilla 1 (Δ 4,3):** también está desfasada, porque W2 corrió la balsa (x de 0,5 a 0,58, lectura B, INC-118) y la caja cae sobre la orilla. Re-apuntada da **Δ 24,8**, un pelo bajo el umbral, y eso es así por diseño: lo único que cambia es el icono de alerta sobre la silueta. **Lo cubre el icono**: el «!» contra la silueta da 5,6:1 (`rnf20-w3.md`), y lo prueba `AssemblyPanel_RNF19_ElEspacioIncorrectoSeResaltaConColorEIcono`. La fila «C8 vacío/correcto (casilla 5)» de arriba da OK de casualidad; la re-apuntada da Δ 47,2.
- **A9 Soplar atenuado/habilitado (Δ 4,4):** el cambio es localizado, solo el candado. **Lo cubre el icono**: candado 13,8:1 (`rnf20-extra.md`), y lo prueba `FirePanel_RNF19_*`.

**Resultado para el tablero:** 45 parejas se distinguen por forma o gris; las 2 de C8 y A9 se distinguen por el icono.
