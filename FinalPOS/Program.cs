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
                // Initialize database schema
                // This will automatically prompt for password if needed
                DatabaseInitializer.EnsureDatabaseSetup();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed: {ex.Message}\n\nPlease ensure MySQL Server is running on port 3306.", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmSecurity());
        }
    }
}
