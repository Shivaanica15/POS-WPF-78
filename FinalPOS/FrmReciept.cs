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
using Microsoft.Reporting.WinForms;

namespace FinalPOS
{
    public partial class FrmReciept : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;
        frmPOS f;
        string store = "Hello Wold Solutions";
        string address = "Light House Gari Khata, Karachi";
        public FrmReciept(frmPOS frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            f = frm;
            this.KeyPreview = true;
        }

 
        public void LoadReport(string pcash, string pchange)
        {
            
            try
            {
                this.reportViewer1.LocalReport.DataSources.Clear();
                
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\Report1.rdlc";
                ReportDataSource rptDatasource;

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
              
                cn.Open();
                da.SelectCommand = new SqlCommand("select c.id, c.transno, c.pcode , c.price, c.qty, c.disc, c.total, c.sdate, status, p.pdesc from tbl_Cart as c inner join tbl_Products as p on p.pcode = c.pcode where transno like '"+ f.lblTransno.Text + "' ", cn);
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

                reportViewer1.LocalReport.SetParameters(pVatable);
                reportViewer1.LocalReport.SetParameters(pVat);
                reportViewer1.LocalReport.SetParameters(pDiscount);
                reportViewer1.LocalReport.SetParameters(pTotal);
                reportViewer1.LocalReport.SetParameters(pCash);
                reportViewer1.LocalReport.SetParameters(pChange);
                reportViewer1.LocalReport.SetParameters(pStore);
                reportViewer1.LocalReport.SetParameters(pAddress);
                reportViewer1.LocalReport.SetParameters(pTransaction);
                reportViewer1.LocalReport.SetParameters(pCashier);

                rptDatasource = new ReportDataSource("DataSet1",ds.Tables["dtSold"]);
                reportViewer1.LocalReport.DataSources.Add(rptDatasource);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
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
