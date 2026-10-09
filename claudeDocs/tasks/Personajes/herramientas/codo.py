#!/usr/bin/env python3
# codo.py: HUMERO Y ANTEBRAZO ENCAJADOS EN EL CODO del arte final de la familia (Santiago, 08/10/2026).
#
#     python3 claudeDocs/tasks/Personajes/herramientas/codo.py                      # propone el ajuste de los cuatro y lo compara con arte_final.json
#     python3 claudeDocs/tasks/Personajes/herramientas/codo.py --aplica             # y lo escribe en arte_final.json (rects del humero y del antebrazo, punto del codo, holgura)
#     python3 claudeDocs/tasks/Personajes/herramientas/codo.py --verifica           # mide lo que HAY en arte_final.json (sale con 1 si un humero asoma del codo)
#     python3 claudeDocs/tasks/Personajes/herramientas/codo.py --autoprueba         # la medida y el ajuste detectan y arreglan un codo mal encajado (con el Nino)
#         ids: papa mama nina nino (por defecto, los cuatro)
#
# EL PROBLEMA. «En las animaciones del papa, cuando el mueve el antebrazo se ve parte del humero.» El antebrazo gira sobre el codo y su CASQUETE (el
# extremo redondo del lado del hombro) tiene que tapar el extremo del humero. El arte de Papa entrego el humero con una punta clara y sin contorno que
# se pasa de su extremo redondo, y el antebrazo SOLAPADO sobre ella (42 px a la izquierda, 21 a la derecha: preparar_arte_final.py lo avisaba y lo
# dejaba en «holguras»): con el brazo recto la punta queda bajo el antebrazo y no se ve, pero el codo giraba en el extremo del humero y no en el
# casquete, asi que al doblarlo el casquete se apartaba y la punta asomaba como un bulto sin contorno. Mama, la Nina y el Nino entregaron el humero
# terminando en un extremo redondo con contorno que cabe en el casquete.
#
# LA REGLA. pose_preview.asoma_codo mide, por brazo, cuanto pasa del borde del casquete el punto del humero mas lejano de su centro (A, px). Si A pasa
# de pose_preview.ASOMA_MAX_CODO, el humero tiene que SUBIR y el antebrazo BAJAR, a lo largo del eje del humero, hasta que ese punto quede MARGEN px dentro
# del casquete: entre los dos suman R = A + MARGEN. Los DOS antebrazos bajan lo mismo, lo que pide el brazo que menos pide (D = min R) y el humero que
# mas pide sube el resto: asi los dos brazos conservan el largo relativo que dibuja el arte (Papa: los dos antebrazos bajan 37 px y solo el humero
# izquierdo sube 19). Si el humero subiera todo (con el hombro quieto) se escondia tras el torso y el reposo de los brazos pasaba de 15 a 49 grados (medido
# el 08/10/2026: con 30 px o mas en el humero izquierdo el abrazo de Papa pasa de la tolerancia de 6 grados del codo, y con 32 o mas el reposo se abre); si
# solo bajara el antebrazo, un brazo quedaria 19 px mas largo que el otro. El hombro NO se mueve: el pivote queda donde hombro.py lo puso y el humero se
# desliza bajo el, a lo largo de su eje (sigue dentro de su capsula). El pivote del codo va al centro del casquete.
#
# Es el paso que sigue a hombro.py (el pivote del hombro se mide con el humero entregado) y precede a articulaciones.py, coreografia.py y pose_preview.py:
# preparar_arte_final.py --aplicar lo corre en ese orden. Idempotente: con los codos ya encajados no cambia nada. Coordenadas: las del lienzo de 1024.
#
# Sin dependencias fuera de Pillow, como el resto de las herramientas.

import argparse
import copy
import math
import os
import subprocess
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
import articulaciones as A  # noqa: E402
import pose_preview as V  # noqa: E402
import prefabs as P  # noqa: E402

MARGEN = 4.0  # px que la punta del humero queda dentro del borde del casquete (el casquete mide su radio sobre el alfa, sin el contorno suavizado)


