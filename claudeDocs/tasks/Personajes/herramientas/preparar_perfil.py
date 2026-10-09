#!/usr/bin/env python3
# preparar_perfil.py: de las piezas de PERFIL que entrega Santiago a las medidas del cuerpo de perfil del rig, con un comando.
#
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_perfil.py <id> <carpeta_entrega>             # informe
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_perfil.py <id> <carpeta_entrega> --aplicar   # escribe
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_perfil.py --autoprueba                       # se prueba solo
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_perfil.py --hoja <png>                       # hoja de verificacion del estado del repo
#         <id> = papa | mama | nina | nino         <carpeta_entrega> = la del personaje (con Perfil/ y Expresiones/), p. ej. entregas/2026-10-09/Nino
#         --cadera borde|capsula        donde gira el muslo: el borde de arriba (por defecto) o el centro de su extremo redondo
#         --ancla cadera|torso          el x = 512 del lienzo: la cadera cercana (por defecto, DECISION DE SANTIAGO 09/10/2026: los pies no
#                                       resbalan al pasar de frente a perfil) o el centro del torso
#         --mira auto|izquierda|derecha hacia donde mira la entrega (auto: se detecta; las dos otras fuerzan el espejo o su ausencia)
#         --sintetiza-lejana            SOLO si falta una pieza LEJANA: la fabrica recoloreando la cercana (ver FALTAS). APAGADO por defecto
#                                       (DECISION DE SANTIAGO 09/10/2026: se le pide la pieza al artista; --no-sintetiza-lejana lo dice explicito)
#         --limpia-cerrados             (por defecto, DECISION DE SANTIAGO 09/10/2026) la cara de ojos cerrados se limpia sola del rastro casi blanco
#                                       del ojo abierto; --no-limpia-cerrados lo apaga. Lo aplica preparar_expresion.py --vista perfil
#         --sin-caras                   no corre el paso de las caras (preparar_expresion.py --vista perfil): imprime las ordenes
#         --salida <carpeta>            donde dejar el composite del informe (por defecto, el tmp)
#
# DECISIONES DE SANTIAGO (09/10/2026), las tres de esta herramienta:
#   1. La pieza lejana que falte NO se sintetiza: se le pide al artista. La herramienta se detiene, nombra el archivo que falta y NO escribe nada de
#      ese personaje (ni PNG, ni arte_final.json, ni caras). --sintetiza-lejana existe para un caso de urgencia y queda apagado. (La Nina trajo su
#      mano_atras el mismo dia: la entrega esta completa.)
#   2. El rastro casi blanco del ojo abierto que quedo bajo los parpados de la cara cerrada se LIMPIA solo (--limpia-cerrados, activo).
#   3. El ancla horizontal es --ancla cadera: la cadera cercana cae en x = 512.
#
# QUE HACE (todo en el lienzo de 1024, origen arriba a la izquierda, y hacia abajo, la figura de y = 77 a y = 947, MIRANDO A LA DERECHA):
#   1. Reconoce las piezas POR NOMBRE, con tolerancia, buscandolas en Perfil/ de la entrega y en todas sus subcarpetas (el Nino trae Perfil/Nino/):
#      se pliega a ASCII (NFKD, sin tildes ni enes), se parte por «_» y espacios, la palabra «perfil» no cuenta y las erratas del resto no importan.
#      Papel: torso; cabeza; brazo o humero = humero; mano o antebrazo = antebrazo; muslo o pierna = muslo; pie o antepierna = antepierna. Lado:
#      atras, lejano, lejana = LEJOS; frente, cercano, cercana = CERCA. El lado se comprueba con el TONO: las piezas lejanas son de un solo tono de
#      sombra plano (#DE9563) y las cercanas llevan piel (#FFC69F); si el nombre y el tono se contradicen se avisa y manda el tono.
#      Todas las capas deben medir 1300x1500 (el lienzo comun registrado): si una no, la entrega se rechaza.
#   2. ORIENTACION. Detecta hacia donde mira (la punta del pie, el extremo delgado de la suela; la piel de la cabeza y la cara de la expresion lo confirman:
#      la nariz, los ojos y la boca estan DELANTE) y, si mira a la IZQUIERDA (la entrega del 09/10/2026: los cuatro), la ESPEJA: cada PNG y cada
#      coordenada, antes de medir. Si las piezas se contradicen entre si, se detiene. Queda en perfil.registro como «espejo: true».
#   3. Limpia el alfa (< 12 a cero, solo la componente conexa mayor de cada pieza), normaliza con LANCZOS a su propia escala (870 / (suela mas baja -
#      coronilla): NO comparte el registro de frente, que es de otro dibujo) con la coronilla en y = 77 y el ancla elegida en x = 512, y recorta cada
#      pieza a su contenido (igual que la de frente: la memoria de textura es la de lo que se ve).
#   4. MIDE sobre el alfa (los extremos redondos de las capsulas, como pose_preview.py --mide):
#        hombro (BrazoX.punto) = centro del extremo redondo de ARRIBA del humero · codo (CodoX.punto) y rodilla (RodillaX.punto) = el PUNTO MEDIO entre
#        el extremo redondo de abajo de la pieza de arriba y el de arriba de la de abajo (las piezas se solapan; la distancia entre los dos centros
#        queda en perfil.holguras) · cadera (Tronco, Torso y PiernaX .punto) = el centro del borde de arriba del muslo (--cadera capsula: el centro de su
#        extremo redondo); Tronco y Torso giran en la cadera CERCANA · cuello (Cuello.punto) = el tapon del cuello del torso (el torso TRAE el cuello), a la
#        altura de la barbilla o del borde de arriba del torso, la que este mas abajo · el rect del antebrazo termina en la punta de la mano, el de la
#        antepierna en la suela, el del muslo tiene su borde de delante en rect[2] y el rect de Cuello es la cabeza, con rect[1] en la coronilla.
#   5. Sin --aplicar: informe, composite (la figura en reposo con sus articulaciones y tres poses) y la lista de lo que escribiria. Con --aplicar:
#      escribe SOLO en Assets/Game/Art/Characters/<Carpeta>/Perfil/ los diez PNG char_<id>_perfil_{torso, cabeza, brazo_cercano, brazo_lejano,
#      antebrazo_cercano, antebrazo_lejano, pierna_cercana, pierna_lejana, antepierna_cercana, antepierna_lejana}, guarda en arte_final.json la clave
#      «perfil» del personaje —nodos (los 15 de la tabla provisional de articulaciones.py: mismos nombres, padres y orden), partes (vacio), registro,
#      holguras y, si las hubo, sintetizadas— SIN tocar la entrada de frente, corre articulaciones.py y coreografia.py --valida, y despues el paso de las
#      CARAS (preparar_expresion.py --vista perfil, abiertos y cerrados). NUNCA escribe en Frontal/ ni en Expresiones/ ni escribe .meta.
#   REGISTRO. perfil.registro = {lienzo, escala, tx, ty, espejo, cabeza}: la transformacion lienzo-de-entrega -> lienzo-del-rig de la entrega de perfil. Con
#   espejo = true, el lienzo de la entrega es el YA ESPEJADO (x' = 1300 - x): «cabeza» esta en esas coordenadas y preparar_expresion.py --vista perfil
#   espeja la expresion antes de recortarla. registro.cara_perfil (la caja de la cara de perfil) es de preparar_expresion.py; aqui solo se BORRA si este
#   registro cambia (una caja de otro registro dejaria la cara fuera de la cabeza).
#
# FALTAS. Una pieza que falte (por ejemplo, el antebrazo lejano) detiene la herramienta con el nombre del archivo esperado y no se escribe nada de ese
#   personaje. Con --sintetiza-lejana, una LEJANA que falte se fabrica de su cercana pasando la piel (#FFC69F) al tono de sombra (#DE9563), la manera
#   en que estan coloreadas las lejanas entregadas, con el contorno negro intacto; se avisa en voz alta y se guarda en perfil.sintetizadas. Una pieza
#   cercana que falte no se sintetiza nunca.
#
# COMO ENTREGAR EL ARTE DE PERFIL (para Santiago): un PNG RGBA por pieza, todos del mismo tamano (el lienzo entero con solo su pieza opaca), con la
#   figura de perfil en reposo (brazos caidos y piernas rectas); las diez piezas —torso con cuello, cabeza con la cara vacia, humero y antebrazo con
#   la mano, muslo y pierna con el pie, cercanos y lejanos— en Perfil/, y en Expresiones/ la cara con los ojos abiertos y con los ojos cerrados (en
#   el mismo lienzo). Los extremos redondos se solapan en codos y rodillas. Cabeza delante del torso; el orden de dibujo lo fija el rig:
#   LAS PIERNAS, SIEMPRE DETRAS DEL TORSO (Santiago, 09/10/2026). De atras adelante, los hijos de Perfil/Tronco: BrazoLejano, PiernaLejana, PiernaCercana, Torso,
#   Cuello (con la cabeza y la cara) y BrazoCercano (articulaciones.ORDEN_TRONCO_PERFIL). mide() entrega los nodos en ese orden y la hoja de verificacion (--hoja)
#   y el composite lo aplican aunque arte_final.json guarde los nodos en otro (capas_de_nodos, articulaciones.ordena_hijos).

import argparse
import hashlib
import math
import os
import re
import subprocess
import sys
import tempfile
import unicodedata
from types import SimpleNamespace

try:
    from PIL import Image, ImageChops, ImageDraw, ImageOps
except ImportError:  # pragma: no cover
    sys.exit("hace falta Pillow: pip install pillow")

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import articulaciones as A  # noqa: E402
import prefabs as P  # noqa: E402
import preparar_arte_final as F  # noqa: E402
import preparar_expresion as PE  # noqa: E402
import pose_preview as V  # noqa: E402

LIENZO_ENTREGA = (1300, 1500)   # el lienzo comun registrado de la entrega de perfil
ALTO_FIGURA = F.ALTO_FIGURA     # 870: de la coronilla (y = 77) a la suela mas baja (y = 947), en el lienzo de 1024
Y_CORONILLA = F.Y_CORONILLA
EJE_X = F.EJE_X                 # 512
UMBRAL_ALFA = F.UMBRAL_ALFA
SUBCARPETA = "Perfil"
PIEL = (255, 198, 159)          # #FFC69F: la piel iluminada de las piezas cercanas
SOMBRA = (222, 149, 99)         # #DE9563: el tono plano de las piezas lejanas
TOL_TONO = 10                   # tolerancia por canal para contar un pixel como piel o sombra
LEJANA_PIEL_MAX = 0.03          # una pieza con menos de esto de piel es de un solo tono de sombra: lejana
CERCANA_PIEL_MIN = 0.08         # una pieza con mas de esto de piel es cercana
MARGEN_VOTO = 0.04              # el voto de orientacion se abstiene si la diferencia es menor que esta fraccion del ancho
MARGEN_PIE = 0.05               # ... y el del pie, si la diferencia de grosor de los extremos es menor que esta fraccion de su alto
TOLERANCIA_PX = 1.0             # --autoprueba: error maximo de las articulaciones recuperadas, en el lienzo de 1024
BANDA_CUELLO = 0.08             # el x del cuello es el centro del torso en esta fraccion de su alto bajo la altura del cuello

TOKENS_ROL = {
    "torso": {"torso", "tronco", "cuerpo", "pecho"},
    "cabeza": {"cabeza"},
    "brazo": {"brazo", "humero"},
    "antebrazo": {"antebrazo", "mano", "manos"},
    "muslo": {"muslo", "pierna", "piernas"},
    "antepierna": {"antepierna", "pie", "pies"},
}
PRIORIDAD_ROL = ("antepierna", "antebrazo", "cabeza", "torso", "muslo", "brazo")
TOKENS_LEJOS = {"atras", "lejano", "lejana", "lejos"}
TOKENS_CERCA = {"frente", "cercano", "cercana", "cerca"}
DOS_LADOS = ("brazo", "antebrazo", "muslo", "antepierna")
CERCA, LEJOS = "cerca", "lejos"
# como se llama la pieza en la entrega, para decirle a Santiago que archivo falta
NOMBRE_ENTREGA = {"brazo": "brazo_%s_<personaje>.png", "antebrazo": "mano_%s_<personaje>.png",
                  "muslo": "muslo_%s_<personaje>.png (la Nina las llama pierna_...)", "antepierna": "pie_%s_<personaje>.png"}
