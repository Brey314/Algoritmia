# Etapa 4a — Algoritm por partes, saludo `Wave` y créditos (08/10/2026)

Parte crudo del implementador. Decisiones D12 (créditos y `Wave`) y D13 (Algoritm por partes) de `brief.md`. **No hice commit**, no toqué
documentación, ni lo de Papá, el laberinto y la narrativa (md5 de `Papa.prefab`, de los 21 `.anim` de Papá, de `Narrative.unity`, `Level1_Cave.unity`,
`Level2_Maze.unity`, `arte_final.json`, `preparar_arte_final.py`, `codo.py`, `hombro.py` y `prefabs.py` iguales antes y después, ver §7). Partí con el Editor abierto en
`Narrative.unity`, limpia, y lo dejé igual. **No corrí PlayMode ni `VisualVerification`**: las PlayMode que escribí las corre el revisor (§8).

## 1. Qué quedó

| Pedido | Resultado |
|---|---|
| Algoritm por partes (D13) | Las **tres formas** (fuego, rueda, gota) comparten geometría —el mismo dibujo recoloreado—, así que las hice las tres: 9 PNG por forma (27) en `Assets/Game/Art/Characters/Algoritm/Frontal/`, `sprites` + `orden` + `clips` corridos solo sobre Algoritm, `Cuerpo` apagado. |
| Ojos y boca | **No los encendí.** La cara está pintada en el cuerpo en las tres formas y el recorte no la separa: `Ojos` y `Boca` siguen apagados y sin sprite (el log de `sprites` los lista «SIN PNG»; no se crea `_cara.asset`). Duplicarlos habría pintado ojos sobre ojos. |
| `ActorAction.Wave = 22` | Al final del enum; `ActionEmotion.For(Wave)` = `Happy`. |
| Clip y estado | `char_algoritm_anim_saludar.anim` (4,8 s, bucle, 30 fps, 11 curvas) y estado `Wave` en `char_algoritm.controller`, con un script de Editor efímero (borrado). |
| Créditos (D12) | `Fondo`, `Marco`, `Sombra` y la `Image` vacía `AlgoritmSaluda` fuera; el rig `Algoritm_Fuego(Creditos)` cuelga directo de `AlgoritmPanel` y llena su ancho (520). `CreditsController.guide` + `Play(Wave)` en `Start`. |

## 2. Las piezas: cómo se cortan y por qué no por rects

`pose_preview.py` ya hacía una maqueta de Algoritm, pero cortaba **por los rects** del JSON y no servía para el juego: con brazos en diagonal que salen del
borde curvo del vientre, un rect se llevaba un trozo del contorno del vientre en cada hombro y cada cadera (al girar el brazo el trozo se iba con él y el
vientre quedaba mordido) y dejaba escalones en el codo y la rodilla. Lo sustituí por `maqueta.piezas_guia` (al final de `maqueta.py`, con el comentario
largo), que `pose_preview` usa para dibujar y probar y que exporta `--exporta-maqueta`:

1. **Cuerpo** = lo que sobrevive a una apertura con disco de 16 px (los palos miden 20 px a 768) en la componente que contiene un punto del vientre, más 2 px de
   borde suave. Se calcula solo en la franja de los hombros a las caderas.
2. **Lo libre** (opaco y no cuerpo) dentro de la zona de cada extremidad es de ella; el torso se queda con todo lo demás, incluido el contorno del vientre entero.
3. Cada extremidad se parte en el codo o la rodilla con un plano perpendicular al eje; **los píxeles se reparten, no se repiten** (en reposo no queda un hilo
   transparente en la costura).
4. La pieza de arriba lleva una **rótula**: un disco del radio del palo (13,5 px del lienzo) en la articulación; en el hombro y la cadera solo con los píxeles
   oscuros (el contorno del vientre y el palo), más el primer trecho del palo (la protuberancia del cuerpo se mete unos px en él). Tapa la cuna que se abre al
   doblar y, en reposo, queda debajo de lo que ya está.
