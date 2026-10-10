#!/usr/bin/env python3
# preparar_arte_final.py: de las piezas que entrega Santiago a las medidas del rig, con un comando.
#
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_arte_final.py <id> <carpeta_entrega>             # informe
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_arte_final.py <id> <carpeta_entrega> --aplicar   # escribe
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_arte_final.py --autoprueba                       # se prueba solo
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_arte_final.py --fabrica-entrega <carpeta>        # una entrega de prueba
#         <id> = papa | mama | nina | nino        --salida <carpeta> para el composite del informe (por defecto, el tmp)
#         --cadera borde|capsula                  donde gira el muslo (ver MEDIDAS)
#
# QUE HACE (todo en el lienzo de 1024, origen arriba a la izquierda, y hacia abajo, la figura de y = 77 a 947):
#   1. Reconoce las piezas POR NOMBRE, con tolerancia (torso, cabeza, brazo, antebrazo o mano, muslo o pierna,
#      antepierna o pie; derecho/izquierdo/der/izq/right/left; con o sin tilde y eñe; cualquier sufijo de
#      personaje) y descarta los duplicados byte a byte. El LADO lo decide la POSICION en el lienzo (la
#      convencion del rig: BrazoIzq es el de la izquierda de la PANTALLA, como en el Nino); si el nombre
#      lo contradice avisa.
#   2. Exige un lienzo comun a todas las capas, limpia el alfa (< 12 a cero, solo la componente conexa de 4
#      vecinos mas grande de cada pieza) y normaliza: escala unica 870 / (pie mas bajo - coronilla), torso
#      centrado en x = 512, recorte al bbox, LANCZOS, y el rect de la tabla es ese bbox.
#   3. MIDE sobre el alfa, como pose_preview.py --mide (MEDIDAS, abajo), y arma nodos y partes como los del Nino.
#   4. Sin --aplicar: solo informe y un composite PNG (la figura en reposo con sus articulaciones y tres poses
#      de prueba) y la lista de lo que escribiria. Con --aplicar: escribe los PNG con los nombres de
#      Personajes-Resultados.md C.4 en Assets/Game/Art/Characters/<Carpeta>/Frontal/ (las partes de frente; los que ya
#      existian se sustituyen DONDE ESTAN, sea Frontal/ o la raiz de la carpeta antes de que el Editor las mueva: mismo
#      .meta, mismo GUID), guarda la entrada del personaje en arte_final.json, regenera
#      rig_articulaciones.json (articulaciones.py) y clips_personajes.json (coreografia.py), corre pose_preview.py
#      y imprime el bloque de ordenes para la sesion local.
#   REGISTRO (06/10/2026). Las capas de Santiago estan registradas entre si (lienzo comun) y la cara (expresiones/) llega en ese mismo lienzo:
#   arte_final.json guarda por personaje la transformacion lienzo-de-entrega -> lienzo-del-rig («registro»: escala, tx, ty, la caja de la cabeza
#   y, cuando llega la expresion neutra, «cara») para que preparar_expresion.py ponga la cara con ella y no a ojo. Y «holguras»: lo que el arte no
#   encaja en codos y rodillas (distancia entre los centros de los dos extremos redondos), que pose_preview.py --mide admite como tolerancia.
#   CUELLO (06/10/2026): con melena o barba (Papa, Mama, Nina: la pieza de la cabeza las incluye) el cuello no esta en la base de la pieza sino en
#   el eje de la cara, a la altura de la barbilla o del borde de arriba del torso (cuello_de); con pelo corto (el Nino), en la base, como siempre.
#   PERFIL (INC-134, 09/10/2026): esta herramienta es SOLO de la vista de FRENTE (escribe en Frontal/). La entrada «perfil» de arte_final.json y
#   «registro.cara_perfil» son de preparar_perfil.py y de preparar_expresion.py --vista perfil: --aplicar los CONSERVA al reescribir la entrada de frente.
#   Por que coreografia.py y pose_preview.py ya tratan al personaje como segmentado ANTES de que el Editor
#   corra «sprites»: prefabs.simula_sprites aplica al arbol del prefab lo que ese modo hara (enciende la Image
#   de cada parte cuyo PNG existe en la carpeta de arte), asi que IsSegmented sale de «el PNG de la antepierna
#   existe» y no del prefab del disco; despues de «sprites» el resultado es el mismo.
#
# MEDIDAS (las mismas reglas que dieron los numeros del Nino; --autoprueba lo demuestra con el):
#   hombro = centro del extremo redondo del humero que queda lejos del antebrazo (esta herramienta mide ESE centro, el centro del pecho; despues
#   de escribir arte_final.json, --aplicar corre hombro.py, que lleva el pivote al borde del torso, donde esta un hombro: 06/10/2026); codo = centro del extremo
#   del antebrazo que toca al humero (si no comparten extremo —la pieza es solo la mano—, avisa y lo pone en
#   el punto de contacto mas cercano; y si el humero se pasa del casquete del antebrazo, --aplicar corre codo.py despues de hombro.py: 08/10/2026); rodilla,
#   igual con muslo y antepierna; cadera = centro del borde de
#   arriba del muslo (--cadera capsula: el centro del extremo redondo, unos 30 px mas abajo: el torso lo tapa y
#   moverla cambiaria los clips del Nino ya revisados); cuello = centro de la base del bbox de la cabeza;
#   pivote del torso = centro de la base de su bbox. CaraBase, Ojos y boca: se colocan en la misma fraccion de la cabeza
#   que tenian en rig_articulaciones.json (llegan despues, en capas propias: C.7, con preparar_expresion.py).
#
# ============================================================================================
# COMO ENTREGAR EL ARTE FINAL (para Santiago)
# ============================================================================================
#   - UN archivo PNG (RGBA) por pieza, y TODOS con EXACTAMENTE el mismo tamano de lienzo: cada archivo es el
#     lienzo entero con solo su pieza opaca, en el mismo sistema de coordenadas (las capas de una figura).
#     Fondo transparente; sin motas sueltas (si las hay, la herramienta las quita).
#   - Diez piezas: torso, cabeza, brazo (humero) y antebrazo-con-mano, derecho e izquierdo, muslo y
#     pierna-con-pie, derecho e izquierdo. El humero y el antebrazo van SEPARADOS, igual el muslo y la
#     pierna con el pie; la cabeza va separada del torso y con la CARA VACIA: los ojos y la boca van en capas
#     propias (Personajes-Resultados.md C.7), no aqui.
#   - Los extremos redondos se SOLAPAN en hombro, codo, cadera y rodilla: cada pieza larga es una capsula con
#     el contorno cerrado en sus dos extremos, y el centro del extremo redondo del antebrazo cae encima del
#     centro del extremo redondo del humero (igual en la rodilla). Asi la articulacion no abre huecos al
#     doblar. Si el antebrazo llega solo con la mano (sin su extremo del codo), la costura se vera.
#   - Orden de dibujo (decision de Santiago, 05/10/2026): la cabeza al fondo, los brazos delante de ella y el TORSO delante
#     de los brazos. Asi el extremo del hombro del humero queda bajo la esquina del torso (el hombro sale por detras) y el
#     torso cubre la base de la cabeza: el cuello de la cabeza tiene que asomar bajo la barbilla sin costura.
#   - Nombres sugeridos: torso_<personaje>, cabeza_<personaje>, brazo_<personaje>_derecho / _izquierdo,
#     mano_<personaje>_derecho / _izquierdo (o antebrazo_...), muslo_<personaje>_derecho / _izquierdo,
#     pie_<personaje>_derecho / _izquierdo (o antepierna_...). Derecho e izquierdo pueden hablar del
#     personaje o de la pantalla: el lado lo decide la posicion de la pieza. Las piezas son de vista frontal,
#     con los brazos y las piernas en reposo (los brazos algo abiertos, como el A-pose del Nino).
# ============================================================================================

import argparse
import hashlib
import json
import math
import os
import random
import re
import subprocess
import sys
import tempfile
import unicodedata
from types import SimpleNamespace

try:
    from PIL import Image, ImageChops, ImageDraw
