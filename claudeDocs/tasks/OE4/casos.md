# Catálogo de casos de prueba funcional — OE4

> Es el *instrumento de evaluación funcional* del cronograma. Las reglas (oráculo, veredictos,
> ejecutores, arnés, semillas, defectos) están en `plan.md` y no se repiten aquí. Los textos
> exactos se tomaron de los assets en `HEAD` el 29/09/2026; T04 los contrasta con `oe4-rc1` antes
> de ejecutar.

## Cómo leer este archivo

- **Una sesión = un corte vertical** del juego, ejecutado de corrido. Cada sesión tiene
  precondición, **guion** numerado y la **tabla de casos** que ese guion verifica.
- En el guion: **acción → esperado**, y entre ‹ › los casos que ese paso alimenta. Un caso aprueba
  si se observan **todos** los esperados de los pasos que lo nombran.
- `«texto»` es el texto exacto en pantalla. Una diferencia de palabras es un F, salvo que T04 lo
  haya corregido antes.
- `(x; y)` son coordenadas normalizadas del área cliente, con origen **arriba a la izquierda**.
  Son aproximadas: se confirman con la primera captura de la pantalla.
- `[PD: INC-nn]`: ese paso sigue la decisión de un INC abierto; si el caso pasa, su veredicto es PD.
- `⚠`: riesgo conocido antes de ejecutar (`plan.md` §9). El esperado es el del requerimiento, no
  el que se teme.
- Cifras de indicadores: `I/C/P` = intentos / errores corregidos / pasos utilizados.
- Al terminar cada sesión que cambie perfiles, se copia el JSON final a
  `evidencias/<sesión>_<perfil>.json`: S-DOC lo reutiliza.

## Índice de cobertura (CT-10)

| Req | Prior. | Casos | | Req | Casos |
|---|---|---|---|---|---|
| RF-01 | A | PF-RF01-01, 02 | | RF-35 | PF-RF35-01 |
| RF-02 | A | PF-RF02-01 … 05 | | RF-36 | PF-RF36-01 |
| RF-03 | A | PF-RF03-01, 02, 03 | | RF-37 | PF-RF37-01 |
| RF-04 | A | PF-RF04-01, 02, 03 | | RF-38 | PF-RF38-01 |
| RF-05 | A | PF-RF05-01, 02, 03 | | RF-39 | PF-RF39-01 |
| RF-06 | M | PF-RF06-01 … 04 | | RF-40 | PF-RF40-01, 02 |
| RF-07 | A | PF-RF07-01 … 06 | | RF-41 | PF-RF41-01 |
| RF-08 | A | PF-RF08-01 | | RF-42 | PF-RF42-01 |
| RF-09 | A | PF-RF09-01, 02, 03 | | RF-43 | PF-RF43-01 |
| RF-10 | A | PF-RF10-01, 02, 03 | | RF-44 | PF-RF44-01 |
| RF-11 | A | PF-RF11-01, 02, 03 | | RF-45 | PF-RF45-01 … 05 |
| RF-12 | A | PF-RF12-01, 02, 03 | | RF-46 | PF-RF46-01, 02, 03 |
| RF-13 | A | PF-RF13-01 … 05 | | RF-47 | PF-RF47-01, 02, 03 |
| RF-14 | A | PF-RF14-01 | | RNF-01 | PF-RNF01-01 |
| RF-15 | A | PF-RF15-01 | | RNF-02 | PF-RNF02-01 |
| RF-16 | A | PF-RF16-01 | | RNF-03 | PF-RNF03-01 |
| RF-17 | A | PF-RF17-01 | | RNF-04 | PF-RNF04-01 |
| RF-18 | A | PF-RF18-01 | | RNF-05 | PF-RNF05-01 |
| RF-19 | A | PF-RF19-01, 02 | | RNF-06 | PF-RNF06-01 |
| RF-20 | A | PF-RF20-01, 02 | | RNF-07 | PF-RNF07-01, 02 |
| RF-21 | B | PF-RF21-01, 02 | | RNF-08 | PF-RNF08-01 |
| RF-22 | A | PF-RF22-01 | | RNF-09 | PF-RNF09-01 |
| RF-23 | A | PF-RF23-01 | | RNF-10 | PF-RNF10-01 |
| RF-24 | A | PF-RF24-01 | | RNF-11 | PF-RNF11-01 |
| RF-25 | A | PF-RF25-01 | | RNF-12 | PF-RNF12-01 |
| RF-26 | A | PF-RF26-01 | | RNF-13 | PF-RNF13-01 |
| RF-27 | A | PF-RF27-01 | | RNF-14 | PF-RNF14-01 … 05 |
| RF-28 | A | PF-RF28-01 | | RNF-15 … RNF-23 | PF-RNF15-01 … PF-RNF23-01 |
| RF-29 | A | PF-RF29-01 | | CT-02 | PF-CT02-01 |
| RF-30 | A | PF-RF30-01 | | Sonido (KPI) | PF-SON-01, 02, 03 |
| RF-31 | A | PF-RF31-01 | | | |
| RF-32 | A | PF-RF32-01 | | | |
| RF-33 | A | PF-RF33-01 | | | |
| RF-34 | A | PF-RF34-01, 02 | | | |

**Mapa de sesiones → tarea:** S-INI T07 · S-N1 T08 · S-N2A T09 · S-N2B T10 · S-N2C T11 ·
S-N3 T12 · S-PAU T13 · S-OMI T14 · S-PER T15 · S-DOC T16 · S-INSP T06/T17 · S-RNF T20 ·
S-HUM T19.

**Teclas de control negativo** (se usan en varias sesiones): `Teclas Esc Enter Espacio Arriba
Abajo Izquierda Derecha W A S D Rueda` → no debe cambiar nada en pantalla.

---

## S-INI · Inicio, perfiles, créditos y salida — T07 · EXE

**Pre:** `Preparar-Datos OE4_B`. Juego cerrado.

1. `Start-Juego`; `Medir-Carga` desde el arranque → pantalla de inicio. Anotar los segundos
   (Boot→MainMenu). ‹PF-RNF04-01›
2. Foto → título «Algoritmia», lema «Piensa el orden, enciende el fuego», botones «Jugar»,
   «Créditos», «Progreso del equipo» y «Salir», todos visibles y sin solaparse. Anotar si se ve
   algún rótulo de trabajo (p. ej. «Silueta · familia + Algoritm · placeholder»).
   ‹PF-RF01-01, PF-RF01-02›
3. Teclas de control negativo → nada cambia. ‹PF-RF01-01›
4. «Créditos» → `Medir-Carga` → «Créditos». Se leen: la rejilla papel/persona;
   «Personajes basados en diseños de la Familia Anonaky, usados con autorización escrita.»;
   «Entornos, objetos e interfaz: originales del proyecto.»; «Tipografías Baloo 2, Nunito y
   Fredoka bajo licencia SIL OFL 1.1.»; ningún enlace. Anotar rótulos de trabajo
   («Algoritm saluda · placeholder»). ‹PF-RF08-01, PF-RF01-02, PF-RNF23-01›
5. Rueda del ratón y un arrastre vertical sobre la lista → anotar si desplaza (observación de
   RNF-02: fuera de las escenas jugables). ‹PF-RNF02-01›
6. «Volver» → pantalla de inicio. ‹PF-RF08-01›
7. «Jugar» → panel «¿Quién juega?» sin recargar la escena: columna «Perfiles guardados» con
   «OE4_B» y su papelera; columna «Perfil nuevo» con «Escribe tu nombre», campo «Tu nombre»,
   «Sin avatar, sin edad y sin curso.» y «Solo el nombre: nada más se guarda»; botones
   «Continuar» y «Volver». ‹PF-RF02-03›
8. Campo vacío → «Continuar» → «Escribe un nombre para empezar.»; no avanza; en `Datos/` no
   aparece nada. ‹PF-RF02-01›
9. `Escribir "   "` (tres espacios) → «Continuar» → el mismo mensaje. ‹PF-RF02-01›
10. `Escribir "oe4_b"` → «Continuar» → «Ya hay un perfil con ese nombre. Elige otro.»
    (el duplicado no distingue mayúsculas). ‹PF-RF02-01›
11. `Escribir "a/b"` → «Continuar» → «Ese nombre no se puede usar. Prueba con otro.»
    ‹PF-RF02-01›
12. `Escribir` 30 caracteres → el campo conserva 24. Vaciar el campo. ‹PF-RF02-03›
13. «Volver» → «Jugar» → clic en «OE4_B» → «Elige un nivel» **sin narrativa**:
    «Nivel 1 · La Oscuridad» y «Nivel 2 · La Rueda» habilitados; «Nivel 3 · El Río» con candado y
    «Bloqueado». ‹PF-RF02-02, PF-RF03-01›
14. Clic en «Nivel 3» → nada cambia. Anotar si aparece un mensaje que diga qué nivel completar
    (CU-02 FA-2a) y si «Nivel 1» lleva alguna marca de completado (HU-14 paso 7).
    ‹PF-RF03-01, PF-RF03-03›
15. «Volver» → pantalla de inicio. Anotar `LastWriteTime` y contenido de `Datos/OE4_B.json`.
    «Salir» → anotar si aparece una confirmación que informe del guardado (HU-18 paso 3). El
    proceso termina en ≤ 5 s; `OE4_B.json` tiene un `LastWriteTime` posterior y el mismo
    contenido. ‹PF-RF09-01, PF-RF09-02›
16. `Start-Juego` → «Salir» sin elegir perfil → ningún `.json` cambia su `LastWriteTime`.
    ‹PF-RF09-03›
17. `Revisar-Log S-INI`.

