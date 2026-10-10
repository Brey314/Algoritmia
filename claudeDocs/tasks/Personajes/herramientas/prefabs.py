#!/usr/bin/env python3
# Lector de prefabs de personajes y del rig_articulaciones.json, compartido por coreografia.py y
# pose_preview.py. NO es codigo del juego ni escribe nada: solo lee.
#
# De cada prefab YAML (Assets/Game/Prefabs/Characters/<Prefab>.prefab) saca el arbol de nodos con
#   - el orden de los hijos (el orden de dibujo de uGUI: padre antes que hijo, hermanos de atras
#     adelante),
#   - el rect y el pivote de cada RectTransform en el lienzo (la matematica es la de
#     articulaciones.leer_prefab y la de BoxOf de BuildRigsFinal.cs.txt: solo anclas, pivote y
#     tamano, sin Canvas),
#   - la Image de cada nodo (encendida o no, sprite o no, preservar aspecto, color).
# Coordenadas del lienzo: 1024, origen ARRIBA a la izquierda, y hacia ABAJO, como cut.py y el JSON.
#
# PERFIL (INC-134, 09/10/2026). Las constantes PF, PT, PBL… son las rutas del cuerpo de perfil (Lienzo/Perfil, hermano de Cuerpo; HUESOS_PERFIL sus 12
# articulaciones) y ACCIONES_PERFIL las acciones que se ven de perfil (la tabla de ActionView.cs, que acciones_de_perfil() lee del .cs). Este lector NO dibuja el
# perfil (no hay arte ni nodos en los prefabs hasta que corra BuildRigsFinal «perfil»): pose_preview.py y la coreografia de frente no lo ven. Y _png_de resuelve
# por NOMBRE con la regla de carpetas de los dos .cs.txt (subcarpeta_de): los char_<id>_perfil_* se buscan en Perfil/ y no chocan con los de frente.
#
# DOS FORMAS DE ANTEBRAZO EN EL PREFAB (INC-133, 06/10/2026), y el arbol que sale de leer_arbol es el mismo:
#   - ANTERIOR (hasta 433603f~1, y Algoritm, que no lista antebrazos): Tronco/BrazoX/CodoX/AntebrazoX; sin ancla.
#   - ACTUAL (la familia tras BuildRigsFinal «orden»): AntebrazoX es el ULTIMO hijo de Tronco (se dibuja delante del torso, de la cara y de las
#     piernas) y bajo cada codo queda un nodo vacio AnclaAntebrazoX con la pose que el antebrazo tenia bajo el codo, que es la que animan los
#     clips; LimbFollower (motor) copia la pose del ancla al antebrazo en cada cuadro, asi que el antebrazo SIGUE donde estaba y solo cambia el
#     orden de dibujo.
# Para la geometria y la cinematica (matrices, hombro, codo, mano) leer_arbol deja el antebrazo BAJO SU CODO, en el sitio del ancla (la ruta es
# siempre Tronco/BrazoX/CodoX/AntebrazoX, con su Image, sprite y rect), lo marca «sigue» y guarda en Tronco.dibujo el orden de dibujo del
# prefab, con el antebrazo donde esta en Tronco. Sobre el arbol anterior la misma simulacion la hace aplicar_orden_tronco.

import json
import os
import re
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import articulaciones as _art  # noqa: E402  (solo para leer_prefab: la geometria de cada nodo)

RAIZ = _art.RAIZ
PREFABS = _art.PREFABS
ARTE = os.path.join(RAIZ, "Assets", "Game", "Art")
PERSONAJES_ARTE = os.path.join(ARTE, "Characters")
RIG_JSON = os.path.join(AQUI, "rig_articulaciones.json")
LIENZO = 1024.0
SUELO = 947.0

C = "Lienzo/Cuerpo"
T = C + "/Tronco"
LA = T + "/BrazoIzq"
RA = T + "/BrazoDer"
LE = LA + "/CodoIzq"
RE = RA + "/CodoDer"
LL = C + "/PiernaIzq"
RL = C + "/PiernaDer"
LK = LL + "/RodillaIzq"
RK = RL + "/RodillaDer"
NK = T + "/Cuello"
HD = NK + "/Cabeza"

# INC-134 (09/10/2026): el cuerpo de PERFIL de la familia, un segundo cuerpo junto a Lienzo/Cuerpo que se enciende en las acciones de
# ActionView (Walk, Run, Carry, Push, PickUp, Kneel, Blow). Todo cuelga de UN Tronco con el pivote en la cadera, y los hijos de Tronco van en
# este orden de dibujo (de atras adelante): BrazoLejano, PiernaLejana, PiernaCercana, Torso, Cuello, BrazoCercano (las piernas, SIEMPRE detras del torso:
# Santiago, 09/10/2026). «Cercano» es el lado que
# mira al espectador; el arte canonico mira a la DERECHA (el motor voltea el Lienzo para mirar a la izquierda). Papa, Mama, Nina y Nino; Algoritm no tiene.
PF = "Lienzo/Perfil"
PT = PF + "/Tronco"
PBL = PT + "/BrazoLejano"
PEL = PBL + "/CodoLejano"
PBC = PT + "/BrazoCercano"
PEC = PBC + "/CodoCercano"
PLL = PT + "/PiernaLejana"
PKL = PLL + "/RodillaLejana"
PLC = PT + "/PiernaCercana"
PKC = PLC + "/RodillaCercana"
PNK = PT + "/Cuello"
PHD = PNK + "/Cabeza"
# las 12 articulaciones con rotacion de perfil (el espejo de las 12 de frente): cada clip de la familia lleva una curva en cada una
HUESOS_PERFIL = [PF, PT, PBL, PBC, PEL, PEC, PLL, PLC, PKL, PKC, PNK, PHD]
# las acciones que se ven de perfil: la MISMA tabla que ActionView.cs (Game.Scaffolding); si cambia una fila alli, cambia aqui
ACCIONES_PERFIL = ("Walk", "Run", "Carry", "Push", "PickUp", "Kneel", "Blow")

# id del JSON -> (prefab, carpeta de arte, guia)
PERSONAJES = {
    "papa": ("Papa", "Father", False),
    "mama": ("Mama", "Mother", False),
    "nina": ("Nina", "Girl", False),
    "nino": ("Nino", "Boy", False),
    "algoritm_fuego": ("Algoritm_Fuego", "Algoritm", True),
    "algoritm_rueda": ("Algoritm_Rueda", "Algoritm", True),
    "algoritm_gota": ("Algoritm_Gota", "Algoritm", True),
}
FAMILIA = ["papa", "mama", "nina", "nino"]

