#!/usr/bin/env python3
# emociones_narrativas.py: aplica la tabla emociones_narrativas.json (la expresion de la cara de cada personaje, linea a linea, segun el guion) a los 18 assets
# Assets/Game/Data/Narrative/N*_*.asset (INC-148, INC-150), escribiendo YAML a nivel de TEXTO.
#
#     python3 claudeDocs/tasks/Personajes/herramientas/emociones_narrativas.py              # --valida (por defecto): informe por secuencia, no escribe nada; sale con 1 si hay errores
#     python3 claudeDocs/tasks/Personajes/herramientas/emociones_narrativas.py --aplicar    # escribe los assets (se niega si hay errores; --parcial aplica lo que si se puede)
#     python3 claudeDocs/tasks/Personajes/herramientas/emociones_narrativas.py --autoprueba # se prueba solo, con un asset sintetico en memoria
#         --json RUTA          la tabla (por defecto herramientas/emociones_narrativas.json)
#         --narrativas CARPETA los assets (por defecto Assets/Game/Data/Narrative)
#         --solo SECUENCIA...  solo esas secuencias
#         --parcial            con --aplicar: las entradas que fallan se saltan y se aplica el resto (por defecto, un error impide escribir nada)
#
# LA TABLA (la escribe quien lee el guion; este script solo la aplica):
#     {"version": 1, "secuencias": {"N1_Apertura": {
#         "lineas": {"0": {"texto": "Noche helada", "papa": "Worried", "mama": "Worried", "_razon": "..."}},   # cambio de emocion de ese personaje desde esa linea
#         "voces":  {"16": {"texto": "...", "emocion": "Focused"}},                                          # solo voces cuyo hablante NO esta en escena
#         "rumbos": {"papa": {"0": "Left", "8": "Left"}}}}}                                                   # hacia donde mira de perfil (ActorBeat.Facing)
#   Personajes: papa | mama | nina | nino | algoritm. Emociones: los nombres de FacialEmotion (Neutral 0, Happy 1, Surprised 2, Worried 3, Focused 4, Sleeping 5). Rumbos: los de
#   ActorFacing (Auto 0, Left 1, Right 2). Las claves que empiezan por «_» son comentarios. «texto» es el principio de la linea y se COMPARA con ella (sin tildes ni mayusculas): es
#   lo que impide que un indice movido ponga la cara en otra linea.
#
# SEMANTICA (la de ActorTimeline.EmotionAt, ActorBeat y DialogueLine; los nombres de campo salen de ActorBeat.cs, DialogueLine.cs y NarrativeProp.cs):
#   - Una entrada = «desde esa linea y hasta su siguiente entrada, este personaje lleva esta emocion». Se escribe como <SetsEmotion>: 1 y <Emotion>: N en el paso (beat) que ese
#     personaje ya tiene en esa linea; si no tiene, se INSERTA un paso que solo la fija y repite lo que el personaje venia haciendo (ActorTimeline.HeldBefore: el Arrival del ultimo paso si se
#     movia, si no su Action; antes de todo paso, ActorStart), con Moves 0 y el rumbo explicito en vigor (un paso nuevo, sin Facing, devolveria el rumbo a Auto: ActorTimeline.ExplicitFacingAt).
#   - NUNCA se inserta con una caminata en curso (un paso anterior con movimiento y ninguno nuevo entre medias: ActorTimeline.WalkUnderway), porque un paso nuevo la interrumpiria
#     (ActorTimeline.Interrupts), salvo en quien tiene <FinishesSteps>: 1. Es un ERROR del informe, con secuencia, linea y personaje: no se aplica esa entrada.
#   - Quien ya tiene una emocion la lleva en TODOS sus pasos siguientes: cada paso posterior a la primera entrada lleva <SetsEmotion>: 1 con la emocion en vigor en su linea.
#   - «rumbos»: <Facing>: 1 (Left) o 2 (Right) en el paso de esa linea, insertando uno que repite lo sostenido si no hay (la linea 0, antes de todo paso, repite ActorStart).
#   - «voces»: <SetsVoiceEmotion>: 1 y <VoiceEmotion>: N tras el <Silence> de esa linea; solo para hablantes que no estan en escena (los demas llevan su emocion en su paso).
#
# COMO ESCRIBE. Texto, no YAML: solo se reescribe el bloque <Beats> de los props tocados (cada paso con todas sus claves en el orden de Unity: Line, Action, Moves, Destination, Seconds,
# Arrival, SetsEmotion, Emotion, Facing; Destination y Seconds con el texto con que ya estaban) y se insertan las dos claves de voz tras <Silence>. Nada mas se re-codifica: los textos con
# escapes \xE1 y las lineas plegadas, los GUID y los numeros quedan byte a byte como estaban. Idempotente: volver a aplicar no cambia nada.
#
# QUE COMPRUEBA (--valida, y antes de escribir): que cada «texto» sea el principio de la linea de ese indice; que los personajes, las emociones y los rumbos existan y que el personaje este
# en escena; la COBERTURA (toda linea hablada cuyo hablante este en escena lleva su emocion en vigor); los choques con caminatas; las voces; y la REGLA DE LA FOGATA (INC-150): quien
# se arrodilla (Kneel), sopla (Blow) o recoge (PickUp) sin moverse a menos de 0,15 en x de un objeto con <Glows>: 1 tiene que MIRARLA (ActorTimeline.FacesLeftAt, portado fiel, con el
# campo Facing). Informa de cuantos casos de espaldas al fuego hay antes y despues de aplicar.

import argparse
import json
import os
import re
import sys
import unicodedata
from types import SimpleNamespace

AQUI = os.path.dirname(os.path.abspath(__file__))
RAIZ = os.path.abspath(os.path.join(AQUI, "..", "..", "..", ".."))
NARRATIVAS = os.path.join(RAIZ, "Assets", "Game", "Data", "Narrative")
PREFABS = os.path.join(RAIZ, "Assets", "Game", "Prefabs", "Characters")
CODIGO = os.path.join(RAIZ, "Assets", "Game", "Scripts", "Runtime", "Scaffolding")
TABLA = os.path.join(AQUI, "emociones_narrativas.json")

PERSONAJES = ("papa", "mama", "nina", "nino", "algoritm")
PREFAB_DE = {"Papa": "papa", "Mama": "mama", "Nina": "nina", "Nino": "nino", "Algoritm_Fuego": "algoritm", "Algoritm_Rueda": "algoritm", "Algoritm_Gota": "algoritm"}
HABLANTES = {"papa": ("papa",), "mama": ("mama",), "nina": ("nina",), "nino": ("nino",), "ninos": ("nina", "nino"), "algoritm": ("algoritm",)}   # el nombre del hablante, plegado
# los valores de los enums del motor (los assets los guardan como numero); se leen de los .cs cuando estan y se comprueban contra estas tablas
EMOCIONES = {"Neutral": 0, "Happy": 1, "Surprised": 2, "Worried": 3, "Focused": 4, "Sleeping": 5}
RUMBOS = {"Auto": 0, "Left": 1, "Right": 2}
ACCIONES = {"Idle": 0, "Hidden": 1, "Walk": 2, "Run": 3, "Talk": 4, "Strike": 5, "Hammer": 6, "Blow": 7, "PickUp": 8, "Kneel": 9, "Carry": 10, "Push": 11, "Point": 12, "Observe": 13,
            "Celebrate": 14, "Encourage": 15, "Hug": 16, "Surprise": 17, "Sleep": 18, "Appear": 19, "Vanish": 20, "Spin": 21, "Wave": 22}
TRABAJO_JUNTO_AL_FUEGO = ("Kneel", "Blow", "PickUp")   # INC-150: las acciones de quien trabaja sobre el fuego
CERCA_DEL_FUEGO = 0.15                                 # en x, fracciones de la ilustracion
DESPLAZAMIENTO_MIN = 0.01                              # ActorTimeline.MinimumDisplacement: un desplazamiento menor no dice hacia donde se mira
CERO_AL_CUADRADO = 1e-10                               # Heading.ZeroSqrLength


