#!/usr/bin/env python3
# pose_preview.py: vista previa y PRUEBA de los clips de clips_personajes.json, sin Unity.
#
#     python3 claudeDocs/tasks/Personajes/herramientas/pose_preview.py                 # prueba + hojas + GIF, en los dos modos
#     python3 .../pose_preview.py --hoy                                                # solo el arte que hay (clips_personajes.json)
#     python3 .../pose_preview.py --maqueta                                            # solo el arte final simulado
#     python3 .../pose_preview.py --sin-hojas                                          # solo la prueba
#     python3 .../pose_preview.py --tira nino Idle 8 nino_idle.png                     # 8 instantes de un clip
#     python3 .../pose_preview.py --salida /ruta/de/trabajo                            # donde dejar los PNG/GIF
#     python3 .../pose_preview.py --orden Torso,Cuello,BrazoIzq,BrazoDer              # otro orden de dibujo de Tronco (el del JSON es el vigente)
#     python3 .../pose_preview.py --autoprueba                                         # la prueba SI detecta poses malas a proposito
#     python3 .../pose_preview.py --mide nino                                          # las articulaciones del rig caen en la rotula
#
# Necesita Pillow (pip install pillow). Sale con codigo 1 si algun clip falla la prueba.
#
# DOS MODOS (Papa, Mama y Nina mientras tengan arte provisional; quien ya tiene arte final —el Nino, y los otros
# tras preparar_arte_final.py --aplicar— se prueba una vez, y Algoritm es siempre maqueta):
#   --hoy      el arte REAL de los prefabs (brazos y piernas de una pieza, la cabeza dentro del torso) con
#              clips_personajes.json, que es lo que se vuelca al motor; comprueba antes que el JSON sea el que
#              coreografia.py calcularia ahora. Hojas: <id>_hoy_idle_8.png, <id>_hoy_todos.png, <id>_hoy_idle.gif.
#   --maqueta  el arte final SIMULADO (maqueta.py: el arte provisional recortado por las articulaciones del
#              rig, con antebrazo, antepierna y cabeza propios) con los clips que el solucionador calcula
#              para esa geometria. Es la prueba de que la coreografia, tal como esta escrita, pasa tambien
#              con el arte final. Hojas: <id>_maqueta_*.
#   Las mismas medidas en los dos: si pasan en «hoy» pero no en «maqueta», la coreografia depende del arte.
#
# QUE DIBUJA. Una cadena de transformaciones como la de uGUI: cada nodo gira y escala alrededor de SU
# pivote y se desplaza con m_AnchoredPosition, y los hijos van dentro (y = abajo en el lienzo; la
# rotacion + de Unity es antihoraria en pantalla). La geometria sale de rig_articulaciones.json (rect y
# pivote de las partes, punto de las articulaciones, rect de las imagenes) y, para lo que el JSON no
# tiene (Lienzo, Cuerpo —en el suelo—, Tronco, sprites, que Image esta encendida y con sprite), de los
# prefabs. El orden de dibujo es el orden de los hijos, con «orden_tronco» del JSON aplicado: simula el
# estado del prefab tras correr el generador. Una capa apagada o sin sprite NO se dibuja. Y POR ACCION: las que lista
# CharacterRig.armsInFrontActions (hoy Strike) se dibujan con los brazos DELANTE del torso, como hace ArmLayering en el
# motor (prefabs.orden_con_brazos_delante: el brazo que va antes del torso pasa justo despues de el; el que ya va despues,
# como los de Algoritm, no cambia). La lista se lee del .cs; si no esta, {Strike} con un aviso.
#
# ALGORITM. Sus tres formas aun tienen un solo sprite (cuerpo, brazos y piernas juntos), asi que sus
# brazos no se pueden esconder ni mover por separado. Para probar la coreografia contra la geometria que
# llevara el arte final, la vista previa hace una MAQUETA: recorta del sprite entero las piezas que
# define el JSON (torso, brazo, antebrazo, pierna, antepierna). Sumadas dan el sprite original. Es solo
# de vista previa: el prefab no se toca.
#
# LA PRUEBA (cada clip, muestreado a 30 fps):
#   (a) cada brazo —humero + antebrazo, o el brazo entero si no esta partido— conserva >= 85 % de sus
#       pixeles opacos visibles (no tapados por capas dibujadas despues que no sean del propio brazo);
#   (b) la cara conserva >= 90 % de su zona visible (ningun brazo la tapa); en Algoritm, cuyos brazos van POR ENCIMA de la
#       cara, el 99,5 % de la caja de ojos y boca ampliada un 30 %;
#   (c) ningun codo ni ninguna rodilla se dobla al reves (hiperextension): la pantorrilla vuelve hacia
#       dentro y el codo sobresale hacia fuera y abajo de la recta hombro-mano (nunca se mete hacia el
#       cuerpo o la cara); tolerancia de 6 grados sobre lo que ya trae el dibujo;
#   (d) el pie de apoyo no atraviesa el suelo mas de 2 px (Algoritm flota: no se mide).
#   (e) ninguna mano (la punta y la muñeca) dentro de la caja de la cara, ampliada un 10 %, salvo en las
#       excepciones explicitas de EXCEPCIONES_CARA (la visera de Observe; el rascado del Nino);
#   (h) solo Strike: el punto de choque (el instante de minima distancia entre las manos) queda por encima de la cintura: de la
#       cadera del JSON menos coreografia.MARGEN_CINTURA, y de la cadera del contexto si es mas alta (nunca en la entrepierna);
#   (f) ningun hombro ni codo gira mas de VELOCIDAD_MAX (1300 grados por segundo): atrapa el salto de rama de la
#       cinematica inversa a mitad de un gesto (salvo el golpe del martillo, EXCEPCIONES_VEL).

import argparse
import copy
import json
import math
import os
import sys
import tempfile

try:
    from PIL import Image, ImageChops, ImageDraw
except ImportError:  # pragma: no cover
    sys.exit("hace falta Pillow: pip install pillow")

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import coreografia as K  # noqa: E402
import maqueta as M  # noqa: E402
import prefabs as P  # noqa: E402

FPS = 30.0
C, T, LA, RA, LE, RE, LL, RL, LK, RK, NK, HD = P.C, P.T, P.LA, P.RA, P.LE, P.RE, P.LL, P.RL, P.LK, P.RK, P.NK, P.HD
CLIPS_JSON = os.path.join(AQUI, "clips_personajes.json")
SUELO = P.SUELO

UMBRAL_BRAZO = 0.85
UMBRAL_CARA = 0.90
# Algoritm: sus brazos se dibujan POR ENCIMA de la cara (orden_tronco [Torso, Ojos, Boca, BrazoIzq, BrazoDer], Santiago 05/10/2026),
# asi que lo que pase por ojos o boca los tapa de verdad. Para el la caja de ojos y boca se amplia un 30 % y no se admite casi
# nada de brazo encima (99,5 %): el gesto no debe ni rozar la cara.
UMBRAL_CARA_GUIA = 0.995
AMPLIA_CARA_GUIA = 1.30
TOLERANCIA_CODO = 6.0      # grados de hiperextension que se perdonan
TOLERANCIA_SUELO = 2.0     # px
AMPLIA_CAJA_CARA = 1.10    # la caja de la cara (ojos y boca) se amplia un 10 % para la prueba de las manos

# (e) Ninguna mano dentro de la caja de la cara, salvo en estos clips, con su motivo (una mano junto a la
# cara se lee como frustracion, miedo o un golpe: CP-02). La clave es (personaje o «*», accion).
EXCEPCIONES_CARA = {
    ("*", "Observe"): "visera: la mano va a la sien para mirar a lo lejos",
    ("nino", "Idle"): "se rasca el costado de la cabeza: con la cabeza mas ancha que el alcance del brazo, la mano solo llega al pelo junto a la oreja",
}
# (f) Ningun hombro ni codo gira mas de VELOCIDAD_MAX grados por segundo (medido cuadro a cuadro a 30 fps): una
# rama de la cinematica inversa que cambia a mitad de un gesto da saltos de 150 grados en un cuadro.
VELOCIDAD_MAX = 1300.0
EXCEPCIONES_VEL = {
    ("*", "Hammer"): "el golpe del martillo es rapido a proposito (baja el brazo en 0,1 s)",
}
# (a') Los brazos van tras el torso, asi que un gesto que los junta delante del pecho los esconde. Las acciones que lo necesitan
# NO son excepciones: el motor las dibuja con los brazos DELANTE del torso (CharacterRig.armsInFrontActions, hoy Strike) y la
# prueba usa para ellas ese orden (Personaje.orden_de). Esta tabla queda para lo que de verdad no pueda cumplir el 85 %: la
# acción, hasta que valor puede bajar y por que. Hoy esta vacia.
EXCEPCIONES_BRAZO = {}
UMBRAL_TORSO = 5.0         # (g) como mucho este % de la caja de ojos y boca tapado por el torso (la cabeza va al fondo)
ESCALA_PRUEBA = 0.25       # la prueba mide a 256 px (la mascara de cada capa)
REGION = (40, 30, 984, 990)  # lo que se ve en las hojas: casi todo el lienzo (Sleep se sale de la figura de pie)
REGION_GUIA = (0, -50, 1024, 1000)  # Algoritm flota: la llama sube por encima del lienzo

