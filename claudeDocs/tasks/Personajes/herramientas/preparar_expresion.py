#!/usr/bin/env python3
# preparar_expresion.py: de UNA imagen de cara completa a las capas del rig, con un comando.
#
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_expresion.py <id> <emocion> <imagen>            # informe y composites
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_expresion.py <id> <emocion> <imagen> --aplicar  # escribe
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_expresion.py --autoprueba                       # se prueba solo
#         <id> = nino (el que ya tiene cabeza propia: papa, mama y nina cuando entreguen su arte final)
#         <emocion> = neutra | alegria | sorpresa | preocupacion | concentracion | sueno | parpadeo_medio | parpadeo_cerrado
#                     (y las bocas de hablar: boca_a | boca_e | boca_u, que solo escriben la boca)
#         --escala F    la fraccion del ancho del ovalo de la cara que ocupa la cara (de extremo a extremo de ojos y cejas);
#                       por defecto la mayor de 0.50 / 0.60 / 0.70 que cabe (cejas bajo el flequillo, boca sobre la barbilla)
#         --asigna capa:x0,y0,x1,y1   corrige a mano: todo pixel de la imagen dentro de esa caja (px de LA IMAGEN) pasa a esa capa
#                       (ojos | boca | nariz | rubor | nada); se puede repetir
#         --base        reescribe tambien char_<x>_cara_base (por defecto solo si aun no existe)
#         --reubica     recalcula el rect comun aunque la cara ya este puesta (por defecto, las emociones siguientes lo heredan)
#         --max-lado N  la imagen se reduce a N px de lado mayor si es mas grande (512): nada depende de que sean 256
#         --salida DIR  donde dejar los composites del informe (por defecto, el tmp)
#
# QUE HACE. Santiago entrega cada expresion como UNA imagen de cara completa con fondo transparente (cejas, ojos, nariz, boca y
# rubor). El rig la quiere en TRES capas dentro de Cabeza (de atras adelante): CaraBase (nariz y rubor, que no cambian de una
# emocion a otra), Ojos (con las cejas) y Boca (que cambia al hablar). La herramienta:
#   a. SEPARA por componentes conexas y color, sin depender del tamano de la imagen (todo es relativo a la caja de lo que hay):
#        - rubor: pixeles rosados (tono ~354 grados; la regla de la direccion de arte, §4, es #F0A5A0) y los trazos oscuros que caen
#          dentro de cada mancha (las rayitas del rubor); manchas a los lados, fuera de la franja central;
#        - boca: la componente oscura mas ancha de la franja central por debajo de la mitad de la cara (con lo que le cuelga:
#          comisuras, dientes y la lengua rosada que quede dentro de su caja);
#        - nariz: lo pequeño de la franja central entre los ojos y la boca;
#        - ojos: todo lo demas (ojos, parpadeos, cejas).
#      Informa lo que asigno a cada capa (cajas y areas) y avisa de lo raro (sin boca, sin nariz, algo grande en la mitad de abajo).
#      Si la heuristica falla, --asigna lo corrige por caja.
#   b. Escribe tres PNG alineados entre si, con el MISMO lienzo que la imagen (reducido si pasa de --max-lado, igual para todos):
#      char_<x>_ojos_<emocion>, char_<x>_boca_<...> (neutra -> boca_0, la boca de reposo; alegria, sorpresa, preocupacion y
#      concentracion -> boca_<emocion>, Personajes-Resultados.md C.4) y char_<x>_cara_base (nariz y rubor). El lienzo no se
#      recorta: un Image estira el sprite a su rect, asi que TODAS las expresiones tienen que compartir lienzo y rect o la cara se
#      deformaria al cambiar; por eso tampoco se recorta cada una a lo suyo.
#   c. COLOCA la cara en la cabeza: mide en char_<x>_parte_cabeza el ovalo de la piel (el color de la piel, su componente mayor),
#      su eje, el borde de abajo del flequillo (el punto mas bajo del pelo sobre la cara) y la barbilla; centra la cara en el eje,
#      pone el borde de arriba de las cejas justo bajo el flequillo (las cejas van sobre piel, no sobre el pelo) y escala para que la
#      cara ocupe la fraccion --escala del ovalo. El rect comun (lienzo de 1024, y hacia abajo) de Ojos, Boca y CaraBase es el del
#      lienzo entero de la imagen puesto asi.
#   d. Sin --aplicar: composites (la cabeza con la cara a tres escalas; con guias del ovalo; el personaje entero en reposo con
#      pose_preview) e informe. Con --aplicar: escribe los PNG en Assets/Game/Art/Characters/<Carpeta>/Expresiones/ (Unity hace los
#      .meta al importarlos), anota el rect en arte_final.json, regenera rig_articulaciones.json y clips_personajes.json, corre
#      pose_preview e imprime las ordenes para la sesion local.
#   e. --autoprueba: una cara y una cabeza sinteticas con piezas conocidas; comprueba que la separacion y la medida las recuperan.
#
# POR QUE no se recorta ni se cuenta con 256: la imagen puede llegar mas grande (un original); el tamano solo sale de --max-lado, y
# todo lo que se mide (areas, cajas, umbrales) es relativo a la caja de lo que hay en la imagen.

import argparse
import copy
import json
import math
import os
import re
import subprocess
import sys
import tempfile
from collections import Counter

try:
    from PIL import Image, ImageChops, ImageDraw, ImageFilter
except ImportError:  # pragma: no cover
    sys.exit("hace falta Pillow: pip install pillow")

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import articulaciones as A  # noqa: E402
import prefabs as P  # noqa: E402
from preparar_arte_final import mayor_componente  # noqa: E402

FONDO = (236, 232, 222, 255)

# nombre del PNG de ojos y de boca por emocion (Personajes-Resultados.md C.4); None = esa capa no cambia con esa emocion.
# Neutral y Sleeping no llevan boca propia: su reposo es boca_0 (aqui «sueno» solo escribe los ojos).
EMOCIONES = {
    "neutra": ("ojos_neutra", "boca_0"),
    "alegria": ("ojos_alegria", "boca_alegria"),
    "sorpresa": ("ojos_sorpresa", "boca_sorpresa"),
    "preocupacion": ("ojos_preocupacion", "boca_preocupacion"),
    "concentracion": ("ojos_concentracion", "boca_concentracion"),
    "sueno": ("ojos_sueno", None),
    "parpadeo_medio": ("ojos_parpadeo_medio", None),
    "parpadeo_cerrado": ("ojos_parpadeo_cerrado", None),
    "boca_a": (None, "boca_a"),
    "boca_e": (None, "boca_e"),
    "boca_u": (None, "boca_u"),
}
CAPAS = ("ojos", "boca", "nariz", "rubor")
CODIGO = {"ojos": 1, "boca": 2, "nariz": 3, "rubor": 4, "nada": 0}
NOMBRE_CODIGO = {v: k for k, v in CODIGO.items()}

UMBRAL_ALFA = 24      # por debajo, el pixel es aliasing tenue y no cuenta para la caja de la cara
FRANJA_CENTRAL = 0.16  # la franja central (nariz y boca): +-16 % del ancho de la cara respecto a su eje
TONO_ROSA = (234, 11)  # tono del rubor en la escala de Pillow (0..255 = 0..360 grados): de 330 a 15 grados, pasando por el 0
TONO_ROSA_BORDE = 17   # el borde blando de la mancha (alfa < ALFA_BORDE) vira al naranja: hasta 24 grados. El iris es opaco, no entra
ALFA_BORDE = 235
SAT_ROSA = 0.28
VAL_ROSA = 0.68
ESCALAS = (0.50, 0.60, 0.70)   # fraccion del ancho del ovalo que ocupa la cara, las tres candidatas del composite


