# Cuenta píxeles del rayo (amarillo pálido) por cuadro de una ráfaga y estima su duración (herramienta).
import sys, glob, csv
import numpy as np
from PIL import Image
pref = sys.argv[1]
x0, y0, x1, y1 = (560, 120, 1400, 820) if len(sys.argv) < 3 else map(int, sys.argv[2:6])
ts = {int(r['frame']): float(r['t_ms']) for r in csv.DictReader(open(f'rafagas/{pref}.csv'))}
vis = []
rows = []
for f in sorted(glob.glob(f'rafagas/{pref}_*.png')):
    n = int(f.split('_')[-1].split('.')[0])
    a = np.asarray(Image.open(f).convert('RGB').crop((x0, y0, x1, y1))).astype(int)
    R, G, B = a[..., 0], a[..., 1], a[..., 2]
    m = (R > 225) & (G > 205) & (B > 130) & (R - B < 120)
    rows.append((n, ts[n], int(m.sum())))
base = sorted(c for _, _, c in rows)[len(rows)//2]
for n, t, c in rows:
    if c - base > 200: vis.append((n, t))
    print(n, t, c - base)
if vis:
    print(f'RAYO visible en cuadros {vis[0][0]}..{vis[-1][0]}: de {vis[0][1]:.0f} a {vis[-1][1]:.0f} ms ({vis[-1][1]-vis[0][1]:.0f} ms entre el primero y el último; cota superior +1 intervalo)')
else:
    print('RAYO no visible')
