# Build rc4 — procedencia

Candidato recompilado el 13/10/2026 (noche) desde `0405eba`, tras corregir en código los defectos de rc3 que se habían dejado para rc4 (DEF-RC3-05, -09, -10 y -11) y los retoques de narrativa de INC-154. **Sustituye al rc4 compilado a las 18:31 desde `0839352`** (426,9 MB, que nunca se etiquetó y quedó en `Build/Algoritmia_rc4a`; ver «rc4 previo»). rc3 sigue en `Build/Algoritmia_rc3`. **Sin etiqueta**: la etiqueta la decide Santiago.

| Dato | Valor |
|---|---|
| Fecha y hora | 13/10/2026, de 23:02 a 23:03 (reloj local; fechas de los archivos de salida 23:02:54 a 23:03:03); `buildTimeMs` 40 827 (40,8 s). Los sellos UTC de la API del Editor salen desfasados y no se usan |
| Rama | `ajustes-OE4` |
| HEAD al compilar | `0405ebae76dde5a95f03b9884fdc6d4ddfe802a1` (`0405eba`) |
| Árbol de trabajo | **Es el del commit en lo que entra al ejecutable.** `git diff HEAD -- Assets Packages ProjectSettings` vacío y ningún archivo sin seguimiento en esas tres carpetas. Fuera de ellas había una captura nueva (`Personajes_N1_NacimientoDelFuego_L16_v3.png`) y las evidencias de la suite, que no entran al build |
| `git diff HEAD -- Assets Packages ProjectSettings` | huella `e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855` (la de la entrada vacía), igual antes del build y después de restaurar `ProjectSettings.asset`. Comando: `git diff --binary --no-textconv --no-ext-diff HEAD -- Assets Packages ProjectSettings \| sha256sum` |
| Archivos sin seguimiento en `Assets/`, `Packages/`, `ProjectSettings/` | ninguno (huella de la lista vacía, la misma `e3b0c442…`). Comando: `git ls-files --others --exclude-standard -- Assets Packages ProjectSettings \| sha256sum` |
| Unity | 6000.5.10f1, StandaloneWindows64, sin development build |
| Vía | `editor.ps1 exec build confirm=true` (API de `com.unity.pipeline`) con el Editor del proyecto abierto y la escena `Boot` limpia, justo después de la suite completa; la salida cayó en `Builds/StandaloneWindows64/` y se movió a `Build/Algoritmia/`. `build_status`: `Succeeded`, 0 errores, 486 avisos (cifra de la API; no se desglosó cuántos son `warning CS`) |
| SHA-256 `Algoritmia.exe` | `6cd16edfbe85284eb69cd2efe1170aa1cffdf0dbdb309c3269e2f8454811adab` (igual que rc1 a rc3 y que el rc4 previo: el `.exe` es solo el lanzador) |
| **Huella del contenido** de `Algoritmia_Data` | `d6d35bd44cf9f4f3a296f948ec93833310e893342d241b545f63076b57930644`, 223 archivos (rc4 previo: `a7569cdb6028f70aba3178b2c3db487ee90769673bebfec51b1a518d4dcfbbfa`, que se recalculó hoy sobre `Build/Algoritmia_rc4a` y dio lo mismo). Comando: `find . -type f -print0 \| xargs -0 sha256sum \| sed 's/ \*/  /' \| awk '{h=$1; $1=""; sub(/^ +/,""); print $0"\t"h}' \| LC_ALL=C sort \| sha256sum`, dentro de `Algoritmia_Data` |
| **Huella de la carpeta entregable** | `21a35e0b19d33530ab317f9e59089a601a2c38422793ee1bb5868fb0d52b2076`, 252 archivos, sin `Datos/` ni `*_DoNotShip`. Comando: `find . -type f ! -path './Datos/*' ! -path './*_DoNotShip/*' \| LC_ALL=C sort \| tr '\n' '\0' \| xargs -0 sha256sum \| sha256sum`. Las dos huellas se recalcularon **después** de las cuatro sesiones sobre el ejecutable y no cambiaron |

## Contenido

