# Dirección de música y sonido — Videojuego educativo 2D

Documento hermano de `Direccion_de_Arte.md` y subordinado a `SPEC.md`, igual que aquel.
**No introduce mecánicas ni requisitos:** enumera el audio que el guion ya exige, pieza por
pieza, y fija las reglas que lo gobiernan.

Precedencia: `SPEC.md` → `Direccion_de_Arte.md` → **este documento** → `Interfaces.md` en lo
que toque a pantallas. Si algo aquí contradice a `SPEC.md`, gana `SPEC.md`.

Fuente del inventario: `docs/md/Guion_Completo_Videojuego (1).md` §3.1 a §9, leído completo,
más `RF-01..RF-47` y `RNF-01..RNF-23` vigentes.

**El guía se llama Algoritm** (`PG-02` cerrado el 02/09/2026, INC-44). En este documento
*chispa* en minúscula es siempre el destello del Nivel 1, nunca el personaje.

---

## Índice

1. Declaración de intención
2. Pilares de dirección de sonido
3. Arquitectura de mezcla
4. Nomenclatura, formato e importación
5. El silencio como material
6. Firma sonora de Algoritm
7. Inventario · Música (16)
8. Inventario · Ambientes (8)
9. Inventario · Interfaz y navegación (14)
10. Inventario · Algoritm y diálogo (10)
11. Inventario · Nivel 1 — La Oscuridad (17)
12. Inventario · Nivel 2 — La Rueda (22)
13. Inventario · Nivel 3 — El Río (17)
14. Accesibilidad y protección del oyente
15. Presupuesto de RNF-06
16. Checklist de aprobación de piezas
17. Nota legal
18. Decisiones pendientes

**Total: 104 piezas.**

---

## 1. Declaración de intención

Este juego suena a **materia golpeada por manos humanas**, no a orquesta. Una familia
prehistórica descubre el fuego, la rueda y la balsa; lo que se oye es piedra contra piedra,
madera contra madera, agua contra madera, y voces que tararean. La música existe para
sostener el estado de ánimo de la escena, no para anunciar quién ganó.

Tres cosas que este juego **no** suena, y que hay que decir porque el género empuja hacia
ellas:

- **No suena a arcade.** Sin fanfarria de logro, sin pitido de error, sin cuenta regresiva.
  Todo eso lo prohíben CP-02 y CP-03 antes de que sea una cuestión de gusto.
- **No suena a documental.** No hay gravedad de museo ni voz en off solemne. El público
  tiene entre nueve y once años y la sesión dura entre veinte y cuarenta minutos.
- **No suena a susto.** La escena de apertura es de noche, con animales y algo que se acerca
  entre la maleza (guion §3.1). Es tensión de cuento, resuelta en veinte segundos: nunca
  un golpe fuerte inesperado.

El eje de toda la dirección es el que ya recorre el arte: **el Nivel 1 es piedra y fuego, el
2 es madera, el 3 es agua**. Es el mismo eje de las tres formas de Algoritm (INC-45), y por
eso la firma del guía cambia de timbre con él (§6).

---

## 2. Pilares de dirección de sonido

De estos cinco se deduce cada decisión del inventario. Si una pieza futura contradice uno,
la pieza está mal, no el pilar.

### 2.1 Ningún sonido de fallo

`Direccion_de_Arte.md` §12.3 ya lo decidió al hablar del color —«el error no se marca en
rojo, **no produce sonido de fallo**»— pero no tenía documento donde vivir. Aquí queda como
ley del audio.

Un intento sin éxito suena **descriptivo**: la chispa que se apaga en el aire, la piedra que
se traba, la balsa que gira. Nunca punitivo: sin pitido descendente, sin acorde menor de
derrota, sin golpe seco de rechazo, sin *buzzer*.

La razón es pedagógica, no técnica: CP-02 sostiene el ensayo y error en entorno seguro.
**El código que consuma estos sonidos lleva el comentario escrito** — sin la nota, una futura
«mejora de *feedback*» reintroduce el pitido en una tarde.

Corolario que se verifica por inspección: ninguna pieza de este juego se llama `derrota`,
`gameover`, `error`, `fallo` ni `wrong`.

### 2.2 Materia antes que instrumento

La paleta de fuentes es piedra, madera, cuero, fibra vegetal, fuego, agua y voz humana. Los
instrumentos afinados —flauta de hueso, percusión de piel, voces tarareando— entran solo en
la música (§7); los efectos son objetos, no notas.

### 2.3 Un evento, un sonido

RNF-03 limita el juego a una tarea activa en pantalla. El audio la acompaña: en ningún
momento suenan dos efectos que compitan por la atención. Si dos eventos coinciden, gana el
que el jugador causó con su clic y el otro se retrasa o se omite.

### 2.4 El audio nunca es el único canal

Espejo exacto de RNF-19, que exige un segundo indicador junto al color. Aquí: **si una pieza
comunica un estado, ese estado ya está dicho por forma, texto o posición**. Un equipo del
colegio con los parlantes dañados —o el aula donde la profesora los baja— debe poder
terminar el juego completo sin perder información.

Consecuencia práctica: el audio **refuerza**, nunca informa en exclusiva. Ninguna fila del
inventario es la única señal de nada.

### 2.5 Sin sonido de «encima de»

La entrada se limita a clic y clic sostenido (CT-06, RNF-02). No existe estado de *hover*
que sonorizar, y sonorizarlo inventaría una interacción que el esquema de controles no
tiene. Los botones suenan **al accionarse**, nunca al pasar por encima.

---

## 3. Arquitectura de mezcla

Cuatro buses bajo el maestro, gestionados por `AudioManager` —uno de los tres únicos
singletons con `DontDestroyOnLoad` (`SPEC.md` §Arquitectura)—.

```
Master
├── Musica      → §7
├── Ambiente    → §8
├── SFX         → §9, §11, §12, §13
└── Voz         → §10 (el blip de diálogo y la presencia de Algoritm)
```

### 3.1 Niveles de referencia

| Bus | Nivel objetivo | Pico verdadero | Nota |
|---|---|---|---|
| Musica | −20 LUFS integrado | ≤ −3 dBFS | Nunca tapa el diálogo |
| Ambiente | −26 LUFS integrado | ≤ −6 dBFS | Fondo, se olvida a los diez segundos |
| SFX | — | ≤ −3 dBFS | Cada efecto normalizado a su familia, no al máximo |
| Voz | — | ≤ −6 dBFS | El blip es discreto: suena cien veces por escena |
| Master | −18 LUFS integrado | ≤ −1 dBFS | Sin limitador agresivo; el rango dinámico se cuida en origen |

