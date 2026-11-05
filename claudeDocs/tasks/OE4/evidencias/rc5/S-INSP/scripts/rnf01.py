import re, sys, glob, os, yaml, json
from collections import Counter
root = 'C:/Dev/Algoritmia/'
skipkeys = {'m_Name', 'm_EditorClassIdentifier', 'm_TagString', 'propertyPath', 'm_Script', 'm_FontData'}
TEXTKEYS = {'m_Text', 'm_text', 'text', 'Text', 'Label', 'Title', 'Message', 'Line', 'line', 'Caption'}


def load(path):
    t = open(path, encoding='utf-8').read()
    t = re.sub(r'^--- !u!\d+ &\d+( stripped)?$', '---', t, flags=re.M)
    t = re.sub(r'^%TAG.*$', '', t, flags=re.M)
    try:
        return list(yaml.safe_load_all(t))
    except Exception as e:
        print('ERR', path, str(e)[:100], file=sys.stderr)
        return []


def walk(o, key, out):
    if isinstance(o, dict):
        for k, v in o.items():
            if k in skipkeys:
                continue
            walk(v, k, out)
    elif isinstance(o, list):
        for v in o:
            walk(v, key, out)
    elif isinstance(o, str):
        out.append((key, o))


files = []
files += sorted(glob.glob(root + 'Assets/Game/Data/**/*.asset', recursive=True))
files += sorted(glob.glob(root + 'Assets/Game/Scenes/*.unity'))
files += sorted(glob.glob(root + 'Assets/Game/Prefabs/UI/*.prefab'))
rows = []
for f in files:
    fn = f.replace(chr(92), '/')
    if '/Reporting/' in fn:
        continue
    out = []
    for d in load(f):
        walk(d, None, out)
    seen = set()
    for k, s in out:
        s = s.strip()
        if not s or s in seen:
            continue
        seen.add(s)
        if not re.search(r'[A-Za-z\u00c1\u00c9\u00cd\u00d3\u00da\u00e1\u00e9\u00ed\u00f3\u00fa\u00f1\u00d1]{2,}', s):
            continue
        if re.fullmatch(r'[\w./\-]+', s) and k not in TEXTKEYS:
            continue
        rows.append((os.path.relpath(fn, root).replace(chr(92), '/'), k, s))
json.dump(rows, open(sys.argv[1], 'w', encoding='utf-8'), ensure_ascii=False, indent=0)
print(len(rows), 'cadenas de', len(files), 'archivos')
print(Counter(k for _, k, _ in rows).most_common(70))
