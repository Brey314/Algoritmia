import re, glob, json, sys, os
root = 'C:/Dev/Algoritmia/'
lit = re.compile(r'\$?@?"((?:[^"\\\n]|\\.)*)"')
out = []
for f in sorted(glob.glob(root + 'Assets/Game/Scripts/Runtime/**/*.cs', recursive=True)):
    fn = f.replace(chr(92), '/')
    if '/Reporting/' in fn or 'TeacherReport' in fn or 'EraseConfirmation' in fn:
        continue
    for i, line in enumerate(open(f, encoding='utf-8-sig'), 1):
        s = line.strip()
        if s.startswith('//') or s.startswith('///') or s.startswith('*') or 'Tooltip(' in s or 'Debug.' in s or 'Exception(' in s or 'Assert' in s or 'FormattableString' in s and False:
            continue
        code = re.sub(r'//.*$', '', line) if '"' not in line.split('//')[0][-1:] else line
        for m in lit.finditer(code):
            t = m.group(1)
            if len(t.split()) >= 3 and re.search(r'[A-Za-z\u00e1\u00e9\u00ed\u00f3\u00fa\u00f1]{3,}', t) and not t.startswith(('Assets/', 'http')):
                out.append((os.path.relpath(fn, root), i, t))
print(len(out), 'literales de 3+ palabras en codigo de runtime (sin Reporting/informe)')
long_ = []
for f, i, t in out:
    tt = re.sub(r'\{[^}]*\}', 'X', t)
    for p in re.split(r'(?<=[.!?\u2026])\s+', tt):
        n = len([w for w in p.split() if re.search(r'\w', w)])
        if n > 20:
            long_.append((n, f, i, p))
print('oraciones > 20 palabras:', len(long_))
for x in long_:
    print(x)
print('--- muestra (primeros 25)')
for f, i, t in out[:25]:
    print(f.split('/')[-1], i, '|', t[:150])