# --------------------------------------------------------------------------- algebra 3x3 (afin)


def mat_id():
    return (1.0, 0.0, 0.0, 0.0, 1.0, 0.0)


def mat_mul(a, b):
    """a . b (primero b, luego a). Matriz afin (m00, m01, m02, m10, m11, m12)."""
    return (a[0] * b[0] + a[1] * b[3], a[0] * b[1] + a[1] * b[4], a[0] * b[2] + a[1] * b[5] + a[2],
            a[3] * b[0] + a[4] * b[3], a[3] * b[1] + a[4] * b[4], a[3] * b[2] + a[4] * b[5] + a[5])


def mat_inv(m):
    det = m[0] * m[4] - m[1] * m[3]
    if abs(det) < 1e-12:
        return None
    i00, i01, i10, i11 = m[4] / det, -m[1] / det, -m[3] / det, m[0] / det
    return (i00, i01, -(i00 * m[2] + i01 * m[5]), i10, i11, -(i10 * m[2] + i11 * m[5]))


def mat_pt(m, p):
    return (m[0] * p[0] + m[1] * p[1] + m[2], m[3] * p[0] + m[4] * p[1] + m[5])


def mat_local(pivote, dx, dy_unity, grados, sx, sy):
    """
    Transformacion local de un nodo de uGUI en coordenadas del lienzo (y hacia abajo): escala y giro
    alrededor del pivote y desplazamiento m_AnchoredPosition (y de Unity va hacia ARRIBA). La rotacion
    + de Unity es antihoraria en pantalla, o sea (con y hacia abajo) x' = x c + y s, y' = -x s + y c.
    """
    c, s = math.cos(math.radians(grados)), math.sin(math.radians(grados))
    px, py = pivote
    a, b, d, e = c * sx, s * sy, -s * sx, c * sy
    return (a, b, px + dx - (a * px + b * py), d, e, py - dy_unity - (d * px + e * py))


# --------------------------------------------------------------------------- el personaje


def _abre(ruta):
    """Abre un sprite; si el .png es un puntero de LFS sin bajar (la sombra, ui_circulo), un circulo blanco."""
    try:
        return Image.open(ruta).convert("RGBA")
    except OSError:
        im = Image.new("RGBA", (256, 256), (255, 255, 255, 0))
        ImageDraw.Draw(im).ellipse([0, 0, 255, 255], fill=(255, 255, 255, 255))
        return im


class Capa:
    """Un nodo que se dibuja: su ruta, su sprite (ya cargado a escala 1) y su rect de reposo."""

    def __init__(self, nodo, rect, imagen, color, maqueta=False):
        self.nodo, self.rect, self.imagen, self.color, self.maqueta = nodo, rect, imagen, color, maqueta
        self.casco = None  # cierre convexo de los pixeles opacos, en coordenadas del lienzo de reposo
        self.escalas = {}  # el sprite reducido a cada tamano de pantalla (no se recalcula en cada cuadro)


