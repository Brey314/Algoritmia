# Movimientos de cámara del Nivel 2 — inventario

Foto del **11/09/2026** (W07-R a W07-R4). Lista todo movimiento de cámara que existe hoy en el
prototipo, en qué parte de la narrativa ocurre y **quién lo decidió**: lo que Santiago ordenó en
revisión frente a lo que Claude puso por su cuenta como primera propuesta. Todos los valores son
contenido de los assets (`Assets/Game/Data/Narrative/N2_*.asset`, `N2_WheelLevelConfig.asset`) y
se afinan en el Inspector sin tocar código.

Leyenda: **[S]** ordenado por Santiago · **[C]** decisión de Claude (primera propuesta, sin revisar
a ojo) · **[S+C]** el qué lo pidió Santiago, los números los puso Claude.

---

## 1. Cómo funciona la cámara (mecanismo)

| Pieza | Qué hace | Quién |
|---|---|---|
| `IllustrationFraming` | La ilustración **cubre** la pantalla sin deformarse a cualquier resolución del arte; sustituir el `.png` basta. | **[S]** «que se ajuste automáticamente al tamaño de la pantalla» |
| `CameraFraming` (foco + zoom) | Un encuadre = qué punto de la imagen queda en el centro y con cuánto acercamiento. Zoom < 1 se lee como 1: nunca se descubre un borde. | **[C]** |
| `CameraStart` → `CameraEnd` | Sin paradas, la cámara va del encuadre inicial al final **según cuánto va leído** (`DialogueRunner.Progress`), no según el reloj. | **[S]** «acorde a la narrativa» · el mecanismo por progreso **[C]** |
| `CameraKeys` (paradas por línea) | Con paradas, la vista se queda quieta hasta que llega la línea de la siguiente parada. | **[S]** «que no cambie hasta que el niño dice…» |
| Suavizado | La cámara se acerca a su objetivo con constante de 1,5 s: llega suave, nunca salta. Sin avanzar líneas no se mueve. | **[C]** |
| Objetos pintados (`NarrativeProp`) | Cuelgan de la ilustración y acompañan paneo y zoom sin cálculo propio. | **[S]** «que lo que se mencione concuerde con lo que se muestra» |

Las tres secuencias del Nivel 1 (`N1_Apertura`, `N1_AparicionGuia`, `N1_Hallazgo`) **no tienen
ilustración ni cámara**: no hay entorno del Nivel 1 todavía.

---

## 2. Bosque jugable (`Level2_Forest`, fase 1)

| Momento | Movimiento | Valores | Quién |
|---|---|---|---|
| Toda la recolección | **Plano general fijo**: el entorno cubre la pantalla, sin paneo ni zoom. | zoom 1 | **[S]** «en la primera mecánica el plano sí es general» |
| Al acopiar el quinto tronco | **Zoom hacia la caja** mientras los troncos vuelan del acopio a la fila. El pivote del zoom es la caja: ella no se mueve, todo lo demás se acerca. Las tablillas (guía, contador, ayuda, «Empujar») quedan fuera del zoom. | `CompletionZoom` 1.6 · `TransitionSeconds` 1.2 s | **[S]** «haz un zoom a la caja … y un zoom en la cámara hacia esa sección» · valores **[C]** |
| Rodado y caída | La cámara **no se mueve**: la caja rueda y cae dentro de la vista acercada. | — | **[C]** (nadie pidió mover la cámara aquí) |

---

## 3. Escenas narrativas del Nivel 2

### `N2_PuenteI` — puente desde el Nivel 1 (12 líneas)

| Parte | Movimiento | Valores | Quién |
|---|---|---|---|
| Línea 0 → última | Paneo lento desde la esquina superior izquierda del claro (los árboles, «la familia sale de la cueva») hacia el centro del suelo, abriendo el plano. | inicio foco (0.15, 0.65) ×1.5 → final (0.60, 0.45) ×1.1 | **[C]** — solo se ordenó «que la cámara se vaya moviendo por el entorno»; el recorrido concreto es propuesta |

### `N2_Escena21_Bosque` — escena 2.1, entrada al bosque (7 líneas)

| Parte | Movimiento | Valores | Quién |
|---|---|---|---|
| Línea 0 («Bosque. Objetos dispersos por el suelo…») | **Abre con la cámara sobre el suelo**, con los catorce objetos del bosque y la caja pintados donde el texto los describe. | inicio foco (0.50, 0.33) ×1.05 | **[S]** «en el primer texto que se vean los objetos repartidos por el suelo» · valores **[C]** |
| Línea 0 → última | Se acerca despacio hacia la izquierda, donde está la caja («a un lado, la caja»). | final foco (0.35, 0.30) ×1.25 | **[C]** |

