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


    public partial class frmVendorList : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        public frmVendorList()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            frmVendor f = new frmVendor(this);
            f.btnSave.Enabled = true;
            f.btnUpdate.Enabled = false;
            f.ShowDialog();
        }

        public void LoadRecords()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            cn.Open();
            cm = new SqlCommand("select * from tbl_Vendor", cn);
            dr = cm.ExecuteReader();
            while(dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                frmVendor frm = new frmVendor(this);
                frm.lblID.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                frm.txtvendor.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                frm.txtaddress.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                frm.txtContactPerson.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                frm.txtTelephone.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                frm.txtEmail.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
                frm.btnSave.Enabled = false;
                frm.btnUpdate.Enabled = true;
                frm.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if(MessageBox.Show("YOU SURE WANT TO DELETE THE VENDOR?", "Delete Vendor", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("delete from tbl_Vendor where id like '"+dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()+"'   ",cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Vendor Deleted Successfully", "Delete Vendor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecords();
                }
            }
            
        }
    }
}
