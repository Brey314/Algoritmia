#!/usr/bin/env python3
# preparar_expresion.py: de UNA imagen de cara completa a las capas del rig, con un comando.
#
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_expresion.py <id> <emocion> <imagen>            # informe y composites
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_expresion.py <id> <emocion> <imagen> --aplicar  # escribe
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_expresion.py <id> abiertos <imagen> --vista perfil   # (INC-134) la cara del cuerpo de perfil
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_expresion.py --autoprueba                       # se prueba solo
#         <id> = nino (el que ya tiene cabeza propia: papa, mama y nina cuando entreguen su arte final)
#         <emocion> = neutra | alegria | sorpresa | preocupacion | concentracion | sueno | parpadeo_medio | parpadeo_cerrado
#                     (y las bocas de hablar: boca_a | boca_e | boca_u, que solo escriben la boca). ALIAS (09/10/2026, la entrega nueva llama a las
#                     expresiones «ojos abiertos» y «ojos cerrados»): abiertos = neutra y cerrados = parpadeo_cerrado (tambien abierto y cerrado).
#                     El parpadeo es de DOS cuadros (INC-135): ojos_neutra (abiertos) y ojos_parpadeo_cerrado; el cuadro medio ya no hace falta.
#         --vista frente|perfil   (INC-134, 09/10/2026) frente = lo de siempre. perfil = la cara del cuerpo de PERFIL: escribe en
#                       Assets/Game/Art/Characters/<Carpeta>/Perfil/ char_<id>_perfil_ojos_<...>, char_<id>_perfil_boca_<...> y
#                       char_<id>_perfil_cara_base, y anota el rect en los nodos CaraBase, Ojos y Boca de la entrada «perfil» de arte_final.json. Solo por
#                       REGISTRO (no hay heuristica de cabeza de perfil): sale de arte_final.json, «perfil.registro» (la transformacion lienzo-de-entrega ->
#                       lienzo-del-rig de la entrega de perfil, que escribe preparar_perfil.py) o, si la entrega de perfil comparte lienzo con la de
#                       frente, de «registro». La caja de la cara de perfil se guarda APARTE, en «registro.cara_perfil» (coordenadas del lienzo de la entrega
#                       de perfil): la caja de frente, «registro.cara», no se toca. Si la imagen no esta en el lienzo del registro, se rechaza. La
#                       separacion en ojos, boca y base es la misma heuristica de siempre (nariz en la franja central…): en un perfil la nariz y la
#                       boca caen al borde de delante, asi que lo normal es revisar el informe y corregir con --asigna; las
#                       composites muestran la cara sobre char_<id>_perfil_cabeza si ya esta puesta.
#                       LA CARA DE PERFIL SE SEPARA CON separa_perfil() (no con separa(), que supone una cara de frente): rubor = los pixeles rosados (sin el blanco
#                       del ojo); boca = la mayor componente (8 vecinos, sin dilatar) cuyo centro cae en el 30 % de abajo de la cara, con lo que le cuelga
#                       (labios rosados, comisuras, el pliegue); ojos = todo lo demas (ojo, parpados, cejas). cara_base de perfil = SOLO el rubor: Papa no tiene y
#                       no se escribe. La boca y el rubor salen una sola vez, de la expresion abierta; la cerrada solo escribe los ojos. El registro de perfil
#                       trae «espejo: true» (lo escribe preparar_perfil.py: las cuatro entregas miran a la izquierda): la expresion se ESPEJA igual que las partes
#                       antes de recortarla. La caja comun de la cara sale de la expresion ABIERTA (haz primero «abiertos» y despues «cerrados»).
#         --limpia-cerrados / --no-limpia-cerrados   (por defecto SI; Santiago, 09/10/2026) la cara de ojos cerrados trae un rastro casi blanco del ojo abierto
#                       bajo los parpados (Nino 241 px, un arco tenue): se quitan los pixeles casi blancos (R, G y B > 215) y su halo (los vecinos claros y
#                       semitransparentes), que no son trazo, y se informa cuantos fueron. Solo en la emocion parpadeo_cerrado de la vista de perfil.
#         --registrada [CARPETA_FRENTE]   (06/10/2026) la expresion llega en el MISMO lienzo que las partes (1300x1500, registradas entre si): su
#                       sitio sale de la transformacion lienzo-de-entrega -> lienzo-del-rig que preparar_arte_final.py uso con las partes
#                       (arte_final.json, «registro»), no de una heuristica. Se activa SOLA si el personaje tiene registro y la imagen mide lo
#                       mismo; --sin-registro la apaga. Ver «MODO REGISTRADO» abajo. La heuristica (--escala...) queda de respaldo para entregas sueltas.
#         --densidad D  (registrada) texeles por px del lienzo del rig; 1 por defecto = la densidad de las partes del cuerpo
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
# MODO REGISTRADO (06/10/2026). Santiago exporta cada expresion en el lienzo de las partes, asi que la cara ya esta en su sitio: no se mide
# nada en la cabeza ni se elige escala. Se recorta una CAJA COMUN de la cara (la de lo que pinta la neutra con MARGEN_CARA, recortada a la
# cabeza; se guarda en arte_final.json, «registro.cara», y las demas expresiones la heredan: si una se sale, se rechaza en vez de desalinear),
# se separa en ojos, boca y base como siempre, se reduce a DENSIDAD texeles por px del lienzo del rig (la de las partes: 1:1) con tope --max-lado
# (512), y el rect comun de las tres capas es la caja mapeada con la MISMA transformacion que las partes (preparar_arte_final.caja_a_lienzo).
# Por que recortar y no guardar el lienzo entero de 1300x1500: la cara ocupa el 25 % de la cabeza; el lienzo entero a 1:1 serian 1014x1170 texeles
# casi vacios por capa y a 512 de lado la cara quedaria a 160 px. Con la caja la textura mide ~350x270 y no cambia de tamano entre emociones
# (RNF-06: cada capa sin comprimir pesa 0,3-0,4 MB; ~13 capas por personaje).
#
# POR QUE (modo heuristica) no se recorta ni se cuenta con 256: la imagen puede llegar mas grande (un original); el tamano solo sale de --max-lado, y
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
import unicodedata
from collections import Counter

try:
    from PIL import Image, ImageChops, ImageDraw, ImageFilter, ImageOps
except ImportError:  # pragma: no cover
    sys.exit("hace falta Pillow: pip install pillow")

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import articulaciones as A  # noqa: E402
import prefabs as P  # noqa: E402
import preparar_arte_final as F  # noqa: E402
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
# Lo que la entrega nueva (09/10/2026) llama «ojos abiertos» y «ojos cerrados»: los dos cuadros del parpadeo (INC-135). Y las bocas de hablar,
# con la letra sola.
ALIAS_EMOCION = {"abiertos": "neutra", "abierto": "neutra", "cerrados": "parpadeo_cerrado", "cerrado": "parpadeo_cerrado",
                 "a": "boca_a", "e": "boca_e", "u": "boca_u"}
VISTAS = ("frente", "perfil")
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
# Modo REGISTRADO (06/10/2026): la expresion llega en el mismo lienzo que las partes. Lo que se guarda como «la cara» es una caja de ese lienzo
# —la de lo que pinta la neutra, con estos margenes por si otra expresion sube las cejas o abre la boca (fraccion del ancho y del alto del
# contenido: izquierda, arriba, derecha, abajo), recortada a la cabeza— y es COMUN a todas las expresiones del personaje (se guarda en
# arte_final.json, «registro.cara»).
MARGEN_CARA = (0.08, 0.25, 0.08, 0.35)
DENSIDAD = 1.0   # texeles de la textura por pixel del lienzo del rig: la misma densidad que las partes del cuerpo (que se guardan a escala 1:1)


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


