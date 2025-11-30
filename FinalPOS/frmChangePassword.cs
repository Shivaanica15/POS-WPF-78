using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace FinalPOS
{
    public partial class frmChangePassword : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        frmPOS f;
        public frmChangePassword(frmPOS frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
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
                string _oldpass = dbcon.GetPassword(f.lblUser.Text);
                if(_oldpass!=txtOld.Text)
                {
                    MessageBox.Show("Old Password Doesn't Matched!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if(txtNew != txtConfNew)
                {
                    MessageBox.Show("Password Doesn't Matched!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (MessageBox.Show("Change Password", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cn.Open();
                        cm = new SqlCommand(" update tbl_Users set password = @password  where username = @username",cn);
                        cm.Parameters.AddWithValue("@password", txtNew.Text);
                        cm.Parameters.AddWithValue("@username", f.lblUser.Text);
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Passsword Changed Successfully", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Dispose();
                    }
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