### 3.2 Atenuación cuando habla Algoritm

Mientras corre una secuencia de diálogo, `Musica` y `Ambiente` bajan **4 dB** con rampa de
300 ms y vuelven con rampa de 500 ms al cerrarse el último globo. Es la única automatización
de mezcla del juego: cuatro decibelios, no ocho — el diálogo se lee, no se escucha, y el
fondo no debe desaparecer.

### 3.3 Transiciones entre escenas

Todo cambio de escena cruza los buses `Musica` y `Ambiente` con fundido de **800 ms de
salida y 1200 ms de entrada**, solapados. `SceneLoader` mide RNF-04 (< 10 s) y el audio no
puede alargar esa medición: los fundidos corren sobre la carga, no después.

Excepción: los tres silencios de §5, donde el corte es **seco y deliberado**.

---

## 4. Nomenclatura, formato e importación

### 4.1 Nombres de archivo

Misma gramática que `Direccion_de_Arte.md` §15.4, con categorías propias:

```
[categoria]_[nivel]_[sujeto]_[variante].wav

mus_n1_cueva_loop.wav
mus_prologo.wav
amb_n2_bosque_dia.wav
sfx_n1_chispa_dentro_3.wav
sfx_ui_boton_primario.wav
sfx_algoritm_presencia_n2.wav
```

Prefijos de categoría: `mus_` (música), `amb_` (ambiente), `sfx_` (efecto).
Niveles: `n1` (La Oscuridad), `n2` (La Rueda), `n3` (El Río); las piezas transversales no
llevan nivel. Sujetos transversales: `ui_`, `algoritm_`, `voz_`, `fin_`.

**Sin tildes, sin espacios, sin mayúsculas.** Igual que en el arte.

### 4.2 Formato de origen

`.wav`, **48 kHz, 16 bit**, mono para todo lo que no sea música. La música es estéreo; los
ambientes son mono y se abren en Unity si hace falta. El juego es 2D sin espacialización:
un efecto en estéreo solo pesa el doble.

Todo archivo entra **sin silencio de cabeza** (el ataque en la muestra cero) y con la cola
completa; recortar la cola produce el clic que delata un asset mal preparado.

Los bucles se entregan con punto de bucle limpio: se comprueba reproduciéndolos tres veces
seguidas y escuchando la costura.

### 4.3 Ajustes de importación en Unity

| Familia | Load Type | Compression | Force To Mono | Preload |
|---|---|---|---|---|
| Música (§7) | Streaming | Vorbis, calidad 70 | no | no |
| Ambientes (§8) | Streaming | Vorbis, calidad 60 | sí | no |
| SFX > 2 s | Compressed In Memory | Vorbis, calidad 70 | sí | no |
| SFX ≤ 2 s | Decompress On Load | PCM | sí | sí |

`Preserve Sample Rate` en todos. Nada de `Resources/`: referencias directas y
ScriptableObjects, como fija `SPEC.md`.

### 4.4 Dónde viven

`Assets/Game/Audio/`, según la estructura ya radicada en `SPEC.md`. El código que los
consume va en `Game.Audio`, que depende de `Game.Core` y de nada más.

---

## 5. El silencio como material

El guion escribe tres silencios de forma explícita. No son ausencia de trabajo: son piezas
con duración y disparador, y **protegerlos es tan obligatorio como producir un efecto**.

| # | Dónde | Qué se calla | Duración | Por qué |
|---|---|---|---|---|
| S1 | Guion §3.1, tras entrar en la cueva | Todo salvo el último eco de los pasos | **2 s exactos** | El guion lo fija: «la pantalla queda completamente negra durante dos segundos». Ese negro sin sonido es la definición del problema del Nivel 1 |
| S2 | Guion §4.1, cuando Algoritm se apaga | Música y presencia del guía; queda solo `amb_cueva_oscura` | 1.5 s | «La oscuridad vuelve de golpe». Si sigue sonando música, no vuelve de golpe |
| S3 | Guion §9, antes de la última frase de Algoritm | Ambiente al mínimo, música suspendida | 1 s | «Y eso ya lo llevan puesto» es la frase que cierra el juego. Se oye sola |

En los tres, el corte es **seco**: no se aplica el fundido de §3.3.

---

## 6. Firma sonora de Algoritm

Paralelo exacto de `Direccion_de_Arte.md` §7.6, que le da un **núcleo de identidad
invariable** y **tres formas**. Aquí: un núcleo sonoro invariable y tres timbres.

### 6.1 Núcleo invariable — en los tres niveles

| Rasgo | Especificación |
|---|---|
| Registro | Agudo, entre 800 Hz y 3 kHz. Nunca grave: es del tamaño de una palma |
| Articulación | Notas cortas y separadas, nunca ligadas. Habla en destellos, como su estela de puntos sueltos |
| Cuenta de cinco | Toda frase suya se cuenta hasta cinco: cinco notas, cinco pulsos, cinco destellos. Es el eco sonoro de la regla visual de las cinco puntas |
| Afinación | Escala pentatónica mayor, sin semitonos. Ningún intervalo suyo suena triste — no existe la tristeza en este personaje, igual que no existe en su set de expresiones (§7.3 del arte) |
| Cola | Siempre con reverberación corta que se apaga sola: emite luz propia y el sonido también se sostiene solo |

Si uno de estos cambia, deja de leerse como el mismo personaje. Es la misma regla que el
arte aplica a los ojos y a la ausencia de extremidades.

### 6.2 Los tres timbres

| Nivel | Forma (arte) | Timbre | Material |
|---|---|---|---|
| 1 · La Oscuridad | Estrella | Campanilla de cristal con cola larga | Fuego: chisporroteo agudo bajo la nota |
| 2 · La Rueda | Rueda | Marimba pequeña, ataque de madera | Madera: el golpe seco de la baqueta se oye |
| 3 · El Río | Gota | Gota en cuenco de agua | Agua: la nota nace mojada y se cierra |

**Cuándo muta.** Solo en los dos barridos `TR-05` y `TR-09`, exactamente como el arte
—entra con el timbre del nivel que termina y sale con el del que empieza—. **Nunca a la
vista, ni al oído, dentro de una escena jugable.**

---

## 7. Inventario · Música (16)

Bus `Musica`. Todas estéreo, streaming. Los bucles duran 45–60 s con costura limpia
(ver `PS-03`).

