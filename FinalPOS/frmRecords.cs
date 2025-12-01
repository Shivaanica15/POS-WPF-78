using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FinalPOS
{
    public partial class frmRecords : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        MySqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        public frmRecords()
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadRecords()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            
            if(cboTopSelect.Text == "SORT BY TOTAL AMOUNT")
            {
                cm = new MySqlCommand("SELECT pcode, pdesc, IFNULL(SUM(qty), 0) AS qty, IFNULL(SUM(total), 0) AS total FROM viewsolditems WHERE DATE(sdate) BETWEEN @date1 AND @date2 AND status = 'Sold' GROUP BY pcode, pdesc ORDER BY qty DESC LIMIT 10", cn);
                cm.Parameters.AddWithValue("@date1", dt1.Value.Date);
                cm.Parameters.AddWithValue("@date2", dt2.Value.Date);
            }
            else if (cboTopSelect.Text == "SORT BY QTY")
            {
                cm = new MySqlCommand("SELECT pcode, pdesc, IFNULL(SUM(qty), 0) AS qty, IFNULL(SUM(total), 0) AS total FROM viewsolditems WHERE DATE(sdate) BETWEEN @date1 AND @date2 AND status = 'Sold' GROUP BY pcode, pdesc ORDER BY total DESC LIMIT 10", cn);
                cm.Parameters.AddWithValue("@date1", dt1.Value.Date);
                cm.Parameters.AddWithValue("@date2", dt2.Value.Date);
            }

            if (!String.IsNullOrEmpty(cm.CommandText)) {
                cn.Open();
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(i, dr["pcode"].ToString(), dr["pdesc"].ToString(), dr["qty"].ToString(), double.Parse(dr["total"].ToString()).ToString("#,##0.00"));
                }
                dr.Close();
                cn.Close();
            }

        }

        public void CancelledOrders()
        {
            int i = 0;
            dataGridView5.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM cancelledorder WHERE DATE(sdate) BETWEEN @date1 AND @date2", cn);
            cm.Parameters.AddWithValue("@date1", dateTimePicker1.Value.Date);
            cm.Parameters.AddWithValue("@date2", dateTimePicker2.Value.Date);
            dr = cm.ExecuteReader();
            while(dr.Read())
            {
                i++;
                dataGridView5.Rows.Add(i, dr["transno"].ToString(), dr["pcode"].ToString(), dr["pdesc"].ToString(), dr["price"].ToString(), dr["qty"].ToString(), dr["total"].ToString(), dr["sdate"].ToString(), dr["voidby"].ToString(), dr["cancelledby"].ToString(), dr["reason"].ToString(), dr["action"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        public void LoadInventory()
        {
            int i = 0;
            dataGridView4.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT p.pcode, p.barcode, p.pdesc, b.brand, c.category, p.price, p.qty, p.reorder FROM tbl_products AS p INNER JOIN tbl_brand AS b ON p.bid = b.id INNER JOIN tbl_category AS c ON p.cid = c.id", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView4.Rows.Add(i, dr["pcode"].ToString(), dr["barcode"].ToString(), dr["pdesc"].ToString(), dr["brand"].ToString(), dr["category"].ToString(), dr["price"].ToString(), dr["reorder"].ToString(), dr["qty"].ToString());
            }
            cm.CommandText = "";
            dr.Close();
            cn.Close();
        }
        public void LoadCriticalItems()
        {
            try
            {
                dataGridView3.Rows.Clear();
                int i = 0;
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM viewcriticalitems", cn);
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dataGridView3.Rows.Add(i,dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString());
                }
                dr.Close();
                cn.Close();
            }
            catch(Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "Warning" ,MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmInventoryReport frm = new frmInventoryReport();
            frm.LoadReport();
            frm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        public void LoadStockInHistory()
        {
            int i = 0;
            dataGridView6.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM viewstocks WHERE DATE(sdate) BETWEEN @date1 AND @date2 AND status = 'Done'", cn);
            cm.Parameters.AddWithValue("@date1", dateTimePicker4.Value.Date);
            cm.Parameters.AddWithValue("@date2", dateTimePicker3.Value.Date);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView6.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), DateTime.Parse(dr[5].ToString()).ToShortDateString(), dr[6].ToString());
            }
            dr.Close();
            cn.Close();

        }

       private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmInventoryReport f = new frmInventoryReport();
            if (cboTopSelect.Text == "SORT BY QTY")
            {
                f.LoadTopSelling("SELECT pcode, pdesc, SUM(qty) AS qty FROM viewsolditems WHERE DATE(sdate) BETWEEN @date1 AND @date2 AND status = 'Sold' GROUP BY pcode, pdesc ORDER BY qty DESC LIMIT 10", "From : " + dt1.Value.ToString() + "To : " + dt2.Value.ToString() , "SORT BY QUANTITY");
            }
            else if (cboTopSelect.Text == "SORT BY TOTAL AMOUNT")
            {
           
                f.LoadTopSelling("SELECT pcode, pdesc, IFNULL(SUM(qty), 0) AS qty, IFNULL(SUM(total), 0) AS total FROM viewsolditems WHERE DATE(sdate) BETWEEN @date1 AND @date2 AND status = 'Sold' GROUP BY pcode, pdesc ORDER BY total DESC LIMIT 10", "From: " + dt1.Value.ToString() + "To: " + dt2.Value.ToString() , "SORT BY TOTAL SALES" );
            }


            
            f.ShowDialog();
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmInventoryReport f = new frmInventoryReport();
            f.LoadSoldItems("SELECT c.pcode, p.pdesc, c.price, SUM(c.qty) AS tot_qty, SUM(c.disc) AS tot_disc, SUM(c.total) AS total FROM tbl_cart AS c INNER JOIN tbl_products AS p ON c.pcode = p.pcode WHERE status = 'Sold' AND DATE(sdate) BETWEEN @date1 AND @date2 GROUP BY c.pcode, p.pdesc, c.price", "From : " + date1.Value.ToString() + "To : " + date2.Value.ToString());
            f.ShowDialog();
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(cboTopSelect.Text == string.Empty)
            {
                MessageBox.Show("Please Select Sorting", "WARNING ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            LoadRecords();
            LoadChartTopSelling();
        }

        public void LoadChartTopSelling()
          {

        
            MySqlDataAdapter da;
            cn.Open();
            if (cboTopSelect.Text == "SORT BY QTY")
            {
                cm = new MySqlCommand("SELECT pcode, IFNULL(SUM(qty), 0) AS qty FROM viewsolditems WHERE DATE(sdate) BETWEEN @date1 AND @date2 AND status = 'Sold' GROUP BY pcode ORDER BY qty DESC LIMIT 10", cn);
                cm.Parameters.AddWithValue("@date1", dt1.Value.Date);
                cm.Parameters.AddWithValue("@date2", dt2.Value.Date);
                da = new MySqlDataAdapter(cm);
            }
            else if (cboTopSelect.Text == "SORT BY TOTAL AMOUNT")
            {
                cm = new MySqlCommand("SELECT pcode, IFNULL(SUM(total), 0) AS total FROM viewsolditems WHERE DATE(sdate) BETWEEN @date1 AND @date2 AND status = 'Sold' GROUP BY pcode ORDER BY total DESC LIMIT 10", cn);
                cm.Parameters.AddWithValue("@date1", dt1.Value.Date);
                cm.Parameters.AddWithValue("@date2", dt2.Value.Date);
                da = new MySqlDataAdapter(cm);
            }
            else
            {
                cn.Close();
                return;
            }
            DataSet ds = new DataSet();
            da.Fill(ds, "TOPSELLING");
            chart1.DataSource = ds.Tables["TOPSELLING"];
            Series series = chart1.Series[0];
            series.ChartType = SeriesChartType.Doughnut;

            series.Name = "TOP SELLING";
            var chart = chart1;
            chart.Series[0].XValueMember = "pcode";
            chart1.Series[0]["PieLabelStyle"] = "Outside";
            chart1.Series[0].BorderColor = System.Drawing.Color.Gray;
            if (cboTopSelect.Text == "SORT BY QTY") { chart.Series[0].YValueMembers = "qty"; }
            if (cboTopSelect.Text == "SORT BY TOTAL AMOUNT") { chart.Series[0].YValueMembers = "total"; }
            chart.Series[0].IsValueShownAsLabel = true;
            if (cboTopSelect.Text == "SORT BY TOTAL AMOUNT") { chart.Series[0].LabelFormat = ("#,##0.00"); }
            if (cboTopSelect.Text == "SORT BY QTY") { chart.Series[0].LabelFormat = ("#,##0"); }
            cn.Close();
        }


        private void cboTopSelect_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                dataGridView2.Rows.Clear();
                int i = 0;
                cn.Open();
                cm = new MySqlCommand("SELECT c.pcode, p.pdesc, c.price, SUM(c.qty) AS tot_qty, SUM(c.disc) AS tot_disc, SUM(c.total) AS total FROM tbl_cart AS c INNER JOIN tbl_products AS p ON c.pcode = p.pcode WHERE status = 'Sold' AND DATE(sdate) BETWEEN @date1 AND @date2 GROUP BY c.pcode, p.pdesc, c.price", cn);
                cm.Parameters.AddWithValue("@date1", date1.Value.Date);
                cm.Parameters.AddWithValue("@date2", date2.Value.Date);
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dataGridView2.Rows.Add(i, dr["pcode"].ToString(), dr["pdesc"].ToString(), Double.Parse(dr["price"].ToString()).ToString("#,##0.00"), dr["tot_qty"].ToString(), dr["tot_disc"].ToString(), Double.Parse(dr["total"].ToString()));
                }
                dr.Close();
                cn.Close();


                cn.Open();
                cm = new MySqlCommand("SELECT IFNULL(SUM(total), 0) AS total FROM tbl_cart WHERE status = 'Sold' AND DATE(sdate) BETWEEN @date1 AND @date2", cn);
                cm.Parameters.AddWithValue("@date1", date1.Value.Date);
                cm.Parameters.AddWithValue("@date2", date2.Value.Date);
                lblTotal.Text = Double.Parse(cm.ExecuteScalar().ToString()).ToString("#,##0.00");
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmChart f = new frmChart();
            f.lblTitle.Text = "SOLD ITEMS [" +date1.Value.ToShortDateString() + " - " +date2.Value.ToShortDateString()+ "]";
            f.LoadChartSold("select  p.pdesc, sum(c.total) as total from tbl_Cart as c inner join tbl_Products as p on c.pcode = p.pcode where status like 'Sold' and sdate between '" + date1.Value.ToString() + "' and '" + date2.Value.ToString() + "' group by p.pdesc order by total desc");
            f.LoadChartSold("select  p.pdesc, sum(c.total) as total from tbl_Cart as c inner join tbl_Products as p on c.pcode = p.pcode where status like 'Sold' and sdate between '" + date1.Value.ToString() + "' and '" + date2.Value.ToString() + "' group by p.pdesc order by total desc");
            f.ShowDialog();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmInventoryReport frm = new frmInventoryReport();
            string param = "Date Covered : " + dateTimePicker4.Value.ToShortDateString() + " - " + dateTimePicker3.Value.ToShortDateString();
            frm.LoadStocksInReports("SELECT * FROM viewstocks WHERE DATE(sdate) BETWEEN @date1 AND @date2 AND status = 'Done'", param);
            frm.ShowDialog();
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadStockInHistory();
        }

        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CancelledOrders();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmInventoryReport f = new frmInventoryReport();
            string param = "Date Covered: " + dateTimePicker4.Value.ToShortDateString() + " - " + dateTimePicker3.Value.ToShortDateString();
            f.LoadCancelledOrders("SELECT * FROM cancelledorder WHERE DATE(sdate) BETWEEN @date1 AND @date2", param);
            f.ShowDialog();
        }
    }
}
