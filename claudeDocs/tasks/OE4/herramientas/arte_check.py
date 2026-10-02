#!/usr/bin/env python
"""arte_check.py - verificación reproducible del arte de Algoritmia (W3 / OE4 T17).

Cubre las casillas «Assets visuales» de los Slices 1..3 y D1/D3 del Slice 4: nombre §15.4,
halo de croma, RNF-19 (doble indicador), RNF-20 (contraste) y RNF-21 (destellos).
Requiere Python 3.12 con PIL y numpy; sin red. LEE el repositorio y el directorio de capturas;
lo único que escribe es lo que se le pida con --out / --crops / --backup-dir y, solo con
`halo --apply`, los PNG (ver HALO). Salida: Markdown UTF-8 por stdout (o a --out) para pegar
en los *-Resultados.md, con coma decimal. Códigos de salida: 0 sin hallazgos, 1 hay hallazgos
(sprites, rnf19, rnf20, rnf21), 2 uso o archivo erróneo. `halo` informa y termina en 0.

USO
  python arte_check.py sprites [--only-fail] [--no-table] [--out F.md] [--repo DIR]
                               [--max-alpha 64] [--excess 40] [--strong 130] [--min-px 10]
  python arte_check.py rnf19   [--spec "etiqueta ; A ; B"]... [--file PAREJAS.txt] [A B]
  python arte_check.py rnf20   [--spec "etiqueta ; captura ; x0,y0,x1,y1"]... [--file CAJAS.txt]
                               [--crops DIR] [--p-text 1] [--p-bg 60] [--min 4.5]
  python arte_check.py rnf21   RUTA... [--fps 30] [--mtime] [--grid 3] [--bg "#000000"]
                               [--delta 0.10] [--dark 0.80] [--max-flashes 3]
  python arte_check.py halo    [RUTA...] [--dry-run] [--max-alpha 64] [--apply --backup-dir DIR]
  python arte_check.py --self-test

SPRITES  Recorre Assets/Game/Art/**/*.png salvo los cuadros fuego/humo con nombre de entrega
  (fuego_*_nivel_N_NNNN.png, humo_nivel_N_NNNN.png; viven en .anim y no se renombran).
  Por archivo: nombre §15.4 ([categoría]_[sujeto]_..., prefijos char_ prop_ env_ ui_ fx_ ref_,
  minúsculas, sin tildes ni espacios), canal alfa (todo salvo lo de Environments/ debe tener
  transparencia: >= 1 % de píxeles no opacos), halo (ver HALO), borde tocado (algún píxel con
  α>=32 en el borde del lienzo; informativo, no se evalúa en Environments/),
  dimensiones y .meta: spriteMode, PPU, filtro (esperado Single / 100 / Bilinear), más la
  forma física, el tamaño máximo y la compresión (informativos).

HALO  Un píxel es halo si 0<α<64, su tono es verde o magenta (exceso >= 40: G-máx(R,B) para el
  verde, mín(R,B)-G para el magenta) y su entorno no es verde/magenta de verdad: entre sus
  vecinos casi opacos (α>=250, ventana 7x7) la mayoría NO tiene ese tinte. Así no cuenta el
  verde legítimo de una planta ni de la franja arcoíris. El contorno desmanchado queda con la
  media de los vecinos opacos sin tinte. El verde del borde suele ser un anillo de 2-3 px: α<64
  es solo la capa exterior; la columna «64≤α<200» de `halo` cuenta lo que seguiría verde dentro.
  Su «nivel» es el canal que delata el croma (G, o mín(R,B) en magenta): >= 130 es INTENSO (el
  croma casi puro de Algoritm y de los retratos); por debajo es TENUE (el verde oscuro del borde
  de las partes de los personajes, que solo se nota sobre fondo claro). Un archivo cuenta como
  halo si tiene >= 10 píxeles intensos (--strong, --min-px); lo tenue y los píxeles sueltos se
  informan sin fallar. `halo` neutraliza poniendo el RGB a la media de esos vecinos (o a su
  gris si no hay); el alfa y todo píxel que no sea halo quedan idénticos. Por defecto es
  --dry-run (solo informe). --apply exige --backup-dir (copia del original, FUERA del repo) y
  rutas explícitas (nunca actúa sobre todo Art/); solo toca PNG RGBA de 8 bits, conserva el
  nombre y los chunks auxiliares (iCCP, gAMA, pHYs, tEXt...), NO toca el .meta, y tras escribir
  relee el archivo y comprueba que solo cambió el halo.

RNF19  Parejas a comparar en gris (Rec. 601 = Color.grayscale de Unity, sobre gris medio 128).
  Se distinguen si 1-IoU de la silueta (α>=128) >= 0,20 O la diferencia media de gris por
  píxel (sobre la unión de lo visible) >= 25 de 255. Cada imagen se lleva a un lienzo de
  256x256: con alfa, encajada y centrada; opaca (captura), estirada. IoU solo existe si las
  dos tienen transparencia. Una «imagen» es `ruta[@x0,y0,x1,y1]`: la caja recorta una captura
  en coordenadas normalizadas 0..1. Las rutas se buscan en cwd, en Assets/Game/Art, en el
  directorio de capturas y, si solo traen nombre, por nombre bajo Art/.
  Línea de --spec / --file:  etiqueta ; A ; B           (una pareja)
                             etiqueta ; A ; B ; C ; D   (todas las parejas del grupo)
  Líneas vacías o que empiecen por # se ignoran. Un cambio localizado (un candado, un visto)
  da diferencia media baja: por eso se informa también el % de píxeles que difieren >= 25.

RNF20  Contraste WCAG 2.x (luminancia relativa, 0,2126/0,7152/0,0722) en una caja de texto de
  una captura, coordenadas normalizadas: percentil --p-text (1) del texto contra percentil
  --p-bg (60) del fondo (polaridad oscuro/claro detectada; en claro/oscuro se usan los
  percentiles espejo). Mínimo 4,5 (RNF-20). Línea:  etiqueta ; captura ; x0,y0,x1,y1 [; mín]
  o con colores dados:  etiqueta ; #texto ; #fondo [; mín]. --crops DIR guarda cada recorte
  ampliado x2 para comprobar a ojo que la caja cae sobre el texto.

RNF21  Luminancia relativa media por cuadro (y por baldosa --grid NxN, que aproxima la ventana
  de 10 grados de WCAG) de una ráfaga de capturas o de una secuencia de PNG (carpeta, comodín
  o archivos; cada carpeta/comodín es una secuencia, los archivos sueltos forman otra). Tiempo
  = número final del nombre / --fps (en los cuadros de entrega es el cuadro a 30 fps; las
  claves del .anim son (n - primer cuadro)/30, comprobado en fx_n1_humo_nacer y prop_n1_fuego_normal,
  y un desplazamiento constante no cambia los destellos; con milisegundos usar --fps 1000), o el
  índice si no hay número; --mtime usa la fecha de modificación. Destello según WCAG 2.3.1: tramo extremo a
  extremo con cambio de luminancia >= 10 % cuyo lado oscuro es < 0,8; dos tramos opuestos son
  un destello; falla si hay más de 3 en cualquier segundo. Los PNG con alfa se componen sobre --bg.

EJEMPLOS
  python arte_check.py sprites --out informe-sprites.md
  python arte_check.py rnf19 --file parejas.txt
  python arte_check.py rnf20 --spec "Jugar ; #3A1E18 ; #E8A33D"
  python arte_check.py rnf21 "Assets/Game/Art/Props/Fire/Animations/Fuego normal" --fps 30
  python arte_check.py halo --apply --backup-dir C:/Temp/respaldo_halo Characters/Girl
"""
from __future__ import annotations

