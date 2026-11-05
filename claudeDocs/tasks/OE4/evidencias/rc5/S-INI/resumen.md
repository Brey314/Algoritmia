# S-INI · rc5 · resumen

- **Versión:** rc5, compilado desde `ca6fd91` (`../../build-rc5.md`). Carpeta `Build\Algoritmia_rc5\`, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3` (253 archivos, `manifiesto-rc5.tsv`). SHA-256 del `.exe` (solo el lanzador, el mismo de rc3) `6CD16EDF…ADAB`.
- **Fecha:** 04/11/2026, 21:19–21:28 (hora local). `Preparar-Datos` 21:19:12; primer `Start-Juego` 21:19:19; `Cerrar-Juego` final 21:27:44. Ejecutor EXE con `oe4.ps1`, sesión de arnés `S-INI-rc5`, ventana de 1920×1080 con marco (cliente en 1,31; monitor principal de 2560×1440), tres lanzamientos (`S-INI-rc5_lanz1_Player.txt`, `_lanz2_` y `S-INI-rc5_Player.txt`, que es el tercero).
- **Casos:** 14 (todos los de la tabla de S-INI). **Veredictos: P 14 · PD 0 · F 0 · B 0 · NA 0 · NE 0.** Detalle en `veredictos.csv`.
- **Pasos 1–21 ejecutados**, sin atajos: lanzamiento 1 (pasos 1–15, perfil `OE4_B`), lanzamiento 2 (paso 16, sin perfil) y lanzamiento 3 (pasos 18–21, ocho perfiles `OE4_A, B, B2, B3, C, D, E, Z`). El paso 17 (`Revisar-Log`) se corrió al cerrar cada lanzamiento.
- **Casos que se alimentan también de otra sesión:** `PF-RF02-01` — aquí se juzgaron los pasos 8 a 11 (vacío, espacios, duplicado e inválido, sin crear archivo); la creación de `Datos/<nombre>.json` con un nombre válido y su contenido (`reachedLevel` 1, `phases` vacío) es el paso 1 de S-N1.

## Lo que se vio en rc5
- **Portada (PF-RF01-03):** los cinco personajes de frente en el bosque, enteros, a la derecha del panel ámbar; ninguno tapa un botón. Ráfaga de 6 s: el fondo no cambia y cada personaje tiene su pico de movimiento en un cuadro distinto (`PF-RF01-03_2_analisis.txt`).
- **Créditos:** Algoritm (llama con pantaloneta) saluda en bucle, con los brazos detrás del cuerpo y sin tapar la lista; los cuatro textos de autoría exactos; ningún enlace. La lista se desplaza solo con la barra (el clic en el riel la pagina).
- **Perfiles:** mensajes exactos de los cuatro rechazos; el campo conserva 24 de 30 caracteres; paginación 3 + 3 + 2 con ▲/▼ en los dos sentidos; rueda y arrastre no mueven la lista; «Conservar» no cambia ningún JSON.
- **Salida:** diálogo «Lo que has logrado ya está guardado. ¿Cerramos el juego?»; el proceso termina ≈ 0,7 s después de «Cerrar el juego»; sin perfil activo no se escribe ningún JSON.

## Medidas de la sesión (aportan a otros casos; aquí no se juzgan)
- **RNF-04** (`PF-RNF04-01`, lo consolida otra sesión): 8 líneas `RNF-04:` en los tres `Player.txt` — MainMenu 0,142 · 0,054 · 0,041 · 0,125 · 0,127 s, Credits 0,018 s, LevelSelect 0,027 y 0,018 s; peor valor 0,142 s. `cargas_oe4.csv` trae 7 (la de MainMenu tras «Volver» desde LevelSelect, 0,041 s, solo está en el log). Boot→MainMenu: ≤ 3,3 s en el lanzamiento 1 (de `Start-Process` a la primera lectura del log) y ≤ 3,1 s en el 3; cotas, no medida directa.
- **RNF-05:** memoria de trabajo máx 433 MiB y privada máx 884 MiB (ambas en `MainMenu`, lanzamiento 1; lanzamientos 2 y 3: 403/862 y 404/864 MiB). Solo se recorrió inicio, créditos, perfiles y menú de niveles: no es la medida de un nivel. Frente a rc3 (405 y 779 MiB) la privada sube ≈ 13 %; sin juicio.
- **RNF-10:** 233 muestras, todas `NINGUNA` (ninguna conexión del proceso). Límite conocido del arnés: no mira los primeros ≈ 3 s de cada arranque.
- **RNF-11 / registro:** `Residuos` (`residuos.txt`) halló `Player.log` en LocalLow (el viejo lo aparta `Start-Juego`) y, en `HKCU\Software\Universidad Catolica de Colombia\Algoritmia`, ocho valores cambiados (`Screenmanager Resolution Width/Height/Use Native/Fullscreen mode`, `unity.player_session_count`, `unity.player_sessionid`, `unity_connect.mega_session_id`, `unity_connect.session_id`); nada más fuera de eso. La clave ya existía antes de la sesión. Tras `Restaurar-Registro`, `Residuos` dice «existía y no cambió» (`residuos_tras_restaurar.txt`).
- **Registro del juego:** sin `Exception`, `Error` ni `Assert` en los tres lanzamientos.
- **Datos de otros casos:** `PF-RNF02-01` (observación fuera de las escenas jugables): ni la rueda ni el arrastre vertical mueven la lista de Créditos ni la de perfiles (0 px de diferencia); solo la barra de Créditos y las flechas ▲/▼ de perfiles. `PF-RNF23-01`: los créditos recogen la autoría de Phosphor (MIT) y de Baloo 2 y Nunito (OFL 1.1) con sus textos exactos. `PF-RF47-01`: `OE4_Z` muestra los tres niveles «Completado».

## Defectos
Ninguno: no se abrió ningún `DEF-RC5-nn` y no se creó `evidencias/rc5/defectos.md`.

## Avisos del arnés y límites
- `Start-Juego` no avisó de contaminación (`contaminacion: []`): Unity y Word estaban cerrados. Todas las llamadas salieron con 0; ninguna salida 3.
- El manifiesto coincidió antes de cada uno de los tres lanzamientos y coincide al terminar (253 archivos, misma huella).
- Un indicador verde «G-SYNC» de NVIDIA aparece fijo en la esquina superior derecha de todas las capturas (entorno, no del juego); es constante entre capturas, así que no altera las diferencias de píxeles ni ningún veredicto.
- La «respiración» de los personajes y el saludo se juzgaron con ráfagas de 10 cuadros por segundo y diferencias de píxeles por zona más la vista de los cuadros; no se midió la fase exacta de cada ciclo, solo que no coinciden.
- Que la rueda llega al juego no se comprueba de forma independiente: el arnés la envía en la posición del cursor y no hay efecto visible (el mismo límite que en rc3).
- El caso de los tres espacios (paso 9) mostró «Escribe un nombre para empezar.», el mismo mensaje que dejó el paso 8 y que persiste al teclear; para descartar que fuera el mensaje viejo se repitió tras el mensaje del paso 11 y volvió a salir (`PF-RF02-01_9_2.png`).
