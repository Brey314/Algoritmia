# Corta a cada personaje en torso, brazos y piernas para animarlo por recorte (cut-out).
# Entrada: los PNG base en A-pose (1024x1024). Salida: partes recortadas + manifest.json con pivotes.
import json, os, sys, math
from PIL import Image, ImageDraw

SRC = r"D:/User/Desktop/Trabajo de greado/assets a postproduccion/familia"
OUT = os.path.dirname(os.path.abspath(__file__)) + "/out"

CHARS = {
    "papa": dict(src="Papá/papa.png",
                 armL=[(400, 376), (372, 462), (357, 540), (265, 650), (250, 790), (30, 790), (30, 560), (200, 470), (335, 392)],
                 armR=[(626, 380), (656, 465), (668, 540), (760, 650), (775, 790), (995, 790), (995, 560), (820, 470), (690, 392)],
                 pivL=(385, 445), pivR=(645, 445),
                 legL=(290, 508), legR=(509, 735), band=700, hipL=(430, 745), hipR=(605, 745),
                 skin=(300, 800, 722, 946), floor=802,
                 head=(312, 60, 712, 460)),
    "mama": dict(src="Mamá/mama base.png",
                 armL=[(447, 352), (425, 405), (421, 435), (330, 520), (240, 660), (140, 660), (140, 520), (300, 420), (380, 378)],
                 armR=[(590, 358), (598, 400), (602, 440), (690, 520), (722, 660), (890, 660), (890, 520), (720, 420), (640, 378)],
                 pivL=(428, 398), pivR=(598, 398),
                 legL=(350, 512), legR=(513, 675), band=650, hipL=(450, 700), hipR=(575, 700),
                 skin=(350, 728, 672, 946), floor=732,
                 head=(315, 40, 715, 440)),
    "nina": dict(src="Niña/niña.png",
                 armL=[(456, 497), (450, 530), (446, 562), (380, 640), (300, 700), (180, 700), (180, 580), (330, 520), (420, 495)],
                 armR=[(569, 496), (576, 530), (586, 566), (650, 640), (730, 700), (845, 700), (845, 580), (700, 520), (610, 495)],
                 pivL=(447, 530), pivR=(580, 530),
                 legL=(295, 505), legR=(506, 730), band=735, hipL=(450, 770), hipR=(570, 770),
                 skin=(300, 798, 726, 946), floor=797,
                 head=(295, 75, 730, 510)),
    "nino": dict(src="Niño/niño.png",
                 armL=[(487, 511), (436, 560), (426, 602), (330, 700), (280, 790), (170, 790), (170, 640), (330, 560), (440, 505)],
                 armR=[(577, 516), (590, 560), (598, 603), (672, 700), (690, 790), (860, 790), (860, 640), (700, 560), (600, 512)],
                 pivL=(440, 562), pivR=(592, 562),
                 legL=(335, 512), legR=(513, 695), band=775, hipL=(455, 815), hipR=(570, 815),
                 skin=(338, 834, 690, 946), floor=834,
                 head=(290, 70, 735, 515)),
}


OUTLINE = (58, 30, 24, 255)  # #3A1E18, el contorno de Direccion_de_Arte §4


def is_green(p):
    r, g, b, a = p
    return a > 0 and g > 120 and g > r + 50 and g > b + 50


def clean_green(im):
    """Quita los restos de croma verde de las puntas del pelo: toma el color del vecino no verde."""
    px = im.load()
    w, h = im.size
    bad = [(x, y) for y in range(h) for x in range(w) if is_green(px[x, y])]
    for _ in range(6):
        rest = []
        for x, y in bad:
            best = None
            for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1)):
                nx, ny = x + dx, y + dy
                if 0 <= nx < w and 0 <= ny < h:
                    q = px[nx, ny]
                    if q[3] > 0 and not is_green(q):
                        best = q
                        break
            if best is None:
                rest.append((x, y))
            else:
                px[x, y] = (best[0], best[1], best[2], px[x, y][3])
        bad = rest
    for x, y in bad:
        px[x, y] = (0, 0, 0, 0)
    return len(bad)


