using System;
using SudokuApp.Models;
using SudokuApp.Solvers;
using SudokuApp.Utils;

namespace SudokuApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Sudoku - jugar y resolver (Backtracking)");
            var printer = new SudokuPrinter();
            ISudokuSolver solver = new BacktrackingSudokuSolver();

            var board = new SudokuBoard();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Opciones: load | input | play | solve | show | clear | exit");
                Console.Write("cmd> ");
                var cmd = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(cmd)) continue;
                if (cmd == "exit") break;

                try
                {
                    switch (cmd)
                    {
                        case "load":
                            Console.WriteLine("Cargando ejemplo fácil...");
                            board = LoadExample();
                            printer.Print(board);
                            break;
                        case "input":
                            board = ReadManualBoard(printer);
                            break;
                        case "play":
                            PlayLoop(board, printer);
                            break;
                        case "solve":
                            Console.WriteLine("Calculando solución (backtracking)...");
                            var solved = solver.Solve(board);
                            if (solved == null) Console.WriteLine("No se encontró solución.");
                            else
                            {
                                Console.WriteLine("Solución encontrada. Presiona Enter para mostrarla...");
                                Console.ReadLine();
                                printer.Print(solved);
                            }
                            break;
                        case "show":
                            printer.Print(board);
                            break;
                        case "clear":
                            board = new SudokuBoard();
                            Console.WriteLine("Tablero limpio.");
                            break;
                        default:
                            Console.WriteLine("Comando no reconocido.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private static SudokuBoard LoadExample()
        {
            // ejemplo medium (0 = vacío)
            int[][] rows = new int[9][]
            {
                new[] {5,3,0,0,7,0,0,0,0},
                new[] {6,0,0,1,9,5,0,0,0},
                new[] {0,9,8,0,0,0,0,6,0},
                new[] {8,0,0,0,6,0,0,0,3},
                new[] {4,0,0,8,0,3,0,0,1},
                new[] {7,0,0,0,2,0,0,0,6},
                new[] {0,6,0,0,0,0,2,8,0},
                new[] {0,0,0,4,1,9,0,0,5},
                new[] {0,0,0,0,8,0,0,7,9}
            };
            return SudokuBoard.FromRows(rows);
        }

        private static SudokuBoard ReadManualBoard(SudokuPrinter printer)
        {
            Console.WriteLine("Introduce 9 líneas con 9 números (0 o . para vacío), separados por espacios.");
            var rows = new int[9][];
            for (int r = 0; r < 9; r++)
            {
                Console.Write($"Fila {r + 1}: ");
                var line = Console.ReadLine() ?? string.Empty;
                var parts = line.Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 9) { Console.WriteLine("Fila inválida, intenta de nuevo."); r--; continue; }
                rows[r] = new int[9];
                for (int c = 0; c < 9; c++) rows[r][c] = parts[c] == "." ? 0 : int.Parse(parts[c]);
            }
            var b = SudokuBoard.FromRows(rows);
            Console.WriteLine("Tablero cargado:");
            printer.Print(b);
            return b;
        }

        private static void PlayLoop(SudokuBoard board, SudokuPrinter printer)
        {
            printer.Print(board);
            while (true)
            {
                Console.Write("play> (r c v / undo / exit): ");
                var line = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                if (line.Equals("undo", StringComparison.OrdinalIgnoreCase)) { Console.WriteLine("Undo no implementado en esta versión."); continue; }

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3) { Console.WriteLine("Formato: r c v (ej: 1 3 5)"); continue; }
                int r = int.Parse(parts[0]) - 1;
                int c = int.Parse(parts[1]) - 1;
                int v = int.Parse(parts[2]);
                if (r < 0 || r >= 9 || c < 0 || c >= 9) { Console.WriteLine("Coordenadas fuera de rango (1..9)"); continue; }
                if (!board.IsEmpty(r, c)) { Console.WriteLine("Celda no vacía"); continue; }
                if (!board.IsValidPlacement(r, c, v)) { Console.WriteLine("Movimiento inválido según reglas Sudoku"); continue; }
                board.Set(r, c, v);
                printer.Print(board);
                if (!board.EmptyCells().Any()) Console.WriteLine("¡Tablero completo! Usar 'solve' para validar o revisar.");
            }
        }
    }
}
