# Transcribe el tablero del laberinto (16x11, fila 0 abajo) de una foto 1920x1080 por luminancia del centro de cada casilla.
import sys
import numpy as np
from PIL import Image
a = np.asarray(Image.open(sys.argv[1]).convert('L')).astype(float)
x0, x1, y0, y1 = 165, 1165, 185, 890
cw, ch = (x1 - x0) / 16, (y1 - y0) / 11
rows = []
for r in range(10, -1, -1):
    s = ''
    for c in range(16):
        cx, cy = x0 + cw * (c + .5), y1 - ch * (r + .5)
        v = a[int(cy - ch * .25):int(cy + ch * .25), int(cx - cw * .25):int(cx + cw * .25)].mean()
        ch_ = '#' if (r in (0, 10) or c in (0, 15)) else ('X' if v < 90 else '.')
        if (c, r) == (0, 8): ch_ = 'S'
        if (c, r) == (15, 2): ch_ = 'R'
        s += ch_
    rows.append(f'{r:2d}   {s}')
print('     0123456789012345'); print('\n'.join(rows))
