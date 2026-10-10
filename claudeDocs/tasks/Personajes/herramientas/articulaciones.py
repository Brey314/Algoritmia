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
#     python3 claudeDocs/tasks/Personajes/herramientas/articulaciones.py --autoprueba   # (INC-134) el cuerpo de perfil cumple el contrato
#
# Coordenadas: las de cut.py, en el lienzo de 1024 unidades con el origen arriba a la izquierda y
# la y hacia ABAJO. La figura va de y = 77 (coronilla) a y = 947 (suelo).
#
# ARTE FINAL. Un personaje con arte final (hoy solo el Nino) NO sale de las formulas de este script —
# esas son para el arte provisional—: sus piezas se miden sobre el alfa de los PNG y sus numeros viven
# en arte_final.json (ARTE_FINAL), que se vuelca tal cual; preparar_arte_final.py los mide y los escribe.
# Sin esa entrada, regenerar la tabla pisaria las articulaciones del Nino con las provisionales (cuello,
# codos y rodillas se recalcularian con la formula). Para comprobar una entrada contra el arte:
# python3 pose_preview.py --mide nino. Algoritm (las tres formas) funciona igual desde el 09/10/2026: preparar_algoritm.py --aplicar escribe
# las entradas «algoritm_fuego|rueda|gota» de arte_final.json y personaje_guia() las vuelca en lugar de ALG.
#
# «orden_tronco»: el orden de dibujo (de atras adelante) de los hijos directos de Cuerpo/Tronco; lo
# aplica BuildRigsFinal.cs.txt. En los prefabs los brazos van PRIMERO (BrazoIzq, BrazoDer, Torso,
# Cuello), o sea detras del torso y de la cabeza. DECISION de Santiago (05/10/2026): en la
# FAMILIA los brazos van DETRAS DEL TORSO y DELANTE DE LA CABEZA (la cara): [Cuello, BrazoIzq, BrazoDer,
# Torso] —la cabeza al fondo, los brazos, el torso delante—; y, desde INC-133 (06/10/2026), el ANTEBRAZO
# con la mano pasa DELANTE del torso, de la cara y de las piernas: se anade al final (AntebrazoIzq, AntebrazoDer). Con el arte final los hombros salen por
# detras de las esquinas del torso y el cuello queda bajo la barbilla sin costura. Con el arte provisional
# de Papa, Mama y Nina la cabeza esta pintada DENTRO del torso: sus brazos quedan detras del torso y de la
# cara (hasta que llegue su arte final, que cumple la regla). En Strike (CharacterRig.armsInFrontActions) el
# motor pasa los brazos DELANTE del torso mientras dura la accion: [Cuello, Torso, BrazoIzq, BrazoDer].
# Algoritm (INC-147, decision de Santiago, 09/10/2026, que REVIERTE para el guia la clausula de INC-132 del 05/10/2026
# «las manos por encima de la cara»): sus BRAZOS van DETRAS DE TODO EL CUERPO: [BrazoIzq, BrazoDer, Torso, Ojos,
# Boca] —los brazos al fondo, luego el torso y la cara encima—. Ninguna mano de Algoritm puede tapar ni rozar su cara, y
# lo que asoma de un brazo es solo lo que sobresale de la silueta del torso. El guia no golpea (no tiene Strike), asi que
# nada lo mueve en ejecucion. La coreografia de la familia respeta la misma idea: los brazos solo van donde se ven
# cuando van detras del torso (Brazo.visible, derivado de la silueta del torso) y delante no hay restriccion.
#
# PERFIL (INC-134, 09/10/2026). La familia gana un SEGUNDO cuerpo, dibujado de perfil y mirando a la DERECHA (el motor voltea el Lienzo para la
# izquierda): Lienzo/Perfil, hermano de Cuerpo y justo detras de el, apagado hasta que la accion lo pide (ActionView: Walk, Run, Carry, Push,
# PickUp, Kneel, Blow). Todo cuelga de UN Tronco con el pivote en la cadera, y sus hijos van en este orden de dibujo (de atras adelante):
#   Perfil/Tronco/{BrazoLejano/CodoLejano/AntebrazoLejano, PiernaLejana/RodillaLejana/AntepiernaLejana,
#                  PiernaCercana/RodillaCercana/AntepiernaCercana, Torso, Cuello/Cabeza/{CaraBase, Ojos, Boca},
#                  BrazoCercano/CodoCercano/AntebrazoCercano}
# LAS PIERNAS, SIEMPRE DETRAS DEL TORSO (Santiago, 09/10/2026): la primera version dibujaba la pierna CERCANA delante del torso y esta mal: de frente
# las piernas ya son hijas de Cuerpo (van antes que Tronco) y de perfil tambien van las dos al fondo, tras el torso; solo el brazo cercano y la cara
# quedan delante de el. ordena_hijos() lleva cualquier tabla «perfil» (la provisional y la de arte_final.json) a este orden.
# Cada personaje de la familia lleva en el JSON una clave «perfil»: {provisional, prefijo, nodos, partes, orden_tronco}, con el esquema de «nodos»
# y «partes» de arriba (todo nodo de perfil es NUEVO: «partes» queda vacio hasta que haya algo que reajustar). Sus medidas son PROVISIONALES: salen
# de la figura de frente (870 px de alto, torso en x = 512, coronilla en y = 77) —cadera, hombro, rodilla y cuello a la misma altura que de frente; el
# torso, mas delgado; brazos y piernas colgando rectos desde esos puntos— y sirven para que la coreografia de perfil (coreografia.py) tenga
# longitudes con las que resolver pasos y agachadas. Cuando llegue el arte de perfil, preparar_perfil.py escribe en arte_final.json una entrada
# «perfil» ({nodos, partes, orden_tronco?}) y este script la vuelca tal cual, como hace con las del arte final de frente. Algoritm no tiene perfil.
#
# CARA FINAL DE ALGORITM Y RETRATO ANIMADO (INC-148, 10/10/2026). La entrada «algoritm_<forma>» de arte_final.json puede traer «prefijo_cara» («char_algoritm»): las tres
# formas comparten UNA cara (Algoritm/Expresiones/char_algoritm_{ojos_*,boca_*}, sin el nombre de la forma, 11 texturas en vez de 33) y personaje_guia() lo lleva tal cual al
# JSON para que BuildRigsFinal (FaceNames) busque primero el prefijo de la forma y despues ese. Y cualquier entrada puede traer «retrato» (lo escribe preparar_retrato.py:
# el recorte de la tarjeta del dialogo, el rect de la cara normalizado y la IoU de registro); al JSON van solo «base» (el sprite char_<x>_retrato_base) y «cara» (x, y, ancho,
# alto normalizados, origen ABAJO a la izquierda: lo que BuildRigsFinal «retrato» escribe en CharacterRig.PortraitBase y PortraitFace).
#
# Por qué el JSON no lleva «segmentado»: se deduce del prefab. Un personaje está segmentado
# cuando la Image de AntepiernaIzq tiene sprite y está encendida; guardarlo también en la tabla
# sería una segunda fuente de verdad que se desincroniza al primer cambio de arte.

