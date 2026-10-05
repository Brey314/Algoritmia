#!/usr/bin/env python3
# Estima la tabla de articulaciones de los siete personajes y la escribe en
# rig_articulaciones.json, que lee BuildRigsFinal.cs.txt (modos «nodos» y «sprites»).
#
# Sin dependencias externas: lee los prefabs YAML de Assets/Game/Prefabs/Characters/ y no abre
# ninguna imagen. Los valores son PROVISIONALES: salen del rect y el pivote de las partes que el
# prefab ya tiene (arte provisional de cinco partes), y de unas pocas medidas a ojo sobre la
# reconstrucción de las partes (las marcadas «a ojo» abajo). Con el arte final se edita el JSON a
# mano, o se ajustan las constantes de este archivo y se vuelve a correr:
#
#     python3 claudeDocs/tasks/Personajes/herramientas/articulaciones.py
#
# Coordenadas: las de cut.py, en el lienzo de 1024 unidades con el origen arriba a la izquierda y
# la y hacia ABAJO. La figura va de y = 77 (coronilla) a y = 947 (suelo).
#
# Por qué el JSON no lleva «segmentado»: se deduce del prefab. Un personaje está segmentado
# cuando la Image de AntepiernaIzq tiene sprite y está encendida; guardarlo también en la tabla
# sería una segunda fuente de verdad que se desincroniza al primer cambio de arte.

import json
import os
import re
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
RAIZ = os.path.abspath(os.path.join(AQUI, "..", "..", "..", ".."))
PREFABS = os.path.join(RAIZ, "Assets", "Game", "Prefabs", "Characters")
SALIDA = os.path.join(AQUI, "rig_articulaciones.json")
LIENZO = 1024.0
SUELO = 947.0  # y del suelo: el pivote de Cuerpo en la familia

# ----------------------------------------------------------------------------- lectura del YAML


def leer_prefab(ruta):
    """Devuelve {ruta_del_nodo: (min_x, min_y, max_x, max_y, pivote_x, pivote_y)} en coordenadas
    del lienzo con la y hacia abajo. Solo mira RectTransform (clase 224) y GameObject (clase 1)."""
    docs = {}
    actual = None
    with open(ruta, encoding="utf-8") as f:
        for linea in f:
            m = re.match(r"--- !u!(\d+) &(-?\d+)", linea)
            if m:
                actual = {"cls": int(m.group(1)), "lineas": []}
                docs[m.group(2)] = actual
            elif actual is not None:
                actual["lineas"].append(linea.rstrip("\n"))

    def campo(doc, nombre):
        for linea in doc["lineas"]:
            s = linea.strip()
            if s.startswith(nombre + ":"):
                return s[len(nombre) + 1:].strip()
        return None

    def vec(texto):
        n = re.findall(r"-?\d+(?:\.\d+)?(?:e-?\d+)?", texto)
        return float(n[0]), float(n[1])

    def fileid(texto):
        return re.search(r"fileID: (-?\d+)", texto).group(1)

    nombres = {k: campo(d, "m_Name") for k, d in docs.items() if d["cls"] == 1}
    nodos = {}
    for k, d in docs.items():
        if d["cls"] != 224:
            continue
        nodos[k] = {
            "nombre": nombres[fileid(campo(d, "m_GameObject"))],
            "padre": fileid(campo(d, "m_Father")),
            "amin": vec(campo(d, "m_AnchorMin")),
            "amax": vec(campo(d, "m_AnchorMax")),
            "pos": vec(campo(d, "m_AnchoredPosition")),
            "size": vec(campo(d, "m_SizeDelta")),
            "pivote": vec(campo(d, "m_Pivot")),
        }

    cajas = {}

    def caja(k):
        """Misma matemática que BoxOf en BuildRigsFinal.cs.txt: solo anclas, pivote y tamaño,
        sin transforms del mundo (en un prefab abierto no hay Canvas). Coordenadas con la y hacia arriba."""
        if k in cajas:
            return cajas[k]
        n = nodos[k]
        if n["nombre"] == "Lienzo":
            res = ((0.0, 0.0), (LIENZO, LIENZO))
        else:
            (pmin, psize) = caja(n["padre"])
            tam = tuple((n["amax"][i] - n["amin"][i]) * psize[i] + n["size"][i] for i in (0, 1))
            ref = tuple(n["amin"][i] + n["pivote"][i] * (n["amax"][i] - n["amin"][i]) for i in (0, 1))
            piv = tuple(pmin[i] + ref[i] * psize[i] + n["pos"][i] for i in (0, 1))
            res = (tuple(piv[i] - n["pivote"][i] * tam[i] for i in (0, 1)), tam)
        cajas[k] = res
        return res

    def ruta_de(k):
        partes = []
        while k in nodos:
            partes.append(nodos[k]["nombre"])
            k = nodos[k]["padre"]
        return "/".join(reversed(partes))

    salida = {}
    for k, n in nodos.items():
        try:
            (mn, tam) = caja(k)
        except KeyError:
            continue  # la raíz del prefab y lo que cuelga de ella fuera de Lienzo (Estela)
        # a coordenadas del lienzo con la y hacia abajo
        x0, x1 = mn[0], mn[0] + tam[0]
        y0, y1 = LIENZO - (mn[1] + tam[1]), LIENZO - mn[1]
        px = x0 + n["pivote"][0] * tam[0]
        py = y1 - n["pivote"][1] * tam[1]
        ruta = ruta_de(k)
        ruta = ruta.split("/", 1)[1] if "/" in ruta else ruta  # sin el nombre de la raíz
        salida[ruta] = (x0, y0, x1, y1, px, py)
    return salida


