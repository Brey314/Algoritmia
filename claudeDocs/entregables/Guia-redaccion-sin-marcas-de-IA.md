# Guía de redacción sin marcas de IA para el trabajo de grado y sus anexos

Lista de control para quien redacte o revise texto del trabajo de grado (ingeniería, Colombia, IEEE/ICONTEC) y de los anexos técnicos. Versión del 06/10/2026. Los números entre corchetes remiten al apartado 7.

**Cómo usarla.** Ninguna señal prueba por sí sola que un texto lo escribió una máquina; lo que delata es el conjunto [1]. Los detectores automáticos tampoco sirven de árbitro: en promedio marcaron como IA el 61 % de unos ensayos de personas no nativas del inglés [7], y la evidencia reciente sigue siendo dispar [21]. Quien revisa con criterio sí acierta: el voto mayoritario de cinco anotadores que usan LLM a menudo falló en 1 de 300 artículos, y su acierto resistió la paráfrasis y los «humanizadores» [6]. Por eso esta guía no enseña a disfrazar un texto. Enseña a escribir lo que solo puede salir de este proyecto: fechas, cifras, nombres de archivo y decisiones propias con su porqué. Si se usó IA para redactar, se declara (apartado 5, regla 10).

**Nota de fiabilidad.** La evidencia cuantitativa es sobre inglés [2][3][4][8]. Para otros idiomas hay un estudio de 34 lenguas, sin desglose del español en su resumen [5]. Las marcas del español de esta guía salen de ese patrón, de artículos de prensa y blogs [16][17][18][19] y de las normas de la RAE; son heurísticas, no un corpus. Los umbrales marcados «(orientativo)» son del equipo, no de la literatura.

## 1. Vocabulario a evitar

Tras 2022 se disparó el uso de ciertas palabras: en 15,1 millones de resúmenes de PubMed, 379 palabras de estilo crecieron en 2024 (66 % verbos), con «delves» ×28,0, «underscores» ×13,8 y «showcasing» ×10,7 sobre lo esperado [2]. Entre 2022 y 2024, «delve» subió un 1 500 %, «underscore» un 1 000 % e «intricate» un 700 %, más en áreas STEM que en ciencias sociales y humanidades [3]. Los verbos del tipo «enfatizar» aparecen entre lo sobreusado en 24 de 34 idiomas [5]. En español sus equivalentes son «subrayar», «destacar» y «resaltar». El origen no está resuelto: los autores no lo atribuyen a la arquitectura ni a los datos de entrenamiento y señalan el ajuste con retroalimentación humana como posible factor [4].

