import subprocess,re
out=subprocess.run(['git','log','--no-merges','--format=@@%h|%ad|%B','--date=short','--name-only','ca6fd91','--','Assets/'],capture_output=True,text=True,encoding='utf-8',cwd='C:/Dev/Algoritmia').stdout
commits=[]
for blk in out.split('@@')[1:]:
    lines=blk.split('\n'); h,d,first=lines[0].split('|',2)
    files=[l for l in lines[1:] if l.startswith('Assets/')]
    body=[]
    for l in lines:
        if l.startswith('Assets/'): break
        body.append(l)
    commits.append((h,d,first,' '.join(body),files))
card=re.compile(r'\b[DO]\d{2}-\d+\b'); mark=re.compile(r'<id>')
cls=lambda m:'tarjeta' if card.search(m) else ('marcador' if mark.search(m) else 'nada')
other=lambda m:bool(re.search(r'\b(INC-\d+|DEF-\w+|R\d+|W\d+|T\d+|Slice \d|Fase \d+|PF-\w+|RF-\d+)\b',m))
res=[(h,d,f,cls(m),other(m),any(x.endswith('.cs') for x in fs)) for h,d,f,m,fs in commits]
def rep(name,sel):
    n=len(sel);t=sum(r[3]=='tarjeta' for r in sel);m=sum(r[3]=='marcador' for r in sel);x=n-t-m;o=sum(r[3]=='nada' and r[4] for r in sel)
    print(f'{name}: {n} commits; con tarjeta {t} ({100*t/n:.1f} %); con marcador <id> {m} ({100*m/n:.1f} %); sin nada {x} ({100*x/n:.1f} %); de estos citan otro identificador (INC/DEF/R/W/T/Slice/PF/RF) pero no tarjeta: {o}; sin ningun identificador: {x-o}')
print('PF-RNF17-01 · git log --no-merges ca6fd91 -- Assets/ · criterio: con tarjeta = Dnn-n u Onn-n en el mensaje; marcador = literal <id> (tarjeta: <id>) sin resolver; sin nada = ninguno.')
print('Denominador de codigo = commits que tocan al menos un .cs bajo Assets/ (los demas son de arte, sonido, datos, escenas o documentacion y no entran).')
rep('A) todos los commits que tocan Assets/ (lectura literal del guion)',res)
code=[r for r in res if r[5]]
rep('B) DENOMINADOR DE CODIGO',code)
print('   excluidos de B (sin .cs):',len(res)-len(code))
rep('C) B desde 02/09/2026 (tras el tablero Kanban)',[r for r in code if r[1]>='2026-09-02'])
print('\n--- con tarjeta'); [print(' ',r[0],r[1],'cs' if r[5] else '--',r[2][:110]) for r in res if r[3]=='tarjeta']
print('--- marcador <id>'); [print(' ',r[0],r[1],'cs' if r[5] else '--',r[2][:110]) for r in res if r[3]=='marcador']
print('--- sin tarjeta ni marcador (con .cs)'); [print(' ',r[0],r[1],'otro-id' if r[4] else 'nada',r[2][:110]) for r in code if r[3]=='nada']
print('--- excluidos (sin .cs)'); [print(' ',r[0],r[1],r[3],r[2][:100]) for r in res if not r[5]]
