#!/usr/bin/env python3
# coreografia.py: la FUENTE UNICA de la coreografia de los siete personajes (Papa, Mama, Nina,
# Nino y Algoritm, que comparte clips en sus tres formas). Escribe clips_personajes.json, que
# BuildRigsFinal.cs.txt (modo «clips») lee y vuelca en los .anim existentes; el C# ya no lleva
# la coreografia.
#
#     python3 claudeDocs/tasks/Personajes/herramientas/coreografia.py              # escribe el JSON (con el arte de HOY) y lo valida
#     python3 claudeDocs/tasks/Personajes/herramientas/coreografia.py --autoprueba # valida() detecta JSON rotos
#     python3 claudeDocs/tasks/Personajes/herramientas/pose_preview.py             # la prueba visual, con el arte de hoy (--hoy)
#                                                                                  # y con el arte final SIMULADO (--maqueta)
#     python3 claudeDocs/tasks/Personajes/herramientas/coreografia_v0.py           # regresion del motor
#
# Orden de trabajo cuando llega arte final de Papa, Mama o Nina:
#   1. preparar_arte_final.py <id> <carpeta_entrega> [--aplicar]: reconoce las piezas, las limpia y normaliza,
#      las mide (rect y punto de cada articulacion, entrada del personaje en arte_final.json) y, con --aplicar,
#      copia los PNG al repo, lleva el pivote del hombro al borde del torso (hombro.py: Santiago, 06/10/2026; el arte entrega el humero con su
#      extremo redondo en el centro del pecho y, girando desde ahi, el reposo salia en A), encaja el humero y el antebrazo en el codo si el humero
#      se pasa del casquete (codo.py: Santiago, 08/10/2026; Papa lo entrego solapado y el humero asomaba al doblar el codo), regenera rig_articulaciones.json (articulaciones.py)
#      y este JSON de clips, y corre pose_preview.py; imprime las ordenes para la sesion local. Es todo lo que hace falta antes de Unity;
#   2. en el Editor, BuildRigsFinal.cs.txt, modos «sprites», «orden» (si hace falta) y «clips».
# La coreografia no se toca: la cinematica de los brazos y las medidas de las piernas salen del JSON del rig y
# de los PNG, y prefabs.simula_sprites hace que este script y pose_preview.py ya traten al personaje como
# segmentado aunque el prefab del disco aun no haya pasado por «sprites».
# INC-133 y la cara registrada (06/10/2026). En la familia el HUMERO va detras del torso (y, en Papa, de la cabeza: su Cuello va despues de los
# brazos) y el ANTEBRAZO delante de todo: el solucionador solo cuenta como oculto el humero (Brazo.oculta, UMBRAL_OCULTO; el reposo es mas estricto,
# UMBRAL_OCULTO_REPOSO) y lo que lo tapa es la silueta del torso mas, en Papa, la de la cabeza. Con la cara de la entrega (ojos y cejas de lado a
# lado del ovalo) una mano o un antebrazo cerca de la cabeza tapa rasgos: Brazo.pose desvia los gestos por angulo que cruzarian la caja de la cara.
# EXCEPCION (Santiago, 06/10/2026, «acepto que cubra el rostro»): los gestos de CARA_TAPADA (visera de Observe, Celebrate, Hammer, Encourage, Carry
# y el rascado del Nino) ya no se desvian: el brazo hace lo que el gesto pide, la mano llega de verdad a la sien o a la frente (cabeza_objetivo) y
# pose_preview.py solo les relaja la cara (nunca los dos ojos a la vez). Los clips NUNCA animan AntebrazoX ni su
# ancla: solo BrazoX y BrazoX/CodoX (el ancla vive bajo el codo y el motor copia su pose al antebrazo).
#
# PERFIL (INC-134, 09/10/2026). La familia (Papa, Mama, Nina y Nino; Algoritm no) tiene un SEGUNDO cuerpo, Lienzo/Perfil, dibujado de perfil y mirando a
# la DERECHA, que el motor enseña en las siete acciones de ActionView (ACCIONES_PERFIL = Walk, Run, Carry, Push, PickUp, Kneel y Blow) y apaga en las
# demas. Este script anade, al mismo clip y con la misma duracion que su version de frente, las curvas del cuerpo de perfil (rutas Lienzo/Perfil/…, las
# mismas propiedades: localEulerAnglesRaw.z, m_AnchoredPosition y m_LocalScale): las piernas oscilan adelante y atras con la rodilla doblada en el balanceo,
# los brazos a contrafase con el codo llegando tarde, el tronco inclinado hacia delante con las piernas compensandolo (cuelgan de el), squash & stretch <= 15 %
# en Perfil, anticipacion al recoger, una rodilla en el suelo al arrodillarse y la agachada al soplar. TODO clip de la familia fija ademas las 12 articulaciones
# de perfil (constantes, en su reposo, en las acciones de frente) para que ninguna caiga a la pose en T; los huesos de frente no cambian en ningun clip. Las
# longitudes salen de la clave «perfil» de rig_articulaciones.json (PROVISIONALES hasta que llegue el arte de perfil: preparar_perfil.py las sustituye y este script
# se vuelve a correr sin tocar la coreografia). --valida y --autoprueba comprueban ademas con una cinematica de la figura de perfil (GeoPerfil, valida_perfil)
# que los pies no atraviesan el suelo, que ninguna rodilla ni codo se dobla al reves, el giro maximo, el squash & stretch y que no hay poses de caida (CP-02).
# OJO: la constante PERFIL de mas abajo es el angulo de reposo de los brazos de cada personaje, no una vista de lado.
#
# Los prefabs se leen con prefabs.py. Pillow hace falta para medir la silueta de los pies y el grosor de los
# brazos (sin el se usan los rects) y para la prueba.
#
# Este archivo tiene tres capas:
#   1. EL MOTOR (Curva, Spec, Ctx, Crouch...): port fiel de la clase Spec de BuildRigsFinal.cs.txt
#      —Rot, Raw, Sym, Vol, Follow, Drop, Finish— y de la evaluacion de curvas ClampedAuto de Unity.
#      coreografia_v0.py lo usa para reproducir la coreografia ANTERIOR y compararla con los .anim
#      del repo (diferencia maxima en valores de clave < 0,01): eso demuestra que el port es fiel.
#   2. LA COREOGRAFIA vigente (HumanClips, GuideClips y los clips de cada accion).
#   3. La escritura y la validacion del JSON.
#
# SIGNOS (los mismos que el comentario de BuildRigsFinal.cs.txt; el eje Z de Unity gira en sentido
# antihorario con valores positivos y la pantalla es el espejo del personaje que mira al jugador):
#   Hombro (BrazoIzq/BrazoDer): + en BrazoDer y - en BrazoIzq ABREN y SUBEN el brazo (elevacion); al
#     reves, el brazo se cierra contra el cuerpo. Una pose de reposo de +20 en BrazoIzq y -20 en
#     BrazoDer deja los brazos 20 grados mas cerca del cuerpo que el A-pose del prefab.
#   Codo: la flexion «hacia arriba» (la mano sube hacia el hombro) es - en CodoIzq y + en CodoDer;
#     el pliegue «hacia dentro» (manos a la cintura) es el contrario.
#   Pierna: - en PiernaIzq y + en PiernaDer ABREN el muslo (rodillas hacia fuera).
#   Rodilla: + en RodillaIzq y - en RodillaDer devuelven la pantorrilla HACIA DENTRO.
#   Cuerpo/Tronco/Cuello/Cabeza: + inclina a la izquierda de la pantalla, - a la derecha. La
#     inclinacion de Walk/Run es negativa: hacia donde mira el personaje.
#   «Sym(izq, der, t, v...)»: v > 0 abre/sube/flexiona hacia arriba; Izq recibe -v y Der +v (sobre la
#     pose de reposo del hueso). «Rot» da el valor tal cual (SUMADO al reposo).
#
# Todo el texto de este archivo y del JSON que escribe es ASCII salvo los comentarios con tildes; el
# JSON se escribe con ensure_ascii y se comprueba (--valida) porque el lector de C# es propio.

import json
import math
import os
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import prefabs as P  # noqa: E402

FPS = 30.0
SALIDA = os.path.join(AQUI, "clips_personajes.json")

# Rutas de los huesos (iguales en la familia y en Algoritm, salvo Cuello y Cabeza).
C, T, LA, RA, LE, RE = P.C, P.T, P.LA, P.RA, P.LE, P.RE
LL, RL, LK, RK, NK, HD = P.LL, P.RL, P.LK, P.RK, P.NK, P.HD

ROT = "localEulerAnglesRaw.z"
POSX = "m_AnchoredPosition.x"
POSY = "m_AnchoredPosition.y"
ESCX = "m_LocalScale.x"
ESCY = "m_LocalScale.y"
ALFA = "m_Alpha"
PROPIEDADES = (ROT, POSX, POSY, ESCX, ESCY, ALFA)

HUESOS_FAMILIA = [C, T, LA, RA, LE, RE, LL, RL, LK, RK, NK, HD]
HUESOS_GUIA = [C, T, LA, RA, LE, RE, LL, RL, LK, RK]

# INC-134 (09/10/2026): el cuerpo de PERFIL de la familia. Cada clip de la familia lleva ademas una curva de rotacion en CADA una de estas 12
# articulaciones (las acciones de frente la dejan constante en su reposo, para que ningun hueso de perfil quede sin curva y caiga a la pose en T),
# y los siete clips de ACCIONES_PERFIL las animan. Los huesos de frente no cambian en ningun clip. OJO con el nombre: PERFIL (mas abajo) es el angulo de
# reposo de los brazos de cada personaje, no una vista de lado.
PF, PT, PBL, PEL, PBC, PEC = P.PF, P.PT, P.PBL, P.PEL, P.PBC, P.PEC
PLL, PKL, PLC, PKC, PNK, PHD = P.PLL, P.PKL, P.PLC, P.PKC, P.PNK, P.PHD
HUESOS_PERFIL = P.HUESOS_PERFIL
ACCIONES_PERFIL = P.ACCIONES_PERFIL
HUESOS_FAMILIA_COMPLETA = HUESOS_FAMILIA + HUESOS_PERFIL


class Familia:
    """Tempo (>1 mas lento), amplitud y flexion de reposo de los codos (grados, hacia arriba)."""

    def __init__(self, carpeta, tempo, amp, codo):
        self.carpeta, self.tempo, self.amp, self.codo = carpeta, tempo, amp, codo


FAMILIA = {
    "papa": Familia("Father", 1.15, 1.1, 4.0),
    "mama": Familia("Mother", 1.0, 0.9, 7.0),
    "nina": Familia("Girl", 0.95, 1.0, 9.0),
    "nino": Familia("Boy", 0.8, 1.2, 6.0),
}


# ============================================================================ 1. EL MOTOR


def repeat(t, largo):
    """Mathf.Repeat."""
    return t - math.floor(t / largo) * largo


def clamp(v, lo, hi):
    return max(lo, min(hi, v))


def pendiente_auto(p, k, n):
    """
    Tangente ClampedAuto de Unity en una clave interior (p, k, n son (tiempo, valor)). Se midio sobre
    los 2584 puntos interiores de los .anim del repo y coincide con todos (error < 2e-6 relativo):
      - extremo local o tramo plano (los dos saltos no tienen el mismo signo): 0;
      - si no: el secante entre vecinos (d1 + d2) / (t_n - t_p), con el modulo recortado a
        4 * min(|d1|, |d2|) / (t_n - t_p) para que la curva no se pase de los vecinos.
    Las claves de los extremos llevan tangente 0.
    """
    d1 = k[1] - p[1]
    d2 = n[1] - k[1]
    if d1 * d2 <= 0:
        return 0.0
    m = min(abs(d1 + d2), 4.0 * min(abs(d1), abs(d2)))
    return (m if d1 > 0 else -m) / (n[0] - p[0])


class CurvaAuto:
    """AnimationCurve con tangentes ClampedAuto; fuera del rango, ClampForever (el Evaluate por defecto)."""

    def __init__(self, claves):
        self.k = [(float(t), float(v)) for t, v in claves]
        n = len(self.k)
        self.m = [0.0] * n
        for i in range(1, n - 1):
            self.m[i] = pendiente_auto(self.k[i - 1], self.k[i], self.k[i + 1])

    def evaluar(self, t):
        k = self.k
        if not k:
            return 0.0
        if t <= k[0][0]:
            return k[0][1]
        if t >= k[-1][0]:
            return k[-1][1]
        lo, hi = 0, len(k) - 1
        while hi - lo > 1:
            mid = (lo + hi) // 2
            if k[mid][0] <= t:
                lo = mid
            else:
                hi = mid
        t0, v0 = k[lo]
        t1, v1 = k[hi]
        dt = t1 - t0
        s = (t - t0) / dt
        s2, s3 = s * s, s * s * s
        return ((2 * s3 - 3 * s2 + 1) * v0 + (s3 - 2 * s2 + s) * dt * self.m[lo]
                + (-2 * s3 + 3 * s2) * v1 + (s3 - s2) * dt * self.m[hi])


class Curva:
    def __init__(self, ruta, prop):
        self.ruta, self.prop = ruta, prop
        self.claves = []  # [tiempo, valor]

    def set(self, t, v):
        for i, c in enumerate(self.claves):
            if abs(c[0] - t) < 1e-4:
                self.claves[i] = [c[0], v]
                return
            if c[0] > t:
                self.claves.insert(i, [t, v])
                return
        self.claves.append([t, v])


class Spec:
    """Un clip: curvas por (ruta, propiedad). Port de la clase Spec de BuildRigsFinal.cs.txt."""

    def __init__(self, accion, archivo, largo_base, k, bucle, reposo, huesos):
        self.accion, self.archivo, self.k, self.bucle = accion, archivo, k, bucle
        self.largo = largo_base * k
        self._reposo, self._huesos = reposo, huesos
        self._curvas = {}
        self.orden = []

    # -- acceso
    def _get(self, ruta, prop, crear=True):
        clave = (ruta, prop)
        c = self._curvas.get(clave)
        if c is None:
            if not crear:
                return None
            c = Curva(ruta, prop)
            self._curvas[clave] = c
            self.orden.append(c)
        return c

    def reposo(self, ruta):
        return self._reposo.get(ruta, 0.0)

    def _base(self, ruta, prop):
        if prop == ROT:
            return self.reposo(ruta)
        return 1.0 if prop in (ESCX, ESCY) else 0.0

    # -- API de coreografia
    def rot(self, ruta, *tv):
        """Rotacion en Z: pares (tiempo base, grados) SUMADOS a la pose de reposo del hueso. Tiempos x tempo."""
        c = self._get(ruta, ROT)
        for i in range(0, len(tv) - 1, 2):
            c.set(tv[i] * self.k, tv[i + 1] + self.reposo(ruta))
        return self

    def raw(self, ruta, prop, *tv):
        """Cualquier otra propiedad, con el valor absoluto."""
        c = self._get(ruta, prop)
        for i in range(0, len(tv) - 1, 2):
            c.set(tv[i] * self.k, tv[i + 1])
        return self

    def sym(self, izq, der, *tv):
        """Movimiento simetrico: v > 0 abre, sube o flexiona hacia arriba (Izq recibe -v, Der +v)."""
        a = self._get(izq, ROT)
        b = self._get(der, ROT)
        for i in range(0, len(tv) - 1, 2):
            a.set(tv[i] * self.k, self.reposo(izq) - tv[i + 1])
            b.set(tv[i] * self.k, self.reposo(der) + tv[i + 1])
        return self

    def vol(self, *tv, ruta=None):
        """
        Squash & stretch de Cuerpo (o de «ruta»: Lienzo/Perfil en el cuerpo de perfil): pares (tiempo, escala Y). X compensa
        (1 - 0,5 (Y - 1)): con Y entre 0,92 y 1,08 la X queda entre 1,04 y 0,96 y ninguna pasa del 15 % (DA 13.2).
        """
        y = self._get(ruta or C, ESCY)
        x = self._get(ruta or C, ESCX)
        for i in range(0, len(tv) - 1, 2):
            y.set(tv[i] * self.k, tv[i + 1])
            x.set(tv[i] * self.k, 1.0 - 0.5 * (tv[i + 1] - 1.0))
        return self

    def drop(self, ruta, prop):
        c = self._curvas.pop((ruta, prop), None)
        if c is not None:
            self.orden.remove(c)
        return self

    def follow(self, src, src_prop, dst, cuadros, ganancia, sesgo=0.0):
        """
        Movimiento secundario: la curva «dst» (rotacion) copia la de «src» con unos cuadros de retraso
        (2 a 4 en la cabeza) y una ganancia. Sustituye lo que «dst» tuviera. En los bucles el retraso da
        la vuelta; en los otros se sujeta a los extremos. «sesgo» resta una media a la fuente.
        """
        fuente = self._get(src, src_prop, False)
        if fuente is None or not fuente.claves:
            raise RuntimeError("%s: follow necesita primero la curva %s %s" % (self.accion, src, src_prop))
        self._cerrar(fuente)
        curva = CurvaAuto(fuente.claves)
        destino = self._get(dst, ROT)
        destino.claves = []
        retraso = cuadros / FPS
        desde = self._base(src, src_prop)
        hasta = self.reposo(dst)
        for t in self._tiempos_muestreo(fuente.claves):
            tt = repeat(t - retraso, self.largo) if self.bucle else clamp(t - retraso, 0.0, self.largo)
            destino.set(t, hasta + ganancia * (curva.evaluar(tt) - desde - sesgo))
        return self

    def _tiempos_muestreo(self, claves):
        ts = [0.0]
        for i, c in enumerate(claves):
            if i > 0:
                ts.append(0.5 * (claves[i - 1][0] + c[0]))
            ts.append(c[0])
        ts.append(self.largo)
        ts = [clamp(t, 0.0, self.largo) for t in ts if -1e-4 <= t <= self.largo + 1e-4]
        out = []
        for t in sorted(ts):
            if not out or abs(t - out[-1]) > 1e-6:
                out.append(t)
        return out

    def _cerrar(self, c):
        """En los bucles, que la curva empiece en 0 y acabe en largo (si falta, con el valor del otro extremo)."""
        if not self.bucle or not c.claves:
            return
        if c.claves[0][0] > 1e-4:
            c.claves.insert(0, [0.0, c.claves[-1][1]])
        if c.claves[-1][0] < self.largo - 1e-4:
            c.claves.append([self.largo, c.claves[0][1]])

    def finish(self):
        """
        Ultimo paso: todo hueso sin rotacion se queda en su pose de reposo (anti pose en T), cada curva
        cubre [0, largo] y, en los bucles, acaba donde empieza. Devuelve los avisos.
        """
        avisos = []
        for hueso in self._huesos:
            if self._get(hueso, ROT, False) is None:
                self._get(hueso, ROT).set(0.0, self.reposo(hueso))
        for c in self.orden:
            if len(c.claves) == 1:
                c.set(self.largo, c.claves[0][1])
            if self.bucle:
                self._cerrar(c)
                brecha = c.claves[-1][1] - c.claves[0][1]
                if c.prop == ROT:
                    brecha = repeat(brecha + 180.0, 360.0) - 180.0  # una vuelta entera (Spin) si cierra
                if abs(brecha) > 1e-3:
                    avisos.append("%s %s %s: el bucle no cierra (%s -> %s)" % (
                        self.accion, c.ruta, c.prop, c.claves[0][1], c.claves[-1][1]))
            else:
                if c.claves[0][0] > 1e-4:
                    c.claves.insert(0, [0.0, c.claves[0][1]])
                if c.claves[-1][0] < self.largo - 1e-4:
                    c.claves.append([self.largo, c.claves[-1][1]])
            for t, _ in c.claves:
                if t < -1e-4 or t > self.largo + 1e-4:
                    avisos.append("%s %s %s: clave fuera de [0, largo] en t=%s" % (self.accion, c.ruta, c.prop, t))
        return avisos


class Ctx:
    """Lo que cada clip necesita saber del personaje (lo deduce de los prefabs y del rig, como ReadContext)."""

    def __init__(self, pid, guia=False, segmentado=False, k=1.0, a=1.0, codo=0.0, pierna=1.0, muslo=1.0, canilla=1.0):
        self.id, self.guia, self.segmentado = pid, guia, segmentado  # segmentado: las PIERNAS vienen partidas (antepierna con sprite)
        self.k, self.a, self.codo = k, a, codo
        self.pierna, self.muslo, self.canilla = pierna, muslo, canilla  # cadera-suelo, cadera-rodilla, rodilla-suelo
        self.brazos = {}      # "Izq"/"Der" -> Brazo (geometria del brazo para la cinematica; cada uno sabe si esta partido)
        self.cabeza_propia = False  # la cabeza es una pieza aparte (Cuello/Cabeza con sprite); si no, va dentro del torso
        self.cx = 512.0       # eje del cuerpo
        self.hombro_y = 0.0
        self.suelo = P.SUELO
        self.cara = None      # (x0, y0, x1, y1): la zona de ojos y boca
        self.y_pecho = self.y_vientre = self.y_cadera = 0.0  # alturas de referencia del tronco
        self.y_cintura = 0.0  # lo mas bajo que puede quedar un choque de manos (Strike): la cadera del JSON menos MARGEN_CINTURA, o y_cadera si es mas alta
        self.pie_x = {"Izq": 512.0, "Der": 512.0}  # x de cada pie (la cadera) en el lienzo
        self.pie_piv = {}     # "Izq"/"Der" -> (x, y) del pivote que gira esa pieza (la cadera, o la rodilla con piernas partidas)
        self.pie_casco = {}   # "Izq"/"Der" -> cierre convexo de la pieza que toca el suelo, relativo a ese pivote
        self.pata = {}        # "Izq"/"Der" -> (pierna, muslo, canilla) de ESA pierna (el dibujo no es simetrico)
        self.muslo_bajo = {"Izq": 0.0, "Der": 0.0}  # con piernas partidas: cuanto baja el muslo (su rotula) por debajo del pivote de la rodilla
        self.silueta = None   # Silueta del torso (lo que tapa a los brazos); None en Algoritm
        self.y_frente = 0.0   # altura de la frente: por encima de ojos y boca y a un brazo estirado del hombro
        self.alcance = 0.0    # largo del brazo con antebrazo (humero + antebrazo): la escala de los gestos de los brazos
        self.hang = 0.0       # reposo: angulo del brazo respecto a la vertical (0 = colgando pegado al cuerpo)
        self.pliegue = 0.0    # reposo: flexion de los codos hacia dentro
        self.delante = set()  # acciones en que los brazos van DELANTE del torso (CharacterRig.armsInFrontActions); las demas, detras
        self.delante_hallada = False  # la lista se leyo de CharacterRig.cs (si no, la de por defecto)
        self.perfil = None    # GeoPerfil: la figura de perfil (INC-134), de la clave «perfil» del JSON del rig; None en Algoritm


def leer_contexto_prefab(pid, rig=None):
    """
    Port exacto de ReadContext: las medidas de la pierna salen del PREFAB tal como esta hoy. Es lo que usa
    la regresion (coreografia_v0.py): el C# que genero los .anim vigentes leyo el prefab.
    """
    rig = rig or P.cargar_rig()
    prefab = P.PERSONAJES[pid][0]
    arbol = P.leer_arbol(prefab)
    guia = P.PERSONAJES[pid][2]
    if guia:
        return Ctx(pid, guia=True)
    info = FAMILIA[pid]
    seg = P.esta_segmentado(arbol)
    cadera = arbol[LL].pivote[1]
    suelo = arbol[C].pivote[1]  # el pivote de Cuerpo esta en el suelo
    rodilla = arbol[LK].pivote[1]
    pierna = max(suelo - cadera, 1.0)
    muslo = clamp(rodilla - cadera, 0.25 * pierna, 0.75 * pierna)
    return Ctx(pid, False, seg, info.tempo, info.amp, info.codo, pierna, muslo, pierna - muslo)


# ---------------------------------------------------------------------------- cinematica de los brazos
#
# Los gestos se piden como «la mano va AQUI» y la cinematica inversa (dos segmentos: humero y antebrazo)
# saca la rotacion del hombro y del codo. Asi un gesto vale para cualquier proporcion y para el arte que
# llegue despues (se vuelve a correr este script con el JSON nuevo): NADA depende del personaje, solo de
# la geometria que se lee (el JSON del rig y el prefab). Si el brazo tiene antebrazo (Image con sprite y
# encendida) son dos segmentos; si es de una pieza (arte provisional) el hombro apunta la mano hacia el
# objetivo sin pasarse del eje del cuerpo ni cruzar la cara, y el codo (un pivote vacio hoy) lleva igual su
# curva, la que le toca con antebrazo, para cuando llegue.
#
# Angulos: «ang» es el angulo polar en pantalla (antihorario, y hacia arriba) de un vector del lienzo
# (y hacia abajo); girar un nodo en Z por phi suma phi a ese angulo (la rotacion + de Unity es antihoraria).


def _ang(v):
    return math.degrees(math.atan2(-v[1], v[0]))


def _norm(a):
    return (a + 180.0) % 360.0 - 180.0


def _pol(grados, largo):
    r = math.radians(grados)
    return (largo * math.cos(r), -largo * math.sin(r))


def _dist_rect(p, r):
    """Distancia de un punto a un rectangulo (x0, y0, x1, y1); 0 si esta dentro."""
    dx = max(r[0] - p[0], 0.0, p[0] - r[2])
    dy = max(r[1] - p[1], 0.0, p[1] - r[3])
    return math.hypot(dx, dy)


class Silueta:
    """
    La silueta opaca de lo que se dibuja DELANTE de los brazos (el torso: «orden_tronco» de la familia pone los brazos
    detras del torso y delante de la cabeza), en el lienzo de 1024 a la mitad de resolucion. Con ella cada brazo sabe
    cuanto de si queda tapado en una pose (Brazo.oculta). Con el arte provisional la cabeza esta pintada dentro del
    torso, asi que su silueta incluye la cabeza.
    """

    ESC = 0.5

    def __init__(self, imagen, rect, otras=()):
        """«otras»: [(imagen, rect)] de lo demas que tambien se dibuja delante del humero (en Papa, la cabeza con la barba: INC-133)."""
        from PIL import Image, ImageChops
        lado = int(1024 * self.ESC) + 2
        m = Image.new("L", (lado, lado), 0)
        for im, r in [(imagen, rect)] + list(otras):
            a = im.getchannel("A").point(lambda v: 255 if v > 127 else 0)
            w = max(1, int(round((r[2] - r[0]) * self.ESC)))
            h = max(1, int(round((r[3] - r[1]) * self.ESC)))
            capa = Image.new("L", (lado, lado), 0)
            capa.paste(a.resize((w, h), Image.BOX), (int(round(r[0] * self.ESC)), int(round(r[1] * self.ESC))))
            m = ImageChops.lighter(m, capa)
        self.px, self.lado = m.load(), lado

    def dentro(self, x, y):
        ix, iy = int(x * self.ESC), int(y * self.ESC)
        return 0 <= ix < self.lado and 0 <= iy < self.lado and self.px[ix, iy] > 127


