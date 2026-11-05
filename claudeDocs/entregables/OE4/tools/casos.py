"""Lee el catálogo de casos PF-* (claudeDocs/tasks/OE4/casos.md) y los veredictos de la pasada final
(src/veredictos-rc5.csv, que escribe tools/veredictos_rc5.py) y escribe las tablas que el documento no debe
contar a mano. La primera pasada (src/veredictos-rc3.csv) entra solo como columna de referencia de la matriz.

Veredictos: P, PD, F, B, NA, y los dos de trabajo NE (no ejecutado, de Claude) y PS (pendiente de una persona, HUM); NE y PS no entran en la eficacia. Sin veredicto, o sin archivo, cada celda queda en PENDIENTE-CIFRA y la
guarda de build.py impide publicar. Los CSV empiezan por «caso,veredicto» (líneas con # se ignoran).
"""
import csv
import re
from collections import Counter, OrderedDict
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[3]
CASOS = ROOT / "claudeDocs" / "tasks" / "OE4" / "casos.md"
OE1 = ROOT / "docs" / "md" / "Solución OE1_Requerimientos.md"
VEREDICTOS = HERE.parent / "src" / "veredictos-rc5.csv"
VEREDICTOS_RC3 = HERE.parent / "src" / "veredictos-rc3.csv"
MANIFIESTO = CASOS.parent / "evidencias" / "rc5" / "manifiesto-rc5.tsv"
PENDING = "PENDIENTE-CIFRA"
VERDICTS = ("P", "PD", "F", "B", "NA", "NE", "PS")  # NE no ejecutado (de Claude) · PS pendiente de Santiago (HUM)
SHOWN = ("P", "PD", "F", "B", "NA", "NE")  # columnas de las tablas: PS no aparece en rc5 (INC-163)
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


def rows_of(path):
    """Las filas de un CSV de veredictos, sin comentarios ni cabecera."""
    if not path.exists():
        return []
    return [row for row in csv.reader(l for l in path.read_text(encoding="utf-8").splitlines()
                                      if l.strip() and not l.startswith("#")) if row[0] != "caso"]


def verdicts(cases):
    known = {c["id"] for c in cases}
    found = {}
    for row in rows_of(VEREDICTOS):
        if row[0] not in known or row[1] not in VERDICTS:
            raise SystemExit(f"{VEREDICTOS.name}: fila no válida {row}")
        found[row[0]] = row[1]
    return found


def cell(text):
    """Texto de casos.md o de una sesión, listo para una celda: sin marcas de Markdown ni «§» (la guarda de
    build.py lo prohíbe) y con el nombre propio de la persona que ejecuta sustituido, por la voz impersonal."""
    text = text.replace("**", "").replace("`", "").replace("§", "apartado ")
    text = re.sub(r"\bSantiago\b", "una persona del equipo", text)
    text = re.sub(r"([\\*_\[\]<>|~^$#])", r"\\\1", text).replace("--", r"-\-")  # «--» sería una raya
    # Espacio de ancho cero tras cada «/»: sin él una ruta es una palabra sin corte y Word estrecha las demás columnas.
    return text.replace(r"\[Pendiente:", "[Pendiente:").replace("/", "/\u200b")


def fingerprints():
    """Identidad de rc5 y la lista de huellas de sus archivos, desde el manifiesto."""
    lines = MANIFIESTO.read_text(encoding="utf-8").splitlines()
    head = dict(l[2:].split("\t")[:2] for l in lines if l.startswith("# ") and "\t" in l)
    files = [l.split("\t") for l in lines if l and not l.startswith("#")][1:]
    return head, files


def pct(done, total):
    return f"{100 * done / total:.1f} %".replace(".", ",") if total else "sin calcular"  # todos los casos en PS/NE: no hay denominador


def table(header, rows):
    return "\n".join(["| " + " | ".join(header) + " |", "|" + "---|" * len(header)]
                     + ["| " + " | ".join(map(str, r)) + " |" for r in rows])


def tally(selected, found):
    """Conteo P/PD/F/B/NA de los casos elegidos; PENDIENTE-CIFRA si alguno aún no tiene veredicto."""
    if any(c["id"] not in found for c in selected):
        return [PENDING] * len(SHOWN), None
    n = Counter(found[c["id"]] for c in selected)
    ok, bad = n["P"] + n["PD"], n["F"]
    return [n[v] for v in SHOWN], pct(ok, ok + bad)


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
    out["{{TABLA_RESULTADOS_SESION}}"] = table(["Sesión", "Casos", *SHOWN, "Eficacia"], rows)
    rows = []
    for label in LABELS:
        sel = [c for c in cases if c["label"] == label]
        counts, eff = tally(sel, found)
        rows.append((label, len(sel), *counts, eff or PENDING))
    out["{{TABLA_RESULTADOS_ETIQUETA}}"] = table(["Etiqueta", "Casos", *SHOWN, "Eficacia"], rows)
    names = {}
    for line in OE1.read_text(encoding="utf-8").splitlines():
        m = re.match(r"^\|\s*(RF-\d\d)\s*\|\s*([^|]+?)\s*\|", line)
        if m:
            names.setdefault(m.group(1), " ".join(m.group(2).split()))
    order = lambda c: (0 if primary(c["id"]).startswith("RF") else 1 if primary(c["id"]).startswith("RNF") else 2,
                       c["id"])
    first = {row[0]: row[1] for row in rows_of(VEREDICTOS_RC3)}
    detail = {row[0]: row for row in rows_of(VEREDICTOS)}
    blank = [PENDING] * 11
    def when(row):
        """Fecha y candidato en una sola celda: en columnas separadas Word parte los identificadores letra a letra."""
        days = "; ".join("/".join(reversed(d.split("-"))) for d in row[2].split("; ")) if row[2] else "Sin ejecutar"
        return f"{days} · {row[3]}"

    out["{{MATRIZ}}"] = table(
        ["Caso", "Requerimiento · etiqueta · sesión · ejecutor", "rc3", "rc5", "Fecha y candidato", "Precondición",
         "Resultado esperado", "Resultado observado", "Registro"],
        [(c["id"].replace("-", "\u2011"), f"{primary(c['id'])} {names.get(primary(c['id']), '')} · {c['label']} · {c['session']} · {c['executor']}",
          first.get(c["id"], "Sin caso"), found.get(c["id"], PENDING), when(detail.get(c["id"], blank)),
          *(cell(detail.get(c["id"], blank)[i]) for i in (4, 5, 6, 7))) for c in sorted(cases, key=order)])
    head, files = fingerprints()
    commits = {row[9] for row in detail.values()}
    out["{{COMMIT}}"] = commits.pop() if len(commits) == 1 else PENDING
    out["{{HUELLA_GLOBAL}}"] = head["huella_global"]
    out["{{HUELLA_DATA}}"] = head["huella_algoritmia_data"]
    out["{{SHA_ZIP}}"] = head["sha256_zip"]
    out["{{N_ARCHIVOS}}"] = head["archivos"]
    out["{{N_ARCHIVOS_HUELLA}}"] = head["archivos_en_huella"]
    out["{{HUELLAS}}"] = table(["Archivo", "Bytes", "SHA-256"],
                               [(cell(ruta) + ("" if inside == "si" else " (fuera de la huella)"), size, sha)
                                for ruta, size, sha, inside in files])
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
