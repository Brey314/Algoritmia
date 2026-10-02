#!/usr/bin/env python3
"""Genera el Anexo A (matriz RF -> pruebas) desde los nombres de método de Assets/Tests (CT-10).

Uso: python claudeDocs/entregables/OE3/tools/rf_matrix.py [COMMIT | --worktree] [--out RUTA]

  (sin argumentos)  lee las pruebas del último commit (git rev-parse --short HEAD).
  COMMIT            cualquier referencia de git (hash, rama, etiqueta): el anexo cita su hash corto y su fecha.
  --worktree        lee las pruebas del árbol de trabajo, con lo que aún no tiene commit: es el corte de un
                    cierre que no tendrá commit propio hasta que alguien lo haga. El anexo cita entonces
                    «el árbol de trabajo», con el commit sobre el que está. Se corre al final, no a medias
                    del trabajo de otros carriles.
  --out RUTA        escribe ahí en lugar de en src/A-matriz-rf.md (para probar sin tocar la fuente del anexo).

Sin --worktree no lee el árbol de trabajo, que otros carriles pueden tener a medias. Los nombres de
RF-01..RF-47 salen de la conversión de OE1 (docs/md/); la matriz queda en src/A-matriz-rf.md, que
tools/build.py convierte en el Anexo A.
"""
import argparse
import re
import subprocess
import sys
from collections import Counter, defaultdict
from pathlib import Path, PurePosixPath

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[3]  # tools -> OE3 -> entregables -> claudeDocs -> raíz del repo
OE1 = ROOT / "docs" / "md" / "Solución OE1_Requerimientos.md"
TESTS = "Assets/Tests"
OUT = HERE.parent / "src" / "A-matriz-rf.md"

RF_ROW = re.compile(r"^\|\s*RF-(\d{2})\s*\|\s*([^|]+?)\s*\|")
ATTR = re.compile(r"^\s*\[(Test\]|TestCase\()")
METHOD = re.compile(r"^\s*public\s+(?:static\s+)?(?:async\s+)?\S+\s+(\w+)\s*\(")
CLASS = re.compile(r"\bclass\s+(\w+)")
RF_IN_NAME = re.compile(r"_RF(\d{2})_")
PERIPHERAL = re.compile(r"Suena|Sonido|Sounds|Audio|Captura")
# Identificadores del medio del nombre que no son RF, por su prefijo (`Guion82` -> GUION).
OTHER_IDS = {
    "RNF": "requerimientos no funcionales (RNF)",
    "CP": "criterios pedagógicos (CP)",
    "CN": "criterios narrativos y de diseño (CN)",  # OE1, apartado 1.3
    "DA": "secciones de la dirección de arte",
    "INC": "hallazgos de consistencia (INC)",
    "HU": "historias de usuario (HU)",
    "CU": "casos de uso (CU)",
    "GUION": "secciones del guion",
    "CT": "restricciones técnicas (CT)",
    "OE": "definición operativa de los indicadores de OE1",
    "PG": "puntos abiertos del guion (PG)",
    "OBS": "observaciones de la evaluación funcional del OE4 (OBS)",
}


def read_rf_names():
    names = {}
    for line in OE1.read_text(encoding="utf-8").splitlines():
        m = RF_ROW.match(line)
        if m:
            names.setdefault(int(m.group(1)), " ".join(m.group(2).split()))
    assert sorted(names) == list(range(1, 48)), f"se esperaban RF-01..RF-47, hay {sorted(names)}"
    return names


def git(*args):
    return subprocess.run(["git", "-C", str(ROOT), *args], capture_output=True, check=True).stdout


def resolve(ref):
    """Hash corto del commit al que apunta `ref`."""
    try:
        return git("rev-parse", "--short", "--verify", f"{ref}^{{commit}}").decode().strip()
    except subprocess.CalledProcessError:
        raise SystemExit(f"rf_matrix: «{ref}» no es un commit de este repositorio")


def commit_date(commit):
    """Fecha del commit como DD/MM/AAAA (git la da como AAAA-MM-DD con %cs)."""
    year, month, day = git("show", "-s", "--format=%cs", commit).decode().strip().split("-")
    return f"{day}/{month}/{year}"


