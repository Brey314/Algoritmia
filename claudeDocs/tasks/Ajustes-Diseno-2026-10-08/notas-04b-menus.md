# Etapa 4b — Menús: portada del inicio, título y menú de niveles (08/10/2026)

Parte crudo del implementador. Decisiones D9, D10 y D11 de `brief.md`. **Sin commit**; no toqué documentación ni lo ajeno (Papá, laberinto, narrativa, Algoritm por partes, `Wave`,
créditos). Partí con el Editor en `Narrative.unity`, limpia, y lo dejo igual. **No corrí PlayMode ni `VisualVerification`** (§8): las PlayMode que escribí las corre el revisor.

## 1. Qué quedó

| Pedido | Resultado |
|---|---|
| Portada (D9) | La mitad izquierda del bosque del N2 (`env_n2_bosque_claro`, foco 0,25, zoom 1) con los cinco personajes en `Idle`, desfasados. La ventana es **siempre cuadrada** (`AspectRatioFitter`): la composición es la misma a 16:9, 16:10 y 4:3. |
| `FramedIllustration` | Componente nuevo en `Game.UI` (`FramedIllustration.cs`, 87 líneas): aplica `IllustrationFraming` a la imagen hija **desde la ventana** (§3, decisión 1). Lo usan la portada y las tres tarjetas del menú de niveles. |
| `CharacterRig.idlePhase` | Campo nuevo `[Range(0,1)]`, 0 por defecto, más `IdlePhase` de solo lectura. `Apply` arranca el `Idle` en esa fase del ciclo y **solo el `Idle`**: los demás gestos arrancan en su primer cuadro. |
| Título (D10) | Fuera de la tarjeta y encima: blanco, 112 px, `Outline` `#3A1E18` de (4, −4). Tarjeta de 280 → 120 px de alto, solo con el lema. Título, tarjeta y botones cuelgan de un `Bloque` anclado en (0; 0,5): 222 px de margen arriba y abajo. `MainMenuController.titleLabel` sigue apuntando al mismo `Text`. |
| Menú de niveles (D11) | Cada tarjeta tiene una `Ventana` de 444 × 250 con la imagen. El icono (candado o visto) va abajo a la izquierda de la imagen, a 16 px de los dos bordes; el texto, centrado entre la imagen y el botón. La imagen del Nivel 2 es ahora el bosque (foco 0,25), no el laberinto. |

## 2. Antes y después

### 2.1 `MainMenu.unity` — portada

| | Antes (HEAD) | Después |
|---|---|---|
| `MainPanel/Silueta/IlustracionInicio` | `Image` `#F7EFE2` Sliced, 952 × 952 (estirada con −128), con `Algoritm_Fuego(MenuInicio)` de 300 × 300 en el centro | Contenedor sin gráfico (se quitaron `Image` y `CanvasRenderer`), mismo rect (estirado con −128) |
| `…/IlustracionInicio/Ventana` | — | `RectTransform` estirado + `AspectRatioFitter` (`FitInParent`, 1) + `RectMask2D` + `FramedIllustration` (`Art` = `Bosque`, foco (0,25; 0,5), zoom 1). A 16:9: 952 × 952; a 4:3 (1600 × 1200): 668 × 668 |
| `…/Ventana/Bosque` | — | `Image` con `env_n2_bosque_claro` (3840 × 1080, `raycastTarget` apagado). A 16:9: tamaño 3840 × 1080, escala 0,8815, posición (846,2; 0); se ve x ∈ [0,1094; 0,3906] del lienzo |
| Personajes | Una instancia de Algoritm de 300 × 300 | Cinco instancias de prefab hijas de `Bosque`, ancladas en fracciones del lienzo, casilla cuadrada = fracción del alto (1080), pivote (0,5; 0,5) |

Los cinco, en el orden de dibujo (de atrás adelante):

| Instancia | Ancla (x, y) en el lienzo | Casilla | `idlePhase` | Proporción en `N2_PuenteI` (casilla) |
|---|---|---|---|---|
| `Mama(MenuInicio)` | (0,19922; 0,46430) | 0,415 → 448,2 px | 0,35 | 0,236 |
| `Papa(MenuInicio)` | (0,27214; 0,48900) | 0,450 → 486,0 px | 0 | 0,2542 |
| `Nina(MenuInicio)` | (0,16146; 0,28760) | 0,280 → 302,4 px | 0,70 | 0,1588 |
| `Nino(MenuInicio)` | (0,33073; 0,27760) | 0,280 → 302,4 px | 0,15 | 0,1595 |
| `Algoritm_Fuego(MenuInicio)` | (0,23568; 0,30980) | 0,190 → 205,2 px | 0,50 | 0,1 |

