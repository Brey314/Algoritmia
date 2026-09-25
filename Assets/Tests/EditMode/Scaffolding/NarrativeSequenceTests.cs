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

        private static bool Anima(NarrativeProp prop, string clip) =>
            prop.FrameAnimation != null && prop.FrameAnimation.name.StartsWith(clip, StringComparison.Ordinal);

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
        /// es quedar tapado.
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
            }
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
