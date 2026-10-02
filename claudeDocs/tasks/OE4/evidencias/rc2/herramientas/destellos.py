# RNF-21 sobre ráfagas: luminancia relativa (sRGB -> lineal, Rec.709) por cuadro, global y por teselas de 6x4,
# sin la ventana flotante de Webex (x >= 1800, y 120-440) ni sus iconos (x >= 1880, y >= 1010).
# Un «destello» (WCAG 2.3.1) = par de cambios opuestos de >= 0,10 con el más oscuro < 0,80. Herramienta, no juego.
import sys, glob, csv
import numpy as np
from PIL import Image
def lin(c):
    c = c / 255.0
    return np.where(c <= 0.04045, c / 12.92, ((c + 0.055) / 1.055) ** 2.4)
series = []; ts = []
for pref in sys.argv[1:]:
    t = {int(r['frame']): float(r['t_ms']) for r in csv.DictReader(open(f'rafagas/{pref}.csv'))}
    off = ts[-1] + 100 if ts else 0
    for f in sorted(glob.glob(f'rafagas/{pref}_*.png')):
        n = int(f.split('_')[-1].split('.')[0])
        a = np.asarray(Image.open(f).convert('RGB').resize((480, 270))).astype(float)
        L = 0.2126 * lin(a[..., 0]) + 0.7152 * lin(a[..., 1]) + 0.0722 * lin(a[..., 2])
        m = np.ones_like(L, bool); m[30:110, 450:] = False; m[252:, 470:] = False
        g = L[m].mean()
        tiles = [L[j*67:(j+1)*67, i*80:(i+1)*80][m[j*67:(j+1)*67, i*80:(i+1)*80]].mean() for j in range(4) for i in range(6)]
        series.append([g] + tiles); ts.append(off + t[n])
S = np.array(series)
d = np.diff(S, axis=0)
print(f'{len(S)} cuadros, {ts[0]:.0f}–{ts[-1]:.0f} ms. Global: {S[:,0].min():.4f}–{S[:,0].max():.4f}; |Δ| máx global {np.abs(d[:,0]).max():.4f}; |Δ| máx en una tesela {np.abs(d[:,1:]).max():.4f}')
def flashes(x):
    ev = []
    for k in range(1, len(x)):
        dv = x[k] - x[k-1]
        if abs(dv) >= 0.10 and min(x[k], x[k-1]) < 0.80: ev.append((k, np.sign(dv)))
    return sum(1 for a, b in zip(ev, ev[1:]) if a[1] != b[1])
fl = [flashes(S[:, c]) for c in range(S.shape[1])]
print('Pares de cambios opuestos >= 0,10 (destellos) por serie [global, 24 teselas]:', fl, '-> máximo', max(fl))
big = [(round(ts[k+1]), round(float(d[k,0]),4)) for k in range(len(d)) if abs(d[k,0]) >= 0.02]
print('Saltos globales >= 0,02 (t ms, Δ):', big)