| id | Descripción sonora | Disparador | Dur. | Loop | Traza |
|---|---|---|---|---|---|
| `mus_menu_loop` | Flauta de hueso sobre bordón grave y percusión de piel muy suave. Cálido, curioso, sin prisa | Entrada a `MainMenu`, sigue en selección de perfil y `LevelSelect` | 50 s | sí | RF-01, RF-02, RF-03 |
| `mus_prologo` | Tensión de cuento: percusión de piel en pulso de caminata, voces graves tarareando lejos, se acelera cuando algo se acerca y **se corta en seco** al entrar en la cueva | Escena de apertura | 40 s | no | RF-05, guion §3.1 |
| `mus_n1_cueva_loop` | Casi nada: un bordón grave sostenido y golpes de piedra espaciados, sin melodía. Crece un escalón cada vez que sube la iluminación | Inicio del Nivel 1 (estado E1) | 55 s | sí | RF-14, RF-21, guion §4.3 |
| `mus_n1_nacimiento_fuego` | La melodía que el bucle nunca terminó, ahora completa: flauta cálida, percusión abierta, voces que entran juntas. Triunfo **familiar**, no épico | Ejecutar «Soplar» en condiciones válidas (E7) | 25 s | no | RF-20, guion §4.4 |
| `mus_puente_1` | Amanecer: flauta sola, luminosa, que se apaga cuando aparece el problema del transporte | Escena puente I | 35 s | no | RF-05, guion §5 |
| `mus_n2_bosque_loop` | Ligero y curioso. Marimba de madera en figuras cortas, como quien mira objetos uno por uno | Inicio del Nivel 2 fase 1 | 50 s | sí | RF-22, RF-23, guion §6.1 |
| `mus_n2_taller_loop` | Constructivo: pulso regular de madera, ritmo de trabajo. Más firme que el del bosque, misma paleta | Inicio del Nivel 2 fase 2 | 50 s | sí | RF-27, RF-29, guion §6.2 |
| `mus_n2_laberinto_loop` | Pulso de secuencia: patrón corto que se repite y se desplaza, exactamente lo que hace un algoritmo. Sin melodía que distraiga de la composición de bloques | Inicio del Nivel 2 fase 3 | 45 s | sí | RF-30, RF-31, guion §6.3 |
| `mus_n2_cierre` | La marimba se ensancha y se le suman voces. Resolución tibia, alrededor del fuego | Escena 2.5 | 25 s | no | RF-12, guion §6.4 |
| `mus_puente_2` | Se abre hacia el horizonte —cuerda frotada larga— y se estrecha de golpe al aparecer el río | Escena puente II | 35 s | no | RF-05, guion §7 |
| `mus_n3_rio_loop` | Agua en movimiento continuo bajo una flauta pausada. Es el nivel donde se piensa antes de actuar | Inicio del Nivel 3, fase de recolección | 60 s | sí | RF-35, RF-36, guion §8.2 |
| `mus_n3_ensamblaje_loop` | Más contenido y más cerca: percusión de agua y madera, sin flauta. La atención está en el panel | Apertura del panel de ensamblaje | 50 s | sí | RF-40, guion §8.3 |
| `mus_n3_reintento` | Cinco notas que caen y **vuelven a subir**. Es el sonido de volver a intentar, no el de haber perdido: sin acorde menor, sin descenso final | Escena 3.2, tras la primera prueba fallida | 12 s | no | RF-42, CP-02, guion §8.4.1 |
| `mus_n3_cruce` | La balsa flota: todo se abre. Voces, flauta y agua juntas, la pieza más amplia del juego | Prueba de la balsa exitosa | 30 s | no | RF-44, guion §8.5 |
| `mus_fin` | Recoge los tres motivos —flauta del 1, marimba del 2, agua del 3— y los deja sonar juntos. Se apaga con Algoritm | Escena final | 45 s | no | RF-12, guion §9 |
| `mus_creditos_loop` | El motivo del menú, más lento y sin percusión. Cierra el círculo | Entrada a `Credits` | 45 s | sí | RF-08 |

---

## 8. Inventario · Ambientes (8)

Bus `Ambiente`. Mono, streaming, sin evento reconocible que se repita —un ambiente que se
nota es un ambiente mal hecho—.

| id | Descripción sonora | Disparador | Dur. | Loop | Traza |
|---|---|---|---|---|---|
| `amb_noche_intemperie` | Viento sobre matorral, insectos, algún animal lejano. **Ningún grito reconocible**: la amenaza del guion es de silueta, no de rugido | Escena de apertura, antes de entrar en la cueva | 30 s | sí | guion §3.1 |
| `amb_n1_cueva_oscura` | Goteo espaciado, eco largo, un hilo de aire. Es el sonido del problema del nivel | Interior de la cueva, estado inicial | 40 s | sí | RF-14, guion §4 |
| `amb_n1_cueva_fuego` | El anterior más el crepitar establecido de la hoguera. Entra por fundido de 3 s al nacer el fuego y ya no se va | Tras `sfx_n1_fuego_nace` | 40 s | sí | RF-20, RF-21 |
| `amb_n2_bosque_dia` | Pájaros dispersos, hojas, viento medio. Luminoso y abierto | Nivel 2 fase 1 | 40 s | sí | RF-22, guion §6.1 |
| `amb_n2_taller_claro` | Bosque más apagado y más cerca: el trabajo ocurre en un claro, no en campo abierto | Nivel 2 fase 2 | 40 s | sí | RF-27, guion §6.2 |
| `amb_n2_refugio_fogata` | Fuego cercano, voces de familia muy bajas, sin palabras distinguibles | Escena 2.5 y escena puente II | 30 s | sí | guion §6.4, §7 |
| `amb_n3_rio_orilla` | Corriente ancha y constante, agua contra piedra, aves de ribera. Es el obstáculo, y suena todo el nivel | Nivel 3 completo | 45 s | sí | RF-35, guion §8 |
| `amb_viento_horizonte` | Viento alto y limpio, sin vegetación. Distancia | Escena puente II al mostrar las fogatas; escena final | 30 s | sí | guion §7, §9 |

---

## 9. Inventario · Interfaz y navegación (14)

Bus `SFX`. Todos ≤ 2 s, PCM, precargados: la respuesta a un clic no espera a descomprimir
(RF-11 exige responder en menos de un segundo).

Material aparente: **piedra y madera**, coherente con la interfaz diegética de
`Interfaces.md` §3.8 —los paneles son tablillas y los botones son piedras—.

