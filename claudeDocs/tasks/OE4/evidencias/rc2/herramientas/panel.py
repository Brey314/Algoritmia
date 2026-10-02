# Recorta el panel derecho del laberinto (x 1296-1920) de una o varias fotos, lado a lado (herramienta).
import sys
from PIL import Image
fs = sys.argv[2:]; out = sys.argv[1]
ims = [Image.open(f'fotos/{f}.png').convert('RGB').crop((1296, 0, 1920, 1080)) for f in fs]
S = Image.new('RGB', (624 * len(ims), 1080), 'white')
for i, im in enumerate(ims): S.paste(im, (624 * i, 0))
S.save(f'fotos/{out}.png'); print(out)
