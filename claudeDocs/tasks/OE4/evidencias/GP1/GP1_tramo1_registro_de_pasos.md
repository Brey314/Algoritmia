# GP1 · tramo 1/3 — S-INI (inicio, créditos, perfil OE4GP1) y S-N1 completo — 01/10/2026

- **Recorrido:** Golden Path #1 (PF-RNF13-01), ventana 1920×1080 (sin marco, el monitor es de 1920×1080), un solo proceso compartido por tres agentes en serie. Sesión del arnés: `GP1`.
- **Exe:** `C:\Dev\Algoritmia\Build\Algoritmia\Algoritmia.exe` (rc1, 479,0 MB). Editor cerrado.
- **Perfil:** `OE4GP1`, nuevo. `Preparar-Datos` sin perfiles a las 09:04 → `Datos/` vacía.
- **Evidencias:** `claudeDocs/tasks/OE4/evidencias/GP1/`; figuras del OE3 en `claudeDocs/tasks/OE4/evidencias/figuras/`.
- **Este tramo NO cierra el juego**: lo recibe el tramo 2 en el menú de niveles con el Nivel 2 desbloqueado.

| # | Hora | Acción | Observado | Veredicto |
|---|---|---|---|---|
| 1 | 09:04:00 | `Preparar-Datos` (sin perfiles) | `Datos/` vacía | OK |
| 2 | 09:04:32 | `Start-Juego GP1 -Sha256 6CD16EDF…` | SHA coincide; sin marco (monitor 1920×1080), cliente 1920×1080 en (0,0); pid 40152, muestreador 33824; sin aviso de contaminación | OK |
| 3 | 09:04:35 | `Esperar-Carga MainMenu` | 0,094 s | OK (RNF-04) |
| 4 | 09:04:43 | Foto gp1_001 | Inicio con título, lema, «Jugar», «Créditos», «Salir», «Progreso del equipo» y Algoritm de fuego; arriba a la derecha asoma el aviso de la superposición de NVIDIA («Presione Alt+Z…»), ajeno al juego | OK (aviso externo) |
| 5 | 09:04:57 | Foto gp1_002 tras 6 s | El aviso de NVIDIA se fue; pantalla limpia, sin rótulos de trabajo ni solapes → **fig-07-1-pantalla-de-inicio.png** y `GP1/PF-RF01-01_inicio.png` | P |
| 6 | 09:05:11 | Teclas de control negativo | Nada cambia (gp1_003) | P |
| 7 | 09:05:20 | Clic «Créditos» + `Esperar-Carga Credits` | 0,040 s; «Créditos» con la rejilla papel/persona (gp1_004) | OK |
| 8 | 09:05:33 | Arrastrar el asa de la barra al final | Se leen exactos: Familia Anonaky…autorización escrita de sus autores; «Entornos, objetos e interfaz: originales del proyecto, salvo los iconos de pausa.»; «Iconos de pausa: Phosphor Icons, licencia MIT.»; «Tipografías Baloo 2 y Nunito bajo licencia SIL OFL 1.1.»; sin enlaces (gp1_005 → `GP1/PF-RF08-01_creditos.png`) | P |
| 9 | 09:06:00 | «Volver» + `Esperar-Carga MainMenu` | 0,052 s | OK |
| 10 | 09:06:02 | «Jugar» | «¿Quién juega?» sin recargar: «Perfiles guardados» vacía; «Perfil nuevo», «Escribe tu nombre», «Tu nombre», «Solo el nombre: nada más se guarda», «Sin avatar, sin edad y sin curso.», «Volver», «Continuar» (gp1_006) | OK |
| 11 | 09:06:12 | Clic en el campo + `Escribir OE4GP1` | El campo muestra «OE4GP1» (gp1_007) | OK |
| 12 | 09:06:21 | «Continuar» | LevelSelect 0,066 s y Narrative 0,731 s; `Datos/OE4GP1.json` = `{"name":"OE4GP1","reachedLevel":1,"phases":[]}` (09:06:20.875) | P (PF-RF02-01) |
| 13 | 09:06:24 | Foto gp1_008 | `N1_Apertura` línea 1 «Noche helada, sin luna. La familia camina a la intemperie.»; sin «Omitir» y sin pausa | P (PF-RF06-01, PF-RF07-05) |
| 14 | 09:07:11 | Clic en el centro de la ilustración (0,5; 0,4) | La línea no cambia (gp1_009) | P (PF-RF05-01) |
| 15 | 09:07:20–31 | «Continuar» ×6 por `N1_Apertura` | 7 líneas distintas, una por clic, hasta «Solo oscuridad.»; sin «Omitir» (hoja_ap) | P |
| 16 | 09:07:47 | «Continuar» → `N1_AparicionGuia` | Fundido; Narrative 0,035 s | OK |
| 17 | 09:07:50–08:12 | «Continuar» ×10 | 11 líneas distintas; Algoritm habla con retrato y nombre en L4, L9 y L10; capa de oscuridad; sin «Omitir» (hoja_ag). L4 «¡Hola, familia! No tengan miedo. Me llamo Algoritm, y vine a acompañarlos.» → **fig-03-1-narrativa-desde-asset.png** | P |
| 18 | 09:08:31 | «Continuar» → `N1_Hallazgo` | Narrative 0,035 s | OK |
| 19 | 09:08:34–09:12 | «Continuar» ×17 | 18 líneas distintas (Niña, Papá, Niño, Mamá y Algoritm con retrato); la última es «Lo que no sabes todavía es cómo. ¿Desde dónde vas a intentarlo?»; sin «Omitir» (hoja_ha → `GP1/PF-RF10-01_hallazgo_L18.png`) | P (PF-RF05-01, PF-RF06-01, PF-RF10-01) |
| 20 | 09:09:26 | Último «Continuar» + `Esperar-Carga Level1_Cave` | 0,156 s; **T0 = 09:09:27,96** | P (RNF-04) |
| 21 | 09:09:30 | Foto gp1_013 | Solo: 5 hojas regadas, sílex y pedernal (7 piezas), la tablilla «Reúne todas las hojas y las dos piedras en el centro de la pantalla.», pista con Algoritm (sin texto) y pausa; sin deslizantes ni botones; cueva en penumbra → **fig-04a-1-reunir.png**, `GP1/PF-RF14-01_inicio_cueva.png` | P |
| 22 | 09:09:48 | Teclas de control negativo | Nada cambia (gp1_014) | P (PF-RNF02-01) |
| 23 | 09:09:51 | Pista | La tablilla repite «Reúne todas…» y aparece el círculo en el centro (gp1_015) | P (PF-RF13-01) |
| 24 | 09:10:06 | Arrastrar una hoja a (0,20; 0,50) | Se queda ahí; ni mensaje ni cambio (gp1_016) | P |
| 25 | 09:10:21–30 | Arrastrar las 5 hojas al círculo | Quedan en el centro; nada más cambia (gp1_017) | OK |
| 26 | 09:10:43–45 | Arrastrar sílex y pedernal; el último con foto a +100 ms | **Primer cuadro del acercamiento (gp1_018): el sílex y el pedernal ya se ven encima del montón, sin hojas encima.** Al terminar (gp1_020): piedras sobre el montón; «Fuerza del golpe» vertical («Fuerte» arriba, «Suave» abajo), «Lejos»–«Cerca», «Golpear» y «Soplar» atenuado con candado; tablilla «Elige la fuerza y qué tan cerca van las piedras, y golpea para hacer chispas.» → `GP1/PF-RF14-01_zoom_100ms.png`, `GP1/PF-RF14-01_panel.png` | **P — corrección «piedras sobre las hojas» comprobada** |
| 27 | 09:11:09 | Pista | La tablilla repite «Elige la fuerza…» (gp1_021) | P |
| 28 | 09:11:10–12 | Fuerza → muesca 3; cercanía → muesca 5 | Las asas y las piedras se mueven (se acercan); la tablilla y la luz no cambian (gp1_022). El asa de la fuerza cambia de color con la muesca (azul abajo → violeta) | P (PF-RF15-01) |
| 29 | 09:11:28 | «Golpear» (golpe 1), foto +0,5 s | «Las piedras apenas se rozan. No sale ninguna chispa.»; sin chispa; luz igual; sin cartel de derrota. El puño de Papá no se alcanza a ver a +0,5 s (brazos ya abajo) | P |
| 30 | 09:11:56 | «Golpear» (golpe 2) | «Los golpes suaves solo hacen ruido: las chispas no llegan a nacer.», distinto del anterior | P (PF-RF18-01) |
| 31 | 09:11:59 | «Golpear» (golpe 3) | Pista «Las chispas dependen de cómo chocan las piedras: de la fuerza y de qué tan juntas están. ¿Qué cambiarías antes del próximo golpe?», sin muescas | P (PF-RF13-01) |
| 32 | 09:12:01 | Clic en «Soplar» atenuado | Nada: ni mensaje ni cambio; el candado sigue (gp1_026) | P (PF-RF19-02) |
| 33 | 09:12:28 | Cercanía → 0; «Golpear» (golpe 4) | «Otra vez las piedras no llegan a tocarse. Sin choque no hay chispa.»; habla de las piedras y no repite (gp1_027) | P |
| 34 | 09:12:54 | Pausa | «Pausa» con «Reanudar», «Reiniciar» y «Volver al menú de niveles»; el juego atenuado detrás (gp1_028) | P (PF-RF07-01) |
| 35 | 09:13:26 | Esperar con la pausa abierta (09:12:54,2 → 09:13:26,4 = **32,1 s**) y «Reanudar» | Deslizantes, tablilla y luz iguales que antes (gp1_029) | P (PF-RF07-02) |
| 36 | 09:13:38–41 | Fuerza → 7, cercanía → 5; «Golpear» (golpe efectivo 1) con foto a +0,1 s | «Una chispa cae dentro de las hojas. Brilla un instante y se apaga.»; **un trazo amarillo pálido corto cae en las hojas de la mitad de abajo del montón, bajo la juntura de las piedras**; la luz sube un escalón (luminancia media de la franja izquierda 16,4 → 22,2; esquina inferior derecha 21,8 → 28,2); «Soplar» sigue con candado → **fig-04a-2-encendido.png**, `GP1/PF-RF16-01_rayo_efectivo1.png` | P |
| 37 | 09:14:12 | «Golpear» (efectivo 2) con foto a +0,2 s | «Otra chispa cae dentro. Esta vez el brillo dura un poco más.»; rayo desde el centro hacia la izquierda, casi horizontal y apenas hacia abajo, que **pasa por encima del sílex** y acaba en las hojas del borde izquierdo; luz 22,2 → 26,9; el candado sigue (gp1_033) | P, con observación OBS-2 |
| 38 | 09:14:35–38 | «Golpear» (efectivo 3) con foto a +0,15 s y dos más | «Un hilo de humo sube despacio, como si la cueva estuviera suspirando.»; rayo desde la juntura de las piedras hacia abajo, a las hojas; luz 26,9 → 30,7. **«Soplar» se habilita sin candado.** **El humo nace en el centro, detrás de las piedras (el pedernal tapa su base), y sube como un hilo blanco sobre las hojas, por encima de ellas y por debajo de las piedras** (gp1_035–037, humo_cmp) | **P — correcciones «chispa como rayo» e «hilo de humo desde el centro» comprobadas** |
| 39 | 09:15:22–24 | Fuerza → 9; «Golpear» con foto a +0,12 s | «Las piedras chocan con fuerza y las chispas saltan por todas partes, lejos de las hojas.»; **el rayo sale del centro hacia arriba a la derecha, más largo, pasa del montón y acaba hacia y = 0,23, antes de la tablilla (borde inferior y ≈ 0,146)**; «Soplar» sigue habilitado; la luz no baja (30,676 → 30,676) (gp1_039) | P (PF-RF16-01, PF-RF19-01) |
| 40 | 09:16:20,9 | «Soplar» con **doble clic** (×2 cada 400 ms), foto a +0,15 s | «El humo se abre. Algo naranja tiembla entre las hojas.» una sola vez; «Soplar» y «Golpear» atenuados en seguida, **sin candado**; las asas de los deslizantes, un poco atenuadas; nace la llama sobre el montón (gp1_041) | P |
| 41 | 09:16:22,7 | Clic en «Golpear» durante el encendido | No responde: la tablilla sigue con «El humo se abre…» y no hay rayo. **El humo sube desde la corona de la llama, detrás de ella, encogido, y pasa por detrás de la tablilla: no asoma por encima ni se corta arriba** (gp1_042 → `GP1/PF-RF20-01_humo_corona_mandos_quietos.png`) | **P — correcciones «mandos quietos al soplar» y «humo a la corona» comprobadas** |
| 42 | 09:16:24,0 | Clic en el riel de la fuerza (muesca 3) durante el encendido | El asa no se mueve (sigue en 9) (gp1_043) | P (mandos quietos) |
| 43 | 09:16:24,4 | Fin del encendido (3,5 s, `N1_Config.IgnitionSeconds`): **T1 ≈ 09:16:24,4**; fundido a `N1_NacimientoDelFuego` (Narrative 0,369 s) | `CompleteLevel` encadena la narrativa en cuanto termina el encendido, así que la ráfaga de 6 s ya cae en la narrativa | OK (ver OBS-3) |
| 44 | 09:16:25–35 | `Rafaga gp1_rafaga_llama 6 10` | 60 cuadros a 10,1 fps: entra el fundido desde negro (Δ de luminancia máx. 0,045 por cuadro en los 0,6 s del fundido) y después el bucle de la llama de la narrativa, con Δ máx. 0,0012 por cuadro: **ningún destello** → `GP1/PF-RNF21-01_rafaga_llama.csv` | P (PF-RNF21-01, sobre el bucle de la narrativa) |
| 45 | 09:18:14–50 | `N1_NacimientoDelFuego` línea a línea (16 «Continuar») | 17 líneas distintas, sin «Omitir» y sin pausa; L15 Algoritm: «Eso tiene nombre: se llama iterar. Probar, mirar el resultado y ajustar.» → **fig-04a-3-cierre-iterar.png**, `GP1/PF-RF12-01_iterar.png` | P (PF-RF05-01, PF-RF06-01, PF-RF12-01) |
| 46 | 09:19:09 | Último «Continuar» + `Esperar-Carga LevelSummary` | 0,035 s. «Esto es lo que pasó en la cueva», «Descubriste que el fuego necesita chispa y aire.», «Probaste golpear con varias fuerzas y desde varios sitios.», «Cuando algo no funcionó, cambiaste la fuerza o el sitio y volviste a intentar.», «Eso se llama probar y ajustar: es lo mismo que hace quien arma un plan paso a paso.»; **ningún dígito** (gp1_045 → `GP1/PF-RF45-01_resumen.png`) | P (PF-RF45-01, PF-RF17-01) |
| 47 | 09:19:09 | `Datos/OE4GP1.json` (escrito 09:19:09,07) | `{"name":"OE4GP1","reachedLevel":2,"phases":[{"level":1,"phase":1,"attempts":5,"correctedErrors":1,"stepsUsed":3,"resolutionSeconds":384.98}]}`. Previsto: I 5 · C 1 · P 3 y T1 − T0 − pausa = 416,4 − 32,1 = **384,3 s** (diferencia 0,7 s) → `GP1/GP1_OE4GP1_tras_N1.json` | P (PF-RF04-01, PF-RF07-06) |
| 48 | 09:19:57 | «Continuar» + `Esperar-Carga LevelSelect` | 0,068 s. «Elige un nivel»: «Nivel 1 · La Oscuridad» con visto y «Completado»; «Nivel 2 · La Rueda» habilitado y sin candado; «Nivel 3 · El Río» con candado y «Bloqueado» → **fig-07-2-menu-de-niveles.png**, `GP1/PF-RF03-02_menu_tras_N1.png` | P (PF-RF03-02) |
| 49 | 09:20:32 | Clic en «Nivel 3» bloqueado, foto a +1,5 s | Nada: la foto es idéntica píxel a píxel y el registro no muestra ninguna carga | P (PF-RF03-03) |
| 50 | 09:20:44 | `Revisar-Log GP1` (con el juego abierto) | 11 líneas RNF-04, todas < 1 s (peor: Narrative 0,731 s); **sin Exception, Error ni Assert** → `GP1/GP1_tramo1_Player.txt` | P |
| 51 | 09:20:45 | `Medidas GP1` (parcial) | RNF-05: Private máx. 1127 MiB (< 2048), CUMPLE. **RNF-10: 725 filas EXTERNA**: las mismas tres conexiones TCP :443 a 34.8.90.77, 34.111.113.40 y 34.107.172.168 que en el spike → **se reproduce DEF-SPIKE-01** (Unity Analytics/Connect en el player). Copias parciales: `GP1/GP1_tramo1_{mem,red}.csv` y `GP1/GP1_tramo1_registro.tsv` | RNF-05 OK · RNF-10 F (DEF-SPIKE-01) |

