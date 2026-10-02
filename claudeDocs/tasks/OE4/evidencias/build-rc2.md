# Build rc2 — procedencia

Candidato vigente tras rc1. No hay etiqueta ni commit: Santiago hará un único commit al final. **rc1 queda sustituido por este build** (se conserva sin tocar en `Build/Algoritmia_rc1`).

| Dato | Valor |
|---|---|
| Fecha y hora (inicio / fin del build) | 01/10/2026 18:03:59 / 18:04:43 (reloj local); 43,9 s según el BuildReport (la hora «Z» del informe del pipeline no es UTC real, valen las del reloj local) |
| Rama | `feat/cierre-de-slices-y-oe3` |
| HEAD | `359365e0a351bd2b8e64bbd0bb75b1c8cef9b2f5` |
| `git status --short \| wc -l` | 262 antes y después del build |
| Unity | 6000.5.10f1, StandaloneWindows64, Mono, sin development build |
| Vía | API del pipeline con el Editor abierto (pid 18904, puerto 7800, escena `Boot` sin cambios): `exec build {confirm, target, output_path}` → `build_status` (buildId `build_818982e2cf22`, Succeeded, 0 errores, 486 avisos) |
| SHA-256 `Algoritmia.exe` | `6cd16edfbe85284eb69cd2efe1170aa1cffdf0dbdb309c3269e2f8454811adab` (igual que rc1: el `.exe` es solo el lanzador; lo que cambia está en `Algoritmia_Data/`) |
| **Huella del contenido** de `Algoritmia_Data` | `a613b851cd05d21d9daded0ee8c93c1f483fed390941b69d71cd6c29c887a704` — SHA-256 del listado ordenado (`LC_ALL=C`) de líneas `ruta-relativa<TAB>SHA-256` de los 223 archivos (rc1: `c3787fd25343d9f41dbc73cbd44bfbd860f908a041701be345b761aeac368d0d`, también 223 archivos y las mismas rutas) |

**Huella de la carpeta entregable** (el método de la revisión de W5, que `build-rc1.md` usa para rc1; calculada el 01/10/2026 a las 21:00): `024076a1fd0f576ac35eb777fd32c8c61eb242a17c65898a11e0e7118a3057e0`, 252 archivos, sin `Datos/` ni `*_DoNotShip` (rc1: `af4278fbab6f09b0aa381b91e06a7ff3b4b909855f1e8704845f5fd0e70bb28e`). Desde `Build/Algoritmia`, en Git Bash: `find . -type f ! -path './Datos/*' ! -path './*_DoNotShip/*' | LC_ALL=C sort | tr '\n' '\0' | xargs -0 sha256sum | sha256sum`.

Comando de la huella (desde la carpeta `Algoritmia_Data`): `find . -type f -print0 | xargs -0 sha256sum | sed 's/ \*/  /' | awk '{h=$1; $1=""; sub(/^ +/,""); print $0"\t"h}' | LC_ALL=C sort | sha256sum`.

Diferencias con rc1 en `Algoritmia_Data`: 83 de 223 archivos distintos, las mismas rutas. Son 52 DLL de `Managed` (los 8 `Game.*` y 44 de paquetes, recompilados: el compilador no es determinista byte a byte), `lib_burst_generated.dll`, `globalgamemanagers*`, `boot.config`, `resources.assets`, las 12 escenas `level0..11` y los `sharedassets*.assets` (serialización); los `.resS` de textura son idénticos salvo `sharedassets1` y `sharedassets9`.

## Servicios de Unity apagados (RNF-10, decisión D3)

