# S-SPIKE — spike del arnés `oe4.ps1` sobre el exe rc1 (T03 del OE4), 01/10/2026

- **Exe:** `C:\Dev\Algoritmia\Build\Algoritmia\Algoritmia.exe`, con SHA-256 `6CD16EDF…11ADAB`. Coincide con `build-rc1.md` en las cuatro corridas.
- **Editor:** cerrado durante toda la sesión. Solo queda Unity Hub abierto; no hubo aviso de contaminación.
- **Monitor real:** la tarea describía otro. En el equipo hay **un solo monitor, de 1920×1080 al 125 %** (DPI 120): `Estado`, `Screen.AllScreens` y WMI solo ven uno, así que el secundario estaba desconectado.
- **Estado compartido:** `OE4_TRABAJO` = esta carpeta.
- **Evidencias:** `claudeDocs/tasks/OE4/evidencias/S-SPIKE/`.

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 08:45:58 | `Estado` | DPI por monitor; escritorio virtual 0 0 1920 1080; un monitor; no hay Player.log | OK |
| 2 | 08:46:47 | `Start-Juego S-SPIKE -Sha256 <hash>` (ventana 1920×1080 con marco) | Lanza; el SHA coincide; muestreador vivo. Cliente 1920×1080 en (1,38): el borde inferior (38 px) cae **fuera de la pantalla** y el arnés lo avisa | OK, con aviso → ver arreglo 3 |
| 3 | 08:46:52 | `Esperar-Carga MainMenu` | 0,100 s; hora de escritura 08:46:52.147 | OK |
| 4 | 08:47 | `Foto spike_01_menu` | **FALLÓ** (salida 2): `CopyPixelOperation (1087111200) is invalid`. El `System.Drawing` de .NET rechaza `SourceCopy \| CaptureBlt` | DEFECTO DEL ARNÉS → arreglo 1 |
| 5 | 08:47 | (arreglo 1 aplicado) `Foto spike_01_menu` | Foto válida, salvo la franja blanca de 38 px abajo | OK |
| 6 | 08:47:3x | `Cerrar-Juego` | Cierre normal (WM_CLOSE); `Datos/` no se creó (no se eligió perfil) | OK |
| 7 | 08:47:54 | `Start-Juego S-SPIKE -SinMarco` | Cliente 1920×1080 en (0,0), entero; el Player.log anterior se apartó como `lanz1` | OK |
| 8 | 08:47:57 | `Esperar-Carga MainMenu` + `Foto spike_02_menu` | 0,095 s. Se ven el título, el lema y los cuatro botones | OK |
| 9 | 08:48:27 | `Clic 0.123 0.706` («Créditos») + `Esperar-Carga Credits` | 0,040 s; cambia a «Créditos» (spike_03) | **CLIC OK** |
| 10 | 08:48:4x | `Arrastrar 0.628 0.35 0.628 0.62 800` (asa de la barra) | La lista baja hasta el final, con «Tipografías Baloo 2 y Nunito…» a la vista (spike_04) | **ARRASTRE OK** |
| 11 | 08:48:5x | `Sostener 0.628 0.23 1200` (riel de la barra) | La lista vuelve al principio, con «Proyecto de grado» a la vista (spike_05) | **SOSTENER OK** |
| 12 | 08:49:12 | `Clic` «Volver» + `Esperar-Carga MainMenu` | 0,052 s; vuelve al inicio | OK |
| 13 | 08:49:3x | `Clic` «Jugar» | Panel «¿Quién juega?» sin recargar la escena (spike_06) | OK |
| 14 | 08:49:4x | `Clic` en el campo + `Escribir OE4SPIKE` | El campo muestra «OE4SPIKE» (spike_07) | **ESCRIBIR OK** |
| 15 | 08:49:54 | `Clic` «Continuar» + `Esperar-Carga *` | LevelSelect 0,066 s y, enseguida, Narrative 0,747 s: un perfil nuevo entra directo a N1_Apertura. Se crea `Datos/OE4SPIKE.json` = `{"name":"OE4SPIKE","reachedLevel":1,"phases":[]}` | OK |
| 16 | 08:50–08:51 | `Clic` «Continuar» ×6, ×1, ×11 | ×6 y ×1 avanzan. Tras cambiar de escena (N1_AparicionGuia), **11 clics y 1 suelto no hacen nada** | DEFECTO DEL ARNÉS → arreglo 2 |
| 17 | 08:52:5x | (arreglo 2 aplicado) `Clic` «Continuar» | Avanza a la línea 2 (spike_12 sin mover → spike_13 con el arreglo) | OK |
| 18 | 08:53 | `Clic` ×10 y ×17 | Recorre AparicionGuia y Hallazgo hasta «Lo que no sabes todavía es cómo. ¿Desde dónde vas a intentarlo?» | OK |
| 19 | 08:54:01 | `Clic` + `Esperar-Carga Level1_Cave` | 0,155 s. Se ven 7 piezas, la tablilla «Reúne todas…», la pista y la pausa (spike_16) | OK |
| 20 | 08:54:2x | `Arrastrar 0.251 0.827 0.49 0.52 900` (una hoja al círculo) | La hoja queda en el centro; las otras 6 piezas no se mueven y el nivel no avanza (spike_17) | **ARRASTRE EN EL N1 OK** |
| 21 | 08:55:2x | `Revisar-Log S-SPIKE` | Sin Exception, Error ni Assert; 8 líneas RNF-04 | OK |
| 22 | 08:55:31 | `Matar` | taskkill /F; el muestreador termina solo | OK |
| 23 | 08:55:4x | `Residuos S-SPIKE -Buscar OE4SPIKE` | Lista lo nuevo en LocalLow, la clave del registro creada (22 valores) y el perfil en `Datos` (salida 1, la esperada con `-Buscar`). Ver el **DEF** de abajo | OK (el arnés funciona) |
| 24 | 08:55:5x | `Restaurar-Registro S-SPIKE` | Borra la clave del producto y la de la empresa, que no existían antes; `Test-Path` da False en las dos | OK |
| 25 | 08:55:5x | `Tamano` | 478 979 915 B = 479,0 MB; RNF-06 CUMPLE (igual que `build-rc1.md`) | OK |
| 26 | 08:56:11 | `Preparar-Datos OE4_Z evidencias\S-SPIKE\S-SPIKE_OE4SPIKE.json` | Vacía `Datos` y copia las dos semillas validadas | OK |
| 27 | 08:56:2x | `Start-Juego -SinMarco`, «Jugar» | El panel lista OE4SPIKE y OE4_Z, cada uno con su papelera (spike_20) | OK |
| 28 | 08:56:35 | `Clic` en la fila OE4_Z + `Esperar-Carga LevelSelect` | 0,064 s; «Elige un nivel» con los tres niveles «Completado» (spike_21) | OK |
| 29 | 08:56:50 | `Clic` «Nivel 3» + `Esperar-Carga Narrative` | 0,701 s; N3_PuenteII **con «Omitir»** (el nivel está terminado) | OK |
| 30 | 08:57 | `Clic` «Omitir» ×4 con `Esperar-Carga *` | Cada «Omitir» salta una secuencia; la 4.ª carga Level3_River en 0,053 s. La 1.ª hora de escritura salió **mal** (08:53:22, de otro lanzamiento) | DEFECTO DEL ARNÉS → arreglo 4 (luego bien: 08:57:56, 08:57:59, 08:58:03) |
| 31 | 08:58:1x | `Sostener 0.833 0.787 600` (flecha →) | Mamá avanza ~400 px a la derecha, hasta el borde de la zona de construcción, y Algoritm dice «Para armar la balsa todavía falta…» (spike_24 → spike_25) | **SOSTENER EN EL N3 OK** |
| 32 | 08:58:2x | `Teclas Esc Enter Espacio Arriba Abajo Izquierda Derecha W A S D Rueda` | Mamá no se mueve (spike_26) | OK |
| 33 | 08:58:3x | `Cerrar-Juego`, `Revisar-Log`, `Medidas S-SPIKE` | RNF-05: máx. 1129 MiB, CUMPLE. RNF-04: 13 cargas, la peor 0,701 s. **RNF-10: 685 filas EXTERNA** (salida 1) | El arnés OK; DEF abajo |
| 34 | 08:59 | `Residuos`, `Restaurar-Registro` | Igual que en 23–24; la clave queda borrada | OK |
| 35 | 08:59:1x | Limpieza: borrar `Datos\OE4SPIKE.json`, `Datos\OE4_Z.json` y la carpeta `Datos` (no existía antes) | `Test-Path Datos` da False | OK |
| 36 | 08:59:4x | (arreglo 3 aplicado) `Start-Juego S-SPIKE` sin `-SinMarco`, con `-Sha256 build-rc1.md` | «AVISO: … se lanza sin marco»; cliente en (0,0) entero (spike_30); MainMenu 0,096 s. Después `Matar` y `Restaurar-Registro` | OK |

