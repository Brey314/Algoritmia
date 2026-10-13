# Build rc4 — procedencia

Candidato compilado tras la sesión con estudiantes del 13/10/2026. Sustituye a rc3 (que queda en `Build/Algoritmia_rc3`). **Sin etiqueta**: la etiqueta la decide Santiago.

| Dato | Valor |
|---|---|
| Fecha y hora (inicio / fin) | 13/10/2026 18:31:45 / 18:32:34 (reloj local); `buildTimeMs` 47 964 (47,96 s) |
| Rama | `ajustes-OE4` |
| HEAD al compilar | `0839352567deff1f96cad18a649365c3a379dbc9`. Después entraron `c6cc745`…`c531ac4` (merge, tesis y guion, estado de la nube): solo documentos, `git diff --stat 0839352 HEAD -- Assets Packages ProjectSettings` sale vacío |
| Árbol de trabajo | **No es el de un commit.** Cuatro archivos de `Assets/` modificados sin commit, todos de esta pasada (ver «Cambios sin commit») |
| `git diff HEAD -- Assets Packages ProjectSettings` | huella `547c20e164a76859f5dce696915e28daa044f9f82b2a0c2ce456c097c2e5a738`, igual antes del build y después de restaurar `ProjectSettings.asset`. Comando: `git diff --binary --no-textconv --no-ext-diff HEAD -- Assets Packages ProjectSettings \| sha256sum` |
| Archivos sin seguimiento en `Assets/`, `Packages/`, `ProjectSettings/` | ninguno (los 33 sin seguimiento del árbol son documentos, `pares/*.json` y capturas, fuera de lo que entra al ejecutable) |
| Unity | 6000.5.10f1, StandaloneWindows64, sin development build |
| Vía | `editor.ps1 exec build confirm=true` (API de `com.unity.pipeline`) con el Editor del proyecto abierto y la escena `Boot` limpia; la salida cayó en `Builds/StandaloneWindows64/` y se movió a `Build/Algoritmia/`. `build_status`: `Succeeded`, 0 errores, 487 avisos, de los cuales 1 `warning CS` (CS0108 de `PropShadow.DestroyObject`, ya existente) |
| SHA-256 `Algoritmia.exe` | `6cd16edfbe85284eb69cd2efe1170aa1cffdf0dbdb309c3269e2f8454811adab` (igual que rc1, rc2 y rc3: el `.exe` es solo el lanzador) |
| **Huella del contenido** de `Algoritmia_Data` | `a7569cdb6028f70aba3178b2c3db487ee90769673bebfec51b1a518d4dcfbbfa`, 223 archivos (rc3: `b8e30db6821b0107005f9eafba8a628e4a3d0a1bcf36b52a8e7c91875e291446`, que se recalculó hoy sobre `Build/Algoritmia_rc3` y dio lo mismo). Comando de `build-rc2.md`: `find . -type f -print0 \| xargs -0 sha256sum \| sed 's/ \*/  /' \| awk '{h=$1; $1=""; sub(/^ +/,""); print $0"\t"h}' \| LC_ALL=C sort \| sha256sum` |
| **Huella de la carpeta entregable** | `746a6c27dc4c74e2d0b3ef32767fd937a2af1bb1b41c5f7f4c580ba12a59bf40`, 252 archivos, sin `Datos/` ni `*_DoNotShip`. Comando: `find . -type f ! -path './Datos/*' ! -path './*_DoNotShip/*' \| LC_ALL=C sort \| tr '\n' '\0' \| xargs -0 sha256sum \| sha256sum` |

## Contenido

