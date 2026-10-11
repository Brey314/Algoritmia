#!/usr/bin/env python3
# preparar_retrato.py: de la imagen del personaje ENTERO de la entrega del 10/10/2026 (la «tarjeta del dialogo») a la base del retrato animado, con un comando (INC-148, INC-149).
#
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_retrato.py                    # informe de los siete retratos (no escribe nada)
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_retrato.py --aplicar          # escribe los PNG y el «retrato» de arte_final.json, y regenera rig_articulaciones.json
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_retrato.py --hoja             # ademas, las dos hojas de contacto (hoja_retratos.png y hoja_caras.png)
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_retrato.py --valida           # comprueba lo ya escrito (PNG, tabla y constantes de encuadre); sale con 1 si algo falla
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_retrato.py --solo papa algoritm_fuego   # solo esos retratos
#     python3 claudeDocs/tasks/Personajes/herramientas/preparar_retrato.py --autoprueba       # se prueba solo
#         --entrega CARPETA    la entrega del 10/10 (por defecto claudeDocs/tasks/Personajes/entregas/2026-10-10): Dialogos/, <Personaje>/Expresiones/ y Algoritm/Dialogos/
#         --cabezas CARPETA    las partes de frente de la familia sin cara (por defecto entregas/2026-10-06): <Personaje>/Frente/cabeza_*
#         --torsos CARPETA     las partes de Algoritm sin cara (por defecto entregas/2026-10-09/Algoritm): <Forma>/torso_*
#         --salida CARPETA     donde dejar las hojas (por defecto, la carpeta de la entrega: entregas/2026-10-10)
#
# DECISIONES DE SANTIAGO (10/10/2026), las que fijan esta herramienta:
#   1. La tarjeta del cuadro de dialogo esta ANIMADA (parpadea y mueve la boca): el retrato es una BASE sin cara mas los sprites de cara del propio rig (ojos y boca de la expresion de
#      la linea), puestos en un rect normalizado. Esta herramienta hace la base; el motor (CharacterRig.PortraitBase y PortraitFace, NarrativeSceneController) hace el resto.
#   2. Encuadre «cabeza y hombros, igual para todos»: la cabeza del mismo tamano y a la misma altura en los retratos de un grupo (los ninos tan grandes como los adultos), y Algoritm desde
#      la punta de su forma hasta los «hombros». Una receta por GRUPO, con las mismas constantes dentro del grupo (ver 4 y 5).
#   3. Todo lo que entra a Assets/ de esta entrega mide 256 px como maximo (INC-149): el retrato es de 256 x 256.
#   4. LA FAMILIA EN PRIMER PLANO Y SIN BRAZOS (Santiago, 10/10/2026: «en los diálogos recorta los brazos, que quede en un primer plano el personaje»): en los cuatro retratos de la
#      familia se QUITAN los brazos y las manos (quita_brazos) y la cabeza ocupa FRACCION_CABEZA_FAMILIA = 0,78 del lado, con AIRE_ARRIBA_FAMILIA = 0,05 de aire arriba; quedan la cabeza,
#      el cuello y los hombros del torso.
#   5. ALGORITM NO CAMBIA (Santiago, 10/10/2026: «cuando es Algoritm quien habla, como está actualmente está bien»): sus tres retratos conservan EXACTAMENTE el encuadre de antes de la
#      decision 4 —con los brazos, la cabeza en FRACCION_CABEZA_GUIA = 0,62 del lado y AIRE_ARRIBA_GUIA = 0,07 de aire—, y por eso la herramienta ni mira sus brazos. Con --aplicar
#      conviene dar --solo papa mama nina nino: sus PNG y su recuadro de cara en arte_final.json no deben moverse (preparar_retrato.py --valida lo comprueba contra las constantes).
#
# QUE HACE (por retrato; la «pieza» es la cabeza sin cara de la familia —entregas/2026-10-06/<X>/Frente/cabeza_*— o el torso sin cara de Algoritm —entregas/2026-10-09/Algoritm/<Forma>/torso_*—;
# las dos estan en el MISMO lienzo registrado de 1300 x 1500 que la imagen del dialogo y que las expresiones):
#   1. REGISTRO. Comprueba que la imagen del dialogo esta en el lienzo de la pieza: la IoU del alfa (>= 128) dentro de la caja de la pieza tiene que ser >= IOU_FAMILIA (0,93; medida
#      0,94 a 1,00) o >= IOU_GUIA (0,97; medida 0,974 a 0,989) y la pieza tiene que quedar cubierta (COBERTURA_MIN). Por debajo se RECHAZA: una imagen de otro encuadre pondria la cara
#      del retrato fuera de sitio.
#   2. BORRA LA CARA PINTADA. La imagen del dialogo trae la cara neutra horneada (la del Nino, con la boca abierta). La mascara de la cara es la UNION del alfa (>= 12) de todas las
#      expresiones de frente del personaje, dilatada DILATA_CARA px; dentro de ella y de la silueta de la pieza, los pixeles del dialogo se sustituyen por los de la pieza (sin cara).
#      La mascara CRECE (AMPLIA_CARA) hacia lo que el dialogo pinta distinto de la pieza (la cara horneada del Nino, con la boca abierta, es mas grande que cualquiera de sus
#      expresiones) y el informe dice cuanto crecio y cuanto se pinta fuera de la silueta de la pieza.
#   3. HORNEA LA BASE ESTATICA de la cara: la nariz y el rubor (CaraBase de la neutra, tal como separa() la da, a resolucion plena) se pegan sobre la base. En Algoritm no hay nada que
#      hornear: su rig no tiene CaraBase y el rubor de los cachetes va en la capa de ojos.
#   2b. QUITA LOS BRAZOS (solo la familia). Las partes de la entrega del 06/10 (<X>/Frente/brazo_* y mano_*, su cuerpo en torso_* y cabeza_*) estan registradas con la imagen del dialogo
#      en el mismo lienzo. Mascara de los brazos = la union del alfa (>= UMBRAL) de los brazos y las manos, dilatada DILATA_BRAZOS px por el borde suavizado. Dentro de ella: si el pixel
#      esta dentro del CUERPO (torso + cabeza, alfa >= UMBRAL) gana el pixel del cuerpo —incluido su contorno— y la silueta del hombro queda entera; si esta fuera, queda transparente.
#      La dilatacion nunca come el contorno del cuerpo: solo se borra lo que cae FUERA de su mascara. Se hace antes de borrar la cara (los pasos de la cara no se enteran).
#      El cuerpo se compone en el ORDEN DEL DIALOGO (cuerpo_del_dialogo): en Papa y en el Nino la cabeza va delante del torso, pero en Mama y en la Nina el pelo largo cae DETRAS del torso y del
#      cuello; se mide donde las partes se solapan, fuera de los brazos, y gana la composicion que mas se parece al dialogo (si no, el pelo de Mama taparia su cuello).
#   4. ENCUADRE (las constantes de su grupo): arriba = el borde de arriba del alfa de la pieza (la punta de la forma en Algoritm); barbilla = la de mide_cabeza() o, si la cara
#      pintada baja mas (la barba de Papa, que cubre la barbilla), la del fondo de la caja union de la cara; en Algoritm, el fondo de su caja union. Lado S = (barbilla - arriba) /
#      fraccion del grupo; el cuadrado empieza «aire» * S sobre «arriba» y se centra en x en el centro de la caja union de la cara. Si el cuadrado se sale del lienzo se rellena con
#      transparencia. Se reduce a 256 x 256 con LANCZOS (Pillow premultiplica el alfa).
#   5. ESCRIBE (con --aplicar), sin tocar ningun .meta: Assets/Game/Art/Characters/<Carpeta>/char_<x>_retrato_base.png (familia) o Algoritm/char_algoritm_<forma>_retrato_base.png y,
#      solo en la familia, REESCRIBE EN SU SITIO char_<x>_retrato_neutra.png (mismo nombre y mismo GUID: lo referencian los prefabs) = base + ojos_neutra + boca_0, tambien de 256 x 256.
#      Algoritm conserva su _reposo como Portrait fijo.
#   6. TABLA. arte_final.json, «retrato» de cada personaje: base, recorte (caja del cuadrado en px del lienzo de la entrega), lado, cara = [x, y, ancho, alto] de la caja union de la cara
#      NORMALIZADA sobre la base con el origen ABAJO a la izquierda (lo que espera CharacterRig.PortraitFace), cabeza (arriba, barbilla, fraccion, aire), iou, cobertura,
#      cara_horneada y, solo en la familia, sin_brazos (true). articulaciones.py lleva base y cara a rig_articulaciones.json, de donde las lee BuildRigsFinal «retrato».
#   7. --hoja: hoja_retratos.png (los siete retratos a 256 y a 128 px dentro del marco de la tarjeta del juego, #C4A882 y #E0D4C0, cada uno con la cara neutra y con la de preocupacion)
#      y hoja_caras.png (cada personaje con cada expresion de frente a 256 px, de la neutra a los ojos cerrados).
#
# ORDEN. Primero preparar_expresion.py --entrega <Expresiones> --aplicar de cada personaje (la caja union de la cara, registro.cara, y las capas de cara en Assets/) y despues esta
# herramienta; y en el Editor: estado -> sprites -> retrato -> estado (BuildRigsFinal.cs.txt).

import argparse
import os
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
import preparar_expresion as E  # noqa: E402

ENTREGAS = os.path.normpath(os.path.join(AQUI, "..", "entregas"))
ENTREGA_10 = os.path.join(ENTREGAS, "2026-10-10")
CABEZAS_06 = os.path.join(ENTREGAS, "2026-10-06")
TORSOS_09 = os.path.join(ENTREGAS, "2026-10-09", "Algoritm")