_GUIA_IMAGE = "fe87c0e1cc204ed48ad3b37840f39efc"  # guid del script UnityEngine.UI.Image


def cargar_rig(ruta=RIG_JSON):
    with open(ruta, encoding="utf-8") as f:
        return json.load(f)


def personaje_rig(rig, pid):
    for p in rig["personajes"]:
        if p["id"] == pid:
            return p
    raise KeyError(pid)


_GUIDS = None


def sprite_por_guid(guid):
    """
    Ruta del .png que tiene ese guid (busca en los .png.meta de Assets/Game/Art), o None. Un «guid» que
    empieza por «ruta:» (lo pone simula_sprites) ya es la ruta del PNG.
    """
    global _GUIDS
    if guid and guid.startswith("ruta:"):
        return guid[5:]
    if _GUIDS is None:
        _GUIDS = {}
        for base, _, archivos in os.walk(ARTE):
            for a in archivos:
                if a.endswith(".png.meta"):
                    with open(os.path.join(base, a), encoding="utf-8") as f:
                        m = re.search(r"^guid: ([0-9a-f]{32})", f.read(), re.M)
                    if m:
                        _GUIDS[m.group(1)] = os.path.join(base, a[:-5])
    return _GUIDS.get(guid)


class Nodo:
    """Un RectTransform del prefab con lo que el dibujo necesita."""

    def __init__(self, ruta, nombre):
        self.ruta = ruta
        self.nombre = nombre
        self.padre = None
        self.hijos = []
        self.rect = None      # (x0, y0, x1, y1) en el lienzo, y hacia abajo
        self.pivote = None    # (x, y) en el lienzo
        self.activo = True
        self.imagen = None    # dict: encendida, guid, aspecto, color; o None si no tiene Image
        self.alfa = None      # CanvasGroup.m_Alpha si lo tiene
        # INC-133: un antebrazo que «orden_tronco» saca de su codo se DIBUJA como hijo de Tronco pero sigue a su ancla bajo el codo,
        # asi que su pose de mundo no cambia: aqui se conserva en el arbol, bajo el codo, para toda la geometria (matrices, manos), y
        # solo cambia el orden de dibujo. «sigue» lo marca (leer_arbol, si el prefab ya tiene el ancla, o aplicar_orden_tronco); «dibujo»
        # (solo en Tronco) es la lista de hijos en orden de dibujo, con ellos.
        self.sigue = False
        self.dibujo = None

    def dibuja(self):
        """Una Image encendida y con sprite es lo unico que uGUI dibuja."""
        i = self.imagen
        return bool(self.activo and i and i["encendida"] and i["guid"])


def leer_arbol(prefab, carpeta=None):
    """
    Devuelve {ruta: Nodo} (la ruta no incluye el nombre de la raiz) con 'hijos' en orden de dibujo. «carpeta»: donde esta el .prefab
    (por defecto Assets/Game/Prefabs/Characters; la autoprueba lee ahi unos prefabs sinteticos).
    """
    ruta = os.path.join(carpeta or PREFABS, prefab + ".prefab")
    geom = _art.leer_prefab(ruta)

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

    def campo(d, nombre):
        for l in d["lineas"]:
            s = l.strip()
            if s.startswith(nombre + ":"):
                return s[len(nombre) + 1:].strip()
        return None

    def fid(texto):
        return re.search(r"fileID: (-?\d+)", texto).group(1)

    go = {k: d for k, d in docs.items() if d["cls"] == 1}
    nombres = {k: campo(d, "m_Name") for k, d in go.items()}
    activo = {k: campo(d, "m_IsActive") != "0" for k, d in go.items()}
    imagenes = {}
    grupos = {}
    for k, d in docs.items():
        if d["cls"] == 114 and "UnityEngine.UI.Image" in (campo(d, "m_EditorClassIdentifier") or ""):
            spr = campo(d, "m_Sprite") or ""
            g = re.search(r"guid: ([0-9a-f]{32})", spr)
            col = re.findall(r"[rgba]: ([0-9.]+)", campo(d, "m_Color") or "")
            imagenes[fid(campo(d, "m_GameObject"))] = {
                "encendida": campo(d, "m_Enabled") != "0",
                "guid": g.group(1) if g else None,
                "aspecto": campo(d, "m_PreserveAspect") == "1",
                "color": tuple(float(c) for c in col) if len(col) == 4 else (1, 1, 1, 1),
            }
        elif d["cls"] == 225:
            grupos[fid(campo(d, "m_GameObject"))] = float(campo(d, "m_Alpha"))

    rts = {}
    for k, d in docs.items():
        if d["cls"] != 224:
            continue
        hijos = []
        dentro = False
        for l in d["lineas"]:
            if l.startswith("  m_Children"):
                dentro = True
                continue
            if dentro:
                if l.startswith("  - {fileID"):
                    hijos.append(fid(l))
                else:
                    dentro = False
        rts[k] = {"go": fid(campo(d, "m_GameObject")), "hijos": hijos, "padre": fid(campo(d, "m_Father"))}

    raiz = [k for k, v in rts.items() if v["padre"] == "0"][0]
    nodos = {}

    def visita(k, prefijo, es_raiz):
        v = rts[k]
        nombre = nombres[v["go"]]
        r = "" if es_raiz else (prefijo + "/" + nombre if prefijo else nombre)
        n = Nodo(r, nombre)
        n.activo = activo[v["go"]]
        n.imagen = imagenes.get(v["go"])
        n.alfa = grupos.get(v["go"])
        if r in geom:
            x0, y0, x1, y1, px, py = geom[r]
            n.rect = (x0, y0, x1, y1)
            n.pivote = (px, py)
        nodos[r] = n
        for h in v["hijos"]:
            hn = visita(h, r, False)
            hn.padre = n
            n.hijos.append(hn)
        return n

    visita(raiz, "", True)
    _antebrazos_a_su_codo(nodos)
    return nodos


