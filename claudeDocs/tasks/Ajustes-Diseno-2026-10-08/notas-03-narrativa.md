# Etapa 3 — Narrativa: parte crudo (08/10/2026)

Implementador: etapa 3 de la ronda de ajustes. Decisiones D5–D8 del `brief.md` más el añadido D17 (más aire
entre el nombre y el texto, menos interlineado). **Sin commit.** Editor abierto todo el trabajo; lo dejo con
`Narrative.unity` activa y limpia, y fuera de Play.

## 1. Qué cambié (antes → después)

### Cuadro de diálogo (D5, D6, D17) — `Assets/Game/Scenes/Narrative.unity`, por `eval` + `SaveScene`

| Objeto | Antes (HEAD) | Después |
|---|---|---|
| `Canvas/CuadroDialogo` | 1400×180, a 16 px del borde | **1400×216**, a 16 px del borde: su cima queda a 232 px de 1080, el cuarto inferior acaba en 270 |
| `…/Fondo` | 1388×168 (stretch −12) | 1388×204 (sin tocarlo: sigue siendo −12) |
| `…/Retrato` | (28, −26), 128×128 | **(28, −38)**: centrado en vertical en el nuevo alto ((204 − 128) / 2) |
| `…/Hablante` | Baloo2-Bold **22**, **#6B5248**, 680×**26** en (184, **−12**) | Baloo2-Bold **30**, **#1F100C**, 680×**52** en (184, **−8**) |
| `…/Cuerpo` | Nunito-**Regular 26**, **#3A1E18**, interlineado **1,35**, 680×**134** en (184, **−38**) | Nunito-**SemiBold 30**, **#1F100C**, interlineado **1,1**, 680×**137** en (184, **−64**) |

`#1F100C` = `(0.1216, 0.0627, 0.0471)`. La fuente SemiBold es `Assets/Game/Art/Fonts/Nunito-SemiBold.ttf` (ya estaba en el proyecto;
GUID `24bd29f3…`, el de Regular era `5dde0d88…`). El diff de la escena son 24 líneas (12 + / 12 −), todas esas.

**Lo que eligió el D17** (viendo la captura `decision-cuadro-interlineado-y-separacion.png`, nueve combinaciones a 1:1):

- **Interlineado 1,1** (antes 1,35). A 30 px el renglón mide 41 y el paso entre renglones pasa de 47 px (a 26 px, 1,35) a 45 px;
  con 1,0 (41 px) los renglones no se tocan pero se ven apretados, 1,15 y 1,25 siguen aireados. Con 1,1 hay un hueco claro entre los
  descendentes de un renglón y las ascendentes del siguiente.
- **Separación nombre → texto: el texto baja de −46 a −64** (18 px). Medida sobre las capturas: blanco entre la tinta del nombre y la del
  primer renglón **10 px → 32 px**; entre líneas base **28 px → 54 px**. Con −64 el nombre se lee como encabezado y no como un renglón más
  (el paso entre renglones es 45; el del nombre al primero, 54).
- Todo cabe sin agrandar el cuadro más allá de los 216 px del D5: la caja del texto acaba a 201 px del borde de arriba del interior (204 de
  alto) y la tinta del tercer renglón, a ~192.
- Efecto colateral que conviene saber: **en las acotaciones** (55 de las 139 líneas, sin nombre) el texto queda 18 px más abajo que antes
  porque el hueco del nombre sigue reservado; así el texto no salta de sitio al pasar de una acotación a un diálogo (como ya era).

### Cámara que sigue a la balsa (D7)

- `NarrativeProp.CameraFollows` (bool, `[field: SerializeField]` + `Tooltip`), junto a `MotionAmbient`.
- `NarrativeSceneController`: `TargetFraming()` ahora es `Followed(StopFraming())`; `StopFraming()` es el cuerpo de siempre (parada en curso)
  y `Followed` le suma al foco **lo que el objeto marcado se ha movido desde donde empezó**, en fracciones de la ilustración:
  `(rect.anchorMin − prop.Position) + rect.anchoredPosition / illustration.sprite.rect.size` (la primera parte cubre a los personajes, que
  mueven el ancla; la segunda, a la balsa). El zoom no cambia, el acotado del foco lo sigue haciendo `IllustrationFraming.Offset`, y el
  desplazamiento **se queda al llegar** (no se quita: sería un salto atrás). `ApplyCut` también pasa por `Followed`. Con ningún objeto
  marcado devuelve la parada tal cual: las otras 17 escenas no cambian.
- `N3_Escena33_Cruce.asset` (YAML a mano, LF): balsa con `<CameraFollows>k__BackingField: 1`; parada de la línea 1: foco x **0,47 → 0,52**
  (zoom 1,58 y y 0,38 intactos).

### Halo de las fogatas (D8)

