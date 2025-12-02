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
    public partial class frmDiscount : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
       
        frmPOS f;
        string stitle = "MyNEW POS System";
        public frmDiscount(frmPOS frm)
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
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
                // Calculate discount: price * (percentage / 100)
                // Example: 4500 * (10 / 100) = 4500 * 0.10 = 450.00
                double price = Double.Parse(txtPrice.Text);
                double percentage = Double.Parse(txtDisocunt.Text);
                double discount = price * (percentage / 100.0);
                txtDiscountAmount.Text = discount.ToString("#,##0.00");
            }
            catch (Exception)
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
                    cm = new MySqlCommand("UPDATE tbl_cart SET disc = @disc, disc_per = @disc_per WHERE id = @id", cn);
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
