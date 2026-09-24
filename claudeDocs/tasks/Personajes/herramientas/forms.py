# Formas de Algoritm: la de fuego es el arte entregado (recortado a cuadrado); rueda y gota son
# provisionales recoloreadas desde el fuego, con su nombre definitivo, para sustituir el archivo.
import colorsys, os
from PIL import Image
SRC = r"D:/User/Desktop/Trabajo de greado/assets a postproduccion/familia/Algoritm/base.png"
OUT = os.path.dirname(os.path.abspath(__file__)) + "/out/algoritm"
os.makedirs(OUT, exist_ok=True)
im = Image.open(SRC).convert("RGBA")
bb = im.getchannel("A").getbbox()
w, h = bb[2] - bb[0], bb[3] - bb[1]
side = max(w, h) + 24
sq = Image.new("RGBA", (side, side), (0, 0, 0, 0))
sq.paste(im.crop(bb), ((side - w) // 2, (side - h) // 2))
sq = sq.resize((768, 768), Image.LANCZOS)
sq.save(os.path.join(OUT, "char_algoritm_n1_fuego_reposo.png"))
belly = int(768 * (870 - bb[1] + (side - h) // 2) / side)  # por debajo empieza la franja de colores

def recolor(hue, sat_k, val_k):
    out = sq.copy()
    px = out.load()
    for y in range(min(belly, 768)):
        for x in range(768):
            r, g, b, a = px[x, y]
            if a == 0:
                continue
            hh, s, v = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
            if s > 0.25 and (hh < 0.17 or hh > 0.95) and v > 0.35:
                nr, ng, nb = colorsys.hsv_to_rgb(hue, min(1, s * sat_k), min(1, v * val_k))
                px[x, y] = (int(nr * 255), int(ng * 255), int(nb * 255), a)
    return out

recolor(0.09, 0.62, 0.86).save(os.path.join(OUT, "char_algoritm_n2_rueda_reposo.png"))  # madera #C79A5E
recolor(0.54, 0.75, 0.92).save(os.path.join(OUT, "char_algoritm_n3_gota_reposo.png"))   # agua #5AA8BF
print("ok", belly)