- **`FireGlow`** (`Game.Scaffolding`, uGUI, 12 pruebas EditMode). Va en el objeto de la llama; el halo es su **hermano** (un hijo se pinta encima
  de su padre y taparía la llama), llamado `Halo_<llama>`, `raycastTarget` apagado. Cada `LateUpdate` copia el centro y el lado de la llama
  (vale cualquier pivote, giro o espejo) y le da la escala `ScaleAt(Time.time)` = una sola onda entre **0,95 y 1,05, ciclo de 2 s, tiempo
  escalado** (con la pausa se queda quieto). La opacidad **no cambia nunca** (RNF-21): solo respira la escala.
  - Color `#F0A84E` al 20 % (`GlowColor`, `Color32(0xF0,0xA8,0x4E,0x33)`).
  - `Spread` 1,8 (diámetro = lado menor del cuadro de la llama × 1,8); `Offset` (0, −0,1) lados: el cuerpo de la llama dibujada está en la
    mitad de abajo de su cuadro (centro de la caja de los 13 dibujos del bucle a 0,576 desde arriba; centroide de masa a 0,661), así que
    la luz baja con ella; `BehindSiblings` (por defecto apagado).
  - Aviso corregido de paso: `FireGlow.Attach` sobre una llama apagada no enciende el halo (`isActiveAndEnabled`).
- **Enganche en las narrativas, por dato:** `NarrativeProp.Glows` (bool, tras `BurnExtent`) y en `PlaceProps`, junto a `BurnReveal`:
  `if (prop.Glows) FireGlow.Attach(go);`. Marcadas las **9 llamas** (`<Glows>k__BackingField: 1`, YAML a mano):
  `N1_NacimientoDelFuego` (objeto 6), `N2_PuenteI` (4), `N2_Escena25_Cierre` (3), `N3_PuenteII` (2), `N3_PuenteII_Horizonte` (2 y 5),
  `N3_EscenaFinal` (2, 5 y 8).
- **Enganche en la cueva:** el componente **en la escena** (`Level1_Cave.unity`, `Canvas/Panel/Suelo/Fuego`), sin código nuevo en
  `Game.Levels.Fire`; el halo nace con la llama (`OnEnable`) y la llama sigue siendo el último hermano.
  Aquí con **`BehindSiblings` = 1**: el halo va al **fondo de `Suelo`** y no justo antes de la llama (ver la elección de abajo).
- **Elección del sprite — la DA decía «círculo plano, sin degradado» y no se sostiene.** Probé primero `PropShadow.GetOrCreateCircleSprite()`
  (disco plano, borde duro) como pide el brief y lo miré en las dos escenas pedidas:
  - **Con capa de oscuridad (`N2_Escena25_Cierre`, de noche):** un disco naranja de ~625 px que tapa media cueva y corta la pared con un
    borde nítido; se lee como una mancha, no como luz.
  - **A plena luz (`N3_EscenaFinal`, tres fogatas):** casi invisible sobre la arena, salvo los **aros fantasma** donde se cruzan dos discos.
  - Lo cambié por **un disco radial generado por código** (sin archivo nuevo, como el disco de `BurnReveal`): 128×128, opacidad 1 hasta
    **0,3 del radio** y de ahí a 0 en el borde con curva de Hermite (`FireGlow.OpacityAt`). Conserva color, 20 % (el del centro) y respiración
    de la DA. Probé seis perfiles (`decision-halo-perfiles-del-degradado.png`) y dos aperturas (1,8 y 2,4); me quedé con núcleo 0,3 y 1,8.
    Resultado: de noche, un resplandor cálido que se funde con la cueva; a plena luz, un calor tenue junto a cada fuego, sin bordes.
  - **Dónde va en la cueva:** con el halo justo antes de la llama (como en las narrativas) cae **encima del montón y de las piedras**, y ese 20 %
    naranja los lava (el sílex gris y la piedra negra pasan a marrón; `decision-halo-cueva-v1-…png`). En las narrativas eso se lee como luz
    sobre lo que está junto al fuego; en la cueva, con las piedras como protagonistas de la pantalla, parece una película. Con
    `BehindSiblings` el halo baña el suelo y el montón y las piedras conservan su contraste (`decision-halo-cueva-definitivo-al-fondo.png`).
  - `Direccion_de_Arte.md` §8.1 debe cambiar de «círculo plano» a «círculo con degradado radial» (ver §6).

### Pruebas (nuevas o tocadas)

EditMode (las corrí):
- `FireGlowTests` (nuevo, 12): `ScaleAt` (0,95–1,05, ciclo 2 s, pendiente máxima 0,157/s = 0,0026 por cuadro a 60 fps), color `F0A84E33`,
  `OpacityAt` sin escalón, halo hermano justo antes / al fondo con la llama última, no se duplica, centrado con cualquier pivote, sigue a la
  llama, no se enciende sobre una llama apagada.
