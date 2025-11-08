using System;
using System.Collections.Generic;
using System.Linq;
using SudokuApp.Models;

namespace SudokuApp.Solvers
{
    /// <summary>
    /// Solver clásico por backtracking que encuentra una solución del Sudoku (si existe).
    /// </summary>
    public sealed class BacktrackingSudokuSolver : ISudokuSolver
    {
        public SudokuBoard Solve(SudokuBoard board)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            var copy = board.Clone();
            if (SolveInternal(copy)) return copy;
            return null;
        }

        private bool SolveInternal(SudokuBoard b)
        {
            var empty = b.EmptyCells().FirstOrDefault();
            if (empty == default) return true; // no hay vacías, resuelto

            int r = empty.row, c = empty.col;
            for (int v = 1; v <= 9; v++)
            {
                if (b.IsValidPlacement(r, c, v))
                {
                    b.Set(r, c, v);
                    if (SolveInternal(b)) return true;
                    b.Set(r, c, 0);
                }
            }

            return false;
        }
    }
}