| id | Descripción sonora | Disparador | Dur. | Loop | Traza |
|---|---|---|---|---|---|
| `sfx_ui_boton_primario` | Piedra redondeada que se asienta: golpe corto, cola breve, cuerpo grave | Clic en botón primario | 0.25 s | no | RF-01, RNF-02 |
| `sfx_ui_boton_secundario` | El mismo gesto, más ligero y más agudo | Clic en botón secundario | 0.20 s | no | RF-01, RF-08 |
| `sfx_ui_boton_atenuado` | **Madera sorda, no error.** Toque apagado, sin tono ni caída. Dice «todavía no», no «te equivocaste» | Clic sobre un botón deshabilitado (p. ej. «Soplar» antes de la convergencia) | 0.15 s | no | RF-19, CP-02, §2.1 |
| `sfx_ui_dialogo_avanzar` | Roce corto de piedra sobre piedra, muy suave: suena una vez por globo y no puede cansar | Clic para avanzar diálogo | 0.12 s | no | RF-05, RF-06 |
| `sfx_ui_dialogo_omitir` | Barrido corto ascendente de madera | Clic en «Omitir» (solo en escenas ya vistas) | 0.30 s | no | RF-06 |
| `sfx_ui_pausa_abrir` | Tablilla que se apoya. El ambiente baja 6 dB mientras la pausa está abierta | Apertura del menú de pausa | 0.35 s | no | RF-07 |
| `sfx_ui_pausa_cerrar` | La tablilla que se retira; el ambiente vuelve | Cierre del menú de pausa | 0.30 s | no | RF-07 |
| `sfx_ui_perfil_crear` | Tres notas cortas ascendentes de flauta. Bienvenida, no fanfarria | Confirmación de creación de perfil | 0.60 s | no | RF-02, HU-01 |
| `sfx_ui_perfil_borrar` | Barrido descendente de arena. **Neutro**: borrar es un derecho del estudiante (RNF-11), no un castigo | Confirmación de eliminación de perfil | 0.50 s | no | RF-47, RNF-11 |
| `sfx_ui_nivel_desbloqueado` | Piedra que se corre y deja pasar. Refuerza el candado que se abre, **no lo sustituye** (§2.4) | Al mostrarse un nivel recién habilitado en `LevelSelect` | 0.70 s | no | RF-03, RNF-19 |
| `sfx_ui_tarea_marcada` | Nudo de cuerda que se aprieta. Coherente con la lista de tareas, que es cuerda con nudos | Marcado de una tarea de la lista del Nivel 3 | 0.30 s | no | RF-36 |
| `sfx_ui_transicion_fundido` | Soplo de aire corto que cubre la costura entre escenas | Fundido de transición (`TR-05`, `TR-09` y demás) | 0.80 s | no | RNF-04 |
| `sfx_ui_arranque` | Una sola nota de flauta con cola larga sobre negro | Escena `Boot`, antes del menú | 1.5 s | no | RNF-04 |
| `sfx_ui_salida` | La misma nota, invertida y apagándose | Salida controlada desde el menú principal | 1.0 s | no | RF-09 |

---

## 10. Inventario · Algoritm y diálogo (10)

Bus `Voz`. Timbre según §6.2.

| id | Descripción sonora | Disparador | Dur. | Loop | Traza |
|---|---|---|---|---|---|
| `sfx_algoritm_aparicion` | Tres destellos de campanilla, espaciados y crecientes, tal como el guion los escribe: «un destello. Luego otro. Y otro más». Termina en la nota sostenida de su presencia | Escena 1.1, aparición del guía | 2.2 s | no | RF-05, RF-10, guion §4.1 |
| `sfx_algoritm_apagado` | Dos parpadeos y corte seco. Después, silencio S2 | Escena 1.1, cuando el guía se apaga | 0.8 s | no | guion §4.1, §5 |
| `sfx_algoritm_presencia_n1` | Campanilla de cristal, cinco notas pentatónicas, cola larga | Entrada de Algoritm en cualquier escena del Nivel 1 | 1.2 s | no | RF-10, §6.2 |
| `sfx_algoritm_presencia_n2` | Marimba pequeña, cinco notas, ataque de madera audible | Entrada de Algoritm en el Nivel 2 | 1.2 s | no | RF-10, §6.2 |
| `sfx_algoritm_presencia_n3` | Gota en cuenco, cinco notas, nacimiento mojado | Entrada de Algoritm en el Nivel 3 | 1.2 s | no | RF-10, §6.2 |
| `sfx_algoritm_pista` | Pulso doble suave, del timbre del nivel. Acompaña el pulso de escala del icono de pista y **suena igual pidiendo ayuda que recibiéndola tras tres fallos**: si sonara distinto, el segundo caso se leería como reproche | Ayuda a demanda, y pista automática tras tres intentos fallidos | 0.9 s | no | RF-13, CP-06, CP-02 |
| `sfx_algoritm_cierre` | Frase completa de cinco notas ascendentes, la única vez que su motivo se resuelve hacia arriba | Cierre reflexivo al completar un nivel | 2.0 s | no | RF-12, CP-07 |
| `sfx_algoritm_barrido_tr05` | Transformación fuego → rueda: la campanilla se ensancha y aterriza en madera | Transición `TR-05`, del Nivel 1 al 2 | 1.8 s | no | INC-45, `Interfaces.md` §1 |
| `sfx_algoritm_barrido_tr09` | Transformación rueda → gota: la madera se moja y se cierra en agua | Transición `TR-09`, del Nivel 2 al 3 | 1.8 s | no | INC-45, `Interfaces.md` §1 |
| `sfx_voz_blip` | **Blip único global para todo el diálogo.** Nota corta y sorda, sin altura reconocible, que suena mientras se escribe el texto —una vez cada dos o tres caracteres, no en cada uno— y calla en espacios y signos de puntuación. El mismo para los cinco hablantes | Escritura de texto en cualquier globo de diálogo | 0.06 s | no | RF-05, RNF-18 |

> **Por qué un solo blip y no locución.** RNF-18 exige que los diálogos vivan en archivos de
> datos editables sin recompilar: cada corrección de texto invalidaría su pista de voz. Y
> ~95 líneas locutadas comprometen RNF-06 (< 500 MB). Quién habla lo dice el retrato y el
> nombre en el globo, que son dos canales visuales, no uno.

---

## 11. Inventario · Nivel 1 — La Oscuridad (17)

Bus `SFX`. Material: **piedra y fuego**. La progresión sonora del nivel es la progresión de
la chispa: muere en el aire, muere en la piedra, vive un instante, vive más, humea.