# El REPOSO es mas estricto que el gesto: los clips mueven los brazos unos grados alrededor de el sin pasar por el solucionador (el vaiven del
# Idle, el balanceo del Walk), y un reposo justo en el borde de lo visible los empujaria al otro lado.
# HOMBRO EN EL HOMBRO (Santiago, 06/10/2026, hombro.py): el pivote del humero ya no esta en el centro del pecho sino en el borde del torso, donde
# esta un hombro, y con el brazo cerca del cuerpo el borde del torso y de la falda (Mama, el Nino) tapan siempre parte del humero, que va detras
# de ellos: con el humero a 10 grados queda a la vista entre el 30 y el 60 % segun el personaje (a 20, entre el 40 y el 70 %). Por eso los
# umbrales bajan de 40/45/60 % a 30/34/40 % a la vista: la prueba (pose_preview.UMBRAL_HUMERO) pide el 30 %, el solucionador de los gestos el 34 %
# y el reposo el 40 %. El antebrazo, que va delante de todo, sigue pidiendo el 85 % y es lo que se lee como brazo.
UMBRAL_OCULTO_REPOSO = 0.60
# INC-133: el ANTEBRAZO va delante de todo y nunca queda tras el torso; solo el HUMERO puede quedar tras el (y, en Papa, tras la cabeza). La
# fraccion que puede ocultarse es la del humero: la prueba (pose_preview.UMBRAL_HUMERO) deja el 70 % y el modelo, un poco menos, para que el
# giro del cuello y el redondeo no la pasen. Con arte provisional (una sola pieza por brazo, sin antebrazo suelto) se mide el brazo entero.
UMBRAL_OCULTO = 0.66    # fraccion del HUMERO que puede quedar tras el torso: la prueba deja el 70 %; el modelo de capsulas usa lo que sobra de la cupula del hombro
PASO_MUESTRA = 9.0     # px entre muestras a lo largo del brazo
ANCHOS_MUESTRA = (-0.4, -0.2, 0.0, 0.2, 0.4)  # a lo ancho, en fracciones del grosor
ATRAS_HOMBRO = 0.0     # el modelo arranca esto (en grosores) por detras del pivote del hombro (la cupula se mide aparte: base_cap)
EXTRA_PUNTA = 1.12     # la mano llega un poco mas alla del punto M del modelo


MIN_ALCANCE = 0.45  # fraccion del largo del brazo a la que la mano puede acercarse al hombro sin doblar el codo en horquilla
PASO_APUNTA = 0.5   # grados: el paso con que el brazo de una pieza busca una direccion que no cruce el eje ni la cara
PISO_MINIMO_STRIKE = 0.22  # lo mas cerca del hombro que puede quedar la mano al chocar (Strike), en fracciones del alcance, si el choque no llega a la cintura con el minimo de siempre
MARGEN_CINTURA = 45.0  # el choque de Strike queda por encima de la cintura: de la cadera del JSON menos esto (px del lienzo); pose_preview lo vigila
ESC_MIN_BRAZO = 0.45  # un brazo de UNA pieza que se acerca la mano al hombro se ENCOGE (escala X e Y, como si apuntara hacia el espectador) hasta esta fraccion


class Brazo:
    """Un brazo en la pose del prefab (A-pose): hombro S, codo E, mano M (puntos del lienzo)."""

    def __init__(self, lado, hombro, codo, mano, partido, mano_dos=None):
        self.lado, self.S, self.E, self.M, self.partido = lado, hombro, codo, mano, partido
        self.v1 = (codo[0] - hombro[0], codo[1] - hombro[1])
        self.v2 = (mano[0] - codo[0], mano[1] - codo[1])
        self.l1 = math.hypot(*self.v1)
        self.l2 = math.hypot(*self.v2)
        self.a1, self.a2 = _ang(self.v1), _ang(self.v2)
        self.largo = math.hypot(mano[0] - hombro[0], mano[1] - hombro[1])  # alcance
        self.lado_sig = -1.0 if lado == "Izq" else 1.0  # +1 = hacia la derecha de la pantalla
        # angulo de la pieza que gira con el hombro respecto a la vertical hacia abajo, en la A-pose (+ = hacia fuera):
        # el humero si hay antebrazo; el brazo entero (hombro-mano) si es de una pieza
        eje = self.v1 if partido else (mano[0] - hombro[0], mano[1] - hombro[1])
        self.alfa = math.degrees(math.atan2(abs(eje[0]), eje[1]))
        # lo que el brazo de una pieza respeta al apuntar (lo rellena leer_contexto): el eje del cuerpo, el grosor
        # del brazo y las cajas que no debe cruzar (la cara)
        self.cx = 512.0
        self.grosor = 0.0
        self.evita = []       # las cajas que el brazo no debe cruzar AHORA (vacia en los gestos que dejan tapar la cara: permite_tapar_cara)
        self.evita_cara = []  # la caja de la cara, siempre: lo que evita cuando el gesto no deja tapar la cara
        self.silueta = None   # Silueta de lo que tapa al brazo (el torso); sin ella no hay restriccion de visibilidad
        self.delante = False  # en la accion que se escribe los brazos van DELANTE del torso (pon_delante): lo tapado no cuenta
        self.antebrazo_delante = False  # INC-133: el antebrazo se dibuja delante del torso y la cabeza (orden_tronco lo lista): solo el humero puede quedar tapado
        self.base_cap = 0.0   # fraccion del brazo que SIEMPRE queda tras el torso (la cupula del hombro): solo con el modelo de capsulas
        self.pts_h = []       # pixeles del sprite del brazo (o del humero) relativos al hombro; los del antebrazo, relativos al codo
        self.pts_f = []
        self.tabla = None     # fraccion oculta por (giro del hombro, giro del codo): ver prepara()
        # el modelo de dos segmentos del mismo brazo: de el sale la curva del codo cuando todavia no hay antebrazo
        self.dos = None if partido else Brazo(lado, hombro, codo, mano_dos or mano, True)

    # --- por angulo respecto a la vertical (0 = colgando, 90 = horizontal, 180 = en alto)
    def hombro_rot(self, theta):
        """Rotacion Z del hombro para que el humero (o el brazo entero) forme theta con la vertical hacia abajo."""
        return self.alfa - theta if self.lado == "Izq" else theta - self.alfa

    def codo_rot(self, theta, pliegue):
        """
        Rotacion Z del codo para plegar el antebrazo «pliegue» grados hacia el eje del cuerpo (con el brazo
        colgando, theta < 30) o hacia la cabeza (con el brazo horizontal o en alto, theta > 90); entre
        medias el sentido se funde, porque de frente ahi no hay uno que se vea mal. Con el brazo recto, 0.
        """
        s = clamp((60.0 - theta) / 30.0, -1.0, 1.0)  # el signo cambia sin salto cerca de la horizontal
        return s * pliegue if self.lado == "Izq" else -s * pliegue

    # --- visibilidad: los brazos van tras el torso, asi que solo valen las poses en que se ven
    def segmentos(self, rs, rc):
        """[(p, q)] con los ejes del brazo (humero y antebrazo, o el brazo entero) para esos giros."""
        if self.partido:
            e, m = self.fk(rs, rc)
            ux, uy = (e[0] - self.S[0]) / self.l1, (e[1] - self.S[1]) / self.l1
            ini = (self.S[0] - ux * ATRAS_HOMBRO * self.grosor, self.S[1] - uy * ATRAS_HOMBRO * self.grosor)
            return [(ini, e), (e, (e[0] + (m[0] - e[0]) * EXTRA_PUNTA, e[1] + (m[1] - e[1]) * EXTRA_PUNTA))]
        base = _ang((self.M[0] - self.S[0], self.M[1] - self.S[1]))
        p = _pol(base + rs, self.largo * EXTRA_PUNTA)
        ux, uy = p[0] / (self.largo * EXTRA_PUNTA), p[1] / (self.largo * EXTRA_PUNTA)
        ini = (self.S[0] - ux * ATRAS_HOMBRO * self.grosor, self.S[1] - uy * ATRAS_HOMBRO * self.grosor)
        return [(ini, (self.S[0] + p[0], self.S[1] + p[1]))]

    def oculta(self, rs, rc=0.0):
        """
        Fraccion de los pixeles del brazo que quedan tras el torso con esos giros. Sobre los pixeles del propio sprite (puntos
        girados con la cinematica directa y buscados en la silueta del torso: lo mismo que mide pose_preview.py); si no se
        pudieron leer, sobre un modelo de capsulas de ancho «grosor» en los ejes del brazo.
        """
        if self.silueta is None:
            return 0.0
        if self.pts_h:
            return self._oculta_sprite(rs, rc)
        return self._oculta_capsula(rs, rc)

    def _oculta_sprite(self, rs, rc):
        sil = self.silueta
        g = math.radians(rs)
        c, sn = math.cos(g), math.sin(g)
        sx, sy = self.S
        dentro = 0
        for dx, dy in self.pts_h:
            dentro += 1 if sil.dentro(sx + dx * c + dy * sn, sy - dx * sn + dy * c) else 0
        if self.pts_f and not self.antebrazo_delante:
            ex = sx + (self.E[0] - sx) * c + (self.E[1] - sy) * sn
            ey = sy - (self.E[0] - sx) * sn + (self.E[1] - sy) * c
            g2 = math.radians(rs + rc)
            c2, s2 = math.cos(g2), math.sin(g2)
            for dx, dy in self.pts_f:
                dentro += 1 if sil.dentro(ex + dx * c2 + dy * s2, ey - dx * s2 + dy * c2) else 0
            return dentro / float(len(self.pts_h) + len(self.pts_f))
        # INC-133: el antebrazo se dibuja delante del torso (y de la cabeza): nunca esta tapado, solo cuenta el humero
        return dentro / float(len(self.pts_h))

    def _oculta_capsula(self, rs, rc=0.0):
        """Fraccion del brazo (capsulas de ancho «grosor» sobre sus ejes) que cae dentro de la silueta del torso."""
        total = oculto = 0
        for p, q in self.segmentos(rs, rc):
            largo = math.hypot(q[0] - p[0], q[1] - p[1])
            if largo < 1e-6:
                continue
            n = max(2, int(largo / PASO_MUESTRA))
            nx, ny = -(q[1] - p[1]) / largo, (q[0] - p[0]) / largo
            for i in range(n + 1):
                cx, cy = p[0] + (q[0] - p[0]) * i / n, p[1] + (q[1] - p[1]) * i / n
                for k in ANCHOS_MUESTRA:
                    total += 1
                    if self.silueta.dentro(cx + nx * k * self.grosor, cy + ny * k * self.grosor):
                        oculto += 1
        return oculto / float(total) if total else 0.0

    def prepara(self, umbral=UMBRAL_OCULTO):
        """
        Tabla de lo oculto que queda el brazo con cada par de giros (hombro, codo; solo el hombro si es de una pieza), para
        poder pedir despues «la pose visible mas cercana» sin recalcular. Paso de 3 grados en el hombro y 5 en el codo.
        """
        if self.silueta is None or self.pts_h:
            self.umbral = umbral  # con los pixeles del sprite se mide lo mismo que la prueba: el umbral es el de la prueba
            if self.silueta is None:
                return
        else:
            self._umbral_capsulas(umbral)
        self._tabla()

    def _umbral_capsulas(self, umbral):
        # Lo que el modelo de capsulas puede ocultar: el margen que deja la cupula del hombro (base_cap, medida sobre el
        # sprite) mas lo que el propio modelo ya cuenta de ella con el brazo bien abierto (su minimo).
        base_modelo = min(self.oculta(self.hombro_rot(float(t)), 0.0) for t in range(40, 131, 15))
        self.umbral = max(0.02, umbral - self.base_cap + base_modelo)

    def _tabla(self):
        self.rs0, self.drs = -200.0, 3.0
        self.nrs = int(400.0 / self.drs) + 1
        if self.partido:
            self.rc0, self.drc = (-170.0, 8.0) if self.pts_h else (-170.0, 5.0)
            self.nrc = int(340.0 / self.drc) + 1
            if self.pts_h and self.antebrazo_delante:
                # INC-133: solo el humero puede quedar tapado, y no depende del codo: una columna por giro del hombro
                filas = [self.oculta(self.rs0 + i * self.drs, 0.0) for i in range(self.nrs)]
                self.tabla = [[v] * self.nrc for v in filas]
            else:
                self.tabla = [[self.oculta(self.rs0 + i * self.drs, self.rc0 + j * self.drc) for j in range(self.nrc)]
                              for i in range(self.nrs)]
        else:
            self.tabla = [self.oculta(self.rs0 + i * self.drs) for i in range(self.nrs)]

    def _celda(self, rs, rc):
        i = int(round((_norm(rs) - self.rs0) / self.drs))
        i = max(0, min(self.nrs - 1, i))
        if not self.partido:
            return i, 0
        j = max(0, min(self.nrc - 1, int(round((_norm(rc) - self.rc0) / self.drc))))
        return i, j

    def _oculto_celda(self, i, j):
        return self.tabla[i][j] if self.partido else self.tabla[i]

    def permite_tapar_cara(self, tapa):
        """
        El gesto que se escribe deja que la mano y el antebrazo tapen parte de la cara (CARA_TAPADA): el brazo ya no se desvia de la caja de la
        cara (pose() devuelve lo que el gesto pide) y la mano llega a la sien o a la frente (cabeza_objetivo). Santiago, 06/10/2026:
        «acepto que cubra el rostro». Los demas gestos siguen esquivandola.
        """
        self.evita = [] if tapa else list(self.evita_cara)
        if self.dos is not None:
            self.dos.evita = list(self.evita)

    def pon_delante(self, delante):
        """
        Los brazos de esta accion van delante del torso (CharacterRig.armsInFrontActions) o detras. Delante no hay nada
        que los tape: no hay restriccion de visibilidad y el gesto se resuelve donde lo pide (Strike, delante del pecho).
        """
        self.delante = bool(delante)
        if self.dos is not None:
            self.dos.delante = self.delante

    def visible(self, rs, rc=0.0, umbral=None):
        """
        El brazo con estos giros se ve (lo oculto no pasa del umbral; «umbral» pide otro, mas estricto: el del reposo). Sin silueta, o
        con los brazos delante del torso, siempre.
        """
        if self.delante or self.tabla is None:
            return True
        i, j = self._celda(rs, rc)
        return self._oculto_celda(i, j) <= (self.umbral if umbral is None else umbral)

    def repara(self, rs, rc=0.0):
        """
        (rs, rc) si ya se ven; si no, la pose visible mas cercana (el hombro pesa mas que el codo). Con un brazo de
        una pieza solo cambia el hombro. Cerca de la frontera de lo visible es continua; entre dos zonas visibles
        separadas por una oculta salta a la mas cercana (un gesto bien pensado no cruza esa franja).
        """
        rs, rc = _norm(rs), _norm(rc)
        if self.visible(rs, rc):
            return rs, rc
        i0, j0 = self._celda(rs, rc)
        mejor, quien = 1e18, None
        radio = 0
        while radio < max(self.nrs, getattr(self, "nrc", 0)):
            radio += 1
            if quien is not None and radio * min(self.drs, getattr(self, "drc", self.drs) * 0.5) > mejor:
                break
            for i in range(max(0, i0 - radio), min(self.nrs, i0 + radio + 1)):
                if not self.partido:
                    cand = [(i, 0)] if abs(i - i0) == radio else []
                else:
                    borde_i = abs(i - i0) == radio
                    cand = [(i, j) for j in range(max(0, j0 - radio), min(self.nrc, j0 + radio + 1))
                            if borde_i or abs(j - j0) == radio]
                for ci, cj in cand:
                    if self._oculto_celda(ci, cj) > self.umbral:
                        continue
                    d = math.hypot((ci - i0) * self.drs, ((cj - j0) * self.drc * 0.5) if self.partido else 0.0)
                    if d < mejor:
                        mejor, quien = d, (ci, cj)
        if quien is None:
            return rs, rc
        ci, cj = quien
        return self.rs0 + ci * self.drs, (self.rc0 + cj * self.drc) if self.partido else rc

    def _pide(self, theta, pliegue, signo=None):
        """(rs, rc) crudos de «el humero a theta grados de la vertical, el codo plegado pliegue»."""
        rs = self.hombro_rot(theta)
        if signo is None:
            rc = self.codo_rot(theta, pliegue)
        else:
            rc = signo * pliegue if self.lado == "Izq" else -signo * pliegue
        return rs, rc

    def pega_en_la_cara(self, rs, rc):
        """
        El brazo con esos giros pasa por la CAJA DE LA CARA (self.evita: la de ojos, cejas y boca con el 10 % de la prueba y un pelo mas). Solo con
        el brazo partido (humero y antebrazo): el eje del humero y el del antebrazo, este con un margen que crece hacia la mano (los dedos
        abren mas que el antebrazo: 0,25 del grosor en el codo y 0,6 en la punta).
        """
        if not self.evita or not self.partido:
            return False
        e, m = self.fk(rs, rc)
        tip = (e[0] + (m[0] - e[0]) * EXTRA_PUNTA, e[1] + (m[1] - e[1]) * EXTRA_PUNTA)
        for p, q, m0, m1 in ((self.S, e, 0.25, 0.25), (e, tip, 0.25, 0.60)):
            largo = math.hypot(q[0] - p[0], q[1] - p[1])
            n = max(2, int(largo / 12.0))
            for i in range(n + 1):
                f = i / float(n)
                pt = (p[0] + (q[0] - p[0]) * f, p[1] + (q[1] - p[1]) * f)
                if any(_dist_rect(pt, r) < (m0 + (m1 - m0) * f) * self.grosor for r in self.evita):
                    return True
        return False

    def pose(self, theta, pliegue, signo=None):
        """
        (rs, rc) de «el humero a theta grados de la vertical, el codo plegado pliegue» (con el sentido opcional), siempre visible y, con el
        brazo partido, SIN cruzar la caja de la cara: con una cabeza ancha y brazos cortos (el Nino, 06/10/2026: la cara registrada de la
        entrega llena el ovalo) un brazo en alto con la mano hacia la cabeza se queda sobre las mejillas, y una mano en la cara se lee como
        desesperacion (CP-02). Si el gesto que se pide la cruza se busca la pose mas cercana que no (primero bajando el humero: el brazo se
        abre hacia fuera, que es hacia donde se celebra, se martilla o se anima, y luego subiendolo), con el mismo sentido de pliegue.
        """
        rs, rc = self.repara(*self._pide(theta, pliegue, signo))
        if not self.pega_en_la_cara(rs, rc):
            return rs, rc
        for d in range(1, 121):
            for th in (theta - d, theta + d):
                for pl in (pliegue, 0.5 * pliegue, 0.0):
                    rs2, rc2 = self.repara(*self._pide(th, pl, signo))
                    if not self.pega_en_la_cara(rs2, rc2):
                        return rs2, rc2
        return rs, rc

    # --- directa
    def fk(self, rot_hombro, rot_codo):
        a1 = self.a1 + rot_hombro
        e = (self.S[0] + _pol(a1, self.l1)[0], self.S[1] + _pol(a1, self.l1)[1])
        a2 = self.a2 + rot_hombro + rot_codo
        m = (e[0] + _pol(a2, self.l2)[0], e[1] + _pol(a2, self.l2)[1])
        return e, m

    # --- inversa
    def ik(self, objetivo, codo_hacia=None, alcance=None, minimo=MIN_ALCANCE):
        """
        (rot_hombro, rot_codo) para que la mano llegue a «objetivo» (o lo mas cerca que alcance). De las dos
        posiciones posibles del codo elige la que queda mas del lado de «codo_hacia» (un vector del lienzo;
        por defecto hacia fuera y un poco abajo: el codo en el interior pasaria por delante de la cara) y, entre
        las dos, la que no cruza el eje del cuerpo. Con un brazo de una pieza el hombro lo apunta (apunta()). Si los
        brazos van detras del torso se prefiere la rama en que se ven (visible()); si van delante, esa restriccion no existe.
        «alcance» (solo con brazo de una pieza): el largo con que se dibuja el brazo (acortado); por defecto el entero.
        «minimo»: la fraccion del largo del brazo a la que la mano puede acercarse al hombro (MIN_ALCANCE: sin codo en horquilla).
        """
        if not self.partido:
            rs = self.apunta(objetivo, alcance)
            return rs, self.dos.ik(objetivo, codo_hacia, None, minimo)[1]
        dx, dy = objetivo[0] - self.S[0], objetivo[1] - self.S[1]
        d = max(abs(self.l1 - self.l2) + 1.0, minimo * (self.l1 + self.l2),
                min(self.l1 + self.l2 - 1.0, math.hypot(dx, dy)))
        base = _ang((dx, dy))
        cos_a = (self.l1 ** 2 + d ** 2 - self.l2 ** 2) / (2 * self.l1 * d)
        alfa = math.degrees(math.acos(clamp(cos_a, -1.0, 1.0)))
        mano = (self.S[0] + _pol(base, d)[0], self.S[1] + _pol(base, d)[1])  # el objetivo, acercado o alejado si no llega
        hacia = codo_hacia or (self.lado_sig, 0.5)  # por defecto, el codo hacia fuera y un poco abajo
        cand = []
        for signo in (1.0, -1.0):
            a1 = base + signo * alfa  # direccion del humero
            e = _pol(a1, self.l1)
            e = (self.S[0] + e[0], self.S[1] + e[1])
            coste = (e[0] - self.S[0]) * hacia[0] + (e[1] - self.S[1]) * hacia[1]
            # un codo no cruza el eje del cuerpo (los antebrazos se cruzarian en X). La cara NO cuenta aqui: con
            # un humero corto y la cabeza justo encima (el Nino) el codo hacia fuera pasa siempre cerca de ella,
            # y esquivarla cambiaria de rama a mitad del gesto (un salto de 120 grados en un cuadro): la cara se
            # cuida eligiendo bien los objetivos y la prueba la vigila
            mal = self.lado_sig * (e[0] - self.cx) < -4.0
            a2 = _ang((mano[0] - e[0], mano[1] - e[1]))
            rs, rc = _norm(a1 - self.a1), _norm((a2 - a1) - (self.a2 - self.a1))
            # y se prefiere la rama en que el brazo se ve (va tras el torso): la otra solo si ninguna se ve
            cand.append((1 if self.visible(rs, rc) else 0, 0 if mal else 1, coste, rs, rc))
        _, _, _, rs, rc = max(cand, key=lambda c: (c[0], c[1], c[2]))
        return self.repara(rs, rc)

    def _choca(self, p, q):
        """El segmento p-q (su eje, a un cuarto de grosor: la caja ya lleva su holgura) entra en alguna de las cajas que el brazo no debe cruzar."""
        if not self.evita:
            return False
        largo = math.hypot(q[0] - p[0], q[1] - p[1])
        n = max(2, int(largo / 12.0))
        for i in range(n + 1):
            pt = (p[0] + (q[0] - p[0]) * i / n, p[1] + (q[1] - p[1]) * i / n)
            if any(_dist_rect(pt, r) < 0.25 * self.grosor for r in self.evita):
                return True
        return False

    def escala_para(self, objetivo, minimo=ESC_MIN_BRAZO):
        """La escala (X e Y: el dibujo del brazo va en diagonal) del brazo de UNA pieza para que su mano llegue a «objetivo» (entre «minimo» y 1): un brazo rigido no se dobla, se encoge."""
        d = math.hypot(objetivo[0] - self.S[0], objetivo[1] - self.S[1])
        return clamp(d / self.largo, minimo, 1.0)

    def apunta(self, objetivo, alcance=None):
        """
        Rotacion del hombro de un brazo de UNA pieza: lo apunta hacia el objetivo (la mano queda a su alcance
        sobre esa recta, pasandose de largo si el objetivo esta mas cerca). Sin antebrazo no se puede doblar:
        lo que un brazo recto no puede hacer sin quedar mal se evita, no se finge. La direccion mas cercana a
        la del objetivo que (1) no lleve la mano mas alla del eje del cuerpo —se formaria una X—, (2) no meta
        el brazo en la cara y (3) deje el brazo VISIBLE (si va detras del torso: Brazo.visible; en las acciones con los brazos delante no hay nada que tape).
        """
        dx, dy = objetivo[0] - self.S[0], objetivo[1] - self.S[1]
        psi0 = _ang((dx, dy))
        base = _ang((self.M[0] - self.S[0], self.M[1] - self.S[1]))
        psi = psi0  # si ninguna direccion cumple, la del objetivo
        largo = alcance or self.largo
        for k in range(int(360.0 / PASO_APUNTA) + 1):
            for signo in ((1.0,) if k == 0 else (1.0, -1.0)):
                c = psi0 + signo * k * PASO_APUNTA
                p = _pol(c, largo)
                tip = (self.S[0] + p[0], self.S[1] + p[1])
                if self.lado_sig * (tip[0] - self.cx) < 0.5 * self.grosor * (largo / self.largo) or self._choca(self.S, tip):
                    continue
                if not self.visible(c - base):
                    continue
                return _norm(c - base)
        return _norm(psi - base)


def _brazos_de(rig, pid, arbol, piezas=None):
    """Construye los dos Brazo del personaje a partir del rig (hombro, codo), los rects (la mano) y el arbol (si hay antebrazo)."""
    p = P.personaje_rig(rig, pid)
    partes = {q["nombre"]: q for q in p["partes"]}
    nodos = {n["nombre"]: n for n in p["nodos"]}
    guia = P.PERSONAJES[pid][2]
    out = {}
    for lado in ("Izq", "Der"):
        codo = nodos["Codo" + lado]
        if guia:
            brazo = nodos["Brazo" + lado]
            hombro, rect = brazo["punto"], brazo["rect"]
        else:
            brazo = partes["Brazo" + lado]
            hombro, rect = brazo["pivote"], brazo["rect"]
        e = codo["punto"]
        ruta_ante = (LE if lado == "Izq" else RE) + "/Antebrazo" + lado
        nodo_ante = arbol.get(ruta_ante)
        # Algoritm siempre se trata como brazo partido (humero y antebrazo): desde el 08/10/2026 su maqueta (maqueta.piezas_guia) y su arte provisional ya
        # traen el antebrazo suelto, y la mano que prueba pose_preview.py (manos_modelo) tiene que seguir al codo.
        partido = bool(guia or (nodo_ante is not None and nodo_ante.dibuja()))
        r = codo["rect"]  # el antebrazo del arte final: la mano esta al 80 % de su largo desde el codo
        c = ((r[0] + r[2]) / 2.0, (r[1] + r[3]) / 2.0)
        m_dos = (e[0] + 1.6 * (c[0] - e[0]), e[1] + 1.6 * (c[1] - e[1]))
        # un solo sprite: la mano esta cerca de la esquina del rect mas lejana al hombro
        esq = max(((rect[0], rect[1]), (rect[2], rect[1]), (rect[0], rect[3]), (rect[2], rect[3])),
                  key=lambda q: (q[0] - hombro[0]) ** 2 + (q[1] - hombro[1]) ** 2)
        m_uno = (hombro[0] + 0.9 * (esq[0] - hombro[0]), hombro[1] + 0.9 * (esq[1] - hombro[1]))
        out[lado] = Brazo(lado, tuple(hombro), tuple(e), m_dos if partido else m_uno, partido, m_dos)
    return out


def _rects_json(p):
    """{ruta del nodo con Image: rect} de una entrada del rig: lo que el sprite mide en el lienzo (el prefab del disco puede ir atras)."""
    out = {q["ruta"]: q["rect"] for q in p["partes"]}
    for n in p["nodos"]:
        if n.get("rect"):
            out[n["padre"] + "/" + n["nombre"] + ("/" + n["imagen"] if n["tipo"] == "articulacion" else "")] = n["rect"]
    return out


def _imagen_de(arbol, piezas, ruta, rects):
    """(imagen RGBA, rect del JSON) del nodo: la pieza de la maqueta, o el PNG de su Image."""
    import maqueta as M
    im, rect = M.pieza_o_png(arbol, piezas or {}, ruta)
    if im is None:
        return None, None
    if piezas and ruta in piezas:
        return im, rect  # la pieza de la maqueta trae su propio rect (el del sprite entero recortado)
    return im, rects.get(ruta) or rect


def _puntos_brazo(b, arbol, piezas, rects, paso=6):
    """Pixeles opacos del sprite del brazo (cada `paso` px): ([relativos al hombro], [del antebrazo, relativos al codo])."""
    def de(ruta, ref):
        im, rect = _imagen_de(arbol, piezas, ruta, rects)
        if im is None:
            return []
        a = im.getchannel("A")
        w, h = a.size
        d = a.load()
        kx, ky = (rect[2] - rect[0]) / float(w), (rect[3] - rect[1]) / float(h)
        return [(rect[0] + x * kx - ref[0], rect[1] + y * ky - ref[1]) for y in range(0, h, max(1, int(round(paso / ky))))
                for x in range(0, w, max(1, int(round(paso / kx)))) if d[x, y] > 127]
    pts_h = de(LA if b.lado == "Izq" else RA, b.S)
    pts_f = de((LE if b.lado == "Izq" else RE) + "/Antebrazo" + b.lado, b.E) if b.partido else []
    return pts_h, pts_f


