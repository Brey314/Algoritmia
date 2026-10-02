# S-RNF-rc2 · tamaño, registros y consolidación de la reverificación sobre rc2 (01/10/2026, 20:35–20:40)

Juego cerrado. Solo lee lo que dejaron las sesiones de hoy sobre rc2: `S-INI-rc2`, `S-N2C-rc2`, `S-PER-rc2` (4 lanzamientos) y `GP3`.

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 20:35 | `Preparar-Datos -SinCarpeta` | `Build/Algoritmia/Datos` eliminada: la carpeta vuelve al estado del build (sin `Datos/`, como dice `build-rc2.md`). Los JSON de las sesiones están copiados en `evidencias/rc2/` | OK |
| 2 | 20:35 | `Tamano` | **478 987 619 B = 456,8 MiB = 479,0 MB** sin `*_DoNotShip` ni `Datos`; con `_DoNotShip` 457,5 MiB. Igual que a las 18:11 y que `build-rc2.md` | **P (PF-RNF06-01)** |
| 3 | 20:35 | Huella del contenido de `Algoritmia_Data` | `a613b851…c887a704`, la del rc2: las sesiones no tocaron el paquete | OK |
| 4 | 20:36 | `Revisar-Log` de las 7 ejecuciones (`logs/*_Player.txt`) | **0 líneas con Exception, Error ni Assert** en las siete (rc1: 1, la del perfil ilegible) | **P** |
| 5 | 20:36 | Cargas RNF-04 de las 7 ejecuciones | **74 cargas**, ninguna ≥ 1 s. Peor por escena: Narrative 0,780 s (n = 35) · Level1_Cave 0,154 (2) · MainMenu 0,104 (11) · Level2_Maze 0,088 (2) · Level2_Workshop 0,086 (1) · Level3_River 0,085 (3) · LevelSelect 0,081 (12) · TeacherReport 0,053 (1) · Level2_Forest 0,052 (1) · Credits 0,037 (1) · LevelSummary 0,035 (5) | **P (PF-RNF04-01)** |
| 6 | 20:36 | Memoria (`*_mem.csv`) | Privada máx. **1125 MiB** (GP3, Narrative); WS máx. 426 MiB (GP3). < 2048 MiB | **P (PF-RNF05-01)** |
| 7 | 20:36 | Red (`*_red.csv`) | **1312 muestras de 2 s** (325 + 323 + 181 + 483, ≈ 44 min de juego): **todas `NINGUNA`**: no se observó ninguna conexión TCP o UDP, ni siquiera de escucha local *(corr. 01/10/2026, 21:45, revisión final de rc2)*: el arranque de cada lanzamiento, de 2,7 a 8,1 s, queda sin muestrear. rc1: 3 TCP :443 a Google Cloud en 20/20 lanzamientos | **P (PF-RNF10-01; cierra DEF-SPIKE-01 en la red)** |
| 8 | 20:36 | Residuos (`residuos/*.txt`) | LocalLow: en las cuatro sesiones **solo `Player.log` nuevo**; ninguna entrada nueva o cambiada en `Unity\…\Analytics`, `Insights` ni `ShaderVariantAnalytics`. %TEMP%: nada del juego. HKCU: en las cuatro, la clave del reproductor con **tres `unity_connect.*`** (`installation_id`, `session_id`, `mega_session_id`) y **sin `unity.cloud_userid`** → **DEF-RC2-01**. `Restaurar-Registro` tras cada sesión: la clave de la empresa ya no existe | P (LocalLow) · **F parcial (HKCU) → DEF-RC2-01** |
| 9 | 20:37 | Estado final del equipo | Ningún `Algoritmia.exe`; Editor de Unity cerrado (no se abrió en toda la tarea: habría contaminado RNF-04 y RNF-05); ningún perfil en la ruta de respaldo de LocalLow; sonido del juego devuelto (el mezclador de Windows recuerda el último estado: con sonido) | OK |

