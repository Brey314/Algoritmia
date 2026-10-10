#!/usr/bin/env python3
# preparar_algoritm.py: del arte final de Algoritm (INC-136) a las medidas del rig, con un comando.
#
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_algoritm.py <carpeta_algoritm>               # informe y composite
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_algoritm.py <carpeta_algoritm> --aplicar     # escribe las partes, la cara provisional y las tablas
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_algoritm.py --reposo                         # rehace en su sitio los tres char_algoritm_n*_reposo.png
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_algoritm.py --autoprueba                     # se prueba solo
#         <carpeta_algoritm> = la de la entrega, con Fuego/, Rueda/ y Gota/ (o madera / agua; sin tildes ni mayusculas), p. ej.
#                              claudeDocs/tasks/Personajes/entregas/2026-10-09/Algoritm
#         --salida <carpeta>   donde dejar el composite del informe (por defecto, el tmp)
#         --rehaz-cara         extrae de nuevo la cara provisional del _reposo del repo, aunque ya este escrita (ver CARA PROVISIONAL)
#         --cintura Y          el y (lienzo de la entrega) del pivote de Tronco, en vez de CINTURA_SOBRE_BASE px sobre la base del torso
#
# DECISIONES DE SANTIAGO (09/10/2026, INC-136), las que fijan esta herramienta:
#   1. El diseno NUEVO de la entrega (llama, disco de madera y gota, con pantaloneta de cuadros, extremidades del color del material y contorno negro) es el
#      oficial: sustituye al provisional de nueve piezas que se recorto del _reposo (bd8b802, pose_preview.py --exporta-maqueta).
#   2. SIETE piezas por forma, todas del mismo lienzo de 1300x1500 registrado: torso_*, brazo_{derecho,izquierdo}_*, mano_{derecho,izquierdo}_* (antebrazo con
#      la mano) y pie_* o pierna_* (la gota las llama pierna). «derecho» y «derecha» son la DERECHA DE LA PANTALLA, que es BrazoDer del rig: no se espeja nada.
#   3. La pierna llega ENTERA (palo y pie), sin muslo ni rodilla: va en PiernaX y AntepiernaX no lleva sprite. RodillaX sigue existiendo (los clips la
#      animan) en el punto medio del palo visible: girarla gira un nodo vacio.
#   4. Escala «opcion A»: una sola transformacion para las tres formas, sacada del Fuego (la forma mas alta), que deja la figura con el alto que tiene HOY
#      en pantalla (ALTO_HOY = 982 px del lienzo de 1024, de y = 21 a 1003) y el eje del cuerpo en x = 512. Asi ningun Size de las 16 narrativas cambia.
#   5. La entrega no trae cara. Mientras llega la del artista se usa una cara PROVISIONAL sacada del sprite de hoy (ver CARA PROVISIONAL). No parpadea.
#
# QUE HACE (todo en el lienzo de 1024, origen arriba a la izquierda, y hacia abajo; las medidas se toman en el lienzo de la entrega, sobre el alfa, y se llevan
# al de 1024 con la transformacion unica):
#   1. Reconoce las piezas POR NOMBRE, con tolerancia (sin tildes ni mayusculas): torso/cuerpo; brazo/humero; mano/antebrazo; pie/pies/pierna. Se ignoran los
#      nombres de la forma (algoritm, fuego, rueda, gota). El lado de cada pieza lo decide su POSICION; si el nombre lo contradice, avisa. Exige los TRES
#      directorios de forma, siete piezas en cada uno y 1300x1500 en todas (los PNG de un lienzo distinto, o una pieza de la familia —cabeza, muslo,
#      antepierna— o una entrega de perfil, se rechazan). Limpia el alfa (< 12 a cero, solo la componente conexa mayor de cada pieza).
#   2. Comprueba que las EXTREMIDADES de las tres formas son la misma silueta (XOR del alfa ~ 0): los clips y coreografia.py suponen una sola geometria.
#   3. Normaliza con la transformacion unica y recorta cada pieza a su contenido (LANCZOS; la memoria de textura es la de lo que se ve).
#   4. MIDE: hombro = centro del tapon de arriba del humero; codo = punto medio entre el tapon de abajo del humero y el de arriba del antebrazo (la holgura
#      entre los dos queda en «holguras»); cadera (PiernaX.punto) = centro del tapon de arriba de la pierna; rodilla = punto medio del palo visible (entre la base
#      del torso y donde empieza el pie); Tronco (la cintura) = el eje del cuerpo a CINTURA_SOBRE_BASE px sobre la base del torso. No se aplica hombro.py: el
#      hombro de Algoritm es el centro del tapon, no el borde del torso. Con el diseno de la entrega el humero iba DELANTE del cuerpo y el tapon se veia sobre
#      el; desde INC-147 (Santiago, 09/10/2026) los dos brazos van DETRAS DE TODO EL CUERPO y el tapon queda oculto tras el torso: solo asoma lo que sobresale de su silueta.
#   5. Sin --aplicar: informe y un composite (las tres formas en reposo con sus articulaciones). Con --aplicar: escribe en Assets/Game/Art/Characters/Algoritm/
#      Frontal/ las 21 partes char_algoritm_<forma>_parte_{torso,brazo_izq,brazo_der,antebrazo_izq,antebrazo_der,pierna_izq,pierna_der}.png (los que ya existen
#      se sustituyen DONDE ESTAN: mismo .meta, mismo GUID), BORRA con su .meta las seis char_algoritm_<forma>_parte_antepierna_{izq,der}.png (la pierna es entera),
#      escribe la cara provisional en Expresiones/, guarda en arte_final.json las tres entradas «algoritm_<forma>» (nodos, registro, holguras y cara_provisional;
#      Ojos, Boca y orden_tronco se conservan: personaje_guia() de articulaciones.py los vuelca tal cual), regenera rig_articulaciones.json, regenera
#      clips_personajes.json y corre coreografia.py --valida. NUNCA escribe .meta.
#   6. --reposo reescribe EN SU SITIO (mismo nombre y mismo GUID) los tres char_algoritm_n1_fuego_reposo.png, n2_rueda y n3_gota (768x768) con el diseno nuevo
#      ensamblado en reposo y la cara (la final compartida desde INC-148; antes, la provisional), al mismo encuadre que tenian (el lienzo de 1024 a 0,75). Los referencian 3 prefabs (el retrato del guia), 5
#      escenas y 16 assets narrativos, asi que el nombre y el GUID no pueden cambiar. Se corre DESPUES de --aplicar.
#
# CARA FINAL (INC-148, 10/10/2026). La entrega del 10/10 SI trae la cara de Algoritm: una sola serie de seis expresiones para las tres formas. La escribe
# preparar_expresion.py --entrega <Algoritm/Expresiones> (Algoritm/Expresiones/char_algoritm_{ojos_*,boca_*}.png, sin el nombre de la forma; los nodos Ojos y Boca de las tres
# entradas con el mismo rect; «cara_provisional»: false y «prefijo_cara»: «char_algoritm» en arte_final.json). DESDE ENTONCES esta herramienta NUNCA vuelve a la provisional: una entrada
# con cara_provisional false conserva sus nodos Ojos y Boca (nombres compartidos y rect), registro.cara (la caja de la cara, lista de cuatro enteros), prefijo_cara y retrato; no escribe
# los PNG provisionales por forma; --rehaz-cara se niega; y --reposo ensambla el _reposo con la cara FINAL (la neutra: ojos_neutra y boca_0 compartidas). Los PNG provisionales
# char_algoritm_<forma>_ojos_neutra y _boca_0 se quedan en disco hasta que el Editor los borre con su .meta; ninguna tabla los nombra ya. Lo que sigue (CARA PROVISIONAL) describe
# lo que paso antes, y sigue valiendo si arte_final.json no trae la marca de la cara final.
#
# CARA PROVISIONAL. La entrega no trae ojos ni boca, y un guia sin cara es un prop (DA §7.6, condicion 2). Se saca la cara del _reposo de HOY de cada forma
# (el de bd8b802, 768x768): los OJOS son los dos aros de contorno cafe con todo lo que encierran (la esclera, el iris), y la BOCA es la sonrisa (el trazo
# oscuro de la franja de la boca); se buscan por color y posicion alrededor de donde estan Ojos y Boca del rig de hoy (ZONA_OJOS y ZONA_BOCA, del lienzo de
# 1024). Se escriben como char_algoritm_<forma>_ojos_neutra.png y char_algoritm_<forma>_boca_0.png en Algoritm/Expresiones/ (BuildRigsFinal ExpectedSubfolder
# manda «_ojos_» y «_boca_» ahi), a la resolucion del sprite (sin remuestrear, que es la de hoy). Se colocan sobre el torso nuevo CENTRADOS EN EL CUERPO y a la
# altura relativa a la que estaban: los ojos OJOS_SOBRE_ANCHA px sobre la fila mas ancha del cuerpo (sin la pantaloneta) y la boca BOCA_DESDE_OJOS bajo
# ellos, lo que en la llama, en el disco y en la gota es el vientre redondo. Llevan contorno cafe y otra mano: se ven de otro dibujo, por eso es provisional.
# arte_final.json lo marca con «cara_provisional»: true en cada entrada. SIN cuadro de ojos cerrados, Algoritm no parpadea todavia (CharacterFaceSet cae al
# reposo). Cuando el artista entregue ojos_neutra, ojos_parpadeo_cerrado y boca_0 en el lienzo de 1300x1500, se sustituyen estos PNG y se quita la marca.
# --aplicar CONSERVA la cara si ya esta escrita (el _reposo del repo deja de ser el de hoy cuando se corre --reposo); --rehaz-cara la extrae de nuevo.
#
# LO QUE SE LLEVA EL EDITOR (sesion local): estado → perfil → sprites → clips → estado. «nodos» no hace falta (todos los nodos existen: solo se recolocan) y
# «orden» SI hace falta desde INC-147 (09/10/2026): orden_tronco pasa de Torso, Ojos, Boca, BrazoIzq, BrazoDer a BrazoIzq, BrazoDer, Torso, Ojos, Boca (los brazos detras de
# todo el cuerpo; articulaciones.ORDEN_TRONCO_GUIA), y --reposo ensambla los tres _reposo con ese mismo orden. «sprites» asigna las siete piezas, apaga y VACIA el sprite de AntepiernaX
# (el PNG se borra: sin sprite ni Image encendida), enciende Ojos y Boca con la cara provisional y crea char_algoritm_<forma>_cara.asset (CharacterFaceSet con
# EyesNeutral y MouthClosed) en Algoritm/Expresiones/, que cablea en CharacterFace.faceSet.

import argparse
import hashlib
import math
import os
import random
import re
import sys
import tempfile
from types import SimpleNamespace

try:
    from PIL import Image, ImageChops, ImageDraw, ImageFilter
except ImportError:  # pragma: no cover
    sys.exit("hace falta Pillow: pip install pillow")

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import articulaciones as A  # noqa: E402
import prefabs as P  # noqa: E402
import preparar_arte_final as F  # noqa: E402
import pose_preview as V  # noqa: E402

LIENZO_ENTREGA = (1300, 1500)    # el lienzo comun registrado de la entrega, el mismo para las siete piezas y para las tres formas
LIENZO = 1024                    # el del rig
UMBRAL_ALFA = F.UMBRAL_ALFA      # alfa por debajo de esto, a cero
# Lo que mide HOY la figura en pantalla (el _reposo de bd8b802, que --reposo reescribe): de y = 21 (la punta de la llama) a y = 1003 (los pies), 982 px del
# lienzo de 1024, con el eje del cuerpo en x = 512. Son constantes y no se miden del _reposo porque ese archivo cambia cuando se corre --reposo.
ALTO_HOY = 982.0
CORONILLA_HOY = 21.0
EJE_X = 512.0
FRACCION_EJE = (0.45, 0.70)      # el eje del cuerpo: el centro de las filas del torso entre estas fracciones del alto de la FIGURA (la forma de la llama sesga el recuadro)
CINTURA_SOBRE_BASE = 157         # px del lienzo de la entrega entre la base del torso (el borde de abajo de la pantaloneta) y el centro de la cinturilla
PANTALONETA_ALTO = 207           # px de la entrega que ocupa la pantaloneta bajo el vientre: el cuerpo (donde va la cara) es lo que hay encima
CAPSULA_MIN = 0.40               # el radio del tapon de arriba de la pierna: la mitad del ancho mayor del primer 40 % del palo
XOR_MAX = 0.002                  # las extremidades de dos formas difieren en menos de esta fraccion de sus pixeles opacos (y de 30 px) o se avisa
FORMAS = [("fuego", "Fuego", "n1_fuego"), ("rueda", "Rueda", "n2_rueda"), ("gota", "Gota", "n3_gota")]   # clave, carpeta de la entrega, parte del nombre del _reposo
ALIAS = {"fuego": {"fuego", "llama", "fire"}, "rueda": {"rueda", "madera", "wheel"}, "gota": {"gota", "agua", "drop", "water"}}
CARPETA_ARTE = os.path.join(P.PERSONAJES_ARTE, "Algoritm")
FRONTAL = os.path.join(CARPETA_ARTE, "Frontal")
EXPRESIONES = os.path.join(CARPETA_ARTE, "Expresiones")

ROLES = {
    "torso": {"torso", "tronco", "cuerpo", "pecho", "body", "chest"},
    "brazo": {"brazo", "humero", "arm", "upperarm"},
    "antebrazo": {"antebrazo", "mano", "manos", "forearm", "hand"},
    "pierna": {"pierna", "piernas", "pie", "pies", "leg", "legs", "foot", "feet"},
}
PARTIDAS = {"muslo", "antepierna", "canilla", "pantorrilla", "cabeza", "head", "thigh", "shin"}   # piezas de la FAMILIA: Algoritm no tiene cabeza y su pierna es entera
CARA = {"ojos", "ojo", "boca", "eyes", "mouth", "cara", "face"}
DOS_LADOS = ("brazo", "antebrazo", "pierna")
LADOS = ("Izq", "Der")
NOMBRE_C4 = {("torso", None): "parte_torso",
             ("brazo", "Izq"): "parte_brazo_izq", ("brazo", "Der"): "parte_brazo_der",
             ("antebrazo", "Izq"): "parte_antebrazo_izq", ("antebrazo", "Der"): "parte_antebrazo_der",
             ("pierna", "Izq"): "parte_pierna_izq", ("pierna", "Der"): "parte_pierna_der"}