def _base_cap(b, arbol, piezas, rects, silueta):
    """
    Fraccion de los pixeles del brazo que SIEMPRE quedan tras el torso: la cupula del hombro, porque el pivote cae dentro
    de la silueta. Se mide sobre el sprite real (el modelo de capsulas no la ve) y es el MINIMO sobre varias aperturas
    del brazo (de 40 a 130 grados con la vertical): donde el brazo ya no pisa el torso, lo oculto es solo la cupula.
    """
    rutas = [(LA if b.lado == "Izq" else RA)]
    if b.partido:
        rutas.append((LE if b.lado == "Izq" else RE) + "/Antebrazo" + b.lado)
    puntos = []
    for ruta in rutas:
        im, rect = _imagen_de(arbol, piezas, ruta, rects)
        if im is None:
            continue
        a = im.getchannel("A")
        w, h = a.size
        d = a.load()
        kx, ky = (rect[2] - rect[0]) / float(w), (rect[3] - rect[1]) / float(h)
        puntos += [(rect[0] + x * kx - b.S[0], rect[1] + y * ky - b.S[1]) for y in range(0, h, 4) for x in range(0, w, 4)
                   if d[x, y] > 127]
    if not puntos:
        return 0.0
    minimo = 1.0
    for theta in range(40, 131, 15):
        giro = math.radians(b.hombro_rot(float(theta)))
        c, sn = math.cos(giro), math.sin(giro)
        dentro = sum(1 for dx, dy in puntos if silueta.dentro(b.S[0] + dx * c + dy * sn, b.S[1] - dx * sn + dy * c))
        minimo = min(minimo, dentro / float(len(puntos)))
    return minimo


def _grosor_brazo(b, arbol, piezas, rects=None):
    """Grosor medio del brazo (px): el area opaca de sus sprites entre su largo. Sin Pillow, un decimo del largo."""
    try:
        import maqueta as M
        rutas = [(LA if b.lado == "Izq" else RA)]
        if b.partido:
            rutas.append((LE if b.lado == "Izq" else RE) + "/Antebrazo" + b.lado)
        area = 0.0
        for ruta in rutas:
            im, rect = _imagen_de(arbol, piezas, ruta, rects or {})
            if im is None:
                continue
            a = im.getchannel("A")
            n = sum(a.histogram()[128:])
            area += n * ((rect[2] - rect[0]) / a.size[0]) * ((rect[3] - rect[1]) / a.size[1])
        largo = (b.l1 + b.l2) if b.partido else b.largo
        return area / largo if area and largo else 0.1 * largo
    except ImportError:  # pragma: no cover
        return 0.1 * (b.l1 + b.l2)


# Reposo de cada personaje: el MENOR angulo con la vertical con que cuelgan sus brazos (grados; 0 = pegados al cuerpo) y cuanto se pliegan los
# codos hacia dentro. El angulo que se usa es el mayor entre este y el que deja el humero a la vista (reposo_visible). Con el hombro en el hombro
# (hombro.py, 06/10/2026) los brazos cuelgan relajados, entre 12 y 25 grados, y no abiertos en A: un adulto no los pega al cuerpo ni los abre.
# Papa, de pecho ancho, los lleva un poco separados; la Nina, ligera, casi pegados; Mama y el Nino los llevan donde los deja ver su torso.
PERFIL = {
    "papa": (15.0, 6.0),
    "mama": (14.0, 10.0),
    "nina": (13.0, 12.0),
    "nino": (16.0, 12.0),
}


def reposo_visible(x, pliegue, minimo=0.0):
    """
    El reposo de los brazos sale de la geometria: el menor angulo con la vertical (desde 10 grados, con un par de
    grados de holgura) con que los DOS brazos se ven (el humero, tras el torso: UMBRAL_OCULTO_REPOSO; el antebrazo va delante), y nunca
    menos de «minimo» (el perfil del personaje). Si el codo plegado del perfil esconde el antebrazo, se afloja a la mitad y luego a cero.
    Con un torso estrecho (Mama, Nina) los brazos cuelgan casi pegados; con uno ancho (Papa, el taparrabos del Nino) quedan mas abiertos:
    lo que dibuja el arte.
    """
    for pl in (pliegue, 0.5 * pliegue, 0.0):
        for th in range(10, 90):
            if all(b.visible(b.hombro_rot(th), b.codo_rot(th, pl), UMBRAL_OCULTO_REPOSO) for b in x.brazos.values()):
                return max(float(th) + 2.0, minimo), pl
    return 60.0, 0.0


def leer_contexto(pid, rig=None, arbol=None, piezas=None):
    """
    El contexto de la coreografia vigente. Como leer_contexto_prefab, pero la geometria sale del JSON del
    rig (lo que el C# aplicara a los prefabs), no del prefab de hoy, que va un paso por detras: la rodilla
    del Nino estaba en el borde del muslo y ahora esta en el centro de la articulacion. Que haya antebrazo,
    antepierna o cabeza propia lo dice el ARBOL (por defecto el del prefab; la maqueta de pose_preview.py
    pasa el suyo, con esas piezas encendidas, y «piezas» con sus imagenes).
    """
    rig = rig or P.cargar_rig()
    arbol = arbol if arbol is not None else P.arbol_vigente(pid, rig)
    piezas = piezas or {}
    guia = P.PERSONAJES[pid][2]
    p = P.personaje_rig(rig, pid)
    nodos = {n["nombre"]: n for n in p["nodos"]}
    partes = {q["nombre"]: q for q in p["partes"]}
    x = Ctx(pid, guia=guia)
    if not guia and p.get("perfil"):
        x.perfil = GeoPerfil(p)   # la figura de perfil: longitudes y puntos del JSON, no del prefab (que aun no tiene Lienzo/Perfil)
    x.delante, x.delante_hallada = P.acciones_brazos_delante()
    x.brazos = _brazos_de(rig, pid, arbol, piezas)
    x.hombro_y = x.brazos["Izq"].S[1]
    x.alcance = x.brazos["Izq"].l1 + x.brazos["Izq"].l2
    x.cx = 0.5 * (x.brazos["Izq"].S[0] + x.brazos["Der"].S[0])
    alto = P.SUELO - x.hombro_y  # del hombro al suelo
    b = x.brazos["Izq"]
    semi = abs(b.S[0] - x.cx)
    # Las manos solo se juntan comodamente a cierta distancia del hombro: con humero y antebrazo cortos (el Nino)
    # el pecho de verdad queda tan cerca del hombro que el codo se doblaria en horquilla. «pecho» es la altura
    # del eje a la que la mano queda al menos al 45 % del largo del brazo (el del modelo de dos segmentos, parta
    # o no el arte de hoy: el pecho no debe cambiar cuando llegue el antebrazo).
    v_min = math.sqrt(max((MIN_ALCANCE * (b.l1 + b.l2)) ** 2 - semi ** 2, 0.0))
    x.y_pecho = x.hombro_y + max(0.12 * alto, v_min)
    x.y_vientre = x.y_pecho + 0.14 * alto
    x.y_cadera = x.y_vientre + 0.10 * alto
    cadera_json = next((q["pivote"][1] for q in p["partes"] if q["nombre"] == "PiernaIzq"), x.y_cadera + MARGEN_CINTURA)
    x.y_cintura = min(x.y_cadera, cadera_json - MARGEN_CINTURA)
    # la caja de la cara: Ojos y Boca (no CaraBase). Con los sprites de la cara puestos, el rect de cada capa es el del lienzo de
    # la cara entera: la caja es la de lo que se pinta (pixeles opacos), no la del margen transparente
    zona = []
    for n in ("Ojos", "Boca"):
        if n in nodos and nodos[n].get("rect"):
            png = P._png_de(p["carpeta"], nodos[n]["sprite"]) if nodos[n].get("sprite") else None
            zona.append(list(P.caja_contenido(png, nodos[n]["rect"])))
    if zona:
        x.cara = (min(r[0] for r in zona), min(r[1] for r in zona), max(r[2] for r in zona), max(r[3] for r in zona))
    rects = _rects_json(p)
    # lo que tapa a los brazos: el torso (con la cabeza dentro, mientras sea arte provisional)
    im_torso, rect_torso = _imagen_de(arbol, piezas, T + "/Torso", rects)
    # INC-133: lo que se dibuja delante del humero es el torso y, si «orden_tronco» pone el Cuello despues de los brazos (Papa, 06/10/2026:
    # la barba y la cara delante del torso), tambien la cabeza; el antebrazo, que sale de su codo, va delante de todo
    nombres_dibujo = [h.nombre for h in P.hijos_de_dibujo(arbol[T])]
    cabeza_delante = bool("Cuello" in nombres_dibujo and "BrazoIzq" in nombres_dibujo
                          and nombres_dibujo.index("Cuello") > nombres_dibujo.index("BrazoIzq")
                          and arbol.get(HD) is not None and arbol[HD].dibuja())
    otras = []
    if cabeza_delante:
        im_cab, rect_cab = _imagen_de(arbol, piezas, HD, rects)
        if im_cab is not None:
            otras.append((im_cab, rect_cab))
    antebrazo_delante = any(getattr(arbol.get(r), "sigue", False) for r in (LE + "/AntebrazoIzq", RE + "/AntebrazoDer"))
    silueta = Silueta(im_torso, rect_torso, otras) if im_torso is not None and not guia else None
    for lado, br in x.brazos.items():
        br.cx = x.cx
        br.antebrazo_delante = antebrazo_delante
        br.grosor = _grosor_brazo(br, arbol, piezas, rects)
        br.silueta = silueta
        if silueta is not None:
            br.pts_h, br.pts_f = _puntos_brazo(br, arbol, piezas, rects)
            if not br.pts_h:
                br.base_cap = _base_cap(br, arbol, piezas, rects, silueta)
        if x.cara:
            c = x.cara
            mx, my = (c[2] - c[0]) * 0.55, (c[3] - c[1]) * 0.55  # la caja de la cara con el 10 % de la prueba, y un pelo mas
            ccx, ccy = (c[0] + c[2]) / 2.0, (c[1] + c[3]) / 2.0
            br.evita = [(ccx - mx - 4, ccy - my - 4, ccx + mx + 4, ccy + my + 4)]
            br.evita_cara = list(br.evita)
            if br.dos:
                br.dos.cx, br.dos.grosor, br.dos.evita = br.cx, br.grosor, br.evita
                br.dos.evita_cara = list(br.evita)
    x.silueta = silueta
    if x.cara:
        c = x.cara
        # La frente: un poco por encima de ojos y cejas (los brazos pueden pasar por delante de la cabeza pero no tapar
        # ojos ni boca), y como mucho a donde llega la mano con el brazo casi estirado.
        alcanzable = x.hombro_y - math.sqrt(max((0.93 * x.alcance) ** 2 - semi ** 2, 0.0))
        x.y_frente = max(c[1] - 0.35 * (c[3] - c[1]), alcanzable)
    if guia:
        return x
    info = FAMILIA[pid]
    x.k, x.a = info.tempo, info.amp
    x.segmentado = P.esta_segmentado(arbol)
    x.cabeza_propia = bool(arbol.get(HD) is not None and arbol[HD].dibuja())
    x.hang, x.pliegue = PERFIL[pid]
    for br in x.brazos.values():
        br.prepara()
    x.hang, x.pliegue = reposo_visible(x, x.pliegue, x.hang)
    x.codo = x.pliegue
    cadera = partes["PiernaIzq"]["pivote"][1]
    x.pie_x = {"Izq": float(partes["PiernaIzq"]["pivote"][0]), "Der": float(partes["PiernaDer"]["pivote"][0])}
    for lado in ("Izq", "Der"):
        if x.segmentado:
            ruta_pieza = (LK if lado == "Izq" else RK) + "/Antepierna" + lado
            r = nodos["Rodilla" + lado]["rect"]
            x.pie_piv[lado] = (float(nodos["Rodilla" + lado]["punto"][0]), float(nodos["Rodilla" + lado]["punto"][1]))
        else:
            ruta_pieza = LL if lado == "Izq" else RL
            r = partes["Pierna" + lado]["rect"]
            x.pie_piv[lado] = (float(partes["Pierna" + lado]["pivote"][0]), float(partes["Pierna" + lado]["pivote"][1]))
        px, py = x.pie_piv[lado]
        if ruta_pieza in piezas:
            casco = P.casco_imagen(piezas[ruta_pieza].imagen, piezas[ruta_pieza].rect)
        else:
            nodo = arbol.get(ruta_pieza)
            png = P.sprite_por_guid(nodo.imagen["guid"]) if nodo is not None and nodo.imagen and nodo.imagen["guid"] else None
            casco = P.casco_sprite(png, r)
        x.pie_casco[lado] = [(a - px, b - py) for a, b in casco]
        if x.segmentado:
            # el muslo termina en una rotula redonda: baja por debajo del pivote de la rodilla lo que mide su cierre convexo
            ruta_muslo = LL if lado == "Izq" else RL
            if ruta_muslo in piezas:
                cm = P.casco_imagen(piezas[ruta_muslo].imagen, piezas[ruta_muslo].rect)
            else:
                nm = arbol.get(ruta_muslo)
                pm = P.sprite_por_guid(nm.imagen["guid"]) if nm is not None and nm.imagen and nm.imagen["guid"] else None
                cm = P.casco_sprite(pm, partes["Pierna" + lado]["rect"])
            x.muslo_bajo[lado] = max(0.0, max(b for _, b in cm) - py)
        cad, rod = partes["Pierna" + lado]["pivote"][1], nodos["Rodilla" + lado]["punto"][1]
        largo = max(P.SUELO - cad, 1.0)
        mus = clamp(rod - cad, 0.25 * largo, 0.75 * largo)
        x.pata[lado] = (largo, mus, largo - mus)
    suelo = arbol[C].pivote[1]
    rodilla = nodos["RodillaIzq"]["punto"][1]
    x.pierna = max(suelo - cadera, 1.0)
    x.muslo = clamp(rodilla - cadera, 0.25 * x.pierna, 0.75 * x.pierna)
    x.canilla = x.pierna - x.muslo
    return x


# ---------------------------------------------------------------------------- la familia: reposo y utilidades


def reposo_familia(x, brazo=0.0, codo=None):
    """
    Pose de reposo de cada hueso (los clips SUMAN sus valores a esta pose). «brazo» es lo que el hombro
    se cierra contra el cuerpo respecto al A-pose del prefab (+ en BrazoIzq, - en BrazoDer) y «codo» la
    flexion de reposo de los codos (hacia arriba); con brazo=0 y codo=x.codo es el reposo de antes.
    """
    codo = x.codo if codo is None else codo
    return {C: 0.0, T: 0.0, LA: brazo, RA: -brazo, LE: -codo, RE: codo,
            LL: 0.0, RL: 0.0, LK: 0.0, RK: 0.0, NK: 0.0, HD: 0.0}


def nuevo(x, accion, archivo, largo_base, bucle=True, reposo=None):
    r = reposo if reposo is not None else reposo_familia(x)
    return Spec(accion, "char_%s_anim_%s" % (x.id, archivo), largo_base, x.k, bucle, r, HUESOS_FAMILIA)


def cabeza_sigue(s, src, src_prop, ganancia_cuello, ganancia_cabeza, sesgo=0.0):
    """La cabeza sigue al cuerpo con 2 y 4 cuadros de retraso: el movimiento secundario."""
    s.follow(src, src_prop, NK, 2, ganancia_cuello, sesgo)
    s.follow(src, src_prop, HD, 4, ganancia_cabeza, sesgo)


def agacharse(x, s, profundidad_frac, muslo_plano, muslo_seg, razon_rodilla, *tw):
    """
    Agacharse a una profundidad. Devuelve la bajada efectiva del tronco (unidades del lienzo) para que
    el clip baje Tronco lo mismo. «tw» son pares (tiempo base, peso 0..1).
    No segmentado: las piernas bajan y se acortan con escala Y (el truco de BuildRigs), con el muslo
    abierto «muslo_plano» grados. Segmentado: el muslo se abre «muslo_seg» grados, la pantorrilla vuelve
    hacia dentro «razon_rodilla» veces eso y se acorta con escala Y de la rodilla lo justo para que el
    pie siga en el suelo; si hiciera falta menos de 0,4 se baja menos. Con piernas partidas ningun clip
    escala las piernas (solo la rodilla, que acorta la antepierna: es la rodilla en profundidad).
    """
    minimo = 0.4
    d = profundidad_frac * x.pierna
    if x.segmentado:
        a = math.radians(muslo_seg)
        neto = math.radians(muslo_seg - muslo_seg * razon_rodilla)
        d = max(0.0, min(d, x.pierna - x.muslo * math.cos(a) - minimo * x.canilla * math.cos(neto)))
    for i in range(0, len(tw) - 1, 2):
        t, w = tw[i], tw[i + 1]
        dd = d * w
        s.raw(LL, POSY, t, -dd)
        s.raw(RL, POSY, t, -dd)
        if not x.segmentado:
            razon = (x.pierna - dd) / x.pierna
            s.raw(LL, ESCY, t, razon)
            s.raw(RL, ESCY, t, razon)
            if muslo_plano > 0.0:
                s.sym(LL, RL, t, muslo_plano * w)
        else:
            abre = muslo_seg * w
            atras = abre * razon_rodilla
            cos_abre = math.cos(math.radians(abre))
            cos_neto = math.cos(math.radians(abre - atras))
            canilla = clamp((x.pierna - dd - x.muslo * cos_abre) / (x.canilla * cos_neto), minimo, 1.0)
            s.sym(LL, RL, t, abre)
            s.sym(LK, RK, t, -atras)
            s.raw(LK, ESCY, t, canilla)
            s.raw(RK, ESCY, t, canilla)
    return d


# ---------------------------------------------------------------------------- Algoritm: utilidades


def reposo_guia(brazo=0.0):
    r = {h: 0.0 for h in HUESOS_GUIA}
    r[LA], r[RA] = brazo, -brazo
    return r


def guia_spec(accion, archivo, largo, bucle=True, reposo=None):
    return Spec(accion, "char_algoritm_anim_" + archivo, largo, 1.0, bucle, reposo if reposo is not None else reposo_guia(), HUESOS_GUIA)


def cuelga(s, media):
    """
    Piernas que cuelgan: siguen la altura del cuerpo con 3 cuadros de retraso (juntas arriba, abiertas
    abajo) y las rodillas devuelven lo que abre el muslo; los brazos suben un poco con 4 cuadros de
    retraso, los codos siguen al brazo y el tronco, al giro del cuerpo. «media» es la altura media de
    la flotacion.
    """
    cuelga_piernas(s, media)
    s.follow(C, POSY, LA, 4, -0.4, media)
    s.follow(C, POSY, RA, 4, 0.4, media)
    s.follow(LA, ROT, LE, 3, 0.8)
    s.follow(RA, ROT, RE, 3, 0.8)
    s.follow(C, ROT, T, 2, 0.4)


def cuelga_piernas(s, media):
    s.follow(C, POSY, LL, 3, 0.5, media)
    s.follow(C, POSY, RL, 3, -0.5, media)
    s.follow(LL, ROT, LK, 3, -0.8)
    s.follow(RL, ROT, RK, 3, -0.8)


# ============================================================================ 2. LA COREOGRAFIA vigente
#
# POSTURA DE REPOSO. El prefab dibuja el A-pose (brazos casi horizontales). El reposo sale de la geometria
# (reposo_visible): el menor angulo con la vertical con que los brazos se ven enteros; el peso cargado sobre una
# pierna se ve en el balanceo del Idle.
#
# LOS BRAZOS VAN DETRAS DEL TORSO Y DELANTE DE LA CABEZA (orden_tronco de la familia, decision de Santiago del 05/10/2026),
# salvo en las acciones de CharacterRig.armsInFrontActions (hoy Strike), donde el motor los pasa DELANTE del torso: ahi no hay
# restriccion de visibilidad (Brazo.pon_delante, que familia_spec fija accion por accion). Detras, se ven: la prueba de pose_preview.py exige >= 85 % de cada brazo visible. Cada Brazo conoce la silueta del torso
# (Silueta) y los pixeles de su sprite, y Brazo.repara lleva cualquier pose pedida a la pose visible mas cercana
# (Brazo.pose para los gestos por angulo, Brazo.ik y Brazo.apunta para los de «la mano va aqui»). Asi lo derivan de la
# geometria el arte de hoy (la cabeza va pintada dentro del torso: hay que abrir mas los brazos) y el arte final. Los
# gestos se piden donde SE VEN —fuera(): la mano que descansa en el costado—, y el que necesita las manos delante del
# pecho (Strike) es de los que van delante, no una excepcion. Los brazos SI pasan por delante de la cabeza,
# pero no tapan ojos ni boca, y el torso (dibujado despues) tampoco debe tapar la cara al inclinarse la cabeza.
#
# LA COREOGRAFIA SE ESCRIBE UNA VEZ, PARA EL ARTE FINAL, Y SE ADAPTA A LO QUE LEE. Los gestos se piden como
# «la mano va AQUI» (manos(), cinematica inversa) o como angulos respecto a la vertical (brazos()); las
# medidas salen de la geometria (JSON del rig + prefab) y nada depende de «quien es provisional»:
#   - brazo con antebrazo (Image con sprite, encendida): dos segmentos, el codo se dobla de verdad;
#   - brazo de una pieza: el hombro apunta la mano al objetivo, sin pasar del eje del cuerpo ni cruzar la
#     cara (Brazo.apunta), dejandose ver, y el codo (un pivote vacio) lleva igual la curva que le toca con antebrazo;
#   - piernas partidas: la rodilla escala la antepierna; de una pieza, la pierna entera;
#   - cabeza propia: el cuello gira y la cabeza llega tarde; si no, el torso se inclina un poco menos.
# Cuando llega el arte final: se meten las piezas, se corrigen rect y punto en el JSON (articulaciones.py),
# se corre este script y pose_preview.py (--hoy ya es el arte final). La coreografia no se toca.

TAU = 2.0 * math.pi


def suave(t, a, b):
    """Escalon suave (smoothstep) de 0 en a a 1 en b."""
    if t <= a:
        return 0.0
    if t >= b:
        return 1.0
    u = (t - a) / (b - a)
    return u * u * (3.0 - 2.0 * u)


def pulso(t, t0, t1, t2, t3):
    """0 antes de t0, sube suave hasta 1 en t1, se mantiene hasta t2 y baja suave hasta 0 en t3."""
    return suave(t, t0, t1) * (1.0 - suave(t, t2, t3))


def bump(t, c, w):
    """Una campana de coseno de ancho 2w centrada en c (altura 1)."""
    return 0.5 * (1.0 + math.cos(math.pi * (t - c) / w)) if abs(t - c) < w else 0.0


def pendulo(t, t0, amp, periodo, vida):
    """Oscilacion amortiguada que arranca en t0 (sin salto: sube con una rampa corta)."""
    if t <= t0:
        return 0.0
    u = t - t0
    return amp * math.exp(-u / vida) * math.sin(TAU * u / periodo) * suave(u, 0.0, 0.08) * (1.0 - suave(u, 1.5 * vida * 2, 2.5 * vida * 2))


def muestrea(s, ruta, prop, f, largo, paso=0.1):
    """Escribe la curva (ruta, prop) muestreando f(t) en t base cada «paso»; f debe cerrar en el bucle."""
    n = max(1, int(round(largo / paso)))
    tv = []
    for i in range(n + 1):
        t = min(largo, i * largo / n)
        tv += [t, f(t)]
    s.raw(ruta, prop, *tv)


def reposo_nuevo(x):
    """Pose de reposo de cada hueso: brazos colgando «hang» grados con los codos algo plegados."""
    r = {C: 0.0, T: 0.0, LL: 0.0, RL: 0.0, LK: 0.0, RK: 0.0, NK: 0.0, HD: 0.0}
    for lado, hom, cod in (("Izq", LA, LE), ("Der", RA, RE)):
        b = x.brazos[lado]
        r[hom], r[cod] = b.pose(x.hang, x.pliegue)
    r.update(reposo_perfil(x))
    return r


def reposo_perfil(x):
    """
    Pose de reposo de las 12 articulaciones del cuerpo de perfil (INC-134): de pie, los brazos colgando rectos y los codos con la flexion de reposo del
    personaje (FAMILIA[...].codo, hacia delante: + en perfil mirando a la derecha). Es lo que llevan, constante, los clips de las acciones de frente.
    """
    r = {h: 0.0 for h in HUESOS_PERFIL}
    r[PEL] = r[PEC] = FAMILIA[x.id].codo
    return r


# Los gestos junto a la cabeza que dejan tapar parte de la cara (Santiago, 06/10/2026: «acepto que cubra el rostro»). Antes la mano se desviaba a un
# lado de la cara (pose() y una funcion que bajaba el objetivo esquivaban su caja) y con una cabeza ancha y brazos cortos el gesto no llegaba a la sien. Ahora la
# mano llega de verdad a la sien o a la frente, o lo mas cerca que alcance, y la mano y el antebrazo pueden quedar delante de parte de la cara.
# La clave es (personaje o «*», accion); pose_preview.EXCEPCIONES_CARA_TAPADA tiene las MISMAS claves (lo vigila) y, para cada una, hasta donde
# puede bajar la cara a la vista. Todo lo demas sigue esquivando la cara (pose_preview exige el 90 %). Una mano en la cara se leia como
# desesperacion (CP-02): por eso es una lista cerrada de gestos de alegria, animo y vigilancia, no un cambio general, y nunca se tapan los dos ojos a
# la vez (pose_preview, «ojos»).
CARA_TAPADA = {
    ("*", "Observe"): "visera: la mano va a la frente o a la sien para mirar a lo lejos",
    ("*", "Celebrate"): "los brazos se abren en V a los lados de la cabeza: con una cabeza ancha el antebrazo pasa por delante de la mejilla",
    ("*", "Hammer"): "el brazo del martillo sube junto a la cabeza y el antebrazo cruza el borde de la cara",
    ("*", "Encourage"): "el puño del animo sube junto a la cabeza, por encima del hombro",
    ("*", "Carry"): "las manos sostienen la carga a los lados de la cabeza",
    ("nino", "Idle"): "se rasca el costado de la cabeza: la mano llega a la sien y la mano y el antebrazo cruzan el borde de la cara",
}


def tapa_cara(pid, accion):
    """El gesto de esa accion deja que la mano y el antebrazo tapen parte de la cara (CARA_TAPADA)."""
    return (pid, accion) in CARA_TAPADA or ("*", accion) in CARA_TAPADA


def familia_spec(x, accion, archivo, largo_base, bucle=True):
    # el solucionador sabe, accion por accion, si los brazos van delante del torso o detras (la lista de CharacterRig.cs): de
    # ello depende si un gesto debe verse al pasar tras el torso o no; el clip lo escribe despues de esta llamada
    tapa = tapa_cara(x.id, accion)
    for b in x.brazos.values():
        b.pon_delante(accion in x.delante)
        b.permite_tapar_cara(tapa)
    return Spec(accion, "char_%s_anim_%s" % (x.id, archivo), largo_base, x.k, bucle, reposo_nuevo(x), HUESOS_FAMILIA_COMPLETA)


def brazos(x, s, izq=None, der=None):
    """Brazos por angulo: listas de (t, theta, pliegue). theta: 0 colgando, 90 horizontal, 180 en alto."""
    for lado, claves, hom, cod in (("Izq", izq, LA, LE), ("Der", der, RA, RE)):
        if not claves:
            continue
        b = x.brazos[lado]
        for c in claves:
            rs, rc = b.pose(c[1], c[2])
            s.raw(hom, ROT, c[0], rs)
            s.raw(cod, ROT, c[0], rc)


