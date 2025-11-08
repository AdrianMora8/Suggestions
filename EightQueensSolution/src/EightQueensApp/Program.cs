using System;
using System.Windows.Forms;
using EightQueensApp.Forms;

namespace EightQueensApp
{
	internal static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			ApplicationConfiguration.Initialize();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new EightQueensForm());
		}
	}
}
