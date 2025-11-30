using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FinalPOS
{
    public partial class frmSoldItems : Form
    {
      //  frmPOS fpos;
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;
        public string suser;
        public frmSoldItems()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            LoadRecords();
            LoadCashier();
            dt1.Value = DateTime.Now;
            dt2.Value = DateTime.Now;
         //   fpos = frm;
        }

     

        public void LoadRecords()
        {
            int i = 0;
            double total = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            if (cboCashier.Text == "All Cashiers")
            {
                cm = new SqlCommand("select c.id, c.transno, c.pcode , p.pdesc, c.price, c.qty, c.disc , c.total from tbl_Cart as c inner join tbl_Products as p on c.pcode = p.pcode where status like 'Sold' and sdate between '" + dt1.Value + "' and '" + dt2.Value + "' ", cn);
            }
            else
            {
                cm = new SqlCommand("select c.id, c.transno, c.pcode , p.pdesc, c.price, c.qty, c.disc , c.total from tbl_Cart as c inner join tbl_Products as p on c.pcode = p.pcode where status like 'Sold' and sdate between '" + dt1.Value + "' and '" + dt2.Value + "' and cashier like '" + cboCashier.Text + "' ", cn);
            }
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i += 1;
                total += double.Parse(dr["total"].ToString());
                dataGridView1.Rows.Add(i, dr["id"].ToString(), dr["transno"].ToString(), dr["pcode"].ToString(), dr["pdesc"].ToString(), dr["price"].ToString(), dr["qty"].ToString(), dr["disc"].ToString(), dr["total"].ToString());
            }
            dr.Close();
            cn.Close();
            lblDailyTotal.Text = total.ToString("#,##0.00");
        }

        private void dt1_ValueChanged(object sender, EventArgs e)
        {
            LoadRecords();
        }

        private void dt2_ValueChanged(object sender, EventArgs e)
        {
            LoadRecords();
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            frmSoldIReport frm = new frmSoldIReport(this);
            frm.LoadReport();
            frm.ShowDialog();
        }

        private void cboCashier_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void LoadCashier()
        {
            cboCashier.Items.Clear();
            cboCashier.Items.Add("All Cashiers");
            cn.Open();
            cm = new SqlCommand("select * from tbl_Users where role like 'Cashier'",cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboCashier.Items.Add(dr["username"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void cboCashier_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRecords();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if(colName == "colCancel")
            {
                frmCancelDetails f = new frmCancelDetails(this);
                f.txtID.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                f.txtTransno.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                f.txtpcode.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                f.txtdesc.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                f.txtPrice.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                f.txtQty.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
                f.txtDiscount.Text = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();
                f.txtTotal.Text = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();
                f.txtCancel.Text = suser;
                
                f.ShowDialog();
            }
        }
    }
}
