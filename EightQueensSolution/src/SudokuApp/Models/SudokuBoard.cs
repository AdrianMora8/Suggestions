using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuApp.Models
{
    /// <summary>
    /// Representa un tablero Sudoku 9x9. Zeros representan celdas vacías.
    /// Provee validación de movimientos, clonación y utilidades de parseo.
    /// </summary>
    public sealed class SudokuBoard
    {
        public const int Size = 9;
        private readonly int[,] _cells; // 9x9

        public SudokuBoard()
        {
            _cells = new int[Size, Size];
        }

        private SudokuBoard(int[,] cells)
        {
            _cells = (int[,])cells.Clone();
        }

        public int Get(int row, int col) => _cells[row, col];

        public void Set(int row, int col, int value)
        {
            _cells[row, col] = value;
        }

        public bool IsEmpty(int row, int col) => _cells[row, col] == 0;

        public bool IsValidPlacement(int row, int col, int value)
        {
            if (value < 1 || value > 9) return false;
            if (!IsEmpty(row, col) && _cells[row, col] != 0) return false;

            // check row
            for (int c = 0; c < Size; c++) if (_cells[row, c] == value) return false;
            // check column
            for (int r = 0; r < Size; r++) if (_cells[r, col] == value) return false;
            // check box
            int br = (row / 3) * 3;
            int bc = (col / 3) * 3;
            for (int r = br; r < br + 3; r++)
                for (int c = bc; c < bc + 3; c++)
                    if (_cells[r, c] == value) return false;

            return true;
        }

        public IEnumerable<(int row, int col)> EmptyCells()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (_cells[r, c] == 0) yield return (r, c);
        }

        public SudokuBoard Clone() => new SudokuBoard(_cells);

        public static SudokuBoard FromRows(int[][] rows)
        {
            if (rows == null) throw new ArgumentNullException(nameof(rows));
            if (rows.Length != Size) throw new ArgumentException("Debe tener 9 filas");
            var b = new SudokuBoard();
            for (int r = 0; r < Size; r++)
            {
                if (rows[r].Length != Size) throw new ArgumentException("Cada fila debe tener 9 columnas");
                for (int c = 0; c < Size; c++) b._cells[r, c] = rows[r][c];
            }
            return b;
        }

        public static SudokuBoard FromString(string s)
        {
            // acepta 81 caracteres (digitos o . para vacio) o 9 líneas separadas
            var cleaned = s.Replace("\r", "").Split('\n').SelectMany(l => l.Trim().Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries)).ToArray();
            var tokens = cleaned.Length == Size * Size ? cleaned : s.Where(ch => char.IsDigit(ch) || ch == '.').Select(ch => ch.ToString()).ToArray();

            if (tokens.Length != Size * Size) throw new ArgumentException("Entrada inválida para Sudoku");
            var rows = new int[Size][];
            for (int r = 0; r < Size; r++)
            {
                rows[r] = new int[Size];
                for (int c = 0; c < Size; c++)
                {
                    var t = tokens[r * Size + c];
                    rows[r][c] = (t == ".") ? 0 : int.Parse(t);
                }
            }
            return FromRows(rows);
        }
    }
}