| id | Descripción sonora | Disparador | Dur. | Loop | Traza |
|---|---|---|---|---|---|
| `sfx_n1_deslizante_muesca` | Piedra que encaja en una muesca. Tres muescas, mismo sonido | Cambio de posición en el control deslizante | 0.15 s | no | RF-15, guion §4.3.1 |
| `sfx_n1_golpe_a` | Sílex contra pedernal: choque seco, brillante, con cola de polvo | Acción «Golpear», variación 1 | 0.35 s | no | RF-16 |
| `sfx_n1_golpe_b` | La misma acción, ángulo distinto: más grave, menos brillo | Acción «Golpear», variación 2 | 0.35 s | no | RF-16 |
| `sfx_n1_golpe_c` | Tercera variación, con raspado antes del choque | Acción «Golpear», variación 3 | 0.40 s | no | RF-16 |
| `sfx_n1_chispa_aire` | Siseo agudo que se extingue **arriba**, sin llegar a nada | Golpe desde «Lejos» | 0.5 s | no | RF-16, RF-17, guion §4.3.3 |
| `sfx_n1_chispa_piedra` | La chispa cae y se apaga contra superficie fría: un `tic` sordo, sin resonancia | Golpe desde «Cerca» | 0.6 s | no | RF-16, RF-17, guion §4.3.3 |
| `sfx_n1_chispa_dentro_1` | Cae dentro de las hojas: chisporroteo de medio segundo que se apaga solo | Primer golpe efectivo | 0.9 s | no | RF-16, guion §4.3.3 |
| `sfx_n1_chispa_dentro_2` | El mismo chisporroteo, más largo y con más cuerpo | Segundo golpe efectivo | 1.2 s | no | RF-16, guion §4.3.3 |
| `sfx_n1_chispa_dentro_3` | No se apaga: al final aparece el siseo suave del humo subiendo | Golpe efectivo final | 1.6 s | no | RF-16, RF-19, guion §4.3.3 |
| `sfx_n1_log_linea` | Roce muy corto de carboncillo sobre piedra, al escribirse cada mensaje del registro | Escritura de una línea en el log de retroalimentación | 0.20 s | no | RF-11, RF-17 |
| `sfx_n1_soplar_habilitado` | La brasa que ya existe: tres crepitaciones cercanas. Refuerza que el candado del botón se retira, **no lo sustituye** | El contador alcanza `golpesEfectivosMinimos` (E6) | 0.8 s | no | RF-19, RNF-19 |
| `sfx_n1_soplo` | Aire humano largo y cercano, con el temblor de quien contiene la respiración | Acción «Soplar» | 1.8 s | no | RF-20 |
| `sfx_n1_fuego_nace` | La llama prende: chisporroteo que se abre y se convierte en crepitar estable. Crece en 3 s, **sin golpe inicial** (RNF-21 y §14) | Resolución del nivel (E7) | 3.5 s | no | RF-20, guion §4.4 |
| `sfx_n1_luz_escalon` | Nota grave muy suave que sube un escalón. Casi subliminal: acompaña la iluminación, que ya es visible | Cada incremento de iluminación del escenario | 0.6 s | no | RF-21, §2.4 |
| `sfx_n1_pieza_tomar` | Toque corto: todo quedó reunido dentro del círculo y el nivel pasa al encendido | Paso de reunir a encender | 1.1 s | no | RF-14, INC-47 |
| `sfx_n1_hojas_acomodo` | Hojas secas moviéndose. Suena en bucle **mientras se arrastra una hoja** (clic sostenido) y calla al soltarla; también al amontonarlas en la escena 1.2 | Arrastre de una hoja · línea «Las amontona con cuidado en el suelo» | 6.0 s | sí | RF-14, RNF-02, INC-47 |
| `sfx_n1_pasos_eco` | Pasos que resuenan contra la piedra al entrar en la cueva. Es «el último eco» que sobrevive al silencio S1 | Escena de apertura, línea «Los pasos resuenan contra la piedra» | 8.1 s | no | guion §3.1, §5 S1 |

---

## 12. Inventario · Nivel 2 — La Rueda (22)

Bus `SFX`. Material: **madera**. Tres fases, tres registros: el bosque es abierto, el taller
es cercano, el laberinto es seco y rítmico.

### 12.1 Fase 1 — Observación y patrón (6)

| id | Descripción sonora | Disparador | Dur. | Loop | Traza |
|---|---|---|---|---|---|
| `sfx_n2_objeto_valido` | Tronco que rueda un cuarto de vuelta y se detiene en el acopio. **El sonido dice «rueda»**, que es justo el patrón que el jugador debe descubrir | Clic sobre un tronco redondo | 0.7 s | no | RF-23, guion §6.1.2 |
| `sfx_n2_objeto_devuelto` | El objeto se traba y vuelve: raspado corto y mate. Descriptivo, sin caída tonal | Clic sobre un distractor | 0.5 s | no | RF-23, CP-02, §2.1 |
| `sfx_n2_contador` | Marca de madera sobre madera al subir el contador de acopio | Incremento de «Troncos redondos: n de 5» | 0.20 s | no | RF-24 |
| `sfx_n2_caja_tomar` | Cuero y peso: la caja se levanta apenas | Inicio del arrastre de la caja de alimentos | 0.30 s | no | RF-25 |
| `sfx_n2_caja_soltar` | La caja se asienta sobre los troncos alineados | Suelta de la caja | 0.40 s | no | RF-25 |
| `sfx_n2_rodado` | **El sonido del descubrimiento del nivel:** troncos girando bajo la caja, rodadura continua y sin esfuerzo, tres segundos | Acción «Empujar» | 3.0 s | no | RF-26, guion §6.1.3 |

### 12.2 Fase 2 — Construcción de la carretilla (7)

| id | Descripción sonora | Disparador | Dur. | Loop | Traza |
|---|---|---|---|---|---|
| `sfx_n2_tronco_seleccion` | Toque de madera hueca; la pieza queda resaltada | Clic sobre un tronco corto | 0.20 s | no | RF-28 |
| `sfx_n2_mecanizar` | **El sonido central del nivel:** herramienta que perfora madera, tres golpes y un giro final que abre el agujero | Acción «Mecanizar» sobre un tronco seleccionado | 2.0 s | no | RF-28, guion §6.2.2 |
| `sfx_n2_eje_insertado` | El tronco largo entra por el centro de ambas ruedas: roce largo y encaje firme | Paso 4 del ensamblaje | 1.0 s | no | RF-29 |
| `sfx_n2_tabla_colocada` | Tabla que se apoya sobre el eje, con un ligero balanceo | Paso 5 del ensamblaje | 0.7 s | no | RF-29 |
| `sfx_n2_caja_colocada` | Peso que se asienta y se estabiliza | Paso 6 del ensamblaje | 0.8 s | no | RF-29 |
| `sfx_n2_carretilla_lista` | Cuatro golpes de madera y un rodar corto: la herramienta existe | Carretilla completa, animación de terminado | 1.5 s | no | RF-29, guion §6.2.2 |
| `sfx_n2_fuera_secuencia` | La pieza no encuentra dónde apoyarse y vuelve: roce sin encaje. **Suena a «falta algo antes», no a error** — que es literalmente lo que dice el mensaje | Intento de ensamblaje fuera de orden | 0.6 s | no | RF-29, CP-02, §2.1 |