except ImportError:  # pragma: no cover
    sys.exit("hace falta Pillow: pip install pillow")

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import articulaciones as A  # noqa: E402
import hombro as H  # noqa: E402
import prefabs as P  # noqa: E402
import pose_preview as V  # noqa: E402

UMBRAL_ALFA = 12         # alfa por debajo de esto, a cero
ALTO_FIGURA = 870.0      # de la coronilla (77) al pie mas bajo (947), en el lienzo de 1024
Y_CORONILLA = 77.0
EJE_X = 512.0            # el centro del torso cae aqui
FRACCION_COMUN = 0.6     # el extremo de la pieza distal «toca» a la proximal si dista menos de esta fraccion del radio
TOLERANCIA_PX = 2.0      # --autoprueba: error maximo contra las medidas del Nino
REGION = (40, 30, 984, 990)

ROLES = {
    "torso": {"torso", "tronco", "cuerpo", "pecho", "body", "chest"},
    "cabeza": {"cabeza", "head"},
    "antebrazo": {"antebrazo", "mano", "manos", "forearm", "hand"},
    "brazo": {"brazo", "humero", "arm", "upperarm"},
    "antepierna": {"antepierna", "pie", "pies", "foot", "feet", "shin", "canilla", "pantorrilla"},  # «pies_mama_izquierdo.png» (06/10/2026)
    "muslo": {"muslo", "pierna", "thigh", "leg"},
}
LADOS = {"Izq": {"izquierdo", "izquierda", "izq", "left", "l"}, "Der": {"derecho", "derecha", "der", "right", "r"}}
DOS_LADOS = ("brazo", "antebrazo", "muslo", "antepierna")
IGNORADOS = {"ojos", "ojo", "boca", "eyes", "mouth", "sombra", "retrato"}
# nombre del PNG que se escribe (C.4) por rol y lado, tras «char_<id>_»
NOMBRE_C4 = {("torso", None): "parte_torso", ("cabeza", None): "parte_cabeza",
             ("brazo", "Izq"): "parte_brazo_izq", ("brazo", "Der"): "parte_brazo_der",
             ("antebrazo", "Izq"): "parte_antebrazo_izq", ("antebrazo", "Der"): "parte_antebrazo_der",
             ("muslo", "Izq"): "parte_pierna_izq", ("muslo", "Der"): "parte_pierna_der",
             ("antepierna", "Izq"): "parte_antepierna_izq", ("antepierna", "Der"): "parte_antepierna_der"}
T, C = P.T, P.C


# ============================================================================ 1. leer la entrega


def normaliza(nombre):
    """Minusculas, sin tildes ni eñes."""
    d = unicodedata.normalize("NFD", nombre)
    return "".join(ch for ch in d if unicodedata.category(ch) != "Mn").lower()


def clasifica(archivo):
    """(rol, lado segun el nombre o None) de un nombre de archivo; rol None si no es una pieza que se reconozca."""
    base = normaliza(os.path.splitext(archivo)[0])
    palabras = re.findall(r"[a-z]+", base)
    if any(p in IGNORADOS for p in palabras):
        return "ignorado", None
    rol = next((r for r, nombres in ROLES.items() if any(p in nombres for p in palabras)), None)
    lado = next((l for l, nombres in LADOS.items() if any(p in nombres for p in palabras)), None)
    return rol, lado


class Pieza:
    """Una pieza ya limpia: su bbox en el lienzo de la entrega, su imagen recortada y, tras normalizar, su rect en el 1024."""

    def __init__(self, archivo, rol, lado_nombre):
        self.archivo, self.rol, self.lado_nombre = archivo, rol, lado_nombre
        self.lado = None       # Izq/Der por la posicion
        self.bbox = None       # (x0, y0, x1, y1) en el lienzo de la entrega
        self.imagen = None     # recortada al bbox, ya limpia
        self.limpieza = {}     # motas, tenue, mayor_descartado
        self.norm = None       # imagen normalizada
        self.rect = None       # [x0, y0, x1, y1] en el lienzo de 1024

    @property
    def clave(self):
        return (self.rol, self.lado)


def mayor_componente(mascara):
    """
    Componentes conexas (4 vecinos) de una mascara L de 0/255, por corridas de cada fila y union-find.
    Devuelve (mascara de la mayor, [(area, bbox) de las demas]).
    """
    w, h = mascara.size
    datos = mascara.tobytes()
    patron = re.compile(rb"\xff+")
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
        for m in patron.finditer(fila):
            i = len(padre)
            padre.append(i)
            actual.append((m.start(), m.end(), i))
            corridas.append((y, m.start(), m.end(), i))
        j = 0
        for a, b, i in actual:
            while j < len(previa) and previa[j][1] <= a:
                j += 1
            k = j
            while k < len(previa) and previa[k][0] < b:
                ra, rb = busca(i), busca(previa[k][2])
                if ra != rb:
                    padre[ra] = rb
                k += 1
            j = max(j, k - 1)
        previa = actual
    if not corridas:
        return mascara, []
    comp = {}
    for y, a, b, i in corridas:
        r = busca(i)
        c = comp.setdefault(r, [0, a, y, b, y + 1])
        c[0] += b - a
        c[1], c[2], c[3], c[4] = min(c[1], a), min(c[2], y), max(c[3], b), max(c[4], y + 1)
    mejor = max(comp, key=lambda r: comp[r][0])
    salida = bytearray(w * h)
    for y, a, b, i in corridas:
        if busca(i) == mejor:
            salida[y * w + a:y * w + b] = b"\xff" * (b - a)
    otras = [(c[0], (c[1], c[2], c[3], c[4])) for r, c in comp.items() if r != mejor]
    return Image.frombytes("L", (w, h), bytes(salida)), sorted(otras, reverse=True)


def limpia(im):
    """
    Alfa < UMBRAL_ALFA a cero y solo la componente mas grande. Devuelve (imagen recortada al bbox, bbox, info) o
    (None, None, info) si la pieza esta vacia.
    """
    a = im.getchannel("A")
    tenue = sum(a.histogram()[1:UMBRAL_ALFA])
    a = a.point(lambda v: 0 if v < UMBRAL_ALFA else v)
    binaria = a.point(lambda v: 255 if v else 0)
    mascara, otras = mayor_componente(binaria)
    a = ImageChops.multiply(a, mascara)
    bbox = a.getbbox()
    info = {"motas": len(otras), "px_motas": sum(o[0] for o in otras), "mayor_descartado": otras[0][0] if otras else 0,
            "tenue": tenue}
    if bbox is None:
        return None, None, info
    rgb = im.convert("RGB")
    out = Image.merge("RGBA", rgb.split() + (a,)).crop(bbox)
    return out, bbox, info


