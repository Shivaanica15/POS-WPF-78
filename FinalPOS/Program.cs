using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalPOS
{
    static class Program
    {

        [STAThread]
        static void Main()
        {
            try
            {
                DatabaseInitializer.EnsureDatabaseSetup();
            }
            catch
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmSecurity());
        }
    }
}
