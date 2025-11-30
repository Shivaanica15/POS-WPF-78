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
    public partial class frmBrandList : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        public frmBrandList()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            LoadRecords();  
        }

 
        public void LoadRecords()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new SqlCommand("select * from tbl_Brand order by brand", cn);
            dr = cm.ExecuteReader();
            while(dr.Read())
            {
                i += 1;
                dataGridView1.Rows.Add(i, dr["id"].ToString(), dr["brand"].ToString());
            }
            cn.Close();
        }



        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            frmBrand frm = new frmBrand(this);
            frm.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                frmBrand frm = new frmBrand(this);
                frm.btnSave.Enabled = false;
                frm.lblID.Visible = false;
                frm.lblID.Text = dataGridView1[1, e.RowIndex].Value.ToString();
                frm.txtBrand.Text = dataGridView1[2, e.RowIndex].Value.ToString();
                frm.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure want to delete this brand?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("delete from tbl_Brand where id like '" + dataGridView1[1, e.RowIndex].Value.ToString() + "'", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Brand has been delete successfully.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecords();
                }
            }
        }
    }
}