### `N2_Escena22_ElPatron` — escena 2.2, el patrón (7 líneas) — **con paradas**

| Parte | Movimiento | Valores | Quién |
|---|---|---|---|
| Línea 0 («La caja rueda sobre los troncos…») | **La misma vista con la que terminó el bosque**: el zoom sobre la caja, los cinco troncos donde estaban y la caja rodando sola (sin botón). **La cámara no se mueve.** | inicio foco (0.34, 0.46) ×1.6 — calculado de la geometría del bosque a 1920×1080 | **[S]** «la vista, la distancia de los troncos y el lugar donde están puestos… que no cambie hasta que el niño dice ¡este tronco rueda!» · «la caja vuelve a rodar sin botón» |
| Línea 1 (NIÑO: «¡Este tronco rueda!») | **Zoom y giro hacia el niño**, abajo a la izquierda, donde levanta el tronco que rueda. | parada foco (0.25, 0.32) ×2.0 | **[S]** «ahí sí cambia la cámara enfocando a lo que sería el niño» · valores **[C]** |
| Línea 2 (PAPÁ: «Pero esa piedra no…») | **Paneo a la derecha** hasta papá, que levanta la piedra y no rueda. | parada foco (0.64, 0.32) ×2.0 | **[S]** «luego paneo a la derecha donde estaría el papá» · valores **[C]** |
| Línea 3 en adelante (MAMÁ «¿Qué diferencia hay?», NIÑA, ALGORITM ×2) | **Paneo más a la derecha y abre un poco el plano**: la familia hablando. Se queda ahí hasta el final. | parada foco (0.84, 0.40) ×1.5 | **[S]** «después de que el papá intente rodar la piedra, que se enfoque al lado derecho donde esté toda la familia» · valores **[C]** |

### `N2_Escena23_Construccion` — escena 2.3, el taller (7 líneas)

| Parte | Movimiento | Valores | Quién |
|---|---|---|---|
| Línea 0 → última | Empieza cerrado sobre la derecha y abre hacia el centro. **Usa el entorno del bosque** como provisional: el taller no tiene arte todavía. | inicio (0.80, 0.50) ×1.5 → final (0.50, 0.45) ×1.2 | **[C]** |

### `N2_Escena24_Regreso` — escena 2.4, el regreso (8 líneas)

| Parte | Movimiento | Valores | Quién |
|---|---|---|---|
| Línea 0 → última | Paneo de derecha a izquierda abriendo hasta el plano general, como quien vuelve por el claro. Entorno del bosque provisional. | inicio (0.75, 0.40) ×1.2 → final (0.20, 0.50) ×1.0 | **[C]** |

### `N2_Escena25_Cierre` — escena 2.5, cierre reflexivo (6 líneas)

| Parte | Movimiento | Valores | Quién |
|---|---|---|---|
| Línea 0 → última | Plano centrado que se aleja despacio hasta el general. Entorno del bosque provisional (la escena ocurre en el refugio). | inicio (0.50, 0.40) ×1.3 → final (0.50, 0.50) ×1.0 | **[C]** |

---

## 4. Objetos que se mueven dentro del encuadre (no son cámara, pero la acompañan)

| Escena · línea | Objeto | Movimiento | Quién |
|---|---|---|---|
| Bosque · «Empujar» | Caja | Rueda girando sobre sí misma (1,5 vueltas) mientras los troncos giran; pasado el último cae al suelo por la derecha, ladeada. Es `RollMotion`, el **mismo** rodado de la narrativa. | **[S]** «que sea la misma animación que la de la narrativa» · «que la caja caiga al piso por el lado derecho» |
| 2.2 · línea 0 | Caja + 5 troncos | La caja rueda y cae igual que en el bosque (misma duración: `RollSeconds` + `FallSeconds`); los troncos giran en el sitio. | **[S]** |
| 2.2 · línea 1 | Un tronco (el del niño) | Sube, cae y rueda. No hay sprite del niño. | **[S]** |
| 2.2 · línea 2 | Una piedra (la de papá) | Sube, cae y se queda con un bamboleo: lo anguloso no rueda. | **[S]** |

---

## 5. Pendiente de revisar a ojo

Ninguna de las cámaras se ha visto en Play con Santiago delante. Lo marcado **[C]** es
propuesta; lo marcado **[S]** cumple la orden en su mecanismo, pero los números concretos
(focos, zooms, duraciones) también son propuesta. Todo se ajusta en los assets.