5. El alfa menor que 8 (la pelusa del borde del sprite original) no es de ninguna pieza.

Se corta **a la resolución del sprite** (768), sin reescalar: cada PNG ocupa su rect del lienzo de 1024 y los rects son múltiplos de 4 para caer en píxeles enteros
(`piezas_guia` lanza `ValueError` si un rect no lo es o si una pieza se sale del suyo). Las piezas sumadas reproducen el sprite: **0,002 %** de píxeles distintos en
las tres formas (`--autoprueba`, caso nuevo; tope 0,6 %).

**Comando que las generó** (reproducible: re-exportar da los 27 PNG idénticos byte a byte):

```
python claudeDocs/tasks/Personajes/herramientas/pose_preview.py --exporta-maqueta [CARPETA]    # por defecto Assets/Game/Art/Characters/Algoritm/Frontal; con --solo, solo esas formas
```

| Pieza (`char_algoritm_<forma>_`) | Rect del lienzo de 1024 | PNG (px) |
|---|---|---|
| `parte_torso` | `[248, 20, 776, 812]` | 396 × 594 |
| `parte_brazo_izq` / `_der` (húmero con rótulas) | `[208, 612, 304, 708]` / `[720, 612, 816, 708]` | 72 × 72 |
| `parte_antebrazo_izq` / `_der` (palo y mano) | `[8, 684, 236, 908]` / `[788, 684, 1016, 908]` | 171 × 168 |
| `parte_pierna_izq` / `_der` (muslo con rótulas) | `[440, 784, 472, 916]` / `[552, 784, 584, 916]` | 24 × 99 |
| `parte_antepierna_izq` / `_der` (pierna y pie) | `[388, 900, 472, 1004]` / `[552, 900, 636, 1004]` | 63 × 78 |

27 PNG, 908,6 KB en disco, **3,63 MiB como textura sin comprimir** (1,21 por forma): es lo que suma al paquete (RNF-06, hoy con 21 MB de margen); no lo medí
en un ejecutable. `ArtImport_*` pasa (3/3): entran sin comprimir y a 4096 de tope por `ArtImportRules`, y `sprites` los pasó a `Single`.

### La tabla de articulaciones (`articulaciones.py` → `rig_articulaciones.json`)

Medí sobre el alfa los ejes de los palos (miden 27 px de ancho, no 17) y las medidas «a ojo» estaban fuera del eje: **el hombro quedaba 17 px por encima del
palo**, y un brazo que gira desde ahí se despega del cuerpo. Constantes nuevas en `ALG` (con su comentario); el JSON se regeneró con `articulaciones.py` y solo cambian
las tres entradas de Algoritm (el resto del archivo era byte a byte lo que ese script produce, comprobado antes de tocarlo):

| Punto | Antes | Ahora |
|---|---|---|
| hombro izq / der | (292, 612) / (735, 612) | **(287, 629) / (736, 630)** |
| codo izq / der | (228, 688) / (798, 684) | **(224, 693) / (800, 693)** |
| cadera izq / der | (455, 800) / (570, 800) | (455, 800) / (568, 800) |
| rodilla izq / der | (445, 901) / (582, 901) | **(455, 901) / (568, 901)** |

Los rects antiguos eran del brazo entero (`[10, 598, 300, 912]`…): ahora son lo que ocupa cada pieza. Efecto lateral: `coreografia._brazos_de` estimaba la mano
de Algoritm con la esquina lejana del rect del húmero; ahora el guía **siempre** se trata como brazo partido (`partido = guia or …`) y la mano sale del rect del
antebrazo. **Los clips de Algoritm no cambian por esto**: `coreografia.py` regeneró `clips_personajes.json` byte a byte igual antes de añadir `Wave`.

## 3. El generador en el Editor

Copié `BuildRigsFinal.cs.txt` a `Assets/Editor/ClaudeBuildRigsFinal.cs` **con un cambio solo en la copia**: `ExecuteGuide(modo)` filtra la tabla y los clips a Algoritm
(el `.txt` versionado solo cambió un comentario: «los 9 de Algoritm» → «los 10»). Motivo: `Execute("sprites")` recorre los siete personajes y vuelve a guardar los prefabs de la familia (con Papá a medias de otra etapa); con el filtro la
familia ni se abre. Orden: `estado` → `sprites` → `orden` → `clips` → `estado`.

