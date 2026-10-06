#!/usr/bin/env python3
"""Verifica el .docx que deja reemplazar_parrafos.ps1 contra el original y la lista de pares.

Solo lee: no abre Word ni escribe nada. Necesita python-docx y lxml (pip install python-docx).

    python claudeDocs/entregables/tools/verificar_reemplazos.py \\
        --docx RESULTADO.docx --original ANTES.docx --pares claudeDocs/entregables/tools/pares/acta_D01.json

Los pares son los mismos del .ps1: una lista de {"antes", "despues", "veces" (opcional), "nota" (opcional)}.

Qué comprueba (cada línea sale como [OK], [AVISO] o [FALLA]; el código de salida es 1 si hay alguna
[FALLA], 0 si no, 2 si el uso es incorrecto):
  1. Cada «antes» aparecía en el original exactamente «veces» veces (1 por omisión) como párrafo
     completo (texto normalizado, sin distinguir mayúsculas; cuerpo y celdas de tabla, también
     anidadas), y en el resultado esos párrafos dicen EXACTAMENTE «despues».
  2. Solo cambian esos párrafos: el resto es idéntico en texto y en estilo de párrafo (y se avisa si
     cambió su formato de caracter o su sangría y espaciado efectivos). El número de párrafos es el
     mismo.
  3. En los párrafos sustituidos el estilo y el formato de párrafo son los del original, y lo que
     «antes» y «despues» comparten por el principio y por el final conserva, carácter a carácter, su
     formato de carácter (por eso «Serie:» sigue en negrita cuando solo cambia lo que va detrás); el
     texto nuevo usa un formato que ya tenía el párrafo.
  4. Mismas tablas (filas, celdas, combinaciones), mismas imágenes, mismas secciones (tamaño de
     página, márgenes, encabezados y pies), mismas notas al pie y comentarios, ningún marcador perdido.
  5. Sin revisiones: cero w:ins, w:del, w:moveFrom, w:moveTo, w:rPrChange ni w:pPrChange.
  6. Las líneas de la tabla de contenido y de las listas de figuras se comparan sin el número de
     página; si una cambió y no la explica ningún par, es un [AVISO].

Alcance igual al del .ps1: cuerpo y celdas de tabla. Los párrafos de cuadros de texto no se buscan, pero
sí se comparan con el original (deben ser idénticos).
"""
from __future__ import annotations

import argparse
import difflib
import hashlib
import json
import re
import sys
from pathlib import Path

try:
    import docx
    from docx.oxml.ns import qn
    from docx.text.paragraph import Paragraph
except ImportError:  # pragma: no cover
    print("Falta python-docx: pip install python-docx", file=sys.stderr)
    sys.exit(2)

NBSP = " "


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


# --- texto -----------------------------------------------------------------------------------

def norm(text: str) -> str:
    """Igual que Normalize-Text del .ps1: sin marcas, espacios duros como espacios, espacios colapsados."""
    t = re.sub(r"[\r\x07\x0b\x0c]", " ", text).replace(NBSP, " ")
    return re.sub(r"\s+", " ", t).strip()


def same(a: str, b: str) -> bool:
    return a.lower() == b.lower()


def short(text: str, n: int = 60) -> str:
    t = text.replace("\n", " ")
    return t if len(t) <= n else t[:n] + "..."


# --- pares -----------------------------------------------------------------------------------