- `NarrativeSequenceTests`: `NarrativeSequence_RF05_CadaLlamaEmiteSuHalo` (toda llama `prop_n1_fuego_normal` lleva `Glows` y solo ellas) y
  `NarrativeSequence_RF44_LaCamaraDelCruceAcompanaALaBalsa` (la balsa lleva `CameraFollows` y es la única de las 18 escenas; con la cámara
  siguiéndola se ve **entera en cada parada y a cinco puntos del cruce**, con 2 % de pantalla de margen; y el **foco que se ve**, ya acotado,
  no retrocede entre paradas hasta el desembarco — sube la parada 1 por encima de 0,547 y falla).
- `CaveSceneDataTests` (nuevo, 1; abre `Level1_Cave` como escena de vista previa): la llama lleva `FireGlow` con `BehindSiblings`, apagada y
  última en disco. Se añadió `Game.Scaffolding` a las referencias de `Game.Levels.Fire.Tests.asmdef`.
- Los dos vigilantes que pedía el brief siguen verdes: `NarrativeSequence_RF05_NingunEncuadreDelNivel3SeRecorta` y
  `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`.

PlayMode (escritas; **no las corrí**, ver §5):
- `NarrativeSceneTests`: **sustituye** `…RNF01_LaLineaMasLargaCabeEnSuCuadroDeDialogo` por
  `NarrativeScene_RNF01_TodasLasLineasDeLasDieciochoNarrativasCabenEnSuCuadro` (18 secuencias, ≥ 139 líneas, `GetPreferredHeight` contra la caja
  del cuerpo, informe con todas las que desborden); nuevas `NarrativeScene_RF05_ElNombreDelHablanteCabeEnUnaLinea`,
  `NarrativeScene_RNF03_ElNombreElTextoYLosBotonesNoSePisanDentroDelCuadro`, `NarrativeScene_RF44_LaCamaraSigueALaBalsaMientrasCruza`,
  `NarrativeScene_RF44_LaBalsaNoSeSaleDelCuadroNiLaCamaraRetrocedeAlLeerElTextoMientrasCruza`,
  `NarrativeScene_RF05_CadaFogataTieneSuHaloJustoAntesDeLaLlama` (2 casos: 2.5 y escena final),
  `NarrativeScene_RNF21_ElHaloDeLaFogataPulsaLentoYSinDestellos`, `NarrativeScene_RNF21_ElHaloDeLaFogataSeQuedaQuietoConLaPausa`;
  `…RF05_LaEscena21MuestraLosObjetosRepartidosPorElSuelo` salta ahora los `Halo_*`; helpers `AltoDe` y `FocoVisto`.
- `FirePanelTests`: `FireLevel_RF20_LaLlamaCenitalEmiteSuHaloSinVelarElMonton` (halo `Halo_Fuego` bajo el montón, color, sin clics, llama última).

## 2. La línea más alta y su altura frente a la caja

Medido con `TextGenerator` en el Editor (mismas fórmulas que la prueba: `GetPreferredHeight(texto, GetGenerationSettings(caja)) / pixelsPerUnit`),
a 1920×1080 (escala de canvas 1), sobre las **139 líneas de las 18 secuencias**, caja de 680 px de ancho:

| | Fuente / interlineado | Más alta | Caja | Holgura | Reparto (1 / 2 / 3 renglones) |
|---|---|---|---|---|---|
| **Antes (HEAD)** | Nunito Regular 26, 1,35 | **129** (9 líneas, p. ej. `N1_AparicionGuia#1`, `#10`, `N1_Hallazgo#6`) | 134 | 5 | 65 / 65 / 9 |
| Primera pasada mía (superada por D17) | Nunito SemiBold 30, 1,35 | 151 | 156 | 5 | 54 / 68 / 17 |
| **Después (final)** | **Nunito SemiBold 30, 1,1** | **131** (17 líneas de tres renglones: `N1_AparicionGuia#0, #1, #7, #10`, `N1_Hallazgo#5, #6`, `N1_NacimientoDelFuego#0, #4, #13`, `N2_Escena21_Bosque#0, #6`, `N2_Escena23_Construccion#7`, `N2_PuenteI_Bosque#1`, `N3_Escena32_PrimerIntento#4`, `N3_Escena33_Cruce#5`, `N3_EscenaFinal#4`, `N3_PuenteII_Rio#0`) | **137** | **6** | 54 / 68 / 17 |

- **Ninguna línea necesita cuatro renglones**, y el alto máximo se mantiene entre 128 y 131 px a escalas de canvas 0,5 / 0,667 / 0,75 / 1 / 1,5 / 2
  (con 1,35 llegaba a 152). La línea de tres renglones más justa de ancho (`N1_AparicionGuia#1`) conserva sus tres renglones hasta 628 px de
  ancho (7,6 % menos que los 680): no hay línea a punto de saltar a cuatro.
