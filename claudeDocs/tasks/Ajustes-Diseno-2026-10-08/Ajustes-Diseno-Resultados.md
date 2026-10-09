# Ronda de ajustes de diseño del 08/10/2026 — resultados

Registro histórico de la ronda. Rama `feat/personajes-animados`. Las decisiones son de Santiago
Benavides Rey (D1 a D22, `brief.md`); cada etapa tuvo su parte crudo (`notas-01-papa.md`,
`notas-02-laberinto.md`, `notas-03-narrativa.md`, `notas-04a-algoritm-creditos.md`,
`notas-04b-menus.md`) y de ellos y de `git show` salen las cifras de abajo. Las etapas no corrieron
PlayMode con el runner: lo hizo la revisión (apartado «Revisión»). Los hallazgos que la ronda abrió
están en `INCONSISTENCIAS.md`, de **INC-137** a **INC-145** (rev. 20); INC-134 a INC-136 son del carril de
perfil y los escribe ese carril.

## 1. Decisiones

| # | Tema | Decisión |
|---|---|---|
| D1 | Laberinto, `Ventana_Secuencia` | `#E0D4C0` con `ui_boton` Sliced, como antes de `79e3ea7` |
| D2 | Laberinto, fondo | Un solo color, `#E8A33D`, en `Canvas/Fondo_Escena` y en `MazeLayout.BackdropColor`; aro de `Boton_Pista` reforzado |
| D3 | Laberinto, `ivoryShadeColor` | Vuelve a `#E0D4C0` (se descarta el `#E8C27D` sin commitear) |
| D4 | Laberinto, contornos | Entorno y tarjeta con el mismo marco (`#C4A882`, `ui_boton` Sliced), 8 px y sin cortes en las esquinas |
| D5, D6 | Diálogo | Texto Nunito SemiBold 30 px y nombre Baloo 2 Bold 30 px, ambos `#1F100C`; cuadro de 180 a 216 px; la prueba recorre las 18 narrativas |
| D7 | N3, cruce 3.3 | La cámara sigue a la balsa (`NarrativeProp.CameraFollows`); parada de la línea 1 a 0,52 |
| D8 | Fogatas | Halo `#F0A84E` al 20 %, pulso de 2 s entre 0,95 y 1,05, en todas, incluida la llama cenital del N1 |
| D9 | Inicio, portada | Mitad izquierda del bosque del N2 (foco 0,25) con los cinco personajes en `Idle` desfasados |
| D10 | Inicio, título | Fuera de la tarjeta, blanco con contorno `#3A1E18` de 4 px, 112 px; tarjeta corta con el lema; bloque centrado en vertical |
| D11 | Menú de niveles | Icono de estado abajo a la izquierda de la imagen, texto entre imagen y botón; la imagen del N2 es el bosque |
| D12 | Créditos | Algoritm con la acción nueva `Wave`, sin tarjeta |
| D13 | Algoritm | Por partes, desde su arte provisional, en todas sus escenas |
| D14 | Papá | Subir el húmero y bajar un poco el antebrazo |
| D15 | Laberinto, tras la etapa 2 | Margen de 40 px alrededor del entorno, aro de la pista en `#A0330D` (3,27:1) y esquinas exteriores de unos 7 px en el marco del entorno |
| D16 | Papá, tras la etapa 1 | Aprobados los brazos unos 37 px más largos y el choque de `Strike` más bajo |
| D17 | Diálogo | Más aire entre el nombre y el texto, y interlineado de 1,1 |
| D18 | Halo a plena luz | Se deja como está (casi no se ve de día) |
| D19 | Cruce 3.3, apertura | `CameraStart` y `CameraEnd` del foco x de 0,38 a 0,50 (editado a mano por el orquestador) |
| D20 | Algoritm por partes, defectos menores | Pendientes para el arte final (apartado 4) |
| D21 | Integración con el carril de perfil | Commits por etapa, `git pull --no-rebase`, revisión con suite completa, documentos, push |
| D22 | Entrega de arte de Algoritm del 09/10 | La ingiere el carril de perfil; el corte en nueve piezas sale del arte provisional |

## 2. Etapas

