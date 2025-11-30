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
    public partial class frmSecurity : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        public string _pass, _username = "";
        public bool _isactive = false;
        public frmSecurity()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
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
            string _role="", _name = "";
            try
            {
                bool found = false;
                cn.Open();
                cm = new SqlCommand("select * from tbl_Users where username = @username and password = @password", cn);
                cm.Parameters.AddWithValue("@username", txtUsername.Text);
                cm.Parameters.AddWithValue("@password", txtPassword.Text);
                dr = cm.ExecuteReader();
                dr.Read();
                if(dr.HasRows)
                {
                    found = true;
                    _username = dr["username"].ToString();
                    _role = dr["role"].ToString();
                    _name = dr["name"].ToString();
                    _pass = dr["password"].ToString();
                    _isactive = bool.Parse(dr["isactive"].ToString());
                }
                else
                {
                    found = false;
                }
                dr.Close();
                cn.Close();

                if(found == true)
                {

                    if(_isactive == false )
                    {
                        MessageBox.Show("Accound is inactive , Unable to Login", "Inactive Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (_role == "Cashier")
                    {
                        MessageBox.Show("Welcome " + _name + "!", "ACCESS GRANTED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtPassword.Clear();
                        txtUsername.Clear();
                        this.Hide();
                        frmPOS frm = new frmPOS(this);
                        frm.lblUser.Text = _username;
                        frm.lblName.Text = _name + " | " + _role;
                        frm.lblUser.Text = _username;
                        frm.ShowDialog();
                    }
                    else if (_role == "System Administrator")
                    {
                        MessageBox.Show("Welcome " + _name + "!", "ACCESS GRANTED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtPassword.Clear();
                        txtUsername.Clear();
                        this.Hide();
                        Form1 frm = new Form1();
                        frm.lblRole.Text = _name;
                        frm.lblRole.Text = _role;
                        frm._pass = _pass;
                        frm._username = _username;
                        frm.ShowDialog();


                    }
                    
                }
                else
                {

                }

                {
                    MessageBox.Show("Invalid Username or Password", "ACCESS DENIED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Clear();
                    txtUsername.Clear();
                    txtUsername.Focus();
                }
            }
            catch(Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            }
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
