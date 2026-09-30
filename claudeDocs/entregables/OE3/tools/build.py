"""Arma el entregable del OE3: el documento principal (capítulos de src/) y cada anexo en su propio
.docx — A (matriz RF→pruebas), B (las fases del Slice 1), C–G (los demás documentos de resultados)
y H (actas).

Uso: python claudeDocs/entregables/OE3/tools/build.py
Requiere pypandoc_binary (pandoc 3.9) y Microsoft Word (para tablas, numeración de páginas,
tabla de contenido y PDF de revisión, vía tools/word_finalize.ps1).
"""
import pathlib
import re
import subprocess
import sys
import zipfile

import pypandoc

OE3 = pathlib.Path(__file__).resolve().parents[1]
ROOT = OE3.parents[2]
TASKS = ROOT / "claudeDocs" / "tasks"
ACTAS = ROOT / "docs" / "actas" / "OE3"
REFERENCE = ROOT / "docs" / "Solucion_OE2_Diseno_final.docx"
DELIVERY = OE3.parent  # claudeDocs/entregables: el documento principal y sus anexos, juntos
BUILD = OE3 / "build"
MAIN = "Solucion_OE3_Prototipo_funcional"

CHAPTERS = [
    "01-objetivo.md", "02-metodologia.md", "03-arquitectura.md", "04a-progresion-nivel1.md",
    "04b-nivel2.md", "04c-nivel3-narrativa.md", "05-mecanicas-andamiaje.md", "07-interfaz.md",
    "08-rendimiento.md", "09-verificacion.md", "10-limitaciones.md", "11-conclusiones.md",
    "12-control-cambios.md",
]

# Letra, sufijo del archivo y origen: un documento de resultados, la matriz generada, el Slice 1
# (sus fases, en un solo anexo) o las actas.
ANNEXES = [
    ("A", "Matriz_trazabilidad", "matriz"),
    ("B", "Slice_1", "slice1"),
    ("C", "Slice_2", "Slice 2/Slice-2-Resultados.md"),
    ("D", "Slice_3", "Slice 3/Slice-3-Resultados.md"),
    ("E", "Arte_y_sonido", "Slice 3/Props-y-Sonidos-Resultados.md"),
    ("F", "Personajes", "Personajes/Personajes-Resultados.md"),
    ("G", "Slice_4", "Slice 4/Slice-4-Resultados.md"),
    ("H", "Actas", "actas"),
]

# Las fases del Slice 1, en el orden en que cerraron; cada una conserva su texto y su numeración.
SLICE_1 = [
    "Slice 1/Fase-0-Resultados.md",
    "Slice 1/Fase-1-Resultados.md",
    "Slice 1/Fase-2-Resultados.md",
    "Slice 1/Fase-3-Resultados.md",
    "Slice 1/Fase-5-6-Resultados.md",
]

# Identificadores que pandoc escribe al pasar de gfm a markdown: no van en el texto.
HEADING_ID = re.compile(r"[ \t]*\{#[^}\n]*\}[ \t]*$", re.M)

EMOJI = {"✅": "✓", "❌": "✗", "\U0001f7e1": "Parcial:", "⏳": "Pendiente"}

PAGE_BREAK = '```{=openxml}\n<w:p><w:r><w:br w:type="page"/></w:r></w:p>\n```'


def toc(depth: int = 2) -> str:
    """Campo de tabla de contenido hasta ese nivel de encabezado; Word lo llena al terminar."""
    return ('```{=openxml}\n'
            '<w:p><w:r><w:fldChar w:fldCharType="begin" w:dirty="true"/></w:r>'
            f'<w:r><w:instrText xml:space="preserve"> TOC \\o "1-{depth}" \\h \\z \\u </w:instrText></w:r>'
            '<w:r><w:fldChar w:fldCharType="separate"/></w:r><w:r><w:t>Tabla de contenido</w:t></w:r>'
            '<w:r><w:fldChar w:fldCharType="end"/></w:r></w:p>\n```')

