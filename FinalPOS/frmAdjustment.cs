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
    public partial class frmAdjustment : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
        MySqlDataReader dr;
        Form1 f;
        int _qty;
        public frmAdjustment(Form1 f)
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
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
            cm = new MySqlCommand("SELECT p.pcode, p.barcode, p.pdesc, b.brand, c.category, p.price, p.qty FROM tbl_products AS p INNER JOIN tbl_brand AS b ON b.id = p.bid INNER JOIN tbl_category AS c ON c.id = p.cid WHERE p.pdesc LIKE @search", cn);
            cm.Parameters.AddWithValue("@search", "%" + txtSearchp.Text + "%");
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
                    SqlStatement("UPDATE tbl_products SET qty = (qty - @qty) WHERE pcode = @pcode", new MySqlParameter[] { new MySqlParameter("@qty", int.Parse(txtQty.Text)), new MySqlParameter("@pcode", txtPcode.Text) });
                }
                else if(cboCommand.Text == "ADD TO INVENTORY")
                {
                    SqlStatement("UPDATE tbl_products SET qty = (qty + @qty) WHERE pcode = @pcode", new MySqlParameter[] { new MySqlParameter("@qty", int.Parse(txtQty.Text)), new MySqlParameter("@pcode", txtPcode.Text) });
                }

                SqlStatement("INSERT INTO tbl_adjustment (referenceno, pcode, qty, action, remarks, sdate, user) VALUES (@refno, @pcode, @qty, @action, @remarks, @sdate, @user)", 
                    new MySqlParameter[] {
                        new MySqlParameter("@refno", txtRefNo.Text),
                        new MySqlParameter("@pcode", txtPcode.Text),
                        new MySqlParameter("@qty", int.Parse(txtQty.Text)),
                        new MySqlParameter("@action", cboCommand.Text),
                        new MySqlParameter("@remarks", txtRemarks.Text),
                        new MySqlParameter("@sdate", DateTime.Now),
                        new MySqlParameter("@user", txtUser.Text)
                    });

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

        public void SqlStatement(string _sql, MySqlParameter[] parameters = null)
        {
            cn.Open();
            cm = new MySqlCommand(_sql, cn);
            if (parameters != null)
            {
                cm.Parameters.AddRange(parameters);
            }
            cm.ExecuteNonQuery();
            cn.Close();
        }
    }
}
