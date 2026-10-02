# Verificación de sprites (`arte_check.py sprites`)

- Comando: `arte_check.py sprites --out sprites.md`
- Carpeta: `C:/Dev/Algoritmia/Assets/Game/Art` · fecha: 01/10/2026
- PNG en disco: 163 · excluidos (cuadros fuego/humo con nombre de entrega): 67 · analizados: 96
- Halo = píxeles 0<α<64 de tono verde/magenta (exceso ≥ 40) cuyos vecinos opacos no comparten el tinte; intenso = canal dominante ≥ 130. Borde tocado = algún píxel con α≥32 en el borde del lienzo (informativo). .meta esperado = Sprite · Single · 100 PPU · Bilinear.

## Resumen

| Comprobación | Resultado |
|---|---|
| Nombre §15.4 | 96 cumplen · **0 fallan** |
| Canal alfa (todo salvo `Environments/` debe tener transparencia) | 96 cumplen · **0 sin transparencia** |
| Halo de croma INTENSO (nivel ≥ 130, ≥ 10 px) | **0 archivos** · 0 px intensos de 0 marcados |
| Halo TENUE o suelto (verde oscuro o pocos píxeles; informativo) | 27 archivos · 8608 px |
| Borde tocado | 30 archivos (informativo) |
| .meta | 84 como se espera · **12 desviados** · 0 sin .meta |
| `Generate Physics Shape` encendido (§15.2 pide apagado; informativo) | 96 de 96 |
| Compresión / tamaño máximo de importación | sin comprimir / 4096: 96 |

## Hallazgos

### Halo tenue o suelto (informativo) (27)

- `Characters/Girl/char_nina_parte_torso.png`: 1529 px, 0 intensos (α máx 63)
- `Characters/Boy/char_nino_parte_torso.png`: 1175 px, 0 intensos (α máx 63)
- `Characters/Father/char_papa_parte_torso.png`: 909 px, 0 intensos (α máx 63)
- `Characters/Mother/char_mama_parte_torso.png`: 904 px, 0 intensos (α máx 63)
- `Characters/Boy/char_nino_parte_brazo_izq.png`: 447 px, 0 intensos (α máx 63)
- `Characters/Father/char_papa_parte_brazo_izq.png`: 419 px, 0 intensos (α máx 63)
- `Characters/Boy/char_nino_parte_brazo_der.png`: 401 px, 0 intensos (α máx 63)
- `Characters/Father/char_papa_parte_brazo_der.png`: 397 px, 0 intensos (α máx 63)
- `Characters/Girl/char_nina_parte_brazo_izq.png`: 395 px, 0 intensos (α máx 63)
- `Characters/Girl/char_nina_parte_brazo_der.png`: 390 px, 0 intensos (α máx 63)
- `Characters/Mother/char_mama_parte_brazo_der.png`: 335 px, 0 intensos (α máx 63)
- `Characters/Mother/char_mama_parte_brazo_izq.png`: 324 px, 0 intensos (α máx 63)
- `Characters/Girl/char_nina_parte_pierna_der.png`: 192 px, 0 intensos (α máx 63)
- `Characters/Girl/char_nina_parte_pierna_izq.png`: 177 px, 0 intensos (α máx 63)
- `Characters/Father/char_papa_parte_pierna_der.png`: 117 px, 0 intensos (α máx 63)
- `Characters/Mother/char_mama_parte_pierna_izq.png`: 114 px, 0 intensos (α máx 63)
- `Characters/Father/char_papa_parte_pierna_izq.png`: 113 px, 0 intensos (α máx 63)
- `Characters/Mother/char_mama_parte_pierna_der.png`: 100 px, 0 intensos (α máx 63)
- `Characters/Boy/char_nino_parte_pierna_der.png`: 70 px, 0 intensos (α máx 63)
- `Characters/Boy/char_nino_parte_pierna_izq.png`: 68 px, 0 intensos (α máx 63)
- `Props/Wheel/prop_n2_laberinto_obstaculo.png`: 13 px, 0 intensos (α máx 6)
- `Props/River/prop_n3_amarre.png`: 11 px, 0 intensos (α máx 32)
- `Props/Wheel/prop_n2_carretilla_e4.png`: 2 px, 1 intensos (α máx 2)
- `Props/Wheel/prop_n2_tronco_a.png`: 2 px, 0 intensos (α máx 3)
- `UI/Common/ui_papelera.png`: 2 px, 0 intensos (α máx 4)
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

