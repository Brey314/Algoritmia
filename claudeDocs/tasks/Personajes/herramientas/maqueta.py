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
# arbol tal cual. INC-133 (06/10/2026): el «orden_tronco» que aplica (prefabs.arbol_vigente / aplicar_orden_tronco) lista los antebrazos de la
# familia al final de Tronco: el antebrazo SIGUE bajo su codo en el arbol (las rutas y la geometria no cambian) y solo se dibuja delante del
# torso y de la cara, como hace el motor con el ancla bajo el codo; las piezas de la maqueta no se enteran.
#
# ALGORITM (08/10/2026, D13): al final de este archivo, piezas_guia parte el sprite entero de cada forma en las nueve piezas del JSON. Fue el arte
# provisional de los prefabs (pose_preview.py --exporta-maqueta las escribia como char_algoritm_<forma>_parte_*.png). RETIRADO el 09/10/2026 (INC-136): el arte
# final de Algoritm son siete piezas con la pierna entera (preparar_algoritm.py) y pose_preview.py ya no corta nada; piezas_guia queda como historia del corte.

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


# ---------------------------------------------------------------------------------------------- Algoritm
#
# El sprite de Algoritm (char_algoritm_n1_fuego_reposo.png y sus dos recoloreados, 768 px) es UNO solo: la llama, el vientre de colores, los
# palos de los brazos y las piernas, las manos y los pies. piezas_guia lo parte en las NUEVE piezas que pide rig_articulaciones.json (el
# torso y, a cada lado, brazo = humero, antebrazo, pierna = muslo y antepierna) para que los brazos y las piernas se muevan por separado.
# pose_preview.py las dibujaba y las probaba y su --exporta-maqueta las escribia como char_algoritm_<forma>_parte_*.png (RETIRADO, INC-136: el arte final de
# Algoritm lo sustituyo con siete piezas de pierna entera; ver preparar_algoritm.py). Esta funcion ya no la llama nadie.
#
# COMO SE CORTA. Cortar por rects no sirve: los brazos van en diagonal y salen del borde curvo del vientre, y un rect se llevaba un trozo del
# contorno del vientre en cada hombro y cada cadera (al girar la extremidad el trozo se iba con ella y el vientre quedaba mordido). Aqui:
#   1. el CUERPO es lo que sobrevive a una apertura (erosion y dilatacion con un disco de R_ABRE px: los palos no caben) en la componente que
#      contiene SEMILLA_CUERPO, con BORDE_CUERPO px mas para su borde suave: la llama y el vientre, sin palos ni manos;
#   2. lo LIBRE es lo opaco que no es cuerpo; dentro de la zona de cada extremidad (ZONA_BRAZO, ZONA_PIERNA) es de esa extremidad, y el torso se
#      queda con todo lo demas;
#   3. cada extremidad se parte en su articulacion (el codo y la rodilla del JSON, sobre el palo) con un plano perpendicular al eje: las dos mitades
#      se reparten los pixeles, sin repetir ninguno, para que en reposo no quede un hilo transparente en la costura;
#   4. la pieza de arriba lleva una ROTULA, un disco del radio del palo en la articulacion con los pixeles del dibujo (en el hombro y en la cadera solo
#      los oscuros: el contorno del vientre y el palo): tapa la cuna que se abre en el lado de fuera al doblar y, en reposo, queda debajo de lo que ya esta.
# Las piezas se cortan a la RESOLUCION del sprite (sin reescalar) y cada una ocupa el rect del JSON en el lienzo de 1024: por eso los rects de
# Algoritm son multiplos de 4 (a 768 px caen en pixeles enteros).

R_ABRE = 16                  # px del sprite: radio de la apertura que deja solo el cuerpo (los palos miden 20 px a 768; el vientre, mucho mas). Cuanto mayor, menos
                             # se mete el cuerpo en el palo donde se unen (la protuberancia es de unos 5 px del lienzo con 16 y de 9 con 11)
BORDE_CUERPO = 2             # px que se le suman al cuerpo para que se quede su borde suave
PALO = 27.0                  # grosor de los palos en el lienzo de 1024, medido sobre el alfa (de 26 a 27,5): la rotula mide la mitad
SEMILLA_CUERPO = (512, 700)  # un punto dentro del vientre (lienzo de 1024)
FRANJA_CUERPO = (500, 860)   # filas del lienzo de 1024 donde se calcula el cuerpo: de los hombros a las caderas, con margen
OSCURO = (115, 50, 30)       # R, G y B por debajo de esto es el contorno del vientre o un palo (la rotula del hombro y de la cadera copia solo eso)
RUIDO = 8                    # el alfa por debajo de esto (3 %) es la pelusa del borde del sprite original: no es de ninguna pieza (ni se ve)
# Donde caen las extremidades en el lienzo de 1024 (x0, y0, x1, y1): lo libre que cae aqui es suyo.
ZONA_BRAZO = {"Izq": (0, 590, 340, 960), "Der": (690, 590, 1024, 960)}
ZONA_PIERNA = {"Izq": (380, 780, 505, 1024), "Der": (505, 780, 650, 1024)}
NOMBRES_GUIA = ("Torso", "BrazoIzq", "BrazoDer", "AntebrazoIzq", "AntebrazoDer", "PiernaIzq", "PiernaDer", "AntepiernaIzq", "AntepiernaDer")
_CACHE_GUIA = {}