def poly_mask(size, poly):
    m = Image.new("L", size, 0)
    ImageDraw.Draw(m).polygon(poly, fill=255)
    return m


def hairlike(p):
    r, g, b, a = p
    return a > 0 and r >= 70 and r > 2.4 * g and r > 2.4 * b and r < 215


def dark(p):
    r, g, b, a = p
    return a > 128 and (0.3 * r + 0.59 * g + 0.11 * b) < 75


def palette(im, box):
    cols = set()
    px = im.load()
    for y in range(box[1], box[3]):
        for x in range(box[0], box[2]):
            p = px[x, y]
            if p[3] > 200:
                cols.add((p[0] // 8, p[1] // 8, p[2] // 8))
    return cols


def skinlike(p, pal):
    if p[3] < 128:
        return False
    k = (p[0] // 8, p[1] // 8, p[2] // 8)
    return any((k[0] + a, k[1] + b, k[2] + c) in pal for a in (-1, 0, 1) for b in (-1, 0, 1) for c in (-1, 0, 1))


def largest_component(img, keep_disk=None):
    """Deja solo la pieza conexa más grande: fuera motas sueltas de otras partes."""
    a = img.getchannel("A").load()
    w, h = img.size
    seen = bytearray(w * h)
    comps = []
    for sy in range(h):
        for sx in range(w):
            if a[sx, sy] == 0 or seen[sy * w + sx]:
                continue
            comp = []
            stack = [(sx, sy)]
            seen[sy * w + sx] = 1
            while stack:
                x, y = stack.pop()
                comp.append((x, y))
                for nx, ny in ((x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1)):
                    if 0 <= nx < w and 0 <= ny < h and not seen[ny * w + nx] and a[nx, ny] > 0:
                        seen[ny * w + nx] = 1
                        stack.append((nx, ny))
            comps.append(comp)
    keep = bytearray(w * h)
    for comp in comps:
        if len(comp) >= 300:  # solo fuera las motas; una pieza real nunca es tan pequeña
            for x, y in comp:
                keep[y * w + x] = 1
    px = img.load()
    removed = 0
    for y in range(h):
        for x in range(w):
            if a[x, y] and not keep[y * w + x]:
                px[x, y] = (0, 0, 0, 0)
                removed += 1
    return removed


def cut(name, cfg):
    im = Image.open(os.path.join(SRC, cfg["src"])).convert("RGBA")
    left = clean_green(im)
    w, h = im.size
    px = im.load()
    pal = palette(im, cfg["skin"])

    torso = im.copy()
    tp = torso.load()
    parts = {}

    # Brazos: dentro del polígono, sin el pelo que roza el hombro.
    for side in ("L", "R"):
        m = poly_mask(im.size, cfg["arm" + side]).load()
        arm = Image.new("RGBA", im.size, (0, 0, 0, 0))
        ap = arm.load()
        for y in range(h):
            for x in range(w):
                # El pelo solo roza el brazo por encima del hombro: por debajo, todo es brazo.
                if m[x, y] and px[x, y][3] > 0 and not (y < cfg["piv" + side][1] - 20 and hairlike(px[x, y])):
                    ap[x, y] = px[x, y]
                    tp[x, y] = (0, 0, 0, 0)
        # Hombro redondo bajo el torso: un disco del color de la piel alrededor del pivote,
        # para que al girar el brazo no asome el hueco del corte.
        pv = cfg["piv" + side]
        skin = px[pv[0], pv[1]]
        rad = cfg.get("rad", 30)
        ring = 7
        for y in range(pv[1] - rad - ring, pv[1] + rad + ring + 1):
            for x in range(pv[0] - rad - ring, pv[0] + rad + ring + 1):
                d2 = (x - pv[0]) ** 2 + (y - pv[1]) ** 2
                if d2 <= (rad + ring) ** 2 and ap[x, y][3] == 0:
                    ap[x, y] = (skin[0], skin[1], skin[2], 255) if d2 <= rad * rad else OUTLINE
        parts["brazo_" + ("izq" if side == "L" else "der")] = (arm, pv)

    # Piernas: por columna, desde el pie hacia arriba mientras sea piel o contorno; lo que queda
    # por encima es la falda. El dobladillo (su contorno) se queda en el torso.
    for side in ("L", "R"):
        x0, x1 = cfg["leg" + side]
        hip = cfg["hip" + side]
        leg = Image.new("RGBA", im.size, (0, 0, 0, 0))
        lp = leg.load()
        for x in range(x0, x1 + 1):
            ys = [y for y in range(cfg["band"], h) if px[x, y][3] > 0]
            if not ys:
                continue
            # Sube desde el pie mientras sea pierna. Un hueco transparente es que se salió de la
            # pierna (junto al pie no hay falda): ahí se corta, sin llevarse el borde de la falda.
            y = max(ys)
            gap = 0
            last = y
            by_skirt = False
            while y > cfg["band"]:
                q = px[x, y]
                if q[3] < 128:
                    gap += 1
                    if gap > 6:
                        break
                elif skinlike(q, pal) or dark(q) or y >= cfg["floor"]:
                    # Bajo el dobladillo no hay falda: un borde antialiasado también es pierna.
                    gap = 0
                    last = y
                else:
                    by_skirt = True
                    break
                y -= 1
            top = last
            k = 0
            while by_skirt and k < 14 and top + k < h and dark(px[x, top + k]):
                k += 1
            cut_y = top + k
            if cut_y >= h or all(px[x, yy][3] == 0 for yy in range(cut_y, h)):
                continue
            for yy in range(cut_y, h):
                if px[x, yy][3] > 0:
                    lp[x, yy] = px[x, yy]
                    tp[x, yy] = (0, 0, 0, 0)
            # Prolonga la pierna bajo la falda (oculta) para que al girar no se vea el hueco.
            fill = px[x, min(cut_y + 3, h - 1)]
            if fill[3] > 200 and (skinlike(fill, pal) or dark(fill)):
                # Solo bajo la falda (donde el torso es opaco): lo que asoma entre dientes no se rellena.
                for yy in range(cut_y - 1, hip[1] - 40, -1):
                    if tp[x, yy][3] < 200:
                        break
                    if lp[x, yy][3] == 0:
                        lp[x, yy] = fill
        parts["pierna_" + ("izq" if side == "L" else "der")] = (leg, hip)

    # Bajo el dobladillo el torso no tiene nada: fuera restos de las piernas y líneas de un píxel.
    for y in range(cfg["band"], h):
        for x in range(1, w - 1):
            if tp[x, y][3] == 0:
                continue
            if y >= cfg["floor"] or (tp[x - 1, y][3] == 0 and tp[x + 1, y][3] == 0):
                tp[x, y] = (0, 0, 0, 0)
    parts["torso"] = (torso, None)

    for pname, (img, _) in parts.items():
        gone = largest_component(img)
        if gone:
            print(" ", name, pname, "motas fuera:", gone)
    os.makedirs(os.path.join(OUT, name), exist_ok=True)
    man = {"size": [w, h], "parts": {}}
    full_bbox = im.getchannel("A").getbbox()
    for pname, (img, piv) in parts.items():
        bb = img.getchannel("A").getbbox()
        crop = img.crop(bb)
        crop.save(os.path.join(OUT, name, f"char_{name}_parte_{pname}.png"))
        if piv is None:
            # El torso gira y se agacha desde la cadera: pivote en el centro de las dos caderas.
            piv = ((cfg["hipL"][0] + cfg["hipR"][0]) // 2, (cfg["hipL"][1] + cfg["hipR"][1]) // 2)
        man["parts"][pname] = {"bbox": list(bb), "pivot": list(piv)}
    im.save(os.path.join(OUT, name, f"char_{name}_base_apose.png"))
    man["bbox"] = list(full_bbox)
    hb = cfg["head"]
    head = im.crop(hb).resize((320, 320), Image.LANCZOS)
    head.save(os.path.join(OUT, name, f"char_{name}_retrato_neutra.png"))
    with open(os.path.join(OUT, name, "manifest.json"), "w") as f:
        json.dump(man, f, indent=1)
    print(name, "green left", left, {k: v["bbox"] for k, v in man["parts"].items()})


if __name__ == "__main__":
    names = sys.argv[1:] or list(CHARS)
    for n in names:
        cut(n, CHARS[n])