# Pandoc marca los párrafos con estilos de su plantilla inglesa (BodyText, Compact, ImageCaption…);
# la del OE2 está en español y no los define con esos identificadores, así que Word los descarta y
# todo queda en «Normal», sin espacio entre párrafos. Se añaden antes de abrir el documento en Word.
PANDOC_STYLES = """
<w:style w:type="paragraph" w:customStyle="1" w:styleId="BodyText"><w:name w:val="Texto OE3"/><w:basedOn w:val="Normal"/><w:qFormat/><w:pPr><w:spacing w:before="0" w:after="120"/></w:pPr></w:style>
<w:style w:type="paragraph" w:customStyle="1" w:styleId="FirstParagraph"><w:name w:val="Primer párrafo OE3"/><w:basedOn w:val="BodyText"/><w:qFormat/></w:style>
<w:style w:type="paragraph" w:customStyle="1" w:styleId="Compact"><w:name w:val="Compacto OE3"/><w:basedOn w:val="BodyText"/><w:qFormat/><w:pPr><w:spacing w:before="0" w:after="40"/></w:pPr></w:style>
<w:style w:type="paragraph" w:customStyle="1" w:styleId="BlockText"><w:name w:val="Cita OE3"/><w:basedOn w:val="BodyText"/><w:qFormat/><w:pPr><w:ind w:left="567" w:right="567"/></w:pPr><w:rPr><w:i/></w:rPr></w:style>
<w:style w:type="paragraph" w:customStyle="1" w:styleId="CaptionedFigure"><w:name w:val="Figura OE3"/><w:basedOn w:val="BodyText"/><w:qFormat/><w:pPr><w:keepNext/><w:spacing w:before="120" w:after="60"/><w:jc w:val="center"/></w:pPr></w:style>
<w:style w:type="paragraph" w:customStyle="1" w:styleId="ImageCaption"><w:name w:val="Leyenda de figura OE3"/><w:basedOn w:val="BodyText"/><w:qFormat/><w:pPr><w:spacing w:after="200"/><w:jc w:val="center"/></w:pPr><w:rPr><w:i/><w:sz w:val="20"/></w:rPr></w:style>
<w:style w:type="paragraph" w:customStyle="1" w:styleId="SourceCode"><w:name w:val="Código OE3"/><w:basedOn w:val="Normal"/><w:pPr><w:spacing w:after="120"/><w:ind w:left="284"/></w:pPr><w:rPr><w:rFonts w:ascii="Consolas" w:hAnsi="Consolas"/><w:sz w:val="18"/></w:rPr></w:style>
<w:style w:type="character" w:customStyle="1" w:styleId="VerbatimChar"><w:name w:val="Código en línea OE3"/><w:rPr><w:rFonts w:ascii="Consolas" w:hAnsi="Consolas"/><w:sz w:val="19"/></w:rPr></w:style>
"""


def to_markdown(gfm: str) -> str:
    """Un documento del repositorio (GitHub Markdown) en el Markdown de pandoc, listo para el Word."""
    # Los enlaces a otros .md del repositorio no existen dentro del Word: queda el texto.
    gfm = re.sub(r"\[([^\]]+)\]\((?!https?:)[^)]*\)", r"\1", gfm)
    # Los emoji de estado salen a color en Word; en un entregable se leen como símbolos sobrios.
    for emoji, plain in EMOJI.items():
        gfm = gfm.replace(emoji, plain)
    text = pypandoc.convert_text(gfm, "markdown", format="gfm", extra_args=["--wrap=none"])
    # En Windows pandoc devuelve CRLF, y con él la expresión de los identificadores no llega al fin de línea.
    return HEADING_ID.sub("", text.replace("\r\n", "\n"))


def headings(lines: list[str], rewrite) -> list[str]:
    """Aplica `rewrite(nivel, texto)` a cada encabezado ATX, sin tocar los bloques de código."""
    out, fenced = [], False
    for line in lines:
        if line.startswith("```") or line.startswith("~~~"):
            fenced = not fenced
        match = None if fenced else re.match(r"^(#{1,6}) (.*)$", line)
        out.append(rewrite(len(match.group(1)), match.group(2)) if match else line)
    return out


def internal(text: str) -> str:
    """Los anexos propios de un documento de resultados pasan a «Apéndice»: dentro del entregable,
    «Anexo B» es otra cosa."""
    return re.sub(r"^Anexo\b", "Apéndice", text)


def origin(letter: str) -> str:
    """La línea que ata un anexo suelto a su documento principal."""
    return (f"*Anexo {letter} del entregable del tercer objetivo específico del trabajo de grado. "
            f"Documento principal: {MAIN}.docx.*")


