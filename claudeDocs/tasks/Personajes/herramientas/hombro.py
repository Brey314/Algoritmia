#!/usr/bin/env python3
# hombro.py: DONDE GIRA EL HOMBRO del arte final de la familia, y como se comprueba (Santiago, 06/10/2026).
#
#     python3 claudeDocs/tasks/Personajes/herramientas/hombro.py                      # propone el pivote de los cuatro y lo compara con arte_final.json
#     python3 claudeDocs/tasks/Personajes/herramientas/hombro.py --aplica             # y lo escribe en arte_final.json (solo el pivote de BrazoIzq/BrazoDer)
#     python3 claudeDocs/tasks/Personajes/herramientas/hombro.py --verifica           # mide lo que HAY en arte_final.json (sale con 1 si no cumple)
#     python3 claudeDocs/tasks/Personajes/herramientas/hombro.py --autoprueba         # la busqueda y la medida detectan un hombro mal puesto
#         ids: papa mama nina nino (por defecto, los cuatro)
#
# EL PROBLEMA. Las capas que entrego Santiago dibujan cada humero como una capsula que arranca en el CENTRO DEL PECHO (con el brazo en A): el
# extremo redondo de ese humero es el «hombro» que media preparar_arte_final.py. Un humero que gira alrededor de ahi cuelga del centro del
# pecho, y como va DETRAS del torso (INC-132) solo se ve si esta muy abierto: el reposo salia en A (Papa 54 grados, Mama 72, Nina 40, Nino 57).
# Un hombro de verdad esta en el BORDE del torso, un poco mas arriba. Mover el PIVOTE (el punto alrededor del que gira BrazoIzq/BrazoDer) no
# mueve el dibujo: el rect de cada pieza no cambia, solo el punto fijo del giro (PlacePart de BuildRigsFinal.cs.txt, modo «sprites»). El codo
# no depende del pivote: es el centro del extremo distal del humero, un punto del lienzo.
#
# LA REGLA (busca). El pivote parte del centro del extremo redondo del humero (C0) y se corre HORIZONTALMENTE hacia fuera, subiendo
# SUBE px, de 2 en 2 px, mientras con el brazo colgando (el humero, de P a E, a 10, 18 y 30 grados con la vertical) se cumpla TODO esto:
#   (1) ARRIBA: lo que se VE del humero (lo que no tapan el torso ni, en Papa, la cabeza, que va delante de los humeros) no sube mas de
#       MAX_ARRIBA px por encima del borde de arriba del torso (el arranque del cuello): el extremo de arriba del humero, que queda tras el
#       torso, no asoma sobre el hombro;
#   (2) HUECO: entre el borde del torso y el borde del humero no se abre un hueco de mas de MAX_HUECO px en los 80 px bajo el hombro;
#   (3) CAPSULA: el pivote no se aleja del eje del humero mas de FRACCION_CAPSULA radios (el hombro cae dentro del brazo que dibuja el arte).
# El resultado es el ultimo punto que cumple. Ningun criterio es una cifra del juego (CP-03): son tolerancias de la herramienta.
# Para el arte de hoy sale (centro del extremo redondo -> pivote, izquierdo y derecho, px del lienzo del rig): Papa (448,483) -> (406,477) y
# (567,483) -> (611,477); Mama (499,543) -> (463,537) y (529,550) -> (563,544); Nina (489,537) -> (451,531) y (541,540) -> (577,534); Nino
# (489,567) -> (463,561) y (535,564) -> (559,558). Papa, Mama y Nina paran en (3) y el Nino en (1): su torso es solo el pecho y el humero sube.
#
# Y DESPUES. El pivote mueve solo el punto fijo del giro: el rect de la pieza, el codo (el centro del extremo distal del humero, un punto del lienzo
# que no depende del pivote) y el ancla del antebrazo no cambian, y el dibujo de la pose de entrega (giro 0) queda igual. BuildRigsFinal «sprites»
# aplica el pivote nuevo con PlacePart (anclas = el rect en fracciones del padre, pivote de la tabla): los hijos de BrazoX cuelgan de puntos del
# rect y no se mueven. Hay que correr, en este orden: hombro.py --aplica (y articulaciones.py, que --aplica ya corre), coreografia.py y pose_preview.py.
#
# Sin dependencias fuera de Pillow, como el resto de las herramientas. Coordenadas: las del lienzo de 1024 (y hacia abajo).

