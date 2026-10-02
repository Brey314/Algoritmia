# Hoja HUM · lo que ejecuta Santiago (decisión D-b)

> **Hoja del 30/09/2026, ampliada el 01/10/2026** (tarea T18). Un guion por cada casilla que D-b
> deja en manos de Santiago, el cronometraje del Golden Path y las comprobaciones de vista y oído
> que pidieron las revisiones del Nivel 3 (H6, H7 y H9 a H12), más lo que el arnés no puede ver con
> la mano (H13, revisión final de rc2). Cada guion trae precondición, pasos,
> qué observar y la plantilla que se devuelve. Claude marca la casilla con lo que diga la plantilla
> y **no completa nada que no esté en ella** (OE4 `plan.md` §4, ejecutor HUM). Se imprime: cada
> comprobación tiene su casilla y su campo de nota.

| # | Comprobación | Caso | Cierra | Hace falta |
|---|---|---|---|---|
| H1 | PG-05 · el cambio de controles entre niveles no confunde | guion §1.2 PG-05 | S2 «PG-05 verificado: el paso del panel del N1 al arrastre del N2» y «PG-05 · cambio de esquema de control N1 → N2» · S3 «PG-05 verificado sobre los tres niveles» | estudiantes de cuarto con consentimiento (RNF-12) |
| H2 | PG-06 · los valores del Nivel 1, validados jugando | guion §1.2 PG-06 | S1 «PG-06 · validar jugando los valores de `FireLevelConfig`» | la misma sesión de H1 |
| H3 | RNF-07 · la carpeta portable en un segundo equipo | PF-RNF07-02 | S1 «Ejecución portable con el adaptador de red deshabilitado» (con H4) · S4 «P12 · Presupuestos y ejecución portable», columna «Equipo 2» | Windows 10/11 de 64 bits, cuenta estándar |
| H4 | RNF-08 · sin conexión a internet | PF-RNF08-01 | S1 «Ejecución portable con el adaptador de red deshabilitado» (con H3) | poder quitar la red |
| H5 | CT-02 · un equipo sin tarjeta gráfica dedicada | PF-CT02-01 | S4 «CT-02: el ejecutable corre en un equipo sin tarjeta gráfica dedicada» | equipo con gráfica integrada |
| H6 | Oído · la escena final: fogata y no cueva, atardecer y no mediodía | PF-SON-03 | ninguna (prueba manual de D10-3) | parlantes o audífonos |
| H7 | Oído · el hundimiento de la balsa y la doble salpicadura | PF-SON-03 | ninguna (prueba manual de D10-3) | ídem |
| H8 | Golden Path con cronómetro, 20–40 min | PF-RNF13-01 | S3 «Golden Path del juego entero … en 20–40 minutos» · fila «Golden Path completo» de la tabla de P12 (S4) | perfil nuevo, pantalla completa |
| H9 | Vista · el cruce 3.3 con lectura rápida | S-N3 paso 21 | ninguna (prueba manual de D10-3, riesgo R-5) | la sesión B |
| H10 | Vista · la recolección jugada con el plano abierto | S-N3 pasos 1–10 | ninguna (D10-3, lectura B; visto bueno D-a) | ídem |
| H11 | «Probar balsa» con la base completa y bien puesta | PF-RF42-02 | ninguna (D10-3, decisión D-j) | ídem |
| H12 | Oído · lo que suena, pieza por pieza | PF-SON-01, 02 y 03 | ninguna (KPI de sonido del OE4) | parlantes o audífonos |
| H13 | Mano · lo que el arnés no ve: el clic tras cargar una escena, la papelera del laberinto y la 8.ª fila del informe | PF-RNF02-01, PF-RF34-01, PF-RF46-01 | ninguna (revisión final de rc2, decisión D-OBS) | un *touchpad* si lo hay; los perfiles semilla para el paso 4 |

**Dos sesiones bastan.**

- **A · En el colegio, con estudiantes:** H1 y H2 a la vez (el Nivel 1 que se observa para H1 es
  el de H2).
