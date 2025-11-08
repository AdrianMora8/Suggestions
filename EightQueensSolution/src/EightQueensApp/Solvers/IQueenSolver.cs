using System.Collections.Generic;
using EightQueensApp.Models;

namespace EightQueensApp.Solvers
{
    /// <summary>
    /// Interfaz para solucionadores del problema N-reinas.
    /// Entrada: tamaño n. Salida: lista de tableros solucion.
    /// </summary>
    public interface IQueenSolver
    {
        IReadOnlyList<Board> Solve(int n);
    }
}