def results_annex(letter: str, relative: str) -> str:
    """Un documento de resultados como anexo: su título pasa a «ANEXO X.»; sus secciones conservan el nivel."""
    lines = to_markdown((TASKS / relative).read_text(encoding="utf-8")).splitlines()
    titled = False

    def rewrite(level: int, text: str) -> str:
        nonlocal titled
        if level == 1 and not titled:
            titled = True
            return f"# ANEXO {letter}. {text.upper()}\n\n{origin(letter)}"
        return "#" * level + " " + internal(text)

    out = headings(lines, rewrite)
    if not titled:
        raise SystemExit(f"{relative}: no tiene título de nivel 1")
    return "\n".join(out)


def slice1_annex(letter: str) -> str:
    """Anexo B: los documentos de resultados de las fases del Slice 1, uno tras otro. El título de
    cada fase pasa a sección y lo suyo baja un nivel; el documento principal las cita como
    «Anexo B, Fase N, §x»."""
    parts = [f"# ANEXO {letter}. SLICE 1 — GOLDEN PATH TEMPRANO: RESULTADOS DE LAS FASES 0 A 3, 5 Y 6",
             "", origin(letter), "",
             "Reúne los cinco documentos de resultados del primer incremento, en el orden en que cerraron "
             "sus fases: Fase 0 (cimientos), Fase 1 (navegación mínima), Fase 2 (andamiaje mínimo), Fase 3 "
             "(primera versión del Nivel 1) y Fases 5 y 6 (mecánica vigente del Nivel 1, INC-47). Cada fase "
             "empieza en página nueva y conserva íntegros su texto y su numeración propia; el documento "
             "principal las cita como «Anexo B, Fase N, §x»."]
    for relative in SLICE_1:
        lines = to_markdown((TASKS / relative).read_text(encoding="utf-8")).splitlines()
        titled = False

        def rewrite(level: int, text: str) -> str:
            nonlocal titled
            if level == 1 and not titled:
                titled = True
                return f"## {text}"
            return "#" * (level + 1) + " " + internal(text)

        body = headings(lines, rewrite)
        if not titled:
            raise SystemExit(f"{relative}: no tiene título de nivel 1")
        parts.append("\n".join(["", PAGE_BREAK, "", *body]))
    return "\n\n".join(parts)


def matrix_annex() -> str:
    """El Anexo A lo genera tools/rf_matrix.py en src/A-matriz-rf.md."""
    lines = (OE3 / "src" / "A-matriz-rf.md").read_text(encoding="utf-8").splitlines()
    return "\n".join(headings(lines, lambda level, text: f"# {text}\n\n{origin('A')}" if level == 1
                              else "#" * level + " " + text))


def actas_annex(letter: str) -> str:
    """El anexo de las actas: las nueve del tercer objetivo, íntegras, una por página."""
    parts = [f"# ANEXO {letter}. ACTAS DE SEGUIMIENTO DEL OBJETIVO ESPECÍFICO 3 (D01–D09)", "", origin(letter), "",
             "Transcripción íntegra de las nueve actas de sesión de trabajo del tercer objetivo, "
             "del 2 al 24 de septiembre de 2026. La sección 6 de cada acta contiene el tablero Kanban "
             "del proyecto (véase el documento principal, §2.3)."]
    for acta in sorted(ACTAS.glob("Acta_D0*.md")):
        date = re.search(r"(\d{4})-(\d{2})-(\d{2})", acta.name)
        lines = acta.read_text(encoding="utf-8").splitlines()
        # El título es la primera línea: en unas actas es encabezado («# ACTA…») y en otras texto
        # plano, con las secciones en el nivel 1. Las secciones pasan al nivel 3 en todas.
        title = lines[0].lstrip("#").strip().strip("*").strip()
        levels = []
        headings(lines[1:], lambda level, text: levels.append(level) or "")
        shift = 3 - min(levels) if levels else 0
        body = headings(lines[1:], lambda level, text: "#" * (level + shift) + " " + text)
        text = to_markdown("\n".join(body))
        series = re.search(r"Acta_(D\d+)", acta.name).group(1)
        if series not in title:
            title += f" ({series})"  # la D01 se registró como O03, la tercera de la serie del OE2
        heading = f"## {title} — {date.group(3)}/{date.group(2)}/{date.group(1)}"
        parts.append("\n".join(["", PAGE_BREAK, "", heading, "", text]))
    return "\n\n".join(parts)


def with_page_breaks(markdown: str) -> str:
    """Salto de página antes de cada capítulo (encabezado de nivel 1), salvo el primero."""
    out, fenced, first = [], False, True
    for line in markdown.splitlines():
        if line.startswith("```") or line.startswith("~~~"):
            fenced = not fenced
        if not fenced and line.startswith("# "):
            if not first:
                out += ["", PAGE_BREAK, ""]
            first = False
        out.append(line)
    return "\n".join(out)