import argparse
import json
import math
import os
import sys

try:
    from PIL import Image, ImageChops
except ImportError:  # pragma: no cover
    sys.exit("hace falta Pillow: pip install pillow")

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import articulaciones as A  # noqa: E402
import prefabs as P  # noqa: E402

ESC = 0.5                 # las mascaras se miden a 512 px (1 px = 2 unidades del lienzo)
LADO = 512
SUBE = 6.0                # el pivote sube esto respecto al centro del extremo redondo (y se corre hacia fuera)
PASO = 2.0                # px del lienzo entre dos candidatos
S_MAX = 120.0
MAX_ARRIBA = 8.0          # px que el humero visible puede subir sobre el borde de arriba del torso
MAX_HUECO = 3.0           # px de hueco entre torso y humero
VENTANA_HUECO = 80.0      # filas, bajo el hombro, donde se mide el hueco
FRACCION_CAPSULA = 1.1    # el pivote cae a menos de esto en radios del eje del humero
ANGULOS = (10.0, 18.0, 30.0)  # el humero colgando: grados con la vertical (de P a E)
ANGULOS_HUECO = (10.0, 18.0)  # el hueco solo cuenta con el brazo cerca del cuerpo: a 30 grados se separa del torso y asi debe verse

C, T = P.C, P.T
LA, RA = P.LA, P.RA
HD = P.HD


# ----------------------------------------------------------------------------- mascaras


def mascara(imagen, rect):
    """La mascara 'L' (255 = opaco, alfa > 127) de una pieza puesta en su rect del lienzo, a ESC."""
    a = imagen.convert("RGBA").getchannel("A").point(lambda v: 255 if v > 127 else 0)
    w = max(1, int(round((rect[2] - rect[0]) * ESC)))
    h = max(1, int(round((rect[3] - rect[1]) * ESC)))
    capa = Image.new("L", (LADO, LADO), 0)
    capa.paste(a.resize((w, h), Image.BOX), (int(round(rect[0] * ESC)), int(round(rect[1] * ESC))))
    return capa


def _fila(m, y):
    """(x0, x1) de los pixeles de la fila y de una mascara, o None si no hay."""
    if y < 0 or y >= LADO:
        return None
    b = m.crop((0, y, LADO, y + 1)).getbbox()
    return None if b is None else (b[0], b[2])


class Pieza:
    """Lo que la medida necesita de un brazo de un personaje: la mascara del humero, la del torso y la de lo que lo tapa."""

    def __init__(self, lado, humero, torso, cabeza, centro, codo, radio):
        self.lado = lado
        self.humero = humero            # mascara del humero en reposo (el dibujo de la entrega)
        self.torso = torso
        self.cubre = torso if cabeza is None else ImageChops.lighter(torso, cabeza)   # lo que se dibuja DELANTE del humero
        self.centro = centro            # C0: el centro del extremo redondo proximal, el «hombro» de la entrega
        self.codo = codo                # E
        self.radio = radio
        b = torso.getbbox()
        self.y_torso = b[1] / ESC       # el borde de arriba del torso: el arranque del cuello
        # el torso por filas, para el hueco
        self.filas_torso = {y: _fila(torso, y) for y in range(LADO)}


def alfa_de(p, codo):
    """Grados que forma con la vertical hacia abajo el eje P -> E."""
    return math.degrees(math.atan2(abs(codo[0] - p[0]), codo[1] - p[1]))


def gira(m, grados, centro):
    """La mascara girada «grados» (+ = antihorario en pantalla, como el z de Unity) alrededor de «centro» (lienzo)."""
    return m.rotate(grados, resample=Image.BILINEAR, center=(centro[0] * ESC, centro[1] * ESC)).point(lambda v: 255 if v > 127 else 0)