import argparse
import collections
import contextlib
import datetime
import glob
import hashlib
import io
import itertools
import os
import re
import shlex
import sys
import tempfile
import unicodedata
from pathlib import Path

import numpy as np
from PIL import Image


def find_repo() -> Path:
    for d in Path(__file__).resolve().parents:
        if (d / "Assets" / "Game" / "Art").is_dir():
            return d
    return Path(os.environ.get("ALGORITMIA_REPO", r"C:\Dev\Algoritmia"))


REPO = find_repo()
CAPTURES = Path.home() / "AppData/LocalLow/Universidad Catolica de Colombia/Algoritmia/TestScreenshots"
PREFIXES = ("char", "prop", "env", "ui", "fx", "ref")
FRAME_RE = re.compile(r"^(fuego|humo)_[a-z_]*nivel_\d+_\d+\.png$")  # cuadros de entrega del fuego y el humo
GRAY = np.array([0.299, 0.587, 0.114])  # Rec. 601: Color.grayscale de Unity y PIL «L»
OPAQUE = 250  # α desde el que un vecino cuenta como opaco al calcular el color de contorno
ARGV: list = []  # argumentos de la ejecución, para dejar el comando en cada informe
_c = np.arange(256) / 255
LIN = np.where(_c <= 0.03928, _c / 12.92, ((_c + 0.055) / 1.055) ** 2.4)  # WCAG 2.x, como las pruebas del proyecto


# ---------------------------------------------------------------- utilidades
def die(msg: str):
    print(f"error: {msg}", file=sys.stderr)
    sys.exit(2)


def n(x, d=1) -> str:
    """Número con coma decimal: los *-Resultados.md van en español."""
    return f"{x:.{d}f}".replace(".", ",")


def repro() -> str:
    """Línea de cabecera de cada informe: cómo se generó (reproducibilidad)."""
    return "- Comando: `arte_check.py " + " ".join(shlex.quote(str(x)) for x in ARGV) + "`"


def md_table(head, rows) -> str:
    out = ["| " + " | ".join(head) + " |", "|" + "|".join("---" for _ in head) + "|"]
    out += ["| " + " | ".join(str(c) for c in r) + " |" for r in rows]
    return "\n".join(out)


def luminance(rgb):
    """(...,3) enteros 0-255 -> luminancia relativa WCAG 0-1."""
    rgb = np.asarray(rgb, dtype=np.intp)
    return LIN[rgb[..., 0]] * 0.2126 + LIN[rgb[..., 1]] * 0.7152 + LIN[rgb[..., 2]] * 0.0722


def contrast(l1, l2) -> float:
    return (max(l1, l2) + 0.05) / (min(l1, l2) + 0.05)


def hex_rgb(s: str):
    s = s.strip().lstrip("#")
    return tuple(int(s[i:i + 2], 16) for i in (0, 2, 4))


def rgb_hex(t) -> str:
    return "#%02X%02X%02X" % tuple(int(v) for v in t)


def load_rgba(p) -> np.ndarray:
    return np.asarray(Image.open(p).convert("RGBA"))


def resolve(spec: str, art: Path) -> Path:
    p = Path(spec)
    if p.is_absolute() and p.is_file():
        return p
    for base in (Path.cwd(), art, CAPTURES):
        if (base / p).is_file():
            return base / p
    if len(p.parts) == 1:  # solo el nombre: búsqueda por nombre bajo Art/
        hits = list(art.rglob(p.name))
        if len(hits) == 1:
            return hits[0]
    die(f"no encuentro «{spec}» (probé cwd, {art} y {CAPTURES})")


def parse_box(s: str):
    try:
        x0, y0, x1, y1 = (float(v) for v in s.split(","))
    except ValueError:
        die(f"caja «{s}»: se esperan cuatro números x0,y0,x1,y1 normalizados")
    if not (0 <= x0 < x1 <= 1 and 0 <= y0 < y1 <= 1):
        die(f"caja «{s}»: debe cumplir 0 <= x0 < x1 <= 1 y 0 <= y0 < y1 <= 1")
    return x0, y0, x1, y1


def crop_box(a: np.ndarray, box) -> np.ndarray:
    h, w = a.shape[:2]
    x0, y0, x1, y1 = box
    return a[int(y0 * h):max(int(y1 * h), int(y0 * h) + 1), int(x0 * w):max(int(x1 * w), int(x0 * w) + 1)]


def spec_lines(specs, file):
    """Líneas `a ; b ; c` de --spec y de --file -> listas de campos."""
    lines = list(specs or [])
    if file:
        lines += Path(file).read_text(encoding="utf-8").splitlines()
    out = []
    for ln in lines:
        ln = ln.strip()
        if ln and not ln.startswith("#"):
            out.append([f.strip() for f in ln.split(";")])
    return out


def sprite_targets(paths, art: Path):
    """Archivos PNG de los argumentos (o todos los de Art/ si no hay), sin cuadros de entrega."""
    if not paths:
        found = sorted(art.rglob("*.png"))
    else:
        found = []
        for s in paths:
            p = Path(s) if Path(s).exists() else art / s
            if p.is_dir():
                found += sorted(p.rglob("*.png"))
            elif p.is_file():
                found.append(p)
            else:
                die(f"no encuentro «{s}»")
    return [p for p in found if not FRAME_RE.match(p.name)]


# ---------------------------------------------------------------- nombre y .meta
def name_problems(name: str):
    stem = Path(name).stem
    out = []
    if not stem.isascii():
        out.append("tildes o caracteres no ASCII")
    if " " in stem:
        out.append("espacios")
    if stem != stem.lower():
        out.append("mayúsculas")
    pre = stem.split("_")[0]
    if pre.lower() not in PREFIXES:
        out.append(f"prefijo «{pre}_» (debe ser {'/'.join(x + '_' for x in PREFIXES)})")
    if stem.isascii() and re.search(r"[^A-Za-z0-9_ ]", stem):
        out.append("caracteres fuera de a-z 0-9 _")
    return out


def read_meta(png: Path):
    f = Path(str(png) + ".meta")
    if not f.exists():
        return None
    s = f.read_text(encoding="utf-8", errors="replace")

    def key(k, src=s):
        m = re.search(rf"^\s+{k}: (\S+)", src, re.M)
        return m.group(1) if m else "?"

    blk = re.search(r"buildTarget: DefaultTexturePlatform\n((?:    .*\n)+)", s)
    b = blk.group(1) if blk else ""
    return dict(
        type=key("textureType"),
        mode={"0": "None", "1": "Single", "2": "Multiple"}.get(key("spriteMode"), "?"),
        ppu=key("spritePixelsToUnits"),
        filt={"0": "Point", "1": "Bilinear", "2": "Trilinear"}.get(key("filterMode"), "?"),
        phys=key("spriteGenerateFallbackPhysicsShape"),
        maxtex=key("maxTextureSize", b),
        comp={"0": "sin comprimir", "1": "normal", "2": "alta", "3": "baja"}.get(key("textureCompression", b), "?"),
    )