def _disco(r):
    return [(dx, dy) for dy in range(-r, r + 1) for dx in range(-r, r + 1) if dx * dx + dy * dy <= r * r]


def _morfo(m, r, op):
    """Erosion (op = ImageChops.darker) o dilatacion (ImageChops.lighter) de una mascara L de 0/255 con un disco de radio r; lo de fuera de la imagen es vacio."""
    w, h = m.size
    pad = Image.new("L", (w + 2 * r, h + 2 * r), 0)
    pad.paste(m, (r, r))
    out = pad
    for dx, dy in _disco(r):
        if dx or dy:
            out = op(out, ImageChops.offset(pad, dx, dy))
    return out.crop((r, r, r + w, r + h))


def _cuerpo(alfa, k):
    """
    La llama y el vientre, sin palos ni manos (mascara L de 0/255): la componente de la apertura que contiene la semilla, con su borde suave. Solo se
    calcula en la franja del lienzo donde estan los hombros y las caderas (FRANJA_CUERPO): arriba esta la llama sola y abajo solo las piernas.
    """
    y0, y1 = int(FRANJA_CUERPO[0] * k), int(FRANJA_CUERPO[1] * k)
    solido = alfa.point(lambda v: 255 if v > 127 else 0).crop((0, y0, alfa.size[0], y1))
    abierto = _morfo(_morfo(solido, R_ABRE, ImageChops.darker), R_ABRE, ImageChops.lighter)
    semilla = (int(SEMILLA_CUERPO[0] * k), int(SEMILLA_CUERPO[1] * k) - y0)
    if abierto.getpixel(semilla) != 255:
        raise ValueError("la semilla %s no esta dentro del cuerpo de Algoritm" % (SEMILLA_CUERPO,))
    ImageDraw.floodfill(abierto, semilla, 128)
    cuerpo = _morfo(abierto.point(lambda v: 255 if v == 128 else 0), BORDE_CUERPO, ImageChops.lighter)
    completo = Image.new("L", alfa.size, 0)
    completo.paste(cuerpo, (0, y0))
    return completo


def _semiplano(tam, p0, u):
    """Mascara L de 0/255: 255 donde (p - p0) . u >= 0, sin suavizar (el otro lado es su inversa: los pixeles se reparten)."""
    w, h = tam
    big = 4.0 * (w + h)
    n = (-u[1], u[0])
    m = Image.new("L", tam, 0)
    ImageDraw.Draw(m).polygon([(p0[0] + n[0] * big, p0[1] + n[1] * big), (p0[0] - n[0] * big, p0[1] - n[1] * big),
                               (p0[0] - n[0] * big + u[0] * big, p0[1] - n[1] * big + u[1] * big),
                               (p0[0] + n[0] * big + u[0] * big, p0[1] + n[1] * big + u[1] * big)], fill=255)
    return m