- Alto por interlineado (máximo de las 139, a escala 1): 1,0 → 123 · 1,05 → 127 · **1,1 → 131** · 1,15 → 135 · 1,2 → 139 · 1,25 → 143 · 1,35 → 151.
- **Hablante:** el nombre más largo es `ALGORITM` (142 px de ancho en una caja de 680; las otras: NIÑA 68, PAPÁ 72, NIÑO 70, MAMÁ 86, NIÑOS 86).
  Una línea de Baloo 2 Bold a 30 px mide 48; la caja mide 52 (4 px por el redondeo de otras resoluciones).
- Con la caja medida en la escena abierta (Edit), la comprobación que hace la prueba da `desbordan = 0` y holgura 6; y lo mismo dentro de Play
  (§3, ensayo manual).

## 3. Comandos y resultados

Todo por `pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 …`. **No corrí `tests-play` ni `VisualVerification`.**

| Comando | Resultado |
|---|---|
| `gameview1080`, `autotick on` | Game View 1920×1080 |
| `eval @measure_*.cs` (TextGenerator sobre las 139 líneas, varios interlineados y escalas) | las cifras de §2 |
| `eval @edit_dialog*.cs` (cuadro, `SaveScene`), `eval @edit_cave*.cs` (`FireGlow` en `Fuego`, `SaveScene`) | guardadas; diff de `Narrative.unity` = 24 líneas; el de `Level1_Cave.unity` = solo el componente (ver §8, `restingSliderAlpha`) |
| edición YAML de 7 `.asset` (LF) + `ImportAsset` + `eval` de comprobación | 9 llamas con `Glows`, 1 balsa con `CameraFollows`, parada 0,52 |
| `recompile` (varias) | `Compilación OK (errores=0)` siempre; un error de mi prueba (`FormattableString` + `+`) lo vi y lo corregí al momento |
| `tests-edit FireGlow_` | **12/12** |
| `tests-edit NarrativeSequence_` | **16/16** (incluye las 2 nuevas) |
| `tests-edit CaveSceneData` | 1/1 |
| mutación: `CameraFollows` a 0 en el asset | `…LaCamaraDelCruceAcompanaALaBalsa` falla («la balsa lleva la marca»); restaurado → verde. Además, la cuenta de márgenes con la cámara sin seguirla da **−0,106** de pantalla (balsa 204 px fuera) frente a **+0,086** siguiéndola |
| mutación: parada 1 a 0,56 | la misma prueba falla («el foco que se ve no retrocede…»); restaurado → verde |
| mutación: un `Glows` a 0 | `…CadaLlamaEmiteSuHalo` falla; restaurado → verde |
| `tests-edit Game.Scaffolding.Tests assembly` | **292/292** (278 de antes + 12 `FireGlow` + 2 `NarrativeSequence`) |
| `tests-edit Game.Levels.Fire.Tests assembly` | **50/50** |
| `tests-edit Game.Architecture.Tests assembly` | **23/23** |
| `tests-edit Game.Content.Tests assembly` | **3/3** |
| `tests-edit Game.UI.Tests assembly` | **29/29** |

**Ensayo manual de las aserciones PlayMode** (no es una corrida del runner): en Play, `eval` asíncrono que replica a mano la lógica de cada prueba
nueva sobre las mismas escenas, 1920×1080, escala de canvas 1. Resultado con el diseño final: **todas pasan** —
`RNF01` (18 secuencias, 139 líneas, caja 680×137, máx. 131, 0 desbordan) · `RF05` hablante (caja 680×52, una línea 48) ·
`RNF03` cuarto (cima a 232 de 270) · `RNF03` nombre/texto/botones (nombre 166–218 sobre el texto 25–162; el texto acaba en x = 1130 y «Omitir» empieza en 1146) ·
`RF44` sin leer (llegada 0,1200, desvío 0, margen mínimo 77 px ≥ 38, retrocede 0) · `RF44` leyendo a los 3 / 5,5 / 8 s (margen mínimo 111 px, retrocede 0) ·
halos de `N2_Escena25_Cierre` y `N3_EscenaFinal` (1 y 3, nombre, color, sprite, centro a < 1 px y ancho) · pulso (escala 0,9500–1,0500, cambio máximo 0,1595/s,
0 cuadros fuera de la onda, 1 color) · pausa (quieto; el ensayo del código final, con la onda a 1,030). Y para la cueva: `Halo_Fuego` en el índice 0 de `Suelo`
(el montón, en el 8), color `F0A84E33`, sin clics, llama la última (13/13), lado 601,2 px (334 × 1,8).

## 4. Archivos tocados

