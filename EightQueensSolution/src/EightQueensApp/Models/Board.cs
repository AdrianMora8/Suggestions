using System;
using System.Linq;

namespace EightQueensApp.Models
{
    /// <summary>
    /// Representa un tablero NxN para el problema de las N-reinas.
    /// Internamente se mantiene un array donde el índice es la fila y el valor la columna de la reina.
    /// </summary>
    public sealed class Board
    {
        private readonly int[] _queens;

        public int Size { get; }

        public Board(int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            Size = size;
            _queens = Enumerable.Repeat(-1, size).ToArray();
        }

        private Board(int size, int[] queens)
        {
            Size = size;
            _queens = queens;
        }

        /// <summary>
        /// Copia de las posiciones de las reinas: índice = fila, valor = columna
        /// </summary>
        public int[] Queens => (int[])_queens.Clone();

        /// <summary>
        /// Comprueba si colocar una reina en (row,col) es seguro con respecto a las filas previas.
        /// Asumimos que solo se colocan reinas fila a fila desde 0..row-1.
        /// </summary>
        public bool IsSafe(int row, int col)
        {
            if (row < 0 || row > Size) throw new ArgumentOutOfRangeException(nameof(row));
            if (col < 0 || col >= Size) throw new ArgumentOutOfRangeException(nameof(col));

            for (int r = 0; r < row; r++)
            {
                int c = _queens[r];
                if (c == col) return false; // misma columna
                if (Math.Abs(c - col) == Math.Abs(r - row)) return false; // diagonal
            }

            return true;
        }

        public void PlaceQueen(int row, int col)
        {
            _queens[row] = col;
        }

        public void RemoveQueen(int row)
        {
            _queens[row] = -1;
        }

        public Board Clone()
        {
            return new Board(Size, (int[])_queens.Clone());
        }

        public override string ToString()
        {
            return string.Join(',', Queens.Select(i => i.ToString()));
        }
    }
}
