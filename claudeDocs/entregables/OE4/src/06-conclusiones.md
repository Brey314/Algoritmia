# 6. CONCLUSIONES

## 6.1 Respuesta al objetivo

El objetivo pedía comprobar el funcionamiento del prototipo con pruebas basadas en los requerimientos e identificar oportunidades de mejora. De los 136 casos del catálogo, 122 terminan en P y 4 en F, lo que da una eficacia de 96,8 % frente a la meta de 90 % (candidato rc3, 10/10/2026). Los 4 F son Menores. En las etiquetas que nombra el indicador, `NAV` llega a 94,1 % y `BOT` a 100,0 %; `SON` no se pudo calcular, porque sus tres casos son de oído y los hace Santiago. De los 45 RF de prioridad Alta, ninguno tiene un defecto Bloqueante ni Mayor abierto, así que el segundo indicador es de 45 de 45. Diez casos están en PS y quedan fuera del cálculo: si los diez fallaran, la eficacia sería 89,7 %.

La evaluación halló 16 defectos, ninguno Bloqueante ni Mayor: 7 Menores y 9 Observaciones. De ellos y del instrumento salen las 29 oportunidades de mejora del capítulo 4, repartidas en interfaz, flujo del juego, arte y paquete, código y trazabilidad, e instrumento de evaluación.

## 6.2 Rendimiento y portabilidad

En el equipo 1, la peor carga de escena de rc3 fue de 0,268 s (`Narrative`) frente a los 10 s de RNF-04, y el arranque de `Boot` a `MainMenu` tardó unos 3,4 s. El pico de memoria privada fue de 1.094 MiB (1,15 GB) frente a los 2 GB de RNF-05. El paquete pesa 426,6 MB frente a los 500 MB de RNF-06, con un margen de 73,4 MB; rc2 pesaba 479,0 MB. RNF-10 midió 0 conexiones en 4.516 muestras. Faltan las medidas del equipo 2 (RNF-04 y RNF-05 en otro equipo, `PF-RNF07-02`), el equipo sin tarjeta gráfica dedicada (`PF-CT02-01`) y el modo avión (`PF-RNF08-01`), que son de Santiago.

## 6.3 Lo que no se evaluó

No se evaluó con estudiantes de grado cuarto: PG-05 y PG-06 siguen abiertos porque exigen observar a niños jugando y esa sesión depende del consentimiento informado de los acudientes (RNF-12), del visto bueno del colegio y de las fechas, datos que solo tiene Santiago (capítulo 5). Tampoco tienen resultado las comprobaciones de oído, de vista y de mano de la hoja HUM, el recorrido completo con cronómetro y las confirmaciones documentales de RNF-22 y RNF-23. Los dos recorridos completos del arnés de `PF-RNF13-01` no se ejecutaron.

## 6.4 Trabajos futuros

Ordenadas por su efecto en quien usa el juego, las oportunidades del capítulo 4 empiezan por las que afectan al estudiante: el doble clic en «Omitir» que salta el resumen del Nivel 1 (DEF-RC3-09), la instrucción del bosque que se sale de su tablilla (DEF-RC3-10) y el avatar del resumen de nivel (DEF-RC3-01). Siguen las del docente: la cabecera del informe con los iconos sobre el texto (DEF-RC3-05) y el botón «Eliminar datos» partido en dos líneas (DEF-RC3-11). Después van las del equipo de desarrollo: los identificadores en español de tres controladores del Nivel 2 (DEF-RC3-I01), la tarjeta del Kanban en cada commit (DEF-RC3-I02), las pruebas de arquitectura que no toleran un nivel ausente (DEF-RC3-12) y la suite sobre el contenido exacto del candidato (DEF-RC3-I04). Cierran las del instrumento: el arnés (`Restaurar-Registro`, el retardo de la primera captura) y las correcciones de `casos.md`. El triaje de cada una lo decide Santiago (apartado 3.6).
