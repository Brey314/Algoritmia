# 1. INTRODUCCIÓN Y OBJETIVO

Este documento es el entregable del cuarto objetivo específico del trabajo de grado. Continúa los entregables del primer objetivo (requerimientos, OE1), del segundo (diseño, OE2) y del tercero (prototipo funcional, OE3), y somete el prototipo del tercero a pruebas funcionales contra los requerimientos del primero. Describe el instrumento de evaluación, los candidatos que se probaron, lo que se midió y lo que queda abierto.

## 1.1 El objetivo específico 4

El trabajo de grado formula el cuarto objetivo específico en los siguientes términos:

> Evaluar el prototipo del videojuego mediante pruebas funcionales basadas en los requerimientos previamente definidos, con el propósito de comprobar su funcionamiento e identificar oportunidades de mejora para trabajos futuros.

Del objetivo salen tres compromisos. El primero es que el criterio de evaluación son los requerimientos ya radicados: los 47 requerimientos funcionales (RF-01 a RF-47) y los 23 no funcionales (RNF-01 a RNF-23) del OE1, junto con los criterios de aceptación de las historias de usuario y de los casos de uso que los trazan. El segundo es que se prueba el funcionamiento del prototipo tal como se entrega, es decir, el ejecutable portable de Windows y no el Editor de Unity. El tercero es que la evaluación termina en oportunidades de mejora para trabajos futuros, que reúne el capítulo 4.

## 1.2 Indicadores

El trabajo de grado fija dos indicadores para este objetivo. La eficacia de las pruebas funcionales, que los botones, los sonidos y los saltos de nivel funcionen, tiene una meta de 90 % o más. El grado de cumplimiento de requerimientos exige que todos los RF de prioridad Alta estén implementados. El apartado 2.5 define cómo se calcula cada uno, y el apartado 3.3 los reporta.

## 1.3 Qué cubre este documento y qué no

El documento cubre la evaluación funcional hecha sobre el ejecutable. La pasada final es la del candidato rc5, con {{N_CASOS}} casos agrupados en {{N_SESIONES}} sesiones: 133 en P, 2 en PD, 4 en F y 6 pendientes de ejecución humana. La primera pasada fue la del candidato rc3, con 136 casos, y se conserva como referencia. El documento da además las mediciones de carga, memoria y tamaño del paquete y los defectos hallados. El capítulo 2 explica el método, el capítulo 3 da los resultados, el capítulo 4 las oportunidades de mejora, el capítulo 5 lo que depende de personas ajenas al equipo de desarrollo y el capítulo 6 las conclusiones. El Anexo A relaciona cada caso con el requerimiento que prueba, con su veredicto en las dos pasadas y con la fecha, el candidato, la precondición, el resultado esperado, el observado y el registro de rc5, y trae la lista de huellas del paquete.

La evaluación con estudiantes de grado quinto no entra en los casos del catálogo ni en los indicadores. Los puntos PG-05 (el cambio de esquema de control entre el Nivel 1 y el Nivel 2 no genera confusión) y PG-06 (los valores del Nivel 1 son propuestas, no valores validados) piden observar a niños jugando, y esa observación exige el consentimiento informado de los acudientes que pide RNF-12. Se hizo el 13/10/2026 en el Colegio El Libertador IED, con 4 estudiantes y con el consentimiento firmado de cada uno. El 16/10/2026 jugaron rc5 los 30 estudiantes de un curso de grado quinto, en una sesión de la investigación doctoral de la docente del colegio. El capítulo 5 registra las dos. Tampoco repite la suite de pruebas automatizadas del OE3: la suite es evidencia de apoyo en la matriz del Anexo A y no cuenta en los indicadores.

## 1.4 Corte del documento

Los veredictos vigentes son los de rc5: etiqueta `rc5`, commit `{{COMMIT}}`, huella global `{{HUELLA_GLOBAL}}`. Las sesiones se hicieron el 04 y el 05/11/2026 sobre el paquete congelado, y el arnés comparó la carpeta con su lista de huellas antes de cada lanzamiento (apartado 2.6). La regresión responde a la Corrección 1 del informe de evaluación, que pide veredictos de una sola versión, identificada por versión, commit y huella de todo su contenido. No se compiló un rc6: los defectos que rc5 deja abiertos se registran y no se corrigen.

La primera pasada, sobre rc3, se hizo el 10/10/2026 y se cerró el 16/10/2026 con 132 de 136 casos (97,1 %). Sus veredictos están en `src/veredictos-rc3.csv`, no se mezclan con los de rc5 y toda cifra de este documento lleva su versión. Ese 132 incluye `PF-RNF13-01`, cerrado con dos recorridos sobre rc5 del 16/10/2026; contados solo los veredictos obtenidos sobre rc3 son 131 de 135 (97,0 %). Incluye también `PF-RF21-02` y `PF-RNF20-01`, que rc5 midió mejor y dejó en F (apartado 3.9).

El candidato rc4 (13/10/2026, `0405eba`) no tuvo pasada propia de casos; de él se dan el tamaño, las cargas, la memoria y la suite (apartados 3.5 y 3.7). La suite de rc5, la medición en un equipo del colegio y la sesión de 30 estudiantes, las tres del 16/10/2026, son evidencia de apoyo y no cambian veredictos. Las pasadas sobre rc1 y rc2 (ambas del 01/10/2026) fueron previas; sus veredictos no se heredan, porque el juego cambió en 108 commits entre rc2 y rc3, y se citan solo como historia de las correcciones en el apartado 3.6.
