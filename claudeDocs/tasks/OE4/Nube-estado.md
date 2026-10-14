# Estado de la sesión en la nube (rama `ajustes-OE4`)

Plan: `claudeDocs/tasks/OE4/Plan-Sesion-13-10.md`, sección «Reparto con la sesión en la nube».
La sesión en la nube actualiza este archivo en cada push: tarea, commit, qué quedó y qué falta.

| Tarea | Estado | Commit | Notas |
|---|---|---|---|
| C1 Vocabulario (INC-157) | hecho, falta cotejo local | `6d26abb` | 22 pares en `tools/pares/guion-inc157.json`, sin cotejar con el guion radicado |
| C2 Oscuridad y caminata (INC-152, INC-153) | hecho, falta Editor | `52ecca5` | Prueba nueva `NarrativeLocomotionTests`, compilada con `mcs` y sin ejecutar |
| C3 Resultados en markdown (INC-151) | hecho | `c62a09f` | Formato de consentimiento en `evidencias/consentimiento/` |
| C4 Hallazgos y correcciones | hecho; INC-154 cotejado en C7 | `f1b689c` | Sobre `52908f1` |
| C5 Créditos (INC-156) | hecho | `ca51d02` | |
| C6 Grado quinto en la tesis (INC-151) | hecho | `5f573b7` | |
| C7 Cotejo de INC-154 con `d8ab0f4` | hecho | `2681a39` | Falta PlayMode y captura del motor |
| C8 Cifras en `OE4-Resultados.md` y `todo.md` | hecho | `37fa4e3` | 97,0 % (128/132, 4 PS) |
| C9 OE3 al estado vigente | hecho, falta publicar | `24aee19` | Solo markdown; `build.py --publish` en local |
| C10 Triaje y `PF-RNF12-01` | hecho | `e4051d3` | 97,0 % (129/133, 3 PS) |

## Lo aplicado (13/10/2026)

### C1 · Vocabulario (INC-157), `6d26abb`

| Archivo | Texto viejo | Texto nuevo |
|---|---|---|
| N1_Apertura | camina a la intemperie | camina al aire libre, sin techo |
| N1_Apertura | la silueta de una cueva | la sombra de una cueva |
| N1_Apertura | entre la maleza | entre los matorrales |
| N1_AparicionGuia | una estela de puntos luminosos | un rastro de puntos luminosos |
| N1_Hallazgo | busca a tientas un montón | busca a ciegas un montón |
| N1_NacimientoDelFuego (solo texto) | el fuego crepita | el fuego chisporrotea |
| N1_NacimientoDelFuego (solo texto) | una silueta pequeña | una sombra pequeña |
| N2_Escena21_Bosque | Objetos dispersos por el suelo: piedras irregulares | Objetos regados por el suelo: piedras de formas distintas |
| N2_PuenteI_Bosque | mantener avivado el fuego | mantener vivo el fuego |
| N3_Escena31_Llegada | Este río no es sencillo | Este río no es fácil |
| N3_Escena31_Llegada | que permanecerá visible | que se queda a la vista |
| N3_Escena33_Cruce | flota con estabilidad | flota derecha, sin voltearse |
| N3_Escena33_Cruce | Llegan a la orilla opuesta. El humo de las fogatas de la civilización se divisa cerca | Llegan a la otra orilla. El humo de las fogatas de otras familias se ve cerca |
| N3_Escena33_Cruce | Eso es pensar computacionalmente. | Eso es pensar como un algoritmo. |
| N3_EscenaFinal | Fundido a negro. | Todo queda en negro. |
| N3_PuenteII_Horizonte | fogatas de la civilización | fogatas de otras familias |
| N3_PuenteII_Rio | se corta abruptamente | se corta de repente |
| River/N3_RaftAssemblyContent | La pieza vuelve al inventario. (y el final de `MissedMessage`) | La pieza vuelve a tus materiales. |
| River/N3_RiverLevelConfig | `CollectedFormat`: {0}: al inventario. | {0}: guardado en tus materiales. |
| River/N3_RiverLevelConfig | `TaskLabels[2]`: Ensamblar la balsa | Armar la balsa |
| Wheel/N2_ResumenNivel | casi de inmediato | casi enseguida |