def brazo_f(x, s, lado, f_theta, f_pliegue, largo, paso=0.1):
    """Un brazo muestreado: theta(t) y pliegue(t) como funciones del tiempo base."""
    b = x.brazos[lado]
    hom, cod = (LA, LE) if lado == "Izq" else (RA, RE)
    muestrea(s, hom, ROT, lambda t: b.pose(f_theta(t), f_pliegue(t))[0], largo, paso)
    muestrea(s, cod, ROT, lambda t: b.pose(f_theta(t), f_pliegue(t))[1], largo, paso)


def brazo_mix(x, s, lado, f_theta, f_pliegue, w, objetivo, largo, hacia=None, paso=0.1, suaviza=0.0):
    """
    Un brazo que mezcla dos modos: por angulo (theta(t), pliegue(t)) con peso 1 - w(t) y por objetivo (la
    mano va a objetivo(t), cinematica inversa) con peso w(t). Mezcla las rotaciones del hombro y del codo.
    «suaviza» (s base) promedia las rotaciones en esa ventana, cerrando el bucle: cerca del brazo estirado el
    codo de la cinematica inversa cambia muy deprisa para lo que se mueve la mano, y la ventana lo reparte
    (la mano se aparta unos pixeles del camino solo en el transito).
    """
    b = x.brazos[lado]
    hom, cod = (LA, LE) if lado == "Izq" else (RA, RE)
    previo = [None, None]
    th_h, th_c, ik_h, ik_c = [], [], [], []
    for t in _rejilla(largo, paso):
        th = f_theta(t)
        pos = objetivo(t) if callable(objetivo) else objetivo
        rs, rc = b.ik(pos, hacia)
        rs, rc = _desenrolla(previo[0], rs), _desenrolla(previo[1], rc)
        previo = [rs, rc]
        wt = w(t)
        r0h, r0c = b.pose(th, f_pliegue(t))
        # el modo por angulo puede quedar a una vuelta del IK: se acerca a la rama del IK
        r0h = rs + _norm(r0h - rs) if wt > 0 else r0h
        th_h += [t, (1 - wt) * r0h + wt * rs]
        th_c += [t, (1 - wt) * r0c + wt * rc]
    if suaviza > 0:
        th_h, th_c = _promedia(th_h, suaviza, paso), _promedia(th_c, suaviza, paso)
    s.raw(hom, ROT, *th_h)
    s.raw(cod, ROT, *th_c)


def _promedia(tv, ventana, paso):
    """Promedio movil triangular (de media ventana «ventana» s) de una lista t, v, t, v... de un bucle (primero = ultimo)."""
    t = tv[0::2]
    v = tv[1::2][:-1]
    n = len(v)
    m = max(1, int(round(ventana / paso)))
    pesos = [m + 1 - abs(i) for i in range(-m, m + 1)]
    suma = float(sum(pesos))
    out = []
    for i in range(n):
        out.append(sum(pesos[j + m] * v[(i + j) % n] for j in range(-m, m + 1)) / suma)
    out.append(out[0])
    res = []
    for ti, vi in zip(t, out):
        res += [ti, vi]
    return res


def _desenrolla(previo, v):
    return v if previo is None else v + 360.0 * round((previo - v) / 360.0)


def _rejilla(largo, paso=0.1):
    n = max(1, int(round(largo / paso)))
    return [min(largo, i * largo / n) for i in range(n + 1)]


PASO_MANOS = 0.04  # s base entre dos muestras de la cinematica inversa de manos()


def manos(x, s, izq=None, der=None, acorta=None, minimo=MIN_ALCANCE):
    """
    Brazos por objetivo: listas de (t, (px, py)[, codo_hacia]) con el punto del lienzo adonde va la mano.
    El codo se dobla en el sentido natural salvo que se pida otra direccion. La MANO sigue el camino que
    dicen los puntos (una curva ClampedAuto por eje, como la que dibuja Unity) y la cinematica inversa se
    resuelve cada PASO_MANOS: interpolar los ANGULOS entre dos poses lejanas barre la mano por donde sea
    (con las manos que van de los brazos abiertos al pecho, por delante de la barbilla).
    «acorta» (una fraccion minima de escala, ESC_MIN_BRAZO): con un brazo de UNA pieza, que no se dobla, la mano solo llega a
    los puntos a un largo de brazo del hombro; para llevarla mas cerca el brazo se ENCOGE (escala X e Y de su nodo, como si
    apuntara hacia el espectador) hasta esa fraccion. Solo en Y se aplastaria: el dibujo del brazo va en diagonal. Con brazo
    partido no hace nada: el codo se dobla. «minimo»: lo mas cerca del hombro que puede quedar la mano, en fracciones del
    alcance (Brazo.ik).
    """
    for lado, claves, hom, cod in (("Izq", izq, LA, LE), ("Der", der, RA, RE)):
        if not claves:
            continue
        b = x.brazos[lado]
        t0, t1 = claves[0][0], claves[-1][0]
        cx = CurvaAuto([[c[0], c[1][0]] for c in claves]) if len(claves) > 1 else None
        cy = CurvaAuto([[c[0], c[1][1]] for c in claves]) if len(claves) > 1 else None
        n = max(1, int(round((t1 - t0) / PASO_MANOS)))
        tiempos = sorted({c[0] for c in claves} | {t0 + (t1 - t0) * i / n for i in range(n + 1)})
        previo = [None, None]
        for t in tiempos:
            previa = [c for c in claves if c[0] <= t + 1e-9][-1]
            pos = (cx.evaluar(t), cy.evaluar(t)) if cx is not None else previa[1]
            esc = b.escala_para(pos, acorta) if (acorta and not b.partido) else None
            rs, rc = b.ik(pos, previa[2] if len(previa) > 2 else None, None if esc is None else esc * b.largo, minimo)
            rs, rc = _desenrolla(previo[0], rs), _desenrolla(previo[1], rc)
            previo = [rs, rc]
            s.raw(hom, ROT, t, rs)
            s.raw(cod, ROT, t, rc)
            if esc is not None:
                s.raw(hom, ESCX, t, esc)
                s.raw(hom, ESCY, t, esc)


def cabeza_objetivo(x, lado, frente=True):
    """
    Donde lleva la mano ese brazo para quedar de verdad junto a la cabeza (los gestos de CARA_TAPADA: la visera de Observe, el rascado del Nino).
    «frente»: la FRENTE (por encima de los ojos y a un 12 % del ancho de la cara del borde de ese lado) si el brazo llega hasta alli con el codo
    algo doblado (el 97 % de su largo), y si no la SIEN (junto al borde de la cara de ese lado, a la altura de la ceja). Con una cabeza ancha y
    brazos cortos (el Nino) la frente no se alcanza ni con el brazo estirado y la mano queda en la sien, o lo mas cerca que llegue en esa direccion
    (ik acerca el objetivo al alcance sin cambiar de direccion): ya no se esquiva la cara, como antes (Santiago, 06/10/2026).
    Con «frente» falso, siempre la sien.
    """
    b = x.brazos[lado]
    c = x.cara
    ancho, alto = c[2] - c[0], c[3] - c[1]
    borde = c[0] if b.lado_sig < 0 else c[2]
    arriba = (borde - b.lado_sig * 0.12 * ancho, c[1] - 0.20 * alto)
    sien = (borde + b.lado_sig * 0.02 * ancho, c[1] + 0.12 * alto)
    if frente and math.hypot(arriba[0] - b.S[0], arriba[1] - b.S[1]) <= 0.97 * (b.l1 + b.l2):
        return arriba
    return sien


def fuera(x, lado, y, extra=0.0):
    """
    El punto a la altura y justo FUERA de la silueta del torso por el lado de ese brazo, mas «extra» px hacia fuera: donde
    descansa una mano (en la cadera, en la cintura) y se ve, porque los brazos van detras del torso.
    """
    sig = x.brazos[lado].lado_sig
    px = x.cx
    if x.silueta is not None:
        paso = 2.0
        while px > 0 and px < 1024 and x.silueta.dentro(px, y):
            px += sig * paso
    return (px + sig * extra, y)


def eje(x, lado, d, y):
    """Punto a «d» px del eje del cuerpo hacia el lado de ese brazo (d < 0 cruza el eje), a altura y."""
    return (x.cx + x.brazos[lado].lado_sig * d, y)


def _escala_para(x, lado, resto, phi):
    """
    La escala Y mas grande de la pieza (pierna o antepierna) con la que ningun punto de su silueta pasa del
    suelo, estando su pivote a «resto» px por encima de el y girada «phi» grados (antihorario +):
    cada punto del cierre convexo, a (dx, dy) del pivote, queda a  s dy cos(phi) - dx sen(phi)  por debajo.
    """
    c, sn = math.cos(math.radians(phi)), math.sin(math.radians(phi))
    mejor = 1.0
    for dx, dy in x.pie_casco[lado]:
        if dy * c > 1e-6:
            mejor = min(mejor, (resto + dx * sn) / (dy * c))
    return mejor


def _bajada_min(x, lado, phi):
    """Lo que baja la silueta de la pieza con la escala minima (0,4)."""
    c, sn = math.cos(math.radians(phi)), math.sin(math.radians(phi))
    return max(0.4 * dy * c - dx * sn for dx, dy in x.pie_casco[lado])


def escala_pierna(x, lado, dd, abre, neto):
    """
    Escala Y de la pierna (no segmentado) o de la rodilla (segmentado) para que el punto mas bajo del pie
    quede en el suelo cuando el tronco baja «dd» y el muslo se abre «abre» grados (la pantorrilla queda
    «neto» grados): se calcula con la silueta del pie (su cierre convexo), no con el eje. Cada pierna con sus
    medidas (el dibujo no es simetrico).
    """
    largo, muslo, canilla = x.pata[lado]
    sg = -1.0 if lado == "Izq" else 1.0  # Izq abre con giro negativo, Der con positivo
    dd = dd + 2.0 * min(1.0, (dd + 5.0 * abre) / 10.0)  # 2 px de holgura: el pie flota un pelo, no se hunde
    if x.segmentado:
        return clamp(_escala_para(x, lado, largo - dd - muslo * math.cos(math.radians(abre)), sg * neto), 0.4, 1.0)
    return clamp(_escala_para(x, lado, largo - dd, sg * abre), 0.4, 1.0)


def profundidad_max(x, abre, neto):
    """Cuanto puede bajar el tronco con la escala minima (0,4) de la pierna o la rodilla, en la peor de las dos piernas."""
    peor = 1e9
    for lado in ("Izq", "Der"):
        largo, muslo, canilla = x.pata[lado]
        sg = -1.0 if lado == "Izq" else 1.0
        if x.segmentado:
            v = largo - muslo * math.cos(math.radians(abre)) - _bajada_min(x, lado, sg * neto)
            # y la rotula del muslo (redonda, por debajo de la rodilla) tampoco puede pasar del suelo
            v = min(v, largo - muslo * math.cos(math.radians(abre)) - x.muslo_bajo[lado])
        else:
            v = largo - _bajada_min(x, lado, sg * abre)
        peor = min(peor, v)
    return max(0.0, peor - 3.0)


def agacha(x, s, prof, muslo_plano, muslo_seg, razon, *tw):
    """
    Agacharse a una profundidad, con las piernas ya contando el ancho del pie (escala_pierna): como
    agacharse() pero los pies no se hunden al abrir el muslo. «tw»: pares (tiempo base, peso 0..1).
    Devuelve la bajada del tronco (lo que Tronco debe bajar).
    """
    abre_max = muslo_seg if x.segmentado else muslo_plano
    neto_max = muslo_seg - muslo_seg * razon if x.segmentado else 0.0
    d = min(prof * x.pierna, profundidad_max(x, abre_max, neto_max))
    for i in range(0, len(tw) - 1, 2):
        t, w = tw[i], tw[i + 1]
        dd = d * w
        abre = abre_max * w
        neto = abre - abre * razon if x.segmentado else 0.0
        s.raw(LL, POSY, t, -dd)
        s.raw(RL, POSY, t, -dd)
        s.sym(LL, RL, t, abre)
        if x.segmentado:
            s.sym(LK, RK, t, -abre * razon)
            s.raw(LK, ESCY, t, escala_pierna(x, "Izq", dd, abre, neto))
            s.raw(RK, ESCY, t, escala_pierna(x, "Der", dd, abre, neto))
        else:
            s.raw(LL, ESCY, t, escala_pierna(x, "Izq", dd, abre, neto))
            s.raw(RL, ESCY, t, escala_pierna(x, "Der", dd, abre, neto))
    return d


def piernas_f(x, s, largo, w, prof, muslo_plano, muslo_seg, razon, desl=None, paso=0.1):
    """
    Piernas muestreadas: «w(t)» (0..1) es cuanto esta agachado y «desl(t)» grados de apertura de los muslos
    (peso cargado de un lado). Mismo reparto que agacharse(): no segmentado, las piernas bajan y se acortan
    (escala Y); segmentado, el muslo se abre, la pantorrilla vuelve hacia dentro y se acorta con la escala
    Y de la rodilla. Devuelve la bajada maxima del tronco.
    """
    desl = desl or (lambda t: 0.0)
    abre_max = muslo_seg if x.segmentado else muslo_plano
    neto_max = muslo_seg - muslo_seg * razon if x.segmentado else 0.0
    d = min(prof * x.pierna, profundidad_max(x, abre_max, neto_max))

    def abre(t):
        return abre_max * w(t) + desl(t)

    def neto(t):
        return (muslo_seg * w(t) - muslo_seg * razon * w(t)) if x.segmentado else 0.0

    muestrea(s, LL, ROT, lambda t: -abre(t), largo, paso)
    muestrea(s, RL, ROT, lambda t: abre(t), largo, paso)
    muestrea(s, LL, POSY, lambda t: -d * w(t), largo, paso)
    muestrea(s, RL, POSY, lambda t: -d * w(t), largo, paso)
    esc_i = lambda t: escala_pierna(x, "Izq", d * w(t), abre(t), neto(t))
    esc_d = lambda t: escala_pierna(x, "Der", d * w(t), abre(t), neto(t))
    if not x.segmentado:
        muestrea(s, LL, ESCY, esc_i, largo, paso)
        muestrea(s, RL, ESCY, esc_d, largo, paso)
    else:
        muestrea(s, LK, ROT, lambda t: muslo_seg * razon * w(t), largo, paso)
        muestrea(s, RK, ROT, lambda t: -muslo_seg * razon * w(t), largo, paso)
        muestrea(s, LK, ESCY, esc_i, largo, paso)
        muestrea(s, RK, ESCY, esc_d, largo, paso)
    return d


def plantar(x, s):
    """
    Que los pies sigan en el suelo cuando Cuerpo se inclina. Cuerpo gira alrededor de su pivote, en el suelo,
    y el borde del pie que queda del lado hacia el que baja el cuerpo se hundiria (dx * sen(giro): con 8
    grados y 100 px son 14 px). A la altura de cada pierna se le suma lo que su pie baja por ese giro, con la
    silueta del pie (su cierre convexo) girada alrededor del pivote de Cuerpo.
    """
    giro = s._get(C, ROT, False)
    if giro is None or not giro.claves or x.guia:
        return
    curva = CurvaAuto(giro.claves)
    for ruta, lado in ((LL, "Izq"), (RL, "Der")):
        px, py = x.pie_piv[lado]
        pts = [(a + px - 512.0, b + py - P.SUELO) for a, b in x.pie_casco[lado]]  # relativos al pivote de Cuerpo
        reposo = max(b for _, b in pts)
        base = s._get(ruta, POSY, False)
        previa = CurvaAuto(base.claves) if base is not None and base.claves else None
        tiempos = {c[0] for c in giro.claves} | ({c[0] for c in base.claves} if previa else set()) | {0.0, s.largo}
        c = s._get(ruta, POSY)
        c.claves = []
        for t in sorted(t for t in tiempos if 0.0 <= t <= s.largo + 1e-9):
            cs, sn = math.cos(math.radians(curva.evaluar(t))), math.sin(math.radians(curva.evaluar(t)))
            baja = max(b * cs - a * sn for a, b in pts) - reposo  # cuanto baja (+) el punto mas bajo del pie
            c.set(t, (previa.evaluar(t) if previa else 0.0) + baja)


def cabeza_f(x, s, largo, f_cuello, f_sube=None, ganancia_cabeza=1.2, retraso=3, paso=0.1):
    """
    Cabeza muestreada. Con cabeza propia (arte final) gira el cuello (f_cuello, grados) y la cabeza llega
    «retraso» cuadros tarde; con el arte provisional (la cabeza va dentro del torso) la inclinacion la
    hace el tronco, un poco menos. Devuelve la funcion que el tronco debe SUMAR (0 con cabeza propia).
    """
    if x.cabeza_propia:
        muestrea(s, NK, ROT, f_cuello, largo, paso)
        s.follow(NK, ROT, HD, retraso, ganancia_cabeza)
        if f_sube:
            muestrea(s, NK, POSY, f_sube, largo, paso)
        return lambda t: 0.0
    return lambda t: 0.5 * f_cuello(t)


# ---------------------------------------------------------------------------- Idle: uno por personaje
#
# Dos respiraciones de 3,2 s (base) = 6,4 s x tempo, la respiracion de la direccion de arte (13.3), y encima
# un «gesto de caracter» por ciclo con anticipacion, accion y asentamiento. Brazos y cabeza llegan tarde al
# balanceo del tronco (movimiento secundario). Solo vista frontal.

IDLE_BASE = 6.4


def _respira(t):
    """0 -> 1 -> 0 en cada 3,2 s."""
    return 0.5 * (1.0 - math.cos(TAU * t / 3.2))


def _volumen(s, f_y, largo):
    """Squash & stretch de Cuerpo: Y = f_y(t); X compensa para conservar el volumen (DA 13.2, <= 15 %)."""
    muestrea(s, C, ESCY, f_y, largo)
    muestrea(s, C, ESCX, lambda t: 1.0 - 0.5 * (f_y(t) - 1.0), largo)


def idle_nino(x):
    """
    Curioso e inquieto: respira, mira arriba a un lado y al otro, rebota sobre las rodillas con los brazos
    colgando como pendulos y, una vez por ciclo, se rasca la cabeza (la mano sube POR EL COSTADO hasta la sien: con su cabeza ancha y sus
    brazos cortos llega con el brazo casi estirado, y la mano y el antebrazo pueden tapar parte de la cara: CARA_TAPADA).
    """
    a, h, pf, L = x.a, x.hang, x.pliegue, IDLE_BASE
    s = familia_spec(x, "Idle", "idle", L)
    # el peso pasa de una pierna a la otra despacio
    bal = lambda t: 1.5 * math.sin(TAU * t / L)
    # los rebotes: se hunde (anticipacion), se estira al subir y asienta
    reb = lambda t: bump(t, 2.60, 0.26) + 0.8 * bump(t, 3.12, 0.26)
    alza = lambda t: 0.9 * bump(t, 2.88, 0.2) + 0.55 * bump(t, 3.40, 0.2)
    vy = lambda t: 1.0 + 0.013 * _respira(t) - 0.028 * reb(t) + 0.016 * alza(t)
    _volumen(s, vy, L)
    muestrea(s, C, ROT, bal, L)
    s.follow(C, ROT, T, 2, -0.6)
    d = piernas_f(x, s, L, reb, 0.05, 4.0, 5.0, 1.1, desl=lambda t: 0.8 * a * math.sin(TAU * t / L))
    mira_izq = lambda t: pulso(t, 0.9, 1.45, 2.0, 2.45)
    mira_der = lambda t: pulso(t, 3.7, 4.1, 4.5, 5.0)
    rasca = lambda t: pulso(t, 3.75, 4.55, 5.35, 6.05)
    f_cuello = lambda t: 5.5 * mira_izq(t) - 5.5 * mira_der(t) - 2.5 * reb(t) - 3.0 * rasca(t) * (1 - mira_der(t))
    f_sube = lambda t: 6.0 * (mira_izq(t) + mira_der(t))
    extra = cabeza_f(x, s, L, f_cuello, f_sube)
    muestrea(s, T, POSY, lambda t: -d * reb(t), L)
    if not x.cabeza_propia:
        muestrea(s, T, ROT, lambda t: -0.6 * bal(t) + extra(t), L)
    # brazos: respiran, cuelgan como pendulos tras el rebote y la derecha sube a rascar
    pend = lambda t: pendulo(t, 2.55, 10.0, 0.62, 0.55)
    th_base = lambda t: h + 2.0 * _respira(t) + pend(t)
    pl_base = lambda t: pf + 0.35 * pend(t) + 2.0 * _respira(t)
    th_izq = lambda t: th_base(t) + 4.0 * rasca(t)
    brazo_f(x, s, "Izq", th_izq, pl_base, L)
    # la mano sube POR EL COSTADO hasta la sien (cabeza_objetivo), a la altura de la ceja, y se rasca con un vaiven corto
    rasc = lambda t: pulso(t, 4.55, 4.75, 5.30, 5.40)
    mueve = lambda t: 0.5 * (1.0 + math.sin(TAU * (t - 4.75) / 0.5 - math.pi / 2)) * rasc(t)
    lado_der = cabeza_objetivo(x, "Der", False)  # la sien: la mano llega de verdad (o lo mas cerca que alcance) y puede tapar parte de la cara
    brazo_mix(x, s, "Der", th_base, pl_base, rasca,
              lambda t: (lado_der[0] + 6 * mueve(t), lado_der[1] - 16 * mueve(t)), L, hacia=(1.0, 0.0))
    return s


def idle_nina(x):
    """
    Alegre y decidida: se mece de lado a lado con los brazos que se balancean como pendulos (cada uno llega tarde al
    mecido del cuerpo), inclina la cabeza con gracia y encoge los hombros una vez. Los brazos van detras del torso: las
    manos entrelazadas delante de la barriga no se verian, asi que el caracter lo da el balanceo.
    """
    a, h, pf, L = x.a, x.hang, x.pliegue, IDLE_BASE
    s = familia_spec(x, "Idle", "idle", L)
    bal = lambda t: 3.0 * a * math.sin(TAU * t / L)
    vy = lambda t: 1.0 + 0.012 * _respira(t) + 0.014 * bump(t, 4.35, 0.5)
    _volumen(s, vy, L)
    muestrea(s, C, ROT, bal, L)
    s.follow(C, ROT, T, 2, -0.7)
    piernas_f(x, s, L, lambda t: 0.0, 0.0, 0.0, 0.0, 1.0, desl=lambda t: 1.0 * a * math.sin(TAU * t / L))
    inclina = lambda t: pulso(t, 1.0, 1.6, 2.6, 3.2)
    otro = lambda t: pulso(t, 3.5, 3.9, 4.7, 5.2)
    f_cuello = lambda t: 7.0 * inclina(t) - 4.0 * otro(t)
    extra = cabeza_f(x, s, L, f_cuello, None, 1.2, 3)
    if not x.cabeza_propia:
        muestrea(s, T, ROT, lambda t: -0.7 * bal(t) + extra(t), L)
    encoge = lambda t: pulso(t, 3.95, 4.25, 4.45, 4.85)
    # los brazos se balancean al reves del cuerpo, con medio segundo de retraso, y en el encogimiento se abren un poco
    sway = lambda t, fase: 7.0 * math.sin(TAU * (t - 0.5) / L + fase)
    brazo_f(x, s, "Izq", lambda t: h + sway(t, 0.0) + 9.0 * encoge(t) + 1.5 * _respira(t), lambda t: pf, L)
    brazo_f(x, s, "Der", lambda t: h - sway(t, 0.0) + 9.0 * encoge(t) + 1.5 * _respira(t), lambda t: pf, L)
    return s


def idle_papa(x):
    """
    Sereno y fuerte: brazos en jarra (manos en la cintura, codos hacia fuera), respiracion profunda con el
    pecho hinchado (Vol hasta 1,03) y un asentimiento lento.
    """
    a, h, pf, L = x.a, x.hang, x.pliegue, IDLE_BASE
    s = familia_spec(x, "Idle", "idle", L)
    jarra = lambda t: pulso(t, 0.3, 1.3, 5.3, 6.3)  # se pone en jarra y los suelta al cerrar el ciclo
    hondo = lambda t: bump(t, 1.9, 1.3) + 0.7 * bump(t, 5.0, 1.3)  # las dos respiraciones, la primera honda
    vy = lambda t: 1.0 + 0.030 * hondo(t)
    _volumen(s, vy, L)
    bal = lambda t: 1.1 * a * math.sin(TAU * t / L)
    muestrea(s, C, ROT, bal, L)
    s.follow(C, ROT, T, 2, -0.5)
    piernas_f(x, s, L, lambda t: 0.0, 0.0, 0.0, 0.0, 1.0, desl=lambda t: 0.6 * a * math.sin(TAU * t / L))
    asiente = lambda t: bump(t, 3.0, 0.7) + 0.6 * bump(t, 3.9, 0.5)
    f_cuello = lambda t: -1.5 * asiente(t) + 1.0 * math.sin(TAU * t / L)
    extra = cabeza_f(x, s, L, f_cuello, lambda t: -7.0 * asiente(t), 1.2, 4)
    if not x.cabeza_propia:
        muestrea(s, T, ROT, lambda t: -0.5 * bal(t) + extra(t) - 0.4 * asiente(t), L)
    # en jarra con los PUÑOS en las caderas, por fuera del torso (los brazos van detras de el: dentro no se verian)
    for lado in ("Izq", "Der"):
        b = x.brazos[lado]
        brazo_mix(x, s, lado, lambda t: h + 1.5 * hondo(t), lambda t: pf, jarra,
                  fuera(x, lado, x.y_cadera, 0.3 * b.grosor), L, hacia=(b.lado_sig, 0.15))
    return s


def idle_mama(x):
    """
    Calida y atenta: brazos relajados que respiran, balanceo suave, mira con atencion inclinando la cabeza y, una vez, lleva una mano a la altura
    de la oreja (como recogerse el pelo). Antes las manos descansaban en la cintura POR FUERA del torso (los brazos iban detras de el y dentro no
    se verian); con el hombro en el hombro (hombro.py, 06/10/2026) esa pose saca los codos y se lee como brazos en jarra, que es de Papa: sus manos
    cuelgan junto a la falda, que es donde las dejan los brazos relajados (y ahora se ven, el antebrazo va delante del torso).
    """
    a, h, pf, L = x.a, x.hang, x.pliegue, IDLE_BASE
    s = familia_spec(x, "Idle", "idle", L)
    vy = lambda t: 1.0 + 0.010 * _respira(t)
    _volumen(s, vy, L)
    bal = lambda t: 2.4 * a * math.sin(TAU * t / L)
    muestrea(s, C, ROT, bal, L)
    s.follow(C, ROT, T, 2, -0.6)
    piernas_f(x, s, L, lambda t: 0.0, 0.0, 0.0, 0.0, 1.0, desl=lambda t: 0.8 * a * math.sin(TAU * t / L))
    atenta = lambda t: pulso(t, 0.6, 1.4, 3.0, 3.8)
    pelo = lambda t: pulso(t, 3.3, 4.7, 5.1, 6.3)
    f_cuello = lambda t: 6.0 * atenta(t) + 3.0 * pelo(t)
    extra = cabeza_f(x, s, L, f_cuello, None, 1.2, 3)
    if not x.cabeza_propia:
        muestrea(s, T, ROT, lambda t: -0.6 * bal(t) + extra(t), L)
    # los brazos cuelgan relajados, se abren un poco al inspirar y los antebrazos se pliegan un par de grados hacia dentro: las manos junto a la falda
    th_base = lambda t: h + 1.5 * _respira(t)
    pl_base = lambda t: pf + 3.0 * _respira(t)
    brazo_f(x, s, "Der", th_base, pl_base, L)
    # la izquierda, una vez, sube junto a la oreja (el pelo, nunca tapando los dos ojos). La mano sube RODEANDO el hombro por fuera (brazo estirado
    # hacia abajo y fuera -> oreja) y no en linea recta: la recta del costado a la oreja pasa por el hombro y un brazo de una pieza daria media vuelta
    # de golpe al cruzarlo, y uno de dos segmentos cambiaria de rama el codo (con el brazo estirado las dos ramas coinciden). El peso es el del
    # propio gesto: fuera de el manda el reposo, y el brazo sale de el y vuelve a el sin saltos
    cara = x.cara
    oreja = (cara[0] - 60, 0.55 * cara[1] + 0.45 * cara[3])  # el costado de la cabeza, sobre el pelo, fuera de la cara
    b = x.brazos["Izq"]
    costado = (b.S[0] + b.lado_sig * 0.95 * x.alcance * math.cos(math.radians(30)), b.S[1] + 0.95 * x.alcance * math.sin(math.radians(30)))

    def sube(t):
        u = pelo(t)
        if u < 0.5:
            return costado
        f = 2.0 * u - 1.0
        return (costado[0] + f * (oreja[0] - costado[0]), costado[1] + f * (oreja[1] - costado[1]))

    # el codo siempre hacia fuera y abajo de la mano (la misma rama de principio a fin: el antebrazo sube a la oreja)
    brazo_mix(x, s, "Izq", th_base, pl_base, lambda t: min(1.0, 2.0 * pelo(t)), sube, L, hacia=(b.lado_sig, 0.4), suaviza=0.3)
    return s