| Caso | Req · trazas | Etq | Pasos | Aprueba si |
|---|---|---|---|---|
| PF-RF01-01 | RF-01 · HU-01, HU-18, CU-01 | NAV | 1–3 | Título y los cuatro botones visibles, alcanzables y sin solapes; ninguna tecla hace nada |
| PF-RF01-02 | RF-01, RF-08 · CN-04 | BOT | 2, 4 | Ninguna pantalla de flujo muestra rótulos de trabajo. ⚠ se sabe de dos |
| PF-RF02-01 | RF-02 · HU-01 FA-01/02, CU-01 4a | DAT | 8–11 y S-N1 paso 1 | Vacío, espacios, duplicado e inválido se rechazan con su mensaje sin crear archivo; el nombre válido crea `Datos/<nombre>.json` con `reachedLevel` 1 y `phases` vacío |
| PF-RF02-02 | RF-02 · HU-01 | DAT | 13 | Elegir un perfil existente abre el menú de niveles con su progreso exacto |
| PF-RF02-03 | RF-02, RNF-09 | DAT | 7, 12 | El formulario pide solo el nombre y lo limita a 24 caracteres |
| PF-RF03-01 | RF-03 · CU-02 | NAV | 13–14 | Se ven los tres niveles; solo los alcanzados responden; el bloqueado lleva candado y rótulo |
| PF-RF03-03 | RF-03 · CU-02 FA-2a, HU-14 paso 7 | NAV | 14 | El nivel bloqueado dice qué completar antes y el completado se distingue. ⚠ probable F |
| PF-RF08-01 | RF-08 · HU-18 FA-02 | NAV | 4, 6 | Reconoce la autoría de personajes y recursos de terceros; sin enlaces; «Volver» regresa |
| PF-RF09-01 | RF-09 · HU-18 | DAT | 15 | «Salir» reescribe el perfil activo y cierra la aplicación |
| PF-RF09-02 | RF-09 · HU-18 paso 3, FA-01 | BOT | 15 | «Salir» pide confirmación informando del guardado. ⚠ probable F |
| PF-RF09-03 | RF-09 · HU-18 FA-03 | DAT | 16 | Sin perfil activo, salir no escribe ningún perfil |

---

## S-N1 · Nivel 1 completo con un perfil nuevo — T08 · EXE

**Pre:** `Preparar-Datos OE4_B` (para que la lista no esté vacía). `Start-Juego`.
**Coordenadas del panel** (referencia 1920×1080; confirmar con la foto del paso 8):
círculo de reunión centrado en (0,50; 0,50), radio ≈ 0,09 de ancho y 0,16 de alto.
Deslizante de fuerza (vertical, a la derecha): x = 0,931; muesca *n* en y = (703,6 − 32,8·*n*)/1080,
es decir, 2 → 0,591, 3 → 0,560, 7 → 0,439, 8 → 0,409. Deslizante de cercanía (horizontal, sobre los
botones): y = 0,794; muesca *n* en x = (788 + 34,4·*n*)/1920, es decir, 0 → 0,410, 5 → 0,500.
Un clic en el riel lleva el asa a ese punto.
**Solución del reto** (`N1_Config.asset`): fuerza 7 u 8 **y** cercanía exactamente 5; 3 golpes
efectivos habilitan «Soplar».
**Cifras previstas:** I 5 · C 1 · P 3; el tiempo es (T1 − T0 − 30 s de pausa) ± 5 s.

1. «Jugar» → `Escribir "OE4N1"` → «Continuar» → `N1_Apertura`, primera línea, **sin «Omitir»** y
   sin botón de pausa (puede asomar un instante el menú de niveles: anotarlo).
   `Datos/OE4N1.json` = `{"name":"OE4N1","reachedLevel":1,"phases":[]}`.
   ‹PF-RF02-01, PF-RF05-01, PF-RF06-01, PF-RF07-05›
2. Clic en el centro de la ilustración → la línea no cambia. ‹PF-RF05-01›
3. «Continuar» línea a línea por `N1_Apertura` (7 líneas), `N1_AparicionGuia` (11) y
   `N1_Hallazgo` (18), con fundido a negro entre escenas; cada clic trae una línea nueva y ninguna
   escena ofrece «Omitir». Foto de ALGORITM: «Lo que no sabes todavía es cómo. ¿Desde dónde vas a
   intentarlo?». ‹PF-RF05-01, PF-RF06-01, PF-RF10-01›
4. Último «Continuar» → `Medir-Carga` → `Level1_Cave`; anotar T0. Se ven **solo**: 7 piezas
   regadas (5 montoncitos de hojas, sílex y pedernal), la tablilla «Reúne todas las hojas y las dos
   piedras en el centro de la pantalla.», el botón de pista (círculo con Algoritm, sin texto, arriba
   a la izquierda) y el de pausa. Sin deslizantes, sin «Golpear», sin «Soplar». Cueva en penumbra.
   ‹PF-RNF04-01, PF-RF10-01, PF-RF14-01, PF-RF21-01›
5. Teclas de control negativo → nada cambia. ‹PF-RNF02-01›
6. Pista → la tablilla repite «Reúne todas…» y aparece el círculo en el centro. ‹PF-RF13-01›
7. `Arrastrar` una hoja a (0,20; 0,50) → no pasa nada: ni mensaje ni cambio. ‹PF-RF14-01›
8. `Arrastrar` las 7 piezas a destinos alrededor de (0,50; 0,50), a ≤ 0,04 del centro → al soltar
   la última, la cámara se acerca y las hojas forman el montón. Aparecen: el deslizante vertical
   «Fuerza del golpe» («Fuerte» arriba, «Suave» abajo), el horizontal «Lejos»–«Cerca», «Golpear» y
   «Soplar» atenuado con candado. La tablilla dice «Elige la fuerza y qué tan cerca van las
   piedras, y golpea para hacer chispas.». ‹PF-RF14-01 [PD: INC-47], PF-RF10-01›
9. Pista → la tablilla repite «Elige la fuerza…». ‹PF-RF13-01›
10. Fuerza a la muesca 3 y cercanía a la 5 → las piedras se mueven a la vista; la tablilla y la
    luz no cambian. ‹PF-RF15-01 [PD: INC-47]›
11. «Golpear» y `Foto` a +0,5 s → «Las piedras apenas se rozan. No sale ninguna chispa.»; Papá
    golpea y levanta el puño; la luz no cambia; ningún cartel de derrota.
    ‹PF-RF11-01, PF-RF16-01, PF-RF17-01, PF-RF18-01›
12. «Golpear» → «Los golpes suaves solo hacen ruido: las chispas no llegan a nacer.», distinto
    del anterior. ‹PF-RF18-01›
13. «Golpear» → la tablilla muestra la pista «Las chispas dependen de cómo chocan las piedras: de
    la fuerza y de qué tan juntas están. ¿Qué cambiarías antes del próximo golpe?», que no nombra
    ninguna muesca. ‹PF-RF13-01, PF-RF17-01›
14. Clic en «Soplar» atenuado → nada: ni mensaje ni cambio. Anotar si algo sugiere qué probar
    (HU-07 FA-01). ‹PF-RF19-01, PF-RF19-02›
15. Cercanía a la 0 → «Golpear» → «Otra vez las piedras no llegan a tocarse. Sin choque no hay
    chispa.»: habla de las piedras y no de la fuerza, y no repite el texto anterior.
    ‹PF-RF16-01, PF-RF18-01›
16. Pausa → «Pausa» con «Reanudar», «Reiniciar» y «Volver al menú de niveles». Esperar **30 s**
    con la pausa abierta → «Reanudar» → los deslizantes, la tablilla y la luz siguen igual.
    ‹PF-RF07-01 [PD: INC-49], PF-RF07-02›
17. Fuerza a la 7 y cercanía a la 5 → «Golpear» → «Una chispa cae dentro de las hojas. Brilla un
    instante y se apaga.»; la luz sube un escalón; «Soplar» sigue con candado.
    ‹PF-RF16-01, PF-RF19-01, PF-RF21-01›
18. «Golpear» → «Otra chispa cae dentro. Esta vez el brillo dura un poco más.»; la luz sube otro
    escalón. ‹PF-RF21-01›
19. «Golpear» → «Un hilo de humo sube despacio, como si la cueva estuviera suspirando.»;
    «Soplar» se habilita y pierde el candado. ‹PF-RF19-01›
20. Fuerza a la 2 → «Golpear» → mensaje de golpe suave; «Soplar» **sigue** habilitado; la luz no
    baja. ‹PF-RF19-01, PF-RF21-01›
21. «Soplar» (un solo clic) → «El humo se abre. Algo naranja tiembla entre las hojas.»; nace la
    llama sobre el montón y la luz sube hasta el máximo en ~3,5 s. Tres fotos en ese lapso y una
    ráfaga de ~10 fps durante 6 s del bucle de la llama (para PF-RNF21-01). Anotar T1 al terminar
    la subida. ‹PF-RF20-01, PF-RF21-01, PF-RNF21-01›
22. Fundido a `N1_NacimientoDelFuego` (17 líneas), **sin «Omitir»**; leerla entera; foto de «Eso
    tiene nombre: se llama iterar. Probar, mirar el resultado y ajustar.».
    ‹PF-RF05-01, PF-RF06-01, PF-RF12-01›
23. `LevelSummary`: «Esto es lo que pasó en la cueva», «Descubriste que el fuego necesita chispa y
    aire.», «Probaste golpear con varias fuerzas y desde varios sitios.», «Cuando algo no
    funcionó, cambiaste la fuerza o el sitio y volviste a intentar.» y «Eso se llama probar y
    ajustar: …»; **ningún dígito en pantalla**. ‹PF-RF45-01, PF-RF17-01›
24. `Datos/OE4N1.json`: `reachedLevel` 2 y la fase `{level 1, phase 1, attempts 5,
    correctedErrors 1, stepsUsed 3, resolutionSeconds ≈ T1 − T0 − 30 (± 5)}`.
    ‹PF-RF04-01, PF-RF07-06›
25. «Continuar» → menú de niveles con «Nivel 2» sin candado y «Nivel 3» bloqueado. ‹PF-RF03-02›
26. `Cerrar-Juego`; copiar el JSON a `evidencias/S-N1_OE4N1.json`; `Revisar-Log S-N1`.