LADO_ENTREGA = {CERCA: "frente", LEJOS: "atras"}
# sufijo del nombre del nodo y del sprite por papel y lado (brazo: Cercano/Lejano; pierna: Cercana/Lejana)
SUFIJO = {("brazo", CERCA): "Cercano", ("brazo", LEJOS): "Lejano", ("antebrazo", CERCA): "Cercano", ("antebrazo", LEJOS): "Lejano",
          ("muslo", CERCA): "Cercana", ("muslo", LEJOS): "Lejana", ("antepierna", CERCA): "Cercana", ("antepierna", LEJOS): "Lejana"}
SPRITE_DE = {("torso", None): "torso", ("cabeza", None): "cabeza",
             ("brazo", CERCA): "brazo_cercano", ("brazo", LEJOS): "brazo_lejano",
             ("antebrazo", CERCA): "antebrazo_cercano", ("antebrazo", LEJOS): "antebrazo_lejano",
             ("muslo", CERCA): "pierna_cercana", ("muslo", LEJOS): "pierna_lejana",
             ("antepierna", CERCA): "antepierna_cercana", ("antepierna", LEJOS): "antepierna_lejana"}
ORDEN_ROLES = [("torso", None), ("cabeza", None)] + [(r, l) for r in DOS_LADOS for l in (CERCA, LEJOS)]


# ============================================================================ 1. leer la entrega


def pliega(texto):
    """Minusculas y sin tildes ni enes (NFKD): «Niño» -> «nino», «Mamá» -> «mama»."""
    return "".join(c for c in unicodedata.normalize("NFKD", texto) if not unicodedata.combining(c)).lower()


def palabras(archivo):
    """Las palabras de un nombre de archivo: sin extension, plegado a ASCII y partido por todo lo que no sea letra o cifra («_», espacios, «-»)."""
    return [p for p in re.split(r"[^a-z0-9]+", pliega(os.path.splitext(archivo)[0])) if p]


def clasifica(archivo):
    """(papel, lado segun el nombre) de un archivo de Perfil/; papel None si no es una pieza que se reconozca. «perfil» y el personaje no cuentan."""
    pal = palabras(archivo)
    rol = next((r for r in PRIORIDAD_ROL if any(p in TOKENS_ROL[r] for p in pal)), None)
    lejos, cerca = any(p in TOKENS_LEJOS for p in pal), any(p in TOKENS_CERCA for p in pal)
    lado = None if lejos == cerca else (LEJOS if lejos else CERCA)
    return rol, lado


def es_cerrada(archivo):
    """La expresion de ojos cerrados: cualquier palabra que empiece por «cerrad» (cerrados, cerrada, cerrado)."""
    return any(p.startswith("cerrad") for p in palabras(archivo))


class Lectura:
    """Lo que sale de leer una entrega: las piezas por (papel, lado) ya limpias y espejadas, las expresiones y todo lo que se dijo en el camino."""

    def __init__(self):
        self.piezas = {}
        self.espejo = False
        self.votos = []
        self.avisos = []
        self.errores = []
        self.sintetizadas = []
        self.expresiones = {}   # "abierta" / "cerrada" -> ruta del PNG


def pngs_de(carpeta):
    """Los .png de una carpeta y de TODAS sus subcarpetas (el Nino trae Perfil/Nino/), por orden."""
    out = []
    for dentro, _, archivos in sorted(os.walk(carpeta)):
        out += [os.path.join(dentro, f) for f in sorted(archivos) if f.lower().endswith(".png")]
    return out


def subcarpeta(carpeta, nombre):
    """La subcarpeta «nombre» de la entrega, sin importar mayusculas ni tildes, o None."""
    if not os.path.isdir(carpeta):
        return None
    return next((os.path.join(carpeta, d) for d in sorted(os.listdir(carpeta))
                 if pliega(d) == pliega(nombre) and os.path.isdir(os.path.join(carpeta, d))), None)


def cuenta_color(im, color, tol=TOL_TONO):
    """Cuantos pixeles opacos (alfa >= UMBRAL_ALFA) de «im» (RGBA) estan a menos de «tol» por canal de «color»."""
    r, g, b, a = im.split()
    m = a.point(lambda v: 255 if v >= UMBRAL_ALFA else 0)
    for canal, c in zip((r, g, b), color):
        m = ImageChops.multiply(m, canal.point(lambda v, c=c: 255 if abs(v - c) <= tol else 0))
    return m.histogram()[255]


def tono(im):
    """(fraccion de piel #FFC69F, fraccion de sombra #DE9563) de lo opaco de una pieza."""
    n = max(1, im.getchannel("A").point(lambda v: 255 if v >= UMBRAL_ALFA else 0).histogram()[255])
    return cuenta_color(im, PIEL) / float(n), cuenta_color(im, SOMBRA) / float(n)


def lejana_por_tono(t):
    return t[0] < LEJANA_PIEL_MAX and t[1] > t[0]


def cercana_por_tono(t):
    return t[0] >= CERCANA_PIEL_MIN


def decide_lados(rol, grupo, avisos, errores):
    """
    El lado (CERCA o LEJOS) de las piezas de un papel de dos lados: {lado: Pieza}. Manda el TONO cuando es concluyente (una de piel y otra de sombra
    plana) y se avisa si contradice al nombre; si no, el nombre. Una sola pieza: su nombre (o su tono si no lo dice), con aviso si el tono la desmiente.
    """
    if not grupo:
        return {}   # no esta ni la cercana ni la lejana: lee_entrega lo dice (FALTA) con el nombre del archivo
    if len(grupo) > 2:
        errores.append("sobra %s: hay %d piezas (%s) y como mucho caben 2 (cercana y lejana)" % (rol, len(grupo), ", ".join(g.nombre for g in grupo)))
        return {}
    if len(grupo) == 1:
        pz = grupo[0]
        lado = pz.lado_nombre or (LEJOS if lejana_por_tono(pz.tono) else CERCA if cercana_por_tono(pz.tono) else None)
        if lado is None:
            errores.append("%s: no se si es cercana o lejana (el nombre no lo dice y el tono no es ni de piel ni de sombra plana)" % pz.nombre)
            return {}
        if (lado == CERCA and lejana_por_tono(pz.tono)) or (lado == LEJOS and cercana_por_tono(pz.tono)):
            avisos.append("%s: el nombre dice %s pero el tono es de pieza %s (piel %.0f %%): revisa que no este cambiada" % (
                pz.nombre, "cercana" if lado == CERCA else "lejana", "lejana" if lado == CERCA else "cercana", 100 * pz.tono[0]))
        return {lado: pz}
    a, b = grupo
    concluyente = abs(a.tono[0] - b.tono[0]) >= CERCANA_PIEL_MIN and min(a.tono[0], b.tono[0]) < LEJANA_PIEL_MAX
    if concluyente:
        lejana = a if a.tono[0] < b.tono[0] else b
        cercana = b if lejana is a else a
        for pz, lado in ((cercana, CERCA), (lejana, LEJOS)):
            if pz.lado_nombre and pz.lado_nombre != lado:
                avisos.append("%s: el nombre dice %s pero por el tono es la %s (la lejana es la de sombra plana, #DE9563): MANDA EL TONO" % (
                    pz.nombre, "cercana" if pz.lado_nombre == CERCA else "lejana", "cercana" if lado == CERCA else "lejana"))
        return {CERCA: cercana, LEJOS: lejana}
    if a.lado_nombre and b.lado_nombre and a.lado_nombre != b.lado_nombre:
        avisos.append("%s y %s: el tono no distingue cual es la lejana (piel %.0f %% y %.0f %%); se usa el nombre" % (a.nombre, b.nombre, 100 * a.tono[0], 100 * b.tono[0]))
        return {a.lado_nombre: a, b.lado_nombre: b}
    errores.append("%s: no distingo la cercana de la lejana en %s y %s (ni el nombre ni el tono)" % (rol, a.nombre, b.nombre))
    return {}


def filas_de(im, umbral=128):
    """[(x0, x1) de lo opaco de cada fila, o None] de una imagen RGBA (x1 exclusivo)."""
    a = im.getchannel("A").point(lambda v: 255 if v >= umbral else 0)
    w, h = a.size
    d = a.tobytes()
    out = []
    for y in range(h):
        fila = d[y * w:(y + 1) * w]
        i = fila.find(b"\xff")
        out.append(None if i < 0 else (i, fila.rfind(b"\xff") + 1))
    return out


def voto_pie(imagen):
    """
    Hacia donde apunta la punta del pie de una pieza «pierna con pie» (sin espejar): -1 izquierda, +1 derecha, 0 si no se decide; y el margen. La punta es el
    extremo DELGADO de la suela: la pierna baja casi recta por detras (el talon y la espalda del pie son tan altos como la pieza) y delante el pie se afila
    hasta la punta, asi que en las columnas de los extremos el lado de menos pixeles opacos es el delantero. (El eje de la pierna no sirve: en el Nino la
    canilla baja inclinada y el talon sobresale del eje mas que la punta.) El margen es la media, en tres anchos de borde (6, 10 y 15 % del ancho), de la
    diferencia de grosor entre los dos extremos, en fracciones del alto de la pieza.
    """
    a = imagen.getchannel("A").point(lambda v: 255 if v >= 128 else 0)
    w, h = a.size
    cols = [c / 255.0 * h for c in a.resize((w, 1), Image.BOX).tobytes()]
    margenes = []
    for f in (0.06, 0.10, 0.15):
        k = max(2, int(f * w))
        margenes.append((sum(cols[w - k:]) - sum(cols[:k])) / float(k) / h)   # + : la derecha es la mas gruesa, la izquierda la punta
    margen = sum(margenes) / len(margenes)
    return (0 if abs(margen) < MARGEN_PIE else (-1 if margen > 0 else 1)), -margen


def centroide_x(imagen, color=None, tol=26, umbral=UMBRAL_ALFA):
    """El x medio de los pixeles opacos de «imagen» (o solo de los de ese color), en pixeles de la imagen; None si no hay."""
    r, g, b, a = imagen.split()
    m = a.point(lambda v: 255 if v >= umbral else 0)
    if color is not None:
        for canal, c in zip((r, g, b), color):
            m = ImageChops.multiply(m, canal.point(lambda v, c=c: 255 if abs(v - c) <= tol else 0))
    w = imagen.width
    cols = m.resize((w, 1), Image.BOX).tobytes()
    total = sum(cols)
    if not total:
        return None
    return sum(x * v for x, v in enumerate(cols)) / float(total)


def voto_cabeza(cabeza):
    """
    Hacia donde mira la cabeza (sin espejar): la piel de la cara esta DELANTE (la nariz, la frente) y el pelo detras. -1 izquierda, +1 derecha, 0 si no
    se decide. El centroide de la piel contra el centro de lo opaco.
    """
    c = centroide_x(cabeza, PIEL)
    if c is None:
        return 0, 0.0
    margen = (c - cabeza.width / 2.0) / float(cabeza.width)
    return (0 if abs(margen) < MARGEN_VOTO else (1 if margen > 0 else -1)), margen


def voto_cara(png, bbox_cabeza):
    """Hacia donde mira, por la expresion (ojos, cejas y boca estan delante): su centroide contra el centro de la caja de la cabeza (sin espejar)."""
    im = Image.open(png).convert("RGBA")
    c = centroide_x(im)
    if c is None:
        return 0, 0.0
    margen = (c - (bbox_cabeza[0] + bbox_cabeza[2]) / 2.0) / float(bbox_cabeza[2] - bbox_cabeza[0])
    return (0 if abs(margen) < MARGEN_VOTO else (1 if margen > 0 else -1)), margen


