import numpy as np, sys, os
from PIL import Image
from collections import Counter
R = 'C:/Dev/Algoritmia/claudeDocs/tasks/OE4/evidencias/rc5/'


def lin(c):
    c = c / 255.0
    return np.where(c <= 0.03928, c / 12.92, ((c + 0.055) / 1.055) ** 2.4)


def lum(rgb):
    r, g, b = [lin(np.array(x, dtype=float)) for x in rgb]
    return 0.2126 * r + 0.7152 * g + 0.0722 * b


def ratio(a, b):
    la, lb = float(lum(a)), float(lum(b))
    hi, lo = max(la, lb), min(la, lb)
    return (hi + 0.05) / (lo + 0.05)


def hexc(c):
    return '#%02X%02X%02X' % tuple(int(round(v)) for v in c)


def bands(path, box, mingap=4):
    im = np.asarray(Image.open(R + path).convert('RGB')).astype(float)
    x0, y0, x1, y1 = box
    reg = im[y0:y1, x0:x1]
    flat = reg.reshape(-1, 3).astype(int)
    bg = Counter(map(tuple, flat)).most_common(1)[0][0]
    bgl = float(lum(bg))
    L = lum(reg.transpose(2, 0, 1))
    d = np.abs(L - bgl)
    mask = d > 0.06
    rows = np.nonzero(mask.any(1))[0]
    out = []
    if len(rows) == 0:
        return bg, out
    start = prev = rows[0]
    groups = []
    for r in rows[1:]:
        if r - prev > mingap:
            groups.append((start, prev))
            start = r
        prev = r
    groups.append((start, prev))
    for a, b in groups:
        sub = reg[a:b + 1]
        sd = d[a:b + 1]
        n = max(5, int(0.03 * sd.size))
        n = min(n, sd.size)
        idx = np.argsort(sd.ravel())[-n:]
        tc = sub.reshape(-1, 3)[idx].mean(0)
        cols = np.nonzero(mask[a:b + 1].any(0))[0]
        out.append(((x0 + cols.min(), y0 + a, x0 + cols.max(), y0 + b), bg, tc, ratio(bg, tc)))
    return bg, out