| Caso | Req · trazas | Etq | Pasos | Aprueba si |
|---|---|---|---|---|
| PF-RF05-01 | RF-05 · HU-02, CU-03 | NAV | 1–3, 22 | Las narrativas del N1 salen en orden, una línea por clic en «Continuar», y solo el botón avanza |
| PF-RF06-01 | RF-06 · HU-14 FA-02, INC-28 | BOT | 1, 3, 22 (y los pasos de primera visita de S-N2A, S-N2B, S-N2C y S-N3) | La primera vez ninguna narrativa ofrece «Omitir» |
| PF-RF07-05 | RF-07 · HU-17 FA-04 | BOT | 1 y toda narrativa | No hay botón de pausa en las narrativas |
| PF-RF10-01 | RF-10 · HU-02 | RETO | 3, 4, 8 | El guía formula el objetivo con preguntas y lo descompone; la tablilla da la tarea activa |
| PF-RF11-01 | RF-11 · HU-05 | RETO | 11–21 | Cada acción recibe un mensaje narrativo en < 1 s, sin palabras como «error», «incorrecto» o «mal», y sin interrumpir la partida |
| PF-RF13-01 | RF-13 · HU-03 | BOT | 6, 9, 13 | La ayuda repite la instrucción vigente sin cambiar nada; al tercer fallo seguido aparece una pista que orienta sin dar la solución |
| PF-RF14-01 | RF-14 · HU-06, CU-05 | RETO | 4, 7, 8 | Tras reunir aparece el panel con los controles de hipótesis, «Golpear» y el área de resultados. [PD: INC-47] |
| PF-RF15-01 | RF-15 · HU-06 | RETO | 10 | Mover los controles no produce ningún efecto hasta golpear. [PD: INC-47] |
| PF-RF16-01 | RF-16 · HU-06 | RETO | 11, 15, 17 | Todo golpe tiene una consecuencia visible; solo la combinación efectiva suma. [PD: INC-47] |
| PF-RF17-01 | RF-17 · HU-05 | RETO | 11–23 | Mensajes narrativos sin cifras ni juicios de valor. [PD: INC-47: solo se ve el último mensaje; HU-05 pide que se acumulen] |
| PF-RF18-01 | RF-18 · HU-04 | RETO | 11–15, 20 | Sin tope ni derrota; dos mensajes seguidos nunca son iguales |
| PF-RF19-01 | RF-19 · HU-07, INC-32 | BOT | 14, 17, 19, 20 | «Soplar» se habilita al tercer golpe efectivo, no antes, y no se vuelve a deshabilitar |
| PF-RF19-02 | RF-19 · HU-07 FA-01 | BOT | 14 | Pulsar «Soplar» deshabilitado sugiere qué probar. ⚠ probable F |
| PF-RF20-01 | RF-20 · HU-07, CU-05 | RETO | 21–22 | «Soplar» reproduce el nacimiento del fuego y lleva a la escena de cierre |
| PF-RF21-01 | RF-21 (Baja) · HU-07 | RETO | 4, 17–21 | La luz sube por escalones con cada acierto, nunca baja y termina en iluminación completa |
| PF-RF12-01 | RF-12 · HU-14 | RETO | 22 | El guía nombra la habilidad (iterar) y la relaciona con lo que hizo el jugador |
| PF-RF45-01 | RF-45 · HU-14 paso 6, INC-26 | DAT | 23 | Resumen narrativo sin dígitos, con las variantes que corresponden a I > 0 y C > 0 |
| PF-RF04-01 | RF-04, RF-45 | DAT | 24 | El JSON guarda la fase del N1 con los cuatro indicadores previstos y `reachedLevel` 2 |
| PF-RF07-06 | RF-07 · OE1 §3.6.1 nota 1, HU-17 | DAT | 16, 24 | El tiempo de resolución no incluye los 30 s de pausa |
| PF-RF03-02 | RF-03 · HU-14 paso 7 | NAV | 25 (y S-N2C paso 18) | Terminar un nivel habilita el siguiente, y solo ese |

---

## S-N2A · Nivel 2, fase 1: el bosque — T09 · EXE

**Pre:** `Preparar-Datos OE4_B`. `Start-Juego`. **Cifras previstas (N2F1):** I 3 · C 1 · P 0.

1. «Jugar» → «OE4_B» → «Nivel 2 · La Rueda» → `N2_PuenteI` (3 líneas) → `N2_PuenteI_Bosque` (9) →
   `N2_Escena21_Bosque` (7), **sin «Omitir»**. Leerlas todas y anotar si ALGORITM formula el
   objetivo con alguna pregunta (RF-10). Foto de «Selecciona los objetos que se muevan con
   facilidad y únelos. Cuando tengas cinco, veremos qué pasa.».
   ‹PF-RF05-02, PF-RF06-01, PF-RF10-02›
2. `Medir-Carga` → `Level2_Forest`; anotar T0. La tablilla muestra la instrucción; el contador
   dice «Troncos redondos: 0 de 5», con 5 casillas vacías; en el suelo hay 14 objetos: 5 troncos
   redondos iguales, 3 piedras, 3 plantas y 3 herramientas. La caja está en el suelo y no hay
   «Empujar». ‹PF-RNF04-01, PF-RF22-01, PF-RF24-01›
3. Teclas de control negativo → nada. ‹PF-RNF02-01›
4. Clic sostenido en la caja y soltar a su lado → «Todavía faltan 5 troncos para mover la caja.»;
   la caja no se mueve. ‹PF-RF25-01›
5. Clic en una piedra → «Esta piedra tiene esquinas. Cuando la empujas, se traba.» con icono de
   alerta; la piedra sigue en su sitio; el contador sigue en «0 de 5».
   ‹PF-RF23-01, PF-RF24-01, PF-RF11-02, PF-RNF19-01›
6. Clic en una planta → «Esto no soporta el peso de la caja.» ‹PF-RF23-01›
7. Clic en una herramienta → en lugar del mensaje, la pista «Mira cómo se mueve cada uno cuando
   lo empujas. ¿Cuáles se traban y cuáles no?», sin icono. ‹PF-RF13-02›
8. Botón de ayuda (círculo con la rueda, abajo a la derecha) → repite la instrucción del paso 2.
   ‹PF-RF13-02›
9. Clic en cada uno de los 5 troncos → cada uno pasa a una casilla; «Este rueda. ¿Qué tiene que
   los otros no tienen?» con icono de acierto; el contador sube de 1 a 5 y nunca baja.
   ‹PF-RF23-01, PF-RF24-01, PF-RF11-02›
10. Con el quinto: desaparecen los distractores, los troncos se alinean junto a la caja, la cámara
    se acerca y aparece «Empujar» deshabilitado. El contador sigue a la vista con «5 de 5».
    ‹PF-RF24-01, PF-RF26-01›
11. `Arrastrar` la caja y soltarla lejos de los troncos → «Ahí la caja no toca los troncos.
    Déjala encima de ellos.»; «Empujar» sigue deshabilitado. ‹PF-RF25-01›
12. `Arrastrar` la caja sobre los troncos → «La caja quedó sobre los troncos. Ahora empújala.»;
    «Empujar» se habilita. ‹PF-RF25-01, PF-RF26-01›
13. «Empujar» → fundido a `N2_Escena22_ElPatron`: la caja rueda sobre los troncos **en la
    narrativa**. ‹PF-RF26-01 [PD: INC-50]›
14. `Datos/OE4_B.json`: fase `{level 2, phase 1, attempts 3, correctedErrors 1, stepsUsed 0}` y
    un tiempo de (T(«Empujar») − T0) ± 5 s. ‹PF-RF04-02›
15. «Continuar» hasta `N2_Escena23_Construccion` → sigue S-N2B en el mismo proceso.

| Caso | Req · trazas | Etq | Pasos | Aprueba si |
|---|---|---|---|---|
| PF-RF05-02 | RF-05 · HU-02 | NAV | S-N2A 1, 13; S-N2B 1; S-N2C 1, 15 | Las siete narrativas del N2 salen en orden, una línea por clic |
| PF-RF10-02 | RF-10 · HU-02, HU-08, CU-06 | RETO | 1, 2 | El guía enuncia el objetivo del bosque **formulado en preguntas** y lo descompone. ⚠ se da en imperativo |
| PF-RF22-01 | RF-22 · HU-08, CU-06 | RETO | 2 | Bosque con troncos redondos, piedras, plantas y herramientas para discriminar |
| PF-RF23-01 | RF-23 · HU-08 FA-01, CU-06 3a | RETO | 5, 6, 9 | Lo redondo se acepta; lo demás se rechaza con un mensaje narrativo propio de su categoría y queda en su lugar |
| PF-RF24-01 | RF-24 · HU-08 | RETO | 2, 5, 9, 10 | El avance «n de 5» está siempre a la vista y nunca retrocede |
| PF-RF25-01 | RF-25 · HU-08, CU-06 4/4a | BOT | 4, 11, 12 | La caja solo se arrastra con los cinco troncos, con clic sostenido, y se coloca al soltarla sobre ellos |
| PF-RF26-01 | RF-26 · HU-08, CU-06 5 | RETO | 10, 12, 13 | «Empujar» se habilita solo con la caja colocada y el rodado se ve. [PD: INC-50] |
| PF-RF13-02 | RF-13 · HU-03 | BOT | 7, 8 | Pista al tercer rechazo seguido; la ayuda repite la instrucción |
| PF-RF11-02 | RF-11 · HU-05 | RETO | 5, 9, S-N2B y S-N2C | En las tres escenas del N2, toda acción evaluada tiene respuesta visible en < 1 s (mensaje, resaltado o movimiento), narrativa y sin juicios |
| PF-RF04-02 | RF-04 | DAT | 14; S-N2B 16; S-N2C 17 | Cada fase del N2 queda en el JSON al completarse, con sus indicadores previstos |

---

## S-N2B · Nivel 2, fase 2: el taller — T10 · EXE

**Pre:** continúa S-N2A. Si se corre sola: `Preparar-Datos OE4_B2` → «Nivel 2» → tres narrativas
(sin «Omitir») → entra directo al taller. **Cifras previstas (N2F2):** I 5 · C 3 · P 6.

1. `N2_Escena23_Construccion` (8 líneas, nombra la cuerda) → `Medir-Carga` → `Level2_Workshop`;
   anotar T0. Se ven 7 piezas: cuerda, dos troncos cortos, tronco largo, mazo, tabla y caja.
   «Mecanizar» está deshabilitado, con candado y «Aún no». ‹PF-RF27-01 [PD: INC-54], PF-RF28-01,
   PF-RNF04-01, PF-RF05-02›
2. `Arrastrar` el mazo sobre un tronco corto sin haberlo elegido → «Mecanizar necesita un tronco
   corto resaltado.» con alerta. (I 1) ‹PF-RF28-01›
3. Clic en el tronco corto A → «El tronco quedó resaltado.»; el tronco lleva contorno y crece un
   poco; «Mecanizar» se habilita y desaparece «Aún no». ‹PF-RF28-01›
4. «Mecanizar» → el tronco se vuelve rueda; «Se abrió un agujero en el centro: el tronco ya es una
   rueda.»; «Mecanizar» vuelve a deshabilitarse. (C 1, P 1) ‹PF-RF28-01›
5. Tabla sobre el tronco B → «La tabla no tiene sobre qué apoyarse todavía.»; la tabla vuelve a
   su sitio y la rueda sigue siendo rueda. (I 2) ‹PF-RF29-01›