def idle(x):
    return {"nino": idle_nino, "nina": idle_nina, "papa": idle_papa, "mama": idle_mama}[x.id](x)


# ---------------------------------------------------------------------------- caminar y correr


def brazos_s(x, s, izq=None, der=None):
    """Como brazos(), con el sentido del pliegue opcional: (t, theta, pliegue[, signo]); signo +1 dentro, -1 a la cabeza."""
    for lado, claves, hom, cod in (("Izq", izq, LA, LE), ("Der", der, RA, RE)):
        if not claves:
            continue
        b = x.brazos[lado]
        for c in claves:
            rs, rc = b.pose(c[1], c[2], c[3] if len(c) > 3 else None)
            s.raw(hom, ROT, c[0], rs)
            s.raw(cod, ROT, c[0], rc)


def paso(x, accion, archivo, p, amp, vaiven, inclina, balanceo, flex_lo, flex_hi, aplasta):
    """
    Caminar y correr (misma estructura que BuildRigs, con inclinacion, rebote, codos, rodillas y cabeza).
    Los pasos alternan: la pierna izquierda sube en 0,25 p y la derecha en 0,75 p. Los brazos cuelgan de la
    pose de reposo y se balancean con el paso; los codos se pliegan mas cuanto mas adelante van.
    """
    sube = 0.14 * x.pierna * amp
    medio = 0.5 * (flex_lo + flex_hi)
    h = x.hang
    s = familia_spec(x, accion, archivo, p)
    s.raw(LL, POSY, 0, 0, 0.25 * p, sube, 0.5 * p, 0, p, 0)
    s.raw(RL, POSY, 0, 0, 0.5 * p, 0, 0.75 * p, sube, p, 0)
    s.rot(LL, 0, 0, 0.25 * p, -4 * amp, 0.5 * p, 0, p, 0)
    s.rot(RL, 0, 0, 0.5 * p, 0, 0.75 * p, 4 * amp, p, 0)
    # La rodilla del pie que vuela se dobla un poco hacia dentro (+ en Izq, - en Der).
    s.rot(LK, 0, 0, 0.25 * p, 6 * amp, 0.5 * p, 0, p, 0)
    s.rot(RK, 0, 0, 0.5 * p, 0, 0.75 * p, -6 * amp, p, 0)
    if x.segmentado:
        s.raw(LK, ESCY, 0, 1, 0.25 * p, 0.9, 0.5 * p, 1, p, 1)
        s.raw(RK, ESCY, 0, 1, 0.5 * p, 1, 0.75 * p, 0.9, p, 1)
    s.raw(C, POSY, 0, 0, 0.25 * p, 10 * amp, 0.5 * p, 0, 0.75 * p, 10 * amp, p, 0)
    s.rot(C, 0, inclina, 0.25 * p, inclina - balanceo, 0.5 * p, inclina, 0.75 * p, inclina + balanceo, p, inclina)
    s.vol(0, 1 - aplasta, 0.25 * p, 1 + 0.7 * aplasta, 0.5 * p, 1 - aplasta, 0.75 * p, 1 + 0.7 * aplasta, p, 1 - aplasta)
    s.rot(T, 0, 0, 0.25 * p, 1.2 * amp, 0.5 * p, 0, 0.75 * p, -1.2 * amp, p, 0)
    brazos(x, s,
           [(0, h, medio), (0.25 * p, h - vaiven, flex_hi), (0.5 * p, h, medio), (0.75 * p, h + vaiven, flex_lo), (p, h, medio)],
           [(0, h, medio), (0.25 * p, h + vaiven, flex_lo), (0.5 * p, h, medio), (0.75 * p, h - vaiven, flex_hi), (p, h, medio)])
    cabeza_sigue(s, C, ROT, -0.35, -0.2)
    return s


def walk(x):
    a = x.a
    return paso(x, "Walk", "caminar", 0.8, a, 8 * a, -4 * a, 1.5 * a, 10, 20, 0.015 * a)


def run(x):
    a = x.a
    return paso(x, "Run", "correr", 0.5, 1.6 * a, 16 * a, -6.5 * a, 2 * a, 30, 46, 0.04 * a)


# ---------------------------------------------------------------------------- hablar, golpear, martillar


def talk(x):
    """Hablar: la mano derecha acompaña lo que dice (sube con la palma hacia dentro) y la cabeza asiente."""
    a, h, pf = x.a, x.hang, x.pliegue
    s = familia_spec(x, "Talk", "hablar", 1.6)
    s.vol(0, 1.0, 0.4, 1.012, 0.8, 1.0, 1.2, 1.012, 1.6, 1.0)
    s.rot(C, 0, 0, 0.8, 1.5, 1.6, 0)
    s.rot(T, 0, 0, 0.4, 1, 0.8, -1, 1.2, 1, 1.6, 0)
    # la mano derecha se abre hacia fuera (el antebrazo sube con la palma hacia dentro), al ritmo de las palabras
    brazos_s(x, s,
             [(0, h, pf), (0.8, h + 5 * a, pf + 4), (1.6, h, pf)],
             [(0, h, pf), (0.4, 52, 62, -1), (0.8, 44, 46, -1), (1.2, 58, 70, -1), (1.6, h, pf)])
    s.sym(LL, RL, 0, 0, 0.8, 0.8 * a, 1.6, 0)
    s.rot(NK, 0, 0, 0.4, -2.5 * a, 0.6, 0.5, 1.2, -2.8 * a, 1.4, 0.5, 1.6, 0)
    s.follow(NK, ROT, HD, 3, 1.0)
    return s


def strike(x):
    """
    Golpear las piedras delante del pecho (0,6 s): las manos se abren (anticipacion: las piedras se separan), chocan
    en el eje y rebotan. En esta accion los brazos pasan DELANTE del torso (CharacterRig.armsInFrontActions: lo decide
    el motor, accion por accion, y el solucionador lo sabe): el choque ocurre donde se ve, a la altura del pecho, una
    mano por lado, cada una a una distancia del eje que es una fraccion del alcance. A la altura de la frente taparia la
    cara con las manos y se leeria como desesperacion (CP-02): por eso el choque queda en el pecho y nunca sube a la cara.

    BRAZO DE UNA PIEZA (arte provisional de Papa, Mama y Nina). Un brazo rigido no se dobla, y dos brazos iguales solo
    se encuentran sobre el eje a un largo de brazo del hombro: ENCIMA de la cabeza (probado: los brazos cruzan por delante
    de la cara y la tapan, pose_preview lo rechaza) o DEBAJO, a la altura de la ingle (inaceptable en un juego de cuarto).
    Dos manos al mismo costado del cuerpo, con las dos por encima de la cintura, tampoco se tocan (quedan a mas de
    110 px). Lo que si cabe es ENCOGER el brazo (escala X e Y, como si apuntara hacia el espectador: ESC_MIN_BRAZO): el choque
    sube hasta el pecho-vientre y las manos se encuentran ahi, siempre por encima de la cintura (pose_preview lo vigila). Con
    brazo partido el codo se dobla y el choque queda en el pecho, sin acortar nada.
    """
    s = familia_spec(x, "Strike", "golpear", 0.6)
    R = x.alcance
    ts = (0.0, 0.14, 0.21, 0.30, 0.36, 0.46, 0.6)
    sep = (0.097, 0.23, 0.29, 0.082, 0.126, 0.097, 0.097)  # mitad de la separacion entre las manos, en alcances (al chocar, una junto a la otra)
    alto = (0, -4, -10, 4, 0, 0, 0)                        # y la altura sobre el pecho (arriba al armar el golpe)
    yy = x.y_pecho + 14
    acorta = None
    minimo = MIN_ALCANCE
    if x.brazos["Izq"].partido:
        # el choque por encima de la cintura (x.y_cintura): con un torso corto (el Nino) el pecho que pide MIN_ALCANCE queda
        # bajo, asi que se sube el choque y se deja que el codo se cierre algo mas
        if yy + 4 > x.y_cintura - 40:
            yy = x.y_cintura - 40 - 4
            minimo = 0.34
            # con los hombros arriba y los brazos largos (la Nina) ese minimo deja el choque bajo la cintura: la mano puede acercarse al
            # hombro lo justo para llegar a ese punto (a la distancia que hay del hombro al choque, con un poco de aire)
            b0 = x.brazos["Izq"]
            llega = math.hypot(abs(b0.S[0] - x.cx) + 0.1 * R, (yy + 4) - b0.S[1]) / float(b0.l1 + b0.l2)
            minimo = max(PISO_MINIMO_STRIKE, min(minimo, llega - 0.02))
    else:
        # el choque, lo mas arriba que deja el encogimiento: ahi el brazo mide ESC_MIN_BRAZO de su largo
        acorta = ESC_MIN_BRAZO
        for b in x.brazos.values():
            dx = abs(abs(b.S[0] - x.cx) - max(0.082 * R, 0.5 * b.grosor * ESC_MIN_BRAZO + 1.0))
            yy = max(yy, b.S[1] + math.sqrt(max((ESC_MIN_BRAZO * b.largo) ** 2 - dx ** 2, 0.0)))
    for lado in ("Izq", "Der"):
        b = x.brazos[lado]
        semi_h = abs(b.S[0] - x.cx)
        tg = []
        for t, d, dy in zip(ts, sep, alto):
            if b.partido and not (0.25 < t < 0.4):   # todo menos el choque y el rebote inmediato; el principio y el final del bucle son iguales
                # al armar el golpe las manos se separan, pero NO mas que los hombros: una mano por fuera del hombro y cerca de el solo se alcanza con
                # el codo hacia ARRIBA (la otra solucion de la cinematica inversa cruza el eje): los codos suben junto a las mejillas y se lee como
                # manos a la cara (Mama y Nina con el arte articulado, 06/10/2026). Debajo del hombro el codo sale hacia fuera, como al golpear.
                d = min(d, semi_h / R)
            tg.append((t, eje(x, lado, d * R, yy + dy)))
        manos(x, s, acorta=acorta, minimo=minimo, **{"izq" if lado == "Izq" else "der": tg})
    s.raw(T, POSY, 0, 0, 0.21, 5, 0.30, -8, 0.38, -3, 0.6, 0)
    s.vol(0, 1.0, 0.21, 1.025, 0.30, 0.965, 0.38, 1.01, 0.6, 1.0)
    s.rot(C, 0, 0, 0.21, 1.2, 0.30, -1.5, 0.6, 0)
    s.follow(T, POSY, NK, 2, 0.25)
    s.follow(T, POSY, HD, 4, 0.15)
    return s


def hammer(x):
    """
    Martillar (0,7 s): retroceso, el brazo derecho sube POR EL COSTADO (el antebrazo junto a la cabeza; el antebrazo puede cruzar el borde de
    la cara: CARA_TAPADA), golpe y asentamiento; la izquierda sostiene.
    """
    s = familia_spec(x, "Hammer", "martillar", 0.7)
    h, pf = x.hang, x.pliegue
    brazos_s(x, s, None, [(0, h, pf, 1), (0.08, h + 16, pf + 14, 1), (0.19, 108, 30, -1), (0.30, 140, 45, -1), (0.45, 136, 50, -1),
                          (0.55, 34, 62, 1), (0.60, 28, 54, 1), (0.65, h + 6, pf + 20, 1), (0.7, h, pf, 1)])
    brazos(x, s, [(0, h, pf), (0.30, h - 4, pf + 3), (0.55, h - 8, pf + 6), (0.65, h, pf), (0.7, h, pf)], None)
    s.rot(C, 0, 0, 0.30, 1.5, 0.55, -3, 0.65, 0.6, 0.7, 0)
    s.vol(0, 1.0, 0.30, 1.03, 0.55, 0.955, 0.62, 1.012, 0.7, 1.0)
    s.raw(T, POSY, 0, 0, 0.08, 2, 0.30, 5, 0.55, -6, 0.62, -1, 0.7, 0)
    cabeza_sigue(s, C, ROT, -0.5, -0.3)
    return s


def blow(x):
    """
    Soplar agachado sobre el monton (0,9 s): agachado, con el tronco inclinado hacia delante (de frente se
    lee como un tronco mas corto), los brazos bajos por delante, junto a los muslos, y tres soplos: el pecho
    se llena y se vacia (Vol) y la cabeza baja con cada soplo.
    """
    h, pf, L = x.hang, x.pliegue, 0.9
    s = familia_spec(x, "Blow", "soplar", L)
    d = agacha(x, s, 0.3, 0, 8, 1.15, 0, 1, 0.9, 1)
    centros = (0.18, 0.46, 0.74)
    llena = lambda t: sum(bump(t, c - 0.08, 0.08) for c in centros)       # inspira
    sopla = lambda t: sum(bump(t, c + 0.03, 0.09) for c in centros)       # sopla
    vy = lambda t: 1.0 + 0.03 * llena(t) - 0.05 * sopla(t)
    _volumen(s, vy, L)
    s.raw(T, POSY, 0, -d, 0.9, -d)
    s.raw(T, ESCY, 0, 0.95, 0.9, 0.95)      # el tronco inclinado hacia delante, visto de frente
    if x.cabeza_propia:
        muestrea(s, NK, ROT, lambda t: 2.0 * llena(t) - 3.0 * sopla(t), L, 0.05)
        muestrea(s, NK, POSY, lambda t: 3.0 * llena(t) - 5.0 * sopla(t), L, 0.05)
        s.follow(NK, ROT, HD, 3, 1.0)
    else:
        muestrea(s, T, ROT, lambda t: 0.0 * t, L)
    # los brazos cuelgan por delante, casi estirados, y se mecen con cada soplo
    brazo_f(x, s, "Izq", lambda t: 16.0 + 3.0 * sopla(t), lambda t: 16.0, L, 0.05)
    brazo_f(x, s, "Der", lambda t: 16.0 + 3.0 * sopla(t), lambda t: 16.0, L, 0.05)
    return s


def pickup(x):
    """Recoger: un respingo de anticipacion, se agacha, alcanza con la derecha, se levanta con lo recogido (1,4 s)."""
    s = familia_spec(x, "PickUp", "recoger", 1.4)
    h, pf = x.hang, x.pliegue
    d = agacha(x, s, 0.35, 0, 10, 1.2, 0, 0, 0.4, 1, 0.7, 1, 1.1, 0, 1.4, 0)
    s.raw(T, POSY, 0, 0, 0.1, 4, 0.4, -d, 0.7, -d, 1.1, 3, 1.25, 0, 1.4, 0)
    brazos(x, s,
           [(0, h, pf), (0.1, h + 5, pf), (0.4, h - 10, pf + 14), (0.7, h - 10, pf + 14), (1.1, h + 2, pf), (1.4, h, pf)],
           [(0, h, pf), (0.1, h + 8, pf), (0.4, h + 14, 6), (0.7, h + 14, 6), (1.1, h - 4, pf + 70), (1.2, h - 8, pf + 80), (1.4, h, pf)])
    s.vol(0, 1.0, 0.1, 1.02, 0.4, 0.965, 0.7, 0.97, 1.1, 1.04, 1.25, 1.01, 1.4, 1.0)
    s.rot(NK, 0, 0, 0.4, 5, 0.7, 5, 1.1, -2, 1.4, 0)
    s.follow(NK, ROT, HD, 3, 1.0)
    return s


def kneel(x):
    """Arrodillado: la cadera baja y las rodillas se abren; de frente es lo que se lee como «de rodillas»."""
    s = familia_spec(x, "Kneel", "arrodillarse", 3.0)
    h, pf = x.hang, x.pliegue
    d = agacha(x, s, 0.62, 14, 14, 1.25, 0, 1, 3, 1)
    s.raw(T, POSY, 0, -d, 3, -d)
    s.vol(0, 1.0, 1.5, 1.008, 3.0, 1.0)
    # las manos descansan sobre los muslos
    brazos(x, s, [(0, h - 2, pf + 8), (1.5, h - 4, pf + 12), (3, h - 2, pf + 8)], [(0, h - 2, pf + 8), (1.5, h - 4, pf + 12), (3, h - 2, pf + 8)])
    s.rot(T, 0, 0, 1.5, 0.8, 3, 0)
    s.rot(NK, 0, 3, 1.5, 4.5, 3, 3)
    s.follow(NK, ROT, HD, 4, 1.0)
    return s


# ---------------------------------------------------------------------------- cargar, empujar, señalar, observar


def carry(x):
    """Cargar: el paso de caminar con los brazos en V sobre los hombros (por los lados de la cabeza; los antebrazos pueden rozar la cara: CARA_TAPADA)."""
    a, h = x.a, x.hang
    s = paso(x, "Carry", "cargar", 0.95, 0.8 * a, 0, -2 * a, 1.2 * a, 0, 0, 0.01 * a)
    for r in (LA, RA, LE, RE):
        s.drop(r, ROT)
    arriba, medio = 122, 116
    brazos_s(x, s, [(0, arriba, 14, -1), (0.475, medio, 22, -1), (0.95, arriba, 14, -1)],
             [(0, arriba, 14, -1), (0.475, medio, 22, -1), (0.95, arriba, 14, -1)])
    return s


def push(x):
    """
    Empujar: inclinado hacia delante y con el paso, los brazos tensos con los puños a los lados de las caderas, POR FUERA
    del torso (las manos juntas delante del pecho no se verian: los brazos van detras de el).
    """
    a, h, pf = x.a, x.hang, x.pliegue
    s = paso(x, "Push", "empujar", 0.9, 0.8 * a, 0, 0, 0, 0, 0, 0.01 * a)
    for r in (LA, RA, LE, RE, C):
        s.drop(r, ROT)
    for lado in ("Izq", "Der"):
        b = x.brazos[lado]
        y = x.y_cadera - 10
        tg = [(0, fuera(x, lado, y, 0.3 * b.grosor)), (0.45, fuera(x, lado, y + 8, 0.45 * b.grosor)), (0.9, fuera(x, lado, y, 0.3 * b.grosor))]
        manos(x, s, **{"izq" if lado == "Izq" else "der": tg})
    s.rot(C, 0, -4, 0.45, -5, 0.9, -4)
    cabeza_sigue(s, C, ROT, -0.35, -0.2)
    return s


def point(x):
    """Señalar: el brazo derecho casi horizontal hacia lo que tiene delante; en diagonal parecia saludar."""
    a, h, pf = x.a, x.hang, x.pliegue
    s = familia_spec(x, "Point", "senalar", 1.2)
    brazos_s(x, s,
             [(0, h + 2, pf), (0.6, h + 5, pf), (1.2, h + 2, pf)],
             [(0, 96, 4, -1), (0.3, 100, 3, -1), (0.6, 94, 6, -1), (0.9, 99, 3, -1), (1.2, 96, 4, -1)])
    s.rot(C, 0, -3, 0.6, -3.5, 1.2, -3)
    s.rot(T, 0, -1.2, 0.6, -1.5, 1.2, -1.2)
    s.vol(0, 1.0, 0.6, 1.006, 1.2, 1.0)
    s.rot(NK, 0, -2.5, 0.6, -3.2, 1.2, -2.5)
    s.follow(NK, ROT, HD, 3, 1.0)
    return s


def observe(x):
    """
    Observar: la izquierda de visera —la mano llega de verdad a la frente, por encima de los ojos, o a la sien si el brazo no alcanza la frente
    (el Nino): cabeza_objetivo; la mano y el antebrazo pueden tapar parte de la cara (CARA_TAPADA) pero nunca los dos ojos— y la derecha en la
    cintura, por fuera del torso; mirando a un lado y a otro (2 s).
    """
    a, h, pf = x.a, x.hang, x.pliegue
    s = familia_spec(x, "Observe", "observar", 2.0)
    s.rot(C, 0, 5, 1, 8, 2, 5)
    s.vol(0, 1.0, 1, 1.008, 2, 1.0)
    s.rot(T, 0, 0, 1, -1.2 * a, 2, 0)
    s.sym(LL, RL, 0, 0, 1, 1.2 * a, 2, 0)
    fuera_ = (-1.0, -0.6)
    visera = cabeza_objetivo(x, "Izq")   # a la frente, o a la sien si el brazo no llega a la frente (el Nino): la mano puede tapar parte de la cara
    manos(x, s, [(0, visera, fuera_), (1.0, (visera[0] + 6, visera[1] + 2), fuera_), (2.0, visera, fuera_)],
          [(0, fuera(x, "Der", x.y_cadera, 0.3 * x.brazos["Der"].grosor), (1.0, 0.15)),
           (2.0, fuera(x, "Der", x.y_cadera, 0.3 * x.brazos["Der"].grosor), (1.0, 0.15))])
    s.rot(NK, 0, 0, 0.5, -6, 1, 0, 1.5, 6, 2, 0)
    s.follow(NK, ROT, HD, 4, 1.0)
    return s


# ---------------------------------------------------------------------------- celebrar, animo, abrazar, sorpresa, dormir


def celebrate(x):
    """Celebrar sin despegar los pies: brazos arriba (en V, a los lados de la cabeza) y rebote de squash & stretch (<= 15 %)."""
    a = x.a
    b = 0.06 * a
    s = familia_spec(x, "Celebrate", "celebrar", 1.2)
    # los brazos se abren en V a los lados de la cabeza (el humero a 25-40 grados sobre la horizontal) y suben
    # y bajan con el rebote; el antebrazo apenas se pliega (con una cabeza ancha, la del Nino, las manos quedan junto a las mejillas: CARA_TAPADA)
    arriba, medio = 130, 112
    brazos_s(x, s,
             [(0, arriba, 8, -1), (0.3, medio, 16, -1), (0.6, arriba, 8, -1), (0.9, medio, 16, -1), (1.2, arriba, 8, -1)],
             [(0, arriba, 8, -1), (0.3, medio, 16, -1), (0.6, arriba, 8, -1), (0.9, medio, 16, -1), (1.2, arriba, 8, -1)])
    s.vol(0, 1.0, 0.15, 1 - b, 0.3, 1 + b, 0.6, 1.0, 0.75, 1 - b, 0.9, 1 + b, 1.2, 1.0)
    s.rot(C, 0, 0, 0.3, 1.5, 0.6, 0, 0.9, -1.5, 1.2, 0)
    s.rot(T, 0, 0, 0.3, -2, 0.6, 0, 0.9, 2, 1.2, 0)
    s.sym(LL, RL, 0, 0, 0.15, 3 * a, 0.3, 0, 0.6, 0, 0.75, 3 * a, 0.9, 0, 1.2, 0)
    s.sym(LK, RK, 0, 0, 0.15, -5 * a, 0.3, 0, 0.6, 0, 0.75, -5 * a, 0.9, 0, 1.2, 0)
    # al abrir las piernas en el rebote el borde del pie baja un par de pixeles bajo el suelo (Papa, el de las piernas mas largas, 2,3 px): las
    # piernas suben lo mismo en esos dos cuadros
    s.raw(LL, POSY, 0, 0, 0.15, 2.4 * a, 0.3, 0, 0.6, 0, 0.75, 2.4 * a, 0.9, 0, 1.2, 0)
    s.raw(RL, POSY, 0, 0, 0.15, 2.4 * a, 0.3, 0, 0.6, 0, 0.75, 2.4 * a, 0.9, 0, 1.2, 0)
    s.rot(NK, 0, 0, 0.3, 4, 0.6, 0, 0.9, -4, 1.2, 0)
    s.follow(NK, ROT, HD, 3, 1.2)
    return s


def encourage(x):
    """
    Animo tras un intento sin exito (CP-02: calido, nunca de reproche). Anticipacion (se agacha un poco y el
    brazo baja), el puño sube por FUERA de la silueta de la cabeza, por encima del hombro, con un bombeo;
    rebote alegre de rodillas, cabeceo afirmativo y la otra mano abierta y relajada. El puño pasa junto a la cabeza (CARA_TAPADA) pero no la tapa.
    Dura 0,9 s x tempo, como siempre: los controladores de nivel lo reproducen con PlayFor un tiempo fijo
    (EncourageSeconds = 0,9) y pasan a Idle; la anticipacion y el rebote son cortos para que el asentamiento
    (de 0,52 a 0,9) quepa entero y el clip acabe en la pose de reposo.
    """
    h, pf, L = x.hang, x.pliegue, 0.9
    s = familia_spec(x, "Encourage", "animo", L)
    w = lambda t: bump(t, 0.13, 0.13) + 0.45 * bump(t, 0.50, 0.12)       # rodillas: anticipacion y rebote
    estira = lambda t: bump(t, 0.33, 0.11)                               # se estira al subir el puño
    vy = lambda t: 1.0 - 0.03 * w(t) + 0.04 * estira(t)
    _volumen(s, vy, L)
    d = piernas_f(x, s, L, w, 0.05, 3.0, 5.0, 1.1, paso=0.05)
    muestrea(s, T, POSY, lambda t: -d * w(t), L, 0.05)
    muestrea(s, C, ROT, lambda t: -1.5 * estira(t), L, 0.05)
    asiente = lambda t: bump(t, 0.14, 0.09) + 0.7 * bump(t, 0.50, 0.09)    # cabeceo afirmativo: baja dos veces
    if x.cabeza_propia:
        muestrea(s, NK, ROT, lambda t: 1.0 * estira(t), L, 0.05)
        muestrea(s, NK, POSY, lambda t: -4.0 * asiente(t), L, 0.05)
        s.follow(NK, ROT, HD, 3, 1.2)
    else:
        muestrea(s, T, ROT, lambda t: 0.0 * t, L, 0.05)
    # el brazo derecho: baja (anticipacion), sube por fuera de la cabeza, bombea y vuelve
    arriba, pf_up, bombeo = 122, 6.0, 8
    brazos_s(x, s,
             [(0, h + 6, pf), (0.3, h + 8, pf), (0.52, h + 6, pf), (0.9, h + 6, pf)],       # izquierda: abierta y relajada
             [(0, h, pf), (0.14, 14, pf + 8), (0.25, 100, pf_up + 10, -1), (0.33, arriba, pf_up, -1),
              (0.41, arriba - bombeo, pf_up + 8, -1), (0.50, arriba, pf_up, -1), (0.66, 60, pf + 6, -1), (0.9, h, pf)])
    return s