def _mascaras_guia(im, p):
    """{nombre: mascara L de 0/255, del tamano de la imagen} de las nueve piezas, sin recortar a su rect. Ver el bloque de arriba."""
    k = im.size[0] / 1024.0
    alfa = im.getchannel("A")
    pos = alfa.point(lambda v: 255 if v >= RUIDO else 0)
    nodos = {n["nombre"]: n for n in p["nodos"]}
    libre = ImageChops.subtract(pos, _cuerpo(alfa, k))
    r, g, b, _ = im.split()
    oscuro = ImageChops.multiply(ImageChops.multiply(r.point(lambda v: 255 if v < OSCURO[0] else 0), g.point(lambda v: 255 if v < OSCURO[1] else 0)),
                                 b.point(lambda v: 255 if v < OSCURO[2] else 0))
    rotula = (PALO / 2.0 + 0.5) * k

    def zona(rect):
        m = Image.new("L", im.size, 0)
        ImageDraw.Draw(m).rectangle([rect[0] * k, rect[1] * k, rect[2] * k - 1, rect[3] * k - 1], fill=255)
        return m

    def disco(c):
        m = Image.new("L", im.size, 0)
        ImageDraw.Draw(m).ellipse([c[0] - rotula, c[1] - rotula, c[0] + rotula, c[1] + rotula], fill=255)
        return ImageChops.multiply(m, pos)

    def arranque(raiz, u):
        """Los pixeles oscuros del primer trecho del palo (un PALO desde el hombro o la cadera): la protuberancia del cuerpo se mete unos px en el palo y ahi se lo habria comido."""
        h, largo = (PALO / 2.0 + 2.0) * k, PALO * k
        m = Image.new("L", im.size, 0)
        ImageDraw.Draw(m).polygon([(raiz[0] - u[1] * h, raiz[1] + u[0] * h), (raiz[0] + u[1] * h, raiz[1] - u[0] * h),
                                   (raiz[0] + u[1] * h + u[0] * largo, raiz[1] - u[0] * h + u[1] * largo),
                                   (raiz[0] - u[1] * h + u[0] * largo, raiz[1] + u[0] * h + u[1] * largo)], fill=255)
        return ImageChops.multiply(ImageChops.multiply(m, pos), oscuro)

    mascaras, zonas = {}, Image.new("L", im.size, 0)
    for sup, inf, art, zonas_de in (("Brazo", "Antebrazo", "Codo", ZONA_BRAZO), ("Pierna", "Antepierna", "Rodilla", ZONA_PIERNA)):
        for lado in ("Izq", "Der"):
            raiz = tuple(v * k for v in nodos[sup + lado]["punto"])     # el hombro o la cadera
            junta = tuple(v * k for v in nodos[art + lado]["punto"])    # el codo o la rodilla
            propias = ImageChops.multiply(libre, zona(zonas_de[lado]))
            zonas = ImageChops.lighter(zonas, propias)
            eje = _unit(raiz, junta)
            mas_alla = _semiplano(im.size, junta, eje)
            mascaras[sup + lado] = ImageChops.lighter(ImageChops.multiply(propias, ImageChops.invert(mas_alla)),
                                                      ImageChops.lighter(disco(junta), ImageChops.lighter(arranque(raiz, eje), ImageChops.multiply(disco(raiz), oscuro))))
            mascaras[inf + lado] = ImageChops.multiply(propias, mas_alla)
    mascaras["Torso"] = ImageChops.subtract(pos, zonas)
    return mascaras


def piezas_guia(png, p):
    """
    {nombre: Pieza} de las nueve piezas de Algoritm (NOMBRES_GUIA), recortadas del sprite «png» a su resolucion y puestas en los rects que
    el JSON da a «p» (la entrada de la forma en rig_articulaciones.json; el rect de un antebrazo o una antepierna es el de su articulacion).
    Lanza ValueError si un rect no cae en pixeles enteros del sprite o si una pieza se sale de el: el JSON y el corte tienen que cuadrar.
    """
    clave = (os.path.abspath(png), repr(p["nodos"]))
    if clave in _CACHE_GUIA:
        return _CACHE_GUIA[clave]
    im = Image.open(png).convert("RGBA")
    k = im.size[0] / 1024.0
    nodos = {n["nombre"]: n for n in p["nodos"]}
    rects = {"Torso": nodos["Torso"]["rect"]}
    for lado in ("Izq", "Der"):
        rects.update({"Brazo" + lado: nodos["Brazo" + lado]["rect"], "Antebrazo" + lado: nodos["Codo" + lado]["rect"],
                      "Pierna" + lado: nodos["Pierna" + lado]["rect"], "Antepierna" + lado: nodos["Rodilla" + lado]["rect"]})
    alfa = im.getchannel("A")
    piezas = {}
    for nombre, mascara in _mascaras_guia(im, p).items():
        caja = tuple(v * k for v in rects[nombre])
        if any(abs(v - round(v)) > 1e-6 for v in caja):
            raise ValueError("%s: el rect %s no cae en pixeles enteros del sprite (a %d px, el lienzo de 1024 vale %.4f px por unidad): usa multiplos de 4" % (
                nombre, list(rects[nombre]), im.size[0], k))
        caja = tuple(int(round(v)) for v in caja)
        pieza = im.copy()
        pieza.putalpha(ImageChops.multiply(alfa, mascara))
        dentro = pieza.getchannel("A").point(lambda v: 255 if v else 0).getbbox()
        if dentro is None or dentro[0] < caja[0] or dentro[1] < caja[1] or dentro[2] > caja[2] or dentro[3] > caja[3]:
            raise ValueError("%s: la pieza ocupa %s (px del sprite) y su rect %s es %s: no cabe" % (
                nombre, dentro, list(rects[nombre]), caja))
        piezas[nombre] = Pieza(rects[nombre], pieza.crop(caja))
    _CACHE_GUIA[clave] = piezas
    return piezas
