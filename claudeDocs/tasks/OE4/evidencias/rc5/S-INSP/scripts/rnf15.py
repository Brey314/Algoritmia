import re, sys, os
root = 'C:/Dev/Algoritmia/'
files = {
 'GameFlow': 'Core/GameFlow.cs', 'GameFlowRunner': 'Core/GameFlowRunner.cs', 'SaveStore': 'Core/SaveStore.cs',
 'PlayerProfile': 'Core/PlayerProfile.cs', 'FirePanelController': 'Levels/Fire/FirePanelController.cs',
 'ForestSceneController': 'Levels/Wheel/ForestSceneController.cs', 'WorkshopSceneController': 'Levels/Wheel/WorkshopSceneController.cs',
 'MazeSceneController': 'Levels/Wheel/MazeSceneController.cs', 'AssemblyPanelController': 'Levels/River/AssemblyPanelController.cs',
 'NarrativeSceneController': 'UI/NarrativeSceneController.cs'}
pascal = re.compile(r'^[A-Z][A-Za-z0-9]*$')
camel = re.compile(r'^[a-z][A-Za-z0-9]*$')
ucamel = re.compile(r'^_[a-z][A-Za-z0-9]*$')
SP_WORDS = set('nivel fuego rueda balsa perfil jugador escena fase paso pasos intento golpe piedra tronco carretilla laberinto bloque secuencia mensaje texto pista guia familia papa mama nina nino cueva bosque taller rio llama humo hoja hojas arbol caja eje cuerda tabla agujero camino casilla espacio pieza materiales'.split())
def words(ident):
    return [w.lower() for w in re.findall(r'[A-Z]?[a-z]+|[A-Z]+(?![a-z])|\d+', ident)]
res = []
for cls, rel in files.items():
    path = root + 'Assets/Game/Scripts/Runtime/' + rel
    lines = open(path, encoding='utf-8-sig').read().split('\n')
    depth = 0
    type_decl = []
    members = []
    pending_attr = ''
    class_depth = None
    summary_ok = False
    # localizar clase y su doc
    for i, l in enumerate(lines):
        m = re.search(r'\b(?:public|internal)\s+(?:sealed\s+|static\s+|abstract\s+|partial\s+)*(class|struct|enum|interface)\s+(\w+)', l)
        if m and m.group(2) == cls:
            j = i - 1
            doc = []
            while j >= 0 and (lines[j].strip().startswith('///') or lines[j].strip().startswith('[')):
                doc.append(lines[j].strip()); j -= 1
            summary_ok = any('<summary>' in d for d in doc)
            sum_text = ' '.join(d for d in reversed(doc) if d.startswith('///'))
            break
    # recorrer miembros del nivel de la clase principal y tipos anidados
    depth = 0
    in_cls = False
    for i, l in enumerate(lines):
        s = l.strip()
        code = re.sub(r'//.*$', '', l)
        if not in_cls:
            m = re.search(r'\b(class|struct)\s+(\w+)', code)
            if m and m.group(2) == cls and '{' in ''.join(lines[i:i + 3]):
                in_cls = True
                body_depth = depth + 1
        if in_cls and depth == body_depth + 0 + (1 if False else 0):
            pass
        # detectar declaraciones cuando depth == nivel miembros (namespace=1, class=2 -> miembros a depth 2 despues de abrir)
        if in_cls and depth >= 2 and not s.startswith(('///', '//', '*')):
            decl = code.strip()
            # campo
            mf = re.match(r'(?:\[[^\]]*\]\s*)*((?:public|private|protected|internal)\s+)?((?:static|readonly|const|volatile|new)\s+)*([\w<>\[\],.?]+(?:<[^>]*>)?)\s+(\w+)\s*(=|;)', decl)
            mm = re.match(r'(?:\[[^\]]*\]\s*)*((?:public|private|protected|internal)\s+)?((?:static|virtual|override|abstract|async|sealed|new|extern)\s+)*([\w<>\[\],.?()\s]+?)\s+(\w+)\s*(<[\w, ]+>)?\(', decl)
            mp = re.match(r'(?:\[[^\]]*\]\s*)*((?:public|private|protected|internal)\s+)?((?:static|virtual|override|abstract|sealed|new)\s+)*([\w<>\[\],.?()]+)\s+(\w+)\s*(\{|=>)', decl)
            kind = None
            if mf and depth == 2 and not decl.startswith(('return', 'var ', 'using', 'if', 'foreach', 'for ', 'while')):
                kind = 'field'; acc = (mf.group(1) or 'private ').strip(); name = mf.group(4)
                mods = (mf.group(2) or '')
            elif mm and depth == 2 and not decl.startswith(('return', 'if', 'foreach', 'for ', 'while', 'switch', 'new ', 'using', 'lock', 'else', 'catch')):
                kind = 'method'; acc = (mm.group(1) or 'private ').strip(); name = mm.group(4); mods = mm.group(2) or ''
            elif mp and depth == 2 and not decl.startswith(('return', 'if', 'else', 'var ')):
                kind = 'prop'; acc = (mp.group(1) or 'private ').strip(); name = mp.group(4); mods = mp.group(2) or ''
            if kind:
                prev = ' '.join(x.strip() for x in lines[max(0, i - 3):i])
                ser = 'SerializeField' in prev or 'SerializeField' in decl
                members.append((i + 1, kind, acc, name, ser, 'const' in mods))
        depth += code.count('{') - code.count('}')
    res.append((cls, rel, summary_ok, sum_text[:260], members))
bad = []
total = 0
for cls, rel, sok, st, members in res:
    nb = []
    vocab = set()
    for ln, kind, acc, name, ser, const in members:
        total += 1
        for w in words(name):
            vocab.add(w)
        ok = True
        if kind in ('method', 'prop'):
            ok = bool(pascal.match(name))
        else:
            if acc in ('public', 'internal', 'protected'):
                ok = bool(pascal.match(name)) or (ser and bool(camel.match(name)))
            else:
                if const:
                    ok = bool(pascal.match(name))
                elif ser:
                    ok = bool(camel.match(name)) or bool(ucamel.match(name))
                else:
                    ok = bool(ucamel.match(name)) or (name.isupper())
        if not ok:
            nb.append((ln, kind, acc, name, 'ser' if ser else ''))
    sp = sorted(w for w in vocab if w in SP_WORDS)
    print('==', cls, rel, '| <summary> en la clase:', sok, '| miembros analizados:', len(members), '| fuera de convencion:', len(nb), '| palabras en espanol (lista corta):', sp)
    print('   summary:', st)
    for x in nb:
        print('   !!', x)
print('total miembros', total)