Cómo salieron: las proporciones son las de las narrativas del N2 a escala ×1,76 (Mamá 0,93 de Papá, los niños 0,62, Algoritm 0,42 frente a 0,39 de `N2_PuenteI`), porque a zoom 1 la ventana
enseña el alto entero del lienzo y la casilla de la narrativa sería pequeña. La **y del ancla** = pies + 0,42 × casilla (en la familia los pies están al 8 % del lienzo del rig y la sombra
va dentro del prefab): pies de los padres a 0,29–0,30 del alto (al fondo), de los niños a 0,16–0,17 (delante) y Algoritm a 0,23 (flota delante de los padres, sin sombra: su prefab no la trae).
Los adultos al centro, los niños en los lados y Algoritm en el centro delante, como un retrato de familia. Se ve en `despues-menu-principal.png`.

Fases de reposo: 0 · 0,15 · 0,35 · 0,50 · 0,70 → huecos de 0,15 / 0,20 / 0,15 / 0,20 / 0,30 (el último, de 0,70 a 1,00 del ciclo de Papá). En la captura los cinco están en poses
distintas (Papá con las manos en la cintura, Mamá levantando una mano, los niños quietos).

### 2.2 `MainMenu.unity` — bloque del título

| Elemento | Antes | Después (dentro de `Bloque`, esquina superior izquierda) |
|---|---|---|
| `Bloque` | — | Anclas (0; 0,5), pivote (0; 0,5), posición (64; 0), tamaño 712 × 636 |
| `TitleLabel` | Hijo de `TitlePanel/Fondo`, (48; −40), 616 × 120, Baloo2-ExtraBold **96** `#3A1E18`, `UpperLeft` | Hijo de `Bloque`, (0; 0), 712 × 140, Baloo2-ExtraBold **112**, **blanco**, `MiddleLeft`, desbordes en `Overflow`, + `Outline` `#3A1E18` (4; −4) |
| `TitlePanel` (tarjeta) | (64; −272), 712 × **280** | (0; −148), 712 × **120** |
| `Lema` | (48; −168), 616 × 60, `UpperLeft` | Estirado en horizontal con 48 de margen, centrado en vertical, `MiddleLeft` |
| `PlayButton` | (64; −600), 712 × 96 | (0; −316) |
| `CreditsButton` / `ExitButton` | (64; −720) / (432; −720), 344 × 88 | (0; −436) / (368; −436) |
| `TeacherReportButton` | (64; −832), 712 × 88 | (0; −548) |
| Margen arriba / abajo | **272 / 160** (el bloque no estaba centrado) | **222 / 222** |

Alturas del bloque: título 140 + hueco 8 + tarjeta 120 + hueco 48 + «Jugar» 96 + 24 + fila 88 + 24 + «Progreso» 88 = 636. La altura natural del título a 112 px es 180 (la línea de Baloo 2 es alta); le di
140 con `verticalOverflow = Overflow` y centrado: así las letras quedan a ~18 px del borde superior del rect y la descendente de la «g» a ~25 px de la tarjeta. A la vista, el espacio de arriba (hasta la
cabeza de las letras, ~240 px) y el de abajo (222) difieren 18 px; el rect del título, que es lo que mide la prueba, sí queda a 222.

**Reborde.** Un solo `Outline` de (4; −4) se ve continuo (`decision-titulo-reborde-ampliado-x3.png`, ×3 sin suavizar): contorno de 4 px parejo en las diagonales de la «A» y en las curvas; el único hueco es una
rendija de ~2 px dentro de la «g», entre su panza y su cola, que el contorno no cierra porque la separación del dibujo es mayor que 8 px. **No apilé un segundo `Outline`**: en uGUI los efectos apilados
**suman** sus desfases (cada uno duplica lo que dejó el anterior) y un segundo de (4; 0) daría un reborde de hasta 8 px.

### 2.3 `LevelSelect.unity` — cada tarjeta (`Level{1,2,3}Card/Fondo`)

