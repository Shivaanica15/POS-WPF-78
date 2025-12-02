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
    public partial class frmSearchProduct_StocksIn : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
        MySqlDataReader dr;
        string stitle = "MyNEW POS System";
        frmStockIn slist;
        public frmSearchProduct_StocksIn(frmStockIn flist)
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
            slist = flist;
        }



        public void LoadProduct()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            try
            {
                cn.Open();
                string searchText = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchText))
                {
                    // Load all products if search is empty
                    cm = new MySqlCommand("SELECT pcode, pdesc, qty, price FROM tbl_products ORDER BY pdesc", cn);
                }
                else
                {
                    // Load filtered products based on search
                    cm = new MySqlCommand("SELECT pcode, pdesc, qty, price FROM tbl_products WHERE pdesc LIKE @search OR pcode LIKE @search ORDER BY pdesc", cn);
                    cm.Parameters.AddWithValue("@search", "%" + searchText + "%");
                }
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("Error loading products: " + ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




       
        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click is on a valid cell
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dataGridView1.Columns[e.ColumnIndex].Name;

            if (colName == "Select")
            {
                if (slist.txtrefno.Text == string.Empty)
                {
                    MessageBox.Show("Please Enter Reference No", stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    slist.txtrefno.Focus();
                    return;
                }
                if (slist.txtstockinby.Text == string.Empty)
                {
                    MessageBox.Show("Please Enter Stock In by Person", stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    slist.txtstockinby.Focus();
                    return;
                }
                if (slist.lblVendorID.Text == string.Empty || slist.lblVendorID.Text == "0" || slist.lblVendorID.Text == "Vendor ID")
                {
                    MessageBox.Show("Please Select a Vendor", stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                if (MessageBox.Show("Add this Item?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        cn.Open();
                        cm = new MySqlCommand("INSERT INTO tbl_stocks_in (refno, pcode, sdate, stockinby, vendorid) VALUES (@refno, @pcode, @sdate, @stockinby, @vendorid)", cn);
                        cm.Parameters.AddWithValue("@refno", slist.txtrefno.Text);
                        cm.Parameters.AddWithValue("@pcode", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                        cm.Parameters.AddWithValue("@sdate", slist.dttime.Value);
                        cm.Parameters.AddWithValue("@stockinby", slist.txtstockinby.Text);
                        cm.Parameters.AddWithValue("@vendorid", slist.lblVendorID.Text);
                        cm.ExecuteNonQuery();
                        cn.Close();

                        MessageBox.Show("Successfully Added", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        slist.LoadStocksIn();
                        // Close the form after successful addition
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        cn.Close();
                        MessageBox.Show("Error adding product: " + ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow all characters for text search - no restrictions needed
            // The TextChanged event will handle the search functionality
        }


        private void txtSearch_TextChanged_1(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