### Etapa 1 — Papá: el húmero ya no asoma por el codo (`a938f9a`, D14, D16)

**Antes.** El arte final de Papá trae el húmero con una punta clara y sin contorno que se pasa de su
extremo redondo, con el antebrazo solapado encima: entre el extremo del húmero y el casquete del
antebrazo había 42 px a la izquierda y 21 px a la derecha (en Mamá, Niña y Niño, de 1 a 8 px). Al
doblar el codo la punta asomaba hasta 7 px, como un bulto sin contorno.

**Después.** `codo.py` sube el húmero y baja el antebrazo a lo largo de su eje hasta dejar el punto más
lejano 4 px dentro del casquete, sin mover el hombro ni tocar ningún PNG. El húmero izquierdo sube 19,2
px, los dos antebrazos bajan 37,4 px y lo que asoma pasa de 52,6 y 33,4 px a −3,2 y 1,0. Los brazos miden
unos 37 px más y `Strike` baja de y = 549 a y = 595 (tope 616) con los codos más abiertos.

**Archivos.** `Papa.prefab` (15 líneas: anclas y pivotes de siete `RectTransform`), los 21
`char_papa_anim_*.anim`, y en `claudeDocs/tasks/Personajes/herramientas/`: `codo.py` (nuevo),
`arte_final.json`, `rig_articulaciones.json`, `preparar_arte_final.py`, `pose_preview.py` (comprobación
(j) del codo) y `clips_personajes.json`. Los clips de los otros cuatro personajes no cambian.

**Pruebas.** Herramientas: `pose_preview.py` pasa en 119 filas (111 clips y 8 codos); autopruebas de
`codo.py` y de las demás. EditMode: `CharacterRig_` 142/142 y `Game.Scaffolding.Tests` 278/278.

### Etapa 2 — Laberinto (`108f95e`, D1 a D4, D15)

**Antes.** `Fondo_Escena` y `BackdropColor` en marfil `#F7EFE2`, `Ventana_Secuencia` sin sprite, el
`Fondo` de la tarjeta cuadrado con 5 px de inset (el contorno medía 0 px en la diagonal) y el entorno con
un `Outline` que escalaba con la ilustración.

**Después.** Un solo ámbar `#E8A33D` detrás de todo; `Ventana_Secuencia` y las muescas en `#E0D4C0`; el
`Fondo` de la tarjeta redondeado, con 8 px de inset y `pixelsPerUnitMultiplier` 1,5 para que el aro sea
uniforme en la curva; el entorno con `Marco_Entorno`, un anillo `ui_boton` sin centro de 8 px, y 40 px de
margen en `Panel_World` (el tablero baja de escala 0,675 a 0,625, es decir, 7,4 %); aro de `Boton_Pista` de
`#E2571F` (1,73:1 sobre el ámbar) a `#A0330D` (3,27:1). Contorno medido en la diagonal de las esquinas: 0 px
antes, 7,1 px en la tarjeta y 8,5 px en el entorno después.

**Archivos.** `Level2_Maze.unity`, `N2_MazeLayout.asset`, `MazeLayout.cs`, `MazeSceneController.cs`
(`environmentFrame`, `environmentBorderWidth`), `Game.Levels.Wheel.Tests.asmdef` y las pruebas.

**Pruebas nuevas o cambiadas.** EditMode, `MazeSceneDataTests` (nuevo): `MazeScene_RF30_ElFondoDeLaEscenaYElDelAssetSonElMismoAmbar`
y `MazeScene_RNF20_ElAroDelBotonDePistaSeDistingueDelFondoAmbar`. PlayMode: nuevas
`MazeScene_RF30_ElEntornoYLaTarjetaLlevanElMismoMarcoRedondeado` y
`MazeScene_RF31_LaZonaDeSoltarEsMarfilSombraSobreLaTarjeta`; renombradas
`MazeScene_RF30_ElEntornoQuedaSinTinte` y `MazeScene_RF30_LaSalidaSeLeeEnElEntornoYElFondoEsUnoSolo`;
ajustada `MazeScene_RNF23_ElEntornoCabeEnteroEnSuPanelSinDeformarseYLaMatrizCubreElSeto`. EditMode:
`Game.Levels.Wheel.Tests` 83/83, `Game.Architecture.Tests` 23/23, `Game.Content.Tests` 3/3; con una mutación
de `BackdropColor` las dos pruebas nuevas fallan y restauradas pasan.

