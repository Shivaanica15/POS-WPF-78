using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalPOS
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Start MySQL if not running
            if (!IsMySQLRunning())
            {
                StartPortableMySQL();
            }

            // Wait a bit for MySQL to start if we just started it
            Thread.Sleep(2000);

            try
            {
                // Initialize database schema
                // This will automatically prompt for password if needed
                DatabaseInitializer.EnsureDatabaseSetup();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed: {ex.Message}\n\nPlease ensure MySQL Server is running on port 3310.", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Handle application exit to stop MySQL
            Application.ApplicationExit += Application_ApplicationExit;
            
            Application.Run(new frmSecurity());
        }

        private static bool IsMySQLRunning()
        {
            try
            {
                IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] tcpConnections = properties.GetActiveTcpListeners();
                return tcpConnections.Any(endpoint => endpoint.Port == 3310);
            }
            catch
            {
                return false;
            }
        }

        private static void StartPortableMySQL()
        {
            try
            {
                // Get application directory
                string appDir = Path.GetDirectoryName(Application.ExecutablePath);
                string startScript = Path.Combine(appDir, "scripts", "Start-MySQL.bat");

                // Check if script exists (portable installation)
                if (File.Exists(startScript))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = startScript,
                        Arguments = "\"" + appDir + "\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = Path.GetDirectoryName(startScript)
                    };

                    Process.Start(psi);
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail - MySQL might already be running or user can start manually
                System.Diagnostics.Debug.WriteLine($"Failed to start MySQL: {ex.Message}");
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                // Stop MySQL when application exits
                string appDir = Path.GetDirectoryName(Application.ExecutablePath);
                string stopScript = Path.Combine(appDir, "scripts", "Stop-MySQL.bat");

                if (File.Exists(stopScript))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = stopScript,
                        Arguments = "\"" + appDir + "\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = Path.GetDirectoryName(stopScript)
                    };

                    Process process = Process.Start(psi);
                    if (process != null)
                    {
                        process.WaitForExit(5000); // Wait up to 5 seconds
                    }
                }
            }
            catch
            {
                // Ignore errors when stopping MySQL on exit
            }
        }
    }
}

