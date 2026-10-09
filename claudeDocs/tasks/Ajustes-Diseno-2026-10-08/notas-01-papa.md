# Notas 01 — Papá: el húmero ya no asoma por el codo (D14)

Parte crudo del implementador de la etapa 1, 08/10/2026. Pedido de Santiago, literal: «En las animaciones
del papá, cuando él mueve el antebrazo se ve parte del húmero; por ende sube el húmero y baja un poco el
antebrazo para que no se vea esa parte de exceso del brazo.» Sin commit; sin tocar documentación.

## 1. Qué lo causaba

- El arte final de Papá trae el húmero con una **punta clara y sin contorno** que se pasa de su extremo
  redondo (30 y 32 px más allá del centro), y el antebrazo **solapado encima**: la distancia entre el centro
  del extremo del húmero y el del casquete del antebrazo (su extremo redondo del lado del hombro) es de
  **42 px a la izquierda y 21 px a la derecha** (Mamá 8 y 6, Niña 1 y 4, Niño 5 y 5).
- `preparar_arte_final.py` lo avisaba («la costura no será limpia») y lo dejaba en `holguras`
  (`CodoIzq: 42.0`, `CodoDer: 20.9`), que `pose_preview.py --mide` admitía como tolerancia.
- Por eso el **pivote del codo** quedó en el extremo del húmero (326, 588) y (672, 574), a 40,0 y 20,9 px del
  casquete (355,3; 560,8) y (656,7; 559,8). Con el brazo recto la punta queda bajo el antebrazo; al doblar el
  codo el casquete se aparta en un círculo de 40 o 21 px de radio y la punta asoma como un bulto sin
  contorno (en los dos codos: hasta 7 px de espesor y 175–195 px², en Idle con las manos en la cadera, Push, Hug,
  Observe, Hammer, Talk, PickUp…). Lo vi con el húmero teñido de magenta y en el Editor (antes/después abajo).
- Medida estática que lo distingue de los otros tres (`pose_preview.asoma_codo`: cuánto pasa el punto del
  húmero más lejano del centro del casquete, hacia la mano, del borde del casquete): **Papá 52,6 y 33,4 px**;
  Mamá −5,4 y −3,0; Niña −0,8 y −3,1; Niño 0,3 y 6,7. (Una medida por cuadro no sirve: el borde con contorno
  del húmero del Niño asoma tanto como el bulto de Papá, pero se lee como brazo grueso; lo que cambia es que
  el de Papá no tiene contorno y que su causa es el registro, no la pose.)

## 2. Qué cambié (px del lienzo del rig; 1024, origen arriba a la izquierda, y hacia abajo)

Regla (`codo.py`): el húmero **sube** y el antebrazo **baja**, a lo largo del eje del húmero, hasta que su
punto más lejano quede 4 px dentro del casquete. Piden R = asoma + 4 = 56,6 (izq.) y 37,4 (der.). Los **dos
antebrazos bajan lo mismo** (D = min R = 37,4) y el húmero que más pide sube el resto (izq.: 19,2; der.: 0). El
**pivote del hombro no se mueve**; el del codo va al centro del casquete.

| Dato (`arte_final.json` → `rig_articulaciones.json`) | Antes | Después |
|---|---|---|
| `BrazoIzq.rect` (húmero izq.) | [296, 457, 478, 612] | [311, 445, 493, 600] (sube 19,2: +15, −12) |
| `BrazoIzq.pivote` (hombro) | [406, 477] | igual |
| `CodoIzq.rect` (antebrazo izq.) | [178, 543, 376, 718] | [150, 567, 348, 742] (baja 37,4: −28, +24) |
| `CodoIzq.punto` (pivote del codo) | [326, 588] | [327, 585] |
| `BrazoDer.rect` / `.pivote` | [537, 457, 700, 601] / [611, 477] | igual |
| `CodoDer.rect` (antebrazo der.) | [637, 542, 835, 717] | [665, 566, 863, 741] (baja 37,4: +28, +24) |
| `CodoDer.punto` | [672, 574] | [685, 584] |
| `holguras` `CodoIzq` / `CodoDer` | 42,0 / 20,9 | 14,2 / 16,1 |
| `asoma_codo` izq. / der. (tope 15) | 52,6 / 33,4 | −3,2 / +1,0 |