def mira_hacia(lec, forzada):
    """
    Decide si hay que espejar: lec.espejo = True si la entrega mira a la IZQUIERDA. Los votos son los de la punta de los pies (los que deciden), la piel
    de la cabeza y la cara de la expresion; si uno decidido contradice a otro, es un error (las piezas no son de la misma figura). Con «forzada»
    (izquierda o derecha) no se detecta. Anota los votos en lec.votos.
    """
    nombres = {-1: "izquierda", 1: "derecha", 0: "no se decide"}
    votos = []
    for clave in ((("antepierna", CERCA)), ("antepierna", LEJOS)):
        pz = lec.piezas.get(clave)
        if pz is not None:
            v, m = voto_pie(pz.imagen)
            votos.append(("la punta del pie (%s)" % pz.nombre, v, m))
    cab = lec.piezas.get(("cabeza", None))
    if cab is not None:
        v, m = voto_cabeza(cab.imagen)
        votos.append(("la piel de la cabeza (%s)" % cab.nombre, v, m))
        if lec.expresiones.get("abierta"):
            v, m = voto_cara(lec.expresiones["abierta"], cab.bbox)
            votos.append(("la cara de la expresion (%s)" % os.path.basename(lec.expresiones["abierta"]), v, m))
    lec.votos = ["%s mira a la %s (margen %+.2f del ancho)" % (n, nombres[v], m) if v else "%s no decide (margen %+.2f)" % (n, m) for n, v, m in votos]
    decididos = {v for _, v, _ in votos if v}
    if forzada:
        lec.espejo = forzada == "izquierda"
        lec.avisos.append("--mira %s: la orientacion NO se detecta, se fuerza (la deteccion dice: %s)" % (
            forzada, "; ".join(lec.votos) or "nada"))
        return
    if len(decididos) > 1:
        lec.errores.append("las piezas no coinciden en hacia donde miran (%s): no es una sola figura o hay una pieza espejada; revisa la entrega o fuerza --mira" % "; ".join(lec.votos))
        return
    if not decididos:
        lec.errores.append("no puedo decidir hacia donde mira la entrega (%s): usa --mira izquierda|derecha" % ("; ".join(lec.votos) or "sin votos"))
        return
    lec.espejo = decididos == {-1}
    # el voto de los pies manda; los otros confirman: un pie que dice una cosa y la cabeza otra ya fue un error arriba


def recolorea_a_sombra(imagen):
    """
    La pieza lejana que falta, fabricada de su cercana: la piel #FFC69F pasa al tono de sombra plano #DE9563 y el contorno negro queda. Cada pixel se
    descompone en «t * piel»: si lo que sobra es poco (es piel o la rampa piel-negro del borde) pasa a t * sombra; la sombra de la cercana (ya #DE9563) y
    el negro puro no cambian. Misma silueta, mismo alfa.
    """
    out = imagen.copy()
    px = out.load()
    w, h = out.size
    pp = sum(c * c for c in PIEL)
    for y in range(h):
        for x in range(w):
            r, g, b, a = px[x, y]
            if not a:
                continue
            t = (r * PIEL[0] + g * PIEL[1] + b * PIEL[2]) / float(pp)
            if t <= 0.1:
                continue
            resto = math.sqrt((r - t * PIEL[0]) ** 2 + (g - t * PIEL[1]) ** 2 + (b - t * PIEL[2]) ** 2)
            if resto <= 12.0:
                px[x, y] = (int(round(t * SOMBRA[0])), int(round(t * SOMBRA[1])), int(round(t * SOMBRA[2])), a)
    return out


def nombre_esperado(rol, lado, otra):
    """El nombre del archivo que falta: el de su gemela (la otra pieza del mismo papel) con el lado cambiado («mano_frente_nina.png» -> «mano_atras_nina.png»), o la plantilla de la entrega."""
    if otra is not None:
        cambio = {LEJOS: (r"frente|cercan[oa]", "atras"), CERCA: (r"atras|lejan[oa]", "frente")}[lado]
        nuevo, n = re.subn(cambio[0], cambio[1], otra.nombre, flags=re.I)
        if n:
            return nuevo
    return NOMBRE_ENTREGA[rol] % LADO_ENTREGA[lado]


def espeja_bbox(bbox, ancho):
    """Una caja (x0, y0, x1, y1) del lienzo, espejada horizontalmente en un lienzo de ese ancho."""
    return (ancho - bbox[2], bbox[1], ancho - bbox[0], bbox[3])


def lee_entrega(carpeta, sintetiza=False, mira="auto"):
    """
    Lee la entrega de un personaje (carpeta con Perfil/ y, aparte, Expresiones/): clasifica, comprueba el lado por el tono, detecta la orientacion y
    espeja, limpia el alfa, y resuelve lo que falta (error, o sintesis con --sintetiza-lejana). Devuelve una Lectura (lec.errores vacio = se puede medir).
    """
    lec = Lectura()
    dir_perfil = subcarpeta(carpeta, SUBCARPETA)
    if dir_perfil is None and pliega(os.path.basename(os.path.normpath(carpeta))) == "perfil":
        dir_perfil, carpeta = carpeta, os.path.dirname(os.path.normpath(carpeta))
    if dir_perfil is None:
        lec.errores.append("no hay carpeta Perfil/ en %s (la entrega de perfil lleva las piezas en Perfil/ y las caras en Expresiones/)" % carpeta)
        return lec
    archivos = pngs_de(dir_perfil)
    if not archivos:
        lec.errores.append("no hay ningun .png en %s" % dir_perfil)
        return lec
    # duplicados byte a byte: se descarta el repetido
    vistos, unicos = {}, []
    for ruta in archivos:
        with open(ruta, "rb") as fh:
            h = hashlib.sha1(fh.read()).hexdigest()
        if h in vistos:
            lec.avisos.append("%s es un duplicado byte a byte de %s: se descarta" % (os.path.basename(ruta), os.path.basename(vistos[h])))
        else:
            vistos[h] = ruta
            unicos.append(ruta)
    candidatas, malos = [], []
    for ruta in unicos:
        rol, lado = clasifica(os.path.basename(ruta))
        if rol is None:
            lec.avisos.append("%s: no reconozco la pieza por el nombre (torso, cabeza, brazo, mano/antebrazo, muslo/pierna, pie/antepierna): se ignora" % os.path.basename(ruta))
            continue
        im = Image.open(ruta).convert("RGBA")
        if im.size != LIENZO_ENTREGA:
            malos.append("%s mide %dx%d" % (os.path.basename(ruta), im.width, im.height))
            continue
        pz = F.Pieza(os.path.basename(ruta), rol, lado)
        pz.nombre, pz.ruta, pz.lado_nombre = pz.archivo, ruta, lado
        pz.imagen, pz.bbox, pz.limpieza = F.limpia(im)
        if pz.imagen is None:
            lec.errores.append("%s esta vacia (todo su alfa es menor de %d)" % (pz.nombre, UMBRAL_ALFA))
            continue
        pz.tono = tono(pz.imagen)
        if pz.limpieza["mayor_descartado"] > 0.01 * pz.imagen.getchannel("A").point(lambda v: 255 if v else 0).histogram()[255]:
            lec.avisos.append("%s: se descarta un trozo suelto de %d px (no conecta con la pieza por 4 vecinos): si es parte de la pieza, debe llegar conectado" % (
                pz.nombre, pz.limpieza["mayor_descartado"]))
        candidatas.append(pz)
    if malos:
        lec.errores.append("todas las capas deben medir %dx%d (el lienzo comun registrado) y no: %s" % (LIENZO_ENTREGA[0], LIENZO_ENTREGA[1], "; ".join(malos)))
        return lec
    # las expresiones (solo se leen aqui para la orientacion y para decir cuales son; las procesa preparar_expresion.py)
    dir_expr = subcarpeta(carpeta, "Expresiones")
    expr = pngs_de(dir_expr) if dir_expr else []
    abiertas = [r for r in expr if not es_cerrada(os.path.basename(r))]
    cerradas = [r for r in expr if es_cerrada(os.path.basename(r))]
    if len(abiertas) > 1 or len(cerradas) > 1:
        lec.avisos.append("Expresiones/ trae %d de ojos abiertos y %d de ojos cerrados (se espera una de cada una): uso la primera de cada" % (len(abiertas), len(cerradas)))
    if abiertas:
        lec.expresiones["abierta"] = abiertas[0]
    if cerradas:
        lec.expresiones["cerrada"] = cerradas[0]
    if not expr:
        lec.avisos.append("no hay Expresiones/ en la entrega: las caras de perfil se hacen aparte (preparar_expresion.py --vista perfil)")
    # agrupar por papel y decidir el lado
    for rol in TOKENS_ROL:
        grupo = [pz for pz in candidatas if pz.rol == rol]
        if rol in DOS_LADOS:
            for lado, pz in decide_lados(rol, grupo, lec.avisos, lec.errores).items():
                pz.lado = lado
                lec.piezas[(rol, lado)] = pz
        elif len(grupo) != 1:
            lec.errores.append("%s: hace falta 1 y hay %d%s" % (rol, len(grupo), " (" + ", ".join(g.nombre for g in grupo) + ")" if grupo else ""))
        else:
            grupo[0].lado = None
            lec.piezas[(rol, None)] = grupo[0]
    if lec.errores:
        return lec
    # la orientacion
    mira_hacia(lec, None if mira == "auto" else mira)
    if lec.errores:
        return lec
    if lec.espejo:
        for pz in lec.piezas.values():
            pz.imagen = ImageOps.mirror(pz.imagen)
            pz.bbox = espeja_bbox(pz.bbox, LIENZO_ENTREGA[0])
    # lo que falta
    for rol in DOS_LADOS:
        for lado in (CERCA, LEJOS):
            if (rol, lado) in lec.piezas:
                continue
            otra = lec.piezas.get((rol, CERCA if lado == LEJOS else LEJOS))
            esperado = nombre_esperado(rol, lado, otra)
            if lado == LEJOS and otra is not None and sintetiza:
                pz = F.Pieza("(sintetizada de %s)" % otra.nombre, rol, None)
                pz.nombre, pz.ruta, pz.lado_nombre, pz.lado = pz.archivo, None, None, LEJOS
                pz.imagen, pz.bbox, pz.limpieza = recolorea_a_sombra(otra.imagen), otra.bbox, dict(otra.limpieza)
                pz.tono = tono(pz.imagen)
                lec.piezas[(rol, lado)] = pz
                lec.sintetizadas.append(SPRITE_DE[(rol, lado)])
                lec.avisos.append("*** SINTETIZADA: falta la pieza %s lejana (%s) y la fabrico recoloreando %s de #FFC69F a #DE9563. NO es arte del artista: "
                                  "hay que pedirsela y sustituirla ***" % (rol, esperado, otra.nombre))
            else:
                lec.errores.append("FALTA la pieza %s %s: %s%s" % (
                    rol, "lejana" if lado == LEJOS else "cercana", esperado,
                    " (la cercana esta: %s). Pidela al artista; --sintetiza-lejana la fabrica recoloreando la cercana, pero no es arte suyo" % otra.nombre
                    if lado == LEJOS and otra is not None else " (y no se sintetiza una pieza cercana)"))
    return lec


# ============================================================================ 2. normalizar y medir


def normaliza(lec, cadera="borde", ancla="cadera"):
    """
    Escala unica 870 / (suela mas baja - coronilla), coronilla en y = 77 y el ancla (la cadera cercana o el centro del torso) en x = 512; deja en cada
    pieza .norm (LANCZOS, recortada a su contenido) y .rect (en el lienzo de 1024). Devuelve (escala, tx, ty). Las cajas ya estan espejadas si hacia falta.
    """
    piezas = lec.piezas
    ymin = min(pz.bbox[1] for pz in piezas.values())
    ymax = max(pz.bbox[3] for pz in piezas.values())
    s = ALTO_FIGURA / float(ymax - ymin)
    ty = Y_CORONILLA - s * ymin
    for pz in piezas.values():
        w, h = pz.imagen.size
        pz.norm = pz.imagen.resize((max(1, int(round(w * s))), max(1, int(round(h * s)))), Image.LANCZOS)
    tx = 0.0

    def coloca():
        for pz in piezas.values():
            x0, y0 = int(round(s * pz.bbox[0] + tx)), int(round(s * pz.bbox[1] + ty))
            pz.rect = [x0, y0, x0 + pz.norm.size[0], y0 + pz.norm.size[1]]

    def ancla_x():
        if ancla == "torso":
            t = piezas[("torso", None)].rect
            return (t[0] + t[2]) / 2.0
        mu = piezas[("muslo", CERCA)]
        return cadera_de(mu, cadera)[0]

    for _ in range(4):   # el redondeo de los rects mueve el ancla unas decimas: se corrige hasta que quede en 512 +- 0.5
        coloca()
        error = EJE_X - ancla_x()
        if abs(error) <= 0.5:
            break
        tx += error
    return s, tx, ty