| Modo | Resultado |
|---|---|
| `sprites` | Por forma: 10 aplicados (`PiernaIzq/Der`, `BrazoIzq/Der`, `Torso`, `AntebrazoIzq/Der`, `AntepiernaIzq/Der` y «Cuerpo apagado»). `SIN PNG`: `ojos_neutra`, `boca_0` (a propósito). Sin avisos. |
| `orden` | «Tronco ya estaba en orden (Torso, Ojos, Boca, BrazoIzq, BrazoDer)» en las tres: no guardó nada. |
| `clips` | `algoritm: 10/10 clips reescritos`, sin avisos de rutas, claves ni bucles. |
| `estado` | Los siete prefabs cumplen la tabla; Algoritm «segmentado sí», sin `_cara.asset`. |

Después de `clips`, antes de las escenas, el `Wave` ya existía: lo creó `ClaudeWaveClip` (clip vacío con `loopTime`, estado con `writeDefaultValues` como los demás) y lo llenó `clips`.

## 4. `Wave`, el saludo

`coreografia.guia_wave` (junto a los otros nueve clips de Algoritm; los 9 anteriores **no cambian**: sus `.anim` son idénticos byte a byte). Bucle de 4,8 s con una pausa de
reposo de 0,9 s dentro:

| t (s) | Qué hace |
|---|---|
| 0 – 0,55 | El hombro derecho sube (94°) y el codo se dobla un poco antes (0 – 0,38), para que el brazo no se estire horizontal. |
| 0,55 – 3,35 | La mano se mece con el antebrazo casi vertical: codo 27° ± 16°, 0,7 s por vaivén (4 vaivenes), el hombro ± 3° y el cuerpo inclinado −2,5° hacia la mano. |
| 3,35 – 3,95 | Baja. |
| 3,95 – 4,8 | Reposo (brazos como en `Idle`); el cuerpo flota todo el clip (dos vueltas de 2,4 s) y el brazo izquierdo y las piernas siguen la flotación como en `Idle`. |

- **Brazo derecho de la pantalla**, como pidió el brief (el lado de fuera: el izquierdo miraría al texto).
- Velocidad máxima 256°/s (tope de `pose_preview`: 1300).
- **El parámetro que manda es la cara**: `pose_preview` exige libre el 99,5 % de la caja de ojos y boca ampliada un 30 %. Un primer 100° con 26 ± 16 pasaba pero rozaba (99,7 %); a 104° llegó a perder el 1,5 %. Barrí hombro,
  codo base y vaivén: con **94° y 27 ± 16 queda 100 %** en las tres formas; el hombro aguanta hasta 100° (99,53 %, en el límite) y el codo hasta 30 ± 19 (99,77 %); 33 ± 19 ya no pasa (99,25 %).
- En el motor lo vi en las capturas (§6): sube, se mece, baja y reposa, sin tocar la cara.

**Clip y estado**: lo del §3. **C#**: `ActorAction.Wave = 22` (último, los assets guardan el número) y `ActionEmotion.For` → `Happy` junto a `Celebrate`, `Hug` y `Encourage`.

## 5. Créditos

- `Credits.unity` (script efímero, borrado): rig bajo `AlgoritmPanel` con anclas de estirar (el lienzo del rig escala a `min(520, 952)/1024` y queda centrado en vertical) y
  `CreditsController.guide` cableado. `git diff`: −312 / +20 líneas (los cuatro objetos de la tarjeta; el resto de la escena igual).
- **Decisión ancho / más abajo, mirando las capturas a 1920×1080**: elegí **ajustado al ancho de 520 y centrado en vertical** (`despues-creditos.png`). Probé también un cuadrado de 520 pegado al
  borde de abajo (`alternativa-creditos-abajo.png`): los pies quedan a unos 80 px del borde de la pantalla y arriba sobran unos 440 px; centrado queda a la altura del centro de la tarjeta de texto y la mano
  en alto tiene aire.