def pliega(texto):
    """Minusculas, sin tildes ni enes (NFKD) y con los blancos colapsados: para comparar textos."""
    t = "".join(c for c in unicodedata.normalize("NFKD", texto) if not unicodedata.combining(c)).lower()
    return re.sub(r"\s+", " ", t).strip()


# ============================================================================ 1. los valores de los enums del motor


def lee_enum(archivo):
    """{nombre: valor} de un enum de C# (los miembros con «Nombre = N»); None si el archivo no esta."""
    ruta = os.path.join(CODIGO, archivo)
    if not os.path.isfile(ruta):
        return None
    with open(ruta, encoding="utf-8") as f:
        texto = re.sub(r"//[^\n]*", "", f.read())
    return {m.group(1): int(m.group(2)) for m in re.finditer(r"\b([A-Z]\w*)\s*=\s*(\d+)", texto)}


def contrato_enums():
    """[(nombre, ok)]: las tablas de arriba son las de los .cs (si estan). Si el motor cambia un valor, la autoprueba avisa antes de que se escriba un numero equivocado."""
    casos = []
    for archivo, tabla in (("FacialEmotion.cs", EMOCIONES), ("ActorFacing.cs", RUMBOS), ("ActorAction.cs", ACCIONES)):
        real = lee_enum(archivo)
        if real is not None:
            casos.append(("%s coincide con la tabla del script" % archivo, real == tabla))
    return casos


# ============================================================================ 2. leer un asset (texto)


RE_CLAVE2 = re.compile(r"^  <(\w+)>k__BackingField:(.*)$")        # una clave de la secuencia (2 espacios)
RE_ITEM2 = re.compile(r"^  - <(\w+)>k__BackingField:(.*)$")       # el primer campo de un elemento de lista de la secuencia
RE_CLAVE4 = re.compile(r"^    <(\w+)>k__BackingField:(.*)$")      # un campo de un prop o de una linea
RE_CAMPO = re.compile(r"^\s*(?:- )?<(\w+)>k__BackingField:\s*(.*)$")
RE_VEC = re.compile(r"x:\s*([-+0-9.eE]+),\s*y:\s*([-+0-9.eE]+)")
RE_GUID = re.compile(r"guid:\s*([0-9a-f]{32})")


def decodifica_doble(s):
    """El contenido de un escalar YAML entre comillas dobles (sin las comillas), con sus escapes: \\xE1, \\uXXXX, \\\" ..."""
    simples = {"n": "\n", "t": "\t", "r": "\r", "0": "\0", "\\": "\\", '"': '"', "/": "/", " ": " ", "a": "\a", "b": "\b", "e": "\x1b", "f": "\f", "v": "\v",
               "N": "\x85", "_": "\xa0", "L": "\u2028", "P": "\u2029"}
    out, i = [], 0
    while i < len(s):
        c = s[i]
        if c != "\\" or i + 1 >= len(s):
            out.append(c)
            i += 1
            continue
        e = s[i + 1]
        if e in "xuU":
            n = {"x": 2, "u": 4, "U": 8}[e]
            out.append(chr(int(s[i + 2:i + 2 + n], 16)))
            i += 2 + n
        else:
            out.append(simples.get(e, e))
            i += 2
    return "".join(out)


def decodifica_escalar(partes):
    """
    El valor de un escalar de Unity (plano, 'simple' o «doble») dado como la lista de sus lineas (la primera, tras «clave: », y las de continuacion): las lineas se pliegan con un solo
    espacio (el plegado de YAML) y, en un escalar entre comillas dobles, una barra al final de linea las une sin espacio.
    """
    partes = [p.strip() for p in partes]
    if not partes or not partes[0]:
        return ""
    if partes[0].startswith('"'):
        unido = ""
        for p in partes:
            if unido.endswith("\\") and not unido.endswith("\\\\"):
                unido = unido[:-1] + p
            else:
                unido = (unido + " " + p) if unido else p
        cuerpo = unido[1:-1] if unido.endswith('"') else unido[1:]
        return decodifica_doble(cuerpo)
    unido = " ".join(p for p in partes if p)
    if unido.startswith("'") and unido.endswith("'") and len(unido) >= 2:
        return unido[1:-1].replace("''", "'")
    return unido


def escalar_en(lineas, i, fin):
    """El valor decodificado del campo de la linea i: su resto y las lineas de continuacion (mas indentadas que el campo, sin ser otro campo ni un elemento)."""
    m = RE_CAMPO.match(lineas[i])
    partes = [m.group(2)]
    j = i + 1
    while j < fin and lineas[j].startswith("      ") and not RE_CAMPO.match(lineas[j]) and not lineas[j].lstrip().startswith("- "):
        partes.append(lineas[j])
        j += 1
    return decodifica_escalar(partes)


class Paso:
    """Un ActorBeat del asset, con los valores TAL COMO ESTAN escritos (Destination y Seconds en crudo: no se reformatean)."""

    CLAVES = ("Line", "Action", "Moves", "Destination", "Seconds", "Arrival", "SetsEmotion", "Emotion", "Facing")

    def __init__(self, **kw):
        self.line = kw.get("Line", 0)
        self.action = kw.get("Action", 0)
        self.moves = kw.get("Moves", 0)
        self.destination = kw.get("Destination", "{x: 0, y: 0}")
        self.seconds = kw.get("Seconds", "0")
        self.arrival = kw.get("Arrival", 0)
        self.sets_emotion = kw.get("SetsEmotion", 0)
        self.emotion = kw.get("Emotion", 0)
        self.facing = kw.get("Facing", 0)

    def copia(self):
        return Paso(Line=self.line, Action=self.action, Moves=self.moves, Destination=self.destination, Seconds=self.seconds, Arrival=self.arrival,
                    SetsEmotion=self.sets_emotion, Emotion=self.emotion, Facing=self.facing)

    @property
    def xy(self):
        m = RE_VEC.search(self.destination)
        return (float(m.group(1)), float(m.group(2))) if m else (0.0, 0.0)

    def lineas(self):
        """Las lineas del paso en el estilo de Unity (la lista cuelga de <Beats> a 4 espacios; los campos, a 6), todas las claves y en el orden de ActorBeat."""
        return ["    - <Line>k__BackingField: %d" % self.line, "      <Action>k__BackingField: %d" % self.action, "      <Moves>k__BackingField: %d" % self.moves,
                "      <Destination>k__BackingField: %s" % self.destination, "      <Seconds>k__BackingField: %s" % self.seconds,
                "      <Arrival>k__BackingField: %d" % self.arrival, "      <SetsEmotion>k__BackingField: %d" % self.sets_emotion,
                "      <Emotion>k__BackingField: %d" % self.emotion, "      <Facing>k__BackingField: %d" % self.facing]

    def igual(self, otro):
        return all(getattr(self, a) == getattr(otro, a) for a in ("line", "action", "moves", "destination", "seconds", "arrival", "sets_emotion", "emotion", "facing"))


class Prop:
    """Un NarrativeProp del asset: lo que las reglas necesitan y donde esta su bloque <Beats> (indices de linea del asset)."""

    def __init__(self):
        self.indice = 0
        self.guid = None
        self.personaje = None
        self.posicion = (0.5, 0.3)
        self.actor_start = 0
        self.finishes = False
        self.glows = False
        self.beats = []
        self.beats_ini = None    # indice de la linea «<Beats>k__BackingField:»
        self.beats_fin = None    # indice de la linea siguiente al bloque
        self.original = []       # los pasos tal como estaban (para saber si algo cambio)

    @property
    def es_actor(self):
        return self.guid is not None


class Linea:
    def __init__(self):
        self.indice = 0
        self.ini = 0
        self.fin = 0
        self.hablante = ""
        self.texto = ""
        self.silence = None       # indice de la linea «<Silence>…»
        self.voz = None           # (indice de <SetsVoiceEmotion>, indice de <VoiceEmotion>) si ya estan
        self.voz_valor = None


class Asset:
    def __init__(self, nombre, lineas, fin_de_linea="\n"):
        self.nombre = nombre
        self.lineas = lineas
        self.fin_de_linea = fin_de_linea
        self.props = []
        self.textos = []