import json
import math
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

# INC-133 (Santiago, 06/10/2026), para la FAMILIA: el HUMERO (BrazoIzq, BrazoDer) va DETRAS del torso y el ANTEBRAZO (con la mano) DELANTE del torso,
# de la cara y de las piernas. uGUI dibuja en orden de jerarquia, y el antebrazo cuelga del codo, que cuelga del humero: para dibujarlo
# delante, BuildRigsFinal «orden» lo pasa a hijo de Tronco (lo hace cuando «orden_tronco» lo LISTA) y deja bajo el codo un ancla vacia
# (AnclaAntebrazoIzq/Der) que es la que animan los clips; CharacterRig copia su pose al antebrazo en cada cuadro. Por eso los clips no
# animan nunca AntebrazoX ni AnclaAntebrazoX: solo BrazoX y BrazoX/CodoX. Algoritm NO lista antebrazos (sigue bajo el codo y sin ancla).
ORDEN_TRONCO_FAMILIA = ["Cuello", "BrazoIzq", "BrazoDer", "Torso", "AntebrazoIzq", "AntebrazoDer"]   # Mama, Nina y Nino: cara al fondo, humeros, torso, antebrazos
ORDEN_TRONCO_PAPA = ["BrazoIzq", "BrazoDer", "Torso", "Cuello", "AntebrazoIzq", "AntebrazoDer"]      # Papa: la barba y la cara DELANTE del torso (Santiago, 06/10/2026); los antebrazos delante de todo
# INC-147 (Santiago, 09/10/2026), para el GUIA: los dos brazos van DETRAS DE TODO EL CUERPO (torso y cara), no delante. Revierte la clausula de INC-132
# (05/10/2026: «las manos por ENCIMA de la cara») solo para Algoritm; la familia no cambia. Como en la familia, el humero asoma solo por donde sobresale
# de la silueta del torso. Algoritm no lista antebrazos: el antebrazo sigue bajo el codo y viaja con el brazo, asi que tambien queda detras.
ORDEN_TRONCO_GUIA = ["BrazoIzq", "BrazoDer", "Torso", "Ojos", "Boca"]            # Algoritm: brazos al fondo, luego torso y cara (Santiago, 09/10/2026, INC-147)

# Arte final del Nino (05/10/2026), medido sobre el alfa de las piezas de Assets/Game/Art/Characters/Boy;
# los numeros viven en arte_final.json (abajo se cargan).
# Cada brazo es una capsula con el contorno cerrado en los dos extremos; el pivote del hombro es el
# CENTRO del extremo redondo proximal del humero, y ese centro cae sobre la esquina superior del torso
# (el torso mide x 451..575 de lado a lado y su hombro arranca en y = 557: izq (466, 560), der (556, 560)).
# El codo es el centro del extremo distal del humero, y el antebrazo se coloca con el centro de su extremo
# redondo proximal encima. Los rect conservan el tamano de los PNG (no se reescala ningun sprite):
#   brazo_izq 156x105: extremo del hombro (126.6, 27.7), del codo (27.4, 77.3), radio 28.8
#   brazo_der 167x99:  extremo del hombro (27.8, 28.3),  del codo (139.2, 73.3), radio 27.6
#   antebrazo_izq 195x113: extremo del codo (171.2, 25.5) · antebrazo_der 197x100: extremo del codo (24.8, 24.5)
# Antes (esquina del rect): hombros en (518, 539) y (507, 536), a 11 px uno del otro, o sea, en el centro
# del pecho: dibujados delante del torso formaban un «yugo».
# Rodillas: lo mismo. La antepierna es un tubo con la punta redonda arriba cuyo centro cae sobre el centro
# del extremo inferior del muslo (izq: muslo (474.7, 841.0), antepierna (482.6, 844.6); der: (564.4, 834.0) y
# (557.6, 842.4)); el pivote de la rodilla es el centro de la antepierna. Antes estaba en el borde del rect del
# muslo (y = 876 y 869), 30 px por debajo de la articulacion: la antepierna giraba alrededor de un punto que
# no es el suyo y se despegaba del muslo al doblar.
# Ojos y boca (sin sprite aun): el ovalo de la cara va de x 100 a 455 y de y 270 (flequillo) a 450 (barbilla)
# dentro de la cabeza (535x456 en (224, 77)); ojos y cejas (200x90, el 55 % del ancho de la cara) centrados en
# (502, 425), boca (110x44) en (502, 490).
ARTE_FINAL_JSON = os.path.join(AQUI, "arte_final.json")


def cargar_arte_final(ruta=ARTE_FINAL_JSON):
    """
    {id: {"nodos": [...], "partes": [...]}} de arte_final.json (ASCII). Hoy los cuatro de la familia; preparar_arte_final.py
    --aplicar anade a quien entregue su arte. Sin el archivo, ninguno: todos salen de las formulas. Desde INC-134 la entrada de un
    personaje puede traer ademas «perfil» ({nodos, partes, orden_tronco?, registro?}: lo escribe preparar_perfil.py) y, dentro de
    «registro», «cara_perfil» (preparar_expresion.py --vista perfil): perfil_familia() vuelca «perfil» tal cual.
    """
    if not os.path.isfile(ruta):
        return {}
    with open(ruta, encoding="utf-8") as f:
        return json.load(f)["personajes"]


def guardar_arte_final(personajes, ruta=ARTE_FINAL_JSON):
    """Escribe arte_final.json (ASCII, con el formato compacto de rig_articulaciones.json)."""
    with open(ruta, encoding="utf-8") as f:
        doc = json.load(f)
    doc["personajes"] = personajes
    with open(ruta, "w", encoding="utf-8", newline="\n") as f:
        f.write(compacto(doc) + "\n")


ARTE_FINAL = cargar_arte_final()  # los numeros del Nino (arriba) viven en arte_final.json


def orden_tronco_de(pid):
    """El orden de dibujo (de atras adelante) de los hijos de Tronco de un personaje de la familia: el contrato de INC-133."""
    return list(ORDEN_TRONCO_PAPA if pid == "papa" else ORDEN_TRONCO_FAMILIA)


