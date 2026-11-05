# S-N3 · rc5 · resumen

- **Versión:** rc5, compilado desde `ca6fd91`. Carpeta `Build\Algoritmia_rc5\`, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3` (253 archivos). SHA-256 del `.exe` `6CD16EDF…ADAB`. El manifiesto coincidió antes de cada lanzamiento y al terminar (253 archivos, misma huella).
- **Fecha:** 05/11/2026 (hora local UTC−5). Partida principal `S-N3-rc5`, perfil `OE4_C`, 02:02–02:23, ventana de 1920×1080 sin marco (`-popupwindow`, el monitor mide 1920×1080). Dos repeticiones cortas con el perfil `OE4_E`, que reabre en mástil y vela: `S-N3b-rc5` (02:24–02:27: «Continuar» del resumen y el retrato animado) y `S-N3c-rc5` (02:29–02:32: los textos de piezas de INC-157).
- **Casos:** 25 filas (los 20 de la tabla de S-N3 más `PF-RF05-04`, `PF-RF05-05`, `PF-RF06-01`, `PF-RF10-04` y `PF-RF07-07`, que se alimentan también de otras sesiones y se juzgaron solo en mi parte). **Veredictos: P 25 · PD 0 · F 0 · B 0 · NA 0 · NE 0.**
- **Pasos 1–26 ejecutados**, sin atajos, hasta Credits y «Volver» al inicio. Todas las coordenadas de `casos.md` (orilla, cruceta, casillas, siluetas, «Listo», «Probar balsa») sirvieron tal cual en el plano fijo.

## Cifras: previstas frente a obtenidas (`S-N3_OE4_C.json`)
| Fase | Previsto I/C/P | Obtenido I/C/P | `resolutionSeconds` |
|---|---|---|---|
| N3F1 | 2/1/1 | 2/1/1 | 85,97 |
| N3F2 | 5/2/2 | 5/2/2 | 130,90 |
| N3F3 | 7/4/3 | 7/4/3 | 82,36 |

Sin diferencias. Los tiempos cuadran con las horas de las acciones (`acciones_S-N3.tsv`): T0 = entrada a la zona 07:11:28,8 UTC; base aprobada 07:12:54,8 (85,97 s); amarre aprobado 07:15:05,7 (130,90 s); prueba buena 07:17:26,8, que da 82,36 s una vez descontadas la 3.2 (~50 s) y la pausa (~10 s). La fase `{3,1}` ya estaba en el JSON al aprobar la base (escrita 07:12:55).

## Lo que se pidió mirar
- **Textos de INC-157.** «Armar la balsa» (tarea de la lista), «Troncos: guardado en tus materiales. Mira la lista: ¿qué falta?» (y la misma frase con «Sogas», «Mástil» y «Tela»), «Llegan a la otra orilla. El humo de las fogatas de otras familias se ve cerca, en el horizonte.» y «Eso es pensar como un algoritmo.» salieron tal cual. **«La pieza vuelve a tus materiales.» no se pudo provocar** con ninguno de los gestos probados (DEF-RC5-07); en cambio salieron «Cayó fuera de la balsa: la pieza vuelve a tus materiales.» y «Ese espacio ya tiene algo. Busca uno vacío.».
- **Paso 17.** La pausa en mástil y vela trae los cinco botones en orden, «Volver a ver la historia» incluido; «Reanudar» deja el ensamblaje igual (`PF-RF07-07`, mi parte). No pulsé ese botón.
- **`PF-RF45-03`.** Avatar de ALGORITM como gota con pantaloneta de cuadros; la tarjeta mide y 45–1043 de 1080 (999 px), entera, con los dos botones dentro; ningún dígito. «Volver al menú de niveles» lleva a `N3_EscenaFinal` (S-N3-rc5) y «Continuar» también (S-N3b-rc5). Dato: las capturas de S-N1 y de S-N2C también miden 45–1043; las de `rc5/DEF-RC4-01_N1.png` y `_N2.png` miden 76–1012 (937 px) y la de `_N3.png` 45–1043. La altura depende de cuántas viñetas salen, así que «la más alta de las tres» vale solo comparada con las figuras de DEF-RC4-01.
- **Escena final.** Siete líneas sin «Omitir» (la última, «Algoritm gira una última vez y se apaga suavemente. Todo queda en negro.»), Credits, «Volver» y de vuelta en el inicio.