| Evitar | En su lugar |
|---|---|
| «cabe destacar», «vale la pena señalar», «es importante señalar / tener en cuenta» [16][19] | Borrarlo y abrir con el hecho. |
| «en el panorama / mundo / contexto actual», «en la era digital», «hoy en día», «en un mundo donde» (calcos de «in today's landscape») [18] | Un dato con fecha («En 2026, …») o suprimir la apertura. |
| «juega un papel crucial / clave» (calco de «plays a crucial role») [1] | El verbo concreto: «valida», «decide», «impide». |
| «subraya», «destaca», «resalta», «pone de manifiesto», «refleja», «es un testimonio de» [1][16] | «muestra», «indica», «da» y la cifra, o nada. |
| «adentrarse», «sumergirse», «embarcarse», «ahondar», «desentrañar», «navegar los desafíos», «desbloquear» (calcos de delve, embark, navigate, unlock) [16][17] | «analizar», «examinar», «estudiar», «revisar». |
| «robusto», «integral», «holístico», «sinergia», «sin costuras», «dinámico», «vibrante», «multifacético» [1][16] | Un criterio medible: «cubre los 47 RF», «pesa 479,0 MB frente al tope de 500 MB». Si no se mide, se borra. |
| «fomentar», «potenciar», «empoderar», «impulsar», «aprovechar», «apalancar» (foster, enhance, empower, leverage) [1][17] | «favorecer», «permitir», «usar», o el efecto: «reduce el tiempo de A a B». |
| «innovador», «de vanguardia», «transformador», «enriquecedor», «inmersivo», «experiencia de aprendizaje significativa» [1][16] | Lo que el juego hace: «el estudiante arrastra bloques y el tronco rueda». «Aprendizaje significativo» solo como concepto citado. |
| «paisaje», «panorama», «tapiz», «ecosistema», «paradigma», «reino» (landscape, tapestry, realm) [1][16] | El nombre concreto de la cosa. |
| «en base a», «a nivel de», «y/o», «impactar», «proveer», «utilizar», «asegurar que», «en orden de» | «con base en» o «según»; «en» o «respecto de»; «o»; «afectar»; «dar»; «usar»; «comprobar que»; «para» [15][24]. |
| «sirve como», «actúa como», «se erige como», «constituye», «cuenta con», «alberga», «ofrece» donde bastaba «es» o «tiene» [1] | «es», «tiene». |
| «en resumen», «en conclusión», «en definitiva», «en última instancia», «sin lugar a dudas», «indudablemente», «ciertamente» [16][17] | Borrar; la conclusión va en su capítulo y con resultados. |
| Residuos de chat: «¡Claro!», «Aquí tienes», «Espero que esto ayude», «¿Quieres que…?», «como modelo de lenguaje» [1] | Borrar y buscarlos también en el Word. |

«Fundamental», «crucial», «clave», «esencial» y «significativo» no están prohibidos, pero cada uso necesita una medida al lado.

## 2. Estructura y retórica

- **Tríadas sistemáticas.** «Rápido, confiable y escalable»: enumerar de a tres, con elementos casi iguales en largo, es un rasgo señalado en inglés y en español [1][18]. Poner los elementos que hay: dos, cuatro o uno.
- **Antítesis.** «No solo X, sino también Y», «No es X, es Y», «más que X, Y» [1][17][18]. Afirmar Y. La negación se reserva para cuando alguien citado afirmó X.
- **Gerundio de cierre.** «…, destacando la importancia de…», «…, garantizando así…», «…, sentando las bases para…». Es la «frase con -ing» sin contenido de la guía de Wikipedia [1]. En español el gerundio que solo indica una acción posterior es incorrecto; se admite si expresa consecuencia directa [13]. Cambiarlo por otra oración con el hecho.
- **Cierres con resumen o moraleja.** Párrafos que terminan en «Esto demuestra la importancia de…», subapartados que se resumen a sí mismos, conclusiones circulares [18][19]. Terminar en el dato.
- **Plantilla «desafíos y perspectivas».** «A pesar de sus limitaciones, el prototipo sienta las bases para…» [1]. Una limitación se escribe con su causa y el punto abierto que la origina (por ejemplo, PG-05 y PG-06 siguen abiertos porque exigen observar a estudiantes jugando).
- **Preguntas retóricas y ganchos.** «¿La razón?», «¿El detalle clave?» [18][22]. Se pregunta solo la pregunta de investigación.
- **Transiciones y anuncios vacíos.** «Veamos», «Exploremos», «Dividámoslo en tres partes», «Como se mencionó anteriormente», «Es importante recordar que» [17][18]. Remitir con «apartado 4.2» o «Tabla 3.1».
- **Exceso de conectores.** «Además», «Asimismo», «Sin embargo», «Por lo tanto» abriendo frases seguidas; incluso al corregir textos, ChatGPT sobreutiliza conectores y pierde naturalidad [20]. Un conector por párrafo y solo si la relación lógica no es obvia (orientativo).
- **Párrafos del mismo largo, ritmo uniforme.** Bloques de tres o cuatro frases con la misma cadencia [17]. En noticias en inglés, las personas dispersan más la longitud de las frases y usan un vocabulario más variado que los LLM, que se agrupan entre 10 y 30 tokens [8].
- **Títulos con dos puntos o «X y Y».** «Impacto y perspectivas», «Desafíos y legado» [1]. El título «Concepto: subtítulo explicativo» lo citan los editores como tic, pero ninguna fuente revisada lo respalda (criterio del equipo). Un título nombra el contenido.
- **Listas con negrita en cada viñeta.** «**Eficiencia:** descripción…» [1][19]. Si es una lista, ítems paralelos sin rótulo; si hay explicación, párrafo.
- **Enumeraciones forzadas y falsos rangos.** «Diversos aspectos como A, B y C» (la guía de Wikipedia señala el «such as» que sugiere listas incompletas [1]) y «desde X hasta Y» sin que X e Y sean extremos de nada (criterio del equipo).
- **Pasiva e impersonal sin agente.** El «se» impersonal es norma en ingeniería; el defecto es ocultar quién decidió y cuándo («se decidió» sin acta) o encadenar nominalizaciones («la realización de la implementación de…») [1]. Nombrar al agente cuando importa; usar el verbo.
- **Hedging y atribuciones vagas.** «Podría decirse», «en muchos casos», «hasta cierto punto», «diversos estudios demuestran», «los expertos coinciden» [1][18]. Citar autor y año, o borrar.
- **Superlativos, adjetivos de relleno y tono promocional.** «Invaluable», «notable», «significativamente», «marca un hito», «representa un paso importante» [1]. Sin medida, fuera.
- **Entusiasmo y analogías tópicas.** «Como una máquina bien engrasada», «Imagina un mundo donde…» [18][22]. Un texto técnico no necesita animar al lector.

## 3. Formato y tipografía

- **Raya (—).** Es la marca más comentada y una de las más débiles: la frecuencia depende del modelo (de 0,0 a 9,1 por cada 1 000 palabras entre doce modelos y según la instrucción) y en casi todos persiste aunque se pida evitar el markdown [9]; además la usan muchos escritores [23]. Cuenta como parte del conjunto. En prosa técnica, comas, paréntesis o punto. Una por página como máximo (orientativo); nunca como golpe de efecto ni con espacios al estilo inglés.
- **Comillas.** La RAE recomienda las angulares « » en primera instancia y las inglesas “ ” solo dentro de otras [14]. Los chatbots mezclan rectas y tipográficas dentro de una misma respuesta [1]. Dejar solo « » y comprobarlo con grep.
- **Negritas.** Solo rótulos de tabla («Tabla 3.1.») y, como mucho, un término al definirlo. Repetir en negrita las mismas palabras imita el estilo de un README [1].
- **Emojis, ✓ y flechas** usados como viñeta o adorno de encabezado [1][22]. Ninguno.
- **Mayúsculas de título al estilo inglés.** «Arquitectura Implementada Frente a la Diseñada» [1]. En español, inicial y nombres propios; los capítulos en mayúsculas siguen la plantilla del documento, no la IA.
- **Viñetas donde iría prosa y tablas para lo que no es tabular.** Una lista se justifica si los ítems son paralelos y se consultan por separado. Evitar encabezados con un solo párrafo o que contienen solo otros encabezados [1].
- **Restos de markdown y de herramienta.** `**`, `##`, `---`, `[texto](url)`, `oaicite`, `turn0search0`, `[cite: 1]` [1]. Buscarlos antes de pasar a Word.
- **Cifras sin uniformar.** Mezclar «0.4» y «0,4», o «1,000» y «1 000», delata un pegado sin revisar (criterio del equipo: coma decimal y fecha dd/mm/aaaa, como el resto del documento).

## 4. Contenido

- **Generalidades sin dato.** «Mejora significativamente el aprendizaje», «tuvo gran aceptación» [1]. Una afirmación lleva cifra, unidad, fuente y versión o fecha.
- **Afirmaciones sin fuente o con atribución vaga.** Cada «según estudios» necesita autor, año y enlace [1].
- **Citas inventadas o mal atribuidas.** En un estudio de 636 referencias de 84 textos generados por ChatGPT, el 55 % (GPT-3.5) y el 18 % (GPT-4) eran inventadas, y entre las reales fallaban volumen, páginas o fecha en el 43 % y el 24 % [10]. Abrir cada DOI o URL, comprobar autor, año, título y páginas, y localizar en la fuente toda frase entre comillas. No atribuir una idea a quien no se ha leído.
- **Sin detalles propios.** El texto genérico describe «un videojuego educativo»; el propio dice `PhaseId.PhasesPerLevel = { 1, 3, 3 }`, «acta D05», «INC-97», «commit 359365e». La guía de Wikipedia toma como señal humana que quien escribe pueda explicar sus decisiones editoriales [1].
- **Tono uniforme y positivo.** Todo sale bien, nadie duda, nada se descarta [1]. El texto humano cuenta lo que falló; el capítulo 3 del OE3 lo hace: «La primera redacción de ese capítulo describía, sin embargo, componentes que el juego no tiene».
- **Ids y cifras que parecen correctos.** Un desplazamiento de numeración mal aplicado produce RNF que existen y no son los citados (CLAUDE.md). Comparar la cita con el nombre del requerimiento, no con su número.
- **Importancia inflada y listas de «reconocimientos».** Decir que algo es significativo en vez de mostrar qué cambió [1].
- **Conclusiones que repiten el resumen** y «trabajo futuro» genérico. Una conclusión responde al objetivo con un resultado verificable.

## 5. Qué hacer en su lugar

1. **El hecho primero.** Una frase, un dato verificable; la valoración, si hace falta, después y con medida.
2. **Cifras con procedencia.** «479,0 MB (rc2, 01/10/2026; `evidencias/build-rc2.md`)»: valor, unidad, versión, fecha y archivo.
3. **Citar por código.** Actas (`D05`), hallazgos (`INC-97`), tarjetas (`D07-2`), commits (`359365e`) y pruebas por su nombre (`GameFlow_RNF13_…`). Es lo que una IA sin acceso al repositorio no puede inventar bien.
4. **Una sola persona gramatical.** La que fije la guía de la facultad: impersonal («se verificó») o primera del plural («decidimos»). No alternar. Nombrar al agente cuando importa («Santiago decidió el 29/09, acta D10»).
5. **Variar el largo.** Frases de ocho palabras junto a otras de treinta; párrafos de una a siete frases. Si el contenido pide dos frases, dos.
6. **Conectores precisos y escasos.** «Porque», «aunque», «en cambio», «por tanto» donde hay una relación lógica que no se ve sola.
7. **Verbos concretos.** «Mide», «rechaza», «carga», «pesa» en lugar de «permite», «facilita», «garantiza».
8. **Un término por concepto.** Definirlo una vez y repetirlo. En un texto técnico alternar sinónimos («prueba», «test», «ensayo») confunde al lector; la «variación elegante» figura en la guía de Wikipedia como marca de textos antiguos de IA [1].
9. **Decir lo descartado y su porqué.** Cada decisión con su alternativa, su fecha y quién la tomó; cada limitación con su causa.
10. **IA como apoyo declarado.** Redactar desde las notas propias (actas, commits, resultados) y usar la IA para revisar. IEEE exige declarar el uso de contenido generado en los agradecimientos, con sistema, secciones y grado; para edición y gramática lo recomienda sin exigirlo [11]. Uniandes, Externado y la Católica de Pereira publicaron lineamientos entre 2024 y 2026, y la de Pereira exige identificar y declarar todo aporte generado por IA [12]. Verificar el reglamento de la propia universidad.
11. **Leer en voz alta** y preguntarse si cada párrafo se defendería en la sustentación sin releerlo [1].
12. **No deformar el texto a propósito.** Gramática correcta y registro formal no delatan por sí solos [1]; la solución es contenido propio, no errores añadidos.

## 6. Lista de comprobación final

- [ ] 1. El grep de abajo da cero coincidencias o cada una está justificada (un gerundio simultáneo es legítimo).
- [ ] 2. Cada cifra tiene unidad, versión o fecha y fuente (archivo, acta, prueba).
- [ ] 3. Cada referencia se abrió y se comparó con autor, año, título y páginas; cada cita textual se localizó en la fuente.
- [ ] 4. No hay más de una raya por página (orientativo) ni rayas con espacios al estilo inglés.
- [ ] 5. Todas las comillas son « »; no quedan rectas ni tipográficas inglesas.
- [ ] 6. Negritas solo en rótulos de tabla (el grep e) no devuelve listas enteras con «**Término:**» al inicio de cada viñeta).
- [ ] 7. Ningún párrafo termina en frase-moraleja y ningún subapartado termina resumiéndose.
- [ ] 8. Se contaron las tríadas «A, B y C»: cada una tiene de verdad tres elementos.
- [ ] 9. «No solo… sino» y «No es X, es Y» aparecen una vez como mucho en todo el documento (orientativo).
- [ ] 10. Tres párrafos al azar: largos de frase y de párrafo distintos entre sí.
- [ ] 11. Ningún gerundio de posterioridad ni de cierre con «destacando», «garantizando», «sentando».
- [ ] 12. Toda valoración («eficaz», «robusto», «mejora») lleva una medida al lado o se borró.
- [ ] 13. Títulos en minúscula salvo la inicial y los nombres propios, sin dos puntos ni forma «X y Y».
- [ ] 14. Cada decisión cita su acta, INC o commit y su porqué; cada limitación, su causa.
- [ ] 15. Los autores pueden defender cada párrafo sin releerlo, la coma decimal es uniforme y el uso de IA está declarado donde la norma lo exige.

