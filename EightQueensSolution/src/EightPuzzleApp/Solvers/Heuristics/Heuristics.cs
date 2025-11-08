using System;
using System.Linq;
using EightPuzzleApp.Models;

namespace EightPuzzleApp.Solvers
{
    public static class Heuristics
    {
        public static int Manhattan(EightPuzzleApp.Models.PuzzleState state)
        {
            var cells = state.Cells;
            int sum = 0;
            for (int i = 0; i < EightPuzzleApp.Models.PuzzleState.Count; i++)
            {
                int val = cells[i];
                if (val == 0) continue;
                int goalIndex = val - 1;
                int r1 = i / EightPuzzleApp.Models.PuzzleState.Size, c1 = i % EightPuzzleApp.Models.PuzzleState.Size;
                int r2 = goalIndex / EightPuzzleApp.Models.PuzzleState.Size, c2 = goalIndex % EightPuzzleApp.Models.PuzzleState.Size;
                sum += Math.Abs(r1 - r2) + Math.Abs(c1 - c2);
            }
            return sum;
        }
    }
}