El resto de `arte_final.json` (Rodillas, Cuello, cara, `registro`) y los PNG no cambian. **No toqué el arte
entregado**: ningún PNG se recorta ni se reescribe.

Cálculo y comprobaciones previas (en memoria, antes de escribir nada):
- Cuánto puede subir el húmero lo limita el torso, que lo esconde: con todo el solape en el húmero (hombro
  quieto) el reposo de los brazos pasaba de 15 a 49 grados; y, con la bajada igual en los dos brazos, desde 30 px en
  el húmero izquierdo el abrazo (`Hug`) rebasa la tolerancia de 6 grados del codo. Con todo en el antebrazo, un brazo
  quedaba 19 px más largo que el otro. Igualar la bajada (37,4) deja los dos brazos simétricos (`l1` 134/130 y
  `l2` 167/168 en el modelo del solucionador; antes 137/115 y 104/136).
- Con 4 px de margen el bulto residual medido en los 21 clips es de ≤ 3 px (≤ 33 px²), por debajo del que
  ya tienen Niña (5 px) y Niño (7 px) sin que nadie lo vea; con 8 px de margen baja a 1 px a costa de 4 px más de
  antebrazo. Dejé 4.
- Reproducibilidad: medir la entrega original de Papá (`entregas/2026-10-06/Papá/Frente`) con
  `preparar_arte_final.procesa`, poner los pivotes de hombro de la tabla y aplicar `codo.propone/aplica`
  da **exactamente** el `arte_final.json` de ahora (0 diferencias, mismas `holguras`).

## 3. Herramientas (`claudeDocs/tasks/Personajes/herramientas/`)

- **`codo.py`, nuevo** (como `hombro.py`): `propone` / `aplica` el ajuste de arriba sobre `arte_final.json` y
  regenera `rig_articulaciones.json` (`--aplica`), `--verifica` (sale con 1 si un húmero asoma) y
  `--autoprueba` (con el Niño: no pide nada; con los antebrazos subidos 40 px detecta y arregla; los dos
  antebrazos bajan lo mismo; el pivote del codo cae en el casquete; el del hombro no se mueve; idempotente; con
  un solo brazo desencajado el otro no se toca).
- **`pose_preview.py`**: comprobación **(j)** genérica para la familia: `asoma_codo` y `ASOMA_MAX_CODO = 15`.
  Corre en la prueba normal (una línea por codo de cada personaje con arte final, cuenta como fallo), en
  `--mide` y en `--autoprueba` (el Niño pasa; el mismo Niño con el antebrazo subido 40 px se detecta: 40 y 47 px).
  Hoy falla solo Papá (sin el arreglo: 2 fallos; con él, 0). El resumen final dice «fallos (clips o codos)».
- **`preparar_arte_final.py`**: `aplica()` corre `codo.py --aplica` justo después de `hombro.py` (3 líneas y un
  párrafo de cabecera). **`coreografia.py`**: solo un comentario de cabecera.
- `clips_personajes.json`: regenerado con `coreografia.py --valida`; cambian **solo los 21 clips de Papá**
  (Mamá, Niña, Niño y Algoritm idénticos al byte; reescribirlo de nuevo da el mismo SHA-256).

## 4. Editor (BuildRigsFinal)

`Assets/Editor/ClaudeBuildRigsFinal.cs` ← `BuildRigsFinal.cs.txt`, `editor.ps1 recompile`, y por
`editor.ps1 eval` (`ClaudeBuildRigsFinal.Execute("<modo>")`): `estado` → `sprites` → `clips` → `estado`. **`orden`
no hizo falta** (no cambia el orden de Tronco ni las anclas). Andamiaje **borrado** (`Assets/Editor` y sus
`.meta`); no queda nada en `Assets/` fuera de lo de abajo.

