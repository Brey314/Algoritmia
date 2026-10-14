# Build rc5 — procedencia

Candidato compilado el 14/10/2026 desde `9dfe513` más la corrección de DEF-RC4-01, que **aún no está en un commit**. Sustituye a rc4 (compilado el 13/10/2026 desde `0405eba`), que queda en `Build/Algoritmia_rc4`. **Sin etiqueta**: la etiqueta la decide Santiago.

**Sin pasada de casos ni medidas de RNF-04 y RNF-05 por decisión del usuario.** Este build se compiló y se pesó; no se ejecutó ni se midió como ejecutable, y tampoco se corrió la suite de pruebas (EditMode ni PlayMode) sobre este árbol.

| Dato | Valor |
|---|---|
| Origen | `ca6fd91` (`9dfe513` + corrección DEF-RC4-01; el build se hizo sobre ese mismo árbol antes del commit, huella del diff `dc025f48…0238877`) |
| Fecha y hora | 14/10/2026, a las 11:37 (reloj local; fechas de los archivos de salida 11:37); `buildTimeMs` 59 164 (59,2 s). Los sellos UTC de la API del Editor salen desfasados y no se usan |
| Rama | `ajustes-OE4` |
| HEAD al compilar | `9dfe51348b5774ee1c59400a2c1474801d36dec5` (`9dfe513`) |
| Árbol de trabajo | **`9dfe513` más un solo archivo con cambios en `Assets/`: `Assets/Game/Scenes/LevelSummary.unity`** (la corrección). Ningún archivo sin seguimiento en `Assets/`, `Packages/` ni `ProjectSettings/`. Fuera de esas carpetas había las capturas nuevas (`evidencias/rc5/`), el `BuildReport` y un `docs/Trabajo_de_Grado_2026_ICONTEC_IEEE (2).docx` borrado en el árbol ajeno a esta tarea; nada de eso entra al build |
| `git diff HEAD -- Assets Packages ProjectSettings` | huella `dc025f481636d00910bad70953329ff1eeb5417d4cd0d0963f806940c0238877`, igual antes del build y después de restaurar `ProjectSettings.asset`. Comando: `git diff --binary --no-textconv --no-ext-diff HEAD -- Assets Packages ProjectSettings \| sha256sum`. Con ella, quien haga el commit comprueba que lo que commitea es lo que se compiló |
| Archivos sin seguimiento en `Assets/`, `Packages/`, `ProjectSettings/` | ninguno (huella de la lista vacía, `e3b0c442…`). Comando: `git ls-files --others --exclude-standard -- Assets Packages ProjectSettings \| sha256sum` |
| Unity | 6000.5.10f1, StandaloneWindows64, sin development build |
| Vía | `editor.ps1 exec build confirm=true` (API de `com.unity.pipeline`) con el Editor del proyecto abierto y la escena `Boot` limpia; la salida cayó en `Builds/StandaloneWindows64/` y se movió a `Build/Algoritmia/`. `build_status`: `Succeeded`, 0 errores, 487 avisos (cifra de la API; no se desglosó cuántos son `warning CS`) |
| SHA-256 `Algoritmia.exe` | `6cd16edfbe85284eb69cd2efe1170aa1cffdf0dbdb309c3269e2f8454811adab` (igual que rc1 a rc4: el `.exe` es solo el lanzador) |
| **Huella del contenido** de `Algoritmia_Data` | `e9ab154985d3a3d97a4ac5c558e494c677461d4c7e0c289222b419b28c2c6e05`, 223 archivos (rc4: `d6d35bd4…930644`). Comando: `find . -type f -print0 \| xargs -0 sha256sum \| sed 's/ \*/  /' \| awk '{h=$1; $1=""; sub(/^ +/,""); print $0"\t"h}' \| LC_ALL=C sort \| sha256sum`, dentro de `Algoritmia_Data` |
| **Huella de la carpeta entregable** | `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3`, 252 archivos, sin `Datos/` ni `*_DoNotShip` (rc4: `21a35e0b…2b2076`). Comando: `find . -type f ! -path './Datos/*' ! -path './*_DoNotShip/*' \| LC_ALL=C sort \| tr '\n' '\0' \| xargs -0 sha256sum \| sha256sum` |

## Contenido