def mide_pose(pz, p, theta):
    """
    El humero girado alrededor de p hasta que P -> E forme «theta» con la vertical. Devuelve {arriba, hueco, visible}:
      arriba   px que lo VISIBLE del humero sube sobre el borde de arriba del torso (negativo: queda por debajo; -1e9 si no se ve nada),
      hueco    el mayor hueco horizontal, en los VENTANA_HUECO px bajo el hombro, entre el borde del torso y el borde del humero,
      visible  fraccion de los pixeles del humero que no tapa el torso (ni, en Papa, la cabeza).
    """
    al = alfa_de(p, pz.codo)
    giro = (al - theta) if pz.lado == "Izq" else (theta - al)
    h = gira(pz.humero, giro, p)
    total = sum(h.histogram()[255:]) or 1
    vis = ImageChops.subtract(h, pz.cubre)
    n_vis = sum(vis.histogram()[255:])
    b = vis.getbbox()
    arriba = -1e9 if b is None else pz.y_torso - b[1] / ESC
    hueco = 0.0
    y0 = int(p[1] * ESC)
    for y in range(y0, min(LADO, y0 + int(VENTANA_HUECO * ESC))):
        t, a = pz.filas_torso.get(y), _fila(h, y)
        if t is None or a is None:
            continue
        g = (t[0] - a[1]) if pz.lado == "Izq" else (a[0] - t[1])
        if g > 0:
            hueco = max(hueco, g / ESC)
    return {"arriba": arriba, "hueco": hueco, "visible": n_vis / float(total)}


def mide(pz, p, angulos=ANGULOS):
    """El peor caso en los angulos de prueba: {arriba, hueco, visible (a 18 grados), capsula (radios del eje), por_angulo}."""
    por = {th: mide_pose(pz, p, th) for th in angulos}
    ux, uy = pz.codo[0] - pz.centro[0], pz.codo[1] - pz.centro[1]
    n = math.hypot(ux, uy)
    dist = abs((p[0] - pz.centro[0]) * uy - (p[1] - pz.centro[1]) * ux) / n
    return {"arriba": max(v["arriba"] for v in por.values()), "hueco": max(por[t]["hueco"] for t in angulos if t in ANGULOS_HUECO),
            "visible": por[18.0]["visible"] if 18.0 in por else 0.0, "capsula": dist / pz.radio, "por_angulo": por}


def busca(pz, sube=SUBE, max_arriba=MAX_ARRIBA, max_hueco=MAX_HUECO, fraccion=FRACCION_CAPSULA):
    """(pivote, medidas, motivo): el ultimo candidato horizontal que cumple (1), (2) y (3). Ver la cabecera."""
    sg = -1.0 if pz.lado == "Izq" else 1.0
    mejor, medidas, motivo = None, None, "fin"
    s = 0.0
    while s <= S_MAX:
        p = (pz.centro[0] + sg * s, pz.centro[1] - sube)
        m = mide(pz, p)
        if m["capsula"] > fraccion:
            motivo = "capsula"
            break
        if m["arriba"] > max_arriba:
            motivo = "arriba"
            break
        if m["hueco"] > max_hueco:
            motivo = "hueco"
            break
        mejor, medidas = p, m
        s += PASO
    if mejor is None:  # ni el centro, subido SUBE px, cumple: se queda donde esta
        mejor = tuple(pz.centro)
        medidas = mide(pz, mejor)
    return mejor, medidas, motivo


# ----------------------------------------------------------------------------- el arte del repo


def _imagen(carpeta, sprite):
    ruta = P._png_de(carpeta, sprite)
    if ruta is None:
        raise FileNotFoundError("falta %s en Assets/Game/Art/Characters/%s" % (sprite, carpeta))
    return Image.open(ruta).convert("RGBA")


def piezas_de(pid, tabla):
    """
    {lado: Pieza} de un personaje de la familia, con el arte que hay en Assets/Game/Art/Characters/<Carpeta>/ y las medidas de «tabla»
    (la entrada del personaje en arte_final.json o en rig_articulaciones.json: nodos y partes). C0 y el radio salen del ALFA del humero
    (el centro del extremo redondo que queda lejos del codo), no del pivote guardado: asi la regla no depende de donde estaba el pivote.
    """
    import pose_preview as V  # solo para extremos_capsula
    carpeta = P.PERSONAJES[pid][1]
    partes = {q["nombre"]: q for q in tabla["partes"]}
    nodos = {n["nombre"]: n for n in tabla["nodos"]}
    torso_q = partes["Torso"]
    torso = mascara(_imagen(carpeta, torso_q["sprite"]), torso_q["rect"])
    orden = A.orden_tronco_de(pid)
    cab_nodo = nodos.get("Cuello")
    cabeza = None
    if cab_nodo is not None and "Cuello" in orden and "BrazoIzq" in orden and orden.index("Cuello") > orden.index("BrazoIzq"):
        cabeza = mascara(_imagen(carpeta, cab_nodo["sprite"]), cab_nodo["rect"])   # la cabeza va DELANTE de los humeros (Papa)
    out = {}
    for lado in ("Izq", "Der"):
        q, codo = partes["Brazo" + lado], nodos["Codo" + lado]["punto"]
        im = _imagen(carpeta, q["sprite"])
        a, b, r = V.extremos_capsula(im)
        w, h = im.size
        rw, rh = q["rect"][2] - q["rect"][0], q["rect"][3] - q["rect"][1]
        f = lambda p: (q["rect"][0] + p[0] * rw / w, q["rect"][1] + p[1] * rh / h)
        a, b = f(a), f(b)
        centro = max((a, b), key=lambda p: math.hypot(p[0] - codo[0], p[1] - codo[1]))   # el extremo redondo que queda lejos del codo
        out[lado] = Pieza(lado, mascara(im, q["rect"]), torso, cabeza, centro, tuple(codo), r * rw / w)
    return out