### Etapa 3 — Narrativa (`08b6c63`, D5 a D8, D17)

**Cuadro de diálogo.** De 1400 × 180 a 1400 × 216 px (cima a 232 px de 1080; el cuarto acaba en 270); texto de
Nunito Regular 26 `#3A1E18` interlineado 1,35 a Nunito SemiBold 30 `#1F100C` interlineado 1,1; nombre de Baloo 2
Bold 22 `#6B5248` a Baloo 2 Bold 30 `#1F100C`; retrato centrado en vertical; el texto baja de −46 a −64 px, y
entre la tinta del nombre y la del primer renglón se pasa de 10 a 32 px. La línea más alta de las 139 de las 18
narrativas mide 131 px en una caja de 137 (antes 129 en una de 134); ninguna necesita cuatro renglones, 17
usan tres.

**Cámara del cruce.** `NarrativeProp.CameraFollows` y `Followed(StopFraming())` en `NarrativeSceneController`: el
foco de cada parada se corre lo que se ha movido la balsa. Sin avanzar el texto, el margen mínimo a la
derecha pasa de −204 px (la balsa acababa medio fuera) a +77 px. `N3_Escena33_Cruce` marca la balsa y sube la
parada de la línea 1 de 0,47 a 0,52; D19 baja después la apertura a 0,50 (la revisión midió que el
deslizamiento de la balsa en pantalla bajó de 581 a 228 px; el modelo de la etapa 3 daba unos 230).

**Halo.** `FireGlow` (`Game.Scaffolding`, 230 líneas): hermano de la llama, escala 0,95 a 1,05 en 2 s sobre
tiempo escalado, opacidad fija, color `#F0A84E` al 20 %. El círculo plano que pedía el documento se probó
primero y se descartó (una mancha naranja de unos 625 px de noche; aros fantasma a plena luz), así que el
sprite es un disco con degradado radial generado por código (opacidad plena hasta 0,3 del radio). Las nueve
llamas de las narrativas lo llevan por `NarrativeProp.Glows`; la cenital de `Level1_Cave`, por el componente en la
escena con `BehindSiblings`, para no lavar el montón y las piedras.

**Archivos.** Siete `N*_*.asset` (`Glows`; `CameraFollows` y la parada en `N3_Escena33_Cruce`), `Narrative.unity`,
`Level1_Cave.unity`, `FireGlow.cs`, `NarrativeProp.cs`, `NarrativeSceneController.cs` y las pruebas.

**Pruebas nuevas.** EditMode: `FireGlowTests` (12), `NarrativeSequence_RF05_CadaLlamaEmiteSuHalo`,
`NarrativeSequence_RF44_LaCamaraDelCruceAcompanaALaBalsa`, `CaveSceneDataTests` (1). PlayMode:
`NarrativeScene_RNF01_TodasLasLineasDeLasDieciochoNarrativasCabenEnSuCuadro` (sustituye a la de la línea más
larga), `NarrativeScene_RF05_ElNombreDelHablanteCabeEnUnaLinea`,
`NarrativeScene_RNF03_ElNombreElTextoYLosBotonesNoSePisanDentroDelCuadro`, `NarrativeScene_RF44_LaCamaraSigueALaBalsaMientrasCruza`,
`NarrativeScene_RF44_LaBalsaNoSeSaleDelCuadroNiLaCamaraRetrocedeAlLeerElTextoMientrasCruza`,
`NarrativeScene_RF05_CadaFogataTieneSuHaloJustoAntesDeLaLlama`, `NarrativeScene_RNF21_ElHaloDeLaFogataPulsaLentoYSinDestellos`,
`NarrativeScene_RNF21_ElHaloDeLaFogataSeQuedaQuietoConLaPausa` y `FireLevel_RF20_LaLlamaCenitalEmiteSuHaloSinVelarElMonton`.
EditMode: `Game.Scaffolding.Tests` 292/292, `Game.Levels.Fire.Tests` 50/50. Mutaciones (`CameraFollows`, la parada de
la línea 1 y un `Glows`) hacen fallar su prueba y restauradas pasan.