- «abstraer», «iterar», «descomponer», «depurar», «algoritmo» y «patrón» se conservan; ya traen su explicación al lado.
- Por «Armar la balsa» se actualizaron `RiverLevelConfigTests.cs`, `TaskListTests.cs` y `RaftAssemblyTests.cs`.
- **Para la sesión local:**
  - Cotejar `guion-inc157.json` con `docs/md/Solucion_OE2_Diseno_final.md` antes de aplicarlo al `.docx`. Los pares salen del asset, no del guion.
  - `RiverLevelConfig.cs:33` todavía tiene «Ensamblar la balsa» como valor por defecto; el asset lo sobrescribe.
  - El botón «Mecanizar» del taller está en la escena o el prefab y no se tocó.

### C2 · Oscuridad (INC-152) y caminata (INC-153), `52ecca5`

| Archivo | Paradas (línea) | Ambient viejo → nuevo |
|---|---|---|
| N1_Apertura | LightStart, 0, 2, 3 | 0,06 → 0,18 · 0,06 → 0,18 · 0,09 → 0,21 · 0,04 → 0,16 |
| N1_Apertura | 6 (HardCut) | 0 → 0, sin cambio |
| N1_AparicionGuia | LightStart, 0, 3, 4, 6, 8, 10 | 0,02 / 0,02 / 0,03 / 0,03 / 0,03 / 0,03 / 0 → 0,15 en todas |
| N1_Hallazgo | LightStart, 0, 2, 5, 7, 8, 9, 12, 14, 16 | 0,08 → 0,20 · 0,08 → 0,20 · 0,07 → 0,19 · 0,03 → 0,15 · 0,04 → 0,16 · 0,06 → 0,18 · 0,08 → 0,20 · 0,08 → 0,20 · 0,06 → 0,18 · 0,05 → 0,17 |
| N1_Hallazgo | 6 (destello del ¡CLIC!) | 0,95 → 1,0 (la regla daba 1,07; se topó en 1, el máximo de `NarrativeLight.Ambient`) |

Línea 7 de `N1_AparicionGuia`: los cuatro beats Walk llevan `Facing: 1` y la `y` no cambió.

| Personaje | Origen x | `To` antes | `To` ahora |
|---|---|---|---|
| Papá | 0,285 | 0,281 | 0,18 |
| Niña | 0,305 | 0,271 | 0,195 |
| Mamá | 0,335 | 0,325 | 0,215 |
| Niño | 0,337 | 0,303 | 0,22 |

- **Prueba nueva:** `NarrativeSequence_INC153_LosPasosDeLocomocionDesplazanAlActor`, en `Assets/Tests/EditMode/Scaffolding/NarrativeLocomotionTests.cs`.
  - Lee `MinimumDisplacement` por reflexión y toma el origen con `ActorTimeline.PositionBefore`.
  - Compila con `mcs` contra los `.cs` reales de la timeline y stubs de Unity y NUnit.
  - Una simulación en Python sobre los YAML da 0 fallos.
  - **No tiene `.meta`**: lo crea Unity al importar.
- **Otros tres beats Push que la prueba encontró** se corrigieron en la misma dirección, con desplazamiento 0,012:
  - `N2_Escena21_Bosque`, objeto 0, línea 1: 0,232 → 0,224.
  - `N2_PuenteI_Bosque`, objeto 0, línea 3: 0,736 → 0,73.
  - `N2_PuenteI_Bosque`, objeto 8, línea 6: 0,673 → 0,68.
