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
    public partial class frmBrand : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        frmBrandList frmlist;
        public frmBrand(frmBrandList flist)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            frmlist = flist;
        }



        private void Clear()
        {
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtBrand.Clear();
            txtBrand.Focus();
        }

    
  

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
            

        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to update this brand?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("update tbl_Brand set brand = @brand where id like '" + lblID.Text + "'", cn);
                    cm.Parameters.AddWithValue("@brand", txtBrand.Text);
                    cm.ExecuteNonQuery();

                    cn.Close();

                    MessageBox.Show("Brand has been updated successfully.");
                    Clear();
                    frmlist.LoadRecords();

                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to save this brand?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("INSERT INTO tbl_Brand(Brand) VALUES (@brand)", cn);
                    cm.Parameters.AddWithValue("@brand", txtBrand.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Record Saved Successfully.");
                    Clear();
                    frmlist.LoadRecords();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