def lee_asset(texto, nombre, guids):
    """
    Parsea el texto de un asset de NarrativeSequence: sus props (con su actor, sus pasos y la posicion de su bloque <Beats>) y sus lineas (hablante, texto decodificado y donde esta
    <Silence>). «guids» es {guid del prefab: personaje}. No modifica nada. Lanza ValueError si falta <Props> o <Lines>.
    """
    fin_de_linea = "\r\n" if "\r\n" in texto else "\n"
    a = Asset(nombre, texto.replace("\r\n", "\n").split("\n"), fin_de_linea)
    lineas = a.lineas
    claves = {}
    for i, l in enumerate(lineas):
        m = RE_CLAVE2.match(l)
        if m:
            claves[m.group(1)] = i
    if "Props" not in claves or "Lines" not in claves:
        raise ValueError("%s: no tiene <Props> o <Lines>" % nombre)

    def bloque(clave):
        ini = claves[clave]
        fin = ini + 1
        while fin < len(lineas) and (lineas[fin].startswith("  - ") or lineas[fin].startswith("    ") or lineas[fin].strip() == ""):
            if lineas[fin].strip() == "" and fin + 1 >= len(lineas):
                break
            fin += 1
        return ini, fin

    def elementos(ini, fin):
        arranques = [j for j in range(ini + 1, fin) if RE_ITEM2.match(lineas[j])]
        return [(s, e) for s, e in zip(arranques, arranques[1:] + [fin])]

    pi, pf = bloque("Props")
    for k, (s, e) in enumerate(elementos(pi, pf)):
        p = Prop()
        p.indice = k
        campos = {RE_ITEM2.match(lineas[s]).group(1): (s, RE_ITEM2.match(lineas[s]).group(2))}
        for j in range(s + 1, e):
            m = RE_CLAVE4.match(lineas[j])
            if m:
                campos[m.group(1)] = (j, m.group(2))
        if "Actor" in campos:
            g = RE_GUID.search(campos["Actor"][1])
            if g:
                p.guid = g.group(1)
                p.personaje = guids.get(p.guid)
        if "Position" in campos:
            v = RE_VEC.search(campos["Position"][1])
            if v:
                p.posicion = (float(v.group(1)), float(v.group(2)))
        p.actor_start = int(campos["ActorStart"][1]) if "ActorStart" in campos else 0
        p.finishes = "FinishesSteps" in campos and campos["FinishesSteps"][1].strip() == "1"
        p.glows = "Glows" in campos and campos["Glows"][1].strip() == "1"
        if "Beats" in campos:
            bi, resto = campos["Beats"]
            p.beats_ini = bi
            j = bi + 1
            if resto.strip() == "[]":
                p.beats_fin = bi + 1
            else:
                actual = None
                while j < e and (lineas[j].startswith("    - ") or lineas[j].startswith("      ")):
                    m = RE_CAMPO.match(lineas[j])
                    if m:
                        if lineas[j].startswith("    - "):
                            actual = {}
                            p.beats.append(actual)
                        if actual is not None:
                            actual[m.group(1)] = m.group(2).strip()
                    j += 1
                p.beats_fin = j
        p.beats = [Paso(**{k_: (v_ if k_ in ("Destination", "Seconds") else int(v_)) for k_, v_ in b.items() if k_ in Paso.CLAVES}) for b in p.beats]
        p.original = [b.copia() for b in p.beats]
        a.props.append(p)

    li, lf = bloque("Lines")
    for k, (s, e) in enumerate(elementos(li, lf)):
        ln = Linea()
        ln.indice, ln.ini, ln.fin = k, s, e
        campos = {RE_ITEM2.match(lineas[s]).group(1): s}
        for j in range(s + 1, e):
            m = RE_CLAVE4.match(lineas[j])
            if m:
                campos[m.group(1)] = j
        ln.hablante = escalar_en(lineas, campos["Speaker"], e) if "Speaker" in campos else ""
        ln.texto = escalar_en(lineas, campos["Text"], e) if "Text" in campos else ""
        ln.silence = campos.get("Silence")
        if "SetsVoiceEmotion" in campos and "VoiceEmotion" in campos:
            ln.voz = (campos["SetsVoiceEmotion"], campos["VoiceEmotion"])
            ln.voz_valor = (int(lineas[campos["SetsVoiceEmotion"]].split(":")[1]), int(lineas[campos["VoiceEmotion"]].split(":")[1]))
        a.textos.append(ln)
    return a


def guids_de_prefabs(carpeta=PREFABS):
    """{guid: personaje} de Assets/Game/Prefabs/Characters/*.prefab.meta."""
    out = {}
    for f in sorted(os.listdir(carpeta)):
        if f.endswith(".prefab.meta") and f[:-len(".prefab.meta")] in PREFAB_DE:
            with open(os.path.join(carpeta, f), encoding="utf-8") as fh:
                m = re.search(r"^guid: ([0-9a-f]{32})", fh.read(), re.M)
            if m:
                out[m.group(1)] = PREFAB_DE[f[:-len(".prefab.meta")]]
    return out


# ============================================================================ 3. ActorTimeline, portado


def paso_en(beats, linea):
    """ActorTimeline.BeatAt: el paso que empieza en esa linea; si hay dos, el ultimo de la lista."""
    hallado = None
    for b in beats:
        if b.line == linea:
            hallado = b
    return hallado


def sostenido_antes(beats, actor_start, linea):
    """ActorTimeline.HeldBefore: lo que mantiene al llegar a la linea: el Arrival del ultimo paso anterior si se movia, si no su Action; sin ninguno, ActorStart."""
    held, ultimo = actor_start, -1
    for b in beats:
        if b.line < linea and b.line >= ultimo:
            held = b.arrival if b.moves else b.action
            ultimo = b.line
    return held


def camino_en_curso(beats, linea):
    """ActorTimeline.WalkUnderway: el ultimo paso anterior a la linea, si es de los que se mueven (un camino que sigue en curso al llegar a ella); None si no."""
    ultimo = None
    for b in beats:
        if b.line < linea and (ultimo is None or b.line >= ultimo.line):
            ultimo = b
    return ultimo if ultimo is not None and ultimo.moves else None


def posicion_antes(posicion, beats, linea):
    """ActorTimeline.PositionBefore: el destino del ultimo paso con movimiento anterior a la linea (o la posicion del prop)."""
    pos, ultimo = posicion, -1
    for b in beats:
        if b.moves and b.line < linea and b.line >= ultimo:
            pos, ultimo = b.xy, b.line
    return pos


def rumbo_de(facing):
    """ActorTimeline.FacesLeftOf: True = Left, False = Right, None = Auto."""
    return {RUMBOS["Left"]: True, RUMBOS["Right"]: False}.get(facing)


def rumbo_explicito_en(beats, linea):
    """ActorTimeline.ExplicitFacingAt: el Facing del paso que rige en la linea (el ultimo que empieza en ella o antes; con dos en la misma, el ultimo de la lista). None = Auto o ningun paso."""
    en_vigor = None
    for b in beats:
        if b.line <= linea and (en_vigor is None or b.line >= en_vigor.line):
            en_vigor = b
    return None if en_vigor is None else rumbo_de(en_vigor.facing)


def mira_izquierda(delta):
    """Heading.FacesLeft: izquierda/arriba -> izquierda, derecha/abajo -> derecha; manda el eje dominante (un empate, el horizontal). None con un vector nulo."""
    dx, dy = delta
    if dx * dx + dy * dy < CERO_AL_CUADRADO:
        return None
    if abs(dx) >= abs(dy):
        return dx < 0.0
    return dy > 0.0


def rumbo_del_desplazamiento(desde, hasta):
    """ActorTimeline.HeadingOfDisplacement: hacia donde mira un desplazamiento; None si mide menos de DESPLAZAMIENTO_MIN."""
    d = (hasta[0] - desde[0], hasta[1] - desde[1])
    if d[0] * d[0] + d[1] * d[1] < DESPLAZAMIENTO_MIN * DESPLAZAMIENTO_MIN:
        return None
    return mira_izquierda(d)