- Salida `C:\Dev\Algoritmia\Build\Algoritmia\`: 12 escenas (`level0..level11`), `Boot` primera. `Licencias/` con `OFL.txt` y `LICENSE-Phosphor.txt`. Sin `Datos/` (build sin ejecutar).
- Tiene una carpeta `Algoritmia_BurstDebugInformation_DoNotShip` (0,7 MiB), que se excluye de la medida y de la huella de la carpeta, como en rc2.
- Respecto a rc3, la lista de archivos es la misma salvo `Algoritmia.provenance.json`, que lo escribe la CLI `unity build` y no la API del Editor: rc3 tenía 253 archivos y rc4 252.
- De los 223 archivos de `Algoritmia_Data`, 38 cambian respecto a rc3: 8 DLL `Managed` (los `Game.*`), los 12 `level*`, 13 `sharedassets*`, `globalgamemanagers` y `globalgamemanagers.assets`, `resources.assets`, `boot.config` y `Resources/unity_builtin_extra`. De los `.resS` solo cambia `sharedassets9.assets.resS`.
- `globalgamemanagers`: 0 apariciones de `unity3d.com`.
- **Lo que no pude comprobar en el registro:** el build salió por la API del Editor y no por `unity build`, así que no hay `build.log`. `Editor.log` lo reescribió el segundo Editor de la suite (17:50), y los pasos del `BuildReport` no incluyen líneas de `UnityServicesOff` ni de `LicenseNotices`. Se comprobó por el resultado: ambos hacen fallar el build si no cumplen y el build terminó en `Succeeded`, `Licencias/` trae los dos avisos y `globalgamemanagers` no nombra `unity3d.com`.

## Tamaño (RNF-06): CUMPLE

`oe4.ps1 Tamano`: **426 930 226 bytes = 407,2 MiB = 426,9 MB decimales** (sin `Datos/` ni `*_DoNotShip`); margen de 73,1 MB frente a 500. rc3 pesó 426,6 MB: rc4 suma 0,3 MB. El `BuildReport` da `totalSizeBytes` 426 924 610 y 286,0 MB de `Texture2D` entre los 300,9 MB de activos de usuario.

## Cambios respecto a rc3 (compilado desde `f49ccc9`; `4dfcf35`, donde se cerró su pasada, no cambia `Assets/`, `Packages/` ni `ProjectSettings/`)

| INC | Qué cambia en el ejecutable |
|---|---|
| INC-152 | Aperturas del Nivel 1 más claras: `N1_Apertura`, `N1_AparicionGuia` y `N1_Hallazgo` suben 0,12 la luz de inicio y de cada parada (mínimo 0,15; se conserva el negro intencional de la línea 6 de la apertura). La cueva jugable pasa de 0,10 a 0,22 en su punto más oscuro y de 0,30 a 0,35 en el más claro |
| INC-153 | Caminata real en la escena 1.1: en la línea 7 de `N1_AparicionGuia` la familia se desplaza hacia la izquierda, de perfil, y una prueba de contenido exige un desplazamiento mínimo a todo paso de locomoción de las 18 narrativas |
| INC-154 | Abrazo y sueño de perfil: `ActionView` pasa `Hug` y `Sleep` a `Profile`, se rehacen los ocho clips `abrazar` y `dormir` de la familia y `N1_NacimientoDelFuego` fija quién mira a quién y dónde duerme cada uno |
| INC-155 | Troncos del bosque: empuje 0,75 → 0,35, rozamiento 1,4 → 3,0, el tronco no se empuja con el cursor sobre su área de clic y esa área cubre el cuerpo que dibuja `RollingLog` |
| INC-156 | Flechas curvas de «Girar»: Phosphor `arrow-arc-left` y `arrow-arc-right` (MIT) a 128 px (`ui_girar_izq`, `ui_girar_der`), con su aviso en los créditos |
| INC-157 | Vocabulario más llano para niños de quinto en las narrativas, los textos del Nivel 3 y el resumen del Nivel 2 |
| INC-158 | Aviso del botón de ayuda en la primera mecánica (`HelpButtonIntro`, `FireHelpIntro`): fondo oscuro, «Pista» iluminada encima, tiempo detenido y un solo botón «Cerrar»; sale mientras el perfil no haya confirmado la fase 1 del Nivel 1 |
| INC-159 | Menú de pausa de cinco botones: Reanudar, Reiniciar, Volver a ver la historia (oculto en la fase sin escena propia), Volver al menú de niveles y Guardar y salir (con confirmación) |
| INC-160 | Botón «Atrás» del diálogo (`DialogueRunner.Back`, `NarrativeSceneController.Back`): relee la línea anterior sin repetir sonidos ni movimientos. Disposición del cuadro: «Omitir» (260 × 76) encima de «Continuar», alineado a la derecha, y «Atrás» a la izquierda de «Continuar» en su misma fila |
| INC-151 | Solo texto: «grado quinto» en documentos; no cambia el ejecutable |

## Cambios sin commit con que se compiló

1. `Assets/Game/Scripts/Runtime/Scaffolding/HelpButtonIntro.cs`: `OnDestroy` ya no destruye el `GraphicRaycaster` con `DestroyImmediate`. Si la escena se descargaba con el aviso abierto —por ejemplo, «Reiniciar» en la fase 1 del Nivel 1—, Unity lanzaba «Destroying object multiple times». Ahora solo devuelve el tiempo; el raycaster y el `Canvas` salen con la escena.
2. `Assets/Tests/PlayMode/Levels/Fire/HelpIntroTests.cs`: prueba de regresión `FireLevel_INC158_UnaEscenaQueSeDescargaConElAvisoAbiertoDevuelveElTiempoSinErrores`.
3. `Assets/Game/Scenes/Narrative.unity`: `BotonOmitir` pasa a 260 × 76 en (−32, 108) y `BotonAtras` a (−308, 22) con 200 × 76.
4. `Assets/Tests/PlayMode/UI/NarrativeSceneTests.cs`: prueba nueva `NarrativeScene_INC160_OmitirVaEncimaDeContinuarAtrasEnSuFilaYNingunoPisaAlOtro`.

## Pruebas

- Suite completa con `suite2.ps1` sobre el árbol con los cambios 1 y 2: EditMode 785 de 786 (1 omitida por entorno, `ProfileEraser_INC34_…`), PlayMode 427 de 427, 0 fallos, cobertura 1213/1213 (`suites/rc4-previa-al-boton-atras/`). La suite precede a los cambios 3 y 4: lo que ellos afectan se volvió a correr aparte y pasó (`NarrativeScene_INC160`, `RNF03`, `RNF01`, `INC28`, `RF45`, `LevelSummary`, `NarrativeSequence` en EditMode).
- Antes de ese arreglo, `PauseMenu_HU17_ContinuarRestituyeElEstadoExacto` fallaba por el error del punto 1.

## Tras el build

- `ProjectSettings/ProjectSettings.asset` salió con `preloadedAssets` apuntando a `ControlesJugables.inputactions` (único cambio, sin `SENTIS_ANALYTICS_ENABLED`) y se restauró con `git checkout`.
- `Build/Algoritmia_rc3` conserva la carpeta `Datos/` (743 bytes) que dejó el uso de rc3; no se tocó.

## Artefactos

- `BuildReport`: `claudeDocs/tasks/OE4/evidencias/build-rc4.buildreport` (copia de `Library/LastBuild.buildreport`).
- Capturas del cuadro de diálogo con los tres botones y de las líneas del abrazo, el sueño y la caminata: `evidencias/rc4/Inc160_*.png` y `claudeDocs/tasks/Personajes/capturas/2026-10-13-inc154/`.
