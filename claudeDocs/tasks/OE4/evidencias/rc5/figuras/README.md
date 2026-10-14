# Capturas de rc5 para el entregable del OE3 y el trabajo de grado

## Procedencia

| Dato | Valor |
|---|---|
| Ejecutable | `C:\Dev\Algoritmia\Build\Algoritmia\Algoritmia.exe`, rc5 (`build-rc5.md`) |
| SHA-256 del `.exe` | `6cd16edfbe85284eb69cd2efe1170aa1cffdf0dbdb309c3269e2f8454811adab` (coincide con `build-rc5.md`; el `.exe` es solo el lanzador, igual desde rc1) |
| Huella del contenido | `Algoritmia_Data` `e9ab1549…6e05`, 223 archivos; carpeta entregable `ab0c386e…46c3`, 252 archivos: ambas coinciden con `build-rc5.md`, así que es rc5 y no otro build con el mismo `.exe` |
| Fecha | 14/10/2026 |
| Arnés | `oe4.ps1`, sesión `FIG-rc5` (5 lanzamientos), cliente 1920 × 1080 con marco en el monitor 1 (2560 × 1440), `-Sha256` en cada `Start-Juego` |
| Contaminación | Había Unity (Editor) y Word abiertos: el arnés lo avisó. No afecta a las capturas (solo falsea RNF-04 y RNF-05, que aquí no se midieron) |
| Salida | PNG originales de 1920 × 1080 en esta carpeta; JPG finales de 1280 × 720, calidad 90, en `docs/OE3/fig/` con el mismo nombre (hechos con Pillow, LANCZOS) |
| Perfiles semilla | `OE4_B` (portada, menú, N2 fase 1 y taller), `OE4_B3` (laberinto), `OE4_A` (N1 completo), `OE4_C` (N3 y escena final) |

Los `Player.log` de los cinco lanzamientos están en `logs/` (sin `Exception`, `Error` ni `Assert`; `Revisar-Log` sin hallazgos). El lanzamiento 2 se cerró **solo** (cierre normal en el log, sin excepción) mientras otra ventana tenía el foco; se repitió el lanzamiento y no afecta a ninguna figura.

