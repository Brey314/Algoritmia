using System;
using System.Collections.Generic;

namespace Game.Levels.Wheel
{
    /// <summary>Los tres bloques del laberinto del mockup 10: avanzar y retroceder (con cuenta) y girar (con lado).</summary>
    public enum BlockKind
    {
        Forward,
        Backward,
        Turn
    }

    public enum TurnDirection
    {
        Left,
        Right
    }

    /// <summary>
    /// Un bloque encajable con su parámetro: «Avanzar ×n» y «Retroceder ×n» (1..9), «Girar» a la
    /// izquierda o a la derecha (mockup 10 · Nivel 2 · laberinto, ajustado por Santiago el
    /// 13/09/2026: retroceder con cuenta en vez de recoger). Relativo a la carretilla, nunca
    /// absoluto (INC-33).
    /// </summary>
    /// <remarks>
    /// Es un valor: editar la cuenta o el lado devuelve otro bloque y la secuencia lo reemplaza
    /// en su sitio (<see cref="BlockSequence.Replace"/>). Respecto de RF-31 («avanzar,
    /// retroceder, girar horario») añade la cuenta y el lado del giro, por decisión de Santiago
    /// del 13/09/2026: el mockup manda y el RF queda por precisar en los documentos.
    /// </remarks>
    public readonly struct InstructionBlock : IEquatable<InstructionBlock>
    {
        public const int MinCount = 1;
        public const int MaxCount = 9;

        public BlockKind Kind { get; }

        /// <summary>Cuántas casillas se mueve. Solo significa algo en <see cref="BlockKind.Forward"/> y <see cref="BlockKind.Backward"/>.</summary>
        public int Count { get; }

        /// <summary>Hacia qué lado gira. Solo significa algo en <see cref="BlockKind.Turn"/>.</summary>
        public TurnDirection Direction { get; }

        private InstructionBlock(BlockKind kind, int count, TurnDirection direction)
        {
            Kind = kind;
            Count = Math.Clamp(count, MinCount, MaxCount);
            Direction = direction;
        }

        public static InstructionBlock Forward(int count = 1) => new InstructionBlock(BlockKind.Forward, count, TurnDirection.Right);

        public static InstructionBlock Backward(int count = 1) => new InstructionBlock(BlockKind.Backward, count, TurnDirection.Right);

        public static InstructionBlock Turn(TurnDirection direction = TurnDirection.Right) => new InstructionBlock(BlockKind.Turn, 1, direction);

        /// <summary>El bloque tal como sale de la paleta.</summary>
        public static InstructionBlock Default(BlockKind kind) => kind switch
        {
            BlockKind.Forward => Forward(),
            BlockKind.Backward => Backward(),
            _ => Turn()
        };

        public InstructionBlock WithCount(int count) => new InstructionBlock(Kind, count, Direction);

        public InstructionBlock WithDirection(TurnDirection direction) => new InstructionBlock(Kind, Count, direction);

        public bool Equals(InstructionBlock other) => Kind == other.Kind && Count == other.Count && Direction == other.Direction;

        public override bool Equals(object obj) => obj is InstructionBlock other && Equals(other);

        public override int GetHashCode() => HashCode.Combine((int)Kind, Count, (int)Direction);

        public override string ToString() => Kind switch
        {
            BlockKind.Forward => $"Avanzar ×{Count}",
            BlockKind.Backward => $"Retroceder ×{Count}",
            _ => Direction == TurnDirection.Left ? "Girar izquierda" : "Girar derecha"
        };
    }

    /// <summary>
    /// La secuencia de bloques como dato: añadir, insertar, retirar, reordenar y editar en su
    /// sitio (RF-31, RF-34). Sin interfaz y sin ejecución: solo la estructura, que es lo que
    /// hace verificable RF-34 sin escena.
    /// </summary>
    /// <remarks>
    /// **No hay tope de bloques ni de ediciones**, y no es un olvido: un límite sería una
    /// penalización disfrazada (CP-02, RF-18). La secuencia y el laberinto son estados separados
    /// a propósito: editarla no reinicia nada (CU-08 FA-6a).
    /// </remarks>
    public class BlockSequence
    {
        private readonly List<InstructionBlock> _blocks = new List<InstructionBlock>();

        public IReadOnlyList<InstructionBlock> Blocks => _blocks;

        public int Count => _blocks.Count;

        public bool IsEmpty => _blocks.Count == 0;

        public InstructionBlock this[int index] => _blocks[index];

        /// <summary>Engancha un bloque al final: es el orden en que se sueltan (guion §6.3.2).</summary>
        public void Add(InstructionBlock block) => _blocks.Add(block);

        /// <summary>Engancha un bloque en una posición; fuera de rango cae al extremo más cercano.</summary>
        public void Insert(int index, InstructionBlock block) =>
            _blocks.Insert(Math.Clamp(index, 0, _blocks.Count), block);

        /// <summary>Retira un bloque; el resto conserva su orden.</summary>
        public InstructionBlock RemoveAt(int index)
        {
            var block = _blocks[index];
            _blocks.RemoveAt(index);
            return block;
        }

        /// <summary>Reubica un bloque: lo retira de donde está y lo engancha en la posición nueva.</summary>
        public void Move(int from, int to) => Insert(to, RemoveAt(from));

        /// <summary>Edita un bloque en su sitio: la cuenta de «Avanzar» o el lado de «Girar».</summary>
        public void Replace(int index, InstructionBlock block) => _blocks[index] = block;

        public void Clear() => _blocks.Clear();
    }
}