def hug(x):
    """
    Abrazar: los brazos se abren de par en par (esperando a quien llega) y se CIERRAN rodeando el cuerpo por fuera, hasta las
    manos a los costados a la altura de la cintura. Van detras del torso: cruzados por delante del pecho desapareceria
    el brazo, asi que el abrazo se lee como brazos que se pliegan en torno a la silueta, sin esconderse. Se mece.
    """
    h, pf = x.hang, x.pliegue
    s = familia_spec(x, "Hug", "abrazar", 2.0)
    R = x.alcance
    semi = abs(x.brazos["Izq"].S[0] - x.cx)
    # abiertos: casi el alcance entero, casi rectos. El codo va SIEMPRE hacia fuera y un poco arriba (la misma rama de la
    # cinematica inversa de principio a fin): si se deja elegir, a mitad del cierre cambia de rama y el brazo da un salto
    abierto = semi + 0.95 * R
    for lado in ("Izq", "Der"):
        b = x.brazos[lado]
        hacia = (b.lado_sig, -0.4)
        cintura = fuera(x, lado, x.y_vientre, 0.2 * b.grosor)
        tg = [(0, eje(x, lado, abierto, x.y_pecho - 10), hacia), (0.8, eje(x, lado, 0.95 * abierto, x.y_pecho - 4), hacia),
              (1.2, cintura, hacia), (1.6, cintura, hacia), (2.0, eje(x, lado, abierto, x.y_pecho - 10), hacia)]
        manos(x, s, **{"izq" if lado == "Izq" else "der": tg})
    s.rot(C, 0, -2.5, 1, 2.5, 2, -2.5)
    s.vol(0, 1.0, 1, 0.985, 2, 1.0)
    s.rot(T, 0, 0, 1, -1, 2, 0)
    cabeza_sigue(s, C, ROT, -1.0, -0.5)  # la cabeza se inclina al lado contrario del mecido
    return s


def surprise(x):
    """Sorpresa: pose sostenida con un temblor leve (los brazos se abren a los lados); el sobresalto lo da el fundido de entrada."""
    h, pf = x.hang, x.pliegue
    s = familia_spec(x, "Surprise", "sorpresa", 1.0)
    brazos(x, s,
           [(0, h + 26, pf + 8), (0.25, h + 29, pf + 8), (0.5, h + 26, pf + 12), (0.75, h + 29, pf + 8), (1, h + 26, pf + 8)],
           [(0, h + 26, pf + 8), (0.25, h + 29, pf + 8), (0.5, h + 26, pf + 12), (0.75, h + 29, pf + 8), (1, h + 26, pf + 8)])
    s.vol(0, 1.03, 0.5, 1.04, 1, 1.03)
    s.sym(LL, RL, 0, 2.5, 0.5, 3, 1, 2.5)
    s.sym(LK, RK, 0, -3, 0.5, -3.5, 1, -3)
    s.rot(T, 0, 0, 0.5, -0.6, 1, 0)
    s.rot(NK, 0, -3, 0.5, -4, 1, -3)  # la cabeza hacia atras
    s.follow(NK, ROT, HD, 3, 1.0)
    return s


def sleep(x):
    """
    Dormir: sentado casi en el suelo, recostado de lado y respirando despacio (4 s). Tumbado del todo, con los
    ojos abiertos del sprite, se leeria como caido (CP-02).
    """
    h, pf = x.hang, x.pliegue
    s = familia_spec(x, "Sleep", "dormir", 4.0)
    d = agacha(x, s, 0.66, 16, 16, 1.25, 0, 1, 4, 1)
    s.raw(T, POSY, 0, -d, 4, -d)
    s.rot(T, 0, 16, 2, 17.5, 4, 16)
    s.vol(0, 1.0, 2, 1.025, 4, 1.0)
    # los brazos descansan: uno sobre el regazo, otro caido junto al cuerpo
    brazos(x, s, [(0, h - 6, pf + 16), (2, h - 6, pf + 20), (4, h - 6, pf + 16)], [(0, h + 8, pf + 6), (2, h + 8, pf + 8), (4, h + 8, pf + 6)])
    s.rot(NK, 0, 8, 2, 9.5, 4, 8)  # la cabeza cae hacia el lado del recostado
    s.follow(NK, ROT, HD, 4, 1.0)
    return s


def visibilidad(x, lista):
    """Oculto, aparecer y apagarse: alfa del lienzo y escala del cuerpo. Parpadeos lentos (RNF-21)."""
    a, h, pf = x.a, x.hang, x.pliegue
    lista.append(familia_spec(x, "Hidden", "oculto", 1.0).raw("Lienzo", ALFA, 0, 0.0, 1, 0.0))
    # Aparecer: la escala sube con estiramiento (Y se pasa y X se afloja) y asienta con un bamboleo
    # amortiguado del cuerpo, los brazos y la cabeza.
    ap = familia_spec(x, "Appear", "aparicion", 1.0, False)
    ap.raw("Lienzo", ALFA, 0, 0.0, 0.5, 1.0, 1, 1.0)
    ap.raw(C, ESCY, 0, 0.3, 0.6, 1.08, 0.8, 0.96, 1, 1.0)
    ap.raw(C, ESCX, 0, 0.3, 0.6, 0.98, 0.8, 1.04, 1, 1.0)
    ap.rot(C, 0, 0, 0.65, -2.5 * a, 0.8, 1.8 * a, 0.92, -0.7 * a, 1, 0)
    brazos(x, ap, [(0, h, pf), (0.7, h + 6, pf), (0.85, h - 2, pf), (1, h, pf)], [(0, h, pf), (0.7, h + 6, pf), (0.85, h - 2, pf), (1, h, pf)])
    cabeza_sigue(ap, C, ROT, -0.8, -0.5)
    lista.append(ap)
    van = familia_spec(x, "Vanish", "apagado", 1.6, False)
    van.raw("Lienzo", ALFA, 0, 1.0, 0.3, 0.35, 0.6, 1.0, 0.9, 0.35, 1.2, 1.0, 1.6, 0.0)
    van.raw(C, ESCX, 0, 1.0, 1.2, 1.0, 1.6, 0.6)
    van.raw(C, ESCY, 0, 1.0, 1.2, 1.0, 1.6, 0.6)
    brazos(x, van, [(0, h, pf), (1.2, h, pf), (1.6, h + 10, pf)], [(0, h, pf), (1.2, h, pf), (1.6, h + 10, pf)])
    lista.append(van)


# ============================================================================ 2b. EL CUERPO DE PERFIL (INC-134, 09/10/2026)
#
# La familia gana un SEGUNDO cuerpo, Lienzo/Perfil, dibujado de perfil y mirando a la DERECHA (el motor voltea el Lienzo para mirar a la izquierda), que
# se ve en las siete acciones de ActionView (ACCIONES_PERFIL: Walk, Run, Carry, Push, PickUp, Kneel y Blow); en las demas se ve el de frente. Los DOS
# cuerpos los anima el mismo clip, con las mismas duraciones: aqui solo se AÑADEN curvas sobre los huesos de perfil (P.HUESOS_PERFIL) y los de frente
# no se tocan en ningun clip. En los clips de frente los 12 huesos de perfil llevan una curva constante en su reposo (Spec.finish), para que ninguno
# caiga a la pose en T.
#
# SIGNOS (el cuerpo mira a la DERECHA; el + de Z de Unity es antihorario en pantalla):
#   Tronco       - inclina hacia DELANTE (la cabeza se va a la derecha) y + hacia atras. Las piernas, los brazos y la cabeza cuelgan de Tronco: si el
#                tronco se inclina, ellos lo hacen con el. Por eso la coreografia se escribe en angulos del MUNDO (el muslo a +20 es 20 grados hacia
#                delante, incline lo que se incline el tronco) y aqui se restan al tronco para sacar la rotacion LOCAL (resuelve_pose).
#   Muslo, brazo + lleva el pie o la mano hacia DELANTE; - hacia atras.
#   Rodilla      - FLEXIONA (la pantorrilla se va hacia atras): una rodilla nunca pasa de 0 hacia el lado contrario (hiperextension).
#   Codo         + FLEXIONA (el antebrazo sube hacia delante): un codo nunca baja de 0.
#   Cuello/Cabeza + levanta la cara hacia atras; - la baja hacia delante. Llegan tarde al tronco (2 y 4 cuadros), como de frente.
# Las coordenadas son las del resto del archivo (lienzo de 1024, y hacia ABAJO); rota() gira un vector en esos ejes.
#
# CINEMATICA (GeoPerfil). La figura son cadenas de segmentos con las longitudes y los puntos de la clave «perfil» de rig_articulaciones.json (que son
# PROVISIONALES hasta que llegue el arte de perfil: se derivan de la figura de frente): el muslo y la canilla de cada pierna, el humero y el antebrazo
# de cada brazo, el tronco con la cabeza. Con ella:
#   - los pies se PLANTAN: en cada cuadro se baja (o sube) Tronco lo justo para que el punto mas bajo de las dos piernas toque el suelo, asi un paso
#     abre las piernas y la cadera BAJA sola, y una agachada baja la cadera lo que pide la flexion; en las agachadas y el arrodillado, ademas, un
#     desplazamiento en X deja el pie de apoyo donde estaba (Tronco POSX), de modo que la cadera va hacia atras y los pies no resbalan;
#   - las manos que tocan algo (los muslos, el monton del suelo) se llevan por cinematica inversa de dos segmentos (GeoPerfil.ik), no a ojo;
#   - la prueba de valida_perfil() recorre cada clip de perfil de clips_personajes.json con esta misma cinematica: pies que no atraviesan el suelo,
#     rodillas y codos que no se doblan al reves, giros de <= 1300 grados por segundo, squash & stretch <= 15 %, manos y cabeza sobre el suelo.
# El pie es parte de la canilla (un solo sprite, como de frente): en una agachada la canilla se inclina y el pie con ella, y el punto mas bajo del
# pie (talon o punta) es el que toca el suelo; GeoPerfil.contactos lleva las dos esquinas de la suela y la rotula (el borde de delante de la rodilla,
# que es lo que toca el suelo al arrodillarse).

DENSIDAD_PLANTA = 3   # el desplazamiento de Tronco (que planta los pies) se muestrea este numero de veces mas denso que las rotaciones


def rota(v, grados):
    """El vector v = (x, y con la y hacia ABAJO, como el lienzo) girado «grados» en sentido antihorario en pantalla (el + de Z de Unity)."""
    c, sn = math.cos(math.radians(grados)), math.sin(math.radians(grados))
    return (v[0] * c + v[1] * sn, -v[0] * sn + v[1] * c)


def _mas(a, b):
    return (a[0] + b[0], a[1] + b[1])


def _menos(a, b):
    return (a[0] - b[0], a[1] - b[1])


def _dist(a, b):
    return math.hypot(a[0] - b[0], a[1] - b[1])


def _angulo(v):
    """El angulo de un vector del lienzo, en grados, medido antihorario desde «hacia abajo» (el inverso de rota((0, 1), a))."""
    return math.degrees(math.atan2(v[0], v[1]))


def biseca(f, lo, hi, iteraciones=48):
    """La raiz de f en [lo, hi] por biseccion; si f no cambia de signo, el extremo donde |f| es menor (el limite al que se llega)."""
    flo, fhi = f(lo), f(hi)
    if flo == 0.0:
        return lo
    if fhi == 0.0:
        return hi
    if flo * fhi > 0.0:
        return lo if abs(flo) <= abs(fhi) else hi
    for _ in range(iteraciones):
        mid = 0.5 * (lo + hi)
        fm = f(mid)
        if flo * fm <= 0.0:
            hi, fhi = mid, fm
        else:
            lo, flo = mid, fm
    return 0.5 * (lo + hi)


class GeoPerfil:
    """
    La figura de perfil como cadenas de segmentos (INC-134): sale de la clave «perfil» de un personaje de rig_articulaciones.json (nodos con su punto
    y su rect) y es lo unico que la coreografia de perfil sabe del cuerpo. Cercano = el lado que mira al espectador (las claves «c»); Lejano = el
    otro («l»). Cuando llegue el arte de perfil se vuelve a correr este script con el JSON nuevo y todo se recalcula.
    """

    def __init__(self, pj):
        pf = pj["perfil"]
        n = {q["nombre"]: q for q in pf["nodos"]}
        self.provisional = bool(pf.get("provisional"))
        self.suelo = P.SUELO
        self.raiz = tuple(float(v) for v in n["Perfil"]["punto"])   # el pivote de Perfil: el suelo, donde escala el squash & stretch
        self.cadera = tuple(float(v) for v in n["Tronco"]["punto"])  # el pivote de Tronco
        self.cuello = tuple(float(v) for v in n["Cuello"]["punto"])
        rc = n["Cuello"]["rect"]
        self.copa = (0.5 * (rc[0] + rc[2]), float(rc[1]))              # lo mas alto de la cabeza
        self.alto = self.suelo - self.copa[1]                          # la figura entera (870 con la coronilla en y = 77)
        self.brazo, self.pierna = {}, {}
        for lado, nb, np_ in (("c", "Cercano", "Cercana"), ("l", "Lejano", "Lejana")):
            hombro = tuple(float(v) for v in n["Brazo" + nb]["punto"])
            codo = tuple(float(v) for v in n["Codo" + nb]["punto"])
            ra = n["Codo" + nb]["rect"]
            mano = (codo[0], float(ra[3]))                              # la punta: el borde de abajo del antebrazo
            self.brazo[lado] = {"hombro": hombro, "codo": codo, "mano": mano, "l1": _dist(hombro, codo), "l2": _dist(codo, mano),
                                "a1": _angulo(_menos(codo, hombro)), "a2": _angulo(_menos(mano, codo))}
            cadera = tuple(float(v) for v in n["Pierna" + np_]["punto"])
            rodilla = tuple(float(v) for v in n["Rodilla" + np_]["punto"])
            rm, rr = n["Pierna" + np_]["rect"], n["Rodilla" + np_]["rect"]
            self.pierna[lado] = {
                "cadera": cadera, "rodilla": rodilla, "l1": _dist(cadera, rodilla), "l2": float(rr[3]) - rodilla[1], "grosor": float(rm[2] - rm[0]),
                # relativos a la rodilla: las dos esquinas de la suela (talon y punta) y la rotula (el borde de delante del muslo a la altura de la rodilla)
                "contactos": [(rr[0] - rodilla[0], rr[3] - rodilla[1]), (rr[2] - rodilla[0], rr[3] - rodilla[1]), (rm[2] - rodilla[0], 0.0)],
            }
        pts = self.puntos({})
        self.pie_x0 = {l: self.x_suela(pts, l) for l in ("c", "l")}
        self.y_reposo = {l: max(p[1] for p in pts["pies_" + l]) - pts["cadera_" + l][1] for l in ("c", "l")}   # del pivote de la cadera al suelo, de pie

    # -- cinematica directa
    def puntos(self, loc, desp=(0.0, 0.0), esc=(1.0, 1.0)):
        """
        Los puntos del cuerpo (lienzo, y hacia abajo) con las rotaciones LOCALES de «loc» (grados; T tronco, pc/pl muslos, kc/kl rodillas, bc/bl brazos,
        ec/el codos, n cuello, h cabeza, P el giro de Perfil; lo que falte vale 0), el desplazamiento «desp» (dx, dy) de Tronco y la escala «esc» de
        Perfil (alrededor del suelo). Devuelve cadera_c/l, rodilla_c/l, pies_c/l (lista de puntos de contacto), hombro_c/l, codo_c/l, mano_c/l,
        cuello y copa.
        """
        tau = loc.get("T", 0.0)
        cad0 = _mas(self.cadera, desp)

        def del_tronco(p):
            return _mas(cad0, rota(_menos(p, self.cadera), tau))

        out = {}
        for lado in ("c", "l"):
            pi, br = self.pierna[lado], self.brazo[lado]
            cadera = del_tronco(pi["cadera"])
            mu = tau + loc.get("p" + lado, 0.0)
            rod = _mas(cadera, rota(_menos(pi["rodilla"], pi["cadera"]), mu))
            ca = mu + loc.get("k" + lado, 0.0)
            out["cadera_" + lado], out["rodilla_" + lado] = cadera, rod
            out["pies_" + lado] = [_mas(rod, rota(q, ca)) for q in pi["contactos"]]
            hombro = del_tronco(br["hombro"])
            ba = tau + loc.get("b" + lado, 0.0)
            codo = _mas(hombro, rota(_menos(br["codo"], br["hombro"]), ba))
            mano = _mas(codo, rota(_menos(br["mano"], br["codo"]), ba + loc.get("e" + lado, 0.0)))
            out["hombro_" + lado], out["codo_" + lado], out["mano_" + lado] = hombro, codo, mano
        cuello = del_tronco(self.cuello)
        out["cuello"] = cuello
        out["copa"] = _mas(cuello, rota(_menos(self.copa, self.cuello), tau + loc.get("n", 0.0) + loc.get("h", 0.0)))
        giro = loc.get("P", 0.0)
        if giro or tuple(esc) != (1.0, 1.0):
            def raiz(q):
                v = rota(((q[0] - self.raiz[0]) * esc[0], (q[1] - self.raiz[1]) * esc[1]), giro)
                return (self.raiz[0] + v[0], self.raiz[1] + v[1])
            out = {k: ([raiz(q) for q in v] if isinstance(v, list) else raiz(v)) for k, v in out.items()}
        return out

    # -- pies en el suelo
    @staticmethod
    def x_suela(pts, lado):
        """La x del centro de la suela (el talon y la punta, no la rotula) de ese lado: el pie que no resbala."""
        return 0.5 * (pts["pies_" + lado][0][0] + pts["pies_" + lado][1][0])

    def baja(self, loc):
        """Cuanto hay que BAJAR el cuerpo (negativo = subirlo) para que el punto mas bajo de las dos piernas toque el suelo."""
        pts = self.puntos(loc)
        return self.suelo - max(p[1] for lado in ("c", "l") for p in pts["pies_" + lado])

    def corre_x(self, loc, lado="c"):
        """El desplazamiento en X de Tronco que deja el pie de ese lado donde estaba de pie: el pie de apoyo no resbala, es la cadera la que se mueve."""
        return self.pie_x0[lado] - self.x_suela(self.puntos(loc), lado)

    def y_apoyo(self, lado, muslo, canilla):
        """El punto mas bajo de una pierna medido desde su cadera, con el muslo y la canilla a esos angulos del MUNDO (la flexion es muslo - canilla)."""
        pi = self.pierna[lado]
        rod = rota(_menos(pi["rodilla"], pi["cadera"]), muslo)
        return max(rod[1] + rota(q, canilla)[1] for q in pi["contactos"])

    def resuelve_muslo(self, lado, canilla, y_objetivo):
        """El angulo del muslo (0 a 100 grados hacia delante) con el que, con la canilla a ese angulo del mundo, el punto mas bajo de la pierna queda a y_objetivo de la cadera."""
        return biseca(lambda m: self.y_apoyo(lado, m, canilla) - y_objetivo, 0.0, 100.0)

    def canilla_apoyada(self, lado, muslo):
        """
        El angulo del mundo de la canilla de una pierna ARRODILLADA (la rodilla en el suelo, la canilla hacia atras y arriba) con el que la rotula y la punta
        del pie tocan el suelo a la vez: ni la rodilla flota ni la punta se clava. Entre -60 y -175 grados: con el arte de perfil de verdad (preparar_perfil.py,
        09/10/2026) el cruce cae cerca de -90 (-79 en el Nino), fuera del intervalo -95..-175 que bastaba para la figura provisional, y la pantorrilla quedaba
        apoyada solo por la rotula con el pie ~12 px en el aire.
        """
        pi = self.pierna[lado]

        def resta(a):
            ys = [rota(q, a)[1] for q in pi["contactos"]]
            return max(ys[0], ys[1]) - ys[2]

        return biseca(resta, -60.0, -175.0)

    # -- cinematica inversa del brazo
    def ik(self, lado, hombro, objetivo):
        """
        (angulo del MUNDO del humero, flexion del codo >= 0) con los que la mano de ese brazo llega a «objetivo» desde «hombro» (el hombro en el lienzo,
        ya con el tronco inclinado): dos segmentos y la ley de los cosenos. El angulo del humero es del mundo, como los de la coreografia: la rotacion local
        sale de restarle el giro del tronco (resuelve_pose). El codo queda del lado de atras de la recta hombro-mano (un codo
        solo flexiona hacia delante). Si el objetivo no se alcanza, el brazo se estira todo lo que puede hacia el.
        """
        br = self.brazo[lado]
        l1, l2 = br["l1"], br["l2"]
        v = _menos(objetivo, hombro)
        d = min(max(math.hypot(v[0], v[1]), abs(l1 - l2) + 1e-3), 0.995 * (l1 + l2))
        th = _angulo(v)
        psi = math.degrees(math.acos(max(-1.0, min(1.0, (l1 * l1 + d * d - l2 * l2) / (2.0 * l1 * d)))))
        interior = math.degrees(math.acos(max(-1.0, min(1.0, (l1 * l1 + l2 * l2 - d * d) / (2.0 * l1 * l2)))))
        humero = th - psi                       # el angulo efectivo del humero
        flexion = 180.0 - interior              # entre el humero y el antebrazo
        return humero - br["a1"], flexion - (br["a2"] - br["a1"])


def resuelve_pose(g, m, reposo_codo):
    """
    De los OBJETIVOS de un instante (grados del MUNDO) a las rotaciones LOCALES de los huesos de perfil y al desplazamiento de Tronco. «m»:
      tau            giro de Tronco (- = hacia delante)         mc, ml     angulo del muslo cercano y del lejano (+ = hacia delante)
      fc, fl         flexion de las rodillas (>= 0)             bc, bl     angulo del humero cercano y del lejano
      ec, el         flexion de los codos (>= 0)                n, h       cuello y cabeza, locales
      extra          px que se SUBE el cuerpo (el salto de correr)         vol    escala Y de Perfil (X la compensa)
      pie_x          "c" o "l": el pie que no resbala (agachadas, arrodillado); sin el, Tronco no se desplaza en X
      mano_c/mano_l  objetivo de la mano (punto del lienzo o funcion de los puntos del cuerpo ya plantado), con peso_c/peso_l (0 a 1) que mezcla
                     la cinematica inversa con el angulo pedido en bc/ec
    El desplazamiento de Tronco PLANTA los pies (GeoPerfil.baja). Devuelve (loc, dx, dy): loc con T, pc, pl, kc, kl, bc, bl, ec, el, n, h; dy es lo que BAJA.
    """
    tau = m.get("tau", 0.0)
    loc = {"T": tau, "pc": m.get("mc", 0.0) - tau, "pl": m.get("ml", 0.0) - tau, "kc": -m.get("fc", 0.0), "kl": -m.get("fl", 0.0),
           "n": m.get("n", 0.0), "h": m.get("h", 0.0), "P": m.get("P", 0.0)}
    dy = g.baja(loc) - m.get("extra", 0.0)
    dx = g.corre_x(loc, m["pie_x"]) if m.get("pie_x") else 0.0
    pts = g.puntos(loc, (dx, dy))
    for lado in ("c", "l"):
        b, e = m.get("b" + lado, 0.0), m.get("e" + lado, reposo_codo)
        objetivo = m.get("mano_" + lado)
        if objetivo is not None:
            peso = m.get("peso_" + lado, 1.0)
            punto_ = objetivo(pts) if callable(objetivo) else objetivo
            bi, ei = g.ik(lado, pts["hombro_" + lado], punto_)
            b, e = b + peso * (bi - b), e + peso * (ei - e)
        loc["b" + lado], loc["e" + lado] = b - tau, e
    return loc, dx, dy


def emite_perfil(x, s, largo, f, paso):
    """
    Muestrea f(t) (t en segundos base) cada «paso» y escribe en el clip «s» las curvas del cuerpo de perfil: la rotacion de las 12 articulaciones, el
    desplazamiento de Tronco que planta los pies (POSX y POSY, si no es cero) y el squash & stretch de Perfil (ESCX y ESCY, si no es 1). Una curva
    que no varia se escribe con dos claves. El desplazamiento de Tronco se muestrea el TRIPLE de denso (DENSIDAD_PLANTA) que las rotaciones: el
    «el punto mas bajo de dos piernas» tiene picos en cada cruce de piernas y una curva suave de pocas claves los redondearia, hundiendo el pie unos px.
    En un bucle f debe ser periodica en «largo», y cierra sola. Devuelve la lista de (t, loc, dx, dy) de todas las muestras.
    """
    g = x.perfil
    codo = reposo_perfil(x)[PEC]
    n = max(1, int(round(largo / paso)))
    paso_ = DENSIDAD_PLANTA
    tiempos = [min(largo, i * largo / (n * paso_)) for i in range(n * paso_ + 1)]
    cols = {}
    muestras = []

    def pon(ruta, prop, i, v):
        cols.setdefault((ruta, prop), []).append((i, v))

    for i, t in enumerate(tiempos):
        m = f(t)
        loc, dx, dy = resuelve_pose(g, m, codo)
        muestras.append((t, loc, dx, dy))
        pon(PT, POSX, i, dx)
        pon(PT, POSY, i, -dy)
        if i % paso_:
            continue      # el resto de las curvas, a la densidad de «paso»
        pon(PF, ROT, i, loc["P"])
        pon(PT, ROT, i, loc["T"])
        pon(PLC, ROT, i, loc["pc"])
        pon(PLL, ROT, i, loc["pl"])
        pon(PKC, ROT, i, loc["kc"])
        pon(PKL, ROT, i, loc["kl"])
        pon(PBC, ROT, i, loc["bc"])
        pon(PBL, ROT, i, loc["bl"])
        pon(PEC, ROT, i, loc["ec"])
        pon(PEL, ROT, i, loc["el"])
        pon(PNK, ROT, i, loc["n"])
        pon(PHD, ROT, i, loc["h"])
        v = m.get("vol", 1.0)
        pon(PF, ESCY, i, v)
        pon(PF, ESCX, i, 1.0 - 0.5 * (v - 1.0))
    for (ruta, prop), pares in cols.items():
        vs = [v for _, v in pares]
        if prop in (POSX, POSY) and max(abs(v) for v in vs) < 1e-3:
            continue
        if prop in (ESCX, ESCY) and all(abs(v - 1.0) < 1e-4 for v in vs):
            continue
        if max(vs) - min(vs) < 1e-4:
            tv = [0.0, vs[0], largo, vs[0]]
        else:
            tv = []
            for i, v in pares:
                tv += [tiempos[i], v]
        s.raw(ruta, prop, *tv)
    return muestras


# ---------------------------------------------------------------------------- las siete acciones de perfil


def _punto_muslo(g, pts, lado, frac=0.6, arriba=0.5):
    """El punto del lienzo sobre el muslo de ese lado, a «frac» del camino de la cadera a la rodilla y «arriba» grosores por encima del muslo: donde descansa una mano."""
    a, b = pts["cadera_" + lado], pts["rodilla_" + lado]
    vx, vy = b[0] - a[0], b[1] - a[1]
    largo = max(math.hypot(vx, vy), 1e-6)
    nx, ny = vy / largo, -vx / largo
    if ny > 0.0:
        nx, ny = -nx, -ny
    r = arriba * g.pierna[lado]["grosor"]
    return (a[0] + frac * vx + nx * r, a[1] + frac * vy + ny * r)