```
 M Assets/Game/Data/Narrative/N1_NacimientoDelFuego.asset      (+1: Glows)
 M Assets/Game/Data/Narrative/N2_Escena25_Cierre.asset         (+1)
 M Assets/Game/Data/Narrative/N2_PuenteI.asset                 (+1)
 M Assets/Game/Data/Narrative/N3_Escena33_Cruce.asset          (CameraFollows, parada 0,47 → 0,52)
 M Assets/Game/Data/Narrative/N3_EscenaFinal.asset             (+3)
 M Assets/Game/Data/Narrative/N3_PuenteII.asset                (+1)
 M Assets/Game/Data/Narrative/N3_PuenteII_Horizonte.asset      (+2)
 M Assets/Game/Scenes/Level1_Cave.unity                        (FireGlow en Fuego)
 M Assets/Game/Scenes/Narrative.unity                          (cuadro de diálogo)
 M Assets/Game/Scripts/Runtime/Scaffolding/NarrativeProp.cs    (Glows, CameraFollows)
 M Assets/Game/Scripts/Runtime/UI/NarrativeSceneController.cs  (Followed/StopFraming, FireGlow.Attach)
?? Assets/Game/Scripts/Runtime/Scaffolding/FireGlow.cs (+ .meta)
 M Assets/Tests/EditMode/Levels/Fire/Game.Levels.Fire.Tests.asmdef  (+ Game.Scaffolding)
?? Assets/Tests/EditMode/Levels/Fire/CaveSceneDataTests.cs (+ .meta)
?? Assets/Tests/EditMode/Scaffolding/FireGlowTests.cs (+ .meta)
 M Assets/Tests/EditMode/Scaffolding/NarrativeSequenceTests.cs
 M Assets/Tests/PlayMode/Levels/Fire/FirePanelTests.cs
 M Assets/Tests/PlayMode/UI/NarrativeSceneTests.cs
?? claudeDocs/tasks/Ajustes-Diseno-2026-10-08/   (este parte + capturas/03-narrativa/)
```
No toqué lo de Papá, el laberinto ni `CLAUDE.md`. Nada en `ProjectSettings`. Ningún archivo escrito dentro de `Assets/` por las capturas (usé
`ScreenCapture.CaptureScreenshotAsTexture` + `File.WriteAllBytes` a rutas absolutas); sin andamiaje de Editor en `Assets/` (los scripts de
trabajo viven en el scratchpad). Los `.cs` nuevos están en CRLF como el resto.

## 5. Qué PlayMode debe correr el revisor

Mínimo (las que escribí o cambié), con el Editor recién abierto:
```
tests-play NarrativeScene_RNF01_TodasLasLineas
tests-play NarrativeScene_RF05_ElNombreDelHablante
tests-play NarrativeScene_RNF03_ElNombreElTexto
tests-play NarrativeScene_RF44_
tests-play NarrativeScene_RF05_CadaFogataTieneSuHalo
tests-play NarrativeScene_RNF21_ElHalo
tests-play NarrativeScene_RF05_LaEscena21MuestraLosObjetos
tests-play FireLevel_RF20_
```
Recomendado, porque el cuadro sube de 196 a 232 px de cima y la cámara del cruce cambia: **la clase entera** `NarrativeScene_` (incluidas
`…RNF03_ElCuadroDeDialogoNoSuperaElCuartoDeLaPantalla`, `…RNF03_NingunObjetoQueSeMueveQuedaBajoElCuadroDeDialogoEnElNivel2`,
`…RNF21_LaBalsaCruzaSinSaltosNiParpadeos`, `…RNF21_AvanzarElTextoDuranteElCruce…`, `…RF05_QuienCaminaTerminaSuCamino…`), `FirePanel` completa (la llama
cenital) y, como `VisualVerification`, `NarrativeScene_RF05_CapturaCadaLineaConLosPersonajes` y `…CapturaCadaParadaDeLaEscenaDelNivel2` para ver el
cuadro nuevo en las 18 escenas. Lo que más riesgo tiene: el alto del cuadro en `…ElNivel2` (los objetos están verificados por encima del cuarto, 270 px, y
el cuadro llega a 232, así que no debería); y el margen de la balsa (`RF44`, 2 % de pantalla = 38 px; medí 77 px en el peor caso).

## 6. Documentos que dicen hoy 26 px, #3A1E18, 22 px, «1,2 s» o «sin degradado» (a corregir por el agente de documentos)