# INC-134: el orden de dibujo de los hijos de Perfil/Tronco, de atras adelante (el contrato con el motor y con BuildRigsFinal «perfil»). Las dos
# piernas van detras del torso (Santiago, 09/10/2026): lo lejano y las piernas al fondo, luego el torso, la cabeza y el brazo cercano delante.
ORDEN_TRONCO_PERFIL = ["BrazoLejano", "PiernaLejana", "PiernaCercana", "Torso", "Cuello", "BrazoCercano"]
PF = "Lienzo/Perfil"
PT = PF + "/Tronco"

# Proporciones PROVISIONALES del cuerpo de perfil respecto del de frente («a ojo»; el arte de perfil las sustituye): el torso de perfil
# mide esta fraccion del ancho del de frente; el muslo, esta vez el ancho del muslo de frente; el brazo, esta fraccion del grosor de la pierna;
# la cabeza, esta fraccion del ancho de la de frente, corrida hacia delante (derecha) esta fraccion de su ancho; la cara de perfil ocupa esta
# fraccion de la cara de frente y cae en la mitad delantera de la cabeza; el pie sobresale hacia delante este multiplo del grosor de la pierna.
PERFIL_TORSO = 0.72
PERFIL_MUSLO = 1.25
PERFIL_BRAZO = 0.90
PERFIL_CABEZA = 0.90
PERFIL_CABEZA_ADELANTE = 0.05
PERFIL_CARA = 0.62
PERFIL_CARA_ADELANTE = 0.17
PERFIL_PIE = 0.90


def _por_nombre(lista):
    return {x["nombre"]: x for x in lista}


def ordena_hijos(nodos, padre, orden):
    """
    La lista «nodos» de una tabla con los hijos directos de «padre» en el orden de dibujo «orden» (de atras adelante), cada uno seguido de lo que cuelga de
    el (el segmento de una articulacion y la cara de la cabeza). Lo que no es hijo de «padre» ni descendiente de uno de ellos (Perfil, Tronco) conserva su
    sitio al principio; un hijo que «orden» no nombra queda delante de los nombrados, en su orden relativo (como ProfileTargetOrder del generador). No
    cambia ningun nodo: solo la posicion en la lista, que es el orden en que BuildRigsFinal los crea y en que dibuja la hoja de verificacion.
    """
    hijos = [n for n in nodos if n["padre"] == padre]
    nombres = [n["nombre"] for n in hijos]
    for nombre in orden:
        if nombres.count(nombre) != 1:
            raise KeyError("«orden» nombra «%s» y %d hijos de %s tienen ese nombre" % (nombre, nombres.count(nombre), padre))
    objetivo = [n for n in nombres if n not in orden] + list(orden)

    def cuelga_de(n, nombre):
        ruta = padre + "/" + nombre
        return n["padre"] == ruta or n["padre"].startswith(ruta + "/")

    cabeza = [n for n in nodos if n["padre"] != padre and not any(cuelga_de(n, h) for h in nombres)]
    por_hijo = {h: [n for n in nodos if n["padre"] == padre and n["nombre"] == h or cuelga_de(n, h)] for h in nombres}
    salida = list(cabeza)
    for h in objetivo:
        salida += por_hijo[h]
    return salida


def perfil_provisional(pid, nodos, partes):
    """
    El cuerpo de perfil de la familia con medidas PROVISIONALES derivadas de la figura de frente (INC-134): «nodos» y «partes» son los de frente ya
    resueltos (de las formulas o de arte_final.json). Devuelve las listas «nodos» (todo nuevo, padre antes que hijo, en el orden de dibujo) y «partes»
    (vacia). Las coordenadas son las del resto del archivo: lienzo de 1024, origen arriba a la izquierda, y hacia abajo, suelo en y = 947.
    """
    pref = "char_%s_perfil" % pid
    n, q = _por_nombre(nodos), _por_nombre(partes)
    cx = 512.0
    cadera_y = 0.5 * (q["PiernaIzq"]["pivote"][1] + q["PiernaDer"]["pivote"][1])
    rodilla_y = 0.5 * (n["RodillaIzq"]["punto"][1] + n["RodillaDer"]["punto"][1])
    hombro_y = 0.5 * (q["BrazoIzq"]["pivote"][1] + q["BrazoDer"]["pivote"][1])
    codo = n["CodoIzq"]["punto"]
    hombro_f = q["BrazoIzq"]["pivote"]
    largo_humero = math.hypot(codo[0] - hombro_f[0], codo[1] - hombro_f[1])
    r = n["CodoIzq"]["rect"]
    largo_antebrazo = max(math.hypot(ex - codo[0], ey - codo[1]) for ex in (r[0], r[2]) for ey in (r[1], r[3]))
    ancho_pierna = q["PiernaIzq"]["rect"][2] - q["PiernaIzq"]["rect"][0]
    grosor_pierna = PERFIL_MUSLO * ancho_pierna
    grosor_brazo = PERFIL_BRAZO * grosor_pierna
    tx0, ty0, tx1, ty1 = q["Torso"]["rect"]
    mitad = 0.5 * PERFIL_TORSO * (tx1 - tx0)
    cabeza = n["Cuello"]["rect"]
    cuello_y = n["Cuello"]["punto"][1]
    ancho_cabeza = PERFIL_CABEZA * (cabeza[2] - cabeza[0])
    cabeza_cx = cx + PERFIL_CABEZA_ADELANTE * ancho_cabeza
    cara = n["Ojos"]["rect"]
    ancho_cara = PERFIL_CARA * (cara[2] - cara[0])
    cara_cx = cabeza_cx + PERFIL_CARA_ADELANTE * ancho_cabeza
    cara_rect = rect_centrado(cara_cx, 0.5 * (cara[1] + cara[3]), ancho_cara, cara[3] - cara[1])

    def nodo(nombre, tipo, padre, punto, imagen="", sprite="", rect=None):
        return {"nombre": nombre, "tipo": tipo, "padre": padre, "punto": punto, "imagen": imagen, "sprite": sprite, "rect": rect}

    lista = [
        nodo("Perfil", "grupo", "Lienzo", punto(cx, SUELO), rect=[0, 0, 1024, 1024]),
        nodo("Tronco", "grupo", PF, punto(cx, cadera_y), rect=[0, 0, 1024, 1024]),
    ]

    def brazo(lado):
        sh = (cx, hombro_y)
        el = (cx, hombro_y + largo_humero)
        humero = [entero(cx - grosor_brazo / 2), entero(hombro_y - grosor_brazo / 2), entero(cx + grosor_brazo / 2), entero(el[1] + grosor_brazo / 2)]
        ante = [entero(cx - 0.45 * grosor_brazo), entero(el[1] - grosor_brazo / 2), entero(cx + 0.45 * grosor_brazo), entero(el[1] + largo_antebrazo)]
        return [
            nodo("Brazo" + lado, "imagen", PT, punto(*sh), sprite="%s_brazo_%s" % (pref, lado.lower()), rect=humero),
            nodo("Codo" + lado, "articulacion", PT + "/Brazo" + lado, punto(*el), imagen="Antebrazo" + lado,
                 sprite="%s_antebrazo_%s" % (pref, lado.lower()), rect=ante),
        ]

    def pierna(lado):
        hip = (cx, cadera_y)
        kn = (cx, rodilla_y)
        muslo = [entero(cx - grosor_pierna / 2), entero(cadera_y - grosor_pierna / 2), entero(cx + grosor_pierna / 2), entero(rodilla_y + 0.3 * grosor_pierna)]
        canilla = [entero(cx - 0.45 * grosor_pierna), entero(rodilla_y - 0.4 * grosor_pierna),
                   entero(cx + 0.45 * grosor_pierna + PERFIL_PIE * grosor_pierna), entero(SUELO)]
        return [
            nodo("Pierna" + lado, "imagen", PT, punto(*hip), sprite="%s_pierna_%s" % (pref, lado.lower()), rect=muslo),
            nodo("Rodilla" + lado, "articulacion", PT + "/Pierna" + lado, punto(*kn), imagen="Antepierna" + lado,
                 sprite="%s_antepierna_%s" % (pref, lado.lower()), rect=canilla),
        ]

    # de atras adelante: brazo lejano, pierna lejana, pierna cercana, torso, cabeza, brazo cercano (las piernas, siempre detras del torso)
    lista += brazo("Lejano") + pierna("Lejana") + pierna("Cercana")
    lista.append(nodo("Torso", "imagen", PT, punto(cx, cadera_y), sprite=pref + "_torso",
                      rect=[entero(cx - mitad), entero(ty0), entero(cx + mitad), entero(ty1)]))
    cuello = (cabeza_cx - 0.02 * ancho_cabeza, cuello_y)
    padre_cabeza = PT + "/Cuello/Cabeza"
    lista.append(nodo("Cuello", "articulacion", PT, punto(*cuello), imagen="Cabeza", sprite=pref + "_cabeza",
                      rect=rect_centrado(cabeza_cx, 0.5 * (cabeza[1] + cabeza[3]), ancho_cabeza, cabeza[3] - cabeza[1])))
    centro_cara = punto(0.5 * (cara_rect[0] + cara_rect[2]), 0.5 * (cara_rect[1] + cara_rect[3]))
    for nombre, sprite in (("CaraBase", pref + "_cara_base"), ("Ojos", pref + "_ojos_neutra"), ("Boca", pref + "_boca_0")):
        lista.append(nodo(nombre, "imagen", padre_cabeza, list(centro_cara), imagen=nombre, sprite=sprite, rect=list(cara_rect)))
    lista += brazo("Cercano")
    return lista, []