### Borde tocado (informativo) (30)

- `Characters/Boy/char_nino_parte_brazo_der.png`: izq, der, arr, abj
- `Characters/Boy/char_nino_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Boy/char_nino_parte_pierna_der.png`: arr, abj
- `Characters/Boy/char_nino_parte_pierna_izq.png`: arr, abj
- `Characters/Boy/char_nino_parte_torso.png`: izq, der, arr, abj
- `Characters/Boy/char_nino_retrato_neutra.png`: izq, der, abj
- `Characters/Father/char_papa_parte_brazo_der.png`: izq, der, arr, abj
- `Characters/Father/char_papa_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Father/char_papa_parte_pierna_der.png`: izq, der, arr, abj
- `Characters/Father/char_papa_parte_pierna_izq.png`: izq, der, arr, abj
- `Characters/Father/char_papa_parte_torso.png`: izq, arr, abj
- `Characters/Father/char_papa_retrato_neutra.png`: izq, der, abj
- `Characters/Girl/char_nina_parte_brazo_der.png`: izq, der, arr, abj
- `Characters/Girl/char_nina_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Girl/char_nina_parte_pierna_der.png`: izq, arr, abj
- `Characters/Girl/char_nina_parte_pierna_izq.png`: der, arr, abj
- `Characters/Girl/char_nina_parte_torso.png`: arr, abj
- `Characters/Girl/char_nina_retrato_neutra.png`: abj
- `Characters/Mother/char_mama_parte_brazo_der.png`: izq, arr, abj
- `Characters/Mother/char_mama_parte_brazo_izq.png`: izq, der, arr, abj
- `Characters/Mother/char_mama_parte_pierna_der.png`: arr
- `Characters/Mother/char_mama_parte_pierna_izq.png`: arr
- `Characters/Mother/char_mama_parte_torso.png`: arr, abj
- `Characters/Mother/char_mama_retrato_neutra.png`: abj
- `Props/River/prop_n3_vela_silueta.png`: arr
- `Props/Wheel/prop_n2_tronco_a.png`: arr
- `Props/Wheel/prop_n2_tronco_textura.png`: izq, arr, abj
- `UI/Common/ui_boton.png`: izq, der, arr, abj
- `UI/Common/ui_circulo.png`: izq, der, arr, abj
- `UI/Common/ui_panel.png`: izq, der, arr, abj

## Tabla

