# Personajes animados — resultados (24/09/2026)

Carril de arte, igual que `Slice 3/Props-y-Sonidos-Resultados.md`: toca `Assets/Game/Art/Characters/`,
`Assets/Game/Prefabs/Characters/`, `Game.Scaffolding`, `Game.UI`, los tres `Game.Levels.*`, las
dieciocho narrativas y las cinco escenas jugables. Lo pidió Santiago: la familia y Algoritm, con
animaciones, en **todo** el juego; objetos donde el guion los nombra; retrato en el cuadro de
diálogo, y el botón de ayuda con la forma de Algoritm, listo para sustituir el sprite.

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
  entonces salta a su destino antes de empezarlo.

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
- Arte de rueda y gota: sustituir los dos `.png` conservando nombre y `.meta`.
- Expresiones del retrato distintas de `neutra`: ninguna línea las pide todavía.
- Objetos que el guion nombra y no tienen sprite: comida, humo, estela de Algoritm, maleza, piedras
  en la mano. Tampoco puede aparecer un objeto a mitad de escena (el montón de la 1.2).
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