| | Antes | Después |
|---|---|---|
| Imagen | `Arte` (444 × 300 en (32; −32), `preserveAspect`, tinte `#E0D4C0`) | `Ventana` (444 × 250 en (32; −32), `RectMask2D` + `FramedIllustration`) con `Arte` dentro, **sin `preserveAspect`**, mismo tinte. N1 y N3: 1920 × 1080 a escala 0,2315, foco (0,5; 0,5). N2: 3840 × 1080, foco (0,25; 0,5), posición (222,2; 0), visible x ∈ [0,0003; 0,4998] |
| Sprite del N2 | `env_n2_laberinto` (1920 × 1080) | `env_n2_bosque_claro` (3840 × 1080) |
| Insignias | `LockedBadge` / `CompletedBadge` (200 × 140) hijas de `Arte`, arriba a la derecha (−16; −16) | Hijas de `Fondo`, estiradas sobre la tarjeta; orden: ventana, insignias, botón. Siguen siendo los mismos GameObject que el controlador activa |
| Icono (`LockIcon` / `CompletedIcon`) | 88 × 88 arriba a la derecha de la imagen | 88 × 88 con ancla y pivote arriba a la izquierda en (48; −178): **16 px a la derecha y 16 px arriba del borde de abajo de la imagen** (la imagen acaba en y = −282) |
| Texto (`LockedText` / `CompletedText`) | 200 × 40, `UpperRight`, en (0; −96) de la insignia, sobre una pastilla | 444 × 40, centrado (`MiddleCenter`) con ancla arriba al centro en (0; −319): el centro de la franja entre la imagen (−282) y el botón (−356), 74 px |
| Pastilla `Fondo` de cada insignia | `ui_panel` `#F7EFE2`, 216 × 48 | **Quitada** (seis objetos): sobre la tarjeta marfil habría sido marfil sobre marfil |

`LevelSelectController` no cambió de lógica: solo ganó dos accesores `internal` (`LockedBadgeFor`, `CompletedBadgeFor`) en su bloque `#if UNITY_INCLUDE_TESTS`.

## 3. Decisiones que tomé

1. **`FramedIllustration` va en la ventana, no en la imagen** (el brief lo ponía en `Bosque`). Lo probé en el motor: con el componente en la imagen, en Play esta se quedó a **escala 0**, y cambiar el tamaño de la ventana (`sizeDelta` ±2) no la arreglaba: la imagen está
   anclada al centro con tamaño fijo y `OnRectTransformDimensionsChange` no le llega cuando cambia su padre. En el primer `OnEnable` la ventana medía 0 × 0 (lo más probable: el `Canvas` aún no había repartido el tamaño), y el encuadre con una ventana vacía da escala 0. Con el componente en la
   ventana, el aviso le llega a ella, y además `Apply` **no hace nada con una ventana vacía**. Lo segundo lo vigila `FramedIllustration_RF01_UnaVentanaSinTamanoNoDejaLaImagenAEscalaCero` (en rojo si se quita la guarda, §4).
2. **Ventana cuadrada con `AspectRatioFitter`.** Con la ventana estirada como estaba, a 4:3 era vertical (695 × 1119) y solo enseñaba 671 de los 1080 px nativos de ancho: la familia se recortaba. Con
   el repartidor (`FitInParent`, 1:1) la composición no cambia con la proporción de pantalla: a 4:3 la portada mide 668 × 668 y queda centrada en el panel beige. Costó un nivel más de jerarquía (`IlustracionInicio` →
   `Ventana` → `Bosque`).
3. **`Bloque`** (como pedía el brief) con anclas (0; 0,5): centrar es una sola ancla y no seis. Los botones cambiaron de padre (`MainPanel` → `Bloque`); ningún código ni prueba los busca por ruta (comprobado con búsqueda en
   `Assets/`): se buscan por su rótulo o por referencia serializada.
4. **Un `Outline`** y no dos (§2.2).
5. **Título a la izquierda** (como en el boceto y como el lema): ver duda 1.
6. **Pastilla de las insignias quitada** (§2.3).
7. **Las pruebas nuevas miden rectángulos dibujados** (`GetWorldCorners`) con un ayudante compartido, `IllustrationProbe` (PlayMode, nuevo): qué tramo del lienzo cae en la ventana, rect unidos, etc. No usan el código que prueban.

## 4. Pruebas

Todo por `editor.ps1`, solo EditMode, con `recompile` antes (`Compilación OK, errores=0` cada vez).