- **Para la sesión local, en el Editor:**
  - Correr la prueba nueva y `NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo`.
  - Mirar el encuadre de la línea 7 de `N1_AparicionGuia`: la familia está en x 0,18-0,22 y Algoritm cerca.
  - Comprobar que la luz de `N1_Apertura` no lave la escena y que la línea 6 siga en negro.
  - Mirar que los tres pasos Push del N2 se vean como empujes y no como un giro.

### C3 · Resultados en markdown (INC-151), `c62a09f`

- **Archivos:** `OE4/src/01` a `06`, `TG/cap9-evaluacion.md`, INC-151 (rev. 25) en `INCONSISTENCIAS.md`, y notas fechadas en `OE4-Resultados.md` (§12), `Hoja-HUM.md` (Anexo C) y `todo.md`. No se marcó ninguna casilla.
- **Formato de consentimiento:** `claudeDocs/tasks/OE4/evidencias/consentimiento/Formato-consentimiento.md`.
  - En blanco, sin datos de menores y para grado quinto.
  - Firma la madre, el padre o el acudiente que sea representante legal.
  - Autoriza la sesión y el perfil con código o apodo; el firmante puede retirar la autorización y los perfiles se borran con «Eliminar datos».
  - Cita la autorización del rector del 17/09/2026.
  - El cap9 lo llama **Anexo F**: la sesión local lo anexa a los `.docx`.
- **Pendientes:**
  - Los veredictos H1-H12 no se pasaron a las Tablas 3.1 y 3.3, al Anexo A ni a `veredictos-rc3.csv`. Las cifras siguen con el corte del 10/10, y el texto lo dice.
  - Hay que regenerar los JSON de `pares/` desde el cap9 (`cap9_a_json.py`).
  - Queda un `[Pendiente]` en `03-resultados.md` (triaje de 16 defectos) y el de la Familia Anonaky en el cap9.

### C4 · Hallazgos e INC-152 a INC-160 (sobre `52908f1`)

- **Entregable OE4:** `05-estudiantes.md` tiene un apartado 5.3 nuevo con la Tabla 5.4 (diez hallazgos con su corrección); `04-mejoras.md` lleva la referencia cruzada desde 4.6.
- **`INCONSISTENCIAS.md`:** entradas INC-152 a INC-160, rev. 26, y una nota fechada junto a INC-49, al que INC-159 deja sin efecto. INC-155 anota que `WheelLevelConfig_RF23_…` pasó a `LessThanOrEqualTo`.
- **`Interfaces.md`:** la pausa con cinco botones y el panel de confirmar salida, el aviso del botón de ayuda, el botón «Atrás» y las flechas de giro.
- **`CLAUDE.md`:** solo la línea del grado, que pasa a quinto. `Inventario.md` no se tocó.
- **Para la sesión local:**
  - **INC-154 se escribió según el plan (P2), no según un diff.** Está como «registrado, código en curso» y las filas de abrazo y sueño de la Tabla 5.4 dicen «según el plan». Hay que cotejarlo con el commit que lo traiga y actualizar la lista de acciones de perfil de INC-134.
  - **PlayMode, la suite completa y rc4** constan como pendientes; no se afirma el peso de rc4.
  - **Los casos `PF-RF07-*`** describen la pausa de tres botones de rc3.
  - **INC-159:** RF-07 y HU-17 radicados siguen con tres rótulos.
  - **INC-151:** el `.docx` de la tesis pasa entero a «quinto» en local, con Word.
  - **`Interfaces.md`** sigue citando `#E8A33D` en la fase 3 del N2, donde `CLAUDE.md` dice `#C4A882`. No es de esta tanda y no se tocó.

### C5 · Créditos, `ca51d02`

Además de «Iconos de pausa y de giro: Phosphor Icons, licencia MIT.», la línea anterior pasó a «…salvo los iconos de pausa y de giro.», para que no dé por originales las flechas. Ninguna prueba cita ese texto.

### C6 · Grado, `5f573b7`

`05-estudiantes.md` y `cap9-evaluacion.md` ahora dicen que la tesis pasa entera a grado quinto y que solo los radicados del OE1 y del OE2 siguen en cuarto.

