# S-PER · rc3 · resumen

- **Versión:** rc3, HEAD `f49ccc9`, SHA-256 `6cd16edf…adab`. 10/10/2026, 19:46–20:04. Doce lanzamientos del `.exe` (cada bloque: `Preparar-Datos`, juego, `Matar`, relanzado, `Revisar-Log`).
- **Casos:** 7. **Veredictos: P 7 · PD 0 · F 0 · B 0 · NA 0.** Detalle en `veredictos.csv`.

## Resultado por bloque
- **PF-RNF14-01** (N2, `Matar` en `N2_Escena22`): relanza entra directo en el taller; el JSON `{2,1}` no cambia.
- **PF-RNF14-02** (N3, ensamblaje en mástil y vela): vuelve al mismo punto, con base y amarres, tareas 1–3 y Mamá en la zona.
- **PF-RNF14-03** (N1, `Matar` en `N1_NacimientoDelFuego`): perfil abre sin error, N1 por completar y N2 bloqueado; rejugado, N1 completo y N2 abierto.
- **PF-RNF14-04** (N2, `Matar` en `N2_Escena25_Cierre`): N2 «Completado» y N3 desbloqueado sin rejugar; la 2.5 no ofrece «Omitir» hasta que se muestra su resumen; el JSON conserva las fases originales (`{2,3}` 0/0/9 de la primera confirmación).
- **PF-RNF14-05** (pausa abierta, `Matar`): el taller empieza de cero; `{2,1}` intacta.
- **PF-RF02-04** (JSON corrupto): «No se pudo abrir ese perfil. Avisa a tu profe.»; el juego sigue operable y el corrupto se borra con su papelera. **PF-RF02-05** («CON»): se crea `CON.json` y el perfil funciona.

## Medidas y límites
- Los cierres forzados se hicieron con `taskkill /F /T`; en los bloques 14-01, 14-02, 14-04 y 14-05 el JSON de antes y después es idéntico (diff); el 14-03 se comprobó por el estado de los niveles.
- En PF-RNF14-04 la repetición del N2 exigió jugar bosque, taller y laberinto otra vez (todas las fases ya estaban confirmadas, el juego repite la pedida).
- RNF-04: cargas de 0,012 a 0,259 s; RNF-05: privada máx. 1.090 MiB (`S-PER6`, `Narrative`); red: 0 conexiones; log sin `Exception` en los doce lanzamientos.

## Defectos
- Ninguno nuevo.
