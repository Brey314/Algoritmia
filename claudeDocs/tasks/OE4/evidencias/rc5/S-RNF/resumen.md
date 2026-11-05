# S-RNF · Medidas sobre el ejecutable (rc5) · 05/11/2026

Candidato rc5 (compilado desde `ca6fd91`), huella global `ab0c386e87045a4c525548109fa5f9e1905a7336dbd14f21f93b97c8bcb246c3`, SHA-256 del exe `6cd16edf…adab`. Equipo 1 (monitor único 1920×1080). Hora local UTC-5, ≈ 12:17–12:25. Sesiones de arnés `S-RNF-rc5` (3 lanzamientos, arranque) y `S-RNF07-rc5` (copia en el escritorio). Unity y Word cerrados al empezar y al terminar; `Start-Juego` no avisó de contaminación.

**Alcance:** los veredictos son solo del equipo 1. Las cifras del equipo 2 (RNF-04, RNF-05) son de la parte humana, aún no hecha.

## Conteo
P 5 · NE 1 (PF-RNF08-01) · F 0 · B 0.

## Fuentes y exclusiones
- 42 `*_Player.txt` únicos (deduplicados por contenido) y 31 `*_mem.csv`/`*_red.csv` de `S-INI, S-N1, S-N2A, S-N2C, S-N3, S-PAU, S-OMI, S-PER, S-DOC, GP2`, más `S-RNF` y `S-RNF07`. S-N2B no dejó logs propios.
- Excluidos: `GP2\interrumpido\` y `evidencias\rc5\GP\` (16/10, Word abierto).
- Ningún `resumen.md` de las demás sesiones declara contaminación (cada una dice que `Start-Juego` no avisó y que Unity y Word estaban cerrados). Por tanto no se excluyó ninguna sesión más.
- Consolidación: `consolidacion_RNF04_05_10.txt` (script en el scratchpad, no en el repo).

## PF-RNF04-01 · Carga de escena (límite 10 s)
Todas las escenas llegaron a ≥ 3 mediciones limpias con lo que ya habían dejado las sesiones; solo se midió aparte el arranque.

| Escena | n | Peor (s) | Sesión del peor | Media (s) |
|---|---|---|---|---|
| Boot→MainMenu (arranque, 3 lanzamientos propios) | 3 | 1,48 | S-RNF-rc5 lanz1 | 1,42 |
| MainMenu (línea `RNF-04`, al volver de otras pantallas) | 9 | 0,121 | S-DOC-rc5 lanz1 | 0,085 |
| LevelSelect | 56 | 0,086 | S-PER7 | 0,065 |
| Credits | 5 | 0,037 | GP-rc5c | 0,033 |
| Narrative | 188 | 0,714 | GP-rc5c | 0,252 |
| LevelSummary | 18 | 0,036 | S-PER4b | 0,033 |
| TeacherReport | 4 | 0,074 | S-DOC11 | 0,064 |
| Level1_Cave | 17 | 0,091 | GP-rc5b | 0,075 |
| Level2_Forest | 8 | 0,054 | GP-rc5c | 0,047 |
| Level2_Workshop | 11 | 0,070 | S-PER5 | 0,052 |
| Level2_Maze | 10 | 0,071 | S-PAU | 0,058 |
| Level3_River | 18 | 0,055 | S-PER2b | 0,051 |

**Cómo se midió Boot→MainMenu:** `Boot` no pasa por `SceneLoader`; es el tiempo entre el inicio del proceso (`Process.StartTime`) y la última escritura de `Player.log`, que en ese momento termina en la línea `RNF-04: «MainMenu»`: 1,477; 1,384 y 1,410 s (la línea de `SceneLoader` dio 0,126; 0,132 y 0,141 s). Es una cota del tiempo hasta que MainMenu está cargada, no el «primer cuadro visible» (el arnés no lo mide); la foto del lanzamiento 1 mostró el inicio completo. Las 42 líneas «primer MainMenu de cada log» (0,12–0,14 s) concuerdan. El cronómetro humano (equipo 2) no se sustituye con esto.

## PF-RNF05-01 · Memoria (límite 2048 MiB)
- Máximo `WorkingSet64`: **474,6 MiB** (497,7 MB), escena `Narrative`, sesión S-N1-rc5. El pico de sistema (`peak_working_set_b`) llega a 475,9 MiB.
- Máximo `PrivateMemorySize64`: **1223,2 MiB** (1282,6 MB), escena `Level1_Cave`, sesión S-N1-rc5; es el que más se acerca al tope (60 % de 2048).
- «Escena» es la última que registró `SceneLoader` al tomar la muestra. El detalle por escena está en el archivo de consolidación.

## PF-RNF06-01 · Tamaño (límite 500 MB)
`Tamano` sobre `Build\Algoritmia_rc5`, sin `Datos/` ni `*_DoNotShip`: **426 935 666 bytes = 426,9 MB (10^6) = 407,2 MiB (2^20)**; margen de 73,1 MB (10^6) frente a 500 MB. Desglose (MiB): `Algoritmia_Data` 341,1 · archivos de la raíz 52,8 · `MonoBleedingEdge` 8,7 · `D3D12` 4,5 · `Licencias` 0,0. Con `Algoritmia_BurstDebugInformation_DoNotShip` (784 239 bytes): 407,9 MiB. Una suma independiente con `Get-ChildItem` da el mismo total. Coincide con `build-rc5.md`. El manifiesto trae 253 archivos (252 en la huella + 1 de `_DoNotShip`), coherente con esa nota. El «MB» del arnés es decimal.

## PF-RNF07-01 · Portabilidad (parte de esta sesión)
Copia idéntica (253 archivos, sin `Datos/`) en `C:\Users\benab\Desktop\Algoritmia prueba\`. `OE4_EXE` apuntando a la copia con `OE4_MANIFIESTO` puesto: la guarda pasó. El proceso `Algoritmia.exe` (pid 4240) tenía token **no elevado** (comprobado con `GetTokenInformation`/`TokenElevation`) y no pidió instalar nada. Con el perfil «RNF» creó `Datos\RNF.json` (43 bytes) junto al exe; 37 intentos de «Continuar» (uno sin enviarse por foco) desde la apertura del N1 hasta `Level1_Cave` (0,089 s), sin `Exception`. Residuos fuera de `Datos/`: solo `Player.log` en LocalLow y el registro del reproductor (restaurado), como en las demás sesiones. **RNF-07 exige también PF-RNF07-02 (humano).**
Nota: `%USERPROFILE%\Desktop` **no existía** en este equipo (el escritorio real está en `D:\User\Desktop`); la copia creó `C:\Users\benab\Desktop\`. Al terminar se borró solo `…\Desktop\Algoritmia prueba\` (ruta comprobada con `Resolve-Path`) y la carpeta `Desktop` vacía que se había creado.

## PF-RNF08-01
NE: exige modo avión, que activa una persona (Hoja-HUM H4).

## PF-RNF10-01 · Red
31 `*_red.csv`, 12 536 muestras (cada 2 s), todas «NINGUNA»: no se observó ninguna conexión TCP ni UDP, ni externa ni loopback. Límites: el muestreo empieza tras el arranque y una conexión de < 2 s puede caer entre muestras.
Inspección de `ca6fd91` (observaciones, no cambian el veredicto):
- `UnityConnectSettings.asset`: `m_Enabled: 0`; Insights, Cloud Diagnostics, Analytics, Ads, Purchasing y PerformanceReporting en 0. Quedan URLs de Unity escritas en el archivo, inertes con `m_Enabled: 0`.
- `ProjectSettings.asset`: `submitAnalytics: 0`; `scriptingDefineSymbols.Standalone: APP_UI_EDITOR_ONLY;SENTIS_ANALYTICS_ENABLED`. **Sí está `SENTIS_ANALYTICS_ENABLED`** (lo repone `com.unity.ai.inference` y, según CLAUDE.md, solo compila código de Editor del paquete); riesgo residual bajo y sin conexiones observadas.
- Paquete: `DirectML.dll` (14,0 MB) en la raíz y `D3D12\D3D12Core.dll` (4,7 MB) presentes.

## Defectos
Ninguno nuevo (DEF-RC5-15 sin usar).

## Límites
- RNF-04 y RNF-05 solo del equipo 1; el arnés no mide el «primer cuadro no negro».
- RNF-08 sin ejecutar.
- La «escena» de cada muestra de memoria es la última cargada, no necesariamente la visible.

## Cierre
`Revisar-Log` sin `Exception`/`Error`/`Assert`; `Residuos` hecho; `Restaurar-Registro` de `S-RNF-rc5` y `S-RNF07-rc5`; `Preparar-Datos -SinCarpeta` (sin `Datos/` en `Build\Algoritmia_rc5`); `manifiesto.ps1 -Verificar` coincide (253 archivos, misma huella); ningún `Algoritmia.exe`, Unity ni Word en marcha. Salidas del arnés: todas 0 salvo una 3 por foco (un «Continuar» no se envió porque Windows Terminal tomó el primer plano; el bucle siguió).