6. Caja sobre el tronco B → «La caja se caería. Falta algo plano debajo.» (I 3) ‹PF-RF29-01›
7. Cuerda sobre el tronco B → la pista «Mira la pieza que quieres poner. ¿Sobre qué se apoyaría
   si la sueltas ahora?». (I 4) ‹PF-RF13-03›
8. Botón de pista (arriba a la izquierda) → la instrucción. ‹PF-RF13-03›
9. Tronco largo soltado en una zona vacía lejos → «La pieza vuelve a su sitio: cayó lejos del
   lugar de armado.» (no suma). ‹PF-RF29-01›
10. Clic en el tronco B → «Mecanizar» → «Segunda rueda lista. Las dos tienen por dónde entrar
    algo.» (C 2, P 2) ‹PF-RF28-01›
11. Tronco largo sobre una rueda → «El tronco largo entró por el centro de las dos ruedas: ya hay
    un eje.»; aparece la carretilla con su eje. (P 3) ‹PF-RF29-01›
12. Caja sobre la carretilla → «La caja se caería. Falta algo plano debajo.» (I 5) ‹PF-RF29-01›
13. Tabla → «La tabla quedó montada sobre el conjunto.» (C 3, P 4)
14. Caja → «La caja va sobre la tabla.» (P 5)
15. Cuerda → «La cuerda sujeta la caja. La carretilla está completa.»; la familia celebra; fundido
    a `N2_Escena24_Regreso`. (P 6) ‹PF-RF29-01 [PD: INC-54]›
16. JSON: fase `{2, 2, attempts 5, correctedErrors 3, stepsUsed 6}` y un tiempo ± 5 s.
    ‹PF-RF04-02›
17. Sigue S-N2C.

| Caso | Req · trazas | Etq | Pasos | Aprueba si |
|---|---|---|---|---|
| PF-RF27-01 | RF-27 · HU-09, CU-07 | RETO | 1 | Área de trabajo con todas las piezas del armado. [PD: INC-54: más la cuerda] |
| PF-RF28-01 | RF-28 · HU-09 FA-01, CU-07 3a | BOT | 1–4, 10 | «Mecanizar» solo está habilitado con un tronco corto elegido, y perfora ese tronco |
| PF-RF29-01 | RF-29 · HU-09 FA-02/03, CU-07 5a/6a | RETO | 5, 6, 9, 11, 12, 15 | Todo paso fuera de orden se rechaza diciendo qué falta antes, sin deshacer lo hecho. [PD: INC-54] |
| PF-RF13-03 | RF-13 · HU-03 | BOT | 7, 8 | Pista al tercer rechazo seguido; la ayuda repite la instrucción |

---

## S-N2C · Nivel 2, fase 3: el laberinto y el cierre — T11 · EXE

**Pre:** continúa S-N2B. Si se corre sola: `Preparar-Datos OE4_B3` → «Nivel 2» → tres narrativas
→ laberinto. Conviene `Start-Juego -Ancho 1600 -Alto 900` si la pantalla lo permite: los arbustos
se dibujan a ×1,65 y tapan casillas vecinas.
**El tablero es aleatorio en cada entrada** (`Seed: 0`). Fijos: matriz 16 × 11 (columnas 0–15 de
izquierda a derecha, filas 0–10 de abajo arriba); salida (0,8) mirando al este; refugio (15,2);
(1,8) y (14,2) siempre libres; la «L» directa nunca está libre. Semántica (RF-31, INC-33):
«Avanzar ×n» y «Retroceder ×n» mueven *n* casillas respecto de donde mira la carretilla; «Girar»
rota 90° al lado elegido (por defecto a la derecha) sin moverse. Si choca, la carretilla intenta,
regresa y ese bloque abandona sus casillas restantes; la ejecución sigue con el bloque siguiente.
**Conteo de ediciones** (lo lleva Claude desde la última ejecución fallida): soltar un bloque en la
secuencia +1, papelera +1, tomar un bloque de la secuencia +1, cada clic en «−», «+», «‹» o «›» +1
(aunque no cambie nada), reordenar +2. No cuentan: «→», abrir o cerrar el cajón, ▲/▼.
**Cifras previstas (N2F3):** I = ejecuciones fallidas con bloques (3 + las que se deban a una
mala lectura del tablero) · C = conteo de ediciones antes de la ejecución ganadora · P = bloques
de la secuencia ganadora.

1. `N2_Escena24_Regreso` (8 líneas) → `Medir-Carga` → `Level2_Maze`; anotar T0. Se ven: el
   laberinto en vista superior con cuadrícula dentro del seto; la carretilla en el hueco izquierdo,
   mirando al este; los arbustos; la familia en el hueco derecho (el refugio). A la derecha: «Tu
   secuencia», «Suelta un bloque aquí», el cajón «Bloques» cerrado y «Ejecutar».
   ‹PF-RF30-01, PF-RF31-01, PF-RNF04-01, PF-RF05-02›
2. Foto y **transcribir el tablero** a una rejilla ASCII 16 × 11 en el registro de la sesión (`#`
   seto, `S` salida, `R` refugio, `X` arbusto). La foto va a evidencias. Es el mapa del paso 12.
3. «Ejecutar» con la secuencia vacía → «Añade al menos un bloque a tu secuencia antes de
   ejecutar.»; la carretilla no se mueve (no suma). ‹PF-RF32-01›
4. Abrir el cajón → «Avanzar» (rectángulo), «Girar» (píldora), «Retroceder» (con rombo) y
   «Arrastra un bloque a tu secuencia». ‹PF-RF31-01, PF-RNF19-01›
5. `Arrastrar` «Girar» a la secuencia y pulsar «‹» (izquierda); `Arrastrar` «Avanzar» debajo
   (×1). ‹PF-RF31-01›
6. «Ejecutar» → la carretilla gira a la izquierda (queda mirando al norte), intenta avanzar contra
   el seto, va y vuelve; queda resaltado el segundo bloque; aparece «La carretilla no llegó al
   refugio. Mira dónde se detuvo y corrige tu secuencia.» con alerta. A los ~0,6 s la carretilla
   reaparece en la salida mirando al este y la secuencia sigue igual. (I 1)
   ‹PF-RF32-01, PF-RF33-01, PF-RF34-01, PF-RNF19-01›
7. «Ejecutar» otra vez → lo mismo (I 2). Una tercera vez → en lugar del mensaje, la pista «Mira en
   qué paso se detuvo la carretilla. ¿Hacia dónde miraba justo antes?» (I 3). A partir de aquí
   empieza el conteo de ediciones. ‹PF-RF13-04, PF-RF34-01›
8. Botón de pista → la instrucción. ‹PF-RF13-04›
9. Papelera del bloque seleccionado (el último soltado, «Avanzar») → desaparece (+1). ‹PF-RF34-01›
10. ⚠ Clic sostenido sobre «Girar» en la secuencia y soltarlo fuera del panel (+1) → esperado: el
    bloque se retira y la secuencia queda vacía. **Si el bloque se queda pegado al cursor** y no se
    puede tomar otro: F de PF-RF34-02; foto y `Revisar-Log`; recuperar con pausa → «Reiniciar» (el
    tablero cambia: volver al paso 2 y reiniciar la cuenta de intentos y ediciones de la fase).
    ‹PF-RF34-02›
11. ⚠ Armar «Avanzar ×1», «Girar», «Avanzar ×2» (con «+»); tomar el tercero y soltarlo encima del
    primero (+2) → esperado: «Avanzar ×2», «Avanzar ×1», «Girar». Mismo riesgo y misma
    recuperación. Después vaciar la secuencia con la papelera (desplegando con «→» si hace falta).
    ‹PF-RF34-02›
12. Sobre la rejilla del paso 2, planificar un camino de (1,8) a (15,2) que entre al refugio desde
    (14,2) mirando al este. Componerlo con «Avanzar ×n» (máximo 9 por bloque) y «Girar» con su
    lado. Anotar el total de ediciones y el número de bloques.
13. Si aparecen ▲/▼ y la barra, pulsarlos (sin arrastrar) → la lista se desplaza.
    ‹PF-RF34-01, PF-RNF02-01›
14. «Ejecutar» → la carretilla recorre la secuencia paso a paso y en todo momento se ve resaltado
    el bloque en curso (fotos a mitad de camino). ⚠ Si con la lista larga el bloque en curso queda
    fuera de la vista, es F de PF-RF32-01. Al llegar: «¡La carretilla llegó al refugio! Los pasos,
    en ese orden, funcionaron.»; celebran; fundido a `N2_Escena25_Cierre`. **Si choca por una mala
    lectura del tablero, no es defecto:** corregir la secuencia (las ediciones siguen contando, la
    ejecución suma un intento), actualizar la predicción y repetir. ‹PF-RF32-01›
15. `N2_Escena25_Cierre` (6 líneas), **sin «Omitir»**; fotos de «…Eso se llama abstraer.» y de
    «…eso es pensar como un algoritmo.». ‹PF-RF12-02, PF-RF06-01, PF-RF05-02›
16. `LevelSummary`: «Esto es lo que pasó con la rueda», «Descubriste que lo redondo rueda y que una
    carretilla se arma en orden.», «Probaste objetos, piezas y caminos que no servían antes de dar
    con los buenos.», «Cuando algo no encajó, cambiaste de idea y volviste a intentar.» y «Eso se
    llama abstraer y pensar como un algoritmo: …»; ningún dígito. ‹PF-RF45-02›
17. JSON: `reachedLevel` 3 y la fase `{2, 3, attempts / correctedErrors / stepsUsed previstos}`.
    ‹PF-RF04-02›
18. «Volver al menú de niveles» → menú de niveles con el N3 desbloqueado. ‹PF-RF03-02›
19. `Cerrar-Juego`; copiar el JSON a `evidencias/S-N2_OE4_B.json`; `Revisar-Log S-N2`.

