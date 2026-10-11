# Orden de ejecución del capítulo 9 (trabajo de grado)

Requiere Word libre (ningún otro agente con Word abierto) y autorización expresa de Santiago para tocar el `.docx`. Todo desde la raíz del proyecto. Cada script deja copia en `%TEMP%\Algoritmia-Docx` y aborta sin guardar si algo falla; si algo aborta, no se fuerza: se restaura el respaldo.

```
$D = "docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx"
$P = "claudeDocs/entregables/tools"

# 0. Regenerar los JSON si se editó cap9-evaluacion.md (se niega si queda PENDIENTE)
python claudeDocs/entregables/TG/cap9_a_json.py

# 1. Comprobar (no escribe nada)
pwsh -NoProfile -File "$P/reemplazar_parrafos.ps1" -Docx $D -Pares "$P/pares/tg_cap9_reemplazos.json" -DryRun
pwsh -NoProfile -File "$P/insertar_parrafos.ps1"   -Docx $D -Parrafos "$P/pares/tg_cap9_insertar.json" -DryRun
pwsh -NoProfile -File "$P/insertar_tabla.ps1"      -Docx $D -Tablas "$P/pares/tg_cap9_tablas.json" -DryRun   # las anclas aun no existen: lo avisa y sigue

# 2. Copia para el verificador y aplicación real, en este orden
Copy-Item $D "$env:TEMP\tg_antes_cap9.docx"
pwsh -NoProfile -File "$P/reemplazar_parrafos.ps1" -Docx $D -Pares "$P/pares/tg_cap9_reemplazos.json"
pwsh -NoProfile -File "$P/insertar_parrafos.ps1"   -Docx $D -Parrafos "$P/pares/tg_cap9_insertar.json"
pwsh -NoProfile -File "$P/insertar_tabla.ps1"      -Docx $D -Tablas "$P/pares/tg_cap9_tablas.json"

# 3. Verificar los reemplazos (compara con la copia de antes del paso 2; los párrafos y tablas nuevos del cap. 9 aparecerán como diferencias esperadas)
python "$P/verificar_reemplazos.py" --docx $D --original "$env:TEMP\tg_antes_cap9.docx" --pares "$P/pares/tg_cap9_reemplazos.json"

# 4. Páginas: el capítulo 9 debe quedar en 10 o menos (ideal 8) y el total en 90 o menos
pwsh -NoProfile -File "$P/medir_paginas.ps1" -Docx $D

# 5. Regenerar el .md y pasar los greps de la guía
$env:PYTHONIOENCODING = "utf-8"
python -m markitdown "docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx" > "docs/md/Trabajo_de_Grado_Entrega_Plantilla_28jul.md"
# greps a) a e) de claudeDocs/entregables/Guia-redaccion-sin-marcas-de-IA.md, apartado 6, sobre el capitulo 9 del .md
```

`tg_cap9_insertar.json` trae al final dos entradas del Anexo G (renglón en la LISTA DE ANEXOS tras la 1.ª aparición de «Anexo F…», y renglón al final de la sección ANEXOS tras el formato del Anexo F), que usan los parámetros `ocurrencia` y `como_ocurrencia` de `insertar_parrafos.ps1`. El DryRun debe confirmar que el último párrafo del formato del Anexo F coincide.

Después de insertar: comprobar que los pies Tabla 12 a Tabla 16 llevan campo SEQ (no texto), que la tabla de contenido y las listas de tablas y figuras traen el capítulo 9 y que «Lo pendiente pasa al cuarto objetivo…» (cierre de 8.3) sigue coherente.

## Celdas y frases que cambian cuando Santiago entregue sus resultados

Los espacios «[Pendiente: …]» de la Tabla 12 (SON), la Tabla 14 (fila CT-02), la Tabla 16 entera, el párrafo de PF-RNF13-01 (9.2), el de los pendientes (9.2), 9.3 (primer y tercer párrafo) y el último párrafo de 9.3.1. Si se elige otra letra que Anexo G, buscar y reemplazar en `cap9-evaluacion.md` y regenerar.
