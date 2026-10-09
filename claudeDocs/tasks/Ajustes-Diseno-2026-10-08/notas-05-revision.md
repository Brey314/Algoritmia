# Notas 05 — Revisión (etapa 5): la ronda del 08/10 integrada con el carril de perfil

Parte crudo del revisor, 09/10/2026. `HEAD` = `eb9be13`: el merge de la ronda (`a938f9a` Papá, `108f95e` laberinto, `08b6c63` narrativa,
`bd8b802` Algoritm por partes con `Wave` y créditos, `c96846f` menús) con el carril de perfil (`910c0f0`, INC-134 e INC-135). **Sin commit,
merge, pull ni push.** No toqué documentación, `ProjectSettings` ni `Packages`. Mientras yo trabajaba, otro agente (documentación) editó
`CLAUDE.md`, `claudeDocs/**`, `docs/**`, `.gitignore` y `Assets/Game/Art/Inventario.md` en el mismo árbol: nada de eso es mío.

## 0. Resumen

- **Compila sin errores** (18 s). No hubo choque de compilación entre los dos carriles. Unity generó los 5 `.meta` que faltaban.
- **Un solo choque semántico**, de prueba y no de código: `ActorAction.Wave = 22` (D12, etapa 4a) es la acción número 23, y
  `ActionView_INC134_LaTablaCubreTodasLasAcciones` (carril de perfil) fijaba el enum en 22 para obligar a decidir la vista de cada acción nueva.
  `Wave` es un saludo a quien juega: **de frente**, que es lo que `ActionView.For` ya devolvía por omisión. Corregí la prueba (2 líneas); el código de producción no cambia.
- **Suite completa (`suite2.ps1`, dos Editores, 24,4 min), ya con la corrección**: EditMode 706/711 (1 omitida, 4 rojos esperados) y PlayMode 416/417
  (1 inconclusiva, del carril de perfil); 1128 pruebas listadas, 1128 ejecutadas, 0 faltan, 0 repetidas. **Ningún otro fallo.**
- **D19 verificado** en Play a 1920×1080: la balsa no se sale del cuadro ni queda bajo el cuadro de diálogo, ni sin leer ni leyendo (§5).
- **El carril de perfil integrado no voltea a nadie ni dibuja dos cuerpos** (§6). Límite: ningún prefab tiene todavía `Lienzo/Perfil`, así que
  el perfil en sí no se pudo ver (es el rojo esperado).

| | Total | Pasan | Fallan | Omitidas | Inconclusivas |
|---|---|---|---|---|---|
| EditMode (suite2, Editor A recién abierto) | 711 | 706 | 4 (rojo esperado) | 1 (entorno, de antes) | 0 |
| PlayMode (suite2, Editores A y B) | 417 | 416 | 0 | 0 | 1 (perfil, `Assume`) |

## 1. Compilación

`editor.ps1 recompile` → «Compilación OK (status=completed, errores=0)», 18 s. Consola de errores vacía durante todo el trabajo y al final.
Avisos de siempre, ninguno nuevo: `Game.Audio.Tests.asmdef` sin scripts, `PropShadow.cs(453) CS0108`, y los de `LimbFollower`/`ArmLayering` que
las pruebas provocan a propósito. Los `.cs` del carril de perfil llegaron sin `.meta`; ver §8.

## 2. EditMode

| Corrida | Editor | Total | Pasan | Fallan | Omitidas | s |
|---|---|---|---|---|---|---|
| `tests-edit -` (entera), **antes** de corregir | proyecto, abierto desde el 08/10 | 711 | 705 | **5** | 1 | 20,4 |
| `tests-edit CharacterViewTests`, tras corregir | proyecto | 12 | 12 | 0 | 0 | 8,3 |
| Dentro de `suite2.ps1` (carril A), con la corrección | proyecto, recién abierto | 711 | 706 | **4** | 1 | 16,4 |

La omitida es `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`: un `Assume` de entorno («Este equipo no hace cumplir el
atributo de solo lectura sobre carpetas»). Viene de antes y no es regresión. Comprobé por nombre que corrieron y pasaron las pruebas EditMode
de la ronda que cita el brief: `NarrativeSequence_RF05_NingunEncuadreDelNivel3SeRecorta`, `…RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`,
`…RF44_LaCamaraDelCruceAcompanaALaBalsa` (y las otras 6 `RF44`), `…CadaLlamaEmiteSuHalo`, las 12 de `FireGlow`, `CaveSceneData`, las 2 de
`MazeSceneData`, las 7 de `FramedIllustration` y las 25 de `CharacterRigIdlePhase`.

