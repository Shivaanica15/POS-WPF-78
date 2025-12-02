using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.IO;

namespace FinalPOS
{
    public partial class frmUserAccounts : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        MySqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        Form1 f;
        public frmUserAccounts(Form1 f)
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
                      


            this.f = f;
        }

        private void frmUserAccounts_Resize(object sender, EventArgs e)
        {
            metroTabControl1.Left = (this.Width - metroTabControl1.Width) / 2;
            metroTabControl1.Top = (this.Width - metroTabControl1.Width) / 2;

        }

        private void frmUserAccounts_Load(object sender, EventArgs e)
        {
            txtUser.Focus();
        }

        private void Clear()
        {
            txtName.Clear();
            txtPassword.Clear();
            txtRePassword.Clear();
            txtUser.Clear();
            cboRole.Text = "";
            txtUser.Focus();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text != txtRePassword.Text)
                {
                    MessageBox.Show("Password didn't matched", "Password Confirmation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (txtPassword.Text == "" && txtRePassword.Text == "")
                {
                    MessageBox.Show("Password is Required");
                    return;
                }
                cn.Open();
                cm = new MySqlCommand("INSERT INTO tbl_users (username, password, role, name) VALUES (@username, @password, @role, @name)", cn);
                cm.Parameters.AddWithValue("@username", txtUser.Text);
                cm.Parameters.AddWithValue("@password", txtPassword.Text);
                cm.Parameters.AddWithValue("@role", cboRole.Text);
                cm.Parameters.AddWithValue("@name", txtName.Text);
                cm.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("User Successfully Saved!");
                Clear();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtOldPassword.Text != f._pass)
                {
                    MessageBox.Show("OLD PASSWORD INCORRECT", "INVALID PASSWORD ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (txtNewPassword.Text != txtRetypeNew.Text)
                {
                    MessageBox.Show("CONFIRM PASSWORD IS INCORRECT", "INVALID PASSWORD ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }



                cn.Open();
                cm = new MySqlCommand("UPDATE tbl_users SET password = @password WHERE username = @username", cn);
                cm.Parameters.AddWithValue("@password", txtNewPassword.Text);
                cm.Parameters.AddWithValue("@username", txtUsername.Text);
                cm.ExecuteNonQuery();
                cn.Close();

                MessageBox.Show("Password has successfully changed", "SUCCESS ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtUser.Clear();
                txtRetypeNew.Clear();
                txtNewPassword.Clear();
                txtOldPassword.Clear();
            }
            catch (Exception ex)
            {
                // cn.Close();
                MessageBox.Show(ex.Message, "WARNING ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tbl_users WHERE username = @username", cn);
                cm.Parameters.AddWithValue("@username", txtUsername.Text);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    checkBox1.Checked = bool.Parse(dr["isactive"].ToString());
                }
                else
                {
                    checkBox1.Checked = false;
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "WARNING ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate username is entered
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Please enter a username", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool found = false;
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tbl_users WHERE username = @username", cn);
                cm.Parameters.AddWithValue("@username", textBox1.Text);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    found = true;
                }
                else
                {
                    found = false;
                }
                dr.Close();
                cn.Close();

                if (found == true)
                {
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tbl_users SET isactive = @isactive WHERE username = @username", cn);
                    cm.Parameters.AddWithValue("@isactive", checkBox1.Checked);
                    cm.Parameters.AddWithValue("@username", textBox1.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Account status has updated successfully", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox1.Clear();
                    checkBox1.Checked = false;
                }
                else
                {
                    MessageBox.Show("Account Not Exists", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select folder to save backup file";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtbackupbrowse.Text = dlg.SelectedPath;
                backupbutton.Enabled = true;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "SQL Files|*.sql|All Files|*.*";
            openFileDialog1.Title = "Select backup file to restore";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtrestorebrowse.Text = openFileDialog1.FileName;
                restorebutton.Enabled = true;
            }
        }

        private string FindMySqlDumpPath()
        {
            // Common MySQL installation paths
            string[] commonPaths = {
                @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.1\bin\mysqldump.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.2\bin\mysqldump.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.3\bin\mysqldump.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.4\bin\mysqldump.exe",
                @"C:\Program Files\MySQL\MySQL Server 5.7\bin\mysqldump.exe",
                @"C:\Program Files (x86)\MySQL\MySQL Server 8.0\bin\mysqldump.exe",
                @"C:\Program Files (x86)\MySQL\MySQL Server 5.7\bin\mysqldump.exe",
                @"C:\xampp\mysql\bin\mysqldump.exe",
                @"C:\wamp64\bin\mysql\mysql8.0.31\bin\mysqldump.exe",
                @"C:\wamp64\bin\mysql\mysql8.0.27\bin\mysqldump.exe",
                @"C:\wamp64\bin\mysql\mysql5.7.36\bin\mysqldump.exe"
            };

            // Check common paths
            foreach (string path in commonPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            // Check if mysqldump is in PATH
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "mysqldump",
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                    if (proc.ExitCode == 0)
                    {
                        return "mysqldump"; // Found in PATH
                    }
                }
            }
            catch
            {
                // mysqldump not in PATH
            }

            return null;
        }

        private string FindMySqlPath()
        {
            // Common MySQL installation paths
            string[] commonPaths = {
                @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.1\bin\mysql.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.2\bin\mysql.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.3\bin\mysql.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.4\bin\mysql.exe",
                @"C:\Program Files\MySQL\MySQL Server 5.7\bin\mysql.exe",
                @"C:\Program Files (x86)\MySQL\MySQL Server 8.0\bin\mysql.exe",
                @"C:\Program Files (x86)\MySQL\MySQL Server 5.7\bin\mysql.exe",
                @"C:\xampp\mysql\bin\mysql.exe",
                @"C:\wamp64\bin\mysql\mysql8.0.31\bin\mysql.exe",
                @"C:\wamp64\bin\mysql\mysql8.0.27\bin\mysql.exe",
                @"C:\wamp64\bin\mysql\mysql5.7.36\bin\mysql.exe"
            };

            // Check common paths
            foreach (string path in commonPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            // Check if mysql is in PATH
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "mysql",
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                    if (proc.ExitCode == 0)
                    {
                        return "mysql"; // Found in PATH
                    }
                }
            }
            catch
            {
                // mysql not in PATH
            }

            return null;
        }

        private void ParseConnectionString(out string server, out string database, out string username, out string password)
        {
            string connString = dbcon.MyConnection();
            server = "localhost";
            database = "newone";
            username = "root";
            password = "";

            // Parse connection string
            string[] parts = connString.Split(';');
            foreach (string part in parts)
            {
                if (part.Contains("Server="))
                {
                    server = part.Split('=')[1].Trim();
                }
                else if (part.Contains("Database="))
                {
                    database = part.Split('=')[1].Trim();
                }
                else if (part.Contains("Uid="))
                {
                    username = part.Split('=')[1].Trim();
                }
                else if (part.Contains("Pwd="))
                {
                    password = part.Split('=')[1].Trim();
                }
            }
        }

        private bool RunMySqlDump(string backupFilePath)
        {
            try
            {
                string mysqldumpPath = FindMySqlDumpPath();
                if (string.IsNullOrEmpty(mysqldumpPath))
                {
                    MessageBox.Show("MySQL dump utility (mysqldump.exe) not found.\n\nPlease ensure MySQL is installed and mysqldump.exe is accessible.\n\nYou can also add MySQL bin folder to your system PATH.", 
                        "MySQL Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                ParseConnectionString(out string server, out string database, out string username, out string password);

                // Build mysqldump command arguments
                StringBuilder args = new StringBuilder();
                args.Append($"-h{server} ");
                args.Append($"-u{username} ");
                
                if (!string.IsNullOrEmpty(password))
                {
                    args.Append($"-p{password} ");
                }
                else
                {
                    args.Append("-p ");
                }

                args.Append($"--databases {database} ");
                args.Append("--single-transaction ");
                args.Append("--routines ");
                args.Append("--triggers ");

                // Setup ProcessStartInfo with file redirection
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = mysqldumpPath,
                    Arguments = args.ToString().Trim(),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(mysqldumpPath)
                };

                Cursor.Current = Cursors.WaitCursor;
                this.Enabled = false;

                using (Process proc = Process.Start(psi))
                {
                    // Read output and write to file
                    string output = proc.StandardOutput.ReadToEnd();
                    string errorOutput = proc.StandardError.ReadToEnd();
                    
                    // Write output to backup file
                    File.WriteAllText(backupFilePath, output, Encoding.UTF8);
                    
                    proc.WaitForExit();

                    if (proc.ExitCode != 0)
                    {
                        MessageBox.Show($"Backup failed!\n\nError: {errorOutput}", 
                            "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (File.Exists(backupFilePath))
                        {
                            File.Delete(backupFilePath);
                        }
                        return false;
                    }

                    if (File.Exists(backupFilePath) && new FileInfo(backupFilePath).Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Backup file was not created or is empty.", 
                            "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during backup:\n\n{ex.Message}", 
                    "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                this.Enabled = true;
            }
        }

        private bool RunMySqlRestore(string restoreFilePath)
        {
            try
            {
                if (!File.Exists(restoreFilePath))
                {
                    MessageBox.Show("Backup file not found!", 
                        "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string mysqlPath = FindMySqlPath();
                if (string.IsNullOrEmpty(mysqlPath))
                {
                    MessageBox.Show("MySQL utility (mysql.exe) not found.\n\nPlease ensure MySQL is installed and mysql.exe is accessible.\n\nYou can also add MySQL bin folder to your system PATH.", 
                        "MySQL Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                ParseConnectionString(out string server, out string database, out string username, out string password);

                // Read SQL file content
                string sqlContent = File.ReadAllText(restoreFilePath, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(sqlContent))
                {
                    MessageBox.Show("Backup file is empty!", 
                        "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Build mysql command arguments
                StringBuilder args = new StringBuilder();
                args.Append($"-h{server} ");
                args.Append($"-u{username} ");
                
                if (!string.IsNullOrEmpty(password))
                {
                    args.Append($"-p{password} ");
                }
                else
                {
                    args.Append("-p ");
                }

                args.Append($"{database} ");

                // Setup ProcessStartInfo
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = mysqlPath,
                    Arguments = args.ToString().Trim(),
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(mysqlPath)
                };

                Cursor.Current = Cursors.WaitCursor;
                this.Enabled = false;

                using (Process proc = Process.Start(psi))
                {
                    // Write SQL content to process input
                    proc.StandardInput.Write(sqlContent);
                    proc.StandardInput.Close();
                    
                    string errorOutput = proc.StandardError.ReadToEnd();
                    string standardOutput = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();

                    if (proc.ExitCode != 0)
                    {
                        MessageBox.Show($"Restore failed!\n\nError: {errorOutput}\n\nOutput: {standardOutput}", 
                            "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during restore:\n\n{ex.Message}", 
                    "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                this.Enabled = true;
            }
        }

        private void backupbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // Show save file dialog
                DateTime PD = DateTime.Now;
                saveFileDialog1.FileName = PD.ToString("yyyy-MM-dd");
                saveFileDialog1.Filter = "SQL Files|*.sql|All Files|*.*";
                saveFileDialog1.DefaultExt = "sql";
                saveFileDialog1.Title = "Save Backup File";
                
                // Set initial directory if backup browse folder was selected
                if (!string.IsNullOrWhiteSpace(txtbackupbrowse.Text) && Directory.Exists(txtbackupbrowse.Text))
                {
                    saveFileDialog1.InitialDirectory = txtbackupbrowse.Text;
                }
                else
                {
                    saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string backupFilePath = saveFileDialog1.FileName;

                    // Ensure .sql extension
                    if (!backupFilePath.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                    {
                        backupFilePath += ".sql";
                    }

                    // Check if file exists
                    if (File.Exists(backupFilePath))
                    {
                        DialogResult result = MessageBox.Show("File already exists. Do you want to overwrite it?", 
                            "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                        {
                            return;
                        }
                        File.Delete(backupFilePath);
                    }

                    // Execute backup
                    if (RunMySqlDump(backupFilePath))
                    {
                        MessageBox.Show($"Backup completed successfully!\n\nFile saved to:\n{backupFilePath}", 
                            "Backup Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtbackupbrowse.Clear();
                        backupbutton.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void restorebutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtrestorebrowse.Text))
                {
                    MessageBox.Show("Please select a backup file to restore.", 
                        "No File Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(txtrestorebrowse.Text))
                {
                    MessageBox.Show("Backup file not found!", 
                        "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Warning confirmation
                DialogResult confirm = MessageBox.Show(
                    "WARNING: Restoring will replace ALL current data in the database!\n\n" +
                    "This action cannot be undone.\n\n" +
                    "Are you sure you want to continue?",
                    "Confirm Restore",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                // Execute restore
                if (RunMySqlRestore(txtrestorebrowse.Text))
                {
                    MessageBox.Show("Database restored successfully!\n\nPlease restart the application to see the changes.", 
                        "Restore Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtrestorebrowse.Clear();
                    restorebutton.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}      