### 12.3 Fase 3 — Regreso al refugio (9)

| id | Descripción sonora | Disparador | Dur. | Loop | Traza |
|---|---|---|---|---|---|
| `sfx_n2_bloque_tomar` | Bloque de madera que se despega del área de bloques | Inicio del arrastre de un bloque | 0.20 s | no | RF-31 |
| `sfx_n2_bloque_engancha` | Encaje seco y satisfactorio: dos piezas de madera que se enganchan | Suelta de un bloque en la secuencia | 0.30 s | no | RF-31 |
| `sfx_n2_bloque_retirar` | El encaje al revés, más suave. Editar no cuesta nada y debe sonar así | Retirada o reubicación de un bloque | 0.25 s | no | RF-34 |
| `sfx_n2_ejecutar` | Un golpe de madera que abre la lectura de la secuencia | Acción «Ejecutar», clic simple (`PG-04`) | 0.40 s | no | RF-32 |
| `sfx_n2_paso_avanzar` | Rueda de carretilla, una casilla. Con el bloque resaltado, es el latido de la ejecución | Ejecución de un bloque «Avanzar» | 0.5 s | no | RF-32 |
| `sfx_n2_paso_retroceder` | La misma rueda, invertida | Ejecución de un bloque «Retroceder» | 0.5 s | no | RF-32 |
| `sfx_n2_paso_girar` | Roce de rueda pivotando 90°, sin desplazamiento | Ejecución de un bloque «Girar» | 0.6 s | no | RF-32 |
| `sfx_n2_retroceso_bloqueado` | **El sonido de la depuración, el que más cuidado necesita.** La carretilla toca el obstáculo y vuelve a la casilla anterior: contacto blando de madera y rodadura de regreso. Sin pitido, sin golpe metálico, sin caída tonal — el jugador debe querer mirar qué paso falló, no sentirse castigado | Instrucción que conduce a obstáculo o casilla inválida | 0.8 s | no | RF-33, CP-02, §2.1 |
| `sfx_n2_refugio_alcanzado` | La carretilla se detiene y la carga se asienta. Cierre de la fase, no fanfarria | La carretilla alcanza la casilla del refugio | 1.2 s | no | RF-32, guion §6.3.2 |

---

## 13. Inventario · Nivel 3 — El Río (17)

Bus `SFX`. Material: **agua, fibra y madera mojada**. Las tres últimas piezas cierran el
juego completo.

| id | Descripción sonora | Disparador | Dur. | Loop | Traza |
|---|---|---|---|---|---|
| `sfx_n3_paso` | Paso descalzo sobre tierra húmeda y hierba. Se reproduce con el desplazamiento, espaciado y bajo: suena muchas veces y no puede molestar | Movimiento del personaje | 0.25 s | no | RF-35 |
| `sfx_n3_boton_direccion` | Toque muy corto y neutro al accionar un botón de dirección. Es interfaz, no pisada | Clic en un botón de dirección | 0.10 s | no | RF-35, CT-06, INC-01 |
| `sfx_n3_recoger` | El material entra al inventario: roce de fibra y un asentamiento corto | Acción «Recoger» sobre un objeto próximo | 0.5 s | no | RF-37 |
| `sfx_n3_inventario_completo` | Cuerda del marco que se tensa. Los cuatro objetos son exactamente los necesarios: informa, no advierte | Cuarto objeto incorporado al inventario | 0.6 s | no | RF-38 |
| `sfx_n3_zona_construccion` | Cambio de espacio: el río se acerca un paso y la madera apilada se asienta | Entrada del personaje en la zona de construcción | 0.9 s | no | RF-39 |
| `sfx_n3_material_falta` | Nota corta y neutra que acompaña al mensaje de qué material falta. **No es negación**: la zona no rechaza, informa | Entrada en la zona sin los materiales requeridos | 0.4 s | no | RF-39, CP-02, §2.1 |
| `sfx_n3_pieza_encaja` | Tronco que se acomoda en su espacio, con el agua debajo | Colocación correcta de una pieza en el panel | 0.5 s | no | RF-40 |
| `sfx_n3_pieza_devuelta` | La pieza no asienta y vuelve al inventario: roce blando, sin golpe | Colocación incorrecta dentro de una fase | 0.5 s | no | RF-40, RF-43, §2.1 |
| `sfx_n3_fase_confirmada` | Amarre que se aprieta y queda firme. **Una fase aprobada no se pierde** (RF-41), y el sonido debe transmitir esa firmeza | Confirmación correcta de una fase con «Listo» | 1.0 s | no | RF-41, guion §8.3 |
| `sfx_n3_probar_balsa` | La balsa entra al agua: empuje y chapoteo grave | Acción «Probar balsa» | 1.2 s | no | RF-42 |
| `sfx_n3_hundimiento` | La balsa gira y se hunde por un costado: madera que cruje, agua que entra, y **un final blando y hasta cómico**. La orilla es poco profunda y nadie corre peligro; el sonido lo dice | Prueba de la balsa fallida | 2.5 s | no | RF-42, CP-02, guion §8.4.1 |
| `sfx_n3_balsa_flota` | La balsa se asienta y se equilibra. Estabilidad audible | Prueba de la balsa exitosa | 1.5 s | no | RF-44, guion §8.5 |
| `sfx_n3_vela_viento` | La tela se llena de golpe y queda tensa | La vela recibe el viento, dentro de la animación de cruce | 1.8 s | no | RF-44, guion §8.5 |
| `sfx_n3_cruce` | Agua contra los troncos durante la travesía, constante y avanzando | Animación de cruce del río | 6.0 s | no | RF-44 |
| `sfx_n3_orilla_opuesta` | La balsa toca tierra: madera contra grava y el agua que se queda atrás | Llegada a la orilla opuesta | 1.0 s | no | RF-44, guion §8.5 |
| `sfx_fin_algoritm_giro` | Algoritm gira una última vez: las cinco notas de su timbre del Nivel 3, y la quinta no se apaga del todo | Escena final, último giro del guía | 2.5 s | no | RF-12, guion §9 |
| `sfx_fin_fundido` | La cola de esa quinta nota entrando en el negro. Es la última pieza del juego | Fundido a negro antes de los créditos | 3.0 s | no | guion §9 |

---

## 14. Accesibilidad y protección del oyente

Análogo sonoro de §14 del arte, y de RNF-21 —que prohíbe destellos de alta frecuencia— en su
lectura auditiva.

