# 1. INTRODUCCIÓN Y OBJETIVO

Este documento es el entregable del cuarto objetivo específico del trabajo de grado. Continúa los entregables del primer objetivo (requerimientos, OE1), del segundo (diseño, OE2) y del tercero (prototipo funcional, OE3), y somete el prototipo del tercero a pruebas funcionales contra los requerimientos del primero. Describe el instrumento de evaluación, el candidato que se probó, lo que se midió y lo que queda abierto.

## 1.1 El objetivo específico 4

El trabajo de grado formula el cuarto objetivo específico en los siguientes términos:

> Evaluar el prototipo del videojuego mediante pruebas funcionales basadas en los requerimientos previamente definidos, con el propósito de comprobar su funcionamiento e identificar oportunidades de mejora para trabajos futuros.

Del objetivo salen tres compromisos. El primero es que el criterio de evaluación son los requerimientos ya radicados: los 47 requerimientos funcionales (RF-01 a RF-47) y los 23 no funcionales (RNF-01 a RNF-23) del OE1, junto con los criterios de aceptación de las historias de usuario y de los casos de uso que los trazan. El segundo es que se prueba el funcionamiento del prototipo tal como se entrega, es decir, el ejecutable portable de Windows y no el Editor de Unity. El tercero es que la evaluación termina en oportunidades de mejora para trabajos futuros, que reúne el capítulo 4.

## 1.2 Indicadores

El trabajo de grado fija dos indicadores para este objetivo. La eficacia de las pruebas funcionales, que los botones, los sonidos y los saltos de nivel funcionen, tiene una meta de 90 % o más. El grado de cumplimiento de requerimientos exige que todos los RF de prioridad Alta estén implementados. El apartado 2.5 define cómo se calcula cada uno, y el apartado 3.3 los reporta.

## 1.3 Qué cubre este documento y qué no

El documento cubre la evaluación funcional hecha sobre el ejecutable del candidato rc3: {{N_CASOS}} casos de prueba, agrupados en {{N_SESIONES}} sesiones, con el veredicto de 132 de ellos (126 del corte del 10/10/2026 y 6 que cerró el 13/10/2026 la hoja de comprobaciones humanas, en una sesión manual sobre rc3 cuyo resultado completo está en el capítulo 5); los otros 4 siguen pendientes, las mediciones de carga, memoria y tamaño del paquete, y los defectos hallados. El capítulo 2 explica el método, el capítulo 3 da los resultados, el capítulo 4 las oportunidades de mejora, el capítulo 5 lo que depende de personas ajenas al equipo de desarrollo y el capítulo 6 las conclusiones. El Anexo A relaciona cada caso con el requerimiento que prueba y con su veredicto.

La evaluación con estudiantes de grado quinto no entra en los 136 casos ni en los indicadores. Los puntos PG-05 (el cambio de esquema de control entre el Nivel 1 y el Nivel 2 no genera confusión) y PG-06 (los valores del Nivel 1 son propuestas, no valores validados) piden observar a niños jugando, y esa observación exige el consentimiento informado de los acudientes que pide RNF-12. Se hizo el 13/10/2026 en el Colegio El Libertador IED, con 4 estudiantes y con el consentimiento firmado de cada uno; el capítulo 5 la registra. Tampoco repite la suite de pruebas automatizadas del OE3: la suite es evidencia de apoyo en la matriz del Anexo A y no cuenta en los indicadores.

## 1.4 Corte del documento

Los veredictos de este documento corresponden al candidato rc3 y a la pasada de pruebas hecha el 10/10/2026. El 13/10/2026 se recompiló el candidato rc4 desde `0405eba`, con las correcciones de código del triaje (apartado 3.6); de rc4 se dan el tamaño, las cargas, la memoria y la suite (apartados 3.5 y 3.7), y no tiene pasada propia de los casos `PF-*`. El 14/10/2026 se compiló rc5 desde `ca6fd91`, con las correcciones de DEF-RC3-01 y DEF-RC4-01 (apartado 3.6); de rc5 solo se da la procedencia y el tamaño (apartado 2.6), sin pasada de casos, sin medidas de carga ni de memoria y sin suite, de modo que las medidas y la suite vigentes son las de rc4 y los veredictos, los de rc3. El 14/10/2026 se compiló rc5 desde `ca6fd91`, con las correcciones de DEF-RC3-01 y DEF-RC4-01 (apartado 3.6); de rc5 solo se da la procedencia y el tamaño (apartado 2.6), sin pasada de casos, sin medidas de carga ni de memoria y sin suite, de modo que las medidas y la suite vigentes son las de rc4 y los veredictos, los de rc3. Las pasadas sobre rc1 y rc2 (ambas del 01/10/2026) fueron previas; sus veredictos no se heredan, porque el juego cambió en 108 commits entre rc2 y rc3, y se citan solo como historia de las correcciones en el apartado 3.6.