## 3. PlayMode — `suite2.ps1`

Comando: `pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/suite2.ps1 -Out %TEMP%\Algoritmia-suite\revision-ronda-2026-10-09`, en segundo plano.
Antes cerré el Editor del proyecto (escenas limpias, `SaveAssets`), y la suite lo abrió de nuevo junto al de la copia `C:\Dev\Algoritmia-B`
(sincronizada por robocopy; la `Library` ya existía). `HEAD eb9be13` + árbol `f2afbb9ed338`, Unity 6000.5.10f1.

| Carril | Editor | Corrida | Total | Pasan | Fallan | Omitidas | Inconcl. | s |
|---|---|---|---|---|---|---|---|---|
| A | proyecto | EditMode (entera) | 711 | 706 | 4 | 1 | 0 | 16,4 |
| A | proyecto | PlayMode `Game.UI.PlayMode.Tests` | 177 | 177 | 0 | 0 | 0 | 1359,6 |
| B | copia | PlayMode `Game.Audio.PlayMode.Tests` | 4 | 4 | 0 | 0 | 0 | 7,2 |
| B | copia | PlayMode `Game.Core.PlayMode.Tests` | 12 | 12 | 0 | 0 | 0 | 22,2 |
| B | copia | PlayMode `Game.Levels.Fire.PlayMode.Tests` | 64 | 64 | 0 | 0 | 0 | 117,4 |
| B | copia | PlayMode `Game.Levels.River.PlayMode.Tests` | 58 | 57 | 0 | 0 | **1** | 147,4 |
| B | copia | PlayMode `Game.Levels.Wheel.PlayMode.Tests` | 102 | 102 | 0 | 0 | 0 | 317,7 |

- **Pared: 24,4 min** (00:27:47 → 00:52:12). Preparación 1 min 27 s (Editor A listo a los 24 s, el B a los 69 s, revisión de scripts sin clase: ninguno);
  carril B terminó a las 00:39:30 (≈ 10,2 min de pruebas); carril A, a las 00:52:11 (23,0 min desde el arranque de los carriles; 22,7 de ellos, `Game.UI`).
- `list_tests`: **1128** (711 EditMode + 417 PlayMode). Cobertura: 1128 ejecutadas, **faltan 0, repetidas 0**. Salida de la suite: 1 (por los 4 rojos esperados).
- `RiverLevel_RNF05` (memoria) pasó: B estaba recién abierto. Sin infraestructura: no hubo que reintentar nada.
- Comprobé por nombre, en los JSON, que las pruebas PlayMode nuevas o ajustadas de las cuatro etapas corrieron y pasaron: las 4 `MainMenu_RF01_*`,
  `LevelSelect_RNF19_ElIconoDeEstado…` y `…RF03_CadaTarjeta…`, `Credits_RF08_AlgoritmSaluda…`, `NarrativeScene_RNF01_TodasLasLineas…` (las 18 narrativas),
  `…RF05_ElNombreDelHablante…`, `…RNF03_ElNombreElTextoYLosBotones…`, las 2 `RF44` de la cámara, `…RF05_CadaFogataTieneSuHalo…` (2 casos), las 2 `RNF21_ElHalo…`,
  `…RNF21_LaBalsaCruzaSinSaltosNiParpadeos`, `…RNF03_ElCuadroDeDialogoNoSuperaElCuartoDeLaPantalla`, `FireLevel_RF20_LaLlamaCenital…` (las 2), las 5 de
  `MazeScene_RF30/RF31/RNF23` ajustadas o nuevas, `FireLevel_DA133_*` (5), `WorkshopScene_DA133_*` (3) y `Personajes_DA133_*` (5).
- Las del carril de perfil: `NarrativeScene_INC134_QuienSeDesplazaVaDePerfilHaciaDondeCamina` **18/18** (con los prefabs actuales solo comprueba la mitad «de frente y sin
  voltear»), `FireLevel_INC134_*` 3/3, `ForestScene_INC134_*` 1/1, `RiverScene_INC134_*` 1 inconclusiva (§4).