def entero(v):
    return int(round(v))


def punto(x, y):
    return [entero(x), entero(y)]


# ----------------------------------------------------------------------------- la familia

# Qué parte de la figura (de y = 77 a y = 947) ocupa la cabeza hasta la barbilla. La regla de la
# dirección de arte (§7.2) es 1/3 en adultos y 2/5 en niños; medida sobre las partes, la cabeza
# real llega más abajo (el torso lleva la cabeza dentro), así que se usa lo medido. «a ojo».
#   Papá: barbilla en y≈415 · Mamá: ≈345 · Niña: ≈490 · Niño: ≈505
FRACCION_CABEZA = {"papa": 0.385, "mama": 0.31, "nina": 0.47, "nino": 0.49}

# Ojos y boca dentro de la cabeza: (cx, cy, ancho, alto) en el lienzo. «a ojo» sobre la
# reconstrucción de las partes; el ojo incluye las cejas.
CARA = {
    "papa": {"ojos": (512, 252, 170, 100), "boca": (512, 335, 130, 60)},
    "mama": {"ojos": (512, 225, 170, 80), "boca": (512, 292, 110, 50)},
    "nina": {"ojos": (512, 372, 200, 110), "boca": (512, 432, 100, 50)},
    "nino": {"ojos": (512, 342, 230, 120), "boca": (512, 425, 150, 60)},
}

FAMILIA = [
    ("papa", "Papa", "Father"),
    ("mama", "Mama", "Mother"),
    ("nina", "Nina", "Girl"),
    ("nino", "Nino", "Boy"),
]

T = "Lienzo/Cuerpo/Tronco"
C = "Lienzo/Cuerpo"


def caja_de(rect):
    return [entero(rect[0]), entero(rect[1]), entero(rect[2]), entero(rect[3])]


def rect_centrado(cx, cy, w, h):
    return [entero(cx - w / 2), entero(cy - h / 2), entero(cx + w / 2), entero(cy + h / 2)]


