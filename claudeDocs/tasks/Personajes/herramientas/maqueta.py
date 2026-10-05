#!/usr/bin/env python3
# maqueta.py: el «ARTE FINAL SIMULADO» de Papa, Mama y Nina, solo para la vista previa y la prueba de
# pose_preview.py. NO es codigo del juego ni escribe nada en el repo.
#
# Con el arte provisional cada brazo y cada pierna es UN sprite y la cabeza va dentro del torso. El arte
# final (como el del Nino) trae los brazos partidos en humero y antebrazo, las piernas en muslo y antepierna
# y la cabeza aparte. Esta maqueta lo SIMULA recortando el arte provisional por las articulaciones del
# rig_articulaciones.json, para comprobar la coreografia contra la geometria que tendra el arte final y no
# tener que esperarlo:
#
#   - brazo: un plano perpendicular al eje hombro-codo, por el punto de CodoIzq/CodoDer. El antebrazo es lo
#     que queda mas alla del plano; el humero, lo de aca mas una rotula (un disco del radio del brazo
#     alrededor del codo) que cubre la cuna que se abre en el lado de fuera cuando el codo se dobla;
#   - pierna: lo mismo con la rodilla (RodillaIzq/Der): antepierna = lo de abajo, muslo = lo de arriba mas
#     la rotula;
#   - cabeza: el torso se corta en horizontal por el punto de Cuello: la cabeza es lo de arriba (con 3 px
#     de solape), el torso lo de abajo.
#
# Cada pieza conserva el rect y la imagen COMPLETOS del sprite de origen (con el alfa a cero fuera de su
# parte): sumadas dan el sprite original. Los rects del JSON de AntebrazoIzq/AntepiernaIzq (que describen
# el arte final que llegara) siguen siendo los que usa coreografia.py para ubicar la mano y la punta del
# pie: la maqueta solo cambia lo que se DIBUJA. El Nino ya tiene arte final: la maqueta le devuelve su
# arbol tal cual.

import math
import os
import sys

from PIL import Image, ImageChops, ImageDraw

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import prefabs as P  # noqa: E402

SOLAPE_CABEZA = 3.0  # px que la cabeza se mete bajo el punto del cuello
SUPER = 4            # supermuestreo de las mascaras (bordes sin escalera)


class Pieza:
    """Un sprite en memoria con su rect del lienzo."""

    def __init__(self, rect, imagen):
        self.rect, self.imagen = tuple(rect), imagen


def _mascara_semiplano(tam, p0, u, signo, r_cap):
    """
    Mascara L (255 donde la pieza conserva el pixel): el semiplano {(p - p0) . u * signo >= 0}, mas el disco
    de radio r_cap alrededor de p0. Las coordenadas son las del sprite (pixeles de la imagen).
    """
    w, h = tam
    big = 4.0 * (w + h)
    n = (-u[1], u[0])
    s = SUPER
    m = Image.new("L", (w * s, h * s), 0)
    d = ImageDraw.Draw(m)
    uu = (u[0] * signo, u[1] * signo)
    pts = [(p0[0] + n[0] * big, p0[1] + n[1] * big), (p0[0] - n[0] * big, p0[1] - n[1] * big),
           (p0[0] - n[0] * big + uu[0] * big, p0[1] - n[1] * big + uu[1] * big),
           (p0[0] + n[0] * big + uu[0] * big, p0[1] + n[1] * big + uu[1] * big)]
    d.polygon([(x * s, y * s) for x, y in pts], fill=255)
    if r_cap > 0:
        d.ellipse([(p0[0] - r_cap) * s, (p0[1] - r_cap) * s, (p0[0] + r_cap) * s, (p0[1] + r_cap) * s], fill=255)
    return m.resize((w, h), Image.BOX)


def _mascara_rect(tam, rect, r):
    """Mascara L con 255 dentro del rect r del lienzo (el sprite es la imagen estirada a «rect»)."""
    w, h = tam
    x0, y0, x1, y1 = rect
    m = Image.new("L", (w, h), 0)
    ImageDraw.Draw(m).rectangle([(r[0] - x0) * w / (x1 - x0), (r[1] - y0) * h / (y1 - y0),
                                 (r[2] - x0) * w / (x1 - x0) - 1, (r[3] - y0) * h / (y1 - y0) - 1], fill=255)
    return m