## Orden de la sesión local (13/10/2026, tarde) — empezar C4 y C5–C6

El código local está en `52908f1` (INC-152 cueva, INC-155, INC-156, INC-158, INC-159, INC-160). Lee su
diff (`git show --stat 52908f1` y los hunks que necesites) y escribe **solo lo que diga el diff**.
INC-154 (abrazo y sueño de perfil) sigue en curso en local: escríbelo según el plan
(`Plan-Sesion-13-10.md`, P2) marcando en `Nube-estado.md` que hay que cotejarlo con el commit que lo traiga.
PlayMode, suite completa y rc4 aún no se corrieron: no afirmes resultados de PlayMode ni peso de rc4.

- **C4** — como dice `Encargo-Nube.md`: sección de hallazgos y correcciones en el OE4 (`src/`),
  INC-152 a INC-160 en `INCONSISTENCIAS.md` (INC-159 deja sin efecto INC-49: la pausa pasa de 3 a 5
  botones; INC-155 relaja `WheelLevelConfig_RF23_…` a «menor o igual»), `claudeDocs/Interfaces.md`
  (pausa, aviso de ayuda, botón Atrás), la línea de grado de `CLAUDE.md` («grado cuarto» → «grado quinto»).
  `Assets/Game/Art/Inventario.md` ya trae las flechas: no lo dupliques.
- **C5** — créditos: «Iconos de pausa: Phosphor Icons, licencia MIT.» pasa a «Iconos de pausa y de giro:
  Phosphor Icons, licencia MIT.» en `Assets/Game/Data/CreditsContent.asset` y en el valor por defecto de
  `Assets/Game/Scripts/Runtime/UI/CreditsContent.cs` (INC-156). Si una prueba cita el texto, actualízala.
- **C6** — corrige la frase que dice que el trabajo de grado habla de grado cuarto en
  `claudeDocs/entregables/OE4/src/05-estudiantes.md` (~l. 9) y `claudeDocs/entregables/TG/cap9-evaluacion.md`
  (~l. 156): la tesis pasa entera a grado quinto; **solo** los radicados del OE1 y del OE2 siguen diciendo
  cuarto (INC-151).

Mismas reglas de identidad (`santivaldiri`, sin coautor) y de archivos vedados que en `Encargo-Nube.md`;
además no toques `ActionView.cs`, `CharacterViewTests.cs` ni `claudeDocs/tasks/Personajes/herramientas/*`.

## Orden de la sesión local (13/10/2026, 14:45) — C7: cotejar INC-154

INC-154 llegó en `d8ab0f4`. Coteja lo que escribiste de INC-154 (OE4 `src/`, `INCONSISTENCIAS.md`,
`Interfaces.md` si aplica) con ese diff y corrige lo que no coincida. Datos del commit:
- `ActionView`: Hug y Sleep → perfil; `CharacterViewTests` con `ActionView_INC154_AbrazarYDormirVanDePerfil`.
- Clips `char_{papa,mama,nina,nino}_anim_{abrazar,dormir}.anim` regenerados (solo curvas de `Lienzo/Perfil`);
  ningún prefab reconstruido. Abrazo: los dos brazos hacia delante; los adultos bajan las manos hacia los niños.
  Dormir: `Lienzo/Perfil` a +90°, boca arriba, respiración de 4 s; la cabeza queda **opuesta** a la mirada
  (a −90° quedaban boca abajo y sin ojos visibles).
- `N1_NacimientoDelFuego`: `Facing` explícito en los abrazos (L4, L5, L12) y en L16 cada uno camina a su
  sitio alrededor del fuego y llega dormido, sin taparlo. `N1_Hallazgo` L12: Mamá abraza mirando a la
  izquierda (`Facing: 1`), hacia el niño.