def propone(pid, tabla):
    """
    {lado: {"sube", "baja", "eje", "fp", "hd", "asoma"}}: lo que sube el humero y baja el antebrazo (px, a lo largo de «eje», el vector unitario del codo al
    hombro) para que el humero no asome (ver la cabecera). «tabla»: la entrada del personaje en arte_final.json. Un brazo que ya encaja (A <= tope) no pide nada.
    """
    medidas = V.asoma_codo(pid, tabla)
    pide = {l: (m["asoma"] + MARGEN if m["asoma"] > V.ASOMA_MAX_CODO else 0.0) for l, m in medidas.items()}
    baja = min(pide.values())
    plan = {}
    for lado, m in medidas.items():
        ux, uy = m["hp"][0] - m["hd"][0], m["hp"][1] - m["hd"][1]
        largo = math.hypot(ux, uy)
        plan[lado] = {"sube": pide[lado] - baja, "baja": baja if pide[lado] else 0.0, "eje": (ux / largo, uy / largo),
                      "fp": m["fp"], "hd": m["hd"], "asoma": m["asoma"]}
    return plan


def aplica(tabla, plan):
    """
    Escribe el plan en «tabla» (en sitio): el rect del humero sube, el del antebrazo baja y el punto del codo va al centro del casquete. No toca el pivote
    del hombro. La holgura del codo (arte_final.json, «holguras») pasa a ser la distancia entre el centro del extremo del humero y el del casquete. Devuelve
    cuantos brazos cambio.
    """
    partes = {q["nombre"]: q for q in tabla["partes"]}
    nodos = {n["nombre"]: n for n in tabla["nodos"]}
    cambios = 0
    for lado, p in plan.items():
        if not p["sube"] and not p["baja"]:
            continue
        ux, uy = p["eje"]
        h = (round(p["sube"] * ux), round(p["sube"] * uy))
        f = (round(-p["baja"] * ux), round(-p["baja"] * uy))
        q, n = partes["Brazo" + lado], nodos["Codo" + lado]
        q["rect"] = [q["rect"][0] + h[0], q["rect"][1] + h[1], q["rect"][2] + h[0], q["rect"][3] + h[1]]
        n["rect"] = [n["rect"][0] + f[0], n["rect"][1] + f[1], n["rect"][2] + f[0], n["rect"][3] + f[1]]
        n["punto"] = [round(p["fp"][0] + f[0]), round(p["fp"][1] + f[1])]
        holgura = math.hypot(p["fp"][0] + f[0] - p["hd"][0] - h[0], p["fp"][1] + f[1] - p["hd"][1] - h[1])
        holguras = tabla.setdefault("holguras", {})
        if holgura > 2.0:
            holguras["Codo" + lado] = round(holgura, 1)
        else:
            holguras.pop("Codo" + lado, None)
        cambios += 1
    return cambios


def _linea(pid, lado, p, despues=None):
    return "%-5s %-3s asoma %5.1f px | el humero sube %4.1f, el antebrazo baja %4.1f px%s" % (
        pid, lado, p["asoma"], p["sube"], p["baja"], "" if despues is None else " -> asoma %.1f px" % despues)