# ---------------------------------------------------------------- halo
def find_halo(arr: np.ndarray, max_alpha=64, excess=40, radius=3):
    """-> (ys, xs, rgb_nuevo, nivel) de los píxeles halo; ver HALO en la cabecera.
    nivel = canal que delata el croma (G en verde, mín(R,B) en magenta): 255 croma puro, <130 verde oscuro."""
    rgb = arr[..., :3].astype(np.int16)
    al = arr[..., 3]
    g = rgb[..., 1] - np.maximum(rgb[..., 0], rgb[..., 2])  # exceso de verde
    m = np.minimum(rgb[..., 0], rgb[..., 2]) - rgb[..., 1]  # exceso de magenta
    ys, xs = np.nonzero((al > 0) & (al < max_alpha) & ((g >= excess) | (m >= excess)))
    if not len(ys):
        return ys, xs, np.zeros((0, 3), np.uint8), np.zeros(0, np.int16)
    r = radius
    pad = np.pad(arr, ((r, r), (r, r), (0, 0)))
    win = np.stack([pad[ys + r + dy, xs + r + dx] for dy in range(-r, r + 1) for dx in range(-r, r + 1) if dy or dx], axis=1)
    w = win[..., :3].astype(np.int16)
    tint = (w[..., 1] - np.maximum(w[..., 0], w[..., 2]) >= excess) | (np.minimum(w[..., 0], w[..., 2]) - w[..., 1] >= excess)
    op = win[..., 3] >= OPAQUE
    good = op & ~tint  # vecinos casi opacos sin tinte: de ahí sale el color de contorno
    n_op, n_good = op.sum(1), good.sum(1)
    legit = (n_op > 0) & (n_good * 2 < n_op)  # el entorno opaco es verde/magenta de verdad
    mean = (win[..., :3] * good[..., None]).sum(1) / np.maximum(n_good, 1)[:, None]
    own_gray = (rgb[ys, xs] @ GRAY)[:, None] * np.ones(3)
    new = np.where(n_good[:, None] > 0, mean, own_gray)
    level = np.where(g[ys, xs] >= excess, rgb[ys, xs, 1], np.minimum(rgb[ys, xs, 0], rgb[ys, xs, 2]))
    k = ~legit
    return ys[k], xs[k], np.clip(np.rint(new[k]), 0, 255).astype(np.uint8), level[k]


def png_chunks(data: bytes):
    i, out = 8, []
    while i < len(data):
        ln = int.from_bytes(data[i:i + 4], "big")
        out.append((data[i + 4:i + 8], data[i:i + 12 + ln]))
        i += 12 + ln
    return out


def rewrite_png(orig: bytes, rgba: np.ndarray) -> bytes:
    """PNG nuevo con los píxeles de `rgba` y los chunks auxiliares del original (iCCP, gAMA, pHYs, tEXt...)."""
    buf = io.BytesIO()
    Image.fromarray(rgba).save(buf, "PNG")
    new, old = png_chunks(buf.getvalue()), png_chunks(orig)
    aux = [c for t, c in old if t not in (b"IHDR", b"PLTE", b"IDAT", b"IEND", b"tRNS")]
    return b"\x89PNG\r\n\x1a\n" + b"".join([c for t, c in new if t == b"IHDR"] + aux + [c for t, c in new if t != b"IHDR"])


def apply_halo(path: Path, arr, ys, xs, new, backup_dir: Path, art: Path):
    """Escribe el PNG desmanchado. No toca el .meta ni el nombre. Verifica en memoria ANTES de escribir
    y deja respaldo del original (nunca pisa un respaldo que ya exista). Todo rechazo es ValueError."""
    orig = path.read_bytes()
    if not (orig[24] == 8 and orig[25] == 6):
        raise ValueError("no es RGBA de 8 bits: no se toca")
    try:
        rel = path.relative_to(art)
    except ValueError:
        rel = Path(path.name)
    dst = backup_dir / rel
    if dst.exists():
        raise ValueError(f"ya existe un respaldo en {dst}: usa otra carpeta de respaldo")
    out = arr.copy()
    out[ys, xs, :3] = new
    data = rewrite_png(orig, out)
    chk = np.asarray(Image.open(io.BytesIO(data)).convert("RGBA"))
    rest = np.ones(arr.shape[:2], bool)
    rest[ys, xs] = False
    if not ((chk[..., 3] == arr[..., 3]).all() and (chk[rest] == arr[rest]).all() and (chk[ys, xs, :3] == new).all()):
        raise ValueError("la verificación en memoria no coincide con lo calculado: no se escribió nada")
    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_bytes(orig)
    path.write_bytes(data)


# ---------------------------------------------------------------- sprites
def inspect_sprite(p: Path, art: Path, a) -> dict:
    im = Image.open(p)
    arr = np.asarray(im.convert("RGBA"))
    al = arr[..., 3]
    rel = p.relative_to(art).as_posix()
    env = rel.startswith("Environments/")
    has_t = float((al < 255).mean()) >= 0.01  # transparencia de verdad: >= 1 % de píxeles no opacos
    vis = al >= 32
    sides = [s for s, v in (("izq", vis[:, 0]), ("der", vis[:, -1]), ("arr", vis[0]), ("abj", vis[-1])) if v.any()]
    ys, xs, _, lvl = find_halo(arr, a.max_alpha, a.excess, a.radius)
    return dict(
        rel=rel, size=im.size, mode=im.mode, has_t=has_t, env=env,
        transp=100 * float((al == 0).mean()), sides=sides if has_t and not env else None,
        halo=len(ys), halo_strong=int((lvl >= a.strong).sum()), halo_max=int(al[ys, xs].max()) if len(ys) else 0,
        meta=read_meta(p), names=name_problems(p.name),
    )


def meta_ok(m) -> bool:
    return bool(m) and m["type"] == "8" and m["mode"] == "Single" and m["ppu"] == "100" and m["filt"] == "Bilinear"


