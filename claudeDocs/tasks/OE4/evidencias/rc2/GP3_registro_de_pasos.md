# GP3 · Golden Path a pantalla completa sobre rc2 (PF-RNF13-01, tercera vez) — 01/10/2026, 20:17:53–20:34:01

- **Recorrido:** perfil nuevo `OE4GP3`, de la pantalla de inicio a los créditos y de vuelta al inicio (N1, N2 con sus tres fases, N3 con la recolección y sus tres fases, escena final), **un solo proceso a pantalla completa** (`Start-Juego GP3 -Completa`), un solo agente, sin errores a propósito.
- **Exe:** rc2 (SHA-256 `6CD16EDF…ADAB` coincide; contenido `a613b851…`). Editor cerrado. **Pantalla:** monitor único de 1920×1080 al 125 % (el de 2560×1440 ya no estaba conectado), así que la pantalla completa es 1920×1080, como GP2.
- **Datos:** `Preparar-Datos` sin perfiles. Sonido silenciado en el mezclador durante el recorrido y devuelto antes de cerrar (el equipo tenía una reunión de Webex abierta hasta ~20:12).
- **Fotos:** las de esta sesión llevan un rectángulo gris arriba a la derecha (x ≥ 1808, y 128–434): es la máscara de la ventana de Webex, que `tapar.py` aplicó a todas las capturas de la carpeta aunque en GP3 la ventana ya no estaba. Tapa media burbuja del botón de ayuda en el N3; no afecta a ningún veredicto.