def separa_perfil(im, correcciones=()):
    """
    Separa una cara de PERFIL (INC-134) en ojos (con cejas), boca y rubor; la cabeza (nariz, oreja) trae la nariz, asi que no hay capa de nariz. Devuelve una
    Separacion como separa(): capas «ojos», «boca» y «base» (= solo el rubor) y el resumen de CAPAS (nariz siempre vacia). La regla, comprobada en las ocho
    imagenes de la entrega del 09/10/2026 (separa() no sirve: supone la boca en el centro y una nariz en la capa):
      - rubor: los pixeles rosados (la misma prueba HSV de separa(), que deja fuera el blanco del ojo);
      - boca: la mayor componente (8 vecinos, SIN dilatar: dilatada se une al contorno de la cara y a los ojos) cuyo centro cae en el 30 % de abajo de la
        caja de la cara; y lo que le cuelga: componentes cuyo centro cae en su caja ensanchada (comisuras, el pliegue) y las manchas rosadas de los labios;
      - ojos: todo lo demas (el ojo, los parpados, las cejas); lo que cae dentro de una mancha de rubor (rayitas, aliasing) es del rubor.
    «correcciones» = [(capa, (x0, y0, x1, y1))] en pixeles de la imagen, como en separa().
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
    res.eje = (caja[0] + caja[2]) / 2.0
    d = max(1, int(round(0.006 * max(W, H))))
    th, ts, tv = im.convert("RGB").convert("HSV").split()
    brillante = _y(_binaria(ts, lambda v: v >= SAT_ROSA * 255), _binaria(tv, lambda v: v >= VAL_ROSA * 255), _binaria(alfa, lambda v: v >= 8))
    rosa = ImageChops.lighter(
        _y(_binaria(th, lambda v: v >= TONO_ROSA[0] or v <= TONO_ROSA[1]), brillante),
        _y(_binaria(th, lambda v: v >= TONO_ROSA[0] or v <= TONO_ROSA_BORDE), _binaria(alfa, lambda v: v < ALFA_BORDE), brillante))
    oscuro = ImageChops.subtract(_binaria(alfa, lambda v: v >= 8), rosa)
    rot_o, comp_o = etiqueta(oscuro)
    rot_r, comp_r = etiqueta(_dilata(rosa, 2 * d))
    bajas = [c for c in comp_o.values() if c.cy >= caja[1] + 0.70 * H]
    boca = max(bajas, key=lambda c: c.area) if bajas else None
    parches = []
    if boca is None:
        res.avisos.append("no encuentro la boca (una componente oscura cuyo centro caiga en el 30 % de abajo de la cara); usa --asigna boca:x0,y0,x1,y1")
        caja_b = None
    else:
        boca.capa = "boca"
        ex, ey = 0.08 * W, 0.10 * H
        caja_b = (boca.x0 - ex, boca.y0 - ey, boca.x1 + ex, boca.y1 + ey)
    for c in comp_r.values():
        c.capa = "boca" if (caja_b is not None and c.dentro_de(caja_b)) else "rubor"
        if c.capa == "rubor":
            parches.append(c)
    for c in comp_o.values():
        if c.capa is not None:
            continue
        if caja_b is not None and c.dentro_de(caja_b) and c.area < 0.5 * boca.area:
            c.capa = "boca"
        elif any(c.dentro_de(p.caja, 2 * d) and c.ancho <= p.ancho + 4 * d and c.alto <= p.alto + 4 * d for p in parches):
            c.capa = "rubor"
        else:
            c.capa = "ojos"
    tabla_o = bytes(CODIGO[comp_o[i].capa] if i in comp_o and comp_o[i].capa else 0 for i in range(256))
    tabla_r = bytes(CODIGO[comp_r[i].capa] if i in comp_r and comp_r[i].capa else 0 for i in range(256))
    cl_o = _y(Image.frombytes("L", (w, h), bytes(rot_o).translate(tabla_o)), _binaria(oscuro, lambda v: v))
    cl_r = _y(Image.frombytes("L", (w, h), bytes(rot_r).translate(tabla_r)), _binaria(rosa, lambda v: v))
    clases = bytearray(ImageChops.add(cl_o, cl_r).tobytes())
    datos_a = alfa.tobytes()
    for capa, (x0, y0, x1, y1) in correcciones:
        x0, y0, x1, y1 = max(0, int(x0)), max(0, int(y0)), min(w, int(x1)), min(h, int(y1))
        for y in range(y0, y1):
            for x in range(x0, x1):
                if datos_a[y * w + x]:
                    clases[y * w + x] = CODIGO[capa]
    clase_im = Image.frombytes("L", (w, h), bytes(clases))
    res.clases = clase_im

    def capa_de(codigos):
        mascara = clase_im.point(lambda v, cs=tuple(codigos): 255 if v in cs else 0)
        out = im.copy()
        out.putalpha(ImageChops.multiply(alfa, mascara))
        return out

    for nombre, codigos in (("ojos", (1,)), ("boca", (2,)), ("base", (3, 4))):
        res.capas[nombre] = capa_de(codigos)
    for nombre in CAPAS:
        m = clase_im.point(lambda v, k=CODIGO[nombre]: 255 if v == k else 0)
        vis = ImageChops.multiply(m, opaco)
        res.resumen[nombre] = {"area": vis.histogram()[255], "caja": vis.getbbox(),
                               "componentes": sum(1 for c in list(comp_o.values()) + list(comp_r.values()) if c.capa == nombre)}
    perdidos = _y(_binaria(alfa, lambda v: v >= UMBRAL_ALFA), clase_im.point(lambda v: 255 if v == 0 else 0)).histogram()[255]
    if perdidos:
        res.avisos.append("%d pixeles opacos sin capa (aislados de todo): se descartan; --asigna los recoge" % perdidos)
    if res.resumen["rubor"]["area"] == 0:
        res.avisos.append("no hay rubor: la cara de perfil no lleva cara_base (se espera en Papa)")
    return res


BLANCO_CASI = 215     # un pixel con R, G y B por encima de esto es «casi blanco»: el blanco del ojo abierto
HALO_CLARO = 150      # el halo de ese blanco: vecinos mas claros que esto...
HALO_ALFA = 200       # ...y semitransparentes (por debajo de esto)
HALO_RADIO = 2        # a esta distancia del blanco (px) como mucho


def limpia_cerrados(im):
    """
    El rastro casi blanco que el ojo ABIERTO deja en la cara de ojos CERRADOS (un arco tenue bajo los parpados): quita (alfa a cero) los pixeles casi blancos
    —R, G y B por encima de BLANCO_CASI— y su halo (los vecinos a HALO_RADIO pixeles, claros y semitransparentes: el borde difuso del blanco). El trazo es oscuro y
    no se toca: ni los parpados ni las cejas ni el contorno. Devuelve (imagen limpia, pixeles casi blancos quitados, pixeles de halo quitados); los
    conteos son de pixeles con alfa >= 12 (lo visible).
    """
    im = im.convert("RGBA")
    r, g, b, a = im.split()
    minimo = ImageChops.darker(ImageChops.darker(r, g), b)
    blanco = _y(_binaria(minimo, lambda v: v > BLANCO_CASI), _binaria(a, lambda v: v > 0))
    cerca = _dilata(blanco, HALO_RADIO)
    halo = _y(cerca, _binaria(minimo, lambda v: v > HALO_CLARO), _binaria(a, lambda v: 0 < v < HALO_ALFA), ImageChops.invert(blanco))
    quitar = ImageChops.lighter(blanco, halo)
    visible = _binaria(a, lambda v: v >= 12)
    n_blanco = _y(blanco, visible).histogram()[255]
    n_halo = _y(halo, visible).histogram()[255]
    out = im.copy()
    out.putalpha(ImageChops.multiply(a, ImageChops.invert(quitar)))
    return out, n_blanco, n_halo


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


def pliega(texto):
    """Minusculas y sin tildes ni enes (NFKD): para comparar nombres de archivo."""
    return "".join(c for c in unicodedata.normalize("NFKD", texto) if not unicodedata.combining(c)).lower()


def prefijo_de(pid, vista="frente"):
    """El prefijo de los sprites de la cara de una vista: char_<id> (frente) o char_<id>_perfil (perfil, INC-134)."""
    return "char_%s_perfil" % pid if vista == "perfil" else "char_%s" % pid


def carpeta_destino(pid, vista="frente"):
    """Donde escribe --aplicar la cara de esa vista: Expresiones/ (frente) o Perfil/ (perfil) de la carpeta del personaje. Lo mismo que dice P.subcarpeta_de por el nombre."""
    return os.path.join(P.PERSONAJES_ARTE, P.PERSONAJES[pid][1], "Perfil" if vista == "perfil" else "Expresiones")


def capas_de(pid, emocion, vista="frente", con_base=True):
    """[(capa, nombre del sprite)] que escribe esta emocion en esa vista: (ojos, boca y, con_base, base), sin las capas que esa emocion no cambia."""
    ojos_n, boca_n = EMOCIONES[emocion]
    pref = prefijo_de(pid, vista)
    capas = [("ojos", ojos_n), ("boca", boca_n), ("base", "cara_base" if con_base else None)]
    return [(clave, "%s_%s" % (pref, n)) for clave, n in capas if n]


def candidatas_png(res, cols, carpeta_png, pid, emocion, vista="frente"):
    """Escribe en carpeta_png los PNG (ya reducidos) y devuelve {nombre de sprite: ruta}: lo que PNG_EXTRA necesita."""
    salida = {}
    os.makedirs(carpeta_png, exist_ok=True)
    for clave, sprite in capas_de(pid, emocion, vista):
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


# ============================================================================ 4b. colocar la cara por REGISTRO


def registro_de(pid, carpeta_frente=None):
    """
    El registro lienzo-de-entrega -> lienzo-del-rig de un personaje. Con «carpeta_frente» se calcula como lo hizo preparar_arte_final.py con las
    partes (F.procesa) y se comprueba contra la tabla; sin ella sale de arte_final.json («registro»). Devuelve (registro o None, texto de donde
    salio, lista de errores).
    """
    entrada = A.cargar_arte_final().get(pid)
    if carpeta_frente:
        res = F.procesa(carpeta_frente, pid)
        if res.errores:
            return None, carpeta_frente, ["las partes de %s no se procesan: %s" % (carpeta_frente, "; ".join(res.errores))]
        registro = dict(res.registro)
        origen = "calculado de las partes de %s" % carpeta_frente
    elif entrada is not None and entrada.get("registro"):
        registro = dict(entrada["registro"])
        origen = "arte_final.json (registro)"
    else:
        return None, "", []
    errores = []
    if entrada is None:
        errores.append("%s no tiene entrada en arte_final.json: primero preparar_arte_final.py %s <carpeta> --aplicar" % (pid, pid))
        return registro, origen, errores
    cuello = next((n for n in entrada["nodos"] if n["nombre"] == "Cuello"), None)
    esperado = F.caja_a_lienzo(registro, registro["cabeza"])
    if cuello is None or max(abs(a - b) for a, b in zip(esperado, cuello["rect"])) > 1:
        errores.append("la cabeza de la entrega cae en %s con este registro y arte_final.json dice %s: las partes aplicadas no son las de esta "
                       "entrega (corre preparar_arte_final.py %s <carpeta> --aplicar)" % (esperado, cuello["rect"] if cuello else None, pid))
    if "cara" in entrada.get("registro", {}):
        registro["cara"] = list(entrada["registro"]["cara"])
    return registro, origen, errores


def registro_perfil(pid):
    """
    El registro de la entrega de PERFIL (INC-134): la transformacion lienzo-de-entrega -> lienzo-del-rig con la que se coloco el cuerpo de perfil de ese
    personaje. Sale de arte_final.json: «perfil.registro» (lo escribe preparar_perfil.py: lienzo, escala, tx, ty y, si la mide, cabeza) o, si la entrega
    de perfil comparte lienzo y registro con la de frente, el «registro» de frente. La caja de la cara de perfil (coordenadas de ESE lienzo) es
    «registro.cara_perfil»; si ya esta, entra como «cara» para que las demas expresiones la hereden. La caja de cara de FRENTE no entra aqui. Devuelve
    (registro o None, de donde salio, errores); sin errores y sin registro es que no hay nada que decir.
    """
    entrada = A.cargar_arte_final().get(pid)
    if entrada is None:
        return None, "", ["%s no tiene entrada en arte_final.json: primero preparar_arte_final.py %s <carpeta> --aplicar" % (pid, pid)]
    perfil = entrada.get("perfil")
    if perfil is None:
        return None, "", ["%s no tiene «perfil» en arte_final.json: primero preparar_perfil.py %s <carpeta> --aplicar (escribe sus piezas, sus nodos y su registro)" % (pid, pid)]
    if perfil.get("registro"):
        base, origen = perfil["registro"], "arte_final.json (perfil.registro)"
    elif entrada.get("registro"):
        base, origen = entrada["registro"], "arte_final.json (registro de frente: la entrega de perfil comparte su lienzo)"
    else:
        return None, "", ["%s no tiene registro: ni «perfil.registro» ni «registro» en arte_final.json" % pid]
    registro = {k: base[k] for k in ("lienzo", "escala", "tx", "ty", "cabeza", "espejo") if k in base}   # «espejo»: la entrega de perfil mira a la izquierda y las partes se espejaron (preparar_perfil.py)
    caja = entrada.get("registro", {}).get("cara_perfil")
    if caja:
        registro["cara"] = list(caja)
    faltan = [k for k in ("lienzo", "escala", "tx", "ty") if k not in registro]
    if faltan:
        return None, origen, ["el registro de perfil de %s no trae %s" % (pid, ", ".join(faltan))]
    return registro, origen, []


def caja_de_cara(entrada, registro, reubica=False):
    """
    La caja del lienzo de la entrega que se guarda como cara, y de donde salio. Si el registro ya trae «cara» (la fijo la neutra) esa; si no,
    la de lo que pinta ESTA imagen con MARGEN_CARA, recortada a la cabeza. Devuelve (caja entera, origen).
    """
    contenido = entrada.getchannel("A").point(lambda v: 255 if v >= UMBRAL_ALFA else 0).getbbox()
    if contenido is None:
        raise ValueError("la imagen esta vacia (todo transparente)")
    if registro.get("cara") and not reubica:
        return list(registro["cara"]), "la del registro (la fijo la neutra)", contenido
    w, h = contenido[2] - contenido[0], contenido[3] - contenido[1]
    ml, mt, mr, mb = MARGEN_CARA
    cab = registro.get("cabeza") or [0, 0, entrada.width, entrada.height]   # un registro de perfil puede no medir la cabeza: sin recorte a ella
    caja = [max(cab[0], int(math.floor(contenido[0] - ml * w))), max(cab[1], int(math.floor(contenido[1] - mt * h))),
            min(cab[2], int(math.ceil(contenido[2] + mr * w))), min(cab[3], int(math.ceil(contenido[3] + mb * h)))]
    return caja, "el contenido de esta imagen con margenes %s, recortado a la cabeza" % (MARGEN_CARA,), contenido


def reduce_a(im, k):
    """La imagen a k veces su tamano (LANCZOS); igual si k >= 1."""
    if k >= 1.0:
        return im
    return im.resize((max(1, int(round(im.width * k))), max(1, int(round(im.height * k)))), Image.LANCZOS)


def prepara_registrada(entrada, registro, args, vista="frente", limpia=False):
    """
    La expresion por REGISTRO: recorta la caja comun de la cara, separa ojos, boca y base y los reduce a la densidad de las partes. Devuelve
    (Separacion con capas_salida, Colocacion, caja de la cara, avisos). El rect de las tres capas es la caja mapeada con la MISMA
    transformacion que las partes: no hay heuristica de escala ni de sitio.
    INC-134: con vista = «perfil» la separacion es separa_perfil(); si el registro trae «espejo» (la entrega de perfil mira a la izquierda) la expresion se
    ESPEJA antes de recortar, igual que las partes, y la caja de la cara y la de la cabeza estan en el lienzo ya espejado; con «limpia» (la cara de ojos cerrados)
    se le quita antes el rastro casi blanco del ojo abierto (limpia_cerrados): res.limpieza = (casi blancos, halo).
    """
    if registro.get("espejo"):
        entrada = ImageOps.mirror(entrada)
    if list(entrada.size) != list(registro["lienzo"]):
        raise ValueError("la expresion mide %dx%d y las partes %dx%d: no estan en el mismo lienzo (sin registro, usa el modo por heuristica)"
                         % (entrada.width, entrada.height, registro["lienzo"][0], registro["lienzo"][1]))
    caja, origen, contenido = caja_de_cara(entrada, registro, args.reubica)
    avisos = []
    if not (caja[0] <= contenido[0] and caja[1] <= contenido[1] and caja[2] >= contenido[2] and caja[3] >= contenido[3]):
        raise ValueError("lo que pinta esta expresion %s se sale de la caja de la cara %s (%s): con otra caja la neutra y las que ya estan "
                         "puestas se desalinean; --reubica en la NEUTRA recalcula la caja (y hay que volver a exportar las demas)" % (contenido, caja, origen))
    recorte = entrada.crop(tuple(caja))
    limpieza = None
    if limpia:
        recorte, n_blanco, n_halo = limpia_cerrados(recorte)
        limpieza = (n_blanco, n_halo)
    res = (separa_perfil if vista == "perfil" else separa)(recorte, parsea_asigna(args.asigna))
    res.limpieza = limpieza
    s = registro["escala"]
    k = min(1.0, args.densidad * s, float(args.max_lado) / max(recorte.size))
    res.capas_salida = {c: reduce_a(v, k) for c, v in res.capas.items()}
    col = Colocacion()
    col.rect = F.caja_a_lienzo(registro, caja)
    col.s, col.fraccion = (col.rect[2] - col.rect[0]) / float(recorte.width), 0.0
    res.registro_info = {"caja": caja, "origen": origen, "contenido": contenido, "k": k, "salida": res.capas_salida["ojos"].size, "entrada": recorte.size,
                         "espejo": bool(registro.get("espejo"))}
    # cuanto cabe, para el informe: los extremos de ojos y cejas, la boca y el rubor dentro del ovalo de la cara y la franja de la barbilla
    return res, col, caja, avisos


# ============================================================================ 4c. la cara de PERFIL (INC-134)


def composites_perfil(pid, emocion, res, col, salida):
    """
    Los composites del informe de la cara de perfil: las tres capas separadas y la cara puesta en su rect, sobre char_<id>_perfil_cabeza si esa pieza ya
    esta en el repo (con su rect de la entrada «perfil» de arte_final.json) y, si no, sobre un fondo liso del tamano del rect.
    """
    from types import SimpleNamespace
    os.makedirs(salida, exist_ok=True)
    rutas = []
    ruta = os.path.join(salida, "%s_perfil_%s_capas.png" % (pid, emocion))
    capas_separadas(res).save(ruta)
    rutas.append(ruta)
    capas = {k: (res.capas_salida[k], col.rect) for k in ("base", "ojos", "boca")}
    png_cab = P._png_de(P.PERSONAJES[pid][1], "char_%s_perfil_cabeza" % pid)
    cuello = next((n for n in A.cargar_arte_final().get(pid, {}).get("perfil", {}).get("nodos", []) if n["nombre"] == "Cuello"), None)
    if png_cab and cuello and cuello.get("rect"):
        cab = SimpleNamespace(imagen=Image.open(png_cab).convert("RGBA"), rect=tuple(cuello["rect"]))
        hoja = compone_cabeza(cab, capas)
    else:
        w, h = col.rect[2] - col.rect[0], col.rect[3] - col.rect[1]
        cab = SimpleNamespace(imagen=Image.new("RGBA", (max(1, w), max(1, h)), FONDO), rect=tuple(col.rect))
        hoja = compone_cabeza(cab, capas)
    ruta = os.path.join(salida, "%s_perfil_%s_cabeza.png" % (pid, emocion))
    hoja.save(ruta)
    rutas.append(ruta)
    return rutas


def main_perfil(a, emocion, entrada):
    """
    --vista perfil: la cara del cuerpo de perfil, solo por REGISTRO (registro_perfil). Imprime el informe de la separacion, los composites y lo que
    escribiria; con --aplicar escribe los PNG en Perfil/ y anota el rect en la entrada «perfil» de arte_final.json y la caja en registro.cara_perfil.
    """
    registro, origen, errores = registro_perfil(a.id)
    if registro is None or errores:
        for e in errores or ["%s no tiene registro de perfil en arte_final.json" % a.id]:
            print("ERROR", e)
        return 2
    print("modo REGISTRADO (perfil): la expresion comparte lienzo (%dx%d) con la entrega de perfil; registro %s: x' = %.6f x + %.3f, y' = %.6f y + %.3f" % (
        registro["lienzo"][0], registro["lienzo"][1], origen, registro["escala"], registro["tx"], registro["escala"], registro["ty"]))
    cerrada = emocion == "parpadeo_cerrado"
    if registro.get("espejo"):
        print("  la entrega de perfil MIRA A LA IZQUIERDA (registro.espejo): la expresion se espeja antes de recortarla, como las partes")
    if cerrada and not registro.get("cara"):
        print("AVISO la caja comun de la cara sale de la expresion ABIERTA y aun no esta (registro.cara_perfil): corre primero «abiertos»; esta cerrada fijaria una caja distinta")
    try:
        res, col, caja, _ = prepara_registrada(entrada, registro, a, vista="perfil", limpia=bool(cerrada and getattr(a, "limpia_cerrados", True)))
    except ValueError as e:
        print("ERROR", e)
        return 2
    informe_separacion(res)
    info = res.registro_info
    if res.limpieza is not None:
        print("  limpieza de la cara cerrada (--limpia-cerrados): %d pixeles casi blancos y %d de halo quitados del rastro del ojo abierto" % res.limpieza)
    elif cerrada:
        print("  limpieza de la cara cerrada APAGADA (--no-limpia-cerrados): el rastro casi blanco del ojo abierto se queda")
    print("  caja de la cara de perfil en el lienzo de la entrega%s: %s (%s); contenido de esta imagen %s" % (" (ya espejado)" if info["espejo"] else "", caja, info["origen"], info["contenido"]))
    print("  recorte %dx%d -> textura %dx%d (x %.3f: %.2f texeles por px del lienzo del rig%s, tope --max-lado %d)" % (
        info["entrada"][0], info["entrada"][1], info["salida"][0], info["salida"][1], info["k"], info["k"] / registro["escala"],
        "" if a.densidad else " (sin reducir por densidad)", a.max_lado))
    print("  rect comun de CaraBase, Ojos y Boca de perfil (lienzo de 1024): %s, la caja mapeada con la transformacion de la entrega de perfil" % col.rect)
    rutas = composites_perfil(a.id, emocion, res, col, a.salida)
    print("\ncomposites:")
    for r in rutas:
        print("  " + r)
    hay_base = P._png_de(P.PERSONAJES[a.id][1], "char_%s_perfil_cara_base" % a.id) is not None
    if emocion == "neutra":
        hay_base = False   # la neutra fija la caja de la cara de perfil: la base (nariz y rubor) se escribe con ella
    print("\nescribiria (con --aplicar), en Assets/Game/Art/Characters/%s/Perfil/:" % P.PERSONAJES[a.id][1])
    for _, sprite in capas_de(a.id, emocion, "perfil", con_base=bool(a.base or not hay_base)):
        print("  %s.png" % sprite)
    print("  arte_final.json: rect %s en CaraBase, Ojos y Boca de «perfil» y registro.cara_perfil %s; y regenera rig_articulaciones.json y clips_personajes.json" % (col.rect, caja))
    if not a.aplicar:
        return 0
    return 1 if aplica(a.id, emocion, res, col, a, hay_base, vista="perfil") else 0


# ============================================================================ 5. aplicar


def carpeta_expresiones(pid):
    return os.path.join(P.PERSONAJES_ARTE, P.PERSONAJES[pid][1], "Expresiones")


def bloque_local_perfil(pid):
    """Las ordenes para la sesion local tras escribir la cara de PERFIL (INC-134): no hay «orden» ni «clips» que rehacer, la cara no cambia la coreografia."""
    prefab, carpeta, _ = P.PERSONAJES[pid]
    return """