def test_sources(commit):
    """Los .cs de Assets/Tests como [(ruta, texto)]: los de `commit`, o los del árbol de trabajo si es None."""
    if commit is None:
        paths = sorted(p.relative_to(ROOT).as_posix() for p in (ROOT / TESTS).rglob("*.cs"))
        return [(p, (ROOT / p).read_text(encoding="utf-8-sig")) for p in paths]
    listing = git("ls-tree", "-r", "-z", "--name-only", commit, "--", TESTS).decode("utf-8")
    paths = sorted(p for p in listing.split("\0") if p.endswith(".cs"))
    return [(p, git("show", f"{commit}:{p}").decode("utf-8-sig")) for p in paths]


def scan_tests(sources):
    """Devuelve [(método, modo, módulo, clase)] de cada método marcado con [Test] o [TestCase]."""
    methods = []
    for path, text in sources:
        rel = PurePosixPath(path).relative_to(TESTS).parts
        mode, module = rel[0], ".".join(rel[1:-1])
        pending, klass = False, None
        for line in text.splitlines():
            c = CLASS.search(line)
            if c and not line.strip().startswith("//"):
                klass = c.group(1)
            if ATTR.match(line):
                pending = True
                continue
            m = pending and METHOD.match(line)
            if m:
                methods.append((m.group(1), mode, module, f"{path}:{klass}"))
                pending = False
    return methods


def other_ids(methods):
    """Cuenta los métodos que no citan un RF por el prefijo de su identificador; None = sin identificador."""
    counts = Counter()
    for name, *_ in methods:
        if RF_IN_NAME.search(name):
            continue
        parts = name.split("_")
        prefix = re.match(r"[A-Za-z]*", parts[1]).group(0).upper() if len(parts) > 2 else None
        assert prefix is None or prefix in OTHER_IDS, f"identificador sin clasificar en {name}"
        counts[prefix] += 1
    return counts


