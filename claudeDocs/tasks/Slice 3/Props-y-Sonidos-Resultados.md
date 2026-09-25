# Carril de arte y sonido — lo construido, archivo por archivo

Documento de resultados de la rama `feat/implementación-de-props-y-sonidos`, hermano de
[`Slice-3-Resultados.md`](Slice-3-Resultados.md). Vive en la carpeta del Slice 3 porque la rama
nació justo después de su cierre (`76d7375`, PR #80), pero **no es un slice**: es el carril de
arte y sonido (tarjetas `D05-2` sonido y `D07-2` sprites/sonido de las actas OE3), que toca
`Assets/Game/Art/`, `Assets/Game/Audio/`, `Game.Audio`, `Game.Levels.Fire`, `Game.Levels.Wheel` y,
de paso, `Game.Core`, `Game.Scaffolding` y `Game.UI`. No rediscute `claudeDocs/SPEC.md`.

| Campo | Dato |
|---|---|
| **Rama** | `feat/implementación-de-props-y-sonidos`, desde `76d7375` (`main`, 21/09/2026) |
| **Fusión** | PR #82 → `d5fa77f` (`main` y la rama apuntan hoy al mismo commit) |
| **Fechas** | 21/09/2026 – 23/09/2026 |
| **Commits** | 6: `2cbe287`, `dc51804`, `aca7acc`, `8ec5120`, `03675b9`, `d7ace67` |
| **Volumen** | 253 archivos, +11 491 / −400 líneas — 161 añadidos, 72 modificados, 18 renombrados, 2 borrados |
| **De ellos** | 68 son los 34 cuadros del fuego con su `.meta`; 60 son `.meta` de assets nuevos o renombrados |
| **Verificación registrada** | Solo la de `8ec5120`: EditMode `NarrativeSequenceTests` 9/9 y `Architecture` en verde; PlayMode 60/65 (4 capturas saltadas en batchmode, `NarrativeScene_RNF01` falla por 1 px en `N1_Hallazgo`, ajeno al cambio). **Los commits del Nivel 2 (`03675b9`, `d7ace67`) no dejan cifra de suite completa**, y este documento no la inventa. |

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

---

## 2. Código de juego (`Assets/Game/Scripts/Runtime/`)

### 2.1 `Game.Audio`

| Archivo | Estado | Qué se hizo |
|---|---|---|
| `Audio/AudioManager.cs` | **Creado** (345 líneas) | Tercer singleton `DontDestroyOnLoad`. Cuatro buses con volumen (`MusicVolume` 0,8 · `AmbientVolume` 0,4 · `SfxVolume` 1 · `VoiceVolume` 0,6). API: `PlayAmbient`/`PlayAmbientLayer` (fundido cruzado; pedir el clip que ya suena no lo reinicia), `PlayMusic`, `PlaySfx(clip, pitchJitter, volume)`, `PlayHeld`/`StopHeld` (bucle mientras dura un clic sostenido), `PlayVoice`, `CutToSilence(keepAmbient)` (silencios del guion §5, corte seco). Expone en `internal` `LastSfx`, `SfxCount`, `HeldClip`, `AmbientClip`, `AmbientLayerClip`, `AmbientStarts` para las pruebas. |
| `Audio/AssemblyInfo.cs` | **Creado** | `InternalsVisibleTo` para `Game.Audio.PlayMode.Tests` y para `Game.Levels.{Fire,Wheel}.PlayMode.Tests`, que comprueban qué clip sonó. |

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
| `Level3_River.unity` | Retirado su `AudioListener`. |
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
| `Environments/Narrative/env_enlace_n2.png` | **Creado** | Entorno del puente II hacia el horizonte; reutiliza el `.meta` (GUID) de `civilización_noche`. |
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
| `Level 3/Rio.wav` | `Level 3/amb_n3_rio_orilla.wav` | Renombrado; sin referenciar |
| `Level 3/viento_rio.wav` | `Level 3/amb_viento_horizonte.wav` | Renombrado; sin referenciar |
| `Level 3/Salpicadura_agua.wav` | `Level 3/sfx_n3_salpicadura.wav` | Renombrado; sin referenciar |

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
  PlayMode con `unity test` (Editor cerrado) para dar cifra de la rama entera.
- `NarrativeScene_RNF01` falla por 1 px en `N1_Hallazgo` (anotado en `8ec5120`, sin corregir).
- Las piezas del N3 y `amb_n2_noche_intemperie` están en disco pero **no las referencia ningún
  asset**; no hay música ni blip de diálogo (`PS-01..PS-05`).
- Los cuadros del fuego conservan el nombre de entrega y no el `prop_n1_…` de
  `Direccion_de_Arte.md`; renombrarlos es trabajo del motor.
- Revisar los dos cambios de configuración que hizo el Editor (§9).