## Correcciones de hoy (D10-1) comprobadas sobre el exe

| Corrección | Veredicto | Evidencia |
|---|---|---|
| Piedras sobre las hojas al pasar a golpear, desde el primer cuadro del acercamiento | **Comprobada** | `GP1/PF-RF14-01_zoom_100ms.png` (+100 ms), `GP1/PF-RF14-01_panel.png` |
| Chispa como rayo: el efectivo sale del centro y cae en las hojas, al azar y cada vez más largo; el de fuerza de más sube y se apaga antes de la tablilla | **Comprobada** (con OBS-2) | `GP1/PF-RF16-01_rayo_efectivo1.png`, gp1_033, `GP1/PF-RF19-01_soplar_habilitado_rayo3.png`, `GP1/PF-RF16-01_rayo_fuerza_de_mas.png` |
| Humo como un hilo que nace en el centro (detrás de las piedras, sobre las hojas) y sube a la corona al prender, detrás de la llama y de la tablilla | **Comprobada** | `GP1/PF-RF19-01_hilo_de_humo.png`, `GP1/PF-RF20-01_humo_corona_mandos_quietos.png` |
| Mandos quietos al soplar (sin doble «Soplar»; «Golpear» y los deslizantes no responden) | **Comprobada** | gp1_041–043; `GP1/PF-RF20-01_humo_corona_mandos_quietos.png` |