- `estado` antes y después: **idéntico** (siete prefabs con nodos completos, Tronco «cumple la tabla»,
  antebrazos en Tronco con ancla; Algoritm «no se sueltan»).
- `Papa.prefab`: 15 líneas cambian (15 + / 15 −), todas `m_AnchorMin/Max`, `m_Pivot` de **7 RectTransform**:
  `BrazoIzq`, `CodoIzq`, `CodoDer`, `AnclaAntebrazoIzq/Der` y `AntebrazoIzq/Der`. Ningún objeto nuevo ni perdido; no
  se quita ninguna línea `--- !u!`; `m_Script`, `m_Controller`, `m_Sprite`, `m_Name` y `m_GameObject` intactos.
- 21 `.anim` de Papá (solo `value`/`inSlope`/`outSlope`; en `golpear` un binding cambia de sitio dentro de
  `m_ClipBindingConstant`, mismo contenido). Ningún `.anim` de los otros cuatro cambia; ningún `.meta` cambia.
- `git status`: `Papa.prefab` y los 21 `char_papa_anim_*.anim` son lo único de `Assets/` que cambia por mí.
  (`Level2_Maze.unity` y `CLAUDE.md` ya venían modificados al empezar; no los toqué.)

## 5. Comandos y resultados (cifras reales)

| Comando | Resultado |
|---|---|
| `pose_preview.py` (línea base, antes de tocar) | «la prueba pasa en todos los clips»: 111/111 (4 × 21 + 3 × 9) |
| `codo.py` (informe) / `--aplica` | Papá izq. sube 19,2 y baja 37,4; der. sube 0 y baja 37,4; los otros tres no piden nada |
| `coreografia.py --valida` | `valida: bien`, exit 0; al repetirlo el JSON queda idéntico |
| `pose_preview.py` (completo, después) | **«la prueba pasa en todos los clips»**: 119 filas ok = 111 clips (21 por miembro de la familia, 9 por forma de Algoritm) + 8 codos |
| autopruebas: `coreografia`, `pose_preview` («la prueba detecta lo que debe»), `hombro`, `preparar_arte_final` («pasa, error máximo 1,0 px»), `preparar_expresion`, `codo` | todas exit 0 |
| `pose_preview.py --mide papa` | codo en el casquete a 0,4 px; hombro a 1,07 y 1,08 radios del eje; «Codo asoma» −3,2 y 1,0; exit 0 |
| `hombro.py --verifica` | los ocho hombros cumplen la regla (Papá izq. 46 %, der. 50 % de húmero visible a 18°) |
| `editor.ps1 tests-edit CharacterRig_` | **142/142** |
| `editor.ps1 tests-edit Game.Scaffolding.Tests assembly` | **278/278** (las 278 de antes) |

No corrí PlayMode ni `VisualVerification`. La consola del Editor no tiene errores; los avisos de `LimbFollower` y
`ArmLayering` salen de las pruebas que los provocan a propósito (`…UnAnclaQueNoCuelgaDeTroncoAvisa…`,
`…SiFaltaUnNodoElGolpeAvisa…`).

## 6. Efectos colaterales que Santiago debería ver (no los pidió)

- **Los brazos de Papá miden ~37 px más** (la mano baja ~24 px y se aleja ~28 px del cuerpo; 4 % de su alto): es
  «bajar el antebrazo». Punta de la mano, antes → después, en los 21 clips: media 35 px, máximo 67 px. Los clips por
  ángulo (Walk, Talk, Carry, Point, Celebrate…) quedan igual salvo ese desplazamiento.
- El modelo del solucionador mide ahora el antebrazo como está dibujado (167 px; antes 104 y 136 porque el codo
  estaba 40 y 21 px dentro del antebrazo): `alcance` 240,6 → 301,0 y `y_pecho` 533,4 → 565,5. Los gestos por objetivo
  se resuelven con otro codo: **`Strike` es el que más cambia**: el choque baja de y = 549 a y = 595 (tope de la
  cintura 616) y los codos se abren más (los antebrazos casi horizontales); la mano izquierda se mueve 62 px de media.
  Pasa la prueba (h) y todas las demás, pero **se ve distinto**: ya estaba «pendiente de que Santiago lo apruebe» (C.9).
  `Hug`, `Idle` con las manos en la cadera, `Push` y `Observe` (la cara visible pasa de 92,5 % a 100 % y el húmero
  visible de 44,7 % a 67,2 %) también cambian algo. Si prefiere el choque más alto, se ajusta `yy` de `strike()` solo para Papá.
