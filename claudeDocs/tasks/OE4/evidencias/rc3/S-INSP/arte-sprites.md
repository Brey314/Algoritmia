# Verificación de sprites (`arte_check.py sprites`)

- Comando: `arte_check.py sprites --only-fail --out claudeDocs/tasks/OE4/evidencias/rc3/S-INSP/arte-sprites.md`
- Carpeta: `C:/Dev/Algoritmia/Assets/Game/Art` · fecha: 10/10/2026
- PNG en disco: 320 · excluidos (cuadros fuego/humo con nombre de entrega): 67 · analizados: 253
- Halo = píxeles 0<α<64 de tono verde/magenta (exceso ≥ 40) cuyos vecinos opacos no comparten el tinte; intenso = canal dominante ≥ 130. Borde tocado = algún píxel con α≥32 en el borde del lienzo (informativo). .meta esperado = Sprite · Single · 100 PPU · Bilinear.

## Resumen

| Comprobación | Resultado |
|---|---|
| Nombre §15.4 | 253 cumplen · **0 fallan** |
| Canal alfa (todo salvo `Environments/` debe tener transparencia) | 253 cumplen · **0 sin transparencia** |
| Halo de croma INTENSO (nivel ≥ 130, ≥ 10 px) | **9 archivos** · 268 px intensos de 607 marcados |
| Halo TENUE o suelto (verde oscuro o pocos píxeles; informativo) | 45 archivos · 134 px |
| Borde tocado | 118 archivos (informativo) |
| .meta | 241 como se espera · **12 desviados** · 0 sin .meta |
| `Generate Physics Shape` encendido (§15.2 pide apagado; informativo) | 253 de 253 |
| Compresión / tamaño máximo de importación | sin comprimir / 256: 85, sin comprimir / 4096: 168 |

## Hallazgos

### Halo de croma intenso (9)

- `Characters/Boy/Expresiones/char_nino_cara_base.png`: 51 px intensos de 78 marcados (α máx 25)
- `Characters/Boy/Perfil/char_nino_perfil_cara_base.png`: 48 px intensos de 102 marcados (α máx 18)
- `Characters/Mother/Expresiones/char_mama_cara_base.png`: 38 px intensos de 87 marcados (α máx 12)
- `Characters/Mother/Expresiones/char_mama_boca_a.png`: 32 px intensos de 76 marcados (α máx 19)
- `Characters/Mother/Perfil/char_mama_perfil_cara_base.png`: 27 px intensos de 76 marcados (α máx 19)
- `Characters/Girl/Perfil/char_nina_perfil_cara_base.png`: 23 px intensos de 66 marcados (α máx 10)
- `Characters/Girl/Expresiones/char_nina_boca_a.png`: 17 px intensos de 41 marcados (α máx 11)
- `Characters/Girl/Expresiones/char_nina_cara_base.png`: 17 px intensos de 45 marcados (α máx 10)
- `Characters/Mother/Expresiones/char_mama_boca_preocupacion.png`: 15 px intensos de 36 marcados (α máx 11)

### Halo tenue o suelto (informativo) (45)

