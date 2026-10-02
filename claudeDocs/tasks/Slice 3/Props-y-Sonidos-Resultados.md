# Carril de arte y sonido — lo construido, archivo por archivo

Documento de resultados de la rama `feat/implementación-de-props-y-sonidos`, hermano de
[`Slice-3-Resultados.md`](Slice-3-Resultados.md). Vive en la carpeta del Slice 3 porque la rama
nació justo después de su cierre (`76d7375`, PR #80), pero **no es un slice**: es el carril de
arte y sonido (tarjetas `D05-2` sonido y `D07-2` sprites/sonido de las actas OE3), que toca
`Assets/Game/Art/`, `Assets/Game/Audio/`, `Game.Audio`, `Game.Levels.Fire`, `Game.Levels.Wheel` y,
de paso, `Game.Core`, `Game.Scaffolding` y `Game.UI`. No rediscute `claudeDocs/SPEC.md`.

> **Estado al 25/09/2026:** el trabajo que registra este documento se fusionó con el PR #82
> (`d5fa77f`, 23/09); el documento mismo es `13e2986` (23/09, hijo de `d5fa77f`), que no entró en
> ese PR y llegó a `main` con el PR #86, porque `88fe0ee` cuelga de él. Después, la misma rama siguió desde `main` (`131b4c0`, tras el PR #86) con cinco commits más del
> carril —`b38c00b` y `f801186` (24/09), `1d5ce58`, `d3a6cc9` y `ccf77e6` (25/09)—, que `main`
> todavía no tiene: troncos del N2 que ruedan como cilindros, humo animado sobre cada fuego de las
> narrativas, caja vacía y cuerda del taller (INC-54), hoguera animada del puente II y de la
> escena final, y el sonido del Nivel 3, que suma `Game.Levels.River` a los módulos que toca el
> carril. Todo eso va archivo por archivo en el **Anexo** del final, con la numeración de estas
> secciones (§A.1 ↔ §1 … §A.10 ↔ §10). Las secciones 1–10 conservan el corte del 23/09 con las
> notas fechadas del 25/09 que ya tenían; lo que dejó de ser cierto lleva una nota *(Vencido…)*.
> Los personajes animados (`88fe0ee`, PR #86) no son de este carril: están en
> [`Personajes-Resultados.md`](../Personajes/Personajes-Resultados.md).
>
> **01/10/2026:** el Anexo B entró con `44fd479` y la rama está fusionada en `main` desde el PR #87
> (`995b26d`, 29/09). Lo que llegó al arte y al sonido hasta el 01/10 —el arte de los menús de
> `37b3cb7` y las correcciones del acta D10: el rayo del N1, la carretilla amarrada en el taller, la
> salpicadura renombrada y cableada, la escena final con ambiente y la 3.1 en 3/4— está en el
> [Anexo C](#anexo-c--lo-que-cambió-después-del-25092026-01102026).

| Campo | Dato |
|---|---|
| **Rama** | `feat/implementación-de-props-y-sonidos`, desde `76d7375` (`main`, 21/09/2026) |
| **Fusión** | PR #82 → `d5fa77f` (`main` y la rama apuntan hoy al mismo commit) *(Vencido el 25/09/2026: la rama siguió desde `131b4c0` con cinco commits que `main` aún no tiene; ver Anexo.)* |
| **Fechas** | 21/09/2026 – 23/09/2026 *(Actualizado el 25/09/2026: primera tanda; la segunda, 24/09 – 25/09, en el Anexo.)* |
| **Commits** | 6: `2cbe287`, `dc51804`, `aca7acc`, `8ec5120`, `03675b9`, `d7ace67` *(Actualizado el 25/09/2026: después del cierre, 5 más: `b38c00b`, `f801186`, `1d5ce58`, `d3a6cc9`, `ccf77e6` — Anexo §A.1.)* |
| **Volumen** | 253 archivos, +11 491 / −400 líneas — 161 añadidos, 72 modificados, 18 renombrados, 2 borrados |
| **De ellos** | 68 son los 34 cuadros del fuego con su `.meta`; 60 son `.meta` de assets nuevos o renombrados |
| **Verificación registrada** | Solo la de `8ec5120`: EditMode `NarrativeSequenceTests` 9/9 y `Architecture` en verde; PlayMode 60/65 (4 capturas saltadas en batchmode, `NarrativeScene_RNF01` falla por 1 px en `N1_Hallazgo`, ajeno al cambio). **Los commits del Nivel 2 (`03675b9`, `d7ace67`) no dejan cifra de suite completa**, y este documento no la inventa. *(Los cinco posteriores tampoco la dejan; la corrida completa del 25/09/2026 está en `claudeDocs/tasks/Slice 4/Slice-4-Resultados.md`, «Corrida completa de la suite (25/09/2026)».)* |

---

## 1. Qué hizo cada commit

| Commit | Fecha | Resumen |
|---|---|---|
| `2cbe287` | 21/09 | **Audio del N1.** `AudioManager` (cuatro buses, ambiente en dos capas, fundidos cruzados, cortes secos, clic sostenido) en `Boot/Persistent` como único `AudioListener`. `N1_Sonidos`, campos de sonido en las secuencias y líneas, `AudioImportRules`, renombrado de los `.wav` a la nomenclatura §4.1. Golpe que suena solo con contacto y chispa solo en el golpe efectivo, ahora la muesca cinco (`EffectiveOverlap` 5 → 30). Sprites definitivos parciales del N1. |
| `dc51804` | 21/09 | **Clips del fuego.** `prop_n1_fuego_normal.anim` (2,667 s) y `prop_n1_fuego_cenital.anim` (5,633 s) con curva PPtr y su `.controller`; cuadros a `Single`; borrados los 215 cuadros duplicados (249 archivos → 34 dibujos, 1469 MB → 235 MB de texturas cargadas). |
| `aca7acc` | 22/09 | **Piezas y fuego del N1.** Giro aleatorio y gesto de levantar al tomar (`DraggablePiece`), montoncitos de hojas (`LeafPile`), montón cenital que sustituye a las hojas sueltas al reunir, llama cenital animada al soplar, `NarrativeProp.FrameAnimation` para la llama del cierre narrativo. |
| `8ec5120` | 22/09 | **Quemado y reparto del N1.** `BurnReveal` (máscara circular en memoria) sustituye al tinte uniforme; `FloorScatter` reparte las piezas al azar fuera de la interfaz (`keepClear`) y del círculo de reunión; montón más grande. |
| `03675b9` | 23/09 | **Audio del N2.** `N2_Sonidos` (`WheelSounds`) en bosque, taller y laberinto; `NarrativeProp.LandSound` en la 2.2; piedras `prop_n2_piedra_a..d` con recorte corregido; la 2.5 y el arranque de `N3_PuenteII` pasan al refugio con fuego; nuevo `N3_PuenteII_Horizonte`. |
| `d7ace67` | 23/09 | **El día del N2 y el laberinto.** Fundido a negro de `SceneLoader` entre narrativa y mecánica; luz del amanecer a la noche en las secuencias del N2 (nuevo `N2_PuenteI_Bosque`); laberinto con `fx_contraste`, cuadrícula, barra de desplazamiento por clic (`ClickRelay`) y papelera del bloque seleccionado; `ClaudeSceneAutosave` + hook `PreToolUse`. |

*(Actualizado el 25/09/2026: los cinco commits posteriores al cierre, del 24/09 y el 25/09, están en el Anexo, §A.1.)*

---

## 2. Código de juego (`Assets/Game/Scripts/Runtime/`)

### 2.1 `Game.Audio`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Audio/AudioManager.cs` | **Creado** (345 líneas) | Tercer singleton `DontDestroyOnLoad`. Cuatro buses con volumen (`MusicVolume` 0,8 · `AmbientVolume` 0,4 · `SfxVolume` 1 · `VoiceVolume` 0,6). API: `PlayAmbient`/`PlayAmbientLayer` (fundido cruzado; pedir el clip que ya suena no lo reinicia), `PlayMusic`, `PlaySfx(clip, pitchJitter, volume)`, `PlayHeld`/`StopHeld` (bucle mientras dura un clic sostenido), `PlayVoice`, `CutToSilence(keepAmbient)` (silencios del guion §5, corte seco). Expone en `internal` `LastSfx`, `SfxCount`, `HeldClip`, `AmbientClip`, `AmbientLayerClip`, `AmbientStarts` para las pruebas. |
| `Audio/AssemblyInfo.cs` | **Creado** | `InternalsVisibleTo` para `Game.Audio.PlayMode.Tests` y para `Game.Levels.{Fire,Wheel}.PlayMode.Tests`, que comprueban qué clip sonó. Desde el 25/09 también `Game.Levels.River.PlayMode.Tests` (sonido del N3). *(Corregido el 25/09/2026: la lista estaba incompleta ya al cierre; el archivo abre además `Game.Audio.Tests` —desde `2cbe287`, cuyo asmdef de EditMode aún no tiene pruebas— y `Game.UI.PlayMode.Tests` —desde `03675b9`—. Mapa completo en §A.2.5.)* |

`Game.Audio.asmdef` ya existía desde la Fase 0; no se tocó.

### 2.2 `Game.Core`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Core/SceneLoader.cs` | Modificado | `Load(sceneName, fade = false)`: con `fade` funde a negro en `FadeSeconds` (0,4 s por mitad, tiempo sin escalar), carga, espera un cuadro a que monten los `Start` y vuelve. Una carga nueva a mitad corta la anterior. El negro se pinta en `OnGUI` (`Game.Core` no referencia uGUI). Nuevas propiedades `FadeAlpha`, `IsFading`. |
| `Core/GameFlowRunner.cs` | Modificado | `internal static FadesBetween(from, to)`: funde narrativa ↔ mecánica y narrativa → narrativa; menús, resumen y reinicio cortan en seco. La llamada a `SceneLoader.Load` pasa ese valor. |
| `Core/AssemblyInfo.cs` | **Creado** | `InternalsVisibleTo` para `Game.Core.Tests` y `Game.Core.PlayMode.Tests` (primer `AssemblyInfo` del módulo, para probar `FadesBetween`). |

### 2.3 `Game.Scaffolding`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Scaffolding/BurnReveal.cs` | **Creado** | Quema una imagen desde `Origin`: crea en `Awake` un hijo `Brasa` con `Mask` y disco generado en memoria (compartido) y dentro una copia `Quemado` teñida de `EmberColor`. `Extent` (fracción del ancho) fija el radio; solo cambia cuando se pide (RNF-21). |
| `Scaffolding/SilenceCut.cs` | **Creado** | Enum `None` · `Music` (calla música y voz, queda el ambiente — S2) · `Everything` (S1). |
| `Scaffolding/DialogueLine.cs` | Modificado | Campos `Sound`, `Ambient` y `Silence` por línea. |
| `Scaffolding/NarrativeSequence.cs` | Modificado | Campos `Ambient` y `AmbientLayer` por secuencia (la hoguera sobre la cueva). |
| `Scaffolding/NarrativeProp.cs` | Modificado | `FrameAnimation` (`RuntimeAnimatorController`), `BurnExtent` y `LandSound` (con `FormerlySerializedAs` del nombre previo `MotionEndSound`). |
| `Scaffolding/Game.Scaffolding.asmdef` | Modificado | Referencia `UnityEngine.UI` (por `BurnReveal`). |

### 2.4 `Game.UI`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `UI/NarrativeSceneController.cs` | Modificado | Pide al gestor el ambiente y la capa de la secuencia; añade `Animator` a los objetos con `FrameAnimation` y `BurnReveal` a los que tienen `BurnExtent`; suena `LandSound` al posarse (a mitad si se levanta, al final si rueda); por línea aplica `Silence`, `Ambient` y `Sound`. Todo protegido contra `AudioManager.Instance` nulo. |
| `UI/Game.UI.asmdef` | Modificado | Referencia `Game.Audio`. |

### 2.5 `Game.Levels.Fire`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Fire/FireSounds.cs` | **Creado** | ScriptableObject `N1_Sonidos`: `CaveAmbient`, `FireAmbient` (+ `FireAmbientFadeSeconds` 3 s), `LeafDrag`, `Gathered`, `Strike` (+ `StrikeMinVolume` 0,3, `StrikePitchJitter` 0,06), `Spark`, `Blow`. Ninguna pieza de fallo (CP-02, comentado). |
| `Fire/FloorScatter.cs` | **Creado** | C# plano, muestreo por rechazo: coloca huellas cuadradas dentro del suelo sin pisar zonas bloqueadas ni entre sí (200 intentos por pieza; si se agotan se queda con la última). |
| `Fire/LeafPile.cs` | **Creado** | Clona el sprite de la hoja en `Leaves − 1` hijos girados y desplazados (`Spread` 0,28): cada pieza se ve como un montoncito; los hijos amplían la zona de agarre. |
| `Fire/DraggablePiece.cs` | Modificado | Giro de reposo aleatorio en `Awake`; al tomar crece (`LiftScale` 1,12) y se inclina (`LiftTiltDegrees` 8°) en `LiftSeconds` 0,12 s y vuelve al soltar, una sola interpolación que se interrumpe limpia. Nuevo evento `PickedUp`. |
| `Fire/FirePanelController.cs` | Modificado | Campos `keepClear`, `leafPile`, `fireFlame`, `burn`, `sounds`. Reparte las piezas con `FloorScatter` al arrancar; hojas suenan mientras se arrastran; al reunir, las hojas convergen y entra por fundido el montón cenital (las sueltas se ocultan); se eliminó el anillo de hojas. Golpe: suena solo si hay contacto, con volumen según `StoneSpacing.Contact`; la chispa solo en el efectivo. Al soplar: soplo, capa de hoguera, llama cenital activa y quemado que crece hasta `BurnExtent` en `IgnitionSeconds` (sustituye al tinte de 0,6 s). |
| `Fire/FireLevelConfig.cs` | Modificado | `EffectiveOverlap` 5 → 30 (solo la muesca cinco); nuevos `IgnitionSeconds` 3,5 y `BurnExtent` 0,5. |
| `Fire/StoneSpacing.cs` | Modificado | `Contact(notch)`: 0 con hueco, 1 una encima de otra. |
| `Fire/Game.Levels.Fire.asmdef` | Modificado | Referencia `Game.Audio`. |

### 2.6 `Game.Levels.Wheel`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Wheel/WheelSounds.cs` | **Creado** | ScriptableObject `N2_Sonidos`: `ForestAmbient`, `LogNudge`, `StoneNudge`, `LeafNudge`, `NudgePitchJitter`, `CartMove`, `LogCollected`, `AllLogsCollected`, `Drilled`, `AssemblyHammer` (+ `AssemblyHits` 3, `AssemblyHitSeconds` 0,3), `CartBuilt`; `NudgeClipFor(categoría)`. |
| `Wheel/ClickRelay.cs` | **Creado** | Solo `IPointerClickHandler`: avisa del clic y su punto de pantalla. Hace pulsable la barra de desplazamiento sin `ScrollRect` ni arrastre (RNF-02, CT-06). |
| `Wheel/ForestObjectNudge.cs` | Modificado | `Step` devuelve si el cursor acaba de entrar en el radio (sonido una vez por acercamiento); `IsMoving` con umbral `StillSpeed`; `_armed` pasa a `_wasInside`. |
| `Wheel/ForestSceneController.cs` | Modificado | Ambiente del bosque sin costura desde la narrativa; sonido por categoría al apartarse un objeto; bucle de hojas mientras alguna planta se mueve; `encaje_pieza` por tronco acopiado y `pieza_tomar` con los cinco. |
| `Wheel/WorkshopSceneController.cs` | Modificado | Ambiente del bosque; `encaje_pieza` al perforar; tres martillazos por pieza encajada (`HammerAsync`); `pieza_tomar` al terminar la carretilla. |
| `Wheel/MazeLayout.cs` | Modificado | Nuevos campos `DeleteIcon`, `LightTint`, `EnvironmentMaterial`, `Contrast`, `Saturation`, `BackdropColor`, `GridColor`, `GridThickness`. |
| `Wheel/MazeSceneController.cs` | Modificado (+285) | Material de contraste por escena (copia, se destruye en `OnDestroy`) y panel del color del borde con la misma luz; `DrawGrid` pinta la cuadrícula del interior del seto detrás de las piezas; el refugio ya no se marca con un cuadro de color; `BuildScrollBar` + `ScrollToPoint` (barra a la derecha, se pulsa, no se arrastra); la lista queda abajo al soltar un bloque y al abrir/cerrar el cajón; papelera junto al bloque seleccionado (`AddDeleteButton`/`DeleteBlock`, sin confirmación, RF-34); carretilla en bucle mientras recorre la secuencia. |
| `Wheel/Game.Levels.Wheel.asmdef` | Modificado | Referencia `Game.Audio`. |

---

## 3. Código de Editor (`Assets/Game/Scripts/Editor/`, `Game.EditorTools`)

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `AudioImportRules.cs` | **Creado** | `AssetPostprocessor` con la tabla §4.3 por prefijo: `mus_` streaming estéreo, `amb_` streaming mono, el resto efecto — hasta 2 s PCM precargado, más largo Vorbis en memoria. Lee la duración de la cabecera del `.wav`. |
| `ClaudeSceneAutosave.cs` | **Creado** | Guarda las escenas que se ensucian en los 2 minutos siguientes a que el hook toque `Temp/claude-active`; lo sucio con Claude inactivo no se toca. Evita el diálogo modal «Scene(s) Have Been Modified». |

---

## 4. Datos (`Assets/Game/Data/`)

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Fire/N1_Sonidos.asset` | **Creado** | Instancia de `FireSounds` con las piezas del N1. |
| `Fire/N1_Config.asset` | Modificado | `EffectiveOverlap` 30, `IgnitionSeconds` 3,5, `BurnExtent` 0,5. |
| `Wheel/N2_Sonidos.asset` | **Creado** | Instancia de `WheelSounds` con las piezas del N2. |
| `Wheel/N2_MazeLayout.asset` | Modificado | Atardecer `LightTint` (1, 0,95, 0,9), `fx_contraste`, saturación 0,75, contraste 1,1, fondo, cuadrícula, `ui_papelera`. |
| `Narrative/N1_Apertura.asset` | Modificado | Ambiente `amb_noche_intemperie`; una línea pasa a `amb_n1_cueva_oscura`, otra suena a `sfx_n1_pasos_eco`, un silencio `Everything` (S1). |
| `Narrative/N1_AparicionGuia.asset` | Modificado | Ambiente `amb_n1_cueva_oscura`; un silencio `Music` (S2). |
| `Narrative/N1_Hallazgo.asset` | Modificado | Ambiente de cueva; líneas que suenan a `sfx_n1_golpe`, `sfx_n1_chispa` y `sfx_n1_hojas_acomodo`. |
| `Narrative/N1_NacimientoDelFuego.asset` | Modificado | Ambiente cueva + capa `amb_n1_cueva_fuego`; las tres hojas sueltas pasan a `prop_n1_monton_hojas` con `BurnExtent` y la llama `prop_n1_fuego_normal` animada, sobre el cuadro de diálogo (RNF-03). |
| `Narrative/N2_PuenteI.asset` | Modificado | Pasa a `entorno_n1_apertura` al amanecer (tinte azulado 0,68/0,78/1) con fogata pequeña; ambiente bosque + capa hoguera; encadena a `N2_PuenteI_Bosque`. |
| `Narrative/N2_PuenteI_Bosque.asset` | **Creado** | Continuación desde «La familia sale a recolectar» sobre `env_n2_bosque_claro`, de tarde sin tinte; encadena a `N2_Escena21_Bosque`. |
| `Narrative/N2_Escena21_Bosque.asset` · `N2_Escena23_Construccion.asset` | Modificado | Ambiente `amb_n2_bosque_dia`; luz de tarde sin tinte; serialización de los campos nuevos. |
| `Narrative/N2_Escena22_ElPatron.asset` | Modificado | Ambiente de bosque; `LandSound`: tronco → `sfx_n2_troncos`, piedra y caja → `sfx_n2_piedra_cae`. |
| `Narrative/N2_Escena24_Regreso.asset` | Modificado | Ambiente de bosque; luz de atardecer (1/0,76/0,58). |
| `Narrative/N2_Escena25_Cierre.asset` | Modificado | Pasa al refugio: `entorno_n1_cueva_2x` con montón quemado y fuego animado, noche rojiza (1/0,62/0,42), ambiente cueva + hoguera. |
| `Narrative/N3_PuenteII.asset` | Modificado | Arranca en el refugio con fuego y la misma noche; encadena a `N3_PuenteII_Horizonte`. |
| `Narrative/N3_PuenteII_Horizonte.asset` | **Creado** | Sobre `env_enlace_n2`: el desplazamiento hacia el horizonte; encadena a `N3_PuenteII_Rio`. Desde el 25/09 la hoguera central ya no va pintada: son objetos (montón de hojas en (0,5023; 0,3617) a 0,11, humo, llama animada a 0,173) sobre el pie de los troncos que pintaba el entorno, con las proporciones de la fogata grande de `N3_EscenaFinal`, y el entorno se entrega sin ella. `N3_EscenaFinal` lleva la misma hoguera en el mismo sitio sobre `env_final_fogatas` (el mismo poblado al atardecer), dibujada después de las otras dos fogatas y antes de los personajes. |

Con estos dos assets nuevos el Nivel 2 y el Nivel 3 pasan de seis a **siete** secuencias cada uno.

---

## 5. Escenas (`Assets/Game/Scenes/`)

| Escena | Qué cambió |
|---|---|
| `Boot.unity` | `Persistent` gana `AudioManager` y el único `AudioListener`; `SceneLoader.FadeSeconds` 0,4. |
| `Level1_Cave.unity` (+310) | Retirado su `AudioListener`. Nuevos `MontonHojas` (con `BurnReveal`) y `Fuego` (Animator con `prop_n1_fuego_cenital`); `LeafPile` en las hojas; `FirePanelController` cableado a `keepClear`, `leafPile`, `fireFlame`, `burn` y `N1_Sonidos`; montón de 300 px, 30 px bajo el punto del fuego. |
| `Level2_Forest.unity` · `Level2_Workshop.unity` | Referencia a `N2_Sonidos`. |
| `Level2_Maze.unity` | Referencia a `N2_Sonidos`; cuatro `RectTransform` reserializados (anclas y tamaño a cero). |
| `Level3_River.unity` | Retirado su `AudioListener`. *(Desde el 25/09/2026 la orilla y el panel referencian además `N3_Sonidos`: ver §A.5.)* |
| `Narrative.unity` | Registra `N2_PuenteI_Bosque` y `N3_PuenteII_Horizonte` en la lista de secuencias. |

---

## 6. Arte (`Assets/Game/Art/`)

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Props/Fire/Animations/prop_n1_fuego_normal.anim` + `.controller` | **Creado** | 2,667 s a 30 fps en bucle, una clave por dibujo distinto (animado a seises). |
| `Props/Fire/Animations/prop_n1_fuego_cenital.anim` + `.controller` | **Creado** | 5,633 s a 30 fps en bucle (animado a ochos). |
| `Props/Fire/Animations/Fuego normal/` (13 PNG) · `Fuego cenital/` (21 PNG) | **Creado** | Los 34 cuadros que referencian los clips, en `Single` con pivote centrado; conservan el nombre de entrega (`fuego_*_nivel_1_NNNN.png`). Los 215 duplicados entraron en `2cbe287` y se borraron en `dc51804`, así que no aparecen en el diff neto. |
| `Props/Fire/prop_n1_monton_hojas.png` · `prop_n1_monton_hojas_cenital.png` | **Creado** | Montón para el cierre narrativo y para la cueva, en `Single`. |
| `Props/Fire/prop_n1_hoja.png` · `prop_n1_pedernal.png` · `prop_n1_silex.png` | Modificado | Sprites definitivos (parciales) del N1. |
| `Props/Wheel/prop_n2_piedra_{a,b,c,d}.png` + `.meta` | Modificado | PNG de 2000×2000; el recorte del sprite pasa de 256×256 en una esquina a la imagen entera, mismo `spriteID`. |
| `Environments/Narrative/env_final_fogatas.png` | Modificado | Nueva versión del entorno. |
| `Environments/Narrative/env_enlace_n2.png` | **Creado** | Entorno del puente II hacia el horizonte; reutiliza el `.meta` (GUID) de `civilización_noche`. *(Actualizado el 25/09/2026: `d3a6cc9` reemplaza la imagen, con el mismo `.meta`, por una versión sin la hoguera central pintada, que pasa a objetos; ver §4 y §A.6.)* |
| `Environments/Wheel/civilización_noche.png` | **Borrado** | Sustituido por `env_enlace_n2`. |
| `FX/fx_contraste.shader` + `fx_contraste.mat` | **Creado** | `Algoritm/UI Contraste`: `UI/Default` con `_Contrast` y `_Saturation`, compatible con máscara y recorte de uGUI; lo usa el laberinto. |

---

## 7. Audio (`Assets/Game/Audio/`)

Los `.wav` de PR #81 entraron sin `.meta`; esta rama les da `.meta` (GUID fijo) y los renombra a
§4.1 desde el motor.

| Antes | Después | Nota |
|---|---|---|
| `Global/Encaje_piezas.wav` | `Global/sfx_encaje_pieza.wav` | Renombrado |
| `Global/Martillo_contra_madera.wav` | `Global/sfx_martillo_madera.wav` | Renombrado |
| `Global/Seleccionar_objeto.wav` | `Global/sfx_n1_pieza_tomar.wav` | Renombrado |
| `Level 1/Cueva.wav` | `Level 1/amb_n1_cueva_oscura.wav` | Renombrado |
| `Level 1/Fogata.wav` | `Level 1/amb_n1_cueva_fuego.wav` | Renombrado |
| `Level 2/Animales_afuera_cueva.wav` | `Level 1/amb_noche_intemperie.wav` | Renombrado y movido al N1 |
| `Level 1/Piedras_golpeando.wav` | `Level 1/sfx_n1_golpe.wav` | Renombrado |
| `Level 1/Hojas.wav` | `Level 1/sfx_n1_hojas_acomodo.wav` | Renombrado |
| `Level 1/Pasos_eco_cueva.wav` | `Level 1/sfx_n1_pasos_eco.wav` | Renombrado |
| `Level 1/Soplar.wav` | `Level 1/sfx_n1_soplo.wav` | Renombrado |
| `Level 1/Chispa.wav` | `Level 1/sfx_n1_chispa.wav` | Borrado y recreado sin el silencio de cabeza |
| `Level 2/Bosque.wav` | `Level 2/amb_n2_bosque_dia.wav` | Renombrado |
| `Level 2/Carretilla.wav` | `Level 2/sfx_n2_carretilla.wav` | Renombrado |
| `Level 2/Piedra_cayendo.wav` | `Level 2/sfx_n2_piedra_cae.wav` | Renombrado |
| `Level 2/Troncos.wav` | `Level 2/sfx_n2_troncos.wav` | Renombrado |
| — | `Level 2/amb_n2_noche_intemperie.wav` | **Creado**; ningún asset lo referencia todavía |
| `Level 3/Rio.wav` | `Level 3/amb_n3_rio_orilla.wav` | Renombrado; desde el 25/09 lo referencian `N3_Sonidos` y las escenas del río |
| `Level 3/viento_rio.wav` | `Level 3/amb_viento_horizonte.wav` | Renombrado; sin referenciar. El 25/09 Santiago lo renombró fuera del motor a `amb_balsa_movimiento.wav` (GUID nuevo), que suena en el cruce de la 3.3 |
| `Level 3/Salpicadura_agua.wav` | `Level 3/sfx_n3_salpicadura.wav` | Renombrado; sin referenciar. El 25/09 pasó a `sfx_n3_salpicadura_undimiento.wav` fuera del motor (GUID nuevo); sigue sin referenciar |

---

## 8. Pruebas (`Assets/Tests/`)

| Archivo | Estado | Casos nuevos o cambiados |
|---|---|---|
| `EditMode/Architecture/AudioImportTest.cs` | **Creado** | `AudioImport_RNF06_CadaFamiliaEntraConLosAjustesDeSuTabla`, `AudioAssets_CP02_NingunaPiezaSeLlamaDerrotaNiError` |
| `EditMode/Architecture/AudioListenerTest.cs` | **Creado** | `AudioListener_RNF13_ElUnicoOyenteVaEnBootYNingunaOtraEscenaTieneElSuyo` |
| `EditMode/Core/SceneFadeTests.cs` | **Creado** | 7 casos de `GameFlowRunner.FadesBetween` (`GameFlowRunner_RF05_…`, `…_RF07_ReiniciarLaMecanicaNoFunde`) |
| `EditMode/Levels/Fire/FloorScatterTests.cs` | **Creado** | `FloorScatter_RF14_NingunaPiezaPisaLaInterfazNiElCirculoNiSeEncimaConOtra` |
| `EditMode/Levels/Fire/StoneSpacingTests.cs` | Modificado | `…_ElGolpeCerteroEsCuandoLasPiedrasSeRozan…` → `StoneSpacing_RF16_ElGolpeCerteroEsSoloLaMuescaCinco`; nuevo `StoneSpacing_RF16_ElContactoEsCeroConHuecoYCreceHastaEncimarlasDelTodo` |
| `EditMode/Levels/Fire/FireLevelConfigTests.cs` | Modificado | Espera `EffectiveOverlap` 30 |
| `EditMode/Levels/Wheel/ForestObjectNudgeTests.cs` | Modificado | `ForestObjectNudge_RF22_AvisaQueEmpiezaAApartarseUnaSolaVezPorAcercamiento`, `ForestObjectNudge_RF22_LaHojaDejaDeMoverseAlPosarse` |
| `EditMode/Scaffolding/NarrativeSequenceTests.cs` | Modificado | Seis → siete secuencias en N2 y N3; nuevo `NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche`; la 2.5 entra en `Verificadas` |
| `PlayMode/Audio/AudioManagerTests.cs` + `Game.Audio.PlayMode.Tests.asmdef` | **Creado** | `AudioManager_RF05_PedirElAmbienteQueYaSuenaNoLoReinicia`, `…_RF05_ElCorteSecoCallaElAmbienteAlInstante`, `…_RF05_ElCorteDeMusicaConservaElAmbiente`, `AudioManager_RF11_UnEfectoSeReproduceConVolumenAudible` |
| `PlayMode/Core/SceneLoaderTests.cs` | Modificado | `SceneLoader_RF05_ConFundidoCargaEnNegroYVuelveALaImagen`, `SceneLoader_RF05_SinFundidoNoOscureceNada` |
| `PlayMode/Levels/Fire/FireSoundsTests.cs` | **Creado** | `FirePanel_CP02_SeparadasNoSuenanEnContactoSuenanAPiedraYSoloElEfectivoAnadeLaChispa`, `FirePanel_RF20_SoplarSuenaYLaHogueraEntraSobreLaCueva`, `FirePanel_RF14_UnaHojaSuenaMientrasSeArrastraYReunirTodoSuenaAlPasarAlEncendido` |
| `PlayMode/Levels/Fire/FirePanelTests.cs` | Modificado | `FirePanel_RF14_LasPiezasAparecenGiradasAlAzarYCadaHojaEsUnMonton`, `…_RF14_LasPiezasAparecenAlAzarFueraDeLaInterfazYDelCirculo`, `FirePanel_RNF02_TomarUnaPiezaLaLevantaYSoltarlaLaPosa`, `FireLevel_RF20_AlSoplarPrendeLaLlamaCenitalSobreElMonton`; el RF14 existente comprueba el montón en lugar del anillo |
| `PlayMode/Levels/Fire/CaveLightingTests.cs` · `PlayMode/UI/LevelSummaryTests.cs` · `PlayMode/UI/PauseMenuTests.cs` | Modificado | La muesca efectiva pasa de 2 a 5 |
| `PlayMode/Levels/Wheel/ForestSceneTests.cs` | Modificado | `ForestScene_RF22_ElBosqueSuenaDeFondoYCadaObjetoSuenaALoQueEsAlApartarse`, `ForestScene_RF24_CadaTroncoAcopiadoSuenaAEncajeYLosCincoReunidosAPiezaTomada` |
| `PlayMode/Levels/Wheel/WorkshopSceneTests.cs` | Modificado | `WorkshopScene_RF29_PerforarSuenaAEncajeCadaPiezaATresMartillazosYElFinalAPiezaTomada` |
| `PlayMode/Levels/Wheel/MazeSceneTests.cs` | Modificado (+250) | `MazeScene_RF32_LaCarretillaSuenaMientrasRecorreLaSecuenciaYCallaAlTerminar`, `…_RF30_ElLaberintoEsAlAtardecer`, `…_RF31_LaCuadriculaDeLaMatrizSeDibujaDentroDelSeto`, `…_RF30_LaSalidaSeLeeEnElEntornoYElPanelLoContinua`, `…_RNF03_AlSoltarUnBloqueYAlAbrirElCajonLaSecuenciaQuedaAbajoDelTodo`, `…_RNF02_LaBarraDeDesplazamientoSePulsaPeroNoSeArrastra`, `…_RF34_ElBloqueSeleccionadoMuestraLaPapeleraYEstaLoRetira` |
| `PlayMode/UI/NarrativeSceneTests.cs` | Modificado | Seis → siete secuencias del N2; `NarrativeScene_RF20_ElCierreDelFuegoPintaElMontonConLaLlamaAnimada`, `NarrativeScene_RF26_LaEscena22SuenaCuandoCadaObjetoTocaElSuelo` |
| `PlayMode/Levels/{Fire,Wheel}/*.PlayMode.Tests.asmdef` · `PlayMode/UI/Game.UI.PlayMode.Tests.asmdef` | Modificado | Referencian `Game.Audio` |

---

## 9. Documentación y configuración

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `claudeDocs/Direccion_de_Musica_y_Sonido.md` | **Creado** (622 líneas) | La ley del audio: pilares, buses, nomenclatura, silencios §5, inventario de 104 piezas, §19 historial de lo aplicado (rev. 2 N1, rev. 3 N2). |
| `CLAUDE.md` | Modificado | Estado del carril de arte y sonido, el día del N2 por la luz, fundidos, `AudioManager` como tercer singleton, `ClaudeSceneAutosave`, reglas de importación de audio y de secuencias de cuadros. |
| `.claude/settings.json` | Modificado | Hook `PreToolUse` sobre `mcp__rider__*` y `mcp__coplay-mcp__*` que toca `Temp/claude-active`. |
| `.graphifyignore` | Modificado | Excluye `Assets/Game/Audio/**` (evita transcribir efectos con Whisper). |
| `ProjectSettings/ProjectSettings.asset` | Modificado | Define `SENTIS_ANALYTICS_ENABLED` en Standalone — lo añadió el Editor, no una tarea de la rama. |
| `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset` | Modificado | `m_List` de 13 entradas pasa a `[]` — reserialización del Editor, no una tarea de la rama. **Conviene revisar que no se haya perdido ningún ajuste de URP.** |

---

## 10. Lo que queda abierto

- **Sin corrida completa registrada** tras `03675b9` y `d7ace67`: hace falta una pasada EditMode +
  PlayMode con `unity test` (Editor cerrado) para dar cifra de la rama entera. *(25/09/2026: la
  corrida completa de ese día está en `claudeDocs/tasks/Slice 4/Slice-4-Resultados.md`,
  «Corrida completa de la suite (25/09/2026)».)*
- `NarrativeScene_RNF01` falla por 1 px en `N1_Hallazgo` (anotado en `8ec5120`, sin corregir).
- `amb_n2_noche_intemperie` y `sfx_n3_salpicadura_undimiento` están en disco pero **no las
  referencia ningún asset**; no hay música ni blip de diálogo (`PS-01..PS-05`). El resto del N3
  entró el 25/09/2026 con `N3_Sonidos` (`RiverSounds`): lo aplicado está en la rev. 4 de
  `Direccion_de_Musica_y_Sonido.md` §19.
- Los cuadros del fuego conservan el nombre de entrega y no el `prop_n1_…` de
  `Direccion_de_Arte.md`; renombrarlos es trabajo del motor.
- Revisar los dos cambios de configuración que hizo el Editor (§9).

---

## Anexo — lo que cambió después del cierre (25/09/2026)

Tras el PR #82 la rama `feat/implementación-de-props-y-sonidos` retomó desde `main` después del
PR #86 (`131b4c0`, 24/09) y siguió con cinco commits del carril. Este anexo los registra con la
misma disposición que las secciones 1–10; lo que ya estaba escrito arriba con fecha 25/09 no se
repite, se remite.

| Campo | Dato |
|---|---|
| **Base** | `131b4c0` (`main` tras el PR #86, personajes animados) |
| **Fusión** | Ninguna todavía: `main` sigue en `131b4c0`, cinco commits por detrás de la rama |
| **Fechas** | 24/09/2026 – 25/09/2026 |
| **Commits** | 5: `b38c00b`, `f801186`, `1d5ce58`, `d3a6cc9`, `ccf77e6` |
| **Volumen** | 173 archivos, +6 888 / −300 líneas (`git diff --shortstat 131b4c0 ccf77e6`); 66 son los 33 cuadros del humo con su `.meta` |
| **Módulos** | `Game.Scaffolding`, `Game.UI`, `Game.Levels.Wheel`, `Game.Levels.River` (nuevo en el carril) y `Game.Audio` (solo su `AssemblyInfo.cs`) |
| **Verificación registrada** | Ninguno de los cinco deja cifra de corrida. `b38c00b`, `f801186` y `ccf77e6` nombran sus pruebas en el mensaje; `1d5ce58` añade `AssemblySequence_INC54_LaCuerdaAmarraLaCajaYCompletaLaCarretilla` y `RollingLog_RF26_EnEspejoLaTexturaGiraComoSeVeGirarElObjeto` y renombra `WheelLevelConfig_RF23_…` sin nombrarlas (§A.8), y `d3a6cc9` no toca pruebas. La corrida completa del 25/09/2026 está en `claudeDocs/tasks/Slice 4/Slice-4-Resultados.md`, «Corrida completa de la suite (25/09/2026)». |

### A.1 Qué hizo cada commit

| Commit | Fecha | Tarjeta | Resumen |
|---|---|---|---|
| `b38c00b` | 24/09 | sin rellenar (`<id>`) | **Troncos que ruedan.** `RollingLog` dibuja el tronco del N2 como cilindro en 3/4 a partir de una sola textura (`prop_n2_tronco_textura`), con la vista en `N2_TroncoRodante.asset` (`NarrativeProp.Rolling`, `WheelLevelConfig.LogLook`); en el bosque el tronco que aparta el cursor rueda. Es el trabajo que pide `D09-1` (acta D09, 24/09), resuelto con una textura y no con cuerpo y veta separados. |
| `f801186` | 24/09 | `D08-2` | **Humo animado.** Dos clips en `Props/Fire/Animations/` (`fx_n1_humo_nacer`, que enlaza con `fx_n1_humo` en bucle) y 33 dibujos en `Smoke/`; humo que nace en `N1_NacimientoDelFuego` y humo en bucle detrás de cada llama de `N2_PuenteI`, `N2_Escena25_Cierre`, `N3_PuenteII`, `N3_PuenteII_Horizonte` y `N3_EscenaFinal`. |
| `1d5ce58` | 25/09 | sin rellenar (`<id>`) | **Caja vacía, cuerda y seto.** En la 2.2 la caja pasa a `prop_n2_caja_suelo_vacía`, se desliza sobre los troncos y solo se ladea al caer; `RollingLog` rueda con espejo o sin él; la cuerda (`prop_n2_pieza_2`) es la séptima pieza del taller y amarrar la caja el último paso (INC-54); `MazeLayout.ObstacleScale` 1,65; la 2.5 y `N2_PuenteI_Bosque` pasan a troncos rodantes; se borran `prop_n2_tronco_b..e`. |
| `d3a6cc9` | 25/09 | sin rellenar (`<id>`) | **Hoguera animada del puente II y del final.** Lo registrado en §4 (fila de `N3_PuenteII_Horizonte`) y la nueva versión de `env_enlace_n2` sin la hoguera pintada. |
| `ccf77e6` | 25/09 | `D08-2` | **Sonido del N3.** `N3_Sonidos` (`RiverSounds`) en la orilla y el panel de `Level3_River`; `NarrativeProp.MotionAmbient` para la balsa que cruza en la 3.3; el panel espera al último martillazo y guarda las fases 1–2 al aprobarse; `PlayFraming` de la recolección de ×2,5 a ×1,9. |

Entre los dos lotes entró `88fe0ee` (24/09, PR #86): la familia y Algoritm animados. No es de
este carril y su registro está en
[`Personajes-Resultados.md`](../Personajes/Personajes-Resultados.md); de él vienen la fogata
pequeña del horizonte y las dos fogatas de `N3_EscenaFinal` (montón y llama `prop_n1_fuego_normal`)
a las que `f801186` les pone humo.

### A.2 Código de juego (`Assets/Game/Scripts/Runtime/`)

#### A.2.1 `Game.Levels.River` — nuevo en el carril

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `River/RiverSounds.cs` | **Creado** (56 líneas, `ccf77e6`) | ScriptableObject `N3_Sonidos` (menú «Algoritm/Sonidos del nivel río»): `RiverAmbient`, `ForestAmbient`, `Collected`, `PiecePlaced`, `PhaseHammer`, `PhaseHits` (3), `PhaseHitSeconds` (0,3 s; también la espera antes de `RaftBuilt`) y `RaftBuilt`. Ninguna pieza de fallo (CP-02, comentado). El cruce no está aquí: es contenido de la 3.3 (`NarrativeProp.MotionAmbient`). |
| `River/RiverSceneController.cs` | Modificado (542 líneas hoy) | Campo `sounds`. En `Start`, `PlayAmbient(RiverAmbient)` y `PlayAmbientLayer(ForestAmbient)`: el río de fondo y el bosque de día en la segunda capa; las escenas del río piden los mismos dos clips, así que de la narrativa a la orilla —y de vuelta tras la 3.2— el fondo no se corta. `Collect` suena `Collected` una vez por material. Sin `AudioManager` se juega en silencio. |
| `River/AssemblyPanelController.cs` | Modificado (784 líneas hoy) | Campo `sounds`. **Un martillazo por pieza puesta**: `PiecePlaced` suena cuando `Release` acepta la pieza en un espacio, también la equivocada, porque colocar no valida (guion §1.8.4); soltarla fuera no suena. **Al aprobar una fase**, `ConfirmedAsync` para el reloj de RF-45 en la confirmación (`_indicators.Complete()`; antes lo hacía `Persist` tras el pulso) y guarda las fases 1 y 2 en ese momento, antes de la animación, para que «Reiniciar» a mitad de los golpes no pierda lo aprobado ni cuente el paso dos veces. `PulseAndHammerAsync` sustituye al `Tween` de 0,35 s: el pulso (`CompletionPulseSeconds` 0,35) y `PhaseHits` golpes separados `PhaseHitSeconds` van en el mismo reloj, y el panel sigue ocupado (`IsBusy`) hasta el último. **Con la última fase** espera `PhaseHitSeconds`, suena `RaftBuilt` y solo entonces guarda la fase 3 y sale al cruce: con el nivel terminado en disco, el cruce —su cierre reflexivo— contaría como visto (CP-07). `Persist(phase, indicators)` recibe los indicadores ya cerrados. |
| `River/RiverLevelConfig.cs` | Modificado | `PlayFraming` por defecto pasa de foco (0,2; 0,2) · zoom 2,5 a (0,2632; 0,2632) · 1,9: asoma la orilla a la derecha (decisión de Santiago del 25/09/2026, registrada también en `Slice-3-Resultados.md`). |
| `River/Game.Levels.River.asmdef` | Modificado | Referencia `Game.Audio`. Sigue sin `Unity.InputSystem`. |

#### A.2.2 `Game.Scaffolding`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Scaffolding/RollingLog.cs` | **Creado** (`b38c00b`; 243 líneas hoy) | `MaskableGraphic` de uGUI que dibuja el tronco como cilindro en 3/4: una malla calculada como la proyección ortográfica del cilindro, en 48 tiras (`Segments`), sin cámara, modelo ni `MeshRenderer`; solo se pintan las tiras que miran a quien juega. `Attach(face, look)` cuelga un hijo `Tronco` de la imagen del objeto y la deja transparente por alfa, así que sigue recibiendo el clic. **Lee el giro, no lo manda**: `RollMotion` en la narrativa y el cursor en el bosque siguen girando el `RectTransform`; aquí se deshace ese giro y se pasa a la textura. El contorno es la silueta en oscuro con el color del anillo del corte. `1d5ce58`: la textura rueda con el giro que se ve, con espejo o sin él, y gana `internal Spin` para las pruebas. |
| `Scaffolding/RollingLogLook.cs` | **Creado** (`b38c00b`) | ScriptableObject (menú «Algoritm/Tronco que rueda»): `Texture` (corteza desenrollada a la izquierda y el corte en un cuadrado a la derecha), `AxisDegrees` 35, `TiltDegrees` 40, `Depth` 1,9 radios, `OutlineWidth` 0,16. Lo comparten el bosque y las narrativas para que el paso de uno a otras no se note (INC-50). |
| `Scaffolding/NarrativeProp.cs` | Modificado | `Rolling` (`RollingLogLook`, `b38c00b`): el objeto es un tronco en 3/4. `MotionAmbient` (`AudioClip`, `ccf77e6`): el ambiente que ocupa la segunda capa mientras el objeto se mueve y se va al llegar. |
| `Scaffolding/AssemblyInfo.cs` | **Creado** (`1d5ce58`) | `InternalsVisibleTo("Game.Scaffolding.Tests")`, solo EditMode, para leer `RollingLog.Spin`. |

#### A.2.3 `Game.UI`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `UI/NarrativeSceneController.cs` | Modificado (870 líneas hoy) | `b38c00b`: cuelga `RollingLog` de los objetos con `Rolling`. `1d5ce58`: lo que rueda gira como tronco solo si lo es (`roll.Spin`); lo que va encima —la caja de la 2.2— se desliza y solo se ladea al caer (`roll.Tilt`); antes sumaba los dos. `ccf77e6`: al empezar a moverse un objeto con `MotionAmbient`, ese clip entra en la capa de ambiente —el río, en la capa principal, no se toca— y dura lo que dura el movimiento; al llegar vuelve el `AmbientLayer` de la secuencia, o se va por fundido si la secuencia no tiene. Sin clip no toca la capa. |

#### A.2.4 `Game.Levels.Wheel`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Wheel/WheelLevelConfig.cs` | Modificado (`b38c00b`) | `LogLook` (`RollingLogLook`). |
| `Wheel/ForestSceneController.cs` | Modificado | `b38c00b`: los troncos redondos (`RoundLog`) llevan `RollingLog`, en el suelo y en la fila del acopio; el tronco que aparta el cursor **rueda** (`Roll`: gira lo que avanza en horizontal entre su radio). `1d5ce58`: deja de compensar el espejo, que ya resuelve `RollingLog`. |
| `Wheel/WorkshopPiece.cs` · `AssemblyStep.cs` · `AssemblySequence.cs` · `AssemblyContent.cs` · `WorkshopSceneController.cs` | Modificado (`1d5ce58`) | La cuerda, séptima pieza y último paso (INC-54): `Rope = 6` en los dos enums; `IsComplete` exige la cuerda y `IsCargoPlaced` cubre la caja; soltar la cuerda sin caja devuelve `RopeTooEarlyMessage`; la caja puesta muestra `CargoPlacedMessage` y la cuerda, `CompleteMessage`; la cuerda amarrada se queda sobre la caja (`RopePlacedPosition`, `RopePlacedSize`) y ya no se vuelve a tomar. La decisión, en `INCONSISTENCIAS.md` INC-54. |
| `Wheel/MazeLayout.cs` · `MazeSceneController.cs` | Modificado (`1d5ce58`) | `ObstacleScale` (1,65): el arbusto se dibuja más grande que su casilla para que los contiguos se monten y se lean como seto. |

#### A.2.5 Los `AssemblyInfo.cs` al 25/09/2026

Solo tres son del carril (`Game.Audio`, `Game.Core`, `Game.Scaffolding`); los demás se listan para
que el mapa quede entero.

| Módulo | Creado | Abre sus internos a |
|---|---|---|
| `Game.Audio` | `2cbe287` (21/09) | `Game.Audio.Tests`, `Game.Audio.PlayMode.Tests`, `Game.Levels.Fire.PlayMode.Tests` (`2cbe287`); `Game.Levels.Wheel.PlayMode.Tests` y `Game.UI.PlayMode.Tests` (`03675b9`); `Game.Levels.River.PlayMode.Tests` (`ccf77e6`) |
| `Game.Core` | `d7ace67` (23/09) | `Game.Core.Tests`, `Game.Core.PlayMode.Tests` |
| `Game.Scaffolding` | `1d5ce58` (25/09) | `Game.Scaffolding.Tests` |
| `Game.UI` | `eab7c57` (06/09) | `Game.UI.PlayMode.Tests` |
| `Game.Levels.Fire` | `79c9b3d` (10/09) | `Game.Levels.Fire.Tests`, `Game.Levels.Fire.PlayMode.Tests` |
| `Game.Levels.Wheel` | `817a327` (10/09) | `Game.Levels.Wheel.Tests`, `Game.Levels.Wheel.PlayMode.Tests` |
| `Game.Levels.River` | `660fd41` (16/09) | `Game.Levels.River.Tests`, `Game.Levels.River.PlayMode.Tests` |

`Game.Reporting` no tiene: se prueba por su superficie pública.

### A.3 Código de Editor (`Game.EditorTools`)

La segunda tanda no toca el Editor. Se registra aquí el gemelo de `AudioImportRules`, que es
anterior al carril y al 23/09 no figuraba en ningún resultado (hoy lo registra también
[`Slice-3-Resultados.md`](Slice-3-Resultados.md), §A.7):

| Archivo | Estado | Qué hace |
|---|---|---|
| `ArtImportRules.cs` | Existía (`a9236f1`, 16/09/2026) | `AssetPostprocessor`: toda textura bajo `Assets/Game/Art/` entra **sin comprimir** (`TextureImporterCompression.Uncompressed`) y con `maxTextureSize` 4096, para que las panorámicas de 3840 no se reduzcan y la ilustración plana no enseñe la rejilla de bloques de 4×4, que el N1 agrava con la capa de oscuridad (RNF-23). **No toca el `Sprite Mode`**: pasar cada imagen nueva a `Single` sigue siendo trabajo del motor. Lo vigila `ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir` (`EditMode/Architecture/ArtImportTest.cs`). Por ella entró el arte de la segunda tanda: los cuadros del humo, `prop_n2_tronco_textura`, `prop_n2_pieza_2` y `prop_n2_caja_suelo_vacía` están sin comprimir, a 4096 y en `Single`. |

### A.4 Datos (`Assets/Game/Data/`)

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `River/N3_Sonidos.asset` | **Creado** (`ccf77e6`) | Instancia de `RiverSounds`: río `amb_n3_rio_orilla`, bosque `amb_n2_bosque_dia`, recoger `sfx_encaje_pieza`, pieza puesta y fase aprobada `sfx_martillo_madera` (3 golpes cada 0,3 s), balsa terminada `sfx_n1_pieza_tomar`. |
| `River/N3_RiverLevelConfig.asset` | Modificado (`ccf77e6`) | `PlayFraming` (0,2632; 0,2632) · 1,9 (antes (0,2; 0,2) · 2,5). |
| `Wheel/N2_TroncoRodante.asset` | **Creado** (`b38c00b`) | Instancia de `RollingLogLook`: `prop_n2_tronco_textura`, 35°, 40°, 1,9, 0,16. La usan `N2_WheelLevelConfig.LogLook` y los `Rolling` de la 2.1 (5 troncos), la 2.2 (6), la 2.5 (3) y `N2_PuenteI_Bosque` (3). |
| `Wheel/N2_WheelLevelConfig.asset` | Modificado | `b38c00b`: `LogLook` → `N2_TroncoRodante`. `1d5ce58`: `planta_2`/`planta_3` y `herramienta_2`/`herramienta_3` pasan a `_b`/`_c` (antes repetían `_a`); los troncos siguen con uno solo (`prop_n2_tronco_a`). |
| `Wheel/N2_AssemblyContent.asset` | Modificado (`1d5ce58`) | Séptima pieza (`6`, la cuerda) con `prop_n2_pieza_2` en (0,6; 0,3) a 0,1; `RopePlacedPosition` (0,681; 0,33), `RopePlacedSize` 0,05; mensajes «La caja va sobre la tabla.», «La cuerda sujeta la caja. La carretilla está completa.» y «La cuerda todavía no tiene nada que sujetar.». |
| `Wheel/N2_MazeLayout.asset` | Modificado (`1d5ce58`) | `ObstacleScale` 1,65. |
| `Guide/N2_Guia.asset` | Modificado (`1d5ce58`) | La instrucción del taller añade «y amárrala con la cuerda» tras la caja de alimentos, antes de «En ese orden: cada paso necesita que el anterior esté hecho.». |
| `Narrative/N1_NacimientoDelFuego.asset` | Modificado (`f801186`) | Humo que nace (`fx_n1_humo_nacer`) detrás de la llama del cierre. |
| `Narrative/N2_PuenteI.asset` · `N2_Escena25_Cierre.asset` · `N3_PuenteII.asset` | Modificado (`f801186`) | Humo en bucle (`fx_n1_humo`) detrás y por encima de la llama. En la 2.5, además (`1d5ce58`), los troncos borrados pasan a `prop_n2_tronco_a` rodante. `N3_PuenteII` suena como la 2.5 —`amb_n1_cueva_oscura` con la capa `amb_n1_cueva_fuego`— desde `03675b9`. |
| `Narrative/N3_PuenteII_Horizonte.asset` | Modificado | `f801186`: humo sobre la hoguera, entonces pintada, y sobre la fogata pequeña de `88fe0ee`. `d3a6cc9`: lo de §4. Dos humos y dos llamas animadas; mismo ambiente que `N3_PuenteII`. |
| `Narrative/N3_EscenaFinal.asset` | Modificado | `f801186`: humo sobre las dos fogatas de `88fe0ee`. `d3a6cc9`: la hoguera central de §4, con su humo. Tres humos y tres llamas. No tiene ambiente ni ningún sonido: `Ambient` y `AmbientLayer` vacíos. |
| `Narrative/N2_Escena21_Bosque.asset` · `N2_Escena22_ElPatron.asset` | Modificado | `b38c00b`: los troncos llevan `Rolling`. `1d5ce58`: en la 2.2 la caja pasa a `prop_n2_caja_suelo_vacía`, más pequeña y apoyada sobre la fila; sigue sonando `sfx_n2_piedra_cae` al caer. |
| `Narrative/N2_PuenteI_Bosque.asset` | Modificado (`1d5ce58`) | Sus tres troncos (`_b`, `_c`, `_d`, borrados) pasan a `prop_n2_tronco_a` rodante con giros distintos. |
| `Narrative/N2_Escena23_Construccion.asset` | Modificado (`1d5ce58`) | La línea del orden de armado añade «luego amárrala con la cuerda». |
| `Narrative/N3_PuenteII_Rio.asset` · `N3_Escena31_Llegada.asset` · `N3_Escena32_PrimerIntento.asset` · `N3_Escena33_Cruce.asset` | Modificado (`ccf77e6`) | Pasan de no tener ambiente a `amb_n3_rio_orilla` con la capa `amb_n2_bosque_dia`, los mismos dos clips que `N3_Sonidos`. En la 3.3 la balsa (`PropMotion.Drift`, 9 s) lleva `MotionAmbient` = `amb_balsa_movimiento`. |

### A.5 Escenas (`Assets/Game/Scenes/`)

| Escena | Qué cambió |
|---|---|
| `Level3_River.unity` | `RiverSceneController.sounds` y `AssemblyPanelController.sounds` apuntan a `N3_Sonidos` (`ccf77e6`). |

Ninguna otra escena cambió en la segunda tanda: lo demás entra por assets (`LogLook`, las secuencias).

### A.6 Arte (`Assets/Game/Art/`)

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Props/Fire/Animations/fx_n1_humo.anim` + `.controller` | **Creado** (`f801186`) | 7,27 s a 30 fps, en bucle; 26 dibujos (estado `Subir`). |
| `Props/Fire/Animations/fx_n1_humo_nacer.anim` + `.controller` | **Creado** (`f801186`) | 1,97 s sin bucle, 7 dibujos; el controlador pasa de `Nacer` a `Subir` (`fx_n1_humo`) al terminar. |
| `Props/Fire/Animations/Smoke/` (33 PNG) | **Creado** (`f801186`) | Los 33 dibujos, de 1123×1933, en `Single`, sin mipmaps; conservan el nombre de entrega (`humo_nivel_1_0009.png` … `_0275.png`). Según el mensaje del commit, la entrega eran 284 cuadros a 30 fps: fuera los duplicados y el cuadro vacío `0000`, y todos recortados al mismo rectángulo. |
| `Props/Wheel/prop_n2_tronco_textura.png` | **Creado** (`b38c00b`; nueva versión en `1d5ce58`) | 768×256: la corteza desenrollada y el corte, la única textura del tronco rodante. |
| `Props/Wheel/prop_n2_tronco_a.png` | Modificado (`b38c00b`, `1d5ce58`) | Médula descentrada; es el único tronco que queda. |
| `Props/Wheel/prop_n2_tronco_b.png` … `_e.png` | **Borrado** (`1d5ce58`) | Sustituidos por `prop_n2_tronco_a` rodante. |
| `Props/Wheel/prop_n2_pieza_2.png` | **Creado** (`1d5ce58`) | La cuerda del taller (INC-54), 2000×2000. |
| `Props/Wheel/prop_n2_caja_suelo_vacía.png` | **Creado** (`1d5ce58`) | La caja vacía de la 2.2, 256×256. El nombre lleva tilde, contra `Direccion_de_Arte.md` §15.4. |
| `Props/Wheel/prop_n2_caja_suelo.png` · `_herramienta_{a,b,c}` · `_laberinto_obstaculo` · `_pieza_{1,3,4}` · `_planta_{a,b,c}` | Modificado (`1d5ce58`) | Nueva versión del dibujo, mismo nombre y GUID. |
| `Environments/Narrative/env_enlace_n2.png` | Modificado (`d3a6cc9`) | Nueva versión sin la hoguera central pintada (854 508 → 777 295 bytes), que pasa a objetos (§4). |

### A.7 Audio (`Assets/Game/Audio/`)

Ningún `.wav` nuevo. Los dos renombrados del N3 ya constan en §7 (notas del 25/09). El Nivel 3
suena con dos piezas propias y cuatro prestadas —tres globales y una del Nivel 2—; todas las
referencia `N3_Sonidos` salvo `amb_balsa_movimiento`, que es de la 3.3:

| Pieza | Dónde suena en el Nivel 3 |
|---|---|
| `Level 3/amb_n3_rio_orilla.wav` | Fondo de la orilla, del ensamblaje y de las cuatro escenas del río |
| `Level 2/amb_n2_bosque_dia.wav` | Segunda capa sobre el río, en los mismos sitios |
| `Level 3/amb_balsa_movimiento.wav` | Segunda capa mientras la balsa cruza en la 3.3 |
| `Global/sfx_encaje_pieza.wav` | Cada material recogido |
| `Global/sfx_martillo_madera.wav` | Una vez por pieza puesta; tres veces al aprobar una fase |
| `Global/sfx_n1_pieza_tomar.wav` | La balsa terminada, antes de salir al cruce |

Lo que se aparta de la dirección de sonido (§8, §13) está en su §19, rev. 4.

### A.8 Pruebas (`Assets/Tests/`)

| Archivo | Estado | Casos nuevos o cambiados |
|---|---|---|
| `EditMode/Scaffolding/RollingLogTests.cs` | **Creado** (`b38c00b`) | 3 casos: `RollingLog_RF26_DeFrenteSoloSeVeElCorteYDeCostadoSoloLaCorteza`, `RollingLog_RF26_En34ElCorteSeAchataYElCostadoSeAlejaPorElEje` y, desde `1d5ce58`, `RollingLog_RF26_EnEspejoLaTexturaGiraComoSeVeGirarElObjeto` |
| `EditMode/Scaffolding/NarrativeSequenceTests.cs` | Modificado (`f801186`) | `NarrativeSequence_RF05_CadaLlamaEchaHumoPorDetrasYPorEncima`: toda llama animada (`prop_n1_fuego_normal`) tiene antes en la lista —detrás— un humo (`fx_n1_humo*`) más alto que ella y a menos de un cuarto de su tamaño en horizontal |
| `EditMode/Levels/Wheel/AssemblySequenceTests.cs` | Modificado (`1d5ce58`) | `AssemblySequence_INC54_LaCuerdaAmarraLaCajaYCompletaLaCarretilla` |
| `EditMode/Levels/Wheel/PatternSelectionTests.cs` | Modificado (`1d5ce58`) | `WheelLevelConfig_RF23_CadaCategoriaDelBosqueSeDibujaConUnSoloSprite` → `WheelLevelConfig_RF23_LosTroncosRedondosSeDibujanConUnSoloSprite` |
| `EditMode/Levels/River/RiverSoundsAssetTests.cs` | **Creado** (`ccf77e6`) | 2 casos: `RiverSounds_RF05_LasEscenasDelRioSuenanComoLaOrilla` (las cuatro escenas del río piden el mismo río y el mismo bosque que `N3_Sonidos`) y `RiverSounds_RF44_CadaMomentoDelNivelSuenaConSuPieza` (pieza por pieza, y `amb_balsa_movimiento` en la balsa de la 3.3) |
| `EditMode/Levels/River/RiverLevelConfigTests.cs` | Modificado (`ccf77e6`) | `RiverLevelConfig_Guion82_ElPlanoDeRecoleccionMuestraSoloElBosqueSinElRio` → `RiverLevelConfig_Guion82_ElPlanoDeRecoleccionEsElBosqueConUnPocoDelRio`, rehecha: el borde derecho del recorte pasa de 0,46 (asoma la orilla) y no de 0,6 (sigue siendo el bosque) |
| `PlayMode/Levels/River/RiverSoundsTests.cs` | **Creado** (`ccf77e6`) | 6 casos: `RiverScene_RF35_LaOrillaSuenaAlRioConElBosqueDeFondo`, `RiverScene_RF37_RecogerUnMaterialSuenaAEncajeUnaVez`, `AssemblyPanel_RF40_ColocarUnaPiezaSuenaUnMartillazoYSoltarlaFueraNada`, `AssemblyPanel_RF41_AprobarUnaFaseSuenaTresMartillazosAntesDeSeguir`, `AssemblyPanel_CP02_UnaFaseQueNoPasaNoSuena`, `AssemblyPanel_RF44_LaBalsaTerminadaSuenaAPiezaTomadaTrasLosMartillazos` |
| `PlayMode/Levels/River/AssemblyPanelTests.cs` · `Game.Levels.River.PlayMode.Tests.asmdef` | Modificado (`ccf77e6`) | `WalkTo` pasa de `private` a `internal` para compartirlo con `RiverSoundsTests`; el asmdef referencia `Game.Audio` |
| `PlayMode/UI/NarrativeSceneTests.cs` | Modificado | `f801186`: `NarrativeScene_RF20_ElCierreDelFuegoPintaElMontonConLaLlamaAnimada` exige también `fx_n1_humo_nacer`. `1d5ce58`: `NarrativeScene_RF23_LaEscena22AnimaLoQueCadaLineaCuenta` comprueba que la caja se desliza sobre los troncos sin girar. `ccf77e6`: nuevo `NarrativeScene_RF44_LaBalsaSuenaMientrasCruzaYAlLlegarVuelveElBosque` |
| `PlayMode/Levels/Wheel/WorkshopSceneTests.cs` · `WheelLevelJourneyTests.cs` · `ForestSceneTests.cs` | Modificado (`1d5ce58`) | La cuerda entra en los recorridos del taller (`WorkshopScene_RF29_…`, `WorkshopScene_DA133_TodaLaFamiliaCelebraLaCarretillaTerminadaHastaSalir`); `WorkshopScene_RNF02_ElMapaDeControlesSoloTieneClicYClicSostenido` cuenta cinco objetos de clic sostenido, con `Pieza_Rope`; `WheelLevel_RNF13_RecorreElNivel2CompletoHastaElMenuConNivel3Desbloqueado` espera `StepsUsed` 6 en la fase 2; `ForestScene_RNF23_CadaObjetoMuestraLaIlustracionDeSuAsset` exige un solo sprite solo a los troncos |

### A.9 Documentación

| Archivo | Commit | Qué se hizo |
|---|---|---|
| `claudeDocs/Direccion_de_Musica_y_Sonido.md` | `ccf77e6` | §19, rev. 4: lo aplicado en el Nivel 3 |
| `claudeDocs/INCONSISTENCIAS.md` | `1d5ce58` | Abre INC-54 (la cuerda del taller) |
| `Assets/Game/Art/Inventario.md` | `f801186` | El humo: clips, carpeta `Smoke/` y nombres de entrega |
| `claudeDocs/tasks/Slice 3/Slice-3-Resultados.md` · `todo.md` | `ccf77e6` | El plano ×1,9 de la recolección |
| Este documento | `d3a6cc9`, `ccf77e6` | Las notas fechadas del 25/09 en §2.1, §4, §7 y §10 |
| `CLAUDE.md` | los cinco | `b38c00b`: puesta al día tras los PR #85 y #86 (`Game.Reporting` creado y referenciando solo `Game.Core`, doce escenas con `TeacherReport`, puntero a `Personajes-Resultados.md`, `Game.Content.Tests`). `f801186`: troncos rodantes, humo en la regla de nombres de entrega, actas hasta D09 y dueño del sonido. `1d5ce58`: INC-54. `d3a6cc9`: `AssemblyInfo` de `Game.Scaffolding`. `ccf77e6`: sonido del N3 |

### A.10 Lo que queda abierto

- **Tres commits sin tarjeta**: `b38c00b`, `1d5ce58` y `d3a6cc9` llevan `(tarjeta: <id>)` sin
  rellenar (CT-11, RNF-17). El primero corresponde por su texto a `D09-1`.
- **La rama no está fusionada**: `main` sigue en `131b4c0`.
- **Memoria del humo** (RNF-05), por cálculo y no por medida: 33 texturas RGBA de 1123×1933 sin
  comprimir ni mipmaps son ≈ 8,3 MB cada una; el bucle (26 dibujos) ≈ 215 MB y el nacimiento
  suma ≈ 58 MB. La medida pendiente es la de `Personajes-Resultados.md` (2191 MB en un Editor
  abierto horas), que hay que repetir en batchmode o con el ejecutable.
- `prop_n2_caja_suelo_vacía.png` lleva tilde (§15.4 pide nombres sin tildes); renombrarlo es
  trabajo del motor (`AssetDatabase.RenameAsset`) para conservar el GUID que usa la 2.2.
- Igual que el fuego, los cuadros del humo conservan el nombre de entrega
  (`humo_nivel_1_NNNN.png`); renombrarlos es trabajo del motor.
- Las piezas propias del Nivel 3 (§13 de la dirección de sonido) no se han entregado: las
  globales hacen de ellas, y `amb_balsa_movimiento` no lleva el `n3` de §4.1. Lo que sigue sin
  referenciar ya está en §10.

## Anexo B — props definitivos del N2 y el N3 (25/09/2026, sin commit)

Llegaron los props definitivos de los niveles 2 y 3. Este anexo registra cómo entraron: el N2
bajó de resolución y el N3, que cambió de estilo (del plano frontal al 3/4 de
`prop_n3_balsa_cruzando`), obligó a recomponer el armado de la balsa.

**Resolución del N2, por lo que se ve en pantalla.** La regla es 256×256; se sube a 512×512 solo
donde el sprite se enseña a más de 256 px a 1080p, medido sobre los encuadres de los assets
(`Size` × 1080 × zoom de la parada más cercana en la que el objeto está en cuadro).

| Archivo | Antes | Ahora | Por qué |
|---|---|---|---|
| `prop_n2_carretilla_e1` | 2000 | 256 | Rueda perforada, en la casilla de la pieza (~130 px) |
| `prop_n2_carretilla_e2` … `_e5` | 760 / 2000 | 512 | El cierre del taller acerca el conjunto a ~505 px y la 2.4 enseña `e5` a ~400; los cuatro con el mismo lienzo para sustituirse en el sitio |
| `prop_n2_pieza_3`, `_4` | 2000 | 512 | La 2.3 los enseña a ~360 y ~340 px |
| `prop_n2_pieza_1`, `_5` | 320 | 256 | ~250 px en la 2.3 |
| `prop_n2_pieza_2`, `prop_n2_laberinto_carretilla`, `prop_n2_piedra_a` … `_d` | 2000 | 256 | Por debajo de 256 px en todos sus usos |

Reducción con Lanczos sobre alfa premultiplicado, para que el borde no se oscurezca. Las
piedras están en modo Multiple: su recorte se escaló con la textura desde el motor
(`ISpriteEditorDataProvider`), conservando el `spriteID` que referencian los assets.

**El taller cambió de etapas.** El arte nuevo corre la numeración un puesto: `e1` es ahora la
rueda perforada (antes lo era `pieza_1`, que pasa a tronco sin perforar) y `e5` trae la cuerda.
`N2_AssemblyContent` pasa a `DrilledWheelArt` = `e1`, `AxleArt` = `e2`, `PlankArt` = `e3` y
`CompleteArt` = `e4`; con el mapeo anterior el tronco perforado habría enseñado el eje ya
montado. La cuerda sigue siendo la pieza que se cuelga sobre la caja (INC-54) y su posición no
cambia: cae sobre la caja de `e4`. `e5` queda para las narrativas.

**La balsa del N3.** Los 17 espacios de `N3_RaftAssemblyContent` se recompusieron sobre la forma
de `prop_n3_balsa_cruzando`: cinco troncos en diagonal, de atrás (arriba a la izquierda) a
delante; dos lianas por tronco, delante y detrás; el mástil plantado en el tronco del centro y la
vela a su derecha. Ningún espacio se gira (el mástil tenía 45°). En el asset cada tronco va
seguido de sus dos amarres, porque el orden del asset es el orden de dibujo y así el tronco de
delante tapa las puntas de las lianas del de atrás. `mastil` y `mastil_silueta` bajaron de 2000
a 512 (~390 px en pantalla) y `amarre_silueta` a 128, el tamaño de `amarre`;
`prop_n3_tronco_silueta` había vuelto en Multiple y se pasó a Single, que es lo que referencia el
asset.

**Soltar y agarrar sobre lo que se ve** (`AssemblyPanelController`). Con troncos diagonales las
cajas cuadradas se solapan y, desde la punta de un tronco, el centro del vecino queda más cerca
que el suyo: medido sobre la composición, un 34 % de lo visible de cada tronco elegía otro
espacio. Ahora los espacios prueban el alfa (`alphaHitTestMinimumThreshold` 0,1), así que el
raycast del agarre ya no entrega el clic al vecino, y al soltar gana el de encima entre los que
tienen dibujo bajo el puntero; si no hay ninguno, el de centro más cercano, como antes, para que
soltar junto a una liana siga contando. Error medido: 0 % en las tres fases. Por eso las ocho
texturas de la balsa son legibles (Read/Write), unos 1,3 MB más.

**Verificación.** Corridas con el Editor abierto (Rider caído) el 25/09/2026. EditMode: 360/361
(la omitida es `ProfileEraser_INC34_…`, la de siempre). PlayMode de `Game.Levels.River` y
`Game.Levels.Wheel`: 133/134, incluida la nueva
`AssemblyPanel_RF40_SoltarSobreLaPuntaDeUnTroncoLoPoneEnEseTroncoYNoEnElVecino`. Falla
`RiverLevel_RNF05_LaMemoriaQuedaBajoDosGigasConElNivel3Cargado` (2941 MB), la misma falla que
registra `Slice-4-Resultados.md` («Corrida completa de la suite»): el Editor en `Boot` y sin el
nivel ya reservaba 2976 MB. Las
capturas de `AssemblyPanel_HU12_*` y `Workshop_*` se revisaron a ojo.

**Queda abierto.**

- `prop_n3_balsa_cruzando` tiene cuatro troncos y la mecánica arma cinco (RF-40, fijado por
  `RaftValidatorTests`).
- `prop_n3_balsa_hundida` (3.2) y `prop_n3_troncos` (3.1) siguen en el estilo plano de antes.
- A la escala del ensamblaje (×2,2) el tronco y la vela, de 256, se ven ampliados ~1,4×.
- En el taller la cuerda es la pieza colgada sobre `e4`, no el dibujo de `e5`. Usar `e5` como
  etapa de la cuerda es un campo más en `AssemblyContent` y cambiar
  `WorkshopScene_…` (la prueba que hoy exige la cuerda colgada).

*(01/10/2026: este anexo entró con `44fd479` (25/09/2026, 23:43), con la tarjeta sin rellenar, y llegó
a `main` con el PR #87 (`995b26d`, 29/09). De «Queda abierto»: la cuerda del taller pasa a `e5`
(INC-121) y la ampliación del ensamblaje desaparece con el plano a ×1,6 (INC-118); la balsa hundida
se acepta como definitiva y la balsa de cuatro troncos se acepta como está, pendiente de la entrega de
la colaboradora (acta D10, §5); la 3.1 deja de usar `prop_n3_troncos`. Ver Anexo C.)*

---

## Anexo C — lo que cambió después del 25/09/2026 (01/10/2026)

Lo que siguió en el carril de arte y sonido hasta el 01/10/2026: un commit del propio carril, el
arte que tocó el cierre de inconsistencias (`37b3cb7`) y las correcciones del acta D10 en la rama
`feat/cierre-de-slices-y-oe3`, que entran en el commit de cierre con las tarjetas D10-1, D10-2 y
D10-3. La verificación del arte por programa (doble indicador, contraste y destellos), la limpieza de
halos, los renombres de nomenclatura y el crédito de los glifos de la pausa son la tarjeta D10-4 y no
están aquí.

### C.1 Qué cambió y dónde

| Fecha | Commit | Qué cambió |
|---|---|---|
| 25/09 | `44fd479` | El Anexo B: props del N2 a 256 y 512 px, la balsa del N3 en 3/4 — C.2 |
| 29/09 | `995b26d` (PR #87) | La rama del carril se fusiona en `main` — C.2 |
| 30/09 | `37b3cb7` | Arte real en los menús, sin rótulos de trabajo (INC-82); estela de Algoritm (INC-52) y sombra de contacto (INC-109); humo y chispa del N1 (INC-47, INC-68) — C.3 |
| 01/10 | sin hash, D10-1 y D10-2 | La chispa del N1 como un rayo (INC-119) y el humo hasta la corona; `e5` en el taller (INC-121) y la caja vacía sin uso (INC-120) — C.4 |
| 01/10 | sin hash, D10-3 | La salpicadura renombrada y cableada (INC-123), la escena final con ambiente (INC-125) y la 3.1 con el tronco y el mástil en 3/4 (INC-118) — C.5 |

### C.2 `44fd479` y la fusión

El Anexo B dejó de estar sin commit: es `44fd479`, que lleva `(tarjeta: <id>)` sin rellenar, como
`b38c00b`, `1d5ce58` y `d3a6cc9`. El PR #87 (`995b26d`, 29/09/2026) llevó a `main` los cinco commits
del Anexo A y `44fd479`; desde el PR #88 (`1c7f4ab`, 30/09) la rama y `main` vuelven a coincidir.
Vence el segundo punto de A.10.

### C.3 `37b3cb7` (30/09/2026): el arte que tocó el cierre de inconsistencias

- **Menús** (INC-82): inicio, créditos y menú de niveles usan arte real en lugar de los rótulos
  «… · placeholder», y la tarjeta del Nivel 2 del menú usa una ilustración 16:9 sin la costura de las
  panorámicas de 3840. Ningún texto de escena usa la fuente integrada del motor ni Fredoka (INC-79).
  Pruebas: `Scenes_RNF01_NingunTextoDeEscenaEsUnRotuloDeTrabajo`,
  `LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura` y
  `Scenes_CN04_NingunTextoUsaLaFuenteIntegradaDelMotor`.
- **Personajes**: la estela de puntos de Algoritm y la sombra de contacto de la familia entran como
  hijos de los prefabs, sin reconstruirlos; el detalle está en `Personajes-Resultados.md`, Anexo B.
- **Nivel 1**: el montón humea al converger con `fx_n1_humo_nacer`, y la chispa es una cruz de dos
  `Image` `#FFE9A8` dibujada por el motor, sin sprite (`Fase-5-6-Resultados.md`, Anexo, A.3).
- **Créditos**: nombran a los autores y la obra de origen de la Familia Anonaky (INC-78), y siguen
  dando por originales del proyecto los entornos, los objetos y la interfaz.
- `Assets/Game/Art/Inventario.md` se puso al día en el mismo commit.

### C.4 Nivel 1 y Nivel 2 (D10-1 y D10-2, 01/10/2026)

- **La chispa del N1 es un solo rayo** (INC-119): un trazo `#FFE9A8` de 4 u, sin sprite, que el motor
  dibuja con `RayoH` estirado sobre `Chispa`; `RayoV` se borra de la escena. Contradice las «cuatro
  líneas radiales» de `Direccion_de_Arte.md` §12.2 y la fila de `FX/` de `Inventario.md`, que INC-119
  corrige.
- **El humo del N1** (`Humo`, los clips `fx_n1_humo_nacer` y `fx_n1_humo`) nace en el punto del golpe
  y, al prender, sube a la corona de la llama encogiéndose a 0,6, el factor «detrás de cada llama» que
  ya usa `Inventario.md`. Los 33 cuadros conservan su nombre de entrega.
- **`prop_n2_carretilla_e5`** se usa ahora también en el taller (`AssemblyContent.TiedArt`, INC-121):
  la carretilla amarrada es el mismo dibujo con que abre la 2.4. Cierra el último punto del Anexo B.
- **`prop_n2_caja_suelo_vacía`** queda sin referencias: la 2.2 vuelve a pintar `prop_n2_caja_suelo`
  (INC-120).

### C.5 Nivel 3 (D10-3, 01/10/2026)

- **La salpicadura suena** (INC-123). `Level 3/sfx_n3_salpicadura_undimiento.wav` se renombró desde el
  motor, con ensayo en seco antes, a `sfx_n3_hundimiento.wav`: mismo GUID
  (`be2ba44e5cfd935448ca5a20ff9f2cb8`), `.meta` idéntico y los mismos 306 434 bytes. Es la pieza
  `sfx_n3_hundimiento` de §13 de la dirección de sonido, de 1,74 s. `RiverSounds.RaftSinking`, en
  `N3_Sonidos`, la hace sonar al empezar todo hundimiento: el de «Probar balsa» antes de la última fase
  y el de la última fase. «Listo» rechazado y la zona sin materiales siguen mudos (§2.1, CP-02).
  Pruebas: `RiverSounds_RF42_LaBalsaQueSeHundeSuenaASalpicadura` (EditMode) y
  `AssemblyPanel_RF42_LaBalsaQueSeHundeSuenaUnaSalpicaduraYNadaMas` (PlayMode); con el nombre nuevo
  siguen en verde `AudioAssets_CP02_NingunaPiezaSeLlamaDerrotaNiError` y
  `AudioImport_RNF06_CadaFamiliaEntraConLosAjustesDeSuTabla`. Vencen la fila de §7, el punto de §10 y
  A.7.
- **La escena final suena** (INC-125): `N3_EscenaFinal` pasa a `amb_n2_bosque_dia` con
  `amb_n1_cueva_fuego` en la segunda capa, el par de `N2_PuenteI`. Prueba:
  `RiverSounds_RF44_LaEscenaFinalSuenaAlBosqueConLasFogatas`. Vence la fila de `N3_EscenaFinal` de A.4.
- **El arte del N3 a la escala de las narrativas** (INC-118). La 3.1 pinta `prop_n3_tronco` en 3/4 y
  `prop_n3_mastil` (0,07 del alto, girado −50°) en lugar de `prop_n3_troncos`, que queda sin uso. Con
  el ensamblaje a ×1,6 la casilla de un tronco mide 256 px y el sprite se ve 1:1. `PlayFraming` de la
  recolección pasa a (0.3572, 0.3572) ×1.4: vencen las filas de `RiverLevelConfig` de A.2.1 y de A.4.

### C.6 Lo que queda abierto al 01/10/2026

- **Cuatro commits sin tarjeta**: `b38c00b`, `1d5ce58`, `d3a6cc9` y `44fd479`.
- **Memoria** (RNF-05): la prueba del Editor mide la sesión y no el nivel —2 127 MB dentro de la suite
  del 01/10, 1 660 MB aislada con el Editor recién abierto—; la medida que vale es la del ejecutable.
- **`prop_n2_caja_suelo_vacía`** sigue con tilde (§15.4), ahora sin uso; los renombres de nomenclatura
  son de la tarjeta D10-4.
- **Los cuadros del fuego y del humo** conservan su nombre de entrega, como fija §15.4.
- **Sonido**: `sfx_n3_probar_balsa` (§13) sigue sin entregar; `amb_balsa_movimiento` no lleva el `n3`
  de §4.1; `amb_n2_noche_intemperie` sigue en disco sin referenciar, y faltan el ambiente nocturno del
  Nivel 2, la música y el sonido del diálogo (D07-2, que sigue, acta D10 §6). El silencio S3 de la
  escena final pide código en `AudioManager`.
- **`NarrativeScene_RNF01_LaLineaMasLargaCabeEnSuCuadroDeDialogo`** pasa con el Editor abierto a
  1920 × 1080 (suite del 01/10); en batchmode es una de las pruebas de disposición que fallan por
  entorno.
- **Arte del N3**: `prop_n3_balsa_cruzando` tiene cuatro troncos y la mecánica arma cinco; se acepta
  como está y queda pendiente de la entrega de la colaboradora del 07/10/2026.

*(01/10/2026, después: `prop_n2_caja_suelo_vacía` ya se llama `prop_n2_caja_suelo_vacia` (INC-126);
lo registra el Anexo D, con el resto de la tarjeta D10-4.)*

---

## Anexo D — la verificación del arte y el peso de los cuadros del fuego (01/10/2026)

Apartado nuevo, con lo que C.1 dejó fuera por ser de la tarjeta D10-4 —los renombres, la limpieza de
halos, el crédito de los glifos de Phosphor y la verificación del arte por programa— y la excepción
de importación de los cuadros del fuego y del humo (INC-130), que pidió el ejecutable candidato.
Todo está en la rama `feat/cierre-de-slices-y-oe3`, sin commit.

### D.1 Qué cambió y dónde

| Qué | Hallazgo | Archivos |
|---|---|---|
| Cinco nombres según la nomenclatura, y una prueba que la vigila — D.2 | INC-126 | los cinco PNG con su `.meta`, el comentario de `MazeLayout.cs` y `ArtImportTest.cs` |
| El halo de croma de siete PNG de personajes | — | tres Algoritm en reposo y los cuatro retratos; detalle en `Personajes/Personajes-Resultados.md`, B.6 |
| Los glifos del menú de pausa, acreditados — D.3 | INC-127 | `CreditsContent.asset`, `CreditsContent.cs` y `Art/UI/Common/LICENSE-Phosphor.txt` |
| Diez PNG del Nivel 2 se quedan en `Multiple` — D.4 | INC-128 | ninguno: solo documentos |
| Nombres, alfa, halo, doble indicador, contraste y destellos, medidos por programa — D.5 | — | `claudeDocs/tasks/OE4/evidencias/arte/` |
| Los cuadros del fuego y del humo, a 1024 px — D.6 | INC-130 | `ArtImportRules.cs`, `ArtImportTest.cs` y los 67 `.meta` de `Props/Fire/Animations/` |

### D.2 Nomenclatura (INC-126)

- **Los cinco nombres** que el informe del 30/09 marcó entre 96 PNG se renombraron desde el motor
  (`AssetDatabase.RenameAsset`, primero en simulación): `entorno_n1_apertura`, `entorno_n1_cueva_2x`,
  `entorno_n1_cueva_cenital` y `entorno_n2_laberinto` pasan a `env_n1_apertura`, `env_n1_cueva_2x`,
  `env_n1_cueva_cenital` y `env_n2_laberinto`, y `prop_n2_caja_suelo_vacía` a
  `prop_n2_caja_suelo_vacia`. Los `.meta` nuevos son idénticos byte a byte a los de antes, GUID
  incluido, y el contenido es el mismo objeto LFS.
- **Nada se rompe.** Escenas y assets citan los entornos por GUID y con el `fileID` 21300000 de un
  sprite `Single` (2, 5, 2 y 2 referencias); la caja vacía no la usa nadie (INC-120). La única cita
  por nombre en el código era un comentario de `MazeLayout.cs`, ya corregido.
- **La prueba.** `ArtImport_RNF23_LosNombresSiguenLaNomenclatura` (`Game.Architecture.Tests`) recorre
  los PNG de `Art/` y exige prefijo, minúsculas y nada de tildes ni espacios. Salió en rojo con
  exactamente esos cinco nombres. Su única excepción, comentada, son los nombres de entrega de los
  cuadros —`fuego_*_nivel_1_####` y `humo_nivel_1_####`—, que referencia la curva del `.anim` (§15.4).
- **En git** cada renombre aparece como un archivo borrado y otro nuevo: hay que añadir las rutas
  viejas y las nuevas, de `.png` y de `.meta`, para que el commit lo registre como renombre.

### D.3 Los glifos del menú de pausa, acreditados (INC-127)

`ui_pausa`, `ui_reanudar` y `ui_reiniciar` son los iconos `pause`, `play` y `arrow-counter-clockwise`
de Phosphor Icons rasterizados a 128 px, y los créditos daban por original toda la interfaz. El
cuerpo de `CreditsContent.asset`, y el valor por defecto de `CreditsContent.cs`, terminan ahora en
dos oraciones:

> «Entornos, objetos e interfaz: originales del proyecto, salvo los iconos de pausa.»
>
> «Iconos de pausa: Phosphor Icons, licencia MIT.»

Tienen 12 y 7 palabras, dentro del límite de 20 (`CreditsContent_RNF01_NingunaOracionSupera20Palabras`).
Una primera redacción —«Iconos de la interfaz: Phosphor Icons…»— se rechazó en la revisión, porque
atribuía a Phosphor todos los iconos y `ui_alerta`, `ui_lock`, `ui_papelera`, `ui_circulo` y
`ui_flecha` son del proyecto, como los cuatro indicadores del informe docente. El aviso MIT
(«Copyright (c) 2023 Phosphor Icons») está en `Art/UI/Common/LICENSE-Phosphor.txt` y viaja con el
ejecutable en `Licencias/`, junto al de la OFL de las tipografías (`Slice 1/Fase-5-6-Resultados.md`,
A.8). En `Credits.unity` el texto ocupa una línea más y solo alarga el desplazamiento: no tapa ni
recorta nada. Siguen en verde `CreditsTests` (4/4, PlayMode) y las tres `Content_*`.

### D.4 La excepción de `Multiple` (INC-128)

`CLAUDE.md` y `Direccion_de_Arte.md` §15.2 piden cada imagen en `Single`, pasada desde el motor
—con `Multiple`, `LoadAssetAtPath<Sprite>` devuelve nulo—, pero doce PNG del Nivel 2
llegaron en `Multiple` con un solo sprite recortado (`<nombre>_0`): `env_enlace_n2`,
`prop_n2_herramienta_a` a `_c`, `prop_n2_piedra_a` a `_d`, `prop_n2_planta_a` a `_c` y
`prop_n2_tronco_a`. Diez los referencian escenas y assets por ese sub-sprite, y pasarlos a `Single`
cambiaría su `fileID` a 21300000 y rompería esas referencias: se quedan como están, como excepción
documentada. `prop_n2_piedra_c` y `_d` tampoco se migraron y no los usa nadie. Son los doce `.meta`
«desviados» del informe de sprites.

### D.5 La verificación por programa

`claudeDocs/tasks/OE4/herramientas/arte_check.py` midió el arte entero; los informes, cada uno con una
sección «Lectura» escrita a mano y con sus archivos de entrada al lado para repetirlos, están en
`claudeDocs/tasks/OE4/evidencias/arte/`.

| Informe | Resultado |
|---|---|
| `sprites.md` | 96 PNG: 0 nombres fuera de la nomenclatura, 0 sin transparencia, 0 halos intensos; 27 halos tenues, informativos (partes de los rigs); 12 `.meta` desviados, la excepción de D.4 |
| `rnf19.md` | Doble indicador: 48 parejas, 45 bien y 3 marcadas: una caja de medida desfasada (B8) y dos diferencias de color pequeñas que lleva el icono (C8 y A9) |
| `rnf19-w3.md` | Con las cajas vueltas a apuntar tras mover la balsa y sortear el laberinto: B8 y C8 bien; la pareja vacío/incorrecto de C8 (Δ 24,8) la distingue el icono |
| `rnf20.md` · `rnf20-extra.md` · `rnf20-w3.md` | Contraste: 10/10 (≥ 6,2:1) y 18/18; el «!» de la alerta, 5,6:1 |
| `rnf21.md` | Destellos: el fuego normal, el cenital y el humo, como mucho un destello por segundo |

Ningún sprite falla en nombre, alfa o halo intenso.

### D.6 Los cuadros del fuego y del humo, a 1024 px (INC-130)

El primer build del ejecutable candidato pesó 866,7 MB frente al límite de 500 MB (RNF-06): los 67
cuadros de `Props/Fire/Animations/` —33 de humo, 13 de fuego normal y 21 de fuego cenital— sumaban
533 MB sin comprimir y a tamaño completo. Ya eran un dibujo por clave, sin duplicados, así que la
convención de los `.anim` no daba más. `ArtImportRules` hace una única excepción: esa carpeta se
importa con `maxTextureSize` 1024, **sin comprimir**, como todo `Art/`. El fuego normal queda en
1024 × 1007, el humo en 595 × 1024 y el cenital, de 500 × 278, no cambia; el tamaño en pantalla
tampoco, porque los píxeles por unidad se escalan con la textura. Los cuadros conservan su nombre de
entrega. Pruebas: `ArtImport_RNF06_LosCuadrosDelFuegoYElHumoSeImportanAMil24SinComprimir` (nueva, en
rojo antes del cambio) y `ArtImport_RNF23_LasIlustracionesEntranSinComprimirYSinReducir`, que admite
esa carpeta y solo esa. Los cuadros pasan a 145,7 MB y el paquete a 479,0 MB, sin pérdida visible en
las capturas ampliadas. Detalle en `Slice 1/Fase-5-6-Resultados.md`, A.8.

### D.7 Pruebas y lo que queda

- **Cifras.** Tras D.2 a D.4, EditMode 429 = 428 + 1 omitida; la verificación final antes del build,
  EditMode 433 = 432 + 1 omitida y PlayMode 364/364; tras D.6, EditMode 434 = 433 + 1 omitida
  (`Slice 4/Slice-4-Resultados.md`, «Verificación final y paquete (01/10/2026)»).
- **Las motas verdes opacas** en las puntas del pelo de los retratos y de Algoritm (B.6 de
  `Personajes-Resultados.md`) piden limpieza a mano del carril de arte.
- **El comentario de `ArtImportRules`** habla de «134 texturas»; son 67 PNG, y 134 es el número de
  entradas del informe del build. Se corrige después de la pasada del OE4, porque cambiar el archivo
  cambia la huella del ejecutable candidato.
- **Siguen** los puntos de C.6, salvo el de la tilde.