- Salida `C:\Dev\Algoritmia\Build\Algoritmia\`: 12 escenas (`level0..level11`), `Boot` primera. `Licencias/` con `OFL.txt` y `LICENSE-Phosphor.txt`. Sin `Datos/`: nadie ha ejecutado este build.
- Tiene una carpeta `Algoritmia_BurstDebugInformation_DoNotShip` (0,7 MiB), que se excluye de la medida y de la huella de la carpeta, como en rc2 a rc4.
- `globalgamemanagers`: 0 apariciones de `unity3d.com`.
- **Lo que no pude comprobar en el registro:** el build salió por la API del Editor y no por `unity build`, así que no hay `build.log`. Se comprobó por el resultado: `UnityServicesOff` y `LicenseNotices` hacen fallar el build si no cumplen y el build terminó en `Succeeded`, `Licencias/` trae los dos avisos y `globalgamemanagers` no nombra `unity3d.com`.

## Tamaño (RNF-06): CUMPLE

`oe4.ps1 Tamano`: **426 935 666 bytes = 407,2 MiB = 426,9 MB decimales** (sin `Datos/` ni `*_DoNotShip`); margen de **73,1 MB** frente a 500. Desglose: `Algoritmia_Data` 341,1 MiB, raíz 52,8, `MonoBleedingEdge` 8,7, `D3D12` 4,5. Con `*_DoNotShip`, 407,9 MiB. El `BuildReport` da `totalSizeBytes` 426 930 050. rc4 pesó 426 932 086 bytes: este suma 3 580.

## Cambios respecto a rc4 (compilado desde `0405eba`)

Entre `0405eba` y `9dfe513` los únicos archivos de `Assets/` que cambian son los de `86a9898`; a ellos se suma la corrección sin commit.

| Id | Qué cambia en el ejecutable |
|---|---|
| DEF-RC3-01 (`86a9898`) | Avatar del resumen de nivel: es Algoritm en la forma del nivel terminado —el prefab del guía llena el hueco `Avatar`, declarado en el asset de mensajes de cada nivel (`LevelSummaryMessages.Guide`)—; el círculo con el texto «Algoritm» desaparece |
| DEF-RC4-01 (sin commit) | Resumen de nivel: la tarjeta del Nivel 3 ya cabe en 1080 px. En `LevelSummary.unity`, el espaciado entre bloques de `Tablilla` baja de 48 a 32 y el ancho preferido del texto del recuadro verde (`FilaHabilidad/Habilidad/Texto`) sube de 710 a 900, de modo que la habilidad del Nivel 3 ocupa dos líneas y no tres. Alto de la tarjeta a 1920 × 1080: Nivel 3, de 1108 a 989 px (45,5 px de margen arriba y abajo); Niveles 1 y 2, de 991 a 927 px. Los textos y las fuentes no cambian |

## Verificación de DEF-RC4-01 (en el Editor, no en el ejecutable)

Capturas a 1920 × 1080 del resumen de los tres niveles, con Play desde `Boot` y un perfil temporal (`RcTmp`, ya borrado de `Datos/`): `evidencias/rc5/DEF-RC4-01_N1.png`, `_N2.png` y `_N3.png`. Alto medido de la tarjeta: N1 927 px, N2 927 px, N3 989 px. Con las variantes de texto «con fallo» (la que cuenta que algo falló y se corrigió) la tarjeta mide 989 px en los tres niveles; antes medía 1053 en el 1 y el 2 y 1108 en el 3. Son medidas del Editor sobre los textos de los assets: **no hay captura sobre el ejecutable rc5**.

## Tras el build

- `ProjectSettings/ProjectSettings.asset` salió con `preloadedAssets` apuntando a `ControlesJugables.inputactions` (único cambio) y se restauró con `git checkout`. El Editor sigue abierto: si guarda los ajustes del proyecto, el cambio vuelve a aparecer y no se commitea.
- No apareció `Assets/Resources/`.

## Artefactos

- `BuildReport`: `claudeDocs/tasks/OE4/evidencias/build-rc5.buildreport` (copia de `Library/LastBuild.buildreport`).
- Capturas de DEF-RC4-01: `claudeDocs/tasks/OE4/evidencias/rc5/`.
- El rc4 anterior, sin tocar, en `Build/Algoritmia_rc4`.