| Corrida | Resultado |
|---|---|
| `tests-edit FramedIllustrationTests` (nueva, `Game.UI.Tests`) | **7 / 7** |
| `tests-edit CharacterRigIdlePhaseTests` (nueva, `Game.Scaffolding.Tests`) | **25 / 25** |
| `tests-edit Game.UI.Tests` (assembly) | **36 / 36** (29 antes + 7) |
| `tests-edit Game.Scaffolding.Tests` (assembly) | **321 / 321** (296 antes + 25) |
| `tests-edit Game.Architecture.Tests` / `Game.Content.Tests` (assembly) | **23 / 23** · **3 / 3** |
| Mutación 1: `idlePhase` ignorado en `CharacterRig.Apply` | `CharacterRigIdlePhaseTests` **15 / 25** (fallan las 10 con fase ≠ 0); restaurado: 25 / 25 |
| Mutación 2: sin la guarda de ventana vacía en `FramedIllustration.Apply` | `FramedIllustrationTests` **6 / 7** (falla `…UnaVentanaSinTamanoNoDejaLaImagenAEscalaCero`); restaurado: 7 / 7 |

Comandos (todos con `pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 …`):

```
open Assets/Game/Scenes/MainMenu.unity ; gameview1080               # Game View a 1920x1080
eval @build_mainmenu.cs 60000   / eval @build_levelselect.cs 60000   # construyen y guardan la escena (efímeros, no versionados)
recompile                                                            # «Compilación OK (status=completed, errores=0)» tras cada cambio de código
tests-edit FramedIllustrationTests | CharacterRigIdlePhaseTests     # y las cuatro assemblies de la tabla, con `assembly` como filter_type
exec editor_play ; eval 'UnityEngine.ScreenCapture.CaptureScreenshot(@"<ruta absoluta>")' ; exec editor_stop      # capturas
eval 'UnityEditor.PlayModeWindow.SetCustomRenderingResolution(1920, 1200, "…")'    # 16:10; 1600 x 1200 para 4:3; luego gameview1080
```

Pruebas nuevas o cambiadas:

- **PlayMode** `MainMenuTests` (13 → 17): `MainMenu_RF01_LosCincoPersonajesEsperanEnReposoSinRespirarAlUnisono` (cinco retratos distintos, todos en `Idle` —también su `Animator`—, fases con huecos ≥ 0,1 del ciclo),
  `MainMenu_RF01_ElBosqueNoCruzaLaCosturaDelLienzo` (sprite `env_n2_bosque_claro`, tramo visible dentro de [0; 0,5] con un píxel de tolerancia), `MainMenu_RF01_TituloTarjetaYBotonesQuedanCentradosEnVertical` (el título no es hijo de la tarjeta, va encima de ella, la tarjeta encima de los botones y el bloque deja
  el mismo espacio arriba y abajo, ±1 px) y, **extra**, `MainMenu_RF01_LosCincoPersonajesQuedanDentroDeLaPortada` (la casilla de cada uno cabe en la ventana: protege que la máscara no recorte a nadie). Se actualizó la descripción de la `VisualVerification` `MainMenu_RNF20_…`.
- **PlayMode** `LevelSelectTests` (12 → 13): `LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura` **ajustada** (ya no exige 16:9: el N2 es el lienzo duplicado; exige 16:9 o duplicado, que la ilustración llene la ventana y que, si es duplicado, la ventana no cruce x = 0,5) y **nueva**
  `LevelSelect_RNF19_ElIconoDeEstadoVaAbajoALaIzquierdaDeLaImagenYSuTextoEntreImagenYBoton` (en las tres tarjetas y con **las dos insignias** activadas a la fuerza: icono a 16 ± 1,5 px de la izquierda y del borde de abajo de la imagen y dentro de ella; texto centrado respecto de la
  imagen, debajo de ella, encima del botón y a media altura de la franja). El ayudante `ArtFor` pasó a `WindowFor`.
- **EditMode** `FramedIllustrationTests` (4 casos de ventana: la portada, la portada a 4:3, la tarjeta y una ventana vertical enseñan solo la mitad izquierda del lienzo duplicado; la imagen cubre sin deformarse; una ventana sin tamaño no la deja a escala cero; se reajusta si cambia la ventana)
  y `CharacterRigIdlePhaseTests` (el `Idle` arranca en la fase pedida en los cinco prefabs con tres fases; un gesto —`Talk`— arranca en 0 aunque haya fase; sin fase, 0 como siempre).