| Ruta : línea | Dice hoy | Debe decir |
|---|---|---|
| `claudeDocs/Direccion_de_Arte.md:861` (§10.3) | «El texto es siempre `#3A1E18` sobre marfil.» | el texto del diálogo es `#1F100C` (carbón más oscuro; `#3A1E18` sigue siendo el de contornos y botones) |
| `claudeDocs/Direccion_de_Arte.md:850` (§10.3) | «una tablilla de 1400 × 180 px centrada abajo, a 16 px del borde» | 1400 × 216 px (cima a 232 px, dentro del cuarto inferior); y que el retrato se centra en vertical |
| `claudeDocs/Direccion_de_Arte.md:895` (§11.3) | «Diálogo \| 26 px (nombre del hablante en Baloo 2 Bold a 22 px) \| Nunito \| 400 \| 1.35» | «Diálogo \| 30 px (nombre del hablante en Baloo 2 Bold a 30 px) \| Nunito \| 600 \| 1.1»; ambos en `#1F100C` |
| `claudeDocs/Direccion_de_Arte.md:900-902` (§11.3) | «Mínimo: 26 px … Solo bajan de ese tamaño los rótulos secundarios —nombre del hablante, rótulo «Algoritm» del resumen, etiqueta del laberinto, «Aún no» del taller, a 22 px—…» | sacar «nombre del hablante» de la lista de rótulos de 22 px (ahora 30) |
| `claudeDocs/Direccion_de_Arte.md:906` (§11.4) | «Máximo 2 líneas por cuadro de diálogo, 12 palabras por línea.» | ya no se cumplía antes (el contenido radicado trae 17 líneas de tres renglones a 30 px, 9 a 26 px): decir tres, o dejarlo como aspiración |
| `claudeDocs/Direccion_de_Arte.md:556` (§8.1, tabla de la hoguera) | «Halo de luz \| `#F0A84E` al 20 % \| Círculo plano, sin degradado, escala oscilante» | círculo con degradado radial (opacidad 1 hasta 0,3 del radio y a 0 en el borde), `#F0A84E` al 20 % en el centro |
| `claudeDocs/Direccion_de_Arte.md:558-559` (§8.1) | «El halo es un círculo de color plano, no un degradado radial. Oscila su escala entre 0.95 y 1.05 en un ciclo de 1.2 s…» | degradado radial **sí** (un disco plano a 20 % se lee como mancha: capturas del 08/10/2026), y ciclo de **2 s** (decisión de Santiago) |
| `claudeDocs/Interfaces.md:155-157` (§Tipografía) | «diálogo a 26 px y texto de lectura desde 26 px; bajan de ahí el nombre del hablante, el rótulo «Algoritm» del resumen, la etiqueta del laberinto y el «Aún no» del taller (22 px)…» | diálogo a 30 px; el nombre del hablante a 30 px (ya no baja de 26) |
| `claudeDocs/Interfaces.md:148` y `claudeDocs/Direccion_de_Arte.md` §4.3 (neutros de interfaz) | lista carbón `#3A1E18` y carbón suave `#6B5248` | si se quiere el `#1F100C` en la paleta, añadirlo como «carbón oscuro, solo texto del diálogo»; el `#6B5248` ya no se usa en el nombre del hablante |
| `claudeDocs/SPEC.md:296` (Andamiaje) y `CLAUDE.md` (párrafo de `PropShadow`, l. 108) | no mencionan `FireGlow` | añadir `FireGlow` al andamiaje; en CLAUDE.md, una frase: el fuego no proyecta sombra y emite `FireGlow` (hermano antes de la llama, `NarrativeProp.Glows`, o `BehindSiblings` en la cueva) |
| `CLAUDE.md:413` (balsa, `MotionAmbient`) y `claudeDocs/entregables/OE3/src/anexos/F-personajes.md:46-50` | la balsa de la 3.3 y `FinishesSteps` | añadir `NarrativeProp.CameraFollows` (la cámara acompaña a la balsa; el foco de cada parada se corre lo que ella se ha movido) |
| `claudeDocs/entregables/OE3/src/anexos/E-arte-y-sonido.md:282` | tabla con `BurnReveal.cs` | añadir `FireGlow.cs` |
| `claudeDocs/tasks/Slice 1/plan.md:898` | «Halo de luz #F0A84E al 20 por ciento, círculo plano» | **no se reescribe** (los `plan.md` son historia) |
| `claudeDocs/INCONSISTENCIAS.md:1264-1268` | «diálogo a 26 px … nombre del hablante … 22 px» (nota de INC-79) | historia: dejarla; si se quiere, una nota fechada 08/10 nueva |

`claudeDocs/Camara_Narrativa_N3.md` ya no existe en el equipo (CLAUDE.md lo dice): lo aplicado de la 3.3 sobrevive solo en `N3_Escena33_Cruce.asset`.

## 7. Capturas (`claudeDocs/tasks/Ajustes-Diseno-2026-10-08/capturas/03-narrativa/`, 1920×1080 salvo hojas y recortes)

Hechas en Play manual (`ScreenCapture.CaptureScreenshotAsTexture`), mismo guion de clics y mismos tiempos antes y después.

