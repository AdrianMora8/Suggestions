using System;
using System.Windows.Forms;
using EightPuzzleApp.Forms;

namespace EightPuzzleApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EightPuzzleForm());
        }
    }
}