- **B · Tú solo, un recorrido completo:** en un computador del colegio o en el equipo 2, si su
  única gráfica es integrada, con la red quitada y el medidor abierto. Un solo recorrido cierra
  H3, H4, H5 y H8, con H6, H7 y H9 a H13 por el camino (≈ 1 h 20 min con la preparación). Si el
  equipo 2 tiene tarjeta dedicada, H5 se repite en otro equipo con los pasos de H3.
  Orden: **B1** preparar (H3 pasos 1–3, H5 precondición, H4 pasos 1–2, la hoja de H12 impresa) ·
  **B2** arranque cronometrado (H3 pasos 4–5) · **B3** recorrido de H8, marcando H12 todo el
  tiempo, con H13 paso 1 en el primer cambio de escena, H6 paso 1 al empezar el Nivel 2, H13 pasos
  2–3 en el laberinto, H10 en la orilla, H7 y H11 en la balsa, H9 en la 3.3 y H6 pasos 2–3 en la
  escena final · **B4** «Progreso del equipo» (también H11 paso 4), «Salir» · **B5** guardar
  archivos (H3 paso 7, H4 paso 5), después H13 paso 4, que pone otros perfiles en `Datos\` y vuelve a
  abrir el juego, y al final reconectar y limpiar (H3 paso 8, H4 paso 6) · **B6** plantillas H3 a
  H13.

## Para todos los guiones

1. **Versión.** La carpeta `Build\Algoritmia\` del ejecutable candidato vigente, **rc2**, descrito en
   `claudeDocs/tasks/OE4/evidencias/build-rc2.md` (o el que lo sustituya, si `OE4-Resultados.md` nombra
   otro). No hay etiqueta de git: la procedencia la fijan ese documento y sus huellas. Se copia entera,
   **con** la carpeta `Licencias\` que va junto al `.exe` —`OFL.txt` de las tipografías y
   `LICENSE-Phosphor.txt` de los iconos de pausa: las dos licencias piden que el aviso viaje con cada
   copia que se entrega—, **sin** `Datos\` y **sin** la carpeta `Algoritmia_BurstDebugInformation_DoNotShip`,
   que el build deja junto al `.exe` y no es parte del juego. Ojo: junto a ella hay tres builds
   apartados (`Algoritmia_rc1`, `Algoritmia_2026-10-01_827MB` y `Algoritmia_2026-09-21`) que no se usan.
   **Lo mismo vale al empaquetar la entrega:** la copia que se entrega al colegio no lleva `Datos\` ni
   ninguna carpeta `…_DoNotShip` (`OE4-Resultados.md` §8.9).
2. **Huella.** En PowerShell, dentro de la carpeta: `(Get-FileHash .\Algoritmia.exe).Hash.Substring(0,8)`
   debe dar `6CD16EDF`. El `.exe` es solo el lanzador y su huella no cambia de un build a otro, así
   que compara también el tamaño de la carpeta sin `Datos\` ni `…_DoNotShip` (Propiedades → «Tamaño»):
   478 987 619 bytes (456 MB en el Explorador). Si algo no coincide con `build-rc2.md`, la sesión no
   cuenta (OE4 `plan.md` §2).
3. **Nunca dentro de OneDrive** ni de otra carpeta que se sincronice (Escritorio y Documentos
   suelen estarlo): `Datos\` subiría a internet, y el consentimiento promete lo contrario.
4. **Player.log** vive en `%USERPROFILE%\AppData\LocalLow\Universidad Catolica de Colombia\Algoritmia\`
   y se reemplaza cada vez que el juego se abre (el anterior queda como `Player-prev.log`).
   Cópialo al cerrar el juego, antes de volver a abrirlo. No hace falta que lo leas: lo lee Claude.
5. **Cargas.** En el ejecutable candidato cada carga deja en Player.log la línea
   `RNF-04: «<escena>» cargó en <s> s`, así que solo se cronometra el arranque (doble clic →
   pantalla de inicio; `Boot` no deja línea). Si el registro no trae esas líneas, cronometra las
   cargas que pida el guion.
6. **Memoria.** El medidor del anexo A o el Administrador de tareas.
7. **Si algo falla, no lo arregles:** hora, pantalla y qué pasó; guarda Player.log y sigue si se
   puede. Cada fallo abre un DEF (OE4 `plan.md` §6).
8. **Qué devuelves:** la plantilla del guion (pegada en el chat o en un `.txt`) y los archivos que
   pida, en una carpeta `HUM_<fecha>`. Claude los pasa a `OE4/evidencias/` (los `.log` como
   `.txt`, porque `.gitignore` excluye `*.log`).

---

## H1 · PG-05 · el cambio de controles entre niveles no confunde

**Cierra** S2 «PG-05 verificado: el paso del panel del N1 al arrastre del N2» (Checkpoint W-F) y «PG-05 · cambio de esquema de control N1 → N2» (bloqueante PG-05), y S3 «PG-05 verificado sobre los tres niveles»
(Checkpoint R-E). **Fuente:** guion §1.2, PG-05: «Verificar durante las pruebas que el cambio de
esquema no genera confusión en el paso del nivel 1 al 2». Su estado en el `.docx` lo cambias tú
(no está en D-o).

**Precondición**
- Consentimiento del acudiente **y** asentimiento del estudiante, firmados
  (`Consentimiento-RNF12.md`). Sin los dos, ese estudiante no juega.
- Visto bueno del Colegio [por completar]. La docente del curso presente: abre el juego y elige
  el perfil (OE1 §2.5).
- En cada computador, la carpeta del juego con `Datos\` vacía y el sonido a volumen de clase.
- Un observador por cada dos o tres estudiantes: lo que importa pasa en el primer minuto de cada
  escena. Cuadro con códigos E1, E2…, sin nombres ni fotos de los niños.

**Pasos**
1. El perfil se crea con el código del estudiante (E1, E2…) como alias: el juego no guarda ningún
   nombre (RF-02, RNF-09).
2. Anota la hora de inicio. El estudiante juega solo, de «Jugar» a los créditos o hasta que acabe
   la clase.
3. En cada momento del cuadro mira el primer minuto y marca **✓** si hizo su primera acción válida
   en menos de 1 min sin ayuda de un adulto (la ayuda y la pista del juego sí valen); **?** si tardó
   más o probó primero un control que no era (arrastrar donde va clic, clic donde hay que sostener)
   y luego acertó solo; **A** si un adulto tuvo que explicarle el control.
4. No des la solución. Si pide ayuda, señala el botón de ayuda del juego; solo si sigue atascado
   2 min, explícale el control y marca A.
5. Al terminar: hora de fin y, si quiere contestar, dos preguntas: «¿Qué fue lo más difícil de
   manejar?» y «¿En qué parte no sabías qué hacer?».
6. Lee los indicadores del Nivel 1 para H2 («Progreso del equipo» → el código). Después borra cada
   perfil (papelera de «¿Quién juega?» o «Eliminar datos») y comprueba que `Datos\` quedó vacía
   (RF-47, RNF-11: lo promete el consentimiento).

| Momento | Control que estrena | E1 | E2 | E3 | E4 | E5 | E6 |
|---|---|---|---|---|---|---|---|
| N1 · reunir | arrastrar hojas y piedras con clic sostenido | | | | | | |
| N1 · encender | dos deslizantes, «Golpear», «Soplar» | | | | | | |
| **N2 · bosque** (el paso de PG-05) | clic para elegir; arrastrar la caja; «Empujar» | | | | | | |
| N2 · taller | clic en un tronco y «Mecanizar»; arrastrar piezas en orden | | | | | | |
| N2 · laberinto | «Bloques» → «Tu secuencia», − + ‹ ›, «Ejecutar» | | | | | | |
| N3 · orilla | sostener las flechas; «Recoger» | | | | | | |
| N3 · balsa | arrastrar a la balsa; «Listo» y «Probar balsa» | | | | | | |
| Inicio–fin · ¿créditos? | | | | | | | |

**Criterio propuesto** (ajústalo antes de la sesión): «no confunde» si no hay ninguna **A**. Una
**?** se anota con el control que probó y no reprueba, porque aprender el control es parte del reto
(CP-02). Cada **A** abre un DEF con el momento y el control.

```
H1 · PG-05 · resultado
Fecha: ___   Lugar: ___   Docente presente: sí/no
Estudiantes: ___ · todos con consentimiento y asentimiento: sí/no
Versión (SHA-256, 8 primeros): ________
Fila del bosque: __ ✓  __ ?  __ A      A en todo el cuadro: __ → momento y control: ___
Controles equivocados que se repitieron: ___
Inicio–fin por estudiante (¿créditos?): E1 ___ · E2 ___ · …
Preguntas finales, en resumen: ___
Perfiles borrados y Datos\ vacía en todos los equipos: sí/no
Veredicto: no confunde / confunde en ___ → propuesta: ___
Adjunto: foto del cuadro (sin nombres)
```

**Claude:** marca las tres casillas con fecha, número de estudiantes y la fila del bosque. Si hay
alguna A, abre el DEF y las deja abiertas hasta que decidas.

---

## H2 · PG-06 · los valores del Nivel 1, validados jugando

**Cierra** S1 «PG-06 · validar jugando los valores de `FireLevelConfig`». **Fuente:** guion §1.2, PG-06: «Ajustarlos tras las primeras pruebas
de juego con estudiantes»; CP-04 (equilibrio entre reto y habilidad). Como en H1, el estado de
PG-06 en el `.docx` lo cambias tú. **Precondición:** la sesión A, con los mismos códigos.

| Valor | Hoy (30/09/2026) | Dónde vive |
|---|---|---|
| Radio del círculo de reunión | del centro a la mitad de «Golpear» (≈ 0,09 del ancho) | escena `Level1_Cave` (`FirePanelController.GatherRadius`): es disposición, no dato de `N1_Config` |
| Fuerza que sirve | muescas 7 y 8, de 0 a 10 | `N1_Config`: `EffectiveForceMin` / `EffectiveForceMax` |
| Cercanía que sirve | solo la muesca 5, de 0 a 10 | `N1_Config`: `EffectiveOverlap` 30 ± `OverlapTolerance` 4 |
| Golpes buenos para «Soplar» | 3 | `N1_Config`: `MinimumEffectiveStrikes` (RF-19) |
| Fallos seguidos antes de la pista | 3 | `N1_Config`: `AttemptsBeforeHint`. El 3 lo fija RF-13: cambiarlo es cambiar un RF radicado |

**Pasos**
1. Durante el Nivel 1 de cada estudiante, marca su columna en el cuadro.
2. Antes del borrado (H1 paso 6): «Progreso del equipo» → su código → «Nivel 1 · Fase 1»:
   intentos, errores corregidos, pasos y tiempo.

| Qué mirar (sí / no) | E1 | E2 | E3 | E4 | E5 | E6 |
|---|---|---|---|---|---|---|
| Soltó piezas fuera del círculo creyendo que ya estaban (radio) | | | | | | |
| Pidió «Pista» para ver el círculo | | | | | | |
| Movió la cercanía con orden, una muesca cada vez | | | | | | |
| Dio con la cercanía 5 sin la pista | | | | | | |
| Le salió la pista de los tres fallos y le sirvió | | | | | | |
| Entendió el golpe demasiado fuerte (la chispa se pasa y se apaga en el aire) | | | | | | |
| Se frustró o quiso dejarlo | | | | | | |
| Indicadores N1: intentos / corregidos / pasos / tiempo | | | | | | |

**Cómo decidir cada valor:** se queda si la mayoría llega al fuego probando y ajustando; se mueve
si la mayoría se atasca en ese mismo punto, o si casi nadie falla (CP-04). Cambiar un valor de
`N1_Config.asset` no exige recompilar (RNF-18): Claude lo aplica, corre las pruebas y actualiza
`casos.md` S-N1, que fija la solución en fuerza 7–8 y cercanía 5.

```
H2 · PG-06 · resultado
Fecha: ___   Estudiantes: ___ (los de H1)
Indicadores N1 (I / C / P / tiempo): E1 ___ · E2 ___ · …
Radio del círculo:     se queda / más grande / más pequeño · por qué: ___
Fuerza 7–8:            se queda / ampliar / mover a ___ · por qué: ___
Cercanía solo 5:       se queda / ampliar a ___ · por qué: ___
3 golpes para soplar:  se queda / cambiar a ___ · por qué: ___
Pista a los 3 fallos:  bien / proponer cambio de RF-13
Veredicto: los valores se quedan / se ajustan (cuáles y a qué)
```

**Claude:** marca esa casilla con la decisión. Si hay ajuste, lo hace como tarjeta aparte, con la
prueba primero.

---

## H3 · RNF-07 · la carpeta portable en un segundo equipo

**Cierra** S1 «Ejecución portable con el adaptador de red deshabilitado» con H4, y la columna «Equipo 2» de la tabla de P12 (S4 «P12 · Presupuestos y ejecución portable»;
la del equipo 1 ya está medida). **Caso:** PF-RNF07-02. **Criterio OE1:** «Ejecución
exitosa desde carpeta portable en al menos dos equipos distintos»; el otro es el equipo 1
(PF-RNF07-01, Claude).

**Precondición**
- Equipo 2 con Windows 10 u 11 de 64 bits (Configuración → Sistema → Información o «Acerca de» →
  «Tipo de sistema»).
- Cuenta estándar: en Configuración → Cuentas → Tu información no dice «Administrador». Si solo hay
  cuenta de administrador, vale que no salga el aviso de Control de cuentas; se anota.
- La carpeta del juego en una memoria USB (preámbulo, punto 1).

**Pasos**
1. Anota procesador, RAM, gráfica (Administrador de tareas → Rendimiento) y resolución de pantalla.
2. Copia la carpeta a `C:\Users\<tu usuario>\Algoritmia prueba\`: ruta con espacio y fuera de
   OneDrive. Comprueba que no trae `Datos\`, que trae `Licencias\` junto al `.exe` y anota su
   tamaño (Propiedades → «Tamaño»). Saca la huella (preámbulo, punto 2).
3. Abre PowerShell y pega el medidor (anexo A): se queda esperando a que abras el juego.
4. Doble clic en `Algoritmia.exe`, sin «Ejecutar como administrador», con el cronómetro en marcha;
   páralo al ver la pantalla de inicio.
5. En la carpeta debe haber nacido `Datos\` junto al `.exe`.
6. «Jugar» → perfil nuevo `Equipo2` → el recorrido completo de H8 (cierra los dos); al final,
   «Progreso del equipo» y «Volver al menú». Con solo el Nivel 1, la columna queda a medias.
7. «Salir» → «Cerrar el juego». Guarda Player.log y `Datos\Equipo2.json`; anota lo que imprime el
   medidor.
8. Limpieza: borra `Algoritmia prueba`. Fuera quedan los residuos declarados de RNF-11 (la carpeta
   `…\AppData\LocalLow\Universidad Catolica de Colombia\` y la clave
   `HKCU\Software\Universidad Catolica de Colombia\Algoritmia`): bórralos si el equipo no es tuyo. La
   clave lleva, además de los valores de pantalla, un contador y un identificador de sesión del
   reproductor y tres valores `unity_connect.*` que escribe el motor: es DEF-RC2-01, documentado como
   residuo del motor (`OE4-Resultados.md` §8.7), no un fallo de esta prueba.

**Qué observar:** si pidió instalar algo (.NET, Visual C++), permisos, o si salió Control de cuentas
o SmartScreen («Windows protegió su PC»); si `Datos\` nació junto al `.exe` y no en otra parte; si
la imagen llena la pantalla sin deformarse y nada queda cortado o bajo el cuadro de diálogo a esa
resolución (RNF-03); cierres inesperados.
**Aprueba si** arranca y se juega desde la carpeta copiada, en ruta con espacio y con cuenta
estándar, sin instalar nada ni pedir permisos, y `Datos\` nace junto al `.exe`.

```
H3 · RNF-07 · PF-RNF07-02 · resultado
Fecha: ___   Equipo 2: ___ (¿del colegio? sí/no)   Windows 10/11, 64 bits: sí/no
Cuenta: estándar / administrador sin elevar   CPU: ___  RAM: ___ GB  Gráfica: ___  Pantalla: ___×___
SHA-256 (8 primeros): ________   Carpeta: ___ (¿con espacio? sí/no)   Tamaño sin Datos\: ___ MB
¿Licencias\ junto al .exe, con OFL.txt y LICENSE-Phosphor.txt? sí/no
¿Instalar, permisos, Control de cuentas o SmartScreen? no / sí: ___
¿Datos\ nació junto al .exe? sí/no      Doble clic → pantalla de inicio: ___ s
Recorrido: completo / solo N1 / otro: ___
Memoria pico: ___ MB (espacio de trabajo) / ___ MB (privada) · medidor / Admin. de tareas
¿Algo cortado, deformado o tapado? no / sí: ___     Cierres inesperados: no / sí: ___
Devuelvo: Player.log ☐   Equipo2.json ☐
Veredicto: P / F (qué falló)
```

**Claude:** comprueba la huella; llena la columna «Equipo 2» de P12 con las líneas `RNF-04:`, el
cronómetro, el medidor y el tamaño; abre un DEF por cada `Exception`; marca «Ejecución portable con el adaptador de red deshabilitado» cuando H3 y H4
sean P.

---

## H4 · RNF-08 · sin conexión a internet

**Cierra** S1 «Ejecución portable con el adaptador de red deshabilitado» junto con H3. **Caso:** PF-RNF08-01. **Criterio OE1:** «Ejecución
completa con el adaptador de red deshabilitado».
**Precondición:** el juego copiado en el equipo (vale el de H3) y el Player.log anterior ya guardado.

**Pasos**
1. Quita el cable de red y apaga el Wi-Fi; en un portátil basta el modo avión. Deshabilitar el
   adaptador en «Conexiones de red» también vale, pero suele pedir administrador.
2. Comprueba que no hay red: el icono de la barra de tareas dice que no hay conexión y una página
   web no abre.
3. Con la red ya quitada, abre el juego.
4. Recorrido: el completo de H8, que es la lectura estricta de «ejecución completa». Si se hace
   aparte, el mínimo de PF-RNF08-01: perfil nuevo → Nivel 1 hasta la cueva → pausa → «Volver al
   menú de niveles» → «Volver» → «Progreso del equipo» → «Volver al menú» → «Salir» → «Cerrar el juego».
5. Copia Player.log **antes** de reconectar y antes de abrir otra vez el juego.
6. Reconecta la red.

**Qué observar:** ningún aviso de conexión, ninguna espera rara al arrancar o al cargar, ningún
cierre. Claude busca en Player.log excepciones e intentos de conexión.
**Aprueba si** el recorrido termina sin avisos ni cierres y Player.log no trae ninguna `Exception`.

```
H4 · RNF-08 · PF-RNF08-01 · resultado
Fecha: ___   Equipo: 1 / 2 / otro: ___
Sin red por: cable fuera + Wi-Fi apagado / modo avión / adaptador deshabilitado
¿Comprobaste que no había internet antes de abrir el juego? sí/no · cómo: ___
Recorrido: completo / corto de PF-RNF08-01
¿Avisos de conexión, esperas raras o cierres? no / sí: ___
Player.log copiado antes de reconectar: sí/no · archivo: ___
Veredicto: P / F (qué falló)
```

**Claude:** lee Player.log; con H3 y H4 en P marca esa casilla de S1 con fecha, equipo y recorrido.

---

## H5 · CT-02 · un equipo sin tarjeta gráfica dedicada

**Cierra** S4 «CT-02: el ejecutable corre en un equipo sin tarjeta gráfica dedicada». **Caso:** PF-CT02-01. **Criterio:** CT-02, «…en los equipos
disponibles en la institución, sin tarjeta gráfica dedicada», dentro de los presupuestos de RNF-04
(cada carga bajo 10 s) y RNF-05 (memoria bajo 2 GB).

**Precondición**
- Lo ideal es un computador de la sala de sistemas del Colegio, que es el equipo del que habla
  CT-02. Puede ser el equipo 2.
- Administrador de dispositivos → «Adaptadores de pantalla» muestra una sola gráfica, integrada
  (Intel UHD o Iris, AMD Radeon Graphics de un Ryzen). Si aparece también una NVIDIA GeForce o una
  AMD Radeon RX, ese equipo no sirve para H5.

**Pasos:** los de H3, pasos 1 a 7, con el recorrido completo: RNF-04 habla de «cualquier escena».
Si es el equipo y la sesión de H3, no se repite nada; solo se anota lo de abajo.

**Qué observar:** las cargas (Player.log o cronómetro) y el pico de memoria; la fluidez de la llama
del N1, la carretilla del laberinto y el cruce de la balsa (se anota, no es criterio). En
Player.log, `Renderer:` nombra la gráfica con la que corrió el juego, y `Device Type:`, si aparece,
no puede decir `Discrete` (en el equipo 1, con su RTX 5070 Ti, dice `Discrete`).
**Aprueba si** el recorrido se completa, cada carga queda bajo 10 s y el pico de memoria bajo
2048 MB.

```
H5 · CT-02 · PF-CT02-01 · resultado
Fecha: ___   ¿Mismo equipo y sesión que H3? sí (solo lo que falta) / no: ___
Adaptadores de pantalla (todos): ___      ¿Equipo del colegio? sí/no
Recorrido: completo / parcial: ___
Carga más lenta: ___ s en ___ (o «ver Player.log»)      Memoria pico: ___ MB
Fluidez (llama, laberinto, cruce): bien / tirones en ___
Veredicto: P / F (qué falló)
```

**Claude:** comprueba en Player.log la gráfica y cada carga; marca esa casilla con el equipo, la carga
más lenta y el pico.

---

## H6 · Oído · la escena final: fogata y no cueva, atardecer y no mediodía

**Cierra** ninguna casilla de los `todo.md`: es la prueba manual de la tarjeta D10-3 (riesgo R-8 de
su especificación) y alimenta PF-SON-03. **Qué cambió:** `N3_EscenaFinal` suena
`amb_n2_bosque_dia` con `amb_n1_cueva_fuego` encima, el mismo par del arranque del Nivel 2
(`N2_PuenteI`). La escena es la familia caminando al aire libre hacia las fogatas (guion §1.9),
pero `amb_n1_cueva_fuego` se pensó como la cueva más el crepitar (dirección de sonido §8): puede
traer eco o goteo. Y el fondo es el bosque **de día** —pájaros dispersos, «luminoso y abierto»
(§8)— en una escena pintada **al atardecer**: no hay toma de atardecer ni de noche al aire libre
(`amb_n2_noche_intemperie` está en disco, pero ninguna escena la usa).
**Precondición:** versión con D10-3; audífonos, o los parlantes de la sala a volumen de clase, en
silencio. Sale sola en la sesión B; aparte, con el perfil `OE4_C` del anexo B y el Nivel 3 entero.

**Pasos** (en la sesión B, con el cronómetro de H8 en pausa)
1. Al empezar el Nivel 2 (`N2_PuenteI`, el amanecer junto a la cueva), escucha 20 s: es la
   referencia del mismo par. Aparte, con `OE4_C`: «Nivel 2» abre `N2_PuenteI` con «Omitir»;
   escúchalo, omite hasta el bosque y sal con pausa → «Volver al menú de niveles».
2. En la escena final, deja cada línea unos 15 s antes de «Continuar».
3. Si lo recuerdas, compáralo con el cierre del Nivel 1 (la cueva con la hoguera, que sí debe
   sonar a cueva).

**Qué escuchar:** bosque abierto con fuego que crepita, o interior de cueva (eco largo, goteo); si
los pájaros y el viento de día chocan con la luz de atardecer que se ve; y si se nota que el sonido
se repite cada pocos segundos (los dos bucles duran 3 s y 4,9 s).
**Decide:** se queda, o se pide una toma de fogata sola, sin cueva, o una de atardecer sin pájaros
de día; mientras llegan, vale el par de `N2_PuenteI` (decisión D-k).

```
H6 · oído · escena final · resultado
Fecha: ___   Escuché con: audífonos / parlantes de ___
¿Bosque al aire libre con fogatas? sí/no    ¿Eco, goteo o «interior de cueva»? no / un poco / sí
¿El bosque suena a mediodía frente a la luz de atardecer? no / un poco / sí
¿Se nota la repetición cada pocos segundos? no/sí
Frente al arranque del Nivel 2: igual / peor / mejor
Veredicto: se queda / pedir toma de fogata sola / pedir toma de atardecer / otro: ___
```

**Claude:** lo registra en PF-SON-03; si pides otra toma, queda como pendiente del carril de sonido.

---

## H7 · Oído · el hundimiento de la balsa

**Cierra** ninguna casilla de los `todo.md`: es la prueba manual de D10-3 (decisión D-j, riesgo
R-10) y alimenta PF-SON-03. **Qué cambió:** «Probar balsa» está en las tres fases (junto a «Listo»
en la base y el amarre); toda balsa que se hunde suena `sfx_n3_hundimiento` (1,7 s), y «Listo»
rechazado sigue mudo, porque ningún sonido marca un fallo (dirección de sonido §2.1). Probar antes
de tiempo cuenta como intento, devuelve solo lo mal puesto, no aprueba la fase y no dispara la
escena 3.2.
**Precondición:** versión con D10-3; audífonos o parlantes a volumen de clase. Va en la sesión B;
aparte, con `OE4_C` (anexo B).

**Pasos** (en la sesión B, con el cronómetro de H8 en pausa)
1. En la base, con dos o tres troncos puestos, «Probar balsa»: la balsa se hunde un momento y suena.
2. «Listo» con la base incompleta: no suena nada; solo el mensaje y el espacio marcado.
3. «Probar balsa» dos veces seguidas, la segunda en cuanto la balsa vuelve a flotar (riesgo R-10):
   el hundimiento dura 0,6 s y su toma 1,74 s, así que la segunda salpicadura empieza sobre la cola
   de la primera.
   ¿Molesta el doble sonido?
4. Termina base y amarre. En mástil y vela, cruza la tela y el mástil → «Probar balsa»: se hunde,
   suena y después llega la escena 3.2 (solo esta primera vez).

**Qué escuchar:** madera que cruje y agua, con un final blando, casi cómico (la orilla es baja y
nadie corre peligro, dirección de sonido §13); nada de pitido, zumbido ni acorde triste; frente al
río de fondo se oye sin asustar; el sonido no se corta feo cuando entra la 3.2.
**Decide:** se queda / recortar / bajar el volumen / pedir otra toma. Si molesta el doble sonido,
lo que se toca es la toma (recortarla), no el panel: alargar el hundimiento para que no se pisen
retrasaría la respuesta, y RF-11 la pide en menos de 1 s.

```
H7 · oído · hundimiento · resultado
Fecha: ___   Escuché con: ___
1 Probar antes de tiempo: ¿se hunde y suena? sí/no     2 «Listo» incompleto: ¿calla? sí/no
3 ¿Madera y agua, final blando? sí/no    ¿Suena a castigo (pitido, zumbido, acorde triste)? no/sí
4 Volumen frente al río: bien / bajo / alto · ¿asusta? no/sí
5 Dos pruebas seguidas (R-10): ¿molesta? no/sí      6 Fase 3: ¿suena y la 3.2 entra sin cortes raros? sí/no
Veredicto: se queda / recortar / bajar volumen / otra toma
```

**Claude:** lo registra en PF-SON-03; un «suena a castigo» abre un DEF contra CP-02 y la dirección de sonido §2.1.

---

## H8 · Golden Path con cronómetro (20–40 min)

**Cierra** S3 «Golden Path del juego entero … en 20–40 minutos» y la fila «Golden Path completo» de la tabla de P12 (S4) en la
columna del equipo usado. **Caso:** PF-RNF13-01, en su parte humana; los dos recorridos sin
incidencias los hace el arnés (D-f). **Referencia:** OE1 §2.3, sesiones de 20 a 40 minutos.
**Precondición:** perfil nuevo, nunca jugado; pantalla completa, como se entrega; sonido encendido;
cronómetro con vueltas y pausa (el del celular). Si el juego abre en ventana, ciérralo y ábrelo
desde PowerShell, en su carpeta, con `.\Algoritmia.exe -screen-fullscreen 1`.

**Pasos**
1. Arranca el cronómetro al pulsar «Jugar».
2. Juega a tu ritmo y lee cada línea entera antes de «Continuar», salvo en la 3.3, que se lee
   deprisa a propósito (H9). Con un perfil nuevo no aparece «Omitir»: si aparece, es un hallazgo.
3. Vuelta al ver cada resumen: «Esto es lo que pasó en la cueva», «…con la rueda», «…en el río».
4. Para el cronómetro cuando salgan los créditos. Solo se pausa mientras haces H6, H7, H10, H11,
   los silencios provocados de H12 (su paso 3) y H13 pasos 2–3; H9, H13 paso 1 y el resto de H12
   corren con el cronómetro.
5. Toda incidencia, en el momento: hora, pantalla, qué pasó y si pudiste seguir.
6. «Volver» → (en la sesión B, antes «Progreso del equipo»: H3 paso 6) → «Salir» → «Cerrar el
   juego»; guarda Player.log (en la sesión B es el mismo de H3–H5).

**Qué medir:** el total y los tres parciales. Conoces las soluciones, así que tu tiempo es una cota
baja; el de un estudiante que llegue a los créditos en H1 es la referencia más fiel y va en la
misma plantilla.
**Aprueba si** llega a los créditos sin bloqueos, cierres inesperados ni estados de los que no se
sale (RNF-13). La duración se registra; si cae fuera de 20–40 min, Claude marca esa casilla con la cifra
solo si en la plantilla aceptas el motivo.

```
H8 · Golden Path · PF-RNF13-01 · resultado
Fecha: ___   Equipo: 1 / 2 / otro: ___   Pantalla completa: sí/no   Perfil: ___
Desde «Jugar»: N1 __:__ · N2 __:__ · N3 __:__ · créditos __:__ (total)
¿Leíste cada línea entera, salvo la 3.3 (H9)? sí/no     ¿Apareció «Omitir»? no / sí, en ___
Incidencias (hora · pantalla · qué pasó · ¿seguiste?): ___
Si el total cae fuera de 20–40 min, por qué (¿lo aceptas?): ___
Tiempo de un estudiante en H1, si lo hay: ___ min
Player.log guardado: sí/no
```

---

## H9 · Vista · el cruce 3.3 con lectura rápida

**Cierra** ninguna casilla de los `todo.md`: es la prueba manual del salto del cruce (tarjeta D10-3,
riesgo R-5 de su especificación). **Qué cambió:** la balsa cruza en 9 s y, si se pulsaba «Continuar»
mientras cruzaba, cada paso nuevo cortaba el camino en curso: la Niña y el Niño saltaban a su
destino en la línea 1, Papá en la 2 y los cuatro en la 3. Ahora los cuatro viajeros terminan sus
pasos (`NarrativeProp.FinishesSteps`): lo que se lee mientras viajan lo hacen al llegar, y después
bajan caminando. Con lectura lenta nada cambia, y lo cubren las pruebas. Queda por ver la lectura
rápida: los niños celebran y Papá señala **al llegar la balsa**, no al leerse su línea, y si las
líneas 2 y 3 pasan durante el cruce, Papá va directo a la orilla sin señalar.
**Precondición:** sesión B, al aprobar mástil y vela (fundido a `N3_Escena33_Cruce`; la primera vez
no hay «Omitir»). Aparte, con `OE4_C` (anexo B) y el Nivel 3 entero.

**Pasos** (en la sesión B, con el cronómetro de H8 en marcha)
1. Al abrir la 3.3, pulsa «Continuar» en cuanto se pueda en las cuatro primeras líneas —el
   narrador «La familia sube a la balsa corregida…», el Niño «¡Funciona! ¡Funciona!», Papá «¡Al
   otro lado!» y el narrador «Llegan a la orilla opuesta…»—, sin esperar a que la balsa llegue.
2. Sigue con la vista a los cuatro hasta que estén en el pasto de la otra orilla; después lee a tu
   ritmo las líneas de Algoritm.
3. (Opcional) En el resto del recorrido no se acelera ninguna narrativa (H8 paso 2). Si aun así, al
   pulsar «Continuar» con alguien caminando, ves a ese personaje aparecer de golpe en su destino,
   anótalo: es el mismo salto, que sigue en otras 14 narrativas (pendiente R-6), no un fallo nuevo.

**Qué observar:** que nadie salte ni aparezca de golpe en otro sitio; que la familia siga sobre la
balsa hasta que llega y baje caminando al pasto, no al agua; cuándo celebran la Niña y el Niño y qué
hace Papá; si la escena se entiende así.
**Decide:** basta esta escenificación, o se pide que los niños celebren ya en la balsa, mientras
cruzan. Eso es trabajo nuevo: distinguir «lo llevan» de «camina» con otra marca en `NarrativeProp`,
que no se hizo (R-5).

```
H9 · vista · cruce 3.3 con lectura rápida · resultado
Fecha: ___   Equipo: ___
¿Alguien saltó o apareció de golpe en otro sitio? no / sí: quién y en qué línea ___
¿La familia llegó con la balsa y bajó caminando al pasto? sí/no
Niña y Niño celebran: al llegar la balsa / al leerse su línea / no lo vi
Papá: señala al llegar / va directo a la orilla / otro: ___
¿Se entiende la escena así? sí / no: ___
Veredicto: basta / pedir que celebren ya en la balsa / otro: ___
(Opcional) Saltos vistos en otras narrativas (escena y personaje): ___
```

**Claude:** lo registra en `Slice 3/Slice-3-Resultados.md` y `Personajes/Personajes-Resultados.md`;
un salto en la 3.3 abre un DEF contra RNF-21; «pedir» queda como tarjeta nueva, y los saltos de otras
narrativas van a la decisión de R-6.

---

## H10 · Vista · la recolección jugada con el plano abierto

**Cierra** ninguna casilla de los `todo.md`: es la prueba manual de la lectura B del Nivel 3
(tarjeta D10-3, INC-118), con los tres puntos de escala que la revisión dejó para tu visto bueno
(D-a). **Qué cambió:** la orilla se ve con el plano abierto (×1,4: el río y el pie de la cascada), y
Mamá, la familia y los ocho materiales, a la escala de las narrativas del río, unas 2,3 veces más
grandes. Mamá se ancla por los pies y se dibuja por profundidad con la familia y los materiales: lo
que está más abajo, delante. «Recoger» aparece cuando los pies de Mamá están a 0,06 de un material,
en fracción de la ilustración: a 1920 × 1080, unos 160 px a los lados y 90 px arriba y abajo.
**Precondición:** sesión B, al llegar a la orilla del Nivel 3; aparte, con `OE4_C`. A la resolución
del equipo; la referencia es 1920 × 1080.

**Pasos** (en la sesión B, con el cronómetro de H8 en pausa)
1. Antes de recoger nada, lleva a Mamá con las flechas por el borde de la orilla: abajo a la
   derecha, donde el río hace curva; arriba, hasta el seto; y a la izquierda, junto al inventario.
2. Pasa por delante y por detrás de un material y de la familia.
3. Acércate despacio, desde lejos, a tres materiales y fíjate en cuándo aparece «Recoger»:
   ¿demasiado lejos, justo, o hay que ponerse encima?
4. Entra a la zona marcada sin tener todo (es también el silencio de H12, paso 3) y sal.
5. Recoge el resto y entra a la zona.

**Qué observar**
- Los materiales parecen tirados en el pasto: ninguno flota ni queda en el agua.
- Mamá no pisa el agua en la curva del río ni se mete bajo el inventario o la cruceta.
- Al pasar por delante y por detrás, se dibuja delante lo que está más abajo, sin saltos.
- «Recoger» aparece a una distancia que parece natural.
- **Para tu visto bueno (D-a):** (a) el tronco mide ≈ 0,6 de la altura de Papá en la 3.1, ≈ 1 en
  la orilla y ≈ 1,3 en la balsa de la 3.3: ¿se nota el cambio de tamaño? (b) El mástil está de pie
  en la orilla y tumbado en la 3.1 (`Collectible` no tiene giro: tumbarlo pide código). (c) La
  familia no se escala por profundidad: si Mamá sube junto a ella, se ve ≈ 11 % más baja frente a
  Papá que en la 3.1.

**Dónde viven los valores:** `N3_RiverLevelConfig` (`ProximityRadius` 0,06 para «Recoger» y
`BuildZoneRadius` 0,06 para la zona). Se cambian sin recompilar (RNF-18), pero ningún material puede
quedar a menos de la suma de los dos de la zona, porque la zona se abre al cruzar su borde: lo vigila
`RiverLevelConfig_RF37_ElRepartoObligaARecorrerLaOrillaYCabeEnElPlanoFijo`. El tamaño de los
objetos de la 3.1 está en `N3_Escena31_Llegada` y no puede pasar de 0,076 sin meterse bajo el cuadro
de diálogo en la línea 6.

```
H10 · vista · recolección · resultado
Fecha: ___   Equipo y resolución: ___
¿Algún material flota o queda en el agua? no / sí: ___      ¿Mamá pisa el agua en la curva? no/sí
¿Delante y detrás se ven bien? sí / no: ___
«Recoger» aparece: demasiado lejos / bien / hay que ponerse encima
D-a (a) tamaño del tronco 3.1 → orilla → balsa: no se nota / se nota y no molesta / se nota y molesta
D-a (b) mástil de pie en la orilla: se acepta / pedir tumbado
D-a (c) Mamá junto a la familia: se ve bien / se ve pequeña
Veredicto: radio se queda / más grande / más pequeño · escala: se acepta / pedir ajuste de ___
```

**Claude:** un material en el agua o Mamá en el río abre un DEF; un cambio de radio lo aplica como
tarjeta aparte, con la prueba primero, y actualiza `casos.md` S-N3 (posiciones y elipse de
«Recoger»); lo de D-a lo registra en `Slice 3/Slice-3-Resultados.md` y, si pides ajuste, queda para
el carril de arte.

---

## H11 · «Probar balsa» con la base completa y bien puesta

**Cierra** ninguna casilla de los `todo.md`: es una observación que pidió la revisión del Nivel 3
sobre la decisión D-j (INC-122) y alimenta PF-RF42-02. **Qué hace el juego:** en la base y el amarre,
«Probar balsa» comprueba sin aprobar. Aunque la base esté completa y bien puesta, la balsa se hunde
un momento, suena `sfx_n3_hundimiento`, sale «La balsa se hundió: todavía no está terminada. ¿Qué
parte falta armar?» y la prueba cuenta como intento en el informe docente (RF-45). Solo «Listo»
aprueba la fase (RF-40). Un estudiante podría esperar que probar una base bien puesta la cierre.
**Precondición:** sesión B, en el ensamblaje, después de H7 pasos 1–3; aparte, con `OE4_C`.

**Pasos** (en la sesión B, con el cronómetro de H8 en pausa)
1. Pon los cinco troncos en sus siluetas —la base completa y bien puesta— y pulsa «Probar balsa».
2. Mira y escucha: el hundimiento, el sonido y el mensaje; ningún espacio marcado ni tronco devuelto
   al inventario; la fase sigue abierta (no salen las siluetas de los amarres).
3. Pulsa «Listo»: martillazos, «La base quedó firme…» y las diez siluetas de los amarres.
4. Al final del recorrido (B4): «Progreso del equipo» → el perfil → «Nivel 3 · Fase 1», y mira los
   intentos. Cuentan los «Probar balsa» de la base (los de H7 y este) y los «Listo» rechazados.

**Decide:** se queda —lo fijan D-j y RF-40: «Probar balsa» comprueba y «Listo» consolida— o se
propone que probar una fase bien puesta no la hunda, o que la apruebe. Eso cambia RF-40, RF-42 y
RF-45, que están radicados: se decide aparte, no en la sesión.

```
H11 · «Probar balsa» con la base bien puesta · resultado
Fecha: ___
Base completa y bien puesta → «Probar balsa»: ¿se hundió y sonó? sí/no   ¿«todavía no está terminada»? sí/no
¿Algún espacio marcado o algún tronco devuelto? no / sí: ___      ¿La fase siguió abierta? sí/no
«Listo» después: ¿aprobó la base? sí/no
Informe, «Nivel 3 · Fase 1»: intentos ___ (¿cuentan las pruebas? sí/no)
¿Esperabas que probar la base bien puesta la aprobara? no/sí · ¿Lo esperaría un estudiante? ___
Veredicto: se queda (solo «Listo» aprueba) / proponer cambio: ___
```

**Claude:** lo registra en PF-RF42-02 y en `Slice 3/Slice-3-Resultados.md`; un «proponer cambio»
queda como pregunta para ti, con los RF que toca.

---

## H12 · Oído · lo que suena, pieza por pieza

**Cierra** ninguna casilla de los `todo.md`: es la hoja de PF-SON-01 (Nivel 1), PF-SON-02 (Nivel 2)
y PF-SON-03 (Nivel 3, con menús y narrativas, más H6 y H7), el KPI de sonido del OE4 (`casos.md`).
**Fuente:** la tabla «Lo que suena hoy (01/10/2026)» de `claudeDocs/Direccion_de_Musica_y_Sonido.md`
§19. Solo suena lo que referencia un asset: 18 de las 19 piezas en disco.
**Precondición:** sesión B, sonido a volumen de clase y esta hoja impresa.

**Pasos** (durante el recorrido de H8)
1. Marca cada fila cuando la oigas en su momento: **✓** sonó, **✗** no sonó, **~** sonó en otro
   momento (anota cuál).
2. Los menús, la pausa, los resúmenes y el informe docente no tienen sonido: la música y los efectos
   de interfaz no se han entregado (§19, «Sin entregar»). Que callen no es un fallo.
3. Provoca una vez cada silencio a propósito, con el cronómetro en pausa; los de la balsa salen en H7
   y en H10.
4. `amb_n2_noche_intemperie` está en disco y no lo usa nadie: no debe sonar en ningún sitio.

**Nivel 1 · PF-SON-01**

| Pieza | Debe sonar | ✓ ✗ ~ |
|---|---|---|
| `amb_noche_intemperie` | la apertura, afuera y de noche (`N1_Apertura`) | |
| `sfx_n1_pasos_eco` | apertura, «Avanzan hacia el fondo. Los pasos resuenan contra la piedra.» | |
| `amb_n1_cueva_oscura` | desde esa línea, ya en la cueva; la 1.1, la 1.2, la mecánica y la 1.3, debajo del fuego | |
| `sfx_n1_hojas_acomodo` | 1.2, «Las amontona con cuidado en el suelo.»; en la mecánica, al arrastrar una hoja | |
| `sfx_n1_golpe` | 1.2, «Las golpea una contra otra…»; en la mecánica, cada «Golpear» con las piedras en contacto (el tono varía un poco) | |
| `sfx_n1_chispa` | 1.2, «¡CLIC!»; en la mecánica, cada golpe que hace chispa en las hojas | |
| `sfx_n1_pieza_tomar` | al terminar de reunir, cuando la escena pasa a encender | |
| `sfx_n1_soplo` | «Soplar» | |
| `amb_n1_cueva_fuego` | entra en 3 s al soplar, sobre la cueva, y sigue en la 1.3 | |

**Nivel 2 · PF-SON-02**

| Pieza | Debe sonar | ✓ ✗ ~ |
|---|---|---|
| `amb_n2_bosque_dia` + `amb_n1_cueva_fuego` | arranque del puente I, al amanecer junto a la cueva; el fuego se va en «La familia sale a recolectar» | |
| `amb_n2_bosque_dia` | el puente I en el bosque, de la 2.1 a la 2.4 y en las tres fases (bosque, taller y laberinto), sin costura | |
| `sfx_n2_troncos` | bosque: un tronco que se aparta al pasar el cursor; 2.2, el tronco que suelta el Niño, al caer | |
| `sfx_n2_piedra_cae` | bosque: una piedra o herramienta que se aparta; 2.2, la piedra de Papá y la caja, al tocar el suelo | |
| `sfx_n1_hojas_acomodo` | bosque: una planta que se aparta, mientras vuela | |
| `sfx_encaje_pieza` | bosque: cada tronco acopiado; taller: cada tronco perforado, con «Mecanizar» o con el mazo | |
| `sfx_n1_pieza_tomar` | bosque: los cinco troncos reunidos; taller: la carretilla terminada | |
| `sfx_martillo_madera` | taller: tres golpes cada vez que una pieza encaja en la carretilla | |
| `sfx_n2_carretilla` | laberinto: en bucle mientras la carretilla recorre la secuencia, también si choca y vuelve | |
| `amb_n1_cueva_oscura` + `amb_n1_cueva_fuego` | 2.5, en el refugio con la hoguera | |

**Nivel 3, narrativas y menús · PF-SON-03**

| Pieza | Debe sonar | ✓ ✗ ~ |
|---|---|---|
| `amb_n1_cueva_oscura` + `amb_n1_cueva_fuego` | arranque del puente II y su horizonte, desde la boca de la cueva | |
| `amb_n3_rio_orilla` + `amb_n2_bosque_dia` | el puente II en el río, de la 3.1 a la 3.3, la orilla y el ensamblaje, sin costura | |
| `sfx_encaje_pieza` | orilla: cada material recogido con «Recoger» | |
| `sfx_martillo_madera` | ensamblaje: un golpe por cada pieza que queda puesta en la balsa, también la equivocada; tres seguidos al aprobar una fase | |
| `sfx_n1_pieza_tomar` | ensamblaje: la balsa terminada, tras el último martillazo | |
| `sfx_n3_hundimiento` | cada hundimiento: «Probar balsa» antes de tiempo (H7, H11) y la balsa terminada que no flota | |
| `amb_balsa_movimiento` | 3.3: mientras la balsa cruza (9 s), en lugar del bosque, que vuelve al llegar | |
| `amb_n2_bosque_dia` + `amb_n1_cueva_fuego` | escena final (H6) | |

**Silencios a propósito** (dirección de sonido §2.1: ningún fallo suena)

| Qué haces | Debe | ✓ calla / ✗ suena |
|---|---|---|
| N1: «Golpear» con las piedras separadas en pantalla (cercanía baja) | callar | |
| N3: «Listo» con la fase incompleta (H7 paso 2) | callar, sin hundimiento | |
| N3: «Listo» con una pieza mal puesta, que vuelve al inventario | callar | |
| N3: entrar a la zona sin todos los materiales (H10 paso 4) | callar | |

**Aprueba si** cada pieza suena en su momento y ningún fallo suena a castigo (criterio de PF-SON en
`casos.md`).

```
H12 · oído · PF-SON · resultado
Fecha: ___   Escuché con: ___   Equipo: ___
N1 (PF-SON-01): ✗ ___   ~ ___
N2 (PF-SON-02): ✗ ___   ~ ___
N3, narrativas y menús (PF-SON-03): ✗ ___   ~ ___
Silencios: ¿sonó alguno de los cuatro? no / sí: ___
¿Sonó algo en los menús, o algo que no está en la hoja? no / sí: ___
¿Algún sonido suena a castigo (pitido, zumbido, acorde triste)? no / sí: ___
Veredicto: PF-SON-01 P/F · PF-SON-02 P/F · PF-SON-03 P/F (con H6 y H7)
Adjunto: foto de la hoja marcada
```

**Claude:** registra PF-SON-01 a 03; cada ✗ o ~ abre un DEF contra la fila de §19; un silencio que
suena o un sonido de castigo abre un DEF contra CP-02 y la dirección de sonido §2.1.

---

## H13 · Mano · lo que el arnés no ve

**Cierra** ninguna casilla de los `todo.md`: son cuatro comprobaciones que pidió la revisión final
de rc2 (01/10/2026) y que el arnés no puede hacer bien, porque mueve el puntero un píxel antes de
cada clic («Nudge», `herramientas/oe4-USO.md`) y no tiene *touchpad*. Alimentan PF-RNF02-01,
PF-RF34-01 y PF-RF46-01, y las observaciones de `OE4-Resultados.md` §8.9, que se documentan con su
arreglo propuesto y no se corrigen en este corte (decisión D-OBS).
**Precondición:** sesión B. Los pasos 1 a 3 van durante el recorrido de H8; el paso 4, al final (B5),
con el juego cerrado. Si el equipo es un portátil, usa su *touchpad* en el paso 1.

**Pasos**
1. **El clic tras cargar una escena** (el riesgo de `OE4-Resultados.md` §6). En tres cambios entre
   narrativas encadenadas —en el Nivel 1, de la apertura a la aparición de Algoritm y de esta al
   hallazgo, y uno más donde quieras—, pulsa «Continuar» en la última línea y **no muevas el
   puntero** ni deslices el dedo. Cuando termine el
   fundido y aparezca la escena nueva, vuelve a pulsar en el mismo sitio, sin moverte. Anota cuántas
   pulsaciones hicieron falta en cada cambio. En el spike del arnés, sin mover el ratón, se perdieron
   12 clics seguidos al pasar de una narrativa a otra: cada escena trae su propio `EventSystem`.
2. **Doble clic en la papelera del laberinto** (N3 de la revisión de código), con el cronómetro de H8
   en pausa. Con tres bloques o más en «Tu secuencia», haz doble clic, rápido, sobre la papelera del
   bloque seleccionado. Se retiran dos: tras retirar, la papelera de la fila siguiente queda bajo el
   cursor. Se acepta porque editar no cuesta (CP-02). Vuelve a poner lo que quitaste; esas ediciones
   suman en «errores corregidos» del informe docente.
3. **La papelera tras un fallo** (OBS-rc2-5), también con el cronómetro en pausa. Con una secuencia
   que no quepa entera en la lista (se ven ▲ y ▼), ejecuta una que choque. Fíjate antes de «Ejecutar»
   dónde está la papelera del bloque seleccionado y, tras el fallo, intenta pulsarla ahí sin mirar: la
   lista se recoloca para mostrar el bloque donde se detuvo, y la papelera puede quedar unos 50 px más
   abajo. El intento de más cuenta en el informe; anótalo en H8.
4. **La 8.ª fila del informe** (decisión D-SCR). Con el juego cerrado y lo de H3 y H4 ya guardado,
   saca `Equipo2.json` de `Datos\` y copia ahí los ocho perfiles semilla `OE4_A`, `OE4_B`, `OE4_B2`,
   `OE4_B3`, `OE4_C`, `OE4_D`, `OE4_E` y `OE4_Z` (`claudeDocs/tasks/OE4/herramientas/perfiles/`;
   llévalos en la memoria junto al juego). Abre el juego → «Progreso del equipo». La lista de la
   izquierda tiene ocho filas y la 8.ª sale con el borde inferior recortado hasta que bajas la lista con
   su barra. Elige «OE4_Z»: el encabezado dice «Perfil de OE4_Z». Cierra el juego y borra los ocho
   perfiles: `Datos\` queda vacía.

**Qué observar:** si alguna pulsación se pierde tras cargar una escena, y con qué puntero; si el doble
clic o la papelera que cambia de sitio confunden; si la 8.ª fila se lee y se elige.
**Decide:** cada punto se queda como está o se pide su arreglo; los arreglos propuestos están en
`OE4-Resultados.md` §8.9.

```
H13 · mano · lo que el arnés no ve · resultado
Fecha: ___   Equipo: ___   Puntero: ratón / touchpad
1 Tras cargar una escena sin mover el puntero, ¿se perdió la primera pulsación? no / sí, en __ de 3 cambios
  Pulsaciones que hicieron falta en cada cambio: ___ · ___ · ___
