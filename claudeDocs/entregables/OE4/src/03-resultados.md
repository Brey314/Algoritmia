# 3. RESULTADOS

Todos los veredictos de este capítulo son los de rc3 (apartado 2.6). Donde un caso se repitió después de una corrección, vale el veredicto del candidato más reciente; la primera pasada se conserva en el registro de trabajo y se resume en el apartado 3.6.

## 3.1 Veredictos por sesión

**Tabla 3.1.** Veredictos de rc3 por sesión. La eficacia es (P + PD) / (P + PD + F).

{{TABLA_RESULTADOS_SESION}}

PENDIENTE-PASADA: comentario de las sesiones con casos F, B o NA, con el defecto o la razón de cada uno.

## 3.2 Veredictos por etiqueta

El indicador de eficacia nombra los botones, los sonidos y los saltos de nivel. La Tabla 3.2 reparte los mismos casos por etiqueta para leerlo así.

**Tabla 3.2.** Veredictos de rc3 por etiqueta.

{{TABLA_RESULTADOS_ETIQUETA}}

## 3.3 Indicadores del objetivo

**Tabla 3.3.** Los dos indicadores del OE4 sobre rc3.

| Indicador | Meta | Resultado en rc3 |
|---|---|---|
| Eficacia de las pruebas funcionales | 90 % o más | PENDIENTE-CIFRA |
| Eficacia en las etiquetas `NAV`, `BOT` y `SON` | 90 % o más cada una | PENDIENTE-CIFRA |
| RF de prioridad Alta con algún F Bloqueante o Mayor abierto | Ninguno de los 45 | PENDIENTE-CIFRA |

## 3.4 Recorridos completos del juego

El caso `PF-RNF13-01` exige recorrer el juego entero, de la pantalla de inicio a los créditos y sin omitir ninguna narrativa, sin bloqueos, cierres inesperados ni estados irrecuperables. Lo ejecutan dos recorridos del arnés, uno en ventana de 1920 × 1080 y otro a pantalla completa, y uno de Santiago con cronómetro (apartado 5.2, comprobación H8). La duración de los recorridos del arnés incluye la latencia de cada llamada y las capturas de control, y no mide a un estudiante; la referencia de 20 a 40 minutos del OE1 se contrasta con el recorrido de Santiago.

**Tabla 3.4.** Recorridos completos sobre rc3.

| Recorrido | Condición | Duración | Fases en el JSON | `Exception` en `Player.log` |
|---|---|---|---|---|
| Arnés 1 | Ventana de 1920 × 1080, perfil nuevo | PENDIENTE-CIFRA | PENDIENTE-CIFRA | PENDIENTE-CIFRA |
| Arnés 2 | Pantalla completa, perfil nuevo | PENDIENTE-CIFRA | PENDIENTE-CIFRA | PENDIENTE-CIFRA |
| Santiago | Pantalla completa, perfil nuevo, cronómetro | [Pendiente: duración y incidencias que anote Santiago en H8] | [Pendiente: Santiago] | [Pendiente: Santiago] |

## 3.5 Requerimientos no funcionales medidos en el ejecutable

Los tres presupuestos duros del proyecto se miden sobre el ejecutable y no se estiman. Las cargas salen de la línea `RNF-04` que `SceneLoader` deja en `Player.log`, tomada tres veces por escena en el equipo 1; el tiempo incluye los dos medios fundidos de 0,4 s, por lo que es una medida conservadora. La memoria sale del muestreo de `WorkingSet64` y `PrivateMemorySize64` del proceso en todas las sesiones. El tamaño es el de la carpeta entregable sin `Datos/` ni las carpetas `*_DoNotShip`.

**Tabla 3.5.** Presupuestos de rendimiento y de paquete en rc3.

| Requerimiento | Umbral | Valor medido | Dónde se dio | Veredicto |
|---|---|---|---|---|
| RNF-04, carga de cualquier escena | Menos de 10 s | PENDIENTE-CIFRA (peor de las 12 escenas) | PENDIENTE-CIFRA | PENDIENTE-CIFRA |
| RNF-05, memoria | Menos de 2 GB | PENDIENTE-CIFRA (máximo de `WorkingSet64` y de `PrivateMemorySize64`) | PENDIENTE-CIFRA | PENDIENTE-CIFRA |
| RNF-06, tamaño del paquete | Menos de 500 MB | PENDIENTE-CIFRA | Carpeta entregable | PENDIENTE-CIFRA |

Para comparar: rc2 pesó 479,0 MB (01/10/2026), 21 MB por debajo del tope. Las cifras de RNF-04 y RNF-05 en un segundo equipo dependen de la sesión de Santiago (apartado 5.2, comprobaciones H3 y H5). Las otras mediciones sobre el ejecutable (RNF-07, RNF-08, RNF-10 y RNF-11) se reportan en el Anexo A con el veredicto de su caso.

## 3.6 Defectos

**Tabla 3.6.** Defectos hallados, con la severidad y el estado en rc3.

| Defecto | Caso | Severidad | Hallado en | Estado en rc3 | Decisión |
|---|---|---|---|---|---|
| PENDIENTE-CIFRA | PENDIENTE-CIFRA | PENDIENTE-CIFRA | PENDIENTE-CIFRA | PENDIENTE-CIFRA | PENDIENTE-CIFRA |

PENDIENTE-PASADA: resumen de la primera pasada (rc1) y de cómo cada corrección se reverificó en rc2 y rc3, con el número de defectos por severidad.