| Caso | Req · trazas | Etq | Pasos | Aprueba si |
|---|---|---|---|---|
| PF-RF30-01 | RF-30 · HU-10, CU-08 | RETO | 1 | Vista superior con carretilla, refugio reconocible y obstáculos. Observación: el refugio no se dibuja (solo el hueco y la familia) y no hay «curvas ni pendientes» |
| PF-RF31-01 | RF-31 · HU-10, INC-33 | BOT | 1, 4–6 | Pantalla dividida, bloques de avanzar, retroceder y girar que se arrastran a una secuencia ordenada y se leen respecto de la orientación. Observación: cuentas ×n y lado de giro, que RF-31 no menciona |
| PF-RF32-01 | RF-32 · HU-10 FA-01, CU-08 3a | RETO | 3, 6, 14 | «Ejecutar» recorre paso a paso con el bloque en curso resaltado y visible; la secuencia vacía se informa sin moverse |
| PF-RF33-01 | RF-33 · HU-10, HU-04 FA-02 | RETO | 6 | Ante un obstáculo, la carretilla intenta, regresa a la casilla anterior y el nivel no se reinicia |
| PF-RF34-01 | RF-34 · HU-10, CU-08 6a | BOT | 6, 7, 9, 13 | Tras un fallo la secuencia sigue en pantalla; se retira con la papelera y se reejecuta sin reiniciar ni perder el escenario |
| PF-RF34-02 | RF-34 · HU-10 «se pueden reordenar libremente» | BOT | 10, 11 | Un bloque de la secuencia se retira soltándolo fuera y se reordena soltándolo en otro sitio. ⚠ riesgo alto |
| PF-RF13-04 | RF-13 · HU-03 | BOT | 7, 8 | Pista a la tercera ejecución fallida seguida; la ayuda repite la instrucción |
| PF-RF12-02 | RF-12 · HU-14 | RETO | 15 | El cierre nombra abstraer y pensar como un algoritmo |
| PF-RF45-02 | RF-45 · HU-14 | DAT | 16 | Resumen del N2 sin dígitos, con las variantes que corresponden |

---

## S-N3 · Nivel 3 y cierre del juego — T12 · EXE

**Pre:** `Preparar-Datos OE4_C`. `Start-Juego`.
**Posiciones en la orilla** (plano fijo de la recolección): Mamá sale en (0,28; 0,72). Materiales:
tronco_2 (0,28; 0,87) · tronco_4 (0,42; 0,87) · sogas (0,51; 0,68) · tela (0,42; 0,56) · mástil
(0,28; 0,56) · tronco_5 (0,28; 0,43) · tronco_3 (0,42; 0,43) · tronco_1 (0,53; 0,51). Zona de
construcción (0,66; 0,60). Zona transitable: x 0,25–0,68, y 0,41–0,91. Cruceta, abajo a la
derecha: ↑ (0,78; 0,69), ↓ (0,78; 0,88), ← (0,73; 0,79), → (0,83; 0,79). «Recoger» en la esquina
inferior derecha. «Recoger» aparece a ≤ 0,04 de un material, una elipse de ~±0,08 × ±0,08 de
pantalla. Velocidad de Mamá: ~0,47 de la pantalla por segundo en cada eje.
**Cifras previstas:** los conteos del N3 **se acumulan entre fases** (`RiverIndicatorCollector`,
«Los conteos siguen acumulando»). N3F1 1/1/1 · N3F2 4/1/2 · N3F3 6/3/3.

1. «Jugar» → «OE4_C» → «Nivel 3 · El Río» → `N3_PuenteII` (2 líneas) → `N3_PuenteII_Horizonte` (5)
   → `N3_PuenteII_Rio` (4) → `N3_Escena31_Llegada` (8), **sin «Omitir»**. Fotos de «Primero,
   descompongamos el problema: ¿cuántas cosas necesitamos para cruzar?» y de la secuencia «¿Qué
   necesitamos? Una balsa.» … «¡Ya tenemos nuestra lista!». ‹PF-RF05-03, PF-RF06-01, PF-RF10-03›
2. `Medir-Carga` → `Level3_River`. Se ven: la lista de tareas arriba a la izquierda («Recoger
   troncos», «Encontrar sogas», «Ensamblar la balsa», «Colocar el mástil y la vela»), todas
   pendientes; la instrucción «Recorre la orilla y recoge lo que la balsa necesita…»; el inventario
   2×2 vacío abajo a la izquierda; la cruceta de cuatro flechas abajo a la derecha; la zona de
   construcción marcada a la derecha, junto al agua. Sin «Recoger».
   ‹PF-RNF04-01, PF-RF36-01, PF-RF38-01, PF-RF39-01, PF-RF35-01, PF-RF10-03›
3. Teclas de control negativo → Mamá no se mueve. ‹PF-RNF02-01›
4. `Sostener` ↓ 300 ms → Mamá baja y se detiene al soltar. `Sostener` ← 3000 ms → llega al borde
   izquierdo y no lo cruza. ‹PF-RF35-01›
5. Llevar a Mamá junto a tronco_2 → aparece «Recoger»; alejarla → desaparece; volver. ‹PF-RF37-01›
6. «Recoger» → el tronco sale de la orilla y entra a su casilla (se enciende 1 de sus 5 marcas);
   «Troncos: al inventario. Mira la lista: ¿qué falta?». ‹PF-RF37-01, PF-RF38-01›
7. Llevar a Mamá a la zona con ese solo tronco → «Para armar la balsa todavía falta: …», con los
   nombres y sin números, e icono de alerta; no se abre nada. ‹PF-RF39-01, PF-RNF19-01›
8. Ayuda (gota, arriba a la derecha) → repite la instrucción; el inventario no cambia.
   ‹PF-RF13-05›
9. Recoger el resto, en este orden: tronco_4, sogas, tela, mástil, tronco_5, tronco_3, tronco_1.
   Con el quinto tronco se marca «Recoger troncos» (cambia la **forma** del icono y su color); con
   las sogas, «Encontrar sogas»; la tela y el mástil no marcan nada. Tras la octava pieza: «Ya
   tienes todos los materiales. Ve a la zona marcada junto al agua.».
   ‹PF-RF36-01, PF-RF37-01, PF-RF38-01›
10. Entrar a la zona → «Tienes todo lo de la lista. Aquí se arma la balsa.»; desaparecen las
    flechas y «Recoger»; la cámara se acerca a la balsa y aparecen las 5 siluetas de la base.
    Anotar T0. ‹PF-RF39-01, PF-RF40-01›
11. Base: `Arrastrar` desde el inventario el **mástil** a una silueta y cuatro troncos a las otras
    → «Listo» → «Algo de esta parte no encaja. Mira lo marcado: ¿esa pieza va ahí?»; el espacio del
    mástil lleva el icono de alerta; el mástil vuelve al inventario; los troncos se quedan. (I 1)
    ‹PF-RF40-01, PF-RF43-01, PF-RNF19-01›
12. Poner el quinto tronco → «Listo» → pulso y martillazos; «La base quedó firme. Ahora hay que
    sujetarla para que no se abra.»; aparecen las 10 siluetas de los amarres. (C 1, P 1) Intentar
    agarrar un tronco de la base → no se mueve. El JSON ya tiene la fase `{3, 1, …}`.
    ‹PF-RF41-01, PF-RF40-01, PF-RF04-03›
13. Amarre: soga en 9 de los 10 amarres → «Listo» → rechazado, con el amarre vacío marcado (I 2).
    Anotar si «Listo» estaba habilitado con la fase incompleta (HU-12 FA-01). «Listo» otra vez sin
    cambiar nada (I 3); una tercera vez → la pista «Si la balsa se abriera al navegar, ¿qué le
    faltaría para quedarse junta?» (I 4). ‹PF-RF40-01, PF-RF40-02, PF-RF13-05›
14. Poner la última soga → «Listo» → «La balsa ya no se abre. Falta lo que atrapa el viento.»; se
    marca «Ensamblar la balsa». (P 2) ‹PF-RF41-01, PF-RF36-01›
15. Mástil y vela: la **tela** en la silueta del mástil y el **mástil** en la de la vela; el
    botón dice «Probar balsa» → la balsa se inclina y se hunde un momento; los dos espacios llevan
    alerta; «La balsa se volteó. Mira el espacio marcado: ¿qué debería ir ahí?»; tela y mástil
    vuelven al inventario; base y amarres siguen puestos (I 5). Fundido a
    `N3_Escena32_PrimerIntento` (5 líneas, dice «depurar»).
    ‹PF-RF42-01, PF-RF43-01, PF-RF11-03›
16. Al volver: `Level3_River` en el ensamblaje, en mástil y vela, con base y amarres armados y tela
    y mástil en el inventario. ‹PF-RF43-01›
17. Cruzarlos otra vez → «Probar balsa» → se hunde y avisa, pero **no** vuelve la escena 3.2.
    (I 6) ‹PF-RF42-01›
18. Mástil en el mástil y tela en la vela → «Probar balsa» → martillazos; «La balsa flota derecha.
    ¡A cruzar!»; se marca «Colocar el mástil y la vela»; fundido a `N3_Escena33_Cruce`, donde la
    balsa cruza el río (~9 s). (C 3, P 3) ‹PF-RF44-01, PF-RF41-01, PF-RF36-01›
19. `N3_Escena33_Cruce` (7 líneas), **sin «Omitir»**; foto de «Eso es pensar
    computacionalmente.». ‹PF-RF12-03, PF-RF05-03›
20. `LevelSummary`: «Esto es lo que pasó en el río», «Probaste piezas que no iban en su sitio y la
    balsa te lo dijo al hundirse.», «Cuando algo falló, buscaste justo la parte rota y arreglaste
    solo esa.» y «Eso se llama descomponer y depurar: …»; ningún dígito. ‹PF-RF45-03›
21. ⚠ «Volver al menú de niveles» → esperado por su rótulo: el menú de niveles (o que el N3 no
    ofrezca ese botón). Se sabe que lleva a `N3_EscenaFinal`. ‹PF-RF45-03›
22. `N3_EscenaFinal` (7 líneas), sin «Omitir» → `Credits` → «Volver» → pantalla de inicio.
    ‹PF-RF44-01, PF-RF08-01, PF-RF05-03›
23. JSON: `{3,1: 1/1/1}`, `{3,2: 4/1/2}`, `{3,3: 6/3/3}` y tiempos plausibles.
    ‹PF-RF04-03›
24. `Cerrar-Juego`; copiar el JSON a `evidencias/S-N3_OE4_C.json`; `Revisar-Log S-N3`.

