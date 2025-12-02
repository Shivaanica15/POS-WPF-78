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
using Microsoft.Reporting.WinForms;

namespace FinalPOS
{
    public partial class FrmReciept : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
        frmPOS f;
        string store = "Bohar Solutions";
        string address = "Amman Road, Kantharmadam, Jaffna";
        public FrmReciept(frmPOS frm)
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
            f = frm;
            this.KeyPreview = true;
        }

 
        public void LoadReport(string pcash, string pchange)
        {
            
            try
            {
                this.reportViewer1.Reset();
                this.reportViewer1.LocalReport.DataSources.Clear();
                
                // Use embedded resource - make sure ReportPath is not set when using embedded resource
                this.reportViewer1.LocalReport.ReportPath = null;
                this.reportViewer1.LocalReport.ReportEmbeddedResource = "FinalPOS.Report1.rdlc";
                ReportDataSource rptDatasource;

                DataSet1 ds = new DataSet1();
                MySqlDataAdapter da = new MySqlDataAdapter();
              
                cn.Open();
                cm = new MySqlCommand("SELECT c.id, c.transno, c.pcode, c.price, c.qty, c.disc, c.total, c.sdate, status, p.pdesc FROM tbl_cart AS c INNER JOIN tbl_products AS p ON p.pcode = c.pcode WHERE transno = @transno", cn);
                cm.Parameters.AddWithValue("@transno", f.lblTransno.Text);
                da.SelectCommand = cm;
                da.Fill(ds.Tables["dtSold"]);
                cn.Close();

                ReportParameter pVatable = new ReportParameter("pVatable", f.lblVatable.Text);
                ReportParameter pVat = new ReportParameter("pVat",f.lblVAT.Text);
                ReportParameter pDiscount = new ReportParameter("pDiscount",f.lblDiscount.Text);
                ReportParameter pTotal = new ReportParameter("pTotal",f.lblDisplayTotal.Text);
                ReportParameter pCash = new ReportParameter("pCash", pcash);
                ReportParameter pChange = new ReportParameter("pChange", pchange);
                ReportParameter pStore = new ReportParameter("pStore", store);
                ReportParameter pAddress = new ReportParameter("pAddress", address);
                ReportParameter pTransaction = new ReportParameter("pTransaction","Invoice #:" + f.lblTransno.Text); 
                ReportParameter pCashier = new ReportParameter("pCashier", f.lblUser.Text);

                List<ReportParameter> parameters = new List<ReportParameter>
                {
                    pVatable, pVat, pDiscount, pTotal, pCash, pChange, pStore, pAddress, pTransaction, pCashier
                };
                reportViewer1.LocalReport.SetParameters(parameters);

                rptDatasource = new ReportDataSource("DataSet1",ds.Tables["dtSold"]);
                reportViewer1.LocalReport.DataSources.Add(rptDatasource);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("Error loading report: " + ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dtSoldBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void FrmReciept_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }

        private void FrmReciept_Load(object sender, EventArgs e)
        {
            this.reportViewer1.RefreshReport();
          
        }
    }
}