def perfil_familia(pid, nodos, partes):
    """
    La entrada «perfil» de un personaje de la familia: {provisional, prefijo, nodos, partes, orden_tronco}. Si arte_final.json trae un «perfil» del
    personaje (lo escribe preparar_perfil.py con el arte de perfil), sale de ahi tal cual; si no, se estima de la figura de frente (PROVISIONAL).
    """
    entrada = ARTE_FINAL.get(pid, {}).get("perfil")
    if entrada:
        orden = entrada.get("orden_tronco") or list(ORDEN_TRONCO_PERFIL)
        lista, fijas, provisional = ordena_hijos(entrada["nodos"], PT, orden), entrada.get("partes", []), False   # el orden del contrato, aunque arte_final.json guarde otro
    else:
        lista, fijas = perfil_provisional(pid, nodos, partes)
        orden, provisional = list(ORDEN_TRONCO_PERFIL), True
    return {"provisional": provisional, "prefijo": "char_%s_perfil" % pid, "nodos": lista, "partes": fijas, "orden_tronco": orden}


def caja_de(rect):
    return [entero(rect[0]), entero(rect[1]), entero(rect[2]), entero(rect[3])]


def rect_centrado(cx, cy, w, h):
    return [entero(cx - w / 2), entero(cy - h / 2), entero(cx + w / 2), entero(cy + h / 2)]


def retrato_de(entrada):
    """{base, cara} del «retrato» de una entrada de arte_final.json (INC-148), o None si aun no lo tiene. «cara» = [x, y, ancho, alto] normalizados con el origen abajo a la izquierda."""
    r = (entrada or {}).get("retrato")
    if not r:
        return None
    return {"base": r["base"], "cara": [round(float(v), 5) for v in r["cara"]]}


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
    # La cara va en TRES capas dentro de Cabeza, de atras adelante: CaraBase (nariz y rubor, que no cambian), Ojos (con cejas) y Boca.
    # CaraBase es el PRIMER hijo de Cabeza. Estimacion provisional (sin sprite: la Image queda apagada): la caja que cubre ojos y
    # boca; con la cara real las tres capas comparten un solo rect (preparar_expresion.py), el del lienzo de la cara entera.
    ex, ey, ew, eh = CARA[pid]["ojos"]
    bx, by, bw, bh = CARA[pid]["boca"]
    union = (min(ex - ew / 2, bx - bw / 2), min(ey - eh / 2, by - bh / 2), max(ex + ew / 2, bx + bw / 2), max(ey + eh / 2, by + bh / 2))
    base = {"nombre": "CaraBase", "tipo": "imagen", "padre": padre_cara,
            "punto": punto((union[0] + union[2]) / 2, (union[1] + union[3]) / 2), "imagen": "CaraBase",
            "sprite": pref + "_cara_base", "rect": caja_de(union)}
    nodos.append(base)
    for nombre, clave, sprite in (("Ojos", "ojos", pref + "_ojos_neutra"), ("Boca", "boca", pref + "_boca_0")):
        ox, oy, ow, oh = CARA[pid][clave]
        nodos.append({
            "nombre": nombre, "tipo": "imagen", "padre": padre_cara,
            "punto": punto(ox, oy), "imagen": nombre, "sprite": sprite,
            "rect": rect_centrado(ox, oy, ow, oh),
        })

    if pid in ARTE_FINAL:  # arte final: las medidas salen del alfa de las piezas, no de las formulas de arriba
        nodos, partes = ARTE_FINAL[pid]["nodos"], ARTE_FINAL[pid]["partes"]
    out = {
        "id": pid, "prefab": prefab, "carpeta": carpeta, "prefijo": pref, "guia": False,
        "orden_tronco": orden_tronco_de(pid), "nodos": nodos, "partes": partes,
        "perfil": perfil_familia(pid, nodos, partes),
    }
    if retrato_de(ARTE_FINAL.get(pid)):
        out["retrato"] = retrato_de(ARTE_FINAL[pid])
    return out


