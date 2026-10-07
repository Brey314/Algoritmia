"""Arma el entregable del OE3: el documento principal (capítulos de src/) y sus siete anexos, cada uno
en su propio .docx. El Anexo A es la matriz RF -> pruebas que escribe tools/rf_matrix.py; los B a G
se redactan en src/anexos/.

Uso (desde cualquier carpeta del repositorio):
    python claudeDocs/entregables/OE3/tools/build.py [--no-word] [--publish]

  (sin opciones)  arma los 8 .docx en claudeDocs/entregables/OE3/build/ (con el Markdown intermedio y
                  un PDF de revisión por documento) y corre las guardas. No toca docs/.
  --no-word       salta tools/word_finalize.ps1: los .docx quedan sin cuadrícula en las tablas, sin
                  número de página, con la tabla de contenido vacía y sin PDF. Es para probar el
                  generador mientras otro proceso usa Word; no se combina con --publish.
  --publish       si las guardas pasan, copia el documento principal a docs/ y los siete anexos a
                  «docs/anexos oe3/» (con espacio en el nombre; se crea si falta), que es donde están
                  versionados los Word publicados. Es lo único que escribe allí: un armado sin
                  --publish no copia ni borra nada. Las figuras no se publican: viven en docs/OE3/fig/
                  (LFS) y son una entrada del armado, no una salida.

Fuentes (todas en Markdown de pandoc, el mismo de los capítulos):
  src/01-... a 12-*.md   los capítulos del documento principal, en el orden de CHAPTERS.
  src/A-matriz-rf.md     Anexo A. Lo escribe rf_matrix.py: se corre antes, y al cierre con --worktree
                         para contar las pruebas del árbol de trabajo.
  src/anexos/B-slice-1.md, C-slice-2.md, D-slice-3.md, E-arte-y-sonido.md, F-personajes.md,
  G-slice-4.md          Anexos B a G. El título de nivel 1 va sin «ANEXO X.»: lo añade este script,
                         junto con la línea que ata el anexo al documento principal; los encabezados
                         «Anexo ...» internos pasan a «Apéndice». Si una fuente no existe, el armado
                         avisa y no genera ese anexo; el resto se arma igual.
Plantilla de estilos: la del OE2 tal como está en el commit 127fbc4, que se extrae con git a
build/ref_oe2_127fbc4.docx (la del árbol cambió el 29-30/09/2026 y el entregable no debe cambiar de
aspecto). Las figuras (fig/) se buscan en OE3/ y en docs/OE3/ (--resource-path): docs/OE3/fig/ guarda
los 16 .jpg de las capturas del prototipo, más las figuras del Anexo F, versionados con Git LFS; un
clon con `git lfs` apagado los trae como punteros de ~130 bytes y la guarda de figuras lo avisa.

Guardas, sobre el Markdown intermedio de build/ (se corren en cada armado; con --publish, si alguna
falla no se abre Word ni se copia nada):
  - se armaron los 8 documentos;
  - ningún «§» (las remisiones del entregable son «apartado 9.1»), ningún «Anexo H» / «ANEXO H» ni
    rango de anexos que llegue a la H: no hay Anexo H. Las actas de seguimiento no se anexan: están en
    Word en el SharePoint de Santiago y, desde el commit 31483e6, también versionadas en docs/actas/;
  - ningún marcador «{{...}}» sin rellenar ni comentario de trabajo «<!--»;
  - el principal trae las Tablas 1.2, 2.2, 4.12, 8.1, 8.2, 8.3, 9.3 y 9.5 y las Figuras 4.2, 4.7 y 4.8,
    que cita el trabajo de grado (CITED_TABLES, CITED_FIGURES);
  - cada figura fig/... existe y está descargada de LFS (pesa más de 1 KB).
Si falta la fuente de un anexo, el armado lo avisa y sigue con los demás; la guarda de los 8 documentos
falla, así que no se publica un conjunto incompleto. Las líneas que citan las guardas son las del
Markdown de build/, no las de src/: se ubican por el extracto.
Sale con 0 si todo está en orden y con 1 si alguna guarda falla (los .docx de build/ quedan armados de
todos modos) o si falta un capítulo o la plantilla.

Requiere pypandoc_binary (pandoc 3.9), git y, salvo con --no-word, Microsoft Word sin otra
automatización en curso: word_finalize.ps1 abre su propia instancia por COM y la cierra al terminar.
"""
import argparse
import os
import pathlib
import re
import shutil
import subprocess
import sys
import zipfile