LADO = 256                     # INC-149: el retrato mide 256 x 256
# ENCUADRE POR GRUPO. «fraccion» = de la punta de arriba a la barbilla, lo que ocupa la cabeza del lado del cuadrado; «aire» = del lado, lo que hay sobre la cabeza. Las mismas dos cifras
# para todo el grupo: la cabeza del mismo tamano y a la misma altura en los cuatro de la familia y en las tres formas de Algoritm.
#   Familia, PRIMER PLANO SIN BRAZOS (Santiago, 10/10/2026: «en los diálogos recorta los brazos, que quede en un primer plano el personaje»): la cabeza llena 0,78 del lado y
#   queda debajo solo lo que cabe de cuello y hombros (0,17 del lado); los brazos y las manos se quitan (quita_brazos).
FRACCION_CABEZA_FAMILIA = 0.78
AIRE_ARRIBA_FAMILIA = 0.05
#   Algoritm, EL ENCUADRE DE HOY (Santiago, 10/10/2026: «cuando es Algoritm quien habla, como está actualmente está bien»): cabeza y hombros con los brazos, 0,62 y 0,07. No se toca:
#   sus PNG, su recuadro de cara y su entrada de arte_final.json tienen que seguir igual (--valida lo vigila).
FRACCION_CABEZA_GUIA = 0.62
AIRE_ARRIBA_GUIA = 0.07
DILATA_BRAZOS = 2              # px del lienzo de la entrega: la mascara de los brazos se dilata para cubrir el borde suavizado de lo pintado (solo fuera del cuerpo se borra)
DILATA_CARA = 3                # px del lienzo de la entrega: la mascara de la cara se dilata para cubrir los halos de lo pintado
AMPLIA_CARA = 24               # px: la mascara CRECE, dentro de la caja de la cara ampliada en esto, hacia lo que el dialogo pinta distinto de la pieza (la boca abierta del Nino es mas grande
                               # que la de cualquiera de sus expresiones y deja un borde rosado fuera de la union)
DILATA_RESTO = 2               # px que se dilata lo que difiere de la pieza antes de sumarlo a la mascara
UMBRAL = 12                    # alfa desde el que un pixel cuenta (mascaras de la cara y de la pieza): por debajo hay halos sueltos casi invisibles
IOU_FAMILIA = 0.93             # registro: IoU minima del dialogo con la cabeza de la familia (medida 0,94 a 1,00)
IOU_GUIA = 0.97                # registro: IoU minima del dialogo con el torso de Algoritm (medida 0,974 a 0,989)
COBERTURA_MIN = 0.98           # registro: fraccion de la pieza que el dialogo cubre (medida 1,00)
TOL_RESTO = 40                 # diferencia por canal a partir de la cual un pixel de la base sigue siendo «cara pintada»
CRECE_MAX = 0.03               # fraccion de la caja de la cara que la mascara puede crecer antes de avisar
ENMARCADO = (0xC4, 0xA8, 0x82)   # el marco de la tarjeta del juego, #C4A882
FONDO_TARJETA = (0xE0, 0xD4, 0xC0)   # y su fondo, #E0D4C0

# clave del retrato (la del arte_final.json) -> (id de la expresion, carpeta de arte, prefijo del archivo del retrato, carpeta de la entrega de las expresiones,
#                                                 ficha del archivo de dialogo, forma o None)
FAMILIA = [("papa", "Father", "papa", "Papá"), ("mama", "Mother", "mama", "Mamá"), ("nina", "Girl", "nina", "Niña"), ("nino", "Boy", "nino", "Niño")]
# «fraccion», «aire» y «sin_brazos» son del GRUPO (ver arriba): la familia en primer plano y sin brazos, Algoritm como estaba.
RETRATOS = [{"clave": pid, "guia": False, "carpeta": carp, "prefijo": "char_%s" % ide, "entrega": ent, "ficha": ide, "forma": None, "expresion": pid,
             "fraccion": FRACCION_CABEZA_FAMILIA, "aire": AIRE_ARRIBA_FAMILIA, "sin_brazos": True}
            for pid, carp, ide, ent in FAMILIA]
RETRATOS += [{"clave": "algoritm_" + f, "guia": True, "carpeta": "Algoritm", "prefijo": "char_algoritm_%s" % f, "entrega": "Algoritm", "ficha": f, "forma": f, "expresion": E.GUIA,
              "fraccion": FRACCION_CABEZA_GUIA, "aire": AIRE_ARRIBA_GUIA, "sin_brazos": False}
             for f in E.FORMAS_GUIA]
NOMBRE_BASE = "%s_retrato_base"          # char_papa_retrato_base, char_algoritm_fuego_retrato_base
NOMBRE_NEUTRA = "%s_retrato_neutra"      # solo la familia: el retrato fijo de siempre, reescrito en su sitio


# ============================================================================ 1. localizar la entrega


def busca(carpeta, *fichas, sin=()):
    """El unico PNG de la carpeta (no recursivo) cuyas fichas NFKD-ASCII contienen todas las pedidas y ninguna de «sin»; None si no hay, ValueError si hay mas de uno."""
    if not os.path.isdir(carpeta):
        return None
    hallados = []
    for f in sorted(os.listdir(carpeta)):
        if f.lower().endswith(".png"):
            fs = E.fichas(f)
            if all(any(x.startswith(p) for x in fs) for p in fichas) and not any(any(x.startswith(p) for x in fs) for p in sin):
                hallados.append(os.path.join(carpeta, f))
    if len(hallados) > 1:
        raise ValueError("en %s hay mas de un PNG con las fichas %s: %s" % (carpeta, fichas, ", ".join(os.path.basename(h) for h in hallados)))
    return hallados[0] if hallados else None


def busca_todos(carpeta, *prefijos):
    """Todos los PNG de la carpeta (no recursivo) cuya PRIMERA ficha NFKD-ASCII empieza por alguno de los prefijos (los nombres de las partes son irregulares: «mano_papa_izquierda», «muslo_papa_dercho»), en orden."""
    if not os.path.isdir(carpeta):
        return []
    return [os.path.join(carpeta, f) for f in sorted(os.listdir(carpeta))
            if f.lower().endswith(".png") and E.fichas(f) and any(E.fichas(f)[0].startswith(p) for p in prefijos)]


def busca_carpeta(raiz, ficha):
    """La subcarpeta de «raiz» cuyo nombre (NFKD-ASCII) es o contiene la ficha, o None."""
    if not os.path.isdir(raiz):
        return None
    for d in sorted(os.listdir(raiz)):
        if os.path.isdir(os.path.join(raiz, d)) and ficha in E.fichas(d):
            return os.path.join(raiz, d)
    return None


def rutas_de(r, a):
    """
    (dialogo, pieza, expresiones, partes): donde estan, en la entrega, la imagen del dialogo, la pieza sin cara y las expresiones del retrato r; «partes» = {"brazos": [rutas de los brazos
    y las manos], "torso": ruta} en la familia (lo que quita_brazos necesita) y None en Algoritm, que conserva los brazos. Lanza ValueError con lo que falte.
    """
    partes = None
    if r["guia"]:
        dialogos = os.path.join(a.entrega, "Algoritm", "Dialogos")
        dialogo = busca(dialogos, r["ficha"])
        carpeta_forma = busca_carpeta(a.torsos, r["forma"])
        pieza = busca(carpeta_forma, "torso") if carpeta_forma else None
        expresiones = os.path.join(a.entrega, "Algoritm", "Expresiones")
    else:
        dialogo = busca(os.path.join(a.entrega, "Dialogos"), "dialogo", r["ficha"])
        carpeta_pers = busca_carpeta(a.cabezas, r["ficha"])
        frente = os.path.join(carpeta_pers, "Frente") if carpeta_pers else None
        pieza = busca(frente, "cabeza") if frente else None
        expresiones = busca_carpeta(a.entrega, r["ficha"])
        expresiones = os.path.join(expresiones, "Expresiones") if expresiones else None
        if r["sin_brazos"] and frente:
            brazos, torso = busca_todos(frente, "brazo", "mano"), busca(frente, "torso")
            if len(brazos) < 4 or torso is None:
                raise ValueError("%s: en %s faltan partes de los brazos (hay %d brazo_* y mano_*; hacen falta 4) o el torso" % (r["clave"], frente, len(brazos)))
            partes = {"brazos": brazos, "torso": torso}
    faltan = [n for n, v in (("la imagen del dialogo", dialogo), ("la pieza sin cara (cabeza o torso)", pieza), ("la carpeta de las expresiones", expresiones)) if not v]
    if faltan:
        raise ValueError("%s: no encuentro %s en la entrega" % (r["clave"], ", ".join(faltan)))
    return dialogo, pieza, expresiones, partes


# ============================================================================ 2. las operaciones sobre imagenes (puras: la autoprueba las prueba sueltas)


def mascara_alfa(im, umbral=UMBRAL):
    return im.convert("RGBA").getchannel("A").point(lambda v: 255 if v >= umbral else 0)


def registro_iou(dialogo, pieza):
    """
    (IoU, cobertura, caja de la pieza): el registro del dialogo con la pieza sin cara en el lienzo comun. IoU del alfa (>= 128) dentro de la caja de la pieza (alfa >= UMBRAL) y la
    fraccion de la pieza (alfa >= 128) que el dialogo cubre. Con la caja, lo que el dialogo trae FUERA de la pieza (el cuerpo) no cuenta; lo que cae dentro (el pelo, los hombros) si.
    """
    caja = mascara_alfa(pieza).getbbox()
    if caja is None:
        raise ValueError("la pieza esta vacia")
    d = dialogo.convert("RGBA").getchannel("A").crop(caja).point(lambda v: 255 if v >= 128 else 0)
    h = pieza.convert("RGBA").getchannel("A").crop(caja).point(lambda v: 255 if v >= 128 else 0)
    inter = ImageChops.multiply(d, h).histogram()[255]
    union = ImageChops.lighter(d, h).histogram()[255]
    total = h.histogram()[255]
    return inter / float(max(1, union)), inter / float(max(1, total)), caja


def difiere(dialogo, pieza, tol=TOL_RESTO):
    """Mascara L (0 / 255) de los pixeles donde el dialogo y la pieza difieren mas de «tol» por canal (alfa incluido)."""
    r, g, b, a = ImageChops.difference(dialogo.convert("RGBA"), pieza.convert("RGBA")).split()
    return ImageChops.lighter(ImageChops.lighter(r, g), ImageChops.lighter(b, a)).point(lambda v: 255 if v > tol else 0)