# ----------------------------------------------------------------------------- Algoritm

# Medidas sobre el ALFA de char_algoritm_n1_fuego_reposo.png (768 px, que el Image con preserveAspect pinta a 1024; maqueta.piezas_guia y su
# bloque de arriba explican como se corta). La rueda y la gota son el mismo dibujo recoloreado (forms.py): la geometria es una sola. Las posiciones
# son pixeles del lienzo de 1024; el script las convierte en fracciones del rect de Cuerpo, que en los prefabs es el lienzo entero.
# 08/10/2026 (D13, Santiago: «sus brazos se moveran en todas sus escenas»): Algoritm se parte en sus nueve piezas, asi que estas medidas ya no son un
# «a ojo» para una sola Image. Los palos miden 27 px (de 26 a 27,5) y cada articulacion cae sobre el EJE de su palo; antes el hombro estaba 17 px
# por encima del eje (en la esquina del palo con el vientre), y un brazo que girara desde ahi se habria despegado del cuerpo.
#   hombro: donde el eje del palo sale del vientre · codo: a mitad de camino entre el hombro y la muneca (donde empieza la mano) ·
#   cadera: el eje del palo, en el borde de abajo del vientre · rodilla: a mitad de la pierna, sobre el eje.
# Los rects de las piezas son lo que ocupa cada una (con el corte de maqueta.piezas_guia), en multiplos de 4: a 768 px caen en pixeles enteros del
# sprite y las piezas se cortan sin reescalar. Con el arte final se miden sobre el alfa de sus PNG, como en la familia.
ALG = {
    "hombro_izq": (287, 629), "hombro_der": (736, 630),
    "codo_izq": (224, 693), "codo_der": (800, 693),
    "cadera_izq": (455, 800), "cadera_der": (568, 800),
    "rodilla_izq": (455, 901), "rodilla_der": (568, 901),
    "brazo_izq": (208, 612, 304, 708), "brazo_der": (720, 612, 816, 708),                   # el palo del hombro al codo, con su rotula
    "antebrazo_izq": (8, 684, 236, 908), "antebrazo_der": (788, 684, 1016, 908),            # el palo del codo a la muneca y la mano
    "muslo_izq": (440, 784, 472, 916), "muslo_der": (552, 784, 584, 916),                   # de la cadera a la rodilla, con las dos rotulas
    "antepierna_izq": (388, 900, 472, 1004), "antepierna_der": (552, 900, 636, 1004),       # de la rodilla al pie
    "cuerpo": (248, 20, 776, 812),     # la llama y el vientre de colores, sin extremidades
    "pivote_tronco": (512, 700),       # a la altura del vientre: de ahi gira el gesto de hablar
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
        nodos.append({
            "nombre": "Pierna" + lado, "tipo": "imagen", "padre": C,
            "punto": punto(*pt(ALG["cadera_" + sufijo])), "imagen": "Pierna" + lado,
            "sprite": "%s_parte_pierna_%s" % (pref, sufijo), "rect": caja_de(rc(ALG["muslo_" + sufijo])),
        })

    pivote = pt(ALG["pivote_tronco"])
    nodos.append({
        "nombre": "Tronco", "tipo": "grupo", "padre": C, "punto": punto(*pivote),
        "imagen": "", "sprite": "", "rect": caja_de((x0, y0, x1, y1)),
    })

    for lado, sufijo in (("Izq", "izq"), ("Der", "der")):
        nodos.append({
            "nombre": "Brazo" + lado, "tipo": "imagen", "padre": T,
            "punto": punto(*pt(ALG["hombro_" + sufijo])), "imagen": "Brazo" + lado,
            "sprite": "%s_parte_brazo_%s" % (pref, sufijo), "rect": caja_de(rc(ALG["brazo_" + sufijo])),
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

    # Codos y rodillas: sobre el eje de su palo (ALG); el antebrazo y la antepierna son lo que queda mas alla.
    for lado, sufijo in (("Izq", "izq"), ("Der", "der")):
        nodos.append({
            "nombre": "Codo" + lado, "tipo": "articulacion", "padre": T + "/Brazo" + lado,
            "punto": punto(*pt(ALG["codo_" + sufijo])), "imagen": "Antebrazo" + lado,
            "sprite": "%s_parte_antebrazo_%s" % (pref, sufijo), "rect": caja_de(rc(ALG["antebrazo_" + sufijo])),
        })

    for lado, sufijo in (("Izq", "izq"), ("Der", "der")):
        nodos.append({
            "nombre": "Rodilla" + lado, "tipo": "articulacion", "padre": C + "/Pierna" + lado,
            "punto": punto(*pt(ALG["rodilla_" + sufijo])), "imagen": "Antepierna" + lado,
            "sprite": "%s_parte_antepierna_%s" % (pref, sufijo), "rect": caja_de(rc(ALG["antepierna_" + sufijo])),
        })

    # ARTE FINAL de Algoritm (preparar_algoritm.py, 09/10/2026): con una entrada «algoritm_<forma>» en arte_final.json las medidas salen del alfa de
    # las siete piezas entregadas (pierna entera, sin rodilla partida) y no de las constantes de ALG, que describian el sprite entero de antes. Los
    # nombres de nodo y «orden_tronco» no cambian: la entrada se vuelca tal cual, como la de la familia. Sin ella, las formulas de siempre.
    partes = []
    entrada = ARTE_FINAL.get("algoritm_" + forma)
    if entrada:
        nodos, partes = entrada["nodos"], entrada.get("partes", [])
    out = {
        "id": "algoritm_" + forma, "prefab": prefab, "carpeta": "Algoritm", "prefijo": pref,
        "guia": True, "orden_tronco": ORDEN_TRONCO_GUIA, "nodos": nodos, "partes": partes,
    }
    if entrada and entrada.get("prefijo_cara"):   # INC-148: la cara FINAL es una sola para las tres formas (char_algoritm_ojos_neutra, sin la forma)
        out["prefijo_cara"] = entrada["prefijo_cara"]
    if retrato_de(entrada):
        out["retrato"] = retrato_de(entrada)
    return out


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
        if all(isinstance(v, (int, float, str)) for v in valor):
            return "[" + ", ".join(json.dumps(v) for v in valor) + "]"
        filas = ["%s  %s" % (sangria, compacto(v, nivel + 1)) for v in valor]
        return "[\n" + ",\n".join(filas) + "\n" + sangria + "]"
    return json.dumps(valor, ensure_ascii=True)


# Los sprites que pide el contrato del cuerpo de perfil (INC-134), en Assets/Game/Art/Characters/<Carpeta>/Perfil/: diez piezas del cuerpo, las tres capas de la cara y el
# set de la cara. Las demas expresiones (ojos_<emocion>, boca_<emocion>…) las escribe preparar_expresion.py --vista perfil con el mismo prefijo.
SPRITES_PERFIL = ["torso", "cabeza", "cara_base", "ojos_neutra", "boca_0",
                  "brazo_cercano", "brazo_lejano", "antebrazo_cercano", "antebrazo_lejano",
                  "pierna_cercana", "pierna_lejana", "antepierna_cercana", "antepierna_lejana"]


def autoprueba():
    """El cuerpo de perfil que sale de la tabla cumple el contrato (INC-134): nodos, orden de dibujo, sprites, figura de 870 px y entrada de arte_final.json."""
    malos = 0

    def caso(nombre, ok, detalle=""):
        nonlocal malos
        malos += 0 if ok else 1
        print("%-60s %s" % (nombre, "bien" if ok else "FALLA " + detalle))

    for f in FAMILIA:
        pid = f[0]
        # el contrato de la tabla PROVISIONAL se comprueba sin la clave «perfil» de arte_final.json (preparar_perfil.py la escribe cuando llega el arte de
        # perfil y entonces las medidas ya no salen de la figura de frente); con arte, mas abajo, las comprobaciones del arte
        con_arte = ARTE_FINAL.get(pid, {}).pop("perfil", None)
        try:
            p = personaje_familia(*f)
        finally:
            if con_arte is not None:
                ARTE_FINAL[pid]["perfil"] = con_arte
        pf = p["perfil"]
        nodos = pf["nodos"]
        rutas = ["Lienzo"]       # lo que existe antes de cada nodo: lo anterior y, de una articulacion, su segmento (Codo → Antebrazo, Cuello → Cabeza)
        padre_antes = True
        for n in nodos:
            padre_antes = padre_antes and n["padre"] in rutas
            rutas.append(n["padre"] + "/" + n["nombre"])
            if n["tipo"] == "articulacion":
                rutas.append(n["padre"] + "/" + n["nombre"] + "/" + n["imagen"])
        caso("%s: 15 nodos, cada padre antes que su hijo" % pid, len(nodos) == 15 and padre_antes)
        imagenes = [n["sprite"] for n in nodos if n["sprite"]]
        esperados = ["char_%s_perfil_%s" % (pid, k) for k in SPRITES_PERFIL]
        caso("%s: los 13 sprites del contrato, ninguno de mas" % pid, sorted(imagenes) == sorted(esperados), str(sorted(set(imagenes) ^ set(esperados))))
        hijos = [n["nombre"] for n in nodos if n["padre"] == PT]
        caso("%s: los hijos de Tronco van en el orden de dibujo" % pid, hijos == ORDEN_TRONCO_PERFIL and pf["orden_tronco"] == ORDEN_TRONCO_PERFIL, str(hijos))
        por = _por_nombre(nodos)
        caso("%s: Perfil (grupo, en el suelo) y Tronco (en la cadera)" % pid,
             por["Perfil"]["tipo"] == "grupo" and por["Perfil"]["punto"] == [512, 947] and por["Tronco"]["tipo"] == "grupo"
             and por["Tronco"]["punto"][1] == entero(0.5 * sum(q["pivote"][1] for q in p["partes"] if q["nombre"] in ("PiernaIzq", "PiernaDer"))))
        cara = [por[k]["rect"] for k in ("CaraBase", "Ojos", "Boca")]
        caso("%s: CaraBase, Ojos y Boca comparten rect" % pid, cara[0] == cara[1] == cara[2])
        caso("%s: la figura mide 870 (coronilla 77, suelo 947) y el torso esta en x = 512" % pid,
             por["Cuello"]["rect"][1] == 77 and por["RodillaCercana"]["rect"][3] == 947 and por["RodillaLejana"]["rect"][3] == 947
             and abs((por["Torso"]["rect"][0] + por["Torso"]["rect"][2]) / 2.0 - 512) <= 1)
        caso("%s: provisional y con prefijo char_%s_perfil" % (pid, pid), pf["provisional"] is True and pf["prefijo"] == "char_%s_perfil" % pid)
        # el frente no cambia: los nodos y las partes de frente no llevan nada de perfil
        caso("%s: nada de perfil entre los nodos de frente" % pid, not any("Perfil" in n["padre"] or "perfil" in n["sprite"] for n in p["nodos"]))
        if con_arte is not None:
            # con el arte de perfil (preparar_perfil.py): se vuelca tal cual, con los MISMOS nodos que la provisional (nombres, padres, orden) y la figura de 870 px
            pa = personaje_familia(*f)["perfil"]
            clave = lambda lista: [(n["nombre"], n["padre"], n["tipo"], n["imagen"], n["sprite"]) for n in lista]
            caso("%s: con arte de perfil, los nodos son los de la provisional y ya no es provisional" % pid, clave(pa["nodos"]) == clave(nodos) and pa["provisional"] is False,
                 "%s" % [a for a, b in zip(clave(pa["nodos"]), clave(nodos)) if a != b][:2])
            pn = _por_nombre(pa["nodos"])
            caso("%s: con arte de perfil, la coronilla en y = 77, la suela mas baja en y = 947 y Torso girando en el pivote de Tronco (la cadera)" % pid,
                 pn["Cuello"]["rect"][1] == 77 and max(pn["RodillaCercana"]["rect"][3], pn["RodillaLejana"]["rect"][3]) == 947
                 and pn["Tronco"]["punto"] == pn["Torso"]["punto"],
                 "%s %s" % (pn["Cuello"]["rect"], pn["Tronco"]["punto"]))
    # con una entrada «perfil» en arte_final.json sale de ahi, tal cual
    antes = dict(ARTE_FINAL)
    try:
        ARTE_FINAL["nino"] = dict(ARTE_FINAL.get("nino", {}), perfil={"nodos": [{"nombre": "X", "padre": PT}], "partes": [], "orden_tronco": ["X"]})
        pf = perfil_familia("nino", [], [])
        caso("arte_final.json con «perfil»: se vuelca tal cual y deja de ser provisional",
             pf["nodos"] == [{"nombre": "X", "padre": PT}] and pf["orden_tronco"] == ["X"] and pf["provisional"] is False and pf["prefijo"] == "char_nino_perfil")
    finally:
        ARTE_FINAL.clear()
        ARTE_FINAL.update(antes)
    # LAS PIERNAS, SIEMPRE DETRAS DEL TORSO (Santiago, 09/10/2026): el contrato pone las dos piernas antes que el torso y ordena_hijos lleva a el cualquier
    # tabla (la de arte_final.json guardada con el orden anterior, la pierna cercana delante del torso) sin perder lo que cuelga de cada hijo
    contrato = ["BrazoLejano", "PiernaLejana", "PiernaCercana", "Torso", "Cuello", "BrazoCercano"]
    caso("ORDEN_TRONCO_PERFIL: las dos piernas detras del torso, el brazo cercano delante", ORDEN_TRONCO_PERFIL == contrato)
    viejo = ["BrazoLejano", "PiernaLejana", "Torso", "PiernaCercana", "Cuello", "BrazoCercano"]

    def tabla_con_orden(orden):
        colgantes = {"BrazoLejano": [("CodoLejano", PT + "/BrazoLejano")], "BrazoCercano": [("CodoCercano", PT + "/BrazoCercano")],
                     "PiernaLejana": [("RodillaLejana", PT + "/PiernaLejana")], "PiernaCercana": [("RodillaCercana", PT + "/PiernaCercana")],
                     "Cuello": [("CaraBase", PT + "/Cuello/Cabeza"), ("Ojos", PT + "/Cuello/Cabeza")]}
        out = [{"nombre": "Perfil", "padre": "Lienzo"}, {"nombre": "Tronco", "padre": PF}]
        for h in orden:
            out.append({"nombre": h, "padre": PT})
            out += [{"nombre": c, "padre": p} for c, p in colgantes.get(h, [])]
        return out

    ordenada = ordena_hijos(tabla_con_orden(viejo), PT, contrato)
    esperada = tabla_con_orden(contrato)
    caso("ordena_hijos lleva la tabla vieja al contrato, cada hijo con lo suyo", ordenada == esperada,
         str([n["nombre"] for n in ordenada if n["padre"] == PT]))
    caso("ordena_hijos: con el contrato ya puesto no cambia nada", ordena_hijos(esperada, PT, contrato) == esperada)
    try:
        ordena_hijos(esperada, PT, contrato + ["Inexistente"])
        caso("ordena_hijos: un nombre que no es hijo se rechaza", False)
    except KeyError:
        caso("ordena_hijos: un nombre que no es hijo se rechaza", True)
    # con arte guardado en el orden anterior, perfil_familia lo entrega en el del contrato
    antes = dict(ARTE_FINAL)
    try:
        ARTE_FINAL["nino"] = dict(ARTE_FINAL.get("nino", {}), perfil={"nodos": tabla_con_orden(viejo), "partes": []})
        pf = perfil_familia("nino", [], [])
        caso("perfil_familia lleva el arte guardado con el orden viejo al del contrato",
             [n["nombre"] for n in pf["nodos"] if n["padre"] == PT] == contrato and pf["orden_tronco"] == contrato)
    finally:
        ARTE_FINAL.clear()
        ARTE_FINAL.update(antes)
    # INC-136: Algoritm con arte final (las entradas «algoritm_<forma>» de arte_final.json que escribe preparar_algoritm.py): la PIERNA ENTERA (PiernaX con su sprite; RodillaX en el palo y
    # sin sprite, o sea AntepiernaX sin PNG), sin sprites de antepierna en ningun nodo, los doce nodos de siempre y el orden de dibujo de Algoritm sin tocar
    for forma, prefab in FORMAS:
        if "algoritm_" + forma not in ARTE_FINAL:
            continue
        g = personaje_guia(forma, prefab)
        n = _por_nombre(g["nodos"])
        pref = "char_algoritm_" + forma
        pref_cara = g.get("prefijo_cara") or pref   # INC-148: con la cara final los nodos nombran la cara COMPARTIDA (char_algoritm_ojos_neutra)
        caso("algoritm_%s: 12 nodos, la pierna entera (RodillaX sin sprite), cara en Ojos y Boca (%s)" % (forma, "final y compartida" if g.get("prefijo_cara") else "provisional"),
             len(g["nodos"]) == 12 and n["PiernaIzq"]["sprite"] == pref + "_parte_pierna_izq" and n["PiernaDer"]["sprite"] == pref + "_parte_pierna_der"
             and n["RodillaIzq"]["sprite"] == "" and n["RodillaDer"]["sprite"] == "" and not any("antepierna" in x["sprite"] for x in g["nodos"])
             and n["Ojos"]["sprite"] == pref_cara + "_ojos_neutra" and n["Boca"]["sprite"] == pref_cara + "_boca_0")
        if g.get("prefijo_cara"):
            caso("algoritm_%s: la cara final comparte rect (Ojos = Boca) en las tres formas y no hay CaraBase" % forma,
                 n["Ojos"]["rect"] == n["Boca"]["rect"] and "CaraBase" not in n and all(
                     _por_nombre(personaje_guia(f2, p2)["nodos"])["Ojos"]["rect"] == n["Ojos"]["rect"] for f2, p2 in FORMAS))
        caso("algoritm_%s: orden_tronco de INC-147 (BrazoIzq, BrazoDer, Torso, Ojos, Boca: brazos detras de todo el cuerpo), cada punto dentro de su rect" % forma,
             g["orden_tronco"] == ["BrazoIzq", "BrazoDer", "Torso", "Ojos", "Boca"] and g["orden_tronco"] == ORDEN_TRONCO_GUIA and all(n[k]["rect"][0] <= n[k]["punto"][0] <= n[k]["rect"][2] and n[k]["rect"][1] <= n[k]["punto"][1] <= n[k]["rect"][3]
                                                            for k in ("PiernaIzq", "PiernaDer", "BrazoIzq", "BrazoDer", "CodoIzq", "CodoDer", "Torso")))
    # INC-148: «prefijo_cara» (Algoritm) y «retrato» (cualquiera) de arte_final.json viajan al JSON, y sin ellos no aparece nada
    antes = dict(ARTE_FINAL)
    try:
        ARTE_FINAL["algoritm_fuego"] = dict(ARTE_FINAL.get("algoritm_fuego", {}), prefijo_cara="char_algoritm",
                                            retrato={"base": "char_algoritm_fuego_retrato_base", "cara": [0.25, 0.3333333, 0.5, 0.4], "iou": 0.99})
        ARTE_FINAL["papa"] = dict(ARTE_FINAL.get("papa", {}), retrato={"base": "char_papa_retrato_base", "cara": [0.1, 0.2, 0.3, 0.4]})
        g = personaje_guia("fuego", "Algoritm_Fuego")
        f = personaje_familia(*FAMILIA[0])
        caso("INC-148: prefijo_cara y retrato de Algoritm viajan al JSON (solo base y cara, redondeados)",
             g.get("prefijo_cara") == "char_algoritm" and g.get("retrato") == {"base": "char_algoritm_fuego_retrato_base", "cara": [0.25, 0.33333, 0.5, 0.4]})
        caso("INC-148: el retrato de la familia viaja al JSON", f.get("retrato") == {"base": "char_papa_retrato_base", "cara": [0.1, 0.2, 0.3, 0.4]})
        ARTE_FINAL["algoritm_fuego"].pop("prefijo_cara"); ARTE_FINAL["algoritm_fuego"].pop("retrato"); ARTE_FINAL["papa"].pop("retrato")
        caso("INC-148: sin prefijo_cara ni retrato el JSON no los trae",
             "prefijo_cara" not in personaje_guia("fuego", "Algoritm_Fuego") and "retrato" not in personaje_guia("fuego", "Algoritm_Fuego") and "retrato" not in personaje_familia(*FAMILIA[0]))
    finally:
        ARTE_FINAL.clear()
        ARTE_FINAL.update(antes)
    caso("el JSON de salida es ASCII", all(ord(c) < 128 for c in compacto({"perfil": [personaje_familia(*f)["perfil"] for f in FAMILIA]})))
    print("autoprueba de articulaciones.py:", "pasa" if not malos else "FALLA en %d casos" % malos)
    return 1 if malos else 0


def main(argv=None):
    if argv and "--autoprueba" in argv:
        return autoprueba()
    personajes = [personaje_familia(*f) for f in FAMILIA]
    personajes += [personaje_guia(*f) for f in FORMAS]
    # La nota va sin tildes ni eñes (y el volcado fuerza ASCII): el JSON debe ser ASCII puro.
    tabla = {
        "version": 1,
        "nota": "Valores PROVISIONALES salvo los de quien tiene entrada en arte_final.json (hoy la familia y las tres formas de Algoritm, INC-136). "
                "Lienzo de 1024, origen arriba a la izquierda, "
                "y hacia abajo. 'nodos' se ANADE a los prefabs (padre antes que hijo); 'partes' describe los "
                "nodos que ya existen. tipo: articulacion = pivote de tamano 0 en 'punto' con un segmento "
                "'imagen' colgado de el; imagen = el propio nodo lleva la Image; grupo = nodo estirado sin Image. "
                "El estado 'segmentado' no se guarda: se deduce del prefab. 'orden_tronco' es el orden de dibujo "
                "(de atras adelante) de los hijos directos de Lienzo/Cuerpo/Tronco: en la familia los humeros van DETRAS DEL TORSO y "
                "DELANTE DE LA CABEZA (decision de Santiago, 05/10/2026: Cuello, BrazoIzq, BrazoDer, Torso; en Papa la cara va despues del torso) y, "
                "desde INC-133 (06/10/2026), los antebrazos (con la mano) DELANTE del torso, de la cara y de las piernas: AntebrazoIzq y AntebrazoDer "
                "al final de la lista (BuildRigsFinal 'orden' los pasa de su codo a Tronco y deja un ancla vacia bajo el codo, que es la que animan los clips); en Algoritm, los brazos "
                "DETRAS DE TODO EL CUERPO: BrazoIzq, BrazoDer, Torso, Ojos, Boca (INC-147, decision de Santiago, 09/10/2026, que revierte para el guia la de INC-132: "
                "sus manos ya no van por encima de la cara). "
                "En las acciones de CharacterRig.armsInFrontActions (Strike) los brazos de la familia pasan delante del torso. Con el arte provisional de Papa, Mama y Nina la cabeza esta pintada "
                "dentro del torso: sus brazos quedan tras torso y cara hasta que llegue su arte final. "
                "PERFIL (INC-134): cada personaje de la familia lleva una clave 'perfil' {provisional, prefijo, nodos, partes, orden_tronco} con el cuerpo de perfil "
                "(Lienzo/Perfil/Tronco, pivote en la cadera, mirando a la DERECHA); 'nodos' es TODO nuevo (padre antes que hijo, en el orden de dibujo) y 'orden_tronco' "
                "el de los hijos de Perfil/Tronco. 'provisional': true = las medidas salen de la figura de frente (no hay arte de perfil todavia); "
                "con una entrada 'perfil' en arte_final.json (preparar_perfil.py) salen de ahi. Las piernas de perfil van SIEMPRE detras del torso (BrazoLejano, PiernaLejana, "
                "PiernaCercana, Torso, Cuello, BrazoCercano; Santiago, 09/10/2026). Algoritm no tiene perfil. ALGORITM (INC-136): sus tres entradas salen de arte_final.json "
                "(preparar_algoritm.py): siete piezas por forma con la PIERNA ENTERA (PiernaX con su sprite; RodillaX en el punto medio del palo, con AntepiernaX sin sprite) y la "
                "cara (Ojos y Boca): hasta INC-148 la provisional, sacada del sprite de hoy; desde INC-148 (10/10/2026) la FINAL de la artista, UNA para las tres formas "
                "(char_algoritm_ojos_*, char_algoritm_boca_*, sin el nombre de la forma): la entrada trae 'prefijo_cara' y BuildRigsFinal busca los sprites de cara con el prefijo de "
                "la forma y despues con ese. 'retrato' (INC-148, preparar_retrato.py): 'base' = el sprite char_<x>_retrato_base de la tarjeta del dialogo (el personaje sin cara, "
                "cuadrado y de 256 px como maximo) y 'cara' = [x, y, ancho, alto] de la cara normalizado sobre ella con el origen ABAJO a la izquierda (CharacterRig.PortraitBase "
                "y PortraitFace; modo «retrato» de BuildRigsFinal).",
        "personajes": personajes,
    }
    with open(SALIDA, "w", encoding="utf-8", newline="\n") as f:
        f.write(compacto(tabla) + "\n")
    print("escrito", os.path.relpath(SALIDA, RAIZ), "con", len(personajes), "personajes")
    for p in personajes:
        pf = p.get("perfil")
        print(" ", p["id"], len(p["nodos"]), "nodos,", len(p["partes"]), "partes" + (
            ", perfil %s: %d nodos" % ("PROVISIONAL" if pf["provisional"] else "con arte", len(pf["nodos"])) if pf else ""))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
