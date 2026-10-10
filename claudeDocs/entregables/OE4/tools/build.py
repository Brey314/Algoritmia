"""Arma el entregable del OE4 (evaluación funcional del prototipo): el documento principal (capítulos
de src/) y su Anexo A, la matriz de casos PF-* a veredictos.

Uso (desde cualquier carpeta del repositorio):
    python claudeDocs/entregables/OE4/tools/build.py [--no-word] [--publish]

  (sin opciones)  arma los dos .docx en claudeDocs/entregables/OE4/build/ (con el Markdown intermedio y
                  un PDF de revisión por documento) y corre las guardas. No toca docs/.
  --no-word       salta word_finalize.ps1 (tablas sin cuadrícula, sin número de página, sin tabla de
                  contenido ni PDF). Sirve para probar el generador; no se combina con --publish.
  --publish       si las guardas pasan, copia los dos .docx a docs/OE4/. Es lo único que escribe allí.

Lo que no se repite del OE3 se importa de ../OE3/tools/build.py (plantilla de estilos del OE2 en el
commit 127fbc4, estilos de pandoc, tabla de contenido, saltos de página) y de ../OE3/tools/word_finalize.ps1.

Fuentes: src/01-... a 06-*.md (capítulos, en el orden de CHAPTERS) y src/A-matriz-casos.md (Anexo A). Los
marcadores {{TABLA_...}} y {{MATRIZ}} los rellena tools/casos.py desde claudeDocs/tasks/OE4/casos.md y
desde src/veredictos-rc3.csv (caso,veredicto). Una cifra que aún no se midió se escribe PENDIENTE-CIFRA.

Guardas, sobre el Markdown intermedio de build/:
  - se armaron los dos documentos y el número de casos del catálogo coincide con el que declara casos.md y cubre RF-01..47 y RNF-01..23 (CT-10);
  - ningún «§», ningún marcador «{{...}}» sin rellenar ni comentario de trabajo «<!--»;
  - ningún PENDIENTE-CIFRA (cifra sin medir) ni PENDIENTE-PASADA (texto que se redacta tras la pasada): impide publicar. Los «[Pendiente: ...]» visibles, que
    esperan datos de Santiago (estudiantes, hoja HUM), sí se publican; el armado los cuenta.
Sale con 0 si todo está en orden y con 1 si alguna guarda falla (los .docx de build/ quedan armados).

Requiere pypandoc_binary, git y, salvo con --no-word, Microsoft Word sin otra automatización en curso.
"""
import argparse
import importlib.util
import os
import pathlib
import re
import shutil
import subprocess
import sys

sys.dont_write_bytecode = True  # no dejar __pycache__ dentro de OE3/tools al importarlo
import pypandoc

import casos

OE4 = pathlib.Path(__file__).resolve().parents[1]
ROOT = OE4.parents[2]
SRC = OE4 / "src"
BUILD = OE4 / "build"
PUBLISHED = ROOT / "docs" / "OE4"
OE3_TOOLS = OE4.parent / "OE3" / "tools"
MAIN = "Solucion_OE4_Evaluacion_prototipo"
ANNEX = "Anexo_A_Matriz_casos"
CHAPTERS = ["01-introduccion.md", "02-metodo.md", "03-resultados.md", "04-mejoras.md",
            "05-estudiantes.md", "06-conclusiones.md"]
ANNEX_SOURCE = "A-matriz-casos.md"
REFERENCE = BUILD / "ref_oe2_127fbc4.docx"
RESOURCE_PATH = [OE4]

FORBIDDEN = (
    ("«§»", re.compile("§")),
    ("un marcador «{{...}}» sin rellenar", re.compile(r"\{\{")),
    ("un comentario de trabajo «<!--»", re.compile(r"<!--")),
    ("una cifra sin medir (PENDIENTE-CIFRA) o un texto sin redactar (PENDIENTE-PASADA)", re.compile(r"PENDIENTE-(?:CIFRA|PASADA)")),
)
VISIBLE_PENDING = re.compile(r"\[Pendiente:")


def load_oe3():
    spec = importlib.util.spec_from_file_location("oe3_build", OE3_TOOLS / "build.py")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


oe3 = load_oe3()


def extract_reference() -> None:
    try:
        blob = subprocess.run(["git", "-C", str(ROOT), "show", oe3.REFERENCE_SOURCE],
                              capture_output=True, check=True).stdout
    except (subprocess.CalledProcessError, FileNotFoundError) as error:
        raise SystemExit(f"no se pudo extraer {oe3.REFERENCE_SOURCE} con git: {error}")
    if not blob.startswith(b"PK"):
        raise SystemExit(f"{oe3.REFERENCE_SOURCE} no es un .docx (¿un puntero de Git LFS?)")
    REFERENCE.write_bytes(blob)


