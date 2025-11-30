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
    public partial class frmDiscount : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;
       
        frmPOS f;
        string stitle = "MyNEW POS System";
        public frmDiscount(frmPOS frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            f = frm;
            this.KeyPreview = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void txtDisocunt_TextChanged(object sender, EventArgs e)
        {
            try
            {

                double discount = Double.Parse(txtPrice.Text) * Double.Parse(txtDisocunt.Text);
                txtDiscountAmount.Text = discount.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                txtDiscountAmount.Text = "0.00";

            }
        }

     

        private void btnConfirm_Click_1(object sender, EventArgs e)
        {
            try
            { 
               
               if (MessageBox.Show("Add Discount? Click Yes To Confirm", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("update tbl_Cart set disc = @disc, disc_per = @disc_per where id = @id", cn);
                    cm.Parameters.AddWithValue("@disc", Double.Parse(txtDiscountAmount.Text));
                    cm.Parameters.AddWithValue("@disc_per", Double.Parse(txtDisocunt.Text));
                    cm.Parameters.AddWithValue("@id", int.Parse(lblID.Text));
                    cm.ExecuteNonQuery();
                    cn.Close();
                    f.LoadCart();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmDiscount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }
    }
}