def lee_entrega(carpeta):
    """
    Lee los PNG de la carpeta. Devuelve (piezas, lienzo, avisos, errores): piezas es {(rol, lado_pos): Pieza} con
    el lado ya decidido por la posicion.
    """
    avisos, errores = [], []
    archivos = sorted(f for f in os.listdir(carpeta) if f.lower().endswith(".png"))
    if not archivos:
        return {}, None, avisos, ["no hay ningun .png en %s" % carpeta]
    # duplicados byte a byte: se queda el que lleva lado en el nombre (o el primero)
    por_hash = {}
    for f in archivos:
        with open(os.path.join(carpeta, f), "rb") as fh:
            por_hash.setdefault(hashlib.sha1(fh.read()).hexdigest(), []).append(f)
    unicos = []
    for grupo in por_hash.values():
        grupo.sort(key=lambda f: (clasifica(f)[1] is None, f))
        unicos.append(grupo[0])
        for d in grupo[1:]:
            avisos.append("%s es un duplicado byte a byte de %s: se descarta" % (d, grupo[0]))
    candidatas = []
    for f in sorted(unicos):
        rol, lado = clasifica(f)
        if rol == "ignorado":
            avisos.append("%s: es una capa de la cara (ojos/boca): no se procesa aqui (C.7)" % f)
        elif rol is None:
            avisos.append("%s: no reconozco la pieza por el nombre (torso, cabeza, brazo, antebrazo/mano, muslo/pierna, "
                          "antepierna/pie): se ignora" % f)
        else:
            candidatas.append(Pieza(f, rol, lado))
    tamanos = {}
    imagenes = {}
    for pz in candidatas:
        im = Image.open(os.path.join(carpeta, pz.archivo)).convert("RGBA")
        imagenes[pz.archivo] = im
        tamanos.setdefault(im.size, []).append(pz.archivo)
    if len(tamanos) > 1:
        detalle = "; ".join("%dx%d: %s" % (w, h, ", ".join(fs)) for (w, h), fs in tamanos.items())
        return {}, None, avisos, ["las capas no comparten lienzo (%s): cada archivo debe ser el lienzo ENTERO con solo su "
                                  "pieza opaca, todos del mismo tamano" % detalle]
    lienzo = next(iter(tamanos)) if tamanos else None
    for pz in candidatas:
        pz.imagen, pz.bbox, pz.limpieza = limpia(imagenes[pz.archivo])
        if pz.imagen is None:
            errores.append("%s esta vacia (todo su alfa es menor de %d)" % (pz.archivo, UMBRAL_ALFA))
        elif pz.limpieza["mayor_descartado"] > 0.01 * sum(pz.imagen.getchannel("A").point(lambda v: 1 if v else 0).histogram()[1:]):
            avisos.append("%s: se descarta un trozo suelto de %d px (no conecta con la pieza por 4 vecinos): si es parte de "
                          "la pieza, debe llegar conectado" % (pz.archivo, pz.limpieza["mayor_descartado"]))
    candidatas = [pz for pz in candidatas if pz.imagen is not None]
    # agrupar por rol y decidir el lado por la posicion
    piezas = {}
    for rol in ROLES:
        grupo = [pz for pz in candidatas if pz.rol == rol]
        if rol in DOS_LADOS:
            if len(grupo) != 2:
                errores.append("falta o sobra %s: hacen falta 2 (derecho e izquierdo) y hay %d%s" % (
                    rol, len(grupo), " (" + ", ".join(g.archivo for g in grupo) + ")" if grupo else ""))
                continue
            grupo.sort(key=lambda pz: (pz.bbox[0] + pz.bbox[2]) / 2.0)
            grupo[0].lado, grupo[1].lado = "Izq", "Der"
        else:
            if len(grupo) != 1:
                errores.append("falta o sobra %s: hace falta 1 y hay %d%s" % (
                    rol, len(grupo), " (" + ", ".join(g.archivo for g in grupo) + ")" if grupo else ""))
                continue
        for pz in grupo:
            piezas[pz.clave] = pz
    # nombre contra posicion
    con_nombre = [pz for pz in piezas.values() if pz.lado_nombre and pz.lado]
    contradicen = [pz for pz in con_nombre if pz.lado_nombre != pz.lado]
    if contradicen and len(contradicen) == len(con_nombre):
        avisos.append("los nombres dicen el lado del PERSONAJE y no el de la pantalla (derecho esta a la izquierda de la "
                      "pantalla, en las %d piezas con lado): es lo normal en una figura de frente; se usa la posicion"
                      % len(con_nombre))
    else:
        for pz in contradicen:
            avisos.append("%s: el nombre dice %s pero la pieza esta a la %s de la pantalla; se usa la posicion (convencion "
                          "del rig: Izq = izquierda de la pantalla). Revisa que el arte no este espejado" % (
                              pz.archivo, "derecho" if pz.lado_nombre == "Der" else "izquierdo",
                              "derecha" if pz.lado == "Der" else "izquierda"))
    return piezas, lienzo, avisos, errores


# ============================================================================ 2. normalizar y medir


def normaliza_piezas(piezas):
    """Escala unica, torso a x = 512, coronilla a y = 77; deja pz.norm y pz.rect. Devuelve (escala, tx, ty)."""
    ymin = min(pz.bbox[1] for pz in piezas.values())
    ymax = max(pz.bbox[3] for pz in piezas.values())
    torso = piezas[("torso", None)]
    cx = (torso.bbox[0] + torso.bbox[2]) / 2.0
    s = ALTO_FIGURA / float(ymax - ymin)
    tx, ty = EJE_X - s * cx, Y_CORONILLA - s * ymin
    for pz in piezas.values():
        w, h = pz.imagen.size
        nw, nh = max(1, int(round(w * s))), max(1, int(round(h * s)))
        pz.norm = pz.imagen.resize((nw, nh), Image.LANCZOS)
        x0, y0 = int(round(s * pz.bbox[0] + tx)), int(round(s * pz.bbox[1] + ty))
        pz.rect = [x0, y0, x0 + nw, y0 + nh]
    return s, tx, ty


def registro_de(res):
    """
    La transformacion lienzo-de-entrega -> lienzo-del-rig de ESTA entrega: x' = escala * x + tx, y' = escala * y + ty. Es la que usaron
    las partes; la cara (que llega en el mismo lienzo registrado) se coloca con la misma, no a ojo. Se guarda en arte_final.json
    («registro»), con la caja de la cabeza en el lienzo de la entrega para poder comprobar que sigue siendo la de la tabla.
    """
    cab = res.piezas[("cabeza", None)]
    return {"lienzo": [res.lienzo[0], res.lienzo[1]], "escala": round(res.escala, 6), "tx": round(res.tx, 3), "ty": round(res.ty, 3),
            "cabeza": [int(v) for v in cab.bbox]}


def caja_a_lienzo(registro, caja):
    """Una caja (x0, y0, x1, y1) del lienzo de la entrega, como rect entero del lienzo de 1024: el mismo redondeo que normaliza_piezas."""
    s, tx, ty = registro["escala"], registro["tx"], registro["ty"]
    return [int(round(s * caja[0] + tx)), int(round(s * caja[1] + ty)), int(round(s * caja[2] + tx)), int(round(s * caja[3] + ty))]


def a_lienzo(pz, p):
    """Un punto de pz.norm (pixeles de la imagen) al lienzo de 1024."""
    return (pz.rect[0] + p[0], pz.rect[1] + p[1])


def extremos(pz):
    """(extremo a, extremo b, radio) de una pieza alargada, en el lienzo de 1024 (V.extremos_capsula sobre el alfa)."""
    try:
        a, b, r = V.extremos_capsula(pz.norm)
    except (ValueError, ZeroDivisionError):  # una pieza que no es una capsula
        w, h = pz.norm.size
        a, b, r = (w / 2.0, 0.0), (w / 2.0, float(h)), min(w, h) / 4.0
    return a_lienzo(pz, a), a_lienzo(pz, b), r


def pixeles(pz, paso=3):
    a = pz.norm.getchannel("A")
    w, h = a.size
    d = a.load()
    return [(pz.rect[0] + x, pz.rect[1] + y) for y in range(0, h, paso) for x in range(0, w, paso) if d[x, y] > 127]


def centroide(pz):
    pts = pixeles(pz, 2)
    return (sum(p[0] for p in pts) / len(pts), sum(p[1] for p in pts) / len(pts))


def dist(p, q):
    return math.hypot(p[0] - q[0], p[1] - q[1])


def junta(proximal, distal, nombre, avisos):
    """
    La articulacion entre dos piezas. (punto, extremo_proximal_de_la_distal_o_None). El extremo de la pieza distal
    que toca a la proximal es el pivote de la distal: gira alrededor de el sin despegarse. Si ese extremo no cae
    sobre el extremo de la proximal (dentro de FRACCION_COMUN de su radio), la costura no sera limpia: aviso y
    punto de contacto mas cercano de las dos piezas.
    """
    a1, a2, r = extremos(proximal)
    cen = centroide(distal)
    # el extremo de la proximal que mira a la distal es el codo/rodilla; el otro es el hombro/cadera
    codo = min((a1, a2), key=lambda p: dist(p, cen))
    b1, b2, rb = extremos(distal)
    propio = min((b1, b2), key=lambda p: dist(p, codo))
    d = dist(propio, codo)
    if d <= FRACCION_COMUN * r:
        return propio, d, False
    avisos.append("%s: la pieza distal no comparte extremo con la proximal (sus extremos distan %.0f px y el radio es %.0f): "
                  "la costura no sera limpia; la articulacion va en el punto de contacto mas cercano" % (nombre, d, r))
    pa, pb = pixeles(proximal), pixeles(distal)
    q = min(pb, key=lambda p: dist(p, codo))
    p = min(pa, key=lambda p: dist(p, q))
    return ((p[0] + q[0]) / 2.0, (p[1] + q[1]) / 2.0), d, True