def render(markdown: str, stem: str):
    (BUILD / f"{stem}.md").write_text(markdown, encoding="utf-8")
    docx = BUILD / f"{stem}.docx"
    pypandoc.convert_text(
        markdown, "docx", format="markdown-auto_identifiers", outputfile=str(docx),
        extra_args=[f"--reference-doc={REFERENCE}",
                    f"--resource-path={os.pathsep.join(map(str, RESOURCE_PATH))}",
                    "--wrap=none", "--syntax-highlighting=none"])
    oe3.add_styles(docx)
    return docx, BUILD / f"{stem}.pdf"


def guards(documents, cases) -> list[str]:
    problems = []
    built = {docx.stem for docx, _ in documents}
    if built != {MAIN, ANNEX}:
        problems.append(f"se esperaban {MAIN} y {ANNEX}; se armaron {sorted(built)}")
    if len(cases) != casos.declared():
        problems.append(f"casos.md trae {len(cases)} filas PF-* y declara {casos.declared()} casos: el catálogo está a medias")
    if missing := casos.coverage(cases):
        problems.append(f"requerimientos sin ningún caso PF-* (CT-10): {', '.join(missing)}")
    visible = 0
    for stem in sorted(built):
        text = (BUILD / f"{stem}.md").read_text(encoding="utf-8")
        visible += len(VISIBLE_PENDING.findall(text))
        lines = text.splitlines()
        for label, pattern in FORBIDDEN:
            hits = [(n, line, m) for n, line in enumerate(lines, 1) for m in pattern.finditer(line)]
            if hits:
                shown = "; ".join(f"l. {n} «...{line[max(0, m.start() - 25):m.end() + 25]}...»"
                                  for n, line, m in hits[:3])
                problems.append(f"{stem}.md: {len(hits)} veces {label} (p. ej. {shown})")
    print(f"AVISO: {visible} «[Pendiente: ...]» visibles esperan datos de Santiago; no impiden publicar")
    return problems


def finalize(documents) -> None:
    listing = BUILD / "documentos.txt"
    listing.write_text("\n".join(f"{docx}\t{pdf}" for docx, pdf in documents) + "\n", encoding="utf-8")
    subprocess.run(["powershell.exe", "-NoProfile", "-ExecutionPolicy", "Bypass", "-File",
                    str(OE3_TOOLS / "word_finalize.ps1"), "-List", str(listing)], check=True)


def publish(documents) -> None:
    PUBLISHED.mkdir(parents=True, exist_ok=True)
    for docx, _ in documents:
        target = PUBLISHED / docx.name
        if target.exists():
            try:
                with open(target, "r+b"):
                    pass
            except PermissionError:
                raise SystemExit(f"{target.name} está abierto en otro programa (¿Word?): ciérrelo y repita --publish")
    for docx, _ in documents:
        shutil.copyfile(docx, PUBLISHED / docx.name)
        print(f"publicado: {PUBLISHED / docx.name}")


def main(argv=None) -> int:
    sys.stdout.reconfigure(encoding="utf-8")
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--no-word", action="store_true", help="salta word_finalize.ps1 (solo para probar el generador)")
    parser.add_argument("--publish", action="store_true", help="si las guardas pasan, copia los .docx a docs/OE4/")
    args = parser.parse_args(argv)
    if args.publish and args.no_word:
        parser.error("--publish exige el paso de Word: un .docx sin terminar no se publica")
    missing = [c for c in [*CHAPTERS, ANNEX_SOURCE] if not (SRC / c).exists()]
    if missing:
        raise SystemExit(f"faltan fuentes en src/: {missing}")
    BUILD.mkdir(exist_ok=True)
    for stale in (*BUILD.glob("Solucion_OE4_*"), *BUILD.glob("Anexo_A_*"), *BUILD.glob("documentos.txt")):
        stale.unlink()
    extract_reference()
    fills, cases = casos.tables()

    def fill(text: str) -> str:
        for marker, value in fills.items():
            text = text.replace(marker, value)
        return text

    body = oe3.with_page_breaks(fill("\n\n".join((SRC / c).read_text(encoding="utf-8") for c in CHAPTERS)))
    documents = [render(f"{oe3.toc()}\n\n{oe3.PAGE_BREAK}\n\n{body}\n", MAIN)]
    annex = oe3.plain(fill((SRC / ANNEX_SOURCE).read_text(encoding="utf-8")))
    documents.append(render(oe3.with_toc(annex) + "\n", ANNEX))
    problems = guards(documents, cases)
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