def mascara_union(imagenes, umbral=UMBRAL):
    """Mascara L (0 / 255) de la union del alfa (>= umbral) de las imagenes RGBA, que comparten lienzo."""
    out = Image.new("L", imagenes[0].size, 0)
    for im in imagenes:
        out = ImageChops.lighter(out, mascara_alfa(im, umbral))
    return out


def quita_brazos(dialogo, brazos, cuerpo, dilata=DILATA_BRAZOS):
    """
    (imagen, borrados, sustituidos, sobreviven): el dialogo SIN los brazos ni las manos (decision de Santiago, 10/10/2026: «recorta los brazos»). «brazos» son las partes RGBA de los brazos y
    las manos y «cuerpo» la imagen RGBA del torso y la cabeza compuestos como los dibuja el dialogo (cuerpo_del_dialogo), todas en el lienzo del dialogo. La mascara de los brazos (alfa >= UMBRAL, dilatada «dilata» px por el borde suavizado)
    se parte en dos: lo que cae FUERA del cuerpo (alfa >= UMBRAL) pasa a transparente, y lo que cae DENTRO toma el pixel del cuerpo, que gana con su propio contorno: asi el hombro conserva la
    silueta del torso y la dilatacion no se come su borde. «borrados» y «sustituidos» son esos dos conteos; «sobreviven» = pixeles de los brazos (sin dilatar) fuera del cuerpo que aun
    se ven (alfa >= UMBRAL): tiene que ser 0.
    """
    dialogo, cuerpo = dialogo.convert("RGBA"), cuerpo.convert("RGBA")
    brazo = mascara_union(brazos)
    zona = brazo.filter(ImageFilter.MaxFilter(2 * dilata + 1)) if dilata > 0 else brazo
    dentro_cuerpo = mascara_alfa(cuerpo)
    fuera = ImageChops.multiply(zona, ImageChops.invert(dentro_cuerpo))
    dentro = ImageChops.multiply(zona, dentro_cuerpo)
    out = dialogo.copy()
    out.paste((0, 0, 0, 0), (0, 0, out.width, out.height), fuera)
    out.paste(cuerpo, (0, 0), dentro)
    sobreviven = ImageChops.multiply(ImageChops.multiply(brazo, ImageChops.invert(dentro_cuerpo)), mascara_alfa(out)).histogram()[255]
    return out, fuera.histogram()[255], dentro.histogram()[255], sobreviven


def cuerpo_del_dialogo(dialogo, torso, cabeza, brazos, dilata=DILATA_BRAZOS):
    """
    (cuerpo, delante): el torso y la cabeza compuestos EN EL ORDEN en que los dibuja el dialogo; «delante» es "cabeza" o "torso", la parte que va por delante donde se solapan. No es el mismo
    en toda la familia: en Papa y en el Nino la cabeza (la barba, el pelo) va delante del torso, pero en Mama y en la Nina el pelo largo cae DETRAS del torso y del cuello. Se decide midiendo,
    donde las dos partes se solapan (alfa >= 128) y fuera de los brazos (la mascara dilatada de quita_brazos), cuantos pixeles del dialogo difieren (mas de TOL_RESTO) de cada
    composicion, y gana la que menos: en la medida real una da 0 y la otra miles. Con el solape vacio (o empatado) va delante la cabeza. Sin este paso, el pelo de Mama se pintaba sobre
    su cuello al rellenar el hueco que dejan las manos.
    """
    dialogo, torso, cabeza = dialogo.convert("RGBA"), torso.convert("RGBA"), cabeza.convert("RGBA")
    brazo = mascara_union(brazos)
    brazo = brazo.filter(ImageFilter.MaxFilter(2 * dilata + 1)) if dilata > 0 else brazo
    solape = ImageChops.multiply(ImageChops.multiply(mascara_alfa(cabeza, 128), mascara_alfa(torso, 128)), ImageChops.invert(brazo))
    cabeza_delante = torso.copy()
    cabeza_delante.alpha_composite(cabeza)
    torso_delante = cabeza.copy()
    torso_delante.alpha_composite(torso)
    fallos = []
    for candidato in (cabeza_delante, torso_delante):
        r, g, b, a = ImageChops.difference(candidato, dialogo).split()
        distinto = ImageChops.lighter(ImageChops.lighter(r, g), ImageChops.lighter(b, a)).point(lambda v: 255 if v > TOL_RESTO else 0)
        fallos.append(ImageChops.multiply(distinto, solape).histogram()[255])
    if fallos[1] < fallos[0]:
        return torso_delante, "torso"
    return cabeza_delante, "cabeza"


def borra_cara(dialogo, pieza, mascara_cara, amplia=AMPLIA_CARA):
    """
    (base, crecio, fuera): el dialogo SIN la cara pintada. Dentro de «mascara_cara» (la union de las expresiones, ya dilatada) y de la silueta de la pieza (alfa >= UMBRAL) los pixeles del
    dialogo pasan a ser los de la pieza. La mascara CRECE antes: dentro de su caja ampliada «amplia» px, todo lo que el dialogo pinta distinto de la pieza (mas de TOL_RESTO) y lo que
    le rodea (DILATA_RESTO) se suma, porque la cara horneada puede ser mas grande que la de cualquier expresion entregada (la boca abierta del Nino). «fuera» = pixeles de la mascara original que el dialogo pinta fuera de la silueta de la pieza
    (cara pintada sobre algo que no es la pieza: ahi no se sustituye). «crecio» = cuantos pixeles de la silueta sumo la mascara por encima de la union de las expresiones (0 si la cara
    horneada cabe en lo entregado; el Nino, que la lleva con la boca abierta, crece).
    """
    dialogo, pieza = dialogo.convert("RGBA"), pieza.convert("RGBA")
    silueta = mascara_alfa(pieza)
    original = mascara_cara
    caja0 = mascara_cara.getbbox()
    if caja0 is not None and amplia > 0:
        region = Image.new("L", mascara_cara.size, 0)
        region.paste(255, (max(0, caja0[0] - amplia), max(0, caja0[1] - amplia), min(region.width, caja0[2] + amplia), min(region.height, caja0[3] + amplia)))
        resto = ImageChops.multiply(ImageChops.multiply(difiere(dialogo, pieza), silueta), region)
        resto = resto.filter(ImageFilter.MaxFilter(2 * DILATA_RESTO + 1)) if DILATA_RESTO > 0 else resto
        mascara_cara = ImageChops.lighter(mascara_cara, ImageChops.multiply(resto, region))
    zona = ImageChops.multiply(mascara_cara, silueta)
    base = dialogo.copy()
    base.paste(pieza, (0, 0), zona)
    crecio = ImageChops.multiply(ImageChops.subtract(mascara_cara, original), silueta).histogram()[255]
    pintado = ImageChops.multiply(original, dialogo.getchannel("A").point(lambda v: 255 if v >= 128 else 0))
    fuera = ImageChops.multiply(pintado, ImageChops.invert(silueta)).histogram()[255]
    return base, crecio, fuera


def encuadre(arriba, barbilla, cx, fraccion, aire):
    """
    (x0, y0, S): el cuadrado de cabeza y hombros. S = (barbilla - arriba) / fraccion; empieza «aire» * S sobre «arriba» y se centra en x en «cx». Enteros (px del lienzo de la entrega).
    «fraccion» y «aire» son los del grupo del retrato (FRACCION_CABEZA_FAMILIA / AIRE_ARRIBA_FAMILIA o los de Algoritm): no hay valor por defecto a proposito.
    """
    lado = int(round((barbilla - arriba) / float(fraccion)))
    return int(round(cx - lado / 2.0)), int(round(arriba - aire * lado)), lado


def recorta_cuadrado(im, x0, y0, lado):
    """El cuadrado (x0, y0, lado) de «im»; lo que cae fuera del lienzo se rellena con transparencia."""
    out = Image.new("RGBA", (lado, lado), (0, 0, 0, 0))
    ax0, ay0, ax1, ay1 = max(0, x0), max(0, y0), min(im.width, x0 + lado), min(im.height, y0 + lado)
    if ax1 > ax0 and ay1 > ay0:
        out.paste(im.convert("RGBA").crop((ax0, ay0, ax1, ay1)), (ax0 - x0, ay0 - y0))
    return out


def cara_normalizada(caja, x0, y0, lado):
    """[x, y, ancho, alto] de la caja de la cara dentro del cuadrado, entre 0 y 1, con el origen ABAJO a la izquierda (CharacterRig.PortraitFace: anclajes de uGUI)."""
    return [round((caja[0] - x0) / float(lado), 6), round(1.0 - (caja[3] - y0) / float(lado), 6),
            round((caja[2] - caja[0]) / float(lado), 6), round((caja[3] - caja[1]) / float(lado), 6)]


def compone_tarjeta(base, cara, ojos, boca):
    """
    La tarjeta del dialogo tal como la compone el motor: la base y, encima, los sprites de ojos y boca estirados al rect «cara» (normalizado, origen abajo a la izquierda). «ojos» y «boca»
    son imagenes RGBA (o None). Devuelve una imagen del tamano de la base.
    """
    out = base.convert("RGBA").copy()
    w, h = out.size
    x0, y1 = cara[0] * w, (1.0 - cara[1]) * h
    rw, rh = max(1, int(round(cara[2] * w))), max(1, int(round(cara[3] * h)))
    px, py = int(round(x0)), int(round(y1 - cara[3] * h))
    for im in (ojos, boca):
        if im is not None:
            _pega_recortado(out, im, (rw, rh), (px, py))
    return out