## Observaciones (no son defectos del tramo)

- **OBS-1 · Superposición de NVIDIA.** Al arrancar asoma unos 5 s, arriba a la derecha, el aviso «Presione Alt+Z para abrir la página NVIDIA Superposición» (GeForce Experience). Es del equipo y no del juego; las capturas de las figuras se tomaron cuando ya no estaba.
- **OBS-2 · Rayo casi horizontal sobre el sílex.** En el golpe efectivo 2 el rayo salió casi horizontal hacia la izquierda (dentro de la mitad de abajo por pocos píxeles) y se dibujó **por encima del sílex** antes de llegar a las hojas. El guion dice «cae sobre las hojas de la mitad de abajo»; acaba en hojas, así que no es F, pero una dirección en el borde del abanico lo hace cruzar la piedra. Para el visto bueno de Santiago: ¿estrechar el abanico o dibujar el rayo bajo las piedras?
- **OBS-3 · La ráfaga del paso 21 no cabe en la cueva.** `CompleteLevel` lanza `N1_NacimientoDelFuego` justo al acabar los 3,5 s del encendido, así que en `Level1_Cave` no hay 6 s de «bucle de la llama» que grabar. La ráfaga de este tramo cubre el bucle de la llama **en la narrativa** (sin destellos). Ajustar `casos.md` S-N1 paso 21: tres fotos durante el encendido y la ráfaga sobre la narrativa.
- **OBS-4 · Asa de la fuerza en colores puros.** El asa del deslizante de la fuerza va de azul puro (≈`#0000FF`, muesca 0) a violeta y rojo puro (muesca 9), fuera de la paleta (Dirección de arte §4). Parece intencional (indica la fuerza), pero conviene que lo mire el carril de arte.
- **OBS-5 · Píldoras «Completado» y «Bloqueado».** El texto toca el borde derecho de la píldora, sin margen (ya anotado en el spike; S-INI paso 14).
- **OBS-6 · Avatar del guía en el resumen.** El círculo del resumen muestra el **rótulo** «Algoritm» y no el dibujo de Algoritm. Está documentado («rótulo «Algoritm» del resumen», Interfaces.md §2 y Dirección de arte), así que no es F; se anota por si el mockup 13 pedía el dibujo.
- **OBS-7 · Papá a +0,5 s del golpe 1.** En la foto no se ve el puño levantado (los brazos ya están abajo); habría que tomarla a +0,2 s. No afecta a ningún caso.