def perfil_ciclo(x, s, par):
    """
    Caminar, correr, cargar y empujar de perfil: un paso con dos zancadas por ciclo (el mismo ciclo de duracion que el clip de frente de la misma
    accion). Cada pierna es una cosenoidal de cadera centrada en par[c] con amplitud par[A] —adelante del todo a fase 0, atras a fase 0,5— con una
    flexion de rodilla en el balanceo (par[Fsw], cuando la pierna vuelve hacia delante) y una pequeña de apoyo (par[Fst]) tras tocar el suelo; la otra
    pierna va a contrafase. Los brazos se contraponen a las piernas (el cercano va atras cuando la pierna cercana va adelante) y el codo llega
    tarde al brazo (3 cuadros) y flexiona mas cuanto mas adelante va; con par[brazos] = (humero, flexion, oscilacion) quedan fijos delante, para
    cargar y empujar. El tronco se inclina hacia delante (par[lean]) y las piernas lo compensan, porque cuelgan de el; la cabeza se queda mas erguida
    y llega tarde. El cuerpo baja solo al abrir las piernas (los pies se plantan) y, al correr, ademas salta (par[hop]). Squash & stretch en Perfil.
    """
    k = x.k
    largo = s.largo / k
    e0 = reposo_perfil(x)[PEC] if par.get("e0") is None else par["e0"]

    def lag(cuadros):
        return cuadros / FPS / k          # cuadros -> segundos BASE

    def tau(t):
        return par["lean"] - par["osc"] * math.cos(TAU * 2.0 * t / largo)

    def pierna(v):
        v %= 1.0
        sw_c, sw_w = par.get("sw", (0.70, 0.25))     # centro y semiancho (en fracciones de ciclo) de la flexion del balanceo
        return (par["c"] + par["A"] * math.cos(TAU * v), par["Fst"] * bump(v, 0.11, 0.11) + par["Fsw"] * bump(v, sw_c, sw_w))

    def f(t):
        u = t / largo
        fase = TAU * u
        mc, fc = pierna(u)
        ml, fl = pierna(u + 0.5)
        m = {"tau": tau(t), "mc": mc, "fc": fc, "ml": ml, "fl": fl,
             "n": -par["nk"] * tau(t - lag(2)), "h": -par["hk"] * tau(t - lag(4)),
             "vol": 1.0 - par["sq"] * math.cos(2.0 * fase)}
        if par.get("brazos"):
            humero, flexion, o = par["brazos"]
            m["bc"], m["bl"] = humero + o * math.sin(2.0 * fase), humero + o * math.sin(2.0 * fase + 0.8)
            m["ec"], m["el"] = flexion + 0.5 * o * math.cos(2.0 * fase), flexion + 0.5 * o * math.cos(2.0 * fase + 0.8)
        else:
            tarde = fase - TAU * lag(3) / largo
            m["bc"], m["bl"] = par["b0"] - par["B"] * math.cos(fase), par["b0"] + par["B"] * math.cos(fase)
            m["ec"] = e0 + par["e1"] * 0.5 * (1.0 - math.cos(tarde))
            m["el"] = e0 + par["e1"] * 0.5 * (1.0 + math.cos(tarde))
        if par.get("hop"):
            m["extra"] = par["hop"] * 0.5 * (1.0 + math.cos(2.0 * TAU * (u - 0.42)))   # el vuelo: dos por ciclo, entre un apoyo y el siguiente
        return m

    return emite_perfil(x, s, largo, f, largo / 20.0)


def par_walk(a):
    return dict(c=4.0, A=19.0 * a, Fsw=56.0, Fst=7.0, lean=-4.0 * a, osc=0.8, b0=3.0, B=18.0 * a, e0=None, e1=20.0, sq=0.015 * a, nk=0.45, hk=0.30)


def par_run(a):
    return dict(c=2.0, A=28.0 * a, Fsw=85.0, Fst=18.0, sw=(0.68, 0.30), lean=-8.0 * a, osc=1.2, b0=8.0, B=38.0, e0=70.0, e1=20.0, hop=14.0 * a, sq=0.04 * a, nk=0.50, hk=0.35)


def par_carry(a):
    # el paso de caminar, mas corto, con los brazos recogidos delante del pecho sosteniendo la carga: humero 40, antebrazo a 118 del suelo
    return dict(c=3.0, A=12.0 * a, Fsw=44.0, Fst=5.0, lean=-2.0 * a, osc=0.6, brazos=(40.0, 78.0, 3.0), sq=0.01 * a, nk=0.45, hk=0.30)


def par_push(a):
    # inclinado hacia delante, las piernas echadas hacia atras (la de atras empuja) y los brazos tensos casi horizontales con los puños a la altura del pecho
    return dict(c=-8.0, A=11.0 * a, Fsw=46.0, Fst=8.0, lean=-13.0, osc=1.0, brazos=(82.0, 6.0, 3.0), sq=0.01 * a, nk=0.55, hk=0.40)


def _m_agachada(g, w, profundidad, canilla, tau, n, h, vol):
    """
    Los objetivos de una agachada con los DOS pies en el suelo, sin manos (perfil_agachada los añade): la cadera baja profundidad * w de la altura de la
    cadera al suelo, con las canillas inclinadas «canilla» grados hacia atras (la lejana, 6 mas) y el muslo que haga falta (GeoPerfil.resuelve_muslo); el pie
    cercano no resbala (pie_x: Tronco POSX).
    """
    d = profundidad * g.y_reposo["c"] * w
    gc, gl = -canilla * w, -(canilla + 6.0) * w
    mc = g.resuelve_muslo("c", gc, g.y_reposo["c"] - d)
    ml = g.resuelve_muslo("l", gl, g.y_reposo["l"] - d)
    return {"tau": tau, "mc": mc, "fc": mc - gc, "ml": ml, "fl": ml - gl, "n": n, "h": h, "vol": vol, "pie_x": "c"}


def perfil_agachada(x, s, profundidad, canilla, tau_f, mano_c, mano_l, cabeza_f, vol_f, paso, w_f=None):
    """
    Agachada de perfil con los DOS pies en el suelo (soplar y recoger). «w_f(t)» (0 a 1, por defecto 1) es cuanto esta agachado (_m_agachada); tau_f(t) es el
    giro del tronco, cabeza_f(t) devuelve (cuello, cabeza), vol_f(t) la escala Y y mano_c/mano_l(t) -> (objetivo, peso) de las manos (o None).
    """
    g = x.perfil
    largo = s.largo / x.k
    w_f = w_f or (lambda t: 1.0)

    def f(t):
        n, h = cabeza_f(t)
        m = _m_agachada(g, w_f(t), profundidad, canilla, tau_f(t), n, h, vol_f(t))
        for lado, mano in (("c", mano_c), ("l", mano_l)):
            if mano is not None:
                objetivo, peso = mano(t)
                m["mano_" + lado], m["peso_" + lado] = objetivo, peso
        return m

    return emite_perfil(x, s, largo, f, paso)


def _monton(g, m_max, fraccion):
    """
    El monton del suelo (fuego, objeto), FIJO: a «fraccion» del alcance horizontal del hombro del personaje agachado del todo (m_max, los objetivos de
    _m_agachada a la profundidad maxima), a ras de suelo (la mano a 0,04 de la figura sobre el). Se mide con el hombro y no a un tanto de la altura:
    un brazo corto lo tiene mas cerca que uno largo y ninguno se estira mas de lo que puede.
    """
    loc, dx, dy = resuelve_pose(g, m_max, 0.0)
    hombro = g.puntos(loc, (dx, dy))["hombro_c"]
    alto_mano = g.suelo - 0.04 * g.alto
    br = g.brazo["c"]
    alcance_x = math.sqrt(max((0.97 * (br["l1"] + br["l2"])) ** 2 - (alto_mano - hombro[1]) ** 2, 0.0))
    return (hombro[0] + fraccion * alcance_x, alto_mano)


def perfil_blow(x, s):
    """
    Soplar sobre el monton (0,9 s): agachado (el 22 % de la altura de la cadera al suelo), el tronco muy inclinado hacia delante, la cabeza hacia el fuego, los
    brazos estirados hacia delante y abajo hasta cerca del suelo, y tres soplos (a 0,18, 0,46 y 0,74 s): inspira (el pecho sube un poco, la cabeza se alza) y
    sopla (el pecho baja y la cabeza se echa hacia delante). El pie cercano no resbala.
    """
    g = x.perfil
    profundidad, canilla, tau_base = 0.22, 14.0, -32.0
    centros = (0.18, 0.46, 0.74)
    llena = lambda t: sum(bump(t, c - 0.08, 0.08) for c in centros)
    sopla = lambda t: sum(bump(t, c + 0.03, 0.09) for c in centros)
    fuego = _monton(g, _m_agachada(g, 1.0, profundidad, canilla, tau_base, 0.0, 0.0, 1.0), 0.75)
    mano_c = lambda t: (fuego, 1.0)
    mano_l = lambda t: ((fuego[0] - 0.02 * g.alto, fuego[1]), 1.0)
    return perfil_agachada(
        x, s, profundidad, canilla,
        lambda t: tau_base + 2.0 * llena(t) - 4.0 * sopla(t),
        mano_c, mano_l,
        lambda t: (-10.0 + 5.0 * llena(t) - 8.0 * sopla(t), -4.0 - 2.0 * sopla(t)),
        lambda t: 1.0 + 0.03 * llena(t) - 0.05 * sopla(t), 0.025)


def perfil_pickup(x, s):
    """
    Recoger (1,4 s): un respingo de anticipacion (0,1 s: el pecho sube y el tronco se echa un poco atras), se agacha hasta el monton del suelo (0,4 s), tira del
    objeto con el brazo cercano (la mano llega al suelo por cinematica inversa, el lejano la acompaña), lo sostiene (0,7 s) y se levanta con el objeto contra
    el vientre (1,1 s) hasta volver al reposo (1,4 s). Los pies no resbalan. El monton esta en el suelo, FIJO (_monton).
    """
    g = x.perfil
    F = g.alto
    profundidad, canilla, tau_max = 0.27, 12.0, -42.0

    def w(t):
        return suave(t, 0.10, 0.40) * (1.0 - suave(t, 0.70, 1.10))

    monton = _monton(g, _m_agachada(g, 1.0, profundidad, canilla, tau_max, 0.0, 0.0, 1.0), 0.9)

    def vientre(pts):
        return (pts["cadera_c"][0] + 0.17 * F, pts["cadera_c"][1] - 0.08 * F)

    def objetivo(t):
        paso_ = suave(t, 0.70, 1.15)
        return lambda pts: tuple(a + paso_ * (b - a) for a, b in zip(monton, vientre(pts)))

    def peso(t):
        return suave(t, 0.10, 0.32) * (1.0 - suave(t, 1.15, 1.38))

    mano_c = lambda t: (objetivo(t), peso(t))
    mano_l = lambda t: (objetivo(t), peso(t))
    ant = lambda t: bump(t, 0.06, 0.06)
    return perfil_agachada(
        x, s, profundidad, canilla,
        lambda t: tau_max * w(t) + 3.0 * ant(t),
        mano_c, mano_l,
        lambda t: (-6.0 * w(t), -3.0 * w(t)),
        lambda t: 1.0 + 0.02 * ant(t) - 0.035 * w(t), 0.05, w_f=w)


def perfil_kneel(x, s):
    """
    Arrodillado (3 s, bucle): una rodilla en el suelo —la lejana, con la canilla hacia atras y arriba y la punta del pie apoyada— y la otra pierna
    adelante con el pie plano y el muslo casi horizontal (GeoPerfil.canilla_apoyada y resuelve_muslo lo calculan de las longitudes), el tronco algo
    inclinado hacia delante y respirando, una mano sobre la rodilla de delante y la otra colgando. Se sostiene la pose: solo respira. El pie cercano no resbala.
    """
    g = x.perfil
    L = s.largo / x.k
    ml = -8.0
    sg = g.canilla_apoyada("l", ml)
    fl = ml - sg
    y_lejana = g.y_apoyo("l", ml, sg)
    gc = -10.0
    mc = g.resuelve_muslo("c", gc, y_lejana)
    fc = mc - gc

    def f(t):
        respira = math.sin(TAU * t / L)
        return {"tau": -6.0 + 1.2 * respira, "mc": mc, "fc": fc, "ml": ml, "fl": fl,
                "n": -3.0 - 0.8 * respira, "h": -1.5 * respira, "vol": 1.0 + 0.012 * respira, "pie_x": "c",
                # la mano cercana descansa sobre la rodilla de la pierna de delante; la lejana cuelga relajada, un poco hacia delante
                "mano_c": lambda pts: _punto_muslo(g, pts, "c", 1.0, 0.55), "bl": 6.0 + 1.5 * respira, "el": reposo_perfil(x)[PEL] + 14.0 + 3.0 * respira}

    return emite_perfil(x, s, L, f, 0.25)


PERFIL_ACCION = {
    "Walk": lambda x, s: perfil_ciclo(x, s, par_walk(x.a)),
    "Run": lambda x, s: perfil_ciclo(x, s, par_run(x.a)),
    "Carry": lambda x, s: perfil_ciclo(x, s, par_carry(x.a)),
    "Push": lambda x, s: perfil_ciclo(x, s, par_push(x.a)),
    "PickUp": perfil_pickup,
    "Kneel": perfil_kneel,
    "Blow": perfil_blow,
}


def perfil_clip(x, s):
    """Añade al clip «s» la coreografia de perfil si su accion es de ACCIONES_PERFIL y el personaje tiene perfil; si no, no hace nada (Spec.finish deja los 12 huesos en reposo)."""
    f = PERFIL_ACCION.get(s.accion)
    if f is not None and x.perfil is not None:
        f(x, s)
    return s


def clips_familia(x):
    lista = _clips_familia(x)
    for sp in lista:
        plantar(x, sp)
        perfil_clip(x, sp)   # INC-134: el cuerpo de perfil, tras el de frente (que no se toca)
    return lista


def _clips_familia(x):
    lista = [idle(x), walk(x), run(x), talk(x), strike(x), hammer(x), blow(x), pickup(x), kneel(x), carry(x), push(x),
             point(x), observe(x), celebrate(x), encourage(x), hug(x), surprise(x), sleep(x)]
    visibilidad(x, lista)
    return lista




# ---------------------------------------------------------------------------- Algoritm: 10 clips (los 9 de siempre y Wave, el saludo, 08/10/2026)
#
# Una llama con extremidades que flota. Conserva la flotacion senoidal, el giro de Spin sobre Cuerpo, el alfa
# y los nombres de siempre. Sus brazos son palitos que salen de los costados del vientre y sus MANOS van por ENCIMA de
# la cara (decision de Santiago, 05/10/2026: «orden_tronco» = Torso, Ojos, Boca, BrazoIzq, BrazoDer): lo que cruce los ojos o
# la boca los tapa de verdad, asi que ningun gesto pasa por ellos —pose_preview.py exige el 99,5 % de la caja de ojos y boca,
# ampliada un 30 %, sin brazo encima—. Con el sprite entero actual lo visible no cambia; la prueba usa una maqueta recortada.


def guia_idle(x):
    """
    Flota y se mece (ciclo de 2 s, 13.3). La llama parpadea: se estira y se aplasta a destiempo del vaiven, y
    los brazos ondean alternados como lenguas de fuego (el antebrazo llega 3 cuadros tarde).
    """
    s = guia_spec("Idle", "flotar", 2.0)
    s.raw(C, POSY, 0, 0, 1, 24, 2, 0).rot(C, 0, -3, 1, 3, 2, -3)
    # parpadeo de llama: Cuerpo se estira cuando sube y Tronco se estira a otro ritmo (a destiempo)
    s.vol(0, 1.0, 0.35, 1.045, 0.85, 0.972, 1.35, 1.04, 1.75, 0.985, 2.0, 1.0)
    s.raw(T, ESCY, 0, 1, 0.55, 1.035, 1.0, 0.98, 1.5, 1.03, 2.0, 1)
    # brazos que ondean, en oposicion
    s.rot(LA, 0, 0, 0.5, -9, 1.0, 0, 1.5, 9, 2.0, 0)
    s.rot(RA, 0, 0, 0.5, 9, 1.0, 0, 1.5, -9, 2.0, 0)
    s.follow(LA, ROT, LE, 3, 1.0)
    s.follow(RA, ROT, RE, 3, 1.0)
    for ruta, delta in ((LE, 7.0), (RE, -7.0)):  # los codos siempre algo plegados hacia dentro: nunca se hiperextienden
        for k in s._get(ruta, ROT).claves:
            k[1] += delta
    cuelga_piernas(s, 12)
    s.follow(C, ROT, T, 2, 0.4)
    return s


def guia_wave(x):
    """
    Saluda (D12, Santiago, 08/10/2026: la pantalla de creditos). El brazo derecho de la pantalla sube, la mano se mece de un lado a otro unos cuatro vaivenes con el
    antebrazo casi vertical, el brazo baja y el clip descansa antes de volver a empezar: un bucle de 4,8 s con una pausa de reposo de 0,9 s dentro, para que el saludo
    no sea un tic continuo. El codo se dobla antes de que el hombro llegue arriba (no se estira horizontal) y la mano no pasa nunca por la cara (pose_preview exige libre el
    99,5 % de la caja de ojos y boca ampliada un 30 %: con el hombro a mas de 98 grados o el codo a 27 +- 18 el antebrazo ya la roza). El cuerpo flota como en Idle (dos
    vueltas de 2,4 s) y se inclina un poco hacia la mano. Es una accion solo del guia, como Spin: la familia no la tiene.
    """
    largo = 4.8
    s = guia_spec("Wave", "saludar", largo)
    alza = lambda t: suave(t, 0.0, 0.55) * (1.0 - suave(t, 3.35, 3.95))                    # 0 = el hombro abajo, 1 = arriba
    dobla = lambda t: suave(t, 0.0, 0.38) * (1.0 - suave(t, 3.55, 3.98))                   # el codo se dobla antes de que suba el hombro y se estira al final
    mece = lambda t: math.sin(TAU * (t - 0.55) / 0.7) * suave(t, 0.5, 0.95) * (1.0 - suave(t, 3.0, 3.4))   # el vaiven de la mano: 0,7 s por vaiven, con rampa de entrada y de salida
    muestrea(s, C, POSY, lambda t: 12.0 * (1.0 - math.cos(TAU * t / 2.4)), largo, 0.05)
    muestrea(s, RA, ROT, lambda t: 94.0 * alza(t) + 3.0 * mece(t), largo, 0.05)               # el humero a unos 139 grados de la vertical (el prefab lo trae a 45)
    muestrea(s, RE, ROT, lambda t: -7.0 * (1.0 - dobla(t)) + (27.0 + 16.0 * mece(t)) * dobla(t), largo, 0.05)   # abajo, algo plegado hacia dentro como en Idle; arriba, 27 +- 16 grados
    muestrea(s, C, ROT, lambda t: alza(t) * (-2.5 + 1.2 * mece(t)), largo, 0.05)
    cuelga_piernas(s, 12)
    s.follow(C, POSY, LA, 4, -0.4, 12)                  # el brazo que no saluda cuelga y sigue la flotacion, con su codo plegado hacia dentro
    s.follow(LA, ROT, LE, 3, 0.8)
    for k in s._get(LE, ROT).claves:
        k[1] += 7.0
    s.follow(C, ROT, T, 2, 0.4)
    return s


def clips_guia(x):
    lista = [guia_idle(x)]

    talk_ = guia_spec("Talk", "hablar", 1.0)
    (talk_.raw(C, POSY, 0, 0, 0.5, 18, 1, 0)
     .raw(C, ESCX, 0, 1, 0.25, 1.05, 0.5, 1, 0.75, 1.05, 1, 1)
     .raw(C, ESCY, 0, 1, 0.25, 1.05, 0.5, 1, 0.75, 1.05, 1, 1)
     .rot(T, 0, 0, 0.25, 2, 0.5, 0, 0.75, -2, 1, 0)
     .raw(T, ESCY, 0, 1, 0.25, 1.03, 0.5, 1, 0.75, 1.03, 1, 1)
     .sym(LA, RA, 0, 0, 0.25, 6, 0.5, 0, 0.75, 6, 1, 0)
     .sym(LE, RE, 0, 0, 0.25, 8, 0.5, 0, 0.75, 8, 1, 0))
    cuelga_piernas(talk_, 9)
    lista.append(talk_)

    # Señala hacia la derecha de la pantalla: el brazo derecho casi horizontal (el cuerpo ya se inclina -10).
    point_ = guia_spec("Point", "senalar", 1.2)
    (point_.rot(C, 0, -10, 0.6, -14, 1.2, -10).raw(C, POSY, 0, 10, 0.6, 24, 1.2, 10)
     .rot(RA, 0, 60, 0.6, 56, 1.2, 60)
     .rot(RE, 0, 3, 0.6, 6, 1.2, 3)
     .rot(LA, 0, 4, 0.6, 8, 1.2, 4)
     .rot(LE, 0, -4, 0.6, -6, 1.2, -4))
    cuelga_piernas(point_, 17)
    point_.follow(C, ROT, T, 2, 0.4)
    lista.append(point_)

    # Gira como un trompo feliz (1.4.1): los brazos y las piernas salen despedidos.
    spin = guia_spec("Spin", "girar", 1.0)
    (spin.rot(C, 0, 0, 1, -360).raw(C, POSY, 0, 0, 0.5, 20, 1, 0)
     .sym(LA, RA, 0, 0, 0.15, 35, 0.85, 35, 1, 0)
     .sym(LE, RE, 0, 0, 0.2, 10, 0.8, 10, 1, 0)
     .sym(LL, RL, 0, 0, 0.15, 18, 0.85, 18, 1, 0)
     .sym(LK, RK, 0, 0, 0.2, -8, 0.8, -8, 1, 0))
    lista.append(spin)

    cel = guia_spec("Celebrate", "celebrar", 1.2)
    (cel.rot(C, 0, 0, 1.2, -360)
     .raw(C, ESCX, 0, 1, 0.3, 1.08, 0.6, 1, 0.9, 1.08, 1.2, 1)
     .raw(C, ESCY, 0, 1, 0.3, 1.08, 0.6, 1, 0.9, 1.08, 1.2, 1)
     .sym(LA, RA, 0, 100, 0.3, 85, 0.6, 100, 0.9, 85, 1.2, 100)
     .sym(LE, RE, 0, 12, 0.3, 30, 0.6, 12, 0.9, 30, 1.2, 12)
     .sym(LL, RL, 0, 10, 0.3, 18, 0.6, 10, 0.9, 18, 1.2, 10)
     .sym(LK, RK, 0, -6, 0.3, -14, 0.6, -6, 0.9, -14, 1.2, -6)
     .rot(T, 0, 0, 0.3, 2, 0.6, 0, 0.9, -2, 1.2, 0))
    lista.append(cel)

    # Animo (CP-02: calido): se hunde un poco (anticipacion), salta y el puño sube POR FUERA de la silueta de la
    # llama, por encima del hombro, con un bombeo; rebota, asiente con el tronco y la otra mano queda abierta
    # y relajada. 0,9 s como siempre (los controladores lo reproducen un tiempo fijo): anticipacion y rebote
    # cortos, y el asentamiento (de 0,5 a 0,9) entero.
    enc = guia_spec("Encourage", "animo", 0.9)
    (enc.raw(C, POSY, 0, 0, 0.14, -14, 0.34, 34, 0.48, 6, 0.62, 14, 0.9, 0)
     .raw(C, ESCY, 0, 1, 0.14, 0.96, 0.34, 1.06, 0.48, 0.985, 0.9, 1)
     .raw(T, ESCY, 0, 1, 0.14, 0.97, 0.34, 1.03, 0.48, 0.98, 0.9, 1)   # el asentimiento del tronco: baja y sube
     .rot(C, 0, 0, 0.34, -2, 0.9, 0)
     .rot(RA, 0, 0, 0.14, -6, 0.25, 70, 0.34, 96, 0.42, 88, 0.50, 98, 0.66, 40, 0.9, 0)
     .rot(RE, 0, 0, 0.14, 4, 0.34, 26, 0.42, 34, 0.50, 24, 0.9, 0)
     .rot(LA, 0, 0, 0.34, -7, 0.9, 0)
     .rot(LE, 0, 0, 0.34, 6, 0.9, 0))
    cuelga_piernas(enc, 15)
    for ruta, delta in ((LK, 4.0), (RK, -4.0)):  # las rodillas siempre algo hacia dentro: el rebote no las dobla al reves
        for k in enc._get(ruta, ROT).claves:
            k[1] += delta
    lista.append(enc)

    lista.append(guia_spec("Hidden", "oculto", 1.0).raw("Lienzo", ALFA, 0, 0.0, 1, 0.0))
    ap = guia_spec("Appear", "aparicion", 1.0, False)
    (ap.raw("Lienzo", ALFA, 0, 0.0, 0.5, 1.0, 1, 1.0)
     .raw(C, ESCX, 0, 0.3, 0.7, 1.06, 1, 1.0)
     .raw(C, ESCY, 0, 0.3, 0.7, 1.06, 1, 1.0)
     .sym(LA, RA, 0, 0, 0.7, 8, 0.85, -2, 1, 0))
    lista.append(ap)
    lista.append(guia_spec("Vanish", "apagado", 1.6, False)
                 .raw("Lienzo", ALFA, 0, 1.0, 0.3, 0.35, 0.6, 1.0, 0.9, 0.35, 1.2, 1.0, 1.6, 0.0)
                 .raw(C, ESCX, 0, 1.0, 1.2, 1.0, 1.6, 0.6)
                 .raw(C, ESCY, 0, 1.0, 1.2, 1.0, 1.6, 0.6))
    lista.append(guia_wave(x))
    return lista




# ============================================================================ 3. ESCRITURA Y VALIDACION


def redondea(v, n=5):
    r = round(v, n)
    return 0.0 if r == 0 else r  # sin «-0.0»


def clip_a_json(spec, avisos):
    """Un clip de Spec al contrato del JSON (claves ordenadas, tiempos a 4 decimales, dentro de [0, duracion])."""
    avisos.extend(spec.finish())
    duracion = redondea(spec.largo, 4)
    curvas = []
    for c in spec.orden:
        claves = []
        for t, v in c.claves:
            t4 = clamp(redondea(t, 4), 0.0, duracion)
            if claves and abs(claves[-1][0] - t4) < 1e-9:
                claves[-1] = [t4, redondea(v)]
            else:
                claves.append([t4, redondea(v)])
        curvas.append({
            "ruta": c.ruta,
            "tipo": "CanvasGroup" if c.prop == ALFA else "RectTransform",
            "propiedad": c.prop,
            "claves": claves,
        })
    return {"archivo": spec.archivo, "accion": spec.accion, "duracion": duracion, "bucle": spec.bucle, "curvas": curvas}


def construir(rig=None, familia=clips_familia, guia=clips_guia, maqueta=False):
    """
    Todos los clips: devuelve (documento JSON, avisos). Con el arte que HAY (el de los prefabs) es lo que se
    escribe en clips_personajes.json. Con «maqueta» los tres de arte provisional se calculan sobre el arte
    final SIMULADO (maqueta.py: brazos, piernas y cabeza partidos): es lo que pose_preview.py prueba para
    saber que la misma coreografia, con el arte final, tambien pasa.
    """
    rig = rig or P.cargar_rig()
    avisos = []
    personajes = []
    for pid in P.FAMILIA:
        if maqueta:
            import maqueta as _mq
            arbol, piezas = _mq.arbol_maqueta(pid, rig)
            x = leer_contexto(pid, rig, arbol, piezas)
        else:
            x = leer_contexto(pid, rig)
        clips = [clip_a_json(s, avisos) for s in familia(x)]
        personajes.append({
            "id": pid,
            "carpeta": "Assets/Game/Art/Characters/%s/Animations" % FAMILIA[pid].carpeta,
            "clips": clips,
        })
    x = leer_contexto("algoritm_fuego", rig)
    personajes.append({
        "id": "algoritm",
        "carpeta": "Assets/Game/Art/Characters/Algoritm/Animations",
        "clips": [clip_a_json(s, avisos) for s in guia(x)],
    })
    return {"version": 1, "generado_por": "coreografia.py", "personajes": personajes}, avisos


def _j(v):
    return json.dumps(v, ensure_ascii=True)


def _num(v):
    return _j(float(v) if isinstance(v, float) else v)


def _curva(c):
    claves = ", ".join("[%s, %s]" % (_num(t), _num(v)) for t, v in c["claves"])
    return '{"ruta": %s, "tipo": %s, "propiedad": %s, "claves": [%s]}' % (_j(c["ruta"]), _j(c["tipo"]), _j(c["propiedad"]), claves)