| Caso | Req · trazas | Etq | Pasos | Aprueba si |
|---|---|---|---|---|
| PF-RF05-03 | RF-05 · HU-02 | NAV | 1, 15, 19, 22 | Las narrativas del N3 salen en orden, incluida la 3.2 tras el primer fallo |
| PF-RF10-03 | RF-10 · HU-02, HU-11 | RETO | 1, 2 | El guía descompone el objetivo con preguntas y la lista lo materializa |
| PF-RF35-01 | RF-35 · HU-11, CU-09 | BOT | 2–4 | Mamá se mueve en dos ejes con **botones en los costados derecho e izquierdo**, dentro de los límites. ⚠ son una cruceta abajo a la derecha, sin INC |
| PF-RF36-01 | RF-36 · HU-11, INC-30, INC-46 | RETO | 2, 9, 14, 18 | Cuatro tareas siempre visibles; cada una se marca al completarse, por forma y color, sin cifras |
| PF-RF37-01 | RF-37 · HU-11, CU-09 | BOT | 5, 6, 9 | «Recoger» aparece solo junto a un material y lo lleva al inventario |
| PF-RF38-01 | RF-38 · HU-11 FA-01 | RETO | 2, 6, 9 | Inventario visible de cuatro casillas; no admite una quinta clase de objeto. Observación: 8 piezas en 4 casillas |
| PF-RF39-01 | RF-39 · HU-11 FA-02 | RETO | 2, 7, 10 | Zona señalizada; sin todo, dice qué falta y no abre; con todo, abre el panel |
| PF-RF40-01 | RF-40 · HU-12, CU-10 | RETO | 10–13 | Tres fases sucesivas (base, amarre, mástil y vela); solo se ven los espacios de la fase activa; ninguna se habilita sin aprobar la anterior |
| PF-RF40-02 | RF-40 · HU-12 FA-01 | BOT | 13 | El botón de confirmación no se habilita con la fase incompleta. ⚠ siempre está habilitado |
| PF-RF41-01 | RF-41 · HU-12 | RETO | 12, 14, 18 | Cada fase aprobada tiene su animación de completado y ya no se puede tocar |
| PF-RF42-01 | RF-42 · HU-13, CU-10 FA-6a/6b | RETO | 15, 17 | «Probar balsa» fallido: hundimiento, espacio marcado con icono y mensaje de qué revisar; la 3.2 solo la primera vez |
| PF-RF43-01 | RF-43 · HU-13, HU-04 FA-01 | DAT | 11, 15, 16 | Tras el fallo, solo lo mal puesto vuelve al inventario y las fases aprobadas se conservan |
| PF-RF44-01 | RF-44 · HU-13, CU-10 | NAV | 18, 22 | Con el ensamblaje correcto: cruce automático y cierre del juego hasta los créditos |
| PF-RF13-05 | RF-13 · HU-03 | BOT | 8, 13 | Ayuda sin alterar el inventario; pista al tercer rechazo seguido |
| PF-RF11-03 | RF-11 · HU-05 | RETO | 6, 7, 11–18 | Toda acción evaluada del N3 tiene una respuesta narrativa en < 1 s (el mensaje de la balsa llega tras 0,6 s de hundimiento) |
| PF-RF12-03 | RF-12 · HU-14 | RETO | 19 | El cierre nombra la habilidad y la relaciona con lo hecho. Observación: «descomponer» y «depurar» están en la 3.1, la 3.2 y el resumen, no en la 3.3 |
| PF-RF45-03 | RF-45 · HU-14 | NAV | 20, 21 | Resumen sin dígitos; cada botón hace lo que dice. ⚠ «Volver al menú de niveles» lleva a la escena final |
| PF-RF04-03 | RF-04 · OE1 §3.6.1 | DAT | 12, 23 | Cada fase del N3 queda en el JSON al aprobarse |

---

## S-PAU · Pausa y reinicio en las cinco escenas jugables — T13 · EXE

**Cómo se llega a cada escena** (siempre con `Preparar-Datos` antes):

| Escena | Semilla y camino | Cambio de estado del paso (a) |
|---|---|---|
| `Level1_Cave` | `OE4_Z` → «Nivel 1» → «Omitir» ×3 | Dos piezas dentro del círculo |
| `Level2_Forest` | `OE4_Z` → «Nivel 2» → «Omitir» ×3 | Dos troncos recogidos |
| `Level2_Workshop` | `OE4_B2` → «Nivel 2» → 3 narrativas | Tronco A mecanizado |
| `Level2_Maze` | `OE4_B3` → «Nivel 2» → 3 narrativas | Una secuencia de 4 bloques en ejecución: **se pausa a mitad de la ejecución** |
| `Level3_River` (orilla) | `OE4_Z` → «Nivel 3» → «Omitir» ×4 | Un material recogido y la flecha → sostenida al pausar |
| `Level3_River` (ensamblaje) | `OE4_D` → «Nivel 3» → 4 narrativas | Tres sogas puestas |

En cada una:

a. Provocar el cambio de estado de la tabla. Foto.
b. Botón de pausa (icono, sin texto) → «Pausa» con «Reanudar», «Reiniciar» y «Volver al menú de
   niveles»; el juego se detiene (la carretilla no avanza; Mamá no se mueve aunque se sostenga una
   flecha; los clics sobre el juego no hacen nada). `Teclas Esc` no abre ni cierra la pausa.
   ‹PF-RF07-01 [PD: INC-49]›
c. «Reanudar» → el estado es exactamente el de (a) (en el laberinto, la ejecución sigue desde
   donde quedó). ‹PF-RF07-02›
d. Pausa → «Reiniciar» → «Vas a volver a empezar esta parte del nivel. Lo que ya guardaste no se
   pierde.» con «Sí, reiniciar» y «Cancelar» → «Cancelar» → vuelven los tres botones; «Reanudar»
   → nada cambió. ‹PF-RF07-03›
e. Pausa → «Reiniciar» → «Sí, reiniciar» → la fase empieza de nuevo (el cambio de (a) se deshizo)
   y la tablilla muestra la instrucción inicial de la fase; no se reproduce ninguna narrativa; el
   JSON no cambia. En el ensamblaje del N3 reabre en el amarre con la base armada; en el laberinto,
   el tablero es otro. ‹PF-RF07-03›
f. Pausa → «Volver al menú de niveles» → menú de niveles; el JSON no cambia y los niveles
   alcanzados siguen habilitados. ‹PF-RF07-04›

| Caso | Req · trazas | Etq | Aprueba si |
|---|---|---|---|
| PF-RF07-01 | RF-07 · HU-17 | BOT | En las cinco escenas hay un menú de pausa que detiene el juego. [PD: INC-49: los rótulos son los del mockup 6] |
| PF-RF07-02 | RF-07 · HU-17 | BOT | «Reanudar» restituye el estado exacto (S-N1 paso 16 y los seis casos de aquí) |
| PF-RF07-03 | RF-07 · HU-17 FA-01/02 | BOT | Reiniciar pide confirmación; cancelar no cambia nada; confirmar reinicia la fase sin tocar lo guardado |
| PF-RF07-04 | RF-07 · HU-17 FA-03 | NAV | Volver al menú conserva el progreso confirmado |

---

## S-OMI · Omisión, rejuego y casos negativos del N1 — T14 · EXE

**Pre:** `Preparar-Datos OE4_Z`. `Start-Juego`.

1. «Jugar» → «OE4_Z» → menú con los tres niveles habilitados.
2. «Nivel 1» → `N1_Apertura` **con** «Omitir» → «Omitir» → `N1_AparicionGuia` con «Omitir» →
   «Omitir» → `N1_Hallazgo` → «Omitir» → `Level1_Cave`. Anotar que son tres clics (HU-02 FA-01
   dice «salta directamente a la escena jugable»; RF-06 se cumple escena por escena: es una
   observación documental). ‹PF-RF06-02›
3. Reunir las piezas; fuerza 7, cercanía 5; «Golpear» ×3 → «Soplar» habilitado. ⚠ Un **cuarto**
   golpe efectivo → anotar si la luz llega al máximo antes de soplar (el guion §1.4.3 E7 deja la
   iluminación completa para el soplido). ‹PF-RF21-02›
4. ⚠ Doble clic rápido en «Soplar» (dos clics a < 150 ms) → esperado: una sola ignición y una sola
   narrativa de cierre, que avanza con normalidad. ‹PF-RF20-02›
5. `N1_NacimientoDelFuego` **con** «Omitir» → **doble clic rápido** en «Omitir» (dos clics a
   < 150 ms) → `LevelSummary` directo, sin saltárselo hacia el menú de niveles. Este bug lo
   corrigió otra sesión el 29/09 en `NarrativeSceneController.Leave()`: el paso es su regresión.
   ‹PF-RF06-03, PF-RF45-05›
6. El resumen usa las viñetas de las cifras **guardadas en la semilla** (I 7 > 0 → «Probaste
   golpear…»; C 3 > 0 → «Cuando algo no funcionó…») y el JSON conserva `{1,1: 7/3/3/125}`.
   Oráculo: OE1 §3.6.1 nota 4 conserva el registro anterior y manda sobre HU-14 FA-01 («los
   indicadores del nuevo intento se registran igual»); el choque se anota como observación
   documental. ‹PF-RF45-04›
7. «Continuar» → «Nivel 3» → `N3_PuenteII` … `N3_Escena31_Llegada`, cada una con «Omitir» (4
   clics) → la orilla, con la recolección desde cero. ‹PF-RF06-02›
8. Recolectar y armar base y amarre; en mástil y vela, cruzar las piezas → «Probar balsa» →
   `N3_Escena32_PrimerIntento` **con** «Omitir» → «Omitir» → vuelve al ensamblaje. ‹PF-RF06-04›
9. Acertar → `N3_Escena33_Cruce` **sin** «Omitir»; tras el resumen, `N3_EscenaFinal` tampoco lo
   ofrece. ‹PF-RF06-04 [PD: INC-51]›
10. `Revisar-Log S-OMI`.

| Caso | Req · trazas | Etq | Pasos | Aprueba si |
|---|---|---|---|---|
| PF-RF06-02 | RF-06 · HU-02 FA-01/02, CU-03 2a | BOT | 2, 7 | En un nivel ya terminado, cada narrativa ofrece un «Omitir» visible que la salta |
| PF-RF06-03 | RF-06 · HU-14 FA-01 | BOT | 5 | Al repetir un nivel, omitir el cierre lleva al resumen |
| PF-RF06-04 | RF-06 · INC-51 | BOT | 8, 9 | La 3.2 es omitible al repetir; el cruce y la escena final no. [PD: INC-51] |
| PF-RF20-02 | RF-20 | RETO | 4 | Un doble clic en «Soplar» resuelve el nivel una sola vez, sin duplicar la narrativa ni dejar excepciones en el log. ⚠ |
| PF-RF21-02 | RF-21 (Baja) · guion §1.4.3 E7 | RETO | 3 | La iluminación completa llega con el soplido, no antes. ⚠ |
| PF-RF45-04 | RF-45 · OE1 §3.6.1 nota 4 | DAT | 6 | Repetir un nivel no borra ni reescribe los indicadores guardados |
| PF-RF45-05 | RF-45, RF-12 | NAV | 5 | Un doble clic al final de un cierre reflexivo lleva al resumen y no se lo salta |

