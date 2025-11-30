using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Common;
using Tulpep.NotificationWindow;

namespace FinalPOS
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;
        public string _pass , _username;   

        public Form1()
        {

            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            NotifyCriticalItems();
            MyDashboard();
            // cn.Open();

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
            while(dr.Read())
            {
                i++;
                critical += i+ ".  "  + dr["pdesc"].ToString() + Environment.NewLine;
            }
            dr.Close();
            cn.Close();

            PopupNotifier popup = new PopupNotifier();
            popup.Image = Properties.Resources.cancel__2_;
            popup.TitleText = count + "CRITICAL ITEM(S)";
            popup.ContentText = critical;
            popup.Popup();
        }



        private void StocksButton_Click(object sender, EventArgs e)
        {
            frmStockIn frm = new frmStockIn();
            frm.TopLevel = false;
            panel3.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        private void POSButton_Click(object sender, EventArgs e)
        {
            //frmPOS frm = new frmPOS();
            //frm.ShowDialog();
        }

        private void ManageBrandButton_Click(object sender, EventArgs e)
        {
            frmBrandList frm = new frmBrandList();
            frm.TopLevel = false;
            panel3.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        private void UserSettingsButton_Click(object sender, EventArgs e)
        {
            frmUserAccounts frm = new frmUserAccounts(this);
            frm.TopLevel = false;
            panel3.Controls.Add(frm);
            frm.txtUsername.Text = _username;
            frm.BringToFront();
            frm.Show();
        }

        private void RecordsButton_Click(object sender, EventArgs e)
        {
            frmRecords frm = new frmRecords();
            frm.TopLevel = false;
            frm.LoadCriticalItems();
            frm.CancelledOrders();
            frm.LoadStockInHistory();
            frm.LoadInventory();
            panel3.Controls.Add(frm);
            frm.BringToFront();
            frm.LoadRecords();
            frm.Show();
        }

        private void ManageProductsButton_Click(object sender, EventArgs e)
        {
            frmProductList frm = new frmProductList();
            frm.TopLevel = false;
            panel3.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        private void ManageCategoryButton_Click(object sender, EventArgs e)
        {
            frmCateogoryList frm = new frmCateogoryList();
            frm.TopLevel = false;
            panel3.Controls.Add(frm);
            frm.BringToFront();
            frm.LoadCategory();
            frm.Show();
        }

        private void SystemSettingButton_Click(object sender, EventArgs e)
        {
            frmStore frm = new frmStore();
            frm.LoadRecords();
            frm.ShowDialog();           
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Logoutbtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
            frmSecurity frm = new frmSecurity();
            frm.Show();
        }

        private void btnSaleHistory_Click(object sender, EventArgs e)
        {
            frmSoldItems frm = new frmSoldItems();
            frm.ShowDialog();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DashboardButton_Click(object sender, EventArgs e)
        {
            MyDashboard();
        }

        public void MyDashboard()
        {
            frmDashboard frm = new frmDashboard();
            frm.TopLevel = false;
            panel3.Controls.Add(frm);
            frm.lblDailySales.Text = dbcon.DailySales().ToString("#,##0.00");
            frm.lblProductLine.Text = dbcon.ProductLine().ToString("#,##0");
            frm.lblStockOnHand.Text = dbcon.StockOnHand().ToString("#,##0");
            frm.lblCriticalItems.Text = dbcon.Critical().ToString("#,##0");
            frm.BringToFront();
            frm.Show();
        }

        private void btnVendor_Click(object sender, EventArgs e)
        {
            frmVendorList frm = new frmVendorList();
            frm.TopLevel = false;
            panel3.Controls.Add(frm);
            frm.BringToFront();
            frm.LoadRecords();
            frm.Show();
        }

        private void btnStockAdjustment_Click(object sender, EventArgs e)
        {
            frmAdjustment frm = new frmAdjustment(this);
            frm.LoadRecords();
            frm.txtUser.Text = lblName.Text;
            frm.RefrenceNo();
            frm.ShowDialog();
        }
    }
}