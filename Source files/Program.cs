using System;
using System.Windows.Forms;

namespace Gerador_De_Atualização
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new lForm());
        }
    }
}