| Archivo | px | Nombre | Alfa | Halo | Borde | .meta |
|---|---|---|---|---|---|---|
| `Characters/Algoritm/char_algoritm_n1_fuego_reposo.png` | 768×768 | OK | RGBA · 67 % transp. | 0 | — | ok |
| `Characters/Algoritm/char_algoritm_n2_rueda_reposo.png` | 768×768 | OK | RGBA · 67 % transp. | 0 | — | ok |
| `Characters/Algoritm/char_algoritm_n3_gota_reposo.png` | 768×768 | OK | RGBA · 67 % transp. | 0 | — | ok |
| `Characters/Boy/char_nino_parte_brazo_der.png` | 280×260 | OK | RGBA · 70 % transp. | 401 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Boy/char_nino_parte_brazo_izq.png` | 298×265 | OK | RGBA · 71 % transp. | 447 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Boy/char_nino_parte_pierna_der.png` | 155×171 | OK | RGBA · 41 % transp. | 70 (0 int., α≤63) | arr, abj | ok |
| `Characters/Boy/char_nino_parte_pierna_izq.png` | 155×171 | OK | RGBA · 41 % transp. | 68 (0 int., α≤63) | arr, abj | ok |
| `Characters/Boy/char_nino_parte_torso.png` | 450×757 | OK | RGBA · 41 % transp. | 1175 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Boy/char_nino_retrato_neutra.png` | 320×320 | OK | RGBA · 28 % transp. | 0 | izq, der, abj | ok |
| `Characters/Father/char_papa_parte_brazo_der.png` | 311×352 | OK | RGBA · 64 % transp. | 397 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Father/char_papa_parte_brazo_izq.png` | 318×354 | OK | RGBA · 65 % transp. | 419 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Father/char_papa_parte_pierna_der.png` | 197×241 | OK | RGBA · 42 % transp. | 117 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Father/char_papa_parte_pierna_izq.png` | 196×241 | OK | RGBA · 43 % transp. | 113 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Father/char_papa_parte_torso.png` | 390×725 | OK | RGBA · 22 % transp. | 909 (0 int., α≤63) | izq, arr, abj | ok |
| `Characters/Father/char_papa_retrato_neutra.png` | 320×320 | OK | RGBA · 24 % transp. | 0 | izq, der, abj | ok |
| `Characters/Girl/char_nina_parte_brazo_der.png` | 275×194 | OK | RGBA · 63 % transp. | 390 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Girl/char_nina_parte_brazo_izq.png` | 279×194 | OK | RGBA · 64 % transp. | 395 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Girl/char_nina_parte_pierna_der.png` | 186×216 | OK | RGBA · 51 % transp. | 192 (0 int., α≤63) | izq, arr, abj | ok |
| `Characters/Girl/char_nina_parte_pierna_izq.png` | 183×216 | OK | RGBA · 51 % transp. | 177 (0 int., α≤63) | der, arr, abj | ok |
| `Characters/Girl/char_nina_parte_torso.png` | 321×720 | OK | RGBA · 36 % transp. | 1529 (0 int., α≤63) | arr, abj | ok |
| `Characters/Girl/char_nina_retrato_neutra.png` | 320×320 | OK | RGBA · 45 % transp. | 0 | abj | ok |
| `Characters/Mother/char_mama_cenital.png` | 256×256 | OK | RGBA · 58 % transp. | 0 | — | ok |
| `Characters/Mother/char_mama_parte_brazo_der.png` | 285×277 | OK | RGBA · 73 % transp. | 335 (0 int., α≤63) | izq, arr, abj | ok |
| `Characters/Mother/char_mama_parte_brazo_izq.png` | 287×279 | OK | RGBA · 73 % transp. | 324 (0 int., α≤63) | izq, der, arr, abj | ok |
| `Characters/Mother/char_mama_parte_pierna_der.png` | 137×287 | OK | RGBA · 50 % transp. | 100 (0 int., α≤63) | arr | ok |
| `Characters/Mother/char_mama_parte_pierna_izq.png` | 136×287 | OK | RGBA · 49 % transp. | 114 (0 int., α≤63) | arr | ok |
| `Characters/Mother/char_mama_parte_torso.png` | 349×655 | OK | RGBA · 31 % transp. | 904 (0 int., α≤63) | arr, abj | ok |
| `Characters/Mother/char_mama_retrato_neutra.png` | 320×320 | OK | RGBA · 32 % transp. | 0 | abj | ok |
| `Environments/Fire/env_n1_apertura.png` | 3840×1080 | OK | RGBA · opaco (env) | 0 | n/a | ok |
| `Environments/Fire/env_n1_cueva_2x.png` | 3840×1080 | OK | RGB · opaco (env) | 0 | n/a | ok |
| `Environments/Fire/env_n1_cueva_cenital.png` | 1920×1080 | OK | RGBA · opaco (env) | 0 | n/a | ok |
| `Environments/Narrative/env_enlace_n2.png` | 1920×1080 | OK | RGBA · opaco (env) | 0 | n/a | 8/Multiple/100/Bilinear |
| `Environments/Narrative/env_final_fogatas.png` | 1920×1080 | OK | RGBA · opaco (env) | 0 | n/a | ok |
| `Environments/River/env_n3_rio.png` | 1920×1080 | OK | RGBA · opaco (env) | 0 | n/a | ok |
| `Environments/River/env_n3_zona_disponible.png` | 256×256 | OK | RGBA · 39 % transp. | 0 | — | ok |
| `Environments/Wheel/env_n2_bosque_claro.png` | 3840×1080 | OK | RGB · opaco (env) | 0 | n/a | ok |
| `Environments/Wheel/env_n2_laberinto.png` | 1920×1080 | OK | RGBA · opaco (env) | 0 | n/a | ok |
| `Props/Fire/prop_n1_hoja.png` | 2000×2000 | OK | RGBA · 82 % transp. | 0 | — | ok |
| `Props/Fire/prop_n1_monton_hojas.png` | 2000×2000 | OK | RGBA · 91 % transp. | 0 | — | ok |
| `Props/Fire/prop_n1_monton_hojas_cenital.png` | 2000×2000 | OK | RGBA · 71 % transp. | 0 | — | ok |
| `Props/Fire/prop_n1_pedernal.png` | 2000×2000 | OK | RGBA · 57 % transp. | 0 | — | ok |
| `Props/Fire/prop_n1_silex.png` | 2000×2000 | OK | RGBA · 64 % transp. | 0 | — | ok |
| `Props/River/prop_n3_amarre.png` | 128×128 | OK | RGBA · 69 % transp. | 11 (0 int., α≤32) | — | ok |
| `Props/River/prop_n3_amarre_silueta.png` | 128×128 | OK | RGBA · 67 % transp. | 0 | — | ok |
| `Props/River/prop_n3_balsa_cruzando.png` | 512×512 | OK | RGBA · 62 % transp. | 0 | — | ok |
| `Props/River/prop_n3_balsa_hundida.png` | 512×512 | OK | RGBA · 62 % transp. | 0 | — | ok |
| `Props/River/prop_n3_mastil.png` | 512×512 | OK | RGBA · 89 % transp. | 0 | — | ok |
| `Props/River/prop_n3_mastil_silueta.png` | 512×512 | OK | RGBA · 88 % transp. | 0 | — | ok |
| `Props/River/prop_n3_sogas.png` | 256×256 | OK | RGBA · 88 % transp. | 0 | — | ok |
| `Props/River/prop_n3_tela.png` | 256×256 | OK | RGBA · 76 % transp. | 0 | — | ok |
| `Props/River/prop_n3_tronco.png` | 256×256 | OK | RGBA · 74 % transp. | 0 | — | ok |
| `Props/River/prop_n3_tronco_silueta.png` | 256×256 | OK | RGBA · 72 % transp. | 0 | — | ok |
| `Props/River/prop_n3_troncos.png` | 256×256 | OK | RGBA · 51 % transp. | 0 | — | ok |
| `Props/River/prop_n3_vela.png` | 256×256 | OK | RGBA · 74 % transp. | 0 | — | ok |
| `Props/River/prop_n3_vela_silueta.png` | 256×256 | OK | RGBA · 72 % transp. | 0 | arr | ok |
| `Props/Wheel/prop_n2_caja_suelo.png` | 256×256 | OK | RGBA · 48 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_caja_suelo_vacia.png` | 256×256 | OK | RGBA · 49 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_carretilla_e1.png` | 256×256 | OK | RGBA · 82 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_carretilla_e2.png` | 512×512 | OK | RGBA · 90 % transp. | 1 (0 int., α≤5) | — | ok |
| `Props/Wheel/prop_n2_carretilla_e3.png` | 512×512 | OK | RGBA · 84 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_carretilla_e4.png` | 512×512 | OK | RGBA · 80 % transp. | 2 (1 int., α≤2) | — | ok |
| `Props/Wheel/prop_n2_carretilla_e5.png` | 512×512 | OK | RGBA · 80 % transp. | 1 (0 int., α≤2) | — | ok |
| `Props/Wheel/prop_n2_herramienta_a.png` | 256×256 | OK | RGBA · 78 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_herramienta_b.png` | 256×256 | OK | RGBA · 78 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_herramienta_c.png` | 256×256 | OK | RGBA · 85 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_laberinto_carretilla.png` | 256×256 | OK | RGBA · 40 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_laberinto_obstaculo.png` | 256×256 | OK | RGBA · 31 % transp. | 13 (0 int., α≤6) | — | ok |
| `Props/Wheel/prop_n2_piedra_a.png` | 256×256 | OK | RGBA · 63 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_piedra_b.png` | 256×256 | OK | RGBA · 63 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_piedra_c.png` | 256×256 | OK | RGBA · 63 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_piedra_d.png` | 256×256 | OK | RGBA · 58 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_pieza_1.png` | 256×256 | OK | RGBA · 82 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_pieza_2.png` | 256×256 | OK | RGBA · 78 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_pieza_3.png` | 512×512 | OK | RGBA · 88 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_pieza_4.png` | 512×512 | OK | RGBA · 79 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_pieza_5.png` | 256×256 | OK | RGBA · 79 % transp. | 0 | — | ok |
| `Props/Wheel/prop_n2_planta_a.png` | 256×256 | OK | RGBA · 74 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_planta_b.png` | 256×256 | OK | RGBA · 74 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_planta_c.png` | 256×256 | OK | RGBA · 74 % transp. | 0 | — | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_tronco_a.png` | 256×256 | OK | RGBA · 22 % transp. | 2 (0 int., α≤3) | arr | 8/Multiple/100/Bilinear |
| `Props/Wheel/prop_n2_tronco_textura.png` | 768×256 | OK | RGBA · 7 % transp. | 0 | izq, arr, abj | ok |
| `UI/Common/ui_alerta.png` | 96×96 | OK | RGBA · 65 % transp. | 0 | — | ok |
| `UI/Common/ui_boton.png` | 80×80 | OK | RGBA · 6 % transp. | 0 | izq, der, arr, abj | ok |
| `UI/Common/ui_circulo.png` | 128×128 | OK | RGBA · 19 % transp. | 0 | izq, der, arr, abj | ok |
| `UI/Common/ui_flecha.png` | 128×128 | OK | RGBA · 73 % transp. | 0 | — | ok |
| `UI/Common/ui_ind_errores_boceto.png` | 256×256 | OK | RGBA · 92 % transp. | 0 | — | ok |
| `UI/Common/ui_ind_intentos_boceto.png` | 256×256 | OK | RGBA · 86 % transp. | 0 | — | ok |
| `UI/Common/ui_ind_pasos_boceto.png` | 256×256 | OK | RGBA · 93 % transp. | 0 | — | ok |
| `UI/Common/ui_ind_tiempo_boceto.png` | 256×256 | OK | RGBA · 85 % transp. | 0 | — | ok |
| `UI/Common/ui_lock.png` | 128×128 | OK | RGBA · 67 % transp. | 0 | — | ok |
| `UI/Common/ui_panel.png` | 96×96 | OK | RGBA · 7 % transp. | 0 | izq, der, arr, abj | ok |
| `UI/Common/ui_papelera.png` | 96×96 | OK | RGBA · 60 % transp. | 2 (0 int., α≤4) | — | ok |
| `UI/Common/ui_pausa.png` | 128×128 | OK | RGBA · 28 % transp. | 0 | — | ok |
| `UI/Common/ui_reanudar.png` | 128×128 | OK | RGBA · 68 % transp. | 0 | — | ok |
| `UI/Common/ui_reiniciar.png` | 128×128 | OK | RGBA · 67 % transp. | 0 | — | ok |
| `UI/River/ui_n3_casilla_hecha.png` | 128×128 | OK | RGBA · 34 % transp. | 0 | — | ok |