def mira_izquierda_en(posicion, beats, linea):
    """
    ActorTimeline.FacesLeftAt, portado fiel: (1) el Facing explicito del paso que rige; (2) hacia donde se desplaza el paso con movimiento de esa linea; (3) hacia donde se desplazo la
    ultima vez antes (el Facing a mano de ese paso pesa mas que su desplazamiento); (4) hacia donde se desplazara la primera vez despues; (5) el centro de la ilustracion (x >= 0,5 mira a la izquierda).
    """
    explicito = rumbo_explicito_en(beats, linea)
    if explicito is not None:
        return explicito
    paso = paso_en(beats, linea)
    if paso is not None and paso.moves:
        propio = rumbo_del_desplazamiento(posicion_antes(posicion, beats, linea), paso.xy)
        if propio is not None:
            return propio
    previo, ultimo = None, -1
    for b in beats:
        if b.moves and b.line < linea and b.line >= ultimo:
            rumbo = rumbo_de(b.facing)
            if rumbo is None:
                rumbo = rumbo_del_desplazamiento(posicion_antes(posicion, beats, b.line), b.xy)
            if rumbo is not None:
                previo, ultimo = rumbo, b.line
    if previo is not None:
        return previo
    siguiente, primero = None, 1 << 30
    for b in beats:
        if b.moves and b.line > linea and b.line <= primero:
            rumbo = rumbo_de(b.facing)
            if rumbo is None:
                rumbo = rumbo_del_desplazamiento(posicion_antes(posicion, beats, b.line), b.xy)
            if rumbo is not None:
                siguiente, primero = rumbo, b.line
    if siguiente is not None:
        return siguiente
    return posicion[0] >= 0.5


def accion_en(beats, actor_start, linea):
    """(accion, se_mueve) durante la linea: la del paso de esa linea, o lo sostenido (ActorTimeline.Cue sin el gesto de hablar, que solo cambia Idle por Talk)."""
    paso = paso_en(beats, linea)
    if paso is None:
        return sostenido_antes(beats, actor_start, linea), False
    if paso.moves:
        return paso.action, True
    return paso.action, False


def casos_de_espaldas_al_fuego(props, n_lineas):
    """
    [(personaje, linea, accion, mira_izquierda, deberia)]: los casos en que un actor que trabaja junto al fuego (Kneel, Blow o PickUp, sin moverse, a menos de CERCA_DEL_FUEGO en x
    de un prop con Glows) no lo mira. «props» son los Prop (con sus beats en el estado que se quiere evaluar).
    """
    fuegos = [p for p in props if p.glows]
    malos = []
    nombres = {v: k for k, v in ACCIONES.items()}
    for p in props:
        if not p.es_actor or not fuegos:
            continue
        for linea in range(n_lineas):
            accion, se_mueve = accion_en(p.beats, p.actor_start, linea)
            if se_mueve or nombres.get(accion) not in TRABAJO_JUNTO_AL_FUEGO:
                continue
            pos = posicion_antes(p.posicion, p.beats, linea)
            cerca = [f for f in fuegos if abs(f.posicion[0] - pos[0]) <= CERCA_DEL_FUEGO]
            if not cerca:
                continue
            fuego = min(cerca, key=lambda f: abs(f.posicion[0] - pos[0]))
            dx = fuego.posicion[0] - pos[0]
            if abs(dx) < 1e-6:
                continue   # en la misma vertical: no hay lado que mirar (Heading.FacesLeftToward conserva el rumbo)
            deberia = dx < 0.0
            mira = mira_izquierda_en(p.posicion, p.beats, linea)
            if mira != deberia:
                malos.append((p.personaje, linea, nombres.get(accion), mira, deberia))
    return malos


# ============================================================================ 4. aplicar la tabla a una secuencia (en el modelo)


def lee_emocion(nombre):
    return EMOCIONES.get(nombre)


def aplica_secuencia(asset, entrada, nombre):
    """
    Aplica la entrada de la tabla de UNA secuencia al modelo del asset (pasos de los props y voces) y devuelve SimpleNamespace(errores, avisos, voces {indice de linea: emocion}, cuenta,
    antes, despues): los errores con secuencia, linea y personaje; «cuenta» = pasos con emocion fijada, pasos insertados, pasos que solo heredan la emocion, rumbos y voces; antes y despues =
    los casos de espaldas al fuego. NO escribe nada: las entradas con error no se aplican.
    """
    res = SimpleNamespace(errores=[], avisos=[], voces={}, cuenta={"fijados": 0, "insertados": 0, "heredados": 0, "rumbos": 0, "voces": 0}, antes=[], despues=[])
    n = len(asset.textos)
    res.antes = casos_de_espaldas_al_fuego(asset.props, n)
    lineas = entrada.get("lineas", {})
    rumbos = entrada.get("rumbos", {})
    voces = entrada.get("voces", {})

    def error(texto):
        res.errores.append("%s: %s" % (nombre, texto))

    # --- los textos citados, los personajes y las emociones
    por_personaje = {p: [] for p in PERSONAJES}     # personaje -> [(linea, emocion int)]
    for clave, ent in sorted(lineas.items(), key=lambda kv: int(kv[0])):
        try:
            idx = int(clave)
        except ValueError:
            error("la linea «%s» no es un numero" % clave)
            continue
        if not 0 <= idx < n:
            error("la linea %d no existe (la secuencia tiene %d)" % (idx, n))
            continue
        cita = ent.get("texto")
        if cita is not None and not pliega(asset.textos[idx].texto).startswith(pliega(cita)):
            error("linea %d: el texto citado «%s» no es el principio de la linea («%s»)" % (idx, cita, asset.textos[idx].texto[:60].replace("\n", " ")))
            continue
        for k, v in ent.items():
            if k == "texto" or k.startswith("_"):
                continue
            if k not in PERSONAJES:
                error("linea %d: personaje desconocido «%s» (%s)" % (idx, k, ", ".join(PERSONAJES)))
                continue
            e = lee_emocion(v)
            if e is None:
                error("linea %d, %s: emocion desconocida «%s» (%s)" % (idx, k, v, ", ".join(EMOCIONES)))
                continue
            por_personaje[k].append((idx, e))
    rumbos_de = {p: [] for p in PERSONAJES}
    for k, por_linea in rumbos.items():
        if k.startswith("_"):
            continue
        if k not in PERSONAJES:
            error("rumbos: personaje desconocido «%s»" % k)
            continue
        for clave, v in por_linea.items():
            if clave.startswith("_"):
                continue
            try:
                idx = int(clave)
            except ValueError:
                error("rumbos, %s: la linea «%s» no es un numero" % (k, clave))
                continue
            if not 0 <= idx < n:
                error("rumbos, %s: la linea %d no existe" % (k, idx))
            elif v not in RUMBOS or v == "Auto":
                error("rumbos, %s, linea %d: rumbo desconocido «%s» (Left o Right)" % (k, idx, v))
            else:
                rumbos_de[k].append((idx, RUMBOS[v]))

    en_escena = {}
    for p in asset.props:
        if p.es_actor and p.personaje:
            en_escena.setdefault(p.personaje, []).append(p)

    # --- los pasos de cada personaje
    for personaje in PERSONAJES:
        emos = sorted(set(por_personaje[personaje]))
        rums = sorted(set(rumbos_de[personaje]))
        if not emos and not rums:
            continue
        if personaje not in en_escena:
            error("%s no esta en escena (ningun prop lo tiene de actor): sus %d entradas no se aplican" % (personaje, len(emos) + len(rums)))
            continue
        for p in en_escena[personaje]:
            aplica_prop(p, personaje, emos, rums, nombre, res, error)

    # --- la cobertura: cada linea hablada por quien esta en escena lleva su emocion en vigor
    for ln in asset.textos:
        habla = HABLANTES.get(pliega(ln.hablante)) if ln.hablante.strip() else None
        if not habla:
            continue
        for personaje in habla:
            if personaje in en_escena and not any(l <= ln.indice for l, _ in por_personaje[personaje]):
                error("linea %d: la dice %s, que esta en escena, y no tiene ninguna emocion en vigor (cobertura)" % (ln.indice, personaje))

    # --- las voces fuera de escena
    for clave, ent in voces.items():
        if clave.startswith("_"):
            continue
        try:
            idx = int(clave)
        except ValueError:
            error("voces: la linea «%s» no es un numero" % clave)
            continue
        if not 0 <= idx < n:
            error("voces: la linea %d no existe" % idx)
            continue
        ln = asset.textos[idx]
        cita = ent.get("texto")
        if cita is not None and not pliega(ln.texto).startswith(pliega(cita)):
            error("voces, linea %d: el texto citado «%s» no es el principio de la linea («%s»)" % (idx, cita, ln.texto[:60].replace("\n", " ")))
            continue
        habla = HABLANTES.get(pliega(ln.hablante)) if ln.hablante.strip() else None
        if not ln.hablante.strip():
            error("voces, linea %d: es una acotacion (sin hablante): no tiene voz" % idx)
            continue
        if habla and any(h in en_escena for h in habla):
            error("voces, linea %d: %s esta en escena: lleva su emocion en su paso, no en la linea" % (idx, ln.hablante))
            continue
        e = lee_emocion(ent.get("emocion", ""))
        if e is None:
            error("voces, linea %d: emocion desconocida «%s»" % (idx, ent.get("emocion")))
            continue
        res.voces[idx] = e
        res.cuenta["voces"] += 1
    res.despues = casos_de_espaldas_al_fuego(asset.props, n)
    return res