def with_toc(markdown: str, depth: int = 2) -> str:
    """Tabla de contenido al principio si el documento tiene secciones que listar."""
    sections = []
    headings(markdown.splitlines(), lambda level, text: sections.append(level) or "")
    return f"{toc(depth)}\n\n{PAGE_BREAK}\n\n{markdown}" if sections.count(2) >= 3 else markdown


def add_styles(docx: pathlib.Path) -> None:
    """Añade al .docx de pandoc los estilos de párrafo y carácter que la plantilla no trae, y deja
    que Word reparta el ancho de las columnas según su contenido."""
    temporary = docx.with_suffix(".tmp")
    with zipfile.ZipFile(docx) as source, zipfile.ZipFile(temporary, "w", zipfile.ZIP_DEFLATED) as target:
        for item in source.infolist():
            data = source.read(item.filename)
            if item.filename == "word/styles.xml":
                data = data.decode("utf-8").replace("</w:styles>", PANDOC_STYLES + "</w:styles>").encode("utf-8")
            elif item.filename == "word/document.xml":
                # Pandoc fija el diseño de las tablas con columnas iguales: una columna de una letra
                # ocupaba lo mismo que una de texto, y Word no lo recalcula aunque se le pida.
                data = data.decode("utf-8").replace('<w:tblLayout w:type="fixed" />',
                                                    '<w:tblLayout w:type="autofit" />').encode("utf-8")
            target.writestr(item, data)
    temporary.replace(docx)


def render(markdown: str, stem: str) -> tuple[pathlib.Path, pathlib.Path]:
    """Markdown → .docx con la plantilla del OE2 y los estilos que faltan. Devuelve (.docx, .pdf de revisión)."""
    (BUILD / f"{stem}.md").write_text(markdown, encoding="utf-8")
    docx = DELIVERY / f"{stem}.docx"
    pypandoc.convert_text(
        markdown, "docx", format="markdown-auto_identifiers", outputfile=str(docx),
        extra_args=[f"--reference-doc={REFERENCE}", f"--resource-path={OE3}", "--wrap=none",
                    "--syntax-highlighting=none"])
    add_styles(docx)
    return docx, BUILD / f"{stem}.pdf"


def annex_markdown(letter: str, source: str) -> str:
    if source == "matriz":
        return matrix_annex()
    if source == "actas":
        return actas_annex(letter)
    if source == "slice1":
        return slice1_annex(letter)
    return results_annex(letter, source)


def main() -> None:
    missing = [c for c in CHAPTERS + ["A-matriz-rf.md"] if not (OE3 / "src" / c).exists()]
    if missing:
        raise SystemExit(f"faltan capítulos: {missing}")
    BUILD.mkdir(exist_ok=True)
    # Lo de un armado anterior —anexos con otra letra o nombre— no puede quedar junto a lo nuevo.
    for stale in [*DELIVERY.glob("Solucion_OE3_Anexo_*.docx"), *BUILD.glob("*.md"), *BUILD.glob("*.pdf")]:
        stale.unlink()
    body = with_page_breaks("\n\n".join((OE3 / "src" / c).read_text(encoding="utf-8") for c in CHAPTERS))
    documents = [render(f"{toc()}\n\n{PAGE_BREAK}\n\n{body}\n", MAIN)]
    for letter, suffix, source in ANNEXES:
        # El del Slice 1 reúne cinco documentos: su tabla de contenido baja a las secciones de cada fase.
        depth = 3 if source == "slice1" else 2
        documents.append(render(with_toc(annex_markdown(letter, source), depth) + "\n",
                                f"Solucion_OE3_Anexo_{letter}_{suffix}"))
    # Un solo Word para todos: tablas, pie con número de página, tabla de contenido y PDF.
    listing = BUILD / "documentos.txt"
    listing.write_text("\n".join(f"{docx}\t{pdf}" for docx, pdf in documents) + "\n", encoding="utf-8")
    subprocess.run(["powershell.exe", "-NoProfile", "-ExecutionPolicy", "Bypass", "-File",
                    str(OE3 / "tools" / "word_finalize.ps1"), "-List", str(listing)], check=True)
    for docx, _ in documents:
        print(docx)


if __name__ == "__main__":
    sys.exit(main())
