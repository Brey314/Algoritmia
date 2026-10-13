# Estado de la sesión en la nube (rama `ajustes-OE4`)

Plan: `claudeDocs/tasks/OE4/Plan-Sesion-13-10.md`, sección «Reparto con la sesión en la nube».
La sesión en la nube actualiza este archivo en cada push: tarea, commit, qué quedó y qué falta.

| Tarea | Estado | Commit | Notas |
|---|---|---|---|
| C1 Vocabulario (INC-157) | hecho, falta cotejo local | `6d26abb` | 22 pares en `tools/pares/guion-inc157.json`, sin cotejar con el guion radicado |
| C2 Oscuridad y caminata (INC-152, INC-153) | hecho, falta Editor | `52ecca5` | Prueba nueva `NarrativeLocomotionTests`, compilada con `mcs` y sin ejecutar |
| C3 Resultados en markdown (INC-151) | hecho | `c62a09f` | Formato de consentimiento en `evidencias/consentimiento/` |
| C4 Hallazgos y correcciones | espera el push del código local | | |

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

### C4

Espera a que este archivo indique el commit del código local.