def _antebrazos_a_su_codo(nodos):
    """
    El prefab con la jerarquia de INC-133 (AntebrazoX al final de Tronco y AnclaAntebrazoX bajo el codo) pasa al arbol de simulacion: el
    antebrazo vuelve a su codo, en el sitio del ancla (la pose que el ancla tiene y el antebrazo copia en el motor) y con su propio tamano,
    conserva su Image y se marca «sigue»; el ancla desaparece del arbol y Tronco.dibujo guarda el orden de dibujo del prefab, con los
    antebrazos donde estan en Tronco (al final). Tronco.hijos queda sin ellos, como en el arbol anterior tras aplicar_orden_tronco.
    Un lado sin ancla (Algoritm, o un prefab anterior a INC-133) no se toca.
    """
    tronco = nodos.get(T)
    if tronco is None:
        return
    dibujo = list(tronco.hijos)
    movidos = []
    for lado, ruta_codo in (("Izq", LE), ("Der", RE)):
        codo = nodos.get(ruta_codo)
        ancla = nodos.get(ruta_codo + "/AnclaAntebrazo" + lado)
        ante = nodos.get(T + "/Antebrazo" + lado)
        if codo is None or ancla is None or ante is None:
            continue
        ruta = ruta_codo + "/Antebrazo" + lado
        _quita_del_arbol(nodos, ancla)
        _quita_del_arbol(nodos, ante)
        ante.padre = codo
        if ante.rect is not None and ancla.pivote is not None:
            # LimbFollower copia la POSICION del ancla (donde esta su pivote) y el antebrazo conserva su tamano y su pivote: el rect
            # propio, con el pivote donde lo pone el ancla. En el prefab guardado los dos rects coinciden y no cambia nada.
            dx, dy = ancla.pivote[0] - ante.pivote[0], ancla.pivote[1] - ante.pivote[1]
            ante.rect = (ante.rect[0] + dx, ante.rect[1] + dy, ante.rect[2] + dx, ante.rect[3] + dy)
            ante.pivote = ancla.pivote
        ante.sigue = True
        codo.hijos = [ante if h is ancla else h for h in codo.hijos]
        _pon_en_el_arbol(nodos, ante, ruta)
        movidos.append(ante)
    if movidos:
        tronco.hijos = [h for h in tronco.hijos if h not in movidos]
        tronco.dibujo = dibujo


def _quita_del_arbol(nodos, n):
    """Borra el nodo y todo lo que cuelga de el del diccionario {ruta: Nodo} (no toca los hijos de nadie)."""
    nodos.pop(n.ruta, None)
    for h in n.hijos:
        _quita_del_arbol(nodos, h)


def _pon_en_el_arbol(nodos, n, ruta):
    """Registra el nodo (y lo que cuelga de el) bajo esa ruta, con la ruta puesta en cada uno."""
    n.ruta = ruta
    nodos[ruta] = n
    for h in n.hijos:
        _pon_en_el_arbol(nodos, h, ruta + "/" + h.nombre)


# {nombre de sprite: ruta de un PNG}: lo que una herramienta quiere VER antes de escribirlo en el repo (el composite de
# preparar_expresion.py, sin --aplicar). Manda sobre lo que haya en disco.
PNG_EXTRA = {}


def subcarpeta_de(nombre):
    """
    La subcarpeta de arte que le toca a un PNG por su NOMBRE: la misma regla que ExpectedSubfolder (BuildRigsFinal.cs.txt) y SubfolderFor
    (OrganizarArtePersonajes.cs.txt). «_perfil_» MANDA sobre las demas (INC-134): char_<x>_perfil_ojos_neutra contiene «_ojos_», pero es
    de Perfil/ y no de Expresiones/, y char_<x>_perfil_cara_base contiene «_cara_base». Las partes del cuerpo de frente (char_<x>_parte_*)
    van a Frontal/ y los ojos, bocas y la cara base de frente a Expresiones/. None = ninguna (retratos, reposos de Algoritm: la raiz).
    """
    if "_perfil_" in nombre:
        return "Perfil"
    if "_parte_" in nombre:
        return "Frontal"
    if "_ojos_" in nombre or "_boca_" in nombre or "_cara_base" in nombre:
        return "Expresiones"
    return None


def _png_de(carpeta, nombre):
    """
    Ruta del PNG «nombre» en la carpeta de arte del personaje o en sus subcarpetas (Frontal/, Expresiones/, Perfil/: el arte
    se guarda por subcarpetas), o None. Si el PNG esta en PNG_EXTRA, el de ahi. Si hay dos con el mismo nombre (uno olvidado en la raiz o
    en la subcarpeta equivocada) gana el de la subcarpeta que le toca por su nombre (subcarpeta_de), luego el de la raiz, luego el primero
    por orden alfabetico: un nombre de perfil se resuelve siempre en Perfil/ y uno de frente nunca en Perfil/ mientras exista fuera de ella.
    Los nombres de perfil (char_<x>_perfil_*) y los de frente no se confunden: se busca el nombre ENTERO, y «_perfil_» los separa.
    """
    if nombre in PNG_EXTRA:
        return PNG_EXTRA[nombre]
    base = os.path.join(PERSONAJES_ARTE, carpeta)
    esperada = subcarpeta_de(nombre)
    candidatos = []
    for dentro, _, archivos in sorted(os.walk(base)):
        if nombre + ".png" in archivos:
            candidatos.append(os.path.join(dentro, nombre + ".png"))
    if not candidatos:
        return None

    def rango(ruta):
        carpeta_png = os.path.basename(os.path.dirname(ruta))
        if esperada is not None and carpeta_png == esperada:
            return 0
        return 1 if os.path.normpath(os.path.dirname(ruta)) == os.path.normpath(base) else 2

    return sorted(candidatos, key=lambda r: (rango(r), r))[0]


def caja_contenido(png, rect):
    """
    El rect (x0, y0, x1, y1) del lienzo que ocupan los pixeles OPACOS del PNG cuando se estira a «rect». Ojos y boca comparten
    un rect del tamano de toda la cara (todas las expresiones tienen el mismo lienzo para que no se deformen al cambiar), asi
    que «la caja de la cara» de las pruebas es la de lo que se pinta, no la del rect entero. Sin PNG, el rect.
    """
    if png is None:
        return tuple(rect)
    from PIL import Image
    a = Image.open(png).convert("RGBA").getchannel("A").point(lambda v: 255 if v > 16 else 0)
    b = a.getbbox()
    if b is None:
        return tuple(rect)
    w, h = a.size
    rw, rh = rect[2] - rect[0], rect[3] - rect[1]
    return (rect[0] + b[0] / w * rw, rect[1] + b[1] / h * rh, rect[0] + b[2] / w * rw, rect[1] + b[3] / h * rh)


