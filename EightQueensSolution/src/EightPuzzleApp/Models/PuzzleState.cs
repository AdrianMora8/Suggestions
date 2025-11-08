using System;
using System.Collections.Generic;
using System.Linq;

namespace EightPuzzleApp.Models
{
    /// <summary>
    /// Representa un estado del 8-puzzle (tablero 3x3). El 0 representa el hueco.
    /// Internamente guardamos un arreglo de 9 enteros en orden fila-major.
    /// </summary>
    public sealed class PuzzleState : IEquatable<PuzzleState>
    {
        private readonly byte[] _cells; // 0..8 values, 0 == blank

        public const int Size = 3; // width/height
        public const int Count = Size * Size;

        public PuzzleState(byte[] cells)
        {
            if (cells == null) throw new ArgumentNullException(nameof(cells));
            if (cells.Length != Count) throw new ArgumentException($"Se requieren {Count} celdas");
            _cells = (byte[])cells.Clone();
        }

        public byte[] Cells => (byte[])_cells.Clone();

        public int BlankIndex => Array.IndexOf(_cells, (byte)0);

        public bool IsGoal()
        {
            // goal: 1,2,3,4,5,6,7,8,0
            for (int i = 0; i < Count - 1; i++)
            {
                if (_cells[i] != i + 1) return false;
            }
            return _cells[Count - 1] == 0;
        }

        public IEnumerable<(PuzzleState Next, string Action)> GetNeighbors()
        {
            int bi = BlankIndex;
            int r = bi / Size;
            int c = bi % Size;

            (int nr, int nc, string action)[] moves = new[]
            {
                (r - 1, c, "Up"),
                (r + 1, c, "Down"),
                (r, c - 1, "Left"),
                (r, c + 1, "Right")
            };

            foreach (var (nr, nc, action) in moves)
            {
                if (nr >= 0 && nr < Size && nc >= 0 && nc < Size)
                {
                    int ni = nr * Size + nc;
                    var copy = (byte[])_cells.Clone();
                    // swap blank and neighbor
                    copy[bi] = copy[ni];
                    copy[ni] = 0;
                    yield return (new PuzzleState(copy), action);
                }
            }
        }

        public PuzzleState Move(string action)
        {
            foreach (var (next, act) in GetNeighbors())
            {
                if (string.Equals(act, action, StringComparison.OrdinalIgnoreCase)) return next;
            }
            throw new InvalidOperationException("Acción no válida");
        }

        public static PuzzleState FromInts(params int[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length != Count) throw new ArgumentException($"Se requieren {Count} valores");
            var bytes = new byte[Count];
            for (int i = 0; i < Count; i++) bytes[i] = (byte)values[i];
            return new PuzzleState(bytes);
        }

        public PuzzleState Clone() => new PuzzleState(_cells);

        public string Key() => string.Join(',', _cells);

        public override string ToString() => Key();

        public bool Equals(PuzzleState other)
        {
            if (other == null) return false;
            for (int i = 0; i < Count; i++) if (_cells[i] != other._cells[i]) return false;
            return true;
        }

        public override bool Equals(object obj) => Equals(obj as PuzzleState);

        public override int GetHashCode()
        {
            unchecked
            {
                int h = 17;
                for (int i = 0; i < Count; i++) h = h * 31 + _cells[i];
                return h;
            }
        }

        public static bool IsSolvable(byte[] cells)
        {
            // For odd grid width (3), puzzle solvable iff inversion count even
            int inv = 0;
            var arr = cells.Where(x => x != 0).ToArray();
            for (int i = 0; i < arr.Length; i++)
                for (int j = i + 1; j < arr.Length; j++)
                    if (arr[i] > arr[j]) inv++;
            return inv % 2 == 0;
        }
    }
}