def personaje_familia(pid, prefab, carpeta):
    g = leer_prefab(os.path.join(PREFABS, prefab + ".prefab"))
    pref = "char_" + pid
    nodos = []
    partes = []

    # Brazos: codo en el punto medio entre el hombro (pivote) y la esquina del rect más lejana a
    # él (la punta de la mano); el antebrazo es el cuadrante distal, entre el codo y esa esquina.
    for lado, sufijo in (("Izq", "izq"), ("Der", "der")):
        x0, y0, x1, y1, px, py = g[T + "/Brazo" + lado]
        esquina = max(((x0, y0), (x1, y0), (x0, y1), (x1, y1)),
                      key=lambda c: (c[0] - px) ** 2 + (c[1] - py) ** 2)
        ex, ey = (px + esquina[0]) / 2, (py + esquina[1]) / 2
        distal = [min(ex, esquina[0]), min(ey, esquina[1]), max(ex, esquina[0]), max(ey, esquina[1])]
        nodos.append({
            "nombre": "Codo" + lado, "tipo": "articulacion", "padre": T + "/Brazo" + lado,
            "punto": punto(ex, ey), "imagen": "Antebrazo" + lado,
            "sprite": "%s_parte_antebrazo_%s" % (pref, sufijo), "rect": caja_de(distal),
        })
        partes.append({
            "nombre": "Brazo" + lado, "ruta": T + "/Brazo" + lado,
            "sprite": "%s_parte_brazo_%s" % (pref, sufijo),
            "rect": caja_de((x0, y0, x1, y1)), "pivote": punto(px, py),
        })

    # Piernas: rodilla en el punto medio entre la cadera (pivote) y el centro de la base del rect;
    # la antepierna es la mitad inferior del rect, de lado a lado. (La esquina más lejana, como en
    # el brazo, desplazaría la rodilla ~60 unidades hacia fuera: las piernas son verticales.)
    for lado, sufijo in (("Izq", "izq"), ("Der", "der")):
        x0, y0, x1, y1, px, py = g[C + "/Pierna" + lado]
        kx, ky = (px + (x0 + x1) / 2) / 2, (py + y1) / 2
        nodos.append({
            "nombre": "Rodilla" + lado, "tipo": "articulacion", "padre": C + "/Pierna" + lado,
            "punto": punto(kx, ky), "imagen": "Antepierna" + lado,
            "sprite": "%s_parte_antepierna_%s" % (pref, sufijo), "rect": caja_de((x0, ky, x1, y1)),
        })
        partes.append({
            "nombre": "Pierna" + lado, "ruta": C + "/Pierna" + lado,
            "sprite": "%s_parte_pierna_%s" % (pref, sufijo),
            "rect": caja_de((x0, y0, x1, y1)), "pivote": punto(px, py),
        })

    # Torso (hoy lleva la cabeza dentro): se conserva su rect; con el arte final se recorta.
    tx0, ty0, tx1, ty1, tpx, tpy = g[T + "/Torso"]
    partes.append({
        "nombre": "Torso", "ruta": T + "/Torso", "sprite": pref + "_parte_torso",
        "rect": caja_de((tx0, ty0, tx1, ty1)), "pivote": punto(tpx, tpy),
    })

    # Cabeza: del ancho del torso (el pelo es lo más ancho), desde la coronilla hasta la barbilla.
    # El cuello gira en la base de la cabeza, un poco por dentro para que no se vea el corte.
    alto_figura = SUELO - ty0
    barbilla = ty0 + FRACCION_CABEZA[pid] * alto_figura
    alto = barbilla - ty0
    cx = (tx0 + tx1) / 2
    cuello = (cx, barbilla - 0.08 * alto)
    cabeza = [tx0, ty0, tx1, barbilla]
    nodos.append({
        "nombre": "Cuello", "tipo": "articulacion", "padre": T,
        "punto": punto(*cuello), "imagen": "Cabeza", "sprite": pref + "_parte_cabeza",
        "rect": caja_de(cabeza),
    })
    padre_cara = T + "/Cuello/Cabeza"
    for nombre, clave, sprite in (("Ojos", "ojos", pref + "_ojos_neutra"), ("Boca", "boca", pref + "_boca_0")):
        ox, oy, ow, oh = CARA[pid][clave]
        nodos.append({
            "nombre": nombre, "tipo": "imagen", "padre": padre_cara,
            "punto": punto(ox, oy), "imagen": nombre, "sprite": sprite,
            "rect": rect_centrado(ox, oy, ow, oh),
        })

    return {
        "id": pid, "prefab": prefab, "carpeta": carpeta, "prefijo": pref, "guia": False,
        "nodos": nodos, "partes": partes,
    }


# ----------------------------------------------------------------------------- Algoritm

