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
    public partial class frmStore : Form
    {
        MySqlConnection cn = new MySqlConnection();
        MySqlCommand cm = new MySqlCommand();
        DBConnection dbcon = new DBConnection();
        MySqlDataReader dr;
        private Dictionary<string, string> storeAddressMap = new Dictionary<string, string>();
        
        public frmStore()
        {
            InitializeComponent();
            cn = new MySqlConnection(dbcon.MyConnection());
            this.KeyPreview = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadRecords()
        {
            try
            {
                cboStore.Items.Clear();
                storeAddressMap.Clear();
                
                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tbl_store ORDER BY store", cn);
                dr = cm.ExecuteReader();
                
                while (dr.Read())
                {
                    string storeName = dr["store"].ToString();
                    string address = dr["address"].ToString();
                    
                    cboStore.Items.Add(storeName);
                    storeAddressMap[storeName] = address;
                }
                dr.Close();
                cn.Close();
                
                // Select first item if available
                if (cboStore.Items.Count > 0)
                {
                    cboStore.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void cboStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboStore.SelectedItem != null)
            {
                string selectedStore = cboStore.SelectedItem.ToString();
                if (storeAddressMap.ContainsKey(selectedStore))
                {
                    txtAddress.Text = storeAddressMap[selectedStore];
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboStore.SelectedItem == null)
                {
                    MessageBox.Show("Please select a store", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtAddress.Text))
                {
                    MessageBox.Show("Please enter store address", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (MessageBox.Show("Save Store Details", "Store Details", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
                {
                    string selectedStore = cboStore.SelectedItem.ToString();
                    string newAddress = txtAddress.Text.Trim();
                    
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tbl_store SET address = @address WHERE store = @store", cn);
                    cm.Parameters.AddWithValue("@store", selectedStore);
                    cm.Parameters.AddWithValue("@address", newAddress);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    
                    // Update the dictionary
                    storeAddressMap[selectedStore] = newAddress;
                    
                    MessageBox.Show("Store Details Saved Successfully", "SAVED RECORD ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "WARNING" ,MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void frmStore_Load(object sender, EventArgs e)
        {
            LoadRecords();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void frmStore_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
