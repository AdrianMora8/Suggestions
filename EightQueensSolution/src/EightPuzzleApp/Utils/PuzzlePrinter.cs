using System;
using System.Linq;
using EightPuzzleApp.Models;

namespace EightPuzzleApp.Utils
{
    public sealed class PuzzlePrinter
    {
        public void Print(PuzzleState state)
        {
            var cells = state.Cells;
            for (int r = 0; r < PuzzleState.Size; r++)
            {
                var row = Enumerable.Range(0, PuzzleState.Size)
                    .Select(c => cells[r * PuzzleState.Size + c] == 0 ? '.' : (char)('0' + cells[r * PuzzleState.Size + c]))
                    .Select(ch => ch.ToString());
                Console.WriteLine(string.Join(' ', row));
            }
        }
    }
}
