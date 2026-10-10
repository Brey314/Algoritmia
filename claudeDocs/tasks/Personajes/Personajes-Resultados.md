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
>
> **05/10/2026:** el rig gana codos, rodillas, cuello, cabeza, ojos y boca, y el código de cara,
> a la espera del arte final de Santiago. Lo registra el
> [Anexo C](#anexo-c--rig-articulado-y-caras-para-el-arte-final-05102026). La verificación en el
> Editor está pendiente.
>
> **09/10/2026:** llegó el arte de perfil de Papá, Mamá, la Niña y el Niño y el diseño nuevo de Algoritm.
> La familia se ve de perfil al recorrer o trabajar el entorno, hacia donde se mueve; el parpadeo es de
> dos cuadros; y Algoritm, con su diseño final, se dibuja en siete piezas por forma (INC-134 a INC-136).
> Lo registra el apartado
> [C.13](#c13-vista-de-perfil-rumbo-parpadeo-de-dos-cuadros-y-el-algoritm-final-inc-134-a-inc-136-09102026).
> Las dos rondas del Editor están hechas (la 2, también el 09/10/2026).
>
> **09/10/2026 (INC-147):** los brazos de Algoritm se dibujan detrás de todo el cuerpo, decisión de
> Santiago. Lo registra el apartado [C.14](#c14-los-brazos-de-algoritm-detrás-de-todo-el-cuerpo-inc-147-09102026);
> su ronda del Editor se hizo ese día (`ca62b9a`).
>
> **10/10/2026 (INC-148 a INC-150):** llegó la entrega de expresiones de Sofía (D11-11). Cada línea de las 18
> narrativas se dice con la expresión del guion, la tarjeta del cuadro de diálogo es una cara animada que
> parpadea y habla, el frente de la familia parpadea, Algoritm tiene su cara final, las caras y los retratos
> entran a 256 px como máximo y quien trabaja junto al fuego lo mira. Lo registra el apartado
> [C.15](#c15-expresiones-según-el-guion-tarjeta-del-diálogo-animada-caras-a-256-px-y-el-efecto-espejo-en-la-fogata-inc-148-a-inc-150-10102026).
> La ronda del Editor se hizo ese día en la sesión local de Santiago (`23d7e8c`): los siete prefabs sin un
> fileID nuevo ni perdido, la suite completa en verde salvo una prueba que depende del tiempo de cuadro, 144
> capturas revisadas y un build de 426,6 MB. Después Santiago pidió que la tarjeta de la familia fuera un
> primer plano sin brazos (`292b108`).

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
  *(05/10/2026: el código de las expresiones ya existe —`FacialEmotion`, `CharacterFace`— y se
  aplica al personaje en escena; el retrato del cuadro de diálogo sigue siendo `neutra`. Anexo C,
  C.7. Vencido el 10/10/2026: el retrato del cuadro es una cara animada con la expresión de cada
  línea, INC-148, C.15.)*
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

---

## Anexo C — Rig articulado y caras para el arte final (05/10/2026)

Decisión de Santiago del 05/10/2026 (INC-131), ejecutada desde la nube en el commit `a12dbb3` de
la rama `feat/personajes-animados` (Fase 2–3 de `Plan-Personajes-Finales.md`). El objetivo es que
cambiar al arte final sea **asignar sprites y regenerar clips**, sin reconstruir prefabs ni
tocar escenas o assets.

> **Verificación en el Editor: pendiente.** Los prefabs y los clips **aún no se han regenerado**:
> el generador (`BuildRigsFinal.cs.txt`) lo corre la sesión local de Santiago en el Editor, con los
> modos «nodos» y «clips». Hasta entonces el disco conserva los siete prefabs de cinco partes y los
> 93 clips de siempre, y este anexo describe lo que el generador **añade** y lo que los clips se
> **reescriben** con, no un resultado comprobado. Las pruebas de C.8 tampoco se han corrido.

### C.1 Qué decidió Santiago (05/10/2026)

- **Los personajes finales** —Papá, Mamá, Niña y Niño— vienen en **vista frontal con más capas**:
  la cabeza separada del torso, con ojos y boca en capas propias y las seis expresiones de
  `Plan-Personajes-Finales.md` §5 (Neutral, Happy, Surprised, Worried, Focused, Sleeping), y las
  extremidades en **dos sprites**: húmero y antebrazo, muslo y antepierna. Hay articulaciones en
  codos y rodillas, además de hombros, cadera y cuello.
- **Algoritm** —las tres formas: fuego, rueda y gota— entra en el alcance: brazos, piernas, codos y
  rodillas en dos tramos, y ojos y boca **sobre el cuerpo**. **No tiene cuello**: la llama no lleva
  cabeza aparte.
- **Mientras no llegue el arte**, codos, rodillas y cuello son **pivotes vacíos** y las capas nuevas
  van **apagadas**: el arte provisional se ve igual que antes.
- **Nombres de sprites**: se conservan los actuales (`char_<x>_parte_brazo_*` pasa a ser el húmero y
  `pierna_*` el muslo, con el mismo GUID) y se añaden los nuevos (C.4). Esto sustituye las carpetas
  `Front/` y los nombres `frente_` de `Plan-Personajes-Finales.md` §4.2.
- Corrige para el arte final la regla de «una sola cara, la neutra» (INC-108) y la de Algoritm de
  «una sola imagen, sin recorte en partes»: INC-131.

### C.2 La jerarquía: solo se añaden nodos

Nada existente cambia de fileID; todo nodo nuevo lleva su `Image` **apagada y sin sprite** (una
`Image` sin sprite pinta un recuadro blanco) y sin `raycastTarget` (RNF-02).

**Familia** (Papá, Mamá, Niña, Niño). Lo nuevo va marcado con `+`:

```
Lienzo/Cuerpo
├─ PiernaIzq (muslo)        + RodillaIzq → AntepiernaIzq (Image)
├─ PiernaDer (muslo)        + RodillaDer → AntepiernaDer (Image)
└─ Tronco
   ├─ BrazoIzq (húmero)     + CodoIzq    → AntebrazoIzq  (Image)
   ├─ BrazoDer (húmero)     + CodoDer    → AntebrazoDer  (Image)
   ├─ Torso
   └─ + Cuello  (después de Torso: la cabeza se dibuja encima)
        └─ Cabeza (Image)
             ├─ Ojos (Image)
             └─ Boca (Image)
```

`CodoX`, `RodillaX` y `Cuello` son **pivotes de tamaño cero** colocados en la articulación; el
segmento (`AntebrazoX`, `AntepiernaX`, `Cabeza`) cuelga de ellos, así que girar el pivote gira el
segmento. `Sombra` y `Estela` (B.2) no se tocan.

**Algoritm** (`Algoritm_Fuego`, `Algoritm_Rueda`, `Algoritm_Gota`). Hoy `Lienzo/Cuerpo` es una sola
`Image`, y **sigue siendo lo visible** *(08/10/2026: ya no; las nueve piezas provisionales están
encendidas y `Cuerpo` apagado, ver C.12)*. Cuelga de `Cuerpo` la misma jerarquía, **sin `Cuello` ni
`Cabeza`**, porque la cara va en el propio cuerpo de la llama:

```
Lienzo/Cuerpo  (Image con el sprite entero; se apaga cuando llegan las partes)
├─ + PiernaIzq → + RodillaIzq → AntepiernaIzq
├─ + PiernaDer → + RodillaDer → AntepiernaDer
└─ + Tronco  (nodo estirado sin Image; pivote a la altura del vientre)
   ├─ + BrazoIzq → + CodoIzq → AntebrazoIzq
   ├─ + BrazoDer → + CodoDer → AntebrazoDer
   ├─ + Torso   (la llama y el vientre de colores, sin extremidades)
   ├─ + Ojos
   └─ + Boca
```

Brazos, codos, piernas y rodillas siguen las **mismas rutas** que en la familia, así que
`Lienzo/Cuerpo/Tronco/Cuello` no existe en Algoritm (lo vigila una prueba, C.8). Las tres formas
comparten `char_algoritm.controller`: **sus clips solo giran y escalan articulaciones**; la posición
de cada nodo vive en cada prefab, y la única curva de posición está en `Cuerpo` y `Lienzo`. La
estela no se toca, y Algoritm sigue sin sombra.

**El componente de cara.** Cada uno de los siete prefabs recibe además un `CharacterFace` en la raíz,
añadido con `AddComponent` sobre el prefab cargado (lo que preserva los fileID), con sus campos
`eyes` y `mouth` apuntando a las `Image` de `Ojos` y `Boca`. El generador solo los rellena si están
vacíos: no pisa un cableado manual.

### C.3 La tabla de articulaciones

`herramientas/rig_articulaciones.json` es la fuente de **dónde va cada nodo**: el generador la lee y
con el arte final es lo único que se edita. Los valores de hoy son **provisionales**.

| Campo | Qué es |
|---|---|
| `version`, `nota` | Versión del esquema (1) y el aviso de que los valores son provisionales |
| `personajes[]` | Siete entradas: `papa`, `mama`, `nina`, `nino`, `algoritm_fuego`, `algoritm_rueda`, `algoritm_gota` |
| `id`, `prefab`, `carpeta`, `prefijo`, `guia` | El personaje, su prefab, su carpeta bajo `Assets/Game/Art/Characters/` (`Father`…), el prefijo de sus archivos (`char_papa`, `char_algoritm_fuego`) y si es Algoritm |
| `nodos[]` | Lo que se **añade**, padre antes que hijo. Cada nodo: `nombre`, `tipo`, `padre` (ruta desde la raíz del prefab), `punto` `[x, y]` (donde cae su pivote), `imagen` (el segmento que cuelga), `sprite` (el PNG que se le asigna) y `rect` `[x0, y0, x1, y1]` (el rectángulo del segmento) |
| `tipo` | `articulacion` = pivote de tamaño 0 en `punto` con un segmento `imagen` colgado de él · `imagen` = el propio nodo lleva la `Image` · `grupo` = nodo estirado sin `Image`, con el pivote en `punto` (el `Tronco` de Algoritm) |
| `partes[]` | Lo que **ya existe** y se reajusta con el arte final: `nombre`, `ruta`, `sprite`, `rect` y `pivote`. Vacío en Algoritm, que no tenía partes |

- **Coordenadas.** Las de `cut.py`: lienzo de 1024 × 1024, origen **arriba a la izquierda**, `y`
  hacia abajo y suelo en `y = 947` (la figura va de 77 a 947). Unity usa `y` hacia arriba; el
  generador convierte.
- **`segmentado` no se guarda.** El plan lo pensaba como un campo por personaje; se **deduce del
  prefab**: un personaje está segmentado cuando la `Image` de `AntepiernaIzq` tiene sprite y está
  encendida. Guardarlo en la tabla sería una segunda fuente de verdad que se desincroniza al primer
  cambio de arte.
- **Cómo se estimaron.** `articulaciones.py` lee los prefabs YAML (sin dependencias, sin abrir
  ninguna imagen) y calcula: el **codo**, en el punto medio entre el hombro y la esquina del rect
  más lejana a él, con el antebrazo como cuadrante distal; la **rodilla**, en el punto medio entre la
  cadera y la base del rect, con la antepierna como mitad inferior; el **cuello**, en la base de la
  cabeza y un poco por dentro, para que no se vea el corte. Lo demás es medida a ojo, y está
  marcada «a ojo» en el script: la fracción de la figura que ocupa la cabeza (`FRACCION_CABEZA`),
  la posición y el tamaño de ojos y boca (`CARA`) y todas las medidas de Algoritm (`ALG`, sobre
  `char_algoritm_n1_fuego_reposo.png`; la rueda y la gota son el mismo dibujo recoloreado, así que
  la geometría es una).
- **Cómo se reestima.** `python3 claudeDocs/tasks/Personajes/herramientas/articulaciones.py`
  reescribe el JSON entero. Con el arte final lo normal es **editar el JSON a mano**, con el rect y
  el pivote exactos de cada PNG, o ajustar las constantes del script y volver a correrlo; correrlo
  sin más pisa lo editado a mano.

### C.4 Nombres de los sprites

Se conservan los actuales, con su GUID; se añaden los nuevos. Para Algoritm el prefijo es
`char_algoritm_<fuego|rueda|gota>_`, **uno por forma**, porque cada forma va recoloreada (INC-52).
Los nombres están también en `Assets/Game/Art/Inventario.md`.

| Parte | Familia (`char_<x>_`) | Algoritm (`char_algoritm_<forma>_`) |
|---|---|---|
| Torso | `parte_torso` (existente) | `parte_torso` (la llama sin extremidades) |
| Cabeza | `parte_cabeza` | — no tiene |
| Brazo, tramo alto (húmero) | `parte_brazo_izq` · `parte_brazo_der` (existentes) | `parte_brazo_izq` · `parte_brazo_der` |
| Antebrazo | `parte_antebrazo_izq` · `parte_antebrazo_der` | ídem |
| Pierna, tramo alto (muslo) | `parte_pierna_izq` · `parte_pierna_der` (existentes) | ídem |
| Antepierna | `parte_antepierna_izq` · `parte_antepierna_der` | ídem |
| Ojos por emoción | `ojos_neutra`, `ojos_alegria`, `ojos_sorpresa`, `ojos_preocupacion`, `ojos_concentracion`, `ojos_sueno` | ídem |
| Ojos al parpadear | `ojos_parpadeo_medio`, `ojos_parpadeo_cerrado` | ídem |
| Boca al hablar | `boca_0` (cerrada), `boca_a`, `boca_e`, `boca_u` | ídem |
| Boca de reposo por emoción | `boca_alegria`, `boca_sorpresa`, `boca_preocupacion`, `boca_concentracion` | ídem |

`Neutral` y `Sleeping` no llevan boca propia: su reposo es `boca_0`. Los PNG se buscan por nombre en
la carpeta del personaje y en sus subcarpetas, y el generador los pasa a `Sprite Mode: Single`.

### C.5 El generador y el orden cuando llegue el arte

`herramientas/BuildRigsFinal.cs.txt` es un constructor efímero (no es código del juego), hijo de
`BuildRigs.cs.txt` pero **sin reconstruir nada**. Se copia a `Assets/Editor/ClaudeBuildRigsFinal.cs`
(el Editor genera el `.meta` solo), se llama por coplay `execute_script` con
`ClaudeBuildRigsFinal.Execute("<modo>")`, que devuelve el log como texto, y al terminar se **borra**
la copia: el `.txt` es lo que queda versionado.

| Modo | Qué hace |
|---|---|
| `"nodos"` | Abre los siete prefabs con `LoadPrefabContents`, **añade** los nodos de la tabla que falten y el `CharacterFace`, y guarda con `SaveAsPrefabAsset`. Idempotente: si un nodo existe no lo toca, y nunca borra ni recrea uno. Todo nodo nuevo nace con la `Image` apagada y sin sprite |
| `"sprites"` | Para cuando llegue el arte. Asigna por nombre los PNG que existan, aplica el rect y el pivote de la tabla (también a las partes que ya existían), enciende las `Image` y crea `<Art>/<Carpeta>/Expresiones/char_<x>_cara.asset` (un `CharacterFaceSet`; desde `63ed2fc` y `64e4b37` vive en `Expresiones/`, y un asset de la raíz se mueve con `MoveAsset`, mismo GUID) asignado al `CharacterFace`. En Algoritm apaga la `Image` de `Cuerpo` cuando torso, brazos y piernas ya están. Lo que no existe queda apagado y se anota en el log |
| `"orden"` | Desde INC-132 (C.9). Reordena con `SetSiblingIndex` los hijos de `Tronco` según `orden_tronco` de la tabla: familia con los brazos detrás del torso y delante de la cabeza; Algoritm, hasta INC-147, con los brazos delante del cuerpo y la cara encima, y desde INC-147 (C.14) con los brazos detrás de todo el cuerpo. No crea ni borra nodos: los fileID no cambian |
| `"clips"` | Reescribe en sitio las curvas de los 84 `.anim` de la familia y los 9 de Algoritm (C.6), **aplicando `clips_personajes.json`** (C.9): el modo ya no contiene coreografía. Se niega si faltan los nodos: exige haber corrido «nodos» |
| `"todo"` · `"estado"` | «Nodos», «sprites», «orden» y «clips», en ese orden · solo informa (nodos completos, `CharacterFace` sí o no, segmentado sí o no), sin escribir |

**Orden mientras no hay arte:** `"nodos"` y después `"clips"`. **Orden cuando llegue el arte
final:**

1. Correr `python3 herramientas/preparar_arte_final.py <id> <carpeta>` (C.9): informe y composite; y,
   tras revisarlos, con `--aplicar`, que escribe los PNG con los nombres de C.4, las medidas en
   `arte_final.json` y `rig_articulaciones.json`, `clips_personajes.json` y las órdenes para la sesión
   local. A mano (sin la herramienta): copiar los PNG a `Assets/Game/Art/Characters/<carpeta>/`
   (el motor los importa; los existentes se sustituyen conservando nombre y `.meta`).
2. Con la herramienta, `rig_articulaciones.json` ya lleva el rect y el pivote de cada PNG (C.3); a
   mano se editan.
3. `"sprites"` y después `"orden"` (C.9).
4. `"clips"` — **después** de «sprites» y de «orden», porque lee el prefab para saber si el personaje está
   segmentado y la flexión de las rodillas depende de ello (C.6). También lee del propio prefab las
   longitudes y los pivotes, no de `rigdata.txt`.
5. Pruebas y capturas (C.8); borrar el andamiaje.

**La regla de los fileID.** Dieciocho assets narrativos y cinco escenas referencian componentes de
estos prefabs por fileID, y reconstruirlos (`Root()`/`Finish()` de `BuildRigs`) cambia todos. Aquí:
los prefabs se abren, se les añaden hijos y componentes y se guardan sobre la misma ruta, y los
clips se cargan con `LoadAssetAtPath` y solo se les cambian las curvas, sin tocar los `.controller`:
los GUID de los `.anim` y los estados siguen iguales. **Comprobación tras correrlo:**
`git diff -U0 Assets/Game/Prefabs/Characters` no debe **quitar** ninguna línea que empiece por
`--- !u!` ni tocar `m_Controller`, `m_Script` o `m_Sprite` de lo que ya existía, y `git diff --stat`
no debe mostrar `.meta` de clips. No hay `.meta` escritos a mano: los genera el Editor.

### C.6 Qué cambió en los clips

Los 93 clips (21 por miembro de la familia, 9 de Algoritm) se reescriben **en sitio** con
`ClearCurves` y curvas nuevas, conservando el GUID, el controlador y los eventos. **Desde INC-132 las
curvas ya no las calcula el C#:** las escribe `herramientas/coreografia.py` en `clips_personajes.json`
(ASCII) y el modo `"clips"` solo las aplica; el port reproduce los 93 `.anim` previos con diferencia
máxima de 0,00002 (C.9). Lo que sigue describe el contenido, no quién lo calcula.

- **Una curva en cada hueso, contra la pose en T.** El hueco que cerró: los clips de Talk, Point o
  Encourage dejaban huesos sin curva y, con `writeDefaultValues`, un hueso sin curva vuelve al
  valor del prefab (rotación 0), la pose en T. Ahora **cada clip lleva rotación en todos los huesos
  de su personaje** —doce en la familia: `Cuerpo`, `Tronco`, los dos brazos, los dos codos, las dos
  piernas, las dos rodillas, `Cuello` y `Cabeza`; diez en Algoritm, que no tiene los dos últimos— y
  uno que el clip no usa lleva una curva constante en su **pose de reposo**. En los bucles la curva
  acaba donde empieza. Reposo: hombros y piernas en cero (el arte provisional se ve igual) y los
  codos con una flexión leve propia del personaje (4° Papá, 7° Mamá, 9° Niña, 6° Niño).
- **Se conserva** el tempo y la amplitud por personaje (Papá 1,15/1,1; Mamá 1,0/0,9; Niña 0,95/1,0;
  Niño 0,8/1,2), las curvas de `m_Alpha` de `Hidden`, `Appear` y `Vanish`, y la ausencia de clips
  de caída, salto o derrota. Las curvas usan tangentes `ClampedAuto`, como en `BuildRigs`.
- **Principios «Actions & Stuff»** (`Plan-Personajes-Finales.md` §3):
  - idle con respiración de 3,2 s: el pecho y los hombros suben al inspirar, el peso pasa de una
    pierna a otra y la cabeza llega tarde;
  - inclinación del cuerpo en caminar y correr, hacia donde mira (−4° a −8°);
  - anticipación en golpear, martillar y recoger (un contramovimiento antes del impacto), y
    asentamiento al parar;
  - estirar y aplastar en `Cuerpo`: escala Y entre 0,92 y 1,08 con X compensada, así que ninguna
    pasa del 15 % (Dirección de arte §13.2);
  - movimiento secundario: la cabeza sigue al cuerpo con 2 a 4 cuadros de retraso.
- **Segmentado deducido del prefab.** Con el arte actual (`segmentado = no`) se conserva el truco de
  `BuildRigs`: en `Kneel` y `Sleep` la **pierna entera** escala en Y, y las curvas de rodilla
  existen pero no se ven. Con las piernas partidas (`segmentado = sí`) **ningún clip escala las
  piernas**: baja el tronco, el muslo se abre y la antepierna se cierra hacia dentro y se acorta con
  la escala Y de la rodilla, lo justo para que el pie siga en el suelo.
- **Rodillas y codos en vista frontal.** La flexión de la rodilla ocurre en profundidad, así que se
  dibuja abriendo el muslo hacia fuera y cerrando la antepierna hacia dentro, no con un giro plano.
  Los codos sí giran en el plano.
- **Algoritm**, nueve clips (`flotar`, `hablar`, `senalar`, `girar`, `celebrar`, `animo`, `oculto`,
  `aparicion`, `apagado`): conservan la flotación senoidal de 2 s, el giro de `Spin` sobre `Cuerpo`
  y el alfa; añaden piernas que cuelgan con 3 cuadros de retraso (juntas arriba, abiertas abajo, y
  las rodillas devuelven lo que abre el muslo), brazos que suben un poco con 4 cuadros de retraso y
  gestos de brazos y codos en señalar, celebrar y ánimo. En `hablar` el gesto lo lleva el **tronco**,
  porque no tiene cuello. Con el sprite entero de hoy, **lo visible no cambia** *(desde el 08/10/2026
  Algoritm se dibuja por partes y tiene diez clips, el décimo `saludar`: ver C.12)*.
- **CP-02**: el ánimo es un puño arriba con un bombeo y nunca un gesto de desánimo;
  `Sleep` queda sentado y recostado, no tumbado, porque con los ojos abiertos se leería como caído.

### C.7 La cara y el habla en el código

Todo en `Game.Scaffolding`, salvo el último punto (`Game.UI`).

- **`FacialEmotion`** — `Neutral = 0`, `Happy`, `Surprised`, `Worried`, `Focused`, `Sleeping = 5`, con
  valores explícitos porque los assets los guardan como número. **No hay tristeza, enfado ni
  derrota** (CP-02, Dirección de arte §7.3). `Worried` es la duda de quien pregunta o espera, con la
  boca «apenas curvada, nunca una mueca de llanto»: más suave que el «curvada abajo» del plan.
- **`ActionEmotion.For(ActorAction)`** (C# plano) — la emoción por defecto de cada acción, así que
  las cinco mecánicas ganan expresión sin tocar sus controladores: `Celebrate`, `Hug` y `Encourage`
  → `Happy`; `Surprise` → `Surprised`; `Sleep` → `Sleeping`; `Strike`, `Hammer`, `Blow`, `Push`,
  `Carry`, `PickUp` y `Kneel` → `Focused`; el resto → `Neutral`. **`Encourage` → `Happy` no estaba
  en el plan**: el ánimo viene tras un intento sin éxito, y la cara que lo acompaña es positiva.
- **`BlinkClock`** (C# plano, azar inyectado) — parpadeo cada 3,5 ± 1,2 s que dura 0,12 s. Pasa por
  medio → cerrado → medio → abierto, en tres tercios de la duración (el plan decía abierto → medio →
  cerrado → abierto; un párpado que baja y sube pasa dos veces por la mitad). Se apaga en
  `Sleeping`, cuyos ojos ya son los cerrados.
- **`MouthFlap`** (C# plano) — mientras habla cicla A, E, U y cerrada, 0,09 s cada una, en orden
  fijo y sin azar (la misma línea se ve igual cada vez; no hay audio fonético que seguir). Abre en A
  de inmediato y, al callar, **cierra en seco** a la boca de reposo de la emoción.
- **`CharacterFaceSet`** (ScriptableObject, CT-05) — ojos por emoción más medio y cerrado, bocas
  0/A/E/U, una boca de reposo por emoción (`Neutral` y `Sleeping` usan la cerrada) y los tiempos
  del parpadeo y del aleteo, todos con `[field: SerializeField]` y `[Tooltip]`. Un asset por
  personaje y, en Algoritm, por forma. Todo campo puede quedar vacío.
- **`CharacterFace`** (MonoBehaviour delgado) — pone el sprite de ojos y boca en las dos `Image`.
  **Sin set o sin el sprite pedido, la `Image` queda desactivada**: no hay nunca un recuadro blanco,
  y el arte provisional se sigue viendo entero. Si falta un cuadro del parpadeo o una boca del
  habla, se queda lo de la emoción en vez de desaparecer. Avanza con tiempo escalado, así que la
  pausa (RF-07) congela el parpadeo y la boca.
- **`CharacterRig`** gana `EmotionOverride` (`FacialEmotion?`), `Emotion` (el override o, si no hay,
  `ActionEmotion.For(Current)`) y `Speaking`, todo delegado en el `CharacterFace` opcional. **Las
  firmas públicas existentes no cambian** (`Play`, `PlayFor`, `Speaks`, `Current`, `Mirrored`).
- **`ActorBeat`** gana `SetsEmotion` y `Emotion` (más `WithEmotion(...)`); **`ActorCue`** gana
  `Emotion` (`FacialEmotion?`, parámetro opcional del constructor); **`ActorTimeline.EmotionAt`**
  devuelve la emoción del último paso que la fija en esa línea o antes, y se mantiene hasta que otro
  la cambia, igual que la acción. `SetsEmotion` vale `false` por defecto: **los 18 assets no
  cambian de comportamiento**.
- **`NarrativeSceneController`** (`Game.UI`) aplica `EmotionOverride` al colocar cada línea y al
  terminar un camino, y en `Render` marca `Speaking` en el rig de quien dice la línea
  (`Rig.Speaks(speaker)`); `Leave` lo apaga en todos. El texto aparece entero de golpe, sin revelado
  progresivo, así que «habla» dura hasta que se avanza la línea. Una acotación no tiene hablante y
  no mueve ninguna boca.

**Lo que este trabajo no hace** (sigue pendiente; ver `Plan-Personajes-Finales.md`): el retrato animado
del cuadro de diálogo, y poblar emociones explícitas en los 18 `N*_*.asset`. *(Los perfiles, que esta
sección daba por pendientes, se abordaron el 09/10/2026: C.13.)*

### C.8 Pruebas nuevas

EditMode, en `Assets/Tests/EditMode/Scaffolding/`:

| Clase | Pruebas |
|---|---|
| `CharacterRigTests` | `CharacterRig_DA131_LaFamiliaTieneCodosRodillasYCuello` · `…_AlgoritmTieneCodosYRodillasYCaraSinCuello` · `…_TodaCurvaApuntaAUnaParteQueExiste` (recorre los `GetCurveBindings` de cada clip: hasta ahora nada lo hacía y una curva colgada no avisaba) · `…_CadaClipFijaTodasLasArticulaciones` · `…_UnaCapaSinSpriteNoSeDibuja` · `…_ConExtremidadesPartidasNingunClipEstiraLasPiernas` · `CharacterRig_DA73_LaCaraDelRigApuntaASusCapasDeOjosYBoca` |
| `FacialEmotionTests` | `FacialEmotion_DA73_CadaAccionTieneSuEmocionPorDefecto` · `…_LaTablaCubreTodasLasAcciones` · `FacialEmotion_CP02_NingunaEmocionEsDeDerrotaNiTristeza` · `…_TrasUnIntentoSinExitoElAnimoEsUnaCaraAlegre` · `BlinkClock_DA73_ParpadeaConElIntervaloYLaDuracion` · `…_ElAzarMueveElIntervaloDentroDelDesvio` · `…_ApagadoNoParpadea` · `MouthFlap_RF05_LaBocaSeMueveMientrasHablaYSeCierraAlCallar` |
| `CharacterFaceTests` | `CharacterFace_DA73_SinSpritesNoDibujaNada` · `…_ToleraCapasSinAsignar` · `…_ConSetMuestraLosOjosDeLaEmocion` · `…_DormidoNoParpadea` · `CharacterFace_RF05_HablandoLaBocaCambiaYAlCallarVuelveAlReposo` · `CharacterRig_DA73_LaEmocionSigueALaAccionSalvoQueUnPasoLaFije` · `CharacterRig_DA73_SinCaraLaEmocionYElHabloSeGuardanSinFallar` |
| `ActorTimelineTests` | `ActorTimeline_RF05_LaEmocionDeUnPasoSeMantieneHastaQueOtroLaCambie` · `…_SinEmocionDeclaradaElCueNoLaFija` |

PlayMode, `Assets/Tests/PlayMode/UI/NarrativeSceneTests.cs`:
`NarrativeScene_RF05_QuienHablaHablaYCallaAlAvanzar`, que recorre `N1_Hallazgo` línea a línea y
comprueba que solo el hablante tiene `Speaking`. Comprueba el **estado del rig y no los sprites**
(hasta que llegue el arte `CharacterFace` no dibuja nada), así que no necesita `Assert.Ignore`.

Las de `CharacterRig_DA131_*` leen los prefabs y los clips del disco: miden el resultado del
generador y no pueden darse por buenas antes de correr «nodos» y «clips». Deben seguir en verde las
existentes: `CharacterRig_DA76_…` (la estela de Algoritm), `CharacterRig_DA53_AlgoritmNoLlevaSombra…`,
las de `ActorTimeline`, las `NarrativeScene_RF05_*` y las `Personajes_DA133_*`.

**Verificación en el Editor: pendiente.** Falta correr el generador, comprobar los fileID (C.5),
EditMode de `Scaffolding`, PlayMode de las escenas con personajes, la suite completa por
`NORMA-PRUEBAS.md` y revisar las capturas de `Personajes_DA133_*` (sin pose en T y con el arte
provisional igual en reposo). Las cifras de esa corrida irán en este anexo cuando existan.


### C.9 Orden de dibujo de los brazos y entrada del arte final (INC-132, 05/10 y 06/10/2026)

**El problema.** Santiago vio un Idle frontal pobre y brazos escondidos detrás de la cabeza o del
cuerpo. La causa: bajo `Lienzo/Cuerpo/Tronco` los hijos iban `BrazoIzq`, `BrazoDer`, `Torso`,
`Cuello` (uGUI pinta de atrás adelante), y el hombro del Niño pivotaba en el centro del pecho.

**La decisión (INC-132), definitiva.** En la **familia**, los brazos se dibujan **detrás del torso y
delante de la cabeza**: `orden_tronco` = `Cuello`, `BrazoIzq`, `BrazoDer`, `Torso`, con la cabeza al
fondo. Con el arte provisional la cabeza está pintada dentro del torso, así que hoy los brazos quedan
tras el torso y tras la cara hasta el arte final. Sustituye para la familia la regla de INC-131 de dibujar el cuello sobre el torso. (Etapas
intermedias del mismo día: delante en el Niño y Algoritm → delante en los siete, commit `5ce0797` →
regla definitiva.)

**Ajustes del 06/10/2026 (decisiones de Santiago).**

- **Strike con los brazos delante.** Mientras el personaje golpea las piedras, los brazos pasan
  **delante del torso**; al cambiar de acción vuelven exactamente a su orden. Lo hace `ArmLayering`
  (C# plano, `Game.Scaffolding`: reordena los hijos de `Tronco` por nombre y restaura el orden de
  origen) desde `CharacterRig.Apply`, según el campo serializado `armsInFrontActions = { Strike }`; el
  inicializador del campo vale para los prefabs que no lo serializan (los tres de Algoritm sí lo
  serializan, con ese valor, al guardarse). El cambio de capa es seco al empezar la acción: no espera
  el fundido de 0,18 s. Si falta un nodo, avisa y no mueve nada. En Algoritm no movía nada, porque sus
  brazos ya iban delante del cuerpo (hasta INC-147; ver C.14).
- **Algoritm: manos sobre la cara (hasta INC-147, C.14).** `orden_tronco` = `Torso`, `Ojos`, `Boca`, `BrazoIzq`, `BrazoDer`.
  La coreografía y `pose_preview.py` impiden que una mano entre en la caja de ojos y boca (ampliada un
  30 %, con al menos el 99,5 % libre).
- **Coreografía de Strike.** Choque delante del pecho con brazo partido; con el brazo de una pieza del
  arte provisional (Papá, Mamá y Niña hoy) el brazo se encoge hasta el 45 % durante el golpe para que
  el choque quede en el vientre, nunca bajo la cintura (por encima de la cabeza tapaba la cara y a un
  costado no había contacto). **Pendiente de que Santiago lo apruebe**; alternativa: golpear sin
  juntar las manos hasta el arte final.

**Qué entró** (commits `8740a64`, `6f2b4cf`, `590866f`, `d817d95`, `5ce0797`, `0b76bbc`, `117287c`,
`a5a887b` y `8cdd4c5`):

- Clave `orden_tronco` en `rig_articulaciones.json` y modo `"orden"` del generador (C.5), sin tocar
  fileIDs. Orden de modos: nodos → sprites → orden → clips (`"todo"`), más `"estado"`.
- **La coreografía pasa del C# a Python.** `coreografia.py` escribe `clips_personajes.json` y el modo
  `"clips"` solo lo aplica; sigue sin crear clips ni tocar controladores, GUID ni eventos. Port
  verificado contra los 93 `.anim` previos (`coreografia_v0.py`, solo regresión): diferencia máxima
  0,00002.
- **Coreografía escrita una vez, para el arte final**, como objetivos de mano: con brazo partido,
  cinemática inversa de dos tramos; con brazo de una pieza, el hombro apunta la mano y el codo ya
  lleva su curva. **Idle de 6,4 s × tempo** (dos respiraciones de 3,2 s, que conservan el ciclo de
  `Direccion_de_Arte.md` §13.3) con gesto de carácter (la visibilidad de cada brazo se deriva de la geometría, con la silueta del torso, y el reposo se calcula por personaje): el Niño mira a los lados, rebota, brazos en
  péndulo y se rasca junto a la oreja; la Niña balancea los brazos (las manos entrelazadas no se verían); Papá, brazos en jarra por fuera
  del torso y pecho hinchado; Mamá, manos a la cintura por fuera del torso y mano a la oreja por
  delante del pelo, sin tapar ojos ni boca; Algoritm, parpadeo de llama con brazos
  alternos. `Encourage`: puño fuera de la cabeza con rebote y cabeceo, 0,9 s × tempo (los
  controladores usan `EncourageSeconds` = 0,9 / 1,035), CP-02. `Blow` agachado con tres soplos;
  `Hug` abre y se pliega a los costados; `Push` con los puños en las caderas; `Strike` con el choque delante del pecho (arriba); `Observe` con visera desde la sien (el Niño, por
  sus brazos cortos y su cabeza grande, no llega a la frente).
- **Geometría del Niño corregida:** hombro y codo en el centro del extremo redondo de cada cápsula
  (`BrazoIzq` pivote [466,560], `BrazoDer` [556,560], `CodoIzq` [366,609], `CodoDer` [667,605]),
  rodillas [483,845] y [558,842], ojos [402,380,602,470], boca [447,468,557,512].

**Herramientas nuevas** (`claudeDocs/tasks/Personajes/herramientas/`):

| Archivo | Para qué |
|---|---|
| `coreografia.py` | La coreografía y su salida, `clips_personajes.json` |
| `coreografia_v0.py` | Solo regresión del port |
| `pose_preview.py` | Renderiza el rig fuera de Unity a 30 fps y lo prueba: brazos ≥ 85 % visibles, cara ≥ 90 %, sin hiperextensión, pies ≤ 2 px bajo el suelo, manos fuera de la caja de la cara salvo excepciones explícitas, giro ≤ 1300 °/s salvo el martillazo, el torso no tapa más del 5 % de ojos y boca, ninguna mano entra en la cara de Algoritm y el choque de `Strike` queda sobre la cintura (comprobación h; `EXCEPCIONES_BRAZO` vacía). Lee `armsInFrontActions` de `CharacterRig.cs` y dibuja cada clip con su orden. Modos `--hoy` y `--maqueta`: 174/174 |
| `maqueta.py` | Arte final simulado, cortando el provisional por las articulaciones |
| `prefabs.py` | Simula lo que hará el modo `"sprites"` (quien ya tiene sus piezas se trata como segmentado) y el orden de dibujo, también el de `armsInFrontActions` |
| `arte_final.json` | Medidas del arte final por personaje; `articulaciones.py` lo lee |
| `preparar_arte_final.py` | Ver abajo |

**`preparar_arte_final.py <id> <carpeta> [--aplicar]`** reconoce las capas, limpia el alfa,
normaliza a 870 unidades con el torso en x = 512, mide las articulaciones sobre el alfa y escribe PNG,
tablas, clips y las órdenes para la sesión local; `--autoprueba` usa una entrega sintética del Niño
(error ≤ 1 px). Su cabecera trae «COMO ENTREGAR EL ARTE FINAL».

**Flujo cuando llegue el arte final de X:** (1) `python3 preparar_arte_final.py X <carpeta>`; revisar
informe y composite; (2) con `--aplicar`; (3) la sesión local copia `BuildRigsFinal.cs.txt` a
`Assets/Editor/ClaudeBuildRigsFinal.cs`, recompila, corre `estado`, `sprites`, `orden`, `clips` y
`estado`, compara los prefabs objeto por objeto, corre las pruebas, hace commit de PNG, `.meta`,
prefab y `.anim`, y borra el andamiaje.

**Pruebas nuevas:** `CharacterRig_INC132_LosBrazosSeDibujanDetrasDelTorsoYDelanteDeLaCabeza` (los cuatro
de la familia) `CharacterRig_INC132_AlgoritmPintaLasManosEncimaDeLaCara` (las tres formas, antes `…DelanteDelCuerpoYLaCaraEncima`; hoy `CharacterRig_INC147_LosBrazosDeAlgoritmVanDetrasDeTodoElCuerpo`, C.14)
y, del 06/10, `CharacterRig_INC132_AlGolpearLosBrazosPasanDelanteDelTorso`,
`…_AlTerminarElGolpeLosBrazosVuelvenDetrasDelTorso`, `…_EnAlgoritmElGolpeNoCambiaElOrdenDeDibujo` (hoy `CharacterRig_INC147_EnAlgoritmGolpearVuelveAlOrdenDeOrigen`, C.14),
`…_SoloElGolpePoneLosBrazosDelante` y `…_SiFaltaUnNodoElGolpeAvisaYNoMueveNada`.

**Cambia una prueba existente:** `CharacterRig_DA131_LaFamiliaTieneCodosRodillasYCuello` deja de
exigir el cuello sobre el torso (INC-132 sustituye esa regla de INC-131).

**Verificado en el Editor.** `Game.Scaffolding.Tests` 215/215 tras `6f2b4cf` y otra vez 215/215 tras
la ronda de `590866f` y `d817d95`. Suite completa tras `6f2b4cf`: 940/942 (1 omitida; 1 fallo ajeno en
`ForestScene_RF22`: el ratón real del Editor hace rodar un tronco del bosque; pasa 3/3 aislada y 38/38
en su grupo; queda como tarea aparte del carril del N2). Ronda de `5ce0797` (brazos delante en los
siete, ya sustituida): `Game.Scaffolding.Tests` 215/215 y suite completa 941/942 (1 omitida
preexistente, 0 fallos; `ForestScene_RF22` pasó); cambiaron 3 prefabs (solo el orden de `Tronco`) y 67
`.anim` (Papá 21, Mamá 21, Niña 21, Niño 4), con fileIDs, `m_Script`, `m_Controller` y `m_Sprite`
intactos en los siete. **Ronda de `0b76bbc` (regla definitiva):** la sesión local corrió `orden` y
`clips` y subió en `117287c` 4 prefabs (solo el orden de `Tronco`) y 77 `.anim` (Papá 21, Mamá 16,
Niña 21, Niño 19); `Game.Scaffolding.Tests` 215/215 y suite completa en un solo Editor 940/942 (1
omitida preexistente, 1 fallo: `RiverLevel_RNF05`, 2199 MB con el Editor abierto todo el día; recién
abierto mide 1533 MB reservados y 1072 asignados, y `Game.Levels.River.PlayMode.Tests` pasa 57/57).
`suite2.ps1` no pudo correr esta ronda: el segundo Editor murió dos veces por memoria al abrirse.
**Ronda de `8cdd4c5` (Strike delante, manos de Algoritm sobre la cara, hasta INC-147):** la sesión local corrió `orden`
y `clips`; cambiaron 3 prefabs de Algoritm (orden de `Tronco` y el campo serializado
`armsInFrontActions`), 4 `.anim` de golpear y se generó el `.meta` de `ArmLayering.cs`; fileIDs,
`m_Script`, `m_Controller` y `m_Sprite` intactos en los siete. `Game.Scaffolding.Tests` 234/234 y suite
completa en un solo Editor (`tests-edit -` y `tests-play -`) 960/961 (1 omitida preexistente, 0
fallos; `ForestScene_RF22` y `RiverLevel_RNF05` en verde). `suite2.ps1` murió dos veces por memoria al
abrir el segundo Editor; `RiverLevel_RNF05` falla con un Editor abierto muchas horas (2199 MB) y pasa
recién abierto (1533 MB), así que conviene reiniciar el Editor antes de la suite.

### C.10 Subcarpetas, caras por registro y antebrazo delante (INC-133, 06/10/2026)

Rama `feat/personajes-animados`, de `63ed2fc` a `433603f`. Lo que sigue se verificó contra `git log` y `git show` de cada commit.

**Por commit**

- `63ed2fc`: `CharacterFaceSet` cae a los ojos neutros y la boca cerrada cuando una emoción no tiene sprite propio, de modo que una sola expresión entregada ya muestra cara en todas las acciones. `CaraBase` pasa a ser el primer hijo de `Cabeza`. `BuildRigsFinal.cs.txt` guarda el set de cara en `Expresiones/` y prefiere la copia de la subcarpeta. Nuevo `OrganizarArtePersonajes.cs.txt`, que reparte el arte de cada personaje en `Frontal/`, `Expresiones/` y `Perfil/` usando solo `AssetDatabase`. Pruebas: `CharacterFace_DA73_SinLaExpresionDeLaEmocionUsaLaNeutra`, `CharacterFace_DA73_UnaEmocionAMediasCompletaConLaNeutra` y `CharacterRig_DA131_LaCaraBaseVaDetrasDeLosOjosYDeLaBoca`.
- `a538ab6`: primera cara neutra del Niño, con una heurística de escala (60 % del óvalo). Superada por `233f2e5`.
- `d26ea8f`: entrega original del 06/10/2026 de Papá, Mamá, Niña y Niño (carpetas Frente y Expresiones; 45 PNG, 2,4 MB) en `claudeDocs/tasks/Personajes/entregas/2026-10-06/`, fuera de `Assets`.
- `64e4b37` (WIP, sesión local): el organizador corrido movió 25 PNG a `Frontal/` con los GUID intactos; `CaraBase` entró en los cuatro prefabs de la familia (cuatro objetos nuevos por prefab, ningún fileID perdido); el Niño tiene su cara en el prefab y en `char_nino_cara.asset`; partes finales de Papá, Mamá y Niña; Papá con la cabeza delante del torso. Sus `sprites`, `orden` y `clips` quedaron pendientes.
- `ef37dd9` (INC-133): decisión de Santiago del 06/10/2026. En la familia el húmero se dibuja detrás del torso y el antebrazo delante del torso, de la cara y de las piernas. Como uGUI pinta por orden de jerarquía, `AntebrazoX` (mismo objeto y fileID) pasa a hijo de `Tronco`, al final, y sigue a un nodo vacío `AnclaAntebrazoX` que se deja bajo `CodoX` con la pose local anterior del antebrazo. `LimbFollower` (C# plano, `Game.Scaffolding`) compone `BrazoX`, `CodoX` y el ancla respecto de `Tronco` y escribe la pose local del antebrazo; `CharacterRig` descubre los pares por nombre y los sincroniza tras `Play` + `Update(0)` y en `LateUpdate`. No hay campos serializados nuevos y los clips no cambian, porque no animan el antebrazo. Los personajes sin anclas (arte provisional, Algoritm) no se tocan. `ArmLayering` conserva su lógica: al golpear sigue poniendo el húmero justo tras `Torso`, todavía detrás de los antebrazos. El modo `"orden"` crea las anclas y mueve los antebrazos cuando `orden_tronco` los lista, sin reconstruir nada.
- `233f2e5`: caras de los cuatro por registro. Los lienzos de 1300×1500 de la entrega están registrados entre sí, así que `preparar_expresion.py --registrada` coloca `Ojos`, `Boca` y `CaraBase` con la misma transformación con que `preparar_arte_final.py` colocó las partes de ese personaje (guardada como `registro` en `arte_final.json`), en lugar de la heurística de escala. El Niño coincide con la foto de Santiago dentro del 0,6 % de la altura (ojos de 295 px de ancho en vez de 188; brazos 20 a 25 px hacia adentro, como en la entrega). Se sustituyeron en su sitio 12 PNG de `Expresiones/` (mismos nombres, `.meta` y GUID). `orden_tronco` de la familia lista los antebrazos al final (Papá: `BrazoIzq`, `BrazoDer`, `Torso`, `Cuello`, antebrazos). `pose_preview.py` mide aparte la visibilidad del húmero (40 %) y la del antebrazo (85 %). El cuello de los personajes con melena o barba gira a la altura de la barbilla. `Strike` mantiene las manos dentro del ancho de hombros y el choque sobre la cintura; los gestos cerca de la cabeza rodean la cara. Se reconocen las piezas `pies_*`.

**Disposición del arte.** Cada personaje queda en `Assets/Game/Art/Characters/<Carpeta>/` con `Frontal/` (partes `char_<x>_parte_*.png`), `Expresiones/` (ojos, bocas, `char_<x>_cara_base.png` y `char_<x>_cara.asset`) y `Perfil/` (vacía, con `.gitkeep`, hasta que haya arte; desde el 09/10/2026 recibe el de perfil de la familia, C.13). Los retratos, los reposos de Algoritm y `Animations/` siguen en la raíz de la carpeta. Los originales de entrega viven en `claudeDocs/tasks/Personajes/entregas/<fecha>/`.

**Corridas**

| Corrida | Alcance | Resultado |
|---|---|---|
| `64e4b37`, Editor local | EditMode | 589 pasan, 1 omitida, 0 fallos |
| `64e4b37`, Editor local | PlayMode | 376/377; falla `RiverLevel_RNF05` con 2084 MB, en un Editor de larga duración |
| `233f2e5`, fuera del Editor | `--autoprueba` (cuatro), `coreografia.py --valida`, `pose_preview.py` | 21/21 por miembro de la familia y 9/9 por Algoritm; sin curvas sobre `AntebrazoX` ni su ancla |
| `ef37dd9`, `CharacterRigLimbTests` | EditMode, jerarquía armada en código | pasan al escribirse (según el mensaje del commit) |
| `ef37dd9`, `CharacterRig_INC133_*` | EditMode, prefabs reales | fallaban hasta correr `orden`; pasan tras la ronda de `433603f` (14/14 con las del antebrazo y el ancla) |
| `433603f`, Editor local | `Game.Scaffolding.Tests` | 278/278 |
| `433603f`, Editor local | antebrazo y ancla | 14/14 |
| `433603f`, Editor local | EditMode | 627 pasan, 1 omitida, 0 fallos |
| `433603f`, Editor local | PlayMode | 376/377; falla `RiverLevel_RNF05` con 2161 MB, en un Editor de larga duración |
| `433603f`, Editor local | `CharacterCapture` | 5/5 |

**Ronda del Editor de `433603f` (06/10/2026).** El generador (`sprites`, `orden` y `clips`) se corrió sobre los cuatro prefabs de la familia: 14 sprites en Papá, Mamá y la Niña y 11 en el Niño; los juegos de cara `char_papa_cara`, `char_mama_cara` y `char_nina_cara` (neutra = `ojos_neutra` + `boca_0`); `AnclaAntebrazoIzq` y `AnclaAntebrazoDer` bajo cada codo, con los antebrazos al final de `Tronco`; Papá con `Cuello` tras `Torso`; 93 clips reescritos. Comparados objeto por objeto contra `HEAD`, cada prefab de la familia tiene 4 objetos nuevos (las dos anclas), ninguno perdido, y `m_Script` y `m_Controller` iguales; Algoritm no cambió; una segunda orden `orden` no guarda nada. 24 `.meta` de PNG pasan a `Single` (`spriteMode` 2 a 1) con los GUID iguales. Capturas en `claudeDocs/tasks/Personajes/capturas/2026-10-06/`: `01` y `01b` (familia en reposo en la escena 2.2), `02`, `02b` y `02c` (Papá en el Nivel 1), `03` (Mamá hablando), `04` (la Niña hablando) y `05` (Papá en reposo en el Nivel 1). No hay cuadro del choque de Papá.

**Puntos abiertos.** Falta el arte final de Algoritm. Dos decisiones de coreografía esperan a Santiago: los gestos junto a la cabeza, que se abren a los lados, y el reposo en A de los brazos (40 a 72°). *(06/10/2026: decididas y aplicadas, ver C.11.)* `RiverLevel_RNF05` falla en el Editor de larga duración (2161 MB aquí, 2199 MB el 05/10) y pasa recién abierto: es un hecho conocido de la sesión del Editor, no del ejecutable. Peso y memoria del arte nuevo no se han medido en un ejecutable.

### C.11 Pivote del hombro y gestos sobre la cara (06/10/2026)

Rama `feat/personajes-animados`, de `6ed4d23` a `28337c0`. Lo que sigue se verificó contra `git show` de cada commit.

**Decisiones de Santiago Benavides Rey (06/10/2026).** Cierran los dos puntos de coreografía que C.10 dejó abiertos. (1) Acepta que la mano cubra la cara en los gestos junto a la cabeza. (2) Acepta subir y mover el pivote del hombro para normalizar el reposo. No hay INC nuevo: `Direccion_de_Arte.md` no prohíbe que una mano tape la cara ni fija el ángulo de reposo; son reglas de las herramientas (`pose_preview.py`, `coreografia.py`). Se comprobó con grep sobre `Direccion_de_Arte.md`.

**Por commit**

- `6ed4d23` (nube, fuera del Editor): `hombro.py`, nuevo, lleva el pivote de `BrazoIzq` y `BrazoDer` del centro del pecho al borde del torso (6 px más arriba y de 24 a 44 px hacia afuera), sin mover el arte registrado; `preparar_arte_final.py` lo corre tras escribir `arte_final.json`. Pivotes en px del lienzo del rig, antes a después: Papá (448, 483) y (567, 483) a (406, 477) y (611, 477); Mamá (499, 543) y (529, 550) a (463, 537) y (563, 544); Niña (489, 537) y (541, 540) a (451, 531) y (577, 534); Niño (489, 567) y (535, 564) a (463, 561) y (559, 558). El ángulo de reposo pasa de 54, 72, 40 y 57° a 15, 20, 13 y 20° (Papá, Mamá, Niña, Niño), con la mano a la altura de la cadera o el muslo. Los gestos junto a la cabeza (la visera de `Observe`, `Celebrate`, `Hammer`, `Encourage`, `Carry` y el rascado del Niño en `Idle`) ya no rodean la cara: la mano llega a la frente o a la sien, y el brazo del Niño se queda de 25 a 33 px corto de la sien. En `pose_preview.py`, `EXCEPCIONES_CARA_TAPADA` fija un piso de cara visible por gesto (`Observe` 60 %, `Celebrate` 80 %, `Hammer` 55 %, `Encourage` 80 %, `Carry` 80 %, el `Idle` del Niño 60 %), `EXCEPCIONES_VISERA` exime al Niño de la visera sobre la frente, y una comprobación nueva exige que nunca se tapen los dos ojos a la vez (el ojo menos tapado, a lo sumo 25 %). `UMBRAL_HUMERO` baja del 40 al 30 % por el pivote nuevo; el antebrazo sigue en el 85 %. El `Idle` de Mamá deja de apoyar la mano en la cadera.
- `28337c0` (sesión local, Editor): aplica lo anterior a los cuatro prefabs de la familia y a sus clips. `BuildRigsFinal` en modos `estado`, `sprites` (11 por prefab de la familia), `clips` (93 reescritos; cambian 84, 21 por miembro de la familia, y los 9 de Algoritm quedan idénticos) y `estado` (los siete prefabs cumplen la tabla). Pose de mundo medida antes y después instanciando cada prefab: solo se mueven `BrazoIzq` y `BrazoDer` (el hombro izquierdo de Papá pasa de (−64, 29) a (−106, 35)); codos, `AnclaAntebrazoX` y antebrazos quedan donde estaban. Comparados contra `HEAD`, ningún objeto nuevo ni perdido en los siete prefabs, `m_Script` y `m_Controller` iguales; en los cuatro de la familia cambian solo 8 líneas `m_Pivot` (2 `RectTransform` por prefab) y Algoritm no cambia. No se tocó ninguna escena, ni `Assets/Game/Data/`, ni `ProjectSettings`.

**Corridas**

| Corrida | Alcance | Resultado |
|---|---|---|
| `6ed4d23`, fuera del Editor | autopruebas de todas las herramientas, `coreografia.py --valida` | pasan (según el mensaje del commit) |
| `6ed4d23`, fuera del Editor | `pose_preview.py` | 21/21 por miembro de la familia y 9/9 por forma de Algoritm; choque de `Strike` sobre la cintura; sin curvas sobre antebrazos ni anclas |
| `28337c0`, Editor local | `Game.Scaffolding.Tests` | 278/278 |
| `28337c0`, Editor local | `CharacterRigLimb` | 14/14 |
| `28337c0`, Editor local | `CharacterCapture` | 5/5 |
| `28337c0`, Editor local | EditMode | 627 pasan, 1 omitida, 0 fallos |
| `28337c0`, Editor local | PlayMode | 376/377; falla `RiverLevel_RNF05` con 2353 MB (2161 MB en la ronda de `433603f`), en un Editor de larga duración |

**Capturas** (`claudeDocs/tasks/Personajes/capturas/2026-10-06b/`, en LFS): `01_reposo_cuatro_zoom.png` y `01_reposo_escena_completa.png` (línea 6 de la escena 2.2 del Nivel 2: el Niño y Mamá con los brazos caídos y sin hueco en el hombro; Papá con las manos en la cintura y la Niña con los brazos en alto son los gestos de esa línea), `02_gestos_cabeza_L05_cuatro_zoom.png` y `02_gestos_cabeza_L05_escena_completa.png` (línea 5: el Niño con las manos junto a la cara, la Niña con los brazos en alto, Mamá señalando), `03_mama_hablando.png` y `04_nina_hablando.png`. No hay captura del Editor de un `Observe` o un `Celebrate` con la mano en la frente, ni del choque de `Strike`.

**Puntos abiertos.** Falta el arte final de Algoritm. El brazo del Niño queda de 25 a 33 px corto de la sien. No hay captura del Editor de los gestos sobre la frente ni del choque de `Strike`: ambos se comprobaron solo en `pose_preview.py`. `RiverLevel_RNF05` falla en el Editor de larga duración (2353 MB aquí): conviene reiniciar el Editor antes de la suite. Peso y memoria del arte nuevo no se han medido en un ejecutable.

### C.12 El codo de Papá y Algoritm por partes con `Wave` (08/10/2026)

Rama `feat/personajes-animados`, ronda de ajustes de diseño del 08/10/2026
(`claudeDocs/tasks/Ajustes-Diseno-2026-10-08/`; decisiones D13, D14, D16, D12 y D20 de su `brief.md`).
Commits: `a938f9a` (Papá), `bd8b802` (Algoritm y créditos), integrados con el carril de perfil en el
merge `eb9be13`. Las cifras de abajo salen de los partes de las etapas 1 y 4a y de `git show` de
cada commit. Es el registro de lo hecho; el estado vigente está en C.2 a C.11 con las notas de
remisión, en `Direccion_de_Arte.md` §7.6 y §13 y en CLAUDE.md.

**Papá: el húmero ya no asoma por el codo (D14, D16).** Santiago vio que, al mover el antebrazo de
Papá, se veía parte del húmero. El arte final de Papá trae el húmero con una punta clara y sin
contorno que se pasa de su extremo redondo, con el antebrazo solapado encima: la distancia entre el
extremo del húmero y el casquete del antebrazo era de 42 px a la izquierda y 21 px a la derecha (en
Mamá, Niña y Niño, de 1 a 8 px). El pivote del codo quedó en el extremo del húmero, a 40,0 y 20,9 px
del casquete, y al doblar el codo la punta salía como un bulto sin contorno. `codo.py`, nuevo, sube el
húmero y baja el antebrazo a lo largo de su eje hasta dejar el punto más lejano 4 px dentro del
casquete, sin mover el pivote del hombro y sin tocar ningún PNG. Resultado, en px del lienzo de 1024:

| Dato | Antes | Después |
|---|---|---|
| `BrazoIzq.rect` (húmero izquierdo) | [296, 457, 478, 612] | [311, 445, 493, 600] (sube 19,2) |
| `CodoIzq.rect` (antebrazo izquierdo) | [178, 543, 376, 718] | [150, 567, 348, 742] (baja 37,4) |
| `CodoDer.rect` (antebrazo derecho) | [637, 542, 835, 717] | [665, 566, 863, 741] (baja 37,4) |
| `BrazoDer.rect` (húmero derecho) | [537, 457, 700, 601] | igual |
| Holguras `CodoIzq` / `CodoDer` | 42,0 / 20,9 | 14,2 / 16,1 |
| Húmero que asoma (`asoma_codo`, tope 15) | 52,6 / 33,4 | −3,2 / +1,0 |

Los dos antebrazos bajan lo mismo para que los brazos queden simétricos. Efectos que Santiago
aprobó (D16): los brazos de Papá miden unos 37 px más (4 % de su alto) y el choque de `Strike`
baja de y = 549 a y = 595, con tope en la cintura a 616, con los codos más abiertos. `Idle` con las
manos en la cadera, `Push`, `Hug` y `Observe` también cambian algo; el reposo no (15° de hombro,
6° de codo). `preparar_arte_final.py` corre ahora `codo.py` justo después de `hombro.py`, y
`pose_preview.py` gana la comprobación (j) del codo (`ASOMA_MAX_CODO = 15`). En el motor, el modo
`sprites` de `BuildRigsFinal` cambió 15 líneas de `Papa.prefab` (anclas y pivotes de siete
`RectTransform`, ningún fileID) y `clips` reescribió los 21 `.anim` de Papá; los clips de los otros
cuatro personajes no cambian. Corridas: `pose_preview.py` pasa en 119 filas (111 clips más 8 codos),
`CharacterRig_` 142/142 y `Game.Scaffolding.Tests` 278/278 en EditMode.

**Algoritm se dibuja por partes con arte provisional (D13).** Decisión de Santiago: sus brazos y
piernas se mueven en todas sus escenas sin esperar al arte final. Las tres formas comparten
geometría, así que se cortaron las tres: nueve piezas por forma (`char_algoritm_<forma>_parte_torso`,
`_brazo_{izq,der}`, `_antebrazo_{izq,der}`, `_pierna_{izq,der}` y `_antepierna_{izq,der}`), 27 PNG
en `Assets/Game/Art/Characters/Algoritm/Frontal/`. Los genera
`pose_preview.py --exporta-maqueta` con `maqueta.piezas_guia`, que corta por la forma del dibujo y
no por rectángulos (un rectángulo se llevaba un trozo del contorno del vientre en cada hombro y
cadera y dejaba escalones en el codo y la rodilla): el cuerpo es lo que sobrevive a una apertura
con disco de 16 px, lo libre dentro de la zona de cada extremidad es de ella, cada extremidad se
parte en el codo o la rodilla con un plano perpendicular al eje y la pieza de arriba lleva una
rótula del radio del palo. Se corta a la resolución del sprite (768) y las piezas sumadas
reproducen el sprite con 0,002 % de píxeles distintos. Pesan 908,6 KB en disco y 3,63 MiB como
textura sin comprimir para las tres formas (sin medir aún en un ejecutable; RNF-06 tenía 21 MB de
margen; medido el 09/10/2026 en C.13: con el arte final el paquete llegó a 499,3 MB y INC-146 lo bajó a 420,6 MB). Los puntos de articulación se midieron sobre el alfa de los palos (27 px de ancho): el
hombro estaba 17 px por encima del palo y pasó de (292, 612) y (735, 612) a (287, 629) y (736, 630),
y el codo, de (228, 688) y (798, 684) a (224, 693) y (800, 693). `articulaciones.py` regeneró
`rig_articulaciones.json` (solo cambian las tres entradas de Algoritm). En el Editor, `BuildRigsFinal`
corrió `sprites`, `orden` y `clips` solo sobre Algoritm, con una copia efímera filtrada para no
volver a guardar los prefabs de la familia: nueve piezas aplicadas por forma y `Cuerpo` apagado,
`orden` sin cambios y 10 clips reescritos. Cada prefab de Algoritm conserva sus 69 objetos y
ningún fileID cambia. `Ojos` y `Boca` siguen apagados y sin sprite, porque la cara está pintada en
el torso: encenderlos habría puesto ojos sobre ojos.

**`Wave`, el saludo de Algoritm (D12).** `ActorAction.Wave = 22`, al final del enum para no mover
los números que guardan los assets; el enum pasa de 22 a 23 acciones (0 a 22) y
`ActionEmotion.For(Wave)` es `Happy`. El clip `char_algoritm_anim_saludar.anim` (4,8 s, bucle,
11 curvas) lo escribe `coreografia.guia_wave` y lo crean, junto con el estado `Wave` de
`char_algoritm.controller`, un script de Editor efímero y el modo `clips`: ese modo no crea clips ni
estados. Es un solo brazo, el derecho de pantalla: sube 94° en 0,55 s, se mece con 4 vaivenes de
0,7 s (codo 27° ± 16°), baja en 0,6 s y descansa unos 0,9 s, con el cuerpo flotando todo el clip. El
parámetro que manda es la cara: `pose_preview.py` exige libre el 99,5 % de la caja de ojos y boca
ampliada un 30 %, y con 94° y 27 ± 16 queda libre el 100 % en las tres formas. Algoritm pasa a
10 clips (94 en total bajo `Characters`: 21 por miembro de la familia y 10 de Algoritm). `Wave` es
solo del guía: a un miembro de la familia le caería a `Idle`.

**Créditos (D12).** `Credits.unity` pierde `Fondo`, `Marco`, `Sombra` y la imagen vacía
`AlgoritmSaluda`; el rig `Algoritm_Fuego(Creditos)` cuelga directo de `AlgoritmPanel` y llena su
ancho de 520 px, centrado en vertical. `CreditsController.guide` (un `CharacterRig`) recibe
`Play(ActorAction.Wave)` en `Start`, escrito como `if (guide != null)` y no con `?.`, que se salta la
comprobación de nulo de Unity. La portada del menú principal también usa los cinco rigs
(`CharacterRig.idlePhase`, etapa 4b): es la escena que más personajes lleva a la vez.

**Pruebas.** `CharacterRig_DA131_AlgoritmSeDibujaPorPartesYSuSpriteEnteroSeApaga` (3 formas),
`CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion` con `Wave` en los tres guías,
`FacialEmotion` con `Wave` a `Happy`, `Credits_RF08_AlgoritmSaludaOcupandoElLugarDeLaTarjetaSinElla`
(PlayMode) y la autoprueba de `pose_preview.py` con el caso de que las piezas suman el sprite. En
EditMode, `CharacterRig` 145/145, `FacialEmotion` 30/30 y `Game.Scaffolding.Tests` 296/296; en
`pose_preview.py`, 10/10 clips por forma de Algoritm y 21/21 por miembro de la familia.

**Pendientes para el arte final de Algoritm (D20).** Santiago decidió no corregir ahora dos defectos
menores de las piezas provisionales; se revisan con el arte que llegue. El corte en nueve piezas es del
arte provisional y lo sustituirá la ingesta del diseño nuevo de Algoritm (INC-136) que hace el carril de
perfil; si ese diseño no solapa piezas o cambia el hombro, los defectos pueden desaparecer solos:

1. **Anillos en los fundidos.** En `Appear`, `Vanish` y `Hidden` las rótulas se ven más oscuras
   unos 0,3 s, porque el alfa del `CanvasGroup` de `Lienzo` multiplica cada `Image` por separado y
   donde dos piezas se solapan el color se oscurece. Con 35 % de alfa quedan anillos oscuros en
   codos, rodillas, hombros y caderas (`capturas/04a-algoritm-creditos/limite-conocido-vanish-alfa-035.png`).
   Es inherente a las piezas solapadas; la familia y su arte final también las solapan.
2. **Talón en el hombro girado.** La rótula copia el contorno del vientre, más claro que el palo, y al
   levantar el brazo asoma un bulto claro de unos 12 px del lienzo, visible ampliando ×6; a escala
   normal pasa por la articulación.

Quedan además `Ojos` y `Boca` apagados hasta que el arte separe la cara, y `pose_preview.py` sigue
dibujando a Algoritm con la maqueta de su sprite entero y no con los PNG de `Frontal/`: hay que
enseñarle a leerlos. El orden de trabajo y los límites están en
`Ajustes-Diseno-2026-10-08/notas-04a-algoritm-creditos.md` (§9 y §10). La entrega de arte de Algoritm
del 09/10/2026 la ingiere el carril de perfil (`545127e` guarda los originales en
`entregas/2026-10-09/Algoritm/`), no esta ronda: el corte en nueve piezas sale del arte provisional
`_reposo` (decisión D22). El prompt para meter el arte final está en
`claudeDocs/tasks/Personajes/Prompt-Arte-Final-Algoritm.md`.

*Nota (09/10/2026): el corte en nueve piezas y los dos defectos de D20 los sustituye la ingesta del arte
final de Algoritm, en siete piezas por forma (C.13, INC-136).*

### C.13 Vista de perfil, rumbo, parpadeo de dos cuadros y el Algoritm final (INC-134 a INC-136, 09/10/2026)

Rama `feat/personajes-animados`, de `ce89320` (el merge de `main`) a `64f3dd1`. Lo que sigue se verificó contra `git show` de cada commit y contra los archivos del árbol, salvo lo marcado como no verificado. El runtime y las herramientas se escribieron en la nube, sin Unity; la ronda 1 del Editor la corrió la sesión local de Santiago. Es el registro de lo hecho; el estado vigente está en `Direccion_de_Arte.md` §7.3, §7.6 y §13.4 y en CLAUDE.md.

> **Verificación en el Editor: rondas 1 y 2 hechas.** Con la ronda 1 (`9966bcb`) los cuatro prefabs de la familia tienen `Lienzo/Perfil`, y `Game.Scaffolding.Tests` pasa 351/351 y `NarrativeScene_` 107/107 en PlayMode. `2cd8a75` cambió después el orden de dibujo del perfil y metió el arte final de Algoritm; la ronda 2 (09/10/2026, `f20b271`) lo aplicó a los prefabs y corrió la suite completa: EditMode 710 de 711 (1 omitida), PlayMode 417/417. Falta, de lo verificable, el build con la medida de RNF-06: ver «Ronda 2 del Editor».

**Decisiones de Santiago Benavides Rey (09/10/2026).** Las tres abren INC propio.

1. **INC-134, perfil al moverse.** Se ve de perfil quien camina, corre, carga, empuja, recoge, se arrodilla o sopla; en todo lo demás, de frente, incluida `Wave`. Lo decide la acción y no el desplazamiento. Mira a la izquierda si va a la izquierda o hacia arriba y a la derecha si va a la derecha o hacia abajo; en una diagonal manda el eje dominante y en un empate, el horizontal. En las narrativas el lado lo decide cada paso (`ActorTimeline.FacesLeftAt`, combinado con el volteo de la casilla); en el río, Mamá sigue las flechas, también las verticales; en el fuego, Papá mira el montón; en el bosque, la Niña mira la caja. El frente nunca se espeja por el rumbo y un rig sin torso de perfil con sprite se queda de frente. Levanta el «ciclo único que se voltea» de `Direccion_de_Arte.md` §13.3.
2. **INC-135, parpadeo de dos cuadros.** La entrega trae ojos abiertos y ojos cerrados, sin párpado a medias; `CharacterFaceSet.Eyes(Half)` cae al cuadro cerrado. Acota §7.3, que pedía tres. Solo parpadea el perfil: faltan los ojos cerrados de frente, pedidos al artista. *(Vencido el 10/10/2026: llegaron, C.15.)*
3. **INC-136, Algoritm nuevo.** El diseño de la entrega del 09/10/2026 —llama, disco de madera y gota, con pantaloneta de cuadros— es el oficial. Siete piezas por forma, con la pierna entera y sin rodilla; escala que conserva la altura de hoy; cara provisional del sprite anterior hasta que el artista entregue la suya. Sustituye al corte provisional en nueve piezas de INC-140. El guion radicado **no se toca** sin otra autorización de Santiago.

Y las decisiones de ingesta de la misma fecha: (a) el ancla horizontal del corte frente↔perfil es la **cadera** (x = 512 del lienzo del rig); (b) el rastro casi blanco del ojo abierto bajo los párpados cerrados **se limpia solo**; (c) la mano lejana de la Niña, que faltaba, **se pide al artista y no se sintetiza**: la entregó el mismo día y entró en `3a0f6ad`, así que la Niña entra de perfil con la pieza real. Y dos correcciones suyas, con el trabajo ya avanzado: (d) **las piernas van siempre detrás del torso** en el perfil —el primer orden dibujaba la pierna cercana delante—, y (e) el arte final de Algoritm **se aplica ya**, sin esperar a la ronda de arreglos.

**Por commit**

| Commit | Qué entró |
|---|---|
| `3efa765` | Entrega original del 09/10/2026 en `claudeDocs/tasks/Personajes/entregas/2026-10-09/` (48 PNG en LFS, fuera de `Assets`): diez piezas de perfil por personaje y dos caras de perfil, ojos abiertos y cerrados. Los cuatro miran a la izquierda; la Niña no traía `mano_atras` y el Niño trae las piezas en una carpeta `Perfil/Niño/` anidada. |
| `910c0f0` | El runtime de INC-134 e INC-135 y sus pruebas: 21 archivos, +1624 −33. Ver «El runtime». |
| `a2d32e2` | `INVENTARIO.md` de la entrega y su `hoja_contacto.png`: papel, recuadro y destino de cada pieza, hacia dónde mira, articulaciones estimadas y memoria. |
| `3a0f6ad` | `mano_atras_niña.png` (el antebrazo con la mano lejano de la Niña), entregado por el artista. |
| `545127e` | Entrega original de Algoritm del 09/10/2026 (21 PNG en LFS): `Fuego/`, `Rueda/` y `Gota/` con siete piezas cada una. |
| `57fbc58` | `Algoritm/INVENTARIO.md` y su hoja de contacto: piezas, articulaciones, escala (opciones A y B), memoria y lo que hay que pedir al artista. |
| `123785e` | `Wave` entra en la tabla de vistas (de frente): `ActionView_INC134_LaTablaCubreTodasLasAcciones` pasa a 23 acciones; `.meta` de los `.cs` nuevos. Lo subió la sesión de arreglos al integrar. |
| `beb1cfe` | Las herramientas de perfil (`preparar_perfil.py`, nuevo, y los modos de perfil de las demás) y los 55 PNG de `Perfil/` que escribió `preparar_perfil.py`. |
| `9966bcb` | Ronda 1 del Editor (sesión local): `BuildRigsFinal` en `estado → perfil → sprites → clips → estado` sobre `e506c82`. `Lienzo/Perfil` en Papa, Mama, Nina y Nino con solo fileID nuevos (82 a 148 bloques, ninguno perdido o cambiado), sprites y capas de cara de perfil cableados, los sets `char_<x>_perfil_cara.asset`, los 84 clips de la familia reescritos y los `.meta` de los 55 PNG. En Algoritm solo aparecen campos nuevos con valor por defecto; sus clips no cambian. |
| `2cd8a75` | La corrección de las piernas (`ORDEN_TRONCO_PERFIL`, la prueba del orden de dibujo, la hoja de `preparar_perfil.py`) y el Algoritm final: `preparar_algoritm.py`, nuevo, con 21 PNG sustituidos en su sitio, seis `antepierna` borradas, la cara provisional en `Expresiones/` y los tres `_reposo` rehechos; el modo `sprites` de `BuildRigsFinal` vacía y apaga el sprite de un nodo sin arte. |
| `64f3dd1` | Merge con `origin` (`9966bcb`). |

**El runtime (`910c0f0`).** Todo en `Game.Scaffolding` salvo los controladores.

- `ActionView` (C# plano, tabla de una línea por acción, como `ActionEmotion`) y el enum `CharacterView { Front = 0, Profile = 1 }`. Perfil: `Walk`, `Run`, `Carry`, `Push`, `PickUp`, `Kneel` y `Blow`; frente: las otras quince acciones de `ActorAction`, `Wave` incluida desde `123785e`. `coreografia.py` lee esa misma tabla del `.cs` (`ACCIONES_PERFIL`), así que no pueden desincronizarse.
- `CharacterRig`: `HasProfile` (existen `Lienzo/Cuerpo` y `Lienzo/Perfil` y el torso de perfil, `Perfil/Tronco/Torso`, tiene sprite; se evalúa al pedirlo, no se cachea) y `View`. `ShowView` enciende un cuerpo y apaga el otro al empezar cada acción, con corte seco, y **siempre**, para que un prefab guardado con los dos encendidos no los enseñe a la vez. Los cuerpos se buscan **por ruta**, como `Tronco`: no hay campo serializado de perfil. `Mirrored` conserva su sentido («mira a la izquierda») pero `Fit` solo voltea el lienzo en perfil. `ArmLayering` y `LimbFollower` siguen ligados al `Tronco` frontal y no cambian.
- `Heading.FacesLeft(Vector2)` devuelve `bool?` (nulo con un vector nulo: no decide nada) y `Heading.FacesLeftToward(fromX, toX, fallback)` es la variante para quien encara un objeto.
- Narrativas: `ActorFacing { Auto = 0, Left, Right }` y el campo `ActorBeat.Facing`, añadido **al final** de los serializados (los 18 assets no se reescriben: sin el campo vale `Auto`). `ActorTimeline.FacesLeftAt(prop, línea)` decide en este orden: el `Facing` explícito; el desplazamiento del paso si mide al menos 0,01; el rumbo del paso anterior; el del siguiente; hacia el centro de la ilustración. `FacesLeftOfStep` hace lo mismo para el paso encadenado (`PendingStep`). `NarrativeSceneController` asigna `rig.Mirrored = facesLeft ^ prop.Mirrored`, para no espejar el perfil dos veces con el volteo de la casilla.
- Mecánicas: `RiverSceneController` usa `Heading.FacesLeft(direction)` siempre que haya movimiento; `FirePanelController.FaceTheFire` pone a Papá de cara al montón antes de recoger, soplar y arrodillarse; `ForestSceneController.FaceCargo` pone a la Niña de cara a la caja y la gira si la caja cruza al otro lado mientras la arrastra.
- `CharacterFace` gana `profileEyes`, `profileMouth` y `profileFaceSet`. **Un solo `BlinkClock` y un solo `MouthFlap`** mueven las dos caras y se actualizan en cada cuadro aunque solo se vea una, de modo que el parpadeo no salta ni se reinicia al cambiar de vista. Una cara sin set o sin sprite se apaga sola sin afectar a la otra.
- `CharacterFaceSet.Eyes(Half)` devuelve `EyesBlinkHalf` si existe y, si no, `EyesBlinkClosed` (INC-135). El cuadro cerrado se ve así los 0,12 s completos y no los 0,04 s del tercio central.

**Orden de dibujo del perfil.** Todo cuelga de `Lienzo/Perfil/Tronco`, con el pivote en la cadera cercana. De atrás adelante: `BrazoLejano/CodoLejano/AntebrazoLejano`, `PiernaLejana/RodillaLejana/AntepiernaLejana`, `PiernaCercana/RodillaCercana/AntepiernaCercana`, `Torso`, `Cuello/Cabeza/{CaraBase, Ojos, Boca}` y `BrazoCercano/CodoCercano/AntebrazoCercano`. **Las dos piernas, detrás del torso**; el brazo cercano y la cara, delante. `Perfil` es hermano de `Cuerpo` y va justo detrás; de frente las piernas ya iban antes de `Tronco`. Esto corrige `Plan-Personajes-Finales.md` §4.3, que ponía la pierna cercana delante del torso. En el Editor, el modo `perfil` reordena los hijos existentes con `SetSiblingIndex`: solo cambia `m_Children` de `Tronco`, ningún fileID.

**Las herramientas** (`claudeDocs/tasks/Personajes/herramientas/`). Cada una conserva su `--autoprueba`.

| Herramienta | Qué hace con el perfil y con Algoritm |
|---|---|
| `preparar_perfil.py` (nuevo) | Entrada del arte de perfil: reconoce las piezas por nombre con tolerancia (sin tildes ni `ñ`, partido por `_` y espacios, recorre subcarpetas, tolera erratas), decide cercana o lejana por el nombre y lo comprueba con el tono (lejana = un solo tono de sombra `#DE9563`; cercana = piel `#FFC69F`), detecta hacia dónde mira y **espeja** si mira a la izquierda, limpia el alfa, normaliza a 870 px con la coronilla en y = 77 y la cadera cercana en x = 512, mide hombro, codo, rodilla, cadera y cuello sobre el alfa y escribe solo en `Perfil/`. |
| `preparar_algoritm.py` (nuevo) | Entrada del arte final de Algoritm: reconoce las siete piezas por forma, exige los tres directorios y 1300 × 1500 en todas, comprueba que las extremidades son la misma silueta en las tres formas (XOR del alfa = 0), las lleva al lienzo del rig con una sola transformación y mide hombro, codo, cadera, rodilla (punto medio del palo visible) y cintura. Con `--aplicar` sustituye en su sitio las 21 partes, borra las seis `antepierna` con su `.meta`, saca la cara provisional del `_reposo` anterior, actualiza `arte_final.json` y `rig_articulaciones.json`, regenera `clips_personajes.json` y corre `coreografia.py --valida`. Con `--reposo` rehace en su sitio los tres `_reposo`. |
| `preparar_expresion.py` | `--vista perfil`: la cara de perfil entra por registro (`perfil.registro`) con su caja aparte, `registro.cara_perfil`, y se separa con `separa_perfil()` (rubor = píxeles rosados; boca = la mayor componente oscura en el 30 % inferior; ojos = el resto), no con `separa()`, que supone un rostro de frente. Alias `abiertos` = `neutra` y `cerrados` = `parpadeo_cerrado`. Sin `--vista perfil` rechaza las imágenes de perfil. |
| `preparar_arte_final.py` | **Rechaza** las entregas de perfil (salida 2, remite a `preparar_perfil.py`) y conserva la entrada `perfil` y `registro.cara_perfil` al reescribir la de frente. |
| `articulaciones.py` y `rig_articulaciones.json` | Clave `perfil` por personaje: 15 nodos, `partes` y `orden_tronco` (`ORDEN_TRONCO_PERFIL`; `ordena_hijos()` lleva cualquier tabla al orden con las piernas detrás), todo colgado de un `Tronco` con el pivote en la cadera. Las entradas `algoritm_<forma>` de `arte_final.json` se vuelcan tal cual, en lugar de las fórmulas provisionales. |
| `coreografia.py` y `clips_personajes.json` | Coreografía de perfil de las siete acciones (piernas que oscilan con la rodilla doblada en el balanceo, brazos a contrafase, tronco inclinado con las piernas compensando, *squash and stretch* ≤ 15 %), con una cinemática (`GeoPerfil`) que planta los pies y lleva las manos por cinemática inversa. **Los doce huesos de perfil quedan fijos en reposo en todos los clips de frente**, para que ninguno caiga a la pose en T, y los de frente no cambian en ningún clip. `--valida` recorre cada clip con esa cinemática. |
| `prefabs.py` | Lee el cuerpo de perfil y resuelve por nombre los `char_<x>_perfil_*` en `Perfil/`. |
| `BuildRigsFinal.cs.txt` | Modo `perfil` (solo **añade** `Lienzo/Perfil` y sus nodos, justo detrás de `Cuerpo` y apagado; crea o reutiliza `char_<x>_perfil_cara.asset` y cablea `profileEyes`, `profileMouth` y `profileFaceSet`; reordena lo existente); `sprites` y `clips` reconocen las rutas de perfil, y `clips` **se niega** a escribir un personaje cuyo prefab no tenga `Lienzo/Perfil`; `sprites` vacía y apaga el sprite de un nodo sin arte (la `AntepiernaX` de Algoritm) y enciende `Ojos` y `Boca` de Algoritm con la cara provisional, cableando su `CharacterFaceSet`; `estado` informa del cuerpo de perfil. El orden con arte de perfil es `estado → perfil → sprites → clips → estado`. |
| `OrganizarArtePersonajes.cs.txt` | `_perfil_` manda sobre `_ojos_`, `_boca_` y `_parte_`: todo lo de perfil va a `Perfil/`. |

**Las dos herramientas nuevas por línea de comandos**

```
python3 claudeDocs/tasks/Personajes/herramientas/preparar_perfil.py <id> <carpeta_entrega>             # informe y composite
python3 claudeDocs/tasks/Personajes/herramientas/preparar_perfil.py <id> <carpeta_entrega> --aplicar   # escribe PNG y tablas
python3 claudeDocs/tasks/Personajes/herramientas/preparar_perfil.py --autoprueba
python3 claudeDocs/tasks/Personajes/herramientas/preparar_perfil.py --hoja <png>                       # hoja de verificación del repo
#   <id> = papa | mama | nina | nino      <carpeta_entrega> = p. ej. entregas/2026-10-09/Nino (con Perfil/ y Expresiones/)
#   --cadera borde|capsula          dónde gira el muslo (por defecto, el borde de arriba)
#   --ancla cadera|torso            el x = 512 del lienzo (por defecto, la cadera cercana: decisión de Santiago)
#   --mira auto|izquierda|derecha   hacia dónde mira la entrega (por defecto se detecta)
#   --sintetiza-lejana              solo si falta una pieza lejana; APAGADO por defecto (--no-sintetiza-lejana)
#   --limpia-cerrados               (por defecto) limpia el rastro del ojo abierto; --no-limpia-cerrados lo apaga
#   --sin-caras                     no corre el paso de las caras (imprime las órdenes)
#   --salida <carpeta>              dónde dejar el composite del informe

python3 claudeDocs/tasks/Personajes/herramientas/preparar_algoritm.py <carpeta_algoritm>               # informe y composite
python3 claudeDocs/tasks/Personajes/herramientas/preparar_algoritm.py <carpeta_algoritm> --aplicar     # escribe partes, cara provisional y tablas
python3 claudeDocs/tasks/Personajes/herramientas/preparar_algoritm.py --reposo                         # rehace en su sitio los tres _reposo
python3 claudeDocs/tasks/Personajes/herramientas/preparar_algoritm.py --autoprueba
#   <carpeta_algoritm> = la de la entrega, con Fuego/, Rueda/ y Gota/ (p. ej. entregas/2026-10-09/Algoritm)
#   --rehaz-cara    extrae de nuevo la cara provisional del _reposo del repo, aunque ya esté escrita
#   --cintura Y     el y (lienzo de la entrega) del pivote de Tronco, en vez de CINTURA_SOBRE_BASE px sobre la base del torso
#   --salida <carpeta>   dónde dejar el composite del informe
```

`preparar_perfil.py --aplicar` escribe **solo** en `Assets/Game/Art/Characters/<Carpeta>/Perfil/` los diez PNG `char_<x>_perfil_{torso,cabeza,brazo_*,antebrazo_*,pierna_*,antepierna_*}`, guarda la clave `perfil` del personaje en `arte_final.json` sin tocar la de frente, corre `articulaciones.py` y `coreografia.py --valida` y después el paso de las caras. Nunca escribe en `Frontal/` ni en `Expresiones/`, ni escribe `.meta`. Si falta una pieza se detiene, nombra el archivo y no escribe nada de ese personaje. Las caras salen como `Perfil/char_<x>_perfil_{ojos_neutra,ojos_parpadeo_cerrado,boca_0,cara_base}` (Papá no tiene `cara_base`: no hay rubor). `preparar_algoritm.py --aplicar` se corre antes que `--reposo`, y ambos antes del modo `sprites`; **nunca escribe `.meta`**.

**Medidas del perfil.** Lienzo del rig de 1024, origen arriba a la izquierda, figura de y = 77 a y = 947 **mirando a la derecha**, cadera cercana en x = 512. «Cercano» es el lado que mira al espectador. Son las de `arte_final.json` tras `preparar_perfil.py`.

| Personaje | Escala de la entrega | Cadera (`Tronco`) | Hombro cercano | Codo cercano | Rodilla cercana | Cuello |
|---|---:|---|---|---|---|---|
| Papá | 0,612676 | (512, 632) | (498, 514) | (491, 632) | (502, 802) | (491, 387) |
| Mamá | 0,625899 | (512, 665) | (506, 555) | (500, 646) | (502, 816) | (498, 492) |
| Niña | 0,598349 | (512, 684) | (510, 541) | (504, 644) | (504, 826) | (514, 478) |
| Niño | 0,614407 | (512, 709) | (494, 569) | (490, 650) | (501, 842) | (502, 506) |

Las piezas se solapan en los extremos redondos; la holgura entre los centros, en px, es de 18,9 en el codo cercano de Papá (22,2 el lejano) y 15,0 en su rodilla, 10,5 y 2,3 en Mamá, 3,0 y 2,4 en la Niña y 1,8 y 5,7 en el Niño.

**Medidas de Algoritm** (las tres formas comparten estas articulaciones, en px del lienzo de 1024). Escala «opción A» `s = 982/1289 = 0,76183`, con `x' = 0,761831·x + 29,714` y `y' = 0,761831·y − 12,521` desde el lienzo de 1300 × 1500: la coronilla del Fuego queda en y = 21, los pies en y = 1003 y el eje del cuerpo en x = 512. Hombros (323, 596) y (704, 598); codos (230, 700) y (805, 697); caderas (413, 837) y (535, 845); cintura, el pivote de `Tronco`, (512, 753). La Rueda y la Gota quedan más bajas, de 818 y 878 px. `Cuerpo` se apaga cuando `Torso`, `BrazoIzq`/`BrazoDer` y `PiernaIzq`/`PiernaDer` tienen sprite; `RodillaX` queda como pivote vacío en el punto medio del palo visible y `AntepiernaX` sin imagen. `IsSegmented` sigue en «no», y a Algoritm le da igual (no tiene `Kneel` ni `Sleep`). La cara provisional va centrada en el cuerpo de cada forma, a la altura relativa a la que estaba en el sprite anterior, y se marca `cara_provisional` en las tres entradas de `arte_final.json`.

**Memoria (RNF-06).** El perfil son 55 PNG (13 de Papá y 14 de cada uno de los otros tres), RGBA sin comprimir y sin mipmaps: **≈ 8,5 MB** (el cálculo sobre los PNG ya normalizados dio 8,14 MB; el inventario de la entrega estimaba 8,68 sin la mano de la Niña). Las 21 piezas de Algoritm suman **6,9 MB** (estimado), y descontado el corte provisional que sustituyen, **≈ 3,4 MB netos**. **Estas cifras eran estimadas; el peso se midió después en un ejecutable (09/10/2026, abajo).** El margen de 21 MB (479,0 MB) es el de rc2, del 01/10/2026, anterior al arte frontal final; perfil y Algoritm lo consumen en unos 12 MB y los ojos cerrados de frente que faltan, unos 1,2 MB, lo reducirán más. Si el build se pasa de 500 MB, la salida propuesta es un tope de 1024 px para `Characters/**`, como hizo INC-130 con el fuego, y **se pregunta antes de aplicarla**.

**Pruebas nuevas** (33 métodos en `910c0f0`, más los cambios de `123785e` y `2cd8a75`)

- INC-134, EditMode: `ActionView_INC134_LaTablaCubreTodasLasAcciones`, `ActionView_INC134_LosGestosHaciaElEstudianteVanDeFrente`, `Heading_INC134_IzquierdaYArribaMiranALaIzquierdaDerechaYAbajoALaDerecha`, `Heading_INC134_UnVectorNuloNoDecideNada`, `Heading_INC134_EncararUnObjetivoMiraHaciaSuLado`, `CharacterRig_INC134_AlMoverseMuestraElPerfilYEnReposoElFrente`, `CharacterRig_INC134_LaVistaLaDecideLaAccionYNoElDesplazamiento`, `CharacterRig_INC134_SinCuerpoDePerfilSiempreDeFrente`, `CharacterRig_INC134_SinArteDePerfilSiempreDeFrente`, `CharacterRig_INC134_SinLienzoNoLanza`, `CharacterRig_INC134_ElEspejoSoloVolteaElPerfil`, `CharacterRig_INC134_ElLadoNoCambiaElTamanoDelLienzo`, `CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte` (un caso por miembro de la familia; comprueba también el orden de dibujo con las piernas detrás del torso y falla si el prefab no tiene el cuerpo de perfil), `CharacterFace_INC134_ElPerfilParpadeaAlCompasDelFrente`, `CharacterFace_INC134_SinCaraDePerfilLaDeFrenteSigueYLaDePerfilNoSeDibuja`, `ActorBeat_INC134_PorDefectoElRumboEsAuto` y nueve `ActorTimeline_INC134_*` (`…ElRumboExplicitoDelPasoMandaSobreElDesplazamiento`, `…ElRumboFijadoRigeUnSoloPasoYElSiguienteVuelveAAuto`, `…SinRumboExplicitoMiraHaciaDondeSeDesplazaElPasoDeLaLinea`, `…UnDesplazamientoMinimoNoDecideElRumbo`, `…SinDesplazamientoEnLaLineaMiraHaciaDondeSeDesplazoAntes`, `…ElRumboFijadoDeUnPasoAnteriorOPosteriorPesaMasQueSuDesplazamiento`, `…SinDesplazamientoPreviosMiraHaciaDondeSeDesplazaDespues`, `…SinNingunDesplazamientoMiraHaciaElCentroDeLaIlustracion`, `…ElPasoEncadenadoMiraHaciaDondeVaSalvoQueNoDecidaNada`).
- INC-135, EditMode: `CharacterFaceSet_INC135_SinCuadroMedioElParpadeoUsaElCerrado` y `CharacterFaceSet_INC135_ConCuadroMedioSigueSiendoElMedioYSinNingunoNoSeInventaNada`.
- INC-136, EditMode: `CharacterRig_DA131_AlgoritmSeDibujaPorPartesYSuSpriteEnteroSeApaga` (3 formas) pasa a exigir las siete piezas, la `AntepiernaX` sin sprite y apagada, y `Ojos` y `Boca` con la cara provisional de la forma; **falla hasta que corra el modo `sprites` sobre los prefabs de Algoritm**.
- PlayMode: `NarrativeScene_INC134_QuienSeDesplazaVaDePerfilHaciaDondeCamina` (18 casos, uno por secuencia), `RiverScene_INC134_MamaVaDePerfilAlCaminarYDeFrenteAlSoltarLaFlecha`, `FireLevel_INC134_PapaMiraAlMontonAlRecoger`, `FireLevel_INC134_PapaMiraAlMontonAlSoplarYAlArrodillarse`, `FireLevel_INC134_PapaGolpeaDeFrenteSinEspejo` y `ForestScene_INC134_LaNinaMiraALaCajaMientrasLaEmpujaYSeGiraSiLaCajaCruza`. Las que dependen de `HasProfile` usan `Assume.That`: sin perfil en el prefab se **omiten**, no se aprueban.
- **Cambia una prueba existente, a propósito:** `RiverScene_DA133_MamaCaminaMientrasSeSostieneUnaFlechaYReposaAlSoltarla` recorre ahora las cuatro flechas y exige `Mirrored` a la izquierda con ← y ↑ y a la derecha con → y ↓.

**Corridas**

| Corrida | Alcance | Resultado |
|---|---|---|
| Fuera del Editor | `--autoprueba` de `preparar_perfil.py`, `articulaciones.py`, `preparar_arte_final.py`, `preparar_expresion.py` y `preparar_algoritm.py`; `coreografia.py --valida` | pasan (según los mensajes de `beb1cfe` y `2cd8a75`; error máximo de las articulaciones recuperadas: 1,5 px en el cuello) |
| `eb9be13`, Editor, antes de generar los prefabs | suite integrada, 1128 corridas | EditMode 706/711 (los 4 rojos esperados, `CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte`, y 1 omitida por entorno); PlayMode 416/417 (1 inconclusa, `RiverScene_INC134_…`, por el `Assume`) |
| `9966bcb`, Editor | `Game.Scaffolding.Tests` | 351/351 |
| `9966bcb`, Editor | `tests-play NarrativeScene_` | 107/107 |
| `64f3dd1`, Editor (ronda 2) | modos `estado → perfil → sprites → clips → estado`; comparación de los prefabs objeto por objeto | ningún fileID perdido ni cambiado |
| Árbol de la ronda 2, Editor | `Game.Scaffolding.Tests` | 348/351: las 3 rojas, `CharacterRig_DA131_AlgoritmSeDibujaPorPartesYSuSpriteEnteroSeApaga`, una por forma (ver «Ronda 2 del Editor») |
| Árbol de la ronda 2, Editor | `tests-play`: `NarrativeScene_` 107/107, `RiverScene_` 23/23, `Fire` 76/76, `Forest` 40/40, `Credits` 6/6, `MainMenu` 19/19 y `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes` 5/5 | pasan |
| `b4aed0e`, Editor | `estado` sin referencias a sprites borrados en los siete prefabs; `sprites` idempotente; `Game.Scaffolding.Tests` | siete prefabs byte a byte iguales al repetir; 351/351 |
| `b4aed0e`, Editor | `suite2.ps1`, 23,9 min | EditMode 711 (710 pasan, 0 fallan, 1 omitida); PlayMode 417/417; cobertura 1128/1128 |

**Ronda 2 del Editor (hecha, 09/10/2026).** Sesión local en el equipo de Santiago, sobre `64f3dd1`. Quedó como sigue; lo que falta va marcado.

- [x] `git pull` de la rama y `editor.ps1 recompile`.
- [x] Modos `estado → perfil → sprites → clips → estado` de `BuildRigsFinal` (sin `nodos` ni `orden`). `perfil` reordenó `Perfil/Tronco` en Papá, Mamá, Niña y Niño: las dos piernas quedan detrás del torso y solo cambia el orden de hermanos. `sprites` aplicó el arte final de Algoritm en los tres prefabs, con siete piezas por forma y la cara provisional (`Ojos`, `Boca` y `char_algoritm_<forma>_cara.asset` en `CharacterFace.faceSet`).
- [x] Comparación objeto por objeto contra `HEAD`: ningún fileID perdido ni cambiado.
- [x] Pruebas. `Game.Scaffolding.Tests` dio primero 348/351 (ver abajo); tras la corrección, 351/351. PlayMode: `NarrativeScene_` 107/107, `RiverScene_` 23/23, `Fire` 76/76, `Forest` 40/40, `Credits` 6/6 y `MainMenu` 19/19.
- [x] Suite completa con `suite2.ps1` (23,9 min), sobre `b4aed0e`: EditMode 711 (710 pasan, 0 fallan, 1 omitida), PlayMode 417/417, cobertura 1128/1128.
- [x] Capturas en `claudeDocs/tasks/Personajes/capturas/2026-10-09/` (`0212469`): 144, de ellas 139 líneas narrativas de `NarrativeScene_RF05_CapturaCadaLineaConLosPersonajes` y las 5 mecánicas de `Personajes_DA133_CapturaCadaMecanicaConSusPersonajes` (5/5). Van en JPEG de calidad 90 a resolución completa (26,7 MB); los PNG originales, 183 MB, quedaron fuera del repositorio para no inflar el LFS. No hay capturas de los créditos ni del menú: sus grupos de PlayMode pasan.
- [x] Revisión de una muestra de las capturas (orquestador). De perfil al recoger y al arrodillarse, mirando hacia su lado y con las piernas detrás del torso (`Personajes_N1_Hallazgo_L16`, Papá arrodillado; `Personajes_N2_PuenteI_Bosque_L01`, Mamá, Niña y Niño recogiendo). De frente en reposo en las cinco mecánicas. Algoritm con el diseño final y la cara provisional en sus tres formas (`Personajes_N1_AparicionGuia_L07`, llama; `Personajes_N2_Escena21_Bosque_L01`, disco; `Personajes_N3_Escena31_Llegada_L02`, gota), sin los anillos de las rótulas de D20 porque la pierna es entera. **Límites:** la prueba captura cada línea cuando su movimiento ya terminó, así que no hay captura de una caminata a mitad de paso (el perfil al caminar lo cubre `NarrativeScene_INC134_QuienSeDesplazaVaDePerfilHaciaDondeCamina`); el talón claro del hombro girado (D20) no se ve en reposo y **sigue sin comprobarse**; una captura fija no muestra el parpadeo.
- [x] Compilar el ejecutable y medir el peso con `oe4.ps1 Tamano` (RNF-06). Build con `editor.ps1 exec build` (sesión local, 09/10/2026) sobre `0212469`: **499 314 282 bytes = 499,3 MB (476,2 MiB)**, frente al tope de 500 MB, con 0,7 MB de margen y +20,3 MB sobre rc2 (479,0 MB). El informe del build: texturas 342,5 MB (95,9 %); `Characters/` suma 35,8 MB (`Frontal/` 13,5 MB en 61 archivos, `Perfil/` 8,2 MB en 59, `Expresiones/` 3,7 MB en 25 y el resto 10,4 MB en 107). El tope no se pasó, así que no hubo que preguntar por el de 1024 px a `Characters/**`: lo más pesado eran cinco props del N1 a 2000 × 2000 (15,3 MB cada uno), y **Santiago decidió (09/10/2026) dejarlos en 256 × 256** (INC-146, `83fd986` y reimport en `4062ae3`). Un build nuevo sobre `4062ae3` pesa **420 561 594 bytes = 420,6 MB (401,1 MiB)**, 78,7 MB menos, con 79,4 MB de margen. Capturas de comprobación en `claudeDocs/tasks/Personajes/capturas/2026-10-09-props256/` (la mecánica de la cueva, `N1_NacimientoDelFuego` L00 a L16 y `N1_Hallazgo` L15 y L16): sin diferencia apreciable frente a `2026-10-09/`, tampoco en los montones. `ArtImport_` 4/4 y PlayMode `Fire` 76/76. Los dos builds son medidas, no candidatos: rc2 sigue siendo el candidato del OE4.
- [ ] Medir RNF-04 (carga) y RNF-05 (memoria) en un ejecutable. **Pendiente:** el arnés exige el Editor cerrado y jugar el ejecutable; queda para el próximo candidato del carril OE4.
- [x] Commit de lo generado: `f20b271`, con los siete prefabs y `Algoritm/Expresiones/` (los `.meta` de las seis caras y los tres `_cara.asset`). El andamiaje de `Assets/Editor/` se borró.

**Las tres correcciones del generador.** Las 3 pruebas rojas de `Game.Scaffolding.Tests` eran `CharacterRig_DA131_AlgoritmSeDibujaPorPartesYSuSpriteEnteroSeApaga` (una por forma): la `AntepiernaX` conservaba la referencia al PNG borrado de la antepierna.

1. `ad4fd58`: limpiar la referencia por la propiedad serializada. No compiló en Unity 6.5, porque `SerializedProperty.objectReferenceInstanceIDValue` es obsoleto con error en favor de `EntityId`.
2. `24ce278`: detectarla por el «nulo falso» de `Image.sprite`. Dio falsos positivos: en el Editor, todo campo `UnityEngine.Object` serializado y sin asignar es también un nulo falso, así que marcó las antepiernas ya limpias y el `CaraBase` de perfil de Papá, vacío a propósito porque no lleva rubor.
3. `b4aed0e`: la referencia rota se busca en el YAML del prefab, como un GUID de `m_Sprite` que `AssetDatabase.GUIDToAssetPath` no resuelve; la prueba DA131 usa el `==` de Unity y comprueba el archivo.

*Lección (09/10/2026).* En Unity 6.5, `== null` y `Image.sprite` devuelven el «nulo falso» tanto para una referencia rota como para un campo serializado sin asignar, y `objectReferenceInstanceIDValue` ya no compila: para saber si un prefab apunta a un sprite borrado se lee el archivo, no el objeto en memoria.

**Puntos abiertos.**

- **Ojos cerrados de frente** de los cuatro, en el lienzo del 06/10/2026: sin ellos el frente no parpadea (INC-135). Pedidos al artista.
- **La cara de Algoritm** (`ojos_neutra`, `ojos_parpadeo_cerrado` y `boca_0` como mínimo, sobre el lienzo de 1300 × 1500): hasta entonces, la del sprite anterior, marcada `cara_provisional` y sin parpadeo ni habla. Pedida al artista.
- **El riesgo del Nivel 2:** el disco de madera se parece a la sección de un tronco y está cerca del acento `#C79A5E`; sin cara se lee como un prop. Hay que verlo en la captura del bosque.
- **Los tonos de piel y el contorno negro** del arte final no son los de `Direccion_de_Arte.md` §4.1; decisión de Santiago.
- **Los radicados** que describen el diseño anterior de Algoritm (guion §1.1.1, las notas de las dos escenas puente, el trabajo de grado) esperan su autorización; `Interfaces.md` §4.3 y la dirección de sonido §6.1, que también lo describían, se revisaron en `1e2fb6a`.
- **Peso y memoria** del arte de perfil y de Algoritm: el peso (RNF-06) está medido, 499,3 MB y, con INC-146, 420,6 MB (ver la casilla de la ronda 2); **la carga (RNF-04) y la memoria (RNF-05) siguen sin medir** en un ejecutable y quedan para el próximo candidato del OE4. Tampoco incluye los ojos cerrados de frente que faltan.
- **La republicación en Word del OE3**: sus fuentes (`claudeDocs/entregables/OE3/src/`, Anexo F apartado 2.13 y tablas) ya nombran INC-134 a INC-136 desde `1e2fb6a`, pero los `.docx` publicados todavía no.
- Siguen sin ejecutarse el retrato animado del cuadro de diálogo y las emociones de las 18 narrativas.

### C.14 Los brazos de Algoritm, detrás de todo el cuerpo (INC-147, 09/10/2026)

Commit `a44ca96` (rama `feat/personajes-animados`, sobre `d89e5fb`). Lo que sigue se verificó contra `git show a44ca96` y contra los archivos del árbol.

**La decisión de Santiago (09/10/2026).** Los brazos de Algoritm se dibujan **detrás de todo el cuerpo**: eligió «Detrás de todo el cuerpo» frente a «sobre el cuerpo, bajo la cara». El orden bajo `Lienzo/Cuerpo/Tronco`, de atrás adelante, pasa de `Torso`, `Ojos`, `Boca`, `BrazoIzq`, `BrazoDer` (INC-132, 05/10/2026, «las manos encima de la cara») a `BrazoIzq`, `BrazoDer`, `Torso`, `Ojos`, `Boca`. **Revierte para el guía la cláusula de Algoritm de INC-132** (C.9); la regla de la familia no cambia (INC-132 e INC-133: húmero detrás del torso, antebrazo delante, los brazos delante solo mientras golpean con `ArmLayering`).

**Qué entró en `a44ca96`** (15 archivos, +178 −89):

| Archivo | Qué cambió |
|---|---|
| `articulaciones.py` y `rig_articulaciones.json` | `ORDEN_TRONCO_GUIA` y la clave `orden_tronco` de las tres formas llevan el orden nuevo. |
| `BuildRigsFinal.cs.txt` | Solo comentarios: el modo `orden` ya reordenaba los hijos de `Tronco` según `orden_tronco` con `SetSiblingIndex` (sin tocar fileIDs), así que aplicará el orden nuevo a los tres prefabs sin cambiar su código. |
| `preparar_algoritm.py` | Compone los `_reposo` desde ese orden y reescribió `char_algoritm_n1_fuego_reposo.png`, `char_algoritm_n2_rueda_reposo.png` y `char_algoritm_n3_gota_reposo.png` en su sitio (mismos nombres y GUID, 768 × 768, mismo alfa): solo cambian los casquetes de los hombros, que ya no se pintan sobre el cuerpo. |
| `preparar_arte_final.py` | Corrige `_mismos_pixeles` (ver la lección). |
| `pose_preview.py` | La prueba mide el antebrazo del guía (al menos el 85 %) y el húmero (al menos el 30 %) como los de la familia (INC-133); la medida de la cara se conserva y da el 100 % en las tres formas. `EXCEPCIONES_BRAZO` registra tres excepciones, todas de `algoritm_rueda` (abajo). |
| `coreografia.py` y `prefabs.py` | Solo comentarios. |
| `ArmLayering.cs`, `CharacterRig.cs`, `LimbFollower.cs` | Solo comentarios: el runtime no cambia. |
| `CharacterRigTests.cs` | Dos pruebas renombradas y reescritas (abajo). |

**Las excepciones de la rueda.** Con el brazo en alto, el húmero desaparece detrás del disco de madera, que es ancho, y solo se ven el antebrazo y la mano. `EXCEPCIONES_BRAZO` deja de estar vacía: `Celebrate` (antebrazo 81,5 %, húmero 1,4 %), `Encourage` (77,8 % y 3,8 %) y `Wave` (48,2 % y 3,7 %), con pisos de 75, 75 y 45 % para el antebrazo y de 0 % para el húmero. Se acepta como consecuencia de la decisión de Santiago; la mano se sigue leyendo. Las formas de fuego y de gota cumplen los umbrales sin excepción.

**Pruebas** (`Game.Scaffolding.Tests`, `CharacterRigTests.cs`).

| Prueba | Antes | Qué comprueba |
|---|---|---|
| `CharacterRig_INC147_LosBrazosDeAlgoritmVanDetrasDeTodoElCuerpo` (las tres formas) | `CharacterRig_INC132_AlgoritmPintaLasManosEncimaDeLaCara` | `BrazoIzq` y `BrazoDer` van antes de `Torso`, y `Ojos` y `Boca` después. |
| `CharacterRig_INC147_EnAlgoritmGolpearVuelveAlOrdenDeOrigen` (las tres formas) | `CharacterRig_INC132_EnAlgoritmElGolpeNoCambiaElOrdenDeDibujo` | Algoritm no tiene el estado `Strike`, pero `armsInFrontActions` lo incluye en los siete prefabs; con los brazos antes del torso, `ArmLayering` sí los movería. La prueba comprueba que cambia el orden al golpear, que las manos nunca pasan por encima de la cara, que al terminar vuelve el orden de origen y que devolverlo dos veces no lo cambia. |

Las dos fallaban hasta correr el modo `orden` sobre los prefabs de Algoritm (estaban escritas para pasar justo después); pasan desde `ca62b9a` (abajo). `CharacterRig_INC133_EnAlgoritmElAntebrazoSigueBajoElCodoYSinAncla` no cambia, solo su comentario.

*Lección (09/10/2026).* En Pillow 12, `getbbox()` sobre una imagen RGBA solo mira el alfa. `_mismos_pixeles` (`preparar_arte_final.py`, que usan los preparadores para decidir si una imagen cambió) comparaba con él, así que dos imágenes con el mismo alfa y distinto color parecían iguales y los `_reposo` rehechos con los brazos detrás del cuerpo no se habrían escrito. Ahora se compara canal por canal. Una comparación de imágenes que debe notar un cambio de color no puede apoyarse en `getbbox()` de RGBA.

**Ronda del Editor del modo `orden` (hecha, 09/10/2026, `ca62b9a`).** Sesión local en el equipo de Santiago, sobre `a44ca96`. Quedó como sigue.

- [x] Modo `estado` antes de empezar: «NO cumple la tabla» en los tres prefabs de Algoritm; la familia ya estaba en orden.
- [x] Modo `orden` de `BuildRigsFinal`: reordenó `Tronco` en `Algoritm_Fuego`, `Algoritm_Rueda` y `Algoritm_Gota` de `Torso`, `Ojos`, `Boca`, `BrazoIzq`, `BrazoDer` a `BrazoIzq`, `BrazoDer`, `Torso`, `Ojos`, `Boca`. Modo `estado` después: «cumple la tabla».
- [x] Comparación objeto por objeto contra `HEAD`. Familia: ningún cambio. Algoritm, en las tres formas: 69 → 69 bloques, ningún fileID añadido, perdido ni cambiado; cambia solo el orden de `m_Children` del `Transform` de `Tronco`. Los `.meta` de los tres `_reposo` no cambiaron.
- [x] Pruebas. `Game.Scaffolding.Tests` 351/351, con `CharacterRig_INC147_LosBrazosDeAlgoritmVanDetrasDeTodoElCuerpo` y `CharacterRig_INC147_EnAlgoritmGolpearVuelveAlOrdenDeOrigen` (las tres formas, en verde). PlayMode: `Credits` 6/6, `MainMenu` 19/19 y `NarrativeScene_` 107/107. No se corrió `suite2.ps1`: solo cambió el orden de los hijos de tres prefabs y tres PNG.
- [x] Capturas en `claudeDocs/tasks/Personajes/capturas/2026-10-09-algoritm-brazos/`: 26 (JPEG de calidad 90, 4,83 MB), `N1_AparicionGuia` L00 a L10, `N2_Escena21_Bosque` L00 a L06 y `N3_Escena31_Llegada` L00 a L07.
- [x] Revisión visual de las 26 capturas (orquestador). En las tres formas los brazos salen por detrás del cuerpo y ninguna mano tapa la cara.
- [x] Commit de lo generado: `ca62b9a`, con los tres prefabs y las 26 capturas.

**Corrección de `pose_preview.py` (`0220b74`, 09/10/2026, INC-134).** Desde `9966bcb`, cuando el cuerpo de perfil entró en los prefabs, la prueba dibujaba a la familia con **los dos cuerpos a la vez** (`Lienzo/Cuerpo` y `Lienzo/Perfil`), y medía capas del cuerpo que el motor mantiene apagado. De ahí salían 74 filas de la familia y 6 casos de la autoprueba en rojo: eran un fallo de la herramienta, no de los clips ni de los prefabs, y no deben leerse como fallos preexistentes del arte. `Personaje` construye ahora una vista por cuerpo y elige una por acción desde `ActionView.cs`, como el motor (un rig sin arte de perfil, Algoritm, se queda de frente). Las acciones de perfil se miden con los nodos del perfil: brazo cercano, cara, codo y rodilla por rotación, suelo y velocidad; el brazo lejano solo se informa, y la comprobación de que no se tapen los dos ojos se omite de perfil. Corrida completa: 0 fallos (antes 74); autoprueba: 0 (antes 6). La Figura F.2 del Anexo F se redibujó con el cuerpo frontal solo (`f290c0a`).

### C.15 Expresiones según el guion, tarjeta del diálogo animada, caras a 256 px y el efecto espejo en la fogata (INC-148 a INC-150, 10/10/2026)

Rama `feat/personajes-animados`, de `2c0f18b` a `8602c79`. Lo que sigue se verificó contra `git show` de cada commit y contra los archivos del árbol, salvo lo marcado como no verificado. El runtime, las pruebas y las herramientas se escribieron en la nube, sin Unity; la ronda del Editor (`23d7e8c`) la corrió la sesión local de Santiago, y sus cifras están en «Ronda del Editor». Es el registro de lo hecho; el estado vigente está en `Direccion_de_Arte.md` §7.3, §7.6, §10.3 y §15.2 y en CLAUDE.md.

**Decisiones de Santiago Benavides Rey (10/10/2026).** Las tres abren INC propio.

1. **INC-148, «Usa cada expresión según el guion».** Cada línea de las 18 narrativas lleva la expresión que el guion pide. La tarjeta del cuadro de diálogo pasa de la imagen fija al personaje animado: eligió la opción «Expresión, parpadeo y boca», con el encuadre «cabeza y hombros, igual para todos».
2. **INC-149, «todo el arte de esta entrega, al importarse, recortado y como máximo 256×256».** Vale para las expresiones de frente y de perfil, la cara de Algoritm y los retratos. El cuerpo no cambia.
3. **INC-150, «aplica efecto espejo».** Papá se arrodillaba junto a la fogata de espaldas a ella; se corrige como dato.

**La entrega (D11-11, `636467a`; inventario en `b163afd`).** 45 PNG de 1300 × 1500 de Sofía Valentina Giraldo Segovia, en el mismo lienzo registrado que las partes del 06/10/2026, en `claudeDocs/tasks/Personajes/entregas/2026-10-10/` (LFS).

| Qué | Familia | Algoritm |
|---|---|---|
| Neutra, concentración, preocupación, sorpresa | Sí; la neutra es idéntica a la del 06/10/2026 | Sí; una sola cara para las tres formas |
| Boca de hablar | Una (abierta) | Una |
| Ojos cerrados **de frente** | Sí (los primeros) | Sí |
| Caras de perfil, abierta y cerrada | Idénticas a las del 09/10/2026 | No tiene perfil |
| **Alegría y sueño** | **No llegaron** | **No llegaron** |
| El personaje entero para la tarjeta | `Dialogos/dialogo_<x>.png` | Una por forma en `Algoritm/Dialogos/` |

Los nombres irregulares (`exoresion_…_papa`, `expresion_perfil _neutra…niña`, `papa_neutro_boca_ abierta`, `algoritm_nuetro_boca_abierta`) se casan por fichas NFKD, como el 09/10/2026. El inventario midió con Pillow: el retrato de Mamá trae una diferencia de pelo y hombros fuera de la cara (40,8 de media) que se revisó en la hoja de retratos, y el del Niño trae la boca abierta pintada. Las herramientas notaron además que **la neutra de la Niña** tiene el iris naranja y un rubor que sus otras cinco expresiones no tienen, y que **las cejas del Niño** difieren un poco: se pedirá una neutra nueva de los dos.

**Por commit**

| Commit | Qué entró |
|---|---|
| `636467a` | La entrega original, sin tocar, en `entregas/2026-10-10/`. |
| `b163afd` | `INVENTARIO.md` con el mapa de nombres, las cajas unión de la cara, el registro de los retratos con la parte sin cara y la memoria estimada; `hoja_contacto.png`. |
| `ce92c0f` | `herramientas/emociones_narrativas.json`: las emociones de la cara por línea, sacadas del guion (`Solucion_OE2_Diseno_final.docx`, §1.3 a §1.6, leído con python-docx), con 189 emociones y 4 voces fuera de escena, más los rumbos del efecto espejo. |
| `146f4f3` | El runtime y sus pruebas: 16 archivos, +1226 −34. Ver «El runtime» y «Pruebas». |
| `cc938e9` | El arte a 256 px (caras, perfil, retratos y los tres `_reposo` de Algoritm), las herramientas y las 18 narrativas con las emociones y los rumbos aplicados: 112 archivos, +4628 −292. |
| `292b108` | Los retratos de la familia pasan a primer plano y sin brazos, por una petición de Santiago al ver las tarjetas: «en los diálogos recorta los brazos, que quede en un primer plano el personaje». Sobre Algoritm dijo «cuando es Algoritm quien habla, como está actualmente está bien», así que conserva el encuadre abierto con brazos. |
| `23d7e8c` | La ronda del Editor: prefabs, clips y capturas generados (ver «Ronda del Editor»). |
| `8602c79` | Quita del repositorio los seis PNG de la cara provisional de Algoritm. |

**El runtime** (`Assets/Game/Scripts/Runtime/`, `146f4f3`).

| Archivo | Qué cambió |
|---|---|
| `Scaffolding/PortraitLook.cs` (nuevo, C# plano) | Decide si la tarjeta es animada o cae al retrato fijo: hace falta una base, un set con `EyesNeutral` y un recuadro de cara dentro del cuadro unidad. |
| `Scaffolding/CharacterRig.cs` | Campos serializados nuevos **al final**, `PortraitBase` (Sprite) y `PortraitFace` (Rect normalizado, origen abajo a la izquierda), y el getter no serializado `FaceSet`. `Portrait` no cambia de nombre ni de uso: lo siguen usando `MainMenuTests`, el botón de ayuda y el respaldo de los props. Los siete prefabs se guardaron sin los campos nuevos; hasta que la ronda del Editor los escribe, la tarjeta sigue con el retrato fijo. |
| `Scaffolding/CharacterFace.cs` | `FaceSet { get; set; }` (redibuja y rehace los relojes, sin saltos si el set no cambia) y `Bind(Image eyes, Image mouth)` para una cara armada en tiempo de ejecución. Sin campos serializados nuevos. |
| `Scaffolding/CharacterFaceSet.cs` | `Eyes(Sleeping)` cae a `EyesSleeping`, luego a `EyesBlinkClosed` y luego a `EyesNeutral`: dormir es tener los ojos cerrados. |
| `Scaffolding/MouthFlap.cs` | `CycleFor(hasA, hasE, hasU)` arma el ciclo con las bocas que el set tiene más la cerrada; con solo `A`, alterna abierta y cerrada cada `FlapSeconds`. El constructor clásico A, E, U y cerrada se conserva. |
| `Scaffolding/ActorTimeline.cs` | `EmotionOf(prop, line, speaking)` es la emoción del guion en esa línea o, si ningún paso la fija, la de la acción (`ActionEmotion`), resuelta desde los datos y no desde el estado del rig. `HeldBefore` pasa a `internal` para que las pruebas de datos reconozcan el paso que solo fija la emoción. |
| `Scaffolding/DialogueLine.cs` | `SetsVoiceEmotion` y `VoiceEmotion` al final, solo para las 4 voces cuyo hablante no está en escena. |
| `UI/NarrativeSceneController.cs` | `EnsurePortraitFace()` arma `Arte/Cara` (con `Ojos`, `Boca` y `CharacterFace`) en tiempo de ejecución, como `EnsureDarkness`: **`Narrative.unity` no se edita**. `ShowPortrait` resuelve al hablante en el orden de siempre (en escena, `cast`, `guideByLevel`), pone la base, ancla la cara a `PortraitFace`, asigna el set y la emoción de la línea y activa el habla; `Leave()` la calla y cada hablante nuevo reinicia el parpadeo. |
| `Editor/ArtImportRules.cs` | La tercera excepción de importación (INC-149): `CharacterFacePaths`, una expresión regular sobre la ruta, y `CharacterFacesMaxTextureSize` = 256, sin comprimir. |

**No hay máquina de escribir.** El texto del cuadro sale entero y `Speaking` dura hasta que el estudiante avanza, así que la boca de la tarjeta hace lo mismo que la del personaje en escena: se mueve mientras la línea está en pantalla.

**Las herramientas** (`claudeDocs/tasks/Personajes/herramientas/`, `cc938e9`).

| Herramienta | Qué hace |
|---|---|
| `preparar_expresion.py` | Alias nuevos (`neutro`, `concentrado`, `preocupado`, `neutro_boca_abierta` → `boca_a`, `neutro_ojos_cerrados` → `parpadeo_cerrado`). Modo por lotes `--entrega <dir>`: la caja común es la **unión** de las seis expresiones más 3 px, con una sola escala y `--max-lado` 256. El perfil se trata igual. Para Algoritm escribe una cara compartida en `Algoritm/Expresiones/char_algoritm_{ojos_*,boca_*}.png` (10 texturas en lugar de 30). |
| `preparar_retrato.py` (nuevo) | Comprueba el registro (la IoU del alfa con la cabeza sin cara debe ser ≥ 0,97; si no, rechaza), borra la cara pintada, hornea la nariz y el rubor en `cara_base`, recorta un cuadrado con la cabeza del mismo tamaño en cada grupo: la familia en primer plano y sin brazos (la cabeza ocupa 0,78 del lado, con 0,05 de aire arriba; los brazos y las manos se borran fuera del cuerpo) y Algoritm con su encuadre abierto (0,62 y 0,07, con los brazos), por dos decisiones de Santiago del mismo día y escribe 256 × 256: `char_<x>_retrato_base.png` y, para la familia, `char_<x>_retrato_neutra.png` reescrito en su sitio (mismo GUID). Guarda el `retrato` en `arte_final.json`. Incluye `--hoja`, `--valida` y `--autoprueba`. |
| `preparar_algoritm.py --reposo` | Rehace los tres `_reposo` con la cara final (mismo nombre y GUID). Una guarda impide que una corrida nueva vuelva a la cara provisional. |
| `emociones_narrativas.py` | Aplica la tabla a los 18 assets. Pone la emoción en el paso que ya existe en esa línea o añade un paso que solo la fija y repite la acción sostenida y el rumbo vigentes; nunca lo añade con una caminata en curso, salvo `FinishesSteps`; quien fija una emoción la fija en todos sus pasos siguientes; solo reescribe el bloque `<Beats>` de los props tocados y respeta los escapes `\xE1`. Modos `--valida` (en seco: casa el texto citado con la línea del asset, sin tildes, y revisa cobertura, choques con caminatas y fuego), `--aplicar` y `--autoprueba`. |
| `BuildRigsFinal.cs.txt` y `articulaciones.py` | Modo nuevo `retrato`, que escribe `PortraitBase` y `PortraitFace` por `SerializedObject` (solo cambian valores, ningún fileID); `FaceNames` busca primero el prefijo de la forma y después `char_algoritm`. |

**Las emociones del guion.** Solo se usan `Neutral`, `Happy`, `Surprised`, `Worried`, `Focused` y `Sleeping`; no hay cara de derrota (CP-02). En la 3.2, tras el primer fallo, nadie queda en `Worried` al cerrar. `Happy` se ve con la cara neutra hasta que llegue `alegria`; `Sleeping`, con los ojos cerrados. Las 4 voces fuera de escena (de 84 líneas habladas) llevan su emoción en `DialogueLine.VoiceEmotion`.

**El efecto espejo (INC-150).** `ActorTimeline.FacesLeftAt` caía, sin `Facing` declarado, a su última regla («hacia el centro de la ilustración», `Position.x ≥ 0,5` a la derecha). Papá, arrodillado junto a la llama (en x 0,215) en `N1_NacimientoDelFuego`, quedaba de espaldas a ella en las líneas 0 a 4 y 8 a 11; el mismo defecto tenían Mamá (líneas 0 a 5) y la Niña (0 y 1) en `N2_Escena25_Cierre`. Se corrigió como dato: `Facing: Left` en esos pasos y un paso nuevo en la línea 0 de Papá. La regla no cambia.

**Pruebas** (nombres tal como están en el árbol).

| Prueba | Qué comprueba |
|---|---|
| `CharacterFaceSet_INC148_DormidoSinSusOjosUsaLosCerradosDelParpadeo` | `Sleeping` cae a los ojos cerrados. |
| `MouthFlap_INC148_ConSoloLaBocaAbiertaAlternaAbiertaYCerrada` y `CharacterFace_INC148_HablandoConSoloLaBocaAbiertaAlternaCadaFlapSeconds` | El habla con una sola boca. `MouthFlap_DA73_ConLasTresBocasSigueElCicloAEUCerrada` conserva el ciclo clásico. |
| `CharacterFace_INC148_CambiarDeSetCambiaLaCaraYSusRelojes` | `FaceSet` rehace los relojes. |
| `PortraitLook_INC148_SinBaseOSinCaraCaeAlRetratoFijo` y `…_ConBaseYCaraEsAnimado` | La decisión de la tarjeta. |
| `ActorTimeline_INC148_LaEmocionDelRetratoEsLaDelGuionYSiNoLaDeSuAccion` | `EmotionOf`. |
| `CharacterRig_INC148_CadaPersonajeTieneRetratoAnimadoConLaCaraDentro` | Los siete prefabs: base cuadrada ≤ 256, recuadro dentro de la base, set con `EyesNeutral`, `EyesBlinkClosed` y `MouthA`. Pasa solo cuando la ronda del Editor escribe los campos nuevos: los prefabs se guardaron sin ellos. |
| `CharacterFace_INC148_LasTresFormasDeAlgoritmCompartenLaCara` y `CharacterRig_INC135_LaFamiliaParpadeaDeFrente` | La cara compartida y el parpadeo de frente. |
| `NarrativeSequence_INC148_CadaLineaHabladaLlevaLaEmocionDelGuion`, `…_SoloLasVocesFueraDeEscenaDeclaranEmocionEnLaLinea`, `…_QuienFijaUnaEmocionLaFijaEnSusPasosSiguientes`, `…_UnPasoQueSoloFijaLaEmocionNoInterrumpeNiGira` y `…_LasEmocionesDelGuionEnLineasClave` | Los datos de las 18 narrativas; la última, con casos anclados al texto del guion. |
| `NarrativeSequence_INC150_QuienTrabajaJuntoAlFuegoLoMira` | Antes fallaba en 17 instancias de línea con un personaje de espaldas al fuego; ahora, en ninguna. |
| `ArtImport_RNF06_LasCarasYLosRetratosDeLosPersonajesSeImportanA256SinComprimir` | La tercera excepción; la prueba de RNF-23 omite esas rutas. |
| `NarrativeScene_INC148_ElRetratoTieneLaCaraLaEmocionYLaBocaDeQuienHabla` (PlayMode) | El set, la emoción, que la boca cambia en 0,3 s, que una acotación oculta la tarjeta y que `Leave` la calla. `NarrativeScene_RF05_ElRetratoEsElDeQuienHablaYNoHayEnLasAcotaciones` se ajustó para aceptar `PortraitBase` o `Portrait`. |

**Ronda del Editor (sesión local de Santiago, 10/10/2026, `23d7e8c`).** Hecha. Los pasos y lo medido:

- [x] `pull`, `recompile` y copia de `BuildRigsFinal`. La recompilación dio 0 errores.
- [x] Modos `estado`, `sprites`, `retrato`, `clips` y `estado`. Los tres `char_algoritm_<forma>_cara.asset` se reapuntaron a la cara compartida y los seis PNG de la cara provisional de Algoritm se borraron con `AssetDatabase.DeleteAsset`, después de comprobar que ningún asset referenciaba sus GUID. Un `git lfs pull` posterior los devolvió al árbol de trabajo, así que se quitaron del repositorio en `8602c79`; siguen sin referencia. Tras `292b108` se forzó la reimportación de los 86 PNG de caras y retratos y se repitió `retrato → estado`. Hizo falta porque la primera importación de `char_nino_retrato_neutra.png` conservó la regla anterior (2048, comprimida): Unity la importó antes de recargar el nuevo `ArtImportRules`. Los cuatro prefabs de la familia reescribieron `PortraitFace`; los tres de Algoritm ya estaban al día.
- [x] Comparación de prefabs objeto por objeto contra `HEAD`. Papá, Mamá, la Niña y el Niño tienen 148 bloques cada uno y las tres formas de Algoritm, 69; no se añadió ni se perdió ninguno y `m_Script` y `m_Controller` no cambian. Solo cambian valores: los `RectTransform` de `Ojos`, `Boca` y `CaraBase` (de frente y de perfil), los sprites de `Ojos` y `Boca` de Algoritm, que ahora apuntan a la cara final compartida, y los campos nuevos `PortraitBase` y `PortraitFace`. Seis clips cambiaron solo en centésimas de grado: `char_nino_anim_idle`, `char_nino_anim_observar`, `char_papa_anim_observar`, `char_nina_anim_observar`, `char_mama_anim_idle` y `char_mama_anim_observar`. Los siete personajes tienen 10 de las 16 casillas de cara llenas (ojos neutros, de concentración, de preocupación y de sorpresa; ojos cerrados; boca cerrada `boca_0`; boca abierta `boca_a`; y las bocas de concentración, preocupación y sorpresa). Quedan vacías la alegría, el sueño, el cuadro de medio parpadeo y las bocas E y U. `PortraitBase` y `PortraitFace` coinciden con la tabla en los siete, y ningún PNG de cara o retrato pasa de 256 px.
- [x] Pruebas por grupos: `Game.Scaffolding.Tests` 387 de 387; `Game.Architecture.Tests` 25 de 25 tras la reimportación (24 de 25 antes, por el límite de 256 px de `char_nino_retrato_neutra`); PlayMode `NarrativeScene_` 108 de 108, `MainMenu` 19 de 19, `Credits` 6 de 6, `Fire` 76 de 76, `Forest` 40 de 40 y `RiverScene_` 23 de 23.
- [x] Suite completa con `suite2.ps1` (dos Editores, 25,9 min, 1167 pruebas listadas, cada una corrida una vez). EditMode: 748 de 749 pasan, 1 omitida (`ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`, la omisión de entorno de siempre) y 0 fallos. PlayMode: 417 de 418 pasan y falla `RiverLevel_RNF21_NingunaAnimacionDelNivel3TieneDestellos`, que midió un salto por cuadro de 0,056 frente al tope de 0,05 en el pulso «completado» de la balsa mientras los dos Editores se disputaban la CPU. Ni ese código ni ese asset cambiaron, y corrida sola la prueba pasa (1 de 1, 12,6 s). Se registra como una dependencia de la prueba del tiempo de cuadro y no como una regresión.
- [x] Capturas en `claudeDocs/tasks/Personajes/capturas/2026-10-10/`: 144 (139 líneas de narrativa y las 5 mecánicas), JPEG de calidad 90, 26,7 MB. Claude las revisó: Papá se arrodilla mirando la llama en N1 1.3 (L02 y L03) y Mamá en N2 2.5 (L02); las expresiones siguen el guion (en la apertura del N1, L01, la familia está preocupada y el Niño sorprendido); la tarjeta de la familia es un primer plano sin brazos y la de Algoritm conserva su encuadre abierto.
- [x] Build con la medida de RNF-06. `exec build` sobre `8602c79`, medido con `oe4.ps1 Tamano`: 426,6 MB (406,9 MiB; 426 633 082 bytes) sin `*_DoNotShip` ni `Datos`, 6,0 MB más que los 420,6 MB del 09/10/2026 y con 73,4 MB de margen frente al tope de 500 MB. No es un candidato del OE4 (sigue siéndolo rc2, de 479,0 MB), y RNF-04 y RNF-05 siguen sin medirse en un ejecutable.

**Abierto.**

- Del artista: la **alegría** y el **sueño** de los cinco personajes, y una **neutra nueva de la Niña y del Niño**.
- Si llegan `boca_e` y `boca_u`, el ciclo del habla vuelve a ser A, E, U y cerrada sin tocar código.
- La autorización de Santiago para editar los radicados que describen la cara de Algoritm y el retrato fijo (`claudeDocs/entregables/tools/pares/tg_expresiones_10oct.json`).
- La medida de RNF-04 y RNF-05 en un ejecutable (próximo candidato del OE4).
- `Level2_Maze.unity` apareció modificada en el árbol local (`RectTransform`, un redondeo de color y un espacio sobrante en un nombre) y quedó fuera de los commits a la espera de la respuesta de Santiago.