### Etapa 4a — Algoritm por partes, `Wave` y créditos (`bd8b802`, D12, D13)

**Antes.** Algoritm era una sola `Image` (`Cuerpo`) con nueve piezas apagadas y sin sprite; los créditos lo
mostraban en una tarjeta de 520 × 952 px en `Idle`.

**Después.** 27 PNG (nueve por forma) en `Assets/Game/Art/Characters/Algoritm/Frontal/`, cortados del `_reposo` con
`pose_preview.py --exporta-maqueta` por la forma del dibujo y con una rótula en cada articulación (las piezas
suman el sprite con 0,002 % de píxeles distintos; 3,63 MiB de textura sin comprimir). Los puntos de articulación
se midieron sobre el alfa (el hombro estaba 17 px por encima del palo). `Cuerpo`, `Ojos` y `Boca` quedan
apagados. `ActorAction.Wave = 22` con emoción `Happy`; el clip `char_algoritm_anim_saludar.anim` (4,8 s, bucle,
con un reposo de unos 0,9 s) y su estado en `char_algoritm.controller`; Algoritm pasa a 10 clips. Los créditos
pierden `Fondo`, `Marco`, `Sombra` y `AlgoritmSaluda`; el rig cuelga directo de `AlgoritmPanel` a 520 px de ancho y
`CreditsController.guide` lo anima con `Wave` en `Start`.

**Archivos.** Los tres prefabs de Algoritm (102 líneas cada uno, 69 objetos, ningún fileID), `Credits.unity`,
`ActorAction.cs`, `ActionEmotion.cs`, `CreditsController.cs`, el controlador y el clip, y en `herramientas/`:
`maqueta.py` (`piezas_guia`), `pose_preview.py`, `articulaciones.py`, `coreografia.py`, `rig_articulaciones.json`,
`clips_personajes.json` y `BuildRigsFinal.cs.txt` (un comentario).

**Pruebas.** Nuevas: `CharacterRig_DA131_AlgoritmSeDibujaPorPartesYSuSpriteEnteroSeApaga` (3 formas) y
`Credits_RF08_AlgoritmSaludaOcupandoElLugarDeLaTarjetaSinElla` (PlayMode); ajustadas
`CharacterRig_DA133_CadaPersonajeTieneUnEstadoPorAccion` y `FacialEmotion` (`Wave` a `Happy`). EditMode:
`CharacterRig` 145/145, `FacialEmotion` 30/30, `Game.Scaffolding.Tests` 296/296, `Game.UI.Tests` 29/29,
`ArtImport` 3/3; `pose_preview.py` 10/10 por forma de Algoritm y 21/21 por miembro de la familia.

### Etapa 4b — Menús (`c96846f`, D9 a D11)

**Antes.** Inicio con el título dentro de una tarjeta de 280 px y Algoritm de 300 px en un cuadro marfil vacío;
bloque con 272 px de margen arriba y 160 abajo. Menú de niveles con la imagen de 444 × 300 y las insignias
arriba a la derecha de ella; el Nivel 2 mostraba el laberinto.

**Después.** La portada es la mitad izquierda de `env_n2_bosque_claro` (foco 0,25) en una ventana siempre cuadrada
(`AspectRatioFitter`), con Mamá, Papá, la Niña, el Niño y Algoritm en fuego en `Idle` con fases 0,35, 0, 0,70, 0,15
y 0,50 (`CharacterRig.idlePhase`). El título (blanco, 112 px, `Outline` `#3A1E18` de 4 px) sale de la tarjeta, que
baja a 120 px y solo lleva el lema; título, tarjeta y botones cuelgan de un `Bloque` anclado en (0; 0,5), con 222 px
de margen arriba y abajo (los botones suben 62 px). En el menú de niveles, cada tarjeta lleva una `Ventana` de
444 × 250 px con la imagen; el icono va abajo a la izquierda, a 16 px de los bordes; el texto, centrado entre la
imagen y el botón, sin la pastilla marfil; la imagen del Nivel 2 es el bosque con foco 0,25.
`FramedIllustration` (`Game.UI`) va en la ventana y no en la imagen: Unity no avisa al hijo de que su padre
cambió de tamaño, y en el primer cuadro el `Canvas` mide 0.

