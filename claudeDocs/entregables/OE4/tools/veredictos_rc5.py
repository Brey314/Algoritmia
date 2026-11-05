"""Consolida los veredictos de la regresión sobre rc5 en src/veredictos-rc5.csv y comprueba el resultado.

Uso (desde cualquier carpeta del repositorio):
    python claudeDocs/entregables/OE4/tools/veredictos_rc5.py [--check]

  (sin opciones)  escribe src/veredictos-rc5.csv y corre las comprobaciones.
  --check         no escribe: compara el CSV en disco con lo que se generaría y corre las comprobaciones.

Fuente: claudeDocs/tasks/OE4/evidencias/rc5/*/veredictos.csv, sin rc5/GP/ (recorridos del 16/10/2026, con
Word abierto). Un caso con varias filas toma la peor (F > B > NE > PD > P > NA). Los seis casos que esperan a
una persona van como PS, que casos.py deja fuera de la eficacia. Precondición y esperado salen de casos.md;
fecha, candidato, observado y registro, de las sesiones; versión, commit y huella, del manifiesto y de git.

Las columnas caso y veredicto van primero porque casos.py lee row[0] y row[1].
Sale con 0 si todo cuadra y con 1 si alguna comprobación falla.
"""
import argparse
import csv
import io
import re
import subprocess
import sys
from collections import Counter

sys.dont_write_bytecode = True
import casos

EVIDENCIAS = casos.CASOS.parent / "evidencias"
RC5 = EVIDENCIAS / "rc5"
MANIFIESTO = RC5 / "manifiesto-rc5.tsv"
SALIDA = casos.HERE.parent / "src" / "veredictos-rc5.csv"
EXCLUIDAS = {"GP"}
PEOR = ("F", "B", "NE", "PD", "P", "NA")
CAMPOS = ("caso", "veredicto", "fecha", "candidato", "precondicion", "esperado", "observado", "registro",
          "version", "commit", "huella")
TAG = "rc5"
ESPERADO = {"filas": 145, "P": 133, "PD": 2, "F": 4, "PS": 6}
# Hoja HUM que ejecuta cada caso pendiente. PF-RNF08-01 tiene fila NE en S-RNF; los otros cinco, ninguna.
PENDIENTES = {"PF-RNF08-01": "H4", "PF-RNF07-02": "H3", "PF-CT02-01": "H5",
              "PF-SON-01": "H12", "PF-SON-02": "H12", "PF-SON-03": "H12, H6 y H7"}
HOJA = "claudeDocs/tasks/OE4/Hoja-HUM.md"
SECCION = re.compile(r"^## (S-[A-Z0-9]+) · ")
BLOQUE_PER = re.compile(r"^\*\*(PF-[A-Z0-9]+-\d\d) · ")
PRE_PAU = ("Semilla y camino de la tabla «Cómo se llega a cada escena» de S-PAU (OE4_Z, OE4_B2, OE4_B3 u OE4_D, "
           "siempre con Preparar-Datos antes)")
PRE_RNF = "Se consolidan los registros de las demás sesiones; lo que falta se completa en la sesión"


def una_linea(texto):
    return " ".join(texto.split())


def catalogo():
    """{caso: (precondición, esperado)}. La precondición es el «**Pre:**» de la sesión; S-PAU, S-PER, S-RNF,
    S-INSP y S-HUM no lo tienen, y ahí vale su tabla de escenas, su bloque propio o la columna del método."""
    lineas = casos.CASOS.read_text(encoding="utf-8").splitlines()
    pre, bloque, filas, sesion = {}, {}, {}, None
    for n, linea in enumerate(lineas):
        if m := SECCION.match(linea):
            sesion = m.group(1)
        parrafo = []
        if linea.startswith("**Pre:**") or BLOQUE_PER.match(linea):
            for siguiente in lineas[n:]:
                if not siguiente.strip():
                    break
                parrafo.append(siguiente)
            texto = una_linea(" ".join(parrafo))
            if linea.startswith("**Pre:**"):
                # Lo que sigue al primer rótulo en negrita son coordenadas y cifras previstas, no la precondición.
                pre[sesion] = texto.removeprefix("**Pre:**").split(" **")[0].strip()
            else:
                bloque[BLOQUE_PER.match(linea).group(1)] = texto
        if sesion and (m := casos.ROW.match(linea)):
            celdas = [c.strip() for c in linea.strip().strip("|").split("|")]
            filas[m.group(1)] = (sesion, celdas)
    resultado, anterior = {}, ""
    for caso, (sesion, celdas) in filas.items():
        if sesion in pre:
            previa = pre[sesion]
        elif sesion == "S-PAU":
            previa = PRE_PAU
        elif sesion == "S-RNF":
            previa = f"{PRE_RNF}: {celdas[-2]}"
        elif sesion in ("S-INSP", "S-HUM"):
            previa = celdas[-2].replace("Ídem para", "Como PF-SON-01, para").replace("ídem", "como PF-SON-01")
        else:
            previa = bloque.get(caso, "Cada bloque de S-PER empieza con su Preparar-Datos y termina con Revisar-Log")
        # «ídem» en la tabla de S-HUM remite al caso de la fila anterior.
        esperado = anterior if celdas[-1] == "ídem" else celdas[-1]
        anterior = esperado
        resultado[caso] = (previa, esperado)
    return resultado