def simula_sprites(arbol, rig_pj, apaga_faltantes=False):
    """
    El estado del prefab DESPUES de correr «sprites» en el Editor (BuildRigsFinal.cs.txt): cada parte y cada
    nodo de imagen que el rig_articulaciones.json nombra y cuyo PNG existe en Assets/Game/Art/Characters/<carpeta>
    queda con su Image encendida y con ese sprite. Asi, entre copiar el arte final al repo y correr el generador
    (el prefab del disco aun es el provisional), coreografia.py y pose_preview.py ya tratan al personaje como
    lo que sera: brazos y piernas partidos, cabeza propia. Es idempotente: sobre un prefab que ya paso por
    «sprites» no cambia nada. Lo que no tiene PNG se deja como esta (apagado).

    «apaga_faltantes» (Algoritm, INC-136, 09/10/2026): «sprites» tambien APAGA y VACIA la Image de un nodo de la tabla cuyo PNG no existe o cuya tabla no le da
    sprite (la AntepiernaX de Algoritm: la pierna es entera y el PNG de la antepierna se borro; sin esto el prefab del disco, que aun la lleva encendida, la daria
    por «segmentada»). Solo para los guias: en la familia un nodo sin PNG nace apagado y no se toca.
    """
    carpeta = rig_pj["carpeta"]
    candidatos = []
    for parte in rig_pj["partes"]:
        candidatos.append((parte["ruta"], parte["sprite"]))
    for nodo in rig_pj["nodos"]:
        if not nodo.get("sprite"):
            continue
        base = nodo["padre"] + "/" + nodo["nombre"]
        candidatos.append((base + "/" + nodo["imagen"] if nodo["tipo"] == "articulacion" else base, nodo["sprite"]))
    nuevos = {}
    for nodo in rig_pj["nodos"]:
        if nodo["tipo"] == "imagen":
            nuevos[nodo["padre"] + "/" + nodo["nombre"]] = nodo
    for ruta, sprite in candidatos:
        n = arbol.get(ruta)
        png = _png_de(carpeta, sprite)
        if png is None:
            continue
        if n is None and ruta in nuevos and arbol.get(nuevos[ruta]["padre"]) is not None:
            # un nodo de imagen que el JSON nombra y el prefab del disco aun no tiene (CaraBase llega con «nodos»): se simula, para
            # poder ver la cara antes de correr el generador. Va en el sitio que dice el JSON: CaraBase, el primero de Cabeza.
            nd = nuevos[ruta]
            padre = arbol[nd["padre"]]
            n = Nodo(ruta, nd["nombre"])
            n.padre = padre
            n.rect = tuple(float(v) for v in nd["rect"])
            n.pivote = tuple(float(v) for v in nd["punto"])
            n.hijos = []
            if nd["nombre"] == "CaraBase":
                padre.hijos.insert(0, n)
            else:
                padre.hijos.append(n)
            arbol[ruta] = n
        if n is None:
            continue
        previa = n.imagen or {}
        n.activo = True
        n.imagen = {"encendida": True, "guid": "ruta:" + png, "aspecto": previa.get("aspecto", False),
                    "color": previa.get("color", (1.0, 1.0, 1.0, 1.0))}
    if apaga_faltantes:
        con_png = {ruta for ruta, sprite in candidatos if _png_de(carpeta, sprite) is not None}
        for nodo in rig_pj["nodos"]:
            if nodo["tipo"] not in ("imagen", "articulacion"):
                continue
            base = nodo["padre"] + "/" + nodo["nombre"]
            ruta = base + "/" + nodo["imagen"] if nodo["tipo"] == "articulacion" else base
            n = arbol.get(ruta)
            if n is not None and n.imagen is not None and ruta not in con_png:
                n.imagen = dict(n.imagen, encendida=False, guid=None)
    return arbol


def arbol_vigente(pid, rig=None):
    """
    El arbol del prefab del personaje tal como quedara tras «sprites» (simula_sprites) y «orden» (aplicar_orden_tronco: el orden de
    dibujo de Tronco con los antebrazos delante, INC-133). Algoritm pasa por «sprites» desde INC-136 (siete piezas de Frontal/ y la cara de Expresiones/; la
    AntepiernaX, sin PNG, queda apagada). Es el que usan
    coreografia.py (leer_contexto: lo que tapa a cada brazo depende del orden), pose_preview.py y maqueta.py. Da el mismo arbol con un prefab
    que ya paso por «orden» (leer_arbol lo lee con el antebrazo bajo su codo y «sigue») que con uno anterior (aplicar_orden_tronco lo simula).
    """
    rig = rig or cargar_rig()
    arbol = leer_arbol(PERSONAJES[pid][0])
    pj = personaje_rig(rig, pid)
    simula_sprites(arbol, pj, apaga_faltantes=PERSONAJES[pid][2])   # Algoritm tambien (INC-136): sus siete piezas y la cara provisional salen de Frontal/ y Expresiones/
    if pj.get("orden_tronco"):
        aplicar_orden_tronco(arbol, pj["orden_tronco"])
    return arbol


def esta_segmentado(arbol):
    """Segmentado = la Image de .../RodillaIzq/AntepiernaIzq tiene sprite y esta encendida (IsSegmented del C#)."""
    n = arbol.get(LK + "/AntepiernaIzq")
    return bool(n is not None and n.dibuja())


def _antebrazo_bajo_codo(arbol, nombre):
    """El nodo AntebrazoIzq/AntebrazoDer que cuelga de su codo, o None (INC-133: «orden_tronco» puede listarlo para sacarlo de el)."""
    for ruta in (LE + "/AntebrazoIzq", RE + "/AntebrazoDer"):
        if ruta.endswith("/" + nombre) and ruta in arbol:
            return arbol[ruta]
    return None