## Arreglos al arnés (`claudeDocs/tasks/OE4/herramientas/oe4.ps1`; es herramienta, no juego)

1. **`Capture`:** `CopyFromScreen` usaba `SourceCopy | CaptureBlt`, que el `System.Drawing` de .NET rechaza. Ahora usa solo `SourceCopy`, y **ninguna `Foto` funcionaba antes de este arreglo**.
2. **`Nudge`:** antes de cada pulsación (`Settle` y cada clic de `-Veces n`) el cursor se mueve un píxel al lado y vuelve.
   - Tras cargar una escena, su EventSystem no sabe dónde está el puntero hasta que el ratón se mueve, así que un clic sin movimiento previo se perdía.
   - Es lo que pasaba en los recorridos de narrativas, donde «Continuar» queda siempre en el mismo punto.
   - Comprobado: 12 clics perdidos antes del arreglo, 0 después (28 líneas avanzadas).
3. **`Start-Juego`:** si el cliente pedido no cabe con marco en el monitor principal (ancho o alto ≥ los del monitor), se lanza sin marco solo (`-popupwindow`) y lo avisa.
4. **`Esperar-Carga`, la hora de escritura:**
   - Antes tomaba la **última** línea igual de `registro.tsv`, que acumula todos los lanzamientos, sin esperar a que el muestreador copiara la nueva. Daba horas de minutos antes.
   - Ahora toma la **primera** línea igual del lanzamiento actual escrita después de la marca. La marca nueva, `marcaUtc`, la ponen `Mark-Offset` y `Start-Juego`.
   - Reintenta hasta 4 × 150 ms y, al encontrarla, avanza la marca.

