using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FinalPOS
{
    public partial class frmLookUp : Form
    {
        frmPOS f;
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
        MySqlDataReader dr;
        string stitle = "MyNEW POS System";
        public frmLookUp(frmPOS frm)
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
            f = frm;

            this.KeyPreview = true;
        }


        public void LoadRecords()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT p.pcode, p.barcode, p.pdesc, b.brand, c.category, p.price, p.qty FROM tbl_products AS p INNER JOIN tbl_brand AS b ON b.id = p.bid INNER JOIN tbl_category AS c ON c.id = p.cid WHERE p.pdesc LIKE @search", cn);
            cm.Parameters.AddWithValue("@search", "%" + txtSearchp.Text + "%");
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
            }
            dr.Close();
            cn.Close();
        }




        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void txtSearchp_TextChanged(object sender, EventArgs e)
        {
            LoadRecords();
        }


        private void frmLookUp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            //if (colName == "Select")
            //{
            //    frmQty frm = new frmQty(f);
            //    frm.ProductDetails(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(), double.Parse(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString()), f.lblTransno.Text, int.Parse(dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString()));
            //    frm.ShowDialog();
            //}

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            frmQty frm = new frmQty(f);
            frm.ProductDetails(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(), double.Parse(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString()), f.lblTransno.Text, int.Parse(dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString()));
            frm.ShowDialog();
        }
    }
}
