# S-TECLAS · rc5 · resumen

- **Versión:** rc5 (`ab0c386e…`), carpeta `Build\Algoritmia_rc5\`. Fecha 05/11/2026, ~14:06–14:15. Sesiones de arnés `S-TECLAS-rc5` (taller, perfil `OE4_B2`) y `S-TECLASb-rc5` (laberinto, perfil `OE4_B3`; segundo lanzamiento con otro nombre para no pisar la foto previa de residuos). Ventana 1920×1080 con marco.
- **Alcance:** parte que faltaba de `PF-RNF02-01` (DEF-RC5-18): paso 1b de S-N2B y S-N2C. Cueva, bosque y orilla las juzgaron S-N1, S-N2A y S-N3. **Una fila: P, solo taller y laberinto.**
- **Llegada:** Jugar → fila del perfil → Nivel 2 → 19 «Continuar» (sin «Omitir») hasta `Level2_Workshop` (0,020 s) y `Level2_Maze` (0,026 s) según el `Player.log`; 3 s de espera antes de la foto «antes».
- **Teclas:** `Teclas Esc Enter Espacio Arriba Abajo Izquierda Derecha W A S D Rueda`, salida 0 y foco correcto (primer plano True al lanzar, ninguna salida 3) en ambas.
- **Píxeles (umbral 16/255, PIL):**
  - Taller: 23.246 px, todos en la banda y 302–540 (x 646–1527): los cuatro personajes respirando. 0 px en piezas y botón «Mecanizar»/«Aún no» (y ≥ 545), y 0 px en tablilla, ayuda y pausa (y < 300). No se abrió la pausa.
  - Laberinto: 4.607 px, todos en la Niña (614 px, x 115–151) y la familia del refugio (3.993 px, x 1165–1260). 0 px en tablero (235–1100 × 245–830), carretilla, panel «Tu secuencia», cajón «Bloques» y «Ejecutar». No se abrió la pausa.
- **Reacción posterior:** taller, clic en el tronco corto → «El tronco quedó resaltado.», el tronco crece con contorno y «Mecanizar» se habilita (sin «Aún no»). Laberinto, clic en el botón del cajón → se abre con «Arrastra un bloque a tu secuencia», «Avanzar», «Girar» y «Retroceder».
- **Cierre:** `Revisar-Log` sin Exception/Error/Assert en ambos; `Residuos` solo el registro de siempre (ocho valores de la clave, que ya existía) y `%TEMP%`; `Restaurar-Registro` de las dos sesiones (la `b` primero, la original después) dejó la clave «existía y no cambió»; `Preparar-Datos -SinCarpeta` hecho; `manifiesto.ps1 -Verificar` coincide (253 archivos, `ab0c386e…`).
- **Avisos del arnés:** `Start-Juego` no avisó de contaminación por Unity Hub. Ninguna salida distinta de 0. No se midieron cargas ni memoria (los `_mem.csv` no son evidencia de RNF-04/05).
- **Defectos:** ninguno. Límites: la comparación es por píxeles de una foto antes y una después, no de una ráfaga; una tecla con efecto transitorio que ya hubiera vuelto al estado inicial no se vería. Las teclas se enviaron con la escena quieta, sin pieza ni bloque elegidos.