import pypandoc

OE3 = pathlib.Path(__file__).resolve().parents[1]
ROOT = OE3.parents[2]
SRC = OE3 / "src"
BUILD = OE3 / "build"
# Destinos de --publish: el principal va a docs/ y los anexos, uno por .docx, a docs/anexos oe3/.
PUBLISHED_MAIN = ROOT / "docs"
PUBLISHED_ANNEXES = ROOT / "docs" / "anexos oe3"
FIGURES_HOME = ROOT / "docs" / "OE3"  # allí está fig/ (LFS): entrada del armado, no destino
RESOURCE_PATH = [OE3, FIGURES_HOME]
MAIN = "Solucion_OE3_Prototipo_funcional"
# docs/Solucion_OE2_Diseno_final.docx es un radicado que se edita (cambió el 29-30/09/2026): la
# plantilla de estilos se toma de como estaba al armarse el entregable, no del árbol.
REFERENCE_SOURCE = "127fbc4:docs/Solucion_OE2_Diseno_final.docx"
REFERENCE = BUILD / "ref_oe2_127fbc4.docx"

CHAPTERS = [
    "01-objetivo.md", "02-metodologia.md", "03-arquitectura.md", "04a-progresion-nivel1.md",
    "04b-nivel2.md", "04c-nivel3-narrativa.md", "05-mecanicas-andamiaje.md", "07-interfaz.md",
    "08-rendimiento.md", "09-verificacion.md", "10-limitaciones.md", "11-conclusiones.md",
    "12-control-cambios.md",
]

# Letra, sufijo del archivo y fuente (relativa a src/) de cada anexo.
ANNEXES = [
    ("A", "Matriz_trazabilidad", "A-matriz-rf.md"),
    ("B", "Slice_1", "anexos/B-slice-1.md"),
    ("C", "Slice_2", "anexos/C-slice-2.md"),
    ("D", "Slice_3", "anexos/D-slice-3.md"),
    ("E", "Arte_y_sonido", "anexos/E-arte-y-sonido.md"),
    ("F", "Personajes", "anexos/F-personajes.md"),
    ("G", "Slice_4", "anexos/G-slice-4.md"),
]

# Las tablas del principal que cita el trabajo de grado: renumerarlas rompe sus remisiones.
CITED_TABLES = ("1.2", "2.2", "4.12", "8.1", "8.2", "8.3", "9.3", "9.5")
CITED_FIGURES = ("4.2", "4.7", "4.8")
FORBIDDEN = (
    ("«§»", re.compile("§")),
    ("«Anexo H»", re.compile(r"\banexo h\b", re.I)),
    ("un rango de anexos que llega a la H", re.compile(r"\banexos? [A-G](?: a |\s*[–-]\s*)H\b", re.I)),
    ("un marcador «{{...}}» sin rellenar", re.compile(r"\{\{")),
    ("un comentario de trabajo «<!--»", re.compile(r"<!--")),
)

# Enlaces a otros archivos del repositorio (no imágenes): dentro del Word no existen y queda el texto.
REPO_LINK = re.compile(r"(?<!!)\[([^\]]+)\]\((?!https?:)[^)]*\)")
# Los emoji de estado salen a color en Word; en un entregable se leen como símbolos sobrios.
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


def plain(text: str) -> str:
    """Quita de una fuente lo que no existe dentro del Word: enlaces a otros archivos y emoji de estado."""
    text = REPO_LINK.sub(r"\1", text)
    for emoji, simple in EMOJI.items():
        text = text.replace(emoji, simple)
    return text


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
    """Un «Anexo» propio de la fuente pasa a «Apéndice»: dentro del entregable, «Anexo B» es otra cosa."""
    return re.sub(r"^Anexo\b", "Apéndice", text)


def origin(letter: str) -> str:
    """La línea que ata un anexo suelto a su documento principal."""
    return (f"*Anexo {letter} del entregable del tercer objetivo específico del trabajo de grado. "
            f"Documento principal: {MAIN}.docx.*")