class Personaje:
    """Todo lo que la vista previa necesita de un personaje; se arma una vez."""

    def __init__(self, pid, rig, orden=None, maqueta=False):
        """
        «maqueta»: el arte final SIMULADO (maqueta.py): brazos, piernas y cabeza recortados del arte
        provisional por las articulaciones del rig, para probar la coreografia contra su geometria. Sin ella
        («hoy»), el arte real del prefab. Algoritm siempre es maqueta (su sprite es uno solo).
        """
        self.pid = pid
        self.rig_todo = rig
        self.prefab, self.carpeta, self.guia = P.PERSONAJES[pid]
        self.rig = P.personaje_rig(rig, pid)
        self.maqueta = maqueta or self.guia
        # las acciones en que el motor pasa los brazos DELANTE del torso (CharacterRig.armsInFrontActions): en ellas se
        # dibuja con otro orden (orden_de)
        self.delante, self.delante_hallada = P.acciones_brazos_delante()
        if maqueta and not self.guia:
            self.arbol, self.piezas = M.arbol_maqueta(pid, rig)
        else:
            self.arbol = P.arbol_vigente(pid, rig)  # el prefab tal como quedara tras «sprites» (prefabs.simula_sprites)
            self.piezas = {}
            orden_json = self.rig.get("orden_tronco")
            if orden_json:
                P.aplicar_orden_tronco(self.arbol, orden_json)
        if orden:
            P.aplicar_orden_tronco(self.arbol, orden)  # otro orden de dibujo (--orden, o una prueba que lo necesite)
        self.segmentado = P.esta_segmentado(self.arbol)
        self._ctx = None
        self._geometria()
        self._dibujables()

    @property
    def ctx(self):
        """El contexto de la coreografia (geometria del brazo, pierna y cabeza) con ESTE arte."""
        if self._ctx is None:
            self._ctx = K.leer_contexto(self.pid, self.rig_todo, self.arbol, self.piezas)
        return self._ctx

    # ---- geometria: el JSON manda sobre el prefab
    def _geometria(self):
        geo = {}
        for parte in self.rig["partes"]:
            geo[parte["ruta"]] = {"pivote": tuple(parte["pivote"]), "rect": tuple(parte["rect"])}
        for nd in self.rig["nodos"]:
            ruta = nd["padre"] + "/" + nd["nombre"]
            punto = tuple(nd["punto"])
            if nd["tipo"] == "articulacion":
                geo[ruta] = {"pivote": punto, "rect": None}
                geo[ruta + "/" + nd["imagen"]] = {"pivote": punto, "rect": tuple(nd["rect"])}
            else:
                geo[ruta] = {"pivote": punto, "rect": tuple(nd["rect"]) if nd.get("rect") else None}
        self.pivote, self.rect = {}, {}
        for ruta, n in self.arbol.items():
            g = geo.get(ruta)
            self.pivote[ruta] = g["pivote"] if g else n.pivote
            self.rect[ruta] = g["rect"] if g and g["rect"] else n.rect

    # ---- que capas se dibujan, en orden
    def _dibujables(self):
        """
        Las capas en el orden del arbol (el de reposo: brazos detras del torso en la familia) y, aparte, en el orden de las
        acciones de «brazos delante» (prefabs.orden_con_brazos_delante: lo que hace ArmLayering en el motor). Las dos listas
        comparten las mismas Capa (y su cache de sprites reducidos).
        """
        self._cache = {}
        base = self._lienzo_baked() if self.guia else None
        hechas = {}

        def recorre():
            capas = []

            def visita(n):
                if n.rect is None and n.ruta != "":
                    return
                ruta = n.ruta
                if ruta:
                    if ruta not in hechas:
                        hechas[ruta] = self._capa(n, base)
                    if hechas[ruta] is not None:
                        capas.append(hechas[ruta])
                for h in n.hijos:
                    visita(h)

            visita(self.arbol[""])
            return capas

        self.capas = recorre()
        self.indice = {c.nodo.ruta: i for i, c in enumerate(self.capas)}
        tronco = self.arbol[T]
        orig = tronco.hijos
        nombres = [h.nombre for h in orig]
        nuevo = P.orden_con_brazos_delante(nombres)
        if nuevo == nombres:
            self.capas_delante, self.indice_delante = self.capas, self.indice
        else:
            por = {h.nombre: h for h in orig}
            tronco.hijos = [por[n] for n in nuevo]
            self.capas_delante = recorre()
            tronco.hijos = orig
            self.indice_delante = {c.nodo.ruta: i for i, c in enumerate(self.capas_delante)}

    def orden_de(self, accion):
        """(capas, indice) en el orden de dibujo de esa accion: con los brazos delante del torso si CharacterRig la lista."""
        if accion in self.delante:
            return self.capas_delante, self.indice_delante
        return self.capas, self.indice

    def _lienzo_baked(self):
        """Algoritm: el sprite entero a 1024 (el Image de Cuerpo conserva el aspecto)."""
        n = self.arbol[C]
        ruta = P.sprite_por_guid(n.imagen["guid"])
        im = Image.open(ruta).convert("RGBA")
        lado = int(round(min(1024, 1024) / max(im.size) * max(im.size)))
        return im.resize((1024, 1024), Image.LANCZOS) if im.size != (lado, lado) else im

    def _capa(self, n, baked):
        ruta = n.ruta
        rect = self.rect[ruta]
        if self.guia:
            return self._capa_guia(n, rect, baked)
        if not n.dibuja() or rect is None:
            return None
        if n.imagen["guid"].startswith("maqueta:"):
            pieza = self.piezas[ruta]
            return Capa(n, pieza.rect, pieza.imagen, n.imagen["color"], maqueta=True)
        arch = P.sprite_por_guid(n.imagen["guid"])
        if arch is None:
            return None
        return Capa(n, rect, _abre(arch), n.imagen["color"])

    # ---- maqueta de Algoritm
    def _capa_guia(self, n, rect, baked):
        nombre = n.nombre
        if nombre == "Cuerpo":
            return None  # su Image se apaga cuando llegan las partes (la maqueta las sustituye)
        if nombre not in ("Torso", "BrazoIzq", "BrazoDer", "AntebrazoIzq", "AntebrazoDer", "PiernaIzq", "PiernaDer",
                          "AntepiernaIzq", "AntepiernaDer"):
            return None
        x0, y0, x1, y1 = [int(round(v)) for v in rect]
        pieza = baked.crop((x0, y0, x1, y1)).copy()
        # Lo que pertenece a otra pieza se borra: torso = lienzo menos extremidades; brazo = brazo menos antebrazo.
        borrar = []
        if nombre == "Torso":
            for r in ("BrazoIzq", "BrazoDer", "PiernaIzq", "PiernaDer"):
                borrar.append(self.rect[T + "/" + r] if r.startswith("Brazo") else self.rect[C + "/" + r])
        elif nombre == "BrazoIzq":
            borrar.append(self.rect[LE + "/AntebrazoIzq"])
        elif nombre == "BrazoDer":
            borrar.append(self.rect[RE + "/AntebrazoDer"])
        elif nombre == "PiernaIzq":
            borrar.append(self.rect[LK + "/AntepiernaIzq"])
        elif nombre == "PiernaDer":
            borrar.append(self.rect[RK + "/AntepiernaDer"])
        if borrar:
            alfa = pieza.getchannel("A")
            d = ImageDraw.Draw(alfa)
            for r in borrar:
                d.rectangle([r[0] - x0, r[1] - y0, r[2] - x0 - 1, r[3] - y0 - 1], fill=0)
            pieza.putalpha(alfa)
        return Capa(n, (x0, y0, x1, y1), pieza, (1, 1, 1, 1), maqueta=True)

    # ---- cierre convexo (perezoso) para el suelo
    def casco(self, capa):
        if capa.casco is None:
            a = capa.imagen.getchannel("A")
            w, h = a.size
            rw, rh = capa.rect[2] - capa.rect[0], capa.rect[3] - capa.rect[1]
            datos = a.load()
            pts = []
            for y in range(0, h, 2):
                fila = [x for x in range(0, w, 2) if datos[x, y] > 127]
                if fila:
                    pts.append((fila[0], y))
                    pts.append((fila[-1], y))
            capa.casco = [(capa.rect[0] + x * rw / w, capa.rect[1] + y * rh / h) for x, y in P.casco_convexo(pts)]
        return capa.casco

    def punta(self, ruta, parte_alta=None, desde_y=None):
        """
        Donde acaba un segmento (la mano o el pie), en el lienzo de reposo: el centroide de los pixeles
        opacos de su sprite (con «parte_alta» solo se cuenta esa fraccion de arriba y con «desde_y» solo lo
        que queda por debajo de esa altura del lienzo: la canilla, sin el pie que sobresale ni la punta que
        se mete en el muslo). Sin sprite (arte provisional), el centro del rect (la base si es pierna).
        """
        capa = next((c for c in self.capas if c.nodo.ruta == ruta), None)
        rect = self.rect.get(ruta)
        if capa is None:
            if rect is None:
                return None
            return ((rect[0] + rect[2]) / 2, rect[3] if parte_alta else (rect[1] + rect[3]) / 2)
        a = capa.imagen.getchannel("A")
        w, h = a.size
        datos = a.load()
        sx = sy = n = 0
        rh0 = capa.rect[3] - capa.rect[1]
        y_ini = 0 if desde_y is None else max(0, int((desde_y - capa.rect[1]) / rh0 * h))
        for y in range(y_ini, int(h * (parte_alta or 1.0)), 2):
            for x in range(0, w, 2):
                if datos[x, y] > 127:
                    sx, sy, n = sx + x, sy + y, n + 1
        if not n:
            return ((rect[0] + rect[2]) / 2, (rect[1] + rect[3]) / 2)
        rw, rh = capa.rect[2] - capa.rect[0], capa.rect[3] - capa.rect[1]
        return (capa.rect[0] + sx / n * rw / w, capa.rect[1] + sy / n * rh / h)

    def manos_modelo(self):
        """
        Puntos de cada mano en reposo: {lado: (ruta del nodo que los mueve, [puntos])}. Salen del modelo de
        brazo de coreografia.py (la mano al 80 % del antebrazo; con un solo sprite, cerca de la esquina lejana)
        y de un punto mas atras, la muñeca.
        """
        if getattr(self, "_manos", None) is None:
            x = self.ctx
            self._manos = {}
            for lado, nodo_p, nodo_c in (("Izq", LA, LE), ("Der", RA, RE)):
                b = x.brazos[lado]
                nodo, base = (nodo_c, b.E) if b.partido else (nodo_p, b.S)
                pts = [b.M, (base[0] + 0.75 * (b.M[0] - base[0]), base[1] + 0.75 * (b.M[1] - base[1]))]
                self._manos[lado] = (nodo, pts)
        return self._manos

    # ---- zona de la cara y puntos de articulacion
    def zona_cara(self):
        """Rects (lienzo, reposo) de ojos y boca y el nodo que los lleva (la cabeza si se dibuja, si no el torso)."""
        rects = []
        for nombre in ("Ojos", "Boca"):
            for ruta, n in self.arbol.items():
                if n.nombre == nombre and self.rect.get(ruta):
                    rects.append(self.rect[ruta])
        cabeza = self.arbol.get(HD)
        lleva = HD if cabeza is not None and (cabeza.dibuja()) else T + "/Torso"
        return rects, lleva


# --------------------------------------------------------------------------- poses


class Clip:
    def __init__(self, d):
        self.archivo, self.accion, self.duracion, self.bucle = d["archivo"], d["accion"], d["duracion"], d["bucle"]
        self.curvas = {}
        for c in d["curvas"]:
            self.curvas[(c["ruta"], c["propiedad"])] = K.CurvaAuto(c["claves"])

    def valor(self, ruta, prop, t, defecto):
        c = self.curvas.get((ruta, prop))
        return defecto if c is None else c.evaluar(t)


def cargar_clips(ruta=CLIPS_JSON):
    with open(ruta, encoding="utf-8") as f:
        doc = json.load(f)
    return {p["id"]: [Clip(c) for c in p["clips"]] for p in doc["personajes"]}


def clips_de(clips, pid):
    return clips["algoritm" if pid.startswith("algoritm") else pid]


def matrices(pj, clip, t):
    """Matriz mundo de cada nodo del arbol (y el alfa de Lienzo) en el instante t del clip."""
    mundo = {}
    alfa = clip.valor("Lienzo", K.ALFA, t, 1.0)

    def visita(n, padre):
        ruta = n.ruta
        if ruta == "":
            mundo[ruta] = mat_id()
        else:
            piv = pj.pivote[ruta]
            if n.ruta == "Lienzo" or piv is None:  # Lienzo no se mueve; Estela (fuera de Lienzo) no tiene geometria
                loc = mat_id()
            else:
                loc = mat_local(piv, clip.valor(ruta, K.POSX, t, 0.0), clip.valor(ruta, K.POSY, t, 0.0),
                                clip.valor(ruta, K.ROT, t, 0.0), clip.valor(ruta, K.ESCX, t, 1.0),
                                clip.valor(ruta, K.ESCY, t, 1.0))
            mundo[ruta] = mat_mul(mundo[padre], loc)
        for h in n.hijos:
            visita(h, ruta)

    visita(pj.arbol[""], "")
    return mundo, alfa


# --------------------------------------------------------------------------- dibujo


def _afin_capa(capa, mundo, escala, origen):
    """Coeficientes de PIL (salida -> sprite) para esta capa, o None si la matriz es singular."""
    w, h = capa.imagen.size
    x0, y0, x1, y1 = capa.rect
    # sprite (u, v) -> lienzo de reposo
    a_sprite = ((x1 - x0) / w, 0.0, x0, 0.0, (y1 - y0) / h, y0)
    a_sal = (escala, 0.0, -origen[0] * escala, 0.0, escala, -origen[1] * escala)
    total = mat_mul(a_sal, mat_mul(mundo[capa.nodo.ruta], a_sprite))
    return mat_inv(total)


