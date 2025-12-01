using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalPOS
{
    public partial class frmCateogory : Form
    {

        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
        frmCateogoryList flist;
        public frmCateogory(frmCateogoryList frm)
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
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
                    cm = new MySqlCommand("INSERT INTO tbl_category (category) VALUES (@category)", cn);
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
                    cm = new MySqlCommand("UPDATE tbl_category SET category = @category WHERE id = @id", cn);
                    cm.Parameters.AddWithValue("@id", lblID.Text);
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