## Lo que se vio de nuevo desde rc3
- Botón «Atrás» desde la línea 2 de cada secuencia del N3 y no en la línea 1; ningún «Omitir» en primera visita (datos para `PF-RF05-09` y `PF-RF06-01`).
- Retrato animado en toda línea con hablante: en una ráfaga de 45 cuadros (2,9 s) la boca alterna abierta y cerrada y hay un parpadeo de dos cuadros (`PF-RF05-04_N3_retrato_analisis.txt`). ALGORITM: disco de madera en `N3_PuenteII` y `Horizonte`, gota desde `N3_PuenteII_Rio`.
- Mamá de perfil al caminar y al recoger, de frente al parar, con corte seco; la cruceta no cambia. La 3.3: los cuatro a bordo hasta ~+0,5 s de la línea 4 y abajo caminando de perfil a ~+2,8 s (DEF-RC5-06).
- Al reabrir con `OE4_E` (N3F1 y N3F2 confirmadas): mástil y vela con base y amarres armados, tela y mástil en el inventario, la lista con tres tareas marcadas y «Coloca lo que atrapa el viento y pulsa «Probar balsa».» (dato para `PF-RNF14-02`).

## Defectos (al final de `evidencias/rc5/defectos.md`)
- `DEF-RC5-06` · Observación (podría ser Menor) · documento y juego: la foto «en la línea 4» del paso 21 no se puede tomar con el arnés y la balsa casi no se desplaza (no se ve una travesía del río; RF-44 pide «la animación automática del cruce del río»).
- `DEF-RC5-07` · Observación · juego (a confirmar) y documento: «La pieza vuelve a tus materiales.» no sale con ningún gesto probado y «Puesto…» sigue en pantalla cuando se retira una pieza.
- Ninguno Bloqueante ni Mayor.

## Medidas y avisos del arnés
- **RNF-04** (líneas `RNF-04` de los `Player.txt`, 31 en total): `Level3_River` 0,053–0,054 s (n=4); `LevelSummary` 0,035–0,036 s; `Credits` 0,036 s; `LevelSelect` 0,068–0,069 s; `MainMenu` 0,087–0,130 s; `Narrative` 0,035–0,602 s (las cargas con fundido —menú, mecánica o resumen hacia una narrativa— tardan 0,53–0,60 s; las encadenadas, 0,035 s). `Medidas` contó menos cargas que el log (5 y 4 frente a 14 y 10 líneas en las dos primeras sesiones), como se advirtió; no usé sus cifras.
- **RNF-05:** `S-N3-rc5` 616 muestras, de trabajo máx 458 MiB (`Level3_River`), privada máx 1.171 MiB (`Narrative`); `S-N3b-rc5` 432 / 1.151 MiB; `S-N3c-rc5` 434 / 1.163 MiB. Cumple frente a 2.048 MiB.
- **RNF-10:** 616 + 84 + 94 muestras, todas `NINGUNA`, ninguna conexión externa.
- Sin `Exception`, `Error` ni `Assert` en los tres `Player.txt`.
- **Salidas del arnés:** una salida 3 (`CURSOR: el cursor se movió a (440,838) en vez de (1491,993)`, no se pulsó nada; era el primer «Continuar» de `N3_PuenteII`, y como el bucle seguía conté las líneas desde las fotos). Todo lo demás, salida 0. Ningún aviso de contaminación (Unity y Word cerrados).
- **Aviso de entorno:** en la primera foto de la partida principal (`Start-Juego`) había encima del juego un cartel de «Chrome Remote Desktop» («En este momento estás compartiendo tu escritorio…», abajo en el centro, x 0,32–0,68 y y 0,92–0,95, justo donde están «Listo» y «Probar balsa») y en el segundo lanzamiento (`S-N3b-rc5`) una ventana de consola apareció ~2 s en el centro de la pantalla. Ninguno estaba ya en la siguiente foto ni bloqueó un clic, pero conviene vigilarlos en las sesiones siguientes. No toqué ninguno.
- **Residuos:** en las tres sesiones, `Player.log` nuevo en LocalLow y los mismos nueve valores del registro de Unity (resolución, modo de pantalla, frecuencia, contadores de sesión). La clave del registro ya existía antes de `S-N3-rc5`, así que `Restaurar-Registro` repuso solo los valores (no la borró). `Preparar-Datos -SinCarpeta` al final y el manifiesto sigue coincidiendo.
- **Captura independiente.** `Foto` y `Clic -ConFoto` llegan ≥ 0,64 s tarde, y `Rafaga` y `Sostener` se excluyen entre sí (mutex). Para ver el movimiento *durante* una acción (Mamá de perfil, el hundimiento de la balsa, el desembarco) lancé en segundo plano un script de PowerShell mío (fuera del repo, en el directorio temporal de la sesión) que copia la pantalla completa con `System.Drawing` a ~15 cuadros por segundo mientras el arnés hace la acción; la hora de cada cuadro queda en su `tiempos.txt`. Las fotos del arnés y esos cuadros coinciden en lo que muestran.