T, C = P.T, P.C

# Donde estan Ojos y Boca del rig de HOY, en el lienzo de 1024 (rig_articulaciones.json antes de INC-136, ALG["ojos"] y ALG["boca"]): de donde se saca la cara
# provisional del _reposo (768x768, o sea 0,75 del lienzo). Son constantes: el JSON cambia con --aplicar.
ZONA_OJOS = (370, 380, 655, 505)
ZONA_BOCA = (410, 505, 610, 595)
SPRITE = 768
K_SPRITE = SPRITE / float(LIENZO)
CINTURA_HOY = 600                # y (1024) del borde de arriba de la franja de colores del sprite de hoy: el cuerpo, sin la pantaloneta, es lo que hay encima
UMBRAL_OSCURO = 110              # luminancia por debajo de la cual un pixel es contorno cafe, pupila o trazo de la sonrisa


# ============================================================================ 1. leer la entrega


def clasifica(archivo):
    """(rol, lado segun el nombre o None) de un nombre de archivo; rol «cara» o «partida» si no es una pieza del cuerpo de Algoritm; None si no se reconoce."""
    base = F.normaliza(os.path.splitext(archivo)[0])
    palabras = re.findall(r"[a-z]+", base)
    if any(p in CARA for p in palabras):
        return "cara", None
    if any(p in PARTIDAS for p in palabras):
        return "partida", None
    rol = next((r for r, nombres in ROLES.items() if any(p in nombres for p in palabras)), None)
    lado = next((l for l, nombres in F.LADOS.items() if any(p in nombres for p in palabras)), None)
    return rol, lado


def lee_forma(carpeta, clave):
    """
    Las siete piezas de UNA forma: SimpleNamespace(clave, carpeta, piezas {(rol, lado): F.Pieza}, avisos, errores). El lado lo decide la posicion (la convencion del
    rig: Izq es la izquierda de la PANTALLA, que es lo que dicen los nombres de esta entrega). Una pieza que falta, de mas o de otro lienzo es un error.
    """
    forma = SimpleNamespace(clave=clave, carpeta=carpeta, piezas={}, avisos=[], errores=[])
    archivos = sorted(f for f in os.listdir(carpeta) if f.lower().endswith(".png"))
    por_hash = {}
    for f in archivos:
        with open(os.path.join(carpeta, f), "rb") as fh:
            por_hash.setdefault(hashlib.sha1(fh.read()).hexdigest(), []).append(f)
    unicos = []
    for grupo in por_hash.values():
        grupo.sort(key=lambda f: (clasifica(f)[1] is None, f))
        unicos.append(grupo[0])
        for d in grupo[1:]:
            forma.avisos.append("%s es un duplicado byte a byte de %s: se descarta" % (d, grupo[0]))
    candidatas = []
    for f in sorted(unicos):
        rol, lado = clasifica(f)
        if rol == "cara":
            forma.avisos.append("%s es una capa de la cara: no se procesa aqui (la cara de la entrega, cuando llegue, entra por preparar_expresion.py)" % f)
        elif rol == "partida":
            forma.errores.append("%s es una pieza de la FAMILIA o de una pierna partida (cabeza, muslo, antepierna): Algoritm tiene la pierna ENTERA y no tiene cabeza (INC-136)" % f)
        elif rol is None:
            forma.avisos.append("%s: no reconozco la pieza por el nombre (torso, brazo, mano/antebrazo, pie/pierna): se ignora" % f)
        else:
            candidatas.append(F.Pieza(f, rol, lado))
    tamanos = {}
    imagenes = {}
    for pz in candidatas:
        im = Image.open(os.path.join(carpeta, pz.archivo)).convert("RGBA")
        imagenes[pz.archivo] = im
        tamanos.setdefault(im.size, []).append(pz.archivo)
    for (w, h), fs in tamanos.items():
        if (w, h) != LIENZO_ENTREGA:
            forma.errores.append("%s no mide %dx%d sino %dx%d: las siete piezas son el lienzo ENTERO con solo su pieza opaca, todas del mismo tamano"
                                 % (", ".join(fs), LIENZO_ENTREGA[0], LIENZO_ENTREGA[1], w, h))
    if forma.errores:
        return forma
    for pz in candidatas:
        pz.imagen, pz.bbox, pz.limpieza = F.limpia(imagenes[pz.archivo])
        if pz.imagen is None:
            forma.errores.append("%s esta vacia (todo su alfa es menor de %d)" % (pz.archivo, UMBRAL_ALFA))
        elif pz.limpieza["mayor_descartado"] > 0.01 * sum(pz.imagen.getchannel("A").point(lambda v: 1 if v else 0).histogram()[1:]):
            forma.avisos.append("%s: se descarta un trozo suelto de %d px que no conecta con la pieza" % (pz.archivo, pz.limpieza["mayor_descartado"]))
    candidatas = [pz for pz in candidatas if pz.imagen is not None]
    esperadas = {"torso": 1, "brazo": 2, "antebrazo": 2, "pierna": 2}
    for rol, n in esperadas.items():
        grupo = [pz for pz in candidatas if pz.rol == rol]
        if len(grupo) != n:
            forma.errores.append("falta o sobra %s en %s: hace falta %d y hay %d%s" % (
                rol, os.path.basename(carpeta), n, len(grupo), " (" + ", ".join(g.archivo for g in grupo) + ")" if grupo else ""))
            continue
        if rol in DOS_LADOS:
            grupo.sort(key=lambda pz: (pz.bbox[0] + pz.bbox[2]) / 2.0)
            grupo[0].lado, grupo[1].lado = "Izq", "Der"
        for pz in grupo:
            forma.piezas[(rol, pz.lado)] = pz
    for pz in forma.piezas.values():
        if pz.lado_nombre and pz.lado and pz.lado_nombre != pz.lado:
            forma.avisos.append("%s: el nombre dice %s pero la pieza esta a la %s de la pantalla; se usa la posicion (Izq = izquierda de la pantalla). Revisa que no este espejada"
                                % (pz.archivo, "derecho" if pz.lado_nombre == "Der" else "izquierdo", "derecha" if pz.lado == "Der" else "izquierda"))
    return forma


def pliega_palabras(texto):
    return set(re.findall(r"[a-z]+", F.normaliza(texto)))


def es_de_otro(carpeta):
    """El motivo (texto) por el que la carpeta es de la familia o de un perfil y no de Algoritm; None si no lo parece."""
    if F.es_entrega_de_perfil(carpeta):
        return "es una entrega de PERFIL (Algoritm no tiene perfil): usar preparar_perfil.py con un miembro de la familia"
    familia = {"papa", "mama", "nina", "nino", "frente", "frontal", "expresiones"}
    partes_familia = {"cabeza", "muslo", "antepierna"}
    vistas = pliega_palabras(os.path.basename(os.path.normpath(carpeta)))
    for dentro in os.listdir(carpeta):
        vistas |= pliega_palabras(os.path.splitext(dentro)[0])
        if os.path.isdir(os.path.join(carpeta, dentro)):
            for n in os.listdir(os.path.join(carpeta, dentro)):
                vistas |= pliega_palabras(os.path.splitext(n)[0])
    if vistas & familia or vistas & partes_familia:
        return "es de un miembro de la FAMILIA (%s): usar preparar_arte_final.py" % ", ".join(sorted((vistas & familia) | (vistas & partes_familia)))
    return None


def lee_entrega(carpeta):
    """
    (formas {clave: forma}, avisos, errores): lee las tres carpetas de forma de la entrega, y compara las extremidades de las tres (XOR del alfa). Sin los tres
    directorios, o con una forma con errores, errores no vacio (y nada se escribe).
    """
    avisos, errores = [], []
    sub = [d for d in sorted(os.listdir(carpeta)) if os.path.isdir(os.path.join(carpeta, d))]
    por_forma = {}
    for d in sub:
        palabras = pliega_palabras(d)
        for clave, alias in ALIAS.items():
            if palabras & alias:
                por_forma.setdefault(clave, d)
    formas = {}
    for clave, nombre, _ in FORMAS:
        if clave not in por_forma:
            errores.append("falta la carpeta de la forma %s (%s/): la entrega son tres carpetas, con las siete piezas en cada una" % (clave, nombre))
            continue
        formas[clave] = lee_forma(os.path.join(carpeta, por_forma[clave]), clave)
        avisos += ["%s: %s" % (nombre, a) for a in formas[clave].avisos]
        errores += ["%s: %s" % (nombre, e) for e in formas[clave].errores]
    if not errores:
        avisos += compara_extremidades(formas)
    return formas, avisos, errores


def _mascara_union(pz, caja):
    """La pieza (alfa >= 128) en una mascara L del tamano de «caja», puesta donde esta en el lienzo."""
    m = Image.new("L", (caja[2] - caja[0], caja[3] - caja[1]), 0)
    m.paste(pz.imagen.getchannel("A").point(lambda v: 255 if v >= 128 else 0), (pz.bbox[0] - caja[0], pz.bbox[1] - caja[1]))
    return m


def compara_extremidades(formas):
    """Avisos: las extremidades (brazos, antebrazos y piernas) de cada forma contra las del Fuego. Los clips suponen una sola geometria para las tres."""
    avisos = []
    base = formas["fuego"]
    for clave in ("rueda", "gota"):
        for k in sorted(base.piezas, key=lambda k: (k[0], k[1] or "")):
            if k[0] == "torso":
                continue
            a, b = base.piezas[k], formas[clave].piezas[k]
            caja = (min(a.bbox[0], b.bbox[0]), min(a.bbox[1], b.bbox[1]), max(a.bbox[2], b.bbox[2]), max(a.bbox[3], b.bbox[3]))
            dif = ImageChops.difference(_mascara_union(a, caja), _mascara_union(b, caja)).histogram()[255]
            opacos = max(1, _mascara_union(a, caja).histogram()[255])
            if dif > max(30, XOR_MAX * opacos):
                avisos.append("las extremidades no coinciden entre formas: %s de %s difiere %d px (%.2f %%) de la del fuego; los clips de Algoritm y coreografia.py suponen UNA geometria "
                              "para las tres (se mide la del fuego)" % (b.archivo, clave, dif, 100.0 * dif / opacos))
    return avisos


# ============================================================================ 2. la transformacion unica y la normalizacion


def eje_cuerpo(torso, ymin, ymax):
    """El x (lienzo de la entrega) del eje del cuerpo: la media de los centros de las filas del torso entre FRACCION_EJE del alto de la figura (ymin..ymax)."""
    m = torso.imagen.getchannel("A").point(lambda v: 255 if v >= 128 else 0)
    w, h = m.size
    y0, y1 = ymin + FRACCION_EJE[0] * (ymax - ymin), ymin + FRACCION_EJE[1] * (ymax - ymin)
    centros = []
    for y in range(int(y0), int(y1)):
        yy = y - torso.bbox[1]
        if 0 <= yy < h:
            caja = m.crop((0, yy, w, yy + 1)).getbbox()
            if caja:
                centros.append((caja[0] + caja[2] - 1) / 2.0 + torso.bbox[0])
    if not centros:
        return (torso.bbox[0] + torso.bbox[2]) / 2.0
    return sum(centros) / len(centros)


def transformacion(fuego):
    """
    (escala, tx, ty, eje) de la opcion A, sacada del Fuego: x' = escala * x + tx, y' = escala * y + ty. Deja la figura (de la punta de la llama a los pies) con
    ALTO_HOY px, la punta en CORONILLA_HOY y el eje del cuerpo en EJE_X.
    """
    ps = list(fuego.piezas.values())
    ymin, ymax = min(p.bbox[1] for p in ps), max(p.bbox[3] for p in ps)
    s = ALTO_HOY / float(ymax - ymin)
    eje = eje_cuerpo(fuego.piezas[("torso", None)], ymin, ymax)
    return s, EJE_X - s * eje, CORONILLA_HOY - s * ymin, eje


def normaliza(forma, tr):
    """Cada pieza recortada y escalada con la transformacion unica: pz.norm (imagen) y pz.rect ([x0, y0, x1, y1] en el lienzo de 1024)."""
    s, tx, ty = tr[0], tr[1], tr[2]
    for pz in forma.piezas.values():
        w, h = pz.imagen.size
        nw, nh = max(1, int(round(w * s))), max(1, int(round(h * s)))
        pz.norm = pz.imagen.resize((nw, nh), Image.LANCZOS)
        x0, y0 = int(round(s * pz.bbox[0] + tx)), int(round(s * pz.bbox[1] + ty))
        pz.rect = [x0, y0, x0 + nw, y0 + nh]


# ============================================================================ 3. medir


def dist(p, q):
    return math.hypot(p[0] - q[0], p[1] - q[1])


def puntos_pieza(pz, paso=2):
    """Pixeles opacos (alfa >= 128) de una pieza, cada «paso», en el lienzo de la entrega."""
    a = pz.imagen.getchannel("A")
    w, h = a.size
    d = a.load()
    return [(x + pz.bbox[0], y + pz.bbox[1]) for y in range(0, h, paso) for x in range(0, w, paso) if d[x, y] >= 128]


def tapones_brazo(brazo, antebrazo):
    """
    (hombro, codo del humero, radio) en el lienzo de la entrega: los centros de los dos extremos redondos del humero (pose_preview.extremos_capsula sobre el
    alfa), el del hombro es el que queda LEJOS del antebrazo y el del codo el que queda cerca.
    """
    a, b, r = V.extremos_capsula(brazo.imagen)
    a, b = (a[0] + brazo.bbox[0], a[1] + brazo.bbox[1]), (b[0] + brazo.bbox[0], b[1] + brazo.bbox[1])
    pts = puntos_pieza(antebrazo, 4)
    cen = (sum(p[0] for p in pts) / len(pts), sum(p[1] for p in pts) / len(pts))
    codo, hombro = (a, b) if dist(a, cen) < dist(b, cen) else (b, a)
    return hombro, codo, r