- Copia del resumen y del registro en `suite-05/` (junto a este parte): `resumen.md`, `resumen.json`, `registro.txt`. Los JSON/XML por corrida quedan en
  `%TEMP%\Algoritmia-suite\revision-ronda-2026-10-09\{A,B}\` (se borran con la carpeta temporal).

## 4. Fallos y clasificación

| Prueba | Resultado | Clasificación | Qué hice |
|---|---|---|---|
| `CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte("Papa")`, `("Mama")`, `("Nina")`, `("Nino")` | Fallan los 4, en las dos corridas EditMode | **Rojo esperado** (carril de perfil, test-first; espera la ronda del Editor de ese carril) | Nada: ni tocada ni saltada |
| `ActionView_INC134_LaTablaCubreTodasLasAcciones` | Falló en la primera corrida; pasa tras la corrección | **Integración**: `ActorAction.Wave = 22` (esta ronda) contra una tabla exhaustiva del carril de perfil | Corregida en la prueba (abajo) |
| `RiverScene_INC134_MamaVaDePerfilAlCaminarYDeFrenteAlSoltarLaFlecha` | Inconclusiva (`Assume`), no es fallo | **Carril de perfil**, esperada: mismo origen que el rojo, sin `Lienzo/Perfil` en el prefab de Mamá | Nada |

Texto exacto, por si hay que mandarlo al carril de perfil:

- Los 4 rojos esperados (cada uno con su personaje): `  Papa: existe Lienzo/Perfil` · `  Expected: not null` · `  But was:  null`.
- El choque, antes de corregir (`CharacterViewTests.cs:57`): `  si el enum crece, esta prueba pide decidir la vista de la acción nueva` · `  Expected: property Length equal to 22` · `  But was:  23`.
- La inconclusiva: `  Mamá tiene cuerpo de perfil (el arte de perfil todavía no llegó a su prefab)` · `  Expected: True` · `  But was:  False`.

**Corrección del choque** (`Assets/Tests/EditMode/Scaffolding/CharacterViewTests.cs`, 2 líneas, una sola vez):

1. `Has.Length.EqualTo(22)` → `EqualTo(23)` en `ActionView_INC134_LaTablaCubreTodasLasAcciones`: la prueba hace justo lo que dice, pedir una decisión por cada acción nueva.
2. `ActorAction.Wave` entra en la lista de gestos hacia el estudiante de `ActionView_INC134_LosGestosHaciaElEstudianteVanDeFrente`. La decisión es **de frente**: es un saludo
   a quien juega, como `Talk`, `Point` o `Celebrate`, y solo lo hace Algoritm, que no tiene perfil. `ActionView.For` ya devolvía `Front` por omisión, así que **no toqué producción**.

Quién tenía razón: la prueba del carril de perfil estaba bien para su rama (no podía conocer `Wave`); el código también. Lo que cambia es la cifra que la prueba usa de testigo.
Hay que avisar a esa sesión: al traer este merge verá el conteo en 23 y `Wave` en la lista de frente (solo habría conflicto de texto si ellos editan esas mismas líneas).

## 5. D19 — apertura centrada del cruce 3.3 (`N3_Escena33_Cruce`, foco x 0,38 → 0,50)

El asset de `HEAD` trae `CameraStart` y `CameraEnd` en `(0,50; 0,37)`, zoom 1,6; la parada de la línea 1 en 0,52 y la balsa con `CameraFollows`. Capturas a 1920×1080 en Play manual,
por el flujo real (`GameFlowRunner`: perfil, `StartNarrative`), con `ScreenCapture`, y la posición de la balsa y de los personajes registrada cada 0,25 s.
`t` = segundos desde que la escena narrativa está activa; los nombres de archivo redondean a una cifra decimal. Pruebas EditMode y PlayMode de la cámara: todas verdes (§2 y §3).

| Recorrido | Balsa (caja dibujada, 484 px) | Margen izq. mín. | Margen der. mín. | Parte más baja | Deslizamiento a la izquierda |
|---|---|---|---|---|---|
| **Sin leer** (sin avanzar el texto, 0,9 → 15,9 s cada 1,5 s) | arranca en x 902–1386 (centro 1144); el centro llega a 1232 a los 5,9 s y vuelve a 1145 | 902 px | **446 px** (a los 5,65 s) | y = 341 (la cima del cuadro está en 232) | 88 px |
| **Leyendo** (clics a 3; 5,5; 8; 11; 13,5 y 16 s; cada 0,75 s) | el centro va de 1144 a 970 (10,9 s) y acaba en 1304 con el plano final | 737 px (10,9 s) | **464 px** (19,65 s) | y = 267 (35 px por encima del cuadro) | **228 px** |

- 228 px es lo que el modelo de la etapa 3 predecía con el arranque en 0,50 (~230 px; con 0,38 eran 581 px). El margen derecho mínimo es de ~450 px; la etapa 3 había medido 77 px con el arranque en 0,38
  (no es exactamente la misma medida: la mía es la caja del sprite de la balsa, la suya el borde visible en las capturas; la diferencia no cambia la conclusión).
- **La balsa nunca se sale del cuadro** (margen mínimo por cualquier lado: 446 px) **y nada queda bajo el cuadro de diálogo**: lo más bajo de la escena es la balsa en el plano final, a 35 px por encima de la cima del cuadro;
  los personajes, a más de 350 px (el más bajo, 359; caja del personaje, que incluye su margen transparente).
- A simple vista: en la apertura (t = 0,9) la balsa con la familia está en la poza bajo la cascada, entera, centrada a la derecha; la cámara la acompaña; al desembarcar la familia camina a la orilla y el
  último plano (zoom 1) enseña la balsa, la familia y a Algoritm (gota) sin nada bajo el cuadro. El cuadro nuevo (216 px, Nunito 30) se lee bien en todos los cuadros.

Archivos en `capturas/05-revision/`: `cruce-sinleer_t*.png` (11) + `hoja-cruce-sin-leer.png` + `cruce-sinleer_log.txt`; `cruce-leyendo_t*.png` (16: cada 1,5 s más los de 9,2 · 10,7 · 12,2 s del desembarco) +
`hoja-cruce-leyendo.png` (cada 1,5 s) + `hoja-cruce-desembarco.png` (8 → 13 s cada 0,75 s) + `cruce-leyendo_log.txt`. Los registros traen, por instante, la caja de cada objeto en píxeles de pantalla (y desde abajo),
la acción, la vista y el volteo de cada personaje (la coma decimal es la de la cultura del Editor).

## 6. El carril de perfil integrado con lo de la ronda

Ningún prefab de personaje tiene un nodo `Perfil` (`grep m_Name: Perfil` en `Assets/Game/Prefabs/Characters/`: nada), que es lo que dice el rojo esperado. Con eso `HasProfile` es falso para todos y la vista es siempre de frente.

- **Menú principal** (`menu-principal-a.png`, `-b.png`, 3 s de diferencia): Papá, Mamá, Niña, Niño y Algoritm (fuego) en reposo, en poses distintas (Papá con las manos en la cintura en una, los brazos caídos en otra; Mamá
  levantando un brazo en una), cada uno con un solo cuerpo y ninguno volteado. Título, tarjeta y botones como en la etapa 4b.
- **Narrativas con personajes que andan** (Play manual, con el estado de cada rig registrado): el cruce 3.3, donde la familia baja caminando de la balsa (`hoja-cruce-desembarco.png`), y `N1_Apertura`, donde anda y corre
  (`hoja-apertura-n1-caminando.png`, escena oscura a propósito). En las 1174 muestras de rig de las tres series (cruce sin leer 330, cruce leyendo 400, apertura del N1 444) `Walk`, `Run`, `Idle`, `Talk`, `Celebrate`,
  `Encourage`, `Observe`, `Point` y `Surprise` salen con `vista = Front` y el lienzo del rig sin voltear (`localScale.x > 0`); ninguna con dos cuerpos.
- Dos datos del registro de `N1_Apertura` que no son defectos: (a) el rig recuerda `Mirrored = true` cuando camina a la izquierda, pero de frente no lo aplica (la regla de INC-134); (b) un personaje sale volteado porque
  **el asset lo declara** `Mirrored: 1` (la casilla se voltea), igual que en `3efa765`, antes del carril de perfil.
- **Algoritm por partes en el motor** (lo que el implementador de la etapa 4a no pudo ver para la rueda y la gota): `algoritm-rueda-y-gota-en-el-motor.png`, recortes ×4 de las capturas que sacó la suite. Las nueve piezas encajan
  sin huecos en reposo en las dos formas. No vi sus gestos en movimiento.
- Capturas de las pruebas `VisualVerification` de esta corrida (carpeta `TestScreenshots` de `LocalLow`), una muestra en `hoja-capturas-de-la-suite.png`: laberinto ámbar, bosque, río, taller, menú, menú de niveles
  (icono abajo a la izquierda de la imagen, el N2 con el bosque), la llama cenital con su halo, dos narrativas con el cuadro nuevo. Nada raro.

## 7. Qué no pude verificar

- **El perfil en sí**: sin `Lienzo/Perfil` en los prefabs no hay vista de perfil, ni volteo, ni corte seco que mirar. Lo cubren las pruebas del carril de perfil, que con arte pasarán de rojo/inconclusivas a verdes.
- **`Strike` de Papá en el motor (D16)**: solo vi a Papá en reposo (manos en la cintura, sin la punta del húmero en el codo) en las capturas de la suite; el choque lo vio el implementador en renders del Editor.
- **Gestos de Algoritm en movimiento** (`Wave` en los créditos, `Point`, `Celebrate`) más allá de lo que ya enseñaron las capturas de la etapa 4a: lo cubren las pruebas verdes, no lo volví a mirar.
- **El ejecutable**: no compilé ningún candidato; el peso de las 27 piezas de Algoritm (+3,63 MiB sin comprimir, RNF-06) sigue sin medirse en un build.
- **Estabilidad**: una sola corrida de la suite; las pruebas nuevas de `MainMenu_RF01_*` esperan solo dos fotogramas y pasaron con dos Editores a la vez, pero no las repetí.
- Las pruebas `VisualVerification` pasan por no tener `Assert`; de sus capturas miré solo la muestra de §6.

## 8. Archivos tocados, `.meta` generados y capturas

```
 M Assets/Tests/EditMode/Scaffolding/CharacterViewTests.cs        (2 líneas: 22 → 23 y Wave en la lista de frente)
