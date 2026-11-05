# ANEXO A. MATRIZ DE CASOS DE PRUEBA FUNCIONAL POR REQUERIMIENTO

*Anexo A del entregable del cuarto objetivo específico del trabajo de grado. Documento principal: Solucion_OE4_Evaluacion_prototipo.docx.*

Esta matriz no se redactó a mano: la genera `tools/build.py` desde el catálogo de casos (`casos.md`) y desde los veredictos consolidados (`src/veredictos-rc5.csv`, con `src/veredictos-rc3.csv` como referencia). Aplica la restricción CT-10, según la cual todo requerimiento tiene al menos un caso de prueba. Cada fila es un caso, ordenado por el requerimiento que su identificador nombra: primero los RF-01 a RF-47, después los RNF-01 a RNF-23 y al final los casos de la restricción CT-02 y de sonido. Un caso puede verificar además otros requerimientos que su guion cita; esos aparecen en `casos.md`.

La segunda columna da el requerimiento, la etiqueta del indicador de eficacia (NAV, BOT, SON, RETO, DAT, RNF), la sesión y quién obtiene el veredicto (apartado 2.3 del documento principal). Las columnas rc3 y rc5 traen el veredicto de cada pasada; el de rc5 es uno de P, PD, F o NE (apartado 2.4). «Sin caso» en la columna rc3 marca los nueve casos nuevos, y «Sin ejecutar» en la fecha, los tres de sonido. La precondición y el resultado esperado salen de `casos.md`; la fecha, el candidato, el resultado observado y el registro, de las sesiones de rc5. Cuando un caso se juzgó en varias sesiones, el observado las separa con «//» y el veredicto es el peor. Las rutas del registro son relativas a `claudeDocs/tasks/OE4/evidencias/`, salvo las que empiezan por `Assets/`, `claudeDocs/` o `docs/`, relativas a la raíz del repositorio.

Todas las filas de rc5 son de la misma versión: etiqueta `rc5`, commit `{{COMMIT}}`, huella global `{{HUELLA_GLOBAL}}`.

**Tabla A.1.** Casos de prueba funcional por requerimiento, con los veredictos de rc3 y rc5 y el detalle de rc5.

{{MATRIZ}}

```{=openxml}
<w:p><w:pPr><w:sectPr><w:pgSz w:w="15840" w:h="12240" w:orient="landscape"/><w:pgMar w:top="720" w:right="720" w:bottom="720" w:left="720" w:header="360" w:footer="360" w:gutter="0"/></w:sectPr></w:pPr></w:p>
```

# LISTA DE HUELLAS DEL PAQUETE RC5

El paquete rc5 se identifica por su versión, su commit y la huella de todo su contenido, porque `Algoritmia.exe` es solo un lanzador con la misma huella en todas las versiones. La Tabla A.2 lista sus {{N_ARCHIVOS}} archivos con su tamaño y su SHA-256, tal como están en `claudeDocs/tasks/OE4/evidencias/rc5/manifiesto-rc5.tsv`.

- Versión: rc5, etiqueta anotada `rc5`.
- Commit: `{{COMMIT}}`.
- Huella global: `{{HUELLA_GLOBAL}}`, sobre {{N_ARCHIVOS_HUELLA}} archivos.
- Huella de `Algoritmia_Data`: `{{HUELLA_DATA}}`.
- SHA-256 de `Build/Algoritmia.zip`: `{{SHA_ZIP}}`.

La huella global es el SHA-256 de la lista ordenada de líneas «sha256 *./ruta» de todos los archivos de la tabla menos el marcado; el comando exacto está en `claudeDocs/tasks/OE4/evidencias/build-rc5.md`. El único archivo fuera de ella pertenece a `Algoritmia_BurstDebugInformation_DoNotShip`.

**Tabla A.2.** Archivos del paquete rc5 con su tamaño en bytes y su SHA-256.

{{HUELLAS}}
