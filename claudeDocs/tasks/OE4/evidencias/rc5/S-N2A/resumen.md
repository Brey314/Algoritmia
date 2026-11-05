# S-N2A · rc5 · resumen

- **Versión:** rc5, compilado desde `ca6fd91` (`../../build-rc5.md`). Carpeta `Build\Algoritmia_rc5\`, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3` (253 archivos, `manifiesto-rc5.tsv`). SHA-256 del `.exe` (solo el lanzador) `6CD16EDF…ADAB`.
- **Fecha:** 04/11/2026. Partida principal de arnés `S-N2A-rc5` de 22:05 a 22:29 (hora local), una sola partida seguida que cubre S-N2A, S-N2B y S-N2C (misma sesión de arnés que en rc3; la evidencia de medidas, registro y residuos queda en esta carpeta). Segunda sesión `S-N2C2-rc5`, 22:30–22:35, solo para ver el mensaje de llegada al refugio (ver S-N2C). Ventana de 1920×1080 con marco (cliente en 1,31; monitor principal de 2560×1440).
- **Perfiles:** `OE4_B` (semilla `reachedLevel 2`, N1 fase 1 con 7 · 3 · 3 · 125 s) para la partida principal; `OE4_B3` (fases 1 y 2 del N2 confirmadas) para la repetición.
- **Casos:** 16 filas (los 11 de la tabla de S-N2A más `PF-RF05-04`, `PF-RF05-05`, `PF-RF05-06`, `PF-RF06-01` y `PF-RF10-04`, que se alimentan también de otras sesiones y se juzgaron solo en mi parte). **Veredictos: P 16 · PD 0 · F 0 · B 0 · NA 0 · NE 0.**
- **Pasos 1–15 ejecutados**, sin atajos. Salida 0 en todas las llamadas del arnés de esta sesión.

## Cifras: previstas frente a obtenidas (N2F1)
| | Previsto | Obtenido (`../S-N2C/S-N2_OE4_B.json`) |
|---|---|---|
| intentos · errores corregidos · pasos | 3 · 1 · 0 | 3 · 1 · 0 |
| `resolutionSeconds` | T(«Empujar») − T0 ± 5 s | 269,61 s (T0 = carga de `Level2_Forest` 03:07:29,8 UTC; «Empujar» 03:11:59,2; diferencia 269,4 s) |

El arrastre por suelo vacío junto a un tronco (paso 7, INC-155) y los cinco clics sobre troncos, uno de ellos junto a su extremo, **no alteraron las cifras previstas**.

## Lo que se pidió mirar
- **`PF-RF23-02` (troncos, INC-155).** Foto antes y a +1,0 s de soltar (acciones.tsv: foto a +1,03–1,19 s): el tronco se apartó 64 px (de ~120 px de ancho) y a +34 s estaba 73 px de su sitio, de modo que casi todo el movimiento ocurre en el primer segundo y luego se frena; no huye. Se alza un poco (+36 % de área) mientras el cursor está cerca y no se mueve ningún otro objeto (`PF-RF23-02_analisis.txt`). En el paso 9 el cuarto tronco se pulsó a unos 10 px de su extremo derecho y lo recogió (`PF-RF23-02_9_4_extremo.png`).
- **DEF-RC3-10 (tablilla del bosque).** La instrucción «Selecciona los objetos que se muevan con facilidad y únelos. Cuando tengas cinco, veremos qué pasa.» ocupa ahora **dos líneas** (la tablilla mide ~1100 px de ancho) y queda entera dentro de su recuadro; la pista de dos líneas también. Captura a pantalla completa: `PF-RNF03-01_2_bosque_tablilla.png` (es la misma de `PF-RF22-01_2_1.png`).
- **`PF-RF45-02` y `PF-RF31-02`:** en S-N2C.
- **`PF-RF10-04` (ALGORITM).** Disco de madera con pantaloneta de cuadros, de frente, brazos a los lados y detrás del cuerpo, manos lejos de la cara (`PF-RF10-04_1_algoritm_zoom.png`); en `N2_PuenteI` sigue siendo la llama (DEF-RC3-08, intencional).
- **¿ALGORITM formula el objetivo con alguna pregunta (RF-10)?** No: el objetivo lo dice como indicación paso a paso («Selecciona los objetos que se muevan con facilidad y únelos…»). Las preguntas de la escena las hacen la Niña («¿Y si el problema no es la fuerza… sino la forma?»), Mamá («Miren… algunas cosas se mueven más fácil que otras.», «¿Qué diferencia hay?») y la pista del juego.

## Lo que se vio
- **Narrativas del bosque:** `N2_PuenteI` 3 líneas, `N2_PuenteI_Bosque` 9, `N2_Escena21_Bosque` 7 y, tras «Empujar», `N2_Escena22_ElPatron` 7; sin «Omitir» ni pausa; «Atrás» desde la 2.ª línea de cada secuencia (hojas `PF-RF05-04_1_hoja_*.png`, una foto por línea).
- **Bosque:** 14 objetos (5 troncos, 3 piedras, 3 plantas, 3 herramientas); mensajes exactos de piedra, planta, herramienta (pista a la tercera), ayuda, caja antes de los cinco troncos, cinco troncos, caja lejos y sobre los troncos. Con el quinto tronco la cámara se acerca y «Empujar» sale con candado; en grises el candado se distingue (`PF-RNF19-01_*_gris.png`).
- **Escena 2.2:** la Niña empuja de perfil, con los dos brazos hacia delante, mientras la caja rueda sobre los troncos; los demás, de frente; sombras bajo personajes, troncos y caja.
- **Retrato animado (`PF-RF05-04`):** medido en ALGORITM en la línea 7 de la 2.2 (ráfaga de 6 s a 10 fps): la boca cambia 299 px por cuadro de forma continua y hay dos parpadeos de 1.495 px (`PF-RF05-04_N2A_retrato_l7.csv`); los demás hablantes, en foto fija. El texto no cambia durante la ráfaga.
- **Control negativo (paso 3):** `Esc Enter Espacio Arriba Abajo Izquierda Derecha W A S D Rueda` no cambian la mecánica: contador en «0 de 5», ningún objeto movido. La diferencia con la foto anterior (21.202 px) está en la familia que respira y en la piedra de abajo a la derecha, que vuelve a su tamaño al soltar el cursor: los objetos se agrandan cuando el cursor está cerca (también se ve al pulsar piedra y herramienta), no es efecto de las teclas (fotos en reposo: solo la familia cambia).
- **Icono de acierto:** es un disco verde liso, sin marca interior, igual que en rc3; el de alerta es un triángulo con «!»; se distinguen por forma en grises.

