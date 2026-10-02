# Hoja de contacto de una ráfaga: recorte y escala de N cuadros (herramienta de evidencia).
import sys, glob
from PIL import Image, ImageDraw
def tapa(im):
    if im.size == (1920, 1080):
        d = ImageDraw.Draw(im); d.rectangle((1808, 128, 1919, 434), fill=(128, 128, 128)); d.rectangle((1884, 1016, 1919, 1079), fill=(128, 128, 128))
    return im
pref, x0, y0, x1, y1, desde, hasta, cols, salida = sys.argv[1], *map(int, sys.argv[2:9]), sys.argv[9]
fs = sorted(glob.glob(f'rafagas/{pref}_*.png'))[desde-1:hasta]
w, h = (x1-x0)//2, (y1-y0)//2
rows = (len(fs)+cols-1)//cols
S = Image.new('RGB', (cols*w, rows*(h+18)), 'white')
d = ImageDraw.Draw(S)
for i, f in enumerate(fs):
    im = tapa(Image.open(f).convert('RGB')).crop((x0, y0, x1, y1)).resize((w, h))
    S.paste(im, ((i % cols)*w, (i//cols)*(h+18)+18))
    d.text(((i % cols)*w+4, (i//cols)*(h+18)+2), f.split('_')[-1].split('.')[0], fill='black')
S.save(salida)
print(salida, len(fs))