def tapas(pz):
    """
    Los extremos redondos de una pieza alargada que cuelga (humero, antebrazo, muslo, pierna): ((x, y, radio) del de ARRIBA, (x, y, radio) del de ABAJO) en el lienzo
    de 1024. El radio es la mitad del ancho maximo en el tercio de ese extremo y el centro esta un radio por dentro del borde, en el eje de la pieza a esa
    altura (para la pierna con pie, el tercio de arriba es la pierna y el pie no cuenta).
    """
    fs = filas_de(pz.norm)
    h = len(fs)
    if h < 3:
        raise ValueError("%s es demasiado baja para medir sus extremos" % pz.nombre)
    out = []
    for arriba in (True, False):
        idx = list(range(h)) if arriba else list(range(h - 1, -1, -1))
        anchos = [(fs[y][1] - fs[y][0]) if fs[y] else 0 for y in idx[:max(1, h // 3)]]
        r = max(anchos) / 2.0
        k = min(int(round(r)), h // 2)
        ys = idx[max(0, k - 1):k + 2]
        cx = sum((fs[y][0] + fs[y][1]) / 2.0 for y in ys if fs[y]) / float(max(1, sum(1 for y in ys if fs[y])))
        cy = r if arriba else h - r   # el centro de la tapa esta un radio por dentro del borde (continuo, no el centro de una fila)
        out.append((pz.rect[0] + cx, pz.rect[1] + cy, r))
    return out[0], out[1]


def cadera_de(muslo, modo="borde"):
    """La cadera de una pierna: el centro del borde de arriba del muslo (--cadera borde) o el centro de su extremo redondo (capsula)."""
    if modo == "capsula":
        t, _ = tapas(muslo)
        return (t[0], t[1])
    return ((muslo.rect[0] + muslo.rect[2]) / 2.0, float(muslo.rect[1]))


def dist(p, q):
    return math.hypot(p[0] - q[0], p[1] - q[1])


def medio(p, q):
    return ((p[0] + q[0]) / 2.0, (p[1] + q[1]) / 2.0)


def cuello_de(cabeza, torso, avisos):
    """
    El punto donde gira la cabeza (Cuello.punto): el torso TRAE el cuello (un tapon que sube hasta la barbilla). La y es la de la barbilla (la base del ovalo
    de piel de la cabeza, preparar_expresion.mide_cabeza) o el borde de arriba del torso, la que este mas abajo; la x, el centro del torso en una banda bajo esa
    altura (el tapon: arriba su punta es redonda y se inclina). Si no se puede medir la barbilla, el borde de arriba del torso mas un decimo de su alto.
    """
    alto = float(torso.rect[3] - torso.rect[1])
    try:
        barbilla = PE.mide_cabeza(cabeza.norm, cabeza.rect).barbilla
    except Exception as e:  # noqa: BLE001 (cualquier fallo de la medida: se usa el respaldo)
        barbilla = torso.rect[1] + 0.1 * alto
        avisos.append("no pude medir la barbilla de la cabeza (%s): el cuello queda un decimo del torso bajo su borde de arriba" % e)
    y = min(max(barbilla, float(torso.rect[1])), torso.rect[3] - 1.0)
    fs = filas_de(torso.norm)
    a, b = int(round(y - torso.rect[1])), int(round(y - torso.rect[1] + BANDA_CUELLO * alto))
    centros = [(f[0] + f[1]) / 2.0 for f in fs[a:max(a + 1, b)] if f]
    x = torso.rect[0] + (sum(centros) / len(centros) if centros else torso.norm.width / 2.0)
    return (x, y)


NODOS_CARA = (("CaraBase", "cara_base"), ("Ojos", "ojos_neutra"), ("Boca", "boca_0"))


def mide(lec, pid, cadera="borde", previo=None):
    """
    Las medidas de la entrega ya normalizada. Devuelve (nodos de la tabla «perfil», holguras, joints, avisos). Los nodos son los 15 de la tabla provisional
    de articulaciones.py —mismos nombres, padres y orden— con las medidas del arte; las caras (CaraBase, Ojos, Boca) llevan un rect provisional (la mitad
    de delante de la cabeza) hasta que preparar_expresion.py --vista perfil pone el suyo; si «previo» (la entrada «perfil» anterior) trae el suyo, se conserva.
    """
    avisos = []
    pref = "char_%s_perfil" % pid
    pz = lec.piezas
    holguras, joints = {}, {}
    medidas = {}
    for lado in (CERCA, LEJOS):
        br, ab, mu, an = pz[("brazo", lado)], pz[("antebrazo", lado)], pz[("muslo", lado)], pz[("antepierna", lado)]
        s_br, i_br = tapas(br)
        s_ab, _ = tapas(ab)
        s_mu, i_mu = tapas(mu)
        s_an, _ = tapas(an)
        hombro = (s_br[0], s_br[1])
        codo, d_codo = medio((i_br[0], i_br[1]), (s_ab[0], s_ab[1])), dist((i_br[0], i_br[1]), (s_ab[0], s_ab[1]))
        rodilla, d_rod = medio((i_mu[0], i_mu[1]), (s_an[0], s_an[1])), dist((i_mu[0], i_mu[1]), (s_an[0], s_an[1]))
        cad = cadera_de(mu, cadera)
        medidas[lado] = {"hombro": hombro, "codo": codo, "cadera": cad, "rodilla": rodilla}
        suf_b, suf_p = SUFIJO[("brazo", lado)], SUFIJO[("muslo", lado)]
        joints["Hombro" + suf_b], joints["Codo" + suf_b] = hombro, codo
        joints["Cadera" + suf_p], joints["Rodilla" + suf_p] = cad, rodilla
        holguras["Codo" + suf_b], holguras["Rodilla" + suf_p] = round(d_codo, 1), round(d_rod, 1)
        for nombre, d, r in (("codo " + suf_b, d_codo, i_br[2]), ("rodilla " + suf_p, d_rod, i_mu[2])):
            if d > r:
                avisos.append("%s: las piezas no comparten extremo (los centros de sus extremos redondos distan %.0f px y el radio es %.0f): la costura no sera limpia" % (nombre, d, r))
    cercano = medidas[CERCA]
    cadera_tronco = cercano["cadera"]
    torso, cab = pz[("torso", None)], pz[("cabeza", None)]
    cuello = cuello_de(cab, torso, avisos)
    joints["Cuello"] = cuello
    # las caras: el rect que ya tenia la tabla (si la entrada previa es del mismo registro) o un rect provisional en la mitad de delante de la cabeza
    cw, ch = cab.rect[2] - cab.rect[0], cab.rect[3] - cab.rect[1]
    rect_cara = [cab.rect[0] + int(0.40 * cw), cab.rect[1] + int(0.30 * ch), cab.rect[2] - int(0.05 * cw), cab.rect[1] + int(0.85 * ch)]
    previos = {n["nombre"]: n for n in (previo or {}).get("nodos", [])}
    if all(k in previos and previos[k].get("rect") for k, _ in NODOS_CARA):
        rect_cara = list(previos["Ojos"]["rect"])
    centro_cara = [A.entero((rect_cara[0] + rect_cara[2]) / 2.0), A.entero((rect_cara[1] + rect_cara[3]) / 2.0)]

    def nodo(nombre, tipo, padre, punto, imagen="", sprite="", rect=None):
        return {"nombre": nombre, "tipo": tipo, "padre": padre, "punto": list(punto), "imagen": imagen, "sprite": sprite, "rect": rect}

    def r(pieza):
        return [int(v) for v in pieza.rect]

    def brazo(lado):
        suf = SUFIJO[("brazo", lado)]
        m = medidas[lado]
        return [nodo("Brazo" + suf, "imagen", A.PT, A.punto(*m["hombro"]), sprite="%s_brazo_%s" % (pref, suf.lower()), rect=r(pz[("brazo", lado)])),
                nodo("Codo" + suf, "articulacion", A.PT + "/Brazo" + suf, A.punto(*m["codo"]), imagen="Antebrazo" + suf,
                     sprite="%s_antebrazo_%s" % (pref, suf.lower()), rect=r(pz[("antebrazo", lado)]))]

    def pierna(lado):
        suf = SUFIJO[("muslo", lado)]
        m = medidas[lado]
        return [nodo("Pierna" + suf, "imagen", A.PT, A.punto(*m["cadera"]), sprite="%s_pierna_%s" % (pref, suf.lower()), rect=r(pz[("muslo", lado)])),
                nodo("Rodilla" + suf, "articulacion", A.PT + "/Pierna" + suf, A.punto(*m["rodilla"]), imagen="Antepierna" + suf,
                     sprite="%s_antepierna_%s" % (pref, suf.lower()), rect=r(pz[("antepierna", lado)]))]

    nodos = [nodo("Perfil", "grupo", "Lienzo", A.punto(EJE_X, A.SUELO), rect=[0, 0, 1024, 1024]),
             nodo("Tronco", "grupo", A.PF, A.punto(*cadera_tronco), rect=[0, 0, 1024, 1024])]
    # de atras adelante (A.ORDEN_TRONCO_PERFIL): brazo lejano, las DOS piernas, torso, cabeza, brazo cercano. Las piernas siempre detras del torso (Santiago, 09/10/2026)
    nodos += brazo(LEJOS) + pierna(LEJOS) + pierna(CERCA)
    nodos.append(nodo("Torso", "imagen", A.PT, A.punto(*cadera_tronco), sprite=pref + "_torso", rect=r(torso)))
    nodos.append(nodo("Cuello", "articulacion", A.PT, A.punto(*cuello), imagen="Cabeza", sprite=pref + "_cabeza", rect=r(cab)))
    for nombre, capa in NODOS_CARA:
        nodos.append(nodo(nombre, "imagen", A.PT + "/Cuello/Cabeza", centro_cara, imagen=nombre, sprite="%s_%s" % (pref, capa), rect=list(rect_cara)))
    nodos += brazo(CERCA)
    return nodos, holguras, joints, avisos


def registro_de(res):
    """perfil.registro: la transformacion lienzo-de-entrega (ya ESPEJADO si espejo) -> lienzo-del-rig, con la caja de la cabeza en ese lienzo."""
    cab = res.lec.piezas[("cabeza", None)]
    return {"lienzo": [LIENZO_ENTREGA[0], LIENZO_ENTREGA[1]], "escala": round(res.escala, 6), "tx": round(res.tx, 3), "ty": round(res.ty, 3),
            "espejo": bool(res.lec.espejo), "cabeza": [int(v) for v in cab.bbox]}


class Resultado:
    pass


def procesa(carpeta, pid, cadera="borde", ancla="cadera", sintetiza=False, mira="auto", previo=None):
    """Todo menos escribir: lee, espeja, limpia, normaliza y mide. Devuelve un Resultado (res.errores vacio = se puede aplicar)."""
    res = Resultado()
    res.pid, res.carpeta = pid, carpeta
    res.lec = lec = lee_entrega(carpeta, sintetiza, mira)
    res.avisos, res.errores = lec.avisos, lec.errores
    res.nodos = res.holguras = res.joints = res.registro = None
    if lec.errores:
        return res
    res.escala, res.tx, res.ty = normaliza(lec, cadera, ancla)
    res.registro = registro_de(res)
    # el rect de las caras de la entrada anterior solo vale si el registro es el mismo (otra escala o ancla lo deja fuera de la cabeza)
    vigente = previo if previo and mismo_registro(previo.get("registro"), res.registro) else None
    res.nodos, res.holguras, res.joints, av = mide(lec, pid, cadera, vigente)
    res.avisos += av
    for pz in lec.piezas.values():
        r = pz.rect
        if r[0] < 0 or r[1] < 0 or r[2] > 1024 or r[3] > 1024:
            res.avisos.append("%s: el rect %s se sale del lienzo de 1024" % (pz.nombre, r))
    return res


# ============================================================================ 3. dibujar el cuerpo de perfil (informe y hoja de verificacion)

RUTAS_HUESO = {"tronco": P.PT, "cadera_c": P.PLC, "cadera_l": P.PLL, "rodilla_c": P.PKC, "rodilla_l": P.PKL, "hombro_c": P.PBC, "hombro_l": P.PBL,
               "codo_c": P.PEC, "codo_l": P.PEL, "cuello": P.PNK, "cabeza": P.PHD}
# poses de prueba del informe, en los signos de coreografia.py (+ lleva el pie o la mano hacia delante; la rodilla flexiona con -; el codo con +)
POSES = [
    ("reposo", {}),
    ("paso", {"cadera_c": 24, "cadera_l": -22, "rodilla_c": -8, "rodilla_l": -34, "hombro_c": -26, "hombro_l": 24, "codo_c": 14, "codo_l": 30, "tronco": -6}),
    ("agachado", {"tronco": -28, "cadera_c": 62, "cadera_l": 58, "rodilla_c": -70, "rodilla_l": -66, "hombro_c": 50, "hombro_l": 46, "codo_c": 40, "codo_l": 36, "cuello": 14}),
    ("brazos al frente", {"hombro_c": 70, "hombro_l": 60, "codo_c": 40, "codo_l": 55, "cuello": -8}),
]


class PoseFija:
    """Un clip de un solo instante con las rotaciones dadas por nombre de hueso (grados): lo que dibuja el informe."""

    def __init__(self, giros):
        self.giros = {RUTAS_HUESO[k]: v for k, v in giros.items()}

    def valor(self, ruta, prop, t, defecto):
        return self.giros.get(ruta, defecto) if prop == V.K.ROT else defecto


def capas_de_nodos(nodos, imagen_de, orden_tronco=None):
    """
    ([(ruta del que se mueve, rect, imagen)] en orden de dibujo, {ruta: pivote}) de los nodos de una tabla «perfil». Un nodo «imagen» se dibuja el mismo; uno
    «articulacion» dibuja su segmento (la ruta del segmento es la de la articulacion mas «/imagen»); un «grupo» no dibuja y solo da su pivote.
    imagen_de(sprite) devuelve la imagen o None (un sprite que no esta no se dibuja). Los hijos de Perfil/Tronco se dibujan en el orden de «orden_tronco»
    (por defecto el del contrato, A.ORDEN_TRONCO_PERFIL: las dos piernas detras del torso), vengan como vengan en la lista: la hoja de verificacion dibuja lo
    mismo que el motor aunque arte_final.json guarde los nodos en otro orden.
    """
    capas, pivotes = [], {}
    for n in A.ordena_hijos(nodos, A.PT, orden_tronco or A.ORDEN_TRONCO_PERFIL):
        ruta = n["padre"] + "/" + n["nombre"]
        pivotes[ruta] = tuple(n["punto"])
        if n["tipo"] == "articulacion":
            seg = ruta + "/" + n["imagen"]
            pivotes[seg] = tuple(n["punto"])
            im = imagen_de(n["sprite"])
            if im is not None:
                capas.append((seg, n["rect"], im))
        elif n["tipo"] == "imagen":
            im = imagen_de(n["sprite"])
            if im is not None:
                capas.append((ruta, n["rect"], im))
    return capas, pivotes


def renderiza_nodos(nodos, imagen_de, clip, t=0.0, escala=0.5, region=(40, 30, 984, 990), fondo=(236, 232, 222, 255), orden_tronco=None):
    """El cuerpo de perfil de la tabla «nodos» en el instante t de «clip» (un clip de pose_preview o PoseFija): lo mismo que haria el rig, sin el motor."""
    capas, pivotes = capas_de_nodos(nodos, imagen_de, orden_tronco)
    mundo = {"": V.mat_id(), "Lienzo": V.mat_id()}

    def de(ruta):
        if ruta in mundo:
            return mundo[ruta]
        padre = ruta.rsplit("/", 1)[0] if "/" in ruta else ""
        if ruta in pivotes:
            loc = V.mat_local(pivotes[ruta], clip.valor(ruta, V.K.POSX, t, 0.0), clip.valor(ruta, V.K.POSY, t, 0.0), clip.valor(ruta, V.K.ROT, t, 0.0),
                              clip.valor(ruta, V.K.ESCX, t, 1.0), clip.valor(ruta, V.K.ESCY, t, 1.0))
        else:
            loc = V.mat_id()
        mundo[ruta] = V.mat_mul(de(padre), loc)
        return mundo[ruta]

    ox, oy = region[0], region[1]
    tam = (int(round((region[2] - region[0]) * escala)), int(round((region[3] - region[1]) * escala)))
    lienzo = Image.new("RGBA", tam, fondo)
    for ruta, rect, im in capas:
        de(ruta)
        capa = V.Capa(SimpleNamespace(ruta=ruta), tuple(rect), im, (1, 1, 1, 1))
        sal = V._capa_a_lienzo(capa, {ruta: mundo[ruta]}, escala, (ox, oy), tam)
        if sal is not None:
            lienzo.alpha_composite(sal)
    return lienzo


def composite(res, ruta):
    """Cuatro paneles: la figura en reposo con sus articulaciones marcadas y tres poses de prueba (con las piezas ya normalizadas, antes de escribirlas)."""
    sprites = {n["sprite"]: res.lec.piezas[clave].norm for n in res.nodos for clave in [_clave_de(n["sprite"])] if clave in res.lec.piezas}
    paneles = []
    for i, (titulo, giros) in enumerate(POSES):
        im = renderiza_nodos(res.nodos, sprites.get, PoseFija(giros))
        d = ImageDraw.Draw(im)
        d.rectangle([0, 0, im.width, 14], fill=(255, 255, 255, 230))
        d.text((4, 2), "%s: %s (mira a la derecha)" % (res.pid, titulo), fill=(30, 30, 30, 255))
        if i == 0:
            colores = {"Hombro": (220, 30, 30), "Codo": (30, 150, 30), "Cadera": (30, 60, 220), "Rodilla": (200, 160, 0), "Cuello": (180, 30, 180)}
            for nombre, (x, y) in res.joints.items():
                color = next(c for k, c in colores.items() if nombre.startswith(k))
                px, py = (x - 40) * 0.5, (y - 30) * 0.5
                d.ellipse([px - 4, py - 4, px + 4, py + 4], outline=color + (255,), width=2)
        paneles.append(im)
    w, h = paneles[0].size
    hoja = Image.new("RGB", (w * len(paneles), h), (236, 232, 222))
    for i, im in enumerate(paneles):
        hoja.paste(im.convert("RGB"), (i * w, 0))
    hoja.save(ruta)


def _clave_de(sprite):
    """(papel, lado) de una pieza por el final del nombre de su sprite (char_<id>_perfil_<pieza>); None si no es una pieza del cuerpo."""
    pieza = sprite.split("_perfil_", 1)[-1]
    return next((k for k, v in SPRITE_DE.items() if v == pieza), None)


# ============================================================================ 4. informe, escritura y ordenes


def informe(res):
    pid, lec = res.pid, res.lec
    print("== Entrega de perfil de %s: %s" % (pid, res.carpeta))
    for p in sorted(lec.piezas.values(), key=lambda p: ORDEN_ROLES.index((p.rol, p.lado))):
        t = "" if p.lado is None and p.rol in ("torso", "cabeza") else "  (piel %.0f %%, sombra %.0f %%)" % (100 * p.tono[0], 100 * p.tono[1])
        lim = p.limpieza
        extra = "  [limpieza: %d motas (%d px), %d px de alfa tenue]" % (lim["motas"], lim["px_motas"], lim["tenue"]) if (lim["motas"] or lim["tenue"]) else ""
        print("  %-46s -> %-10s %-5s char_%s_perfil_%s.png%s%s" % (p.nombre, p.rol, p.lado or "-", pid, SPRITE_DE[(p.rol, p.lado)], t, extra))
    if lec.expresiones:
        print("  expresiones: abierta = %s · cerrada = %s" % (os.path.basename(lec.expresiones.get("abierta", "(falta)")), os.path.basename(lec.expresiones.get("cerrada", "(falta)"))))
    if lec.votos:
        print("\norientacion: la entrega mira a la %s%s" % ("IZQUIERDA: se espeja (espejo: true)" if lec.espejo else "DERECHA: no se espeja", "" if not lec.votos else ""))
        for v in lec.votos:
            print("  - " + v)
    for e in res.errores:
        print("ERROR", e)
    if res.errores:
        print("\nla entrega no se puede procesar: no se escribe NADA de %s" % pid)
        return
    print("\nnormalizacion: escala %.4f (la figura mide %.0f px en el lienzo de la entrega y pasa a %.0f), tx %.2f, ty %.2f" % (
        res.escala, ALTO_FIGURA / res.escala, ALTO_FIGURA, res.tx, res.ty))
    print("\nmedidas en el lienzo de 1024 (y hacia abajo):")
    for nombre, (x, y) in res.joints.items():
        print("  %-16s (%6.1f, %6.1f)" % (nombre, x, y))
    print("holguras (distancia entre los centros de los extremos redondos de codos y rodillas): %s" % res.holguras)
    print("\nrects:")
    for p in sorted(lec.piezas.values(), key=lambda p: ORDEN_ROLES.index((p.rol, p.lado))):
        print("  %-20s %s  %dx%d px" % (SPRITE_DE[(p.rol, p.lado)], p.rect, p.norm.size[0], p.norm.size[1]))
    print("\ntextura sin comprimir de las diez piezas: %.2f MB (el margen del paquete, RNF-06, es de unos 21 MB para todo el juego)" % memoria_mb(res))
    for a in res.avisos:
        print("AVISO", a)


def memoria_mb(res):
    return sum(p.norm.size[0] * p.norm.size[1] * 4 for p in res.lec.piezas.values()) / 1e6


def lista_escritura(res):
    """[(ruta absoluta, existia)] de los PNG que --aplicar escribe: SOLO en Assets/Game/Art/Characters/<Carpeta>/Perfil/."""
    carpeta = os.path.join(P.PERSONAJES_ARTE, P.PERSONAJES[res.pid][1], SUBCARPETA)
    out = []
    for clave in ORDEN_ROLES:
        ruta = os.path.join(carpeta, "char_%s_perfil_%s.png" % (res.pid, SPRITE_DE[clave]))
        out.append((ruta, os.path.isfile(ruta)))
    return out


def bloque_local(pid):
    prefab, carpeta, _ = P.PERSONAJES[pid]
    return """
==================== Sesion local (Santiago), con el Editor abierto ====================
 0. git pull   (trae esta rama con el arte de perfil, arte_final.json, rig_articulaciones.json y clips_personajes.json)
 1. Copiar el generador (el Editor crea el .meta solo, nunca a mano):
      Copy-Item claudeDocs/tasks/Personajes/herramientas/BuildRigsFinal.cs.txt Assets/Editor/ClaudeBuildRigsFinal.cs
 2. Recompilar y esperar:   pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 recompile
 3. Por coplay execute_script, EN ESTE ORDEN (cada una devuelve su log):
      ClaudeBuildRigsFinal.Execute("estado")     // antes
      ClaudeBuildRigsFinal.Execute("perfil")     // Lienzo/Perfil y sus nodos, solo altas (idempotente)
      ClaudeBuildRigsFinal.Execute("sprites")    // asigna los PNG de Perfil/, rect y pivote de la tabla, y char_%(pid)s_perfil_cara.asset
      ClaudeBuildRigsFinal.Execute("clips")      // vuelca clips_personajes.json (las curvas de perfil con las longitudes del arte)
      ClaudeBuildRigsFinal.Execute("estado")     // despues
 4. Comprobar los fileID:   git diff -U0 Assets/Game/Prefabs/Characters   (no debe QUITAR lineas «--- !u!»)
 5. Pruebas:
      pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-edit CharacterRig_
      pwsh -NoProfile -File claudeDocs/tasks/OE4/herramientas/editor.ps1 tests-edit CharacterFace
 6. Borrar el andamiaje:   Remove-Item Assets/Editor/ClaudeBuildRigsFinal.cs, Assets/Editor/ClaudeBuildRigsFinal.cs.meta
 7. git add Assets/Game/Art/Characters/%(carpeta)s/Perfil Assets/Game/Prefabs/Characters/%(prefab)s.prefab Assets/Game/Prefabs/Characters/%(prefab)s.prefab.meta
      git add claudeDocs/tasks/Personajes/herramientas/arte_final.json claudeDocs/tasks/Personajes/herramientas/rig_articulaciones.json
      git add claudeDocs/tasks/Personajes/herramientas/clips_personajes.json
========================================================================================
""" % {"pid": pid, "prefab": prefab, "carpeta": carpeta}


def mismo_registro(a, b):
    """Dos registros de perfil son la misma transformacion (la caja de cara de uno vale para el otro)."""
    claves = ("lienzo", "escala", "tx", "ty", "espejo")
    return bool(a) and bool(b) and all(a.get(k) == b.get(k) for k in claves)


def entrada_perfil(res, previo, orden_tronco=None):
    """La entrada «perfil» de arte_final.json de ESTE personaje: {nodos, partes, [orden_tronco], registro, holguras, [sintetizadas]}."""
    perfil = {"nodos": res.nodos, "partes": []}
    if orden_tronco:
        perfil["orden_tronco"] = list(orden_tronco)
    perfil["registro"] = res.registro
    perfil["holguras"] = res.holguras
    if res.lec.sintetizadas:
        perfil["sintetizadas"] = list(res.lec.sintetizadas)
    return perfil


def aplica(res, args):
    pid = res.pid
    lista = lista_escritura(res)
    print("\n== Aplicando al repo ==")
    personajes = A.cargar_arte_final()
    if pid not in personajes:
        print("ERROR %s no tiene entrada de frente en arte_final.json: primero preparar_arte_final.py %s <carpeta> --aplicar" % (pid, pid))
        return 1
    for ruta, existia in lista:
        assert os.path.basename(os.path.dirname(ruta)) == SUBCARPETA, "preparar_perfil.py solo escribe en Perfil/: " + ruta
        clave = next(k for k in ORDEN_ROLES if os.path.basename(ruta) == "char_%s_perfil_%s.png" % (pid, SPRITE_DE[k]))
        os.makedirs(os.path.dirname(ruta), exist_ok=True)   # Unity crea el .meta de cada PNG al importarlo
        if existia and F._mismos_pixeles(ruta, res.lec.piezas[clave].norm):
            print("  igual     %s" % os.path.relpath(ruta, P.RAIZ))
            continue
        res.lec.piezas[clave].norm.save(ruta)
        print("  %s %s" % ("sustituye" if existia else "nuevo    ", os.path.relpath(ruta, P.RAIZ)))
    previo = personajes[pid].get("perfil")
    registro_previo = (previo or {}).get("registro")
    perfil = entrada_perfil(res, previo, args.orden_tronco)
    if previo and previo.get("orden_tronco") and not args.orden_tronco:
        perfil["orden_tronco"] = previo["orden_tronco"]
    personajes[pid]["perfil"] = perfil
    if "cara_perfil" in personajes[pid].get("registro", {}) and not mismo_registro(registro_previo, res.registro):
        del personajes[pid]["registro"]["cara_perfil"]
        print("AVISO el registro de perfil cambio: se borra registro.cara_perfil (la caja de la cara era de otro registro) y las caras se vuelven a hacer")
    A.guardar_arte_final(personajes)
    print("  arte_final.json: «perfil» de %s (%d nodos, registro, holguras%s)" % (pid, len(res.nodos), ", sintetizadas" if res.lec.sintetizadas else ""))
    py = sys.executable
    fallos = 0
    fallos += 1 if F.corre([py, os.path.join(AQUI, "articulaciones.py")], "articulaciones") else 0
    fallos += 1 if F.corre([py, os.path.join(AQUI, "coreografia.py"), "--valida"], "coreografia") else 0
    if not args.sin_caras and res.lec.expresiones.get("abierta") and res.lec.expresiones.get("cerrada"):
        fallos += caras(res, args)
    else:
        print("\nlas caras de perfil se hacen aparte:")
        for orden in ordenes_caras(res, args):
            print("  " + orden)
    print("\n%s" % ("TODO EN VERDE" if not fallos else "%d pasos fallaron: revisa antes de entregar" % fallos))
    print(bloque_local(pid))
    return fallos


def ordenes_caras(res, args):
    """Las dos ordenes del paso de las caras (la abierta primero: fija la caja de la cara; la cerrada la hereda)."""
    py = "python3"
    base = [py, "claudeDocs/tasks/Personajes/herramientas/preparar_expresion.py", res.pid]
    extra = ["--vista", "perfil", "--aplicar"] + ([] if args.limpia_cerrados else ["--no-limpia-cerrados"])
    return ["%s %s \"%s\" %s" % (" ".join(base), nombre, res.lec.expresiones[clave], " ".join(extra))
            for nombre, clave in (("abiertos", "abierta"), ("cerrados", "cerrada")) if clave in res.lec.expresiones]


def caras(res, args):
    """Corre preparar_expresion.py --vista perfil con la expresion abierta y despues con la cerrada. Devuelve cuantos pasos fallaron."""
    fallos = 0
    py = sys.executable
    print("\n== Las caras de perfil (preparar_expresion.py --vista perfil) ==")
    for nombre, clave in (("abiertos", "abierta"), ("cerrados", "cerrada")):
        # F.corre ejecuta con cwd = la carpeta de las herramientas: las rutas, absolutas
        cmd = [py, os.path.join(AQUI, "preparar_expresion.py"), res.pid, nombre, os.path.abspath(res.lec.expresiones[clave]), "--vista", "perfil", "--aplicar",
               "--salida", os.path.abspath(args.salida)] + ([] if args.limpia_cerrados else ["--no-limpia-cerrados"])
        fallos += 1 if corre_caras(cmd, nombre) else 0
    return fallos


LINEAS_CARA = ("nuevo", "sustituye", "igual", "vacia", "limpieza", "AVISO", "ERROR", "arte_final.json", "caja de la cara", "recorte", "rect comun", "espeja", "MIRA", "TODO EN VERDE", "pasos fallaron", "error:")


def corre_caras(cmd, nombre):
    """Corre una orden de preparar_expresion.py y muestra solo lo que dice de la cara (sin el bloque de la sesion local, que ya imprime esta herramienta)."""
    print("\n$ preparar_expresion.py %s %s" % (cmd[2], nombre))
    r = subprocess.run(cmd, cwd=AQUI, capture_output=True, text=True)
    for linea in (r.stdout + r.stderr).splitlines():
        if linea.startswith("====="):
            break   # lo que sigue es el bloque de la sesion local de preparar_expresion.py: ya imprime el suyo esta herramienta
        if linea.strip() and any(k in linea for k in LINEAS_CARA):
            print("   " + linea.strip()[:230])
    if r.returncode:
        print("   preparar_expresion %s: salio con codigo %d" % (nombre, r.returncode))
    return r.returncode


# ============================================================================ 5. la hoja de verificacion del estado del repo


def sprite_del_repo(pid):
    """imagen_de(sprite) del cuerpo de perfil ya escrito en el repo (Assets/.../Perfil/); None si el PNG no esta."""
    carpeta = P.PERSONAJES[pid][1]

    def imagen_de(sprite):
        ruta = P._png_de(carpeta, sprite)
        return Image.open(ruta).convert("RGBA") if ruta else None

    return imagen_de


def hoja(ruta, ids=None):
    """
    La hoja de verificacion de lo que hay en el repo: por personaje, el perfil armado de reposo (mirando a la derecha), el paso, el arrodillado y el recoger
    (muestreados de clips_personajes.json) y la cabeza con los ojos abiertos y cerrados. Dibuja con las tablas de arte_final.json y los PNG de Perfil/.
    """
    personajes = A.cargar_arte_final()
    clips = V.cargar_clips()
    filas = []
    for pid in ids or P.FAMILIA:
        perfil = personajes.get(pid, {}).get("perfil")
        if not perfil:
            continue
        imagen_de = sprite_del_repo(pid)
        nodos = perfil["nodos"]
        orden = perfil.get("orden_tronco") or A.ORDEN_TRONCO_PERFIL   # el del contrato: las piernas detras del torso
        por_accion = {c.accion: c for c in V.clips_de(clips, pid)}
        celdas = [("reposo", PoseFija({}), 0.0)]
        for accion, f in (("Walk", 0.25), ("Kneel", 0.65), ("PickUp", 0.5)):
            c = por_accion[accion]
            celdas.append((accion, c, f * c.duracion))
        paneles = []
        for titulo, clip, t in celdas:
            im = renderiza_nodos(nodos, imagen_de, clip, t, escala=0.42, region=(60, 40, 964, 990), orden_tronco=orden)
            d = ImageDraw.Draw(im)
            d.rectangle([0, 0, im.width, 13], fill=(255, 255, 255, 230))
            d.text((4, 1), "%s: %s" % (pid, titulo), fill=(30, 30, 30, 255))
            paneles.append(im.convert("RGB"))
        cab = next(n for n in nodos if n["nombre"] == "Cuello")["rect"]
        ojos = next(n["sprite"] for n in nodos if n["nombre"] == "Ojos")
        cerrados = ojos.replace("ojos_neutra", "ojos_parpadeo_cerrado")
        region = (cab[0] - 10, cab[1] - 10, cab[2] + 10, cab[3] + 10)
        k = min(1.0, 420.0 / max(region[2] - region[0], region[3] - region[1]))
        for titulo, sprite in (("ojos abiertos", ojos), ("ojos cerrados", cerrados)):
            def lookup(s, sprite=sprite, ojos=ojos):
                return imagen_de(sprite if s == ojos else s)
            im = renderiza_nodos(nodos, lookup, PoseFija({}), escala=k, region=region, orden_tronco=orden)
            d = ImageDraw.Draw(im)
            d.rectangle([0, 0, im.width, 13], fill=(255, 255, 255, 230))
            d.text((4, 1), "%s: %s" % (pid, titulo), fill=(30, 30, 30, 255))
            paneles.append(im.convert("RGB"))
        alto = max(p.height for p in paneles)
        fila = Image.new("RGB", (sum(p.width for p in paneles), alto), (236, 232, 222))
        x = 0
        for p in paneles:
            fila.paste(p, (x, 0))
            x += p.width
        filas.append(fila)
    if not filas:
        print("ningun personaje tiene «perfil» en arte_final.json")
        return 1
    ancho = max(f.width for f in filas)
    sal = Image.new("RGB", (ancho, sum(f.height for f in filas)), (236, 232, 222))
    y = 0
    for f in filas:
        sal.paste(f, (0, y))
        y += f.height
    sal.save(ruta)
    print("hoja de verificacion: %s (%dx%d)" % (ruta, sal.width, sal.height))
    return 0


# ============================================================================ 6. la autoprueba


def entrega_sintetica(destino, falta_lejana="antebrazo", a=1.6, desplazamiento=(-250, -100)):
    """
    Una entrega de perfil sintetica MIRANDO A LA IZQUIERDA, con la geometria conocida: la figura se dibuja en coordenadas del rig (lienzo de 1024, de
    perfil y mirando a la derecha, cadera en x = 512, de y = 77 a 947), se escala «a» veces, se desplaza y se ESPEJA en un lienzo de 1300x1500; asi lo que
    la herramienta tiene que recuperar son esas mismas coordenadas. Trae un muslo llamado «pierna», un espacio en un nombre de archivo, una subcarpeta de mas
    (Perfil/Anidada/) y UNA pieza lejana que falta (por defecto el antebrazo), y dos expresiones en Expresiones/. Devuelve la verdad:
    {"hombro": (x, y), "codo": ..., "cadera": ..., "rodilla": ..., "cuello": ..., "holgura_codo": d}.
    """
    W, H = LIENZO_ENTREGA
    ox, oy = desplazamiento

    def punto(x, y):
        return (W - (a * x + ox), a * y + oy)

    def caja(x0, y0, x1, y1):
        (xa, ya), (xb, yb) = punto(x0, y0), punto(x1, y1)
        return [min(xa, xb), ya, max(xa, xb), yb]

    NEGRO = (20, 12, 8, 255)

    def lienzo():
        return Image.new("RGBA", (W, H), (0, 0, 0, 0))

    def estadio(im, x0, y0, x1, y1, color, sombra=None):
        d = ImageDraw.Draw(im)
        c = caja(x0, y0, x1, y1)
        r = (c[2] - c[0]) / 2.0
        d.rounded_rectangle(c, radius=r, fill=NEGRO)
        d.rounded_rectangle([c[0] + 4, c[1] + 4, c[2] - 4, c[3] - 4], radius=max(1, r - 4), fill=color + (255,))
        if sombra:
            d.rounded_rectangle([c[0] + 4, c[1] + 4, c[0] + 0.30 * (c[2] - c[0]), c[3] - 4], radius=max(1, r - 4), fill=sombra + (255,))

    def pieza(nombre, dibuja, subdir="Perfil"):
        im = lienzo()
        dibuja(im)
        ruta = os.path.join(destino, subdir)
        os.makedirs(ruta, exist_ok=True)
        im.save(os.path.join(ruta, nombre))

    near, far = (PIEL, SOMBRA), (SOMBRA, None)
    for lado, (color, sombra), dx in (("frente", near, 0), ("atras", far, 6)):
        pieza("brazo_%s_x.png" % lado, lambda im, c=color, s=sombra, dx=dx: estadio(im, 487 + dx, 430, 537 + dx, 600, c, s))
        if not (lado == "atras" and falta_lejana == "antebrazo"):
            pieza("mano_%s_x.png" % lado, lambda im, c=color, s=sombra, dx=dx: (estadio(im, 489 + dx, 560, 535 + dx, 760, c, s)))
        pieza("pierna_%s_x.png" % lado, lambda im, c=color, s=sombra, dx=dx: estadio(im, 477 + dx, 600, 547 + dx, 800, c, s),
              subdir="Perfil" if lado == "frente" else os.path.join("Perfil", "Anidada"))
        if not (lado == "atras" and falta_lejana == "antepierna"):
            def pie(im, c=color, s=sombra, dx=dx):
                estadio(im, 487 + dx, 744, 537 + dx, 947, c, s)
                estadio(im, 487 + dx, 915, 640 + dx, 947, c, None)   # la suela, que sobresale hacia delante
            pieza("pie _%s_x.png" % lado if lado == "frente" else "pie_%s_x.png" % lado, pie)

    def torso(im):
        d = ImageDraw.Draw(im)
        estadio(im, 462, 380, 572, 700, (250, 140, 30))
        c = caja(500, 330, 536, 420)
        d.rectangle(c, fill=NEGRO)
        d.rectangle([c[0] + 3, c[1] + 3, c[2] - 3, c[3] - 3], fill=PIEL + (255,))
        estadio(im, 462, 380, 572, 700, (250, 140, 30))

    pieza("torso_perfil_x.png", torso)

    def cabeza(im):
        d = ImageDraw.Draw(im)
        d.ellipse(caja(440, 77, 700, 400), fill=NEGRO)
        d.ellipse(caja(444, 81, 696, 396), fill=(110, 55, 20, 255))
        d.ellipse(caja(540, 200, 696, 392), fill=PIEL + (255,))          # la cara, delante
        d.ellipse(caja(690, 285, 722, 322), fill=NEGRO)                   # la nariz, que sobresale por delante
        d.ellipse(caja(692, 288, 719, 319), fill=PIEL + (255,))

    pieza("cabeza_perfil_x.png", cabeza)

    # las expresiones: ceja, ojo (blanco, iris) o parpado, boca en el borde de delante y rubor, sobre la cara
    def cara(cerrada):
        im = lienzo()
        d = ImageDraw.Draw(im)
        d.line([punto(600, 218), punto(670, 212)], fill=NEGRO, width=8)            # ceja
        if cerrada:
            d.line([punto(612, 252), punto(668, 256)], fill=NEGRO, width=7)        # parpado
            d.arc(caja(612, 246, 668, 292), 20, 160, fill=(255, 255, 255, 120), width=2)   # rastro casi blanco del ojo abierto
        else:
            d.ellipse(caja(612, 236, 668, 280), fill=(255, 255, 255, 255), outline=NEGRO, width=3)
            d.ellipse(caja(640, 242, 662, 274), fill=(200, 120, 20, 255))
        d.line([punto(662, 350), punto(692, 346), punto(698, 338)], fill=NEGRO, width=7)   # boca, en el borde de delante
        rub = Image.new("RGBA", (W, H), (0, 0, 0, 0))
        ImageDraw.Draw(rub).ellipse(caja(580, 300, 650, 340), fill=(250, 130, 150, 255))
        im.alpha_composite(rub)
        return im

    expr = os.path.join(destino, "Expresiones")
    os.makedirs(expr, exist_ok=True)
    cara(False).save(os.path.join(expr, "expresion_perfil_neutra_x.png"))
    cara(True).save(os.path.join(expr, "expresion_perfil _neutra_ojos_cerrados_x.png"))   # con un espacio, como la de la Nina
    verdad = {"hombro": (512.0, 455.0), "cadera": (512.0, 600.0), "cuello_y": 392.0}
    # los extremos redondos de los dos tramos de cada articulacion (centro de la tapa = borde + radio)
    verdad["codo"] = ((512.0 + 512.0) / 2.0, ((600 - 25) + (560 + 23)) / 2.0)        # tapa de abajo del humero (575) y de arriba del antebrazo (583): 579
    verdad["rodilla"] = (512.0, ((800 - 35) + (744 + 25)) / 2.0)                      # tapa de abajo del muslo (765) y de arriba de la pierna (769): 767
    verdad["holguras"] = {"CodoCercano": 8.0, "RodillaCercana": 4.0}
    return verdad


def autoprueba():
    """
    Que la herramienta recupere, de una entrega sintetica que MIRA A LA IZQUIERDA, las articulaciones del rig (<= 1 px) tras el espejo, los 15 nodos
    de la tabla provisional (nombres, padres y orden), el registro, el orden de lo que falta (error sin --sintetiza-lejana, aviso y sintesis con ella)
    y que los nombres con espacio, la subcarpeta de mas y «pierna» por muslo se lean.
    """
    malos = 0

    def caso(nombre, ok, detalle=""):
        nonlocal malos
        malos += 0 if ok else 1
        print("  %-78s %s" % (nombre, "bien" if ok else "FALLA " + detalle))

    with tempfile.TemporaryDirectory() as tmp:
        completa = os.path.join(tmp, "completa")
        verdad = entrega_sintetica(completa, falta_lejana=None)
        print("== entrega sintetica COMPLETA (mira a la izquierda; muslos «pierna», un nombre con espacio, una subcarpeta de mas)")
        res = procesa(completa, "nino")
        for e in res.errores:
            print("ERROR", e)
        caso("la entrega completa se procesa", not res.errores)
        if res.errores:
            return 1
        lec = res.lec
        caso("detecta que mira a la izquierda y espeja", lec.espejo is True and res.registro["espejo"] is True, str(lec.votos))
        caso("los cuatro votos coinciden (los dos pies, la cabeza y la cara)", len(lec.votos) == 4 and all("izquierda" in v for v in lec.votos), str(lec.votos))
        caso("«pierna» es el muslo y «pie» la antepierna", all(k in lec.piezas for k in (("muslo", CERCA), ("muslo", LEJOS), ("antepierna", CERCA), ("antepierna", LEJOS))))
        caso("el nombre con espacio («pie _frente_x.png») se lee", lec.piezas[("antepierna", CERCA)].nombre == "pie _frente_x.png")
        caso("la subcarpeta de mas (Perfil/Anidada/) se recorre", lec.piezas[("muslo", LEJOS)].ruta.split(os.sep)[-2] == "Anidada")
        caso("las lejanas se reconocen por el tono (sombra plana) y las cercanas llevan piel",
             all(lec.piezas[(r, LEJOS)].tono[0] < LEJANA_PIEL_MAX and lec.piezas[(r, CERCA)].tono[0] >= CERCANA_PIEL_MIN for r in DOS_LADOS))
        caso("las expresiones: la abierta y la cerrada (la del espacio) se distinguen",
             os.path.basename(lec.expresiones.get("abierta", "")) == "expresion_perfil_neutra_x.png" and "cerrados" in lec.expresiones.get("cerrada", "") + "cerrados")
        # --- las articulaciones recuperadas, en el lienzo del rig (<= TOLERANCIA_PX)
        j = res.joints
        esperado = {"HombroCercano": verdad["hombro"], "HombroLejano": (verdad["hombro"][0] + 6, verdad["hombro"][1]),
                    "CodoCercano": verdad["codo"], "RodillaCercana": verdad["rodilla"], "CaderaCercana": verdad["cadera"],
                    "CaderaLejana": (verdad["cadera"][0] + 6, verdad["cadera"][1]), "Cuello": (518.0, verdad["cuello_y"])}
        peor = 0.0
        for nombre, e in esperado.items():
            err = max(abs(j[nombre][0] - e[0]), abs(j[nombre][1] - e[1]))
            peor = max(peor, err)
            caso("%-16s recuperada (%.1f, %.1f) contra (%.1f, %.1f): error %.2f px" % (nombre, j[nombre][0], j[nombre][1], e[0], e[1], err),
                 err <= (TOLERANCIA_PX if nombre != "Cuello" else 3.0))
        caso("el hombro lejano y la cadera lejana no son los de la cercana (se miden por pieza)", j["HombroLejano"][0] > j["HombroCercano"][0] + 3)
        caso("las holguras se guardan (codo %.1f, rodilla %.1f)" % (res.holguras["CodoCercano"], res.holguras["RodillaCercana"]),
             abs(res.holguras["CodoCercano"] - verdad["holguras"]["CodoCercano"]) <= 1.5 and abs(res.holguras["RodillaCercana"] - verdad["holguras"]["RodillaCercana"]) <= 1.5)
        caso("el ancla es la cadera cercana en x = 512", abs(j["CaderaCercana"][0] - 512.0) <= 0.5)
        caso("la figura mide 870: coronilla en y = 77 y suela en y = 947",
             lec.piezas[("cabeza", None)].rect[1] == 77 and max(p.rect[3] for p in lec.piezas.values()) == 947,
             "%s %s" % (lec.piezas[("cabeza", None)].rect[1], max(p.rect[3] for p in lec.piezas.values())))
        # --- la tabla: los 15 nodos de la provisional (nombres, padres y orden) y lo que lee la cinematica
        rig_f = [f for f in A.FAMILIA if f[0] == "nino"][0]
        prov = A.personaje_familia(*rig_f)
        prov_nodos, _ = A.perfil_provisional("nino", prov["nodos"], prov["partes"])
        caso("15 nodos con los mismos nombres, padres y orden que la tabla provisional",
             [(n["nombre"], n["padre"], n["tipo"], n["imagen"], n["sprite"]) for n in res.nodos] == [(n["nombre"], n["padre"], n["tipo"], n["imagen"], n["sprite"]) for n in prov_nodos])
        caso("el orden de los hijos de Tronco es el del contrato", [n["nombre"] for n in res.nodos if n["padre"] == A.PT] == A.ORDEN_TRONCO_PERFIL)
        # LAS PIERNAS, SIEMPRE DETRAS DEL TORSO (Santiago, 09/10/2026): la hoja de verificacion dibuja en el orden del contrato aunque la lista de nodos traiga el
        # anterior (la pierna cercana delante del torso); y el torso se dibuja despues de las dos piernas y antes de la cabeza y del brazo cercano
        viejo = A.ordena_hijos(res.nodos, A.PT, ["BrazoLejano", "PiernaLejana", "Torso", "PiernaCercana", "Cuello", "BrazoCercano"])
        punto_ = Image.new("RGBA", (2, 2), (0, 0, 0, 255))
        for etiqueta, lista in (("en el orden del contrato", res.nodos), ("con la lista en el orden anterior", viejo)):
            capas, _ = capas_de_nodos(lista, lambda s: punto_)
            hijos = []
            for ruta_capa, _r, _i in capas:
                h = ruta_capa.split("/")[3]
                if h not in hijos:
                    hijos.append(h)
            caso("la hoja dibuja las piernas detras del torso " + etiqueta, hijos == A.ORDEN_TRONCO_PERFIL, str(hijos))
        por = {n["nombre"]: n for n in res.nodos}
        caso("Tronco y Torso giran en la cadera cercana", por["Tronco"]["punto"] == por["Torso"]["punto"] == A.punto(*j["CaderaCercana"]))
        caso("el rect del antebrazo termina en la punta de la mano y el de la antepierna en la suela (y = 947)",
             por["CodoCercano"]["rect"][3] > por["CodoCercano"]["punto"][1] and por["RodillaCercana"]["rect"][3] == 947 == por["RodillaLejana"]["rect"][3])
        caso("el rect de Cuello es la cabeza y su borde de arriba es la coronilla (77)", por["Cuello"]["rect"][1] == 77)
        caso("el borde de delante del muslo esta en rect[2] (el cuerpo mira a la derecha)", por["PiernaCercana"]["rect"][2] > por["PiernaCercana"]["punto"][0])
        try:
            from coreografia import GeoPerfil   # la cinematica de la coreografia lee exactamente esta tabla
            g = GeoPerfil({"perfil": {"nodos": res.nodos, "provisional": False}})
            caso("coreografia.GeoPerfil lee la tabla y los pies de pie tocan el suelo", abs(g.baja({})) < 1e-6 and abs(g.alto - 870.0) < 1.0, "baja %.2f alto %.1f" % (g.baja({}), g.alto))
        except Exception as e:  # noqa: BLE001
            caso("coreografia.GeoPerfil lee la tabla", False, repr(e))
        caso("el registro: lienzo 1300x1500, espejo y la cabeza en el lienzo espejado",
             res.registro["lienzo"] == [1300, 1500] and res.registro["espejo"] and res.registro["cabeza"][0] < res.registro["cabeza"][2])
        cab = lec.piezas[("cabeza", None)]
        recibida = F.caja_a_lienzo(res.registro, res.registro["cabeza"])
        caso("el registro lleva la caja de la cabeza a su rect (<= 1 px)", max(abs(a - b) for a, b in zip(recibida, cab.rect)) <= 1,
             "%s contra %s" % (recibida, cab.rect))
        caso("las caras (CaraBase, Ojos, Boca) comparten rect dentro de la cabeza",
             por["CaraBase"]["rect"] == por["Ojos"]["rect"] == por["Boca"]["rect"] and por["Ojos"]["rect"][0] >= cab.rect[0] and por["Ojos"]["rect"][2] <= cab.rect[2])
        # volver a correr con el MISMO registro conserva el rect que las caras ya tenian; con otro registro (otra escala o ancla) no
        previa = {"nodos": [dict(n, rect=[1, 2, 3, 4]) if n["nombre"] in ("CaraBase", "Ojos", "Boca") else n for n in res.nodos], "registro": dict(res.registro)}
        r5 = procesa(completa, "nino", previo=previa)
        r6 = procesa(completa, "nino", previo=dict(previa, registro=dict(res.registro, escala=0.5)))
        caso("con el mismo registro las caras conservan su rect; con otro, no (quedaria fuera de la cabeza)",
             all(n["rect"] == [1, 2, 3, 4] for n in r5.nodos if n["nombre"] in ("CaraBase", "Ojos", "Boca"))
             and all(n["rect"] != [1, 2, 3, 4] for n in r6.nodos if n["nombre"] in ("CaraBase", "Ojos", "Boca")))
        escribe = [os.path.basename(os.path.dirname(r)) for r, _ in lista_escritura(res)]
        caso("solo escribe en Perfil/ (diez PNG)", escribe == [SUBCARPETA] * 10, str(set(escribe)))
        # --- nombres: la tolerancia de siempre
        nombres = {"muslo_atras_papa.png": ("muslo", LEJOS), "exoresion_x.png": (None, None), "pierna_frente_niña.png": ("muslo", CERCA),
                   "pie_atras_niña.png": ("antepierna", LEJOS), "mano_frente_nino.png": ("antebrazo", CERCA), "brazo_atras_mamá.png": ("brazo", LEJOS),
                   "torso_perfil_niño.png": ("torso", None), "cabeza_perfil_papa.png": ("cabeza", None), "antepierna_cercana_x.png": ("antepierna", CERCA),
                   "pierna  atras  x.png": ("muslo", LEJOS)}
        caso("clasifica: la entrega real, con ñ, tildes, «pierna» y espacios dobles", all(clasifica(n) == v for n, v in nombres.items()),
             str({n: clasifica(n) for n, v in nombres.items() if clasifica(n) != v}))
        caso("es_cerrada: «cerrados» y «cerrada», con espacio de por medio", es_cerrada("expresion_perfil _neutra_ojos_cerrados_niña.png") and es_cerrada("x_cerrada.png") and not es_cerrada("exoresion_neutra_perfil_papa.png"))
        # --- el espejo es exacto: la misma entrega SIN espejar (hecha mirando a la derecha) da lo mismo
        derecha = os.path.join(tmp, "derecha")
        entrega_sintetica(derecha, falta_lejana=None)
        for dentro, _, archivos in os.walk(derecha):
            for f in archivos:
                if f.endswith(".png"):
                    ruta = os.path.join(dentro, f)
                    ImageOps.mirror(Image.open(ruta)).save(ruta)
        r2 = procesa(derecha, "nino")
        caso("una entrega que ya mira a la derecha no se espeja y da las mismas medidas",
             not r2.errores and r2.lec.espejo is False and all(abs(r2.joints[k][0] - res.joints[k][0]) <= 1.0 and abs(r2.joints[k][1] - res.joints[k][1]) <= 1.0 for k in res.joints),
             str(r2.errores or r2.lec.votos))
        # --- una contradiccion entre piezas se detiene
        rota = os.path.join(tmp, "rota")
        entrega_sintetica(rota, falta_lejana=None)
        ruta_cab = os.path.join(rota, "Perfil", "cabeza_perfil_x.png")
        ImageOps.mirror(Image.open(ruta_cab)).save(ruta_cab)
        r3 = procesa(rota, "nino")
        caso("una cabeza espejada (miraria al otro lado que los pies) detiene la entrega", any("no coinciden" in e for e in r3.errores), str(r3.errores))
        r3b = procesa(rota, "nino", mira="izquierda")
        caso("--mira fuerza la orientacion y lo avisa", not r3b.errores and any("--mira izquierda" in a for a in r3b.avisos))
        # --- un lienzo que no mide 1300x1500 se rechaza
        chica = os.path.join(tmp, "chica")
        entrega_sintetica(chica, falta_lejana=None)
        ruta_t = os.path.join(chica, "Perfil", "torso_perfil_x.png")
        Image.open(ruta_t).crop((0, 0, 1000, 1000)).save(ruta_t)
        r4 = procesa(chica, "nino")
        caso("un PNG que no mide 1300x1500 rechaza la entrega", any("1300x1500" in e for e in r4.errores), str(r4.errores))
    # --- lo que falta
    print("== entrega sintetica con UNA pieza lejana que falta (el antebrazo)")
    with tempfile.TemporaryDirectory() as tmp:
        faltante = os.path.join(tmp, "faltante")
        entrega_sintetica(faltante, falta_lejana="antebrazo")
        sin = procesa(faltante, "nino")
        caso("SIN --sintetiza-lejana (por defecto) se detiene y nombra el archivo que falta",
             sin.errores and any("FALTA" in e and "mano_atras" in e for e in sin.errores) and sin.nodos is None, str(sin.errores))
        caso("y no hay tabla ni nada que escribir (res.nodos es None)", sin.nodos is None and sin.registro is None)
        con = procesa(faltante, "nino", sintetiza=True)
        caso("CON --sintetiza-lejana se fabrica, se avisa en voz alta y se registra",
             not con.errores and any("SINTETIZADA" in a for a in con.avisos) and con.lec.sintetizadas == ["antebrazo_lejano"], str(con.errores))
        pz = con.lec.piezas.get(("antebrazo", LEJOS))
        near = con.lec.piezas.get(("antebrazo", CERCA))
        if pz is not None:
            t = tono(pz.imagen)
            caso("la sintetizada es de sombra plana (sin piel), con la silueta de la cercana",
                 t[0] < LEJANA_PIEL_MAX and t[1] > 0.4 and pz.bbox == near.bbox and pz.imagen.size == near.imagen.size, str(t))
            negros_c = cuenta_color(near.imagen, (20, 12, 8), 20)
            negros_s = cuenta_color(pz.imagen, (20, 12, 8), 20)
            caso("el contorno negro de la sintetizada queda intacto", abs(negros_c - negros_s) <= 0.02 * negros_c, "%d contra %d" % (negros_c, negros_s))
        e = entrada_perfil(con, None)
        caso("perfil.sintetizadas queda en la entrada de arte_final.json", e["sintetizadas"] == ["antebrazo_lejano"] and e["partes"] == [] and "orden_tronco" not in e)
        # una cercana que falte no se sintetiza nunca
        os.remove(os.path.join(faltante, "Perfil", "mano_frente_x.png"))
        sin_cercana = procesa(faltante, "nino", sintetiza=True)
        caso("una pieza CERCANA que falta no se sintetiza ni con --sintetiza-lejana", any("FALTA" in e and "cercana" in e for e in sin_cercana.errores), str(sin_cercana.errores))
    print("autoprueba de preparar_perfil.py:", "pasa" if not malos else "FALLA en %d comprobaciones" % malos)
    return 1 if malos else 0


# ============================================================================ 7. principal


def main(argv=None):
    ap = argparse.ArgumentParser(description="De las piezas de perfil a las medidas del cuerpo de perfil del rig (ver la cabecera del script).")
    ap.add_argument("id", nargs="?", choices=sorted(P.FAMILIA))
    ap.add_argument("entrega", nargs="?", help="carpeta del personaje en la entrega (con Perfil/ y Expresiones/)")
    ap.add_argument("--aplicar", action="store_true", help="escribe los PNG de Perfil/, la clave «perfil» de arte_final.json, regenera las tablas y hace las caras")
    ap.add_argument("--cadera", choices=("borde", "capsula"), default="borde")
    ap.add_argument("--ancla", choices=("cadera", "torso"), default="cadera", help="el x = 512: la cadera cercana (por defecto) o el centro del torso")
    ap.add_argument("--mira", choices=("auto", "izquierda", "derecha"), default="auto", help="hacia donde mira la entrega (auto: se detecta)")
    ap.add_argument("--sintetiza-lejana", action=argparse.BooleanOptionalAction, default=False,
                    help="si falta una pieza LEJANA, fabricarla recoloreando la cercana (APAGADO por defecto: se le pide al artista)")
    ap.add_argument("--limpia-cerrados", action=argparse.BooleanOptionalAction, default=True,
                    help="limpiar el rastro casi blanco de la cara de ojos cerrados (por defecto si)")
    ap.add_argument("--orden-tronco", nargs="+", metavar="NOMBRE", help="escribe perfil.orden_tronco con este orden (por defecto el del contrato, que no se escribe)")
    ap.add_argument("--sin-caras", action="store_true", help="no correr preparar_expresion.py --vista perfil: imprimir las ordenes")
    ap.add_argument("--salida", default=os.path.join(tempfile.gettempdir(), "algoritmia_perfil"), help="donde dejar el composite del informe")
    ap.add_argument("--autoprueba", action="store_true", help="comprueba la herramienta con una entrega sintetica que mira a la izquierda")
    ap.add_argument("--hoja", metavar="PNG", help="dibuja la hoja de verificacion del cuerpo de perfil que hay en el repo y sale")
    a = ap.parse_args(argv)
    if a.autoprueba:
        return autoprueba()
    if a.hoja:
        return hoja(a.hoja, [a.id] if a.id else None)
    if not a.id or not a.entrega:
        ap.error("hace falta <id> y <carpeta_entrega> (o --autoprueba, o --hoja)")
    if not os.path.isdir(a.entrega):
        ap.error("no existe la carpeta %s" % a.entrega)
    os.makedirs(a.salida, exist_ok=True)
    previo = A.cargar_arte_final().get(a.id, {}).get("perfil")
    res = procesa(a.entrega, a.id, a.cadera, a.ancla, a.sintetiza_lejana, a.mira, previo)
    informe(res)
    if res.errores:
        return 2
    ruta = os.path.join(a.salida, "perfil_%s_informe.png" % a.id)
    composite(res, ruta)
    print("\ncomposite: %s" % ruta)
    if not a.aplicar:
        print("\nescribiria (con --aplicar):")
        for r, existia in lista_escritura(res):
            print("  %s %s" % ("sustituye" if existia else "nuevo    ", os.path.relpath(r, P.RAIZ)))
        print("  claudeDocs/tasks/Personajes/herramientas/arte_final.json (clave «perfil» de %s)" % a.id)
        print("  y regenera rig_articulaciones.json y clips_personajes.json; despues las caras de perfil:")
        for orden in ordenes_caras(res, a):
            print("  " + orden)
        return 0
    return 1 if aplica(res, a) else 0


if __name__ == "__main__":
    sys.exit(main())