- `CreditsController`: `[SerializeField] private CharacterRig guide;` y `internal CharacterRig Guide` bajo `UNITY_INCLUDE_TESTS`. **Escribí `if (guide != null) guide.Play(ActorAction.Wave)` y no `guide?.Play(...)`** (§10, duda 1).
- **PlayMode nueva**: `Credits_RF08_AlgoritmSaludaOcupandoElLugarDeLaTarjetaSinElla` (`CreditsTests.cs`): `Guide` no nulo y `Current == Wave`; el rig es hijo directo de `AlgoritmPanel`; `Fondo`,
  `Marco` y `Sombra` no existen; ninguna `Image` del panel fuera del rig; el ancho del lienzo del rig es el del panel (±1 px).
  **No la corrí.** La ensayé a mano en Play con las mismas comprobaciones, sobre `Credits` cargada directo: `Current=Wave`, panel `AlgoritmPanel`, `Fondo/Marco/Sombra = null`, 0 imágenes fuera del rig, ancho del lienzo 520 = ancho del panel 520.

## 6. Capturas — `claudeDocs/tasks/Ajustes-Diseno-2026-10-08/capturas/04a-algoritm-creditos/` (Play manual, Game View 1920×1080, `ScreenCapture.CaptureScreenshotAsTexture`, fuera de `Assets`)

| Archivo | Qué es |
|---|---|
| `hoja-saludo-12-fotogramas.png` (1920 × 1485) | 12 instantes del saludo en los créditos (0,0 – 4,4 s, el Animator parado en cada uno), recorte del panel de Algoritm |
| `antes-creditos.png` · `despues-creditos.png` | La pantalla de créditos antes (tarjeta con Algoritm de 300 px) y después |
| `alternativa-creditos-abajo.png` | La otra disposición probada (pegado al borde de abajo) |
| `antes-narrativa-n1-{talk,point,celebrate}.png` · `despues-…` | `N1_NacimientoDelFuego`, línea 10 (habla Algoritm, `Talk`) y forzando `Point` y `Celebrate`, a 1920×1080 |
| `narrativa-n1-antes-despues-recorte.png` | Lo mismo, recortado y ampliado ×4, antes | después |
| `laberinto-retrato-del-guia.png` | `Level2_Maze` en Play: el retrato del guía (círculo arriba a la izquierda) |
| `limite-conocido-vanish-alfa-035.png` | El límite del §10 (duda 2): el guía al 35 % de alfa |

«**Antes**» en las narrativas = el mismo rig con `Cuerpo` encendido y las nueve piezas apagadas, en el mismo instante con el Animator parado (es exactamente el prefab de `HEAD`, que tenía las piezas sin sprite). El «antes»
de los créditos sí es la escena de `HEAD`, pero con el prefab ya partido: en `Idle` se ve igual.

Lo que se ve: en `Point` el brazo derecho se estira y señala (antes no se movía); en `Celebrate` los brazos suben y bajan por separado; en `Talk` los brazos se mecen unos grados. **Ninguna pieza desencajada ni hueco en los
hombros** a escala de pantalla (ampliando la captura ×4 y los créditos ×6). El laberinto: el retrato es el **sprite** `char_algoritm_n2_rueda_reposo` dentro del botón de pista, no el rig (igual en el bosque y el taller), así que no
cambia nada; lo capturé para dejarlo dicho.

## 7. Qué cambió, comprobado

Contra una huella md5 de `Assets/Game/Art/Characters`, `Assets/Game/Prefabs/Characters`, `Assets/Game/Scenes` y `claudeDocs/tasks/Personajes/herramientas` tomada **antes de empezar** (y `git diff`):

