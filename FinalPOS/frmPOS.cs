using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace FinalPOS
{
    public partial class frmPOS : Form
    {
        string id;
        string price;
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        string stitle = "MyNEW POS System";
        frmSecurity f;
        int qty;


        public frmPOS(frmSecurity frm)
        {
            InitializeComponent();
            lblDate.Text = DateTime.Now.ToLongDateString();
            cn = new SqlConnection(dbcon.MyConnection());
            dataGridView1.Rows.Clear();
            this.KeyPreview = true;
            NotifyCriticalItems();
            LoadCart();
            f = frm;
         
        }

        public void NotifyCriticalItems()
        {
            string critical = "";
            cn.Open();
            cm = new SqlCommand("select count(*) from  ViewCriticalItems  ", cn);

            int i = 0;
            string count = cm.ExecuteScalar().ToString();
            cn.Close();

            cn.Open();
            cm = new SqlCommand("select * from  ViewCriticalItems  ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                critical += i + ".  " + dr["pdesc"].ToString() + Environment.NewLine;
            }
            dr.Close();
            cn.Close();

            PopupNotifier popup = new PopupNotifier();
            popup.Image = Properties.Resources.cancel__2_;
            popup.TitleText = count + "CRITICAL ITEM(S)";
            popup.ContentText = critical;
            popup.Popup();
        }

        public void GetTransNo()
        {
            try
            {
                string sdate = DateTime.Now.ToString("yyyyMMdd");
                string transno;
                int count;
                cn.Open();
                cm = new SqlCommand("select top 1 transno from tbl_Cart where transno like '" + sdate + "%' order by id desc  ", cn);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    transno = dr[0].ToString();
                    count = int.Parse(transno.Substring(8, 4));
                    lblTransno.Text = sdate + (count + 1);
                }
                else
                {
                    transno = sdate + "1001";
                    lblTransno.Text = transno;
                }
                dr.Close();
                cn.Close();
                Searchhp.Focus();

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

 


        private void btnNewTransaction_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count > 0)
            {
                return;
            }
            GetTransNo();
            Searchhp.Enabled = true;
            Searchhp.Focus();
     
        }


        private void AddToCart(String _pcode, double _price, int _qty)
        {
            string id = "";
            bool found = false;
            int cart_qty = 0;
            cn.Open();
            cm = new SqlCommand("select * from tbl_Cart  where transno =@transno and pcode = @pcode", cn);
            cm.Parameters.AddWithValue("@transno", lblTransno.Text);
            cm.Parameters.AddWithValue("@pcode", _pcode);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
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

            if (found == true)
            {
                if (qty < (int.Parse(txtQuantity.Text) + cart_qty))
                {
                    MessageBox.Show("Unable to Add Remaining Quantity on hand is " + qty, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                cn.Open();
                cm = new SqlCommand("update tbl_Cart set qty = (qty +" + _qty + ") where id = '" + id + "'   ", cn);
                cm.ExecuteNonQuery();
                cn.Close();

                Searchhp.SelectionStart = 0;
                Searchhp.SelectionLength = Searchhp.Text.Length;
                LoadCart();
              //  this.Dispose();
            }
            else
            {
                if (qty < (int.Parse(txtQuantity.Text) + cart_qty))
                {
                    MessageBox.Show("Unable to Add Remaining Quantity on hand is " + qty, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                cn.Open();
                cm = new SqlCommand("insert  into tbl_Cart (transno, pcode , price , qty, sdate, cashier) values (@transno, @pcode , @price , @qty , @sdate, @cashier)", cn);
                cm.Parameters.AddWithValue("transno", lblTransno.Text);
                cm.Parameters.AddWithValue("pcode", _pcode);
                cm.Parameters.AddWithValue("price", _price);
                cm.Parameters.AddWithValue("qty", _qty);
                cm.Parameters.AddWithValue("sdate", DateTime.Now);
                cm.Parameters.AddWithValue("cashier", lblUser.Text);
                cm.ExecuteNonQuery();
                cn.Close();

                Searchhp.SelectionStart = 0;
                Searchhp.SelectionLength = Searchhp.Text.Length;
                LoadCart();
              //  this.Dispose();
            }
        }


        public void LoadCart()
        {
            try
            {

                dataGridView1.Rows.Clear();
                Boolean hasrecords = false;
               
                
               
                int i = 0;
                double total = 0;
                double discount = 0;
                cn.Open();
                cm = new SqlCommand("select c.id, c.pcode, p.pdesc,c.price, c.qty, c.disc, c.total from tbl_Cart as c inner join tbl_Products as p on c.pcode = p.pcode where transno like '" + lblTransno.Text + "' and status like 'Pending' ", cn);
                dr = cm.ExecuteReader();
    
                    if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        total += Double.Parse(dr["total"].ToString());
                        discount += Double.Parse(dr["disc"].ToString());
                        dataGridView1.Rows.Add(i, dr["id"].ToString(), dr["pcode"].ToString(), dr["pdesc"].ToString(), dr["price"].ToString(), dr["qty"].ToString(), dr["disc"].ToString(),double.Parse(dr["total"].ToString()).ToString("#,##0.00"), "ADD 1" , "REMOVE 1");
                        hasrecords = true;
                        i++;
                    }

                    lblSalesTotal.Text = total.ToString("#,##0.00");
                    lblDiscount.Text = discount.ToString("#,##0.00");
                    dr.Close();
                    cn.Close();
                    
                    GetCartTotal();
                    if (hasrecords == true)
                    {
                        btnSettlePayment.Enabled = true;
                        btnDiscount.Enabled = true;
                        btnClearCart.Enabled = true;
                    }
                  
                }
                else
                {
                    dataGridView1.Rows.Clear();
                    btnSettlePayment.Enabled = false;
                    btnClearCart.Enabled = false;
                    btnDiscount.Enabled = false;
                    cn.Close();
                  
                }
        
            }
          
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cn.Close();
            }
            
        }

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            if (lblTransno.Text == "0000000000000000")
            {
                return;
            }
            frmLookUp frm = new frmLookUp(this);
            frm.LoadRecords();
            frm.ShowDialog();
        }


        private void bntClose_Click_1(object sender, EventArgs e)
        {
           if(dataGridView1.Rows.Count > 0)
            {
                MessageBox.Show("Unable to Logout Please Clear The Cart", "Warning" , MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            
            
            if(MessageBox.Show("Logout Application?","Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
            {
                this.Hide();
                frmSecurity frm = new frmSecurity();
                frm.ShowDialog();
            }
        }

        public void GetCartTotal()
        {


            double discount = Double.Parse(lblDiscount.Text);
            double sales = Double.Parse(lblSalesTotal.Text);
            double vat = sales * dbcon.GetVal();
            double vatable = sales - vat;

            lblVAT.Text = vat.ToString("#,##0.00");
            lblVatable.Text = vatable.ToString("#,##0.00");
            lblDisplayTotal.Text = sales.ToString("#,##0.00");
        }

        private void Searchhp_TextChanged_2(object sender, EventArgs e)
        {
            try
            {
                if (Searchhp.Text == string.Empty)
                {
                    return;
                }
                else
                {
                    string _pcode;
                    double _price;
                    int _qty;
                    cn.Open();
                    cm = new SqlCommand("select * from tbl_Products where barcode like  '" + Searchhp.Text + "' ", cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        qty = int.Parse(dr["qty"].ToString());
                        _pcode = dr["pcode"].ToString();
                        _price = double.Parse(dr["price"].ToString());
                        _qty = int.Parse(txtQuantity.Text);

                        dr.Close();
                        cn.Close();
                        AddToCart(_pcode, _price, _qty);
                    }
                    else
                    {
                        dr.Close();
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cn.Close();
            }
        }


        private void MyClear()
        {
            

                btnSettlePayment.Enabled = false;
                btnDiscount.Enabled = false;
                lblDisplayTotal.Text = "0.00";
                lblDiscount.Text = "0.00";
                lblVAT.Text = "0.00";
                lblVatable.Text = "0.00";
                lblSalesTotal.Text = "0.00";
                Searchhp.Clear();
               

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (MessageBox.Show("Remove this Item?", "Remove Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("delete from tbl_Cart where id like '" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() + "'  ", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Item Removed Successfully", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCart();
                   if(dataGridView1.Rows.Count < 1)
                    {
                        MyClear();
                    }

                }
            }
            else if(colName == "colAdd")
            {
                int i = 0;
                cn.Open();
                cm = new SqlCommand("select sum(qty)  as qty from tbl_Products where  pcode like '"+dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString()+"' group by pcode   ",cn);
                i = int.Parse(cm.ExecuteScalar().ToString());
                cn.Close();

                if(int.Parse(dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString()) < i)
                {
                    cn.Open();
                 
                    cm = new SqlCommand("update tbl_Cart set qty = qty + " + int.Parse(txtQuantity.Text) + " where transno like '" + lblTransno.Text + "' and pcode like '" + dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() + "'          ", cn);
                    
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadCart();
                }
                else
                {
                    MessageBox.Show("Remaing Quantity on Hand is " + i +  "!", "Out of Stock" ,MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (colName == "colRemove")
            {
                int i = 0;
                cn.Open();
                cm = new SqlCommand("select sum(qty)  as qty from tbl_Cart where  pcode like '" + dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() + "' and transno like '"+lblTransno.Text+"' group by transno, pcode   ", cn);
                i = int.Parse(cm.ExecuteScalar().ToString());
                cn.Close();

                if (i > 0)
                {
                    cn.Open();

                    cm = new SqlCommand("update tbl_Cart set qty = qty - " + int.Parse(txtQuantity.Text) + " where transno like '" + lblTransno.Text + "' and pcode like '" + dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() + "'          ", cn);

                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadCart();
                    if (dataGridView1.Rows.Count < 1)
                    {
                        MyClear();
                    }

                }
                else
                {
                    MessageBox.Show("Remaing Quantity on Cart is " + i + "!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

        } 

        private void btnDiscount_Click(object sender, EventArgs e)
        {
            frmDiscount frm = new frmDiscount(this);
            frm.lblID.Text = id;
            frm.txtPrice.Text = price;
            if (dataGridView1.Rows.Count < 1) {
                MessageBox.Show("NOTHING TO ADD DISCOUNT ON", "DISCOUNT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frm.ShowDialog();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int i = dataGridView1.CurrentRow.Index;
            id = dataGridView1[1, i].Value.ToString();
            price = dataGridView1[7, i].Value.ToString();
        }

   
        private void btnSettlePayment_Click(object sender, EventArgs e)
        {
            frmSettle frm = new frmSettle(this);
            frm.txtSale.Text = lblDisplayTotal.Text;
            frm.ShowDialog();
        }

        private void btnDailySales_Click(object sender, EventArgs e)
        {
            frmSoldItems frm = new frmSoldItems();
            frm.dt1.Enabled = false;
            frm.suser = lblUser.Text;
            frm.dt2.Enabled = false;
            frm.cboCashier.Enabled = false;
            frm.cboCashier.Text = lblUser.Text;
            frm.ShowDialog();
        }

        private void frmPOS_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F1)
            {
                btnNewTransaction_Click(sender, e);
            }
            
            if (e.KeyCode == Keys.F2)
            {
                btnSearchProduct_Click(sender, e);
            }

            if (e.KeyCode == Keys.F3)
            {
                               
                    btnDiscount_Click(sender, e);
                
               
            }

            if (e.KeyCode == Keys.F4)
            {
             
                    btnSettlePayment_Click(sender, e);
                
            }
            if (e.KeyCode == Keys.F5)
            {
                btnClearCart_Click(sender, e);
            }

            if (e.KeyCode == Keys.F6)
            {
                btnDailySales_Click(sender, e);
            }

            if (e.KeyCode == Keys.F7)
            {
                btnChangePassword_Click(sender, e);
            }
            if (e.KeyCode == Keys.F10)
            {
                Searchhp.SelectionStart = 0;
                Searchhp.SelectionLength = Searchhp.Text.Length;
                bntClose_Click_1(sender, e);
            }
        }


        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            frmChangePassword frm = new frmChangePassword(this);
            frm.ShowDialog();
        }

       

        private void Searchhp_TextChanged_1(object sender, EventArgs e)
        {
            
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count < 1)
            {
                MessageBox.Show("CART CLEARED ALREADY", "Cart Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

           else if(MessageBox.Show("Remove Everything from Cart?", "Clear Cart", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
            {
                cn.Open();
                cm = new SqlCommand("delete from tbl_Cart where transno like '"+lblTransno.Text+"'   ",cn);
                cm.ExecuteNonQuery();
                cn.Close();
                dataGridView1.Rows.Clear();
                MyClear();
                MessageBox.Show("Cart Cleared", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void cbocustomer_CheckedChanged(object sender, EventArgs e)
        {
            if(cbocustomer.Checked == true)
            {
                btnSettlePayment.Visible = false;
                btnCustomer.Visible = true;
            }
            else
            {
                return;
            }
        }
    }
}