**Archivos.** `MainMenu.unity`, `LevelSelect.unity`, `CharacterRig.cs`, `LevelSelectController.cs` (dos
accesores de prueba), `FramedIllustration.cs` (nuevo, 87 líneas), `IllustrationProbe.cs` y las pruebas.

**Pruebas nuevas.** PlayMode: `MainMenu_RF01_LosCincoPersonajesEsperanEnReposoSinRespirarAlUnisono`,
`MainMenu_RF01_ElBosqueNoCruzaLaCosturaDelLienzo`, `MainMenu_RF01_TituloTarjetaYBotonesQuedanCentradosEnVertical`,
`MainMenu_RF01_LosCincoPersonajesQuedanDentroDeLaPortada` (extra),
`LevelSelect_RNF19_ElIconoDeEstadoVaAbajoALaIzquierdaDeLaImagenYSuTextoEntreImagenYBoton` y la ajustada
`LevelSelect_RF03_CadaTarjetaMuestraUnaIlustracionSinCostura`. EditMode: `FramedIllustrationTests` (7) y
`CharacterRigIdlePhaseTests` (25); `Game.UI.Tests` 36/36, `Game.Scaffolding.Tests` 321/321. Dos mutaciones
(`idlePhase` ignorado, sin la guarda de ventana vacía) hacen fallar su prueba.

## 3. Capturas

En `claudeDocs/tasks/Ajustes-Diseno-2026-10-08/capturas/` (LFS), 1920 × 1080 salvo recortes y hojas:

| Carpeta | Contenido |
|---|---|
| `01-papa/` (10) | Codos de Papá antes y después, con el húmero teñido de magenta, con los colores reales y renderizados por el Editor |
| `02-laberinto/` (17) | Antes y después de la pantalla, las esquinas de la tarjeta y del entorno, el botón de pista, los bloques; decisiones (aro, esquina, margen) y robustez a 16:10, 4:3 y 21:9 |
| `03-narrativa/` (30) | El cuadro con la línea más alta, el cruce de la balsa cada segundo, el halo con oscuridad y a plena luz, la llama del N1; decisiones (interlineado, perfiles del degradado, halo en la cueva) |
| `04a-algoritm-creditos/` (13) | Hoja de 12 fotogramas del saludo, créditos antes y después, Algoritm en tres gestos de la narrativa del N1, el límite del fundido |
| `04b-menus/` (7) | Inicio y menú de niveles antes y después, el inicio a 16:10 y 4:3, el reborde del título ×3 |
| `05-revision/` | Las de la revisión (fase B) |

Las del entregable del OE3 (Figura 4.7 del laberinto y Figuras 7.1 y 7.2) salen de esta carpeta; ver el apartado 5.

## 4. Pendientes

- **Arte final de Algoritm (D20), dos defectos que se corrigen cuando llegue.** (1) En `Appear`, `Vanish` y
  `Hidden` las rótulas se ven más oscuras unos 0,3 s, porque el alfa del `CanvasGroup` multiplica cada `Image`
  por separado donde se solapan. (2) El hombro girado deja ver un talón claro de unos 12 px. Además `Ojos` y
  `Boca` siguen apagados, y `pose_preview.py` dibuja a Algoritm con la maqueta de su sprite entero y no con los
  PNG de `Frontal/`. La guía para meter ese arte es `claudeDocs/tasks/Personajes/Prompt-Arte-Final-Algoritm.md`; la
  entrega del 09/10/2026 la ingiere el carril de perfil (D22). Ese carril adoptó además un diseño nuevo de
  Algoritm (INC-136: llama, disco de madera y gota con pantaloneta, siete piezas sin rodilla, cara
  provisional) que sustituirá el corte en nueve piezas de esta ronda; los dos defectos se revisan con ese
  arte.
- **Peso del arte nuevo de Algoritm**: 3,63 MiB de textura sin comprimir, sin medir en un ejecutable; RNF-06 tenía
  21 MB de margen (rc2, 479,0 MB). El ejecutable no se recompiló en la ronda, y el trabajo de grado y el entregable
  siguen citando rc2.