def read_pairs(path: Path) -> list[dict]:
    raw = json.loads(path.read_text(encoding="utf-8-sig"))
    if isinstance(raw, dict) and "pares" in raw:
        raw = raw["pares"]
    if not isinstance(raw, list) or not raw:
        raise ValueError("el .json debe ser una lista de pares no vacía")
    pairs = []
    for n, it in enumerate(raw, 1):
        if not isinstance(it, dict):
            raise ValueError(f"el par {n} no es un objeto")
        extra = set(it) - {"antes", "despues", "veces", "nota"}
        if extra:
            raise ValueError(f"el par {n} trae claves desconocidas: {sorted(extra)}")
        if not isinstance(it.get("antes"), str) or not isinstance(it.get("despues"), str):
            raise ValueError(f"el par {n} necesita 'antes' y 'despues' como texto")
        veces = it.get("veces", 1)
        if not isinstance(veces, int) or isinstance(veces, bool) or veces < 1:
            raise ValueError(f"el par {n}: 'veces' debe ser un entero de 1 o más")
        an, dn = norm(it["antes"]), norm(it["despues"])
        if not an or not it["despues"].strip():
            raise ValueError(f"el par {n} trae 'antes' o 'despues' vacío")
        if an == dn:
            raise ValueError(f"el par {n} no cambia nada una vez normalizado")
        pairs.append({
            "n": n, "antes": it["antes"], "despues": it["despues"], "an": an, "dn": dn,
            "veces": veces, "cs": same(an, dn),  # cambio solo de mayúsculas: se compara distinguiéndolas
        })
    for a in pairs:
        for b in pairs:
            if a is b:
                continue
            if same(a["an"], b["an"]):
                raise ValueError(f"los pares {a['n']} y {b['n']} tienen el mismo 'antes'")
            if same(a["dn"], b["an"]):
                raise ValueError(f"el 'despues' del par {a['n']} es el 'antes' del par {b['n']}")
    return pairs


def matches(pair: dict, text: str) -> bool:
    t = norm(text)
    return t == pair["an"] if pair["cs"] else same(t, pair["an"])


# --- recorrido del documento -----------------------------------------------------------------

W_R, W_T, W_TAB, W_BR, W_CR, W_NBH = (qn(x) for x in ("w:r", "w:t", "w:tab", "w:br", "w:cr", "w:noBreakHyphen"))
SKIP_IN_P = {qn(x) for x in ("w:pPr", "w:del", "w:moveFrom", "w:instrText", "w:delText")}
INDEX_STYLE = re.compile(r"^(toc\s?\d|tdc\s?\d|table of figures|tabla de ilustraciones|tabladeilustraciones|index\s?\d)", re.I)


def own_runs(p_el):
    """Las w:r que pertenecen a este párrafo (no las de párrafos anidados en cuadros de texto)."""
    stack = list(reversed(list(p_el)))
    while stack:
        el = stack.pop()
        if el.tag in SKIP_IN_P:
            continue
        if el.tag == W_R:
            yield el
        elif el.tag in (qn("w:hyperlink"), qn("w:fldSimple"), qn("w:smartTag"), qn("w:sdt"), qn("w:sdtContent"), qn("w:customXml"), qn("w:ins")):
            stack.extend(reversed(list(el)))


def run_text(r_el) -> str:
    out = []
    for ch in r_el:
        if ch.tag == W_T:
            out.append(ch.text or "")
        elif ch.tag in (W_TAB, qn("w:ptab")):
            out.append("\t")
        elif ch.tag == W_BR:
            t = ch.get(qn("w:type"))
            out.append("\f" if t in ("page", "column") else "\n")
        elif ch.tag == W_CR:
            out.append("\n")
        elif ch.tag == W_NBH:
            out.append("-")
    return "".join(out)


def _on(el) -> bool:
    return el is not None and el.get(qn("w:val")) not in ("0", "false", "off")


def run_key(r_el) -> tuple:
    """Formato de carácter directo del run, reducido a lo que se ve (sin rsid, idioma ni variantes «Cs»)."""
    rpr = r_el.find(qn("w:rPr"))
    key = []
    if rpr is not None:
        for tag in ("b", "i", "strike", "caps", "smallCaps", "dstrike", "vanish"):
            el = rpr.find(qn("w:" + tag))
            if el is not None:
                key.append((tag, _on(el)))
        for tag, att in (("u", "val"), ("color", "val"), ("sz", "val"), ("highlight", "val"), ("vertAlign", "val"), ("rStyle", "val")):
            el = rpr.find(qn("w:" + tag))
            if el is not None and el.get(qn("w:" + att)) is not None:
                v = el.get(qn("w:" + att))
                if tag == "u" and v == "none":
                    continue
                if tag == "color" and v.lower() in ("auto", "000000"):
                    continue
                key.append((tag, v.lower() if tag == "color" else v))
        rf = rpr.find(qn("w:rFonts"))
        if rf is not None:
            for att in ("ascii", "hAnsi"):
                if rf.get(qn("w:" + att)):
                    key.append(("font", rf.get(qn("w:" + att))))
                    break
    return tuple(sorted(set((k for k in key if not (k[0] in ("b", "i", "strike", "caps", "smallCaps", "dstrike", "vanish") and k[1] is False)))))


