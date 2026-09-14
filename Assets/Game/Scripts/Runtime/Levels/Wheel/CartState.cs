using System;
using UnityEngine;

namespace Game.Levels.Wheel
{
    /// <summary>Hacia dónde mira la carretilla sobre la rejilla. En sentido horario: girar es +1.</summary>
    public enum Orientation
    {
        North,
        East,
        South,
        West
    }

    /// <summary>
    /// La carretilla en el laberinto: casilla **y** orientación (RF-31, INC-33). Es un valor:
    /// cada instrucción devuelve el estado siguiente y no toca el anterior, lo que hace trivial
    /// «intentar y regresar» (RF-33).
    /// </summary>
    /// <remarks>
    /// Nada aquí lee una dirección absoluta: «Avanzar» y «Retroceder» se resuelven contra
    /// <see cref="Facing"/>. Leer «Avanzar» como «arriba» compila, se ve razonable y hace el
    /// refugio inalcanzable — es el error que INC-33 cerró y que la prueba
    /// <c>CartState_RF31_AvanzarEsRelativoALaOrientacionNoAbsoluto</c> vigila.
    /// </remarks>
    public readonly struct CartState : IEquatable<CartState>
    {
        /// <summary>Columna desde la izquierda, fila desde abajo — el mismo sentido que las anclas de uGUI.</summary>
        public Vector2Int Cell { get; }

        public Orientation Facing { get; }

        public CartState(Vector2Int cell, Orientation facing)
        {
            Cell = cell;
            Facing = facing;
        }

        /// <summary>Una casilla en la dirección dada.</summary>
        public static Vector2Int Delta(Orientation facing) => facing switch
        {
            Orientation.North => Vector2Int.up,
            Orientation.East => Vector2Int.right,
            Orientation.South => Vector2Int.down,
            _ => Vector2Int.left
        };

        /// <summary>«Avanzar»: una casilla hacia donde mira, sin girar.</summary>
        public CartState Ahead() => new CartState(Cell + Delta(Facing), Facing);

        /// <summary>«Retroceder»: una casilla hacia atrás respecto de donde mira, **sin cambiar** la orientación.</summary>
        public CartState Behind() => new CartState(Cell - Delta(Facing), Facing);

        /// <summary>«Girar» a la derecha: 90° en sentido horario, sin moverse. Cuatro giros vuelven al inicio.</summary>
        public CartState TurnedClockwise() => new CartState(Cell, (Orientation)(((int)Facing + 1) % 4));

        /// <summary>«Girar» a la izquierda: 90° en sentido antihorario, sin moverse.</summary>
        public CartState TurnedCounterclockwise() => new CartState(Cell, (Orientation)(((int)Facing + 3) % 4));

        public bool Equals(CartState other) => Cell == other.Cell && Facing == other.Facing;

        public override bool Equals(object obj) => obj is CartState other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Cell, (int)Facing);

        public override string ToString() => $"({Cell.x},{Cell.y}) {Facing}";
    }
}
