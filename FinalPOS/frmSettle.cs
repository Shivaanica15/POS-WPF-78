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
    public partial class frmSettle : Form
    {
        frmPOS fpos;
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;
        public frmSettle(frmPOS fp)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            fpos = fp;
            this.KeyPreview = true;
            txtCash.Focus();
        }

        private void txtCash_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double sale = double.Parse(txtSale.Text);
                double cash = double.Parse(txtCash.Text);
                double change = cash - sale;
                txtChange.Text = change.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                txtChange.Text = "0.00";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            txtCash.Text += button7.Text;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            txtCash.Text += button8.Text;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            txtCash.Text += button9.Text;
        }

        private void button0_Click(object sender, EventArgs e)
        {
            txtCash.Text += button0.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            txtCash.Text += button4.Text;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            txtCash.Text += button5.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            txtCash.Text += button6.Text;
        }

        private void button00_Click(object sender, EventArgs e)
        {
            txtCash.Text += button00.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtCash.Text += button1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtCash.Text += button2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtCash.Text += button3.Text;
        }

        private void buttonC_Click(object sender, EventArgs e)
        {
            txtCash.Clear();
            txtCash.Focus();
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonEnter_Click_1(object sender, EventArgs e)
        {
            try
            {

                if ((double.Parse(txtChange.Text) < 0) || (txtCash.Text == String.Empty))
                {
                    MessageBox.Show("Insufficient Amount. Please Enter Correct Amount", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    for (int i = 0; i < fpos.dataGridView1.Rows.Count; i++)
                    {
                        cn.Open();


                        cm = new SqlCommand("update tbl_Products set qty = qty - " + fpos.dataGridView1.Rows[i].Cells[5].Value.ToString() + "  where pcode = '" + fpos.dataGridView1.Rows[i].Cells[2].Value.ToString() + "' ", cn);
                        cm.ExecuteNonQuery();
                        cn.Close();


                        cn.Open();
                        cm = new SqlCommand("update tbl_Cart set status = 'Sold' where id = '" + fpos.dataGridView1.Rows[i].Cells[1].Value.ToString() + "'  ", cn);
                        cm.ExecuteNonQuery();
                        cn.Close();

                    }
                    fpos.LoadCart();
                    FrmReciept frm = new FrmReciept(fpos);
                    frm.LoadReport(txtCash.Text, txtChange.Text);
                    frm.ShowDialog();

                    MessageBox.Show("Payment Saved Successfully.", "Payment", MessageBoxButtons.OK, MessageBoxIcon.Information);               
                    fpos.LoadCart();
                    this.Dispose();

                }
                if (fpos.dataGridView1.Rows.Count < 1)
                {

                    fpos.lblDisplayTotal.Text = "0.00";
                    fpos.lblDiscount.Text = "0.00";
                    fpos.lblVAT.Text = "0.00";
                    fpos.lblVatable.Text = "0.00";
                    fpos.lblSalesTotal.Text = "0.00";
                    fpos.dataGridView1.Rows.Clear();
                    fpos.btnSettlePayment.Enabled = false;
                    fpos.btnDiscount.Enabled = false;
                    fpos.GetTransNo();

                }
                
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // MessageBox.Show("Some Products are not in Stocks", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            
        }

        private void txtCash_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                 if (e.KeyChar == 8)
                {
                    //accept backspace
                }

                else if ((e.KeyChar < 48) || (e.KeyChar > 57))
                {
                    e.Handled = true;
                }
            }
        }

        private void frmSettle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
            else if(e.KeyCode == Keys.Enter)
            {
                buttonEnter_Click_1(sender , e);
            }
        }
    }
}