def _sprite_a_escala(capa, escala):
    """El sprite reducido a lo que ocupa en pantalla (el afin solo corrige el giro y el resto)."""
    w, h = capa.imagen.size
    rw, rh = capa.rect[2] - capa.rect[0], capa.rect[3] - capa.rect[1]
    tw, th = max(1, int(round(rw * escala))), max(1, int(round(rh * escala)))
    im = capa.escalas.get((tw, th))
    if im is None:
        im = capa.imagen.resize((tw, th), Image.LANCZOS) if (tw, th) != (w, h) else capa.imagen
        capa.escalas[(tw, th)] = im
    return im


def _capa_a_lienzo(capa, mundo, escala, origen, tam, remuestreo=Image.BILINEAR):
    """La capa transformada sobre un RGBA transparente de tam x tam."""
    im = _sprite_a_escala(capa, escala)
    w, h = im.size
    x0, y0, x1, y1 = capa.rect
    a_sprite = ((x1 - x0) / w, 0.0, x0, 0.0, (y1 - y0) / h, y0)
    a_sal = (escala, 0.0, -origen[0] * escala, 0.0, escala, -origen[1] * escala)
    total = mat_mul(a_sal, mat_mul(mundo[capa.nodo.ruta], a_sprite))
    inv = mat_inv(total)
    if inv is None:
        return None
    return im.transform(tam, Image.AFFINE, inv, remuestreo)


def render(pj, clip, t, escala=0.5, region=None, fondo=(236, 232, 222, 255), alfa_grupo=True):
    """El personaje en el instante t como imagen RGBA (region del lienzo recortada, escala dada)."""
    mundo, alfa = matrices(pj, clip, t)
    region = region or (REGION_GUIA if pj.guia else REGION)
    ox, oy = region[0], region[1]
    tam = (int(round((region[2] - region[0]) * escala)), int(round((region[3] - region[1]) * escala)))
    lienzo = Image.new("RGBA", tam, fondo)
    for capa in pj.orden_de(clip.accion)[0]:
        sal = _capa_a_lienzo(capa, mundo, escala, (ox, oy), tam)
        if sal is None:
            continue
        a = sal.getchannel("A")
        k = capa.color[3] * (alfa if alfa_grupo else 1.0)
        if k < 0.999:
            a = a.point(lambda v, k=k: int(v * k))
        if capa.color[:3] != (1, 1, 1):
            r, g, b = sal.getchannel("R"), sal.getchannel("G"), sal.getchannel("B")
            r, g, b = [c.point(lambda v, k=k2: int(v * k)) for c, k2 in zip((r, g, b), capa.color[:3])]
            sal = Image.merge("RGBA", (r, g, b, a))
        else:
            sal.putalpha(a)
        lienzo.alpha_composite(sal)
    return lienzo


# --------------------------------------------------------------------------- la prueba


class Resultado:
    """El peor valor de cada medida en un clip, con el instante en que ocurre."""

    def __init__(self):
        self.brazo = (100.0, 0.0, "")
        self.cara = (100.0, 0.0)
        self.codo = (0.0, 0.0, "")
        self.suelo = (-999.0, 0.0)
        self.mano = (0, 0.0, "")  # cuadros con una mano en la caja de la cara, primer instante, lado
        self.vel = (0.0, 0.0, "")  # la mayor velocidad angular de un hombro o un codo, instante, hueso
        self.torso = (0.0, 0.0)    # el mayor % de la caja de ojos y boca que tapa el torso (con la cabeza al fondo), instante
        self.choque = None         # solo Strike: (y del choque, instante, distancia minima entre manos, limite de altura) del instante de minima distancia

    def mejor_peor(self, nombre, valor, t, extra=""):
        if nombre == "brazo" and valor < self.brazo[0]:
            self.brazo = (valor, t, extra)
        elif nombre == "cara" and valor < self.cara[0]:
            self.cara = (valor, t)
        elif nombre == "codo" and valor > self.codo[0]:
            self.codo = (valor, t, extra)
        elif nombre == "suelo" and valor > self.suelo[0]:
            self.suelo = (valor, t)
        elif nombre == "vel" and valor > self.vel[0]:
            self.vel = (valor, t, extra)
        elif nombre == "torso" and valor > self.torso[0]:
            self.torso = (valor, t)


def _mascara(im):
    return im.getchannel("A").point(lambda v: 255 if v > 127 else 0)


def _cuenta(m):
    h = m.histogram()
    return sum(h[1:])


def _excepcion_cara(pid, accion):
    return (pid, accion) in EXCEPCIONES_CARA or ("*", accion) in EXCEPCIONES_CARA


def _dentro(p, poligono):
    """Punto dentro de un cuadrilatero convexo (los cuatro vertices en orden)."""
    signos = []
    for i in range(4):
        a, b = poligono[i], poligono[(i + 1) % 4]
        signos.append((b[0] - a[0]) * (p[1] - a[1]) - (b[1] - a[1]) * (p[0] - a[0]))
    return all(v >= 0 for v in signos) or all(v <= 0 for v in signos)


def _poligono(rect, mundo_nodo, escala):
    x0, y0, x1, y1 = rect
    return [tuple(c * escala for c in mat_pt(mundo_nodo, p)) for p in ((x0, y0), (x1, y0), (x1, y1), (x0, y1))]


def _angulo(u):
    return math.degrees(math.atan2(-u[1], u[0]))  # y hacia arriba


def _giro(v, grados):
    c, s = math.cos(math.radians(grados)), math.sin(math.radians(grados))
    # rotacion antihoraria en pantalla (y hacia abajo): x' = x c + y s ; y' = -x s + y c
    return (v[0] * c + v[1] * s, -v[0] * s + v[1] * c)


def codo_nu(lado, hombro, codo, mano):
    """
    Cuanto sobresale el codo hacia FUERA y hacia ABAJO, en grados. La esquina del codo (el vector del punto
    medio de la recta hombro-mano al codo) apunta hacia donde apuntan los codos que se doblan bien: hacia el
    costado del brazo y hacia abajo (manos a la cintura, manos a las mejillas, la mano a la oreja, los brazos
    en V). Si apunta hacia el cuerpo y hacia arriba, el codo se dobla al reves (hiperextension). El valor es
    el angulo que forma el humero con esa recta, multiplicado por el coseno entre la esquina y esa
    direccion: continuo, + natural, - al reves, 0 en brazo recto.
    """
    d = (mano[0] - hombro[0], mano[1] - hombro[1])
    e = (codo[0] - hombro[0], codo[1] - hombro[1])
    l, l1 = math.hypot(*d), math.hypot(*e)
    if l < 1e-6 or l1 < 1e-6:
        return 0.0
    ang = math.degrees(math.acos(max(-1.0, min(1.0, (d[0] * e[0] + d[1] * e[1]) / (l * l1)))))
    c = (e[0] - d[0] / 2.0, e[1] - d[1] / 2.0)
    w = (-1.0 if lado == "Izq" else 1.0, 0.6)
    nc = math.hypot(*c)
    if nc < 1e-6:
        return 0.0
    return ang * (c[0] * w[0] + c[1] * w[1]) / (nc * math.hypot(*w))


def bisagras(pj, mundo, clip=None, t=0.0):
    """
    El pliegue firmado de cada codo y rodilla en una pose: {nombre: nu}. El codo se mide sobre la geometria
    (la mano respecto al humero). La rodilla, sobre su rotacion: medir el pie respecto al muslo da un angulo
    que cambia solo con acortar la antepierna (el pie queda ladeado respecto a la caña), y no es doblar.
    """
    out = {}
    piv = pj.pivote
    if LA in piv and RA in piv:
        for lado, brazo, codo, ante in (("Izq", LA, LE, LE + "/AntebrazoIzq"), ("Der", RA, RE, RE + "/AntebrazoDer")):
            pt = pj.punta(ante)
            if pt is None or ante not in pj.indice:  # sin antebrazo dibujado (arte provisional) no hay codo que se vea
                continue
            # en el marco del tronco: si el cuerpo entero gira (Spin) o se inclina, «abajo» sigue siendo hacia los pies
            inv = mat_inv(mundo[T])
            en_tronco = lambda p: mat_pt(inv, p)
            out["codo" + lado] = codo_nu(lado, en_tronco(mat_pt(mundo[brazo], piv[brazo])), en_tronco(mat_pt(mundo[brazo], piv[codo])),
                                         en_tronco(mat_pt(mundo[codo], pt)))
    if LL in piv and RL in piv:
        for lado, pierna, rod, ante in (("Izq", LL, LK, LK + "/AntepiernaIzq"), ("Der", RL, RK, RK + "/AntepiernaDer")):
            if ante not in pj.indice:  # sin antepierna dibujada no hay rodilla que se vea
                continue
            giro = clip.valor(rod, K.ROT, t, 0.0) if clip is not None else 0.0
            out["rodilla" + lado] = giro if lado == "Izq" else -giro
    return out