---

## S-PER · Persistencia, cierre forzado y robustez — T15 · EXE

Cada bloque empieza con su `Preparar-Datos` y termina con `Revisar-Log`.

**PF-RNF14-01 · N2 en medio del nivel.** `OE4_B` → «Nivel 2» → bosque completo → «Empujar» →
durante `N2_Escena22`, `Matar`. Relanzar → «OE4_B» → «Nivel 2» → tres narrativas → **entra directo
al taller** (no al bosque). El JSON conserva `{2,1}`.

**PF-RNF14-02 · N3 en el ensamblaje.** `OE4_E` → «Nivel 3» → cuatro narrativas → ensamblaje en
mástil y vela, con base y amarres armados. Poner una pieza → `Matar`. Relanzar → «Nivel 3» →
vuelve al ensamblaje en mástil y vela, con base y amarres, las tareas 1–3 marcadas y tela y mástil
en el inventario.

**PF-RNF14-03 · ⚠ N1 durante su cierre.** Perfil nuevo «OE4N1b» → N1 hasta «Soplar» → durante
`N1_NacimientoDelFuego`, `Matar`. Relanzar → «OE4N1b». Esperado (RF-04, «guardar… al completar
cada fase»): la fase del N1 está guardada y el N2 se puede jugar. Riesgo: el N1 se pierde entero.

**PF-RNF14-04 · ⚠ N2 durante su cierre.** `OE4_B3` → «Nivel 2» → laberinto resuelto → durante
`N2_Escena25_Cierre`, `Matar`. Relanzar. Esperado (RF-03, «habilitar… cuando el nivel anterior haya
sido completado»): las tres fases del N2 están en el JSON y el N3 se puede jugar. Riesgo: el N3
sigue bloqueado y el N2 se rejuega desde el bosque.

**PF-RNF14-05 · Con la pausa abierta** (HU-17 FA-05). `OE4_B2` → taller → mecanizar A → pausa
abierta → `Matar`. Relanzar → «Nivel 2» → el taller empieza de cero (la fase 2 no estaba
confirmada) y `{2,1}` sigue intacta.

**PF-RF02-04 · JSON corrupto.** `Preparar-Datos OE4_B OE4_Corrupto` → «Jugar» → la lista muestra
los dos → clic en «OE4_Corrupto». Esperado: el juego sigue operable: no se cierra, se puede elegir
«OE4_B» y jugar, y el corrupto se puede borrar con su papelera. Una `Exception` en el log es un DEF
Menor aunque el juego siga.

**PF-RF02-05 · Nombre reservado de Windows.** «Jugar» → `Escribir "CON"` → «Continuar». Esperado:
o se rechaza con «Ese nombre no se puede usar…», o se crea un perfil que funciona (se elige,
guarda y aparece en `Datos/`). En ningún caso se cierra el juego ni queda un perfil inservible.

| Caso | Req · trazas | Etq | Aprueba si |
|---|---|---|---|
| PF-RNF14-01 | RNF-14 · HU-18 FA-05 | DAT | Tras un cierre forzado, el N2 retoma en la primera fase pendiente |
| PF-RNF14-02 | RNF-14 | DAT | Tras un cierre forzado, el N3 retoma en la fase pendiente con lo aprobado armado |
| PF-RNF14-03 | RNF-14, RF-04 | DAT | Un cierre durante la narrativa de cierre del N1 no pierde el nivel completado. ⚠ |
| PF-RNF14-04 | RNF-14, RF-03 | DAT | Un cierre durante la narrativa de cierre del N2 no deja el N3 bloqueado. ⚠ |
| PF-RNF14-05 | RNF-14 · HU-17 FA-05 | DAT | Un cierre con la pausa abierta retoma desde la última fase confirmada |
| PF-RF02-04 | RF-02, RNF-13 | DAT | Un perfil ilegible no deja el juego inservible |
| PF-RF02-05 | RF-02 | DAT | Un nombre reservado del sistema no rompe nada |

---

## S-DOC · Informe docente, borrado y residuos — T16 · EXE + INSP

**Pre:** `Preparar-Datos evidencias/S-N1_OE4N1.json evidencias/S-N2_OE4_B.json
evidencias/S-N3_OE4_C.json OE4_Z`. Anotar el `LastWriteTime` de los cuatro.
**Cifras de `OE4_Z`:** N1F1 7/3/3/«2:05» · N2F1 4/2/0/«1:01» · N2F2 5/3/6/«1:35» · N2F3
2/5/12/«2:20» · N3F1 1/1/1/«0:40» · N3F2 2/1/2/«1:10» · N3F3 3/2/3/«1:40».

1. «Progreso del equipo» → `Medir-Carga` → «Progreso del equipo»: la lista con los cuatro nombres,
   «Eliminar datos» y la tabla con «Nivel», «Fase», «Intentos», «Errores corregidos», «Pasos
   utilizados», «Tiempo de resolución», más «Volver al menú». ‹PF-RF46-01, PF-RNF04-01›
2. «OE4_Z» → siete filas («Nivel 1 · Fase 1» … «Nivel 3 · Fase 3») con **exactamente** las cifras
   de la semilla y el tiempo en `m:ss`. ‹PF-RF46-01›
3. «OE4N1» → N1F1 = 5/1/3 y el tiempo de S-N1; las otras seis filas dicen «Sin datos», nunca 0.
   ‹PF-RF46-02›
4. «OE4_B» → N1F1 7/3/3/«2:05» (semilla), N2F1 3/1/0, N2F2 5/3/6, N2F3 lo previsto en S-N2C, y el
   N3 «Sin datos». Observación: la definición implementada de «errores corregidos» en el
   laberinto (toda edición) es más amplia que la de OE1 §3.6.1 («bloques retirados o
   reordenados»). ‹PF-RF46-02›
5. «OE4_C» → N3F1 1/1/1, N3F2 4/1/2, N3F3 6/3/3, con los tiempos de S-N3. Observación para
   Santiago: las cifras del N3 están acumuladas entre fases, y RF-45 dice «por cada fase».
   ‹PF-RF46-02›
6. «Volver al menú» → pantalla de inicio; ningún JSON cambió su `LastWriteTime` (el informe es de
   solo lectura). ‹PF-RF46-01›
7. Borrado desde el panel: «Jugar» → papelera de «OE4N1» → «¿Borras el perfil de OE4N1?» y «Su
   avance se pierde y no se puede recuperar.», con «Conservar» y «Borrar» → «Conservar» → el perfil
   sigue en la lista y en disco. Papelera otra vez → «Borrar» → desaparece de la lista y
   `Datos/OE4N1.json` ya no existe. ‹PF-RF47-01›
8. Borrado desde el informe: «Progreso del equipo» → «OE4_C» → «Eliminar datos» → «¿Eliminas
   definitivamente los datos de OE4_C? Esta acción no se puede deshacer.», con «Cancelar» y
   «Eliminar datos» → «Cancelar» → nada cambia. Otra vez → «Eliminar datos» → desaparece de la
   lista y del disco, y la tabla pasa a otro perfil. ‹PF-RF47-02›