def _alas_de_pelo(im, rect, cuello, cx, semi, tope=140):
    """
    Mascara L del pelo que cuelga POR DEBAJO del punto del cuello, fuera de los hombros (|x - cx| > 0.9 semi):
    es de la cabeza, y si fuera del torso, al inclinarse la cabeza se abriria un hueco blanco entre las dos
    piezas. Por cada columna baja desde el cuello mientras el pixel siga opaco y de un color parecido al del
    pelo justo encima (otro material —piel, vestido— corta la columna), y se afloja 3 pixeles mas para
    recoger el contorno.
    """
    w, h = im.size
    x0, y0, x1, y1 = rect
    kx, ky = (x1 - x0) / w, (y1 - y0) / h
    px = im.load()
    masc = Image.new("L", (w, h), 0)
    mp = masc.load()
    ny = int(round((cuello[1] - y0) / ky))
    for xi in range(w):
        if abs(x0 + xi * kx - cx) <= 0.9 * semi:
            continue
        muestras = [px[xi, y][:3] for y in range(max(0, ny - 14), ny - 2) if px[xi, y][3] > 200]
        if len(muestras) < 4:
            continue  # a la altura del cuello no hay pelo en esta columna
        ref = [sum(c[i] for c in muestras) / len(muestras) for i in range(3)]
        y = ny
        while y < min(h, ny + int(tope / ky)):
            r, g, b, a = px[xi, y]
            if a < 128 or abs(r - ref[0]) + abs(g - ref[1]) + abs(b - ref[2]) > 130:
                break
            mp[xi, y] = 255
            y += 1
        for extra in range(3):
            if y + extra < h and px[xi, y + extra][3] > 128:
                mp[xi, y + extra] = 255
    return masc


def _mitad(im, rect, e, u, signo, r_cap=0.0, suma=None, resta=None):
    """
    La parte de «im» (el sprite de un rect del lienzo) que queda en el lado «signo» del plano perpendicular a
    «u» por el punto del lienzo «e», con una rotula de radio r_cap (px del lienzo) alrededor de e.
    """
    w, h = im.size
    x0, y0, x1, y1 = rect
    k = ((x1 - x0) / w, (y1 - y0) / h)  # lienzo por pixel del sprite (1 con el arte actual)
    p0 = ((e[0] - x0) / k[0], (e[1] - y0) / k[1])
    m = _mascara_semiplano((w, h), p0, u, signo, r_cap / k[0])
    if suma is not None:
        m = ImageChops.lighter(m, suma)
    if resta is not None:
        m = ImageChops.subtract(m, resta)
    out = im.copy()
    out.putalpha(ImageChops.multiply(out.getchannel("A"), m))
    return out


def _radio(im, rect, e, u):
    """Medio ancho de la pieza en el punto e, medido perpendicular al eje u sobre el alfa (el radio de la rotula)."""
    a = im.getchannel("A")
    w, h = a.size
    x0, y0, x1, y1 = rect
    kx, ky = (x1 - x0) / w, (y1 - y0) / h
    n = (-u[1], u[0])

    def opaco(t):
        px, py = int(round((e[0] + n[0] * t - x0) / kx)), int(round((e[1] + n[1] * t - y0) / ky))
        return 0 <= px < w and 0 <= py < h and a.getpixel((px, py)) > 127

    lo = hi = 0.0
    while hi < 300 and opaco(hi + 1.0):
        hi += 1.0
    while lo > -300 and opaco(lo - 1.0):
        lo -= 1.0
    return max(8.0, (hi - lo) / 2.0)


def _unit(a, b):
    d = (b[0] - a[0], b[1] - a[1])
    n = math.hypot(*d) or 1.0
    return (d[0] / n, d[1] / n)


def _png(arbol, ruta):
    n = arbol[ruta]
    return Image.open(P.sprite_por_guid(n.imagen["guid"])).convert("RGBA")


