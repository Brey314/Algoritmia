# S-OMI · rc5 · resumen

- **Versión:** rc5, compilado desde `ca6fd91` (`../../build-rc5.md`). Carpeta `Build\Algoritmia_rc5\`, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3` (253 archivos, `manifiesto-rc5.tsv`). SHA-256 del `.exe` (solo el lanzador) `6CD16EDF…ADAB`.
- **Fecha y horas (hora local, 05/11/2026):** `Preparar-Datos OE4_Z` 03:24:21; `Start-Juego S-OMI-rc5` 03:24:23 (un solo lanzamiento, pid 41272); última acción antes de la interrupción 03:37:22; primera foto tras reanudar 07:02:04; `Cerrar-Juego` 07:09:19. Ejecutor EXE con `oe4.ps1`; ventana de 1920×1080 **sin marco** (el equipo tiene ahora un solo monitor de 1920×1080, así que el arnés lanzó con `-popupwindow`; en S-INI y S-N1 el monitor principal era de 2560×1440 y el cliente llevaba marco).
- **Perfil:** `OE4_Z` (semilla, los tres niveles terminados), sin crear perfiles. Cinco entradas al N1 y una al N3.
- **Casos:** 8 (los de la tabla de S-OMI). **Veredictos: P 7 · PD 0 · F 1 · B 0 · NA 0 · NE 0.** El F es `PF-RF21-02` (DEF-RC5-10, Menor). Detalle en `veredictos.csv`.
- **Casos que se alimentan también de otra sesión y se juzgaron solo en mi parte:** `PF-RF20-03` (la parte de S-N1 paso 21 la juzga S-N1; aquí solo S-OMI paso 4).

## Interrupción
- Un límite de uso detuvo la sesión desde ≈ 03:37 hasta ≈ 07:02 (3 h 24 min 42 s entre mi última acción registrada, 03:37:22 en `acciones.tsv`, y la primera tras reanudar, 07:02:04). El juego quedó abierto y no se relanzó.
- **Qué estaba en curso:** ninguna acción. La última llamada fue la `Foto omi_z2` del ensamblaje del N3 (base de cinco siluetas, recolección terminada, sin ninguna pieza puesta) y la anterior, `Sostener ↑ 500 ms` para entrar a la zona; ambas habían salido con 0 y con la pantalla comprobada.
- **Cómo seguí:** (1) `Estado` y una `Foto` antes de pulsar nada: el juego vivo (pid 41272, 412 MB), en primer plano, misma pantalla; la diferencia de píxeles con `omi_z2` fue 0,93 % y solo en la zona de la familia (animaciones de reposo, caja 502-883 × 295-775). (2) `Datos\OE4_Z.json` sin cambios (MD5 `81CEDE67…` = semilla, `LastWriteTime` 03:35:05) y `Player.log` sin `Exception`; el muestreador seguía vivo, con 6 748 muestras y sin huecos de más de 10 s. (3) Como pantalla y datos coincidían con el paso en que iba, continué desde ahí (arrastrar los cinco troncos a la base) sin repetir nada. No puedo probar que nadie tocara el ratón durante esas horas; lo que sí comprobé es que nada cambió en pantalla ni en el JSON.
- **Efecto en las medidas:** el lanzamiento incluye ≈ 3 h 24 min de juego **inactivo** (6 135 de las 6 748 muestras de memoria y red, entre 08:37:30Z y 12:02:00Z). **No son las medidas de una sesión normal**; abajo van separadas por tramos.

