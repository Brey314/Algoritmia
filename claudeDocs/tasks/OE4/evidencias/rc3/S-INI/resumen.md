# S-INI · rc3 · resumen

- **Versión:** rc3, HEAD `f49ccc9` (árbol limpio), SHA-256 del `.exe` `6cd16edf…adab`; huella de la carpeta `61612836…bb9f` (`../../build-rc3.md`).
- **Fecha:** 10/10/2026, 17:27–17:33. Ejecutor EXE con `oe4.ps1`, ventana de 1920×1080 sin marco, tres lanzamientos (S-INI-rc3, _lanz2 y _lanz3 en esta carpeta).
- **Casos:** 14 (todos los de la tabla de S-INI). **Veredictos: P 14 · PD 0 · F 0 · B 0 · NA 0.** Detalle en `veredictos.csv`.
- **Pasos 1–21 ejecutados**, con las semillas `OE4_B` y los ocho perfiles (`OE4_A, B, B2, B3, C, D, E, Z`).

## Lo nuevo del 10/10 que se vio
- **Portada (PF-RF01-03):** los cinco personajes de frente en el bosque, enteros; respiran con fases distintas (ráfaga `PF-RF01-03_rafaga.csv`). Disposición lateral, no «detrás del título» (DEF-RC3-02).
- **Créditos:** Algoritm final (llama con pantaloneta) saluda en bucle, brazos detrás del cuerpo, sin tapar la lista; todos los textos exactos.
- **Paginación de perfiles:** 3+3+2, flechas ▲/▼ en los dos sentidos, la lista no se mueve con rueda ni arrastre.

## Medidas de la sesión
- RNF-04: MainMenu 0,052–0,123 s, Credits 0,022 s, LevelSelect 0,024–0,029 s. Boot→MainMenu ≈ 3,4 s.
- RNF-05: privada máx 779 MiB, trabajo máx 405 MiB. RNF-10: 143 muestras, ninguna conexión.
- Registro del juego sin `Exception`, `Error` ni `Assert` (tres lanzamientos).
- Residuos (`residuos.txt`): `Player.log` en LocalLow y valores `unity.*`/`unity_connect.*`/`Screenmanager*` en `HKCU\Software\Universidad Catolica de Colombia\Algoritmia`; nada fuera de eso.

## Defectos
Ninguno funcional. Observaciones: DEF-RC3-02 (texto de casos.md sobre la portada).

## Notas del arnés (no son defectos del juego)
- Un aviso de NVIDIA Superposición («Presione Alt+Z…») cae unos segundos sobre la esquina superior derecha tras arrancar; aparece en `PF-RF01-01_1.png`.
- `Restaurar-Registro` termina con salida 2 («The type of the value object did not match the specified RegistryValueKind», línea 1502 de `oe4.ps1`); borra los valores nuevos pero no repone los cambiados. Los valores son de Unity (sesión) y de pantalla.
- La rueda y el arrastre no mueven la lista de Créditos: sí la barra (observación de RNF-02, paso 5).