- Verificado en EditMode (`Character` 220/220, `NarrativeSequence` 34/34 con RNF-03) y con `pose_preview.py`;
  **sin** PlayMode ni captura del motor todavía.
Añade además una entrada fechada en `claudeDocs/tasks/Personajes/Personajes-Resultados.md` (registro
histórico: solo se añade, no se reescribe). Mismas reglas de identidad y archivos vedados.

### C7 · INC-154 cotejado con `d8ab0f4`, `2681a39`

- **`INCONSISTENCIAS.md`:** INC-154 pasa de «registrado, código en curso» a **cerrado**.
  - La entrada se reescribió con lo que trae el diff: `ActionView`, los clips (solo `Lienzo/Perfil`), el abrazo con los dos brazos hacia delante, el sueño a +90° boca arriba con una respiración de 4 s, y el `Facing` de las líneas 4, 5, 12 y 16 de `N1_NacimientoDelFuego` y de la línea 12 de `N1_Hallazgo`.
  - Ahí mismo, la lista de acciones de perfil de INC-134 se amplía con `Hug` y `Sleep`.
  - También se actualizaron la cabecera y la rev. 26.
- **OE4, `05-estudiantes.md`:** en la Tabla 5.4, las filas del abrazo y del sueño ya no dicen «según el plan». El texto cita `d8ab0f4` y lo que la sesión local verificó: EditMode `Character` 220 de 220 y `NarrativeSequence` 34 de 34, sin PlayMode.
- **`Personajes-Resultados.md`:** nueva entrada C.16, fechada y añadida al final.
- **`Interfaces.md`** no menciona el abrazo ni el sueño, y no se tocó.
- **Para la sesión local:** `CLAUDE.md` (apartado de los personajes) todavía lista como acciones de perfil solo `Walk`…`Blow`. Falta `Hug` y `Sleep`, pero la orden solo pedía la línea del grado y no se tocó.


## Orden de la sesión local (13/10/2026, noche) — C8: cifras de `OE4-Resultados.md` y `todo.md`

En `e9a26fc` la eficacia del OE4 pasó de 96,8 % (122/126, 10 PS) a **97,0 % (128/132, 4 PS)**: la sesión
manual del 13/10/2026 sobre rc3 cerró como P `PF-RNF07-02` (H3), `PF-RNF08-01` (H4), `PF-CT02-01` (H5) y
`PF-SON-01..03` (H12, con H6/H7). Siguen PS `PF-RNF13-01` (H8: el caso pide tres recorridos y los del arnés no
se corrieron), `PF-RNF12-01`, `PF-RNF22-01`, `PF-RNF23-01`. H13 no se hizo: `PF-RNF02-01`, `PF-RF34-01`,
`PF-RF46-01` conservan su P de rc3. Por sesión: S-RNF 6·0·0, S-HUM 5·0·1; RNF 82,4 % (14/17); SON 100 % (3/3).
Lee `claudeDocs/entregables/OE4/src/veredictos-rc3.csv` y `03-resultados.md` para confirmarlo.

- Añade a `claudeDocs/tasks/OE4/OE4-Resultados.md` una nota **fechada** (registro histórico: no reescribas lo
  anterior) con esas cifras, y marca en `todo.md` lo que la sesión cerró (T19 HUM salvo H13, T20, Checkpoint F
  parcial). No afirmes nada de PlayMode ni de rc4: siguen en curso en local.
Mismas reglas de identidad y archivos vedados.

### C8 · Cifras de `OE4-Resultados.md` y `todo.md`, `37fa4e3`

- **Comprobación:** las cifras se cotejaron con `veredictos-rc3.csv` (128 P, 4 F, 4 PS) y con `03-resultados.md` de `e9a26fc`. Las del 10/10 salen de `03-resultados.md` en `4dfcf35`.
- **`OE4-Resultados.md`:** nueva §13, fechada, que no reescribe nada anterior.
  - Recoge los seis casos que pasaron a P, los cuatro que siguen en PS y H13.
  - Trae una tabla de antes y después: eficacia 96,8 % → 97,0 %; S-RNF 5·0·1 → 6·0·0; S-HUM 0·0·6 → 5·0·1; RNF 78,6 % → 82,4 %; SON 100 %.
