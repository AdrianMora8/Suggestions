using System;
using System.Windows.Forms;
using CookieGameApp.Forms;

namespace CookieGameApp
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación Dots and Boxes (Juego de la Galleta).
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Configuración de la aplicación Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Iniciar el juego Dots and Boxes
            Application.Run(new DotsAndBoxesForm());
        }
    }
}