## Pasos ejecutados (1–10, sin atajos)
- **1–2:** «Jugar» → `OE4_Z` → menú con los tres niveles «Completado» y habilitados → «Nivel 1». Tres narrativas con «Omitir» (encima de «Continuar»; el «Atrás» no sale en la primera línea de ninguna) y cueva. Tres clics en cada una de las cinco entradas.
- **3:** reunir las siete piezas, fuerza 7 (muesca 7), cercanía 5, «Golpear» ×3 (y ×4, ×6 en dos entradas). Mensajes: 1.º «Una chispa cae dentro de las hojas. Brilla un instante y se apaga.»; del 3.º en adelante «Un hilo de humo sube despacio, como si la cueva estuviera suspirando.», que se repite igual en los golpes 4.º, 5.º y 6.º (observación, abajo). «Soplar» pierde el candado con el 3.º.
- **4:** doble clic en «Soplar» (cinco entradas): una sola ignición, mandos atenuados sin candado, «Pista» y pausa normales; «Golpear» a +1,45 s sin efecto (dos entradas); una sola carga de `Narrative` tras la cueva en las cinco.
- **5:** doble clic en «Omitir» de `N1_NacimientoDelFuego` (cuatro entradas, cuatro intentos), siempre al resumen. **Intervalos reales** (duración de la llamada `Clic -Veces 2 -Intervalo 70` en `acciones.tsv` menos 238,8 ms de un solo clic; `PF-RF45-05_5_intervalos.tsv`): **133, 134, 131 y 136 ms** (cursor en (0,777; 0,843), (0,777; 0,843), (0,790; 0,850) y (0,760; 0,835)). Seis pares de calibración dieron 133-139 ms; los de «Soplar», 129-147 ms. El `Intervalo` nominal no es el real: 70 ms es el mínimo del arnés y deja ≈ 135 ms entre pulsaciones (un nominal de 100 ms, como en rc3, deja ≈ 165 ms).
- **6:** el resumen trae las dos viñetas (I 7, C 3) y el JSON es idéntico a la semilla.
- **7–9:** N3 con cuatro «Omitir» → orilla desde cero; recolección de los ocho materiales con las retenciones de `pasada-rc5.md` (los nueve movimientos entraron a la primera); base (5 troncos), amarre (10 sogas), tela en el mástil y mástil en la vela → «Probar balsa» → 3.2 con «Omitir» → vuelve al ensamblaje; piezas correctas → `N3_Escena33_Cruce` (7 líneas) y `N3_EscenaFinal` (7 líneas), ninguna con «Omitir», → Créditos.
- **10:** `Revisar-Log`, `Residuos`, `Restaurar-Registro`, `Preparar-Datos -SinCarpeta` y manifiesto, en ese orden.

## Cifras: previstas frente a obtenidas
| | Previsto | Obtenido |
|---|---|---|
| Intervalo entre las dos pulsaciones del doble clic | < 150 ms | 131-136 ms (Omitir) y 129-147 ms (Soplar) |
| Dobles clics en «Omitir» | ≥ 3, en entradas distintas | 4, en 4 entradas; 4 de 4 al resumen |
| JSON de `OE4_Z` | `{1,1: 7/3/3/125}` sin cambios | idéntico a la semilla (MD5) |
| Cargas de `Narrative` tras «Soplar» | 1 | 1 en cada una de las 5 entradas |
| Luz (borde izquierdo, 0-255) | sube un escalón por golpe y llega al máximo al soplar | 29,12 → 31,63 → 33,98 → 36,37 → **38,35** (4.º golpe) → 38,35 → 38,35; con 3 golpes el soplido sube de 36,91 a 38,37 |
| Líneas de `N1_NacimientoDelFuego` | 17 | 17, una por clic en la quinta entrada |

## Medidas de la sesión (aportan a otros casos; aquí no se juzgan)
- **RNF-04** (`PF-RNF04-01`): 49 líneas `RNF-04:` en `S-OMI-rc5_Player.txt` — Narrative 27 cargas (0,035-0,618 s; la más lenta, la primera tras elegir nivel), Level1_Cave 5 (0,087-0,089), Level3_River 2 (0,054), LevelSummary 6 (0,034-0,036), LevelSelect 6 (0,065-0,085), MainMenu 2 (0,086 y 0,143), Credits 1 (0,036). Peor valor 0,618 s.
- **RNF-05** (`PF-RNF05-01`), por tramos de `S-OMI-rc5_mem.csv`: antes de la interrupción (392 muestras, 08:24-08:37Z) memoria de trabajo máx 440 MiB (`Narrative`) y privada máx 1.178 MiB (`Level1_Cave`); **inactivo** (6 135 muestras) 442 y 901 MiB (`Level3_River`, constantes); después (221 muestras) 417 y 1.178 MiB (`Level1_Cave`). El máximo de la sesión, 1.178 MiB de memoria privada, es de la cueva y se repite en los dos tramos activos; el tramo inactivo no lo altera pero tampoco lo mide. No es la medida de una sesión normal.
- **RNF-10** (`PF-RNF10-01`): 6 748 muestras (incluidas las del tramo inactivo), todas `NINGUNA`. Límite del arnés: no mira los primeros ≈ 3 s del arranque.
- **RNF-11 / registro:** `Residuos` halló `Player.log` en LocalLow y nueve valores cambiados en `HKCU\Software\Universidad Catolica de Colombia\Algoritmia` (resolución, modo de pantalla, frecuencia nativa, `unity.player_session_*`, `unity_connect.*`); la clave ya existía. Tras `Restaurar-Registro`, «existía y no cambió» (`residuos_tras_restaurar.txt`). Sin `Exception`, `Error` ni `Assert` en el registro.
- **Luz de la ignición** (`PF-RF21-02_3_analisis.txt`, ráfagas a 12,1 fps de las entradas 2 y 3): la luminancia global durante la ignición varía entre 77,7 y 79,6 (±1,2 %); el fundido a negro empieza ≈ 3,4 s después del doble clic, baja a 0 en ≈ 0,35 s y la narrativa vuelve a subir en ≈ 0,45 s. Un solo ciclo de ida y vuelta por la transición; lo juzga S-RNF/S-INSP.

