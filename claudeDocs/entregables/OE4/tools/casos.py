"""Lee el catálogo de casos PF-* (claudeDocs/tasks/OE4/casos.md) y los veredictos de la pasada
(src/veredictos-rc3.csv) y escribe las tablas que el documento no debe contar a mano.

Veredictos: P, PD, F, B, NA. Sin veredicto, o sin archivo, cada celda queda en PENDIENTE-CIFRA y la
guarda de build.py impide publicar. El CSV es «caso,veredicto» (líneas con # se ignoran).
"""
import csv
import re
from collections import Counter, OrderedDict
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[3]
CASOS = ROOT / "claudeDocs" / "tasks" / "OE4" / "casos.md"
OE1 = ROOT / "docs" / "md" / "Solución OE1_Requerimientos.md"
VEREDICTOS = HERE.parent / "src" / "veredictos-rc3.csv"
PENDING = "PENDIENTE-CIFRA"
VERDICTS = ("P", "PD", "F", "B", "NA")
LABELS = ("NAV", "BOT", "SON", "RETO", "DAT", "RNF")
HEADER = re.compile(r"^## (S-[A-Z0-9]+) · (.+?) — (.+?) · (.+)$")
ROW = re.compile(r"^\| (PF-[A-Z0-9]+-\d\d) \|")


def parse():
    """Devuelve (sesiones, casos): sesiones {id: (título, ejecutor)} y casos [dict] en el orden del catálogo."""
    sessions, cases, current = OrderedDict(), [], None
    for line in CASOS.read_text(encoding="utf-8").splitlines():
        h = HEADER.match(line)
        if h:
            current = h.group(1)
            sessions[current] = (h.group(2), h.group(4).strip())
            continue
        m = ROW.match(line)
        if m and current:
            cells = [c.strip() for c in line.strip().strip("|").split("|")]
            executor = cells[3] if current == "S-INSP" else sessions[current][1]
            req = re.match(r"(RF|RNF|CT)-(\d+)", cells[1])
            cases.append({"id": m.group(1), "req": cells[1].split("·")[0].strip(), "label": cells[2],
                          "session": current, "executor": executor,
                          "key": req.group(0) if req else cells[1]})
    return sessions, cases


def primary(case_id):
    """Requerimiento principal, desde el id: PF-RF04-02 -> RF-04; PF-SON-01 -> Sonido (KPI)."""
    m = re.match(r"PF-(RF|RNF|CT)(\d+)-", case_id)
    return f"{m.group(1)}-{m.group(2)}" if m else "Sonido (KPI)"


def verdicts(cases):
    known = {c["id"] for c in cases}
    found = {}
    if VEREDICTOS.exists():
        for row in csv.reader(l for l in VEREDICTOS.read_text(encoding="utf-8").splitlines()
                              if l.strip() and not l.startswith("#")):
            if row[0] not in known or row[1] not in VERDICTS:
                raise SystemExit(f"{VEREDICTOS.name}: fila no válida {row}")
            found[row[0]] = row[1]
    return found


def pct(done, total):
    return f"{100 * done / total:.1f} %".replace(".", ",") if total else PENDING


def table(header, rows):
    return "\n".join(["| " + " | ".join(header) + " |", "|" + "---|" * len(header)]
                     + ["| " + " | ".join(map(str, r)) + " |" for r in rows])


def tally(selected, found):
    """Conteo P/PD/F/B/NA de los casos elegidos; PENDIENTE-CIFRA si alguno aún no tiene veredicto."""
    if any(c["id"] not in found for c in selected):
        return [PENDING] * len(VERDICTS), None
    n = Counter(found[c["id"]] for c in selected)
    ok, bad = n["P"] + n["PD"], n["F"]
    return [n[v] for v in VERDICTS], pct(ok, ok + bad)


def tables():
    """Los marcadores {{...}} de src/ y la matriz del Anexo A."""
    sessions, cases = parse()
    found = verdicts(cases)
    by_session = {s: [c for c in cases if c["session"] == s] for s in sessions}
    out = {"{{N_CASOS}}": str(len(cases)), "{{N_SESIONES}}": str(len(sessions))}
    out["{{TABLA_SESIONES}}"] = table(
        ["Sesión", "Qué recorre", "Ejecutor", "Casos"],
        [(s, t, e, len(by_session[s])) for s, (t, e) in sessions.items()] + [("Total", "", "", len(cases))])
    execs = Counter(c["executor"] for c in cases)
    out["{{TABLA_EJECUTORES}}"] = table(["Ejecutor (como lo fija el caso)", "Casos"],
                                        sorted(execs.items(), key=lambda kv: (-kv[1], kv[0])))
    rows = []
    for s, (t, _) in sessions.items():
        counts, eff = tally(by_session[s], found)
        rows.append((s, len(by_session[s]), *counts, eff or PENDING))
    counts, eff = tally(cases, found)
    rows.append(("Total", len(cases), *counts, eff or PENDING))
    out["{{TABLA_RESULTADOS_SESION}}"] = table(["Sesión", "Casos", *VERDICTS, "Eficacia"], rows)
    rows = []
    for label in LABELS:
        sel = [c for c in cases if c["label"] == label]
        counts, eff = tally(sel, found)
        rows.append((label, len(sel), *counts, eff or PENDING))
    out["{{TABLA_RESULTADOS_ETIQUETA}}"] = table(["Etiqueta", "Casos", *VERDICTS, "Eficacia"], rows)
    names = {}
    for line in OE1.read_text(encoding="utf-8").splitlines():
        m = re.match(r"^\|\s*(RF-\d\d)\s*\|\s*([^|]+?)\s*\|", line)
        if m:
            names.setdefault(m.group(1), " ".join(m.group(2).split()))
    order = lambda c: (0 if primary(c["id"]).startswith("RF") else 1 if primary(c["id"]).startswith("RNF") else 2,
                       c["id"])
    out["{{MATRIZ}}"] = table(
        ["Requerimiento", "Caso", "Etiqueta", "Sesión", "Ejecutor", "Veredicto rc3"],
        [(f"{primary(c['id'])} {names.get(primary(c['id']), '')}".strip(), c["id"], c["label"], c["session"],
          c["executor"], found.get(c["id"], PENDING)) for c in sorted(cases, key=order)])
    return out, cases


def coverage(cases):
    """CT-10: todo RF-01..47 y RNF-01..23 tiene al menos un caso. Devuelve los que faltan."""
    have = {primary(c["id"]) for c in cases}
    want = [f"RF-{i:02d}" for i in range(1, 48)] + [f"RNF-{i:02d}" for i in range(1, 24)]
    return [r for r in want if r not in have]


def declared():
    """El tamaño que casos.md declara de su propio catálogo («Tamaño del catálogo: N casos»)."""
    m = re.search(r"Tamaño del catálogo: (\d+) casos", CASOS.read_text(encoding="utf-8"))
    return int(m.group(1)) if m else None