def cmd_sprites(a) -> int:
    repo = Path(a.repo)
    art = repo / "Assets/Game/Art"
    allp = sorted(art.rglob("*.png"))
    items = [inspect_sprite(p, art, a) for p in allp if not FRAME_RE.match(p.name)]
    skipped = len(allp) - len(items)
    bad_name = [i for i in items if i["names"]]
    no_alpha = [i for i in items if not i["env"] and not i["has_t"]]
    halo = [i for i in items if i["halo_strong"] >= a.min_px]
    faint = [i for i in items if i["halo"] and i not in halo]
    border = [i for i in items if i["sides"]]
    no_meta = [i for i in items if not i["meta"]]
    bad_meta = [i for i in items if i["meta"] and not meta_ok(i["meta"])]
    phys = [i for i in items if i["meta"] and i["meta"]["phys"] == "1"]
    comp = collections.Counter((i["meta"]["comp"], i["meta"]["maxtex"]) for i in items if i["meta"])
    findings = len(bad_name) + len(no_alpha) + len(halo) + len(no_meta) + len(bad_meta)

    print("# Verificación de sprites (`arte_check.py sprites`)\n")
    print(repro())
    print(f"- Carpeta: `{art.as_posix()}` · fecha: {datetime.date.today():%d/%m/%Y}")
    print(f"- PNG en disco: {len(allp)} · excluidos (cuadros fuego/humo con nombre de entrega): {skipped} · analizados: {len(items)}")
    print(f"- Halo = píxeles 0<α<{a.max_alpha} de tono verde/magenta (exceso ≥ {a.excess}) cuyos vecinos opacos no comparten el tinte; "
          f"intenso = canal dominante ≥ {a.strong}. Borde tocado = algún píxel con α≥32 en el borde del lienzo (informativo). .meta esperado = Sprite · Single · 100 PPU · Bilinear.\n")
    print("## Resumen\n")
    print(md_table(["Comprobación", "Resultado"], [
        ["Nombre §15.4", f"{len(items) - len(bad_name)} cumplen · **{len(bad_name)} fallan**"],
        ["Canal alfa (todo salvo `Environments/` debe tener transparencia)", f"{len(items) - len(no_alpha)} cumplen · **{len(no_alpha)} sin transparencia**"],
        [f"Halo de croma INTENSO (nivel ≥ {a.strong}, ≥ {a.min_px} px)", f"**{len(halo)} archivos** · {sum(i['halo_strong'] for i in halo)} px intensos de {sum(i['halo'] for i in halo)} marcados"],
        ["Halo TENUE o suelto (verde oscuro o pocos píxeles; informativo)", f"{len(faint)} archivos · {sum(i['halo'] for i in faint)} px"],
        ["Borde tocado", f"{len(border)} archivos (informativo)"],
        [".meta", f"{len(items) - len(bad_meta) - len(no_meta)} como se espera · **{len(bad_meta)} desviados** · {len(no_meta)} sin .meta"],
        ["`Generate Physics Shape` encendido (§15.2 pide apagado; informativo)", f"{len(phys)} de {len(items) - len(no_meta)}"],
        ["Compresión / tamaño máximo de importación", ", ".join(f"{k[0]} / {k[1]}: {v}" for k, v in sorted(comp.items()))],
    ]))

    def lst(title, rows):
        if rows:
            print(f"\n### {title} ({len(rows)})\n")
            print("\n".join(rows))

    print("\n## Hallazgos")
    lst("Nombre fuera de §15.4", [f"- `{i['rel']}`: {'; '.join(i['names'])}" for i in bad_name])
    lst("Sin transparencia", [f"- `{i['rel']}` ({i['mode']})" for i in no_alpha])
    lst("Halo de croma intenso", [f"- `{i['rel']}`: {i['halo_strong']} px intensos de {i['halo']} marcados (α máx {i['halo_max']})" for i in sorted(halo, key=lambda i: -i['halo_strong'])])
    lst("Halo tenue o suelto (informativo)", [f"- `{i['rel']}`: {i['halo']} px, {i['halo_strong']} intensos (α máx {i['halo_max']})" for i in sorted(faint, key=lambda i: -i['halo'])])
    lst(".meta desviado", [f"- `{i['rel']}`: tipo {i['meta']['type']} · {i['meta']['mode']} · {i['meta']['ppu']} PPU · {i['meta']['filt']}" for i in bad_meta])
    lst("Sin .meta", [f"- `{i['rel']}`" for i in no_meta])
    lst("Borde tocado (informativo)", [f"- `{i['rel']}`: {', '.join(i['sides'])}" for i in border])

    if not a.no_table:
        print("\n## Tabla\n")
        rows = []
        for i in items:
            ok = not (i["names"] or (not i["env"] and not i["has_t"]) or i in halo or not meta_ok(i["meta"]))
            if a.only_fail and ok:
                continue
            m = i["meta"]
            rows.append([
                f"`{i['rel']}`", f"{i['size'][0]}×{i['size'][1]}",
                "OK" if not i["names"] else "FALLA: " + "; ".join(i["names"]),
                f"{i['mode']} · {n(i['transp'], 0)} % transp." if i["has_t"] else f"{i['mode']} · opaco" + (" (env)" if i["env"] else " **FALTA alfa**"),
                f"{i['halo']} ({i['halo_strong']} int., α≤{i['halo_max']})" if i["halo"] else "0",
                ", ".join(i["sides"]) if i["sides"] else ("—" if i["has_t"] else "n/a"),
                "sin .meta" if not m else ("ok" if meta_ok(m) else f"{m['type']}/{m['mode']}/{m['ppu']}/{m['filt']}"),
            ])
        print(md_table(["Archivo", "px", "Nombre", "Alfa", "Halo", "Borde", ".meta"], rows))
    return 1 if findings else 0


# ---------------------------------------------------------------- halo (informe / aplicación)
def cmd_halo(a) -> int:
    if a.apply and a.dry_run:
        die("--apply y --dry-run son excluyentes")
    if a.apply and not (a.backup_dir and a.paths):
        die("--apply exige --backup-dir y las rutas a tocar (no actúa sobre todo Art/)")
    repo = Path(a.repo)
    art = repo / "Assets/Game/Art"
    backup = Path(a.backup_dir).resolve() if a.backup_dir else None
    if backup and repo.resolve() in (backup, *backup.parents):
        die("--backup-dir debe estar fuera del repositorio")
    files = sprite_targets(a.paths, art)
    main_rows, faint_rows, clean, done = [], [], 0, []
    for p in files:
        arr = load_rgba(p)
        ys, xs, new, lvl = find_halo(arr, a.max_alpha, a.excess, a.radius)
        if not len(ys):
            clean += 1
            continue
        strong = int((lvl >= a.strong).sum())
        state = "—"
        if a.apply:
            try:
                apply_halo(p, arr, ys, xs, new, backup, art)
                done.append(p.name)
                state = "escrito"
            except ValueError as e:
                state = f"OMITIDO: {e}"
        inner = len(find_halo(arr, 200, a.excess, a.radius)[0]) - len(ys) if a.max_alpha < 200 else 0
        row = [f"`{p.relative_to(art).as_posix() if art in p.parents else p}`", len(ys), strong, int(arr[ys, xs, 3].max()), inner,
               f"{rgb_hex(arr[ys, xs, :3].mean(0))} → {rgb_hex(new.mean(0))}", state]
        (main_rows if strong >= a.min_px else faint_rows).append(row)
    head = ["Archivo", "Píxeles", "Intensos", "α máx", f"Más con {a.max_alpha}≤α<200", "Color medio antes → después", "Estado"]
    print(f"# Halo de croma (`arte_check.py halo` · {'APLICADO' if a.apply else 'solo informe, --dry-run'})\n")
    print(repro())
    print(f"- Regla: 0<α<{a.max_alpha}, exceso verde/magenta ≥ {a.excess}, vecinos opacos en ventana {2 * a.radius + 1}×{2 * a.radius + 1}; "
          "se neutraliza el RGB (media de vecinos opacos, o gris propio si no hay). El alfa y los demás píxeles no cambian.")
    print(f"- Intenso = canal dominante ≥ {a.strong} (croma casi puro); tenue = verde oscuro; suelto = menos de {a.min_px} intensos. "
          f"«Más con {a.max_alpha}≤α<200» = píxeles verdes del anillo interior que NO se tocarían con este alcance (`--max-alpha 200` los incluye).")
    print(f"- Revisados: {len(files)} · con halo intenso: {len(main_rows)} ({sum(r[1] for r in main_rows)} px) · "
          f"solo tenue o suelto: {len(faint_rows)} ({sum(r[1] for r in faint_rows)} px) · sin halo: {clean}\n")
    if main_rows:
        print("## Con halo intenso (el alcance del desmanchado)\n")
        print(md_table(head, main_rows))
    if faint_rows:
        print("\n## Solo halo tenue o suelto (informativo; solo se tocaría si se nombra el archivo con --apply)\n")
        print(md_table(head, faint_rows))
    if a.apply:
        print(f"\nEscritos {len(done)} archivos; respaldo en `{backup}`. Los .meta no se tocaron: Unity reimporta al enfocar el Editor.")
    else:
        print("\nNo se escribió nada. Para aplicar: `halo --apply --backup-dir DIR RUTA...` (rutas explícitas; el respaldo, fuera del repo).")
    return 0