## Defectos
- **DEF-RC5-10** (Menor, `PF-RF21-02`): la luz de la cueva llega a su máximo con el 4.º golpe efectivo, antes de soplar; solo en el camino de tres golpes el soplido completa la luz. Detalle y lectura de los documentos en `../defectos.md`.
- Observación sin defecto: con golpes efectivos del 3.º en adelante, la tablilla repite el mismo mensaje cuatro veces seguidas («Un hilo de humo sube despacio…»). RF-18 pide consecuencias distintas para los intentos **fallidos** «cuando corresponda», así que no lo abro; lo anoto por si el evaluador lo lee con `PF-RF18-01` (que en S-N1 pasó con mensajes distintos).
- No reapareció DEF-RC3-09 (el doble clic en «Omitir» ya no deja en el menú de niveles): 4 de 4. Tampoco DEF-RC3-01: el avatar del resumen es la llama final de Algoritm.

## Avisos del arnés y límites
- `Start-Juego` no avisó de contaminación (`contaminacion: []`). Todas las llamadas del arnés salieron con 0; ninguna salida 3. El manifiesto coincidió antes del lanzamiento y coincide al terminar (253 archivos, misma huella).
- En una captura del cruce (`omi_e4_cruce5`, línea 5 de `N3_Escena33_Cruce`, fuera de la evidencia copiada; sale en `PF-RF06-04_9_2_cruce_lineas_1_a_7_y_resumen.png`, celda 5) se ve un fantasma translúcido de ventana de Windows (botones minimizar, maximizar y cerrar) en la parte superior izquierda: no es del juego, no tapó ningún punto de clic y no estaba en la foto siguiente. El indicador «G-SYNC» de NVIDIA no aparece en estas capturas.
- Los ids de las narrativas no se ven en pantalla: las identifiqué por su primera línea y su orden (`N1_Apertura` «Noche helada…», `N1_AparicionGuia` «En la oscuridad total…», `N1_Hallazgo` «¿Se fue?»; en el N3 por el número de cargas `Narrative` y por sus textos).
- Que el segundo clic del doble clic llegó al juego no se ve de forma directa: se deduce de que el arnés confirma el envío de las dos pulsaciones y de que, con el cursor sobre el «Continuar» del resumen, un clic admitido habría cargado `LevelSelect` (como en el paso 7), cosa que el log no registra.
- `PF-RF21-02`: el brillo se midió en el borde izquierdo (sin controles ni personajes) con fotos tomadas ≈ 1,8 s tras cada golpe; con el soplido, en ráfagas a 12 fps, el primer cuadro sale ≈ 0,8 s después del doble clic.
- La coordenada de la fila de `OE4_Z` (0,25; 0,305) y las de las piezas de la cueva se tomaron de la foto de cada entrada.
- El tramo de N3 usa un perfil que ya terminó el nivel: las cifras de N3 (`PF-RF45-04` solo mira N1F1) no se compararon.

## Para la siguiente sesión
- Para un doble clic «< 150 ms» con este arnés: `Clic x y -Veces 2 -Intervalo 70` y medir el intervalo real como duración de la llamada menos la de un clic simple en la misma pantalla (≈ 239 ms).
- Para «Soplar» seguido de «Golpear» a +0,1 s: dos llamadas encadenadas en el mismo bloque de PowerShell (el segundo clic cae ≈ 1,45 s después del doble clic, antes del fundido de ≈ 3,4 s); no mirar fotos entre medias.
- Una ráfaga justo después del clic en «Soplar» (`Rafaga … 6 12`) arranca ≈ 0,8 s tarde pero cubre el fundido y la entrada de la narrativa.