def aplicar_orden_tronco(arbol, orden):
    """
    Simula lo que hace el C# con «orden_tronco» (BuildRigsFinal «orden»): el orden de dibujo de los hijos de Tronco (de atras
    adelante). Desde INC-133 la lista puede nombrar AntebrazoIzq/AntebrazoDer: BuildRigsFinal los pasa de su codo a Tronco y deja
    bajo el codo un ancla vacia que animan los clips; CharacterRig copia su pose al antebrazo en cada cuadro, asi que el antebrazo
    sigue donde estaba y SOLO cambia el orden de dibujo. Aqui eso es: el nodo se queda bajo el codo (la geometria no cambia),
    se marca «sigue» y Tronco guarda en «dibujo» la lista de hijos en orden de dibujo, con el antebrazo donde diga la lista.
    Sobre un prefab que YA lo paso (leer_arbol los deja «sigue») sirve igual para probar otro orden de dibujo; un antebrazo que sigue a
    su ancla y la lista no nombra queda al final, donde esta en el prefab (nunca se pierde: «recorre» de pose_preview no lo dibuja bajo el codo).
    """
    tronco = arbol[T]
    por_nombre = {h.nombre: h for h in tronco.hijos}
    sin_nombrar = [a for a in (arbol.get(LE + "/AntebrazoIzq"), arbol.get(RE + "/AntebrazoDer"))
                   if a is not None and a.sigue and a.nombre not in orden]
    seguidores = {}
    faltan = []
    for n in orden:
        if n in por_nombre:
            continue
        ante = _antebrazo_bajo_codo(arbol, n)
        if ante is None:
            faltan.append(n)
        else:
            seguidores[n] = ante
    if faltan:
        raise KeyError("orden_tronco nombra hijos que no existen: %s" % faltan)
    for n in seguidores.values():
        n.sigue = True
    resto = [h for h in tronco.hijos if h.nombre not in orden]
    tronco.dibujo = [por_nombre[n] if n in por_nombre else seguidores[n] for n in orden] + resto + sin_nombrar
    # los hijos que no cambian de padre conservan ese orden tambien en «hijos» (el que usan las matrices: no depende del orden)
    tronco.hijos = [por_nombre[n] for n in orden if n in por_nombre] + resto


def hijos_de_dibujo(n):
    """Los hijos de un nodo en orden de dibujo: Tronco con «orden_tronco» aplicado trae su lista (con los antebrazos), el resto sus hijos."""
    return n.dibujo if n.dibujo is not None else n.hijos


CHARACTER_RIG_CS = os.path.join(RAIZ, "Assets", "Game", "Scripts", "Runtime", "Scaffolding", "CharacterRig.cs")
ACCIONES_DELANTE_POR_DEFECTO = {"Strike"}


def lee_acciones_delante(texto):
    """
    Las acciones de la linea «private ActorAction[] armsInFrontActions = { ActorAction.Strike, ... };» de CharacterRig.cs
    (el contrato: en ellas los brazos se dibujan DELANTE del torso, en tiempo de ejecucion), o None si no la encuentra.
    """
    m = re.search(r"ActorAction\s*\[\s*\]\s+armsInFrontActions\s*=\s*(?:new\s+ActorAction\s*\[\s*\]\s*)?\{([^}]*)\}", texto)
    if not m:
        return None
    return set(re.findall(r"ActorAction\.(\w+)", m.group(1)))


def acciones_brazos_delante(ruta=CHARACTER_RIG_CS):
    """
    (acciones, encontrada): las acciones en que los brazos pasan delante del torso, leidas de CharacterRig.cs. Si el
    archivo aun no trae la lista, {«Strike»} y encontrada = False (quien llame lo avisa).
    """
    try:
        with open(ruta, encoding="utf-8") as f:
            acciones = lee_acciones_delante(f.read())
    except OSError:
        acciones = None
    if acciones is None:
        return set(ACCIONES_DELANTE_POR_DEFECTO), False
    return acciones, True


def orden_con_brazos_delante(nombres):
    """
    El orden de los hijos de Tronco en las acciones de «brazos delante» (el contrato de CharacterRig): cada BrazoIzq/BrazoDer
    que se dibuja antes de Torso pasa inmediatamente despues de Torso; el que ya esta despues no cambia (desde INC-147, 09/10/2026, tampoco los de
    Algoritm van despues: [BrazoIzq, BrazoDer, Torso, Ojos, Boca], asi que la regla los pasa entre el torso y la cara, igual que ArmLayering). INC-133: los
    antebrazos (AntebrazoIzq/Der, al final de la lista de la familia) no se mueven: el humero queda justo tras el torso y sigue
    DETRAS de ellos.
    """
    if "Torso" not in nombres:
        return list(nombres)
    t = nombres.index("Torso")
    mover = [n for n in nombres[:t] if n in ("BrazoIzq", "BrazoDer")]
    resto = [n for n in nombres if n not in mover]
    i = resto.index("Torso")
    return resto[:i + 1] + mover + resto[i + 1:]


ACTION_VIEW_CS = os.path.join(RAIZ, "Assets", "Game", "Scripts", "Runtime", "Scaffolding", "ActionView.cs")


def lee_acciones_perfil(texto):
    """
    Las acciones de ActionView.cs (la tabla de INC-134: que accion se ve de perfil) que devuelven CharacterView.Profile: los «case ActorAction.X:» que
    se apilan antes de «return CharacterView.Profile;». None si no encuentra ese return. Es el contrato con el motor: ACCIONES_PERFIL tiene que ser esta lista.
    """
    m = re.search(r"((?:\s*case\s+ActorAction\.\w+\s*:)+)\s*return\s+CharacterView\.Profile\s*;", texto)
    if not m:
        return None
    return set(re.findall(r"ActorAction\.(\w+)", m.group(1)))


def acciones_de_perfil(ruta=ACTION_VIEW_CS):
    """(acciones, encontrada): las de ActionView.cs si el archivo existe y se lee; si no, ACCIONES_PERFIL (el contrato acordado) y encontrada = False."""
    try:
        with open(ruta, encoding="utf-8") as f:
            acciones = lee_acciones_perfil(f.read())
    except OSError:
        acciones = None
    if acciones is None:
        return set(ACCIONES_PERFIL), False
    return acciones, True


