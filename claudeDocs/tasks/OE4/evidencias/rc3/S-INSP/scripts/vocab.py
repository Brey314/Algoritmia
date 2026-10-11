import re, sys
root = 'C:/Dev/Algoritmia/Assets/Game/Scripts/Runtime/'
files = ['Core/GameFlow.cs', 'Core/GameFlowRunner.cs', 'Core/SaveStore.cs', 'Core/PlayerProfile.cs', 'Levels/Fire/FirePanelController.cs',
         'Levels/Wheel/ForestSceneController.cs', 'Levels/Wheel/WorkshopSceneController.cs', 'Levels/Wheel/MazeSceneController.cs',
         'Levels/River/AssemblyPanelController.cs', 'UI/NarrativeSceneController.cs']
allw = {}
for f in files:
    txt = open(root + f, encoding='utf-8-sig').read()
    # quitar comentarios y cadenas
    txt = re.sub(r'///.*|//.*', '', txt)
    txt = re.sub(r'\$?@?"(?:[^"\\\n]|\\.)*"', '""', txt)
    for ident in set(re.findall(r'\b[A-Za-z_][A-Za-z0-9_]*\b', txt)):
        for w in re.findall(r'[A-Z]?[a-z]+|[A-Z]+(?![a-z])', ident):
            allw.setdefault(w.lower(), set()).add(ident)
print(len(allw), 'palabras distintas')
print(' '.join(sorted(allw)))