- **Prefabs**: solo `Algoritm_Fuego`, `Algoritm_Rueda` y `Algoritm_Gota` (102 líneas cada uno: sprite, `m_Enabled`, tamaños, pivotes y anclas de las nueve piezas; `Cuerpo` 1 → 0). Cada prefab sigue con **69 objetos**; ninguna
  línea `--- !u!` quitada, ni `m_Script`, `m_Controller`, `m_Name`, `m_Father` o `m_Children` tocados: **los fileID no cambian**. Ningún prefab de la familia.
- **Clips y controlador**: nuevo `char_algoritm_anim_saludar.anim` (+ `.meta`); `char_algoritm.controller` +29 líneas (el estado `Wave`). Los **9 `.anim` anteriores de Algoritm: byte a byte iguales** (el modo `clips` es idempotente); **ningún `.anim` de la familia**
  (los 21 de Papá que figuran como modificados ya lo estaban al empezar, de la etapa 1: mismo md5).
- **Escena**: `Credits.unity`. Las otras tres con cambios ajenos (`Narrative`, `Level1_Cave`, `Level2_Maze`) no las guardé.
- **Arte nuevo**: 27 PNG y 27 `.meta` en `Algoritm/Frontal/`. **Tablas**: `rig_articulaciones.json` (solo las tres entradas de Algoritm), `clips_personajes.json` (+14 líneas: el clip nuevo).
- **Herramientas** (encima de lo que traían de la etapa de Papá, sin deshacerlo: `arte_final.json`, `preparar_arte_final.py`, `codo.py`, `hombro.py`, `prefabs.py` intactos; `BuildRigsFinal.cs.txt` +1 −1, un comentario): `maqueta.py` (+168: `piezas_guia`), `pose_preview.py` (+74 −44: usa `piezas_guia`,
  `--exporta-maqueta`, caso nuevo de la autoprueba, comentarios), `coreografia.py` (+31 −2: `guia_wave` y `partido` del guía), `articulaciones.py` (+29 −35: `ALG` y `personaje_guia`).
- Andamiaje: `Assets/Editor/` (`ClaudeBuildRigsFinal`, `ClaudeWaveClip`, `ClaudeCreditsEdit`, `ClaudeCapture`) **borrado con `Assets/Editor.meta`**; `git status` sin nada de `Assets/Editor`. Nada escrito dentro de `Assets/` por las capturas. El `__pycache__/` de `herramientas/`
  ya estaba sin seguimiento al empezar (ruido de Python, no se commitea).

## 8. Pruebas

Todo por `editor.ps1`, solo EditMode, tras `recompile`:

| Corrida | Resultado |
|---|---|
| `tests-edit CharacterRig` | **145 / 145** (incluye `CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion` con `Wave` en los tres guías, `_DA131_CadaClipFijaTodasLasArticulaciones` con el clip nuevo —10 clips, rotación en los 10 huesos—, `_DA131_TodaCurvaApuntaAUnaParteQueExiste`, `_DA131_UnaCapaSinSpriteNoSeDibuja`, `_CP02_NingunClipEsDeDerrotaCaidaNiSalto` —escanea los 94 `.anim` bajo `Characters`, entre ellos `char_algoritm_anim_saludar`— y la nueva) |
| `tests-edit FacialEmotion` | **30 / 30** (con `Wave → Happy` y `LaTablaCubreTodasLasAcciones`) |
| `tests-edit Game.Scaffolding.Tests` (assembly) | **296 / 296** |
| `tests-edit Game.UI.Tests` (assembly) | **29 / 29** |
| `tests-edit ArtImport` | 3 / 3 |
| `pose_preview.py` completo (`--hoy` y `--maqueta`, hojas y GIF) | «la prueba pasa en todos los clips»: **Algoritm 10 / 10 por forma** (los 9 de antes y `Wave`), **21 / 21 en Papá, Mamá, Niña y Niño**; 1 min 47 s |
| `pose_preview.py --autoprueba` | «la prueba detecta lo que debe» (con el caso nuevo: las piezas de Algoritm suman el sprite, 0,002 % ×3) |
| `coreografia.py --valida` / `--autoprueba` | bien / bien (exit 0; antes de existir el `.anim` daba «el documento bueno: FALSO POSITIVO», y es lo esperado) |
| `preparar_arte_final.py --autoprueba` | pasa (error máximo 1,0 px) |

