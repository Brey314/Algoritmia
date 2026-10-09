# Etapa 2 — Laberinto (`Level2_Maze`): parte crudo (08/10/2026)

Implementador: etapa 2 de la ronda de ajustes. Decisiones D1–D4 del `brief.md`. **Sin commit.**
Editor abierto durante todo el trabajo; lo dejo con `Narrative.unity` activa y limpia (como estaba al empezar).

## 1. Qué cambié (antes → después)

### Escena `Assets/Game/Scenes/Level2_Maze.unity` (editada por `eval` del Editor, guardada con `EditorSceneManager.SaveScene`)

| Objeto / campo | Antes (HEAD) | Después |
|---|---|---|
| `Canvas/Fondo_Escena` · Image `m_Color` | #F7EFE2 `(0.969, 0.937, 0.886)` (en el árbol había #E8C27D sin commitear: descartado) | **#E8A33D** `(0.9098039, 0.6392157, 0.23921569)` |
| `Canvas/Panel_Secuencia/Fondo/Ventana_Secuencia` · Image | #F7EFE2, sin sprite, `Simple` | **#E0D4C0** `(0.878, 0.831, 0.753)`, `ui_boton` (guid `5ec80065…`), `Sliced` (`m_Type: 1`) |
| `Canvas/Panel_Secuencia/Fondo` · Image | sin sprite, `Simple`, inset 5 (`sizeDelta -10,-10`), ppum 1 | `ui_boton` `Sliced`, **inset 8** (`sizeDelta -16,-16`), **`pixelsPerUnitMultiplier` 1,5** |
| `Canvas/Boton_Pista` · Image (el aro) | #E2571F `(0.886, 0.341, 0.122)` | **#A0330D** `(0.627451, 0.2, 0.050980393)` |
| `Canvas/Panel_World` · RectTransform | offsets 0 / 0 (`sizeDelta 0,0`) | offsets **izq +40 / der −40** (`sizeDelta -80,0`) |
| `Canvas/Panel_World/Marco_Entorno` (nuevo, primer hijo: detrás de `Image_Environment`) | — | Image `ui_boton` `Sliced`, `fillCenter` **apagado**, #C4A882, ppum 3,25 (=26/8), `raycastTarget` apagado, capa UI; tamaño guardado 1615×915 (el real lo pone `FitEnvironment`: visible + 16) |
| `MazeScene` · `MazeSceneController` | (sin los dos campos) | `environmentFrame` → `Marco_Entorno`; `environmentBorderWidth` = 8 |
| `MazeScene` · `ivoryShadeColor` | #E0D4C0 en HEAD (#E8C27D en el árbol) | **#E0D4C0** (descartado el cambio sin commitear: D3) |
| `Cajon_Bloques` · `anchoredPosition`/`sizeDelta` | (287,−797) / (526,0) | (284,−791) / (520,0): **efecto del relayout** por el inset 8, valor que Unity recalcula siempre |

D3 (descartar los tres cambios sin commitear): hecho con `git checkout -- Assets/Game/Scenes/Level2_Maze.unity` **antes** de abrir la escena (no estaba abierta en el Editor). Los tres eran `ivoryShadeColor`, `Fondo_Escena` y el `UniversalAdditionalCameraData` de `Main Camera`.
**Unity NO regeneró el componente de cámara**: tras abrir la escena, guardarla por script (dos veces) y entrar y salir de Play tres veces después de guardar, `git diff HEAD` no lo contiene (0 coincidencias de `UniversalAdditionalCameraData`).

### Asset `Assets/Game/Data/Wheel/N2_MazeLayout.asset` (edición de texto, formato Unity)
`<BackdropColor>`: `(0.9686275, 0.9372549, 0.8862745)` #F7EFE2 → **`(0.9098039, 0.6392157, 0.23921569)` #E8A33D**.