# Medidas «a ojo» sobre char_algoritm_n1_fuego_reposo.png (768 px, que el Image con
# preserveAspect pinta a 1024). La rueda y la gota son el mismo dibujo recoloreado (forms.py), así
# que la geometría es una sola. Las posiciones son píxeles del lienzo de 1024; el script las
# convierte en fracciones del rect de Cuerpo, que en los prefabs es el lienzo entero.
ALG = {
    "hombro_izq": (292, 612), "hombro_der": (735, 612),
    "muneca_izq": (165, 765), "muneca_der": (860, 757),
    "mano_izq": (10, 598, 300, 912),   # rect del brazo entero (hombro a punta de los dedos)
    "mano_der": (730, 598, 1014, 912),
    "cadera_izq": (455, 800), "cadera_der": (570, 800),
    "pierna_izq": (390, 795, 480, 1002),
    "pierna_der": (550, 795, 640, 1002),
    "cuerpo": (250, 20, 780, 812),     # la llama y el vientre de colores, sin extremidades
    "pivote_tronco": (512, 700),       # a la altura del vientre: de ahí gira el gesto de hablar
    "ojos": (370, 380, 655, 505),
    "boca": (410, 505, 610, 595),
}

FORMAS = [
    ("fuego", "Algoritm_Fuego"),
    ("rueda", "Algoritm_Rueda"),
    ("gota", "Algoritm_Gota"),
]


def personaje_guia(forma, prefab):
    g = leer_prefab(os.path.join(PREFABS, prefab + ".prefab"))
    x0, y0, x1, y1, _, _ = g[C]
    ancho, alto = x1 - x0, y1 - y0

    def px(x):
        return x0 + x / LIENZO * ancho

    def py(y):
        return y0 + y / LIENZO * alto

    def pt(p):
        return (px(p[0]), py(p[1]))

    def rc(r):
        return [px(r[0]), py(r[1]), px(r[2]), py(r[3])]

    pref = "char_algoritm_" + forma
    nodos = []

    for lado, sufijo in (("Izq", "izq"), ("Der", "der")):
        cadera = pt(ALG["cadera_" + sufijo])
        nodos.append({
            "nombre": "Pierna" + lado, "tipo": "imagen", "padre": C,
            "punto": punto(*cadera), "imagen": "Pierna" + lado,
            "sprite": "%s_parte_pierna_%s" % (pref, sufijo), "rect": caja_de(rc(ALG["pierna_" + sufijo])),
        })

    pivote = pt(ALG["pivote_tronco"])
    nodos.append({
        "nombre": "Tronco", "tipo": "grupo", "padre": C, "punto": punto(*pivote),
        "imagen": "", "sprite": "", "rect": caja_de((x0, y0, x1, y1)),
    })

    for lado, sufijo in (("Izq", "izq"), ("Der", "der")):
        hombro = pt(ALG["hombro_" + sufijo])
        nodos.append({
            "nombre": "Brazo" + lado, "tipo": "imagen", "padre": T,
            "punto": punto(*hombro), "imagen": "Brazo" + lado,
            "sprite": "%s_parte_brazo_%s" % (pref, sufijo), "rect": caja_de(rc(ALG["mano_" + sufijo])),
        })

    nodos.append({
        "nombre": "Torso", "tipo": "imagen", "padre": T, "punto": punto(*pivote),
        "imagen": "Torso", "sprite": pref + "_parte_torso", "rect": caja_de(rc(ALG["cuerpo"])),
    })
    for nombre, clave, sprite in (("Ojos", "ojos", pref + "_ojos_neutra"), ("Boca", "boca", pref + "_boca_0")):
        r = rc(ALG[clave])
        nodos.append({
            "nombre": nombre, "tipo": "imagen", "padre": T,
            "punto": punto((r[0] + r[2]) / 2, (r[1] + r[3]) / 2), "imagen": nombre,
            "sprite": sprite, "rect": caja_de(r),
        })

    # Codos: en el punto medio entre el hombro y la muñeca (el brazo es un palo recto); el
    # antebrazo es el cuadrante distal, entre el codo y la esquina lejana del rect (la mano).
    for lado, sufijo in (("Izq", "izq"), ("Der", "der")):
        hombro, muneca = pt(ALG["hombro_" + sufijo]), pt(ALG["muneca_" + sufijo])
        r = rc(ALG["mano_" + sufijo])
        ex, ey = (hombro[0] + muneca[0]) / 2, (hombro[1] + muneca[1]) / 2
        esquina = max(((r[0], r[1]), (r[2], r[1]), (r[0], r[3]), (r[2], r[3])),
                      key=lambda c: (c[0] - hombro[0]) ** 2 + (c[1] - hombro[1]) ** 2)
        distal = [min(ex, esquina[0]), min(ey, esquina[1]), max(ex, esquina[0]), max(ey, esquina[1])]
        nodos.append({
            "nombre": "Codo" + lado, "tipo": "articulacion", "padre": T + "/Brazo" + lado,
            "punto": punto(ex, ey), "imagen": "Antebrazo" + lado,
            "sprite": "%s_parte_antebrazo_%s" % (pref, sufijo), "rect": caja_de(distal),
        })

    # Rodillas: mismo criterio que la familia (punto medio entre cadera y centro de la base).
    for lado, sufijo in (("Izq", "izq"), ("Der", "der")):
        cadera = pt(ALG["cadera_" + sufijo])
        r = rc(ALG["pierna_" + sufijo])
        kx, ky = (cadera[0] + (r[0] + r[2]) / 2) / 2, (cadera[1] + r[3]) / 2
        nodos.append({
            "nombre": "Rodilla" + lado, "tipo": "articulacion", "padre": C + "/Pierna" + lado,
            "punto": punto(kx, ky), "imagen": "Antepierna" + lado,
            "sprite": "%s_parte_antepierna_%s" % (pref, sufijo), "rect": caja_de((r[0], ky, r[2], r[3])),
        })

    return {
        "id": "algoritm_" + forma, "prefab": prefab, "carpeta": "Algoritm", "prefijo": pref,
        "guia": True, "nodos": nodos, "partes": [],
    }


