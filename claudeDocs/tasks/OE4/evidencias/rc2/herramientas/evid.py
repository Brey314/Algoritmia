# Copia fotos o recortes a evidencias/rc2 tapando la ventana flotante de Webex (reunión ajena al juego).
# Uso: evid.py <origen.png> <destino-nombre.png>   (rutas relativas a esta carpeta). Herramienta, no juego.
import sys, os
from PIL import Image, ImageDraw
src, dst = sys.argv[1], sys.argv[2]
im = Image.open(src).convert('RGB'); W, H = im.size
if (W, H) == (1920, 1080):
    s = 1.0
    d = ImageDraw.Draw(im)
    d.rectangle((1808, 128, 1919, 434), fill=(128, 128, 128)); d.text((1814, 270), 'ventana\najena', fill='white')
    d.rectangle((1884, 1016, 1919, 1079), fill=(128, 128, 128))
os.makedirs('C:/Dev/Algoritmia/claudeDocs/tasks/OE4/evidencias/rc2', exist_ok=True)
out = 'C:/Dev/Algoritmia/claudeDocs/tasks/OE4/evidencias/rc2/' + dst
im.save(out); print(out)
