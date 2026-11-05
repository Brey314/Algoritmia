# Instrucciones comunes: pasada de regresión del OE4 sobre rc5

Un evaluador pidió veredictos de una sola versión. rc5 se compiló desde `ca6fd91` y no se recompila. Paquete congelado en `C:\Dev\Algoritmia\Build\Algoritmia_rc5\`, huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3`. Comunícate en español.

## Qué leer antes de empezar
1. `C:\Dev\Algoritmia\claudeDocs\tasks\OE4\casos.md`: líneas 1–110 (cabecera, sobre todo «La pasada vigente es sobre rc5» y «Cómo leer este archivo») y la sección de tu sesión, que es tu guion: ejecuta todos los pasos, sin atajos.
2. `C:\Dev\Algoritmia\claudeDocs\tasks\OE4\herramientas\oe4-USO.md`, para los subcomandos del arnés.
3. Como modelo de formato: `evidencias\rc5\S-INI\` (`veredictos.csv`, `resumen.md`). El `resumen.md` de tu sesión en `evidencias\rc3\` sirve para saber qué costó allí; no heredes de ahí ningún veredicto.

## Cómo lanzar el arnés
Cada llamada es un proceso nuevo y necesita las variables. Con la herramienta PowerShell:
```
$env:OE4_EXE='C:\Dev\Algoritmia\Build\Algoritmia_rc5\Algoritmia.exe'; $env:OE4_MANIFIESTO='C:\Dev\Algoritmia\claudeDocs\tasks\OE4\evidencias\rc5\manifiesto-rc5.tsv'; pwsh -NoProfile -File 'C:\Dev\Algoritmia\claudeDocs\tasks\OE4\herramientas\oe4.ps1' <Subcomando> ...
```
Nombra la sesión de arnés `<SESIÓN>-rc5` (con sufijo por lanzamiento si hace falta). `Start-Juego` verifica la carpeta contra el manifiesto y sale con 3 si no coincide: **no eludas esa guarda**, no quites la variable y no cambies de carpeta. Si falla, para y repórtalo.

## Reglas
- Caja negra: no leas código del juego para decidir un veredicto; decide por lo que se ve en pantalla, el `Player.log` y los JSON de `Datos\`. Sí puedes consultar los assets para entender una discrepancia de texto.
- Antes de cada clic mira la pantalla (`Foto` o `-ConFoto`): las coordenadas del catálogo son aproximadas. Tras cargar una escena el juego ignora clics 0,3 s: espera antes de pulsar.
- **Mira de verdad las capturas** con la herramienta Read antes de dar un veredicto.
- **No edites** `casos.md`, ni código, ni assets, ni `oe4.ps1`, ni hagas commits. Si el catálogo está mal o el arnés falla, anótalo y repórtalo.
- Unity y Word deben estar cerrados; si el arnés avisa de contaminación, dilo en el resumen y no reportes como válidas las medidas de carga y memoria.
- Salida 3 por foco, cursor u otra ventana: mira qué pasó y reintenta como máximo 3 veces esa acción. Si persiste, deja el caso en `B` con el motivo. Nunca inventes un resultado.

## Veredictos
`P` aprobado · `PD` aprobado con desviación documentada · `F` fallido · `B` bloqueado · `NA` no aplica · `NE` no ejecutado. Un caso aprueba solo si se observan **todos** los esperados de los pasos que lo nombran. Una diferencia de palabras en un `«texto»` es F, salvo que sea error del catálogo por un cambio ya cerrado en un INC: entonces repórtalo como defecto de documento. Los casos que se alimentan también de otras sesiones (como `PF-RF05-04`, `PF-RF05-05`, `PF-RF06-01`, `PF-RF07-01`, `PF-RF10-04`, `PF-RF13-06`) se juzgan solo en tu parte: dilo en la nota y escribe una fila por cada uno. Lo que alimenta a casos de S-RNF o S-INSP (`PF-RNF02-01`, `PF-RNF03-01`, `PF-RNF04-01`, `PF-RNF05-01`, `PF-RNF10-01`, `PF-RNF19-01`, `PF-RNF21-01`, `PF-RNF23-01`) no se juzga aquí: guarda las capturas que pide el guion, a pantalla completa cuando el caso lo pida, y da tus datos en el resumen.

## Dónde dejar la evidencia, en `C:\Dev\Algoritmia\claudeDocs\tasks\OE4\evidencias\rc5\<SESIÓN>\`
- Las capturas que respaldan cada veredicto, con nombre `<caso>_<paso>_<n>.png`. Copia también `*_mem.csv`, `*_red.csv`, `*_registro.tsv`, `*_Player.txt` y los JSON finales; los `.log` se guardan como `.txt`. Cuando `casos.md` pide «copiar el JSON a `evidencias/...`», va en esta carpeta.
- `veredictos.csv`, en UTF-8, con cabecera exacta `caso,veredicto,fecha,candidato,huella,observado,registro`: `fecha` la real en formato AAAA-MM-DD, `candidato` rc5, `huella` ab0c386e, `observado` lo que se vio en una o dos frases con los textos y las cifras exactas, `registro` las rutas de evidencia relativas a `evidencias/`, separadas por `; `. Una fila por cada caso de la tabla de la sesión.
- `resumen.md`: versión y huella, fecha y horas reales, perfiles, cifras medidas, avisos del arnés, defectos y límites de lo que no se pudo ver.
- Cada defecto va al final de `evidencias\rc5\defectos.md` (créalo si no existe) como `### DEF-RC5-nn · título`, con severidad propuesta (Bloqueante, Mayor, Menor u Observación), tipo (juego, documento o arnés), caso, pasos y evidencia. Usa el siguiente número libre.

