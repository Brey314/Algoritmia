using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scaffolding;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Scaffolding.Tests
{
    /// <summary>
    /// Comprueba el **contenido** de las secuencias narrativas que hay en el proyecto, no la
    /// clase. Los textos viven en assets (RNF-18, CT-05), así que el requisito de legibilidad
    /// también hay que verificarlo sobre el asset y no sobre el código.
    /// </summary>
    public class NarrativeSequenceTests
    {
        private const int MaximoPalabrasPorOracion = 20;

        [Test]
        public void NarrativeSequence_RNF01_NingunaOracionSupera20Palabras()
        {
            var excedidas = TodasLasSecuencias()
                .SelectMany(sequence => sequence.Lines.Select(line => (sequence, line)))
                .SelectMany(par => Oraciones(par.line.Text)
                    .Where(oracion => Palabras(oracion) > MaximoPalabrasPorOracion)
                    .Select(oracion => $"{par.sequence.Id} · {Palabras(oracion)} palabras: «{oracion}»"))
                .ToArray();

            Assert.That(excedidas, Is.Empty);
        }

        [Test]
        public void NarrativeSequence_RNF18_CadaSecuenciaTieneIdYLineas()
        {
            var incompletas = TodasLasSecuencias()
                .Where(sequence => string.IsNullOrWhiteSpace(sequence.Id) || sequence.Lines.Length == 0)
                .Select(sequence => sequence.name)
                .ToArray();

            Assert.That(incompletas, Is.Empty);
        }

        [Test]
        public void NarrativeSequence_RF05_LosIdentificadoresNoSeRepiten()
        {
            var repetidos = TodasLasSecuencias()
                .GroupBy(sequence => sequence.Id)
                .Where(grupo => grupo.Count() > 1)
                .Select(grupo => grupo.Key)
                .ToArray();

            Assert.That(repetidos, Is.Empty);
        }

        [Test]
        public void NarrativeSequence_RF05_ElNivel2TieneSusSieteSecuencias()
        {
            var esperadas = new[]
            {
                "N2_PuenteI", "N2_PuenteI_Bosque", "N2_Escena21_Bosque", "N2_Escena22_ElPatron",
                "N2_Escena23_Construccion", "N2_Escena24_Regreso", "N2_Escena25_Cierre"
            };

            var delNivel2 = TodasLasSecuencias()
                .Where(sequence => sequence.Level == Game.Core.LevelId.Wheel)
                .Select(sequence => sequence.Id)
                .ToArray();

            Assert.That(delNivel2, Is.EquivalentTo(esperadas));
        }

        /// <summary>
        /// El Nivel 2 dura un día entero y la luz lo cuenta: amanece junto a la cueva (azul),
        /// recolectar y armar es de tarde (sin tinte), la vuelta al refugio es al atardecer (cálido)
        /// y el cierre es de noche, donde solo el fuego alumbra de lleno. El laberinto lleva su
        /// propio tinte de atardecer en <c>N2_MazeLayout</c>.
        /// </summary>
        [Test]
        public void NarrativeSequence_RF05_ElNivel2TranscurreDelAmanecerALaNoche()
        {
            var secuencias = TodasLasSecuencias().ToDictionary(sequence => sequence.Id);
            var amanecer = secuencias["N2_PuenteI"].LightStart;
            var tarde = secuencias["N2_Escena21_Bosque"].LightStart;
            var atardecer = secuencias["N2_Escena24_Regreso"].LightStart;
            var noche = secuencias["N2_Escena25_Cierre"].LightStart;

            Assert.That(amanecer.Tint.b, Is.GreaterThan(amanecer.Tint.r), "amanece: luz azulada");
            Assert.That(tarde.Tint, Is.EqualTo(Color.white), "la tarde no lleva tinte");
            Assert.That(tarde.Ambient, Is.EqualTo(1f), "ni oscurece");
            Assert.That(atardecer.Tint.r, Is.GreaterThan(atardecer.Tint.b), "atardece: luz cálida");
            Assert.That(noche.Ambient, Is.LessThan(atardecer.Ambient), "de noche lo que no alumbra el fuego se oscurece");
            Assert.That(noche.Radius, Is.GreaterThan(0f), "y el fuego sí alumbra");
        }

        [Test]
        public void NarrativeSequence_RF05_ElNivel3TieneSusSieteSecuencias()
        {
            // Cinco escenas del guion (§7, §8.1, §8.4.1, §8.5, §9) en siete assets: el puente II
            // cambia dos veces de ilustración —el refugio en la cueva, el horizonte con las
            // fogatas y el río— y la ilustración es por secuencia, así que cada corte es un
            // asset encadenado.
            var esperadas = new[]
            {
                "N3_PuenteII", "N3_PuenteII_Horizonte", "N3_PuenteII_Rio", "N3_Escena31_Llegada",
                "N3_Escena32_PrimerIntento", "N3_Escena33_Cruce", "N3_EscenaFinal"
            };

            var delNivel3 = TodasLasSecuencias()
                .Where(sequence => sequence.Level == Game.Core.LevelId.River)
                .Select(sequence => sequence.Id)
                .ToArray();

            Assert.That(delNivel3, Is.EquivalentTo(esperadas));
        }

        [Test]
        public void NarrativeSequence_RF05_NingunEncuadreDelNivel3SeRecorta()
        {
            // Camara_Narrativa_N3.md §3 R1: con un sprite 16:9 sin duplicar el foco vive en
            // [0.5/z, 1−0.5/z] en los dos ejes, y un valor fuera se recorta en silencio.
            var recortados = TodasLasSecuencias()
                .Where(sequence => sequence.Level == Game.Core.LevelId.River && sequence.Illustration != null)
                .SelectMany(sequence =>
                {
                    var aspecto = sequence.Illustration.rect.width / sequence.Illustration.rect.height;
                    return new[] { ("inicio", sequence.CameraStart) }
                        .Concat(sequence.CameraKeys.Select(key => ($"línea {key.Line}", key.Framing)))
                        .SelectMany(parada => IllustrationFraming.Warnings(parada.Item2, null, false, aspecto)
                            .Select(aviso => $"{sequence.Id} · {parada.Item1}: {aviso}"));
                })
                .ToArray();

            Assert.That(recortados, Is.Empty);
        }

        /// <summary>
        /// El borde inferior de la espuma de la cascada de <c>env_n3_rio</c>, como fracción del alto de
        /// la ilustración. env_n3_rio: espuma en y ∈ [0.403, 0.544], x ∈ [0.46, 0.67] (PIL,
        /// 30/09/2026); se redondea hacia abajo.
        /// </summary>
        private const float BordeInferiorDeLaEspuma = 0.40f;

        /// <summary>
        /// La altura de cada viajero sobre el centro de la balsa, en fracciones del alto de la
        /// ilustración: la composición aprobada sobre la balsa (capturas del 25/09/2026).
        /// </summary>
        private static readonly Dictionary<string, float> AlturaSobreLaBalsa = new Dictionary<string, float>
        {
            ["Papa"] = 0.002f, ["Mama"] = 0.002f, ["Nina"] = -0.050f, ["Nino"] = -0.054f
        };

        /// <summary>«¡Al otro lado!»: en la línea 3 la familia baja a la orilla.</summary>
        private const int LineaDeDesembarco = 3;

        /// <summary>
        /// La balsa del cruce navega en la poza, por debajo de la espuma de la cascada, y no sobre
        /// ella (RF-44, D-i de la tarjeta D10-3): con el centro más arriba la cubierta se pintaba
        /// encima de la espuma blanca y se leía como si flotara en el aire.
        /// </summary>
        /// <remarks>
        /// Con la balsa de 0.28 el pie del mástil queda 0.024 más abajo que el centro, y solo roza la
        /// espuma el canto de los troncos de atrás (3,9 % de la cubierta, frente al 32 % de antes).
        /// Si cambia el arte de la cascada hay que volver a medir (RNF-23).
        /// </remarks>
        [Test]
        [Category("Acceptance")]
        public void NarrativeSequence_RF44_LaBalsaDelCruceNavegaBajoLaEspumaDeLaCascada()
        {
            var cruce = TodasLasSecuencias().Single(sequence => sequence.Id == "N3_Escena33_Cruce");
            var balsa = cruce.Props.Single(prop => prop.Motion == PropMotion.Drift);

            Assert.That(cruce.Illustration.name, Is.EqualTo("env_n3_rio"), "la medida de la espuma es de esa ilustración");
            Assert.That(balsa.Position.y, Is.LessThanOrEqualTo(BordeInferiorDeLaEspuma), "al empezar el cruce");
            Assert.That(balsa.Position.y - balsa.MotionDrop, Is.LessThanOrEqualTo(BordeInferiorDeLaEspuma), "y al terminarlo");
        }

        /// <summary>
        /// La familia baja con la balsa y viaja en su sitio (RF-44, D-i): cada viajero queda a la
        /// altura aprobada sobre el centro de la balsa —con los pies sobre la cubierta— al subir y al
        /// terminar cada paso del viaje. Bajar solo la balsa dejaría a Papá y a Mamá flotando 0.06
        /// por encima de la cubierta.
        /// </summary>
        /// <remarks>
        /// Los pasos de la línea 3 bajan de la balsa a la orilla y no viajan en ella: no se miden, y
        /// no se bajan con la balsa, porque sus destinos están sobre el pasto y a 0.06 más abajo la
        /// Niña pisaría el agua.
        /// </remarks>
        [Test]
        public void NarrativeSequence_RF44_LaFamiliaBajaConLaBalsaYViajaEnSuSitio()
        {
            const float tolerancia = 0.0005f;
            var cruce = TodasLasSecuencias().Single(sequence => sequence.Id == "N3_Escena33_Cruce");
            var balsa = cruce.Props.Single(prop => prop.Motion == PropMotion.Drift);
            var finDelCruce = balsa.Position.y - balsa.MotionDrop;
            var viajeros = cruce.Props
                .Where(prop => prop.Actor != null && AlturaSobreLaBalsa.ContainsKey(prop.Actor.name))
                .ToArray();

            var desviados = viajeros
                .SelectMany(viajero =>
                {
                    var esperada = AlturaSobreLaBalsa[viajero.Actor.name];
                    return new[] { ("al subir", viajero.Position.y - balsa.Position.y) }
                        .Concat(viajero.Beats
                            .Where(paso => paso.Moves && paso.Line < LineaDeDesembarco)
                            .Select(paso => ($"en el paso de la línea {paso.Line}", paso.Destination.y - finDelCruce)))
                        .Where(medida => Mathf.Abs(medida.Item2 - esperada) > tolerancia)
                        .Select(medida => FormattableString.Invariant(
                            $"«{viajero.Actor.name}» {medida.Item1}: queda a {medida.Item2:+0.000;-0.000} de la balsa y debía estar a {esperada:+0.000;-0.000}"));
                })
                .ToArray();

            Assert.That(viajeros.Select(viajero => viajero.Actor.name), Is.EquivalentTo(AlturaSobreLaBalsa.Keys),
                "los cuatro viajan en la balsa");
            Assert.That(desviados, Is.Empty);
        }

        /// <summary>
        /// La cámara del cruce acompaña a la balsa (RF-44, decisión de Santiago, 08/10/2026): la
        /// balsa lleva la marca <see cref="NarrativeProp.CameraFollows"/> y, con la cámara
        /// siguiéndola, no se sale del cuadro en ninguna parada ni en ningún punto del cruce, y la
        /// cámara no retrocede mientras cruza.
        /// </summary>
        /// <remarks>
        /// Sin la marca la balsa se sale del cuadro por la derecha si el texto tarda en avanzar: el
        /// primer encuadre la deja a 164 px del borde y recorre 0,12 de la ilustración en 9 s.
        /// «No retrocede» se mide sobre el foco que de verdad se ve, ya acotado por
        /// <see cref="IllustrationFraming"/>, con la balsa llegada a la otra orilla: subir la parada de
        /// la línea 1 por encima de 0,547 lo haría bajar al pasar a la línea 2, porque ahí el plano
        /// abre (zoom 1,5) y el acotado del foco se cierra.
        /// </remarks>
        [Test]
        public void NarrativeSequence_RF44_LaCamaraDelCruceAcompanaALaBalsa()
        {
            const float margen = 0.02f; // de la pantalla: la cámara se suaviza y la balsa llega a ir 77 px por detrás
            var secuencias = TodasLasSecuencias().ToArray();
            var cruce = secuencias.Single(sequence => sequence.Id == "N3_Escena33_Cruce");
            var balsa = cruce.Props.Single(prop => prop.Motion == PropMotion.Drift);
            var imagen = cruce.Illustration.rect.size;
            var proporcionDeLaBalsa = balsa.Art.rect.width / balsa.Art.rect.height;
            var llegada = new Vector2(balsa.MotionDistance, -balsa.MotionDrop);
            var paradas = new[] { cruce.CameraStart }
                .Concat(cruce.CameraKeys.Where(key => key.Line <= LineaDeDesembarco).Select(key => key.Framing))
                .ToArray();

            // Dónde queda la cámara con la balsa a mitad de camino, y dónde se ve el foco ya acotado.
            CameraFraming Siguiendo(CameraFraming parada, Vector2 movido) =>
                balsa.CameraFollows ? new CameraFraming(parada.Focus + movido, parada.Zoom) : parada;

            Vector2 FocoVisto(CameraFraming encuadre)
            {
                var tamano = IllustrationFraming.CoverSize(imagen, Ventana, encuadre.Zoom);
                var desplazamiento = IllustrationFraming.Offset(tamano, Ventana, encuadre.Focus);
                return new Vector2(0.5f, 0.5f) - new Vector2(desplazamiento.x / tamano.x, desplazamiento.y / tamano.y);
            }

            var seSale = paradas
                .SelectMany(parada => new[] { 0f, 0.25f, 0.5f, 0.75f, 1f }
                    .Select(avance => (parada, movido: llegada * avance)))
                .Select(caso =>
                {
                    var encuadre = Siguiendo(caso.parada, caso.movido);
                    var tamano = IllustrationFraming.CoverSize(imagen, Ventana, encuadre.Zoom);
                    var centro = IllustrationFraming.Offset(tamano, Ventana, encuadre.Focus)
                                 + (balsa.Position + caso.movido - new Vector2(0.5f, 0.5f)) * tamano;
                    var medioAncho = tamano.y * balsa.Size * Mathf.Min(1f, proporcionDeLaBalsa) / 2f;
                    var izquierda = 0.5f + (centro.x - medioAncho) / Ventana.x;
                    var derecha = 0.5f + (centro.x + medioAncho) / Ventana.x;
                    return (caso, izquierda, derecha);
                })
                .Where(medida => medida.izquierda < margen || medida.derecha > 1f - margen)
                .Select(medida => FormattableString.Invariant(
                    $"foco ({medida.caso.parada.Focus.x:0.00}, {medida.caso.parada.Focus.y:0.00}) zoom {medida.caso.parada.Zoom:0.00} con la balsa {medida.caso.movido.x:0.000} más allá: ocupa de {medida.izquierda:0.000} a {medida.derecha:0.000} de la pantalla"))
                .ToArray();

            var focosAlLlegar = paradas.Select(parada => FocoVisto(Siguiendo(parada, llegada)).x).ToArray();

            Assert.That(balsa.CameraFollows, Is.True, "la balsa lleva la marca: la cámara la acompaña");
            Assert.That(secuencias.SelectMany(sequence => sequence.Props).Count(prop => prop.CameraFollows), Is.EqualTo(1),
                "solo la balsa: en las otras escenas la cámara no sigue a nadie");
            Assert.That(seSale, Is.Empty, "la balsa se ve entera en cada parada, con la cámara siguiéndola");
            Assert.That(focosAlLlegar, Is.Ordered, "el foco que se ve no retrocede de una parada a la siguiente mientras cruzan");
        }

        [Test]
        public void NarrativeVisitPolicy_CP07_ElCruceYLaEscenaFinalNoSeOmitenLaPrimeraVez()
        {
            // RF-12, CP-07: el cierre reflexivo del río (§8.5) y la escena final (§9) se leen
            // enteros la primera vez. Aquí «primera vez» ya tiene todas las fases confirmadas —se
            // llega justo después de la última— y no hay nivel siguiente que desbloquear.
            var perfil = Game.Core.PlayerProfile.Create("Ana", Array.Empty<string>()).Profile;
            perfil.Reach(Game.Core.LevelId.River);
            foreach (var fase in Game.Core.PhaseId.AllOf(Game.Core.LevelId.River))
            {
                perfil.ConfirmPhase(fase, default);
            }

            var cierres = TodasLasSecuencias()
                .Where(sequence => sequence.Id == "N3_Escena33_Cruce" || sequence.Id == "N3_EscenaFinal")
                .ToArray();

            Assert.That(cierres, Has.Length.EqualTo(2));
            foreach (var cierre in cierres)
            {
                Assert.That(cierre.IsReflectiveClosing, Is.True, $"{cierre.Id} se declara cierre reflexivo");
                Assert.That(NarrativeVisitPolicy.AlreadySeen(perfil, cierre), Is.False, $"{cierre.Id} no ofrece omitir");
                // La escena final además sale a los créditos (INC-39): lleva las dos marcas, y es la
                // de cierre reflexivo la que le niega el botón de omitir (R14).
                Assert.That(cierre.EndsInCredits, Is.EqualTo(cierre.Id == "N3_EscenaFinal"),
                    $"{cierre.Id}: solo la escena final sale a los créditos");
            }
        }

        /// <summary>
        /// OBS-13: en <c>N3_EscenaFinal</c> la llama de la fogata central salía justo detrás de la
        /// cabeza de la Niña y parecía salirle del pelo. Ninguna llama puede quedar cerca, en
        /// horizontal, de la cabeza de un personaje si su base arranca a la altura de esa cabeza.
        /// </summary>
        /// <remarks>
        /// «Cabeza» es una aproximación sobre datos del asset: la altura de los pies más la mitad del
        /// tamaño del personaje. Las tolerancias son holgadas a propósito (un tercio de la
        /// separación real): el aviso es para quien mueva la fogata o a la familia sin mirar.
        /// </remarks>
        [Test]
        public void NarrativeSequence_OBS13_NingunaLlamaDeLaEscenaFinalSaleDeTrasLaCabezaDeLaFamilia()
        {
            const float margenHorizontal = 0.03f;
            const float margenVertical = 0.03f;
            var escena = TodasLasSecuencias().Single(sequence => sequence.Id == "N3_EscenaFinal");
            var llamas = escena.Props.Where(prop => prop.Actor == null && prop.Art != null && prop.Art.name.StartsWith("fuego")).ToArray();
            var cabezas = escena.Props
                .Where(prop => prop.Actor != null)
                .Select(prop => (
                    // Donde termina de caminar: el último destino con movimiento, o donde empieza.
                    x: prop.Beats.LastOrDefault(beat => beat.Moves)?.Destination.x ?? prop.Position.x,
                    y: prop.Beats.LastOrDefault(beat => beat.Moves)?.Destination.y ?? prop.Position.y,
                    size: prop.Size))
                .ToArray();

            Assume.That(llamas, Is.Not.Empty, "la escena final tiene fogatas");
            Assume.That(cabezas, Has.Length.GreaterThanOrEqualTo(4), "y a la familia");

            var detras = llamas
                .SelectMany(llama => cabezas
                    .Where(cabeza =>
                        Mathf.Abs(llama.Position.x - cabeza.x) < margenHorizontal
                        && llama.Position.y - llama.Size / 2f < cabeza.y + cabeza.size / 2f + margenVertical)
                    .Select(cabeza => $"llama en ({llama.Position.x:0.###}, {llama.Position.y:0.###}) tras la cabeza en ({cabeza.x:0.###}, {cabeza.y + cabeza.size / 2f:0.###})"))
                .ToArray();

            Assert.That(detras, Is.Empty);
        }

        /// <summary>
        /// Los objetos que la narrativa pinta sobre el entorno tienen que quedar **por encima**
        /// del cuadro de diálogo, que ocupa el cuarto inferior de la pantalla
        /// (<c>NarrativeScene_RNF03_ElCuadroDeDialogoNoSuperaElCuartoDeLaPantalla</c>).
        /// </summary>
        /// <remarks>
        /// La regla es la misma en los dos niveles: si el texto lo nombra, se ve entero. En el
        /// Nivel 1 porque la cueva está a oscuras y el objeto es lo único que hay; en el Nivel 2
        /// porque los objetos del suelo **son** el enunciado del ejercicio (RF-22, RF-23).
        ///
        /// Cuenta también las paradas, no solo el encuadre inicial: un objeto bien colocado al
        /// abrir se mete debajo del cuadro en cuanto la cámara cierra el plano.
        /// </remarks>
        [Test]
        public void NarrativeSequence_RNF03_LosObjetosNoQuedanBajoElCuadroDeDialogo()
        {
            var tapados = TodasLasSecuencias()
                .Where(sequence => Verificadas.Contains(sequence.Id) && sequence.Props.Length > 0)
                .SelectMany(ObjetosTapados)
                .ToArray();

            Assert.That(tapados, Is.Empty);
        }

        /// <summary>
        /// Un objeto pintado sin ilustración no desaparece: <c>Image</c> dibuja su casilla en
        /// blanco, así que un sprite que no resuelve sale en pantalla como un cuadrado blanco
        /// encima del entorno (RNF-23).
        /// </summary>
        /// <remarks>
        /// Se cuela solo: la referencia del asset apunta a un archivo que existe, y el `.meta` es
        /// quien decide si dentro hay un sprite con ese id. Pasar una textura de `Single` a
        /// `Multiple` —o escribir la referencia suponiendo `Single`— deja el asset compilando,
        /// abriendo bien en el Inspector y pintando un cuadrado blanco al jugar. Lo vio Santiago
        /// en la 2.4 el 17/09/2026.
        /// </remarks>
        [Test]
        public void NarrativeSequence_RNF23_CadaObjetoPintadoTieneSuIlustracion()
        {
            var sinArte = TodasLasSecuencias()
                .SelectMany(sequence => sequence.Props.Select((prop, indice) => (sequence, prop, indice)))
                .Where(entrada => entrada.prop.Art == null)
                .Select(entrada => FormattableString.Invariant(
                    $"{entrada.sequence.Id} · objeto {entrada.indice} en ({entrada.prop.Position.x:0.000}, {entrada.prop.Position.y:0.000})"))
                .ToArray();

            Assert.That(sinArte, Is.Empty,
                "un objeto sin sprite se pinta como un cuadrado blanco: " + string.Join(" · ", sinArte));
        }

        /// <summary>
        /// Cada llama animada de las narrativas echa humo: un objeto con el clip del humo
        /// (<c>fx_n1_humo</c>) sobre la misma llama, más arriba, y **antes** en la lista para que
        /// se dibuje detrás y la llama tape su arranque.
        /// </summary>
        /// <remarks>
        /// Se cuela solo: quien añade una fogata copia el objeto de la llama y se olvida del humo,
        /// o lo pone después y el humo tapa la punta del fuego.
        /// </remarks>
        [Test]
        public void NarrativeSequence_RF05_CadaLlamaEchaHumoPorDetrasYPorEncima()
        {
            var sinHumo = TodasLasSecuencias()
                .SelectMany(sequence => sequence.Props
                    .Select((prop, indice) => (sequence, prop, indice))
                    .Where(entrada => Anima(entrada.prop, "prop_n1_fuego_normal"))
                    .Where(llama => !sequence.Props
                        .Take(llama.indice)
                        .Any(humo => Anima(humo, "fx_n1_humo")
                                     && humo.Position.y > llama.prop.Position.y
                                     && Mathf.Abs(humo.Position.x - llama.prop.Position.x) < llama.prop.Size / 4f)))
                .Select(llama => FormattableString.Invariant(
                    $"{llama.sequence.Id} · llama {llama.indice} en ({llama.prop.Position.x:0.000}, {llama.prop.Position.y:0.000})"))
                .ToArray();

            Assert.That(sinHumo, Is.Empty);
        }

        /// <summary>
        /// Cada llama de las narrativas (<c>prop_n1_fuego_normal</c>) emite su halo de luz
        /// (<see cref="NarrativeProp.Glows"/>, FireGlow) y solo ellas: el halo en una piedra o en el
        /// humo sería una mancha naranja sin fuego (decisión de Santiago, 08/10/2026).
        /// </summary>
        /// <remarks>
        /// Se cuela solo: quien añade una fogata copia el objeto de la llama de otra escena —el
        /// archivo ya trae el halo— o la dibuja nueva y se olvida de marcarla.
        /// </remarks>
        [Test]
        public void NarrativeSequence_RF05_CadaLlamaEmiteSuHalo()
        {
            var entradas = TodasLasSecuencias()
                .SelectMany(sequence => sequence.Props.Select((prop, indice) => (sequence, prop, indice)))
                .ToArray();
            var llamas = entradas.Where(entrada => Anima(entrada.prop, "prop_n1_fuego_normal")).ToArray();

            var mal = entradas
                .Where(entrada => Anima(entrada.prop, "prop_n1_fuego_normal") != entrada.prop.Glows)
                .Select(entrada => FormattableString.Invariant(
                    $"{entrada.sequence.Id} · objeto {entrada.indice} en ({entrada.prop.Position.x:0.000}, {entrada.prop.Position.y:0.000}): {Falla(entrada.prop)}"))
                .ToArray();

            Assert.That(llamas, Is.Not.Empty, "las narrativas tienen fogatas");
            Assert.That(mal, Is.Empty);
        }

        private static string Falla(NarrativeProp prop) =>
            prop.Glows ? "tiene halo y no es una llama" : "es una llama sin halo";

        private static bool Anima(NarrativeProp prop, string clip) =>
            prop.FrameAnimation != null && prop.FrameAnimation.name.StartsWith(clip, StringComparison.Ordinal);

        /// <summary>
        /// Solo los cuatro que cruzan sobre la balsa de la 3.3 terminan sus pasos aunque el texto
        /// avance (RF-44, RNF-21). En las demás escenas un paso nuevo interrumpe el camino, como
        /// siempre: la marca en otra escena cambiaría lo que se ve en ella al avanzar deprisa.
        /// </summary>
        [Test]
        [Category("Acceptance")]
        public void NarrativeSequence_RF44_SoloLosViajerosDelCruceTerminanSusPasos()
        {
            var marcados = TodasLasSecuencias()
                .SelectMany(sequence => sequence.Props
                    .Where(prop => prop.FinishesSteps)
                    .Select(prop => $"{sequence.Id} · {prop.Actor?.name ?? prop.Art?.name}"))
                .ToArray();

            Assert.That(marcados, Is.EquivalentTo(new[]
            {
                "N3_Escena33_Cruce · Papa", "N3_Escena33_Cruce · Mama",
                "N3_Escena33_Cruce · Nina", "N3_Escena33_Cruce · Nino"
            }));
        }

        /// <summary>
        /// Las secuencias cuyos encuadres están verificados contra el cuadro de diálogo. No es
        /// «todas» a propósito: una escena entra en la lista cuando se revisa su encuadre, no
        /// antes; una prueba que se salta lo que no cumple no comprueba nada.
        /// </summary>
        private static readonly string[] Verificadas =
        {
            "N1_Apertura", "N1_AparicionGuia", "N1_Hallazgo", "N1_NacimientoDelFuego",
            "N2_PuenteI", "N2_PuenteI_Bosque", "N2_Escena21_Bosque", "N2_Escena22_ElPatron",
            "N2_Escena23_Construccion", "N2_Escena24_Regreso", "N2_Escena25_Cierre",
            "N3_PuenteII", "N3_PuenteII_Horizonte", "N3_PuenteII_Rio", "N3_Escena31_Llegada",
            "N3_Escena32_PrimerIntento", "N3_Escena33_Cruce", "N3_EscenaFinal"
        };

        /// <summary>Alto del cuadro de diálogo como fracción de la pantalla (RNF-03).</summary>
        private const float CuadroDeDialogo = 0.25f;

        /// <summary>Una ventana 16:9 cualquiera: la geometría es proporcional, el tamaño da igual.</summary>
        private static readonly Vector2 Ventana = new Vector2(1600f, 900f);

        private static IEnumerable<string> ObjetosTapados(NarrativeSequence sequence)
        {
            if (sequence.Illustration == null)
            {
                yield break;
            }

            var imagen = sequence.Illustration.rect.size;
            var encuadres = new[] { ("inicio", 0, sequence.CameraStart) }
                .Concat(sequence.CameraKeys.Select(key => ($"línea {key.Line}", key.Line, key.Framing)))
                .ToArray();

            for (var k = 0; k < encuadres.Length; k++)
            foreach (var prop in sequence.Props)
            foreach (var posicion in Posiciones(sequence, prop, encuadres, k))
            {
                var (parada, _, framing) = encuadres[k];
                // La misma geometría que aplica el controlador: el objeto cuelga de la
                // ilustración, así que hereda su escala y su desplazamiento.
                var tamano = IllustrationFraming.CoverSize(imagen, Ventana, framing.Zoom);
                var centro = IllustrationFraming.Offset(tamano, Ventana, framing.Focus)
                             + (posicion - new Vector2(0.5f, 0.5f)) * tamano;
                var medioAlto = tamano.y * prop.Size / 2f;
                var medioAncho = medioAlto; // preserveAspect sobre sprites cuadrados
                var abajo = 0.5f + (centro.y - medioAlto) / Ventana.y;
                var izquierda = 0.5f + (centro.x - medioAncho) / Ventana.x;
                var derecha = 0.5f + (centro.x + medioAncho) / Ventana.x;

                // Lo que la cámara no encuadra no lo tapa nadie.
                if (derecha < 0f || izquierda > 1f || abajo >= CuadroDeDialogo)
                {
                    continue;
                }

                yield return FormattableString.Invariant(
                    $"{sequence.Id} · {parada}: «{prop.Actor?.name ?? prop.Art?.name}» en ({posicion.x:0.000}, {posicion.y:0.000}) baja hasta y={abajo:0.000} y el cuadro de diálogo llega a {CuadroDeDialogo:0.00}");
            }
        }

        /// <summary>
        /// Dónde está el objeto mientras dura una parada de la cámara. Un objeto quieto, en su
        /// posición. Un personaje, en todos los sitios por los que pasa entre esa parada y la
        /// siguiente —de dónde sale y a dónde llega en cada línea—, salvo en las líneas en que no
        /// se ve: la cámara no se mueve mientras camina, así que llegar debajo del cuadro también
        /// es quedar tapado. Quien termina sus pasos, además, en los de las líneas anteriores:
        /// ningún paso nuevo lo interrumpe.
        /// </summary>
        private static IEnumerable<Vector2> Posiciones(NarrativeSequence sequence, NarrativeProp prop,
            (string, int Line, CameraFraming)[] encuadres, int k)
        {
            if (prop.Actor == null)
            {
                yield return prop.Position;
                yield break;
            }

            var desde = encuadres[k].Line;
            var hasta = k + 1 < encuadres.Length ? encuadres[k + 1].Line - 1 : sequence.Lines.Length - 1;
            for (var linea = desde; linea <= Mathf.Max(desde, hasta); linea++)
            {
                var cue = ActorTimeline.Cue(prop, linea, speaking: false);
                var oculto = cue.During == ActorAction.Hidden
                             || (cue.During == ActorAction.Vanish && ActorTimeline.BeatAt(prop, linea) == null);
                if (oculto)
                {
                    continue;
                }

                yield return cue.From;
                yield return cue.To;

                // Quien va de camino termina su camino aunque el texto avance: bajo esta parada
                // puede estar todavía entre el origen y el destino de un paso anterior.
                var enCurso = ActorTimeline.BeatAt(prop, linea) == null ? ActorTimeline.WalkUnderway(prop, linea) : null;
                if (enCurso != null)
                {
                    yield return ActorTimeline.PositionBefore(prop, enCurso.Line);
                }

                // Quien termina sus pasos no se baja de ninguno (NarrativeProp.FinishesSteps): bajo
                // esta parada puede seguir en cualquiera de los que empezó antes, de punta a punta.
                if (prop.FinishesSteps)
                {
                    foreach (var paso in prop.Beats.Where(beat => beat != null && beat.Moves && beat.Line < linea))
                    {
                        yield return ActorTimeline.PositionBefore(prop, paso.Line);
                        yield return paso.Destination;
                    }
                }
            }
        }

        // --- INC-148 / INC-150: las expresiones del guion y el rumbo de quien trabaja junto al fuego -----------

        /// <summary>
        /// Los pasos que repiten la acción que ya se mantenía, sin moverse, y que NO son un paso que solo fija la
        /// expresión: el guion los escribió para cortar algo. Papá deja de empujar la caja en la línea 2 de la 2.1 —el
        /// paso Idle interrumpe el empuje en curso— y ese corte es el gesto del guion, no un añadido de la cara.
        /// </summary>
        private static readonly (string Secuencia, string Personaje, int Linea)[] PasosPropiosDelGuion =
        {
            ("N2_Escena21_Bosque", "Papa", 2),
        };

        /// <summary>Los objetos de la secuencia que son personajes (llevan un rig).</summary>
        private static IEnumerable<NarrativeProp> Personajes(NarrativeSequence sequence) =>
            sequence.Props.Where(prop => prop != null && prop.Actor != null);

        /// <summary>Si quien dice la línea está pintado en la escena: entonces su retrato lleva la expresión de sus pasos, no la de la línea.</summary>
        private static bool EstaEnEscena(NarrativeSequence sequence, string hablante) =>
            Personajes(sequence).Any(prop => prop.Actor.Speaks(hablante));

        /// <summary>
        /// Cada línea que dice un personaje en escena lleva su expresión del guion (INC-148): el paso del personaje
        /// la fija en esa línea o antes. La tarjeta del diálogo y el personaje muestran esa cara; sin ella, la tarjeta
        /// caería a la expresión de la acción (<see cref="ActionEmotion"/>) y la línea se vería sin la intención que
        /// el guion le da. Solo hay expresiones de duda, atención, sorpresa, concentración o alegría: ninguna de derrota
        /// ni de tristeza (CP-02).
        /// </summary>
        [Test]
        public void NarrativeSequence_INC148_CadaLineaHabladaLlevaLaEmocionDelGuion()
        {
            var habladas = 0;
            var sinEmocion = new List<string>();
            foreach (var sequence in TodasLasSecuencias())
            {
                for (var linea = 0; linea < sequence.Lines.Length; linea++)
                {
                    var dicha = sequence.Lines[linea];
                    if (dicha.IsStageDirection)
                    {
                        continue;
                    }

                    foreach (var prop in Personajes(sequence).Where(candidato => candidato.Actor.Speaks(dicha.Speaker)))
                    {
                        habladas++;
                        if (ActorTimeline.EmotionAt(prop, linea) == null)
                        {
                            sinEmocion.Add($"{sequence.Id} · L{linea} «{dicha.Speaker}»: {prop.Actor.name} no tiene expresión fijada por el guion");
                        }
                    }
                }
            }

            Assert.That(habladas, Is.GreaterThan(0), "las narrativas tienen líneas dichas por personajes que están en escena");
            Assert.That(sinEmocion, Is.Empty);
        }

        /// <summary>
        /// La expresión de quien está en escena la fija su paso (<see cref="ActorBeat.SetsEmotion"/>); la línea solo
        /// la declara (<see cref="DialogueLine.SetsVoiceEmotion"/>) cuando la dice una voz que no está pintada, la
        /// voz fuera de cuadro. En una acotación no hay retrato, y en la línea de alguien en escena la casilla no
        /// cuenta: sería un segundo sitio donde cambiar la misma cara y confundiría a quien edite el asset (INC-148).
        /// </summary>
        [Test]
        public void NarrativeSequence_INC148_SoloLasVocesFueraDeEscenaDeclaranEmocionEnLaLinea()
        {
            var mal = TodasLasSecuencias()
                .SelectMany(sequence => sequence.Lines.Select((line, indice) => (sequence, line, indice)))
                .Where(entrada => entrada.line.SetsVoiceEmotion)
                .Where(entrada => entrada.line.IsStageDirection || EstaEnEscena(entrada.sequence, entrada.line.Speaker))
                .Select(entrada => entrada.line.IsStageDirection
                    ? $"{entrada.sequence.Id} · L{entrada.indice}: una acotación no tiene retrato y no declara expresión"
                    : $"{entrada.sequence.Id} · L{entrada.indice} «{entrada.line.Speaker}»: está en escena, su expresión la fija su paso")
                .ToArray();

            Assert.That(mal, Is.Empty);
        }

        /// <summary>
        /// Quien fija una expresión la fija en todos sus pasos siguientes (INC-148): en los assets, a partir de su
        /// primera expresión cada paso del personaje lleva la suya. Así cambiar un paso o añadir uno nunca devuelve
        /// la cara, sin que nadie lo note, a la expresión de la acción.
        /// </summary>
        [Test]
        public void NarrativeSequence_INC148_QuienFijaUnaEmocionLaFijaEnSusPasosSiguientes()
        {
            var conExpresion = 0;
            var mal = new List<string>();
            foreach (var sequence in TodasLasSecuencias())
            {
                foreach (var prop in Personajes(sequence))
                {
                    var fijan = prop.Beats.Where(paso => paso != null && paso.SetsEmotion).ToArray();
                    if (fijan.Length == 0)
                    {
                        continue;
                    }

                    conExpresion++;
                    var primera = fijan.Min(paso => paso.Line);
                    mal.AddRange(prop.Beats
                        .Where(paso => paso != null && paso.Line > primera && !paso.SetsEmotion)
                        .Select(paso => $"{sequence.Id} · {prop.Actor.name} L{paso.Line} ({paso.Action}): no fija la expresión y ya la fija desde la línea {primera}"));
                }
            }

            Assert.That(conExpresion, Is.GreaterThan(0), "el guion ya fija expresiones en los assets");
            Assert.That(mal, Is.Empty);
        }

        /// <summary>
        /// Un paso que solo fija la expresión (no se mueve y repite la acción que ya se mantenía) no cambia nada más
        /// de lo que se ve (INC-148): no comparte línea con otro paso —manda el último de la lista—, no interrumpe la
        /// caminata en curso —un paso nuevo hace saltar a quien camina a donde llegaría, salvo a quien termina sus
        /// pasos— y no hace girar al personaje: el rumbo fijado a mano rige un solo paso
        /// (<see cref="ActorTimeline.ExplicitFacingAt"/>), así que si había uno vigente el paso nuevo tiene que
        /// repetirlo. Si no había ninguno, un rumbo explícito es una decisión del guion (INC-150: Papá arrodillado
        /// mira la llama) y no se discute aquí.
        /// </summary>
        [Test]
        public void NarrativeSequence_INC148_UnPasoQueSoloFijaLaEmocionNoInterrumpeNiGira()
        {
            var mal = new List<string>();
            foreach (var sequence in TodasLasSecuencias())
            {
                foreach (var prop in Personajes(sequence))
                {
                    foreach (var paso in prop.Beats.Where(candidato => candidato != null && candidato.SetsEmotion && !candidato.Moves
                                                              && candidato.Action == ActorTimeline.HeldBefore(prop, candidato.Line)))
                    {
                        if (PasosPropiosDelGuion.Contains((sequence.Id, prop.Actor.name, paso.Line)))
                        {
                            continue;
                        }

                        var quien = $"{sequence.Id} · {prop.Actor.name} L{paso.Line} ({paso.Action})";
                        if (prop.Beats.Count(otro => otro != null && otro.Line == paso.Line) > 1)
                        {
                            mal.Add($"{quien}: comparte línea con otro paso, y manda el último de la lista");
                        }

                        if (!prop.FinishesSteps && ActorTimeline.WalkUnderway(prop, paso.Line) != null)
                        {
                            mal.Add($"{quien}: cae con una caminata en curso y la interrumpiría");
                        }

                        // El rumbo fijado a mano que regía justo antes de este paso (sin él en la lista).
                        var vigente = ActorTimeline.ExplicitFacingAt(SinElPaso(prop, paso), paso.Line);
                        if (vigente.HasValue && paso.Facing != (vigente.Value ? ActorFacing.Left : ActorFacing.Right))
                        {
                            mal.Add($"{quien}: hace girar al personaje; debe repetir el rumbo vigente ({(vigente.Value ? "izquierda" : "derecha")}) y declara {paso.Facing}");
                        }
                    }
                }
            }

            Assert.That(mal, Is.Empty);
        }

        /// <summary>Una copia del personaje sin ese paso, para saber qué regía en la escena antes de él.</summary>
        private static NarrativeProp SinElPaso(NarrativeProp prop, ActorBeat paso) =>
            new NarrativeProp(null, prop.Position, prop.Size)
                .WithActor(null, prop.ActorStart, prop.Beats.Where(otro => !ReferenceEquals(otro, paso)).ToArray());

        /// <summary>
        /// Momentos del guion donde la expresión es inequívoca (INC-148): la familia camina asustada en la apertura
        /// (guion §1.3.1), los niños abren los ojos al ver a Algoritm y Papá y Mamá se miran sin creerlo (§1.4.1), y
        /// los niños gritan de alegría cuando Papá hace el fuego (§1.4.3).
        /// </summary>
        [TestCase("N1_Apertura", 0, "Papa", FacialEmotion.Worried)]
        [TestCase("N1_Apertura", 0, "Mama", FacialEmotion.Worried)]
        [TestCase("N1_Apertura", 0, "Nina", FacialEmotion.Worried)]
        [TestCase("N1_Apertura", 0, "Nino", FacialEmotion.Worried)]
        [TestCase("N1_AparicionGuia", 4, "Nina", FacialEmotion.Surprised)]
        [TestCase("N1_AparicionGuia", 4, "Nino", FacialEmotion.Surprised)]
        [TestCase("N1_AparicionGuia", 5, "Papa", FacialEmotion.Surprised)]
        [TestCase("N1_AparicionGuia", 5, "Mama", FacialEmotion.Surprised)]
        [TestCase("N1_NacimientoDelFuego", 3, "Nina", FacialEmotion.Happy)]
        [TestCase("N1_NacimientoDelFuego", 3, "Nino", FacialEmotion.Happy)]
        public void NarrativeSequence_INC148_LasEmocionesDelGuionEnLineasClave(string id, int linea, string personaje, FacialEmotion esperada)
        {
            var sequence = TodasLasSecuencias().Single(candidata => candidata.Id == id);
            var prop = Personajes(sequence).Single(candidato => candidato.Actor.name == personaje);

            Assert.That(ActorTimeline.EmotionAt(prop, linea), Is.EqualTo(esperada),
                $"{id} L{linea}: «{sequence.Lines[linea].Text}» — {personaje} debe verse {esperada}");
            Assert.That(ActorTimeline.EmotionOf(prop, linea, speaking: true), Is.EqualTo(esperada), "y es la que ve el retrato");
        }

        /// <summary>
        /// En la 3.2, tras el primer intento sin éxito, nadie cierra la escena con la cara de duda: el ánimo es
        /// alegre o concentrado, nunca el de haber perdido (CP-02, guion §1.8.4). Worried es la duda de quien pregunta
        /// y espera, y al cerrar la escena el intento ya pasó.
        /// </summary>
        [Test]
        public void NarrativeSequence_CP02_LaEscena32NoCierraConNadieEnWorried()
        {
            var sequence = TodasLasSecuencias().Single(candidata => candidata.Id == "N3_Escena32_PrimerIntento");
            var ultima = sequence.Lines.Length - 1;
            var personajes = Personajes(sequence).ToArray();

            Assert.That(personajes, Is.Not.Empty, "la 3.2 tiene personajes en escena");
            foreach (var prop in personajes)
            {
                Assert.That(ActorTimeline.EmotionOf(prop, ultima, speaking: false), Is.Not.EqualTo(FacialEmotion.Worried),
                    $"{prop.Actor.name} cierra la 3.2 (L{ultima}) y no puede quedar en duda: es el ánimo tras el intento (CP-02)");
            }
        }

        /// <summary>
        /// Quien trabaja junto al fuego lo mira (INC-150): si un personaje se arrodilla, sopla o recoge algo a menos
        /// de 0,15 de ancho de una fogata, mira hacia ella y no hacia el otro lado. Con la regla de antes, sin pasos que
        /// lo desplacen caía en «hacia el centro de la ilustración» y Papá quedaba arrodillado de espaldas a la llama
        /// (N1 1.3, líneas 0–4 y 8–11; la 2.5 con Mamá y la Niña). Se mide en cada línea donde la acción vigente es
        /// una de las tres y el personaje no se mueve en ella.
        /// </summary>
        [Test]
        public void NarrativeSequence_INC150_QuienTrabajaJuntoAlFuegoLoMira()
        {
            const float alcance = 0.15f;
            var gestos = new[] { ActorAction.Kneel, ActorAction.Blow, ActorAction.PickUp };
            var comprobadas = 0;
            var deEspaldas = new List<string>();
            foreach (var sequence in TodasLasSecuencias())
            {
                var llamas = sequence.Props.Where(prop => prop != null && prop.Glows).ToArray();
                if (llamas.Length == 0)
                {
                    continue;
                }

                foreach (var prop in Personajes(sequence))
                {
                    for (var linea = 0; linea < sequence.Lines.Length; linea++)
                    {
                        var cue = ActorTimeline.Cue(prop, linea, speaking: false);
                        if (cue.Moves || Array.IndexOf(gestos, cue.During) < 0)
                        {
                            continue;
                        }

                        var x = cue.From.x;
                        var llama = llamas.OrderBy(candidata => Mathf.Abs(candidata.Position.x - x)).First();
                        if (Mathf.Abs(llama.Position.x - x) > alcance)
                        {
                            continue;
                        }

                        comprobadas++;
                        var debeMirarIzquierda = llama.Position.x < x;
                        if (ActorTimeline.FacesLeftAt(prop, linea) != debeMirarIzquierda)
                        {
                            deEspaldas.Add(FormattableString.Invariant(
                                $"{sequence.Id} · {prop.Actor.name} L{linea} ({cue.During}) en x={x:0.000}: la llama está en x={llama.Position.x:0.000} y mira {(debeMirarIzquierda ? "a la derecha" : "a la izquierda")}"));
                        }
                    }
                }
            }

            Assert.That(comprobadas, Is.GreaterThan(0), "las narrativas tienen a alguien arrodillado, soplando o recogiendo junto a una fogata");
            Assert.That(deEspaldas, Is.Empty, "de espaldas al fuego");
        }

        // --- helpers -----------------------------------------------------------------------

        private static IEnumerable<NarrativeSequence> TodasLasSecuencias() =>
            AssetDatabase.FindAssets($"t:{nameof(NarrativeSequence)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NarrativeSequence>)
                .Where(sequence => sequence != null);

        /// <summary>
        /// Parte el texto en oraciones. Los signos de apertura «¿» y «¡» no cierran nada, así que
        /// solo cuentan los de cierre más el punto y los puntos suspensivos.
        /// </summary>
        private static IEnumerable<string> Oraciones(string text) => (text ?? string.Empty)
            .Replace("…", ".")
            .Split(new[] { '.', '!', '?' }, System.StringSplitOptions.RemoveEmptyEntries)
            .Select(oracion => oracion.Trim())
            .Where(oracion => oracion.Length > 0);

        private static int Palabras(string oracion) =>
            oracion.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