def autoprueba_contrato():
    """
    Lo que la lectura de la lista tiene que cumplir, para las autopruebas de coreografia.py y pose_preview.py:
    [(nombre, ok)]. Con CharacterRig.cs real: si menciona armsInFrontActions la regex TIENE que encontrarla (si no, la
    coreografia seguiria con la lista por defecto sin enterarse de que el contrato cambio de forma).
    """
    casos = []
    uno = "private ActorAction[] armsInFrontActions = { ActorAction.Strike };"
    casos.append(("la linea del contrato", lee_acciones_delante(uno) == {"Strike"}))
    dos = "[SerializeField] private static readonly ActorAction[] armsInFrontActions = new ActorAction[] {\n  ActorAction.Strike,\n  ActorAction.Hammer };"
    casos.append(("varias acciones, con new y saltos", lee_acciones_delante(dos) == {"Strike", "Hammer"}))
    casos.append(("sin la lista", lee_acciones_delante("private ActorAction[] otra = { ActorAction.Strike };") is None))
    try:
        with open(CHARACTER_RIG_CS, encoding="utf-8") as f:
            real = f.read()
    except OSError:
        real = None
    if real is not None and "armsInFrontActions" in real:
        casos.append(("CharacterRig.cs trae la lista y se lee", lee_acciones_delante(real) is not None))
    # INC-134: la tabla de ActionView.cs. La lectura sobre un texto sintetico, y (si el archivo existe) que ACCIONES_PERFIL sea justo esa tabla
    sintetico = "switch (action)\n{\n case ActorAction.Walk:\n case ActorAction.Kneel:\n return CharacterView.Profile;\n default:\n return CharacterView.Front;\n}"
    casos.append(("ActionView: la lectura de la tabla", lee_acciones_perfil(sintetico) == {"Walk", "Kneel"}))
    casos.append(("ActionView: sin tabla", lee_acciones_perfil("return CharacterView.Front;") is None))
    acciones, hallada = acciones_de_perfil()
    if hallada:
        casos.append(("ActionView.cs coincide con ACCIONES_PERFIL", acciones == set(ACCIONES_PERFIL)))
    casos += autoprueba_nombres()
    casos += autoprueba_reglas_cs()
    return casos


def autoprueba_reglas_cs():
    """
    [(nombre, ok)]: los dos .cs.txt que reparten el arte por nombre (BuildRigsFinal: ExpectedSubfolder; OrganizarArtePersonajes: SubfolderFor) tienen la regla
    de perfil ANTES que las demas (INC-134): sin ella char_<x>_perfil_ojos_neutra iria a Expresiones/ por contener «_ojos_». Es la misma regla que subcarpeta_de.
    """
    casos = []
    for archivo, funcion in (("BuildRigsFinal.cs.txt", "ExpectedSubfolder"), ("OrganizarArtePersonajes.cs.txt", "SubfolderFor")):
        try:
            with open(os.path.join(AQUI, archivo), encoding="utf-8") as f:
                texto = f.read()
        except OSError:
            continue
        m = re.search(r"static string " + funcion + r"\([^)]*\)\s*\{(.*?)\n    \}", texto, re.S)
        cuerpo = m.group(1) if m else ""
        i, j = cuerpo.find('"_perfil_"'), min([k for k in (cuerpo.find('"_parte_"'), cuerpo.find('"_ojos_"')) if k >= 0] or [10 ** 9])
        casos.append(("reglas: %s da prioridad a _perfil_" % funcion, m is not None and 0 <= i < j))
    return casos


def autoprueba_nombres():
    """
    [(nombre, ok)]: la regla de carpetas por nombre (subcarpeta_de) y _png_de sobre un arbol de arte SINTETICO en un directorio temporal (PERSONAJES_ARTE se
    desvia mientras dura la prueba): lo de perfil se resuelve en Perfil/ aunque su nombre contenga «_ojos_», «_boca_» o «_cara_base» y no se confunde con
    lo de frente; con una copia perdida fuera de su carpeta gana la que esta en la que le toca; lo que no existe es None.
    """
    import tempfile
    global PERSONAJES_ARTE
    casos = []
    reglas = {"char_papa_perfil_ojos_neutra": "Perfil", "char_papa_perfil_boca_0": "Perfil", "char_papa_perfil_cara_base": "Perfil",
              "char_papa_perfil_torso": "Perfil", "char_papa_perfil_antepierna_cercana": "Perfil",
              "char_papa_parte_torso": "Frontal", "char_papa_ojos_neutra": "Expresiones", "char_papa_boca_a": "Expresiones", "char_papa_cara_base": "Expresiones",
              "char_papa_retrato_neutra": None}
    casos.append(("nombres: subcarpeta_de (perfil manda sobre ojos, boca y cara_base)", all(subcarpeta_de(n) == v for n, v in reglas.items())))
    previo = PERSONAJES_ARTE
    with tempfile.TemporaryDirectory() as tmp:
        base = os.path.join(tmp, "Father")
        for sub, nombre in (("Frontal", "char_papa_parte_torso"), ("Expresiones", "char_papa_ojos_neutra"), ("Expresiones", "char_papa_cara_base"),
                            ("Perfil", "char_papa_perfil_ojos_neutra"), ("Perfil", "char_papa_perfil_cara_base"),
                            ("Expresiones", "char_papa_perfil_torso"), ("Perfil", "char_papa_perfil_torso"), (".", "char_papa_perfil_boca_0"),
                            ("Perfil", "char_papa_ojos_neutra"), (".", "char_papa_retrato_neutra")):
            os.makedirs(os.path.join(base, sub), exist_ok=True)
            open(os.path.join(base, sub, nombre + ".png"), "wb").close()

        def donde(nombre):
            ruta = _png_de("Father", nombre)
            return None if ruta is None else os.path.relpath(ruta, base).replace(os.sep, "/")

        PERSONAJES_ARTE = tmp
        try:
            casos.append(("nombres: un nombre de perfil se resuelve en Perfil/", donde("char_papa_perfil_ojos_neutra") == "Perfil/char_papa_perfil_ojos_neutra.png"
                          and donde("char_papa_perfil_cara_base") == "Perfil/char_papa_perfil_cara_base.png"))
            casos.append(("nombres: uno de frente nunca cae en Perfil/ si existe fuera", donde("char_papa_ojos_neutra") == "Expresiones/char_papa_ojos_neutra.png"
                          and donde("char_papa_parte_torso") == "Frontal/char_papa_parte_torso.png"))
            casos.append(("nombres: con una copia perdida gana la de su carpeta", donde("char_papa_perfil_torso") == "Perfil/char_papa_perfil_torso.png"))
            casos.append(("nombres: sin copia en su carpeta se usa la que haya", donde("char_papa_perfil_boca_0") == "char_papa_perfil_boca_0.png"
                          and donde("char_papa_retrato_neutra") == "char_papa_retrato_neutra.png"))
            casos.append(("nombres: lo que no existe es None", donde("char_papa_perfil_ojos_alegria") is None))
        finally:
            PERSONAJES_ARTE = previo
    return casos


