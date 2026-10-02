# Build rc1 — procedencia

**Sustituido por rc2 (01/10/2026 18:04; ver `build-rc2.md`). La carpeta se conserva en `Build/Algoritmia_rc1`.** Candidato que evaluó W5. No hay etiqueta ni commit: Santiago hará un único commit al final.

## Intento descartado (W4, 05:17): 826,6 MiB, no cumple RNF-06

El primer build rc1 pesó **866 718 251 bytes = 826,6 MiB (866,7 MB)** frente al tope de RNF-06 (< 500 MB) y **se descartó**. SHA-256 de su `Algoritmia.exe`: `6cd16edf…adab`. Causa: `Art/Props/Fire/Animations` sumaba 533 MB en 134 entradas de textura (67 PNG: 33 de humo de 1123×1933, 13 de fuego normal de 2144×2108 y 21 de fuego cenital de 500×278) importadas sin comprimir a tamaño completo; ya eran un dibujo por clave, sin duplicados, así que no había arreglo sin pérdida. Se conserva renombrado como `Build/Algoritmia_2026-10-01_827MB` (no se borró).

**Corrección (W4b):** excepción documentada en `ArtImportRules`: las texturas bajo `Assets/Game/Art/Props/Fire/Animations/` se importan con `maxTextureSize` 1024, **sin comprimir** (se mantiene el motivo de la regla general, la rejilla de bloques de 4×4). En pantalla el fuego normal mide como mucho ~650 px y el humo menos de 300. Prueba nueva `ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir` (rojo → verde) y `ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir` acepta esa carpeta y solo esa. Los 67 `.meta` quedaron con `maxTextureSize: 1024` en `DefaultTexturePlatform` (`overridden: 0` en Standalone). EditMode completo: 434 = 433 + 1 omitida, 0 fallos (`evidencias/suites/W4b/`).

## Candidato vigente

| Dato | Valor |
|---|---|
| Fecha y hora (inicio / fin del build) | 01/10/2026 08:29:53 / 08:30:10 (UTC-5); 16,8 s según el BuildReport (la hora «Z» del informe del pipeline no es UTC real, valen las del reloj local) |
| Rama | `feat/cierre-de-slices-y-oe3` |
| HEAD | `359365e0a351bd2b8e64bbd0bb75b1c8cef9b2f5` |
| `git status --short \| wc -l` | 208 antes y después del build (los documentos los mueve el otro flujo) |
| Unity | 6000.5.10f1 (3bd4f66ad299), StandaloneWindows64, Mono, sin development build |
| Vía | API del pipeline con el Editor abierto: `exec build {confirm, target, output_path}` → `build_status` (buildId `build_33f03cc1b1e3`, Succeeded, 0 errores, 486 avisos) |
| SHA-256 `Algoritmia.exe` | `6cd16edfbe85284eb69cd2efe1170aa1cffdf0dbdb309c3269e2f8454811adab` (idéntico al del intento descartado: el `.exe` es solo el lanzador; lo que cambia está en `Algoritmia_Data/`) |

## Huellas del árbol

Comando: `git diff --binary --no-textconv --no-ext-diff HEAD [-- Assets Packages ProjectSettings] | sha256sum` (sin `--no-textconv`, `git diff` falla con los `.docx`). La huella de `Assets Packages ProjectSettings` es la que identifica el código y los assets del juego; los documentos de `claudeDocs/` cambian por el otro flujo.

| Huella | Antes del build (08:29) | Después del build y de restaurar `ProjectSettings` (08:31) |
|---|---|---|
| `git diff --binary HEAD` completo | `9f9c15485fb0a41e4a4f4cb4c86b262700ca2fe295134ced31c17671291ae01c` | `9f9c15485fb0a41e4a4f4cb4c86b262700ca2fe295134ced31c17671291ae01c` |
| idem, solo `Assets Packages ProjectSettings` | `659bea0183be9c99c49294be3831d366116801143d434916e44179bac5658468` | `659bea0183be9c99c49294be3831d366116801143d434916e44179bac5658468` |
| Lista ordenada de `git ls-files --others --exclude-standard` | `eb2e3e718a46749c3e4cb1a708012084b7aa27ed67c26debc6dece2235bf44fa` (84 archivos) | `eb2e3e718a46749c3e4cb1a708012084b7aa27ed67c26debc6dece2235bf44fa` |
| idem, solo `Assets Packages ProjectSettings` | `d687417893b5d5db1eab61e0bf827827908f9740b948e1760a6682a6a9db6173` | `d687417893b5d5db1eab61e0bf827827908f9740b948e1760a6682a6a9db6173` |

**Nota ProjectSettings.** Igual que en el primer intento, durante el build Unity reescribió `ProjectSettings/ProjectSettings.asset` (`preloadedAssets` con `ControlesJugables.inputactions`) y `ProjectSettings/UnityConnectSettings.asset` (`m_Enabled: 0 -> 1`). Ambos se restauraron con `git checkout` (ruido del Editor, no cambio de contenido) y las huellas «después» coinciden con las «de antes». El player ya estaba empaquetado con esa configuración.

**Estas huellas del árbol ya no se reproducen.** El árbol siguió cambiando después del build. Primero cambió `Assets/Game/Art/Inventario.md` a las 08:49, que es un documento, y la revisión de W5 midió entonces `acd75243…` para `Assets Packages ProjectSettings`. Después entró el código de rc2. El contenido de rc1 no cambió, como prueba la huella siguiente.

## Huella del contenido (pedida por la revisión de W5)

El SHA-256 del `.exe` no identifica el build: es el lanzador, idéntico en el intento descartado, en rc1 y en rc2. Lo que identifica rc1 es su contenido.