## Al terminar, en este orden y con el juego cerrado
`Revisar-Log` · `Residuos` · `Restaurar-Registro -Sesion <sesión>` · `Preparar-Datos -SinCarpeta` · `pwsh -NoProfile -File ...\manifiesto.ps1 -Verificar -Carpeta C:\Dev\Algoritmia\Build\Algoritmia_rc5 -Manifiesto ...\manifiesto-rc5.tsv`, que debe seguir coincidiendo.

## Lo aprendido en el piloto S-INI (cliente 1920×1080)
- **Campo de nombre:** tras un «Continuar» rechazado el foco sale del campo y `Escribir` no teclea. Haz `Clic 0.683 0.353` y `Teclas Ctrl+A Retroceso` antes de cada `Escribir`.
- **Coordenadas:** Inicio: Jugar (0,219; 0,540), Créditos (0,123; 0,647), Salir (0,314; 0,646). Salida: «Quedarme» (0,495; 0,646), «Cerrar el juego» (0,633; 0,646). Perfiles: campo (0,683; 0,353), Continuar (0,558; 0,894), Volver (0,410; 0,896), ▲ (0,409; 0,222), ▼ (0,447; 0,222), filas en y = 0,305, 0,419 y 0,534 con x ≈ 0,25. Menú de niveles: Nivel 3 (0,831; 0,604), Volver (0,085; 0,896).
- **Esperas:** `Clic -ConFoto -FotoMs 700–1500` basta dentro de una escena; tras cambiar de escena, 1500–2000 ms. `Esperar-Carga` después del clic sí encuentra la línea.
- **Player.log:** `Revisar-Log` copia solo el último lanzamiento como `<sesión>_Player.txt`; los anteriores los guarda `Start-Juego` como `_lanz1_`, `_lanz2_`.
- `Medidas` puede contar menos cargas que el log: para cifras, usa las líneas `RNF-04` del `Player.txt`.
- `python -I` no ve numpy ni PIL: usa `python`.
- El rótulo `G-SYNC` de la esquina superior derecha de las capturas es de NVIDIA y no es del juego.

## Tu reporte final
Dame, sin adornos: conteo de veredictos; cada caso que no sea P con su motivo; defectos con severidad; cifras previstas frente a las obtenidas; salidas y fallos del arnés; si el manifiesto sigue coincidiendo; datos para casos de otras sesiones; ajustes de procedimiento para la siguiente sesión; y lo que no hayas podido comprobar. Si aparece algo Bloqueante o Mayor, repórtalo tal cual: no lo corrijas.