- `Props/Wheel/prop_n2_laberinto_obstaculo.png`: 13 px, 0 intensos (α máx 6)
- `Props/River/prop_n3_amarre.png`: 11 px, 0 intensos (α máx 32)
- `Characters/Algoritm/Expresiones/char_algoritm_ojos_concentracion.png`: 8 px, 7 intensos (α máx 50)
- `Characters/Algoritm/Expresiones/char_algoritm_ojos_neutra.png`: 8 px, 7 intensos (α máx 50)
- `Characters/Algoritm/Expresiones/char_algoritm_ojos_parpadeo_cerrado.png`: 8 px, 7 intensos (α máx 50)
- `Characters/Algoritm/Expresiones/char_algoritm_ojos_preocupacion.png`: 8 px, 7 intensos (α máx 50)
- `Characters/Algoritm/Expresiones/char_algoritm_ojos_sorpresa.png`: 8 px, 7 intensos (α máx 50)
- `Characters/Girl/Perfil/char_nina_perfil_ojos_neutra.png`: 7 px, 0 intensos (α máx 6)
- `Characters/Boy/Frontal/char_nino_parte_torso.png`: 6 px, 3 intensos (α máx 17)
- `Characters/Algoritm/char_algoritm_fuego_retrato_base.png`: 4 px, 0 intensos (α máx 6)
- `Characters/Boy/char_nino_retrato_base.png`: 3 px, 1 intensos (α máx 5)
- `Characters/Boy/char_nino_retrato_neutra.png`: 3 px, 1 intensos (α máx 5)
- `Characters/Boy/Perfil/char_nino_perfil_ojos_neutra.png`: 3 px, 1 intensos (α máx 6)
- `Characters/Boy/Expresiones/char_nino_ojos_concentracion.png`: 2 px, 1 intensos (α máx 6)
- `Characters/Boy/Expresiones/char_nino_ojos_neutra.png`: 2 px, 1 intensos (α máx 6)
- `Characters/Boy/Expresiones/char_nino_ojos_preocupacion.png`: 2 px, 1 intensos (α máx 6)
- `Characters/Boy/Expresiones/char_nino_ojos_sorpresa.png`: 2 px, 1 intensos (α máx 6)
- `Characters/Girl/Expresiones/char_nina_ojos_concentracion.png`: 2 px, 0 intensos (α máx 6)
- `Characters/Girl/Expresiones/char_nina_ojos_preocupacion.png`: 2 px, 0 intensos (α máx 6)
- `Characters/Girl/Expresiones/char_nina_ojos_sorpresa.png`: 2 px, 0 intensos (α máx 6)
- `Characters/Girl/Perfil/char_nina_perfil_ojos_parpadeo_cerrado.png`: 2 px, 2 intensos (α máx 6)
- `Characters/Mother/Perfil/char_mama_perfil_ojos_neutra.png`: 2 px, 1 intensos (α máx 12)
- `Props/Wheel/prop_n2_carretilla_e4.png`: 2 px, 1 intensos (α máx 2)
- `Props/Wheel/prop_n2_tronco_a.png`: 2 px, 0 intensos (α máx 3)
- `UI/Common/ui_papelera.png`: 2 px, 0 intensos (α máx 4)
- `Characters/Algoritm/char_algoritm_n1_fuego_reposo.png`: 1 px, 0 intensos (α máx 3)
- `Characters/Father/Frontal/char_papa_parte_antepierna_izq.png`: 1 px, 1 intensos (α máx 1)
- `Characters/Father/Perfil/char_papa_perfil_antebrazo_cercano.png`: 1 px, 1 intensos (α máx 1)
- `Characters/Father/Perfil/char_papa_perfil_antepierna_cercana.png`: 1 px, 1 intensos (α máx 1)
- `Characters/Girl/Expresiones/char_nina_boca_0.png`: 1 px, 1 intensos (α máx 3)
- `Characters/Girl/Expresiones/char_nina_boca_concentracion.png`: 1 px, 0 intensos (α máx 5)
- `Characters/Girl/Expresiones/char_nina_ojos_neutra.png`: 1 px, 0 intensos (α máx 6)
- `Characters/Girl/Frontal/char_nina_parte_antepierna_der.png`: 1 px, 1 intensos (α máx 1)
- `Characters/Girl/Perfil/char_nina_perfil_antebrazo_cercano.png`: 1 px, 0 intensos (α máx 4)
- `Characters/Girl/Perfil/char_nina_perfil_antepierna_cercana.png`: 1 px, 1 intensos (α máx 1)
- `Characters/Girl/Perfil/char_nina_perfil_boca_0.png`: 1 px, 0 intensos (α máx 6)
- `Characters/Mother/Expresiones/char_mama_boca_0.png`: 1 px, 0 intensos (α máx 4)
- `Characters/Mother/Expresiones/char_mama_boca_concentracion.png`: 1 px, 0 intensos (α máx 2)
- `Characters/Mother/Expresiones/char_mama_ojos_concentracion.png`: 1 px, 1 intensos (α máx 4)
- `Characters/Mother/Expresiones/char_mama_ojos_neutra.png`: 1 px, 1 intensos (α máx 4)
- `Characters/Mother/Expresiones/char_mama_ojos_preocupacion.png`: 1 px, 1 intensos (α máx 4)
- `Characters/Mother/Expresiones/char_mama_ojos_sorpresa.png`: 1 px, 1 intensos (α máx 4)
- `Characters/Mother/Perfil/char_mama_perfil_antepierna_cercana.png`: 1 px, 1 intensos (α máx 1)
- `Props/Wheel/prop_n2_carretilla_e2.png`: 1 px, 0 intensos (α máx 5)
- `Props/Wheel/prop_n2_carretilla_e5.png`: 1 px, 0 intensos (α máx 2)

### .meta desviado (12)

- `Environments/Narrative/env_enlace_n2.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_herramienta_a.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_herramienta_b.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_herramienta_c.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_piedra_a.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_piedra_b.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_piedra_c.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_piedra_d.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_planta_a.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_planta_b.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_planta_c.png`: tipo 8 · Multiple · 100 PPU · Bilinear
- `Props/Wheel/prop_n2_tronco_a.png`: tipo 8 · Multiple · 100 PPU · Bilinear