def cargar_tabla(pid, ruta=None):
    """La entrada del personaje en arte_final.json (o en otro JSON de la misma forma)."""
    with open(ruta or A.ARTE_FINAL_JSON, encoding="utf-8") as f:
        return json.load(f)["personajes"][pid]


def verifica(pid, tabla=None):
    """
    Mide el pivote que HAY en la tabla: [(lado, pivote, medidas, ok, por_que)]. Cumple (1), (2) y (3) de la regla en los tres angulos.
    El pivote es el de las partes BrazoIzq y BrazoDer.
    """
    tabla = tabla or cargar_tabla(pid)
    pzs = piezas_de(pid, tabla)
    partes = {q["nombre"]: q for q in tabla["partes"]}
    out = []
    for lado, pz in pzs.items():
        p = tuple(float(v) for v in partes["Brazo" + lado]["pivote"])
        m = mide(pz, p)
        malos = []
        if m["capsula"] > FRACCION_CAPSULA + 0.02:
            malos.append("el pivote cae a %.2f radios del eje (maximo %.2f)" % (m["capsula"], FRACCION_CAPSULA))
        if m["arriba"] > MAX_ARRIBA + 2.0:
            malos.append("el humero visible sube %.0f px sobre el torso (maximo %.0f)" % (m["arriba"], MAX_ARRIBA))
        if m["hueco"] > MAX_HUECO + 2.0:
            malos.append("hueco de %.0f px entre torso y brazo (maximo %.0f)" % (m["hueco"], MAX_HUECO))
        out.append((lado, p, m, not malos, "; ".join(malos)))
    return out


# ----------------------------------------------------------------------------- principal


def _linea(pid, lado, c0, p, m, motivo=""):
    pa = m["por_angulo"]
    return "%-5s %-3s C0 (%5.0f,%5.0f) -> pivote (%5.0f,%5.0f)  desplaza %3.0f px, sube %2.0f | arriba %s hueco %s px | humero visible a 18: %3.0f %% | pivote a %.2f radios del eje %s" % (
        pid, lado, c0[0], c0[1], p[0], p[1], abs(p[0] - c0[0]), c0[1] - p[1],
        "/".join("%+.0f" % pa[a]["arriba"] for a in ANGULOS), "/".join("%.0f" % pa[a]["hueco"] for a in ANGULOS),
        100.0 * m["visible"], m["capsula"], ("(la busqueda paro por: %s)" % motivo) if motivo else "")