def aplica_prop(p, personaje, emos, rums, nombre, res, error):
    """
    Los pasos de UN prop de actor con las entradas de emocion («emos»: [(linea, emocion)]) y de rumbo («rums»: [(linea, facing)]) de su personaje. Modifica p.beats solo si todo el prop sale
    bien (un error en cualquier entrada deja el prop como estaba: aplicar a medias dejaria la emocion y el rumbo desparejados).
    """
    beats = [b.copia() for b in p.beats]
    cuenta = {"fijados": 0, "insertados": 0, "heredados": 0, "rumbos": 0}
    emo_en = dict(emos)
    rum_en = dict(rums)
    fallo = False
    for linea in sorted(set(emo_en) | set(rum_en)):
        destino = paso_en(beats, linea)
        if destino is None:
            if camino_en_curso(beats, linea) is not None and not p.finishes:
                prev = camino_en_curso(beats, linea)
                error("linea %d, %s: habria que insertar un paso en la linea %d y interrumpiria su caminata (el paso de la linea %d se mueve y %s no termina sus pasos): no se aplica" % (
                    linea, personaje, linea, prev.line, personaje))
                fallo = True
                continue
            rumbo = rumbo_explicito_en(beats, linea)
            destino = Paso(Line=linea, Action=sostenido_antes(beats, p.actor_start, linea), Moves=0, Destination="{x: 0, y: 0}", Seconds="0", Arrival=0,
                           Facing=(RUMBOS["Left"] if rumbo is True else RUMBOS["Right"] if rumbo is False else RUMBOS["Auto"]))
            lugar = next((i for i, b in enumerate(beats) if b.line > linea), len(beats))
            beats.insert(lugar, destino)
            cuenta["insertados"] += 1
        if linea in emo_en:
            destino.sets_emotion, destino.emotion = 1, emo_en[linea]
            cuenta["fijados"] += 1
        if linea in rum_en:
            destino.facing = rum_en[linea]
            cuenta["rumbos"] += 1
    if fallo:
        return
    if emo_en:   # quien ya tiene una emocion la lleva en TODOS sus pasos siguientes (la que rige en la linea de cada uno)
        primera = min(emo_en)
        vigente = sorted(emo_en.items())
        for b in beats:
            if b.line < primera:
                continue
            e = [v for l, v in vigente if l <= b.line][-1]
            if b.line not in emo_en and not (b.sets_emotion == 1 and b.emotion == e):
                cuenta["heredados"] += 1
            b.sets_emotion, b.emotion = 1, e
    p.beats = beats
    for k, v in cuenta.items():
        res.cuenta[k] += v


# ============================================================================ 5. escribir (texto)


def bloque_beats(p):
    """Las lineas nuevas del bloque <Beats> del prop (la clave y sus pasos, o «[]» si no hay)."""
    if not p.beats:
        return ["    <Beats>k__BackingField: []"]
    out = ["    <Beats>k__BackingField:"]
    for b in p.beats:
        out += b.lineas()
    return out


def cambios_de_texto(asset, res):
    """
    [(indice de linea inicial, indice final, [lineas nuevas])] de lo que hay que reescribir en el asset: el bloque <Beats> de cada prop cuyos pasos cambiaron (o que aun no traen las tres claves
    nuevas y se tocaron) y las dos claves de voz tras <Silence> de cada linea con voz. Ordenados de abajo arriba, para aplicarlos sin que se muevan los indices.
    """
    cambios = []
    for p in asset.props:
        if p.beats_ini is None:
            continue
        nuevas = bloque_beats(p)
        viejas = asset.lineas[p.beats_ini:p.beats_fin]
        tocado = len(p.beats) != len(p.original) or any(not a.igual(b) for a, b in zip(p.beats, p.original))
        if tocado and nuevas != viejas:
            cambios.append((p.beats_ini, p.beats_fin, nuevas))
    for idx, emocion in sorted(res.voces.items()):
        ln = asset.textos[idx]
        if ln.silence is None:
            continue
        nuevas = ["    <SetsVoiceEmotion>k__BackingField: 1", "    <VoiceEmotion>k__BackingField: %d" % emocion]
        if ln.voz is not None:
            if ln.voz_valor != (1, emocion):
                cambios.append((ln.voz[0], ln.voz[1] + 1, nuevas))
        else:
            cambios.append((ln.silence + 1, ln.silence + 1, nuevas))
    cambios.sort(key=lambda c: c[0], reverse=True)
    return cambios


def escribe_texto(asset, res):
    """El texto del asset con los cambios aplicados (los demas bytes, tal cual) y cuantos bloques se reescribieron."""
    lineas = list(asset.lineas)
    cambios = cambios_de_texto(asset, res)
    for ini, fin, nuevas in cambios:
        lineas[ini:fin] = nuevas
    return asset.fin_de_linea.join(lineas), len(cambios)


# ============================================================================ 6. informe y principal


def informe(nombre, res, cambios):
    c = res.cuenta
    partes = ["%d fijadas" % c["fijados"], "%d insertados" % c["insertados"], "%d heredadas" % c["heredados"], "%d rumbos" % c["rumbos"], "%d voces" % c["voces"]]
    fuego = "fuego: %d de espaldas antes, %d despues" % (len(res.antes), len(res.despues)) if (res.antes or res.despues) else "fuego: sin casos"
    print("  %-26s %s | %s | %d bloques a reescribir%s" % (nombre, ", ".join(partes), fuego, cambios, "" if not res.errores else "  (%d ERRORES)" % len(res.errores)))
    for e in res.errores:
        print("    ERROR " + e)
    for a in res.avisos:
        print("    AVISO " + a)
    for personaje, linea, accion, mira, deberia in res.despues:
        print("    FUEGO %s: %s en la linea %d mira a %s y deberia mirar a %s" % (personaje, accion, linea, "la izquierda" if mira else "la derecha", "la izquierda" if deberia else "la derecha"))


