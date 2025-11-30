using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalPOS
{
    public partial class frmCateogory : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        frmCateogoryList flist;
        public frmCateogory(frmCateogoryList frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            flist = frm;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void Clear()
        {
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtCategory.Clear();
            txtCategory.Focus();
        }

    

      

     
        private void btnCancel_Click(object sender, EventArgs e)
        {
          
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to save this Category?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("INSERT into tbl_category(category) VALUES(@category)", cn);
                    cm.Parameters.AddWithValue("@category", txtCategory.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Category has been added successfully.");
                    Clear();
                    flist.LoadCategory();

                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to update this category?", "Update Category", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("UPDATE tbl_category set category = @category where id like '" + lblID.Text + "'", cn);
                    cm.Parameters.AddWithValue("@category", txtCategory.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Category Updated Successfully");
                    flist.LoadCategory();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
            this.Dispose();
        }
    }
}
