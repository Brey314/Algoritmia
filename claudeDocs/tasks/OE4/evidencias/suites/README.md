# Evidencias de las suites de pruebas (cierre, 30/09 – 01/10/2026)

XML NUnit de las corridas del Editor de Unity (6000.5.10f1) durante el cierre de los slices, sin commit intermedio.
Cada carpeta conserva los XML tal como los dejó `claudeDocs/tasks/OE4/herramientas/editor.ps1`
(`<fecha>_<hora>_<modo>.xml`; las horas del nombre son locales, los `start-time` internos están en UTC).
En todas las suites completas de EditMode la única prueba omitida es
`ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`, que se ignora por entorno.

| Corrida | Fecha | Totales | Contexto |
|---|---|---|---|
| `base` | 30/09/2026 19:19 / 19:47 | EditMode 391 (390 + 1 omitida) · PlayMode 332/332 | Línea base W0 sobre el commit `359365e`, antes de tocar nada. |
| `W1-full` | 01/10/2026 00:30 – 01:30 | EditMode 393 (392 + 1 omitida) · PlayMode 350/351, 1 fallo | Tras W1. El fallo fue `RiverLevel_RNF05_LaMemoriaQuedaBajoDosGigasConElNivel3Cargado` (2127 MB medidos en el Editor, por encima del límite de 2 GB); al repetirla sola pasa (`…_013043_playmode.xml`, 1/1). Los XML `…_011721` (1 fallo) y `…_013043` (1 verde) son esa repetición. |
| `W2-full` | 01/10/2026 03:28 – 03:56 | EditMode 427 (426 + 1 omitida) · PlayMode 363/363 | Tras W2, suite completa en verde. |
| `W3-review` | 01/10/2026 04:13 – 04:17 | 9 corridas parciales de PlayMode, 59 pruebas, todas en verde | Revisión de W3: pruebas filtradas por clase (`AssemblyPanelTests`, `WorkshopSceneTests`, `NarrativeSceneTests`, `CreditsTests`, `ConditionalNarrativeTests`, `RiverLevelJourneyTests`, `GameEndingTests`, `CharacterCaptureTests`). |
| `W3-review2` | 01/10/2026 04:29 | EditMode 429 (428 + 1 omitida) · PlayMode filtrado 4/4 | Segunda revisión de W3 tras los arreglos de la revisión (PlayMode: `CreditsTests`). |
| `final` | 01/10/2026 04:37 / 05:05 | **EditMode 433 (432 + 1 omitida) · PlayMode 364/364** | Verificación final del código, con el Editor reiniciado (el anterior llegaba a 3,9 GB de working set) y compilación sin errores. No cambia nada más después. |
| `W4b` | 01/10/2026 08:29 | **EditMode 434 (433 + 1 omitida)** | Tras la excepción de importación de `Props/Fire/Animations` (1024 px, RNF-06) y su prueba `ArtImport_RNF06_…`. Antes del cambio de la regla esa prueba nueva fallaba (rojo con 433 + 1 fallo) y tras reimportar pasa. PlayMode no se repitió en esta tarjeta. |
| `rc2-final` | 01/10/2026 17:31 / 18:01 | **PlayMode 376/376 · EditMode 470 (469 + 1 omitida)** | Verificación final del código de rc2 (tercera revisión), justo antes del build rc2: PlayMode como primera corrida de una sesión del Editor recién abierta, sin EditMode antes; después EditMode. La PlayMode duró 1743,5 s según el runner (`duration` del XML, 22:31:47Z → 23:00:51Z); los 1755,5 s del JSON (`elapsedSec`) son el reloj de pared de `editor.ps1`, desde la petición (22:31:41Z) hasta el último sondeo (23:00:57Z). Cada corrida tiene su XML y el resumen JSON de `editor.ps1` con la lista de resultados. *(Corregido el 01/10/2026, 21:45: decía que la EditMode no dejó XML; sí lo dejó, en la carpeta temporal de la sesión, y se copió aquí con el JSON de la PlayMode.)* `OE4-Resultados.md` §8.1. |
| `rc2-posterior` | 01/10/2026 21:38 | EditMode filtrada: `ProfilePaging` 9/9 · `ArtImport` 3/3 | Cambios posteriores al build rc2 (decisión D-PAG): la prueba de paginación pasa a tres perfiles por página y se corrige el comentario de `ArtImportRules.cs`. No cambian el ejecutable (`build-rc2.md`, «Cambios posteriores al build»). |

Nota: los totales de PlayMode suben 332 → 351 → 363 → 364 → 376 porque cada paquete de trabajo añadió pruebas, no porque
se hayan recontado; ninguna prueba se ha omitido ni se ha relajado para llegar a verde.