spec = [
    ('Inicio · lema', 'S-INI/PF-RF01-01_2_1.png', (90, 395, 750, 470)),
    ('Inicio · Jugar (texto sobre ámbar)', 'S-INI/PF-RF01-01_2_1.png', (90, 560, 750, 605)),
    ('Inicio · Créditos', 'S-INI/PF-RF01-01_2_1.png', (90, 675, 385, 722)),
    ('Inicio · Salir', 'S-INI/PF-RF01-01_2_1.png', (460, 675, 750, 722)),
    ('Inicio · Progreso del equipo', 'S-INI/PF-RF01-01_2_1.png', (90, 790, 750, 835)),
    ('N1 · tablilla estado más oscuro', 'S-N1/PF-RNF20-01_S-N1_tablilla_estado_mas_oscuro.png', (480, 75, 1440, 150)),
    ('N1 · tablilla (reunir)', 'S-N1/PF-RNF03-01_S-N1_reunir.png', (480, 75, 1440, 150)),
    ('N1 · tablilla (llama)', 'S-N1/PF-RNF03-01_S-N1_llama.png', (480, 75, 1440, 150)),
    ('N1 · rótulo «Fuerza del golpe»', 'S-N1/PF-RNF03-01_S-N1_panel_hipotesis.png', (1672, 254, 1892, 288)),
    ('N1 · «Fuerte»', 'S-N1/PF-RNF03-01_S-N1_panel_hipotesis.png', (1585, 347, 1688, 381)),
    ('N1 · «Lejos»', 'S-N1/PF-RNF03-01_S-N1_panel_hipotesis.png', (612, 842, 716, 876)),
    ('N1 · Golpear (texto sobre ámbar)', 'S-N1/PF-RNF03-01_S-N1_panel_hipotesis.png', (990, 945, 1275, 995)),
    ('N1 · Soplar con candado', 'S-N1/PF-RNF03-01_S-N1_panel_hipotesis.png', (700, 945, 935, 995)),
    ('Diálogo · nombre y línea (Algoritm)', 'S-N1/PF-RNF03-01_S-N1_narrativa_hallazgo.png', (440, 870, 1130, 1010)),
    ('Diálogo · Continuar (sobre ámbar)', 'S-N1/PF-RNF03-01_S-N1_narrativa_hallazgo.png', (1375, 972, 1610, 1018)),
    ('Diálogo · Atrás', 'S-N1/PF-RNF03-01_S-N1_narrativa_hallazgo.png', (1210, 976, 1285, 1012)),
    ('Diálogo narrador (N1 cierre)', 'S-N1/PF-RNF03-01_S-N1_narrativa_cierre.png', (440, 915, 1130, 965)),
    ('Diálogo N3 3.1 (nombre y línea)', 'S-N3/PF-RF10-03_1_1.png', (440, 870, 1130, 1010)),
    ('N2 bosque · tablilla', 'S-N2A/PF-RNF03-01_2_bosque_tablilla.png', (90, 80, 1150, 190)),
    ('N2 bosque · contador', 'S-N2A/PF-RNF03-01_2_bosque_tablilla.png', (75, 812, 690, 855)),
    ('N2 taller · tablilla', 'S-N2B/PF-RNF03-01_1_taller.png', (380, 80, 1545, 200)),
    ('N2 taller · «Aún no»', 'S-N2B/PF-RNF03-01_1_taller.png', (940, 885, 1050, 915)),
    ('N2 taller · Mecanizar deshabilitado', 'S-N2B/PF-RNF03-01_1_taller.png', (830, 945, 1090, 995)),
    ('N2 laberinto · tablilla', 'S-N2C/PF-RNF03-01_1_laberinto.png', (250, 50, 1235, 135)),
    ('N2 laberinto · «Tu secuencia»', 'S-N2C/PF-RNF03-01_1_laberinto.png', (1330, 85, 1550, 125)),
    ('N2 laberinto · «Suelta un bloque aquí»', 'S-N2C/PF-RNF03-01_1_laberinto.png', (1365, 205, 1725, 265)),
    ('N2 laberinto · «Bloques»', 'S-N2C/PF-RNF03-01_1_laberinto.png', (1345, 812, 1480, 858)),
    ('N2 laberinto · Ejecutar (sobre ámbar)', 'S-N2C/PF-RNF03-01_1_laberinto.png', (1345, 935, 1830, 990)),
    ('N2 laberinto · «Arrastra un bloque a tu secuencia»', 'S-N2C/PF-RF31-01_12_b13.png', (1348, 576, 1700, 602)),
    ('N2 laberinto · bloque Avanzar (cajón)', 'S-N2C/PF-RF31-01_12_b13.png', (1372, 640, 1520, 685)),
    ('N2 laberinto · bloque Girar (cajón)', 'S-N2C/PF-RF31-01_12_b13.png', (1600, 640, 1790, 685)),
    ('N2 laberinto · bloque Retroceder (cajón)', 'S-N2C/PF-RF31-01_12_b13.png', (1400, 750, 1740, 790)),
    ('N2 laberinto · bloque Avanzar con cuenta', 'S-N2C/PF-RF31-01_12_b13.png', (1370, 210, 1530, 272)),
    ('N3 · lista (tarea marcada)', 'S-N3/PF-RF36-01_9_1_sogas_marcada.png', (145, 148, 380, 180)),
    ('N3 · lista (tarea pendiente)', 'S-N3/PF-RF36-01_9_1_sogas_marcada.png', (145, 202, 350, 236)),
    ('N3 · tablilla', 'S-N3/PF-RF36-01_9_1_sogas_marcada.png', (775, 90, 1460, 175)),
    ('N3 · tablilla con alerta', 'S-N3/PF-RF42-01_17_2_cuadro_mensaje_y_dos_alertas.png', (775, 90, 1460, 175)),
    ('N3 · Listo (sobre ámbar)', 'S-N3/PF-RF41-01_13_2_base_firme.png', (800, 960, 1120, 1020)),
    ('N3 · Probar balsa (sobre ámbar)', 'S-N3/PF-RF41-01_13_2_base_firme.png', (1190, 960, 1510, 1020)),
    ('Resumen N1 · título', 'S-N1/PF-RNF20-01_S-N1_resumen.png', (480, 140, 1400, 215)),
    ('Resumen N1 · texto', 'S-N1/PF-RNF20-01_S-N1_resumen.png', (320, 280, 1100, 330)),
    ('Resumen N1 · viñetas', 'S-N1/PF-RNF20-01_S-N1_resumen.png', (400, 360, 1100, 670)),
    ('Resumen N1 · cierre «Eso se llama…»', 'S-N1/PF-RNF20-01_S-N1_resumen.png', (460, 725, 1370, 825)),
    ('Resumen N1 · Continuar', 'S-N1/PF-RNF20-01_S-N1_resumen.png', (1300, 893, 1570, 945)),
    ('Resumen N1 · Volver al menú', 'S-N1/PF-RNF20-01_S-N1_resumen.png', (800, 893, 1230, 945)),
    ('Resumen N2 · texto', 'GP2/gp6v_resumen2.png', (320, 312, 1440, 362)),
    ('Resumen N2 · viñetas', 'GP2/gp6v_resumen2.png', (400, 390, 1100, 640)),
    ('Resumen N2 · cierre «Eso se llama…»', 'GP2/gp6v_resumen2.png', (460, 695, 1370, 790)),
    ('Resumen N2 · Continuar', 'GP2/gp6v_resumen2.png', (1300, 862, 1570, 915)),
    ('Resumen N3 · texto', 'GP2/gp6v_resumen3.png', (320, 280, 1560, 400)),
    ('Resumen N3 · viñetas', 'GP2/gp6v_resumen3.png', (400, 420, 1100, 670)),
    ('Resumen N3 · cierre «Eso se llama…»', 'GP2/gp6v_resumen3.png', (460, 725, 1370, 825)),
    ('Resumen N3 · Continuar', 'GP2/gp6v_resumen3.png', (1300, 893, 1570, 945)),
    ('Créditos · título', 'GP2/gp6v_creditos.png', (110, 130, 400, 200)),
    ('Créditos · encabezados y nombres', 'GP2/gp6v_creditos.png', (110, 225, 1180, 840)),
    ('Créditos · Volver', 'GP2/gp6v_creditos.png', (135, 890, 300, 940)),
    ('Informe · título', 'S-DOC/PF-RF46-01_2_OE4_Z.png', (700, 60, 1220, 125)),
    ('Informe · cabeceras', 'S-DOC/PF-RF46-01_2_OE4_Z.png', (700, 290, 1900, 360)),
    ('Informe · celdas', 'S-DOC/PF-RF46-01_2_OE4_Z.png', (700, 385, 1900, 790)),
    ('Informe · lista de perfiles', 'S-DOC/PF-RF46-01_2_OE4_Z.png', (75, 205, 600, 245)),
    ('Informe · Eliminar datos', 'S-DOC/PF-RF46-01_2_OE4_Z.png', (75, 845, 600, 895)),
    ('Informe · Volver al menú', 'S-DOC/PF-RF46-01_2_OE4_Z.png', (75, 960, 300, 1005)),
]

rows = []
for name, p, box in spec:
    if not os.path.exists(R + p):
        rows.append((name, p, 'FALTA', None))
        continue
    bg, bs = bands(p, box)
    if not bs:
        rows.append((name, p, 'sin texto en la caja', None))
        continue
    worst = min(bs, key=lambda b: b[3])
    rows.append((name, p, 'ok', (len(bs), worst)))
print('| Pantalla / elemento | Captura | Fondo | Texto (peor línea) | Razón | ≥ 4,5 |')
print('|---|---|---|---|---|---|')
bad = 0
for name, p, st, v in rows:
    if v is None:
        print(f'| {name} | {p} | - | {st} | - | - |')
        continue
    n, (bx, bg, tc, r) = v
    ok = 'sí' if r >= 4.5 else 'NO'
    if r < 4.5:
        bad += 1
    print(f'| {name} | {p.split("/")[-1]} | {hexc(bg)} | {hexc(tc)} ({n} líneas; caja {bx}) | {r:.2f}:1 | {ok} |')
print('\nmedidas por debajo de 4,5:1:', bad, 'de', len(rows))
