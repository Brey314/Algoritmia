# S-DOC · rc3 · resumen

- **Versión:** rc3, HEAD `f49ccc9`, SHA-256 `6cd16edf…adab`. 10/10/2026, 20:05–20:20. Cinco lanzamientos (`S-DOC-rc3` … `S-DOC5-rc3`). Semillas: `S-N1_OE4N1`, `S-N2_OE4_B`, `S-N3_OE4_C` (JSON de las sesiones de esta pasada) y `OE4_Z`.
- **Casos:** 9. **Veredictos: P 9 · PD 0 · F 0 · B 0 · NA 0.** Detalle en `veredictos.csv`.

## Informe docente
- Carga de `TeacherReport` 0,029 s. Cifras exactas de `OE4_Z` (7 filas) y de OE4N1, OE4_B y OE4_C iguales a las de S-N1, S-N2C y S-N3; «Sin datos», nunca 0. El informe no cambia ningún JSON (`LastWriteTime` igual).
- **DEF-RC3-05 confirmado, Menor:** en la cabecera de la tabla los iconos de «Errores corregidos» (herradura) y «Tiempo de resolución» (reloj de arena) cruzan la primera línea del texto; los de «Intentos» y «Pasos utilizados» quedan sueltos por encima, muy pequeños. Texto y cifras siguen legibles; el botón «Volver al menú» parte el texto en dos líneas. `PF-RF46-01_1_cabeceras-x2.png`.

## Borrado
- Papelera del panel: confirmación con «Conservar»/«Borrar»; «Conservar» no toca nada; «Borrar» quita la fila y el archivo. Informe: «Cancelar»/«Eliminar datos» con aviso de irreversibilidad; sin perfiles, «Todavía no hay perfiles registrados en este equipo.».
- Residuos de «OE4N1» y «OE4_C»: ninguno en la carpeta del build, LocalLow, registro ni `Player.log` (solo en la foto previa que guarda el arnés en `%TEMP%`). Fuera de la carpeta quedan `Player.log` y los valores de sesión de Unity en `HKCU` (observación de portabilidad).
- Ruta de respaldo: con un archivo `Datos` en lugar de la carpeta, OE4R se crea en LocalLow, se lista y se borra; «Salir» avisa con la ruta entera sin montarse sobre los botones (~8 px de hueco). Se restauró `Datos/`.

## JSON (INSP, PF-RNF09-01)
- 14 JSON de perfil: solo `name`, `reachedLevel` y `phases[level, phase, attempts, correctedErrors, stepsUsed, resolutionSeconds]`.

## Observaciones sin defecto de severidad
- DEF-RC3-11 (Observación): «Eliminar datos» se parte en dos líneas dentro de su botón; la lista de «¿Quién juega?» con cuatro perfiles muestra tres y ▼.
- Cargas 0,013–0,058 s; log de los cinco lanzamientos sin `Exception`.
- Limitación: el arnés lanza con la carpeta `Datos/` del build; al terminar se restauró el contenido previo de `Build/Algoritmia/Datos`.