### Ensayo a mano de las PlayMode (no es una corrida del runner)

Sin `tests-play`: entré en Play a mano (`exec editor_play`, Game View a 1920 × 1080) y ejecuté con `eval` **las mismas comprobaciones, con las mismas fórmulas**, sobre la escena recién cargada con `SceneManager.LoadSceneAsync` y la misma espera de las pruebas (dos fotogramas):

- `MainMenu`: 5 rigs con los cinco retratos, todos `Idle` (rig y `Animator`), fases 0 · 0,15 · 0,35 · 0,5 · 0,7 con huecos ≥ 0,15; tramo visible del bosque [0,1094; 0,3906]; las cinco casillas dentro de la ventana; el título fuera de la tarjeta, título.yMin 718 ≥ tarjeta.yMax 710, tarjeta.yMin 590 ≥
  botones.yMax 542, márgenes 222,00 / 222,00. Los cuatro botones se alcanzan por raycast y caben sin solaparse (las pruebas viejas `…AlcanzablesPorRaycast` y `…NoSeSolapan`).
- `LevelSelect`: las tres tarjetas, las dos insignias de cada una: **0 fallos** en las ocho condiciones (icono a 16 px, texto a 505,0 = (542 + 468) / 2).
- Apagar y encender `MainPanel` (lo que hace «Jugar» → «Volver»): la escala del bosque y las fases se conservan.
- Consola sin errores ni avisos nuevos de lo mío (los de `LimbFollower`/`ArmLayering` los provocan a propósito pruebas de otra etapa; `PropShadow.cs(453) CS0108` es de antes).
- Carga en el Editor (línea `RNF-04` de `SceneLoader`): `MainMenu` 0,55–0,86 s después del cambio (0,56 s antes); `LevelSelect` 0,55 s (0,64 s antes). No es una medida del ejecutable.

## 5. Archivos tocados (solo los míos)

```
 M Assets/Game/Scenes/MainMenu.unity                        (+815 −216)
 M Assets/Game/Scenes/LevelSelect.unity                     (+263 −515)
 M Assets/Game/Scripts/Runtime/Scaffolding/CharacterRig.cs  (+12 −1: idlePhase, IdlePhase, Apply)
 M Assets/Game/Scripts/Runtime/UI/LevelSelectController.cs  (+2: accesores de prueba)
 M Assets/Tests/PlayMode/UI/MainMenuTests.cs                (+94 −2)
 M Assets/Tests/PlayMode/UI/LevelSelectTests.cs             (+64 −9)
?? Assets/Game/Scripts/Runtime/UI/FramedIllustration.cs     (+ .meta)
?? Assets/Tests/PlayMode/UI/IllustrationProbe.cs            (+ .meta)
?? Assets/Tests/EditMode/UI/FramedIllustrationTests.cs      (+ .meta)
?? Assets/Tests/EditMode/Scaffolding/CharacterRigIdlePhaseTests.cs (+ .meta)
?? claudeDocs/tasks/Ajustes-Diseno-2026-10-08/notas-04b-menus.md y capturas/04b-menus/
```

Las escenas las edité con scripts efímeros por `editor.ps1 eval` (`ObjectFactory`, `PrefabUtility.InstantiatePrefab`, `SerializedObject`, `EditorSceneManager.SaveScene`), guardados en el directorio de trabajo de la sesión y **no versionados**; el diff de las escenas es el registro. No quedó nada en
`Assets/Editor` ni en `Assets/` fuera de lo de arriba (`git status` comprobado). Sin cambios en `ProjectSettings`, `Packages`, prefabs, arte ni `.asmdef`. Las capturas se escribieron con `ScreenCapture.CaptureScreenshot` a rutas absolutas, fuera de `Assets/`.

## 6. PlayMode que debe correr el revisor

Mínimo (las cinco nuevas y la ajustada):

```
pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-play MainMenu_RF01_
pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-play LevelSelect_RNF19_ElIconoDeEstado
pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-play LevelSelect_RF03_CadaTarjeta
```

Recomendado, porque las dos escenas cambiaron de jerarquía y de peso (la portada carga el lienzo de 3840 y cinco rigs):