def _autoprueba():
    """
    La medida y el ajuste con el Nino (su humero encaja): (a) no pide nada; (b) con los dos antebrazos subidos 40 px a lo largo del brazo, el humero se pasa
    del casquete, se detecta, el ajuste lo deja dentro del tope, el pivote del codo cae en el casquete y el del hombro no se mueve; (c) un segundo ajuste no
    cambia nada; (d) con un solo brazo desencajado, el otro no se toca.
    """
    malos = 0
    nino = P.personaje_rig(P.cargar_rig(), "nino")
    tabla = {"nodos": copy.deepcopy(nino["nodos"]), "partes": copy.deepcopy(nino["partes"])}
    base = propone("nino", tabla)

    def sube_antebrazo(t, lados, px=40):
        for lado in lados:
            m = V.asoma_codo("nino", t)[lado]
            ux, uy = m["hp"][0] - m["hd"][0], m["hp"][1] - m["hd"][1]
            largo = math.hypot(ux, uy)
            n = next(n for n in t["nodos"] if n["nombre"] == "Codo" + lado)
            dx, dy = round(px * ux / largo), round(px * uy / largo)
            n["rect"] = [n["rect"][0] + dx, n["rect"][1] + dy, n["rect"][2] + dx, n["rect"][3] + dy]

    mal = copy.deepcopy(tabla)
    sube_antebrazo(mal, ("Izq", "Der"))
    plan = propone("nino", mal)
    pivotes = {q["nombre"]: list(q["pivote"]) for q in mal["partes"] if q["nombre"].startswith("Brazo")}
    cambio = aplica(mal, plan)
    despues = V.asoma_codo("nino", mal)
    nodos = {n["nombre"]: n for n in mal["nodos"]}
    cae = all(math.hypot(nodos["Codo" + l]["punto"][0] - despues[l]["fp"][0], nodos["Codo" + l]["punto"][1] - despues[l]["fp"][1]) <= 1.5 for l in ("Izq", "Der"))
    segundo = propone("nino", mal)
    una = copy.deepcopy(tabla)
    sube_antebrazo(una, ("Izq",))
    plan_una = propone("nino", una)
    casos = [
        ("el Nino ya encaja: no pide nada", not any(p["sube"] or p["baja"] for p in base.values())),
        ("con el antebrazo subido 40 px, el humero asoma (%.0f y %.0f px)" % (plan["Izq"]["asoma"], plan["Der"]["asoma"]), all(p["asoma"] > V.ASOMA_MAX_CODO for p in plan.values())),
        ("el ajuste cambia los dos brazos", cambio == 2),
        ("despues asoma %.1f y %.1f px (tope %.0f)" % (despues["Izq"]["asoma"], despues["Der"]["asoma"], V.ASOMA_MAX_CODO), all(d["asoma"] <= V.ASOMA_MAX_CODO for d in despues.values())),
        ("los dos antebrazos bajan lo mismo", abs(plan["Izq"]["baja"] - plan["Der"]["baja"]) < 1e-9),
        ("el pivote del codo cae en el casquete", cae),
        ("el pivote del hombro no se mueve", all(list(q["pivote"]) == pivotes[q["nombre"]] for q in mal["partes"] if q["nombre"].startswith("Brazo"))),
        ("un segundo ajuste no cambia nada", not any(p["sube"] or p["baja"] for p in segundo.values())),
        ("con un solo brazo desencajado, el otro no se toca", plan_una["Der"]["sube"] == 0 and plan_una["Der"]["baja"] == 0 and plan_una["Izq"]["sube"] > 0),
    ]
    for nombre, ok in casos:
        malos += 0 if ok else 1
        print("  %-70s %s" % (nombre, "ok" if ok else "FALLA"))
    print("autoprueba de codo.py:", "pasa" if not malos else "FALLA en %d casos" % malos)
    return 1 if malos else 0


def main(argv=None):
    ap = argparse.ArgumentParser(description="Humero y antebrazo encajados en el codo del arte final (ver la cabecera).")
    ap.add_argument("ids", nargs="*", help="papa mama nina nino (por defecto, los cuatro)")
    ap.add_argument("--aplica", action="store_true", help="escribe el ajuste en arte_final.json")
    ap.add_argument("--verifica", action="store_true", help="mide lo que hay en arte_final.json (sale con 1 si un humero asoma)")
    ap.add_argument("--autoprueba", action="store_true")
    a = ap.parse_args(argv)
    if a.autoprueba:
        return _autoprueba()
    ids = a.ids or list(P.FAMILIA)
    for pid in ids:
        if pid not in P.FAMILIA:
            ap.error("personaje desconocido: %s" % pid)
    personajes = A.cargar_arte_final()
    if a.verifica:
        fallos = 0
        for pid in ids:
            for lado, m in V.asoma_codo(pid, personajes[pid]).items():
                ok = m["asoma"] <= V.ASOMA_MAX_CODO
                fallos += 0 if ok else 1
                print("%-5s %-3s la punta del humero pasa %5.1f px del casquete del antebrazo (maximo %.0f) %s" % (
                    pid, lado, m["asoma"], V.ASOMA_MAX_CODO, "ok" if ok else "FALLA"))
        print("los codos encajan" if not fallos else "%d humeros asoman del codo" % fallos)
        return 1 if fallos else 0
    cambios = 0
    for pid in ids:
        plan = propone(pid, personajes[pid])
        ajustada = copy.deepcopy(personajes[pid])
        hechos = aplica(ajustada, plan)
        despues = V.asoma_codo(pid, ajustada) if hechos else None
        for lado, p in plan.items():
            print(_linea(pid, lado, p, None if despues is None else despues[lado]["asoma"]))
        if a.aplica:
            personajes[pid] = ajustada
        cambios += hechos
    if a.aplica:
        if cambios:
            A.guardar_arte_final(personajes)
            # rig_articulaciones.json sale de arte_final.json: se regenera en otro proceso (articulaciones.py lee la tabla al importarse)
            subprocess.call([sys.executable, "-B", os.path.join(AQUI, "articulaciones.py")], stdout=subprocess.DEVNULL)
        print("arte_final.json: %d brazos ajustados%s" % (cambios, " y rig_articulaciones.json regenerado (falta coreografia.py)" if cambios else ""))
    return 0


if __name__ == "__main__":
    sys.exit(main())