def _autoprueba():
    """
    La busqueda y la medida detectan un hombro mal puesto. Con el arte del Nino: (a) el centro del extremo redondo (donde estaba) tiene el humero
    casi oculto a 18 grados y no se pasa de «arriba» ni de «hueco», pero el pivote que busca la regla queda mas afuera; (b) un pivote 30 px mas
    afuera que el de la regla se pasa de «arriba»; (c) un pivote muy lejos abre un hueco; (d) uno fuera de la capsula falla la regla (3).
    """
    malos = 0
    pzs = piezas_de("nino", cargar_tabla("nino"))
    pz = pzs["Izq"]
    p, m, _ = busca(pz)
    centro = mide(pz, tuple(pz.centro))
    casos = [
        ("el pivote de la regla esta mas afuera que el centro del extremo redondo", p[0] < pz.centro[0] - 8),
        ("el pivote de la regla cumple arriba y hueco", m["arriba"] <= MAX_ARRIBA and m["hueco"] <= MAX_HUECO),
        ("en el centro del extremo redondo se ve menos del 25 %% del humero a 18 grados (%.0f %%)" % (100 * centro["visible"]), centro["visible"] < 0.25),
        ("30 px mas afuera el humero sube de mas (%.0f px)" % mide(pz, (p[0] - 30, p[1]))["arriba"], mide(pz, (p[0] - 30, p[1]))["arriba"] > MAX_ARRIBA),
        ("80 px mas afuera se abre un hueco (%.0f px)" % mide(pz, (p[0] - 80, p[1]))["hueco"], mide(pz, (p[0] - 80, p[1]))["hueco"] > MAX_HUECO),
        ("un pivote a 3 radios del eje no cumple la regla (3)", mide(pz, (pz.centro[0] - 20, pz.centro[1] - 3 * pz.radio))["capsula"] > FRACCION_CAPSULA),
    ]
    # el lado derecho es el espejo en lo esencial: el pivote queda a la derecha del centro
    pd, md, _ = busca(pzs["Der"])
    casos.append(("el pivote del lado derecho queda a la derecha del centro", pd[0] > pzs["Der"].centro[0] + 8))
    for nombre, ok in casos:
        malos += 0 if ok else 1
        print("  %-86s %s" % (nombre, "ok" if ok else "FALLA"))
    print("autoprueba de hombro.py:", "pasa" if not malos else "FALLA en %d casos" % malos)
    return 1 if malos else 0


def main(argv=None):
    ap = argparse.ArgumentParser(description="Pivote del hombro del arte final (ver la cabecera).")
    ap.add_argument("ids", nargs="*", help="papa mama nina nino (por defecto, los cuatro)")
    ap.add_argument("--aplica", action="store_true", help="escribe el pivote propuesto en arte_final.json (solo BrazoIzq y BrazoDer)")
    ap.add_argument("--verifica", action="store_true", help="mide el pivote que hay en arte_final.json (sale con 1 si no cumple)")
    ap.add_argument("--autoprueba", action="store_true")
    a = ap.parse_args(argv)
    if a.autoprueba:
        return _autoprueba()
    ids = a.ids or list(P.FAMILIA)
    for pid in ids:
        if pid not in P.FAMILIA:
            ap.error("personaje desconocido: %s" % pid)
    if a.verifica:
        fallos = 0
        for pid in ids:
            for lado, p, m, ok, por in verifica(pid):
                pa = m["por_angulo"]
                print("%-5s %-3s pivote (%5.0f,%5.0f) | arriba %s hueco %s px a 10/18/30 | humero visible a 18: %3.0f %% | %.2f radios del eje | %s" % (
                    pid, lado, p[0], p[1], "/".join("%+.0f" % pa[t]["arriba"] for t in ANGULOS), "/".join("%.0f" % pa[t]["hueco"] for t in ANGULOS),
                    100.0 * m["visible"], m["capsula"], "ok" if ok else "FALLA: " + por))
                fallos += 0 if ok else 1
        print("los hombros cumplen la regla" if not fallos else "%d hombros no cumplen" % fallos)
        return 1 if fallos else 0
    with open(A.ARTE_FINAL_JSON, encoding="utf-8") as f:
        doc = json.load(f)
    cambios = 0
    for pid in ids:
        tabla = doc["personajes"][pid]
        pzs = piezas_de(pid, tabla)
        partes = {q["nombre"]: q for q in tabla["partes"]}
        for lado, pz in pzs.items():
            p, m, motivo = busca(pz)
            nuevo = [int(round(p[0])), int(round(p[1]))]
            print(_linea(pid, lado, pz.centro, p, m, motivo) + ("" if nuevo == partes["Brazo" + lado]["pivote"] else "  [en la tabla: %s]" % partes["Brazo" + lado]["pivote"]))
            if a.aplica and nuevo != partes["Brazo" + lado]["pivote"]:
                partes["Brazo" + lado]["pivote"] = nuevo
                cambios += 1
    if a.aplica:
        if cambios:
            A.guardar_arte_final(doc["personajes"])
            # rig_articulaciones.json sale de arte_final.json: se regenera en otro proceso (articulaciones.py lee la tabla al importarse)
            import subprocess
            subprocess.call([sys.executable, "-B", os.path.join(AQUI, "articulaciones.py")], stdout=subprocess.DEVNULL)
        print("arte_final.json: %d pivotes escritos%s" % (cambios, " y rig_articulaciones.json regenerado (falta coreografia.py)" if cambios else ""))
    return 0


if __name__ == "__main__":
    sys.exit(main())