==================== Sesion local (Santiago), con el Editor abierto ====================
 0. git pull   (trae esta rama con la cara de perfil, arte_final.json y rig_articulaciones.json)
 1. Copiar el generador (el Editor crea el .meta solo, nunca a mano):
      Copy-Item claudeDocs/tasks/Personajes/herramientas/BuildRigsFinal.cs.txt Assets/Editor/ClaudeBuildRigsFinal.cs
 2. Recompilar y esperar:   pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 recompile
 3. Por coplay execute_script, EN ESTE ORDEN (cada una devuelve su log):
      ClaudeBuildRigsFinal.Execute("estado")     // antes
      ClaudeBuildRigsFinal.Execute("perfil")     // Lienzo/Perfil y la cara de perfil cableada en CharacterFace (idempotente)
      ClaudeBuildRigsFinal.Execute("sprites")    // asigna los PNG de Perfil/, el rect comun y crea o actualiza char_%(pid)s_perfil_cara.asset
      ClaudeBuildRigsFinal.Execute("estado")     // despues
 4. Comprobar el set de cara de perfil: Assets/Game/Art/Characters/%(carpeta)s/Perfil/char_%(pid)s_perfil_cara.asset y que la Image de CaraBase, Ojos
    y Boca bajo Lienzo/Perfil/Tronco/Cuello/Cabeza de %(prefab)s.prefab tenga sprite y el mismo rect.
    git diff -U0 Assets/Game/Prefabs/Characters   (no debe QUITAR lineas «--- !u!»)
 5. Pruebas:   pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-edit CharacterFace
               pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-edit CharacterRig_
 6. Borrar el andamiaje:   Remove-Item Assets/Editor/ClaudeBuildRigsFinal.cs, Assets/Editor/ClaudeBuildRigsFinal.cs.meta
 7. git add Assets/Game/Art/Characters/%(carpeta)s Assets/Game/Prefabs/Characters/%(prefab)s.prefab Assets/Game/Prefabs/Characters/%(prefab)s.prefab.meta
      git add claudeDocs/tasks/Personajes/herramientas/arte_final.json claudeDocs/tasks/Personajes/herramientas/rig_articulaciones.json