```bash
export LC_ALL=C.UTF-8; F=claudeDocs/entregables/OE3/src/*.md   # o el archivo a revisar
# a) vocabulario y muletillas (revisar cada coincidencia: puede ser legítima)
grep -n -i -E '\b(cabe (destacar|señalar|mencionar|resaltar)|vale la pena (señalar|destacar|mencionar)|es (importante|fundamental|crucial|esencial|necesario|preciso) (señalar|destacar|mencionar|resaltar|subrayar|recordar|tener en cuenta)|en (resumen|conclusión|definitiva|síntesis)|para resumir|en última instancia|sin (lugar a )?dudas?( alguna)?|indudablemente|ciertamente|en el (panorama|mundo|contexto|escenario) actual|en la era (digital|actual)|en un mundo|hoy en día|(juega|juegan|jugar|desempeña|desempeñan) un papel (crucial|fundamental|clave|vital|esencial|central)|testimonio de|tapiz|paisaje|panorama|ecosistema|paradigma|holístic|sinergi|sinérgic|vibrante|robust[oa]s?\b|multifacétic|invaluable|inestimable|intrincad|meticulos|adentrar|sumergi|embarcar|desentra|ahondar|foment|potenci|empoder|apalanc|revolucion|transformador|innovador|de vanguardia|enriquecedor|inmersiv|en base a|a nivel de|y/o|aquí tienes|espero que (esto|te) |como modelo de lenguaje)' $F
# b) estructura: antítesis, gerundios de cierre, preguntas retóricas, anuncios vacíos
grep -n -i -E 'no (solo|sólo|solamente) .{1,90} sino (también|que|además)|\bno es [^,.;]{1,50}, (es|sino)\b|, (destacando|subrayando|resaltando|enfatizando|garantizando|asegurando|fomentando|reflejando|evidenciando|promoviendo|contribuyendo a|sentando)\b|^[^|]*[¿][^?]{3,60}[?] *$|\b(veamos|exploremos|dividámoslo|como se (mencionó|señaló) (anteriormente|previamente)|diversos estudios|los expertos)\b' $F
# c) conectores que abren frase (cada uno más de dos veces por página: reescribir)
grep -o -h -E '(^|[.:] )(Además|Asimismo|Adicionalmente|Por otro lado|Dicho esto|Ahora bien|Por lo tanto|En consecuencia|Sin embargo)' $F | sed -E 's/^[.:] //' | sort | uniq -c | sort -rn
# d) formato: rayas, comillas tipográficas, emojis, restos de markdown, títulos con dos puntos
grep -n -P '—|“|”|[\x{1F300}-\x{1FAFF}\x{2600}-\x{27BF}]|oaicite|turn0search|\[cite: ?\d|^#+ .*:' $F
# e) negritas: viñetas que abren con rótulo en negrita y párrafos con rótulo (contar por archivo; ver reglas de apartado 2 y 3)
grep -c -P '^\s*[-*] \*\*|^\*\*[^*]{2,60}[.:]\*\*' $F
```