- **Cuadro de diálogo con la línea más alta:** `antes-01-cuadro-linea-mas-alta.png` / `despues-01-cuadro-linea-mas-alta.png` (acotación `N1_AparicionGuia#1`,
  tres renglones) y `antes-02-cuadro-hablante-tres-renglones.png` / `despues-02-…png` (`N1_NacimientoDelFuego#13`, ALGORITM con retrato).
  **`comparacion-cuadro-hablante-1a1.png`**: recorte a 1:1 antes/después **con los valores en el pie de cada mitad** (fuente, tamaño, color, interlineado,
  caja, separación nombre-texto, alto del cuadro). Decisión: `decision-cuadro-interlineado-y-separacion.png` (nueve combinaciones, la elegida marcada).
- **Cruce de la balsa (cada segundo, `t` en cada cuadro):** `antes-cruce-cada-segundo-leyendo.png` / `despues-cruce-cada-segundo-leyendo.png` (clics a 3, 5,5, 8 y 11 s;
  hojas de 1920×1080), `antes-cruce-cada-segundo-sin-leer.png` / `despues-…-sin-leer.png` (sin avanzar el texto), `despues-cruce-cada-segundo-lectura-rapida.png`,
  `despues-cruce-cada-dos-segundos-lectura-lenta.png`, y cuadros enteros `antes-cruce-sin-leer-t06.3.png` / `despues-…`, `…-t09.3.png`.
- **Halo con oscuridad y a plena luz:** `antes-halo-oscuridad.png` / `despues-halo-oscuridad.png` (`N2_Escena25_Cierre`, línea 0), `antes-halo-plena-luz.png` /
  `despues-halo-plena-luz.png` (`N3_EscenaFinal`, línea 4). Decisiones: `decision-halo-plano-frente-a-degradado.png` (disco plano y degradado, noche y plena luz),
  `decision-halo-perfiles-del-degradado.png`, `decision-halo-cueva-v1-justo-antes-de-la-llama-vela-el-monton.png` y `decision-halo-cueva-definitivo-al-fondo.png`.
- **La llama del N1:** `antes-llama-n1-cierre.png` / `despues-llama-n1-cierre.png` (cierre del nivel, narrativa) y `antes-llama-n1-cenital.png` /
  `despues-llama-n1-cenital.png` + `…-brasa.png` (la llama cenital de `Level1_Cave`, dos instantes del bucle, la cámara acercada).

Medido sobre las capturas del cruce (centro y bordes de la balsa en pantalla, 1920 de ancho):

| Recorrido | Antes | Después |
|---|---|---|
| Sin avanzar el texto: margen mínimo a la derecha | **−204 px** (la balsa asoma desde los 4,3 s y acaba medio fuera) | **+77 px** (a los 6,4 s) |
| Leyendo a los 3 / 5,5 / 8 s: margen mínimo a la derecha | 100 px | 125 px |
| Leyendo: foco interno de la cámara hasta el desembarco | no retrocede | no retrocede (el **foco que se ve** tampoco, dentro de 0,001) |
| Lectura rápida (clics a 1, 1,6, 2,2 y 2,8 s) / lenta (6, 12, 18, 24 s) | — | margen mínimo 164 px / 88 px |
| Centro de la balsa en pantalla, leyendo: del máximo al mínimo | 1578 → 1117 px (−461) | 1553 → 972 px (**−581**) |

## 8. Dudas y observaciones

1. **«Para que no retroceda» (D7) — lo interpreté, y quiero que lo confirmen.** Con mi implementación (`foco = parada + lo que se movió la balsa`) la **cámara nunca
   retrocede** y la **balsa no se sale del cuadro** (lo pedido y medido arriba); subir la parada de la línea 1 de 0,47 a 0,52 no hacía falta para eso (con 0,47
   tampoco retrocede), pero la dejé como pidió: con ella la balsa llega centrada ya en la línea 1 y no hay retroceso del foco acotado hasta el límite 0,547.
   **Lo que sí se ve —y no quita ningún valor del brief— es que la balsa se desliza hacia la izquierda de la pantalla** (581 px, antes 461) cuando las paradas 1–3
   adelantan la cámara hacia la orilla de llegada: la apertura (foco 0,38) la deja a la derecha, y el plano tiene que terminar centrado en la orilla. Si por «retroceda»
   Santiago se refería a **esa** reversión de la balsa en pantalla, el arranque es lo que la causa: con el arranque en **0,50** (en vez de 0,38) el deslizamiento baja a
   ~227 px y el margen derecho sube a ~480 px (modelo del movimiento calibrado contra las mediciones, con error de ~10–20 px); con 0,46, ~350 px. **No toqué
   `CameraStart`**: es el encuadre de apertura de la escena y no me lo pidieron. Otra lectura posible del brief (el seguimiento solo hasta la primera parada, con
   paradas absolutas después) da ~446 px y no cubre al estudiante que se queda en la línea 1.
2. **`FireGlow` en la cueva con `BehindSiblings`** (decisión mía, razonada en §1): el brief pedía «hermano justo antes del fuego» y eso en la cueva lava el montón y las
   piedras. Si Santiago prefiere la regla única, basta desmarcar `BehindSiblings` en `Level1_Cave` (la prueba `CaveSceneDataTests` y la PlayMode de la cueva dejarían de pedirlo).
