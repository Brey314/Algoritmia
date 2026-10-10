# Prompt: entrada del arte final de Algoritm

Prompt listo para pegar en una sesión nueva de Claude Code cuando llegue la entrega del arte final
de Algoritm. Lo dejó el orquestador de la ronda de ajustes del 08/10/2026 (decisión D20 de
`claudeDocs/tasks/Ajustes-Diseno-2026-10-08/brief.md`).

---

```
Santiago entrega el arte final de Algoritm. Los archivos están en <RUTA DE LA ENTREGA>. Hay que meterlo en el juego y cerrar los pendientes que quedaron para este momento.

Contexto que debes leer antes de tocar nada:
- CLAUDE.md, sobre todo los párrafos de personajes (rig por recorte, INC-131 a INC-133). Reconstruir un prefab cambia sus fileID y rompe escenas y narrativas: se trabaja por los modos de BuildRigsFinal, nunca recreando el prefab.
- claudeDocs/tasks/Ajustes-Diseno-2026-10-08/notas-04a-algoritm-creditos.md. Cómo se partió Algoritm con el arte provisional el 08/10/2026: §2 el corte, §3 el generador, §4 Wave, §9 los pasos para el arte final, §10 los límites conocidos.
- claudeDocs/tasks/Personajes/Personajes-Resultados.md, Anexo C (C.4 caras, C.9 a C.11), y Plan-Personajes-Finales.md.
- claudeDocs/Direccion_de_Arte.md, §7.6 Algoritm y §13. Aplica el checklist del §17 a la entrega.

Tarea:
1. Revisa la entrega contra la dirección de arte: tres formas (fuego, rueda, gota), recoloreadas en madera y agua (INC-45, INC-52), y piezas que encajen con las nueve de rig_articulaciones.json. Si la entrega trae la cara aparte (ojos y boca), habrá que encender Ojos y Boca (nombres de C.4). Si algo no encaja, para y pregúntale a Santiago.
2. Guarda los originales fuera de Assets, como se hizo con la familia (d26ea8f).
3. Sustituye los 27 PNG de Assets/Game/Art/Characters/Algoritm/Frontal/ (char_algoritm_<fuego|rueda|gota>_parte_*.png) por los nuevos CON EL MISMO NOMBRE, conservando .meta y GUID. Las imágenes nuevas pasan a Sprite Mode Single desde el motor (ArtImportRules).
4. Mide sobre el alfa de los PNG nuevos el rect y el punto de cada articulación y escríbelos en rig_articulaciones.json. Hoy son las constantes ALG de articulaciones.py, medidas sobre el arte provisional. Corre hombro.py y codo.py si aplican a Algoritm.
5. Enseña a pose_preview.py a dibujar a Algoritm desde los PNG de Frontal/ y no desde la maqueta del sprite entero, como hace la familia con es_final. Corre pose_preview.py completo: 10/10 por forma de Algoritm y 21/21 por miembro de la familia. Corre también sus autopruebas y coreografia.py --valida.
6. En el Editor, corre BuildRigsFinal en los modos sprites, orden y clips, por ese orden, filtrando solo a Algoritm. El .txt recorre los siete personajes; el filtro _soloGuia se explica en §9 de las notas de la 4a. Comprueba con git diff que ningún prefab ni clip de la familia cambia, que cada prefab de Algoritm conserva sus objetos y fileID, y que los clips solo cambian si cambió la geometría.
7. Pendientes de D20, que se cierran ahora:
   a. Fundidos (Appear, Vanish, Hidden): las articulaciones se ven más oscuras unos 0,3 s, porque el alfa del CanvasGroup multiplica cada Image por separado y los solapes se oscurecen. Busca la solución mínima y pruébala en captura con alfa 0,35. Si el arte nuevo ya no solapa piezas, puede que desaparezca solo: compruébalo antes de escribir código.
   b. Talón claro de unos 12 px en el hombro girado (se ve ampliando ×6). Comprueba si sigue con el arte nuevo y corrígelo en el dato del rig, no en el arte.
8. Revisa a Algoritm en captura, a 1920×1080 en Play manual, en: los créditos (Wave), el menú principal (Idle con la familia en el bosque), la narrativa N1_NacimientoDelFuego (Appear, Talk, Point, Celebrate), una narrativa del N2 (rueda) y una del N3 (gota). Revisa también el retrato del guía en las mecánicas, que usa el sprite _reposo y no el rig: si la entrega trae un _reposo nuevo, sustitúyelo con el mismo nombre. Guarda las capturas en claudeDocs/tasks/Personajes/capturas/<fecha>/.
9. Mide el peso: el paquete debe seguir por debajo de 500 MB (RNF-06; rc2 pesaba 479,0 MB) y la memoria por debajo de 2 GB (RNF-05). Si no puedes compilar, dilo.

Pruebas:
- Solo EditMode durante la implementación, por editor.ps1: CharacterRig, FacialEmotion, Game.Scaffolding.Tests, ArtImport.
- PlayMode al final y solo de lo afectado: Credits_, MainMenu_, NarrativeScene_, Personajes_DA133_, CharacterCapture.

Documentos: registra la entrada como un apartado nuevo del Anexo C de Personajes-Resultados.md, marca como cerrados los pendientes de D20 y actualiza Direccion_de_Arte.md §7.6, Assets/Game/Art/Inventario.md, CLAUDE.md (que hoy dice que de Algoritm «falta» el arte final) y el Anexo F del OE3 (src/anexos/F-personajes.md, y luego build.py --publish).

No hagas commit: deja los mensajes, uno para el arte y el rig y otro para los documentos, con la tarjeta Kanban «Personajes finales» (INC-131).
```
