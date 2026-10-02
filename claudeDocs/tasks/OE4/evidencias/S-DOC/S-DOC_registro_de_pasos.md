# S-DOC — Informe docente, borrado y residuos (T16 del OE4) + RNF-07 local (PF-RNF07-01) — 01/10/2026

- **Exe:** `C:\Dev\Algoritmia\Build\Algoritmia\Algoritmia.exe` (rc1, 479,0 MB; SHA-256 `6CD16EDF…ADAB`, comprobado en cada `Start-Juego`). Editor cerrado (solo Unity Hub).
- **Arnés:** `oe4.ps1`, sesión `S-DOC` (y `S-RNF07` para la copia portable), ventana sin marco 1920×1080 en (0,0).
- **Evidencias:** `claudeDocs/tasks/OE4/evidencias/S-DOC/`.
- **Perfiles (adaptación de la precondición).** `casos.md` pide los JSON finales de S-N1, S-N2 y S-N3, que no existen: esas sesiones las cubrieron los Golden Path y S-PER. Se usan los JSON reales equivalentes, cada uno con cifras conocidas y distintas:
  - `OE4N1b` ← `evidencias/S-PER/S-PER_OE4N1b_final.json` (solo N1F1 = 0/0/3/80,21 s): hace de «OE4N1» (pasos 3 y 7);
  - `OE4_B` ← `evidencias/S-PER/S-PER_OE4_B_final.json` (N1F1 semilla 7/3/3/125 + N2F1 0/0/0/81,41 · N2F2 0/0/6/97,66 · N2F3 0/0/13/466,48): hace de «OE4_B» (paso 4);
  - `OE4GP1` ← `evidencias/GP1/GP1_OE4GP1_final.json` (las siete fases jugadas en GP1, con errores): hace de «OE4_C» (pasos 5 y 8);
  - `OE4_Z` ← semilla (paso 2).
  El oráculo de los pasos 2–5 es el propio JSON: el informe debe mostrar exactamente lo persistido (que esas cifras salen bien del juego ya lo comprobaron GP1, GP2 y S-PER).

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 0 | 12:16:56 | Listado previo de LocalLow, HKCU y `Datos` (`listar_residuos.ps1`) | LocalLow: `Player.log` (de S-PER), los restos de Unity Analytics/Insights de DEF-SPIKE-01 y lo que deja el **Editor** con el mismo producto (`CoplayImagePreviews\`, `TestScreenshots\`, `TestResults.xml`), ajeno al exe; HKCU: ni la clave de la empresa ni la del producto existen → `S-DOC_residuos_antes.txt` | OK |
| 1 | 12:17:31 | `Preparar-Datos` con los cuatro perfiles | `OE4GP1.json` 12:17:31,031 · `OE4N1b.json` 12:17:31,025 · `OE4_B.json` 12:17:31,029 · `OE4_Z.json` 12:17:31,032 (LastWriteTime) | OK |
| 2 | 12:17:37–44 | `Start-Juego S-DOC` → «Progreso del equipo» + `Medir-Carga` | Foto previa de residuos (279 entradas, clave no existe); MainMenu 0,095 s; **TeacherReport 0,057 s**. «Progreso del equipo», la lista OE4GP1 · OE4N1b · OE4_B · OE4_Z, «Eliminar datos», la tabla con «Nivel», «Fase», «Intentos», «Errores corregidos», «Pasos utilizados», «Tiempo de resolución», y «Volver al menú». Abre mostrando el primero (OE4GP1): 5/1/3/«6:25» · 3/1/0/«3:27» · 5/3/6/«3:38» · 3/40/10/«15:37» · 2/1/1/«3:13» · 2/1/2/«1:28» · 3/3/3/«1:50» = **exactamente** el JSON (384,98 · 207,48 · 217,93 · 936,72 · 192,80 · 88,02 · 110,01 s, redondeados al segundo) (sdoc_001 → `PF-RF46-01_informe_OE4GP1.png`) | P |
| 3 | 12:18:39 | «OE4_Z» | Siete filas, de «Nivel 1 · Fase 1» a «Nivel 3 · Fase 3»: 7/3/3/«2:05» · 4/2/0/«1:01» · 5/3/6/«1:35» · 2/5/12/«2:20» · 1/1/1/«0:40» · 2/1/2/«1:10» · 3/2/3/«1:40» = **exactamente** las cifras de la semilla, tiempo en `m:ss` (`PF-RF46-01_informe_OE4_Z_cifras_semilla.png`) | **P** |
| 4 | 12:18:40 | «OE4N1b» (hace de OE4N1) | N1F1 = 0/0/3/«1:20» (80,21 s, jugado en S-PER); **las otras seis filas dicen «Sin datos» en las cuatro columnas, nunca 0** (`PF-RF46-02_informe_OE4N1b_sin_datos.png`) | P |
| 5 | 12:18:42 | «OE4_B» (hace de OE4_B) | N1F1 7/3/3/«2:05» (semilla), N2F1 0/0/0/«1:21», N2F2 0/0/6/«1:38», N2F3 0/0/13/«7:46» (81,41 · 97,66 · 466,48 s, jugados en S-PER) y el N3 «Sin datos». Los **0** de las fases sin errores se muestran como 0 y no como «Sin datos»: el informe distingue «jugado sin errores» de «no jugado» (`PF-RF46-02_informe_OE4_B.png`) | P |
| 6 | — | OE4GP1 (hace de OE4_C, paso 5 del catálogo) | Visto en el paso 2: las cifras del N3 de GP1 (2/1/1, 2/1/2, 3/3/3) coinciden con su JSON; ya acumuladas entre fases por el juego (INC-88) | P |
| 7 | 12:19:09 | «Volver al menú» | Pantalla de inicio 0,069 s; **ningún JSON cambió su LastWriteTime** (`diff` del listado antes/después: sin cambios). El informe es de solo lectura | **P — PF-RF46-01** |
| 8 | 12:19:12–30 | «Jugar» → papelera de «OE4N1b» → «Conservar» | «¿Borras el perfil de OE4N1b?», «Su avance se pierde y no se puede recuperar.», «Conservar» y «Borrar» con la papelera; tras «Conservar» el perfil sigue en la lista y `OE4N1b.json` en disco (`PF-RF47-01_dialogo_y_conservar.png`) | P |
| 9 | 12:19:45–47 | Papelera otra vez → «Borrar» | Desaparece de la lista (quedan OE4GP1, OE4_B, OE4_Z) y `Datos/OE4N1b.json` **ya no existe** (`PF-RF47-01_borrado_desde_el_panel.png`) | **P — PF-RF47-01** |
| 10 | 12:20:00–07 | «Volver» → «Progreso del equipo» → «OE4GP1» → «Eliminar datos» | «¿Eliminas definitivamente los datos de OE4GP1? Esta acción no se puede deshacer.», con «Cancelar» y «Eliminar datos» (`PF-RF47-02_dialogo_eliminar_datos.png`) | P |
| 11 | 12:20:21–25 | «Cancelar»; otra vez «Eliminar datos» → «Eliminar datos» | Tras «Cancelar» nada cambia (lista, tabla y disco). Tras confirmar, OE4GP1 desaparece de la lista y del disco y **la tabla pasa a OE4_B** (`PF-RF47-02_cancelar_y_eliminar.png`) | P |
| 12 | 12:20:59–21:07 | «Eliminar datos» → confirmar para OE4_B y luego para OE4_Z | **«Todavía no hay perfiles registrados en este equipo.»** y «Volver al menú»; `Datos/` vacía (`PF-RF46-03_sin_perfiles.png`) | **P — PF-RF46-03 y PF-RF47-02** |
| 13 | 12:21:21–25 | «Volver al menú» → `Cerrar-Juego` → `Residuos S-DOC -Buscar OE4N1b,OE4GP1,OE4_B,OE4_Z` | **Cero apariciones** de los cuatro nombres en `Build/Algoritmia` entero, en LocalLow (incluidos `Player.log` y los archivos de analítica), en lo nuevo de %TEMP% y en HKCU (`S-DOC_residuos_rastreo_tras_borrado.txt`). Fuera de `Datos/` el juego dejó: `Player.log`; los archivos de Unity Analytics/Insights (DEF-SPIKE-01); en HKCU, la clave del producto con 16 valores de pantalla y **6 `unity.*`/`unity_connect.*`** (DEF-SPIKE-01) → `S-DOC_residuos_despues_del_borrado.txt` | **P — PF-RNF11-01** (rastro del perfil); la parte de portabilidad, F por DEF-SPIKE-01 |
| 14 | 12:22:2x | Con el juego cerrado: `Datos/` (vacía) se aparta como `Datos_aparte_SDOC` y en su lugar se crea un **archivo** vacío `Datos`, sin extensión | `Build/Algoritmia/Datos` es un archivo de 0 B | OK |
| 15 | 12:22:42–52 | `Start-Juego` (lanz. 2) → «Jugar» → `Escribir OE4R` → «Continuar» | El juego no se queja al crear el perfil y entra a `N1_Apertura`; el perfil queda en **`%USERPROFILE%\AppData\LocalLow\Universidad Catolica de Colombia\Algoritmia\OE4R.json`** = `{"name":"OE4R","reachedLevel":1,"phases":[]}` (`PF-RF47-03_crear_OE4R_con_Datos_no_escribible.png`) | P |
| 16 | 12:23:41–24:27 | Narrativas → cueva → pausa → «Volver al menú de niveles» → «Volver» → «Progreso del equipo» | El informe **lista «OE4R»** (todas las filas «Sin datos») y avisa arriba: «Este equipo no deja guardar en la carpeta del juego: los perfiles se están guardando en la carpeta de datos de Windows.» (`PF-RF47-03_informe_lista_OE4R_y_aviso.png`) | P |
| 17 | 12:24:41–44 | «Volver al menú» → «Salir» (con OE4R activo) | **Antes de cerrar avisa**: «Lo que has logrado ya está guardado. ¿Cerramos el juego?» + «Este equipo no deja guardar en la carpeta Datos del juego. El progreso quedó guardado en:» + la **ruta de respaldo entera** `C:\Users\benab\AppData\LocalLow\Universidad Catolica de Colombia\Algoritmia`, en su línea, **sin montarse** sobre «Quedarme» ni «Cerrar el juego» (queda a ~8 px; `PF-RF09-04_salir_avisa_ruta_de_respaldo.png`, `PF-RF09-04_ruta_entera_sin_solaparse.png`). El botón «Cerrar el juego» repite DEF-SPER-01 | **P — PF-RF09-04** |
| 18 | 12:25:22–31 | «Quedarme» → «Progreso del equipo» → «Eliminar datos» → «Eliminar datos» | «¿Eliminas definitivamente los datos de OE4R? Esta acción no se puede deshacer.»; tras confirmar, «Todavía no hay perfiles registrados en este equipo.» con el aviso de la ruta de respaldo arriba; **`OE4R.json` desaparece de la ruta de respaldo** (`PF-RF47-03_eliminar_OE4R_del_informe.png`) | **P — PF-RF47-03** |
| 19 | 12:26:05–09 | «Volver al menú» → «Salir» → «Cerrar el juego» | El mismo aviso con la ruta (sdoc_021); el proceso termina en < 4 s. **El perfil borrado no se recrea al salir**: en LocalLow solo `Player.log` y `TestResults.xml` (del Editor) | P |
| 20 | 12:26:2x | Borrar el archivo `Datos` y devolver la carpeta | `Build/Algoritmia/Datos/` vuelve a ser la carpeta (vacía) | OK |
| 21 | 12:26:29 | `Revisar-Log S-DOC` (lanzamientos 1–2) | **Sin Exception, Error ni Assert** (tampoco al caer a la ruta de respaldo) | OK |
| 22 | 12:26:3x | INSP: todos los JSON de perfil de `evidencias/` (`S-DOC_PF-RNF09-01_inspeccion_json.txt`) | **15 perfiles** (GP1, GP2, S-PER, S-SPIKE): todos con **solo** `name`, `reachedLevel` y `phases[level, phase, attempts, correctedErrors, stepsUsed, resolutionSeconds]`; también `CON.json` y `OE4R.json`, vistos en vivo. El único otro JSON es `build-rc1.build_status.json`, que no es un perfil | **P — PF-RNF09-01** |
| 23 | 12:26:51 | `Medidas S-DOC` | RNF-05: máx. 1123 MiB privados, CUMPLE. RNF-04: TeacherReport 0,051–0,057 s, todas < 1 s. **RNF-10: 455 filas EXTERNA** (TCP a `34.8.90.77`, `34.111.113.40`, `34.107.172.168`, :443): **DEF-SPIKE-01 reproducido**; ningún socket en `Listen`, ningún UDP (las 455 «escucha-local» son los `Bound` 0.0.0.0 de esas mismas conexiones) | RNF-10 F (DEF-SPIKE-01) |
| 24 | 12:27:01 | `Restaurar-Registro S-DOC` + listado final | Borradas la clave del producto y la de la empresa (no existían antes); `S-DOC_residuos_al_cerrar_la_sesion.txt` | OK |
| **R** | | **PF-RNF07-01 · copia portable en una ruta con espacio, sin privilegios elevados (sesión del arnés `S-RNF07`)** | | |
| 25 | 12:28:0x–12:28:3x | Copiar `Build\Algoritmia` (sin `Datos`) a **`D:\User\Desktop\Algoritmia prueba\`** | `%USERPROFILE%\Desktop` no existe en este equipo: el escritorio del usuario está redirigido a `D:\User\Desktop` (`[Environment]::GetFolderPath("Desktop")`), así que la copia va al escritorio real, con espacio en la ruta y fuera del repo. 458 MB, 252 archivos; `diff -rq` idéntico salvo `Datos`; SHA-256 del exe igual al rc1 | OK |
| 26 | 12:28:41–46 | `Start-Juego S-RNF07 -Exe "D:\User\Desktop\Algoritmia prueba\Algoritmia.exe"` | Arranca sin pedir nada: MainMenu 0,094 s, pantalla de inicio normal. Proceso `D:\User\Desktop\Algoritmia prueba\Algoritmia.exe`; **token no elevado** (`TokenElevation=0`, tipo 3 «limitado»); el manifiesto pide `asInvoker`; el shell del arnés no es administrador (Medium). Todavía no hay `Datos` | P |
| 27 | 12:29:08–14 | «Jugar» → `Escribir OE4P7` → «Continuar» | `Datos\` **nace junto a ESE exe** al abrir «¿Quién juega?» (12:29:08) y guarda `OE4P7.json` = `{"name":"OE4P7","reachedLevel":1,"phases":[]}`; `Build\Algoritmia\Datos` no se toca | P |
| 28 | 12:29:20–30:06 | Narrativas (×7, ×11, ×18) → cueva → dos hojas al centro | `Level1_Cave` 0,155 s; se juega con normalidad | P |
| 29 | 12:30:07–16 | Pausa → «Volver al menú de niveles» → «Volver» → «Salir» → «Cerrar el juego» | El proceso termina; `OE4P7.json` reescrito a las 12:30:16,48 **en la carpeta de la copia** (`PF-RNF07-01_copia_con_espacios_inicio_cueva_salir.png`, `S-RNF07_portabilidad.txt`) | P |
| 30 | 12:30:38–55 | `Revisar-Log S-RNF07`, `Medidas`, `Residuos -Buscar OE4P7`, `Restaurar-Registro` | Sin Exception ni Error. RNF-05 1121 MiB. **RNF-10: 138 filas EXTERNA** (DEF-SPIKE-01). «OE4P7» solo aparece en `Datos\OE4P7.json` de la copia: en ningún otro sitio. Fuera de la carpeta, lo de siempre: `Player.log`, Unity Analytics y la clave de HKCU (borrada) | **PF-RNF07-01: P** en este equipo (falta PF-RNF07-02, equipo 2) |
| 31 | 12:31:03 | Borrar la copia (temporal, creada por esta sesión) | `D:\User\Desktop\Algoritmia prueba` ya no existe; HKCU sin la clave de la empresa | OK |
| 32 | 12:31:1x | Listado final de residuos (`S-DOC_residuos_final_tras_RNF07.txt`) frente al previo | Las mismas 261 entradas que antes de la sesión; solo cambian los nombres de dos lotes de `Unity\…\Analytics\ArchivedEvents` (DEF-SPIKE-01) y desaparece el `OE4_B.json` que quedaba de S-PER. HKCU: ninguna clave | OK |

## Casos de S-DOC y RNF-07

| Caso | Veredicto | Evidencia | Nota |
|---|---|---|---|
| **PF-RF46-01** · consultar cada perfil sin alterar nada | **P** | `PF-RF46-01_*.png`, pasos 2, 3 y 7 | Cuatro indicadores por nivel y fase, tiempos en `m:ss`, solo lectura (ningún LastWriteTime cambia). Con OBS-SDOC-1 y OBS-SDOC-2 |
| **PF-RF46-02** · las cifras coinciden con lo previsto | **P** | `PF-RF46-02_*.png`, pasos 2, 4–6 | Coinciden al segundo con los JSON de GP1 y S-PER (que a su vez coincidieron con el cronómetro del arnés ±1 s). «Sin datos» para lo no jugado, 0 para lo jugado sin errores. Adaptado: OE4N1b/OE4_B/OE4GP1 en lugar de los JSON de S-N1/S-N2/S-N3, que no existen |
| **PF-RF46-03** · sin perfiles lo informa | **P** | `PF-RF46-03_sin_perfiles.png` | «Todavía no hay perfiles registrados en este equipo.» y «Volver al menú» |
| **PF-RF47-01** · borrar desde el panel | **P** | `PF-RF47-01_*.png` | Confirmación explícita, «Conservar» cancela, «Borrar» quita de la lista y del disco |
| **PF-RF47-02** · borrar desde el informe | **P** | `PF-RF47-02_*.png`, `PF-RF46-03_sin_perfiles.png` | Advierte la irreversibilidad; «Cancelar» no cambia nada; la tabla pasa a otro perfil |
| **PF-RF47-03** · ruta de respaldo (INC-34) | **P** | `PF-RF47-03_*.png` | Con `Datos` convertido en archivo, el perfil cae en `LocalLow\…\Algoritmia\OE4R.json`, el informe lo lista y avisa, y se borra de ahí |
| **PF-RF09-04** · «Salir» con `Datos/` no escribible | **P** | `PF-RF09-04_*.png` | Avisa antes de cerrar y muestra la ruta entera sin montarse sobre los botones (con DEF-SPER-01 en el botón) |
| **PF-RNF11-01** · sin rastro del perfil borrado | **P** | `S-DOC_residuos_rastreo_tras_borrado.txt` | Cero apariciones de OE4N1b, OE4GP1, OE4_B y OE4_Z en la carpeta portable, LocalLow (con `Player.log`), %TEMP% y HKCU |
| Decisión 29/09 · fuera de `Datos/` solo `Player.log`/`Player-prev.log` y claves de pantalla | **F** (DEF-SPIKE-01) | `S-DOC_residuos_*.txt` | Además de eso quedan `LocalLow\…\Unity\<id>\Analytics`, `Insights` y `ShaderVariantAnalytics` y 6 valores `unity.*`/`unity_connect.*` en HKCU. Ninguno contiene datos del jugador |
| **PF-RNF09-01** · solo nombre, progreso e indicadores | **P** | `S-DOC_PF-RNF09-01_inspeccion_json.txt` | 15 JSON de perfil de las evidencias + CON y OE4R vistos en vivo |
| **PF-RNF07-01** · portable en ruta con espacio, sin administrador | **P** | `PF-RNF07-01_*.png`, `S-RNF07_portabilidad.txt` | `Datos/` nace junto a la copia; nada se instala ni se pide. **RNF-07 exige también PF-RNF07-02** (equipo 2, Santiago) |
| Apoyo a RNF-08/RNF-10 (`red.csv`) | **F** (DEF-SPIKE-01) | `S-DOC_red.csv`, `S-RNF07_red.csv`, `*_medidas.txt` | TCP salientes a :443 (Google Cloud) en los tres lanzamientos; ningún puerto en escucha, ningún UDP |

## Defectos

- **Nuevos:** ninguno en S-DOC. El botón «Cerrar el juego» del aviso de respaldo repite **DEF-SPER-01**.
- **Reproducido:** **DEF-SPIKE-01** (Unity Analytics/Connect): conexiones a Internet en S-DOC y en la copia portable, y residuos fuera de `Datos/` (LocalLow y HKCU).

## Observaciones (no son defectos)

- **OBS-SDOC-1 · Iconos de la cabecera del informe encima del texto.** En «Errores corregidos» y «Tiempo de resolución», que ocupan dos líneas, el icono de boceto se dibuja sobre la primera palabra («Errores», «Tiempo»); y «Volver al menú» parte en dos líneas que tocan el borde del botón (`OBS-SDOC-1_iconos_sobre_cabecera_y_volver.png`). Los iconos son los bocetos provisionales de Slice 4 D1, todavía abierto: se arregla cuando entren los definitivos. El informe no lo ve el estudiante.
- **OBS-SDOC-2 · El informe no marca de quién es la tabla.** Las cuatro filas de la lista se ven iguales y la tabla no lleva el nombre del perfil: tras un clic, el docente solo sabe a quién mira por memoria. Mejora de usabilidad para Santiago (ningún RF lo exige).
- **OBS-SDOC-3 · La lista de «¿Quién juega?» con cuatro perfiles** ya roza el borde inferior del panel (la cuarta fila queda cortada abajo, `sdoc_005`). Con un curso entero (20–30 perfiles) habría que ver cómo se desplaza: no lo cubre ningún caso del catálogo; conviene uno en la Hoja-HUM o en la siguiente tanda.
- **OBS-SDOC-4 · `Datos/` nace al abrir «¿Quién juega?»,** no al arrancar el exe (en la copia: arranque 12:28:41, carpeta 12:29:08). Sigue siendo «en la primera ejecución»; se anota por si algún documento dice «al arrancar».
- **OBS-SDOC-5 · LocalLow comparte carpeta con el Editor.** `CoplayImagePreviews\`, `TestScreenshots\` y `TestResults.xml` los deja el Editor de Unity (mismo `companyName`/`productName`), no el exe: en el equipo de un docente no aparecerán.

## Estado al terminar

- **Juego:** cerrado; muestreadores terminados. Ninguna copia del juego fuera de `Build\Algoritmia` (la de prueba, borrada).
- **Registro:** restaurado (sin la clave de la empresa).
- **Datos:** `Build\Algoritmia\Datos\` existe y está **vacía** (la carpeta original, devuelta tras el paso de INC-34). Ningún perfil en la ruta de respaldo.
- **LocalLow:** `Player.log` y los restos de Unity Analytics de DEF-SPIKE-01 (evidencia; no se borran).
- **Editor:** cerrado todo el tiempo. No se tocó código.
