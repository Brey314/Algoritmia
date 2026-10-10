using UnityEngine;

namespace Game.Scaffolding
{
    /// <summary>
    /// Cómo se pinta la tarjeta del cuadro de diálogo para un hablante: el retrato fijo de siempre o la
    /// base sin cara con la cara animada encima (INC-148, 10/10/2026). C# plano: la decisión se prueba en
    /// EditMode sin escena, y <c>NarrativeSceneController</c> solo la ejecuta.
    /// </summary>
    /// <remarks>
    /// **La tarjeta es animada solo si tiene con qué serlo:** una base (<see cref="CharacterRig.PortraitBase"/>),
    /// un set de cara con al menos los ojos neutros y un recuadro de cara que cabe en la base. Si falta
    /// cualquiera de las tres cosas —el arte llega por entregas y los siete prefabs se completan en la ronda del
    /// Editor— la tarjeta cae al retrato fijo, que siempre estuvo ahí: ninguna línea se queda sin retrato ni
    /// pinta una cara a medias sobre una base sin cara. Sin set con ojos neutros tampoco habría con qué dibujar
    /// la cara (<see cref="CharacterFaceSet.Eyes(FacialEmotion)"/> devolvería <c>null</c> en todas las
    /// emociones).
    /// </remarks>
    public readonly struct PortraitLook
    {
        /// <summary>
        /// Lo que cabe de más en el cuadro unidad, por error de redondeo del flotante: una cara que termina en
        /// 1,0000001 por haberse escrito como x + ancho sigue siendo una cara que cabe.
        /// </summary>
        private const float Tolerance = 0.0001f;

        private PortraitLook(Sprite art, bool animated, Rect face, CharacterFaceSet set)
        {
            Art = art;
            Animated = animated;
            Face = face;
            Set = set;
        }

        /// <summary>El sprite de la tarjeta: la base sin cara si es animada, el retrato fijo si no. Puede ser <c>null</c> si no hay ni uno.</summary>
        public Sprite Art { get; }

        /// <summary>Si sobre <see cref="Art"/> se pinta la cara animada.</summary>
        public bool Animated { get; }

        /// <summary>El recuadro de la cara dentro de <see cref="Art"/>, normalizado y con el origen abajo a la izquierda. Vacío si no es animada.</summary>
        public Rect Face { get; }

        /// <summary>El set con los ojos y las bocas de la cara. <c>null</c> si no es animada.</summary>
        public CharacterFaceSet Set { get; }

        /// <summary>
        /// Resuelve la tarjeta de un hablante.
        /// </summary>
        /// <param name="portraitBase">La base sin cara, cuadrada; nula si todavía no llegó.</param>
        /// <param name="face">Dónde va la cara dentro de la base, normalizado (origen abajo a la izquierda).</param>
        /// <param name="set">El set de cara del hablante.</param>
        /// <param name="still">El retrato fijo, al que cae la tarjeta si no es animada.</param>
        public static PortraitLook Of(Sprite portraitBase, Rect face, CharacterFaceSet set, Sprite still)
        {
            // Comparaciones explícitas con null: «?.» y «??» se saltan la comprobación de objetos destruidos.
            var animated = portraitBase != null && set != null && set.EyesNeutral != null && FitsInUnitSquare(face);
            return animated
                ? new PortraitLook(portraitBase, true, face, set)
                : new PortraitLook(still, false, default, null);
        }

        /// <summary>Un recuadro de tamaño positivo que no se sale del cuadro de 0 a 1 en ningún eje.</summary>
        private static bool FitsInUnitSquare(Rect face) =>
            face.width > 0f && face.height > 0f
            && face.xMin >= -Tolerance && face.yMin >= -Tolerance
            && face.xMax <= 1f + Tolerance && face.yMax <= 1f + Tolerance;
    }
}
