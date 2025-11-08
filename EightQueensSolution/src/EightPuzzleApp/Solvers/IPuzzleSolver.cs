using System.Collections.Generic;
using EightPuzzleApp.Models;

namespace EightPuzzleApp.Solvers
{
    public interface IPuzzleSolver
    {
        /// <summary>
        /// Devuelve la lista de estados desde inicial hasta objetivo (inclusive), o null si no hay solución.
        /// </summary>
        IList<PuzzleState> Solve(PuzzleState start);
    }
}
