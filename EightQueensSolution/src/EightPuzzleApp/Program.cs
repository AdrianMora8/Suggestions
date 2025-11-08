using System;
using System.Linq;
using EightPuzzleApp.Models;
using EightPuzzleApp.Solvers;
using EightPuzzleApp.Utils;

namespace EightPuzzleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("8-Puzzle interactive + solver");
            var printer = new PuzzlePrinter();
            var solver = new AStarSolver();

            PuzzleState current = null;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Menu:");
                Console.WriteLine("1) Ingresar estado inicial manualmente");
                Console.WriteLine("2) Generar estado aleatorio (barajar)");
                Console.WriteLine("3) Jugar interactivo (mover con Up/Down/Left/Right)");
                Console.WriteLine("4) Que el computador resuelva (A*)");
                Console.WriteLine("5) Mostrar estado actual");
                Console.WriteLine("0) Salir");
                Console.Write("Elige una opción: ");
                var opt = Console.ReadLine()?.Trim();
                if (opt == "0") break;

                try
                {
                    switch (opt)
                    {
                        case "1":
                            current = ReadManualState();
                            Console.WriteLine("Estado establecido:");
                            printer.Print(current);
                            break;
                        case "2":
                            current = GenerateRandom(20);
                            Console.WriteLine("Estado aleatorio generado:");
                            printer.Print(current);
                            break;
                        case "3":
                            if (current == null) { Console.WriteLine("Define primero un estado (opción 1 o 2)."); break; }
                            InteractivePlay(current, printer);
                            break;
                        case "4":
                            if (current == null) { Console.WriteLine("Define primero un estado (opción 1 o 2)."); break; }
                            Console.WriteLine("Resolviendo con A*... (puede tardar)");
                            var path = solver.Solve(current);
                            if (path == null) Console.WriteLine("No se encontró solución");
                            else
                            {
                                Console.WriteLine($"Solución encontrada: {path.Count - 1} pasos.");
                                for (int i = 0; i < path.Count; i++)
                                {
                                    Console.WriteLine($"Paso {i}:");
                                    printer.Print(path[i]);
                                }
                            }
                            break;
                        case "5":
                            if (current == null) Console.WriteLine("No hay estado actual."); else printer.Print(current);
                            break;
                        default:
                            Console.WriteLine("Opción no válida");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private static PuzzleState ReadManualState()
        {
            Console.WriteLine("Introduce 9 números separados por espacios (0 para hueco), por ejemplo: 1 2 3 4 5 6 7 8 0");
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(new[] {' ', '\t', ','}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != PuzzleState.Count) { Console.WriteLine("Se requieren 9 valores"); continue; }
                try
                {
                    var vals = parts.Select(s => int.Parse(s)).ToArray();
                    var bytes = vals.Select(v => (byte)v).ToArray();
                    if (!PuzzleState.IsSolvable(bytes)) { Console.WriteLine("El puzzle no es solvable. Intenta otro."); continue; }
                    return new PuzzleState(bytes);
                }
                catch { Console.WriteLine("Entrada inválida. Intenta de nuevo."); }
            }
        }

        private static PuzzleState GenerateRandom(int moves)
        {
            var rnd = new Random();
            var state = new PuzzleState(new byte[] {1,2,3,4,5,6,7,8,0});
            for (int i = 0; i < moves; i++)
            {
                var neigh = state.GetNeighbors().ToList();
                var pick = neigh[rnd.Next(neigh.Count)];
                state = pick.Next;
            }
            return state;
        }

        private static void InteractivePlay(PuzzleState start, PuzzlePrinter printer)
        {
            var state = start.Clone();
            printer.Print(state);
            while (true)
            {
                Console.WriteLine("Mover: Up/Down/Left/Right, show, exit");
                Console.Write("> ");
                var cmd = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(cmd)) continue;
                if (cmd.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                if (cmd.Equals("show", StringComparison.OrdinalIgnoreCase)) { printer.Print(state); continue; }
                try
                {
                    state = state.Move(cmd);
                    printer.Print(state);
                    if (state.IsGoal()) { Console.WriteLine("¡Felicidades! Resuelto."); break; }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Movimiento inválido: " + ex.Message);
                }
            }
        }
    }
}