# ---------------------------------------------------------------- RNF-19
def canvas(a: np.ndarray, size=256) -> np.ndarray:
    # ponytail: lienzo fijo de 256² y las capturas se estiran; si hiciera falta comparar escala o proporción, recortar al bbox
    im = Image.fromarray(a)
    if a[..., 3].min() == 255:  # captura u opaca: se estira
        return np.asarray(im.resize((size, size), Image.Resampling.BILINEAR))
    s = size / max(im.size)
    im = im.resize((max(1, round(im.width * s)), max(1, round(im.height * s))), Image.Resampling.BILINEAR)
    out = Image.new("RGBA", (size, size))
    out.paste(im, ((size - im.width) // 2, (size - im.height) // 2))
    return np.asarray(out)


def gray(c: np.ndarray) -> np.ndarray:
    al = c[..., 3:] / 255.0
    return (c[..., :3] * al + 128 * (1 - al)) @ GRAY


def pair_metrics(a: np.ndarray, b: np.ndarray):
    """-> (IoU o None, diferencia media de gris, % de píxeles con diferencia >= 25)."""
    ca, cb = canvas(a), canvas(b)
    d = np.abs(gray(ca) - gray(cb))
    u = (ca[..., 3] >= 32) | (cb[..., 3] >= 32)
    mae = float(d[u].mean()) if u.any() else 0.0
    big = float((d[u] >= 25).mean() * 100) if u.any() else 0.0
    iou = None
    if a[..., 3].min() < 255 and b[..., 3].min() < 255:
        sa, sb = ca[..., 3] >= 128, cb[..., 3] >= 128
        iou = float((sa & sb).sum() / max((sa | sb).sum(), 1))
    return iou, mae, big


def verdict19(iou, mae, shape_min=0.2, gray_min=25.0):
    by_shape = iou is not None and 1 - iou >= shape_min
    by_gray = mae >= gray_min
    if by_shape and by_gray:
        return "OK (forma y gris)"
    return "OK (forma)" if by_shape else "OK (gris)" if by_gray else "**FALLA**"


def load_ref(spec: str, art: Path):
    path, _, box = spec.partition("@")
    arr = load_rgba(resolve(path, art))
    return crop_box(arr, parse_box(box)) if box else arr


def cmd_rnf19(a) -> int:
    art = Path(a.repo) / "Assets/Game/Art"
    lines = spec_lines(a.spec, a.file)
    if a.images:
        lines.append(["CLI"] + a.images)
    if not lines:
        die("rnf19 necesita --spec, --file o dos imágenes")
    rows, fails = [], 0
    cache = {}
    for ln in lines:
        label, imgs = ln[0], ln[1:]
        if len(imgs) < 2:
            die(f"línea «{label}»: se necesitan al menos dos imágenes")
        for s in imgs:
            if s not in cache:
                cache[s] = load_ref(s, art)
        for x, y in itertools.combinations(imgs, 2):
            iou, mae, big = pair_metrics(cache[x], cache[y])
            v = verdict19(iou, mae)
            fails += v.startswith("**")
            rows.append([label, f"`{x}`", f"`{y}`", n(1 - iou, 2) if iou is not None else "n/d", n(mae, 1), n(big, 1) + " %", v])
    print("# RNF-19: parejas en escala de grises (`arte_check.py rnf19`)\n")
    print(repro())
    print("- Criterio: se distinguen si **1−IoU de la silueta ≥ 0,20** (forma) o **diferencia media de gris ≥ 25** de 255 (brillo). "
          "Gris Rec. 601 sobre gris medio; lienzo de 256×256 (con alfa, encajado; captura, estirada). "
          "«n/d»: sin transparencia en alguna de las dos, solo cuenta el gris. La columna «Δ ≥ 25» es informativa: "
          "un cambio pequeño y localizado (candado, visto) da diferencia media baja aunque se vea.\n")
    print(md_table(["Grupo", "A", "B", "1−IoU", "Δ gris medio", "Δ ≥ 25", "Veredicto"], rows))
    print(f"\n**{len(rows)} parejas · {len(rows) - fails} se distinguen · {fails} no cumplen el umbral.**")
    return 1 if fails else 0


# ---------------------------------------------------------------- RNF-20
def box_contrast(a: np.ndarray, box, p_text=1.0, p_bg=60.0, polarity="auto"):
    """-> (contraste, RGB texto, RGB fondo, polaridad) dentro de la caja normalizada."""
    px = crop_box(a, box)[..., :3].reshape(-1, 3)
    lum = luminance(px)
    order = np.argsort(lum, kind="stable")

    def at(p):
        return int(order[min(len(order) - 1, int(round(p / 100 * (len(order) - 1))))])

    lo, med, hi = np.percentile(lum, [1, 50, 99])
    dark_on_light = polarity == "dark-on-light" or (polarity == "auto" and med - lo >= hi - med)
    t, b = (at(p_text), at(p_bg)) if dark_on_light else (at(100 - p_text), at(100 - p_bg))
    return contrast(lum[t], lum[b]), tuple(px[t]), tuple(px[b]), "oscuro/claro" if dark_on_light else "claro/oscuro"


def cmd_rnf20(a) -> int:
    art = Path(a.repo) / "Assets/Game/Art"
    lines = spec_lines(a.spec, a.file)
    if not lines:
        die("rnf20 necesita --spec o --file")
    crops = Path(a.crops) if a.crops else None
    if crops:
        crops.mkdir(parents=True, exist_ok=True)
    rows, fails, cache = [], 0, {}
    for k, ln in enumerate(lines):
        if len(ln) < 3:
            die(f"línea «{ln[0]}»: formato `etiqueta ; captura ; x0,y0,x1,y1` o `etiqueta ; #texto ; #fondo`")
        label, f1, f2 = ln[:3]
        minimum = float(ln[3].replace(",", ".")) if len(ln) > 3 and ln[3] else a.min
        if f1.startswith("#"):
            c1, c2 = hex_rgb(f1), hex_rgb(f2)
            ratio, tx, bg, pol, where = contrast(luminance(c1), luminance(c2)), c1, c2, "dado", "colores dados"
        else:
            if f1 not in cache:
                cache[f1] = Image.open(resolve(f1, art)).convert("RGB")
            box = parse_box(f2)
            arr = np.asarray(cache[f1])
            ratio, tx, bg, pol = box_contrast(arr, box, a.p_text, a.p_bg, a.polarity)
            where = f"`{f1}` @ {f2}"
            if crops:
                c = Image.fromarray(np.ascontiguousarray(crop_box(arr, box)))
                slug = re.sub(r"[^A-Za-z0-9]+", "_", unicodedata.normalize("NFKD", label).encode("ascii", "ignore").decode())[:40]
                c.resize((c.width * 2, c.height * 2), Image.Resampling.NEAREST).save(crops / f"{k:02d}_{slug}.png")
        ok = ratio >= minimum
        fails += not ok
        rows.append([label, where, pol, rgb_hex(tx), rgb_hex(bg), n(ratio, 1) + ":1", n(minimum, 1) + ":1", "OK" if ok else "**FALLA**"])
    print("# RNF-20: contraste texto/fondo (`arte_check.py rnf20`)\n")
    print(repro())
    print(f"- Contraste WCAG 2.x: percentil {n(a.p_text, 0)} de la luminancia del texto contra percentil {n(a.p_bg, 0)} del fondo dentro de la caja "
          "(coordenadas normalizadas; en claro/oscuro se usan los percentiles espejo). Mínimo RNF-20 = 4,5:1. "
          "Los colores son los del píxel que cae en cada percentil.\n")
    print(md_table(["Región", "Captura y caja", "Polaridad", "Texto", "Fondo", "Contraste", "Mínimo", "Veredicto"], rows))
    print(f"\n**{len(rows)} regiones · {len(rows) - fails} cumplen · {fails} no cumplen.**")
    return 1 if fails else 0


# ---------------------------------------------------------------- RNF-21
def zigzag(v, delta):
    """Índices de los extremos: la serie revierte >= delta entre extremos consecutivos."""
    piv, up, ext = [0], None, 0
    for i in range(1, len(v)):
        if up is None:
            if abs(v[i] - v[0]) >= delta:
                up, ext = v[i] > v[0], i
        elif up:
            if v[i] > v[ext]:
                ext = i
            elif v[ext] - v[i] >= delta:
                piv.append(ext)
                up, ext = False, i
        else:
            if v[i] < v[ext]:
                ext = i
            elif v[i] - v[ext] >= delta:
                piv.append(ext)
                up, ext = True, i
    if up is not None:
        piv.append(ext)
    return piv


def flash_count(v, t, delta=0.10, dark=0.80, window=1.0):
    # ponytail: O(tramos²) por ventana; sobra para cientos de cuadros; con miles, dos punteros
    """-> (máx. destellos en una ventana de `window` s, instante, nº de tramos). WCAG 2.3.1."""
    piv = zigzag(v, delta)
    ends = [t[b] for a_, b in zip(piv, piv[1:]) if min(v[a_], v[b]) < dark]  # tramo >= delta con lado oscuro < 0,8
    best, at = 0, 0.0
    for i, t0 in enumerate(ends):
        k = sum(1 for e in ends[i:] if e < t0 + window - 1e-9) // 2  # dos tramos opuestos = un destello
        if k > best:
            best, at = k, t0
    return best, at, len(ends)


def frame_lum(path, bg, grid):
    # ponytail: baldosas disjuntas; un destello a caballo entre dos se diluye. Mejora: ventanas deslizantes de 1/3 del cuadro
    a = np.asarray(Image.open(path).convert("RGBA")).astype(np.float32)
    al = a[..., 3:] / 255
    lum = luminance((a[..., :3] * al + np.array(bg, np.float32) * (1 - al)).round().astype(np.uint8))
    h, w = lum.shape
    gh, gw = h // grid * grid, w // grid * grid
    tiles = lum[:gh, :gw].reshape(grid, gh // grid, grid, gw // grid).mean((1, 3)).ravel()
    return np.concatenate([[lum.mean()], tiles])


def natural(p: Path):
    return [int(s) if s.isdigit() else s.lower() for s in re.split(r"(\d+)", p.name)]


def seq_times(files, fps, use_mtime):
    if use_mtime:
        m = [f.stat().st_mtime for f in files]
        return [x - m[0] for x in m]
    nums = [re.search(r"(\d+)$", f.stem) for f in files]
    return [int(x.group(1)) / fps for x in nums] if all(nums) else [i / fps for i in range(len(files))]


def cmd_rnf21(a) -> int:
    seqs, loose = [], []
    for s in a.paths:
        if Path(s).is_dir():
            seqs.append((s, sorted(Path(s).glob("*.png"), key=natural)))
        elif any(ch in s for ch in "*?["):
            seqs.append((s, sorted(map(Path, glob.glob(s)), key=natural)))
        elif Path(s).is_file():
            loose.append(Path(s))
        else:
            die(f"no encuentro «{s}»")
    if loose:
        seqs.append(("(archivos sueltos)", sorted(loose, key=natural)))
    bg = hex_rgb(a.bg)
    rows, fails = [], 0
    for name, files in seqs:
        if len(files) < 2:
            die(f"«{name}»: se necesitan al menos dos cuadros")
        t = seq_times(files, a.fps, a.mtime)
        series = np.array([frame_lum(f, bg, a.grid) for f in files])
        (cnt, at, legs), k = max(((flash_count(series[:, k], t, a.delta, a.dark), k) for k in range(series.shape[1])),
                                 key=lambda r: (r[0][0], r[0][2]))
        ok = cnt <= a.max_flashes
        fails += not ok
        where = "cuadro completo" if k == 0 else f"baldosa fila {(k - 1) // a.grid + 1}, col. {(k - 1) % a.grid + 1}"
        rows.append([f"`{name}`", len(files), n(t[-1] - t[0], 2) + " s", f"{n(series[:, 0].min(), 3)}–{n(series[:, 0].max(), 3)}",
                     legs, f"{cnt} ({where}" + (f", hacia {n(at, 2)} s)" if cnt else ")"), "OK" if ok else "**FALLA**"])
    print("# RNF-21: destellos (`arte_check.py rnf21`)\n")
    print(repro())
    print(f"- WCAG 2.3.1: tramo extremo a extremo con cambio de luminancia relativa ≥ {n(a.delta * 100, 0)} % y lado oscuro < {n(a.dark, 2)}; "
          f"dos tramos opuestos = un destello; falla con más de {a.max_flashes} en cualquier segundo. Se evalúa el cuadro completo y cada "
          f"baldosa de {a.grid}×{a.grid}; se informa el peor caso. Los PNG con alfa se componen sobre {a.bg}. "
          "Tiempo = número final del nombre / fps, o la fecha de modificación con --mtime.\n")
    print(md_table(["Secuencia", "Cuadros", "Duración", "Luminancia (cuadro completo)", "Tramos ≥ umbral", "Destellos/s máx.", "Veredicto"], rows))
    print(f"\n**{len(rows)} secuencias · {len(rows) - fails} cumplen · {fails} no cumplen.**")
    return 1 if fails else 0


# ---------------------------------------------------------------- auto-prueba con imágenes sintéticas
def self_test() -> int:
    ok = 0

    def check(cond, msg):
        nonlocal ok
        assert cond, "FALLÓ: " + msg
        ok += 1

    # nombres §15.4
    check(not name_problems("prop_n2_pieza_1.png"), "nombre válido")
    check(any("prefijo" in s for s in name_problems("entorno_n1_cueva_2x.png")), "prefijo entorno_")
    check(any("tildes" in s for s in name_problems("prop_n2_caja_suelo_vacía.png")), "tilde")
    check(name_problems("Prop_N2.png") and name_problems("ui lock.png"), "mayúsculas y espacios")

    # contraste WCAG
    check(abs(contrast(luminance((0, 0, 0)), luminance((255, 255, 255))) - 21) < 1e-9, "negro/blanco = 21")
    check(abs(contrast(luminance((0x76, 0x76, 0x76)), luminance((255, 255, 255))) - 4.54) < 0.01, "#767676/blanco = 4,54")

    # caja de texto: texto negro (≈10 % de la caja) sobre blanco = 21:1; polaridad invertida igual
    img = np.full((100, 200, 3), 255, np.uint8)
    img[40:60, 20:60] = 0
    r, tx, bg, pol = box_contrast(img, (0, 0, 1, 1))
    check(abs(r - 21) < 1e-6 and pol == "oscuro/claro" and tx == (0, 0, 0) and bg == (255, 255, 255), "caja oscuro/claro")
    r, _, _, pol = box_contrast(255 - img, (0, 0, 1, 1))
    check(abs(r - 21) < 1e-6 and pol == "claro/oscuro", "caja claro/oscuro")
    check(box_contrast(np.full((50, 50, 3), 128, np.uint8), (0, 0, 1, 1))[0] == 1.0, "caja lisa = 1:1")

    # RNF-19: círculo contra barra (forma), gris 40 contra 200 con la misma forma (brillo), idénticos (falla)
    yy, xx = np.mgrid[:64, :64]

    def sprite(mask, rgb):
        s = np.zeros((64, 64, 4), np.uint8)
        s[mask] = (*rgb, 255)
        return s

    disk, bar = (yy - 32) ** 2 + (xx - 32) ** 2 < 24 ** 2, (abs(yy - 32) < 5) & (abs(xx - 32) < 28)
    iou, mae, _ = pair_metrics(sprite(disk, (128, 128, 128)), sprite(bar, (128, 128, 128)))  # gris = fondo: solo difiere la forma
    check(iou is not None and 1 - iou >= 0.2 and verdict19(iou, mae) == "OK (forma)", "disco/barra se distinguen por forma")
    iou, mae, _ = pair_metrics(sprite(disk, (40, 40, 40)), sprite(disk, (200, 200, 200)))
    check(iou > 0.99 and mae >= 25 and verdict19(iou, mae) == "OK (gris)", "mismo disco, 40 contra 200: por gris")
    iou, mae, _ = pair_metrics(sprite(disk, (120, 80, 60)), sprite(disk, (120, 80, 60)))
    check(verdict19(iou, mae) == "**FALLA**", "idénticos no se distinguen")
    iou, mae, _ = pair_metrics(np.dstack([np.full((40, 40, 3), 30, np.uint8), np.full((40, 40), 255, np.uint8)]),
                               np.dstack([np.full((40, 40, 3), 220, np.uint8), np.full((40, 40), 255, np.uint8)]))
    check(iou is None and abs(mae - (220 - 30)) < 1 and verdict19(iou, mae) == "OK (gris)", "capturas opacas: sin IoU, gris")

    # halo: anillo verde α=20 alrededor de un cuadrado marrón; el verde legítimo y el magenta
    def with_ring(core_rgb, ring_rgb):
        s = np.zeros((16, 16, 4), np.uint8)
        s[4:12, 4:12] = (*ring_rgb, 20)
        s[5:11, 5:11] = (*core_rgb, 255)
        return s

    brown = with_ring((60, 30, 24), (0, 255, 0))
    ys, xs, new, lvl = find_halo(brown)
    check(len(ys) == 28 and (new == (60, 30, 24)).all() and (lvl == 255).all(), "anillo verde sobre marrón: 28 px a marrón, nivel 255")
    check((find_halo(with_ring((60, 30, 24), (0, 90, 0)))[3] == 90).all(), "verde oscuro: nivel 90 (tenue)")
    check(len(find_halo(with_ring((60, 30, 24), (255, 0, 255)))[0]) == 28, "anillo magenta")
    check(len(find_halo(with_ring((30, 160, 40), (0, 255, 0)))[0]) == 0, "verde legítimo no se toca")
    lone = np.zeros((9, 9, 4), np.uint8)
    lone[4, 4] = (0, 255, 0, 20)
    _, _, new, _ = find_halo(lone)
    check(len(new) == 1 and abs(int(new[0][0]) - round(255 * 0.587)) <= 1, "sin vecinos opacos: gris propio")

    def lum_to_gray(lum):  # luminancia relativa -> gris de 8 bits
        return int(np.argmin(abs(LIN - lum)))

    # aplicar en un directorio temporal: nombre, .meta y chunks auxiliares intactos; solo cambia el halo
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        art, backup = tmp / "Art", tmp / "respaldo"
        art.mkdir()
        png = art / "char_prueba_retrato_neutra.png"
        buf = io.BytesIO()
        Image.fromarray(brown).save(buf, "PNG", dpi=(72, 72))
        png.write_bytes(buf.getvalue())
        meta = Path(str(png) + ".meta")
        meta.write_text("fileFormatVersion: 2\nguid: 0123456789abcdef0123456789abcdef\n", encoding="utf-8")
        h0 = hashlib.sha256(meta.read_bytes()).hexdigest()
        arr = load_rgba(png)
        ys, xs, new, _ = find_halo(arr)
        apply_halo(png, arr, ys, xs, new, backup, art)
        after = load_rgba(png)
        check(len(find_halo(after)[0]) == 0 and (after[..., 3] == arr[..., 3]).all(), "tras aplicar no queda halo y el alfa es el mismo")
        check(hashlib.sha256(meta.read_bytes()).hexdigest() == h0 and sorted(p.name for p in art.iterdir()) == [png.name, meta.name], "el .meta y el nombre no cambian")
        check((backup / png.name).read_bytes() == buf.getvalue(), "respaldo idéntico al original")
        check(b"pHYs" in [t for t, _ in png_chunks(png.read_bytes())], "se conserva pHYs")
        try:
            apply_halo(png, arr, ys, xs, new, backup, art)
            pisa = True
        except ValueError:
            pisa = False
        check(not pisa and (backup / png.name).read_bytes() == buf.getvalue(), "no pisa un respaldo existente")
        rgb_png = art / "char_prueba_rgb.png"
        Image.fromarray(brown[..., :3]).save(rgb_png)
        try:
            apply_halo(rgb_png, arr, ys, xs, new, tmp / "otro", art)
            toca = True
        except ValueError:
            toca = False
        check(not toca, "un PNG que no es RGBA de 8 bits se rechaza")

        # RNF-21 con archivos reales: 10 cuadros/s alternando 0.2 / 0.7 de luminancia relativa
        for i in range(12):
            g = lum_to_gray(0.2 if i % 2 == 0 else 0.7)
            Image.new("RGB", (30, 30), (g, g, g)).save(tmp / f"f_{i:04d}.png")
        series = np.array([frame_lum(tmp / f"f_{i:04d}.png", (0, 0, 0), 3) for i in range(12)])
        check(abs(series[0, 0] - 0.2) < 0.01 and abs(series[1, 0] - 0.7) < 0.01 and series.shape == (12, 10), "luminancia por cuadro y baldosas")

    # RNF-21: umbrales de WCAG sobre series sintéticas a 12 cuadros/s
    t = [i / 12 for i in range(48)]
    flicker = lambda lo, hi, period: [lo if (i // period) % 2 == 0 else hi for i in range(48)]  # noqa: E731
    check(flash_count(flicker(0.2, 0.7, 1), t)[0] >= 4, "parpadeo a 6 Hz falla (>3/s)")
    check(flash_count(flicker(0.2, 0.7, 2), t)[0] == 3, "3 Hz = 3 destellos, no más de tres")
    check(flash_count(flicker(0.2, 0.7, 4), t)[0] == 1, "1,5 Hz = 1 destello")
    check(flash_count(flicker(0.85, 0.99, 1), t)[0] == 0, "lado oscuro >= 0,8 no cuenta")
    check(flash_count(flicker(0.40, 0.45, 1), t)[0] == 0, "cambio de 5 % no cuenta")
    check(flash_count([0.5 + 0.3 * np.sin(2 * np.pi * x / 2) for x in t], t)[0] <= 1, "oscilación lenta de 0,5 Hz no es destello")
    check(flash_count([0.5] * 48, t) == (0, 0.0, 0), "serie constante")

    # extremo a extremo por la CLI (main) con archivos sintéticos en un directorio temporal
    def run(*args):
        buf = io.StringIO()
        with contextlib.redirect_stdout(buf):
            rc = main([str(x) for x in args])
        return rc, buf.getvalue()

    def refuses(*args):
        try:
            with contextlib.redirect_stderr(io.StringIO()):
                run(*args)
        except SystemExit as e:
            return e.code == 2
        return False

    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        art = tmp / "Assets" / "Game" / "Art"
        (art / "Props").mkdir(parents=True)
        Image.fromarray(sprite(disk, (90, 60, 30))).save(art / "Props" / "prop_n1_disco.png")
        Image.fromarray(sprite(bar, (90, 60, 30))).save(art / "Props" / "prop_n1_barra.png")
        Image.fromarray(brown).save(art / "Props" / "prop_n1_halo.png")
        Image.fromarray(sprite(disk, (90, 60, 30))).save(art / "Props" / "Mal Nombre.png")
        # sprites: nombre, halo y .meta ausente se reportan y la salida es 1; --only-fail solo lista lo que falla
        rc, out = run("sprites", "--repo", tmp, "--min-px", 5)
        check(rc == 1 and "Mal Nombre.png" in out and "mayúsculas" in out and "sin .meta" in out and "prop_n1_halo.png" in out, "sprites e2e")
        # halo e2e: informe por defecto (no escribe), --apply exige rutas y respaldo
        before = (art / "Props" / "prop_n1_halo.png").read_bytes()
        rc, out = run("halo", "--repo", tmp, "--min-px", 5)
        check(rc == 0 and "Props/prop_n1_halo.png" in out and (art / "Props" / "prop_n1_halo.png").read_bytes() == before, "halo e2e: informe sin escribir")
        check(refuses("halo", "--repo", tmp, "--apply"), "halo --apply sin rutas ni respaldo se rechaza")
        check(refuses("halo", "--repo", tmp, "--apply", "--backup-dir", tmp / "Assets" / "x", "Props"), "halo --apply con respaldo dentro del repo se rechaza")
        # rnf19 e2e: forma distinta pasa (0); idénticos fallan (1)
        rc, out = run("rnf19", "--repo", tmp, "prop_n1_disco.png", "prop_n1_barra.png")
        check(rc == 0 and "OK (forma" in out, "rnf19 e2e: pareja distinta")
        rc, out = run("rnf19", "--repo", tmp, "prop_n1_disco.png", "prop_n1_disco.png")
        check(rc == 1 and "FALLA" in out, "rnf19 e2e: pareja idéntica falla")
        spec = tmp / "parejas.txt"
        spec.write_text("# grupo\nG ; prop_n1_disco.png ; prop_n1_barra.png ; prop_n1_halo.png\n", encoding="utf-8")
        check(run("rnf19", "--repo", tmp, "--file", spec)[1].count("| G |") == 3, "rnf19 e2e: grupo de tres = tres parejas")
        # rnf20 e2e: colores dados y caja sobre una captura sintética
        rc, out = run("rnf20", "--spec", "negro/blanco ; #000000 ; #FFFFFF")
        check(rc == 0 and "21,0:1" in out, "rnf20 e2e: colores dados")
        check(run("rnf20", "--spec", "gris ; #777777 ; #888888")[0] == 1, "rnf20 e2e: contraste insuficiente falla")
        Image.fromarray(img).save(art / "captura.png")
        rc, out = run("rnf20", "--repo", tmp, "--spec", "caja ; captura.png ; 0,0,1,1", "--crops", tmp / "crops")
        check(rc == 0 and "21,0:1" in out and (tmp / "crops" / "00_caja.png").exists(), "rnf20 e2e: caja sobre captura y recorte guardado")
        check(refuses("rnf20", "--spec", "mala ; captura.png ; 0.5,0.5,0.2,0.2"), "rnf20: caja invertida se rechaza")
        # rnf21 e2e: parpadeo de 6 Hz (12 cuadros/s alternando) falla; rampa lenta pasa; el nombre da el tiempo
        flick, calm = tmp / "flick", tmp / "calm"
        flick.mkdir()
        calm.mkdir()
        for i in range(24):
            g = lum_to_gray(0.2 if i % 2 == 0 else 0.7)
            Image.new("RGB", (30, 30), (g, g, g)).save(flick / f"r_{i:04d}.png")
            g = lum_to_gray(0.2 + 0.5 * i / 23)
            Image.new("RGB", (30, 30), (g, g, g)).save(calm / f"r_{i:04d}.png")
        rc, out = run("rnf21", flick, "--fps", 12)
        check(rc == 1 and "FALLA" in out, "rnf21 e2e: parpadeo rápido falla")
        rc, out = run("rnf21", calm, "--fps", 12, "--grid", 1)
        check(rc == 0 and "| OK |" in out, "rnf21 e2e: rampa lenta pasa")

    print(f"self-test OK: {ok} comprobaciones")
    return 0


# ---------------------------------------------------------------- línea de comandos
def main(argv=None) -> int:
    for s in (sys.stdout, sys.stderr):
        if hasattr(s, "reconfigure"):
            s.reconfigure(encoding="utf-8")
    common = argparse.ArgumentParser(add_help=False)
    common.add_argument("--out", help="escribe el Markdown aquí (UTF-8) en vez de stdout")
    common.add_argument("--repo", default=str(REPO), help=f"raíz del proyecto (por defecto {REPO})")
    ap = argparse.ArgumentParser(prog="arte_check.py", description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--self-test", action="store_true", help="comprobaciones mínimas sobre imágenes sintéticas")
    sub = ap.add_subparsers(dest="cmd")

    def halo_args(q):
        q.add_argument("--max-alpha", type=int, default=64, help="el halo vive en los píxeles con α menor que esto")
        q.add_argument("--excess", type=int, default=40, help="exceso de verde/magenta que cuenta como tinte")
        q.add_argument("--strong", type=int, default=130, help="nivel desde el que el halo es intenso (canal dominante)")
        q.add_argument("--min-px", type=int, default=10, help="píxeles intensos desde los que un archivo cuenta como halo")
        q.add_argument("--radius", type=int, default=3, help="radio de la ventana de vecinos opacos")

    p = sub.add_parser("sprites", parents=[common], help="nombre §15.4, alfa, halo, borde y .meta de cada PNG de Art/")
    p.add_argument("--only-fail", action="store_true", help="la tabla final solo lista los archivos con hallazgos")
    p.add_argument("--no-table", action="store_true", help="sin la tabla final")
    halo_args(p)

    p = sub.add_parser("halo", parents=[common], help="halo de croma: informe (por defecto) o --apply")
    p.add_argument("paths", nargs="*", help="archivos o carpetas (relativos a Art/); por defecto todo Art/")
    p.add_argument("--dry-run", action="store_true", help="solo informe (es lo que hace por defecto)")
    p.add_argument("--apply", action="store_true", help="escribe los PNG; exige --backup-dir")
    p.add_argument("--backup-dir", help="respaldo de los originales, fuera del repo")
    halo_args(p)

    p = sub.add_parser("rnf19", parents=[common], help="parejas en gris: IoU de silueta y diferencia media de gris")
    p.add_argument("images", nargs="*", help="dos imágenes (una pareja) o más (todas las parejas); ruta[@x0,y0,x1,y1]")
    p.add_argument("--spec", action="append", help="«etiqueta ; A ; B [; C ...]» (repetible)")
    p.add_argument("--file", help="archivo con una línea por pareja o grupo")

    p = sub.add_parser("rnf20", parents=[common], help="contraste WCAG en cajas de texto de capturas")
    p.add_argument("--spec", action="append", help="«etiqueta ; captura ; x0,y0,x1,y1 [; mín]» o «etiqueta ; #texto ; #fondo [; mín]»")
    p.add_argument("--file", help="archivo con una línea por región")
    p.add_argument("--crops", help="carpeta donde guardar cada recorte ampliado x2")
    p.add_argument("--p-text", type=float, default=1.0, help="percentil del texto (1)")
    p.add_argument("--p-bg", type=float, default=60.0, help="percentil del fondo (60)")
    p.add_argument("--polarity", choices=("auto", "dark-on-light", "light-on-dark"), default="auto")
    p.add_argument("--min", type=float, default=4.5, help="contraste mínimo por defecto (4,5)")

    p = sub.add_parser("rnf21", parents=[common], help="destellos de una ráfaga de capturas o secuencia de PNG")
    p.add_argument("paths", nargs="+", help="carpeta, comodín o archivos PNG")
    p.add_argument("--fps", type=float, default=30.0)
    p.add_argument("--mtime", action="store_true", help="tiempos = fecha de modificación de los archivos")
    p.add_argument("--grid", type=int, default=3, help="baldosas NxN además del cuadro completo (1 = solo completo)")
    p.add_argument("--bg", default="#000000", help="fondo bajo los PNG con alfa")
    p.add_argument("--delta", type=float, default=0.10)
    p.add_argument("--dark", type=float, default=0.80)
    p.add_argument("--max-flashes", type=int, default=3)

    a = ap.parse_args(argv)
    ARGV[:] = sys.argv[1:] if argv is None else argv
    if a.self_test:
        return self_test()
    if not a.cmd:
        ap.print_help()
        return 2
    if a.out:
        sys.stdout = open(a.out, "w", encoding="utf-8", newline="\n")
    try:
        return {"sprites": cmd_sprites, "halo": cmd_halo, "rnf19": cmd_rnf19, "rnf20": cmd_rnf20, "rnf21": cmd_rnf21}[a.cmd](a)
    finally:
        sys.stdout.flush()


if __name__ == "__main__":
    sys.exit(main())