## 7. Fuentes consultadas (todas el 06/10/2026)

Las páginas de la RAE, de Nature y del Washington Post devolvieron error 403 o redirección al descargarlas; de esas se usó lo que muestran los resultados de búsqueda y se marca «(vía búsqueda)». Los resúmenes de las demás los generó la herramienta de lectura, y las cifras se contrastaron con el resumen del propio autor cuando fue posible.

1. Wikipedia, WikiProject AI Cleanup. «Wikipedia:Signs of AI writing». https://en.wikipedia.org/wiki/Wikipedia:Signs_of_AI_writing
2. Kobak, González-Márquez, Horvát, Lause (2025). «Delving into LLM-assisted writing in biomedical publications through excess vocabulary». Science Advances. https://arxiv.org/abs/2406.07016 (versión editorial: https://www.science.org/doi/10.1126/sciadv.adt3813)
3. Kousha, Thelwall (2026). «How much are LLMs changing the language of academic papers after ChatGPT?». arXiv. https://arxiv.org/abs/2509.09596
4. Juzek, Ward (2025). «Why Does ChatGPT "Delve" So Much?». COLING. https://arxiv.org/abs/2412.11385
5. Juzek (2026). «AI-Associated Lexical Shifts Across 34 Languages». arXiv. https://arxiv.org/abs/2605.25358
6. Russell, Karpinska, Iyyer (2025). «People who frequently use ChatGPT for writing tasks are accurate and robust detectors of AI-generated text». ACL. https://aclanthology.org/2025.acl-long.267/
7. Liang, Yuksekgonul, Mao, Wu, Zou (2023). «GPT detectors are biased against non-native English writers». Patterns. https://www.cell.com/patterns/fulltext/S2666-3899(23)00130-7 (vía búsqueda)
8. Muñoz-Ortiz, Gómez-Rodríguez, Vilares (2024). «Contrasting Linguistic Patterns in Human and LLM-Generated News Text». Artificial Intelligence Review. https://pmc.ncbi.nlm.nih.gov/articles/PMC11422446/
9. Freeburg (2026). «The Last Fingerprint: How Markdown Training Shapes LLM Prose». Preprint arXiv. https://arxiv.org/abs/2603.27006
10. Walters, Wilder (2023). «Fabrication and errors in the bibliographic citations generated by ChatGPT». Scientific Reports 13, 14045. https://pmc.ncbi.nlm.nih.gov/articles/PMC10484980/
11. IEEE Author Center. «Submission and peer review policies» (apartado sobre contenido generado por IA). https://journals.ieeeauthorcenter.ieee.org/become-an-ieee-journal-author/publishing-ethics/guidelines-and-policies/submission-and-peer-review-policies/
12. Universidad de los Andes (2024), «Lineamientos para el uso de IA generativa»: https://secretariageneral.uniandes.edu.co/images/documents/lineamientos-uso-inteligencia-artificial-generativa-IAG-uniandes.pdf · Universidad Externado (2024): https://www.uexternado.edu.co/wp-content/uploads/2024/10/Lineamientos-para-el-uso-de-IA-1.pdf · Universidad Católica de Pereira (2026): https://www.ucp.edu.co/wp-content/uploads/2026/02/Lineamientos-de-integridad-academica-Codificado-1.pdf (los tres vía búsqueda)
13. RAE. «Gerundio de posterioridad», Libro de estilo de la justicia. https://www.rae.es/libro-estilo-justicia/las-palabras-y-sus-grupos-problemas-y-actuaciones/gerundio/usos-incorrectos/gerundio-de-posterioridad (vía búsqueda)
14. RAE. «Las comillas». https://www.rae.es/buen-uso-español/las-comillas y https://www.rae.es/dpd/comillas (vía búsqueda)
15. RAE. Diccionario panhispánico de dudas, «base» y «nivel». https://www.rae.es/dpd/base · https://www.rae.es/dpd/nivel (vía búsqueda)
16. Applesfera. «Te han pillado: las frases y palabras que hacen evidente que utilizaste ChatGPT». https://www.applesfera.com/curiosidades/te-han-pillado-palabras-frases-que-hacen-evidente-que-utilizaste-chatgpt-lugar-pensar-a-apple (lista de prensa, probablemente traducida del inglés)
17. Girón, B. «Las palabras y frases que repite ChatGPT». https://borjagiron.com/palabras-frases-repite-chatgpt/ (blog)
18. Lencarpio, L. O. «11 señales de que ChatGPT escribió tu texto». https://luisorlandolencarpio.substack.com/p/11-senales-de-que-chatgpt-escribio (blog)
19. De Haro, J. J. (06/06/2023). «Cómo detectar textos escritos por ChatGPT». Bilateria. https://educacion.bilateria.org/como-detectar-textos-escritos-por-chatgpt
20. Salas Acuña, Amador Solano (2023). «Usos de ChatGPT para la revisión de textos académicos». Revista Innovaciones Educativas. https://portal.amelica.org/ameli/journal/428/4284911009/html/
21. Observatorio GATE, Universidad Politécnica de Madrid (18/09/2026). «Detectores de IA: ¿funcionan realmente?». https://blogs.upm.es/observatoriogate/2026/09/18/detectores-de-ia-funcionan-realmente-lo-que-dicen-los-estudios-de-2026/ (cita estudios de 2026 que no se verificaron; solo respalda la cautela con los detectores)
22. «AI Writing Tropes to Avoid» (tropes.fyi, lista comunitaria sin revisión por pares). https://gist.github.com/ossa-ma/f3baa9d25154c33095e22272c631f5a1
23. The Washington Post (09/04/2025). «Some think the em dash is a "ChatGPT hyphen." Writers disagree.». https://www.washingtonpost.com/technology/2025/04/09/ai-em-dash-writing-punctuation-chatgpt/ (solo titular, vía búsqueda)
24. FundéuRAE (01/04/2024). «"y/o", fórmula innecesaria». https://www.infobae.com/america/agencias/2024/04/01/fundeurae-yo-formula-innecesaria/ (vía búsqueda)
