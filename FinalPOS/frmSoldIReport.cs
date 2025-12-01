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
using Microsoft.Reporting.WinForms;

namespace FinalPOS
{
    public partial class frmSoldIReport : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
        frmSoldItems f;
        public frmSoldIReport(frmSoldItems frm)
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
            f = frm;
        }

        private void frmSoldIReport_Load(object sender, EventArgs e)
        {


            this.reportViewer2.RefreshReport();
        }

        

        public void LoadReport()
        {
            try
            {
                ReportDataSource rptDS;
                this.reportViewer2.LocalReport.ReportPath = Application.StartupPath + @"\Reports\Report2.rdlc";
                this.reportViewer2.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                MySqlDataAdapter da = new MySqlDataAdapter();

                cn.Open();
                if (f.cboCashier.Text == "All Cashiers")
                {
                    cm = new MySqlCommand("SELECT c.id, c.transno, c.pcode, p.pdesc, c.price, c.qty, c.disc AS discount, c.total FROM tbl_cart AS c INNER JOIN tbl_products AS p ON c.pcode = p.pcode WHERE status = 'Sold' AND DATE(sdate) BETWEEN @date1 AND @date2", cn);
                    cm.Parameters.AddWithValue("@date1", f.dt1.Value.Date);
                    cm.Parameters.AddWithValue("@date2", f.dt2.Value.Date);
                    da.SelectCommand = cm;
                }
                else
                {
                    cm = new MySqlCommand("SELECT c.id, c.transno, c.pcode, p.pdesc, c.price, c.qty, c.disc AS discount, c.total FROM tbl_cart AS c INNER JOIN tbl_products AS p ON c.pcode = p.pcode WHERE status = 'Sold' AND DATE(sdate) BETWEEN @date1 AND @date2 AND cashier = @cashier", cn);
                    cm.Parameters.AddWithValue("@date1", f.dt1.Value.Date);
                    cm.Parameters.AddWithValue("@date2", f.dt2.Value.Date);
                    cm.Parameters.AddWithValue("@cashier", f.cboCashier.Text);
                    da.SelectCommand = cm;
                }
                da.Fill(ds.Tables["dtSoldReport"]);
                cn.Close();

                ReportParameter pDate = new ReportParameter("pDate", "Date From: " + f.dt1.Value.ToShortDateString() + " To " + f.dt2.Value.ToShortDateString());
                ReportParameter pCashier = new ReportParameter("pCashier", "Cashier: " + f.cboCashier.Text);
                ReportParameter pHeader = new ReportParameter("pHeader","SALES REPORT");

                reportViewer2.LocalReport.SetParameters(pDate);
                reportViewer2.LocalReport.SetParameters(pCashier);
                reportViewer2.LocalReport.SetParameters(pHeader);

                rptDS = new ReportDataSource("DataSet1", ds.Tables["dtSoldReport"]);
                reportViewer2.LocalReport.DataSources.Add(rptDS);
                reportViewer2.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer2.ZoomMode = ZoomMode.Percent;
                reportViewer2.ZoomPercent = 50;
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