- El paso previo es `Game.EditorTools.UnityServicesOff` (`IPreprocessBuildWithReport`, `callbackOrder = int.MinValue`, más `IPostprocessBuildWithReport` que verifica `globalgamemanagers`). Está cargado en el Editor (comprobado por `eval`: el tipo existe e implementa la interfaz).
- **El log no trae la línea «[RNF-10] Servicios de Unity … se apagaron»**, porque esa línea solo se escribe cuando algún interruptor estaba encendido: antes del build los nueve ya estaban apagados (comprobado por `eval` sobre `RealSwitches()` tras el build: todos `False`). La evidencia de que el paso corrió es indirecta: el build terminó sin `BuildFailedException`, el paso «Postprocess built player» (11,7 s) ejecutó `Verify` y `globalgamemanagers` contiene **0** apariciones de `unity3d.com`.
- `ProjectSettings/UnityConnectSettings.asset` sigue con `m_Enabled: 0` y su huella no cambió con el build (`11251dae…c6f`; difiere de `HEAD` solo por `m_EngineDiagnosticsEnabled: 1 → 0`, cambio previo y deliberado del árbol, que **no** se toca).
- `ProjectSettings/ProjectSettings.asset`: como en rc1, el Editor añadió `preloadedAssets` con `ControlesJugables.inputactions`. Se restauró a lo que tenía antes del build desde una copia hecha antes (no con `git checkout`, que habría borrado cambios previos del árbol); huella final `b23a06c0…1e59`, igual que la de antes.

## Huellas del árbol

Mismo comando que rc1: `git diff --binary --no-textconv --no-ext-diff HEAD [-- Assets Packages ProjectSettings] | sha256sum`.

| Huella | Antes del build (18:03) | Después (18:06, tras restaurar `ProjectSettings.asset`) |
|---|---|---|
| `git diff` completo | `a7e611b082623a90eaeb34f24501fb12185e24c96b67de77ce8b4883367d75ca` | igual |
| idem, solo `Assets Packages ProjectSettings` | `f5bb1efefddc69a23fc752604f2449474f4dfaecd38b14d43fc8f057af1fd726` | igual |
| Archivos sin seguimiento, solo `Assets Packages ProjectSettings` (hash de la lista ordenada) | `14f713bba27a7c8da8bbbf24c53180c53ba6c836e90fbd5bdd01fbfcad355cef` | igual |
| Archivos sin seguimiento, todo el árbol | `144c52f5abdd464d502e583bab4a88341fde60625d5cd98c4bad6eb269cf5b6e` | `ca57c29901532e160c33f0262e56a2370b40958ae5c09a004f5a9762e4623c12` (428 archivos; cambió por documentos nuevos de otros flujos, no por el build) |

La huella de `Assets Packages ProjectSettings` identifica el código y los assets del juego: coincide antes y después. El build no dejó `Assets/Resources/PerformanceTest*.json` (el Performance Testing los crea y los borra).

**Estas huellas describen el árbol al compilar** (18:03–18:06), no el de después. La revisión final de rc2 las reprodujo esa noche, antes de los cambios del apartado siguiente; desde entonces el árbol cambió en una prueba, un script de Editor y documentos, y la huella de `Assets Packages ProjectSettings` ya no da `f5bb1efe…`. Lo que identifica el ejecutable es la **huella del contenido** de la carpeta (arriba), que no depende del árbol.

## Contenido