- **`todo.md`:**
  - Se marcaron T19 (salvo H13) y T20, cada uno con su nota.
  - Del checkpoint F solo se marcó la casilla de S-HUM y PG-05/PG-06.
  - Se añadió una nota fechada.
  - T21, el checkpoint D y el resto del checkpoint F siguen abiertos.
- **Para la sesión local:** T20 queda marcado como pide la orden. Su criterio menciona cargas y memoria también en el equipo 2, pero RNF-04 y RNF-05 siguen medidos solo en el equipo 1, y la nota de T20 lo dice. Nada de esto viene de PlayMode ni de rc4.


## Orden de la sesión local (13/10/2026, cierre) — C9 y C10

Decisiones del usuario (13/10/2026): triaje de los 16 defectos de rc3; `PF-RNF12-01` pasa a **P**; se
redactan formatos de autorización para la Familia Anonaky y la colaboradora de arte (los hace la sesión local en
Word); se corrigen en código DEF-RC3-05, -09, -10 y -11 (en curso en local, entrarán en un rc4 recompilado).

- **C9 · OE3 al estado vigente.** El entregable del OE3 se reescribe al estado vigente del prototipo (acta D10).
  Actualiza `claudeDocs/entregables/OE3/src/` y `src/anexos/` con INC-151 a INC-160: grado quinto, pausa de 5
  botones, aviso del botón de ayuda, botón Atrás (Omitir encima de Continuar), troncos, flechas curvas,
  vocabulario, abrazo y sueño de perfil, luz del N1, caminata de 1.1. Lee `INCONSISTENCIAS.md` (INC-151..160) y los
  commits `52908f1`, `d8ab0f4`, `56897fd`. Sin «§», sin Anexo H. La publicación (`build.py --publish`, Word COM) la
  hace la sesión local: solo markdown.
- **C10 · Triaje y RNF-12 en el OE4.**
  - `PF-RNF12-01` → P en `claudeDocs/entregables/OE4/src/veredictos-rc3.csv` (comentario: formato en el Anexo F
    y 4 consentimientos firmados, sesión del 13/10/2026). Recalcula y propaga: P/F/PS = 129/4/3, eficacia 97,0 %
    (129/133) — compruébalo con el CSV —, RNF y por sesión, en `src/*.md`, `claudeDocs/entregables/TG/cap9-evaluacion.md`
    y una nota fechada en `claudeDocs/tasks/OE4/OE4-Resultados.md`.
  - Apartado 3.6 de `03-resultados.md`: cambia el `[Pendiente: triaje…]` por la decisión, fecha 13/10/2026:
    **documento** DEF-RC3-02, -04, -06, -07, -12 (y aplica la corrección en `claudeDocs/tasks/OE4/casos.md`:
    02 describe la portada como es; 04 sin coordenadas fijas de las siete piezas; 06 el paso 21 de S-N3 como se
    reproduce; 07 admite ±1 en 8 bits; 12 lista las cinco pruebas que fallan al retirar un nivel como esperadas —
    lee el defecto en `OE4-Resultados.md` para los nombres); **código** DEF-RC3-05, -09, -10, -11 («se corrige en
    rc4», la sesión local confirmará con el commit); **trabajo futuro** DEF-RC3-01, -03, -08, -I01, -I02, -I03, -I04,
    cada uno con su causa. Refleja lo mismo en la tabla de 3.6 y en `cap9-evaluacion.md` (9.3.1) si los cita.
Mismas reglas de identidad y de archivos vedados. No toques `.docx`.

### C9 · OE3 al estado vigente, `24aee19`