def hombro_o_cadera(proximal, distal):
    """El extremo de la pieza proximal que NO mira a la distal: el hombro (humero) o la cadera (muslo)."""
    a1, a2, r = extremos(proximal)
    cen = centroide(distal)
    return max((a1, a2), key=lambda p: dist(p, cen)), r


def borde_superior(pz):
    """(x, y) del centro del borde de arriba del bbox de la pieza (como el pivote de cadera del Nino)."""
    return ((pz.rect[0] + pz.rect[2]) / 2.0, float(pz.rect[1]))


def entero(v):
    return int(round(v))


FRACCION_PELO_BAJO_CUELLO = 0.05   # si el pelo o la barba llegan mas abajo que esto (del alto de la cabeza), el cuello no esta en su base


def cuello_de(cab, torso, avisos):
    """
    El punto donde gira la cabeza. Con el pelo o la barba CORTOS (el Nino) es el centro de la base de la cabeza, como siempre. Con pelo largo o barba
    (Papa, Mama, Nina: la pieza de la cabeza incluye la melena o la barba, que cuelgan muy por debajo de la barbilla) la base esta a media
    espalda: girar la cabeza alrededor de ella la hace oscilar de lado como un pendulo (Mama, 06/10/2026: 230 px del lienzo de 1024 bajo la
    barbilla). El cuello esta donde la cabeza apoya en el cuerpo: sobre el EJE de la cara y a la altura de la barbilla, y nunca mas arriba que el
    borde de arriba del torso (Papa: la barba cae sobre el pecho, y la barbilla queda sobre el torso). La barbilla es la base del ovalo de piel
    que mide preparar_expresion.mide_cabeza (lo mide sobre el PNG ya normalizado); si no se puede medir, la base de la cabeza.
    """
    base = float(cab.rect[3])
    centro = ((cab.rect[0] + cab.rect[2]) / 2.0, base)
    try:
        import preparar_expresion as PE
        medida = PE.mide_cabeza(cab.norm, cab.rect)
    except Exception as e:  # noqa: BLE001 (cualquier fallo de la medida: se vuelve a la regla de siempre)
        avisos.append("no pude medir la barbilla de la cabeza (%s): el cuello queda en la base de la cabeza" % e)
        return centro
    alto = float(cab.rect[3] - cab.rect[1])
    y = max(medida.barbilla, float(torso.rect[1]))
    if base - y <= FRACCION_PELO_BAJO_CUELLO * alto:
        return centro
    avisos.append("el pelo o la barba llegan %.0f px por debajo del cuello: la cabeza gira en (%.0f, %.0f) (eje de la cara, a la altura de la barbilla o del borde "
                  "de arriba del torso) y no en la base de la pieza (%.0f, %.0f)" % (base - y, medida.eje, y, centro[0], centro[1]))
    return (medida.eje, min(y, base))


def mide(piezas, pid, cadera="borde"):
    """
    Las medidas de la entrega ya normalizada. Devuelve (tabla {"nodos", "partes"}, joints {nombre: (x, y)}, avisos),
    con la forma de arte_final.json.
    """
    avisos = []
    rig = P.cargar_rig()
    actual = P.personaje_rig(rig, pid)
    pref = "char_" + pid
    nodos, partes = [], []
    joints = {}
    holguras = {}
    # humero / antebrazo y muslo / antepierna
    medidas = {}
    for lado in ("Izq", "Der"):
        br, ab = piezas[("brazo", lado)], piezas[("antebrazo", lado)]
        hombro, r_h = hombro_o_cadera(br, ab)
        codo, d_codo, _ = junta(br, ab, "codo " + lado, avisos)
        mu, an = piezas[("muslo", lado)], piezas[("antepierna", lado)]
        cad_c, r_m = hombro_o_cadera(mu, an)
        cad = borde_superior(mu) if cadera == "borde" else cad_c
        rodilla, d_rod, _ = junta(mu, an, "rodilla " + lado, avisos)
        medidas[lado] = (hombro, codo, cad, rodilla)
        joints["Hombro" + lado], joints["Codo" + lado] = hombro, codo
        joints["Cadera" + lado], joints["Rodilla" + lado] = cad, rodilla
        # lo que el arte no encaja: distancia entre los centros de los dos extremos redondos de la articulacion (la pieza que gira esta
        # sobre el pivote; la otra queda a esta distancia). pose_preview.py --mide la admite como tolerancia de esa articulacion
        for nombre, d in (("Codo" + lado, d_codo), ("Rodilla" + lado, d_rod)):
            if d > 2.0:
                holguras[nombre] = round(d, 1)
    for lado, suf in (("Izq", "izq"), ("Der", "der")):
        ab = piezas[("antebrazo", lado)]
        nodos.append({"nombre": "Codo" + lado, "tipo": "articulacion", "padre": T + "/Brazo" + lado,
                      "punto": [entero(v) for v in medidas[lado][1]], "imagen": "Antebrazo" + lado,
                      "sprite": "%s_parte_antebrazo_%s" % (pref, suf), "rect": list(ab.rect)})
    for lado, suf in (("Izq", "izq"), ("Der", "der")):
        an = piezas[("antepierna", lado)]
        nodos.append({"nombre": "Rodilla" + lado, "tipo": "articulacion", "padre": C + "/Pierna" + lado,
                      "punto": [entero(v) for v in medidas[lado][3]], "imagen": "Antepierna" + lado,
                      "sprite": "%s_parte_antepierna_%s" % (pref, suf), "rect": list(an.rect)})
    cab = piezas[("cabeza", None)]
    cuello = cuello_de(cab, piezas[("torso", None)], avisos)
    joints["Cuello"] = cuello
    nodos.append({"nombre": "Cuello", "tipo": "articulacion", "padre": T, "punto": [entero(v) for v in cuello],
                  "imagen": "Cabeza", "sprite": pref + "_parte_cabeza", "rect": list(cab.rect)})
    # ojos y boca: la misma fraccion de la cabeza que tenian en la tabla actual
    cab_antes = next((n for n in actual["nodos"] if n["nombre"] == "Cuello"), None)
    for nombre in ("CaraBase", "Ojos", "Boca"):  # CaraBase, el primer hijo de Cabeza, antes que Ojos y Boca
        previo = next((n for n in actual["nodos"] if n["nombre"] == nombre), None)
        if previo is None or cab_antes is None:
            continue
        ax0, ay0, ax1, ay1 = cab_antes["rect"]
        aw, ah = float(ax1 - ax0), float(ay1 - ay0)
        w, h = cab.rect[2] - cab.rect[0], cab.rect[3] - cab.rect[1]
        f = lambda p: (cab.rect[0] + (p[0] - ax0) / aw * w, cab.rect[1] + (p[1] - ay0) / ah * h)
        r = previo["rect"]
        (rx0, ry0), (rx1, ry1) = f((r[0], r[1])), f((r[2], r[3]))
        nodos.append({"nombre": nombre, "tipo": "imagen", "padre": T + "/Cuello/Cabeza",
                      "punto": [entero(v) for v in f(previo["punto"])], "imagen": nombre, "sprite": previo["sprite"],
                      "rect": [entero(rx0), entero(ry0), entero(rx1), entero(ry1)]})
    for lado, suf in (("Izq", "izq"), ("Der", "der")):
        br = piezas[("brazo", lado)]
        partes.append({"nombre": "Brazo" + lado, "ruta": T + "/Brazo" + lado, "sprite": "%s_parte_brazo_%s" % (pref, suf),
                       "rect": list(br.rect), "pivote": [entero(v) for v in medidas[lado][0]]})
    for lado, suf in (("Izq", "izq"), ("Der", "der")):
        mu = piezas[("muslo", lado)]
        partes.append({"nombre": "Pierna" + lado, "ruta": C + "/Pierna" + lado, "sprite": "%s_parte_pierna_%s" % (pref, suf),
                       "rect": list(mu.rect), "pivote": [entero(v) for v in medidas[lado][2]]})
    to = piezas[("torso", None)]
    partes.append({"nombre": "Torso", "ruta": T + "/Torso", "sprite": pref + "_parte_torso", "rect": list(to.rect),
                   "pivote": [entero((to.rect[0] + to.rect[2]) / 2.0), to.rect[3]]})
    return {"nodos": nodos, "partes": partes, "holguras": holguras}, joints, avisos


