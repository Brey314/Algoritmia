#!/usr/bin/env python3
"""Verifica el .docx que deja insertar_cap5.ps1 (o cualquier otro) contra cap5-fases.md.

Solo lee: no abre Word ni escribe nada. Necesita python-docx y lxml (pip install python-docx).

    python claudeDocs/entregables/TG/tools/verificar_cap5.py --docx RESULTADO.docx
    python claudeDocs/entregables/TG/tools/verificar_cap5.py --docx RESULTADO.docx --original ANTES.docx --sin-indice

Qué comprueba (cada línea sale como [OK], [AVISO] o [FALLA]; el código de salida es 1 si hay alguna
[FALLA], 0 si no, 2 si el uso es incorrecto):
  1. El apartado está antes de «Instrumentos o herramientas utilizadas» y los títulos del capítulo 5
     quedan 5.1 Fases del trabajo de grado, 5.1.1 a 5.1.6, 5.2 Instrumentos..., 5.3 Población y muestra.
  2. Los párrafos insertados son idénticos, uno a uno, al .md, y tienen el estilo y el formato
     efectivos de sus modelos: títulos de nivel 2 como los del capítulo 5, de nivel 3 como los del
     capítulo 6 y prosa como el cuerpo del 5.3 (Normal, 12 pt antes y después). Sin numeración
     automática, sin negrita ni cursiva sueltas, sin campos ni hipervínculos.
  3. La referencia [53] está en la bibliografía una sola vez, justo después de la [52], con la
     sangría de las demás y el texto del .md; la numeración 1..53 no tiene huecos; cada cita [n]
     del texto insertado existe en la bibliografía, la primera [53] va después de la primera [52]
     (orden de aparición, IEEE) y todas apuntan a ella.
  4. Nada más cambió: el texto y el estilo de todos los demás párrafos son los del original salvo
     los dos números de título que se corren (y las líneas de las listas de tablas y figuras, donde
     solo cambian los números de página); mismas tablas, imágenes, secciones y pies de página; sin
     revisiones (w:ins / w:del).
  5. Cero rayas (U+2014) y cero comillas rectas o inglesas en lo insertado.
  6. La tabla de contenido trae los títulos nuevos y los dos renumerados (se omite con --sin-indice).

El original se toma de «git show HEAD:docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx» salvo que
se dé --original: así funciona aunque el .ps1 haya modificado el archivo del repositorio en el sitio.
"""
from __future__ import annotations

import argparse
import difflib
import re
import subprocess
import sys
import tempfile
from pathlib import Path

try:
    import docx
    from docx.oxml.ns import qn
except ImportError:  # pragma: no cover
    print("Falta python-docx: pip install python-docx", file=sys.stderr)
    sys.exit(2)

REPO = Path(__file__).resolve().parents[4]
DOCX_REL = "docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx"
MD_DEFAULT = REPO / "claudeDocs/entregables/TG/cap5-fases.md"

FORBIDDEN = {
    "—": "raya (U+2014)",
    '"': "comilla recta doble",
    "'": "comilla recta simple",
    "“": "comilla inglesa de apertura",
    "”": "comilla inglesa de cierre",
    "‘": "comilla simple inglesa de apertura",
    "’": "comilla simple inglesa de cierre",
}
SOFT = {"–": "raya corta (U+2013)"}


class Report:
    def __init__(self) -> None:
        self.fails = 0
        self.warns = 0

    def ok(self, msg: str) -> None:
        print(f"[OK]     {msg}")

    def fail(self, msg: str) -> None:
        self.fails += 1
        print(f"[FALLA]  {msg}")

    def warn(self, msg: str) -> None:
        self.warns += 1
        print(f"[AVISO]  {msg}")

    def check(self, cond: bool, ok_msg: str, fail_msg: str) -> bool:
        if cond:
            self.ok(ok_msg)
        else:
            self.fail(fail_msg)
        return cond


# --- .md ------------------------------------------------------------------------------------