| # | Hora (UTC) | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 01:17:53,251 | `Start-Juego GP3 -Completa`; `Esperar-Carga MainMenu` | Pantalla completa 1920×1080 en (0,0); MainMenu 0,094 s | P (RNF-04) |
| 2 | 01:18 | «Jugar» → campo → `Escribir OE4GP3` → «Continuar» | Narrative 0,779 s (la peor carga del recorrido); `OE4GP3.json` = `{"name":"OE4GP3","reachedLevel":1,"phases":[]}`; `N1_Apertura` **sin «Omitir»** | P (PF-RF02-01, PF-RF06-01) |
| 3 | 01:18–01:19:46 | 37 «Continuar» por `N1_Apertura`, `N1_AparicionGuia` y `N1_Hallazgo` | Una línea por clic, sin «Omitir»; `Level1_Cave` 0,154 s (**T0 = 01:19:46,710**) | P (PF-RF05-01) |
| 4 | 01:20 | 7 piezas al centro; fuerza 7, cercanía 5; «Golpear» ×3; «Soplar» (01:20:44,0) | Panel con las piedras sobre el montón; «Soplar» sin candado al 3.º; nace la llama; `N1_NacimientoDelFuego` sin «Omitir» (0,386 s), 17 líneas | P (PF-RF14-01, PF-RF16-01, PF-RF19-01, PF-RF20-01) |
| 5 | 01:21 | `LevelSummary` del N1 | Variante sin errores, sin dígitos. JSON `{1,1: 0/0/3/61,10}`; previsto 0 · 0 · 3 · (01:20:44,0 + 3,5 s) − T0 ≈ 60,8 s | P (PF-RF45-01, PF-RF04-01) |
| 6 | 01:21–01:22:18 | «Continuar» → menú (N2 habilitado, N3 bloqueado) → «Nivel 2» → 20 «Continuar» (`N2_PuenteI`, `_Bosque`, `N2_Escena21`) | `Level2_Forest` 0,052 s (**T0 = 01:22:18,909**) | P (PF-RF03-02) |
| 7 | 01:22–01:23:35 | 5 troncos (sin rechazos) → caja sobre los troncos → «Empujar» (01:23:35,5) | «Este rueda…»; 5 de 5; caja sobre los troncos; «Empujar» sin candado. JSON `{2,1: 0/0/0/76,83}`; previsto 76,5 s | P (PF-RF22..26, PF-RF04-02) |
| 8 | 01:23–01:24:06 | 2.2 y 2.3 (16 «Continuar») | `Level2_Workshop` 0,086 s (**T0 = 01:24:06,729**) | P |
| 9 | 01:24–01:25:09 | Troncos A y B + «Mecanizar»; eje; tabla, caja y cuerda | «Segunda rueda lista…», «…ya hay un eje.»; carretilla completa. JSON (escrito 01:25:09,109) `{2,2: 0/0/6/62,76}`; previsto 62,38 s | P (PF-RF27..29, PF-RF04-02) |
| 10 | 01:25:29,368 | 2.4 (9 «Continuar») → `Level2_Maze` 0,071 s (**T0**) | **Incidencia del arnés:** el 9.º «Continuar» cayó sobre «Ejecutar» del laberinto recién cargado → «Añade al menos un bloque…». No suma intento (JSON: attempts 0) | OK (no es del juego) |
| 11 | 01:25–01:26 | Tablero (`GP3_tablero.txt`) y 10 bloques solo desde el cajón: A1 · G› · A7 · G‹ · A9 · A4 · G‹ · A1 · G› · A1; cerrar el cajón; ▲ ×4 y ▼ ×5 | Otro tablero que el de S-N2C; la lista de 10 se recorre con las flechas (`GP3_laberinto_10_bloques.png`) | P |
| 12 | 01:26:53,6 | «Ejecutar» con la lista abajo + ráfaga a 2 fps | **Desde el primer cuadro la lista está arriba con el bloque 1 resaltado**; el resaltado baja con la lista hasta el último (`GP3_DEF-GP1-02_bloque_en_curso.png`). Llega a la primera. JSON (01:27:13,495) `{2,3: 0/0/10/104,14}`; previsto 104,13 s | **P (PF-RF32-01, 2.ª vez con lista larga)** |
| 13 | 01:27–01:28 | 2.5 (6 «Continuar», sin «Omitir») → resumen → «Continuar» → menú | Resumen sin errores ni dígitos; `reachedLevel` 3; N3 habilitado | P (PF-RF45-02, PF-RF03-02) |
| 14 | 01:28 | «Nivel 3» → 19 «Continuar» por `N3_PuenteII`, `_Horizonte`, `_Rio`, `N3_Escena31_Llegada` | `Level3_River` 0,052 s: orilla con la lista de 4 tareas, inventario vacío, flechas | P |
| 15 | 01:28–01:30 | Recolección con la ruta de `coords.json` (`Sostener` ↓300 · →800 ↑300 · ↓450 · ←280 · ←260 ↓200 · ↑620 · ←330 ↑220 · ↑260 →300 · →280) y «Recoger» tras cada llegada | Al pasar por la zona incompleta: «Para armar la balsa todavía falta: Troncos, Sogas, Tela y Mástil. Sigue buscando por la orilla.» (no se persiste). Las 8 piezas; «Recoger troncos» y «Encontrar sogas» marcadas; «Ya tienes todos los materiales…» | P (PF-RF35..38) |
| 16 | 01:30:35,64 | ↓720 →290 ↑120 a la zona (**T0 N3F1**) | «Tienes todo lo de la lista. Aquí se arma la balsa.»; 5 siluetas, «Listo» y «Probar balsa» | P (PF-RF39-01, PF-RF40-01) |
| 17 | 01:31:21,6 | 5 troncos → «Listo» | «La base quedó firme…». JSON `{3,1: 0/0/1/46,12}`; previsto ≈ 45,96 s | P |
| 18 | 01:31:53,8 | 10 sogas → «Listo» | «La balsa ya no se abre…»; «Ensamblar la balsa» marcada. JSON `{3,2: 0/0/2/32,21}`; previsto 32,2 s | P |
| 19 | 01:32:11,05 | Mástil y vela → «Probar balsa» | «La balsa flota derecha. ¡A cruzar!»; directo a `N3_Escena33_Cruce` (sin 3.2, sin fallo). JSON `{3,3: 0/0/3/17,23}`; previsto 17,25 s | P (PF-RF44-01, PF-RF41-01) |
| 20 | 01:32 | 3.3 a ritmo de lectura (7 × 2 s) | L1 «La familia sube a la balsa corregida…» (OBS-GP2-1 sigue: da por hecho un fallo que no hubo); L7 «Eso es pensar computacionalmente.» con la balsa en la otra orilla | P (con OBS-GP2-1, decisión de Santiago) |
| 21 | 01:33 | Resumen del N3 → «Continuar» | «Esto es lo que pasó en el río», variante sin errores, sin dígitos → `N3_EscenaFinal` (0,718 s), sin «Omitir» | P (PF-RF45-03) |
| 22 | 01:33 | **Escena final, foto en cada una de las 7 líneas** | **La fogata central ya no sale detrás de la cabeza de la Niña en ninguna línea**: queda a la derecha del Niño. En L6 (brazos en alto) la mano del Niño queda cerca de las hojas de la fogata, se lee «al lado» (N10 de la revisión) (`GP3_resumen_N3_y_escena_final.png`, `OBS-13_fogata_junto_al_Nino_L5_L6.png`) | **P (cierra OBS-13)** |
| 23 | 01:33:4x | Último «Continuar» → `Credits` (0,037 s) → «Volver» → `MainMenu` (0,052 s) | Créditos con la rejilla y Algoritm de fuego; inicio | P (PF-RF08-01) |
| 24 | 01:34:01,371 | `Cerrar-Juego` | Cierre normal (WM_CLOSE) | OK |
| 25 | 01:34 | `Revisar-Log GP3` | **31 cargas RNF-04, todas < 1 s** (peor: Narrative 0,779 s tras crear el perfil); **sin Exception, Error ni Assert** | P (PF-RNF04-01) |
| 26 | 01:34 | `Medidas GP3` | 483 muestras: **ninguna conexión** (483/483 sin sockets). WS máx. 426 MiB, privada máx. 1125 MiB (Narrative) | P (RNF-10, RNF-05) |
| 27 | 01:34 | `Residuos GP3 -Buscar OE4GP3`; `Restaurar-Registro` | LocalLow: solo `Player.log`; «OE4GP3» solo en `Datos/OE4GP3.json`; HKCU con 19 valores, entre ellos los tres `unity_connect.*` (DEF-RC2-01), borrados. *(corr. 01/10/2026, 21:45, revisión final de rc2)* `GP3_residuos.txt` no trae la salida de `-Buscar OE4GP3`: lo de «solo en `Datos/OE4GP3.json`» consta solo en este paso | P · DEF-RC2-01 |