# ============================================================================ 3. el informe y el composite


POSES = [
    ("reposo", {}),
    ("brazos en alto, codos y cuello", {"hombro_Izq": -55, "hombro_Der": 55, "codo_Izq": -45, "codo_Der": 45, "cuello": 8}),
    ("manos al frente", {"hombro_Izq": 24, "hombro_Der": -24, "codo_Izq": 70, "codo_Der": -70}),
    ("rodillas y muslos", {"cadera_Izq": -12, "cadera_Der": 12, "rodilla_Izq": 28, "rodilla_Der": -28, "cuello": -6}),
]


def renderiza(piezas, joints, giros, escala=0.5, pid=None):
    """
    La figura con los giros dados (grados, + antihorario) sobre un RGBA: lo mismo que haria el rig, con las piezas ya normalizadas.
    El orden de dibujo es el del rig (articulaciones.orden_tronco_de): las piernas al fondo y los hijos de Tronco en el orden de la tabla.
    """
    ox, oy = REGION[0], REGION[1]
    tam = (int(round((REGION[2] - REGION[0]) * escala)), int(round((REGION[3] - REGION[1]) * escala)))
    lienzo = Image.new("RGBA", tam, (236, 232, 222, 255))
    M = V
    ident = M.mat_id()

    def rot(nombre, pivote):
        return M.mat_local(pivote, 0.0, 0.0, giros.get(nombre, 0.0), 1.0, 1.0)

    m = {}
    for lado in ("Izq", "Der"):
        m[("muslo", lado)] = rot("cadera_" + lado, joints["Cadera" + lado])
        m[("antepierna", lado)] = M.mat_mul(m[("muslo", lado)], rot("rodilla_" + lado, joints["Rodilla" + lado]))
        m[("brazo", lado)] = rot("hombro_" + lado, joints["Hombro" + lado])
        m[("antebrazo", lado)] = M.mat_mul(m[("brazo", lado)], rot("codo_" + lado, joints["Codo" + lado]))
    m[("torso", None)] = ident
    m[("cabeza", None)] = rot("cuello", joints["Cuello"])
    # el orden de dibujo del rig: las piernas al fondo y, despues, los hijos de Tronco como los ordena orden_tronco (INC-133: el
    # humero tras el torso y el antebrazo delante de el, de la cara y de las piernas)
    por_nombre = {"Cuello": ("cabeza", None), "Torso": ("torso", None)}
    for lado in ("Izq", "Der"):
        por_nombre["Brazo" + lado] = ("brazo", lado)
        por_nombre["Antebrazo" + lado] = ("antebrazo", lado)
    orden = [("muslo", "Izq"), ("antepierna", "Izq"), ("muslo", "Der"), ("antepierna", "Der")]
    orden += [por_nombre[n] for n in A.orden_tronco_de(pid or "nino")]
    for clave in orden:
        pz = piezas[clave]
        capa = M.Capa(SimpleNamespace(ruta=str(clave)), tuple(pz.rect), pz.norm, (1, 1, 1, 1))
        sal = M._capa_a_lienzo(capa, {str(clave): m[clave]}, escala, (ox, oy), tam)
        if sal is not None:
            lienzo.alpha_composite(sal)
    return lienzo


def composite(piezas, joints, ruta, pid):
    """Cuatro paneles: reposo con las articulaciones marcadas y tres poses de prueba."""
    paneles = []
    for i, (titulo, giros) in enumerate(POSES):
        im = renderiza(piezas, joints, giros, pid=pid)
        d = ImageDraw.Draw(im)
        d.rectangle([0, 0, im.width, 14], fill=(255, 255, 255, 230))
        d.text((4, 2), "%s: %s" % (pid, titulo), fill=(30, 30, 30, 255))
        if i == 0:
            colores = {"Hombro": (220, 30, 30), "Codo": (30, 150, 30), "Cadera": (30, 60, 220), "Rodilla": (200, 160, 0),
                       "Cuello": (180, 30, 180)}
            for nombre, (x, y) in joints.items():
                color = next(c for k, c in colores.items() if nombre.startswith(k))
                px, py = (x - REGION[0]) * 0.5, (y - REGION[1]) * 0.5
                d.ellipse([px - 4, py - 4, px + 4, py + 4], outline=color + (255,), width=2)
        paneles.append(im)
    w, h = paneles[0].size
    hoja = Image.new("RGB", (w * len(paneles), h), (236, 232, 222))
    for i, im in enumerate(paneles):
        hoja.paste(im.convert("RGB"), (i * w, 0))
    hoja.save(ruta)


class Resultado:
    pass


def procesa(carpeta, pid, cadera="borde"):
    """Todo menos escribir: lee, limpia, normaliza y mide. Devuelve un Resultado (res.errores vacio = se puede aplicar)."""
    res = Resultado()
    res.pid, res.carpeta = pid, carpeta
    res.piezas, res.lienzo, res.avisos, res.errores = lee_entrega(carpeta)
    res.tabla = res.joints = None
    if res.errores:
        return res
    res.escala, res.tx, res.ty = normaliza_piezas(res.piezas)
    res.registro = registro_de(res)
    res.tabla, res.joints, av = mide(res.piezas, pid, cadera)
    res.avisos += av
    for pz in res.piezas.values():
        r = pz.rect
        if r[0] < 0 or r[1] < 0 or r[2] > 1024 or r[3] > 1024:
            res.avisos.append("%s: el rect %s se sale del lienzo de 1024" % (pz.archivo, r))
    return res


def informe(res):
    pid = res.pid
    print("== Entrega de %s: %s" % (pid, res.carpeta))
    if res.lienzo:
        print("lienzo comun: %dx%d" % res.lienzo)
    if res.piezas:
        print("\npiezas reconocidas (%d):" % len(res.piezas))
        for clave, pz in sorted(res.piezas.items(), key=lambda kv: list(ROLES).index(kv[0][0])):
            nombre = NOMBRE_C4[clave]
            lim = pz.limpieza
            extra = ""
            if lim["motas"] or lim["tenue"]:
                extra = "  [limpieza: %d motas sueltas (%d px), %d px de alfa tenue]" % (lim["motas"], lim["px_motas"], lim["tenue"])
            print("  %-26s -> %-3s %-11s char_%s_%s.png%s" % (pz.archivo, pz.lado or "-", pz.rol, pid, nombre, extra))
    for e in res.errores:
        print("ERROR", e)
    if res.errores:
        print("\nla entrega no se puede procesar: corrige lo anterior")
        return
    print("\nnormalizacion: escala %.4f (la figura mide %.0f px en el lienzo de la entrega y pasa a %.0f), torso en x = %.0f" % (
        res.escala, ALTO_FIGURA / res.escala, ALTO_FIGURA, EJE_X))
    print("\nmedidas en el lienzo de 1024 (y hacia abajo):")
    for nombre, (x, y) in res.joints.items():
        print("  %-12s (%6.1f, %6.1f)" % (nombre, x, y))
    print("\nrects:")
    for clave, pz in sorted(res.piezas.items(), key=lambda kv: list(ROLES).index(kv[0][0])):
        print("  %-26s %-10s %s  %dx%d px" % (NOMBRE_C4[clave], pz.lado or "", pz.rect, pz.norm.size[0], pz.norm.size[1]))
    mb = sum(pz.norm.size[0] * pz.norm.size[1] * 4 for pz in res.piezas.values()) / 1e6
    print("\ntextura sin comprimir de las diez piezas: %.1f MB (el margen del paquete, RNF-06, es de unos 21 MB para todo el juego)" % mb)
    for a in res.avisos:
        print("AVISO", a)