```
tests-play MainMenu_              # toda la clase (17; `MainMenu_RNF20_…` es VisualVerification)
tests-play LevelSelect_           # toda la clase (13; `LevelSelect_RNF19_ElEstadoBloqueado…` es VisualVerification)
tests-play ProfileSelect_         # comparte la escena MainMenu: su panel cuelga de otra rama, pero la escena es la misma
tests-play BootFlow_              # y GameFlowRunner_ y SceneLoader_: cargan MainMenu o LevelSelect
```

y, si quiere verlo, `MainMenu_RNF20_ContrasteTextoFondoSuficiente` (`VisualVerification`, no la corrí): su descripción ahora pide mirar el reborde del título y los cinco personajes.

## 7. Capturas — `claudeDocs/tasks/Ajustes-Diseno-2026-10-08/capturas/04b-menus/` (Play manual, Game View 1920 × 1080, `ScreenCapture.CaptureScreenshot`)

| Archivo | Qué es |
|---|---|
| `antes-menu-principal.png` · `despues-menu-principal.png` | El inicio, antes (tarjeta con título y lema, Algoritm de 300 px en un cuadro marfil vacío) y después |
| `despues-menu-principal-16x10.png` (1920 × 1200) · `despues-menu-principal-4x3.png` (1600 × 1200) | El inicio en otras proporciones: el bloque sigue centrado y la portada, cuadrada y con los cinco enteros |
| `antes-seleccion-niveles-completado-disponible-bloqueado.png` · `despues-seleccion-niveles-…` | Un perfil con el N1 completado, el N2 disponible y el N3 bloqueado: los tres estados en una sola captura, antes y después |
| `decision-titulo-reborde-ampliado-x3.png` | El título ×3 sin suavizar, para ver que el reborde es continuo |

En el menú de niveles, el nivel **disponible** no lleva insignia (hay un hueco entre la imagen y el botón; es la franja que ocupan «Completado» y «Bloqueado»).

## 8. Dudas y límites

1. **Título a la izquierda o centrado.** Lo dejé a la izquierda, alineado con la tarjeta y los botones, como en el boceto. Centrado sobre el bloque también se leería bien; es un valor de escena (`TitleLabel`: alineación y rect).
2. **A 4:3 la portada encoge** (668 px frente a 952) por mantener la composición. Si Santiago la prefiere llenando el panel, hay que quitar el `AspectRatioFitter` y mover a los personajes hacia el centro (a 4:3 solo caben 671 de los 1080 px del lienzo).
3. **RNF-20 y el título.** El blanco sobre el ocre `#E8C27D` da 1,69:1 por sí solo; lo que lo hace legible es el reborde (`#3A1E18` sobre el ocre, 9,03:1; blanco contra el reborde, 15,23:1). El caso `PF-RNF20-01` del OE4 muestrea «color del texto y del fondo» en la pantalla de inicio: si
   el muestreo ignora el reborde, el título saldrá bajo 4,5:1. El texto del menú de niveles (`#3A1E18` sobre marfil) da 13,34:1.
4. **El borde derecho de la ventana de la tarjeta del N2 toca el eje del espejo** (visible hasta x = 0,4998). La ventana mide 444 × 250 (1,776 de proporción, apenas bajo 16:9 = 1,778): una más ancha cruzaría la costura. Lo vigila `LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura`.
5. **Algoritm no tiene sombra** (su prefab no la trae); en la portada flota a 0,23 del alto sobre el prado. No usé `PropShadow` (su regla excluye a los personajes, que traen la suya en el prefab). Si se quiere, es una elipse en el prefab.
6. **Las pruebas PlayMode son nuevas y no las corrí**; solo el ensayo de §4. El riesgo mayor es el tiempo: pasan dos fotogramas entre la carga de `MainMenu` y la medida, y la ventana depende de que el `Canvas` haya repartido el tamaño (en el ensayo ya lo había hecho a los dos fotogramas).
7. **Evidencias del OE4 y casos que se quedan viejos**: `claudeDocs/tasks/OE4/evidencias/figuras/fig-07-1-pantalla-de-inicio.png` y `fig-07-2-menu-de-niveles.png` son del ejecutable rc2 y muestran el diseño anterior; los casos `PF-*` que miren la posición de los botones del inicio
   (`casos.md`) o de las insignias del menú de niveles deberían releerse: los botones del inicio **subieron 62 px** (de 600 / 720 / 832 a 538 / 658 / 770 desde arriba, a 1080 p), y «Completado» / «Bloqueado» ya no están dentro de la imagen.