Pruebas nuevas o cambiadas:

- `CharacterRigTests`: `AccionesDeLaFamilia` excluye `Wave` (como `Spin`), `AccionesDelGuia` lo incluye, y **nueva** `CharacterRig_DA131_AlgoritmSeDibujaPorPartesYSuSpriteEnteroSeApaga` (×3 formas: las nueve `Image` encendidas con el sprite **de su forma**
  —`char_algoritm_<forma>_parte_*`—, `Cuerpo` apagado, `Ojos` y `Boca` apagados).
- `FacialEmotionTests`: `[TestCase(ActorAction.Wave, FacialEmotion.Happy)]`.
- `CreditsTests` (PlayMode): la del §5. No corrí PlayMode: ni ellas ni las existentes.

Consola de Unity sin errores ni avisos nuevos de lo mío (el aviso `PropShadow.cs(453) CS0108` y los de `ArmLayering`/`LimbFollower` que las pruebas provocan a propósito son de antes).

**PlayMode que debe correr el revisor** (`tests-play <filtro>`):

```
tests-play Credits_                    # las 4 de siempre y la nueva Credits_RF08_AlgoritmSaludaOcupandoElLugarDeLaTarjetaSinElla
tests-play NarrativeScene_             # las narrativas con Algoritm (N1_NacimientoDelFuego, N2_PuenteI, N3_*): ahora se dibuja por partes
tests-play MainMenu_                   # el menú lleva una instancia de Algoritm_Fuego (la reforma la etapa 4b)
```

y, si quiere verlo, `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes` (`VisualVerification`, no la corrí).

## 9. Cuando llegue el arte final de Algoritm

1. Sustituir los PNG de `Assets/Game/Art/Characters/Algoritm/Frontal/` por los entregados **con el mismo nombre** (`char_algoritm_<fuego|rueda|gota>_parte_{torso,brazo_*,antebrazo_*,pierna_*,antepierna_*}.png`, en el Explorador sobre el archivo: así
   se conservan `.meta` y GUID). Si el arte separa la cara, añadir también `ojos_*` y `boca_*` (nombres de C.4): `sprites` enciende `Ojos` y `Boca` y crea `char_algoritm_<forma>_cara.asset`.
2. Poner en el JSON el rect y el punto de cada articulación **medidos sobre el alfa** de los PNG nuevos, como en la familia: hoy son las constantes de `ALG` en `articulaciones.py` (correr `articulaciones.py` reescribe el JSON; ya no son «a ojo»
   pero sí del arte provisional). Los rects de Algoritm dejan de tener que ser múltiplos de 4 (eso era del corte por píxel de la maqueta).
3. En el Editor: `sprites`, `orden` y `clips`, por ese orden. **Ojo**: el `.txt` recorre los siete personajes; si la familia no está al día con su tabla, `sprites` le vuelve a guardar los prefabs y le aplica lo pendiente. Para tocar solo a Algoritm hay que filtrar como
   hice en la copia efímera (`_soloGuia` en `LoadTable` y `LoadChoreography`); no lo subí al `.txt`.
4. **`pose_preview.py` seguirá dibujando a Algoritm con la maqueta de su sprite entero, no con los PNG de `Frontal/`** (como la familia con `es_final`). Hay que enseñarle a leer las piezas cuando existan los PNG finales; hoy no hay nada que leer distinto de la maqueta.

## 10. Dudas y límites conocidos