def annex(letter: str, source: pathlib.Path) -> str:
    """Un anexo: su título de nivel 1 pasa a «ANEXO X. …»; sus secciones conservan el nivel."""
    titled = False

    def rewrite(level: int, text: str) -> str:
        nonlocal titled
        if level == 1 and not titled:
            titled = True
            title = re.sub(r"^ANEXO [A-Z]\.\s*", "", text, flags=re.I)  # rf_matrix.py ya lo escribe puesto
            return f"# ANEXO {letter}. {title.upper()}\n\n{origin(letter)}"
        return "#" * level + " " + internal(text)

    out = headings(plain(source.read_text(encoding="utf-8")).splitlines(), rewrite)
    if not titled:
        raise SystemExit(f"{source.name}: no tiene título de nivel 1")
    return "\n".join(out)


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


def with_toc(markdown: str) -> str:
    """Tabla de contenido al principio si el documento tiene secciones que listar."""
    sections = []
    headings(markdown.splitlines(), lambda level, text: sections.append(level) or "")
    return f"{toc()}\n\n{PAGE_BREAK}\n\n{markdown}" if sections.count(2) >= 3 else markdown


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


def extract_reference() -> None:
    """Deja en build/ la plantilla de estilos: el .docx del OE2 en el commit 127fbc4."""
    try:
        blob = subprocess.run(["git", "-C", str(ROOT), "show", REFERENCE_SOURCE],
                              capture_output=True, check=True).stdout
    except (subprocess.CalledProcessError, FileNotFoundError) as error:
        raise SystemExit(f"no se pudo extraer {REFERENCE_SOURCE} con git (¿clon incompleto?): {error}")
    if not blob.startswith(b"PK"):
        raise SystemExit(f"{REFERENCE_SOURCE} no es un .docx (¿un puntero de Git LFS?)")
    REFERENCE.write_bytes(blob)


def render(markdown: str, stem: str) -> tuple[pathlib.Path, pathlib.Path]:
    """Markdown -> .docx con la plantilla del OE2 y los estilos que faltan. Devuelve (.docx, .pdf de revisión)."""
    (BUILD / f"{stem}.md").write_text(markdown, encoding="utf-8")
    docx = BUILD / f"{stem}.docx"
    pypandoc.convert_text(
        markdown, "docx", format="markdown-auto_identifiers", outputfile=str(docx),
        extra_args=[f"--reference-doc={REFERENCE}",
                    f"--resource-path={os.pathsep.join(map(str, RESOURCE_PATH))}",
                    "--wrap=none", "--syntax-highlighting=none"])
    add_styles(docx)
    return docx, BUILD / f"{stem}.pdf"


def guards(documents: list[tuple[pathlib.Path, pathlib.Path]]) -> list[str]:
    """Lo que impide publicar, leído del Markdown intermedio (build/*.md), que es lo que llega al Word."""
    problems = []
    built = {docx.stem for docx, _ in documents}
    absent = [letter for letter, suffix, _ in ANNEXES if f"Solucion_OE3_Anexo_{letter}_{suffix}" not in built]
    if absent:
        problems.append(f"no se armaron los anexos {', '.join(absent)}: falta su fuente en src/")
    for stem in sorted(built):
        text = (BUILD / f"{stem}.md").read_text(encoding="utf-8")
        lines = text.splitlines()
        for label, pattern in FORBIDDEN:
            hits = [(number, line, match) for number, line in enumerate(lines, 1)
                    for match in pattern.finditer(line)]
            if hits:
                shown = "; ".join(f"l. {number} «...{line[max(0, match.start() - 25):match.end() + 25]}...»"
                                  for number, line, match in hits[:3])
                problems.append(f"{stem}.md: {len(hits)} veces {label} (p. ej. {shown})")
        for figure in sorted(set(re.findall(r"\]\((fig/[^)\s]+)\)", text))):
            size = max(((base / figure).stat().st_size for base in RESOURCE_PATH if (base / figure).is_file()),
                       default=0)
            if size < 1024:  # un puntero de LFS sin descargar pesa unos cientos de bytes
                problems.append(f"{stem}.md: la figura {figure} no está o no está descargada de LFS")
    main_text = (BUILD / f"{MAIN}.md").read_text(encoding="utf-8")
    for number in CITED_TABLES:
        if not re.search(rf"^\*\*Tabla {re.escape(number)}\.\*\*", main_text, re.M):
            problems.append(f"{MAIN}.md: falta la Tabla {number}, que cita el trabajo de grado")
    for number in CITED_FIGURES:
        if not re.search(rf"!\[Figura {re.escape(number)}\.", main_text):
            problems.append(f"{MAIN}.md: falta la Figura {number}, que cita el trabajo de grado")
    return problems


