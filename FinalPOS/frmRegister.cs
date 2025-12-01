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
    public partial class frmRegister : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        MySqlDataReader dr;
        DBConnection dbcon = new DBConnection();

        public frmRegister()
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
            txtFullName.Focus();
            this.KeyPreview = true;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate empty fields
                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    MessageBox.Show("Please enter Full Name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFullName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show("Please enter Username", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please enter Password", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
                {
                    MessageBox.Show("Please confirm Password", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfirmPassword.Focus();
                    return;
                }

                if (cmbRole.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a Role", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbRole.Focus();
                    return;
                }

                // Validate password match
                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("Password and Confirm Password do not match", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfirmPassword.Clear();
                    txtConfirmPassword.Focus();
                    return;
                }

                // Check if username already exists
                cn.Open();
                cm = new MySqlCommand("SELECT COUNT(*) FROM tbl_users WHERE username = @username", cn);
                cm.Parameters.AddWithValue("@username", txtUsername.Text);
                int userCount = Convert.ToInt32(cm.ExecuteScalar());
                cn.Close();

                if (userCount > 0)
                {
                    MessageBox.Show("Username already taken", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Clear();
                    txtUsername.Focus();
                    return;
                }

                // Insert new user
                cn.Open();
                cm = new MySqlCommand("INSERT INTO tbl_users (name, username, password, role, isactive) VALUES (@name, @username, @password, @role, @isactive)", cn);
                cm.Parameters.AddWithValue("@name", txtFullName.Text);
                cm.Parameters.AddWithValue("@username", txtUsername.Text);
                cm.Parameters.AddWithValue("@password", txtPassword.Text);
                cm.Parameters.AddWithValue("@role", cmbRole.SelectedItem.ToString());
                cm.Parameters.AddWithValue("@isactive", true);
                cm.ExecuteNonQuery();
                cn.Close();

                MessageBox.Show("Account created successfully!", "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Clear fields
                txtFullName.Clear();
                txtUsername.Clear();
                txtPassword.Clear();
                txtConfirmPassword.Clear();
                cmbRole.SelectedIndex = -1;

                // Close register form and open login form
                this.Hide();
                frmSecurity frm = new frmSecurity();
                frm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBackToLogin_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmSecurity frm = new frmSecurity();
            frm.ShowDialog();
            this.Close();
        }

        private void frmRegister_Load(object sender, EventArgs e)
        {
            // Populate role combobox - only roles with dashboards
            cmbRole.Items.Add("System Administrator");
            cmbRole.Items.Add("Cashier");
        }

        private void frmRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnRegister_Click(sender, e);
            }
        }
    }
}