- **Alcance:** 18 archivos de `claudeDocs/entregables/OE3/src/` y de `src/anexos/`, con INC-151 a INC-160, cada cambio contrastado con `52908f1`, `d8ab0f4`, `56897fd`, `OE4-Resultados.md` §14 y `build-rc4.md`.
- **Qué entró:** grado quinto y pausa de cinco botones; el aviso del botón de ayuda y el botón «Atrás», con la disposición final de `56897fd`; troncos y flechas curvas; vocabulario («Armar la balsa», «pensar como un algoritmo»); abrazo y sueño de perfil; luz del N1 y caminata 1.1.
- **Dónde:** apartado 2.6 nuevo del Anexo G y 2.15 del Anexo F; filas nuevas en las Tablas 5.4, 6.3, 9.6, B.4, C.5, D.4, E.4, E.9, F.5, F.6, F.13 y G.2 a G.11 (las que aplican), y una fila del 13/10 en el control de cambios.
- **Guardas a mano:** cero «§» y cero «Anexo H». Las tablas y figuras que cita el trabajo de grado siguen en su sitio.
- **Revisión:** se quitaron, en el OE3 y en `INCONSISTENCIAS.md` (INC-152, INC-156 e INC-160), frases que atribuían a los estudiantes dichos sin fuente («dijeron», «no entendían», «pidieron»). Ahora dicen que la sesión dejó el hallazgo, salvo la oscuridad, que el encargo sí registra («vieron el inicio del Nivel 1 demasiado oscuro»).
- **Para la sesión local, antes de `build.py --publish`:**
  - Regenerar el Anexo A con `rf_matrix.py --worktree`, que sigue sobre `123785e`. INC-152 a INC-160 añadieron pruebas; puede cambiar la Tabla 9.4 y su texto, y la fila del control de cambios dice «sin modificar el Anexo A».
  - Correr las guardas completas (pandoc y Word) y mirar en el PDF las celdas largas de F.13 y 9.6.
  - El ámbar `#E8A33D` sigue en C (2.4) y E (2.1), aunque se revirtió a `#C4A882` el 10/10. No es de esta tanda y no se tocó.
  - Los capítulos 9 y 10 conservan las cifras del OE4 al corte (103 casos); el estado vigente lo lleva el OE4.
  - En la columna «Autor» de G.2 y en el control de cambios se supuso `santivaldiri` = Santiago Valdiri García y `Brey314` = Santiago Benavides Rey.
  - `01-objetivo.md` cita el objetivo general con «quinto»: el `.docx` de la tesis debe decir lo mismo.

### C10 · Triaje y `PF-RNF12-01`, `e4051d3`

- **Cifras** (del CSV): 129 P, 4 F y 3 PS (`PF-RNF13-01`, `PF-RNF22-01`, `PF-RNF23-01`); eficacia 97,0 % (129/133); RNF 83,3 % (15/18); S-INSP 8·3·2 (72,7 %); peor caso 94,9 % (129/136).
  - Se propagaron a `OE4/src` (02 a 06), al cap9 (Tablas 12 y 13 y 9.3.1) y a una nota §15 en `OE4-Resultados.md`.
- **Triaje en 3.6, con columna nueva en su tabla:**
  - **Documento:** DEF-RC3-02, -04, -06, -07 y -12, corregidos en `casos.md`.
  - **Código:** DEF-RC3-05, -09, -10 y -11, «se corrige en rc4»; no se afirma que ya estén corregidos.
  - **Trabajo futuro:** DEF-RC3-01, -03, -08 e -I01 a -I04, cada uno con su causa.
- **`todo.md`:** T21 marcado con nota fechada; T22 sigue abierto.
- **Para la sesión local:**
  - `build.py` del OE4 no corrió aquí porque falta `docs/md/` del OE1.
  - Revisar que el nuevo paso 21 de S-N3 en `casos.md` (DEF-06) se pueda ejecutar con el arnés.
  - El `[Pendiente: …]` de `PF-RNF22-01` y `PF-RNF23-01` en la Tabla 16 del cap9 sigue igual.

