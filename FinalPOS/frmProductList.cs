using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace FinalPOS
{
    public partial class frmProductList : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
        MySqlDataReader dr;
        string stitle = "MyNEW POS System";
        public frmProductList()
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
            
        }

       
        public void LoadRecords()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            
            cn.Open();
            cm = new MySqlCommand("SELECT p.pcode, p.barcode, p.pdesc, b.brand, c.category, p.price, p.reorder FROM tbl_products AS p INNER JOIN tbl_brand AS b ON b.id = p.bid INNER JOIN tbl_category AS c ON c.id = p.cid WHERE p.barcode LIKE @search OR p.pdesc LIKE @search", cn);
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

     
        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            frmProduct frm = new frmProduct(this);
            frm.btnSave.Enabled = true;
            frm.btnUpdate.Enabled = false;
            frm.LoadBrand();
            frm.LoadCategory();
            
            frm.ShowDialog();
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void txtSearchp_TextChanged_1(object sender, EventArgs e)
        {
            LoadRecords();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                frmProduct frm = new frmProduct(this);
                frm.LoadCategory();
                frm.LoadBrand();
                frm.btnSave.Enabled = false;
                frm.btnUpdate.Enabled = true;
                frm.txtpcode.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                frm.txtBarcode.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                frm.descriptionTxtBox.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                frm.brandcbo.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                frm.categorycbo.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                frm.pricetxtbox.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
                frm.txtReOrder.Text = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();
                frm.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure want to delete this product?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tbl_products WHERE pcode = @pcode", cn);
                    cm.Parameters.AddWithValue("@pcode", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecords();
                }
            }
        }

        private void frmProductList_Load(object sender, EventArgs e)
        {
            LoadRecords();
        }
    }
}