def prueba_clip(pj, clip, tiempos=None):
    res = Resultado()
    esc = ESCALA_PRUEBA
    tam = (int(1024 * esc), int(1024 * esc))
    zona, lleva = pj.zona_cara()
    capas, indice = pj.orden_de(clip.accion)  # el orden de dibujo de ESTA accion (brazos delante o detras del torso)
    brazos = {}
    for lado, (b, a) in {"Izq": (LA, LE + "/AntebrazoIzq"), "Der": (RA, RE + "/AntebrazoDer")}.items():
        grupo = [r for r in (b, a) if r in indice]
        if grupo:
            brazos[lado] = grupo
    n = int(round(clip.duracion * FPS))
    tiempos = tiempos if tiempos is not None else [min(i / FPS, clip.duracion) for i in range(n + 1)]
    es_guia = pj.guia
    # (h) Strike: el punto de choque (el instante de minima distancia entre las manos) por encima de la cintura
    es_strike = clip.accion == "Strike" and not es_guia
    choque = (1e18, 0.0, 0.0)  # distancia, instante, altura media de las manos
    vacio = Clip({"archivo": "", "accion": "", "duracion": 1.0, "bucle": True, "curvas": []})
    reposo_nu = bisagras(pj, matrices(pj, vacio, 0.0)[0], vacio, 0.0)
    previo = None
    for t in tiempos:
        # (f) velocidad angular de hombros y codos entre cuadros
        vals = {r: clip.valor(r, K.ROT, t, 0.0) for r in (LA, RA, LE, RE)}
        if previo is not None and t > previo[0]:
            for r in vals:
                v = abs(vals[r] - previo[1][r]) / (t - previo[0])
                res.mejor_peor("vel", v, t, r.split("/")[-1])
        previo = (t, vals)
        mundo, _ = matrices(pj, clip, t)
        masc = {}
        for capa in capas:
            ruta = capa.nodo.ruta
            if ruta == "Lienzo/Sombra":
                continue
            im = _capa_a_lienzo(capa, mundo, esc, (0, 0), tam)
            if im is not None:
                masc[ruta] = _mascara(im)
        # (a) brazos visibles
        union_brazos = Image.new("L", tam, 0)
        for lado, grupo in brazos.items():
            total = oculto = 0
            for ruta in grupo:
                if ruta not in masc:
                    continue
                m = masc[ruta]
                union_brazos = ImageChops.lighter(union_brazos, m)
                total += _cuenta(m)
                oc = Image.new("L", tam, 0)
                for r2, m2 in masc.items():
                    if indice[r2] > indice[ruta] and r2 not in grupo:
                        oc = ImageChops.lighter(oc, m2)
                oculto += _cuenta(ImageChops.multiply(m, oc.point(lambda v: 255 if v else 0)))
            if total:
                res.mejor_peor("brazo", 100.0 * (total - oculto) / total, t, lado)
        # (b) cara: ningun brazo la tapa (Algoritm: la caja entera de ojos y boca, ampliada, y casi nada de brazo encima)
        if zona:
            z = Image.new("L", tam, 0)
            dz = ImageDraw.Draw(z)
            if es_guia:
                x0 = min(r[0] for r in zona); y0 = min(r[1] for r in zona)
                x1 = max(r[2] for r in zona); y1 = max(r[3] for r in zona)
                cx_, cy_ = (x0 + x1) / 2, (y0 + y1) / 2
                mw, mh = (x1 - x0) * AMPLIA_CARA_GUIA / 2, (y1 - y0) * AMPLIA_CARA_GUIA / 2
                dz.polygon(_poligono((cx_ - mw, cy_ - mh, cx_ + mw, cy_ + mh), mundo[lleva], esc), fill=255)
            else:
                for r in zona:
                    dz.polygon(_poligono(r, mundo[lleva], esc), fill=255)
            area = _cuenta(z)
            if area:
                tapado = _cuenta(ImageChops.multiply(z, union_brazos))
                res.mejor_peor("cara", 100.0 * (area - tapado) / area, t)
                # (g) con la cabeza al fondo el torso (dibujado despues) puede tapar la barbilla y la boca al inclinarse
                if lleva == HD and (T + "/Torso") in masc:
                    res.mejor_peor("torso", 100.0 * _cuenta(ImageChops.multiply(z, masc[T + "/Torso"])) / area, t)
        # (e) manos fuera de la caja de la cara (ampliada un 10 %), salvo las excepciones explicitas
        if zona and not _excepcion_cara(pj.pid, clip.accion):
            x0 = min(r[0] for r in zona); y0 = min(r[1] for r in zona)
            x1 = max(r[2] for r in zona); y1 = max(r[3] for r in zona)
            cx_, cy_, mw, mh = (x0 + x1) / 2, (y0 + y1) / 2, (x1 - x0) * AMPLIA_CAJA_CARA / 2, (y1 - y0) * AMPLIA_CAJA_CARA / 2
            caja = _poligono((cx_ - mw, cy_ - mh, cx_ + mw, cy_ + mh), mundo[lleva], 1.0)
            for lado, (nodo, pts) in pj.manos_modelo().items():
                if any(_dentro(mat_pt(mundo[nodo], p), caja) for p in pts):
                    n_, t_, l_ = res.mano
                    res.mano = (n_ + 1, t_ if n_ else t, l_ or lado)
        # (h) la altura de las manos en el instante en que mas se acercan
        if es_strike:
            ptos = [mat_pt(mundo[nodo], pts[0]) for nodo, pts in pj.manos_modelo().values()]
            d_ = math.hypot(ptos[0][0] - ptos[1][0], ptos[0][1] - ptos[1][1])
            if d_ < choque[0]:
                choque = (d_, t, (ptos[0][1] + ptos[1][1]) / 2.0)
        # (c) codos y rodillas: el pliegue no puede ser mas «al reves» que el del propio dibujo (el arte
        # puede traer un pie o una mano ladeados) por mas de la tolerancia
        peor, quien = 0.0, ""
        for nombre, nu in bisagras(pj, mundo, clip, t).items():
            malo = max(0.0, -nu - max(0.0, -reposo_nu[nombre])) - TOLERANCIA_CODO
            if malo > peor:
                peor, quien = malo, nombre
        res.mejor_peor("codo", peor, t, quien)
        # (d) suelo (Algoritm flota: no se mide)
        if not es_guia:
            bajo = -1e9
            for capa in pj.capas:
                nombre = capa.nodo.nombre
                if nombre.startswith("Pierna") or nombre.startswith("Antepierna"):
                    m = mundo[capa.nodo.ruta]
                    for p in pj.casco(capa):
                        bajo = max(bajo, mat_pt(m, p)[1])
            res.mejor_peor("suelo", bajo - SUELO, t)
    if es_strike:
        res.choque = (choque[2], choque[1], choque[0], limite_cintura(pj))
    return res


def limite_cintura(pj):
    """
    La altura (y del lienzo) por debajo de la cual un choque queda en la cintura o en la entrepierna: la cadera del JSON menos
    coreografia.MARGEN_CINTURA, o la cadera del contexto si es mas alta. Es la misma que usa el solucionador (Ctx.y_cintura).
    """
    return pj.ctx.y_cintura


def fila(pid, clip, r):
    ok_b = r.brazo[0] >= (EXCEPCIONES_BRAZO.get(("*", clip.accion), (UMBRAL_BRAZO * 100,))[0])
    ok_c = r.cara[0] >= (UMBRAL_CARA_GUIA if pid.startswith("algoritm") else UMBRAL_CARA) * 100
    ok_k = r.codo[0] <= 1e-9
    ok_s = r.suelo[0] <= TOLERANCIA_SUELO
    ok_m = r.mano[0] == 0
    ok_v = r.vel[0] <= VELOCIDAD_MAX or ("*", clip.accion) in EXCEPCIONES_VEL
    ok_t = r.torso[0] <= UMBRAL_TORSO
    ok_h = r.choque is None or r.choque[0] <= r.choque[3]
    ok = ok_b and ok_c and ok_k and ok_s and ok_m and ok_v and ok_t and ok_h
    choque = "" if r.choque is None else " | choque y=%.0f @%.2fs (manos a %.0f px; limite y=%.0f) %s" % (
        r.choque[0], r.choque[1], r.choque[2], r.choque[3], "" if ok_h else "FALLA: bajo la cintura")
    mano = "libre" if ok_m else "%d cuadros @%.2fs %s" % (r.mano[0], r.mano[1], r.mano[2])
    return ok, ("%-14s %-10s %-8s brazo %5.1f%% @%.2fs %-5s | cara %5.1f%% @%.2fs | bisagra %4.1f deg @%.2fs %-9s | suelo %+6.1f px @%.2fs | mano en la caja: %s | giro max %4.0f deg/s %s | torso sobre la cara %4.1f%% %s%s" % (
        pid, clip.accion, "ok" if ok else "FALLA", r.brazo[0], r.brazo[1], r.brazo[2], r.cara[0], r.cara[1],
        r.codo[0], r.codo[1], r.codo[2], r.suelo[0] if r.suelo[0] > -900 else 0.0, r.suelo[1], mano, r.vel[0], "" if ok_v else "FALLA", r.torso[0], "" if ok_t else "FALLA", choque))


