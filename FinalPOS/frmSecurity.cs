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

namespace FinalPOS
{
    public partial class frmSecurity : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        MySqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        public string _pass, _username = "";
        public bool _isactive = false;
        public frmSecurity()
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
            txtUsername.Focus();
            this.KeyPreview = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtPassword.Clear();
            txtUsername.Clear();
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your username and password.", "Missing Credentials", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string role = string.Empty;
            string name = string.Empty;
            bool isActive = false;
            bool found = false;

            try
            {
                cn.Open();
                using (var command = new MySqlCommand("SELECT * FROM tbl_users WHERE username = @username AND password = @password LIMIT 1", cn))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            found = true;
                            _username = reader["username"].ToString();
                            _pass = reader["password"].ToString();
                            role = reader["role"].ToString();
                            name = reader["name"].ToString();
                            isActive = ConvertToBoolean(reader["is_active"]);
                            _isactive = isActive;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to process login. {ex.Message}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }

            if (!found)
            {
                ShowInvalidCredentials();
                return;
            }

            if (!isActive)
            {
                MessageBox.Show("Account disabled.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtUsername.SelectAll();
                txtUsername.Focus();
                return;
            }

            MessageBox.Show($"Welcome {name}!", "ACCESS GRANTED", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtPassword.Clear();
            txtUsername.Clear();
            if (role.Equals("Cashier", StringComparison.OrdinalIgnoreCase))
            {
                this.Hide();
                frmPOS frm = new frmPOS(this);
                frm.lblUser.Text = _username;
                frm.lblName.Text = name + " | " + role;
                frm.ShowDialog();
            }
            else
            {
                this.Hide();
                Form1 frm = new Form1();
                frm.lblRole.Text = role;
                frm._pass = _pass;
                frm._username = _username;
                frm.ShowDialog();
            }
        }

        private void ShowInvalidCredentials()
        {
            MessageBox.Show("Invalid username or password.", "ACCESS DENIED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPassword.Clear();
            txtUsername.Clear();
            txtUsername.Focus();
        }

        private bool ConvertToBoolean(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            string stringValue = value.ToString().Trim();
            if (string.IsNullOrEmpty(stringValue))
            {
                return false;
            }

            if (stringValue == "1")
            {
                return true;
            }

            if (stringValue == "0")
            {
                return false;
            }

            if (bool.TryParse(stringValue, out bool parsed))
            {
                return parsed;
            }

            if (int.TryParse(stringValue, out int numeric))
            {
                return numeric != 0;
            }

            return false;
        }

        private void frmSecurity_Load(object sender, EventArgs e)
        {

        }

        private void frmSecurity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}