def procesa(tabla, carpeta, guids, solo=None, parcial=False, escribe=False):
    """Valida (y, con «escribe», escribe) todas las secuencias de la tabla. Devuelve (errores totales, {nombre: texto nuevo}, totales)."""
    secuencias = tabla.get("secuencias", {})
    errores_total, nuevos = 0, {}
    tot = {"fijados": 0, "insertados": 0, "heredados": 0, "rumbos": 0, "voces": 0, "antes": 0, "despues": 0}
    for nombre in sorted(secuencias):
        if solo and nombre not in solo:
            continue
        ruta = os.path.join(carpeta, nombre + ".asset")
        if not os.path.isfile(ruta):
            print("  %-26s ERROR no existe %s" % (nombre, os.path.relpath(ruta, RAIZ)))
            errores_total += 1
            continue
        with open(ruta, encoding="utf-8", newline="") as f:
            texto = f.read()
        asset = lee_asset(texto, nombre, guids)
        res = aplica_secuencia(asset, secuencias[nombre], nombre)
        nuevo, cambios = escribe_texto(asset, res)
        informe(nombre, res, cambios)
        errores_total += len(res.errores)
        for k in ("fijados", "insertados", "heredados", "rumbos", "voces"):
            tot[k] += res.cuenta[k]
        tot["antes"] += len(res.antes)
        tot["despues"] += len(res.despues)
        if nuevo != texto and (not res.errores or parcial):
            nuevos[nombre] = nuevo
    return errores_total, nuevos, tot


def main(argv=None):
    ap = argparse.ArgumentParser(description="Aplica emociones_narrativas.json a los assets de las narrativas (ver la cabecera del script).")
    ap.add_argument("--json", default=TABLA)
    ap.add_argument("--narrativas", default=NARRATIVAS)
    ap.add_argument("--solo", nargs="*")
    ap.add_argument("--valida", action="store_true", help="(por defecto) informe sin escribir")
    ap.add_argument("--aplicar", action="store_true", help="escribe los assets (se niega si hay errores)")
    ap.add_argument("--parcial", action="store_true", help="con --aplicar: salta lo que falla y aplica lo demas")
    ap.add_argument("--autoprueba", action="store_true")
    a = ap.parse_args(argv)
    if a.autoprueba:
        return autoprueba()
    if not os.path.isfile(a.json):
        print("ERROR no existe %s" % a.json)
        return 2
    with open(a.json, encoding="utf-8") as f:
        tabla = json.load(f)
    if tabla.get("version") != 1:
        print("ERROR version de la tabla desconocida: %r" % tabla.get("version"))
        return 2
    guids = guids_de_prefabs()
    if len(set(guids.values())) != len(PERSONAJES):
        print("ERROR no encuentro los .prefab.meta de los cinco personajes en %s" % PREFABS)
        return 2
    print("== %s: %d secuencias" % (os.path.relpath(a.json, RAIZ), len(tabla.get("secuencias", {}))))
    errores, nuevos, tot = procesa(tabla, a.narrativas, guids, a.solo, a.parcial)
    print("\nTOTAL: %d emociones fijadas en pasos que ya existian o insertados (%d insertados), %d pasos que heredan la emocion, %d rumbos, %d voces; "
          "casos de espaldas al fuego: %d antes, %d despues; %d errores" % (tot["fijados"], tot["insertados"], tot["heredados"], tot["rumbos"], tot["voces"], tot["antes"], tot["despues"], errores))
    if not a.aplicar:
        print("(sin --aplicar: no se escribio nada)")
        return 1 if errores else 0
    if errores and not a.parcial:
        print("\nNO SE ESCRIBE NADA: hay errores (--parcial aplica lo que si se puede)")
        return 1
    for nombre, texto in sorted(nuevos.items()):
        ruta = os.path.join(a.narrativas, nombre + ".asset")
        with open(ruta, "w", encoding="utf-8", newline="") as f:
            f.write(texto)
        print("  escribe %s" % os.path.relpath(ruta, RAIZ))
    print("%d assets escritos" % len(nuevos))
    return 1 if errores else 0


# ============================================================================ 7. la autoprueba


ASSET_SINTETICO = '''%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_Name: N9_Prueba
  <Id>k__BackingField: N9_Prueba
  <Props>k__BackingField:
  - <Art>k__BackingField: {fileID: 21300000, guid: 11111111111111111111111111111111, type: 3}
    <Position>k__BackingField: {x: 0.3, y: 0.4}
    <Size>k__BackingField: 0.3
    <Actor>k__BackingField: {fileID: 1, guid: aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa, type: 3}
    <ActorStart>k__BackingField: 0
    <Beats>k__BackingField:
    - <Line>k__BackingField: 3
      <Action>k__BackingField: 2
      <Moves>k__BackingField: 1
      <Destination>k__BackingField: {x: 0.6, y: 0.4}
      <Seconds>k__BackingField: 3.4
      <Arrival>k__BackingField: 16
    - <Line>k__BackingField: 7
      <Action>k__BackingField: 9
      <Moves>k__BackingField: 0
      <Destination>k__BackingField: {x: 0, y: 0}
      <Seconds>k__BackingField: 0
      <Arrival>k__BackingField: 0
  - <Art>k__BackingField: {fileID: 21300000, guid: 22222222222222222222222222222222, type: 3}
    <Position>k__BackingField: {x: 0.7, y: 0.4}
    <Size>k__BackingField: 0.3
    <Actor>k__BackingField: {fileID: 2, guid: bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb, type: 3}
    <ActorStart>k__BackingField: 9
    <FinishesSteps>k__BackingField: 1
    <Beats>k__BackingField:
    - <Line>k__BackingField: 1
      <Action>k__BackingField: 2
      <Moves>k__BackingField: 1
      <Destination>k__BackingField: {x: 0.5, y: 0.4}
      <Seconds>k__BackingField: 2
      <Arrival>k__BackingField: 0
  - <Art>k__BackingField: {fileID: 21300000, guid: 33333333333333333333333333333333, type: 3}
    <Position>k__BackingField: {x: 0.5, y: 0.42}
    <Size>k__BackingField: 0.2
    <Glows>k__BackingField: 1
    <Actor>k__BackingField: {fileID: 0}
    <ActorStart>k__BackingField: 0
    <Beats>k__BackingField: []
  <Lines>k__BackingField:
  - <Speaker>k__BackingField:
    <Text>k__BackingField: "Una llama naranja crece despacio:
      m\\xE1s firme, m\\xE1s alta."
    <Sound>k__BackingField: {fileID: 0}
    <Silence>k__BackingField: 0
  - <Speaker>k__BackingField: "PAP\\xC1"
    <Text>k__BackingField: Plano y
      plegado.
    <Sound>k__BackingField: {fileID: 0}
    <Silence>k__BackingField: 0
  - <Speaker>k__BackingField: ALGORITM
    <Text>k__BackingField: "\\xBFVen? Yo no traje la luz."
    <Sound>k__BackingField: {fileID: 0}
    <Silence>k__BackingField: 0
  - <Speaker>k__BackingField:
    <Text>k__BackingField: Cuarta
    <Sound>k__BackingField: {fileID: 0}
    <Silence>k__BackingField: 0
  - <Speaker>k__BackingField: "MAM\\xC1"
    <Text>k__BackingField: Quinta
    <Sound>k__BackingField: {fileID: 0}
    <Silence>k__BackingField: 0
  - <Speaker>k__BackingField:
    <Text>k__BackingField: Sexta
    <Sound>k__BackingField: {fileID: 0}
    <Silence>k__BackingField: 0
  - <Speaker>k__BackingField:
    <Text>k__BackingField: Septima
    <Sound>k__BackingField: {fileID: 0}
    <Silence>k__BackingField: 0
  - <Speaker>k__BackingField:
    <Text>k__BackingField: Octava
    <Sound>k__BackingField: {fileID: 0}
    <Silence>k__BackingField: 0
  <Other>k__BackingField: 5
'''
GUIDS_SINTETICOS = {"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa": "papa", "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb": "mama"}