Estado final: juego cerrado; `Datos/` eliminada (no existía antes: `Preparar-Datos -SinCarpeta`); clave del registro igual que en la foto previa (`Residuos` dice «existía y no cambió»). **`Restaurar-Registro` falló** con `SetValue … RegistryValueKind` en los valores `Binary` (error del arnés: `FromBase64String` dentro del `switch` se desenrolla a `object[]`); los cuatro valores (`unity.player_session_count`, `unity.player_sessionid`, `unity_connect.mega_session_id`, `unity_connect.session_id`) se repusieron a mano con `winreg` desde `residuos\previo-FIG-rc5.json`. En LocalLow cambió `Player.log` y el arnés movió `Player-prev.log` a `%TEMP%\Algoritmia-OE4\logs\`: son registros volátiles.

## Figuras (las 16)

Todas reemplazadas. «Pie» = el pie de figura actual en `claudeDocs/entregables/OE3/src/*.md`. Donde dice «coincide», el texto exacto del pie se ve en pantalla.

| Figura (archivo) | Cómo se llegó | Pie vs. pantalla |
|---|---|---|
| 3.1 `fig-03-1-narrativa-desde-asset` | `OE4_A` → Jugar → OE4_A → Nivel 1; 7 líneas de `N1_Apertura` + 3 de `N1_AparicionGuia`; la 4.ª línea de la secuencia es la de Algoritm | Texto **coincide**: «¡Hola, familia! No tengan miedo. Me llamo Algoritm, y vine a acompañarlos.». **Cambia:** el cuadro lleva dos botones, «Atrás» y «Continuar» (INC-160); el pie dice «junto al botón «Continuar»». La familia sale con expresión de sorpresa (boca abierta), la cueva más clara que en la captura vieja (INC-152) y Algoritm con pantaloneta de cuadros verdes |
| 4.1 `fig-04a-1-reunir` | Cueva tras las 36 líneas; **al entrar sale un aviso** («Este es el botón de ayuda. Si no recuerdas qué hacer, haz clic en él y Algoritm te repetirá la instrucción.», botón «Cerrar», INC-158); la captura es después de cerrarlo | **Coincide**: tablilla «Reúne todas las hojas y las dos piedras en el centro de la pantalla.», ayuda arriba a la izquierda con la figura de fuego, Papá a un lado del círculo de luz y los otros tres al otro. La tablilla no lleva icono. Las piezas cayeron al azar (cinco hojas, sílex, pedernal) |
| 4.2 `fig-04a-2-encendido` | Siete piezas arrastradas al centro; fuerza en la muesca 7, cercanía en la 5, «Golpear»; foto a +0,4 s del primer golpe efectivo | **Coincide**: tablilla «Una chispa cae dentro de las hojas. Brilla un instante y se apaga.»; deslizante vertical con «Fuerza del golpe», «Fuerte» arriba y «Suave» abajo; horizontal «Lejos»–«Cerca»; «Golpear» activo y «Soplar» atenuado con candado. Papá con las manos juntas tras el golpe |
| 4.3 `fig-04a-3-cierre-iterar` | Tres golpes efectivos + «Soplar» + 14 líneas de `N1_NacimientoDelFuego` (línea 14, Algoritm) | **Coincide**: «Eso tiene nombre: se llama iterar. Probar, mirar el resultado y ajustar.»; Algoritm (llama) sobre las brasas, humo subiendo. Detalle no dicho por el pie: Papá, de perfil, abraza a la Niña; el cuadro lleva «Atrás» |
| 4.4 `fig-04b-1-bosque-rechazo` | `OE4_B` → Nivel 2 → 19 líneas (PuenteI 3, Bosque 9, Escena 2.1 7) → bosque; un tronco y luego una piedra (4.º rechazo seguido, foto a +0,28 s, cuando la Niña levanta el brazo) | **Coincide**: «Esta piedra tiene esquinas. Cuando la empujas, se traba.» con icono de alerta, «Troncos redondos: 1 de 5», la Niña anima junto a la caja, ayuda abajo a la derecha con Algoritm de rueda. Pausa arriba a la derecha |
| 4.5 `fig-04b-2-escena22-patron` | Seguido del anterior: cinco troncos, caja sobre ellos, «Empujar»; línea 6 de `N2_Escena22_ElPatron` | **Coincide**: «Acabas de encontrar un patrón.» con Algoritm de rueda, caja ladeada junto a los troncos alineados. Detalles no dichos: un sexto tronco grande al frente, Papá con el sílex en la mano, el Niño y la Niña con los brazos en alto; el cuadro lleva «Atrás» |
| 4.6 `fig-04b-3-taller-fuera-de-orden` | Seguido: Escena 2.2 completa y `N2_Escena23_Construccion` (8 líneas) → taller; tronco corto A + «Mecanizar»; tabla arrastrada al tronco B | **Coincide**: «La tabla no tiene sobre qué apoyarse todavía.» con alerta; la tabla vuelve a su sitio; el primer tronco corto conserva su agujero; «Mecanizar» atenuado con candado y «Aún no». El mazo ahora se ve como una maza de madera |
| 4.7 `fig-04b-4-laberinto-editor` | `OE4_B3` → Nivel 2 → 19 líneas → laberinto; cajón abierto; cuatro bloques arrastrados y lista desplazada con ▲ | **Coincide en lo estructural:** marco redondeado sobre fondo ámbar (hoy `#C4A882`, más apagado que el `#E8A33D` de la captura vieja), carretilla en la salida con la Niña al lado, Papá arriba y Mamá y el Niño abajo en el refugio, «Tu secuencia» con la lista desplazada («Avanzar», «Girar ›», «Retroceder ›» comprimidos al rótulo, botones ▲ ▼ y barra), cajón «Bloques» abierto con «Avanzar», «Girar», «Retroceder» con su silueta y «Arrastra un bloque a tu secuencia»; arriba la tablilla «Lleva la carretilla hasta el refugio. Pero no la vas a empujar tú. Vas a escribir antes todos los pasos, y luego los ejecutas de una sola vez.». **Cambia:** el cierre «Captura del Editor del 08/10/2026» (ahora es del ejecutable rc5, 14/10/2026); el tablero es otro (es aleatorio) |
| 4.8 `fig-04c-1-orilla` | `OE4_C` → Nivel 3 → 19 líneas (2 + 5 + 4 + 8) → orilla | **Coincide**: lista de cuatro tareas pendientes («Recoger troncos», «Encontrar sogas», «Armar la balsa», «Colocar el mástil y la vela»), tablilla «Recorre la orilla y recoge lo que la balsa necesita. La lista te dice qué falta.», inventario de cuatro casillas y cruceta abajo, anillo de la zona en el centro, río al pie de la cascada. Detalle: la ayuda (gota) está arriba a la derecha bajo la pausa. Esta vez no salió el aviso del botón de ayuda |
| 4.9 `fig-04c-2-base-incorrecta` | Ocho materiales recogidos con la cruceta, zona, mástil y cuatro troncos en las siluetas, «Listo» | **Coincide**: «Algo de esta parte no encaja. Mira lo marcado: ¿esa pieza va ahí?», espacio del quinto tronco con alerta, mástil de vuelta en su casilla, cuatro troncos puestos, «Listo» y «Probar balsa» juntos, dos tareas de recolección marcadas |
| 4.10 `fig-04c-3-prueba-fallida` | Base y amarre aprobados; tela en la silueta del mástil y mástil en la de la vela; «Probar balsa». **Es el segundo fallo seguido de la balsa terminada** (el primero funde a la 3.2 antes de que el arnés alcance la foto, ~1,3 s) | **Coincide**: «La balsa se volteó. Mira el espacio marcado: ¿qué debería ir ahí?», dos espacios con alerta, tela y mástil de vuelta en el inventario, solo «Probar balsa», tres tareas marcadas, base y amarres en su sitio. El estado visible es el mismo que en el primer fallo |
| 4.11 `fig-04c-4-escena-3-2` | Tras el primer fallo: línea 4 de `N3_Escena32_PrimerIntento` | **Coincide**: «No pasa nada. Esto se llama depurar. No hay que hacer todo otra vez: hay que encontrar la parte que falló. ¿Qué salió mal?» con Algoritm de gota, balsa hundida por un costado, familia en el agua. El cuadro lleva «Atrás» |
| 7.1 `fig-07-1-pantalla-de-inicio` | Primer cuadro tras `Esperar-Carga MainMenu`; se repitió 1,5 s después para evitar un parpadeo de Mamá | **Coincide**: título «Algoritmia», tablilla «Piensa el orden, enciende el fuego», «Jugar», «Créditos» y «Salir» en fila, «Progreso del equipo». Los cinco personajes en el bosque (Mamá saluda, Papá con las manos en la cintura; Algoritm de fuego con pantaloneta de cuadros verdes). **Cambia:** «Captura del Editor del 08/10/2026» → ejecutable rc5 |
| 7.2 `fig-07-2-menu-de-niveles` | Jugar → OE4_B | **Coincide** palabra por palabra (visto y «Completado» en el Nivel 1, Nivel 2 sin insignia, Nivel 3 oscurecido con candado y «Bloqueado»). **Cambia:** «Captura del Editor del 08/10/2026» → ejecutable rc5 |
| 7.3 `fig-07-3-escena-final` | Tras el flujo del N3 correcto (3.3 y resumen «Continuar»), línea 4 de `N3_EscenaFinal`. Recorte (60, 67)–(1861, 1080) de 1920 × 1080, escalado a 1280 × 720: es el recorte que más se parece al viejo (borde casi a ras del cuadro y ligera ampliación) | **Coincide**: «En el bosque miraste muchas cosas y encontraste lo que tenían en común.» con Algoritm de gota sobre Papá y Mamá, tres fogatas con humo a la derecha del Niño. El cuadro lleva «Atrás» |
| 7.4 `fig-07-4-lista-en-grises` | Octava pieza recogida; `Foto -Gris` a +1,5 s; el JPG sale de `fig-07-4-lista-en-grises_gris.png` (el PNG en color también está) | **Coincide**: dos tareas con visto dentro del círculo, mensaje «Ya tienes todos los materiales. Ve a la zona marcada junto al agua.» con su icono, cruceta abajo a la derecha. Detalle: el botón de ayuda se ve pálido |

## Pies que hay que corregir (texto exacto en pantalla)

1. **Figura 3.1** (`03-arquitectura.md`): «…junto al botón «Continuar»» → junto a los botones **«Atrás»** y **«Continuar»** (el «Atrás» sale desde la línea 2 de toda secuencia, INC-160).
2. **Figura 4.7** (`04b-nivel2.md`): «Captura del Editor del 08/10/2026» → captura del ejecutable (rc5, 14/10/2026). El resto del pie vale.
3. **Figura 7.1** y **7.2** (`07-interfaz.md`): «Captura del Editor del 08/10/2026» → captura del ejecutable (rc5, 14/10/2026).

Opcionales, no son errores: 4.5 (hay un tronco grande de más al frente y Papá sostiene el sílex), 4.3 (Papá abraza a la Niña de perfil), 4.1 (al entrar a la cueva sale primero el aviso «Este es el botón de ayuda…» con «Cerrar»; la figura es posterior).

## Límites y avisos

- La 4.10 viene del segundo fallo, no del primero (ver arriba); el estado visible es el descrito por el pie.
- El arnés tarda ~1,3 s en tomar una foto tras un clic: lo transitorio (chispa del golpe, hundimiento) no se captura; las figuras muestran el mensaje ya estable.
- La 7.3 es un recorte; el original completo está en `fig-07-3-escena-final.png`.
- No se tocaron el Editor, Word ni las figuras F-1..F-4 del Anexo F; no hay commit.