def para_chars(p_el) -> list[tuple[str, tuple]]:
    out: list[tuple[str, tuple]] = []
    for r in own_runs(p_el):
        k = run_key(r)
        out.extend((ch, k) for ch in run_text(r))
    return out


class Unit:
    """Un párrafo del documento, con su posición, texto, estilo y formato de carácter por carácter."""

    def __init__(self, i: int, p_el, doc) -> None:
        self.i = i
        self.el = p_el
        self.para = Paragraph(p_el, doc)
        self.chars = para_chars(p_el)
        self.text = "".join(c for c, _ in self.chars)
        self.style = self.para.style.name if self.para.style is not None else ""
        self.in_box = any(a.tag == qn("w:txbxContent") for a in p_el.iterancestors())
        self.is_index = bool(INDEX_STYLE.match(self.style or "")) or bool(INDEX_STYLE.match(self.para.style.style_id if self.para.style is not None else ""))
        self.in_table = any(a.tag == qn("w:tc") for a in p_el.iterancestors())


def units(doc) -> list[Unit]:
    return [Unit(i, p, doc) for i, p in enumerate(doc.element.body.iter(qn("w:p")))]


# --- formato de párrafo efectivo (como verificar_cap5.py) --------------------------------------

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
    for e in levels:
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
        "jc": {"start": "left", "end": "right"}.get(attr("w:jc", "w:val") or "left", attr("w:jc", "w:val") or "left"),
        "keepNext": flag("w:keepNext"),
        "numbered": any(e.find(qn("w:numPr")) is not None for e in levels[:-1]),
    }


def ppr_diff(a: dict, b: dict) -> list[str]:
    bad = []
    for k in a:
        if isinstance(a[k], float):
            if abs(a[k] - b[k]) > 0.5:
                bad.append(f"{k} {a[k]:g} contra {b[k]:g}")
        elif a[k] != b[k]:
            bad.append(f"{k} {a[k]} contra {b[k]}")
    return bad


# --- utilidades de comparación ----------------------------------------------------------------

def strip_page(text: str) -> str:
    return norm(re.sub(r"\t\s*\d+\s*$", "", text))


def common_span(old: str, new: str) -> tuple[int, int]:
    """Caracteres que old y new comparten por el principio y por el final (distinguiendo mayúsculas)."""
    mx = min(len(old), len(new))
    p = 0
    while p < mx and old[p] == new[p]:
        p += 1
    s = 0
    while s < mx - p and old[len(old) - 1 - s] == new[len(new) - 1 - s]:
        s += 1
    return p, s


def table_sig(doc) -> list[tuple]:
    out = []
    for t in doc.element.body.iter(qn("w:tbl")):
        rows = t.findall(qn("w:tr"))
        cells = sum(len(r.findall(qn("w:tc"))) for r in rows)
        spans = sum(int(g.get(qn("w:val")) or 1) for g in t.iter(qn("w:gridSpan")))
        vm = len(list(t.iter(qn("w:vMerge"))))
        out.append((len(rows), cells, spans, vm))
    return out


def image_hashes(doc) -> list[str]:
    hs = []
    for part in doc.part.package.iter_parts():
        if str(part.partname).startswith("/word/media/"):
            hs.append(hashlib.sha256(part.blob).hexdigest())
    return sorted(hs)


def drawings(doc) -> int:
    b = doc.element.body
    return len(list(b.iter(qn("w:drawing")))) + len(list(b.iter(qn("w:pict")))) + len(list(b.iter(qn("w:object"))))


def section_sig(doc) -> list[tuple]:
    out = []
    for s in doc.sections:
        out.append(tuple(int(v) if v is not None else -1 for v in (
            s.page_width, s.page_height, s.left_margin, s.right_margin, s.top_margin, s.bottom_margin, int(s.orientation))))
    return out


