using System;
using System.Diagnostics;
using EightQueensApp.Solvers;
using EightQueensApp.Utils;

namespace EightQueensApp
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			int n = 8;
			if (args.Length > 0 && int.TryParse(args[0], out var parsed) && parsed > 0)
			{
				n = parsed;
			}

			IQueenSolver solver = new BacktrackingSolver();

			var sw = Stopwatch.StartNew();
			var solutions = solver.Solve(n);
			sw.Stop();

			Console.WriteLine($"N={n} -> {solutions.Count} soluciones en {sw.ElapsedMilliseconds} ms");

			var printer = new SolutionPrinter();
			var show = Math.Min(5, solutions.Count);
			for (int i = 0; i < show; i++)
			{
				Console.WriteLine($"Solución {i + 1}: {printer.Format(solutions[i])}");
				printer.Print(solutions[i]);
			}

			Console.WriteLine("Fin.");
		}
	}
}
