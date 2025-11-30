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
    public partial class frmUserAccounts : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        Form1 f;
        public frmUserAccounts(Form1 f)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
                      

            //  SqlConnection con = new SqlConnection(FinalPOS.Properties.Settings.Default.@"Data Source=DESKTOP-7UQI14K;Initial Catalog=NewOne;Integrated Security=True");

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
                cm = new SqlCommand(" insert into tbl_Users (username, password, role, name) values (@username, @password, @role, @name) ", cn);
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
                cm = new SqlCommand("update tbl_Users set password = @password where username =@username", cn);
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
                cm = new SqlCommand("select * from tbl_Users where username =@username", cn);
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
                bool found = true;
                cn.Open();
                cm = new SqlCommand("select * from tbl_Users where username =@username", cn);
                cm.Parameters.AddWithValue("@username", txtUsername.Text);
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
                cn.Close();



                if (found == true)
                {
                    cn.Open();
                    cm = new SqlCommand("update tbl_Users set isactive = @isactive where username = @username ", cn);
                    cm.Parameters.AddWithValue("@isactive", checkBox1.Checked.ToString());
                    cm.Parameters.AddWithValue("@username", txtUsername.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Account status has updated successfully ", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUsername.Clear();
                    checkBox1.Checked = false;
                }

                else
                {
                    MessageBox.Show("Account Not Exists ", "WARNING ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "WARNING ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtbackupbrowse.Text = dlg.SelectedPath;
                backupbutton.Enabled = true;
            }
        }

        private void backupbutton_Click(object sender, EventArgs e)
        {

            DateTime PD = DateTime.Now;
            saveFileDialog1.FileName = PD.ToString("yyyy-MM-dd");
            saveFileDialog1.Filter = " files|*.bak;*.BAK";
            saveFileDialog1.InitialDirectory = "d:";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = string.Empty;
                    path = saveFileDialog1.FileName;

                    path = path.Replace("\\", "//");
                    bool bBackUpStatus = true;
                    Cursor.Current = Cursors.WaitCursor;
                    if (System.IO.File.Exists(path))
                    {
                        if (MessageBox.Show(@"do you want a new one?", "the file is existing", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.IO.File.Delete(path);
                        }
                        else
                            bBackUpStatus = false;
                    }
                    if (bBackUpStatus)
                    {
                        //Connect to DB
                        SqlConnection con = new SqlConnection("Data Source=DESKTOP-7UQI14K;Initial Catalog=NewOne;Integrated Security=True");
                        //string con = @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirector  y|\SRVCARD.mdf;Integrated Security=True;User Instance=True";
                        if (con.State.ToString() == "Open")
                        {
                            con.Close();
                        }
                        con.Open();
                        SqlCommand command = new SqlCommand(@"backup database Exchange To Disk='" + path + "' with stats=10", con);
                        command.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Backup successfully", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //MessageBox.Show(ex.ToString());
                }
            }
        }
    }

}      