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
        # solo cambia el orden de dibujo. «sigue» lo marca; «dibujo» (solo en Tronco) es la lista de hijos en orden de dibujo, con ellos.
        self.sigue = False
        self.dibujo = None

    def dibuja(self):
        """Una Image encendida y con sprite es lo unico que uGUI dibuja."""
        i = self.imagen
        return bool(self.activo and i and i["encendida"] and i["guid"])


def leer_arbol(prefab):
    """Devuelve {ruta: Nodo} (la ruta no incluye el nombre de la raiz) con 'hijos' en orden de dibujo."""
    ruta = os.path.join(PREFABS, prefab + ".prefab")
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
    return nodos


# {nombre de sprite: ruta de un PNG}: lo que una herramienta quiere VER antes de escribirlo en el repo (el composite de
# preparar_expresion.py, sin --aplicar). Manda sobre lo que haya en disco.
PNG_EXTRA = {}


def _png_de(carpeta, nombre):
    """
    Ruta del PNG «nombre» en la carpeta de arte del personaje o en sus subcarpetas (Frontal/, Expresiones/, Perfil/: el arte
    se guarda por subcarpetas, y os.walk las recorre todas), o None. Si el PNG esta en PNG_EXTRA, el de ahi.
    """
    if nombre in PNG_EXTRA:
        return PNG_EXTRA[nombre]
    base = os.path.join(PERSONAJES_ARTE, carpeta)
    for dentro, _, archivos in sorted(os.walk(base)):
        if nombre + ".png" in archivos:
            return os.path.join(dentro, nombre + ".png")
    return None


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


def simula_sprites(arbol, rig_pj):
    """
    El estado del prefab DESPUES de correr «sprites» en el Editor (BuildRigsFinal.cs.txt): cada parte y cada
    nodo de imagen que el rig_articulaciones.json nombra y cuyo PNG existe en Assets/Game/Art/Characters/<carpeta>
    queda con su Image encendida y con ese sprite. Asi, entre copiar el arte final al repo y correr el generador
    (el prefab del disco aun es el provisional), coreografia.py y pose_preview.py ya tratan al personaje como
    lo que sera: brazos y piernas partidos, cabeza propia. Es idempotente: sobre un prefab que ya paso por
    «sprites» no cambia nada. Lo que no tiene PNG se deja como esta (apagado).
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
    return arbol


def arbol_vigente(pid, rig=None):
    """
    El arbol del prefab del personaje tal como quedara tras «sprites» (simula_sprites) y «orden» (aplicar_orden_tronco: el orden de
    dibujo de Tronco con los antebrazos delante, INC-133). Algoritm no pasa por «sprites»: su arte es un solo sprite. Es el que usan
    coreografia.py (leer_contexto: lo que tapa a cada brazo depende del orden), pose_preview.py y maqueta.py.
    """
    rig = rig or cargar_rig()
    arbol = leer_arbol(PERSONAJES[pid][0])
    pj = personaje_rig(rig, pid)
    if not PERSONAJES[pid][2]:
        simula_sprites(arbol, pj)
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
    """
    tronco = arbol[T]
    por_nombre = {h.nombre: h for h in tronco.hijos}
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
    tronco.dibujo = [por_nombre[n] if n in por_nombre else seguidores[n] for n in orden] + resto
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
    que se dibuja antes de Torso pasa inmediatamente despues de Torso; el que ya esta despues (Algoritm) no cambia. INC-133: los
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
