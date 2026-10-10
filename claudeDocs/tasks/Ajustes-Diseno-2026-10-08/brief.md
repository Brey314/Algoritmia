# Ronda de ajustes de diseño del 08/10/2026 — brief de implementación

Documento de trabajo del orquestador. Lo escribió el planificador (solo lectura, 08/10/2026) y lo
fijan las decisiones de Santiago de abajo, que **mandan sobre el brief** cuando chocan.

## Decisiones de Santiago (08/10/2026)

| # | Tema | Decisión |
|---|---|---|
| D1 | Laberinto: zona donde se coloca la secuencia (`Ventana_Secuencia`) | Restaurar el beige oscuro de `fig-04b-4`: #E0D4C0 con `ui_boton` Sliced, como antes de `79e3ea7`. |
| D2 | Laberinto: fondo detrás de la tarjeta y detrás del entorno | **Un solo color, #E8A33D**, en `Canvas/Fondo_Escena` y en `MazeLayout.BackdropColor` (`N2_MazeLayout.asset`). Reforzar el aro de `Boton_Pista` para que no se pierda ámbar sobre ámbar. |
| D3 | Laberinto: `ivoryShadeColor` (lados y muescas de los bloques) | **Volver a #E0D4C0** (el árbol tenía #E8C27D sin commitear: se descarta). |
| D4 | Laberinto: contornos | Entorno con el mismo contorno que `Panel_Secuencia` (color #C4A882, `ui_boton` Sliced); los dos **8 px** y **sin cortes en las esquinas**. |
| D5 | Narrativa: texto del cuadro de diálogo | Nunito SemiBold 30 px, **color más oscuro #1F100C** (se corrige `Direccion_de_Arte.md` §10.3/§11.3); cuadro de 180 → ~216 px de alto, dentro del cuarto inferior. **Todos** los textos de las 18 narrativas deben caber (prueba nueva que las recorra todas). |
| D6 | Narrativa: nombre del hablante | Baloo2-Bold 30 px, mismo carbón oscuro #1F100C. |
| D7 | N3, cruce 3.3 | La cámara sigue a la balsa (`NarrativeProp.CameraFollows`); la parada de la línea 1 sube a ~0.52 para que no retroceda. |
| D8 | Fogatas | Halo naranja tenue, **pulso lento y suave**: #F0A84E al 20 %, escala 0,95–1,05, ciclo de 2 s (se corrige el 1,2 s de `Direccion_de_Arte.md`). En todas las fogatas encendidas, **incluida la llama cenital del N1**. |
| D9 | MainMenu: portada | Parte izquierda del bosque del N2 (`env_n2_bosque_claro`, foco 0.25) con los 5 personajes (Papá, Mamá, Niño, Niña, Algoritm **fuego**) en **Idle**, desfasados. |
| D10 | MainMenu: título | Fuera de la tarjeta, encima, blanco con contorno #3A1E18 de 4 px, 112 px. Tarjeta más corta solo con el lema «Piensa el orden, enciende el fuego». Bloque centrado en vertical (boceto `D:\User\Desktop\Captura de pantalla 2026-10-08 173959.png`). |
| D11 | LevelSelect | Icono de completado/bloqueado abajo a la izquierda de la imagen; su texto centrado entre la imagen y el botón; la imagen del N2 pasa a ser el bosque (foco 0.25). |
| D12 | Credits | Algoritm (fuego) con una **acción nueva `Wave`** (saludo, en bucle con pausa de reposo), del tamaño de la tarjeta que lo contenía; la tarjeta se elimina. |
| D13 | Algoritm por partes | Se recortan sus piezas del arte provisional (maqueta de `pose_preview.py`) y se encienden: **sus brazos se moverán en todas sus escenas**. Cuando llegue el arte final, se sustituyen los PNG con el mismo nombre y se corre el modo `sprites`. |
| D14 | Papá | Subir el húmero y bajar un poco el antebrazo para que no asome húmero por el codo (etapa 1). |
| D23 | Numeración INC | El carril de perfil reserva INC-134 (perfil al moverse), INC-135 (parpadeo de dos cuadros) e INC-136 (diseño nuevo de Algoritm adoptado el 09/10). Esta ronda usa **INC-137 en adelante**. El corte de Algoritm en 9 piezas es provisional: lo sustituye la ingesta de INC-136. |
| D22 | Entrega de arte de Algoritm del 09/10/2026 | La ingiere el **carril de perfil**, no esta ronda. Nuestro corte en 9 piezas sale del arte **provisional** (`_reposo`), no de esa entrega: decirlo a la nube con el hash del push. `Prompt-Arte-Final-Algoritm.md` le sirve a ese carril como guía. La sesión local algoritmia-40 sube los originales a `claudeDocs/tasks/Personajes/entregas/2026-10-09/Algoritm/` en el mismo árbol: solo esa ruta. |
| D21 | Integración con el carril de perfil (nube, `910c0f0`) | Commits locales por etapa → `git pull --no-rebase` (merge; nunca rebase ni force-push) conservando `CharacterRig.ShowView/HasProfile/View`, `Fit` que voltea solo en perfil y `rig.Mirrored = facesLeft ^ prop.Mirrored` en `PlaceActor`/`PlayActors`/`WalkAsync` → revisión con suite completa sobre lo integrado → docs → push. **Roja esperada, no tocar ni saltar:** `CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte` (4 casos) hasta la ronda del Editor del carril de perfil. Cualquier otro fallo INC134/INC135 se le manda a la nube con el texto exacto. Delegación de Santiago (08/10/2026): las decisiones de desarrollo las toma el orquestador; commit y push a nombre de Santiago sin `Co-Authored-By`. |
| D20 | Algoritm por partes: defectos menores | **Pendiente para el arte final de Algoritm** (no se corrige ahora): (1) en los fundidos (`Appear`, `Vanish`, `Hidden`) las rótulas se ven más oscuras ~0,3 s porque el alfa del `CanvasGroup` multiplica cada `Image` por separado; (2) talón claro de ~12 px en el hombro girado. El agente de documentos los registra como pendientes en `Personajes-Resultados.md` y `Plan-Personajes-Finales.md`. |
| D18 | Halo a plena luz | Se deja como está (casi no se ve de día; coherente). |
| D19 | Cruce 3.3: apertura | `CameraStart` y `CameraEnd` de `N3_Escena33_Cruce.asset`: foco x 0.38 → **0.50** (y 0.37, zoom 1.6), para que la balsa se deslice menos hacia la izquierda (~580 → ~230 px según el modelo de la etapa 3). Editado a mano por el orquestador; **el revisor debe verificarlo**: `NarrativeSequence_RF05_NingunEncuadreDelNivel3SeRecorta`, `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`, `NarrativeSequence_RF44_…`, `NarrativeScene_RF44_…` y una captura de la apertura y del cruce. |
| D17 | Narrativa: cuadro de diálogo | Más espacio entre el nombre del hablante y el texto, y menos interlineado en el texto (por debajo del 1.25 propuesto). |
| D16 | Papá, tras la etapa 1 | Aprobados: brazos ~37 px más largos (4 % de su alto, efecto de bajar el antebrazo con `codo.py`) y el choque de `Strike` más bajo (y = 549 → 595, tope 616) con los codos más abiertos. |
| D15 | Laberinto, tras la etapa 2 | Aprobados: margen de 40 px alrededor del entorno (tablero −7,4 %), aro de `Boton_Pista` en #A0330D (3,27:1 sobre el ámbar; color fuera de la paleta, se añade a la dirección de arte) y esquinas exteriores de ~7 px en el marco del entorno frente a 24 px en la tarjeta. |

## Brief del planificador

Ver la respuesta completa del planificador, resumida aquí por etapa. Rutas y líneas son de
`HEAD` del 08/10/2026; verificarlas antes de editar.

### Etapa 2 — Laberinto (`Level2_Maze`)
- `Ventana_Secuencia` (`Canvas/Panel_Secuencia/Fondo/Ventana_Secuencia`, Image `&341287497`): volver a
  #E0D4C0 con `ui_boton` Sliced (guid `5ec80065…`, `m_Type: 1`), como antes de `79e3ea7`.
- Fondo único: `Canvas/Fondo_Escena` (`&1900669105`) y `MazeLayout.BackdropColor` (`MazeLayout.cs:105-107`,
  valor en `N2_MazeLayout.asset:35`), que `FitEnvironment` pinta en `Panel_World`
  (`MazeSceneController.cs:427-431`). Tooltip de `MazeLayout.cs:106` a corregir.
- Esquinas: `Panel_Secuencia` es Image #C4A882 con `ui_boton` Sliced; su hijo `Fondo` (inset 5 px) perdió
  el sprite en `79e3ea7` y sus esquinas cuadradas asoman sobre el arco → devolverle `ui_boton` Sliced y
  subir el inset a 8 px (`sizeDelta -16`). El entorno usa un `Outline` (`MazeSceneController.cs:433-442`)
  sobre una Image escalada (grosor 4×escala) → sustituirlo por un hermano `Marco_Entorno` detrás, Image
  `ui_boton` Sliced, `fillCenter = false`, color `environmentBorderColor` (#C4A882), tamaño visible + 2×8 px,
  `pixelsPerUnitMultiplier` ≈ 26/8.
- Pruebas: `MazeSceneTests.MazeScene_RF30_LaSalidaSeLeeEnElEntornoYElPanelEsMarfil` (l.342) y
  `MazeScene_RF30_ElEntornoQuedaSinTinteYConSuContorno` (l.280) se ajustan/renombran
  (`…ElFondoEsUnoSolo`, `…ElEntornoYLaTarjetaLlevanElMismoMarcoRedondeado`); nueva
  `MazeScene_RF31_LaZonaDeSoltarEsMarfilSombraSobreLaTarjeta`.

### Etapa 3 — Narrativa (`Narrative.unity`, `NarrativeSceneController`)
- Texto: `Canvas/CuadroDialogo/Fondo/Cuerpo` (`UnityEngine.UI.Text` legado, no TMP; `&1399545944`, l.1400-1427;
  hoy Nunito-Regular 26, #3A1E18, interlineado 1.35, sin BestFit, 680×134 en (184,-38)). `CuadroDialogo`
  1400×180, `Fondo` 1388×168. Ancho máximo 680 (antes de «Omitir», x = 880).
- Hablante: `Hablante` (`&1555468459`, l.1562-1588; hoy Baloo2-Bold 22, #6B5248, 680×26 en (184,-12)).
- Pruebas: sustituir `NarrativeSceneTests.NarrativeScene_RNF01_LaLineaMasLargaCabeEnSuCuadroDeDialogo` (l.255)
  por `NarrativeScene_RNF01_TodasLasLineasDeLasDieciochoNarrativasCabenEnSuCuadro` (recorre
  `controller.Sequences`, 18 secuencias, 139 líneas, `GetPreferredHeight` contra la caja); añadir
  `NarrativeScene_RF05_ElNombreDelHablanteCabeEnUnaLinea`. Vigila el alto
  `NarrativeScene_RNF03_ElCuadroDeDialogoNoSuperaElCuartoDeLaPantalla` (l.478).
- Cámara: solo `N3_Escena33_Cruce.asset` mueve un prop (balsa, `Motion: 4 = Drift`, l.82-94). `MoveAsync`
  (`NarrativeSceneController.cs:686-770`) y `TargetFraming` (l.786-804). Campo nuevo
  `NarrativeProp.CameraFollows`; en `TargetFraming`, si sigue, foco = parada + `anchoredPosition / sprite.rect.size`
  (el clamp lo hace `IllustrationFraming.Offset`, l.105-112). Pruebas nuevas
  `NarrativeScene_RF44_LaCamaraSigueALaBalsaMientrasCruza` (PlayMode) y
  `NarrativeSequence_RF44_LaCamaraDelCruceAcompanaALaBalsa` (EditMode); cuidar
  `NarrativeSequence_RF05_NingunEncuadreDelNivel3SeRecorta` ([0.316, 0.684]) y
  `NarrativeScene_RNF21_LaBalsaCruzaSinSaltosNiParpadeos` (l.749).
- Fogatas (props con `prop_n1_fuego_normal`): `N1_NacimientoDelFuego` (1), `N2_PuenteI` (1),
  `N2_Escena25_Cierre` (1), `N3_PuenteII` (1), `N3_PuenteII_Horizonte` (2), `N3_EscenaFinal` (3); más la llama
  cenital de `Level1_Cave` (`FirePanelController.fireFlame`, l.860-861; ya la ilumina `CaveLightingController`,
  l.29-32). `Direccion_de_Arte.md` l.547-559 ya especifica el halo (sin implementar). Componente nuevo
  `FireGlow` (`Game.Scaffolding`, uGUI): hermano `Halo_<nombre>` justo antes del fuego, sprite
  `PropShadow.GetOrCreateCircleSprite()` (`PropShadow.cs:468`), copia ancla/pos/escala ×`spread` (~1.8),
  escala `1 + 0.05·sin(2π t/2)` con tiempo escalado, alfa fijo, `raycastTarget = false`. Enganche por dato:
  `NarrativeProp.Glows` y una línea en `PlaceProps` junto a `BurnReveal` (l.454); en N1 el componente en
  `fireFlame` de la escena. Pruebas: `NarrativeSequence_RF05_CadaLlamaEmiteSuHalo` (EditMode, patrón de
  `CadaLlamaEchaHumoPorDetrasYPorEncima`, l.348) y `NarrativeScene_RNF21_ElHaloDeLaFogataPulsaLentoYSinDestellos`
  (PlayMode). Riesgos: `NarrativeScene_RF05_LaEscena21MuestraLosObjetosRepartidosPorElSuelo` (l.532, filtrar
  `Halo_*`) y `FireLevel_RF20_AlSoplarPrendeLaLlamaCenitalSobreElMonton` (l.1029, la llama debe seguir siendo el
  último hermano).

### Etapa 4 — Menús (`Game.UI`)
- MainMenu: `MainPanel/Silueta/IlustracionInicio` (Image #F7EFE2 sin sprite, 952×952) con la instancia
  `Algoritm_Fuego(MenuInicio)`. `RectMask2D` + hijo `Bosque` con `env_n2_bosque_claro` y componente nuevo
  `FramedIllustration` (`Game.UI`, aplica `IllustrationFraming` en `Start`; se reutiliza en LevelSelect), foco
  (0.25, 0.5), zoom 1 (x ∈ [0.11, 0.39], sin cruzar la costura). Los 5 prefabs hijos de `Bosque` como
  `PlaceActor`. Desfase: campo `CharacterRig.idlePhase` (`[Range(0,1)]`, 0 por defecto) usado en `Apply`
  (`CharacterRig.cs:252-256`). Título: `TitleLabel` fuera de `TitlePanel` (conserva la referencia de
  `MainMenuController.titleLabel`, l.17), blanco 112 px, `UnityEngine.UI.Outline` #3A1E18 (4,-4); `TitlePanel`
  ~120 de alto con el lema; `Bloque` anclado (0, 0.5). Pruebas nuevas:
  `MainMenu_RF01_LosCincoPersonajesEsperanEnReposoSinRespirarAlUnisono`,
  `MainMenu_RF01_ElBosqueNoCruzaLaCosturaDelLienzo`,
  `MainMenu_RF01_TituloTarjetaYBotonesQuedanCentradosEnVertical`.
- LevelSelect: hoy el N2 usa `env_n2_laberinto.png`; `Arte` 444×300 `preserveAspect` con tinte #E0D4C0;
  insignias hijas de `Arte` arriba a la derecha. Nuevo `Fondo/Ventana` (444×250, `RectMask2D`) con `Arte`
  + `FramedIllustration`; insignias como hijas de `Fondo`: icono abajo a la izquierda de la ventana (+16, +16),
  texto centrado en la franja entre imagen y botón. Se rompe
  `LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura` (l.69; helper `ArtFor`, l.262) → ajustar.
  Nueva `LevelSelect_RNF19_ElIconoDeEstadoVaAbajoALaIzquierdaDeLaImagenYSuTextoEntreImagenYBoton`.
- Credits: `AlgoritmPanel` (520×952, `Fondo`/`Marco`/`Sombra`) contiene `AlgoritmSaluda` (Image vacía) y la
  instancia de rig `Algoritm_Fuego(Creditos)` (300×300, Idle). Borrar `Fondo`/`Marco`/`Sombra`, rig directo
  bajo `AlgoritmPanel` a pantalla del panel; `CreditsController`: `[SerializeField] CharacterRig guide;` y
  `guide?.Play(ActorAction.Wave)` en `Start`. Prueba nueva
  `Credits_RF08_AlgoritmSaludaOcupandoElLugarDeLaTarjetaSinElla`.
- Algoritm por partes: el prefab ya tiene `Tronco/BrazoIzq|Der/CodoIzq|Der/AntebrazoIzq|Der`, piernas, `Ojos`,
  `Boca`, **todas apagadas sin sprite**; el arte entero va en `Cuerpo` (`char_algoritm_n1_fuego_reposo`).
  Generar `char_algoritm_fuego_parte_*.png` con los nombres de `rig_articulaciones.json` (l.482-600): ninguna
  herramienta lo hace hoy; `pose_preview.py` ya construye esa maqueta en memoria (l.38-42, 351-383) → añadirle
  `--exporta-maqueta`. Luego `BuildRigsFinal` modos `sprites` → `orden` → `clips`.
- `Wave`: `ActorAction.Wave = 22` (al final), `ActionEmotion.For` → Happy, `coreografia.py` `clips_guia`
  (l.2268) `guia_spec("Wave", "saludar", …)` (brazo derecho ~110°, codo ±25°, bucle con pausa). El modo
  `clips` no crea clips ni estados (`BuildRigsFinal.cs.txt:51-53`): script de Editor efímero que cree
  `char_algoritm_anim_saludar.anim` y el estado `Wave` en `char_algoritm.controller` (compartido por las tres
  formas). `CharacterRigTests.CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion` (l.89): excluir `Wave` de
  `AccionesDeLaFamilia` como `Spin` y añadirlo a `AccionesDelGuia` (l.26-30); `FacialEmotionTests` necesita
  `[TestCase(ActorAction.Wave, FacialEmotion.Happy)]`.
- Entorno: el Python del equipo **no tiene PIL**, que las herramientas de personajes necesitan.