?? Assets/Game/Scripts/Runtime/Scaffolding/ActionView.cs.meta     (generado por Unity)
?? Assets/Game/Scripts/Runtime/Scaffolding/ActorFacing.cs.meta    (generado por Unity)
?? Assets/Game/Scripts/Runtime/Scaffolding/CharacterView.cs.meta  (generado por Unity)
?? Assets/Game/Scripts/Runtime/Scaffolding/Heading.cs.meta        (generado por Unity)
?? Assets/Tests/EditMode/Scaffolding/CharacterViewTests.cs.meta   (generado por Unity)
?? claudeDocs/tasks/Ajustes-Diseno-2026-10-08/notas-05-revision.md
?? claudeDocs/tasks/Ajustes-Diseno-2026-10-08/suite-05/            (resumen.md, resumen.json, registro.txt)
?? claudeDocs/tasks/Ajustes-Diseno-2026-10-08/capturas/05-revision/ (≈ 50 MB: las capturas de §5 y §6)
```

Los `.meta` son los cinco de los `.cs` que el carril de perfil dejó sin ellos (formato compacto de dos líneas, como el de `ActorBeat.cs.meta`); ningún GUID repetido en `Assets/`.
Nada escrito dentro de `Assets/` por las capturas (usé `ScreenCapture` con rutas absolutas), ningún andamiaje de Editor, `ProjectSettings` y `Packages` sin cambios (no corrí batchmode),
ningún `InitTestScene*`. Los scripts de captura viven en el scratchpad de la sesión y no se versionan. Dejé abierto el Editor del proyecto, sin escenas sucias; cerré el de la copia.

## 9. Para la documentación (no la toqué)

- Quien documente INC-134 (`ActionView`) debe listar `Wave` entre las acciones de frente; el enum es de 23 acciones (0–22). Hoy ningún documento nombra `ActionView`.
- El parte de la etapa 4a ya recoge «22 acciones (0–21) → 23» para `Personajes-Resultados.md`.

## 10. Mensaje de commit propuesto

```
Review of the 08/10 round on top of the profile lane: Wave joins the INC-134 view table