def tapon_antebrazo(antebrazo, codo_h, hombro):
    """
    (centro, radio) del tapon del antebrazo que se mete en el codo del humero. El antebrazo sigue la direccion del humero (brazos casi rectos): el extremo es su punto
    mas alto sobre ese eje, el radio es la mitad del ancho de la pieza a un radio de ese extremo (se afina en seis pasos) y el centro esta a un radio del extremo.
    """
    ux, uy = codo_h[0] - hombro[0], codo_h[1] - hombro[1]
    largo = math.hypot(ux, uy)
    ux, uy = ux / largo, uy / largo
    nx, ny = -uy, ux
    pr = [((p[0] - codo_h[0]) * ux + (p[1] - codo_h[1]) * uy, (p[0] - codo_h[0]) * nx + (p[1] - codo_h[1]) * ny) for p in puntos_pieza(antebrazo, 2)]
    punta = min(q[0] for q in pr)
    r, c = 35.0, 0.0
    for _ in range(6):
        fila = [q[1] for q in pr if abs(q[0] - (punta + r)) <= 1.5]
        if not fila:
            break
        r, c = max(4.0, (max(fila) - min(fila)) / 2.0), (max(fila) + min(fila)) / 2.0
    eje, lado = punta + r, c
    return (codo_h[0] + ux * eje + nx * lado, codo_h[1] + uy * eje + ny * lado), r


def anchos_filas(pz):
    """[(x0, x1) o None] del tramo opaco (alfa >= 128) de cada fila de la pieza recortada (x en el lienzo de la entrega, exclusivo en x1)."""
    m = pz.imagen.getchannel("A").point(lambda v: 255 if v >= 128 else 0)
    w, h = m.size
    out = []
    for y in range(h):
        caja = m.crop((0, y, w, y + 1)).getbbox()
        out.append((caja[0] + pz.bbox[0], caja[2] + pz.bbox[0]) if caja else None)
    return out