def _yaml_sintetico(raiz):
    """
    El YAML minimo de un prefab (GameObject, RectTransform e Image: lo unico que leen leer_arbol y articulaciones.leer_prefab) de un arbol
    de dicts {n, pos, size, estira, guid, hijos}. Solo para autoprueba_arbol: nada de esto se escribe en el repo.
    """
    cuenta = [100]

    def numera(d):
        d["go"], d["rt"], d["im"] = cuenta[0] + 1, cuenta[0] + 2, cuenta[0] + 3
        cuenta[0] += 3
        for h in d["hijos"]:
            numera(h)

    docs = []

    def emite(d, padre):
        amin, amax = ((0, 0), (1, 1)) if d["estira"] else ((0.5, 0.5), (0.5, 0.5))
        hijos = "".join("  - {fileID: %d}\n" % h["rt"] for h in d["hijos"])
        docs.append("--- !u!1 &%d\nGameObject:\n  m_Name: %s\n  m_IsActive: 1\n" % (d["go"], d["n"]))
        docs.append("--- !u!224 &%d\nRectTransform:\n  m_GameObject: {fileID: %d}\n  m_Children:\n%s  m_Father: {fileID: %d}\n"
                    "  m_AnchorMin: {x: %s, y: %s}\n  m_AnchorMax: {x: %s, y: %s}\n  m_AnchoredPosition: {x: %s, y: %s}\n"
                    "  m_SizeDelta: {x: %s, y: %s}\n  m_Pivot: {x: 0.5, y: 0.5}\n"
                    % (d["rt"], d["go"], hijos, padre, amin[0], amin[1], amax[0], amax[1], d["pos"][0], d["pos"][1], d["size"][0], d["size"][1]))
        if d["guid"]:
            docs.append("--- !u!114 &%d\nMonoBehaviour:\n  m_GameObject: {fileID: %d}\n  m_Enabled: 1\n"
                        "  m_EditorClassIdentifier: UnityEngine.UI::UnityEngine.UI.Image\n"
                        "  m_Sprite: {fileID: 21300000, guid: %s, type: 3}\n  m_Color: {r: 1, g: 1, b: 1, a: 1}\n  m_PreserveAspect: 0\n"
                        % (d["im"], d["go"], d["guid"]))
        for h in d["hijos"]:
            emite(h, d["rt"])

    numera(raiz)
    emite(raiz, 0)
    return "".join(docs)


def _prefab_sintetico(nuevo, segmentado=True):
    """
    Un personaje de la familia en miniatura (Lienzo > Cuerpo > Tronco con dos brazos, torso y una pierna), con la jerarquia ANTERIOR a
    INC-133 (nuevo=False: AntebrazoX bajo su codo) o la ACTUAL (nuevo=True: AnclaAntebrazoX bajo el codo y AntebrazoX al final de Tronco,
    con una pose que NO es la del ancla a proposito: en el prefab guardado coinciden, pero quien manda es el ancla, que es la que animan los
    clips y la que el motor copia). Devuelve el YAML.
    """
    def d(n, pos=(0, 0), size=(0, 0), estira=False, guid=None, hijos=()):
        return {"n": n, "pos": pos, "size": size, "estira": estira, "guid": guid, "hijos": list(hijos)}

    g = ["%032x" % (i + 1) for i in range(7)]
    codo_i = d("CodoIzq", (-30, 0), hijos=[d("AnclaAntebrazoIzq", (-40, 0), (80, 30)) if nuevo else d("AntebrazoIzq", (-40, 0), (80, 30), guid=g[1])])
    codo_d = d("CodoDer", (30, 0), hijos=[d("AnclaAntebrazoDer", (40, 0), (80, 30)) if nuevo else d("AntebrazoDer", (40, 0), (80, 30), guid=g[3])])
    tronco = [d("BrazoIzq", (-100, 0), (100, 40), guid=g[0], hijos=[codo_i]), d("BrazoDer", (100, 0), (100, 40), guid=g[2], hijos=[codo_d]),
              d("Torso", (0, 0), (80, 200), guid=g[4])]
    if nuevo:
        tronco += [d("AntebrazoIzq", (0, 0), (80, 30), guid=g[1]), d("AntebrazoDer", (0, 0), (80, 30), guid=g[3])]
    pierna = d("PiernaIzq", (-20, -200), (40, 100), guid=g[5], hijos=[
        d("RodillaIzq", (0, -40), hijos=[d("AntepiernaIzq", (0, -30), (40, 60), guid=g[6] if segmentado else None)])])
    cuerpo = d("Cuerpo", estira=True, hijos=[d("Tronco", estira=True, hijos=tronco), pierna])
    return _yaml_sintetico(d("Sintetico", estira=True, hijos=[d("Lienzo", estira=True, hijos=[cuerpo])]))