| Huella | Valor | Método |
|---|---|---|
| Carpeta entregable (252 archivos, sin `Datos/`) | `af4278fbab6f09b0aa381b91e06a7ff3b4b909855f1e8704845f5fd0e70bb28e` | El de la revisión de W5 (14:28): SHA-256 del listado de SHA-256 de los archivos, ordenado por ruta. Desde la carpeta, en Git Bash (donde `sha256sum` escribe `<hash> *./<ruta>`): `find . -type f ! -path './Datos/*' \| LC_ALL=C sort \| tr '\n' '\0' \| xargs -0 sha256sum \| sha256sum` |
| `Algoritmia_Data` (223 archivos) | `c3787fd25343d9f41dbc73cbd44bfbd860f908a041701be345b761aeac368d0d` | El de `build-rc2.md`: listado `ruta<TAB>SHA-256` ordenado con `LC_ALL=C` |

- Se recalcularon el 01/10/2026 a las 21:00 sobre `Build/Algoritmia_rc1`, que es la misma carpeta renombrada antes del build rc2, y dieron los mismos valores. Ningún archivo es posterior a las 08:30 del build, el tamaño sigue en 478 979 915 B y `Datos/` está vacía.
- Con los mismos métodos, rc2 da `024076a1fd0f576ac35eb777fd32c8c61eb242a17c65898a11e0e7118a3057e0` para la carpeta, sin `Datos/` ni `*_DoNotShip`, y `a613b851…c887a704` para `Algoritmia_Data` (`build-rc2.md`, `OE4-Resultados.md` §8.1).
- **La SUITE PlayMode completa no se corrió sobre el árbol de rc1.** El 364/364 de W4 es anterior al cambio de importación de W4b, y después solo se corrieron PlayMode dirigidas. rc2 sí la tiene: 376/376, en `suites/rc2-final/`.

## Contenido

- Salida: `C:\Dev\Algoritmia\Build\Algoritmia\` (la API ignoró `output_path` y escribió en `Builds/StandaloneWindows64/`; se movió a `Build/Algoritmia/` sin recompilar y `Builds/` ya no existe). Builds anteriores sin tocar: `Build/Algoritmia_2026-09-21` (217 MB) y `Build/Algoritmia_2026-10-01_827MB` (intento descartado).
- **12 escenas** (`level0..level11` en `Algoritmia_Data`), orden de EditorBuildSettings, `Boot` primera.
- `Licencias/` presente: `OFL.txt`, `LICENSE-Phosphor.txt`.
- `Datos/` **no existe** (nace en la primera ejecución).
- `DirectML.dll`, `D3D12/D3D12Core.dll`, `dstorage*.dll`: presentes (decisión D3: se quedan).
- No hay carpeta `*_DoNotShip` en la salida de este build.
- BuildReport: `claudeDocs/tasks/OE4/evidencias/build-rc1.buildreport` (copia de `Library/LastBuild.buildreport`) y `build-rc1.build_status.json` (informe completo con archivos y activos empaquetados), ambos sustituidos por los de este build.

## Tamaño (RNF-06, paquete < 500 MB) — CUMPLE

Carpeta completa (sin `Datos`, que no existe): **478 979 915 bytes = 456,8 MiB (479,0 MB)**. Cumple tanto con 500 MB decimales como con 500 MiB; el margen decimal es de 21 MB. El intento descartado pesaba 866,7 MB; el build de 21/09, 217 MB.

| Parte | MB | MiB |
|---|---|---|
| `Algoritmia_Data/sharedassets4.assets.resS` (texturas del N1) | 276,4 | 263,6 |
| `Algoritmia_Data/Managed` | 47,1 | 44,9 |
| `UnityPlayer.dll` | 37,3 | 35,6 |
| `sharedassets5.assets.resS` | 27,9 | 26,6 |
| `sharedassets2.assets.resS` | 18,8 | 17,9 |
| `DirectML.dll` | 14,0 | 13,4 |
| `Algoritmia_Data/resources.assets` | 9,3 | 8,8 |
| `MonoBleedingEdge` | 9,1 | 8,7 |

Por tipo de activo empaquetado (BuildReport): Texture2D 340,2 MB (antes 727,8); ComputeShader 8,8; Shader 1,4; AudioClip 1,1; Cubemap 0,5.

Texturas por carpeta: `Art/Props/Fire` 225,7 MB, de ellos **`Props/Fire/Animations` 145,7 MB (67 texturas; antes 533 MB)**; `Art/Environments/Fire` 35,3; `Environments/Wheel` 18,7; `Environments/Narrative` 12,4; `Props/Wheel` 11,3; `Characters/Algoritm` 7,1. Lo que queda grande en `Props/Fire` sin animación son los props del N1 (`prop_n1_silex`, `hoja`, `pedernal`, `monton_hojas*`, 16 MB cada uno): siguen a 4096 como pide la regla general.

## Pendiente para W5

*(Resuelto. La memoria se midió en W5 y en rc2 (`OE4-Resultados.md` §4.2 y §8.6). El fuego y el humo aparecen en las fotos a 1080p de GP1, GP2 y GP3. La PlayMode completa se corrió sobre rc2, como dice la sección anterior.)*

- RNF-05 (memoria < 2 GB): las texturas del fuego se cargan, ahora a 1024. Medir sobre este build.
- PlayMode no se repitió tras el cambio de importación (solo EditMode completo): conviene correr las PlayMode del fuego del N1, las narrativas con fuego y `RNF-21` antes de cerrar.
- Mirar a ojo el fuego y el humo a 1080p en el build (la reducción de 2144 → 1024 en el fuego normal es 2,1× y se espera invisible a ≤ 650 px en pantalla).

## Cierre

El Editor **se deja abierto** en esta tarea (lo usa el revisor).