## DEF-SPIKE-01 · El ejecutable rc1 se conecta a Internet: Unity Analytics/Connect activo (RNF-08, RNF-10, RNF-11)

- **Qué se ve:** en los cuatro lanzamientos, el proceso del juego mantiene desde el primer segundo **tres conexiones TCP establecidas a :443**: `34.8.90.77`, `34.111.113.40` y `34.107.172.168` (Google Cloud).
  - `red.csv` tiene 685 filas `EXTERNA`.
- **Huellas fuera de la carpeta portable:**
  - LocalLow: `Unity\<id>\Analytics\{config,values,ArchivedEvents\…}` y `Unity\ShaderVariantAnalytics\ShaderRuntimeInfoEvent.json`.
  - Registro: `unity.cloud_userid`, `unity_connect.installation_id`, `unity_connect.session_id` y `unity_connect.mega_session_id`.
  - Ninguna línea del Player.log lo menciona.
- **Causa probable:** `build-rc1.md` ya anota que, durante el build, el Editor puso `UnityConnectSettings.asset` en `m_Enabled: 1` y que «el player ya estaba empaquetado con esa configuración». Después se restauró con git, pero el player salió con los servicios de Unity activos.
  - `globalgamemanagers` lleva `cdp.cloud.unity3d.com`, `config.uca.cloud.unity3d.com` y `perf-events.cloud.unity3d.com`.
  - `ScriptingAssemblies.json` incluye además `Unity.AI.MCP.Runtime.dll` y `Unity.AI.Tracing.dll` (`com.unity.ai.assistant`).
- **Impacto:** RNF-10, ningún dato por red, **NO CUMPLE** con este build. También RNF-08 y RNF-11 (residuos fuera de la carpeta portable, además del Player.log).
- **Recomendación (no aplicada: es cambio del juego):**
  1. desactivar Analytics y Connect en Project Settings → Services;
  2. comprobar en el player que `m_Enabled` queda en 0 (que el build no lo vuelva a encender);
  3. valorar sacar del build el runtime de `com.unity.ai.assistant`;
  4. recompilar el rc y repetir `Medidas` y `Residuos`.
- **Evidencia:** `evidencias/S-SPIKE/red.csv`, `residuos.txt` y `mem.csv`, y el «RNF-10: 685 fila(s) EXTERNA(s)» de `Medidas`.

## Observaciones menores (no son del spike)

- En «Elige un nivel» (spike_21) el rótulo «Completado» toca el borde derecho de su píldora en las tres tarjetas, sin margen a la derecha. Lo ve S-INI paso 14.
- El SHA-256 del `.exe` es idéntico entre el build descartado de 827 MB y el rc1, porque el `.exe` es solo el lanzador. **La guarda `-Sha256` no distingue builds:** solo prueba que el lanzador es el mismo. Para congelar de verdad hace falta una huella de `Algoritmia_Data` (por ejemplo `globalgamemanagers` + `Managed\Game.*.dll`).
- `Medidas` muestra las muestras sin conexión como clase vacía («=1»). Es cosmético.

## Veredicto del spike

**PASA.** Con los cuatro arreglos, el arnés maneja el exe real:

- **clic:** menú, créditos, perfiles, niveles, «Continuar» y «Omitir»;
- **escritura:** el nombre del perfil;
- **arrastre:** la barra de créditos y una hoja del N1 al círculo;
- **clic sostenido:** el riel de créditos y la flecha → del N3, que mueve a Mamá;
- y lo demás: teclas, cargas RNF-04 desde el Player.log, muestreo de memoria y red, revisión del log, cierre forzado y normal, residuos, restauración del registro y tamaño.

**Queda:** el perfil de prueba y `Datos/` borrados; el registro restaurado; ningún juego en marcha; el Editor cerrado.
