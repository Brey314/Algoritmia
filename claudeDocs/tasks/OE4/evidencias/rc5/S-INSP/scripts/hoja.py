import os
import sys
from PIL import Image, ImageDraw
out=sys.argv[1]; files=sys.argv[2:]
W,H=800,450
cols=2; rows=(len(files)+1)//2
S=Image.new('RGB',(cols*W,rows*(H+18)),'white'); d=ImageDraw.Draw(S)
for i,f in enumerate(files):
    im=Image.open(f).convert('RGB').resize((W,H),Image.LANCZOS)
    x=(i%cols)*W; y=(i//cols)*(H+18)
    S.paste(im,(x,y+18)); d.text((x+4,y+2),os.path.basename(f),fill='black')
S.save(out)