def story_texts(doc) -> list[str]:
    out = []
    for s in doc.sections:
        for part in (s.header, s.footer, s.first_page_header, s.first_page_footer, s.even_page_header, s.even_page_footer):
            out.append(norm(" ".join(p.text for p in part.paragraphs)))
    return out


def side_parts_text(doc) -> dict[str, str]:
    """Texto de las notas al pie, notas finales y comentarios (si el paquete los trae)."""
    out = {}
    for part in doc.part.package.iter_parts():
        name = str(part.partname)
        if name in ("/word/footnotes.xml", "/word/endnotes.xml", "/word/comments.xml"):
            out[name] = norm(re.sub(r"<[^>]+>", " ", part.blob.decode("utf-8", "replace")))
    return out


def bookmark_names(doc) -> set[str]:
    return {b.get(qn("w:name")) for b in doc.element.body.iter(qn("w:bookmarkStart"))}


def revision_count(doc) -> int:
    tags = ("w:ins", "w:del", "w:moveFrom", "w:moveTo", "w:rPrChange", "w:pPrChange")
    return sum(len(list(doc.element.body.iter(qn(t)))) for t in tags)


# --- programa ----------------------------------------------------------------------------------

def main(argv=None) -> int:
    for stream in (sys.stdout, sys.stderr):  # el texto trae tildes y «»: que no falle con la página de códigos de Windows
        try:
            stream.reconfigure(encoding="utf-8", errors="replace")
        except (AttributeError, ValueError):
            pass
    ap = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    ap.add_argument("--docx", required=True, help="el .docx resultante")
    ap.add_argument("--original", required=True, help="el .docx de antes")
    ap.add_argument("--pares", required=True, help="el .json de pares")
    args = ap.parse_args(argv)
    for f in (args.docx, args.original, args.pares):
        if not Path(f).exists():
            print(f"No existe: {f}", file=sys.stderr)
            return 2
    try:
        pairs = read_pairs(Path(args.pares))
    except (ValueError, json.JSONDecodeError) as e:
        print(f"Uso: {e}", file=sys.stderr)
        return 2

    R = Report()
    new = docx.Document(args.docx)
    old = docx.Document(args.original)
    un, uo = units(new), units(old)

    # --- 1. los pares en el original ---------------------------------------------------------
    print("== 1. Los pares en el original")
    expect: dict[int, dict] = {}   # posición (en unidades no de índice) -> par
    ok_pairs = True
    # Las líneas de índice (TOC, listas de figuras) y los cuadros de texto quedan fuera de la búsqueda.
    searchable = [u for u in uo if not u.is_index and not u.in_box]
    for pr in pairs:
        hit = [u for u in searchable if matches(pr, u.text)]
        if len(hit) != pr["veces"]:
            R.fail(f"par {pr['n']} [{short(pr['antes'], 45)}]: aparece {len(hit)} vez/veces en el original y se esperaba {pr['veces']}")
            ok_pairs = False
            continue
        for u in hit:
            expect[u.i] = pr
        R.ok(f"par {pr['n']} [{short(pr['antes'], 45)}] -> [{short(pr['despues'], 45)}]: {len(hit)} párrafo(s) en el original")
    if not ok_pairs:
        return _end(R)

    # --- 2. alineación de párrafos ------------------------------------------------------------
    print("== 2. Párrafos")
    fixed_o = [u for u in uo if not u.is_index]
    fixed_n = [u for u in un if not u.is_index]
    if len(fixed_o) != len(fixed_n):
        R.fail(f"el número de párrafos cambió: {len(fixed_o)} en el original y {len(fixed_n)} en el resultado")
        ko = [(expect[u.i]["despues"] if u.i in expect else u.text) for u in fixed_o]
        sm = difflib.SequenceMatcher(a=ko, b=[u.text for u in fixed_n], autojunk=False)
        for tag, i1, i2, j1, j2 in [op for op in sm.get_opcodes() if op[0] != "equal"][:5]:
            print(f"      {tag}: original[{i1}:{i2}] {[short(x, 50) for x in ko[i1:i2][:2]]} -> resultado[{j1}:{j2}] {[short(u.text, 50) for u in fixed_n[j1:j2][:2]]}")
        return _end(R)
    R.ok(f"mismo número de párrafos ({len(fixed_o)} fuera de los índices)")

    bad_text, bad_style, bad_ppr, bad_chars = [], [], [], []
    changed_ok = 0
    for uo_, un_ in zip(fixed_o, fixed_n):
        pr = expect.get(uo_.i)
        label = f"{uo_.i} «{short(uo_.text, 40)}»"
        if pr is None:
            if uo_.text != un_.text:
                bad_text.append(f"{label}: ahora dice «{short(un_.text, 60)}»")
            if uo_.style != un_.style:
                bad_style.append(f"{label}: estilo {uo_.style} -> {un_.style}")
            d = ppr_diff(eff_ppr(uo_.para, old), eff_ppr(un_.para, new))
            if d:
                bad_ppr.append(f"{label}: {'; '.join(d)}")
            if uo_.text == un_.text and [k for _, k in uo_.chars] != [k for _, k in un_.chars]:
                bad_chars.append(label)
            continue
        # párrafo sustituido
        if un_.text != pr["despues"]:
            bad_text.append(f"{label}: debía decir «{short(pr['despues'], 60)}» y dice «{short(un_.text, 60)}»")
            continue
        changed_ok += 1
        if uo_.style != un_.style:
            bad_style.append(f"{label}: estilo {uo_.style} -> {un_.style}")
        d = ppr_diff(eff_ppr(uo_.para, old), eff_ppr(un_.para, new))
        if d:
            bad_ppr.append(f"{label} (sustituido): {'; '.join(d)}")
        p, s = common_span(uo_.text, un_.text)
        if p > 0 and [k for _, k in uo_.chars[:p]] != [k for _, k in un_.chars[:p]]:
            bad_chars.append(f"{label} (sustituido): cambió el formato de lo que se conservaba al principio")
        if s > 0 and [k for _, k in uo_.chars[len(uo_.chars) - s:]] != [k for _, k in un_.chars[len(un_.chars) - s:]]:
            bad_chars.append(f"{label} (sustituido): cambió el formato de lo que se conservaba al final")
        old_mid = [k for _, k in uo_.chars[p:len(uo_.chars) - s]]
        new_mid = [k for _, k in un_.chars[p:len(un_.chars) - s]]
        if new_mid:
            allowed = set(k for _, k in uo_.chars)
            if old_mid and len(set(old_mid)) == 1 and set(new_mid) != {old_mid[0]}:
                bad_chars.append(f"{label} (sustituido): el texto nuevo no tiene el formato del tramo que sustituye")
            elif not set(new_mid) <= allowed:
                R.warn(f"{label}: el texto nuevo usa un formato que el párrafo no tenía")
    R.check(changed_ok == sum(p["veces"] for p in pairs) and not [b for b in bad_text if "debía decir" in b],
            f"los {changed_ok} párrafos sustituidos dicen exactamente su «despues»",
            "hay párrafos sustituidos que no dicen su «despues»:\n      " + "\n      ".join(b for b in bad_text if "debía decir" in b))
    others = [b for b in bad_text if "debía decir" not in b]
    R.check(not others, f"el texto de los {len(fixed_o) - len(expect)} párrafos restantes es idéntico al original",
            f"{len(others)} párrafos que no debían cambiar tienen otro texto:\n      " + "\n      ".join(others[:8]))
    R.check(not bad_style, "el estilo de párrafo de todos los párrafos es el del original", f"{len(bad_style)} párrafos cambiaron de estilo:\n      " + "\n      ".join(bad_style[:8]))
    changed_ppr = [b for b in bad_ppr if "(sustituido)" in b]
    R.check(not changed_ppr, "sangría, espaciado y alineación de los párrafos sustituidos son los del original", "cambió el formato de párrafo de un párrafo sustituido:\n      " + "\n      ".join(changed_ppr[:8]))
    if [b for b in bad_ppr if "(sustituido)" not in b]:
        R.warn(f"{len(bad_ppr) - len(changed_ppr)} párrafos sin sustituir con sangría, espaciado o alineación efectivos distintos:\n      " + "\n      ".join([b for b in bad_ppr if "(sustituido)" not in b][:6]))
    fmt_fail = [b for b in bad_chars if "(sustituido)" in b]
    R.check(not fmt_fail, "en los párrafos sustituidos, lo que se conserva mantiene su formato de carácter (negritas, cursivas...)", "cambió el formato de carácter en párrafos sustituidos:\n      " + "\n      ".join(fmt_fail[:8]))
    fmt_warn = [b for b in bad_chars if "(sustituido)" not in b]
    if fmt_warn:
        R.warn(f"{len(fmt_warn)} párrafos sin sustituir con otro formato de carácter, p. ej. {fmt_warn[:3]}")
    else:
        R.ok("el formato de carácter de los párrafos sin sustituir es el del original")

    # --- 3. líneas de índice -------------------------------------------------------------------
    idx_o = [u for u in uo if u.is_index]
    idx_n = [u for u in un if u.is_index]
    if idx_o or idx_n:
        print("== 3. Tabla de contenido y listas de figuras")
        if len(idx_o) != len(idx_n):
            R.warn(f"el índice pasó de {len(idx_o)} a {len(idx_n)} líneas (Word lo regeneró)")
        else:
            odd = []
            for a, b in zip(idx_o, idx_n):
                ta, tb = strip_page(a.text), strip_page(b.text)
                if ta == tb:
                    continue
                explained = any(pr["an"].lower() in ta.lower() and pr["dn"].lower() in tb.lower() for pr in pairs)
                if not explained:
                    odd.append(f"«{short(ta, 50)}» -> «{short(tb, 50)}»")
            if odd:
                R.warn(f"{len(odd)} líneas de índice cambiaron sin que un par las explique (¿índice desactualizado antes?):\n      " + "\n      ".join(odd[:6]))
            else:
                R.ok(f"las {len(idx_o)} líneas de índice son las del original o las explican los pares (sin contar números de página)")

    # --- 4. estructura ---------------------------------------------------------------------------
    print("== 4. Estructura del documento")
    to, tn = table_sig(old), table_sig(new)
    R.check(to == tn, f"mismas tablas ({len(to)}): filas, celdas y combinaciones", f"cambiaron las tablas: {to} contra {tn}")
    R.check(drawings(old) == drawings(new) and len(old.inline_shapes) == len(new.inline_shapes), f"mismas imágenes y dibujos ({drawings(old)})", "cambió el número de imágenes o dibujos")
    ho, hn = image_hashes(old), image_hashes(new)
    if len(ho) != len(hn):
        R.fail(f"cambió el número de archivos de imagen: {len(ho)} contra {len(hn)}")
    elif ho != hn:
        R.warn("las imágenes tienen el mismo número pero no los mismos bytes (Word las recodificó)")
    else:
        R.ok(f"los {len(ho)} archivos de imagen son idénticos byte a byte")
    so, sn = section_sig(old), section_sig(new)
    R.check(so == sn, f"mismas secciones ({len(so)}) con el mismo tamaño de página, márgenes y orientación", f"cambiaron las secciones: {so} contra {sn}")
    R.check(story_texts(old) == story_texts(new), "encabezados y pies de página iguales", "cambiaron los encabezados o pies de página")
    spo, spn = side_parts_text(old), side_parts_text(new)
    R.check(spo == spn, "notas al pie, notas finales y comentarios iguales" + ("" if spo else " (el documento no tiene)"), "cambiaron las notas al pie, las notas finales o los comentarios: " + ", ".join(k for k in set(spo) | set(spn) if spo.get(k) != spn.get(k)))
    lost = {b for b in bookmark_names(old) - bookmark_names(new) if b != "_GoBack"}
    lost_hard = sorted(b for b in lost if not b.startswith("_Toc"))
    R.check(not lost_hard, "ningún marcador se perdió (los de referencias cruzadas siguen)" if not lost else f"ningún marcador se perdió salvo {len(lost)} de índice (_Toc), que Word regenera", "se perdieron marcadores: " + ", ".join(lost_hard[:8]))
    r = revision_count(new)
    R.check(r == 0, "sin revisiones (w:ins, w:del, w:moveFrom, w:moveTo, w:rPrChange, w:pPrChange)", f"el documento trae {r} revisiones")

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