2 Doble clic en la papelera: ¿se fueron dos bloques? sí/no      ¿Molesta? no / sí: ___
3 Papelera tras un fallo: ¿fallaste el primer clic? no/sí      ¿Lo notaría un estudiante? ___
4 Informe con ocho perfiles: ¿8.ª fila recortada? no/sí   ¿Se lee y se elige? sí/no   ¿«Perfil de OE4_Z»? sí/no
Perfiles semilla borrados y Datos\ vacía: sí/no
Veredicto: 1 bien / DEF · 2 se queda / pedir arreglo · 3 se queda / pedir arreglo · 4 se queda / pedir arreglo
```

**Claude:** una pulsación perdida en el paso 1 abre un DEF con la escena y el puntero; lo demás lo
registra en `OE4-Resultados.md` §8.9 y en PF-RF34-01 y PF-RF46-01. Un «pedir arreglo» queda como
tarjeta nueva, con el arreglo de §8.9 y la prueba primero.

---

## Anexo A · medidor de memoria

Antes de abrir el juego, abre **Windows PowerShell** desde el menú Inicio (sin administrador), pega
esto y pulsa Intro. Espera a que el juego arranque, lo sigue mientras juegas y, al cerrarlo,
imprime el pico de memoria y, del registro, las cargas, las excepciones y la gráfica usada. Anota
el pico en la plantilla. Pegar en la consola no choca con la política de ejecución de scripts.
Probado el 30/09/2026 en Windows PowerShell 5.1 y en PowerShell 7, con un proceso y un registro de
muestra.

```powershell
& {
  while (-not ($p = Get-Process Algoritmia -ErrorAction SilentlyContinue | Select-Object -First 1)) { Start-Sleep -Seconds 1 }
  $ws = 0; $pm = 0
  while (-not $p.HasExited) {
    try { $p.Refresh(); $ws = [Math]::Max($ws, $p.PeakWorkingSet64); $pm = [Math]::Max($pm, $p.PrivateMemorySize64) } catch {}
    Start-Sleep -Seconds 2
  }
  'Pico de memoria: {0:N0} MB (espacio de trabajo) / {1:N0} MB (privada)' -f ($ws / 1MB), ($pm / 1MB)
  $log = "$env:USERPROFILE\AppData\LocalLow\Universidad Catolica de Colombia\Algoritmia\Player.log"
  Select-String -Path $log -Encoding UTF8 -Pattern 'RNF-04:', 'Exception', 'Renderer:', 'Device Type:' | ForEach-Object { $_.Line.Trim() }
}
```

Si PowerShell está bloqueado en el equipo: Administrador de tareas → Detalles → clic derecho en un
encabezado → «Seleccionar columnas» → «Espacio de trabajo máximo (memoria)» (*Peak working set*),
leída en la fila de `Algoritmia.exe` justo antes de cerrar el juego. El presupuesto es 2048 MB
(RNF-05, PF-RNF05-01).

## Anexo B · perfil `OE4_C`, para hacer H6 y H7 sin jugar los niveles 1 y 2

Con el juego cerrado, guarda esta línea como `Datos\OE4_C.json` junto al `.exe`; el nombre del
archivo tiene que coincidir con `name`. Es la semilla de OE4 `plan.md` §7: Nivel 2 completo y
Nivel 3 por jugar (sus narrativas salen enteras, sin «Omitir»). Bórralo al terminar.

```json
{"name":"OE4_C","reachedLevel":3,"phases":[{"level":1,"phase":1,"attempts":7,"correctedErrors":3,"stepsUsed":3,"resolutionSeconds":125.0},{"level":2,"phase":1,"attempts":4,"correctedErrors":2,"stepsUsed":0,"resolutionSeconds":61.0},{"level":2,"phase":2,"attempts":5,"correctedErrors":3,"stepsUsed":6,"resolutionSeconds":95.0},{"level":2,"phase":3,"attempts":2,"correctedErrors":5,"stepsUsed":12,"resolutionSeconds":140.0}]}
```
