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
    public partial class frmAdjustment : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;
        Form1 f;
        int _qty;
        public frmAdjustment(Form1 f)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.f = f;
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        public void RefrenceNo()
        {
            Random rnd = new Random();
            txtRefNo.Text = rnd.Next().ToString();
        }




        public void LoadRecords()
        {
            dataGridView1.Rows.Clear();
            int i = 0;

            cn.Open();
            cm = new SqlCommand("Select p.pcode, p.barcode, p.pdesc, b.brand, c.category , p.price , p.qty from tbl_Products as p inner join tbl_Brand as b on b.id = p.bid inner join tbl_category as c on c.id = p.cid where p.pdesc like '%" + txtSearchp.Text + "%' ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if(colName == "Select")
            {
                txtPcode.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtDescription.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString() + " " + dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString() + " "+ dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                _qty = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Validation for Empty Fields
                if(int.Parse(txtQty.Text)> _qty)
                {
                    MessageBox.Show("STOCK QUANTITY SHOULD BE GREATER THAN ADJUSTMENT QUANTITY", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //update stock
                if(cboCommand.Text == "REMOVE FROM INVENTORY")
                {
                    SqlStatement ("update tbl_Products set qty = (qty -" + int.Parse(txtQty.Text) + ") where pcode like '"+txtPcode.Text+"' ");
                }
                else if(cboCommand.Text == "ADD TO INVENTORY")
                {
                    SqlStatement("update tbl_Products set qty = (qty +" + int.Parse(txtQty.Text) + ") where pcode like '" + txtPcode.Text + "' ");
                }

                SqlStatement("insert into tbl_Adjustment(referenceno, pcode, qty, action, remarks, sdate , [user]) values ('"+txtRefNo.Text+ "', '" + txtPcode.Text + "' , '" + int.Parse(txtQty.Text) + "' , '" + cboCommand.Text + "' ,'" + txtRemarks.Text + "' , '"    + DateTime.Now.ToShortDateString() + "' , '" + txtUser.Text + "')");

                MessageBox.Show("STOCK HAS BEEN ADJUSTED", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
               
                LoadRecords();
                Clear();
            }
            catch(Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void Clear()
        {
            txtDescription.Clear();
            txtPcode.Clear();
            txtQty.Clear();
            txtRefNo.Clear();
            txtRemarks.Clear();
            cboCommand.Text = "";
            RefrenceNo();
        }

        public void SqlStatement(string _sql)
        {
            cn.Open();
            cm = new SqlCommand(_sql, cn);
            cm.ExecuteNonQuery();
            cn.Close();
        }
    }
}