# ============================================================================ 1. componentes conexas


class Comp:
    __slots__ = ("id", "area", "x0", "y0", "x1", "y1", "sx", "sy", "capa")

    def __init__(self, i):
        self.id, self.area, self.sx, self.sy = i, 0, 0, 0
        self.x0 = self.y0 = 1 << 30
        self.x1 = self.y1 = -1
        self.capa = None

    @property
    def cx(self):
        return self.sx / max(1, self.area)

    @property
    def cy(self):
        return self.sy / max(1, self.area)

    @property
    def ancho(self):
        return self.x1 - self.x0

    @property
    def alto(self):
        return self.y1 - self.y0

    @property
    def caja(self):
        return (self.x0, self.y0, self.x1, self.y1)

    def dentro_de(self, caja, holgura=0):
        return (caja[0] - holgura <= self.cx <= caja[2] + holgura) and (caja[1] - holgura <= self.cy <= caja[3] + holgura)


_CORRIDA = re.compile(rb"\xff+")


def etiqueta(mascara):
    """
    Componentes conexas (8 vecinos) de una mascara L de 0/255, por corridas de cada fila y union-find. Devuelve
    (rotulos, comps): rotulos es un bytearray de ancho x alto con el id de cada pixel (0 = fondo; las componentes van de la
    mayor a la menor, de 1 a 254, y las demas comparten el 255) y comps es {id: Comp} (el 255, con las sumas de todas ellas).
    """
    w, h = mascara.size
    datos = mascara.tobytes()
    padre = []

    def busca(i):
        while padre[i] != i:
            padre[i] = padre[padre[i]]
            i = padre[i]
        return i

    corridas = []
    previa = []
    for y in range(h):
        fila = datos[y * w:(y + 1) * w]
        actual = []
        for m in _CORRIDA.finditer(fila):
            i = len(padre)
            padre.append(i)
            actual.append((m.start(), m.end(), i))
            corridas.append((y, m.start(), m.end(), i))
        j = 0
        for a, b, i in actual:
            while j < len(previa) and previa[j][1] < a:   # una corrida de arriba que termina antes de a no toca (8 vecinos: b >= a)
                j += 1
            k = j
            while k < len(previa) and previa[k][0] <= b:
                ra, rb = busca(i), busca(previa[k][2])
                if ra != rb:
                    padre[ra] = rb
                k += 1
        previa = actual
    raices = {}
    for y, a, b, i in corridas:
        r = busca(i)
        c = raices.get(r)
        if c is None:
            c = raices[r] = Comp(r)
        n = b - a
        c.area += n
        c.sx += (a + b - 1) * n / 2.0
        c.sy += y * n
        c.x0, c.y0, c.x1, c.y1 = min(c.x0, a), min(c.y0, y), max(c.x1, b), max(c.y1, y + 1)
    orden = sorted(raices.values(), key=lambda c: -c.area)
    nuevo = {}
    comps = {}
    for k, c in enumerate(orden):
        ident = min(k + 1, 255)
        nuevo[c.id] = ident
        if ident in comps:  # los de despues del 254 se funden en uno solo
            o = comps[ident]
            o.area += c.area
            o.sx += c.sx
            o.sy += c.sy
            o.x0, o.y0, o.x1, o.y1 = min(o.x0, c.x0), min(o.y0, c.y0), max(o.x1, c.x1), max(o.y1, c.y1)
        else:
            c.id = ident
            comps[ident] = c
    rotulos = bytearray(w * h)
    for y, a, b, i in corridas:
        rotulos[y * w + a:y * w + b] = bytes([nuevo[busca(i)]]) * (b - a)
    return rotulos, comps


# ============================================================================ 2. separar la cara


def _binaria(im, f):
    return im.point(lambda v: 255 if f(v) else 0)


def _y(*mascaras):
    out = mascaras[0]
    for m in mascaras[1:]:
        out = ImageChops.multiply(out, m)
    return out


def _dilata(mascara, d):
    return mascara.filter(ImageFilter.MaxFilter(2 * d + 1)) if d > 0 else mascara


class Separacion:
    """Lo que separa la heuristica: las capas (RGBA del mismo lienzo), lo asignado a cada una y los avisos."""

    def __init__(self):
        self.capas = {}      # "ojos" | "boca" | "base" -> imagen RGBA
        self.clases = None   # imagen L con el codigo de capa de cada pixel
        self.resumen = {}    # capa -> {"componentes", "area", "caja"}
        self.avisos = []
        self.caja = None     # caja de todo lo opaco (x0, y0, x1, y1), px de la imagen
        self.eje = 0.0
        self.tamano = (0, 0)