8. **El brief dice que el Python del equipo no tiene PIL**: sí lo tiene (12.3.0); lo usé solo para recortar y ampliar las capturas.

## 9. Documentos que habrá que corregir (ruta y línea; las líneas son del árbol de trabajo del 09/10/2026)

| Archivo | Línea | Qué dice hoy | Corrección |
|---|---|---|---|
| `claudeDocs/Interfaces.md` | 43–44 («**2 · Pantalla de inicio**») | «Título del juego (`GameTitleConfig`), el lema «Piensa el orden, enciende el fuego», la figura de Algoritm en fuego, botón primario de jugar, …» | El título (blanco, reborde carbón de 4 px) encima de una tarjeta corta con el lema; la portada a la derecha es la mitad izquierda del bosque del N2 con Papá, Mamá, Niño, Niña y Algoritm de fuego en reposo, desfasados; título, tarjeta y botones, centrados en vertical |
| `claudeDocs/Interfaces.md` | 57–63 («**4 · Menú de niveles**») | Describe el bloqueo y la marca «Completado» sin decir dónde van | Icono abajo a la izquierda de la imagen y su texto centrado entre la imagen y el botón; la imagen del Nivel 2 es el bosque (foco 0,25), no el laberinto |
| `claudeDocs/Direccion_de_Arte.md` | 893 (§11.3, fila «Título de pantalla») | «64 px (el título del juego en la portada, 96 px en Baloo 2 ExtraBold)» | **112 px**, blanco con reborde `#3A1E18` de 4 px |
| `claudeDocs/Direccion_de_Arte.md` | 910–911 (§11.4) | «Contorno de texto: `#3A1E18` de 3 px cuando el texto va sobre el escenario y no sobre panel.» | Añadir que el título del juego lleva 4 px (va sobre el fondo ocre, no sobre panel) |
| `claudeDocs/Direccion_de_Arte.md` | 1350 (fila `PG-01`) | «La pantalla de inicio lo rotula en Baloo 2 ExtraBold, 96 px, `#3A1E18`, sin logotipo en imagen» | 112 px, blanco con reborde `#3A1E18` de 4 px, fuera de la tarjeta |
| `claudeDocs/Mockups de interfaz Algoritmia.html` | 384 (la única línea con contenido: el índice está en la columna ~252 250, `{ id: "inicio", label: "2 · Inicio", nota: "definitiva" }` y `{ id: "niveles", label: "4 · Menú de niveles", nota: "línea del tiempo con tarjeta" }`) | **Solo anotar, no editar.** El mockup 2 enseña el título dentro de la tarjeta y a Algoritm solo a la derecha; el mockup 4, las insignias arriba a la derecha de la imagen | Quedan superados por D9–D11: el diseño vigente es el de `despues-menu-principal.png` y `despues-seleccion-niveles-…png`. El dibujo de cada mockup no es texto legible en el archivo |
| `Assets/Game/Art/Inventario.md` | 188 | «`env_n2_bosque_claro.png` … Lo usan las narrativas del N2, `Level2_Forest` y `Level2_Workshop`» | Añadir la portada del inicio (`MainMenu`) y la tarjeta del Nivel 2 del menú de niveles |
| `Assets/Game/Art/Inventario.md` | 190 | «`env_n2_laberinto.png` … lo usan `Level2_Maze` y la tarjeta del nivel en el menú de niveles» | Solo `Level2_Maze` |
| `claudeDocs/INCONSISTENCIAS.md` | 1332–1334 (nota de INC-82, 30/09/2026) | «la tarjeta del Nivel 2 muestra `entorno_n2_laberinto`, de 16:9, en lugar del bosque de 3840 px, que se veía repetido y partido por la costura» | Nota fechada 08/10: vuelve el bosque, encuadrado con foco 0,25 (`FramedIllustration`) para no cruzar la costura; y, si se quiere, un INC propio para D9–D11 |
| `claudeDocs/INCONSISTENCIAS.md` | 2760–2761 | «la tarjeta del Nivel 2 del menú de niveles, sin costura (INC-82)» | Sigue siendo cierto (sin costura), pero con el bosque encuadrado y no con el laberinto |
| `claudeDocs/entregables/OE3/src/07-interfaz.md` | 25 | «La pantalla de inicio muestra el título «Algoritmia», el lema …, a Algoritm en su forma de fuego y cuatro botones…» | La portada con los cinco personajes en el bosque; el título encima de la tarjeta del lema |
| `claudeDocs/entregables/OE3/src/07-interfaz.md` | 27 (Figura 7.1) | Pie: «el título … y el lema … sobre la tablilla marfil; …, y a la derecha Algoritm en su forma de fuego» e imagen `fig/fig-07-1-pantalla-de-inicio.jpg` | Pie nuevo y figura recapturada del ejecutable (§8, punto 7) |
| `claudeDocs/entregables/OE3/src/07-interfaz.md` | 29 y 31 (Figura 7.2) | 29: «cada uno con la ilustración de su escenario»; 31: «…la cueva, el laberinto y el río» e imagen `fig/fig-07-2-menu-de-niveles.jpg` | «la cueva, el bosque y el río»; icono abajo a la izquierda de la imagen y texto entre imagen y botón; figura recapturada |
| `claudeDocs/entregables/OE3/src/anexos/E-arte-y-sonido.md` | 222–223 | 222: «`Wheel/env_n2_bosque_claro.png` … narrativas del Nivel 2, bosque y taller»; 223: «`Wheel/env_n2_laberinto.png` … laberinto y tarjeta del Nivel 2» | 222: añadir portada del inicio y tarjeta del Nivel 2; 223: solo el laberinto |
| `claudeDocs/entregables/OE3/src/anexos/G-slice-4.md` | 80 | «…la tarjeta del Nivel 2 usa una ilustración de proporción 16:9 sin costura (`Scenes_RNF01_…`, `LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura`)» | Ya no es 16:9: es el bosque duplicado, encuadrado sin costura (la prueba sigue existiendo, con otra condición) |
| `claudeDocs/entregables/OE3/src/A-matriz-rf.md` | RF-01 y RF-03 | La matriz se genera: correr `rf_matrix.py --worktree` | Recogerá las cuatro `MainMenu_RF01_…` nuevas, `LevelSelect_RNF19_ElIconoDeEstado…`, y las de `FramedIllustration_RF01_…` y `CharacterRig_RF01_…` |
| `CLAUDE.md` | 435 (viñeta «Los encuadres son fracciones del lienzo») | Habla de `IllustrationFraming` y de quien lo usa (`NarrativeSceneController`, escenas jugables) | Añadir `FramedIllustration` (`Game.UI`), que lo usa en el inicio y el menú de niveles, **desde la ventana** y no desde la imagen |
| `CLAUDE.md` | 450 (viñeta «Los personajes son rigs por recorte en uGUI») | Lista los campos y componentes del rig | Añadir `CharacterRig.idlePhase` (fase de arranque del `Idle`, solo el `Idle`; la usa la portada del inicio) |
| `claudeDocs/tasks/Personajes/Personajes-Resultados.md` | (sin línea fija: la lista de usos de los rigs) | — | Añadir la portada del inicio como escena que lleva a los cinco personajes en `Idle` |