# ----------------------------------------------------------------------------- escritura


def compacto(valor, nivel=0):
    """JSON con indentación, pero con las listas de números en una sola línea. Solo ASCII
    (ensure_ascii=True): el lector de BuildRigsFinal.cs.txt no debe tropezar con tildes ni eñes."""
    sangria = "  " * nivel
    if isinstance(valor, dict):
        if not valor:
            return "{}"
        filas = ['%s  %s: %s' % (sangria, json.dumps(k, ensure_ascii=True), compacto(v, nivel + 1))
                 for k, v in valor.items()]
        return "{\n" + ",\n".join(filas) + "\n" + sangria + "}"
    if isinstance(valor, list):
        if all(isinstance(v, (int, float)) for v in valor):
            return "[" + ", ".join(json.dumps(v) for v in valor) + "]"
        filas = ["%s  %s" % (sangria, compacto(v, nivel + 1)) for v in valor]
        return "[\n" + ",\n".join(filas) + "\n" + sangria + "]"
    return json.dumps(valor, ensure_ascii=True)


def main():
    personajes = [personaje_familia(*f) for f in FAMILIA]
    personajes += [personaje_guia(*f) for f in FORMAS]
    # La nota va sin tildes ni eñes (y el volcado fuerza ASCII): el JSON debe ser ASCII puro.
    tabla = {
        "version": 1,
        "nota": "Valores PROVISIONALES (articulaciones.py). Lienzo de 1024, origen arriba a la izquierda, "
                "y hacia abajo. 'nodos' se ANADE a los prefabs (padre antes que hijo); 'partes' describe los "
                "nodos que ya existen. tipo: articulacion = pivote de tamano 0 en 'punto' con un segmento "
                "'imagen' colgado de el; imagen = el propio nodo lleva la Image; grupo = nodo estirado sin Image. "
                "El estado 'segmentado' no se guarda: se deduce del prefab.",
        "personajes": personajes,
    }
    with open(SALIDA, "w", encoding="utf-8", newline="\n") as f:
        f.write(compacto(tabla) + "\n")
    print("escrito", os.path.relpath(SALIDA, RAIZ), "con", len(personajes), "personajes")
    for p in personajes:
        print(" ", p["id"], len(p["nodos"]), "nodos,", len(p["partes"]), "partes")
    return 0


if __name__ == "__main__":
    sys.exit(main())