- **Strike de Papá**: aprobado en D16; el parte propone, si se prefiere más alto, ajustar `yy` de `strike()` solo
  para Papá.
- **Tablero del laberinto 7,4 % más pequeño** por el margen de 40 px (D15, aprobado). El `Panel_World` conserva su color
  guardado `#6E698C` (el código lo pisa al arrancar).
- **Títulos y contraste.** El blanco del título sobre el ocre da 1,69:1 por sí solo; lo hace legible el contorno
  (`#3A1E18` sobre el ocre, 9,03:1). El caso `PF-RNF20-01` del OE4 muestrea el color del texto y el del fondo: si
  ignora el contorno, el título saldrá bajo 4,5:1.
- **OE4.** Las figuras `fig-07-1-pantalla-de-inicio.png` y `fig-07-2-menu-de-niveles.png` de `claudeDocs/tasks/OE4/evidencias/figuras/`
  son de rc2 y muestran el diseño anterior; los casos `PF-*` que miren la posición de los botones del inicio
  (subieron 62 px) o de las insignias del menú de niveles deben releerse.
- **Portada a 4:3** encoge a 668 px (frente a 952) para conservar la composición; Algoritm no tiene sombra en el
  inicio; el borde derecho de la ventana de la tarjeta del Nivel 2 toca el eje del espejo (x = 0,4998), vigilado por
  `LevelSelect_RF03_…`.
- **Halo.** Respira todo a la vez (misma onda, sin desfase), también las tres fogatas de la escena final; el
  degradado y su `CoreRadius` son constantes de `FireGlow` si Santiago quiere más contraste.
- **Acotaciones.** El texto queda 18 px más abajo que antes (el hueco del nombre sigue reservado) y el cuadro tapa
  36 px más de pantalla.
- **`Level1_Cave.unity`.** Cada guardado de Unity vuelve a escribir `restingSliderAlpha: 0.5` en
  `FirePanelController`: inofensivo.
- **Excepción previa, no de la ronda.** `NullReferenceException` en `GuideTrail.Step` al cargar `Level1_Cave` justo
  después de una narrativa dentro de la misma sesión de Play del arnés; desde una sesión limpia no aparece.
- **Trabajo de grado.** El apartado de Agradecimientos sigue siendo la plantilla vacía y debería nombrar a la
  colaboradora; el Anexo G con la autorización de la Familia Anonaky sigue a la espera del escaneo. El
  capítulo 8 sigue contando el corte del 1 de octubre y las Tablas 9 a 11 (rc2); si se compila un candidato nuevo
  se reescriben juntos.
- **Kanban.** La última acta es la D10 (30/09/2026) y su §6 no trae tarjeta que cubra esta ronda; los commits no citan tarjeta.

## 5. Documentos que cambiaron

- **Rectores.** `CLAUDE.md`, `claudeDocs/SPEC.md`, `claudeDocs/Direccion_de_Arte.md` (paleta con `#1F100C` y
  `#A0330D`; §7.3, §7.6, §8.1, §8.2, §10.2 a §10.4, §11.3, §11.4, §12.2, §13.1, §13.3, §14.3, §18),
  `claudeDocs/Interfaces.md`, `Assets/Game/Art/Inventario.md`, `claudeDocs/INCONSISTENCIAS.md` (rev. 20, INC-137 a
  INC-145; se cierran los pendientes de herramientas de ilustración y de colaboración en el arte),
  `claudeDocs/tasks/Personajes/Personajes-Resultados.md` (C.12) y `Plan-Personajes-Finales.md` (nota del 08/10).
- **Colaboración en el arte.** Sofía Valentina Giraldo Segovia, estudiante de animación y diseño, vinculada en el
  acta D01 (02/09/2026) y contratada para el diseño y la asesoría de los sprites; su participación es de diseño y
  asesoría gráfica, y la autoría y las decisiones pedagógicas y de mecánica son del equipo. Herramienta: CLIP
  Studio. Cada sprite pasa por cinco pasos: inspiración en equipo (lluvia de ideas), boceto, limpieza de la línea,
  color y sombras, entrega y correcciones.
