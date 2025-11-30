using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalPOS
{
    public partial class frmCateogoryList : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;   
        public frmCateogoryList()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

      private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        public void LoadCategory()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new SqlCommand("SELECT * FROM tbl_category order by category", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString());
            }
            dr.Close();


            cn.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            frmCateogory frm = new frmCateogory(this);
            frm.btnSave.Enabled = true;
            frm.btnUpdate.Enabled = false;
            frm.ShowDialog();


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                frmCateogory frm = new frmCateogory(this);
                frm.txtCategory.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                frm.lblID.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                frm.btnSave.Enabled = false;
                frm.btnUpdate.Enabled = true;
                frm.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure want to delete this category?", "Delete Category", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("delete from tbl_category where id like '" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() + "'", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Category Deleted Successfully");
                    LoadCategory();
                }
            }
        }
    }
}
