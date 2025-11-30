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
using System.Windows.Forms.DataVisualization.Charting;
namespace FinalPOS
{
    public partial class frmDashboard : Form
    {
        SqlConnection cn;
        SqlCommand cm;
        DBConnection db = new DBConnection();
        public frmDashboard()
        {
            InitializeComponent();
            cn = new SqlConnection();
            cn.ConnectionString = db.MyConnection();
            LoadChart();
        }

        private void frmDashboard_Resize(object sender, EventArgs e)
        {
            panel1.Left = (this.Width - panel1.Width) / 2;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        public void LoadChart()
        {
            cn.Open();
            SqlDataAdapter da = new SqlDataAdapter("select Year(sdate) as year, isnull(sum(total),0.0) as total from tbl_Cart where status like 'Sold' group by Year(sdate) ",cn);
            DataSet ds = new DataSet();

            da.Fill(ds, "Sales");
            chart1.DataSource = ds.Tables["Sales"];
            Series series1 = chart1.Series["Series1"];
            series1.ChartType = SeriesChartType.Doughnut;

            series1.Name = "SALES";

            var chart = chart1;
            chart.Series[series1.Name].XValueMember = "year";
            chart.Series[series1.Name].YValueMembers = "total";
            chart.Series[0].IsValueShownAsLabel = true;
            //chart.Series[0].LegendText = "#";

          
            cn.Close();
        }
    }
}