9. Residuos: con el juego cerrado, buscar «OE4N1» y «OE4_C» en `Build/Algoritmia/` entero, en
   `%USERPROFILE%\AppData\LocalLow\DefaultCompany\My project\` (incluido `Player.log`), en `%TEMP%`
   y en el registro `HKCU:\Software\DefaultCompany\My project` → cero apariciones. Anotar como
   observación de portabilidad lo que el juego deja fuera de su carpeta (Player.log, claves de
   pantalla en el registro). ‹PF-RNF11-01›
10. Borrar los perfiles restantes desde el informe → «Todavía no hay perfiles registrados en este
    equipo.» y «Volver al menú». ‹PF-RF46-03, PF-RF47-02›
11. Ruta de respaldo (INC-34): con el juego cerrado, mover `Datos/` aparte y crear en su lugar un
    **archivo** vacío llamado `Datos`, sin extensión. `Start-Juego` → crear «OE4R» → el perfil queda
    en `%USERPROFILE%\AppData\LocalLow\DefaultCompany\My project\OE4R.json` (anotar si el juego
    avisa dónde quedó, como pide HU-18 FA-04). El informe lo lista; borrarlo desde el informe → el
    archivo desaparece de la ruta de respaldo. Cerrar, borrar el archivo `Datos` y devolver la
    carpeta. ‹PF-RF47-03›
12. INSP: todos los JSON de `evidencias/` → solo `name`, `reachedLevel` y `phases[level, phase,
    attempts, correctedErrors, stepsUsed, resolutionSeconds]`; ningún otro dato. ‹PF-RNF09-01›
13. `Revisar-Log S-DOC`.

| Caso | Req · trazas | Etq | Pasos | Aprueba si |
|---|---|---|---|---|
| PF-RF46-01 | RF-46 · CU-11, HU-16, INC-35 | DAT | 1, 2, 6 | Desde el menú se consulta cada perfil con los cuatro indicadores por nivel y por fase, sin alterar nada |
| PF-RF46-02 | RF-46, RF-45 · OE1 §3.6.1 | DAT | 3–5 | Las cifras de las partidas guionizadas coinciden con las previstas (tiempos ± 5 s) |
| PF-RF46-03 | RF-46 · CU-11 FA-2a | DAT | 10 | Sin perfiles, lo informa y ofrece volver |
| PF-RF47-01 | RF-47 · HU-16, CU-12 | DAT | 7 | Borrar desde el panel exige confirmación explícita, se puede cancelar y borra de la lista y del disco |
| PF-RF47-02 | RF-47 · HU-16 | DAT | 8, 10 | Lo mismo desde el informe docente, advirtiendo la irreversibilidad |
| PF-RF47-03 | RF-47 · INC-34 | DAT | 11 | Con `Datos/` no escribible, el perfil cae en la ruta de respaldo y se borra de ahí |
| PF-RNF11-01 | RNF-11 | DAT | 9 | Tras borrar no queda rastro del perfil en ningún almacenamiento local |
| PF-RNF09-01 | RNF-09 · INC-27 | DAT | 12 | Lo persistido es solo nombre, progreso e indicadores |

---

## S-RNF · Medidas sobre el ejecutable — T20 · EXE + INSP

Se consolidan de lo que dejaron las sesiones: `mem.csv`, `red.csv` y los tiempos de `Medir-Carga`.
Lo que falte se completa aquí.

| Caso | Req | Etq | Cómo | Aprueba si |
|---|---|---|---|---|
| PF-RNF04-01 | RNF-04 | RNF | Tabla con las 12 escenas: Boot→MainMenu desde el arranque, MainMenu, LevelSelect, Credits, Narrative, LevelSummary, TeacherReport y las cinco jugables; **3 mediciones** de cada una en el equipo 1 (EXE) y las del equipo 2 (HUM, cronómetro) | El peor valor de cada escena es < 10 s |
| PF-RNF05-01 | RNF-05 | RNF | Máximos de `WorkingSet64` y `PrivateMemorySize64` de todas las sesiones, con la escena en la que se dieron; en el equipo 2, el Administrador de tareas | Ambos < 2048 MB |
| PF-RNF06-01 | RNF-06 | RNF | Tamaño de `Build/Algoritmia` sin `…_DoNotShip` ni `Datos/`, con desglose por carpeta; se anota también el tamaño con `_DoNotShip` | < 500 MB |
| PF-RNF07-01 | RNF-07 · CT-03 | RNF | Equipo 1: copiar la carpeta a `%USERPROFILE%\Desktop\Algoritmia prueba\` (ruta con espacio, fuera del repo) y lanzarla como usuario estándar, sin «Ejecutar como administrador» → arranca, crea `Datos/` junto al exe y se juega hasta la cueva; no pide instalar nada | Todo lo anterior. **RNF-07 exige también PF-RNF07-02** |
| PF-RNF08-01 | RNF-08 | RNF | Santiago activa el modo avión (HUM). Claude corre un recorrido corto: arranque, perfil nuevo, N1 hasta la cueva, pausa, menú e informe docente | Funciona entero y sin excepciones en el log |
| PF-RNF10-01 | RNF-10 | RNF | `red.csv` de todas las sesiones, más INSP de `rc1`: `UnityConnectSettings` (`m_Enabled`), analítica, `scriptingDefineSymbols` (¿`SENTIS_ANALYTICS_ENABLED`?) y presencia de DirectML.dll y `D3D12/` en el paquete | Cero conexiones TCP y UDP a direcciones que no sean de loopback. Lo de INSP se anota como observación o riesgo |

---

## S-INSP · Inspecciones y accesibilidad — T06 / T17 · INSP / EDIT / SUITE

| Caso | Req | Etq | Ejec. | Cómo | Aprueba si |
|---|---|---|---|---|---|
| PF-RNF01-01 | RNF-01 · CP-08 | RNF | INSP | Script de un solo uso (scratchpad) que extrae todo texto visible al estudiante: líneas de las 18 `NarrativeSequence`, mensajes, guías, resúmenes, rótulos de escenas y prefabs. Parte en oraciones (`. ! ? …`), cuenta palabras y lista las de más de 20. Revisa que cada «iterar», «abstraer», «algoritmo», «descomponer» y «depurar» llegue explicado en su contexto | Ninguna oración de más de 20 palabras en lo que ve el estudiante (el informe docente no cuenta) y ningún tecnicismo sin explicar |
| PF-RNF02-01 | RNF-02 · CT-06 | BOT | EXE + SUITE | Los pasos de teclas de control negativo de las cinco escenas, más `Controls_RNF02_*` y `RiverScene_INC01_*` en la SUITE | En las cinco escenas jugables, ninguna tecla ni la rueda hacen nada. Observación: créditos e informe docente se desplazan con rueda o arrastre |
| PF-RNF03-01 | RNF-03 · CP-08 | RNF | INSP | Sobre las capturas de cada fase de las sesiones S-N1 a S-N3 | En cada fase hay una sola tarea activa; la lista del N3 muestra cuatro, pero una sola en curso |
| PF-RNF15-01 | RNF-15 | RNF | INSP | Muestra de 10 clases: `GameFlow`, `GameFlowRunner`, `SaveStore`, `PlayerProfile`, `FirePanelController`, `ForestSceneController`, `WorkshopSceneController`, `MazeSceneController`, `AssemblyPanelController`, `NarrativeSceneController`. Se revisan identificadores en inglés, PascalCase en tipos y miembros públicos, `_camelCase` en campos privados, y un `<summary>` que diga la responsabilidad de la clase | Las 10 cumplen |
| PF-RNF16-01 | RNF-16 · CT-04 | RNF | SUITE | `git worktree add ../Algoritmia-sinN2 oe4-rc1`. En el worktree: borrar `Assets/Game/Scripts/Runtime/Levels/Wheel/`, `Assets/Tests/*/Levels/Wheel/` y `Assets/Tests/EditMode/Content/` (el único assembly que ve los tres niveles), y quitar las tres `Level2_*` de Build Settings con un script de editor efímero. Correr `unity test` EditMode y PlayMode sobre esa ruta (comprobar el parámetro de proyecto con `unity test --help`). La primera importación tarda. Al final, `git worktree remove` | Compila sin tocar ningún archivo de los otros niveles y pasan sus pruebas; también `Architecture_RNF16_*` en la SUITE normal |
| PF-RNF17-01 | RNF-17 · CT-11 | RNF | INSP | `git log --no-merges oe4-rc1 -- Assets/` clasificado en: con tarjeta (`Dnn-n`, `Onn-n`), con marcador `<id>`, o sin nada | El 100 % de los commits de código lleva tarjeta. ⚠ se sabe que no |
| PF-RNF18-01 | RNF-18 · CT-05 | RNF | EDIT | Con el Editor abierto: en `N1_Config.asset`, `MinimumEffectiveStrikes` de 3 a 2, y cambiar el texto de una línea de `N1_Hallazgo` (Inspector o `coplay set_property`). Play desde `Boot` con un perfil nuevo. `git status` y `check_compile_errors`. **Revertir** con `git checkout -- <los dos .asset>` y comprobar `git status` limpio | La línea muestra el texto nuevo; «Soplar» se habilita al segundo golpe efectivo; solo cambiaron dos `.asset`, sin ningún `.cs` ni recompilación |
| PF-RNF19-01 | RNF-19 | RNF | INSP | Pasar a escala de grises las capturas de: niveles bloqueados, «Soplar» con candado, «Mecanizar» con candado y «Aún no», los rechazos del bosque, del taller y del laberinto, el bloque detenido, los espacios marcados de la balsa y la lista del N3 | Cada estado se distingue sin color. Observaciones que no son estados de error: el lado de «Girar» solo por color; «Empujar» deshabilitado solo por tinte |
| PF-RNF20-01 | RNF-20 | RNF | INSP | Muestrear el color del texto y del fondo (script con la fórmula de luminancia relativa de WCAG) en capturas de: pantalla de inicio, tablilla del N1 en su estado más oscuro, tablillas del N2 y N3, bloques, cuadro de diálogo, resumen, créditos e informe docente | Todas ≥ 4,5:1 |
| PF-RNF21-01 | RNF-21 | RNF | INSP + SUITE | Ráfagas de S-N1 paso 21 (llama e ignición): luminancia media por cuadro; contar oscilaciones de ida y vuelta de más del 10 % dentro de cada ventana de 1 s. Más `*_RNF21_*` de N2 y N3 en la SUITE. El «¡CLIC!» de `N1_Hallazgo` es un destello único | Menos de 3 destellos por segundo (umbral de WCAG 2.3.1) |
| PF-RNF22-01 | RNF-22 | RNF | INSP + HUM | Textos de PF-RNF01-01 sin «http», «www», «.com», precios ni llamadas a comprar; revisión de las ilustraciones vistas en las sesiones y de `Assets/Game/Art/Environments` | Sin violencia explícita, publicidad, compras ni enlaces. Santiago lo confirma |
| PF-RNF23-01 | RNF-23 · CT-09 | RNF | INSP + HUM | Créditos (S-INI paso 4) más el soporte documental que aporte Santiago: la autorización escrita de los autores de la Familia Anonaky y la de la colaboradora de arte | Los créditos reconocen la autoría y existe el soporte. Observación: los recursos sonoros no se nombran (punto PS abierto) |
| PF-RNF12-01 | RNF-12 | RNF | HUM | Documental: el formato de consentimiento informado en los anexos del trabajo de grado | Existe. Si el OE4 no usa estudiantes: **NA**, con esa justificación escrita |

---

## S-HUM · Pruebas con personas y en otros equipos — T19 · HUM

Claude prepara la hoja en T18 y registra lo que Santiago reporte; no completa nada de su cosecha.

| Caso | Req | Etq | Qué hace Santiago | Aprueba si |
|---|---|---|---|---|
| PF-RNF13-01 | RNF-13 · CT-08 | NAV | **Dos recorridos completos**, cada uno con un perfil nuevo, a **pantalla completa** (como se entrega), de la pantalla de inicio a los créditos, sin «Omitir», con cronómetro y anotando cualquier incidencia | Los dos terminan sin bloqueo, cierre inesperado ni estado irrecuperable. La duración (referencia OE1 §2.3: 20–40 min) se registra; no es criterio de aprobación |
| PF-SON-01 | KPI OE4 · CP-02 · Dir. de sonido §19 | SON | En el primer recorrido, marcar en la hoja cada pieza **cableada** del N1 (la lista sale de §19 en T18: solo lo que un asset referencia) | Todas suenan en su momento y ningún fallo suena a castigo |
| PF-SON-02 | ídem | SON | Ídem para el N2 | ídem |
| PF-SON-03 | ídem | SON | Ídem para el N3, más menús y narrativas | ídem |
| PF-RNF07-02 | RNF-07 · CT-03 | RNF | Equipo 2 (Windows 10+ de 64 bits): copiar la carpeta portable por USB, abrirla sin instalar ni pedir administrador, comprobar que crea `Datos/`, jugar el N1, cronometrar tres cargas (Boot→MainMenu, MainMenu→narrativa, narrativa→`Level1_Cave`) y leer la memoria en el Administrador de tareas | Funciona sin instalación ni privilegios; las cifras entran en PF-RNF04-01 y PF-RNF05-01 |
| PF-CT02-01 | CT-02 | RNF | En un equipo **sin tarjeta gráfica dedicada** (puede ser el equipo 2), lo mismo que PF-RNF07-02 | Funciona dentro de los presupuestos de carga y memoria |