def _pega_recortado(out, im, tam, pos):
    """alpha_composite de «im» a «tam» px en «pos», recortado a lo que cae dentro (la cara siempre cabe en el retrato; esto solo cubre redondeos de un borde)."""
    im = im.convert("RGBA").resize(tam, Image.LANCZOS)
    x, y = pos
    ax0, ay0, ax1, ay1 = max(0, x), max(0, y), min(out.width, x + tam[0]), min(out.height, y + tam[1])
    if ax1 > ax0 and ay1 > ay0:
        out.alpha_composite(im.crop((ax0 - x, ay0 - y, ax1 - x, ay1 - y)), (ax0, ay0))


def construye(dialogo, pieza, union_alfa, caja, contenido, base_cara, arriba, barbilla, iou_min, fraccion, aire, brazos=None, cuerpo=None, lado=LADO):
    """
    Todo el retrato de UNA imagen, sin tocar el disco: SimpleNamespace(base, recorte, lado_px, cara, iou, cobertura, crecio, fuera, arriba, barbilla, brazos). Lanza ValueError si el dialogo
    no esta registrado con la pieza. «union_alfa» es la mascara L (lienzo entero) de la union de las expresiones; «caja» la caja de la cara del rig (registro.cara), «contenido» la de lo pintado
    (sin margen) y «base_cara» la capa de nariz y rubor a resolucion plena (RGBA del tamano de «caja») o None. «fraccion» y «aire» son los del grupo (ver las constantes). Con «brazos» (las
    partes RGBA de los brazos y las manos) y «cuerpo» (torso y cabeza en el orden del dialogo) se quitan los brazos antes de borrar la cara; sin ellos (Algoritm) la imagen queda como esta.
    «res.brazos» = (borrados, sustituidos, sobreviven) de quita_brazos, o None.
    """
    iou, cobertura, _ = registro_iou(dialogo, pieza)
    if iou < iou_min or cobertura < COBERTURA_MIN:
        raise ValueError("el dialogo no esta registrado con la pieza: IoU %.3f (minimo %.2f) y cobertura %.3f (minimo %.2f)" % (iou, iou_min, cobertura, COBERTURA_MIN))
    quitados = None
    if brazos:
        if cuerpo is None:
            raise ValueError("para quitar los brazos hace falta el cuerpo (torso con la cabeza encima)")
        dialogo, borrados, sustituidos, sobreviven = quita_brazos(dialogo, brazos, cuerpo)
        quitados = (borrados, sustituidos, sobreviven)
    mascara = union_alfa.filter(ImageFilter.MaxFilter(2 * DILATA_CARA + 1)) if DILATA_CARA > 0 else union_alfa
    base, crecio, fuera = borra_cara(dialogo, pieza, mascara)
    if base_cara is not None:
        capa = Image.new("RGBA", base.size, (0, 0, 0, 0))
        capa.paste(base_cara.convert("RGBA"), (caja[0], caja[1]))
        base.alpha_composite(capa)
    cx = (contenido[0] + contenido[2]) / 2.0
    x0, y0, s = encuadre(arriba, barbilla, cx, fraccion, aire)
    cuadrado = recorta_cuadrado(base, x0, y0, s)
    return SimpleNamespace(base=cuadrado.resize((lado, lado), Image.LANCZOS), recorte=[x0, y0, x0 + s, y0 + s], lado_px=s, cara=cara_normalizada(caja, x0, y0, s),
                           iou=round(iou, 4), cobertura=round(cobertura, 4), crecio=crecio, fuera=fuera, arriba=arriba, barbilla=barbilla, cx=cx,
                           fraccion=fraccion, aire=aire, brazos=quitados)


# ============================================================================ 3. un retrato de la entrega


def argumentos_lote():
    """Los argumentos que preparar_expresion.prepara_lote espera (los de siempre, con el tope de 256)."""
    return SimpleNamespace(densidad=E.DENSIDAD, max_lado=E.MAX_LADO, limpia_cerrados=True)


def prepara(r, a):
    """
    El retrato r (una fila de RETRATOS) leido de la entrega: SimpleNamespace(r, rutas, retrato, lote, ...). Lanza ValueError si falta algo o no esta registrado. Pide que la caja union de la
    cara que ve el lote sea la que ya guarda arte_final.json (registro.cara): si no, falta correr preparar_expresion.py --entrega --aplicar.
    """
    dialogo_ruta, pieza_ruta, expresiones, partes = rutas_de(r, a)
    por, errores = E.lee_lote(expresiones)
    if errores:
        raise ValueError("%s: %s" % (r["clave"], "; ".join(errores)))
    lote = E.prepara_lote(r["expresion"], "frente", por, argumentos_lote())
    tabla = A.cargar_arte_final().get(r["clave"])
    guardada = (tabla or {}).get("registro", {}).get("cara")
    if guardada is None or list(guardada) != list(lote.caja):
        raise ValueError("%s: la caja de la cara del lote %s no es la de arte_final.json (%s): corre antes preparar_expresion.py --entrega %s --aplicar" % (
            r["clave"], lote.caja, guardada, expresiones))
    dialogo = Image.open(dialogo_ruta).convert("RGBA")
    pieza = Image.open(pieza_ruta).convert("RGBA")
    if list(dialogo.size) != list(lote.registro["lienzo"]) or list(pieza.size) != list(lote.registro["lienzo"]):
        raise ValueError("%s: el dialogo (%s) o la pieza (%s) no miden el lienzo registrado %s" % (r["clave"], dialogo.size, pieza.size, lote.registro["lienzo"]))
    # la union de las expresiones de frente, en el lienzo entero (las imagenes se leen otra vez, SIN la limpieza de los ojos cerrados: lo pintado es lo que hay que borrar del dialogo)
    union = Image.new("L", dialogo.size, 0)
    for emo, ruta in lote.archivos.items():
        union = ImageChops.lighter(union, mascara_alfa(Image.open(ruta)))
    brazos, cuerpo, delante = None, None, None
    if partes:
        brazos = [Image.open(ruta).convert("RGBA") for ruta in partes["brazos"]]
        torso = Image.open(partes["torso"]).convert("RGBA")
        for nombre, im in [(os.path.basename(x), i) for x, i in zip(partes["brazos"], brazos)] + [(os.path.basename(partes["torso"]), torso)]:
            if list(im.size) != list(dialogo.size):
                raise ValueError("%s: %s mide %s y el dialogo %s: las partes tienen que estar en el mismo lienzo" % (r["clave"], nombre, im.size, dialogo.size))
        cuerpo, delante = cuerpo_del_dialogo(dialogo, torso, pieza, brazos)      # lo que cubre el hombro, en el orden en que el dialogo dibuja la cabeza y el torso
    caja_pieza = mascara_alfa(pieza).getbbox()
    contenido = lote.contenido
    if r["guia"]:
        arriba, barbilla, base_cara, medida = caja_pieza[1], contenido[3], None, "el fondo de la caja union de la cara"
    else:
        cab = E.mide_cabeza(pieza.crop(caja_pieza), caja_pieza)
        barbilla = max(cab.barbilla, contenido[3])
        arriba, base_cara = caja_pieza[1], lote.emociones["neutra"].capas_plenas["base"]
        medida = "mide_cabeza (%.0f)%s" % (cab.barbilla, " y el fondo de la caja union de la cara (%d), que baja mas" % contenido[3] if contenido[3] > cab.barbilla else "")
    res = construye(dialogo, pieza, union, lote.caja, contenido, base_cara, arriba, barbilla, IOU_GUIA if r["guia"] else IOU_FAMILIA, r["fraccion"], r["aire"], brazos, cuerpo)
    res.medida_barbilla = medida
    res.delante = delante
    return SimpleNamespace(r=r, rutas=(dialogo_ruta, pieza_ruta, expresiones), retrato=res, lote=lote, caja_pieza=caja_pieza, partes=partes)


def ruta_base(r):
    return os.path.join(P.PERSONAJES_ARTE, r["carpeta"], (NOMBRE_BASE % r["prefijo"]) + ".png")


def ruta_neutra(r):
    return os.path.join(P.PERSONAJES_ARTE, r["carpeta"], (NOMBRE_NEUTRA % r["prefijo"]) + ".png")


def capa_de_cara(r, nombre):
    """La capa de cara ya escrita en Assets/ (char_<x>_<nombre>.png de Expresiones/), como RGBA, o None."""
    prefijo = "char_algoritm" if r["guia"] else r["prefijo"]
    ruta = P._png_de(r["carpeta"], "%s_%s" % (prefijo, nombre))
    return Image.open(ruta).convert("RGBA") if ruta else None


def tarjeta_con(r, base, cara, emocion):
    """La tarjeta con la cara de esa expresion de frente, hecha con las capas ya escritas en Assets/ (o None si falta alguna). Las que solo cambian los ojos o la boca llevan la otra de la neutra."""
    ojos_n, boca_n = E.EMOCIONES[emocion]
    ojos = capa_de_cara(r, ojos_n or "ojos_neutra")
    boca = capa_de_cara(r, boca_n or "boca_0")
    if ojos is None or boca is None:
        return None
    return compone_tarjeta(base, cara, ojos, boca)


# ============================================================================ 4. informe, escritura y tabla