def mide_pierna(pierna, base_torso):
    """
    En el lienzo de la entrega: {cadera, rodilla, radio, y_pie}. La pierna es un palo con el extremo de arriba redondo (se mete bajo la pantaloneta) y un pie ovalado: el palo es
    el ancho mediano de las filas del segundo cuarto de la pieza; el pie empieza donde una fila pasa de 1,5 veces ese ancho. Cadera = centro del tapon de arriba (a un radio del
    borde, con el radio igual a la mitad del ancho mayor del primer CAPSULA_MIN del palo); rodilla = el punto medio del palo visible, entre la base del torso y el pie.
    """
    filas = anchos_filas(pierna)
    h = len(filas)
    ancho = [(f[1] - f[0]) if f else 0 for f in filas]
    y0 = pierna.bbox[1]
    palo = sorted(ancho[int(0.25 * h):int(0.50 * h) + 1])
    palo = palo[len(palo) // 2] if palo else max(ancho)
    pie = next((y for y in range(int(0.25 * h), h) if ancho[y] > 1.5 * palo), h)
    r = 0.5 * max(ancho[:max(1, int(CAPSULA_MIN * pie))])
    fila_cadera = filas[min(h - 1, int(round(r)))]
    cadera = ((fila_cadera[0] + fila_cadera[1]) / 2.0 if fila_cadera else (pierna.bbox[0] + pierna.bbox[2]) / 2.0, y0 + r)
    y_pie = y0 + pie
    y_rod = 0.5 * (max(base_torso, cadera[1] + 0.25 * (y_pie - cadera[1])) + y_pie)
    fila_rod = filas[max(0, min(h - 1, int(round(y_rod - y0))))] or (pierna.bbox[0], pierna.bbox[2])
    return {"cadera": cadera, "rodilla": ((fila_rod[0] + fila_rod[1]) / 2.0, y_rod), "radio": r, "y_pie": y_pie}


def mide(forma, eje, cintura=None):
    """Las articulaciones del Fuego (las extremidades de las tres formas son la misma silueta), en el lienzo de la entrega: {nombre: (x, y)} y las holguras de los codos."""
    pz = forma.piezas
    j, holguras = {}, {}
    base_torso = float(pz[("torso", None)].bbox[3])
    for lado in LADOS:
        hombro, codo_h, r = tapones_brazo(pz[("brazo", lado)], pz[("antebrazo", lado)])
        cf, _ = tapon_antebrazo(pz[("antebrazo", lado)], codo_h, hombro)
        j["Hombro" + lado] = hombro
        j["Codo" + lado] = ((codo_h[0] + cf[0]) / 2.0, (codo_h[1] + cf[1]) / 2.0)
        holguras["Codo" + lado] = round(dist(codo_h, cf), 1)
        m = mide_pierna(pz[("pierna", lado)], base_torso)
        j["Cadera" + lado], j["Rodilla" + lado] = m["cadera"], m["rodilla"]
    j["Tronco"] = (eje, float(cintura) if cintura is not None else base_torso - CINTURA_SOBRE_BASE)
    return j, holguras


def a_rig(tr, p):
    """Un punto del lienzo de la entrega al lienzo de 1024, redondeado: [x, y]."""
    return [int(round(tr[0] * p[0] + tr[1])), int(round(tr[0] * p[1] + tr[2]))]


# ============================================================================ 4. la cara provisional


def luminancia(im):
    r, g, b = im.convert("RGB").split()
    return ImageChops.add(ImageChops.add(r.point(lambda v: v // 3), g.point(lambda v: v // 3)), b.point(lambda v: v // 3))


def rellena(m):
    """La mascara L (0 / 255) con sus huecos cerrados rellenos (lo que encierra un aro)."""
    f = m.copy()
    w, h = f.size
    for sx, sy in ((0, 0), (w - 1, 0), (0, h - 1), (w - 1, h - 1)):
        if f.getpixel((sx, sy)) == 0:
            ImageDraw.floodfill(f, (sx, sy), 128)
    return f.point(lambda v: 0 if v == 128 else 255)


def moda(valores):
    cuenta = {}
    for v in valores:
        cuenta[v] = cuenta.get(v, 0) + 1
    return max(cuenta, key=cuenta.get)


def capa_de_trazo(imagen, relleno, trazo):
    """
    La capa RGBA de una forma cuyo contorno es un trazo oscuro: «relleno» es la mascara (0/255) de la forma entera (el trazo y lo que encierra) y «trazo» la de sus pixeles
    oscuros. Dentro de la forma, el color original; en el anillo de 2 px de fuera, el alfa sale de cuanto oscurece el borde respecto del fondo (el borde suavizado del trazo) y
    el color es el del trazo, para que no quede un halo del color del fondo. Recorta a lo que se pinta. None si no hay nada.
    """
    lum = luminancia(imagen)
    anillo = ImageChops.subtract(relleno.filter(ImageFilter.MaxFilter(5)), relleno)
    lejos = ImageChops.invert(relleno.filter(ImageFilter.MaxFilter(11)))
    rgb = imagen.convert("RGB")
    lb, lj, tr_, cb = lum.tobytes(), lejos.tobytes(), trazo.tobytes(), rgb.tobytes()
    fondo = moda([lb[i] for i in range(len(lb)) if lj[i]])
    marcados = [i for i in range(len(lb)) if tr_[i]]
    tinta = sorted(lb[i] for i in marcados)[len(marcados) // 2]
    colores = sorted((cb[3 * i], cb[3 * i + 1], cb[3 * i + 2]) for i in marcados)
    color = colores[len(colores) // 2]
    out = Image.new("RGBA", imagen.size, (0, 0, 0, 0))
    po, pl, pa, pr, pf = out.load(), lum.load(), anillo.load(), rgb.load(), relleno.load()
    for y in range(imagen.size[1]):
        for x in range(imagen.size[0]):
            if pf[x, y]:
                po[x, y] = pr[x, y] + (255,)
            elif pa[x, y]:
                a = max(0.0, min(1.0, (fondo - pl[x, y]) / float(max(1, fondo - tinta))))
                if a > 0.02:
                    po[x, y] = color + (int(round(255 * a)),)
    caja = out.getchannel("A").getbbox()
    return None if caja is None else (out.crop(caja), caja)


def extrae_cara(sprite):
    """
    La cara provisional de un _reposo de hoy (768x768, RGBA): SimpleNamespace(ojos, boca, ojos_centro, boca_centro, sobre_ancha, desde, errores), las dos imagenes
    recortadas a lo que se pinta, sus centros en el sprite, cuanto sobre la fila mas ancha del cuerpo estan los ojos y cuanto bajo los ojos esta la boca (px del lienzo de
    1024). Los ojos son los dos aros de contorno oscuro con lo que encierran; la boca, el trazo oscuro mayor de la franja de la boca que no es de los ojos.
    """
    res = SimpleNamespace(ojos=None, boca=None, ojos_centro=None, boca_centro=None, sobre_ancha=None, desde=None, errores=[])
    if sprite.size != (SPRITE, SPRITE):
        res.errores.append("el _reposo no mide %dx%d sino %dx%d" % (SPRITE, SPRITE, sprite.size[0], sprite.size[1]))
        return res
    zona = lambda r, m: tuple(int(round(v * K_SPRITE)) + d for v, d in zip(r, (-m, -m, m, m)))
    zo = zona(ZONA_OJOS, 6)
    crop_o = sprite.crop(zo)
    oscuros = luminancia(crop_o).point(lambda v: 255 if v < UMBRAL_OSCURO else 0)
    lleno = rellena(oscuros)
    uno, _ = F.mayor_componente(lleno)
    otro, _ = F.mayor_componente(ImageChops.subtract(lleno, uno))
    a1, a2 = uno.histogram()[255], otro.histogram()[255]
    if min(a1, a2) < 500 or min(a1, a2) < 0.5 * max(a1, a2):
        res.errores.append("no encuentro los dos ojos (aros oscuros cerrados de area parecida) en la zona %s del sprite: %d y %d px" % (list(zo), a1, a2))
        return res
    ojos_mask = ImageChops.lighter(uno, otro)
    r_ojos = capa_de_trazo(crop_o, ojos_mask, ImageChops.multiply(oscuros, ojos_mask))
    # la boca: lo oscuro de su franja que no es de los ojos (los ojos bajan hasta el borde de arriba de la franja)
    zb = zona(ZONA_BOCA, 0)
    crop_b = sprite.crop(zb)
    ojos_en_b = Image.new("L", crop_b.size, 0)
    ojos_en_b.paste(ojos_mask.filter(ImageFilter.MaxFilter(5)), (zo[0] - zb[0], zo[1] - zb[1]))
    osc_b = ImageChops.subtract(luminancia(crop_b).point(lambda v: 255 if v < UMBRAL_OSCURO else 0), ojos_en_b)
    sonrisa, _ = F.mayor_componente(osc_b)
    if sonrisa.histogram()[255] < 150:
        res.errores.append("no encuentro la sonrisa (trazo oscuro) en la zona %s del sprite" % (list(zb),))
        return res
    r_boca = capa_de_trazo(crop_b, sonrisa, sonrisa)
    if r_ojos is None or r_boca is None:
        res.errores.append("la cara salio vacia")
        return res
    res.ojos, caja_o = r_ojos
    res.boca, caja_b = r_boca
    res.ojos_centro = (zo[0] + (caja_o[0] + caja_o[2]) / 2.0, zo[1] + (caja_o[1] + caja_o[3]) / 2.0)
    res.boca_centro = (zb[0] + (caja_b[0] + caja_b[2]) / 2.0, zb[1] + (caja_b[1] + caja_b[3]) / 2.0)
    # la fila mas ancha del cuerpo del sprite (sin la franja de colores): de ahi cuelga la altura de los ojos
    ancho_max, y_ancha = 0, 0
    m = sprite.getchannel("A").point(lambda v: 255 if v >= 128 else 0)
    for y in range(int(CINTURA_HOY * K_SPRITE)):
        caja = m.crop((0, y, SPRITE, y + 1)).getbbox()
        if caja and caja[2] - caja[0] > ancho_max:
            ancho_max, y_ancha = caja[2] - caja[0], y
    res.sobre_ancha = round((res.ojos_centro[1] - y_ancha) / K_SPRITE, 1)
    res.desde = [round((res.boca_centro[0] - res.ojos_centro[0]) / K_SPRITE, 1), round((res.boca_centro[1] - res.ojos_centro[1]) / K_SPRITE, 1)]
    return res


def cuerpo_de(torso, tr):
    """(mascara L de lo que hay sobre la pantaloneta del torso, en el lienzo de 1024: imagen y su rect). La pantaloneta son los PANTALONETA_ALTO px de abajo de la entrega."""
    s = tr[0]
    alto = int(round((torso.bbox[3] - PANTALONETA_ALTO - torso.bbox[1]) * s))
    m = torso.norm.getchannel("A").point(lambda v: 255 if v >= 128 else 0)
    return m.crop((0, 0, m.width, max(1, min(m.height, alto))))


def coloca_cara(torso, tr, cara):
    """
    (rect de Ojos, rect de Boca) en el lienzo de 1024: los ojos CENTRADOS EN EL CUERPO (el centro del tramo opaco de la fila) y a «sobre_ancha» px de la fila mas ancha del cuerpo
    (sin la pantaloneta); la boca a «desde» de los ojos. Cada rect es el de la imagen a 1024/768 de su resolucion (la de hoy, sin remuestrear).
    """
    cuerpo = cuerpo_de(torso, tr)
    w, h = cuerpo.size
    ancho_max, fila = 0, 0
    for y in range(h):
        caja = cuerpo.crop((0, y, w, y + 1)).getbbox()
        if caja and caja[2] - caja[0] > ancho_max:
            ancho_max, fila = caja[2] - caja[0], y
    y_ojos = torso.rect[1] + fila + cara.sobre_ancha
    yy = max(0, min(h - 1, int(round(y_ojos - torso.rect[1]))))
    caja = cuerpo.crop((0, yy, w, yy + 1)).getbbox() or (0, 0, w, 1)
    x_ojos = torso.rect[0] + (caja[0] + caja[2]) / 2.0
    k = 1.0 / K_SPRITE

    def rect(im, cx, cy):
        wi, hi = int(round(im.size[0] * k)), int(round(im.size[1] * k))
        x0, y0 = int(round(cx - wi / 2.0)), int(round(cy - hi / 2.0))
        return [x0, y0, x0 + wi, y0 + hi]

    return rect(cara.ojos, x_ojos, y_ojos), rect(cara.boca, x_ojos + cara.desde[0], y_ojos + cara.desde[1])


# ============================================================================ 5. la tabla (arte_final.json y rig_articulaciones.json)


def nodos_de(clave, forma, j, tr, rect_ojos, rect_boca):
    """
    Los 12 nodos de la entrada algoritm_<clave> de arte_final.json, en el orden y con los nombres de articulaciones.personaje_guia (padre antes que hijo): la pierna ENTERA en
    PiernaX (RodillaX sigue en el punto medio del palo, con AntepiernaX sin sprite), el humero en BrazoX con CodoX y el antebrazo, Tronco en la cintura, Torso, Ojos y Boca.
    """
    pref = "char_algoritm_" + clave
    pz = forma.piezas
    nodos = []
    for lado in LADOS:
        s = lado.lower()
        nodos.append({"nombre": "Pierna" + lado, "tipo": "imagen", "padre": C, "punto": a_rig(tr, j["Cadera" + lado]), "imagen": "Pierna" + lado,
                      "sprite": "%s_parte_pierna_%s" % (pref, s), "rect": list(pz[("pierna", lado)].rect)})
    cintura = a_rig(tr, j["Tronco"])
    nodos.append({"nombre": "Tronco", "tipo": "grupo", "padre": C, "punto": cintura, "imagen": "", "sprite": "", "rect": [0, 0, LIENZO, LIENZO]})
    for lado in LADOS:
        nodos.append({"nombre": "Brazo" + lado, "tipo": "imagen", "padre": T, "punto": a_rig(tr, j["Hombro" + lado]), "imagen": "Brazo" + lado,
                      "sprite": "%s_parte_brazo_%s" % (pref, lado.lower()), "rect": list(pz[("brazo", lado)].rect)})
    nodos.append({"nombre": "Torso", "tipo": "imagen", "padre": T, "punto": list(cintura), "imagen": "Torso", "sprite": pref + "_parte_torso", "rect": list(pz[("torso", None)].rect)})
    for nombre, sprite, rect in (("Ojos", pref + "_ojos_neutra", rect_ojos), ("Boca", pref + "_boca_0", rect_boca)):
        nodos.append({"nombre": nombre, "tipo": "imagen", "padre": T, "punto": [int(round((rect[0] + rect[2]) / 2.0)), int(round((rect[1] + rect[3]) / 2.0))],
                      "imagen": nombre, "sprite": sprite, "rect": list(rect)})
    for lado in LADOS:
        nodos.append({"nombre": "Codo" + lado, "tipo": "articulacion", "padre": T + "/Brazo" + lado, "punto": a_rig(tr, j["Codo" + lado]), "imagen": "Antebrazo" + lado,
                      "sprite": "%s_parte_antebrazo_%s" % (pref, lado.lower()), "rect": list(pz[("antebrazo", lado)].rect)})
    for lado in LADOS:
        pierna = pz[("pierna", lado)]
        r = pierna.rect
        rod = a_rig(tr, j["Rodilla" + lado])
        # sin sprite: la pierna es entera. El rect (la mitad de abajo de la pierna) solo coloca el nodo vacio; las herramientas y el generador lo piden
        nodos.append({"nombre": "Rodilla" + lado, "tipo": "articulacion", "padre": C + "/Pierna" + lado, "punto": rod, "imagen": "Antepierna" + lado,
                      "sprite": "", "rect": [r[0], rod[1], r[2], r[3]]})
    return nodos


def entrada_de(clave, forma, j, holguras, tr, eje, rects_cara, cara_info):
    """La entrada algoritm_<clave> de arte_final.json: {nodos, partes, holguras, registro, cara_provisional}."""
    nodos = nodos_de(clave, forma, j, tr, *rects_cara)
    registro = {"lienzo": [LIENZO_ENTREGA[0], LIENZO_ENTREGA[1]], "escala": round(tr[0], 6), "tx": round(tr[1], 3), "ty": round(tr[2], 3),
                "escala_opcion": "A", "alto_hoy": ALTO_HOY, "eje_entrega": round(eje, 1), "cara": cara_info}
    return {"nodos": nodos, "partes": [], "holguras": holguras, "registro": registro, "cara_provisional": True}


# ============================================================================ 6. ensamblar el reposo (composite, informe y _reposo)


def ensambla(nodos, imagen_de, escala=1.0, fondo=(0, 0, 0, 0), origen=(0, 0), tam=None):
    """
    La figura en reposo (sin ningun giro) en el orden de dibujo del rig: las dos piernas (hijas de Cuerpo, antes de Tronco) y luego los hijos de Tronco en el orden de
    articulaciones.ORDEN_TRONCO_GUIA —desde INC-147 (09/10/2026), cada brazo, el torso y la cara: los brazos DETRAS de todo el cuerpo; antes de INC-147, el torso, la cara y
    los brazos encima—. Cada humero va con su antebrazo (el antebrazo es hijo del codo: se pinta justo despues de su humero). imagen_de(sprite) devuelve la imagen o None. El lienzo es el de 1024 por «escala»
    (o «tam» px); «origen» es la esquina de arriba a la izquierda del encuadre, en el lienzo de 1024.
    """
    por = {n["nombre"]: n for n in nodos}
    # el orden de Tronco sale de la unica fuente (el mismo que escribe rig_articulaciones.json y aplica BuildRigsFinal «orden»); el codo de cada brazo lo acompana
    codo_de = {"BrazoIzq": "CodoIzq", "BrazoDer": "CodoDer"}
    orden = [("PiernaIzq", None), ("PiernaDer", None)] + [(nombre, codo_de.get(nombre)) for nombre in A.ORDEN_TRONCO_GUIA]
    lado = tam if tam is not None else int(round(LIENZO * escala))
    lienzo = Image.new("RGBA", (lado, lado), fondo)
    for nombre, codo in orden:
        for n in [por[nombre]] + ([por[codo]] if codo else []):
            im = imagen_de(n["sprite"]) if n.get("sprite") else None
            if im is None:
                continue
            r = n["rect"]
            w, h = int(round((r[2] - r[0]) * escala)), int(round((r[3] - r[1]) * escala))
            if w > 0 and h > 0:
                _pega(lienzo, im, (w, h), (int(round((r[0] - origen[0]) * escala)), int(round((r[1] - origen[1]) * escala))))
    return lienzo


def _pega(lienzo, im, tam, pos):
    """alpha_composite de «im» (a «tam» px) en «pos», recortado a lo que cae dentro del lienzo (la llama sube por encima del lienzo en las hojas)."""
    im = im.resize(tam, Image.LANCZOS) if im.size != tam else im
    x, y = pos
    x0, y0 = max(0, -x), max(0, -y)
    x1, y1 = min(im.size[0], lienzo.size[0] - x), min(im.size[1], lienzo.size[1] - y)
    if x1 > x0 and y1 > y0:
        lienzo.alpha_composite(im.crop((x0, y0, x1, y1)), (x + x0, y + y0))


def composite(formas, entradas, caras, ruta):
    """El informe: cada forma ensamblada en reposo (a 0,5) con sus articulaciones marcadas (hombro, codo, cadera, rodilla y cintura) sobre un fondo liso."""
    paneles = []
    colores = {"Brazo": (220, 30, 30), "Codo": (30, 150, 30), "Pierna": (30, 60, 220), "Rodilla": (200, 160, 0), "Tronco": (180, 30, 180)}
    for clave, nombre, _ in FORMAS:
        nodos = entradas[clave]["nodos"]
        sprites = {"char_algoritm_%s_%s" % (clave, NOMBRE_C4[k]): pz.norm for k, pz in formas[clave].piezas.items()}
        final = getattr(caras[clave], "final", False)
        for n in nodos:   # la cara va por el nombre que le da el nodo: el de la forma (provisional) o el compartido (final, INC-148, leida del repo)
            if n["nombre"] in ("Ojos", "Boca"):
                if final:
                    ruta_png = P._png_de("Algoritm", n["sprite"])
                    sprites[n["sprite"]] = Image.open(ruta_png).convert("RGBA") if ruta_png else None
                else:
                    sprites[n["sprite"]] = caras[clave].ojos if n["nombre"] == "Ojos" else caras[clave].boca
        im = Image.new("RGBA", (LIENZO // 2, LIENZO // 2), (236, 232, 222, 255))
        im.alpha_composite(ensambla(nodos, sprites.get, escala=0.5))
        d = ImageDraw.Draw(im)
        d.rectangle([0, 0, im.width, 14], fill=(255, 255, 255, 230))
        d.text((4, 2), "algoritm_%s: reposo (cara %s)" % (clave, "final" if final else "provisional"), fill=(30, 30, 30, 255))
        for n in nodos:
            color = next((c for k, c in colores.items() if n["nombre"].startswith(k)), None)
            if color is not None:
                x, y = n["punto"][0] * 0.5, n["punto"][1] * 0.5
                d.ellipse([x - 4, y - 4, x + 4, y + 4], outline=color + (255,), width=2)
        paneles.append(im)
    hoja = Image.new("RGB", (sum(p.width for p in paneles), paneles[0].height), (236, 232, 222))
    for i, im in enumerate(paneles):
        hoja.paste(im.convert("RGB"), (i * im.width, 0))
    hoja.save(ruta)


def reposo_de(clave, nodos, imagen_de):
    """El _reposo (768x768, RGBA) de una forma: la figura en reposo en el lienzo de 1024 y reducida a SPRITE px (el mismo encuadre que el de hoy)."""
    return ensambla(nodos, imagen_de).resize((SPRITE, SPRITE), Image.LANCZOS)


def ruta_reposo(parte):
    return os.path.join(CARPETA_ARTE, "char_algoritm_%s_reposo.png" % parte)


# ============================================================================ 7. informe, escritura y ordenes


def informe(formas, tr, j, holguras, avisos, errores, caras):
    print("== Entrega de Algoritm (INC-136)")
    for e in errores:
        print("ERROR", e)
    if errores:
        for a in avisos:
            print("AVISO", a)
        print("\nla entrega no se puede procesar: corrige lo anterior")
        return
    s, tx, ty, eje = tr
    for clave, nombre, _ in FORMAS:
        print("\n%s: %d piezas" % (nombre, len(formas[clave].piezas)))
        for k in sorted(formas[clave].piezas, key=lambda k: (list(ROLES).index(k[0]), k[1] or "")):
            pz = formas[clave].piezas[k]
            print("  %-26s -> %-3s %-10s char_algoritm_%s_%s.png  rect %s  %dx%d px" % (pz.archivo, pz.lado or "-", pz.rol, clave, NOMBRE_C4[k], pz.rect, pz.norm.size[0], pz.norm.size[1]))
    print("\ntransformacion unica (opcion A, la del fuego): escala %.5f, x' = %.5f x + %.2f, y' = %.5f y %+.2f; el eje del cuerpo (x = %.1f en la entrega) cae en x = %d" % (
        s, s, tx, s, ty, eje, EJE_X))
    print("la figura mide %.0f px (como hoy), de y = %d a y = %d" % (ALTO_HOY, CORONILLA_HOY, CORONILLA_HOY + ALTO_HOY))
    print("\narticulaciones (lienzo de la entrega -> lienzo de 1024):")
    for nombre, p in j.items():
        print("  %-12s (%7.1f, %7.1f) -> %s" % (nombre, p[0], p[1], a_rig((s, tx, ty), p)))
    print("holguras de los codos (px de la entrega entre los dos tapones): %s" % holguras)
    for clave, nombre, _ in FORMAS:
        c = caras[clave]
        if getattr(c, "final", False):
            print("cara FINAL de %s (INC-148): %s en Ojos y Boca, caja %s; no se escribe desde aqui" % (nombre, c.prefijo, c.caja))
        else:
            print("cara provisional de %s: ojos %dx%d px, boca %dx%d px (a la resolucion del sprite)" % (nombre, c.ojos.size[0], c.ojos.size[1], c.boca.size[0], c.boca.size[1]))
    mb = 0.0
    for clave, _, _ in FORMAS:
        mb += sum(pz.norm.size[0] * pz.norm.size[1] * 4 for pz in formas[clave].piezas.values()) / 1e6
    mb_cara = sum((c.ojos.size[0] * c.ojos.size[1] + c.boca.size[0] * c.boca.size[1]) * 4 for c in caras.values() if c.ojos is not None) / 1e6
    print("\ntextura sin comprimir: %.2f MB las 21 partes + %.2f MB la cara provisional (las seis antepiernas que se borran pesaban 0,14 MB; el margen del paquete, RNF-06, es de unos 21 MB)" % (mb, mb_cara))
    for a in avisos:
        print("AVISO", a)


def ruta_parte(clave, k):
    """Donde va el PNG de una parte: donde ya esta (Frontal/ o la raiz, para conservar su .meta y su GUID) o, si es nueva, en Frontal/."""
    nombre = "char_algoritm_%s_%s" % (clave, NOMBRE_C4[k])
    return P._png_de("Algoritm", nombre) or os.path.join(FRONTAL, nombre + ".png")


def sobrantes():
    """Los PNG de antepierna del corte provisional (char_algoritm_<forma>_parte_antepierna_{izq,der}), si siguen en el repo: sobran con la pierna entera."""
    out = []
    for clave, _, _ in FORMAS:
        for s in ("izq", "der"):
            ruta = P._png_de("Algoritm", "char_algoritm_%s_parte_antepierna_%s" % (clave, s))
            if ruta:
                out.append(ruta)
    return out


def cara_final(previo):
    """
    ¿La entrada de arte_final.json ya lleva la cara FINAL (INC-148)? Si: «cara_provisional» es false (explicito: no basta con que falte) y trae «prefijo_cara». Con la cara final esta
    herramienta no vuelve a la provisional bajo ningun camino (aplica, cara_de, --rehaz-cara, --reposo).
    """
    return bool(previo) and previo.get("cara_provisional") is False and bool(previo.get("prefijo_cara"))


def cara_de_la_entrada(previo):
    """
    SimpleNamespace(final=True, ...) con lo de la cara final de una entrada: los nodos Ojos y Boca (sprite, rect, punto), registro.cara (la caja), prefijo_cara y el retrato; los PNG
    se leen del repo (los nombres compartidos). Sin ojos ni boca: la cara final no se vuelve a escribir desde aqui.
    """
    nodos = {n["nombre"]: n for n in previo["nodos"] if n["nombre"] in ("Ojos", "Boca")}
    return SimpleNamespace(final=True, ojos=None, boca=None, nodos=nodos, caja=list(previo["registro"]["cara"]), prefijo=previo["prefijo_cara"], retrato=previo.get("retrato"),
                           sobre_ancha=None, desde=None, errores=[], reutilizada=True)


def carga_cara_escrita(clave, previo):
    """La cara provisional que ya esta escrita (los dos PNG de Expresiones/ y los numeros de colocacion de arte_final.json), o None."""
    reg = (previo or {}).get("registro", {}).get("cara")
    o, b = (P._png_de("Algoritm", "char_algoritm_%s_%s" % (clave, n)) for n in ("ojos_neutra", "boca_0"))
    if not (previo and previo.get("cara_provisional") and isinstance(reg, dict) and o and b):
        return None
    return SimpleNamespace(ojos=Image.open(o).convert("RGBA"), boca=Image.open(b).convert("RGBA"), sobre_ancha=reg["sobre_ancha"], desde=list(reg["desde"]),
                           errores=[], reutilizada=True)


def cara_de(clave, nombre_reposo, previo, rehacer):
    """
    La cara de una forma: la FINAL si la entrada ya la lleva (cara_final: nunca la provisional; --rehaz-cara se niega porque extraeria del _reposo la cara provisional vieja),
    la provisional escrita (si no se pide rehacerla) o la extraida del _reposo de hoy del repo.
    """
    if cara_final(previo):
        c = cara_de_la_entrada(previo)
        if rehacer:
            c.errores.append("la cara de Algoritm ya es la FINAL (INC-148): --rehaz-cara la sustituiria por la provisional del _reposo; no se hace")
        return c
    if not rehacer:
        c = carga_cara_escrita(clave, previo)
        if c is not None:
            return c
    ruta = ruta_reposo(nombre_reposo)
    c = extrae_cara(Image.open(ruta).convert("RGBA"))
    c.reutilizada = False
    return c


def corre(args, etiqueta):
    """Como preparar_arte_final.corre: lanza un script y muestra solo lo importante. Devuelve su codigo de salida."""
    return F.corre(args, etiqueta)


def escribe_png(ruta, imagen, log):
    os.makedirs(os.path.dirname(ruta), exist_ok=True)
    existia = os.path.isfile(ruta)
    if existia and F._mismos_pixeles(ruta, imagen):
        log.append("  igual     %s" % os.path.relpath(ruta, P.RAIZ))
        return False
    imagen.save(ruta)
    log.append("  %s %s" % ("sustituye" if existia else "nuevo    ", os.path.relpath(ruta, P.RAIZ)))
    return True


def aplica(formas, tr, j, holguras, eje, caras, json_ruta=None, escribe_tablas=True):
    """
    --aplicar: escribe las 21 partes, borra las seis antepiernas, escribe la cara provisional, guarda arte_final.json (o «json_ruta», para la autoprueba) y, con
    «escribe_tablas», regenera rig_articulaciones.json y clips_personajes.json. Devuelve cuantos pasos fallaron.
    """
    print("\n== Aplicando al repo ==")
    log = []
    personajes = A.cargar_arte_final() if json_ruta is None else A.cargar_arte_final(json_ruta)
    # GUARDA (INC-148): una entrada con la cara FINAL no vuelve a la provisional, llegue lo que llegue en «caras»
    finales = {clave for clave, _, _ in FORMAS if cara_final(personajes.get("algoritm_" + clave))}
    for clave, _, _ in FORMAS:
        for k, pz in formas[clave].piezas.items():
            escribe_png(ruta_parte(clave, k), pz.norm, log)
        if clave in finales:
            log.append("  cara FINAL de %s (INC-148): no se escribe la provisional, sus nodos Ojos y Boca y registro.cara se conservan" % clave)
        elif not getattr(caras[clave], "reutilizada", False):
            escribe_png(os.path.join(EXPRESIONES, "char_algoritm_%s_ojos_neutra.png" % clave), caras[clave].ojos, log)
            escribe_png(os.path.join(EXPRESIONES, "char_algoritm_%s_boca_0.png" % clave), caras[clave].boca, log)
        else:
            log.append("  cara provisional de %s: ya escrita, se conserva (--rehaz-cara la extrae de nuevo)" % clave)
    for ruta in sobrantes():
        for r in (ruta, ruta + ".meta"):
            if os.path.isfile(r):
                os.remove(r)
                log.append("  borra     %s" % os.path.relpath(r, P.RAIZ))
    print("\n".join(log))
    for clave, _, _ in FORMAS:
        previo = personajes.get("algoritm_" + clave)
        if clave in finales:
            fin = cara_de_la_entrada(previo)
            rect_ojos, rect_boca = list(fin.nodos["Ojos"]["rect"]), list(fin.nodos["Boca"]["rect"])
            entrada = entrada_de(clave, formas[clave], j, holguras, tr, eje, (rect_ojos, rect_boca), {})
            for n in entrada["nodos"]:   # los nodos de la cara y su registro son los de la cara final, no los provisionales que nodos_de() propone
                if n["nombre"] in fin.nodos:
                    n["sprite"], n["rect"], n["punto"] = fin.nodos[n["nombre"]]["sprite"], list(fin.nodos[n["nombre"]]["rect"]), list(fin.nodos[n["nombre"]]["punto"])
            entrada["registro"]["cara"] = list(fin.caja)
            entrada["cara_provisional"] = False
            entrada["prefijo_cara"] = fin.prefijo
            if fin.retrato:
                entrada["retrato"] = fin.retrato
            personajes["algoritm_" + clave] = entrada
            continue
        cara = caras[clave]
        rect_ojos, rect_boca = coloca_cara(formas[clave].piezas[("torso", None)], tr, cara)
        info = {"fuente": "reposo de hoy (bd8b802), provisional", "sobre_ancha": cara.sobre_ancha, "desde": list(cara.desde),
                "ojos": list(rect_ojos), "boca": list(rect_boca)}
        personajes["algoritm_" + clave] = entrada_de(clave, formas[clave], j, holguras, tr, eje, (rect_ojos, rect_boca), info)
    if json_ruta is None:
        A.guardar_arte_final(personajes)
    else:
        A.guardar_arte_final(personajes, json_ruta)
    print("  arte_final.json: las tres entradas algoritm_*")
    fallos = 0
    if escribe_tablas:
        py = sys.executable
        fallos += 1 if corre([py, os.path.join(AQUI, "articulaciones.py")], "articulaciones") else 0
        fallos += 1 if corre([py, os.path.join(AQUI, "coreografia.py"), "--valida"], "coreografia") else 0
        print("\n%s" % ("TODO EN VERDE" if not fallos else "%d pasos fallaron: revisa antes de entregar" % fallos))
        print(bloque_local())
    return fallos


def bloque_local():
    return """
==================== Sesion local (Santiago), con el Editor abierto ====================
 0. git pull   (trae el arte, arte_final.json, rig_articulaciones.json, clips_personajes.json y BuildRigsFinal.cs.txt)
 1. Copiar el generador y recompilar:
      Copy-Item claudeDocs/tasks/Personajes/herramientas/BuildRigsFinal.cs.txt Assets/Editor/ClaudeBuildRigsFinal.cs
      pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 recompile
 2. Por coplay execute_script, EN ESTE ORDEN:
      ClaudeBuildRigsFinal.Execute("estado")     // antes
      ClaudeBuildRigsFinal.Execute("perfil")     // la familia: las piernas detras del torso (SetSiblingIndex, ningun fileID)
      ClaudeBuildRigsFinal.Execute("sprites")    // Algoritm: siete piezas por forma, AntepiernaX sin sprite y apagada, Ojos y Boca encendidos, <forma>_cara.asset
      ClaudeBuildRigsFinal.Execute("orden")      // INC-147: los brazos de Algoritm detras de todo el cuerpo (SetSiblingIndex, ningun fileID)
      ClaudeBuildRigsFinal.Execute("clips")      // vuelca clips_personajes.json en los .anim
      ClaudeBuildRigsFinal.Execute("estado")     // despues
    «nodos» no hace falta (todos los nodos existen); «orden» SI, desde INC-147 (orden_tronco de Algoritm: BrazoIzq, BrazoDer, Torso, Ojos, Boca).
 3. Comprobar los fileID:  git diff -U0 Assets/Game/Prefabs/Characters  (ninguna linea «--- !u!» quitada; en Algoritm cambian sprites, rects, pivotes y el CharacterFace.faceSet)
 4. Pruebas: tests-edit CharacterRig_ · tests-edit CharacterFace · tests-play Credits_ · tests-play MainMenu_ · tests-play NarrativeScene_
 5. Borrar el andamiaje (Assets/Editor/ClaudeBuildRigsFinal.cs y su .meta) y subir los .meta nuevos (las caras en Algoritm/Expresiones/) y los _cara.asset.
========================================================================================
"""


def reposo(json_ruta=None):
    """
    --reposo: reescribe los tres _reposo con el diseno nuevo y la cara provisional. Lee del repo las partes y la cara (Frontal/ y Expresiones/) y las tres entradas de arte_final.json
    (o de «json_ruta»). Devuelve 0, o 1 si falta algo (se corre despues de --aplicar).
    """
    personajes = A.cargar_arte_final() if json_ruta is None else A.cargar_arte_final(json_ruta)
    cache = {}

    def imagen_del_repo(sprite):
        ruta = P._png_de("Algoritm", sprite)
        if ruta is None:
            return None
        if ruta not in cache:
            cache[ruta] = Image.open(ruta).convert("RGBA")
        return cache[ruta]

    escritos = 0
    for clave, _, parte in FORMAS:
        entrada = personajes.get("algoritm_" + clave)
        if not entrada:
            print("ERROR no hay entrada algoritm_%s en arte_final.json: corre antes --aplicar" % clave)
            return 1
        if cara_final(entrada):
            # GUARDA (INC-148): con la cara final marcada, Ojos y Boca tienen que nombrar la cara COMPARTIDA; si una tabla a medias nombrara la provisional, el _reposo
            # volveria a ensenar la cara vieja (esa es justo la regresion que esta guarda impide)
            malos = [n["nombre"] + " = " + str(n.get("sprite")) for n in entrada["nodos"] if n["nombre"] in ("Ojos", "Boca")
                     and not str(n.get("sprite", "")).startswith(entrada["prefijo_cara"] + "_" + {"Ojos": "ojos_", "Boca": "boca_"}[n["nombre"]])]
            if malos:
                print("ERROR algoritm_%s dice cara final (%s) pero sus nodos nombran otra: %s: el _reposo volveria a la cara provisional; corre preparar_expresion.py --entrega" % (
                    clave, entrada["prefijo_cara"], ", ".join(malos)))
                return 1
        faltan = [n["sprite"] for n in entrada["nodos"] if n.get("sprite") and P._png_de("Algoritm", n["sprite"]) is None]
        if faltan:
            print("ERROR faltan los PNG de %s: %s (corre antes --aplicar)" % (clave, ", ".join(faltan)))
            return 1
        im = reposo_de(clave, entrada["nodos"], imagen_del_repo)
        ruta = ruta_reposo(parte)
        existia = os.path.isfile(ruta)
        if existia and F._mismos_pixeles(ruta, im):
            print("  igual     %s" % os.path.relpath(ruta, P.RAIZ))
            continue
        im.save(ruta)
        escritos += 1
        print("  %s %s  (768x768, mismo nombre y mismo GUID)" % ("sustituye" if existia else "nuevo    ", os.path.relpath(ruta, P.RAIZ)))
    print("%d _reposo reescritos" % escritos)
    return 0


# ============================================================================ 8. la autoprueba (entrega sintetica)


def _capsula(d, p, q, r, color, contorno=(0, 0, 0, 255), borde=3):
    """Una capsula (dos circulos y el rectangulo que los une) de radio r entre los centros p y q, con contorno."""
    for radio, col in ((r, contorno), (r - borde, color)):
        d.ellipse([p[0] - radio, p[1] - radio, p[0] + radio, p[1] + radio], fill=col)
        d.ellipse([q[0] - radio, q[1] - radio, q[0] + radio, q[1] + radio], fill=col)
        dx, dy = q[0] - p[0], q[1] - p[1]
        L = math.hypot(dx, dy)
        nx, ny = -dy / L * radio, dx / L * radio
        d.polygon([(p[0] + nx, p[1] + ny), (q[0] + nx, q[1] + ny), (q[0] - nx, q[1] - ny), (p[0] - nx, p[1] - ny)], fill=col)


# la verdad de la entrega sintetica, en el lienzo de 1300x1500 (las medidas de la entrega real: hombros, codos, caderas, pies y base del torso)
SINTETICA = {
    "hombro_izq": (385, 797), "codo_izq": (260, 939), "hombro_der": (885, 801), "codo_der": (1018, 933),
    "cadera_izq": (503, 1115), "cadera_der": (662, 1125), "base_torso": 1162, "r_brazo": 30, "r_antebrazo": 38, "r_pierna": 30,
    "top_izq": 1085, "top_der": 1095, "pie_izq": 1252, "pie_der": 1262,
}


def entrega_sintetica(destino):
    """
    Una entrega de Algoritm sintetica con tres formas (torso distinto, mismas extremidades), 1300x1500 cada pieza: «derecho» y «derecha» mezclados en los nombres, «pie» en
    dos formas y «pierna» en la tercera, y motas sueltas (una grande en un humero, para el aviso). La verdad esta en SINTETICA. Devuelve la lista de archivos escritos.
    """
    azar = random.Random(7)
    escritos = []
    S = SINTETICA
    colores = {"fuego": (255, 145, 34, 255), "rueda": (91, 65, 52, 255), "gota": (110, 214, 251, 255)}
    cuerpos = {"fuego": ((319, 44, 948, 960), (255, 220, 60, 255)), "rueda": ((272, 259, 1018, 960), (190, 160, 130, 255)), "gota": ((299, 180, 946, 960), (120, 230, 255, 255))}
    nombres = {"fuego": ("torso_fuego_algoritm", "pie_derecho_fuego", "pie_izquierdo_fuego", "mano_derecho_fuego", "mano_izquierdo_fuego", "brazo_derecho_fuego", "brazo_izquierdo_fuego"),
               "rueda": ("torso_rueda", "pie_derecho_rueda", "pie_izquierdo_rueda", "mano_derecho_rueda", "mano_izquierdo_rueda", "brazo_derecho_rueda", "brazo_izquierdo_rueda"),
               "gota": ("torso_gota_algoritm", "pierna_derecha_gota", "pierna_izquierdo_gota", "mano_derecha_gota", "mano_izquierdo_gota", "brazo_derecho_gota", "brazo_izquierdo_gota")}
    for clave, carpeta in (("fuego", "Fuego"), ("rueda", "Rueda"), ("gota", "Gota")):
        os.makedirs(os.path.join(destino, carpeta), exist_ok=True)
        torso_n, pie_d, pie_i, mano_d, mano_i, brazo_d, brazo_i = nombres[clave]

        def lienzo():
            return Image.new("RGBA", LIENZO_ENTREGA, (0, 0, 0, 0))

        def guarda(nombre, im):
            ruta = os.path.join(destino, carpeta, nombre + ".png")
            im.save(ruta)
            escritos.append(ruta)

        (x0, y0, x1, y1), cuerpo = cuerpos[clave]
        im = lienzo()
        d = ImageDraw.Draw(im)
        d.ellipse([x0, y0, x1, y1 + 60], fill=(0, 0, 0, 255))
        d.ellipse([x0 + 6, y0 + 6, x1 - 6, y1 + 54], fill=cuerpo)
        d.rectangle([380, y1 - 10, 860, S["base_torso"] - 1], fill=(0, 0, 0, 255))
        d.rectangle([386, y1 - 4, 854, S["base_torso"] - 7], fill=(40, 125, 74, 255))    # la pantaloneta: la fila mas ancha del cuerpo queda arriba
        d.point((azar.randrange(20, 120), azar.randrange(20, 120)), fill=(255, 0, 0, 255))   # una mota de un pixel
        guarda(torso_n, im)
        for lado, nombre_h, nombre_a, nombre_p in (("izq", brazo_i, mano_i, pie_i), ("der", brazo_d, mano_d, pie_d)):
            im = lienzo()
            _capsula(ImageDraw.Draw(im), S["hombro_" + lado], S["codo_" + lado], S["r_brazo"], colores[clave])
            ImageDraw.Draw(im).rectangle([1200, 1400, 1213, 1413], fill=(255, 0, 0, 255))   # una mota de 14x14: pasa del 1 % del humero y se avisa
            guarda(nombre_h, im)
            # el antebrazo con la mano: un palo de radio 38 cuyo tapon cae a 3 px del del humero, y una mano de varios dedos
            sx = -1 if lado == "izq" else 1
            tapon = (S["codo_" + lado][0] + sx * 2, S["codo_" + lado][1] - 2)
            muneca = (tapon[0] + sx * 120, tapon[1] + 140)
            im = lienzo()
            d = ImageDraw.Draw(im)
            _capsula(d, tapon, muneca, S["r_antebrazo"], colores[clave])
            for k in range(3):
                _capsula(d, muneca, (muneca[0] + sx * (50 + 18 * k), muneca[1] + 30 + 25 * k), 14, colores[clave], borde=2)
            guarda(nombre_a, im)
            # la pierna: un palo con el tapon de arriba redondo y un pie ovalado que sale hacia fuera
            cx, top, pie = S["cadera_" + lado][0], S["top_" + lado], S["pie_" + lado]
            im = lienzo()
            d = ImageDraw.Draw(im)
            _capsula(d, (cx, top + S["r_pierna"]), (cx, pie - 10), S["r_pierna"], colores[clave])
            izq = lado == "izq"
            d.ellipse([cx - 70 if izq else cx - 30, pie - 20, cx + 30 if izq else cx + 70, pie + 75], fill=(0, 0, 0, 255))
            d.ellipse([cx - 64 if izq else cx - 24, pie - 14, cx + 24 if izq else cx + 64, pie + 69], fill=colores[clave])
            guarda(nombre_p, im)
    return escritos


def sprite_sintetico():
    """Un _reposo (768x768) sintetico con la cara del de hoy: dos ojos con aro oscuro, esclera clara, pupila y brillo, y una sonrisa, sobre un cuerpo de color liso (se dibuja al doble y se reduce: bordes suaves)."""
    k = 2
    im = Image.new("RGBA", (SPRITE * k, SPRITE * k), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    e = lambda *v: [c * k for c in v]
    d.ellipse(e(190, 40, 580, 470), fill=(255, 226, 149, 255), outline=(193, 101, 39, 255), width=8 * k)
    d.rectangle(e(200, 450, 570, 520), fill=(40, 160, 90, 255))
    for cx in (320, 440):
        d.ellipse(e(cx - 43, 332 - 43, cx + 43, 332 + 43), fill=(59, 18, 5, 255))
        d.ellipse(e(cx - 36, 332 - 36, cx + 36, 332 + 36), fill=(250, 238, 205, 255))
        d.ellipse(e(cx - 20, 332 - 20, cx + 20, 332 + 20), fill=(59, 18, 5, 255))
        d.ellipse(e(cx - 8, 332 - 14, cx, 332 - 6), fill=(255, 255, 255, 255))
    d.arc(e(330, 335, 440, 425), 10, 170, fill=(59, 18, 5, 255), width=9 * k)
    return im.resize((SPRITE, SPRITE), Image.LANCZOS)


class EnArte:
    """Mientras dura el with, las rutas del arte de Algoritm (y las de prefabs._png_de) apuntan a una carpeta temporal: --aplicar y --reposo se prueban sin tocar el repo."""

    def __init__(self, base):
        self.base = base

    def __enter__(self):
        g = globals()
        self.previo = (g["CARPETA_ARTE"], g["FRONTAL"], g["EXPRESIONES"], P.PERSONAJES_ARTE)
        g["CARPETA_ARTE"] = os.path.join(self.base, "Algoritm")
        g["FRONTAL"], g["EXPRESIONES"] = os.path.join(g["CARPETA_ARTE"], "Frontal"), os.path.join(g["CARPETA_ARTE"], "Expresiones")
        P.PERSONAJES_ARTE = self.base
        return self

    def __exit__(self, *exc):
        g = globals()
        g["CARPETA_ARTE"], g["FRONTAL"], g["EXPRESIONES"], P.PERSONAJES_ARTE = self.previo
        return False


def autoprueba():
    """La herramienta recupera de una entrega sintetica las medidas conocidas, escribe y reensambla en una copia, rechaza lo que debe y avisa de lo que debe."""
    import contextlib
    import io
    import shutil
    malos = 0

    def caso(nombre, ok, detalle=""):
        nonlocal malos
        malos += 0 if ok else 1
        print("  %-100s %s" % (nombre, "bien" if ok else "FALLA " + detalle))

    def sale(argv):
        err = io.StringIO()
        try:
            with contextlib.redirect_stderr(err), contextlib.redirect_stdout(io.StringIO()):
                codigo = main(argv)
        except SystemExit as e:
            return e.code, err.getvalue()
        return codigo, err.getvalue()

    S = SINTETICA
    with tempfile.TemporaryDirectory() as tmp:
        entrega = os.path.join(tmp, "Algoritm")
        entrega_sintetica(entrega)
        print("== entrega sintetica de tres formas (derecha y derecho mezclados, «pie» y «pierna», motas)")
        formas, avisos, errores = lee_entrega(entrega)
        caso("se lee sin errores", not errores, str(errores))
        caso("las tres formas con siete piezas cada una", sorted(formas) == ["fuego", "gota", "rueda"] and all(len(f.piezas) == 7 for f in formas.values()))
        caso("«pie» (fuego, rueda) y «pierna» (gota) van los dos a la pierna entera de cada lado",
             all((("pierna", "Izq") in f.piezas and ("pierna", "Der") in f.piezas) for f in formas.values()))
        caso("el lado lo decide la posicion (Izq a la izquierda de la pantalla) aunque se mezcle derecha y derecho",
             all(f.piezas[("brazo", "Izq")].bbox[0] < f.piezas[("brazo", "Der")].bbox[0] and f.piezas[("pierna", "Izq")].bbox[0] < f.piezas[("pierna", "Der")].bbox[0]
                 and f.piezas[("antebrazo", "Izq")].bbox[0] < f.piezas[("antebrazo", "Der")].bbox[0] for f in formas.values()))
        caso("las motas sueltas se cuentan y se quitan; la grande (14x14) de un humero se avisa",
             all(f.piezas[("torso", None)].limpieza["motas"] >= 1 for f in formas.values()) and sum("trozo suelto" in a for a in avisos) >= 6, str(avisos[:2]))
        caso("las extremidades de las tres formas coinciden: no hay aviso de geometria", not any("no coinciden" in a for a in avisos), str([a for a in avisos if "no coinciden" in a]))
        tr = transformacion(formas["fuego"])
        s, tx, ty, eje = tr
        ps = list(formas["fuego"].piezas.values())
        alto = max(p.bbox[3] for p in ps) - min(p.bbox[1] for p in ps)
        caso("la escala deja la figura en ALTO_HOY (%.0f px)" % ALTO_HOY, abs(s * alto - ALTO_HOY) < 1e-6, "%.3f" % (s * alto))
        caso("la punta de la llama cae en CORONILLA_HOY y el eje del cuerpo en x = 512",
             abs(s * min(p.bbox[1] for p in ps) + ty - CORONILLA_HOY) < 1e-6 and abs(s * eje + tx - EJE_X) < 1e-6)
        for f in formas.values():
            normaliza(f, tr)
        caso("una sola transformacion: las extremidades de las tres formas quedan en el mismo rect",
             all(formas["fuego"].piezas[k].rect == formas[c].piezas[k].rect for c in ("rueda", "gota") for k in formas["fuego"].piezas if k[0] != "torso"))
        caso("todo cabe en el lienzo de 1024", all(0 <= p.rect[0] and 0 <= p.rect[1] and p.rect[2] <= LIENZO and p.rect[3] <= LIENZO for f in formas.values() for p in f.piezas.values()))
        j, holguras = mide(formas["fuego"], eje)
        for nombre, v in (("HombroIzq", S["hombro_izq"]), ("HombroDer", S["hombro_der"]), ("CaderaIzq", S["cadera_izq"]), ("CaderaDer", S["cadera_der"])):
            e = dist(j[nombre], v)
            caso("%-10s recuperado (%.1f, %.1f) contra (%d, %d): error %.2f px de la entrega" % (nombre, j[nombre][0], j[nombre][1], v[0], v[1], e), e <= 2.0)
        for lado in LADOS:
            e = dist(j["Codo" + lado], S["codo_" + lado.lower()])
            caso("Codo%s: el punto medio de los tapones, a %.1f px del tapon del humero (holgura %.1f)" % (lado, e, holguras["Codo" + lado]), e <= 4.0 and holguras["Codo" + lado] <= 6.0)
        for lado in LADOS:
            top, pie = S["top_" + lado.lower()], S["pie_" + lado.lower()]
            yr = j["Rodilla" + lado][1]
            esperado = 0.5 * (S["base_torso"] + pie - 10)
            caso("Rodilla%s: el punto medio del palo visible (y = %.0f, esperaba ~%.0f)" % (lado, yr, esperado), abs(yr - esperado) <= 14.0 and top < yr < pie)
        caso("Tronco: la cintura, CINTURA_SOBRE_BASE px sobre la base del torso, en el eje del cuerpo",
             abs(j["Tronco"][1] - (S["base_torso"] - CINTURA_SOBRE_BASE)) < 1.5 and abs(j["Tronco"][0] - eje) < 1e-6,
             str(j["Tronco"]))
        # --- la cara provisional
        sprite = sprite_sintetico()
        cara = extrae_cara(sprite)
        caso("la cara se extrae del sprite: dos ojos y una sonrisa", not cara.errores and cara.ojos is not None and cara.boca is not None, str(cara.errores))
        torso = formas["fuego"].piezas[("torso", None)]
        if not cara.errores:
            caso("los ojos miden lo que dibuje (dos aros de 43 px de radio con 120 px entre centros: 206 x 87)", abs(cara.ojos.size[0] - 206) <= 6 and abs(cara.ojos.size[1] - 87) <= 4, str(cara.ojos.size))
            caso("la boca es solo la sonrisa, sin lo de los ojos (mide unos 117 x 45)", 100 <= cara.boca.size[0] <= 130 and 35 <= cara.boca.size[1] <= 55, str(cara.boca.size))
            a_ojos = cara.ojos.getchannel("A")
            caso("los ojos: opacos dentro (la pupila y la esclera), transparentes entre los dos y con borde suave",
                 a_ojos.getpixel((43, 43)) == 255 and a_ojos.getpixel((60, 43)) == 255 and a_ojos.getpixel((103, 43)) == 0 and 0 < sum(1 for v in a_ojos.tobytes() if 0 < v < 255))
            caso("la sonrisa es del color del trazo (cafe oscuro) y no del fondo",
                 all(p[3] == 0 or sum(p[:3]) / 3.0 < 90 for p in (cara.boca.getpixel((x, y)) for x in range(0, cara.boca.size[0], 7) for y in range(0, cara.boca.size[1], 5))))
            rect_o, rect_b = coloca_cara(torso, tr, cara)
            cuerpo = cuerpo_de(torso, tr)
            cx_cuerpo = torso.rect[0] + 0.5 * (cuerpo.getbbox()[0] + cuerpo.getbbox()[2])
            caso("los ojos se centran en el cuerpo, la boca queda bajo ellos y todo dentro del torso",
                 abs(0.5 * (rect_o[0] + rect_o[2]) - cx_cuerpo) <= 40 and rect_b[1] > rect_o[1] and rect_b[3] < torso.rect[3] - PANTALONETA_ALTO * s
                 and rect_o[0] >= torso.rect[0] and rect_o[2] <= torso.rect[2], "%s %s torso %s" % (rect_o, rect_b, torso.rect))
            caso("la cara se coloca a la altura relativa de hoy: los ojos a sobre_ancha px de la fila mas ancha del cuerpo",
                 abs((0.5 * (rect_o[1] + rect_o[3])) - (torso.rect[1] + max(range(cuerpo.size[1]), key=lambda y: (lambda b: (b[2] - b[0]) if b else 0)(cuerpo.crop((0, y, cuerpo.size[0], y + 1)).getbbox())) + cara.sobre_ancha)) <= 2.0)
        # --- la tabla
        entradas, caras = {}, {}
        for clave, _, _ in FORMAS:
            caras[clave] = cara
            ro, rb = coloca_cara(formas[clave].piezas[("torso", None)], tr, cara)
            entradas[clave] = entrada_de(clave, formas[clave], j, holguras, tr, eje, (ro, rb), {"sobre_ancha": cara.sobre_ancha, "desde": list(cara.desde), "ojos": ro, "boca": rb})
        n = {x["nombre"]: x for x in entradas["fuego"]["nodos"]}
        caso("doce nodos, padre antes que hijo, con los nombres de personaje_guia",
             [x["nombre"] for x in entradas["fuego"]["nodos"]] == ["PiernaIzq", "PiernaDer", "Tronco", "BrazoIzq", "BrazoDer", "Torso", "Ojos", "Boca", "CodoIzq", "CodoDer", "RodillaIzq", "RodillaDer"])
        caso("la pierna es ENTERA: PiernaX con su sprite, y AntepiernaX (bajo RodillaX) sin sprite ni PNG en la tabla",
             n["PiernaIzq"]["sprite"] == "char_algoritm_fuego_parte_pierna_izq" and n["RodillaIzq"]["sprite"] == "" and n["RodillaIzq"]["imagen"] == "AntepiernaIzq"
             and n["RodillaDer"]["sprite"] == "" and not any("antepierna" in x["sprite"] for x in entradas["fuego"]["nodos"]))
        caso("ninguna pieza se espeja: derecho es BrazoDer, a la derecha de la pantalla",
             n["BrazoDer"]["punto"][0] > n["BrazoIzq"]["punto"][0] and n["BrazoDer"]["sprite"].endswith("brazo_der") and n["CodoDer"]["sprite"].endswith("antebrazo_der"))
        caso("el hombro y la cadera caen dentro del rect de su pieza",
             all(r[0] <= p[0] <= r[2] and r[1] <= p[1] <= r[3] for r, p in ((n[k]["rect"], n[k]["punto"]) for k in ("BrazoIzq", "BrazoDer", "PiernaIzq", "PiernaDer"))))
        caso("la entrada lleva cara_provisional y el registro (lienzo, escala, tx, ty)",
             entradas["fuego"]["cara_provisional"] is True and entradas["fuego"]["registro"]["lienzo"] == [1300, 1500] and entradas["fuego"]["registro"]["escala"] > 0)
        # --- el reposo
        sprites = {"char_algoritm_fuego_" + NOMBRE_C4[k]: pz.norm for k, pz in formas["fuego"].piezas.items()}
        sprites["char_algoritm_fuego_ojos_neutra"], sprites["char_algoritm_fuego_boca_0"] = cara.ojos, cara.boca
        rep = reposo_de("fuego", entradas["fuego"]["nodos"], sprites.get)
        caso("el _reposo mide 768x768 RGBA", rep.size == (SPRITE, SPRITE) and rep.mode == "RGBA")
        caja = rep.getchannel("A").point(lambda v: 255 if v >= 128 else 0).getbbox()
        caso("el _reposo tiene la figura del alto que tiene la de hoy (%.0f px del lienzo de 1024, de y = %d)" % (ALTO_HOY, CORONILLA_HOY),
             caja is not None and abs((caja[3] - caja[1]) / K_SPRITE - ALTO_HOY) <= 6.0 and abs(caja[1] / K_SPRITE - CORONILLA_HOY) <= 3.0, str(caja))
        # las piernas van DETRAS del torso: donde se solapan se ve el torso
        ps_ = formas["fuego"].piezas
        tor, pie = ps_[("torso", None)], ps_[("pierna", "Izq")]
        solape = None
        for y in range(pie.rect[1], pie.rect[3]):
            for x in range(pie.rect[0], pie.rect[2]):
                if tor.rect[0] <= x < tor.rect[2] and tor.rect[1] <= y < tor.rect[3]:
                    tc, lc = tor.norm.getpixel((x - tor.rect[0], y - tor.rect[1])), pie.norm.getpixel((x - pie.rect[0], y - pie.rect[1]))
                    if tc[3] == 255 and lc[3] == 255 and tc[:3] != lc[:3]:
                        solape = (x, y, tc)
                        break
            if solape:
                break
        if solape:
            visto = ensambla(entradas["fuego"]["nodos"], sprites.get).getpixel(solape[:2])
            caso("las piernas van detras del torso en el reposo (donde se solapan se ve el torso)", visto[:3] == solape[2][:3], "%s contra %s" % (visto, solape[2]))
        else:
            caso("la entrega sintetica solapa la pierna con el torso (para probar el orden)", False, "no hay solape")
        # INC-147: los brazos van DETRAS de todo el cuerpo: donde un humero se solapa con el torso se ve el torso (antes de INC-147 se veia el brazo)
        sol_b = None
        for lado_ in ("Izq", "Der"):
            bra = ps_[("brazo", lado_)]
            for y in range(bra.rect[1], bra.rect[3]):
                for x in range(bra.rect[0], bra.rect[2]):
                    if tor.rect[0] <= x < tor.rect[2] and tor.rect[1] <= y < tor.rect[3]:
                        tc, bc = tor.norm.getpixel((x - tor.rect[0], y - tor.rect[1])), bra.norm.getpixel((x - bra.rect[0], y - bra.rect[1]))
                        if tc[3] == 255 and bc[3] == 255 and tc[:3] != bc[:3]:
                            sol_b = (x, y, tc)
                            break
                if sol_b:
                    break
            if sol_b:
                break
        if sol_b:
            visto = ensambla(entradas["fuego"]["nodos"], sprites.get).getpixel(sol_b[:2])
            caso("los brazos van detras del torso en el reposo (INC-147: donde se solapan se ve el torso)", visto[:3] == sol_b[2][:3], "%s contra %s" % (visto, sol_b[2]))
        else:
            caso("la entrega sintetica solapa un humero con el torso (para probar el orden de INC-147)", False, "no hay solape")
        caso("ensambla toma el orden de Tronco de articulaciones.ORDEN_TRONCO_GUIA (INC-147: brazos, torso, cara)", A.ORDEN_TRONCO_GUIA == ["BrazoIzq", "BrazoDer", "Torso", "Ojos", "Boca"])
        # --- aplicar y reposo sobre una copia: el repo no se toca
        print("== --aplicar y --reposo en una copia del arte")
        with tempfile.TemporaryDirectory() as tmp2:
            base = os.path.join(tmp2, "Characters")
            alg = os.path.join(base, "Algoritm")
            os.makedirs(os.path.join(alg, "Frontal"))
            for clave, _, parte in FORMAS:
                sprite_sintetico().save(os.path.join(alg, "char_algoritm_%s_reposo.png" % parte))
                for s_ in ("izq", "der"):
                    for sufijo in ("", ".meta"):
                        with open(os.path.join(alg, "Frontal", "char_algoritm_%s_parte_antepierna_%s.png%s" % (clave, s_, sufijo)), "wb") as f:
                            f.write(b"viejo")
            vieja = os.path.join(alg, "Frontal", "char_algoritm_fuego_parte_torso.png")
            Image.new("RGBA", (10, 10), (1, 2, 3, 255)).save(vieja)
            with open(vieja + ".meta", "w") as f:
                f.write("guid: aaaa")
            copia = os.path.join(tmp2, "arte_final.json")
            shutil.copyfile(A.ARTE_FINAL_JSON, copia)
            # el arte_final.json de verdad ya lleva la cara FINAL (INC-148): la copia vuelve a ser «de antes», con la cara provisional, para las pruebas de siempre; la guarda de la
            # cara final se prueba mas abajo con otra copia
            antes_copia = A.cargar_arte_final(copia)
            for c_ in ("fuego", "rueda", "gota"):
                if "algoritm_" + c_ in antes_copia:
                    antes_copia["algoritm_" + c_]["cara_provisional"] = True
                    antes_copia["algoritm_" + c_].pop("prefijo_cara", None)
                    antes_copia["algoritm_" + c_].pop("retrato", None)
            A.guardar_arte_final(antes_copia, copia)
            antes_repo = open(A.ARTE_FINAL_JSON, "rb").read()
            reposos_repo = {p: open(os.path.join(P.PERSONAJES_ARTE, "Algoritm", "char_algoritm_%s_reposo.png" % p), "rb").read() for _, _, p in FORMAS if os.path.isfile(os.path.join(P.PERSONAJES_ARTE, "Algoritm", "char_algoritm_%s_reposo.png" % p))}
            with EnArte(base):
                caras2 = {clave: cara_de(clave, parte, None, False) for clave, _, parte in FORMAS}
                caso("la cara se extrae del _reposo de la carpeta (no del repo) para las tres formas", all(not c.errores and c.ojos is not None for c in caras2.values()))
                with contextlib.redirect_stdout(io.StringIO()):
                    fallos = aplica(formas, tr, j, holguras, eje, caras2, json_ruta=copia, escribe_tablas=False)
                partes = [os.path.join(alg, "Frontal", "char_algoritm_%s_%s.png" % (c, NOMBRE_C4[k])) for c, _, _ in FORMAS for k in NOMBRE_C4]
                caso("--aplicar escribe las 21 partes en Frontal/, del tamano de su rect", fallos == 0 and all(os.path.isfile(p) for p in partes) and
                     all(Image.open(os.path.join(alg, "Frontal", "char_algoritm_%s_%s.png" % (c, NOMBRE_C4[k]))).size == (formas[c].piezas[k].rect[2] - formas[c].piezas[k].rect[0], formas[c].piezas[k].rect[3] - formas[c].piezas[k].rect[1])
                         for c, _, _ in FORMAS for k in NOMBRE_C4))
                caso("sustituye la parte que ya estaba donde estaba, con su .meta intacto", Image.open(vieja).size == formas["fuego"].piezas[("torso", None)].norm.size and open(vieja + ".meta").read() == "guid: aaaa")
                caso("borra las seis antepiernas con su .meta", not [f for f in os.listdir(os.path.join(alg, "Frontal")) if "antepierna" in f])
                caso("escribe la cara provisional de cada forma en Expresiones/ (ojos_neutra y boca_0)",
                     all(os.path.isfile(os.path.join(alg, "Expresiones", "char_algoritm_%s_%s.png" % (c, n_))) for c, _, _ in FORMAS for n_ in ("ojos_neutra", "boca_0")))
                caso("no escribe ningun .meta", not [f for d_, _, fs in os.walk(base) for f in fs if f.endswith(".meta") and "antepierna" not in f and f != "char_algoritm_fuego_parte_torso.png.meta"])
                rel = A.cargar_arte_final(copia)
                caso("arte_final.json (la copia) trae las tres entradas, con cara_provisional y los numeros de la cara",
                     all(rel["algoritm_" + c]["cara_provisional"] is True and rel["algoritm_" + c]["registro"]["cara"]["sobre_ancha"] == cara.sobre_ancha for c, _, _ in FORMAS)
                     and all(ord(ch) < 128 for ch in open(copia, encoding="utf-8").read()) and open(A.ARTE_FINAL_JSON, "rb").read() == antes_repo)
                # una segunda pasada conserva la cara escrita
                cara_b = cara_de("fuego", "n1_fuego", rel["algoritm_fuego"], False)
                caso("una segunda pasada reutiliza la cara ya escrita (--rehaz-cara la extrae de nuevo)", getattr(cara_b, "reutilizada", False) is True
                     and not getattr(cara_de("fuego", "n1_fuego", rel["algoritm_fuego"], True), "reutilizada", True))
                with contextlib.redirect_stdout(io.StringIO()):
                    codigo = reposo(copia)
                reps = {p: Image.open(os.path.join(alg, "char_algoritm_%s_reposo.png" % p)) for _, _, p in FORMAS}
                caso("--reposo reescribe los tres _reposo en su sitio (mismo nombre), 768x768, distintos de los de antes",
                     codigo == 0 and all(r.size == (SPRITE, SPRITE) for r in reps.values()) and all(ImageChops.difference(r.convert("RGBA"), sprite_sintetico()).getbbox() is not None for r in reps.values()))
                with contextlib.redirect_stdout(io.StringIO()) as buf:
                    reposo(copia)
                caso("y es idempotente: la segunda vez dice «igual» y no reescribe", buf.getvalue().count("igual") == 3)
                # --- INC-148: la GUARDA de la cara final. Una copia donde las tres entradas ya llevan la cara final (nombres compartidos, rect, caja, prefijo_cara, retrato)
                expr = os.path.join(alg, "Expresiones")
                final = A.cargar_arte_final(copia)
                rect_f = {"Ojos": [300, 400, 700, 500], "Boca": [300, 500, 700, 600]}   # (la cara real comparte rect; aqui van separados para ver cada una)
                for c_, _, _ in FORMAS:
                    e_ = final["algoritm_" + c_]
                    e_["cara_provisional"], e_["prefijo_cara"] = False, "char_algoritm"
                    e_["registro"]["cara"] = [100, 200, 500, 600]
                    e_["retrato"] = {"base": "char_algoritm_%s_retrato_base" % c_, "cara": [0.25, 0.3, 0.5, 0.4]}
                    for n_ in e_["nodos"]:
                        if n_["nombre"] in rect_f:
                            n_["sprite"] = "char_algoritm_ojos_neutra" if n_["nombre"] == "Ojos" else "char_algoritm_boca_0"
                            n_["rect"], n_["punto"] = list(rect_f[n_["nombre"]]), [500, 500]
                A.guardar_arte_final(final, copia)
                for nombre_ in ("ojos_neutra", "boca_0"):   # la cara compartida de la copia: un rectangulo azul liso y otro rojo liso
                    Image.new("RGBA", (40, 20), (0, 0, 255, 255) if nombre_ == "ojos_neutra" else (255, 0, 0, 255)).save(os.path.join(expr, "char_algoritm_%s.png" % nombre_))
                for c_, _, _ in FORMAS:   # y las provisionales por forma que ya no deben volver
                    for nombre_ in ("ojos_neutra", "boca_0"):
                        ruta_ = os.path.join(expr, "char_algoritm_%s_%s.png" % (c_, nombre_))
                        if os.path.isfile(ruta_):
                            os.remove(ruta_)
                caras3 = {c_: cara_de(c_, parte_, final["algoritm_" + c_], False) for c_, _, parte_ in FORMAS}
                caso("INC-148: con la cara final la entrada no extrae la provisional del _reposo: cara_de devuelve la final (nodos, caja y prefijo)",
                     all(getattr(c3, "final", False) and c3.ojos is None and c3.caja == [100, 200, 500, 600] and c3.prefijo == "char_algoritm" for c3 in caras3.values()))
                caso("INC-148: --rehaz-cara se niega con la cara final (extraeria la provisional vieja)",
                     all(cara_de(c_, parte_, final["algoritm_" + c_], True).errores for c_, _, parte_ in FORMAS))
                with contextlib.redirect_stdout(io.StringIO()):
                    fallos = aplica(formas, tr, j, holguras, eje, caras2, json_ruta=copia, escribe_tablas=False)   # caras2 es la PROVISIONAL: la guarda manda sobre lo que llegue
                otra = A.cargar_arte_final(copia)
                caso("INC-148: aplicar de nuevo NO restaura la cara provisional (nodos compartidos, rect, caja, prefijo_cara, retrato y cara_provisional false se conservan)",
                     fallos == 0 and all(otra["algoritm_" + c_]["cara_provisional"] is False and otra["algoritm_" + c_]["prefijo_cara"] == "char_algoritm"
                                         and otra["algoritm_" + c_]["registro"]["cara"] == [100, 200, 500, 600] and otra["algoritm_" + c_]["retrato"] == final["algoritm_" + c_]["retrato"]
                                         and {n_["nombre"]: (n_["sprite"], n_["rect"]) for n_ in otra["algoritm_" + c_]["nodos"] if n_["nombre"] in rect_f}
                                         == {"Ojos": ("char_algoritm_ojos_neutra", rect_f["Ojos"]), "Boca": ("char_algoritm_boca_0", rect_f["Boca"])} for c_, _, _ in FORMAS))
                caso("INC-148: y no vuelve a escribir los PNG provisionales por forma",
                     not [f for f in os.listdir(expr) if any(f == "char_algoritm_%s_%s.png" % (c_, n_) for c_, _, _ in FORMAS for n_ in ("ojos_neutra", "boca_0"))])
                with contextlib.redirect_stdout(io.StringIO()):
                    codigo = reposo(copia)
                rep_f = Image.open(os.path.join(alg, "char_algoritm_n1_fuego_reposo.png")).convert("RGBA")
                oj, bo = (int(round(500 * K_SPRITE)), int(round(450 * K_SPRITE))), (int(round(500 * K_SPRITE)), int(round(550 * K_SPRITE)))   # el centro de cada rect de la cara, en el _reposo
                caso("INC-148: --reposo ensambla la cara FINAL (donde van los ojos compartidos hay azul y en la boca, rojo; no lo de la provisional)",
                     codigo == 0 and rep_f.getpixel(oj)[:3] == (0, 0, 255) and rep_f.getpixel(bo)[:3] == (255, 0, 0), "%s %s" % (rep_f.getpixel(oj), rep_f.getpixel(bo)))
                roto_ = A.cargar_arte_final(copia)
                for n_ in roto_["algoritm_fuego"]["nodos"]:
                    if n_["nombre"] == "Ojos":
                        n_["sprite"] = "char_algoritm_fuego_ojos_neutra"
                A.guardar_arte_final(roto_, copia)
                with contextlib.redirect_stdout(io.StringIO()) as buf_:
                    codigo = reposo(copia)
                caso("INC-148: una tabla que dice cara final pero nombra la provisional hace fallar --reposo (no restaura la cara vieja)", codigo == 1 and "cara final" in buf_.getvalue())
                os.remove(partes[0])
                with contextlib.redirect_stdout(io.StringIO()):
                    codigo = reposo(copia)
                caso("--reposo sin las partes (o sin --aplicar antes) falla con un mensaje y no escribe", codigo == 1)
            caso("el repo queda como estaba (arte_final.json y los _reposo, byte a byte)", open(A.ARTE_FINAL_JSON, "rb").read() == antes_repo and all(
                open(os.path.join(P.PERSONAJES_ARTE, "Algoritm", "char_algoritm_%s_reposo.png" % p), "rb").read() == b_ for p, b_ in reposos_repo.items()))

        print("== lo que se rechaza o se avisa")
        roto = os.path.join(tmp, "rotas")
        shutil.copytree(entrega, roto)
        os.remove(os.path.join(roto, "Rueda", "mano_izquierdo_rueda.png"))
        _, _, errores = lee_entrega(roto)
        caso("una pieza que falta (la mano izquierda de la rueda) se rechaza y la nombra", any("antebrazo" in e and "Rueda" in e for e in errores), str(errores))
        codigo, _ = sale([roto, "--salida", os.path.join(tmp, "sal")])
        caso("y la herramienta sale con 2 sin escribir nada", codigo == 2, str(codigo))
        sin_forma = os.path.join(tmp, "sinrueda")
        shutil.copytree(entrega, sin_forma)
        shutil.rmtree(os.path.join(sin_forma, "Rueda"))
        _, _, errores = lee_entrega(sin_forma)
        caso("una forma que falta entera (sin la carpeta Rueda) se rechaza", any("rueda" in e for e in errores), str(errores))
        otro = os.path.join(tmp, "lienzo")
        shutil.copytree(entrega, otro)
        Image.new("RGBA", (1200, 1400), (0, 0, 0, 0)).save(os.path.join(otro, "Gota", "torso_gota_algoritm.png"))
        _, _, errores = lee_entrega(otro)
        caso("un PNG de otro lienzo se rechaza", any("1300x1500" in e for e in errores), str(errores))
        distintas = os.path.join(tmp, "distintas")
        shutil.copytree(entrega, distintas)
        im = Image.open(os.path.join(distintas, "Gota", "brazo_derecho_gota.png")).convert("RGBA")
        ImageDraw.Draw(im).ellipse([930, 840, 1010, 930], fill=(110, 214, 251, 255))
        im.save(os.path.join(distintas, "Gota", "brazo_derecho_gota.png"))
        _, avisos_, errores = lee_entrega(distintas)
        caso("unas extremidades que no coinciden entre formas AVISAN (no son un error)", not errores and any("no coinciden" in a and "gota" in a for a in avisos_), str(avisos_[:2]))
        partida = os.path.join(tmp, "partida")
        shutil.copytree(entrega, partida)
        im = Image.open(os.path.join(partida, "Fuego", "pie_derecho_fuego.png")).convert("RGBA")
        im.putpixel((5, 5), (9, 9, 9, 255))   # un archivo distinto (los duplicados byte a byte se descartan antes de clasificar)
        im.save(os.path.join(partida, "Fuego", "antepierna_derecha_fuego.png"))
        _, _, errores = lee_entrega(partida)
        caso("una pierna partida (antepierna) se rechaza: la de Algoritm es entera", any("antepierna" in e for e in errores), str(errores))
        for nombre, subcarpetas, esperado in (("de la familia (Frente y Expresiones)", ("Frente", "Expresiones"), "preparar_arte_final.py"), ("de perfil", ("Perfil",), "preparar_perfil.py")):
            fam = os.path.join(tmp, "Nino_" + esperado[:12])
            for sc in subcarpetas:
                os.makedirs(os.path.join(fam, sc))
                Image.new("RGBA", LIENZO_ENTREGA, (0, 0, 0, 0)).save(os.path.join(fam, sc, "torso_nino.png"))
            codigo, err = sale([fam, "--salida", os.path.join(tmp, "sal")])
            caso("una entrega %s se rechaza y manda a %s" % (nombre, esperado), codigo == 2 and esperado in err, "%s: %s" % (codigo, err.strip()[-120:]))
        fam = os.path.join(tmp, "Mama")
        os.makedirs(fam)
        Image.new("RGBA", LIENZO_ENTREGA, (0, 0, 0, 0)).save(os.path.join(fam, "cabeza_mama.png"))
        codigo, err = sale([fam, "--salida", os.path.join(tmp, "sal")])
        caso("una carpeta con una cabeza (la familia, sin subcarpetas) se rechaza", codigo == 2 and "preparar_arte_final.py" in err, "%s: %s" % (codigo, err.strip()[-120:]))
        caso("clasifica: «PIÉ_Derecha_Gota.PNG», «Mano_Izquierdo», «TORSO», «Pierna_izq» y «pies_algoritm_izquierdo»",
             clasifica("PIÉ_Derecha_Gota.PNG") == ("pierna", "Der") and clasifica("Mano_Izquierdo.png")[0] == "antebrazo" and clasifica("TORSO_x.png")[0] == "torso"
             and clasifica("Pierna_izq.png") == ("pierna", "Izq") and clasifica("pies_algoritm_izquierdo.png") == ("pierna", "Izq"))
        caso("clasifica: una pieza de la familia (muslo, antepierna, cabeza) no es de Algoritm y una capa de cara se aparta",
             all(clasifica(n_)[0] == "partida" for n_ in ("muslo_x.png", "antepierna_x.png", "cabeza_x.png")) and clasifica("ojos_neutra.png")[0] == "cara")
        mala = os.path.join(tmp, "nombres")
        shutil.copytree(entrega, mala)
        os.rename(os.path.join(mala, "Fuego", "pie_derecho_fuego.png"), os.path.join(mala, "Fuego", "pie_izquierdo_fuego_b.png"))
        _, avisos_, errores = lee_entrega(mala)
        caso("un nombre que contradice la posicion avisa y manda la posicion", not errores and any("posicion" in a for a in avisos_), str(errores or avisos_[:1]))
        sal = os.path.join(tmp, "informe")
        antes = open(A.ARTE_FINAL_JSON, "rb").read()
        codigo, err = sale([entrega, "--salida", sal])
        caso("el informe (sin --aplicar) escribe el composite y no toca arte_final.json", codigo == 0 and os.path.isfile(os.path.join(sal, "algoritm_informe.png"))
             and open(A.ARTE_FINAL_JSON, "rb").read() == antes, "%s %s" % (codigo, err.strip()[-120:]))
    print("autoprueba de preparar_algoritm.py:", "pasa" if not malos else "FALLA en %d casos" % malos)
    return 1 if malos else 0


# ============================================================================ 9. principal


def main(argv=None):
    ap = argparse.ArgumentParser(description="Del arte final de Algoritm (INC-136) a las medidas del rig (ver la cabecera del script).")
    ap.add_argument("entrega", nargs="?", help="carpeta de la entrega, con Fuego/, Rueda/ y Gota/")
    ap.add_argument("--aplicar", action="store_true", help="escribe las partes, la cara provisional, arte_final.json y regenera las tablas")
    ap.add_argument("--reposo", action="store_true", help="reescribe en su sitio los tres char_algoritm_n*_reposo.png con el diseno nuevo y la cara vigente (la final compartida desde INC-148)")
    ap.add_argument("--rehaz-cara", action="store_true", help="extrae de nuevo la cara provisional del _reposo del repo aunque ya este escrita (se niega con la cara final)")
    ap.add_argument("--cintura", type=float, help="y (lienzo de la entrega) del pivote de Tronco")
    ap.add_argument("--salida", default=os.path.join(tempfile.gettempdir(), "algoritmia_algoritm"), help="donde dejar el composite del informe")
    ap.add_argument("--autoprueba", action="store_true", help="comprueba la herramienta con una entrega sintetica")
    a = ap.parse_args(argv)
    if a.autoprueba:
        return autoprueba()
    if a.reposo and not a.entrega and not a.aplicar:
        return reposo()
    if not a.entrega:
        ap.error("hace falta <carpeta_algoritm> (o --reposo, o --autoprueba)")
    if not os.path.isdir(a.entrega):
        ap.error("no existe la carpeta %s" % a.entrega)
    motivo = es_de_otro(a.entrega)
    if motivo:
        ap.error("la entrega " + motivo)
    os.makedirs(a.salida, exist_ok=True)
    formas, avisos, errores = lee_entrega(a.entrega)
    if errores:
        informe(formas, None, None, None, avisos, errores, None)
        return 2
    tr = transformacion(formas["fuego"])
    for f in formas.values():
        normaliza(f, tr)
    j, holguras = mide(formas["fuego"], tr[3], a.cintura)
    previos = A.cargar_arte_final()
    caras = {}
    for clave, nombre, parte in FORMAS:
        caras[clave] = cara_de(clave, parte, previos.get("algoritm_" + clave), a.rehaz_cara)
        if caras[clave].errores:
            for e in caras[clave].errores:
                errores.append("cara de %s: %s" % (nombre, e))
    if errores:
        informe(formas, tr, j, holguras, avisos, errores, None)
        return 2
    informe(formas, tr, j, holguras, avisos, errores, caras)
    entradas = {}
    for clave, _, _ in FORMAS:
        if getattr(caras[clave], "final", False):   # la cara final: los rect y los nombres de sus nodos, tal cual
            ro, rb = caras[clave].nodos["Ojos"]["rect"], caras[clave].nodos["Boca"]["rect"]
            nodos = nodos_de(clave, formas[clave], j, tr, ro, rb)
            for n in nodos:
                if n["nombre"] in caras[clave].nodos:
                    n["sprite"] = caras[clave].nodos[n["nombre"]]["sprite"]
            entradas[clave] = {"nodos": nodos}
            continue
        ro, rb = coloca_cara(formas[clave].piezas[("torso", None)], tr, caras[clave])
        entradas[clave] = {"nodos": nodos_de(clave, formas[clave], j, tr, ro, rb)}
    ruta = os.path.join(a.salida, "algoritm_informe.png")
    composite(formas, entradas, caras, ruta)
    print("\ncomposite: %s" % ruta)
    if not a.aplicar:
        print("\nescribiria (con --aplicar): las 21 partes en Frontal/, %s, arte_final.json (tres entradas), y borraria las seis antepiernas:" % (
            "la cara FINAL no se toca (INC-148)" if all(getattr(c, "final", False) for c in caras.values()) else "la cara provisional en Expresiones/"))
        for r in sobrantes():
            print("  borra     %s" % os.path.relpath(r, P.RAIZ))
        return 0
    fallos = aplica(formas, tr, j, holguras, tr[3], caras)
    if fallos:
        return 1
    if a.reposo:
        return reposo()
    return 0


if __name__ == "__main__":
    sys.exit(main())