## Lo aprendido en S-N1 y S-N2
- Las fotos «a +x s» solo son fiables con `-ConFoto -FotoMs` en el mismo clic, y salen entre 0,64 y 0,94 s después del final de la acción aunque se pida `-FotoMs 600`: para «+1 s» pide `-FotoMs 1000`. Una `Foto` aparte llega ≥ 1,3 s tarde.
- Para «esperar 30 s con la pausa abierta», mide con `acciones.tsv` entre el clic de pausa y el de «Reanudar».
- No definas funciones de PowerShell con nombres que coincidan con alias (`Gi`, `Mas`…).
- Una salida 3 durante un `Arrastrar` puede significar que el juego sí recibió la suelta: mira la pantalla antes de repetirlo, porque repetirlo sumaría un intento y alteraría las cifras.
- Si repites parte de la partida, usa otro nombre de sesión de arnés (por ejemplo `S-N3b-rc5`) para no sobrescribir la foto previa de residuos.
- Las líneas que los INC numeran se cuentan desde 0; compruébalo contra el texto de la línea.
- Las piezas de la cueva cambian de sitio en cada entrada: toma las coordenadas de la foto de cada entrada. En un rejuego hacen falta dos «Omitir» para llegar de Apertura a la cueva.
- El laberinto cambia en cada entrada; una ejecución de 13 bloques tarda ≈ 22 s: para ver la llegada, ráfaga de 30 s desde el clic en «Ejecutar».

## Lo aprendido en S-N3 y S-PAU
- Un clic en la fila de un perfil entra directo al menú de niveles, sin «Continuar». `OE4_E` llega a mástil y vela con 19 clics.
- Con `OE4_B2`, `OE4_B3` y `OE4_D` no hay «Omitir»: en un bucle de «Continuar» comprueba antes de cada clic la línea `Level… cargó` del `Player.log` para no pasarte de escena.
- `Rafaga` y `Sostener` se excluyen por el mutex del arnés. Para ver una acción en curso sirve un script en segundo plano que copie la pantalla, guardado en tu scratchpad y no en el repo.
- Recolección del N3 desde la salida, retención en ms: tronco_2 →250; tronco_3 ↑540 + →190; tronco_4 ←260; mástil ←314 + ↓257; tronco_1 ↓570; sogas →257 + ↓257; tela →257 + ↑200; tronco_5 →286 + ↓143; a la zona ↑500. «Recoger» en (0,888; 0,895).
- La superposición de NVIDIA «Presione Alt+Z…» sale unos segundos tras cada arranque, arriba a la derecha. Si aparece un cartel de Chrome Remote Desktop sobre un botón o el cursor se mueve solo, anótalo con la hora y espera; no cierres nada que no sea del juego.
- Tampoco uses `Mv` como nombre de función.

## Lo aprendido en S-OMI
- El equipo tiene ahora un solo monitor de 1920×1080: la ventana sale sin marco (`-popupwindow`).
- El `Intervalo` del arnés no es el real: `-Intervalo 70` es el mínimo y deja ≈ 135 ms entre pulsaciones. Mídelo contra un clic simple (≈ 239 ms) en `acciones.tsv`.
- Si hay que encadenar dos acciones a tiempo, ponlas en el mismo bloque de PowerShell, sin fotos entre medias. Una `Rafaga` justo tras una acción arranca ≈ 0,8 s tarde.

## Lo aprendido en S-PER
- Laberinto, calibración medida: x = 216 + 60,2·c, y = 845 − 61,3·r, en píxeles de 1920×1080.
- Reanudar el N2 sale con la apertura del bosque: hacen falta 19 «Continuar» (o «Omitir» ×3, ×2 y ×1 hasta bosque, taller y laberinto, con el nivel terminado). N3 con `OE4_E`: 19 «Continuar». N1 sin terminar: 36.
- Para detectar escenas en un bucle, lee el `Player.log` con `FileShare.ReadWrite` y busca `RNF-04: [^\r\n]*<Escena>`.
