using System;
using System.Linq;
using EightQueensApp.Models;

namespace EightQueensApp.Utils
{
    /// <summary>
    /// Clase responsable únicamente de formatear e imprimir soluciones.
    /// Sigue SRP: separación de responsabilidades respecto al solver.
    /// </summary>
    public sealed class SolutionPrinter
    {
        public void Print(Board board)
        {
            for (int r = 0; r < board.Size; r++)
            {
                var row = Enumerable.Range(0, board.Size)
                    .Select(c => board.Queens[r] == c ? 'Q' : '.')
                    .Select(ch => ch.ToString());
                Console.WriteLine(string.Join(' ', row));
            }
            Console.WriteLine();
        }

        public string Format(Board board)
        {
            // representación compacta: lista de columnas por fila
            return string.Join(',', board.Queens.Select(i => i.ToString()));
        }
    }
}
