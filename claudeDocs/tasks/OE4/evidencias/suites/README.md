# Evidencias de las suites de pruebas (cierre, 30/09 – 02/10/2026)

XML NUnit de las corridas del Editor de Unity (6000.5.10f1) durante el cierre de los slices, sin commit intermedio.
Cada carpeta conserva los XML tal como los dejó `claudeDocs/tasks/OE4/herramientas/editor.ps1`
(`<fecha>_<hora>_<modo>.xml`; las horas del nombre son locales, los `start-time` internos están en UTC).
En todas las suites completas de EditMode la única prueba omitida es
`ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`, que se ignora por entorno.

| Corrida | Fecha | Totales | Contexto |
|---|---|---|---|
| `rc2-final` | 01/10/2026 17:31 / 18:01 | **PlayMode 376/376 · EditMode 470 (469 + 1 omitida)** | Verificación final del código de rc2 (tercera revisión), justo antes del build rc2: PlayMode como primera corrida de una sesión del Editor recién abierta, sin EditMode antes; después EditMode. La PlayMode duró 1743,5 s según el runner (`duration` del XML, 22:31:47Z → 23:00:51Z); los 1755,5 s del JSON (`elapsedSec`) son el reloj de pared de `editor.ps1`, desde la petición (22:31:41Z) hasta el último sondeo (23:00:57Z). Cada corrida tiene su XML y el resumen JSON de `editor.ps1` con la lista de resultados. *(Corregido el 01/10/2026, 21:45: decía que la EditMode no dejó XML; sí lo dejó, en la carpeta temporal de la sesión, y se copió aquí con el JSON de la PlayMode.)* `OE4-Resultados.md` §8.1. |
| `suite2-validacion` | 02/10/2026 00:50 – 01:11 | **EditMode 471 (470 + 1 omitida) · PlayMode 376/376** | Primera suite completa con `herramientas/suite2.ps1`, en dos Editores en paralelo (21,2 min de pared; norma en `../../NORMA-PRUEBAS.md`), sobre `f6e9206` con las esperas de las pruebas PlayMode de `Game.Core` pasadas de 600 frames a 10 s. Cobertura contra `list_tests`: 847/847. `resumen.md` da el reparto: carril A (proyecto) EditMode + `Game.UI`, carril B (copia) el resto. |

*(10/10/2026)* Solo se conservan `rc2-final` y `suite2-validacion`; las demás corridas (`base`, `W1-full`, `W2-full`, `W3-review`, `W3-review2`, `W4b`, `final`, `rc2-posterior`) se retiraron y se recuperan con `git show 4e78f47:<ruta>` (ver `../README.md`). La suite de la pasada rc3 irá en `rc3/`.

Nota: los totales de PlayMode suben 332 → 351 → 363 → 364 → 376 porque cada paquete de trabajo añadió pruebas, no porque
se hayan recontado; ninguna prueba se ha omitido ni se ha relajado para llegar a verde.
