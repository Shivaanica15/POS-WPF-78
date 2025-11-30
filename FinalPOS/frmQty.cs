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
    public partial class frmQty : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        private string pcode;
        private int qty;
        private double price;
        private string transno;
        frmPOS f;
        public frmQty(frmPOS frmpos)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.KeyPreview = true;
            f = frmpos;
        }

        public void ProductDetails(string pcode, double price, string transno, int qty)
        {
            this.pcode = pcode;
            this.price = price;
            this.transno = transno;
            this.qty = qty;
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        
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
            if ((e.KeyChar == 13) && (txtQty.Text != string.Empty))
            {
                string id="";
                int cart_qty=0;
                bool found = false;
                
                cn.Open();
                cm = new SqlCommand("select * from tbl_Cart  where transno =@transno and pcode = @pcode" ,cn);
                cm.Parameters.AddWithValue("@transno", f.lblTransno.Text);
                cm.Parameters.AddWithValue("@pcode", pcode);
                dr = cm.ExecuteReader();
                dr.Read();
                if(dr.HasRows)
                {
                    found = true;
                    id = dr["id"].ToString();
                    cart_qty = int.Parse(dr["qty"].ToString());
                }
                else
                {
                    found = false;
                }
                dr.Close();
                cn.Close();


                if(found == true)
                {
                   
                    cn.Open();
                    cm = new SqlCommand("update tbl_Cart set qty = (qty +" + int.Parse(txtQty.Text)+") where id = '"+id+"'   ", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();

                    f.Searchhp.Clear();
                    f.Searchhp.Focus();
                    f.LoadCart();
                    this.Dispose();
                }
                else
                {
                    if (qty < (int.Parse(txtQty.Text) + cart_qty))
                    {
                        MessageBox.Show("Unable to Add Remaining Quantity on hand is " + qty, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    cn.Open();
                    cm = new SqlCommand("insert  into tbl_Cart (transno, pcode , price , qty,  sdate, cashier) values (@transno, @pcode , @price , @qty,  @sdate, @cashier)", cn);
                    cm.Parameters.AddWithValue("transno", transno);
                    cm.Parameters.AddWithValue("pcode", pcode);
                    cm.Parameters.AddWithValue("price", price);
                    cm.Parameters.AddWithValue("qty", int.Parse(txtQty.Text));
                    cm.Parameters.AddWithValue("sdate", DateTime.Now);
                    cm.Parameters.AddWithValue("cashier", f.lblUser.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();

                    f.Searchhp.Clear();
                    f.Searchhp.Focus();
                    f.LoadCart();
                    this.Dispose();
                }

            }
        }

        private void frmQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }
    }
}