## Datos para casos de otras sesiones
- **`PF-RNF02-01`:** control negativo en la orilla del N3 (paso 3): Mamá no se movió (los pies en los mismos píxeles; solo cambia el gesto de reposo) y no se abrió la pausa con Esc: `PF-RNF02-01_3_antes_despues.png`, `PF-RF35-01_3_mama_antes_despues_recorte.png`.
- **`PF-RNF03-01`:** capturas a 1920×1080 por fase: orilla `PF-RF35-01_2_1.png`; zona sin todo `PF-RF39-01_7_1.png`; base `PF-RF39-01_10_1.png`; base rechazada `PF-RF43-01_12_1.png`; base firme `PF-RF41-01_13_2_base_firme.png`; amarre `PF-RF40-02_15_2_listo_amarre_vacio.png`; mástil y vela `PF-RF41-01_16_1.png`; pausa `PF-RF07-07_17_1_pausa_cinco_botones.png`; resumen `PF-RF45-03_22_1.png`; créditos `PF-RF08-01_24_creditos.png`; las narrativas, en hojas a media escala (`PF-RF05-04_*_hoja_N3_*.png`). La lista muestra siempre las cuatro tareas, con visto o sin él; no resalta cuál está en curso. Nada queda bajo el cuadro de diálogo en ninguna narrativa del N3.
- **`PF-RNF04-01` / `PF-RNF05-01` / `PF-RNF10-01`:** las cifras de arriba; `Level3_River` carga en 0,054 s.
- **`PF-RNF19-01`:** capturas en grises (`PF-RNF19-01_*_gris.png`): la lista del N3 (dos tareas marcadas, la marcada lleva visto y la pendiente un círculo liso), la alerta de la zona, los espacios marcados de la base, del amarre (tela y amarre vacío) y de mástil y vela. Se distinguen por la forma (triángulo con «!»), pero en el amarre el contorno de la alerta es gris medio (≈ 97 de 255, calculado del color de la alerta) sobre troncos de ≈ 48–82, es decir ≈ 1,3–2,1:1, el punto más flojo; lo juzga S-INSP.
- **`PF-RNF14-02` (S-PER):** reapertura de `OE4_E` en mástil y vela (`PF-RNF14-02_N3b_reapertura_mastil_y_vela_OE4_E.png`).
- **`PF-RF05-09` (S-N1):** en el N3, «Atrás» no sale en la línea 1 de ninguna secuencia y sí desde la 2.
- **`PF-RF07-01` (S-PAU):** en mástil y vela hay cinco botones; no miré la pausa del amarre (debería tener cuatro).
- **`PF-SON-03` (Santiago):** no se oyó nada. Quedan para él el hundimiento de la balsa (H7), los martillazos al aprobar una fase y el ambiente de la escena final (H6).

## Límites de lo que no se pudo ver
- Sin audio: el arnés no oye.
- «Se detiene al avanzar» del retrato no se midió (lo mide S-N2A); sí se midió que boca y parpadeo funcionan.
- `PF-RF41-01`: «ya no se puede tocar» solo se probó con la base; no se arrastró una soga del amarre aprobado ni la pieza de mástil y vela.
- La pausa del amarre (cuatro botones) y «Volver a ver la historia» no se probaron aquí (S-PAU).
- La expresión de cada línea se comparó solo a ojo con el contexto, no contra el guion de emociones.
- El cruce de la 3.3 se midió con 100 cuadros desde la línea 4 (6,6 s); no medí el desplazamiento de la balsa durante las líneas 5 a 7, porque el encuadre cambia con cada línea.
