using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalPOS
{
    public partial class frmProduct : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;
        frmProductList flist;
        public frmProduct(frmProductList frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            flist = frm;
        }

       

        public void LoadCategory()
        {
            categorycbo.Items.Clear();
            cn.Open();
            cm = new SqlCommand("select category from tbl_category ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                categorycbo.Items.Add(dr[0].ToString());
            }
            dr.Close();
            cn.Close();

        }

        public void LoadBrand()
        {
            brandcbo.Items.Clear();
            cn.Open();
            cm = new SqlCommand("select brand from tbl_Brand ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                brandcbo.Items.Add(dr[0].ToString());
            }
            dr.Close();
            cn.Close();

        }

     
        public void Clear()
        {
            txtpcode.Clear();
            descriptionTxtBox.Clear();
            categorycbo.Text = "";
            brandcbo.Text = "";
            txtBarcode.Clear();
            pricetxtbox.Clear();
            txtpcode.Focus();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtReOrder.Clear();
        }



        private void pricetxtbox_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 46)
            {

                //accept. character
            }
            else if (e.KeyChar == 8)
            {
                //accept backspace
            }

            else if ((e.KeyChar < 48) || (e.KeyChar > 57))
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to update this product?", "Save Product", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string bid = ""; string cid = "";
                    cn.Open();
                    cm = new SqlCommand("Select id from tbl_Brand where brand like '" + brandcbo.Text + "'", cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        bid = dr[0].ToString();
                    }
                    dr.Close();
                    cn.Close();

                    cn.Open();
                    cm = new SqlCommand("Select id from tbl_category where category like '" + categorycbo.Text + "'", cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cid = dr[0].ToString();
                    }
                    dr.Close();
                    cn.Close();

                    cn.Open();
                    cm = new SqlCommand("UPDATE  tbl_Products SET barcode = @barcode , pdesc = @pdesc , bid = @bid, cid=@cid ,price= @price, reorder=@reorder where pcode like @pcode", cn);
                    cm.Parameters.AddWithValue("@pcode", txtpcode.Text);
                    cm.Parameters.AddWithValue("@barcode", txtBarcode.Text);
                    cm.Parameters.AddWithValue("@pdesc", descriptionTxtBox.Text);
                    cm.Parameters.AddWithValue("@bid", bid);
                    cm.Parameters.AddWithValue("@cid", cid);
                    cm.Parameters.AddWithValue("@price", Double.Parse(pricetxtbox.Text));
                    cm.Parameters.AddWithValue("@reorder",int.Parse(txtReOrder.Text));
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Product has been updated successfully.");
                    Clear();
                    flist.LoadRecords();
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
                if (MessageBox.Show("Are you sure want to save this product?", "Save Product", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string bid = ""; string cid = "";
                    cn.Open();
                    cm = new SqlCommand("Select id from tbl_Brand where brand like '" + brandcbo.Text + "'", cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        bid = dr[0].ToString();
                    }
                    dr.Close();
                    cn.Close();

                    cn.Open();
                    cm = new SqlCommand("Select id from tbl_category where category like '" + categorycbo.Text + "'", cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cid = dr[0].ToString();
                    }
                    dr.Close();
                    cn.Close();

                    cn.Open();
                    cm = new SqlCommand("INSERT INTO tbl_Products (pcode ,barcode, pdesc, bid, cid ,price, reorder) VALUES(@pcode, @barcode, @pdesc, @bid, @cid , @price, @reorder)", cn);
                    cm.Parameters.AddWithValue("@pcode", txtpcode.Text);
                    cm.Parameters.AddWithValue("@barcode", txtBarcode.Text);
                    cm.Parameters.AddWithValue("@pdesc", descriptionTxtBox.Text);
                    cm.Parameters.AddWithValue("@bid", bid);
                    cm.Parameters.AddWithValue("@cid", cid);
                    cm.Parameters.AddWithValue("@price", pricetxtbox.Text);
                    cm.Parameters.AddWithValue("@reorder", txtReOrder.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Product has been added successfully.");
                    Clear();
                    flist.LoadRecords();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void pricetxtbox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