SUBCARPETA_PARTES = "Frontal"  # las partes de frente de cada personaje: Assets/Game/Art/Characters/<Carpeta>/Frontal/


def lista_escritura(res):
    """
    [(ruta absoluta, existia)] de los PNG que --aplicar escribiria: en <Carpeta>/Frontal/. Una parte que YA esta en algun sitio de
    la carpeta del personaje (en Frontal/, o en la raiz mientras el Editor no ha movido las partes) se sustituye ahi mismo, para
    conservar su .meta y su GUID; las nuevas nacen en Frontal/.
    """
    carpeta = P.PERSONAJES[res.pid][1]
    out = []
    for clave in res.piezas:
        nombre = "char_%s_%s" % (res.pid, NOMBRE_C4[clave])
        previa = P._png_de(carpeta, nombre)
        ruta = previa or os.path.join(P.PERSONAJES_ARTE, carpeta, SUBCARPETA_PARTES, nombre + ".png")
        out.append((ruta, previa is not None))
    return sorted(out)


def bloque_local(pid):
    prefab, carpeta, _ = P.PERSONAJES[pid]
    art = "Assets/Game/Art/Characters/" + carpeta
    return """
==================== Sesion local (Santiago), con el Editor abierto ====================
 0. git pull   (trae esta rama con el arte, arte_final.json, rig_articulaciones.json y clips_personajes.json)
 1. Copiar el generador (el Editor crea el .meta solo, nunca a mano):
      Copy-Item claudeDocs/tasks/Personajes/herramientas/BuildRigsFinal.cs.txt Assets/Editor/ClaudeBuildRigsFinal.cs
 2. Recompilar y esperar:   pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 recompile
 3. Por coplay execute_script, EN ESTE ORDEN (cada una devuelve su log; «clips» se niega si faltan los nodos):
      ClaudeBuildRigsFinal.Execute("estado")     // antes: el personaje aun sin segmentar
      ClaudeBuildRigsFinal.Execute("nodos")      // lo que la tabla pide y el prefab no tiene (codos, rodillas, cuello, CaraBase); idempotente
      ClaudeBuildRigsFinal.Execute("sprites")    // asigna los PNG nuevos, rect y pivote de la tabla (tambien el cuello y los codos que cambiaron)
      ClaudeBuildRigsFinal.Execute("orden")      // INC-133: orden de Tronco y antebrazos delante, con su ancla bajo el codo (idempotente)
      ClaudeBuildRigsFinal.Execute("clips")      // vuelca clips_personajes.json en los .anim
      ClaudeBuildRigsFinal.Execute("estado")     // despues: segmentado = si, nodos completos, antebrazos en Tronco con ancla
 4. Comprobar los fileID:   git diff -U0 Assets/Game/Prefabs/Characters   (no debe QUITAR lineas «--- !u!» ni tocar
    m_Controller / m_Script de lo que ya existia; los .meta de clips no cambian)
 5. Pruebas:
      pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-edit CharacterRig_
      pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-play CharacterRig
 6. Borrar el andamiaje:   Remove-Item Assets/Editor/ClaudeBuildRigsFinal.cs, Assets/Editor/ClaudeBuildRigsFinal.cs.meta
 7. git add (el arte con sus .meta, el prefab, los .anim y las tablas):
      git add %(art)s Assets/Game/Prefabs/Characters/%(prefab)s.prefab Assets/Game/Prefabs/Characters/%(prefab)s.prefab.meta
      git add Assets/Game/Art/Characters/%(carpeta)s/Animations claudeDocs/tasks/Personajes/herramientas/arte_final.json
      git add claudeDocs/tasks/Personajes/herramientas/rig_articulaciones.json claudeDocs/tasks/Personajes/herramientas/clips_personajes.json
    (las escenas y los 18 assets narrativos no deben aparecer en git status: si aparecen, un fileID cambio)
========================================================================================
""" % {"art": art, "prefab": prefab, "carpeta": carpeta}


def corre(args, etiqueta):
    print("\n$ %s" % " ".join(os.path.basename(a) if i == 1 else a for i, a in enumerate(args)))
    r = subprocess.run(args, cwd=AQUI, capture_output=True, text=True)
    lineas = [l for l in r.stdout.splitlines() if l.strip()]
    fallas = [l for l in lineas if "FALLA" in l]  # los clips o articulaciones que no pasan, enteros
    for l in (fallas + [l for l in lineas[-3:] if l not in fallas]) if fallas else lineas[-4:]:
        print("   " + l[:200])
    if r.returncode:
        print("   %s: salio con codigo %d %s" % (etiqueta, r.returncode, r.stderr.strip()[-300:]))
    return r.returncode


def _mismos_pixeles(ruta, imagen):
    """
    El PNG de disco decodifica a los mismos pixeles RGBA que «imagen». Se compara canal por canal: en una imagen RGBA, Image.getbbox() mira por defecto SOLO el alfa
    (Pillow reciente, alpha_only=True), asi que dos imagenes con el mismo alfa y otros colores —el _reposo de Algoritm con los brazos detras del torso en vez de delante,
    INC-147— se daban por iguales y no se reescribian.
    """
    try:
        previa = Image.open(ruta).convert("RGBA")
    except Exception:
        return False
    return previa.size == imagen.size and not any(canal.getbbox() for canal in ImageChops.difference(previa, imagen.convert("RGBA")).split())


def aplica(res, args):
    pid = res.pid
    carpeta = os.path.join(P.PERSONAJES_ARTE, P.PERSONAJES[pid][1])
    print("\n== Aplicando al repo ==")
    for ruta, existia in lista_escritura(res):
        clave = next(k for k in res.piezas if os.path.basename(ruta) == "char_%s_%s.png" % (pid, NOMBRE_C4[k]))
        os.makedirs(os.path.dirname(ruta), exist_ok=True)  # Frontal/ (Unity crea el .meta de la carpeta y de los PNG al importarlos)
        if existia and _mismos_pixeles(ruta, res.piezas[clave].norm):
            # los mismos pixeles: no se reescribe (Pillow no devuelve los mismos bytes y el diff de git se llenaria de PNG iguales)
            print("  igual     %s" % os.path.relpath(ruta, P.RAIZ))
            continue
        res.piezas[clave].norm.save(ruta)
        print("  %s %s" % ("sustituye" if existia else "nuevo    ", os.path.relpath(ruta, P.RAIZ)))
    personajes = A.cargar_arte_final()
    registro = dict(res.registro)
    previo = personajes.get(pid, {}).get("registro", {})
    if "cara" in previo and previo.get("escala") == registro["escala"] and previo.get("tx") == registro["tx"]:
        registro["cara"] = previo["cara"]   # la caja de la cara que fijo la expresion neutra sigue valiendo si el registro es el mismo
    if "cara_perfil" in previo:
        registro["cara_perfil"] = previo["cara_perfil"]   # INC-134: la caja de la cara de PERFIL es de otro lienzo (el de la entrega de perfil): esta entrega de frente no la invalida
    perfil_previo = personajes.get(pid, {}).get("perfil")   # INC-134: la entrada «perfil» (preparar_perfil.py) es del cuerpo de perfil: reaplicar el frente no la borra
    personajes[pid] = dict(res.tabla, registro=registro)   # «registro»: de donde sale el sitio de la cara (preparar_expresion.py)
    if perfil_previo is not None:
        personajes[pid]["perfil"] = perfil_previo
    A.guardar_arte_final(personajes)
    print("  arte_final.json: entrada de %s" % pid)
    py = sys.executable
    fallos = 0
    # el pivote del hombro: de aqui sale el centro del extremo redondo del humero (el centro del pecho); hombro.py lo lleva al borde del torso, donde esta un
    # hombro, para que los brazos cuelguen relajados y no abiertos en A (Santiago, 06/10/2026). Con los PNG ya en el repo y antes de regenerar nada
    fallos += 1 if corre([py, os.path.join(AQUI, "hombro.py"), pid, "--aplica"], "hombro") else 0
    # el humero y el antebrazo encajados en el codo (Santiago, 08/10/2026): si el humero se pasa del casquete del antebrazo (Papa lo entrego solapado 42 y 21 px,
    # el aviso de «la costura no sera limpia» de arriba) asomaba por el codo al doblarlo; codo.py sube el humero y baja el antebrazo, despues de hombro.py y sin mover el hombro
    fallos += 1 if corre([py, os.path.join(AQUI, "codo.py"), pid, "--aplica"], "codo") else 0
    fallos += 1 if corre([py, os.path.join(AQUI, "articulaciones.py")], "articulaciones") else 0
    fallos += 1 if corre([py, os.path.join(AQUI, "coreografia.py"), "--valida"], "coreografia") else 0
    fallos += 1 if corre([py, os.path.join(AQUI, "pose_preview.py"), "--solo", pid, "--salida", args.salida], "pose_preview") else 0
    fallos += 1 if corre([py, os.path.join(AQUI, "pose_preview.py"), "--mide", pid], "pose_preview --mide") else 0
    print("\n%s" % ("TODO EN VERDE" if not fallos else "%d pasos fallaron: revisa antes de entregar" % fallos))
    print(bloque_local(pid))
    return fallos