## Duración e indicadores

**Total, de `Start-Juego` a `Cerrar-Juego`: 01:17:53,251Z → 01:34:01,371Z = 16 min 8,1 s** (20:17:53 → 20:34:01, hora de Bogotá).
Un solo proceso (pid 18436), sin relanzar ni relevos. GP2 (rc1) tardó 32 min 57 s con el mismo camino sin errores:
la diferencia es del arnés (bucles de «Continuar» sin foto, sin transcripción manual del tablero y sin fotos de
control), no del juego. **No mide a un estudiante** (Hoja-HUM, H8).

| Fase | JSON (I · C · P · T) | Previsto | Veredicto |
|---|---|---|---|
| N1F1 | 0 · 0 · 3 · 61,10 s | 0 · 0 · 3 · ≈ 60,8 s | P |
| N2F1 | 0 · 0 · 0 · 76,83 s | 76,5 s | P |
| N2F2 | 0 · 0 · 6 · 62,76 s | 62,38 s | P |
| N2F3 | 0 · 0 · 10 · 104,14 s | 104,13 s | P |
| N3F1 | 0 · 0 · 1 · 46,12 s | ≈ 45,96 s | P |
| N3F2 | 0 · 0 · 2 · 32,21 s | 32,2 s | P |
| N3F3 | 0 · 0 · 3 · 17,23 s | 17,25 s | P |

Suma de los tiempos de resolución: 400,4 s (6 min 40 s). JSON final: `evidencias/rc2/GP3_OE4GP3_final.json`.

## Veredicto

**PF-RNF13-01 (GP3, pantalla completa, rc2): P.** Inicio → perfil nuevo → N1 → N2 → N3 → escena final → créditos → inicio en un
solo proceso, **sin bloqueos, cierres inesperados ni estados irrecuperables**, sin Exception en el log, las siete fases en el JSON
con las cifras previstas, **sin ninguna conexión de red** y sin residuos en LocalLow salvo el `Player.log`.

**Incidencias:** ninguna del juego. Del arnés, una: un «Continuar» de más cayó en «Ejecutar» con la secuencia vacía (no suma).
**Defectos abiertos que tocan el recorrido:** DEF-RC2-01 (registro). **Observaciones:** OBS-GP2-1 (3.3 «corregida») sigue; las demás
de rc1 (OBS-5, -6, -8, -12, -15) no se revisaron una por una en este recorrido: rc2 no tocó esas pantallas.
