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
    public partial class frmChangePassword : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
        frmPOS f;
        public frmChangePassword(frmPOS frm)
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
            f = frm;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate empty fields
                if (string.IsNullOrWhiteSpace(txtOld.Text))
                {
                    MessageBox.Show("Please enter your old password", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtNew.Text))
                {
                    MessageBox.Show("Please enter a new password", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtConfNew.Text))
                {
                    MessageBox.Show("Please confirm your new password", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if new password and confirm password match
                if(txtNew.Text.Trim() != txtConfNew.Text.Trim())
                {
                    MessageBox.Show("New Password and Confirm Password Doesn't Matched!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verify old password by querying database directly (same way as login)
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tbl_users WHERE username = @username AND password = @password", cn);
                cm.Parameters.AddWithValue("@username", f.lblUser.Text);
                cm.Parameters.AddWithValue("@password", txtOld.Text.Trim());
                MySqlDataReader dr = cm.ExecuteReader();
                bool passwordMatch = dr.HasRows;
                dr.Close();
                cn.Close();

                if(!passwordMatch)
                {
                    MessageBox.Show("Old Password Doesn't Matched!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Old password is correct, proceed to change password
                if (MessageBox.Show("Change Password", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tbl_users SET password = @password WHERE username = @username", cn);
                    cm.Parameters.AddWithValue("@password", txtNew.Text.Trim());
                    cm.Parameters.AddWithValue("@username", f.lblUser.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Password Changed Successfully", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Dispose();
                }
            }
            catch(Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
