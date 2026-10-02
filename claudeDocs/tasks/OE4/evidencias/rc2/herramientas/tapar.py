# Tapa en el sitio la ventana flotante de Webex (reunión ajena al juego: caras y diapositivas de terceros)
# en todas las fotos y cuadros de 1920x1080 de esta carpeta. Herramienta, no juego.
import glob
from PIL import Image, ImageDraw
n = 0
for f in glob.glob('fotos/*.png') + glob.glob('rafagas/*.png'):
    im = Image.open(f)
    if im.size != (1920, 1080): continue
    im = im.convert('RGB'); d = ImageDraw.Draw(im)
    d.rectangle((1808, 128, 1919, 434), fill=(128, 128, 128)); d.rectangle((1884, 1016, 1919, 1079), fill=(128, 128, 128))
    im.save(f); n += 1
print(n, 'imágenes tapadas')
