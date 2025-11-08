using System;
using System.Windows.Forms;
using SudokuApp.Forms;

namespace SudokuApp
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación Sudoku con interfaz gráfica.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Configuración de la aplicación Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Iniciar la aplicación Sudoku con interfaz gráfica
            Application.Run(new SudokuForm());
        }
    }
}