def informe(p):
    r, res, lote = p.r, p.retrato, p.lote
    print("\n== %s" % r["clave"])
    print("  dialogo %s | pieza %s" % (os.path.basename(p.rutas[0]), os.path.basename(p.rutas[1])))
    print("  registro: IoU %.3f (minimo %.2f), cobertura %.3f; caja de la pieza %s" % (res.iou, IOU_GUIA if r["guia"] else IOU_FAMILIA, res.cobertura, list(p.caja_pieza)))
    print("  cara pintada borrada: la mascara crecio %d px sobre la union de las expresiones (lo que el dialogo pinta distinto de la pieza fuera de ellas), pintado fuera de la silueta %d px%s" % (
        res.crecio, res.fuera, "" if r["guia"] else "; nariz y rubor horneados"))
    if res.brazos is not None:
        print("  brazos quitados (%d partes de brazo y mano): %d px fuera del cuerpo borrados, %d dentro sustituidos por el torso o la cabeza (donde se solapan va delante: %s); sobreviven fuera del cuerpo: %d px" % (
            len(p.partes["brazos"]), res.brazos[0], res.brazos[1], res.delante, res.brazos[2]))
        if res.brazos[2]:
            print("  AVISO %d px de brazo siguen visibles fuera del cuerpo" % res.brazos[2])
    else:
        print("  brazos: se conservan (el encuadre de Algoritm no cambia)")
    print("  encuadre: arriba %d, barbilla %.0f (%s), lado %d px, recorte %s; cabeza %.3f del lado (pide %.2f), aire %.3f (pide %.2f)" % (
        res.arriba, res.barbilla, res.medida_barbilla, res.lado_px, res.recorte, (res.barbilla - res.arriba) / res.lado_px, res.fraccion,
        (res.arriba - res.recorte[1]) / float(res.lado_px), res.aire))
    print("  cara (x, y, ancho, alto; origen abajo a la izquierda): %s" % res.cara)
    for a_ in lote.avisos:
        print("  AVISO (lote) %s" % a_)
    area = max(1, (lote.caja[2] - lote.caja[0]) * (lote.caja[3] - lote.caja[1]))
    if res.crecio > CRECE_MAX * area:
        print("  AVISO la mascara de la cara crecio %.1f %% de la caja de la cara: la cara horneada del dialogo es bastante mas grande que las expresiones entregadas; revisa la hoja" % (100.0 * res.crecio / area))
    if res.fuera:
        print("  AVISO %d px de cara pintados fuera de la silueta de la pieza no se sustituyen" % res.fuera)


def entrada_retrato(p):
    """El «retrato» de arte_final.json de este retrato."""
    r, res = p.r, p.retrato
    entrada = {"base": NOMBRE_BASE % r["prefijo"], "fuente": os.path.basename(p.rutas[0]), "recorte": list(res.recorte), "lado": res.lado_px, "textura": LADO,
               "cara": list(res.cara), "cabeza": {"arriba": int(res.arriba), "barbilla": int(round(res.barbilla)), "fraccion": r["fraccion"], "aire": r["aire"]},
               "iou": res.iou, "cobertura": res.cobertura, "cara_horneada": bool(not r["guia"])}
    if r["sin_brazos"]:
        entrada["sin_brazos"] = True      # solo la familia: la entrada de Algoritm no gana ninguna clave (no cambia)
    return entrada


def escribe(ps, log):
    """Escribe los PNG de los retratos (la base y, en la familia, el retrato neutro reescrito en su sitio) y devuelve cuantos cambiaron. NUNCA .meta."""
    cambios = 0
    for p in ps:
        r = p.r
        destinos = [(ruta_base(r), p.retrato.base)]
        if not r["guia"]:
            neutra = tarjeta_con_en_memoria(p, "neutra")
            destinos.append((ruta_neutra(r), neutra))
        for ruta, im in destinos:
            existia = os.path.isfile(ruta)
            if existia and F._mismos_pixeles(ruta, im):
                log.append("  igual     %s" % os.path.relpath(ruta, P.RAIZ))
                continue
            os.makedirs(os.path.dirname(ruta), exist_ok=True)
            im.save(ruta)
            cambios += 1
            log.append("  %s %s  %dx%d" % ("sustituye" if existia else "nuevo    ", os.path.relpath(ruta, P.RAIZ), im.width, im.height))
    return cambios


def tarjeta_con_en_memoria(p, emocion):
    """La tarjeta del retrato p con la cara de esa emocion, con las capas que ya estan en Assets/ o, si faltan, las del lote en memoria (lo que --aplicar de preparar_expresion escribiria)."""
    r = p.r
    ojos_n, boca_n = E.EMOCIONES[emocion]
    capas = p.lote.emociones[emocion].capas_salida
    neutra = p.lote.emociones["neutra"].capas_salida
    ojos = capas.get("ojos", neutra["ojos"])
    boca = capas.get("boca", neutra["boca"])
    return compone_tarjeta(p.retrato.base, p.retrato.cara, ojos, boca)


def actualiza_tabla(ps):
    """Anota el «retrato» en las entradas de arte_final.json y regenera rig_articulaciones.json."""
    personajes = A.cargar_arte_final()
    for p in ps:
        personajes[p.r["clave"]]["retrato"] = entrada_retrato(p)
    A.guardar_arte_final(personajes)


# ============================================================================ 5. las hojas


