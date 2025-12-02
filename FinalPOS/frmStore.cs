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
        private bool isNewStore = false;
        
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
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
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
                    isNewStore = false;
                    btnDelete.Enabled = true;
                }
            }
        }

        private void cboStore_TextChanged(object sender, EventArgs e)
        {
            string enteredText = cboStore.Text.Trim();
            if (!string.IsNullOrEmpty(enteredText))
            {
                // Check if the entered text exists in the store list
                if (storeAddressMap.ContainsKey(enteredText))
                {
                    txtAddress.Text = storeAddressMap[enteredText];
                    isNewStore = false;
                    btnDelete.Enabled = true;
                }
                else
                {
                    // New store - clear address and disable delete
                    txtAddress.Clear();
                    isNewStore = true;
                    btnDelete.Enabled = false;
                }
            }
            else
            {
                txtAddress.Clear();
                isNewStore = false;
                btnDelete.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string storeName = cboStore.Text.Trim();
                
                if (string.IsNullOrWhiteSpace(storeName))
                {
                    MessageBox.Show("Please enter store name", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtAddress.Text))
                {
                    MessageBox.Show("Please enter store address", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                string message = isNewStore ? "Add new store?" : "Update store details?";
                string title = isNewStore ? "Add Store" : "Update Store";
                
                if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string address = txtAddress.Text.Trim();
                    
                    cn.Open();
                    
                    if (isNewStore)
                    {
                        // INSERT new store
                        cm = new MySqlCommand("INSERT INTO tbl_store (store, address) VALUES (@store, @address)", cn);
                        cm.Parameters.AddWithValue("@store", storeName);
                        cm.Parameters.AddWithValue("@address", address);
                        cm.ExecuteNonQuery();
                        
                        MessageBox.Show("Store Added Successfully", "SAVED RECORD", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // UPDATE existing store
                        cm = new MySqlCommand("UPDATE tbl_store SET address = @address WHERE store = @store", cn);
                        cm.Parameters.AddWithValue("@store", storeName);
                        cm.Parameters.AddWithValue("@address", address);
                        cm.ExecuteNonQuery();
                        
                        MessageBox.Show("Store Details Updated Successfully", "UPDATED RECORD", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                    cn.Close();
                    
                    // Reload records to refresh the list
                    LoadRecords();
                    
                    // Select the current store
                    if (cboStore.Items.Contains(storeName))
                    {
                        cboStore.SelectedItem = storeName;
                    }
                    else
                    {
                        cboStore.Text = storeName;
                    }
                }
            }
            catch(Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void frmStore_Load(object sender, EventArgs e)
        {
            LoadRecords();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string storeName = cboStore.Text.Trim();
                
                if (string.IsNullOrWhiteSpace(storeName))
                {
                    MessageBox.Show("Please select a store to delete", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (!storeAddressMap.ContainsKey(storeName))
                {
                    MessageBox.Show("Store not found in database", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (MessageBox.Show("Are you sure you want to delete this store?\n\nStore: " + storeName, "Delete Store", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tbl_store WHERE store = @store", cn);
                    cm.Parameters.AddWithValue("@store", storeName);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    
                    MessageBox.Show("Store Deleted Successfully", "DELETED RECORD", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Reload records
                    LoadRecords();
                    
                    // Clear form
                    Clear();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void Clear()
        {
            cboStore.Text = "";
            txtAddress.Clear();
            isNewStore = false;
            btnDelete.Enabled = false;
            cboStore.Focus();
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