def autoprueba_arbol():
    """
    [(nombre, ok)]: leer_arbol con las DOS jerarquias del antebrazo (INC-133), sobre prefabs sinteticos y sobre los reales. Las dos tienen que
    dar el MISMO arbol de simulacion: el antebrazo bajo su codo, «sigue» y el orden de dibujo del prefab en Tronco.dibujo; sobre la anterior
    lo da aplicar_orden_tronco. Para pose_preview.py y coreografia.py, que no pueden dar por bueno un arbol que lee mal el prefab.
    """
    import tempfile
    casos = []
    orden = ["BrazoIzq", "BrazoDer", "Torso", "AntebrazoIzq", "AntebrazoDer"]
    ante_i, ante_d = LE + "/AntebrazoIzq", RE + "/AntebrazoDer"

    def firma(arbol):
        f = {}
        for r, n in arbol.items():
            f[r] = (n.padre.ruta if n.padre else None, n.sigue, tuple(round(v, 3) for v in n.rect) if n.rect else None,
                    tuple(round(v, 3) for v in n.pivote) if n.pivote else None, (n.imagen or {}).get("guid"),
                    tuple(h.ruta for h in n.hijos), tuple(h.ruta for h in n.dibujo) if n.dibujo is not None else None)
        return f

    with tempfile.TemporaryDirectory() as carpeta:
        arboles = {}
        for nombre, hay_ancla, seg in (("Viejo", False, True), ("ViejoSinSegmentar", False, False),
                                       ("Nuevo", True, True), ("NuevoSinSegmentar", True, False)):
            with open(os.path.join(carpeta, nombre + ".prefab"), "w", encoding="utf-8", newline="\n") as f:
                f.write(_prefab_sintetico(hay_ancla, seg))
            arboles[nombre] = leer_arbol(nombre, carpeta)
    viejo, nuevo = arboles["Viejo"], arboles["Nuevo"]
    ok_viejo = not viejo[ante_i].sigue and not viejo[ante_d].sigue and viejo[T].dibujo is None
    casos.append(("arbol anterior: el antebrazo sigue bajo su codo, sin marcar", ok_viejo and viejo[ante_i].padre is viejo[LE]))
    casos.append(("arbol nuevo: el antebrazo vuelve a su codo y sigue al ancla",
                  all(n in nuevo and nuevo[n].sigue and nuevo[n].padre is nuevo[c] and nuevo[n].dibuja()
                      for n, c in ((ante_i, LE), (ante_d, RE)))))
    casos.append(("arbol nuevo: sin ancla ni antebrazo bajo Tronco",
                  not any(r in nuevo for r in (LE + "/AnclaAntebrazoIzq", RE + "/AnclaAntebrazoDer", T + "/AntebrazoIzq", T + "/AntebrazoDer"))
                  and [h.nombre for h in nuevo[T].hijos] == orden[:3]))
    casos.append(("arbol nuevo: Tronco.dibujo es el orden del prefab", [h.nombre for h in hijos_de_dibujo(nuevo[T])] == orden))
    # la pose: la del ancla, no la que el antebrazo trae bajo Tronco (que el motor sobrescribe); el tamano, el del antebrazo
    casos.append(("arbol nuevo: la pose es la del ancla",
                  all(nuevo[n].pivote == viejo[n].pivote and nuevo[n].rect == viejo[n].rect for n in (ante_i, ante_d))))
    # «segmentado» son las piernas: no depende de la jerarquia del brazo
    casos.append(("segmentado se deduce igual con las dos jerarquias",
                  all(esta_segmentado(arboles[n]) == seg for n, seg in (("Viejo", True), ("ViejoSinSegmentar", False),
                                                                        ("Nuevo", True), ("NuevoSinSegmentar", False)))))
    aplicar_orden_tronco(viejo, orden)
    casos.append(("arbol anterior + orden_tronco = arbol nuevo", firma(viejo) == firma(nuevo)))
    aplicar_orden_tronco(nuevo, orden)
    casos.append(("orden_tronco sobre el arbol nuevo no cambia nada", firma(viejo) == firma(nuevo)))
    aplicar_orden_tronco(nuevo, ["Torso", "BrazoIzq", "BrazoDer"])
    casos.append(("otro orden sin antebrazos: quedan al final, no se pierden",
                  [h.nombre for h in hijos_de_dibujo(nuevo[T])] == ["Torso", "BrazoIzq", "BrazoDer", "AntebrazoIzq", "AntebrazoDer"]
                  and nuevo[ante_i].sigue and nuevo[ante_d].sigue))
    # los prefabs del disco, tengan la jerarquia que tengan (la familia de HEAD, la actual; Algoritm, la anterior): ningun ancla suelta ni
    # antebrazo colgando de Tronco, y «sigue» solo donde hay ancla
    ok = True
    for pid in PERSONAJES:
        with open(os.path.join(PREFABS, PERSONAJES[pid][0] + ".prefab"), encoding="utf-8") as f:
            hay_ancla = "AnclaAntebrazoIzq" in f.read()
        a = leer_arbol(PERSONAJES[pid][0])
        ok = ok and ante_i in a and ante_d in a and a[ante_i].sigue == hay_ancla and a[ante_d].sigue == hay_ancla \
            and not any(n.nombre.startswith("Ancla") for n in a.values()) and not any(r.startswith(T + "/Antebrazo") for r in a)
    casos.append(("prefabs del disco: antebrazo bajo su codo, sin anclas sueltas", ok))
    return casos


def casco_convexo(puntos):
    """Cierre convexo (Andrew, monotone chain). Para saber el punto mas bajo de una pieza sin rasterizar."""
    pts = sorted(set(puntos))
    if len(pts) <= 2:
        return pts

    def cruz(o, a, b):
        return (a[0] - o[0]) * (b[1] - o[1]) - (a[1] - o[1]) * (b[0] - o[0])

    bajo = []
    for p in pts:
        while len(bajo) >= 2 and cruz(bajo[-2], bajo[-1], p) <= 0:
            bajo.pop()
        bajo.append(p)
    alto = []
    for p in reversed(pts):
        while len(alto) >= 2 and cruz(alto[-2], alto[-1], p) <= 0:
            alto.pop()
        alto.append(p)
    return bajo[:-1] + alto[:-1]


def casco_imagen(im, rect, paso=2):
    """
    El cierre convexo de los pixeles opacos de una imagen PIL (RGBA), en coordenadas del lienzo (la imagen
    estirada a «rect»). Lo usan casco_sprite (un PNG del repo) y la maqueta de pose_preview (una pieza
    recortada en memoria).
    """
    x0, y0, x1, y1 = rect
    a = im.convert("RGBA").getchannel("A")
    w, h = a.size
    datos = a.load()
    pts = []
    for y in range(0, h, paso):
        fila = [x for x in range(0, w, paso) if datos[x, y] > 127]
        if fila:
            pts.append((fila[0], y))
            pts.append((fila[-1], y))
    if not pts:
        return [(x0, y0), (x1, y0), (x1, y1), (x0, y1)]
    return [(x0 + x * (x1 - x0) / w, y0 + y * (y1 - y0) / h) for x, y in casco_convexo(pts)]


def casco_sprite(ruta_png, rect, paso=2):
    """
    El cierre convexo de los pixeles opacos de un sprite, en coordenadas del lienzo (el sprite estirado a
    «rect»). Necesita Pillow; sin el, o sin el PNG (un puntero de LFS), las cuatro esquinas del rect.
    """
    x0, y0, x1, y1 = rect
    try:
        from PIL import Image
        im = Image.open(ruta_png).convert("RGBA")
    except Exception:
        return [(x0, y0), (x1, y0), (x1, y1), (x0, y1)]
    return casco_imagen(im, rect, paso)