def sesiones():
    """{caso: [(sesión, fila)]} de los CSV de rc5."""
    filas = {}
    for ruta in sorted(RC5.glob("*/veredictos.csv")):
        if ruta.parent.name in EXCLUIDAS:
            continue
        with ruta.open(encoding="utf-8-sig", newline="") as archivo:
            for fila in csv.DictReader(archivo):
                filas.setdefault(fila["caso"], []).append((ruta.parent.name, fila))
    return filas


def identidad():
    """(huella global, huella corta de 8, commit completo) de rc5."""
    cabecera = dict(l[2:].split("\t")[:2] for l in MANIFIESTO.read_text(encoding="utf-8").splitlines()
                    if l.startswith("# ") and "\t" in l)
    commit = subprocess.run(["git", "-C", str(casos.ROOT), "rev-parse", f"{TAG}^{{commit}}"],
                            capture_output=True, text=True, check=True).stdout.strip()
    return cabecera["huella_global"], commit


def consolidar():
    huella, commit = identidad()
    previas, filas = catalogo(), sesiones()
    _, lista = casos.parse()
    salida = []
    for caso in (c["id"] for c in lista):
        partes = filas.get(caso, [])
        peor = min((f["veredicto"] for _, f in partes), key=PEOR.index, default=None)
        previa, esperado = previas[caso]
        if caso in PENDIENTES:
            if peor not in (None, "NE"):
                raise SystemExit(f"{caso} figura como pendiente y tiene veredicto {peor}")
            salida.append((caso, "PS", "", TAG, previa, esperado,
                           f"[Pendiente: ejecutar sobre rc5 la hoja HUM, {PENDIENTES[caso]}.] "
                           "El resultado del 13/10/2026 es de rc3 y no se hereda.",
                           HOJA, TAG, commit, huella))
            continue
        if not partes:
            raise SystemExit(f"{caso} no tiene veredicto y no es un pendiente declarado")
        fechas = sorted({f["fecha"] for _, f in partes})
        vistos = [(s, f) for s, f in partes]
        observado = (una_linea(vistos[0][1]["observado"]) if len(vistos) == 1 else
                     " // ".join(f"{s}: {una_linea(f['observado'])}" for s, f in vistos))
        rutas = list(dict.fromkeys(r.strip() for _, f in partes for r in f["registro"].split(";") if r.strip()))
        candidatos = {f["candidato"] for _, f in partes}
        huellas = {f["huella"] for _, f in partes}
        if candidatos != {TAG} or any(not huella.startswith(h) for h in huellas):
            raise SystemExit(f"{caso}: candidato {candidatos} o huella {huellas} no son los de rc5")
        salida.append((caso, peor, "; ".join(fechas), TAG, previa, esperado, observado, "; ".join(rutas),
                       TAG, commit, huella))
    return salida


def como_texto(filas):
    buffer = io.StringIO()
    escritor = csv.writer(buffer, lineterminator="\n")
    escritor.writerow(CAMPOS)
    escritor.writerows(filas)
    return buffer.getvalue()


def comprobar(filas):
    problemas = []
    _, lista = casos.parse()
    ids = [f[0] for f in filas]
    if len(ids) != ESPERADO["filas"]:
        problemas.append(f"{len(ids)} filas, se esperaban {ESPERADO['filas']}")
    if sorted(ids) != sorted(c["id"] for c in lista) or len(set(ids)) != len(ids):
        problemas.append("los casos no son exactamente los del catálogo, una vez cada uno")
    cuenta = Counter(f[1] for f in filas)
    for veredicto in ("P", "PD", "F", "PS"):
        if cuenta[veredicto] != ESPERADO[veredicto]:
            problemas.append(f"{cuenta[veredicto]} {veredicto}, se esperaban {ESPERADO[veredicto]}")
    if set(cuenta) - {"P", "PD", "F", "PS"}:
        problemas.append(f"veredictos inesperados: {cuenta}")
    rutas = 0
    for fila in filas:
        registro = dict(zip(CAMPOS, fila))
        if any(not registro[campo] for campo in CAMPOS if campo != "fecha" or registro["veredicto"] != "PS"):
            problemas.append(f"{fila[0]}: campo vacío")
        for ruta in registro["registro"].split("; "):
            rutas += 1
            if not ((EVIDENCIAS / ruta).exists() or (casos.ROOT / ruta).exists()):
                problemas.append(f"{fila[0]}: no existe {ruta}")
    print(f"{len(ids)} filas · {dict(cuenta)} · {rutas} rutas de registro comprobadas")
    return problemas


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--check", action="store_true", help="no escribe; compara con el CSV en disco")
    args = parser.parse_args()
    filas = consolidar()
    texto = como_texto(filas)
    if args.check:
        problemas = [] if SALIDA.read_text(encoding="utf-8") == texto else [f"{SALIDA.name} no está al día"]
    else:
        SALIDA.write_text(texto, encoding="utf-8", newline="\n")
        problemas = []
    problemas += comprobar(filas)
    for problema in problemas:
        print(f"FALLA: {problema}")
    print("comprobaciones: todas pasan" if not problemas else f"comprobaciones: {len(problemas)} fallan")
    return 1 if problemas else 0


if __name__ == "__main__":
    sys.exit(main())
