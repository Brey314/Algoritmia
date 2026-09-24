namespace Game.Scaffolding
{
    /// <summary>
    /// Lo que hace un personaje: cada valor es un estado de su Animator con el mismo nombre
    /// (<c>char_papa_anim_golpear</c> vive en el estado <c>Strike</c>). Un rig que no tiene el
    /// estado pedido cae a <see cref="Idle"/> (Dirección de arte §13.3).
    /// </summary>
    /// <remarks>
    /// **No existen saltar, caer ni derrota** (CT-06, RNF-02, CP-02): tras un intento sin éxito
    /// el personaje hace <see cref="Encourage"/>, nunca un gesto de desánimo. Celebrar rebota
    /// sin despegar los pies. Los valores son explícitos porque los assets los guardan como
    /// número: añadir uno nuevo va al final.
    /// </remarks>
    public enum ActorAction
    {
        /// <summary>De pie, respirando: el reposo de todos (§13.3, ciclo de 3 s).</summary>
        Idle = 0,

        /// <summary>No se ve. Algoritm antes de aparecer.</summary>
        Hidden = 1,

        /// <summary>Camina. El desplazamiento lo pone quien lo mueve; el clip es el paso.</summary>
        Walk = 2,

        /// <summary>Corre: el paso, más rápido y más amplio.</summary>
        Run = 3,

        /// <summary>Habla: gesto de la mano y leve balanceo mientras su línea está en pantalla.</summary>
        Talk = 4,

        /// <summary>Golpea las piedras entre sí delante del pecho (Papá, la Niña en la 1.2).</summary>
        Strike = 5,

        /// <summary>Martilla con un brazo, de arriba abajo (el taller del Nivel 2).</summary>
        Hammer = 6,

        /// <summary>Agachado, sopla sobre el montón (Papá, §13.3, 0,9 s).</summary>
        Blow = 7,

        /// <summary>Se agacha, recoge algo del suelo y se levanta (Mamá, §13.3, 0,5 s).</summary>
        PickUp = 8,

        /// <summary>Arrodillado, quieto: busca a tientas, mira las llamas.</summary>
        Kneel = 9,

        /// <summary>Carga algo pesado sobre los hombros mientras camina (Papá con los troncos).</summary>
        Carry = 10,

        /// <summary>Empuja con las dos manos, inclinado (la caja, la carretilla).</summary>
        Push = 11,

        /// <summary>Señala con el brazo extendido (§13.3, 0,6 s).</summary>
        Point = 12,

        /// <summary>Mira con la mano en la frente y la cabeza inclinada (la Niña, §7.4).</summary>
        Observe = 13,

        /// <summary>Brazos arriba y rebote sin despegar los pies (§13.3, 1,2 s).</summary>
        Celebrate = 14,

        /// <summary>Puño arriba: el «ánimo» tras un intento sin éxito, nunca derrota (§7.3, CP-02).</summary>
        Encourage = 15,

        /// <summary>Abraza: los dos brazos hacia el centro.</summary>
        Hug = 16,

        /// <summary>Sorpresa: se estira y abre los brazos un poco.</summary>
        Surprise = 17,

        /// <summary>Duerme recogido, respirando despacio.</summary>
        Sleep = 18,

        /// <summary>Aparece: crece desde el centro y se hace visible. Después flota.</summary>
        Appear = 19,

        /// <summary>Se apaga: parpadea despacio y desaparece. Se queda oculto.</summary>
        Vanish = 20,

        /// <summary>Gira sobre sí mismo como un trompo (Algoritm, §1.4.1).</summary>
        Spin = 21
    }
}