# ============================================================================ 4. la autoprueba


def es_entrega_de_perfil(carpeta):
    """True si la ruta de la entrega o el nombre de algun archivo de ella dicen «perfil» (sin tildes ni mayusculas): es de preparar_perfil.py (INC-134)."""
    def pliega(t):
        return "".join(c for c in unicodedata.normalize("NFKD", t) if not unicodedata.combining(c)).lower()
    return "perfil" in pliega(carpeta) or any("perfil" in pliega(f) for f in os.listdir(carpeta))


def entrega_sintetica(destino, a=1.3, centro=(600, 150), lienzo=(1300, 1500)):
    """
    Fabrica una entrega a partir del arte final del Nino del repo: sus diez piezas en un lienzo de 1300x1500 con
    una escala y un desplazamiento cualquiera, con motas de alfa sueltas, alfa tenue y un duplicado, y nombres a
    la manera de la entrega real (los lados hablan del personaje: derecho = izquierda de la pantalla).
    """
    rig = P.cargar_rig()
    nino = P.personaje_rig(rig, "nino")
    piezas = {}
    for p in nino["partes"]:
        piezas[p["nombre"]] = (p["sprite"], p["rect"])
    for n in nino["nodos"]:
        if n["nombre"] in ("CodoIzq", "CodoDer", "RodillaIzq", "RodillaDer", "Cuello"):
            piezas[n["imagen"]] = (n["sprite"], n["rect"])
    nombres = {"Torso": "torso_niño", "Cabeza": "cabeza_niño",
               "BrazoIzq": "brazo_niño_derecho", "BrazoDer": "brazo_niño_izquierdo",
               "AntebrazoIzq": "mano_niño_derecho", "AntebrazoDer": "mano_niño_izquierdo",
               "PiernaIzq": "muslo_niño_derecho", "PiernaDer": "muslo_niño_izquierdo",
               "AntepiernaIzq": "pie_niño_derecho", "AntepiernaDer": "pie_niño_izquierdo"}
    azar = random.Random(11)
    os.makedirs(destino, exist_ok=True)
    for clave, (sprite, rect) in piezas.items():
        im = Image.open(P._png_de("Boy", sprite)).convert("RGBA")  # Frontal/ o la raiz de la carpeta: _png_de las recorre todas
        w, h = im.size
        im = im.resize((int(round(w * a)), int(round(h * a))), Image.LANCZOS)
        x = int(round(a * (rect[0] - 512.0) + centro[0]))
        y = int(round(a * (rect[1] - Y_CORONILLA) + centro[1]))
        cv = Image.new("RGBA", lienzo, (0, 0, 0, 0))
        cv.alpha_composite(im, (x, y))
        d = ImageDraw.Draw(cv)
        for _ in range(5):  # motas sueltas, opacas, de 1 a 6 px
            mx, my, t = azar.randrange(0, lienzo[0] - 8), azar.randrange(0, lienzo[1] - 8), azar.randrange(1, 7)
            d.rectangle([mx, my, mx + t, my + t], fill=(255, 40, 40, azar.randrange(150, 256)))
        d.rectangle([20, 20, 140, 90], fill=(255, 255, 255, 7))  # alfa tenue, por debajo del umbral
        cv.save(os.path.join(destino, nombres[clave] + ".png"))
    import shutil
    shutil.copyfile(os.path.join(destino, "muslo_niño_derecho.png"), os.path.join(destino, "muslo_niño.png"))
    return nombres


def autoprueba_cuello():
    """cuello_de con una cabeza sintetica sin y con melena que cuelga mucho mas abajo de la barbilla. Devuelve los casos mal."""
    import preparar_expresion as PE
    cab_im, ovalo, eje, flequillo, barbilla = PE.cabeza_sintetica()   # 535x456, la barbilla en y = 448
    malos = 0
    avisos = []
    torso = SimpleNamespace(rect=[250, 77 + 430, 760, 800])
    corta = SimpleNamespace(norm=cab_im, rect=[224, 77, 224 + 535, 77 + 456])
    x, y = cuello_de(corta, torso, avisos)
    ok = abs(y - (77 + 456)) < 0.5 and abs(x - (224 + 535 / 2.0)) < 0.5
    malos += 0 if ok else 1
    print("  %-60s %s" % ("cabeza de pelo corto: el cuello en la base", "ok" if ok else "FALLA (%.1f, %.1f)" % (x, y)))
    larga = Image.new("RGBA", (535, 456 + 220), (0, 0, 0, 0))
    ImageDraw.Draw(larga).polygon([(40, 300), (495, 300), (470, 676), (70, 676)], fill=(30, 20, 15, 255))   # la melena cuelga 220 px bajo la barbilla
    larga.alpha_composite(cab_im, (0, 0))                                                                      # y la cara va delante
    pelo = SimpleNamespace(norm=larga, rect=[224, 77, 224 + 535, 77 + 676])
    x, y = cuello_de(pelo, torso, avisos)
    esperado_y = max(77 + barbilla, 77 + 430)   # la barbilla o el borde de arriba del torso, el que este mas abajo
    ok = abs(y - esperado_y) <= 4.0 and abs(x - (224 + eje)) <= 6.0
    malos += 0 if ok else 1
    print("  %-60s %s" % ("cabeza de melena: el cuello en el eje, a la barbilla/torso", "ok (%.0f, %.0f)" % (x, y) if ok else "FALLA (%.1f, %.1f; esperaba y = %.0f)" % (x, y, esperado_y)))
    return malos


def autoprueba_perfil_rechazado(frente):
    """
    INC-134: main() sale con 2 y manda a preparar_perfil.py si la ruta de la entrega o un archivo de ella dicen «perfil» (sin tildes ni mayusculas), y no si es
    de frente («frente» es la entrega sintetica ya hecha). Devuelve los casos mal.
    """
    import contextlib
    import io
    malos = 0

    def sale(argv):
        err = io.StringIO()
        try:
            with contextlib.redirect_stderr(err), contextlib.redirect_stdout(io.StringIO()):
                main(argv)
        except SystemExit as e:
            return e.code, err.getvalue()
        return 0, err.getvalue()

    with tempfile.TemporaryDirectory() as tmp:
        por_ruta = os.path.join(tmp, "Mam\u00e1", "Perfil")
        os.makedirs(por_ruta)
        shutil_copy_all(frente, por_ruta)
        por_nombre = os.path.join(tmp, "entrega")
        os.makedirs(por_nombre)
        shutil_copy_all(frente, por_nombre)
        os.rename(os.path.join(por_nombre, "torso_ni\u00f1o.png"), os.path.join(por_nombre, "torso_PERFIL_ni\u00f1o.png"))
        for nombre, carpeta in (("por la ruta (Mam\u00e1/Perfil)", por_ruta), ("por el nombre de un archivo (torso_PERFIL_...)", por_nombre)):
            codigo, msg = sale(["mama", carpeta, "--salida", os.path.join(tmp, "sal")])
            ok = codigo == 2 and "preparar_perfil.py" in msg
            malos += 0 if ok else 1
            print("%-60s %s" % ("una entrega de perfil se rechaza " + nombre, "bien" if ok else "FALLA (%s: %s)" % (codigo, msg.strip()[-120:])))
    codigo, msg = sale(["nino", frente, "--salida", os.path.join(tempfile.gettempdir(), "algoritmia_perfil_rechazo")])
    ok = codigo == 0 and "preparar_perfil.py" not in msg
    malos += 0 if ok else 1
    print("%-60s %s" % ("una entrega de frente sigue entrando", "bien" if ok else "FALLA (%s: %s)" % (codigo, msg.strip()[-120:])))
    return malos