1. **`guide?.Play(...)` → `if (guide != null)`.** El brief decía `guide?.Play(ActorAction.Wave)`; `?.` se salta la comprobación de nulo de Unity y un campo sin asignar en el Editor no es `null`, así que lanzaría `MissingReferenceException` en una escena sin cablear. Quedó el `if`, con un comentario.
2. **Durante los fundidos (`Appear`, `Vanish`, `Hidden`) las rótulas se ven.** El alfa de `Lienzo` (`CanvasGroup`) multiplica cada `Image` por separado, así que donde dos piezas se solapan (rótulas de codos, rodillas, hombros y caderas) se ve más oscuro:
   con 35 % de alfa quedan anillos oscuros en las articulaciones (`limite-conocido-vanish-alfa-035.png`). Es inherente a las piezas solapadas (la maqueta de la familia y su arte final también las solapan) y dura ~0,3 s por parpadeo del `Vanish`; no lo corregí.
   Alternativas: rótula concéntrica con el hueco en la pieza de abajo (sin solape, pero deja un hilo claro al girar) o fundir con otro mecanismo.
3. **El hombro girado deja ver un talón más claro** (la rótula copia el contorno del vientre, que es más claro que el palo): un bultito de ~12 px en el lienzo al levantar el brazo, visible ampliando los créditos ×6; a escala normal pasa por la articulación. Un disco liso del color del palo lo haría visible en reposo, así que lo dejé.
4. **Rueda y gota** son el mismo dibujo recoloreado, así que el corte vale para las tres; lo vi montando los PNG de cada forma y comparando con su reposo, no en el motor. En el motor solo vi la de fuego (Narrativa N1 y créditos).
5. **Peso**: +3,63 MiB de textura sin comprimir, sin medir en un ejecutable.
6. `Wave` solo lo tiene el guía; si un guion lo pidiera a un miembro de la familia caería a `Idle` (como cualquier estado que falte).

## 11. Documentos que habrá que corregir (ruta y línea; las líneas son del árbol de trabajo del 08/10/2026)