1. **Sin sustos.** Ningún salto de nivel mayor a **6 dB** entre dos momentos consecutivos.
   Toda entrada fuerte se aborda con rampa de al menos 300 ms. El único corte seco permitido
   es el de los tres silencios de §5, y esos cortan **hacia** el silencio, no desde él.
2. **Rango dinámico acotado.** El juego se usa en aula, con parlantes de computador o
   proyector: lo que se mezcla para auriculares de estudio desaparece ahí. Diferencia máxima
   entre la pieza más suave y la más fuerte: 18 dB.
3. **Sin brillo agresivo.** Filtro de estante por encima de 12 kHz en todo el bus maestro.
   Los chisporroteos y los siseos son los candidatos naturales a fatigar.
4. **Sin frecuencias inaudibles.** Corte bajo a 60 Hz: lo que un parlante de aula no puede
   reproducir solo consume bytes contra RNF-06.
5. **Nada depende del oído.** Ver §2.4. Es la regla que hace el juego completable con el
   sonido apagado, y se verifica jugando una partida completa en silencio.
6. **Sin repetición inmediata.** Ningún efecto suena idéntico dos veces seguidas cuando
   existe una variación aplicable — la misma regla que el guion §4.3.4 impone a los mensajes
   del log. Por eso el golpe del Nivel 1 tiene tres variaciones.

---

## 15. Presupuesto de RNF-06

RNF-06 fija 500 MB para el paquete completo. El arte se lleva la mayor parte; **el audio
tiene un techo de 60 MB**, con esta distribución estimada tras compresión:

| Familia | Piezas | Ajuste | Peso estimado |
|---|---|---|---|
| Música | 16 | Vorbis 70, estéreo, streaming | ~13 MB |
| Ambientes | 8 | Vorbis 60, mono, streaming | ~3 MB |
| SFX ≤ 2 s | 68 | PCM mono 48 kHz/16 bit | ~9 MB |
| SFX > 2 s | 9 | Vorbis 70, mono | ~2 MB |
| **Total** | **101** | | **~27 MB** |

Margen de más del doble sobre el techo. **Si aun así se excede, el orden de recorte es
fijo:**

1. **Duración de los bucles** — de 50 s a 35 s. Ahorra ~30 % de la música sin perder
   ninguna pieza.
2. **Calidad Vorbis** — de 70 a 55 en música y de 60 a 45 en ambientes.
3. **Variaciones** — las tres del golpe del Nivel 1 pasan a dos.
4. **Nunca la cobertura.** No se retira una pieza del inventario para ganar megabytes: una
   escena sin su sonido es una escena rota, y el peso se recupera en cualquiera de los tres
   pasos anteriores.

La medición real se hace sobre la carpeta de `unity build`, que es el entregable portable,
junto con las de RNF-04 y RNF-05.

---

## 16. Checklist de aprobación de piezas

Espejo de §17 del arte. **Toda pieza cumple las siete antes de entrar al proyecto.**

- [ ] El nombre sigue §4.1: prefijo correcto, sin tildes, sin espacios, sin mayúsculas.
- [ ] `.wav` 48 kHz/16 bit, mono salvo música, sin silencio de cabeza y con la cola completa.
- [ ] Si es bucle, se reproduce tres veces seguidas sin costura audible.
- [ ] Nivel dentro del objetivo de su bus (§3.1) y pico verdadero bajo el límite.
- [ ] **No es un sonido de fallo** (§2.1): sin pitido descendente, sin acorde de derrota,
      sin golpe de rechazo. Se comprueba escuchándola aislada y preguntando «¿esto me dice
      que hice algo mal?».
- [ ] El estado que acompaña **ya está comunicado** por forma o texto (§2.4).
- [ ] Tiene fila en el inventario de este documento, con disparador y traza a RF o a sección
      del guion. Una pieza sin fila no entra.

Y una octava que se verifica una sola vez, al cerrar el proyecto: **una partida completa con
el sonido apagado, de principio a fin, sin perder información.**

---

## 17. Nota legal

`SPEC.md` §Nunca lo dice sin ambigüedad: no entra **«un asset gráfico o sonoro sin autoría
propia ni autorización escrita, o sin su reconocimiento en la pantalla de créditos»**
(CT-09, RNF-23).

Consecuencias vigentes para el audio:

1. Cada pieza producida se registra en `CreditsContent.asset`, igual que cualquier asset
   visual. La pantalla de créditos es requisito radicado (RF-08), no cortesía.
2. Si una pieza proviene de una biblioteca externa, la autorización escrita se archiva con
   los anexos del trabajo de grado, junto a la del uso de los personajes y al formato de
   consentimiento informado de RNF-12.
3. La generación con las herramientas del propio proyecto produce **asset propio** y
   satisface el criterio, pero **no exime del registro en créditos**: el reconocimiento es de
   autoría, y la autoría existe igual.

> **Hallazgo documental.** La letra de **RNF-23 dice «recursos gráficos»**, no sonoros;
> quien verifique el requisito contra su redacción literal podría concluir que el audio
> queda fuera. `SPEC.md` ya cerró la laguna extendiéndolo a lo sonoro, y este documento se
> apoya en esa extensión. Conviene registrarlo en `INCONSISTENCIAS.md` y corregir la
> redacción de OE1 a «recursos gráficos y sonoros» — es una corrección de alcance de un
> requerimiento ya radicado, de las de **preguntar primero**.

---

## 18. Decisiones pendientes

Igual que §18 del arte: se dejan escritas, no se toman por cuenta propia.

| ID | Situación | Acción requerida | Estado |
|---|---|---|---|
| `PS-01` | **No existe control de volumen en ninguna pantalla.** RF-01 da una pantalla de inicio sin menú de opciones —`Interfaces.md` §1.1 lo confirma— y RF-07 da una pausa con exactamente tres opciones. Añadir un deslizante sería interfaz que ningún RF pide, y `SPEC.md` clasifica eso como «preguntar primero». Este documento fija por tanto una mezcla correcta por omisión (§3.1) | Decidir si se añade control de volumen —lo que exige tocar RF-01 o RF-07— o si la mezcla fija es la respuesta definitiva. En un aula, el volumen del sistema operativo suele bastar | Abierto |
| `PS-02` | El tema del menú y el de créditos son el mismo motivo, y ese motivo es la identidad sonora del producto. Pero el producto **no tiene título** (`PG-01`, abierto en el guion §12) | Definir el título antes de producir `mus_menu_loop`, o producirlo asumiendo que el motivo no cambiará al nombrarlo | Abierto |
| `PS-03` | Las duraciones de bucle de §7 (45–60 s) son propuestas, no valores medidos. Un bucle de 90 s pesa el triple que uno de 30 s | Ajustar tras la primera medición del paquete de `unity build`, siguiendo el orden de recorte de §15 | Abierto |
| `PS-04` | La fuente de producción no está elegida. Generación propia satisface CT-09/RNF-23 como asset propio; una biblioteca externa exige autorización escrita y línea en créditos | Elegir y dejar escrito **antes** de producir la primera pieza, no después | Abierto |
| `PS-05` | RNF-23 dice «recursos gráficos» y no menciona los sonoros (ver §17) | Registrar el hallazgo en `INCONSISTENCIAS.md` y corregir la redacción de OE1. Modificar un RNF radicado es de las de «preguntar primero» | Abierto |