---

## Lectura (revisión W3, 01/10/2026; escrita a mano, no sale del script)

- **Nombres §15.4: 0 fallan.** Los cinco que fallaban (`entorno_n1_apertura`, `entorno_n1_cueva_2x`, `entorno_n1_cueva_cenital`, `entorno_n2_laberinto`, `prop_n2_caja_suelo_vacía`) se renombraron desde el motor con el GUID conservado (INC-126). Lo vigila `ArtImport_RNF23_LosNombresSiguenLaNomenclatura`.
- **Halo intenso: 0 archivos.** Los 7 PNG del alcance (Algoritm ×3 y los cuatro `char_*_retrato_neutra`) se desmancharon con `halo --apply --max-alpha 200`: alfa idéntico y ningún píxel con α ≥ 200 tocado. **Queda un resto que esta comprobación no mide** (solo mira α < 64): motas verde oscuro opacas (α ≥ 200) en las puntas del pelo — Niña 136 px, Papá 68, Niño 30, Mamá 306, Algoritm 1 636 (67 intensos) según `halo --dry-run --max-alpha 256`. En pantalla, a 1080p, se ven unas 12–13 motas en los retratos de Niña y Papá; en Algoritm, reducido al cuadro de diálogo, no se ven.
- **Halo tenue (27):** las partes de los rigs (`char_*_parte_*`), verde oscuro que solo se nota sobre fondo claro, y 7 píxeles sueltos. Informativo: fuera del alcance de W3.
- **.meta desviado (12):** los 12 PNG del N2 en `Sprite Mode: Multiple` son la excepción de INC-128. Diez se referencian por un fileID de sub-sprite distinto de 21300000, así que pasarlos a `Single` rompería esas referencias; `prop_n2_piedra_c` y `prop_n2_piedra_d` no tienen referencias. No se migró nada.
