import csv, sys

R = 'C:/Dev/Algoritmia/claudeDocs/tasks/OE4/evidencias/rc5/S-N1/'
FILES = ['PF-RNF21-01_21_rafaga_cueva_llama.csv', 'PF-RNF21-01_22_rafaga_cierre_llama.csv']


def load(f):
    rows = list(csv.DictReader(open(R + f, encoding='utf-8')))
    return [(float(r['t_ms']) / 1000.0, float(r['luma8'])) for r in rows]


def extrema(vals):
    """Puntos de giro de la serie (picos y valles), con su indice."""
    pts = [0]
    for i in range(1, len(vals) - 1):
        a, b, c = vals[i - 1], vals[i], vals[i + 1]
        if (b - a) * (c - b) < 0 or (b == a and c != b and (b - a) * (c - b) <= 0):
            pts.append(i)
    pts.append(len(vals) - 1)
    return pts


def flashes_in_window(ts, ls, t0, t1, thr=0.10):
    """Cuenta cambios de ida y vuelta: dos transiciones consecutivas de sentido opuesto, cada una de mas del thr
    (relativo a la mayor de las dos luminancias) = 1 destello. Se mira el rango [t0, t1]."""
    idx = [i for i, t in enumerate(ts) if t0 <= t <= t1]
    if len(idx) < 3:
        return 0
    v = [ls[i] for i in idx]
    ex = extrema(v)
    trans = []
    for a, b in zip(ex, ex[1:]):
        va, vb = v[a], v[b]
        rel = abs(vb - va) / max(va, vb, 1e-9)
        if rel > thr:
            trans.append(1 if vb > va else -1)
    n = 0
    for x, y in zip(trans, trans[1:]):
        if x != y:
            n += 1
    return n


out = []
out.append('PF-RNF21-01 · rafagas de S-N1 (paso 21) · luminancia media global por cuadro (columna luma8, 8 bits)')
out.append('Criterio: oscilacion de ida y vuelta = dos transiciones consecutivas de sentido opuesto, cada una de mas del 10 % de la luminancia (relativo al mayor de los dos extremos), dentro de una ventana deslizante de 1 s. Umbral de aprobacion: menos de 3 destellos por segundo.')
out.append('Limite: la ráfaga se toma a ~10,1 fps (cuadros cada ~100 ms), de modo que solo detecta oscilaciones por debajo de ~5 Hz; la serie es de luminancia global, no de regiones.')
worst_all = 0
for f in FILES:
    d = load(f)
    ts = [t for t, _ in d]
    ls = [l for _, l in d]
    dur = ts[-1] - ts[0]
    out.append('')
    out.append('## %s: %d cuadros en %.2f s (%.1f fps)' % (f, len(d), dur, (len(d) - 1) / dur))
    out.append('luminancia: min %.2f, max %.2f, rango %.2f (%.1f %% del maximo)' % (min(ls), max(ls), max(ls) - min(ls), 100 * (max(ls) - min(ls)) / max(ls)))
    jumps = [abs(b - a) / max(a, b) for a, b in zip(ls, ls[1:])]
    out.append('salto mayor entre cuadros consecutivos: %.2f %% (cuadro %d a %d, %.2f -> %.2f)' % (
        100 * max(jumps), jumps.index(max(jumps)) + 1, jumps.index(max(jumps)) + 2, ls[jumps.index(max(jumps))], ls[jumps.index(max(jumps)) + 1]))
    big = [(i + 1, ts[i + 1], ls[i], ls[i + 1]) for i, j in enumerate(jumps) if j > 0.10]
    out.append('cuadros con salto > 10 %%: %d %s' % (len(big), ['cuadro %d a t=%.1fs: %.1f -> %.1f' % b for b in big]))
    wmax = 0
    wat = 0
    t = ts[0]
    while t + 1.0 <= ts[-1] + 1e-9:
        n = flashes_in_window(ts, ls, t, t + 1.0)
        if n > wmax:
            wmax, wat = n, t
        t += 0.1
    out.append('maximo de oscilaciones >10 %% en una ventana de 1 s: %d (ventana desde t=%.1f s)' % (wmax, wat))
    worst_all = max(worst_all, wmax)
    # la parte con la cueva (cuadros antes del fundido)
out.append('')
out.append('MAXIMO GLOBAL: %d destellos por ventana de 1 s (umbral: menos de 3)' % worst_all)
print('\n'.join(out))