| Archivo | Línea | Qué dice hoy | Corrección |
|---|---|---|---|
| `claudeDocs/Direccion_de_Arte.md` | 474–478 (§7.6) | «**Arte actual (provisional): una sola imagen por forma, sin recorte en partes.** Los prefabs … animan el cuerpo entero … así que sustituir el archivo basta» | Algoritm **se dibuja por partes** desde el 08/10/2026 (D13): nueve piezas recortadas de su `_reposo` (`Frontal/char_algoritm_<forma>_parte_*.png`, con `pose_preview.py --exporta-maqueta`), `Cuerpo` apagado; el `_reposo` sigue siendo el retrato y el botón de ayuda |
| `claudeDocs/Direccion_de_Arte.md` | 985–987 (§13.1) | «Algoritm es una sola `Image` por forma, sin recorte, con nueve clips que comparten las tres formas, para que sustituir su arte sea cambiar un archivo» | nueve piezas y **diez clips** (con `Wave`); sustituir su arte es sustituir los PNG con el mismo nombre y correr `sprites` |
| `claudeDocs/Direccion_de_Arte.md` | 1029–1043 (§13.3, tabla) | No hay saludo; la fila de Algoritm es «Flotación y giro del guía» | Fila nueva: Saludo (`Wave`) · Algoritm (créditos) · bucle de 4,8 s con pausa de 0,9 s · Media |
| `claudeDocs/Direccion_de_Arte.md` | 338 (§7.3) | «un clip del rig por acción (§13.3, `ActorAction`)» y la tabla de acciones de debajo | Añadir `Wave` (emoción `Happy`) si la tabla enumera las acciones |
| `claudeDocs/tasks/Personajes/Personajes-Resultados.md` | 42 | «21 clips en la familia (9 en Algoritm)» | 10 en Algoritm |
| ídem | 162 | «El enum de 22 acciones (0–21)» | 23 acciones (0–22): `Wave = 22` |
| ídem | 464–469 (C.2) | «`Lienzo/Cuerpo` es una sola `Image`, y **sigue siendo lo visible** … se apaga cuando llegan las partes» | Ya llegaron (provisionales, 08/10/2026): `Cuerpo` apagado; `Ojos` y `Boca` siguen apagados (la cara va en el torso) |
| ídem | 625–630 (C.6) | «Algoritm, nueve clips … Con el sprite entero de hoy, **lo visible no cambia**» | diez clips; los brazos y las piernas se mueven por separado |
| ídem | 548–587 (C.5), 681–705 (C.8) | El flujo y la lista de pruebas | `--exporta-maqueta`, `sprites` de Algoritm y las pruebas nuevas (§8) |
| `claudeDocs/tasks/Personajes/Plan-Personajes-Finales.md` | 329 · 346 | «9 clips .anim + char_algoritm.controller» · «ActorAction.cs (enum intacto)» | 10 clips · el enum gana `Wave = 22` |
| `claudeDocs/INCONSISTENCIAS.md` | 29–31 y 2488–2530 (INC-131) | «levanta … la regla de Algoritm de una sola imagen **para el arte final**» | La regla también cae para el arte **provisional** (D13); y la acción `Wave` y la tarjeta de créditos fuera (D12) merecen INC propio, o una nota fechada en INC-131 |
| `Assets/Game/Art/Inventario.md` | 97–101 | «**Arte actual:** una sola `Image` por forma y ningún recorte … clips `char_algoritm_anim_{flotar,hablar,senalar,girar,celebrar,animo,oculto,aparicion,apagado}`» | partido en nueve piezas; añadir `saludar` a la lista de clips |
| ídem | 106–118 | Las nueve piezas con estado `○` y «se sigue viendo el `_reposo`» | `◐` provisional (existen y se usan); ojos y boca siguen `○` |
| `CLAUDE.md` | 46 | «…del arte final … falta Algoritm…» | sigue cierto para el arte final; añadir que Algoritm ya se dibuja por partes con arte provisional |
| `CLAUDE.md` | 456 | «el rig tiene además codos, rodillas, cuello (no Algoritm), cabeza, ojos y boca como nodos y capas apagadas hasta el arte final» | Algoritm: las nueve piezas ya están encendidas (arte provisional recortado) y `Cuerpo` apagado; ojos y boca siguen apagados; y una frase para `Wave` |
| `claudeDocs/Interfaces.md` | 36 (fila 14, Créditos) · 283 | La fila de `Credits.unity` y «un clip del rig por acción (`ActorAction`, §7.3)» | Mencionar que Algoritm saluda (`Wave`) sin tarjeta, y la acción nueva |
| `claudeDocs/entregables/OE3/src/anexos/F-personajes.md` | 105 · 200 · 324 | «En Algoritm, `Cuerpo` sigue siendo una sola `Image`…» · «los 9 de Algoritm quedan idénticos» · «la coreografía de los 93 clips» | partido; 94 clips y 10 de Algoritm; y el Anexo A (`A-matriz-rf.md`, RF-08) debe nombrar `Credits_RF08_AlgoritmSaluda…` y las pruebas nuevas de `CharacterRig` |
| `claudeDocs/tasks/Slice 1/Fase-1-Resultados.md` | 398–399 | «un hueco de arte con el rótulo «**Algoritm saluda · placeholder**» (`2893cdb`)» | Es registro histórico de esa fase: no se toca. **Lo que se dice de la imagen `AlgoritmSaluda`** está solo ahí; el `Image` vacío ya no existe (se fue con la tarjeta) y ningún otro documento lo nombra |

## 12. Mensaje de commit propuesto

```
Algoritm drawn in nine parts with a Wave greeting; credits card removed

Kanban "Personajes finales" (INC-131): the three Algoritm forms are cut from
their rest sprite into the nine pieces of rig_articulaciones.json (pose_preview
--exporta-maqueta writes 27 PNG into Algoritm/Frontal), so his arms and legs
move in every scene. The cut keeps the belly outline whole at the shoulders and
hips and gives each joint a ball cap; the joint points are measured on the stick
axes. BuildRigsFinal sprites/orden/clips run on Algoritm only; Cuerpo is off and
Ojos/Boca stay off (the face is painted on the torso). New ActorAction.Wave = 22
(Happy), clip char_algoritm_anim_saludar and its state, both made in the Editor.

Decisión de Santiago, 08/10/2026 (D12): the credits screen loses the card behind
Algoritm; the Fuego rig hangs straight from AlgoritmPanel at its full width and
CreditsController plays Wave on Start.

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
```
