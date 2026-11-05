import json, re, sys
rows = json.load(open(sys.argv[1], encoding='utf-8'))
out = []
long_ = []
terms = {}
TERMS = ['iterar', 'iteración', 'iteracion', 'abstra', 'algoritm', 'descompon', 'descomposic', 'deprur', 'depur', 'patrón', 'patron', 'secuencia', 'bucle', 'variable', 'función', 'condición', 'parámetro', 'lógic', 'program', 'código', 'indicador', 'pensamiento computacional']
nwords = 0
for f, k, s in rows:
    if 'TeacherReport' in f or '/Reporting/' in f:
        continue
    t = re.sub(r'\{\d*[^}]*\}', 'X', s)  # placeholders
    t = t.replace('\n', ' ')
    # oraciones: . ! ? … seguidos de espacio o fin
    parts = re.split(r'(?<=[.!?…])\s+', t)
    for p in parts:
        p = p.strip()
        if not p:
            continue
        w = [x for x in re.split(r'\s+', p) if re.search(r'\w', x)]
        nwords += len(w)
        out.append((f, k, len(w), p))
        if len(w) > 20:
            long_.append((f, k, len(w), p))
    low = t.lower()
    for term in TERMS:
        if term in low:
            terms.setdefault(term, []).append((f, k, s))
print('oraciones', len(out), 'palabras', nwords, 'max', max(o[2] for o in out))
print('--- oraciones > 20 palabras:', len(long_))
for f, k, n, p in long_:
    print(n, f, k.strip('<>').replace('k__BackingField', ''), '|', p)
print('--- top 15 por longitud')
for f, k, n, p in sorted(out, key=lambda o: -o[2])[:15]:
    print(n, f.split('/')[-1], '|', p[:200])
print('--- tecnicismos')
for term, lst in terms.items():
    print('##', term, len(lst))
    for f, k, s in lst:
        print('   ', f.split('/')[-1], '|', s.replace('\n', ' ')[:300])
