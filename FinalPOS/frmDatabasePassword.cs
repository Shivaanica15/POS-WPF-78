using System;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FinalPOS
{
    public partial class frmDatabasePassword : Form
    {
        public string DatabasePassword { get; private set; } = "";
        public bool PasswordProvided { get; private set; } = false;

        public frmDatabasePassword()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter MySQL root password.", "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Test the connection with provided password
            // Use 127.0.0.1 instead of localhost to avoid DNS resolution issues
            string testConnectionString = $"Server=127.0.0.1;Port=3310;Uid=root;Pwd={txtPassword.Text.Trim()};AllowPublicKeyRetrieval=True;";
            
            try
            {
                using (var testConnection = new MySqlConnection(testConnectionString))
                {
                    testConnection.Open();
                    // Connection successful
                    DatabasePassword = txtPassword.Text.Trim();
                    PasswordProvided = true;
                    
                    // Save password to App.config
                    SavePasswordToConfig(DatabasePassword);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (MySqlException mysqlEx)
            {
                string errorMsg = "";
                if (mysqlEx.Number == 0 || mysqlEx.Message.Contains("Unable to connect") || mysqlEx.Message.Contains("specified MySQL hosts"))
                {
                    errorMsg = "Cannot connect to MySQL server on port 3310.\n\n";
                    errorMsg += "MySQL is not running or not accessible.\n\n";
                    errorMsg += "Please:\n";
                    errorMsg += "1. Start MySQL service\n";
                    errorMsg += "2. Verify MySQL is running on port 3310\n";
                    errorMsg += "3. Check MySQL configuration";
                }
                else if (mysqlEx.Number == 1045 || mysqlEx.Message.Contains("Access denied"))
                {
                    errorMsg = $"Authentication failed: {mysqlEx.Message}\n\n";
                    errorMsg += "Please check your password and try again.";
                }
                else
                {
                    errorMsg = $"Connection failed: {mysqlEx.Message}\n\nPlease check your password and try again.";
                }
                
                MessageBox.Show(errorMsg, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}\n\nPlease check your password and try again.", 
                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SavePasswordToConfig(string password)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                
                if (connectionStringsSection != null)
                {
                    // Update or add the connection string
                    // Use 127.0.0.1 instead of localhost to avoid DNS resolution issues
                    var connectionString = $"Server=127.0.0.1;Port=3310;Database=POS_NEXA_ERP;Uid=root;Pwd={password};AllowPublicKeyRetrieval=True;";
                    
                    if (connectionStringsSection.ConnectionStrings["FinalPOS.Properties.Settings.NewOneConnectionString"] != null)
                    {
                        connectionStringsSection.ConnectionStrings["FinalPOS.Properties.Settings.NewOneConnectionString"].ConnectionString = connectionString;
                    }
                    else
                    {
                        connectionStringsSection.ConnectionStrings.Add(
                            new ConnectionStringSettings("FinalPOS.Properties.Settings.NewOneConnectionString", connectionString, "MySql.Data.MySqlClient"));
                    }
                    
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                }
            }
            catch (Exception ex)
            {
                // If we can't save to config, we'll use it in memory for this session
                System.Diagnostics.Debug.WriteLine($"Could not save password to config: {ex.Message}");
            }
        }

        private void frmDatabasePassword_Load(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(sender, e);
            }
        }
    }
}

