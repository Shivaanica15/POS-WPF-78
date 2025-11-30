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
    public partial class frmStore : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;
        public frmStore()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.KeyPreview = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadRecords()
        {
            cn.Open();
            cm = new SqlCommand("select * from tbl_Store ", cn);
            dr = cm.ExecuteReader();
            dr.Read();
            if(dr.Read())
            {
                txtStore.Text = dr["store"].ToString();
                txtAddress.Text = dr["address"].ToString();
            }
            else
            {
                txtStore.Clear();
                txtAddress.Clear();
            }
            dr.Close();
            cn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Save Store Details", "Store Details", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
                {
                    int count;
                    cn.Open();
                    cm = new SqlCommand("select count(*) from tbl_Store ", cn);
                    count = int.Parse(cm.ExecuteScalar().ToString());
                    cn.Close();
                    if(count > 0)
                    {
                        cn.Open();
                        cm = new SqlCommand("update tbl_Store set store =@store , address = @address ", cn);
                        cm.Parameters.AddWithValue("@store", txtStore.Text);
                        cm.Parameters.AddWithValue("@address", txtAddress.Text);
                        cm.ExecuteNonQuery();
                        cn.Close();
                    }else
                    {
                        cn.Open();
                        cm = new SqlCommand("insert  tbl_Store (store, address) values (@store, @address)  ", cn);
                        cm.Parameters.AddWithValue("@store", txtStore.Text);
                        cm.Parameters.AddWithValue("@address", txtAddress.Text);
                        cm.ExecuteNonQuery();
                        cn.Close();

                    }
                    MessageBox.Show("Store Details Saved Successfully", "SAVED RECORD ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtStore.Clear();
                    txtAddress.Clear();
                    txtStore.Focus();


                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "WARNING" ,MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void frmStore_Load(object sender, EventArgs e)
        {
            txtStore.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtStore.Clear();
            txtAddress.Clear();
            this.Dispose();

        }

        private void frmStore_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
