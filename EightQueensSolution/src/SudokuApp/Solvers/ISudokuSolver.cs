using SudokuApp.Models;

namespace SudokuApp.Solvers
{
    public interface ISudokuSolver
    {
        /// <summary>
        /// Devuelve un tablero solucionado a partir del inicial, o null si no tiene solución.
        /// </summary>
        SudokuBoard Solve(SudokuBoard board);
    }
}