- Salida `C:\Dev\Algoritmia\Build\Algoritmia\`: 12 escenas (`level0..level11`), `Boot` primera. `Licencias/` con `OFL.txt` y `LICENSE-Phosphor.txt`. Sin `Datos/`: las sesiones lo crearon y se quitó con `Preparar-Datos -SinCarpeta`.
- Tiene una carpeta `Algoritmia_BurstDebugInformation_DoNotShip` (0,7 MiB), que se excluye de la medida y de la huella de la carpeta, como en rc2.
- `globalgamemanagers`: 0 apariciones de `unity3d.com`.
- **Lo que no pude comprobar en el registro:** el build salió por la API del Editor y no por `unity build`, así que no hay `build.log`, y `Editor.log` lo reescribe el segundo Editor de la suite. Se comprobó por el resultado: `UnityServicesOff` y `LicenseNotices` hacen fallar el build si no cumplen y el build terminó en `Succeeded`, `Licencias/` trae los dos avisos y `globalgamemanagers` no nombra `unity3d.com`.

## Tamaño (RNF-06): CUMPLE

`oe4.ps1 Tamano`: **426 932 086 bytes = 407,2 MiB = 426,9 MB decimales** (sin `Datos/` ni `*_DoNotShip`); margen de **73,1 MB** frente a 500. Desglose: `Algoritmia_Data` 341,1 MiB, raíz 52,8, `MonoBleedingEdge` 8,7, `D3D12` 4,5. Con `*_DoNotShip`, 407,9 MiB. El `BuildReport` da `totalSizeBytes` 426 926 470. rc3 pesó 426,6 MB y el rc4 previo 426,9 MB: este suma 1 856 bytes.

## Cargas de escena (RNF-04): CUMPLE

Sobre el ejecutable, con la línea `RNF-04: «<escena>» cargó en N s` que `SceneLoader` escribe en el `Player.log`. Cuatro lanzamientos (`S-RNF-rc4-a` a `-d`), 46 cargas, a 1920 × 1080 sin marco, con los dos Editores de Unity cerrados. **El peor valor de todo el recorrido fue 0,403 s** (la primera `Narrative`, que arma el cuadro de diálogo y los personajes); el límite es 10 s.

| Escena | n | Peor | Media |
|---|---|---|---|
| `Boot` → `MainMenu` (desde `Start-Process`) | 2 | 2,76 s | 2,6 s |
| `MainMenu` | 8 | 0,139 s | 0,084 s |
| `LevelSelect` | 9 | 0,042 s | 0,025 s |
| `Credits` | 1 | 0,037 s | 0,037 s |
| `Narrative` | 17 | 0,403 s | 0,082 s |
| `LevelSummary` | 1 | 0,024 s | 0,024 s |
| `TeacherReport` | 1 | 0,036 s | 0,036 s |
| `Level1_Cave` | 1 | 0,045 s | 0,045 s |
| `Level2_Forest` | 2 | 0,030 s | 0,022 s |
| `Level2_Workshop` | 2 | 0,042 s | 0,028 s |
| `Level2_Maze` | 2 | 0,037 s | 0,026 s |
| `Level3_River` | 2 | 0,047 s | 0,031 s |

- Cubre las 12 escenas del build. `Boot` no pasa por `SceneLoader`: su cota es de `Start-Process` a la línea de `MainMenu` (lanzamientos `b` 2,76 s y `d` 2,46 s). Del primero (`a`, con el sistema en frío) no se guardó la hora de arranque; la línea de `MainMenu` salió a lo sumo 9 s después de apartar el registro anterior.
- Cada jugable se midió dos veces: la entrada y un «Reiniciar» confirmado desde el menú de pausa. `Level1_Cave` una vez (se jugó entera). **No son las tres mediciones por escena de PF-RNF04-01**; con un peor valor 20 veces por debajo del límite no cambia el veredicto, pero la tabla no sustituye a la de rc3 como evidencia de ese caso.
- Recorrido: `OE4_A` (perfil sin progreso) jugó el Nivel 1 de verdad —siete piezas arrastradas, fuerza 7 y cercanía 5, tres golpes, «Soplar», la narrativa de cierre y el resumen—, y pasó por créditos y «Progreso del equipo». Después, «Nivel 2» hasta el bosque. Con `OE4_B2`, `OE4_B3` y `OE4_D` (semillas de `herramientas/perfiles/`, tres filas en la misma página) se llegó al taller, al laberinto y al río.
- DEF-RC3-09: el recorrido envió 177 clics (108 llamadas del arnés) y 7 arrastres, con 1,3 a 1,5 s entre los de la narrativa y esperas de 1 a 1,5 s tras cada carga; no se vio ninguno perdido (cada paso se comprobó por la línea de `RNF-04` o por una foto), pero el arnés no cuenta los clics que el juego descarta.
- `Revisar-Log` sobre `a` y `b`: sin `Exception`, `Error` ni `Assert`.

## Memoria (RNF-05): CUMPLE

El muestreador del arnés guarda cada 2 s `WorkingSet64` y `PrivateMemorySize64`. 373 muestras en los cuatro lanzamientos:

| Medida | Máximo | Dónde |
|---|---|---|
| Memoria de trabajo (`WorkingSet64`) | **434,6 MiB** | `Level3_River` (lanzamiento `b`) |
| **Memoria privada (`PrivateMemorySize64`), el pico** | **1 083,3 MiB = 1 136,0 MB = 1,06 GiB** | `Level1_Cave` (lanzamiento `a`) |
| Picos del sistema | `PeakWorkingSet64` 434,6 MiB · `PeakPagedMemorySize64` 1 083,3 MiB | |

El límite es 2 048 MiB: el pico queda en el 52,9 %. rc3 dio 1 094 MiB y 438 MiB. **RNF-10:** las 373 filas de `*_red.csv` sin ninguna conexión TCP ni UDP a una dirección que no sea de loopback.

**Límites de la medida:** un solo equipo; Rider y el cliente de escritorio de Claude seguían abiertos (los Editores, no); el recorrido es de unos 20 minutos, no el *Golden Path* completo; no se midió el «primer cuadro no negro».

## Cambios respecto a rc3 (compilado desde `f49ccc9`)

| Id | Qué cambia en el ejecutable |
|---|---|
| INC-152 | Aperturas del Nivel 1 más claras: `N1_Apertura`, `N1_AparicionGuia` y `N1_Hallazgo` suben 0,12 la luz de inicio y de cada parada (mínimo 0,15; se conserva el negro intencional de la línea 6 de la apertura). La cueva jugable pasa de 0,10 a 0,22 en su punto más oscuro y de 0,30 a 0,35 en el más claro |
| INC-153 | Caminata real en la escena 1.1: en la línea 7 de `N1_AparicionGuia` la familia se desplaza hacia la izquierda, de perfil, y una prueba de contenido exige un desplazamiento mínimo a todo paso de locomoción de las 18 narrativas |
| INC-154 | Abrazo y sueño de perfil: `ActionView` pasa `Hug` y `Sleep` a `Profile`, se rehacen los ocho clips `abrazar` y `dormir` de la familia y `N1_NacimientoDelFuego` fija quién mira a quién y dónde duerme cada uno. **Retoques de esta recompilación:** Papá se inclina hacia el abrazo en la 1.4.4 (clip `char_papa_anim_abrazar`, destinos de Mamá, la Niña y el Niño en las líneas 4 de `N1_NacimientoDelFuego`) y nadie queda escondido tras Algoritm en la 1.1 (`N1_AparicionGuia`) |
| INC-155 | Troncos del bosque: empuje 0,75 → 0,35, rozamiento 1,4 → 3,0, el tronco no se empuja con el cursor sobre su área de clic y esa área cubre el cuerpo que dibuja `RollingLog` |
| INC-156 | Flechas curvas de «Girar»: Phosphor `arrow-arc-left` y `arrow-arc-right` (MIT) a 128 px, con su aviso en los créditos |
| INC-157 | Vocabulario más llano para niños de quinto en las narrativas, los textos del Nivel 3 y el resumen del Nivel 2 |
| INC-158 | Aviso del botón de ayuda en la primera mecánica (`HelpButtonIntro`, `FireHelpIntro`): fondo oscuro, «Pista» iluminada encima, tiempo detenido y un solo botón «Cerrar»; sale mientras el perfil no haya confirmado la fase 1 del Nivel 1. `HelpButtonIntro.OnDestroy` ya no destruye el `GraphicRaycaster` con `DestroyImmediate` |
| INC-159 | Menú de pausa de cinco botones: Reanudar, Reiniciar, Volver a ver la historia (oculto en la fase sin escena propia), Volver al menú de niveles y Guardar y salir (con confirmación) |
| INC-160 | Botón «Atrás» del diálogo (`DialogueRunner.Back`, `NarrativeSceneController.Back`): relee la línea anterior sin repetir sonidos ni movimientos. «Omitir» (260 × 76) encima de «Continuar», alineado a la derecha, y «Atrás» a la izquierda de «Continuar» en su misma fila |
| INC-151 | Solo texto: «grado quinto» en documentos; no cambia el ejecutable |
| DEF-RC3-05 | Informe docente: las cabeceras de las columnas ya no se montan con sus iconos (`TeacherReport.unity`) |
| DEF-RC3-09 | `InputGuard` (nuevo, `Game.UI`, en `Boot`): el juego ignora los clics durante 0,3 s tras cada carga de escena, así que un doble clic en «Omitir» ya no se cuela en la escena siguiente |
| DEF-RC3-10 | Bosque del N2: la tablilla de instrucciones es más ancha y las tres líneas caben (`Level2_Forest.unity`) |
| DEF-RC3-11 | Textos de botones partidos en dos líneas en el informe docente y en los diálogos de borrar y salir (`TeacherReport.unity`, `MainMenu.unity`) |

## Pruebas

- **Suite completa** con `suite2.ps1` sobre `0405eba` (24,4 min de pared): **EditMode 793 = 792 pasan + 1 omitida** (la de siempre, `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`, por entorno), **PlayMode 435/435**, 0 fallos. Cobertura contra `list_tests`: 1 228 listadas, 1 228 ejecutadas, faltan 0, repetidas 0. Carril A: EditMode 20,4 s y `Game.UI.PlayMode.Tests` 190/190 en 1 384,7 s; carril B: Audio 4, Core 12, Fire 68, River 58, Wheel 103, todas en verde. Evidencia: `suites/rc4-0405eba/`.
- **Corridas filtradas antes de la suite**, todas en verde: `InputGuard` 1/1, `DEFRC3` 7/7, `TeacherReport` 10/10, `EraseDialog` 4/4, `MainMenu` 20/20, `ForestScene` 40/40; EditMode `NarrativeSequence_RNF03` 1/1. El filtro `NarrativeScene` no se corrió aparte: la suite ejecuta `NarrativeSceneTests` entera.
- **Captura de `N1_NacimientoDelFuego`** (`NarrativeScene_RF05_CapturaCadaLineaConLosPersonajes`): sin cambios en el asset. La línea 16 se ve completa con el Niño y la Niña, y se comprobó también en el ejecutable (`evidencias/rc4/S-RNF-rc4_L16_NacimientoDelFuego_exe.png`).

## Tras el build

- `ProjectSettings/ProjectSettings.asset` salió con `preloadedAssets` apuntando a `ControlesJugables.inputactions` (único cambio) y se restauró con `git checkout` una vez cerrados los Editores, para que no lo reescribieran.
- `Restaurar-Registro S-RNF-rc4-a` falló con un error del arnés al escribir un valor (`SetValue … RegistryValueKind`); las claves del reproductor en `HKCU\Software\Universidad Catolica de Colombia\Algoritmia` quedan como las dejó el juego (el residuo del motor ya registrado como DEF-RC2-01).
- Los dos Editores (el del proyecto y el de `C:\Dev\Algoritmia-B`) se cerraron para medir RNF-04 y RNF-05.

## rc4 previo (apartado en `Build/Algoritmia_rc4a`)

Compilado el 13/10/2026 a las 18:31 desde `0839352567deff1f96cad18a649365c3a379dbc9` con cuatro archivos de `Assets/` sin commit (`HelpButtonIntro.cs`, su prueba, `Narrative.unity` y la prueba de `NarrativeSceneTests`, hoy en `HEAD`). Pesó **426 930 226 bytes = 426,9 MB**; huella del contenido `a7569cdb6028f70aba3178b2c3db487ee90769673bebfec51b1a518d4dcfbbfa` (223 archivos), de la carpeta `746a6c27dc4c74e2d0b3ef32767fd937a2af1bb1b41c5f7f4c580ba12a59bf40` (252), `git diff` `547c20e164a76859f5dce696915e28daa044f9f82b2a0c2ce456c097c2e5a738`. Suite de entonces: EditMode 785 de 786 y PlayMode 427/427 (`suites/rc4-previa-al-boton-atras/`). No llevaba mediciones de carga ni de memoria. Su `BuildReport` es `build-rc4a.buildreport`. No se etiquetó; lo sustituye este.

## Artefactos

- `BuildReport`: `claudeDocs/tasks/OE4/evidencias/build-rc4.buildreport` (copia de `Library/LastBuild.buildreport`); el del rc4 previo, `build-rc4a.buildreport`.
- Mediciones del recorrido: `evidencias/rc4/S-RNF-rc4-{a,b,c,d}_mem.csv` y `_red.csv`, y los `Player.log` de `a` y `b` (`S-RNF-rc4-{a,b}_Player.txt`). Los de `c` y `d` solo traen una carga de `MainMenu`.
- Suite: `evidencias/suites/rc4-0405eba/`.
- Capturas del cuadro de diálogo con los tres botones y de las líneas del abrazo, el sueño y la caminata: `evidencias/rc4/Inc160_*.png` y `claudeDocs/tasks/Personajes/capturas/2026-10-13-inc154/` (con `_v3` de la línea 16 de `N1_NacimientoDelFuego`).