- **Entregable del OE3** (`claudeDocs/entregables/OE3/src/`): capítulos 2, 4.3, 4.5, 5, 7 (inicio, menú de niveles,
  créditos, cuadro de diálogo, contraste del título), 10 y 11, la fila nueva de `12-control-cambios.md` y los
  anexos C (laberinto), D (cámara de la balsa), E (arte y sonido: la colaboradora, CLIP Studio y los cinco pasos,
  el halo, los entornos), F (apartado 2.12 nuevo: Papá, Algoritm por partes y `Wave`; 10 clips, 94 en total) y G.
  Figuras sustituidas en `docs/OE3/fig/`: 4.7 (`despues-05-bloques-y-cajon`), 7.1 (`despues-menu-principal`) y 7.2
  (`despues-seleccion-niveles-completado-disponible-bloqueado`). Tras la revisión (fase B): `rf_matrix.py --worktree`
  regeneró el Anexo A sobre `123785e` (857 métodos, 416 citan un RF, 47 de 47 RF cubiertos); el capítulo 9 y el
  Anexo F (§4.1 y Tabla F.13) ganaron la corrida del 09/10/2026 con su fecha, y las cifras del ejecutable siguen
  siendo las de rc2; `build.py --publish`: «guardas: todas pasan» y los `.docx` publicados.
- **Trabajo de grado** (`docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx`, por Word COM, con
  `claudeDocs/entregables/tools/insertar_parrafos.ps1` y `reemplazar_parrafos.ps1`): §5.1.4, §5.1.5, §5.2 (CLIP
  Studio en lugar de Illustrator y Photoshop), §8.1.1 (la producción de los sprites en cinco pasos) y §8.1.3
  (interfaz, narrativa y personajes de la ronda). Tras la revisión: la Figura 7 (laberinto) es la captura del 08/10
  y su fuente lo dice, y las cifras de la suite del 09/10/2026 se añadieron junto a las del 01/10 sin tocar las
  Tablas 9 a 11. De 75 a 76 páginas (se mantienen en 76); el capítulo 8 pasa de 7 a 8 de las 10 permitidas.

## 6. Revisión

Parte crudo: `notas-05-revision.md`. La revisión corrió el 09/10/2026 sobre `eb9be13` (el merge de la ronda con
el carril de perfil, `910c0f0`, INC-134 e INC-135); su corrección es el commit `123785e`. Compila sin errores y
no hubo choque de compilación entre los dos carriles.

**Choque semántico, uno solo y de prueba.** `ActorAction.Wave = 22` (D12) es la acción número 23, y
`ActionView_INC134_LaTablaCubreTodasLasAcciones`, del carril de perfil, fijaba el enum en 22 para obligar a
decidir la vista de cada acción nueva. `Wave` es un saludo a quien juega, así que se ve de frente, que es lo que
`ActionView.For` ya devolvía por omisión: la prueba espera ahora 23 acciones y lista `Wave` entre los gestos de
frente (`ActionView_INC134_LosGestosHaciaElEstudianteVanDeFrente`), en dos líneas de
`CharacterViewTests.cs`. El código de producción no cambia. `123785e` añade además los cinco `.meta` que Unity
generó para los scripts del carril de perfil.

**Suite completa** (`suite2.ps1`, dos Editores, 24,4 min de pared; ya con la corrección). Se listaron 1128
pruebas (711 EditMode y 417 PlayMode), corrieron todas una vez, no faltó ninguna y ninguna se repitió.

| | Total | Pasan | Fallan | Omitidas | Inconclusivas |
|---|---|---|---|---|---|
| EditMode (Editor del proyecto, recién abierto) | 711 | 706 | 4 (rojo esperado) | 1 (entorno) | 0 |
| PlayMode (Editores del proyecto y de la copia) | 417 | 416 | 0 | 0 | 1 (`Assume` del carril de perfil) |