def separa(im, correcciones=()):
    """
    Separa la cara de «im» (RGBA) en ojos (con cejas), boca, nariz y rubor. «correcciones» es [(capa, (x0, y0, x1, y1))] en pixeles
    de la imagen: todo pixel no transparente dentro de la caja pasa a esa capa («nada» lo borra). Devuelve una Separacion.
    """
    im = im.convert("RGBA")
    w, h = im.size
    res = Separacion()
    res.tamano = (w, h)
    alfa = im.getchannel("A")
    opaco = _binaria(alfa, lambda v: v >= UMBRAL_ALFA)
    caja = opaco.getbbox()
    if caja is None:
        raise ValueError("la imagen esta vacia (todo transparente)")
    res.caja = caja
    W, H = caja[2] - caja[0], caja[3] - caja[1]
    eje = (caja[0] + caja[2]) / 2.0
    res.eje = eje
    d = max(1, int(round(0.006 * max(W, H))))

    # pixeles rosados (el rubor, y la lengua si la boca la lleva): tono, saturacion y brillo en HSV
    th, ts, tv = im.convert("RGB").convert("HSV").split()
    brillante = _y(_binaria(ts, lambda v: v >= SAT_ROSA * 255), _binaria(tv, lambda v: v >= VAL_ROSA * 255), _binaria(alfa, lambda v: v >= 8))
    rosa = ImageChops.lighter(
        _y(_binaria(th, lambda v: v >= TONO_ROSA[0] or v <= TONO_ROSA[1]), brillante),
        _y(_binaria(th, lambda v: v >= TONO_ROSA[0] or v <= TONO_ROSA_BORDE), _binaria(alfa, lambda v: v < ALFA_BORDE), brillante))
    visible = _binaria(alfa, lambda v: v >= 8)   # lo que se agrupa y se reparte: tambien el aliasing tenue de los bordes
    oscuro = ImageChops.subtract(visible, rosa)

    rot_o, comp_o = etiqueta(_dilata(oscuro, d))
    rot_r, comp_r = etiqueta(_dilata(rosa, 2 * d))

    # --- rosa: manchas a los lados (rubor); lo rosado de la franja central, lo decide la boca (la lengua)
    centro = FRANJA_CENTRAL * W
    parches = [c for c in comp_r.values() if abs(c.cx - eje) > centro]
    for c in parches:
        c.capa = "rubor"
    centrales_rosa = [c for c in comp_r.values() if abs(c.cx - eje) <= centro]

    # --- lo oscuro: las rayitas dentro de una mancha son del rubor
    sueltos = []
    for c in comp_o.values():
        dentro = next((p for p in parches if c.dentro_de(p.caja, 2 * d) and c.ancho <= p.ancho + 4 * d and c.alto <= p.alto + 4 * d), None)
        if dentro is not None:
            c.capa = "rubor"
        else:
            sueltos.append(c)

    # --- boca: la mas ancha de la franja central, por debajo de la mitad de la cara; nariz: lo pequeño entre los ojos y ella
    en_centro = [c for c in sueltos if abs(c.cx - eje) <= centro]
    bajas = [c for c in en_centro if c.cy >= caja[1] + 0.45 * H]
    boca = max(bajas, key=lambda c: c.ancho) if bajas else None
    if boca is None:
        res.avisos.append("no encuentro la boca (una componente oscura en la franja central por debajo de la mitad); usa --asigna boca:x0,y0,x1,y1")
    else:
        boca.capa = "boca"
        nariz = [c for c in en_centro if c is not boca and c.cy < boca.cy and c.cy > caja[1] + 0.25 * H and c.area < 0.5 * boca.area]
        for c in nariz:
            c.capa = "nariz"
        if not nariz:
            res.avisos.append("no encuentro la nariz (algo pequeño en la franja central entre los ojos y la boca); si la cara no la lleva, ignora este aviso")
        # lo que cuelga de la boca: comisuras, dientes, la lengua rosada
        ex, ey = 0.08 * W, 0.10 * H
        caja_b = (boca.x0 - ex, boca.y0 - ey, boca.x1 + ex, boca.y1 + ey)
        for c in sueltos:
            if c.capa is None and c.dentro_de(caja_b):
                c.capa = "boca"
        for c in centrales_rosa:
            c.capa = "boca" if c.dentro_de(caja_b) else "rubor"
    for c in centrales_rosa:
        if c.capa is None:
            c.capa = "rubor"
    for c in sueltos:
        if c.capa is None:
            c.capa = "ojos"   # lo demas: ojos, parpadeos y cejas
            if c.cy > caja[1] + 0.75 * H and c.area > 0.02 * W * H:
                res.avisos.append("una componente grande (%d px) cae en el cuarto de abajo y la asigno a los ojos: revisa (%s)" % (c.area, c.caja))

    # --- a clase por pixel: cada pixel hereda la de su componente
    tabla_o = bytes(CODIGO[comp_o[i].capa] if i in comp_o and comp_o[i].capa else 0 for i in range(256))
    tabla_r = bytes(CODIGO[comp_r[i].capa] if i in comp_r and comp_r[i].capa else 0 for i in range(256))
    cl_o = _y(Image.frombytes("L", (w, h), bytes(rot_o).translate(tabla_o)), _binaria(oscuro, lambda v: v))
    cl_r = _y(Image.frombytes("L", (w, h), bytes(rot_r).translate(tabla_r)), _binaria(rosa, lambda v: v))
    clases = bytearray(ImageChops.add(cl_o, cl_r).tobytes())

    # --- correcciones a mano, por caja y por pixel
    datos_a = alfa.tobytes()
    for capa, (x0, y0, x1, y1) in correcciones:
        x0, y0, x1, y1 = max(0, int(x0)), max(0, int(y0)), min(w, int(x1)), min(h, int(y1))
        for y in range(y0, y1):
            base = y * w
            for x in range(x0, x1):
                if datos_a[base + x]:
                    clases[base + x] = CODIGO[capa]
    clase_im = Image.frombytes("L", (w, h), bytes(clases))
    res.clases = clase_im

    def capa_de(codigos):
        mascara = clase_im.point(lambda v, cs=tuple(codigos): 255 if v in cs else 0)
        out = im.copy()
        out.putalpha(ImageChops.multiply(alfa, mascara))
        return out, mascara

    for nombre, codigos in (("ojos", (1,)), ("boca", (2,)), ("base", (3, 4))):
        res.capas[nombre], _ = capa_de(codigos)
    for nombre in CAPAS:
        m = clase_im.point(lambda v, k=CODIGO[nombre]: 255 if v == k else 0)
        # area y caja: solo lo que se ve (alfa >= UMBRAL_ALFA); el aliasing tenue de los bordes viaja con su capa pero no cuenta
        vis = ImageChops.multiply(m, opaco)
        res.resumen[nombre] = {"area": vis.histogram()[255], "caja": vis.getbbox(),
                               "componentes": sum(1 for c in list(comp_o.values()) + list(comp_r.values()) if c.capa == nombre)}
    perdidos = _y(_binaria(alfa, lambda v: v >= UMBRAL_ALFA), clase_im.point(lambda v: 255 if v == 0 else 0)).histogram()[255]
    if perdidos:
        res.avisos.append("%d pixeles opacos sin capa (aislados de todo): se descartan; --asigna los recoge" % perdidos)
    return res


def informe_separacion(res):
    print("== Separacion de la cara (imagen %dx%d; lo que hay ocupa %s, eje x = %.1f)" % (res.tamano[0], res.tamano[1], res.caja, res.eje))
    print("  %-8s %-12s %10s   %s" % ("capa", "componentes", "area (px)", "caja (x0, y0, x1, y1) en px de la imagen"))
    for nombre in CAPAS:
        r = res.resumen[nombre]
        print("  %-8s %-12d %10d   %s" % (nombre, r["componentes"], r["area"], r["caja"]))
    for a in res.avisos:
        print("AVISO", a)


# ============================================================================ 3. medir la cabeza y colocar la cara


class Cabeza:
    """El ovalo de piel de la cabeza, medido sobre su PNG, en el lienzo de 1024."""

    def __init__(self):
        self.imagen = None   # RGBA de la cabeza
        self.rect = None     # su rect en el lienzo
        self.ovalo = None    # (x0, y0, x1, y1) de la piel
        self.eje = 0.0
        self.flequillo = 0.0  # el punto mas bajo del pelo sobre la cara (y)
        self.barbilla = 0.0
        self.piel = None
        self.izq = {}        # y (lienzo) -> x de la izquierda del ovalo en esa fila
        self.der = {}

    def ancho_en(self, y):
        """Ancho del ovalo a la altura y (la fila mas cercana medida)."""
        k = min(self.izq, key=lambda v: abs(v - y))
        return self.der[k] - self.izq[k]

    def extremos_en(self, y):
        k = min(self.izq, key=lambda v: abs(v - y))
        return self.izq[k], self.der[k]


