using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace FinalPOS
{
   
public partial class frmStockIn : Form
{
    MySqlConnection cn = new MySqlConnection();
    MySqlCommand cm = new MySqlCommand();
    DBConnection dbcon = new DBConnection();
    MySqlDataReader dr;
    string stitle = "MyNEW POS System";
    public frmStockIn()
    {
        InitializeComponent();
        cn = new MySqlConnection(dbcon.MyConnection());
        LoadVendor();

    }



    public void LoadStocksIn()
    {
        int i=0;
        stgrids.Rows.Clear();
        cn.Open();
        cm = new MySqlCommand("SELECT * FROM viewstocks WHERE refno = @refno AND status = 'Pending'", cn);
        cm.Parameters.AddWithValue("@refno", txtrefno.Text);
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
            cm = new MySqlCommand("SELECT * FROM viewstocks WHERE DATE(sdate) BETWEEN @date1 AND @date2 AND status = 'Done'", cn);
            cm.Parameters.AddWithValue("@date1", date1.Value.Date);
            cm.Parameters.AddWithValue("@date2", date2.Value.Date);
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
                    cm = new MySqlCommand("DELETE FROM tbl_stocks_in WHERE id = @id", cn);
                    cm.Parameters.AddWithValue("@id", stgrids.Rows[e.RowIndex].Cells[1].Value.ToString());
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
                            cm = new MySqlCommand("UPDATE tbl_products SET qty = qty + @qty WHERE pcode = @pcode", cn);
                            cm.Parameters.AddWithValue("@qty", int.Parse(stgrids.Rows[i].Cells[5].Value.ToString()));
                            cm.Parameters.AddWithValue("@pcode", stgrids.Rows[i].Cells[3].Value.ToString());
                            cm.ExecuteNonQuery();
                            cn.Close();


                            //update tblstockin qty
                            cn.Open();
                            cm = new MySqlCommand("UPDATE tbl_stocks_in SET qty = qty + @qty, status = 'Done' WHERE id = @id", cn);
                            cm.Parameters.AddWithValue("@qty", int.Parse(stgrids.Rows[i].Cells[5].Value.ToString()));
                            cm.Parameters.AddWithValue("@id", stgrids.Rows[i].Cells[1].Value.ToString());
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
                    cm = new MySqlCommand("DELETE FROM tbl_stocks_in WHERE id = @id", cn);
                    cm.Parameters.AddWithValue("@id", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
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
            cm = new MySqlCommand("SELECT * FROM tbl_vendor", cn);
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
            cm = new MySqlCommand("SELECT * FROM tbl_vendor WHERE vendor = @vendor", cn);
            cm.Parameters.AddWithValue("@vendor", cboVendor.Text);
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
