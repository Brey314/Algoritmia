# Build rc3 — procedencia

Candidato de la pasada OE4 del 10/10/2026. Sustituye a rc2. Sin etiqueta: la etiqueta la decide Santiago.

| Dato | Valor |
|---|---|
| Fecha y hora (inicio / fin) | 10/10/2026 17:24:53 / 17:26:31 (reloj local); `unity build` 1 min 39 s |
| Rama | `feat/personajes-animados` |
| HEAD | `f49ccc9cebc246c77975bccec88812391e249716` |
| `git status --short` antes del build | vacío (árbol limpio) |
| `git diff HEAD` | vacío (0 bytes) antes y después; tras el build `ProjectSettings.asset` no figuraba cambiado, se corrió `git checkout -- ProjectSettings/ProjectSettings.asset` igualmente |
| Archivos sin seguimiento al compilar | ninguno (los `evidencias/rc3/S-INSP/*.txt` que aparecen después son de otro flujo, posteriores al build) |
| Unity | 6000.5.10f1 (changeset 3bd4f66ad299), StandaloneWindows64, sin development build |
| Vía | `unity build --target StandaloneWindows64 -o "Build/Algoritmia/Algoritmia.exe" --log-file build.log` con los dos Editores cerrados (cierre ordenado con `unity close`); resultado «Build Finished, Result: Success.», `outcome: success` en `Algoritmia.provenance.json` (revision = HEAD, `dirty: false`) |
| SHA-256 `Algoritmia.exe` | `6cd16edfbe85284eb69cd2efe1170aa1cffdf0dbdb309c3269e2f8454811adab` (igual que rc1/rc2: el `.exe` es solo el lanzador) |
| **Huella del contenido** de `Algoritmia_Data` | `b8e30db6821b0107005f9eafba8a628e4a3d0a1bcf36b52a8e7c91875e291446` (comando de `build-rc2.md`) |
| **Huella de la carpeta entregable** | `616128366bf1fe15fcc604d14322ec98898c395f5ab435cbb02d589ad776bb9f`, 253 archivos (incluye `Algoritmia.provenance.json`, nuevo respecto a rc2), sin `Datos/` ni `*_DoNotShip` |

## Contenido

- Salida `C:\Dev\Algoritmia\Build\Algoritmia\`; 12 escenas (`level0..level11`), `Boot` primera. `Licencias/` con `OFL.txt` y `LICENSE-Phosphor.txt`. Sin `Datos/` (build sin ejecutar). **Sin** carpeta `*_DoNotShip` en esta salida.
- Se borró antes el build de medida viejo de `Build/Algoritmia`; los demás builds de `Build/` no se tocaron.
- `globalgamemanagers`: 0 apariciones de `unity3d.com`. El log no trae la línea «[RNF-10] Servicios de Unity… se apagaron» (los interruptores ya estaban apagados, como en rc2).

## Tamaño (RNF-06) — CUMPLE

`oe4.ps1 Tamano`: **426 633 111 bytes = 406,9 MiB = 426,6 MB decimales** (sin `Datos/` ni `*_DoNotShip`); margen de 73,4 MB frente a 500. Igual que la medida del 09/10 sobre `4062ae3` (420,6 MB) más las imágenes nuevas.
BuildReport (build.log): Complete build size 406,9 mb; Textures 273,1 mb (95 % de los activos de usuario); User assets 287,6 mb.

## Avisos del build.log

0 errores `error CS`. Avisos: 4 líneas `warning CS` (ver `build.log`, ignorado por git, en la raíz del proyecto) y 489 líneas que contienen «warning» en total (rc2: 486 avisos del pipeline). Ninguno detuvo el build.

## Artefactos

- BuildReport: `claudeDocs/tasks/OE4/evidencias/build-rc3.buildreport` (copia de `Library/LastBuild.buildreport`).
- `build.log` queda en la raíz del proyecto (ignorado).

## Comprobación tras las sesiones (10/10/2026, 17:58)

Tras S-INI, S-N1 y S-RNF (siete lanzamientos del `.exe`), con `Preparar-Datos -SinCarpeta` ya aplicado, la huella de la carpeta entregable sigue en `616128366bf1fe15fcc604d14322ec98898c395f5ab435cbb02d589ad776bb9f` (253 archivos): el ejecutable no cambió. `Build/Algoritmia/Datos/` no existe. `git status` limpio salvo los documentos de evidencias. Las cifras de RNF-04/05/06/10 están en `rc3/S-RNF/resumen.md`.
