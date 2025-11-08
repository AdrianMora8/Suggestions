using System;
using SudokuApp.Models;

namespace SudokuApp.Utils
{
    public sealed class SudokuPrinter
    {
        public void Print(SudokuBoard board)
        {
            for (int r = 0; r < SudokuBoard.Size; r++)
            {
                if (r % 3 == 0) Console.WriteLine("+-------+-------+-------+");
                for (int c = 0; c < SudokuBoard.Size; c++)
                {
                    if (c % 3 == 0) Console.Write("| ");
                    var v = board.Get(r, c);
                    Console.Write(v == 0 ? ". " : v + " ");
                }
                Console.WriteLine("|");
            }
            Console.WriteLine("+-------+-------+-------+");
        }
    }
}