def shutil_copy_all(origen, destino):
    import shutil
    for f in os.listdir(origen):
        shutil.copyfile(os.path.join(origen, f), os.path.join(destino, f))


def autoprueba():
    """Que la herramienta recupere, de una entrega sintetica hecha con el arte del Nino, sus rects y pivotes (<= 2 px)."""
    esperado = A.cargar_arte_final()["nino"]
    with tempfile.TemporaryDirectory() as tmp:
        entrega_sintetica(tmp)
        res = procesa(tmp, "nino")
        informe(res)
        if res.errores:
            print("\nAUTOPRUEBA: FALLA (la entrega sintetica no se proceso)")
            return 1
        print("\n== Comparacion con arte_final.json del Nino (error maximo %.0f px) ==" % TOLERANCIA_PX)
        malos = 0
        peor = 0.0

        def compara(que, a, b):
            nonlocal malos, peor
            e = max(abs(float(x) - float(y)) for x, y in zip(a, b))
            peor = max(peor, e)
            ok = e <= TOLERANCIA_PX
            malos += 0 if ok else 1
            print("  %-26s %-28s %-28s %4.1f %s" % (que, a, b, e, "ok" if ok else "FALLA"))

        for kind in ("nodos", "partes"):
            por_nombre = {n["nombre"]: n for n in res.tabla[kind]}
            for e in esperado[kind]:
                n = por_nombre.get(e["nombre"])
                if n is None:
                    print("  falta %s" % e["nombre"])
                    malos += 1
                    continue
                compara(e["nombre"] + " rect", n["rect"], e["rect"])
                clave = "punto" if kind == "nodos" else "pivote"
                if kind == "partes" and e["nombre"] in ("BrazoIzq", "BrazoDer"):
                    # el pivote del hombro de la tabla no es lo que mide esta herramienta (el centro del extremo redondo del humero): hombro.py lo lleva
                    # al borde del torso despues (aplica). Lo que se recupera es ese centro, que hombro.py mide del mismo arte
                    centro = H.piezas_de("nino", esperado)[e["nombre"][5:]].centro
                    compara(e["nombre"] + " pivote (centro redondo)", n[clave], [int(round(centro[0])), int(round(centro[1]))])
                    continue
                compara(e["nombre"] + " " + clave, n[clave], e[clave])
        # el SUCESO de cada aviso esperado: el duplicado y los nombres
        textos = " | ".join(res.avisos)
        for esperado_aviso, que in (("duplicado byte a byte", "el duplicado byte a byte"),
                                    ("nombres dicen el lado del PERSONAJE", "los nombres hablan del personaje")):
            ok = esperado_aviso in textos
            malos += 0 if ok else 1
            print("  %-60s %s" % (que + " se avisa", "ok" if ok else "NO SE AVISA"))
        motas = sum(pz.limpieza["motas"] for pz in res.piezas.values())
        ok = motas >= 10
        malos += 0 if ok else 1
        print("  %-60s %s" % ("las motas sueltas se quitan (%d)" % motas, "ok" if ok else "NO SE QUITAN"))
        # el registro lienzo-de-entrega -> lienzo-del-rig: la cabeza de la entrega cae en el rect de la tabla (la cara se coloca con el)
        cab_tabla = next(n for n in res.tabla["nodos"] if n["nombre"] == "Cuello")["rect"]
        ok = caja_a_lienzo(res.registro, res.registro["cabeza"]) == cab_tabla and res.registro["lienzo"] == [1300, 1500]
        malos += 0 if ok else 1
        print("  %-60s %s" % ("el registro lleva la cabeza de la entrega a su rect", "ok" if ok else "NO COINCIDE"))
        # los nombres con plural («pies_mama_izquierdo.png», 06/10/2026) y el lado por la posicion
        ok = clasifica("pies_mama_izquierdo.png")[0] == "antepierna" and clasifica("muslo_papa_dercho.png")[0] == "muslo"
        malos += 0 if ok else 1
        print("  %-60s %s" % ("se reconocen «pies_...» y los nombres con errata", "ok" if ok else "NO SE RECONOCEN"))
        # el cuello: con pelo corto (el Nino) la base de la cabeza; con melena o barba, la barbilla o el borde de arriba del torso
        malos += autoprueba_cuello()
        # INC-134: una entrega de PERFIL no entra por aqui (sustituiria Frontal/): por la ruta, por el nombre de un archivo, y la de frente sigue entrando
        malos += autoprueba_perfil_rechazado(tmp)
        print("\nAUTOPRUEBA: %s (error maximo %.1f px)" % ("pasa" if not malos else "FALLA en %d comprobaciones" % malos, peor))
        return 1 if malos else 0


# ============================================================================ 5. principal


def main(argv=None):
    ap = argparse.ArgumentParser(description="De las piezas del arte final a las medidas del rig (ver la cabecera del script).")
    ap.add_argument("id", nargs="?", choices=sorted(P.FAMILIA))
    ap.add_argument("entrega", nargs="?", help="carpeta con los PNG de la entrega")
    ap.add_argument("--aplicar", action="store_true", help="escribe los PNG, arte_final.json y regenera las tablas y los clips")
    ap.add_argument("--cadera", choices=("borde", "capsula"), default="borde")
    ap.add_argument("--salida", default=os.path.join(tempfile.gettempdir(), "algoritmia_arte_final"),
                    help="donde dejar el composite del informe")
    ap.add_argument("--autoprueba", action="store_true", help="comprueba la herramienta con el arte final del Nino")
    ap.add_argument("--fabrica-entrega", metavar="CARPETA",
                    help="escribe en CARPETA la entrega sintetica de la autoprueba (para probar el informe a mano)")
    a = ap.parse_args(argv)
    if a.fabrica_entrega:
        entrega_sintetica(a.fabrica_entrega)
        print("entrega sintetica en", a.fabrica_entrega)
        return 0
    if a.autoprueba:
        return autoprueba()
    if not a.id or not a.entrega:
        ap.error("hace falta <id> y <carpeta_entrega> (o --autoprueba)")
    if not os.path.isdir(a.entrega):
        ap.error("no existe la carpeta %s" % a.entrega)
    if es_entrega_de_perfil(a.entrega):
        # una entrega de perfil (INC-134) pasaria por esta herramienta como si fuera de frente y --aplicar SUSTITUIRIA las diez partes de Frontal/
        ap.error("entrega de perfil: usar preparar_perfil.py")
    os.makedirs(a.salida, exist_ok=True)
    res = procesa(a.entrega, a.id, a.cadera)
    informe(res)
    if res.errores:
        return 2
    ruta = os.path.join(a.salida, "arte_final_%s_informe.png" % a.id)
    composite(res.piezas, res.joints, ruta, a.id)
    print("\ncomposite: %s" % ruta)
    if not a.aplicar:
        print("\nescribiria (con --aplicar):")
        for r, existia in lista_escritura(res):
            print("  %s %s" % ("sustituye" if existia else "nuevo    ", os.path.relpath(r, P.RAIZ)))
        print("  claudeDocs/tasks/Personajes/herramientas/arte_final.json (entrada de %s)" % a.id)
        print("  y regenera rig_articulaciones.json y clips_personajes.json")
        return 0
    return 1 if aplica(res, a) else 0


if __name__ == "__main__":
    sys.exit(main())