========================================================================================
""" % {"pid": pid, "prefab": prefab, "carpeta": carpeta}


def bloque_local(pid, vista="frente"):
    if vista == "perfil":
        return bloque_local_perfil(pid)
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
      ClaudeBuildRigsFinal.Execute("orden")      // INC-133: antebrazos delante (idempotente: no hace nada si ya estan)
      ClaudeBuildRigsFinal.Execute("clips")      // la caja de la cara cambio y con ella los gestos junto a la cabeza
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


def actualiza_tabla(pid, rect, con_base=True, cara=None, vista="frente"):
    """
    Pone el rect comun en CaraBase, Ojos y Boca de la entrada de arte_final.json (CaraBase se crea si falta, antes de Ojos). Con «cara» (modo
    registrado) guarda tambien la caja del lienzo de la entrega que es la cara (registro.cara): comun a todas las expresiones.
    En la vista de PERFIL (INC-134) los nodos son los de la entrada «perfil» (Lienzo/Perfil/Tronco/Cuello/Cabeza/…) y la caja se guarda APARTE, en
    registro.cara_perfil: la de frente (registro.cara) no se toca.
    """
    personajes = A.cargar_arte_final()
    if pid not in personajes:
        raise KeyError("%s no tiene entrada en arte_final.json: primero preparar_arte_final.py %s <carpeta>" % (pid, pid))
    if vista == "perfil":
        perfil = personajes[pid].get("perfil")
        if perfil is None:
            raise KeyError("%s no tiene «perfil» en arte_final.json: primero preparar_perfil.py %s <carpeta>" % (pid, pid))
        nodos, padre, base = perfil["nodos"], A.PT + "/Cuello/Cabeza", "char_%s_perfil_cara_base" % pid
    else:
        nodos, padre, base = personajes[pid]["nodos"], A.T + "/Cuello/Cabeza", "char_%s_cara_base" % pid
    centro = [(rect[0] + rect[2]) // 2, (rect[1] + rect[3]) // 2]
    if not any(n["nombre"] == "CaraBase" for n in nodos):
        i = next(k for k, n in enumerate(nodos) if n["nombre"] == "Ojos")
        nodos.insert(i, {"nombre": "CaraBase", "tipo": "imagen", "padre": padre, "punto": centro, "imagen": "CaraBase",
                         "sprite": base, "rect": list(rect)})
    for n in nodos:
        if n["nombre"] in ("CaraBase", "Ojos", "Boca"):
            n["rect"], n["punto"] = list(rect), centro
    if cara is not None:
        personajes[pid].setdefault("registro", {})["cara_perfil" if vista == "perfil" else "cara"] = [int(v) for v in cara]
    A.guardar_arte_final(personajes)


def rect_vigente(pid):
    """El rect comun que ya tienen Ojos y Boca en arte_final.json si su PNG neutro esta puesto, o None."""
    if P._png_de(P.PERSONAJES[pid][1], "char_%s_ojos_neutra" % pid) is None:
        return None
    n = next((n for n in A.cargar_arte_final().get(pid, {}).get("nodos", []) if n["nombre"] == "Ojos"), None)
    return list(n["rect"]) if n else None


def aplica(pid, emocion, res, col, args, hay_base, vista="frente"):
    carpeta = carpeta_destino(pid, vista)
    os.makedirs(carpeta, exist_ok=True)
    print("\n== Aplicando al repo (%s) ==" % vista)
    for clave, sprite in capas_de(pid, emocion, vista, con_base=bool(args.base or not hay_base)):
        ruta = os.path.join(carpeta, sprite + ".png")
        existia = os.path.isfile(ruta)
        if vista == "perfil" and res.capas_salida[clave].getchannel("A").getbbox() is None:
            print("  vacia     %s: no hay nada de esta capa (%s) y no se escribe" % (os.path.relpath(ruta, P.RAIZ), "el rubor: Papa no lo lleva" if clave == "base" else clave))
            continue
        res.capas_salida[clave].save(ruta)
        print("  %s %s" % ("sustituye" if existia else "nuevo    ", os.path.relpath(ruta, P.RAIZ)))
    actualiza_tabla(pid, col.rect, cara=getattr(res, "registro_info", {}).get("caja"), vista=vista)
    destino = "«perfil» de arte_final.json" if vista == "perfil" else "arte_final.json"
    caja = "registro.cara_perfil" if vista == "perfil" else "registro.cara"
    print("  %s: rect %s en CaraBase, Ojos y Boca de %s%s" % (destino, col.rect, pid, "; %s %s" % (caja, getattr(res, "registro_info")["caja"])
                                                              if hasattr(res, "registro_info") else ""))
    py = sys.executable
    fallos = 0
    fallos += 1 if corre([py, os.path.join(AQUI, "articulaciones.py")], "articulaciones") else 0
    fallos += 1 if corre([py, os.path.join(AQUI, "coreografia.py"), "--valida"], "coreografia") else 0
    if vista != "perfil":   # pose_preview dibuja el cuerpo de frente: la cara de perfil no cambia nada de lo que ve
        fallos += 1 if corre([py, os.path.join(AQUI, "pose_preview.py"), "--solo", pid, "--salida", args.salida], "pose_preview") else 0
    print("\n%s" % ("TODO EN VERDE" if not fallos else "%d pasos fallaron: revisa antes de entregar" % fallos))
    print(bloque_local(pid, vista))
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
    malos += autoprueba_registrada()
    malos += autoprueba_perfil()
    malos += autoprueba_perfil_cara()
    print("la herramienta recupera lo conocido" if not malos else "%d casos mal" % malos)
    return 1 if malos else 0


def autoprueba_registrada():
    """
    El modo REGISTRADO con una entrega sintetica: las partes del Nino en un lienzo de 1300x1500 (preparar_arte_final.entrega_sintetica) y una
    cara sintetica puesta en ese MISMO lienzo en un sitio conocido. La cara tiene que caer, en el lienzo de 1024, donde la transformacion de
    las partes manda (<= 2 px), y una segunda expresion tiene que heredar la caja de la primera.
    """
    from types import SimpleNamespace
    malos = 0
    with tempfile.TemporaryDirectory() as tmp:
        F.entrega_sintetica(tmp)
        res_f = F.procesa(tmp, "nino")
        if res_f.errores:
            print("%-46s %s" % ("registro: la entrega sintetica no se procesa", "FALLA"))
            return 1
        reg = dict(res_f.registro)
        cab = reg["cabeza"]
        cara, _ = cara_sintetica(3)                              # 768x768, con cejas, ojos, nariz, boca y rubor
        k = 0.5
        cara = cara.resize((int(cara.width * k), int(cara.height * k)), Image.LANCZOS)
        pos = ((cab[0] + cab[2]) // 2 - cara.width // 2, cab[1] + int(0.62 * (cab[3] - cab[1])) - cara.height // 2)
        lienzo = Image.new("RGBA", tuple(reg["lienzo"]), (0, 0, 0, 0))
        lienzo.alpha_composite(cara, pos)
        args = SimpleNamespace(reubica=False, asigna=None, densidad=DENSIDAD, max_lado=512)
        res, col, caja, _ = prepara_registrada(lienzo, reg, args)
        # donde debe caer lo pintado de ojos y cejas: su caja en el recorte, al lienzo de la entrega, al del rig
        ojos = res.resumen["ojos"]["caja"]
        s_, tx, ty = reg["escala"], reg["tx"], reg["ty"]
        esperado = (s_ * (caja[0] + ojos[0]) + tx, s_ * (caja[1] + ojos[1]) + ty, s_ * (caja[0] + ojos[2]) + tx, s_ * (caja[1] + ojos[3]) + ty)
        im = res.capas_salida["ojos"]
        bb = im.getchannel("A").point(lambda v: 255 if v >= UMBRAL_ALFA else 0).getbbox()
        kx, ky = (col.rect[2] - col.rect[0]) / float(im.width), (col.rect[3] - col.rect[1]) / float(im.height)
        medido = (col.rect[0] + bb[0] * kx, col.rect[1] + bb[1] * ky, col.rect[0] + bb[2] * kx, col.rect[1] + bb[3] * ky)
        error = max(abs(a - b) for a, b in zip(esperado, medido))
        ok = error <= 2.5
        malos += 0 if ok else 1
        print("%-46s %s" % ("registro: ojos y cejas caen donde mandan las partes", "bien (error %.1f px)" % error if ok else "FALLA (error %.1f px: %s contra %s)" % (error, medido, esperado)))
        # la textura tiene la densidad de las partes (1 texel por px del lienzo del rig) y la caja cabe en la cabeza
        dens = im.width / float(col.rect[2] - col.rect[0])
        ok = abs(dens - DENSIDAD) < 0.03 and caja[0] >= cab[0] and caja[1] >= cab[1] and caja[2] <= cab[2] and caja[3] <= cab[3]
        malos += 0 if ok else 1
        print("%-46s %s" % ("registro: densidad de las partes y caja dentro de la cabeza", "bien (%.2f texeles/px)" % dens if ok else "FALLA"))
        # una segunda expresion hereda la caja (registro.cara) y su rect sale igual
        reg2 = dict(reg, cara=list(caja))
        res2, col2, caja2, _ = prepara_registrada(lienzo, reg2, args)
        ok = caja2 == caja and col2.rect == col.rect
        malos += 0 if ok else 1
        print("%-46s %s" % ("registro: la segunda expresion hereda la caja", "bien" if ok else "FALLA"))
        # y una cara que se sale de la caja guardada se rechaza en vez de desalinear la neutra
        movida = Image.new("RGBA", tuple(reg["lienzo"]), (0, 0, 0, 0))
        movida.alpha_composite(cara, (pos[0] + 150, pos[1]))
        try:
            prepara_registrada(movida, reg2, args)
            rechazada = False
        except ValueError:
            rechazada = True
        malos += 0 if rechazada else 1
        print("%-46s %s" % ("registro: lo que se sale de la caja se rechaza", "bien" if rechazada else "FALLA"))
        # una imagen de otro tamano no es una expresion registrada
        try:
            prepara_registrada(Image.new("RGBA", (512, 512), (0, 0, 0, 0)), reg, args)
            rechazada = False
        except ValueError:
            rechazada = True
        malos += 0 if rechazada else 1
        print("%-46s %s" % ("registro: otro tamano de lienzo se rechaza", "bien" if rechazada else "FALLA"))
    return malos


def autoprueba_perfil():
    """
    INC-134 y los alias: (1) «abiertos» y «cerrados» son neutra y parpadeo_cerrado y escriben los ojos de esos cuadros; (2) la vista de perfil escribe
    char_<id>_perfil_* en Perfil/ y nunca en Expresiones/ (la regla de prefabs.subcarpeta_de y la de los dos .cs.txt); (3) con una entrega de perfil
    sintetica (el lienzo del Nino) la cara de perfil se coloca por registro, su caja se guarda en registro.cara_perfil SIN tocar registro.cara de frente
    ni los nodos de frente, y los nodos CaraBase, Ojos y Boca de «perfil» llevan el rect comun; (4) sin «perfil» en arte_final.json avisa y manda a
    preparar_perfil.py; (5) la tabla de arte_final.json de verdad no se toca (la prueba trabaja sobre una copia).
    """
    import shutil
    from types import SimpleNamespace
    malos = 0
    with open(A.ARTE_FINAL_JSON, "rb") as f:
        bytes_antes = f.read()   # la tabla de verdad no se toca: se compara al final (con el arte de perfil ya aplicado ya no esta «sin perfil»)
    # --- los alias
    ok = ALIAS_EMOCION["abiertos"] == "neutra" and ALIAS_EMOCION["cerrados"] == "parpadeo_cerrado" and ALIAS_EMOCION["a"] == "boca_a"
    ok = ok and all(ALIAS_EMOCION[k] in EMOCIONES for k in ALIAS_EMOCION)
    malos += 0 if ok else 1
    print("%-46s %s" % ("alias: abiertos = neutra, cerrados = parpadeo_cerrado", "bien" if ok else "FALLA"))
    cerr = [n for _, n in capas_de("nino", ALIAS_EMOCION["cerrados"], "perfil", con_base=False)]
    abie = [n for _, n in capas_de("nino", ALIAS_EMOCION["abiertos"], "perfil")]
    ok = cerr == ["char_nino_perfil_ojos_parpadeo_cerrado"] and abie == ["char_nino_perfil_ojos_neutra", "char_nino_perfil_boca_0", "char_nino_perfil_cara_base"]
    malos += 0 if ok else 1
    print("%-46s %s" % ("alias: los PNG de perfil que escribe cada uno", "bien" if ok else "FALLA %s %s" % (cerr, abie)))
    # --- las carpetas
    perfil_ok = all(P.subcarpeta_de(n) == "Perfil" for _, n in capas_de("papa", "alegria", "perfil") + capas_de("papa", "boca_a", "perfil"))
    frente_ok = all(P.subcarpeta_de(n) == "Expresiones" for _, n in capas_de("papa", "alegria", "frente"))
    dest_ok = carpeta_destino("papa", "perfil").endswith(os.path.join("Father", "Perfil")) and carpeta_destino("papa").endswith(os.path.join("Father", "Expresiones"))
    ok = perfil_ok and frente_ok and dest_ok and prefijo_de("nina", "perfil") == "char_nina_perfil" and prefijo_de("nina") == "char_nina"
    malos += 0 if ok else 1
    print("%-46s %s" % ("perfil: lo de perfil va a Perfil/, lo de frente a Expresiones/", "bien" if ok else "FALLA"))
    # --- la vista de perfil con una entrega sintetica, sobre una COPIA de arte_final.json
    with tempfile.TemporaryDirectory() as tmp:
        F.entrega_sintetica(tmp)
        res_f = F.procesa(tmp, "nino")
        if res_f.errores:
            print("%-46s %s" % ("perfil: la entrega sintetica no se procesa", "FALLA"))
            return malos + 1
        reg = dict(res_f.registro)
        ruta_tabla = os.path.join(tmp, "arte_final_copia.json")
        shutil.copy(A.ARTE_FINAL_JSON, ruta_tabla)
        with open(ruta_tabla, encoding="utf-8") as f:
            doc = json.load(f)
        # la copia es «antes de que llegara el arte de perfil»: sin su clave «perfil» ni la caja de la cara de perfil (preparar_perfil.py las escribe en la de verdad)
        doc["personajes"]["nino"].pop("perfil", None)
        doc["personajes"]["nino"].get("registro", {}).pop("cara_perfil", None)
        with open(ruta_tabla, "w", encoding="utf-8") as f:
            json.dump(doc, f)
        antes = copy.deepcopy(doc["personajes"]["nino"])
        reg_rig = P.personaje_rig(P.cargar_rig(), "nino")["perfil"]
        original = (A.cargar_arte_final, A.guardar_arte_final)
        A.cargar_arte_final = lambda ruta=ruta_tabla: original[0](ruta)
        A.guardar_arte_final = lambda personajes, ruta=ruta_tabla: original[1](personajes, ruta)
        try:
            # sin «perfil»: se niega y manda a preparar_perfil.py
            registro, _, errores = registro_perfil("nino")
            ok = registro is None and any("preparar_perfil.py" in e for e in errores)
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: sin «perfil» en arte_final.json se niega", "bien" if ok else "FALLA"))
            try:
                actualiza_tabla("nino", [0, 0, 10, 10], vista="perfil")
                ok = False
            except KeyError:
                ok = True
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: actualiza_tabla sin «perfil» no inventa nodos", "bien" if ok else "FALLA"))
            # con «perfil» (los nodos provisionales del rig y el registro de la entrega sintetica)
            doc["personajes"]["nino"]["perfil"] = {"nodos": copy.deepcopy(reg_rig["nodos"]), "partes": [], "registro": {k: reg[k] for k in ("lienzo", "escala", "tx", "ty", "cabeza")}}
            with open(ruta_tabla, "w", encoding="utf-8") as f:
                json.dump(doc, f)
            registro, origen, errores = registro_perfil("nino")
            ok = registro is not None and not errores and "perfil.registro" in origen and "cara" not in registro
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: el registro sale de «perfil.registro»", "bien" if ok else "FALLA %s %s" % (origen, errores)))
            cara, _ = cara_sintetica(3)
            k = 0.5
            cara = cara.resize((int(cara.width * k), int(cara.height * k)), Image.LANCZOS)
            cab = registro["cabeza"]
            pos = ((cab[0] + cab[2]) // 2 - cara.width // 2, cab[1] + int(0.62 * (cab[3] - cab[1])) - cara.height // 2)
            lienzo = Image.new("RGBA", tuple(registro["lienzo"]), (0, 0, 0, 0))
            lienzo.alpha_composite(cara, pos)
            args = SimpleNamespace(reubica=False, asigna=None, densidad=DENSIDAD, max_lado=512)
            res, col, caja, _ = prepara_registrada(lienzo, registro, args)
            actualiza_tabla("nino", col.rect, cara=caja, vista="perfil")
            despues = A.cargar_arte_final()["nino"]
            nodos_p = {n["nombre"]: n for n in despues["perfil"]["nodos"]}
            ok = despues["registro"].get("cara_perfil") == list(caja)
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: la caja se guarda en registro.cara_perfil", "bien" if ok else "FALLA"))
            ok = despues["registro"].get("cara") == antes["registro"].get("cara") and despues["nodos"] == antes["nodos"] and despues["partes"] == antes["partes"]
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: la cara y los nodos de frente no se tocan", "bien" if ok else "FALLA"))
            ok = all(nodos_p[n]["rect"] == list(col.rect) for n in ("CaraBase", "Ojos", "Boca"))
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: CaraBase, Ojos y Boca de perfil llevan el rect", "bien" if ok else "FALLA"))
            registro2, _, _ = registro_perfil("nino")
            ok = registro2.get("cara") == list(caja)
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: la siguiente expresion hereda la caja", "bien" if ok else "FALLA"))
            # el registro de perfil sin «cabeza» (preparar_perfil.py puede no medirla): no recorta y no falla
            sin_cabeza = {k: v for k, v in registro.items() if k not in ("cabeza", "cara")}
            try:
                res3, col3, caja3, _ = prepara_registrada(lienzo, sin_cabeza, args)
                ok = col3.rect is not None
            except ValueError:
                ok = False
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: un registro sin cabeza se admite", "bien" if ok else "FALLA"))
            # el lienzo de otro tamano se rechaza, como de frente
            try:
                prepara_registrada(Image.new("RGBA", (512, 512), (0, 0, 0, 0)), registro, args)
                ok = False
            except ValueError:
                ok = True
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: otro tamano de lienzo se rechaza", "bien" if ok else "FALLA"))
            # los composites de perfil se pueden hacer (sin cabeza de perfil en el repo: sobre fondo liso)
            rutas = composites_perfil("nino", "neutra", res, col, os.path.join(tmp, "comp"))
            ok = len(rutas) == 2 and all(os.path.isfile(r) for r in rutas)
            malos += 0 if ok else 1
            print("%-46s %s" % ("perfil: los composites se escriben", "bien" if ok else "FALLA"))
        finally:
            A.cargar_arte_final, A.guardar_arte_final = original
    # la tabla de verdad sigue como estaba
    with open(A.ARTE_FINAL_JSON, "rb") as f:
        ok = f.read() == bytes_antes
    malos += 0 if ok else 1
    print("%-46s %s" % ("perfil: arte_final.json de verdad sin tocar", "bien" if ok else "FALLA"))
    return malos


def cara_perfil_sintetica(k=1, con_rubor=True):
    """
    Una cara de PERFIL sintetica (como las de la entrega del 09/10/2026) en un lienzo de 256k, MIRANDO A LA DERECHA: una ceja, un ojo (blanco, iris) o su
    parpado, una boca oscura en el BORDE DE DELANTE y abajo (no en el centro) y un rubor rosa blando; sin nariz (esta en la cabeza). Con «cerrada», el ojo
    es un parpado y queda un arco casi blanco del ojo abierto. Devuelve (imagen RGBA, {capa: mascara L de lo que le toca}).
    """
    n = 256 * k
    S = lambda *v: tuple(int(round(x * k)) for x in v)
    capas = {c: Image.new("RGBA", (n, n), (0, 0, 0, 0)) for c in ("ojos", "boca", "rubor")}
    d = ImageDraw.Draw(capas["ojos"])
    d.polygon(S(70, 52, 150, 44, 176, 56, 120, 62), fill=(110, 55, 20, 255), outline=(0, 0, 0, 255))   # ceja
    d.ellipse(S(96, 76, 160, 128), fill=(255, 255, 255, 255), outline=(0, 0, 0, 255), width=2 * k)      # ojo
    d.ellipse(S(122, 82, 156, 124), fill=(224, 128, 32, 255), outline=(60, 30, 10, 255), width=k)
    d.ellipse(S(134, 92, 150, 114), fill=(0, 0, 0, 255))
    d = ImageDraw.Draw(capas["boca"])
    d.line(S(176, 206, 204, 210, 214, 200), fill=(0, 0, 0, 255), width=4 * k)                            # boca: borde de delante, abajo
    d.line(S(204, 196, 212, 190), fill=(0, 0, 0, 255), width=3 * k)                                      # y su pliegue
    m = Image.new("L", (n, n), 0)
    ImageDraw.Draw(m).ellipse(S(100, 150, 170, 196), fill=200)
    m = m.filter(ImageFilter.GaussianBlur(4 * k))
    rosa = Image.new("RGBA", (n, n), (250, 120, 140, 255))
    rosa.putalpha(m)
    if con_rubor:
        capas["rubor"].alpha_composite(rosa)
    im = Image.new("RGBA", (n, n), (0, 0, 0, 0))
    for c in ("rubor", "ojos", "boca"):
        im.alpha_composite(capas[c])
    verdad = {c: _binaria(capas[c].getchannel("A"), lambda v: v >= UMBRAL_ALFA) for c in capas}
    return im, verdad


def cara_perfil_cerrada_sintetica(k=1):
    """La misma cara con el ojo CERRADO: un parpado negro y, debajo, el rastro casi blanco y semitransparente del ojo abierto (un arco). Devuelve (imagen, arco: mascara de los pixeles del rastro)."""
    im, _ = cara_perfil_sintetica(k)
    S = lambda *v: tuple(int(round(x * k)) for x in v)
    # quitar el ojo abierto y poner el parpado
    ojo = Image.new("RGBA", im.size, (0, 0, 0, 0))
    ImageDraw.Draw(ojo).ellipse(S(94, 74, 162, 130), fill=(255, 255, 255, 255))
    im.paste(Image.new("RGBA", im.size, (0, 0, 0, 0)), (0, 0), ojo.getchannel("A"))
    d = ImageDraw.Draw(im)
    d.line(S(100, 100, 130, 104, 158, 98), fill=(0, 0, 0, 255), width=5 * k)
    arco = Image.new("RGBA", im.size, (0, 0, 0, 0))
    da = ImageDraw.Draw(arco)
    da.arc(S(98, 84, 160, 126), 20, 160, fill=(190, 170, 175, 90), width=4 * k)    # el halo: claro y semitransparente, a un pixel del blanco
    da.arc(S(98, 84, 160, 126), 20, 160, fill=(255, 255, 255, 200), width=2 * k)   # el rastro casi blanco
    im.alpha_composite(arco)
    return im, _binaria(arco.getchannel("A"), lambda v: v > 0)


def autoprueba_perfil_cara():
    """
    La cara de PERFIL (INC-134, Santiago 09/10/2026): (1) separa_perfil encuentra la boca en el borde de delante y abajo (separa(), que la busca en el centro, no),
    el rubor es solo lo rosado y los ojos todo lo demas, a dos tamanos de lienzo; (2) la cara que no tiene rubor (Papa) no tiene capa base; (3) limpia_cerrados
    quita el arco casi blanco y su halo sin tocar el trazo negro y dice cuantos quito; (4) con «espejo» en el registro la expresion se espeja antes de recortar y
    cae donde manda el registro espejado; (5) --vista perfil rechaza lo que no sea un lienzo registrado; (6) una imagen de perfil sin --vista perfil se rechaza.
    """
    import contextlib
    import io
    from types import SimpleNamespace
    malos = 0

    def caso(nombre, ok, detalle=""):
        nonlocal malos
        malos += 0 if ok else 1
        print("%-46s %s" % (nombre, "bien" if ok else "FALLA " + detalle))

    # --- (1) y (2) la separacion
    for k in (1, 3):
        im, verdad = cara_perfil_sintetica(k)
        res = separa_perfil(im)
        for capa, clave in (("ojos", "ojos"), ("boca", "boca"), ("rubor", "base")):
            a = _binaria(res.capas[clave].getchannel("A"), lambda v: v >= UMBRAL_ALFA)
            inter = _y(a, verdad[capa]).histogram()[255]
            union = ImageChops.lighter(a, verdad[capa]).histogram()[255]
            iou = inter / float(union)
            caso("separa_perfil lienzo %dx%d: %s (IoU %.3f)" % (256 * k, 256 * k, capa, iou), iou > 0.93, "IoU %.3f" % iou)
    r2 = separa_perfil(cara_perfil_sintetica(1, con_rubor=False)[0])
    caso("una cara sin rubor (Papa): la capa base queda vacia", r2.capas["base"].getchannel("A").getbbox() is None and any("no hay rubor" in a for a in r2.avisos))
    # --- (3) la limpieza de los ojos cerrados
    cerr, arco = cara_perfil_cerrada_sintetica(1)
    n_arco = arco.histogram()[255]
    limpia, n_blanco, n_halo = limpia_cerrados(cerr)
    negros = lambda im: _y(_binaria(im.getchannel("A"), lambda v: v >= 200), _binaria(ImageChops.darker(ImageChops.darker(*im.split()[:2]), im.split()[2]), lambda v: v < 60)).histogram()[255]
    resto = _y(arco, _binaria(limpia.getchannel("A"), lambda v: v > 12)).histogram()[255]
    caso("limpia_cerrados quita el arco (%d px casi blancos y %d de halo de %d)" % (n_blanco, n_halo, n_arco), n_blanco > 0 and n_halo > 0 and resto <= 0.05 * n_arco, "quedan %d de %d" % (resto, n_arco))
    caso("limpia_cerrados no toca el trazo negro del parpado ni de la ceja", negros(limpia) >= 0.99 * negros(cerr), "%d contra %d" % (negros(limpia), negros(cerr)))
    abierta, _ = cara_perfil_sintetica(1)
    iguales = ImageChops.difference(limpia_cerrados(abierta)[0].getchannel("A"), abierta.getchannel("A")).getbbox()
    caso("(una cara abierta pasada por limpia_cerrados pierde el blanco del ojo: por eso solo se aplica a la cerrada)", iguales is not None)
    # --- (4) el espejo del registro
    with tempfile.TemporaryDirectory() as tmp:
        F.entrega_sintetica(tmp)
        res_f = F.procesa(tmp, "nino")
        reg = dict(res_f.registro)
    cara, _ = cara_perfil_sintetica(3)
    cara = cara.resize((cara.width // 2, cara.height // 2), Image.LANCZOS)
    W, H = reg["lienzo"]
    pos = (300, 500)
    lienzo = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    lienzo.alpha_composite(cara, pos)   # la cara en el lienzo de la entrega, SIN espejar (mirando a la derecha); el registro dice que se espeja
    reg_e = dict(reg, espejo=True, cabeza=[0, 0, W, H])
    reg_e.pop("cara", None)
    args = SimpleNamespace(reubica=False, asigna=None, densidad=DENSIDAD, max_lado=512)
    res, col, caja, _ = prepara_registrada(lienzo, reg_e, args, vista="perfil")
    ojos = res.resumen["ojos"]["caja"]
    bo = separa_perfil(cara).resumen["ojos"]["caja"]             # la caja de los ojos en la cara sin espejar
    ex0, ex1 = W - (pos[0] + bo[2]), W - (pos[0] + bo[0])        # espejada, en el lienzo de la entrega
    mx0, mx1 = caja[0] + ojos[0], caja[0] + ojos[2]
    caso("espejo: los ojos caen donde manda el registro espejado (<= 2 px)", abs(mx0 - ex0) <= 2 and abs(mx1 - ex1) <= 2, "%s..%s contra %s..%s" % (mx0, mx1, ex0, ex1))
    res_s, _, caja_s, _ = prepara_registrada(lienzo, dict(reg_e, espejo=False), args, vista="perfil")
    sx0 = caja_s[0] + res_s.resumen["ojos"]["caja"][0]
    caso("sin «espejo» la misma imagen cae del otro lado, sin espejar (<= 2 px)", abs(sx0 - (pos[0] + bo[0])) <= 2 and abs(sx0 - mx0) > 50, "%s contra %s" % (sx0, pos[0] + bo[0]))
    caso("el rect comun sale de la caja mapeada con la transformacion (la de las partes)", col.rect == F.caja_a_lienzo(reg_e, caja))
    # --- (5) y (6) los resguardos de main()
    def sale(argv):
        err = io.StringIO()
        try:
            with contextlib.redirect_stderr(err), contextlib.redirect_stdout(io.StringIO()):
                main(argv)
        except SystemExit as e:
            return e.code, err.getvalue()
        return 0, err.getvalue()

    with tempfile.TemporaryDirectory() as tmp:
        for nombre in ("exoresion_neutra_perfil_papa.png", "Expresi\u00f3n_PERFIL_ni\u00f1o.png"):
            ruta = os.path.join(tmp, nombre)
            Image.new("RGBA", (1300, 1500), (0, 0, 0, 0)).save(ruta)
            codigo, msg = sale(["papa", "abiertos", ruta])
            caso("sin --vista perfil se rechaza «%s»" % nombre.encode("ascii", "replace").decode(), codigo == 2 and "--vista perfil" in msg, "%s %s" % (codigo, msg[-120:]))
        ruta = os.path.join(tmp, "x_perfil_papa.png")
        Image.new("RGBA", (1300, 1500), (0, 0, 0, 0)).save(ruta)
        codigo, msg = sale(["papa", "abiertos", ruta, "--vista", "perfil"])
        caso("con --vista perfil el resguardo no salta (y una imagen vacia se rechaza por otra razon)", "imagen de perfil" not in msg, msg[-120:])
        ruta = os.path.join(tmp, "expresion_neutra_papa.png")
        Image.new("RGBA", (1300, 1500), (0, 0, 0, 0)).save(ruta)
        codigo, msg = sale(["papa", "abiertos", ruta])
        caso("sin «perfil» en el nombre el resguardo no salta (la de frente sigue su camino)", "imagen de perfil" not in msg, msg[-120:])
    return malos


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
    ap.add_argument("id", nargs="?", help="personaje: nino, papa, mama o nina")
    ap.add_argument("emocion", nargs="?", help="|".join(EMOCIONES) + " (alias: abiertos = neutra, cerrados = parpadeo_cerrado)")
    ap.add_argument("imagen", nargs="?", help="PNG de la cara completa, fondo transparente")
    ap.add_argument("--registrada", nargs="?", const=True, default=False, metavar="CARPETA_FRENTE",
                    help="la expresion esta en el MISMO lienzo que las partes (registrada): su sitio sale de la transformacion que uso preparar_arte_final.py "
                         "con las partes, no de una heuristica. Con la carpeta Frente de la entrega la calcula; sin ella usa el registro de arte_final.json. "
                         "Se activa sola si arte_final.json trae el registro del personaje y la imagen mide lo mismo que sus partes (--sin-registro la apaga)")
    ap.add_argument("--sin-registro", action="store_true", help="usa la heuristica de escala y sitio aunque la imagen este registrada")
    ap.add_argument("--densidad", type=float, default=DENSIDAD,
                    help="(modo registrado) texeles de la textura por pixel del lienzo del rig: por defecto 1, la de las partes del cuerpo; 0 = sin reducir por densidad")
    ap.add_argument("--escala", type=float, help="fraccion del ancho del ovalo que ocupa la cara (0.4 a 0.8); por defecto la mayor de 0.50/0.60/0.70 que cabe")
    ap.add_argument("--aplicar", action="store_true")
    ap.add_argument("--base", action="store_true", help="reescribe tambien char_<x>_cara_base")
    ap.add_argument("--reubica", action="store_true", help="recalcula el rect comun (heuristica) o la caja comun de la cara (registrada) aunque ya este puesta")
    ap.add_argument("--asigna", action="append", metavar="CAPA:X0,Y0,X1,Y1", help="corrige a mano: lo que cae en esa caja (px de la imagen) pasa a esa capa")
    ap.add_argument("--vista", choices=VISTAS, default="frente",
                    help="frente (por defecto) o perfil (INC-134): la cara del cuerpo de perfil, que se escribe en Perfil/ como char_<id>_perfil_* y se coloca por registro; "
                         "la caja de su cara se guarda aparte, en registro.cara_perfil de arte_final.json")
    ap.add_argument("--limpia-cerrados", action=argparse.BooleanOptionalAction, default=True,
                    help="(vista perfil, emocion parpadeo_cerrado) quitar el rastro casi blanco del ojo abierto y su halo; por defecto si (Santiago, 09/10/2026)")
    ap.add_argument("--max-lado", type=int, default=512)
    ap.add_argument("--salida", default=os.path.join(tempfile.gettempdir(), "algoritmia_expresion"))
    ap.add_argument("--autoprueba", action="store_true")
    a = ap.parse_args(argv)
    if a.autoprueba:
        return autoprueba()
    if not (a.id and a.emocion and a.imagen):
        ap.error("hace falta <id> <emocion> <imagen> (o --autoprueba)")
    emocion = ALIAS_EMOCION.get(a.emocion, a.emocion)   # abiertos = neutra, cerrados = parpadeo_cerrado; a, e y u = las bocas del habla
    if emocion not in EMOCIONES:
        ap.error("emocion %r desconocida: %s" % (a.emocion, ", ".join(EMOCIONES)))
    if a.id not in P.FAMILIA:
        ap.error("id %r desconocido: %s" % (a.id, ", ".join(P.FAMILIA)))
    if not os.path.isfile(a.imagen):
        ap.error("no existe %s" % a.imagen)
    if a.vista != "perfil" and "perfil" in pliega(os.path.basename(a.imagen)):
        # una expresion de perfil comparte lienzo (1300x1500) con las de frente: en modo registrado se pondria SOBRE la cara de frente sin que nada lo notara
        ap.error("imagen de perfil: añade --vista perfil")

    rig = P.cargar_rig()
    pj_rig = P.personaje_rig(rig, a.id)
    carpeta = P.PERSONAJES[a.id][1]
    entrada = Image.open(a.imagen).convert("RGBA")
    print("== %s / %s%s: %s (%dx%d)" % (a.id, emocion, " (%s)" % a.emocion if emocion != a.emocion else "", a.imagen, entrada.width, entrada.height)
          + ("  [vista de PERFIL]" if a.vista == "perfil" else ""))
    if a.vista == "perfil":
        return main_perfil(a, emocion, entrada)

    png_cab = P._png_de(carpeta, "char_%s_parte_cabeza" % a.id)
    cuello = next((n for n in pj_rig["nodos"] if n["nombre"] == "Cuello"), None)
    if png_cab is None or cuello is None:
        print("\nNo hay char_%s_parte_cabeza.png (la cabeza propia llega con el arte final: preparar_arte_final.py %s <carpeta>): no puedo colocar la cara." % (a.id, a.id))
        return 2

    # --- el modo: registrada (mismo lienzo que las partes) o por heuristica
    registro, origen_reg, errores_reg = (None, "", [])
    if not a.sin_registro:
        registro, origen_reg, errores_reg = registro_de(a.id, a.registrada if isinstance(a.registrada, str) else None)
    usa_registro = registro is not None and (bool(a.registrada) or list(entrada.size) == list(registro["lienzo"]))
    if a.registrada and registro is None:
        print("\nERROR --registrada: %s no tiene registro (ni carpeta Frente, ni «registro» en arte_final.json): preparar_arte_final.py %s <carpeta> --aplicar lo escribe" % (a.id, a.id))
        return 2
    if usa_registro and errores_reg:
        for e in errores_reg:
            print("ERROR", e)
        return 2
    if usa_registro:
        print("modo REGISTRADO: la expresion comparte lienzo (%dx%d) con las partes; registro %s: x' = %.6f x + %.3f, y' = %.6f y + %.3f" % (
            registro["lienzo"][0], registro["lienzo"][1], origen_reg, registro["escala"], registro["tx"], registro["escala"], registro["ty"]))
        try:
            res, col, caja, _ = prepara_registrada(entrada, registro, a)
        except ValueError as e:
            print("ERROR", e)
            return 2
        informe_separacion(res)
        info = res.registro_info
        print("  caja de la cara en el lienzo de la entrega: %s (%s); contenido de esta imagen %s" % (caja, info["origen"], info["contenido"]))
        print("  recorte %dx%d -> textura %dx%d (x %.3f: %.2f texeles por px del lienzo del rig%s, tope --max-lado %d)" % (
            info["entrada"][0], info["entrada"][1], info["salida"][0], info["salida"][1], info["k"], info["k"] / registro["escala"],
            "" if a.densidad else " (sin reducir por densidad)", a.max_lado))
        print("  rect comun de CaraBase, Ojos y Boca (lienzo de 1024): %s, la caja mapeada con la transformacion de las partes" % col.rect)
        cab = mide_cabeza(png_cab, cuello["rect"])
        print("\n== La cabeza (%s)" % os.path.relpath(png_cab, P.RAIZ))
        print("  piel RGB %s | ovalo x %.0f..%.0f, y %.0f..%.0f | eje x %.1f | flequillo hasta y %.0f | barbilla y %.0f" % (
            cab.piel, cab.ovalo[0], cab.ovalo[2], cab.ovalo[1], cab.ovalo[3], cab.eje, cab.flequillo, cab.barbilla))
        cols, rec = [col], 0
        # informacion (no decide nada): donde cae la cara respecto del ovalo de piel
        ojos = res.resumen["ojos"]["caja"]
        if ojos is not None:
            x0 = col.rect[0] + ojos[0] * col.s
            x1 = col.rect[0] + ojos[2] * col.s
            print("  ojos y cejas: x %.0f..%.0f (centro %.0f; eje de la cabeza %.0f), ancho %.0f = %.0f %% del ancho del ovalo" % (
                x0, x1, (x0 + x1) / 2.0, cab.eje, x1 - x0, 100.0 * (x1 - x0) / (cab.ovalo[2] - cab.ovalo[0])))
    else:
        if a.registrada is False and registro is not None and list(entrada.size) != list(registro["lienzo"]):
            print("AVISO la imagen mide %dx%d y las partes %dx%d: no estan registradas, uso la heuristica de escala y sitio" % (
                entrada.width, entrada.height, registro["lienzo"][0], registro["lienzo"][1]))
        res = separa(entrada, parsea_asigna(a.asigna))
        informe_separacion(res)
        # lo que se escribe: el lienzo entero (reducido a --max-lado), igual para las tres capas
        res.capas_salida = {k: reduce(v, a.max_lado) for k, v in res.capas.items()}
        k = res.capas_salida["ojos"].width / float(entrada.width)
        print("  lienzo de salida: %dx%d%s" % (res.capas_salida["ojos"].width, res.capas_salida["ojos"].height,
                                              "" if k >= 1.0 else " (reducido x%.3f: --max-lado %d)" % (k, a.max_lado)))
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
    if usa_registro and emocion == "neutra":
        hay_base = False   # la neutra fija la caja de la cara: la base (nariz y rubor) se escribe con ella
    print("\nescribiria (con --aplicar), en Assets/Game/Art/Characters/%s/Expresiones/:" % carpeta)
    for n in (ojos_n, boca_n, "cara_base" if (a.base or not hay_base) else None):
        if n:
            print("  char_%s_%s.png" % (a.id, n))
    print("  arte_final.json: rect %s en CaraBase, Ojos y Boca%s; y regenera rig_articulaciones.json y clips_personajes.json" % (
        cols[rec].rect, " y registro.cara %s" % res.registro_info["caja"] if usa_registro else ""))
    if not a.aplicar:
        return 0
    return 1 if aplica(a.id, emocion, res, cols[rec], a, hay_base) else 0


if __name__ == "__main__":
    sys.exit(main())