---

## 19. Historial

- **rev. 3 (23/09/2026)** — Nivel 2, a petición de Santiago, con cuatro de las piezas entregadas en
  PR #81 y una del Nivel 1. El nivel lleva `N2_Sonidos.asset` (`WheelSounds`), que referencian las
  tres escenas jugables. Aplicado: `amb_n2_bosque_dia` suena de fondo en el puente I, en las
  escenas 2.1 a 2.4 y en **las tres fases**, sin costura entre ellas. Eso se aparta de §8, que lo
  pone solo en la fase 1: `amb_n2_taller_claro` no se ha entregado, y la fase 3 no tenía ambiente
  propio. La 2.5 queda sin ambiente hasta que llegue `amb_n2_refugio_fogata`. El apartarse de los
  objetos del bosque al pasar el cursor suena a lo que es, una vez por acercamiento:
  `sfx_n2_troncos` el tronco, `sfx_n2_piedra_cae` la piedra **y la herramienta**, y la planta
  reutiliza `sfx_n1_hojas_acomodo` **en bucle mientras vuela**, porque son 6 s y el vuelo dura
  menos. `sfx_n2_carretilla` suena en bucle mientras la carretilla recorre la secuencia del
  laberinto, también en el intento que choca y vuelve (§2.1). Una sola pieza hace lo que §12.3
  reparte entre `sfx_n2_paso_avanzar`/`_retroceder`/`_girar`/`_retroceso_bloqueado`, que siguen
  sin entregar. Ninguna de las piezas de §12 con su nombre está entregada todavía.
  **Segunda tanda, el mismo día:**
  - Las globales hacen de piezas de §12 que no se han entregado. `sfx_encaje_pieza` suena por cada
    tronco acopiado (en lugar de `sfx_n2_objeto_valido`) y al perforar, con «Mecanizar» o con el
    martillo (en lugar de `sfx_n2_mecanizar`).
  - `sfx_n1_pieza_tomar` suena cuando los cinco troncos quedan reunidos y cuando la carretilla
    está terminada (en lugar de `sfx_n2_carretilla_lista`).
  - `sfx_martillo_madera` suena tres veces seguidas, separadas por 0,3 s, cuando una pieza encaja
    en la carretilla (en lugar de `sfx_n2_eje_insertado`, `_tabla_colocada` y `_caja_colocada`).
  - En la escena 2.2 cada objeto suena **al tocar el suelo** (`NarrativeProp.LandSound`), no al
    empezar su línea. El tronco que suelta el niño suena con `sfx_n2_troncos` y la piedra que
    suelta papá con `sfx_n2_piedra_cae`, las dos a mitad de su movimiento, cuando caen. La caja
    suena con `sfx_n2_piedra_cae` al terminar de caer tras el rodado. No hay pieza propia para
    esa caída, y `sfx_n2_rodado` tampoco se ha entregado.
  - La 2.5 y el arranque del puente II ocurren en el refugio, que es la cueva del Nivel 1 con la
    hoguera. Suenan `amb_n1_cueva_oscura` con `amb_n1_cueva_fuego` encima, igual que en la 1.3,
    en lugar de `amb_n2_refugio_fogata`, que no se ha entregado. El horizonte
    (`N3_PuenteII_Horizonte`) se mira desde la boca de la cueva y conserva los dos ambientes sin
    costura.
- **rev. 2 (21/09/2026)** — Primera incorporación al juego (compromiso `D05-2`/`D07-2`), Nivel 1
  completo con las nueve piezas entregadas. `AudioManager` (`Game.Audio`) con los cuatro buses de
  §3, los fundidos de §3.3 y los cortes secos de §5; el contenido va en assets (CT-05): cada
  `NarrativeSequence` declara su ambiente, cada línea su efecto y su silencio, y el nivel lleva
  `N1_Sonidos.asset`. Aplicado: `amb_noche_intemperie` (apertura) → S1 en «Solo oscuridad» →
  `amb_n1_cueva_oscura` (1.1, 1.2 y el nivel, sin costura) → S2 al apagarse Algoritm →
  `amb_n1_cueva_fuego` entra en 3 s al soplar **como capa sobre la cueva**, que sigue debajo, y las
  dos continúan en la 1.3 (`NarrativeSequence.AmbientLayer`); la cueva entra en la apertura en la
  línea «Avanzan hacia el fondo» (`DialogueLine.Ambient`), no al abrir la 1.1. El golpe solo se oye
  cuando las piedras se tocan (desde la muesca dos) y sube de volumen con lo encimado (`StoneSpacing.
  Contact`); la muesca efectiva es **solo la cinco** (`N1_Config.EffectiveOverlap = 30`). Tres piezas nuevas en §11 que la
  mecánica de reunir (INC-47) y el prólogo exigían. El golpe llegó en **una toma** (`sfx_n1_golpe`)
  en vez de las tres variaciones a/b/c: §14.6 se cubre con una variación de tono ±6 %. Regla de
  importación de §4.3 en `Game.EditorTools/AudioImportRules.cs`, vigilada por
  `AudioImport_RNF06_…`; el corolario de §2.1 lo vigila `AudioAssets_CP02_…`. **Sin música ni
  blip todavía**: ninguna pieza `mus_` ni `sfx_voz_blip` fue entregada; S2 queda cableado y es
  inocuo hasta que exista `mus_n1_cueva_loop`. Este archivo se mueve a `claudeDocs/` porque
  `docs/md/` está en `.gitignore` y lo escrito a mano ahí ya se perdió una vez (20/09/2026).
- **rev. 1 (09/09/2026)** — Documento inicial. Inventario de 101 piezas derivado del guion
  §3.1..§9 y de `RF-01..RF-47`. Fija como ley del audio la decisión que `Direccion_de_Arte.md`
  §12.3 había tomado de paso (ningún sonido de fallo, CP-02). Diálogo resuelto con un blip
  único global, sin locución. Deja abiertas `PS-01`..`PS-05`.