def parse_md(path: Path) -> dict:
    raw = path.read_text(encoding="utf-8-sig")
    anchor = None
    renumber: list[tuple[str, str]] = []
    items: list[tuple[str, str]] = []
    ref_number = None
    ref_text = None
    after = False
    want_ref = False
    for line in raw.splitlines():
        t = line.strip()
        if not t:
            continue
        m = re.match(r"^<!--\s*inserta-antes:\s*(.+?)\s*-->$", t)
        if m:
            anchor = m.group(1)
            continue
        m = re.match(r"^<!--\s*renumerar:\s*(.+?)\s*-->$", t)
        if m:
            after = True
            for part in m.group(1).split(";"):
                mm = re.match(r"^\s*(.+?)\s*->\s*(\d+(?:\.\d+)*)\s*$", part)
                if not mm:
                    raise ValueError(f"directiva renumerar ilegible: {part!r}")
                renumber.append((mm.group(1), mm.group(2)))
            continue
        m = re.match(r"^<!--\s*referencia-(\d+)\s*-->$", t)
        if m:
            after = True
            ref_number = int(m.group(1))
            want_ref = True
            continue
        if t.startswith("<!--"):
            continue
        if want_ref:
            if not re.match(rf"^\[{ref_number}\]\s+\S", t):
                raise ValueError(f"tras referencia-{ref_number} se esperaba una línea [{ref_number}]")
            ref_text = t
            want_ref = False
            continue
        if after:
            raise ValueError(f"texto inesperado tras las directivas: {t[:50]!r}")
        m = re.match(r"^###\s+(.+)$", t)
        if m:
            items.append(("h3", m.group(1).strip()))
            continue
        m = re.match(r"^##\s+(.+)$", t)
        if m:
            items.append(("h2", m.group(1).strip()))
            continue
        items.append(("p", t))
    if not anchor or not items:
        raise ValueError("el .md no trae ancla o contenido")
    return {"anchor": anchor, "renumber": renumber, "items": items, "ref_number": ref_number, "ref_text": ref_text}


# --- .docx ----------------------------------------------------------------------------------

def norm(text: str) -> str:
    return re.sub(r"\s+", " ", text.replace(" ", " ")).strip()


def heading_level(p) -> int | None:
    m = re.match(r"^Heading (\d)$", p.style.name or "")
    return int(m.group(1)) if m else None


def style_chain(p):
    s = p.style
    while s is not None:
        yield s
        s = s.base_style


def _doc_defaults_ppr(doc):
    el = doc.styles.element.find(qn("w:docDefaults"))
    if el is None:
        return None
    pd = el.find(qn("w:pPrDefault"))
    return None if pd is None else pd.find(qn("w:pPr"))


def eff_ppr(p, doc) -> dict:
    """Formato de párrafo efectivo en puntos: directo, luego estilo y estilos base, luego docDefaults."""
    levels = [p._p.pPr] + [s.element.pPr for s in style_chain(p)] + [_doc_defaults_ppr(doc)]
    levels = [e for e in levels if e is not None]

    def attr(child: str, name: str):
        for e in levels:
            c = e.find(qn(child))
            if c is not None and c.get(qn(name)) is not None:
                return c.get(qn(name))
        return None

    def flag(child: str) -> bool:
        for e in levels:
            c = e.find(qn(child))
            if c is not None:
                return c.get(qn("w:val")) not in ("0", "false")
        return False

    first = 0.0
    for e in levels:  # el primer nivel que dice firstLine o hanging manda
        c = e.find(qn("w:ind"))
        if c is not None and (c.get(qn("w:firstLine")) is not None or c.get(qn("w:hanging")) is not None):
            first = int(c.get(qn("w:firstLine"))) / 20 if c.get(qn("w:firstLine")) is not None else -int(c.get(qn("w:hanging"))) / 20
            break
    left = attr("w:ind", "w:left") or attr("w:ind", "w:start")
    return {
        "before": int(attr("w:spacing", "w:before") or 0) / 20,
        "after": int(attr("w:spacing", "w:after") or 0) / 20,
        "left": int(left or 0) / 20,
        "first": first,
        "jc": attr("w:jc", "w:val") or "left",
        "keepNext": flag("w:keepNext"),
        "numbered": any(e.find(qn("w:numPr")) is not None for e in levels[:-1]),
    }