## Figuras del entregable OE3 tomadas en este tramo (`evidencias/figuras/`)

| Fig. | Archivo | Origen | Contenido |
|---|---|---|---|
| 3.1 | `fig-03-1-narrativa-desde-asset.png` | gp1_011_ag_L04 | `N1_AparicionGuia` L4: capa de oscuridad, Algoritm de fuego, la familia y el cuadro con retrato y nombre «ALGORITM» |
| 4.1 | `fig-04a-1-reunir.png` | gp1_013 | Inicio de la cueva: 5 hojas, sílex y pedernal regados, tablilla, pista con Algoritm de fuego y pausa |
| 4.2 | `fig-04a-2-encendido.png` | gp1_031 | Panel de encendido antes de la convergencia: los dos deslizantes, «Golpear», «Soplar» atenuado con candado, las piedras sobre las hojas y el rayo del primer golpe efectivo |
| 4.3 | `fig-04a-3-cierre-iterar.png` | gp1_044_nf_L15 | `N1_NacimientoDelFuego`: «Eso tiene nombre: se llama iterar. Probar, mirar el resultado y ajustar.» |
| 7.1 | `fig-07-1-pantalla-de-inicio.png` | gp1_002 | Inicio con arte real: «Jugar», «Créditos», «Salir» y «Progreso del equipo» |
| 7.2 | `fig-07-2-menu-de-niveles.png` | gp1_046 | Menú de niveles: N1 «Completado» con visto, N2 disponible, N3 «Bloqueado» con candado |

## Entrega al tramo 2

- **Juego ABIERTO:** pid 40152, sesión del arnés `GP1`, lanzamiento 1, ventana sin marco de 1920×1080 en (0,0); muestreador pid 33824 vivo.
- **Pantalla actual:** «Elige un nivel» (LevelSelect), con el perfil `OE4GP1` activo y el Nivel 2 habilitado.
- **Perfil:** `Datos/OE4GP1.json` con N1F1 = 5/1/3/384,98 y `reachedLevel` 2.
- **Para seguir:** clic en «Nivel 2 · La Rueda» (0,500; 0,606) → `Esperar-Carga Narrative` → `N2_PuenteI` (primera visita: sin «Omitir»).
- No se cerró el juego ni se restauró el registro (lo hace el tramo 3 al cerrar). El Editor sigue cerrado.
- **Herramientas de este tramo:**
  - `oe4/o.sh`, envoltorio con `OE4_TRABAJO` ya puesto: `o.sh <Subcomando> …`;
  - `oe4/hoja.py`, hoja de contacto: `python hoja.py salida.png dialogo|grid fotos…`.
- Para esperar con la pausa abierta, `sleep 30` está bloqueado: usar `until [ $(date +%s) -ge $T ]; do sleep 2; done`.