## Resumen de la reverificación sobre rc2

| Punto de la tarea | Sesión | Veredicto |
|---|---|---|
| (1) Red y residuos (DEF-SPIKE-01), ≥ 2 min por menú, un nivel y una narrativa | S-INI-rc2 (10 min 48 s) y todas las demás | **Red: P (ninguna conexión observada). LocalLow: P. HKCU: F parcial** (sin `cloud_userid`, con `unity_connect.*` → **DEF-RC2-01**, Menor, resto de DEF-SPIKE-01). *(corr. 01/10/2026, 21:45, revisión final de rc2)* Decía «quitarlo pide módulos de `manifest.json`: D3, decide Santiago»; eso no está probado y hay opciones compatibles con D3. Decisión D-RC2-01: residuo del motor documentado para la entrega (`OE4-Resultados.md` §8.7 y §8.10) |
| (2) S-INI con 8 perfiles y PF-RF02-06 | S-INI-rc2 | **P**: 3 + 3 + 2, ▲/▼, ninguna fila tapada, la 8.ª se elige y se puede borrar. Cierra DEF-W5R-01 |
| (3) S-N2C entera | S-N2C-rc2 (y GP3) | **P**: retirar y reordenar en un gesto, nada pegado; la lista larga muestra el bloque en curso (dos tableros, 9 y 10 bloques); papelera con el cajón cerrado ×3. Cifras 3/40/9/497,8 s como lo previsto. Cierra DEF-GP1-01, -02 y -03 |
| (4) S-PER bloque D y cierre forzado al guardar | S-PER-rc2 | **P**: aviso sin Exception (cierra DEF-SPER-02); `CON` funciona; dos cierres forzados en el instante del guardado (entre la escritura del temporal y el reemplazo, y justo después del reemplazo) sin JSON truncado. *(corr. 01/10/2026, 21:45, revisión final de rc2)* Decía que uno fue «a mitad» y otro dentro del reemplazo: ninguna prueba cortó la escritura del `.tmp` ni alcanzó la ventana interna de `File.Replace` (R-RC2-A) |
| (5) S-N1 pasos 17–22 | S-INI-rc2 (con OE4_Z) | **P**: mandos atenuados al soplar (cierra DEF-W5R-02, con N6); doble «Soplar» a 85 ms, una sola ignición; ráfaga de la ignición sin destellos; duración del rayo medida a 20 fps |
| (6) Diálogo «Salir», informe con el nombre, escena final | S-INI-rc2, GP3 | **P · P · P**: cierra DEF-SPER-01, OBS-SDOC-2 y OBS-13 |
| (7) GP3 a pantalla completa con perfil nuevo | GP3 | **P**: 16 min 8 s, sin incidencias del juego (una del arnés, inocua), 7 fases con las cifras previstas |
| (8) Tamano y Revisar-Log | S-RNF-rc2 | **P**: 479,0 MB; 0 Exception en 7 ejecuciones; 74 cargas < 1 s |

**Defecto abierto:** DEF-RC2-01 (Menor, registro HKCU; resto de DEF-SPIKE-01, no un defecto nuevo *(corr. 01/10/2026, 21:45, revisión final de rc2)*). **Ningún defecto de rc1 reabierto.**
**Observaciones para la Hoja-HUM o el triaje:** OBS-rc2-2 (lista con `ScrollRect` en el informe docente, anterior a rc2; es la excepción del 29/09, decisión D-SCR), OBS-rc2-5
(la papelera del bloque en curso se desplaza tras un fallo), N4, N5 y N6 de la revisión de código, OBS-GP2-1.
**Condición de la prueba:** monitor único de 1920×1080 al 125 % (el de 2560×1440 que vio W5 ya no estaba conectado al empezar las
sesiones), así que **RNF-03 sigue sin probarse en otra resolución**. Rider abierto todo el tiempo (no lo cuenta el arnés).