def mide_cabeza(png, rect, tolerancia=26):
    """
    Mide el ovalo de la cara en el PNG de la cabeza. El color de la piel es el mas comun en el centro-abajo de lo opaco; el ovalo es
    la componente mayor de ese color (las orejas quedan aparte, tras el contorno). Devuelve una Cabeza en coordenadas del lienzo.
    """
    im = Image.open(png).convert("RGBA") if isinstance(png, str) else png.convert("RGBA")
    w, h = im.size
    alfa = im.getchannel("A")
    bb = alfa.point(lambda v: 255 if v > 200 else 0).getbbox()
    if bb is None:
        raise ValueError("la cabeza esta vacia")
    sx, sy = (rect[2] - rect[0]) / float(w), (rect[3] - rect[1]) / float(h)
    # el color de la piel: el mas comun en el cuadro central, del 55 al 85 % de la altura y del 40 al 60 % del ancho de lo opaco
    zona = im.crop((int(bb[0] + 0.40 * (bb[2] - bb[0])), int(bb[1] + 0.55 * (bb[3] - bb[1])),
                    int(bb[0] + 0.60 * (bb[2] - bb[0])), int(bb[1] + 0.85 * (bb[3] - bb[1]))))
    bruto = zona.tobytes()   # RGBA a RGBA (getdata esta en desuso en Pillow nuevos y get_flattened_data no existe en los viejos)
    pixeles = [(bruto[i], bruto[i + 1], bruto[i + 2]) for i in range(0, len(bruto), 4) if bruto[i + 3] > 200]
    # solo tonos de piel (durazno): con barba o pelo largo el centro-abajo de la cabeza es todo pelo (Papa y Mama, 06/10/2026);
    # si ahi no hay piel se busca en toda la cabeza
    def es_piel(c):
        return c[0] >= 200 and 120 <= c[1] <= 225 and c[2] >= 90 and c[0] - c[2] >= 40
    piel_zona = [c for c in pixeles if es_piel(c)]
    if not piel_zona:
        bruto = im.tobytes()
        piel_zona = [c for c in ((bruto[i], bruto[i + 1], bruto[i + 2]) for i in range(0, len(bruto), 4) if bruto[i + 3] > 200) if es_piel(c)]
    pixeles = piel_zona or pixeles
    cuenta = Counter((r // 8, g // 8, b // 8) for (r, g, b) in pixeles)
    if not cuenta:
        raise ValueError("no encuentro piel en el centro de la cabeza")
    cubo = cuenta.most_common(1)[0][0]
    muestra = [(r, g, b) for (r, g, b) in pixeles if (r // 8, g // 8, b // 8) == cubo]
    piel = tuple(int(round(sum(c[k] for c in muestra) / len(muestra))) for k in range(3))
    dif = ImageChops.difference(im.convert("RGB"), Image.new("RGB", (w, h), piel)).split()
    mascara = _y(*[_binaria(c, lambda v: v <= tolerancia) for c in dif], _binaria(alfa, lambda v: v > 200))
    oval, _ = mayor_componente(mascara)
    cab = Cabeza()
    cab.imagen, cab.rect, cab.piel = im, tuple(rect), piel
    caja = oval.getbbox()
    cab.ovalo = (rect[0] + caja[0] * sx, rect[1] + caja[1] * sy, rect[0] + caja[2] * sx, rect[1] + caja[3] * sy)
    cab.barbilla = rect[1] + caja[3] * sy
    datos = oval.tobytes()
    ejes = []
    for y in range(caja[1], caja[3]):
        fila = datos[y * w:(y + 1) * w]
        a, b = fila.find(b"\xff"), fila.rfind(b"\xff")
        if a < 0:
            continue
        cab.izq[rect[1] + y * sy], cab.der[rect[1] + y * sy] = rect[0] + a * sx, rect[0] + (b + 1) * sx
        if caja[1] + 0.55 * (caja[3] - caja[1]) <= y <= caja[1] + 0.90 * (caja[3] - caja[1]):
            ejes.append((a + b + 1) / 2.0)
    cab.eje = rect[0] + (sum(ejes) / len(ejes)) * sx
    # el borde de abajo del flequillo: por columna, la primera fila de piel; en el 60 % central del ovalo, la mas baja (la punta del pelo
    # que mas baja: una ceja que la toque se veria sobre el pelo)
    trans = oval.transpose(Image.TRANSPOSE).tobytes()  # fila x = columna x del original
    tops = []
    c0, c1 = int(caja[0] + 0.20 * (caja[2] - caja[0])), int(caja[0] + 0.80 * (caja[2] - caja[0]))
    for x in range(c0, c1):
        col = trans[x * h:(x + 1) * h]
        y = col.find(b"\xff")
        if y >= 0:
            tops.append(y)
    cab.flequillo = rect[1] + max(tops) * sy
    return cab


class Colocacion:
    """La cara puesta en la cabeza con una escala."""

    def __init__(self):
        self.fraccion = 0.0   # del ancho del ovalo
        self.s = 0.0          # px del lienzo de 1024 por px de la imagen
        self.rect = None      # [x0, y0, x1, y1] del lienzo entero de la imagen, en el lienzo de 1024
        self.margenes = {}    # nombre -> px (>= 0 es que cabe)
        self.cabe = True


def coloca(cab, res, fraccion):
    """Coloca la cara (la caja de ojos y cejas, boca y rubor) en la cabeza: centrada en el eje, bajo el flequillo y con ese ancho."""
    w, h = res.tamano
    ojos = res.resumen["ojos"]["caja"]
    if ojos is None:
        raise ValueError("la separacion no tiene ojos: no se puede colocar la cara")
    lo, hi = ojos[0], ojos[2]   # extremos de ojos y cejas: lo que mide «el ancho de la cara»
    contenido = res.caja
    # la altura de las cejas: su borde de arriba bajo el flequillo; el ancho del ovalo se mide a la altura de los ojos ya puestos
    margen_f = 0.02 * (cab.barbilla - cab.flequillo)
    top = cab.flequillo + margen_f
    ancho_ojos = float(hi - lo)
    # primera estimacion del ancho del ovalo a media altura de los ojos, con la escala que sale de «la cara ocupa tal fraccion»
    s = fraccion * (cab.ovalo[2] - cab.ovalo[0]) / ancho_ojos
    for _ in range(3):  # el ancho del ovalo depende de la altura de los ojos, que depende de s: converge en dos vueltas
        y_ojos = top + ((ojos[1] + ojos[3]) / 2.0 - contenido[1]) * s
        s = fraccion * cab.ancho_en(y_ojos) / ancho_ojos
    c = Colocacion()
    c.fraccion, c.s = fraccion, s
    cx_ojos = (lo + hi) / 2.0
    x0 = cab.eje - cx_ojos * s
    y0 = top - contenido[1] * s
    c.rect = [int(round(x0)), int(round(y0)), int(round(x0 + w * s)), int(round(y0 + h * s))]
    # los margenes (px del lienzo): >= 0 es que cabe
    y_abajo = y0 + max(res.resumen["boca"]["caja"][3] if res.resumen["boca"]["caja"] else 0, res.resumen["rubor"]["caja"][3] if res.resumen["rubor"]["caja"] else 0) * s
    c.margenes["cejas bajo el flequillo"] = top - cab.flequillo
    c.margenes["boca y rubor sobre la barbilla"] = cab.barbilla - y_abajo - 0.04 * (cab.barbilla - cab.flequillo)
    r = res.resumen["rubor"]["caja"]
    if r is not None:
        yr = y0 + (r[1] + r[3]) / 2.0 * s
        izq, der = cab.extremos_en(yr)
        c.margenes["rubor dentro del ovalo"] = min(x0 + r[0] * s - izq, der - (x0 + r[2] * s))
    c.cabe = all(v >= 0 for v in c.margenes.values())
    return c


def elige(cab, res, fraccion=None):
    """[(Colocacion)] de las candidatas y el indice de la recomendada: la mayor fraccion que cabe (o la menor, si ninguna)."""
    fracciones = (fraccion,) if fraccion else ESCALAS
    cols = [coloca(cab, res, f) for f in fracciones]
    cabe = [i for i, c in enumerate(cols) if c.cabe]
    return cols, (max(cabe) if cabe else 0)


# ============================================================================ 4. imagenes de informe


def _rotulo(im, texto, color=(40, 40, 40, 255)):
    d = ImageDraw.Draw(im)
    d.rectangle([0, 0, im.width, 14], fill=(255, 255, 255, 220))
    d.text((3, 1), texto, fill=color)


def reduce(im, max_lado):
    """La imagen reducida a «max_lado» px de lado mayor (LANCZOS, con alfa premultiplicado por Pillow); igual si ya cabe."""
    k = min(1.0, float(max_lado) / max(im.size))
    if k >= 1.0:
        return im
    return im.resize((max(1, int(round(im.width * k))), max(1, int(round(im.height * k)))), Image.LANCZOS)


def compone_cabeza(cab, capas_rect, guias=False):
    """La cabeza con CaraBase, Ojos y Boca encima (cada capa: (imagen RGBA, rect en el lienzo)). Con guias, el ovalo, el eje y el flequillo."""
    base = Image.new("RGBA", cab.imagen.size, FONDO)
    base.alpha_composite(cab.imagen)
    sx, sy = cab.imagen.width / float(cab.rect[2] - cab.rect[0]), cab.imagen.height / float(cab.rect[3] - cab.rect[1])
    for nombre in ("base", "ojos", "boca"):
        im, rect = capas_rect[nombre]
        rw, rh = rect[2] - rect[0], rect[3] - rect[1]
        sprite = im.resize((rw, rh), Image.LANCZOS)
        capa = Image.new("RGBA", base.size, (0, 0, 0, 0))
        capa.paste(sprite, (int(round((rect[0] - cab.rect[0]) * sx)), int(round((rect[1] - cab.rect[1]) * sy))))
        base.alpha_composite(capa)
    if guias:
        d = ImageDraw.Draw(base)
        a = lambda p: ((p[0] - cab.rect[0]) * sx, (p[1] - cab.rect[1]) * sy)
        (x0, y0), (x1, y1) = a((cab.ovalo[0], cab.ovalo[1])), a((cab.ovalo[2], cab.ovalo[3]))
        d.rectangle([x0, y0, x1, y1], outline=(40, 90, 220, 255))
        ex = a((cab.eje, 0))[0]
        d.line([ex, y0, ex, y1], fill=(40, 160, 90, 255))
        fy = a((0, cab.flequillo))[1]
        d.line([x0, fy, x1, fy], fill=(220, 40, 160, 255))
    return base.convert("RGB")


def candidatas_png(res, cols, carpeta_png, pid, emocion):
    """Escribe en carpeta_png los PNG (ya reducidos) y devuelve {nombre de sprite: ruta}: lo que PNG_EXTRA necesita."""
    ojos_n, boca_n = EMOCIONES[emocion]
    salida = {}
    os.makedirs(carpeta_png, exist_ok=True)
    for clave, nombre in (("ojos", ojos_n), ("boca", boca_n), ("base", "cara_base")):
        if nombre is None:
            continue
        sprite = "char_%s_%s" % (pid, nombre)
        ruta = os.path.join(carpeta_png, sprite + ".png")
        res.capas_salida[clave].save(ruta)
        salida[sprite] = ruta
    return salida


def capas_separadas(res, ancho=360):
    """Las tres capas que salen de la separacion, una al lado de otra y con la caja de cada una, para revisar la heuristica a ojo."""
    paneles = []
    k = ancho / float(res.tamano[0])
    for nombre, clave, color in (("ojos y cejas", "ojos", (206, 226, 240)), ("boca", "boca", (240, 226, 206)), ("nariz + rubor (CaraBase)", "base", (214, 238, 214))):
        fondo = Image.new("RGBA", res.tamano, color + (255,))
        fondo.alpha_composite(res.capas[clave])
        im = fondo.resize((ancho, int(round(res.tamano[1] * k))), Image.LANCZOS)
        d = ImageDraw.Draw(im)
        caja = res.capas[clave].getchannel("A").point(lambda v: 255 if v >= UMBRAL_ALFA else 0).getbbox()
        if caja:
            d.rectangle([caja[0] * k, caja[1] * k, caja[2] * k, caja[3] * k], outline=(220, 40, 40, 255))
        _rotulo(im, "%s  %s" % (nombre, caja))
        paneles.append(im.convert("RGB"))
    hoja = Image.new("RGB", (sum(p.width for p in paneles), paneles[0].height), FONDO[:3])
    x = 0
    for p in paneles:
        hoja.paste(p, (x, 0))
        x += p.width
    return hoja


def composites(pid, emocion, res, cab, cols, recomendada, rig, salida):
    """Los composites del informe: las capas separadas, la cabeza a tres escalas, con guias, y el personaje entero en reposo con pose_preview."""
    os.makedirs(salida, exist_ok=True)
    rutas = []
    ruta = os.path.join(salida, "%s_%s_capas.png" % (pid, emocion))
    capas_separadas(res).save(ruta)
    rutas.append(ruta)
    paneles = []
    for c in cols:
        capas = {k: (res.capas_salida[k], c.rect) for k in ("base", "ojos", "boca")}
        p = compone_cabeza(cab, capas).convert("RGBA")
        _rotulo(p, "cara = %d %% del ovalo | S %.3f | rect %s%s" % (round(100 * c.fraccion), c.s, c.rect, "  <- recomendada" if c is cols[recomendada] else ""))
        paneles.append(p.convert("RGB"))
    ancho = sum(p.width for p in paneles)
    hoja = Image.new("RGB", (ancho, paneles[0].height), FONDO[:3])
    x = 0
    for p in paneles:
        hoja.paste(p, (x, 0))
        x += p.width
    ruta = os.path.join(salida, "%s_%s_cabeza_escalas.png" % (pid, emocion))
    hoja.save(ruta)
    rutas.append(ruta)
    c = cols[recomendada]
    g = compone_cabeza(cab, {k: (res.capas_salida[k], c.rect) for k in ("base", "ojos", "boca")}, guias=True)
    ruta = os.path.join(salida, "%s_%s_guias.png" % (pid, emocion))
    g.save(ruta)
    rutas.append(ruta)
    # el personaje entero en reposo, con pose_preview: la tabla se copia en memoria con los rect de la cara y los PNG se ven
    # sin escribirlos en el repo (prefabs.PNG_EXTRA)
    import pose_preview as V
    carpeta_png = os.path.join(salida, "_png_%s_%s" % (pid, emocion))
    extra = candidatas_png(res, cols, carpeta_png, pid, emocion)
    P.PNG_EXTRA.update(extra)
    try:
        cuerpos = []
        clip = next(k for k in V.clips_de(V.cargar_clips(), pid) if k.accion == "Idle")
        for c in cols:
            rig2 = copy.deepcopy(rig)
            nodos = P.personaje_rig(rig2, pid)["nodos"]
            ojos_n, boca_n = EMOCIONES[emocion]
            for n in nodos:
                if n["nombre"] in ("CaraBase", "Ojos", "Boca"):
                    n["rect"] = list(c.rect)
                    n["punto"] = [(c.rect[0] + c.rect[2]) // 2, (c.rect[1] + c.rect[3]) // 2]
                if n["nombre"] == "Ojos" and ojos_n:
                    n["sprite"] = "char_%s_%s" % (pid, ojos_n)   # la emocion que se esta viendo
                if n["nombre"] == "Boca" and boca_n:
                    n["sprite"] = "char_%s_%s" % (pid, boca_n)
            pj = V.Personaje(pid, rig2)
            im = V.render(pj, clip, 0.0, 0.55, region=(150, 40, 880, 960)).convert("RGBA")
            _rotulo(im, "cara = %d %% del ovalo (S %.3f)" % (round(100 * c.fraccion), c.s))
            cuerpos.append(im.convert("RGB"))
        hoja2 = Image.new("RGB", (sum(i.width for i in cuerpos), cuerpos[0].height), FONDO[:3])
        x = 0
        for im in cuerpos:
            hoja2.paste(im, (x, 0))
            x += im.width
        ruta = os.path.join(salida, "%s_%s_cuerpo_escalas.png" % (pid, emocion))
        hoja2.save(ruta)
        rutas.append(ruta)
    finally:
        for k in extra:
            P.PNG_EXTRA.pop(k, None)
    return rutas


# ============================================================================ 5. aplicar


def carpeta_expresiones(pid):
    return os.path.join(P.PERSONAJES_ARTE, P.PERSONAJES[pid][1], "Expresiones")


def bloque_local(pid):
    prefab, carpeta, _ = P.PERSONAJES[pid]
    return """
==================== Sesion local (Santiago), con el Editor abierto ====================
 0. git pull   (trae esta rama con la cara, arte_final.json, rig_articulaciones.json y clips_personajes.json)
 1. Copiar el generador (el Editor crea el .meta solo, nunca a mano):
      Copy-Item claudeDocs/tasks/Personajes/herramientas/BuildRigsFinal.cs.txt Assets/Editor/ClaudeBuildRigsFinal.cs
 2. Recompilar y esperar:   pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 recompile
 3. Por coplay execute_script, EN ESTE ORDEN (cada una devuelve su log):
      ClaudeBuildRigsFinal.Execute("nodos")      // anade CaraBase (primer hijo de Cabeza) si el prefab aun no lo tiene
      ClaudeBuildRigsFinal.Execute("sprites")    // asigna los PNG de Expresiones/, rect comun, y crea o actualiza char_%(pid)s_cara.asset
      ClaudeBuildRigsFinal.Execute("estado")
 4. Comprobar el set de cara: Assets/Game/Art/Characters/%(carpeta)s/Expresiones/char_%(pid)s_cara.asset (Neutral = ojos_neutra y boca_0) y
    que la Image de CaraBase, Ojos y Boca de %(prefab)s.prefab tenga sprite y el mismo rect.
    git diff -U0 Assets/Game/Prefabs/Characters   (no debe QUITAR lineas «--- !u!»)
 5. Pruebas:
      pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-edit CharacterRig_
      pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-play CharacterRig
 6. Borrar el andamiaje:   Remove-Item Assets/Editor/ClaudeBuildRigsFinal.cs, Assets/Editor/ClaudeBuildRigsFinal.cs.meta
 7. git add Assets/Game/Art/Characters/%(carpeta)s Assets/Game/Prefabs/Characters/%(prefab)s.prefab Assets/Game/Prefabs/Characters/%(prefab)s.prefab.meta
      git add claudeDocs/tasks/Personajes/herramientas/arte_final.json claudeDocs/tasks/Personajes/herramientas/rig_articulaciones.json
      git add claudeDocs/tasks/Personajes/herramientas/clips_personajes.json
========================================================================================
""" % {"pid": pid, "prefab": prefab, "carpeta": carpeta}


def corre(args, etiqueta):
    print("\n$ %s" % " ".join(os.path.basename(a) if i == 1 else a for i, a in enumerate(args)))
    r = subprocess.run(args, cwd=AQUI, capture_output=True, text=True)
    lineas = [l for l in r.stdout.splitlines() if l.strip()]
    fallas = [l for l in lineas if "FALLA" in l]
    for l in (fallas + [l for l in lineas[-3:] if l not in fallas]) if fallas else lineas[-4:]:
        print("   " + l[:200])
    if r.returncode:
        print("   %s: salio con codigo %d %s" % (etiqueta, r.returncode, r.stderr.strip()[-300:]))
    return r.returncode


def actualiza_tabla(pid, rect, con_base=True):
    """Pone el rect comun en CaraBase, Ojos y Boca de la entrada de arte_final.json (CaraBase se crea si falta, antes de Ojos)."""
    personajes = A.cargar_arte_final()
    if pid not in personajes:
        raise KeyError("%s no tiene entrada en arte_final.json: primero preparar_arte_final.py %s <carpeta>" % (pid, pid))
    nodos = personajes[pid]["nodos"]
    padre = A.T + "/Cuello/Cabeza"
    centro = [(rect[0] + rect[2]) // 2, (rect[1] + rect[3]) // 2]
    if not any(n["nombre"] == "CaraBase" for n in nodos):
        i = next(k for k, n in enumerate(nodos) if n["nombre"] == "Ojos")
        nodos.insert(i, {"nombre": "CaraBase", "tipo": "imagen", "padre": padre, "punto": centro, "imagen": "CaraBase",
                         "sprite": "char_%s_cara_base" % pid, "rect": list(rect)})
    for n in nodos:
        if n["nombre"] in ("CaraBase", "Ojos", "Boca"):
            n["rect"], n["punto"] = list(rect), centro
    A.guardar_arte_final(personajes)


def rect_vigente(pid):
    """El rect comun que ya tienen Ojos y Boca en arte_final.json si su PNG neutro esta puesto, o None."""
    if P._png_de(P.PERSONAJES[pid][1], "char_%s_ojos_neutra" % pid) is None:
        return None
    n = next((n for n in A.cargar_arte_final().get(pid, {}).get("nodos", []) if n["nombre"] == "Ojos"), None)
    return list(n["rect"]) if n else None


def aplica(pid, emocion, res, col, args, hay_base):
    carpeta = carpeta_expresiones(pid)
    os.makedirs(carpeta, exist_ok=True)
    ojos_n, boca_n = EMOCIONES[emocion]
    print("\n== Aplicando al repo ==")
    escribe = []
    if ojos_n:
        escribe.append(("ojos", ojos_n))
    if boca_n:
        escribe.append(("boca", boca_n))
    if args.base or not hay_base:
        escribe.append(("base", "cara_base"))
    for clave, nombre in escribe:
        ruta = os.path.join(carpeta, "char_%s_%s.png" % (pid, nombre))
        existia = os.path.isfile(ruta)
        res.capas_salida[clave].save(ruta)
        print("  %s %s" % ("sustituye" if existia else "nuevo    ", os.path.relpath(ruta, P.RAIZ)))
    actualiza_tabla(pid, col.rect)
    print("  arte_final.json: rect %s en CaraBase, Ojos y Boca de %s" % (col.rect, pid))
    py = sys.executable
    fallos = 0
    fallos += 1 if corre([py, os.path.join(AQUI, "articulaciones.py")], "articulaciones") else 0
    fallos += 1 if corre([py, os.path.join(AQUI, "coreografia.py"), "--valida"], "coreografia") else 0
    fallos += 1 if corre([py, os.path.join(AQUI, "pose_preview.py"), "--solo", pid, "--salida", args.salida], "pose_preview") else 0
    print("\n%s" % ("TODO EN VERDE" if not fallos else "%d pasos fallaron: revisa antes de entregar" % fallos))
    print(bloque_local(pid))
    return fallos


# ============================================================================ 6. la autoprueba


def cara_sintetica(k=1):
    """
    Una cara con piezas conocidas, a la manera de la entrega (cejas, ojos con parpadeo y pupila, nariz, boca con lengua rosada,
    rubor semitransparente con rayitas) en un lienzo de 256k. Devuelve (imagen RGBA, {capa: mascara L de lo que le toca}).
    """
    n = 256 * k
    capas = {c: Image.new("RGBA", (n, n), (0, 0, 0, 0)) for c in CAPAS}
    S = lambda *v: tuple(int(round(x * k)) for x in v)
    # ojos y cejas
    d = ImageDraw.Draw(capas["ojos"])
    for cx in (70, 188):
        d.ellipse(S(cx - 38, 100, cx + 38, 148), fill=(255, 255, 255, 255), outline=(0, 0, 0, 255), width=2 * k)
        d.ellipse(S(cx - 18, 102, cx + 18, 146), fill=(224, 128, 32, 255), outline=(60, 30, 10, 255), width=k)
        d.ellipse(S(cx - 8, 112, cx + 8, 136), fill=(0, 0, 0, 255))
        d.polygon(S(cx - 40, 78, cx + 6, 70, cx + 36, 86, cx - 4, 90), fill=(110, 55, 20, 255), outline=(0, 0, 0, 255))
    # nariz
    d = ImageDraw.Draw(capas["nariz"])
    d.arc(S(118, 138, 140, 152), 20, 160, fill=(0, 0, 0, 255), width=2 * k)
    # boca: sonrisa oscura con lengua rosada entre los labios y comisuras
    d = ImageDraw.Draw(capas["boca"])
    d.chord(S(82, 140, 176, 188), 0, 180, fill=(40, 10, 10, 255))
    d.ellipse(S(112, 166, 146, 182), fill=(255, 120, 140, 255))
    d.arc(S(82, 140, 176, 188), 0, 180, fill=(0, 0, 0, 255), width=2 * k)
    d.line(S(80, 156, 86, 164), fill=(0, 0, 0, 255), width=2 * k)
    d.line(S(172, 164, 178, 156), fill=(0, 0, 0, 255), width=2 * k)
    # rubor: dos manchas rosadas blandas con rayitas oscuras
    for cx in (42, 214):
        m = Image.new("L", (n, n), 0)
        ImageDraw.Draw(m).ellipse(S(cx - 24, 148, cx + 24, 176), fill=190)
        m = m.filter(ImageFilter.GaussianBlur(3 * k))
        rosa = Image.new("RGBA", (n, n), (255, 90, 105, 255))
        rosa.putalpha(m)
        capas["rubor"].alpha_composite(rosa)
        d = ImageDraw.Draw(capas["rubor"])
        for dx in (-10, 0, 10):
            d.line(S(cx + dx - 3, 153, cx + dx + 3, 169), fill=(0, 0, 0, 200), width=k)
    # la imagen y la verdad por capa
    im = Image.new("RGBA", (n, n), (0, 0, 0, 0))
    for c in ("rubor", "ojos", "boca", "nariz"):
        im.alpha_composite(capas[c])
    verdad = {c: _binaria(capas[c].getchannel("A"), lambda v: v >= UMBRAL_ALFA) for c in CAPAS}
    return im, verdad


def cabeza_sintetica():
    """
    Una cabeza como la del Nino: pelo con flequillo irregular, ovalo de piel con contorno y orejas aparte. Devuelve
    (imagen RGBA 535x456, ovalo (x0, y0, x1, y1), eje, y mas baja del flequillo, barbilla).
    """
    im = Image.new("RGBA", (535, 456), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    piel = (255, 200, 160, 255)
    d.ellipse([92, 140, 460, 452], fill=(0, 0, 0, 255))                    # contorno de la cara
    d.ellipse([96, 144, 456, 448], fill=piel)
    for ex in (60, 480):                                                  # orejas, separadas por el contorno
        d.ellipse([ex - 24, 280, ex + 24, 350], fill=(0, 0, 0, 255))
        d.ellipse([ex - 20, 284, ex + 20, 346], fill=piel)
    # pelo: cubre la parte de arriba hasta un borde de abajo irregular (el punto mas bajo, y = 300)
    borde = [(60, 300), (120, 250), (170, 285), (230, 230), (290, 300), (340, 240), (400, 280), (470, 250), (500, 300)]
    d.polygon([(40, 60), (500, 60)] + borde[::-1] + [(40, 300)], fill=(110, 55, 20, 255))
    d.line(borde, fill=(0, 0, 0, 255), width=4)
    return im, (96, 144, 456, 448), 276.0, 300.0, 448.0


def autoprueba():
    malos = 0
    print("%-46s %s" % ("separar una cara sintetica", "IoU de cada capa con lo conocido (tope: > 0,96)"))
    for k in (1, 3):
        im, verdad = cara_sintetica(k)
        res = separa(im)
        for capa in ("ojos", "boca"):
            a = _binaria(res.capas[capa].getchannel("A"), lambda v: v >= UMBRAL_ALFA)
            inter = _y(a, verdad[capa]).histogram()[255]
            union = ImageChops.lighter(a, verdad[capa]).histogram()[255]
            iou = inter / float(union)
            ok = iou > 0.96
            malos += 0 if ok else 1
            print("%-46s %.4f  %s" % ("lienzo %dx%d: %s" % (256 * k, 256 * k, capa), iou, "bien" if ok else "FALLA"))
        base = _binaria(res.capas["base"].getchannel("A"), lambda v: v >= UMBRAL_ALFA)
        verdad_base = ImageChops.lighter(verdad["nariz"], verdad["rubor"])
        iou = _y(base, verdad_base).histogram()[255] / float(ImageChops.lighter(base, verdad_base).histogram()[255])
        ok = iou > 0.96
        malos += 0 if ok else 1
        print("%-46s %.4f  %s" % ("lienzo %dx%d: nariz + rubor (con rayitas)" % (256 * k, 256 * k), iou, "bien" if ok else "FALLA"))
    # la lengua rosada va a la boca, no al rubor
    im, verdad = cara_sintetica(1)
    res = separa(im)
    tocan = res.capas["boca"].getpixel((130, 174))[3] > 200 and res.capas["base"].getpixel((130, 174))[3] == 0
    malos += 0 if tocan else 1
    print("%-46s %s" % ("la lengua rosada es de la boca", "bien" if tocan else "FALLA"))
    # --asigna: borrar la nariz por caja, y pasar a la boca algo que la heuristica puso en los ojos
    res2 = separa(im, [("nada", (116, 136, 142, 154))])
    ok = res2.resumen["nariz"]["area"] == 0 and res2.capas["base"].crop((116, 136, 142, 154)).getchannel("A").getbbox() is None
    malos += 0 if ok else 1
    print("%-46s %s" % ("--asigna nada:caja borra la nariz", "bien" if ok else "FALLA"))
    # la cara no se parte por tamano: lo que mas pesa en cada capa no cambia al triplicar el lienzo
    # --- la cabeza
    cab_im, ovalo, eje, flequillo, barbilla = cabeza_sintetica()
    rect = (224, 77, 759, 533)
    cab = mide_cabeza(cab_im, rect)
    esperado = {"ovalo x0": (cab.ovalo[0], rect[0] + ovalo[0]), "ovalo x1": (cab.ovalo[2], rect[0] + ovalo[2]),
                "eje": (cab.eje, rect[0] + eje), "barbilla": (cab.barbilla, rect[1] + barbilla),
                "flequillo": (cab.flequillo, rect[1] + flequillo)}
    print("%-46s %s" % ("medir una cabeza sintetica", "medido / conocido (tolerancia 4 px)"))
    for nombre, (medido, real) in esperado.items():
        ok = abs(medido - real) <= 4.0
        malos += 0 if ok else 1
        print("%-46s %7.1f / %7.1f  %s" % (nombre, medido, real, "bien" if ok else "FALLA"))
    # --- la colocacion: centrada en el eje, cejas bajo el flequillo
    res = separa(im)
    cols, rec = elige(cab, res)
    c = cols[rec]
    centro = (c.rect[0] + c.rect[2]) / 2.0
    ojos = res.resumen["ojos"]["caja"]
    cx_ojos = c.rect[0] + ((ojos[0] + ojos[2]) / 2.0) * c.s
    ok = abs(cx_ojos - cab.eje) <= 2.0 and c.margenes["cejas bajo el flequillo"] >= 0
    malos += 0 if ok else 1
    print("%-46s %s" % ("la cara queda en el eje, bajo el flequillo", "bien (eje %.1f, ojos %.1f)" % (cab.eje, cx_ojos) if ok else "FALLA"))
    del centro
    # el rect no depende del tamano del lienzo de la imagen: la misma cara a 256 y a 768 da el mismo rect
    res3 = separa(cara_sintetica(3)[0])
    c3 = coloca(cab, res3, c.fraccion)
    ok = all(abs(a - b) <= 3 for a, b in zip(c.rect, c3.rect))
    malos += 0 if ok else 1
    print("%-46s %s" % ("el rect no depende del tamano de la imagen", "bien %s / %s" % (c.rect, c3.rect) if ok else "FALLA %s / %s" % (c.rect, c3.rect)))
    print("la herramienta recupera lo conocido" if not malos else "%d casos mal" % malos)
    return 1 if malos else 0


# ============================================================================ 7. principal


def parsea_asigna(textos):
    out = []
    for t in textos or []:
        m = re.fullmatch(r"(ojos|boca|nariz|rubor|nada):(\d+),(\d+),(\d+),(\d+)", t.strip())
        if not m:
            raise SystemExit("--asigna espera capa:x0,y0,x1,y1 con capa = ojos|boca|nariz|rubor|nada (recibi %r)" % t)
        out.append((m.group(1), tuple(int(m.group(i)) for i in range(2, 6))))
    return out


def main(argv=None):
    ap = argparse.ArgumentParser(description="De una imagen de cara completa a las capas CaraBase, Ojos y Boca del rig")
    ap.add_argument("id", nargs="?", help="personaje: nino (papa, mama o nina cuando tengan cabeza propia)")
    ap.add_argument("emocion", nargs="?", help="|".join(EMOCIONES))
    ap.add_argument("imagen", nargs="?", help="PNG de la cara completa, fondo transparente")
    ap.add_argument("--escala", type=float, help="fraccion del ancho del ovalo que ocupa la cara (0.4 a 0.8); por defecto la mayor de 0.50/0.60/0.70 que cabe")
    ap.add_argument("--aplicar", action="store_true")
    ap.add_argument("--base", action="store_true", help="reescribe tambien char_<x>_cara_base")
    ap.add_argument("--reubica", action="store_true", help="recalcula el rect comun aunque la cara ya este puesta")
    ap.add_argument("--asigna", action="append", metavar="CAPA:X0,Y0,X1,Y1", help="corrige a mano: lo que cae en esa caja (px de la imagen) pasa a esa capa")
    ap.add_argument("--max-lado", type=int, default=512)
    ap.add_argument("--salida", default=os.path.join(tempfile.gettempdir(), "algoritmia_expresion"))
    ap.add_argument("--autoprueba", action="store_true")
    a = ap.parse_args(argv)
    if a.autoprueba:
        return autoprueba()
    if not (a.id and a.emocion and a.imagen):
        ap.error("hace falta <id> <emocion> <imagen> (o --autoprueba)")
    emocion = {"a": "boca_a", "e": "boca_e", "u": "boca_u"}.get(a.emocion, a.emocion)
    if emocion not in EMOCIONES:
        ap.error("emocion %r desconocida: %s" % (a.emocion, ", ".join(EMOCIONES)))
    if a.id not in P.FAMILIA:
        ap.error("id %r desconocido: %s" % (a.id, ", ".join(P.FAMILIA)))
    if not os.path.isfile(a.imagen):
        ap.error("no existe %s" % a.imagen)

    rig = P.cargar_rig()
    pj_rig = P.personaje_rig(rig, a.id)
    carpeta = P.PERSONAJES[a.id][1]
    entrada = Image.open(a.imagen).convert("RGBA")
    print("== %s / %s: %s (%dx%d)" % (a.id, emocion, a.imagen, entrada.width, entrada.height))
    res = separa(entrada, parsea_asigna(a.asigna))
    informe_separacion(res)
    # lo que se escribe: el lienzo entero (reducido a --max-lado), igual para las tres capas
    res.capas_salida = {k: reduce(v, a.max_lado) for k, v in res.capas.items()}
    k = res.capas_salida["ojos"].width / float(entrada.width)
    print("  lienzo de salida: %dx%d%s" % (res.capas_salida["ojos"].width, res.capas_salida["ojos"].height,
                                          "" if k >= 1.0 else " (reducido x%.3f: --max-lado %d)" % (k, a.max_lado)))

    png_cab = P._png_de(carpeta, "char_%s_parte_cabeza" % a.id)
    cuello = next((n for n in pj_rig["nodos"] if n["nombre"] == "Cuello"), None)
    if png_cab is None or cuello is None:
        print("\nNo hay char_%s_parte_cabeza.png (la cabeza propia llega con el arte final: preparar_arte_final.py %s <carpeta>): no puedo colocar la cara." % (a.id, a.id))
        return 2
    cab = mide_cabeza(png_cab, cuello["rect"])
    print("\n== La cabeza (%s)" % os.path.relpath(png_cab, P.RAIZ))
    print("  piel RGB %s | ovalo x %.0f..%.0f, y %.0f..%.0f (ancho %.0f) | eje x %.1f | flequillo hasta y %.0f | barbilla y %.0f" % (
        cab.piel, cab.ovalo[0], cab.ovalo[2], cab.ovalo[1], cab.ovalo[3], cab.ovalo[2] - cab.ovalo[0], cab.eje, cab.flequillo, cab.barbilla))

    heredado = None if a.reubica else rect_vigente(a.id)
    cols, rec = elige(cab, res, a.escala)
    if heredado is not None and not a.reubica:
        print("\nLa cara neutra ya esta puesta: esta emocion hereda su rect %s (--reubica lo recalcula)." % heredado)
        w, h = res.tamano
        asp_p, asp_h = (heredado[2] - heredado[0]) / float(heredado[3] - heredado[1]), w / float(h)
        if abs(asp_p / asp_h - 1.0) > 0.02:
            print("AVISO la imagen no tiene la proporcion del lienzo de la neutra (%.3f contra %.3f): saldria deformada" % (asp_h, asp_p))
        col = Colocacion()
        col.rect, col.s, col.fraccion = heredado, (heredado[2] - heredado[0]) / float(w), 0.0
        cols, rec = [col], 0
    print("\n== Escalas candidatas (la cara = el ancho de ojos y cejas; el rect es el del lienzo de la imagen, en el de 1024)")
    for i, c in enumerate(cols):
        marcas = ", ".join("%s %+.0f px" % (n, v) for n, v in c.margenes.items())
        print("  %s cara = %s del ovalo | S %.3f | rect %s | %s%s" % (">>" if i == rec else "  ", "%3.0f %%" % (100 * c.fraccion) if c.fraccion else "(la de la neutra)",
                                                                  c.s, c.rect, marcas, "" if c.cabe else "   (NO CABE)"))
    print("  recomendada: %s" % ("%d %%" % round(100 * cols[rec].fraccion) if cols[rec].fraccion else "la heredada"))

    rutas = composites(a.id, emocion, res, cab, cols, rec, rig, a.salida)
    print("\ncomposites:")
    for r in rutas:
        print("  " + r)
    ojos_n, boca_n = EMOCIONES[emocion]
    hay_base = P._png_de(carpeta, "char_%s_cara_base" % a.id) is not None
    print("\nescribiria (con --aplicar), en Assets/Game/Art/Characters/%s/Expresiones/:" % carpeta)
    for n in (ojos_n, boca_n, "cara_base" if (a.base or not hay_base) else None):
        if n:
            print("  char_%s_%s.png" % (a.id, n))
    print("  arte_final.json: rect %s en CaraBase, Ojos y Boca; y regenera rig_articulaciones.json y clips_personajes.json" % cols[rec].rect)
    if not a.aplicar:
        return 0
    return 1 if aplica(a.id, emocion, res, cols[rec], a, hay_base) else 0


if __name__ == "__main__":
    sys.exit(main())