3. **El degradado radial contradice a la DA** («círculo plano, sin degradado»): lo decidí por las capturas y está documentado arriba; la DA debe actualizarse.
   El pulso de 0,95–1,05 sobre un degradado suave es discreto (el brillo del halo respira poco): si se quiere más evidente, `CoreRadius` más alto (0,5 da un borde más definido)
   o `Spread` mayor; son constantes de `FireGlow`.
4. **Las llamas respiran todas a la vez** (misma onda de `Time.time`, sin desfase) — en la escena final, las tres. No lo pidieron; si se quiere desfasarlas es una línea.
5. **El cuadro más alto tapa 36 px más de pantalla** (cima a 232 px en vez de 196; el cuarto, 270). Los objetos están verificados por encima del cuarto, pero conviene
   la corrida de `…ElNivel2` y las capturas `VisualVerification`.
6. **En las acotaciones el texto queda 18 px más abajo** (el hueco del nombre sigue reservado, §1). Si no gusta, se puede subir el texto cuando no hay nombre, con un `if` en
   `NarrativeSceneController.Render` (hoy no hay ninguno por el diseño «el texto no salta»).
7. **`Level1_Cave.unity`:** cada vez que Unity guarda esta escena escribe `restingSliderAlpha: 0.5` en `FirePanelController` (campo que se añadió al código después del último guardado de la
   escena; mismo valor que el por defecto). **Lo quité dos veces a mano** para que mi diff sea solo el componente; reaparecerá en el próximo guardado de cualquiera: inofensivo.
8. **Excepción preexistente que vi y no es mía:** `NullReferenceException` en `GuideTrail.Step` (`GuideTrail.cs:70`, `_path` nulo) al cargar `Level1_Cave` justo después de una escena
   narrativa dentro de la misma sesión de Play del arnés; desde una sesión limpia no aparece. No toqué `GuideTrail`.
9. **Python con PIL:** el del equipo sí lo tiene (12.3.0), al contrario de lo que dice el brief en «Entorno» (ya lo avisó la etapa 2).
10. **No pude verificar** (no se debía): ninguna PlayMode con el runner, ni `VisualVerification`. Cubrí con el ensayo manual de §3, las mutaciones de las EditMode y las capturas.

## 9. Kanban y mensaje de commit

La última acta es la **D10 (30/09/2026)** y su §6 no trae ninguna tarjeta que cubra la narrativa, el halo de las fogatas ni la cámara del cruce: **no hay tarjeta abierta** para esta ronda.

```
Narrative: bigger dark dialogue text, camera follows the raft, soft glow on every fire

D5/D6/D17: the dialogue text is Nunito SemiBold 30 px (line spacing 1.1, was
Regular 26 px at 1.35) and the speaker name Baloo 2 Bold 30 px, both #1F100C
(were #3A1E18 and #6B5248 at 22 px). The panel goes from 180 to 216 px, still
inside the bottom quarter, the text starts 18 px lower so the name has room,
and the portrait is centred. The tallest of the 139 lines is 131 px in a 137 px
box, no line needs a fourth row.

D7: NarrativeProp.CameraFollows. The followed object's displacement is added to
the focus of the current stop, so the raft no longer sails out of the frame when
the text takes its time (it was 204 px outside at the end); the focus the camera
shows never goes back before the landing. N3_Escena33_Cruce marks the raft and
raises the line-1 stop from 0.47 to 0.52.

D8: FireGlow, a #F0A84E glow at 20 % that breathes between 0.95 and 1.05 over
2 s on scaled time, no flashing (RNF-21). A flat circle at 20 % read as an
orange blotch (night) or as ghost rings (daylight), so the sprite is a radial
falloff generated in code. NarrativeProp.Glows marks the nine narrative flames;
the cenital flame of Level1_Cave carries the component in the scene and its glow
sits at the bottom of Suelo so it does not wash out the pile and the stones.

Tests: FireGlowTests, NarrativeSequence_RF05_CadaLlamaEmiteSuHalo,
NarrativeSequence_RF44_LaCamaraDelCruceAcompanaALaBalsa and CaveSceneDataTests
(EditMode); NarrativeScene_RNF01_TodasLasLineasDeLasDieciochoNarrativasCabenEnSuCuadro
replaces the longest-line test, plus the speaker, layout, camera and halo tests
and FireLevel_RF20_LaLlamaCenitalEmiteSuHaloSinVelarElMonton (PlayMode, not run
by the implementer).

Decisión de Santiago, 08/10/2026. Documents (Direccion_de_Arte.md, Interfaces.md,
SPEC.md, CLAUDE.md, OE3 annexes) follow in their own commit.
Kanban: no open card covers it (last acta D10, 30/09/2026).

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
```
