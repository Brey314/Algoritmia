# Personajes animados — resultados (24/09/2026)

Carril de arte, igual que `Slice 3/Props-y-Sonidos-Resultados.md`: toca `Assets/Game/Art/Characters/`,
`Assets/Game/Prefabs/Characters/`, `Game.Scaffolding`, `Game.UI`, los tres `Game.Levels.*`, las
dieciocho narrativas y las cinco escenas jugables. Lo pidió Santiago: la familia y Algoritm, con
animaciones, en **todo** el juego; objetos donde el guion los nombra; retrato en el cuadro de
diálogo, y el botón de ayuda con la forma de Algoritm, listo para sustituir el sprite.

> **Estado al 25/09/2026:** todo lo de este documento entró con `88fe0ee` (24/09/2026), que el
> PR #86 fusionó en `main` (`131b4c0`). Desde entonces nadie ha tocado los cinco scripts del rig
> (`CharacterRig`, `ActorAction`, `ActorBeat`, `ActorCue`, `ActorTimeline`), los siete prefabs ni
> el arte de `Art/Characters/`. Alrededor sí cambiaron dos cosas:
> - Desde `f801186` (24/09/2026) el humo tiene sprite animado, así que en ese punto la lista de
>   objetos sin sprite de «Pendiente» está vencida.
> - Las fogatas del N3 que puso esta tarea echan humo, y desde `d3a6cc9` (25/09/2026) tienen al
>   lado la hoguera central.
>
> El documento no traía inventario de archivos ni pruebas: los dos están en el anexo del final. La
> cifra de la suite completa está en `Slice 4/Slice-4-Resultados.md`, en «Corrida completa de la
> suite (25/09/2026)».
>
> **01/10/2026:** después del 25/09 el carril sí cambió. `37b3cb7` (30/09) añadió la estela de
> Algoritm y la sombra de contacto de la familia sin reconstruir los prefabs; el 01/10, en el cruce de
> la 3.3, quien termina sus pasos ya no salta aunque el texto avance, y en la mecánica del río la
> familia y Mamá toman la escala de las narrativas. Lo registra el
> [Anexo B](#anexo-b--lo-que-cambió-después-del-25092026-01102026); la suite vigente es la del
> 01/10/2026, en el mismo `Slice-4-Resultados.md`.

## Qué entró

**Arte** (`Inventario.md`, sección `Characters/`). Viene de los sprites base entregados
(`…/assets a postproduccion/familia/`): cinco partes por miembro de la familia y un retrato
`neutra`. Algoritm tiene tres formas; la rueda y la gota son **provisionales** con nombre
definitivo (INC-52).

**Rig por recorte** (INC-53). `CharacterRig` (`Game.Scaffolding`) está en un prefab por personaje,
con un `Animator` de un estado por `ActorAction` y 21 clips en la familia (9 en Algoritm). La lámina
de poses se revisó a mano. No hay saltar, caer ni derrota: tras un fallo, `Encourage`.

**Narrativas.** Un personaje es un `NarrativeProp` con `Actor` (el prefab) y `Beats`, los pasos
por línea. Así hereda la casilla, el orden de dibujo, el paneo y la prueba del cuadro de diálogo.
`ActorTimeline` (C# plano) decide qué hace cada uno en cada línea:
- **Entre pasos mantiene lo último.** Quien está arrodillado sigue arrodillado.
- **Quien dice la línea de pie gesticula.** Si su paso es `Idle`, también.
- **Quien camina termina su camino aunque el texto avance.** Solo un paso nuevo lo interrumpe, y
  entonces salta a su destino antes de empezarlo. *(01/10/2026: salvo quien termina sus pasos
  —`NarrativeProp.FinishesSteps`, hoy solo los cuatro viajeros de la 3.3—, a quien ningún paso nuevo
  interrumpe: lo que se lee mientras camina lo hace al llegar. Ver Anexo B, B.3.)*

El retrato del cuadro de diálogo sale del personaje en escena que dice la línea. Si solo se oye su
voz, del reparto (`cast`), y en el caso de Algoritm, con la forma del nivel (`guideByLevel`). En
las acotaciones no hay retrato.

**Contenido.** Las 18 secuencias tienen personajes, pasos y objetos nuevos. Los objetos salen de
sprites que ya existían, y en la 2.2 y en el N3 se movió algún objeto existente. Cada entrada
lleva la cita del guion que la justifica. El contenido se escribió, se verificó con una réplica de
la prueba y con láminas por parada de cámara, y se aplicó desde el editor.

**Mecánicas.**

| Escena | Quién | Qué hace |
|---|---|---|
| `Level1_Cave` | Papá junto al círculo; la familia atrás | Papá recoge al tomar una pieza, golpea, se anima tras un golpe sin chispa, sopla y se queda arrodillado. El Niño, inquieto, observa |
| `Level2_Forest` | La Niña junto a la caja; la familia al pie de los árboles | Señala el tronco aceptado, se anima tras un distractor, empuja mientras sostiene la caja. Todos celebran el acopio |
| `Level2_Workshop` | La familia detrás del banco | La Niña señala y martilla; Papá martilla con ella en el montaje; todos celebran la carretilla |
| `Level2_Maze` | La Niña junto a la salida; la familia en el refugio | La Niña observa, señala al ejecutar, se anima si no llega. Al llegar celebran todos. Sin tinte de atardecer (§4.2, §5.4) |
| `Level3_River` | Mamá (rig dentro de `Personaje_Mama`); la familia junto a la zona | Camina mientras se sostiene una flecha, volteándose a izquierda y derecha. Recoge, se anima si falta algo o la balsa se hunde; celebran cada fase aprobada |

En las cinco, el botón de ayuda es el círculo del mockup con Algoritm del nivel dentro
(`Fondo/Algoritm`): fuego en el N1, rueda en el N2 y gota en el N3. El del bosque dejó de ser el
rectángulo «Ayuda», y el del laberinto perdió su «?».

*(01/10/2026: en `Level3_River` la familia y Mamá se ven ya a la escala de las narrativas del río,
unas 2,3 veces más grandes; Mamá se ancla por los pies, la familia espera detrás de la zona, y Mamá,
la familia y los materiales se dibujan por profundidad. Ver Anexo B, B.4.)*

## Revisión visual (24/09/2026)

Se revisaron las 138 capturas reales del motor, una por línea, con la prueba
`NarrativeScene_RF05_CapturaCadaLineaConLosPersonajes` (en `persistentDataPath/TestScreenshots`),
más las cinco mecánicas (`Personajes_DA133_CapturaCadaMecanicaConSusPersonajes`). Lo que se
corrigió:
- **Clips.** Arrodillarse y dormir bajan casi al suelo con las rodillas abiertas; con acortar las
  piernas parecía estar de pie. Abrazar abre los brazos, porque cerrarlos los escondía tras el
  torso. Celebrar no pasa de ~100°, porque con la cabeza grande de Mamá y los niños las manos
  quedaban detrás. Señalar va casi horizontal, porque en diagonal parecía saludar al cielo.
  Observar lleva las manos a la cintura y el cuerpo inclinado, para distinguirse del reposo.
- **Puesta en escena.** La Niña golpea las piedras junto a ellas en la 1.2; antes, lejos. En la
  2.2 deja de estar de pie sobre la fila de troncos. Papá deja de pisar el martillo en la 2.1 y
  de flotar sobre las rocas en la 2.5. Los materiales de la 3.1 apoyan en el suelo. Algoritm deja
  de quedar pegado a la mano de Papá o entre dos cabezas. Se quitaron cortes en el borde de
  personajes que hablan, y en el Puente I se añadieron alimentos a los pies de quien recoge.

## Cómo regenerar

`herramientas/` guarda lo que produjo el arte y los rigs. Las rutas de trabajo apuntan a la carpeta
temporal de la sesión: hay que ajustarlas antes de usarlas.
- `cut.py`: corta cada personaje en partes, con los polígonos de hombro y la paleta de piernas de
  cada uno, limpia el croma verde y escribe un `manifest.json` con cajas y pivotes.
- `forms.py`: las tres formas de Algoritm.
- `preview.py`: una lámina de poses en Python para revisar los cortes.
- `BuildRigs.cs.txt`: script de editor. Sin argumento reconstruye prefabs, clips y controladores;
  con `"clips"` solo reescribe las curvas de los clips existentes. **Reconstruir los prefabs cambia
  los fileID de sus componentes y rompe las referencias** de `Narrative.unity`, de los 18 assets y
  de las cinco escenas. Para retocar animaciones, usar `"clips"`.

## Pendiente

- INC-52 e INC-53: corregir `Direccion_de_Arte.md` §7.6/§13.1 e `Interfaces.md` §4.3, o el arte.
  *(Vencido el 29/09/2026: INC-52 e INC-53 se cerraron corrigiendo §7.6, §13.1, `Interfaces.md` §4.3
  y el guion §1.1.1, que describen ya el guía entregado —una llama con extremidades, de fuego,
  madera y agua— y la animación por recorte con `CharacterRig`.)*
- Arte de rueda y gota: sustituir los dos `.png` conservando nombre y `.meta`.
- Expresiones del retrato distintas de `neutra`: ninguna línea las pide todavía.
- Objetos que el guion nombra y no tienen sprite: comida, humo, estela de Algoritm, maleza, piedras
  en la mano. Tampoco puede aparecer un objeto a mitad de escena (el montón de la 1.2).
  *(Vencido el 25/09/2026 en cuanto al humo: desde `f801186` (24/09/2026) es un sprite animado.
  `fx_n1_humo_nacer` está en `N1_NacimientoDelFuego` y `fx_n1_humo` en otras cinco narrativas; ver
  el anexo, A.2. El resto de la lista sigue igual.)*
- El pulso de la pista tras tres fallos (§12.3, §14.2): hoy solo pulsa, y siempre, el del N1.
- **3.3, línea 0:** la balsa se desliza 9 s y al final sale por el borde derecho del encuadre de
  apertura, con la familia encima. El encuadre es el diseño de cámara del N3; el arreglo es moverlo
  (`CameraStart` hacia 0,47, como la parada de L1) o acortar el deslizamiento. Lo decide Santiago.
- **3.2:** la balsa ya se ve hundida desde L0 («la empujan al agua y suben»). Hace falta que un
  objeto aparezca, desaparezca o cambie de sprite en una línea: el motor solo lo permite a los
  personajes (`Hidden`/`Appear`/`Vanish`).
- **Señalar tiene una sola dirección:** horizontal hacia la derecha de pantalla, o hacia la
  izquierda con `Mirrored`. Para apuntar al suelo o al cielo haría falta un ángulo por paso.
- **RNF-05** (memoria < 2 GB): la prueba `RiverLevel_RNF05_…` dio 2191 MB en un Editor que llevaba
  horas abierto; la línea base del 21/09 en batchmode fue 1226 MB. Todo el arte de personajes
  suma 16,3 MB. Hay que repetir la medida en batchmode (`unity test`) o con el ejecutable.
- Vistas cenitales (N1 y laberinto) con figuras frontales: decisión de dirección de arte.

*(01/10/2026: la estela de Algoritm existe desde `37b3cb7` (30/09), y no como sprite: la dibuja el
motor con puntos que se apagan. La 3.3, línea 0, sigue igual: el 01/10 la balsa del cruce bajó, pero
ni su deslizamiento ni `CameraStart` cambiaron. La memoria se mide sobre el ejecutable: dentro de la
suite del 01/10 el Editor reservaba 2 127 MB por la sesión, y aislado, recién abierto, 1 660 MB. Ver
Anexo B, B.5.)*

## Anexo — lo que cambió después del cierre (25/09/2026)

Este anexo sale del diff de `88fe0ee` y del árbol en `ccf77e6`. Después del 24/09 no hubo trabajo
nuevo de personajes: lo que falta arriba es el registro, no la obra.

### A.1 Archivos de `88fe0ee`

Es un solo commit: 333 archivos, +66 460 / −447 líneas.

**Código de juego** (`Assets/Game/Scripts/Runtime/`, líneas añadidas entre paréntesis)

| Archivo | Estado | Qué es |
|---|---|---|
| `Scaffolding/CharacterRig.cs` | **Creado** (194) | El rig: un `MonoBehaviour` con el `Animator` y el lienzo `Stage`, de 1024 × 1024 (`CanvasUnits`), que se escala a la casilla. `Play` pasa a una acción con un fundido de 0,18 s. Si el controlador no tiene ese estado cae a `Idle`, y pedir la acción que ya hace no la reinicia. `PlayFor` la sostiene unos segundos en tiempo escalado (la pausa la congela) y vuelve sola. `Speaks` compara el hablante con `SpeakerNames` sin distinguir mayúsculas. `Portrait` es el retrato del diálogo, y `Mirrored` voltea el lienzo, no la raíz. `Tint` existe, pero hoy no lo llama ningún script. |
| `Scaffolding/ActorAction.cs` | **Creado** (82) | El enum de 22 acciones (0–21). Cada una es un estado del `Animator` con el mismo nombre. Los valores son explícitos porque los assets guardan el número. No hay saltar, caer ni derrota. |
| `Scaffolding/ActorBeat.cs` | **Creado** (62) | Un paso serializable: `Line`, `Action`, `Moves`, `Destination` (en fracciones de la ilustración), `Seconds` (2 s por defecto) y `Arrival`. |
| `Scaffolding/ActorCue.cs` | **Creado** (38) | `readonly struct` con la indicación ya resuelta para una línea: `From`, `To`, `Moves`, `Seconds`, `During` y `After`. La escena la ejecuta sin decidir nada. |
| `Scaffolding/ActorTimeline.cs` | **Creado** (121) | Clase estática en C# plano: `Cue(prop, line, speaking)`, `PositionBefore`/`PositionAfter`, `WalkUnderway` (el camino que sigue en curso) y `BeatAt`. Aquí viven las tres reglas de «Narrativas». |
| `Scaffolding/NarrativeProp.cs` | Modificado (+20) | `Actor` (el prefab), `ActorStart` (`Idle`, o `Hidden` para Algoritm antes de aparecer), `Beats` y `WithActor(…)`. |
| `UI/NarrativeSceneController.cs` | Modificado (+181) | Coloca y mueve los personajes (`PlaceActor`, `PlayActors`, `WalkAsync`). Pinta el retrato (`ShowPortrait`, `PortraitOf`) a partir de los personajes en escena, `cast` y `guideByLevel`. Expone `Actors`, `Portrait` y `PortraitFrame` como `internal`. |
| `Levels/Fire/FirePanelController.cs` | Modificado (+79) | `player` (Papá), `family` y `restless` (el Niño, que observa en vez de reposar). |
| `Levels/Wheel/ForestSceneController.cs` | Modificado (+66) | `player` (la Niña) y `family`. |
| `Levels/Wheel/MazeSceneController.cs` | Modificado (+55) | `player` (la Niña) y `family` (en el refugio). |
| `Levels/Wheel/WorkshopSceneController.cs` | Modificado (+83) | `player` (la Niña), `helper` (Papá) y `onlookers` (Mamá y el Niño). |
| `Levels/River/RiverSceneController.cs` | Modificado (+76 / −1) | `playerRig` (Mamá, el rig hijo de `Personaje_Mama`) y `family`. |
| `Levels/River/AssemblyPanelController.cs` | Modificado (+10 / −2) | Recibe un `Action<ActorAction>`. El panel solo dice cuándo celebrar y cuándo animar; los personajes son de la escena. |

**Prefabs** (`Assets/Game/Prefabs/Characters/`): siete, `Papa`, `Mama`, `Nina`, `Nino`,
`Algoritm_Fuego`, `Algoritm_Rueda` y `Algoritm_Gota`. La advertencia sobre los fileID está en
«Cómo regenerar».

**Arte y clips** (`Assets/Game/Art/Characters/{Father,Mother,Girl,Boy,Algoritm}/`)

| Qué | Cuántos | Detalle |
|---|---|---|
| Partes y retrato de la familia | 24 `.png` | `char_<x>_parte_{torso,brazo_izq,brazo_der,pierna_izq,pierna_der}` y `char_<x>_retrato_neutra`, para `papa`, `mama`, `nina` y `nino` |
| Formas de Algoritm | 3 `.png` | `char_algoritm_n1_fuego_reposo`, `…_n2_rueda_reposo` y `…_n3_gota_reposo`. La rueda y la gota son provisionales (INC-52) |
| Clips de la familia | 84 `.anim` | 21 por miembro (`char_<x>_anim_*`): abrazar, animo, apagado, aparicion, arrodillarse, caminar, cargar, celebrar, correr, dormir, empujar, golpear, hablar, idle, martillar, observar, oculto, recoger, senalar, soplar y sorpresa |
| Clips de Algoritm | 9 `.anim` | animo, apagado, aparicion, celebrar, flotar, girar, hablar, oculto y senalar |
| Controladores | 5 `.controller` | `char_papa`, `char_mama`, `char_nina`, `char_nino` y `char_algoritm`, este último compartido por los tres prefabs de Algoritm |

`Mother/` guarda además `char_mama_cenital.png`, que no es de esta tarea (`3894426`, 17/09/2026).

**Contenido y escenas.** Se tocaron los 18 `N*_*.asset` de `Assets/Game/Data/Narrative/`. Hoy
todos llevan al menos un personaje; `N3_PuenteII_Horizonte` lleva solo a `Algoritm_Rueda`.
También se tocaron las cinco escenas jugables y `Narrative.unity`, donde quedan cableados
`portraitFrame`, `cast` y `guideByLevel`.

**Documentación y herramientas.**
- `CLAUDE.md`.
- `claudeDocs/INCONSISTENCIAS.md`: INC-52 e INC-53, las dos abiertas.
- `Assets/Game/Art/Inventario.md`, sección `Characters/`.
- Este documento.
- `herramientas/`: `BuildRigs.cs.txt` (536 líneas), `cut.py` (270), `forms.py` (34) y
  `preview.py` (65).
- `Game.Scaffolding.Tests.asmdef` gana la referencia a `UnityEngine.UI`.

### A.2 El fuego de las narrativas: fogatas y humo

**Fogatas.** «Contenido» no las nombra. `88fe0ee` puso tres fogatas pequeñas en el N3, cada una
con un montón (`prop_n1_monton_hojas`) y una llama animada (`prop_n1_fuego_normal.controller`)
encima:
- `N3_PuenteII_Horizonte`: una, con el montón en (0,625; 0,418) y la llama en (0,625; 0,4444).
- `N3_EscenaFinal`: dos, con los montones en (0,575; 0,46) y (0,67; 0,40) y las llamas en
  (0,575; 0,4864) y (0,67; 0,4528).

Después entraron dos cambios del carril de arte, que se detallan en
`Slice 3/Props-y-Sonidos-Resultados.md`:
- `f801186` (24/09/2026) puso humo detrás de cada llama.
- `d3a6cc9` (25/09/2026) añadió la hoguera central en las dos escenas.

**Humo.** Corrige el «humo» de «Pendiente». `f801186` entró cinco horas después de `88fe0ee` con
dos clips de `Assets/Game/Art/Props/Fire/Animations/`:
- `fx_n1_humo_nacer`, en `N1_NacimientoDelFuego`;
- `fx_n1_humo`, en bucle, en `N2_PuenteI`, `N2_Escena25_Cierre`, `N3_PuenteII`,
  `N3_PuenteII_Horizonte` y `N3_EscenaFinal`.

Lo vigila `NarrativeSequence_RF05_CadaLlamaEchaHumoPorDetrasYPorEncima`.

### A.3 Verificación: las pruebas de los personajes

`88fe0ee` añadió 50 métodos de prueba (+1308 / −7 en `Assets/Tests`), que suman 106 casos:
- EditMode: 14 métodos y 32 casos.
- PlayMode: 36 métodos y 74 casos.

Recuento por `[Test]`, `[UnityTest]`, `[TestCase]` y `[Values]` en el árbol de `ccf77e6`. Los
cincuenta nombres siguen existiendo. En los nombres, `DA133` y `DA76` citan
`Direccion_de_Arte.md` §13.3 y §7.6.

| Clase (`Assets/Tests/`) | Qué añadió `88fe0ee` | Casos hoy en la clase |
|---|---|---|
| `EditMode/Scaffolding/ActorTimelineTests.cs` | **Creada**: 9 `ActorTimeline_RF05_*`. Cubren las tres reglas de «Narrativas»: mantener lo último entre pasos, gesticular al decir la línea de pie y terminar el camino aunque el texto avance | 9 |
| `EditMode/Scaffolding/CharacterRigTests.cs` | **Creada**: 5 métodos, tres de ellos con `[Values]` sobre los siete prefabs (3 × 7 + 2 = 23 casos). Son `CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion` (la familia, todas salvo `Spin`; Algoritm, sus nueve), `…_RNF02_NingunaParteDeUnPersonajeRecibeClics`, `…_CP02_NingunClipEsDeDerrotaCaidaNiSalto`, `…_RF05_CadaPersonajeTieneRetratoYNombresConQueHabla` y `…_RF05_LosDosNinosHablanComoNinos` | 23 |
| `EditMode/Scaffolding/NarrativeSequenceTests.cs` | Modificada: `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo` comprueba ahora a cada personaje en todos los sitios por los que pasa entre dos paradas de cámara, salvo en las líneas en que no se ve | 11 |
| `PlayMode/Levels/Fire/FirePanelTests.cs` | 7: `FireLevel_DA133_*` ×5, `FireLevel_CP02_TrasUnGolpeSinChispaPapaSeAnimaYNuncaHaceUnGestoDeDerrota` y `FirePanel_DA76_LaPistaMuestraAAlgoritmConSuFormaDeFuego` | 34 (33 métodos) |
| `PlayMode/Levels/Wheel/ForestSceneTests.cs` | 7: `ForestScene_DA133_*` ×4, `…_RNF02_LosPersonajesNoLeQuitanElClicANingunObjeto`, `…_RNF03_LosPersonajesSeVenEnterosYNoQuedanBajoLaInterfaz` y `…_DA76_ElBotonDeAyudaMuestraAAlgoritmConSuFormaDeRueda` | 37 |
| `PlayMode/Levels/Wheel/MazeSceneTests.cs` | 5: `MazeScene_DA133_*` ×4 y `…_DA76_ElBotonDePistaMuestraAAlgoritmEnFormaDeRueda` | 26 |
| `PlayMode/Levels/Wheel/WorkshopSceneTests.cs` | 6: `WorkshopScene_DA133_*` ×3, `…_CP02_TrasUnPasoFueraDeOrdenLaNinaAnimaYNuncaHaceOtroGesto`, `…_RNF03_LaFamiliaNoTapaPiezasCarretillaNiInterfazYNoRecibeClics` y `…_INC45_ElBotonDePistaMuestraAAlgoritmConSuFormaDeRueda` | 18 |
| `PlayMode/Levels/River/AssemblyPanelTests.cs` | 2: `AssemblyPanel_DA133_AlAprobarUnaFaseMamaYLaFamiliaCelebran` y `…_CuandoLaBalsaSeHundeLaFamiliaAnimaYNingunoHaceOtroGesto` | 8 |
| `PlayMode/Levels/River/BuildZoneTests.cs` | 2: `RiverScene_DA133_MamaRecogeAlPulsarRecogerYVuelveSolaAlReposo` y `BuildZone_DA133_EntrarSinTodoLosMaterialesDaAnimoYNingunGestoDeDerrota` | 5 |
| `PlayMode/Levels/River/RiverMovementTests.cs` | 2: `RiverScene_DA133_MamaCaminaMientrasSeSostieneUnaFlechaYReposaAlSoltarla` y `RiverScene_DA76_ElBotonDeAyudaMuestraAAlgoritmEnSuFormaDeGota` | 8 |
| `PlayMode/UI/NarrativeSceneTests.cs` | 4 métodos. `NarrativeScene_RF05_ElRetratoEsElDeQuienHablaYNoHayEnLasAcotaciones` y `…_RF05_QuienCaminaTerminaSuCaminoAunqueElTextoAvance` son de un caso cada una. `…_RF05_CadaPersonajeHaceLoQueDiceSuPasoCuandoSeLeeLaLinea` y `…_RF05_CapturaCadaLineaConLosPersonajes` tienen 18 casos cada una, uno por secuencia (38 casos en total) | 75 (29 métodos) |
| `PlayMode/UI/CharacterCaptureTests.cs` | **Creada**: `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes`, un caso por escena jugable (5) | 5 |
| `PlayMode/UI/LevelSummaryTests.cs` · `PlayMode/UI/PauseMenuTests.cs` | Ninguna prueba nueva. Su ayudante `ReunirAsync` arrastra ahora solo lo que implementa `IEndDragHandler`; antes la prueba quedaba sin concluir desde el 22/09/2026 | 3 · 4 |

**Las dos pruebas de captura** (`…_CapturaCadaLineaConLosPersonajes` y
`Personajes_DA133_CapturaCadaMecanicaConSusPersonajes`) llevan `[Category("VisualVerification")]`.
En batchmode (`unity test`) salen antes de `ScreenCapture`, así que solo dejan imagen fuera de
batchmode, con Game View.

**`CharacterProbeMotionTests`** (`EditMode/EditorTools/`, 5 pruebas: `CharacterProbeMotion_DA131_*`
×4 y `…_RNF03_NuncaSaleDeLaPantallaYSeDevuelveEnElBorde`) no es de este carril. Entró con
`eb941c8` (10/09/2026, W05/W06-R) y prueba la maqueta `Sandbox/CharacterProbe`, de
`Game.EditorTools` y solo Editor. Esa maqueta ensayaba la animación por fotogramas frente al rig
de §13.1, así que no es código del juego y no toca `CharacterRig`.

**Suite completa.** Este documento no registra una corrida propia. La del 25/09/2026 está en
`Slice 4/Slice-4-Resultados.md`, en «Corrida completa de la suite (25/09/2026)».

---

## Anexo B — lo que cambió después del 25/09/2026 (01/10/2026)

El Anexo A dice que después del 24/09 no hubo trabajo nuevo de personajes. Lo hubo en dos tandas: el
cierre de inconsistencias (`37b3cb7`, 30/09, en `main` con el PR #88) y las correcciones del Nivel 3
del acta D10, en la rama `feat/cierre-de-slices-y-oe3`, que entran en el commit de cierre con la
tarjeta D10-3.

### B.1 Qué cambió y dónde

| Fecha | Commit | Qué cambió |
|---|---|---|
| 30/09 | `37b3cb7` | Estela de Algoritm (INC-52) y sombra de contacto de la familia (INC-109), añadidas a los prefabs sin reconstruirlos; `CharacterRig.Tint` borrado — B.2 |
| 01/10 | sin hash, D10-3 | En el cruce de la 3.3, quien termina sus pasos no salta aunque el texto avance — B.3 |
| 01/10 | sin hash, D10-3 | En `Level3_River`, la familia y Mamá a la escala de las narrativas (INC-118) — B.4 |

### B.2 `37b3cb7` (30/09/2026): estela y sombra

- **La estela de Algoritm** (INC-52; guion §1.1.1, `Direccion_de_Arte.md` §7.6). `GuideTrail`
  (`Game.Scaffolding`, nuevo) cuelga del rig de las tres formas como hijo `Estela`, hermano de
  `Lienzo`. Muestrea la posición de su padre —el `RectTransform` que mueven la escena o la mecánica;
  `Lienzo` y `Cuerpo` no se mueven dentro de él— y coloca de cinco a siete puntos (`Image`) con escala
  y alfa decrecientes hacia atrás. Si el cuerpo se queda quieto la estela se olvida (`TrailPath`, C#
  plano). Ningún punto recibe clics (RNF-02) y se apagan solo por alfa, sin parpadeo (RNF-21).
- **La sombra de contacto** (INC-109): un hijo `Sombra` bajo los pies de Papá, Mamá, la Niña y el
  Niño. Algoritm no lleva.
- **Sin reconstruir los prefabs**: se añadieron hijos y los fileID de los componentes que ya existían
  —los que referencian `Narrative.unity`, los 18 assets y las cinco escenas— no cambian («Cómo
  regenerar»).
- **`CharacterRig.Tint` se borró**: no tenía llamadores y contradecía el diseño (A.1).
- **Pruebas** (EditMode): `CharacterRig_DA76_LasTresFormasDeAlgoritmLlevanEstelaSinRecibirClics`,
  `CharacterRig_DA53_LaFamiliaLlevaSombraDeContactoBajoLosPies`,
  `CharacterRig_DA53_AlgoritmNoLlevaSombraDeContacto`,
  `GuideTrail_DA76_DejaEntreCincoYSietePuntosDeTamanoDecreciente`,
  `GuideTrail_DA76_SinMovimientoNoDejaEstela`,
  `GuideTrail_DA76_LaEstelaSigueALaCasillaQueCaminaYSeBorraAlDetenerse` y
  `TrailPath_DA76_OlvidaLaEstelaSiElCuerpoSeQuedaQuieto`. En el mismo commit, las acotaciones del
  Nivel 1 dejan de llamar «estrella» a Algoritm (`Content_DA76_NingunTextoVisibleDescribeAAlgoritmComoUnaEstrella`).

### B.3 Quien termina sus pasos: el cruce de la 3.3 (01/10/2026)

La tercera regla de «Narrativas» hacía saltar a los viajeros de `N3_Escena33_Cruce` si se pulsaba
«Continuar» durante los 9 s del cruce: la Niña y el Niño en la línea 1, Papá en la 2 y los cuatro en
la 3 y la 4, porque cada paso nuevo cortaba el camino en curso.

- **La marca.** `NarrativeProp.FinishesSteps` señala a quien termina sus pasos. A ese personaje
  ningún paso nuevo le corta el camino (`ActorTimeline.Interrupts`); lo que se lee mientras camina lo
  hace al llegar y, si algo lo movía, baja después desde donde llegó (`ActorTimeline.PendingStep`: el
  último paso con movimiento leído después del que estaba dando). Las dos funciones son C# plano;
  `NarrativeSceneController.PlayActors` pregunta a `Interrupts` y `WalkAsync` da el paso pendiente.
- **El alcance.** Llevan la marca Papá, Mamá, la Niña y el Niño de la 3.3, y nadie más: las otras 17
  narrativas siguen la regla de siempre. Una réplica cuadro a cuadro dio cero saltos en 3 000 ritmos
  de lectura simulados.
- **Lo que se acepta.** Con lectura rápida, los niños celebran y Papá señala al llegar la balsa y no
  al leerse su línea; si las líneas 2 y 3 se leen durante el cruce, Papá va directo a la orilla sin
  señalar. Lo comprueba Santiago jugando.
- **Con la balsa.** Por INC-124 los cuatro bajaron 0,06 con la balsa —posiciones y pasos de las
  líneas 0 y 2— y viajan en su sitio (`NarrativeSequence_RF44_LaFamiliaBajaConLaBalsaYViajaEnSuSitio`).
- **Pruebas.** `ActorTimelineTests` pasa de 9 a 24 casos con ocho métodos nuevos
  (`ActorTimeline_RF05_*` y `ActorTimeline_RNF21_*`), más
  `NarrativeSequence_RF44_SoloLosViajerosDelCruceTerminanSusPasos` y la ampliación de
  `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo` a los sitios por los que pasa
  quien termina sus pasos (EditMode, en verde); en PlayMode,
  `NarrativeScene_RNF21_AvanzarElTextoDuranteElCruceNoHaceSaltarALosViajeros`.

### B.4 La familia del río a la escala de las narrativas (INC-118, 01/10/2026)

Decisión de Santiago del 30/09/2026 (lectura B, acta D10 §5). En la mecánica, la familia se veía a
0,4 de la escala con que la muestran las narrativas del mismo río.

| Casilla en `Level3_River` | Hasta el 30/09 | Desde el 01/10 | En pantalla, a 1080p |
|---|---|---|---|
| Papá | 77,7 | 178,75 | 148 → 250 px |
| Niña y Niño | 46,6 | 107,25 | 89 → 150 px |
| Mamá | 104, pivote centrado | 240, anclada por los pies (0,5; 0,075) | 121–185 → 193–315 px, según su altura |

- **Frente a la 3.1**: Papá, la Niña y el Niño quedan en 0,933 del tamaño con que los pinta la 3.1, y
  Mamá, de pie en la zona, en 0,988.
- **Mamá se ancla por los pies**, como la familia: así el área por la que anda es el pasto que pisa y
  su escala por profundidad se mide donde apoya.
- **La familia espera detrás de la zona de construcción**, y Mamá espera en la zona al retomar el
  ensamblaje —al volver de la 3.2, desde disco o al reiniciar la fase— (`RiverSceneController.ResumeAt`).
- **Orden de dibujo**: Mamá, la familia y los materiales se ordenan por altura cada vez que Mamá se
  coloca; lo que está más abajo se dibuja delante (`DepthOrder`, C# plano).
- **Sin reconstruir prefabs**: cambian el tamaño, el ancla y el pivote de las instancias de la escena;
  los fileID siguen.
- **Pruebas**: `RiverScene_INC118_LosPersonajesDeLaMecanicaTienenLaEscalaDeLaNarrativa`,
  `RiverScene_DA83_MamaSeAnclaPorLosPiesComoLaFamilia` y `DepthOrder_DA83_*` (EditMode);
  `RiverScene_RNF14_AlRetomarElEnsamblajeMamaEsperaEnLaZona` (PlayMode), y la captura
  `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes` de `Level3_River`, que se repite a la escala
  nueva.

### B.5 Lo que queda abierto al 01/10/2026

Frente a «Pendiente»:

- **Resuelto**: la estela de Algoritm (B.2).
- **Siguen**: el arte definitivo de la rueda y la gota; las expresiones del retrato; los objetos sin
  sprite que nombra el guion —comida, maleza, piedras en la mano— y que un objeto no pueda aparecer a
  mitad de escena; el pulso de la pista; la 3.2 con la balsa ya hundida desde L0; «Señalar» con una
  sola dirección, y las vistas cenitales con figuras frontales.
- **La 3.3, línea 0**: la balsa sigue saliendo por el borde derecho del encuadre de apertura; INC-124
  la bajó, pero no tocó su deslizamiento ni `CameraStart`.
- **El mismo salto de B.3 en otras narrativas**: existe en 14 más (68 situaciones; por ejemplo,
  `N1_Apertura`, donde los cuatro caminan 7 s y la línea 1 los corta). Arreglarlo es marcarlos con
  `FinishesSteps`, pero cambia la puesta en escena de escenas ya revisadas: queda recomendado, con el
  sí de Santiago.
- **RNF-05**: se mide sobre el ejecutable. En el Editor, la prueba del Nivel 3 midió 2 127 MB dentro
  de la suite del 01/10 —la sesión, no el nivel— y 1 660 MB aislada con el Editor recién abierto.

### B.6 El halo de croma de siete PNG y lo que entró después (01/10/2026)

Apartado nuevo, de la tarjeta D10-4 (verificación del arte) y de las revisiones del 01/10/2026.

- **El halo, limpiado.** Los tres Algoritm en reposo (`char_algoritm_n1_fuego_reposo`,
  `_n2_rueda_reposo` y `_n3_gota_reposo`) y los cuatro retratos (`char_papa_`, `char_mama_`,
  `char_nina_` y `char_nino_retrato_neutra`) traían un anillo verde de 2–3 px en el borde, resto del
  fondo de croma. `claudeDocs/tasks/OE4/herramientas/arte_check.py halo --apply --max-alpha 200`
  quitó el exceso de verde de los píxeles semitransparentes del borde: 6 008 en cada Algoritm y entre
  977 y 1 785 en cada retrato, todos con alfa entre 1 y 199 y a no más de 3 o 4 px del transparente. El alfa
  queda idéntico; ningún píxel opaco (alfa ≥ 200) ni transparente cambió, y los `.meta` tampoco. El
  informe de sprites (`claudeDocs/tasks/OE4/evidencias/arte/sprites.md`) da 0 halos intensos. Los
  retratos en el cuadro de diálogo de `N1_Hallazgo` y Algoritm en el botón de ayuda y en las
  narrativas se revisaron en captura: sin halo visible.
- **Queda: las motas verde oscuro opacas** en las puntas del pelo, que el umbral de 200 deja fuera a
  propósito. A 1080p se ven unas doce en cada retrato de la Niña y de Papá, y quedan también en Algoritm.
  Quitarlas con la herramienta tocaría píxeles opacos del dibujo: es limpieza a mano del carril de
  arte, para la entrega del 07/10/2026.
- **Al retomar el ensamblaje, Mamá se mueve también en el modelo de caminata** (B.4): `ResumeAt`
  colocaba solo la vista. Lo corrige `RiverWalk.MoveTo`; detalle en `Slice 3/Slice-3-Resultados.md`,
  B.11.
- **La 3.3 con lectura rápida** (B.3, «Lo que se acepta») y los puntos de escala de la orilla quedan
  como guiones de Santiago en `claudeDocs/tasks/OE4/Hoja-HUM.md`, H9 y H10.
- **Cifras.** `CharacterCaptureTests` (5) y las capturas por línea de las 18 narrativas siguen en
  verde en la verificación final del 01/10/2026 (EditMode 433 = 432 + 1 omitida, PlayMode 364/364).
  En ella `RiverLevel_RNF05_…` pasó con 1 966 MB, con el Editor reiniciado; RNF-05 se mide sobre el
  ejecutable.