def serializa(doc):
    """
    El documento como texto: una curva por linea (el diff de un retoque se lee), indentado y solo ASCII
    (ensure_ascii): el lector de C# es propio y los datos no llevan tildes.
    """
    out = ['{', '  "version": %s,' % _j(doc["version"]), '  "generado_por": %s,' % _j(doc["generado_por"]), '  "personajes": [']
    pers = []
    for p in doc["personajes"]:
        clips = []
        for c in p["clips"]:
            curvas = ",\n".join("          " + _curva(k) for k in c["curvas"])
            clips.append('        {"archivo": %s, "accion": %s, "duracion": %s, "bucle": %s,\n         "curvas": [\n%s\n         ]}' % (
                _j(c["archivo"]), _j(c["accion"]), _num(c["duracion"]), _j(c["bucle"]), curvas))
        pers.append('    {\n      "id": %s,\n      "carpeta": %s,\n      "clips": [\n%s\n      ]}' % (
            _j(p["id"]), _j(p["carpeta"]), ",\n".join(clips)))
    out.append(",\n".join(pers))
    out += ['  ]', '}']
    return "\n".join(out) + "\n"


def escribe(doc, ruta=SALIDA):
    with open(ruta, "w", encoding="ascii", newline="\n") as f:
        f.write(serializa(doc))


def valida(doc, ruta=SALIDA):
    """Comprueba el contrato; devuelve la lista de problemas (vacia = bien)."""
    errores = []
    try:
        with open(ruta, "rb") as f:
            f.read().decode("ascii")
    except UnicodeDecodeError as e:
        errores.append("el JSON no es ASCII puro: %s" % e)
    for p in doc["personajes"]:
        huesos = HUESOS_GUIA if p["id"] == "algoritm" else HUESOS_FAMILIA
        carpeta = os.path.join(P.RAIZ, p["carpeta"])
        archivos = set()
        for clip in p["clips"]:
            ctx = "%s/%s" % (p["id"], clip["archivo"])
            if clip["archivo"] in archivos:
                errores.append("%s: clip repetido" % ctx)
            archivos.add(clip["archivo"])
            if not os.path.isfile(os.path.join(carpeta, clip["archivo"] + ".anim")):
                errores.append("%s: no existe el .anim en %s" % (ctx, p["carpeta"]))
            rutas_rot = set()
            for c in clip["curvas"]:
                if c["propiedad"] not in PROPIEDADES:
                    errores.append("%s: propiedad no permitida %s" % (ctx, c["propiedad"]))
                if (c["propiedad"] == ALFA) != (c["tipo"] == "CanvasGroup"):
                    errores.append("%s: tipo %s para %s" % (ctx, c["tipo"], c["propiedad"]))
                ts = [k[0] for k in c["claves"]]
                if ts != sorted(ts) or len(set(ts)) != len(ts):
                    errores.append("%s %s: claves desordenadas o repetidas" % (ctx, c["ruta"]))
                if ts and (min(ts) < 0 or max(ts) > clip["duracion"] + 1e-9):
                    errores.append("%s %s: claves fuera de [0, duracion]" % (ctx, c["ruta"]))
                if c["propiedad"] == ROT:
                    rutas_rot.add(c["ruta"])
                if clip["bucle"] and c["propiedad"] != ALFA and len(c["claves"]) > 1:
                    a, b = c["claves"][0][1], c["claves"][-1][1]
                    if c["propiedad"] == ROT:
                        b = a + (((b - a) + 180.0) % 360.0 - 180.0)
                    if abs(a - b) > 1e-3:
                        errores.append("%s %s %s: el bucle no cierra" % (ctx, c["ruta"], c["propiedad"]))
            falta = [h for h in huesos if h not in rutas_rot]
            if falta:
                errores.append("%s: sin rotacion en %s" % (ctx, falta))
            if p["id"] != "algoritm":   # INC-134: cada clip de la familia (de frente o de perfil) fija las 12 articulaciones de perfil: ninguna cae a la pose en T
                falta_p = [h for h in HUESOS_PERFIL if h not in rutas_rot]
                if falta_p:
                    errores.append("%s: sin rotacion de perfil en %s" % (ctx, falta_p))
    return errores


# ---------------------------------------------------------------------------- la prueba del cuerpo de perfil (INC-134)

# Lo que la prueba cinematica deja pasar (px y grados): el suelo (un pie puede bajar esto por debajo y seguir "apoyado"), cuanto puede flotar el cuerpo
# en una accion que no salta, el giro por segundo, la escala y la inclinacion del tronco.
TOL_SUELO = 2.0
TOL_FLOTA = 4.0
VELOCIDAD_PERFIL = 1300.0
ESCALA_PERFIL = (0.85, 1.15)        # squash & stretch <= 15 % (DA 13.2)
TRONCO_PERFIL = (-48.0, 10.0)       # hacia delante hasta 48 grados (soplar y recoger); hacia atras, casi nada: sin poses de caida (CP-02)
SIN_SALTO = ("Walk", "Carry", "Push", "PickUp", "Kneel", "Blow")   # Run salta: es la unica que puede tener los dos pies en el aire
SOSTENIDAS = ("Kneel",)             # poses que se sostienen (solo respiran): se piden lejos del reposo, no en movimiento


def valida_perfil(doc, rig=None):
    """
    La prueba de que la coreografia de perfil se puede ver: recorre cada clip de ACCIONES_PERFIL de los cuatro de la familia a 30 cuadros por segundo con la
    MISMA cinematica con que se escribio (GeoPerfil, con la geometria del JSON del rig) y comprueba, desde las curvas del JSON y no desde las funciones:
      - el punto mas bajo de las piernas no atraviesa el suelo mas de TOL_SUELO px y, salvo al correr, no flota mas de TOL_FLOTA (los pies se plantan);
      - ningun codo (>= 0) ni rodilla (<= 0) se dobla al reves, descontando 1 grado;
      - ninguna articulacion gira mas de VELOCIDAD_PERFIL grados por segundo;
      - la escala de Perfil se queda en ESCALA_PERFIL (squash & stretch de a lo sumo el 15 %);
      - el tronco no pasa de TRONCO_PERFIL grados: sin poses de caida ni de derrota (CP-02);
      - la mano y la cabeza no bajan del suelo;
      - la accion MUEVE el cuerpo de perfil (una accion de perfil con las curvas constantes es un cuerpo de perfil que no se anima).
    Devuelve la lista de problemas (vacia = bien).
    """
    errores = []
    rig = rig or P.cargar_rig()
    for p in doc["personajes"]:
        if p["id"] not in FAMILIA:
            continue
        try:
            g = GeoPerfil(P.personaje_rig(rig, p["id"]))
        except KeyError:
            errores.append("%s: rig_articulaciones.json no tiene la clave «perfil»" % p["id"])
            continue
        for clip in p["clips"]:
            if clip["accion"] not in ACCIONES_PERFIL:
                continue
            ctx = "%s/%s" % (p["id"], clip["archivo"])
            curvas = {(c["ruta"], c["propiedad"]): CurvaAuto(c["claves"]) for c in clip["curvas"]}
            falta = [h for h in HUESOS_PERFIL if (h, ROT) not in curvas]
            if falta:
                errores.append("%s: sin rotacion de perfil en %s" % (ctx, falta))
                continue
            dur = clip["duracion"]
            n = max(2, int(math.ceil(dur * FPS)))
            previo, peor = None, {}
            var = {h: [1e9, -1e9] for h in HUESOS_PERFIL}

            def v(ruta, prop, t, defecto=0.0):
                c = curvas.get((ruta, prop))
                return c.evaluar(t) if c is not None else defecto

            for i in range(n + 1):
                t = min(dur, i * dur / n)
                loc = {"P": v(PF, ROT, t), "T": v(PT, ROT, t), "pc": v(PLC, ROT, t), "pl": v(PLL, ROT, t), "kc": v(PKC, ROT, t), "kl": v(PKL, ROT, t),
                       "bc": v(PBC, ROT, t), "bl": v(PBL, ROT, t), "ec": v(PEC, ROT, t), "el": v(PEL, ROT, t), "n": v(PNK, ROT, t), "h": v(PHD, ROT, t)}
                esc = (v(PF, ESCX, t, 1.0), v(PF, ESCY, t, 1.0))
                pts = g.puntos(loc, (v(PT, POSX, t), -v(PT, POSY, t)), esc)
                bajo = max(q[1] for lado in ("c", "l") for q in pts["pies_" + lado])
                peor["hunde"] = max(peor.get("hunde", -1e9), bajo - g.suelo)
                peor["flota"] = max(peor.get("flota", -1e9), g.suelo - bajo)
                peor["rodilla"] = max(peor.get("rodilla", -1e9), loc["kc"], loc["kl"])
                peor["codo"] = max(peor.get("codo", -1e9), -loc["ec"], -loc["el"])
                peor["esc_min"] = min(peor.get("esc_min", 1e9), esc[0], esc[1])
                peor["esc_max"] = max(peor.get("esc_max", -1e9), esc[0], esc[1])
                peor["tau_min"] = min(peor.get("tau_min", 1e9), loc["T"])
                peor["tau_max"] = max(peor.get("tau_max", -1e9), loc["T"])
                peor["mano"] = max(peor.get("mano", -1e9), pts["mano_c"][1] - g.suelo, pts["mano_l"][1] - g.suelo)
                peor["copa"] = max(peor.get("copa", -1e9), pts["copa"][1] - g.suelo)
                for h in HUESOS_PERFIL:
                    var[h][0], var[h][1] = min(var[h][0], loc_de(loc, h)), max(var[h][1], loc_de(loc, h))
                if previo is not None:
                    dt = t - previo[0]
                    for h in HUESOS_PERFIL:
                        rapido = abs(loc_de(loc, h) - loc_de(previo[1], h)) / dt
                        peor["vel"] = max(peor.get("vel", 0.0), rapido)
                        if rapido > VELOCIDAD_PERFIL:
                            peor.setdefault("vel_en", (h.split("/")[-1], round(t, 3), round(rapido)))
                previo = (t, loc)
            if peor["hunde"] > TOL_SUELO:
                errores.append("%s: un pie atraviesa el suelo %.1f px" % (ctx, peor["hunde"]))
            if clip["accion"] in SIN_SALTO and peor["flota"] > TOL_FLOTA:
                errores.append("%s: el cuerpo flota %.1f px sobre el suelo" % (ctx, peor["flota"]))
            if peor["rodilla"] > 1.0:
                errores.append("%s: una rodilla se dobla al reves (%.1f grados)" % (ctx, peor["rodilla"]))
            if peor["codo"] > 1.0:
                errores.append("%s: un codo se dobla al reves (%.1f grados)" % (ctx, peor["codo"]))
            if peor["vel"] > VELOCIDAD_PERFIL:
                errores.append("%s: giro demasiado rapido (%.0f grados/s en %s a los %.2f s)" % ((ctx, peor["vel"]) + peor["vel_en"][:1] + peor["vel_en"][1:2]))
            if peor["esc_min"] < ESCALA_PERFIL[0] or peor["esc_max"] > ESCALA_PERFIL[1]:
                errores.append("%s: squash & stretch de Perfil fuera del 15 %% (%.3f a %.3f)" % (ctx, peor["esc_min"], peor["esc_max"]))
            if peor["tau_min"] < TRONCO_PERFIL[0] or peor["tau_max"] > TRONCO_PERFIL[1]:
                errores.append("%s: el tronco se inclina de %.1f a %.1f grados (limite %s)" % (ctx, peor["tau_min"], peor["tau_max"], TRONCO_PERFIL))
            if peor["mano"] > TOL_SUELO:
                errores.append("%s: una mano baja %.1f px del suelo" % (ctx, peor["mano"]))
            if peor["copa"] > 0.0:
                errores.append("%s: la cabeza baja del suelo" % ctx)
            reposo = reposo_perfil(Ctx(p["id"]))
            if clip["accion"] in SOSTENIDAS:
                if max(max(abs(a - reposo[h]), abs(b - reposo[h])) for h, (a, b) in var.items()) < 8.0:
                    errores.append("%s: la pose sostenida no se aparta del reposo (ninguna articulacion pasa de 8 grados)" % ctx)
            elif max(b - a for a, b in var.values()) < 8.0:
                errores.append("%s: la accion de perfil no mueve el cuerpo de perfil (ninguna articulacion varia 8 grados)" % ctx)
    return errores


def loc_de(loc, hueso):
    """La rotacion local de un hueso de perfil dentro del dict de GeoPerfil.puntos."""
    return loc[{PF: "P", PT: "T", PLC: "pc", PLL: "pl", PKC: "kc", PKL: "kl", PBC: "bc", PBL: "bl", PEC: "ec", PEL: "el", PNK: "n", PHD: "h"}[hueso]]


def autoprueba():
    """valida() tiene que encontrar lo que esta mal: se le dan cinco JSON rotos a proposito."""
    import copy
    import tempfile
    doc, _ = construir()
    casos = []
    d = copy.deepcopy(doc)
    d["personajes"][0]["clips"][1]["curvas"] = [c for c in d["personajes"][0]["clips"][1]["curvas"] if c["ruta"] != LL]
    casos.append(("falta la rotacion de un hueso", d, "sin rotacion"))
    d = copy.deepcopy(doc)
    d["personajes"][3]["clips"][0]["curvas"][2]["claves"][-1][1] += 5.0
    casos.append(("un bucle que no cierra", d, "no cierra"))
    d = copy.deepcopy(doc)
    d["personajes"][0]["clips"][2]["archivo"] = "char_papa_anim_noexiste"
    casos.append(("un clip sin .anim", d, "no existe el .anim"))
    d = copy.deepcopy(doc)
    d["personajes"][1]["clips"][3]["curvas"][0]["claves"][0][0] = 9.0
    casos.append(("claves fuera de la duracion", d, "fuera de"))
    d = copy.deepcopy(doc)
    d["personajes"][2]["clips"][4]["curvas"][0]["propiedad"] = "m_LocalScale.z"
    casos.append(("una propiedad no permitida", d, "no permitida"))
    malos = 0
    for nombre, dd, esperado in casos:
        ruta = os.path.join(tempfile.gettempdir(), "clips_autoprueba.json")
        escribe(dd, ruta)
        errores = valida(dd, ruta)
        ok = any(esperado in e for e in errores)
        malos += 0 if ok else 1
        print("%-34s %s" % (nombre, "detectado" if ok else "NO SE DETECTA"))
    # un caracter no ASCII
    ruta = os.path.join(tempfile.gettempdir(), "clips_autoprueba.json")
    with open(ruta, "w", encoding="utf-8") as f:
        f.write(serializa(doc).replace("Idle", "Idl\u00e9", 1))
    ok = any("ASCII" in e for e in valida(doc, ruta))
    malos += 0 if ok else 1
    print("%-34s %s" % ("un caracter con tilde", "detectado" if ok else "NO SE DETECTA"))
    escribe(doc, ruta)
    ok = not valida(doc, ruta)
    malos += 0 if ok else 1
    print("%-34s %s" % ("el documento bueno", "bien" if ok else "FALSO POSITIVO"))
    os.remove(ruta)
    # la lectura de la lista de acciones con los brazos delante (el contrato con CharacterRig.cs)
    for nombre, ok in P.autoprueba_contrato():
        malos += 0 if ok else 1
        print("%-34s %s" % ("contrato: " + nombre, "bien" if ok else "FALLA"))
    # la lectura del prefab con las dos jerarquias del antebrazo (INC-133): sin ella el solucionador mediria un brazo que no existe
    for nombre, ok in P.autoprueba_arbol():
        malos += 0 if ok else 1
        print("%-34s %s" % ("prefab: " + nombre, "bien" if ok else "FALLA"))
    # INC-134: el cuerpo de perfil: la prueba cinematica y la validacion de que cada clip de la familia lleva las 12 articulaciones de perfil
    for nombre, ok in autoprueba_perfil(doc):
        malos += 0 if ok else 1
        print("%-34s %s" % (nombre, ("detectado" if nombre.startswith("perfil: se detecta") else "bien") if ok else "FALLA"))
    # INC-133 y la cara registrada: el solucionador sabe que el antebrazo va delante (solo el humero se tapa), que en Papa la cabeza tapa el
    # humero, que no deja una mano sobre la cara salvo en CARA_TAPADA y que la visera llega a la frente o a la sien
    for nombre, ok in autoprueba_inc133():
        malos += 0 if ok else 1
        print("%-34s %s" % (nombre, "bien" if ok else "FALLA"))
    return 1 if malos else 0


def autoprueba_perfil(doc):
    """
    [(nombre, ok)]: lo que el cuerpo de perfil (INC-134) tiene que cumplir. (1) La cinematica: los pies de pie tocan el suelo, la IK llega adonde
    se le pide, la rodilla arrodillada apoya rotula y punta a la vez, el giro de rota() es el antihorario de Unity. (2) El documento bueno pasa
    valida() y valida_perfil(). (3) Cada defecto introducido a proposito se detecta: una articulacion de perfil sin curva, una rodilla al reves,
    un codo al reves, un squash del 30 %, un pie bajo el suelo, un giro de 3000 grados por segundo y una accion de perfil sin movimiento.
    """
    import copy
    import tempfile
    casos = []
    rig = P.cargar_rig()
    # --- la cinematica
    for pid in FAMILIA:
        g = GeoPerfil(P.personaje_rig(rig, pid))
        casos.append(("perfil: %s de pie toca el suelo" % pid, abs(g.baja({})) < 1e-6))
        # la IK: la mano llega al objetivo (o se estira hacia el) desde un hombro con el tronco inclinado
        loc = {"T": -20.0}
        hombro = g.puntos(loc)["hombro_c"]
        br = g.brazo["c"]
        alcanzable = (hombro[0] + 0.6 * (br["l1"] + br["l2"]), hombro[1] + 0.5 * (br["l1"] + br["l2"]))
        b, e = g.ik("c", hombro, alcanzable)
        loc.update({"bc": b + 20.0, "ec": e})            # la IK da el angulo del MUNDO; el local resta el tronco
        mano = g.puntos(loc)["mano_c"]
        casos.append(("perfil: la IK de %s llega al objetivo" % pid, _dist(mano, alcanzable) < 0.5 and e >= 0.0))
        lejos = (hombro[0] + 3.0 * (br["l1"] + br["l2"]), hombro[1])
        b, e = g.ik("c", hombro, lejos)
        loc.update({"bc": b + 20.0, "ec": e})
        casos.append(("perfil: la IK de %s se estira hacia lo inalcanzable" % pid, abs(_dist(g.puntos(loc)["mano_c"], hombro) - 0.995 * (br["l1"] + br["l2"])) < 1.0))
        sg = g.canilla_apoyada("l", -8.0)
        ys = [rota(q, sg)[1] for q in g.pierna["l"]["contactos"]]
        casos.append(("perfil: %s arrodillado apoya rotula y punta" % pid, -175.0 < sg < -60.0 and abs(max(ys[0], ys[1]) - ys[2]) < 0.5))
        m = g.resuelve_muslo("c", -10.0, g.y_reposo["c"] - 60.0)
        casos.append(("perfil: %s agachado 60 px baja 60 px" % pid, abs(g.y_apoyo("c", m, -10.0) - (g.y_reposo["c"] - 60.0)) < 0.5))
    v = rota((0.0, 1.0), 90.0)
    casos.append(("perfil: rota() es el antihorario de Unity", abs(v[0] - 1.0) < 1e-9 and abs(v[1]) < 1e-9))   # un vector hacia abajo gira a la DERECHA con +
    # --- el documento bueno, y los defectos que se detectan
    casos.append(("perfil: el documento bueno pasa", not valida(doc) and not valida_perfil(doc, rig)))

    def con(cambia, esperado, buscar=valida_perfil, nombre=None):
        d = copy.deepcopy(doc)
        cambia(d)
        errores = buscar(d) if buscar is valida else buscar(d, rig)
        casos.append(("perfil: se detecta %s" % nombre, any(esperado in e for e in errores)))

    def clip(d, pid, accion):
        return next(c for q in d["personajes"] if q["id"] == pid for c in q["clips"] if c["accion"] == accion)

    def curva(d, pid, accion, ruta, prop):
        return next(c for c in clip(d, pid, accion)["curvas"] if c["ruta"] == ruta and c["propiedad"] == prop)

    def sin_curva(d):
        c = clip(d, "papa", "Idle")
        c["curvas"] = [k for k in c["curvas"] if not (k["ruta"] == PKL and k["propiedad"] == ROT)]

    con(sin_curva, "sin rotacion de perfil", valida, "una articulacion de perfil sin curva en un clip de frente")

    def sin_curva_perfil(d):
        c = clip(d, "nino", "Walk")
        c["curvas"] = [k for k in c["curvas"] if not (k["ruta"] == PHD and k["propiedad"] == ROT)]

    con(sin_curva_perfil, "sin rotacion de perfil", valida, "una articulacion de perfil sin curva en un clip de perfil")

    def rodilla_al_reves(d):
        for k in curva(d, "mama", "Walk", PKC, ROT)["claves"]:
            k[1] += 30.0

    con(rodilla_al_reves, "rodilla se dobla al reves", nombre="una rodilla al reves")

    def codo_al_reves(d):
        for k in curva(d, "nina", "Run", PEL, ROT)["claves"]:
            k[1] -= 120.0

    con(codo_al_reves, "codo se dobla al reves", nombre="un codo al reves")

    def squash(d):
        for k in curva(d, "papa", "Carry", PF, ESCY)["claves"]:
            k[1] += 0.3

    con(squash, "squash & stretch", nombre="un squash del 30 %")

    def sin_suelo(d):
        c = curva(d, "nino", "Push", PT, POSY)
        for k in c["claves"]:
            k[1] -= 40.0

    con(sin_suelo, "atraviesa el suelo", nombre="un pie bajo el suelo")

    def giro(d):
        c = curva(d, "papa", "Walk", PBC, ROT)
        c["claves"][2][1] += 90.0

    con(giro, "giro demasiado rapido", nombre="un giro de miles de grados por segundo")

    def quieto(d):
        # todas las rotaciones de perfil de Blow, fijas en su primer valor: la accion no mueve el cuerpo de perfil
        for k in clip(d, "mama", "Blow")["curvas"]:
            if k["ruta"].startswith(PF) and k["propiedad"] == ROT:
                k["claves"] = [[k["claves"][0][0], k["claves"][0][1]], [k["claves"][-1][0], k["claves"][0][1]]]

    con(quieto, "no mueve el cuerpo de perfil", nombre="una accion de perfil sin movimiento")
    # --- el contrato con ActionView.cs: las siete acciones
    acciones, hallada = P.acciones_de_perfil()
    casos.append(("perfil: ActionView.cs " + ("coincide con ACCIONES_PERFIL" if hallada else "no esta: se usa el contrato"), acciones == set(ACCIONES_PERFIL)))
    # --- que las 7 acciones de perfil y solo ellas se muevan, y los clips de frente lleven los 12 huesos en reposo constante
    for q in doc["personajes"]:
        if q["id"] not in FAMILIA:
            continue
        ok = True
        for c in q["clips"]:
            rot = {k["ruta"]: k["claves"] for k in c["curvas"] if k["propiedad"] == ROT}
            if c["accion"] not in ACCIONES_PERFIL:
                ok = ok and all(len({round(v, 4) for _, v in rot[h]}) == 1 for h in HUESOS_PERFIL)
        casos.append(("perfil: %s deja los 12 huesos de perfil quietos en los clips de frente" % q["id"], ok))
    return casos


def autoprueba_inc133():
    """[(nombre, ok)]: lo que el solucionador tiene que saber desde INC-133 y desde la cara registrada de la entrega (06/10/2026)."""
    casos = []
    rig = P.cargar_rig()
    nino = leer_contexto("nino", rig)
    papa = leer_contexto("papa", rig)
    bn = nino.brazos["Der"]
    casos.append(("INC-133: antebrazo delante (Nino)", bn.antebrazo_delante and bn.partido))
    # la silueta que tapa el humero: en Papa incluye la cabeza (su Cuello va despues de los brazos); en el Nino solo el torso
    sp, sn = papa.silueta, nino.silueta
    punto_barba = (papa.cx, 470.0)   # sobre el torso de Papa no hay nada a esa altura (su torso empieza en y = 415 y es angosto), pero si la barba
    casos.append(("INC-133: la silueta de Papa incluye la cabeza", sp is not None and any(sp.dentro(papa.cx + dx, 500.0) for dx in range(-120, 121, 4))
                  and sp.dentro(*punto_barba)))
    casos.append(("INC-133: la del Nino es solo el torso", sn is not None and not sn.dentro(nino.cx, 300.0)))
    # un brazo en alto que pegaria en la cara se desvia en los gestos que no dejan tapar la cara; en los de CARA_TAPADA hace lo que se pide
    raw = bn._pide(130.0, 8.0, -1)
    bn.permite_tapar_cara(False)
    casos.append(("cara: el gesto crudo pega, la pose no (no tapa)", bn.pega_en_la_cara(*bn.repara(*raw)) and not bn.pega_en_la_cara(*bn.pose(130.0, 8.0, -1))))
    bn.permite_tapar_cara(True)
    casos.append(("cara: en CARA_TAPADA la pose no se desvia", bn.pose(130.0, 8.0, -1) == bn.repara(*raw)))
    # la visera llega a la frente (Papa, que alcanza) y a la sien (el Nino, que no): la mano queda donde se pide o lo mas cerca que llegue
    for pid_, x_ in (("papa", papa), ("nino", nino)):
        b_ = x_.brazos["Izq"]
        b_.permite_tapar_cara(True)
        obj = cabeza_objetivo(x_, "Izq")
        rs, rc = b_.ik(obj, (-1.0, -0.6))
        _, m = b_.fk(rs, rc)
        falta = math.hypot(m[0] - obj[0], m[1] - obj[1])
        alcanza = math.hypot(obj[0] - b_.S[0], obj[1] - b_.S[1]) <= 0.97 * (b_.l1 + b_.l2) + 1.0
        arriba = obj[1] < x_.cara[1]
        casos.append(("cara: la visera de %s llega a %s" % (pid_, "la frente" if arriba else "la sien"),
                      (falta < 6.0 if alcanza else (math.hypot(obj[0] - b_.S[0], obj[1] - b_.S[1]) > b_.l1 + b_.l2 - 1.0))
                      and (arriba == (pid_ == "papa"))))
        b_.permite_tapar_cara(False)
    # el reposo deja ver el humero como pide UMBRAL_OCULTO_REPOSO (sin eso el Idle se sale del umbral al balancearse)
    ok = all(b.visible(b.hombro_rot(nino.hang), b.codo_rot(nino.hang, nino.pliegue), UMBRAL_OCULTO_REPOSO) for b in nino.brazos.values())
    casos.append(("INC-133: el reposo cumple el umbral estricto", ok))
    return casos


def main(argv):
    if "--autoprueba" in argv:
        return autoprueba()
    doc, avisos = construir()
    escribe(doc)
    print("escrito %s (%d personajes, %d clips)" % (os.path.relpath(SALIDA, P.RAIZ), len(doc["personajes"]),
                                                    sum(len(p["clips"]) for p in doc["personajes"])))
    for a in avisos:
        print("AVISO", a)
    acciones, hallada = P.acciones_brazos_delante()
    if not hallada:
        # no es un error: CharacterRig.cs aun no trae la lista y se sigue con la del contrato acordado
        print("AVISO CharacterRig.cs no trae armsInFrontActions: se usa la lista por defecto %s" % sorted(acciones))
    else:
        print("brazos delante del torso en: %s (leido de CharacterRig.cs)" % ", ".join(sorted(acciones)))
    errores = valida(doc) + valida_perfil(doc)
    for e in errores:
        print("ERROR", e)
    if "--valida" in argv or errores:
        print("valida:", "bien" if not errores else "%d problemas" % len(errores))
    return 1 if errores or avisos else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