### Código
- `MazeLayout.cs`: tooltip de `BackdropColor` corregido (ya no dice «marfil de interfaz (#F7EFE2)»; dice ámbar #E8A33D, igual que `Fondo_Escena`, y nombra la prueba que lo vigila).
- `MazeSceneController.cs`:
  - quita el bloque del `Outline` del entorno (era `effectDistance (4,−4)`, que se escala con `localScale`);
  - `FitEnvironment` descuenta `2 × environmentBorderWidth` al panel antes de calcular la escala y llama a `FitFrame`;
  - `FitFrame` (nuevo): color `environmentBorderColor`, `pixelsPerUnitMultiplier = border.x / (ancho × pixelsPerUnit)` (da 8 px del lienzo con cualquier sprite), tamaño = ilustración visible + 2×8, centrado;
  - campos nuevos `environmentFrame` (Image) y `environmentBorderWidth` (8, `[Min(0)]`, con tooltip), tooltip de `environmentBorderColor` actualizado; accesor `internal EnvironmentFrame` dentro del `#if UNITY_INCLUDE_TESTS`.

### Pruebas
- **PlayMode** `Assets/Tests/PlayMode/Levels/Wheel/MazeSceneTests.cs`:
  - `MazeScene_RNF23_ElEntornoCabeEnteroEnSuPanelSinDeformarseYLaMatrizCubreElSeto` — ajustada: lo que llena el panel por un eje es ahora el **marco** (la ilustración se ajusta a lo que queda tras descontar 8 px por lado).
  - `MazeScene_RF30_ElEntornoQuedaSinTinteYConSuContorno` → renombrada **`MazeScene_RF30_ElEntornoQuedaSinTinte`** (solo el tinte; fuera lo del `Outline`).
  - **nueva** `MazeScene_RF30_ElEntornoYLaTarjetaLlevanElMismoMarcoRedondeado`: color #C4A882, `ui_boton`, `Sliced` en los dos; `Fondo` de la tarjeta redondeado, lleno, ppum 1,5; 8 px por los cuatro lados de la tarjeta y del entorno (medidos en pantalla, ×`scaleFactor`); el sprite pinta 8 px del lienzo; marco hermano, detrás, sin centro y sin raycast; marco y tarjeta enteros en pantalla y sin tocarse.
  - `MazeScene_RF30_LaSalidaSeLeeEnElEntornoYElPanelEsMarfil` → renombrada **`MazeScene_RF30_LaSalidaSeLeeEnElEntornoYElFondoEsUnoSolo`**: el panel se pinta con `BackdropColor` y es ámbar #E8A33D (antes comprobaba marfil).
  - **nueva** `MazeScene_RF31_LaZonaDeSoltarEsMarfilSombraSobreLaTarjeta`: la ventana es #E0D4C0, `ui_boton` `Sliced`, distinta del `Fondo` de la tarjeta; y la muesca vacía del primer bloque (`ivoryShadeColor`) es #E0D4C0 = la zona.
  - helper nuevo `AssertGrosor(Rect, Rect, float, string)`.
- **EditMode** (nuevo) `Assets/Tests/EditMode/Levels/Wheel/MazeSceneDataTests.cs` (+ su `.meta`, que generó Unity): abre `Level2_Maze.unity` como escena de vista previa (patrón de `RiverLevelConfigTests.ConLaOrilla`) y comprueba lo guardado en disco:
  - `MazeScene_RF30_ElFondoDeLaEscenaYElDelAssetSonElMismoAmbar` — **el guardián pedido**: `Fondo_Escena` == `MazeLayout.BackdropColor` (en hex RGBA, exacto) == `E8A33DFF`.
  - `MazeScene_RNF20_ElAroDelBotonDePistaSeDistingueDelFondoAmbar` — contraste aro/fondo ≥ 3:1 (WCAG 1.4.11 para no texto; el 4,5:1 de RNF-20 es para texto).
- `Assets/Tests/EditMode/Levels/Wheel/Game.Levels.Wheel.Tests.asmdef`: añadida la referencia `UnityEngine.UI` (el test nuevo usa `Image`).

## 2. Decisiones que tomé (las pedía «mira cómo está hecho y decide»)

1. **Aro de `Boton_Pista`: más oscuro, sin tocar grosor ni hijos.** Medido: #E2571F sobre #E8A33D = **1,73:1** (sobre el marfil eran 3,28:1: por eso antes se leía). #A0330D = **3,27:1**, mismo matiz (≈16–17°) que el aro original. Probé en caliente seis variantes (`capturas/02-laberinto/decision-aro-pista.png`): base, carbón #3A1E18 (7,06:1, pierde el color del fuego), #A8340F (3,08:1, justo), 14 px del mismo color (más grueso pero sigue en 1,73:1 y obliga a tocar el `Fondo` y a `Algoritm`), carbón 10 px y #B8431A a 12 px (2,53:1). Elegí #A0330D: un solo valor serializado y 0,27 de margen sobre 3:1. Solo en este laberinto: los aros de Bosque, Río y Resumen siguen en #E2571F.
2. **`Fondo` de la tarjeta con ppum 1,5 (arcos concéntricos).** Con `ui_boton` a ppum 1 el radio de esquina es 24 px (medido sobre el PNG: el arco cruza la diagonal a ~7 px). Con inset 8 y mismo radio el aro mide **11,3 px en la diagonal** (v1). Con ppum 1,5 el radio interior es 16 = 24 − 8: aro uniforme de 8 px también en la curva. Comparación en `decision-esquina-tarjeta-ppum.png`.
3. **Margen de 40 px a cada lado del panel del entorno** (`Panel_World` offsets ±40, dato de escena; el código solo descuenta el grosor del marco). Sin margen (v1, `decision-v1-sin-margen.png`) la banda izquierda del marco quedaba **fuera de pantalla** (el entorno llena los 1296 px del panel) y la derecha se fundía con el contorno de la tarjeta: justo el «corte en las esquinas» de D4. 40 px es la cuadrícula que ya tiene la pantalla: el botón de pista empieza en x=40, la tablilla acaba en x=1256 y la tarjeta deja 40 px a la derecha; el marco ocupa x∈[40,1256] y deja 40 px hasta la tarjeta. Coste: el tablero baja de escala 0,675 a **0,625 (−7,4 %)**, casilla 62,3 → 57,7 px. Alternativas vistas: 24 px (−4,9 %, `decision-margen-24.png`) y 0 px (−1,2 %, v1).
4. **Marco del entorno detrás, anillo sin centro, ppum 3,25** (como pedía el brief). Con un hueco cuadrado y 8 px de grosor, 3,25 es el mayor redondeo exterior que mantiene 8 px también en la diagonal (radio ~7); uno mayor se comería el anillo en las esquinas. Por eso el marco del entorno tiene esquinas exteriores más cerradas (~7 px) que la tarjeta (24 px): mismo color, sprite y grosor, no mismo radio.
5. Guardianes en **EditMode** (escena guardada como vista previa) para lo estático que puedo ejecutar y ver en rojo, y en PlayMode lo que depende de `FitEnvironment`. Mutación comprobada: con `BackdropColor` puesto al tono del aro, las dos EditMode fallan (`Fondo_Escena y MazeLayout.BackdropColor son el mismo color` / `aro #A0330D sobre fondo #A0330D: 1,00:1`); restaurado, vuelven a verde.

## 3. Comandos y resultados

| Comando | Resultado |
|---|---|
| `git checkout -- Assets/Game/Scenes/Level2_Maze.unity` | restaura HEAD (descarta los 3 cambios de D3) |
| `editor.ps1 open Level2_Maze.unity`, `gameview1080` | Game View 1920×1080 |
| `editor.ps1 recompile` (varias veces) | `Compilación OK (errores=0)` siempre: tras el código, tras las pruebas nuevas (incluye los asmdef de PlayMode) y al refrescar el asset en la mutación |
| `editor.ps1 eval @edit1.cs` / `@edit2.cs` (edición de escena + `SaveScene`) | `guardada=True`; diff de la escena revisado línea a línea |
| `editor.ps1 tests-edit MazeSceneDataTests` | **2/2 pasan** |
| mutación de `BackdropColor` + `tests-edit MazeSceneDataTests` | 0/2 (fallan con el mensaje esperado); restaurado: **2/2** |
| `editor.ps1 tests-edit Game.Levels.Wheel.Tests assembly` | **83/83** (incluye las 2 nuevas) |
| `editor.ps1 tests-edit Game.Architecture.Tests assembly` | **23/23** |
| `editor.ps1 tests-edit Game.Content.Tests assembly` | **3/3** |
| **PlayMode** | **no corrí `tests-play` ni `VisualVerification`.** Sí usé Play **manual** (`exec editor_play` → `SceneManager.LoadScene("Level2_Maze")` → `ScreenCapture.CaptureScreenshot`) para las capturas y para **ensayar a mano las aserciones** de las pruebas PlayMode tocadas/nuevas (reflexión sobre los mismos miembros, mismas fórmulas, 1920×1080): 30 comprobaciones (29 de las pruebas y una extra, `Fondo_Escena` == panel), **0 fallos** (marco en x 40–1256, y 194,5–885,5; entorno 1200×675; grosores 8/8/8/8 en tarjeta y entorno; ppum 1,5; muesca #E0D4C0; panel #E8A33D). No son una corrida del runner. |

Reparo en procedimiento (para quien siga): `exec capture_game_view` con `save_path` guarda **relativo a `Assets/`** y dejó `Assets/claudeDocs/…` con sus `.meta` dentro del proyecto; lo borré con `AssetDatabase.DeleteAsset("Assets/claudeDocs")` y comprobé `git status`. Para capturar a 1920×1080 usé `ScreenCapture.CaptureScreenshot("<ruta absoluta>")` por `eval` en Play (el comando saca 1280×720). El Python del equipo **sí** tiene PIL (12.3.0, `C:\Program Files\Python312`), al contrario de lo que dice el brief en «Entorno».

## 4. Archivos tocados

```
 M Assets/Game/Data/Wheel/N2_MazeLayout.asset
 M Assets/Game/Scenes/Level2_Maze.unity
 M Assets/Game/Scripts/Runtime/Levels/Wheel/MazeLayout.cs
 M Assets/Game/Scripts/Runtime/Levels/Wheel/MazeSceneController.cs
 M Assets/Tests/EditMode/Levels/Wheel/Game.Levels.Wheel.Tests.asmdef
 M Assets/Tests/PlayMode/Levels/Wheel/MazeSceneTests.cs
?? Assets/Tests/EditMode/Levels/Wheel/MazeSceneDataTests.cs
?? Assets/Tests/EditMode/Levels/Wheel/MazeSceneDataTests.cs.meta
?? claudeDocs/tasks/Ajustes-Diseno-2026-10-08/   (este parte + capturas/02-laberinto/)
```
No toqué lo de Papá (`Papa.prefab`, sus `.anim`, herramientas de personajes) ni `CLAUDE.md`. Ningún cambio en `ProjectSettings`.

## 5. PlayMode que debe correr el revisor

Mínimo (las cinco que toqué o añadí):
```
pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-play MazeScene_RNF23_ElEntornoCabeEntero
pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-play MazeScene_RF30_
pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-play MazeScene_RF31_LaZonaDeSoltar
```
Recomendado, porque el inset de la tarjeta (5 → 8 px, −6 px de ancho y de alto de lista) y la escala del entorno (0,675 → 0,625) mueven geometría que otras pruebas miden: **toda la clase** `tests-play MazeScene_` (35 pruebas), más `WheelLevel_RNF19`, `WheelLevel_RNF20` y `WheelLevel_RNF21` (`WheelAccessibilityTests`, abren el laberinto), `PauseMenu_HU17` (`PauseMenuWheelTests`) y `CharacterCapture` con `Level2_Maze` (`CharacterCaptureTests`, capturas). Las que más riesgo tienen por geometría: `MazeScene_RNF03_*`, `MazeScene_RNF02_LaBarraDeDesplazamiento…`, `MazeScene_DA133_LaNinaMiraElTablero…` (todas relativas al rectángulo del entorno o de la ventana; no hay cifras fijas de la tarjeta en ninguna: lo busqué) y `MazeScene_DA76_ElBotonDePista…` (el `Fondo` y `Algoritm` del botón no cambiaron de tamaño).

## 6. Documentos que dicen hoy #F7EFE2 o el `Outline` del laberinto (a corregir por el agente de documentos)

| Ruta : líneas | Qué dice hoy | Debe decir |
|---|---|---|
| `CLAUDE.md:93-98` | «`MazeLayout.BackdropColor` pinta el panel que lo rodea con el marfil de interfaz (`#F7EFE2`), con un contorno `#C4A882` (`environmentBorderColor` en `MazeSceneController`) como el marco del panel de diálogo» | fondo único ámbar `#E8A33D` (`Fondo_Escena` + `BackdropColor`, los vigila `MazeSceneDataTests`); marco `Marco_Entorno` (anillo `ui_boton`, 8 px del lienzo, `environmentBorderColor`/`environmentBorderWidth`) igual que `Panel_Secuencia`, ya no un `Outline`; margen de 40 px del panel |
| `claudeDocs/SPEC.md:258-260` | «`MazeLayout.BackdropColor` pinta su panel con el marfil de interfaz y un contorno `#C4A882`» | ídem |
| `claudeDocs/INCONSISTENCIAS.md:1113-1116` | Nota 07/10: «…con panel marfil `#F7EFE2` y contorno `#C4A882`» | nota fechada 08/10 (Santiago) que lo corrija; la nota del 07/10 puede quedar como historia |
| `claudeDocs/Direccion_de_Arte.md:604-606` | «…su entorno se ve tal cual el arte, con panel marfil `#F7EFE2` y contorno `#C4A882` (decisión de Santiago, 07/10/2026)» | fondo ámbar `#E8A33D`, contorno `#C4A882` de 8 px |
| `claudeDocs/entregables/OE3/src/anexos/C-slice-2.md:125` | «…con el panel que lo rodea en marfil de interfaz (`#F7EFE2`) y un contorno `#C4A882`, como el marco del panel de diálogo» | ídem |
| `claudeDocs/entregables/OE3/src/A-matriz-rf.md:94` | RF-30 → `MazeScene_RF30_ElLaberintoEsAlAtardecer` (ya estaba obsoleta desde el 07/10) | se regenera con `rf_matrix.py --worktree`: recogerá `MazeScene_RF30_ElEntornoQuedaSinTinte`, `…ElEntornoYLaTarjetaLlevanElMismoMarcoRedondeado`, `…LaSalidaSeLeeEnElEntornoYElFondoEsUnoSolo`, `…ElFondoDeLaEscenaYElDelAssetSonElMismoAmbar`, `MazeScene_RF31_LaZonaDeSoltar…` y `MazeScene_RNF20_ElAroDelBotonDePista…` |
| `claudeDocs/entregables/OE3/src/04b-nivel2.md:106` (Figura 4.7: `fig-04b-4-laberinto-editor.jpg`, fuente `claudeDocs/tasks/OE4/evidencias/figuras/fig-04b-4-laberinto-editor.png`) | pie «Fase 3 del Nivel 2 **al atardecer**…» y la imagen con el fondo verde oliva | ya contradecía la decisión del 07/10 (sin tinte); conviene recapturar (sirven `capturas/02-laberinto/despues-05-bloques-y-cajon.png` o una nueva con «Girar» elegido) |

Otros sitios que citan nombres de pruebas renombradas: solo `brief.md` (no es documentación del proyecto). No hay `Outline` del laberinto descrito en `Interfaces.md` (su fila 10 solo dice «✓ `MazeSceneController` (W13/W14…)»).
Matiz para `Direccion_de_Arte.md` §4.3/§10.2: el ámbar `#E8A33D` pasa a ser, en el laberinto, a la vez el color de «Atención» (lado elegido de «Girar», relleno de «Ejecutar», pista) y el fondo de toda la pantalla; el aro de la pista ya no es el de fuego `#E2571F` sino `#A0330D` solo en este nivel.

## 7. Capturas (`claudeDocs/tasks/Ajustes-Diseno-2026-10-08/capturas/02-laberinto/`, todas 1920×1080 salvo recortes)

- Antes (HEAD): `antes-01-completa.png`, `antes-02-esquinas-tarjeta.png`, `antes-03-esquinas-entorno.png`, `antes-04-boton-pista.png`.
- Después: `despues-01-completa.png` (reposo), `despues-02-esquinas-tarjeta.png`, `despues-03-esquinas-entorno.png` (cuatro esquinas de cada una, ×6), `despues-04-boton-pista.png`, `despues-05-bloques-y-cajon.png`, `despues-06-muchos-bloques.png`, `despues-07-union-entorno-tarjeta.png` (el hueco de 40 px entre los dos marcos).
- Decisiones: `decision-aro-pista.png`, `decision-esquina-tarjeta-ppum.png`, `decision-margen-24.png`, `decision-margen-40.png`, `decision-v1-sin-margen.png`.
- Robustez: `robustez-16x10-4x3-21x9.png` (1280×800, 1024×768, 2560×1080: marco entero y cerrado en 16:10 y 4:3; en 21:9 el entorno se limita por la altura —ya llenaba el alto del panel antes— y la tablilla de mensaje le tapa la parte alta (también la banda superior del marco); en 16:10 y 4:3 la tablilla se mete bajo la tarjeta: este cambio no mueve ni la tablilla ni la tarjeta y no lo comparé contra `HEAD`; el juego se fija a 16:9).

Esquinas cerradas, medido sobre las capturas (px perpendiculares de contorno #C4A882; lados / diagonal):

| | lados | diagonal de las 4 esquinas |
|---|---|---|
| Antes · tarjeta | 5 | **0** (cortado) |
| v1 · tarjeta (ppum 1) | 8 | 11,3 |
| **Final · tarjeta (ppum 1,5)** | 8 | 7,1 (8 con el antialiasing de los bordes) |
| **Final · marco del entorno** | 8 | 8,5 |
| v1 · marco del entorno (sin margen) | 0 a la izq. (fuera de pantalla) · 11 a la der. (fundido con la tarjeta) | 0 / 4,2 |

## 8. Dudas

1. **El tablero encoge 7,4 %** por el margen de 40 px. Si Santiago lo prefiere grande: `Panel_World` offsets a 0 (−1,2 %), pero el marco queda pegado al borde izquierdo y a la tarjeta; o 24 px (−4,9 %).
2. **Las esquinas exteriores del marco del entorno (~7 px) son más cerradas que las de la tarjeta (24 px).** Es el máximo redondeo que deja 8 px en la diagonal con un hueco cuadrado. Para igualar el radio habría que enmascarar la ilustración con una `Mask` redondeada (más trabajo y una pasada de stencil por la escena). Lo dejé como el brief.
3. **`Panel_World` conserva su color guardado #6E698C** (marcador de posición que el código pisa al arrancar). En el Editor, sin Play, se ve violeta sobre el ámbar; en el juego es ámbar. No lo toqué (tercera copia del mismo valor); dime si prefieres igualarlo.
4. **El aro #A0330D no está en la paleta** de `Direccion_de_Arte.md` (es #E2571F oscurecido al mismo matiz). La alternativa con paleta es el carbón #3A1E18 (7,06:1, como el borde de «Ejecutar» y de pausa), a costa de perder el color del fuego.
5. `Image_Environment` guarda `sizeDelta` 1599×899 pero el sprite mide 1920×1080 (lo sobrescribe `FitEnvironment`): preexistente, sin efecto en juego.
6. El pixel del marco se vigila en PlayMode por estructura (sprite, grosor, hueco = ilustración, marco entero en pantalla), **no** por muestreo de píxeles de captura. Lo de las esquinas lo comprobé yo sobre capturas (tabla de arriba). Si se quiere un guardián por píxeles, se puede añadir como `[Category("VisualVerification")]`.

## 9. Kanban

La última acta es la **D10 (30/09/2026)**; su §6 solo trae D06-2, D07-2, D08-2 y D10-1…D10-8 (nivel 1, nivel 2: escena 2.2 y carretilla amarrada, nivel 3, arte, cargas, arnés, entregable, capítulo 8). **Ninguna tarjeta cubre el laberinto ni estos ajustes de interfaz**, y `79e3ea7` (laberinto, 07/10) tampoco cita tarjeta. Los commits de personajes cierran con `Kanban: Personajes finales (INC-131, INC-133).`

## 10. Mensaje de commit propuesto

```
Maze: one amber backdrop, rounded 8 px frames on environment and card, drop zone restored

D2: Canvas/Fondo_Escena and MazeLayout.BackdropColor are the same amber
#E8A33D (the old ivory is gone); MazeSceneDataTests keeps the two from drifting
apart. The Boton_Pista ring goes from #E2571F to #A0330D (3.27:1 on the amber,
was 1.73:1) in this scene only.

D1/D3: Ventana_Secuencia is #E0D4C0 with ui_boton Sliced again, and
ivoryShadeColor stays #E0D4C0 (the uncommitted #E8C27D, Fondo_Escena #E8C27D
and the URP camera component are discarded).

D4: the card's Fondo is a rounded ui_boton again, inset 8 px with a concentric
arc (ppum 1.5), so the outline no longer vanishes at the corners (0 px on the
diagonal before, ~8 px now). The environment's Outline, which scaled with the
illustration, is replaced by a Marco_Entorno ring sibling (ui_boton, no centre,
8 px of canvas, sized by FitEnvironment); the environment fits inside
Panel_World minus the frame, on a 40 px margin, so the whole frame is on screen
and does not touch the card.

Tests: MazeScene_RF30_ElEntornoYLaTarjetaLlevanElMismoMarcoRedondeado and
MazeScene_RF31_LaZonaDeSoltarEsMarfilSombraSobreLaTarjeta (PlayMode), plus
MazeSceneDataTests (EditMode); the two RF30 tests that asserted the ivory
panel and the Outline are adjusted and renamed.

Decisión de Santiago, 08/10/2026. Documents (CLAUDE.md, SPEC.md,
Direccion_de_Arte.md, INCONSISTENCIAS.md, OE3 Annex C) follow in their own commit.
Kanban: no open card covers it (last acta D10, 30/09/2026).

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
```
