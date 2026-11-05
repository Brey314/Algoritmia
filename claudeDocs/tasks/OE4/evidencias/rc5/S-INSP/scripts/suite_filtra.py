import glob,re,os
from collections import Counter
import xml.etree.ElementTree as ET
base='C:/Dev/Algoritmia/claudeDocs/tasks/OE4/evidencias/suites/rc5-06a5b61/'
rows=[]
for f in glob.glob(base+'*/*.xml'):
    t=ET.parse(f).getroot()
    for tc in t.iter('test-case'):
        rows.append((tc.get('fullname'),tc.get('result'),os.path.basename(os.path.dirname(f))))
print(len(rows),'casos en los XML')
pats={'RNF02':r'Controls_RNF02_|RiverScene_INC01_|_RNF02_|Architecture_INC01','RNF03':r'_RNF03_','RNF19':r'_RNF19_','RNF20':r'_RNF20_','RNF21':r'_RNF21_'}
out=[]
for k,p in pats.items():
    sel=[r for r in rows if re.search(p,r[0])]
    c=Counter(r[1] for r in sel)
    out.append(f'## {k}: {len(sel)} pruebas {dict(c)}')
    for r in sorted(sel): out.append('   %-8s %s'%(r[1],r[0]))
open('suite-rc5-pruebas-relevantes.txt','w',encoding='utf-8').write('Suite rc5-06a5b61 (HEAD 06a5b61 + arbol c599b82e13ec; Assets/ identico a ca6fd91 salvo Inventario.md)\n'+'\n'.join(out))
print('\n'.join(l for l in out if l.startswith('##')))