# --------------------------------------------------------------------------- hojas y GIF


def _rotulo(im, texto, color=(40, 40, 40, 255)):
    d = ImageDraw.Draw(im)
    d.rectangle([0, 0, im.width, 14], fill=(255, 255, 255, 220))
    d.text((3, 1), texto, fill=color)


def tira(pj, clip, n, escala=0.5, tiempos=None):
    """n instantes de un clip en una fila."""
    ts = tiempos if tiempos is not None else [clip.duracion * i / n for i in range(n)]
    celdas = [render(pj, clip, t, escala) for t in ts]
    w, h = celdas[0].size
    hoja = Image.new("RGBA", (w * len(celdas), h), (236, 232, 222, 255))
    for i, c in enumerate(celdas):
        hoja.paste(c, (i * w, 0))
        d = ImageDraw.Draw(hoja)
        d.text((i * w + 4, 2), "%s t=%.2fs" % (clip.accion, ts[i]), fill=(60, 60, 60, 255))
    return hoja


def hoja_idle(pj, clips, ruta, escala=0.5):
    clip = next(c for c in clips if c.accion == "Idle")
    hoja = tira(pj, clip, 8, escala)
    # 8 instantes en 2 filas de 4
    w = hoja.width // 8
    h = hoja.height
    dos = Image.new("RGBA", (w * 4, h * 2), (236, 232, 222, 255))
    for i in range(8):
        dos.paste(hoja.crop((i * w, 0, (i + 1) * w, h)), ((i % 4) * w, (i // 4) * h))
    dos.convert("RGB").save(ruta)


def hoja_todos(pj, clips, ruta, escala=0.2, columnas_clip=3):
    fr = (0.15, 0.5, 0.85)
    celdas = []
    for c in clips:
        fila_ = [render(pj, c, c.duracion * f, escala) for f in fr]
        celdas.append((c, fila_))
    w, h = celdas[0][1][0].size
    filas = (len(celdas) + columnas_clip - 1) // columnas_clip
    hoja = Image.new("RGBA", (w * 3 * columnas_clip, (h + 16) * filas), (236, 232, 222, 255))
    d = ImageDraw.Draw(hoja)
    for i, (c, fl) in enumerate(celdas):
        gx = (i % columnas_clip) * w * 3
        gy = (i // columnas_clip) * (h + 16)
        d.rectangle([gx, gy, gx + w * 3, gy + 14], fill=(255, 255, 255, 255))
        d.text((gx + 4, gy + 1), "%s (%.2fs)" % (c.accion, c.duracion), fill=(20, 20, 20, 255))
        for j, im in enumerate(fl):
            hoja.paste(im, (gx + j * w, gy + 16))
        d.line([gx, gy, gx, gy + h + 16], fill=(150, 150, 150, 255))
    hoja.convert("RGB").save(ruta)


def gif_idle(pj, clips, ruta, escala=0.4, fps=20):
    clip = next(c for c in clips if c.accion == "Idle")
    n = max(2, int(round(clip.duracion * fps)))
    cuadros = [render(pj, clip, clip.duracion * i / n, escala).convert("RGB") for i in range(n)]
    cuadros[0].save(ruta, save_all=True, append_images=cuadros[1:], duration=int(round(1000 / fps)), loop=0, optimize=False)


# --------------------------------------------------------------------------- medir y autoprueba


def extremos_capsula(imagen):
    """
    Los centros de los dos extremos redondos de una pieza alargada (un brazo, una pierna: capsulas con el
    contorno cerrado) y su radio, en pixeles del sprite. Eje principal por momentos de segunda orden.
    """
    a = imagen.getchannel("A")
    w, h = a.size
    datos = a.load()
    pts = [(x, y) for y in range(h) for x in range(w) if datos[x, y] > 127]
    n = len(pts)
    cx, cy = sum(p[0] for p in pts) / n, sum(p[1] for p in pts) / n
    sxx = sum((p[0] - cx) ** 2 for p in pts) / n
    syy = sum((p[1] - cy) ** 2 for p in pts) / n
    sxy = sum((p[0] - cx) * (p[1] - cy) for p in pts) / n
    ang = 0.5 * math.atan2(2 * sxy, sxx - syy)
    ux, uy = math.cos(ang), math.sin(ang)
    proy = [((p[0] - cx) * ux + (p[1] - cy) * uy, -(p[0] - cx) * uy + (p[1] - cy) * ux) for p in pts]
    lo, hi = min(q[0] for q in proy), max(q[0] for q in proy)
    medio = [q[1] for q in proy if abs(q[0] - (lo + hi) / 2) < 3]
    r = (max(medio) - min(medio)) / 2
    return (cx + (lo + r) * ux, cy + (lo + r) * uy), (cx + (hi - r) * ux, cy + (hi - r) * uy), r


def mide(pj):
    """
    Que cada articulacion del rig caiga en el centro del extremo redondo de la pieza que gira (la rotula),
    medido sobre el alfa de los PNG. Un pivote en la esquina del rect hace que la pieza se despegue de la
    vecina al girar. Solo para personajes con las dos piezas dibujadas (arte final). Sale con 1 si alguna
    queda a mas de 2 px (el pivote de la rodilla es el centro de la antepierna, que gira; el muslo, que no
    gira, queda a mas de 12: el arte entrega muslo y antepierna alineados solo a unos 10 px).
    """
    piv = pj.pivote
    capas = {c.nodo.ruta: c for c in pj.capas}
    fallos = 0

    def centros(ruta):
        c = capas.get(ruta)
        if c is None:
            return None
        a, b, r = extremos_capsula(c.imagen)
        w, h = c.imagen.size
        rw, rh = c.rect[2] - c.rect[0], c.rect[3] - c.rect[1]
        f = lambda p: (c.rect[0] + p[0] * rw / w, c.rect[1] + p[1] * rh / h)
        return f(a), f(b), r

    def cerca(p, centros_):
        return min((math.hypot(p[0] - q[0], p[1] - q[1]), q) for q in centros_[:2])

    print("%-22s %-34s %8s" % ("articulacion", "punto del rig / centro redondo", "distancia"))
    for lado in ("Izq", "Der"):
        b, a = centros(LA if lado == "Izq" else RA), centros((LE if lado == "Izq" else RE) + "/Antebrazo" + lado)
        hombro, codo = piv[LA if lado == "Izq" else RA], piv[LE if lado == "Izq" else RE]
        if b is None or a is None:
            continue
        for nombre, punto, cs, tol in (("Hombro" + lado, hombro, b, 2.0), ("Codo (humero) " + lado, codo, b, 2.0), ("Codo (antebrazo) " + lado, codo, a, 2.0)):
            d, q = cerca(punto, cs)
            ok = d <= tol
            fallos += 0 if ok else 1
            print("%-22s (%5.1f, %5.1f) / (%5.1f, %5.1f) %8.1f %s" % (nombre, punto[0], punto[1], q[0], q[1], d, "ok" if ok else "FALLA"))
    for lado in ("Izq", "Der"):
        m, a = centros((LL if lado == "Izq" else RL)), centros((LK if lado == "Izq" else RK) + "/Antepierna" + lado)
        rod = piv[LK if lado == "Izq" else RK]
        if m is None or a is None:
            continue
        for nombre, cs in (("Rodilla (muslo) " + lado, m), ("Rodilla (antepierna) " + lado, a)):
            d, q = cerca(rod, cs)
            ok = d <= 12.0
            fallos += 0 if ok else 1
            print("%-22s (%5.1f, %5.1f) / (%5.1f, %5.1f) %8.1f %s" % (nombre, rod[0], rod[1], q[0], q[1], d, "ok" if ok else "FALLA"))
    print("las articulaciones caen en el centro de la rotula" if not fallos else "%d articulaciones mal colocadas" % fallos)
    return 1 if fallos else 0


def _clip_pose(valores, duracion=1.0):
    cur = [{"ruta": r, "propiedad": p, "claves": [[0.0, v], [duracion, v]]} for (r, p), v in valores.items()]
    return Clip({"archivo": "x", "accion": "x", "duracion": duracion, "bucle": True, "curvas": cur})


def autoprueba(rig):
    """
    La prueba tiene que FALLAR con poses malas a proposito (si no, un verde no quiere decir nada). Cada caso
    arma una pose del Nino y espera que falle la medida indicada.
    """
    R = K.ROT
    nino = Personaje("nino", rig)
    xn = K.leer_contexto("nino", rig)
    b = xn.brazos
    casos = []
    # 1. brazos colgando pegados al cuerpo: van tras el torso y se esconden
    casos.append(("brazos pegados, tras el torso", nino, {(LA, R): b["Izq"].hombro_rot(8), (RA, R): b["Der"].hombro_rot(8)}, "brazo"))
    # 1b. la cabeza se hunde tras el torso: la boca queda tapada por el torso (la cabeza va al fondo)
    casos.append(("barbilla y boca tras el torso", nino, {(NK, K.POSY): -70.0}, "torso"))
    # 2. un brazo en alto por delante de la cara
    casos.append(("brazo en alto delante de la cara", nino, {(RA, R): b["Der"].hombro_rot(165), (RE, R): 0.0}, "cara"))
    # 3. un codo al reves: brazo en alto y el antebrazo se dobla hacia fuera y abajo, no hacia la cabeza
    casos.append(("codo al reves (hiperextendido)", nino, {(LA, R): b["Izq"].hombro_rot(170), (LE, R): 100.0}, "codo"))
    # 3b. la mano a la cara (brazo doblado: la mano sube hasta la boca)
    rs, rc = b["Der"].ik((530, 470))
    casos.append(("mano en la cara", nino, {(RA, R): rs, (RE, R): rc}, "mano"))
    # 3c. lo mismo con el arte final SIMULADO de Mama (brazos, piernas y cabeza recortados del provisional)
    mama = Personaje("mama", rig, maqueta=True)
    bm = mama.ctx.brazos
    c = mama.ctx.cara
    rs, rc = bm["Der"].ik(((c[0] + c[2]) / 2.0 + 20, (c[1] + c[3]) / 2.0))
    casos.append(("maqueta de Mama: mano en la cara", mama, {(RA, R): rs, (RE, R): rc}, "mano"))
    casos.append(("maqueta de Mama: codo al reves", mama, {(LA, R): bm["Izq"].hombro_rot(170), (LE, R): 100.0}, "codo"))
    casos.append(("maqueta de Mama: brazos pegados", mama,
                  {(LA, R): bm["Izq"].hombro_rot(8), (RA, R): bm["Der"].hombro_rot(8)}, "brazo"))
    # 4. el pie atraviesa el suelo
    casos.append(("pie bajo el suelo", nino, {(LL, K.POSY): -30.0, (RL, K.POSY): -30.0}, "suelo"))
    # 5. la rodilla se dobla hacia fuera
    casos.append(("rodilla hacia fuera", nino, {(LK, R): -30.0, (RK, R): 30.0}, "codo"))
    malos = 0
    # el contrato con CharacterRig.cs: la lectura de armsInFrontActions
    for nombre, ok in P.autoprueba_contrato():
        malos += 0 if ok else 1
        print("%-40s %-9s %s" % ("contrato: " + nombre, "lectura", "bien" if ok else "FALLA"))
    # el orden de dibujo por accion: en Strike los brazos de la familia pasan tras el torso al frente; Algoritm no cambia
    for pid_, esperado in (("papa", True), ("nino", True), ("algoritm_fuego", False)):  # esperado: antes iban detras del torso
        pj_ = nino if pid_ == "nino" else Personaje(pid_, rig)
        ind_det = pj_.orden_de("Idle")[1]
        ind_del = pj_.orden_de("Strike")[1]
        brazo_r, torso_r = T + "/BrazoIzq", T + "/Torso"
        antes = ind_det[brazo_r] < ind_det[torso_r]
        delante = ind_del[brazo_r] > ind_del[torso_r]
        ok = (antes and delante) if esperado else (not antes and delante)
        malos += 0 if ok else 1
        print("%-40s %-9s %s" % ("orden de dibujo en Strike: " + pid_, "orden", "bien" if ok else "FALLA (los brazos no pasan delante del torso)"))
    # Strike delante del pecho: con los brazos delante se ve; la MISMA pose como accion de brazos detras (Idle) los esconde
    strike_nino = next(c for c in clips_de(cargar_clips(), "nino") if c.accion == "Strike")
    como_idle = copy.copy(strike_nino)
    como_idle.accion = "Idle"
    r1 = prueba_clip(nino, strike_nino, tiempos=[0.30])
    r2 = prueba_clip(nino, como_idle, tiempos=[0.30])
    ok = r1.brazo[0] >= UMBRAL_BRAZO * 100 and r2.brazo[0] < UMBRAL_BRAZO * 100 and "Strike" in nino.delante
    malos += 0 if ok else 1
    print("%-40s %-9s %s" % ("Strike al choque: delante se ve (%.0f %%), detras no (%.0f %%)" % (r1.brazo[0], r2.brazo[0]), "brazo",
                             "bien" if ok else "FALLA (el orden por accion no cambia lo que se ve)"))
    # Strike con las manos juntas a la altura de la ingle (lo que dan dos brazos rigidos iguales, que solo se encuentran en el eje a un
    # largo de brazo debajo del hombro): el choque bajo la cintura se detecta; y el Strike vigente del mismo personaje, no
    papa = Personaje("papa", rig)
    xp = papa.ctx
    vals = {}
    for lado, hom in (("Izq", LA), ("Der", RA)):
        br = xp.brazos[lado]
        meta = (xp.cx, br.S[1] + math.sqrt(br.largo ** 2 - (br.S[0] - xp.cx) ** 2))
        base = K._ang((br.M[0] - br.S[0], br.M[1] - br.S[1]))
        vals[(hom, R)] = K._norm(K._ang((meta[0] - br.S[0], meta[1] - br.S[1])) - base)
    ingle = _clip_pose(vals)
    ingle.accion = "Strike"
    r = prueba_clip(papa, ingle, tiempos=[0.0])
    ok, _ = fila("papa", ingle, r)
    detecta = not ok and r.choque is not None and r.choque[0] > r.choque[3]
    malos += 0 if detecta else 1
    print("%-40s %-9s %s" % ("Strike con las manos en la ingle", "choque", "detectada (y=%.0f, limite %.0f)" % (r.choque[0], r.choque[3]) if detecta else "NO SE DETECTA"))
    real = next(c for c in clips_de(cargar_clips(), "papa") if c.accion == "Strike")
    r = prueba_clip(papa, real, tiempos=[i / FPS for i in range(int(round(real.duracion * FPS)) + 1)])
    ok = r.choque is not None and r.choque[0] <= r.choque[3]
    malos += 0 if ok else 1
    print("%-40s %-9s %s" % ("el Strike vigente de Papa (hoy)", "choque", "bien (y=%.0f, limite %.0f)" % (r.choque[0], r.choque[3]) if ok else "FALLA (falso positivo)"))
    # Algoritm: sus manos van por encima de la cara y no deben ni rozarla (99,5 % de la caja ampliada)
    alg = Personaje("algoritm_fuego", rig)
    for ruta, ang, nombre in ((LA, 150.0, "un brazo sube a la cara"), (RA, 150.0, "un brazo apenas roza la cara")):
        r = prueba_clip(alg, _clip_pose({(ruta, R): ang}), tiempos=[0.0])
        ok, _ = fila("algoritm_fuego", _clip_pose({}), r)
        detecta = not ok and r.cara[0] < UMBRAL_CARA_GUIA * 100
        malos += 0 if detecta else 1
        print("%-40s %-9s %s" % ("Algoritm: " + nombre, "cara", "detectada (%.1f %%)" % r.cara[0] if detecta else "NO SE DETECTA"))
    # la maqueta, sumada, es el sprite original: sin eso la prueba con ella no vale (las piezas recortadas
    # tienen que dar el mismo dibujo en reposo que el arte provisional entero)
    vacio = Clip({"archivo": "x", "accion": "x", "duracion": 1.0, "bucle": True, "curvas": []})
    print("%-40s %s" % ("la maqueta suma el sprite original", "diferencia de pixeles en reposo (tope 0,6 %)"))
    for pid in ("papa", "mama", "nina"):
        im_hoy = render(Personaje(pid, rig), vacio, 0.0, 0.5, alfa_grupo=False).convert("RGB")
        im_maq = render(Personaje(pid, rig, maqueta=True), vacio, 0.0, 0.5, alfa_grupo=False).convert("RGB")
        dif = ImageChops.difference(im_hoy, im_maq).convert("L").point(lambda v: 255 if v > 40 else 0)
        frac = 100.0 * _cuenta(dif) / (dif.width * dif.height)
        ok = frac < 0.6
        malos += 0 if ok else 1
        print("%-40s %.3f %%  %s" % (pid, frac, "bien" if ok else "LA MAQUETA NO SUMA EL SPRITE"))
    print("%-40s %-9s %s" % ("pose mala", "medida", "resultado"))
    for nombre, pj, vals, medida in casos:
        r = prueba_clip(pj, _clip_pose(vals), tiempos=[0.0])
        ok, _ = fila("nino", _clip_pose(vals), r)
        detecta = not ok
        print("%-40s %-9s %s" % (nombre, medida, "detectada" if detecta else "NO SE DETECTA"))
        malos += 0 if detecta else 1
    # 6. un salto de rama: el codo da 150 grados en un cuadro (clip de dos claves con 1/30 s entre ellas)
    salto = Clip({"archivo": "x", "accion": "x", "duracion": 1.0, "bucle": True,
                  "curvas": [{"ruta": RE, "propiedad": K.ROT, "claves": [[0.0, 0.0], [1.0 / 30.0, 150.0], [1.0, 150.0]]}]})
    r = prueba_clip(nino, salto, tiempos=[0.0, 1.0 / 30.0, 2.0 / 30.0])
    ok, _ = fila("nino", salto, r)
    print("%-40s %-9s %s" % ("salto de rama del codo (150 deg/cuadro)", "giro", "detectada" if not ok and r.vel[0] > VELOCIDAD_MAX else "NO SE DETECTA"))
    malos += 0 if not ok and r.vel[0] > VELOCIDAD_MAX else 1
    # y la pose buena no debe fallar
    r = prueba_clip(nino, _clip_pose({(LA, R): b["Izq"].hombro_rot(xn.hang), (RA, R): b["Der"].hombro_rot(xn.hang)}), tiempos=[0.0])
    ok, _ = fila("nino", _clip_pose({}), r)
    print("%-40s %-9s %s" % ("reposo de brazos colgando", "todas", "bien" if ok else "FALLA (falso positivo)"))
    malos += 0 if ok else 1
    print("la prueba detecta lo que debe" if not malos else "%d casos mal" % malos)
    return 1 if malos else 0


# --------------------------------------------------------------------------- principal


def es_final(pid, rig):
    """El personaje ya tiene arte final entero (brazos, piernas y cabeza partidos) en el estado de «tras sprites»."""
    if P.PERSONAJES[pid][2]:
        return False
    a = P.arbol_vigente(pid, rig)
    return (P.esta_segmentado(a) and a[P.HD].dibuja() and a[P.LE + "/AntebrazoIzq"].dibuja()
            and a[P.RE + "/AntebrazoDer"].dibuja() and a[P.RK + "/AntepiernaDer"].dibuja())


def clips_de_doc(doc):
    return {p["id"]: [Clip(c) for c in p["clips"]] for p in doc["personajes"]}


def corrida(pid, modo, rig, clips, a):
    """Prueba (y hojas) de un personaje con un arte («hoy», «maqueta» o «final»): devuelve cuantos clips fallan."""
    pj = Personaje(pid, rig, a.orden, maqueta=(modo == "maqueta"))
    cl = clips_de(clips, pid)
    fallos = 0
    for clip in cl:
        r = prueba_clip(pj, clip)
        ok, linea = fila(pid + ("/" + modo if modo in ("hoy", "maqueta") else ""), clip, r)
        fallos += 0 if ok else 1
        print(linea)
    if not a.sin_hojas:
        base = pid if modo in ("final", "guia") else "%s_%s" % (pid, modo)
        hoja_idle(pj, cl, os.path.join(a.salida, "%s_idle_8.png" % base))
        hoja_todos(pj, cl, os.path.join(a.salida, "%s_todos.png" % base))
        gif_idle(pj, cl, os.path.join(a.salida, "%s_idle.gif" % base))
    return fallos


def main(argv=None):
    ap = argparse.ArgumentParser(description="Vista previa y prueba de visibilidad de clips_personajes.json")
    ap.add_argument("--json", default=CLIPS_JSON)
    ap.add_argument("--salida", default=os.path.join(tempfile.gettempdir(), "algoritmia_pose_preview"))
    ap.add_argument("--sin-hojas", action="store_true")
    ap.add_argument("--solo", nargs="*", help="ids de personaje (papa mama nina nino algoritm_fuego ...)")
    ap.add_argument("--hoy", action="store_true",
                    help="el arte que hay (el de los prefabs) con clips_personajes.json, que es lo que se vuelca al motor")
    ap.add_argument("--maqueta", action="store_true",
                    help="el arte final SIMULADO (maqueta.py) con los clips que el solucionador calcula para el: la coreografia "
                         "tiene que pasar tambien con el")
    ap.add_argument("--orden", help="otro orden de dibujo de Tronco, de atras adelante (Cuello,BrazoIzq,BrazoDer,Torso); por defecto el del JSON")
    ap.add_argument("--tira", nargs=4, metavar=("PERSONAJE", "ACCION", "N", "PNG"))
    ap.add_argument("--mide", metavar="PERSONAJE", help="comprueba las articulaciones del rig contra el alfa de las piezas")
    ap.add_argument("--autoprueba", action="store_true", help="comprueba que la prueba SI falla con poses malas a proposito")
    a = ap.parse_args(argv)
    a.orden = a.orden.split(",") if a.orden else None
    modos = [m for m, on in (("hoy", a.hoy), ("maqueta", a.maqueta)) if on] or ["hoy", "maqueta"]

    rig = P.cargar_rig()
    clips = cargar_clips(a.json)
    os.makedirs(a.salida, exist_ok=True)

    if a.mide:
        return mide(Personaje(a.mide, rig))
    if a.autoprueba:
        return autoprueba(rig)
    if a.tira:
        pid, accion, n, png = a.tira
        pj = Personaje(pid, rig, a.orden, maqueta=("maqueta" in modos and not a.hoy))
        cc = clips_de_doc(K.construir(rig, maqueta=True)[0]) if pj.maqueta else clips
        clip = next(c for c in clips_de(cc, pid) if c.accion.lower() == accion.lower())
        tira(pj, clip, int(n)).convert("RGB").save(png)
        print("escrito", png)
        return 0

    ids = a.solo or list(P.PERSONAJES)
    fallos = 0
    if "hoy" in modos and os.path.abspath(a.json) == os.path.abspath(CLIPS_JSON):
        # «hoy» prueba el JSON que se vuelca al motor: si no es lo que el solucionador calcula ahora, la prueba
        # diria algo de unos clips que ya no son los vigentes
        with open(a.json, encoding="ascii") as f:
            if f.read() != K.serializa(K.construir(rig)[0]):
                print("ERROR clips_personajes.json esta DESACTUALIZADO respecto a coreografia.py: corre coreografia.py")
                return 1
    print("excepciones de «mano en la caja de la cara»:")
    for (p_, c_), motivo in EXCEPCIONES_CARA.items():
        print("  %-8s %-8s %s" % (p_, c_, motivo))
    print("excepciones de «brazo visible >= 85 %%»:%s" % ("" if EXCEPCIONES_BRAZO else " ninguna"))
    for (p_, c_), (minimo, motivo) in EXCEPCIONES_BRAZO.items():
        print("  %-8s %-8s hasta %.0f %%: %s" % (p_, c_, minimo, motivo))
    delante, hallada = P.acciones_brazos_delante()
    print("brazos DELANTE del torso en: %s (%s)" % (", ".join(sorted(delante)),
          "leido de CharacterRig.armsInFrontActions" if hallada else "AVISO: CharacterRig.cs no trae la lista; lista por defecto"))
    print("excepciones de «giro maximo %.0f grados por segundo»:" % VELOCIDAD_MAX)
    for (p_, c_), motivo in EXCEPCIONES_VEL.items():
        print("  %-8s %-8s %s" % (p_, c_, motivo))
    print("%-18s %-10s %-8s %s" % ("personaje", "clip", "", "peor instante de cada medida"))
    clips_maqueta = None
    for pid in ids:
        if P.PERSONAJES[pid][2]:
            # Algoritm: su sprite es uno solo, asi que SIEMPRE es maqueta (pose_preview._capa_guia)
            print("--- %s: maqueta del sprite entero" % pid)
            fallos += corrida(pid, "guia", rig, clips, a)
        elif es_final(pid, rig):
            # ya tiene arte final (el Nino; Papa, Mama o Nina tras preparar_arte_final.py --aplicar): lo que hay es lo que habra
            print("--- %s: arte final (hoy = maqueta)" % pid)
            fallos += corrida(pid, "final", rig, clips, a)
        else:
            for modo in modos:
                if modo == "hoy":
                    print("--- %s hoy: el arte provisional real, brazos detras del torso (clips_personajes.json)" % pid)
                    fallos += corrida(pid, "hoy", rig, clips, a)
                else:
                    if clips_maqueta is None:
                        clips_maqueta = clips_de_doc(K.construir(rig, maqueta=True)[0])
                    print("--- %s maqueta: el arte final simulado (brazos, piernas y cabeza partidos), clips calculados para el" % pid)
                    fallos += corrida(pid, "maqueta", rig, clips_maqueta, a)
    print("\n%d clips con fallos" % fallos if fallos else "\nla prueba pasa en todos los clips")
    if not a.sin_hojas:
        print("hojas y GIF en", a.salida)
    return 1 if fallos else 0


if __name__ == "__main__":
    sys.exit(main())
