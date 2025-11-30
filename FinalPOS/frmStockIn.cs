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
   
public partial class frmStockIn : Form
{
    SqlConnection cn = new SqlConnection();
    SqlCommand cm = new SqlCommand();
    DBConnection dbcon = new DBConnection();
    SqlDataReader dr;
    string stitle = "MyNEW POS System";
    public frmStockIn()
    {
        InitializeComponent();
        cn = new SqlConnection(dbcon.MyConnection());
        LoadVendor();

    }



    public void LoadStocksIn()
    {
        int i=0;
        stgrids.Rows.Clear();
        cn.Open();
        cm = new SqlCommand("select * from ViewStocks where refno like '"+txtrefno.Text+"' and status like 'Pending' ",cn);
        dr = cm.ExecuteReader();
        while(dr.Read())
        {
            i++;
            stgrids.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr["vendor"].ToString());
        }
        dr.Close();
        cn.Close();
            
    }
    public void Clear()
    {
        txtstockinby.Clear();
        txtrefno.Clear();
        cboVendor.Text = "";
            txtContactPerson.Clear();
        dttime.Value = DateTime.Now;
    }


   

        private void button1_Click(object sender, EventArgs e)
        {
            LoadStockInHistory();
        }

        private void LoadStockInHistory()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new SqlCommand("select * from ViewStocks where cast(sdate as date) between '" + date1.Value.ToShortDateString() + "' and '" + date2.Value.ToShortDateString() + "' and status like 'Done' ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), DateTime.Parse(dr[5].ToString()).ToShortDateString(), dr[6].ToString(), dr["vendor"].ToString());
            }
            dr.Close();
            cn.Close();
            
        }

        private void stgrids_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {
            string colName = stgrids.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (MessageBox.Show("Remove this item?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("delete from tbl_Stocks_In where id = '" + stgrids.Rows[e.RowIndex].Cells[1].Value.ToString() + "' ", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Item has been successfully removed", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStocksIn();
                }

            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (stgrids.Rows.Count > 0)
                {
                    if (MessageBox.Show("Are you sure to add this record?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {


                        for (int i = 0; i < stgrids.Rows.Count; i++)
                        {
                            //update product quantity   
                            cn.Open();
                            cm = new SqlCommand("update tbl_Products set qty = qty+ " + int.Parse(stgrids.Rows[i].Cells[5].Value.ToString()) + " where pcode like '" + stgrids.Rows[i].Cells[3].Value.ToString() + "'  ", cn);
                            cm.ExecuteNonQuery();
                            cn.Close();


                            //update tblstockin qty
                            cn.Open();
                            cm = new SqlCommand("update tbl_Stocks_In set qty = qty+ " + int.Parse(stgrids.Rows[i].Cells[5].Value.ToString()) + " , status = 'Done' where id like '" + stgrids.Rows[i].Cells[1].Value.ToString() + "'", cn);
                            cm.ExecuteNonQuery();
                            cn.Close();
                        }
                        Clear();
                        LoadStocksIn();
                    }
                }

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmSearchProduct_StocksIn frm = new frmSearchProduct_StocksIn(this);
            frm.LoadProduct();
            frm.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (MessageBox.Show("Remove this item?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("delete from ViewStocks where id = '" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() + "' ", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Item has been successfully removed", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStockInHistory();
                }

            }
        }
        
     
        private void button2_Click(object sender, EventArgs e)
        {
            Clear();
            txtrefno.Focus();
            stgrids.Rows.Clear();
            
           
        }

        public void LoadVendor()
        {
            cboVendor.Items.Clear();
            cn.Open();
            cm = new SqlCommand("select * from tbl_Vendor", cn);
            dr = cm.ExecuteReader();
            while(dr.Read())
            {
                cboVendor.Items.Add(dr["vendor"].ToString());
               
                 
            }
            cn.Close();
        }

        private void cboVendor_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cboVendor_TextChanged(object sender, EventArgs e)
        {
            cn.Open();
            cm = new SqlCommand("select * from tbl_Vendor where vendor like '" + cboVendor.Text + "'  ", cn);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                lblVendorID.Text = dr["id"].ToString();
                txtContactPerson.Text = dr["contactperson"].ToString();
            }
            dr.Close();
            cn.Close();
        }

        private void cboVendor_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Random rnd = new Random();
            txtrefno.Clear();
                txtrefno.Text += rnd.Next();
            
        }
    }

}