## Medidas de la sesión (aportan a otros casos; aquí no se juzgan)
- **RNF-04** (`PF-RNF04-01`): `Level2_Forest` 0,020 · `Level2_Workshop` 0,025 · `Level2_Maze` 0,027 · `LevelSummary` 0,014 · `Narrative` 0,013 a 0,207 s (la más lenta, la primera tras «Continuar» de una pantalla de flujo) · `MainMenu` 0,118 · `LevelSelect` 0,023 y 0,026 s; 15 líneas en `S-N2A-rc5_Player.txt`, 7 en `S-N2C2-rc5_Player.txt` (`../S-N2C/`); peor valor 0,207 s. `cargas_oe4.csv` trae solo las que se esperaron con `Esperar-Carga`.
- **RNF-05** (`PF-RNF05-01`): 714 muestras: memoria de trabajo máx 469 MiB y privada máx 1.184 MiB (`Narrative`); por escena, privada máx `Level2_Forest` 1.145, `Level2_Workshop` 1.156, `Level2_Maze` 1.155, `LevelSummary` 1.123 MiB. Segunda sesión: 136 muestras, 438 y 1.165 MiB. Frente a rc3 (privada máx 1.081 MiB en esta sesión) sube ≈ 10 %; sin juicio.
- **RNF-10** (`PF-RNF10-01`): 714 + 136 muestras, todas `NINGUNA`. Límite del arnés: no mira los primeros ≈ 3 s de cada arranque.
- **RNF-11 / registro:** `Residuos` halló `Player.log` nuevo en LocalLow y, en `HKCU\Software\Universidad Catolica de Colombia\Algoritmia`, ocho valores cambiados (`Screenmanager Resolution Width/Height/Use Native/Fullscreen mode`, `unity.player_session_count`, `unity.player_sessionid`, `unity_connect.mega_session_id`, `unity_connect.session_id`); la clave ya existía. `Restaurar-Registro` los repuso y `Residuos` dijo «existía y no cambió» (`residuos.txt`). La segunda sesión tomó su propia foto previa tras restaurar.
- **Registro del juego:** sin `Exception`, `Error` ni `Assert` en los dos `Player.txt`.

## Datos para casos de otras sesiones
- `PF-RNF02-01`: el control negativo del bosque (arriba). Rueda y arrastre sobre la lista del laberinto no se probaron (el guion no los pide).
- `PF-RNF03-01`: capturas a pantalla completa de cada fase: bosque `PF-RNF03-01_2_bosque_tablilla.png`, taller `../S-N2B/PF-RNF03-01_1_taller.png`, laberinto `../S-N2C/PF-RNF03-01_1_laberinto.png`. Dato: con la cámara acercada (pasos 10 a 12) Mamá queda con su cuerpo parcialmente detrás del borde derecho de la tablilla (`PF-RF26-01_10_1.png`); el texto no la nombra. La caja soltada lejos (paso 11, la soltó mi arrastre) quedó detrás del contador «Troncos redondos: 5 de 5» (`PF-RF25-01_11_1.png`).
- `PF-RNF19-01`: en grises, los rechazos del bosque (piedra, planta, «Ahí la caja no toca…»), «Empujar» con candado (`PF-RNF19-01_10_1_gris.png`, `_5_1_`, `_6_1_`, `_11_1_`); taller y laberinto en sus carpetas.
- `PF-RNF04-01`, `PF-RNF05-01`, `PF-RNF10-01`: las cifras de arriba.

## Defectos
Ninguno funcional en esta parte. Del conjunto de la partida: `DEF-RC5-04` (documento) y `DEF-RC5-05` (juego, observación), ambos en S-N2C, en `../defectos.md`.

## Avisos del arnés y límites
- `Start-Juego` no avisó de contaminación (Unity y Word cerrados). El manifiesto coincidió antes de los dos lanzamientos y coincide al terminar (253 archivos, misma huella).
- Una salida 3 en todo el trabajo, en S-N2B (ver allí): «se perdió el primer plano durante el arrastre»; el juego recibió la suelta.
- Un indicador verde «G-SYNC» de NVIDIA aparece fijo en la esquina superior derecha de las capturas (entorno, no del juego).
- Las fotos «a +x s» salen entre 0,64 y 0,94 s después del final de la acción incluso con `-FotoMs 600`; por eso «+1 s» de `PF-RF23-02` se logró con `-FotoMs 1000` (+1,03–1,19 s).
- La temporización de «Empujar» → fundido → narrativa se vio por fotos, no con cronómetro; los pasos 11 y 12 usaron coordenadas leídas de la captura del paso 10.
- Peso de las tres carpetas de evidencia: ≈ 120 MB.
