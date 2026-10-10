# ANEXO A. MATRIZ DE CASOS DE PRUEBA FUNCIONAL POR REQUERIMIENTO

*Anexo A del entregable del cuarto objetivo específico del trabajo de grado. Documento principal: Solucion_OE4_Evaluacion_prototipo.docx.*

Esta matriz no se redactó a mano: la genera `tools/build.py` desde el catálogo de casos (`casos.md`) y desde los veredictos de la pasada sobre rc3 (`src/veredictos-rc3.csv`). Aplica la restricción CT-10, según la cual todo requerimiento tiene al menos un caso de prueba. Cada fila es un caso, ordenado por el requerimiento que su identificador nombra: primero los RF-01 a RF-47, después los RNF-01 a RNF-23 y al final los casos de la restricción CT-02 y de sonido. Un caso puede verificar además otros requerimientos que su guion cita; esos aparecen en `casos.md`.

La columna «Etiqueta» es la del indicador de eficacia (NAV, BOT, SON, RETO, DAT, RNF) y la columna «Ejecutor» dice quién obtuvo el veredicto (apartado 2.3 del documento principal). El veredicto es uno de P, PD, F, B o NA (apartado 2.4).

**Tabla A.1.** Casos de prueba funcional por requerimiento, con el veredicto obtenido sobre rc3.

{{MATRIZ}}