### Borde tocado (informativo) (118)

- `Characters/Algoritm/char_algoritm_fuego_retrato_base.png`: abj
- `Characters/Algoritm/char_algoritm_gota_retrato_base.png`: izq, der, abj
- `Characters/Algoritm/char_algoritm_rueda_retrato_base.png`: izq, der, abj
- `Characters/Algoritm/Frontal/char_algoritm_fuego_parte_antebrazo_der.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_fuego_parte_antebrazo_izq.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_fuego_parte_brazo_der.png`: izq, der, arr
- `Characters/Algoritm/Frontal/char_algoritm_fuego_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_fuego_parte_pierna_der.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_fuego_parte_pierna_izq.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_fuego_parte_torso.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_gota_parte_antebrazo_der.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_gota_parte_antebrazo_izq.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_gota_parte_brazo_der.png`: izq, der, arr
- `Characters/Algoritm/Frontal/char_algoritm_gota_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_gota_parte_pierna_der.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_gota_parte_pierna_izq.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_gota_parte_torso.png`: der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_rueda_parte_antebrazo_der.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_rueda_parte_antebrazo_izq.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_rueda_parte_brazo_der.png`: izq, der, arr
- `Characters/Algoritm/Frontal/char_algoritm_rueda_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_rueda_parte_pierna_der.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_rueda_parte_pierna_izq.png`: izq, der, arr, abj
- `Characters/Algoritm/Frontal/char_algoritm_rueda_parte_torso.png`: izq, der, arr, abj
- `Characters/Boy/char_nino_retrato_base.png`: abj
- `Characters/Boy/char_nino_retrato_neutra.png`: abj
- `Characters/Boy/Frontal/char_nino_parte_antebrazo_der.png`: izq, der, arr, abj
- `Characters/Boy/Frontal/char_nino_parte_antebrazo_izq.png`: izq, der, abj
- `Characters/Boy/Frontal/char_nino_parte_antepierna_der.png`: izq, der, arr, abj
- `Characters/Boy/Frontal/char_nino_parte_antepierna_izq.png`: izq, der, arr, abj
- `Characters/Boy/Frontal/char_nino_parte_brazo_der.png`: izq, der, arr, abj
- `Characters/Boy/Frontal/char_nino_parte_brazo_izq.png`: izq, der, arr
- `Characters/Boy/Frontal/char_nino_parte_cabeza.png`: der, arr, abj
- `Characters/Boy/Frontal/char_nino_parte_pierna_der.png`: izq, der, arr, abj
- `Characters/Boy/Frontal/char_nino_parte_pierna_izq.png`: izq, der, arr, abj
- `Characters/Boy/Frontal/char_nino_parte_torso.png`: izq, der, arr, abj
- `Characters/Boy/Perfil/char_nino_perfil_antebrazo_cercano.png`: izq, der, arr
- `Characters/Boy/Perfil/char_nino_perfil_antebrazo_lejano.png`: izq, der, arr
- `Characters/Boy/Perfil/char_nino_perfil_antepierna_cercana.png`: izq, arr
- `Characters/Boy/Perfil/char_nino_perfil_antepierna_lejana.png`: izq, arr
- `Characters/Boy/Perfil/char_nino_perfil_brazo_cercano.png`: izq, der, arr, abj
- `Characters/Boy/Perfil/char_nino_perfil_brazo_lejano.png`: izq, der, abj
- `Characters/Boy/Perfil/char_nino_perfil_cabeza.png`: izq, der, arr
- `Characters/Boy/Perfil/char_nino_perfil_pierna_cercana.png`: izq, der, abj
- `Characters/Boy/Perfil/char_nino_perfil_pierna_lejana.png`: izq, der, abj
- `Characters/Boy/Perfil/char_nino_perfil_torso.png`: izq, der, arr, abj
- `Characters/Father/char_papa_retrato_base.png`: abj
- `Characters/Father/char_papa_retrato_neutra.png`: abj
- `Characters/Father/Frontal/char_papa_parte_antebrazo_der.png`: izq, der, arr, abj
- `Characters/Father/Frontal/char_papa_parte_antebrazo_izq.png`: izq, der, arr, abj
- `Characters/Father/Frontal/char_papa_parte_antepierna_der.png`: izq, der, arr, abj
- `Characters/Father/Frontal/char_papa_parte_antepierna_izq.png`: izq, der, arr, abj
- `Characters/Father/Frontal/char_papa_parte_brazo_der.png`: izq, der, arr, abj
- `Characters/Father/Frontal/char_papa_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Father/Frontal/char_papa_parte_cabeza.png`: izq, der, arr, abj
- `Characters/Father/Frontal/char_papa_parte_pierna_der.png`: izq, der, arr, abj
- `Characters/Father/Frontal/char_papa_parte_pierna_izq.png`: izq, der, arr, abj
- `Characters/Father/Frontal/char_papa_parte_torso.png`: izq, der, arr, abj
- `Characters/Father/Perfil/char_papa_perfil_antebrazo_cercano.png`: izq, der
- `Characters/Father/Perfil/char_papa_perfil_antebrazo_lejano.png`: izq, der, arr
- `Characters/Father/Perfil/char_papa_perfil_antepierna_cercana.png`: izq, der, abj
- `Characters/Father/Perfil/char_papa_perfil_antepierna_lejana.png`: izq
- `Characters/Father/Perfil/char_papa_perfil_brazo_cercano.png`: der, arr
- `Characters/Father/Perfil/char_papa_perfil_brazo_lejano.png`: der, arr
- `Characters/Father/Perfil/char_papa_perfil_cabeza.png`: izq, der, arr, abj
- `Characters/Father/Perfil/char_papa_perfil_pierna_cercana.png`: der
- `Characters/Father/Perfil/char_papa_perfil_pierna_lejana.png`: abj
- `Characters/Father/Perfil/char_papa_perfil_torso.png`: izq, der, arr
- `Characters/Girl/char_nina_retrato_base.png`: izq, abj
- `Characters/Girl/char_nina_retrato_neutra.png`: izq, abj
- `Characters/Girl/Frontal/char_nina_parte_antebrazo_der.png`: izq, der, arr, abj
- `Characters/Girl/Frontal/char_nina_parte_antebrazo_izq.png`: izq, der, arr, abj
- `Characters/Girl/Frontal/char_nina_parte_antepierna_der.png`: izq, der, arr, abj
- `Characters/Girl/Frontal/char_nina_parte_antepierna_izq.png`: izq, der, arr, abj
- `Characters/Girl/Frontal/char_nina_parte_brazo_der.png`: izq, der, arr, abj
- `Characters/Girl/Frontal/char_nina_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Girl/Frontal/char_nina_parte_cabeza.png`: izq, arr, abj
- `Characters/Girl/Frontal/char_nina_parte_pierna_der.png`: izq, der, arr, abj
- `Characters/Girl/Frontal/char_nina_parte_pierna_izq.png`: izq, der, arr, abj
- `Characters/Girl/Frontal/char_nina_parte_torso.png`: izq, der, arr, abj
- `Characters/Girl/Perfil/char_nina_perfil_antebrazo_cercano.png`: izq, der, abj
- `Characters/Girl/Perfil/char_nina_perfil_antebrazo_lejano.png`: izq, arr, abj
- `Characters/Girl/Perfil/char_nina_perfil_antepierna_cercana.png`: izq, der, arr, abj
- `Characters/Girl/Perfil/char_nina_perfil_antepierna_lejana.png`: izq, der, arr, abj
- `Characters/Girl/Perfil/char_nina_perfil_brazo_cercano.png`: izq, der, abj
- `Characters/Girl/Perfil/char_nina_perfil_brazo_lejano.png`: izq, arr, abj
- `Characters/Girl/Perfil/char_nina_perfil_cabeza.png`: der, arr
- `Characters/Girl/Perfil/char_nina_perfil_pierna_cercana.png`: izq, der, arr, abj
- `Characters/Girl/Perfil/char_nina_perfil_pierna_lejana.png`: izq, der, arr, abj
- `Characters/Girl/Perfil/char_nina_perfil_torso.png`: der, abj
- `Characters/Mother/char_mama_retrato_base.png`: abj
- `Characters/Mother/char_mama_retrato_neutra.png`: abj
- `Characters/Mother/Frontal/char_mama_parte_antebrazo_der.png`: izq, der, arr, abj
- `Characters/Mother/Frontal/char_mama_parte_antebrazo_izq.png`: izq, der, arr, abj
- `Characters/Mother/Frontal/char_mama_parte_antepierna_der.png`: izq, der, arr, abj
- `Characters/Mother/Frontal/char_mama_parte_antepierna_izq.png`: izq, der, arr, abj
- `Characters/Mother/Frontal/char_mama_parte_brazo_der.png`: izq, der, arr, abj
- `Characters/Mother/Frontal/char_mama_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Mother/Frontal/char_mama_parte_cabeza.png`: izq, der, arr, abj
- `Characters/Mother/Frontal/char_mama_parte_pierna_der.png`: izq, der, arr, abj
- `Characters/Mother/Frontal/char_mama_parte_pierna_izq.png`: izq, der, arr, abj
- `Characters/Mother/Frontal/char_mama_parte_torso.png`: izq, der, arr, abj
- `Characters/Mother/Perfil/char_mama_perfil_antebrazo_cercano.png`: izq, der
- `Characters/Mother/Perfil/char_mama_perfil_antebrazo_lejano.png`: izq, der, arr, abj
- `Characters/Mother/Perfil/char_mama_perfil_antepierna_cercana.png`: izq, abj
- `Characters/Mother/Perfil/char_mama_perfil_antepierna_lejana.png`: izq, abj
- `Characters/Mother/Perfil/char_mama_perfil_brazo_cercano.png`: arr
- `Characters/Mother/Perfil/char_mama_perfil_brazo_lejano.png`: izq, der, abj
- `Characters/Mother/Perfil/char_mama_perfil_cabeza.png`: izq, der, arr, abj
- `Characters/Mother/Perfil/char_mama_perfil_pierna_cercana.png`: izq, arr, abj
- `Characters/Mother/Perfil/char_mama_perfil_pierna_lejana.png`: izq, arr, abj
- `Characters/Mother/Perfil/char_mama_perfil_torso.png`: izq, der
- `Props/River/prop_n3_vela_silueta.png`: arr
- `Props/Wheel/prop_n2_tronco_a.png`: arr
- `Props/Wheel/prop_n2_tronco_textura.png`: izq, arr, abj
- `UI/Common/ui_boton.png`: izq, der, arr, abj
- `UI/Common/ui_circulo.png`: izq, der, arr, abj
- `UI/Common/ui_panel.png`: izq, der, arr, abj

## Tabla

| Archivo | px | Nombre | Alfa | Halo | Borde | .meta |
|---|---|---|---|---|---|---|
| `Characters/Boy/Expresiones/char_nino_cara_base.png` | 256×179 | OK | RGBA · 86 % transp. | 78 (51 int., α≤25) | — | ok |
| `Characters/Boy/Perfil/char_nino_perfil_cara_base.png` | 155×168 | OK | RGBA · 81 % transp. | 102 (48 int., α≤18) | — | ok |
| `Characters/Girl/Expresiones/char_nina_boca_a.png` | 256×160 | OK | RGBA · 84 % transp. | 41 (17 int., α≤11) | — | ok |
| `Characters/Girl/Expresiones/char_nina_cara_base.png` | 256×160 | OK | RGBA · 85 % transp. | 45 (17 int., α≤10) | — | ok |
| `Characters/Girl/Perfil/char_nina_perfil_cara_base.png` | 139×138 | OK | RGBA · 81 % transp. | 66 (23 int., α≤10) | — | ok |
| `Characters/Mother/Expresiones/char_mama_boca_a.png` | 256×144 | OK | RGBA · 85 % transp. | 76 (32 int., α≤19) | — | ok |
| `Characters/Mother/Expresiones/char_mama_boca_preocupacion.png` | 256×144 | OK | RGBA · 91 % transp. | 36 (15 int., α≤11) | — | ok |
| `Characters/Mother/Expresiones/char_mama_cara_base.png` | 256×144 | OK | RGBA · 88 % transp. | 87 (38 int., α≤12) | — | ok |
| `Characters/Mother/Perfil/char_mama_perfil_cara_base.png` | 140×128 | OK | RGBA · 75 % transp. | 76 (27 int., α≤19) | — | ok |
| `Environments/Narrative/env_enlace_n2.png` | 1920×1080 | OK | RGBA · opaco (env) | 0 | n/a | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_herramienta_a.png` | 256×256 | OK | RGBA · 78 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_herramienta_b.png` | 256×256 | OK | RGBA · 78 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_herramienta_c.png` | 256×256 | OK | RGBA · 85 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_piedra_a.png` | 256×256 | OK | RGBA · 63 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_piedra_b.png` | 256×256 | OK | RGBA · 63 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_piedra_c.png` | 256×256 | OK | RGBA · 63 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_piedra_d.png` | 256×256 | OK | RGBA · 58 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_planta_a.png` | 256×256 | OK | RGBA · 74 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_planta_b.png` | 256×256 | OK | RGBA · 74 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_planta_c.png` | 256×256 | OK | RGBA · 74 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_tronco_a.png` | 256×256 | OK | RGBA · 22 % transp. | 2 (0 int., α≤3) | arr | 8/Multiple/100/Bilinear |
