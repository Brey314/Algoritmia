#!/usr/bin/env python3
"""Genera los JSON de inserción del capítulo 9 desde cap9-evaluacion.md (la fuente).

    python claudeDocs/entregables/TG/cap9_a_json.py

Escribe en claudeDocs/entregables/tools/pares/:
  tg_cap9_insertar.json  párrafos para insertar_parrafos.ps1 (títulos y cuerpo, en orden)
  tg_cap9_tablas.json    tablas para insertar_tabla.ps1 (cada una tras el párrafo que la precede)
Se niega a escribir si queda una marca PENDIENTE o un párrafo con saltos de línea.
"""
import json
import re
import sys
from pathlib import Path

AQUI = Path(__file__).resolve().parent
SALIDA = AQUI.parent / "tools" / "pares"
FUENTE = AQUI / "cap9-evaluacion.md"

# Último párrafo del apartado 8.3: ancla de la primera entrada y modelo del estilo Normal.
CIERRE_8_3 = ("Lo pendiente pasó al cuarto objetivo: correr la carpeta portable en otro equipo y en uno sin tarjeta "
              "gráfica dedicada, jugar sin conexión a internet, cronometrar un recorrido completo hecho por una "
              "persona y observar a estudiantes jugando para cerrar PG-05 y PG-06, con el consentimiento informado "
              "de sus acudientes (RNF-12, Anexo F). Esas comprobaciones se hicieron el 13/10/2026 y sus resultados "
              "están en el apartado 9.2.")
MODELOS = {  # estilo -> texto completo de un párrafo del capítulo 8 con ese estilo
    "Título 1": "8. DESARROLLO DEL PROTOTIPO FUNCIONAL",
    "Título 2": "8.1 Desarrollo",
    "Título 3": "8.1.1 Organización del trabajo",
    "Normal": CIERRE_8_3,
}
# Tabla del capítulo 8 con la misma cantidad de columnas (título del pie, sin «Tabla N. ») y anchos en %.
MODELO_TABLA = {3: "Metas del tercer objetivo y resultado obtenido",
                4: "Presupuestos de rendimiento y estado de su medición",
                5: "Resultado de la suite de pruebas del 1 de octubre de 2026"}
ANCHOS = {"Metas del cuarto objetivo y resultado obtenido": [30, 25, 45],
          "Veredictos de rc3 por sesión": [58, 12, 30],
          "Presupuestos de rendimiento y de paquete en rc3": [22, 12, 33, 33],
          "Defectos hallados por severidad y estado al cierre de la pasada sobre rc3": [20, 20, 20, 20, 20],
          "Comprobaciones pendientes de Santiago Benavides Rey al 10 de octubre de 2026 y espacio para sus resultados": [34, 24, 42]}


def bloques(texto):
    """[(estilo, [líneas])] a partir de los comentarios «estilo: X»."""
    out, estilo, buf = [], None, []
    for linea in texto.splitlines():
        m = re.fullmatch(r"<!-- estilo: (.+?) -->", linea.strip())
        if m:
            if estilo:
                out.append((estilo, buf))
            estilo, buf = m.group(1), []
        elif estilo and linea.strip():
            buf.append(linea.rstrip())
    if estilo:
        out.append((estilo, buf))
    return out


def tabla(lineas):
    filas = []
    for l in lineas:
        celdas = [c.strip() for c in l.strip().strip("|").split("|")]
        if all(re.fullmatch(r"-+", c) for c in celdas):
            continue
        filas.append(celdas)
    n = {len(f) for f in filas}
    assert len(n) == 1, f"filas de distinto ancho: {n}"
    return filas


def main():
    texto = FUENTE.read_text(encoding="utf-8")
    cuerpo = re.sub(r"<!--(?!\s*estilo:).*?-->", "", texto, flags=re.S)  # quita el comentario de cabecera
    if "PENDIENTE" in cuerpo:
        sys.exit("quedan marcas PENDIENTE en el texto")
    bl = bloques(cuerpo)
    parrafos, tablas = [], []
    previo = CIERRE_8_3  # último párrafo ya colocado (ancla de lo que sigue)
    for i, (estilo, lin) in enumerate(bl):
        if estilo in MODELOS:
            assert len(lin) == 1, f"un bloque {estilo} con {len(lin)} líneas"
            t = lin[0].lstrip("#").strip() if estilo.startswith("Título") else lin[0]
            parrafos.append({"despues_de": previo, "texto": t, "como": MODELOS[estilo]})
            previo = t
        elif estilo == "Descripción":  # pie «Tabla N. …» y filas en el mismo bloque; el «Fuente» es el siguiente
            titulo = re.sub(r"^Tabla \d+\.\s*", "", lin[0])
            filas = tabla(lin[1:])
            assert bl[i + 1][0] == "Fuente", "tras la tabla debe ir el bloque Fuente"
            tablas.append({"despues_de": previo, "titulo": titulo, "modelo": MODELO_TABLA[len(filas[0])],
                           "anchos": ANCHOS[titulo], "filas": filas, "fuente": bl[i + 1][1][0]})
    # Anexo G (OE4): una linea en la LISTA DE ANEXOS (1.ª aparicion de «Anexo F…», antes del GLOSARIO) y otra en la
    # seccion ANEXOS, tras el ultimo parrafo del formato del Anexo F; las dos con el formato del renglon del Anexo E.
    renglon_f = ("Anexo F. Formato de consentimiento informado para la sesión con estudiantes: autorización de los acudientes "
                 "y asentimiento del estudiante (RNF-12)")
    renglon_e = ("Anexo E. Solución del tercer objetivo específico: prototipo funcional y sus anexos "
                 "(Solucion_OE3_Prototipo_funcional.docx)")
    renglon_g = ("Anexo G. Solución del cuarto objetivo específico: evaluación del prototipo "
                 "(Solucion_OE4_Evaluacion_prototipo.docx)")
    fin_formato_f = '☐ Lo leí yo. ☐ Me lo leyó: ________________________________'
    parrafos.append({"despues_de": renglon_f, "texto": renglon_g, "como": renglon_e, "ocurrencia": 1, "como_ocurrencia": 1})
    parrafos.append({"despues_de": fin_formato_f, "texto": renglon_g, "como": renglon_e, "ocurrencia": 1, "como_ocurrencia": 2})
    for t in tablas:
        assert sum(t["anchos"]) == 100 and len(t["anchos"]) == len(t["filas"][0]), t["titulo"]
    SALIDA.mkdir(parents=True, exist_ok=True)
    for nombre, datos in (("tg_cap9_insertar.json", parrafos), ("tg_cap9_tablas.json", tablas)):
        (SALIDA / nombre).write_text(json.dumps(datos, ensure_ascii=False, indent=1), encoding="utf-8")
    pal = sum(len(p["texto"].split()) for p in parrafos)
    cel = sum(len(c.split()) for t in tablas for f in t["filas"] for c in f)
    print(f"{len(parrafos)} párrafos ({pal} palabras), {len(tablas)} tablas ({cel} palabras de celdas)")


if __name__ == "__main__":
    main()
