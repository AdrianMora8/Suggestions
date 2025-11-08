using System.Collections.Generic;
using EightQueensApp.Models;

namespace EightQueensApp.Solvers
{
    /// <summary>
    /// Implementación clásica por Backtracking que genera todas las soluciones para N reinas.
    /// Es simple, eficiente para N=8 y suficientemente clara para fines pedagógicos.
    /// </summary>
    public sealed class BacktrackingSolver : IQueenSolver
    {
        public IReadOnlyList<Board> Solve(int n)
        {
            var results = new List<Board>();
            var board = new Board(n);
            PlaceRow(0, board, results);
            return results;
        }

        private void PlaceRow(int row, Board board, List<Board> results)
        {
            int n = board.Size;
            if (row == n)
            {
                // solución completa — clonamos para guardar el estado
                results.Add(board.Clone());
                return;
            }

            for (int col = 0; col < n; col++)
            {
                if (board.IsSafe(row, col))
                {
                    board.PlaceQueen(row, col);
                    PlaceRow(row + 1, board, results);
                    board.RemoveQueen(row);
                }
            }
        }
    }
}
