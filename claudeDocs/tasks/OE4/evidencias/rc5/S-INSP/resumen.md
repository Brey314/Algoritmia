# S-INSP · rc5 · resumen (05/11/2026)

Candidato rc5 (`ca6fd91`), huella `ab0c386e`. Parte documental: sin Unity, sin el ejecutable y sin escritura fuera de `evidencias/rc5/S-INSP/` (salvo los defectos nuevos al final de `evidencias/rc5/defectos.md`, DEF-RC5-15 a DEF-RC5-19). Veredictos en `veredictos.csv` (11 filas).

**Árbol inspeccionado.** `git diff --stat ca6fd91 HEAD -- Assets Packages ProjectSettings` solo cambia `Assets/Game/Art/Inventario.md`; se leyó el árbol de trabajo. La suite `rc5-06a5b61` también difiere de `ca6fd91` solo en ese archivo.

| Veredicto | N | Casos |
|---|---|---|
| P | 7 | PF-RNF01-01, PF-RNF03-01, PF-RNF19-01, PF-RNF21-01, PF-RNF22-01, PF-RNF23-01, PF-RNF12-01 |
| F | 3 | PF-RNF15-01, PF-RNF17-01, PF-RNF20-01 |
| B | 1 | PF-RNF02-01 |

## Por caso
- **PF-RNF01-01 (P).** 355 cadenas de 53 archivos y 126 literales de código: 492 oraciones, máximo 20 palabras, ninguna de más de 20. Los cinco tecnicismos y «patrón» se explican en su contexto. Observación: «Eso es pensar como un algoritmo.» del N3 no define el término en el N3 (lo hace el N2).
- **PF-RNF02-01 (B).** Cueva, bosque y orilla probaron teclas y rueda sobre la escena viva y no pasó nada (cueva: 18.916 px de diferencia solo en los personajes que respiran y el botón de ayuda; hojas y piedras 0 px; no se abrió la pausa). **Taller y laberinto no probaron teclas en rc5**: el guion no trae el paso (DEF-RC5-18) y S-PAU solo pulsó Esc con la pausa abierta. SUITE: 28 de 28 `*_RNF02_` y `Architecture_INC01`, incluidas `Controls_RNF02_LasCincoEscenasJugables…` y `RiverScene_INC01_…`.
- **PF-RNF03-01 (P).** 22 capturas a pantalla completa vistas, una por fase y estado listado, cuatro narrativas de N1 y la 3.1 del N3. La tablilla del bosque (DEF-RC3-10) está corregida: texto de dos líneas dentro de la tablilla (x 64-1164, y 64-204), sin objetos ni contador debajo. SUITE: 35 de 35 `*_RNF03_`.
- **PF-RNF15-01 (F).** Identificadores en español: ForestSceneController 20, WorkshopSceneController 2 y MazeSceneController 1 (clase y línea en `rnf15-identificadores-en-ingles.txt`). Los otros tres criterios cumplen en las 10 clases. DEF-RC5-16.
- **PF-RNF17-01 (F).** Denominador de código = commits con algún `.cs` bajo `Assets/` (92 de 131; el resto es arte, sonido, datos, escenas o documentación). 9 con tarjeta (9,8 %), 3 con marcador `<id>` (3,3 %), 80 sin nada (87,0 %). Lectura literal (131): 6,9 %, 3,1 % y 90,1 %. DEF-RC5-17.
- **PF-RNF19-01 (P).** Once estados vistos en gris; ninguno sin captura. El más flojo: la alerta del amarre (DEF-RC5-19, Observación).
- **PF-RNF20-01 (F).** 62 medidas; 61 con 4,5:1 o más. Falla «Arrastra un bloque a tu secuencia» (laberinto, cajón abierto): 3,15:1. DEF-RC5-15.
- **PF-RNF21-01 (P).** Dos ráfagas de S-N1: máximo 1 oscilación de más del 10 % por ventana de 1 s (el fundido a negro y de vuelta). SUITE: 24 de 24 `*_RNF21_`.
- **PF-RNF22-01, PF-RNF23-01, PF-RNF12-01 (P, sin repetir).** INSP de textos y entornos hecha sobre rc5; la confirmación de Santiago y las autorizaciones firmadas constan en `veredictos-rc3.csv` (16/10/2026) y en el trabajo de grado; no se abrieron los `.docx`.

## SUITE (`suites/rc5-06a5b61`, HEAD `06a5b61` + árbol `c599b82e13ec`)
796 EditMode (795 + 1 omitida) y 438 PlayMode, 1234 de 1234 listadas ejecutadas. De los XML: `*_RNF02_`/`INC01` 28/28, `*_RNF03_` 35/35, `*_RNF19_` 12/12, `*_RNF20_` 7/7, `*_RNF21_` 24/24 (`suite-rc5-pruebas-relevantes.txt`).

## Límites
- No se corrió Unity ni el ejecutable. Las capturas de taller y laberinto con teclas no existen.
- `cueva_5_antes.png` y `cueva_5_despues.png` solo estaban en `%TEMP%\Algoritmia-OE4\fotos` y se copiaron a `capturas/`.
- PF-RNF03-01: no hay captura a pantalla completa de cada estado intermedio (rechazos, bosque con cinco troncos, ejecución del laberinto); se miró una por fase y estado listado.
- PF-RNF21-01: ráfagas a 10 fps, luminancia global.
- PF-RNF20-01: las cajas de muestreo las fijé a mano mirando cada captura; el texto es el 3 % de píxeles más lejano del fondo en cada banda.
- PF-RNF15-01: el recuento de español es por barrido de una lista de palabras y lectura de cada línea; puede haber algún término que la lista no cubra.
- Sin ráfagas de la fogata de las narrativas (solo la cueva y el cierre).