def autoprueba():
    malos = 0

    def caso(nombre, ok, detalle=""):
        nonlocal malos
        malos += 0 if ok else 1
        print("%-110s %s" % (nombre, "bien" if ok else "FALLA " + detalle))

    for nombre, ok in contrato_enums():
        caso(nombre, ok)
    caso("pliega: sin tildes, sin mayusculas y con los blancos colapsados", pliega("  ¡PAPÁ  Ñandú! ") == "¡papa nandu!")
    caso("decodifica_doble: \\xE1, \\u00e9, \\\" y la barra", decodifica_doble('Pap\\xE1 \\u00e9 \\"x\\" \\\\') == 'Papá é "x" \\')
    caso("decodifica_escalar: plegado de dos lineas y barra al final de linea", decodifica_escalar(['"uno', 'dos"']) == "uno dos" and decodifica_escalar(['"uno\\', 'dos"']) == "unodos"
         and decodifica_escalar(["plano", "plegado"]) == "plano plegado" and decodifica_escalar(["'don''t'"]) == "don't")

    asset = lee_asset(ASSET_SINTETICO, "N9_Prueba", GUIDS_SINTETICOS)
    caso("lee_asset: tres props (dos actores), ocho lineas y los hablantes decodificados",
         len(asset.props) == 3 and [p.personaje for p in asset.props] == ["papa", "mama", None] and len(asset.textos) == 8
         and [l.hablante for l in asset.textos][:5] == ["", "PAPÁ", "ALGORITM", "", "MAMÁ"])
    caso("lee_asset: el texto con escapes y plegado se decodifica", asset.textos[0].texto == "Una llama naranja crece despacio: más firme, más alta." and asset.textos[1].texto == "Plano y plegado.")
    caso("lee_asset: pasos, FinishesSteps, Glows, posicion y bloque <Beats> (con y sin pasos)",
         len(asset.props[0].beats) == 2 and asset.props[0].beats[0].destination == "{x: 0.6, y: 0.4}" and asset.props[0].beats[0].seconds == "3.4" and asset.props[1].finishes
         and asset.props[2].glows and asset.props[2].beats == [] and asset.props[0].posicion == (0.3, 0.4) and asset.props[0].beats_ini < asset.props[0].beats_fin)

    # --- aplicar: papa tiene beats en 3 (camina hasta la 4) y 7; mama termina sus pasos
    entrada = {"lineas": {"0": {"texto": "Una llama", "papa": "Surprised", "mama": "Happy"},
                          "2": {"texto": "¿Ven? yo no traje", "mama": "Focused", "_razon": "x"},
                          "4": {"texto": "quinta", "mama": "Neutral"}},
               "voces": {"2": {"texto": "¿Ven", "emocion": "Focused"}},
               "rumbos": {"papa": {"0": "Left", "7": "Left"}, "mama": {"2": "Right"}}}
    # papa: entrada en 0 (sin pasos antes: inserta), rumbo en 0 y en 7; sus pasos de 3 y 7 heredan la emocion. La linea 2 de algoritm no esta en escena: voz. mama: FinishesSteps.
    res = aplica_secuencia(lee_asset(ASSET_SINTETICO, "N9_Prueba", GUIDS_SINTETICOS), entrada, "N9_Prueba")
    caso("sin errores en el caso bueno (la voz de la linea 2 es de ALGORITM, que no esta en escena)", res.errores == [], str(res.errores))
    a2 = lee_asset(ASSET_SINTETICO, "N9_Prueba", GUIDS_SINTETICOS)
    res = aplica_secuencia(a2, entrada, "N9_Prueba")
    papa = a2.props[0].beats
    caso("papa: un paso nuevo en la linea 0 que repite ActorStart (Idle), no se mueve, fija Surprised y el rumbo Left",
         papa[0].line == 0 and papa[0].action == 0 and papa[0].moves == 0 and papa[0].sets_emotion == 1 and papa[0].emotion == 2 and papa[0].facing == 1 and papa[0].destination == "{x: 0, y: 0}")
    caso("papa: sus pasos de las lineas 3 y 7 heredan la emocion (Surprised) y el de la 7 lleva el rumbo Left a mano, el de la 3 conserva su Facing (Auto)",
         [b.line for b in papa] == [0, 3, 7] and papa[1].sets_emotion == 1 and papa[1].emotion == 2 and papa[2].sets_emotion == 1 and papa[2].emotion == 2 and papa[2].facing == 1 and papa[1].facing == 0)
    caso("papa: el paso que se mueve conserva su destino y sus segundos tal como estaban (texto crudo)", papa[1].destination == "{x: 0.6, y: 0.4}" and papa[1].seconds == "3.4" and papa[1].arrival == 16)
    mama = a2.props[1].beats
    caso("mama (FinishesSteps): se inserta con la caminata en curso (linea 2, tras el paso de la 1 que se mueve): repite su Arrival (Idle) y fija Focused y el rumbo Right; el de la linea 0 va antes y repite ActorStart (Kneel)",
         [b.line for b in mama] == [0, 1, 2, 4] and mama[0].action == 9 and mama[0].emotion == 1 and mama[2].action == 0 and mama[2].emotion == 4 and mama[2].facing == 2)
    caso("mama: el paso de la linea 4 (insertado) fija Neutral; el de la 1 hereda Happy", mama[3].emotion == 0 and mama[3].sets_emotion == 1 and mama[1].emotion == 1 and mama[1].sets_emotion == 1)
    caso("mama: el paso insertado en la linea 4 conserva el rumbo explicito en vigor (Right, del paso de la linea 2): un paso nuevo sin Facing lo devolveria a Auto", mama[3].facing == 2)
    caso("la voz de la linea 2 queda anotada", res.voces == {2: 4})
    caso("cuenta: 4 emociones fijadas, 4 insertados, 2 heredadas (los pasos 3 y 7 de papa) y 1 de mama (el de la 1), 3 rumbos, 1 voz",
         res.cuenta["fijados"] == 4 and res.cuenta["insertados"] == 4 and res.cuenta["heredados"] == 3 and res.cuenta["rumbos"] == 3 and res.cuenta["voces"] == 1, str(res.cuenta))

    # --- el texto: solo cambian los bloques tocados
    nuevo, n_cambios = escribe_texto(a2, res)
    viejas, nuevas = ASSET_SINTETICO.split("\n"), nuevo.split("\n")
    caso("escribe_texto: dos bloques <Beats> reescritos y una voz (3 cambios)", n_cambios == 3, str(n_cambios))
    caso("escribe_texto: el texto de las lineas (escapes \\xE1 y plegado) y el prop 3 quedan byte a byte como estaban",
         'm\\xE1s firme, m\\xE1s alta."' in nuevo and "PAP\\xC1" in nuevo and "<Beats>k__BackingField: []" in nuevo and nuevo.split("<Props>")[0] == ASSET_SINTETICO.split("<Props>")[0])
    caso("escribe_texto: las dos claves de voz van justo tras <Silence> de la linea 2 y <Other> sigue intacto",
         "    <Silence>k__BackingField: 0\n    <SetsVoiceEmotion>k__BackingField: 1\n    <VoiceEmotion>k__BackingField: 4\n  - <Speaker>k__BackingField:" in nuevo.replace("k__BackingField: \n", "k__BackingField:\n")
         and nuevo.endswith("  <Other>k__BackingField: 5\n"))
    caso("escribe_texto: cada paso lleva las nueve claves en el orden de Unity",
         all(("      <SetsEmotion>k__BackingField:" in nuevo and "      <Emotion>k__BackingField:" in nuevo and "      <Facing>k__BackingField:" in nuevo) for _ in (0,))
         and nuevo.count("    - <Line>k__BackingField:") == nuevo.count("      <Facing>k__BackingField:") == 7)
    # --- idempotencia: aplicar el resultado otra vez no cambia nada
    a3 = lee_asset(nuevo, "N9_Prueba", GUIDS_SINTETICOS)
    res3 = aplica_secuencia(a3, entrada, "N9_Prueba")
    nuevo3, n3 = escribe_texto(a3, res3)
    caso("idempotente: volver a aplicar sobre el resultado no cambia ni un byte (0 bloques)", nuevo3 == nuevo and n3 == 0 and res3.errores == [], "%d bloques" % n3)
    # --- choques con caminatas
    a4 = lee_asset(ASSET_SINTETICO, "N9_Prueba", GUIDS_SINTETICOS)
    choque = aplica_secuencia(a4, {"lineas": {"5": {"papa": "Happy"}}}, "N9_Prueba")
    caso("choque: papa camina desde la linea 3 (paso con movimiento) y no termina sus pasos: insertar en la 5 es un ERROR con linea y personaje, y no se toca el prop",
         any("linea 5" in e and "papa" in e and "caminata" in e for e in choque.errores) and [b.line for b in a4.props[0].beats] == [3, 7] and a4.props[0].beats[0].sets_emotion == 0, str(choque.errores))
    a5 = lee_asset(ASSET_SINTETICO, "N9_Prueba", GUIDS_SINTETICOS)
    choque2 = aplica_secuencia(a5, {"lineas": {"3": {"papa": "Happy"}}}, "N9_Prueba")
    caso("sin choque: si el paso YA existe en esa linea (la 3) se le pone la emocion sin insertar nada, y el siguiente la hereda",
         not any("caminata" in e for e in choque2.errores) and [b.line for b in a5.props[0].beats] == [3, 7] and a5.props[0].beats[0].emotion == 1 and a5.props[0].beats[1].emotion == 1)
    # --- validaciones
    for titulo, ent, buscar in (("el texto citado no es el de la linea", {"lineas": {"0": {"texto": "Otra cosa", "papa": "Happy"}}}, "no es el principio"),
                                ("personaje desconocido", {"lineas": {"0": {"texto": "Una llama", "abuela": "Happy"}}}, "personaje desconocido"),
                                ("emocion desconocida (no hay cara triste)", {"lineas": {"0": {"texto": "Una llama", "papa": "Sad"}}}, "emocion desconocida"),
                                ("linea fuera de la secuencia", {"lineas": {"40": {"papa": "Happy"}}}, "no existe"),
                                ("personaje que no esta en escena", {"lineas": {"0": {"nina": "Happy"}}}, "no esta en escena"),
                                ("rumbo desconocido", {"rumbos": {"papa": {"0": "Up"}}}, "rumbo desconocido"),
                                ("voz de quien esta en escena", {"voces": {"1": {"emocion": "Happy"}}}, "esta en escena"),
                                ("voz de una acotacion", {"voces": {"0": {"emocion": "Happy"}}}, "acotacion")):
        r_ = aplica_secuencia(lee_asset(ASSET_SINTETICO, "N9_Prueba", GUIDS_SINTETICOS), ent, "N9_Prueba")
        caso("error: " + titulo, any(buscar in e for e in r_.errores), str(r_.errores))
    # --- la cobertura: la linea 1 la dice PAPA, que esta en escena
    r_ = aplica_secuencia(lee_asset(ASSET_SINTETICO, "N9_Prueba", GUIDS_SINTETICOS), {"lineas": {"2": {"papa": "Happy"}}}, "N9_Prueba")
    caso("cobertura: PAPA habla en la linea 1 y su primera emocion llega en la 2: error", any("linea 1" in e and "cobertura" in e for e in r_.errores), str(r_.errores))
    r_ = aplica_secuencia(lee_asset(ASSET_SINTETICO, "N9_Prueba", GUIDS_SINTETICOS), {"lineas": {"0": {"papa": "Happy", "mama": "Happy"}}}, "N9_Prueba")
    caso("cobertura: con una emocion en la linea 0 PAPA y MAMA (lineas 1 y 4) quedan cubiertos", not any("cobertura" in e for e in r_.errores), str(r_.errores))
    # --- la regla de la fogata, con el Facing portado de FacesLeftAt
    fuego = ASSET_SINTETICO  # papa (x 0,3) camina a x 0,6 en la linea 3 y se arrodilla (Kneel) en la 7 junto a la fogata (x 0,5): a 0,1, a la izquierda de su destino: debe mirar a la IZQUIERDA
    a6 = lee_asset(fuego, "N9_Prueba", GUIDS_SINTETICOS)
    malos_f = casos_de_espaldas_al_fuego(a6.props, 8)
    caso("fogata: papa, arrodillado en x 0,6 junto a la fogata de x 0,5, mira hacia donde camino (derecha): es un caso de espaldas", [m[:3] for m in malos_f] == [("papa", 7, "Kneel")], str(malos_f))
    r_ = aplica_secuencia(lee_asset(fuego, "N9_Prueba", GUIDS_SINTETICOS), {"rumbos": {"papa": {"7": "Left"}}}, "N9_Prueba")
    caso("fogata: el rumbo Left en el paso de la linea 7 lo corrige (1 caso antes, 0 despues)", len(r_.antes) == 1 and len(r_.despues) == 0, "%s %s" % (r_.antes, r_.despues))
    caso("FacesLeftAt portado: Facing a mano manda sobre el desplazamiento, luego el desplazamiento propio, el previo, el siguiente y el centro",
         mira_izquierda_en((0.3, 0.4), [Paso(Line=3, Action=2, Moves=1, Destination="{x: 0.6, y: 0.4}")], 3) is False and
         mira_izquierda_en((0.3, 0.4), [Paso(Line=3, Action=2, Moves=1, Destination="{x: 0.6, y: 0.4}", Facing=1)], 3) is True and
         mira_izquierda_en((0.3, 0.4), [Paso(Line=3, Action=2, Moves=1, Destination="{x: 0.6, y: 0.4}")], 5) is False and
         mira_izquierda_en((0.3, 0.4), [Paso(Line=3, Action=2, Moves=1, Destination="{x: 0.6, y: 0.4}")], 1) is False and
         mira_izquierda_en((0.7, 0.4), [], 2) is True and mira_izquierda_en((0.2, 0.4), [], 2) is False and
         mira_izquierda_en((0.5, 0.4), [Paso(Line=1, Action=2, Moves=1, Destination="{x: 0.5, y: 0.405}")], 1) is True)
    caso("Heading.FacesLeft portado: el eje dominante, el empate (horizontal) y el vector nulo",
         mira_izquierda((-1, 0)) is True and mira_izquierda((1, 0)) is False and mira_izquierda((0, 1)) is True and mira_izquierda((0, -1)) is False
         and mira_izquierda((-1, 1)) is True and mira_izquierda((1, 1)) is False and mira_izquierda((0, 0)) is None)
    caso("HeldBefore portado: Arrival del ultimo paso si se movia, si no su Action, y ActorStart antes de todo",
         sostenido_antes([Paso(Line=3, Action=2, Moves=1, Arrival=16), Paso(Line=7, Action=9, Moves=0)], 5, 2) == 5
         and sostenido_antes([Paso(Line=3, Action=2, Moves=1, Arrival=16), Paso(Line=7, Action=9, Moves=0)], 5, 5) == 16
         and sostenido_antes([Paso(Line=3, Action=2, Moves=1, Arrival=16), Paso(Line=7, Action=9, Moves=0)], 5, 9) == 9)
    # --- los CRLF se conservan
    crlf = ASSET_SINTETICO.replace("\n", "\r\n")
    a7 = lee_asset(crlf, "N9_Prueba", GUIDS_SINTETICOS)
    r7 = aplica_secuencia(a7, entrada, "N9_Prueba")
    nuevo7, _ = escribe_texto(a7, r7)
    caso("los finales de linea CRLF se conservan", "\r\n" in nuevo7 and nuevo7.replace("\r\n", "\n") == nuevo)
    # --- los assets de verdad: se parsean todos y las cuentas de siempre
    if os.path.isdir(NARRATIVAS) and os.path.isdir(PREFABS):
        guids = guids_de_prefabs()
        assets = sorted(f for f in os.listdir(NARRATIVAS) if f.endswith(".asset"))
        ok, total_beats, total_lineas = True, 0, 0
        for f in assets:
            with open(os.path.join(NARRATIVAS, f), encoding="utf-8", newline="") as fh:
                a_ = lee_asset(fh.read(), f, guids)
            total_beats += sum(len(p.beats) for p in a_.props)
            total_lineas += len(a_.textos)
            ok = ok and all(p.personaje for p in a_.props if p.es_actor)
        caso("los %d assets de verdad se parsean: todos los actores se resuelven por GUID a un personaje (%d pasos, %d lineas)" % (len(assets), total_beats, total_lineas), ok and len(assets) == 18)
    print("autoprueba de emociones_narrativas.py:", "pasa" if not malos else "FALLA en %d casos" % malos)
    return 1 if malos else 0


if __name__ == "__main__":
    sys.exit(main())