- Salida: `C:\Dev\Algoritmia\Build\Algoritmia\` (la API escribió en `Builds/StandaloneWindows64/`, ignorando `output_path`; se movió a `Build/Algoritmia/` sin recompilar y `Builds/` ya no existe). Builds anteriores sin tocar: `Build/Algoritmia_rc1`, `Build/Algoritmia_2026-09-21`, `Build/Algoritmia_2026-10-01_827MB`.
- **12 escenas** (`level0..level11` en `Algoritmia_Data`), `Boot` primera.
- `Licencias/` presente: `OFL.txt`, `LICENSE-Phosphor.txt`.
- **`Datos/` no existe** (rc1 tenía una `Datos/` vacía, de una ejecución posterior al build; esta salida no se ha ejecutado).
- `Algoritmia_Data/Managed` **sigue incluyendo** `Unity.AI.MCP.Runtime.dll` y `Unity.AI.Tracing.dll` (igual que rc1). Con los servicios apagados y 0 `unity3d.com` en `globalgamemanagers`, no hay conexión de Unity en el juego; se anota para decidir si se excluyen del player.
- `DirectML.dll`, `D3D12/D3D12Core.dll`, `dstorage*.dll`: presentes (decisión D3: se quedan).
- Esta vez **sí** hay una carpeta `Algoritmia_BurstDebugInformation_DoNotShip` (784 239 bytes) en la salida; no es parte del entregable y no se cuenta en el tamaño. Debe retirarse al empaquetar.
- BuildReport: `claudeDocs/tasks/OE4/evidencias/build-rc2.buildreport` (copia de `Library/LastBuild.buildreport`) y `build-rc2.build_status.json`.

## Tamaño (RNF-06, paquete < 500 MB) — CUMPLE

Carpeta completa sin `*_DoNotShip` y sin `Datos`: **478 987 619 bytes = 456,8 MiB (479,0 MB)**; margen decimal de 21 MB. Con la carpeta `DoNotShip`: 479 771 858 bytes. rc1: 478 979 915 bytes.

| Parte | MB |
|---|---|
| `Algoritmia_Data/sharedassets4.assets.resS` (texturas del N1) | 276,4 |
| `Algoritmia_Data/Managed` | 47,1 |
| `UnityPlayer.dll` | 37,3 |
| `sharedassets5.assets.resS` | 27,9 |
| `sharedassets2.assets.resS` | 18,8 |
| `DirectML.dll` | 14,0 |
| `Algoritmia_Data/resources.assets` | 9,3 |
| `MonoBleedingEdge` | 9,1 |

Por tipo de activo (BuildReport): Texture2D 340,2 MB; ComputeShader 8,8; Shader 1,4; AudioClip 1,1; Cubemap 0,5. `Props/Fire/Animations` 145,7 MB.

## Cambios posteriores al build

Ninguno altera el ejecutable: `Build/Algoritmia` sigue con la huella del contenido de arriba (`024076a1…57e0`
para la carpeta entregable, `a613b851…c887a704` para `Algoritmia_Data`), y todo lo que se verificó sobre el exe vale
para rc2 tal como se compiló.

**Código, por la decisión D-PAG** (`OE4-Resultados.md` §8.10), el 01/10/2026 hacia las 21:37:

| Archivo | Qué cambió | Por qué no toca el exe |
|---|---|---|
| `Assets/Tests/EditMode/UI/ProfilePagingTests.cs` | La prueba de páginas pasa de cuatro a tres perfiles por página: `ProfilePaging_RF02_ContarPaginasDeTresPerfiles`, con 8 perfiles en 3 páginas, que es lo que se entrega y se ve en el exe (B2). Antes se llamaba `…ContarPaginasDeCuatroPerfiles` y usaba `perPage = 4` | Es una prueba de EditMode: no entra al player |
| `Assets/Game/Scripts/Editor/ArtImportRules.cs` | Solo el comentario de la excepción de `Props/Fire/Animations`: «67 PNG (134 entradas del BuildReport)» y «866,7 MB (826,6 MiB)», donde decía 134 texturas y 827 MB | Es un script de Editor (`Game.EditorTools`) y el cambio es un comentario; la regla de importación no cambia |

Las dos pasan tras el cambio: `ProfilePaging` 9/9 y `ArtImport` 3/3 (EditMode filtrada, 21:38;
`suites/rc2-posterior/`).

**Documentos.** Después del build cambiaron los documentos del cierre, sin tocar el juego: los del carril OE4
(`OE4-Resultados.md` §8, `casos.md`, `Hoja-HUM.md`, `todo.md`, `herramientas/oe4-USO.md` y las notas de
`evidencias/rc2/` y `evidencias/suites/`), `Assets/Game/Art/Inventario.md` y los demás documentos de `claudeDocs/`
que actualizan otros flujos.

## Cierre

El Editor se cierra ordenadamente al terminar esta tarea.