def main():
    sys.stdout.reconfigure(encoding="utf-8")  # canalizada, Windows la dejaría en cp1252 y los acentos saldrían rotos
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("commit", nargs="?", help="commit, rama o etiqueta (por defecto, HEAD)")
    parser.add_argument("--worktree", action="store_true", help="lee las pruebas del árbol de trabajo")
    parser.add_argument("--out", type=Path, default=OUT, help="archivo de salida (por defecto, src/A-matriz-rf.md)")
    args = parser.parse_args()
    if args.worktree and args.commit:
        parser.error("COMMIT y --worktree se excluyen")

    if args.worktree:
        head = resolve("HEAD")
        sources = test_sources(None)
        where = "el árbol de trabajo"
        where_long = f"{where} del corte (sobre el commit `{head}`)"
    else:
        commit = resolve(args.commit or "HEAD")
        sources = test_sources(commit)
        where = f"el commit `{commit}`"
        where_long = f"{where} ({commit_date(commit)})"

    names = read_rf_names()
    methods = scan_tests(sources)
    by_rf = defaultdict(list)
    for name, mode, module, _ in methods:
        for rf in {int(x) for x in RF_IN_NAME.findall(name)}:
            by_rf[rf].append((name, mode, module))

    covered = [rf for rf in names if by_rf[rf]]
    missing = [rf for rf in names if not by_rf[rf]]
    citing = sum(len(v) for v in by_rf.values())

    others = other_ids(methods)
    unnamed = others.pop(None, 0)
    rest = len(methods) - citing
    breakdown = "; ".join(f"{OTHER_IDS[k]}, {n}" for k, n in
                          sorted(others.items(), key=lambda kv: -kv[1]))  # estable: empates en el orden de OTHER_IDS
    unnamed_txt = "" if not unnamed else (
        " El otro no lleva identificador en su nombre: lo lleva en el de cada uno de sus casos "
        "parametrizados (véase el documento principal, apartado 9.1)." if unnamed == 1 else
        f" Los otros {unnamed} no llevan identificador en su nombre.")

    rows, examples = [], []
    for rf in sorted(names):
        tests = by_rf[rf]
        modules = Counter(module for _, _, module in tests)
        mod_txt = ", ".join(k for k, _ in sorted(modules.items(), key=lambda kv: (-kv[1], kv[0]))) or "—"
        em = sum(mode == "EditMode" for _, mode, _ in tests)
        rows.append(f"| RF-{rf:02d} | {names[rf]} | {len(tests)} ({em} EM, {len(tests) - em} PM) | {mod_txt} |")
        # Ejemplo: la prueba más corta, dejando atrás las de sonido o captura, que no muestran el RF.
        example = f"`{min(tests, key=lambda t: (bool(PERIPHERAL.search(t[0])), len(t[0]), t[0]))[0]}`" if tests else "—"
        examples.append(f"| RF-{rf:02d} | {example} |")

    closing = ("Todos los requerimientos funcionales tienen al menos una prueba que los nombra; "
               "no hay RF sin prueba.") if not missing else (
        "RF sin ninguna prueba que los nombre: " + ", ".join(f"RF-{rf:02d}" for rf in missing) + ".")

    text = f"""# ANEXO A. MATRIZ DE TRAZABILIDAD DE REQUERIMIENTOS FUNCIONALES A PRUEBAS

Esta matriz no se redactó a mano: la generó el script `rf_matrix.py`, que acompaña a este documento, a partir de los nombres de los métodos de prueba del proyecto en {where_long}. Aplica la regla CT-10 del proyecto, según la cual el nombre de cada prueba sigue el patrón `<Sujeto>_<Requisito>_<QuéHace>` y el identificador del medio es la trazabilidad. El script recorre los archivos de prueba de las carpetas EditMode y PlayMode, toma cada método marcado con `[Test]` o `[TestCase]` y lo asigna al RF que su nombre cita. Los nombres de los requerimientos se toman de la tabla de requerimientos funcionales del documento de solución del objetivo específico 1.

La columna «Pruebas» de la Tabla A.1 cuenta métodos, no casos: un método parametrizado con varios `[TestCase]` cuenta una vez, y una prueba que verifica un RF sin citarlo en su nombre no se cuenta. Entre paréntesis separa los métodos del modo EditMode («EM», lógica en C# sin escena) de los del modo PlayMode («PM», escenas reales cargadas). La columna «Módulos» indica los módulos de pruebas donde viven esos métodos, ordenados de más a menos pruebas. La Tabla A.2 muestra, como ejemplo, una de las pruebas de cada RF: la de nombre más corto entre las que verifican su comportamiento, dejando atrás las que solo comprueban el sonido o toman capturas.

De los {len(methods)} métodos de prueba del proyecto, {citing} citan un RF en su nombre y cubren {len(covered)} de los 47 RF. De los {rest} restantes, {rest - unnamed} citan otro identificador: {breakdown}.{unnamed_txt} Que todo RF tenga al menos una prueba que lo nombre lo comprueba además, de forma automática, la prueba `Traceability_CT10_TodoRFTieneAlMenosUnaPruebaQueLoNombra` en cada corrida de la suite.

**Tabla A.1.** Pruebas automatizadas que nombran cada requerimiento funcional (RF-01 a RF-47) en {where}, por modo y por módulo.

| RF | Requerimiento | Pruebas | Módulos |
|----|--------------------|----------|--------------------|
""" + "\n".join(rows) + f"""

**Tabla A.2.** Una prueba de ejemplo por requerimiento funcional en {where}.

| RF | Ejemplo de prueba |
|----|------------------------------------------|
""" + "\n".join(examples) + f"\n\n{closing}\n"
    args.out.write_text(text, encoding="utf-8", newline="\n")

    classes = {m[3] for m in methods}
    print(f"métodos={len(methods)} (EM={sum(m[1] == 'EditMode' for m in methods)}, "
          f"PM={sum(m[1] == 'PlayMode' for m in methods)}) clases={len(classes)} "
          f"citanRF={citing} RFcubiertos={len(covered)}/47 sinPrueba={missing} "
          f"otros={dict(others)} sinId={unnamed}")
    print(f"fuente: {where_long}")
    print(f"escrito: {args.out}")


if __name__ == "__main__":
    main()