def tarjeta_marco(im, lado):
    """La tarjeta enmarcada como la del juego: fondo #E0D4C0 y marco redondeado de 8 px #C4A882, de «lado» px de lado (el retrato va dentro, a lado - 2 * 8)."""
    marco = max(2, lado // 32)
    fondo = Image.new("RGBA", (lado, lado), (0, 0, 0, 0))
    d = ImageDraw.Draw(fondo)
    d.rounded_rectangle([0, 0, lado - 1, lado - 1], radius=lado // 10, fill=ENMARCADO + (255,))
    d.rounded_rectangle([marco, marco, lado - 1 - marco, lado - 1 - marco], radius=max(1, lado // 10 - marco), fill=FONDO_TARJETA + (255,))
    dentro = lado - 2 * marco
    fondo.alpha_composite(im.convert("RGBA").resize((dentro, dentro), Image.LANCZOS), (marco, marco))
    return fondo


def hoja_retratos(ps, ruta):
    """hoja_retratos.png: los siete retratos a 256 y a 128 px, cada uno con la cara neutra y con la de preocupacion (dos filas por tamano)."""
    pad, titulo = 14, 18
    fondo = (236, 232, 222)
    columnas = len(ps)
    ancho = columnas * (LADO + pad) + pad
    alto = titulo + (2 * (LADO + pad)) + (2 * (128 + pad)) + pad
    hoja = Image.new("RGB", (ancho, alto), fondo)
    d = ImageDraw.Draw(hoja)
    y = titulo
    for lado, filas in ((256, ("neutra", "preocupacion")), (128, ("neutra", "preocupacion"))):
        for emo in filas:
            for i, p in enumerate(ps):
                tarjeta = tarjeta_con(p.r, p.retrato.base, p.retrato.cara, emo)
                if tarjeta is None:
                    tarjeta = tarjeta_con_en_memoria(p, emo)
                celda = tarjeta_marco(tarjeta, lado)
                x = pad + i * (LADO + pad) + (LADO - lado) // 2
                bg = Image.new("RGBA", celda.size, fondo + (255,))
                bg.alpha_composite(celda)
                hoja.paste(bg.convert("RGB"), (x, y))
                if lado == 256 and emo == "neutra":
                    d.text((pad + i * (LADO + pad), 2), p.r["clave"], fill=(40, 40, 40))
            y += lado + pad
    hoja.save(ruta)
    return ruta


def hoja_caras(ps, ruta):
    """hoja_caras.png: cada personaje (una fila) con cada expresion de frente a 256 px, de la neutra a los ojos cerrados, compuesta sobre su cabeza (la tarjeta)."""
    columnas = ("neutra", "concentracion", "preocupacion", "sorpresa", "boca_a", "parpadeo_cerrado")
    pad, titulo = 8, 16
    fondo = (236, 232, 222)
    hoja = Image.new("RGB", (len(columnas) * (LADO + pad) + pad, titulo + len(ps) * (LADO + pad) + pad), fondo)
    d = ImageDraw.Draw(hoja)
    for j, emo in enumerate(columnas):
        d.text((pad + j * (LADO + pad), 2), emo, fill=(40, 40, 40))
    for i, p in enumerate(ps):
        for j, emo in enumerate(columnas):
            tarjeta = tarjeta_con(p.r, p.retrato.base, p.retrato.cara, emo)
            if tarjeta is None:
                tarjeta = tarjeta_con_en_memoria(p, emo)
            bg = Image.new("RGBA", tarjeta.size, fondo + (255,))
            bg.alpha_composite(tarjeta)
            hoja.paste(bg.convert("RGB"), (pad + j * (LADO + pad), titulo + pad + i * (LADO + pad)))
        d.text((pad + 2, titulo + pad + i * (LADO + pad) + 2), p.r["clave"], fill=(40, 40, 40))
    hoja.save(ruta)
    return ruta


# ============================================================================ 6. --valida: lo ya escrito


def valida():
    """
    Comprueba lo que ya esta en el repo: para cada retrato, el PNG de la base (256 x 256), el retrato neutro de la familia (256 x 256), el «retrato» de arte_final.json (caja de la cara
    dentro de 0..1, centrada en x), la caja de la cara de la tabla y el borde de arriba de la base. Y el ENCUADRE POR GRUPO: la cabeza ocupa la fraccion y deja el aire de su grupo
    (familia: FRACCION_CABEZA_FAMILIA y AIRE_ARRIBA_FAMILIA, «sin_brazos»; Algoritm: FRACCION_CABEZA_GUIA y AIRE_ARRIBA_GUIA, sin esa clave), tanto en la tabla como en la medida real
    de la tabla (la fraccion que sale de arriba, barbilla y lado) y el borde de arriba de cada base cae a la misma fila dentro del grupo. Devuelve la lista de errores.
    """
    errores = []
    tabla = A.cargar_arte_final()
    fracciones, aires, altos = {}, {}, {}      # por grupo: "familia" / "guia"
    for r in RETRATOS:
        g = "guia" if r["guia"] else "familia"
        e = (tabla.get(r["clave"]) or {}).get("retrato")
        if not e:
            errores.append("%s: arte_final.json no trae «retrato»: corre preparar_retrato.py --aplicar" % r["clave"])
            continue
        for ruta in [ruta_base(r)] + ([] if r["guia"] else [ruta_neutra(r)]):
            if not os.path.isfile(ruta):
                errores.append("%s: falta %s" % (r["clave"], os.path.relpath(ruta, P.RAIZ)))
            else:
                tam = Image.open(ruta).size
                if tam != (LADO, LADO):
                    errores.append("%s: %s mide %dx%d y deberia medir %dx%d" % (r["clave"], os.path.basename(ruta), tam[0], tam[1], LADO, LADO))
        x, y, w, h = e["cara"]
        if not (0 <= x and 0 <= y and w > 0 and h > 0 and x + w <= 1.0 + 1e-6 and y + h <= 1.0 + 1e-6):
            errores.append("%s: la cara %s no cabe en el cuadrado unidad" % (r["clave"], e["cara"]))
        if abs((x + w / 2.0) - 0.5) > 0.02:
            errores.append("%s: la cara no esta centrada en x (centro %.3f)" % (r["clave"], x + w / 2.0))
        caja = tabla[r["clave"]]["registro"].get("cara")
        if not (isinstance(caja, list) and len(caja) == 4):
            errores.append("%s: registro.cara no es una caja de cuatro enteros" % r["clave"])
        else:
            esperada = cara_normalizada(caja, e["recorte"][0], e["recorte"][1], e["lado"])
            if max(abs(a - b) for a, b in zip(esperada, e["cara"])) > 1e-4:
                errores.append("%s: la cara %s no es la de registro.cara %s mapeada al recorte (%s): la caja cambio despues del retrato" % (r["clave"], e["cara"], caja, esperada))
        c = e["cabeza"]
        if abs(c.get("fraccion", -1) - r["fraccion"]) > 1e-9 or abs(c.get("aire", -1) - r["aire"]) > 1e-9:
            errores.append("%s: la tabla anota la cabeza en %s del lado con %s de aire y %s pide %s y %s: corre preparar_retrato.py --aplicar --solo %s" % (
                r["clave"], c.get("fraccion"), c.get("aire"), g, r["fraccion"], r["aire"], r["clave"]))
        if bool(e.get("sin_brazos")) != r["sin_brazos"]:
            errores.append("%s: sin_brazos deberia ser %s en la tabla (la familia sin brazos, Algoritm como estaba)" % (r["clave"], r["sin_brazos"]))
        fracciones.setdefault(g, []).append((r["clave"], (c["barbilla"] - c["arriba"]) / float(e["lado"])))
        aires.setdefault(g, []).append((r["clave"], (c["arriba"] - e["recorte"][1]) / float(e["lado"])))
        if e.get("textura") != LADO or e["recorte"][2] - e["recorte"][0] != e["lado"] or e["recorte"][3] - e["recorte"][1] != e["lado"]:
            errores.append("%s: el recorte %s no es un cuadrado de lado %s" % (r["clave"], e["recorte"], e["lado"]))
        if not e.get("cara_horneada") == (not r["guia"]):
            errores.append("%s: cara_horneada deberia ser %s" % (r["clave"], not r["guia"]))
        if os.path.isfile(ruta_base(r)):
            fila = mascara_alfa(Image.open(ruta_base(r)), 24).getbbox()
            if fila is not None:
                altos.setdefault(g, []).append((r["clave"], fila[1]))
    for g, pide_f, pide_a in (("familia", FRACCION_CABEZA_FAMILIA, AIRE_ARRIBA_FAMILIA), ("guia", FRACCION_CABEZA_GUIA, AIRE_ARRIBA_GUIA)):
        fs, as_, hs = fracciones.get(g, []), aires.get(g, []), altos.get(g, [])
        if any(abs(f - pide_f) > 0.004 for _, f in fs):
            errores.append("grupo %s: la cabeza no ocupa %.2f del lado en todos: %s" % (g, pide_f, ", ".join("%s %.3f" % x for x in fs)))
        if any(abs(v - pide_a) > 0.004 for _, v in as_):
            errores.append("grupo %s: el aire sobre la cabeza no es %.2f en todos: %s" % (g, pide_a, ", ".join("%s %.3f" % x for x in as_)))
        if hs and (max(h for _, h in hs) - min(h for _, h in hs) > 3 or any(abs(h - pide_a * LADO) > 3 for _, h in hs)):
            errores.append("grupo %s: el borde de arriba de la base no cae a la misma fila (%.1f +- 3) en todos: %s" % (g, pide_a * LADO, ", ".join("%s %d" % x for x in hs)))
    return errores


# ============================================================================ 7. la autoprueba (un personaje sintetico)


AZUL_BRAZO = (40, 120, 200, 255)       # el color de los brazos del personaje sintetico: ningun pixel azul puede quedar en un retrato de la familia
NARANJA_TORSO = (220, 120, 40, 255)
CONTORNO_TORSO = (60, 30, 10, 255)


def _sintetico():
    """
    Un «personaje» sintetico en un lienzo de 1300 x 1500: la pieza (una cabeza de pelo oscuro con la piel en el ovalo, sin cara), el torso con hombros y contorno, dos brazos azules con su
    mano delante del torso y con un halo suave de 1 px (como el borde suavizado del arte real), el dialogo (todo junto mas la cara pintada) y dos expresiones.
    Devuelve (dialogo, pieza, union de las expresiones, caja de la cara (con margen), contenido, base_cara, arriba, barbilla, neutra, brazos, cuerpo): «brazos» = las partes RGBA de los brazos
    y las manos y «cuerpo» = el torso con la cabeza encima (en el sintetico va delante la cabeza), como las pide construye().
    """
    W, H = 1300, 1500
    piel, pelo = (255, 198, 159, 255), (40, 25, 15, 255)
    pieza = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    d = ImageDraw.Draw(pieza)
    d.ellipse([400, 100, 900, 700], fill=pelo)                    # el pelo
    d.ellipse([450, 330, 850, 690], fill=piel)                    # la piel del ovalo: de y 330 a 690 (la barbilla)
    torso = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    dt = ImageDraw.Draw(torso)
    dt.rectangle([520, 690, 780, 1300], fill=NARANJA_TORSO, outline=CONTORNO_TORSO, width=3)       # cuello y torso, con contorno
    dt.rectangle([440, 760, 860, 900], fill=NARANJA_TORSO, outline=CONTORNO_TORSO, width=3)        # los hombros
    dt.rectangle([525, 765, 775, 895], fill=NARANJA_TORSO)                                         # (el contorno interior se tapa: queda la silueta)
    brazo_i = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    ImageDraw.Draw(brazo_i).rectangle([300, 780, 479, 1100], fill=AZUL_BRAZO)                      # el brazo izquierdo: de 300 a 479, cruza el hombro (440..)
    brazo_d = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    ImageDraw.Draw(brazo_d).rectangle([821, 780, 1000, 1100], fill=AZUL_BRAZO)                     # el derecho: de 821 a 1000, cruza el hombro (..860)
    manos = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    ImageDraw.Draw(manos).rectangle([300, 1100, 479, 1180], fill=AZUL_BRAZO)
    ImageDraw.Draw(manos).rectangle([821, 1100, 1000, 1180], fill=AZUL_BRAZO)
    brazos = [brazo_i, brazo_d, manos]
    cuerpo = torso.copy()
    cuerpo.alpha_composite(pieza)
    dialogo = torso.copy()
    for b in brazos:                                               # el dialogo trae cada brazo con 1 px de halo suave (alfa 80) alrededor de su nucleo
        halo = b.getchannel("A").filter(ImageFilter.MaxFilter(3)).point(lambda v: 80 if v else 0)
        dialogo.alpha_composite(Image.merge("RGBA", (halo.point(lambda v: AZUL_BRAZO[0]), halo.point(lambda v: AZUL_BRAZO[1]), halo.point(lambda v: AZUL_BRAZO[2]), halo)))
        dialogo.alpha_composite(b)
    dialogo.alpha_composite(pieza)

    def cara(ojos_alto, boca_ancho):
        im = Image.new("RGBA", (W, H), (0, 0, 0, 0))
        dd = ImageDraw.Draw(im)
        for cx in (570, 730):
            dd.ellipse([cx - 40, 430 - ojos_alto // 2, cx + 40, 430 + ojos_alto // 2], fill=(255, 255, 255, 255), outline=(0, 0, 0, 255), width=4)   # ojos
        dd.line([620, 520, 680, 520], fill=(0, 0, 0, 255), width=3)                                                                                  # nariz
        dd.rounded_rectangle([650 - boca_ancho // 2, 590, 650 + boca_ancho // 2, 620], radius=10, fill=(200, 40, 60, 255))                              # boca
        for cx in (520, 780):
            dd.ellipse([cx - 25, 520, cx + 25, 560], fill=(255, 120, 130, 255))                                                                       # rubor
        return im

    neutra, abierta = cara(70, 60), cara(70, 120)
    dialogo.alpha_composite(neutra)
    union = ImageChops.lighter(mascara_alfa(neutra), mascara_alfa(abierta))
    contenido = union.getbbox()
    caja = [contenido[0] - 3, contenido[1] - 3, contenido[2] + 3, contenido[3] + 3]
    base_cara = Image.new("RGBA", (caja[2] - caja[0], caja[3] - caja[1]), (0, 0, 0, 0))
    nariz = Image.new("RGBA", (1300, 1500), (0, 0, 0, 0))
    ImageDraw.Draw(nariz).line([620, 520, 680, 520], fill=(0, 0, 0, 255), width=3)
    base_cara.alpha_composite(nariz.crop(tuple(caja)))
    return dialogo, pieza, union, caja, contenido, base_cara, 100, 690, neutra, brazos, cuerpo


def _azules(im, alfa_min=24):
    """Cuantos pixeles de «im» (RGBA) son del azul de los brazos sinteticos y se ven (alfa >= alfa_min)."""
    r, g, b, a = im.convert("RGBA").split()
    azul = ImageChops.multiply(ImageChops.multiply(r.point(lambda v: 255 if v < 110 else 0), g.point(lambda v: 255 if v < 160 else 0)),
                               ImageChops.multiply(b.point(lambda v: 255 if v > 150 else 0), a.point(lambda v: 255 if v >= alfa_min else 0)))
    return azul.histogram()[255]


def autoprueba():
    malos = 0

    def caso(nombre, ok, detalle=""):
        nonlocal malos
        malos += 0 if ok else 1
        print("%-84s %s" % (nombre, "bien" if ok else "FALLA " + detalle))

    dialogo, pieza, union, caja, contenido, base_cara, arriba, barbilla, neutra, brazos, cuerpo = _sintetico()
    F_, A_ = FRACCION_CABEZA_FAMILIA, AIRE_ARRIBA_FAMILIA
    # --- el registro
    iou, cob, _ = registro_iou(dialogo, pieza)
    caso("el registro de un dialogo bien puesto es 1,00 (IoU %.3f, cobertura %.3f)" % (iou, cob), iou > 0.97 and cob > 0.99)
    movido = Image.new("RGBA", dialogo.size, (0, 0, 0, 0))
    movido.alpha_composite(dialogo, (40, 25))
    try:
        construye(movido, pieza, union, caja, contenido, base_cara, arriba, barbilla, IOU_FAMILIA, F_, A_)
        rechazado = False
    except ValueError as e:
        rechazado = "no esta registrado" in str(e)
    caso("un dialogo corrido 40 px no esta registrado: se rechaza", rechazado)
    # --- los brazos se quitan (decision de Santiago, 10/10/2026: «recorta los brazos»)
    sin, borrados, sustituidos, sobreviven = quita_brazos(dialogo, brazos, cuerpo)
    caso("brazos: el dialogo sintetico trae brazos azules (%d px) y despues de quitarlos no queda ninguno (%d)" % (_azules(dialogo), _azules(sin)), _azules(dialogo) > 10000 and _azules(sin) == 0)
    caso("brazos: ningun pixel de brazo sobrevive fuera de la mascara del cuerpo (sobreviven %d; borrados %d, sustituidos %d)" % (sobreviven, borrados, sustituidos),
         sobreviven == 0 and borrados > 0 and sustituidos > 0)
    brazo_total = mascara_union(brazos)
    fuera_del_cuerpo = ImageChops.multiply(brazo_total, ImageChops.invert(mascara_alfa(cuerpo)))
    caso("brazos: todo pixel del brazo fuera del cuerpo quedo transparente (alfa 0)", ImageChops.multiply(fuera_del_cuerpo, mascara_alfa(sin, 1)).getbbox() is None)
    halo_a, halo_b = sin.getpixel((299, 900))[3], sin.getpixel((298, 900))[3]
    caso("brazos: el halo suave del brazo (1 px, alfa 80) y la dilatacion de %d px tambien se borran fuera del cuerpo (alfa %d y %d)" % (DILATA_BRAZOS, halo_a, halo_b), halo_a == 0 and halo_b == 0)
    caso("brazos: dentro del hombro gana el pixel del cuerpo (naranja del torso, no el azul del brazo)", sin.getpixel((470, 840)) == NARANJA_TORSO and sin.getpixel((830, 840)) == NARANJA_TORSO)
    caso("brazos: el contorno del hombro sigue ahi, entero (la dilatacion no se come el borde del cuerpo)",
         all(sin.getpixel((x, y)) == CONTORNO_TORSO for x in (440, 441, 442) for y in (850, 880)) and all(sin.getpixel((x, y)) == CONTORNO_TORSO for x in (858, 859, 860) for y in (850, 880)))
    resto = (0, 700, 1300, 1500)
    caso("brazos: debajo de la barbilla la silueta y los pixeles del dialogo son exactamente los del cuerpo (el hombro queda tal cual)", ImageChops.difference(sin.crop(resto), cuerpo.crop(resto)).getbbox() is None)
    caso("brazos: lo que no es brazo no cambia (la cabeza y la cara pintada siguen como estaban)", ImageChops.difference(sin.crop((0, 0, 1300, 690)), dialogo.crop((0, 0, 1300, 690))).getbbox() is None)
    # --- la cara pintada se borra
    res = construye(dialogo, pieza, union, caja, contenido, base_cara, arriba, barbilla, IOU_FAMILIA, F_, A_, brazos, cuerpo)
    x0, y0, s = res.recorte[0], res.recorte[1], res.lado_px
    esc = LADO / float(s)
    con_brazos = _azules(construye(dialogo, pieza, union, caja, contenido, base_cara, arriba, barbilla, IOU_FAMILIA, F_, A_).base)
    caso("construye con brazos: el cuadrado de primer plano no trae ni un pixel azul de brazo (%d) y SIN quitarlos si (%d)" % (_azules(res.base), con_brazos),
         _azules(res.base) == 0 and con_brazos > 0)
    caso("construye informa de los brazos: (borrados, sustituidos, sobreviven) = (%d, %d, %d)" % res.brazos, res.brazos == (borrados, sustituidos, sobreviven))
    try:
        construye(dialogo, pieza, union, caja, contenido, base_cara, arriba, barbilla, IOU_FAMILIA, F_, A_, brazos, None)
        pide_cuerpo = False
    except ValueError as e:
        pide_cuerpo = "cuerpo" in str(e)
    caso("quitar los brazos sin el cuerpo se rechaza (no hay con que rellenar el hombro)", pide_cuerpo)
    # --- el orden del cuerpo: la cabeza delante (Papa, Nino) o el torso delante (Mama, Nina: el pelo cae detras del cuello)
    def rect(caja, color):
        im = Image.new("RGBA", (500, 500), (0, 0, 0, 0))
        ImageDraw.Draw(im).rectangle(caja, fill=color)
        return im
    rojo, verde, azul = (200, 30, 30, 255), (30, 160, 60, 255), (40, 120, 200, 255)
    cab, tor, bra = rect([100, 100, 299, 299], rojo), rect([200, 200, 399, 399], verde), [rect([250, 150, 449, 349], azul)]
    d_cab = tor.copy()
    d_cab.alpha_composite(cab)
    d_cab.alpha_composite(bra[0])
    d_tor = cab.copy()
    d_tor.alpha_composite(tor)
    d_tor.alpha_composite(bra[0])
    c1, o1 = cuerpo_del_dialogo(d_cab, tor, cab, bra)
    c2, o2 = cuerpo_del_dialogo(d_tor, tor, cab, bra)
    caso("el orden del cuerpo se lee del dialogo: cabeza delante -> «%s», torso delante -> «%s»" % (o1, o2), (o1, o2) == ("cabeza", "torso"))
    caso("con el solape vacio o sin nada que comparar va delante la cabeza", cuerpo_del_dialogo(rect([0, 0, 10, 10], rojo), rect([300, 300, 310, 310], verde), rect([0, 0, 10, 10], rojo), bra)[1] == "cabeza")
    s1 = quita_brazos(d_cab, bra, c1)[0]
    s2 = quita_brazos(d_tor, bra, c2)[0]
    caso("el hueco del brazo se rellena con lo que habia debajo: rojo si la cabeza va delante (%s), verde si va el torso (%s); con el orden equivocado saldria el otro" % (s1.getpixel((275, 275))[:3], s2.getpixel((275, 275))[:3]),
         s1.getpixel((275, 275)) == rojo and s2.getpixel((275, 275)) == verde and quita_brazos(d_tor, bra, c1)[0].getpixel((275, 275)) == rojo)
    ojo = res.base.getpixel((int(round((570 - x0) * esc)), int(round((430 - y0) * esc))))
    caso("el ojo pintado se borro: en su sitio hay piel y no el blanco del ojo", ojo[:3] != (255, 255, 255) and abs(ojo[0] - 255) <= 6 and abs(ojo[1] - 198) <= 12, str(ojo))
    boca = res.base.getpixel((int(round((650 - x0) * esc)), int(round((605 - y0) * esc))))
    caso("la boca pintada se borro (no queda el rojo de la boca)", not (boca[0] > 180 and boca[1] < 90), str(boca))
    caso("y la cara horneada cabe en lo entregado (la mascara crece %d px) y no hay nada pintado fuera de la silueta (%d)" % (res.crecio, res.fuera), res.crecio == 0 and res.fuera == 0)
    nariz = res.base.getpixel((int(round((650 - x0) * esc)), int(round((520 - y0) * esc))))
    caso("la nariz SI esta horneada en la base (un trazo oscuro sobre la piel)", nariz[0] < 200 and nariz[3] == 255, str(nariz))
    # --- el encuadre de la familia: primer plano
    caso("lado = (barbilla - arriba) / %.2f = %d" % (F_, res.lado_px), res.lado_px == int(round((barbilla - arriba) / F_)))
    caso("el recorte es un cuadrado que empieza AIRE_ARRIBA_FAMILIA * lado sobre la cabeza", res.recorte[3] - res.recorte[1] == res.lado_px and res.recorte[2] - res.recorte[0] == res.lado_px
         and abs((arriba - res.recorte[1]) / float(res.lado_px) - A_) < 0.002)
    caso("la base mide 256 x 256", res.base.size == (LADO, LADO))
    caso("el cuadrado se centra en x en la caja de la cara", abs((res.recorte[0] + res.recorte[2]) / 2.0 - (contenido[0] + contenido[2]) / 2.0) <= 1.0)
    fila = mascara_alfa(res.base, 24).getbbox()[1]
    caso("el borde de arriba de la cabeza cae en AIRE_ARRIBA_FAMILIA * 256 = %.1f (fila %d)" % (A_ * LADO, fila), abs(fila - A_ * LADO) <= 2.0)
    # --- la cara normalizada, con el origen ABAJO a la izquierda
    x, y, w, h = res.cara
    caso("cara: x, ancho y alto salen de la caja y del lado", abs(x - (caja[0] - x0) / float(s)) < 1e-5 and abs(w - (caja[2] - caja[0]) / float(s)) < 1e-5 and abs(h - (caja[3] - caja[1]) / float(s)) < 1e-5)
    caso("cara: y cuenta desde ABAJO (anclaje de uGUI): y + alto = 1 - (borde de arriba de la caja - y0) / lado", abs((y + h) - (1.0 - (caja[1] - y0) / float(s))) < 1e-5)
    caso("cara: un ejemplo a mano: la caja (100, 200, 300, 400) en un cuadrado de 1000 en el origen es (0,1; 0,6; 0,2; 0,2)", cara_normalizada([100, 200, 300, 400], 0, 0, 1000) == [0.1, 0.6, 0.2, 0.2])
    # la tarjeta compuesta con los sprites de la cara cae sobre el sitio del ojo: la composicion usa el mismo rect
    ojos_capa = Image.new("RGBA", (caja[2] - caja[0], caja[3] - caja[1]), (0, 0, 0, 0))
    ojos_capa.alpha_composite(neutra.crop(tuple(caja)))
    tarjeta = compone_tarjeta(res.base, res.cara, ojos_capa, None)
    px = tarjeta.getpixel((int(round((570 - x0) * esc)), int(round((430 - y0) * esc))))
    caso("la tarjeta compuesta (base + capa de cara en PortraitFace) vuelve a pintar el ojo en su sitio", px[0] > 230 and px[1] > 230 and px[2] > 230, str(px))
    # --- el padding transparente si el cuadrado se sale del lienzo
    cuad = recorta_cuadrado(dialogo, -50, -40, 200)
    caso("un cuadrado que se sale del lienzo se rellena con transparencia", cuad.getpixel((5, 5))[3] == 0 and cuad.getpixel((60, 60)) == dialogo.getpixel((10, 20)) and cuad.size == (200, 200))
    # --- lo igual para dos personajes de tamanos distintos, y cada grupo con sus cifras
    a1 = encuadre(100, 690, 650, F_, A_)
    a2 = encuadre(100, 100 + (690 - 100) * 1.5, 650, F_, A_)
    caso("la receta no depende del tamano del personaje: la cabeza siempre ocupa %.2f del lado y a %.2f del borde" % (F_, A_),
         abs((690 - 100) / float(a1[2]) - F_) < 0.002 and abs((100 + 590 * 1.5 - 100) / float(a2[2]) - F_) < 0.002
         and abs((100 - a1[1]) / float(a1[2]) - A_) < 0.002 and abs((100 - a2[1]) / float(a2[2]) - A_) < 0.002)
    g1 = encuadre(100, 690, 650, FRACCION_CABEZA_GUIA, AIRE_ARRIBA_GUIA)
    caso("Algoritm conserva el encuadre de hoy: cabeza %.2f del lado y %.2f de aire (lado %d, no el de la familia %d)" % (FRACCION_CABEZA_GUIA, AIRE_ARRIBA_GUIA, g1[2], a1[2]),
         (FRACCION_CABEZA_GUIA, AIRE_ARRIBA_GUIA) == (0.62, 0.07) and g1[2] == int(round(590 / 0.62)) and abs((100 - g1[1]) / float(g1[2]) - 0.07) < 0.002 and g1[2] > a1[2])
    caso("la familia sale en primer plano (0,78 y 0,05) y sin brazos; Algoritm como estaba (0,62 y 0,07) y con brazos",
         all((r["fraccion"], r["aire"], r["sin_brazos"]) == (FRACCION_CABEZA_FAMILIA, AIRE_ARRIBA_FAMILIA, True) for r in RETRATOS if not r["guia"])
         and all((r["fraccion"], r["aire"], r["sin_brazos"]) == (0.62, 0.07, False) for r in RETRATOS if r["guia"]) and (FRACCION_CABEZA_FAMILIA, AIRE_ARRIBA_FAMILIA) == (0.78, 0.05))
    # Algoritm sin brazos que quitar: construye con la imagen como esta (sin «brazos») conserva los pixeles azules del dialogo
    guia = construye(dialogo, pieza, union, caja, contenido, base_cara, arriba, barbilla, IOU_GUIA, FRACCION_CABEZA_GUIA, AIRE_ARRIBA_GUIA)
    caso("el encuadre de Algoritm no quita nada: sin «brazos» la imagen queda como esta (res.brazos es None y los brazos siguen en el cuadrado)", guia.brazos is None and _azules(guia.base) > 0)
    # --- el marco de la tarjeta y las hojas (con el personaje sintetico)
    celda = tarjeta_marco(tarjeta, 128)
    caso("el marco: #C4A882 en el borde, #E0D4C0 detras y 128 px", celda.size == (128, 128) and celda.getpixel((64, 2))[:3] == ENMARCADO and celda.getpixel((10, 10))[:3] == FONDO_TARJETA)
    # --- ficheros: busca por fichas y --valida sobre el repo
    with tempfile.TemporaryDirectory() as tmp:
        for n in ("dialogo_niña.png", "dialogo_niño.png", "dialogo_papa.png"):
            Image.new("RGBA", (4, 4)).save(os.path.join(tmp, n))
        caso("busca: «dialogo_niña» y «dialogo_niño» no se confunden (fichas NFKD-ASCII: nina / nino)", os.path.basename(busca(tmp, "dialogo", "nina")) == "dialogo_niña.png"
             and os.path.basename(busca(tmp, "dialogo", "nino")) == "dialogo_niño.png" and busca(tmp, "dialogo", "mama") is None)
    with tempfile.TemporaryDirectory() as tmp:
        nombres = ("brazo_papa_derecho.png", "brazo_papa_izquierdo.png", "mano_papa_derecho.png", "mano_papa_izquierda.png", "muslo_papa_dercho.png", "muslo_papa_izquierdo.png",
                   "pie_papa_derecho.png", "torso_papa_frente.png", "cabeza_papa_frente.png", "Brazo_Niña_izquierdo.png", "notas.txt")
        for n in nombres:
            open(os.path.join(tmp, n), "wb").write(b"") if n.endswith(".txt") else Image.new("RGBA", (4, 4)).save(os.path.join(tmp, n))
        hallados = [os.path.basename(x) for x in busca_todos(tmp, "brazo", "mano")]
        caso("busca_todos: los brazos y las manos por el prefijo (NFKD-ASCII), con los nombres irregulares, y ni los muslos ni el torso", hallados == [
            "Brazo_Niña_izquierdo.png", "brazo_papa_derecho.png", "brazo_papa_izquierdo.png", "mano_papa_derecho.png", "mano_papa_izquierda.png"] and busca_todos(tmp + "_no_existe", "brazo") == [])
    caso("las siete filas de RETRATOS: cuatro de la familia y las tres formas de Algoritm", len(RETRATOS) == 7 and [r["clave"] for r in RETRATOS if r["guia"]] == ["algoritm_fuego", "algoritm_rueda", "algoritm_gota"])
    print("autoprueba de preparar_retrato.py:", "pasa" if not malos else "FALLA en %d casos" % malos)
    return 1 if malos else 0


# ============================================================================ 8. principal


def main(argv=None):
    ap = argparse.ArgumentParser(description="Del personaje entero del dialogo (entrega del 10/10/2026) a la base del retrato animado (ver la cabecera del script).")
    ap.add_argument("--entrega", default=ENTREGA_10, help="la entrega del 10/10 (Dialogos/, <Personaje>/Expresiones/, Algoritm/Dialogos/)")
    ap.add_argument("--cabezas", default=CABEZAS_06, help="las partes de frente de la familia sin cara (entregas/2026-10-06)")
    ap.add_argument("--torsos", default=TORSOS_09, help="las partes de Algoritm sin cara (entregas/2026-10-09/Algoritm)")
    ap.add_argument("--salida", default=None, help="donde dejar las hojas (por defecto, la carpeta de la entrega)")
    ap.add_argument("--solo", nargs="*", help="claves de retrato (papa mama nina nino algoritm_fuego algoritm_rueda algoritm_gota)")
    ap.add_argument("--aplicar", action="store_true", help="escribe los PNG y el «retrato» de arte_final.json y regenera rig_articulaciones.json")
    ap.add_argument("--hoja", action="store_true", help="escribe hoja_retratos.png y hoja_caras.png")
    ap.add_argument("--valida", action="store_true", help="comprueba lo ya escrito en el repo")
    ap.add_argument("--autoprueba", action="store_true")
    a = ap.parse_args(argv)
    if a.autoprueba:
        return autoprueba()
    if a.valida:
        errores = valida()
        for e in errores:
            print("ERROR", e)
        print("valida:", "bien: la familia cumple su primer plano sin brazos y Algoritm su encuadre de siempre" if not errores else "%d problemas" % len(errores))
        return 1 if errores else 0
    filas = [r for r in RETRATOS if not a.solo or r["clave"] in a.solo]
    desconocidos = sorted(set(a.solo or []) - {r["clave"] for r in RETRATOS})
    if desconocidos:
        ap.error("retratos desconocidos: %s" % ", ".join(desconocidos))
    ps = []
    for r in filas:
        try:
            p = prepara(r, a)
        except ValueError as e:
            print("ERROR %s" % e)
            return 2
        informe(p)
        ps.append(p)
    salida = a.salida or a.entrega
    if a.hoja:
        os.makedirs(salida, exist_ok=True)
        print("\nhoja: %s" % hoja_retratos(ps, os.path.join(salida, "hoja_retratos.png")))
        print("hoja: %s" % hoja_caras(ps, os.path.join(salida, "hoja_caras.png")))
    if not a.aplicar:
        print("\n(sin --aplicar: no se escribio nada)")
        return 0
    log = []
    escribe(ps, log)
    print("\n== Aplicando al repo ==")
    print("\n".join(log))
    actualiza_tabla(ps)
    print("  arte_final.json: «retrato» de %s" % ", ".join(p.r["clave"] for p in ps))
    fallos = 0
    fallos += 1 if E.corre([sys.executable, os.path.join(AQUI, "articulaciones.py")], "articulaciones") else 0
    errores = valida() if len(ps) == len(RETRATOS) else []
    for e in errores:
        print("ERROR", e)
    print("\n%s" % ("TODO EN VERDE" if not fallos and not errores else "revisa antes de entregar"))
    print("""
==================== Sesion local (Santiago), con el Editor abierto ====================
 Copy-Item claudeDocs/tasks/Personajes/herramientas/BuildRigsFinal.cs.txt Assets/Editor/ClaudeBuildRigsFinal.cs ; editor.ps1 recompile
 ClaudeBuildRigsFinal.Execute("estado") -> Execute("sprites") -> Execute("retrato") -> Execute("estado")     // PortraitBase y PortraitFace de los siete prefabs
========================================================================================""")
    return 1 if fallos or errores else 0


if __name__ == "__main__":
    sys.exit(main())