The round (Papá's arms, maze, narrative box, Algoritm in nine parts with Wave,
menus) and the profile lane (CharacterView, ActionView, Heading, ActorFacing;
INC-134, INC-135) merge and compile cleanly. The one clash was semantic:
ActorAction.Wave = 22 is a 23rd action, and
ActionView_INC134_LaTablaCubreTodasLasAcciones pinned the count at 22 so that
every new action forces a decision about its view. Wave is a greeting to the
player, so it is seen from the front (ActionView.For already returned Front by
default); the test now expects 23 actions and lists Wave among the front
gestures. No production code changes.

Adds the .meta files Unity generated for the profile lane's new scripts and
test (ActionView, ActorFacing, CharacterView, Heading, CharacterViewTests).

Full suite on the merged tree (two Editors, 24.4 min): EditMode 706/711 (1
environment skip, 4 expected red: CharacterRig_INC134_LaFamiliaTieneCuerpoDe
PerfilConArte until the profile art lands), PlayMode 416/417 (1 inconclusive:
RiverScene_INC134_MamaVaDePerfil..., an Assume until the profile body exists),
1128 of 1128 listed tests run once, no other failure. D19 checked in Play at
1920x1080: with the opening focus at 0.50 the raft stays whole on screen (least
margin 446 px) and never under the dialogue box; the largest leftward slide is
228 px. Review notes, suite summary and captures under
claudeDocs/tasks/Ajustes-Diseno-2026-10-08/.

Kanban: Personajes finales (INC-134, INC-135); the design round has no card.
```