- El húmero visible baja unos puntos (Idle 48,7 → 44,3 %; el piso de la prueba es 30 %).
- El reposo no cambia: 15° de hombro y 6° de codo (el solucionador lo mantiene).

## 7. Capturas (`capturas/01-papa/`)

- `antes_codos_tinte.png` / `despues_codos_tinte.png`: codos izquierdo y derecho en cinco poses (Idle, Push, Hug,
  Observe, Hammer) con el **húmero en magenta**: antes se ve la punta fuera del casquete; después no.
- `antes_codos.png` / `despues_codos.png`: lo mismo con los colores reales. `antes_papa_cuerpo.png` /
  `despues_papa_cuerpo.png`: cuerpo entero en seis instantes. (Renderizador de `pose_preview.py`, con los JSON de antes
  y de después.)
- `antes_editor_codos.png` / `despues_editor_codos.png`, `antes_editor_papa_idle_cadera.png` /
  `despues_editor_papa_idle_cadera.png`: **render del Editor de Unity en modo edición** (sin Play) del prefab y los `.anim`
  reales, antes (copias temporales del prefab y los clips de `HEAD`, ya borradas) y después. Cámara ortográfica a una
  `Canvas` World Space en una escena aditiva temporal, `AnimationClip.SampleAnimation` más `LimbFollower.Sync` (por
  reflexión: es `internal`) para que el antebrazo siga al ancla. La escena activa (`Narrative`) sigue limpia.
  Confirman en Unity lo que predice el modelo: el bulto está antes y no está después.
- Instantes (clip, t): Idle 3,68 s (cadera), Push 0,41 s, Hug 1,10 s, Observe 0,30 s.

## 8. Qué debería correr el revisor (PlayMode)

`tests-play` filtrado, con el Editor recién abierto: `FireLevel_DA133_` y `FireLevel_CP02_` (Papá recoge, golpea,
sopla y se anima en la cueva), `WorkshopScene_DA133_` (Papá martilla), `NarrativeScene_RF05_` (las narrativas con
Papá, p. ej. la 2.2), y `Personajes_DA133_` (**VisualVerification**: `Personajes_Mecanica_*.png` de `Level1_Cave`,
`Level2_Forest`, `Level2_Workshop`, `Level2_Maze` y `Level3_River`: mirar los codos y las manos de Papá, sobre todo
el golpe del N1). Y la suite completa de la verificación final. Ningún código de juego referencia los nodos del
brazo salvo `CharacterRig`, `LimbFollower` y `ArmLayering`, así que no esperaría fallos de lógica; lo que sí conviene
mirar es que ninguna narrativa ponga un objeto en las manos de Papá a ojo (las manos quedan ~35 px más abajo).

## 9. Dudas abiertas

1. ¿Le parece bien a Santiago el **Strike** nuevo (más bajo y con los codos abiertos), o prefiere conservar el choque
   a la altura del pecho? (Ver §6.)
2. «Baja un poco el antebrazo»: 37 px (4 % del alto) es el mínimo que esconde la punta sin abrir el reposo de los
   brazos ni pasar la tolerancia del codo de `Hug`. Más cerca del arte entregado solo se llega **recortando la punta
   sin contorno en los dos PNG del húmero**, lo que cambia arte entregado y no hice.
3. `ASOMA_MAX_CODO = 15` px y el margen de 4 px son tolerancias de la herramienta (no son cifras del juego).
4. `codo.py` es un archivo nuevo (como `hombro.py`); sin él el arreglo sería solo un número escrito a mano en
   `arte_final.json` que `preparar_arte_final.py --aplicar` volvería a pisar.