def arbol_maqueta(pid, rig, orden=True):
    """
    (arbol, piezas): el arbol del prefab con el «orden_tronco» del JSON, con las piezas del arte final
    ENCENDIDAS (una Image con guid «maqueta:<ruta>») y el dict {ruta: Pieza} que las dibuja. Lo que el arte
    de hoy ya trae partido (el Nino) se deja como esta.
    """
    arbol = P.arbol_vigente(pid, rig)
    p = P.personaje_rig(rig, pid)
    if orden and p.get("orden_tronco"):
        P.aplicar_orden_tronco(arbol, p["orden_tronco"])
    piezas = {}
    if P.PERSONAJES[pid][2]:
        return arbol, piezas  # Algoritm tiene su propia maqueta (pose_preview._capa_guia)
    nodos = {n["nombre"]: n for n in p["nodos"]}
    partes = {q["nombre"]: q for q in p["partes"]}

    def enciende(ruta, pieza):
        n = arbol[ruta]
        n.activo = True
        n.imagen = {"encendida": True, "guid": "maqueta:" + ruta, "aspecto": False, "color": (1.0, 1.0, 1.0, 1.0)}
        piezas[ruta] = pieza

    # --- brazos
    for lado, brazo_r, codo_r in (("Izq", P.LA, P.LE), ("Der", P.RA, P.RE)):
        ante = codo_r + "/Antebrazo" + lado
        if arbol[ante].dibuja():
            continue
        parte = partes["Brazo" + lado]
        hombro, codo = tuple(parte["pivote"]), tuple(nodos["Codo" + lado]["punto"])
        im, rect = _png(arbol, brazo_r), tuple(parte["rect"])
        u = _unit(hombro, codo)
        r = _radio(im, rect, codo, u)
        enciende(brazo_r, Pieza(rect, _mitad(im, rect, codo, u, -1, r)))
        enciende(ante, Pieza(rect, _mitad(im, rect, codo, u, +1)))
    # --- piernas
    for lado, pierna_r, rod_r in (("Izq", P.LL, P.LK), ("Der", P.RL, P.RK)):
        ante = rod_r + "/Antepierna" + lado
        if arbol[ante].dibuja():
            continue
        parte = partes["Pierna" + lado]
        cadera, rodilla = tuple(parte["pivote"]), tuple(nodos["Rodilla" + lado]["punto"])
        im, rect = _png(arbol, pierna_r), tuple(parte["rect"])
        u = _unit(cadera, rodilla)
        r = _radio(im, rect, rodilla, u)
        enciende(pierna_r, Pieza(rect, _mitad(im, rect, rodilla, u, -1, r)))
        enciende(ante, Pieza(rect, _mitad(im, rect, rodilla, u, +1)))
    # --- cabeza
    if not arbol[P.HD].dibuja():
        torso_r = P.T + "/Torso"
        parte = partes["Torso"]
        cuello = tuple(nodos["Cuello"]["punto"])
        im, rect = _png(arbol, torso_r), tuple(parte["rect"])
        u = (0.0, 1.0)
        # el pelo que cuelga por los lados del cuello (por debajo del punto del cuello, fuera de los hombros) es de
        # la cabeza: si fuera del torso, al inclinarse la cabeza se abriria un hueco entre los dos
        semi = abs(partes["BrazoIzq"]["pivote"][0] - partes["BrazoDer"]["pivote"][0]) / 2.0
        cx = (partes["BrazoIzq"]["pivote"][0] + partes["BrazoDer"]["pivote"][0]) / 2.0
        alas = _alas_de_pelo(im, rect, cuello, cx, semi)
        # y la barbilla y la mandibula, que pasan por debajo del punto del cuello (el JSON lo pone «a ojo» en la
        # barbilla): una franja central del ancho de la cara hasta el borde de abajo del rect de la cabeza, que la
        # cabeza comparte con el torso para que no se abra una cuna entre las dos piezas al inclinarse
        cab = nodos["Cuello"]["rect"]
        ancho = 0.35 * (cab[2] - cab[0])
        franja = _mascara_rect(im.size, rect, (cuello[0] - ancho, cuello[1], cuello[0] + ancho, cab[3] + 12))
        enciende(torso_r, Pieza(rect, _mitad(im, rect, cuello, u, +1, resta=alas)))
        enciende(P.HD, Pieza(rect, _mitad(im, rect, (cuello[0], cuello[1] + SOLAPE_CABEZA), u, -1,
                                          suma=ImageChops.lighter(alas, franja))))
    return arbol, piezas  # Algoritm tiene su propia maqueta (pose_preview._capa_guia)
    nodos = {n["nombre"]: n for n in p["nodos"]}
    partes = {q["nombre"]: q for q in p["partes"]}

    def enciende(ruta, pieza):
        n = arbol[ruta]
        n.activo = True
        n.imagen = {"encendida": True, "guid": "maqueta:" + ruta, "aspecto": False, "color": (1.0, 1.0, 1.0, 1.0)}
        piezas[ruta] = pieza

    # --- brazos
    for lado, brazo_r, codo_r in (("Izq", P.LA, P.LE), ("Der", P.RA, P.RE)):
        ante = codo_r + "/Antebrazo" + lado
        if arbol[ante].dibuja():
            continue
        parte = partes["Brazo" + lado]
        hombro, codo = tuple(parte["pivote"]), tuple(nodos["Codo" + lado]["punto"])
        im, rect = _png(arbol, brazo_r), tuple(parte["rect"])
        u = _unit(hombro, codo)
        r = _radio(im, rect, codo, u)
        enciende(brazo_r, Pieza(rect, _mitad(im, rect, codo, u, -1, r)))
        enciende(ante, Pieza(rect, _mitad(im, rect, codo, u, +1)))
    # --- piernas
    for lado, pierna_r, rod_r in (("Izq", P.LL, P.LK), ("Der", P.RL, P.RK)):
        ante = rod_r + "/Antepierna" + lado
        if arbol[ante].dibuja():
            continue
        parte = partes["Pierna" + lado]
        cadera, rodilla = tuple(parte["pivote"]), tuple(nodos["Rodilla" + lado]["punto"])
        im, rect = _png(arbol, pierna_r), tuple(parte["rect"])
        u = _unit(cadera, rodilla)
        r = _radio(im, rect, rodilla, u)
        enciende(pierna_r, Pieza(rect, _mitad(im, rect, rodilla, u, -1, r)))
        enciende(ante, Pieza(rect, _mitad(im, rect, rodilla, u, +1)))
    # --- cabeza
    if not arbol[P.HD].dibuja():
        torso_r = P.T + "/Torso"
        parte = partes["Torso"]
        cuello = tuple(nodos["Cuello"]["punto"])
        im, rect = _png(arbol, torso_r), tuple(parte["rect"])
        u = (0.0, 1.0)
        # el pelo que cuelga por los lados del cuello (por debajo del punto del cuello, fuera de los hombros) es de
        # la cabeza: si fuera del torso, al inclinarse la cabeza se abriria un hueco entre los dos
        cab = nodos["Cuello"]["rect"]
        semi = abs(partes["BrazoIzq"]["pivote"][0] - partes["BrazoDer"]["pivote"][0]) / 2.0
        cx = (partes["BrazoIzq"]["pivote"][0] + partes["BrazoDer"]["pivote"][0]) / 2.0
        alas = [(rect[0], cuello[1], cx - 0.9 * semi, cab[3]), (cx + 0.9 * semi, cuello[1], rect[2], cab[3])]
        enciende(torso_r, Pieza(rect, _mitad(im, rect, cuello, u, +1, resta=alas)))
        enciende(P.HD, Pieza(rect, _mitad(im, rect, (cuello[0], cuello[1] + SOLAPE_CABEZA), u, -1, suma=alas)))
    return arbol, piezas


def pieza_o_png(arbol, piezas, ruta):
    """La imagen y el rect (de dibujo) de un nodo: la pieza de la maqueta si la hay, si no el PNG de su Image."""
    if ruta in piezas:
        return piezas[ruta].imagen, piezas[ruta].rect
    n = arbol.get(ruta)
    if n is None or not n.imagen or not n.imagen.get("guid"):
        return None, None
    arch = P.sprite_por_guid(n.imagen["guid"])
    if arch is None:
        return None, None
    return Image.open(arch).convert("RGBA"), n.rect