def finalize(documents: list[tuple[pathlib.Path, pathlib.Path]]) -> None:
    """Un solo Word para todos: tablas, pie con número de página, tabla de contenido y PDF."""
    listing = BUILD / "documentos.txt"
    listing.write_text("\n".join(f"{docx}\t{pdf}" for docx, pdf in documents) + "\n", encoding="utf-8")
    subprocess.run(["powershell.exe", "-NoProfile", "-ExecutionPolicy", "Bypass", "-File",
                    str(OE3 / "tools" / "word_finalize.ps1"), "-List", str(listing)], check=True)


def destination(docx: pathlib.Path) -> pathlib.Path:
    """Dónde se publica un .docx: el principal en docs/, cada anexo en docs/anexos oe3/."""
    return PUBLISHED_MAIN / docx.name if docx.stem == MAIN else PUBLISHED_ANNEXES / docx.name


def publish(documents: list[tuple[pathlib.Path, pathlib.Path]]) -> None:
    """Copia el principal a docs/ y los anexos a docs/anexos oe3/ (que se crea si falta). Antes de copiar
    nada se comprueba que ninguno esté abierto en Word, para no dejar el conjunto a medias."""
    PUBLISHED_MAIN.mkdir(parents=True, exist_ok=True)
    PUBLISHED_ANNEXES.mkdir(parents=True, exist_ok=True)
    for docx, _ in documents:
        target = destination(docx)
        if target.exists():
            try:
                with open(target, "r+b"):
                    pass
            except PermissionError:
                raise SystemExit(f"{target.name} está abierto en otro programa (¿Word?): "
                                 "ciérrelo y repita --publish")
    for docx, _ in documents:
        shutil.copyfile(docx, destination(docx))
        print(f"publicado: {destination(docx)}")


def main(argv: list[str] | None = None) -> int:
    sys.stdout.reconfigure(encoding="utf-8")  # canalizada, Windows la deja en cp1252: acentos rotos
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--no-word", action="store_true", help="salta word_finalize.ps1 (solo para probar el generador)")
    parser.add_argument("--publish", action="store_true", help="si las guardas pasan, copia el principal a docs/ y los anexos a «docs/anexos oe3/»")
    args = parser.parse_args(argv)
    if args.publish and args.no_word:
        parser.error("--publish exige el paso de Word: un .docx sin terminar no se publica")
    missing = [c for c in CHAPTERS if not (SRC / c).exists()]
    if missing:
        raise SystemExit(f"faltan capítulos: {missing}")
    BUILD.mkdir(exist_ok=True)
    # Lo de un armado anterior —anexos con otra letra o nombre— no puede quedar junto a lo nuevo. Solo
    # build/: docs/ y docs/anexos oe3/ tienen los Word versionados y únicamente --publish los toca.
    for stale in (*BUILD.glob("Solucion_OE3_*"), *BUILD.glob("documentos.txt")):
        stale.unlink()
    extract_reference()
    body = with_page_breaks("\n\n".join((SRC / c).read_text(encoding="utf-8") for c in CHAPTERS))
    documents = [render(f"{toc()}\n\n{PAGE_BREAK}\n\n{body}\n", MAIN)]
    for letter, suffix, name in ANNEXES:
        if not (SRC / name).exists():
            print(f"AVISO: falta src/{name}: el Anexo {letter} no se arma")
            continue
        documents.append(render(with_toc(annex(letter, SRC / name)) + "\n", f"Solucion_OE3_Anexo_{letter}_{suffix}"))
    problems = guards(documents)
    for problem in problems:
        print(f"GUARDA: {problem}")
    if args.publish and problems:
        print(f"No se publica: {len(problems)} guardas fallan. No se abrió Word ni se tocó docs/.")
        return 1
    if not args.no_word:
        finalize(documents)
    for docx, _ in documents:
        print(docx)
    if args.publish:
        publish(documents)
    print("guardas: todas pasan" if not problems else f"guardas: {len(problems)} fallan")
    return 1 if problems else 0


if __name__ == "__main__":
    sys.exit(main())