def ppr_close(a: dict, b: dict, keys=("before", "after", "left", "first", "jc")) -> list[str]:
    bad = []
    for k in keys:
        if isinstance(a[k], float):
            if abs(a[k] - b[k]) > 0.5:
                bad.append(f"{k} {a[k]:g} contra {b[k]:g}")
        elif a[k] != b[k]:
            bad.append(f"{k} {a[k]} contra {b[k]}")
    return bad


def body(doc):
    out = []
    for i, p in enumerate(doc.paragraphs):
        out.append({"i": i, "p": p, "text": p.text, "sid": p.style.style_id, "sname": p.style.name, "lvl": heading_level(p)})
    return out


def toc_entries(doc) -> list[tuple[str, str]]:
    """Líneas de la tabla de contenido (los párrafos del control de contenido): (estilo, texto del
    título, sin el número de página, que va tras el tabulador)."""
    out = []
    for sdt in doc.element.body.iter(qn("w:sdt")):
        for p in sdt.iter(qn("w:p")):
            st = p.find(qn("w:pPr") + "/" + qn("w:pStyle"))
            sid = st.get(qn("w:val")) if st is not None else ""
            if not re.match(r"^(TDC|TOC)\d", sid or ""):
                continue
            parts = []
            stop = False
            for run in p.iter(qn("w:r")):  # solo las ejecuciones: «w:tabs» de pPr no cuenta
                for node in run:
                    if node.tag == qn("w:tab"):
                        stop = True
                        break
                    if node.tag == qn("w:t") and node.text:
                        parts.append(node.text)
                if stop:
                    break
            text = norm("".join(parts))
            if text:
                out.append((sid, text))
    return out


CITE = re.compile(r"\[(\d+(?:\s*[,\-\u2013]\s*\d+)*)(?:,\s*pp?\.\s*[\d\-\u2013, ]+)?\]")


def cites(text: str) -> set[int]:
    out: set[int] = set()
    for m in CITE.finditer(text):
        for part in re.split(r"\s*,\s*", m.group(1)):
            rng = re.split(r"\s*[\-\u2013]\s*", part)
            out.update(range(int(rng[0]), int(rng[-1]) + 1))
    return out


def strip_page(text: str) -> str:
    return norm(re.sub(r"\t\s*\d+\s*$", "", text))


def load_original(arg: str | None) -> Path:
    if arg and arg != "git":
        return Path(arg)
    res = subprocess.run(["git", "-C", str(REPO), "show", f"HEAD:{DOCX_REL}"], capture_output=True)
    if res.returncode != 0:
        raise RuntimeError("no se pudo leer el original con git show; dé --original RUTA")
    tmp = tempfile.NamedTemporaryFile(suffix=".docx", delete=False)
    tmp.write(res.stdout)
    tmp.close()
    return Path(tmp.name)


# --- comprobaciones -------------------------------------------------------------------------