PlayMode por assembly: `Game.UI` 177/177 (22,7 min, el carril más largo), `Game.Audio` 4/4, `Game.Core` 12/12,
`Game.Levels.Fire` 64/64, `Game.Levels.River` 57 de 58 (1 inconclusiva) y `Game.Levels.Wheel` 102/102.
`RiverLevel_RNF05` (memoria) pasó con el Editor recién abierto. Antes de corregir, la corrida de EditMode dio
705 de 711, con 5 fallos (los 4 de abajo y el de `ActionView`).

- **Rojos esperados (4):** `CharacterRig_INC134_LaFamiliaTieneCuerpoDePerfilConArte` para Papa, Mama, Nina y
  Nino (`existe Lienzo/Perfil`, `Expected: not null`), a la espera del arte de perfil en los prefabs; no se
  tocaron ni se saltaron.
- **Omitida (1):** `ProfileEraser_INC34_SobreDiscoRealCubreLosDosEscenariosDeAlmacenamiento`, un `Assume` de
  entorno (el equipo no hace cumplir el atributo de solo lectura en carpetas), anterior a la ronda.
- **Inconclusiva (1):** `RiverScene_INC134_MamaVaDePerfilAlCaminarYDeFrenteAlSoltarLaFlecha`, `Assume` porque
  el prefab de Mamá no tiene todavía cuerpo de perfil.
- Ningún otro fallo. Corrieron y pasaron por nombre las pruebas nuevas o ajustadas de las cuatro etapas
  (`MainMenu_RF01_*`, `LevelSelect_RNF19_*` y `…RF03_*`, `Credits_RF08_*`, `NarrativeScene_RNF01_*` con las 18
  narrativas, `…RF44_*`, `…RF05_CadaFogataTieneSuHalo…`, `…RNF21_ElHalo…`, `FireLevel_RF20_*`, las cinco del
  laberinto y las `DA133` de los personajes).

**D19, medido en Play a 1920 × 1080** por el flujo real del juego, con la caja de la balsa (484 px) registrada
cada 0,25 s. Con el foco de apertura en 0,50:

| Recorrido | Margen derecho mínimo | Parte más baja | Deslizamiento a la izquierda |
|---|---|---|---|
| Sin leer (sin avanzar el texto) | 446 px | y = 341 (la cima del cuadro está en 232) | 88 px |
| Leyendo (clics a 3; 5,5; 8; 11; 13,5 y 16 s) | 464 px | y = 267 (35 px por encima del cuadro) | 228 px |

El deslizamiento máximo es de 228 px, y con 0,38 eran 581. La balsa no se sale del cuadro (margen mínimo por
cualquier lado, 446 px) ni queda bajo el cuadro de diálogo, ni sin leer ni leyendo. La cifra de 77 px de la
etapa 3 era otra medida (el borde visible de la balsa en las capturas, con el arranque en 0,38).

**El carril de perfil integrado.** Ningún prefab tiene nodo `Perfil`, así que la vista es siempre de frente y no se
pudo ver el perfil en sí. En 1174 muestras de rig (el cruce 3.3 sin leer y leyendo, y `N1_Apertura`), `Walk`,
`Run`, `Idle`, `Talk`, `Celebrate`, `Encourage`, `Observe`, `Point` y `Surprise` salen de frente y sin voltear; ningún
personaje dibuja dos cuerpos. En el menú principal los cinco están en reposo, en poses distintas y sin voltear.
Las nueve piezas de Algoritm encajan sin huecos en reposo en la rueda y en la gota (hasta ahora solo se había visto
el fuego).

**No se pudo verificar.** El perfil en sí; el choque de `Strike` de Papá (D16) en el motor (solo se vio en renders
del Editor); los gestos de Algoritm en movimiento más allá de lo que mostraron las capturas de la etapa 4a; el
ejecutable (no se compiló ningún candidato, y el peso de las 27 piezas de Algoritm sigue sin medirse en un
build); la estabilidad (una sola corrida, y las `MainMenu_RF01_*` esperan solo dos fotogramas). Las pruebas
`VisualVerification` pasan por no tener `Assert`.

Capturas de la revisión en `capturas/05-revision/` (cruce sin leer y leyendo, hojas del desembarco, menú
principal, Algoritm en la rueda y la gota, muestra de las pruebas `VisualVerification`); resumen y registro de la
suite en `suite-05/`.