## 10. Mensaje de commit propuesto

```
Main menu cover with the five characters in the forest; title above a shorter card

Decisión de Santiago, 08/10/2026 (D9, D10, D11), no Kanban card.

Cover: the right panel of MainMenu is the left half of env_n2_bosque_claro
(focus 0.25, so it never crosses the x = 0.5 mirror seam) with Papá, Mamá,
Niño, Niña and Algoritm (fire) in Idle. The window is always square
(AspectRatioFitter), so the composition holds at 16:10 and 4:3. New
FramedIllustration (Game.UI) applies IllustrationFraming from the window,
not from the image: Unity does not tell a child that its parent was resized,
and the first frame has an empty Canvas, so Apply skips an empty window.
New CharacterRig.idlePhase starts the Idle at a phase of its cycle (Idle
only) so the five do not breathe in unison.

Title: the game name leaves the card, white 112 px with a 4 px #3A1E18
Outline; the card keeps only the tagline (280 -> 120 px); title, card and
buttons hang from a Bloque anchored at (0, 0.5), 222 px of margin above and
below.

Level select: each card gets a masked Ventana with FramedIllustration; the
status icon sits bottom-left of the image and its text is centred between
image and button (the ivory pill behind the text is gone); the Level 2 image
is the forest again, framed.

Tests: four MainMenu_RF01 tests, LevelSelect_RNF19 icon/text layout,
LevelSelect_RF03 seam test adjusted to the framed window, EditMode tests for
FramedIllustration and idlePhase. PlayMode tests not run by the implementer.

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
```