def main(argv=None) -> int:
    for stream in (sys.stdout, sys.stderr):  # los títulos traen «↔»: que no falle con la página de códigos de Windows
        try:
            stream.reconfigure(encoding="utf-8", errors="replace")
        except (AttributeError, ValueError):
            pass
    ap = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    ap.add_argument("--docx", required=True, help="el .docx resultante")
    ap.add_argument("--md", default=str(MD_DEFAULT), help="cap5-fases.md")
    ap.add_argument("--original", default="git", help="el .docx de antes, o «git» (HEAD)")
    ap.add_argument("--sin-indice", action="store_true", help="no comprobar la tabla de contenido")
    args = ap.parse_args(argv)

    for f in (args.docx, args.md):
        if not Path(f).exists():
            print(f"No existe: {f}", file=sys.stderr)
            return 2
    try:
        plan = parse_md(Path(args.md))
        orig_path = load_original(args.original)
    except (ValueError, RuntimeError) as e:
        print(f"Uso: {e}", file=sys.stderr)
        return 2

    R = Report()
    res = docx.Document(args.docx)
    org = docx.Document(str(orig_path))
    if args.original == "git":
        orig_path.unlink()  # copia temporal del HEAD
    mine: list = []
    rb, ob = body(res), body(org)
    items = plan["items"]
    n = len(items)
    ref_no = plan["ref_number"]
    anchor_text = plan["anchor"]

    # Textos esperados tras renumerar el ancla y el siguiente.
    def renumbered(text: str) -> str:
        for old_prefix, new_num in plan["renumber"]:
            if text.startswith(old_prefix):
                return new_num + text[len(old_prefix.split(" ")[0]):]
        return text

    anchor_new = renumbered(anchor_text)
    chap_no = int(anchor_text.split(".")[0])  # el capítulo del ancla («5.1 ...» -> 5)

    # --- 1. orden y títulos ---------------------------------------------------------------
    print("== 1. Orden y títulos del capítulo 5")
    first_title = items[0][1]
    h2_idx = [r["i"] for r in rb if r["lvl"] == 2 and r["text"].strip() == first_title]
    anchor_idx = [r["i"] for r in rb if r["lvl"] == 2 and r["text"].strip() == anchor_new]
    if not R.check(len(h2_idx) == 1, f"un solo título «{first_title}» (nivel 2)", f"se esperaba un título «{first_title}» y hay {len(h2_idx)}"):
        return _end(R)
    if not R.check(len(anchor_idx) == 1, f"un solo título «{anchor_new}»", f"se esperaba un título «{anchor_new}» y hay {len(anchor_idx)}"):
        return _end(R)
    h = h2_idx[0]
    R.check(h < anchor_idx[0], f"«{first_title}» va antes de «{anchor_new}»", f"«{first_title}» NO va antes de «{anchor_new}»")
    chap = [r for r in rb if r["lvl"] in (1, 2, 3)]
    start5 = next((k for k, r in enumerate(chap) if re.match(rf"^{chap_no}\.\s", r["text"]) and r["lvl"] == 1), None)
    end5 = next((k for k, r in enumerate(chap) if re.match(rf"^{chap_no + 1}\.\s", r["text"]) and r["lvl"] == 1), None)
    if start5 is None or end5 is None:
        R.fail("no se hallaron los títulos de capítulo «5.» y «6.»")
        return _end(R)
    got = [(r["lvl"], norm(r["text"])) for r in chap[start5 + 1:end5]]
    # Lo esperado: los títulos nuevos del .md y, tras ellos, los que ya tenía el capítulo 5 en el
    # original, con los dos números corridos.
    ochap = [r for r in ob if r["lvl"] in (1, 2, 3)]
    os5 = next((k for k, r in enumerate(ochap) if re.match(rf"^{chap_no}\.\s", r["text"]) and r["lvl"] == 1), None)
    oe5 = next((k for k, r in enumerate(ochap) if re.match(rf"^{chap_no + 1}\.\s", r["text"]) and r["lvl"] == 1), None)
    if os5 is None or oe5 is None:
        R.fail("el original no trae los títulos de capítulo «5.» y «6.»")
        return _end(R)
    orig5 = [(r["lvl"], norm(r["text"])) for r in ochap[os5 + 1:oe5]]
    want = [(2 if k == "h2" else 3, norm(t)) for k, t in items if k in ("h2", "h3")] + [(l, renumbered(t)) for l, t in orig5]
    after_anchor = [t for l, t in orig5 if l == 2]
    pob_title = renumbered(after_anchor[after_anchor.index(norm(anchor_text)) + 1]) if norm(anchor_text) in after_anchor and after_anchor.index(norm(anchor_text)) + 1 < len(after_anchor) else None
    pob = next((r for r in rb if r["lvl"] == 2 and norm(r["text"]) == pob_title), None)
    R.check(got == want, "títulos del capítulo 5: " + " | ".join(t for _, t in got), "títulos del capítulo 5 distintos de lo esperado:\n      tiene:    " + " | ".join(t for _, t in got) + "\n      esperado: " + " | ".join(t for _, t in want))
    nums2 = [re.match(r"^(\d+)\.(\d+)\s", t) for lvl, t in got if lvl == 2]  # numeración escrita a mano en el texto del título
    seq2 = [int(m.group(2)) if m else -1 for m in nums2]
    R.check(seq2 == list(range(1, len(seq2) + 1)), f"los títulos de nivel 2 del capítulo 5 numeran 5.1..5.{len(seq2)} sin saltos", f"numeración de nivel 2 no consecutiva: {seq2}")
    sub = [re.match(rf"^{chap_no}\.1\.(\d+)\s", t) for lvl, t in got if lvl == 3]
    seq3 = [int(m.group(1)) if m else -1 for m in sub]
    R.check(seq3 == list(range(1, len(seq3) + 1)), f"los títulos de nivel 3 numeran 5.1.1..5.1.{len(seq3)} sin saltos", f"numeración de nivel 3 no consecutiva: {seq3}")

    # --- 2. el bloque insertado -----------------------------------------------------------
    print("== 2. Texto, estilo y formato de lo insertado")
    block = rb[h:h + n]
    texts_ok = [b["text"] for b in block] == [t for _, t in items]
    if not R.check(texts_ok, f"los {n} párrafos insertados son idénticos al .md", "el texto insertado difiere del .md"):
        for k, (b, (_, t)) in enumerate(zip(block, items)):
            if b["text"] != t:
                print(f"      primer desvío en el elemento {k + 1}: {b['text'][:70]!r} contra {t[:70]!r}")
                break
    before = rb[h - 1] if h >= 1 else None
    after = rb[h + n] if h + n < len(rb) else None
    R.check(before is not None and before["text"].strip() == "", "hay un párrafo vacío antes del título nuevo", "falta el separador vacío antes del título nuevo")
    R.check(after is not None and after["text"].strip() == "" and rb[h + n + 1]["i"] == anchor_idx[0], "hay un párrafo vacío entre el apartado y «" + anchor_new + "»", "falta el separador vacío antes de «" + anchor_new + "» o hay algo entre medias")

    # Modelos
    ch6 = next((r for r in rb if r["lvl"] == 3 and r["text"].startswith("6.1.1")), None)
    body_model = None
    if pob is not None:
        for r in rb[pob["i"] + 1:]:
            if r["lvl"] is None and len(r["text"]) > 40 and r["sname"] == "Normal":
                body_model = r
                break
    models = {"h2": rb[anchor_idx[0]], "h3": ch6, "p": body_model}
    if not all(models.values()):
        R.fail("faltan modelos de formato (título 2 del 5, título 3 del 6 o cuerpo del 5.3)")
        return _end(R)
    eff_model = {k: eff_ppr(m["p"], res) for k, m in models.items()}
    want_style = {"h2": models["h2"]["sid"], "h3": models["h3"]["sid"], "p": models["p"]["sid"]}
    bad_style = []
    bad_fmt = []
    bad_run = []
    bad_misc = []
    for b, (kind, _) in zip(block, items):
        if b["sid"] != want_style[kind]:
            bad_style.append(f"{kind} «{b['text'][:30]}» tiene estilo {b['sid']} y se esperaba {want_style[kind]}")
        diff = ppr_close(eff_ppr(b["p"], res), eff_model[kind])
        if diff:
            bad_fmt.append(f"{kind} «{b['text'][:30]}»: {'; '.join(diff)}")
        e = eff_ppr(b["p"], res)
        if e["numbered"]:
            bad_misc.append(f"{kind} «{b['text'][:30]}» lleva numeración automática")
        pel = b["p"]._p
        if pel.findall(".//" + qn("w:fldChar")) or pel.findall(".//" + qn("w:fldSimple")) or pel.findall(".//" + qn("w:hyperlink")) or pel.findall(".//" + qn("w:drawing")):
            bad_misc.append(f"{kind} «{b['text'][:30]}» trae campos, hipervínculos o imágenes")
        for run in b["p"].runs:
            f = run.font
            if kind == "p" and (f.bold or f.italic or f.underline or f.strike or f.highlight_color is not None):
                bad_run.append(f"«{b['text'][:30]}»: negrita, cursiva, subrayado o resaltado suelto")
            if f.size is not None and f.size.pt != 12:
                bad_run.append(f"«{b['text'][:30]}»: tamaño {f.size.pt} pt")
            if f.name not in (None, "Arial"):
                bad_run.append(f"«{b['text'][:30]}»: fuente {f.name}")
            if f.color is not None and f.color.rgb is not None and str(f.color.rgb) != "000000":
                bad_run.append(f"«{b['text'][:30]}»: color {f.color.rgb}")
    R.check(not bad_style, "estilos: títulos de nivel 2 y 3 y prosa como sus modelos (" + ", ".join(f"{k}={v}" for k, v in want_style.items()) + ")", "estilos distintos de los modelos:\n      " + "\n      ".join(bad_style[:6]))
    R.check(not bad_fmt, "formato efectivo (espaciado, sangría, alineación) igual al de sus modelos", "formato distinto del modelo:\n      " + "\n      ".join(bad_fmt[:6]))
    R.check(not bad_run, "sin negrita, cursiva, tamaño, fuente ni color sueltos", "formato de carácter suelto:\n      " + "\n      ".join(bad_run[:6]))
    R.check(not bad_misc, "sin numeración automática, campos, hipervínculos ni imágenes en lo insertado", "\n      ".join(bad_misc[:6]))
    spacing = eff_model["p"]
    R.check(spacing["before"] == 12 and spacing["after"] == 12, "el cuerpo del capítulo 5 (modelo) lleva 12 pt antes y después, y lo insertado igual", f"el modelo de cuerpo no tiene 12/12 pt: {spacing['before']}/{spacing['after']}")

    # --- 3. caracteres --------------------------------------------------------------------
    print("== 3. Caracteres prohibidos")
    inserted_texts = [b["text"] for b in block]
    if ref_no:
        inserted_texts.append(plan["ref_text"])
    hits: dict[str, int] = {}
    softs: dict[str, int] = {}
    for t in inserted_texts:
        for ch, name in FORBIDDEN.items():
            if ch in t:
                hits[name] = hits.get(name, 0) + t.count(ch)
        for ch, name in SOFT.items():
            if ch in t:
                softs[name] = softs.get(name, 0) + t.count(ch)
    R.check(not hits, "cero rayas y cero comillas rectas o inglesas en lo insertado", "lo insertado trae: " + ", ".join(f"{k} x{v}" for k, v in hits.items()))
    if softs:
        R.warn("lo insertado trae " + ", ".join(f"{k} x{v}" for k, v in softs.items()))

    # --- 4. bibliografía y citas ----------------------------------------------------------
    print("== 4. Bibliografía y citas")
    if ref_no:
        bib_h = next((r for r in rb if r["lvl"] == 1 and re.match(r"^BIBLIOGRAF.A$", r["text"].strip())), None)
        if bib_h is None:
            R.fail("no se halló el título BIBLIOGRAFÍA")
            return _end(R)
        entries = []
        for r in rb[bib_h["i"] + 1:]:
            if r["lvl"] == 1 or re.match(r"^(OTRAS FUENTES|ANEXOS?\b)", r["text"].strip()):
                break
            m = re.match(r"^\[(\d+)\]\s", r["text"])
            if m:
                entries.append((int(m.group(1)), r))
        nums = [k for k, _ in entries]
        R.check(nums == list(range(1, ref_no + 1)), f"la bibliografía numera [1]..[{ref_no}] sin huecos ni repetidos", f"numeración de la bibliografía: {nums[:3]}...{nums[-3:]} ({len(nums)} entradas)")
        mine = [r for k, r in entries if k == ref_no]
        if R.check(len(mine) == 1, f"la entrada [{ref_no}] existe una sola vez", f"la entrada [{ref_no}] aparece {len(mine)} veces"):
            e = mine[0]
            R.check(e["text"].strip() == plan["ref_text"], f"el texto de [{ref_no}] es el del .md", f"el texto de [{ref_no}] difiere del .md: {e['text'][:80]!r}")
            prev = next((r for k, r in entries if k == ref_no - 1), None)
            R.check(prev is not None and e["i"] == prev["i"] + 1, f"[{ref_no}] va justo después de [{ref_no - 1}]", f"[{ref_no}] no va justo después de [{ref_no - 1}]")
            nxt = rb[e["i"] + 1] if e["i"] + 1 < len(rb) else None
            R.check(nxt is not None and nxt["text"].strip().startswith("OTRAS FUENTES"), "tras la última entrada sigue «OTRAS FUENTES CONSULTADAS»", "tras la última entrada no sigue «OTRAS FUENTES CONSULTADAS»")
            if prev is not None:
                R.check(e["sid"] == prev["sid"], f"[{ref_no}] tiene el estilo de [{ref_no - 1}] ({e['sid']})", f"estilo de [{ref_no}] distinto del de [{ref_no - 1}]")
                d = ppr_close(eff_ppr(e["p"], res), eff_ppr(prev["p"], res), keys=("before", "after", "left", "first", "jc"))
                R.check(not d, f"[{ref_no}] tiene la sangría y el espaciado de las demás entradas", f"[{ref_no}] difiere de [{ref_no - 1}]: {'; '.join(d)}")
                fe, fp = e["p"].runs, prev["p"].runs
                bad = [r for r in fe if r.font.bold or r.font.italic or r.font.underline or (r.font.color is not None and r.font.color.rgb is not None and str(r.font.color.rgb) != "000000")]
                R.check(not bad, f"[{ref_no}] sin negrita, cursiva, subrayado ni color (como las demás)", f"[{ref_no}] trae formato de carácter suelto")
        cited: dict[int, int] = {}
        for b in block:
            for k in cites(b["text"]):
                cited[k] = cited.get(k, 0) + 1
        R.check(bool(cited), "el texto insertado trae citas: " + ", ".join(f"[{k}] x{v}" for k, v in sorted(cited.items())), "el texto insertado no trae ninguna cita [n]")
        R.check(all(k in nums for k in cited), "toda cita del texto insertado existe en la bibliografía", "hay citas sin entrada en la bibliografía: " + ", ".join(f"[{k}]" for k in cited if k not in nums))
        others = [k for k in cited if k != ref_no]
        if others:
            R.warn("el texto insertado cita además " + ", ".join(f"[{k}]" for k in others) + f" (se esperaba solo [{ref_no}])")
        R.check(ref_no in cited, f"[{ref_no}] está citada en el texto insertado", f"[{ref_no}] no está citada en el texto insertado")

        def first_cite(num: int):
            for r in rb:
                if r["i"] >= bib_h["i"]:
                    break
                if num in cites(r["text"]):
                    return r["i"]
            return None

        fc, f52 = first_cite(ref_no), first_cite(ref_no - 1)
        R.check(fc is not None and h <= fc < h + n, f"la primera cita de [{ref_no}] del documento cae dentro del apartado nuevo", f"la primera cita de [{ref_no}] está en el párrafo {fc}, fuera del bloque insertado")
        R.check(fc is not None and f52 is not None and f52 < fc, f"orden IEEE: la primera [{ref_no - 1}] (párrafo {f52}) va antes de la primera [{ref_no}] (párrafo {fc})", f"orden IEEE roto: primera [{ref_no - 1}] en {f52}, primera [{ref_no}] en {fc}")

    # --- 5. nada más cambió ---------------------------------------------------------------
    print("== 5. El resto del documento no cambió")
    drop = set(range(h, h + n + 1))  # el apartado y un separador vacío (el otro es el que ya tenía el documento)
    if ref_no and mine:
        drop.add(mine[0]["i"])
    rest = [r for r in rb if r["i"] not in drop]
    prior = [dict(r, text=renumbered(r["text"]) if r["lvl"] == 2 else r["text"]) for r in ob]

    def key(r):
        t = r["text"]
        if r["sid"] in ("Tabladeilustraciones",) or r["sname"].lower() == "table of figures":
            t = strip_page(t)
        return norm(t)

    ka, kb = [key(r) for r in prior], [key(r) for r in rest]
    if ka == kb:
        R.ok(f"los {len(ka)} párrafos restantes tienen el texto del original (salvo los títulos renumerados)")
    else:
        sm = difflib.SequenceMatcher(a=ka, b=kb, autojunk=False)
        diffs = [op for op in sm.get_opcodes() if op[0] != "equal"]
        lines = []
        for tag, i1, i2, j1, j2 in diffs[:5]:
            lines.append(f"{tag}: original[{i1}:{i2}] {ka[i1:i2][:1]} -> resultado[{j1}:{j2}] {kb[j1:j2][:1]}")
        R.fail(f"{len(diffs)} diferencias de texto respecto del original:\n      " + "\n      ".join(l[:230] for l in lines))
    if len(ka) == len(kb):
        bad_sid = [(i, a["sid"], b["sid"]) for i, (a, b) in enumerate(zip(prior, rest)) if a["sid"] != b["sid"]]
        R.check(not bad_sid, "el estilo de cada párrafo restante es el del original", f"{len(bad_sid)} párrafos cambiaron de estilo, p. ej. {bad_sid[:3]}")
        drift = []
        for i, (a, b) in enumerate(zip(prior, rest)):
            if a["sname"].lower() == "table of figures":
                continue
            d = ppr_close(eff_ppr(a["p"], org), eff_ppr(b["p"], res), keys=("before", "after", "left", "first", "jc"))
            if d:
                drift.append(f"{i} «{a['text'][:30]}»: {'; '.join(d)}")
        if drift:
            R.warn(f"{len(drift)} párrafos con espaciado, sangría o alineación efectivos distintos del original:\n      " + "\n      ".join(drift[:5]))
        else:
            R.ok("espaciado, sangría y alineación efectivos de los párrafos restantes iguales a los del original")
    tabs_o = [[c.text for row in t.rows for c in row.cells] for t in org.tables]
    tabs_r = [[c.text for row in t.rows for c in row.cells] for t in res.tables]
    R.check(tabs_o == tabs_r, f"las {len(tabs_o)} tablas conservan su contenido", "las tablas cambiaron")
    drw = lambda d: len(d.element.body.findall(".//" + qn("w:drawing"))) + len(d.element.body.findall(".//" + qn("w:pict")))
    R.check(drw(org) == drw(res) and len(org.inline_shapes) == len(res.inline_shapes), f"mismas imágenes ({drw(org)})", "cambió el número de imágenes")
    R.check(len(org.sections) == len(res.sections), f"mismas secciones ({len(org.sections)})", "cambió el número de secciones")
    ft = lambda d: [norm(p.text) for s in d.sections for p in s.footer.paragraphs] + [norm(p.text) for s in d.sections for p in s.header.paragraphs]
    R.check(ft(org) == ft(res), "encabezados y pies de página iguales", "cambiaron los encabezados o pies de página")
    revs = len(res.element.body.findall(".//" + qn("w:ins"))) + len(res.element.body.findall(".//" + qn("w:del")))
    R.check(revs == 0, "sin revisiones (w:ins / w:del)", f"el documento trae {revs} revisiones")

    # --- 6. tabla de contenido -------------------------------------------------------------
    if not args.sin_indice:
        print("== 6. Tabla de contenido")
        toc_o, toc_r = toc_entries(org), toc_entries(res)
        want_toc = [(sid, renumbered(t)) for sid, t in toc_o]
        new_entries = [("TDC2" if k == "h2" else "TDC3", norm(t)) for k, t in items if k in ("h2", "h3")]
        k = next((j for j, (_, t) in enumerate(want_toc) if t == anchor_new), None)
        if k is None:
            R.fail("la tabla de contenido del original no trae la línea del ancla")
        else:
            want_toc = want_toc[:k] + new_entries + want_toc[k:]
            if toc_r == want_toc:
                R.ok(f"la tabla de contenido trae los {len(new_entries)} títulos nuevos y los dos renumerados ({len(toc_r)} líneas)")
            else:
                sm = difflib.SequenceMatcher(a=want_toc, b=toc_r, autojunk=False)
                d = [op for op in sm.get_opcodes() if op[0] != "equal"]
                R.fail("la tabla de contenido no está al día (en Word: clic en el índice y F9, o rehaga la actualización):\n      " + "\n      ".join(f"{t}: esperado {want_toc[i1:i2][:2]} / tiene {toc_r[j1:j2][:2]}" for t, i1, i2, j1, j2 in d[:4]))
    else:
        print("== 6. Tabla de contenido: omitida (--sin-indice)")

    return _end(R)


def _end(R: Report) -> int:
    print()
    if R.fails:
        print(f"RESULTADO: {R.fails} falla(s), {R.warns} aviso(s).")
        return 1
    print(f"RESULTADO: todo bien ({R.warns} aviso(s)).")
    return 0


if __name__ == "__main__":
    sys.exit(main())
